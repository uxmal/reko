#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Scanning
{
    public interface IScannerServices
    {
        IServiceProvider Services { get; }

        ProcedureBase ScanProcedure(IProcessorArchitecture arch, Address addr, string? procedureName, ProcessorState state);


        void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint);
        void EnqueueProcedure(IProcessorArchitecture arch, Address addr);
        void EnqueueUserProcedure(IProcessorArchitecture arch, Address addr, FunctionType sig, string? name);
        void EnqueueUserGlobalData(Address addr, DataType dt, string? name);

        void TerminateBlock(Block block, Address addrEnd);

        Block? EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state);
        Address? EnqueueUserProcedure(IProcessorArchitecture arch, UserProcedure sp);


        /// <summary>
        /// Find the block that contains the address <paramref name="addr"/>, or return null if there
        /// is no such block.
        /// </summary>
        /// <param name="addrStart"></param>
        /// <returns></returns>
        Block? FindContainingBlock(Address addr);
        Block? FindExactBlock(Address addr);
        Block SplitBlock(Block block, Address addr);

        Block CreateCallRetThunk(Address addrFrom, Procedure procOld, ProcessorState state, Procedure procNew);
        void SetProcedureStackDelta(
            Procedure proc, int stackDelta, Address address);
        void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address);

        IEnumerable<RtlInstructionCluster> GetTrace(IProcessorArchitecture arch, Address addrStart, ProcessorState state, IStorageBinder binder);

        ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrImportThunk, Address addrInstruction);

        ProcedureBase? GetTrampoline(IProcessorArchitecture arch, Address addr);


        //$REVIEW: these methods don't belong here. 
        // Callers should use IDecompilerEventListener or IRewriterHost.
        void Warn(Address addr, string message);
        void Warn(Address addr, string message, params object[] args);
        void Error(Address addr, string message, params object[] args);
    }
}