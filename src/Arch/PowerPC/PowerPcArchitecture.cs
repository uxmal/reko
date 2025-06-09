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
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;

    [Designer("Reko.Arch.PowerPC.Design.PowerPCArchitectureDesigner,Reko.Arch.PowerPC.Design")]
    public abstract class PowerPcArchitecture : ProcessorArchitecture
    {
        public const int CcFieldMin = 0x40;
        public const int CcFieldMax = 0x48;

        private readonly ReadOnlyCollection<RegisterStorage> regs;
        private readonly ReadOnlyCollection<RegisterStorage> fpregs;
        private readonly ReadOnlyCollection<RegisterStorage> vregs;
        private readonly ReadOnlyCollection<RegisterStorage> cregs;
        private readonly Dictionary<int, RegisterStorage> spregs;
        private readonly Dictionary<uint, FlagGroupStorage> ccFlagGroups;

        private readonly Dictionary<string, FlagGroupStorage> ccFlagGroupsByName;
        private Decoder[]? primaryDecoders;

        public RegisterStorage lr { get; }
        public RegisterStorage ctr { get; }
        public RegisterStorage xer { get; }
        public RegisterStorage fpscr { get; }
        public RegisterStorage cr { get; }

        public FlagGroupStorage cr0 { get; }
        public FlagGroupStorage cr1 { get; }

        public RegisterStorage acc { get; }

        /// <summary>
        /// Creates an instance of PowerPcArchitecture.
        /// </summary>
        /// <param name="wordWidth">Supplies the word width of the PowerPC architecture.</param>
        public PowerPcArchitecture(IServiceProvider services, string archId, EndianServices endianness, PrimitiveType wordWidth, PrimitiveType signedWord, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            Endianness = endianness;
            WordWidth = wordWidth;
            SignedWord = signedWord;
            PointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.BitSize);
            FramePointerType = PointerType;
            InstructionBitSize = 32;

            this.lr = new RegisterStorage("lr", 0x48, 0, wordWidth);
            this.ctr = new RegisterStorage("ctr", 0x49, 0, wordWidth);
            this.xer = new RegisterStorage("xer", 0x4A, 0, wordWidth);
            this.fpscr = new RegisterStorage("fpscr", 0x4B, 0, wordWidth);

            this.cr = new RegisterStorage("cr", 0x4C, 0, wordWidth);
            this.acc = RegisterStorage.Reg64("acc", 0x4D);

            // gp regs   0..1F
            // fpu regs 20..3F
            // CR regs  40..47
            // vectors  80..FF
            fpregs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("f" + n, n + 0x20, 0, PrimitiveType.Word64))
                    .ToList());
            cregs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 8)
                    .Select(n => new RegisterStorage("cr" + n, n + CcFieldMin, 0, PrimitiveType.Byte))
                    .ToList());

            vregs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 128)        // VMX128 extension has 128 regs
                    .Select(n => new RegisterStorage("v" + n, n + 0x80, 0, PrimitiveType.Word128))
                    .ToList());

            ccFlagGroups = Enumerable.Range(0, 8)
                .Select(n => new FlagGroupStorage(cr, 0xFu << (n * 4), $"cr{n}"))
                .ToDictionary(f => f.FlagGroupBits);
            ccFlagGroupsByName = ccFlagGroups.Values
                .ToDictionary(f => f.Name);
            this.cr0 = ccFlagGroupsByName["cr0"];
            this.cr1 = ccFlagGroupsByName["cr1"];


            regs = new ReadOnlyCollection<RegisterStorage>(
                Enumerable.Range(0, 0x20)
                    .Select(n => new RegisterStorage("r" + n, n, 0, wordWidth))
                .Concat(fpregs)
                .Concat(cregs)
                .Concat(new[] { lr, ctr, xer })
                .Concat(vregs)
                .ToList());

            spregs = new Dictionary<int, RegisterStorage>
            {
                { 8, new RegisterStorage("lr", 0x0100 + 8, 0, PointerType) },
                { 9, new RegisterStorage("ctr", 0x0100 + 9, 0, WordWidth) },
                { 26, RegisterStorage.Reg32("srr0", 0x0100 + 26) },
                { 27, RegisterStorage.Reg32("srr1", 0x0100 + 27) },
            };

            //$REVIEW: using R1 as the stack register is a _convention_. It 
            // should be platform-specific at the very least.
            StackRegister = regs[1];
            LoadUserOptions(options);
        }

        public ReadOnlyCollection<RegisterStorage> Registers
        {
            get { return regs; }
        }

        public ReadOnlyCollection<RegisterStorage> FpRegisters
        {
            get { return fpregs; }
        }

        public ReadOnlyCollection<RegisterStorage> VecRegisters
        {
            get { return vregs; }
        }
        public ReadOnlyCollection<RegisterStorage> CrRegisters
        {
            get { return cregs; }
        }
        public Dictionary<int, RegisterStorage> SpRegisters
        {
            get { return spregs; }
        }

        public PrimitiveType SignedWord { get; }

        #region IProcessorArchitecture Members

        public PowerPcDisassembler CreateDisassemblerImpl(EndianImageReader rdr)
        {
            return new PowerPcDisassembler(this, EnsureDecoders(), rdr, WordWidth);
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            var dasm = new PowerPcDisassembler(this, EnsureDecoders(), rdr, WordWidth);
            return new LongConstantFuser(dasm);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return new PowerPcEmulator(this, segmentMap, envEmulator);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new PowerPcInstructionComparer(norm);
        }

        public override abstract IEnumerable<Address> CreatePointerScanner(
            SegmentMap map, 
            EndianImageReader rdr,
            IEnumerable<Address> addrs, 
            PointerScannerFlags flags);

        //public override ProcedureBase GetTrampolineDestination(EndianImageReader rdr, IRewriterHost host)
        //{
        //    var dasm = new PowerPcDisassembler(this, rdr, WordWidth);
        //    return GetTrampolineDestination(dasm, host);
        //}

        /// <summary>
        /// Detects the presence of a PowerPC trampoline and returns the imported function 
        /// that is actually being requested.
        /// </summary>
        /// <remarks>
        /// A PowerPC trampoline looks like this:
        ///     addis  rX,r0,XXXX (or oris rx,r0,XXXX)
        ///     lwz    rY,YYYY(rX)
        ///     mtctr  rY
        ///     bctr   rY
        /// When loading the ELF binary, we discovered the memory locations
        /// that will contain pointers to imported functions. If the address
        /// XXXXYYYY matches one of those memory locations, we have found a
        /// trampoline.
        /// </remarks>
        /// <param name="rdr"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public ProcedureBase? GetTrampolineDestination(IEnumerable<PowerPcInstruction> rdr, IRewriterHost host)
        {
            var e = rdr.GetEnumerator();

            if (!e.MoveNext() || (e.Current.Mnemonic != Mnemonic.addis && e.Current.Mnemonic != Mnemonic.oris))
                return null;
            var addrInstr = e.Current.Address;
            var reg = (RegisterStorage)e.Current.Operands[0];
            var uAddr = ((Constant)e.Current.Operands[2]).ToUInt32() << 16;
             
            if (!e.MoveNext() || e.Current.Mnemonic != Mnemonic.lwz)
                return null;
            if (!(e.Current.Operands[1] is MemoryOperand mem))
                return null;
            if (mem.BaseRegister != reg)
                return null;
            uAddr = (uint)((int)uAddr + mem.IntOffset());
            reg = (RegisterStorage)e.Current.Operands[0];

            if (!e.MoveNext() || e.Current.Mnemonic != Mnemonic.mtctr)
                return null;
            if ((RegisterStorage)e.Current.Operands[0] != reg)
                return null;

            if (!e.MoveNext() || e.Current.Mnemonic != Mnemonic.bcctr)
                return null;

            // We saw a thunk! now try to resolve it.

            var addr = Address.Ptr32(uAddr);
            var ep = host.GetImportedProcedure(this, addr, addrInstr);
            if (ep is not null)
                return ep;
            return host.GetInterceptedCall(this, addr);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new PowerPcState(this);
        }

        // PowerPC uses a link register
        public override int ReturnAddressOnStack => 0;

        private Decoder[] EnsureDecoders()
        {
            if (this.primaryDecoders is null)
            {
                this.Options.TryGetValue(ProcessorOption.Model, out var model);
                var iset = InstructionSet.Create((string?)model);
                this.primaryDecoders = iset.CreateDecoders();
            }
            return this.primaryDecoders;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(v => Enum.GetName(typeof(Mnemonic), v)!, v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Mnemonic result))
                return null;
            return (int)result;
        }

        public RegisterStorage? GetRegister(int i)
        {
            if (0 <= i && i < regs.Count)
                return regs[i];
            else
                return null;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return GetRegister(domain - StorageDomain.Register);
        }

        public override RegisterStorage? GetRegister(string name)
        {
            return this.regs.Where(r => r.Name == name).SingleOrDefault();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs.ToArray();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            reg = GetRegister(name);
            return reg is not null;
        }

        public FlagGroupStorage? GetCcFieldAsFlagGroup(RegisterStorage reg)
        {
            if (IsCcField(reg))
            {
                var field = reg.Number - CcFieldMin;
                var grf = 0xFu << (4 *field);
                if (this.ccFlagGroups.TryGetValue(grf, out var f))
                    return f;
            }
            return null;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            foreach (var f in ccFlagGroups)
            {
                if ((grf & f.Key) != 0)
                    return f.Value;
            }
            throw new NotImplementedException("This shouldn't happen.");
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            return ccFlagGroups.Values
                .Where(cc => cc.OverlapsWith(flags));
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            //$BUG: this needs to be better conceved. There are 
            // 32 (!) condition codes in the PowerPC architecture
            return "crX";
        }

        public bool IsCcField(RegisterStorage reg)
        {
            return CcFieldMin <= reg.Number && reg.Number < CcFieldMax;
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PowerPcRewriter(this, rdr, binder, host);
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options is null)
                return;
            foreach (var option in options.ToList())
            {
                this.Options[option.Key] = option.Value;
            }
            // Clearing primarydecoders will force the creation of a new decoder tree next time
            // a disassembler is created.
            this.primaryDecoders = null;
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return Options;
        }

        public override abstract Address MakeAddressFromConstant(Constant c, bool codeAlign);

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class PowerPcBe32Architecture : PowerPcArchitecture
    {
        public PowerPcBe32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Big, PrimitiveType.Word32, PrimitiveType.Int32, options)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            SegmentMap map, 
            EndianImageReader rdr, 
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToUInt32())
                .ToHashSet();
            return new PowerPcPointerScanner32(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr32(u));
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }

    public class PowerPcLe32Architecture : PowerPcArchitecture
    {
        public PowerPcLe32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Little, PrimitiveType.Word32, PrimitiveType.Int32, options)
        {

        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addrs, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }

    public class PowerPcBe64Architecture : PowerPcArchitecture
    {
        public PowerPcBe64Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Big, PrimitiveType.Word64, PrimitiveType.Int64, options)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            SegmentMap map,
            EndianImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToLinear())
                .ToHashSet();
            return new PowerPcPointerScanner64(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr64(u));
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt64();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr64(uAddr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }
    }

    public class PowerPcLe64Architecture : PowerPcArchitecture
    {
        public PowerPcLe64Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, EndianServices.Little, PrimitiveType.Word64, PrimitiveType.Int64, options)
        { }

        public override IEnumerable<Address> CreatePointerScanner(
            SegmentMap map,
            EndianImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses
                .Select(a => a.ToLinear())
                .ToHashSet();
            return new PowerPcPointerScanner64(rdr, knownLinAddresses, flags)
                .Select(u => Address.Ptr64(u));
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt64();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr64(uAddr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }
    }
}
