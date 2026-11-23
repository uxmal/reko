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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Sharp;

public static class Registers
{
    public static RegisterStorage A { get; }
    public static RegisterStorage U { get; }
    public static RegisterStorage X { get; }
    public static RegisterStorage Y { get; }
    public static RegisterStorage S { get; }
    public static RegisterStorage P { get; }
    public static RegisterStorage UL { get; }
    public static RegisterStorage UH { get; }
    public static RegisterStorage XL { get; }
    public static RegisterStorage XH { get; }
    public static RegisterStorage YL { get; }
    public static RegisterStorage YH { get; }


    public static RegisterStorage Status { get; }

    public static FlagGroupStorage C { get; }
    public static FlagGroupStorage IE { get; }
    public static FlagGroupStorage Z { get; }
    public static FlagGroupStorage V { get; }
    public static FlagGroupStorage H { get; }
    public static FlagGroupStorage CZVH { get; }
    public static RegisterBank RegisterBank { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        A = factory.Reg8("a");
        U = factory.Reg16("u");
        X = factory.Reg16("x");
        Y = factory.Reg16("y");
        S = factory.Reg16("s");
        P = factory.Reg16("p");
        Status = factory.Reg8("status");
        UL = new RegisterStorage("ul", U.Number, 0, PrimitiveType.Byte);
        UH = new RegisterStorage("uh", U.Number, 8, PrimitiveType.Byte);
        XL = new RegisterStorage("xl", X.Number, 0, PrimitiveType.Byte);
        XH = new RegisterStorage("xh", X.Number, 8, PrimitiveType.Byte);
        YL = new RegisterStorage("yl", Y.Number, 0, PrimitiveType.Byte);
        YH = new RegisterStorage("yh", Y.Number, 8, PrimitiveType.Byte);

        C = new FlagGroupStorage(Status, 1, "C");
        IE = new FlagGroupStorage(Status, 2, "I");
        Z = new FlagGroupStorage(Status, 4, "Z");
        V = new FlagGroupStorage(Status, 8, "V");
        H = new FlagGroupStorage(Status, 16, "H");

        CZVH = new FlagGroupStorage(Status, 0x1D, "CZVH");

        RegisterBank = new RegisterBank(
            factory.DomainsToRegisters.Values
            .Concat([ UL, UH, XL, XH, YL, YH ]));
    }
}
