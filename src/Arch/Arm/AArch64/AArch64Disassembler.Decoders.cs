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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm.AArch64
{
    using Mutator = Func<uint, AArch64Disassembler, bool>;

    public partial class AArch64Disassembler
    {
        public abstract class Decoder
        {
            public abstract AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm);

            protected void DumpMaskedInstruction(uint wInstr, Bitfield[] bitfields)
            {
                var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
                DumpMaskedInstruction(wInstr, shMask);
            }

            protected void DumpMaskedInstruction(uint wInstr, uint shMask)
            {
                return;
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
                DumpMaskedInstruction(wInstr, this.mask << shift);
            }
        }

        public class BitfieldDecoder : Decoder
        {
            private readonly Bitfield[] bitfields;
            private readonly Decoder[] decoders;

            public BitfieldDecoder(Bitfield[] bitfields, params Decoder[] decoders)
            {
                Debug.Assert(1 << bitfields.Sum(b => b.Length) == decoders.Length, 
                    $"Expected {1 << bitfields.Sum(b => b.Length)} decoders but found {decoders.Length}.");
                this.bitfields = bitfields;
                this.decoders = decoders;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                TraceDecoder(wInstr);
                uint op = Bitfield.ReadFields(bitfields, wInstr);
                return decoders[op].Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                DumpMaskedInstruction(wInstr, bitfields);
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
                TraceDecoder(wInstr);
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                DumpMaskedInstruction(wInstr, this.mask << shift);
            }
        }

        public class SelectDecoder : Decoder
        {
            private readonly Bitfield[] bitfields;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectDecoder(Bitfield[] bitfields, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.bitfields = bitfields;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                base.DumpMaskedInstruction(wInstr, this.bitfields);
                uint n = Bitfield.ReadFields(bitfields, wInstr);
                var decoder = predicate(n) ? trueDecoder : falseDecoder;
                return decoder.Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr)
            {
                DumpMaskedInstruction(wInstr, bitfields);
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                return dasm.NotYetImplemented(message, wInstr);
            }
        }

        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly VectorData vectorData;
            private readonly Mutator[] mutators;

            public InstrDecoder(Opcode opcode, VectorData vectorData, params Mutator[] mutators)
            {
                this.opcode = opcode;
                this.vectorData = vectorData;
                this.mutators = mutators;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                dasm.state.opcode = this.opcode;
                dasm.state.vectorData = this.vectorData;
                for (int i = 0; i < mutators.Length; ++i)
                {
                    if (!mutators[i](wInstr, dasm))
                    {
                        dasm.state.Invalid();
                        break;
                    }
                }
                var instr = dasm.state.MakeInstruction();
                return instr;
            }
        }
    }
}
