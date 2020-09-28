#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        public ScanResults()
        {
            this.TransferTargets = new HashSet<Address>();
            this.DirectlyCalledAddresses = new Dictionary<Address, int>();
            this.Instructions = new Dictionary<Address, RtlInstructionCluster>();
            this.FlatInstructions = new Dictionary<ulong, ScanResults.instr>();
            this.FlatEdges = new List<link>();
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
        public Dictionary<Address, RtlInstructionCluster> Instructions;

        /// <summary>
        /// Interprocedural control flow graph, consisting of all
        /// direct calls and jumps. Each edge goes from a jump or a call
        /// to its destination. Branches have two destinations.
        /// </summary>
        public DiGraph<RtlBlock> ICFG;

        /// <summary>
        /// Tally of how many times each address is called by a direct call
        /// instruction.
        /// </summary>
        public Dictionary<Address, int> DirectlyCalledAddresses;

        /// <summary>
        /// Addresses that are targets (destinations) of jumps or calls.
        /// </summary>
        public HashSet<Address> TransferTargets;

        /// <summary>
        /// These are addresses that are known, because metadata in the
        /// executable image describes them as such, or because the user
        /// specified the address.
        /// </summary>
        public Dictionary<Address, ImageSymbol> KnownAddresses;

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
        public List<RtlProcedure> Procedures { get;  set; }
        public Dictionary<ulong, instr> FlatInstructions { get;  set; }
        public List<link> FlatEdges { get; set; }

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
        public Dictionary<Address, int> PossibleAddresses;

        /// <summary>
        /// Addresses at which indirect jumps happen.
        /// </summary>
        public HashSet<Address> IndirectJumps;

        /// <summary>
        /// Addresses at which indirect calls happen
        /// </summary>
        public HashSet<Address> IndirectCalls;

        /// <summary>
        /// Addresses which cannot possibly be valid instructions.
        /// </summary>
        public HashSet<Address> Invalid;
        /// <summary>
        /// Useful for debugging.
        /// </summary>
        public HashSet<Address> WatchedAddresses;

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

        public class instr
        {
            public Address addr;
            public int size;
            public ushort type;
            public Address block_id;
            public int pred;
            public int succ;
            public RtlInstructionCluster rtl;
        }

        public class link
        {
            public Address first;
            public Address second;

            public override bool Equals(object obj)
            {
                if (!(obj is link that))
                    return false;
                return that.first == this.first && that.second == this.second;
            }

            public override int GetHashCode()
            {
                return first.GetHashCode() ^ 13 * second.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("[{0:X8} -> {1:X8}]", first, second);
            }
        }

        public class block
        {
            public Address id;                  // Address of block
            public Address component_id;        // Component we're part of.
            public instr[] instrs;              // The instructions of the block.
        }

        [Conditional("DEBUG")]
        public void BreakOnWatchedAddress(IEnumerable<Address> enumerable)
        {
            var hits = enumerable.Intersect(this.WatchedAddresses).ToArray();
            if (hits.Length > 0)
                hits.ToString();
        }
    }
}
