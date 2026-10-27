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

using Reko.Arch.Dec.Memory;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Dec.Linc;

public static class Registers
{
    public static RegisterBank RegisterBank { get; }
    public static RegisterStorage[] IndexRegisters { get; }
    public static RegisterStorage Accumulator { get; }

    public static RegisterStorage Mq { get; }

    /// <summary>
    /// Tape accumulator
    /// </summary>
    public static RegisterStorage Tac { get; }

    /// <summary>
    /// Link bit (think of it as a carry bit).
    /// </summary>
    public static FlagGroupStorage L { get; }

    /// <summary>
    /// Gets the flag group storage for flow-related flags.
    /// </summary>
    public static FlagGroupStorage Flow { get; }


    /// <summary>
    /// Fictitious status register to group bits, not present in the
    /// actual architecture.
    /// </summary>
    public static RegisterStorage Status { get; }

    /// <summary>
    /// Gets the stack pointer register storage.
    /// </summary>
    /// <remarks>Set access is restricted to the containing assembly.</remarks>
    public static RegisterStorage SP { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        IndexRegisters = factory.RangeOfReg(16, n => n == 0 ? "PC" : $"R{n}", PdpTypes.Word12);
        Accumulator = factory.Reg("AC", LincArchitecture.Word12);
        Mq = factory.Reg("MQ", LincArchitecture.Word12);                // PDP-12?
        Tac = factory.Reg("TAC", LincArchitecture.Word12);
        Status = factory.Reg("Status", LincArchitecture.Word12);
        SP = factory.Reg("SP", LincArchitecture.Word12);
        L = new FlagGroupStorage(Status, 1, "L");
        Flow = new FlagGroupStorage(Status, 2, "F");
        RegisterBank = new RegisterBank(factory.DomainsToRegisters.Values);
    }
}
