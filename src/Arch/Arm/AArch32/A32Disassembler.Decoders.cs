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

namespace Reko.Arch.Arm.AArch32
{
    public partial class A32Disassembler
    {
        public struct Bitfield
        {
            public readonly int Position;
            public readonly int Length;
            public readonly uint Mask;

            public Bitfield(int position, int length)
            {
                this.Position = position;
                this.Length = length;
                this.Mask = (1U << length) - 1U;
            }
        }

        public abstract class Decoder
        {
            public abstract AArch32Instruction Decode(uint wInstr, A32Disassembler dasm);

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
                Debug.Assert(decoders.Length == mask + 1, $"Inconsistent number of decoders {decoders.Length} (shift {shift} mask {mask:X})");
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = (wInstr >> shift) & mask;
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                return;
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

        public class BitfieldDecoder : Decoder
        {
            private Bitfield [] bitfields;
            private Decoder[] decoders;

            public BitfieldDecoder(Bitfield[] bitfields, Decoder[] decoders)
            {
                this.bitfields = bitfields;
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                uint n = 0;
                foreach (var bitfield in bitfields)
                {
                    n = n << bitfield.Length | ((wInstr >> bitfield.Position) & bitfield.Mask);
                }
                return this.decoders[n].Decode(wInstr, dasm);
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

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                return dasm.NotYetImplemented(message, wInstr);
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

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                if (format == null)
                    return dasm.NotYetImplemented($"Format missing for ARM instruction {opcode}.", wInstr);
                return dasm.Decode(wInstr, opcode, format);
            }
        }

        private class CondMaskDecoder : MaskDecoder
        {
            public CondMaskDecoder(int shift, uint mask, params Decoder[] decoders)
                : base(shift, mask, decoders)
            { }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var instr = base.Decode(wInstr, dasm);
                instr.condition = (ArmCondition)(wInstr >> 28);
                return instr;
            }
        }

        // Special decoder for when a 4-bit field has the bit pattern 1111 or not.
        private class PcDecoder : Decoder
        {
            private readonly int shift;
            private readonly Decoder not1111;
            private readonly Decoder is1111;

            public PcDecoder(int shift, Decoder not1111, Decoder is1111)
            {
                this.shift = shift;
                this.not1111 = not1111;
                this.is1111 = is1111;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> shift) & 0xF;
                if (op == 0xF)
                    return is1111.Decode(wInstr, dasm);
                else
                    return not1111.Decode(wInstr, dasm);
            }
        }

        class CustomDecoder : Decoder
        {
            private readonly Func<uint, A32Disassembler, Decoder> decode;

            public CustomDecoder(Func<uint, A32Disassembler, Decoder> decode)
            {
                this.decode = decode;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                return decode(wInstr, dasm).Decode(wInstr, dasm);
            }
        }

        private class MovDecoder : InstrDecoder
        {
            public MovDecoder(Opcode opcode, string format) : base(opcode, format) { }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var instr = base.Decode(wInstr, dasm);
                if (instr.ShiftType != Opcode.Invalid)
                {
                    instr.opcode = instr.ShiftType;
                }
                return instr;
            }
        }

        private class SelectDecoder:Decoder
        {
            private readonly int pos;
            private readonly uint mask;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectDecoder(int pos, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.pos = pos;
                this.mask = mask;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                var op = (wInstr >> pos) & mask;
                var decoder = predicate(op) ? trueDecoder : falseDecoder;
                return decoder.Decode(wInstr, dasm);
            }
        }
    }
}
