#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    /// <summary>
    /// Intel x86 machine code disassembler 
    /// </summary>
    public partial class X86Disassembler : DisassemblerBase<X86Instruction, Mnemonic>
	{
#pragma warning disable IDE1006
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

            /// <summary>
            /// Rex.W = 48h
            /// </summary>
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
            byte? modRegMemByte;

            bool isSegmentOverrideActive;
            RegisterStorage segmentOverride;

            bool isRegisterExtensionActive;
            X86LegacyCodeRegisterExtension registerExtension;

            // These fields are for synthesis.
            public PrimitiveType dataWidth;
            public PrimitiveType addressWidth;
            public List<MachineOperand> ops;
            public PrimitiveType iWidth;

#nullable disable
            internal X86InstructionDecodeInfo()
            {
                this.registerExtension = new X86LegacyCodeRegisterExtension(0);
                this.ops = new List<MachineOperand>();
                this.Reset();
            }
#nullable enable

            internal void Reset()
            {
                this.modRegMemByte = null;

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

            public X86Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
            {
                return new X86Instruction(mnemonic, iclass, this.iWidth, this.addressWidth, this.ops.ToArray())
                {
                    repPrefix = iclass.HasFlag(InstrClass.Invalid)
                        ? 0
                        : this.F2Prefix 
                            ? 2 :
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
                return this.modRegMemByte.HasValue;
            }

            public bool IsVex { get; set; }

            internal byte ModRegMemByte
            {
                get
                {
                    if (!this.modRegMemByte.HasValue)
                    {
                        throw new InvalidOperationException("The modrm byte was accessed without checking for validity. Check the code.");
                    }
                    return this.modRegMemByte.Value;
                }
                set
                {
                    this.modRegMemByte = value;
                }
            }

            internal bool F2Prefix { get; set; }

            internal bool F3Prefix { get; set; }

            internal bool SizeOverridePrefix { get; set; }

            public byte VexRegister { get; set; }

            public bool VexLong { get; set; } // If true, use YMM or 256-bit memory access.
        }

        //$REVIEW: Instructions longer than this cause exceptions on modern x86 processors.
        // On 8086's though you could have an aribitrary number of prefixes.
        private const int MaxInstructionLength = 15;

        private readonly IServiceProvider services;
        private readonly Decoder[] rootDecoders;
        private readonly ProcessorMode mode;
		private readonly PrimitiveType defaultDataWidth;
		private readonly PrimitiveType defaultAddressWidth;
		private readonly EndianImageReader rdr;

        private readonly bool isRegisterExtensionEnabled;
        private readonly X86InstructionDecodeInfo decodingContext;

        private Address addr;
        private long rdrOffset;

		/// <summary>
		/// Creates a disassembler that uses the specified reader to fetch bytes
        /// from the program image.
        /// </summary>
		/// <param name="width">Default address and data widths. PrimitiveType.Word16 for 
        /// 16-bit operation, PrimitiveType.Word32 for 32-bit operation.</param>
		public X86Disassembler(
            IServiceProvider services,
            Decoder[] rootDecoders,
            ProcessorMode mode,
            EndianImageReader rdr,
            PrimitiveType defaultWordSize,
            PrimitiveType defaultAddressSize,
            bool useRexPrefix)
		{
            this.services = services;
            this.rootDecoders = rootDecoders;
            Debug.Assert(rootDecoders != null);
            this.mode = mode;
			this.rdr = rdr;
            this.addr = rdr.Address;
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
        public override X86Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.rdrOffset = rdr.Offset;
            if (!rdr.TryReadByte(out byte op))
                return null;

            // Reset the state of the currentInstruction
            this.decodingContext.Reset();
            this.decodingContext.dataWidth = defaultDataWidth;
            this.decodingContext.addressWidth = defaultAddressWidth;
            this.decodingContext.iWidth = defaultDataWidth;

            X86Instruction instr = rootDecoders[op].Decode(op, this);
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }

        public override X86Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return decodingContext.MakeInstruction(iclass, mnemonic);
        }

        public override X86Instruction CreateInvalidInstruction()
        {
            return new X86Instruction(Mnemonic.illegal, InstrClass.Invalid, decodingContext.dataWidth, decodingContext.addressWidth);
        }

        public override X86Instruction NotYetImplemented(string message)
        {
            var testGenSvc = services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("X86Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryReadByte(out byte b)
        {
            return (rdr.TryReadByte(out b) &&
                rdr.Offset - rdrOffset <= MaxInstructionLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryReadLeUInt16(out ushort us)
        {
            return rdr.TryReadLeUInt16(out us) &&
                rdr.Offset - rdrOffset <= MaxInstructionLength;
        }

        private bool TryReadLe(PrimitiveType offsetWidth, [MaybeNullWhen(false)] out Constant? offset)
        {
            offset = null;
            if (!rdr.IsValidOffset(rdr.Offset + (uint) offsetWidth.Size - 1))
                return false;
            if (!rdr.TryReadLe(offsetWidth, out offset))
                return false;
            return (rdr.Offset - this.rdrOffset <= MaxInstructionLength);
        }

        private RegisterStorage RegFromBitsRexB(int bits, PrimitiveType dataWidth)
        {
            int reg_bits = bits & 7;
            reg_bits |= this.decodingContext.RegisterExtension.FlagTargetModrmRegOrMem ? 8 : 0;
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

        /// <summary>
        /// Absolute memory address
        /// </summary>
        public static bool Ap(uint op, X86Disassembler dasm)
        {
            if (!dasm.TryReadLeUInt16(out ushort off))
                return false;
            if (!dasm.TryReadLeUInt16(out ushort seg))
                return false;
            var addr = dasm.mode.CreateSegmentedAddress(seg, off);
            if (addr == null)
                return false;
            dasm.decodingContext.ops.Add(AddressOperand.Create(addr));
            return true;
        }

        /// <summary>
        /// Control register encoded in the reg field.
        /// </summary>
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

        /// <summary>
        /// Debug register encoded in the reg field.
        /// </summary>
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

        /// <summary>
        /// Memory or register operand specified by mod & r/m fields.
        /// </summary>
        private static Mutator<X86Disassembler> E(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
                d.decodingContext.iWidth = width;
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.GpRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Eb = E(OperandType.b);
        private static readonly Mutator<X86Disassembler> Ed = E(OperandType.d);
        private static readonly Mutator<X86Disassembler> Ep = E(OperandType.p);
        private static readonly Mutator<X86Disassembler> Eq = E(OperandType.q);
        private static readonly Mutator<X86Disassembler> Ev = E(OperandType.v);
        private static readonly Mutator<X86Disassembler> Ey = E(OperandType.y);
        private static readonly Mutator<X86Disassembler> Ew = E(OperandType.w);

        /// <summary>
        /// Hybrid decoding: if effective memory address, use word16 size, if register 
        /// use the current GP register size.
        /// </summary>
        public static bool Ewv(uint top, X86Disassembler dasm)
        {
            if (!dasm.TryEnsureModRM(out byte modRm))
                return false;
            PrimitiveType width;
            if ((modRm & 0xC0) == 0xC0)
                width = dasm.decodingContext.dataWidth;
            else
                width = PrimitiveType.Word16;
            var op = dasm.DecodeModRM(width, dasm.decodingContext.SegmentOverride, dasm.GpRegFromBits);
            if (op is null)
                return false;
            dasm.decodingContext.ops.Add(op);
            return true;
        }

        /// <summary>
        /// B: The VEX.vvvv field of the VEX prefix selects a general purpose register
        /// </summary>
        private static Mutator<X86Disassembler> B(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
                d.decodingContext.iWidth = width;
                var op = d.GpRegFromBits(d.decodingContext.VexRegister, width);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(new RegisterOperand(op));
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> By = B(OperandType.y);

        /// <summary>
        /// Floating-point ST(x)
        /// </summary>
        private static bool F(uint op, X86Disassembler d)
        {
            if (!d.TryEnsureModRM(out byte modRm))
                return false;
            d.decodingContext.ops.Add(new FpuOperand(modRm & 0x07));
            return true;
        }

        /// <summary>
        /// Floating-point ST(0)
        /// </summary>
        private static bool f(uint op, X86Disassembler d)
        {
            d.decodingContext.ops.Add(new FpuOperand(0));
            return true;
        }

        /// <summary>
        /// General purpose register operand specified by the reg field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> G(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
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

        private static readonly Mutator<X86Disassembler> Gb = G(OperandType.b);
        private static readonly Mutator<X86Disassembler> Gd = G(OperandType.d);
        private static readonly Mutator<X86Disassembler> Gv = G(OperandType.v);
        private static readonly Mutator<X86Disassembler> Gw = G(OperandType.w);
        private static readonly Mutator<X86Disassembler> Gy = G(OperandType.y);

        /// <summary>
        /// If VEX encoding, use vvvv register, otherwise ignore.
        /// </summary>
        private static Mutator<X86Disassembler> H(OperandType opType)
        {
            return (u, d) => {
                if (d.decodingContext.IsVex)
                {
                    var width = d.SseOperandWidth(opType);
                    var op = new RegisterOperand(d.XmmRegFromBits(
                        d.decodingContext.VexRegister,
                        width));
                    d.decodingContext.ops.Add(op);
                }
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Hdq = H(OperandType.dq);
        private static readonly Mutator<X86Disassembler> Hpd = H(OperandType.pd);
        private static readonly Mutator<X86Disassembler> Hps = H(OperandType.ps);
        private static readonly Mutator<X86Disassembler> Hqq = H(OperandType.qq);
        private static readonly Mutator<X86Disassembler> Hsd = H(OperandType.sd);
        private static readonly Mutator<X86Disassembler> Hss = H(OperandType.ss);
        private static readonly Mutator<X86Disassembler> Hq = H(OperandType.q);
        private static readonly Mutator<X86Disassembler> Hx = H(OperandType.x);

        /// <summary>
        /// Immediate operand.
        /// </summary>
        private static Mutator<X86Disassembler> I(OperandType opType)
        {
            return (u, d) =>
            {
                PrimitiveType width;
                var ops = d.decodingContext.ops;
                if (opType == OperandType.x)
                {
                    width = ops[ops.Count - 1].Width;
                    d.decodingContext.iWidth = width;
                }
                else
                { 
                    width = d.OperandWidth(opType); //  Don't use the width of the previous operand.
                }
                var op = d.CreateImmediateOperand(width);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Ib = I(OperandType.b);
        private static readonly Mutator<X86Disassembler> Iv = I(OperandType.v);
        private static readonly Mutator<X86Disassembler> Iw = I(OperandType.w);
        private static readonly Mutator<X86Disassembler> Ix = I(OperandType.x);
        private static readonly Mutator<X86Disassembler> Iz = I(OperandType.z);

        /// <summary>
        /// Relative ("near") jump.
        /// </summary>
        private static Mutator<X86Disassembler> J(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
                if (!d.TryReadLe(width, out Constant? cOffset))
                    return false;
                long jOffset = cOffset!.ToInt64();
                ulong uAddr = (ulong) ((long) d.rdr.Address.Offset + jOffset);
                MachineOperand op;
                if (d.defaultAddressWidth.BitSize == 64)      //$REVIEW: not too keen on the switch statement here.
                    op = AddressOperand.Ptr64(uAddr);
                else if (d.defaultAddressWidth.BitSize == 32)
                    op = AddressOperand.Ptr32((uint) uAddr);
                else
                {
                    op = new ImmediateOperand(Constant.Create(PrimitiveType.Offset16, uAddr));
                }
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Jb = J(OperandType.b);
        private static readonly Mutator<X86Disassembler> Jv = J(OperandType.v);

        /// <summary>
        /// The upper 4 bits of the 8-bit immediate selects a 128-bit XMM register or a 256-bit YMM register, determined
        /// by operand type.
        /// </summary>
        public static bool Lx(uint op, X86Disassembler d)
        {
            if (!d.TryReadByte(out var lReg))
                return false;
            var ops = d.decodingContext.ops;
            var width =  ops[ops.Count-1].Width; // Use width of the previous operand.
            d.decodingContext.iWidth = width;
            //width = OperandWidth(strFormat, ref i); //  Don't use the width of the previous operand.
            var operand = new RegisterOperand(d.XmmRegFromBits((lReg >> 4) & 0xF, width));
            ops.Add(operand);
            return true;
        }

        /// <summary>
        /// modRM may only refer to memory.
        /// </summary>
        private static Mutator<X86Disassembler> M(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                if ((modRm & 0xC0) == 0xC0)
                    return false;
                if (!(d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.GpRegFromBits) is MemoryOperand mem))
                    return false;
                d.decodingContext.ops.Add(mem);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> MB = M(OperandType.B);
        private static readonly Mutator<X86Disassembler> Mb = M(OperandType.b);
        private static readonly Mutator<X86Disassembler> Md = M(OperandType.d);
        private static readonly Mutator<X86Disassembler> Mdq = M(OperandType.dq);
        private static readonly Mutator<X86Disassembler> Mf = M(OperandType.f);
        private static readonly Mutator<X86Disassembler> Mg = M(OperandType.g);
        private static readonly Mutator<X86Disassembler> Mh = M(OperandType.h);
        private static readonly Mutator<X86Disassembler> Mp = M(OperandType.p);
        private static readonly Mutator<X86Disassembler> Mpd = M(OperandType.pd);
        private static readonly Mutator<X86Disassembler> Mps = M(OperandType.ps);
        private static readonly Mutator<X86Disassembler> Mq = M(OperandType.q);
        private static readonly Mutator<X86Disassembler> Ms = M(OperandType.s);
        private static readonly Mutator<X86Disassembler> Mv = M(OperandType.v);
        private static readonly Mutator<X86Disassembler> Mw = M(OperandType.w);
        private static readonly Mutator<X86Disassembler> Mx = M(OperandType.x);
        private static readonly Mutator<X86Disassembler> My = M(OperandType.y);

        /// <summary>
        /// MMX register operand specified by the r/m field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> N(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.SseOperandWidth(opType);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(d.RegFromBitsRexR(modRm, width, d.MmxRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<X86Disassembler> Nq = N(OperandType.q);

        /// <summary>
        /// Offset of the operand is encoded directly after the opcode.
        /// </summary>
        private static Mutator<X86Disassembler> O(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
                if (!d.TryReadLe(d.decodingContext.addressWidth, out var offset))
                    return false;
                var memOp = new MemoryOperand(width, offset)
                {
                    SegOverride = d.decodingContext.SegmentOverride
                };
                d.decodingContext.ops.Add(memOp);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Ob = O(OperandType.b);
        private static readonly Mutator<X86Disassembler> Ov = O(OperandType.v);

        /// <summary>
        /// MMX register operand specified by the reg field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> P(OperandType opType)
        {
            return (u, d) => {
                var width = d.SseOperandWidth(opType);
                if (!d.TryEnsureModRM(out byte modRm))
                    return false;
                var op = new RegisterOperand(d.RegFromBitsRexR(modRm >> 3, width, d.MmxRegFromBits));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Pd = P(OperandType.d);
        private static readonly Mutator<X86Disassembler> Ppi = P(OperandType.pi);
        private static readonly Mutator<X86Disassembler> Pq = P(OperandType.q);
        private static readonly Mutator<X86Disassembler> Py = P(OperandType.y);

        /// <summary>
        /// Memory or register MMX operand specified by mod & r/m fields.
        /// </summary>
        private static Mutator<X86Disassembler> Q(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.SseOperandWidth(opType);
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.MmxRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Qd = Q(OperandType.d);
        private static readonly Mutator<X86Disassembler> Qpi = Q(OperandType.pi);
        private static readonly Mutator<X86Disassembler> Qq = Q(OperandType.q);

        /// <summary>
        /// Register operand specified by the mod field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> R(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.OperandWidth(opType);
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

        private static readonly Mutator<X86Disassembler> Rv = R(OperandType.v);
        private static readonly Mutator<X86Disassembler> Rw = R(OperandType.w);
        private static readonly Mutator<X86Disassembler> Rd = R(OperandType.d);
        private static readonly Mutator<X86Disassembler> Rq = R(OperandType.q);
        private static readonly Mutator<X86Disassembler> Ry = R(OperandType.y);

        /// <summary>
        /// XMM operand specified by the modRm field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> U(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.SseOperandWidth(opType);
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

        private static readonly Mutator<X86Disassembler> Udq = U(OperandType.dq);
        private static readonly Mutator<X86Disassembler> Upd = U(OperandType.pd);
        private static readonly Mutator<X86Disassembler> Ups = U(OperandType.ps);
        private static readonly Mutator<X86Disassembler> Ux = U(OperandType.x);

        /// <summary>
        /// XMM operand specified by the reg field of the modRM byte.
        /// </summary>
        private static Mutator<X86Disassembler> V(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.SseOperandWidth(opType);
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


        private static readonly Mutator<X86Disassembler> Vdq = V(OperandType.dq);
        private static readonly Mutator<X86Disassembler> Vpd = V(OperandType.pd);
        private static readonly Mutator<X86Disassembler> Vps = V(OperandType.ps);
        private static readonly Mutator<X86Disassembler> Vsd = V(OperandType.sd);
        private static readonly Mutator<X86Disassembler> Vss = V(OperandType.ss);
        private static readonly Mutator<X86Disassembler> Vq = V(OperandType.q);
        private static readonly Mutator<X86Disassembler> Vqq = V(OperandType.qq);
        private static readonly Mutator<X86Disassembler> Vx = V(OperandType.x);
        private static readonly Mutator<X86Disassembler> Vy = V(OperandType.y);

        /// <summary>
        /// Memory or XMM operand specified by mod & r/m fields.
        /// </summary>
        private static Mutator<X86Disassembler> W(OperandType opType)
        {
            return (u, d) =>
            {
                var width = d.SseOperandWidth(opType);
                var op = d.DecodeModRM(width, d.decodingContext.SegmentOverride, d.XmmRegFromBits);
                if (op == null)
                    return false;
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> Wd = W(OperandType.d);
        private static readonly Mutator<X86Disassembler> Wdq = W(OperandType.dq);
        private static readonly Mutator<X86Disassembler> Wpd = W(OperandType.pd);
        private static readonly Mutator<X86Disassembler> Wps = W(OperandType.ps);
        private static readonly Mutator<X86Disassembler> Wq = W(OperandType.q);
        private static readonly Mutator<X86Disassembler> Wqq = W(OperandType.qq);
        private static readonly Mutator<X86Disassembler> Wsd = W(OperandType.sd);
        private static readonly Mutator<X86Disassembler> Wss = W(OperandType.ss);
        private static readonly Mutator<X86Disassembler> Wx = W(OperandType.x);
        private static readonly Mutator<X86Disassembler> Wy = W(OperandType.y);

        /// <summary>
        /// Segment register encoded by reg field of modRM byte.
        /// </summary>
        public static bool Sw(uint op, X86Disassembler d)
        {
            if (!d.TryEnsureModRM(out byte modRm))
                return false;
            d.decodingContext.ops.Add(new RegisterOperand(SegFromBits(modRm >> 3)));
            return true;
        }

        /// <summary>
        /// Implicit use of accumulator.
        /// </summary>
        private static bool AL(uint uInstr, X86Disassembler dasm)
        {
            dasm.decodingContext.ops.Add(new RegisterOperand(Registers.al));
            return true;
        }

        private static bool AX(uint uInstr, X86Disassembler dasm)
        {
            dasm.decodingContext.ops.Add(new RegisterOperand(Registers.ax));
            return true;
        }

        private static bool eAX(uint uInstr, X86Disassembler dasm)
        {
            RegisterStorage reg;
            if (dasm.decodingContext.dataWidth.BitSize == 16)
                reg = Registers.ax;
            else
                reg = Registers.eax;
            dasm.decodingContext.ops.Add(new RegisterOperand(reg));
            return true;
        }

        private static bool rAX(uint uInstr, X86Disassembler dasm)
        {
            RegisterStorage reg;
            var bitsize = dasm.decodingContext.dataWidth.BitSize;
            if (bitsize == 16)
                reg = Registers.ax;
            else if (bitsize == 32)
                reg = Registers.eax;
            else
                reg = Registers.rax;
            dasm.decodingContext.ops.Add(new RegisterOperand(reg));
            return true;
        }

        internal static bool rV(uint uInstr, X86Disassembler dasm)
        {
            RegisterStorage reg;
            if (dasm.decodingContext.SizeOverridePrefix)
                reg = dasm.RegFromBitsRexB((int)uInstr & 7, dasm.decodingContext.dataWidth);
            else
                reg = dasm.RegFromBitsRexB((int) uInstr & 7, dasm.mode.WordWidth);
            dasm.decodingContext.ops.Add(new RegisterOperand(reg));
            return true;
        }
        /*                d.decodingContext.iWidth = d.OperandWidth(width);
                var op = new RegisterOperand(d.RegFromBitsRexB(
                    (byte)u, 
                    d.decodingContext.iWidth));
                d.decodingContext.ops.Add(op);
*/

        /// <summary>
        /// Force the current data width to 'byte'.
        /// </summary>
        private static bool b(uint op, X86Disassembler d)
        {
            d.decodingContext.iWidth = PrimitiveType.Byte;
            return true;
        }

        /// <summary>
        /// Implicit use of CL.
        /// </summary>
        private static bool c(uint u, X86Disassembler d)
        {
            d.decodingContext.ops.Add(new RegisterOperand(Registers.cl));
            return true;
        }

        /// <summary>
        /// Implicit use of DX or EDX.
        /// </summary>
        private static bool DX(uint u, X86Disassembler dasm)
        {
            dasm.decodingContext.ops.Add(new RegisterOperand(Registers.dx));
            return true;
        }

        private static bool rDX(uint u, X86Disassembler dasm)
        {
            RegisterStorage reg;
            var bitsize = dasm.decodingContext.dataWidth.BitSize;
            if (bitsize == 16)
            {
                reg = Registers.dx;
            }
            else
            {
                if (dasm.decodingContext.RegisterExtension.FlagWideValue)
                    reg = Registers.rdx;
                else
                    reg = Registers.edx;
            }
            dasm.decodingContext.ops.Add(new RegisterOperand(reg));
            return true;
        }

        /// <summary>
        /// Constant 1
        /// </summary>
        private static bool n1(uint u, X86Disassembler d)
        {
            var op = new ImmediateOperand(Constant.Byte(1));
            d.decodingContext.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Constant 3
        /// </summary>
        private static bool n3(uint u, X86Disassembler d)
        {
            var op = new ImmediateOperand(Constant.Byte(3));
            d.decodingContext.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Register encoded as last 3 bits of instruction.
        /// </summary>
        private static Mutator<X86Disassembler> r(OperandType width)
        {
            return (u, d) =>
            {
                d.decodingContext.iWidth = d.OperandWidth(width);
                var op = new RegisterOperand(d.RegFromBitsRexB(
                    (byte)u, 
                    d.decodingContext.iWidth));
                d.decodingContext.ops.Add(op);
                return true;
            };
        }

        private static readonly Mutator<X86Disassembler> rb = r(OperandType.b);
        private static readonly Mutator<X86Disassembler> rv = r(OperandType.v);
        private static readonly Mutator<X86Disassembler> rw = r(OperandType.w);


        /// <summary>
        /// Specific register encoding.
        /// </summary>
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

        public static InstrDecoder<X86Disassembler, Mnemonic, X86Instruction> Instr(Mnemonic mnemonic, params Mutator<X86Disassembler> [] mutators)
        {
            return new InstrDecoder<X86Disassembler, Mnemonic, X86Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        public static InstrDecoder<X86Disassembler, Mnemonic, X86Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<X86Disassembler> [] mutators)
        {
            return new InstrDecoder<X86Disassembler, Mnemonic, X86Instruction>(iclass, mnemonic, mutators);
        }

        public static MemRegDecoder MemReg(Decoder mem, Decoder reg)
        {
            return new MemRegDecoder(mem, reg);
        }

        /// <summary>
        /// Use the <see cref="NyiDecoder{TDasm, TMnemonic, TInstr}"/> to mark instructions for which no decoder has 
        /// been written yet.
        /// </summary>
        /// <remarks>
        /// The x86 instruction set is large and keeps growing....
        /// </remarks>
        public static NyiDecoder<X86Disassembler, Mnemonic, X86Instruction> nyi(string message)
        {
            return new NyiDecoder<X86Disassembler, Mnemonic, X86Instruction>(message);
        }

		/// <summary>
		/// If the ModR/M byte hasn't been read yet, do so now.
		/// </summary>
		/// <returns></returns>
		private bool TryEnsureModRM(out byte modRm)
		{
            if (!this.decodingContext.IsModRegMemByteActive())
            {
                if (!TryReadByte(out byte modrm))
                {
                    modRm = 0;
                    return false;
                }
                this.decodingContext.ModRegMemByte = modrm;
            }
            modRm = this.decodingContext.ModRegMemByte;
            return true;
		}

        // Operand types as defined by the Intel manual section A.2.2
        private enum OperandType
        {
            None,
            a,      // Two one-word operands in memory or two double-word operands in memory, depending on operand-size
                    // attribute(used only by the BOUND instruction).
            b,      // Byte, regardless of operand-size attribute.
            B,      // BCD tenbyte - Reko extension
            c,      // Byte or word, depending on operand-size attribute.
            d,      // Doubleword, regardless of operand-size attribute.
            dq,     // Double-quadword, regardless of operand-size attribute.
            f,      // 32-bit floating point - Reko extension
            g,      // 64-bit floating point - Reko extension
            h,      // 80-bit floating point - Reko extension
            p,      // 32-bit, 48-bit, or 80-bit pointer, depending on operand-size attribute.
            pd,     // 128-bit or 256-bit packed double-precision floating-point data.
            pi,     // Quadword MMX technology register (for example: mm0).
            ps,     // 128-bit or 256-bit packed single-precision floating-point data.
            q,      // Quadword, regardless of operand-size attribute.
            qq,     // Quad-Quadword (256-bits), regardless of operand-size attribute.
            s,      // 6-byte or 10-byte pseudo-descriptor.
            sd,     // Scalar element of a 128-bit double-precision floating data.
            ss,     // Scalar element of a 128-bit single-precision floating data.
            si,     // Doubleword integer register (for example: eax).
            v,      // Word, doubleword or quadword(in 64-bit mode), depending on operand-size attribute.
            w,      // Word, regardless of operand-size attribute.
            x,      // dq or qq based on the operand-size attribute.
            y,      // Doubleword or quadword (in 64-bit mode), depending on operand-size attribute.
            z,      // Word for 16-bit operand-size or doubleword for 32 or 64-bit operand-size.
        }

		/// <summary>
		/// Returns the operand width of the operand type.
		/// </summary>
        /// <remarks>
        /// This extends the specification of Intel's section A.2.2 "Codes for operand type"
        /// to include BCD80.
        /// </remarks>
		private PrimitiveType OperandWidth(OperandType fmt)
		{
			switch (fmt)
			{
			default:
                throw new ArgumentOutOfRangeException($"Unknown operand width specifier '{fmt}'.");
			case OperandType.b:
				return PrimitiveType.Byte;
            case OperandType.B:
                return PrimitiveType.Bcd80;
			case OperandType.v:
	    		return decodingContext.dataWidth;
            case OperandType.w:
				return PrimitiveType.Word16;
            case OperandType.dq:
                return PrimitiveType.Word128;
            case OperandType.d:
                return PrimitiveType.Word32;
            case OperandType.ps:
            case OperandType.pd:
                return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128;
            case OperandType.p:
				return PrimitiveType.Ptr32;     // Far pointer.
            case OperandType.f:
                return PrimitiveType.Real32;
            case OperandType.g:
				return PrimitiveType.Real64;
			case OperandType.h:
                return PrimitiveType.Real80;
            case OperandType.q:
                return PrimitiveType.Word64;
            case OperandType.s:
                return this.decodingContext.dataWidth.BitSize == 64 ? PrimitiveType.CreateWord(80) : PrimitiveType.CreateWord(48);
            case OperandType.x:
                return this.decodingContext.VexLong
                    ? PrimitiveType.Word256
                    : PrimitiveType.Word128;
            case OperandType.y:
                return (this.isRegisterExtensionEnabled && this.decodingContext.RegisterExtension.FlagWideValue) ? PrimitiveType.Word64: PrimitiveType.Word32;
            case OperandType.z:
                return this.decodingContext.dataWidth.BitSize == 64 ? PrimitiveType.Int32 : this.decodingContext.dataWidth;
            }
		}

        private PrimitiveType SseOperandWidth(OperandType fmt)
        {
            switch (fmt)
            {
            default:
                throw new ArgumentOutOfRangeException($"Unknown SSE operand width '{fmt}'.");
            case OperandType.dq:
                 return PrimitiveType.Word128;
            case OperandType.d:
                return PrimitiveType.Word32;
            case OperandType.pd:
                return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[2] of double32
            case OperandType.pi:
                return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[4] of int32
            case OperandType.ps:
                return this.decodingContext.VexLong ? PrimitiveType.Word256 : PrimitiveType.Word128; //$TODO: this should be array[4] of real32
            case OperandType.qq:
                return PrimitiveType.Word256;
            case OperandType.q:
                return PrimitiveType.Word64;
            case OperandType.sd:
                return PrimitiveType.Real64;
            case OperandType.si:
                return PrimitiveType.Int32;
            case OperandType.ss:
                return PrimitiveType.Real32;
            case OperandType.x:
                return this.decodingContext.VexLong
                    ? PrimitiveType.Word256
                    : PrimitiveType.Word128;
            case OperandType.y:
                return this.decodingContext.SizeOverridePrefix
                    ? PrimitiveType.Word128
                    : PrimitiveType.Word64;
            }
        }

		private static readonly RegisterStorage [] s_ma16Base = 
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

		private static readonly RegisterStorage [] s_ma16Index =
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

		public ImmediateOperand? CreateImmediateOperand(PrimitiveType immWidth)
		{
            if (!TryReadLe(immWidth, out Constant? c))
                return null;
			return new ImmediateOperand(c!);
		}

		private MachineOperand? DecodeModRM(PrimitiveType dataWidth, RegisterStorage segOverride, Func<int, PrimitiveType, RegisterStorage> regFn)
		{
            if (!TryEnsureModRM(out byte modRm))
                return null;

			int rm = this.decodingContext.ModRegMemByte & 0x07;
			int mod = this.decodingContext.ModRegMemByte >> 6;

			RegisterStorage b;
            RegisterStorage idx;
			byte scale = 1;
			PrimitiveType? offsetWidth = null;

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

                    if (!TryReadByte(out byte sib))
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

            Constant? offset;
            if (offsetWidth != null)
            {
                if (!TryReadLe(offsetWidth, out offset))
                    return null;
            }
            else
            {
                offset = null;
            }

            return new MemoryOperand(dataWidth, offset)
            {
                Base = b,
                Index = idx,
                Scale = scale,
                SegOverride = segOverride,
            };
		}

#nullable disable
        static X86Disassembler()
		{
		}
#nullable enable
    }
}
