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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.LatticeMico
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }

        public static RegisterStorage gp { get; }
        public static RegisterStorage fp { get; }
        public static RegisterStorage sp { get; }
        public static RegisterStorage ra { get; }
        public static RegisterStorage ea { get; }
        public static RegisterStorage ba { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            var regs = factory.RangeOfReg32(26, "r{0}");
            gp = factory.Reg32("gp");
            fp = factory.Reg32("fp");
            sp = factory.Reg32("sp");
            ra = factory.Reg32("ra");
            ea = factory.Reg32("ea");
            ba = factory.Reg32("ba");

            GpRegs = regs.Concat(new[] { gp, fp, sp, ra, ea, ba }).ToArray();
        }
    }
}
