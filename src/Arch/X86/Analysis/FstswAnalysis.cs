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

using Reko.Arch.X86.Rewriter;
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using System;

namespace Reko.Arch.X86.Analysis
{
    /// <summary>
    /// This analysis evaluates X86 procedures, looking for specific code
    /// patterns involving the <c>fstsw</c> instruction. If the patterns are
    /// present, this analyis will replace them with constructs that will be
    /// cleaned up by the <see cref="ConditionCodeEliminator"/>.
    /// </summary>
    /// <remarks>
    /// The patterns usually  manifest as sequences of instructions like:
    /// <code>
    ///     fstsw   ax
    ///     test    ah,40h
    ///     jpe     somewhere
    /// </code>
    ///     
    /// where the bits in the x87 status word are copied to the AX register 
    /// and then examined. 
    /// 
    /// By the time this transform is applied, lifting and SSA transformation
    /// will have resulted in intermediate code like:
    /// 
    /// <code>
    ///     ax_1 = __fstsw(FPUF)
    ///     ah_2 = SLICE(ax_1, byte, 0)
    ///     SZP_3 = cond(ah_2 &amp; 0x40&lt;8&gt;)
    ///     O_4 = false
    ///     C_4 = false
    ///     P_5 = SZP_3 &amp; 0x20&lt;32&gt;
    ///     branch Test(PE,P_5) somewhere
    /// </code>
    ///     
    /// The net effect of this analysis is to bypass all that bit twiddling,
    /// resulting in the following code:
    /// 
    /// <code>
    ///     ax_1 = __fstsw(FPUF)
    ///     ah_2 = SLICE(ax_1, byte, 0)
    ///     SZP_3 = cond(ah_2 &amp; 0x40&lt;8&gt;)
    ///     O_4 = false
    ///     C_4 = false
    ///     P_5 = SZP_3 &amp; 0x20&lt;32&gt;
    ///     branch Test(EQ,FPUF) somewhere &lt;= note the changed Test instruction
    /// </code>
    ///     
    /// After value propagation and dead code elimination, we get:
    /// 
    /// <code>
    ///     branch Test(EQ,FPUF)
    /// </code>
    /// 
    /// This analysis used to be done by the <see cref="X86Rewriter"/>, but
    /// because no SSA is available that early, simplifying assumptions had
    /// to be made that affected the accuracy of these tranformations. By
    /// moving it to the analysis stage, we can benefit from the much
    /// improved data flow information that the <see cref="SsaState"/> 
    /// provides.
    /// </remarks>
    public class FstswAnalysis : IAnalysis<SsaState>
    {
        private readonly IReadOnlyProgram program;
        private readonly IEventListener listener;
        private SsaState ssa;

        public FstswAnalysis(IReadOnlyProgram program, IEventListener listener)
        {
            this.program = program;
            this.ssa = default!;
            this.listener = listener;
        }

        public string Id => "FstswAnalysis";

        public string Description => "X86-specific analysis of `fstsw` instructions.";

        public (SsaState, bool) Transform(SsaState ssa)
        {
            this.ssa = ssa;
            bool changed = false;
            foreach (var block in ssa.Procedure.ControlGraph.Blocks)
            {
                foreach (var stm in block.Statements)
                {
                    var (fpuf, ax) = FindFpufArgumentOfFstsw(stm);
                    if (fpuf is null || ax is null)
                        continue;
                    changed |= TransformFststw(ssa.Identifiers[ax], ssa.Identifiers[fpuf]);
                }
            }
            return (ssa, changed);
        }

        /// <summary>
        /// Determine whether the given <see cref="Statement"/> is the rewritten
        /// result of an x86 `fstsw` instruction.
        /// </summary>
        private (Identifier? fpuf, Identifier? ax) FindFpufArgumentOfFstsw(Statement stm)
        {
            if (stm.Instruction is Assignment ass &&
                ass.Src is Application appl &&
                appl.Procedure is ProcedureConstant pc &&
                pc.Procedure is IntrinsicProcedure intrinsic &&
                intrinsic == X86Rewriter.fstsw_intrinsic)
            {
                return (appl.Arguments[0] as Identifier, ass.Dst);
            }
            else
            {
                return (null, null);
            }
        }

        private bool TransformFststw(SsaIdentifier sid, SsaIdentifier sidFpuf)
        {
            var wl = new WorkList<(SsaIdentifier, int, int)>();
            var sidStart = sid;
            int shift = 0;
            int mask = 0;
            wl.Add((sid, shift, mask));
            bool changed = false;

            while (wl.TryGetWorkItem(out var item))
            {
                (sid, shift, mask) = item;
                foreach (var use in sid.Uses.ToArray())
                {
                    if (use.Instruction is not Assignment ass)
                        continue;
                    switch (ass.Src)
                    {
                    case BinaryExpression bin:
                        if (bin.Left == sid.Identifier &&
                            bin.Right is Constant andMask)
                        {
                            switch (bin.Operator.Type)
                            {
                            case OperatorType.And:
                                mask = andMask.ToInt32() >> (8 - shift);
                                wl.Add((ssa.Identifiers[ass.Dst], shift, mask));
                                break;
                            case OperatorType.Xor:
                                // If no AND instruction has been seen before the XOR,
                                // give up.
                                if (mask == 0)
                                    break;
                                mask = andMask.ToInt32() >> (8 - shift);
                                var (stmBranch, test) = FindUsingStatement(ass.Dst, true); //$REVIEW could there be many?
                                changed |= ReplaceTest(sid, sidFpuf, mask, stmBranch, test, EvaluateFstswXorInstructions);
                                break;
                            }
                        }
                        else
                        {
                            if (bin.Operator.Type == OperatorType.Or)
                            {
                                if (sid.Identifier.Storage is FlagGroupStorage)
                                {
                                    if (ass.Dst.Storage is FlagGroupStorage)
                                    {
                                        sid = ssa.Identifiers[ass.Dst];
                                        wl.Add((sid, shift, mask));
                                    }
                                }
                            }
                        }
                        break;
                    case Slice slice when slice.Expression == sid.Identifier:
                        shift = slice.Offset;
                        wl.Add((ssa.Identifiers[ass.Dst], shift, mask));
                        break;
                    case ConditionOf cof:
                        if (cof.Expression is BinaryExpression binCof &&
                            ass.Dst.Storage is FlagGroupStorage flg &&
                            binCof.Right is Constant imm &&
                            binCof.Left is Identifier acc && 
                            acc == sid.Identifier)
                        {
                            switch (binCof.Operator.Type)
                            {
                            case OperatorType.And:
                                // This is a rewritten TEST instruction acting directly on the AX/AH register
                                mask = imm.ToInt32() >> (8 - shift);
                                var (stmUse, test) = FindUsingStatement(ass.Dst, false); //$REVIEW could there be many?
                                changed |= ReplaceTest(sid, sidFpuf, mask, stmUse, test, EvaluateFstswTestInstructions);
                                break;
                            case OperatorType.ISub:
                                // This is a rewritten CMP instruction, which acts on result of a 
                                // previous AND instruction. If the mask is 0, no AND has been seen.
                                if (mask == 0)
                                    break;
                                mask = mask & (imm.ToInt32() >> (8 - shift));
                                (stmUse, test) = FindUsingStatement(ass.Dst, false); //$REVIEW: could there be many?
                                changed |= ReplaceTest(sid, sidFpuf, mask, stmUse, test, EvaluateFstswCmpInstructions);
                                break;
                            }
                        }
                        break;
                    case Identifier idSrc:
                        if (ass.Dst.Storage is FlagGroupStorage)
                        {
                            // This is a rewritten SAHF instruction
                            ReplaceWithFpufCopy(use, ass.Dst, sidFpuf);
                            changed = true;
                        }
                        else if (ass.Dst.Storage is RegisterStorage &&
                            idSrc.Storage is FlagGroupStorage)
                        {
                            // This is a rewritten LAHF instruction
                            sid = ssa.Identifiers[ass.Dst];
                            wl.Add((sid, shift, mask));
                        }
                        break;
                    case MkSequence _:
                        // An aliasing use of ax or ah; eax = SEQ(<stuff>, ax) can be ignored.
                        continue;
                    case Conversion conv:
                        if (conv.DataType.Domain == Core.Types.Domain.SignedInt)
                            goto default;
                        wl.Add((ssa.Identifiers[ass.Dst], shift, mask));
                        break;
                    default:
                        throw new NotImplementedException($"Fstsw of {ass} not implemented yet.");
                    }
                }
            }
            return changed;
        }

        private bool ReplaceTest(
            SsaIdentifier sid,
            SsaIdentifier sidFpuf,
            int mask, 
            Statement? stmUse,
            TestCondition? test,
            Func<Statement, ConditionCode, int, ConditionCode> fn)
        {
            if (stmUse is null || test is null)
            {
                listener.Warn(listener.CreateStatementNavigator(program, sid.DefStatement), "Couldn't find a branch instruction using `fstsw`.");
                return false;
            }
            var ccode = fn(stmUse, test.ConditionCode, mask);
            if (ccode == ConditionCode.None)
                return false;

            ssa.RemoveUses(stmUse, test.Expression);
            if (stmUse.Instruction is Branch branch)
            {
                var newCondition = ExpressionReplacer.Replace(
                    test,
                    new TestCondition(ccode, sidFpuf.Identifier),
                    branch.Condition);
                stmUse.Instruction = new Branch(newCondition, branch.Target);
            }
            else if (stmUse.Instruction is Assignment ass)
            {
                var newSrc = ExpressionReplacer.Replace(
                    test,
                    new TestCondition(ccode, sidFpuf.Identifier),
                    ass.Src);
                stmUse.Instruction = new Assignment(ass.Dst, newSrc);
            }
            ssa.AddUses(stmUse, sidFpuf.Identifier);
            return true;
        }

        private void ReplaceWithFpufCopy(Statement use, Identifier dst, SsaIdentifier sidStart)
        {
            ssa.RemoveUses(use);
            use.Instruction = new Assignment(dst, sidStart.Identifier);
            ssa.AddUses(use);
        }

        /// <summary>
        /// Bypass all conversions and bit masks of the FPUF flags to find a statement that actually uses
        /// the value: a Test() or a Branch()
        /// </summary>
        /// <param name="id">Identifier whose uses we are tracing.</param>
        /// <param name="findCond">If true, allow COND() expressions in the search.</param>
        /// <returns></returns>
        private (Statement?, TestCondition?) FindUsingStatement(Identifier id, bool findCond)
        {
            var sid = ssa.Identifiers[id];
            var wl = new WorkList<SsaIdentifier>();
            wl.Add(sid);
            while (wl.TryGetWorkItem(out sid))
            {
                foreach (var use in sid.Uses)
                {
                    switch (use.Instruction)
                    {
                    case Assignment ass:
                        if (ass.Src is Slice slice &&
                            slice.Expression == sid.Identifier)
                        {
                            wl.Add(ssa.Identifiers[ass.Dst]);
                            break;
                        }
                        if (ass.Src is BinaryExpression bin &&
                            bin.Operator == Operator.And &&
                            bin.Left == sid.Identifier &&
                            bin.Right is Constant)
                        {
                            wl.Add(ssa.Identifiers[ass.Dst]);
                            break;
                        }
                        else if (findCond &&
                            ass.Src is ConditionOf cof &&
                            cof.Expression == sid.Identifier)
                        {
                            wl.Add(ssa.Identifiers[ass.Dst]);
                            break;
                        }
                        else if (ass.Src is Conversion cvt &&
                            cvt.Expression is TestCondition test &&
                            test.Expression == sid.Identifier)
                        {
                            return (use, test);
                        }
                        break;
                    case Branch b:
                        //$REVIEW what if there are more uses?
                        if (b.Condition is TestCondition testBr)
                            return (use, testBr);
                        break;
                    default:
                        break;
                    }
                }
            }
            return (null, null);
        }

        // 8087 status register bits:
        // bit 8: C0        - 0x0100
        // bit 9: C1        - 0x0200
        // bit 10: C2       - 0x0400
        // bit 14: C3       - 0x4000
        // 8086 flag register bits:
        // bit 0: CF        - 0x0001
        // bit 1: RESERVED
        // bit 2: PF        - 0x0004
        // bit 6: ZF        - 0x0040


        // https://www.plantation-productions.com/Webster/www.artofasm.com/DOS/ch14/CH14-3.html
        // Instruction          Condition Code Bits Condition
        //                      C3 C2 C1 C0 
        // fcom, fcomp, fcompp, 0 0 X 0             ST > source
        // ficom, ficomp        0 0 X 1             ST < source
        //                      1 0 X 0             ST == source
        //                      1 1 X 1             ST or source undefined
        // fist                 0 0 X 0             ST > 0
        //                      0 0 X 1             ST < 0
        //                      1 0 X 0             ST == +/- 0
        //                      1 1 X 1             ST uncomparable
        // fxam                 0 0 0 0             + unnormalized
        //                      0 0 1 0             - unnormalized
        //                      0 1 0 0             + normalized
        //                      0 1 1 0             - normalized
        //                      1 0 0 0             + 0
        //                      1 0 1 0             - 0
        //                      1 1 0 0             + denormalized
        //                      1 1 1 0             - denormalized
        //                      0 0 0 1             + NaN
        //                      0 0 1 1             - NaN
        //                      0 1 0 1             + Infinity
        //                      0 1 1 1             - Infinity
        //                      1 X X 1             Empty register
        // fucom, fucomp,       0 0 X 0             ST > source
        // fucompp              0 0 X 1             ST < source
        //                      1 0 X 0             ST = source
        //                      1 1 X 1             Unordered

        private ConditionCode EvaluateFstswCmpInstructions(Statement stm, ConditionCode ccode, int mask)
        {
            switch (ccode)
            {
            case ConditionCode.EQ:
                switch (mask)
                {
                case 0x01: return ConditionCode.GE;
                case 0x40: return ConditionCode.EQ;
                }
                break;
            case ConditionCode.NE:
                switch (mask)
                {
                case 0x01: return ConditionCode.LT;
                case 0x40: return ConditionCode.NE;
                }
                break;
            }
            this.listener.Warn(
                listener.CreateStatementNavigator(program, stm),
                "Unexpected {0} fstsw;cmp mask for {1} mnemonic.", mask, ccode);
            return ConditionCode.None;
        }

        private ConditionCode EvaluateFstswXorInstructions(Statement stm, ConditionCode ccode, int mask)
        {
            switch (ccode)
            {
            case ConditionCode.EQ:
                switch (mask)
                {
                case 0x40: return ConditionCode.EQ;
                }
                break;
            case ConditionCode.NE:
                switch (mask)
                {
                case 0x40: return ConditionCode.NE;
                }
                break;
            }
            this.listener.Warn(
                listener.CreateStatementNavigator(program, stm),
                "Unexpected {0} fstsw;xor mask for {1} mnemonic.", mask, ccode);
            return ConditionCode.None;
        }
      
        private ConditionCode EvaluateFstswTestInstructions(Statement stm, ConditionCode ccode, int mask)
        {
            /* fcom/fcomp/fcompp Results:
                Condition      C3  C2  C0
                ST(0) > SRC     0   0   0
                ST(0) < SRC     0   0   1
                ST(0) = SRC     1   0   0
                Unordered       1   1   1

               Masks:
                Mask   Flags
                0x01   C0
                0x04   C2
                0x40   C3
                0x05   C2 and C0
                0x41   C3 and C0
                0x44   C3 and C2

              Masks && jump operations:
                Mnem   Mask Condition
                jpe    0x05    >=
                jpe    0x41    >
                jpe    0x44    !=
                jpo    0x05    <
                jpo    0x41    <=
                jpo    0x44    =
                jz     0x01    >=
                jz     0x40    !=
                jz     0x41    >
                jnz    0x01    <
                jnz    0x40    =
                jnz    0x41    <=
            */

            switch (ccode)
            {
            case ConditionCode.PE:
                switch (mask)
                {
                case 0x05: return ConditionCode.GE;
                case 0x41: return ConditionCode.GT;
                case 0x44: return ConditionCode.NE;
                default:
                    this.listener.Warn(
                        listener.CreateStatementNavigator(program, stm),
                        "Unexpected {0} fstsw mask for {1} mnemonic.", mask, ccode);
                    return ConditionCode.None;
                }
            case ConditionCode.PO:
                switch (mask)
                {
                case 0x44: return ConditionCode.EQ;
                case 0x41: return ConditionCode.LE;
                case 0x05: return ConditionCode.LT;
                default:
                    this.listener.Warn(
                        listener.CreateStatementNavigator(program, stm),
                        "Unexpected {0} fstsw mask for {1} mnemonic.", mask, ccode);
                    return ConditionCode.None;
                }
            case ConditionCode.EQ:
                switch (mask)
                {
                case 0x40: return ConditionCode.NE;
                case 0x41: return ConditionCode.GT;
                case 0x45: return ConditionCode.GT; //$TODO: or unordered.
                case 0x01: return ConditionCode.GE;
                case 0x05: return ConditionCode.GE;   //$TODO: or unordered
                default:
                    this.listener.Warn(
                        listener.CreateStatementNavigator(program, stm),
                        "Unexpected {0} fstsw mask for {1} mnemonic.", mask, ccode);
                    return ConditionCode.None;
                }
            case ConditionCode.NE:
                switch (mask)
                {
                case 0x40: return ConditionCode.EQ;
                case 0x41: return ConditionCode.LE;
                case 0x45: return ConditionCode.LE;  //$TODO: or unordered
                case 0x01: return ConditionCode.LT;
                case 0x05: return ConditionCode.LT;   //$TODO: or unordered
                default:
                    this.listener.Warn(
                        listener.CreateStatementNavigator(program, stm),
                        "Unexpected {0} fstsw mask for {1} mnemonic.", mask, ccode);
                    return ConditionCode.None;
                }
            }
            return ConditionCode.None;
        }
    }
}
