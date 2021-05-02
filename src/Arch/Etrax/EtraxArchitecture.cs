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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Etrax
{
    public class EtraxArchitecture : ProcessorArchitecture
    {
        public EtraxArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options) : base(services, archId, options)
        {
            this.CarryFlagMask = 0;//$TODO
            this.Endianness = EndianServices.Little;
            this.FramePointerType = PrimitiveType.Word32;
            this.InstructionBitSize = 16;
            this.MemoryGranularity = 8;
            this.PointerType = PrimitiveType.Ptr32;
            this.StackRegister = Registers.sp;
            this.WordWidth = PrimitiveType.Word32;
        }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new EtraxDisassembler(this, rdr);
        }

        public override IProcessorEmulator CreateEmulator(SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var name = GrfToString(flagRegister, "", grf);
            return new FlagGroupStorage(flagRegister, grf, name, PrimitiveType.Word16);
        }

        public override FlagGroupStorage? GetFlagGroup(string name)
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
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var mask = (FlagM) grf;
            var sb = new StringBuilder();
            if (mask.HasFlag(FlagM.FF)) sb.Append('F');
            if (mask.HasFlag(FlagM.PF)) sb.Append('P');
            if (mask.HasFlag(FlagM.UF)) sb.Append('U');
            if (mask.HasFlag(FlagM.MF)) sb.Append('M');
            if (mask.HasFlag(FlagM.BF)) sb.Append('B');
            if (mask.HasFlag(FlagM.IF)) sb.Append('I');
            if (mask.HasFlag(FlagM.XF)) sb.Append('X');
            if (mask.HasFlag(FlagM.NF)) sb.Append('N');
            if (mask.HasFlag(FlagM.ZF)) sb.Append('Z');
            if (mask.HasFlag(FlagM.VF)) sb.Append('V');
            if (mask.HasFlag(FlagM.CF)) sb.Append('C');
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
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
            return Address.TryParse32(txtAddr, out addr);
        }
    }
}
