#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Alpha
{
    public static class Registers
    {
        public static readonly RegisterStorage r0 = RegisterStorage.Reg64("r0", 0);
        public static readonly RegisterStorage r1 = RegisterStorage.Reg64("r1", 1);
        public static readonly RegisterStorage r2 = RegisterStorage.Reg64("r2", 2);
        public static readonly RegisterStorage r3 = RegisterStorage.Reg64("r3", 3);

        public static readonly RegisterStorage r4 = RegisterStorage.Reg64("r4", 4);
        public static readonly RegisterStorage r5 = RegisterStorage.Reg64("r5", 5);
        public static readonly RegisterStorage r6 = RegisterStorage.Reg64("r6", 6);
        public static readonly RegisterStorage r7 = RegisterStorage.Reg64("r7", 7);

        public static readonly RegisterStorage r8 = RegisterStorage.Reg64("r8", 8);
        public static readonly RegisterStorage r9 = RegisterStorage.Reg64("r9", 9);
        public static readonly RegisterStorage r10 = RegisterStorage.Reg64("r10", 10);
        public static readonly RegisterStorage r11 = RegisterStorage.Reg64("r11", 11);

        public static readonly RegisterStorage r12 = RegisterStorage.Reg64("r12", 12);
        public static readonly RegisterStorage r13 = RegisterStorage.Reg64("r13", 13);
        public static readonly RegisterStorage r14 = RegisterStorage.Reg64("r14", 14);
        public static readonly RegisterStorage r15 = RegisterStorage.Reg64("r15", 15);

        public static readonly RegisterStorage r16 = RegisterStorage.Reg64("r16", 16);
        public static readonly RegisterStorage r17 = RegisterStorage.Reg64("r17", 17);
        public static readonly RegisterStorage r18 = RegisterStorage.Reg64("r18", 18);
        public static readonly RegisterStorage r19 = RegisterStorage.Reg64("r19", 19);

        public static readonly RegisterStorage r20 = RegisterStorage.Reg64("r20", 20);
        public static readonly RegisterStorage r21 = RegisterStorage.Reg64("r21", 21);
        public static readonly RegisterStorage r22 = RegisterStorage.Reg64("r22", 22);
        public static readonly RegisterStorage r23 = RegisterStorage.Reg64("r23", 23);

        public static readonly RegisterStorage r24 = RegisterStorage.Reg64("r24", 24);
        public static readonly RegisterStorage r25 = RegisterStorage.Reg64("r25", 25);
        public static readonly RegisterStorage r26 = RegisterStorage.Reg64("r26", 26);
        public static readonly RegisterStorage r27 = RegisterStorage.Reg64("r27", 27);

        public static readonly RegisterStorage r28 = RegisterStorage.Reg64("r28", 28);
        public static readonly RegisterStorage r29 = RegisterStorage.Reg64("r29", 29);
        public static readonly RegisterStorage r30 = RegisterStorage.Reg64("r30", 30);
        public static readonly RegisterStorage zero = RegisterStorage.Reg64("zero", 31);

        public static readonly RegisterStorage f0 = RegisterStorage.Reg64("f0", 32);
        public static readonly RegisterStorage f1 = RegisterStorage.Reg64("f1", 33);
        public static readonly RegisterStorage f2 = RegisterStorage.Reg64("f2", 34);
        public static readonly RegisterStorage f3 = RegisterStorage.Reg64("f3", 35);

        public static readonly RegisterStorage f4 = RegisterStorage.Reg64("f4", 36);
        public static readonly RegisterStorage f5 = RegisterStorage.Reg64("f5", 37);
        public static readonly RegisterStorage f6 = RegisterStorage.Reg64("f6", 38);
        public static readonly RegisterStorage f7 = RegisterStorage.Reg64("f7", 39);

        public static readonly RegisterStorage f8 = RegisterStorage.Reg64("f8", 40);
        public static readonly RegisterStorage f9 = RegisterStorage.Reg64("f9", 41);
        public static readonly RegisterStorage f10 = RegisterStorage.Reg64("f10", 42);
        public static readonly RegisterStorage f11 = RegisterStorage.Reg64("f11", 43);

        public static readonly RegisterStorage f12 = RegisterStorage.Reg64("f12", 44);
        public static readonly RegisterStorage f13 = RegisterStorage.Reg64("f13", 45);
        public static readonly RegisterStorage f14 = RegisterStorage.Reg64("f14", 46);
        public static readonly RegisterStorage f15 = RegisterStorage.Reg64("f15", 47);

        public static readonly RegisterStorage f16 = RegisterStorage.Reg64("f16", 48);
        public static readonly RegisterStorage f17 = RegisterStorage.Reg64("f17", 49);
        public static readonly RegisterStorage f18 = RegisterStorage.Reg64("f18", 50);
        public static readonly RegisterStorage f19 = RegisterStorage.Reg64("f19", 51);

        public static readonly RegisterStorage f20 = RegisterStorage.Reg64("f20", 52);
        public static readonly RegisterStorage f21 = RegisterStorage.Reg64("f21", 53);
        public static readonly RegisterStorage f22 = RegisterStorage.Reg64("f22", 54);
        public static readonly RegisterStorage f23 = RegisterStorage.Reg64("f23", 55);

        public static readonly RegisterStorage f24 = RegisterStorage.Reg64("f24", 56);
        public static readonly RegisterStorage f25 = RegisterStorage.Reg64("f25", 57);
        public static readonly RegisterStorage f26 = RegisterStorage.Reg64("f26", 58);
        public static readonly RegisterStorage f27 = RegisterStorage.Reg64("f27", 59);

        public static readonly RegisterStorage f28 = RegisterStorage.Reg64("f28", 60);
        public static readonly RegisterStorage f29 = RegisterStorage.Reg64("f29", 61);
        public static readonly RegisterStorage f30 = RegisterStorage.Reg64("f30", 62);
        public static readonly RegisterStorage f31 = RegisterStorage.Reg64("f31", 63);


        public static readonly Dictionary<string, RegisterStorage> AllRegisters;

        public static readonly RegisterStorage[] AluRegisters;

        public static readonly RegisterStorage[] FpuRegisters;

        public static readonly Dictionary<StorageDomain, RegisterStorage> ByDomain;
        public static readonly Dictionary<string, RegisterStorage> ByName;

        static Registers()
        {
            AllRegisters = new[]
            {
                r0 , r1 , r2,  r3,  r4,  r5,  r6,  r7,  r8,  r9,  r10, r11, r12, r13, r14, r15,
                r16, r17, r18, r19, r20, r21, r22, r23, r24, r25, r26, r27, r28, r29, r30, zero,
                f0 , f1 , f2,  f3,  f4,  f5,  f6,  f7,  f8,  f9,  f10, f11, f12, f13, f14, f15,
                f16, f17, f18, f19, f20, f21, f22, f23, f24, f25, f26, f27, f28, f29, f30, f31,
            }.ToDictionary(r => r.Name);

            AluRegisters = new[]
            {
                r0 , r1 , r2,  r3,  r4,  r5,  r6,  r7,  r8,  r9,  r10, r11, r12, r13, r14, r15,
                r16, r17, r18, r19, r20, r21, r22, r23, r24, r25, r26, r27, r28, r29, r30, zero,
            };

            FpuRegisters = new[]
            {
                f0 , f1 , f2,  f3,  f4,  f5,  f6,  f7,  f8,  f9,  f10, f11, f12, f13, f14, f15,
                f16, f17, f18, f19, f20, f21, f22, f23, f24, f25, f26, f27, f28, f29, f30, f31,
            };

            ByDomain = AllRegisters.Values.ToDictionary(r => r.Domain);
            ByName = AllRegisters.Values.ToDictionary(r => r.Name);
        }
    }
}
