#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Decompiler.Arch.X86
{
	/// <summary>
	/// Intel x86 opcode disassembler 
	/// </summary>
	public partial class X86Disassembler : DisassemblerBase<IntelInstruction>
	{
        private IntelInstruction instrCur;
		private PrimitiveType dataWidth;
		private PrimitiveType addressWidth;
		private PrimitiveType defaultDataWidth;
		private PrimitiveType defaultAddressWidth;
		private byte modrm;
		private bool isModrmValid;
		private RegisterStorage segmentOverride;
		private ImageReader	rdr;
        private bool useRexPrefix;
        private byte rexPrefix;
        private bool dataSizeOverride;
        private bool f2PrefixSeen;
        private bool f3PrefixSeen;

		/// <summary>
		/// Creates a disassember that uses the specified reader to fetch bytes from the program image.
        /// </summary>
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 
        /// 16-bit operation, PrimitiveType.Word32 for 32-bit operation.</param>
		public X86Disassembler(
            ImageReader rdr,
            PrimitiveType defaultWordSize,
            PrimitiveType defaultAddressSize,
            bool useRexPrefix)
		{
			this.rdr = rdr;
			this.defaultDataWidth = defaultWordSize;
			this.defaultAddressWidth = defaultAddressSize;
            this.useRexPrefix = useRexPrefix;
        }

        /// <summary>
        /// Disassembles the current instruction. The address is incremented
        /// to point at the first address after the instruction and returned to the caller.
        /// </summary>
        /// <returns>A single disassembled instruction.</returns>
        public override IntelInstruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;

            dataWidth = defaultDataWidth;
            addressWidth = defaultAddressWidth;
            isModrmValid = false;
            rexPrefix = 0;
            segmentOverride = RegisterStorage.None;
            byte op;
            if (!rdr.TryReadByte(out op))
                return null;
            instrCur = s_aOpRec[op].Decode(this, op, "");
            if (instrCur == null)
                return null;
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        private IntelRegister RegFromBitsRexW(int bits, PrimitiveType dataWidth)
        {
            return GpRegFromBits((bits & 7) | ((rexPrefix & 8)), dataWidth);
        }

        private RegisterStorage RegFromBitsRexR(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            return fnReg((bits & 7) | ((rexPrefix & 4) << 1), dataWidth);
        }

        private RegisterStorage MmxRegFromBits(int bits, PrimitiveType dataWidth)
        {
            switch (bits & 7)
            {
            case 0: return Registers.mm0;
            case 1: return Registers.mm1;
            case 2: return Registers.mm2;
            case 3: return Registers.mm3;
            case 4: return Registers.mm4;
            case 5: return Registers.mm5;
            case 6: return Registers.mm6;
            case 7: return Registers.mm7;
            }
            throw new ArgumentOutOfRangeException(string.Format(
                "Unsupported register {0} or data width {1}.",
                bits, dataWidth));
        }

        private RegisterStorage RegFromBitsRexX(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            return fnReg((bits & 7) | ((rexPrefix & 2) << 2), dataWidth);
        }

        private RegisterStorage RegFromBitsRexB(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            return fnReg((bits & 7) | ((rexPrefix & 1) << 3), dataWidth);
        }

        private IntelRegister GpRegFromBits(int bits, PrimitiveType dataWidth)
		{
            int bitSize = dataWidth.BitSize;
			switch (bitSize)
            {
            case 8:
				switch (bits)
				{
				case 0: return Registers.al;
				case 1: return Registers.cl;
				case 2: return Registers.dl;
				case 3: return Registers.bl;
				case 4: return rexPrefix != 0 ? Registers.spl : (IntelRegister) Registers.ah;
                case 5: return rexPrefix != 0 ? Registers.bpl : (IntelRegister) Registers.ch;
                case 6: return rexPrefix != 0 ? Registers.sil : (IntelRegister) Registers.dh;
                case 7: return rexPrefix != 0 ? Registers.dil : (IntelRegister) Registers.bh;
                case 8: return Registers.r8b;
                case 9: return Registers.r9b;
                case 10: return Registers.r10b;
                case 11: return Registers.r11b;
                case 12: return Registers.r12b;
                case 13: return Registers.r13b;
                case 14: return Registers.r14b;
                case 15: return Registers.r15b;
				}
                break;
            case 16:
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
                case 8: return Registers.r8w;
                case 9: return Registers.r9w;
                case 10: return Registers.r10w;
                case 11: return Registers.r11w;
                case 12: return Registers.r12w;
                case 13: return Registers.r13w;
                case 14: return Registers.r14w;
                case 15: return Registers.r15w;
				}
			    break;
            case 32:
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
                case 8: return Registers.r8d;
                case 9: return Registers.r9d;
                case 10: return Registers.r10d;
                case 11: return Registers.r11d;
                case 12: return Registers.r12d;
                case 13: return Registers.r13d;
                case 14: return Registers.r14d;
                case 15: return Registers.r15d;
                }
                break;
            case 64:
                switch (bits)
                {
                case 0: return Registers.rax;
                case 1: return Registers.rcx;
                case 2: return Registers.rdx;
                case 3: return Registers.rbx;
                case 4: return Registers.rsp;
                case 5: return Registers.rbp;
                case 6: return Registers.rsi;
                case 7: return Registers.rdi;
                case 8: return Registers.r8;
                case 9: return Registers.r9;
                case 10: return Registers.r10;
                case 11: return Registers.r11;
                case 12: return Registers.r12;
                case 13: return Registers.r13;
                case 14: return Registers.r14;
                case 15: return Registers.r15;
                }
                break;
			}
			throw new ArgumentOutOfRangeException("Unsupported data width: " + dataWidth.ToString());
		}

        private RegisterStorage XmmRegFromBits(int bits, PrimitiveType dataWidth)
        {
            switch (bits)
            {
            case 0: return Registers.xmm0;
            case 1: return Registers.xmm1;
            case 2: return Registers.xmm2;
            case 3: return Registers.xmm3;
            case 4: return Registers.xmm4;
            case 5: return Registers.xmm5;
            case 6: return Registers.xmm6;
            case 7: return Registers.xmm7;
            case 8: return Registers.xmm8;
            case 9: return Registers.xmm9;
            case 10: return Registers.xmm10;
            case 11: return Registers.xmm11;
            case 12: return Registers.xmm12;
            case 13: return Registers.xmm13;
            case 14: return Registers.xmm14;
            case 15: return Registers.xmm15;
            }
            throw new NotImplementedException();
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
            public abstract IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat);
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

			public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
			{
                return disasm.DecodeOperands(opcode, op, opFormat + format);
			}
		}

        public class Rex_SingleByteOpRec : SingleByteOpRec
        {
            public Rex_SingleByteOpRec(Opcode op, string fmt)
                : base(op, fmt)
            {
            }

            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.useRexPrefix)
                {
                    disasm.rexPrefix = op;
                    if ((op & 8) != 0)
                    {
                        disasm.dataWidth = PrimitiveType.Word64;
                    }
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                else
                    return base.Decode(disasm, op, opFormat);
            }
        }

		public class SegmentOverrideOprec : OpRec
		{
            private int seg;

            public SegmentOverrideOprec(int seg)
			{
                this.seg = seg;
			}

			public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
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

            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                int grp = Group - 1;
                byte modRm;
                if (!disasm.TryEnsureModRM(out modRm))
                    return null;
                OpRec opRec = s_aOpRecGrp[grp * 8 + ((modRm >> 3) & 0x07)];
                return opRec.Decode(disasm, op, opFormat + format);
            }
        }

        // Uses the 2 high bits of the ModRM word for further discrimination
		public class Group7OpRec : OpRec
        {
            private OpRec memInstr;
            private OpRec[] regInstrs;

            public Group7OpRec(
                OpRec memInstr,
                params OpRec[] regInstrs)
            {
                this.memInstr = memInstr;
                this.regInstrs = regInstrs;
            }

            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                byte modRm;
                if (!disasm.TryEnsureModRM(out modRm))
                    return null;
                if ((modRm & 0xC0) == 0xC0)
                    return regInstrs[modRm & 0x07].Decode(disasm, op, opFormat);
                else
                    return memInstr.Decode(disasm, op, opFormat);
            }
        }
        
        public class FpuOpRec : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                byte modRM;
                if (!disasm.TryEnsureModRM(out modRM))
                    return null;
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
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                op = disasm.rdr.ReadByte();
                return s_aOpRec0F[op].Decode(disasm, op, "");
            }
        }

        public class ThreeByteOpRec : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                switch (op)
                {
                case 0x3A:
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec0F3A[op].Decode(disasm, op, "");
                default: return null;
                }
            }
        }
        public class F2ByteOpRec : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.rdr.PeekByte(0) == 0x0F)
                {
                    disasm.f2PrefixSeen = true;
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                return disasm.DecodeOperands(Opcode.repne, 0xF2, opFormat);
            }
        }

        public class F3ByteOpRec : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.rdr.PeekByte(0) == 0x0F)
                {
                    disasm.f3PrefixSeen = true;
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                return disasm.DecodeOperands(Opcode.rep, 0xF3, opFormat);
            }
        }

        public class ChangeDataWidth : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.dataSizeOverride = true;
                disasm.dataWidth = (disasm.dataWidth == PrimitiveType.Word16)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeAddressWidth : OpRec
        {
            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.addressWidth = (disasm.addressWidth == PrimitiveType.Word16)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class PrefixedOpRec : OpRec
        {
            private Opcode op;
            private Opcode op66;
            private Opcode opWide;
            private Opcode op66Wide;
            private Opcode opF3;
            private Opcode opF2;
            private string opFmt;
            private string op66Fmt;
            private string opF3Fmt;
            private string opF2Fmt;

            public PrefixedOpRec(
                Opcode op,
                string opFmt,
                Opcode op66 = Opcode.illegal,
                string op66Fmt = null,
                Opcode opF3 = Opcode.illegal,
                string opF3Fmt = null,
                Opcode opF2 = Opcode.illegal,
                string opF2Fmt = null)
            {
                this.op =   this.opWide = op;
                this.op66 = this.op66Wide = op66;
                this.opF3 = opF3;
                this.opF2 = opF2;
                this.opFmt = opFmt;
                this.op66Fmt = op66Fmt;
                this.opF3Fmt = opF3Fmt;
                this.opF2Fmt = opF2Fmt;
            }

            public PrefixedOpRec(
                Opcode op,
                Opcode opWide, 
                string opFmt, 
                Opcode op66,
                Opcode op66Wide,
                string op66Fmt,
                Opcode opF3 = Opcode.illegal,
                string opF3Fmt = null)
            {
                this.op = op;
                this.opWide = opWide;
                this.op66 = op66;
                this.op66Wide = op66Wide;
                this.opF3 = opF3;
                this.opF2 = Opcode.illegal;
                this.opFmt = opFmt;
                this.op66Fmt = op66Fmt;
                this.opF3Fmt = opF3Fmt;
                this.opF2Fmt = null;
            }

            public override IntelInstruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.f2PrefixSeen)
                    return disasm.DecodeOperands(this.opF2, op, opF2Fmt);
                else if (disasm.f3PrefixSeen)
                    return disasm.DecodeOperands(this.opF3, op, opF3Fmt);
                else if (disasm.dataSizeOverride)
                {
                    if (disasm.useRexPrefix && (disasm.rexPrefix & 8) != 0)
                        return disasm.DecodeOperands(this.op66Wide, op, op66Fmt);
                    else
                        return disasm.DecodeOperands(this.op66, op, op66Fmt);
                }
                else
                {
                    if (disasm.useRexPrefix && (disasm.rexPrefix & 8) != 0)
                        return disasm.DecodeOperands(this.opWide, op, opFmt);
                    else
                        return disasm.DecodeOperands(this.op, op, opFmt);
                }
            }
        }

		/// <summary>
		/// If the ModR/M byte hasn't been read yet, do so now.
		/// </summary>
		/// <returns></returns>
		private bool TryEnsureModRM(out byte modRm)
		{
            if (!isModrmValid)
            {
                if (!rdr.TryReadByte(out this.modrm))
                {
                    modRm = 0;
                    return false;
                }
                isModrmValid = true;
            }
            modRm = this.modrm;
            return true;
		}

        private IntelInstruction DecodeOperands(Opcode opcode, byte op, string strFormat)
        {
            MachineOperand pOperand;
            PrimitiveType width = null;
            PrimitiveType iWidth = dataWidth;
            byte modRm;
            List<MachineOperand> ops = new List<MachineOperand>();
            int i = 0;
            while (i != strFormat.Length)
            {
                if (strFormat[i] == ',')
                    ++i;

                pOperand = null;
                ImmediateOperand immOp;
                MemoryOperand memOp;
                X86AddressOperand addrOp;
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
                    ushort off = rdr.ReadLeUInt16();
                    ushort seg = rdr.ReadLeUInt16();
                    pOperand = addrOp = new X86AddressOperand(Address.SegPtr(seg, off));
                    break;
                case 'E':		// memory or register operand specified by mod & r/m fields.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(width, segmentOverride, GpRegFromBits);
                    break;
                case 'Q':		// memory or register MMX operand specified by mod & r/m fields.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(width, segmentOverride, MmxRegFromBits);
                    break;
                case 'G':		// register operand specified by the reg field of the modRM byte.
                    width = OperandWidth(strFormat[i++]);
                    if (!TryEnsureModRM(out modRm))
                        return null;
                    pOperand = new RegisterOperand(RegFromBitsRexR(modRm >> 3, width, GpRegFromBits));
                    break;
                case 'P':		// MMX register operand specified by the reg field of the modRM byte.
                    width = OperandWidth(strFormat[i++]);
                    if (!TryEnsureModRM(out modRm))
                        return null;
                    pOperand = new RegisterOperand(RegFromBitsRexR(modRm >> 3, width, MmxRegFromBits));
                    break;
                case 'I':		// Immediate operand.
                    if (strFormat[i] == 'x')
                    {
                        iWidth = width; // Use width of the previous operand.
                    }
                    else
                    {
                        width = OperandWidth(strFormat[i]); //  Don't use the width of the previous operand.
                    }
                    ++i;
                    pOperand = CreateImmediateOperand(width, dataWidth);
                    break;
                case 'J':		// Relative ("near") jump.
                    width = OperandWidth(strFormat[i++]);
                    offset = rdr.ReadLeSigned(width);
                    uint uAddr = (uint) (rdr.Address.Offset + offset);
                    if (defaultAddressWidth.BitSize == 32)
                        pOperand = AddressOperand.Ptr32(uAddr);
                    else
                        pOperand = new ImmediateOperand(Constant.Create(defaultDataWidth, uAddr));
                    break;
                case 'M':		// modRM may only refer to memory.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(dataWidth, segmentOverride, GpRegFromBits);
                    break;
                case 'O':		// Offset of the operand is encoded directly after the opcode.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = memOp = new MemoryOperand(width, rdr.ReadLe(addressWidth));
                    memOp.SegOverride = segmentOverride;
                    break;
                case 'S':		// Segment register encoded by reg field of modRM byte.
                    ++i;        // Skip over the 'w'.
                    if (!TryEnsureModRM(out modRm))
                        return null;
                    pOperand = new RegisterOperand(SegFromBits(modRm >> 3));
                    break;
                case 'V':		// XMM operand specified by the reg field of the modRM byte.
                    width = SseOperandWidth(strFormat, ref i);
                    if (!TryEnsureModRM(out modRm))
                        return null;
                    pOperand = new RegisterOperand(RegFromBitsRexR(modRm >> 3, width, XmmRegFromBits));
                    break;
                case 'W':		// memory or XMM operand specified by mod & r/m fields.
                    width = SseOperandWidth(strFormat, ref i);
                    pOperand = DecodeModRM(width, segmentOverride, XmmRegFromBits);
                    break;
                case 'a':		// Implicit use of accumulator.
                    pOperand = new RegisterOperand(RegFromBitsRexW(0, OperandWidth(strFormat[i++])));
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
                    pOperand = new RegisterOperand(RegFromBitsRexW(2, width));
                    break;
                case 'r':		// Register encoded as last 3 bits of instruction.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = new RegisterOperand(RegFromBitsRexW(op, width));
                    break;
                case 's':		// Segment encoded as next byte of the format string.
                    pOperand = new RegisterOperand(SegFromBits(strFormat[i++] - '0'));
                    break;
                case 'F':		// Floating-point ST(x)
                    if (!TryEnsureModRM(out modRm))
                        return null;
                    pOperand = new FpuOperand(modRm & 0x07);
                    break;
                case 'f':		// ST(0)
                    pOperand = new FpuOperand(0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown format specifier '{0}' at position {1} of format string '{2}'.", chFmt, i, strFormat));
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
				throw new ArgumentOutOfRangeException(string.Format("Unknown operand width specifier '{0}'.", ch));
			case 'b':
				dataWidth = PrimitiveType.Byte;
				break;
            case 'B':
                dataWidth = PrimitiveType.Bcd80;
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
                dataWidth = dataSizeOverride ?  PrimitiveType.Word128 : PrimitiveType.Word64;
                break;
            case 'y':
                dataWidth = (useRexPrefix && (rexPrefix & 8) != 0) ? PrimitiveType.Word64: PrimitiveType.Word32;
                break;
            }
			return dataWidth;
		}

        private PrimitiveType SseOperandWidth(string fmt, ref int i)
        {
            switch (fmt[i++])
            {
            case 'p':
                switch (fmt[i++])
                {
                case 's': return PrimitiveType.Word128;
                default: throw new NotImplementedException(string.Format("Unknown operand width p{0}", fmt[i-1]));
                }
            case 'q':
                return PrimitiveType.Word64;
            case 'x':
                return defaultDataWidth != dataWidth
                    ? PrimitiveType.Word128
                    : PrimitiveType.Word256;
            case 'y':
                return dataSizeOverride
                    ? PrimitiveType.Word128
                    : PrimitiveType.Word64;
            default: throw new NotImplementedException(string.Format("Unknown operand width {0}", fmt[i-1]));
            }
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

		private static RegisterStorage [] s_ma16Index =
		{
			Registers.si,	 
			Registers.di,	 
			Registers.si,	 
			Registers.di,	 
			RegisterStorage.None,
			RegisterStorage.None,
			RegisterStorage.None,
			RegisterStorage.None,
		};

		public ImmediateOperand CreateImmediateOperand(PrimitiveType immWidth, PrimitiveType instrWidth)
		{
			return new ImmediateOperand(rdr.ReadLe(immWidth));
		}

		private MachineOperand DecodeModRM(PrimitiveType dataWidth, RegisterStorage segOverride, Func<int, PrimitiveType, RegisterStorage> regFn)
		{
            byte modRm;
            if (!TryEnsureModRM(out modRm))
                return null;

			int  rm = this.modrm & 0x07;
			int  mod = this.modrm >> 6;

			RegisterStorage b;
            RegisterStorage idx;
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
						b = RegisterStorage.None;
						idx = RegisterStorage.None;
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
					return new RegisterOperand(RegFromBitsRexB(rm, dataWidth, GpRegFromBits));
				}
			}
			else 
			{
				b = RegFromBitsRexR(rm, addressWidth, GpRegFromBits);
				idx = RegisterStorage.None;

				switch (mod)
				{
				case 0:
					if (rm == 0x05)
					{
						offsetWidth = PrimitiveType.Pointer32;
						b = RegisterStorage.None;
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
					return new RegisterOperand(RegFromBitsRexB(rm, dataWidth, regFn));
				}

				// Handle possible s-i-b byte.

				if (rm == 0x04)
				{
					// We have SIB'ness, your majesty!

					byte sib = rdr.ReadByte();
					if (((this.modrm & 0xC0) == 0) && ((sib & 0x7) == 5))
					{
						offsetWidth = PrimitiveType.Word32;
						b = RegisterStorage.None;
					}
					else
					{
						b = RegFromBitsRexB(sib, addressWidth, GpRegFromBits);
					}
			
					int i = (sib >> 3) & 0x7;
					idx = (i == 0x04) ? RegisterStorage.None : RegFromBitsRexX(i, addressWidth, GpRegFromBits);
					scale = (byte) (1 << (sib >> 6));
				}
			}

			// Now fetch the offset if there was any.

			Constant offset = (offsetWidth != null)
			    ? rdr.ReadLe(offsetWidth)
			    : Constant.Invalid;

            return new MemoryOperand(dataWidth, offset)
            {
                Base = b,
                Index = idx,
                Scale = scale,
                SegOverride = segOverride,
            };
		}


		private bool ImplicitWidth(MachineOperand op)
		{
			return op is RegisterOperand || op is X86AddressOperand;
		}

		private static OpRec [] s_aOpRec;
		private static OpRec [] s_aOpRec0F;
		private static OpRec [] s_aOpRec0F3A;
		private static OpRec [] s_aOpRecGrp;
		private static OpRec [] s_aFpOpRec;

		static X86Disassembler()
		{
            s_aOpRec = CreateOnebyteOprecs();
            s_aOpRec0F = CreateTwobyteOprecs();
            s_aOpRec0F3A = Create0F3AOprecs();

            s_aOpRecGrp = CreateGroupOprecs();
            s_aFpOpRec = CreateFpuOprecs();
            Debug.Assert(s_aFpOpRec.Length == 8 * 0x48);
		}
	}
}	
