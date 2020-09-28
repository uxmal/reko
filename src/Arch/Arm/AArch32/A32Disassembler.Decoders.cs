#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
    using Decoder = Reko.Core.Machine.Decoder<A32Disassembler, Mnemonic, AArch32Instruction>;

    public partial class A32Disassembler
    {
        public static uint bitmask(uint u, int shift, uint mask)
        {
            return (u >> shift) & mask;
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
            private readonly Mnemonic mnemonic;
            private readonly InstrClass iclass;
            private readonly ArmVectorData vectorData;
            private readonly Mutator<A32Disassembler>[] mutators;

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, ArmVectorData vectorData, params Mutator<A32Disassembler>[] mutators)
            {
                this.mnemonic = mnemonic;
                this.iclass = iclass;
                this.vectorData = vectorData;
                this.mutators = mutators;
            }

            public override AArch32Instruction Decode(uint wInstr, A32Disassembler dasm)
            {
                DumpMaskedInstruction(wInstr, 0, this.mnemonic);
                dasm.state.iclass = iclass;
                dasm.state.mnemonic = this.mnemonic;
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

        private class CondMaskDecoder : MaskDecoder<A32Disassembler, Mnemonic, AArch32Instruction>
        {
            public CondMaskDecoder(int bitPos, int bitLength, string tag, params Decoder[] decoders)
                : base(bitPos, bitLength, tag, decoders)
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
