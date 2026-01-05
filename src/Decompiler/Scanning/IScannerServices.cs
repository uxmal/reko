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
using Reko.Core.Loading;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Interface containing methods used to scan a program image.
    /// </summary>
    public interface IScannerServices
    {
        /// <summary>
        /// <see cref="IServiceProvider"/> providing runtime services.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Starts a recursive scan of a procedure at the address <paramref name="addr"/>.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use when scanning
        /// the procedure.</param>
        /// <param name="addr">Address at which to start scanning.</param>
        /// <param name="procedureName">Optional name of the procedure.</param>
        /// <param name="state">Processor state at the start of the procedure.
        /// </param>
        /// <returns></returns>
        ProcedureBase ScanProcedure(
            IProcessorArchitecture arch,
            Address addr,
            string? procedureName,
            ProcessorState state);


        /// <summary>
        /// Enqueues an <see cref="ImageSymbol"/> for processing.
        /// </summary>
        /// <param name="sym">The <see cref="ImageSymbol"/> to process.
        /// </param>
        /// <param name="isEntryPoint">True if the <see cref="ImageSymbol"/> is also
        /// an entry point to the executable; false otherwise.
        /// </param>
        void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint);

        /// <summary>
        /// Enqueues a procedure for scanning.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use when scanning
        /// the procedure.</param>
        /// <param name="addr">Address at which to start scanning.
        /// </param>
        void EnqueueProcedure(IProcessorArchitecture arch, Address addr);

        /// <summary>
        /// Enqueues a user-defined procedure for scanning.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use when scanning
        /// the procedure.</param>
        /// <param name="addr">Address at which to start scanning.
        /// </param>
        /// <param name="sig">User-provided function signature.</param>
        /// <param name="name">User-provided name.</param>
        void EnqueueUserProcedure(IProcessorArchitecture arch, Address addr, FunctionType sig, string? name);

        /// <summary>
        /// Enqueues a user-defined global data object for scanning.
        /// </summary>
        /// <param name="addr">Address of the data object.</param>
        /// <param name="dt"><see cref="DataType"/> of the data object.</param>
        /// <param name="name">Optional name of the data object.</param>
        void EnqueueUserGlobalData(Address addr, DataType dt, string? name);

        /// <summary>
        /// Ensures that <paramref name="block"/> ends at the given address <paramref name="addrEnd"/>.
        /// </summary>
        /// <param name="block"><see cref="Block"/> to terminate.</param>
        /// <param name="addrEnd">Address at which <paramref name="block"/> should end.
        /// </param>
        void TerminateBlock(Block block, Address addrEnd);

        /// <summary>
        /// Enqueues a jump target for processing.
        /// </summary>
        /// <param name="addrSrc">The address from which the jump originated.</param>
        /// <param name="addrDst">The address where the jump goes to.</param>
        /// <param name="proc">Procedure in which the jump occurred.</param>
        /// <param name="state">Current <see cref="ProcessorState"/>.
        /// </param>
        /// <returns>A new basic block if the block hadn't been processed before;
        /// otherwise false.
        /// </returns>
        Block? EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state);

        /// <summary>
        /// Enqueues a user-defined procedure for scanning.
        /// </summary>
        /// <param name="arch">Processor architecture to use when scanning.</param>
        /// <param name="sp">User-provided details for use when scanning.</param>
        /// <returns>The address of the procedure if it hadn't been scanned already; otherwise
        /// null.
        /// </returns>
        Address? EnqueueUserProcedure(IProcessorArchitecture arch, UserProcedure sp);


        /// <summary>
        /// Find the block that contains the address <paramref name="addr"/>, or return null if there
        /// is no such block.
        /// </summary>
        /// <param name="addr">Address to use when probing.</param>
        /// <returns><see cref="Block"/> that contains the given address, or null
        /// if no such block exists.
        /// </returns>
        Block? FindContainingBlock(Address addr);

        /// <summary>
        /// Finds the block that starts exactly at the given address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to probe.</param>
        /// <returns>The block that starts at <paramref name="addr"/> if it exists; otherwise null.</returns>
        Block? FindExactBlock(Address addr);

        /// <summary>
        /// Splits the given <paramref name="block"/> at the specified address <paramref name="addr"/>.
        /// </summary>
        /// <param name="block">Basic block to split.</param>
        /// <param name="addr">Address at which to split it.
        /// </param>
        /// <returns>The newly created block, starting att address <paramref name="addr"/>.</returns>
        Block SplitBlock(Block block, Address addr);

        /// <summary>
        /// Creates a call/return thunk used when jumping (using goto) from one procedure
        /// to another.
        /// </summary>
        /// <param name="addrFrom">Address from which the jump is made.</param>
        /// <param name="procOld">Procedure containing the jump.</param>
        /// <param name="state">Processor state at the time of the jump.</param>
        /// <param name="procNew">The procedure being jumped to.</param>
        /// <returns>A basic block containing a <see cref="Core.Code.CallInstruction"/>
        /// followed by a <see cref="Core.Code.ReturnInstruction"/>.</returns>

        Block CreateCallRetThunk(Address addrFrom, Procedure procOld, ProcessorState state, Procedure procNew);

        /// <summary>
        /// Sets the number of address locations that the called procedure <paramref name="proc"/>
        /// adjusts the stack pointer with after being called.
        /// </summary>
        /// <param name="proc">Procedure being called.</param>
        /// <param name="stackDelta">Amount by which the stack pointer is adjusted after
        /// reeturning from the call to <paramref name="proc" />.</param>
        /// <param name="address">Address of the <see cref="Core.Code.ReturnInstruction"/>
        /// that set this value.</param>
        void SetProcedureStackDelta(
            Procedure proc, int stackDelta, Address address);


        /// <summary>
        /// Sets the number of address locations required to store the return address on the 
        /// stack when calling the procedure <paramref name="proc"/>.
        /// </summary>
        /// <param name="proc">Procedure being called.</param>
        /// <param name="returnAddressBytes">Number of address locations needed to store the return
        /// address on the stack. RISC architectures will typically specify 0.
        /// </param>
        /// <param name="address">Address of the return instruction recording this fact.
        /// </param>
        void SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address);

        /// <summary>
        /// Creates a <b>trace</b> of RTL instructions starting at the given address <paramref name="addrStart"/>.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use when lifting the instructions
        /// of the instruction trace.</param>
        /// <param name="addrStart">Address at which to start the instruction trace.</param>
        /// <param name="state"><see cref="ProcessorState"/> used when lifting the address
        /// trace.</param>
        /// <param name="binder"><see cref="IStorageBinder"/> instance that binds <see cref="Storage"/>s to
        /// <see cref="Core.Expressions.Identifier"/>s.
        /// </param>
        /// <returns></returns>
        IEnumerable<RtlInstructionCluster> GetTrace(IProcessorArchitecture arch, Address addrStart, ProcessorState state, IStorageBinder binder);

        /// <summary>
        /// Given the address of a indirect call instruction and the address of an import thunk,
        /// returns the external procedure being called.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use.</param>
        /// <param name="addrImportThunk">Address of the short instruction thunk that calls the external,
        /// dynamically linked procedure.</param>
        /// <param name="addrInstruction">The address of the call instruction.</param>
        /// <returns>A dynamically linked <see cref="ExternalProcedure"/> if one is found;
        /// otherwise null.</returns>
        ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrImportThunk, Address addrInstruction);


        /// <summary>
        /// Tries to determine if the instruction at <paramref name="addr"/> is 
        /// a trampoline instruction. If so, we return a call to the imported 
        /// function directly.
        /// procedure.
        /// </summary>
        /// <remarks>
        /// A trampoline is a procedure whose only contents is an indirect
        /// JUMP to a location that contains the address of an imported
        /// function. Because these trampolines may take on different
        /// appearances depending on the processor architecture, we have to 
        /// call out to the architecture to assist in matching them.
        /// </remarks>
        /// <param name="arch">The processor architecture to use.</param>
        /// <param name="addr"></param>
        /// <returns>Null if there was no trampoline.</returns>
        ProcedureBase? GetTrampoline(IProcessorArchitecture arch, Address addr);


        //$REVIEW: these methods don't belong here. 
        // Callers should use IDecompilerEventListener or IRewriterHost.

        /// <summary>Soon to be removed.</summary>
        void Warn(Address addr, string message);
        /// <summary>Soon to be removed.</summary>
        void Warn(Address addr, string message, params object[] args);
        /// <summary>Soon to be removed.</summary>
        void Error(Address addr, string message, params object[] args);
    }
}