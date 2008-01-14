/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Core.Types;
using System;
using System.Text;

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Models an X86 instruction.
	/// </summary>
	public class IntelInstruction
	{
		public FlagM DefCc()  { return DefCc(code); }		// Condition codes defined by this instruction.
		public FlagM UseCc()  { return UseCc(code); }		// Condition codes used by this instruction.

		public Opcode code;		// Opcode of the instruction.
		public byte	cOperands;
		public PrimitiveType dataWidth;	// Width of the data (if it's a word).
		public PrimitiveType addrWidth;	// width of the address mode.	// TODO: belongs in MemoryOperand

		public Operand		op1;
		public Operand		op2;
		public Operand		op3;

		public IntelInstruction()
		{
		}

		public IntelInstruction(Opcode code, PrimitiveType dataWidth, PrimitiveType addrWidth, params Operand [] ops)
		{
			this.code = code;
			this.dataWidth = dataWidth;
			this.addrWidth = addrWidth;
			this.cOperands = (byte) ops.Length;
			if (ops.Length >= 1)
			{
				op1 = ops[0];
				if (ops.Length >= 2)
				{
					op2 = ops[1];
					if (ops.Length == 3)
						op3 = ops[2];
					else if (ops.Length >= 4)
						throw new ArgumentException("Too many operands.");
				}
			}
		}

		private bool NeedsExplicitMemorySize()
		{
			if (code == Opcode.movsx || code == Opcode.movzx)
				return true;
			return 
				 (cOperands < 1 || !ImplicitWidth(op1)) &&
				 (cOperands < 2 || !ImplicitWidth(op2)) &&
				 (cOperands < 3 || !ImplicitWidth(op3));
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// Get opcode. 

			string s = code.ToString();
			switch (code)
			{
			case Opcode.cwd:
				if (dataWidth == PrimitiveType.Word32)
				{
					s = "cdq";
				}
				break;
			case Opcode.cbw:
				if (dataWidth == PrimitiveType.Word32)
				{
					s = "cwde";
				}
				break;
			case Opcode.ins:
			case Opcode.outs:
			case Opcode.movs:
			case Opcode.cmps:
			case Opcode.stos:
			case Opcode.lods:
			case Opcode.scas:
				switch (dataWidth.Size)
				{
				case 1: s += 'b'; break;
				case 2: s += 'w'; break;
				case 4: s += 'd'; break;
				default: throw new ArgumentOutOfRangeException();
				}
				break;
			}
			sb.Append(s);

			sb.Append('\t');

			bool fExplicit = NeedsExplicitMemorySize();

			if (cOperands >= 1)
			{
				sb.Append(op1.ToString());
				if (cOperands >= 2)
				{
					sb.Append(',');
					sb.Append(op2.ToString(fExplicit));
					if (cOperands >= 3)
					{
						sb.Append(",");
						sb.Append(op3.ToString(fExplicit));
					}
				}
			}
			return sb.ToString();
		}

		// Returns the condition codes that an instruction modifies.

		public static FlagM DefCc(Opcode opcode)
		{
			switch (opcode)
			{
			case Opcode.aaa:
			case Opcode.aas:
				return FlagM.CF;
			case Opcode.aad:
			case Opcode.aam:
				return FlagM.SF|FlagM.ZF;
			case Opcode.bt:
			case Opcode.bts:
			case Opcode.btc:
			case Opcode.btr:
				return FlagM.CF;
			case Opcode.clc:
			case Opcode.cmc:
			case Opcode.stc:
				return FlagM.CF;
			case Opcode.cld:
			case Opcode.std:
				return FlagM.DF;
			case Opcode.daa:
			case Opcode.das:
				return FlagM.CF|FlagM.SF|FlagM.ZF;
			case Opcode.adc:
			case Opcode.add:
			case Opcode.sbb:
			case Opcode.sub:
			case Opcode.cmp:
			case Opcode.cmpsb:
			case Opcode.cmps:
			case Opcode.div:
			case Opcode.idiv:
			case Opcode.imul:
			case Opcode.mul:
			case Opcode.neg:
			case Opcode.scas:
			case Opcode.scasb:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.dec:
			case Opcode.inc:
				return FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.rcl:
			case Opcode.rcr:
			case Opcode.rol:
			case Opcode.ror:
				return FlagM.OF|FlagM.CF;		// if shift count > 1, FlagM.OF is not set.
			case Opcode.sar:
			case Opcode.shl:
			case Opcode.shr:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;	// if shift count > 1, FlagM.OF is not set.
			case Opcode.shld:
			case Opcode.shrd:
				return FlagM.SF|FlagM.ZF|FlagM.CF;
			case Opcode.bsf:
			case Opcode.bsr:
				return FlagM.ZF;
			case Opcode.and:
			case Opcode.or:
			case Opcode.test:
			case Opcode.xor:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;
			case Opcode.sahf:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;
			default:
				return 0;
			}
		}

		// Returns the condition codes an instruction uses.

		public static FlagM UseCc(Opcode opcode)
		{
			switch (opcode)
			{
			case Opcode.adc:
			case Opcode.sbb:
				return FlagM.CF;
			case Opcode.daa:
			case Opcode.das:
				return FlagM.CF;
			case Opcode.ins:
			case Opcode.insb:
			case Opcode.lods:
			case Opcode.lodsb:
			case Opcode.movs:
			case Opcode.movsb:
			case Opcode.outs:
			case Opcode.outsb:
			case Opcode.scas:
			case Opcode.scasb:
			case Opcode.stos:
			case Opcode.stosb:
				return FlagM.DF;
			case Opcode.ja:
			case Opcode.seta:
				return FlagM.CF|FlagM.ZF;
			case Opcode.jbe:
			case Opcode.setbe:
				return FlagM.CF|FlagM.ZF;
			case Opcode.jc:
			case Opcode.setc:
				return FlagM.CF;
			case Opcode.jg:
			case Opcode.setg:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
			case Opcode.jge:
			case Opcode.setge:
				return FlagM.SF|FlagM.OF;
			case Opcode.jl:
			case Opcode.setl:
				return FlagM.SF|FlagM.OF;
			case Opcode.jle:
			case Opcode.setle:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
			case Opcode.jnc:
			case Opcode.setnc:
				return FlagM.CF;
			case Opcode.jno:
			case Opcode.setno:
				return FlagM.OF;
			case Opcode.jns:
			case Opcode.setns:
				return FlagM.SF;
			case Opcode.jnz:
			case Opcode.setnz:
				return FlagM.ZF;
			case Opcode.jo:
			case Opcode.seto:
				return FlagM.OF;
			case Opcode.jpe:
			case Opcode.setpe:
			case Opcode.jpo:
			case Opcode.setpo:
				throw new ArgumentOutOfRangeException("NYI");
			case Opcode.js:
			case Opcode.sets:
				return FlagM.SF;
			case Opcode.jz:
			case Opcode.setz:
				return FlagM.ZF;
			case Opcode.lahf:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.loope:
			case Opcode.loopne:
				return FlagM.ZF;
			case Opcode.rcl:
			case Opcode.rcr:
				return FlagM.CF;
			default:
				return 0;
			}
		}

		private bool ImplicitWidth(Operand op)
		{
			return op is RegisterOperand || op is AddressOperand || op is FpuOperand;
		}
	}
}
