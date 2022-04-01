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
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ProcedureWorker), "ProcedureWorker tracing")
        {
            Level = TraceLevel.Verbose,
        };

        private readonly RecursiveScanner scanner;
        private readonly Proc proc;
        private readonly WorkList<BlockItem> workList;
        private readonly IStorageBinder binder;
        private Dictionary<Address, (Address, ProcedureWorker, ProcessorState)> callersWaitingForReturn;

        public ProcedureWorker(RecursiveScanner scanner, Proc proc, ProcessorState state)
        {
            this.scanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockItem>();
            this.binder = new StorageBinder();
            this.callersWaitingForReturn = new();
            AddJob(proc.Address, state);
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
                        HandleBlockEnd(block, work.Trace, state);
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

        public void AddJob(Address addr, ProcessorState state)
        {
            var trace = scanner.MakeTrace(addr, state, binder).GetEnumerator();
            this.workList.Add(new BlockItem(addr, trace, state));
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
                    trace_Verbose("      {0}: {1}", cluster.Address, rtl);
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
                    case RtlCall call:
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

        private Block HandleBlockEnd(
            Block block, 
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            var (lastAddr, lastInstr) = block.Instructions[^1];
            if (scanner.TryRegisterBlockEnd(block.Address, lastAddr))
            {
                var edges = ComputeEdges(lastInstr, lastAddr, block);
                foreach (var edge in edges)
                {
                    switch (edge.Type)
                    {
                    case EdgeType.Fallthrough:
                        // Reuse the same trace. This is necessary to handle the 
                        // ARM Thumb IT instruction, which puts state in the trace.
                        workList.Add(new BlockItem(edge.To, trace, state));
                        scanner.RegisterEdge(edge);
                        trace_Verbose("    {0}: added edge {1}, {2}", proc.Address, edge.From, edge.To);
                        break;
                    case EdgeType.Jump:
                        // Start a new trace somewhere else.
                        trace = scanner.MakeTrace(edge.To, state, binder).GetEnumerator();
                        workList.Add(new BlockItem(edge.To, trace, state));
                        scanner.RegisterEdge(edge);
                        trace_Verbose("    {0}: added edge {1}, {2}", proc.Address, edge.From, edge.To);
                        break;
                    case EdgeType.Call:
                        if (scanner.GetProcedureReturnStatus(edge.To) != ReturnStatus.Unknown)
                            break;
                        //$TODO: won't work with delay slots.
                        scanner.TrySuspendWorker(this);
                        if (scanner.TryStartProcedureWorker(edge.To, state, out var calleeWorker))
                        {
                            calleeWorker.TryEnqueueCaller(this, edge.From, block.Address + block.Length, state);
                            trace_Verbose("    {0}: suspended, waiting for {1} to return", proc.Address, calleeWorker.ProcedureAddress);
                        }
                        else
                            throw new NotImplementedException("Couldn't start procedure worker");
                        break;
                    case EdgeType.Return:
                        ProcessReturn();
                        break;
                    default:
                        throw new NotImplementedException($"{edge.Type} edge not handled yet.");
                    }
                }
            }
            else
            {
                trace_Verbose("    {0}: Splitting block at [{1}-{2}]", proc.Address, block.Address, lastAddr);
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
            scanner.SetProcedureReturnStatus(proc.Address, ReturnStatus.Returns);
            var empty = new Dictionary<Address, (Address, ProcedureWorker, ProcessorState)>();
            var callers = Interlocked.Exchange(ref callersWaitingForReturn, empty);
            while (callers.Count != 0)
            {
                foreach (var (addrFallThrough, (addrCaller, caller, state)) in callers)
                {
                    trace_Verbose("   {0}: resuming worker {1} at {2}", proc.Address, caller.ProcedureAddress, addrFallThrough);
                    scanner.ResumeWorker(caller, addrCaller, addrFallThrough, state);
                }
                empty = new Dictionary<Address, (Address, ProcedureWorker, ProcessorState)>();
                callers = Interlocked.Exchange(ref callersWaitingForReturn, empty);
            }
        }

        private List<Edge> ComputeEdges(RtlInstruction instr, Address addrInstr, Block block)
        {
            var result = new List<Edge>();
            switch (instr)
            {
            case RtlBranch b:
                var addrFallThrough = block.Address + block.Length;
                result.Add(new Edge(block.Address, addrFallThrough, EdgeType.Jump));
                result.Add(new Edge(block.Address, (Address)b.Target, EdgeType.Jump));
                break;
            case RtlGoto g:
                addrFallThrough = block.Address + block.Length;
                if (g.Target is Address addrGotoTarget)
                {
                    result.Add(new Edge(block.Address, addrGotoTarget, EdgeType.Jump));
                }
                else
                {
                    throw new NotImplementedException("//$TODO: indirect calls");
                }
                break;
            case RtlCall call:
                addrFallThrough = block.Address + block.Length;
                if (call.Target is Address addrTarget)
                {
                    result.Add(new Edge(block.Address, addrTarget, EdgeType.Call));
                }
                else
                {
                    throw new NotImplementedException("//$TODO: indirect calls");
                }
                break;
            case RtlReturn:
                result.Add(new Edge(block.Address, proc.Address, EdgeType.Return));
                break;
            default:
                throw new NotImplementedException();
            }
            return result;
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

        private record BlockItem(
            Address Address,
            IEnumerator<RtlInstructionCluster> Trace,
            ProcessorState State);
    }
}
