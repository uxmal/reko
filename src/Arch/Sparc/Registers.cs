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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public static class Registers
    {
        public static RegisterStorage g0;
        public static RegisterStorage sp;   // stack pointer
        public static RegisterStorage o7;

        public static RegisterStorage[] OutRegisters;
        public static RegisterStorage[] InRegisters;
        public static RegisterStorage y;

        public static RegisterStorage psr;
        public static RegisterStorage fsr;

        public static FlagGroupStorage N;
        public static FlagGroupStorage Z;
        public static FlagGroupStorage V;
        public static FlagGroupStorage C;

        public static FlagGroupStorage E;
        public static FlagGroupStorage L;
        public static FlagGroupStorage G;
        public static FlagGroupStorage U;

        public static RegisterStorage[] IntegerRegisters;
        public static RegisterStorage[] FloatRegisters;

        private static Dictionary<string, RegisterStorage> mpNameToReg;
        private static Dictionary<StorageDomain, RegisterStorage> mpDomainToReg;

        static Registers()
        {
            var stg = new StorageFactory();

            var globRegs = stg.RangeOfReg32(8, "g{0}");
            g0 = globRegs[0];
            // outgoing parameter 0 / return value from callee
            OutRegisters = stg.RangeOfReg(8, n => n == 6 ? "sp" : $"o{n}", PrimitiveType.Word32);
            sp = OutRegisters[6];
            o7 = OutRegisters[7];

            var localRegs = stg.RangeOfReg32(8, "l{0}");
            // incoming parameters / return value to caller
            // i6 = frame pointer
            // i7 = return address - 8
            InRegisters = stg.RangeOfReg32(8, "i{0}");

            y = RegisterStorage.Reg32("y", 65);

            IntegerRegisters = globRegs
                .Concat(OutRegisters)
                .Concat(localRegs)
                .Concat(InRegisters)
                .Concat(new[] { y })
                .ToArray();

            // Sparc floating point registers can contain integers, which is 
            // why they can't be real32. This also forces our hand into
            // making float-point versions of add, sub, mul, div. 

            FloatRegisters = stg.RangeOfReg32(64, "f{0}");

            psr = stg.Reg32("psr");

            N = new FlagGroupStorage(psr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            Z = new FlagGroupStorage(psr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            V = new FlagGroupStorage(psr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            C = new FlagGroupStorage(psr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
 
            E = new FlagGroupStorage(psr, (uint) FlagM.EF, "E", PrimitiveType.Bool);
            L = new FlagGroupStorage(psr, (uint) FlagM.LF, "L", PrimitiveType.Bool);
            G = new FlagGroupStorage(psr, (uint) FlagM.GF, "G", PrimitiveType.Bool);
            U = new FlagGroupStorage(psr, (uint) FlagM.UF, "U", PrimitiveType.Bool);

            fsr = stg.Reg32("fsr");

            mpNameToReg = stg.NamesToRegisters;
            mpDomainToReg = stg.DomainsToRegisters;
        }

        public static RegisterStorage GetRegister(uint r)
        {
            return IntegerRegisters[r & 0x1F];
        }

        public static RegisterStorage GetFpuRegister(int f)
        {
            return FloatRegisters[f];
        }

        public static bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            return mpNameToReg.TryGetValue(regName, out reg);
        }

        public static RegisterStorage GetRegister(StorageDomain domain)
        {
            if (mpDomainToReg.TryGetValue(domain, out var reg))
                return reg;
            else
                return null;
        }

        public static bool IsGpRegister(RegisterStorage reg)
        {
            var iReg = reg.Number;
            return 0 <= iReg && iReg < 32;
        }
    }

    [Flags]
    public enum FlagM : uint
    {
        NF = 8,             // sign
        ZF = 4,             // zero
        VF = 2,             // overflow
        CF = 1,             // carry
        
        EF = 0x80,          // FPU equality
        LF = 0x40,          // FPU less than
        GF = 0x20,          // FPU greater than
        UF = 0x10,          // FPU unordered
    }
}
