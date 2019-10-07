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
    using Decoder = Reko.Core.Machine.Decoder<MipsDisassembler, Opcode, MipsInstruction>;

    public partial class MipsDisassembler
    {
        public class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator<MipsDisassembler>[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator<MipsDisassembler>[] mutators)
            {
                this.iclass = InstrClass.Linear;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public InstrDecoder(InstrClass iclass, Opcode opcode, params Mutator<MipsDisassembler>[] mutators)
            {
                this.iclass = iclass;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                    {
                        return new MipsInstruction
                        {
                            InstructionClass = InstrClass.Invalid,
                            opcode = Opcode.illegal
                        };
                    }
                }
                return new MipsInstruction
                {
                    opcode = opcode,
                    InstructionClass = iclass,
                    Address = dasm.addr,
                    Length = 4,
                    op1 = dasm.ops.Count > 0 ? dasm.ops[0] : null,
                    op2 = dasm.ops.Count > 1 ? dasm.ops[1] : null,
                    op3 = dasm.ops.Count > 2 ? dasm.ops[2] : null,
                };
            }
        }

        /// <summary>
        /// This instruction encoding is only valid on 64-bit MIPS architecture.
        /// </summary>
        class A64Decoder : InstrDecoder
        {
            private readonly Opcode opcode;
            private readonly Mutator<MipsDisassembler>[] mutators;

            public A64Decoder(Opcode opcode, params Mutator<MipsDisassembler>[] mutators) : base(opcode, mutators)
            {
                this.opcode = opcode;
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

        internal class Version6Decoder : Decoder
        {
            private readonly Decoder preV6Odecoder;
            private readonly Decoder v6Odecoder;

            public Version6Decoder(Decoder preDecoder, Decoder postDecoder)
            {
                this.preV6Odecoder = preDecoder;
                this.v6Odecoder = postDecoder;
            }

            public override MipsInstruction Decode(uint wInstr, MipsDisassembler dasm)
            {
                if (dasm.isVersion6OrLater)
                    return v6Odecoder.Decode(wInstr, dasm);
                else
                    return preV6Odecoder.Decode(wInstr, dasm);
            }
        }
    }
}