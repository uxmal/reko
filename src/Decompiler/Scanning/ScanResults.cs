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

#nullable disable

using Reko.Core;
using Reko.Core.Graphs;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// A data class containing the accumulated knowledge garnered during
    /// Reko's scanning phase.
    /// </summary>
    public class ScanResults
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="ScanResults"/> class.
        /// </summary>
        public ScanResults()
        {
            this.TransferTargets = new HashSet<Address>();
            this.DirectlyCalledAddresses = new Dictionary<Address, int>();
            this.Instructions = new Dictionary<Address, RtlInstructionCluster>();
            this.FlatInstructions = new Dictionary<ulong, ScanResults.Instr>();
            this.FlatEdges = new List<Link>();
            this.IndirectCalls = new HashSet<Address>();
            this.IndirectJumps = new HashSet<Address>();
            this.Procedures = new List<RtlProcedure>();
            this.Invalid = new HashSet<Address>();
            this.WatchedAddresses = new HashSet<Address>();
        }

        /// <summary>
        /// All the discovered machine instructions, rewritten into RTL
        /// instruction clusters.
        /// </summary>
        public Dictionary<Address, RtlInstructionCluster> Instructions { get; set; }

        /// <summary>
        /// Interprocedural control flow graph, consisting of all
        /// direct calls and jumps. Each edge goes from a jump or a call
        /// to its destination. Branches have two destinations.
        /// </summary>
        public DiGraph<RtlBlock> ICFG { get; set; }

        /// <summary>
        /// Tally of how many times each address is called by a direct call
        /// instruction.
        /// </summary>
        public Dictionary<Address, int> DirectlyCalledAddresses { get; set; }

        /// <summary>
        /// Addresses that are targets (destinations) of jumps or calls.
        /// </summary>
        public HashSet<Address> TransferTargets { get; set; }

        /// <summary>
        /// These are addresses that are known, because metadata in the
        /// executable image describes them as such, or because the user
        /// specified the address.
        /// </summary>
        public Dictionary<Address, ImageSymbol> KnownAddresses { get; set; }

        /// <summary>
        /// More specifically, these are addresses that are known to be
        /// function entry points because they are called from other 
        /// instructions.
        /// </summary>
        public HashSet<Address> KnownProcedures { get; set; }

        /// <summary>
        /// These are procedures, either because they were called,
        /// or because they were jumped to from different procedures.
        /// This is a key end result of the scanning stage.
        /// </summary>
        public List<RtlProcedure> Procedures { get; set; }

        /// <summary>
        /// All the instructions in the program, addressed by their
        /// linear address.
        /// </summary>
        public Dictionary<ulong, Instr> FlatInstructions { get; set; }

        /// <summary>
        /// All the edges in the interprocedural control flow graph,
        /// </summary>
        public List<Link> FlatEdges { get; set; }

        /// <summary>
        /// All the places that were identified as padding.
        /// </summary>
        public List<RtlBlock> RemovedPadding { get; set; }

        /// <summary>
        /// Tally of occurrences of bitpatterns that look like addresses,
        /// excluding relocations which are known to be addresses.
        /// </summary>
        /// <remarks>
        /// The shorter the addresses are, the less reliable this information
        /// becomes as the probability that a random bit pattern coincides
        /// with a real address increases the shorter the bit pattern is.
        /// </remarks>
        public Dictionary<Address, int> PossibleAddresses { get; set; }

        /// <summary>
        /// Addresses at which indirect jumps happen.
        /// </summary>
        public HashSet<Address> IndirectJumps { get; set; }

        /// <summary>
        /// Addresses at which indirect calls happen
        /// </summary>
        public HashSet<Address> IndirectCalls { get; set; }

        /// <summary>
        /// Addresses which cannot possibly be valid instructions.
        /// </summary>
        public HashSet<Address> Invalid { get; set; }

        /// <summary>
        /// Useful for debugging.
        /// </summary>
        public HashSet<Address> WatchedAddresses;

        /// <summary>
        /// Dumps the contents of this <see cref="ScanResults"/> instance to
        /// the debuggers diagnostic console.
        /// </summary>
        /// <param name="caption"></param>

        [Conditional("DEBUG")]
        public void Dump(string caption = "Dump")
        {
            //BreakOnWatchedAddress(ICFG.Nodes.Select(n => n.Address));

            return;     // This is horribly verbose, so only use it when debugging unit tests.
#if VERBOSE
            Debug.Print("== {0} =====================", caption);
            Debug.Print("{0} nodes", ICFG.Nodes.Count);
            foreach (var block in ICFG.Nodes.OrderBy(n => n.Address))
            {
                var addrEnd = block.GetEndAddress();
                if (KnownProcedures.Contains(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: known procedure ----------", block.Address);
                }
                else if (DirectlyCalledAddresses.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: possible procedure, called {1} time(s) ----------",
                        block.Address,
                        DirectlyCalledAddresses[block.Address]);
                }
                Debug.Print("{0}:  //  pred: {1}",
                    block.Name,
                    string.Join(" ", ICFG.Predecessors(block)
                        .OrderBy(n => n.Address)
                        .Select(n => n.Address)));
                foreach (var cluster in block.Instructions)
                {
                    Debug.Print("  {0}", cluster);
                    foreach (var instr in cluster.Instructions)
                    {
                        Debug.Print("    {0}", instr);
                    }
                }
                Debug.Print("  // succ: {0}", string.Join(" ", ICFG.Successors(block)
                    .OrderBy(n => n.Address)
                    .Select(n => n.Address)));
            }
#endif
        }

        /// <summary>
        /// Represents a single RTL cluster in the program, augmented with
        /// a block identifier, predecessor and successor counts.
        /// </summary>
        public class Instr
        {
            /// <summary>
            /// Processor architecture of the instruction.
            /// </summary>
            public IProcessorArchitecture Architecture { get; set; }

            /// <summary>
            /// Address of the instruction.
            /// </summary>
            public Address Address => rtl.Address;

            /// <summary>
            /// Length of the instruction in storage units.
            /// </summary>
            public int Length => rtl.Length;

            /// <summary>
            /// Instruction class of the instruction.
            /// </summary>
            public InstrClass Class => rtl.Class;

            /// <summary>
            /// Block identifier of the instruction.
            /// </summary>
            public Address block_id;

            /// <summary>
            /// Number of predecessors of this instruction.
            /// </summary>
            public int pred;

            /// <summary>
            /// Number of successors of this instruction.
            /// </summary>
            public int succ;

            /// <summary>
            /// The RTL instruction cluster.
            /// </summary>
            public RtlInstructionCluster rtl { get; set; }
        }

        /// <summary>
        /// Represents an edge between two addresses in the
        /// interprocedural control flow graph.
        /// </summary>
        public class Link
        {
            /// <summary>
            /// Source of the edge.
            /// </summary>
            public Address From { get; }

            /// <summary>
            /// Destination of the edge.
            /// </summary>
            public Address To { get; }

            /// <summary>
            /// Constructs a new link between two addresses.
            /// </summary>
            /// <param name="first">Source of the edge.</param>
            /// <param name="second">Destination of the edge.</param>
            public Link(Address first, Address second)
            {
                this.From = first;
                this.To = second;
            }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                if (obj is not Link that)
                    return false;
                return that.From == this.From && that.To == this.To;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return From.GetHashCode() ^ 13 * To.GetHashCode();
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return string.Format("[{0:X8} -> {1:X8}]", From, To);
            }
        }

        /// <summary>
        /// Breaks into the debugger if any of the addresses in the provided
        /// collection <paramref name="enumerable"/> matches any of the
        /// watched addresses.
        /// </summary>
        /// <param name="enumerable">Addresses to test.</param>
        [Conditional("DEBUG")]
        public void BreakOnWatchedAddress(IEnumerable<Address> enumerable)
        {
            var hits = enumerable.Intersect(this.WatchedAddresses).ToArray();
            if (hits.Length > 0)
                Debugger.Break();
        }
    }
}
