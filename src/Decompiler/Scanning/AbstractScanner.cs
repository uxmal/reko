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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public abstract class AbstractScanner
    {
        protected static readonly TraceSwitch trace = new(nameof(AbstractScanner), "")
        {
            Level = TraceLevel.Verbose
        };

        protected readonly Program program;
        protected readonly ScanResultsV2 sr;
        protected readonly DecompilerEventListener listener;
        protected readonly ConcurrentDictionary<Address, Address> blockStarts;
        protected readonly ConcurrentDictionary<Address, Address> blockEnds;
        private readonly IRewriterHost host;

        protected AbstractScanner(Program program, ScanResultsV2 sr, DecompilerEventListener listener)
        {
            this.program = program;
            this.sr = sr;
            this.listener = listener;
            this.host = new RewriterHost(program, listener);
            this.blockStarts = new ConcurrentDictionary<Address, Address>();
            this.blockEnds = new ConcurrentDictionary<Address, Address>();
        }

        /// <summary>
        /// Returns true if the given address is inside an executable
        /// <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>Whether or not the address is executable.</returns>
        public bool IsExecutableAddress(Address addr) =>
            program.SegmentMap.IsExecutableAddress(addr);


        public IBackWalkHost<RtlBlock, RtlInstruction> MakeBackwardSlicerHost(
            IProcessorArchitecture arch,
            Dictionary<Address, List<Address>> backEdges)
        {
            return new CfgBackWalkHost(program, arch, sr, backEdges);
        }

        public IEnumerable<RtlInstructionCluster> MakeTrace(
            Address addr,
            ProcessorState state,
            IStorageBinder binder)
        {
            var arch = state.Architecture;
            var rdr = program.CreateImageReader(arch, addr);
            var rw = arch.CreateRewriter(rdr, state, binder, host);
            return rw;
        }

        public void MarkDataInImageMap(Address addr, DataType dt)
        {
            if (program.ImageMap is null)
                return;
            var item = new ImageMapItem(addr, (uint)dt.Size)
            {
                DataType = dt
            };
            program.ImageMap.AddItemWithSize(addr, item);
        }

        /// <summary>
        /// Register a block starting at address <paramref name="addrBlock"/>.
        /// </summary>
        /// <remarks>
        /// A precondition is that a successful call to <see cref="TryRegisterBlockStart(Address, Address)"/>
        /// was done first; no other thread will attempt to register a block
        /// after that.
        /// </remarks>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> for this block.</param>
        /// <param name="addrBlock">The <see cref="Address"/> at which the block is located.</param>
        /// <param name="length">The length of the block, excluding</param>
        /// <param name="addrFallthrough"></param>
        /// <param name="instrs"></param>
        /// <returns></returns>
        public RtlBlock RegisterBlock(
            IProcessorArchitecture arch,
            Address addrBlock,
            long length,
            Address addrFallthrough,
            List<RtlInstructionCluster> instrs)
        {
            Debug.Assert(this.blockStarts.ContainsKey(addrBlock));
            var id = program.NamingPolicy.BlockName(addrBlock);
            var block = new RtlBlock(arch, addrBlock, id, (int)length, addrFallthrough, instrs);
            var success = sr.Blocks.TryAdd(addrBlock, block);
            Debug.Assert(success, $"Failed registering block at {addrBlock}");
            return block;
        }

        public void RegisterEdge(Edge edge)
        {
            if (!sr.Successors.TryGetValue(edge.From, out var edges))
            {
                edges = new List<Address>();
                sr.Successors.TryAdd(edge.From, edges);
            }
            //$TODO: make this concurrent safe.
            edges.Add(edge.To);
        }

        public ScanResultsV2 RegisterPredecessors()
        {
            foreach (var (pred, succs) in sr.Successors)
            {
                foreach (var succ in succs)
                {
                    if (!sr.Predecessors.TryGetValue(succ, out var sps))
                    {
                        sps = new List<Address>();
                        sr.Predecessors.TryAdd(succ, sps);
                    }
                    sps.Add(pred);
                }
            }
            return sr;
        }

        public void RegisterSpeculativeProcedure(Address addrProc)
        {
            sr.SpeculativeProcedures.AddOrUpdate(addrProc, 1, (a, v) => v+1);
        }

        protected void StealEdges(Address from, Address to)
        {
            if (sr.Successors.TryGetValue(from, out var succs))
            {
                sr.Successors.TryRemove(from, out _);
                var newEges = succs.ToList();
                sr.Successors.TryAdd(to, newEges);
            }
        }

        public bool TryRegisterBlockStart(Address addrBlock, Address addrProc)
        {
            return this.blockStarts.TryAdd(addrBlock, addrProc);
        }

        public virtual bool TryRegisterBlockEnd(Address addrBlockStart, Address addrBlockLast)
        {
            return this.blockEnds.TryAdd(addrBlockLast, addrBlockStart);
        }

        /// <summary>
        /// Creates a new block from an existing block, using the instruction range
        /// [iStart, iEnd).
        /// </summary>
        /// <param name="block">The block to chop.</param>
        /// <param name="iStart">Index of first instruction in the new block.</param>
        /// <param name="iEnd">Index of the first instruction to not include.</param>
        /// <param name="blockSize">The size in address units of the resulting block.</param>
        /// <returns>A new, terminated but unregistered block. The caller is responsible for 
        /// registering it.
        /// </returns>
        protected RtlBlock Chop(RtlBlock block, int iStart, int iEnd, long blockSize)
        {
            var instrs = new List<RtlInstructionCluster>(iEnd - iStart);
            for (int i = iStart; i < iEnd; ++i)
            {
                instrs.Add(block.Instructions[i]);
            }
            var addr = instrs[0].Address;
            var instrLast = instrs[^1];
            var id = program.NamingPolicy.BlockName(addr);
            var addrFallthrough = addr + blockSize;
            trace.Verbose("      new block at {0}", addr);
            return new RtlBlock(block.Architecture, addr, id, (int)blockSize, addrFallthrough, instrs);
        }

        public ExpressionSimplifier CreateEvaluator(ProcessorState state)
        {
            return new ExpressionSimplifier(
                program.SegmentMap,
                state,
                listener);
        }

        public void SplitBlockEndingAt(RtlBlock block, Address addrEnd)
        {
            var wl = new Queue<(RtlBlock block, Address addrEnd)>();
            wl.Enqueue((block, addrEnd));
            while (wl.TryDequeue(out var item))
            {
                trace.Verbose("    Splitting block {0} at {1}", block.Address, addrEnd);
                RtlBlock blockB;
                (blockB, addrEnd) = item;
                // Invariant: we arrive here if blockB ends at the same
                // address as some other block A. Get the address of that
                // block.

                var addrA = this.blockEnds[addrEnd];
                var addrB = block.Address;

                // If both addresses are the same, we have a self-loop. No need for splitting.
                if (addrA == addrB)
                    continue;

                var blockA = sr.Blocks[addrA];

                // Find the indices where both blocks have the same instructions: "shared tails"
                // Because both blocks end at addrEnd, we're guaranteed at least one shared instruction.
                var (a, b) = FindSharedInstructions(blockA, blockB);

                if (a > 0 && b > 0)
                {
                    // The shared instructions S do not fully take up either of the
                    // two blocks. One of the blocks starts in the middle of an
                    // instruction in the other block:
                    //
                    //  addrA     S
                    //  +-+--+--+-+-----+
                    //      +--+--+-----+
                    //      addrB
                    //
                    // We want to create a new block S with the shared instructions:
                    //  addrA       S
                    //  +-+--+--+-+ +-----+
                    //      +--+--+
                    var addrS = blockA.Instructions[a].Address;
                    if (TryRegisterBlockStart(addrS, blockA.Address))
                    {
                        // S didn't exist already.
                        var sSize = blockA.Length - (addrS - blockA.Address);
                        var instrs = blockA.Instructions.Skip(a).ToList();
                        var blockS = RegisterBlock(
                            blockA.Architecture,
                            addrS,
                            sSize,
                            blockA.FallThrough,
                            instrs);
                        trace.Verbose("      new block at {0}", addrS);
                        StealEdges(addrA, addrS);
                    }
                    // Trim off the last instructions of A and B, then
                    // replace their original values
                    //$REVIEW: the below code is not thread safe.
                    RegisterEdge(new Edge(addrA, addrS, EdgeType.Jump));
                    RegisterEdge(new Edge(addrB, addrS, EdgeType.Jump));
                    var newA = Chop(blockA, 0, a, addrS - addrA);
                    var newB = Chop(blockB, 0, b, addrS - addrB);
                    sr.Blocks.TryUpdate(addrA, newA, blockA);
                    sr.Blocks.TryUpdate(addrB, newB, blockB);
                    blockEnds.TryUpdate(newA.Address, addrS, addrEnd);
                    blockEnds.TryUpdate(newB.Address, addrS, addrEnd);
                }
                else if (a == 0)
                {
                    // Block B falls through into block A
                    // 
                    //       addrA
                    //       +-----------+
                    //  +----------------+
                    //  addrB
                    //
                    // We split block B so that it falls through to block A
                    var newB = Chop(blockB, 0, b, addrA - addrB);
                    sr.Blocks.TryUpdate(addrB, newB, blockB);
                    RegisterEdge(new Edge(blockB.Address, addrA, EdgeType.Jump));
                    var newBEnd = newB.Instructions[^1].Address;
                    if (!TryRegisterBlockEnd(newB.Address, newBEnd))
                    {
                        // There is already a block ending at newBEnd.
                        wl.Enqueue((newB, newBEnd));
                    }
                }
                else
                {
                    Debug.Assert(b == 0);
                    // Block A falls through into B
                    //
                    // addrA
                    // +----------------+
                    //       +----------+
                    //       addrB
                    // We split block A so that it falls through to B. We make
                    // B be the new block end, and move the out edges of block
                    // A to block B.
                    var newA = Chop(blockA, 0, a, addrB - addrA);
                    sr.Blocks.TryUpdate(addrA, newA, blockA);
                    //$TODO: check for race conditions.
                    if (!blockEnds.TryUpdate(addrEnd, addrB, addrA))
                    {
                        throw new Exception("Who stole it?");
                    }
                    StealEdges(blockA.Address, blockB.Address);
                    RegisterEdge(new Edge(newA.Address, blockB.Address, EdgeType.Jump));
                    var newAEnd = newA.Instructions[^1].Address;
                    if (!TryRegisterBlockEnd(newA.Address, newAEnd))
                    {
                        // There is already a block ending at newAEnd
                        wl.Enqueue((newA, newAEnd));
                    }
                }
            }
        }

        /// <summary>
        /// Starting at the end of two blocks, walk towards their respective beginnings
        /// while the addresses match. 
        /// </summary>
        /// <param name="blockA">First block</param>
        /// <param name="blockB">Second block</param>
        /// <returns>A pair of indices into the respective instruction lists where the 
        /// instruction addresses start matching.
        /// </returns>
        private (int, int) FindSharedInstructions(RtlBlock blockA, RtlBlock blockB)
        {
            int a = blockA.Instructions.Count - 1;
            int b = blockB.Instructions.Count - 1;
            for (; a >= 0 && b >= 0; --a, --b)
            {
                if (blockA.Instructions[a].Address != blockB.Instructions[b].Address)
                    break;
            }
            return (a + 1, b + 1);
        }

        private void DumpBlocks(ScanResultsV2 sr, IDictionary<Address, RtlBlock> blocks)
        {
            DumpBlocks(sr, blocks, s => Debug.WriteLine(s));
        }

        // Writes the start and end addresses, size, and successor edges of each block, 
        public void DumpBlocks(ScanResultsV2 sr, IDictionary<Address, RtlBlock> blocks, Action<string> writeLine)
        {
            writeLine(
               string.Join(Environment.NewLine,
               from b in blocks.Values
               orderby b.Address
               select string.Format(
                   "{0:X8}-{1:X8} ({2}): {3}{4}",
                       b.Address,
                       b.FallThrough,
                       b.Length,
                       RenderType(b.Instructions[^1].Class),
                       string.Join(", ",sr.Successors.TryGetValue(b.Address, out var succ)
                            ? succ
                            : new List<Address>()))));

            string RenderType(InstrClass t)
            {
                if ((t & InstrClass.Zero) != 0)
                    return "Zer ";
                if ((t & InstrClass.Padding) != 0)
                    return "Pad ";
                if ((t & InstrClass.Call) != 0)
                    return "Cal ";
                if ((t & InstrClass.ConditionalTransfer) == InstrClass.ConditionalTransfer)
                    return "Bra ";
                if ((t & InstrClass.Transfer) != 0)
                    return "End";
                return "Lin ";
            }
        }

        private class RewriterHost : IRewriterHost
        {
            private readonly Program program;
            private readonly DecompilerEventListener listener;

            public RewriterHost(Program program, DecompilerEventListener listener)
            {
                this.program = program;
                this.listener = listener;
            }

            public Constant? GlobalRegisterValue => program.GlobalRegisterValue;

            public IProcessorArchitecture GetArchitecture(string archMoniker)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public IntrinsicProcedure EnsureIntrinsic(string name, bool hasSideEffect, DataType returnType, int arity)
            {
                var args = Enumerable.Range(0, arity).Select(i => Constant.Create(program.Architecture.WordWidth, 0)).ToArray();
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                return intrinsic;
            }

            public Expression Intrinsic(string name, bool hasSideEffect, DataType returnType, params Expression[] args)
            {
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                return new Application(
                    new ProcedureConstant(program.Architecture.PointerType, intrinsic),
                    returnType,
                    args);
            }

            public Expression Intrinsic(string name, bool hasSideEffect, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                var intrinsic = program.EnsureIntrinsicProcedure(name, hasSideEffect, returnType, args);
                intrinsic.Characteristics = c;
                return new Application(
                    new ProcedureConstant(program.Architecture.PointerType, intrinsic),
                    returnType,
                    args);
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string format, params object[] args)
            {
                var location = listener.CreateAddressNavigator(program, address);
                listener.Error(location, format, args);
            }

            public void Warn(Address address, string format, params object[] args)
            {
                var location = listener.CreateAddressNavigator(program, address);
                listener.Error(location, format, args);
            }
        }
    }
}
