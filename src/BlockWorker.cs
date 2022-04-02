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
    public class BlockWorker
    {
        private static TraceSwitch trace = new TraceSwitch(nameof(BlockWorker), "");

        private readonly RecursiveScanner scanner;
        private readonly IStorageBinder binder;
        private readonly ProcedureWorker pw;
        public Address Address { get; }
        public readonly IEnumerator<RtlInstructionCluster> Trace;
        public readonly ProcessorState State;

        public BlockWorker(
            RecursiveScanner scanner,
            IStorageBinder binder,
            ProcedureWorker pw,
            Address Address,
            IEnumerator<RtlInstructionCluster> Trace,
            ProcessorState State)
        {
            this.scanner = scanner;
            this.binder = binder;
            this.pw = pw;
            this.Address = Address;
            this.Trace = Trace;
            this.State = State;
        }

        /// <summary>
        /// Performs a linear scan, stopping when a CTI is encountered,
        /// or we run off the edge of the world.
        /// </summary>
        /// <param name="work">Work item to use</param>
        /// <returns></returns>
        public (Block?, ProcessorState) ParseBlock()
        {
            trace_Verbose("    {0}: Parsing block at {1}", pw.Procedure.Address, this.Address);
            var instrs = new List<(Address, RtlInstruction)>();
            var trace = this.Trace;
            var state = this.State;
            while (trace.MoveNext())
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
                    return ParseTransferInstruction(instrs, state, cluster.Address, rtl);
                }
            }
            // Fell off the end.
            return (null, state);
        }

        private (Block?, ProcessorState) ParseTransferInstruction(
            List<(Address, RtlInstruction)> instrs,
            ProcessorState state,
            Address addr,
            RtlInstruction rtl)
        {
            var size = addr - this.Address + this.Trace.Current.Length;
            if (rtl.Class.HasFlag(InstrClass.Delay))
            {
                if (!MaybeStealDelaySlot(rtl, instrs))
                    return (null, state);
            }
            else
            {
                instrs.Add((addr, rtl));
            }
            var addrFallthrough = Trace.Current.Address + Trace.Current.Length;
            var block = scanner.RegisterBlock(
                this.State.Architecture,
                this.Address,
                size,
                addrFallthrough,
                instrs);
            return (block, state);
        }


        private bool MaybeStealDelaySlot(
            RtlInstruction rtlTransfer,
            List<(Address, RtlInstruction)> instrs)
        {
            Expression MkTmp(Address addr, Expression e)
            {
                var tmp = binder.CreateTemporary(e.DataType);
                instrs.Add((addr, new RtlAssignment(tmp, e)));
                return tmp;
            }

            Expression? tmp = null;
            var addrTransfer = Trace.Current.Address;
            switch (rtlTransfer)
            {
            case RtlBranch branch:
                if (branch.Condition is not Constant)
                {
                    tmp = MkTmp(addrTransfer, branch.Condition);
                    rtlTransfer = new RtlBranch(tmp, (Address)branch.Target, InstrClass.ConditionalTransfer);
                }
                break;
            case RtlGoto g:
                if (g.Target is not Core.Address)
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
            if (!Trace.MoveNext())
                return false;
            var rtlDelayed = Trace.Current;
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
