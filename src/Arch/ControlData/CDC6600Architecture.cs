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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Reko.Arch.ControlData.CDC6600;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;

namespace Reko.Arch.ControlData;

public class CDC6600Architecture : ProcessorArchitecture
{
    public CDC6600Architecture(
        IServiceProvider services,
        string archId,
        Dictionary<string, object> options)
        : base(services, archId, options, Registers.RegisterBank)
    {
        this.CodeMemoryGranularity = 60;
        this.DefaultBase = 8;
        this.Endianness = EndianServices.Big;
        this.FramePointerType = Ptr18;
        this.InstructionBitSize = 60;
        this.MemoryGranularity = 60;
        this.PointerType = Ptr18;
        this.WordWidth = Word60;
    }

    public static PrimitiveType Ptr18 { get; } = PrimitiveType.CreateWord(18);
    public static PrimitiveType Word18 { get; } = PrimitiveType.CreateWord(18);
    public static PrimitiveType Word60 { get; } = PrimitiveType.CreateWord(60);

    public static Address Ptr(uint uAddr)
    {
        return Address.OctalPtr(Ptr18, uAddr);
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
    {
        return new CDC6600Disassembler(this, imageReader);
    }

    public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
    {
        return new CDC6600InstructionComparer(norm);
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
        return new CDC6600Rewriter(this, rdr, state, binder, host);
    }

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, ulong grf)
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

    public override string GrfToString(RegisterStorage flagRegister, string prefix, ulong grf)
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

    /*
// - One's Complement Negation (CDC 6600): Unlike IEEE 754 (which uses sign-magnitude and only flips the sign bit),
//   the CDC 6600 negates a floating-point number by taking the bitwise one's complement of the entire 60-bit word.
// - Integer Mantissa Scaling (CDC 6600): The CDC 6600 treats the 48-bit mantissa as a binary integer (1 followed by
//   47 zeros equals 2^47) rather than a fraction. Because of this, 1.0 = 2^47 * 2^-47, giving an unbiased exponent
//   of -47 (biased exponent 1023 - 47 = 976, or 1720 in octal).
// - Special Values (CDC 6600): +inf is represented by 3777 0000... (octal), -inf is its bitwise complement
//   (4000 7777...), and the hardware equivalent of NaN ("Indefinite") is 1777 0000... (with a negative complement 6000 7777...).
// - Special Values (IEEE 754): Infinity has an all-ones exponent and an all-zeros fraction. NaN has an all-ones
//   exponent and any non-zero fraction (a canonical quiet NaN is shown below).
// - Edge Cases Included: 1/3 and -1/3 (repeating binary fractions rounded to 48 vs. 53 bits of precision),
//   0.1 (non-terminating decimal fraction), -NaN (negative Indefinite / negative quiet NaN), max finite positive,
//   min positive normal, and min positive subnormal / unnormalized ULP.

string[][] testData = new string[][]
{
    new string[] { "0.0",           "0_00000000000_000000000000000000000000000000000000000000000000", "0_00000000000_0000000000000000000000000000000000000000000000000000" },
    new string[] { "-0.0",          "1_11111111111_111111111111111111111111111111111111111111111111", "1_00000000000_0000000000000000000000000000000000000000000000000000" },
    new string[] { "1.0",           "0_01111010000_100000000000000000000000000000000000000000000000", "0_01111111111_0000000000000000000000000000000000000000000000000000" },
    new string[] { "-1.0",          "1_10000101111_011111111111111111111111111111111111111111111111", "1_01111111111_0000000000000000000000000000000000000000000000000000" },
    new string[] { "0.5",           "0_01111001111_100000000000000000000000000000000000000000000000", "0_01111111110_0000000000000000000000000000000000000000000000000000" },
    new string[] { "-0.5",          "1_10000110000_011111111111111111111111111111111111111111111111", "1_01111111110_0000000000000000000000000000000000000000000000000000" },
    new string[] { "2.0",           "0_01111010001_100000000000000000000000000000000000000000000000", "0_10000000000_0000000000000000000000000000000000000000000000000000" },
    new string[] { "-2.0",          "1_10000101110_011111111111111111111111111111111111111111111111", "1_10000000000_0000000000000000000000000000000000000000000000000000" },
    new string[] { "1/3",           "0_01111001110_101010101010101010101010101010101010101010101011", "0_01111111101_0101010101010101010101010101010101010101010101010101" },
    new string[] { "-1/3",          "1_10000110001_010101010101010101010101010101010101010101010100", "1_01111111101_0101010101010101010101010101010101010101010101010101" },
    new string[] { "0.1",           "0_01111001100_110011001100110011001100110011001100110011001101", "0_01111111011_1001100110011001100110011001100110011001100110011010" },
    new string[] { "+inf",          "0_11111111111_000000000000000000000000000000000000000000000000", "0_11111111111_0000000000000000000000000000000000000000000000000000" },
    new string[] { "-inf",          "1_00000000000_111111111111111111111111111111111111111111111111", "1_11111111111_0000000000000000000000000000000000000000000000000000" },
    new string[] { "NaN",           "0_01111111111_000000000000000000000000000000000000000000000000", "0_11111111111_1000000000000000000000000000000000000000000000000000" },
    new string[] { "-NaN",          "1_10000000000_111111111111111111111111111111111111111111111111", "1_11111111111_1000000000000000000000000000000000000000000000000000" },
    new string[] { "MaxFinite",     "0_11111111110_111111111111111111111111111111111111111111111111", "0_11111111110_1111111111111111111111111111111111111111111111111111" },
    new string[] { "MinNormal",     "0_00000000001_100000000000000000000000000000000000000000000000", "0_00000000001_0000000000000000000000000000000000000000000000000000" },
    new string[] { "MinSubnormal",  "0_00000000000_000000000000000000000000000000000000000000000001", "0_00000000000_0000000000000000000000000000000000000000000000000001" }
};
     * 
     */
}