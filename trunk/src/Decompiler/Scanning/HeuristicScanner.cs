#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning
{
	/// <summary>
	/// In the absence of any other information, scans address ranges in 
    /// search of code sequences that may represent valid procedures. It
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
        private Program prog;
        private IRewriterHost host;
        private Dictionary<Address, HeuristicBlock> blockMap;

        public HeuristicScanner(Program prog, IRewriterHost host)
        {
            this.prog = prog;
            this.host = host;
        }

        /// <summary>
        /// Determines the locations of all instructions that perform a 
        /// CALL / JSR / BL to a _known_ procedure address.
        /// </summary>
        /// <param name="knownProcedureAddresses">A sequence of addresses
        /// that are known to be procedures.</param>
        /// <returns>A sequence of linear addresses where those call 
        /// instructions are.</returns>
        public IEnumerable<Address> FindCallOpcodes(IEnumerable<Address> knownProcedureAddresses)
        {
            return prog.Architecture.CreatePointerScanner(
                prog.ImageMap,
                prog.Architecture.CreateImageReader(prog.Image, 0),
                knownProcedureAddresses,
                PointerScannerFlags.Calls);
        }

        public IEnumerable<Tuple<Address,Address>> FindUnscannedRanges()
        {
            return prog.ImageMap.Items
                .Where(de => de.Value.DataType is UnknownType)
                .Select(de => Tuple.Create(de.Key, de.Key + de.Value.Size));
        }

        /// <summary>
        /// Heuristically locates previously unscanned functions in the image. 
        /// If all fails, assume the whole range is a function.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<Address, Address>> FindPossibleFunctions(
            IEnumerable<Tuple<Address, Address>> ranges)
        {
            foreach (var range in ranges)
            {
                SortedSet<Address> possibleEntries = FindPossibleProcedureEntries(range.Item1, range.Item2)
                    .Concat(prog.EntryPoints.Select(ep => ep.Address))
                    .ToSortedSet();
                var e = possibleEntries.GetEnumerator();
                Address aEnd = range.Item1;
                if (e.MoveNext())
                {
                    aEnd = e.Current;
                    while (e.MoveNext())
                    {
                        var aStart = aEnd;
                        aEnd = e.Current;
                        yield return Tuple.Create(aStart, aEnd);
                    }
                    yield return Tuple.Create(aEnd, range.Item2);
                }
            }
        }

        private IEnumerable<Address> FindPossibleProcedureEntries(Address addrBegin, Address addrEnd)
        {
            var pattern = new byte[] { 0x55, 0x8B, 0xEC };  //$TODO: platform-dependent.
            var search = new AhoCorasickSearch<byte>(new[] { pattern }, true, true);
            return search.GetMatchPositions(prog.Image.Bytes)
                .Select(i => prog.Image.BaseAddress + i);
        }

        // Plan of attack:
        // For each "hole", look for signatures of program entry points.
        // These are procedure entry candidates of .
        // Next scan all executable code segments for:
        //  - calls that reach those candidates
        //  - jmps to those candidates
        //  - pointers to those candidates.
        // Each time we find a call, we increase the score of the candidate.
        // At the end we have a list of scored candidates.
        //
        public HeuristicProcedure DisassembleProcedure(Address addrStart, Address addrEnd)
        {
            blockMap = new Dictionary<Address, HeuristicBlock>();
            var proc = new HeuristicProcedure
            {
                BeginAddress = addrStart,
                EndAddress = addrEnd,
                Frame = prog.Architecture.CreateFrame()
            };
            for (Address addr = addrStart; addr < addrEnd; addr = addr + prog.Architecture.InstructionBitSize / 8)
            {
                HeuristicDisassemble(addr, proc);
            }
            DumpBlocks(proc.Cfg.Nodes);
            return proc;
        }

        private void DumpBlocks(IEnumerable<HeuristicBlock> blocks)
        {
            foreach (var block in blocks.OrderBy(b => b.Address.ToLinear()))
            {
                var addrEnd = block.GetEndAddress();
                var sb = new StringBuilder();
                var rdr = prog.CreateImageReader(block.Address);
                sb.AppendFormat("{0} - {1} ", block.Address, addrEnd);
                while (rdr.Address < addrEnd)
                {
                    sb.AppendFormat("{0:X2} ", (int)rdr.ReadByte());
                }
                Debug.Print(sb.ToString());
            }
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

        private HeuristicBlock SplitBlock(HeuristicBlock block, Address addr, HeuristicProcedure proc)
        {
            var newBlock = new HeuristicBlock(addr, string.Format("l{0:X}", addr));
            proc.Cfg.Nodes.Add(newBlock);
            newBlock.Statements.AddRange(
                block.Statements.Where(r => r.Address >= addr).OrderBy(r => r.Address));
            foreach (var de in blockMap.Where(d => d.Key >= addr && d.Value == block).ToList())
            {
                blockMap[de.Key] = newBlock;
            }
            block.Statements.RemoveAll(r => r.Address >= addr);
            var succs = proc.Cfg.Successors(block).ToArray();
            foreach (var s in succs)
            {
                proc.Cfg.AddEdge(newBlock, s);
                proc.Cfg.RemoveEdge(block, s);
            }
            proc.Cfg.AddEdge(block, newBlock);
            return newBlock;
        }

        /// <summary>
        /// Recursively disassembles the range of addresses specified by the guessed procedure.
        /// <paramref name="proc"/>.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public HeuristicBlock HeuristicDisassemble(Address addr, HeuristicProcedure proc)
        {
            var current = new HeuristicBlock(addr, string.Format("l{0:X}", addr));
            var rAddr = prog.Architecture.CreateRewriter(
                     prog.CreateImageReader(addr),
                     prog.Architecture.CreateProcessorState(),
                     proc.Frame,
                     host);
            foreach (var rtl in rAddr.TakeWhile(r => r.Address < proc.EndAddress))
            {
                HeuristicBlock block;
                if (blockMap.TryGetValue(rtl.Address, out block))
                {
                    // This instruction was already disassembled before.
                    if (rtl.Address.ToLinear() != block.Address.ToLinear())
                    {
                        block = SplitBlock(block, rtl.Address, proc);
                    }
                    if (current.Statements.Count == 0)
                    {
                        // Coincides exactly, return the old block.
                        return block;
                    }
                    else
                    {
                        // Fell into 'block' while disassembling
                        // 'current'. Create a fall-though edge
                        if (!proc.Cfg.Nodes.Contains(current))
                        {
                            proc.Cfg.Nodes.Add(current);
                        }
                        proc.Cfg.AddEdge(current, block);
                        return current;
                    }
                }
                else
                {
                    // Fresh instruction
                    if (!proc.Cfg.Nodes.Contains(current))
                    {
                        proc.Cfg.Nodes.Add(current);
                    }
                    current.Statements.Add(rtl);
                    blockMap.Add(rtl.Address, current);
                    var rtlLast = rtl.Instructions.Last();
                    if (rtlLast is RtlCall || rtlLast is RtlReturn)
                    {
                        // Since calls cannot be dependent on to return, 
                        // we stop disassembling.
                        return current;
                    }
                    var rtlJump = rtlLast as RtlGoto;
                    if (rtlJump != null)
                    {
                        var target = rtlJump.Target as Address;
                        if (target == null ||
                            target < proc.BeginAddress ||
                            target >= proc.EndAddress)
                        {
                            // Stop disassembling if you get outside
                            // the procedure or a computed goto.
                            return current;
                        }
                        block = HeuristicDisassemble(target, proc);
                        proc.Cfg.AddEdge(current, block);
                        return current;
                    }
                    var rtlBranch = rtlLast as RtlBranch;
                    if (rtlBranch != null)
                    {
                        block = HeuristicDisassemble(rtlBranch.Target, proc);
                        proc.Cfg.AddEdge(current, block);
                        block = HeuristicDisassemble(rtl.Address + rtl.Length, proc);
                        proc.Cfg.AddEdge(current, block);
                        return current;
                    }
                }
            }
            return current;
        }
    }
}
