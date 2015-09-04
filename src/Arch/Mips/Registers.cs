#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Mips
{
    public static class Registers
    {
        public readonly static RegisterStorage r0 = new RegisterStorage("r0", 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r1 = new RegisterStorage("r1", 1, PrimitiveType.Word32);
        public readonly static RegisterStorage r2 = new RegisterStorage("r2", 2, PrimitiveType.Word32);
        public readonly static RegisterStorage r3 = new RegisterStorage("r3", 3, PrimitiveType.Word32);
        public readonly static RegisterStorage r4 = new RegisterStorage("r4", 4, PrimitiveType.Word32);
        public readonly static RegisterStorage r5 = new RegisterStorage("r5", 5, PrimitiveType.Word32);
        public readonly static RegisterStorage r6 = new RegisterStorage("r6", 6, PrimitiveType.Word32);
        public readonly static RegisterStorage r7 = new RegisterStorage("r7", 7, PrimitiveType.Word32);
        public readonly static RegisterStorage r8 = new RegisterStorage("r8", 8, PrimitiveType.Word32);
        public readonly static RegisterStorage r9 = new RegisterStorage("r9", 9, PrimitiveType.Word32);
        public readonly static RegisterStorage r10 = new RegisterStorage("r10", 10, PrimitiveType.Word32);
        public readonly static RegisterStorage r11 = new RegisterStorage("r11", 11, PrimitiveType.Word32);
        public readonly static RegisterStorage r12 = new RegisterStorage("r12", 12, PrimitiveType.Word32);
        public readonly static RegisterStorage r13 = new RegisterStorage("r13", 13, PrimitiveType.Word32);
        public readonly static RegisterStorage r14 = new RegisterStorage("r14", 14, PrimitiveType.Word32);
        public readonly static RegisterStorage r15 = new RegisterStorage("r15", 15, PrimitiveType.Word32);
        public readonly static RegisterStorage r16 = new RegisterStorage("r16", 16, PrimitiveType.Word32);
        public readonly static RegisterStorage r17 = new RegisterStorage("r17", 17, PrimitiveType.Word32);
        public readonly static RegisterStorage r18 = new RegisterStorage("r18", 18, PrimitiveType.Word32);
        public readonly static RegisterStorage r19 = new RegisterStorage("r19", 19, PrimitiveType.Word32);
        public readonly static RegisterStorage r20 = new RegisterStorage("r20", 20, PrimitiveType.Word32);
        public readonly static RegisterStorage r21 = new RegisterStorage("r21", 21, PrimitiveType.Word32);
        public readonly static RegisterStorage r22 = new RegisterStorage("r22", 22, PrimitiveType.Word32);
        public readonly static RegisterStorage r23 = new RegisterStorage("r23", 23, PrimitiveType.Word32);
        public readonly static RegisterStorage r24 = new RegisterStorage("r24", 24, PrimitiveType.Word32);
        public readonly static RegisterStorage r25 = new RegisterStorage("r25", 25, PrimitiveType.Word32);
        public readonly static RegisterStorage r26 = new RegisterStorage("r26", 26, PrimitiveType.Word32);
        public readonly static RegisterStorage r27 = new RegisterStorage("r27", 27, PrimitiveType.Word32);
        public readonly static RegisterStorage r28 = new RegisterStorage("r28", 28, PrimitiveType.Word32);
        public readonly static RegisterStorage r29 = new RegisterStorage("r29", 29, PrimitiveType.Word32);
        public readonly static RegisterStorage r30 = new RegisterStorage("r30", 30, PrimitiveType.Word32);
        public readonly static RegisterStorage r31 = new RegisterStorage("r31", 31, PrimitiveType.Word32);

        public readonly static RegisterStorage hi = new RegisterStorage("hi", 32, PrimitiveType.Word32);
        public readonly static RegisterStorage lo = new RegisterStorage("lo", 33, PrimitiveType.Word32);

        public readonly static RegisterStorage f0 = new RegisterStorage("f0", 64, PrimitiveType.Word32);
        public readonly static RegisterStorage f1 = new RegisterStorage("f1", 65, PrimitiveType.Word32);
        public readonly static RegisterStorage f2 = new RegisterStorage("f2", 66, PrimitiveType.Word32);
        public readonly static RegisterStorage f3 = new RegisterStorage("f3", 67, PrimitiveType.Word32);
        public readonly static RegisterStorage f4 = new RegisterStorage("f4", 68, PrimitiveType.Word32);
        public readonly static RegisterStorage f5 = new RegisterStorage("f5", 69, PrimitiveType.Word32);
        public readonly static RegisterStorage f6 = new RegisterStorage("f6", 70, PrimitiveType.Word32);
        public readonly static RegisterStorage f7 = new RegisterStorage("f7", 71, PrimitiveType.Word32);
        public readonly static RegisterStorage f8 = new RegisterStorage("f8", 72, PrimitiveType.Word32);
        public readonly static RegisterStorage f9 = new RegisterStorage("f9", 73, PrimitiveType.Word32);
        public readonly static RegisterStorage f10 = new RegisterStorage("f10", 74, PrimitiveType.Word32);
        public readonly static RegisterStorage f11 = new RegisterStorage("f11", 75, PrimitiveType.Word32);
        public readonly static RegisterStorage f12 = new RegisterStorage("f12", 76, PrimitiveType.Word32);
        public readonly static RegisterStorage f13 = new RegisterStorage("f13", 77, PrimitiveType.Word32);
        public readonly static RegisterStorage f14 = new RegisterStorage("f14", 78, PrimitiveType.Word32);
        public readonly static RegisterStorage f15 = new RegisterStorage("f15", 79, PrimitiveType.Word32);
        public readonly static RegisterStorage f16 = new RegisterStorage("f16", 80, PrimitiveType.Word32);
        public readonly static RegisterStorage f17 = new RegisterStorage("f17", 81, PrimitiveType.Word32);
        public readonly static RegisterStorage f18 = new RegisterStorage("f18", 82, PrimitiveType.Word32);
        public readonly static RegisterStorage f19 = new RegisterStorage("f19", 83, PrimitiveType.Word32);
        public readonly static RegisterStorage f20 = new RegisterStorage("f20", 84, PrimitiveType.Word32);
        public readonly static RegisterStorage f21 = new RegisterStorage("f21", 85, PrimitiveType.Word32);
        public readonly static RegisterStorage f22 = new RegisterStorage("f22", 86, PrimitiveType.Word32);
        public readonly static RegisterStorage f23 = new RegisterStorage("f23", 87, PrimitiveType.Word32);
        public readonly static RegisterStorage f24 = new RegisterStorage("f24", 88, PrimitiveType.Word32);
        public readonly static RegisterStorage f25 = new RegisterStorage("f25", 89, PrimitiveType.Word32);
        public readonly static RegisterStorage f26 = new RegisterStorage("f26", 90, PrimitiveType.Word32);
        public readonly static RegisterStorage f27 = new RegisterStorage("f27", 91, PrimitiveType.Word32);
        public readonly static RegisterStorage f28 = new RegisterStorage("f28", 92, PrimitiveType.Word32);
        public readonly static RegisterStorage f29 = new RegisterStorage("f29", 93, PrimitiveType.Word32);
        public readonly static RegisterStorage f30 = new RegisterStorage("f30", 94, PrimitiveType.Word32);
        public readonly static RegisterStorage f31 = new RegisterStorage("f31", 95, PrimitiveType.Word32);

        internal readonly static RegisterStorage[] generalRegs = new RegisterStorage[]
        {
            r0, 
            r1, 
            r2, 
            r3, 
            r4, 
            r5, 
            r6, 
            r7, 
            r8, 
            r9, 
            r10,
            r11,
            r12,
            r13,
            r14,
            r15,
            r16,
            r17,
            r18,
            r19,
            r20,
            r21,
            r22,
            r23,
            r24,
            r25,
            r26,
            r27,
            r28,
            r29,
            r30,
            r31,

            hi,
            lo,
        };
    }
}
