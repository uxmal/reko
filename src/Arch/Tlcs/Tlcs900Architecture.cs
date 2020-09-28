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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Core.Lib;
using Registers = Reko.Arch.Tlcs.Tlcs900.Tlcs900Registers;

namespace Reko.Arch.Tlcs
{
    /// <summary>
    /// Architecture definition for the 32-bit Toshiba TLCS-900
    /// processor.
    /// </summary>
    // https://toshiba.semicon-storage.com/product/micro/900H1_CPU_BOOK_CP3_CPU_en.pdf
    public class Tlcs900Architecture : ProcessorArchitecture
    {
        public Tlcs900Architecture(string archId) : base(archId)
        {
            this.CarryFlagMask = Registers.C.FlagGroupBits;
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 8;        // Instruction alignment, really.
            this.FramePointerType = PrimitiveType.Ptr32;
            this.PointerType = PrimitiveType.Ptr32;
            this.WordWidth = PrimitiveType.Word32;
            this.StackRegister = Registers.xsp;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Tlcs900.Tlcs900Disassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new Tlcs900.Tlcs900InstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new Tlcs900ProcessorState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Tlcs900.Tlcs900Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            uint grf = 0;
            foreach (var c in name)
            {
                switch (c)
                {
                case 'S': grf |= Registers.S.FlagGroupBits; break;
                case 'Z': grf |= Registers.Z.FlagGroupBits; break;
                case 'H': grf |= Registers.H.FlagGroupBits; break;
                case 'V': grf |= Registers.V.FlagGroupBits; break;
                case 'N': grf |= Registers.N.FlagGroupBits; break;
                case 'C': grf |= Registers.C.FlagGroupBits; break;
                }
            }
            return GetFlagGroup(Registers.f, grf);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            foreach (FlagGroupStorage f in Registers.flagBits)
            {
                if (f.FlagGroupBits == grf)
                    return f;
            }

            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.f, grf, GrfToString(flagRegister, "", grf), dt);
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

        public override RegisterStorage GetRegister(StorageDomain regDomain, BitRange range)
        {
            if (!Registers.Subregisters.TryGetValue(regDomain, out var subs))
                return null;
            int key = (range.Extent << 4) | range.Lsb;
            if (!subs.TryGetValue(key, out var subreg))
                return null;
            return subreg;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.regs.ToArray();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.H.FlagGroupBits) != 0) yield return Registers.H;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
        }

        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (!Registers.Subregisters.TryGetValue(reg.Domain, out var subs))
                return null;
            int key = (width << 4) | offset;
            if (!subs.TryGetValue(key, out var subreg))
                return null;
            return subreg;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder s = new StringBuilder();
            foreach (var freg in Registers.flagBits)
            {
                if ((freg.FlagGroupBits & grf) != 0)
                    s.Append(freg.Name);
            }
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
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
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
