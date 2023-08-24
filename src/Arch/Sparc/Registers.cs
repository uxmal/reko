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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public readonly FlagGroupStorage NV;
        public readonly FlagGroupStorage NZ;
        public readonly FlagGroupStorage NZV;
        public readonly FlagGroupStorage NZVC;
        public readonly FlagGroupStorage ZC;

        public readonly FlagGroupStorage xN;
        public readonly FlagGroupStorage xZ;
        public readonly FlagGroupStorage xV;
        public readonly FlagGroupStorage xC;
        public readonly FlagGroupStorage xNV;
        public readonly FlagGroupStorage xNZ;
        public readonly FlagGroupStorage xNZV;
        public readonly FlagGroupStorage xNZVC;
        public readonly FlagGroupStorage xZC;

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
                    RegisterStorage.Reg64($"d{i * 2}", q.Number, 64),
                    RegisterStorage.Reg64($"d{i * 2 + 1}", q.Number, 0),
                };
            }).ToArray();
            FFloatRegisters= QFloatRegisters.Take(8).SelectMany((q, i) =>
            {
                return new[]
                {
                    RegisterStorage.Reg32($"f{i * 4}", q.Number, 96),
                    RegisterStorage.Reg32($"f{i * 4 + 1}", q.Number, 64),
                    RegisterStorage.Reg32($"f{i * 4 + 2}", q.Number, 32),
                    RegisterStorage.Reg32($"f{i * 4 + 3}", q.Number, 0),
                };
            }).ToArray();

            psr = stg.Reg32("psr");

            N = new FlagGroupStorage(psr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            Z = new FlagGroupStorage(psr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            V = new FlagGroupStorage(psr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            C = new FlagGroupStorage(psr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            NV = new FlagGroupStorage(psr, (uint) (FlagM.NF | FlagM.VF), "NV", PrimitiveType.Byte);
            NZ = new FlagGroupStorage(psr, (uint) (FlagM.NF | FlagM.ZF), "NZ", PrimitiveType.Byte);
            NZV = new FlagGroupStorage(psr, (uint) (FlagM.NF | FlagM.ZF | FlagM.VF), "NZV", PrimitiveType.Byte);
            NZVC = new FlagGroupStorage(psr, (uint) (FlagM.NF | FlagM.ZF |FlagM.VF | FlagM.CF), "NZVC", PrimitiveType.Byte);
            ZC = new FlagGroupStorage(psr, (uint) (FlagM.ZF | FlagM.CF), "ZC", PrimitiveType.Byte);

            xN = new FlagGroupStorage(psr, (uint) FlagM.xNF, "xN", PrimitiveType.Bool);
            xZ = new FlagGroupStorage(psr, (uint) FlagM.xZF, "xZ", PrimitiveType.Bool);
            xV = new FlagGroupStorage(psr, (uint) FlagM.xVF, "xV", PrimitiveType.Bool);
            xC = new FlagGroupStorage(psr, (uint) FlagM.xCF, "xC", PrimitiveType.Bool);
            xNV = new FlagGroupStorage(psr, (uint) (FlagM.xNF | FlagM.xVF), "xNV", PrimitiveType.Byte);
            xNZ = new FlagGroupStorage(psr, (uint) (FlagM.xNF | FlagM.xZF), "xNZ", PrimitiveType.Byte);
            xNZV = new FlagGroupStorage(psr, (uint) (FlagM.xNF | FlagM.xZF | FlagM.xVF), "xNZV", PrimitiveType.Byte);
            xNZVC = new FlagGroupStorage(psr, (uint) (FlagM.xNF | FlagM.xZF | FlagM.xVF | FlagM.xCF), "xNZVC", PrimitiveType.Byte);
            xZC = new FlagGroupStorage(psr, (uint) (FlagM.xZF | FlagM.xCF), "xZC", PrimitiveType.Byte);


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

        public bool TryGetRegister(string regName, [MaybeNullWhen(false)] out RegisterStorage reg)
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

        xNF = 0x800,        // sign     (64-bit)
        xZF = 0x400,        // zero     (64-bit)
        xVF = 0x200,        // overflow (64-bit)
        xCF = 0x100,        // carry    (64-bit)
    }
}
