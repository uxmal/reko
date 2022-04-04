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
    public class ProcedureWorker
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(ProcedureWorker), "ProcedureWorker tracing")
        {
            Level = TraceLevel.Warning,
        };

        private readonly RecursiveScanner scanner;
        private readonly Proc proc;
        private readonly WorkList<BlockWorker> workList;
        private readonly IStorageBinder binder;
        private Dictionary<Address, (Address, ProcedureWorker, ProcessorState)> callersWaitingForReturn;

        public ProcedureWorker(RecursiveScanner scanner, Proc proc, ProcessorState state)
        {
            this.scanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockWorker>();
            this.binder = new StorageBinder();
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
                    if (!scanner.TryRegisterBlockStart(work.Address, proc.Address))
                        continue;
                    trace_Verbose("    {0}: Parsing block at {1}", proc.Address, work.Address);
                    var (block,state) = work.ParseBlock();
                    if (block is not null)
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

        /// <summary>
        /// Add a job to this worker.
        /// </summary>
        /// <param name="addr">The address at which to start.</param>
        /// <param name="state">The state to use.</param>
        public void AddJob(Address addr, ProcessorState state)
        {
            var trace = scanner.MakeTrace(addr, state, binder).GetEnumerator();
            AddJob(addr, trace, state);
        }

        public void AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state)
        {
            this.workList.Add(new BlockWorker(scanner, binder, addr, trace, state));
        }

        private Block HandleBlockEnd(
            Block block, 
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            trace_Verbose("    {0}: Parsed block at {1}", proc.Name, block.Address);
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
                        AddJob(edge.To, trace, state);
                        scanner.RegisterEdge(edge);
                        trace_Verbose("    {0}: added edge {1}, {2}", proc.Address, edge.From, edge.To);
                        break;
                    case EdgeType.Jump:
                        // Start a new trace somewhere else.
                        AddJob(edge.To, state);
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
                            trace_Verbose("    {0}: suspended, waiting for {1} to return", proc.Address, calleeWorker.Procedure.Address);
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
            trace_Verbose("    {0}: Bad block at {1}", proc.Name,addrBadBlock);
            //$TODO: enqueue low-prio item for throw new NotImplementedException($"Bad block at {addrBadBlock}");
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
                    trace_Verbose("   {0}: resuming worker {1} at {2}", proc.Address, caller.Procedure.Address, addrFallThrough);
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
                    //$TODO: indirect jumps
                    result.Add(new Edge(block.Address, proc.Address, EdgeType.Return));
                }
                break;
            case RtlCall call:
                if (call.Target is Address addrTarget)
                {
                    result.Add(new Edge(block.Address, addrTarget, EdgeType.Call));
                }
                else
                {
                    //$TODO: indirect calls
                    result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough));
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
    }
}
