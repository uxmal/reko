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
        public static readonly RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r6 = new RegisterStorage("r6", 6, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r7 = new RegisterStorage("r7", 7, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r8 = new RegisterStorage("r8", 8, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r11 = new RegisterStorage("r11", 11, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r12 = new RegisterStorage("r12", 12, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r13 = new RegisterStorage("r13", 13, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r14 = new RegisterStorage("r14", 14, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r15 = new RegisterStorage("r15", 15, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r16 = new RegisterStorage("r16", 16, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r17 = new RegisterStorage("r17", 17, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r18 = new RegisterStorage("r18", 18, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r19 = new RegisterStorage("r19", 19, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r20 = new RegisterStorage("r20", 20, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r21 = new RegisterStorage("r21", 21, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r22 = new RegisterStorage("r22", 22, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r23 = new RegisterStorage("r23", 23, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r24 = new RegisterStorage("r24", 24, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r25 = new RegisterStorage("r25", 25, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r26 = new RegisterStorage("r26", 26, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r27 = new RegisterStorage("r27", 27, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r28 = new RegisterStorage("r28", 28, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r29 = new RegisterStorage("r29", 29, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r30 = new RegisterStorage("r30", 30, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage zero = new RegisterStorage("zero", 31, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f0 = new RegisterStorage("f0", 32, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f1 = new RegisterStorage("f1", 33, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f2 = new RegisterStorage("f2", 34, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f3 = new RegisterStorage("f3", 35, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f4 = new RegisterStorage("f4", 36, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f5 = new RegisterStorage("f5", 37, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f6 = new RegisterStorage("f6", 38, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f7 = new RegisterStorage("f7", 39, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f8 = new RegisterStorage("f8", 40, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f9 = new RegisterStorage("f9", 41, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f10 = new RegisterStorage("f10", 42, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f11 = new RegisterStorage("f11", 43, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f12 = new RegisterStorage("f12", 44, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f13 = new RegisterStorage("f13", 45, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f14 = new RegisterStorage("f14", 46, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f15 = new RegisterStorage("f15", 47, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f16 = new RegisterStorage("f16", 48, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f17 = new RegisterStorage("f17", 49, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f18 = new RegisterStorage("f18", 50, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f19 = new RegisterStorage("f19", 51, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f20 = new RegisterStorage("f20", 52, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f21 = new RegisterStorage("f21", 53, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f22 = new RegisterStorage("f22", 54, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f23 = new RegisterStorage("f23", 55, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f24 = new RegisterStorage("f24", 56, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f25 = new RegisterStorage("f25", 57, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f26 = new RegisterStorage("f26", 58, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f27 = new RegisterStorage("f27", 59, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage f28 = new RegisterStorage("f28", 60, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f29 = new RegisterStorage("f29", 61, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f30 = new RegisterStorage("f30", 62, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage f31 = new RegisterStorage("f31", 63, 0, PrimitiveType.Word64);


        public static readonly Dictionary<string, RegisterStorage> AllRegisters;

        public static readonly RegisterStorage[] AluRegisters;

        public static readonly RegisterStorage[] FpuRegisters;

        public static readonly Dictionary<StorageDomain, RegisterStorage> ByDomain;

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

        }
    }
}
