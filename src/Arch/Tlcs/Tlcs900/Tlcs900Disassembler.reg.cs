#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Mnemonic = Reko.Arch.Tlcs.Tlcs900.Tlcs900Mnemonic;

namespace Reko.Arch.Tlcs.Tlcs900
{
    using Decoder = Decoder<Tlcs900Disassembler, Mnemonic, Tlcs900Instruction>;

    partial class Tlcs900Disassembler
    {
        private static Decoder[] regDecoders = 
        {
            // 00
            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.ld, Iz),

            new SecondDecoder(Mnemonic.push),
            new SecondDecoder(Mnemonic.pop),
            new SecondDecoder(Mnemonic.cpl),
            new SecondDecoder(Mnemonic.neg),

            Instr(Mnemonic.mul, Iz),
            Instr(Mnemonic.muls, Iz),
            Instr(Mnemonic.div, Iz),
            Instr(Mnemonic.divs, BW,Iz),

            Instr(Mnemonic.link, Iw),
            new SecondDecoder(Mnemonic.unlk),
            new SecondDecoder(Mnemonic.bs1f, A),
            new SecondDecoder(Mnemonic.bs1b, A),
            // 10
            new SecondDecoder(Mnemonic.daa),
            invalid,
            new SecondDecoder(Mnemonic.extz),
            new SecondDecoder(Mnemonic.exts),

            new SecondDecoder(Mnemonic.paa),
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.djnz, jb),
            invalid,
            invalid,
            invalid,
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
            new InvDecoder(Mnemonic.res, Ib),
            new InvDecoder(Mnemonic.set, Ib),
            new InvDecoder(Mnemonic.chg, Ib),
            new InvDecoder(Mnemonic.bit, Ib),

            new InvDecoder(Mnemonic.tset, Ib),
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
            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),

            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),
            new InvDecoder(Mnemonic.mul, BW,Rz),

            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),

            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),
            new InvDecoder(Mnemonic.muls, BW,Rz),
            // 50
            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),

            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),
            new InvDecoder(Mnemonic.div, BW,Rz),

            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),

            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),
            new InvDecoder(Mnemonic.divs, BW,Rz),
            // 60
            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),

            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),
            new InvDecoder(Mnemonic.inc, qz),

            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),

            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),
            new InvDecoder(Mnemonic.dec, qz),
            // 70
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),

            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),

            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),

            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            new InvDecoder(Mnemonic.scc, BW,C),
            // 80
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),

            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),

            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),

            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),
            new InvDecoder(Mnemonic.ld, Rz),
            // 90
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),

            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),

            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),

            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),
            Instr(Mnemonic.ld, Rz),
            // A0
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),

            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),

            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),

            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),
            Instr(Mnemonic.ld,i3z),
            // B0
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),

            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),

            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),

            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),
            new InvDecoder(Mnemonic.ex, BW,Rz),
            // C0
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),

            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),

            Instr(Mnemonic.add, Iz),
            Instr(Mnemonic.adc, Iz),
            Instr(Mnemonic.sub, Iz),
            Instr(Mnemonic.sbc, Iz),

            Instr(Mnemonic.and, Iz),
            Instr(Mnemonic.xor, Iz),
            Instr(Mnemonic.or,  Iz),
            Instr(Mnemonic.cp,  Iz),
            // D0
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),

            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),

            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),

            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),
            Instr(Mnemonic.cp,i3z),
            // E0
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),

            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),

            new InvDecoder(Mnemonic.rlc, Ib),
            new InvDecoder(Mnemonic.rrc, Ib),
            new InvDecoder(Mnemonic.rl,  Ib),
            new InvDecoder(Mnemonic.rr,  Ib),

            new InvDecoder(Mnemonic.sla, Ib),
            new InvDecoder(Mnemonic.sra, Ib),
            new InvDecoder(Mnemonic.sll, Ib),
            new InvDecoder(Mnemonic.srl, Ib),
            // F0
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),

            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),

            new InvDecoder(Mnemonic.rlc, A),
            new InvDecoder(Mnemonic.rrc, A),
            new InvDecoder(Mnemonic.rl, A),
            new InvDecoder(Mnemonic.rr, A),

            new InvDecoder(Mnemonic.sla, A),
            new InvDecoder(Mnemonic.sra, A),
            new InvDecoder(Mnemonic.sll, A),
            new InvDecoder(Mnemonic.srl, A),
        };
    }
}
