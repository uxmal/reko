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
        private static Decoder[] memDecoders = 
        {
            // 00
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            new SecondDecoder(Mnemonic.rld, A),
            new SecondDecoder(Mnemonic.rrd, A),

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
            new LdirDecoder(),
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
            // 20
            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),

            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),
            new SecondDecoder(Mnemonic.ld, Rz),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 30
            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),

            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),
            Instr(Mnemonic.ex, Rz),

            Instr(Mnemonic.add, Iz),
            Instr(Mnemonic.adc, Iz),
            Instr(Mnemonic.sub, Iz),
            Instr(Mnemonic.sbc, Iz),

            Instr(Mnemonic.and, Iz),
            Instr(Mnemonic.xor, Iz),
            Instr(Mnemonic.or, Iz),
            Instr(Mnemonic.cp, Iz),
            // 40
            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),

            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),
            new InvDecoder(Mnemonic.mul, Rz),

            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),

            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),
            new InvDecoder(Mnemonic.muls, Rz),
            // 50
            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),

            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),
            new InvDecoder(Mnemonic.div, Rz),

            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),

            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),
            new InvDecoder(Mnemonic.divs, Rz),
            // 60
            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),

            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),
            new SecondDecoder(Mnemonic.inc, qz),

            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),

            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),
            new SecondDecoder(Mnemonic.dec, qz),
            // 70
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            new SecondDecoder(Mnemonic.rlc),
            new SecondDecoder(Mnemonic.rrc),
            new SecondDecoder(Mnemonic.rl),
            new SecondDecoder(Mnemonic.rr),

            new SecondDecoder(Mnemonic.sla),
            new SecondDecoder(Mnemonic.sra),
            new SecondDecoder(Mnemonic.sll),
            new SecondDecoder(Mnemonic.srl),
            // 80
            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),

            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),
            new SecondDecoder(Mnemonic.add, Rz),

            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),

            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            new InvDecoder(Mnemonic.add, Rz),
            // 90
            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),

            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),
            new SecondDecoder(Mnemonic.adc, Rz),

            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),

            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            new InvDecoder(Mnemonic.adc, Rz),
            // A0
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),

            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),
            new InvDecoder(Mnemonic.sub, Rz),

            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),

            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),
            Instr(Mnemonic.sub, Rz),
            // B0
            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),

            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),
            new SecondDecoder(Mnemonic.sbc, Rz),

            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),

            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            new InvDecoder(Mnemonic.sbc, Rz),
            // C0
            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),

            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),
            new SecondDecoder(Mnemonic.and, Rz),

            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),

            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            new InvDecoder(Mnemonic.and, Rz),
            // D0
            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),

            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),
            new SecondDecoder(Mnemonic.xor, Rz),

            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),

            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            new InvDecoder(Mnemonic.xor, Rz),
            // E0
            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),

            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),
            new SecondDecoder(Mnemonic.or, Rz),

            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),

            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            new InvDecoder(Mnemonic.or, Rz),
            // F0
            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),

            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),
            new SecondDecoder(Mnemonic.cp, Rz),

            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),

            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
            new InvDecoder(Mnemonic.cp, Rz),
        };
    }
}
