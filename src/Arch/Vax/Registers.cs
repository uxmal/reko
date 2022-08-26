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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public static class Registers
    {
        public static RegisterStorage r0 = RegisterStorage.Reg32("r0", 0);
        public static RegisterStorage r1 = RegisterStorage.Reg32("r1", 1);
        public static RegisterStorage r2 = RegisterStorage.Reg32("r2", 2);
        public static RegisterStorage r3 = RegisterStorage.Reg32("r3", 3);
                                                                        
        public static RegisterStorage r4 = RegisterStorage.Reg32("r4", 4);
        public static RegisterStorage r5 = RegisterStorage.Reg32("r5", 5);
        public static RegisterStorage r6 = RegisterStorage.Reg32("r6", 6);
        public static RegisterStorage r7 = RegisterStorage.Reg32("r7", 7);
                                                                        
        public static RegisterStorage r8 = RegisterStorage.Reg32("r8", 8);
        public static RegisterStorage r9 = RegisterStorage.Reg32("r9", 9);
        public static RegisterStorage r10 = RegisterStorage.Reg32("r10", 10);
        public static RegisterStorage r11 = RegisterStorage.Reg32("r11", 11);

        public static RegisterStorage ap = RegisterStorage.Reg32("ap", 12);
        public static RegisterStorage fp = RegisterStorage.Reg32("fp", 13);
        public static RegisterStorage sp = RegisterStorage.Reg32("sp", 14);
        public static RegisterStorage pc = RegisterStorage.Reg32("pc", 15);

        public static readonly RegisterStorage psw = RegisterStorage.Reg32("psw", 20);


        public static readonly FlagGroupStorage C = new FlagGroupStorage(psw, (uint) FlagM.CF, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(psw, (uint) FlagM.VF, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(psw, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(psw, (uint) FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage CVN = new FlagGroupStorage(psw, (uint) FlagM.CVN, "CVN", PrimitiveType.Byte);
        public static readonly FlagGroupStorage CVZN = new FlagGroupStorage(psw, (uint) FlagM.CVZN, "CVZN", PrimitiveType.Byte);
        public static readonly FlagGroupStorage CZ = new FlagGroupStorage(psw, (uint) FlagM.CZ, "CZ", PrimitiveType.Byte);
        public static readonly FlagGroupStorage CZN = new FlagGroupStorage(psw, (uint) FlagM.CZN, "CZN", PrimitiveType.Byte);
        public static readonly FlagGroupStorage VZN = new FlagGroupStorage(psw, (uint) FlagM.VZN, "VZN", PrimitiveType.Byte);
        public static readonly FlagGroupStorage ZN = new FlagGroupStorage(psw, (uint) FlagM.ZN, "ZN", PrimitiveType.Byte);
    }

    [Flags]
    public enum FlagM
    {
        NF = 8,
        ZF = 4,
        VF = 2,
        CF = 1,

        CVZN = NF|ZF|VF|CF,
        CVN = NF|VF| CF,
        CZ = CF|ZF,
        CZN = NF|ZF|CF,
        VZN = NF|ZF|VF,
        ZN = NF|ZF,
    }
}
