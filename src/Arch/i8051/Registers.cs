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
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.i8051
{
    public static class Registers
    {
        public static RegisterStorage R0 = new RegisterStorage("R0", 0x00, 0, PrimitiveType.Byte);
        public static RegisterStorage R1 = new RegisterStorage("R1", 0x01, 0, PrimitiveType.Byte);
        public static RegisterStorage R2 = new RegisterStorage("R2", 0x02, 0, PrimitiveType.Byte);
        public static RegisterStorage R3 = new RegisterStorage("R3", 0x03, 0, PrimitiveType.Byte);
        public static RegisterStorage R4 = new RegisterStorage("R4", 0x04, 0, PrimitiveType.Byte);
        public static RegisterStorage R5 = new RegisterStorage("R5", 0x05, 0, PrimitiveType.Byte);
        public static RegisterStorage R6 = new RegisterStorage("R6", 0x06, 0, PrimitiveType.Byte);
        public static RegisterStorage R7 = new RegisterStorage("R7", 0x07, 0, PrimitiveType.Byte);

        public static RegisterStorage R8 = new RegisterStorage("R8", 0x08, 0, PrimitiveType.Byte);
        public static RegisterStorage R9 = new RegisterStorage("R9", 0x09, 0, PrimitiveType.Byte);
        public static RegisterStorage R10 = new RegisterStorage("R10", 0x0A, 0, PrimitiveType.Byte);
        public static RegisterStorage R11 = new RegisterStorage("R11", 0x0B, 0, PrimitiveType.Byte);
        public static RegisterStorage R12 = new RegisterStorage("R12", 0x0C, 0, PrimitiveType.Byte);
        public static RegisterStorage R13 = new RegisterStorage("R13", 0x0D, 0, PrimitiveType.Byte);
        public static RegisterStorage R14 = new RegisterStorage("R14", 0x0E, 0, PrimitiveType.Byte);
        public static RegisterStorage R15 = new RegisterStorage("R15", 0x0F, 0, PrimitiveType.Byte);

        public static RegisterStorage R16 = new RegisterStorage("R16", 0x10, 0, PrimitiveType.Byte);
        public static RegisterStorage R17 = new RegisterStorage("R17", 0x11, 0, PrimitiveType.Byte);
        public static RegisterStorage R18 = new RegisterStorage("R18", 0x12, 0, PrimitiveType.Byte);
        public static RegisterStorage R19 = new RegisterStorage("R19", 0x13, 0, PrimitiveType.Byte);
        public static RegisterStorage R20 = new RegisterStorage("R20", 0x14, 0, PrimitiveType.Byte);
        public static RegisterStorage R21 = new RegisterStorage("R21", 0x15, 0, PrimitiveType.Byte);
        public static RegisterStorage R22 = new RegisterStorage("R22", 0x16, 0, PrimitiveType.Byte);
        public static RegisterStorage R23 = new RegisterStorage("R23", 0x17, 0, PrimitiveType.Byte);

        public static RegisterStorage R24 = new RegisterStorage("R24", 0x18, 0, PrimitiveType.Byte);
        public static RegisterStorage R25 = new RegisterStorage("R25", 0x19, 0, PrimitiveType.Byte);
        public static RegisterStorage R26 = new RegisterStorage("R26", 0x1A, 0, PrimitiveType.Byte);
        public static RegisterStorage R27 = new RegisterStorage("R27", 0x1B, 0, PrimitiveType.Byte);
        public static RegisterStorage R28 = new RegisterStorage("R28", 0x1C, 0, PrimitiveType.Byte);
        public static RegisterStorage R29 = new RegisterStorage("R29", 0x1D, 0, PrimitiveType.Byte);
        public static RegisterStorage R30 = new RegisterStorage("R30", 0x1E, 0, PrimitiveType.Byte);
        public static RegisterStorage R31 = new RegisterStorage("R31", 0x1F, 0, PrimitiveType.Byte);

        // Special function registers.

        public static RegisterStorage P0 = new RegisterStorage("P0", 0x80, 0, PrimitiveType.Byte);
        public static RegisterStorage SP = new RegisterStorage("SP", 0x81, 0, PrimitiveType.Byte);
        public static RegisterStorage DPL = new RegisterStorage("DPL", 0x82, 0, PrimitiveType.Byte);
        public static RegisterStorage DPH = new RegisterStorage("DPH", 0x83, 0, PrimitiveType.Byte);
        public static SequenceStorage DPTR = new SequenceStorage("DPTR", PrimitiveType.Word16, DPH, DPL);
        public static RegisterStorage PCON = new RegisterStorage("PCON", 0x87, 0, PrimitiveType.Byte);
        public static RegisterStorage TCON = new RegisterStorage("TCON", 0x88, 0, PrimitiveType.Byte);
        public static RegisterStorage TMOD = new RegisterStorage("TMOD", 0x89, 0, PrimitiveType.Byte);
        public static RegisterStorage TL0 = new RegisterStorage("TL0", 0x8A, 0, PrimitiveType.Byte);
        public static RegisterStorage TH0 = new RegisterStorage("TH0", 0x8C, 0, PrimitiveType.Byte);
        public static SequenceStorage T0 = new SequenceStorage("T0", PrimitiveType.Word16, TH0, TL0);
        public static RegisterStorage TL1 = new RegisterStorage("TL1", 0x8B, 0, PrimitiveType.Byte);
        public static RegisterStorage TH1 = new RegisterStorage("TH1", 0x8D, 0, PrimitiveType.Byte);
        public static SequenceStorage T1 = new SequenceStorage("T1", PrimitiveType.Word16, TH1, TL1);
        public static RegisterStorage P1 = new RegisterStorage("P1", 0x90, 0, PrimitiveType.Byte);
        public static RegisterStorage SCON = new RegisterStorage("SCON", 0x98, 0, PrimitiveType.Byte);
        public static RegisterStorage SBUF = new RegisterStorage("SBUF", 0x99, 0, PrimitiveType.Byte);
        public static RegisterStorage P2 = new RegisterStorage("P2", 0xA0, 0, PrimitiveType.Byte);
        public static RegisterStorage IE = new RegisterStorage("IE", 0xA8, 0, PrimitiveType.Byte);
        public static RegisterStorage P3 = new RegisterStorage("P2", 0xB0, 0, PrimitiveType.Byte);
        public static RegisterStorage IP = new RegisterStorage("IP", 0xB8, 0, PrimitiveType.Byte);
        public static RegisterStorage PSW = new RegisterStorage("PSW", 0xD0, 0, PrimitiveType.Byte);
        public static RegisterStorage A = new RegisterStorage("A", 0xE0, 0, PrimitiveType.Byte);
        public static RegisterStorage B = new RegisterStorage("B", 0xF0, 0, PrimitiveType.Byte);
        public static SequenceStorage AB = new SequenceStorage("AB", PrimitiveType.Word16, A, B);

        public static RegisterStorage PC = new RegisterStorage("PC", 0x100, 0, PrimitiveType.Ptr16);


        public static FlagGroupStorage CFlag = new FlagGroupStorage(PSW, (uint) FlagM.C, "C", PrimitiveType.Bool);
        public static FlagGroupStorage AFlag = new FlagGroupStorage(PSW, (uint) FlagM.AC, "A", PrimitiveType.Bool);
        public static FlagGroupStorage OFlag = new FlagGroupStorage(PSW, (uint) FlagM.OV, "O", PrimitiveType.Bool);
        public static FlagGroupStorage PFlag = new FlagGroupStorage(PSW, (uint) FlagM.P, "P", PrimitiveType.Bool);

        private static Dictionary<int, RegisterStorage> regsByNumber;
        private static Dictionary<StorageDomain, RegisterStorage> regsByDomain;

        static Registers()
        {
            regsByNumber = new[] {
                R0,  R1,  R2,  R3,  R4,  R5,  R6,  R7,
                R8,  R9,  R10, R11, R12, R13, R14, R15,
                R16, R17, R18, R19, R20, R21, R22, R23, 
                R24, R25, R26, R27, R28, R29, R30, R31,

                P0,
                SP,
                DPL,
                DPH,
                PCON,
                TCON,
                TMOD,
                TL0,
                TH0,
                TL1,
                TH1,
                P1,
                SCON,
                SBUF,
                P2,
                IE,
                P3,
                IP,
                PSW,
                A,
                B,
            }.ToDictionary(k  => k.Number);
            regsByDomain = regsByNumber.Values
                .ToDictionary(k => k.Domain);
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
