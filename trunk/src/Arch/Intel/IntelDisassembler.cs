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
using Decompiler.Core.Types;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Decompiler.Arch.Intel
{
	/// <summary>
	/// Intel x86 opcode disassembler 
	/// </summary>
	public class IntelDisassembler : Disassembler
	{
		private PrimitiveType dataWidth;
		private PrimitiveType addressWidth;
		private PrimitiveType defaultDataWidth;
		private PrimitiveType defaultAddressWidth;
		private byte m_modrm;
		private bool isModrmValid;
		private MachineRegister segmentOverride;
		private ImageReader	rdr;

		/// <summary>
		/// Creates a disassember that uses the specified reader to fetch bytes from the program image.
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 
        /// 16-bit operation, PrimitiveType.Word32 for 32-bit operation.</param>
		public IntelDisassembler(ImageReader rdr, PrimitiveType defaultWordSize)
		{
			this.rdr = rdr;
			defaultDataWidth = defaultWordSize;
			defaultAddressWidth = defaultWordSize;
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
        public enum OpFlag
        {
            None = 0,
            X = 0x8000     // hasn't been tested.
        }

        /// <summary>
        /// Opcode Records are used to pick apart the somewhat complex x86 instructions, which have many optional
        /// prefixes, segment overrides, and two classes of instructions, single-byte and two-byte (that is,
        /// prefixed with 0F)
        /// </summary>
		public abstract class OpRec
		{
            public abstract IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat);
		}

		/// <summary>
		/// Single byte opcode record.
		/// </summary>
		public class SingleByteOpRec : OpRec
		{
			public Opcode	opcode;
			public string	format;
			public OpFlag	Flags;

            public SingleByteOpRec(Opcode op): this(op, "", OpFlag.None)
            {
            }

			public SingleByteOpRec(Opcode op, string fmt) : this(op, fmt, OpFlag.None)
			{
			}

			public SingleByteOpRec(Opcode op, string fmt, OpFlag flags)
			{
				opcode = op;
				format = fmt;
				Flags = flags;
			}

			public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
			{
                return disasm.DecodeOperands(opcode, op, opFormat + format);
			}
		}

		public class SegmentOverrideOprec : OpRec
		{
            private int seg;

            public SegmentOverrideOprec(int seg)
			{
                this.seg = seg;
			}

			public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
			{
                disasm.segmentOverride = SegFromBits(seg);
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
			}
		}

        public class GroupOpRec : OpRec
        {
            public int Group;
            public string format;

            public GroupOpRec(int group, string format)
            {
                this.Group = group;
                this.format = format;
            }

            public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
            {
                int grp = Group - 1;
                OpRec opRec = s_aOpRecGrp[grp * 8 + ((disasm.EnsureModRM() >> 3) & 0x07)];
                return opRec.Decode(disasm, op, opFormat + format);
            }
        }

        public class FpuOpRec : OpRec
        {
            public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
            {
                byte modRM = disasm.EnsureModRM();
                OpRec opRec;
                int iOpRec = (op & 0x07) * 0x48;
                if (modRM < 0xC0)
                {
                    opRec = s_aFpOpRec[iOpRec + ((modRM >> 3) & 0x07)];
                }
                else
                {
                    opRec = s_aFpOpRec[iOpRec + modRM - 0xB8];
                }
                return opRec.Decode(disasm, op, opFormat);
            }
        }

        public class TwoByteOpRec : OpRec
        {
            public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
            {
                op = disasm.rdr.ReadByte();
                return s_aOpRec0F[op].Decode(disasm, op, "");
            }
        }

        public class ChangeDataWidth : OpRec
        {
            public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
            {
                disasm.dataWidth = (disasm.dataWidth == PrimitiveType.Word16)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeAddressWidth : OpRec
        {
            public override IntelInstruction Decode(IntelDisassembler disasm, byte op, string opFormat)
            {
                disasm.addressWidth = (disasm.addressWidth == PrimitiveType.Word16)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
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
		/// Disassembles the current instruction. The address is incremented
		/// to point at the first address after the instruction and returned to the caller.
		/// </summary>
		/// <returns>A single disassembled instruction.</returns>
		public virtual IntelInstruction Disassemble()
		{
			dataWidth = defaultDataWidth;
			addressWidth = defaultAddressWidth;
			isModrmValid = false;
			segmentOverride = MachineRegister.None;
            byte op = rdr.ReadByte();
			OpRec opRec = s_aOpRec[op];
            return opRec.Decode(this, op, "");
        }

        private IntelInstruction DecodeOperands(Opcode opcode, byte op, string strFormat)
        {
            MachineOperand pOperand;
            PrimitiveType width = null;
            PrimitiveType iWidth = dataWidth;

            List<MachineOperand> ops = new List<MachineOperand>();
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
                    pOperand = immOp = new ImmediateOperand(Constant.Byte(1));
                    break;
                case '3':
                    pOperand = immOp = new ImmediateOperand(Constant.Byte(3));
                    break;
                case 'A':		// Absolute memory address.
                    ++i;
                    ushort off = rdr.ReadLeUint16();
                    ushort seg = rdr.ReadLeUint16();
                    pOperand = addrOp = new AddressOperand(new Address(seg, off));
                    break;
                case 'E':		// memory or register operand specified by mod & r/m fields.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(width, segmentOverride);
                    break;
                case 'G':		// register operand specified by the reg field of the modRM byte.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = new RegisterOperand(RegFromBits(EnsureModRM() >> 3, width));
                    break;
                case 'I':		// Immediate operand.
                    if (strFormat[i] == 'x')
                    {
                        // Use width of the previous operand.
                        iWidth = width;
                    }
                    else
                    {
                        //  Don't use the width of the previous operand.
                        width = OperandWidth(strFormat[i]);
                    }
                    ++i;
                    pOperand = CreateImmediateOperand(width, dataWidth);
                    break;
                case 'J':		// Relative jump.
                    width = OperandWidth(strFormat[i++]);
                    offset = rdr.ReadLeSigned(width);
                    pOperand = new ImmediateOperand(new Constant(defaultDataWidth, (uint) (rdr.Address.Offset + offset)));
                    break;
                case 'M':		// modRM may only refer to memory.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(dataWidth, segmentOverride);
                    break;
                case 'O':		// Offset of the operand is encoded directly after the opcode.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = memOp = new MemoryOperand(width, rdr.ReadLe(addressWidth));
                    memOp.SegOverride = segmentOverride;
                    break;
                case 'S':		// Segment register encoded by reg field of modRM byte.
                    Debug.Assert(strFormat[i++] == 'w');
                    pOperand = new RegisterOperand(SegFromBits(EnsureModRM() >> 3));
                    break;
                case 'a':		// Implicit use of accumulator.
                    pOperand = new RegisterOperand(RegFromBits(0, OperandWidth(strFormat[i++])));
                    break;
                case 'b':
                    iWidth = PrimitiveType.Byte;
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
                if (pOperand != null)
                {
                    ops.Add(pOperand);
                }
            }
            return new IntelInstruction(opcode, iWidth, addressWidth, ops.ToArray());
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
				dataWidth = PrimitiveType.Byte;
				break;
			case 'v':
				break;
			case 'w':
				dataWidth = PrimitiveType.Word16;
				break;
			case 'd':
				dataWidth = PrimitiveType.Word32;
				break;
			case 'p':
				dataWidth = PrimitiveType.Pointer32;		// Far pointer.
				break;
			case 'f':
				dataWidth = PrimitiveType.Real32;
				break;
			case 'g':
				dataWidth = PrimitiveType.Real64;
				break;
			case 'h':
				dataWidth = PrimitiveType.Real80;
				break;
			case 'q':
				dataWidth = PrimitiveType.Word64;
				break;
			}
			return dataWidth;
		}

		public override MachineInstruction DisassembleInstruction()
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

		private static MachineRegister [] s_ma16Index =
		{
			Registers.si,	 
			Registers.di,	 
			Registers.si,	 
			Registers.di,	 
			MachineRegister.None,
			MachineRegister.None,
			MachineRegister.None,
			MachineRegister.None,
		};


		public ImmediateOperand CreateImmediateOperand(PrimitiveType immWidth, PrimitiveType instrWidth)
		{
			return new ImmediateOperand(rdr.ReadLe(immWidth));
		}

		private MachineOperand DecodeModRM(PrimitiveType dataWidth, MachineRegister segOverride)
		{
			EnsureModRM();

			int  rm = m_modrm & 0x07;
			int  mod = m_modrm >> 6;

			MachineRegister b;
            MachineRegister idx;
			byte scale = 1;
			PrimitiveType offsetWidth = null;

			if (addressWidth == PrimitiveType.Word16)
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
						b = MachineRegister.None;
						idx = MachineRegister.None;
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
				b = RegFromBits(rm, addressWidth);
				idx = MachineRegister.None;

				switch (mod)
				{
				case 0:
					if (rm == 0x05)
					{
						offsetWidth = PrimitiveType.Word32;
						b = MachineRegister.None;
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
						b = MachineRegister.None;
					}
					else
					{
						b = RegFromBits(sib, addressWidth);
					}
			
					int i = (sib >> 3) & 0x7;
					idx = (i == 0x04) ? MachineRegister.None : RegFromBits(i, addressWidth);
					scale = (byte) (1 << (sib >> 6));
				}
			}

			// Now fetch the offset if there was any.

			Constant offset = null;
			if (offsetWidth != null)
			{
				offset = rdr.ReadLe(offsetWidth);
			}
			else
				offset = Constant.Invalid;

			MemoryOperand memOp = new MemoryOperand(dataWidth, offset);
			memOp.Base = b;
			memOp.Index = idx;
			memOp.Scale = scale;
			memOp.SegOverride = segOverride;
			return memOp;
		}


		private bool ImplicitWidth(MachineOperand op)
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
				new SingleByteOpRec(Opcode.add, "Eb,Gb"),
				new SingleByteOpRec(Opcode.add, "Ev,Gv"),
				new SingleByteOpRec(Opcode.add, "Gb,Eb"),
				new SingleByteOpRec(Opcode.add, "Gv,Ev"),
				new SingleByteOpRec(Opcode.add, "ab,Ib"),
				new SingleByteOpRec(Opcode.add, "av,Iv"),
				new SingleByteOpRec(Opcode.push, "s0"),
				new SingleByteOpRec(Opcode.pop, "s0"),

				new SingleByteOpRec(Opcode.or, "Eb,Gb"),
				new SingleByteOpRec(Opcode.or, "Ev,Gv"),
				new SingleByteOpRec(Opcode.or, "Gb,Eb"),
				new SingleByteOpRec(Opcode.or, "Gv,Ev"),
				new SingleByteOpRec(Opcode.or, "ab,Ib"),
				new SingleByteOpRec(Opcode.or, "av,Iv"),
				new SingleByteOpRec(Opcode.push, "s1"),
				new TwoByteOpRec(),

				// 10
				new SingleByteOpRec(Opcode.adc, "Eb,Gb"),
				new SingleByteOpRec(Opcode.adc, "Ev,Gv"),
				new SingleByteOpRec(Opcode.adc, "Gb,Eb"),
				new SingleByteOpRec(Opcode.adc, "Gv,Ev"),
				new SingleByteOpRec(Opcode.adc, "ab,Ib"),
				new SingleByteOpRec(Opcode.adc, "av,Iv"),
				new SingleByteOpRec(Opcode.push, "s2"),
				new SingleByteOpRec(Opcode.pop, "s2"),

				new SingleByteOpRec(Opcode.sbb, "Eb,Gb"),
				new SingleByteOpRec(Opcode.sbb, "Ev,Gv"),
				new SingleByteOpRec(Opcode.sbb, "Gb,Eb"),
				new SingleByteOpRec(Opcode.sbb, "Gv,Ev"),
				new SingleByteOpRec(Opcode.sbb, "ab,Ib"),
				new SingleByteOpRec(Opcode.sbb, "av,Iv"),
				new SingleByteOpRec(Opcode.push, "s3"),
				new SingleByteOpRec(Opcode.pop, "s3"),

				// 20
				new SingleByteOpRec(Opcode.and, "Eb,Gb"), 
				new SingleByteOpRec(Opcode.and, "Ev,Gv"),
				new SingleByteOpRec(Opcode.and, "Gb,Eb"),
				new SingleByteOpRec(Opcode.and, "Gv,Ev"),
				new SingleByteOpRec(Opcode.and, "ab,Ib"),
				new SingleByteOpRec(Opcode.and, "av,Iv"),
				new SegmentOverrideOprec(0),
				new SingleByteOpRec(Opcode.daa),

				new SingleByteOpRec(Opcode.sub, "Eb,Gb"),
				new SingleByteOpRec(Opcode.sub, "Ev,Gv"),
				new SingleByteOpRec(Opcode.sub, "Gb,Eb"),
				new SingleByteOpRec(Opcode.sub, "Gv,Ev"),
				new SingleByteOpRec(Opcode.sub, "ab,Ib"),
				new SingleByteOpRec(Opcode.sub, "av,Iv"),
                new SegmentOverrideOprec(1),
				new SingleByteOpRec(Opcode.das),

				// 30
				new SingleByteOpRec(Opcode.xor, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xor, "Ev,Gv"),
				new SingleByteOpRec(Opcode.xor, "Gb,Eb"),
				new SingleByteOpRec(Opcode.xor, "Gv,Ev"),
				new SingleByteOpRec(Opcode.xor, "ab,Ib"),
				new SingleByteOpRec(Opcode.xor, "av,Iv"),
                new SegmentOverrideOprec(2),
				new SingleByteOpRec(Opcode.aaa),

				new SingleByteOpRec(Opcode.cmp, "Eb,Gb"),
				new SingleByteOpRec(Opcode.cmp, "Ev,Gv"),
				new SingleByteOpRec(Opcode.cmp, "Gb,Eb"),
				new SingleByteOpRec(Opcode.cmp, "Gv,Ev"),
				new SingleByteOpRec(Opcode.cmp, "ab,Ib"),
				new SingleByteOpRec(Opcode.cmp, "av,Iv"),
                new SegmentOverrideOprec(3),
				new SingleByteOpRec(Opcode.aas),

				// 40
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),
				new SingleByteOpRec(Opcode.inc, "rv"),

				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),
				new SingleByteOpRec(Opcode.dec, "rv"),

				// 50
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),
				new SingleByteOpRec(Opcode.push, "rv"),

				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),
				new SingleByteOpRec(Opcode.pop, "rv"),

				// 60
				new SingleByteOpRec(Opcode.pusha),
				new SingleByteOpRec(Opcode.popa),
				new SingleByteOpRec(Opcode.bound, "Gv,Mv", OpFlag.X),
				new SingleByteOpRec(Opcode.arpl, "Ew,rw"),
				new SegmentOverrideOprec(4),
				new SegmentOverrideOprec(5),
				new ChangeDataWidth(),
				new ChangeAddressWidth(),

				new SingleByteOpRec(Opcode.push, "Iv"),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev,Iv"),
				new SingleByteOpRec(Opcode.push, "Ib"),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev,Ib"),
				new SingleByteOpRec(Opcode.insb, "b"),
				new SingleByteOpRec(Opcode.ins,  ""),
				new SingleByteOpRec(Opcode.outsb, "b"),
				new SingleByteOpRec(Opcode.outs),

				// 70
				new SingleByteOpRec(Opcode.jo, "Jb"),
				new SingleByteOpRec(Opcode.jno, "Jb"),
				new SingleByteOpRec(Opcode.jc, "Jb"),
				new SingleByteOpRec(Opcode.jnc, "Jb"),
				new SingleByteOpRec(Opcode.jz, "Jb"),
				new SingleByteOpRec(Opcode.jnz, "Jb"),
				new SingleByteOpRec(Opcode.jbe, "Jb"),
				new SingleByteOpRec(Opcode.ja, "Jb"),

				new SingleByteOpRec(Opcode.js, "Jb"),
				new SingleByteOpRec(Opcode.jns, "Jb"),
				new SingleByteOpRec(Opcode.jpe, "Jb"),
				new SingleByteOpRec(Opcode.jpo, "Jb"),
				new SingleByteOpRec(Opcode.jl, "Jb"),
				new SingleByteOpRec(Opcode.jge, "Jb"),
				new SingleByteOpRec(Opcode.jle, "Jb"),
				new SingleByteOpRec(Opcode.jg, "Jb"),

				// 80
				new GroupOpRec(1, "Eb,Ib"),
				new GroupOpRec(1, "Ev,Iv"),
				new SingleByteOpRec(Opcode.illegal),
				new GroupOpRec(1, "Ev,Ib"),
				new SingleByteOpRec(Opcode.test, "Eb,Gb"),
				new SingleByteOpRec(Opcode.test, "Ev,Gv"),
				new SingleByteOpRec(Opcode.xchg, "Eb,Gb"),
				new SingleByteOpRec(Opcode.xchg, "Ev,Gv"),

				new SingleByteOpRec(Opcode.mov, "Eb,Gb"),
				new SingleByteOpRec(Opcode.mov, "Ev,Gv"),
				new SingleByteOpRec(Opcode.mov, "Gb,Eb"),
				new SingleByteOpRec(Opcode.mov, "Gv,Ev"),
				new SingleByteOpRec(Opcode.mov, "Ew,Sw"),
				new SingleByteOpRec(Opcode.lea, "Gv,Mv"),
				new SingleByteOpRec(Opcode.mov, "Sw,Ew"),
				new SingleByteOpRec(Opcode.pop, "Ev"),

				// 90
				new SingleByteOpRec(Opcode.nop),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),
				new SingleByteOpRec(Opcode.xchg, "av,rv"),

				new SingleByteOpRec(Opcode.cbw),
				new SingleByteOpRec(Opcode.cwd),
				new SingleByteOpRec(Opcode.call, "Ap"),
				new SingleByteOpRec(Opcode.wait),
				new SingleByteOpRec(Opcode.pushf),
				new SingleByteOpRec(Opcode.popf),
				new SingleByteOpRec(Opcode.sahf),
				new SingleByteOpRec(Opcode.lahf),

				// A0
				new SingleByteOpRec(Opcode.mov, "ab,Ob"),
				new SingleByteOpRec(Opcode.mov, "av,Ov"),
				new SingleByteOpRec(Opcode.mov, "Ob,ab"),
				new SingleByteOpRec(Opcode.mov, "Ov,av"),
				new SingleByteOpRec(Opcode.movsb, "b"),
				new SingleByteOpRec(Opcode.movs),
				new SingleByteOpRec(Opcode.cmpsb, "b"),
				new SingleByteOpRec(Opcode.cmps),

				new SingleByteOpRec(Opcode.test, "ab,Ib"),
				new SingleByteOpRec(Opcode.test, "av,Iv"),
				new SingleByteOpRec(Opcode.stosb, "b"),
				new SingleByteOpRec(Opcode.stos),
				new SingleByteOpRec(Opcode.lodsb, "b"),
				new SingleByteOpRec(Opcode.lods),
				new SingleByteOpRec(Opcode.scasb, "b"),
				new SingleByteOpRec(Opcode.scas),

				// B0
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),
				new SingleByteOpRec(Opcode.mov, "rb,Ib"),

				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),
				new SingleByteOpRec(Opcode.mov, "rv,Iv"),

				// C0
				new GroupOpRec(2, "Eb,Ib"),
				new GroupOpRec(2, "Ev,Ib"),
				new SingleByteOpRec(Opcode.ret,	"Iw"),
				new SingleByteOpRec(Opcode.ret),
				new SingleByteOpRec(Opcode.les,	"Gv,Mp"),
				new SingleByteOpRec(Opcode.lds,	"Gv,Mp"),
				new SingleByteOpRec(Opcode.mov,	"Eb,Ib"),
				new SingleByteOpRec(Opcode.mov,	"Ev,Iv"),

				new SingleByteOpRec(Opcode.enter, "Iw,Ib"),
				new SingleByteOpRec(Opcode.leave),
				new SingleByteOpRec(Opcode.retf,	"Iw"),
				new SingleByteOpRec(Opcode.retf,	""),
				new SingleByteOpRec(Opcode.@int,	"3"),
				new SingleByteOpRec(Opcode.@int,	"Ib"),
				new SingleByteOpRec(Opcode.into,	"", OpFlag.X),
				new SingleByteOpRec(Opcode.iret,	""),

				// D0
				new GroupOpRec(2, "Eb,1"),
				new GroupOpRec(2, "Ev,1"),
				new GroupOpRec(2, "Eb,c"),
				new GroupOpRec(2, "Ev,c"),
				new SingleByteOpRec(Opcode.aam, "Ib"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.xlat),

				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),
				new FpuOpRec(),

				// E0
				new SingleByteOpRec(Opcode.loopne,"Jb"),
				new SingleByteOpRec(Opcode.loope, "Jb"),
				new SingleByteOpRec(Opcode.loop, "Jb"),
				new SingleByteOpRec(Opcode.jcxz, "Jb"),
				new SingleByteOpRec(Opcode.@in, "ab,Ib"),
				new SingleByteOpRec(Opcode.@in, "av,Ib"),
				new SingleByteOpRec(Opcode.@out, "Ib,ab"),
				new SingleByteOpRec(Opcode.@out, "Ib,av"),

				new SingleByteOpRec(Opcode.call, "Jv"),
				new SingleByteOpRec(Opcode.jmp, "Jv"),
				new SingleByteOpRec(Opcode.jmp, "Ap"),
				new SingleByteOpRec(Opcode.jmp, "Jb"),
				new SingleByteOpRec(Opcode.@in, "ab,dw"),
				new SingleByteOpRec(Opcode.@in, "av,dw"),
				new SingleByteOpRec(Opcode.@out, "dw,ab"),
				new SingleByteOpRec(Opcode.@out, "dw,av"),

				// F0
				new SingleByteOpRec(Opcode.@lock),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.repne),
				new SingleByteOpRec(Opcode.rep),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.cmc),
				new GroupOpRec(3, "Eb"),
				new GroupOpRec(3, "Ev"),

				new SingleByteOpRec(Opcode.clc),
				new SingleByteOpRec(Opcode.stc),
				new SingleByteOpRec(Opcode.cli),
				new SingleByteOpRec(Opcode.sti),
				new SingleByteOpRec(Opcode.cld),
				new SingleByteOpRec(Opcode.std),
				new GroupOpRec(4, ""),
				new GroupOpRec(5, "")
			};
		

			s_aOpRec0F = new OpRec[]
			{
				// 00
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 10
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 20
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 30
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 40
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 50
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
					
				// 60
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 70
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// 80
				new SingleByteOpRec(Opcode.jo,	"Jv"),
				new SingleByteOpRec(Opcode.jno,   "Jv"),
				new SingleByteOpRec(Opcode.jc,	"Jv"),
				new SingleByteOpRec(Opcode.jnc,	"Jv"),
				new SingleByteOpRec(Opcode.jz,	"Jv"),
				new SingleByteOpRec(Opcode.jnz,   "Jv"),
				new SingleByteOpRec(Opcode.jbe,   "Jv"),
				new SingleByteOpRec(Opcode.ja,    "Jv"),

				new SingleByteOpRec(Opcode.js,    "Jv"),
				new SingleByteOpRec(Opcode.jns,   "Jv"),
				new SingleByteOpRec(Opcode.jpe,   "Jv"),
				new SingleByteOpRec(Opcode.jpo,   "Jv"),
				new SingleByteOpRec(Opcode.jl,    "Jv"),
				new SingleByteOpRec(Opcode.jge,   "Jv"),
				new SingleByteOpRec(Opcode.jle,   "Jv"),
				new SingleByteOpRec(Opcode.jg,    "Jv"),

				// 90
				new SingleByteOpRec(Opcode.seto, "Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setno,"Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setc, "Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setnc,"Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setz, "Eb"),
				new SingleByteOpRec(Opcode.setnz,"Eb"),
				new SingleByteOpRec(Opcode.setbe,"Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.seta, "Eb", OpFlag.X),

				new SingleByteOpRec(Opcode.sets,    "Eb"),
				new SingleByteOpRec(Opcode.setns, "Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setpe, "Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setpo, "Eb", OpFlag.X),
				new SingleByteOpRec(Opcode.setl, "Eb"),
				new SingleByteOpRec(Opcode.setge,   "Eb"),
				new SingleByteOpRec(Opcode.setle, "Eb"),
				new SingleByteOpRec(Opcode.setg, "Eb"),

				// A0
				new SingleByteOpRec(Opcode.push, "s4"),
				new SingleByteOpRec(Opcode.pop, "s4"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bt, "Ev,Gv"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shld, "Ev,Gv,c"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.push, "s5"),
				new SingleByteOpRec(Opcode.pop, "s5"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,Ib"),
				new SingleByteOpRec(Opcode.shrd, "Ev,Gv,c"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.imul, "Gv,Ev"),

				// B0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.lss, "Gv,Mp"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.lfs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.lgs, "Gv,Mp"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movzx, "Gv,Ew"),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new GroupOpRec(8, "Ev,Ib"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bsr, "Gv,Ev"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Eb"),
				new SingleByteOpRec(Opcode.movsx, "Gv,Ew"),

				// C0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),
				new SingleByteOpRec(Opcode.bswap, "rv"),

				// D0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// E0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// F0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
			};

			s_aOpRecGrp = new SingleByteOpRec[] 
			{
				// group 1
				new SingleByteOpRec(Opcode.add),
				new SingleByteOpRec(Opcode.or),
				new SingleByteOpRec(Opcode.adc),
				new SingleByteOpRec(Opcode.sbb),
				new SingleByteOpRec(Opcode.and),
				new SingleByteOpRec(Opcode.sub),
				new SingleByteOpRec(Opcode.xor),
				new SingleByteOpRec(Opcode.cmp),

				// group 2
				new SingleByteOpRec(Opcode.rol),
				new SingleByteOpRec(Opcode.ror),
				new SingleByteOpRec(Opcode.rcl),
				new SingleByteOpRec(Opcode.rcr),
				new SingleByteOpRec(Opcode.shl),
				new SingleByteOpRec(Opcode.shr),
				new SingleByteOpRec(Opcode.shl),
				new SingleByteOpRec(Opcode.sar),

				// group 3
				new SingleByteOpRec(Opcode.test, ",Ix"),
				new SingleByteOpRec(Opcode.test, ",Ix", OpFlag.X),
				new SingleByteOpRec(Opcode.not),
				new SingleByteOpRec(Opcode.neg),
				new SingleByteOpRec(Opcode.mul),
				new SingleByteOpRec(Opcode.imul),
				new SingleByteOpRec(Opcode.div),
				new SingleByteOpRec(Opcode.idiv),
				
				// group 4
				new SingleByteOpRec(Opcode.inc, "Eb"),
				new SingleByteOpRec(Opcode.dec, "Eb"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal), 

				// group 5
				new SingleByteOpRec(Opcode.inc, "Ev"),
				new SingleByteOpRec(Opcode.dec, "Ev"),
				new SingleByteOpRec(Opcode.call, "Ev"),
				new SingleByteOpRec(Opcode.call, "Ep"),
				new SingleByteOpRec(Opcode.jmp, "Ev"),
				new SingleByteOpRec(Opcode.jmp, "Ep"),
				new SingleByteOpRec(Opcode.push, "Ev"),
				new SingleByteOpRec(Opcode.illegal),

				// group 6
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 7
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),

				// group 8
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.bt),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
			};

			s_aFpOpRec = new SingleByteOpRec[]  
			{
				// D8 /////////////////////////

				new SingleByteOpRec(Opcode.fadd, "Mf"),
				new SingleByteOpRec(Opcode.fmul, "Mf"),
				new SingleByteOpRec(Opcode.fcom, "Mf"),
				new SingleByteOpRec(Opcode.fcomp, "Mf"),
				new SingleByteOpRec(Opcode.fsub,  "Mf"),
				new SingleByteOpRec(Opcode.fsubr, "Mf"),
				new SingleByteOpRec(Opcode.fdiv,  "Mf"),
				new SingleByteOpRec(Opcode.fdivr, "Mf"),
				// D8 C0
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F"),

				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F"),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				// D8 D0		
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				new SingleByteOpRec(Opcode.fcomp, "f,F"),
				// D8 E0
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
				new SingleByteOpRec(Opcode.fsub, "f,F"),
						
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				new SingleByteOpRec(Opcode.fsubr, "f,F"),
				// D8 F0
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F"),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				
				// D9 ////////////////////////////////
				
				new SingleByteOpRec(Opcode.fld, "Mf"),
				new SingleByteOpRec(Opcode.illegal, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "Mf"),
				new SingleByteOpRec(Opcode.fstp, "Mf"),
				new SingleByteOpRec(Opcode.fldenv, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fldcw, "Mw"),
				new SingleByteOpRec(Opcode.fstenv, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fstcw, "Mw"),
						
				// D9 C0
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
				new SingleByteOpRec(Opcode.fld, "F"),
						
				new SingleByteOpRec(Opcode.fxch, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
				new SingleByteOpRec(Opcode.fxch, "f,F"),
						
				// D9 D0
				new SingleByteOpRec(Opcode.fnop, "", OpFlag.X),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// E0
				new SingleByteOpRec(Opcode.fchs),
				new SingleByteOpRec(Opcode.fabs, "", OpFlag.X),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.ftst),
				new SingleByteOpRec(Opcode.fxam),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.fld1),
				new SingleByteOpRec(Opcode.fldl2t, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fldl2e, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fldpi),
				new SingleByteOpRec(Opcode.fldlg2, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fldln2),
				new SingleByteOpRec(Opcode.fldz),
				new SingleByteOpRec(Opcode.illegal),
						
				// D9 F0
				new SingleByteOpRec(Opcode.f2xm1, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fyl2x, "F,f"),
				new SingleByteOpRec(Opcode.fptan, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fpatan, "F,f"),
				new SingleByteOpRec(Opcode.fxtract, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fprem1, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdecstp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fincstp, "F,f", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fprem, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fyl2xp1, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fsqrt),
				new SingleByteOpRec(Opcode.fsincos),
				new SingleByteOpRec(Opcode.frndint),
				new SingleByteOpRec(Opcode.fscale, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fsin),
				new SingleByteOpRec(Opcode.fcos),
				
				// DA //////////////
				
				new SingleByteOpRec(Opcode.fiadd, "Md"),
				new SingleByteOpRec(Opcode.fimul, "Md"),
				new SingleByteOpRec(Opcode.ficom, "Md"),
				new SingleByteOpRec(Opcode.ficomp, "Md"),
				new SingleByteOpRec(Opcode.fisub, "Md"),
				new SingleByteOpRec(Opcode.fisubr, "Md"),
				new SingleByteOpRec(Opcode.fidiv, "Md"),
				new SingleByteOpRec(Opcode.fidivr, "Md"),
				
				// C0 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				// DB ///////////////////////////
				
				new SingleByteOpRec(Opcode.fild, "Md"),
				new SingleByteOpRec(Opcode.illegal, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fist, "Md", OpFlag.X),
				new SingleByteOpRec(Opcode.fistp, "Md"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.fld, "Mh"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.fstp, "Mh", OpFlag.X),
						
				// C0, Conditional moves.

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.fclex), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 

				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
				new SingleByteOpRec(Opcode.illegal), 
					
				// DC ////////////////////

				new SingleByteOpRec(Opcode.fadd, "Mg"),
				new SingleByteOpRec(Opcode.fmul, "Mg"),
				new SingleByteOpRec(Opcode.fcom, "Mg"),
				new SingleByteOpRec(Opcode.fcomp, "Mg"),
				new SingleByteOpRec(Opcode.fsub, "Mg"),
				new SingleByteOpRec(Opcode.fsubr, "Mg"),
				new SingleByteOpRec(Opcode.fdiv, "Mg"),
				new SingleByteOpRec(Opcode.fdivr, "Mg"),
						
				new SingleByteOpRec(Opcode.fadd, "f,F"),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fadd, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fmul, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcom, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fcomp, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsub, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubr, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdiv, "f,F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivr, "f,F", OpFlag.X),

				// DD ////////////////

				new SingleByteOpRec(Opcode.fld, "Mg"),
				new SingleByteOpRec(Opcode.illegal, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "Mg"),
				new SingleByteOpRec(Opcode.fstp, "Mg"),
				new SingleByteOpRec(Opcode.frstor, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.fsave, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fstsw, "Mw"),
						
				// DD C0
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.ffree, "F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				// DD D0
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
				new SingleByteOpRec(Opcode.fst, "F", OpFlag.X),
						
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
				new SingleByteOpRec(Opcode.fstp, "F"),
						
				// E0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// F0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// DE //////////////////////////

				new SingleByteOpRec(Opcode.fiadd, "Mw"),
				new SingleByteOpRec(Opcode.fimul, "Mw"),
				new SingleByteOpRec(Opcode.ficom, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.ficomp, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fisub, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fisubr, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fidiv, "Mw"),
				new SingleByteOpRec(Opcode.fidivr, "Mw", OpFlag.X),
				// DE C0
				new SingleByteOpRec(Opcode.faddp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
				new SingleByteOpRec(Opcode.faddp, "F,f"),
						
				new SingleByteOpRec(Opcode.fmulp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
				new SingleByteOpRec(Opcode.fmulp, "F,f"),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.fcompp),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				// DE E0	
				new SingleByteOpRec(Opcode.fsubrp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
				new SingleByteOpRec(Opcode.fsubrp, "F,f"),
						
				new SingleByteOpRec(Opcode.fsubp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				new SingleByteOpRec(Opcode.fsubp, "F,f"),
				// DE F0
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
				new SingleByteOpRec(Opcode.fdivrp, "F,f"),
						
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f"),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				new SingleByteOpRec(Opcode.fdivp, "F,f", OpFlag.X),
				
				// DF //////////////////////

				new SingleByteOpRec(Opcode.fild, "Mw"),
				new SingleByteOpRec(Opcode.illegal, "", OpFlag.X),
				new SingleByteOpRec(Opcode.fist, "Mw", OpFlag.X),
				new SingleByteOpRec(Opcode.fistp, "Mw"),
				new SingleByteOpRec(Opcode.fbld, "MB", OpFlag.X),
				new SingleByteOpRec(Opcode.fild, "Mq", OpFlag.X),
				new SingleByteOpRec(Opcode.fbstp, "MB", OpFlag.X),
				new SingleByteOpRec(Opcode.fistp, "Mq"),

				// C0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// C0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// E0
				new SingleByteOpRec(Opcode.fstsw, "aw"),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				// F0
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
						
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
				new SingleByteOpRec(Opcode.illegal),
			};
			Debug.Assert(s_aFpOpRec.Length == 8 * 0x48);
		}
	}
}	
