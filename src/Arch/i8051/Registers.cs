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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.i8051
{
    public static class Registers
    {
        public static RegisterStorage R0 = RegisterStorage.Reg8("R0", 0x00);
        public static RegisterStorage R1 = RegisterStorage.Reg8("R1", 0x01);
        public static RegisterStorage R2 = RegisterStorage.Reg8("R2", 0x02);
        public static RegisterStorage R3 = RegisterStorage.Reg8("R3", 0x03);
        public static RegisterStorage R4 = RegisterStorage.Reg8("R4", 0x04);
        public static RegisterStorage R5 = RegisterStorage.Reg8("R5", 0x05);
        public static RegisterStorage R6 = RegisterStorage.Reg8("R6", 0x06);
        public static RegisterStorage R7 = RegisterStorage.Reg8("R7", 0x07);

        public static RegisterStorage R8 = RegisterStorage.Reg8("R8", 0x08);
        public static RegisterStorage R9 = RegisterStorage.Reg8("R9", 0x09);
        public static RegisterStorage R10 = RegisterStorage.Reg8("R10", 0x0A);
        public static RegisterStorage R11 = RegisterStorage.Reg8("R11", 0x0B);
        public static RegisterStorage R12 = RegisterStorage.Reg8("R12", 0x0C);
        public static RegisterStorage R13 = RegisterStorage.Reg8("R13", 0x0D);
        public static RegisterStorage R14 = RegisterStorage.Reg8("R14", 0x0E);
        public static RegisterStorage R15 = RegisterStorage.Reg8("R15", 0x0F);

        public static RegisterStorage R16 = RegisterStorage.Reg8("R16", 0x10);
        public static RegisterStorage R17 = RegisterStorage.Reg8("R17", 0x11);
        public static RegisterStorage R18 = RegisterStorage.Reg8("R18", 0x12);
        public static RegisterStorage R19 = RegisterStorage.Reg8("R19", 0x13);
        public static RegisterStorage R20 = RegisterStorage.Reg8("R20", 0x14);
        public static RegisterStorage R21 = RegisterStorage.Reg8("R21", 0x15);
        public static RegisterStorage R22 = RegisterStorage.Reg8("R22", 0x16);
        public static RegisterStorage R23 = RegisterStorage.Reg8("R23", 0x17);

        public static RegisterStorage R24 = RegisterStorage.Reg8("R24", 0x18);
        public static RegisterStorage R25 = RegisterStorage.Reg8("R25", 0x19);
        public static RegisterStorage R26 = RegisterStorage.Reg8("R26", 0x1A);
        public static RegisterStorage R27 = RegisterStorage.Reg8("R27", 0x1B);
        public static RegisterStorage R28 = RegisterStorage.Reg8("R28", 0x1C);
        public static RegisterStorage R29 = RegisterStorage.Reg8("R29", 0x1D);
        public static RegisterStorage R30 = RegisterStorage.Reg8("R30", 0x1E);
        public static RegisterStorage R31 = RegisterStorage.Reg8("R31", 0x1F);

        // Special function registers.

        public static readonly RegisterStorage P0 = RegisterStorage.Reg8("P0", 0x80);
        public static readonly RegisterStorage SP = RegisterStorage.Reg8("SP", 0x81);
        public static readonly RegisterStorage DPL = RegisterStorage.Reg8("DPL", 0x82);
        public static readonly RegisterStorage DPH = RegisterStorage.Reg8("DPH", 0x83);
        public static readonly SequenceStorage DPTR = new SequenceStorage("DPTR", PrimitiveType.Word16, DPH, DPL);
        public static readonly RegisterStorage PCON = RegisterStorage.Reg8("PCON", 0x87);
        public static readonly RegisterStorage TCON = RegisterStorage.Reg8("TCON", 0x88);
        public static readonly RegisterStorage TMOD = RegisterStorage.Reg8("TMOD", 0x89);
        public static readonly RegisterStorage TL0 = RegisterStorage.Reg8("TL0", 0x8A);
        public static readonly RegisterStorage TH0 = RegisterStorage.Reg8("TH0", 0x8C);
        public static readonly SequenceStorage T0 = new SequenceStorage("T0", PrimitiveType.Word16, TH0, TL0);
        public static readonly RegisterStorage TL1 = RegisterStorage.Reg8("TL1", 0x8B);
        public static readonly RegisterStorage TH1 = RegisterStorage.Reg8("TH1", 0x8D);
        public static readonly SequenceStorage T1 = new SequenceStorage("T1", PrimitiveType.Word16, TH1, TL1);
        public static readonly RegisterStorage P1 = RegisterStorage.Reg8("P1", 0x90);
        public static readonly RegisterStorage SCON = RegisterStorage.Reg8("SCON", 0x98);
        public static readonly RegisterStorage SBUF = RegisterStorage.Reg8("SBUF", 0x99);
        public static readonly RegisterStorage P2 = RegisterStorage.Reg8("P2", 0xA0);
        public static readonly RegisterStorage IE = RegisterStorage.Reg8("IE", 0xA8);
        public static readonly RegisterStorage P3 = RegisterStorage.Reg8("P3", 0xB0);
        public static readonly RegisterStorage IP = RegisterStorage.Reg8("IP", 0xB8);
        public static readonly RegisterStorage PSW = RegisterStorage.Reg8("PSW", 0xD0);
        public static readonly RegisterStorage A = RegisterStorage.Reg8("A", 0xE0);
        public static readonly RegisterStorage B = RegisterStorage.Reg8("B", 0xF0);
        public static readonly SequenceStorage AB = new SequenceStorage("AB", PrimitiveType.Word16, A, B);

        public static readonly RegisterStorage PC = new RegisterStorage("PC", 0x100, 0, PrimitiveType.Ptr16);


        public static readonly FlagGroupStorage CFlag = new FlagGroupStorage(PSW, (uint) FlagM.C, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage AFlag = new FlagGroupStorage(PSW, (uint) FlagM.AC, "A", PrimitiveType.Bool);
        public static readonly FlagGroupStorage OFlag = new FlagGroupStorage(PSW, (uint) FlagM.OV, "O", PrimitiveType.Bool);
        public static readonly FlagGroupStorage PFlag = new FlagGroupStorage(PSW, (uint) FlagM.P, "P", PrimitiveType.Bool);
        public static readonly FlagGroupStorage CAOP = new FlagGroupStorage(PSW, (uint) (FlagM.C | FlagM.AC | FlagM.OV | FlagM.P), "CAOP", PrimitiveType.Byte);
        


        private static Dictionary<int, RegisterStorage> regsByNumber;
        private static Dictionary<StorageDomain, RegisterStorage> regsByDomain;
        private static Dictionary<string, RegisterStorage> regsByName;

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
            regsByName = regsByNumber.Values
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
            return regsByName.TryGetValue(regName, out reg);
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
