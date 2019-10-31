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
using System;

namespace Reko.Arch.MicroBlaze
{
    public class Registers
    {
        public static readonly RegisterStorage[] GpRegs;
        public static readonly RegisterStorage msr;

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg32(32, "r{0}");

            msr = factory.Reg32("msr");
        }
    }

    [Flags]
    public enum FlagM
    {
        CY = 1 << 2, // 29
    }
}