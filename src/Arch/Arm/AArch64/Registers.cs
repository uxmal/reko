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

        public static readonly Dictionary<uint, RegisterStorage> SystemRegisters;

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
                .Select((r, i) => new RegisterStorage($"w{i}", r.Number, 0, PrimitiveType.Word32))
                .ToArray();
            AddrRegs64 = GpRegs64.ToArray();
            AddrRegs32 = GpRegs32.ToArray();

            // 'v', 'q', 'd', 's', 'h', b' regsters overlap
            SimdRegs128 = stg.RangeOfReg(32, n => $"q{n}", PrimitiveType.Word128);
            SimdRegs64 = SimdRegs128
                .Select((r, i) => new RegisterStorage($"d{i}", r.Number, 0, PrimitiveType.Word64))
                .ToArray();
            SimdRegs32 = SimdRegs128
                .Select((r, i) => new RegisterStorage($"s{i}", r.Number, 0, PrimitiveType.Word32))
                .ToArray();
            SimdRegs16 = SimdRegs128
                .Select((r, i) => new RegisterStorage($"h{i}", r.Number, 0, PrimitiveType.Word16))
                .ToArray();
            SimdRegs8 = SimdRegs128
                .Select((r, i) => new RegisterStorage($"b{i}", r.Number, 0, PrimitiveType.Byte))
                .ToArray();

            SimdVectorReg128 = SimdRegs128
                .Select((r, i) => new RegisterStorage($"v{i}", r.Number, 0, PrimitiveType.Word128))
                .ToArray();

            // The stack register can only be accessed via effective address.
            sp = stg.Reg64("sp");
            wsp = new RegisterStorage("wsp", sp.Number, 0, PrimitiveType.Word32);
            AddrRegs64[31] = sp;
            AddrRegs32[31] = wsp;

            pstate = stg.Reg32("pstate");
            fpcr = stg.Reg32("fpcr");
            fpsr = stg.Reg32("fpsr");

            var sys = new StorageFactory(StorageDomain.SystemRegister);
            SystemRegisters = new[] {
                (0b11_000_0001_0000_001u, sys.Reg64("ACTLR_EL1")),
                (0b11_000_1101_0000_100u, sys.Reg64("TPIDR_EL1")),
                (0b11_011_1110_0000_000u, sys.Reg32("CNTFRQ_EL0")),
                (0b11_000_1110_0001_000u, sys.Reg32("CNTKCTL_EL1")),
                (0b11_101_1110_0001_000u, sys.Reg32("CNTKCTL_EL12")),
                (0b11_011_1110_0010_001u, sys.Reg32("CNTP_CTL_EL0")),
                (0b11_101_1110_0010_001u, sys.Reg32("CNTP_CTL_EL02")),
                (0b11_011_1110_0010_010u, sys.Reg64("CNTP_CVAL_EL0")),
                (0b11_101_1110_0010_010u, sys.Reg64("CNTP_CVAL_EL02")),
                (0b11_011_1110_0010_000u, sys.Reg32("CNTP_TVAL_EL0")),
                (0b11_101_1110_0010_000u, sys.Reg32("CNTP_TVAL_EL02")),
                (0b11_011_1110_0000_001u, sys.Reg64("CNTPCT_EL0")),
                (0b11_011_1110_0011_001u, sys.Reg32("CNTV_CTL_EL0")),
                (0b11_101_1110_0011_001u, sys.Reg32("CNTV_CTL_EL02")),
                (0b11_011_1110_0011_010u, sys.Reg64("CNTV_CVAL_EL0")),
                (0b11_101_1110_0011_010u, sys.Reg64("CNTV_CVAL_EL02")),
                (0b11_011_1110_0011_000u, sys.Reg32("CNTV_TVAL_EL0")),
                (0b11_101_1110_0011_000u, sys.Reg32("CNTV_TVAL_EL02")),
                (0b11_011_1110_0000_010u, sys.Reg64("CNTVCT_EL0")),

                (0b11_100_1110_0000_011u, sys.Reg64("CNTVOFF_EL2")),
                (0b11_011_0000_0000_001u, sys.Reg32("CTR_EL0")),
                (0b11_000_0100_0010_010u, sys.Reg32("CurrentEL")),
                (0b11_011_0100_0010_001u, sys.Reg32("DAIF")),

                (0b11_011_0000_0000_111u, sys.Reg32("DCZID_EL0")),
                (0b11_000_0001_0000_000u, sys.Reg32("SCTLR_EL1")),
                (0b11_101_0001_0000_000u, sys.Reg32("SCTLR_EL12")),
                (0b11_100_0001_0000_000u, sys.Reg32("SCTLR_EL2")),
                (0b11_110_0001_0000_000u, sys.Reg32("SCTLR_EL3")),
                (0b11_000_0010_0000_010u, sys.Reg64("TCR_EL1")),
                (0b11_101_0010_0000_010u, sys.Reg64("TCR_EL12")),
                (0b11_100_0010_0000_010u, sys.Reg64("TCR_EL2")),
                (0b11_110_0010_0000_010u, sys.Reg32("TCR_EL3")),
                (0b11_000_0010_0000_000u, sys.Reg64("TTBR0_EL1")),
                (0b11_101_0010_0000_000u, sys.Reg64("TTBR0_EL12")),
                (0b11_100_0010_0000_000u, sys.Reg64("TTBR0_EL2")),
            }.ToDictionary(sr => sr.Item1, sr => sr.Item2);

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
