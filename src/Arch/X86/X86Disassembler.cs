#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;

namespace Reko.Arch.X86
{
	/// <summary>
	/// Intel x86 opcode disassembler 
	/// </summary>
	public partial class X86Disassembler : DisassemblerBase<X86Instruction, Mnemonic>
	{
        private class X86LegacyCodeRegisterExtension
        {
            internal X86LegacyCodeRegisterExtension(byte value)
            {
                this.ByteValue = value;
            }

            public void Reset()
            {
                ByteValue = 0;
            }

            internal byte ByteValue { get; set; }

            internal bool FlagWideValue
            {
                get
                {
                    return ((this.ByteValue & 0x8) == 0x8);
                }
                set
                {
                    if (value)
                    {
                        this.ByteValue = (byte)(this.ByteValue | 0x8);
                    }
                    else
                    {
                        this.ByteValue = (byte)(this.ByteValue & ~0x8);
                    }
                }
            }
            internal bool FlagTargetModrmRegister
            {
                get
                {
                    return ((this.ByteValue & 0x4) == 0x4);
                }
                set
                {
                    if (value)
                    {
                        this.ByteValue = (byte)(this.ByteValue | 0x4);
                    }
                    else
                    {
                        this.ByteValue = (byte)(this.ByteValue & ~0x4);
                    }
                }
            }
            internal bool FlagTargetModrmRegOrMem
            {
                get
                {
                    return ((this.ByteValue & 0x1) == 0x1);
                }
                set
                {
                    if (value)
                    {
                        this.ByteValue = (byte)(this.ByteValue | 0x1);
                    }
                    else
                    {
                        this.ByteValue = (byte)(this.ByteValue & ~0x1);
                    }
                }
            }
            internal bool FlagTargetSIBIndex
            {
                get
                {
                    return ((this.ByteValue & 0x2) == 0x2);
                }
                set
                {
                    if (value)
                    {
                        this.ByteValue = (byte)(this.ByteValue | 0x2);
                    }
                    else
                    {
                        this.ByteValue = (byte)(this.ByteValue & ~0x2);
                    }
                }
            }
        }

        private class X86InstructionDecodeInfo
        {
            // These fields are for analysis.
            bool isModRegMemActive;
            byte modRegMemByte;

            bool isSegmentOverrideActive;
            RegisterStorage segmentOverride;

            bool isRegisterExtensionActive;
            X86LegacyCodeRegisterExtension registerExtension;

            // These fields are for synthesis.
            public Mnemonic mnemonic;
            public InstrClass iclass;
            public PrimitiveType dataWidth;
            public PrimitiveType addressWidth;
            public List<MachineOperand> ops;
            public PrimitiveType iWidth;


            internal X86InstructionDecodeInfo()
            {
                this.registerExtension = new X86LegacyCodeRegisterExtension(0);
                this.ops = new List<MachineOperand>();
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
                this.registerExtension.Reset();

                this.F2Prefix = false;
                this.F3Prefix = false;
                this.SizeOverridePrefix = false;
                this.IsVex = false;
                this.VexRegister = 0;
                this.VexLong = false;

                this.ops.Clear();
            }

            public X86Instruction MakeInstruction()
            {
                return new X86Instruction(this.mnemonic, this.iclass, this.iWidth, this.addressWidth, this.ops.ToArray())
                {
                    repPrefix = this.F2Prefix ? 2 :
                                this.F3Prefix ? 3 : 0
                };
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
                    this.registerExtension.ByteValue = value;
                }
            }

            internal bool IsModRegMemByteActive()
            {
                return this.isModRegMemActive;
            }

            public bool IsVex { get; set; }

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

            internal bool F2Prefix { get; set; }

            internal bool F3Prefix { get; set; }

            internal bool SizeOverridePrefix { get; set; }

            public byte VexRegister { get; set; }

            public bool VexLong { get; set; } // If true, use YMM or 256-bit memory access.
        }

        private readonly ProcessorMode mode;
		private readonly PrimitiveType defaultDataWidth;
		private readonly PrimitiveType defaultAddressWidth;
		private readonly EndianImageReader rdr;

        private readonly bool isRegisterExtensionEnabled;

        private Address addr;
        private X86InstructionDecodeInfo decodingContext;

		/// <summary>
		/// Creates a disassembler that uses the specified reader to fetch bytes
        /// from the program image.
        /// </summary>
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 
        /// 16-bit operation, PrimitiveType.Word32 for 32-bit operation.</param>
		public X86Disassembler(
            ProcessorMode mode,
            EndianImageReader rdr,
            PrimitiveType defaultWordSize,
            PrimitiveType defaultAddressSize,
            bool useRexPrefix)
		{
            this.mode = mode;
			this.rdr = rdr;
			this.defaultDataWidth = defaultWordSize;
			this.defaultAddressWidth = defaultAddressSize;
            this.isRegisterExtensionEnabled = useRexPrefix;
            this.decodingContext = new X86InstructionDecodeInfo();
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
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;

            // Reset the state of the currentInstruction
            this.decodingContext.Reset();
            this.decodingContext.dataWidth = defaultDataWidth;
            this.decodingContext.addressWidth = defaultAddressWidth;
            this.decodingContext.iWidth = defaultDataWidth;

            X86Instruction instr;
            if (s_rootDecoders[op].Decode(this, op))
            {
                instr = decodingContext.MakeInstruction();
            }
            else 
            {
                instr = CreateInvalidInstruction();
            }
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override X86Instruction CreateInvalidInstruction()
        {
            return new X86Instruction(Mnemonic.illegal, InstrClass.Invalid, decodingContext.dataWidth, decodingContext.addressWidth);
        }

        private void NotYetImplemented(string message)
        {
            // collect bytes from rdr.addr to this.addr 
            var r2 = rdr.Clone();
            int len = (int) (r2.Address - this.addr);
            r2.Offset -= len;
            var bytes = r2.ReadBytes(len);
            var strBytes = string.Join("", bytes.Select(b => b.ToString("X2")));

            base.EmitUnitTest("x86", strBytes, message, "X86Dis", this.addr, w =>
            {
                w.WriteLine("    AssertCode32(\"@@@\", {0});",
                    string.Join(", ", bytes.Select(b => $"0x{b:X2}")));
            });
        }

        private RegisterStorage RegFromBitsRexB(int bits, PrimitiveType dataWidth)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.decodingContext.RegisterExtension.FlagTargetModrmRegOrMem ? 8 : 0;
            return GpRegFromBits(reg_bits, dataWidth);
        }

        private RegisterStorage RegFromBitsRexW(int bits, PrimitiveType dataWidth)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.decodingContext.RegisterExtension.FlagWideValue ? 8 : 0;
            return GpRegFromBits(reg_bits, dataWidth);
        }

        private RegisterStorage RegFromBitsRexR(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.decodingContext.RegisterExtension.FlagTargetModrmRegister ? 8 : 0;
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
            reg_bits |= this.decodingContext.RegisterExtension.FlagTargetSIBIndex ? 8 : 0;
            return fnReg(reg_bits, dataWidth);
        }

        private RegisterStorage RegFromBitsRexB(int bits, PrimitiveType dataWidth, Func<int, PrimitiveType, RegisterStorage> fnReg)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.decodingContext.RegisterExtension.FlagTargetModrmRegOrMem ? 8 : 0;
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
                case 4: return this.decodingContext.IsRegisterExtensionActive() ? Registers.spl : Registers.ah;
                case 5: return this.decodingContext.IsRegisterExtensionActive() ? Registers.bpl : Registers.ch;
                case 6: return this.decodingContext.IsRegisterExtensionActive() ? Registers.sil : Registers.dh;
                case 7: return this.decodingContext.IsRegisterExtensionActive() ? Registers.dil : Registers.bh;
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
            if (dataWidth.BitSize == 256)
            {
                switch (bits)
                {
                case 0: return Registers.ymm0;
                case 1: return Registers.ymm1;
                case 2: return Registers.ymm2;
                case 3: return Registers.ymm3;
                case 4: return Registers.ymm4;
                case 5: return Registers.ymm5;
                case 6: return Registers.ymm6;
                case 7: return Registers.ymm7;
                case 8: return Registers.ymm8;
                case 9: return Registers.ymm9;
                case 10: return Registers.ymm10;
                case 11: return Registers.ymm11;
                case 12: return Registers.ymm12;
                case 13: return Registers.ymm13;
                case 14: return Registers.ymm14;
                case 15: return Registers.ymm15;
                }
            }
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
            throw new InvalidOperationException();
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

        // Operand decoders //////

        // Absolute memory address
        public static bool Ap(uint op, X86Disassembler dasm)
        {
            var rdr = dasm.rdr;
            if (!rdr.TryReadLeUInt16(out ushort off))
                return false;
            if (!rdr.TryReadLeUInt16(out ushort seg))
                return false;
            var addr = dasm.mode.CreateSegmentedAddress(seg, off);
            if (addr == null)
                return false;
            dasm.decodingContext.ops.Add(new X86AddressOperand(addr));
            return true;
        }

        // control register encoded in the reg field.
        private static bool Cd(uint op, X86Disassembler dasm)
        {
            if (!dasm.TryEnsureModRM(out byte modRm))
                return false;
            var creg = dasm.mode.GetControlRegister((modRm >> 3) & 7);
            if (creg == null)
                return false;
            var operand = new RegisterOperand(creg);
            dasm.decodingContext.ops.Add(operand);
            return true;
        }

        // debug register encoded in the reg field.
        private static bool Dd(uint op, X86Disassembler dasm)
        {
            if (!dasm.TryEnsureModRM(out byte modRm))
                return false;
            var dreg = dasm.mode.GetDebugRegister((modRm >> 3) & 7);
            if (dreg == null)
                return false;
            var operand = new RegisterOperand(dreg);
            dasm.decodingContext.ops.Add(operand);
            return true;

        }

        // memory or register operand specified by mod & r/m fields.
        private static Mutator<X86Disassembler> E(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                d.decodingContext.iWidth = width;
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.GpRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Eb = E("b");
        private static readonly Mutator<X86Disassembler> Ed = E("d");
        private static readonly Mutator<X86Disassembler> Ep = E("p");
        private static readonly Mutator<X86Disassembler> Eq = E("q");
        private static readonly Mutator<X86Disassembler> Ev = E("v");
        private static readonly Mutator<X86Disassembler> Ey = E("y");
        private static readonly Mutator<X86Disassembler> Ew = E("w");

        // Floating-point ST(x)
        private static bool F(uint op, X86Disassembler d)
        {
            if (!d.TryEnsureModRM(out byte modRm))
                return false;
            d.decodingContext.ops.Add(new FpuOperand(modRm & 0x07));
            return true;
        }

        // Floating-point ST(0)
        private static bool f(uint op, X86Disassembler d)
        {
            d.decodingContext.ops.Add(new FpuOperand(0));
            return true;
        }

        // General purpose register operand specified by the reg field of the modRM byte.
        public static Mutator<X86Disassembler> G(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(
                    d.RegFromBitsRexR(
                        modRm >> 3,
                        width,
                        d.GpRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Gb = G("b");
        private static readonly Mutator<X86Disassembler> Gd = G("d");
        private static readonly Mutator<X86Disassembler> Gv = G("v");
        private static readonly Mutator<X86Disassembler> Gy = G("y");

        // If VEX encoding, use vvvv register.
        public static Mutator<X86Disassembler> H(string sWidth)
        {
            return (u, d) => {
                if (d.decodingContext.IsVex)
                {
                    int i = 0;
                    var width = d.SseOperandWidth(sWidth, ref i);
                    var op = new RegisterOperand(d.XmmRegFromBits(
                        d.decodingContext.VexRegister,
                        width));
                    d.decodingContext.ops.Add(op);
                }
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Hdq = H("dq");
        private static readonly Mutator<X86Disassembler> Hpd = H("pd");
        private static readonly Mutator<X86Disassembler> Hps = H("ps");
        private static readonly Mutator<X86Disassembler> Hqq = H("qq");
        private static readonly Mutator<X86Disassembler> Hsd = H("sd");
        private static readonly Mutator<X86Disassembler> Hss = H("ss");
        private static readonly Mutator<X86Disassembler> Hq = H("q");
        private static readonly Mutator<X86Disassembler> Hx = H("x");

        // Immediate operand.
        public static Mutator<X86Disassembler> I(string sWidth)
        {
            return (u, d) =>
            {
                PrimitiveType width;
                var ops = d.decodingContext.ops;
                if (sWidth[0] == 'x')
                {
                    width = ops[ops.Count - 1].Width;
                    d.decodingContext.iWidth = width;
                }
                else
                { 
                    int i = 0;
                    width = d.OperandWidth(sWidth, ref i); //  Don't use the width of the previous operand.
                }
                var op = d.CreateImmediateOperand(width, d.decodingContext.dataWidth);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Ib = I("b");
        private static readonly Mutator<X86Disassembler> Iv = I("v");
        private static readonly Mutator<X86Disassembler> Iw = I("w");
        private static readonly Mutator<X86Disassembler> Ix = I("x");
        private static readonly Mutator<X86Disassembler> Iz = I("z");

        // Relative ("near") jump.
        public static Mutator<X86Disassembler> J(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                if (!d.rdr.TryRead(width, out Constant cOffset))
                    return false;
                long jOffset = cOffset.ToInt64();
                ulong uAddr = (ulong) ((long) d.rdr.Address.Offset + jOffset);
                MachineOperand op;
                if (d.defaultAddressWidth.BitSize == 64)      //$REVIEW: not too keen on the switch statement here.
                    op = AddressOperand.Ptr64(uAddr);
                else if (d.defaultAddressWidth.BitSize == 32)
                    op = AddressOperand.Ptr32((uint) uAddr);
                else
                    op = new ImmediateOperand(Constant.Create(d.defaultAddressWidth, uAddr));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Jb = J("b");
        private static readonly Mutator<X86Disassembler> Jv = J("v");

        // The upper 4 bits of the 8-bit immediate selects a 128-bit XMM register or a 256-bit YMM register, determined
        // by operand type.
        public static bool Lx(uint op, X86Disassembler d)
        {
            if (!d.rdr.TryReadByte(out var lReg))
                return false;
            var ops = d.decodingContext.ops;
            var width =  ops[ops.Count-1].Width; // Use width of the previous operand.
            d.decodingContext.iWidth = width;
            //width = OperandWidth(strFormat, ref i); //  Don't use the width of the previous operand.
            var operand = new RegisterOperand(d.XmmRegFromBits((lReg >> 4) & 0xF, width));
            ops.Add(operand);
            return true;
        }

        // modRM may only refer to memory.
        public static Mutator<X86Disassembler> M(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                if ((modRm & 0xC0) == 0xC0)
                    return false;
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.GpRegFromBits) as MemoryOperand;
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> MB = M("B");
        private static readonly Mutator<X86Disassembler> Mb = M("b");
        private static readonly Mutator<X86Disassembler> Md = M("d");
        private static readonly Mutator<X86Disassembler> Mdq = M("dq");
        private static readonly Mutator<X86Disassembler> Mf = M("f");
        private static readonly Mutator<X86Disassembler> Mg = M("g");
        private static readonly Mutator<X86Disassembler> Mh = M("h");
        private static readonly Mutator<X86Disassembler> Mp = M("p");
        private static readonly Mutator<X86Disassembler> Mpd = M("pd");
        private static readonly Mutator<X86Disassembler> Mps = M("ps");
        private static readonly Mutator<X86Disassembler> Mq = M("q");
        private static readonly Mutator<X86Disassembler> Ms = M("s");
        private static readonly Mutator<X86Disassembler> Mv = M("v");
        private static readonly Mutator<X86Disassembler> Mw = M("w");
        private static readonly Mutator<X86Disassembler> Mx = M("x");
        private static readonly Mutator<X86Disassembler> My = M("y");

        // MMX register operand specified by the r/m field of the modRM byte.
        private static Mutator<X86Disassembler> N(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(d.RegFromBitsRexR(modRm, width, d.MmxRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Nq = N("q");

        // Offset of the operand is encoded directly after the opcode.
        public static Mutator<X86Disassembler> O(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                if (!d.rdr.TryReadLe(d.decodingContext.addressWidth, out var offset))
                    return false;
                var memOp = new MemoryOperand(width, offset);
                memOp.SegOverride = d.decodingContext.SegmentOverride;
                d.decodingContext.ops.Add(memOp);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Ob = O("b");
        private static readonly Mutator<X86Disassembler> Ov = O("v");

        // MMX register operand specified by the reg field of the modRM byte.
        public static Mutator<X86Disassembler> P(string sWidth)
        {
            return (u, d) => {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(d.RegFromBitsRexR(modRm >> 3, width, d.MmxRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Pd = P("d");
        private static readonly Mutator<X86Disassembler> Ppi = P("pi");
        private static readonly Mutator<X86Disassembler> Pq = P("q");

        // memory or register MMX operand specified by mod & r/m fields.
        public static Mutator<X86Disassembler> Q(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.MmxRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Qd = Q("d");
        private static readonly Mutator<X86Disassembler> Qpi = Q("pi");
        private static readonly Mutator<X86Disassembler> Qq = Q("q");

        // register operand specified by the mod field of the modRM byte.
        public static Mutator<X86Disassembler> R(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(
                    d.RegFromBitsRexR(
                        modRm,
                        width,
                        d.GpRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Rv = R("v");
        private static readonly Mutator<X86Disassembler> Ry = R("y");

        // XMM operand specified by the modRm field of the modRM byte.
        public static Mutator<X86Disassembler> U(string sWidth)
        {
            return (u, d) => {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(d.RegFromBitsRexR(
                    modRm, 
                    width, 
                    d.XmmRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Udq = U("dq");
        private static readonly Mutator<X86Disassembler> Upd = U("pd");
        private static readonly Mutator<X86Disassembler> Ups = U("ps");
        private static readonly Mutator<X86Disassembler> Ux = U("x");

        // XMM operand specified by the reg field of the modRM byte.
        public static Mutator<X86Disassembler> V(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(
                    d.RegFromBitsRexR(modRm >> 3,
                    width,
                    d.XmmRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }


        private static readonly Mutator<X86Disassembler> Vdq = V("dq");
        private static readonly Mutator<X86Disassembler> Vpd = V("pd");
        private static readonly Mutator<X86Disassembler> Vps = V("ps");
        private static readonly Mutator<X86Disassembler> Vsd = V("sd");
        private static readonly Mutator<X86Disassembler> Vss = V("ss");
        private static readonly Mutator<X86Disassembler> Vq = V("q");
        private static readonly Mutator<X86Disassembler> Vqq = V("qq");
        private static readonly Mutator<X86Disassembler> Vx = V("x");
        private static readonly Mutator<X86Disassembler> Vy = V("y");

        // memory or XMM operand specified by mod & r/m fields.
        public static Mutator<X86Disassembler> W(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.SseOperandWidth(sWidth, ref i);
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.XmmRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Wd = W("d");
        private static readonly Mutator<X86Disassembler> Wdq = W("dq");
        private static readonly Mutator<X86Disassembler> Wpd = W("pd");
        private static readonly Mutator<X86Disassembler> Wpq = W("pq");
        private static readonly Mutator<X86Disassembler> Wps = W("ps");
        private static readonly Mutator<X86Disassembler> Wq = W("q");
        private static readonly Mutator<X86Disassembler> Wqq = W("qq");
        private static readonly Mutator<X86Disassembler> Wsd = W("sd");
        private static readonly Mutator<X86Disassembler> Wss = W("ss");
        private static readonly Mutator<X86Disassembler> Wx = W("x");
        private static readonly Mutator<X86Disassembler> Wy = W("y");


        // Segment register encoded by reg field of modRM byte.
        public static bool Sw(uint op, X86Disassembler d)
        {
            if (!d.TryEnsureModRM(out byte modRm))
                return false;
            d.decodingContext.ops.Add(new RegisterOperand(SegFromBits(modRm >> 3)));
            return true;
        }

        // Implicit use of accumulator.
        public static Mutator<X86Disassembler> a(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var op = new RegisterOperand(d.RegFromBitsRexW(0, d.OperandWidth(sWidth, ref i)));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> ab = a("b");
        private static readonly Mutator<X86Disassembler> av = a("v");
        private static readonly Mutator<X86Disassembler> aw = a("w");

        private static bool b(uint op, X86Disassembler d)
        {
            d.decodingContext.iWidth = PrimitiveType.Byte;
            return true;
        }

        // Implicit use of CL.
        private static bool c(uint u, X86Disassembler d)
        {
            d.decodingContext.ops.Add(new RegisterOperand(Registers.cl));
            return true;
        }

        // Implicit use of DX or EDX.
        private static Mutator<X86Disassembler> d(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                var width = d.OperandWidth(sWidth, ref i);
                var op = new RegisterOperand(d.RegFromBitsRexW(2, width));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> dw = d("w");

        private static bool n1(uint u, X86Disassembler d)
        {
            var op = new ImmediateOperand(Constant.Byte(1));
            d.decodingContext.ops.Add(op);
            return true;
        }

        private static bool n3(uint u, X86Disassembler d)
        {
            var op = new ImmediateOperand(Constant.Byte(3));
            d.decodingContext.ops.Add(op);
            return true;
        }

        // Register encoded as last 3 bits of instruction.
        private static Mutator<X86Disassembler> r(string sWidth)
        {
            return (u, d) =>
            {
                int i = 0;
                d.decodingContext.iWidth =
                    d.OperandWidth(sWidth, ref i);
                var op = new RegisterOperand(d.RegFromBitsRexB(
                    (byte)u, 
                    d.decodingContext.iWidth));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> rb = r("b");
        private static readonly Mutator<X86Disassembler> rq = r("q");
        private static readonly Mutator<X86Disassembler> rv = r("v");
        private static readonly Mutator<X86Disassembler> rw = r("w");


        private static Mutator<X86Disassembler> Reg(RegisterStorage reg)
        {
            var op = new RegisterOperand(reg);
            return (u, d) =>
            {
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> s0 = Reg(Registers.es);
        private static readonly Mutator<X86Disassembler> s1 = Reg(Registers.cs);
        private static readonly Mutator<X86Disassembler> s2 = Reg(Registers.ss);
        private static readonly Mutator<X86Disassembler> s3 = Reg(Registers.ds);
        private static readonly Mutator<X86Disassembler> s4 = Reg(Registers.fs);
        private static readonly Mutator<X86Disassembler> s5 = Reg(Registers.gs);

        public static InstructionDecoder Instr(Mnemonic op)
        {
            return new InstructionDecoder(op, InstrClass.Linear);
        }

        public static InstructionDecoder Instr(Mnemonic op, params Mutator<X86Disassembler> [] mutators)
        {
            return new InstructionDecoder(op, InstrClass.Linear, mutators);
        }

        public static InstructionDecoder Instr(Mnemonic op, InstrClass iclass, params Mutator<X86Disassembler> [] mutators)
        {
            return new InstructionDecoder(op, iclass, mutators);
        }

        public static PrefixedDecoder Prefixed(Mnemonic op, string format)
        {
            return new PrefixedDecoder();
        }

        public static NyiDecoder nyi(string message)
        {
            return new NyiDecoder(message);
        }

		/// <summary>
		/// If the ModR/M byte hasn't been read yet, do so now.
		/// </summary>
		/// <returns></returns>
		private bool TryEnsureModRM(out byte modRm)
		{
            if (!this.decodingContext.IsModRegMemByteActive())
            {
                if (!rdr.TryReadByte(out byte modrm))
                {
                    modRm = 0;
                    return false;
                }
                this.decodingContext.ModRegMemByte = modrm;
            }
            modRm = this.decodingContext.ModRegMemByte;
            return true;
		}

		/// <summary>
		/// Returns the operand width of the operand type.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private PrimitiveType OperandWidth(string fmt, ref int i)
		{
			switch (fmt[i])
			{
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unknown operand width specifier '{0}'.", fmt[i]));
			case 'b':
				decodingContext.dataWidth = PrimitiveType.Byte;
				break;
            case 'B':
                decodingContext.dataWidth = PrimitiveType.Bcd80;
                break;
			case 'v':
				break;
			case 'w':
				decodingContext.dataWidth = PrimitiveType.Word16;
				break;
			case 'd':
                if (i < fmt.Length - 1 && fmt[i + 1] == 'q')
                {
                    ++i;
                    decodingContext.dataWidth = PrimitiveType.Word128;
                }
                else
                {
                    decodingContext.dataWidth = PrimitiveType.Word32;
                }
				break;
			case 'p':
                if (i < fmt.Length - 1 &&
                    (fmt[i + 1] == 's' || fmt[i + 1] == 'd'))
                {
                    ++i;
                    decodingContext.dataWidth =  this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128;
                }
                else
                {
				decodingContext.dataWidth = PrimitiveType.Ptr32;		// Far pointer.
                }
				break;
			case 'f':
				decodingContext.dataWidth = PrimitiveType.Real32;
				break;
			case 'g':
				decodingContext.dataWidth = PrimitiveType.Real64;
				break;
			case 'h':
				decodingContext.dataWidth = PrimitiveType.Real80;
				break;
            case 'q':
                decodingContext.dataWidth = PrimitiveType.Word64;
                break;
            case 's':
                decodingContext.dataWidth = this.decodingContext.dataWidth.BitSize == 64 ? PrimitiveType.CreateWord(80) : PrimitiveType.CreateWord(48);
                break;
            case 'x':
                decodingContext.dataWidth = this.decodingContext.VexLong
                    ? PrimitiveType.Word256
                    : PrimitiveType.Word128;
                break;
            case 'y':
                decodingContext.dataWidth = (this.isRegisterExtensionEnabled && this.decodingContext.RegisterExtension.FlagWideValue) ? PrimitiveType.Word64: PrimitiveType.Word32;
                break;
            case 'z':
                decodingContext.dataWidth = this.decodingContext.dataWidth.BitSize == 64 ? PrimitiveType.Int32 : this.decodingContext.dataWidth;
                break;
            }
			return decodingContext.dataWidth;
		}

        private PrimitiveType SseOperandWidth(string fmt, ref int i)
        {
            switch (fmt[i++])
            {
            case 'd':
                if (i < fmt.Length && fmt[i] == 'q')
                {
                    ++i; return PrimitiveType.Word128;
                }
                return PrimitiveType.Word32;
            case 'p':
                switch (fmt[i++])
                {
                case 'd': return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[2] of double32
                case 'i': return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[4] of int32
                case 's': return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[4] of real32
                default: throw new NotImplementedException(string.Format("Unknown operand width p{0}", fmt[i-1]));
                }
            case 'q':
                if (i < fmt.Length && fmt[i] == 'q')
                {
                    ++i; return PrimitiveType.Word256;
                }
                return PrimitiveType.Word64;
            case 's':
                switch (fmt[i++])
                {
                case 'd': return PrimitiveType.Real64;
                case 'i': return PrimitiveType.Int32;
                case 's': return PrimitiveType.Real32;
                default: throw new NotImplementedException(string.Format("Unknown operand width s{0}", fmt[i - 1]));
                }
            case 'x':
                return this.decodingContext.VexLong
                    ? PrimitiveType.Word256
                    : PrimitiveType.Word128;
            case 'y':
                return this.decodingContext.SizeOverridePrefix
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
            if (!rdr.TryReadLe(immWidth, out Constant c))
                return null;
			return new ImmediateOperand(c);
		}

		private MachineOperand DecodeModRM(PrimitiveType dataWidth, RegisterStorage segOverride, Func<int, PrimitiveType, RegisterStorage> regFn)
		{
            if (!TryEnsureModRM(out byte modRm))
                return null;

			int rm = this.decodingContext.ModRegMemByte & 0x07;
			int mod = this.decodingContext.ModRegMemByte >> 6;

			RegisterStorage b;
            RegisterStorage idx;
			byte scale = 1;
			PrimitiveType offsetWidth = null;

			if (decodingContext.addressWidth == PrimitiveType.Word16)
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
					return new RegisterOperand(RegFromBitsRexB(rm, dataWidth, regFn));
				}
			}
			else 
			{
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
                            offsetWidth = PrimitiveType.Ptr32;
                            b = RegisterStorage.None;
                        }
					}
					else
					{
                        b = RegFromBitsRexB(rm, decodingContext.addressWidth, GpRegFromBits);
                        offsetWidth = null;
					}
					break;
				case 1:
                    b = RegFromBitsRexB(rm, decodingContext.addressWidth, GpRegFromBits);
                    offsetWidth = PrimitiveType.SByte;
					break;
				case 2:
                    b = RegFromBitsRexB(rm, decodingContext.addressWidth, GpRegFromBits);
                    offsetWidth = PrimitiveType.Word32;
					break;
				case 3:
					return new RegisterOperand(RegFromBitsRexB(rm, dataWidth, regFn));
                default:
                    throw new InvalidOperationException("Impossiburu.");
				}

				// Handle possible s-i-b byte.

				if (rm == 0x04)
				{
                    // We have SIB'ness, your majesty!

                    if (!rdr.TryReadByte(out byte sib))
                        return null;
					if (((this.decodingContext.ModRegMemByte & 0xC0) == 0) && ((sib & 0x7) == 5))
					{
						offsetWidth = PrimitiveType.Word32;
						b = RegisterStorage.None;
					}
					else
					{
						b = RegFromBitsRexB(sib, decodingContext.addressWidth, GpRegFromBits);
					}
			
					int i = (sib >> 3) & 0x7;
					idx = (i == 0x04) ? RegisterStorage.None : RegFromBitsRexX(i, decodingContext.addressWidth, GpRegFromBits);
					scale = (byte) (1 << (sib >> 6));
				}
			}

            // Now fetch the offset if there was any.

            Constant offset;
            if (offsetWidth != null)
            {
                if (!rdr.IsValidOffset(rdr.Offset + (uint)offsetWidth.Size -1))
                    return null;
                if (!rdr.TryReadLe(offsetWidth, out offset))
                    return null;
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

        private static Decoder s_invalid;
        private static Decoder s_nyi;
		private static Decoder [] s_rootDecoders;
		private static Decoder [] s_decoders0F;
		private static Decoder [] s_decoders0F38;
		private static Decoder [] s_decoders0F3A;
        private static Decoder [] s_groupDecoders;
		private static Decoder [] s_fpuDecoders;
        private static Dictionary<Mnemonic, Mnemonic> s_mpVex;

        static X86Disassembler()
		{
            s_invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);
            s_nyi = nyi("This could be invalid or it could be not yet implemented");
            s_rootDecoders = CreateOnebyteDecoders();
            s_decoders0F = CreateTwobyteDecoders();
            s_decoders0F38 = Create0F38Decoders();
            s_decoders0F3A = Create0F3ADecoders();

            s_groupDecoders = CreateGroupDecoders();
            s_fpuDecoders = CreateFpuDecoders();
            s_mpVex = CreateVexMapping();
            Debug.Assert(s_fpuDecoders.Length == 8 * 0x48);
		}
    }
}
