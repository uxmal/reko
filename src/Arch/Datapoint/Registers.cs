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
using Reko.Core.Machine;
using System.Linq;

namespace Reko.Arch.Datapoint;

public static class Registers
{
    public static RegisterStorage A { get; }
    public static RegisterStorage B { get; }
    public static RegisterStorage C { get; }
    public static RegisterStorage D { get; }
    public static RegisterStorage E { get; }
    public static RegisterStorage H { get; }
    public static RegisterStorage L { get; }

    public static RegisterStorage HL { get; }

    public static RegisterBank RegisterBank { get; }

    /// <summary>
    /// Gets the stack pointer register storage.
    /// </summary>
    public static RegisterStorage Sp { get; }
    public static RegisterStorage Status { get; }

    public static FlagGroupStorage Cy { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage S { get; }
    public static FlagGroupStorage P { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        A = factory.Reg8("A");
        B = factory.Reg8("B");
        C = factory.Reg8("C");
        D = factory.Reg8("D");
        E = factory.Reg8("E");
        HL = factory.Reg16("HL");
        H = RegisterStorage.Reg8("H", HL.Number, 8);
        L = RegisterStorage.Reg8("L", HL.Number, 0);
        Sp = factory.Reg16("SP");
        Status = factory.Reg8("STATUS");
        RegisterBank = new RegisterBank(
            factory.DomainsToRegisters.Values
            .Concat([H, L]));

        Cy = new FlagGroupStorage(Status, 1, "C");
        Z = new FlagGroupStorage(Status, 2, "Z");
        S = new FlagGroupStorage(Status, 4, "S");
        P = new FlagGroupStorage(Status, 8, "P");
    }
}
