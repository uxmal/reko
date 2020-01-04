#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Classifies an instruction based on general execution properties. The properties can be used
    /// by tools without knowing the exact details of a specific instruction set.
    /// </summary>
    [Flags]
    [NativeInterop]
    public enum InstrClass
    {
        None,
        Linear  = 1,            // ALU instruction, computational (like ADD, SHR, or MOVE)
        Transfer = 2,           // Control flow transfer like JMP, CALL
        Conditional = 4,        // Conditionally executed (like branches or CMOV instructions)
        Call = 8,               // Instruction saves its continuation, and may resume execution to the following instruction.
        Delay = 16,             // The following instruction is in a delay slot.
        Annul = 32,             // The following instruction is anulled.
        Terminates = 64,        // Instruction terminates execution.
        System = 128,           // Privileged instruction
        Padding = 256,          // Instruction _could_ be used as alignment padding between procedures.
        Invalid = 512,          // The instruction is invalid
        Zero = 1024,            // The instruction first "unit" was zero.

        ConditionalTransfer = Conditional | Transfer,
    }
}
