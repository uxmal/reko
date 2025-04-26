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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Represents a directed graph of <see cref="Block"/>s.
    /// </summary>
    public class BlockGraph : DirectedGraph<Block>
    {
        private IList<Block> blocks;

        /// <summary>
        /// Constructs a new <see cref="BlockGraph"/> from the given list of blocks.
        /// </summary>
        /// <param name="blocks">The list of blocks from a <see cref="Procedure"/>.</param>
        public BlockGraph(IList<Block> blocks)
        {
            this.blocks = blocks;
        }

        /// <summary>
        /// Removes a block from the graph, as well as any edges
        /// to other blocks in the graph.
        /// </summary>
        /// <param name="block"></param>
        public void RemoveBlock(Block block)
        {
            var preds = block.Pred.ToList();
            foreach (var p in preds)
            {
                RemoveEdge(p, block);
            }
            var succs = block.Succ.ToList();
            foreach (var p in succs)
            {
                RemoveEdge(block, p);
            }
            blocks.Remove(block);
        }

        #region DirectedGraph<Block> Members

        /// <inheritdoc/>
        public ICollection<Block> Predecessors(Block node)
        {
            return node.Pred;
        }

        /// <inheritdoc/>
        public ICollection<Block> Successors(Block node)
        {
            return node.Succ;
        }

        /// <inheritdoc/>
        public IList<Block> Blocks { get { return blocks; } }
        ICollection<Block> DirectedGraph<Block>.Nodes { get { return blocks; } }

        /// <inheritdoc/>
        public void AddEdge(Block nodeFrom, Block nodeTo)
        {
            nodeFrom.Succ.Add(nodeTo);
            nodeTo.Pred.Add(nodeFrom);
        }

        /// <inheritdoc/>
        public void RemoveEdge(Block nodeFrom, Block nodeTo)
        {
            if (nodeFrom.Succ.Contains(nodeTo) && nodeTo.Pred.Contains(nodeFrom))
            {
                nodeFrom.Succ.Remove(nodeTo);
                nodeTo.Pred.Remove(nodeFrom);
            }
        }

        /// <inheritdoc/>
        public bool ContainsEdge(Block from, Block to)
        {
            return from.Succ.Contains(to);
        }

        #endregion

        /// <summary>
        /// Validates the graph by checking that the edges are consistent.
        /// </summary>
        /// <returns>True if the graph is consistent.</returns>
        public bool Validate()
        {
            // p -> sedges
            var p2s_edges = new List<(Block, Block)>();
            var s2p_edges = new List<(Block, Block)>();
            foreach (var b in blocks)
            {
                p2s_edges.AddRange(b.Succ.Select(s => (b, s)));
                s2p_edges.AddRange(b.Pred.Select(p => (p, b)));
            }
            static int Cmp((Block, Block) a, (Block, Block) b)
            {
                int d = a.Item1.Id.CompareTo(b.Item1.Id);
                if (d != 0)
                    return d;
                return a.Item2.Id.CompareTo(b.Item2.Id);

            }

            bool Dump(int iBad)
            {
                for (int i = 0; i < Math.Max(p2s_edges.Count, s2p_edges.Count); ++i)
                {
                    Debug.Print("{0}:{1,-55} {2} {3}",
                        i,
                        i < p2s_edges.Count ? p2s_edges[i] : "***",
                        i < s2p_edges.Count ? s2p_edges[i] : "***",
                        i == iBad ? "<--" : "");
                }
                return false;
            }

            p2s_edges.Sort(Cmp);
            s2p_edges.Sort(Cmp);
            int i;
            for (i = 0; i < Math.Min(p2s_edges.Count, s2p_edges.Count); ++i)
            {
                if (p2s_edges[i] != s2p_edges[i])
                {
                    return Dump(i);
                }
            }
            if (i < p2s_edges.Count)
            {
                Dump(i);
            }
            if (i < s2p_edges.Count)
            {
                return Dump(i);
            }
            return true;
        }
    }
}
