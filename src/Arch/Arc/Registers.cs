#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.Arch.Arc
{
    public class Registers
    {
        public static readonly RegisterStorage [] CoreRegisters;
        public static readonly RegisterStorage Gp;
        public static readonly RegisterStorage Fp;
        public static readonly RegisterStorage Sp;
        public static readonly RegisterStorage Blink;
        public static readonly RegisterStorage LP_count;
        public static readonly RegisterStorage Pcl;

        public static readonly RegisterStorage Status32;

        static Registers()
        {
            var factory = new StorageFactory();
            CoreRegisters = factory.RangeOfReg32(64, "r{0}");

            Gp = RenameRegister(26, "gp");
            Fp = RenameRegister(27, "fp");
            Sp = RenameRegister(28, "sp");
            Blink = RenameRegister(31, "blink");
            LP_count = RenameRegister(60, "lp_count");
            Pcl  = RenameRegister(63, "pcl");

            var sysFactory = new StorageFactory(StorageDomain.SystemRegister);
            Status32 = sysFactory.Reg("STATUS32", PrimitiveType.Word32);
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
}