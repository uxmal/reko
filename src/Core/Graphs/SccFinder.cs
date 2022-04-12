#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Linq;

namespace Reko.Core.Graphs
{
	/// <summary>
	/// Implementation of Tarjan's algorithm for finding strongly connected
    /// components (SCC's) in a directed graph.
	/// </summary>
	/// </remarks>
	public static class SccFinder
	{

        /// <summary>
        /// Find the Strongly Connected Components (SCCs) of a <see cref="DirectedGraph{T}"/>,
        /// using O(1) processor stack space.
        /// </summary>
        /// <typeparam name="TNode">The type of the nodes in the graph.</typeparam>
        /// <param name="graph">A directed graph of nodes.</param>
        /// <returns>A <see cref="List{TNode[]}"/>. Each item in the list is an array
        /// consisting of the nodes forming the SCC.</returns>
        /// <remarks>
        /// This implementation avoids overflowing the processor stack if the
        /// graph is very deep.
        /// http://www.logarithmic.net/pfh-files/blog/01208083168/sort.py
        /// https://stackoverflow.com/questions/46511682/non-recursive-version-of-tarjans-algorithm
        /// </remarks>
        public static List<TNode[]> FindAll<TNode>(DirectedGraph<TNode> graph)
            where TNode : notnull
        {
            var result = new List<TNode[]>();
            var stack = new Stack<TNode>();
            var low = new Dictionary<TNode, int>();
            var callstack = new Stack<(TNode, IEnumerator<TNode>?, int)>();
            var eqc = EqualityComparer<TNode>.Default;
            foreach (var w in graph.Nodes)
            {
                callstack.Push((w, null, low.Count));
                while (callstack.Count > 0)
                {
                    var (v, eSucc, num) = callstack.Pop();
                    if (eSucc is null)
                    {
                        if (low.ContainsKey(v))
                            continue;
                        low[v] = num;
                        stack.Push(v);
                        eSucc = graph.Successors(v).GetEnumerator();
                    }
                    else
                    {
                        low[v] = Math.Min(low[v], low[eSucc.Current]);
                    }
                    if (eSucc.MoveNext())
                    {
                        callstack.Push((v, eSucc, num));
                        callstack.Push((eSucc.Current, null, low.Count));
                        continue;
                    }
                    // eSucc is exhausted. Check if we're at the start of a SCC. 
                    if (num == low[v])
                    {
                        var comp = new List<TNode>();
                        while (true)
                        {
                            var n = stack.Pop();
                            comp.Add(n);
                            low[n] = graph.Nodes.Count;
                            if (eqc.Equals(n, v)) break;
                        }
                        result.Add(comp.ToArray());
                    }
                }
            }
            return result;
        }

        public static List<TNode[]> RobustTopologicalSort<TNode>(DirectedGraph<TNode> graph)
            where TNode : notnull
        {
            // First identify strongly connected components,
            // then perform a topological sort on these components.

            var components = FindAll(graph);

            var node_component = new Dictionary<TNode, TNode[]>();
            foreach (var component in components)
            {
                foreach (var node in component)
                {
                    node_component[node] = component;
                }
            }

            var component_graph = new DiGraph<TNode[]>();
            foreach (var component in components)
            {
                component_graph.Nodes.Add(component);
            }

            foreach (var node in graph.Nodes)
            {
                var node_c = node_component[node];
                foreach (var successor in graph.Successors(node))
                {
                    var successor_c = node_component[successor];
                    if (node_c != successor_c)
                    {
                        component_graph.AddEdge(node_c, successor_c);
                    }
                }
            }
            return TopologicalSort(component_graph);
        }

        public static List<TNode> TopologicalSort<TNode>(DirectedGraph<TNode> graph)
            where TNode : notnull
        {
            var count = graph.Nodes.ToDictionary(n => n, n => 0);
            foreach (var node in graph.Nodes)
            {
                foreach (var successor in graph.Successors(node))
                    ++count[successor];
            }

            var ready = (from node in graph.Nodes where count[node] == 0 select node).ToList();
            var result = new List<TNode>();
            while (ready.Count > 0)
            {
                var node = ready[^1];
                ready.RemoveAt(ready.Count - 1);
                result.Add(node);

                foreach (var successor in graph.Successors(node))
                {
                    if (--count[successor] == 0)
                        ready.Add(successor);
                }
            }
            return result;
        }
    }
}
