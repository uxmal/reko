#region License
/* 
 * Copyright (C) 1999-2016 John K�ll�n.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Reko.Arch.X86
{
	/// <summary>
	/// Intel x86 opcode disassembler 
	/// </summary>
	public partial class X86Disassembler : DisassemblerBase<X86Instruction>
	{
        private class X86LegacyCodeRegisterExtension
        {
            const byte MAGIC = 0x40;
            const byte MAGIC_MASK = 0xf0;
            internal static X86LegacyCodeRegisterExtension Disabled = new X86LegacyCodeRegisterExtension(0);
            
            byte val;

            internal X86LegacyCodeRegisterExtension(byte value)
            {
                this.val = value;
            }
            internal X86LegacyCodeRegisterExtension(byte magic, bool wide, bool modrm_reg, bool sib_idx, bool modrm_rm)
            {
                this.ByteValue = (byte)((this.val & 0xf) | ((magic & 0xf) << 4));
                this.FlagWideValue = wide;
                this.FlagTargetModrmRegister = modrm_reg;
                this.FlagTargetSIBIndex = sib_idx;
                this.FlagTargetModrmRegOrMem = modrm_rm;
            }

            internal byte ByteValue { get; set; }
            internal bool IsActive()
            {
                return this.val != 0;
            }
            internal bool FlagWideValue
            {
                get
                {
                    return ((this.val & 0x8) == 0x8);
                }
                set
                {
                    if (value)
                    {
                        this.val = (byte)(this.val | 0x8);
                    }
                    else
                    {
                        this.val = (byte)(this.val & ~0x8);
                    }
                }
            }
            internal bool FlagTargetModrmRegister
            {
                get
                {
                    return ((this.val & 0x4) == 0x4);
                }
                set
                {
                    if (value)
                    {
                        this.val = (byte)(this.val | 0x4);
                    }
                    else
                    {
                        this.val = (byte)(this.val & ~0x4);
                    }
                }
            }
            internal bool FlagTargetModrmRegOrMem
            {
                get
                {
                    return ((this.val & 0x1) == 0x1);
                }
                set
                {
                    if (value)
                    {
                        this.val = (byte)(this.val | 0x1);
                    }
                    else
                    {
                        this.val = (byte)(this.val & ~0x1);
                    }
                }
            }
            internal bool FlagTargetSIBIndex
            {
                get
                {
                    return ((this.val & 0x2) == 0x2);
                }
                set
                {
                    if (value)
                    {
                        this.val = (byte)(this.val | 0x2);
                    }
                    else
                    {
                        this.val = (byte)(this.val & ~0x2);
                    }
                }
            }
        }
        private class X86InstructionDecodeInfo
        {
            bool isModRegMemActive;
            byte modRegMemByte;

            bool isSegmentOverrideActive;
            RegisterStorage segmentOverride;

            bool isRegisterExtensionActive;
            X86LegacyCodeRegisterExtension registerExtension;

            bool repetitionPrefixF2;
            bool repetitionPrefixF3;

            bool sizeOverridePrefix;

            internal X86InstructionDecodeInfo()
            {
                this.Reset();
            }

            internal void Reset()
            {
                this.isModRegMemActive = false;
                this.modRegMemByte = 0;

                this.isSegmentOverrideActive = false;
                this.segmentOverride = RegisterStorage.None;

                // We do not reset isRegisterExtensionPrefixEnabled as that is set by the processor mode
                this.isRegisterExtensionActive = false;
                this.registerExtension = X86LegacyCodeRegisterExtension.Disabled;

                this.repetitionPrefixF2 = false;
                this.repetitionPrefixF3 = false;

                this.sizeOverridePrefix = false;
            }

            internal bool IsSegmentOverrideActive()
            {
                return this.isSegmentOverrideActive;
            }
            internal RegisterStorage SegmentOverride
            {
                get
                {
                    return this.isSegmentOverrideActive ? this.segmentOverride : RegisterStorage.None;
                }
                set
                {
                    this.isSegmentOverrideActive = true;
                    this.segmentOverride = value;
                }
            }

            internal bool IsRegisterExtensionActive()
            {
                return this.isRegisterExtensionActive;
            }
            internal X86LegacyCodeRegisterExtension RegisterExtension
            {
                get
                {
                    return this.registerExtension;
                }
            }
            internal byte RegisterExtensionPrefixByte
            {
                get
                {
                    return this.isRegisterExtensionActive ? this.registerExtension.ByteValue : (byte)0;
                }
                set
                {
                    this.isRegisterExtensionActive = true;
                    this.registerExtension = new X86LegacyCodeRegisterExtension(value);
                }
            }

            internal bool IsModRegMemByteActive()
            {
                return this.isModRegMemActive;
            }
            internal byte ModRegMemByte
            {
                get
                {
                    if (!this.isModRegMemActive)
                    {
                        throw new InvalidOperationException("The modrm byte was accessed without checking for validity. Check the code.");
                    }
                    return this.modRegMemByte;
                }
                set
                {
                    this.isModRegMemActive = true;
                    this.modRegMemByte = value;
                }
            }

            internal bool RepetitionPrefixF2
            {
                get
                {
                    return this.repetitionPrefixF2;
                }
                set
                {
                    if (!value)
                    {
                        throw new ArgumentException("In what case is it reasonable to not call Reset() instead???");
                    }
                    this.repetitionPrefixF2 = value;
                }
            }
            internal bool RepetitionPrefixF3
            {
                get
                {
                    return this.repetitionPrefixF3;
                }
                set
                {
                    if (!value)
                    {
                        throw new ArgumentException("In what case is it reasonable to not call Reset() instead???");
                    }
                    this.repetitionPrefixF3 = value;
                }
            }
            internal bool SizeOverridePrefix
            {
                get
                {
                    return this.sizeOverridePrefix;
                }
                set
                {
                    if (!value)
                    {
                        throw new ArgumentException("In what case is it reasonable to not call Reset() instead???");
                    }
                    this.sizeOverridePrefix = value;
                }
            }
        };
        
        private ProcessorMode mode;
        private X86Instruction instrCur;
		private PrimitiveType dataWidth;
		private PrimitiveType addressWidth;
		private PrimitiveType defaultDataWidth;
		private PrimitiveType defaultAddressWidth;
		private ImageReader	rdr;

        bool isRegisterExtensionEnabled;

        private X86InstructionDecodeInfo currentDecodingContext;

		/// <summary>
		/// Creates a disassembler that uses the specified reader to fetch bytes from the program image.
        /// </summary>
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 
        /// 16-bit operation, PrimitiveType.Word32 for 32-bit operation.</param>
		public X86Disassembler(
            ProcessorMode mode,
            ImageReader rdr,
            PrimitiveType defaultWordSize,
            PrimitiveType defaultAddressSize,
            bool useRexPrefix)
		{
            this.mode = mode;
			this.rdr = rdr;
			this.defaultDataWidth = defaultWordSize;
			this.defaultAddressWidth = defaultAddressSize;
            this.isRegisterExtensionEnabled = useRexPrefix;
            this.currentDecodingContext = new X86InstructionDecodeInfo();
        }

        /// <summary>
        /// If set, then x86 instruction section
        /// </summary>
        public bool Emulate8087 { get; set; }

        /// <summary>
        /// Disassembles the current instruction. The address is incremented
        /// to point at the first address after the instruction and returned to the caller.
        /// </summary>
        /// <returns>A single disassembled instruction.</returns>
        public override X86Instruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            var addr = rdr.Address;
            dataWidth = defaultDataWidth;
            addressWidth = defaultAddressWidth;

            // Reset the state of the currentInstruction
            this.currentDecodingContext.Reset();

            byte op;
            if (!rdr.TryReadByte(out op))
                return null;
            try
            {
                instrCur = s_aOpRec[op].Decode(this, op, "");
            }
            catch (Exception ex)
            {
                throw new AddressCorrelatedException(addr, ex, "An exception occurred when disassembling x86 code.");
            }
            if (instrCur == null)
            {
                return new X86Instruction(Opcode.illegal, dataWidth, addressWidth) 
                { Address = addr };
            }
            instrCur.Address = addr;
            instrCur.Length = (int)(rdr.Address - addr);
            return instrCur;
        }

        private RegisterStorage RegFromBitsRexW(int bits, PrimitiveType dataWidth)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.currentDecodingContext.RegisterExtension.FlagWideValue ? 8 : 0;
            return GpRegFromBits(reg_bits, dataWidth);
        }

        private RegisterStorage RegFromBitsRexR(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.currentDecodingContext.RegisterExtension.FlagTargetModrmRegister ? 8 : 0;
            return fnReg(reg_bits, dataWidth);
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
            int reg_bits = bits & 7;
            reg_bits |= this.currentDecodingContext.RegisterExtension.FlagTargetSIBIndex ? 8 : 0;
            return fnReg(reg_bits, dataWidth);
        }

        private RegisterStorage RegFromBitsRexB(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.currentDecodingContext.RegisterExtension.FlagTargetModrmRegOrMem ? 8 : 0;
            return fnReg(reg_bits, dataWidth);
        }

        private RegisterStorage GpRegFromBits(int bits, PrimitiveType dataWidth)
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
                case 4: return this.currentDecodingContext.IsRegisterExtensionActive() ? Registers.spl : Registers.ah;
                case 5: return this.currentDecodingContext.IsRegisterExtensionActive() ? Registers.bpl : Registers.ch;
                case 6: return this.currentDecodingContext.IsRegisterExtensionActive() ? Registers.sil : Registers.dh;
                case 7: return this.currentDecodingContext.IsRegisterExtensionActive() ? Registers.dil : Registers.bh;
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

		public static RegisterStorage SegFromBits(int bits)
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
            public abstract X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat);
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

			public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
			{
                return disasm.DecodeOperands(opcode, op, opFormat + format);
			}
		}

        /// <summary>
        /// Use this OpRec when an instruction encoding is dependent on whether the processor
        /// is in 64-bit mode or not.
        /// </summary>
        public class Alternative64OpRec : OpRec
        {
            private OpRec oprec32;
            private OpRec oprec64;

            public Alternative64OpRec(OpRec oprec32, OpRec oprec64)
            {
                this.oprec32 = oprec32;
                this.oprec64 = oprec64;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.defaultAddressWidth.BitSize == 64)
                    return oprec64.Decode(disasm, op, opFormat);
                else
                    return oprec32.Decode(disasm, op, opFormat);
            }
        }

        public class Rex_SingleByteOpRec : SingleByteOpRec
        {
            public Rex_SingleByteOpRec(Opcode op, string fmt)
                : base(op, fmt)
            {
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.isRegisterExtensionEnabled)
                {
                    disasm.currentDecodingContext.RegisterExtensionPrefixByte = op;
                    if (disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
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

			public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
			{
                disasm.currentDecodingContext.SegmentOverride = SegFromBits(seg);
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

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
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

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
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
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
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
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                op = disasm.rdr.ReadByte();
                return s_aOpRec0F[op].Decode(disasm, op, "");
            }
        }

        public class ThreeByteOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
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
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                byte b = disasm.rdr.PeekByte(0);
                if (b == 0x0F)
                {
                    disasm.currentDecodingContext.RepetitionPrefixF2 = true;
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                if (b == 0xC3)
                {
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[b].Decode(disasm, op, opFormat);
                }
                return disasm.DecodeOperands(Opcode.repne, 0xF2, opFormat);
            }
        }

        public class F3ByteOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                byte b = disasm.rdr.PeekByte(0);
                if (b == 0x0F)
                {
                    disasm.currentDecodingContext.RepetitionPrefixF3 = true;
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                if (b == 0xC3)
                {
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[b].Decode(disasm, op, opFormat);
                }
                return disasm.DecodeOperands(Opcode.rep, 0xF3, opFormat);
            }
        }

        public class ChangeDataWidth : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.SizeOverridePrefix = true;
                disasm.dataWidth = (disasm.dataWidth == PrimitiveType.Word16)
                        ? PrimitiveType.Word32
                        : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeAddressWidth : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
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

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.currentDecodingContext.RepetitionPrefixF2)
                    return disasm.DecodeOperands(this.opF2, op, opF2Fmt);
                else if (disasm.currentDecodingContext.RepetitionPrefixF3)
                    return disasm.DecodeOperands(this.opF3, op, opF3Fmt);
                else if (disasm.currentDecodingContext.SizeOverridePrefix)
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return disasm.DecodeOperands(this.op66Wide, op, op66Fmt);
                    else
                        return disasm.DecodeOperands(this.op66, op, op66Fmt);
                }
                else
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return disasm.DecodeOperands(this.opWide, op, opFmt);
                    else
                        return disasm.DecodeOperands(this.op, op, opFmt);
                }
            }
        }

        public class InterruptOpRec : SingleByteOpRec
        {
            public InterruptOpRec(Opcode op, string fmt) : base(op, fmt)
            {
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                var instr = base.Decode(disasm, op, opFormat);
                if (disasm.Emulate8087)
                {
                    var imm = (ImmediateOperand)instr.op1;
                    var vector = imm.Value.ToByte();
                    if (disasm.IsEmulated8087Vector(vector))
                    {
                        return disasm.RewriteEmulated8087Instruction(vector);
                    }
                }
                return instr;
            }
        }

		/// <summary>
		/// If the ModR/M byte hasn't been read yet, do so now.
		/// </summary>
		/// <returns></returns>
		private bool TryEnsureModRM(out byte modRm)
		{
            if (!this.currentDecodingContext.IsModRegMemByteActive())
            {
                byte modrm = 0;
                if (!rdr.TryReadByte(out modrm))
                {
                    modRm = 0;
                    return false;
                }
                this.currentDecodingContext.ModRegMemByte = modrm;
            }
            modRm = this.currentDecodingContext.ModRegMemByte;
            return true;
		}

        private X86Instruction DecodeOperands(Opcode opcode, byte op, string strFormat)
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
                MemoryOperand memOp;
                char chFmt = strFormat[i++];
                switch (chFmt)
                {
                case '1':
                    pOperand = new ImmediateOperand(Constant.Byte(1));
                    break;
                case '3':
                    pOperand = new ImmediateOperand(Constant.Byte(3));
                    break;
                case 'A':		// Absolute memory address.
                    ++i;
                    ushort off = rdr.ReadLeUInt16();
                    ushort seg = rdr.ReadLeUInt16();
                    var addr = mode.CreateSegmentedAddress(seg, off);
                    if (addr == null)
                        return null;
                    pOperand = new X86AddressOperand(addr);
                    break;
                case 'E':		// memory or register operand specified by mod & r/m fields.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(width, this.currentDecodingContext.SegmentOverride, GpRegFromBits);
                    if (pOperand == null)
                        return null;
                    break;
                case 'Q':		// memory or register MMX operand specified by mod & r/m fields.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(width, this.currentDecodingContext.SegmentOverride, MmxRegFromBits);
                    if (pOperand == null)
                        return null;
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
                    if (pOperand == null)
                        return null;
                    break;
                case 'J':		// Relative ("near") jump.
                    width = OperandWidth(strFormat[i++]);
                    long jOffset = rdr.ReadLeSigned(width);
                    ulong uAddr = (ulong) ((long)rdr.Address.Offset + jOffset);
                    if (defaultAddressWidth.BitSize == 64)      //$REVIEW: not too keen on the switch statement here.
                        pOperand = AddressOperand.Ptr64(uAddr);
                    else if (defaultAddressWidth.BitSize == 32)
                        pOperand = AddressOperand.Ptr32((uint)uAddr);
                    else
                        pOperand = new ImmediateOperand(Constant.Create(defaultDataWidth, uAddr));
                    break;
                case 'M':		// modRM may only refer to memory.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = DecodeModRM(dataWidth, this.currentDecodingContext.SegmentOverride, GpRegFromBits);
                    if (pOperand is RegisterOperand)
                        return null;
                    break;
                case 'O':		// Offset of the operand is encoded directly after the opcode.
                    width = OperandWidth(strFormat[i++]);
                    pOperand = memOp = new MemoryOperand(width, rdr.ReadLe(addressWidth));
                    memOp.SegOverride = this.currentDecodingContext.SegmentOverride;
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
                    pOperand = DecodeModRM(width, this.currentDecodingContext.SegmentOverride, XmmRegFromBits);
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
            return new X86Instruction(opcode, iWidth, addressWidth, ops.ToArray());
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
                dataWidth = PrimitiveType.Word64;
                break;
            case 'y':
                dataWidth = (this.isRegisterExtensionEnabled && this.currentDecodingContext.RegisterExtension.FlagWideValue) ? PrimitiveType.Word64: PrimitiveType.Word32;
                break;
            case 'z':
                dataWidth = this.dataWidth.BitSize == 64 ? PrimitiveType.Int32 : this.dataWidth;
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
                return this.currentDecodingContext.SizeOverridePrefix
                    ? PrimitiveType.Word128
                    : PrimitiveType.Word64;
            default: throw new NotImplementedException(string.Format("Unknown operand width {0}", fmt[i-1]));
            }
        }

		private static RegisterStorage [] s_ma16Base = 
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
            Constant c;
            if (!rdr.TryReadLe(immWidth, out c))
                return null;
			return new ImmediateOperand(c);
		}

		private MachineOperand DecodeModRM(PrimitiveType dataWidth, RegisterStorage segOverride, Func<int, PrimitiveType, RegisterStorage> regFn)
		{
            byte modRm;
            if (!TryEnsureModRM(out modRm))
                return null;

			int  rm = this.currentDecodingContext.ModRegMemByte & 0x07;
			int  mod = this.currentDecodingContext.ModRegMemByte >> 6;

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
                        if (defaultAddressWidth.BitSize == 64)      //$REFACTOR: should subclass this type of behaviour.
                        {
                            b = Registers.rip;
                            offsetWidth = PrimitiveType.Int32;
                        }
                        else
                        {
                            offsetWidth = PrimitiveType.Pointer32;
                            b = RegisterStorage.None;
                        }
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

                    byte sib;
                    if (!rdr.TryReadByte(out sib))
                        return null;
					if (((this.currentDecodingContext.ModRegMemByte & 0xC0) == 0) && ((sib & 0x7) == 5))
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

            Constant offset;
            if (offsetWidth != null)
            {
                if (!rdr.IsValidOffset(rdr.Offset + (uint)offsetWidth.Size -1))
                    return null;
                offset = rdr.ReadLe(offsetWidth);
            }
            else
            {
                offset = Constant.Invalid;
            }

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
