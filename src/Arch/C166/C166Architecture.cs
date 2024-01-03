#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.C166
{
    public class C166Architecture : ProcessorArchitecture
    {
        public C166Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            Endianness = EndianServices.Little;
            FramePointerType = PrimitiveType.Ptr16;
            InstructionBitSize = 16;
            PointerType = PrimitiveType.Ptr16;
            StackRegister = Registers.GpRegs[0];
            WordWidth = PrimitiveType.Word16;

            /*
R13	This register may be used but its contents must be saved (on entry) and restored (before returning).
R14	This register may be used but its contents must be saved (on entry) and restored (before returning).
R15	This register may be used but its contents must be saved (on entry) and restored (before returning).
DPP1	This DPP register may not be modified by assembler subroutines. It contains the memory class page and may be used in an interrupt service routine that interrupts the assembler subroutine.
DPP2	This DPP register may not be modified by assembler subroutines. It contains the memory class page and may be used in an interrupt service routine that interrupts the assembler subroutine.
DPP3	If DPP3 is modified in the assembler subroutine, it must be reset to 3 (SYSTEM PAGE) before returning.
            */
        }
        
        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new C166Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState() => new C166ProcessorState(this);

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new C166Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            PrimitiveType dt = Bits.IsSingleBitSet(grf) ? PrimitiveType.Bool : PrimitiveType.Byte;
            var fl = new FlagGroupStorage(Registers.PSW, grf, GrfToString(flagRegister, "", grf), dt);
            return fl;
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

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            var nDomain = (int) domain;
            if (0 <= nDomain && nDomain < Registers.GpRegs.Length)
            {
                if (range.Lsb < 8 && range.Msb <= 8)
                    return Registers.LoByteRegs[nDomain / 2];
                if (range.Lsb >= 8 && range.Msb < 16)
                    return Registers.HiByteRegs[nDomain / 2];
                return Registers.GpRegs[nDomain];
            }
            return Registers.ByDomain[domain];
        }

        public override RegisterStorage[] GetRegisters()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.E.FlagGroupBits) != 0) yield return Registers.E;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
            if ((grf & Registers.N.FlagGroupBits) != 0) yield return Registers.N;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            StringBuilder sb = new StringBuilder();
            if ((grf & Registers.E.FlagGroupBits) != 0) sb.Append(Registers.E.Name);
            if ((grf & Registers.Z.FlagGroupBits) != 0) sb.Append(Registers.Z.Name);
            if ((grf & Registers.V.FlagGroupBits) != 0) sb.Append(Registers.V.Name);
            if ((grf & Registers.C.FlagGroupBits) != 0) sb.Append(Registers.C.Name);
            if ((grf & Registers.N.FlagGroupBits) != 0) sb.Append(Registers.N.Name);
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

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }
    }
}
