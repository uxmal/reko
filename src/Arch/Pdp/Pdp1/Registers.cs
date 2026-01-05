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

using Reko.Arch.Pdp.Memory;
using Reko.Core;

namespace Reko.Arch.Pdp.Pdp1;

public static class Registers
{
    public static RegisterStorage Acc { get; }
    public static RegisterStorage IO { get; }
    public static RegisterStorage Psw { get; }
    public static FlagGroupStorage V { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        Acc = factory.Reg("acc", PdpTypes.Word18);
        IO = factory.Reg("io", PdpTypes.Word18);
        Psw = factory.Reg("psw", PdpTypes.Word18);

        V = new FlagGroupStorage(Psw, 1, "V");
    }
}
