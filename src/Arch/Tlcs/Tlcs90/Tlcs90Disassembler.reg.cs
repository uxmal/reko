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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
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
            Instr(Opcode.mul, H,g),
            Instr(Opcode.div, H,g),

            Instr(Opcode.add, X,G),
            Instr(Opcode.add, Y,G),
            Instr(Opcode.add, S,G),
            invalid,

            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),

            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),
            Instr(Opcode.tset, i,g),

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
            Instr(Opcode.ld, r,g),
            Instr(Opcode.ld, r,g),
            Instr(Opcode.ld, r,g),
            Instr(Opcode.ld, r,g),

            Instr(Opcode.ld, r,g),
            Instr(Opcode.ld, r,g),
            Instr(Opcode.ld, r,g),
            invalid,

            Instr(Opcode.ld, B,G),
            Instr(Opcode.ld, D,G),
            Instr(Opcode.ld, H,G),
            invalid,

            Instr(Opcode.ld, X,G),
            Instr(Opcode.ld, Y,G),
            Instr(Opcode.ld, S,G),
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

            Instr(Opcode.ldi),
            Instr(Opcode.ldir),
            Instr(Opcode.ldd),
            Instr(Opcode.lddr),

            Instr(Opcode.cpi),
            Instr(Opcode.cpir),
            Instr(Opcode.cpd),
            Instr(Opcode.cpdr),

            // 60
            Instr(Opcode.add, a,g),
            Instr(Opcode.adc, a,g),
            Instr(Opcode.sub, a,g),
            Instr(Opcode.sbc, a,g),

            Instr(Opcode.and, a,g),
            Instr(Opcode.xor, a,g),
            Instr(Opcode.or,  a,g),
            Instr(Opcode.cp,  a,g),

            Instr(Opcode.add, g,Ib),
            Instr(Opcode.adc, g,Ib),
            Instr(Opcode.sub, g,Ib),
            Instr(Opcode.sbc, g,Ib),

            Instr(Opcode.and, g,Ib),
            Instr(Opcode.xor, g,Ib),
            Instr(Opcode.or,  g,Ib),
            Instr(Opcode.cp,  g,Ib),

            // 70
            Instr(Opcode.add, H,G),
            Instr(Opcode.adc, H,G),
            Instr(Opcode.sub, H,G),
            Instr(Opcode.sbc, H,G),

            Instr(Opcode.and, H,G),
            Instr(Opcode.xor, H,G),
            Instr(Opcode.or,  H,G),
            Instr(Opcode.cp,  H,G),

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
            Instr(Opcode.rlc, g),
            Instr(Opcode.rrc, g),
            Instr(Opcode.rl, g),
            Instr(Opcode.rr, g),

            Instr(Opcode.sla, g),
            Instr(Opcode.sra, g),
            Instr(Opcode.sll, g),
            Instr(Opcode.srl, g),

            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),

            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),
            Instr(Opcode.bit, i,g),

            // B0
            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),

            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),
            Instr(Opcode.res, i,g),

            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),

            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),
            Instr(Opcode.set, i,g),

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
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),

            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),

            Instr(Opcode.ret, InstrClass.Transfer),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),

            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),
            Instr(Opcode.ret, InstrClass.ConditionalTransfer, c),

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
