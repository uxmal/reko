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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.OpenRISC
{
    public class OpenRISCArchitecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups = new Dictionary<uint, FlagGroupStorage>();

        public OpenRISCArchitecture(string archId) : base(archId)
        {
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Word32;
            this.InstructionBitSize = 32;
            this.PointerType = PrimitiveType.Ptr32;
            this.SignedWordWidth = PrimitiveType.Int32;
            this.StackRegister = Registers.GpRegs[1];
            this.WordWidth = PrimitiveType.Word32;
        }

        public PrimitiveType SignedWordWidth { get; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new OpenRISCDisassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new OpenRISCState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new OpenRISCRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (this.flagGroups.TryGetValue(grf, out var stg))
                return stg;

            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var flagregister = Registers.sr;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
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

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.RegistersByDomain[domain];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegs;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            var grf = (FlagM) flags.FlagGroupBits;
            if ((grf & FlagM.CY) != 0) yield return Registers.C;
            if ((grf & FlagM.OV) != 0) yield return Registers.V;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            if ((grf & (uint) FlagM.CY) != 0) s.Append('C');
            if ((grf & (uint) FlagM.OV) != 0) s.Append('V');
            return s.ToString();
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            this.Endianness = options.TryGetValue("Endianness", out var oEnd) && 
                (string) oEnd == "le"
                ? EndianServices.Little
                : EndianServices.Big;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return new Dictionary<string, object>
            {
                {
                    "Endianness",
                    this.Endianness == EndianServices.Little
                        ? "le"
                        : "be"
                }
            };
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.RegisterByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
