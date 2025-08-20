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
using System;
using System.Collections.Generic;

namespace Reko.Arch.MN103
{
    public class Registers
    {
        public static RegisterStorage[] DRegs { get; }
        public static RegisterStorage[] ARegs { get; }
        public static RegisterStorage sp { get; }
        public static RegisterStorage pc { get; }
        public static RegisterStorage mdr { get; }
        public static RegisterStorage psw { get; }
        public static RegisterStorage lir { get; }
        public static RegisterStorage lar { get; }
        public static RegisterStorage usp { get; }
        public static RegisterStorage ssp { get; }
        public static RegisterStorage msp { get; }

        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage CNZ { get; }
        public static FlagGroupStorage CZ { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage NZ { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage VC { get; }
        public static FlagGroupStorage VCN { get; }
        public static FlagGroupStorage VCNZ { get; }
        public static FlagGroupStorage VN { get; }
        public static FlagGroupStorage VNZ { get; }
        public static FlagGroupStorage Z { get; }
        public static Dictionary<string, RegisterStorage> RegistersByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> RegistersByDomain { get; }

        static Registers()
        {
            var factory = new StorageFactory();

            DRegs = factory.RangeOfReg32(4, "d{0}");
            ARegs = factory.RangeOfReg32(4, "a{0}");
            sp = factory.Reg32("sp");
            pc = factory.Reg32("pc");
            mdr = factory.Reg32("mdr");
            psw = factory.Reg16("psw");
            lir = factory.Reg32("lir");
            lar = factory.Reg32("lar");
            usp = factory.Reg32("usp");
            ssp = factory.Reg32("ssp");
            msp = factory.Reg32("msp");

            C = new FlagGroupStorage(psw, (uint) FlagM.CF, "C");
            CNZ = new FlagGroupStorage(psw, (uint) (FlagM.CF | FlagM.NF | FlagM.ZF), "CNZ");
            CZ = new FlagGroupStorage(psw, (uint) (FlagM.CF | FlagM.ZF), "CZ");
            N = new FlagGroupStorage(psw, (uint) FlagM.NF, "N");
            NZ = new FlagGroupStorage(psw, (uint) (FlagM.NF | FlagM.VF), "NZ");
            V = new FlagGroupStorage(psw, (uint) FlagM.VF, "V");
            VC = new FlagGroupStorage(psw, (uint) (FlagM.CF | FlagM.VF), "VC");
            VCN = new FlagGroupStorage(psw, (uint) (FlagM.NF | FlagM.CF | FlagM.VF), "VCN");
            VCNZ = new FlagGroupStorage(psw, (uint) (FlagM.ZF | FlagM.NF | FlagM.CF | FlagM.VF), "VCNZ");
            VN = new FlagGroupStorage(psw, (uint) (FlagM.NF | FlagM.VF), "VN");
            VNZ = new FlagGroupStorage(psw, (uint) (FlagM.NF | FlagM.VF| FlagM.ZF), "VNZ");
            Z = new FlagGroupStorage(psw, (uint) FlagM.ZF, "Z");

            RegistersByName = factory.NamesToRegisters;
            RegistersByDomain = factory.DomainsToRegisters;
        }
    }

    [Flags]
    public enum FlagM
    {
        ZF = 1,
        NF = 2,
        CF = 4,
        VF = 8,
    }
}
