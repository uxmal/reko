using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ProcedureWorker : Worker
    {
        private static TraceSwitch trace = new TraceSwitch(nameof(ProcedureWorker), "ProcedureWorker tracing")
        {
            Level = TraceLevel.Verbose,
        };

        private readonly RecursiveScanner scanner;
        private readonly Proc proc;
        private readonly WorkList<BlockItem> workList;
        private readonly IStorageBinder binder;
        private Dictionary<Address, (Address, ProcedureWorker)> callsWaitingForReturn;

        public ProcedureWorker(RecursiveScanner scanner, Proc proc, ProcessorState state)
        {
            this.scanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockItem>();
            this.binder = new StorageBinder();
            this.callsWaitingForReturn = new Dictionary<Address, (Address, ProcedureWorker)>();
            var trace = scanner.MakeTrace(proc.Address, state, binder).GetEnumerator();
            this.workList.Add(new BlockItem(proc.Address, trace, state));
        }

        public Address ProcedureAddress => proc.Address;

        public override void Run()
        {
            trace_Inform("PW: {0} Processing", proc.Name);
            for (;;)
            {
                while (workList.TryGetWorkItem(out var work))
                {
                    if (!scanner.TryRegisterBlockStart(work.Address, proc.Address))
                        continue;
                    trace_Verbose("    {0}: Parsing block at {1}", proc.Name, work.Address);
                    var (block,state) = ParseBlock(work);
                    if (block is not null)
                    {
                        trace_Verbose("    {0}: Parsed block at {1}", proc.Name, work.Address);
                        TerminateBlock(block, state);
                    }
                    else
                    {
                        trace_Verbose("    {0}: Bad block at {1}", proc.Name, work.Address);
                        HandleBadBlock(work.Address);
                    }
                }
                //$Check for indirects
                //$Check for calls.
                trace_Inform("    {0}: Finished", proc.Name);
                return;
            }
        }

        private (Block?, ProcessorState) ParseBlock(BlockItem work)
        {
            var instrs = new List<(Address, RtlInstruction)>();
            var trace = work.Trace;
            var state = work.State;
            while (work.Trace.MoveNext())
            {
                var cluster = work.Trace.Current;
                foreach (var rtl in cluster.Instructions)
                {
                    trace_Verbose("    {0}: {1}", cluster.Address, rtl);
                    instrs.Add((cluster.Address, rtl));
                    switch (rtl)
                    {
                    case RtlAssignment ass:
                        //$TODO: emulate state;
                        break;
                    case RtlBranch branch:
                        return (
                            scanner.RegisterBlock(
                                work.State.Architecture,
                                work.Address,
                                cluster.Address - work.Address + cluster.Length,
                                instrs),
                            state);
                    case RtlGoto g:
                        throw new NotImplementedException();
                    case RtlReturn ret:
                        return (
                            scanner.RegisterBlock(
                                work.State.Architecture,
                                work.Address,
                                cluster.Address - work.Address + cluster.Length,
                                instrs),
                            state);
                    }
                }
            }
            // Fell off the end.
            return (null, work.State);
        }

        private Block TerminateBlock(Block block, ProcessorState state)
        {
            var (lastAddr, lastInstr) = block.Instructions[^1];
            if (scanner.TryRegisterBlockEnd(block.Address, lastAddr))
            {
                var edges = ComputeEdges(lastInstr, lastAddr, block);
                foreach (var edge in edges)
                {
                    switch (edge.Type)
                    {
                    case EdgeType.DirectJump:
                        //$TODO mutate state depending on outcome
                        var trace = scanner.MakeTrace(edge.To, state, binder).GetEnumerator();
                        workList.Add(new BlockItem(edge.To, trace, state));
                        scanner.RegisterEdge(edge);
                        break;
                    default:
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                scanner.Splitblock(block, lastAddr);
            }
            return block;
        }

        private void HandleBadBlock(Address addrBadBlock)
        {
            throw new NotImplementedException();
        }

        private void ProcessReturn()
        {
            var empty = new Dictionary<Address, (Address, ProcedureWorker)>();
            Interlocked.Exchange(ref callsWaitingForReturn, empty);
            foreach (var (addrFallThrough, (addrCaller, caller)) in callsWaitingForReturn)
            {
                scanner.ResumeWorker(caller, addrCaller, addrFallThrough);
            }
        }

        private List<Edge> ComputeEdges(RtlInstruction instr, Address addrInstr, Block block)
        {
            switch (instr)
            {
            case RtlBranch b:
                var addrFallThrough = block.Address + block.Length;
                return new List<Edge>
                {
                    new Edge(addrInstr, addrFallThrough, EdgeType.DirectJump),
                    new Edge(addrInstr, (Address) b.Target, EdgeType.DirectJump),
                };
            case RtlReturn:
                return new List<Edge> { };
            }
            throw new NotImplementedException();
        }

        public bool TryEnqueueCaller(ProcedureWorker procedureWorker, Address addrCall, Address addrFallthrough)
        {
            lock (callsWaitingForReturn)
            {
                callsWaitingForReturn.Add(addrFallthrough, (addrCall, procedureWorker));
            }
            return true;
        }

        [Conditional("DEBUG")]
        private void trace_Inform(string format, params object[] args)
        {
            if (trace.TraceInfo)
                Debug.Print(format, args);
        }

        [Conditional("DEBUG")]
        private void trace_Verbose(string format, params object[] args)
        {
            if (trace.TraceVerbose)
                Debug.Print(format, args);
        }

        public bool VisitAssignment(RtlAssignment ass)
        {
            throw new NotImplementedException();
        }

        public bool VisitBranch(RtlBranch branch)
        {
            throw new NotImplementedException();
        }

        public bool VisitCall(RtlCall call)
        {
            throw new NotImplementedException();
        }

        public bool VisitGoto(RtlGoto go)
        {
            throw new NotImplementedException();
        }

        public bool VisitIf(RtlIf rtlIf)
        {
            throw new NotImplementedException();
        }

        public bool VisitInvalid(RtlInvalid invalid)
        {
            throw new NotImplementedException();
        }

        public bool VisitMicroGoto(RtlMicroGoto uGoto)
        {
            throw new NotImplementedException();
        }

        public bool VisitMicroLabel(RtlMicroLabel uLabel)
        {
            throw new NotImplementedException();
        }

        public bool VisitNop(RtlNop rtlNop)
        {
            throw new NotImplementedException();
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            throw new NotImplementedException();
        }

        private record BlockItem(
            Address Address,
            IEnumerator<RtlInstructionCluster> Trace,
            ProcessorState State);
    }
}
