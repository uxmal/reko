#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        public static readonly RegisterStorage x0 = new RegisterStorage("x0", 0, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x1 = new RegisterStorage("x1", 1, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x2 = new RegisterStorage("x2", 2, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x3 = new RegisterStorage("x3", 3, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x4 = new RegisterStorage("x4", 4, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x5 = new RegisterStorage("x5", 5, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x6 = new RegisterStorage("x6", 6, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x7 = new RegisterStorage("x7", 7, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x8 = new RegisterStorage("x8", 8, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x9 = new RegisterStorage("x9", 9, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x10 = new RegisterStorage("x10", 10, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x11 = new RegisterStorage("x11", 11, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x12 = new RegisterStorage("x12", 12, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x13 = new RegisterStorage("x13", 13, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x14 = new RegisterStorage("x14", 14, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x15 = new RegisterStorage("x15", 15, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x16 = new RegisterStorage("x16", 16, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x17 = new RegisterStorage("x17", 17, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x18 = new RegisterStorage("x18", 18, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x19 = new RegisterStorage("x19", 19, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x20 = new RegisterStorage("x20", 20, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x21 = new RegisterStorage("x21", 21, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x22 = new RegisterStorage("x22", 22, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x23 = new RegisterStorage("x23", 23, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x24 = new RegisterStorage("x24", 24, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x25 = new RegisterStorage("x25", 25, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x26 = new RegisterStorage("x26", 26, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x27 = new RegisterStorage("x27", 27, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x28 = new RegisterStorage("x28", 28, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x29 = new RegisterStorage("x29", 29, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x30 = new RegisterStorage("x30", 30, 0, PrimitiveType.Word64);
        public static readonly RegisterStorage x31 = new RegisterStorage("x31", 31, 0, PrimitiveType.Word64);

        public static readonly RegisterStorage w0 = new RegisterStorage("w0", 32, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w1 = new RegisterStorage("w1", 33, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w2 = new RegisterStorage("w2", 34, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w3 = new RegisterStorage("w3", 35, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w4 = new RegisterStorage("w4", 36, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w5 = new RegisterStorage("w5", 37, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w6 = new RegisterStorage("w6", 38, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w7 = new RegisterStorage("w7", 39, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w8 = new RegisterStorage("w8", 40, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w9 = new RegisterStorage("w9", 41, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w10 = new RegisterStorage("w10", 42, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w11 = new RegisterStorage("w11", 43, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w12 = new RegisterStorage("w12", 44, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w13 = new RegisterStorage("w13", 45, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w14 = new RegisterStorage("w14", 46, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w15 = new RegisterStorage("w15", 47, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w16 = new RegisterStorage("w16", 48, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w17 = new RegisterStorage("w17", 49, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w18 = new RegisterStorage("w18", 50, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w19 = new RegisterStorage("w19", 51, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w20 = new RegisterStorage("w20", 52, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w21 = new RegisterStorage("w21", 53, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w22 = new RegisterStorage("w22", 54, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w23 = new RegisterStorage("w23", 55, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w24 = new RegisterStorage("w24", 56, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w25 = new RegisterStorage("w25", 57, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w26 = new RegisterStorage("w26", 58, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w27 = new RegisterStorage("w27", 59, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w28 = new RegisterStorage("w28", 60, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w29 = new RegisterStorage("w29", 61, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w30 = new RegisterStorage("w30", 62, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage w31 = new RegisterStorage("w31", 63, 0, PrimitiveType.Word32);

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
