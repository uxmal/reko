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

namespace Reko.Arch.Arc
{
    public class Registers
    {
        public static readonly RegisterStorage [] CoreRegisters;
        public static readonly RegisterStorage Gp;
        public static readonly RegisterStorage Fp;
        public static readonly RegisterStorage Sp;
        public static readonly RegisterStorage Blink;
        public static readonly RegisterStorage Mlo;
        public static readonly RegisterStorage Mhi;
        public static readonly RegisterStorage LP_count;
        public static readonly RegisterStorage Pcl;

        public static readonly RegisterStorage Status32;
        public static readonly RegisterStorage LpStart;
        public static readonly RegisterStorage LpEnd;
        public static readonly RegisterStorage AuxMacmode;

        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage N;
        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage S;

        public static readonly Dictionary<StorageDomain, RegisterStorage> ByStorageDomain;
        public static readonly Dictionary<string, RegisterStorage> ByName;

        static Registers()
        {
            var factory = new StorageFactory();
            CoreRegisters = factory.RangeOfReg32(64, "r{0}");

            Gp = RenameRegister(26, "gp");
            Fp = RenameRegister(27, "fp");
            Sp = RenameRegister(28, "sp");
            Blink = RenameRegister(31, "blink");
            Mlo = RenameRegister(57, "mlo");
            Mhi = RenameRegister(59, "mhi");
            LP_count = RenameRegister(60, "lp_count");
            Pcl  = RenameRegister(63, "pcl");

            var sysFactory = new StorageFactory(StorageDomain.SystemRegister);
            Status32 = sysFactory.Reg("STATUS32", PrimitiveType.Word32);

            LpStart = sysFactory.Reg("LP_START", PrimitiveType.Word32);
            LpEnd = sysFactory.Reg("LP_END", PrimitiveType.Word32);
            AuxMacmode = sysFactory.Reg("AUX_MACMODE", PrimitiveType.Word32);

            Z = new FlagGroupStorage(Status32, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            N = new FlagGroupStorage(Status32, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            C = new FlagGroupStorage(Status32, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(Status32, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            S = new FlagGroupStorage(AuxMacmode, (uint) AuxFlagM.Sat, "S", PrimitiveType.Bool);

            ByStorageDomain = factory.DomainsToRegisters
                .Concat(sysFactory.DomainsToRegisters)
                .ToDictionary(k => k.Key, v => v.Value);
            ByName = factory.NamesToRegisters
                .Concat(sysFactory.NamesToRegisters)
                .ToDictionary(k => k.Key, v => v.Value);
        }

        private static RegisterStorage RenameRegister(int iGpReg, string regName)
        {
            var oldReg = CoreRegisters[iGpReg];
            var newReg = new RegisterStorage(regName, oldReg.Number, 0, oldReg.DataType);
            CoreRegisters[iGpReg] = newReg;
            return newReg;
        }
    }

    [Flags]
    public enum FlagM
    {
        ZF = 2048,
        NF = 1024,
        CF = 512,
        VF = 256,
    }

    [Flags]
    public enum AuxFlagM
    {
        Sat = 1 << 4,
    }
}