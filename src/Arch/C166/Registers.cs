#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.C166
{
    public static class Registers
    {
        public static readonly RegisterStorage[] GpRegs;
        public static readonly RegisterStorage[] ByteRegs;
        public static readonly RegisterStorage[] LoByteRegs;
        public static readonly RegisterStorage[] HiByteRegs;
        public static readonly Dictionary<int, RegisterStorage> SpecialFunctionRegs;
        public static readonly RegisterStorage PSW;
        public static readonly RegisterStorage SP;
        public static readonly Dictionary<StorageDomain, RegisterStorage> ByDomain;

        public static readonly FlagGroupStorage E;
        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage N;

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg(16, n => $"r{n}", PrimitiveType.Word16);
            LoByteRegs = GpRegs.Take(8)
                .Select(r => RegisterStorage.Reg8($"rl{r.Number}", r.Number, 0))
                .ToArray();
            HiByteRegs = GpRegs.Take(8)
                .Select(r => RegisterStorage.Reg8($"rh{r.Number}", r.Number, 8))
                .ToArray();
            ByteRegs = Enumerable.Range(0, 8)
                .SelectMany(i => new[] { LoByteRegs[i], HiByteRegs[i] })
                .ToArray();

            static RegisterStorage Sfr(string name, int regNo, string description) =>
                RegisterStorage.Reg16(name, regNo | (int)StorageDomain.SystemRegister, 0);

            PSW = Sfr("PSW", 0xFF10, "CPU Program Status Word 0000");
            SP = Sfr("SP", 0xFE12, "CPU System Stack Pointer Register FC00");

            SpecialFunctionRegs = new Dictionary<int, RegisterStorage>
            {
                { 0xFF98, Sfr("ADCIC", 0xFF98, "A/D Converter End of Conversion Interrupt Control Register") },
                { 0xFFA0, Sfr("ADCON", 0xFFA0, "A/D Converter Control Register") },
                { 0xFEA0, Sfr("ADDAT", 0xFEA0, "A/D Converter Result Register") },
                { 0xFE18, Sfr("ADDRSEL1", 0xFE18, "Address Select Register 1 0000") },
                { 0xFF9A, Sfr("ADEIC", 0xFF9A, "A/D Converter Overrun Error Interrupt Control Register 0000") },
                { 0xFF14, Sfr("BUSCON1", 0xFF14, "Bus Configuration Register 1 0000") },
                { 0xFE4A, Sfr("CAPREL", 0xFE4A, "GPT2 Capture/Reload Register 0000") },
                { 0xFE80, Sfr("CC0", 0xFE80, "CAPCOM Register 0 0000") },
                { 0xFF78, Sfr("CC0IC", 0xFF78, "CAPCOM Register 0 Interrupt Control Register 0000") },
                { 0xFE82, Sfr("CC1", 0xFE82, "CAPCOM Register 1 0000") },
                { 0xFF7A, Sfr("CC1IC", 0xFF7A, "CAPCOM Register 1 Interrupt Control Register 0000") },
                { 0xFE84, Sfr("CC2", 0xFE84, "CAPCOM Register 2 0000") },
                { 0xFF7C, Sfr("CC2IC", 0xFF7C, "CAPCOM Register 2 Interrupt Control Register 0000") },
                { 0xFE86, Sfr("CC3", 0xFE86, "CAPCOM Register 3 0000") },
                { 0xFF7E, Sfr("CC3IC", 0xFF7E, "CAPCOM Register 3 Interrupt Control Register 0000") },
                { 0xFE88, Sfr("CC4", 0xFE88, "CAPCOM Register 4 0000") },
                { 0xFF80, Sfr("CC4IC", 0xFF80, "CAPCOM Register 4 Interrupt Control Register 0000") },
                { 0xFE8A, Sfr("CC5", 0xFE8A, "CAPCOM Register 5 0000") },
                { 0xFF82, Sfr("CC5IC", 0xFF82, "CAPCOM Register 5 Interrupt Control Register 0000") },
                { 0xFE8C, Sfr("CC6", 0xFE8C, "CAPCOM Register 6 0000") },
                { 0xFF84, Sfr("CC6IC", 0xFF84, "CAPCOM Register 6 Interrupt Control Register 0000") },
                { 0xFE8E, Sfr("CC7", 0xFE8E, "CAPCOM Register 7 0000") },
                { 0xFF86, Sfr("CC7IC", 0xFF86, "CAPCOM Register 7 Interrupt Control Register 0000") },
                { 0xFE90, Sfr("CC8", 0xFE90, "CAPCOM Register 8 0000") },
                { 0xFF88, Sfr("CC8IC", 0xFF88, "CAPCOM Register 8 Interrupt Control Register 0000") },
                { 0xFE92, Sfr("CC9", 0xFE92, "CAPCOM Register 9 0000") },
                { 0xFF8A, Sfr("CC9IC", 0xFF8A, "CAPCOM Register 9 Interrupt Control Register 0000") },
                { 0xFE94, Sfr("CC10", 0xFE94, "CAPCOM Register 10 0000") },
                { 0xFF8C, Sfr("CC10IC", 0xFF8C, "CAPCOM Register 10 Interrupt Control Register 0000") },
                { 0xFE96, Sfr("CC11", 0xFE96, "CAPCOM Register 11 0000") },
                { 0xFF8E, Sfr("CC11IC", 0xFF8E, "CAPCOM Register 11 Interrupt Control Register 0000") },
                { 0xFE98, Sfr("CC12", 0xFE98, "CAPCOM Register 12 0000") },
                { 0xFF90, Sfr("CC12IC", 0xFF90, "CAPCOM Register 12 Interrupt Control Register 0000") },
                { 0xFE9A, Sfr("CC13", 0xFE9A, "CAPCOM Register 13 0000") },
                { 0xFF92, Sfr("CC13IC", 0xFF92, "CAPCOM Register 13 Interrupt Control Register 0000") },
                { 0xFE9C, Sfr("CC14", 0xFE9C, "CAPCOM Register 14 0000") },
                { 0xFF94, Sfr("CC14IC", 0xFF94, "CAPCOM Register 14 Interrupt Control Register 0000") },
                { 0xFE9E, Sfr("CC15", 0xFE9E, "CAPCOM Register 15 0000") },
                { 0xFF96, Sfr("CC15IC", 0xFF96, "CAPCOM Register 15 Interrupt Control Register 0000") },
                { 0xFF52, Sfr("CCM0", 0xFF52, "CAPCOM Mode Control Register 0 0000") },
                { 0xFF54, Sfr("CCM1", 0xFF54, "CAPCOM Mode Control Register 1 0000") },
                { 0xFF56, Sfr("CCM2", 0xFF56, "CAPCOM Mode Control Register 2 0000") },
                { 0xFF58, Sfr("CCM3", 0xFF58, "CAPCOM Mode Control Register 3 0000") },
                { 0xFE10, Sfr("CP", 0xFE10, "CPU Context Pointer Register FC00") },
                { 0xFF6A, Sfr("CRIC", 0xFF6A, "GPT2 CAPREL Interrupt Control Register 0000") },
                { 0xFE08, Sfr("CSP", 0xFE08, "CPU Code Segment Pointer Register (2 bits, read only) 0000") },
                { 0xFF02, Sfr("DP0", 0xFF02, "Port 0 Direction Control Register 0000") },
                { 0xFF06, Sfr("DP1", 0xFF06, "Port 1 Direction Control Register 0000") },
                { 0xFFC2, Sfr("DP2", 0xFFC2, "Port 2 Direction Control Register 0000") },
                { 0xFFC6, Sfr("DP3", 0xFFC6, "Port 3 Direction Control Register 0000") },
                { 0xFF0A, Sfr("DP4", 0xFF0A, "Port 4 Direction Control Register (2 bits) 00") },
                { 0xFE00, Sfr("DPP0", 0xFE00, "CPU Data Page Pointer 0 Register (4 bits) 0000") },
                { 0xFE02, Sfr("DPP1", 0xFE02, "CPU Data Page Pointer 1 Register (4 bits) 0001") },
                { 0xFE04, Sfr("DPP2", 0xFE04, "CPU Data Page Pointer 2 Register (4 bits) 0002") },
                { 0xFE06, Sfr("DPP3", 0xFE06, "CPU Data Page Pointer 3 Register (4 bits) 0003") },
                { 0xFF0E, Sfr("MDC", 0xFF0E, "CPU Multiply / Divide Control Register 0000") },
                { 0xFE0C, Sfr("MDH", 0xFE0C, "CPU Multiply / Divide Register – High Word 0000") },
                { 0xFE0E, Sfr("MDL", 0xFE0E, "CPU Multiply / Divide Register – Low Word 0000") },
                { 0xFF1E, Sfr("ONES", 0xFF1E, "Constant Value 1’s Register (read only) FFFF") },
                { 0xFF00, Sfr("P0", 0xFF00, "Port 0 Register 0000") },
                { 0xFF04, Sfr("P1", 0xFF04, "Port 1 Register 0000") },
                { 0xFFC0, Sfr("P2", 0xFFC0, "Port 2 Register 0000") },
                { 0xFFC4, Sfr("P3", 0xFFC4, "Port 3 Register 0000") },
                { 0xFFC8, Sfr("P4", 0xFFC8, "Port 4 Register (2 bits) 00") },
                { 0xFFA2, Sfr("P5", 0xFFA2, "Port 5 Register (read only) XXXX") },
                { 0xFEC0, Sfr("PECC0", 0xFEC0, "PEC Channel 0 Control Register 0000") },
                { 0xFEC2, Sfr("PECC1", 0xFEC2, "PEC Channel 1 Control Register 0000") },
                { 0xFEC4, Sfr("PECC2", 0xFEC4, "PEC Channel 2 Control Register 0000") },
                { 0xFEC6, Sfr("PECC3", 0xFEC6, "PEC Channel 3 Control Register 0000") },
                { 0xFEC8, Sfr("PECC4", 0xFEC8, "PEC Channel 4 Control Register 0000") },
                { 0xFECA, Sfr("PECC5", 0xFECA, "PEC Channel 5 Control Register 0000") },
                { 0xFECC, Sfr("PECC6", 0xFECC, "PEC Channel 6 Control Register 0000") },
                { 0xFECE, Sfr("PECC7", 0xFECE, "PEC Channel 7 Control Register 0000") },
                { 0xFF10, PSW },
                { 0xFEB4, Sfr("S0BG", 0xFEB4, "Serial Channel 0 Baud Rate Generator Reload Register 0000") },
                { 0xFFB0, Sfr("S0CON", 0xFFB0, "Serial Channel 0 Control Register 0000") },
                { 0xFF70, Sfr("S0EIC", 0xFF70, "Serial Channel 0 Error Interrupt Control Register 0000") },
                { 0xFEB2, Sfr("S0RBUF", 0xFEB2, "Serial Channel 0 Receive Buffer Register (read only) XX") },
                { 0xFF6E, Sfr("S0RIC", 0xFF6E, "Serial Channel 0 Receive Interrupt Control Register 0000") },
                { 0xFEB0, Sfr("S0TBUF", 0xFEB0, "Serial Channel 0 Transmit Buffer Register (write only) 00") },
                { 0xFF6C, Sfr("S0TIC", 0xFF6C, "Serial Channel 0 Transmit Interrupt Control Register 0000") },
                { 0xFEBC, Sfr("S1BG", 0xFEBC, "Serial Channel 1 Baud Rate Generator Reload Register 0000") },
                { 0xFFB8, Sfr("S1CON", 0xFFB8, "Serial Channel 1 Control Register 0000") },
                { 0xFF76, Sfr("S1EIC", 0xFF76, "Serial Channel 1 Error Interrupt Control Register 0000") },
                { 0xFEBA, Sfr("S1RBUF", 0xFEBA, "Serial Channel 1 Receive Buffer Register (read only) XX") },
                { 0xFF74, Sfr("S1RIC", 0xFF74, "Serial Channel 1 Receive Interrupt Control Register 0000") },
                { 0xFEB8, Sfr("S1TBUF", 0xFEB8, "Serial Channel 1 Transmit Buffer Register (write only) 00") },
                { 0xFF72, Sfr("S1TIC", 0xFF72, "Serial Channel 1 Transmit Interrupt Control Register 0000") },
                { 0xFE12, SP },
                { 0xFE14, Sfr("STKOV", 0xFE14, "CPU Stack Overflow Pointer Register FA00") },
                { 0xFE16, Sfr("STKUN", 0xFE16, "CPU Stack Underflow Pointer Register FC00") },
                { 0xFF0C, Sfr("SYSCON", 0xFF0C, "CPU System Configuration Register 0xx0") },
                { 0xFE50, Sfr("T0", 0xFE50, "CAPCOM Timer 0 Register 0000") },
                { 0xFF50, Sfr("T01CON", 0xFF50, "CAPCOM Timer 0 and Timer 1 Control Register 0000") },
                { 0xFF9C, Sfr("T0IC", 0xFF9C, "CAPCOM Timer 0 Interrupt Control Register 0000") },
                { 0xFE54, Sfr("T0REL", 0xFE54, "CAPCOM Timer 0 Reload Register 0000") },
                { 0xFE52, Sfr("T1", 0xFE52, "CAPCOM Timer 1 Register 0000") },
                { 0xFF9E, Sfr("T1IC", 0xFF9E, "CAPCOM Timer 1 Interrupt Control Register 0000") },
                { 0xFE56, Sfr("T1REL", 0xFE56, "CAPCOM Timer 1 Reload Register 0000") },
                { 0xFE40, Sfr("T2", 0xFE40, "GPT1 Timer 2 Register 0000") },
                { 0xFF40, Sfr("T2CON", 0xFF40, "GPT1 Timer 2 Control Register 0000") },
                { 0xFF60, Sfr("T2IC", 0xFF60, "GPT1 Timer 2 Interrupt Control Register 0000") },
                { 0xFE42, Sfr("T3", 0xFE42, "GPT1 Timer 3 Register 0000") },
                { 0xFF42, Sfr("T3CON", 0xFF42, "GPT1 Timer 3 Control Register 0000") },
                { 0xFF62, Sfr("T3IC", 0xFF62, "GPT1 Timer 3 Interrupt Control Register 0000") },
                { 0xFE44, Sfr("T4", 0xFE44, "GPT1 Timer 4 Register 0000") },
                { 0xFF44, Sfr("T4CON", 0xFF44, "GPT1 Timer 4 Control Register 0000") },
                { 0xFF64, Sfr("T4IC", 0xFF64, "GPT1 Timer 4 Interrupt Control Register 0000") },
                { 0xFE46, Sfr("T5", 0xFE46, "GPT2 Timer 5 Register 0000") },
                { 0xFF46, Sfr("T5CON", 0xFF46, "GPT2 Timer 5 Control Register 0000") },
                { 0xFF66, Sfr("T5IC", 0xFF66, "GPT2 Timer 5 Interrupt Control Register 0000") },
                { 0xFE48, Sfr("T6", 0xFE48, "GPT2 Timer 6 Register 0000") },
                { 0xFF48, Sfr("T6CON", 0xFF48, "GPT2 Timer 6 Control Register 0000") },
                { 0xFF68, Sfr("T6IC", 0xFF68, "GPT2 Timer 6 Interrupt Control Register 0000") },
                { 0xFFAC, Sfr("TFR", 0xFFAC, "Trap Flag Register 0000") },
                { 0xFEAE, Sfr("WDT", 0xFEAE, "Watchdog Timer Register (read only) 0000") },
                { 0xFFAE, Sfr("WDTCON", 0xFFAE, "Watchdog Timer Control Register 0000") },
                { 0xFF1C, Sfr("ZEROS", 0xFF1C, "Constant Value 0’s Register (read only) 0000") },
            };
            ByDomain = GpRegs.Concat(SpecialFunctionRegs.Values)
                .ToDictionary(r => r.Domain);

            E = new FlagGroupStorage(PSW, (uint)FlagM.EF, nameof(E));
            Z = new FlagGroupStorage(PSW, (uint)FlagM.ZF, nameof(Z));
            V = new FlagGroupStorage(PSW, (uint)FlagM.VF, nameof(V));
            C = new FlagGroupStorage(PSW, (uint)FlagM.CF, nameof(C));
            N = new FlagGroupStorage(PSW, (uint)FlagM.NF, nameof(N));
        }
    }

    [Flags]
    public enum FlagM
    {
        EF = 0x10,
        ZF = 0x08,
        VF = 0x04,
        CF = 0x02,
        NF = 0x01,
    }
}
