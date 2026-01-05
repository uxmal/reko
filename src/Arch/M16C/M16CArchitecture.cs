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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.M16C;

public class M16CArchitecture : ProcessorArchitecture
{
    public readonly static PrimitiveType Ptr20 = PrimitiveType.Create(Domain.Pointer, 20);
    public readonly static PrimitiveType Word20 = PrimitiveType.CreateWord(20);
    private readonly static ConcurrentDictionary<uint, FlagGroupStorage> flagGroups = new();

    public M16CArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, Registers.ByName, [])
    {
        Endianness = EndianServices.Little;
        this.CarryFlag = Registers.C;
        this.FramePointerType = PrimitiveType.Ptr16;
        this.InstructionBitSize = 8;
        this.PointerType = PrimitiveType.Ptr16;
        this.StackRegister = Registers.usp;
        this.WordWidth = PrimitiveType.Word16;
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
    {
        return new M16CDisassembler(this, rdr);
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
        return new M16CRewriter(this, rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
    {
        FlagGroupStorage? f;
        while (!flagGroups.TryGetValue(grf, out f))
        {
            var flagregister = Registers.flg;
            f = new FlagGroupStorage(flagregister, grf, GrfToString(flagRegister, "", grf));
            if (flagGroups.TryAdd(grf, f))
                return f;
        }
        return f;
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

    public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
    {
        var n = (int) domain;
        bool isHi = 8 <= range.Lsb && range.Msb <= 16;
        bool isLo = 0 <= range.Lsb && range.Msb <= 8;
        switch (n)
        {
        case 0:
            if (isLo)
                return Registers.r0l;
            if (isHi)
                return Registers.r0h;
            return Registers.r0;
        case 1:
            if (isLo)
                return Registers.r1l;
            if (isHi)
                return Registers.r1h;
            return Registers.r1;
        }
        if (Registers.ByDomain.TryGetValue(domain, out var reg))
            return reg;
        return null;
    }

    public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
    {
        uint grf = flags.FlagGroupBits;
        if ((grf & (uint) FlagM.UF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.UF)!;
        if ((grf & (uint) FlagM.IF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.IF)!;
        if ((grf & (uint) FlagM.OF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.OF)!;
        if ((grf & (uint) FlagM.BF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.BF)!;
        if ((grf & (uint) FlagM.SF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.SF)!;
        if ((grf & (uint) FlagM.ZF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.ZF)!;
        if ((grf & (uint) FlagM.DF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.DF)!;
        if ((grf & (uint) FlagM.CF) != 0) yield return GetFlagGroup(flags.FlagRegister, (uint) FlagM.CF)!;
    }

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
    {
        var sb = new StringBuilder();
        if ((grf & (uint) FlagM.UF) != 0) sb.Append('U');
        if ((grf & (uint) FlagM.IF) != 0) sb.Append('I');
        if ((grf & (uint) FlagM.OF) != 0) sb.Append('O');
        if ((grf & (uint) FlagM.BF) != 0) sb.Append('B');
        if ((grf & (uint) FlagM.SF) != 0) sb.Append('S');
        if ((grf & (uint) FlagM.ZF) != 0) sb.Append('Z');
        if ((grf & (uint) FlagM.DF) != 0) sb.Append('D');
        if ((grf & (uint) FlagM.CF) != 0) sb.Append('C');
        return sb.ToString();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        if (c.DataType.BitSize <= 16)
            return Address.Ptr16(c.ToUInt16());
        else
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
