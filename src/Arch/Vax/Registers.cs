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

namespace Reko.Arch.Vax
{
    public static class Registers
    {
        public static RegisterStorage r0 { get; }
        public static RegisterStorage r1 { get; }
        public static RegisterStorage r2 { get; }
        public static RegisterStorage r3 { get; }

        public static RegisterStorage r4 { get; }
        public static RegisterStorage r5 { get; }
        public static RegisterStorage r6 { get; }
        public static RegisterStorage r7 { get; }

        public static RegisterStorage r8 { get; }
        public static RegisterStorage r9 { get; }
        public static RegisterStorage r10 { get; }
        public static RegisterStorage r11 { get; }

        public static RegisterStorage ap { get; }
        public static RegisterStorage fp { get; }
        public static RegisterStorage sp { get; }
        public static RegisterStorage pc { get; }

        public static RegisterStorage psw { get; }


        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage CVN  { get; }
        public static FlagGroupStorage CVZN { get; }
        public static FlagGroupStorage CZ { get; }
        public static FlagGroupStorage CZN  { get; }
        public static FlagGroupStorage VZN  { get; }
        public static FlagGroupStorage ZN { get; }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            r0 = factory.Reg32("r0");
            r1 = factory.Reg32("r1");
            r2 = factory.Reg32("r2");
            r3 = factory.Reg32("r3");

            r4 = factory.Reg32("r4");
            r5 = factory.Reg32("r5");
            r6 = factory.Reg32("r6");
            r7 = factory.Reg32("r7");

            r8 = factory.Reg32("r8");
            r9 = factory.Reg32("r9");
            r10 = factory.Reg32("r10");
            r11 = factory.Reg32("r11");

            ap = factory.Reg32("ap");
            fp = factory.Reg32("fp");
            sp = factory.Reg32("sp");
            pc = factory.Reg32("pc");

            psw = RegisterStorage.Reg32("psw", 20);

            C = new FlagGroupStorage(psw, (uint) FlagM.CF, "C");
            V = new FlagGroupStorage(psw, (uint) FlagM.VF, "V");
            Z = new FlagGroupStorage(psw, (uint) FlagM.ZF, "Z");
            N = new FlagGroupStorage(psw, (uint) FlagM.NF, "N");
            CVN = new FlagGroupStorage(psw, (uint) FlagM.CVN, "CVN");
            CVZN = new FlagGroupStorage(psw, (uint) FlagM.CVZN, "CVZN");
            CZ = new FlagGroupStorage(psw, (uint) FlagM.CZ, "CZ");
            CZN = new FlagGroupStorage(psw, (uint) FlagM.CZN, "CZN");
            VZN = new FlagGroupStorage(psw, (uint) FlagM.VZN, "VZN");
            ZN = new FlagGroupStorage(psw, (uint) FlagM.ZN, "ZN");

            ByName = factory.NamesToRegisters;
            ByDomain = factory.DomainsToRegisters;
        }
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
