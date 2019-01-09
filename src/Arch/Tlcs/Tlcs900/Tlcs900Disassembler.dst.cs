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
        private static OpRecBase[] dstOpRecs = 
        {
            // 00
            new InvOpRec(Opcode.ld, "Ib"),
            new OpRec(Opcode.invalid, ""),
            new InvOpRec(Opcode.ld, "Iw"),
            new OpRec(Opcode.invalid, ""),

            new SecondOpRec(Opcode.pop, "Zb"),
            new OpRec(Opcode.invalid, ""),
            new SecondOpRec(Opcode.pop, "Zw"),
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
            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),

            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),
            new SecondOpRec(Opcode.lda, "Rw"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 30
            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),

            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),
            new SecondOpRec(Opcode.lda, "Rx"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 40
            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),

            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),
            new InvOpRec(Opcode.ld, "Rb"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 50
            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),

            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),
            new InvOpRec(Opcode.ld, "Rw"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            // 60
            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),

            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),
            new InvOpRec(Opcode.ld, "Rx"),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
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

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),

            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
            new OpRec(Opcode.invalid, ""),
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
            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),

            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),
            new SecondOpRec(Opcode.res, "3b"),

            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),

            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),
            new SecondOpRec(Opcode.set, "3b"),
            // C0
            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),

            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),
            new SecondOpRec(Opcode.chg, "3b"),

            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),

            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),
            new SecondOpRec(Opcode.bit, "3b"),
            // D0
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),

            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),

            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),

            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            new SecondOpRec(Opcode.jp, "C"),
            // E0
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),

            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),

            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),

            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            new SecondOpRec(Opcode.call, "C"),
            // F0
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),

            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),

            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),

            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
            new OpRec(Opcode.ret, "C"),
        };
    }
}
