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
        /// <summary>
        /// Constructs a new instance of <see cref="ScanResultsV2"/>.
        /// </summary>
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
            this.WatchedAddresses = [];

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
        /// Maps the entry point address to <see cref="RtlProcedure"/>s.
        /// </summary>
        /// <remarks>
        /// These procedures were found either from the binary or during
        /// recursive scanning, and are considered "reliable".
        /// </remarks>
        public ConcurrentDictionary<Address, RtlProcedure> Procedures { get; }

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

        /// <summary>
        /// Procedures marked by the user as "no-decompile".
        /// </summary>
        public ConcurrentDictionary<Address, ExternalProcedure> NoDecompiles { get; }

        /// <summary>
        /// The return statuses of procedures, indexed by their entry point address.
        /// </summary>
        public ConcurrentDictionary<Address, ReturnStatus> ProcReturnStatus { get; }

        /// <summary>
        /// The interprocedural control graph formed by <see cref="Successors"/> and
        /// <see cref="Predecessors"/>.
        /// </summary>
        public ScanResultsGraph ICFG { get; }

        /// <summary>
        /// Addresses being watched for changes. This is primarily intended
        /// for debugging purposes, to allow the user to break when a watched address
        /// is reached during scanning.
        /// </summary>
        public HashSet<Address> WatchedAddresses { get; }

        /// <summary>
        /// Dumps the contents of the scan results to the debug output.
        /// </summary>
        /// <param name="caption">Optional caption.</param>
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

    /// <summary>
    /// Classification of edges in the interprocedural control flow graph.
    /// </summary>
    public enum EdgeType
    {
        /// <summary>
        /// An edge resulting from a control flow instruction.
        /// </summary>
        Jump,

        /// <summary>
        /// An edge that is the fall-through destination of a 
        /// branch instruction or a the next instruction after a
        /// call instruction.
        /// </summary>
        Fallthrough,

        /// <summary>
        /// An edge that results from a call instruction.
        /// </summary>
        Call,

        /// <summary>
        /// An edge that results from a return instruction.
        /// </summary>
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
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{From} -> {To} ({Type})";
        }
    }
}
