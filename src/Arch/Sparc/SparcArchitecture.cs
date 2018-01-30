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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public class SparcArchitecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public SparcArchitecture(PrimitiveType wordWidth)
        {
            this.WordWidth = wordWidth;
            this.PointerType = PrimitiveType.Create(Domain.Pointer, wordWidth.Size);
            this.StackRegister = Registers.sp;
            this.FramePointerType = PointerType;
            this.InstructionBitSize = 32;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        #region IProcessorArchitecture Members

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new SparcDisassembler(this, imageReader);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
        {
            return new BeImageReader(image, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
        {
            return new BeImageReader(image, addrBegin, addrEnd);
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
        {
            return new BeImageReader(image, offset);
        }

        public override ImageWriter CreateImageWriter()
        {
            return new BeImageWriter();
        }

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
        {
            return new BeImageWriter(mem, addr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new SparcInstructionComparer(norm);
        }

        public override ProcessorState CreateProcessorState()
        {
            return new SparcProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new SparcRewriter(this, rdr, (SparcProcessorState)state, binder, host);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            Opcode result;
            if (!Enum.TryParse(name, true, out result))
                return null;
            return (int)result;
        }

        public override RegisterStorage GetRegister(int i)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(string name)
        {
            return Registers.GetRegister(name);
        }

        public override RegisterStorage[] GetRegisters()
        {
            return
                Registers.IntegerRegisters
                .Concat(Registers.FloatRegisters).ToArray();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0)
                return reg;
            else
                return null;
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            FlagGroupStorage fl;
            if (flagGroups.TryGetValue(grf, out fl))
                return fl;

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            fl = new FlagGroupStorage(Registers.psr, grf, GrfToString(grf), dt);
            flagGroups.Add(grf, fl);
            return fl;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            FlagM grf = 0;
            for (int i = 0; i < name.Length; ++i)
            {
                switch (name[i])
                {
                case 'N': grf |= FlagM.NF; break;
                case 'C': grf |= FlagM.CF; break;
                case 'Z': grf |= FlagM.ZF; break;
                case 'V': grf |= FlagM.VF; break;

                case 'E': grf |= FlagM.EF; break;
                case 'L': grf |= FlagM.LF; break;
                case 'G': grf |= FlagM.GF; break;
                case 'U': grf |= FlagM.UF; break;
                default: return null;
                }
            }
            return GetFlagGroup((uint)grf);
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }


        public override string GrfToString(uint grf)
        {
            StringBuilder s = new StringBuilder();
            if ((grf & Registers.N.FlagGroupBits) != 0) s.Append(Registers.N.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) s.Append(Registers.Z.Name);
            if ((grf & Registers.V.FlagGroupBits) != 0) s.Append(Registers.V.Name);
            if ((grf & Registers.C.FlagGroupBits) != 0) s.Append(Registers.C.Name);

            if ((grf & Registers.E.FlagGroupBits) != 0) s.Append(Registers.E.Name);
            if ((grf & Registers.L.FlagGroupBits) != 0) s.Append(Registers.L.Name);
            if ((grf & Registers.G.FlagGroupBits) != 0) s.Append(Registers.G.Name);
            if ((grf & Registers.U.FlagGroupBits) != 0) s.Append(Registers.U.Name);

            return s.ToString();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
        #endregion
    }

    public class SparcArchitecture32 : SparcArchitecture
    {
        public SparcArchitecture32() : base(PrimitiveType.Word32)
        {
        }
    }

    public class SparcArchitecture64 : SparcArchitecture
    {
        public SparcArchitecture64() : base(PrimitiveType.Word64)
        {
        }
    }
}