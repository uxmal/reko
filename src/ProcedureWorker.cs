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

        /// <summary>
        /// The procedure on which this <see cref="ProcedureWorker"/> is working.
        /// </summary>
        public Address ProcedureAddress => proc.Address;

        /// <summary>
        /// Executes work items in the scope of the current procedure.
        /// </summary>
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
                //$TODO: Check for indirects
                //$TODO: Check for calls.
                trace_Inform("    {0}: Finished", proc.Name);
                return;
            }
        }

        /// <summary>
        /// Add a job to this worker.
        /// </summary>
        /// <param name="addr">The address at which to start.</param>
        /// <param name="state">The state to use.</param>
        public void AddJob(Address addr, ProcessorState state)
        {
            var trace = scanner.MakeTrace(addr, state, binder).GetEnumerator();
            this.workList.Add(new BlockItem(addr, trace, state));
        }

        /// <summary>
        /// Performs a linear scan, stopping when a CTI is encountered,
        /// or we run off the edge of the world.
        /// </summary>
        /// <param name="work">Work item to use</param>
        /// <returns></returns>
        private (Block?, ProcessorState) ParseBlock(BlockItem work)
        {
            var instrs = new List<(Address, RtlInstruction)>();
            var trace = work.Trace;
            var state = work.State;
            while (work.Trace.MoveNext())
            {
                var cluster = trace.Current;
                foreach (var rtl in cluster.Instructions)
                {
                    trace_Verbose("      {0}: {1}", cluster.Address, rtl);
                    switch (rtl)
                    {
                    case RtlAssignment ass:
                        instrs.Add((cluster.Address, rtl));
                        //$TODO: emulate state;
                        continue;
                    case RtlBranch branch:
                    case RtlGoto g:
                    case RtlCall call:
                    case RtlReturn ret:
                        break;
                    default:
                        throw new NotImplementedException($"{rtl.GetType()} - not implemented");
                    }
                    Debug.Assert(rtl.Class.HasFlag(InstrClass.Transfer));
                    return ParseTransferInstruction(work, instrs, trace, state, cluster.Address, rtl);
                }
            }
            // Fell off the end.
            return (null, work.State);
        }

        private (Block?, ProcessorState) ParseTransferInstruction(
            BlockItem work,
            List<(Address, RtlInstruction)> instrs,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state,
            Address addr,
            RtlInstruction rtl)
        {
            var size = addr - work.Address + trace.Current.Length;
            if (rtl.Class.HasFlag(InstrClass.Delay))
            {
                if (!MaybeStealDelaySlot(rtl, instrs, trace))
                    return (null, state);
            }
            else
            {
                instrs.Add((addr, rtl));
            }
            var addrFallthrough = trace.Current.Address + trace.Current.Length; 
            var block = scanner.RegisterBlock(
                work.State.Architecture,
                work.Address,
                size,
                addrFallthrough,
                instrs);
            return (block, state);
        }

        private bool MaybeStealDelaySlot(
            RtlInstruction rtlTransfer,
            List<(Address,RtlInstruction)> instrs,
            IEnumerator<RtlInstructionCluster> trace)
        {
            Expression MkTmp(Address addr, Expression e)
            {
                var tmp = binder.CreateTemporary(e.DataType);
                instrs.Add((addr, new RtlAssignment(tmp, e)));
                return tmp;
            }

            Expression? tmp = null;
            var addrTransfer = trace.Current.Address;
            switch (rtlTransfer)
            {
            case RtlBranch branch:
                if (branch.Condition is not Constant)
                {
                    tmp = MkTmp(addrTransfer , branch.Condition);
                    rtlTransfer = new RtlBranch(tmp, (Address)branch.Target, InstrClass.ConditionalTransfer);
                }
                break;
            case RtlGoto g:
                if (g.Target is not Address)
                {
                    tmp = MkTmp(addrTransfer, g.Target);
                    rtlTransfer = new RtlGoto(tmp, InstrClass.Transfer);
                }
                break;
            case RtlReturn ret:
                break;
            default:
                throw new NotImplementedException($"{rtlTransfer.GetType().Name} - not implemented.");
            }
            if (!trace.MoveNext())
                return false;
            var rtlDelayed = trace.Current;
            if (rtlDelayed.Class.HasFlag(InstrClass.Transfer))
            {
                // Can't deal with transfer functions in delay slots yet.
                return false;
            }
            foreach (var instr in rtlDelayed.Instructions)
            {
                instrs.Add((rtlDelayed.Address, instr));
            }
            instrs.Add((addrTransfer, rtlTransfer));
            return true;
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
                result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Jump));
                result.Add(new Edge(block.Address, (Address)b.Target, EdgeType.Jump));
                break;
            case RtlGoto g:
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
