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
    public class Registers
    {
        public readonly RegisterStorage g0;
        public readonly RegisterStorage sp;   // stack pointer
        public readonly RegisterStorage o7;

        public readonly RegisterStorage[] OutRegisters;
        public readonly RegisterStorage[] InRegisters;
        public readonly RegisterStorage y;

        public readonly RegisterStorage psr;
        public readonly RegisterStorage fsr;

        public readonly FlagGroupStorage N;
        public readonly FlagGroupStorage Z;
        public readonly FlagGroupStorage V;
        public readonly FlagGroupStorage C;

        public readonly FlagGroupStorage E;
        public readonly FlagGroupStorage L;
        public readonly FlagGroupStorage G;
        public readonly FlagGroupStorage U;

        public readonly RegisterStorage[] IntegerRegisters;
        public readonly RegisterStorage[] QFloatRegisters;
        public readonly RegisterStorage[] DFloatRegisters;
        public readonly RegisterStorage[] FFloatRegisters;

        private readonly Dictionary<string, RegisterStorage> mpNameToReg;
        private readonly Dictionary<StorageDomain, RegisterStorage> mpDomainToReg;

        public Registers(PrimitiveType wordSize)
        {
            var stg = new StorageFactory();

            var globRegs = stg.RangeOfReg(8, n => $"g{n}", wordSize);
            g0 = globRegs[0];
            // outgoing parameter 0 / return value from callee
            OutRegisters = stg.RangeOfReg(8, n => n == 6 ? "sp" : $"o{n}", wordSize);
            sp = OutRegisters[6];
            o7 = OutRegisters[7];

            var localRegs = stg.RangeOfReg(8, n => $"l{n}", wordSize);
            // incoming parameters / return value to caller
            // i6 = frame pointer
            // i7 = return address - 8
            InRegisters = stg.RangeOfReg(8, n => $"i{n}", wordSize);

            y = stg.Reg("y", wordSize);

            IntegerRegisters = globRegs
                .Concat(OutRegisters)
                .Concat(localRegs)
                .Concat(InRegisters)
                .Concat(new[] { y })
                .ToArray();

            // Sparc floating point registers can contain integers, which is 
            // why they can't be real32. This also forces our hand into
            // making float-point versions of add, sub, mul, div. 

            QFloatRegisters = stg.RangeOfReg(16, n => $"q{n*4}", PrimitiveType.Word128);
            DFloatRegisters = QFloatRegisters.SelectMany((q, i) =>
            {
                return new[]
                {
                    new RegisterStorage($"d{i * 2}", q.Number, 64, PrimitiveType.Word64),
                    new RegisterStorage($"d{i * 2 + 1}", q.Number, 0, PrimitiveType.Word64),
                };
            }).ToArray();
            FFloatRegisters= QFloatRegisters.Take(8).SelectMany((q, i) =>
            {
                return new[]
                {
                    new RegisterStorage($"f{i * 4}", q.Number, 96, PrimitiveType.Word32),
                    new RegisterStorage($"f{i * 4 + 1}", q.Number, 64, PrimitiveType.Word32),
                    new RegisterStorage($"f{i * 4 + 2}", q.Number, 32, PrimitiveType.Word32),
                    new RegisterStorage($"f{i * 4 + 3}", q.Number, 0, PrimitiveType.Word32),
                };
            }).ToArray();

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
            foreach (var fpreg in DFloatRegisters.Concat(FFloatRegisters))
            {
                mpNameToReg.Add(fpreg.Name, fpreg);
            }
            mpDomainToReg = stg.DomainsToRegisters;
        }

        public RegisterStorage GetRegister(uint r)
        {
            return IntegerRegisters[r & 0x1F];
        }

        public bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            return mpNameToReg.TryGetValue(regName, out reg);
        }

        public RegisterStorage? GetRegister(StorageDomain domain)
        {
            if (mpDomainToReg.TryGetValue(domain, out var reg))
                return reg;
            else
                return null;
        }

        public bool IsGpRegister(RegisterStorage reg)
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
