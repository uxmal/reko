#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.Motorola
{
    using FlagM = Reko.Arch.Motorola.M6809.FlagM;

    public class M6809Architecture : ProcessorArchitecture
    {
        public M6809Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null!, null!)
        {
            this.Endianness = EndianServices.Big;
            this.FramePointerType = PrimitiveType.Ptr16;
            this.InstructionBitSize = 8;
            this.PointerType = PrimitiveType.Ptr16;
            this.StackRegister = M6809.Registers.S;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new M6809.M6809Disassembler(this, rdr);
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
            return new M6809.M6809State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new M6809.M6809Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var fl = new FlagGroupStorage(M6809.Registers.CC, grf, GrfToString(M6809.Registers.CC, "", grf));
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

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var sb = new StringBuilder();
            if ((grf & (uint) FlagM.N) != 0) sb.Append('N');
            if ((grf & (uint) FlagM.Z) != 0) sb.Append('Z');
            if ((grf & (uint) FlagM.V) != 0) sb.Append('V');
            if ((grf & (uint) FlagM.C) != 0) sb.Append('C');
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

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            throw new NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }
    }
}
