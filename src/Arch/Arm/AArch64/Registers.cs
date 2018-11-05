#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Arm.AArch64
{

    public static class Registers
    {
        public static readonly RegisterStorage[] GpRegs64;
        public static readonly RegisterStorage[] GpRegs32;
        public static readonly RegisterStorage[] AddrRegs64;
        public static readonly RegisterStorage[] AddrRegs32;

        public static readonly RegisterStorage[] SimdVectorReg128;
        public static readonly RegisterStorage[] SimdRegs128;
        public static readonly RegisterStorage[] SimdRegs64;
        public static readonly RegisterStorage[] SimdRegs32;

        public static readonly RegisterStorage[] SimdRegs16;
        public static readonly RegisterStorage[] SimdRegs8;

        public static readonly RegisterStorage sp;
        public static readonly RegisterStorage wsp;
        public static readonly RegisterStorage pstate;
        public static readonly RegisterStorage fpcr;
        public static readonly RegisterStorage fpsr;

        public static readonly Dictionary<string, RegisterStorage> ByName;

        internal static bool IsIntegerRegister(RegisterStorage reg)
        {
            var prefix = reg.Name[0];
            return prefix == 'w' || prefix == 'x';
        }

        static Registers()
        {
            GpRegs64 = Enumerable.Range(0, 32)
                .Select(n => new RegisterStorage($"x{n}", n, 0, PrimitiveType.Word64))
                .ToArray();
            GpRegs32 = Enumerable.Range(0, 32)
                .Select(n => new RegisterStorage($"w{n}", n, 0, PrimitiveType.Word32))
                .ToArray();
            AddrRegs64 = GpRegs64.ToArray();
            AddrRegs32 = GpRegs32.ToArray();

            SimdRegs128 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"q{n-32}", n, 0, PrimitiveType.Word128))
                .ToArray();
            SimdRegs64 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"d{n-32}", n, 0, PrimitiveType.Word64))
                .ToArray();
            SimdRegs32 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"s{n-32}", n, 0, PrimitiveType.Word32))
                .ToArray();
            SimdRegs16 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"h{n-32}", n, 0, PrimitiveType.Word16))
                .ToArray();
            SimdRegs8 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"b{n-32}", n, 0, PrimitiveType.Byte))
                .ToArray();

            SimdVectorReg128 = Enumerable.Range(32, 32)
                .Select(n => new RegisterStorage($"v{n-32}", n, 0, PrimitiveType.Word128))
                .ToArray();

            sp = new RegisterStorage("sp", 64, 0, PrimitiveType.Word64);
            wsp = new RegisterStorage("wsp", 64, 0, PrimitiveType.Word32);
            AddrRegs64[31] = sp;
            AddrRegs32[31] = wsp;

            pstate = new RegisterStorage("pstate", 65, 0, PrimitiveType.Word32);
            fpcr = new RegisterStorage("fpcr", 65, 0, PrimitiveType.Word32);
            fpsr = new RegisterStorage("fpsr", 66, 0, PrimitiveType.Word32);

            ByName = GpRegs64
                .Concat(GpRegs32)
                .Concat(SimdRegs128)
                .Concat(SimdRegs64)
                .Concat(SimdRegs32)
                .Concat(SimdRegs16)
                .Concat(SimdRegs8)
                .Concat(new[]
                {
                    sp,
                    wsp,
                    pstate,
                    fpcr,
                    fpsr,
                })
                .ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
