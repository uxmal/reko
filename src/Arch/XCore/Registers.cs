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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.XCore
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        private static RegisterStorage cp { get; }
        private static RegisterStorage dp { get; }
        private static RegisterStorage sp { get; }
        private static RegisterStorage lr { get; }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }


        static Registers()
        {
            var factory = new StorageFactory();
            var regs = factory.RangeOfReg32(12, "r{0}");
            cp = factory.Reg32("cp");
            dp = factory.Reg32("dp");
            sp = factory.Reg32("sp");
            lr = factory.Reg32("lr");
            GpRegs = regs.Concat(new[] { cp, dp, sp, lr }).ToArray();

            ByName = factory.NamesToRegisters;
            ByDomain = factory.DomainsToRegisters;
        }
    }
}