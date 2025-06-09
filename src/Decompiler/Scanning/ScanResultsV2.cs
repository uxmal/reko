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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Scanning
{
    /// <summary>
    /// The information collected by scanning a binary.
    /// </summary>
    public class ScanResultsV2
    {
        public ScanResultsV2()
        {
            this.Blocks = new();
            this.Successors = new();
            this.Predecessors = new();
            this.Procedures = new();
            this.InvalidBlocks = new();
            this.SpeculativeBlocks = new();
            this.SpeculativeProcedures = new();
            this.TrampolineStubStarts = new();
            this.TrampolineStubEnds = new();
            this.NoDecompiles = new();
            this.ICFG = new ScanResultsGraph(this);
            this.WatchedAddresses = new HashSet<Address>();

            this.ProcReturnStatus = new();
        }

        /// <summary>
        /// Maps addresses to the <see cref="RtlBlock"/>s at those addresses.
        /// </summary>
        public ConcurrentDictionary<Address, RtlBlock> Blocks { get; }

        /// <summary>
        /// Maps start ("from") addresses to destination <see cref="Address"/>es
        /// reached by edges those addresses.
        /// </summary>
        public ConcurrentDictionary<Address, List<Address>> Successors { get; }

        /// <summary>
        /// Maps end ("to") addresses to edges arriving at those
        /// addresses.
        /// </summary>
        public ConcurrentDictionary<Address, List<Address>> Predecessors { get; }

        /// <summary>
        /// Maps the entry point address to <see cref="Proc"/>s.
        /// </summary>
        /// <remarks>
        /// These procedures were found either from the binary or during
        /// recursive scanning, and are considered "reliable".
        /// </remarks>
        public ConcurrentDictionary<Address, Proc> Procedures { get; }

        /// <summary>
        /// Keeps track of all basic blocks that have been found to 
        /// be invalid.
        /// </summary>
        public ConcurrentDictionary<Address, Address> InvalidBlocks { get; }

        /// <summary>
        /// Addresses of blocks that were discovered during speculative scanning.
        /// </summary>
        /// <remarks>
        /// These blocks were found during speculative scaning, and aren't 
        /// considered "reliable" without more information.
        /// </remarks>
        public ConcurrentDictionary<Address, int> SpeculativeBlocks { get; }

        /// <summary>
        /// Addresses of possible procedures that were discovered during 
        /// speculative scanning. The dictionary maintains a count of how many
        /// times the address was called.
        /// </summary>
        /// <remarks>
        /// These procedures were found during speculative scanning, and aren't
        /// considered "reliable" without more information.
        /// </remarks>
        public ConcurrentDictionary<Address, int> SpeculativeProcedures { get; }

        /// <summary>
        /// Address of the start of short sequences of instructions identified
        /// as dynamic link stubs (such as those in an ELF PLT).
        /// </summary>
        public ConcurrentDictionary<Address, Trampoline> TrampolineStubStarts { get; }

        /// <summary>
        /// Address of the ends short sequences of instructions identified as
        /// dynamic link stubs (such as those in an ELF PLT)
        /// </summary>
        public ConcurrentDictionary<Address, Trampoline> TrampolineStubEnds { get; }

        public ConcurrentDictionary<Address, ExternalProcedure> NoDecompiles { get; }

        public ConcurrentDictionary<Address, Reko.Scanning.ReturnStatus> ProcReturnStatus { get; }

        /// <summary>
        /// The interprocedural control graph formed by <see cref="Successors"/> and
        /// <see cref="Predecessors"/>.
        /// </summary>
        public ScanResultsGraph ICFG { get; }

        public HashSet<Address> WatchedAddresses { get; }

        [Conditional("DEBUG")]
        public void Dump(string caption = "Dump")
        {
            //BreakOnWatchedAddress(ICFG.Nodes.Select(n => n.Address));

            return;     // This is horribly verbose, so only use it when debugging unit tests.
#if VERBOSE
            Debug.Print("== {0} =====================", caption);
            Debug.Print("{0} nodes", ICFG.Nodes.Count);
            foreach (var addBlock in ICFG.Nodes.OrderBy(n => n))
            {
                var block = Blocks[addBlock];
                var addrEnd = block.GetEndAddress();
                if (Procedures.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: known procedure ----------", block.Address);
                }
                else if (SpeculativeProcedures.ContainsKey(block.Address))
                {
                    Debug.WriteLine("");
                    Debug.Print("-- {0}: possible procedure, called {1} time(s) ----------",
                        block.Address,
                        SpeculativeProcedures[block.Address]);
                }
                Debug.Print("{0}:  //  pred: {1}",
                    block.Name,
                    string.Join(" ", ICFG.Predecessors(block.Address)
                        .OrderBy(n => n)));
                foreach (var cluster in block.Instructions)
                {
                    Debug.Print("  {0}", cluster);
                    foreach (var instr in cluster.Instructions)
                    {
                        Debug.Print("    {0}", instr);
                    }
                }
                Debug.Print("  // succ: {0}", string.Join(" ", ICFG.Successors(block.Address)
                    .OrderBy(n => n)));
            }
#endif
        }
    }

    public enum BlockFlags
    {
        Valid = 1,      // Contains valid instructions.
        Invalid = 2,    // Contains invalid instructions.
        Privileged = 3, // Contains privileged instructions.
        Padding = 4,    // Consists only of padding instructions.
    }

    public record Proc(
        Address Address,
        ProvenanceType Provenance,
        IProcessorArchitecture Architecture,
        string Name);

    public enum EdgeType
    {
        Jump,
        Fallthrough,
        Call,
        Return,
    }

    /// <summary>
    /// Represents an edge in the <see cref="ScanResultsV2"/>.
    /// </summary>
    /// <param name="From">The address from which the edge goes.</param>
    /// <param name="To">The address to which the edge goes.</param>
    /// <param name="Type">The type of edge.</param>
    public record Edge(
        Address From,
        Address To,
        EdgeType Type)
    {
        public override string ToString()
        {
            return $"{From} -> {To} ({Type})";
        }
    }
}
