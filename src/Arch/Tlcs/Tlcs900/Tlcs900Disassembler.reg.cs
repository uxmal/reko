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
    using Decoder = Decoder<Tlcs900Disassembler, Opcode, Tlcs900Instruction>;

    partial class Tlcs900Disassembler
    {
        private static Decoder[] regOpRecs = 
        {
            // 00
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.ld, Iz),

            new SecondDecoder(Opcode.push),
            new SecondDecoder(Opcode.pop),
            new SecondDecoder(Opcode.cpl),
            new SecondDecoder(Opcode.neg),

            Instr(Opcode.mul, Iz),
            Instr(Opcode.muls, Iz),
            Instr(Opcode.div, Iz),
            Instr(Opcode.divs, Iz),

            Instr(Opcode.link, Iw),
            new SecondDecoder(Opcode.unlk),
            new SecondDecoder(Opcode.bs1f, A),
            new SecondDecoder(Opcode.bs1b, A),
            // 10
            new SecondDecoder(Opcode.daa),
            Instr(Opcode.invalid),
            new SecondDecoder(Opcode.extz),
            new SecondDecoder(Opcode.exts),

            new SecondDecoder(Opcode.paa),
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
            new InvDecoder(Opcode.res, Ib),
            new InvDecoder(Opcode.set, Ib),
            new InvDecoder(Opcode.chg, Ib),
            new InvDecoder(Opcode.bit, Ib),

            new InvDecoder(Opcode.tset, Ib),
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
            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),

            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),
            new SecondDecoder(Opcode.mul, Rz),

            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),

            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),
            new SecondDecoder(Opcode.muls, Rz),
            // 50
            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),

            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),
            new SecondDecoder(Opcode.div, Rz),

            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),

            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),
            new SecondDecoder(Opcode.divs, Rz),
            // 60
            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),

            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),
            new SecondDecoder(Opcode.inc, qz),

            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),

            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),
            new SecondDecoder(Opcode.dec, qz),
            // 70
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),

            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),

            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),

            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            new SecondDecoder(Opcode.scc, C),
            // 80
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),

            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),

            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),

            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            // 90
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),

            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            // A0
            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),

            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),
            new SecondDecoder(Opcode.sub, Rz),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            // B0
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),

            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),

            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),

            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),
            new SecondDecoder(Opcode.ex, Rz),
            // C0
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),

            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),

            Instr(Opcode.add, Iz),
            Instr(Opcode.adc, Iz),
            Instr(Opcode.sub, Iz),
            Instr(Opcode.sbc, Iz),

            Instr(Opcode.and, Iz),
            Instr(Opcode.xor, Iz),
            Instr(Opcode.or,  Iz),
            Instr(Opcode.cp,  Iz),
            // D0
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),

            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            // E0
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),

            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),

            new SecondDecoder(Opcode.rlc, Ib),
            new SecondDecoder(Opcode.rrc, Ib),
            new SecondDecoder(Opcode.rl,  Ib),
            new SecondDecoder(Opcode.rr,  Ib),

            new SecondDecoder(Opcode.sla, Ib),
            new SecondDecoder(Opcode.sra, Ib),
            new SecondDecoder(Opcode.sll, Ib),
            new SecondDecoder(Opcode.srl, Ib),
            // F0
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),

            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),

            new SecondDecoder(Opcode.rlc, A),
            new SecondDecoder(Opcode.rrc, A),
            new SecondDecoder(Opcode.rl, A),
            new SecondDecoder(Opcode.rr, A),

            new SecondDecoder(Opcode.sla, A),
            new SecondDecoder(Opcode.sra, A),
            new SecondDecoder(Opcode.sll, A),
            new SecondDecoder(Opcode.srl, A),
        };
    }
}
