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
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Arch.Avr.Avr32
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            var regs = factory.RangeOfReg32(16, "r{0}");
            regs[13] = new RegisterStorage("sp", regs[13].Number, 0, PrimitiveType.Word32);
            regs[14] = new RegisterStorage("lr", regs[14].Number, 0, PrimitiveType.Word32);
            regs[15] = new RegisterStorage("pc", regs[14].Number, 0, PrimitiveType.Word32);

            RegistersByName = regs.ToDictionary(r => r.Name);
        }

        public static Dictionary<string, RegisterStorage> RegistersByName { get; }
    }
}
