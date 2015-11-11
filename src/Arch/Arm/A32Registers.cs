#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        public static readonly RegisterStorage r0  = new RegisterStorage("r0", 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r1  = new RegisterStorage("r1", 1, PrimitiveType.Word32);
        public static readonly RegisterStorage r2  = new RegisterStorage("r2", 2, PrimitiveType.Word32);
        public static readonly RegisterStorage r3  = new RegisterStorage("r3", 3, PrimitiveType.Word32);
        public static readonly RegisterStorage r4  = new RegisterStorage("r4", 4, PrimitiveType.Word32);
        public static readonly RegisterStorage r5  = new RegisterStorage("r5", 5, PrimitiveType.Word32);
        public static readonly RegisterStorage r6  = new RegisterStorage("r6", 6, PrimitiveType.Word32);
        public static readonly RegisterStorage r7  = new RegisterStorage("r7", 7, PrimitiveType.Word32);
        public static readonly RegisterStorage r8  = new RegisterStorage("r8", 8, PrimitiveType.Word32);
        public static readonly RegisterStorage r9  = new RegisterStorage("r9", 9, PrimitiveType.Word32);
        public static readonly RegisterStorage r10 = new RegisterStorage("r10", 10, PrimitiveType.Word32);
        public static readonly RegisterStorage r11 = new RegisterStorage("fp", 11, PrimitiveType.Word32);
        public static readonly RegisterStorage ip  = new RegisterStorage("ip", 12, PrimitiveType.Word32);
        public static readonly RegisterStorage sp  = new RegisterStorage("sp", 13, PrimitiveType.Word32);
        public static readonly RegisterStorage lr  = new RegisterStorage("lr", 14, PrimitiveType.Word32);
        public static readonly RegisterStorage pc  = new RegisterStorage("pc", 15, PrimitiveType.Word32);

        public static readonly RegisterStorage[] GpRegs;

        public static readonly FlagRegister cpsr = new FlagRegister("cpsr", PrimitiveType.Word32);

        public static readonly Dictionary<ArmRegister, RegisterStorage> RegisterByCapstoneID;

        public static Dictionary<string, RegisterStorage> RegistersByName { get; set; }

        static A32Registers()
        {
            r0 = new RegisterStorage("r0", 0, PrimitiveType.Word32);
            r1 = new RegisterStorage("r1", 1, PrimitiveType.Word32);
            r2 = new RegisterStorage("r2", 2, PrimitiveType.Word32);
            r3 = new RegisterStorage("r3", 3, PrimitiveType.Word32);
            r4 = new RegisterStorage("r4", 4, PrimitiveType.Word32);
            r5 = new RegisterStorage("r5", 5, PrimitiveType.Word32);
            r6 = new RegisterStorage("r6", 6, PrimitiveType.Word32);
            r7 = new RegisterStorage("r7", 7, PrimitiveType.Word32);
            r8 = new RegisterStorage("r8", 8, PrimitiveType.Word32);
            r9 = new RegisterStorage("r9", 9, PrimitiveType.Word32);
            r10 = new RegisterStorage("r10", 10, PrimitiveType.Word32);
            r11 = new RegisterStorage("fp", 11, PrimitiveType.Word32);
            ip = new RegisterStorage("ip", 12, PrimitiveType.Word32);
            sp = new RegisterStorage("sp", 13, PrimitiveType.Word32);
            lr = new RegisterStorage("lr", 14, PrimitiveType.Word32);
            pc = new RegisterStorage("pc", 15, PrimitiveType.Word32);

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
            };
        }

        public static readonly RegisterStorage f0 = new RegisterStorage("f0", 0, PrimitiveType.Real64);
        public static readonly RegisterStorage f1 = new RegisterStorage("f1", 1, PrimitiveType.Real64);
        public static readonly RegisterStorage f2 = new RegisterStorage("f2", 2, PrimitiveType.Real64);
        public static readonly RegisterStorage f3 = new RegisterStorage("f3", 3, PrimitiveType.Real64);
        public static readonly RegisterStorage f4 = new RegisterStorage("f4", 4, PrimitiveType.Real64);
        public static readonly RegisterStorage f5 = new RegisterStorage("f5", 5, PrimitiveType.Real64);
        public static readonly RegisterStorage f6 = new RegisterStorage("f6", 6, PrimitiveType.Real64);
        public static readonly RegisterStorage f7 = new RegisterStorage("f7", 7, PrimitiveType.Real64);

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
