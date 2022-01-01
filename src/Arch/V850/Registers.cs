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

namespace Reko.Arch.V850
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        public static RegisterStorage ElementPtr { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            var gpRegs = factory.RangeOfReg32(32, "r{0}");
            gpRegs[30] = new RegisterStorage("ep", 30, 0, PrimitiveType.Word32);
            GpRegs = gpRegs;
            ElementPtr = gpRegs[30];
        }
    }
}