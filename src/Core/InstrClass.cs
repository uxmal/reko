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

using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Classifies an instruction based on general execution properties. The
    /// properties can be used by tools without knowing the exact details of a
    /// specific instruction set.
    /// </summary>
    [Flags]
    [NativeInterop]
    public enum InstrClass
    {
        None,

        // Main instruction types
        Linear =      0x0001,   // ALU instruction, computational (like ADD, SHR, or MOVE)
        Transfer =    0x0002,   // Control flow transfer like JMP, CALL
        Terminates =  0x0004,   // Instruction terminates execution.

        // Modifiers
        Conditional = 0x0008,   // Conditionally executed (like branches or CMOV instructions)
        Privileged =  0x0010,   // Privileged instruction

        // Transfer instructions
        Call =        0x0020,   // Instruction saves its continuation, and may resume execution to the following instruction.
        Return =      0x0040,   // Return instruction
        Indirect =    0x0080,   // Indirect transfer instruction
        Delay =       0x0100,   // The following instruction is in a delay slot.
        Annul =       0x0200,   // The following instruction is anulled.

        // Further classification used by scanners
        Padding =     0x0400,   // Instruction _could_ be used as alignment padding between procedures.
        Invalid =     0x0800,   // The instruction is invalid
        Zero =        0x1000,   // The instruction's first "unit" was zero.
        Unlikely =    0x2000,   // The instruction is valid, but unlikely to exist in a real program.

        ConditionalTransfer = Conditional | Transfer,
    }
}
