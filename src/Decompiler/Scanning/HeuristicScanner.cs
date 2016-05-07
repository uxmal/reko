#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
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
    public class HeuristicScanner
    {
        private Program program;
        private IRewriterHost host;
        private DecompilerEventListener eventListener;

        public HeuristicScanner(Program program, IRewriterHost host, DecompilerEventListener eventListener)
        {
            this.program = program;
            this.host = host;
            this.eventListener = eventListener;
        }

        /// Plan of attack:
        /// In each unscanned "hole", look for signatures of procedure entries.
        /// These are procedure entry candidates. 
        /// Scan each of the procedure entry candidates heuristically.
        /// 
        /// Next scan all executable code segments for:
        ///  - calls that reach those candidates
        ///  - jmps to those candidates
        ///  - pointers to those candidates.
        /// Each time we find a call, we increase the score of the candidate.
        /// At the end we have a list of scored candidates.
        public void ScanImageHeuristically()
        {
            var sw = new Stopwatch();
            sw.Start();
            var list = new List<HeuristicBlock>();
            var ranges = FindUnscannedRanges();
            var fnRanges = FindPossibleFunctions(ranges).ToList();
            int n = 0;
            foreach (var range in fnRanges)
            {
                var hproc = DisassembleProcedure(range.Item1, range.Item2);
                var hps = new HeuristicProcedureScanner(program, hproc, host);
                hps.BlockConflictResolution();
                DumpBlocks(hproc.Cfg.Nodes);
                hps.GapResolution();
                // TODO: add all guessed code to image map -- clearly labelled.
                AddBlocks(hproc);
                list.AddRange(hproc.Cfg.Nodes);
                eventListener.ShowProgress("Estimating procedures", n, fnRanges.Count);
                ++n;
            }
            eventListener.Warn(
                new Reko.Core.Services.NullCodeLocation("Heuristics"),
                string.Format("Scanned image in {0} seconds, finding {1} blocks.",
                    sw.Elapsed.TotalSeconds, list.Count));
            list.ToString();
        }

        private void AddBlocks(HeuristicProcedure hproc)
        {
            var proc = Procedure.Create(hproc.BeginAddress, hproc.Frame);
            foreach (var block in hproc.Cfg.Nodes.Where(bb => bb.Instructions.Count > 0))
            {
                var last = block.Instructions.Last();
                var b = new Block(proc, "l" + block.Address);
                if (program.ImageMap.Items.ContainsKey(block.Address))
                    continue;
                program.ImageMap.AddItemWithSize(
                    block.Address,
                    new ImageMapBlock
                    {
                        Block = b,
                        Address = block.Address,
                        Size = (uint)(last.Address - block.Address) + (uint)last.Length
                    });
            }
        }

        /// <summary>
        /// Scans the Program object looking for address ranges that have not
        /// been identified as code/data yet.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<MemoryArea, Address, Address>> FindUnscannedRanges()
        {
#if !NOT_USE_WHOLE_IMAGE

            return program.ImageMap.Items
                .Where(de => de.Value.DataType is UnknownType)
                .Select(de => Tuple.Create(this.program.SegmentMap.Segments[de.Key].MemoryArea, de.Key, de.Key + de.Value.Size));
#else
            return program.SegmentMap.Segments.Values
                .Where(s => (s.Access & AccessMode.Execute) != 0)
                .Select(s => Tuple.Create(s.MemoryArea, s.Address, s.Address + s.ContentSize));
#endif
        }

        /// <summary>
        /// Looks for byte patterns that look like procedure entries.
        /// </summary>
        /// <param name="addrBegin"></param>
        /// <param name="addrEnd"></param>
        /// <returns></returns>
        public IEnumerable<Address> FindPossibleProcedureEntries(MemoryArea mem, Address addrBegin, Address addrEnd)
        {
            var h = program.Platform.Heuristics;
            if (h.ProcedurePrologs == null || h.ProcedurePrologs.Length == 0)
                return new Address[0];

            byte[] pattern = h.ProcedurePrologs[0].Bytes;
            var search = new AhoCorasickSearch<byte>(new[] { pattern }, true, true);
            return search.GetMatchPositions(mem.Bytes)
                .Select(i => mem.BaseAddress + i);
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
        public IEnumerable<Address> FindCallOpcodes(MemoryArea mem, IEnumerable<Address> knownProcedureAddresses)
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
            IEnumerable<Tuple<MemoryArea, Address, Address>> ranges)
        {
            foreach (var range in ranges)
            {
                var possibleEntries = FindPossibleProcedureEntries(range.Item1, range.Item2, range.Item3)
                    .Concat(program.EntryPoints.Keys)
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
        /// Heuristically disassembles a procedure that has been assumed to 
        /// be located between <paramref name="addrStart"/> and <paramref name="addrEnd"/>. 
        /// </summary>
        /// <param name="addrStart"></param>
        /// <param name="addrEnd"></param>
        /// <returns></returns>
        public HeuristicProcedure DisassembleProcedure(Address addrStart, Address addrEnd)
        {
            var proc = new HeuristicProcedure
            {
                BeginAddress = addrStart,
                EndAddress = addrEnd,
                Frame = program.Architecture.CreateFrame()
            };
            var dasm = new HeuristicDisassembler(program, proc, host);
            int instrByteGranularity = program.Architecture.InstructionBitSize / 8;
            for (Address addr = addrStart; addr < addrEnd; addr = addr + instrByteGranularity)
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
        private void DumpBlocks(IEnumerable<HeuristicBlock> blocks)
        {
            if (blocks != null)
                return;
            foreach (var block in blocks.OrderBy(b => b.Address.ToLinear()))
            {
                var addrEnd = block.GetEndAddress();
                var sb = new StringBuilder();
                var rdr = program.CreateImageReader(block.Address);
                sb.AppendFormat("{0} - {1} ", block.Address, addrEnd);
                while (rdr.Address < addrEnd)
                {
                    sb.AppendFormat("{0:X2} ", (int)rdr.ReadByte());
                }
                Debug.Print(sb.ToString());
            }
        }
    }
}