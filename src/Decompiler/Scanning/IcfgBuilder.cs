#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Graphs;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Builds the interpretation control flow graph (ICFG) for a program.
    /// </summary>
    public class IcfgBuilder
    {
        /// <summary>
        /// The graph of all known basic blocks.
        /// </summary>
        public DiGraph<RtlBlock> Blocks;

        /// <summary>
        /// Edges between basic blocks.
        /// </summary>
        public List<(RtlBlock, Address)> Edges;

        /// <summary>
        /// Mapping from addresses to basic blocks.
        /// </summary>
        public Dictionary<Address, RtlBlock> AddrToBlock;

        /// <summary>
        /// Constructs an <see cref="IcfgBuilder"/> instance.
        /// </summary>
        /// <param name="edges">Edges between basic blocks.</param>
        /// <param name="mpBlocks">Mapping from addresses to basic blocks,</param>
        /// <param name="allBlocks">The graph of all known basic blocks.</param>
        public IcfgBuilder(List<(RtlBlock, Address)> edges, Dictionary<Address, RtlBlock> mpBlocks, DiGraph<RtlBlock> allBlocks)
        {
            Edges = edges;
            AddrToBlock = mpBlocks;
            Blocks = allBlocks;
        }
    }
}
