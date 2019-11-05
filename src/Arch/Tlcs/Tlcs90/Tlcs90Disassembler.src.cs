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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    using Decoder = Decoder<Tlcs90Disassembler, Opcode, Tlcs90Instruction>;

    partial class Tlcs90Disassembler
    {
        private static Decoder[] srcEncodings = new Decoder[]
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
            Instr(Opcode.rld, x),
            Instr(Opcode.rrd, x),
            Instr(Opcode.mul, H,x),
            Instr(Opcode.div, H,x),

            Instr(Opcode.add, X,x),
            Instr(Opcode.add, Y,x),
            Instr(Opcode.add, S,x),
            invalid,

            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),

            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),
            Instr(Opcode.tset, i,x),

            // 20
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Opcode.ld, r,x),
            Instr(Opcode.ld, r,x),
            Instr(Opcode.ld, r,x),
            Instr(Opcode.ld, r,x),

            Instr(Opcode.ld, r,x),
            Instr(Opcode.ld, r,x),
            Instr(Opcode.ld, r,x),
            invalid,

            // 30
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

            // 40
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Opcode.ld, B,x),
            Instr(Opcode.ld, D,x),
            Instr(Opcode.ld, H,x),
            invalid,

            Instr(Opcode.ld, X,x),
            Instr(Opcode.ld, Y,x),
            Instr(Opcode.ld, S,x),
            invalid,

            // 50
            Instr(Opcode.ex, x,B),
            Instr(Opcode.ex, x,D),
            Instr(Opcode.ex, x,H),
            invalid,

            Instr(Opcode.ex, x,X),
            Instr(Opcode.ex, x,Y),
            Instr(Opcode.ex, x,S),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 60
            Instr(Opcode.add, a,x),
            Instr(Opcode.adc, a,x),
            Instr(Opcode.sub, a,x),
            Instr(Opcode.sbc, a,x),

            Instr(Opcode.and, a,x),
            Instr(Opcode.xor, a,x),
            Instr(Opcode.or,  a,x),
            Instr(Opcode.cp,  a,x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 70
            Instr(Opcode.add, H,x),
            Instr(Opcode.adc, H,x),
            Instr(Opcode.sub, H,x),
            Instr(Opcode.sbc, H,x),

            Instr(Opcode.and, H,x),
            Instr(Opcode.xor, H,x),
            Instr(Opcode.or,  H,x),
            Instr(Opcode.cp,  H,x),

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
            Instr(Opcode.inc, x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Opcode.dec, x),

            // 90
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Opcode.incw, x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Opcode.decw, x),

            // A0
            Instr(Opcode.rlc, x),
            Instr(Opcode.rrc, x),
            Instr(Opcode.rl, x),
            Instr(Opcode.rr, x),

            Instr(Opcode.sla, x),
            Instr(Opcode.sra, x),
            Instr(Opcode.sll, x),
            Instr(Opcode.srl, x),

            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),

            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),
            Instr(Opcode.bit, i,x),

            // B0
            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),

            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),
            Instr(Opcode.res, i,x),

            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),

            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),
            Instr(Opcode.set, i,x),

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

        };
    }
}
