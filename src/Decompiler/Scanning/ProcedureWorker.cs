#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// This scanner worker will discover <see cref="RtlBlock"/>s by tracing
    /// their execution flow. The tracing stops when <see cref="RtlReturn"/>
    /// instructions are encountered, or function calls to provably diverging
    /// procedures (like exit(), longjmp(), or ExitProcess()).
    /// </summary>
    public class ProcedureWorker : AbstractProcedureWorker
    {
        private readonly RecursiveScanner recScanner;
        private readonly Proc proc;
        private readonly WorkList<BlockWorker> workList;
        private readonly ConcurrentDictionary<Address, Address> suspendedCalls;
        private readonly ConcurrentQueue<WaitingCaller> callersWaitingForReturn;

        /// <summary>
        /// Constructs an instance of <see cref="ProcedureWorker"/>.
        /// </summary>
        /// <param name="scanner">Recursive scanner orchestrating this procedure
        /// worker.</param>
        /// <param name="proc"></param>
        /// <param name="state"></param>
        /// <param name="rejectMask"></param>
        /// <param name="listener"></param>
        public ProcedureWorker(
            RecursiveScanner scanner,
            Proc proc,
            ProcessorState state,
            InstrClass rejectMask,
            IEventListener listener)
            : base(scanner, proc.Address, rejectMask, listener)
        {
            this.recScanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockWorker>();
            this.suspendedCalls = new();
            this.callersWaitingForReturn = new();
            AddJob(proc.Address, state);
        }

        /// <summary>
        /// The procedure on which this <see cref="ProcedureWorker"/> is working.
        /// </summary>
        public Proc Procedure => proc;

        /// <summary>
        /// Executes work items in the scope of the current procedure.
        /// </summary>
        public void Run()
        {
            log.Inform("PW: {0} Processing", proc.Name);
            for (;;)
            {
                while (workList.TryGetWorkItem(out var work))
                {
                    bool newBlockCreated = recScanner.TryRegisterBlockStart(work.Address, proc.Address);
                    if (work.CallerBlockAddress is not null)
                    {
                        ProcessFallthroughAfterCall(work, work.CallerBlockAddress.Value, newBlockCreated);
                    }

                    // Nothing to do if the block aready existed.
                    if (!newBlockCreated)
                    {
                        continue;
                    }

                    log.Verbose("    {0}: Parsing block at {1}", this.Address, work.Address);
                    var (block, subinstrTargets, state) = work.ParseBlock();
                    if (block is not null && block.IsValid)
                    {
                        HandleBlockEnd(block, work.Trace, subinstrTargets, state);
                    }
                    else
                    {
                        HandleBadBlock(work.Address);
                    }
                }

                static string AbbreviatedList(ICollection<Address> addresses)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("{0} [", addresses.Count);
                    sb.Append(string.Join(",", addresses.Take(4)));
                    if (addresses.Count > 4)
                        sb.Append("...");
                    sb.Append("]");
                    return sb.ToString();
                }

                //$TODO: Check for indirects
                //$TODO: Check for calls.
                log.Inform("    {0}: No more work items; waiting for {1} callees; {2} suspended callers",
                    proc.Name,
                    AbbreviatedList(this.suspendedCalls.Keys),
                    AbbreviatedList(this.callersWaitingForReturn.Select(w => w.CallAddress).ToList()));
                //$TODO: could this be a race condition? someone may have snuck a work item onto the queue.
                recScanner.OnWorkerCompleted(this, suspendedCalls.Count);
                return;
            }
        }

        private void ProcessFallthroughAfterCall(BlockWorker work, Address addrCallBlock, bool newBlockCreated)
        {
            var edge = new Edge(addrCallBlock, work.Address, EdgeType.Fallthrough);
            RegisterEdge(edge);
            if (newBlockCreated)
                return;
            // We fell through a call, and landed on an already 
            // existing basic block. We guess that this is a
            // tail call to another procedure, and further assume that
            // the other procedure returns. Later passes of the 
            // scanner (notably the ProcedureBuilder) can improve
            // this guess. We will now behave as we had seen
            // a "return" immediately after the call.
            this.ProcessReturn();
        }

        /// <inheritdoc />
        public override BlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            var worker = base.CreateBlockWorker(recScanner, this, addr, trace, state);
            this.workList.Add(worker);
            return worker;
        }

        /// <inheritdoc />
        public override BlockWorker AddFallthroughJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            return AddJob(addr, trace, state);
        }

        /// <inheritdoc />
        public override bool TryMarkVisited(Address addr) => true;

        /// <summary>
        /// Called when a call has been proven to return.
        /// </summary>
        /// <param name="addrCall"></param>
        public void UnsuspendCall(Address addrCall)
        {
            var removed = this.suspendedCalls.TryRemove(addrCall, out var addrDest);
            log.Verbose("    {0}: Removing suspended call at {1} to {2} {3}",
                this.Address,
                addrCall,
                addrDest.ToString() ?? "<null>",
                removed ? "succeeded" : "failed");
        }

        /// <inheritdoc/>
        protected override void ProcessCall(RtlBlock block, Edge edge, ProcessorState state)
        {
            switch (recScanner.GetProcedureReturnStatus(edge.To))
            {
            case ReturnStatus.Diverges:
                return;
            case ReturnStatus.Returns:
                // If the fallthrough address is not executable, it means the 
                // called procedure (if valid) never returns.
                if (!recScanner.IsExecutableAddress(block.FallThrough))
                    return;
                var fallThrough = new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough);
                recScanner.RegisterEdge(fallThrough);
                AddJob(fallThrough.To, state);
                log.Verbose("    {0}: added edge from {1} to {2}", this.Address, fallThrough.From, fallThrough.To);
                return;
            }

            // We can't tell if the target procedure will terminate or not. We add
            // an unresolved call, but continue working.
            this.suspendedCalls.TryAdd(edge.From, edge.To);

            // Try starting a worker for the callee.
            if (recScanner.TryStartProcedureWorker(edge.To, state, out var calleeWorker))
            {
                var waitingCaller = new WaitingCaller(
                    this,
                    edge.From,
                    block.FallThrough,
                    state);

                calleeWorker.TryEnqueueCaller(waitingCaller);
                log.Verbose("    {0}: suspended block {1}, waiting for {2} to return",
                    this.Address, block.Address, calleeWorker.Procedure.Address);
            }
            else
                throw new NotImplementedException("Couldn't start procedure worker.");
        }

        /// <inheritdoc />
        protected override void ProcessReturn()
        {
            // By setting the procedure return status, other workers will
            // no longer enqueue callers in callersWaitingForReturn since
            // the return status of the procedure is known.
            recScanner.SetProcedureReturnStatus(proc.Address, ReturnStatus.Returns);
            log.Verbose("    {0}: resolved as returning", proc.Address);
            while (callersWaitingForReturn.TryDequeue(out var wc))
            {
                log.Verbose("        {0}: resuming worker {1} at {2}", proc.Address, wc.Worker.Procedure.Address, wc.FallthroughAddress);
                recScanner.ResumeWorker(
                    wc.Worker, wc.CallAddress, wc.FallthroughAddress, wc.State);
            }
        }

        /// <inheritdoc />
        public override RtlBlock? SplitExistingBlock(Address addr)
        {
            // This overridden method is not expected to be called since
            // this.TryMarkVisited always returns true.
            throw new NotSupportedException();
        }

        
        private bool TryEnqueueCaller(WaitingCaller waitingCaller)
        {
            this.callersWaitingForReturn.Enqueue(waitingCaller);
            return true;
        }

        /// <inheritdoc/>
        protected override bool TryRegisterTrampoline(
            Address addrFinalInstr, 
            List<RtlInstructionCluster> trampolineStub, 
            [MaybeNullWhen(false)] out Trampoline trampoline)
        {
            return this.recScanner.TryRegisterTrampoline(addrFinalInstr, trampolineStub, out trampoline);
        }
    }
}
