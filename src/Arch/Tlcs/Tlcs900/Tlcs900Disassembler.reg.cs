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
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Opcode;

namespace Reko.Arch.Tlcs.Tlcs900
{
    partial class Tlcs900Disassembler
    {
        private static OpRecBase[] regOpRecs = 
        {
            // 00
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.ld, Iz),

            new SecondOpRec(Opcode.push),
            new SecondOpRec(Opcode.pop),
            new SecondOpRec(Opcode.cpl),
            new SecondOpRec(Opcode.neg),

            Instr(Opcode.mul, Iz),
            Instr(Opcode.muls, Iz),
            Instr(Opcode.div, Iz),
            Instr(Opcode.divs, Iz),

            Instr(Opcode.link, Iw),
            new SecondOpRec(Opcode.unlk),
            new SecondOpRec(Opcode.bs1f, A),
            new SecondOpRec(Opcode.bs1b, A),
            // 10
            new SecondOpRec(Opcode.daa),
            Instr(Opcode.invalid),
            new SecondOpRec(Opcode.extz),
            new SecondOpRec(Opcode.exts),

            new SecondOpRec(Opcode.paa),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.djnz, jb),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 20
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 30
            new InvOpRec(Opcode.res, Ib),
            new InvOpRec(Opcode.set, Ib),
            new InvOpRec(Opcode.chg, Ib),
            new InvOpRec(Opcode.bit, Ib),

            new InvOpRec(Opcode.tset, Ib),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 40
            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),

            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),
            new SecondOpRec(Opcode.mul, Rz),

            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),

            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),
            new SecondOpRec(Opcode.muls, Rz),
            // 50
            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),

            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),
            new SecondOpRec(Opcode.div, Rz),

            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),

            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),
            new SecondOpRec(Opcode.divs, Rz),
            // 60
            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),

            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),
            new SecondOpRec(Opcode.inc, qz),

            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),

            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),
            new SecondOpRec(Opcode.dec, qz),
            // 70
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),

            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),

            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),

            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            new SecondOpRec(Opcode.scc, C),
            // 80
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),

            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),

            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),

            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            // 90
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),

            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            // A0
            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),

            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),
            new SecondOpRec(Opcode.sub, Rz),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            // B0
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),

            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),

            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),

            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),
            new SecondOpRec(Opcode.ex, Rz),
            // C0
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),

            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),

            Instr(Opcode.add, Iz),
            Instr(Opcode.adc, Iz),
            Instr(Opcode.sub, Iz),
            Instr(Opcode.sbc, Iz),

            Instr(Opcode.and, Iz),
            Instr(Opcode.xor, Iz),
            Instr(Opcode.or,  Iz),
            Instr(Opcode.cp,  Iz),
            // D0
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),

            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            // E0
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),

            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),

            new SecondOpRec(Opcode.rlc, Ib),
            new SecondOpRec(Opcode.rrc, Ib),
            new SecondOpRec(Opcode.rl,  Ib),
            new SecondOpRec(Opcode.rr,  Ib),

            new SecondOpRec(Opcode.sla, Ib),
            new SecondOpRec(Opcode.sra, Ib),
            new SecondOpRec(Opcode.sll, Ib),
            new SecondOpRec(Opcode.srl, Ib),
            // F0
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),

            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),

            new SecondOpRec(Opcode.rlc, A),
            new SecondOpRec(Opcode.rrc, A),
            new SecondOpRec(Opcode.rl, A),
            new SecondOpRec(Opcode.rr, A),

            new SecondOpRec(Opcode.sla, A),
            new SecondOpRec(Opcode.sra, A),
            new SecondOpRec(Opcode.sll, A),
            new SecondOpRec(Opcode.srl, A),
        };
    }
}
