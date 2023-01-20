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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    public static class Registers
    {
        public static readonly RegisterStorage d0;
        public static readonly RegisterStorage d1;
        public static readonly RegisterStorage d2;
        public static readonly RegisterStorage d3;
        public static readonly RegisterStorage d4;
        public static readonly RegisterStorage d5;
        public static readonly RegisterStorage d6;
        public static readonly RegisterStorage d7;

        public static readonly RegisterStorage a0;
        public static readonly RegisterStorage a1;
        public static readonly RegisterStorage a2;
        public static readonly RegisterStorage a3;
        public static readonly RegisterStorage a4;
        public static readonly RegisterStorage a5;
        public static readonly RegisterStorage a6;
        public static readonly RegisterStorage a7;

        public static readonly RegisterStorage fp0;
        public static readonly RegisterStorage fp1;
        public static readonly RegisterStorage fp2;
        public static readonly RegisterStorage fp3;
        public static readonly RegisterStorage fp4;
        public static readonly RegisterStorage fp5;
        public static readonly RegisterStorage fp6;
        public static readonly RegisterStorage fp7;

        public static readonly RegisterStorage ccr;
        public static readonly RegisterStorage sr;
        public static readonly RegisterStorage usp;
        public static readonly RegisterStorage pc;
        public static readonly RegisterStorage fpsr;

        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage N;
        public static readonly FlagGroupStorage X;
        public static readonly FlagGroupStorage VN;
        public static readonly FlagGroupStorage ZN;
        public static readonly FlagGroupStorage VZN;
        public static readonly FlagGroupStorage CVZN;
        public static readonly FlagGroupStorage CZ;
        public static readonly FlagGroupStorage CZN;
        public static readonly FlagGroupStorage CZNX;
        public static readonly FlagGroupStorage CVZNX;

        internal static RegisterStorage[] regs;
        internal static FlagGroupStorage[] flags;
        internal static int Max;
        internal static readonly Dictionary<string, RegisterStorage> regsByName;

        static Registers()
        {
            d0 = RegisterStorage.Reg32("d0", 0);
            d1 = RegisterStorage.Reg32("d1", 1);
            d2 = RegisterStorage.Reg32("d2", 2);
            d3 = RegisterStorage.Reg32("d3", 3);
            d4 = RegisterStorage.Reg32("d4", 4);
            d5 = RegisterStorage.Reg32("d5", 5);
            d6 = RegisterStorage.Reg32("d6", 6);
            d7 = RegisterStorage.Reg32("d7", 7);

            a0 = RegisterStorage.Reg32("a0", 8);
            a1 = RegisterStorage.Reg32("a1", 9);
            a2 = RegisterStorage.Reg32("a2", 10);
            a3 = RegisterStorage.Reg32("a3", 11);
            a4 = RegisterStorage.Reg32("a4", 12);
            a5 = RegisterStorage.Reg32("a5", 13);
            a6 = RegisterStorage.Reg32("a6", 14);
            a7 = RegisterStorage.Reg32("a7", 15);

            fp0 = new RegisterStorage("fp0", 16, 0, PrimitiveType.Real96);
            fp1 = new RegisterStorage("fp1", 17, 0, PrimitiveType.Real96);
            fp2 = new RegisterStorage("fp2", 18, 0, PrimitiveType.Real96);
            fp3 = new RegisterStorage("fp3", 19, 0, PrimitiveType.Real96);
            fp4 = new RegisterStorage("fp4", 20, 0, PrimitiveType.Real96);
            fp5 = new RegisterStorage("fp5", 21, 0, PrimitiveType.Real96);
            fp6 = new RegisterStorage("fp6", 22, 0, PrimitiveType.Real96);
            fp7 = new RegisterStorage("fp7", 23, 0, PrimitiveType.Real96);

            ccr = RegisterStorage.Reg8("ccr", 24);
            sr = RegisterStorage.Reg16("sr", 25);
            usp = RegisterStorage.Reg32("usp", 26);
            pc = RegisterStorage.Reg32("pc", 27);
            fpsr = RegisterStorage.Reg32("fpsr", 28);

            C = new FlagGroupStorage(ccr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(ccr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            Z = new FlagGroupStorage(ccr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            N = new FlagGroupStorage(ccr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            X = new FlagGroupStorage(ccr, (uint) FlagM.XF, "X", PrimitiveType.Bool);

            CVZN = new FlagGroupStorage(ccr, (uint) (FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF), "CVZN", PrimitiveType.Byte);
            CZ = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.CF), "CZ", PrimitiveType.Byte);
            CZN = new FlagGroupStorage(ccr, (uint) (FlagM.NF | FlagM.ZF | FlagM.CF), "CZN", PrimitiveType.Byte);
            CZNX = new FlagGroupStorage(ccr, (uint) (FlagM.XF | FlagM.NF | FlagM.ZF | FlagM.CF), "CZNX", PrimitiveType.Byte);
            CVZNX = new FlagGroupStorage(ccr, (uint) (FlagM.XF | FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF), "CVZNX", PrimitiveType.Byte);
            VN = new FlagGroupStorage(ccr, (uint) (FlagM.NF | FlagM.VF), "VN", PrimitiveType.Byte);
            VZN = new FlagGroupStorage(ccr, (uint) (FlagM.NF | FlagM.ZF | FlagM.VF), "VZN", PrimitiveType.Byte);
            ZN = new FlagGroupStorage(ccr, (uint) (FlagM.NF | FlagM.ZF), "ZN", PrimitiveType.Byte);
            Max = 29;

            regs = new RegisterStorage[] { 
                d0, 
                d1, 
                d2, 
                d3, 
                d4, 
                d5, 
                d6, 
                d7, 

                a0, 
                a1, 
                a2, 
                a3, 
                a4, 
                a5, 
                a6, 
                a7, 

                fp0,
                fp1,
                fp2,
                fp3,
                fp4,
                fp5,
                fp6,
                fp7,

                ccr,
                sr,
                usp,
                pc,
                fpsr,
            };

            regsByName = regs.ToDictionary(r => r.Name, StringComparer.InvariantCultureIgnoreCase);
            flags = new[] { C, V, Z, N, X };
        }

        public static RegisterStorage GetRegister(int reg)
        {
            return regs[reg];
        }

        public static RegisterStorage DataRegister(int reg)
        {
            return regs[reg];
        }

        public static RegisterStorage DataRegister(uint reg)
        {
            return regs[reg];
        }

        public static RegisterStorage AddressRegister(int reg)
        {
            return regs[reg + 8];
        }

        public static RegisterStorage AddressRegister(uint reg)
        {
            return regs[reg + 8];
        }

        public static RegisterStorage FpRegister(int reg)
        {
            return regs[reg + 16];
        }

        public static RegisterStorage GetRegister(string name)
        {
            if (!regsByName.TryGetValue(name, out RegisterStorage? reg))
            {
                reg = RegisterStorage.None;
            }
            return reg;
        }

        public static bool IsAddressRegister(RegisterStorage rop)
        {
            return 8 <= rop.Number && rop.Number < 16;
        }

        public static bool IsDataRegister(RegisterStorage rop)
        {
            return 0 <= rop.Number && rop.Number < 8;
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        CF = 1,
        VF = 2,
        ZF = 4,
        NF = 8,
        XF = 16,

        CVZN = CF | VF | ZF | NF,
        CVZNX = CF | VF | ZF | NF | XF,
    }
}
