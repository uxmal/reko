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

namespace Reko.Arch.Tlcs.Tlcs900
{
    public static class Tlcs900Registers
    {
        public static readonly RegisterStorage xwa = new RegisterStorage("xwa", 0, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xbc = new RegisterStorage("xbc", 1, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xde = new RegisterStorage("xde", 2, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xhl = new RegisterStorage("xhl", 3, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xix = new RegisterStorage("xix", 4, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xiy = new RegisterStorage("xiy", 5, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xiz = new RegisterStorage("xiz", 6, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage xsp = new RegisterStorage("xsp", 7, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage wa = new RegisterStorage("wa", 0, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage bc = new RegisterStorage("bc", 1, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage de = new RegisterStorage("de", 2, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage hl = new RegisterStorage("hl", 3, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage ix = new RegisterStorage("ix", 4, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage iy = new RegisterStorage("iy", 5, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage iz = new RegisterStorage("iz", 6, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage sp = new RegisterStorage("sp", 7, 0, PrimitiveType.Word16);

        public static readonly RegisterStorage w = new RegisterStorage("w", 0, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage a = new RegisterStorage("a", 0, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage b = new RegisterStorage("b", 1, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage c = new RegisterStorage("c", 1, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage d = new RegisterStorage("d", 2, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage e = new RegisterStorage("e", 2, 0, PrimitiveType.Byte);
        public static readonly RegisterStorage h = new RegisterStorage("h", 3, 8, PrimitiveType.Byte);
        public static readonly RegisterStorage l = new RegisterStorage("l", 3, 0, PrimitiveType.Byte);

        public static readonly RegisterStorage sr = new RegisterStorage("sr", 8, 0, PrimitiveType.Word16);
        public static readonly RegisterStorage f = new RegisterStorage("f", 8, 0, PrimitiveType.Byte);

        internal static RegisterStorage[] regs;
        internal static Dictionary<StorageDomain, Dictionary<int, RegisterStorage>> Subregisters;

        public static readonly FlagGroupStorage S = new FlagGroupStorage(sr, 32, "S", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(sr, 16, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage H = new FlagGroupStorage(sr,  8, "H", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(sr,  4, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(sr,  2, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(sr,  1, "C", PrimitiveType.Bool);

        internal static FlagGroupStorage[] flagBits =
        {
            S,Z,H,V,N,C,
        };

        static Tlcs900Registers()
        {
            regs = new RegisterStorage[]
            {
                    xwa,
                    xbc,
                    xde,
                    xhl,
                    xix,
                    xiy,
                    xiz,
                    xsp,

                    wa,
                    bc,
                    de,
                    hl,
                    ix,
                    iy,
                    iz,
                    sp,

                    w,
                    a,
                    b,
                    c,
                    d,
                    e,
                    h,
                    l,
            };

            Subregisters = new Dictionary<StorageDomain, Dictionary<int, RegisterStorage>>()
            {
                {
                    xwa.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xwa },
                        { 0x100, wa },
                        { 0x080, a },
                        { 0x088, w }
                    }
                },
                {
                    xbc.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xbc },
                        { 0x100, bc },
                        { 0x080, c },
                        { 0x088, b }
                    }
                },
                {
                    xde.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xde },
                        { 0x100, de },
                        { 0x080, e },
                        { 0x088, d }
                    }
                },
                {
                    xhl.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xhl },
                        { 0x100, hl },
                        { 0x080, l },
                        { 0x088, h }
                    }
                },
                {
                    xix.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xix },
                        { 0x100, ix },
                    }
                },
                {
                    xiy.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xiy },
                        { 0x100, iy },
                    }
                },
                {
                    xiz.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xiz },
                        { 0x100, iz },
                    }
                },
                {
                    xsp.Domain, new Dictionary<int, RegisterStorage>
                    {
                        { 0x200, xsp },
                        { 0x100, sp },
                    }
                },
            };
        }
    }
}
