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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Qualcomm
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }

        // R0 - r5 arg regs
        // r0 return
        // caller save r6-r5
        // callee save r16-r27
        // r29 sp
        // r30 fp
        // r31 lr

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg32(32, "r{0}");
            ByName = GpRegs.ToDictionary(k => k.Name);
        }
    }
}