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
        public static readonly RegisterStorage[][] SubRegisters;

        internal static bool IsIntegerRegister(RegisterStorage reg)
        {
            var prefix = reg.Name[0];
            return prefix == 'w' || prefix == 'x';
        }

        static Registers()
        {
            var stg = new StorageFactory();

            // 'x' and 'w' registers share the same storage.
            GpRegs64 = stg.RangeOfReg64(32, "x{0}");
            GpRegs32 = GpRegs64
                .Select(r => new RegisterStorage($"w{r.Number}", r.Number, 0, PrimitiveType.Word32))
                .ToArray();
            AddrRegs64 = GpRegs64.ToArray();
            AddrRegs32 = GpRegs32.ToArray();

            // 'v', 'q', 'd', 's', 'h', b' regsters overlap
            SimdRegs128 = stg.RangeOfReg(32, n => $"q{n}", PrimitiveType.Word128);
            SimdRegs64 = SimdRegs128
                .Select(r => new RegisterStorage($"d{r.Number}", r.Number, 0, PrimitiveType.Word64))
                .ToArray();
            SimdRegs32 = SimdRegs128
                .Select(r => new RegisterStorage($"s{r.Number}", r.Number, 0, PrimitiveType.Word32))
                .ToArray();
            SimdRegs16 = SimdRegs128
                .Select(r => new RegisterStorage($"h{r.Number}", r.Number, 0, PrimitiveType.Word16))
                .ToArray();
            SimdRegs8 = SimdRegs128
                .Select(r => new RegisterStorage($"b{r.Number}", r.Number, 0, PrimitiveType.Byte))
                .ToArray();

            SimdVectorReg128 = SimdRegs128
                .Select(r => new RegisterStorage($"v{r.Number}", r.Number, 0, PrimitiveType.Word128))
                .ToArray();

            // The stack register can only be accessed via effective address.
            sp = stg.Reg64("sp");
            wsp = new RegisterStorage("wsp", sp.Number, 0, PrimitiveType.Word32);
            AddrRegs64[31] = sp;
            AddrRegs32[31] = wsp;

            pstate = stg.Reg32("pstate");
            fpcr = stg.Reg32("fpcr");
            fpsr = stg.Reg32("fpsr");

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

            SubRegisters = 
                Enumerable.Range(0, 32)
                    .Select(i => new[] { GpRegs64[i], GpRegs32[i] })
                .Concat(Enumerable.Range(0, 32)
                    .Select(i => new[] { SimdRegs128[i], SimdRegs64[i], SimdRegs32[i], SimdRegs16[i], SimdRegs8[i] }))
                .Concat(new RegisterStorage[][]
                {
                    new [] { sp, wsp },
                    new [] { pstate },
                    new [] { fpcr },
                    new [] { fpsr },
                }).ToArray();
        }
    }
}
