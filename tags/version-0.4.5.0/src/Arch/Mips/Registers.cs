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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Arch.Mips
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
        };
    }
}
