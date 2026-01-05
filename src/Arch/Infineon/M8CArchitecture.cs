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

using Reko.Arch.Infineon.M8C;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Infineon;

public class M8CArchitecture : ProcessorArchitecture
{
    public M8CArchitecture(
        IServiceProvider services,
        string archId,
        Dictionary<string, object> options)
        : base(services, archId, options, Registers.ByName, Registers.ByDomain)
    {
        Endianness = EndianServices.Big;
        CodeMemoryGranularity = 8;
        FramePointerType = PrimitiveType.Ptr16;
        InstructionBitSize = 8;
        MemoryGranularity = 8;
        PointerType = PrimitiveType.Ptr16;
        WordWidth = PrimitiveType.Ptr16;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new M8CDisassembler(this, imageReader);
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
        throw new NotImplementedException();
    }
}
