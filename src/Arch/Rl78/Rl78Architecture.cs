#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Text;

namespace Reko.Arch.Rl78
{
    public class Rl78Architecture : ProcessorArchitecture
    {
        public Rl78Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options)
        {
            InstructionBitSize = 8;
            FramePointerType = PrimitiveType.Ptr32;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Word32;
            CarryFlagMask = (uint) FlagM.CF;
            StackRegister = Registers.sp;
            Endianness = EndianServices.Little;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Rl78Disassembler(this, rdr);
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
            return new Rl78ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Rl78Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage reg, uint grf)
        {
            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.psw, grf, GrfToString(reg, "", grf), dt);
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

        public override RegisterStorage GetRegister(StorageDomain domain, BitRange range) {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage reg, string str, uint grf)
        {
            var s = new StringBuilder();
            var flags = (FlagM) grf;
            if ((flags & FlagM.CF) != 0) s.Append('C');
            if ((flags & FlagM.ZF) != 0) s.Append('Z');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            return Address.Ptr32(uAddr);
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
