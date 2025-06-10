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
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// In the absence of any other information, scans address ranges in 
    /// search of code sequences that may represent valid procedures. Reko
    /// needs help from the processor architecture to specify what byte 
    /// patterns to look for.
    /// </summary>
    /// <remarks>
    /// Static Disassembly of Obfuscated Binaries
    /// Christopher Kruegel, William Robertson, Fredrik Valeur and Giovanni Vigna
    /// Reliable Software Group
    /// University of California Santa Barbara
    /// {chris,wkr,fredrik,vigna}@cs.ucsb.edu
    /// </remarks>
    public class HeuristicScanner // : IScanner
    {
        private readonly Program program;
        private readonly IRewriterHost host;
        private readonly IDecompilerEventListener eventListener;
        private readonly IStorageBinder storageBinder;
        private readonly RtlBlock invalidBlock;
        private readonly IStorageBinder binder;

        public HeuristicScanner(
            IServiceProvider services,
            Program program, 
            IRewriterHost host, 
            IDecompilerEventListener eventListener)
        {
            this.Services = services;
            this.program = program;
            this.host = host;
            this.storageBinder = program.Architecture.CreateFrame();
            this.eventListener = eventListener;
            this.invalidBlock = RtlBlock.CreateEmpty(null!, default, "<invalid>");
            this.binder = program.Architecture.CreateFrame();
        }

        public IServiceProvider Services { get; }

        public ScanResults? ScanImage(ScanResults sr)
        {
            // At this point, we have some entries in the image map
            // that are data, and unscanned ranges in betweeen. We
            // have hopefully a bunch of procedure addresses to
            // break up the unscanned ranges.

            var ranges = FindUnscannedRanges();

            var stopwatch = new Stopwatch();
            var shsc = new ShingledScanner(program, host, storageBinder, sr, eventListener);
            bool unscanned = false;
            foreach (var range in ranges)
            {
                unscanned = true;
                try
                {
                    shsc.ScanRange(new(
                        program.Architecture,
                        range.Item1,
                        range.Item2,
                        range.Item3),
                        range.Item3);
                }
                catch (AddressCorrelatedException aex)
                {
                    host.Error(aex.Address, aex.Message);
                }
            }
            if (!unscanned)
            {
                // No unscanned blocks were found.
                return null;
            }
            // Remove blocks that fall off the end of the segment
            // or into data.
            Probe(sr);
            shsc.Dump("After shingle scan graph built");
            var deadNodes = shsc.RemoveBadInstructionsFromGraph();
            shsc.BuildIcfg(deadNodes);
            Probe(sr);
            sr.Dump("After shingle scan");

            ScanResultsV2 sr2 = MakeResults(sr);

            // On processors with variable length instructions,
            // there may be many blocks that partially overlap the 
            // "real" blocks that would actually have been executed
            // by the processor. Starting with known "roots", try to
            // remove as many invalid blocks as possible.

            var hsc = new BlockConflictResolver(
                program,
                sr2,
                program.Memory.IsValidAddress,
                host);
            RemoveInvalidBlocks(sr);
            Probe(sr);
            hsc.ResolveBlockConflicts(sr.KnownProcedures.Concat(sr.DirectlyCalledAddresses.Keys));
            Probe(sr);
            sr.Dump("After block conflict resolution");
            var pd = new ProcedureDetector(sr2, this.eventListener);
            var procs = pd.DetectProcedures();
            sr.Procedures = procs;
            return sr;
        }

        private ScanResultsV2 MakeResults(ScanResults sr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this procedure as a handy place to put 
        /// breakpoints when debugging large binaries.
        /// </summary>
        /// <param name="sr"></param>
        [Conditional("DEBUG")]
        private void Probe(ScanResults sr)
        {
        }

        private static void RemoveInvalidBlocks(ScanResults sr)
        {
            var revGraph = new DiGraph<RtlBlock>();
            var invalid = RtlBlock.CreateEmpty(null!, default, "<invalid>");
            revGraph.AddNode(invalid);
            foreach (var b in sr.ICFG.Nodes)
            {
                revGraph.AddNode(b);
            }
            foreach (var b in sr.ICFG.Nodes)
            {
                foreach (var s in sr.ICFG.Successors(b))
                {
                    revGraph.AddEdge(s, b);
                }
                if (!b.IsValid)
                {
                    revGraph.AddEdge(invalid, b);
                }
            }

            // Find the transitive closure of invalid nodes.

            var invalidNodes = new DfsIterator<RtlBlock>(revGraph)
                .PreOrder(invalid)
                .ToList();
            foreach (var n in invalidNodes.Where(nn => nn != invalid))
            {
                sr.ICFG.RemoveNode(n);
                sr.DirectlyCalledAddresses.Remove(n.Address);
                // Debug.Print("Removed invalid node {0}", n.Address);  // commented out as this becomes very verbose.
            }
        }

        /// <summary>
        /// Scans the Program object looking for address ranges that have not
        /// been identified as code/data yet.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(MemoryArea, Address, uint)> FindUnscannedRanges()
        {
            return this.program.ImageMap.Items
                .Where(de => IsExecutable(de))
                .Select(de => CreateUnscannedArea(de));
        }

        private bool IsExecutable(KeyValuePair<Address,ImageMapItem> de)
        {
            return
                de.Value.DataType is UnknownType &&
                this.program.Memory.IsExecutableAddress(de.Key);
        }

        private (MemoryArea, Address, uint) CreateUnscannedArea(KeyValuePair<Address, ImageMapItem> de)
        {
            this.program.SegmentMap.TryFindSegment(de.Key, out var seg);
            return (seg!.MemoryArea,
                de.Key,
                de.Value.Size);
        }

        /// <summary>
        /// Looks for byte patterns that look like procedure entries.
        /// </summary>
        /// <param name="addrBegin"></param>
        /// <param name="addrEnd"></param>
        /// <returns></returns>
        public IEnumerable<Address> FindPossibleProcedureEntries(
            ByteMemoryArea mem,
            Address addrBegin,
            Address addrEnd)
        {
            var h = program.Platform.Heuristics;
            if (h.ProcedurePrologs is null || h.ProcedurePrologs.Length == 0)
                return [];

            byte[]? pattern = h.ProcedurePrologs[0].Bytes;
            if (pattern is not null)
            {
                var search = new AhoCorasickSearch<byte>(new[] { pattern }, true, true);
                return search.GetMatchPositions(mem.Bytes)
                    .Select(i => mem.BaseAddress + i);
            }
            else
            {
                return Array.Empty<Address>();
            }
        }

        /// <summary>
        /// Determines the locations of all instructions in a segment
        /// that perform a  CALL / JSR / BL to a _known_ procedure 
        /// address.
        /// </summary>
        /// <param name="knownProcedureAddresses">A sequence of addresses
        /// that are known to be procedures.</param>
        /// <returns>A sequence of linear addresses where those call 
        /// instructions are.</returns>
        public IEnumerable<Address> FindCallOpcodes(ByteMemoryArea mem, IEnumerable<Address> knownProcedureAddresses)
        {
            return program.Architecture.CreatePointerScanner(
                program.SegmentMap,
                program.Architecture.CreateImageReader(mem, 0),
                knownProcedureAddresses,
                PointerScannerFlags.Calls);
        }

        /// <summary>
        /// Heuristically locates previously unscanned functions in the image. 
        /// If all fails, assume the whole range is a function.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<Address, Address>> FindPossibleFunctions(
            IEnumerable<Tuple<ByteMemoryArea, Address, Address>> ranges)
        {
            foreach (var range in ranges)
            {
                var possibleEntries = FindPossibleProcedureEntries(range.Item1, range.Item2, range.Item3)
                    .Concat(program.EntryPoints.Keys)
                    .Concat(program.ImageSymbols.Values
                        .Where(s => s.Type == SymbolType.Procedure)
                        .Select(s => s.Address!))
                    .ToSortedSet();
                var e = possibleEntries.GetEnumerator();
                Address aEnd = range.Item2;
                if (e.MoveNext())
                {
                    aEnd = e.Current;
                    while (e.MoveNext())
                    {
                        var aStart = aEnd;
                        aEnd = e.Current;
                        yield return Tuple.Create(aStart, aEnd);
                    }
                    yield return Tuple.Create(aEnd, range.Item3);
                }
            }
        }

        /// <summary>
        /// Heuristically disassembles a range of addresses between 
        /// <paramref name="addrStart"/> and <paramref name="addrEnd"/>. 
        /// </summary>
        /// <param name="addrStart"></param>
        /// <param name="addrEnd"></param>
        /// <returns></returns>
        public HeuristicProcedure DisassembleProcedure(Address addrStart, Address addrEnd)
        {
            var proc = new HeuristicProcedure(
                addrStart,
                addrEnd,
                program.Architecture.CreateFrame());
            var sr = new ScanResults
            {
                ICFG = proc.Cfg,
                DirectlyCalledAddresses = new Dictionary<Address, int>(),
            };
            var dasm = new HeuristicDisassembler(
                program, 
                binder,
                sr,
                proc.IsValidAddress,
                true,
                host);
            int instrByteGranularity = program.Architecture.InstructionBitSize / program.Architecture.MemoryGranularity;
            for (Address addr = addrStart; addr < addrEnd; addr += instrByteGranularity)
            {
                dasm.Disassemble(addr);
            }
            DumpBlocks(proc.Cfg.Nodes);
            return proc;
        }

        // Partition memory into chunks betweeen each candidate.
        // Decode each possible instruction at each possible address, yielding a list of potential instructions.
        // Identify intra procedural xfers:
        //   - target is in this chunk.
        //   - conditional jmp.
        // HeuristicFunction will hve
        //   - start address
        //   - end address
        // To find all of these, scan the all the potential_instructions, if any of them are a GOTO or a RtlBranch.
        //   if found, add to <Set>jump_candidates
        // Now use scanner to build initial CFG
        // feed scanner with fn start and all jump_candidates
        // this may yield dupes and broken blocks.

        // SpuriousNodes: how to get rid of.

        // it is possible
        //to have instructions in the initial call graph that overlap.
        //In this case, two different basic blocks in the call graph
        //can contain overlapping instructions starting at slightly
        //different addresses. When following a sequence of instructions,
        //the disassembler can arrive at an instruction
        //that is already part of a previously found basic block. In
        //the regular case, this instruction is the first instruction of
        //the existing block. The disassembler can complete the
        //instruction sequence of the current block and create a
        //link to the existing basic block in the control flow graph

        [Conditional("DEBUG")]
        private void DumpBlocks(IEnumerable<RtlBlock>? blocks)
        {
            if (blocks is null)
                return;
            foreach (var block in blocks.OrderBy(b => b.Address.ToLinear()))
            {
                var addrEnd = block.GetEndAddress();
                var sb = new StringBuilder();
                if (!program.TryCreateImageReader(program.Architecture, block.Address, out var rdr))
                    continue;
                sb.AppendFormat("{0} - {1} ", block.Address, addrEnd);
                while (rdr.Address < addrEnd)
                {
                    sb.AppendFormat("{0:X2} ", (int)rdr.ReadByte());
                }
                Debug.Print(sb.ToString());
            }
        }

        // IScanner interface.

            /*
        void IScannerQueue.Warn(Address addr, string message)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(program, addr), message);
        }

        void IScannerQueue.Warn(Address addr, string message, params object[] args)
        {
            eventListener.Warn(eventListener.CreateAddressNavigator(program, addr), message, args);
        }

        void IScannerQueue.Error(Address addr, string message, params object[] args)
        {
            eventListener.Error(eventListener.CreateAddressNavigator(program, addr), message);
        }

        ProcedureBase IScanner.ScanProcedure(Address addr, string procedureName, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        void IScanner.TerminateBlock(Block block, Address addrEnd)
        {
            throw new NotImplementedException();
        }

        Block IScanner.FindContainingBlock(Address addr)
        {
            throw new NotImplementedException();
        }

        Block IScanner.FindExactBlock(Address addr)
        {
            throw new NotImplementedException();
        }

        Block IScanner.SplitBlock(Block block, Address addr)
        {
            throw new NotImplementedException();
        }

        Block IScanner.CreateCallRetThunk(Address addrFrom, Procedure procOld, Procedure procNew)
        {
            throw new NotImplementedException();
        }

        void IScanner.SetProcedureReturnAddressBytes(Procedure proc, int returnAddressBytes, Address address)
        {
            throw new NotImplementedException();
        }

        IEnumerable<RtlInstructionCluster> IScanner.GetTrace(Address addrStart, ProcessorState state, IStorageBinder binder)
        {
            throw new NotImplementedException();
        }

        void IScanner.ScanImage()
        {
            throw new NotImplementedException();
        }

        void IScannerQueue.EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            throw new NotImplementedException();
        }

        void IScannerQueue.EnqueueProcedure(Address addr)
        {
            throw new NotImplementedException();
        }

        Block IScanner.EnqueueJumpTarget(Address addrSrc, Address addrDst, Procedure proc, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        Address IScanner.EnqueueUserProcedure(Procedure_v1 sp)
        {
            throw new NotImplementedException();
        }

        void IScannerQueue.EnqueueUserProcedure(Address addr, FunctionType sig, string name)
        {
            throw new NotImplementedException();
        }

        void IScannerQueue.EnqueueUserGlobalData(Address addr, DataType dt, string name)
        {
            throw new NotImplementedException();
        }

        void IScanner.ScanImageSymbol(Program program, ImageSymbol sym, bool isEntryPoint)
        {
            throw new NotImplementedException();
        }

        ExternalProcedure IScanner.GetImportedProcedure(Address addrImportThunk, Address addrInstruction)
        {
            throw new NotImplementedException();
        }
        */
    }
}