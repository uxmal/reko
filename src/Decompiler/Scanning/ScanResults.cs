#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System;
using System.Collections.Generic;
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
        public ScanResults() { }
        /// <summary>
        /// All the discovered machine instructions, rewritten into RTL
        /// instruction clusters.
        /// </summary>
        public SortedList<Address, RtlInstructionCluster> Instructions;

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
        /// </summary>
        public List<RtlProcedure> Procedures { get; internal set; }

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

    }
}
