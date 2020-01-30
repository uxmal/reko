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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Alpha
{
    public class AlphaArchitecture : ProcessorArchitecture
    {
        public AlphaArchitecture(string archId) : base(archId)
        {
            this.Endianness = EndianServices.Little;
            this.WordWidth = PrimitiveType.Word64;
            this.PointerType = PrimitiveType.Ptr64;
            this.FramePointerType = PrimitiveType.Ptr64;
            this.InstructionBitSize = 32;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new AlphaDisassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return null;
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new AlphaProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new AlphaRewriter(this, rdr, binder, host);
        }

        // Alpha uses a link register
        public override int ReturnAddressOnStack => 0;


        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
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
            return Registers.AllRegisters.TryGetValue(name, out var reg)
                ? reg
                : null;
        }

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.ByDomain.TryGetValue(domain, out var reg)
                ? reg
                : null;
        }
        public override RegisterStorage[] GetRegisters()
        {
            return Registers.AllRegisters.Values.ToArray();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            // Alpha has no traditional condition codes.
            return "";
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            //$TODO: this should be in Platform since pointer sizes != word sizes.
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.AllRegisters.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            //$TODO: this should be in the platform not the architecture.
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
