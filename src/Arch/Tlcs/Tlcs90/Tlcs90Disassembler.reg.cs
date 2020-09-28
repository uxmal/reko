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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    using Decoder = Decoder<Tlcs90Disassembler, Mnemonic, Tlcs90Instruction>;

    partial class Tlcs90Disassembler
    {
        private static Decoder[] regEncodings = new Decoder[]
        {
            // 00
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
            invalid,
            invalid,

            // 10
            invalid,
            invalid,
            Instr(Mnemonic.mul, H,g),
            Instr(Mnemonic.div, H,g),

            Instr(Mnemonic.add, X,G),
            Instr(Mnemonic.add, Y,G),
            Instr(Mnemonic.add, S,G),
            invalid,

            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),

            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),
            Instr(Mnemonic.tset, i,g),

            // 20
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
            invalid,
            invalid,

            // 30
            Instr(Mnemonic.ld, r,g),
            Instr(Mnemonic.ld, r,g),
            Instr(Mnemonic.ld, r,g),
            Instr(Mnemonic.ld, r,g),

            Instr(Mnemonic.ld, r,g),
            Instr(Mnemonic.ld, r,g),
            Instr(Mnemonic.ld, r,g),
            invalid,

            Instr(Mnemonic.ld, B,G),
            Instr(Mnemonic.ld, D,G),
            Instr(Mnemonic.ld, H,G),
            invalid,

            Instr(Mnemonic.ld, X,G),
            Instr(Mnemonic.ld, Y,G),
            Instr(Mnemonic.ld, S,G),
            invalid,

            // 40
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
            invalid,
            invalid,

            // 50
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.ldi),
            Instr(Mnemonic.ldir),
            Instr(Mnemonic.ldd),
            Instr(Mnemonic.lddr),

            Instr(Mnemonic.cpi),
            Instr(Mnemonic.cpir),
            Instr(Mnemonic.cpd),
            Instr(Mnemonic.cpdr),

            // 60
            Instr(Mnemonic.add, a,g),
            Instr(Mnemonic.adc, a,g),
            Instr(Mnemonic.sub, a,g),
            Instr(Mnemonic.sbc, a,g),

            Instr(Mnemonic.and, a,g),
            Instr(Mnemonic.xor, a,g),
            Instr(Mnemonic.or,  a,g),
            Instr(Mnemonic.cp,  a,g),

            Instr(Mnemonic.add, g,Ib),
            Instr(Mnemonic.adc, g,Ib),
            Instr(Mnemonic.sub, g,Ib),
            Instr(Mnemonic.sbc, g,Ib),

            Instr(Mnemonic.and, g,Ib),
            Instr(Mnemonic.xor, g,Ib),
            Instr(Mnemonic.or,  g,Ib),
            Instr(Mnemonic.cp,  g,Ib),

            // 70
            Instr(Mnemonic.add, H,G),
            Instr(Mnemonic.adc, H,G),
            Instr(Mnemonic.sub, H,G),
            Instr(Mnemonic.sbc, H,G),

            Instr(Mnemonic.and, H,G),
            Instr(Mnemonic.xor, H,G),
            Instr(Mnemonic.or,  H,G),
            Instr(Mnemonic.cp,  H,G),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 80
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
            invalid,
            invalid,

            // 90
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
            invalid,
            invalid,

            // A0
            Instr(Mnemonic.rlc, g),
            Instr(Mnemonic.rrc, g),
            Instr(Mnemonic.rl, g),
            Instr(Mnemonic.rr, g),

            Instr(Mnemonic.sla, g),
            Instr(Mnemonic.sra, g),
            Instr(Mnemonic.sll, g),
            Instr(Mnemonic.srl, g),

            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),

            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),
            Instr(Mnemonic.bit, i,g),

            // B0
            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),

            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),
            Instr(Mnemonic.res, i,g),

            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),

            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),
            Instr(Mnemonic.set, i,g),

            // C0
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
            invalid,
            invalid,

            // D0
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),

            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),

            Instr(Mnemonic.ret, InstrClass.Transfer),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),

            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),
            Instr(Mnemonic.ret, InstrClass.ConditionalTransfer, c),

            // E0
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
            invalid,
            invalid,

            // F0
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
            invalid,
            invalid,
        };
    }
}
