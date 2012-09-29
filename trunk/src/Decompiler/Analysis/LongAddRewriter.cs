#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decompiler.Analysis
{
    /// <summary>
    /// Locates instances of add aLo, bLow, adc aHi, bHi and merges them into (add a, b)
    /// </summary>
    public class LongAddRewriter
    {
        private Frame frame;
        private Expression dst;
        private Expression src;
        private bool useStore;
        private IProcessorArchitecture arch;
        private InstructionMatcher adcPattern;
        private InstructionMatcher addPattern;
        private ExpressionMatcher memOffset;
        private ExpressionMatcher segMemOffset;

        public LongAddRewriter(IProcessorArchitecture arch, Frame frame)
        {
            this.arch = arch;
            this.frame = frame;

            this.addPattern = new InstructionMatcher(
                new Assignment(
                    ExpressionMatcher.AnyId("dst"),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"), null,
                        ExpressionMatcher.AnyExpression("left"),
                        ExpressionMatcher.AnyExpression("right"))));

            this.adcPattern = new InstructionMatcher(
                new Assignment(
                    ExpressionMatcher.AnyId("dst"),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op1"), null,
                        new BinaryExpression(
                            ExpressionMatcher.AnyOperator("op2"), null,
                            ExpressionMatcher.AnyExpression("left"),
                            ExpressionMatcher.AnyExpression("right")),
                        ExpressionMatcher.AnyExpression("cf"))));

            this.memOffset = new ExpressionMatcher(
                new MemoryAccess(
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"), null,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("offset")),
                    ExpressionMatcher.AnyDataType("dt")));
            this.segMemOffset = new ExpressionMatcher(
                new SegmentedAccess(
                    null,
                    ExpressionMatcher.AnyId(),
                    new BinaryExpression(
                        ExpressionMatcher.AnyOperator("op"), null,
                        ExpressionMatcher.AnyExpression("base"),
                        ExpressionMatcher.AnyConstant("offset")),
                    ExpressionMatcher.AnyDataType("dt")));
        }

        private Instruction CreateInstruction(Expression dst, Operator op, Expression left, Expression right)
        {
            var expSum = new BinaryExpression(op, left.DataType, left, right);
            var idDst = dst as Identifier;
            if (idDst != null)
            {
                return new Assignment(idDst, expSum);
            }
            else
            {
                return new Store(dst, expSum);
            }
        }

        /// <summary>
        /// Determines if the carry flag reaches a using instruction.
        /// </summary>
        /// <param name="instrs"></param>
        /// <param name="i"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public int IndexOfUsingOpcode(StatementList stms, int i, Operator next)
        {
            for (++i; i < stms.Count; ++i)
            {
                var ass = stms[i].Instruction as Assignment;
                if (ass == null)
                    continue;
                var bin = ass.Src as BinaryExpression;
                if (bin.Operator == next)
                    return i;
                if (IsCarryFlag(ass.Dst))
                    return -1;
            }
            return -1;
        }

        public bool IsCarryFlag(Expression exp)
        {
            var cf = exp as Identifier;
            if (cf == null)
                return false;
            var grf = cf.Storage as FlagGroupStorage;
            if (grf == null)
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
        public CondMatch FindCond(StatementList stms, int iStm, Expression exp)
        {
            var condm = new InstructionMatcher(
                new Assignment(
                    ExpressionMatcher.AnyId("grf"),
                    new ConditionOf(
                        ExpressionMatcher.AnyExpression("exp"))));
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

        public Expression MakeMatch(Expression expLo, Expression expHi, DataType totalSize, bool isDef)
        {
            var idLo = expLo as Identifier;
            var idHi = expHi as Identifier;
            if (idLo != null && idHi != null)
            {
                if (isDef)
                    useStore = false;
                return frame.EnsureSequence(idHi, idLo, totalSize);
            }
            var memDstLo = expLo as MemoryAccess;
            var memDstHi = expHi as MemoryAccess;
            if (memDstLo != null && memDstHi != null && MemoryOperandsAdjacent(memDstLo, memDstHi))
            {
                if (isDef)
                    useStore = true;
                return CreateMemoryAccess(memDstLo, totalSize);
            }
            var immLo = expLo as Constant;
            var immHi = expHi as Constant;
            if (immLo != null && immHi != null)
            {
                return new Constant(totalSize, ((ulong)immHi.ToUInt32() << expLo.DataType.BitSize) | immLo.ToUInt32());
            }
            return null;
        }

        private Expression CreateMemoryAccess(MemoryAccess mem, DataType totalSize)
        {
            var segmem = mem as SegmentedAccess;
            if (segmem != null)
            {
                return new SegmentedAccess(segmem.MemoryId, segmem.BasePointer, segmem.EffectiveAddress, totalSize);
            }
            else
            {
                return new MemoryAccess(mem.MemoryId, mem.EffectiveAddress, totalSize);
            }
        }

        public Instruction Match(Instruction loInstr, Instruction hiInstr)
        {
            var loAss = MatchAddSub(loInstr);
            var hiAss = MatchAdcSbc(hiInstr);
            if (loAss == null || hiAss == null)
                return null;
            if (loAss.Op != hiAss.Op)
                return null;
            var totalSize = PrimitiveType.Create(Domain.SignedInt | Domain.UnsignedInt, loAss.Dst.DataType.Size + loAss.Dst.DataType.Size);
            var left = MakeMatch(loAss.Left, hiAss.Left, totalSize, false);
            var right = MakeMatch(loAss.Right, hiAss.Right, totalSize, false);
            var dst = MakeMatch(loAss.Dst, hiAss.Dst, totalSize, true);

            if (left != null && right != null && dst != null)
                return CreateInstruction(dst, loAss.Op, left, right);
            else
                return null;
        }

        public class CondMatch
        {
            public Expression src;
            public Identifier FlagGroup;
            public int StatementIndex;
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


        public Expression Src
        {
            get { return src; }
        }

        public Expression Dst
        {
            get { return dst; }
        }

        public bool MemoryOperandsAdjacent(MemoryAccess m1, MemoryAccess m2)
        {
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
            if (!IsAddOrSub(op))
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
            if (!IsAddOrSub(op))
                return null;
            return new AddSubCandidate
            {
                Dst = addPattern.CapturedExpressions("dst"),
                Op = op,
                Left = addPattern.CapturedExpressions("left"),
                Right = addPattern.CapturedExpressions("right")
            };
        }

        private static bool IsAddOrSub(Operator op)
        {
            return (op == Operator.Add || op == Operator.Sub);
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
