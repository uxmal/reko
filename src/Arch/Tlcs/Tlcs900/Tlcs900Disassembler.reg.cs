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
using Opcode = Reko.Arch.Tlcs.Tlcs900.Tlcs900Mnemonic;

namespace Reko.Arch.Tlcs.Tlcs900
{
    using Decoder = Decoder<Tlcs900Disassembler, Opcode, Tlcs900Instruction>;

    partial class Tlcs900Disassembler
    {
        private static Decoder[] regDecoders = 
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
            Instr(Opcode.divs, BW,Iz),

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
            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),

            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),
            new InvDecoder(Opcode.mul, BW,Rz),

            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),

            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),
            new InvDecoder(Opcode.muls, BW,Rz),
            // 50
            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),

            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),
            new InvDecoder(Opcode.div, BW,Rz),

            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),

            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),
            new InvDecoder(Opcode.divs, BW,Rz),
            // 60
            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),

            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),
            new InvDecoder(Opcode.inc, qz),

            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),

            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),
            new InvDecoder(Opcode.dec, qz),
            // 70
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),

            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),

            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),

            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            new InvDecoder(Opcode.scc, BW,C),
            // 80
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),

            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),

            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),

            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),
            new InvDecoder(Opcode.ld, Rz),
            // 90
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),

            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),

            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            Instr(Opcode.ld, Rz),
            // A0
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),

            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),

            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            Instr(Opcode.ld,i3z),
            // B0
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),

            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),

            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),

            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),
            new InvDecoder(Opcode.ex, BW,Rz),
            // C0
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),

            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),

            Instr(Opcode.add, Iz),
            Instr(Opcode.adc, Iz),
            Instr(Opcode.sub, Iz),
            Instr(Opcode.sbc, Iz),

            Instr(Opcode.and, Iz),
            Instr(Opcode.xor, Iz),
            Instr(Opcode.or,  Iz),
            Instr(Opcode.cp,  Iz),
            // D0
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),

            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),

            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            Instr(Opcode.cp,i3z),
            // E0
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),

            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),

            new InvDecoder(Opcode.rlc, Ib),
            new InvDecoder(Opcode.rrc, Ib),
            new InvDecoder(Opcode.rl,  Ib),
            new InvDecoder(Opcode.rr,  Ib),

            new InvDecoder(Opcode.sla, Ib),
            new InvDecoder(Opcode.sra, Ib),
            new InvDecoder(Opcode.sll, Ib),
            new InvDecoder(Opcode.srl, Ib),
            // F0
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),

            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),

            new InvDecoder(Opcode.rlc, A),
            new InvDecoder(Opcode.rrc, A),
            new InvDecoder(Opcode.rl, A),
            new InvDecoder(Opcode.rr, A),

            new InvDecoder(Opcode.sla, A),
            new InvDecoder(Opcode.sra, A),
            new InvDecoder(Opcode.sll, A),
            new InvDecoder(Opcode.srl, A),
        };
    }
}
