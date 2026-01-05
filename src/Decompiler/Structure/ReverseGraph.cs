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

using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Structure
{
    /// <summary>
    /// This class is used for the computation of post-dominators
    /// in the StructureAnalyis class.
    /// </summary>
    //$REVIEW: move to Reko.Core.Graphs?
    public class ReverseGraph : DirectedGraph<Region>
    {
        private DirectedGraph<Region> graph;

        private struct Edge
        {
            public Edge(Region from, Region to) { this.From = from; this.To = to; }

            public Region From;
            public Region To;
        }

        /// <summary>
        /// Constructs a reverse graph from the given directed graph.
        /// </summary>
        /// <param name="graph"></param>
        public ReverseGraph(DirectedGraph<Region> graph)
        {
            this.graph = new DiGraph<Region>();
            foreach (var n in graph.Nodes)
            {
                this.graph.Nodes.Add(n);
            }
            foreach (var n in graph.Nodes)
            {
                foreach (var s in graph.Successors(n))
                {
                    this.graph.AddEdge(n, s);
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<Region> Predecessors(Region node)
        {
            return graph.Successors(node);
        }

        /// <inheritdoc/>
        public ICollection<Region> Successors(Region node)
        {
            return graph.Predecessors(node);
        }

        /// <inheritdoc/>
        public ICollection<Region> Nodes
        {
            get { return graph.Nodes; }
        }

        /// <inheritdoc/>
        public void AddEdge(Region nodeFrom, Region nodeTo)
        {
            graph.AddEdge(nodeTo, nodeFrom);
        }

        /// <inheritdoc/>
        public void RemoveEdge(Region nodeFrom, Region nodeTo)
        {
            graph.RemoveEdge(nodeTo, nodeFrom);
        }

        /// <inheritdoc/>
        public bool ContainsEdge(Region nodeFrom, Region nodeTo)
        {
            return graph.ContainsEdge(nodeTo, nodeFrom);
        }
    }
}
