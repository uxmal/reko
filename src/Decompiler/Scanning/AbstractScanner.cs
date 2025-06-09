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
using Reko.Core.Configuration;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Abstract base class for Scanner classes.
    /// </summary>
    public abstract class AbstractScanner
    {
        protected static readonly TraceSwitch trace = new(nameof(AbstractScanner), "")
        {
            Level = TraceLevel.Warning
        };

        protected readonly IServiceProvider services;
        protected readonly Program program;
        protected readonly ScanResultsV2 sr;
        protected readonly IDecompilerEventListener listener;
        protected readonly ConcurrentDictionary<Address, Address> blockStarts;
        protected readonly ConcurrentDictionary<Address, Address> blockEnds;
        protected readonly InstrClass rejectMask;
        private readonly IRewriterHost host;

        protected AbstractScanner(
            Program program,
            ScanResultsV2 sr,
            ProvenanceType provenance,
            IDynamicLinker dynamicLinker,
            IDecompilerEventListener listener,
            IServiceProvider services)
        {
            this.services = services;
            this.program = program;
            this.sr = sr;
            this.Provenance = provenance;
            this.host = new RewriterHost(services, program, dynamicLinker, listener);
            this.listener = listener;
            this.blockStarts = new ConcurrentDictionary<Address, Address>();
            this.blockEnds = new ConcurrentDictionary<Address, Address>();

            this.rejectMask = program.User.Heuristics.Contains(ScannerHeuristics.Unlikely)
                ? InstrClass.Unlikely
                : 0;
            this.rejectMask |= program.User.Heuristics.Contains(ScannerHeuristics.UserMode)
                ? InstrClass.Privileged
                : 0;
        }

        protected ProvenanceType Provenance { get; }


        /// <summary>
        /// If true, allow speculative transfers to nonexisting addresses.
        /// </summary>
        public abstract bool AllowSpeculativeTransfers { get; }

        /// <summary>
        /// Returns true if the given address is inside an executable
        /// area of the loaded program.
        /// <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>Whether or not the address is executable.</returns>
        public bool IsExecutableAddress(Address addr) =>
            program.Memory.IsExecutableAddress(addr);

        /// <summary>
        /// Returns true if the given address is inside any part
        /// of the loaded program.
        /// <see cref="ImageSegment"/>.
        /// </summary>
        /// <param name="addr">Address to test.</param>
        /// <returns>Whether or not the address is executable.</returns>
        /// <remarks>
        public bool IsValidAddress(Address addr) =>
            program.Memory.IsValidAddress(addr);

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
            if (!program.TryCreateImageReader(arch, addr, out var rdr))
                return Array.Empty<RtlInstructionCluster>();
            var rw = arch.CreateRewriter(rdr, state, binder, host);
            return rw;
        }

        /// <summary>
        /// Marks the presence of a chunk of data of type <paramref name="dt"/> at
        /// address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">The address at which the data is located.</param>
        /// <param name="dt">The <see cref="DataType"/> of the data.</param>
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
        /// <returns>A new <see cref="RtlBlock"/>.</returns>
        public RtlBlock RegisterBlock(
            IProcessorArchitecture arch,
            Address addrBlock,
            long length,
            Address addrFallthrough,
            List<RtlInstructionCluster> instrs)
        {
            var id = program.NamingPolicy.BlockName(addrBlock);
            var block = RtlBlock.Create(arch, addrBlock, id, (int)length, addrFallthrough, this.Provenance, instrs);
            var success = sr.Blocks.TryAdd(addrBlock, block);
            Debug.Assert(success, $"Failed registering block at {addrBlock}");
            return block;
        }

        public void RegisterEdge(Edge edge)
        {
            List<Address>? edges;
            while (!sr.Successors.TryGetValue(edge.From, out edges))
            {
                // The most common successor cardinality is 2.
                edges = new List<Address>(2);
                if (sr.Successors.TryAdd(edge.From, edges))
                {
                    edges.Add(edge.To);
                    return;
                }
            }
            //$TODO: make this concurrent safe.
            edges.Add(edge.To);
        }

        /// <summary>
        /// Registers an invalid block starting at <paramref name="addrInvalidBlockStart"/>.
        /// </summary>
        /// <param name="addrInvalidBlockStart">Address at which the block starts.</param>
        public void RegisterInvalidBlock(Address addrInvalidBlockStart)
        {
            sr.InvalidBlocks.TryAdd(addrInvalidBlockStart, addrInvalidBlockStart);
        }

        public ScanResultsV2 RegisterPredecessors()
        {
            sr.Predecessors.Clear();
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

        public bool TryRegisterTrampoline(
            Address addrFinalGoto,
            List<RtlInstructionCluster> trampolineStub,
            [MaybeNullWhen(false)] out Trampoline trampoline)
        {
            // Has already been determined to be a stub?
            if (this.sr.TrampolineStubEnds.TryGetValue(addrFinalGoto, out trampoline))
                return true;
            // Check if it is a stub.
            trampoline = program.Platform.GetTrampolineDestination(addrFinalGoto, trampolineStub, host);
            if (trampoline is null)
                return false;
            // Here starts a race with other threads. Attempt to register this trampoline.
            if (sr.TrampolineStubEnds.TryAdd(addrFinalGoto, trampoline))
            {
                sr.TrampolineStubStarts.TryAdd(trampoline.StubAddress, trampoline);
            }
            else
            {
                // Someone else beat us to it; fetch the value
                trampoline = sr.TrampolineStubEnds[addrFinalGoto];
            }
            return true;
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
        private RtlBlock Chop(RtlBlock block, int iStart, int iEnd, long blockSize)
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
            return RtlBlock.Create(block.Architecture, addr, id, (int)blockSize, addrFallthrough, Provenance, instrs);
        }

        public ExpressionSimplifier CreateEvaluator(ProcessorState state)
        {
            return new ExpressionSimplifier(
                program.Memory,
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
                   "{0:X8}-{1:X8} ({2}){3}: {4}{5}",
                       b.Address,
                       b.FallThrough,
                       b.Length,
                       b.IsValid ? "" : " - Invalid",
                       RenderType(b.Instructions[^1].Class),
                       RenderSuccessors(sr, b))));

            static string RenderSuccessors(ScanResultsV2 sr, RtlBlock b)
            {
                if (!sr.Successors.TryGetValue(b.Address, out var succ) ||
                    succ.Count == 0)
                    return "";
                return " " + string.Join(", ", succ);
            }

            static string RenderType(InstrClass t)
            {
                if ((t & InstrClass.Invalid) != 0)
                    return "???";
                if ((t & InstrClass.Zero) != 0)
                    return "Zer";
                if ((t & InstrClass.Padding) != 0)
                    return "Pad";
                if ((t & InstrClass.Call) != 0)
                    return "Cal";
                if ((t & InstrClass.ConditionalTransfer) == InstrClass.ConditionalTransfer)
                    return "Bra";
                if ((t & InstrClass.Transfer) != 0)
                    return "End";
                if ((t & InstrClass.Terminates) != 0)
                    return "Trm";
                return "Lin";
            }
        }

        private class RewriterHost : IRewriterHost
        {
            private readonly IServiceProvider services;
            private readonly Program program;
            private readonly IDynamicLinker dynamicLinker;
            private readonly IDecompilerEventListener listener;

            public RewriterHost(
                IServiceProvider services,
                Program program,
                IDynamicLinker dynamicLinker,
                IDecompilerEventListener listener)
            {
                this.services = services;
                this.program = program;
                this.dynamicLinker = dynamicLinker;
                this.listener = listener;
            }

            public Constant? GlobalRegisterValue => program.GlobalRegisterValue;

            public IProcessorArchitecture GetArchitecture(string archMoniker)
            {
                var cfgSvc = services.RequireService<IConfigurationService>();
                return program.EnsureArchitecture(archMoniker, cfgSvc.GetArchitecture!);
            }

            public Expression? GetImport(Address addrImportThunk, Address addrInstruction)
            {
                if (program.ImportReferences.TryGetValue(addrImportThunk, out var impref))
                {
                    var global = impref.ResolveImport(
                        dynamicLinker,
                        program.Platform,
                        new ProgramAddress(program, addrInstruction),
                        this.listener);
                    return global;
                }
                return null;
            }

            /// <summary>
            /// If <paramref name="addrImportThunk"/> is the known address of 
            /// an import thunk / trampoline, return the imported function as
            /// an <see cref="ExternalProcedure"/>. Otherwise, check to see if
            /// the call is an intercepted call.
            /// </summary>
            /// <param name="addrImportThunk"></param>
            /// <param name="addrInstruction">Used to display diagnostics.</param>
            /// <returns></returns>
            public ExternalProcedure? GetImportedProcedure(
                IProcessorArchitecture arch,
                Address addrImportThunk,
                Address addrInstruction)
            {
                if (program.ImportReferences.TryGetValue(addrImportThunk, out var impref))
                {
                    var extProc = impref.ResolveImportedProcedure(
                        dynamicLinker,
                        program.Platform,
                        new ProgramAddress(program, addrInstruction),
                        this.listener);
                    return extProc;
                }

                if (program.InterceptedCalls.TryGetValue(addrImportThunk, out var ep))
                    return ep;
                return GetInterceptedCall(arch, addrImportThunk);
            }

            public ExternalProcedure? GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                if (!program.TryCreateImageReader(arch, addrImportThunk, out var rdr))
                    return null;
                //$REVIEW: WHOA! This is 32-bit code!
                if (!rdr.TryReadUInt32(out var uDest))
                    return null;
                var addrDest = Address.Ptr32(uDest);
                program.InterceptedCalls.TryGetValue(addrDest, out var ep);
                return ep;
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
            {
                return arch.TryRead(program.Memory, addr, dt, out value!);
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
