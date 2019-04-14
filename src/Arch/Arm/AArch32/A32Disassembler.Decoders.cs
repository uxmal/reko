#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
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
        public abstract class Decoder
        {
            public abstract AArch32Instruction Decode(uint wInstr, A32Disassembler dasm);

            public static uint bitmask(uint u, int shift, uint mask)
            {
                return (u >> shift) & mask;
            }

            protected void DumpMaskedInstruction(uint wInstr, uint shMask, string tag)
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
                if (!string.IsNullOrEmpty(tag))
                {
                    sb.AppendFormat(" {0}", tag);
                }
                Debug.Print(sb.ToString());
            }
        }

        public class MaskDecoder : Decoder
        {
            private readonly string tag;
            private readonly int shift;
            private readonly uint mask;
            private readonly Decoder[] decoders;

            public MaskDecoder(string tag, int shift, uint mask, params Decoder[] decoders)
            {
                this.tag = tag;
                this.shift = shift;
                this.mask = mask;
                Debug.Assert(decoders.Length == mask + 1, $"Inconsistent number of decoders {decoders.Length} (shift {shift} mask {mask:X})");
                this.decoders = decoders;
            }

            public MaskDecoder(int shift, uint mask, params Decoder[] decoders): this("", shift, mask, decoders)
            {
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
                var shMask = this.mask << shift;
                DumpMaskedInstruction(wInstr, shMask, tag);
            }
        }

        public class BitfieldDecoder : Decoder
        {
            private readonly string tag;
            private readonly Bitfield [] bitfields;
            private readonly Decoder[] decoders;

            public BitfieldDecoder(string tag, Bitfield[] bitfields, Decoder[] decoders)
            {
                this.tag = tag;
                this.bitfields = bitfields;
                this.decoders = decoders;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceDecoder(wInstr, tag);
                uint n = Bitfield.ReadFields(bitfields, wInstr);
                return this.decoders[n].Decode(wInstr, dasm);
            }


            [Conditional("DEBUG")]
            public void TraceDecoder(uint wInstr, string tag = "")
            {
                var shMask = bitfields.Aggregate(0u, (mask, bf) => mask | bf.Mask << bf.Position);
                DumpMaskedInstruction(wInstr, shMask, tag);
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
                Debug.Print("NYI: {0}", message);
                return dasm.NotYetImplemented(message, wInstr);
            }

            public override string ToString()
            {
                return $"nyi({message})";
            }
        }

        private class InstrDecoder : Decoder
        {
            private readonly Opcode opcode;
            private readonly InstrClass iclass;
            private readonly ArmVectorData vectorData;
            private readonly Mutator<A32Disassembler>[] mutators;

            public InstrDecoder(Opcode opcode, InstrClass iclass, ArmVectorData vectorData, params Mutator<A32Disassembler>[] mutators)
            {
                this.opcode = opcode;
                this.iclass = iclass;
                this.vectorData = vectorData;
                this.mutators = mutators;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                dasm.state.iclass = iclass;
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
            private readonly string tag;
            private readonly int shift;
            private readonly Decoder not1111;
            private readonly Decoder is1111;

            public PcDecoder(string tag, int shift, Decoder not1111, Decoder is1111)
            {
                this.tag = tag;
                this.shift = shift;
                this.not1111 = not1111;
                this.is1111 = is1111;
            }

            public PcDecoder(int shift, Decoder not1111, Decoder is1111) : this("", shift, not1111, is1111)
            {
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceMask(wInstr, tag);
                var op = (wInstr >> shift) & 0xF;
                if (op == 0xF)
                    return is1111.Decode(wInstr, dasm);
                else
                    return not1111.Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            public void TraceMask(uint wInstr, string tag)
            {
                var shMask = 0xFu << shift;
                DumpMaskedInstruction(wInstr, shMask, tag);
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

        private class SelectDecoder:Decoder
        {
            private readonly string tag;
            private readonly int pos;
            private readonly uint mask;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public SelectDecoder(string tag, int pos, uint mask, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.tag = tag;
                this.pos = pos;
                this.mask = mask;
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                TraceMask(wInstr);
                var op = (wInstr >> pos) & mask;
                var decoder = predicate(op) ? trueDecoder : falseDecoder;
                return decoder.Decode(wInstr, dasm);
            }

            [Conditional("DEBUG")]
            private void TraceMask(uint wInstr)
            {
                var uMask = mask << pos;
                DumpMaskedInstruction(wInstr, uMask, tag);
            }
        }
    }
}
