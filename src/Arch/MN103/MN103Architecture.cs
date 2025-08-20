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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Arch.MN103
{
    public class MN103Architecture : ProcessorArchitecture
    {
        public MN103Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : this(services, archId, options, Registers.RegistersByName, Registers.RegistersByDomain)
        {
        }

        public MN103Architecture(IServiceProvider services, string archId, Dictionary<string, object> options, Dictionary<string, RegisterStorage>? regsByName, Dictionary<StorageDomain, RegisterStorage>? regsByDomain) 
            : base(services, archId, options, regsByName, regsByDomain)
        {   
            this.CarryFlag = Registers.C;
            this.CodeMemoryGranularity = 8;
            this.Endianness = EndianServices.Little;
            this.FramePointerType = PrimitiveType.Ptr32;
            this.InstructionBitSize = 8;
            this.MemoryGranularity = 8;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;
            this.WordWidth = PrimitiveType.Word32;
        }

        public override IEnumerable<MN103Instruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new MN103Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new System.NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new MN103State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new MN103Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var flagregister = Registers.psw;
            var fl = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf));
            return fl;
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
        {
            throw new System.NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new System.NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new System.NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.RegistersByDomain.Values.ToArray();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if (flags.FlagRegister == Registers.psw)
            {
                if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
                if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
                if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
                if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            }
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var sb = new StringBuilder();
            if (flagRegister == Registers.psw)
            {
                if ((grf & (uint) FlagM.ZF) != 0) sb.Append('Z');
                if ((grf & (uint) FlagM.NF) != 0) sb.Append('N');
                if ((grf & (uint) FlagM.CF) != 0) sb.Append('C');
                if ((grf & (uint) FlagM.VF) != 0) sb.Append('V');
            }
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr32(c.ToUInt32());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
