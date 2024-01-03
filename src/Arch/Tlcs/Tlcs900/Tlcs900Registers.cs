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

namespace Reko.Arch.Tlcs.Tlcs900
{
    public static class Tlcs900Registers
    {
        public static readonly RegisterStorage xwa = RegisterStorage.Reg32("xwa", 0);
        public static readonly RegisterStorage xbc = RegisterStorage.Reg32("xbc", 1);
        public static readonly RegisterStorage xde = RegisterStorage.Reg32("xde", 2);
        public static readonly RegisterStorage xhl = RegisterStorage.Reg32("xhl", 3);
        public static readonly RegisterStorage xix = RegisterStorage.Reg32("xix", 4);
        public static readonly RegisterStorage xiy = RegisterStorage.Reg32("xiy", 5);
        public static readonly RegisterStorage xiz = RegisterStorage.Reg32("xiz", 6);
        public static readonly RegisterStorage xsp = RegisterStorage.Reg32("xsp", 7);

        public static readonly RegisterStorage wa = RegisterStorage.Reg16("wa", 0);
        public static readonly RegisterStorage bc = RegisterStorage.Reg16("bc", 1);
        public static readonly RegisterStorage de = RegisterStorage.Reg16("de", 2);
        public static readonly RegisterStorage hl = RegisterStorage.Reg16("hl", 3);
        public static readonly RegisterStorage ix = RegisterStorage.Reg16("ix", 4);
        public static readonly RegisterStorage iy = RegisterStorage.Reg16("iy", 5);
        public static readonly RegisterStorage iz = RegisterStorage.Reg16("iz", 6);
        public static readonly RegisterStorage sp = RegisterStorage.Reg16("sp", 7);

        public static readonly RegisterStorage w = RegisterStorage.Reg8("w", 0, 8);
        public static readonly RegisterStorage a = RegisterStorage.Reg8("a", 0);
        public static readonly RegisterStorage b = RegisterStorage.Reg8("b", 1, 8);
        public static readonly RegisterStorage c = RegisterStorage.Reg8("c", 1);
        public static readonly RegisterStorage d = RegisterStorage.Reg8("d", 2, 8);
        public static readonly RegisterStorage e = RegisterStorage.Reg8("e", 2);
        public static readonly RegisterStorage h = RegisterStorage.Reg8("h", 3, 8);
        public static readonly RegisterStorage l = RegisterStorage.Reg8("l", 3);

        public static readonly RegisterStorage sr = RegisterStorage.Reg16("sr", 8);
        public static readonly RegisterStorage f = RegisterStorage.Reg8("f", 8);

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
