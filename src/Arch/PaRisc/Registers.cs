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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public class Registers
    {
        public static readonly RegisterStorage[] GpRegs;
        public static readonly RegisterStorage[] SpaceRegs;
        public static readonly RegisterStorage[] FpRegs;
        public static readonly RegisterStorage[] FpLefts;
        public static readonly RegisterStorage[] FpRights;
        public static readonly RegisterStorage[] FpRegs32;

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg32(32, "r{0}");
            SpaceRegs = factory.RangeOfReg32(8, "sr{0}");
            FpRegs = factory.RangeOfReg64(32, "fr{0}");
            FpLefts = FpRegs
                .Select(fr => new RegisterStorage(fr.Name + "L", fr.Number, 32, PrimitiveType.Word32))
                .ToArray();
            FpRights = FpRegs
                .Select(fr => new RegisterStorage(fr.Name + "R", fr.Number, 0, PrimitiveType.Word32))
                .ToArray();
            //$BUG: triple-check the formatting of 6-bit floating point
            // register identifiers.
            FpRegs32 = FpLefts.Concat(FpRights).ToArray();
        }
    }
}
