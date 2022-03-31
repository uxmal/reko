using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public class ProcedureWorker : Worker
    {
        private RecursiveScanner scanner;
        private Proc proc;
        private WorkList<BlockItem> workList;
        private IStorageBinder binder;

        public ProcedureWorker(RecursiveScanner scanner, Proc proc, ProcessorState state)
        {
            this.scanner = scanner;
            this.proc = proc;
            this.workList = new WorkList<BlockItem>();
            this.binder = new StorageBinder();
            var trace = scanner.MakeTrace(proc.Address, state, binder).GetEnumerator();
            this.workList.Add(new BlockItem(proc.Address, trace, state));
        }

        public override void Run()
        {
            for (; ; )
            {
                while (workList.TryGetWorkItem(out var work))
                {
                    var (addr, trace, state) = work;
                    if (!scanner.TryRegisterBlockStart(addr, proc.Address))
                        //$TODO someone else scanned our block;
                        continue;
                    var (block, stateNew) = ParseBlock(work);
                    HandleBlockEnd(block, stateNew);
                }
                //$Check for indirects
                //$Check for calls.
                return;
            }
        }

        private (Block, ProcessorState) ParseBlock(BlockItem work)
        {
            var instrs = new List<RtlInstruction>();
            var trace = work.Trace;
            while (work.Trace.MoveNext())
            {
                var cluster = work.Trace.Current;
                foreach (var rtl in cluster.Instructions)
                {
                    switch (rtl)
                    {
                    case RtlAssignment ass:
                        //$TODO: emulate state;
                        break;
                    case RtlBranch branch:
                        break;
                    }
                }
            }
            // Fell off the end.
            return (null, null);
        }

        private void HandleBlockEnd(Block block, ProcessorState state)
        {
            var (lastAddr, lastInstr) = block.Instructions[^1];
            if (scanner.TryRegisterBlockEnd(block.Address, lastAddr))
            {
                var edges = ComputeEdges(lastInstr);
                foreach (var edge in edges)
                {
                    switch (edge.Type)
                    {
                    case EdgeType.DirectJump:
                        //$TODO mutate state depending on outcome
                        var trace = scanner.MakeTrace(edge.To, state, binder).GetEnumerator();
                        workList.Add(new BlockItem(edge.To, trace, state));
                        break;
                    case EdgeType.Return:
                        scanner.ResumeWorkers(proc.Address);
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
        }

        private List<Edge> ComputeEdges(Instruction instr)
        {
            throw new NotImplementedException();
        }

        private record BlockItem(
            Address Address,
            IEnumerator<RtlInstructionCluster> Trace,
            ProcessorState State);
    }
}
