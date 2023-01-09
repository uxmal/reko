#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.Padauk
{
    public class PadaukArchitecture : ProcessorArchitecture
    {
        public PadaukArchitecture(
            IServiceProvider services, 
            string archId,
            Dictionary<string, object> options)
            : base(services, archId, options, Registers.RegistersByName, Registers.RegistersByDomain)
        {
            this.Endianness = EndianServices.Little;
            this.CodeMemoryGranularity = 16;
            this.MemoryGranularity = 8;
            this.CarryFlagMask = (uint) FlagM.CF;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = Registers.sp;
            this.WordWidth = PrimitiveType.Byte;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new PadaukDisassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override MemoryArea CreateCodeMemoryArea(Address addr, byte[] bytes)
        {
            // NOTE: assumes the bytes are provided in little-endian form.
            return Word16MemoryArea.CreateFromLeBytes(addr, bytes);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new DefaultProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new PadaukRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var f = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf), dt);
            return f;
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.OV) != 0) yield return Registers.V;
            if ((grf & (uint) FlagM.AC) != 0) yield return Registers.AC;
            if ((grf & (uint) FlagM.ZF) != 0) yield return Registers.Z;
            if ((grf & (uint) FlagM.CF) != 0) yield return Registers.C;
        }

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            foreach (var fr in Registers.FlagBits)
            {
                if ((fr.FlagGroupBits & grf) != 0) s.Append(fr.Name);
            }
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        internal PadaukDisassembler.InstructionSet CreateInstructionSet()
        {
            //$REVIEW: what's a sensible default here?
            if (!this.Options.TryGetValue(ProcessorOption.InstructionSet, out var oIsa) ||
                oIsa is not string isa)

                return new PadaukDisassembler.Pdk13InstructionSet();
            switch (isa.ToLower())
            {
            case "13":
            case "pdk13":
                return new PadaukDisassembler.Pdk13InstructionSet();
            case "14":
            case "pdk14":
                return new PadaukDisassembler.Pdk14InstructionSet();
            case "15":
            case "pdk15":
                return new PadaukDisassembler.Pdk15InstructionSet();
            }
            throw new NotImplementedException($"Instruction set {isa} not implemented yet.");
        }

        public override Dictionary<string, object>? SaveUserOptions() => this.Options;
    }
}
