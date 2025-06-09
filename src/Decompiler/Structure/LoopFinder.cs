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

using Reko.Core.Graphs;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Structure
{
    public enum NodeColor
    {
        Gray,
        Black
    }

    /// <summary>
    /// This class finds all nodes on the paths from a given node to itself
    /// ending with a back edge to the given node. All the discovered nodes
    /// are expected to belong to the loop with the given node being its entry.
    /// </summary>
    public partial class LoopFinder<T> where T : class
    {
        private DirectedGraph<T> graph;
        private T entry;
        private Dictionary<T, NodeColor> nodeColor;
        private ISet<T> loopNodes;

        public LoopFinder(DirectedGraph<T> graph, T entry, DominatorGraph<T> doms)
        {
            this.graph = graph;
            this.entry = entry;
            this.loopNodes = new HashSet<T>();
            this.nodeColor = new Dictionary<T, NodeColor>();
            
            Debug.Assert(entry is not null);

            // Find all nodes that can be reached from the back-edge
            // predecessors by reversed edges and paint them gray.
            foreach (var p in graph.Predecessors(entry!))
            {
                if (doms.DominatesStrictly(entry, p))
                {
                    // Back edge!
                    if (!nodeColor.ContainsKey(p))
                    {
                        // Unvisited back edge!
                        BackwardVisit(p);
                    }
                } 
                else if (p == entry)
                {
                    // Self-loop.
                    loopNodes.Add(p);
                }
            }

            // Find all gray nodes that can be visited from the suspected
            // loop entry and color them black. Black nodes belong
            // to the loop.
            if (nodeColor.TryGetValue(entry!, out NodeColor color) &&
                color == NodeColor.Gray)
            {
                ForwardVisit(entry!);
            }
        }

        public ISet<T> LoopNodes { get { return loopNodes; } }

        /// <summary>
        /// Visits given node and, if the node is not entry, recursively
        /// visits all its unvisited predecessors. All the visited nodes are
        /// painted Gray.
        /// </summary>
        /// <param name="node">Reference to an unvisited node.</param>
        void BackwardVisit(T node)
        {
            Debug.Assert(!nodeColor.ContainsKey(node));

            nodeColor[node] = NodeColor.Gray;

            if (node.Equals(entry))
            {
                return;
            }

            foreach (var p in graph.Predecessors(node))
            {
                if (!nodeColor.ContainsKey(p))
                {
                    BackwardVisit(p);
                }
            }
        }

        /// <summary>
        /// Visits given node and recursively visits all its GRAY successors.
        /// All the visited nodes are painted Black.
        /// </summary>
        /// <param name="node">Reference to a Gray node.</param> 
        void ForwardVisit(T node)
        {
            Debug.Assert(nodeColor[node] == NodeColor.Gray);

            nodeColor[node] = NodeColor.Black;
            loopNodes.Add(node);

            foreach (var s in graph.Successors(node))
            {
                if (nodeColor.TryGetValue(s, out NodeColor color) &&
                    color == NodeColor.Gray)
                {
                    ForwardVisit(s);
                }
            }
        }
    }
}