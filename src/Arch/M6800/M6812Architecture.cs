#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.Arch.M6800
{
    public class M6812Architecture : ProcessorArchitecture
    {
        private readonly Dictionary<uint, FlagGroupStorage> flagGroups;

        public M6812Architecture(string archId) : base(archId)
        {
            InstructionBitSize = 8;
            FramePointerType = PrimitiveType.Ptr16;
            PointerType = PrimitiveType.Ptr16;
            WordWidth = PrimitiveType.Word16;
            StackRegister = M6812.Registers.sp;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new M6812.M6812Disassembler(imageReader);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new BeImageReader(img, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(img, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            return new BeImageReader(img, off);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea img, Address addr)
        {
            return new BeImageWriter(img, addr);
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
            return new M6812.M6812State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new M6812.M6812Rewriter(this, rdr, (M6812.M6812State)state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            if (flagGroups.TryGetValue(grf, out var f))
            {
                return f;
            }

            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var flagregister = M6812.Registers.ccr;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
        {
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

        public override string GrfToString(uint grf)
        {
            var sb = new StringBuilder();
            if ((grf & (uint)FlagM.NF) != 0) sb.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) sb.Append('Z');
            if ((grf & (uint)FlagM.VF) != 0) sb.Append('V');
            if ((grf & (uint)FlagM.CF) != 0) sb.Append('C');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddr, out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        CF = 1,             // carry
        VF = 2,             // overflow
        ZF = 4,             // zero
        NF = 8,             // sign
    }
}
