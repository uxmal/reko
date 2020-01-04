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
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public class Registers
    {
        public static RegisterStorage[] GpRegisters;
        public static RegisterStorage[] FpRegisters;
        public static Dictionary<string, RegisterStorage> RegistersByName;
        public static FlagGroupStorage CC;

        static Registers()
        {
            GpRegisters = Enumerable.Range(0, 16)
                .Select(n => new RegisterStorage($"r{n}", n, 0, PrimitiveType.Word64))
                .ToArray();
            FpRegisters = Enumerable.Range(0, 16)
                .Select(n => new RegisterStorage($"f{n}", n + 16, 0, PrimitiveType.Word64))
                .ToArray();

            RegistersByName = GpRegisters.Concat(FpRegisters)
                .ToDictionary(r => r.Name);

            //$REVIEW: this is probably not correct, but close enough to get us started.
            var ccReg = new RegisterStorage("ccReg", 40, 0, PrimitiveType.Byte);
            CC = new FlagGroupStorage(ccReg, 0xF, "CC", PrimitiveType.Byte);
        }
    }
}
