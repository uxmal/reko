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

namespace Reko.Arch.M16C;

public static class Registers
{
    public static RegisterStorage r0 { get; }
    public static RegisterStorage r1 { get; }
    public static RegisterStorage r2 { get; }
    public static RegisterStorage r3 { get; }
    public static RegisterStorage a0 { get; }
    public static RegisterStorage a1 { get; }
    public static RegisterStorage fb { get; }
    public static RegisterStorage sb { get; }
    public static RegisterStorage usp { get; }
    public static RegisterStorage isp { get; }
    public static RegisterStorage flg { get; }
    public static RegisterStorage intbl { get; }
    public static RegisterStorage intbh { get; }

    public static RegisterStorage r0l { get; }
    public static RegisterStorage r0h { get; }
    public static RegisterStorage r1l { get; }
    public static RegisterStorage r1h { get; }

    public static FlagGroupStorage B { get; }
    public static FlagGroupStorage C { get; }
    public static FlagGroupStorage D { get; }
    public static FlagGroupStorage I { get; }
    public static FlagGroupStorage O { get; }
    public static FlagGroupStorage OS { get; }
    public static FlagGroupStorage OSZ { get; }
    public static FlagGroupStorage OSZC { get; }
    public static FlagGroupStorage S { get; }
    public static FlagGroupStorage SZ { get; }
    public static FlagGroupStorage SZC { get; }
    public static FlagGroupStorage U { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage ZC { get; }

    public static SequenceStorage a1a0 { get; }
    public static SequenceStorage r1r0 { get; }
    public static SequenceStorage r2r0 { get; }
    public static SequenceStorage r3r1 { get; }


    public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
    public static Dictionary<string, RegisterStorage> ByName { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        r0 = factory.Reg16("r0");
        r1 = factory.Reg16("r1");
        r2 = factory.Reg16("r2");
        r3 = factory.Reg16("r3");
        a0 = factory.Reg16("a0");
        a1 = factory.Reg16("a1");
        fb = factory.Reg16("fb");
        sb = factory.Reg16("sb");
        usp = factory.Reg16("usp");
        isp = factory.Reg16("isp");
        flg = factory.Reg16("flg");
        intbl = factory.Reg16("intbl");
        intbh = factory.Reg16("intbh");

        r0l = new RegisterStorage("r0l", r0.Number, 0, PrimitiveType.Byte);
        r0h = new RegisterStorage("r0h", r0.Number, 8, PrimitiveType.Byte);
        r1l = new RegisterStorage("r1l", r1.Number, 0, PrimitiveType.Byte);
        r1h = new RegisterStorage("r1h", r1.Number, 8, PrimitiveType.Byte);

        B = new FlagGroupStorage(flg, (uint) FlagM.BF, "B");
        C = new FlagGroupStorage(flg, (uint) FlagM.CF, "C");
        D = new FlagGroupStorage(flg, (uint) FlagM.DF, "D");
        I = new FlagGroupStorage(flg, (uint) FlagM.IF, "I");
        O = new FlagGroupStorage(flg, (uint) FlagM.OF, "O");
        OS = new FlagGroupStorage(flg, (uint) (FlagM.SF | FlagM.OF), "OS");
        OSZ = new FlagGroupStorage(flg, (uint) (FlagM.SF | FlagM.OF | FlagM.ZF), "OSZ");
        OSZC = new FlagGroupStorage(flg, (uint) (FlagM.SF | FlagM.OF | FlagM.ZF | FlagM.CF), "OSZC");
        S = new FlagGroupStorage(flg, (uint) FlagM.SF, "S");
        SZ = new FlagGroupStorage(flg, (uint) (FlagM.SF | FlagM.ZF), "SZ");
        SZC = new FlagGroupStorage(flg, (uint) (FlagM.SF | FlagM.ZF | FlagM.CF), "SZC");
        U = new FlagGroupStorage(flg, (uint) FlagM.UF, "U");
        Z = new FlagGroupStorage(flg, (uint) FlagM.ZF, "Z");
        ZC = new FlagGroupStorage(flg, (uint) (FlagM.ZF | FlagM.CF), "ZC");

        a1a0 = new SequenceStorage("a1a0", PrimitiveType.Word32, a1, r0);
        r1r0 = new SequenceStorage("r1r0", PrimitiveType.Word32, r1, r0);
        r2r0 = new SequenceStorage("r2r0", PrimitiveType.Word32, r2, r0);
        r3r1 = new SequenceStorage("r3r1", PrimitiveType.Word32, r3, r1);

        ByDomain = new[] {
            r0,
            r1,
            r2,
            r3,
            a0,
            a1,
            fb,
            sb,
            usp,
            isp,
            flg,
        }.ToDictionary(r => r.Domain);
        ByName = new[] {
            r0,
            r1,
            r2,
            r3,
            a0,
            a1,
            fb,
            sb,
            usp,
            isp,
            flg,
            r0l,
            r0h,
            r1l,
            r1h,
        }.ToDictionary(r => r.Name);
    }
}

[Flags]
public enum FlagM
{
    CF = 1,
    DF = 2,
    ZF = 4,
    SF = 8,
    BF = 16,
    OF = 32,
    IF = 64,
    UF = 128
}
