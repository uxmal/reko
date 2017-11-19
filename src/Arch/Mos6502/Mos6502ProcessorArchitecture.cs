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
using Reko.Core.Rtl;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Serialization;

namespace Reko.Arch.Mos6502
{
    public class Mos6502ProcessorArchitecture : ProcessorArchitecture
    {
        public Mos6502ProcessorArchitecture()
        {
            CarryFlagMask = (uint)FlagM.CF;
            InstructionBitSize = 8;
            FramePointerType = PrimitiveType.Byte;       // Yup, stack pointer is a byte register (!)
            PointerType = PrimitiveType.Ptr16;
            StackRegister = Registers.s;
            WordWidth = PrimitiveType.Byte;       // 8-bit, baby!
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new Disassembler((LeImageReader)imageReader);
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
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Mos6502ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Rewriter(this, rdr.Clone(), state, binder, host);
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
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.All;
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override Expression CreateStackAccess(IStorageBinder frame, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState state)
        {
            throw new NotImplementedException();
        }


        public override string GrfToString(uint grf)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string txtAddress, out Address addr)
        {
            return Address.TryParse16(txtAddress, out addr);
        }
    }

    public  static class Registers
    {
        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0);
        public static readonly RegisterStorage x = RegisterStorage.Reg8("x", 1);
        public static readonly RegisterStorage y = RegisterStorage.Reg8("y", 2);
        public static readonly RegisterStorage s = RegisterStorage.Reg8("s", 3);

        public static readonly FlagRegister p = new FlagRegister("p", 10, PrimitiveType.Byte);

        public static readonly RegisterStorage N = new RegisterStorage("N", 4, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage V = new RegisterStorage("V", 5, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage C = new RegisterStorage("C", 6, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage Z = new RegisterStorage("Z", 7, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage I = new RegisterStorage("I", 8, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage D = new RegisterStorage("D", 9, 0, PrimitiveType.Byte);
        
        internal static RegisterStorage[] All;

        public static RegisterStorage GetRegister(int reg)
        {
            return All[reg];
        }

        static Registers()
        {
            All = new RegisterStorage[]
            {
                a,
                x,
                y,
                s,

                N,
                V,
                C,
                Z,
                I,
                D,
            };
        }
    }
}
