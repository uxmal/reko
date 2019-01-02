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
            new OpRec(Opcode.ld, "x,r"),
            new OpRec(Opcode.ld, "x,r"),
            new OpRec(Opcode.ld, "x,r"),
            new OpRec(Opcode.ld, "x,r"),

            new OpRec(Opcode.ld, "x,r"),
            new OpRec(Opcode.ld, "x,r"),
            new OpRec(Opcode.ld, "x,r"),
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
            new OpRec(Opcode.ld, "x,Ib"),

            new OpRec(Opcode.ld, "B,x"),
            new OpRec(Opcode.ld, "D,x"),
            new OpRec(Opcode.ld, "H,x"),
            invalid,

            new OpRec(Opcode.ld, "X,x"),
            new OpRec(Opcode.ld, "Y,x"),
            new OpRec(Opcode.ld, "S,x"),
            new OpRec(Opcode.ldw, "x,Iw"),

            // 40
            new OpRec(Opcode.ld, "x,B"),
            new OpRec(Opcode.ld, "x,D"),
            new OpRec(Opcode.ld, "x,H"),
            invalid,

            invalid,
            invalid,
            invalid,
            invalid,

            new OpRec(Opcode.ld, "x,X"),
            new OpRec(Opcode.ld, "x,Y"),
            new OpRec(Opcode.ld, "x,S"),
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

            new OpRec(Opcode.add, "x,Ib"),
            new OpRec(Opcode.adc, "x,Ib"),
            new OpRec(Opcode.sub, "x,Ib"),
            new OpRec(Opcode.sbc, "x,Ib"),

            new OpRec(Opcode.and, "x,Ib"),
            new OpRec(Opcode.xor, "x,Ib"),
            new OpRec(Opcode.or,  "x,Ib"),
            new OpRec(Opcode.cp,  "x,Ib"),

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
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.jp, "x", InstrClass.Transfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.jp, "c,x", InstrClass.ConditionalTransfer),

            // D0
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.call, "x"),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),

            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),
            new OpRec(Opcode.call, "c,x", InstrClass.ConditionalTransfer),

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
