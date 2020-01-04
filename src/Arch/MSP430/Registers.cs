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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Msp430
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters;
        public readonly static Dictionary<string, RegisterStorage> ByName;

        public static RegisterStorage pc = new RegisterStorage("pc", 0, 0, Msp430Architecture.Word20);
        public static RegisterStorage sp = new RegisterStorage("sp", 1, 0, Msp430Architecture.Word20);
        public static RegisterStorage sr = new RegisterStorage("sr", 2, 0, Msp430Architecture.Word20);

        static Registers()
        {
            GpRegisters = new RegisterStorage[]
            {
                pc,
                sp,
                sr,
                new RegisterStorage("cg2", 3, 0, Msp430Architecture.Word20),
            }.Concat(
                Enumerable.Range(4, 12)
                .Select(i => new RegisterStorage(
                    string.Format("r{0}", i), i, 0, Msp430Architecture.Word20)))

                .ToArray();

            ByName = GpRegisters.ToDictionary(r => r.Name);
        }
    }

    [Flags]
    public enum FlagM
    {
        VF = 0x100,
        NF = 0x004,
        ZF = 0x002,
        CF = 0x001
    }
}