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

using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp1;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Pdp;

public class Pdp1Architecture : ProcessorArchitecture
{
    public Pdp1Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, null, null)
    {
        this.CodeMemoryGranularity = 18;
        this.Endianness = EndianServices.Big;
        this.MemoryGranularity = 18;
        this.PointerType = PdpTypes.Ptr18;
        this.WordWidth = PdpTypes.Word18;
        this.FramePointerType = PdpTypes.Ptr18;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new Pdp1Disassembler(this, (Word18BeImageReader) imageReader);
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
        return new DefaultProcessorState(this);
    }

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        return new Pdp1Rewriter(this, (Word18BeImageReader) rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
    {
        throw new NotImplementedException();
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

    public override RegisterStorage[] GetRegisters()
    {
        throw new NotImplementedException();
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        throw new NotImplementedException();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        throw new NotImplementedException();
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        return PdpTypes.TryParseAddress(txtAddr, out addr); 
    }
}
