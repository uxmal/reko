#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly ConcurrentQueue<WaitingCaller> callersWaitingForReturn;

        public ProcedureWorker(
            RecursiveScanner scanner,
            Proc proc,
            ProcessorState state,
            InstrClass rejectMask,
            IDecompilerEventListener listener)
            : base(scanner, rejectMask, listener)
        {
            this.recScanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockWorker>();
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
                    if (!recScanner.TryRegisterBlockStart(work.Address, proc.Address))
                        continue;
                    log.Verbose("    {0}: Parsing block at {1}", proc.Address, work.Address);
                    var (block,state) = work.ParseBlock();
                    if (block is not null && block.IsValid)
                    {
                        HandleBlockEnd(block, work.Trace, state);
                    }
                    else
                    {
                        HandleBadBlock(work.Address);
                    }
                }
                //$TODO: Check for indirects
                //$TODO: Check for calls.
                log.Inform("    {0}: Finished", proc.Name);
                return;
            }
        }

        public override BlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            var worker = base.CreateBlockWorker(recScanner, this, addr, trace, state);
            this.workList.Add(worker);
            return worker;
        }

        public override BlockWorker AddFallthroughJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            return AddJob(addr, trace, state);
        }

        public override bool TryMarkVisited(Address addr) => true;

        protected override void ProcessCall(RtlBlock block, Edge edge, ProcessorState state)
        {
            switch (recScanner.GetProcedureReturnStatus(edge.To))
            {
            case ReturnStatus.Diverges:
                return;
            case ReturnStatus.Returns:
                if (!recScanner.IsExecutableAddress(block.FallThrough))
                    return;
                var fallThrough = new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough);
                recScanner.RegisterEdge(fallThrough);
                AddJob(fallThrough.To, state);
                log.Verbose("    {0}: added edge to {1}", fallThrough.From, fallThrough.To);
                return;
            }
            // The target procedure has not been processed yet, so try to suspend
            // this worker waiting for the target procedure to finish. Suspending
            // this worker will always succeed.
            
            recScanner.TrySuspendWorker(this);

            // Try starting a worker for the callee.
            if (recScanner.TryStartProcedureWorker(edge.To, state, out var calleeWorker))
            {
                var waitingCaller = new WaitingCaller(
                    this,
                    edge.From,
                    block.FallThrough,
                    state);

                calleeWorker.TryEnqueueCaller(waitingCaller);
                log.Verbose("    {0}: suspended, waiting for {1} to return", proc.Address, calleeWorker.Procedure.Address);
            }
            else
                throw new NotImplementedException("Couldn't start procedure worker");
        }

        protected override void ProcessReturn()
        {
            // By setting the procedure return status, other threads will
            // no longer enqueue callers in callersWaitingForReturn since
            // the return status of the procedure is known.
            recScanner.SetProcedureReturnStatus(proc.Address, ReturnStatus.Returns);
            while (callersWaitingForReturn.TryDequeue(out var wc))
            {
                log.Verbose("   {0}: resuming worker {1} at {2}", proc.Address, wc.Worker.Procedure.Address, wc.FallthroughAddress);
                recScanner.ResumeWorker(
                    wc.Worker, wc.CallAddress, wc.FallthroughAddress, wc.State);
            }
        }

        public override RtlBlock? SplitExistingBlock(Address addr)
        {
            // This overridden method is not expected to be called since
            // this.TryMarkVisited always returns true.
            throw new NotSupportedException();
        }

        public bool TryEnqueueCaller(WaitingCaller waitingCaller)
        {
            callersWaitingForReturn.Enqueue(waitingCaller);
            return true;
        }

        protected override bool TryRegisterTrampoline(
            Address addrFinalInstr, 
            List<RtlInstructionCluster> trampolineStub, 
            [MaybeNullWhen(false)] out Trampoline trampoline)
        {
            return this.recScanner.TryRegisterTrampoline(addrFinalInstr, trampolineStub, out trampoline);
        }
    }
}
