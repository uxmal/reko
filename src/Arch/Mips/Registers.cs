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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Mips
{
    public static class Registers
    {
        public readonly static RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r6 = new RegisterStorage("r6", 6, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r7 = new RegisterStorage("r7", 7, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r8 = new RegisterStorage("r8", 8, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r11 = new RegisterStorage("r11", 11, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r12 = new RegisterStorage("r12", 12, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r13 = new RegisterStorage("r13", 13, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r14 = new RegisterStorage("r14", 14, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r15 = new RegisterStorage("r15", 15, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r16 = new RegisterStorage("r16", 16, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r17 = new RegisterStorage("r17", 17, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r18 = new RegisterStorage("r18", 18, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r19 = new RegisterStorage("r19", 19, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r20 = new RegisterStorage("r20", 20, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r21 = new RegisterStorage("r21", 21, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r22 = new RegisterStorage("r22", 22, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r23 = new RegisterStorage("r23", 23, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r24 = new RegisterStorage("r24", 24, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r25 = new RegisterStorage("r25", 25, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r26 = new RegisterStorage("r26", 26, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r27 = new RegisterStorage("r27", 27, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r28 = new RegisterStorage("r28", 28, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage sp = new RegisterStorage("sp", 29, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage r30 = new RegisterStorage("r30", 30, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage ra = new RegisterStorage("ra", 31, 0, PrimitiveType.Word32);

        public readonly static RegisterStorage hi = new RegisterStorage("hi", 32, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage lo = new RegisterStorage("lo", 33, 0, PrimitiveType.Word32);

        public readonly static RegisterStorage f0 = new RegisterStorage("f0", 64, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f1 = new RegisterStorage("f1", 65, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f2 = new RegisterStorage("f2", 66, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f3 = new RegisterStorage("f3", 67, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f4 = new RegisterStorage("f4", 68, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f5 = new RegisterStorage("f5", 69, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f6 = new RegisterStorage("f6", 70, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f7 = new RegisterStorage("f7", 71, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f8 = new RegisterStorage("f8", 72, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f9 = new RegisterStorage("f9", 73, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f10 = new RegisterStorage("f10", 74, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f11 = new RegisterStorage("f11", 75, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f12 = new RegisterStorage("f12", 76, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f13 = new RegisterStorage("f13", 77, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f14 = new RegisterStorage("f14", 78, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f15 = new RegisterStorage("f15", 79, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f16 = new RegisterStorage("f16", 80, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f17 = new RegisterStorage("f17", 81, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f18 = new RegisterStorage("f18", 82, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f19 = new RegisterStorage("f19", 83, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f20 = new RegisterStorage("f20", 84, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f21 = new RegisterStorage("f21", 85, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f22 = new RegisterStorage("f22", 86, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f23 = new RegisterStorage("f23", 87, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f24 = new RegisterStorage("f24", 88, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f25 = new RegisterStorage("f25", 89, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f26 = new RegisterStorage("f26", 90, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f27 = new RegisterStorage("f27", 91, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f28 = new RegisterStorage("f28", 92, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f29 = new RegisterStorage("f29", 93, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f30 = new RegisterStorage("f30", 94, 0, PrimitiveType.Word32);
        public readonly static RegisterStorage f31 = new RegisterStorage("f31", 95, 0, PrimitiveType.Word32);

        public readonly static RegisterStorage FCSR = new RegisterStorage("FCSR", 0x201F, 0, PrimitiveType.Word32);

        public readonly static RegisterStorage cc0 = new RegisterStorage("cc0", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc1 = new RegisterStorage("cc1", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc2 = new RegisterStorage("cc2", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc3 = new RegisterStorage("cc3", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc4 = new RegisterStorage("cc4", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc5 = new RegisterStorage("cc5", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc6 = new RegisterStorage("cc6", 0x3000, 0, PrimitiveType.Bool);
        public readonly static RegisterStorage cc7 = new RegisterStorage("cc7", 0x3000, 0, PrimitiveType.Bool);

        internal readonly static RegisterStorage[] generalRegs;
        internal readonly static RegisterStorage[] fpuRegs;
        internal readonly static Dictionary<uint, RegisterStorage> fpuCtrlRegs;
        internal readonly static RegisterStorage[] ccRegs;

        internal static readonly Dictionary<string, RegisterStorage> mpNameToReg;

        static Registers()
        {
            generalRegs = new RegisterStorage[]
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
                sp,
                r30,
                ra,

                hi,
                lo,
            };

            fpuRegs = new[]
            {
                f0 ,
                f1 ,
                f2 ,
                f3 ,
                f4 ,
                f5 ,
                f6 ,
                f7 ,
                f8 ,
                f9 ,
                f10,
                f11,
                f12,
                f13,
                f14,
                f15,
                f16,
                f17,
                f18,
                f19,
                f20,
                f21,
                f22,
                f23,
                f24,
                f25,
                f26,
                f27,
                f28,
                f29,
                f30,
                f31,
            };

            fpuCtrlRegs = new Dictionary<uint, RegisterStorage>
            {
                { 0x1F, FCSR }
            };

            ccRegs = new[]
            {
                cc0,cc1,cc2,cc3, cc4,cc5,cc6,cc7,
            };

            mpNameToReg = generalRegs.Concat(fpuRegs).ToDictionary(r => r.Name);
        }
    }
}
