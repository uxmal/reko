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

namespace Reko.Arch.Arm.AArch64
{
    using Decoder = Reko.Core.Machine.Decoder<AArch64Disassembler, Mnemonic, AArch64Instruction>;

    public partial class AArch64Disassembler
    {
        public class SparseMaskDecoder : Decoder
        {
            private readonly string tag;
            private readonly int shift;
            private readonly uint mask;
            private readonly Dictionary<uint, Decoder> decoders;
            private readonly Decoder @default;

            public SparseMaskDecoder(string tag, int shift, uint mask, Dictionary<uint, Decoder> decoders, Decoder @default)
            {
                this.tag = tag;
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
                this.@default = @default;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                DumpMaskedInstruction(wInstr, mask << shift, tag);
                var op = (wInstr >> shift) & mask;
                if (!decoders.TryGetValue(op, out Decoder decoder))
                    decoder = @default;
                return decoder.Decode(wInstr, dasm);
            }
        }

        private class InstrDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly InstrClass iclass;
            private readonly VectorData vectorData;
            private readonly Mutator<AArch64Disassembler>[] mutators;

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, VectorData vectorData, params Mutator<AArch64Disassembler>[] mutators)
            {
                this.mnemonic = mnemonic;
                this.iclass = iclass;
                this.vectorData = vectorData;
                this.mutators = mutators;
            }

            public override AArch64Instruction Decode(uint wInstr, AArch64Disassembler dasm)
            {
                dasm.state.mnemonic = this.mnemonic;
                dasm.state.iclass = this.iclass;
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
