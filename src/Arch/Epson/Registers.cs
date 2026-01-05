#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Epson;

public static class Registers
{
    public static RegisterStorage[] GpRegisters { get; }
    public static RegisterStorage PSR { get; } 
    public static RegisterStorage SP { get; } 
    public static RegisterStorage ALR { get; } 
    public static RegisterStorage AHR { get; } 
    public static RegisterStorage TTBR { get; }
    public static RegisterStorage IDIR { get; }
    public static RegisterStorage DBBR { get; }
    public static RegisterStorage PC { get; }
    public static Dictionary<string, RegisterStorage>? ByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage>? ByDomain { get; }


    public static FlagGroupStorage C { get; }
    public static FlagGroupStorage CNVZ { get; }
    public static FlagGroupStorage CZ { get; }
    public static FlagGroupStorage NV { get; }
    public static FlagGroupStorage NVZ { get; }
    public static FlagGroupStorage NZ { get; }
    public static FlagGroupStorage Z { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        GpRegisters = factory.RangeOfReg32(32, "r{0}");

        var sysFactory = new StorageFactory(StorageDomain.SystemRegister);
        PSR = sysFactory.Reg32("psr");
        SP = sysFactory.Reg32("sp");
        ALR = sysFactory.Reg32("alr");
        AHR = sysFactory.Reg32("ahr");
        TTBR  = sysFactory.Reg32("ttbr");
        IDIR = sysFactory.Reg32("idir");
        DBBR = sysFactory.Reg32("dbbr");
        PC = sysFactory.Reg32("pc");

        C = new FlagGroupStorage(PSR, (uint) (FlagM.CF), "C");
        CZ = new FlagGroupStorage(PSR, (uint) (FlagM.CF|FlagM.ZF), "CZ");
        CNVZ = new FlagGroupStorage(PSR, (uint) (FlagM.CF|FlagM.NF|FlagM.VF|FlagM.ZF), "CNVZ");
        NV = new FlagGroupStorage(PSR, (uint) (FlagM.NF|FlagM.VF), "NV");
        NVZ = new FlagGroupStorage(PSR, (uint) (FlagM.NF|FlagM.VF|FlagM.ZF), "NVZ");
        NZ = new FlagGroupStorage(PSR, (uint) (FlagM.NF|FlagM.ZF), "NZ");
        Z = new FlagGroupStorage(PSR, (uint) FlagM.ZF, "Z");

        ByName = factory.NamesToRegisters
            .Concat(sysFactory.NamesToRegisters)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        ByDomain = factory.DomainsToRegisters
            .Concat(sysFactory.DomainsToRegisters)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    internal static bool IsGpRegister(RegisterStorage reg)
    {
        return (uint) reg.Number < (uint)GpRegisters.Length;
    }
}

[Flags]
public enum FlagM : uint
{
    NF = 1,
    ZF = 2,
    VF =4,
    CF = 8,
}
