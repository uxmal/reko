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
using System.Text;

namespace Reko.Arch.Vax
{
    public class VaxArchitecture : ProcessorArchitecture
    {
        public VaxArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.ByName, Registers.ByDomain)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new VaxDisassembler(this, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new VaxInstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new VaxProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new VaxRewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(Registers.psw, "", grf));
            return fl;
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            return Enum.GetValues(typeof(Mnemonic))
                .Cast<Mnemonic>()
                .ToSortedList(v => Enum.GetName(typeof(Mnemonic), v)!, v => (int)v);
        }

        public override int? GetMnemonicNumber(string name)
        {
            if (!Enum.TryParse(name, out Mnemonic result))
                return null;
            return (int)result;
        }

        public RegisterStorage? GetRegister(int i)
        {
            if (Registers.ByDomain.TryGetValue(StorageDomain.Register + i, out var reg))
                return reg;
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.ByDomain.Values.ToArray();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            foreach (var flag in flagRegisters)
            {
                if ((flags.FlagGroupBits & flag.FlagGroupBits) != 0)
                    yield return flag;
            }
        }

        //$REVIEW: shouldn't this be flaggroup?
        private static readonly FlagGroupStorage[] flagRegisters = {
            new FlagGroupStorage(Registers.psw, (uint)FlagM.CF, "C"),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.VF, "V"),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.ZF, "Z"),
            new FlagGroupStorage(Registers.psw, (uint)FlagM.NF, "N"),
        };

        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            for (int r = 0; grf != 0; ++r, grf >>= 1)
            {
                if ((grf & 1) != 0)
                    s.Append(flagRegisters[r].Name);
            }
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
