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
using Reko.Core.Lib;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Interface used for backwalkers to get services from host.
    /// </summary>
    public interface IBackWalkHost<TBlock, TInstr>
        where TInstr : class
    {
        /// <summary>
        /// <see cref="IProcessorArchitecture"/> to use when
        /// backwalking RTL blocks.
        /// </summary>
        IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// The <see cref="Program"/> being analyzed.
        /// </summary>
        Program Program { get; }

        /// <summary>
        /// Count the number of instructions in the given RTL block.
        /// </summary>
        /// <param name="rtlBlock">Basic block.</param>
        /// <returns>The number of instructions in <paramref name="rtlBlock"/>.
        /// </returns>
        int BlockInstructionCount(TBlock rtlBlock);

        /// <summary>
        /// Given a constant, return an address if it can be interpreted as such.
        /// </summary>
        /// <param name="c">Constant to be reinterpreted.</param>
        /// <returns>A corresponding <see cref="Address"/> if the environment and
        /// processor allow it.</returns>
        Address? MakeAddressFromConstant(Constant c);

        /// <summary>
        /// Given a <paramref name="selector"/> and an <paramref name="offset"/>,
        /// returns a segmented address.
        /// </summary>
        /// <param name="selector">Selector to use for the address.</param>
        /// <param name="offset">Offset to use for the address.</param>
        /// <returns>A segment address.</returns>
        Address MakeSegmentedAddress(Constant selector, Constant offset);

        /// <summary>
        /// Gets the single predecessor of the given block, if it exists.
        /// </summary>
        /// <param name="block">Basic block to test.</param>
        /// <returns>If the block has exactly one predecessor, returns it;
        /// if the block has more than one predecessor, or no predecessors,
        /// returns null.
        /// </returns>
        TBlock? GetSinglePredecessor(TBlock block);

        /// <summary>
        /// Gets a list of all predecessors of the given basic block.
        /// </summary>
        /// <param name="block">Basic block to test.</param>
        /// <returns>All predecessor basic blocks of <paramref name="block"/>.
        /// </returns>
        List<TBlock> GetPredecessors(TBlock block);

        /// <summary>
        /// Deterimines whether the given address is valid in the current
        /// program.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>True if the address is a valid address; false if not.
        /// </returns>
        bool IsValidAddress(Address addr);

        /// <summary>
        /// Given a register <paramref name="reg"/> and a bit range <paramref name="range"/>,
        /// extracts a subregister from the register storage if it is supported by the 
        /// current processor architecture.
        /// </summary>
        /// <param name="reg">Register whose possible subregister we are interested in.
        /// </param>
        /// <param name="range">Bitrange to extract.</param>
        /// <returns>The smallest sub-register of <paramref name="reg"/> that covers the
        /// bit range <paramref name="range"/>.
        /// </returns>
        RegisterStorage GetSubregister(RegisterStorage reg, BitRange range);

        /// <summary>
        /// Gets a list of instructions in the given block, in asecending address order.
        /// </summary>
        /// <param name="block">Block whose instructions are to be extracted.
        /// </param>
        /// <returns></returns>
        IEnumerable<(Address, TInstr?)> GetBlockInstructions(TBlock block);

        /// <summary>
        /// Return [dst,src] tuple if <paramref name="instr"/> is an assignment;
        /// otherwise null.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        (Expression?, Expression?) AsAssignment(TInstr instr);

        /// <summary>
        /// Return the branch condition if <paramref name="instr"/> is a branch, null otherwise.
        /// </summary>
        Expression? AsBranch(TInstr instr);

        /// <summary>
        /// Tests whether the given storage is the stack register.
        /// </summary>
        /// <param name="storage"><see cref="Storage"/> to test.</param>
        /// <returns>True if <paramref name="storage"/> is the stack register;
        /// otherwise false.</returns>
        bool IsStackRegister(Storage storage);

        /// <summary>
        /// Tests whether the <paramref name="block"/> is the fallthrough
        /// (false) branch of the instruction <paramref name="instr"/>.
        /// </summary>
        /// <param name="instr">Instruction to test.</param>
        /// <param name="block">Possible fallthrough block.</param>
        /// <returns>True if the <paramref name="instr"/> is a branch instruction
        /// that falls into <paramref name="block"/> if the branch condition
        /// is false.</returns>
        bool IsFallthrough(TInstr instr, TBlock block);
    }
}
