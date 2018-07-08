#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class AArch64Disassembler
    {
        public abstract class Decoder
        {
            public abstract AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm);

            public static uint bitmask(uint u, int shift, uint mask)
            {
                return (u >> shift) & mask;
            }

            public static bool bit(uint u, int shift)
            {
                return ((u >> shift) & 1) != 0;
            }
        }

        public class MaskDecoder : Decoder
        {
            private readonly int shift;
            private readonly uint mask;
            private readonly Decoder[] decoders;

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders)
            {
                this.shift = shift;
                this.mask = mask;
                Debug.Assert(decoders.Length == mask + 1);
                this.decoders = decoders;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = (wInstr >> shift) & mask;
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                var shMask = this.mask << shift;
                var hibit = 0x80000000u;
                var sb = new StringBuilder();
                for (int i = 0; i < 32; ++i)
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
                Debug.Print(sb.ToString());
            }
        }

        public class SparseMaskDecoder : Decoder
        {
            private readonly int shift;
            private readonly uint mask;
            private readonly Dictionary<uint, Decoder> decoders;
            private readonly Decoder @default;

            public SparseMaskDecoder(int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
            {
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
                this.@default = @default;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }
        }

        public class SelectDecoder : Decoder
        {
            private string bitfields;
            private Predicate<int> predicate;
            private Decoder trueDecoder;
            private Decoder falseDecoder;

            public SelectDecoder(string bitfields, Predicate<int> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.bitfields = bitfields;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                int i = 0;
                int n = dasm.ReadSignedBitfield(wInstr, bitfields, ref i);
                var decoder = predicate(n) ? trueDecoder : falseDecoder;
                return decoder.Decode(wInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                return dasm.NotYetImplemented(message, wInstr);
            }
        }

        class CustomDecoder : Decoder
        {
            private Func<uint, AArch64Disassembler, Decoder> decode;

            public CustomDecoder(Func<uint, AArch64Disassembler, Decoder> decode)
            {
                this.decode = decode;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                return decode(wInstr, dasm).Decode(wInstr, dasm);
            }
        }

        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly string format;

            public InstrDecoder(Opcode opcode, string format)
            {
                this.opcode = opcode;
                this.format = format;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                return dasm.Decode(wInstr, opcode, format);
            }
        }
    }
}
