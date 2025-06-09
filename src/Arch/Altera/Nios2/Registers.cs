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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Altera.Nios2
{
    public static class Registers
    {
        public static RegisterStorage sp { get; }
        public static RegisterStorage ra { get; }

        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage?[] ControlRegisters { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg32(32, "r{0}");
            GpRegisters[0] = RegisterStorage.Reg32("zero", 0);
            sp = GpRegisters[27] = RegisterStorage.Reg32("sp", 27);
            ra = GpRegisters[31] = RegisterStorage.Reg32("ra", 31);

            var ctrlFactory = new StorageFactory(StorageDomain.SystemRegister);
            ControlRegisters = new RegisterStorage?[]
            {
                ctrlFactory.Reg32("status"),    // 0 - Refer to The status Register on page 45
                ctrlFactory.Reg32("estatus"),   // 1 - Refer to The estatus Register on page 47
                ctrlFactory.Reg32("bstatus"),   // 2 - Refer to The bstatus Register
                ctrlFactory.Reg32("ienable"),   // 3 - Internal interrupt - enable bits - Available only when the external interrupt controller interface is not present.Otherwise reserved.
                ctrlFactory.Reg32("ipending"),  // 4 - Pending internal interrupt bits  Available only when the external interrupt controller interface is not present.
                ctrlFactory.Reg32("cpuid"),     // 5 - Unique processor identifier
                null,                           // 6 - Reserved
                ctrlFactory.Reg32("exception"), // 7 - Refer to The exception Register
                ctrlFactory.Reg32("pteaddr"),   // 8 - Refer to The pteaddr Register Available only when the MMU is present.

                ctrlFactory.Reg32("tlbacc"),    // 9 - Refer to The tlbacc Register Available only when the MMU is present.

                ctrlFactory.Reg32("tlbmisc"),   // 10 - Refer to The tlbmisc Register Available only when the MMU is present.
                ctrlFactory.Reg32("eccinj"),    // 11 - Refer to The eccinj Register Available only when ECC is present.
                ctrlFactory.Reg32("badaddr"),   // 12 - Refer to The badaddr Register
                ctrlFactory.Reg32("config"),    // 13 - Refer to The config Register on page 55. Available only when the MPU or ECC is present.
                ctrlFactory.Reg32("mpubase"),   // 14 - Refer to The mpubase Register Available only when the MPU is present.
                ctrlFactory.Reg32("mpuacc"),    // 15 - Refer to The mpuacc Register for MASK variations table
            };

            ByName = GpRegisters.ToDictionary(r => r.Name);
        }

        public static bool TryGetControlRegister(uint iregS, out RegisterStorage regS)
        {
            if (iregS < ControlRegisters.Length)
            {
                regS = ControlRegisters[iregS]!;
                return regS is not null;
            }
            regS = null!;
            return false;
        }
    }
}
