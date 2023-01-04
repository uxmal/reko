#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.MCore
{
    public static class Registers
    {
        public static RegisterStorage [] GpRegisters { get; }
        public static RegisterStorage [] CrRegisters { get; }

        public static RegisterStorage R0 { get; }
        public static RegisterStorage PSR { get; }

        public static FlagGroupStorage C { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg32(16, "r{0}");
            CrRegisters = factory.RangeOfReg32(32, "cr{0}");
            Replace(CrRegisters, 0, "psr");
            Replace(CrRegisters, 1, "vbr");
            Replace(CrRegisters, 2, "epsr");
            Replace(CrRegisters, 3, "fpsr");
            Replace(CrRegisters, 4, "epc");
            Replace(CrRegisters, 5, "fpc");
            Replace(CrRegisters, 6, "ss0");
            Replace(CrRegisters, 7, "ss1");
            Replace(CrRegisters, 8, "ss2");
            Replace(CrRegisters, 9, "ss3");
            Replace(CrRegisters, 10, "ss4");
            Replace(CrRegisters, 11, "gcr");
            Replace(CrRegisters, 12, "gsr");
            R0 = GpRegisters[0];

            PSR = factory.Reg32("psr");
            C = new FlagGroupStorage(PSR, (uint) FlagM.CF, "C", PrimitiveType.Bool);
        }

        private static void Replace(RegisterStorage[] regs, int iReg, string newName)
        {
            var reg = regs[iReg];
            var regNew = new RegisterStorage(newName, reg.Number, (uint)reg.BitAddress, reg.DataType);
            regs[iReg] = regNew;
        }
    }

    public enum FlagM
    {
        CF = 0x1,   // Carry
    }
}