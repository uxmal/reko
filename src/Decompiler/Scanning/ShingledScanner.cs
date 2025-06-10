#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
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
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This scanner disassembles all possible instruction locations of 
    /// an image, and discards instructions that (transitively) result 
    /// in conflicts.
    /// </summary>
    /// <remarks>
    /// Inspired by the paper "Shingled Graph Disassembly:
    /// Finding the Undecidable Path" by Richard Wartell, Yan Zhou, 
    /// Kevin W.Hamlen, and Murat Kantarcioglu.
    /// </remarks>
    public class ShingledScanner
    {
        private const byte MaybeCode = 1;
        private const byte Data = 0;

        private const InstrClass L = InstrClass.Linear;
        private const InstrClass T = InstrClass.Transfer;
        
        private const InstrClass CL = InstrClass.Linear | InstrClass.Conditional;
        private const InstrClass CT = InstrClass.Transfer | InstrClass.Conditional;
        
        private const InstrClass DT = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass DCT = InstrClass.Transfer | InstrClass.Conditional | InstrClass.Delay;

        public readonly Address Bad;

        private readonly Program program;
        private readonly ScanResults sr;
        private readonly IRewriterHost host;
        private readonly IStorageBinder storageBinder;
        private readonly IDecompilerEventListener eventListener;

        public ShingledScanner(Program program, IRewriterHost host, IStorageBinder storageBinder, ScanResults sr, IDecompilerEventListener eventListener)
        {
            this.program = program;
            this.host = host;
            this.storageBinder = storageBinder;
            this.sr = sr;
            this.eventListener = eventListener;
            this.Bad = program.Platform.MakeAddressFromLinear(~0ul, false);
        }

        /// <summary>
        /// Performs a shingle scan of the executable segments,
        /// returning a list of addresses to probable functions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Address,int>> Scan()
        {
            try
            {
                var map = ScanExecutableSegments();
                return SpeculateCallDests(map);
            }
            catch (Exception ex)
            {
                Debug.Print("Error: {0}", ex.Message);
                return new KeyValuePair<Address, int>[0];
            }
        }

        /// <summary>
        /// Performs a shingle scan of the executable segments of the program,
        /// returning the interprocedural control flow graph (ICFG) and locations
        /// of likely call destinations.
        /// </summary>
        /// <returns></returns>
        public ScanResults ScanNew()
        {
            ICodeLocation? location = null;
            Exception error;
            try
            {
                var q = ScanExecutableSegments();
            }
            catch (AddressCorrelatedException aex)
            {
                location = eventListener.CreateAddressNavigator(program, aex.Address);
                error = aex;
            }
            catch (Exception ex)
            {
                location = new NullCodeLocation("");
                error = ex;
            }
            if (location is not null)
            {
                eventListener.Error(location, "An error occurred while scanning {0}.");
            }
            return new ScanResults
            {
                ICFG = new DiGraph<RtlBlock>(),
                DirectlyCalledAddresses = this.sr.DirectlyCalledAddresses,
                KnownProcedures = sr.KnownProcedures,
            };
        }

        public Dictionary<ImageSegment, byte[]> ScanExecutableSegments()
        {
            var map = new Dictionary<ImageSegment, byte[]>();
            var workToDo = program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable)
                .Select(s => s.Size)
                .Aggregate(0ul, (s, n) => s + n);

            var arch = program.Architecture;
            foreach (var segment in program.SegmentMap.Segments.Values
                .Where(s => s.IsExecutable))
            {
                var sc = ScanRange(new Chunk(arch, segment.MemoryArea, segment.Address, segment.Size), workToDo);
                map.Add(segment, sc);
            }
            return map;
        }

        /// <summary>
        /// Disassemble every byte of a range of addresses, marking those 
        /// addresses that likely are code as MaybeCode, everything else as
        /// data. Simultaneously, the graph G of cross references is built
        /// up.
        /// </summary>
        /// <remarks>
        /// The plan is to disassemble every location of the range, building
        /// a reverse control graph. Any jump to an illegal address or any
        /// invalid instruction will result in an edge from "bad" to that 
        /// instruction.
        /// </remarks>
        /// <param name="segment"></param>
        /// <returns>An array of bytes classifying each byte as code or data.
        /// </returns>
        public byte[] ScanRange(in Chunk chunk, ulong workToDo)
        {
            var (arch, mem, addrStart, cbAlloc) = chunk;
            Debug.Assert(arch is not null);
            const int ProgressBarUpdateInterval = 256 * 1024;

            var y = new byte[cbAlloc];
            // Advance by the instruction granularity.
            var step = arch.InstructionBitSize / arch.MemoryGranularity;
            
            // Align the start address to instruction granularity. If we align off 
            // into invalid memory, return immediately.
            addrStart = addrStart.Align(step);
            if (!program.SegmentMap.IsExecutableAddress(addrStart))
                return y;

            var delaySlot = InstrClass.None;
            var rewriterCache = new Dictionary<Address, IEnumerator<RtlInstructionCluster>>();
            var instrCount = 0;

            for (var a = 0; a < y.Length; a += step)
            {
                y[a] = MaybeCode;
                var addr = addrStart + a;
                var dasm = GetRewriter(arch, addr, rewriterCache);
                if (!dasm.MoveNext())
                {
                    sr.Invalid.Add(addr);
                    AddEdge(Bad, addr);
                    continue;
                }
                var i = dasm.Current;
                if (IsInvalid(mem, i))
                {
                    sr.Invalid.Add(addr);
                    AddEdge(Bad, i.Address);
                    i.Class = InstrClass.Invalid;
                    AddInstruction(arch, i);
                    delaySlot = InstrClass.None;
                    y[a] = Data;
                }
                else
                {
                    if (MayFallThrough(i))
                    {
                        if ((delaySlot & DT) != DT)
                        {
                            if (a + i.Length < y.Length)
                            {
                                // Still inside the segment.
                                AddEdge(i.Address + i.Length, i.Address);
                            }
                            else
                            {
                                // Fell off segment, i must be a bad instruction.
                                AddEdge(Bad, i.Address);
                                i.Class = InstrClass.Invalid;
                                AddInstruction(arch, i);
                                y[a] = Data;
                            }
                        }
                    }
                    if ((i.Class & InstrClass.Transfer) != 0)
                    {
                        var aa = DestinationAddress(i);
                        if (aa is not null)
                        {
                            var addrDest = aa.Value;
                            if (IsExecutable(addrDest))
                            {
                                // call / jump destination is executable
                                if ((i.Class & InstrClass.Call) != 0)
                                {
                                    // Don't add edges to other procedures.
                                    if (!this.sr.DirectlyCalledAddresses.TryGetValue(addrDest, out int callTally))
                                        callTally = 0;
                                    this.sr.DirectlyCalledAddresses[addrDest] = callTally + 1;
                                }
                                else
                                {
                                    AddEdge(addrDest, i.Address);
                                }
                            }
                            else
                            {
                                // Jump to data / hyperspace.
                                AddEdge(Bad, i.Address);
                                i.Class = InstrClass.Invalid;
                                AddInstruction(arch, i);
                                y[a] = Data;
                            }
                        }
                        else
                        {
                            if ((i.Class & InstrClass.Call) != 0)
                            {
                                this.sr.IndirectCalls.Add(i.Address);
                            }
                            else if ((i.Class & InstrClass.Return) == 0)
                            {
                                this.sr.IndirectJumps.Add(i.Address);
                            }
                        }
                    }

                    // If this is a delayed unconditional branch...
                    delaySlot = i.Class;
                }

                if (y[a] == MaybeCode)
                {
                    AddInstruction(arch, i);
                }
                SaveRewriter(addr + i.Length, dasm, rewriterCache);
                if (++instrCount >= ProgressBarUpdateInterval)
                {
                    instrCount = 0;
                    eventListener.Progress.ShowProgress("Shingle scanning", sr.FlatInstructions.Count, (int) workToDo);
                }
            }
            return y;
        }

        private void AddInstruction(IProcessorArchitecture arch, RtlInstructionCluster i)
        {
            sr.FlatInstructions.Add(i.Address.ToLinear(), new ScanResults.Instr
            {
                Architecture = arch,
                block_id = i.Address,
                rtl = i,
                pred = 0,
                succ = 0,
            });
        }


        public void AddEdge(Address from, Address to)
        {
            if (from == Bad)
                return;
            sr.FlatEdges.Add(new ScanResults.Link(to, from));
        }

        // Remove blocks that fall off the end of the segment
        // or into data.
        public HashSet<Address> RemoveBadInstructionsFromGraph()
        {
            // Use only for debugging the bad paths.
            //var d = Dijkstra<Address>.ShortestPath(G, bad, (u, v) => 1.0);
            //Action<Address> DumpPath = (addr) =>
            //{
            //    Debug.Print("Path from {0}", addr);
            //    var path = d.GetPath(addr);
            //    path.Reverse();
            //    foreach (var a in path)
            //    {
            //        Debug.Print("  {0}", a);
            //    }
            //};
            //DumpPath(Address.SegPtr(0x0800, 0));


            // Find all places that are reachable from "bad" addresses.
            // By transitivity, they must also be be bad.
            var deadNodes = new HashSet<Address>();
            //foreach (var a in new DfsIterator<Address>(G).PreOrder(Bad))
            //{
            //    if (a != Bad)
            //    {
            //        deadNodes.Add(a);
            //    }
            //}

            var oldinstrs = sr.Instructions;
            sr.Instructions = oldinstrs
                .Where(o => !deadNodes.Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);

            var oldDirectCalls = sr.DirectlyCalledAddresses;
            sr.DirectlyCalledAddresses = oldDirectCalls
                .Where(o => !deadNodes.Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);

            return deadNodes;
        }

        /// <summary>
        /// Get a rewriter for the specified address. If a rewriter is already
        /// available from the rewriter pool, remove it from the pool and use it,
        /// otherwise create a new one.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private IEnumerator<RtlInstructionCluster> GetRewriter(
            IProcessorArchitecture arch,
            Address addr, 
            IDictionary<Address, IEnumerator<RtlInstructionCluster>> pool)
        {
            if (!pool.Remove(addr, out var e))
            {
                if (!program.TryCreateImageReader(arch, addr, out var rdr))
                    return Array.Empty<RtlInstructionCluster>().AsEnumerable().GetEnumerator();
                var rw = arch.CreateRewriter(
                    rdr,
                    arch.CreateProcessorState(),
                    storageBinder,
                    this.host);
                return rw.GetEnumerator();
            }
            else
            {
                return e;
            }
        }

        private static void SaveRewriter(
            Address addr,
            IEnumerator<RtlInstructionCluster> e,
            IDictionary<Address, IEnumerator<RtlInstructionCluster>> pool)
        {
            if (!pool.ContainsKey(addr))
            {
                pool.Add(addr, e);
            }
        }

        public DiGraph<RtlBlock> BuildIcfg(HashSet<Address> deadNodes)
        {
            throw new NotImplementedException();
            //var icb = BuildBlocks(G);
            //BuildEdges(icb);
            //sr.ICFG = icb.Blocks;
            //return sr.ICFG;
        }

        /// <summary>
        /// Build Shingle blocks from the graph. An instruction can only be 
        /// in one block at a time, so at each point in the graph where the 
        /// successors > 1 or the predecessors > 1, we create a new node.
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public IcfgBuilder BuildBlocks(DiGraph<Address> graph)
        {
            // Remember, the graph is backwards!
            var allBlocks = new DiGraph<RtlBlock>();
            var edges = new List<(RtlBlock, Address)>();
            var mpBlocks = new Dictionary<Address, RtlBlock>();
            var wl = sr.Instructions.Keys.ToSortedSet();

            while (wl.Count > 0)
            {
                var addr = wl.First();
                wl.Remove(addr);

                var instr = sr.Instructions[addr];
                var label = program.NamingPolicy.BlockName(addr);
                var block = RtlBlock.CreateEmpty(program.Architecture, addr, label);
                block.Instructions.Add(instr);
                allBlocks.AddNode(block);
                mpBlocks.Add(addr, block);
                bool endBlockNow = false;
                bool terminateDeferred = false;
                bool addFallthroughEdge = false;
                bool addFallthroughEdgeDeferred = false;
                for (;;)
                {
                    var addrInstrEnd = instr.Address + instr.Length;
                    if ((instr.Class & DT) != 0)
                    {
                        if (MayFallThrough(instr))
                        {
                            addFallthroughEdge = (instr.Class & DT) == T;
                            addFallthroughEdgeDeferred = (instr.Class & DT) == DT;
                        }
                        var addrDst = DestinationAddress(instr);
                        if (addrDst is not null && (instr.Class & InstrClass.Call) == 0)
                        {
                            edges.Add((block, addrDst.Value));
                        }

                        if ((instr.Class & DT) == DT)
                        {
                            terminateDeferred = true;
                        }
                        else
                        {
                            endBlockNow = true;
                        }
                    }
                    else if (instr.Class == InstrClass.Terminates)
                    {
                        endBlockNow = true;
                        addFallthroughEdge = false;
                    }
                    else
                    {
                        endBlockNow = terminateDeferred;
                        addFallthroughEdge = addFallthroughEdgeDeferred;
                        addFallthroughEdgeDeferred = false;
                    }

                    if (sr.DirectlyCalledAddresses.Keys.Contains(addrInstrEnd) ||
                        sr.KnownProcedures.Contains(addrInstrEnd))
                    {
                        // If control falls into what looks like a procedure, don't
                        // add an edge.
                        addFallthroughEdge = false;
                        endBlockNow = true;
                    }
                    if (addFallthroughEdge)
                    {
                        edges.Add((block, addrInstrEnd));
                    }

                    if (endBlockNow || 
                        !wl.Contains(addrInstrEnd) ||
                        !graph.Nodes.Contains(addrInstrEnd) ||
                        graph.Successors(addrInstrEnd).Count != 1)
                    {
                        
                        //Debug.Print("addr: {0}, end {1}, term: {2}, wl: {3}, nodes: {4}, succ: {5}",
                        //    addr,
                        //    addrInstrEnd,
                        //    endBlockNow,
                        //    !wl.Contains(addrInstrEnd),
                        //    !graph.Nodes.Contains(addrInstrEnd),
                        //    graph.Nodes.Contains(addrInstrEnd)
                        //        ? graph.Successors(addrInstrEnd).Count
                        //        : 0);
                        
                        if (!endBlockNow && !addFallthroughEdge)
                        {
                            edges.Add((block, addrInstrEnd));
                        }
                        break;
                    }

                    wl.Remove(addrInstrEnd);
                    instr = sr.Instructions[addrInstrEnd];
                    block.Instructions.Add(instr);
                    endBlockNow = terminateDeferred;
                }
            }
            return new IcfgBuilder(edges, mpBlocks, allBlocks);
        }

        private void BuildEdges(IcfgBuilder icb)
        {
            foreach (var edge in icb.Edges)
            {
                var from = edge.Item1;
                if (!icb.AddrToBlock.TryGetValue(edge.Item2, out var to))
                {
                    continue;
                }
                if (!sr.KnownProcedures.Contains(edge.Item2) &&
                    !sr.DirectlyCalledAddresses.ContainsKey(edge.Item2))
                {
                    icb.Blocks.AddEdge(from, to);
                }
            }
        }

        private static bool IsInvalid(MemoryArea mem, RtlInstructionCluster instr)
        {
            if (instr.Class == InstrClass.Invalid)
                return true;
            // If an instruction straddles a relocation, it can't be 
            // a real instruction.
            if (mem.Relocations.Overlaps(instr.Address, (uint)instr.Length))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if this function might continue to the next instruction.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static bool MayFallThrough(RtlInstructionCluster i)
        {
            return 
                (i.Class & InstrClass.Terminates) == 0
                &&
                (i.Class &
                  (InstrClass.Linear 
                   | InstrClass.Conditional 
                   | InstrClass.Call)) != 0;        //$REVIEW: what if you call a terminating function?
        }

        /// <summary>
        /// Scans through each address in each segment to find things that
        /// look like pointers. If these pointers point into a valid segment,
        /// increment the tally for that address.
        /// </summary>
        /// <returns>A dictionary mapping segments to their pointer tallies.</returns>
        public Dictionary<ImageSegment, int[]> GetPossiblePointerTargets()
        {
            var targetMap = program.SegmentMap.Segments.ToDictionary(
                s => s.Value, 
                s => new int[s.Value.Size]);
            foreach (var seg in program.SegmentMap.Segments.Values)
            {
                foreach (var pointer in GetPossiblePointers(seg))
                {
                    if (program.SegmentMap.TryFindSegment(pointer, out var segPointee) &&
                        segPointee.IsInRange(pointer))
                    {
                        int segOffset = (int)(pointer - segPointee.Address);
                        int hits = targetMap[segPointee][segOffset];
                        targetMap[segPointee][segOffset] = hits + 1;

                        if (!this.sr.DirectlyCalledAddresses.TryGetValue(pointer, out hits))
                            hits = 0;
                        this.sr.DirectlyCalledAddresses[pointer] = hits + 1;
                    }
                }
            }
            return targetMap;
        }

        /// <summary>
        /// For each location in the segment, read a pointer-sized chunk and return it.
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        public IEnumerable<Address> GetPossiblePointers(ImageSegment seg)
        {
            //$TODO: this assumes pointers must be aligned. Not the case for older machines.
            var arch = program.Architecture;
            if (!program.TryCreateImageReader(arch, seg.Address, out var rdr))
                yield break;
            while (rdr.TryRead(program.Platform.PointerType, out Constant? c))
            {
                var addr = program.Architecture.MakeAddressFromConstant(c, false);
                if (addr is { })
                    yield return addr;
            }
        }

        private bool IsExecutable(Address address)
        {
            if (!program.SegmentMap.TryFindSegment(address, out ImageSegment? seg))
                return false;
            return seg.IsExecutable;
        }

        /// <summary>
        /// Find the constant destination of a transfer instruction.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Address? DestinationAddress(RtlInstructionCluster i)
        {
            var rtl = i.Instructions[^1];
            for (;;)
            {
                if (rtl is not RtlIf rif)
                    break;
                rtl = rif.Instruction;
            }
            if (rtl is RtlTransfer xfer)
            {
                return xfer.Target as Address?;
            }
            return null;
        }

        public IEnumerable<KeyValuePair<Address, int>> SpeculateCallDests(IDictionary<ImageSegment, byte[]> map)
        {
            var addrs = 
                from addr in this.sr.DirectlyCalledAddresses
                orderby addr.Value descending
                where IsPossibleExecutableCodeDestination(addr.Key, map)
                select addr;
            return addrs;
        }

        private bool IsPossibleExecutableCodeDestination(
            Address addr, 
            IDictionary<ImageSegment, byte[]> map)
        {
            if (!program.SegmentMap.TryFindSegment(addr, out ImageSegment? seg))
                throw new InvalidOperationException(string.Format("Address {0} doesn't belong to any segment.", addr));
            return map[seg][addr - seg.Address] == MaybeCode;
        }

        [Conditional("DEBUG")]
        public void Dump(string caption)
        {
            return;     // This is horribly verbose, so only use it when debugging unit tests.
#if VERBOSE
            Debug.Print("== {0} =====================", caption);
            Debug.Print("{0} nodes", G.Nodes.Count);
            foreach (var block in G.Nodes.OrderBy(n => n))
            {
                Debug.Print("{0}:  //  pred: {1}",
                    block,
                    string.Join(" ", G.Successors(block)
                        .OrderBy(n => n)));
                RtlInstructionCluster cluster;
                if (!sr.Instructions.TryGetValue(block, out cluster))
                {
                    Debug.Print("  *****");
                }
                else
                { 
                    Debug.Print("  {0}", cluster);
                    foreach (var instr in cluster.Instructions)
                    {
                        Debug.Print("    {0}", instr);
                    }
                }
                Debug.Print("  // succ: {0}", string.Join(" ", G.Predecessors(block)
                    .OrderBy(n => n)));
            }
#endif
        }
    }
}
