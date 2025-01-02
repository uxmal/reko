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

namespace Reko.Arch.Padauk
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            f = factory.Reg("f", PrimitiveType.Byte);
            a = factory.Reg("a", PrimitiveType.Byte);
            sp = factory.Reg("sp", PrimitiveType.Ptr16);

            AC = new FlagGroupStorage(f, (uint) FlagM.AC, nameof(AC));
            C = new FlagGroupStorage(f, (uint) FlagM.CF, nameof(C));
            V = new FlagGroupStorage(f, (uint) FlagM.OV, nameof(V));
            Z = new FlagGroupStorage(f, (uint) FlagM.ZF, nameof(Z));
            ZCAV = new FlagGroupStorage(f , (uint) (FlagM.ZF | FlagM.CF | FlagM.AC | FlagM.OV), nameof(ZCAV));
            FlagBits = new[] { AC, C, V, Z };
            RegistersByName = factory.NamesToRegisters;
            RegistersByDomain = factory.DomainsToRegisters;
        }

        public static RegisterStorage a { get; }
        public static RegisterStorage f { get; }
        public static RegisterStorage sp { get; }
        public static FlagGroupStorage AC { get; }
        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage ZCAV { get; }
        public static Dictionary<string, RegisterStorage> RegistersByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> RegistersByDomain { get; }
        public static FlagGroupStorage [] FlagBits { get; }
    }

    [Flags]
    public enum FlagM
    {
        ZF = 1,
        CF = 2,
        AC = 4,
        OV = 8,
    }
}
