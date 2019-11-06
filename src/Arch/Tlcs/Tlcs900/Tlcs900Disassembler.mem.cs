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
        private static Decoder[] memOpRecs = 
        {
            // 00
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            new SecondDecoder(Opcode.rld, A),
            new SecondDecoder(Opcode.rrd, A),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 10
            Instr(Opcode.invalid),
            new LdirDecoder(),
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
            // 20
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),

            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),
            new SecondDecoder(Opcode.ld, Rz),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 30
            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),

            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),
            Instr(Opcode.ex, Rz),

            Instr(Opcode.add, Iz),
            Instr(Opcode.adc, Iz),
            Instr(Opcode.sub, Iz),
            Instr(Opcode.sbc, Iz),

            Instr(Opcode.and, Iz),
            Instr(Opcode.xor, Iz),
            Instr(Opcode.or, Iz),
            Instr(Opcode.cp, Iz),
            // 40
            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),

            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),
            new InvDecoder(Opcode.mul, Rz),

            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),

            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),
            new InvDecoder(Opcode.muls, Rz),
            // 50
            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),

            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),
            new InvDecoder(Opcode.div, Rz),

            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),

            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),
            new InvDecoder(Opcode.divs, Rz),
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
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            new SecondDecoder(Opcode.rlc),
            new SecondDecoder(Opcode.rrc),
            new SecondDecoder(Opcode.rl),
            new SecondDecoder(Opcode.rr),

            new SecondDecoder(Opcode.sla),
            new SecondDecoder(Opcode.sra),
            new SecondDecoder(Opcode.sll),
            new SecondDecoder(Opcode.srl),
            // 80
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),

            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),
            new SecondDecoder(Opcode.add, Rz),

            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),

            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            new InvDecoder(Opcode.add, Rz),
            // 90
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),

            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),
            new SecondDecoder(Opcode.adc, Rz),

            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),

            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            new InvDecoder(Opcode.adc, Rz),
            // A0
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),

            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),
            new InvDecoder(Opcode.sub, Rz),

            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),

            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            // B0
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),

            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),
            new SecondDecoder(Opcode.sbc, Rz),

            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),

            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            new InvDecoder(Opcode.sbc, Rz),
            // C0
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),

            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),
            new SecondDecoder(Opcode.and, Rz),

            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),

            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            new InvDecoder(Opcode.and, Rz),
            // D0
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),

            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),
            new SecondDecoder(Opcode.xor, Rz),

            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),

            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            new InvDecoder(Opcode.xor, Rz),
            // E0
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),

            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),
            new SecondDecoder(Opcode.or, Rz),

            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),

            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            new InvDecoder(Opcode.or, Rz),
            // F0
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),

            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),
            new SecondDecoder(Opcode.cp, Rz),

            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),

            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
            new InvDecoder(Opcode.cp, Rz),
        };
    }
}
