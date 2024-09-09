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
using System.Text;

#pragma warning disable IDE1006

namespace Reko.Arch.MilStd1750
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        public static RegisterStorage pir { get; }    /* 16 */
        public static RegisterStorage mk { get; }     /* 17 */
        public static RegisterStorage ft { get; }     /* 18 */
        public static RegisterStorage ic { get; }     /* 19 */
        public static RegisterStorage sw { get; }     /* 20 */
        public static RegisterStorage ta { get; }     /* 21 */
        public static RegisterStorage tb { get; }     /* 22 */

        public static RegisterStorage go { get; }     /* not a real register but handled like TA/TB */
        public static RegisterStorage sys { get; }    /* system configuration register */

        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }

        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage P { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage N { get; }


        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg(16, n => $"gp{n}", PrimitiveType.Word16);
            pir = factory.Reg16("pir");
            mk = factory.Reg16("mk");
            ft = factory.Reg16("ft");
            ic = factory.Reg16("ic");
            sw = factory.Reg16("sw");
            ta = factory.Reg16("ta");
            tb = factory.Reg16("tb");
            go = factory.Reg16("go");
            sys = factory.Reg16("sys");
            ByName = factory.NamesToRegisters;
            ByDomain = factory.DomainsToRegisters;

            C = new FlagGroupStorage(Registers.sw, (uint) FlagM.CF, nameof(C));
            P = new FlagGroupStorage(Registers.sw, (uint) FlagM.PF, nameof(P));
            Z = new FlagGroupStorage(Registers.sw, (uint) FlagM.ZF, nameof(Z));
            N = new FlagGroupStorage(Registers.sw, (uint) FlagM.NF, nameof(N));
        }
    }

    [Flags]
    public enum FlagM
    {
        CF = 8,
        PF = 4,
        ZF = 2,
        NF = 1,
    }
}
