#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Core.Lib;

namespace Reko.Arch.Msp430
{
    public class Msp430Architecture : ProcessorArchitecture
    {
        public readonly static PrimitiveType Word20 = PrimitiveType.CreateWord(20);
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public Msp430Architecture(string archName) : base(archName)
        {
            this.InstructionBitSize = 16;
            this.StackRegister = Registers.sp;
            this.WordWidth = PrimitiveType.Word16;
            this.PointerType = PrimitiveType.Word16;
            this.FramePointerType = PrimitiveType.Word16;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Msp430Disassembler(this, imageReader);
        }

        public override EndianImageReader CreateImageReader(MemoryArea mem, ulong off)
        {
            return new LeImageReader(mem, off);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter()
        {
            throw new NotImplementedException();
        }

        public override ImageWriter CreateImageWriter(MemoryArea img, Address addr)
        {
            throw new NotImplementedException();
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
            return new Msp430Rewriter(this, rdr, state, binder, host);
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            FlagGroupStorage f;
            if (flagGroups.TryGetValue(grf, out f))
            {
                return f;
            }
            var dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.sr, grf, GrfToString(grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetOpcodeNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            RegisterStorage reg;
            return Registers.ByName.TryGetValue(name, out reg)
                ? reg
                : null;
        }

        public override RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegisters;
        }

        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & (uint)FlagM.VF) != 0) s.Append('V');
            if ((grf & (uint)FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint)FlagM.ZF) != 0) s.Append('Z');
            if ((grf & (uint)FlagM.CF) != 0) s.Append('C');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
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
            return mem.TryReadLe(addr, dt, out value);
        }
    }
}
