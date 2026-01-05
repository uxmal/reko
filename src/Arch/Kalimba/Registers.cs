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

namespace Reko.Arch.Kalimba;

public static class Registers
{
    public static Dictionary<string, RegisterStorage>? ByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }
    public static RegisterStorage Null { get; }
    public static RegisterStorage rMAC { get; }
    public static RegisterStorage[] GpRegisters { get; }
    public static RegisterStorage rMAC0 { get; }
    public static RegisterStorage rMAC1 { get; }
    public static RegisterStorage rMAC2 { get; }
    public static RegisterStorage rMAC12 { get; }
    public static RegisterStorage rLink { get; }
    public static RegisterStorage rFlags { get; }
    public static RegisterStorage rIntLink { get; }
    public static RegisterStorage[] IndexRegisters { get; }
    public static RegisterStorage[] ModifyRegisters { get; }
    public static FlagGroupStorage Carry { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        Null = factory.Reg("Null", KalimbaArchitecture.Word24);
        rMAC = factory.Reg("rMAC", KalimbaArchitecture.Word56);
        GpRegisters = factory.RangeOfReg(11, r => $"r{r}", KalimbaArchitecture.Word24);
        rLink = factory.Reg16("rLink");
        rFlags = factory.Reg16("rFlags");
        rIntLink = factory.Reg16("rIntLink");

        // Bank 2

        IndexRegisters = factory.RangeOfReg(8, r => $"i{r}", PrimitiveType.Word16);
        ModifyRegisters = factory.RangeOfReg(4, r => $"m{r}", PrimitiveType.Word16);
        factory.RangeOfReg(4, r => $"l{r}", PrimitiveType.Word16);

        Carry = new FlagGroupStorage(rFlags, (uint)FlagM.CF, "Carry");

        // Aliases

        rMAC0 = new RegisterStorage("rMAC0", rMAC.Number, 0,  KalimbaArchitecture.Word24);
        rMAC1 = new RegisterStorage("rMAC1", rMAC.Number, 24, KalimbaArchitecture.Word24);
        rMAC2 = new RegisterStorage("rMAC2", rMAC.Number, 48, PrimitiveType.Byte);
        rMAC12 = new RegisterStorage("rMAC12", rMAC.Number, 24, PrimitiveType.Word32);

        ByDomain = factory.DomainsToRegisters;
        ByName = factory.NamesToRegisters.Values.Concat([
            rMAC0,
                        rMAC1,
                        rMAC2,
                        rMAC12
            ])
            .ToDictionary(r => r.Name);
    }
}

[Flags]
public enum FlagM
{
    NF = 1,
    ZF = 2,
    CF = 4,
    VF = 8,
    UDF = 16,   // User defined
    SVF = 32,   // sticky overflow
    BRF = 64,   // bit-reverse
    UMF = 128,  // User mode flag

}
