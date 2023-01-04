#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.IA64
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg64(127, "r{0}");
            PredicateRegisters = factory.RangeOfReg(64, n => $"p{n:00}", PrimitiveType.Bool);
            RegistersByName = GpRegisters
                .Concat(PredicateRegisters)
                .ToDictionary(r => r.Name);
        }

        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage[] PredicateRegisters { get; }
        public static Dictionary<string, RegisterStorage> RegistersByName { get; }
    }
}
