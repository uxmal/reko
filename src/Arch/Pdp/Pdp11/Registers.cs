#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
        public static RegisterStorage r0 = RegisterStorage.Reg16("r0", 0);
        public static RegisterStorage r1 = RegisterStorage.Reg16("r1", 1);
        public static RegisterStorage r2 = RegisterStorage.Reg16("r2", 2);
        public static RegisterStorage r3 = RegisterStorage.Reg16("r3", 3);
        public static RegisterStorage r4 = RegisterStorage.Reg16("r4", 4);
        public static RegisterStorage r5 = RegisterStorage.Reg16("r5", 5);
        public static RegisterStorage sp = RegisterStorage.Reg16("sp", 6);
        public static RegisterStorage pc = RegisterStorage.Reg16("pc", 7);

        public static RegisterStorage psw = RegisterStorage.Reg16("psw", 12);

        public static RegisterStorage ac0 = new RegisterStorage("ac0", 16, 0, PrimitiveType.Real64);
        public static RegisterStorage ac1 = new RegisterStorage("ac1", 17, 0, PrimitiveType.Real64);
        public static RegisterStorage ac2 = new RegisterStorage("ac2", 18, 0, PrimitiveType.Real64);
        public static RegisterStorage ac3 = new RegisterStorage("ac3", 19, 0, PrimitiveType.Real64);
        public static RegisterStorage ac4 = new RegisterStorage("ac4", 20, 0, PrimitiveType.Real64);
        public static RegisterStorage ac5 = new RegisterStorage("ac5", 21, 0, PrimitiveType.Real64);

        public static readonly FlagGroupStorage N = new FlagGroupStorage(psw, 8, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(psw, 4, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(psw, 2, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage C = new FlagGroupStorage(psw, 1, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage NV = new FlagGroupStorage(psw, 0xA, "NV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NVC = new FlagGroupStorage(psw, 0xB, "NVC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZ = new FlagGroupStorage(psw, 0xC, "NZ", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZC = new FlagGroupStorage(psw, 0xD, "NZC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZV = new FlagGroupStorage(psw, 0xE, "NZV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZVC = new FlagGroupStorage(psw, 0xF, "NZVC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage ZC = new FlagGroupStorage(psw, 0x5, "ZC", PrimitiveType.Byte);
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
