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
using Reko.Core.Types;

namespace Reko.Arch.Oki.NX8_200;

public static class Registers
{
    public static Dictionary<string, RegisterStorage>? ByName { get; internal set; }
    public static Dictionary<StorageDomain, RegisterStorage>? ByDomain { get; internal set; }
    public static RegisterStorage Acc { get; }
    public static RegisterStorage Psw { get; }
    public static RegisterStorage Pswl { get; }
    public static RegisterStorage Pswh { get; }

    /// <summary>
    /// Program counter register.
    /// </summary>
    public static RegisterStorage Pc { get; }

    /// <summary>
    /// Local register base.
    /// </summary>
    public static RegisterStorage Lrb { get; }
    public static RegisterStorage Ssp { get; }
    public static RegisterStorage X1 { get; }
    public static RegisterStorage X2 { get; }
    public static RegisterStorage Dp { get; }
    public static RegisterStorage Usp { get; }
    public static RegisterStorage[] ERegisters { get; }
    public static RegisterStorage[] BRegisters { get; }
    public static RegisterStorage Csr { get; }
    public static RegisterStorage Tsr { get; }
    public static RegisterStorage Dsr { get; }


    public static FlagGroupStorage C { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage CZ { get; }
    public static FlagGroupStorage CZSV { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        Registers.Acc = factory.Reg16("a");
        Registers.Psw = factory.Reg16("psw");
        Registers.Pc = factory.Reg16("pc");
        Registers.Lrb = factory.Reg16("lrb");
        Registers.Ssp = factory.Reg16("ssp");

        Registers.X1 = factory.Reg16("x1");
        Registers.X2 = factory.Reg16("x2");
        Registers.Dp = factory.Reg16("dp");
        Registers.Usp = factory.Reg16("usp");

        Registers.ERegisters = factory.RangeOfReg(4, n => $"er{n}", PrimitiveType.Word16);

        Registers.Csr = factory.Reg("csr", PrimitiveType.Byte);
        Registers.Tsr = factory.Reg("tsr", PrimitiveType.Byte);
        Registers.Dsr = factory.Reg("dsr", PrimitiveType.Byte);


        BRegisters = ERegisters.SelectMany((er, i) => new[]{
                RegisterStorage.Reg8($"r{i*2}", er.Number, 0),
                RegisterStorage.Reg8($"r{i*2+1}", er.Number, 8)
            })
            .ToArray();
        Pswl = new RegisterStorage("pswl", Psw.Number, 0, PrimitiveType.Byte);
        Pswh = new RegisterStorage("pswh", Psw.Number, 8, PrimitiveType.Byte);

        C = new FlagGroupStorage(Psw, (uint) FlagM.C, "C");
        Z = new FlagGroupStorage(Psw, (uint) FlagM.Z, "Z");
        CZ = new FlagGroupStorage(Psw, (uint)(FlagM.C | FlagM.Z), "CZ");
        CZSV = new FlagGroupStorage(Psw, (uint)(FlagM.C | FlagM.Z | FlagM.S | FlagM.OV), "CZSV");
    }

    [Flags]
    public enum FlagM
    {
        SCB0 = 0x01,
        SCB1 = 0x02,
        SCB2 = 0x04,
        F0 = 0x08,

        BCB0 = 0x10,
        BCB1 = 0x20,
        F1 = 0x40,
        MAB = 0x80,

        MIE = 0x100,
        OV = 0x200,
        MIP = 0x400,
        S = 0x800,
        DD = 0x1000,
        HC = 0x2000,
        Z = 0x4000,
        C = 0x8000,


    }
}
