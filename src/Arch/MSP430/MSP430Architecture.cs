#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.Msp430
{
    using Decoder = Decoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;

    public class Msp430Architecture : ProcessorArchitecture
    {
        public readonly static PrimitiveType Ptr20 = PrimitiveType.Create(Domain.Pointer, 20);
        public readonly static PrimitiveType Word20 = PrimitiveType.CreateWord(20);

        private Decoder[] decoders = default!;

        public Msp430Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null!, null!)
        {
            this.InstructionBitSize = 16;
            this.WordWidth = PrimitiveType.Word16;
            this.PointerType = PrimitiveType.Ptr16;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.Endianness = EndianServices.Little;
            SetOptionDependentProperties(options);
            this.StackRegister = Registers.sp;
            this.CarryFlag = Registers.C;
        }

        public Registers Registers { get; private set; } = default!;
        public PrimitiveType RegisterType { get; private set; } = default!;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Msp430Disassembler(this, decoders, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Msp430InstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Msp430State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Msp430Rewriter(this, decoders, rdr, state, binder, host);
        }

        public override ICallingConvention? GetCallingConvention(string? name)
        {
            return new Msp430CallingConvention(this);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var fl = new FlagGroupStorage(Registers.sr, grf, GrfToString(flagRegister, "", grf));
            return fl;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(string name)
        {
            return Registers.ByName.TryGetValue(name, out var reg)
                ? reg
                : null;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            var reg = Registers.GpRegisters[(int) domain];
            if (reg is { } && reg.Covers(range))
                return reg;
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegisters;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.NF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.NF);
            if ((grf & (uint) FlagM.ZF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.ZF);
            if ((grf & (uint) FlagM.CF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.CF);
            if ((grf & (uint) FlagM.VF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.VF);
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (uint)FlagM.VF) != 0) s.Append('V');
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            return s.ToString();
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            base.LoadUserOptions(options);
            SetOptionDependentProperties(options);
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        private void SetOptionDependentProperties(Dictionary<string,object>? options)
        {
            bool useExtensions = false;
            if (options is { } &&
                options.TryGetValue(ProcessorOption.InstructionSet, out var oIsa) &&
                oIsa is string sIsa &&
                sIsa == "MSP430X")
            {
                useExtensions = true;
                this.RegisterType = Word20;
                this.PointerType = Ptr20;
            }
            else
            {
                this.RegisterType = PrimitiveType.Word16;
                this.PointerType = PrimitiveType.Ptr16;
            }
            var isa = new Msp430Disassembler.InstructionSet(useExtensions);
            decoders = isa.CreateRootDecoders();
            this.Registers = new Registers(this.RegisterType);
            this.StackRegister = Registers.sp;
            this.CarryFlag = Registers.C;
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }
    }
}
