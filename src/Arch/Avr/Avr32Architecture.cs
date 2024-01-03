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

using Reko.Arch.Avr.Avr32;
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

namespace Reko.Arch.Avr
{
    public class Avr32Architecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Avr32Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.ByName, Registers.ByDomain)
        {
            this.CarryFlag = Registers.C;
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;
            this.WordWidth = PrimitiveType.Word32;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Avr32Disassembler(this, rdr);
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
            return new Avr32.Avr32ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Avr32Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var grfStg))
                return grfStg;

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            grfStg = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grfStg.FlagGroupBits, grfStg);
            return grfStg;
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

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.VF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.VF);
            if ((grf & (uint) FlagM.NF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.NF);
            if ((grf & (uint) FlagM.ZF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.ZF);
            if ((grf & (uint) FlagM.CF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.CF);
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder(prefix);
            if ((grf & (uint) FlagM.VF) != 0) s.Append('V');
            if ((grf & (uint) FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint) FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint) FlagM.CF) != 0) s.Append('C');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }


        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
