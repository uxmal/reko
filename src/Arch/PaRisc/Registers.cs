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

namespace Reko.Arch.PaRisc
{
    public class Registers
    {
        public readonly RegisterStorage[] GpRegs;
        public readonly RegisterStorage[] FpRegs;
        public readonly RegisterStorage[] FpLefts;
        public readonly RegisterStorage[] FpRights;
        public readonly RegisterStorage[] FpRegs32;

        public static readonly RegisterStorage[] SpaceRegs;
        public static readonly RegisterStorage PSW; // Program Status Word

        public static readonly RegisterStorage SAR; // Shift amount register

        public static readonly Dictionary<int, RegisterStorage> ControlRegisters;

        public static readonly FlagGroupStorage CF;
        public Dictionary<StorageDomain, RegisterStorage> RegistersByStorageDomain { get; }
        public Dictionary<string, RegisterStorage> RegistersByName { get; }

        public Registers(PrimitiveType gpRegSize)
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg(32, i => $"r{i}", gpRegSize);
            FpRegs = factory.RangeOfReg64(32, "fr{0}");
            FpLefts = FpRegs
                .Select(fr => new RegisterStorage(fr.Name + "L", fr.Number, 32, PrimitiveType.Word32))
                .ToArray();
            FpRights = FpRegs
                .Select(fr => new RegisterStorage(fr.Name + "R", fr.Number, 0, PrimitiveType.Word32))
                .ToArray();
            //$BUG: triple-check the formatting of 6-bit floating point
            // register identifiers.
            FpRegs32 = FpLefts.Concat(FpRights).ToArray();
            RegistersByStorageDomain =
                GpRegs.ToDictionary(r => r.Domain);
            RegistersByName = GpRegs.Concat(FpRegs)
                .Concat(FpLefts)
                .Concat(FpRights)
                .ToDictionary(r => r.Name);
        }

        static Registers()
        {
            var factory = new StorageFactory(StorageDomain.SystemRegister);
            SpaceRegs = factory.RangeOfReg32(8, "sr{0}");
            PSW = factory.Reg64("psw");
            ControlRegisters = new[] {
                "rctr",
                "cr1",
                "cr2",
                "cr3",

                "cr4",
                "cr5",
                "cr6",
                "cr7",

                // CR[8]
                "pidr1",
                "pidr2",
                "ccr",
                "sar",

                "pidr3",
                "pidr4",
                "iva",
                "eiem",

                // CR[16]
                "itmr",
                "pcsq",
                "pcoq",
                "iir",

                "isr",
                "ior",
                "ipsw",
                "eirr",

                // CR[24]
                "tr0",
                "tr1",
                "tr2",
                "tr3",

                "tr4",
                "tr5",
                "tr6",
                "tr7",
            }.Select((regName, i) => (i, factory.Reg32(regName)))
            .ToDictionary(item => item.i, item => item.Item2);

            SAR = ControlRegisters[11];

            CF = new FlagGroupStorage(PSW, 0xF000, "C", PrimitiveType.Byte);
        }
    }
}
