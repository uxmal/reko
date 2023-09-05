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
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public static class Registers
    {
        public static RegisterStorage a { get; }
        public static RegisterStorage usp { get; }
        public static RegisterStorage ssp { get; }
        public static RegisterStorage ps { get; }
        public static RegisterStorage pc { get; }
        public static RegisterStorage pcb { get; }
        public static RegisterStorage dtb { get; }
        public static RegisterStorage usb { get; }
        public static RegisterStorage ssb { get; }
        public static RegisterStorage adb { get; }
        public static RegisterStorage al { get; }
        public static RegisterStorage ah { get; }

        public static RegisterStorage ccr { get; }
        public static RegisterStorage rp { get; }
        public static RegisterStorage ilm { get; }

        public static RegisterStorage dpr { get; }
        public static RegisterStorage cmr { get; }
        public static RegisterStorage ncc { get; }

        public static RegisterStorage[] rl { get; }
        public static RegisterStorage[] rw { get; }
        public static RegisterStorage[] r { get; }

        public static FlagGroupStorage C { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            a = factory.Reg32("a");
            usp = factory.Reg16("usp"); // usb
            ssp = factory.Reg16("ssp"); // ssb
            ps = factory.Reg16("ps");
            pc = factory.Reg16("pc");
            pcb = factory.Reg("pcb", PrimitiveType.Byte);
            dtb = factory.Reg("dtb", PrimitiveType.Byte);
            usb = factory.Reg("usb", PrimitiveType.Byte);
            ssb = factory.Reg("ssb", PrimitiveType.Byte);
            adb = factory.Reg("adb", PrimitiveType.Byte);
            dpr = factory.Reg("dpr", PrimitiveType.Byte);
            cmr = factory.Reg("cmr", PrimitiveType.Byte);   // Pseudo-register
            ncc = factory.Reg("ncc", PrimitiveType.Byte);   // Pseudo-register

            rl = factory.RangeOfReg32(4, "rl{0}");
            rw = Enumerable.Range(0, 4).SelectMany(i => new[]
            {
                RegisterStorage.Reg16($"rw{i*2}", i, 0),
                RegisterStorage.Reg16($"rw{i*2+1}", i, 16),
            }).ToArray();
            r = Enumerable.Range(2, 2).SelectMany(i =>
            {
                int iReg = (i - 2) * 4;
                return new[]
                {
                    RegisterStorage.Reg8($"r{iReg}",     i, 0),
                    RegisterStorage.Reg8($"r{iReg + 1}", i, 8),
                    RegisterStorage.Reg8($"r{iReg + 2}", i, 16),
                    RegisterStorage.Reg8($"r{iReg + 3}", i, 24),
                };
            }).ToArray();

            al = RegisterStorage.Reg16("al", a.Number);
            ah = RegisterStorage.Reg16("ah", a.Number, 16);

            var w3 = PrimitiveType.CreateWord(3);
            var w5 = PrimitiveType.CreateWord(5);

            ccr = RegisterStorage.Reg8("ccr", ps.Number);
            rp = new RegisterStorage("rp", ps.Number, 8, w5);
            ilm = new RegisterStorage("ilm", ps.Number, 13, w3);

            var ioPortFactory = new StorageFactory(StorageDomain.SystemRegister);
            var ioPorts = new Dictionary<int, RegisterStorage>
            {
                { 0x01, ioPortFactory.Reg("PDR1", PrimitiveType.Byte) },
                { 0x02, ioPortFactory.Reg("PDR2", PrimitiveType.Byte) },
                { 0x03, ioPortFactory.Reg("PDR3", PrimitiveType.Byte) },
                { 0x04, ioPortFactory.Reg("PDR4", PrimitiveType.Byte) },
                { 0x05, ioPortFactory.Reg("PDR5", PrimitiveType.Byte) },
                { 0x06, ioPortFactory.Reg("PDR6", PrimitiveType.Byte) },
                { 0x07, ioPortFactory.Reg("PDR7", PrimitiveType.Byte) },
                { 0x08, ioPortFactory.Reg("PDR8", PrimitiveType.Byte) },
                { 0x09, ioPortFactory.Reg("PDR9", PrimitiveType.Byte) },
                { 0x0A, ioPortFactory.Reg("PDRA", PrimitiveType.Byte) },
                { 0x11, ioPortFactory.Reg("DDR1", PrimitiveType.Byte) },
                { 0x12, ioPortFactory.Reg("DDR2", PrimitiveType.Byte) },
                { 0x13, ioPortFactory.Reg("DDR3", PrimitiveType.Byte) },
                { 0x14, ioPortFactory.Reg("DDR4", PrimitiveType.Byte) },
                { 0x15, ioPortFactory.Reg("DDR5", PrimitiveType.Byte) },
                { 0x16, ioPortFactory.Reg("ADER", PrimitiveType.Byte) },
                { 0x17, ioPortFactory.Reg("DDR7", PrimitiveType.Byte) },
                { 0x18, ioPortFactory.Reg("DDR8", PrimitiveType.Byte) },
                { 0x19, ioPortFactory.Reg("DDR9", PrimitiveType.Byte) },
                { 0x1A, ioPortFactory.Reg("DDRA", PrimitiveType.Byte) },
                { 0x20, ioPortFactory.Reg("SMR0", PrimitiveType.Byte) },
                { 0x21, ioPortFactory.Reg("SCR0", PrimitiveType.Byte) },
                { 0x22, ioPortFactory.Reg("S[IO]DR0", PrimitiveType.Byte) },
                { 0x23, ioPortFactory.Reg("SSR0", PrimitiveType.Byte) },
                { 0x24, ioPortFactory.Reg("SMR1", PrimitiveType.Byte) },
                { 0x25, ioPortFactory.Reg("SCR1", PrimitiveType.Byte) },
                { 0x26, ioPortFactory.Reg("S[IO]DR1", PrimitiveType.Byte) },
                { 0x27, ioPortFactory.Reg("SSR1", PrimitiveType.Byte) },
                { 0x28, ioPortFactory.Reg("ENIR", PrimitiveType.Byte) },
                { 0x29, ioPortFactory.Reg("EIRR", PrimitiveType.Byte) },
                { 0x2A, ioPortFactory.Reg("ELVRl", PrimitiveType.Byte) },
                { 0x2B, ioPortFactory.Reg("ELVRh", PrimitiveType.Byte) },
                { 0x2C, ioPortFactory.Reg("ADCSl", PrimitiveType.Byte) },
                { 0x2D, ioPortFactory.Reg("ADCSh", PrimitiveType.Byte) },
                { 0x2E, ioPortFactory.Reg("ADCRl", PrimitiveType.Byte) },
                { 0x2F, ioPortFactory.Reg("ADCRh", PrimitiveType.Byte) },
                { 0x30, ioPortFactory.Reg("PPGC0", PrimitiveType.Byte) },
                { 0x31, ioPortFactory.Reg("PPGC1", PrimitiveType.Byte) },
                { 0x34, ioPortFactory.Reg("PRL0l", PrimitiveType.Byte) },
                { 0x35, ioPortFactory.Reg("PRL0h", PrimitiveType.Byte) },
                { 0x36, ioPortFactory.Reg("PRL1l", PrimitiveType.Byte) },
                { 0x37, ioPortFactory.Reg("PRL1h", PrimitiveType.Byte) },
                { 0x38, ioPortFactory.Reg("TMCSR0l", PrimitiveType.Byte) },
                { 0x39, ioPortFactory.Reg("TMCSR0h", PrimitiveType.Byte) },
                { 0x3A, ioPortFactory.Reg("TMR(LR)0l", PrimitiveType.Byte) },
                { 0x3B, ioPortFactory.Reg("TMR(LR)0h", PrimitiveType.Byte) },
                { 0x3C, ioPortFactory.Reg("TMCSR1l", PrimitiveType.Byte) },
                { 0x3D, ioPortFactory.Reg("TMCSR1h", PrimitiveType.Byte) },
                { 0x3E, ioPortFactory.Reg("TMR(LR)1l", PrimitiveType.Byte) },
                { 0x3F, ioPortFactory.Reg("TMR(LR)1h", PrimitiveType.Byte) },
                { 0x44, ioPortFactory.Reg("SMR2", PrimitiveType.Byte) },
                { 0x45, ioPortFactory.Reg("SCR2", PrimitiveType.Byte) },
                { 0x46, ioPortFactory.Reg("S[IO]DR2", PrimitiveType.Byte) },
                { 0x47, ioPortFactory.Reg("SSR2", PrimitiveType.Byte) },
                { 0x48, ioPortFactory.Reg("CSCR0", PrimitiveType.Byte) },
                { 0x49, ioPortFactory.Reg("CSCR1", PrimitiveType.Byte) },
                { 0x4A, ioPortFactory.Reg("CSCR2", PrimitiveType.Byte) },
                { 0x4B, ioPortFactory.Reg("CSCR3", PrimitiveType.Byte) },
                { 0x4C, ioPortFactory.Reg("CSCR4", PrimitiveType.Byte) },
                { 0x4D, ioPortFactory.Reg("CSCR5", PrimitiveType.Byte) },
                { 0x4E, ioPortFactory.Reg("CSCR6", PrimitiveType.Byte) },
                { 0x4F, ioPortFactory.Reg("CSCR7", PrimitiveType.Byte) },
                { 0x51, ioPortFactory.Reg("CDCR0", PrimitiveType.Byte) },
                { 0x53, ioPortFactory.Reg("CDCR1", PrimitiveType.Byte) },
                { 0x9F, ioPortFactory.Reg("DIRR", PrimitiveType.Byte) },
                { 0xA0, ioPortFactory.Reg("LPMCR", PrimitiveType.Byte) },
                { 0xA1, ioPortFactory.Reg("CKSCR", PrimitiveType.Byte) },
                { 0xA5, ioPortFactory.Reg("ARSR", PrimitiveType.Byte) },
                { 0xA6, ioPortFactory.Reg("HACR", PrimitiveType.Byte) },
                { 0xA7, ioPortFactory.Reg("ECSR", PrimitiveType.Byte) },
                { 0xA8, ioPortFactory.Reg("WDTC", PrimitiveType.Byte) },
                { 0xA9, ioPortFactory.Reg("TBTC", PrimitiveType.Byte) },
                { 0xB0, ioPortFactory.Reg("ICR00", PrimitiveType.Byte) },
                { 0xB1, ioPortFactory.Reg("ICR01", PrimitiveType.Byte) },
                { 0xB2, ioPortFactory.Reg("ICR02", PrimitiveType.Byte) },
                { 0xB3, ioPortFactory.Reg("ICR03", PrimitiveType.Byte) },
                { 0xB4, ioPortFactory.Reg("ICR04", PrimitiveType.Byte) },
                { 0xB5, ioPortFactory.Reg("ICR05", PrimitiveType.Byte) },
                { 0xB6, ioPortFactory.Reg("ICR06", PrimitiveType.Byte) },
                { 0xB7, ioPortFactory.Reg("ICR07", PrimitiveType.Byte) },
                { 0xB8, ioPortFactory.Reg("ICR08", PrimitiveType.Byte) },
                { 0xB9, ioPortFactory.Reg("ICR09", PrimitiveType.Byte) },
                { 0xBA, ioPortFactory.Reg("ICR10", PrimitiveType.Byte) },
                { 0xBB, ioPortFactory.Reg("ICR11", PrimitiveType.Byte) },
                { 0xBC, ioPortFactory.Reg("ICR12", PrimitiveType.Byte) },
                { 0xBD, ioPortFactory.Reg("ICR13", PrimitiveType.Byte) },
                { 0xBE, ioPortFactory.Reg("ICR14", PrimitiveType.Byte) },
                { 0xBF, ioPortFactory.Reg("ICR15", PrimitiveType.Byte) },
                // Specific to LC2412
                { 0xC0, ioPortFactory.Reg("KEY0", PrimitiveType.Byte) },
                { 0xC1, ioPortFactory.Reg("KEY1", PrimitiveType.Byte) },
                { 0xC2, ioPortFactory.Reg("KEY2", PrimitiveType.Byte) },
                { 0xC3, ioPortFactory.Reg("KEY3", PrimitiveType.Byte) },
                { 0xC4, ioPortFactory.Reg("KEY4", PrimitiveType.Byte) },
                { 0xC5, ioPortFactory.Reg("LEDC", PrimitiveType.Byte) },
                { 0xC6, ioPortFactory.Reg("LCD", PrimitiveType.Byte) },
                { 0xC8, ioPortFactory.Reg("PC_ADL", PrimitiveType.Byte) },
                { 0xC9, ioPortFactory.Reg("PC_ADM", PrimitiveType.Byte) },
                { 0xCA, ioPortFactory.Reg("PC_ADH", PrimitiveType.Byte) },
                { 0xCB, ioPortFactory.Reg("PC_DW", PrimitiveType.Byte) },
                { 0xCC, ioPortFactory.Reg("PC_DR", PrimitiveType.Byte) },
                { 0xCD, ioPortFactory.Reg("PC_CR", PrimitiveType.Byte) },
            };
            C = new FlagGroupStorage(ccr, (uint) FlagM.C, "C", PrimitiveType.Bool);
        }
    }

    [Flags]
    public enum FlagM
    {
        C = 0x01,
        V = 0x02,
        Z = 0x04,
        N = 0x08,
        T = 0x10,
        S = 0x20,
        I = 0x40,
        P = 0x80,
    }
}
