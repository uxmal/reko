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
            new OpRec(Opcode.mul, "H,g"),
            new OpRec(Opcode.div, "H,g"),

            new OpRec(Opcode.add, "X,G"),
            new OpRec(Opcode.add, "Y,G"),
            new OpRec(Opcode.add, "S,G"),
            invalid,

            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),

            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),
            new OpRec(Opcode.tset, "i,g"),

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
            new OpRec(Opcode.ld, "r,g"),
            new OpRec(Opcode.ld, "r,g"),
            new OpRec(Opcode.ld, "r,g"),
            new OpRec(Opcode.ld, "r,g"),

            new OpRec(Opcode.ld, "r,g"),
            new OpRec(Opcode.ld, "r,g"),
            new OpRec(Opcode.ld, "r,g"),
            invalid,

            new OpRec(Opcode.ld, "B,G"),
            new OpRec(Opcode.ld, "D,G"),
            new OpRec(Opcode.ld, "H,G"),
            invalid,

            new OpRec(Opcode.ld, "X,G"),
            new OpRec(Opcode.ld, "Y,G"),
            new OpRec(Opcode.ld, "S,G"),
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

            new OpRec(Opcode.ldi, ""),
            new OpRec(Opcode.ldir, ""),
            new OpRec(Opcode.ldd, ""),
            new OpRec(Opcode.lddr, ""),

            new OpRec(Opcode.cpi, ""),
            new OpRec(Opcode.cpir, ""),
            new OpRec(Opcode.cpd, ""),
            new OpRec(Opcode.cpdr, ""),

            // 60
            new OpRec(Opcode.add, "a,g"),
            new OpRec(Opcode.adc, "a,g"),
            new OpRec(Opcode.sub, "a,g"),
            new OpRec(Opcode.sbc, "a,g"),

            new OpRec(Opcode.and, "a,g"),
            new OpRec(Opcode.xor, "a,g"),
            new OpRec(Opcode.or,  "a,g"),
            new OpRec(Opcode.cp,  "a,g"),

            new OpRec(Opcode.add, "g,Ib"),
            new OpRec(Opcode.adc, "g,Ib"),
            new OpRec(Opcode.sub, "g,Ib"),
            new OpRec(Opcode.sbc, "g,Ib"),

            new OpRec(Opcode.and, "g,Ib"),
            new OpRec(Opcode.xor, "g,Ib"),
            new OpRec(Opcode.or,  "g,Ib"),
            new OpRec(Opcode.cp,  "g,Ib"),

            // 70
            new OpRec(Opcode.add, "H,G"),
            new OpRec(Opcode.adc, "H,G"),
            new OpRec(Opcode.sub, "H,G"),
            new OpRec(Opcode.sbc, "H,G"),

            new OpRec(Opcode.and, "H,G"),
            new OpRec(Opcode.xor, "H,G"),
            new OpRec(Opcode.or,  "H,G"),
            new OpRec(Opcode.cp,  "H,G"),

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
            new OpRec(Opcode.rlc, "g"),
            new OpRec(Opcode.rrc, "g"),
            new OpRec(Opcode.rl, "g"),
            new OpRec(Opcode.rr, "g"),

            new OpRec(Opcode.sla, "g"),
            new OpRec(Opcode.sra, "g"),
            new OpRec(Opcode.sll, "g"),
            new OpRec(Opcode.srl, "g"),

            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),

            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),
            new OpRec(Opcode.bit, "i,g"),

            // B0
            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),

            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),
            new OpRec(Opcode.res, "i,g"),

            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),

            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),
            new OpRec(Opcode.set, "i,g"),

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
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.ret, "", InstrClass.Transfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.ret, "c", InstrClass.ConditionalTransfer),

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
