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
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
    using Decoder = Decoder<Tlcs90Disassembler, Mnemonic, Tlcs90Instruction>;

    partial class Tlcs90Disassembler
    {
        private static Decoder[] dstEncodings = new Decoder[]
        {
            // 00
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
            Instr(Mnemonic.ld, x,r),
            Instr(Mnemonic.ld, x,r),
            Instr(Mnemonic.ld, x,r),
            Instr(Mnemonic.ld, x,r),

            Instr(Mnemonic.ld, x,r),
            Instr(Mnemonic.ld, x,r),
            Instr(Mnemonic.ld, x,r),
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
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            Instr(Mnemonic.ld, x,Ib),

            Instr(Mnemonic.ld, B,x),
            Instr(Mnemonic.ld, D,x),
            Instr(Mnemonic.ld, H,x),
            invalid,

            Instr(Mnemonic.ld, X,x),
            Instr(Mnemonic.ld, Y,x),
            Instr(Mnemonic.ld, S,x),
            Instr(Mnemonic.ldw, x,Iw),

            // 40
            Instr(Mnemonic.ld, x,B),
            Instr(Mnemonic.ld, x,D),
            Instr(Mnemonic.ld, x,H),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.ld, x,X),
            Instr(Mnemonic.ld, x,Y),
            Instr(Mnemonic.ld, x,S),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            // 50
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

            // 60
            invalid,
            invalid,
            invalid,
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.add, x,Ib),
            Instr(Mnemonic.adc, x,Ib),
            Instr(Mnemonic.sub, x,Ib),
            Instr(Mnemonic.sbc, x,Ib),

            Instr(Mnemonic.and, x,Ib),
            Instr(Mnemonic.xor, x,Ib),
            Instr(Mnemonic.or,  x,Ib),
            Instr(Mnemonic.cp,  x,Ib),

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

            // C0
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.jp, InstrClass.Transfer, x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.jp, InstrClass.ConditionalTransfer, c,x),

            // D0
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.call, x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Mnemonic.call, InstrClass.ConditionalTransfer, c,x),

            // E0
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

            // F0
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
        };
    }
}
