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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// A directed graph implemented as a dictionary of lists.
    /// </summary>
    public class AdjacencyListGraph<T> : DirectedGraph<T>
        where T : notnull
    {
        private readonly Dictionary<T, List<T>> predecessors;

        /// <summary>
        /// Constructs an adjacency list graph from a dictionary of lists.
        /// </summary>
        /// <param name="adjacencyList">A dictionary mapping nodes to lists of successors.
        /// </param>
        public AdjacencyListGraph(IDictionary<T, List<T>> adjacencyList)
        {
            this.AdjacencyList = adjacencyList;
            this.predecessors = new Dictionary<T, List<T>>();
            foreach (var (n, succs) in adjacencyList)
            {
                foreach (var s in succs)
                {
                    if (!predecessors.TryGetValue(s, out var preds))
                    {
                        preds = [];
                        predecessors.Add(s, preds);
                    }
                    preds.Add(n);
                }
            }
        }

        /// <summary>
        /// The underlying adjacency list of the graph.
        /// </summary>
        public IDictionary<T, List<T>> AdjacencyList { get; }


        /// <inheritdoc/>
        public ICollection<T> Nodes => this.AdjacencyList.Keys;

        public IEnumerable<(T, T)> Edges =>
            AdjacencyList.SelectMany(kvp => kvp.Value.Select(s => (kvp.Key, s)));

        /// <inheritdoc/>
        public void AddEdge(T nodeFrom, T nodeTo)
        {
            if (!AdjacencyList.TryGetValue(nodeFrom, out var succs))
            {
                succs = new List<T>();
                AdjacencyList.Add(nodeFrom, succs);
            }
            succs.Add(nodeTo);

            if (!predecessors.TryGetValue(nodeTo, out var preds))
            {
                preds = new List<T>();
                predecessors.Add(nodeTo, preds);
            }
            preds.Add(nodeFrom);
        }

        /// <inheritdoc/>
        public bool ContainsEdge(T nodeFrom, T nodeTo)
        {
            if (AdjacencyList.TryGetValue(nodeFrom, out var succs))
                return succs.Contains(nodeTo);
            return false;
        }

        /// <inheritdoc/>
        public ICollection<T> Predecessors(T node)
        {
            if (predecessors.TryGetValue(node, out var preds))
                return preds;
            else
                return Array.Empty<T>();
        }

        /// <inheritdoc/>
        public void RemoveEdge(T nodeFrom, T nodeTo)
        {
            if (AdjacencyList.TryGetValue(nodeFrom, out var succs))
                succs.Remove(nodeTo);
            if (predecessors.TryGetValue(nodeTo, out var preds))
                preds.Remove(nodeFrom);
        }

        /// <inheritdoc/>
        public ICollection<T> Successors(T node)
        {
            if (AdjacencyList.TryGetValue(node, out var succs))
                return succs;
            else
                return Array.Empty<T>();
        }
    }
}
