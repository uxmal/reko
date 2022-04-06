using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public abstract class AbstractProcedureWorker
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(AbstractProcedureWorker), "AbstractProcedureWorker tracing")
        {
            Level = TraceLevel.Warning,
        };

        private AbstractScanner scanner;
        protected readonly IStorageBinder binder;

        protected AbstractProcedureWorker(AbstractScanner scanner)
        {
            this.scanner = scanner;
            this.binder = new StorageBinder();
        }

        public Block HandleBlockEnd(
            Block block,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            trace_Verbose("    {0}: Finished block", block.Address);
            var lastCluster = block.Instructions[^1];
            var lastInstr = lastCluster.Instructions[^1];
            var lastAddr = lastCluster.Address;
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
                        scanner.RegisterEdge(edge);
                        AddJob(edge.To, trace, state);
                        trace_Verbose("    {0}: added edge to {1}", edge.From, edge.To);
                        break;
                    case EdgeType.Jump:
                        // Start a new trace somewhere else.
                        scanner.RegisterEdge(edge);
                        AddJob(edge.To, state);
                        trace_Verbose("    {0}: added edge to {1}", edge.From, edge.To);
                        break;
                    case EdgeType.Call:
                        ProcessCall(block, edge, state);
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
                trace_Verbose("    {0}: Splitting block at [{1}-{2}]", block.Address, block.Address, lastAddr);
                scanner.Splitblock(block, lastAddr);
            }
            return block;
        }

        protected void HandleBadBlock(Address addrBadBlock)
        {
            trace_Verbose("    {0}: Bad block", addrBadBlock);
            //$TODO: enqueue low-prio item for throw new NotImplementedException($"Bad block at {addrBadBlock}");
        }

        private List<Edge> ComputeEdges(RtlInstruction instr, Address addrInstr, Block block)
        {
            var result = new List<Edge>();
            switch (instr)
            {
            case RtlBranch b:
                result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough));
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
                    result.Add(new Edge(block.Address, block.Address, EdgeType.Return));
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
                result.Add(new Edge(block.Address, block.Address, EdgeType.Return));
                break;
            case RtlAssignment:
            case RtlSideEffect:
            case RtlNop:
                result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough));
                break;
            default:
                throw new NotImplementedException();
            }
            return result;
        }

        public RtlAssignment MkTmp(Expression e)
        {
            var tmp = binder.CreateTemporary(e.DataType);
            return new RtlAssignment(tmp, e);
        }

        /// <summary>
        /// Add a job to this worker.
        /// </summary>
        /// <param name="addr">The address at which to start.</param>
        /// <param name="state">The state to use.</param>
        public AbstractBlockWorker AddJob(Address addr, ProcessorState state)
        {
            var trace = scanner.MakeTrace(addr, state, binder).GetEnumerator();
            return AddJob(addr, trace, state);
        }

        public abstract AbstractBlockWorker AddJob(Address addr, IEnumerator<RtlInstructionCluster> trace, ProcessorState state);

        protected abstract void ProcessCall(Block block, Edge edge, ProcessorState state);

        protected abstract void ProcessReturn();

        [Conditional("DEBUG")]
        protected void trace_Inform(string format, params object[] args)
        {
            if (trace.TraceInfo)
                Debug.Print(format, args);
        }

        [Conditional("DEBUG")]
        protected void trace_Verbose(string format, params object[] args)
        {
            if (trace.TraceVerbose)
                Debug.Print(format, args);
        }
    }
}
