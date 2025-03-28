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
        throw new System.NotImplementedException();
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

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        throw new System.NotImplementedException();
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
