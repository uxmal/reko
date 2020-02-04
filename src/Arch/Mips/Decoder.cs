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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Reko.Arch.Mips.MipsDisassembler;

namespace Reko.Arch.Mips
{
    using Decoder = Decoder<MipsDisassembler, Mnemonic, MipsInstruction>;

    public partial class MipsDisassembler
    {
        /// <summary>
        /// This instruction encoding is only valid on 64-bit MIPS architecture.
        /// </summary>
        class A64Decoder : InstrDecoder<MipsDisassembler, Mnemonic, MipsInstruction>
        {
            private readonly Mnemonic mnemonic;
            private readonly Mutator<MipsDisassembler>[] mutators;

            public A64Decoder(Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators) : base(InstrClass.Linear, mnemonic, mutators)
            {
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
            {
                if (dasm.arch.PointerType.Size == 8)
                    return base.Decode(wInstr, dasm);
                else
                    return dasm.CreateInvalidInstruction();
            }
        }

        internal class BcNDecoder : Decoder
        {
            private readonly Decoder opFalse;
            private readonly Decoder opTrue;

            public BcNDecoder(Decoder f, Decoder t)
            {
                this.opFalse = f;
                this.opTrue = t;
            }

            public override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
            {
                var decoder = ((wInstr & (1u << 16)) != 0) ? opTrue : opFalse;
                return decoder.Decode(wInstr, dasm);
            }
        }
    }
}