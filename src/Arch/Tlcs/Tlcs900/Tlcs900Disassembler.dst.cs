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
        private static Decoder[] dstDecoders = 
        {
            // 00
            Instr(Opcode.ld, Ib),
            Instr(Opcode.invalid),
            Instr(Opcode.ld, Iw),
            Instr(Opcode.invalid),

            new SecondDecoder(Opcode.pop, Zb),
            Instr(Opcode.invalid),
            new SecondDecoder(Opcode.pop, Zw),
            Instr(Opcode.invalid),

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
            // 20
            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),

            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),
            new SecondDecoder(Opcode.lda, Rw),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 30
            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),

            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),
            new SecondDecoder(Opcode.lda, Rx),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 40
            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),

            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),
            new InvDecoder(Opcode.ld, Rb),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 50
            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),

            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),
            Instr(Opcode.ld, Rw),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 60
            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),

            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),
            Instr(Opcode.ld, Rx),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),

            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            Instr(Opcode.invalid),
            // 70
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
            // 80
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
            // 90
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
            // A0
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
            // B0
            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),

            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),
            new InvDecoder(Opcode.res, i3b,Zb),

            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),

            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),
            new InvDecoder(Opcode.set, i3b,Zb),
            // C0
            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),

            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),
            new InvDecoder(Opcode.chg, i3b,Zb),

            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),

            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),
            new InvDecoder(Opcode.bit, i3b,Zb),
            // D0
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),

            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),

            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),

            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            new InvDecoder(Opcode.jp, C,Zx),
            // E0
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),

            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),

            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),

            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            new InvDecoder(Opcode.call, C,Zx),
            // F0
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),

            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),

            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),

            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
            Instr(Opcode.ret, clr,C),
        };
    }
}
