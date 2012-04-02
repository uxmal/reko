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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Scanning
{
    public class LongAddRewriter
    {
        private Frame frame;
        private Expression dst;
        private Expression src;
        private bool useStore;
        private IProcessorArchitecture arch;

        public LongAddRewriter(IProcessorArchitecture arch,  Frame frame)
        {
            this.arch = arch;
            this.frame = frame;
        }

        public void EmitInstruction(BinaryOperator op, CodeEmitter emitter)
        {
            var b = new BinaryExpression(op, dst.DataType, dst, src);
            if (useStore)
            {
                emitter.Store((MemoryAccess)dst.CloneExpression(), b);
            }
            else
            {
                emitter.Assign((Identifier)dst, b);
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
                if (bin.op == next)
                    return i;
                var grfDef = ass.Dst.Storage as FlagGroupStorage;
                if (grfDef == null)
                    continue;
                if ((arch.CarryFlagMask & grfDef.FlagGroupBits) != 0)
                    return -1;
            }
            return -1;
        }

        public Expression MakeMatch(Expression opLo, Expression opHi, DataType totalSize, bool isDef)
        {
            var regDstLo = opLo as Identifier;
            var regDstHi = opHi as Identifier;
            if (regDstLo != null && regDstHi != null)
            {
                if (isDef)
                    useStore = false;
                return frame.EnsureSequence(regDstHi, regDstLo, totalSize);
            }
            //MemoryOperand memDstLo = opLo as MemoryOperand;
            //MemoryOperand memDstHi = opHi as MemoryOperand;
            //if (memDstLo != null && memDstHi != null && MemoryOperandsAdjacent(memDstLo, memDstHi))
            //{
            //    if (isDef)
            //        useStore = true;
            //    throw new NotImplementedException("The <null> on the next line needs an IntelState, not an IntelRewriterState.");
            //    return orw.CreateMemoryAccess(memDstLo, totalSize, null);
            //}
            var immLo = opLo as Constant;
            var immHi = opHi as Constant;
            if (immLo != null && immHi != null)
            {
                return new Constant(totalSize, ((ulong)immHi.ToUInt32() << opLo.DataType.BitSize) | immLo.ToUInt32());
            }
            return null;
        }

        public bool Match(Instruction loInstr, Instruction hiInstr)
        {
            var loAss = loInstr as Assignment;
            var hiAss = loInstr as Assignment;
            if (loAss != null && hiAss != null)
            {
                var totalSize = PrimitiveType.Create(Domain.SignedInt | Domain.UnsignedInt, loAss.Dst.DataType.Size + loAss.Dst.DataType.Size);
                src = MakeMatch(loAss.Src, hiAss.Src, totalSize, false);
                dst = MakeMatch(loAss.Dst, hiAss.Dst, totalSize, true);
            }
            else
            {
                var loStore = loInstr as Store;
                var hiStore = hiInstr as Store;
                if (loStore != null && hiStore != null)
                {
                    var totalSize = PrimitiveType.Create(Domain.SignedInt | Domain.UnsignedInt, loStore.Dst.DataType.Size + loStore.Dst.DataType.Size);
                    src = MakeMatch(loStore.Src, hiStore.Src, totalSize, false);
                    dst = MakeMatch(loStore.Dst, hiStore.Dst, totalSize, true);
                    return dst != null && src != null;

                }
            }
            return dst != null && src != null;
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
            if (!off1.IsValid || !off2.IsValid)
                return false;
            return off1.ToInt32() + m1.DataType.Size == off2.ToInt32();
        }

        private Constant GetOffset(MemoryAccess access)
        {
            throw new NotImplementedException();
        }

        private Identifier AddSubDestination(Instruction instruction)
        {
            var ass = instruction as Assignment;
            if (ass == null)
                return null;
            var bin = ass.Src as BinaryExpression;
            if (bin == null || (bin.op != Operator.Add && bin.op != Operator.Sub))
                return null;
            return ass.Dst;
        }

        public IEnumerable<CarryLinkedInstructions> FindCarryLinkedInstructions(Block block)
        {
            throw new NotImplementedException();
        }
    }

    public class CarryLinkedInstructions
    {
        public Instruction High;
        public Instruction Low;
    }
}
