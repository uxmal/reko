#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.Zilog.Z80
{
    public static class Registers
    {
        public static readonly RegisterStorage b = RegisterStorage.Reg8("b", 1, 8);
        public static readonly RegisterStorage c = RegisterStorage.Reg8("c", 1);
        public static readonly RegisterStorage d = RegisterStorage.Reg8("d", 2, 8);
        public static readonly RegisterStorage e = RegisterStorage.Reg8("e", 2);
        public static readonly RegisterStorage h = RegisterStorage.Reg8("h", 3, 8);
        public static readonly RegisterStorage l = RegisterStorage.Reg8("l", 3);
        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0, 8);

        public static readonly RegisterStorage bc = RegisterStorage.Reg16("bc", 1);
        public static readonly RegisterStorage de = RegisterStorage.Reg16("de", 2);
        public static readonly RegisterStorage hl = RegisterStorage.Reg16("hl", 3);
        public static readonly RegisterStorage sp = RegisterStorage.Reg16("sp", 4);
        public static readonly RegisterStorage ix = RegisterStorage.Reg16("ix", 5);
        public static readonly RegisterStorage iy = RegisterStorage.Reg16("iy", 6);
        public static readonly RegisterStorage af = RegisterStorage.Reg16("af", 0);

        public static readonly RegisterStorage f = RegisterStorage.Reg8("f", 0);

        public static readonly RegisterStorage i = RegisterStorage.Reg8("i", 8);
        public static readonly RegisterStorage r = RegisterStorage.Reg8("r", 9);

        public static readonly RegisterStorage bc_ = RegisterStorage.Reg16("bc'", 12);
        public static readonly RegisterStorage de_ = RegisterStorage.Reg16("de'", 13);
        public static readonly RegisterStorage hl_ = RegisterStorage.Reg16("hl'", 14);
        public static readonly RegisterStorage af_ = RegisterStorage.Reg16("af'", 15);

        public static readonly FlagGroupStorage S = new FlagGroupStorage(f, (uint) FlagM.SF, nameof(S));
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(f, (uint) FlagM.ZF, nameof(Z));
        public static readonly FlagGroupStorage P = new FlagGroupStorage(f, (uint) FlagM.PF, nameof(P));
        public static readonly FlagGroupStorage N = new FlagGroupStorage(f, (uint) FlagM.NF, nameof(N));
        public static readonly FlagGroupStorage C = new FlagGroupStorage(f, (uint) FlagM.CF, nameof(C));
        public static readonly FlagGroupStorage SZ = new FlagGroupStorage(f, (uint) (FlagM.SF | FlagM.ZF), nameof(SZ));
        public static readonly FlagGroupStorage SZP = new FlagGroupStorage(f, (uint) (FlagM.ZF | FlagM.SF | FlagM.PF), nameof(SZP));
        public static readonly FlagGroupStorage SZPC = new FlagGroupStorage(f, (uint) (FlagM.CF | FlagM.ZF | FlagM.SF | FlagM.PF), nameof(SZPC));

        internal static RegisterStorage?[] All;
        internal static FlagGroupStorage[] Flags = new[] { S, Z, P, N, C };
        internal static Dictionary<StorageDomain, RegisterStorage[]> SubRegisters;
        internal static Dictionary<string, RegisterStorage> regsByName;
        private readonly static RegisterStorage?[] regsByStorage;

        static Registers()
        {
            All = new RegisterStorage?[] {
                b ,
                c ,
                d ,
                e ,

                h ,
                l ,
                a ,
                f,

                bc,
                de,
                hl,
                sp,
                ix,
                iy,
                af,
                null,

                i ,
                r ,
                null,
                null,

                bc_,
                de_,
                hl_,
                null,
            };

            Registers.regsByName = All.Where(reg => reg is not null).ToDictionary(reg => reg!.Name, reg => reg!);
            regsByStorage = new[]
            {
                af, bc,  de, hl, sp, ix, iy, null,
                i,  r,   null, null, bc_, de_, hl_, af_,
            };

            SubRegisters = new Dictionary<StorageDomain, RegisterStorage[]>
            {
                {
                    af.Domain, new [] { Registers.a, Registers.f }
                },
                {
                    bc.Domain, new [] { Registers.c, Registers.b }
                },
                {
                    de.Domain, new []  { Registers.e, Registers.d }
                },
                {
                    hl.Domain, new[] { Registers.l, Registers.h }
                },
            };
        }

        internal static RegisterStorage? GetRegister(int r)
        {
            return All[r];
        }

        internal static RegisterStorage? GetRegister(string name)
        {
            return regsByName.TryGetValue(name, out var reg) ? reg : null;
        }

        internal static RegisterStorage? GetRegister(StorageDomain domain, ulong mask)
        {
            var iReg = domain - StorageDomain.Register;
            if (iReg < 0 || iReg >= regsByStorage.Length)
                return null;
            RegisterStorage? regBest = regsByStorage[iReg];
            if (regBest is null)
                return null;
            if (SubRegisters.TryGetValue(domain, out var subregs))
            {
                for (int i = 0; i < subregs.Length; ++i)
                {
                    var reg = subregs[i];
                    var regMask = reg.BitMask;
                    if ((mask & (~regMask)) == 0)
                        regBest = reg;
                }
            }
            return regBest;
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        SF = 0x80,             // sign
        ZF = 0x40,             // zero
        PF = 0x04,             // overflow / parity
        NF = 0x02,             // carry
        CF = 0x01,             // carry
    }

    public enum CondCode
    {
        nz,
        z,
        nc,
        c,
        po,
        pe,
        p,
        m,
    }
}
