#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Opcode = Reko.Arch.Tlcs.Tlcs900Opcode;

namespace Reko.Arch.Tlcs
{
    partial class Tlcs900Disassembler
    {
        private static OpRecBase[] regOpRecs = {
            // 00
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
            // 10
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new SecondOpRec(Opcode.extz, ""),
            new SecondOpRec(Opcode.exts, ""),

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
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.push, "Rw"),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 30
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
            // 40
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
            // 50
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

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 80
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),

            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),
            new SecondOpRec(Opcode.ld, "rz"),
            // 90
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
            // A0
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
            // B0
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
            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),

            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),
            new OpRec(Opcode.xor, "Rz"),

            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),

            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            new InvOpRec(Opcode.cp, "3z"),
            // E0
            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),

            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),
            new OpRec(Opcode.or, "Rz"),

            new SecondOpRec(Opcode.rlc, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),

            new SecondOpRec(Opcode.invalid, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),
            new SecondOpRec(Opcode.invalid, "Ib"),
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
