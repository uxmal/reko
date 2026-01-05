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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.CompactRisc
{
    public class Cr16Architecture : ProcessorArchitecture
    {
        public static PrimitiveType Word24 = PrimitiveType.CreateWord(24);
        public static PrimitiveType Ptr24 = PrimitiveType.Create(Domain.Pointer, 24);

        public Cr16Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, Registers.ByName, Registers.ByDomain)
        {
            this.CarryFlag = Registers.C;
            this.Endianness = EndianServices.Little;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 16;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.GpRegisters[15];
            this.WordWidth = PrimitiveType.Word16;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new Cr16cDisassembler(this, rdr);
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
            return new Cr16State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new Cr16Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var flagregister = Registers.PSR;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf));
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


        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GpRegisters;
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & (uint) FlagM.CF) != 0) yield return Registers.C;
            if ((grf & (uint) FlagM.EF) != 0) yield return Registers.E;
            if ((grf & (uint) FlagM.FF) != 0) yield return Registers.F;
            if ((grf & (uint) FlagM.IF) != 0) yield return Registers.I;
            if ((grf & (uint) FlagM.LF) != 0) yield return Registers.L;
            if ((grf & (uint) FlagM.NF) != 0) yield return Registers.N;
            if ((grf & (uint) FlagM.PF) != 0) yield return Registers.P;
            if ((grf & (uint) FlagM.TF) != 0) yield return Registers.T;
            if ((grf & (uint) FlagM.UF) != 0) yield return Registers.U;
            if ((grf & (uint) FlagM.ZF) != 0) yield return Registers.Z;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var s = new StringBuilder();
            if ((grf & (uint) FlagM.CF) != 0) s.Append('C');
            if ((grf & (uint) FlagM.EF) != 0) s.Append('E');
            if ((grf & (uint) FlagM.FF) != 0) s.Append('F');
            if ((grf & (uint) FlagM.IF) != 0) s.Append('I');
            if ((grf & (uint) FlagM.LF) != 0) s.Append('L');
            if ((grf & (uint) FlagM.NF) != 0) s.Append('N');
            if ((grf & (uint) FlagM.PF) != 0) s.Append('P');
            if ((grf & (uint) FlagM.TF) != 0) s.Append('T');
            if ((grf & (uint) FlagM.UF) != 0) s.Append('U');
            if ((grf & (uint) FlagM.ZF) != 0) s.Append('Z');
            return s.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.ByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
