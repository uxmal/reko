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

namespace Reko.Arch.Epson;

public class C33Architecture : ProcessorArchitecture
{
    public C33Architecture(IServiceProvider services, string archId, Dictionary<string, object> options) 
        : base(services, archId, options, Registers.ByName, Registers.ByDomain)
    {
        this.CodeMemoryGranularity = 8;
        this.Endianness = EndianServices.Little;
        this.FramePointerType = PrimitiveType.Ptr32;
        this.InstructionBitSize = 16;
        this.MemoryGranularity = 8;
        this.PointerType = PrimitiveType.Ptr32;
        this.WordWidth = PrimitiveType.Word32;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
    {
        return new C33Disassembler(this, rdr);
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
        return new DefaultProcessorState(this);
    }

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        return new C33Rewriter(this, rdr, state, binder, host);
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
        throw new System.NotImplementedException();
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        throw new System.NotImplementedException();
    }
}
