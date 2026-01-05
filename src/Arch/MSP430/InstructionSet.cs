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

using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Msp430
{
    using Decoder = Decoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;
    using MaskDecoder = MaskDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;


    public partial class Msp430Disassembler
    {
        internal class InstructionSet
        {
            private bool useExtensions;
            private Decoder invalid;

            public InstructionSet(bool useExtensions)
            {
                 this.useExtensions = useExtensions;
                 this.invalid = Instr(Mnemonics.invalid, InstrClass.Invalid);
            }

            public Decoder[] CreateRootDecoders()
            {
                Decoder nyi = new NyiDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>("");

                Decoder[] extDecoders = new Decoder[16]
                {
                nyi,
                Sparse(6, 6, "  01", nyi,
                    ( 0x00, nyi ),
                    ( 0x01, nyi ),
                    ( 0x02, nyi ),
                    ( 0x04, InstrX(Mnemonics.rrax, W,s)),
                    ( 0x05, InstrX(Mnemonics.rrax, W,s)),
                    ( 0x06, nyi ),
                    ( 0x08, nyi ),
                    ( 0x09, nyi ),
                    ( 0x0A, nyi ),
                    ( 0x0C, Sparse(0, 6,  "  0C", invalid,
                        ( 0x00, InstrX(Mnemonics.reti, InstrClass.Transfer|InstrClass.Return))))),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
                };

                ExtDecoder extDecoder = new ExtDecoder(extDecoders);


                Decoder rotations = Mask(8, 2, "  rotations",
                   InstrX(Mnemonics.rrcm, a, N, r2),
                   InstrX(Mnemonics.rram, a, N, r2),
                   InstrX(Mnemonics.rlam, a, N, r2),
                   InstrX(Mnemonics.rrum, a, N, r2));

                (Mnemonics, InstrClass)[] jmps = new (Mnemonics, InstrClass)[8]
                {
                ( Mnemonics.jnz,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jz,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jnc,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jc,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jn,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jge,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jl,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jmp,  InstrClass.Transfer ),
                };

                return new Decoder[16]
                {
                Mask(4, 4, "  op0",
                    InstrX(Mnemonics.mova, p,At,r2),
                    InstrX(Mnemonics.mova, p,Post,r2),
                    InstrX(Mnemonics.mova, p,Amp,r2),
                    InstrX(Mnemonics.mova, p,Indirect(8),r2),

                    rotations,
                    rotations,
                    InstrX(Mnemonics.mova, p,r,Amp),
                    InstrX(Mnemonics.mova, p,r,Indirect(0)),

                    InstrX(Mnemonics.mova, p,Y,r2),
                    InstrX(Mnemonics.cmpa, p,Y,r2),
                    InstrX(Mnemonics.adda, p,Y,r2),
                    InstrX(Mnemonics.suba, p,Y,r2),

                    InstrX(Mnemonics.mova, p,r,r2),
                    InstrX(Mnemonics.cmpa, p,r,r2),
                    InstrX(Mnemonics.adda, p,r,r2),
                    InstrX(Mnemonics.suba, p,r,r2)
                ),
                Mask(6, 6, "  op1",
                    Instr(Mnemonics.rrc, w, d),
                    Instr(Mnemonics.rrc, w, d),
                    Instr(Mnemonics.swpb, w16, d),
                    invalid,

                    Instr(Mnemonics.rra, w,d),
                    Instr(Mnemonics.rra, w,d),
                    Instr(Mnemonics.sxt, w,d),
                    invalid,

                    Instr(Mnemonics.push, w,s),
                    Instr(Mnemonics.push, w,s),
                    Instr(Mnemonics.call, InstrClass.Transfer|InstrClass.Call, s),
                    invalid,

                    Sparse(0, 6, "  0C", invalid,
                        ( 0x00, Instr(Mnemonics.reti, InstrClass.Transfer) )),
                    nyi, // InstrX(Mnemonics.calla, S),
                    invalid,
                    invalid,

                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),

                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),
                    InstrX(Mnemonics.pushm, x,n,r2),

                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),

                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),
                    InstrX(Mnemonics.popm, x,n,r2),

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder
                ),
                new JmpDecoder(jmps),
                new JmpDecoder(jmps),

                Select(u => (u & 0x0F3F) == 0x0130, // mask: As & S
                    Instr(Mnemonics.ret, InstrClass.Transfer|InstrClass.Return),
                    Select(u => (u & 0x8F) == 0, // mask: Ad & D
                        Instr(Mnemonics.br, InstrClass.Transfer, Saddr),
                        Instr(Mnemonics.mov, w,S,D))),
                Instr(Mnemonics.add, w,S,D),
                Instr(Mnemonics.addc, w,S,D),
                Instr(Mnemonics.subc, w,S,D),

                Instr(Mnemonics.sub, w,S,D),
                Instr(Mnemonics.cmp, w,S,D),
                Instr(Mnemonics.dadd, w,S,D),
                Instr(Mnemonics.bit, w,S,D),

                Select((0, 16), u => u == 0xC232, "  Cxxx",
                    Instr(Mnemonics.dint),
                    Instr(Mnemonics.bic, w,S,D)),
                Select((0, 16), u => u == 0xD232, "  Dxxx",
                    Instr(Mnemonics.eint),
                    Instr(Mnemonics.bis, w,S,D)),
                Instr(Mnemonics.xor, w,S,D),
                Instr(Mnemonics.and, w,S,D),
                };
            }

            private static Decoder Instr(Mnemonics mnemonic, params Mutator<Msp430Disassembler>[] mutators)
            {
                return new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(InstrClass.Linear, mnemonic, mutators);
            }

            private static Decoder Instr(Mnemonics mnemonic, InstrClass iclass, params Mutator<Msp430Disassembler>[] mutators)
            {
                return new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(iclass, mnemonic, mutators);
            }

            private Decoder InstrX(Mnemonics mnemonic, params Mutator<Msp430Disassembler>[] mutators)
            {
                return useExtensions
                    ? new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(InstrClass.Linear, mnemonic, mutators)
                    : invalid;
            }

            private Decoder InstrX(Mnemonics mnemonic, InstrClass iclass, params Mutator<Msp430Disassembler>[] mutators)
            {
                return useExtensions
                    ? new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(iclass, mnemonic, mutators)
                    : invalid;
            }
        }
    }
}
