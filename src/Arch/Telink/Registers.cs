#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.Telink
{
    public static class Registers
    {
        public static Dictionary<string, RegisterStorage>? ByName { get; internal set; }
        public static Dictionary<StorageDomain, RegisterStorage>? ByDomain { get; internal set; }
        public static RegisterStorage sp { get; }
        public static RegisterStorage lr { get; }
        public static RegisterStorage pc { get; }

        public static RegisterStorage[] GpRegisters { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            var gpRegs = new List<RegisterStorage>();
            gpRegs.AddRange(factory.RangeOfReg(13, n => $"r{n}", PrimitiveType.Word32));
            gpRegs.Add(sp = factory.Reg32("sp"));
            gpRegs.Add(lr = factory.Reg32("lr"));
            gpRegs.Add(pc = factory.Reg32("pc"));
            GpRegisters = gpRegs.ToArray();
        }
    }
}
