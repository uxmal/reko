#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public class ProcedureWorker : AbstractProcedureWorker
    {
        private readonly RecursiveScanner recScanner;
        private readonly Proc proc;
        private readonly WorkList<BlockWorker> workList;
        private Dictionary<Address, (Address, ProcedureWorker, ProcessorState)> callersWaitingForReturn;

        public ProcedureWorker(
            RecursiveScanner scanner,
            Proc proc,
            ProcessorState state,
            DecompilerEventListener listener)
            : base(scanner, listener)
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
            trace_Inform("PW: {0} Processing", proc.Name);
            for (;;)
            {
                while (workList.TryGetWorkItem(out var work))
                {
                    if (!recScanner.TryRegisterBlockStart(work.Address, proc.Address))
                        continue;
                    trace_Verbose("    {0}: Parsing block at {1}", proc.Address, work.Address);
                    var (block,state) = work.ParseBlock();
                    if (block.IsValid)
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
                trace_Inform("    {0}: Finished", proc.Name);
                return;
            }
        }

        public override AbstractBlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            var worker = new BlockWorker(recScanner, this, addr, trace, state);
            this.workList.Add(worker);
            return worker;
        }

        public override bool MarkVisited(Address addr) => true;

        protected override void ProcessCall(RtlBlock block, Edge edge, ProcessorState state)
        {
            if (recScanner.GetProcedureReturnStatus(edge.To) != ReturnStatus.Unknown)
                return;
            //$TODO: won't work with delay slots.
            recScanner.TrySuspendWorker(this);
            if (recScanner.TryStartProcedureWorker(edge.To, state, out var calleeWorker))
            {
                calleeWorker.TryEnqueueCaller(this, edge.From, block.Address + block.Length, state);
                trace_Verbose("    {0}: suspended, waiting for {1} to return", proc.Address, calleeWorker.Procedure.Address);
            }
            else
                throw new NotImplementedException("Couldn't start procedure worker");
        }

        protected override void ProcessReturn()
        {
            recScanner.SetProcedureReturnStatus(proc.Address, ReturnStatus.Returns);
            var empty = new Dictionary<Address, (Address, ProcedureWorker, ProcessorState)>();
            var callers = Interlocked.Exchange(ref callersWaitingForReturn, empty);
            while (callers.Count != 0)
            {
                foreach (var (addrFallThrough, (addrCaller, caller, state)) in callers)
                {
                    trace_Verbose("   {0}: resuming worker {1} at {2}", proc.Address, caller.Procedure.Address, addrFallThrough);
                    recScanner.ResumeWorker(caller, addrCaller, addrFallThrough, state);
                }
                empty = new Dictionary<Address, (Address, ProcedureWorker, ProcessorState)>();
                callers = Interlocked.Exchange(ref callersWaitingForReturn, empty);
            }
        }

        public bool TryEnqueueCaller(
            ProcedureWorker procedureWorker,
            Address addrCall,
            Address addrFallthrough,
            ProcessorState state)
        {
            lock (callersWaitingForReturn)
            {
                callersWaitingForReturn.Add(addrFallthrough, (addrCall, procedureWorker, state));
            }
            return true;
        }
    }
}
