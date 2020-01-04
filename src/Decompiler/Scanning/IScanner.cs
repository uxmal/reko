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

        /// <summary>
        /// Scans the image, locating blobs of data and procedures.
        /// After completion, the program.ImageMap is populated with
        /// chunks of data, and the program.Procedures dictionary contains
        /// all procedures to decompile.
        /// </summary>
        void ScanImage();

        /// <summary>
        /// Performs a scan of the blocks that constitute a procedure, optionally named 
        /// <paramref name="procedureName"/>
        /// </summary>
        /// <param name="arch">Processor architecture the procedure is written in.</param>
        /// <param name="addr">Address of the code from which we will start scanning.</param>
        /// <param name="procedureName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        ProcedureBase ScanProcedure(IProcessorArchitecture arch, Address addr, string procedureName, ProcessorState state);

/// <summary>
/// Performs a scan starting at the address of the given image symbol
/// </summary>
/// <param name="sym"></param>
/// <param name="isEntryPoint"></param>
        void ScanImageSymbol(ImageSymbol sym, bool isEntryPoint);

        Block EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state);
        Address EnqueueUserProcedure(IProcessorArchitecture arch, Procedure_v1 sp);

        ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrImportThunk, Address addrInstruction);

        ProcedureBase GetTrampoline(IProcessorArchitecture arch, Address addr);

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

        IEnumerable<RtlInstructionCluster> GetTrace(IProcessorArchitecture arch, Address addrStart, ProcessorState state, IStorageBinder binder);
    }
}
