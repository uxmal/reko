#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
    public abstract class MipsProcessorArchitecture : ProcessorArchitecture
    {
        public Dictionary<uint, RegisterStorage> fpuCtrlRegs;
        public RegisterStorage[] GeneralRegs;
        public RegisterStorage[] fpuRegs;
        public RegisterStorage[] ccRegs;
        public RegisterStorage[] fpuCcRegs;
        public RegisterStorage LinkRegister;
        public RegisterStorage hi;
        public RegisterStorage lo;
        public RegisterStorage pc;
        protected ulong uCodeAddressMask;
        private string? instructionSetEncoding;
        private Decoder<MipsDisassembler, Mnemonic, MipsInstruction>? rootDecoder;

        public MipsProcessorArchitecture(IServiceProvider services, string archId, EndianServices endianness, PrimitiveType wordSize, PrimitiveType ptrSize, Dictionary<string, object> options) 
            : base(services, archId, options, null!, null!)
        {
            this.Endianness = endianness;
            this.WordWidth = wordSize;
            this.PointerType = ptrSize;
            this.FramePointerType = ptrSize;
            this.InstructionBitSize = 32;
            this.GeneralRegs = CreateGeneralRegisters().ToArray();
            this.StackRegister = GeneralRegs[29];
            this.LinkRegister = GeneralRegs[31];

            this.hi = new RegisterStorage("hi", 32, 0, wordSize);
            this.lo = new RegisterStorage("lo", 33, 0, wordSize);
            this.pc = new RegisterStorage("pc", 34, 0, wordSize);
            this.fpuRegs = CreateFpuRegisters();
            this.FCSR = RegisterStorage.Reg32("FCSR", 0x201F);
            this.ccRegs = CreateCcRegs();
            this.fpuCcRegs = CreateFpuCcRegs();
            this.fpuCtrlRegs = new Dictionary<uint, RegisterStorage>
            {
                { 0x1F, FCSR }
            };

            this.regsByName = GeneralRegs
                .Concat(fpuRegs)
                .Concat(fpuCtrlRegs.Values)
                .Concat(ccRegs)
                .Concat(new[] { hi, lo })
                .ToDictionary(k => k.Name);
            uCodeAddressMask = ~3ul;

            LoadUserOptions(options);
            if (this.Intrinsics is null)
                Intrinsics = new MipsIntrinsics(this);
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
                    var factory = new MipsDisassembler.DecoderFactory(this.instructionSetEncoding);
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

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            var i = domain - StorageDomain.Register;
            return GetRegister(i);
        }

        public RegisterStorage? GetRegister(int i)
        {
            if (i >= GeneralRegs.Length)
                return null;
            return GeneralRegs[i];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return GeneralRegs;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotSupportedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            this.Options = options ?? new Dictionary<string, object>();
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

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            if (grf != 0)   // MIPS has no traditional status register.
                throw new NotSupportedException();
            return "";
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

        private IEnumerable<RegisterStorage> CreateGeneralRegisters()
        {
            return from i in Enumerable.Range(0, 32)
                join name in new[] {
                    new { id = 29, n = "sp" },
                    new { id = 31, n = "ra" }
                } on i equals name.id into names
                from name in names.DefaultIfEmpty()
                select new RegisterStorage(
                    name is not null 
                        ? name.n 
                        : string.Format("r{0}", i),
                    i,
                    0,
                    WordWidth);
        }

        private RegisterStorage[] CreateFpuRegisters()
        {
            return Enumerable.Range(0, 32)
                .Select(i => new RegisterStorage(
                    string.Format("f{0}", i),
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

    public class MipsBe32Architecture : MipsProcessorArchitecture
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

    public class MipsLe32Architecture : MipsProcessorArchitecture
    {
        public MipsLe32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Little, PrimitiveType.Word32, PrimitiveType.Ptr32, options) 
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

    public class MipsBe64Architecture : MipsProcessorArchitecture
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

    public class MipsLe64Architecture : MipsProcessorArchitecture
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