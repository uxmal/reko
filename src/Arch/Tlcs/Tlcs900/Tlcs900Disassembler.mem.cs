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
        private static OpRecBase[] memOpRecs = 
        {
            // 00
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            new SecondOpRec(Opcode.rld, A),
            new SecondOpRec(Opcode.rrd, A),

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
            new LdirOpRec(),
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
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),

            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),
            new SecondOpRec(Opcode.ld, Rz),

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
            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),

            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),
            new InvOpRec(Opcode.mul, Rz),

            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),

            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),
            new InvOpRec(Opcode.muls, Rz),
            // 50
            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),

            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),
            new InvOpRec(Opcode.div, Rz),

            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),

            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),
            new InvOpRec(Opcode.divs, Rz),
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
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            new SecondOpRec(Opcode.rlc),
            new SecondOpRec(Opcode.rrc),
            new SecondOpRec(Opcode.rl),
            new SecondOpRec(Opcode.rr),

            new SecondOpRec(Opcode.sla),
            new SecondOpRec(Opcode.sra),
            new SecondOpRec(Opcode.sll),
            new SecondOpRec(Opcode.srl),
            // 80
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),

            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),
            new SecondOpRec(Opcode.add, Rz),

            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),

            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            new InvOpRec(Opcode.add, Rz),
            // 90
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),

            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),
            new SecondOpRec(Opcode.adc, Rz),

            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),

            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            new InvOpRec(Opcode.adc, Rz),
            // A0
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),

            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),
            new InvOpRec(Opcode.sub, Rz),

            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),

            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            Instr(Opcode.sub, Rz),
            // B0
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),

            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),
            new SecondOpRec(Opcode.sbc, Rz),

            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),

            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            new InvOpRec(Opcode.sbc, Rz),
            // C0
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),

            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),
            new SecondOpRec(Opcode.and, Rz),

            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),

            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            new InvOpRec(Opcode.and, Rz),
            // D0
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),

            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),
            new SecondOpRec(Opcode.xor, Rz),

            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),

            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            new InvOpRec(Opcode.xor, Rz),
            // E0
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),

            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),
            new SecondOpRec(Opcode.or, Rz),

            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),

            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            new InvOpRec(Opcode.or, Rz),
            // F0
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),

            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),
            new SecondOpRec(Opcode.cp, Rz),

            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),

            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
            new InvOpRec(Opcode.cp, Rz),
        };
    }
}
