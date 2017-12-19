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
        private static OpRecBase[] memOpRecs = 
        {
            // 00
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new SecondOpRec(Opcode.rld, "A"),
            new SecondOpRec(Opcode.rrd, "A"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 10
            new OpRec(Opcode.invalid, ""),
            new LdirOpRec(),
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
            // 20
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),

            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),
            new SecondOpRec(Opcode.ld, "Rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 30
            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),

            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),
            new InvOpRec(Opcode.ex, "Rz"),

            new InvOpRec(Opcode.add, "Iz"),
            new InvOpRec(Opcode.adc, "Iz"),
            new InvOpRec(Opcode.sub, "Iz"),
            new InvOpRec(Opcode.sbc, "Iz"),

            new InvOpRec(Opcode.and, "Iz"),
            new InvOpRec(Opcode.xor, "Iz"),
            new InvOpRec(Opcode.or, "Iz"),
            new InvOpRec(Opcode.cp, "Iz"),
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
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new SecondOpRec(Opcode.rlc, ""),
            new SecondOpRec(Opcode.rrc, ""),
            new SecondOpRec(Opcode.rl, ""),
            new SecondOpRec(Opcode.rr, ""),

            new SecondOpRec(Opcode.sla, ""),
            new SecondOpRec(Opcode.sra, ""),
            new SecondOpRec(Opcode.sll, ""),
            new SecondOpRec(Opcode.srl, ""),
            // 80
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),

            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),
            new SecondOpRec(Opcode.add, "Rz"),

            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),

            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),
            new InvOpRec(Opcode.add, "Rz"),
            // 90
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),

            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),
            new SecondOpRec(Opcode.adc, "Rz"),

            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),

            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),
            new InvOpRec(Opcode.adc, "Rz"),
            // A0
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),

            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),
            new SecondOpRec(Opcode.sub, "Rz"),

            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),

            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),
            new InvOpRec(Opcode.sub, "Rz"),
            // B0
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),

            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),
            new SecondOpRec(Opcode.sbc, "Rz"),

            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),

            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),
            new InvOpRec(Opcode.sbc, "Rz"),
            // C0
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),

            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),
            new SecondOpRec(Opcode.and, "Rz"),

            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),

            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),
            new InvOpRec(Opcode.and, "Rz"),
            // D0
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),

            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),
            new SecondOpRec(Opcode.xor, "Rz"),

            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),

            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),
            new InvOpRec(Opcode.xor, "Rz"),
            // E0
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),

            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),
            new SecondOpRec(Opcode.or, "Rz"),

            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),

            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),
            new InvOpRec(Opcode.or, "Rz"),
            // F0
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),

            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),
            new SecondOpRec(Opcode.cp, "Rz"),

            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),

            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),
            new InvOpRec(Opcode.cp, "Rz"),
        };
    }
}
