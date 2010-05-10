/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Arch.Intel
{
	public class LongAddRewriter
	{
		private Frame frame;
		private OperandRewriter orw;
		private IntelRewriterState state;

		private Expression dst;
		private Expression src;
		private bool useStore;

		public LongAddRewriter(Frame frame, OperandRewriter orw, IntelRewriterState state)
		{
			this.frame = frame;
			this.orw = orw;
			this.state = state;
		}

        public void EmitInstruction(BinaryOperator op, CodeEmitter emitter)
        {
            BinaryExpression b = new BinaryExpression(op, dst.DataType, dst, src);
            if (useStore)
            {
                emitter.Store((MemoryAccess) dst.CloneExpression(), b);
            }
            else
                emitter.Assign((Identifier) dst, b);
        }

		/// <summary>
		/// Determines if the carry flag reaches a using instruction.
		/// </summary>
		/// <param name="instrs"></param>
		/// <param name="i"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		public int IndexOfUsingOpcode(IntelInstruction [] instrs, int i, Opcode next)
		{
			for (++i; i < instrs.Length; ++i)
			{
				if (instrs[i].code == next)
					return i;
				if ((instrs[i].DefCc() & (uint) FlagM.CF) != 0)
					break;
			}
			return -1;
		}

		public Expression MakeMatch(MachineOperand opLo, MachineOperand opHi, DataType totalSize, bool isDef)
		{
			RegisterOperand regDstLo = opLo as RegisterOperand;
			RegisterOperand regDstHi = opHi as RegisterOperand;
			if (regDstLo != null && regDstHi != null)
			{
				if (isDef)
					useStore = false;
				return frame.EnsureSequence(
					orw.AluRegister(regDstHi.Register),
					orw.AluRegister(regDstLo.Register), 
					totalSize);
			}
			MemoryOperand memDstLo = opLo as MemoryOperand;
			MemoryOperand memDstHi = opHi as MemoryOperand;
			if (memDstLo != null && memDstHi != null && MemoryOperandsAdjacent(memDstLo, memDstHi))
			{
				if (isDef)
					useStore = true;
				return orw.CreateMemoryAccess(memDstLo, totalSize, state);
			}
			ImmediateOperand immLo = opLo as ImmediateOperand;
			ImmediateOperand immHi = opHi as ImmediateOperand;
			if (immLo != null && immHi != null)
			{
				return new Constant(totalSize, ((ulong)immHi.Value.ToUInt32() << opLo.Width.BitSize) | immLo.Value.ToUInt32());
			}

			return null;
		}

		public bool Match(IntelInstruction loInstr, IntelInstruction hiInstr)
		{
			DataType totalSize = PrimitiveType.Create(Domain.SignedInt|Domain.UnsignedInt, loInstr.dataWidth.Size + hiInstr.dataWidth.Size);
			src = MakeMatch(loInstr.op2, hiInstr.op2, totalSize, false);
			dst = MakeMatch(loInstr.op1, hiInstr.op1, totalSize, true);
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

		public bool MemoryOperandsAdjacent(MemoryOperand m1, MemoryOperand m2)
		{
			long off = 0;
			if (m1.Offset.IsValid)
			{
				off = m1.Offset.ToInt32();
			}
			long off2 = m2.Offset.ToInt32();
			return off + m1.Width.Size == off2;
		}
	}
}
