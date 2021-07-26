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

namespace Reko.Arch.H8
{
    public class H8Architecture : ProcessorArchitecture
    {
        private readonly Dictionary<uint, FlagGroupStorage> flagGroups;

        public H8Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options)
        {
            this.CarryFlagMask = (uint) FlagM.CF;
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = Registers.GpRegisters[7];
            this.WordWidth = PrimitiveType.Word16;
            this.Ptr24 = PrimitiveType.Create(Domain.Pointer, 24);
            this.flagGroups = new Dictionary<uint, FlagGroupStorage>();
        }

        public PrimitiveType Ptr24 { get; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new H8Disassembler(this, rdr);
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
            return new H8State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new H8Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            if (flagRegister != Registers.CcRegister)
                throw new ArgumentException($"'{flagRegister.Name}' is not a flag register on this architecture.");
            if (flagGroups.TryGetValue(grf, out var flags))
                return flags;
            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
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

        public override RegisterStorage GetRegister(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            RegisterStorage[] regs;
            if (range.Msb <= 8)
                regs = Registers.RlRegisters;
            else if (range.Msb <= 16)
            {
                if (8 <= range.Lsb)
                    regs = Registers.RhRegisters;
                else
                    regs = Registers.RRegisters;
            }
            else
            {
                regs = Registers.GpRegisters;
            }
            var i = domain - regs[0].Domain;
            if (0 <= i && i < regs.Length)
                return regs[i];
            else
                return null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            var ccr = Registers.CcRegister;
            if ((grf & (uint) FlagM.NF) != 0) yield return this.GetFlagGroup(ccr, (uint) FlagM.NF)!;
            if ((grf & (uint) FlagM.ZF) != 0) yield return this.GetFlagGroup(ccr, (uint) FlagM.ZF)!;
            if ((grf & (uint) FlagM.VF) != 0) yield return this.GetFlagGroup(ccr, (uint) FlagM.VF)!;
            if ((grf & (uint) FlagM.CF) != 0) yield return this.GetFlagGroup(ccr, (uint) FlagM.CF)!;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var sb = new StringBuilder();
            if ((grf & (uint) FlagM.NF) != 0) sb.Append('N');
            if ((grf & (uint) FlagM.ZF) != 0) sb.Append('Z');
            if ((grf & (uint) FlagM.VF) != 0) sb.Append('V');
            if ((grf & (uint) FlagM.CF) != 0) sb.Append('C');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
