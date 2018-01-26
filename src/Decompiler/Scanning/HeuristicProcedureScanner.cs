﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Given a heuristically discovered procedure, attempt to discard as 
    /// many basic blocks as possible.
    /// </summary>
    public class HeuristicProcedureScanner
    {
        private Program program;
        private IRewriterHost host;
        private ScanResults sr;
        private DiGraph<RtlBlock> blocks;
        private Func<Address, bool> isAddressValid;
        private HashSet<Tuple<RtlBlock, RtlBlock>> conflicts;

        public HeuristicProcedureScanner(
            Program program, 
            ScanResults sr,
            Func<Address,bool> isAddressValid,
            IRewriterHost host)
        {
            this.program = program;
            this.host = host;
            this.sr = sr;
            this.blocks = sr.ICFG;
            this.isAddressValid = isAddressValid;
            this.conflicts = BuildConflictGraph(blocks.Nodes);
        }

        public void ResolveBlockConflicts(IEnumerable<Address> procedureStarts)
        {
            var reachable = TraceReachableBlocks(procedureStarts);
            ComputeStatistics(reachable);
            this.sr.Dump("Before conflict resolution");
            RemoveBlocksEndingWithInvalidInstruction();
            this.sr.Dump("After invalid instruction eliminiation");
            RemoveBlocksConflictingWithValidBlocks(reachable);
            this.sr.Dump("After conflicting block removal");
            RemoveParentsOfConflictingBlocks();
            this.sr.Dump("After parents of conflicting blocks removed");
            //RemoveBlocksWithFewPredecessors();
            //DumpGraph();
            RemoveBlocksWithFewSuccessors();
            this.sr.Dump("After few successor removal");
            RemoveConflictsRandomly();
        }

        /// <summary>
        /// Resolve all block conflicts.
        /// </summary>
        /// <param name="blocks"></param>
        public void BlockConflictResolution(Address addrProcedureStart)
        {
            var valid = TraceReachableBlocks(new[] { addrProcedureStart });
            ComputeStatistics(valid);
            this.sr.Dump();
            RemoveBlocksEndingWithInvalidInstruction();
            this.sr.Dump();
            RemoveBlocksConflictingWithValidBlocks(valid);
            this.sr.Dump();
            RemoveParentsOfConflictingBlocks();
            this.sr.Dump();
            RemoveBlocksWithFewPredecessors();
            this.sr.Dump();
            RemoveBlocksWithFewSuccessors();
            this.sr.Dump();
            RemoveConflictsRandomly();
        }

        /// <summary>
        /// Trace the reachable blocks using DFS; call them 'reachable'.
        /// </summary>
        /// <returns>A set of blocks considered "valid".</returns>
        private HashSet<RtlBlock> TraceReachableBlocks(IEnumerable<Address> procstarts)
        {
            var reachable = new HashSet<RtlBlock>();
            var mpAddrToBlock = blocks.Nodes.ToDictionary(k => k.Address);
            foreach (var addrProcStart in procstarts)
            {
                if (mpAddrToBlock.TryGetValue(addrProcStart,out var entry))
                {
                    var r = new DfsIterator<RtlBlock>(blocks).PreOrder(entry).ToHashSet();
                    reachable.UnionWith(r);
                }
            }
            return reachable;
        }

        /// <summary>
        /// Given a set of the provably valid basic blocks in the program,
        /// create a five-level deep trie of instructions. Blocks that haven't
        /// been proved valid, but which starting with such instructions are
        /// likely to be valid.
        /// </summary>
        /// <param name="valid"></param>
        private void ComputeStatistics(ISet<RtlBlock> valid)
        {
            var cmp = program.Architecture.CreateInstructionComparer(Normalize.Constants);
            if (cmp == null)
                return;
            //$REVIEW: to what use can we put this?
            var trie = new Trie<MachineInstruction>(cmp);
            foreach (var item in valid.OrderBy(i => i.Address))
            {
                var dasm = program.CreateDisassembler(item.Address);
                var instrs = dasm.Take(5);
                trie.Add(instrs.ToArray());
            }
            trie.Dump();
        }

        /// <summary>
        /// Given a list of blocks, creates an undirected graph of all blocks which overlap.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        public static HashSet<Tuple<RtlBlock, RtlBlock>> BuildConflictGraph(IEnumerable<RtlBlock> blocks)
        {
            var conflicts = new HashSet<Tuple<RtlBlock, RtlBlock>>(new CollisionComparer());
            // Find all conflicting blocks: pairs that overlap.
            var blockMap = blocks.OrderBy(n => n.Address).ToList();
            for (int i = 0; i < blockMap.Count; ++i)
            {
                var u = blockMap[i];
                var uEnd = u.GetEndAddress();
                for (int j = i + 1; j < blockMap.Count; ++j)
                {
                    var v = blockMap[j];
                    if (v.Address >= uEnd)
                        break;
                    conflicts.Add(Tuple.Create(u, v));
                }
            }
            return conflicts;
        }

        private void RemoveBlocksEndingWithInvalidInstruction()
        {
            foreach (var n in blocks.Nodes.Where(n => !n.IsValid).ToList())
            {
                RemoveBlockFromGraph(n);
            }
        }

        /// <summary>
        /// Any node that is in conflict with a valid node must be removed.
        /// </summary>
        /// <param name="valid"></param>
        private void RemoveBlocksConflictingWithValidBlocks(HashSet<RtlBlock> valid)
        {
            // `nodes` are all blocks that weren't reachable by DFS.
            var nodes = blocks.Nodes.Where(nn => !valid.Contains(nn)).ToHashSet();
            foreach (var cc in
                (from c in conflicts
                 where nodes.Contains(c.Item1) && valid.Contains(c.Item2)
                 select c.Item1))
            {
                nodes.Remove(cc);
                RemoveBlockFromGraph(cc);
            }
            foreach (var cc in 
                (from c in conflicts
                 where nodes.Contains(c.Item2) && valid.Contains(c.Item1)
                 select c.Item2))
            {
                nodes.Remove(cc);
                RemoveBlockFromGraph(cc);
            }
        }

        private void RemoveBlockFromGraph(RtlBlock n)
        {
            //Debug.Print("Removing block: {0}", n.Address);
            blocks.Nodes.Remove(n);
        }

        private void RemoveConflictsRandomly()
        {
            // foreach (conflict u, v)
            //    pick u, v randomly and remove it.
            foreach (var conflict in conflicts.Where(c => Remaining(c)))
            {
                if (blocks.Nodes.Contains(conflict.Item1) &&
                    blocks.Nodes.Contains(conflict.Item2))
                {
                    RemoveBlockFromGraph(conflict.Item2);
                }
            }
        }

        private void RemoveBlocksWithFewSuccessors()
        {
            // foreach (conflict (u, v)
            //    if (u.succ.Count < v.succ.Count)
            //      remove u
            //    else if (u.succ.Count > v.succ.count)
            //      remove v
            foreach (var conflict in conflicts.Where(c => Remaining(c)))
            {
                if (blocks.Nodes.Contains(conflict.Item1) &&
                    blocks.Nodes.Contains(conflict.Item2))
                {
                    var uCount = blocks.Successors(conflict.Item1).Count;
                    var vCount = blocks.Successors(conflict.Item2).Count;
                    if (uCount < vCount)
                        RemoveBlockFromGraph(conflict.Item1);
                }
            }
        }

        private void RemoveBlocksWithFewPredecessors()
        {
            conflicts = BuildConflictGraph(blocks.Nodes);
            //    if (u.pred.Count < v.pred.Count)
            //      remove u
            //    else if (u.pred.Count > v.pred.count)
            //      remove v
            foreach (var conflict in conflicts.Where(c => Remaining(c)))
            {
                var uCount = blocks.Predecessors(conflict.Item1).Count;
                var vCount = blocks.Predecessors(conflict.Item2).Count;
                if (uCount < vCount)
                    RemoveBlockFromGraph(conflict.Item1);
            }
        }

        private bool Remaining(Tuple<RtlBlock, RtlBlock> c)
        {
            var nodes = blocks.Nodes;
            return 
                nodes.Contains(c.Item1) &&
                nodes.Contains(c.Item2);
        }
        
        public ISet<RtlBlock> GetAncestors(RtlBlock n)
        {
            var anc = new HashSet<RtlBlock>();
            foreach (var p in blocks.Predecessors(n))
            {
                GetAncestorsAux(p, n, anc);
            }
            return anc;
        }

        private ISet<RtlBlock> GetAncestorsAux(
            RtlBlock n, 
            RtlBlock orig, 
            ISet<RtlBlock> ancestors)
        {
            if (ancestors.Contains(n) || n == orig)
                return ancestors;
            ancestors.Add(n);
            foreach (var p in blocks.Predecessors(n))
            {
                GetAncestorsAux(p, orig, ancestors);
            }
            return ancestors;
        }

        private void RemoveParentsOfConflictingBlocks()
        {
            // for all conflicting (u,v)
            //    for all common_parents p
            //        remove p.
            foreach (var conflict in conflicts.Where(c => Remaining(c)))
            {
                var uParents = GetAncestors(conflict.Item1);
                var vParents = GetAncestors(conflict.Item2);
                foreach (var uP in uParents)
                    if (vParents.Contains(uP))
                        RemoveBlockFromGraph(uP);
            }
        }

        private IEnumerable<Tuple<Address, Address>> GetGaps()
        {
            var blockMap = blocks.Nodes.OrderBy(n => n.Address).ToList();
            var addrLastEnd = blockMap[0].Address;
            foreach (var b in blockMap)
            {
                if (addrLastEnd < b.Address)
                    yield return Tuple.Create(addrLastEnd, b.Address);
                addrLastEnd = b.GetEndAddress();
            }
        }

        /// <summary>
        /// The task of the gap completion phase is to improve the
        /// results of our analysis by filling the gaps between basic
        /// blocks in the control flow graph with instructions that
        /// are likely to be valid.
        /// When all possible instruction sequences are determined,
        /// the one with the highest sequence score is selected as the
        /// valid instruction sequence between b1 and b2.
        /// </summary>
        public void GapResolution()
        {
            foreach (var gap in GetGaps())
            {
                int bestScore = 0;
                Tuple<Address, Address> bestSequence = null;
                foreach (var sequence in GetValidSequences(gap))
                {
                    int score = ScoreSequence(sequence);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestSequence = sequence;
                    }
                }
            }
        }

        /// <summary>
        /// Find all possibly valid sequences in an address range. A necessary
        /// condition for a valid instruction sequence is that its last 
        /// instruction either (i) ends with the last byte of the gap or 
        /// (ii) its last instruction is a non intra-procedural control 
        /// transfer instruction.
        /// </summary>
        private IEnumerable<Tuple<Address,Address>> GetValidSequences(Tuple<Address, Address> gap)
        {
            int instrByteGranularity = program.Architecture.InstructionBitSize / 8;
            for (Address addr = gap.Item1; addr < gap.Item2; addr = addr + instrByteGranularity)
            {
                var addrStart = addr;
                var dasm = CreateRewriter(addr);
                bool isValid = false;
                foreach (var instr in dasm)
                {
                    var addrNext = instr.Address + instr.Length;
                    if (addrNext > gap.Item2)
                        break;
                    if (addrNext.ToLinear() == gap.Item2.ToLinear())
                    {
                        // Falls out of the gap
                        isValid = true;
                        break;
                    }
                    if (NonLocalTransferInstruction(instr))
                    {
                        isValid = true;
                        break;
                    }
                }
                if (isValid)
                    yield return Tuple.Create(addrStart, addr);
            }
        }

        /// <summary>
        /// The sequence
        /// score is a measure of the likelihood that this instruction
        /// sequence appears in an executable. It is calculated
        /// as the sum of the instruction scores of all instructions
        /// in the sequence. The instruction score is similar to
        /// the sequence score and reflects the likelihood of an individual
        /// instruction. Instruction scores are always greater
        /// or equal than zero. Therefore, the score of a sequence
        /// cannot decrease when more instructions are added. We
        /// calculate instruction scores using statistical techniques
        /// and heuristics to identify improbable instructions.
        /// </summary>    
        private int ScoreSequence(Tuple<Address,Address> sequence)
        {
            return 0;
        }

        private IEnumerable<RtlInstructionCluster> CreateRewriter(Address addr)
        {
            var rw = program.Architecture.CreateRewriter(
                program.CreateImageReader(addr),
                program.Architecture.CreateProcessorState(),
                program.Architecture.CreateFrame(),
                host);
            return new RobustRewriter(rw, program.Architecture.InstructionBitSize / 8);
        }

        private bool NonLocalTransferInstruction(RtlInstructionCluster cluster)
        {
            if (cluster.Class == RtlClass.Linear)
                return false;
            var last = cluster.Instructions.Last();
            if (last is RtlCall)
                return true;
            if (last is RtlGoto rtlGoto && rtlGoto.Target is Address target)
            {
                return !isAddressValid(target);
            }
            return true;
        }

        // Block conflict resolution

        /// <summary>
        /// Collects all the ancestors of a node 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <param name="node"></param>
        /// <param name="visited"></param>
        public void GetAllAncestors<T>(DirectedGraph<T> graph, T node, HashSet<T> visited)
        {
            foreach (var p in graph.Predecessors(node))
            {
                if (!visited.Contains(p))
                {
                    visited.Add(p);
                    GetAllAncestors(graph, p, visited);
                }
            }
        }

        private class CollisionComparer : IEqualityComparer<Tuple<RtlBlock, RtlBlock>>
        {
            public bool Equals(Tuple<RtlBlock, RtlBlock> x, Tuple<RtlBlock, RtlBlock> y)
            {
                return x.Item1 == y.Item1 && x.Item2 == y.Item2 ||
                       x.Item1 == y.Item2 && x.Item2 == y.Item1;
            }

            public int GetHashCode(Tuple<RtlBlock, RtlBlock> obj)
            {
                return obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
            }
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
