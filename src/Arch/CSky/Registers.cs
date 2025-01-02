#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Arch.CSky
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        public static RegisterStorage[] VrRegs { get; }
        public static RegisterStorage Macc { get; }
        public static RegisterStorage Lo { get; }
        public static RegisterStorage Hi { get; }
        public static RegisterStorage Pc { get; }

        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

        public static Dictionary<StorageDomain, RegisterStorage> ControlRegisters { get; }

        public static Dictionary<int, RegisterStorage> CodesToControlRegisters { get; }
        public static RegisterStorage Psr { get; }
        public static RegisterStorage Vbr { get; }

        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage V { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg32(32, "r{0}");
            Macc = factory.Reg64("macc");
            Lo = RegisterStorage.Reg32("lo", Macc.Number, 0);
            Hi = RegisterStorage.Reg32("hi", Macc.Number, 32);
            Pc = factory.Reg32("pc");
            VrRegs = factory.RangeOfReg64(16, "vr{0}");

            ByName = factory.NamesToRegisters;
            ByDomain = factory.DomainsToRegisters;

            var crFactory = new StorageFactory(StorageDomain.SystemRegister);
            CodesToControlRegisters = new Dictionary<int, RegisterStorage>
            {
                { 0,  crFactory.Reg32("psr") },
                { 1,  crFactory.Reg32("vbr") },
                { 2,  crFactory.Reg32("epsr") },
                { 3,  crFactory.Reg32("fpsr") },
                { 4,  crFactory.Reg32("epc") },
                { 5,  crFactory.Reg32("fpc") },

                { 11,  crFactory.Reg32("gcr") },
                { 12,  crFactory.Reg32("gsr") },
                { 13,  crFactory.Reg32("cpid") },

                { 18,  crFactory.Reg32("ccr") },
                { 19,  crFactory.Reg32("capr") },
                { 20,  crFactory.Reg32("pacr") },
                { 21,  crFactory.Reg32("rid") },
            };
            ControlRegisters = crFactory.DomainsToRegisters;

            Psr = CodesToControlRegisters[0];
            Vbr = CodesToControlRegisters[1];
            //$TODO: manual is unclear on the positions of the bits in the psr
            C = new FlagGroupStorage(Psr, (uint) FlagM.CF, nameof(C));
            V = new FlagGroupStorage(Psr, (uint) FlagM.VF, nameof(V));
        }

        public static RegisterStorage ControlRegister(uint ireg)
        {
            var domain = StorageDomain.SystemRegister + (int) ireg;
            if (ControlRegisters.TryGetValue(domain, out var reg))
                return reg;
            return new RegisterStorage($"cr{ireg}", (int) domain, 0, PrimitiveType.Word32);
        }
    }

    [Flags]
    public enum FlagM
    {
        CF = 1,
        VF = 2,
    }
}
