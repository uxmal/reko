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
using System.Text;

namespace Reko.Arch.Maxim;

public class MaxqArchitecture : ProcessorArchitecture
{
    public MaxqArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, Registers.ByName, Registers.ByDomain) 
    {
        this.CarryFlag = Registers.C;
        this.Endianness = EndianServices.Little;
        this.InstructionBitSize = 16;
        this.MemoryGranularity = 16;
        this.CodeMemoryGranularity = 16;
        this.FramePointerType = PrimitiveType.Ptr16;
        this.PointerType = PrimitiveType.Ptr16;
        this.WordWidth = PrimitiveType.Word16;
        this.StackRegister = Registers.SP;
    }

    public override MemoryArea CreateCodeMemoryArea(Address addr, byte[] bytes)
    {
        return Word16MemoryArea.CreateFromLeBytes(addr, bytes);
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new MaxqDisassembler(this, imageReader);
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
        return new MaxqState(this);
    }

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        return new MaxqRewriter(this, rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
    {
        var flagregister = Registers.PSF;
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
        throw new System.NotImplementedException();
    }

    public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
    {
        uint grf = flags.FlagGroupBits;
        if (flags.FlagRegister == Registers.PSF)
        {
            if ((grf & Registers.C.FlagGroupBits) != 0) yield return Registers.C;
            if ((grf & Registers.S.FlagGroupBits) != 0) yield return Registers.S;
            if ((grf & Registers.Z.FlagGroupBits) != 0) yield return Registers.Z;
            if ((grf & Registers.E.FlagGroupBits) != 0) yield return Registers.E;
            if ((grf & Registers.V.FlagGroupBits) != 0) yield return Registers.V;
        }
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        var sb = new StringBuilder();
        if (flagRegister == Registers.PSF)
        {
            if ((grf & (uint) FlagM.CF) != 0) sb.Append('c');
            if ((grf & (uint) FlagM.SF) != 0) sb.Append('s');
            if ((grf & (uint) FlagM.ZF) != 0) sb.Append('z');
            if ((grf & (uint) FlagM.EF) != 0) sb.Append('e');
            if ((grf & (uint) FlagM.OV) != 0) sb.Append('v');
        }
        return sb.ToString();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        return Address.Ptr16(c.ToUInt16());
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        return Address.TryParse16(txtAddr, out addr);
    }
}
