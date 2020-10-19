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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750Architecture : ProcessorArchitecture
    {
        private Dictionary<uint, FlagGroupStorage> flagGroups;

        public static PrimitiveType Real48 { get; } 

        public MilStd1750Architecture(IServiceProvider services, string archId) : base(services, archId)
        {
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 16;
            this.MemoryGranularity = 16;        // Memory is organized as 16-bit words, not 8-bit bytes
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = Registers.GpRegs[15];
            this.WordWidth = PrimitiveType.Word16;
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new MilStd1750Disassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override MemoryArea CreateMemoryArea(Address addr, byte[] bytes)
        {
            // NOTE: assumes the bytes are provided in big-endian form.
            var words = new ushort[bytes.Length / 2];
            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = (ushort)((bytes[i * 2] << 8) | bytes[i * 2 + 1]); 
            }
            return new Word16MemoryArea(addr, words);
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
            return new MilStd1750State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new MilStd1750Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagGroups.TryGetValue(grf, out FlagGroupStorage f))
                return f;

            PrimitiveType dt = PrimitiveType.Byte;
            var fl = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf), dt);
            flagGroups.Add(grf, fl);
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

        public override RegisterStorage? GetRegister(string name)
        {
            return Registers.ByName.TryGetValue(name, out var reg) ? reg : null;
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.ByDomain.TryGetValue(domain, out var reg) ? reg : null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.CF) != 0) yield return Registers.C;
            if ((grf & (uint) FlagM.PF) != 0) yield return Registers.P;
            if ((grf & (uint) FlagM.ZF) != 0) yield return Registers.Z;
            if ((grf & (uint) FlagM.NF) != 0) yield return Registers.N;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder sb = new StringBuilder();
            if ((grf & (uint) FlagM.CF) != 0) sb.Append('C');
            if ((grf & (uint) FlagM.PF) != 0) sb.Append('P');
            if ((grf & (uint) FlagM.ZF) != 0) sb.Append('Z');
            if ((grf & (uint) FlagM.NF) != 0) sb.Append('N');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        static MilStd1750Architecture()
        {
            Real48 = PrimitiveType.Create(Domain.Real, 48);
        }
    }
}
