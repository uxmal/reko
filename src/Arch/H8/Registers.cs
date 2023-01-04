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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.H8
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = factory.RangeOfReg32(8, "er{0}");
            GpRegisters[7] = new RegisterStorage("sp", 7, 0, PrimitiveType.Ptr32);
            RRegisters = GpRegisters.Select((r, i) => RegisterStorage.Reg16($"r{i}", r.Number, 0)).ToArray();
            ERegisters = GpRegisters.Select((r, i) => RegisterStorage.Reg16($"e{i}", r.Number, 16)).ToArray();
            RlRegisters = GpRegisters.Select((r, i) => RegisterStorage.Reg8($"r{i}l", r.Number, 0)).ToArray();
            RhRegisters = GpRegisters.Select((r, i) => RegisterStorage.Reg8($"r{i}h", r.Number, 8)).ToArray();
            Gp16Registers = RRegisters.Concat(ERegisters).ToArray();
            Gp8Registers = RhRegisters.Concat(RlRegisters).ToArray();

            PcRegister = factory.Reg32("pc");
            CcRegister = factory.Reg("ccr", PrimitiveType.Byte);

            ByName = GpRegisters
                .Concat(Gp16Registers)
                .Concat(Gp8Registers)
                .Concat(new[]
                {
                    PcRegister,
                    CcRegister
                })
                .ToDictionary(r => r.Name);
        }

        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage[] Gp16Registers { get; }
        public static RegisterStorage[] Gp8Registers { get; }
        public static RegisterStorage[] RRegisters { get; }
        public static RegisterStorage[] ERegisters { get; }
        public static RegisterStorage[] RlRegisters { get; }
        public static RegisterStorage[] RhRegisters { get; }

        public static RegisterStorage PcRegister { get; }
        public static RegisterStorage CcRegister { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }
    }

    [Flags]
    public enum FlagM
    {
        IF = 0x80,      // Interrupt
        HF = 0x20,      // Half-carry
        UF = 0x10,      // User
        NF = 0x08,      // Negative
        ZF = 0x04,      // Zero
        VF = 0x02,      // Overflow
        CF = 0x01,      // Carry
    }
}
