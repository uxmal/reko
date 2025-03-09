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

using Reko.Core.Lib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Abstract base class for instruction decoders.
    /// </summary>
    /// <typeparam name="TDasm">Disassembler</typeparam>
    /// <typeparam name="TInstr">Instruction type</typeparam>
    public abstract class Decoder<TDasm, TMnemonic, TInstr> : Decoder
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        public abstract TInstr Decode(uint wInstr, TDasm dasm);

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(uint wInstr, uint shMask, TMnemonic mnemonic)
        {
            DumpMaskedInstruction(32, wInstr, shMask, mnemonic.ToString()!);
        }

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction64(uint wInstr, uint shMask, TMnemonic mnemonic)
        {
            DumpMaskedInstruction(64, wInstr, shMask, mnemonic.ToString()!);
        }

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction64(uint wInstr, uint shMask, string tag)
        {
            DumpMaskedInstruction(64, wInstr, shMask, tag);
        }

        public static Decoder<TDasm, TMnemonic, TInstr>[] BuildSparseDecoderArray(int bits, Decoder<TDasm, TMnemonic, TInstr> defaultDecoder, (uint, Decoder<TDasm, TMnemonic, TInstr>)[] sparseDecoders)
        {
            var decoders = new Decoder<TDasm, TMnemonic, TInstr>[1 << bits];
            foreach (var (code, decoder) in sparseDecoders)
            {
                Debug.Assert(0 <= code && code < decoders.Length);
                Debug.Assert(decoders[code] is null, $"Decoder {code:X} has already a value!");
                decoders[code] = decoder;
            }
            for (int i = 0; i < decoders.Length; ++i)
            {
                if (decoders[i] is null)
                    decoders[i] = defaultDecoder;
            }
            return decoders;
        }
    }

    /// <summary>
    /// Decoder for use with opcodes that are wider than 32 bits.
    /// </summary>
    public abstract class WideDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        public sealed override TInstr Decode(uint wInstr, TDasm dasm)
        {
            throw new InvalidOperationException("32-bit decoding is not allowed with wide decoders.");
        }

        public abstract TInstr Decode(ulong ulInstr, TDasm dasm);
    }

    public class Decoder
    {
        public static readonly TraceSwitch trace = new TraceSwitch(nameof(Decoder), "Trace the progress of machine code decoders")
        {
            Level = TraceLevel.Warning
        };

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(int instrBitSize, ulong wInstr, Bitfield[] bitfields, string tag)
        {
            var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
            DumpMaskedInstruction(instrBitSize, wInstr, shMask, tag);
        }

        [Conditional("DEBUG")]
        public static void DumpMaskedInstruction(int instrBitSize, ulong wInstr, ulong shMask, string tag)
        {
            if (!trace.TraceVerbose)
                return;
            var hibit = 1ul << (instrBitSize - 1);
            var sb = new StringBuilder("// ");
            for (int i = 0; i < instrBitSize; ++i)
            {
                if ((shMask & hibit) != 0)
                {
                    sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                }
                else
                {
                    sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                }
                shMask <<= 1;
                wInstr <<= 1;
            }
            if (!string.IsNullOrEmpty(tag))
            {
                sb.AppendFormat(" {0}", tag);
            }
            Debug.WriteLine(sb.ToString());
            Console.WriteLine(sb.ToString());
        }
    }
}
