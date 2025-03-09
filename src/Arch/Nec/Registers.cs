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
using System.Threading.Tasks;

namespace Reko.Arch.Nec
{
    public static class Registers
    {
        public static RegisterStorage[] Scalars { get; }
        public static RegisterStorage[] Vectors { get; }

        public static RegisterStorage[] VectorMasks { get; }

        /// <summary>
        /// Comminucation registers.
        /// </summary>
        public static RegisterStorage[] CR { get; }

        /// <summary>
        /// Vector index register.
        /// </summary>
        public static RegisterStorage Vixr { get; }

        /// <summary>
        /// Vector length register.
        /// </summary>
        public static RegisterStorage Vl { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            Scalars = factory.RangeOfReg(64, n => $"s{n}", PrimitiveType.Word64);
            Vectors = factory.RangeOfReg(64, n => $"v{n}", PrimitiveType.Word64);
            VectorMasks = factory.RangeOfReg(16, n => $"vm{n}", PrimitiveType.Word256);
            Vixr = factory.Reg("vixr", PrimitiveType.Word64); // Actually, 6 bits.
            Vl = factory.Reg("vl", PrimitiveType.Word64); // Actually, 9 bits.
            CR = factory.RangeOfReg(1024, n => $"cr{n}", PrimitiveType.Word64);
        }
    }
}
