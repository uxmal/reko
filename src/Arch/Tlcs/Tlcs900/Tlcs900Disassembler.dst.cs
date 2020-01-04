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
        private static Decoder[] dstDecoders = 
        {
            // 00
            Instr(Mnemonic.ld, Ib),
            invalid,
            Instr(Mnemonic.ld, Iw),
            invalid,

            new SecondDecoder(Mnemonic.pop, Zb),
            invalid,
            new SecondDecoder(Mnemonic.pop, Zw),
            invalid,

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
            // 20
            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),

            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),
            new SecondDecoder(Mnemonic.lda, Rw),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 30
            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),

            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),
            new SecondDecoder(Mnemonic.lda, Rx),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 40
            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),

            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),
            new InvDecoder(Mnemonic.ld, Rb),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 50
            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),

            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),
            Instr(Mnemonic.ld, Rw),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 60
            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),

            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),
            Instr(Mnemonic.ld, Rx),

            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,
            // 70
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
            // 80
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
            // 90
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
            // A0
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
            // B0
            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),

            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),
            new InvDecoder(Mnemonic.res, i3b,Zb),

            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),

            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),
            new InvDecoder(Mnemonic.set, i3b,Zb),
            // C0
            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),

            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),
            new InvDecoder(Mnemonic.chg, i3b,Zb),

            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),

            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),
            new InvDecoder(Mnemonic.bit, i3b,Zb),
            // D0
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),

            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),

            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),

            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            new InvDecoder(Mnemonic.jp, C,Zx),
            // E0
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),

            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),

            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),

            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            new InvDecoder(Mnemonic.call, C,Zx),
            // F0
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),

            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),

            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),

            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
            Instr(Mnemonic.ret, clr,C),
        };
    }
}
