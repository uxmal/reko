#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.H8
{
    public class H8Architecture : ProcessorArchitecture
    {
        private readonly ConcurrentDictionary<ulong, FlagGroupStorage> flagGroups;
        private readonly H8CallingConvention cc;

        public H8Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.All)
        {
            this.CarryFlag = Registers.C;
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = Registers.GpRegisters[7];
            this.WordWidth = PrimitiveType.Word16;      //$TODO: could vary by model.
            this.Ptr24 = PrimitiveType.Create(Domain.Pointer, 24);
            this.flagGroups = [];
            this.cc = new H8CallingConvention(WordWidth);
        }

        public PrimitiveType Ptr24 { get; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new H8Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new H8InstructionComparer(norm);
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

        public override ICallingConvention? GetCallingConvention(string? name)
        {
            return cc;
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, ulong grf)
        {
            if (flagRegister != Registers.CcRegister)
                throw new ArgumentException($"'{flagRegister.Name}' is not a flag register on this architecture.");
            FlagGroupStorage? flags;
            while (!flagGroups.TryGetValue(grf, out flags))
            {
                flags = new FlagGroupStorage(flagRegister, grf, GrfToString(flagRegister, "", grf));
                if (flagGroups.TryAdd(grf, flags))
                    break;
            }
            return flags;
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

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            ulong grf = flags.FlagGroupBits;
            var ccr = Registers.CcRegister;
            if ((grf & (ulong) FlagM.NF) != 0) yield return this.GetFlagGroup(ccr, (ulong) FlagM.NF)!;
            if ((grf & (ulong) FlagM.ZF) != 0) yield return this.GetFlagGroup(ccr, (ulong) FlagM.ZF)!;
            if ((grf & (ulong) FlagM.VF) != 0) yield return this.GetFlagGroup(ccr, (ulong) FlagM.VF)!;
            if ((grf & (ulong) FlagM.CF) != 0) yield return this.GetFlagGroup(ccr, (ulong) FlagM.CF)!;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, ulong grf)
        {
            var sb = new StringBuilder();
            if ((grf & (ulong) FlagM.NF) != 0) sb.Append('N');
            if ((grf & (ulong) FlagM.ZF) != 0) sb.Append('Z');
            if ((grf & (ulong) FlagM.VF) != 0) sb.Append('V');
            if ((grf & (ulong) FlagM.CF) != 0) sb.Append('C');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            //$REVIEW: depends on CPU model; could be 16-bit ptrs
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
