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
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Scanning
{
    /// <summary>
    /// This class processes a linear sequence of instructions, resulting in
    /// an <see cref="RtlBlock"/>
    /// </summary>
    public abstract class AbstractBlockWorker
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(BlockWorker), "");

        private readonly AbstractScanner scanner;
        private readonly AbstractProcedureWorker worker;
        private ProcessorState state;

        protected AbstractBlockWorker(
            AbstractScanner scanner,
            AbstractProcedureWorker worker,
            Address address,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            this.scanner = scanner;
            this.worker = worker;
            this.Address = address;
            this.Trace = trace;
            this.state = state;
        }

        /// <summary>
        /// Address of the start of the current block being parsed.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// Trace of RTL instructions being read from the binary.
        /// </summary>
        public IEnumerator<RtlInstructionCluster> Trace { get; }

        /// <summary>
        /// Performs a linear scan, stopping when a CTI is encountered, or we
        /// run off the edge of the world.
        /// </summary>
        /// <returns>
        /// A pair of a completed <see cref="Block"/> and an updated <see cref="ProcessorState"/>.
        /// If parsing runs off the end of memory, the returned block will be marked
        /// as invalid.
        /// </returns>
        public (RtlBlock, ProcessorState) ParseBlock()
        {
            var instrs = new List<RtlInstructionCluster>();
            var trace = this.Trace;
            var state = this.state;
            var addrLast = this.Address;
            while (trace.MoveNext())
            {
                var cluster = trace.Current;
                addrLast = cluster.Address;
                if (!worker.MarkVisited(addrLast))
                {
                    var block = MakeFallthroughBlock(addrLast, instrs);
                    return (block, state);
                }
                foreach (var rtl in cluster.Instructions)
                {
                    trace_Verbose("      {0}: {1}", cluster.Address, rtl);
                    switch (rtl)
                    {
                    case RtlAssignment ass:
                        EmulateState(ass);
                        continue;
                    case RtlSideEffect side:
                        //$TODO: emulate side effect.
                        continue;
                    case RtlNop _:
                        continue;
                    case RtlInvalid _:
                        instrs.Add(cluster);
                        var size = addrLast - this.Address + cluster.Length;
                        return (MakeInvalidBlock(instrs, size), state);
                    case RtlBranch branch:
                        //Expand sub-instruction statements in a later pass.
                        if (branch.NextStatementRequiresLabel)
                        {
                            continue;
                        }
                        break;
                    case RtlGoto g:
                    case RtlCall call:
                    case RtlReturn ret:
                        break;
                    default:
                        throw new NotImplementedException($"{rtl.GetType()} - not implemented");
                    }
                    Debug.Assert(rtl.Class.HasFlag(InstrClass.Transfer));
                    return MakeBlock(instrs, state, rtl);
                }
                instrs.Add(cluster);
            }
            // Fell off the end, mark as bad.
            return (MakeInvalidBlock(instrs, addrLast - this.Address), state);
        }

        private RtlBlock MakeFallthroughBlock(Address addrFallthrough, List<RtlInstructionCluster> instrs)
        {
            return scanner.RegisterBlock(
                this.state.Architecture,
                this.Address,
                addrFallthrough - this.Address,
                addrFallthrough,
                instrs);
        }

        protected abstract void EmulateState(RtlAssignment ass);

        /// <summary>
        /// The <see cref="Trace"/> is positioned on a CTI with a delay slot.
        /// We swap the positions of the delay slot instruction and the CTI
        /// so that the CTI appears last in the block.
        /// </summary>
        /// <param name="rtlTransfer">The CTI instruction.</param>
        /// <param name="instrs"></param>
        /// <returns>False if another CTI was found in the first CTI delay 
        /// slot. Reko currently doesn't handle this rare idiom, although 
        /// SPARC does allow it.
        /// </returns>
        protected bool StealDelaySlot(
            RtlInstructionCluster rtlTransfer,
            List<RtlInstructionCluster> instrs)
        {
            if (!Trace.MoveNext())
            {
                // Fell off the end of memory, CTI is an invalid instruction.
                return false;
            }
            var rtlDelayed = Trace.Current;
            if (rtlDelayed.Class.HasFlag(InstrClass.Transfer))
            {
                // Can't deal with transfer functions in delay slots yet.
                return false;
            }
            // If the delay slot instruction is a nop (padding), we 
            // can ignore it.
            if (!rtlDelayed.Class.HasFlag(InstrClass.Padding))
            {
                // Delay slot instruction does real work, so we will insert it
                // before the CTI instruction.
                switch (rtlTransfer.Instructions[^1])
                {
                case RtlBranch branch:
                    if (branch.Condition is not Constant)
                    {
                        var tmp = worker.MkTmp(branch.Condition);
                        rtlTransfer = new RtlInstructionCluster(
                            rtlTransfer.Address,
                            rtlTransfer.Length,
                            tmp,
                            new RtlBranch(tmp.Dst, (Address)branch.Target, InstrClass.ConditionalTransfer));
                    }
                    break;
                case RtlGoto g:
                    if (g.Target is not Core.Address)
                    {
                        var tmp = worker.MkTmp(g.Target);
                        rtlTransfer = new RtlInstructionCluster(
                            rtlTransfer.Address,
                            rtlTransfer.Length,
                            tmp,
                            new RtlGoto(tmp.Dst, InstrClass.Transfer));
                    }
                    break;
                case RtlReturn:
                    break;
                default:
                    throw new NotImplementedException($"{rtlTransfer.GetType().Name} - not implemented.");
                }
                instrs.Add(rtlDelayed);
            }
            instrs.Add(rtlTransfer);
            return true;
        }

        /// <summary>
        /// After reaching a CTI, make a <see cref="Block"/>.
        /// </summary>
        /// <param name="instrs">The instuctions of the resulting <see cref="Block"/>.</param>
        /// <param name="state">The current <see cref="ProcessorState"/>.</param>
        /// <param name="rtlTransfer">The transfer instruction that triggers the creation
        /// of the block.</param>
        /// A pair of a completed <see cref="Block"/> and an updated <see cref="ProcessorState"/>.
        /// If parsing runs off the end of memory, the block reference will be null.
        /// </returns>
        private (RtlBlock, ProcessorState) MakeBlock(
            List<RtlInstructionCluster> instrs,
            ProcessorState state,
            RtlInstruction rtlTransfer)
        {
            // We are positioned at the CTI.
            var cluster = this.Trace.Current;

            var size = cluster.Address - this.Address + cluster.Length;

            // Make sure we're not heading to hyperspace.
            if (rtlTransfer is RtlTransfer xfer &&
                xfer.Target is Address addrTarget &&
                !scanner.IsExecutableAddress(addrTarget))
            {
                instrs.Add(new RtlInstructionCluster(cluster.Address, cluster.Length, new RtlInvalid()));
                return (MakeInvalidBlock(instrs, size), state);
            }

            if (rtlTransfer.Class.HasFlag(InstrClass.Delay))
            {
                if (!StealDelaySlot(cluster, instrs))
                    return (MakeInvalidBlock(instrs, size), state);
            }
            else
            {
                instrs.Add(cluster);
            }
            // The trace may have moved if a delay slot was consumed.
            cluster = this.Trace.Current;
            var addrFallthrough = cluster.Address + cluster.Length;
            var block = scanner.RegisterBlock(
                this.state.Architecture,
                this.Address,
                size,
                addrFallthrough,
                instrs);
            return (block, state);
        }

        private RtlBlock MakeInvalidBlock(
            List<RtlInstructionCluster> instrs,
            long size)
        {
            var block = scanner.RegisterBlock(
                this.state.Architecture,
                this.Address,
                size,
                this.Address + size,
                instrs);
            block.IsValid = false;
            return block;
        }

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
