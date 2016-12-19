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
        private static OpRecBase[] memOpRecs = {
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
            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),

            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),
            new SecondOpRec(Opcode.inc, "3z"),

            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),

            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),
            new SecondOpRec(Opcode.dec, "3z"),
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
            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),

            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),
            new SecondOpRec(Opcode.add, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 90
            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),

            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),
            new SecondOpRec(Opcode.adc, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // A0
            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),

            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),
            new SecondOpRec(Opcode.sub, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // B0
            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),

            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),
            new SecondOpRec(Opcode.sbc, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // C0
            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),

            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),
            new SecondOpRec(Opcode.and, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // D0
            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),

            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),
            new SecondOpRec(Opcode.xor, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // E0
            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),

            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),
            new SecondOpRec(Opcode.or, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // F0
            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),

            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),
            new SecondOpRec(Opcode.cp, "rz"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
        };
    }
}
