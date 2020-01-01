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
        public readonly static RegisterStorage r0 = RegisterStorage.Reg32("r0", 0);
        public readonly static RegisterStorage r1 = RegisterStorage.Reg32("r1", 1);
        public readonly static RegisterStorage r2 = RegisterStorage.Reg32("r2", 2);
        public readonly static RegisterStorage r3 = RegisterStorage.Reg32("r3", 3);
        public readonly static RegisterStorage r4 = RegisterStorage.Reg32("r4", 4);
        public readonly static RegisterStorage r5 = RegisterStorage.Reg32("r5", 5);
        public readonly static RegisterStorage r6 = RegisterStorage.Reg32("r6", 6);
        public readonly static RegisterStorage r7 = RegisterStorage.Reg32("r7", 7);
        public readonly static RegisterStorage r8 = RegisterStorage.Reg32("r8", 8);
        public readonly static RegisterStorage r9 = RegisterStorage.Reg32("r9", 9);
        public readonly static RegisterStorage r10 = RegisterStorage.Reg32("r10", 10);
        public readonly static RegisterStorage r11 = RegisterStorage.Reg32("r11", 11);
        public readonly static RegisterStorage r12 = RegisterStorage.Reg32("r12", 12);
        public readonly static RegisterStorage r13 = RegisterStorage.Reg32("r13", 13);
        public readonly static RegisterStorage r14 = RegisterStorage.Reg32("r14", 14);
        public readonly static RegisterStorage r15 = RegisterStorage.Reg32("r15", 15);
        public readonly static RegisterStorage r16 = RegisterStorage.Reg32("r16", 16);
        public readonly static RegisterStorage r17 = RegisterStorage.Reg32("r17", 17);
        public readonly static RegisterStorage r18 = RegisterStorage.Reg32("r18", 18);
        public readonly static RegisterStorage r19 = RegisterStorage.Reg32("r19", 19);
        public readonly static RegisterStorage r20 = RegisterStorage.Reg32("r20", 20);
        public readonly static RegisterStorage r21 = RegisterStorage.Reg32("r21", 21);
        public readonly static RegisterStorage r22 = RegisterStorage.Reg32("r22", 22);
        public readonly static RegisterStorage r23 = RegisterStorage.Reg32("r23", 23);
        public readonly static RegisterStorage r24 = RegisterStorage.Reg32("r24", 24);
        public readonly static RegisterStorage r25 = RegisterStorage.Reg32("r25", 25);
        public readonly static RegisterStorage r26 = RegisterStorage.Reg32("r26", 26);
        public readonly static RegisterStorage r27 = RegisterStorage.Reg32("r27", 27);
        public readonly static RegisterStorage r28 = RegisterStorage.Reg32("r28", 28);
        public readonly static RegisterStorage sp =  RegisterStorage.Reg32("sp", 29);
        public readonly static RegisterStorage r30 = RegisterStorage.Reg32("r30", 30);
        public readonly static RegisterStorage ra = RegisterStorage.Reg32("ra", 31);

        public readonly static RegisterStorage hi = RegisterStorage.Reg32("hi", 32);
        public readonly static RegisterStorage lo = RegisterStorage.Reg32("lo", 33);

        public readonly static RegisterStorage f0 = RegisterStorage.Reg32("f0", 64);
        public readonly static RegisterStorage f1 = RegisterStorage.Reg32("f1", 65);
        public readonly static RegisterStorage f2 = RegisterStorage.Reg32("f2", 66);
        public readonly static RegisterStorage f3 = RegisterStorage.Reg32("f3", 67);
        public readonly static RegisterStorage f4 = RegisterStorage.Reg32("f4", 68);
        public readonly static RegisterStorage f5 = RegisterStorage.Reg32("f5", 69);
        public readonly static RegisterStorage f6 = RegisterStorage.Reg32("f6", 70);
        public readonly static RegisterStorage f7 = RegisterStorage.Reg32("f7", 71);
        public readonly static RegisterStorage f8 = RegisterStorage.Reg32("f8", 72);
        public readonly static RegisterStorage f9 = RegisterStorage.Reg32("f9", 73);
        public readonly static RegisterStorage f10 = RegisterStorage.Reg32("f10", 74);
        public readonly static RegisterStorage f11 = RegisterStorage.Reg32("f11", 75);
        public readonly static RegisterStorage f12 = RegisterStorage.Reg32("f12", 76);
        public readonly static RegisterStorage f13 = RegisterStorage.Reg32("f13", 77);
        public readonly static RegisterStorage f14 = RegisterStorage.Reg32("f14", 78);
        public readonly static RegisterStorage f15 = RegisterStorage.Reg32("f15", 79);
        public readonly static RegisterStorage f16 = RegisterStorage.Reg32("f16", 80);
        public readonly static RegisterStorage f17 = RegisterStorage.Reg32("f17", 81);
        public readonly static RegisterStorage f18 = RegisterStorage.Reg32("f18", 82);
        public readonly static RegisterStorage f19 = RegisterStorage.Reg32("f19", 83);
        public readonly static RegisterStorage f20 = RegisterStorage.Reg32("f20", 84);
        public readonly static RegisterStorage f21 = RegisterStorage.Reg32("f21", 85);
        public readonly static RegisterStorage f22 = RegisterStorage.Reg32("f22", 86);
        public readonly static RegisterStorage f23 = RegisterStorage.Reg32("f23", 87);
        public readonly static RegisterStorage f24 = RegisterStorage.Reg32("f24", 88);
        public readonly static RegisterStorage f25 = RegisterStorage.Reg32("f25", 89);
        public readonly static RegisterStorage f26 = RegisterStorage.Reg32("f26", 90);
        public readonly static RegisterStorage f27 = RegisterStorage.Reg32("f27", 91);
        public readonly static RegisterStorage f28 = RegisterStorage.Reg32("f28", 92);
        public readonly static RegisterStorage f29 = RegisterStorage.Reg32("f29", 93);
        public readonly static RegisterStorage f30 = RegisterStorage.Reg32("f30", 94);
        public readonly static RegisterStorage f31 = RegisterStorage.Reg32("f31", 95);

        public readonly static RegisterStorage FCSR = RegisterStorage.Reg32("FCSR", 0x201F);

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
