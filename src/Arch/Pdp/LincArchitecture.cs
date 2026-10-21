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

using Reko.Arch.Pdp.Linc;
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Pdp;

public class LincArchitecture : ProcessorArchitecture
{
    public LincArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, Registers.RegisterBank)
    {
        Endianness = EndianServices.Big;
        CarryFlag = Registers.L;
    }

    public static PrimitiveType Word12 => PdpTypes.Word12;

    /// <summary>
    /// Converts a string of octal word values to an array of 16-bit unsigned integers.
    /// </summary>
    /// <param name="octalwords">String that contains the octal word values to convert.</param>
    /// <returns>An array of 16-bit unsigned integers parsed from the specified octal string.</returns>
    public static ushort[] OctalStringToWords(string octalwords)
    {
        List<ushort> words = [];
        int cDigits = 0;
        uint word = 0;
        for (int i = 0; i < octalwords.Length; ++i)
        {
            uint digit = (uint) (octalwords[i] - '0');
            if (digit > 7)
                continue;
            word = (word << 3) | digit;
            if (++cDigits == 4)
            {
                words.Add((ushort) word);
                cDigits = 0;
                word = 0;
            }
        }
        return words.ToArray();
    }

    public static Address Ptr12(uint uAddr)
    {
        return Address.OctalPtr(PdpTypes.Ptr12, uAddr);
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new LincDisassembler(imageReader, this);
    }

    public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
    {
        return new LincInstructionComparer(norm);
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
        throw new NotImplementedException();
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, ulong grf)
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

    public override string GrfToString(RegisterStorage flagRegister, string prefix, ulong grf)
    {
        throw new NotImplementedException();
    }

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
    {
        return Ptr12(c.ToUInt16() & 0xFFFu);
    }

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
    {
        throw new NotImplementedException();
    }

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
    {
        if (txtAddr is null)
        {
            addr = default;
            return false;
        }
        uint uAddr = 0;
        foreach (var c in txtAddr)
        {
            var u = (uint) (c - '0');
            if (u >= 8)
            {
                addr = default;
                return false;
            }
            uAddr = (uAddr << 3) | u;
        }
        addr = Ptr12(uAddr & 0xFFF);
        return true;
    }
}
