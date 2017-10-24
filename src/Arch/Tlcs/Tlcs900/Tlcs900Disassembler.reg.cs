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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Opcode;

namespace Reko.Arch.Tlcs.Tlcs900
{
    partial class Tlcs900Disassembler
    {
        private static OpRecBase[] regOpRecs = 
        {
            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new InvOpRec(Opcode.ld, "Iz"),

            new SecondOpRec(Opcode.push, ""),
            new SecondOpRec(Opcode.pop, ""),
            new SecondOpRec(Opcode.cpl, ""),
            new SecondOpRec(Opcode.neg, ""),

            new InvOpRec(Opcode.mul, "Iz"),
            new InvOpRec(Opcode.muls, "Iz"),
            new InvOpRec(Opcode.div, "Iz"),
            new InvOpRec(Opcode.divs, "Iz"),

            new InvOpRec(Opcode.link, "Iw"),
            new SecondOpRec(Opcode.unlk, ""),
            new SecondOpRec(Opcode.bs1f, "A"),
            new SecondOpRec(Opcode.bs1b, "A"),
            // 10
            new SecondOpRec(Opcode.daa, ""),
            new OpRec(Opcode.invalid, ""),
            new SecondOpRec(Opcode.extz, ""),
            new SecondOpRec(Opcode.exts, ""),

            new SecondOpRec(Opcode.paa, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new InvOpRec(Opcode.djnz, "jb"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 20
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
            // 30
            new OpRec(Opcode.res, "Ib,Rz"),
            new OpRec(Opcode.set, "Ib,Rz"),
            new OpRec(Opcode.chg, "Ib,Rz"),
            new OpRec(Opcode.bit, "Ib,Rz"),

            new OpRec(Opcode.tset, "Ib,Rz"),
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
            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),

            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),
            new SecondOpRec(Opcode.mul, "Rz"),

            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),

            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),
            new SecondOpRec(Opcode.muls, "Rz"),
            // 50
            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),

            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),
            new SecondOpRec(Opcode.div, "Rz"),

            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),

            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),
            new SecondOpRec(Opcode.divs, "Rz"),
            // 60
            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),

            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),
            new SecondOpRec(Opcode.inc, "#z"),

            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),

            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),
            new SecondOpRec(Opcode.dec, "#z"),
            // 70
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),

            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),

            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),

            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            new SecondOpRec(Opcode.scc, "C"),
            // 80
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),

            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),

            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),

            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            // 90
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),

            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),

            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),

            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),
            new InvOpRec(Opcode.ld, "Rz"),
            // A0
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),

            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),

            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),

            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),
            new InvOpRec(Opcode.ld, "3z"),
            // B0
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),

            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),

            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),

            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),
            new SecondOpRec(Opcode.ex, "Rz"),
            // C0
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),

            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),

            new InvOpRec(Opcode.add, "Iz"),
            new InvOpRec(Opcode.adc, "Iz"),
            new InvOpRec(Opcode.sub, "Iz"),
            new InvOpRec(Opcode.sbc, "Iz"),

            new InvOpRec(Opcode.and, "Iz"),
            new InvOpRec(Opcode.xor, "Iz"),
            new InvOpRec(Opcode.or,  "Iz"),
            new InvOpRec(Opcode.cp,  "Iz"),
            // D0
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),

            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),

            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),

            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            // E0
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),

            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),

            new SecondOpRec(Opcode.rlc, "Ib"),
            new SecondOpRec(Opcode.rrc, "Ib"),
            new SecondOpRec(Opcode.rl,  "Ib"),
            new SecondOpRec(Opcode.rr,  "Ib"),

            new SecondOpRec(Opcode.sla, "Ib"),
            new SecondOpRec(Opcode.sra, "Ib"),
            new SecondOpRec(Opcode.sll, "Ib"),
            new SecondOpRec(Opcode.srl, "Ib"),
            // F0
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),

            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),

            new SecondOpRec(Opcode.rlc, "A"),
            new SecondOpRec(Opcode.rrc, "A"),
            new SecondOpRec(Opcode.rl, "A"),
            new SecondOpRec(Opcode.rr, "A"),

            new SecondOpRec(Opcode.sla, "A"),
            new SecondOpRec(Opcode.sra, "A"),
            new SecondOpRec(Opcode.sll, "A"),
            new SecondOpRec(Opcode.srl, "A"),
        };
    }
}
