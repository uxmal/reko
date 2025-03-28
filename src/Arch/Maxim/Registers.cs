#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Maxim;

public static class Registers
{
    public static Dictionary<string, RegisterStorage> ByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

    public static RegisterStorage[] Accumulators { get; }
    public static RegisterStorage[] DataPointers { get; }
    public static RegisterStorage[] Prefixes { get; }
    public static RegisterStorage[] LoopCounters { get; }
    public static Dictionary<RegisterStorage, string> IndexedNames { get; }
    public static RegisterStorage AP { get; }
    public static RegisterStorage A_AP { get; }
    public static RegisterStorage APC { get; }
    public static RegisterStorage GRL { get; }
    public static RegisterStorage IC { get; }
    public static RegisterStorage IIR { get; }
    public static RegisterStorage IMR { get; }
    public static RegisterStorage MDP { get; }
    public static RegisterStorage PSF { get; }
    public static RegisterStorage SC { get; }
    public static RegisterStorage CKCN { get; }
    public static RegisterStorage WDCN { get; }
    public static RegisterStorage Acc { get; }
    public static RegisterStorage SP { get; }
    public static RegisterStorage IV { get; }
    public static RegisterStorage OFFS { get; }
    public static RegisterStorage IP { get; }
    public static RegisterStorage BP { get; }
    public static RegisterStorage DPC { get; }
    public static RegisterStorage FP { get; }
    public static RegisterStorage GR { get; }
    public static RegisterStorage GRH { get; }
    public static RegisterStorage GRXL { get; }
    public static RegisterStorage GRS { get; }

    public static FlagGroupStorage C { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage S { get; }
    public static FlagGroupStorage E { get; }

    public static FlagGroupStorage CSZ { get; }
    public static FlagGroupStorage CSZE { get; }
    public static FlagGroupStorage CSZV { get; }
    public static FlagGroupStorage SZ { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        Accumulators = factory.RangeOfReg(16, n => $"a{n}", PrimitiveType.Word16);
        DataPointers = factory.RangeOfReg(2, n => $"dp{n}", PrimitiveType.Word16);
        Prefixes = factory.RangeOfReg(8, n => $"pfx{n}", PrimitiveType.Word16);
        LoopCounters = factory.RangeOfReg(2, n => $"lc{n}", PrimitiveType.Word16);
        AP = factory.Reg16("ap");
        A_AP = factory.Reg16("a_ap");
        APC = factory.Reg16("apc");
        GRL = factory.Reg16("grl");
        IC = factory.Reg16("ic");
        IIR = factory.Reg16("iir");
        IMR = factory.Reg16("imr");
        MDP = factory.Reg16("mdp");
        PSF = factory.Reg16("psf");
        SC = factory.Reg16("sc");
        CKCN = factory.Reg16("ckcn");
        WDCN = factory.Reg16("wdcn");
        Acc = factory.Reg16("acc");
        SP = factory.Reg16("sp");
        IV = factory.Reg16("iv");
        OFFS = factory.Reg16("offs");
        IP = factory.Reg16("ip");
        BP = factory.Reg16("bp");
        DPC = factory.Reg16("dpc");
        FP = factory.Reg16("fp");
        GR = factory.Reg16("gr");
        GRH = factory.Reg16("grh");
        GRXL = factory.Reg16("grxl");
        GRS = factory.Reg16("grs");

        C = new FlagGroupStorage(PSF, (uint) FlagM.CF, "c");
        Z = new FlagGroupStorage(PSF, (uint) FlagM.ZF, "z");
        S = new FlagGroupStorage(PSF, (uint) FlagM.SF, "s");
        E = new FlagGroupStorage(PSF, (uint) FlagM.EF, "e");
        CSZ = new FlagGroupStorage(PSF, (uint) (FlagM.CF | FlagM.SF | FlagM.ZF), "csz");
        CSZE = new FlagGroupStorage(PSF, (uint) (FlagM.CF | FlagM.SF | FlagM.ZF | FlagM.EF), "csze");
        CSZV = new FlagGroupStorage(PSF, (uint) (FlagM.CF | FlagM.SF | FlagM.ZF | FlagM.OV), "cszv");
        SZ = new FlagGroupStorage(PSF, (uint) (FlagM.SF | FlagM.ZF), "sz");

        IndexedNames = Accumulators.Select((a, i) => (a, $"a[{i}]"))
            .Concat(DataPointers.Select((dp, i) => (dp, $"dp[{i}]")))
            .Concat(Prefixes.Select((pfx, i) => (pfx, $"pfx[{i}]")))
            .Concat(LoopCounters.Select((pfx, i) => (pfx, $"lc[{i}]")))
            .Concat([(A_AP, "a[ap]")])
            .ToDictionary(p => p.Item1, p => p.Item2);

        ByName = factory.NamesToRegisters;
        ByDomain = factory.DomainsToRegisters;
    }
}

[Flags]
public enum FlagM
{
    EF = 1,
    CF = 2,
    OV = 4,
    GPF0 = 8,
    GPF1 = 16,
    SF = 64,
    ZF = 128,
}
