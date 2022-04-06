using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtlBlock = Reko.Scanning.RtlBlock;
using BackwardSlicer = Reko.Scanning.BackwardSlicer;

namespace Reko.ScannerV2
{
    public abstract class AbstractProcedureWorker
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(AbstractProcedureWorker), "AbstractProcedureWorker tracing")
        {
            Level = TraceLevel.Warning,
        };

        private readonly AbstractScanner scanner;
        private readonly DecompilerEventListener listener;
        private readonly IStorageBinder binder;
        private readonly Dictionary<Address, List<Address>> miniCfg;

        protected AbstractProcedureWorker(AbstractScanner scanner, DecompilerEventListener listener)
        {
            this.scanner = scanner;
            this.listener = listener;
            this.binder = new StorageBinder();
            this.miniCfg = new Dictionary<Address, List<Address>>();
        }

        public RtlBlock HandleBlockEnd(
            RtlBlock block,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            trace_Verbose("    {0}: Finished block", block.Address);
            var lastCluster = block.Instructions[^1];
            var lastInstr = lastCluster.Instructions[^1];
            var lastAddr = lastCluster.Address;
            if (scanner.TryRegisterBlockEnd(block.Address, lastAddr))
            {
                var edges = ComputeEdges(lastInstr, lastAddr, block, state);
                foreach (var edge in edges)
                {
                    switch (edge.Type)
                    {
                    case EdgeType.Fallthrough:
                        // Reuse the same trace. This is necessary to handle the 
                        // ARM Thumb IT instruction, which puts state in the trace.
                        RegisterEdge(edge);
                        AddJob(edge.To, trace, state);
                        trace_Verbose("    {0}: added edge to {1}", edge.From, edge.To);
                        break;
                    case EdgeType.Jump:
                        // Start a new trace somewhere else.
                        RegisterEdge(edge);
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
                scanner.SplitBlockEndingAt(block, lastAddr);
            }
            return block;
        }

        protected void HandleBadBlock(Address addrBadBlock)
        {
            trace_Verbose("    {0}: Bad block", addrBadBlock);
            //$TODO: enqueue low-prio item for throw new NotImplementedException($"Bad block at {addrBadBlock}");
        }

        private List<Edge> ComputeEdges(
            RtlInstruction instr,
            Address addrInstr,
            RtlBlock block,
            ProcessorState state)
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
                else if (DiscoverTableExtent(
                        block.Architecture,
                        block,
                        state,
                        addrInstr,
                        g,
                        out var vector,
                        out _,
                        out var switchExp))
                {
                    var lastCluster = block.Instructions[^1];
                    Debug.Assert(lastCluster.Instructions[^1] == g);
                    var sw = new RtlSwitch(switchExp, vector.ToArray());
                    lastCluster.Instructions[^1] = sw;
                    result.AddRange(vector.Select(a => new Edge(block.Address, a, EdgeType.Jump)));
                }
                else
                {
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

        /// <summary>
        /// Discovers the extent of a jump/call table by walking backwards from the 
        /// jump/call until some gating condition (index < value, index & bitmask etc)
        /// can be found.
        /// </summary>
        /// <param name="addrSwitch">Address of the indirect transfer instruction</param>
        /// <param name="xfer">Expression that computes the transfer destination.
        /// It is never a constant value</param>
        /// <param name="vector">If successful, returns the list of addresses
        /// jumped/called to</param>
        /// <param name="imgVector"></param>
        /// <param name="switchExp">The expression to use in the resulting switch / call.</param>
        /// <returns></returns>
        private bool DiscoverTableExtent(
            IProcessorArchitecture arch,
            RtlBlock rtlBlock,
            ProcessorState state,
            Address addrSwitch,
            RtlTransfer xfer,
            out List<Address> vector,
            out ImageMapVectorTable imgVector,
            out Expression switchExp)
        {
            Debug.Assert(!(xfer.Target is Address || xfer.Target is Constant), $"This should not be a constant {xfer}.");
            vector = null!;
            imgVector = null!;
            switchExp = null!;

            var bwsHost = scanner.MakeBackwardSlicerHost(arch, miniCfg);
            var bws = new BackwardSlicer(bwsHost, rtlBlock, state);
            var te = bws.DiscoverTableExtent(addrSwitch, xfer, listener);
            if (te == null)
                return false;
            foreach (var (addr, dt) in te.Accesses!)
            {
                scanner.MarkDataInImageMap(addr, dt);
            }
            imgVector = new ImageMapVectorTable(
                null!, // bw.VectorAddress,
                te.Targets!.ToArray(),
                4); // builder.TableByteSize);
            vector = te.Targets;
            switchExp = te.Index!;
            return true;
        }

        protected void RegisterEdge(Edge edge)
        {
            RegisterEdgeInMiniCfg(edge);
            scanner.RegisterEdge(edge);
        }

        private void RegisterEdgeInMiniCfg(Edge edge)
        {
            if (!miniCfg.TryGetValue(edge.To, out var edges))
            {
                edges = new List<Address>();
                miniCfg.Add(edge.To, edges);
            }
            edges.Add(edge.From);
        }

        public RtlAssignment MkTmp(Expression e)
        {
            var tmp = binder.CreateTemporary(e.DataType);
            return new RtlAssignment(tmp, e);
        }

        public IEnumerable<RtlInstructionCluster> MakeTrace(Address addr, ProcessorState state)
        {
            return scanner.MakeTrace(addr, state, binder);
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

        protected abstract void ProcessCall(RtlBlock block, Edge edge, ProcessorState state);

        protected abstract void ProcessReturn();

        /// <summary>
        /// Attempt to mark the address <paramref name="addr"/> as visited.
        /// </summary>
        /// <param name="addr">Address to mark as visited.</param>
        /// <returns>True if the address hadn't been visited before, false
        /// if the address had been visited before.
        /// </returns>
        public abstract bool MarkVisited(Address addr);

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
