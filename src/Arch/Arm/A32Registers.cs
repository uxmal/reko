#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using ArmRegister = Gee.External.Capstone.Arm.ArmRegister;
using ArmSysRegister = Gee.External.Capstone.Arm.ArmSysRegister;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public static class A32Registers
    {
        public static readonly RegisterStorage r0  = RegisterStorage.Reg32("r0", 0);
        public static readonly RegisterStorage r1  = RegisterStorage.Reg32("r1", 1);
        public static readonly RegisterStorage r2  = RegisterStorage.Reg32("r2", 2);
        public static readonly RegisterStorage r3  = RegisterStorage.Reg32("r3", 3);
        public static readonly RegisterStorage r4  = RegisterStorage.Reg32("r4", 4);
        public static readonly RegisterStorage r5  = RegisterStorage.Reg32("r5", 5);
        public static readonly RegisterStorage r6  = RegisterStorage.Reg32("r6", 6);
        public static readonly RegisterStorage r7  = RegisterStorage.Reg32("r7", 7);
        public static readonly RegisterStorage r8  = RegisterStorage.Reg32("r8", 8);
        public static readonly RegisterStorage r9 = RegisterStorage.Reg32("r9", 9);
        public static readonly RegisterStorage r10 = RegisterStorage.Reg32("r10", 10);
        public static readonly RegisterStorage r11 = RegisterStorage.Reg32("fp", 11);
        public static readonly RegisterStorage ip  = RegisterStorage.Reg32("ip", 12);
        public static readonly RegisterStorage sp  = RegisterStorage.Reg32("sp", 13);
        public static readonly RegisterStorage lr  = RegisterStorage.Reg32("lr", 14);
        public static readonly RegisterStorage pc  = RegisterStorage.Reg32("pc", 15);


        public static readonly RegisterStorage[] GpRegs;

        public static readonly FlagRegister fpscr = new FlagRegister("fpscr", 30, PrimitiveType.Word32);
        public static readonly FlagRegister cpsr = new FlagRegister("cpsr", 31, PrimitiveType.Word32);

        public static readonly RegisterStorage q0 =  new RegisterStorage("q0" , 32, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q1 =  new RegisterStorage("q1" , 33, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q2 =  new RegisterStorage("q2" , 34, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q3 =  new RegisterStorage("q3" , 35, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q4 =  new RegisterStorage("q4" , 36, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q5 =  new RegisterStorage("q5" , 37, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q6 =  new RegisterStorage("q6" , 38, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q7 =  new RegisterStorage("q7" , 39, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q8 =  new RegisterStorage("q8" , 40, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q9 =  new RegisterStorage("q9" , 41, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q10 = new RegisterStorage("q10", 42, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q11 = new RegisterStorage("q11", 43, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q12 = new RegisterStorage("q12", 44, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q13 = new RegisterStorage("q13", 45, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q14 = new RegisterStorage("q14", 46, 0, PrimitiveType.Word128) ;
        public static readonly RegisterStorage q15 = new RegisterStorage("q15", 47, 0, PrimitiveType.Word128);

        public static readonly RegisterStorage d0 = new RegisterStorage("d0", 32, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d1 = new RegisterStorage("d1", 32, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d2 = new RegisterStorage("d2", 33, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d3 = new RegisterStorage("d3", 33, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d4 = new RegisterStorage("d4", 34, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d5 = new RegisterStorage("d5", 34, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d6 = new RegisterStorage("d6", 35, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d7 = new RegisterStorage("d7", 35, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d8 = new RegisterStorage("d8", 36, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d9 = new RegisterStorage("d9", 36, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d10 = new RegisterStorage("d10", 37, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d11 = new RegisterStorage("d11", 37, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d12 = new RegisterStorage("d12", 38, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d13 = new RegisterStorage("d13", 38, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d14 = new RegisterStorage("d14", 39, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d15 = new RegisterStorage("d15", 39, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d16 = new RegisterStorage("d16", 40, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d17 = new RegisterStorage("d17", 40, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d18 = new RegisterStorage("d18", 41, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d19 = new RegisterStorage("d19", 41, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d20 = new RegisterStorage("d20", 42, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d21 = new RegisterStorage("d20", 42, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d22 = new RegisterStorage("d20", 43, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d23 = new RegisterStorage("d20", 43, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d24 = new RegisterStorage("d20", 44, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d25 = new RegisterStorage("d20", 44, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d26 = new RegisterStorage("d20", 45, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d27 = new RegisterStorage("d20", 45, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d28 = new RegisterStorage("d20", 46, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d29 = new RegisterStorage("d20", 46, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage d30 = new RegisterStorage("d30", 47, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage d31 = new RegisterStorage("d31", 47, 64, PrimitiveType.Word64);

        public static readonly RegisterStorage s0 = new RegisterStorage("s0", 32, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s1 = new RegisterStorage("s1", 32, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s2 = new RegisterStorage("s2", 32, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s3 = new RegisterStorage("s3", 32, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s4 = new RegisterStorage("s4", 33, 0,  PrimitiveType.Word64);
        public static readonly RegisterStorage s5 = new RegisterStorage("s5", 33, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s6 = new RegisterStorage("s6", 33, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s7 = new RegisterStorage("s7", 33, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s8 = new RegisterStorage("s8", 34, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s9 = new RegisterStorage("s9", 34, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s10 = new RegisterStorage("s10", 34, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s11 = new RegisterStorage("s11", 34, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s12 = new RegisterStorage("s12", 35, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s13 = new RegisterStorage("s13", 35, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s14 = new RegisterStorage("s14", 35, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s15 = new RegisterStorage("s15", 35, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s16 = new RegisterStorage("s16", 36, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s17 = new RegisterStorage("s17", 36, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s18 = new RegisterStorage("s18", 36, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s19 = new RegisterStorage("s19", 36, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s20 = new RegisterStorage("s20", 37, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s21 = new RegisterStorage("s20", 37, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s22 = new RegisterStorage("s20", 37, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s23 = new RegisterStorage("s20", 37, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s24 = new RegisterStorage("s20", 38, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s25 = new RegisterStorage("s20", 38, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s26 = new RegisterStorage("s20", 38, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s27 = new RegisterStorage("s20", 38, 96, PrimitiveType.Word64);
        public static readonly RegisterStorage s28 = new RegisterStorage("s20", 39, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage s29 = new RegisterStorage("s20", 39, 32, PrimitiveType.Word64);
        public static readonly RegisterStorage s30 = new RegisterStorage("s30", 39, 64, PrimitiveType.Word64);
        public static readonly RegisterStorage s31 = new RegisterStorage("s31", 39, 96, PrimitiveType.Word64);

        public static readonly FlagRegister spsr = new FlagRegister("spsr", 40, PrimitiveType.Word32);

        public static readonly Dictionary<ArmRegister, RegisterStorage> RegisterByCapstoneID;
        public static readonly Dictionary<ArmSysRegister, RegisterStorage> SysRegisterByCapstoneID;

        public static Dictionary<string, RegisterStorage> RegistersByName { get; set; }

        static A32Registers()
        {
            GpRegs = new RegisterStorage[] {
                    r0, 
                    r1, 
                    r2, 
                    r3, 
                    r4, 
                    r5, 
                    r6, 
                    r7, 
                    r8, 
                    r9, 
                    r10,
                    r11,
                    ip, 
                    sp, 
                    lr, 
                    pc, 
            };

            RegisterByCapstoneID = new Dictionary<ArmRegister, RegisterStorage>
            {
                  { ArmRegister.R0  ,     r0  },
                  { ArmRegister.R1  ,     r1  },
                  { ArmRegister.R2  ,     r2  },
                  { ArmRegister.R3  ,     r3  },
                  { ArmRegister.R4  ,     r4  },
                  { ArmRegister.R5  ,     r5  },
                  { ArmRegister.R6  ,     r6  },
                  { ArmRegister.R7  ,     r7  },
                  { ArmRegister.R8  ,     r8  },
                  { ArmRegister.R9  ,     r9  },
                  { ArmRegister.R10 ,     r10 },
                  { ArmRegister.R11 ,     r11 },
                  { ArmRegister.IP  ,     ip  },
                  { ArmRegister.SP  ,     sp  },
                  { ArmRegister.LR  ,     lr  },
                  { ArmRegister.PC  ,     pc  },

                  { ArmRegister.Q0  ,     q0  },
                  { ArmRegister.Q1  ,     q1  },
                  { ArmRegister.Q2  ,     q2  },
                  { ArmRegister.Q3  ,     q3  },
                  { ArmRegister.Q4  ,     q4  },
                  { ArmRegister.Q5  ,     q5  },
                  { ArmRegister.Q6  ,     q6  },
                  { ArmRegister.Q7  ,     q7  },
                  { ArmRegister.Q8  ,     q8  },
                  { ArmRegister.Q9  ,     q9  },
                  { ArmRegister.Q10 ,     q10 },
                  { ArmRegister.Q11 ,     q11 },
                  { ArmRegister.Q12 ,     q12  },
                  { ArmRegister.Q13 ,     q13 },
                  { ArmRegister.Q14 ,     q14  },
                  { ArmRegister.Q15,      q15  },

                  { ArmRegister.D0 ,      d0 },
                  { ArmRegister.D1 ,      d1 },
                  { ArmRegister.D2 ,      d2 },
                  { ArmRegister.D3 ,      d3 },
                  { ArmRegister.D4 ,      d4 },
                  { ArmRegister.D5 ,      d5 },
                  { ArmRegister.D6 ,      d6 },
                  { ArmRegister.D7 ,      d7 },
                  { ArmRegister.D8 ,      d8 },
                  { ArmRegister.D9 ,      d9 },
                  { ArmRegister.D10,      d10 },
                  { ArmRegister.D11,      d11 },
                  { ArmRegister.D12,      d12 },
                  { ArmRegister.D13,      d13 },
                  { ArmRegister.D14,      d14 },
                  { ArmRegister.D15,      d15 },
                  { ArmRegister.D16,      d16 },
                  { ArmRegister.D17,      d17 },
                  { ArmRegister.D18,      d18 },
                  { ArmRegister.D19,      d19 },
                  { ArmRegister.D20,      d20 },
                  { ArmRegister.D21,      d21 },
                  { ArmRegister.D22,      d22 },
                  { ArmRegister.D23,      d23 },
                  { ArmRegister.D24,      d24 },
                  { ArmRegister.D25,      d25 },
                  { ArmRegister.D26,      d26 },
                  { ArmRegister.D27,      d27 },
                  { ArmRegister.D28,      d28 },
                  { ArmRegister.D29,      d29 },
                  { ArmRegister.D30,      d30 },
                  { ArmRegister.D31,      d31 },

                  { ArmRegister.S0 ,      s0 },
                  { ArmRegister.S1 ,      s1 },
                  { ArmRegister.S2 ,      s2 },
                  { ArmRegister.S3 ,      s3 },
                  { ArmRegister.S4 ,      s4 },
                  { ArmRegister.S5 ,      s5 },
                  { ArmRegister.S6 ,      s6 },
                  { ArmRegister.S7 ,      s7 },
                  { ArmRegister.S8 ,      s8 },
                  { ArmRegister.S9 ,      s9 },
                  { ArmRegister.S10,      s10 },
                  { ArmRegister.S11,      s11 },
                  { ArmRegister.S12,      s12 },
                  { ArmRegister.S13,      s13 },
                  { ArmRegister.S14,      s14 },
                  { ArmRegister.S15,      s15 },
                  { ArmRegister.S16,      s16 },
                  { ArmRegister.S17,      s17 },
                  { ArmRegister.S18,      s18 },
                  { ArmRegister.S19,      s19 },
                  { ArmRegister.S20,      s20 },
                  { ArmRegister.S21,      s21 },
                  { ArmRegister.S22,      s22 },
                  { ArmRegister.S23,      s23 },
                  { ArmRegister.S24,      s24 },
                  { ArmRegister.S25,      s25 },
                  { ArmRegister.S26,      s26 },
                  { ArmRegister.S27,      s27 },
                  { ArmRegister.S28,      s28 },
                  { ArmRegister.S29,      s29 },
                  { ArmRegister.S30,      s30 },
                  { ArmRegister.S31,      s31 },

                  { ArmRegister.APSR, cpsr },
                  { ArmRegister.APSR_NZCV, cpsr },
                  { ArmRegister.FPSCR, fpscr },
                  { ArmRegister.SPSR, spsr }
            };

            SysRegisterByCapstoneID = new Dictionary<ArmSysRegister, RegisterStorage>
            {
                { ArmSysRegister.CPSR_C, cpsr },
            };

            RegistersByName = new Dictionary<string, RegisterStorage>(StringComparer.InvariantCultureIgnoreCase)
            {
                 { "r0",        r0  },
                 { "r1",        r1  },
                 { "r2",        r2  },
                 { "r3",        r3  },
                 { "r4",        r4  },
                 { "r5",        r5  },
                 { "r6",        r6  },
                 { "r7",        r7  },
                 { "r8",        r8  },
                 { "r9",        r9  },
                 { "r10",       r10 },
                 { "r11",       r11 },
                 { "ip",        ip  },
                 { "sp",        sp  },
                 { "lr",        lr  },
                 { "pc",        pc  },
                 { "q0" ,       q0  },
                 { "q1" ,       q1  },
                 { "q2" ,       q2  },
                 { "q3" ,       q3  },
                 { "q4" ,       q4  },
                 { "q5" ,       q5  },
                 { "q6" ,       q6  },
                 { "q7" ,       q7  },
                 { "q8" ,       q8  },
                 { "q9" ,       q9  },
                 { "q10",       q10 },
                 { "q11",       q11 },
                 { "q12",       q12 },
                 { "q13",       q13 },
                 { "q14",       q14 },
                 { "q15",       q15 },
                 { "d0",        d0  },
                 { "d1",        d1  },
                 { "d2",        d2  },
                 { "d3",        d3  },
                 { "d4",        d4  },
                 { "d5",        d5  },
                 { "d6",        d6  },
                 { "d7",        d7  },
                 { "d8",        d8  },
                 { "d9",        d9  },
                 { "d10",       d10 },
                 { "d11",       d11 },
                 { "d12",       d12 },
                 { "d13",       d13 },
                 { "d14",       d14 },
                 { "d15",       d15 },
                 { "d16",       d16 },
                 { "d17",       d17 },
                 { "d18",       d18 },
                 { "d19",       d19 },
                 { "d20",       d20 },
                 { "d21",       d21 },
                 { "d22",       d22 },
                 { "d23",       d23 },
                 { "d24",       d24 },
                 { "d25",       d25 },
                 { "d26",       d26 },
                 { "d27",       d27 },
                 { "d28",       d28 },
                 { "d29",       d29 },
                 { "d30",       d30 },
                 { "d31",       d31 },
            };
        }

        public static readonly RegisterStorage f0 = new RegisterStorage("f0", 0, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f1 = new RegisterStorage("f1", 1, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f2 = new RegisterStorage("f2", 2, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f3 = new RegisterStorage("f3", 3, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f4 = new RegisterStorage("f4", 4, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f5 = new RegisterStorage("f5", 5, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f6 = new RegisterStorage("f6", 6, 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f7 = new RegisterStorage("f7", 7, 0, PrimitiveType.Real64);

        public static readonly RegisterStorage[] FpRegs =
        {
            f0, f1, f2, f3, f4, f5, f6, f7
        };
    }

    [Flags]
    public enum FlagM
    {
        NF = 8,
        ZF = 4,
        CF = 2,
        VF = 1
    }
}
