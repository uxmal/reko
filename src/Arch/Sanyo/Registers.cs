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

// https://mc.pp.se/dc/vms/sfr.html

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Sanyo
{
    public static class Registers
    {
        public static RegisterStorage[] Regs { get; }
        public static Dictionary<int, RegisterStorage> SFR { get; }

        public static RegisterStorage ACC { get; }
        public static RegisterStorage PSW { get; }
        public static RegisterStorage B { get; }
        public static RegisterStorage C { get; }
        public static RegisterStorage SP { get; }
        public static RegisterStorage TRH { get; }
        public static RegisterStorage TRL { get; }

        static Registers()
        {
            static RegisterStorage Reg(string name, int number) =>
                new RegisterStorage(name, number, 0, PrimitiveType.Byte);

            Regs = new RegisterStorage[]
            {
                Reg("R0", 0),
                Reg("R1", 1),
                Reg("R2", 2),
                Reg("R3", 3),
            };

            ACC = Reg("ACC", 0x100);
            PSW = Reg("PSW", 0x101);
            B = Reg("B", 0x102);
            C = Reg("C", 0x103);
            TRL = Reg("TRL", 0x104);
            TRH = Reg("TRH", 0x105);
            SP = Reg("SP", 0x106);

            SFR = new RegisterStorage[]
            { 
                ACC,
                PSW,
                B,
                C,     // C Register 
                TRL,
                TRH,
                SP,       // Stack register
                Reg("PCON", 0x107),
                Reg("IE", 0x108),
                Reg("IP", 0x109),    // Interrupt Priority Ranking Control Register 
                                                                            // 0x10A - 0x10C - Not Used 
                Reg("EXT", 0x10D),   // External Memory Control Register
                Reg("OCR", 0x10e),   // Oscillation Control Register (32kHz/600kHz/6MHz/and 5 Mhz)(Dreamcast Connected) 
                                                                             // 0x10f - Not Used 
                Reg("T0CON", 0x110), // Timer/Counter 0 Control Register 
                Reg("T0PRR", 0x111), // Timer 0 Prescaler Data Register 
                Reg("T0L", 0x112),
                Reg("T0LR", 0x113),  // Timer 0 Low Reload Register 
                Reg("T0H", 0x114),
                Reg("T0HR", 0x115),  // Timer 0 High Reload Register 
                                                                            // 0x116-0x117 - Not Used 
                Reg("T1CNT", 0x118), // Timer 1 Control Register 
                                                                            // 0x119 - Not Used 
                Reg("T1LC", 0x11A),  // Timer 1 Low Compare Data Register 
                Reg("T1L", 0x11B),   // Timer 1 Low Register 
 //               Reg("T1LR", 0x11B),  // Timer 1 Low Reload Register 
                Reg("T1HC", 0x11C),  // Timer 1 High Compare Data Register 
                Reg("T1H", 0x11D),   // Timer 1 High Register 
//                Reg("T1HR", 0x11D),  // Timer 1 High Reload Register 
                                                                            // 0x11E - 0x11F - Not used 
                Reg("MCR", 0x120),    // Mode Control Register 
                                                                             // 0x121 - Not Used 
                Reg("STAD", 0x122),  // Start Addresss Register 
                Reg("CNR", 0x123),   // Character Number Register 
                Reg("TDR", 0x124),   // Time Division Register 
                Reg("XBNK", 0x125),   // Bank Address Register 
                                                                             // 0x126 - Not Used 
                Reg("VCCR", 0x127),   // LCD Contrast Control Register 
                                                                             // 0x128-0x12f - Not Used 
                Reg("SCON0", 0x130),  // SIO0 Control Register 
                Reg("SBUF0", 0x131),  // SIO0 Buffer 
                Reg("SBR", 0x132),    // SIO Baud Rate Generator Register 
                                                                             // 0x133 - Not Used 
                Reg("SCON1", 0x134), // SIO1 Control Register 
                Reg("SBUF1", 0x135), // SIO1 Buffer 
                                                                            // 0x136-0x143 - Not Used 
                Reg("P1", 0x144),
                Reg("P1DDR", 0x145),
                Reg("P1FCR", 0x146),  // Port 1 Function Control Register 
                                                                             // 0x147-0x14b - Not Used 
                Reg("P3", 0x14c),
                Reg("P3DDR", 0x14d),
                Reg("P3INT", 0x14e),
                // 0x14F-0x15B - Not Used 
                Reg("P7", 0x15C),    // Port 7 Latch 
                Reg("I01CR", 0x15D), // External Interrupt 0, 1 Control Register 
                Reg("I23CR", 0x15E), // External Interrupt 2, 3 Control Register 
                Reg("ISL", 0x15F),   // Input Signal Selection Register 
                                                                            // 0x160 - 0x162 - Not Used 
                Reg("VSEL", 0x163),   // VMS Control Register 
                Reg("VRMAD1", 0x164), // Work RAM Access Address 1 
                Reg("VRMAD2", 0x165), // Work RAM Access Address 2 
                Reg("VTRBF", 0x166),  // Send/Receive Buffer 
                Reg("VLREG", 0x167),  // Length registration 
                                                                             // 0x168-0x17E - Not Used 
                Reg("BTCR", 0x17F),  // Base Timer Control Register 
                Reg("XRAM", 0x180),  // 0x180-0x1FB - XRAM (Bank 0)[Lines 0 -> 15] 
                //Reg("XRAM", 0x180),  // 0x180-0x1FB - XRAM (Bank 1)[Lines 16 -> 31] 
                //Reg("XRAM", 0x180),  // 0x180-0x185 - XRAM (Bank 2)[4 Icons on bottom of LCD - DO NOT USE!] 
                                                                            // 0x1FB - 0x1FF - Not Used 
            }.ToDictionary(r => r.Number);
        }

        internal static RegisterStorage? DReg(uint n)
        {
            if (SFR.TryGetValue((int) n, out var reg))
                return reg;
            else
                return null;
        }

        public static RegisterStorage Reg(uint n)
        {
            return Regs[n];
        }
    }

    [Flags]
    public enum FlagM
    {
        P = 1,          // Parity
        Rambk0 = 2, // RAM bank
        OV = 4,     // Overflow
        Irbk0 = 8,  // Indirect bank (used to set indirect addressing)
        Irbk1 = 16, // Indirect bank
        AC = 64,    // Auxiliary carry
        CY = 128,   // Carry.
    }
}
