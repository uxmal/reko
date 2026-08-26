#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// Playstation Geometry transformation engine https://psx-spx.consoledev.net/geometrytransformationenginegte/

namespace Reko.Arch.Mips
{
    /// <summary>
    /// MIPS processor architecture
    /// </summary>
    /// <remarks>
    /// R4000 = MIPS III instruction set
    /// MIPS16 = compact 16-bit encoding
    /// </remarks>
    public abstract class MipsArchitecture : ProcessorArchitecture
    {
        public Dictionary<uint, RegisterStorage> fpuCtrlRegs;
        public RegisterStorage[] GeneralRegs;
        public RegisterStorage[] fpuRegs;
        public RegisterStorage[] ccRegs;
        public RegisterStorage[] fpuCcRegs;
        public RegisterStorage LinkRegister;
        public RegisterStorage hi = null!;
        public RegisterStorage lo = null!;
        public RegisterStorage? hi1;
        public RegisterStorage? lo1;
        public RegisterStorage pc;
        protected ulong uCodeAddressMask;
        private string? instructionSetEncoding;
        private Decoder<MipsDisassembler, Mnemonic, MipsInstruction>? rootDecoder;

        public MipsArchitecture(IServiceProvider services, string archId, EndianServices endianness, PrimitiveType wordSize, PrimitiveType ptrSize, Dictionary<string, object> options) 
            : base(services, archId, options, new([]))
        {
            this.Endianness = endianness;
            this.WordWidth = wordSize;
            this.PointerType = ptrSize;
            this.FramePointerType = ptrSize;
            this.InstructionBitSize = 32;
            this.GeneralRegs = CreateGeneralRegisters(options).ToArray();
            this.StackRegister = GeneralRegs[29];
            this.LinkRegister = GeneralRegs[31];

            this.pc = new RegisterStorage("pc", 34, 0, this.PointerType);
            this.fpuRegs = CreateFpuRegisters();
            this.FCSR = RegisterStorage.Reg32("FCSR", 0x201F);
            this.ccRegs = CreateCcRegs();
            this.fpuCcRegs = CreateFpuCcRegs();
            this.fpuCtrlRegs = new Dictionary<uint, RegisterStorage>
            {
                { 0x1F, FCSR }
            };

            var regs = GeneralRegs
                .Concat(fpuRegs)
                .Concat(fpuCtrlRegs.Values)
                .Concat(ccRegs)
                .Concat([ hi, lo ]);
            if (hi1 is not null && lo1 is not null)
            {
                regs = regs.Concat([hi1, lo1]);
            }
            var aliases = GenerateRegisterAliases(GeneralRegs);
            this.RegisterBank = new RegisterBank(regs, aliases);
            uCodeAddressMask = ~3ul;

            LoadUserOptions(options);
            if (this.Intrinsics is null)
                Intrinsics = new MipsIntrinsics(this);
        }

        private Dictionary<string, RegisterStorage> GenerateRegisterAliases(RegisterStorage[] generalRegs)
        {
            return new()
            {
                { "zero", generalRegs[0] },
                { "at", generalRegs[1] },

                { "v0", generalRegs[2] },
                { "v1", generalRegs[3] },

                { "a0", generalRegs[4] },
                { "a1", generalRegs[5] },
                { "a2", generalRegs[6] },
                { "a3", generalRegs[7] },

                { "t0", generalRegs[8] },
                { "t1", generalRegs[9] },
                { "t2", generalRegs[10] },
                { "t3", generalRegs[11] },
                { "t4", generalRegs[12] },
                { "t5", generalRegs[13] },
                { "t6", generalRegs[14] },
                { "t7", generalRegs[15] },
                { "s0", generalRegs[16] },
                { "s1", generalRegs[17] },
                { "s2", generalRegs[18] },
                { "s3", generalRegs[19] },
                { "s4", generalRegs[20] },
                { "s5", generalRegs[21] },
                { "s6", generalRegs[22] },
                { "s7", generalRegs[23] },
                { "t8", generalRegs[24] },
                { "t9", generalRegs[25] },
                { "k0", generalRegs[26] },
                { "k1", generalRegs[27] }
            };
        }

        public RegisterStorage FCSR { get; private set; }

        public MipsIntrinsics? Intrinsics { get; private set; }


        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            var dasm = CreateDisassemblerInternal(imageReader);
            return new LongConstantFuser(dasm);
        }

        private IEnumerable<MipsInstruction> CreateDisassemblerInternal(EndianImageReader imageReader)
        {
            switch (this.instructionSetEncoding)
            {
            case "micro": return new MicroMipsDisassembler(this, imageReader);
            case "mips16e": return new Mips16eDisassembler(this, imageReader);
            case "nano": return new NanoMipsDisassembler(this, imageReader);
            default:
                if (rootDecoder is null)
                {
                    var factory = MipsDisassembler.DecoderFactory.Create(this.instructionSetEncoding);
                    rootDecoder = factory.CreateRootDecoder();
                }
                return new MipsDisassembler(this, rootDecoder, imageReader);
            }
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new MipsInstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new MipsProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            if (this.Intrinsics is null)
                Intrinsics = new MipsIntrinsics(this);
            if (instructionSetEncoding == "mips16e")
            {
                return new Mips16eRewriter(
                    this,
                    Intrinsics,
                    rdr,
                    CreateDisassemblerInternal(rdr),
                    binder, 
                    host);
            }
            else
            {
                return new MipsRewriter(
                    this,
                    Intrinsics,
                    rdr,
                    CreateDisassemblerInternal(rdr),
                    binder,
                    host);
            }
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (uint)a.ToLinear()).ToHashSet();
            return new MipsPointerScanner32(rdr, knownLinAddresses, flags).Select(l => Address.Ptr32(l));
        }

        // MIPS uses a link register
        public override int ReturnAddressOnStack => 0;

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }


        public RegisterStorage? GetRegister(int i)
        {
            if (i >= GeneralRegs.Length)
                return null;
            return GeneralRegs[i];
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, ulong grf)
        {
            throw new NotSupportedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            this.Options = options ?? [];
            if (Options.TryGetValue(ProcessorOption.InstructionSet, out var oDecoderName) && 
                oDecoderName is string decoderName)
            {
                this.instructionSetEncoding = decoderName;
                switch (decoderName)
                {
                case "micro":
                case "mips16e":
                case "nano":
                    this.InstructionBitSize = 16;
                    this.uCodeAddressMask = ~1ul;
                    break;
                default:
                    this.InstructionBitSize = 32;
                    this.uCodeAddressMask = ~3ul;
                    break;
                }
            }
            this.rootDecoder = null;
            this.Intrinsics = null;
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            if (rdr.TryReadUInt32(out var uaddr))
            {
                return Address.Ptr32(uaddr);
            }
            else
            {
                return null;
            }
        }

        public override ICallingConvention? GetCallingConvention(string? name)
        {
            var defaultAbi = base.StringOption(ProcessorOption.ABI) ?? "";
            if (string.Compare("o32", name, StringComparison.OrdinalIgnoreCase) == 0 ||
                (string.IsNullOrEmpty(name) && string.Compare("o32", defaultAbi, StringComparison.OrdinalIgnoreCase) == 0))
            {
                int? floatAbi = base.IntegerOption(ProcessorOption.FloatABI);
                return new O32CallingConvention(this, floatAbi is null || floatAbi.Value != 0);
            }
            if (string.Compare("nanoo32", name, StringComparison.OrdinalIgnoreCase) == 0 ||
                (string.IsNullOrEmpty(name) && string.Compare("nanoo32", defaultAbi, StringComparison.OrdinalIgnoreCase) == 0))
            {
                int? floatAbi = base.IntegerOption(ProcessorOption.FloatABI);
                return new NanoO32CallingConvention(this, floatAbi is null || floatAbi.Value != 0);
            }
            return base.GetCallingConvention(name);
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, ulong grf)
        {
            if (grf != 0)   // MIPS has no traditional status register.
                throw new NotSupportedException();
            return "";
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        private IEnumerable<RegisterStorage> CreateGeneralRegisters(Dictionary<string, object> options)
        {
            if (options.TryGetValue(ProcessorOption.InstructionSet, out var oIsa) &&
                oIsa is string isa &&
                isa == "ps2ee")
            {
                WordWidth = PrimitiveType.Word128;
                hi = new RegisterStorage("hi", 32, 0, PrimitiveType.Word64);
                lo = new RegisterStorage("lo", 33, 0, PrimitiveType.Word64);
                hi1 = new RegisterStorage("hi1", 48, 0, PrimitiveType.Word64);
                lo1 = new RegisterStorage("lo1", 49, 0, PrimitiveType.Word64);
            }
            else
            {
                hi = new RegisterStorage("hi", 32, 0, WordWidth);
                lo = new RegisterStorage("lo", 33, 0, WordWidth);
                hi1 = null;
                lo1 = null;
            }

            var dt = WordWidth;

            return from i in Enumerable.Range(0, 32)
                join name in new[] {
                    new { id = 29, n = "sp" },
                    new { id = 31, n = "ra" }
                } on i equals name.id into names
                from name in names.DefaultIfEmpty()
                select new RegisterStorage(
                    name is not null 
                        ? name.n 
                        : $"r{i}",
                    i,
                    0,
                    dt);
        }

        private RegisterStorage[] CreateFpuRegisters()
        {
            return Enumerable.Range(0, 32)
                .Select(i => new RegisterStorage(
                    $"f{i}",
                    i + 64,
                    0,
                    PrimitiveType.Word32))
                .ToArray();
        }

        private RegisterStorage[] CreateCcRegs()
        {
            return Enumerable.Range(0, 8)
                .Select(i => new RegisterStorage(
                    string.Format("cc{0}", i),
                    0x3000,
                    0,
                    PrimitiveType.Bool))
                .ToArray();
        }

        private RegisterStorage[] CreateFpuCcRegs()
        {
            return Enumerable.Range(0, 8)
                .Select(i => new RegisterStorage(
                    string.Format("fcc{0}", i),
                    0x3000,
                    0,
                    PrimitiveType.Bool))
                .ToArray();
        }
    }

    public class MipsBe32Architecture : MipsArchitecture
    {
        public MipsBe32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options) 
            : base(services, archId, EndianServices.Big,  PrimitiveType.Word32, PrimitiveType.Ptr32, options) 
        {
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= (uint)base.uCodeAddressMask;
            return Address.Ptr32(uAddr);
        }
    }

    public class MipsLe32Architecture : MipsArchitecture
    {
        public MipsLe32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Little, PrimitiveType.Word32, PrimitiveType.Ptr32, options) 
        {
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            if (c.DataType.BitSize > 64)
                c = c.Slice(PrimitiveType.Word32, 0);
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= (uint)base.uCodeAddressMask;
            return Address.Ptr32(uAddr);
        }
    }

    public class MipsBe64Architecture : MipsArchitecture
    {
        public MipsBe64Architecture(IServiceProvider services, string archId, Dictionary<string, object> options) 
            : base(services, archId, EndianServices.Big, PrimitiveType.Word64, PrimitiveType.Ptr64, options)
        { 
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt64();
            if (codeAlign)
                uAddr &= base.uCodeAddressMask;
            return Address.Ptr64(uAddr);
        }
    }

    public class MipsLe64Architecture : MipsArchitecture
    {
        public MipsLe64Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Little, PrimitiveType.Word64, PrimitiveType.Ptr64, options)
        {
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt64();
            if (codeAlign)
                uAddr &= base.uCodeAddressMask;
            return Address.Ptr64(uAddr);
        }
    }
}