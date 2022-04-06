using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Threading;
using RtlBlock = Reko.Scanning.RtlBlock;

namespace Reko.ScannerV2
{
    public class ProcedureWorker : AbstractProcedureWorker
    {
        private readonly RecursiveScanner recScanner;
        private readonly Proc proc;
        private readonly WorkList<BlockWorker> workList;
        private Dictionary<Address, (Address, ProcedureWorker, ProcessorState)> callersWaitingForReturn;

        public ProcedureWorker(RecursiveScanner scanner, Proc proc, ProcessorState state)
            : base(scanner)
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
