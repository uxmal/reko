#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Types;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Pdp.Pdp11
{
    public class Registers
    {
        public static readonly RegisterStorage r0 = RegisterStorage.Reg16("r0", 0);
        public static readonly RegisterStorage r1 = RegisterStorage.Reg16("r1", 1);
        public static readonly RegisterStorage r2 = RegisterStorage.Reg16("r2", 2);
        public static readonly RegisterStorage r3 = RegisterStorage.Reg16("r3", 3);
        public static readonly RegisterStorage r4 = RegisterStorage.Reg16("r4", 4);
        public static readonly RegisterStorage r5 = RegisterStorage.Reg16("r5", 5);
        public static readonly RegisterStorage sp = RegisterStorage.Reg16("sp", 6);
        public static readonly RegisterStorage pc = RegisterStorage.Reg16("pc", 7);

        public static readonly RegisterStorage psw = RegisterStorage.Reg16("psw", 12);

        public static readonly RegisterStorage ac0 = new RegisterStorage("ac0", 16, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage ac1 = new RegisterStorage("ac1", 17, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage ac2 = new RegisterStorage("ac2", 18, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage ac3 = new RegisterStorage("ac3", 19, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage ac4 = new RegisterStorage("ac4", 20, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage ac5 = new RegisterStorage("ac5", 21, 0, PrimitiveType.Real64);

        public static readonly FlagGroupStorage N = new FlagGroupStorage(psw, 8, nameof(N));
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(psw, 4, nameof(Z));
        public static readonly FlagGroupStorage V = new FlagGroupStorage(psw, 2, nameof(V));
        public static readonly FlagGroupStorage C = new FlagGroupStorage(psw, 1, nameof(C));
        public static readonly FlagGroupStorage NV = new FlagGroupStorage(psw, 0xA, nameof(NV));
        public static readonly FlagGroupStorage NVC = new FlagGroupStorage(psw, 0xB, nameof(NVC));
        public static readonly FlagGroupStorage NZ = new FlagGroupStorage(psw, 0xC, nameof(NZ));
        public static readonly FlagGroupStorage NZC = new FlagGroupStorage(psw, 0xD, nameof(NZC));
        public static readonly FlagGroupStorage NZV = new FlagGroupStorage(psw, 0xE, nameof(NZV));
        public static readonly FlagGroupStorage NZVC = new FlagGroupStorage(psw, 0xF, nameof(NZVC));
        public static readonly FlagGroupStorage ZC = new FlagGroupStorage(psw, 0x5, nameof(ZC));
    }

    [Flags]
    public enum FlagM
    {
        NF = 8,
        ZF = 4,
        VF = 2,
        CF = 1,
    }
}
