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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.i8051
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters { get; }

        // Special function registers.

        public static RegisterStorage P0 { get; }
        public static RegisterStorage SP { get; }
        public static RegisterStorage DPL { get; }
        public static RegisterStorage DPH { get; }
        public static RegisterStorage DPTR { get; }
        public static RegisterStorage PCON { get; }
        public static RegisterStorage TCON { get; }
        public static RegisterStorage TMOD { get; }
        public static RegisterStorage TL0 { get; }
        public static RegisterStorage TH0 { get; }
        public static RegisterStorage T0 { get; }
        public static RegisterStorage TL1 { get; }
        public static RegisterStorage TH1 { get; }
        public static RegisterStorage T1 { get; }
        public static RegisterStorage P1 { get; }
        public static RegisterStorage SCON { get; }
        public static RegisterStorage SBUF { get; }
        public static RegisterStorage P2 { get; }
        public static RegisterStorage IE { get; }
        public static RegisterStorage P3 { get; }
        public static RegisterStorage IP { get; }
        public static RegisterStorage PSW { get; }
        public static RegisterStorage A { get; }
        public static RegisterStorage B { get; }
        public static RegisterStorage AB { get; }

        public static RegisterStorage PC { get; }


        public static FlagGroupStorage CFlag { get; }
        public static FlagGroupStorage AFlag { get; }
        public static FlagGroupStorage OFlag { get; }
        public static FlagGroupStorage PFlag { get; }
        public static FlagGroupStorage CAOP  { get; }
        


        private static Dictionary<int, RegisterStorage> regsByNumber;
        public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
        public static Dictionary<string, RegisterStorage> ByName { get; }
        public static Dictionary<StorageDomain, RegisterStorage[]> Subregisters { get; }


        static Registers()
        {
            var factory = new StorageFactory();
            GpRegisters = Enumerable.Range(0, 32)
                .Select(i => RegisterStorage.Reg8($"R{i}", i))
                .ToArray();

            var ctrFactory = new StorageFactory(StorageDomain.SystemRegister);
            P0 = ctrFactory.Reg8("P0");
            SP = ctrFactory.Reg8("SP");
            DPTR = ctrFactory.Reg16("DPTR");
            PCON = ctrFactory.Reg8("PCON");
            TCON = ctrFactory.Reg8("TCON");
            TMOD = ctrFactory.Reg8("TMOD");
            TL0 = ctrFactory.Reg8("TL0");
            TH0 = ctrFactory.Reg8("TH0");
            T0 = ctrFactory.Reg16("T0");
            T1 = ctrFactory.Reg16("T1");
            P1 = ctrFactory.Reg8("P1");
            SCON = ctrFactory.Reg8("SCON");
            SBUF = ctrFactory.Reg8("SBUF");
            P2 = ctrFactory.Reg8("P2");
            IE = ctrFactory.Reg8("IE");
            P3 = ctrFactory.Reg8("P3");
            IP = ctrFactory.Reg8("IP");
            PSW = ctrFactory.Reg8("PSW");
            AB = ctrFactory.Reg16("AB");

            DPL = RegisterStorage.Reg8("DPL", DPTR.Number, 0);
            DPH = RegisterStorage.Reg8("DPH", DPTR.Number, 8);
            TL0 = RegisterStorage.Reg8("TL0", T1.Number, 0);
            TH0 = RegisterStorage.Reg8("TH0", T1.Number, 8);
            TL1 = RegisterStorage.Reg8("TL1", T1.Number, 0);
            TH1 = RegisterStorage.Reg8("TH1", T1.Number, 8);
            A = RegisterStorage.Reg8("A", AB.Number, 8);
            B = RegisterStorage.Reg8("B", AB.Number, 0);

            Subregisters = new()
            {
                { DPTR.Domain, new[] { DPL, DPH }  },
                { T0.Domain, new[] { TL0, TH0 } },
                { T1.Domain, new[] { TL1, TH1 } },
                { AB.Domain, new[] { A, B } },
            };


            PC = new RegisterStorage("PC", 0x100, 0, PrimitiveType.Ptr16);
            var r = GpRegisters;
            regsByNumber = new Dictionary<int, RegisterStorage> {
                { 0x00, r[0] },
                { 0x01, r[1] },
                { 0x02, r[2] },
                { 0x03, r[3] },
                { 0x04, r[4] },
                { 0x05, r[5] },
                { 0x06, r[6] },
                { 0x07, r[7] },

                { 0x08, r[8] },
                { 0x09, r[9] },
                { 0x0A, r[10] },
                { 0x0B, r[11] },
                { 0x0C, r[12] },
                { 0x0D, r[13] },
                { 0x0E, r[14] },
                { 0x0F, r[15] },

                { 0x10, r[16] },
                { 0x11, r[17] },
                { 0x12, r[18] },
                { 0x13, r[19] },
                { 0x14, r[20] },
                { 0x15, r[21] },
                { 0x16, r[22] },
                { 0x17, r[23] },

                { 0x18, r[24] },
                { 0x19, r[25] },
                { 0x1A, r[26] },
                { 0x1B, r[27] },
                { 0x1C, r[28] },
                { 0x1D, r[29] },
                { 0x1E, r[30] },
                { 0x1F, r[31] },

                // Special function registers.

                { 0x80, P0 },
                { 0x81, SP },
                { 0x82, DPL },
                { 0x83, DPH },
                { 0x87, PCON },
                { 0x88, TCON },
                { 0x89, TMOD },
                { 0x8A, TL0 },
                { 0x8C, TH0 },
                { 0x8B, TL1 },
                { 0x8D, TH1 },
                { 0x90, P1 },
                { 0x98, SCON },
                { 0x99, SBUF },
                { 0xA0, P2 },
                { 0xA8, IE },
                { 0xB0, P3 },
                { 0xB8, IP },
                { 0xD0, PSW },
                { 0xE0, A },
                { 0xF0, B },

            };

            CFlag = new FlagGroupStorage(PSW, (uint) FlagM.C, "C");
            AFlag = new FlagGroupStorage(PSW, (uint) FlagM.AC, "A");
            OFlag = new FlagGroupStorage(PSW, (uint) FlagM.OV, "O");
            PFlag = new FlagGroupStorage(PSW, (uint) FlagM.P, "P");
            CAOP = new FlagGroupStorage(PSW, (uint) (FlagM.C | FlagM.AC | FlagM.OV | FlagM.P), nameof(CAOP));

            ByDomain = factory.DomainsToRegisters.Values
                .Concat(ctrFactory.DomainsToRegisters.Values)
                .ToDictionary(k => k.Domain);
            ByName = regsByNumber.Values
                .ToDictionary(k => k.Name);
        }

        public static RegisterStorage GetRegister(int i)
        {
            if (regsByNumber.TryGetValue(i, out var reg))
                return reg;
            reg = new RegisterStorage($"SFR{i:X2}", i, 0, PrimitiveType.Byte);
            regsByNumber.Add(i, reg);
            return reg;
        }

        public static RegisterStorage[] GetRegisters()
        {
            return regsByNumber.Values.OrderBy(r => r.Number).ToArray();
        }

        public static bool TryGetRegister(string regName, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return ByName.TryGetValue(regName, out reg);
        }
    }

    [Flags]
    public enum FlagM
    {
        C = 0x80,
        AC  = 0x40,
        F0 = 0x20,
        RS1 = 0x10,
        RS0 = 0x08,
        OV = 0x04,
        P = 0x1,
    }
}
