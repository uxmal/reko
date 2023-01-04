#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
        private static readonly Decoder[] srcEncodings = new Decoder[]
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
            Instr(Mnemonic.rld, db,x),
            Instr(Mnemonic.rrd, db,x),
            Instr(Mnemonic.mul, H,x),
            Instr(Mnemonic.div, H,x),

            Instr(Mnemonic.add, X,x),
            Instr(Mnemonic.add, Y,x),
            Instr(Mnemonic.add, S,x),
            invalid,

            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),

            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),
            Instr(Mnemonic.tset, db,i,x),

            // 20
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.ld, r,x),
            Instr(Mnemonic.ld, r,x),
            Instr(Mnemonic.ld, r,x),
            Instr(Mnemonic.ld, r,x),

            Instr(Mnemonic.ld, r,x),
            Instr(Mnemonic.ld, r,x),
            Instr(Mnemonic.ld, r,x),
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

            Instr(Mnemonic.ld, B,x),
            Instr(Mnemonic.ld, D,x),
            Instr(Mnemonic.ld, H,x),
            invalid,

            Instr(Mnemonic.ld, X,x),
            Instr(Mnemonic.ld, Y,x),
            Instr(Mnemonic.ld, S,x),
            invalid,

            // 50
            Instr(Mnemonic.ex, x,B),
            Instr(Mnemonic.ex, x,D),
            Instr(Mnemonic.ex, x,H),
            invalid,

            Instr(Mnemonic.ex, x,X),
            Instr(Mnemonic.ex, x,Y),
            Instr(Mnemonic.ex, x,S),
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
            Instr(Mnemonic.add, a,x),
            Instr(Mnemonic.adc, a,x),
            Instr(Mnemonic.sub, a,x),
            Instr(Mnemonic.sbc, a,x),

            Instr(Mnemonic.and, a,x),
            Instr(Mnemonic.xor, a,x),
            Instr(Mnemonic.or,  a,x),
            Instr(Mnemonic.cp,  a,x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 70
            Instr(Mnemonic.add, H,x),
            Instr(Mnemonic.adc, H,x),
            Instr(Mnemonic.sub, H,x),
            Instr(Mnemonic.sbc, H,x),

            Instr(Mnemonic.and, H,x),
            Instr(Mnemonic.xor, H,x),
            Instr(Mnemonic.or,  H,x),
            Instr(Mnemonic.cp,  H,x),

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
            Instr(Mnemonic.inc, db, x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.dec, db, x),

            // 90
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.incw, dw, x),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.decw, dw,x),

            // A0
            Instr(Mnemonic.rlc, db,x),
            Instr(Mnemonic.rrc, db,x),
            Instr(Mnemonic.rl, db,x),
            Instr(Mnemonic.rr, db,x),

            Instr(Mnemonic.sla, db,x),
            Instr(Mnemonic.sra, db,x),
            Instr(Mnemonic.sll, db,x),
            Instr(Mnemonic.srl, db,x),

            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),

            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),
            Instr(Mnemonic.bit, db,i,x),

            // B0
            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),

            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),
            Instr(Mnemonic.res, db,i,x),

            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),

            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),
            Instr(Mnemonic.set, db,i,x),

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
