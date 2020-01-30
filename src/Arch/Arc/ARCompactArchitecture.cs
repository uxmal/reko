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

namespace Reko.Arch.Arc
{
    public class ARCompactArchitecture : ProcessorArchitecture
    {
        private readonly Dictionary<uint, FlagGroupStorage> flagGroups;

        public ARCompactArchitecture(string archId) : base(archId)
        {
            base.Endianness = EndianServices.Little;
            base.FramePointerType = PrimitiveType.Ptr32;
            base.InstructionBitSize = 16;
            base.PointerType = PrimitiveType.Ptr32;
            base.StackRegister = Registers.Sp;
            base.WordWidth = PrimitiveType.Word32;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new ArcDisassembler(this, rdr);
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
            return new ARCompactState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new ARCompactRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var fl))
            {
                return fl;
            }

            var dt = Bits.IsSingleBitSet((uint)grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var flagregister = Registers.Status32;
            fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf), dt);
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

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.ByStorageDomain[domain];
        }

        public override RegisterStorage GetRegister(string name)
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
            if (flags.FlagRegister == Registers.Status32)
            {
                if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
                if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
                if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
                if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            }
            else if (flags.FlagRegister == Registers.AuxMacmode)
            {
                if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            }
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix,  uint grf)
        {
            StringBuilder s = new StringBuilder();
            if (flagRegister == Registers.Status32)
            {
                if ((grf & (uint) FlagM.ZF) != 0) s.Append('Z');
                if ((grf & (uint) FlagM.NF) != 0) s.Append('N');
                if ((grf & (uint) FlagM.CF) != 0) s.Append('C');
                if ((grf & (uint) FlagM.VF) != 0) s.Append('V');
            }
            else
            {
                if ((grf & (uint) AuxFlagM.Sat) != 0) s.Append('S');
            }
            return s.ToString();
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            Endianness = (options.TryGetValue("Endianness", out var oEndian)
                && oEndian is string sEndian
                && string.Compare(sEndian, "be") == 0)
                ? EndianServices.Big
                : EndianServices.Little;
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~1u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
