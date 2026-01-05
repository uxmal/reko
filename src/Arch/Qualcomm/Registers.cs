#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.IO;
using System.Linq;

namespace Reko.Arch.Qualcomm
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }

        public static RegisterStorage[] ModifierRegisters { get; }

        public static RegisterStorage[] PredicateRegisters { get; }
        
        public static RegisterStorage[] ControlRegisters { get; }
        public static Dictionary<uint, RegisterStorage> SystemRegisters { get; }
        public static RegisterStorage[]  GuestControlRegisters { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

        public static RegisterStorage sp { get; }
        public static RegisterStorage fp { get; }
        public static RegisterStorage lr { get; }
        public static RegisterStorage gp { get; }

        public static RegisterStorage sgp0 { get; private set; }
        public static RegisterStorage sgp1 { get; private set; }
        public static RegisterStorage pc { get; private set; }

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
            sp = GpRegs[29];
            fp = GpRegs[30];
            lr = GpRegs[31];
            ModifierRegisters = factory.RangeOfReg32(2, "m{0}");
            PredicateRegisters = factory.RangeOfReg(4, n => $"p{n}", PrimitiveType.Byte);

            var sysfactory = new StorageFactory(StorageDomain.SystemRegister);
            ControlRegisters = GenerateControlRegisters(sysfactory);
            GuestControlRegisters = GenerateGuestControlRegisters(sysfactory);
            SystemRegisters = GenerateSystemRegisters(sysfactory);
            sgp0 = SystemRegisters[0];
            sgp1 = SystemRegisters[1];
            pc = ControlRegisters[9];
            gp = Rename(ControlRegisters, 11, "gp");

            ByName = GpRegs
                .Concat(PredicateRegisters)
                .Concat(ControlRegisters)
                .Concat(SystemRegisters.Values)
                .ToDictionary(k => k.Name);
            ByDomain = ByName.Values
                .ToDictionary(k => k.Domain);
        }

        private static RegisterStorage[] GenerateControlRegisters(StorageFactory sysfactory)
        {
            //return sysfactory.RangeOfReg32(16, "c{0}");

            return new RegisterStorage[32] {
                sysfactory.Reg32("SA0"),   //  Loop start address register 0
                sysfactory.Reg32("LC0"),   //  Loop count register 0
                sysfactory.Reg32("SA1"),   //  Loop start address register 1
                sysfactory.Reg32("LC1"),   //  Loop count register 1

                sysfactory.Reg32("P3:0"),   //  Predicate registers 3:0
                sysfactory.Reg32("reserved{0}"),   //  –
                sysfactory.Reg32("M0"),   //  Modifier register 0
                sysfactory.Reg32("M1"),   //  Modifier register 1

                sysfactory.Reg32("USR"),   //  User status register
                sysfactory.Reg32("PC"),   //  Program counter
                sysfactory.Reg32("UGP"),   //  User general pointer
                sysfactory.Reg32("GP"),   //  Global pointer

                sysfactory.Reg32("CS0"),   //  Circular start register 0
                sysfactory.Reg32("CS1"),   //  Circular start register 1
                sysfactory.Reg32("UPCYCLELO"),   //  Cycle count register (low)
                sysfactory.Reg32("UPCYCLEHI"),  //  Cycle count register (high)
                //sysfactory.Reg32("UPCYCLE"),  // :14 Cycle count register

                sysfactory.Reg32("FRAMELIMIT"), //  Frame limit register
                sysfactory.Reg32("FRAMEKEY"),   //  Frame key register
                sysfactory.Reg32("PKTCOUNTLO"), //  Packet count register (low)
                sysfactory.Reg32("PKTCOUNTHI"), //  Packet count register (high)
                //sysfactory.Reg32("PKTCOUNT"), // :18 Packet count register

                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –

                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –

                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("reserved{0}"),   // 20-29 –
                sysfactory.Reg32("UTIMERLO"),   //  Qtimer register (low)
                sysfactory.Reg32("UTIMERHI"),   //  Qtimer register (high)
                                                // /*UTIMER C31*/:30 Qtimer register
            };
        }

        private static RegisterStorage[] GenerateGuestControlRegisters(StorageFactory sysfactory)
        {
            return sysfactory.RangeOfReg32(32, "g{0}");
        }

        private static Dictionary<uint, RegisterStorage> GenerateSystemRegisters(StorageFactory sysfactory)
        {
            var sysregs = new Dictionary<uint, RegisterStorage>
            {
                { 0, sysfactory.Reg32("sgp0") },
                { 1, sysfactory.Reg32("sgp1") },
                { 2, sysfactory.Reg32("stid") },
                { 3, sysfactory.Reg32("elr") },
                { 4, sysfactory.Reg32("badva0") },
                { 5, sysfactory.Reg32("badva1") },
                { 6, sysfactory.Reg32("ssr") },
                { 7, sysfactory.Reg32("ccr") },
                { 8, sysfactory.Reg32("htid") },
                { 9, sysfactory.Reg32("badva") },
                { 10, sysfactory.Reg32("imask") },

                { 16, sysfactory.Reg32("evb") },
                { 17, sysfactory.Reg32("modectl") },
                { 18, sysfactory.Reg32("syscfg") },
                { 20, sysfactory.Reg32("ipend") },
                { 21, sysfactory.Reg32("vid") },
                { 22, sysfactory.Reg32("iad") },
                { 24, sysfactory.Reg32("iel") },
                { 26, sysfactory.Reg32("iahl") },
                { 27, sysfactory.Reg32("cfgbase") },
                { 28, sysfactory.Reg32("diag") },
                { 29, sysfactory.Reg32("rev") },
                { 30, sysfactory.Reg32("pcyclelo") },
                { 31, sysfactory.Reg32("pcyclehi") },

                { 32, sysfactory.Reg32("isdbst") },
                { 33, sysfactory.Reg32("isdbcfg0") },
                { 34, sysfactory.Reg32("isdbcfg1") },
                { 36, sysfactory.Reg32("brkptpc0") },
                { 37, sysfactory.Reg32("brkptcfg0") },
                { 38, sysfactory.Reg32("brkptpc1") },
                { 39, sysfactory.Reg32("brkptcfg1") },
                { 40, sysfactory.Reg32("isdbmbxin") },
                { 41, sysfactory.Reg32("isdbmbxout") },
                { 42, sysfactory.Reg32("isdben") },
                { 43, sysfactory.Reg32("isdgpr") },

                { 48, sysfactory.Reg32("pmucnt0") },
                { 49, sysfactory.Reg32("pmucnt1") },
                { 50, sysfactory.Reg32("pmucnt2") },
                { 51, sysfactory.Reg32("pmucnt3") },
                { 52, sysfactory.Reg32("pmuevtcfg") },
                { 53, sysfactory.Reg32("pmucfg") },

                { 60, sysfactory.Reg32("reseved60") },
                { 61, sysfactory.Reg32("reseved61") },
                { 62, sysfactory.Reg32("reseved62") },
                { 63, sysfactory.Reg32("reseved63") },
            };
            return sysregs;
        }

        private static RegisterStorage Rename(RegisterStorage[] regs, int index, string newName)
        {
            var regOld = regs[index];
            var regNew = new RegisterStorage(newName, regOld.Number, (uint)regOld.BitAddress, (PrimitiveType) regOld.DataType);
            regs[index] = regNew;
            return regNew;
        }
    }
}