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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.OpenRISC
{
    public static class Registers
    {
        public static readonly RegisterStorage[] GpRegs;
        public static readonly RegisterStorage sr;
        public static readonly RegisterStorage machi;
        public static readonly RegisterStorage maclo;
        public static readonly Dictionary<int, RegisterStorage> SpecialRegisters;
        public static readonly Dictionary<string, RegisterStorage> RegisterByName;
        public static readonly Dictionary<StorageDomain, RegisterStorage> RegistersByDomain;

        public static readonly FlagGroupStorage F;
        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage V;

        static Registers()
        {
            var factory = new StorageFactory();
            var sprFactory = new StorageFactory(StorageDomain.SystemRegister);

            GpRegs = factory.RangeOfReg32(32, "r{0}");
            SpecialRegisters = new (int, string)[]
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
                ( 9, "VR2"), // Version register 2
                ( 10, "AVR"), //  Architecture version register 
                ( 11, "EVBAR"), // R/W Exception vector base address register
                ( 12, "AECR"), // R/W Arithmetic Exception Control Register
                ( 13, "AESR"), // R/W Arithmetic Exception Status Register
                ( 16, "NPC"), // R/W PC mapped to SPR space (next PC)
                ( 17, "SR"),// R/W Supervision register 
                ( 18, "PPC"),// R PC mapped to SPR space (previous PC)
                ( 20, "FPCSR"), // R/W FP Control Status register

                ( (5 << 11) + 1, "MACLO" ),
                ( (5 << 11) + 2, "MACHI" )
            }.ToDictionary(e => e.Item1, e=> sprFactory.Reg32(e.Item2));

            sr = SpecialRegisters[17];
            maclo = SpecialRegisters[(5 << 11) + 1];
            machi = SpecialRegisters[(5 << 11) + 2];
            RegisterByName = GpRegs
                .Concat(SpecialRegisters.Values)
                .ToDictionary(r => r.Name);
            RegistersByDomain = GpRegs
                .Concat(SpecialRegisters.Values)
                .ToDictionary(r => r.Domain);

            F = new FlagGroupStorage(sr, (uint)FlagM.F, "F", PrimitiveType.Bool);
            C = new FlagGroupStorage(sr, (uint)FlagM.CY, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(sr, (uint)FlagM.OV, "V", PrimitiveType.Bool);
        }
    }

    [Flags]
    public enum FlagM : uint
    {
        F = 1 << 9,
        CY = 1 << 10,
        OV = 1 << 11,
    }
}