#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Linq;
using System.Text;

using Microchip.Crownking;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// PIC18 processor architecture.
    /// </summary>
    public class PIC18Architecture : ProcessorArchitecture
    {
        private List<FlagGroupStorage> flagGroups;

        public PIC18Architecture(PIC picDescr)
        {
            this.PICDescriptor = picDescr;
            this.flagGroups = new List<FlagGroupStorage>();
            base.Name = picDescr.Name;
            base.Description = picDescr.Desc;
            base.FramePointerType = PrimitiveType.Offset16;
            base.InstructionBitSize = 16;
            base.PointerType = PrimitiveType.Pointer32;
            base.WordWidth = PrimitiveType.UInt16;
        }

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor { get; }

        public PIC18Disassembler CreateDisassemblerImpl(EndianImageReader imageReader)
        {
            return new PIC18Disassembler(this, imageReader);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new LeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new LeImageReader(image, offset);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new LeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new LeImageWriter(mem, addr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new PIC18InstructionComparer(norm);
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString().ToUpper(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            Opcode result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            foreach (FlagGroupStorage f in flagGroups)
            {
                if (f.FlagGroupBits == grf)
                    return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.status, grf, GrfToString(grf), dt);
            flagGroups.Add(fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            FlagM grf = 0;
            for (int i = 0; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case 'C': grf |= FlagM.C; break;
                    case 'Z': grf |= FlagM.Z; break;
                    case 'D': grf |= FlagM.DC; break;
                    case 'O': grf |= FlagM.OV; break;
                    case 'N': grf |= FlagM.N; break;
                    case 'P': grf |= FlagM.PD; break;
                    case 'T': grf |= FlagM.TO; break;
                    default: return null;
                }
            }
            return GetFlagGroup((uint)grf);
        }

        public override RegisterStorage GetRegister(int i)
        {
            return Registers.GetRegister(i);
        }

        public override RegisterStorage GetRegister(string name)
        {
            var r = Registers.GetRegister(name);
            if (r == RegisterStorage.None)
                throw new ArgumentException($"'{name}' is not a register name.");
            return r;
        }

        public override IEnumerable<RegisterStorage> GetAliases(RegisterStorage reg)
        {
            yield break;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.RegsByAddr.Values.ToArray();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return CreateDisassemblerImpl(imageReader);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new PIC18State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
        {
            return new PIC18Rewriter(this, rdr, (PIC18State)state, frame, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            //TODO: CreatePointerScanner - understand purpose, implement
            throw new NotImplementedException($"{nameof(CreatePointerScanner)} not implemented.");
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int offset, DataType dataType)
        {
            throw new NotImplementedException("PIC18 has no explicit stack");
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
        {
            //TODO: ReadCodeAddress - understand purpose, implement
            throw new NotImplementedException($"{nameof(ReadCodeAddress)} not implemented.");
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            reg = Registers.GetRegister(name);
            return (reg != RegisterStorage.None);
        }

        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            for (int r = Registers.status.Number; grf != 0; ++r, grf >>= 1)
            {
                if ((grf & 1) != 0)
                    s.Append(Registers.GetRegister(r).Name);
            }
            return s.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }

    }

}
