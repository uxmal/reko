#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// A class implementing IScanner is responsible for tracing the execution
    /// flow of a binary image. It uses a Rewriter to convert the machine-
    /// specific instructions into lower-level register-transfer (RTL) 
    /// instructions. By simulating the execution of branch and call
    /// instructions, the IScanner can reconstruct the basic blocks and 
    /// procedures that constituted the original source program. Any 
    /// discoveries made are added to the Program instance.
    /// </summary>
    public interface IScanner : IScannerQueue
    {
        IServiceProvider Services { get; }

        void ScanImage();

        ProcedureBase ScanProcedure(Address addr, string procedureName, ProcessorState state);

        Block EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state);
        Address EnqueueUserProcedure(Procedure_v1 sp);

        ExternalProcedure GetImportedProcedure(Address addrImportThunk, Address addrInstruction);
        void TerminateBlock(Block block, Address addrEnd);

        /// <summary>
        /// Find the block that contains the address <paramref name="addr"/>, or return null if there
        /// is no such block.
        /// </summary>
        /// <param name="addrStart"></param>
        /// <returns></returns>
        Block FindContainingBlock(Address addr);
        Block FindExactBlock(Address addr);
        Block SplitBlock(Block block, Address addr);

        Block CreateCallRetThunk(Address addrFrom, Procedure procOld, Procedure procNew);
        void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address);

        IEnumerable<RtlInstructionCluster> GetTrace(Address addrStart, ProcessorState state, IStorageBinder frame);
        void ScanImageSymbol(Program program, ImageSymbol sym, bool isEntryPoint);
    }
}
