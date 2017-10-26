#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        public static readonly RegisterStorage r1 = new RegisterStorage("r1", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r2 = new RegisterStorage("r2", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r3 = new RegisterStorage("r3", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r4 = new RegisterStorage("r4", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r5 = new RegisterStorage("r5", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r6 = new RegisterStorage("r6", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r7 = new RegisterStorage("r7", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r8 = new RegisterStorage("r8", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r9 = new RegisterStorage("r9", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r10 = new RegisterStorage("r10", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r11 = new RegisterStorage("r11", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r12 = new RegisterStorage("r12", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r13 = new RegisterStorage("r13", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r14 = new RegisterStorage("r14", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r15 = new RegisterStorage("r15", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r16 = new RegisterStorage("r16", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r17 = new RegisterStorage("r17", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r18 = new RegisterStorage("r18", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r19 = new RegisterStorage("r19", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r20 = new RegisterStorage("r20", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r21 = new RegisterStorage("r21", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r22 = new RegisterStorage("r22", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r23 = new RegisterStorage("r23", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r24 = new RegisterStorage("r24", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r25 = new RegisterStorage("r25", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r26 = new RegisterStorage("r26", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r27 = new RegisterStorage("r27", 0, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage r28 = new RegisterStorage("r28", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r29 = new RegisterStorage("r29", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage r30 = new RegisterStorage("r30", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage zero = new RegisterStorage("zero", 31, 0, PrimitiveType.Word64);

        public static readonly Dictionary<string, RegisterStorage> AllRegisters;

        public static readonly RegisterStorage[] AluRegisters;

        static Registers()
        {
            AllRegisters = new[]
            {
                r0 , r1 , r2,  r3,  r4,  r5,  r6,  r7,  r8,  r9,  r10, r11, r12, r13, r14, r15,
                r16, r17, r18, r19, r20, r21, r22, r23, r24, r25, r26, r27, r28, r29, r30, zero,
            }.ToDictionary(r => r.Name);

            AluRegisters = new[]
            {
                r0 , r1 , r2,  r3,  r4,  r5,  r6,  r7,  r8,  r9,  r10, r11, r12, r13, r14, r15,
                r16, r17, r18, r19, r20, r21, r22, r23, r24, r25, r26, r27, r28, r29, r30, zero,
            };
        }
    }
}
