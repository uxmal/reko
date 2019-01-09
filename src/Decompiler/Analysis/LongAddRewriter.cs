#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
    /// </remarks>
    public class LongAddRewriter
    {
        private Procedure proc;
        private Expression dst;
        private IProcessorArchitecture arch;

        private static InstructionMatcher adcPattern;
        private static InstructionMatcher addPattern;
        private static ExpressionMatcher memOffset;
        private static ExpressionMatcher segMemOffset;
        private static InstructionMatcher condm;

        public LongAddRewriter(Procedure proc)
        {
            this.arch = proc.Architecture;
            this.proc = proc;
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
                    null,
                    ExpressionMatcher.AnyId(),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"),
                        VoidType.Instance,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("offset")),
                    ExpressionMatcher.AnyDataType("dt")));
        }

        public Instruction CreateLongInstruction(AddSubCandidate loCandidate, AddSubCandidate hiCandidate)
        {
            var totalSize = PrimitiveType.Create(
                Domain.SignedInt | Domain.UnsignedInt,
                loCandidate.Dst.DataType.BitSize + loCandidate.Dst.DataType.BitSize);
            var left = CreateCandidate(loCandidate.Left, hiCandidate.Left, totalSize);
            var right = CreateCandidate(loCandidate.Right, hiCandidate.Right, totalSize);
            this.dst = CreateCandidate(loCandidate.Dst, hiCandidate.Dst, totalSize);

            if (left == null || right == null || dst == null)
                return null;
        
            var expSum = new BinaryExpression(loCandidate.Op, left.DataType, left, right);
            if (dst is Identifier idDst)
            {
                return new Assignment(idDst, expSum);
            }
            else
            {
                return new Store(dst, expSum);
            }
        }

        public void Transform()
        {
            foreach (var block in proc.ControlGraph.Blocks)
            {
                ReplaceLongAdditions(block);
            }
        }

        public void ReplaceLongAdditions(Block block)
        {
            for (int i = 0; i < block.Statements.Count; ++i)
            {
                var loInstr = MatchAddSub(block.Statements[i].Instruction);
                if (loInstr == null)
                    continue;
                var cond = FindConditionOf(block.Statements, i, loInstr.Dst);
                if (cond == null)
                    continue;

                var hiInstr = FindUsingInstruction(block.Statements, cond.StatementIndex, loInstr);
                if (hiInstr == null)
                    continue;

                var longInstr = CreateLongInstruction(loInstr, hiInstr);
                if (longInstr != null)
                {
                    block.Statements[hiInstr.StatementIndex].Instruction = longInstr;
                    cond = FindConditionOf(block.Statements, hiInstr.StatementIndex, hiInstr.Dst);
                    if (cond != null)
                    {
                        block.Statements[cond.StatementIndex].Instruction =
                            new Assignment(
                                cond.FlagGroup, new ConditionOf(dst));
                        i = cond.StatementIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the carry flag reaches a using instruction.
        /// </summary>
        /// <param name="instrs"></param>
        /// <param name="i"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public AddSubCandidate FindUsingInstruction(StatementList stms, int i, AddSubCandidate loInstr)
        {
            for (++i; i < stms.Count; ++i)
            {
                var asc = MatchAdcSbc(stms[i].Instruction);
                if (asc != null)
                {
                    //Debug.Print("Left sides: [{0}] [{1}]", asc.Left, loInstr.Left);
                    if (asc.Left.GetType() != loInstr.Left.GetType())
                        return null;
                    asc.StatementIndex = i;
                    return asc;
                }
                if (!(stms[i].Instruction is Assignment ass))
                    continue;
                if (IsCarryFlag(ass.Dst))
                    return null;
            }
            return null;
        }

        public int FindUsingInstruction2(StatementList stms, int i, Operator next)
        {
            for (++i; i < stms.Count; ++i)
            {
                if (!(stms[i].Instruction is Assignment ass))
                    continue;
                if (!(ass.Src is BinaryExpression bin))
                    continue;
                if (bin.Operator == next)
                    return i;
                if (IsCarryFlag(ass.Dst))
                    return -1;
            }
            return -1;
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
        public CondMatch FindConditionOf(StatementList stms, int iStm, Expression exp)
        {
            for (int i = iStm + 1; i < stms.Count; ++i)
            {
                if (!condm.Match(stms[i].Instruction))
                    continue;
                var grf = (Identifier) condm.CapturedExpressions("grf");
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
            var idLo = expLo as Identifier;
            var idHi = expHi as Identifier;
            if (idLo != null && idHi != null)
            {
                return proc.Frame.EnsureSequence(idHi.Storage, idLo.Storage, totalSize);
            }
            var memDstLo = expLo as MemoryAccess;
            var memDstHi = expHi as MemoryAccess;
            if (memDstLo != null && memDstHi != null && MemoryOperandsAdjacent(memDstLo, memDstHi))
            {
                return CreateMemoryAccess(memDstLo, totalSize);
            }
            var immLo = expLo as Constant;
            var immHi = expHi as Constant;
            if (immLo != null && immHi != null)
            {
                return Constant.Create(totalSize, ((ulong)immHi.ToUInt32() << expLo.DataType.BitSize) | immLo.ToUInt32());
            }
            return null;
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
        }

        public CondMatch MatchCond(Instruction instr)
        {
            var ass = instr as Assignment;
            if (ass == null)
                return null;
            var cond = ass.Src as ConditionOf;
            if (cond == null)
                return null;
            var src = cond.Expression as Identifier;
            if (cond == null)
                return null;
            return new CondMatch { src = src, FlagGroup = ass.Dst };
        }

        public bool MemoryOperandsAdjacent(MemoryAccess m1, MemoryAccess m2)
        {
            //$TODO: endianness?
            var off1 = GetOffset(m1);
            var off2 = GetOffset(m2);
            if (off1 == null || off2 == null)
                return false;
            return off1.ToInt32() + m1.DataType.Size == off2.ToInt32();
        }

        private Constant GetOffset(MemoryAccess access)
        {
            if (memOffset.Match(access))
            {
                return (Constant) memOffset.CapturedExpression("offset");
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
        public AddSubCandidate MatchAdcSbc(Instruction instr)
        {
            if (!adcPattern.Match(instr))
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
                Right = adcPattern.CapturedExpressions("right")
            };
        }

        public AddSubCandidate MatchAddSub(Instruction instr)
        {
            if (!addPattern.Match(instr))
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

        public IEnumerable<CarryLinkedInstructions> FindCarryLinkedInstructions(Block block)
        {
            for (var i = block.Statements.Count - 1; i>= 0; --i) 
            {
                //FindUseCarryFlagInAddition(stm);
                //FindDefOfCarry(stm);

            }
            yield break;
        }

        public static void TestCondition()
        {
            //LongAddRewriter larw = new LongAddRewriter(this.frame, state);
            //int iUse = larw.IndexOfUsingOpcode(instrs, i, next);
            //if (iUse >= 0 && larw.Match(instrCur, instrs[iUse]))
            //{
            //    instrs[iUse].code = Opcode.nop;
            //    larw.EmitInstruction(op, emitter);
            //    return larw.Dst;
            //}
        }

    }

    public class AddSubCandidate
    {
        public int StatementIndex;
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
