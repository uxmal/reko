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
        /// <summary>
        /// Undefined instruction class.
        /// </summary>
        None,

        // Main instruction types

        /// <summary>
        /// ALU instruction, computational (like ADD, SHR, or MOVE).
        /// </summary>
        Linear =      0x0001,

        /// <summary>
        /// Control flow transfer like JMP, CALL.
        /// </summary>
        Transfer =    0x0002,

        /// <summary>
        /// Instruction terminates execution.
        /// </summary>
        Terminates =  0x0004,

        // Modifiers

        /// <summary>
        /// Conditionally executed (like branches or CMOV instructions)
        /// </summary>
        Conditional = 0x0008,

        /// <summary>
        /// Privileged instruction.
        /// </summary>
        Privileged =  0x0010,

        // Transfer instructions

        /// <summary>
        /// Instruction saves its continuation, and may resume execution to the following instruction.
        /// </summary>
        Call =        0x0020,

        /// <summary>
        /// Return instruction.
        /// </summary>
        Return =      0x0040,

        /// <summary>
        /// Indirect transfer instruction.
        /// </summary>
        Indirect =    0x0080,

        /// <summary>
        /// The following instruction is in a delay slot.
        /// </summary>
        Delay =       0x0100,

        /// <summary>
        /// The following instruction is anulled.
        /// </summary>
        Annul =       0x0200,

        // Further classification used by scanners

        /// <summary>
        /// Instruction <i>could</i> be used as alignment padding between procedures.
        /// </summary>
        Padding =     0x0400,

        /// <summary>
        /// The instruction is invalid.
        /// </summary>
        Invalid = 0x0800,

        /// <summary>
        /// The instruction's first "unit" was zero.
        /// </summary>
        Zero =        0x1000,
        
        /// <summary>
        /// The instruction is valid, but unlikely to exist in a real program.
        /// </summary>
        Unlikely =    0x2000,

        /// <summary>
        /// Instruction is a conditional transfer instruction.
        /// </summary>
        ConditionalTransfer = Conditional | Transfer,
    }
}
