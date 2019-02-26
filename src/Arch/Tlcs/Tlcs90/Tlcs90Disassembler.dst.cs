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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Tlcs.Tlcs90
{
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
            Instr(Opcode.ld, x,r),
            Instr(Opcode.ld, x,r),
            Instr(Opcode.ld, x,r),
            Instr(Opcode.ld, x,r),

            Instr(Opcode.ld, x,r),
            Instr(Opcode.ld, x,r),
            Instr(Opcode.ld, x,r),
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
            Instr(Opcode.ld, x,Ib),

            Instr(Opcode.ld, B,x),
            Instr(Opcode.ld, D,x),
            Instr(Opcode.ld, H,x),
            invalid,

            Instr(Opcode.ld, X,x),
            Instr(Opcode.ld, Y,x),
            Instr(Opcode.ld, S,x),
            Instr(Opcode.ldw, x,Iw),

            // 40
            Instr(Opcode.ld, x,B),
            Instr(Opcode.ld, x,D),
            Instr(Opcode.ld, x,H),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Opcode.ld, x,X),
            Instr(Opcode.ld, x,Y),
            Instr(Opcode.ld, x,S),
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

            Instr(Opcode.add, x,Ib),
            Instr(Opcode.adc, x,Ib),
            Instr(Opcode.sub, x,Ib),
            Instr(Opcode.sbc, x,Ib),

            Instr(Opcode.and, x,Ib),
            Instr(Opcode.xor, x,Ib),
            Instr(Opcode.or,  x,Ib),
            Instr(Opcode.cp,  x,Ib),

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
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.jp, InstrClass.Transfer, x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.jp, InstrClass.ConditionalTransfer, c,x),

            // D0
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.call, x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),

            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),
            Instr(Opcode.call, InstrClass.ConditionalTransfer, c,x),

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
