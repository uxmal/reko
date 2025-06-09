#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis;

/// <summary>
/// Locates instances of add aLo, bLow followed later adc aHi, bHi and
/// merges them into (add a, b).
/// </summary>
/// <remarks>
/// Limitations: only does this on pairs within the same basic block,
/// as dominator analysis and SSA analysis haven't been done this early. 
/// //$TODO: consider doing this _after_ SSA, so that we reap the benefit
/// of performing this across basic block boundaries. The challenge is
/// to introduce new variables xx_yy that interfere with existing xx 
/// and yy references.
/// This code must be run immediately after SSA translation. In particular
/// it must happen before value propagation since VP changes 
/// <code>
/// adc r1,0,C
/// </code>
/// to
/// <code>
/// add r1,C
/// </code>
/// </remarks>
public class LongAddRewriter : IAnalysis<SsaState>
{
    private static readonly TraceSwitch trace = new(nameof(LongAddRewriter), "Trace LongAddRewriter operations") { Level = TraceLevel.Warning };
    private static readonly InstructionMatcher adcPattern;
    private static readonly InstructionMatcher addPattern;
    private static readonly ExpressionMatcher memOffset;
    private static readonly ExpressionMatcher segMemOffset;
    private static readonly InstructionMatcher condm;

    private readonly AnalysisContext context;

    public LongAddRewriter(AnalysisContext context)
    {
        this.context = context;
    }

    public string Id => "larw";

    public string Description => "Rewrites ADD/ADC and SUB/SUBC sequences to long adds/subs";

    public (SsaState, bool) Transform(SsaState ssa)
    {
        var w = CreateWorker(ssa);
        w.Transform();
        return (ssa, w.Changed);
    }

    public Worker CreateWorker(SsaState ssa)
    {
        return new Worker(ssa, context);
    }

    public class Worker
    {
        private readonly SsaState ssa;
        private readonly SsaMutator ssam;
        private readonly IProcessorArchitecture arch;
        private readonly IEventListener listener;
        private Expression? dst;

        public Worker(SsaState ssa, AnalysisContext context)
        {
            this.ssa = ssa;
            this.ssam = new SsaMutator(ssa);
            this.arch = ssa.Procedure.Architecture;
            this.listener = context.EventListener;
        }


        public bool Changed { get; private set; }

        public void CreateLongBinaryInstruction(Candidate loCandidate, Candidate hiCandidate)
        {
            var totalSize = PrimitiveType.Create(
                Domain.SignedInt | Domain.UnsignedInt,
                loCandidate.Dst!.DataType.BitSize + hiCandidate.Dst!.DataType.BitSize);
            var left = CreateWideExpression(ssa, loCandidate.Left, hiCandidate.Left, totalSize);
            var right = CreateWideExpression(ssa, loCandidate.Right!, hiCandidate.Right!, totalSize);
            this.dst = CreateWideExpression(ssa, loCandidate.Dst, hiCandidate.Dst, totalSize);
            var stmts = hiCandidate.Statement!.Block.Statements;
            var addr = hiCandidate.Statement.Address;
            var iStm = FindInsertPosition(loCandidate, hiCandidate, stmts);
            Statement? stmMkLeft = null;
            if (left is Identifier)
            {
                stmMkLeft = stmts.Insert(
                    iStm++,
                    addr,
                    CreateMkSeq(left, hiCandidate.Left, loCandidate.Left));
                left = ReplaceDstWithSsaIdentifier(left, stmMkLeft);
            }

            Statement? stmMkRight = null;
            if (right is Identifier)
            {
                stmMkRight = stmts.Insert(
                    iStm++,
                    addr,
                    CreateMkSeq(right, hiCandidate.Right!, loCandidate.Right!));
                right = ReplaceDstWithSsaIdentifier(right, stmMkRight);
            }

            var expSum = new BinaryExpression(loCandidate.Op, left.DataType, left, right);
            Instruction instr = Assign(dst, expSum);
            var stmLong = stmts.Insert(iStm++, addr, instr);
            this.dst = ReplaceDstWithSsaIdentifier(this.dst, stmLong);

            var sidDst = GetSsaIdentifierOf(dst);
            //var sidLeft = GetSsaIdentifierOf(left);
            //var sidRight = GetSsaIdentifierOf(right);
            if (stmMkLeft is not null)
                ssa.AddUses(stmMkLeft);
            if (stmMkRight is not null)
                ssa.AddUses(stmMkRight);
            ssa.AddUses(stmLong);

            var sidDstLo = GetSsaIdentifierOf(loCandidate.Dst);
            if (sidDstLo is not null)
            {
                var cast = new Slice(loCandidate.Dst.DataType, dst, 0);
                var stmCastLo = stmts.Insert(iStm++, addr, new AliasAssignment(
                    sidDstLo.Identifier, cast));
                var stmDeadLo = sidDstLo.DefStatement;
                sidDstLo.DefStatement = stmCastLo;

                var sidDstHi = GetSsaIdentifierOf(hiCandidate.Dst);
                var slice = new Slice(hiCandidate.Dst.DataType, dst, loCandidate.Dst.DataType.BitSize);
                var stmSliceHi = stmts.Insert(iStm++, addr, new AliasAssignment(
                    sidDstHi!.Identifier, slice));
                var stmDeadHi = sidDstHi.DefStatement;
                sidDstHi.DefStatement = stmSliceHi;

                if (sidDstLo is not null)
                {
                    sidDst!.Uses.Add(stmCastLo);
                }
                if (sidDstHi is not null)
                {
                    sidDst!.Uses.Add(stmSliceHi);
                }
                ssa.DeleteStatement(stmDeadLo!);
                ssa.DeleteStatement(stmDeadHi!);
            }
        }

        /// <summary>
        /// Changes the code from:
        /// <code>
        /// 1: hi_1 = -hi
        /// 2: lo_2 = -lo
        /// 3: C_3 = lo_2 == 0
        /// 4: hi_2 = (h1_1 - 0) - C_3
        /// </code>
        /// to:
        /// 1: hi_lo = SEQ(hi,lo)
        ///    hi_lo_4 = -hi_lo
        ///    hi_1 = -hi
        /// 2: lo_2 = SLICE(hi_lo_4)
        /// 3: hi_3 = SLICE(hi_lo_4)
        /// </summary>
        /// <param name="loCandidate"></param>
        /// <param name="hiCandidate"></param>
        private void CreateLongNegationInstruction(Candidate loCandidate, Candidate hiCandidate)
        {
            var stmHi = ssa.Identifiers[(Identifier) hiCandidate.Dst!].DefStatement;
            var stmLo = ssa.Identifiers[(Identifier) loCandidate.Dst!].DefStatement;
            var iStm = Math.Min(
                stmHi.Block.Statements.IndexOf(stmHi),
                stmLo.Block.Statements.IndexOf(stmLo));

            SsaIdentifier AddSsaId(Identifier id) => ssa.Identifiers.Add(id, stmHi, false);

            var totalSize = PrimitiveType.Create(
                Domain.SignedInt | Domain.UnsignedInt,
                loCandidate.Dst!.DataType.BitSize + hiCandidate.Dst!.DataType.BitSize);
            var wideSrc = CreateWideExpression(ssa, loCandidate.Left, hiCandidate.Left, totalSize);
            if (wideSrc is Identifier idWideSrc)
            {
                var sidWideSrc = AddSsaId(idWideSrc);
                wideSrc = sidWideSrc.Identifier;
                sidWideSrc.DefStatement = stmHi.Block.Statements.Insert(
                    iStm++,
                    stmHi.Address,
                    CreateMkSeq(wideSrc, hiCandidate.Left, loCandidate.Left));
                GetSsaIdentifierOf(hiCandidate.Left)?.Uses.Add(sidWideSrc.DefStatement);
                GetSsaIdentifierOf(loCandidate.Left)?.Uses.Add(sidWideSrc.DefStatement);
            }

            var sidWideDst = AddSsaId((Identifier) CreateWideExpression(ssa, loCandidate.Dst, hiCandidate.Dst, totalSize));

            var wideDst = sidWideDst.Identifier;
            sidWideDst.DefStatement = stmHi.Block.Statements.Insert(iStm++, stmHi.Address,
                Assign(wideDst, new UnaryExpression(Operator.Neg, wideSrc.DataType, wideSrc)));
            GetSsaIdentifierOf(wideSrc)?.Uses.Add(sidWideDst.DefStatement);

            var sidLo = GetSsaIdentifierOf(loCandidate.Dst);
            if (sidLo is not null)
            {
                ssa.RemoveUses(stmLo);
                stmLo.Instruction = new Assignment(sidLo.Identifier, new Slice(sidLo.Identifier.DataType, wideDst, 0));
                ssa.AddUses(stmLo);
            }
            var sidHi = GetSsaIdentifierOf(hiCandidate.Dst);
            if (sidHi is not null)
            {
                ssa.RemoveUses(sidHi.DefStatement);
                sidHi.DefStatement.Instruction = new Assignment(sidHi.Identifier, new Slice(sidHi.Identifier.DataType, wideDst, loCandidate.Dst.DataType.BitSize));
                ssa.AddUses(sidHi.DefStatement);
            }
        }

        /// <summary>
        /// Find a statement index appropriate for insert the new
        /// long addition statements.
        /// </summary>
        /// <returns></returns>
        private int FindInsertPosition(Candidate loCandidate, Candidate hiCandidate, StatementList stmts)
        {
            int iStm = stmts.IndexOf(hiCandidate.Statement!);
            if (loCandidate.Dst is Identifier idLow)
            {
                int iFirstLowUsage = ssa.Identifiers[idLow].Uses
                    .Select(u => stmts.IndexOf(u))
                    .Where(i => i >= 0)
                    .Min();
                iStm = Math.Min(iStm, iFirstLowUsage);
            }
            return iStm;
        }

        private Expression ReplaceDstWithSsaIdentifier(Expression dst, Statement stmLong)
        {
            if (stmLong.Instruction is Assignment ass)
            {
                var sid = ssa.Identifiers.Add(ass.Dst, stmLong, false);
                ass.Dst = sid.Identifier;
                return ass.Dst;
            }
            return dst;
        }

        private static Instruction CreateMkSeq(Expression dst, Expression hi, Expression lo)
        {
            return Assign(dst, new MkSequence(dst.DataType, hi, lo));
        }

        private static Instruction Assign(Expression dst, Expression src)
        {
            if (dst is Identifier idDst)
            {
                return new Assignment(idDst, src);
            }
            else
            {
                return new Store(dst, src);
            }
        }

        private SsaIdentifier? GetSsaIdentifierOf(Expression? dst)
        {
            if (dst is Identifier id)
                return ssa.Identifiers[id];
            else if (dst is MkSequence seq)
            {
                //$TODO what if there are many identifiers?
                foreach (var e in seq.Expressions)
                    if (e is Identifier eId)
                        return ssa.Identifiers[eId];
            }
            else if (dst is Conversion conv && conv.Expression is Identifier idConv)
                return ssa.Identifiers[idConv];
            return null;
        }

        public void Transform()
        {
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                if (listener.IsCanceled())
                    return;
                ReplaceLongOperations(block);
            }
        }

        /// <summary>
        /// Look for add/adc or sub/sbc pairs that look like long additions
        /// by scanning the statements from the beginning of the block to the end.
        /// </summary>
        public void ReplaceLongOperations(Block block)
        {
            var stmtsOrig = block.Statements.ToList();
            for (int i = 0; i < block.Statements.Count; ++i)
            {
                if (listener.IsCanceled())
                    return;
                var loInstr = MatchAddSub(block.Statements[i]);
                if (loInstr is not null)
                {
                    var cond = FindConditionOf(stmtsOrig, i, loInstr.Dst!);
                    if (cond is null)
                        continue;

                    var hiInstr = FindUsingAddSub(block, cond.FlagGroup, loInstr);
                    if (hiInstr is null)
                        continue;
                    trace.Verbose("Larw: {0}: found add/sub pair {1} / {2}", block.DisplayName, loInstr.Statement!, hiInstr.Statement!);
                    CreateLongBinaryInstruction(loInstr, hiInstr);
                    Changed = true;
                }
                loInstr = MatchNegation(block.Statements[i]);
                if (loInstr is not null)
                {
                    var hiInstr = FindNegationHighPart(block, stmtsOrig, i, loInstr);
                    if (hiInstr is null)
                        continue;
                    trace.Verbose("Larw: {0}: found neg/sbc {1} / {2}", block.DisplayName, loInstr.Statement!, hiInstr.Statement!);
                    CreateLongNegationInstruction(loInstr, hiInstr);
                    Changed = true;
                }

                var stm = block.Statements[i];

                loInstr = MatchOr(block.Statements[i]);
                if (loInstr is not null)
                {
                    var (hi, lo) = FindHighShift(loInstr);
                    if (hi is null)
                        continue;

                    //trace.Verbose("Larw: {0}: found shift/sbc {1} / {2}", block.DisplayName, loInstr.Statement!, hiInstr.Statement!);
                    CreateLongShiftInstruction(hi, lo!);
                    Changed = true;
                }
            }
        }

        private Candidate? FindNegationHighPart(Block block, List<Statement> stmtsOrig, int i, Candidate loInstr)
        {
            var cond = FindConditionOf(stmtsOrig, i, loInstr.Dst!) ??
                       FindConditionOfNeg(loInstr.Left);
            if (cond is not null)
            {
                var hiInstr = FindUsingNegation(block, cond.FlagGroup, loInstr);
                return hiInstr;
            }
            var hiNegsub = FindUsingNegSub(stmtsOrig, loInstr.Dst);
            if (hiNegsub is not null)
            {
                return hiNegsub;
            }
            return null;
        }

        private (Identifier?, Statement?) FindUse(Identifier id,  Func<Identifier, Statement, Identifier?> predicate)
        {
            var sid = ssa.Identifiers[id];
            foreach (var use in sid.Uses)
            {
                var idDef = predicate(id, use);
                if (idDef is not null)
                    return (idDef, use);
            }
            return default;
        }

        private Candidate? FindUsingNegSub(List<Statement> stmtsOrig, Expression? neg)
        {
            if (neg is not Identifier idNeg)
                return null;
            var (idConv, useConv) = FindUse(idNeg, (id, use) =>
                (use.Instruction is Assignment ass &&
                    ass.Src is Conversion conv &&
                    conv.Expression is BinaryExpression bin &&
                    bin.Operator.Type == OperatorType.Ult &&
                    bin.Right.IsZero &&
                    bin.Left == id)
                    ? ass.Dst : null);
            if (idConv is null)
                return null;
            var (idSub, useSub) = FindUse(idConv, (id, use) =>
                (use.Instruction is Assignment ass &&
                ass.Src is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.ISub &&
                bin.Right == id)
                ? ass.Dst : null);
            if (idSub is null)
                return null;

            // Found the instruction the computes the high part.
            Debug.Assert(useSub is not null);
            var assSub = (Assignment) useSub.Instruction;
            var sub = (BinaryExpression) assSub.Src;
            if (sub.Left is not Identifier idHiNegDst)
                return null;
            var stmHiNeg = ssa.Identifiers[idHiNegDst].DefStatement;
            if (stmHiNeg is null || stmHiNeg.Instruction is not Assignment assHiNeg ||
                assHiNeg.Src is not UnaryExpression hiNeg ||
                hiNeg.Operator.Type != OperatorType.Neg)
                return null;
            return new Candidate(Operator.ISub, hiNeg.Expression, null)
            {
                Dst = idSub,
            };
        }

        /// <summary>
        /// Determines if the carry flag reaches a using instruction, and 
        /// if that using instruction is an ADC or SBC type instruction.
        /// </summary>
        /// <param name="block">The block in which the ADD was found. The ADC
        /// must be in the same block.</param>
        /// <param name="cy">The identifier that tracks the carry flag.</param>
        /// <param name="loInstr">The candidate instruction for the low half
        /// of the addition.</param>
        /// <returns>The MSB part of the ADD/ADC - SUB/SBC pair if successful,
        /// otherwise null.</returns>
        public Candidate? FindUsingAddSub(
            Block block,
            Identifier cy,
            Candidate loInstr)
        {
            var queue = new Queue<Statement>(ssa.Identifiers[cy].Uses);
            while (queue.TryDequeue(out var use))
            {
                if (use.Instruction is not Assignment ass)
                    continue;

                if (ConditionCodeEliminator.FindSlicedFlagRegister(ass.Src) is not null)
                {
                    // This is a "slice" of a flag group, continue.
                    queue.EnqueueRange(ssa.Identifiers[ass.Dst].Uses);
                    continue;
                }
                var asc = MatchAdcSbc(use);
                if (asc is not null && asc.Statement is not null && asc.Statement.Block == block)
                {
                    if (asc.Op != loInstr.Op)
                        return null;
                    if (asc.Left.GetType() != loInstr.Left.GetType())
                        return null;
                    asc.Statement = use;
                    return asc;
                }
                if (IsCarryFlag(ass.Dst))
                    return null;
            }
            return null;
        }

        public bool IsCarryFlag(Expression? exp)
        {
            return
                exp is Identifier cf &&
                cf.Storage.Equals(arch.CarryFlag);
        }

        public Candidate? FindUsingNegation(
            Block block,
            Identifier cy,
            Candidate loInstr)
        {
            var queue = new Queue<Statement>(ssa.Identifiers[cy].Uses);
            while (queue.TryDequeue(out var use))
            {
                if (use.Instruction is not Assignment ass)
                    continue;

                if (ConditionCodeEliminator.FindSlicedFlagRegister(ass.Src) is not null)
                {
                    // This is a "slice" of a flag group, continue.
                    queue.EnqueueRange(ssa.Identifiers[ass.Dst].Uses);
                    continue;
                }

                if (ass.Src is BinaryExpression bin && 
                    bin.Operator == Operator.ISub &&
                    bin.Left is UnaryExpression subu &&
                    subu.Operator == Operator.Neg && 
                    subu.Expression is Identifier idSubu &&
                    IsCarryFlag(bin.Right))
                {
                    return new Candidate(bin.Operator, subu.Expression, idSubu)
                    {
                        Dst = ass.Dst
                    };
                }
                var asc = MatchAdcSbc(use);
                if (asc is null)
                    continue;
                if (asc.Op == Operator.ISub &&
                    asc.Right!.IsZero &&
                    asc.Left is Identifier idNegatedHi)
                {
                    var sidNegatedHi = ssa.Identifiers[idNegatedHi];
                    var def = sidNegatedHi.GetDefiningExpression();
                    if (def is UnaryExpression u && u.Operator == Operator.Neg)
                    {
                        return new Candidate(asc.Op, u.Expression, asc.Dst)
                        {
                            Dst = asc.Dst,
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the subsequent statement in this block that defines a condition code based on the
        /// result in expression <paramref name="exp"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ax"></param>
        /// <returns></returns>
        public CondMatch? FindConditionOf(List<Statement> stms, int iStm, Expression exp)
        {
            if (exp is Identifier idLo)
            {
                foreach (var use in ssa.Identifiers[idLo].Uses)
                {
                    var m = condm.Match(use.Instruction);
                    if (!m.Success)
                        continue;
                    var grf = (Identifier) m.CapturedExpression("grf")!;
                    //var condExp = m.CapturedExpression("exp");
                    if (grf.Storage is FlagGroupStorage) // && exp == condExp)
                    {
                        return new CondMatch(grf, exp, use, 0);
                    }
                }
            }
            for (int i = iStm + 1; i < stms.Count; ++i)
            {
                var m = condm.Match(stms[i].Instruction);
                if (!m.Success)
                    continue;
                var grf = (Identifier) m.CapturedExpression("grf")!;
                var condExp = m.CapturedExpression("exp");
                if (grf.Storage is FlagGroupStorage && exp == condExp)
                {
                    return new CondMatch(grf, exp, null!, i);
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the statement that defines the condition code for a negation.
        /// Some architectures check for zeroness before the negation.
        /// result in expression <paramref name="exp"/>.
        /// </summary>
        public CondMatch? FindConditionOfNeg(Expression? exp)
        {
            if (exp is Identifier idLo)
            {
                foreach (var use in ssa.Identifiers[idLo].Uses)
                {
                    var grf = IsGrfSetToNe0(use.Instruction);
                    if (grf is null)
                    {
                        var m = condm.Match(use.Instruction);
                        if (!m.Success)
                            continue;
                        grf = (Identifier) m.CapturedExpression("grf")!;
                    }
                    //var condExp = m.CapturedExpression("exp");
                    if (grf.Storage is FlagGroupStorage) // && exp == condExp)
                    {
                        return new CondMatch(grf, exp, use, 0);
                    }
                }
            }
            return null;
        }

        private Identifier? IsGrfSetToNe0(Instruction instruction)
        {
            if (instruction is Assignment ass &&
                ass.Dst.Storage is FlagGroupStorage &&
                ass.Src is BinaryExpression cmp &&
                cmp.Operator == Operator.Ne &&
                cmp.Right.IsZero)
            {
                return ass.Dst;
            }
            return null;
        }

        private Expression CreateWideExpression(SsaState ssa, Expression expLo, Expression expHi, DataType totalSize)
        {
            if (expLo is Identifier idLo && expHi is Identifier idHi)
            {
                // If the high part is a zeroed identifier, we are dealing with 
                // a zero-extended value.
                if (ssa.Identifiers[idHi].DefStatement?.Instruction is Assignment ass &&
                    ass.Src.IsZero)
                {
                    var dt = PrimitiveType.Create(Domain.UnsignedInt, totalSize.BitSize);
                    return new Conversion(idLo, idLo.DataType, dt);
                }
                return ssa.Procedure.Frame.EnsureSequence(totalSize, idHi.Storage, idLo.Storage);
            }
            if (expLo is MemoryAccess memDstLo && expHi is MemoryAccess memDstHi &&
                MemoryOperandsAdjacent(ssa.Procedure.Architecture, memDstLo, memDstHi))
            {
                return CreateMemoryAccess(memDstLo, totalSize);
            }
            if (expLo is Constant immLo && expHi is Constant immHi)
            {
                return Constant.Create(totalSize, (immHi.ToUInt64() << expLo.DataType.BitSize) | immLo.ToUInt32());
            }
            return new MkSequence(totalSize, expHi, expLo);
        }

        private static Expression CreateMemoryAccess(MemoryAccess mem, DataType totalSize)
        {
            return new MemoryAccess(mem.MemoryId, mem.EffectiveAddress, totalSize);
        }

        public class CondMatch
        {
            public CondMatch(
                Identifier grf,
                Expression src,
                Statement stm,
                int index)
            {
                this.FlagGroup = grf;
                this.src = src;
                this.Statement = stm;
                this.StatementIndex = index;
            }

            public readonly Identifier FlagGroup;
            public readonly Expression src;
            public readonly Statement Statement;
            public readonly int StatementIndex;
        }

        public static bool MemoryOperandsAdjacent(IProcessorArchitecture arch, MemoryAccess m1, MemoryAccess m2)
        {
            var off1 = GetOffset(m1);
            var off2 = GetOffset(m2);
            if (off1 is null || off2 is null)
                return false;
            return arch.Endianness.OffsetsAdjacent(off1.ToInt64(), off2.ToInt64(), m1.DataType.Size);
        }

        private static Constant? GetOffset(MemoryAccess access)
        {
            var match = memOffset.Match(access);
            if (match.Success)
            {
                return (Constant) match.CapturedExpression("Offset")!;
            }
            match = segMemOffset.Match(access);
            if (match.Success)
            {
                return (Constant) match.CapturedExpression("Offset")!;
            }
            if (access.EffectiveAddress is Constant c)
                return c;
            return null;
        }

        /// <summary>
        /// Matches an "ADC" or "SBB/SBC" pattern.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>If the match succeeded, returns a partial BinaryExpression
        /// with the left and right side of the ADC/SBC instruction.</returns>
        public Candidate? MatchAdcSbc(Statement stm)
        {
            var m = adcPattern.Match(stm.Instruction);
            if (m.Success)
            {
                if (!IsCarryFlag(m.CapturedExpression("cf")))
                    return null;
                var op = m.CapturedOperator("op2");
                if (op is not BinaryOperator binop || !binop.Type.IsAddOrSub())
                    return null;
                return new Candidate(
                    binop,
                    m.CapturedExpression("left")!,
                    m.CapturedExpression("right")!)
                {
                    Dst = m.CapturedExpression("dst")!,
                    Statement = stm
                };
            }
            m = addPattern.Match(stm.Instruction);
            if (m.Success)
            {
                if (!IsCarryFlag(m.CapturedExpression("right")))
                    return null;
                var op = m.CapturedOperator("op") as BinaryOperator;
                if (op is not BinaryOperator binOp || !binOp.Type.IsAddOrSub())
                    return null;
                var dst = m.CapturedExpression("dst")!;
                var left = m.CapturedExpression("left")!;
                Candidate? pdp11dst = IsPdp11StyleAdcSbc(binOp, dst, left);
                if (pdp11dst is not null)
                    return pdp11dst;
                return new Candidate(
                    binOp,
                    left,
                    Constant.Zero(left.DataType))
                {
                    Dst = dst,
                    Statement = stm
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks to see if this is a PDP-11 style long add.
        /// </summary>
        /// <param name="op">IAdd or ISub</param>
        /// <param name="dst">Possible intermediate step</param>
        /// <param name="left">intermediate addend.</param>
        /// <returns></returns>
        /// <remarks>
        /// The PDP-11 long add sequence looks like this:
        ///     add r1,r2
        ///     adc r3
        ///     add r3,r4
        /// </remarks>
        private Candidate? IsPdp11StyleAdcSbc(BinaryOperator op, Expression dst, Expression left)
        {
            if (dst is not Identifier idIntermediate)
                return null;
            var uses = ssa.Identifiers[idIntermediate].Uses;
            foreach (var use in uses)
            {
                var m = addPattern.Match(use.Instruction);
                if (m.Success)
                {
                    var dstFinal = m.CapturedExpression("dst");
                    var opFinal = (BinaryOperator?) m.CapturedOperator("op");
                    var leftFinal = m.CapturedExpression("left");
                    var rightFinal = m.CapturedExpression("right")!;
                    if (leftFinal == idIntermediate && op == opFinal)
                    {
                        return new Candidate(op, left, rightFinal)
                        {
                            Dst = dstFinal,
                            Statement = use
                        };
                    }
                }
            }
            return null;
        }

        public Candidate? MatchAddSub(Statement stm)
        {
            var m = addPattern.Match(stm.Instruction);
            if (!m.Success)
                return null;
            var op = (BinaryOperator) m.CapturedOperator("op")!;
            if (!op.Type.IsAddOrSub())
                return null;
            return new Candidate(
                (BinaryOperator) op,
                m.CapturedExpression("left")!,
                m.CapturedExpression("right")!)
            {
                Dst = m.CapturedExpression("dst")!,
            };
        }

        private Candidate? MatchNegation(Statement stm)
        {
            if (stm.Instruction is Assignment ass &&
                ass.Src is UnaryExpression u &&
                u.Operator == Operator.Neg)
            {
                return new Candidate(Operator.ISub, u.Expression, null)
                {
                    Dst = ass.Dst
                };
            }
            return null;
        }

        private Expression? GetExp(Expression exp)
        {
            while (exp is Identifier id)
            {
                var sid = ssa.Identifiers[id];
                var def = sid.GetDefiningExpression();
                if (def is null)
                    return exp;
                if (def is BinaryExpression b)
                    return b;
                if (def is not Identifier idDef)
                    return id;
                exp = def;
            }
            return exp;
        }

        private bool IsShl(Expression exp)
        {
            return exp is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.Shl;
        }

        private bool IsShr(Expression exp)
        {
            return exp is BinaryExpression bin &&
                (bin.Operator.Type == OperatorType.Shr ||
                 bin.Operator.Type == OperatorType.Sar);
        }


        private Candidate? MatchOr(Statement stm)
        {
            if (stm.Instruction is Assignment ass &&
                ass.Src is BinaryExpression bin &&
                bin.Operator == Operator.Or)
            {
                var left = GetExp(bin.Left);
                var right = GetExp(bin.Right);
                if (left is null || right is null)
                    return null;
                if (IsShr(left) && IsShl(right) ||
                    IsShr(right) && IsShl(left))
                {
                    return new Candidate(bin.Operator, left, right)
                    {
                        Dst = ass.Dst,
                        Statement = stm,
                    };
                }
            }
            return null;
        }


        private (
            Candidate? hi,
            Candidate? lo)
            FindHighShift(Candidate orInstr)
        {
            var (op1, exp1, sh1, shConst1) = UnpackShift(orInstr.Left);
            var (op2, exp2, sh2, shConst2) = UnpackShift(orInstr.Right!);
            if (exp1 is null || exp2 is null)
                return default;
            if (op1 is null || op2 is null)
                return default;
            if (shConst1 is not null)
            {
                if (exp1 is not Identifier idRight)
                    return default;
                var (sidOtherDst, otherSrc) = FindOtherShiftUse(op1.Type == OperatorType.Shl, idRight);
                if (sidOtherDst is null)
                    return default;
                Debug.Assert(otherSrc is not null);
                var hiCandidate = new Candidate(op2, exp2, sh2)
                {
                    Dst = orInstr.Dst,
                    Statement = orInstr.Statement,
                };
                var loCandidate = new Candidate(op1, otherSrc, sh1)
                {
                    Dst = sidOtherDst.Identifier,
                    Statement = sidOtherDst.DefStatement,
                };
                if (op1.Type == OperatorType.Shl)
                {
                    return (loCandidate, hiCandidate);
                }
                else
                {
                    return (hiCandidate, loCandidate);
                }
            }
            else if (shConst2 is not null)
            {
                if (exp2 is not Identifier idRight)
                    return default;
                var (sidOtherDst, otherSrc) = FindOtherShiftUse(op1.Type == OperatorType.Shl, idRight);
                if (sidOtherDst is null)
                    return default;
                Debug.Assert(otherSrc is not null);
                var hiCandidate = new Candidate(op1, exp1, sh1)
                {
                    Dst = orInstr.Dst,
                    Statement = orInstr.Statement,
                };
                var loCandidate = new Candidate(op1, otherSrc, sh1)
                {
                    Dst = sidOtherDst.Identifier,
                    Statement = sidOtherDst.DefStatement,
                };
                if (op1.Type == OperatorType.Shl)
                {
                    return (hiCandidate, loCandidate);
                }
                else
                {
                    return (loCandidate, hiCandidate);
                }
            }
            return default;
        }

        private (SsaIdentifier?, Expression?) FindOtherShiftUse(bool isLeftShift, Identifier idRight)
        {
            if (GetExp(idRight) is not Identifier id)
                return default;
            var sid = ssa.Identifiers[id];
            foreach (var use in sid.Uses)
            {
                if (use.Instruction is Assignment ass &&
                    ass.Src is BinaryExpression bin &&
                    ((isLeftShift && bin.Operator.Type == OperatorType.Shl) ||
                     (!isLeftShift && (bin.Operator.Type == OperatorType.Shr ||
                                       bin.Operator.Type == OperatorType.Sar))))
                {
                    return (GetSsaIdentifierOf(ass.Dst), bin.Left);
                }
            }
            return default;
        }

        private (BinaryOperator? opType, Expression? left, Expression? shAmt, Constant? c) UnpackShift(Expression e)
        {
            var bin = (BinaryExpression) e;
            var shAmt = GetExp(bin.Right);
            Constant? c = null;
            if (shAmt is BinaryExpression binShift)
            {
                if (binShift.Right is Constant cc &&
                    cc.ToInt32() == e.DataType.BitSize)
                {
                    c = cc;
                    shAmt = binShift.Left;
                }
                else if (binShift.Operator.Type == OperatorType.ISub &&
                        binShift.Left is Constant ccSub &&
                        ccSub.ToInt32() == e.DataType.BitSize)
                {
                    c = ccSub;
                    shAmt = binShift.Right;
                }
                else
                    return default;
            }
            return (bin.Operator, bin.Left, shAmt, c);
        }

        private void CreateLongShiftInstruction(Candidate hiCandidate, Candidate loCandidate)
        {
            var totalSize = PrimitiveType.Create(
                Domain.SignedInt | Domain.UnsignedInt,
                hiCandidate.Dst!.DataType.BitSize + loCandidate.Dst!.DataType.BitSize);
            var left = CreateWideExpression(ssa, loCandidate.Left, hiCandidate.Left, totalSize);
            var shift = hiCandidate.Right!;
            this.dst = CreateWideExpression(ssa, loCandidate.Dst, hiCandidate.Dst, totalSize);
            var stmts = hiCandidate.Statement!.Block.Statements;
            var addr = hiCandidate.Statement.Address;
            var iStm = FindLongShiftInsertPosition(loCandidate, hiCandidate, stmts);
            Statement? stmMkLeft = null;
            if (left is Identifier)
            {
                stmMkLeft = stmts.Insert(
                    iStm++,
                    addr,
                    CreateMkSeq(left, hiCandidate.Left, loCandidate.Left));
                left = ReplaceDstWithSsaIdentifier(left, stmMkLeft);
            }

            var expSum = new BinaryExpression(loCandidate.Op, left.DataType, left, loCandidate.Right!);
            Instruction instr = Assign(dst, expSum);
            var stmLong = stmts.Insert(iStm++, addr, instr);
            this.dst = ReplaceDstWithSsaIdentifier(this.dst, stmLong);

            var sidDst = GetSsaIdentifierOf(dst);
            if (stmMkLeft is not null)
                ssa.AddUses(stmMkLeft);
            ssa.AddUses(stmLong);

            var sidDstLo = GetSsaIdentifierOf(loCandidate.Dst);
            if (sidDstLo is not null)
            {
                var cast = new Slice(loCandidate.Dst.DataType, dst, 0);
                var stmCastLo = stmts.Insert(iStm++, addr, new AliasAssignment(
                    sidDstLo.Identifier, cast));
                var stmDeadLo = sidDstLo.DefStatement;
                sidDstLo.DefStatement = stmCastLo;

                var sidDstHi = GetSsaIdentifierOf(hiCandidate.Dst);
                var slice = new Slice(hiCandidate.Dst.DataType, dst, loCandidate.Dst.DataType.BitSize);
                var stmSliceHi = stmts.Insert(iStm++, addr, new AliasAssignment(
                    sidDstHi!.Identifier, slice));
                var stmDeadHi = sidDstHi.DefStatement;
                sidDstHi.DefStatement = stmSliceHi;

                if (sidDstLo is not null)
                {
                    sidDst!.Uses.Add(stmCastLo);
                }
                if (sidDstHi is not null)
                {
                    sidDst!.Uses.Add(stmSliceHi);
                }
                ssa.DeleteStatement(stmDeadLo!);
                ssa.DeleteStatement(stmDeadHi!);
            }
        }

        private int FindLongShiftInsertPosition(Candidate loCandidate, Candidate hiCandidate, StatementList stmts)
        {
            var istmHi = stmts.IndexOf(hiCandidate.Statement!);
            var istmLo = stmts.IndexOf(loCandidate.Statement!);
            var i = Math.Min(istmHi, istmLo);
            return Math.Max(i, 0);
        }
    }

    public class Candidate
    {
        public Candidate(BinaryOperator op, Expression left, Expression? right)
        {
            this.Op = op;
            this.Left = left;
            this.Right = right;
        }

        public Statement? Statement;
        public Expression? Dst;
        public readonly BinaryOperator Op;
        public readonly Expression Left;
        public readonly Expression? Right;
    }

    static LongAddRewriter()
    {
        condm = new InstructionMatcher(
            new Assignment(
                ExpressionMatcher.AnyId("grf"),
                new ConditionOf(
                    ExpressionMatcher.AnyExpression("exp"))));

        addPattern = new InstructionMatcher(
            new Assignment(
                ExpressionMatcher.AnyId("dst"),
                new BinaryExpression(
                    ExpressionMatcher.AnyBinaryOperator("op"),
                    VoidType.Instance,
                    ExpressionMatcher.AnyExpression("left"),
                    ExpressionMatcher.AnyExpression("right"))));

        adcPattern = new InstructionMatcher(
            new Assignment(
                ExpressionMatcher.AnyId("dst"),
                new BinaryExpression(
                    ExpressionMatcher.AnyBinaryOperator("op1"),
                    VoidType.Instance,
                    new BinaryExpression(
                        ExpressionMatcher.AnyBinaryOperator("op2"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("left"),
                        ExpressionMatcher.AnyExpression("right")),
                    ExpressionMatcher.AnyExpression("cf"))));

        memOffset = ExpressionMatcher.Build(m =>
            new MemoryAccess(
                new BinaryExpression(
                    ExpressionMatcher.AnyBinaryOperator("op"),
                    VoidType.Instance,
                    ExpressionMatcher.AnyExpression("base"),
                    ExpressionMatcher.AnyConstant("Offset")),
                ExpressionMatcher.AnyDataType("dt")));

        segMemOffset = new ExpressionMatcher(
            new MemoryAccess(
                MemoryStorage.GlobalMemory,
                new SegmentedPointer(
                    ExpressionMatcher.AnyDataType(null),
                    ExpressionMatcher.AnyId(),
                    new BinaryExpression(
                        ExpressionMatcher.AnyBinaryOperator("op"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("Offset"))),
                ExpressionMatcher.AnyDataType("dt")));
    }


}
