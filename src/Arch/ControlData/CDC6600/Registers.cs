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

namespace Reko.Arch.ControlData.CDC6600;

public static class Registers
{
    static Registers()
    {
        var factory = new StorageFactory();
        ARegisters = factory.RangeOfReg(8, n => $"A{n}", CDC6600Architecture.Word18);
        BRegisters = factory.RangeOfReg(8, n => $"B{n}", CDC6600Architecture.Word18);
        XRegisters = factory.RangeOfReg(8, n => $"X{n}", CDC6600Architecture.Word60);
        RegisterBank = factory.MakeRegisterBank();
    }

    public static RegisterBank RegisterBank { get; }
    public static RegisterStorage[] ARegisters { get; }
    public static RegisterStorage[] BRegisters { get; }
    public static RegisterStorage[] XRegisters { get; }
}
