#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Text;

namespace Reko.Arch.Renesas.Rx
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage sp { get; }

        public static RegisterStorage ACC0 { get; }
        public static RegisterStorage ACC1 { get; }

        public static RegisterStorage ISP { get; }
        public static RegisterStorage USP { get; }
        public static RegisterStorage INTB { get; }
        public static RegisterStorage PC { get; }
        public static RegisterStorage PSW { get; }
        public static RegisterStorage BPC { get; }
        public static RegisterStorage BPSW { get; }
        public static RegisterStorage FINTV { get; }
        public static RegisterStorage FPSW { get; }
        public static RegisterStorage EXTB { get; }

        public static FlagGroupStorage C { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg(16, n => n == 0 ? "sp" : $"r{n}", PrimitiveType.Word32);
            sp = GpRegisters[0];

            ACC0 = factory.Reg("ACC0", PrimitiveType.CreateWord(72));
            ACC1 = factory.Reg("ACC1", PrimitiveType.CreateWord(72));

            factory = new StorageFactory(StorageDomain.SystemRegister);
            ISP = factory.Reg32("ISP");     // Interrupt stack pointer
            USP = factory.Reg32("USP");     // User stack pointer
            INTB = factory.Reg32("INTB");   // Interrupt table register
            PC = factory.Reg32("PC");       // Program counter
            PSW = factory.Reg32("PSW");     // Processor status word
            BPC = factory.Reg32("BPC");     // Backup PC
            BPSW = factory.Reg32("BPSW");   // Backup PSW
            FINTV = factory.Reg32("FINTV"); // Fast interrupt vector register
            FPSW = factory.Reg32("FPSW");   // Floating-point status word
            EXTB = factory.Reg32("EXTB");   // Exception table register

            C = new FlagGroupStorage(PSW, (uint) FlagM.CF, "C");
        }
    }

    public enum FlagM
    {
        CF = 1,
        ZF = 2,
        SF = 4,
        OF = 8,
    }
}
