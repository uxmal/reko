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

namespace Decompiler.Scanning
{
	/// <summary>
	/// In the absence of any other information, scans address ranges in search of code sequences that may represent
	/// valid procedures. It needs help from the processor architecture to specify what byte patterns to look for.
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

        public HeuristicScanner(Program prog)
        {
            this.prog = prog;
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

        public IEnumerable<Tuple<Address, Address>> FindPossibleFunctions()
        {
            foreach (var range in FindUnscannedRanges())
            {
                SortedSet<Address> possibleEntries = FindPossibleProcedureEntries(range.Item1, range.Item2)
                    .Concat(prog.EntryPoints.Select(ep => ep.Address))
                    .ToSortedSet();
                var e = possibleEntries.GetEnumerator();
                if (!e.MoveNext())
                    continue;
                var aEnd = e.Current;
                while (e.MoveNext())
                {
                    var aStart = aEnd;
                    aEnd = e.Current;
                    yield return Tuple.Create(aStart, aEnd);
                }
            }
        }

        private IEnumerable<Address> FindPossibleProcedureEntries(Address addrBegin, Address addrEnd)
        {
            throw new NotImplementedException();
        }

        /// Plan of attack:
        // For each "hole", look for signatures of program entry points.
        // These are procedure entry candidates of .
        // Next scan all executable code segments for:
        //  - calls that reach those candidates
        //  - jmps to those candidates
        //  - pointers to those candidates.
        // Each time we find a call, we increase the score of the candidate.
        // At the end we have a list of scored candidates.
        

        public void DisassembleProcedure(Address addrStart, Address addrEnd)
        {
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

        public class HeuristicBlock
        {
            public List<RtlInstructionCluster> Statements;
            private HeuristicProcedure proc;
            private string name;

            public HeuristicBlock(HeuristicProcedure proc, string name)
            {
                this.proc = proc;
                this.name = name;
            }

            public Address Address { get; set; }

            public Address GetEndAddress()
            {
                throw new NotImplementedException();
            }
        }

        public class HeuristicProcedure
        {
            public DirectedGraphImpl<HeuristicBlock> Cfg = new DirectedGraphImpl<HeuristicBlock>();
            public Frame Frame;

            public Address BeginAddress;
            public Address EndAddress;
        }

        private HeuristicBlock SplitBlock(HeuristicBlock block, Address addr, HeuristicProcedure proc)
        {
            var newBlock = new HeuristicBlock(proc, string.Format("l{0:X}", addr.ToLinear()));
            newBlock.Statements.AddRange(
                block.Statements.Where(r => r.Address >= addr).OrderBy(r => r.Address));
            var succs = proc.Cfg.Successors(block).ToArray();
            foreach (var s in succs)
            {
                proc.Cfg.AddEdge(newBlock, s);
                proc.Cfg.RemoveEdge(block, s);
            }
            proc.Cfg.AddEdge(newBlock, block);
            return newBlock;
        }

        public HeuristicBlock HeuristicDisassemble(Address addr, HeuristicProcedure proc)
        {
            var blockMap = new Dictionary<Address, HeuristicBlock>();

            var current = new HeuristicBlock(proc, string.Format("l{0:X}", addr));

            var rAddr = prog.Architecture.CreateRewriter(
                     prog.Architecture.CreateImageReader(prog.Image, addr),
                     prog.Architecture.CreateProcessorState(),
                     proc.Frame,
                     null);
            foreach (var rtl in rAddr.TakeWhile(r => r.Address < proc.EndAddress))
            {
                HeuristicBlock block;
                if (blockMap.TryGetValue(rtl.Address, out block))
                {
                    if (rtl.Address != block.Address)
                    {
                        block = SplitBlock(block, rtl.Address, proc);
                    }
                    if (current.Statements.Count == 0)
                    {
                        return block;
                    }
                    else
                    {
                        proc.Cfg.AddEdge(current, block);
                        return current;
                    }
                }
                else
                {
                    current.Statements.Add(rtl);
                    blockMap.Add(rtl.Address, current);
                        var rtlLast = rtl.Instructions.Last();
                    if (rtlLast is RtlCall)
                        return current;
                    var rtlJump  = rtlLast as RtlGoto;
                    if (rtlJump != null)
                    {
                        var target = rtlJump.Target as Address;
                        if (target == null ||
                            target < proc.BeginAddress ||
                            target >= proc.EndAddress)
                            return current;
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

        // Block conflict resolution

        public void GetAllAncestors<T>(DirectedGraph<T> graph, T node, HashSet<T> visited)
        {
            visited.Add(node);
            foreach (var p in graph.Predecessors(node))
            {
                if (!visited.Contains(p))
                    GetAllAncestors(graph, p, visited);
            }
        }

        private class CollisionComparer : IEqualityComparer<Tuple<HeuristicBlock, HeuristicBlock>>
        {
        public bool Equals(Tuple<HeuristicBlock,HeuristicBlock> x, Tuple<HeuristicBlock,HeuristicBlock> y)
        {
 	        return x.Item1 == y.Item1 && x.Item2 == y.Item2 ||
                   x.Item1 == y.Item2 && x.Item2 == y.Item1;
        }

        public int GetHashCode(Tuple<HeuristicBlock,HeuristicBlock> obj)
        {
            return obj.Item1.GetHashCode() ^obj.Item2.GetHashCode();
        }
        }

        public void BlockConflictResolution(DirectedGraph<HeuristicBlock> blocks)
        {
            // First trace the reachable blocks using DFS
            var valid = new DfsIterator<HeuristicBlock>(blocks).PreOrder().ToHashSet();

            // Find all conflicting blocks: pairs that overlap, 
            var blockMap = blocks.Nodes.ToSortedList(n => n.Address);
            var conflicts = new HashSet<Tuple<HeuristicBlock, HeuristicBlock>>(new CollisionComparer());
            for (int i = 0; i < blockMap.Count; ++i)
            {
                var u = blockMap.Values[i];
                var uEnd = u.GetEndAddress();
                for (int j = i + 1; j < blockMap.Count; ++j)
                {
                    var v = blockMap.Values[j];
                    if (v.Address >= uEnd)
                        break;
                    conflicts.Add(Tuple.Create(u, v));
                }
            }

            // Any node that is in conflict with a valid node must be removed.
            var deleted = new HashSet<HeuristicBlock>();
            foreach (var n in blocks.Nodes)
            {
                if (!valid.Contains(n))
                {
                    foreach (var v in valid)
                    {
                        if (conflicts.Contains(Tuple.Create(n, v)))
                            blocks.Nodes.Remove(n);
                    }
                }
            }

            // for all conflicting (u,v)
            //    for all common_parents p
            //        remove p.
            deleted = new HashSet<HeuristicBlock>();
            foreach (var conflict in conflicts)
            {
                throw new NotImplementedException();
                //var uParents = cp.GetParents(conflict.Item1);
                //var vParents = cp.GetParents(conflict.Item2);
                //foreach (var uP in uParents)
                //    if (vParents.Contains(uP))
                //        blocks.Nodes.Remove(uP);
            }

            // foreach conflict (u, v)
            //    if (u.pred.Count < v.pred.Count)
            //      remove u
            //    else if (u.pred.Count > v.pred.count)
            //      remove v
            foreach (var conflict in conflicts)
            {
                var uCount = blocks.Predecessors(conflict.Item1).Count;
                var vCount = blocks.Predecessors(conflict.Item2).Count;
                if (uCount < vCount)
                    blocks.Nodes.Remove(conflict.Item1);
            }
            // foreach (conflict (u, v)
            //    if (u.succ.Count < v.succ.Count)
            //      remove u
            //    else if (u.succ.Count > v.succ.count)
            //      remove v
            foreach (var conflict in conflicts)
            {
                if (blocks.Nodes.Contains(conflict.Item1) &&
                    blocks.Nodes.Contains(conflict.Item2))
                {
                    var uCount = blocks.Successors(conflict.Item1).Count;
                    var vCount = blocks.Successors(conflict.Item2).Count;
                    if (uCount < vCount)
                        blocks.Nodes.Remove(conflict.Item1);
                }
            }

            // foreach (conflict u, v)
            //    pick u, v randomly and remove it.
            foreach (var conflict in conflicts)
            {
                if (blocks.Nodes.Contains(conflict.Item1) &&
                    blocks.Nodes.Contains(conflict.Item2))
                {
                    blocks.Nodes.Remove(conflict.Item2);
                }
            }
        }

        private IEnumerable<Tuple<Address, Address>> GetGaps(HeuristicProcedure proc)
        {
            var blockMap = proc.Cfg.Nodes.ToSortedList(n => n.Address);
            var addrLastEnd = blockMap.Values[0].Address;
            foreach (var b in blockMap.Values)
            {
                if (addrLastEnd < b.Address)
                    yield return Tuple.Create(addrLastEnd, b.Address);
                addrLastEnd = b.GetEndAddress();
            }
        }

        public void GapResolution(HeuristicProcedure proc)
        {
            foreach (var gap in GetGaps(proc))
            {
                var scores = new List<Tuple<int, Address>>();
                foreach (var sequence in GetValidSequences(gap))
                {
                    int score = ScoreSequence(sequence);
                    scores.Add(Tuple.Create(score, sequence));
                }
            }
        }

        private int ScoreSequence(Address sequence)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Address> GetValidSequences(Tuple<Address, Address> gap)
        {
            for (Address addr = gap.Item1; addr < gap.Item2; addr = addr + prog.Architecture.InstructionBitSize / 8)
            {
                var block = new HeuristicBlock(null, "@@") { Address = addr };
                var dasm = prog.Architecture.CreateRewriter(
                    prog.Architecture.CreateImageReader(
                        prog.Image, addr),
                    prog.Architecture.CreateProcessorState(),
                    prog.Architecture.CreateFrame(),
                    null);
                bool isValid = false;
                foreach (var instr in dasm)
                {
                    if (instr.Address + instr.Length > gap.Item2)
                        break;
                    var lastInstr = instr.Instructions.Last();
                    if (NonLocalTransferInstruction())
                    {
                        isValid = true;
                        break;
                    }
                }
                if (isValid)
                    yield return addr;
            }
        }

        private bool NonLocalTransferInstruction()
        {
            throw new NotImplementedException();
        }


        // Gap resolution

        // find all valid instruction sequences
        //   valid: if (last_sequence) ends at gap.end
        //   or : last instr is non-intra-procedure control
        //
// Instruction sequences are found by considering each
//byte between the start and the end of the gap as a potential
//start of a valid instruction sequence. Subsequent
//instructions are then decoded until the instruction sequence
//either meets or violates one of the necessary conditions
//defined above. When an instruction sequence
//meets a necessary condition, it is considered possibly
//valid and a sequence score is calculated for it. The sequence
//score is a measure of the likelihood that this instruction
//sequence appears in an executable. It is calculated
//as the sum of the instruction scores of all instructions
//in the sequence. The instruction score is similar to
//the sequence score and reflects the likelihood of an individual
//instruction. Instruction scores are always greater
//or equal than zero. Therefore, the score of a sequence
//cannot decrease when more instructions are added. We
//calculate instruction scores using statistical techniques
//and heuristics to identify improbable instructions

//        calculate instruction scores using statistical techniques
//and heuristics to identify improbable instructions.
//The statistical techniques are based on instruction probabilities
//and digraphs. Our approach utilizes tables that
//denote both the likelihood of individual instructions appearing
//in a binary as well as the likelihood of two instructions
//occurring as a consecutive pair. The tables
//were built by disassembling a large set of common executables
//and tabulating counts for the occurrence of each
//individual instruction as well as counts for each occurrence
//of a pair of instructions. These counts were subsequently
//stored for later use during the disassembly of
//an obfuscated binary. It is important to note that only instruction
//opcodes are taken into account with this technique;
//operands are not considered. The basic score
//for a particular instruction is calculated as the sum of
//the probability of occurrence of this instruction and the
//probability of occurrence of this instruction followed by
//the next instruction in the sequence.

//In addition to the statistical technique, a set of heuristics
//are used to identify improbable instructions. This
//analysis focuses on instruction arguments and observed
//notions of the validity of certain combinations of operations,
//registers, and accessing modes. Each heuristic is
//applied to an individual instruction and can modify the
//basic score calculated by the statistical technique. In our
//current implementation, the score of the corresponding
//instruction is set to zero whenever a rule matches. Examples
//of these rules include the following:
//• operand size mismatches;
//• certain arithmetic on special-purpose registers;
//• unexpected register-to-registermoves (e.g., moving
//from a register other than %ebp into %esp);
//• moves of a register value into memory referenced
//by the same register.

//        When all possible instruction sequences are determined,
//the one with the highest sequence score is selected as the
//valid instruction sequence between b1 and b2.
    }
}
