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
            Instr(Opcode.divs, BW,Iz),

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
            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),

            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),
            new InvOpRec(Opcode.mul, BW,Rz),

            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),

            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),
            new InvOpRec(Opcode.muls, BW,Rz),
            // 50
            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),

            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),
            new InvOpRec(Opcode.div, BW,Rz),

            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),

            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),
            new InvOpRec(Opcode.divs, BW,Rz),
            // 60
            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),

            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),
            new InvOpRec(Opcode.inc, qz),

            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),

            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),
            new InvOpRec(Opcode.dec, qz),
            // 70
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),

            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),

            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),

            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            new InvOpRec(Opcode.scc, BW,C),
            // 80
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),

            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),

            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),

            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),
            new InvOpRec(Opcode.ld, Rz),
            // 90
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),

            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            // A0
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),

            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            // B0
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),

            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),

            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),

            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),
            new InvOpRec(Opcode.ex, BW,Rz),
            // C0
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),

            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),

            Instr(Opcode.add, Iz),
            Instr(Opcode.adc, Iz),
            Instr(Opcode.sub, Iz),
            Instr(Opcode.sbc, Iz),

            Instr(Opcode.and, Iz),
            Instr(Opcode.xor, Iz),
            Instr(Opcode.or,  Iz),
            Instr(Opcode.cp,  Iz),
            // D0
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),

            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            // E0
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),

            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),

            new InvOpRec(Opcode.rlc, Ib),
            new InvOpRec(Opcode.rrc, Ib),
            new InvOpRec(Opcode.rl,  Ib),
            new InvOpRec(Opcode.rr,  Ib),

            new InvOpRec(Opcode.sla, Ib),
            new InvOpRec(Opcode.sra, Ib),
            new InvOpRec(Opcode.sll, Ib),
            new InvOpRec(Opcode.srl, Ib),
            // F0
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),

            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),

            new InvOpRec(Opcode.rlc, A),
            new InvOpRec(Opcode.rrc, A),
            new InvOpRec(Opcode.rl, A),
            new InvOpRec(Opcode.rr, A),

            new InvOpRec(Opcode.sla, A),
            new InvOpRec(Opcode.sra, A),
            new InvOpRec(Opcode.sll, A),
            new InvOpRec(Opcode.srl, A),
        };
    }
}
