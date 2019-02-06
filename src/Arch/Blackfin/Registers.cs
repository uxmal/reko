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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Blackfin
{
    public static class Registers
    {
        public static readonly RegisterStorage[] Data;
        public static readonly RegisterStorage[] Pointers;
        public static readonly RegisterStorage[] Indices;
        public static readonly RegisterStorage[] Bases;
        public static readonly RegisterStorage[] RPIB;
        public static readonly RegisterStorage FP;
        public static readonly RegisterStorage SP;
        public static readonly RegisterStorage PC;

        public static readonly RegisterStorage[] RPIB_Lo;
        public static readonly RegisterStorage[] RPI_Hi;

        static Registers()
        {
            var factory = new StorageFactory();
            Data = factory.RangeOfReg32(8, "R{0}");
            Pointers = factory.RangeOfReg32(8, "P{0}");
            Indices = factory.RangeOfReg32(8, "I{0}");
            Bases = factory.RangeOfReg32(8, "B{0}");
            Rename(ref Pointers[6], "SP");
            Rename(ref Pointers[7], "FP");
            SP = Pointers[6];
            FP = Pointers[7];
            PC = factory.Reg32("PC");

            RPIB = Data
                .Concat(Pointers)
                .Concat(Indices)
                .Concat(Bases)
                .ToArray();
            RPIB_Lo = RPIB
                .Select(r => new RegisterStorage(r.Name + ".L", r.Number, 0, PrimitiveType.Word16))
                .ToArray();
            RPI_Hi = Data
                .Concat(Pointers)
                .Concat(Indices)
                .Concat(Enumerable.Range(0, 8).Select(i => (RegisterStorage) null))
                .Select(r => r != null
                    ? new RegisterStorage(r.Name + ".H", r.Number, 16, PrimitiveType.Word16)
                    : null)
                .ToArray();
        }   

        private static void Rename(ref RegisterStorage reg, string name)
        {
            var regNew = new RegisterStorage(name, reg.Number, (uint)reg.BitAddress, reg.DataType);
            reg = regNew;
        }
    }
}
