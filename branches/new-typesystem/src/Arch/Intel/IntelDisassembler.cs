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
using System.Diagnostics;

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Intel x86 opcode disassembler 
	/// </summary>
	public class IntelDisassembler : Disassembler
	{
		private PrimitiveType m_dataWidth;
		private PrimitiveType m_addressWidth;
		private PrimitiveType m_dataWidthSeg;
		private PrimitiveType m_addressWidthSeg;
		private byte m_modrm;
		private bool isModrmValid;
		private IntelRegister m_segOverride;
		private ImageReader	rdr;
		private OpRec [] m_aopRecs;

		/// <summary>
		/// Creates a disassember that uses the specified reader to fetch bytes from the program image.
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 16-bit operation, PrimitiveType.Word32
		/// for 32-bit operation.</param>
		public IntelDisassembler(ImageReader rdr, PrimitiveType width)
		{
			this.rdr = rdr;
			m_dataWidthSeg = width;
			m_addressWidthSeg = width;
		}

		private static IntelRegister RegFromBits(int bits, PrimitiveType dataWidth)
		{
			bits = bits & 0x7;
			if (dataWidth == PrimitiveType.Byte)
			{
				switch (bits)
				{
				case 0: return Registers.al;
				case 1: return Registers.cl;
				case 2: return Registers.dl;
				case 3: return Registers.bl;
				case 4: return Registers.ah;
				case 5: return Registers.ch;
				case 6: return Registers.dh;
				case 7: return Registers.bh;
				}
			}
			else if (dataWidth == PrimitiveType.Word16)
			{
				switch (bits)
				{
				case 0: return Registers.ax;
				case 1: return Registers.cx;
				case 2: return Registers.dx;
				case 3: return Registers.bx;
				case 4: return Registers.sp;
				case 5: return Registers.bp;
				case 6: return Registers.si;
				case 7: return Registers.di;
				}
			}
			else if (dataWidth == PrimitiveType.Word32)
			{
				switch (bits)
				{
				case 0: return Registers.eax;
				case 1: return Registers.ecx;
				case 2: return Registers.edx;
				case 3: return Registers.ebx;
				case 4: return Registers.esp;
				case 5: return Registers.ebp;
				case 6: return Registers.esi;
				case 7: return Registers.edi;
				}
			}
			throw new ArgumentOutOfRangeException("Unsupported data width: " + dataWidth.ToString());
		}

		public static IntelRegister SegFromBits(int bits)
		{
			switch (bits&0x7)
			{
			case 0: return Registers.es;
			case 1: return Registers.cs;
			case 2: return Registers.ss;
			case 3: return Registers.ds;
			case 4: return Registers.fs;
			case 5: return Registers.gs;
			case 6: return Registers.ss;
			case 7: return Registers.ds;
			}
			throw new ArgumentOutOfRangeException("bits", string.Format("{0} doesn't correspond to a segment register.", bits));
		}

		[Flags]
		private enum OpFlag
		{
			None = 0,
			GRP_1 = 0x0001,
			GRP_2 =	0x0002,
			GRP_3 =	0x0003,
			GRP_4 =	0x0004,
			GRP_5 =	0x0005,
			GRP_8 = 0x0008,

			GRP_MASK = 0x000F,

			FP = 0x0080,
			X =  0x8000
		}

		/// <summary>
		/// Simple opcode record.
		/// </summary>
		private class OpRec
		{
			public Opcode	opcode;
			public string	Format;
			public OpFlag	Flags;
			public OpRec [] indirect;

			public OpRec(Opcode op, string fmt)
			{
				opcode = op;
				Format = fmt;
				Flags = OpFlag.None;
				indirect = null;
			}

			public OpRec(Opcode op, string fmt, OpFlag flags)
			{
				opcode = op;
				Format = fmt;
				Flags = flags;
			}
		}

	
		/// <summary>
		/// Current address of the disassembler.
		/// </summary>
		public override Address Address
		{
			get { return rdr.Address; }
		}

		/// <summary>
		/// If the ModR/M byte hasn't been read yet, do so now.
		/// </summary>
		/// <returns></returns>
		private byte EnsureModRM()
		{
			if (!isModrmValid)
			{
				m_modrm = rdr.ReadByte();
				isModrmValid = true;
			}
			return m_modrm;
		}


		/// <summary>
		/// Consumes prefixes that alter the operation of the instruction, such as 
		/// widening/narrowing the instruction data width and segment overrides.
		/// </summary>
		/// <returns></returns>

		private byte ConsumePrefixes()
		{
			for (;;)
			{
				byte op = rdr.ReadByte();
				switch (op)
				{
				case 0x0F:		// 0F: prefix for two-byte opcodes.
					m_aopRecs = s_aOpRec0F;
					return rdr.ReadByte();
				case 0x26:		// segment overrides
				case 0x2E:
				case 0x36:
				case 0x3E:
					m_segOverride = SegFromBits((op >> 3) & 0x03);
					break;
				case 0x64:
					m_segOverride = Registers.fs;
					break;
				case 0x65:
					m_segOverride = Registers.gs;
					break;
				case 0x66:		// 66: changes the width of the data operands.
					m_dataWidth = (m_dataWidth == PrimitiveType.Word16)
						? PrimitiveType.Word32
						: PrimitiveType.Word16;
					break;
				case 0x67:		// 67: changes the width of the address operands 
					m_addressWidth = (m_addressWidth == PrimitiveType.Word16)
						? PrimitiveType.Word32
						: PrimitiveType.Word16;
					break;
				default:
					return op;
				}
			}
		}	

		/// <summary>
		/// Disassembles the current instruction. The address is incremented
		/// to point at the first address after the instruction and returned to the caller.
		/// </summary>
		/// <returns>A single disassembled instruction.</returns>
		public virtual IntelInstruction Disassemble()
		{
			byte op;
			OpRec 	opRec;
			m_dataWidth = m_dataWidthSeg;
			m_addressWidth = m_addressWidthSeg;
			isModrmValid = false;
			m_segOverride = Registers.None;
			m_aopRecs = s_aOpRec;
			IntelInstruction pInstr = new IntelInstruction();

			// Fetch the opcode description.

			op = ConsumePrefixes();
			opRec = m_aopRecs[op];
			string strFormat = opRec.Format;

			// Group instructions need to be handled separately.

			if ((opRec.Flags & OpFlag.GRP_MASK) != 0)
			{
				int grp = (int)(opRec.Flags & OpFlag.GRP_MASK) - 1;
				opRec = s_aOpRecGrp[grp * 8 + ((EnsureModRM() >> 3) & 0x07)];
				if (opRec.Format != null)
				{
					if (strFormat != null)
					{
						strFormat = strFormat + opRec.Format;
					}
					else
					{
						strFormat = opRec.Format;
					}
				}
			}

			// Floating-point instructions need to be handled separately.

			int iOpRec;
			if ((opRec.Flags & OpFlag.FP) != 0)
			{
				byte modRM = EnsureModRM();
				iOpRec = (op & 0x07) * 0x48;
				if (modRM < 0xC0)
				{
					opRec = s_aFpOpRec[iOpRec + ((modRM >> 3) & 0x07)];
				}
				else
				{
					opRec = s_aFpOpRec[iOpRec + modRM - 0xB8];
				}
				strFormat = opRec.Format;
			}

			if (opRec.opcode == Opcode.illegal)
			{
				pInstr.cOperands = 0;
				pInstr.code = Opcode.illegal;
				return pInstr;
			}

			pInstr.code = opRec.opcode;
			pInstr.dataWidth = m_dataWidth;
			pInstr.addrWidth = m_addressWidth;

			// Now decode instruction format.

			int iOp = 0;
			Operand pOperand;
			PrimitiveType width = null;
			int i = 0;
			while (i != strFormat.Length)
			{
				if (strFormat[i] == ',')
					++i;

				pOperand = null;
				ImmediateOperand immOp;
				MemoryOperand memOp;
				AddressOperand addrOp;
				int offset;

				char chFmt = strFormat[i++];
				switch (chFmt)
				{
				case '1':
					pOperand = immOp = new ImmediateOperand(PrimitiveType.Byte, 1);
					break;
				case '3':
					pOperand = immOp = new ImmediateOperand(PrimitiveType.Byte, 3);
					break;
				case 'A':		// Absolute memory address.
					++i;
					ushort off = rdr.ReadUShort();
					ushort seg = rdr.ReadUShort();
					pOperand = addrOp = new AddressOperand(new Address(seg, off));
					break;
				case 'E':		// memory or register operand specified by mod & r/m fields.
					width = OperandWidth(strFormat[i++]);
					pOperand = DecodeModRM(width, m_segOverride);
					break;
				case 'G':		// register operand specified by the reg field of the modRM byte.
					width = OperandWidth(strFormat[i++]);
					pOperand = new RegisterOperand(RegFromBits(EnsureModRM() >> 3, width));
					break;
				case 'I':		// Immediate operand.
					if (strFormat[i] != 'x')
					{
						//  Don't use the width of the previous operand.
						width = OperandWidth(strFormat[i]);
					}
					++i;
					pOperand = CreateImmediateOperand(width, pInstr.dataWidth);
					break;
				case 'J':		// Relative jump.
					width = OperandWidth(strFormat[i++]);
					offset = rdr.ReadSigned(width);
					pOperand = new ImmediateOperand(m_dataWidthSeg, (uint)(rdr.Address.off + offset));
					break;
				case 'M':		// modRM may only refer to memory.
					width = OperandWidth(strFormat[i++]);
					pOperand = DecodeModRM(m_dataWidth, m_segOverride);
					break;
				case 'O':		// Offset of the operand is encoded directly after the opcode.
					width = OperandWidth(strFormat[i++]);
					pOperand = memOp = new MemoryOperand(
						width, 
						new Value(
							m_addressWidth,
							rdr.ReadUnsigned(m_addressWidth)));
					memOp.SegOverride = m_segOverride;
					break;
				case 'S':		// Segment register encoded by reg field of modRM byte.
					Debug.Assert(strFormat[i++] == 'w');
					pOperand = new RegisterOperand(SegFromBits(EnsureModRM() >> 3));
					break;
				case 'a':		// Implicit use of accumulator.
					pOperand = new RegisterOperand(RegFromBits(0, OperandWidth(strFormat[i++])));
					break;
				case 'b':
					pInstr.dataWidth = PrimitiveType.Byte;
					pOperand = null;
					break;
				case 'c':		// Implicit use of CL.
					pOperand = new RegisterOperand(Registers.cl);
					break;
				case 'd':		// Implicit use of DX or EDX.
					width = OperandWidth(strFormat[i++]);
					pOperand = new RegisterOperand(RegFromBits(2, width));
					break;
				case 'r':		// Register encoded as last 3 bits of instruction.
					width = OperandWidth(strFormat[i++]);
					pOperand = new RegisterOperand(RegFromBits(op, width));
					break;
				case 's':		// Segment encoded as next byte of the format string.
					pOperand = new RegisterOperand(SegFromBits(strFormat[i++] - '0'));
					break;
				case 'F':		// Floating-point ST(x)
					pOperand = new FpuOperand(EnsureModRM() & 0x07);
					break;
				case 'f':		// ST(0)
					pOperand = new FpuOperand(0);
					break;
				default:
					throw new ArgumentOutOfRangeException("unknown format specifier: " + chFmt.ToString());
				}
				switch (iOp)
				{
				case 0: pInstr.op1 = pOperand; break;
				case 1: pInstr.op2 = pOperand; break;
				case 2: pInstr.op3 = pOperand; break;
				default: throw new ArgumentOutOfRangeException();
				}

				if (pOperand != null)
					++iOp;
			}

			pInstr.cOperands = (byte) iOp;
			return pInstr;
		}

		/// <summary>
		/// Returns the operand width of the operand type.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private PrimitiveType OperandWidth(char ch)
		{
			switch (ch)
			{
			default:
				throw new ArgumentOutOfRangeException();
			case 'b':
				m_dataWidth = PrimitiveType.Byte;
				break;
			case 'v':
				break;
			case 'w':
				m_dataWidth = PrimitiveType.Word16;
				break;
			case 'd':
				m_dataWidth = PrimitiveType.Word32;
				break;
			case 'p':
				m_dataWidth = PrimitiveType.Pointer32;		// Far pointer.
				break;
			case 'f':
				m_dataWidth = PrimitiveType.Real32;
				break;
			case 'g':
				m_dataWidth = PrimitiveType.Real64;
				break;
			case 'h':
				m_dataWidth = PrimitiveType.Real80;
				break;
			case 'q':
				m_dataWidth = PrimitiveType.Word64;
				break;
			}
			return m_dataWidth;
		}

		public override object DisassembleInstruction()
		{
			return Disassemble();
		}

		private static IntelRegister [] s_ma16Base = 
		{
			Registers.bx,
			Registers.bx,
			Registers.bp,
			Registers.bp,
			Registers.si,
			Registers.di,
			Registers.bp,
			Registers.bx,
		};

		private static IntelRegister [] s_ma16Index =
		{
			Registers.si,	 
			Registers.di,	 
			Registers.si,	 
			Registers.di,	 
			Registers.None,
			Registers.None,
			Registers.None,
			Registers.None,
		};


		public ImmediateOperand CreateImmediateOperand(PrimitiveType immWidth, PrimitiveType instrWidth)
		{
			return new ImmediateOperand(immWidth, rdr.ReadUnsigned(immWidth));
		}

		private Operand DecodeModRM(PrimitiveType dataWidth, IntelRegister segOverride)
		{
			EnsureModRM();

			int  rm = m_modrm & 0x07;
			int  mod = m_modrm >> 6;

			IntelRegister b;
			IntelRegister idx;
			byte scale = 1;
			PrimitiveType offsetWidth = null;

			if (m_addressWidth == PrimitiveType.Word16)
			{
				// 16-bit addressing modes are weird.

				b = s_ma16Base[rm];
				idx = s_ma16Index[rm];
				scale = 1;

				switch (mod)
				{
				case 0:
					if (rm == 0x06)
					{
						offsetWidth = PrimitiveType.Word16;
						b = Registers.None;
						idx = Registers.None;
					}
					else
					{
						offsetWidth = null;
					}
					break;
				case 1:
					offsetWidth = PrimitiveType.SByte;
					break;
				case 2:
					offsetWidth = PrimitiveType.Word16;
					break;
				case 3:
					return new RegisterOperand(RegFromBits(rm, dataWidth));
				}
			}
			else 
			{
				b = RegFromBits(rm, m_addressWidth);
				idx = Registers.None;

				switch (mod)
				{
				case 0:
					if (rm == 0x05)
					{
						offsetWidth = PrimitiveType.Word32;
						b = Registers.None;
					}
					else
					{
						offsetWidth = null;
					}
					break;
				case 1:
					offsetWidth = PrimitiveType.SByte;
					break;
				case 2:
					offsetWidth = PrimitiveType.Word32;
					break;
				case 3:
					return new RegisterOperand(RegFromBits(rm, dataWidth));
				}

				// Handle possible s-i-b byte.

				if (rm == 0x04)
				{
					// We have SIB'ness, your majesty!

					byte sib = rdr.ReadByte();
					if (((m_modrm & 0xC0) == 0) && ((sib & 0x7) == 5))
					{
						offsetWidth = PrimitiveType.Word32;
						b = Registers.None;
					}
					else
					{
						b = RegFromBits(sib, m_addressWidth);
					}
			
					int i = (sib >> 3) & 0x7;
					idx = (i == 0x04) ? Registers.None : RegFromBits(i, m_addressWidth);
					scale = (byte) (1 << (sib >> 6));
				}
			}

			// Now fetch the offset if there was any.

			Value offset = null;
			if (offsetWidth == PrimitiveType.SByte)
			{
				offset = new Value(offsetWidth, (sbyte) rdr.ReadSigned(offsetWidth));
			}
			else if (offsetWidth != null)
			{
				offset = new Value(offsetWidth, rdr.ReadUnsigned(offsetWidth));
			}
			else
				offset = Value.Invalid;

			MemoryOperand memOp = new MemoryOperand(dataWidth, offset);
			memOp.Base = b;
			memOp.Index = idx;
			memOp.Scale = scale;
			memOp.SegOverride = segOverride;
			return memOp;
		}


		private bool ImplicitWidth(Operand op)
		{
			return op is RegisterOperand || op is AddressOperand;
		}

		// A - direct address
		// E - modrm specifies the argument
		// G - reg part of modrm specifies the argument, which is a reg.
		// J - relative from offset.
		// r - register is encoded in the last 3 bits of the instruction.

		// 'X' means the opcode hasn't been tested in the disassembler.


		private static OpRec [] s_aOpRec;
		private static OpRec [] s_aOpRec0F;
		private static OpRec [] s_aOpRecGrp;
		private static OpRec [] s_aFpOpRec;

		static IntelDisassembler()
		{
			s_aOpRec = new OpRec[] { 
				// 00
				new OpRec(Opcode.add, "Eb,Gb"),
				new OpRec(Opcode.add, "Ev,Gv"),
				new OpRec(Opcode.add, "Gb,Eb"),
				new OpRec(Opcode.add, "Gv,Ev"),
				new OpRec(Opcode.add, "ab,Ib"),
				new OpRec(Opcode.add, "av,Iv"),
				new OpRec(Opcode.push, "s0"),
				new OpRec(Opcode.pop, "s0"),

				new OpRec(Opcode.or, "Eb,Gb"),
				new OpRec(Opcode.or, "Ev,Gv"),
				new OpRec(Opcode.or, "Gb,Eb"),
				new OpRec(Opcode.or, "Gv,Ev"),
				new OpRec(Opcode.or, "ab,Ib"),
				new OpRec(Opcode.or, "av,Iv"),
				new OpRec(Opcode.push, "s1"),
				new OpRec(Opcode.illegal, null),

				// 10
				new OpRec(Opcode.adc, "Eb,Gb"),
				new OpRec(Opcode.adc, "Ev,Gv"),
				new OpRec(Opcode.adc, "Gb,Eb"),
				new OpRec(Opcode.adc, "Gv,Ev"),
				new OpRec(Opcode.adc, "ab,Ib"),
				new OpRec(Opcode.adc, "av,Iv"),
				new OpRec(Opcode.push, "s2"),
				new OpRec(Opcode.pop, "s2"),

				new OpRec(Opcode.sbb, "Eb,Gb"),
				new OpRec(Opcode.sbb, "Ev,Gv"),
				new OpRec(Opcode.sbb, "Gb,Eb"),
				new OpRec(Opcode.sbb, "Gv,Ev"),
				new OpRec(Opcode.sbb, "ab,Ib"),
				new OpRec(Opcode.sbb, "av,Iv"),
				new OpRec(Opcode.push, "s3"),
				new OpRec(Opcode.pop, "s3"),

				// 20
				new OpRec(Opcode.and, "Eb,Gb"), 
				new OpRec(Opcode.and, "Ev,Gv"),
				new OpRec(Opcode.and, "Gb,Eb"),
				new OpRec(Opcode.and, "Gv,Ev"),
				new OpRec(Opcode.and, "ab,Ib"),
				new OpRec(Opcode.and, "av,Iv"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.daa, ""),

				new OpRec(Opcode.sub, "Eb,Gb"),
				new OpRec(Opcode.sub, "Ev,Gv"),
				new OpRec(Opcode.sub, "Gb,Eb"),
				new OpRec(Opcode.sub, "Gv,Ev"),
				new OpRec(Opcode.sub, "ab,Ib"),
				new OpRec(Opcode.sub, "av,Iv"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.das, ""),

				// 30
				new OpRec(Opcode.xor, "Eb,Gb"),
				new OpRec(Opcode.xor, "Ev,Gv"),
				new OpRec(Opcode.xor, "Gb,Eb"),
				new OpRec(Opcode.xor, "Gv,Ev"),
				new OpRec(Opcode.xor, "ab,Ib"),
				new OpRec(Opcode.xor, "av,Iv"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.aaa, ""),

				new OpRec(Opcode.cmp, "Eb,Gb"),
				new OpRec(Opcode.cmp, "Ev,Gv"),
				new OpRec(Opcode.cmp, "Gb,Eb"),
				new OpRec(Opcode.cmp, "Gv,Ev"),
				new OpRec(Opcode.cmp, "ab,Ib"),
				new OpRec(Opcode.cmp, "av,Iv"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.aas, ""),

				// 40
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),
				new OpRec(Opcode.inc, "rv"),

				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),
				new OpRec(Opcode.dec, "rv"),

				// 50
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),
				new OpRec(Opcode.push, "rv"),

				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),
				new OpRec(Opcode.pop, "rv"),

				// 60
				new OpRec(Opcode.pusha, ""),
				new OpRec(Opcode.popa, ""),
				new OpRec(Opcode.bound, "Gv,Mv", OpFlag.X),
				new OpRec(Opcode.arpl, "Ew,rw"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.push, "Iv"),
				new OpRec(Opcode.imul, "Gv,Ev,Iv"),
				new OpRec(Opcode.push, "Ib"),
				new OpRec(Opcode.imul, "Gv,Ev,Ib"),
				new OpRec(Opcode.insb, "b"),
				new OpRec(Opcode.ins,  ""),
				new OpRec(Opcode.outsb,"b"),
				new OpRec(Opcode.outs, ""),

				// 70
				new OpRec(Opcode.jo, "Jb"),
				new OpRec(Opcode.jno, "Jb"),
				new OpRec(Opcode.jc, "Jb"),
				new OpRec(Opcode.jnc, "Jb"),
				new OpRec(Opcode.jz, "Jb"),
				new OpRec(Opcode.jnz, "Jb"),
				new OpRec(Opcode.jbe, "Jb"),
				new OpRec(Opcode.ja, "Jb"),

				new OpRec(Opcode.js, "Jb"),
				new OpRec(Opcode.jns, "Jb"),
				new OpRec(Opcode.jpe, "Jb"),
				new OpRec(Opcode.jpo, "Jb"),
				new OpRec(Opcode.jl, "Jb"),
				new OpRec(Opcode.jge, "Jb"),
				new OpRec(Opcode.jle, "Jb"),
				new OpRec(Opcode.jg, "Jb"),

				// 80
				new OpRec(Opcode.illegal, "Eb,Ib", OpFlag.GRP_1),
				new OpRec(Opcode.illegal, "Ev,Iv", OpFlag.GRP_1),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, "Ev,Ib", OpFlag.GRP_1),
				new OpRec(Opcode.test, "Eb,Gb"),
				new OpRec(Opcode.test, "Ev,Gv"),
				new OpRec(Opcode.xchg, "Eb,Gb"),
				new OpRec(Opcode.xchg, "Ev,Gv"),

				new OpRec(Opcode.mov, "Eb,Gb"),
				new OpRec(Opcode.mov, "Ev,Gv"),
				new OpRec(Opcode.mov, "Gb,Eb"),
				new OpRec(Opcode.mov, "Gv,Ev"),
				new OpRec(Opcode.mov, "Ew,Sw"),
				new OpRec(Opcode.lea, "Gv,Mv"),
				new OpRec(Opcode.mov, "Sw,Ew"),
				new OpRec(Opcode.pop, "Ev"),

				// 90
				new OpRec(Opcode.nop, ""),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),
				new OpRec(Opcode.xchg, "av,rv"),

				new OpRec(Opcode.cbw, ""),
				new OpRec(Opcode.cwd, ""),
				new OpRec(Opcode.call, "Ap"),
				new OpRec(Opcode.wait, ""),
				new OpRec(Opcode.pushf, ""),
				new OpRec(Opcode.popf, ""),
				new OpRec(Opcode.sahf, ""),
				new OpRec(Opcode.lahf, ""),

				// A0
				new OpRec(Opcode.mov, "ab,Ob"),
				new OpRec(Opcode.mov, "av,Ov"),
				new OpRec(Opcode.mov, "Ob,ab"),
				new OpRec(Opcode.mov, "Ov,av"),
				new OpRec(Opcode.movsb, "b"),
				new OpRec(Opcode.movs, ""),
				new OpRec(Opcode.cmpsb, "b"),
				new OpRec(Opcode.cmps, ""),

				new OpRec(Opcode.test, "ab,Ib"),
				new OpRec(Opcode.test, "av,Iv"),
				new OpRec(Opcode.stosb, "b"),
				new OpRec(Opcode.stos, ""),
				new OpRec(Opcode.lodsb, "b"),
				new OpRec(Opcode.lods, ""),
				new OpRec(Opcode.scasb, "b"),
				new OpRec(Opcode.scas, ""),

				// B0
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),
				new OpRec(Opcode.mov, "rb,Ib"),

				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),
				new OpRec(Opcode.mov, "rv,Iv"),

				// C0
				new OpRec(Opcode.illegal, "Eb,Ib", OpFlag.GRP_2),
				new OpRec(Opcode.illegal, "Ev,Ib", OpFlag.GRP_2),
				new OpRec(Opcode.ret,	"Iw"),
				new OpRec(Opcode.ret,	""),
				new OpRec(Opcode.les,	"Gv,Mp"),
				new OpRec(Opcode.lds,	"Gv,Mp"),
				new OpRec(Opcode.mov,	"Eb,Ib"),
				new OpRec(Opcode.mov,	"Ev,Iv"),

				new OpRec(Opcode.enter, "Iw,Ib"),
				new OpRec(Opcode.leave, ""),
				new OpRec(Opcode.retf,	"Iw"),
				new OpRec(Opcode.retf,	""),
				new OpRec(Opcode.@int,	"3"),
				new OpRec(Opcode.@int,	"Ib"),
				new OpRec(Opcode.into,	"", OpFlag.X),
				new OpRec(Opcode.iret,	""),

				// D0
				new OpRec(Opcode.illegal, "Eb,1", OpFlag.GRP_2),
				new OpRec(Opcode.illegal, "Ev,1", OpFlag.GRP_2),
				new OpRec(Opcode.illegal, "Eb,c", OpFlag.GRP_2),
				new OpRec(Opcode.illegal, "Ev,c", OpFlag.GRP_2),
				new OpRec(Opcode.aam, "Ib"),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.xlat, ""),

				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),
				new OpRec(Opcode.illegal, null, OpFlag.FP),

				// E0
				new OpRec(Opcode.loopne,"Jb"),
				new OpRec(Opcode.loope, "Jb"),
				new OpRec(Opcode.loop, "Jb"),
				new OpRec(Opcode.jcxz, "Jb"),
				new OpRec(Opcode.@in, "ab,Ib"),
				new OpRec(Opcode.@in, "av,Ib", OpFlag.X),
				new OpRec(Opcode.@out, "Ib,ab"),
				new OpRec(Opcode.@out, "Ib,av", OpFlag.X),

				new OpRec(Opcode.call, "Jv"),
				new OpRec(Opcode.jmp, "Jv"),
				new OpRec(Opcode.jmp, "Ap"),
				new OpRec(Opcode.jmp, "Jb"),
				new OpRec(Opcode.@in, "ab,dw"),
				new OpRec(Opcode.@in, "av,dw", OpFlag.X),
				new OpRec(Opcode.@out, "dw,ab"),
				new OpRec(Opcode.@out, "dw,av"),

				// F0
				new OpRec(Opcode.@lock, ""),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.repne, ""),
				new OpRec(Opcode.rep, ""),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.cmc, ""),
				new OpRec(Opcode.illegal, "Eb", OpFlag.GRP_3),
				new OpRec(Opcode.illegal, "Ev", OpFlag.GRP_3),

				new OpRec(Opcode.clc, ""),
				new OpRec(Opcode.stc, ""),
				new OpRec(Opcode.cli, ""),
				new OpRec(Opcode.sti, ""),
				new OpRec(Opcode.cld, ""),
				new OpRec(Opcode.std, ""),
				new OpRec(Opcode.illegal, "", OpFlag.GRP_4),
				new OpRec(Opcode.illegal, "", OpFlag.GRP_5),
			};
		

			s_aOpRec0F = new OpRec[]
			{
				// 00
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 10
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 20
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 30
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 40
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 50
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
					
				// 60
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 70
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// 80
				new OpRec(Opcode.jo,	"Jv"),
				new OpRec(Opcode.jno,   "Jv"),
				new OpRec(Opcode.jc,	"Jv"),
				new OpRec(Opcode.jnc,	 "Jv"),
				new OpRec(Opcode.jz,	"Jv"),
				new OpRec(Opcode.jnz,   "Jv"),
				new OpRec(Opcode.jbe,   "Jv"),
				new OpRec(Opcode.ja,    "Jv"),

				new OpRec(Opcode.js,    "Jv"),
				new OpRec(Opcode.jns,   "Jv"),
				new OpRec(Opcode.jpe,   "Jv"),
				new OpRec(Opcode.jpo,   "Jv"),
				new OpRec(Opcode.jl,    "Jv"),
				new OpRec(Opcode.jge,   "Jv"),
				new OpRec(Opcode.jle,   "Jv"),
				new OpRec(Opcode.jg,    "Jv"),

				// 90
				new OpRec(Opcode.seto, "Eb", OpFlag.X),
				new OpRec(Opcode.setno, "Eb", OpFlag.X),
				new OpRec(Opcode.setc, "Eb", OpFlag.X),
				new OpRec(Opcode.setnc, "Eb", OpFlag.X),
				new OpRec(Opcode.setz, "Eb"),
				new OpRec(Opcode.setnz, "Eb"),
				new OpRec(Opcode.setbe, "Eb", OpFlag.X),
				new OpRec(Opcode.seta, "Eb", OpFlag.X),

				new OpRec(Opcode.sets,    "Eb"),
				new OpRec(Opcode.setns, "Eb", OpFlag.X),
				new OpRec(Opcode.setpe, "Eb", OpFlag.X),
				new OpRec(Opcode.setpo, "Eb", OpFlag.X),
				new OpRec(Opcode.setl, "Eb"),
				new OpRec(Opcode.setge,   "Eb"),
				new OpRec(Opcode.setle, "Eb"),
				new OpRec(Opcode.setg, "Eb"),

				// A0
				new OpRec(Opcode.push, "s4"),
				new OpRec(Opcode.pop, "s4"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.bt, "Ev,Gv"),
				new OpRec(Opcode.shld, "Ev,Gv,Ib"),
				new OpRec(Opcode.shld, "Ev,Gv,c"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.push, "s5"),
				new OpRec(Opcode.pop, "s5"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.shrd, "Ev,Gv,Ib"),
				new OpRec(Opcode.shrd, "Ev,Gv,c"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.imul, "Gv,Ev"),

				// B0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.lss, "Gv,Mp"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.lfs, "Gv,Mp"),
				new OpRec(Opcode.lgs, "Gv,Mp"),
				new OpRec(Opcode.movzx, "Gv,Eb"),
				new OpRec(Opcode.movzx, "Gv,Ew"),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, "Ev,Ib", OpFlag.GRP_8),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.bsr, "Gv,Ev"),
				new OpRec(Opcode.movsx, "Gv,Eb"),
				new OpRec(Opcode.movsx, "Gv,Ew"),

				// C0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),
				new OpRec(Opcode.bswap, "rv"),

				// D0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// E0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// F0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
			};

			s_aOpRecGrp = new OpRec[] 
			{
				// group 1
				new OpRec(Opcode.add, null),
				new OpRec(Opcode.or, null),
				new OpRec(Opcode.adc, null),
				new OpRec(Opcode.sbb, null),
				new OpRec(Opcode.and, null),
				new OpRec(Opcode.sub, null),
				new OpRec(Opcode.xor, null),
				new OpRec(Opcode.cmp, null),

				// group 2
				new OpRec(Opcode.rol, null),
				new OpRec(Opcode.ror, null),
				new OpRec(Opcode.rcl, null),
				new OpRec(Opcode.rcr, null),
				new OpRec(Opcode.shl, null),
				new OpRec(Opcode.shr, null),
				new OpRec(Opcode.shl, null),
				new OpRec(Opcode.sar, null),

				// group 3
				new OpRec(Opcode.test, "Ix"),
				new OpRec(Opcode.test, "Ix", OpFlag.X),
				new OpRec(Opcode.not, null),
				new OpRec(Opcode.neg, null),
				new OpRec(Opcode.mul, null),
				new OpRec(Opcode.imul, null),
				new OpRec(Opcode.div, null),
				new OpRec(Opcode.idiv, null),
				
				// group 4
				new OpRec(Opcode.inc, "Eb"),
				new OpRec(Opcode.dec, "Eb"),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, "", OpFlag.X),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null), 

				// group 5
				new OpRec(Opcode.inc, "Ev"),
				new OpRec(Opcode.dec, "Ev"),
				new OpRec(Opcode.call, "Ev"),
				new OpRec(Opcode.call, "Ep"),
				new OpRec(Opcode.jmp, "Ev"),
				new OpRec(Opcode.jmp, "Ep"),
				new OpRec(Opcode.push, "Ev"),
				new OpRec(Opcode.illegal, null),

				// group 6
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// group 7
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),

				// group 8
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.bt, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
			};

			s_aFpOpRec = new OpRec[]  
			{
				// D8 /////////////////////////

				new OpRec(Opcode.fadd, "Mf"),
				new OpRec(Opcode.fmul, "Mf"),
				new OpRec(Opcode.fcom, "Mf"),
				new OpRec(Opcode.fcomp, "Mf"),
				new OpRec(Opcode.fsub, "Mf"),
				new OpRec(Opcode.fsubr, "Mf"),
				new OpRec(Opcode.fdiv, "Mf"),
				new OpRec(Opcode.fdivr, "Mf"),
				// D8 C0
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F"),

				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F"),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				// D8 D0		
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				new OpRec(Opcode.fcomp, "f,F"),
				// D8 E0
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
				new OpRec(Opcode.fsub, "f,F"),
						
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				new OpRec(Opcode.fsubr, "f,F"),
				// D8 F0
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F"),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F"),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				
				// D9 ////////////////////////////////
				
				new OpRec(Opcode.fld, "Mf"),
				new OpRec(Opcode.illegal, null, OpFlag.X),
				new OpRec(Opcode.fst, "Mf"),
				new OpRec(Opcode.fstp, "Mf"),
				new OpRec(Opcode.fldenv, "Mw", OpFlag.X),
				new OpRec(Opcode.fldcw, "Mw"),
				new OpRec(Opcode.fstenv, "Mw", OpFlag.X),
				new OpRec(Opcode.fstcw, "Mw"),
						
				// D9 C0
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
				new OpRec(Opcode.fld, "F"),
						
				new OpRec(Opcode.fxch, "f,F", OpFlag.X),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
				new OpRec(Opcode.fxch, "f,F"),
						
				// D9 D0
				new OpRec(Opcode.fnop, "", OpFlag.X),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// E0
				new OpRec(Opcode.fchs, ""),
				new OpRec(Opcode.fabs, "", OpFlag.X),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.ftst, ""),
				new OpRec(Opcode.fxam, ""),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.fld1, ""),
				new OpRec(Opcode.fldl2t, "", OpFlag.X),
				new OpRec(Opcode.fldl2e, "", OpFlag.X),
				new OpRec(Opcode.fldpi, ""),
				new OpRec(Opcode.fldlg2, "", OpFlag.X),
				new OpRec(Opcode.fldln2, ""),
				new OpRec(Opcode.fldz, ""),
				new OpRec(Opcode.illegal, null),
						
				// D9 F0
				new OpRec(Opcode.f2xm1, "F,f", OpFlag.X),
				new OpRec(Opcode.fyl2x, "F,f"),
				new OpRec(Opcode.fptan, "F,f", OpFlag.X),
				new OpRec(Opcode.fpatan, "F,f"),
				new OpRec(Opcode.fxtract, "F,f", OpFlag.X),
				new OpRec(Opcode.fprem1, "F,f", OpFlag.X),
				new OpRec(Opcode.fdecstp, "F,f", OpFlag.X),
				new OpRec(Opcode.fincstp, "F,f", OpFlag.X),
						
				new OpRec(Opcode.fprem, "F,f", OpFlag.X),
				new OpRec(Opcode.fyl2xp1, "F,f", OpFlag.X),
				new OpRec(Opcode.fsqrt, ""),
				new OpRec(Opcode.fsincos, ""),
				new OpRec(Opcode.frndint, ""),
				new OpRec(Opcode.fscale, "F,f", OpFlag.X),
				new OpRec(Opcode.fsin, ""),
				new OpRec(Opcode.fcos, ""),
				
				// DA //////////////
				
				new OpRec(Opcode.fiadd, "Md"),
				new OpRec(Opcode.fimul, "Md"),
				new OpRec(Opcode.ficom, "Md"),
				new OpRec(Opcode.ficomp, "Md"),
				new OpRec(Opcode.fisub, "Md"),
				new OpRec(Opcode.fisubr, "Md"),
				new OpRec(Opcode.fidiv, "Md"),
				new OpRec(Opcode.fidivr, "Md"),
				
				// C0 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				// DB ///////////////////////////
				
				new OpRec(Opcode.fild, "Md"),
				new OpRec(Opcode.illegal, null, OpFlag.X),
				new OpRec(Opcode.fist, "Md", OpFlag.X),
				new OpRec(Opcode.fistp, "Md"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.fld, "Mh"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.fstp, "Mh", OpFlag.X),
						
				// C0, Conditional moves.

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.fclex, ""), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 

				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
				new OpRec(Opcode.illegal, null), 
					
				// DC ////////////////////

				new OpRec(Opcode.fadd, "Mg"),
				new OpRec(Opcode.fmul, "Mg"),
				new OpRec(Opcode.fcom, "Mg"),
				new OpRec(Opcode.fcomp, "Mg"),
				new OpRec(Opcode.fsub, "Mg"),
				new OpRec(Opcode.fsubr, "Mg"),
				new OpRec(Opcode.fdiv, "Mg"),
				new OpRec(Opcode.fdivr, "Mg"),
						
				new OpRec(Opcode.fadd, "f,F"),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
				new OpRec(Opcode.fadd, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
				new OpRec(Opcode.fmul, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
				new OpRec(Opcode.fcom, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new OpRec(Opcode.fcomp, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
				new OpRec(Opcode.fsub, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new OpRec(Opcode.fsubr, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new OpRec(Opcode.fdiv, "f,F", OpFlag.X),
						
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new OpRec(Opcode.fdivr, "f,F", OpFlag.X),

				// DD ////////////////

				new OpRec(Opcode.fld, "Mg"),
				new OpRec(Opcode.illegal, null, OpFlag.X),
				new OpRec(Opcode.fst, "Mg"),
				new OpRec(Opcode.fstp, "Mg"),
				new OpRec(Opcode.frstor, "Mw", OpFlag.X),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.fsave, "Mw", OpFlag.X),
				new OpRec(Opcode.fstsw, "Mw"),
						
				// DD C0
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
				new OpRec(Opcode.ffree, "F", OpFlag.X),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				// DD D0
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
				new OpRec(Opcode.fst, "F", OpFlag.X),
						
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
				new OpRec(Opcode.fstp, "F"),
						
				// E0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// F0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// DE //////////////////////////

				new OpRec(Opcode.fiadd, "Mw"),
				new OpRec(Opcode.fimul, "Mw"),
				new OpRec(Opcode.ficom, "Mw", OpFlag.X),
				new OpRec(Opcode.ficomp, "Mw", OpFlag.X),
				new OpRec(Opcode.fisub, "Mw", OpFlag.X),
				new OpRec(Opcode.fisubr, "Mw", OpFlag.X),
				new OpRec(Opcode.fidiv, "Mw"),
				new OpRec(Opcode.fidivr, "Mw", OpFlag.X),
				// DE C0
				new OpRec(Opcode.faddp, "F,f", OpFlag.X),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
				new OpRec(Opcode.faddp, "F,f"),
						
				new OpRec(Opcode.fmulp, "F,f", OpFlag.X),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
				new OpRec(Opcode.fmulp, "F,f"),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.fcompp, ""),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				// DE E0	
				new OpRec(Opcode.fsubrp, "F,f", OpFlag.X),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
				new OpRec(Opcode.fsubrp, "F,f"),
						
				new OpRec(Opcode.fsubp, "F,f", OpFlag.X),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				new OpRec(Opcode.fsubp, "F,f"),
				// DE F0
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
				new OpRec(Opcode.fdivrp, "F,f"),
						
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f"),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new OpRec(Opcode.fdivp, "F,f", OpFlag.X),
				
				// DF //////////////////////

				new OpRec(Opcode.fild, "Mw"),
				new OpRec(Opcode.illegal, null, OpFlag.X),
				new OpRec(Opcode.fist, "Mw", OpFlag.X),
				new OpRec(Opcode.fistp, "Mw"),
				new OpRec(Opcode.fbld, "MB", OpFlag.X),
				new OpRec(Opcode.fild, "Mq", OpFlag.X),
				new OpRec(Opcode.fbstp, "MB", OpFlag.X),
				new OpRec(Opcode.fistp, "Mq"),

				// C0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// C0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// E0
				new OpRec(Opcode.fstsw, "aw"),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				// F0
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
						
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
				new OpRec(Opcode.illegal, null),
			};
			Debug.Assert(s_aFpOpRec.Length == 8 * 0x48);
		}
	}
}	
