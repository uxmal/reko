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
        public static DataRegister d0;
        public static DataRegister d1;
        public static DataRegister d2;
        public static DataRegister d3;
        public static DataRegister d4;
        public static DataRegister d5;
        public static DataRegister d6;
        public static DataRegister d7;

        public static AddressRegister a0;
        public static AddressRegister a1;
        public static AddressRegister a2;
        public static AddressRegister a3;
        public static AddressRegister a4;
        public static AddressRegister a5;
        public static AddressRegister a6;
        public static AddressRegister a7;

        public static FpRegister fp0;
        public static FpRegister fp1;
        public static FpRegister fp2;
        public static FpRegister fp3;
        public static FpRegister fp4;
        public static FpRegister fp5;
        public static FpRegister fp6;
        public static FpRegister fp7;

        public static readonly RegisterStorage ccr;
        public static readonly RegisterStorage sr;
        public static readonly RegisterStorage usp;
        public static readonly AddressRegister pc;
        public static readonly RegisterStorage fpsr;

        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage N;
        public static readonly FlagGroupStorage X;

        internal static RegisterStorage[] regs;
        internal static int Max;
        internal static readonly Dictionary<string, RegisterStorage> regsByName;

        static Registers()
        {
            d0 = new DataRegister("d0", 0, PrimitiveType.Word32);
            d1 = new DataRegister("d1", 1, PrimitiveType.Word32);
            d2 = new DataRegister("d2", 2, PrimitiveType.Word32);
            d3 = new DataRegister("d3", 3, PrimitiveType.Word32);
            d4 = new DataRegister("d4", 4, PrimitiveType.Word32);
            d5 = new DataRegister("d5", 5, PrimitiveType.Word32);
            d6 = new DataRegister("d6", 6, PrimitiveType.Word32);
            d7 = new DataRegister("d7", 7, PrimitiveType.Word32);

            a0 = new AddressRegister("a0", 8, PrimitiveType.Word32);
            a1 = new AddressRegister("a1", 9, PrimitiveType.Word32);
            a2 = new AddressRegister("a2", 10, PrimitiveType.Word32);
            a3 = new AddressRegister("a3", 11, PrimitiveType.Word32);
            a4 = new AddressRegister("a4", 12, PrimitiveType.Word32);
            a5 = new AddressRegister("a5", 13, PrimitiveType.Word32);
            a6 = new AddressRegister("a6", 14, PrimitiveType.Word32);
            a7 = new AddressRegister("a7", 15, PrimitiveType.Word32);

            fp0 = new FpRegister("fp0", 16, PrimitiveType.Real96);
            fp1 = new FpRegister("fp1", 17, PrimitiveType.Real96);
            fp2 = new FpRegister("fp2", 18, PrimitiveType.Real96);
            fp3 = new FpRegister("fp3", 19, PrimitiveType.Real96);
            fp4 = new FpRegister("fp4", 20, PrimitiveType.Real96);
            fp5 = new FpRegister("fp5", 21, PrimitiveType.Real96);
            fp6 = new FpRegister("fp6", 22, PrimitiveType.Real96);
            fp7 = new FpRegister("fp7", 23, PrimitiveType.Real96);

            ccr = new RegisterStorage("ccr", 24, 0, PrimitiveType.Byte);
            sr = new RegisterStorage("sr", 25, 0, PrimitiveType.Word16);
            usp = new RegisterStorage("usp", 26, 0, PrimitiveType.Word32);
            pc = new AddressRegister("pc", 27, PrimitiveType.Ptr32);
            fpsr = new RegisterStorage("fpsr", 28, 0, PrimitiveType.Word32);

            C = new FlagGroupStorage(ccr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            V = new FlagGroupStorage(ccr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            Z = new FlagGroupStorage(ccr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            N = new FlagGroupStorage(ccr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            X = new FlagGroupStorage(ccr, (uint) FlagM.XF, "X", PrimitiveType.Bool);

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

        public static AddressRegister AddressRegister(int reg)
        {
            return (AddressRegister)regs[reg + 8];
        }

        public static AddressRegister AddressRegister(uint reg)
        {
            return (AddressRegister) regs[reg + 8];
        }

        public static RegisterStorage FpRegister(int reg)
        {
            return regs[reg + 16];
        }

        public static RegisterStorage GetRegister(string name)
        {
            RegisterStorage reg;
            if (!regsByName.TryGetValue(name, out reg))
            {
                reg = RegisterStorage.None;
            }
            return reg;
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

    public class AddressRegister : RegisterStorage
    {
        public AddressRegister(string name, int number, PrimitiveType dt) : base(name, number, 0, dt)
        {
        }
    }

    public class DataRegister : RegisterStorage
    {
        public DataRegister(string name, int number, PrimitiveType dt) : base(name, number, 0, dt)
        {
        }
    }

    public class FpRegister : RegisterStorage
    {
        public FpRegister(string name, int number, PrimitiveType dt)
            : base(name, number, 0, dt)
        {
        }
    }
}
