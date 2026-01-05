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

using Reko.Arch.Motorola.M88k;
using Reko.Core;
using Reko.Core.Collections;
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
using System.Threading.Tasks;

namespace Reko.Arch.Motorola;

public class M88kArchitecture : ProcessorArchitecture
{
    public M88kArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, Registers.ByName, Registers.ByDomain)
    {
        this.CodeMemoryGranularity = 8;
        this.Endianness = EndianServices.Big;
        this.FramePointerType = PrimitiveType.Ptr32;
        this.InstructionBitSize = 32;
        this.MemoryGranularity = 8; 
        this.PointerType = PrimitiveType.Ptr32;
        this.StackRegister = Registers.GpRegisters[31];
        this.WordWidth = PrimitiveType.Word32;
    }

    /// <inheritdoc/>
    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new M88kDisassembler(this, imageReader);
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
        return new M88kRewriter(this, this.Services, rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
    {
        if (grf != (uint)FlagM.CF)
            return null;
        return Registers.C;
    }

    public override FlagGroupStorage? GetFlagGroup(string name)
    {
        if (name != "C")
            return null;
        return Registers.C;
    }

    public override SortedList<string, int> GetMnemonicNames()
    {
        return Enum.GetValues<Mnemonic>()
            .Where(m => m != Mnemonic.Invalid)
            .ToSortedList(m => m.ToString(), m => (int)m);
    }

    public override int? GetMnemonicNumber(string name)
    {
        if (!Enum.TryParse<Mnemonic>(name, true, out var m))
            return null;
        return (int) m;
    }

    public override RegisterStorage[] GetRegisters()
    {
        return Registers.GpRegisters;
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        throw new NotSupportedException();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        return Address.Ptr32(c.ToUInt32());
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        return Address.TryParse32(txtAddr, out addr);
    }
}
