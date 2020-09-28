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

namespace Reko.Arch.WE32100
{
    public static class Registers
    {
        public static RegisterStorage sp { get; }
        public static RegisterStorage[] GpRegs { get; }

        private static readonly string[] regNames = new[] {
                "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7",
                "r8", "fp", "ap", "psw","sp", "pcbp", "isp", "pc"
        };

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg(16, i => regNames[i], PrimitiveType.Word32);
        }
    }

    [Flags]
    public enum FlagM
    {
        N = 1 << 21,
        Z = 1 << 20,
        V = 1 << 19,
        C = 1 << 18,
    }
}