#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Arch.Tlcs.Tlcs90
{
    public static class Registers
    {
        public static readonly RegisterStorage af = RegisterStorage.Reg16("af", 0);
        public static readonly RegisterStorage bc = RegisterStorage.Reg16("bc", 1);
        public static readonly RegisterStorage de = RegisterStorage.Reg16("de", 2);
        public static readonly RegisterStorage hl = RegisterStorage.Reg16("hl", 3);

        public static readonly RegisterStorage af_ = RegisterStorage.Reg16("af'", 4);
        public static readonly RegisterStorage bc_ = RegisterStorage.Reg16("bc'", 5);
        public static readonly RegisterStorage de_ = RegisterStorage.Reg16("de'", 6);
        public static readonly RegisterStorage hl_ = RegisterStorage.Reg16("hl'", 7);

        public static readonly RegisterStorage bx = RegisterStorage.Reg32("bx", 8);
        public static readonly RegisterStorage by = RegisterStorage.Reg32("by", 9);
        public static readonly RegisterStorage sp = RegisterStorage.Reg16("sp", 10);
        public static readonly RegisterStorage pc = RegisterStorage.Reg16("pc", 11);

        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0, 8);
        public static readonly RegisterStorage b = RegisterStorage.Reg8("b", 1, 8);
        public static readonly RegisterStorage c = RegisterStorage.Reg8("c", 1);
        public static readonly RegisterStorage d = RegisterStorage.Reg8("d", 2, 8);
        public static readonly RegisterStorage e = RegisterStorage.Reg8("e", 2);
        public static readonly RegisterStorage h = RegisterStorage.Reg8("h", 3, 8);
        public static readonly RegisterStorage l = RegisterStorage.Reg8("l", 3);
        public static readonly RegisterStorage ix = RegisterStorage.Reg16("ix", 8);
        public static readonly RegisterStorage iy = RegisterStorage.Reg16("iy", 9);

        public static readonly RegisterStorage f = RegisterStorage.Reg8("f", 32);

        public static readonly FlagGroupStorage S = new FlagGroupStorage(f, (uint)FlagM.SF, "S", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(f, (uint)FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage I = new FlagGroupStorage(f, (uint)FlagM.IF, "I", PrimitiveType.Bool);
        public static readonly FlagGroupStorage H = new FlagGroupStorage(f, (uint)FlagM.HF, "H", PrimitiveType.Bool);
        public static readonly FlagGroupStorage X = new FlagGroupStorage(f, (uint)FlagM.XF, "X", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(f, (uint)FlagM.VF, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(f, (uint)FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(f, (uint)FlagM.CF, "C", PrimitiveType.Bool);

        public static RegisterStorage[] byteRegs = new[]
        {
            b, c, d, e, h, l, a
        };

        public static FlagGroupStorage[] flagBits = new[]
        {
            S, Z, H, X, V, N, C
        };

        public static RegisterStorage[] allRegs = new[]
        {
            bc, de, hl, ix, iy, sp,
            a, b, c, d, e, f, h, l,
            af_, bc_, de_, hl_,
        };

    }

    public enum FlagM
    {
        SF = 128,
        ZF = 64,
        IF = 32,
        HF = 16,
        XF = 8,
        VF = 4,
        NF = 2,
        CF = 1
    }
}