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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Core.Lib;

namespace Reko.Arch.Avr
{
    public class Avr8Architecture : ProcessorArchitecture
    {
        private RegisterStorage[] regs;
        private Dictionary<uint, FlagGroupStorage> grfs;
        private List<Tuple<FlagM, char>> grfToString;

        public Avr8Architecture()
        {
            this.PointerType = PrimitiveType.Ptr16;
            this.WordWidth = PrimitiveType.Word16;
            this.FramePointerType = PrimitiveType.UInt8;
            this.InstructionBitSize = 16;
            this.x = new RegisterStorage("x", 33, 0, PrimitiveType.Word16);
            this.y = new RegisterStorage("y", 34, 0, PrimitiveType.Word16);
            this.z = new RegisterStorage("z", 35, 0, PrimitiveType.Word16);
            this.sreg = new FlagRegister("sreg", 36, PrimitiveType.Byte);
            this.code = new RegisterStorage("code", 100, 0, PrimitiveType.SegmentSelector);
            this.StackRegister = new RegisterStorage("SP", 0x3D, 0, PrimitiveType.Word16);
            this.ByteRegs = Enumerable.Range(0, 32)
                .Select(n => new RegisterStorage(
                    string.Format("r{0}", n),
                    n,
                    0,
                    PrimitiveType.Byte))
                .ToArray();
            this.regs =
                ByteRegs
                .Concat(new[] { this.x, this.y, this.z, this.sreg })
                .ToArray();
            this.grfs = new Dictionary<uint, FlagGroupStorage>();
            this.grfToString = new List<Tuple<FlagM, char>>
            {
                Tuple.Create(FlagM.IF, 'I'),
                Tuple.Create(FlagM.TF, 'T'),
                Tuple.Create(FlagM.HF, 'H'),
                Tuple.Create(FlagM.SF, 'S'),
                Tuple.Create(FlagM.VF, 'V'),
                Tuple.Create(FlagM.NF, 'N'),
                Tuple.Create(FlagM.ZF, 'Z'),
                Tuple.Create(FlagM.CF, 'C'),
            };
        }
        
        public FlagRegister sreg { get; private set; }
        public RegisterStorage x { get; private set; }
        public RegisterStorage y { get; private set; }
        public RegisterStorage z { get; private set; }
        public RegisterStorage code { get; private set; }

        public RegisterStorage[] ByteRegs { get; }

		public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Avr8Disassembler(this, rdr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, ulong off)
        {
            return new LeImageReader(img, off);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addr)
        {
            return new LeImageReader(img, addr);
        }

        public override EndianImageReader CreateImageReader(MemoryArea img, Address addrBegin, Address addrEnd)
        {
            return new LeImageReader(img, addrBegin, addrEnd);
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
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Avr8State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Avr8Rewriter(this, rdr, state, binder, host);
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int cbOffset, DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(uint grf)
        {
            FlagGroupStorage fl;
            if (!grfs.TryGetValue(grf, out fl))
            {
                PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
                fl = new FlagGroupStorage(this.sreg, grf, GrfToString(grf), dt);
                grfs.Add(grf, fl);
            }
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
            throw new NotImplementedException();
        }

        public override RegisterStorage GetRegister(int i)
        {
            return regs[i];
        }

        public override RegisterStorage[] GetRegisters()
        {
            return regs;
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (reg == this.z)
            {
                if (offset == 0)
                    return regs[30];
                else
                    return regs[31];
            }
            return reg;
        }

        public override string GrfToString(uint grf)
        {
            var s = new StringBuilder();
            foreach (var tpl in this.grfToString)
            {
                if ((grf & (uint)tpl.Item1) != 0)
                {
                    s.Append(tpl.Item2);
                }
            }
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr16((ushort)c.ToUInt32());
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
    }

    [Flags]
    public enum FlagM
    {
        CF = 1,
        ZF = 2,
        NF = 4,
        VF = 8,
        SF = 16,
        HF = 32,
        TF = 64,
        IF = 128
    }
}