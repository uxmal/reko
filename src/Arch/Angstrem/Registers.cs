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

namespace Reko.Arch.Angstrem
{
    public static class Registers
    {
        public static RegisterStorage [] GpRegisters { get; }
        public static RegisterStorage [] ServiceRegisters { get; }

        /// <summary>
        /// Status register.
        /// </summary>
        public static RegisterStorage rs { get; }
        public static FlagGroupStorage S { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage H { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg(32, n => $"r{n}", PrimitiveType.Byte);
            rs = factory.Reg("rs", PrimitiveType.Byte);
            S = new FlagGroupStorage(rs, (uint) FlagM.S, "S");
            Z = new FlagGroupStorage(rs, (uint) FlagM.Z, "Z");
            C = new FlagGroupStorage(rs, (uint) FlagM.C, "C");
            V = new FlagGroupStorage(rs, (uint) FlagM.V, "V");
            H = new FlagGroupStorage(rs, (uint) FlagM.H, "H");
            factory = new StorageFactory(StorageDomain.SystemRegister);
            ServiceRegisters = factory.RangeOfReg(8, n => $"sr{n}", PrimitiveType.Byte);
        }
    }

    [Flags]
    public enum FlagM
    {
        S = 0x80,
        Z = 0x40,
        C = 0x20,
        V = 0x10,
        H = 0x08,
    }
}
