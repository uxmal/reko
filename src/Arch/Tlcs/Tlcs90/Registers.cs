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

namespace Reko.Arch.Tlcs.Tlcs90
{
    public static class Registers
    {
        public static readonly RegisterStorage af = new RegisterStorage("af", 0, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage bc = new RegisterStorage("bc", 1, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage de = new RegisterStorage("de", 2, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage hl = new RegisterStorage("hl", 3, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage af_ = new RegisterStorage("af'", 4, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage bc_ = new RegisterStorage("bc'", 5, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage de_ = new RegisterStorage("de'", 6, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage hl_ = new RegisterStorage("hl'", 7, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage bx = new RegisterStorage("bx", 8, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage by = new RegisterStorage("by", 9, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp = new RegisterStorage("sp", 10, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage pc = new RegisterStorage("pc", 11, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage a = new RegisterStorage("a", 0, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage b = new RegisterStorage("b", 1, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage c = new RegisterStorage("c", 1, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage d = new RegisterStorage("d", 2, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage e = new RegisterStorage("e", 2, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage h = new RegisterStorage("h", 3, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage l = new RegisterStorage("l", 3, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage ix = new RegisterStorage("ix", 8, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage iy = new RegisterStorage("iy", 9, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage f = new RegisterStorage("f", 32, 0, PrimitiveType.Byte);

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