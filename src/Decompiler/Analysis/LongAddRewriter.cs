#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Lib;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Analysis
{
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
    public class LongAddRewriter
    {
        private SsaState ssa;
        private Expression dst;
        private IProcessorArchitecture arch;

        private static InstructionMatcher adcPattern;
        private static InstructionMatcher addPattern;
        private static ExpressionMatcher memOffset;
        private static ExpressionMatcher segMemOffset;
        private static InstructionMatcher condm;

        public LongAddRewriter(SsaState ssa)
        {
            this.ssa = ssa;
            this.arch = ssa.Procedure.Architecture;
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
                        ExpressionMatcher.AnyOperator("op"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("left"),
                        ExpressionMatcher.AnyExpression("right"))));

            adcPattern = new InstructionMatcher(
                new Assignment(
                    ExpressionMatcher.AnyId("dst"),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op1"),
                        VoidType.Instance,
                        new BinaryExpression(
                            ExpressionMatcher.AnyOperator("op2"),
                            VoidType.Instance,
                            ExpressionMatcher.AnyExpression("left"),
                            ExpressionMatcher.AnyExpression("right")),
                        ExpressionMatcher.AnyExpression("cf"))));

            memOffset = new ExpressionMatcher(
                new MemoryAccess(
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("offset")),
                    ExpressionMatcher.AnyDataType("dt")));

            segMemOffset = new ExpressionMatcher(
                new SegmentedAccess(
                    MemoryIdentifier.GlobalMemory,
                    ExpressionMatcher.AnyId(),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("offset")),
                    ExpressionMatcher.AnyDataType("dt")));
        }

        public void CreateLongInstruction(AddSubCandidate loCandidate, AddSubCandidate hiCandidate)
        {
            var totalSize = PrimitiveType.Create(
                Domain.SignedInt | Domain.UnsignedInt,
                loCandidate.Dst.DataType.BitSize + hiCandidate.Dst.DataType.BitSize);
            var left = CreateCandidate(loCandidate.Left, hiCandidate.Left, totalSize);
            var right = CreateCandidate(loCandidate.Right, hiCandidate.Right, totalSize);
            this.dst = CreateCandidate(loCandidate.Dst, hiCandidate.Dst, totalSize);
            var stmts = hiCandidate.Statement.Block.Statements;
            var linAddr = hiCandidate.Statement.LinearAddress;
            var iStm = FindInsertPosition(loCandidate, hiCandidate, stmts);
            Statement stmMkLeft = null;
            if (left is Identifier)
            {
                stmMkLeft = stmts.Insert(
                    iStm++,
                    linAddr,
                    CreateMkSeq(left, hiCandidate.Left, loCandidate.Left));
                left = ReplaceDstWithSsaIdentifier(left, null, stmMkLeft);
            }

            Statement stmMkRight = null;
            if (right is Identifier)
            {
                stmMkRight = stmts.Insert(
                    iStm++,
                    linAddr,
                    CreateMkSeq(right, hiCandidate.Right, loCandidate.Right));
                right = ReplaceDstWithSsaIdentifier(right, null, stmMkRight);
            }

            var expSum = new BinaryExpression(loCandidate.Op, left.DataType, left, right);
            Instruction instr = Assign(dst, expSum);
            var stmLong = stmts.Insert(iStm++, linAddr, instr);
            this.dst = ReplaceDstWithSsaIdentifier(this.dst, expSum, stmLong);

            var sidDst = GetSsaIdentifierOf(dst);
            var sidLeft = GetSsaIdentifierOf(left);
            var sidRight = GetSsaIdentifierOf(right);
            if (stmMkLeft != null && sidLeft != null)
            {
                GetSsaIdentifierOf(loCandidate.Left)?.Uses.Add(stmMkLeft);
                GetSsaIdentifierOf(hiCandidate.Left)?.Uses.Add(stmMkLeft);
            }
            if (stmMkRight != null && sidRight != null)
            {
                GetSsaIdentifierOf(loCandidate.Right)?.Uses.Add(stmMkRight);
                GetSsaIdentifierOf(hiCandidate.Right)?.Uses.Add(stmMkRight);
            }
            if (sidDst != null)
            {
                if (sidLeft != null)
                    sidLeft.Uses.Add(stmLong);
                if (sidRight != null)
                    sidRight.Uses.Add(stmLong);
            }

            var sidDstLo = GetSsaIdentifierOf(loCandidate.Dst);
            if (sidDstLo != null)
            {
                var cast = new Slice(loCandidate.Dst.DataType, dst, 0);
                var stmCastLo = stmts.Insert(iStm++, linAddr, new AliasAssignment(
                    sidDstLo.Identifier, cast));
                var stmDeadLo = sidDstLo.DefStatement;
                sidDstLo.DefExpression = cast;
                sidDstLo.DefStatement = stmCastLo;

                var sidDstHi = GetSsaIdentifierOf(hiCandidate.Dst);
                var slice = new Slice(hiCandidate.Dst.DataType, dst, loCandidate.Dst.DataType.BitSize);
                var stmSliceHi = stmts.Insert(iStm++, linAddr, new AliasAssignment(
                    sidDstHi.Identifier, slice));
                var stmDeadHi = sidDstHi.DefStatement;
                sidDstHi.DefExpression = slice;
                sidDstHi.DefStatement = stmSliceHi;

                if (sidDstLo != null)
                {
                    sidDst.Uses.Add(stmCastLo);
                }
                if (sidDstHi != null)
                {
                    sidDst.Uses.Add(stmSliceHi);
                }
                ssa.DeleteStatement(stmDeadLo);
                ssa.DeleteStatement(stmDeadHi);
            }
        }

        /// <summary>
        /// Find a statement index appropriate for insert the new
        /// long addition statements.
        /// </summary>
        /// <returns></returns>
        private int FindInsertPosition(AddSubCandidate loCandidate, AddSubCandidate hiCandidate, StatementList stmts)
        {
            int iStm = stmts.IndexOf(hiCandidate.Statement);
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

        private Expression ReplaceDstWithSsaIdentifier(Expression dst, BinaryExpression src, Statement stmLong)
        {
            if (stmLong.Instruction is Assignment ass) {
                var sid = ssa.Identifiers.Add(ass.Dst, stmLong, src, false);
                ass.Dst = sid.Identifier;
                return ass.Dst;
            }
            return dst;
        }

        private Instruction CreateMkSeq(Expression dst, Expression hi, Expression lo)
        {
            return Assign(dst, new MkSequence(dst.DataType, hi, lo));
        }

        private Instruction Assign(Expression dst, Expression src)
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

        private SsaIdentifier GetSsaIdentifierOf(Expression dst)
        {
            if (dst is Identifier id)
                return ssa.Identifiers[id];
            else
                return null;
        }

        public void Transform()
        {
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                ReplaceLongAdditions(block);
            }
        }

        /// <summary>
        /// Look for add/adc or sub/sbc pairs that look like long additions
        /// by scanning the statements from the beginning of the block to the end.
        /// </summary>
        public void ReplaceLongAdditions(Block block)
        {
            var stmtsOrig = block.Statements.ToList();
            for (int i = 0; i < block.Statements.Count; ++i)
            {
                var loInstr = MatchAddSub(block.Statements[i]);
                if (loInstr == null)
                    continue;
                var cond = FindConditionOf(stmtsOrig, i, loInstr.Dst);
                if (cond == null)
                    continue;
                var hiInstr = FindUsingInstruction(cond.FlagGroup, loInstr);
                if (hiInstr == null)
                    continue;

                CreateLongInstruction(loInstr, hiInstr);
            }
        }

        /// <summary>
        /// Determines if the carry flag reaches a using instruction.
        /// </summary>
        /// <param name="instrs"></param>
        /// <param name="i"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public AddSubCandidate FindUsingInstruction(Identifier cy, AddSubCandidate loInstr)
        {
            var queue = new Queue<Statement>(ssa.Identifiers[cy].Uses);
            while (queue.Count > 0)
            {
                var use = queue.Dequeue();
                var asc = MatchAdcSbc(use);
                if (asc != null)
                {
                    //Debug.Print("Left sides: [{0}] [{1}]", asc.Left, loInstr.Left);
                    if (asc.Left.GetType() != loInstr.Left.GetType())
                        return null;
                    asc.Statement = use;
                    return asc;
                }
                if (!(use.Instruction is Assignment ass))
                    continue;
                if (ass.Src is Slice)
                {
                    queue.EnqueueRange(ssa.Identifiers[ass.Dst].Uses);
                    continue;
                }
                if (IsCarryFlag(ass.Dst))
                    return null;
            }
            return null;
        }

        public bool IsCarryFlag(Expression exp)
        {
            if (!(exp is Identifier cf))
                return false;
            if (!(cf.Storage is FlagGroupStorage grf))
                return false;
            return (arch.CarryFlagMask & grf.FlagGroupBits) != 0;
        }

        /// <summary>
        /// Finds the subsequent statement in this block that defines a condition code based on the
        /// result in expression <paramref name="exp"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ax"></param>
        /// <returns></returns>
        public CondMatch FindConditionOf(List<Statement> stms, int iStm, Expression exp)
        {
            if (exp is Identifier idLo)
            {
                foreach (var use in ssa.Identifiers[idLo].Uses.Where(u => condm.Match(u.Instruction)))
                {
                    var grf = (Identifier)condm.CapturedExpressions("grf");
                    var condExp = condm.CapturedExpressions("exp");
                    if (grf.Storage is FlagGroupStorage && exp == condExp)
                    {
                        return new CondMatch { FlagGroup = grf, src = exp, Statement = use };
                    }
                }
            }
            for (int i = iStm + 1; i < stms.Count; ++i)
            {
                if (!condm.Match(stms[i].Instruction))
                    continue;
                var grf = (Identifier)condm.CapturedExpressions("grf");
                var condExp = condm.CapturedExpressions("exp");
                if (grf.Storage is FlagGroupStorage && exp == condExp)
                {
                    return new CondMatch { FlagGroup = grf, src = exp, StatementIndex = i };
                }
            }
            return null;
        }

        private Expression CreateCandidate(Expression expLo, Expression expHi, DataType totalSize)
        {
            if (expLo is Identifier idLo && expHi is Identifier idHi)
            {
                return ssa.Procedure.Frame.EnsureSequence(totalSize, idHi.Storage, idLo.Storage);
            }
            if (expLo is MemoryAccess memDstLo && expHi is MemoryAccess memDstHi && MemoryOperandsAdjacent(memDstLo, memDstHi))
            {
                return CreateMemoryAccess(memDstLo, totalSize);
            }
            if (expLo is Constant immLo && expHi is Constant immHi)
            {
                return Constant.Create(totalSize, (immHi.ToUInt64() << expLo.DataType.BitSize) | immLo.ToUInt32());
            }
            return new MkSequence(totalSize, expHi, expLo);
        }

        private Expression CreateMemoryAccess(MemoryAccess mem, DataType totalSize)
        {
            if (mem is SegmentedAccess segmem)
            {
                return new SegmentedAccess(segmem.MemoryId, segmem.BasePointer, segmem.EffectiveAddress, totalSize);
            }
            else
            {
                return new MemoryAccess(mem.MemoryId, mem.EffectiveAddress, totalSize);
            }
        }

        public class CondMatch
        {
            public int StatementIndex;
            public Identifier FlagGroup;
            public Expression src;
            public Statement Statement;
        }

        public bool MemoryOperandsAdjacent(MemoryAccess m1, MemoryAccess m2)
        {
            var off1 = GetOffset(m1);
            var off2 = GetOffset(m2);
            if (off1 == null || off2 == null)
                return false;
            return ssa.Procedure.Architecture.Endianness.OffsetsAdjacent(off1.ToInt32(), off2.ToInt32(), m1.DataType.Size);
        }

        private Constant GetOffset(MemoryAccess access)
        {
            if (memOffset.Match(access))
            {
                return (Constant)memOffset.CapturedExpression("offset");
            }
            if (segMemOffset.Match(access))
            {
                return (Constant)segMemOffset.CapturedExpression("offset");
            }
            return null;
        }

        /// <summary>
        /// Matches an "ADC" or "SBB/SBC" pattern.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>If the match succeeded, returns a partial BinaryExpression
        /// with the left and right side of the ADC/SBC instruction.</returns>
        public AddSubCandidate MatchAdcSbc(Statement stm)
        {
            if (!adcPattern.Match(stm.Instruction))
                return null;
            if (!IsCarryFlag(adcPattern.CapturedExpressions("cf")))
                return null;
            var op = adcPattern.CapturedOperators("op2");
            if (!IsIAddOrISub(op))
                return null;
            return new AddSubCandidate
            {
                Dst = adcPattern.CapturedExpressions("dst"),
                Op = op,
                Left = adcPattern.CapturedExpressions("left"),
                Right = adcPattern.CapturedExpressions("right"),
                Statement = stm
            };
        }

        public AddSubCandidate MatchAddSub(Statement stm)
        {
            if (!addPattern.Match(stm.Instruction))
                return null;
            var op = addPattern.CapturedOperators("op");
            if (!IsIAddOrISub(op))
                return null;
            return new AddSubCandidate
            {
                Dst = addPattern.CapturedExpressions("dst"),
                Op = op,
                Left = addPattern.CapturedExpressions("left"),
                Right = addPattern.CapturedExpressions("right")
            };
        }

        private static bool IsIAddOrISub(Operator op)
        {
            return (op == Operator.IAdd || op == Operator.ISub);
        }
    }

    public class AddSubCandidate
    {
        public int StatementIndex;
        public Statement Statement;
        public Expression Dst;
        public Operator Op;
        public Expression Left;
        public Expression Right;
    }

    public class CarryLinkedInstructions
    {
        public Instruction High;
        public Instruction Low;
    }
}
