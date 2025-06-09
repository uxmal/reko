#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class provides services for building control flow graph edges.
    /// Derived class instances should be able to run in separate threads, 
    /// so avoid introducing _mutable_ shared state as much as possible.
    /// </summary>
    public abstract class AbstractProcedureWorker
    {
        protected static readonly TraceSwitch log = new TraceSwitch(nameof(AbstractProcedureWorker), "AbstractProcedureWorker tracing")
        {
            Level = TraceLevel.Warning,
        };

        private readonly AbstractScanner scanner;
        private readonly InstrClass rejectMask;
        private readonly IDecompilerEventListener listener;
        private readonly IStorageBinder binder;
        private readonly Dictionary<Address, List<Address>> miniCfg;

        protected AbstractProcedureWorker(
            AbstractScanner scanner, 
            Address address, 
            InstrClass rejectMask, 
            IDecompilerEventListener listener)
        {
            this.scanner = scanner;
            this.Address = address;
            this.rejectMask = rejectMask;
            this.listener = listener;
            this.binder = new StorageBinder();
            this.miniCfg = new Dictionary<Address, List<Address>>();
        }

        public Address Address { get; }

        /// <summary>
        /// Handle the situation when a control transfer instruction (CTI) has
        /// been reached.
        /// </summary>
        /// <param name="block">The unsealed block we are currently building</param>
        /// <param name="trace">A source of <see cref="RtlInstructionCluster"/>s.</param>
        /// <param name="subinstrTargets">A possibly null list of addresses that are
        /// targeted by sub-instructions.</param>
        /// <param name="state">Current simulated processor state.</param>
        /// <returns>The completed basic block.</returns>
        public RtlBlock HandleBlockEnd(
            RtlBlock block,
            IEnumerator<RtlInstructionCluster> trace,
            List<Address>? subinstrTargets,
            ProcessorState state)
        {
            log.Verbose("    {0}: Finished block at {1}", this.Address, block.Address);
            var lastCluster = block.Instructions[^1];
            var lastInstr = lastCluster.Instructions[^1];
            var lastAddr = lastCluster.Address;
            if (scanner.TryRegisterBlockEnd(block.Address, lastAddr))
            {
                // This thread is the first one to see this block. Create the
                // out edges from the block.
                var edges = ComputeEdges(lastInstr, lastAddr, block, state);
                foreach (var edge in edges)
                {
                    switch (edge.Type)
                    {
                    case EdgeType.Fallthrough:
                        // Reuse the same trace. This is necessary to handle the 
                        // ARM Thumb IT instruction, which puts state in the trace.
                        RegisterEdge(edge);
                        AddFallthroughJob(edge.To, trace, state);
                        log.Verbose("    {0}: added edge from {1} to {2}", this.Address, edge.From, edge.To);
                        break;
                    case EdgeType.Jump:
                        // Start a new trace somewhere else.
                        RegisterEdge(edge);
                        AddJob(edge.To, state);
                        log.Verbose("    {0}: added edge from {1} to {2}", this.Address, edge.From, edge.To);
                        break;
                    case EdgeType.Call:
                        // Processing calls may involve "parking" this worker while
                        // waiting for the callee to return.
                        ProcessCall(block, edge, state);
                        break;
                    case EdgeType.Return:
                        ProcessReturn();
                        break;
                    default:
                        throw new NotImplementedException($"{edge.Type} edge not handled yet.");
                    }
                }
                if (subinstrTargets is not null)
                {
                    foreach (var addrTarget in subinstrTargets)
                    {
                        var edge = new Edge(block.Address, addrTarget, EdgeType.Jump);
                        RegisterEdge(edge);
                        AddJob(addrTarget, state);
                        log.Verbose("    {0}: added sub-instruction edge from {1} to {2}", this.Address, edge.From, edge.To);
                    }
                }
            }
            else
            {
                // Some other thread reached the end of this block first. We now 
                // have potentially two blocks sharing the same end address.
                log.Verbose("    {0}: Splitting block at [{1}-{2}]", this.Address, block.Address, lastAddr);
                scanner.SplitBlockEndingAt(block, lastAddr);
            }
            return block;
        }

        protected void HandleBadBlock(Address addrBadBlock)
        {
            log.Verbose("    {0}: Bad block", addrBadBlock);
            scanner.RegisterInvalidBlock(addrBadBlock);
            //$TODO: enqueue low-prio item for removing all unconditional predecessor blocks
            // that reach this block
        }

        /// <summary>
        /// Given an <see cref="RtlInstruction"/> <paramref name="instr"/>, determine 
        /// what edges in the control flow graph are generated by it.
        /// </summary>
        /// <param name="instr">The control transfer instruction.</param>
        /// <param name="addrInstr">The address of that instruction.</param>
        /// <param name="block">The current basic block.</param>
        /// <param name="state">Current simulated processor state.</param>
        /// <returns>A list of <see cref="Edge"/>s exiting from this block.</returns>
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
                else if (TryRegisterTrampoline(addrInstr, block.Instructions, out var trampoline))
                {
                    result.Add(new Edge(block.Address, block.Address, EdgeType.Return));
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
                    //$TODO: indirect calls. For now assume that indirect calls return
                    // to their caller, so we fall through to the next statement.
                    result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough));
                }
                break;
            case RtlReturn:
                result.Add(new Edge(block.Address, block.Address, EdgeType.Return));
                break;
            case RtlAssignment:
            case RtlSideEffect:
            case RtlNop:
                if (!instr.Class.HasFlag(InstrClass.Terminates))
                {
                    result.Add(new Edge(block.Address, block.FallThrough, EdgeType.Fallthrough));
                }
                break;
            default:
                throw new NotImplementedException();
            }
            return result;
        }

        protected BlockWorker CreateBlockWorker(
            AbstractScanner scanner,
            AbstractProcedureWorker worker,
            Address addr,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
        {
            return new BlockWorker(scanner, worker, addr, trace, state, this.rejectMask);
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
            [MaybeNullWhen(false)] out List<Address> vector,
            [MaybeNullWhen(false)] out ImageMapVectorTable imgVector,
            [MaybeNullWhen(false)] out Expression switchExp)
        {
            Debug.Assert(!(xfer.Target is Address || xfer.Target is Constant), $"This should not be a constant {xfer}.");
            vector = null;
            imgVector = null;
            switchExp = null;

            // We need the `miniCfg` dictionary to support backtracking; it contains
            // back edges that the BacwardSlicer uses.
            var bwsHost = scanner.MakeBackwardSlicerHost(arch, miniCfg);
            var bws = new BackwardSlicer(bwsHost, rtlBlock, state);
            var te = bws.DiscoverTableExtent(addrSwitch, xfer, listener);
            if (te is null)
                return false;
            foreach (var (addr, dt) in te.Accesses!)
            {
                scanner.MarkDataInImageMap(addr, dt);
            }
            imgVector = new ImageMapVectorTable(
                default, // bw.VectorAddress,
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

        public (Identifier tmp, RtlInstructionCluster) MkTmp(RtlInstructionCluster cluster, Expression e)
        {
            var tmp = binder.CreateTemporary(e.DataType);
            var ass = new RtlAssignment(tmp, e);
            return (tmp, new RtlInstructionCluster(cluster.Address, cluster.Length, ass));
        }

        public IEnumerator<RtlInstructionCluster> MakeTrace(Address addr, ProcessorState state)
        {
            return scanner.MakeTrace(addr, state, binder).GetEnumerator();
        }

        /// <summary>
        /// Add a job to this worker, by creating a new trace.
        /// </summary>
        /// <param name="addr">The address at which to start.</param>
        /// <param name="state">The state to use.</param>
        public BlockWorker AddJob(Address addr, ProcessorState state)
        {
            var trace = scanner.MakeTrace(addr, state, binder).GetEnumerator();
            return AddJob(addr, trace, state);
        }

        /// <summary>
        /// Add a job, using the provided trace of <see cref="RtlInstructionCluster"/>s.
        /// </summary>
        public abstract BlockWorker AddJob(
            Address addr,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state);

        public void AddCallFallthroughJob(Address addrCaller, Address addrFallthrough, ProcessorState state)
        {
            var blockWorker = AddJob(addrFallthrough, state);
            blockWorker.CallerBlockAddress = addrCaller;
        }

        /// <summary>
        /// Add a job, using the provided trace of <see cref="RtlInstructionCluster"/>s, and
        /// hinting that this trace is to be reused because the scanning process is crossing
        /// a block boundary without encountering a control transfer instrution.
        /// </summary>
        /// <param name="addr">Address at which the next block should start.</param>
        /// <param name="trace">Instruction trace to continue using.</param>
        /// <param name="state">Current emulated processor state.</param>
        /// <returns>A <see cref="BlockWorker"/> reusing the trace.</returns>
        public abstract BlockWorker AddFallthroughJob(
            Address addr,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state);


        protected abstract void ProcessCall(RtlBlock blockCaller, Edge edge, ProcessorState state);

        protected abstract void ProcessReturn();

        protected abstract bool TryRegisterTrampoline(
            Address addrFinalInstr,
            List<RtlInstructionCluster> trampolineStub,
            [MaybeNullWhen(false)] out Trampoline trampoline);


        /// <summary>
        /// Attempt to mark the address <paramref name="addr"/> as visited.
        /// </summary>
        /// <param name="addr">Address to mark as visited.</param>
        /// <returns>True if the address hadn't been visited before, false
        /// if the address had been visited before.
        /// </returns>
        /// <remarks>This is a common operation when shingle scanning,
        /// so it has to be very fast.
        /// </remarks>
        public abstract bool TryMarkVisited(Address addr);
        
        /// <summary>
        /// Splits any existing block that contains an instruction
        /// exactly on the address <paramref name="addr"/>.
        /// </summary>
        /// <returns>The newly split block, or null if there is no
        /// block with an instruction at address <paramref name="addr"/>.</returns>
        public abstract RtlBlock? SplitExistingBlock(Address addr);
    }
}
