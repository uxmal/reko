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
        public static RegisterStorage[] VecRegisters;

        public static readonly Dictionary<StorageDomain, RegisterStorage> ByDomain;
        public static readonly Dictionary<string, RegisterStorage> ByName;
        public static readonly FlagGroupStorage CC;

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg64(16, "r{0}");
            VecRegisters = factory.RangeOfReg(32, n => $"v{n}", PrimitiveType.Word128);
            FpRegisters = VecRegisters
                .Take(16)
                .Select((vr, i) => RegisterStorage.Reg64($"f{i}", vr.Number, 64))
                .ToArray();

            ByDomain = GpRegisters
                .Concat(VecRegisters)
                .ToDictionary(r => r.Domain);
            ByName = GpRegisters
                .Concat(FpRegisters)
                .Concat(VecRegisters)
                .ToDictionary(r => r.Name);

            //$REVIEW: this should be a PSW.
            var ccReg = factory.Reg("ccReg", PrimitiveType.Byte);
            CC = new FlagGroupStorage(ccReg, 0xF, "CC", PrimitiveType.Byte);
        }
    }
}
