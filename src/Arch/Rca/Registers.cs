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

namespace Reko.Arch.Rca;

public static class Registers
{
    public static RegisterStorage D { get; }
    public static RegisterStorage T { get; }
    public static RegisterStorage X { get; }

    public static RegisterBank RegisterBank { get; }
    public static RegisterStorage[] GpRegisters { get; }

    public static RegisterStorage Status { get; }

    public static FlagGroupStorage DF { get; }


    static Registers()
    {
        var factory = new StorageFactory();
        GpRegisters = factory.RangeOfReg(16, r => $"R{r}", PrimitiveType.Word16);
        D = factory.Reg8("D");
        T = factory.Reg8("T");
        var nybble = PrimitiveType.CreateWord(4);
        X = factory.Reg("X", nybble);
        Status = factory.Reg("Status", PrimitiveType.Byte);
        DF = new FlagGroupStorage(Status, 0xF, "DF");

        RegisterBank = factory.MakeRegisterBank();
    }
}
