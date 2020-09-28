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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Reko.Core
{
	/// <summary>
	/// Interface used for backwalkers to get services from host.
	/// </summary>
	public interface IBackWalkHost<TBlock, TInstr>
	{
        IProcessorArchitecture Architecture { get; }
        Program Program { get; }
        SegmentMap SegmentMap { get; }

		AddressRange GetSinglePredecessorAddressRange(Address block);
        int BlockInstructionCount(TBlock rtlBlock);
        Address GetBlockStartAddress(Address addr);
        Address MakeAddressFromConstant(Constant c);
        Address MakeSegmentedAddress(Constant selector, Constant offset);

        TBlock GetSinglePredecessor(TBlock block);
        List<TBlock> GetPredecessors(TBlock block);

        bool IsValidAddress(Address addr);
        RegisterStorage GetSubregister(RegisterStorage rIdx, int v1, int v2);
        IEnumerable<TInstr> GetBlockInstructions(TBlock block);

        // Return [dst,src] tuple if TInstr is an assignment, null otherwise.
        Tuple<Expression,Expression> AsAssignment(TInstr instr);
        // Return the branch condition if TINstr is a branch, null otherwise.
        Expression AsBranch(TInstr instr);

        bool IsStackRegister(Storage storage);
        bool IsFallthrough(TInstr instr, TBlock block);
    }
}
