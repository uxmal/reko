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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.OpenRISC.Aeon
{
    public static class Registers
    {
        static Registers()
        {
            var factory = new StorageFactory();
            Registers.GpRegisters = factory.RangeOfReg(32, r => $"r{r}", PrimitiveType.Word32);

            var sprFactory = new StorageFactory(StorageDomain.SystemRegister);
            Registers.SpecialRegisters = new (int, string)[]
            {
                ( 0, "VR"), // Version register
                ( 1, "UPR"), //Unit Present register
                ( 2, "CPUCFGR"), //CPU Configuration register
                ( 3, "DMMUCFGR"), //Data MMU Configuration register
                ( 4, "IMMUCFGR"), //Instruction MMU Configuration register
                ( 5, "DCCFGR"), //Data Cache Configuration register
                ( 6, "ICCFGR"), //Instruction Cache Configuration register
                ( 7, "DCFGR"), //Debug Configuration register
                ( 8, "PCCFGR"), // Performance Counters Configuration register
                ( 16, "NPC"), // R/W PC mapped to SPR space (next PC)
                ( 17, "SR"),// R/W Supervision register 
                ( 18, "PPC"),// R PC mapped to SPR space (previous PC)

                ( (5 << 11) + 1, "MACLO" ), // low word of MAC (multiply/accumulate reigster)
                ( (5 << 11) + 2, "MACHI" ), // high word of MAC
                ( (5 << 11) + 3, "MACHI2" ), // highest? word of MAC
            }.ToDictionary(e => e.Item1, e => sprFactory.Reg32(e.Item2));

            for (int i = 0; i < 16; i++) {
                // exception saved PC
                Registers.SpecialRegisters.Add(32 + i, sprFactory.Reg32($"EPCR_{i}"));
                // exception effective address
                Registers.SpecialRegisters.Add(48 + i, sprFactory.Reg32($"EEAR_{i}"));
                // exception saved SR
                Registers.SpecialRegisters.Add(64 + i, sprFactory.Reg32($"ESR_{i}"));
            }

            SR = Registers.SpecialRegisters[17];
            EPCR = Registers.SpecialRegisters[32];
            EEAR = Registers.SpecialRegisters[48];
            ESR = Registers.SpecialRegisters[64];
            MACLO = Registers.SpecialRegisters[(5 << 11) + 1];
            MACHI = Registers.SpecialRegisters[(5 << 11) + 2];
            MACHI2 = Registers.SpecialRegisters[(5 << 11) + 3];

            F = new FlagGroupStorage(SR, (uint) FlagSR.F, "f", PrimitiveType.Bool);
            Registers.ByDomain = Registers.GpRegisters
                .Concat(Registers.SpecialRegisters.Values)
                .ToDictionary(r => r.Domain);
            Registers.ByName = Registers.GpRegisters
                .Concat(Registers.SpecialRegisters.Values)
                .ToDictionary(r => r.Name);
        }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
        public static RegisterStorage[] GpRegisters { get; }
        public static Dictionary<int, RegisterStorage> SpecialRegisters  { get; }

        public static RegisterStorage SR { get; }
        public static RegisterStorage EPCR { get; }
        public static RegisterStorage EEAR { get; }
        public static RegisterStorage ESR { get; }
        public static RegisterStorage MACLO { get; }
        public static RegisterStorage MACHI { get; }
        public static RegisterStorage MACHI2 { get; }

        public static FlagGroupStorage F { get; }
    }

    // Supervision Register flags
    [Flags]
    public enum FlagSR : uint
    {
        SM = (1 << 0),
        TEE = (1 << 1),
        IEE = (1 << 2),
        DCE = (1 << 3),
        ICE = (1 << 4),
        DME = (1 << 5),
        IME = (1 << 6),
        LEE = (1 << 7),
        CE = (1 << 8),
        F = (1 << 9),
        CY = (1 << 10),
        OV = (1 << 11),
        OVE = (1 << 12),
        DSX = (1 << 13),
        EPH = (1 << 14),
        FO = (1 << 15),
        SUMRA = (1 << 16),
    }
}
