#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private static OpRecBase[] srcEncodings = new OpRecBase[]
        {
            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 10
            new OpRec(Opcode.rld, "x"),
            new OpRec(Opcode.rrd, "x"),
            new OpRec(Opcode.mul, "H,x"),
            new OpRec(Opcode.div, "H,x"),

            new OpRec(Opcode.add, "X,x"),
            new OpRec(Opcode.add, "Y,x"),
            new OpRec(Opcode.add, "S,x"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),

            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),
            new OpRec(Opcode.tset, "i,x"),

            // 20
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.ld, "r,x"),

            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.ld, "r,x"),
            new OpRec(Opcode.invalid, ""),

            // 30
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 40
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "B,x"),
            new OpRec(Opcode.ld, "D,x"),
            new OpRec(Opcode.ld, "H,x"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ld, "X,x"),
            new OpRec(Opcode.ld, "Y,x"),
            new OpRec(Opcode.ld, "S,x"),
            new OpRec(Opcode.invalid, ""),

            // 50
            new OpRec(Opcode.ex, "x,B"),
            new OpRec(Opcode.ex, "x,D"),
            new OpRec(Opcode.ex, "x,H"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.ex, "x,X"),
            new OpRec(Opcode.ex, "x,Y"),
            new OpRec(Opcode.ex, "x,S"),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 60
            new OpRec(Opcode.add, "a,x"),
            new OpRec(Opcode.adc, "a,x"),
            new OpRec(Opcode.sub, "a,x"),
            new OpRec(Opcode.sbc, "a,x"),

            new OpRec(Opcode.and, "a,x"),
            new OpRec(Opcode.xor, "a,x"),
            new OpRec(Opcode.or,  "a,x"),
            new OpRec(Opcode.cp,  "a,x"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 70
            new OpRec(Opcode.add, "H,x"),
            new OpRec(Opcode.adc, "H,x"),
            new OpRec(Opcode.sub, "H,x"),
            new OpRec(Opcode.sbc, "H,x"),

            new OpRec(Opcode.and, "H,x"),
            new OpRec(Opcode.xor, "H,x"),
            new OpRec(Opcode.or,  "H,x"),
            new OpRec(Opcode.cp,  "H,x"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 80
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.inc, "x"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.dec, "x"),

            // 90
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.incw, "x"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.decw, "x"),

            // A0
            new OpRec(Opcode.rlc, "x"),
            new OpRec(Opcode.rrc, "x"),
            new OpRec(Opcode.rl, "x"),
            new OpRec(Opcode.rr, "x"),

            new OpRec(Opcode.sla, "x"),
            new OpRec(Opcode.sra, "x"),
            new OpRec(Opcode.sll, "x"),
            new OpRec(Opcode.srl, "x"),

            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),

            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),
            new OpRec(Opcode.bit, "i,x"),

            // B0
            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),

            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),
            new OpRec(Opcode.res, "i,x"),

            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),

            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),
            new OpRec(Opcode.set, "i,x"),

            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

        };
    }
}
