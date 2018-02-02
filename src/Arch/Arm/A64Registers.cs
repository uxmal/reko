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

using Gee.External.Capstone.Arm64;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public static class A64Registers
    {
        public static readonly RegisterStorage x0 = RegisterStorage.Reg64("x0", 0);
        public static readonly RegisterStorage x1 = RegisterStorage.Reg64("x1", 1);
        public static readonly RegisterStorage x2 = RegisterStorage.Reg64("x2", 2);
        public static readonly RegisterStorage x3 = RegisterStorage.Reg64("x3", 3);
        public static readonly RegisterStorage x4 = RegisterStorage.Reg64("x4", 4);
        public static readonly RegisterStorage x5 = RegisterStorage.Reg64("x5", 5);
        public static readonly RegisterStorage x6 = RegisterStorage.Reg64("x6", 6);
        public static readonly RegisterStorage x7 = RegisterStorage.Reg64("x7", 7);
        public static readonly RegisterStorage x8 = RegisterStorage.Reg64("x8", 8);
        public static readonly RegisterStorage x9 = RegisterStorage.Reg64("x9", 9);
        public static readonly RegisterStorage x10 = RegisterStorage.Reg64("x10", 10);
        public static readonly RegisterStorage x11 = RegisterStorage.Reg64("x11", 11);
        public static readonly RegisterStorage x12 = RegisterStorage.Reg64("x12", 12);
        public static readonly RegisterStorage x13 = RegisterStorage.Reg64("x13", 13);
        public static readonly RegisterStorage x14 = RegisterStorage.Reg64("x14", 14);
        public static readonly RegisterStorage x15 = RegisterStorage.Reg64("x15", 15);
        public static readonly RegisterStorage x16 = RegisterStorage.Reg64("x16", 16);
        public static readonly RegisterStorage x17 = RegisterStorage.Reg64("x17", 17);
        public static readonly RegisterStorage x18 = RegisterStorage.Reg64("x18", 18);
        public static readonly RegisterStorage x19 = RegisterStorage.Reg64("x19", 19);
        public static readonly RegisterStorage x20 = RegisterStorage.Reg64("x20", 20);
        public static readonly RegisterStorage x21 = RegisterStorage.Reg64("x21", 21);
        public static readonly RegisterStorage x22 = RegisterStorage.Reg64("x22", 22);
        public static readonly RegisterStorage x23 = RegisterStorage.Reg64("x23", 23);
        public static readonly RegisterStorage x24 = RegisterStorage.Reg64("x24", 24);
        public static readonly RegisterStorage x25 = RegisterStorage.Reg64("x25", 25);
        public static readonly RegisterStorage x26 = RegisterStorage.Reg64("x26", 26);
        public static readonly RegisterStorage x27 = RegisterStorage.Reg64("x27", 27);
        public static readonly RegisterStorage x28 = RegisterStorage.Reg64("x28", 28);
        public static readonly RegisterStorage x29 = RegisterStorage.Reg64("x29", 29);
        public static readonly RegisterStorage x30 = RegisterStorage.Reg64("x30", 30);
        public static readonly RegisterStorage x31 = RegisterStorage.Reg64("x31", 31);

        public static readonly RegisterStorage w0 = RegisterStorage.Reg32("w0", 32);
        public static readonly RegisterStorage w1 = RegisterStorage.Reg32("w1", 33);
        public static readonly RegisterStorage w2 = RegisterStorage.Reg32("w2", 34);
        public static readonly RegisterStorage w3 = RegisterStorage.Reg32("w3", 35);
        public static readonly RegisterStorage w4 = RegisterStorage.Reg32("w4", 36);
        public static readonly RegisterStorage w5 = RegisterStorage.Reg32("w5", 37);
        public static readonly RegisterStorage w6 = RegisterStorage.Reg32("w6", 38);
        public static readonly RegisterStorage w7 = RegisterStorage.Reg32("w7", 39);
        public static readonly RegisterStorage w8 = RegisterStorage.Reg32("w8", 40);
        public static readonly RegisterStorage w9 = RegisterStorage.Reg32("w9", 41);
        public static readonly RegisterStorage w10 = RegisterStorage.Reg32("w10", 42);
        public static readonly RegisterStorage w11 = RegisterStorage.Reg32("w11", 43);
        public static readonly RegisterStorage w12 = RegisterStorage.Reg32("w12", 44);
        public static readonly RegisterStorage w13 = RegisterStorage.Reg32("w13", 45);
        public static readonly RegisterStorage w14 = RegisterStorage.Reg32("w14", 46);
        public static readonly RegisterStorage w15 = RegisterStorage.Reg32("w15", 47);
        public static readonly RegisterStorage w16 = RegisterStorage.Reg32("w16", 48);
        public static readonly RegisterStorage w17 = RegisterStorage.Reg32("w17", 49);
        public static readonly RegisterStorage w18 = RegisterStorage.Reg32("w18", 50);
        public static readonly RegisterStorage w19 = RegisterStorage.Reg32("w19", 51);
        public static readonly RegisterStorage w20 = RegisterStorage.Reg32("w20", 52);
        public static readonly RegisterStorage w21 = RegisterStorage.Reg32("w21", 53);
        public static readonly RegisterStorage w22 = RegisterStorage.Reg32("w22", 54);
        public static readonly RegisterStorage w23 = RegisterStorage.Reg32("w23", 55);
        public static readonly RegisterStorage w24 = RegisterStorage.Reg32("w24", 56);
        public static readonly RegisterStorage w25 = RegisterStorage.Reg32("w25", 57);
        public static readonly RegisterStorage w26 = RegisterStorage.Reg32("w26", 58);
        public static readonly RegisterStorage w27 = RegisterStorage.Reg32("w27", 59);
        public static readonly RegisterStorage w28 = RegisterStorage.Reg32("w28", 60);
        public static readonly RegisterStorage w29 = RegisterStorage.Reg32("w29", 61);
        public static readonly RegisterStorage w30 = RegisterStorage.Reg32("w30", 62);
        public static readonly RegisterStorage w31 = RegisterStorage.Reg32("w31", 63);

        public static RegisterStorage GetXReg(int i) { return XRegs[i]; }
        public static RegisterStorage GetWReg(int i) { return XRegs[i + 32]; }
        public static readonly RegisterStorage[] XRegs = {
            x0,
            x1,
            x2,
            x3,
            x4,
            x5,
            x6,
            x7,
            x8,
            x9,
            x10,
            x11,
            x12,
            x13,
            x14,
            x15,
            x16,
            x17,
            x18,
            x19,
            x20,
            x21,
            x22,
            x23,
            x24,
            x25,
            x26,
            x27,
            x28,
            x29,
            x30,
            x31,

            w0, 
            w1, 
            w2, 
            w3, 
            w4, 
            w5, 
            w6, 
            w7, 
            w8, 
            w9, 
            w10,
            w11,
            w12,
            w13,
            w14,
            w15,
            w16,
            w17,
            w18,
            w19,
            w20,
            w21,
            w22,
            w23,
            w24,
            w25,
            w26,
            w27,
            w28,
            w29,
            w30,
            w31,
        };

        public static Dictionary<Arm64Register, RegisterStorage> RegisterByCapstoneID { get; private set; }

        static A64Registers()
        {
            RegisterByCapstoneID = new Dictionary<Arm64Register, RegisterStorage>
            {
                { Arm64Register. X0,      x0},
                { Arm64Register. X1,      x1},
                { Arm64Register. X2,      x2},
                { Arm64Register. X3,      x3},
                { Arm64Register. X4,      x4},
                { Arm64Register. X5,      x5},
                { Arm64Register. X6,      x6},
                { Arm64Register. X7,      x7},
                { Arm64Register. X8,      x8},
                { Arm64Register. X9,      x9},
                { Arm64Register. X10,     x10 },
                { Arm64Register. X11,     x11 },
                { Arm64Register. X12,     x12 },
                { Arm64Register. X13,     x13 },
                { Arm64Register. X14,     x14 },
                { Arm64Register. X15,     x15 },
                { Arm64Register. X16,     x16 },
                { Arm64Register. X17,     x17 },
                { Arm64Register. X18,     x18 },
                { Arm64Register. X19,     x19 },
                { Arm64Register. X20,     x20 },
                { Arm64Register. X21,     x21 },
                { Arm64Register. X22,     x22 },
                { Arm64Register. X23,     x23 },
                { Arm64Register. X24,     x24 },
                { Arm64Register. X25,     x25 },
                { Arm64Register. X26,     x26 },
                { Arm64Register. X27,     x27 },
                { Arm64Register. X28,     x28 },
                { Arm64Register. X29,     x29 },
                { Arm64Register. X30,     x30 },

                { Arm64Register.W0,      w0},
                { Arm64Register.W1,      w1},
                { Arm64Register.W2,      w2},
                { Arm64Register.W3,      w3},
                { Arm64Register.W4,      w4},
                { Arm64Register.W5,      w5},
                { Arm64Register.W6,      w6},
                { Arm64Register.W7,      w7},
                { Arm64Register.W8,      w8},
                { Arm64Register.W9,      w9},
                { Arm64Register.W10,     w10 },
                { Arm64Register.W11,     w11 },
                { Arm64Register.W12,     w12 },
                { Arm64Register.W13,     w13 },
                { Arm64Register.W14,     w14 },
                { Arm64Register.W15,     w15 },
                { Arm64Register.W16,     w16 },
                { Arm64Register.W17,     w17 },
                { Arm64Register.W18,     w18 },
                { Arm64Register.W19,     w19 },
                { Arm64Register.W20,     w20 },
                { Arm64Register.W21,     w21 },
                { Arm64Register.W22,     w22 },
                { Arm64Register.W23,     w23 },
                { Arm64Register.W24,     w24 },
                { Arm64Register.W25,     w25 },
                { Arm64Register.W26,     w26 },
                { Arm64Register.W27,     w27 },
                { Arm64Register.W28,     w28 },
                { Arm64Register.W29,     w29 },
                { Arm64Register.W30,     w30 }
            };
        }
    }
}
