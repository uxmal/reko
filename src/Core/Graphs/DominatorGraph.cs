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
using System.IO;

namespace Reko.Core.Graphs
{
    /// <summary>
    /// Describes the dominator structure of a particular graph.
    /// </summary>
    public class DominatorGraph<T> where T : class
    {
        private readonly Dictionary<T, T?> idoms;             // immediate dominators for each vertex.
        private readonly Dictionary<T, List<T>> domFrontier;
        private Dictionary<T, int> reversePostOrder;

        /// <summary>
        /// Creates a new dominator graph for the given directed graph.
        /// </summary>
        /// <param name="graph">A directed graph.</param>
        /// <param name="entryNode">The entry node of the directed graph.</param>
        public DominatorGraph(DirectedGraph<T> graph, T entryNode)
        {
            this.idoms = new Dictionary<T, T?>();
            this.reversePostOrder = new Dictionary<T, int>();
            this.idoms = Build(graph, entryNode);
            this.idoms[entryNode] = null;		// No-one dominates the root node.
                this.domFrontier = BuildDominanceFrontiers(graph, idoms);
            }

        private Dictionary<T, int> ReversePostorderNumbering(DirectedGraph<T> graph)
        {
            var reversePostOrder = new Dictionary<T, int>();
            foreach (T node in new DfsIterator<T>(graph).PostOrder())
            {
                reversePostOrder.Add(node, graph.Nodes.Count - (reversePostOrder.Count + 1));
            }
            return reversePostOrder;
        }

        private Dictionary<T, int> ReversePostorderNumbering(DirectedGraph<T> graph, T entry)
        {
            var postorder = new Dictionary<T, int>();
            foreach (T node in new DfsIterator<T>(graph).PostOrder(entry))
            {
                postorder.Add(node, graph.Nodes.Count - (postorder.Count + 1));
            }
            return postorder;
        }

        /// <summary>
        /// Find the common dominator of all the provided blocks.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        public T? CommonDominator(IEnumerable<T> blocks)
        {
            if (blocks is null)
                return null;

            T? dominator = null;
            foreach (T b in blocks)
            {
                if (b is null)
                    return null;
                if (dominator is null)
                {
                    dominator = b;
                }
                else if (b != dominator && !DominatesStrictly(dominator, b))
                {
                    if (DominatesStrictly(b, dominator))
                    {
                        dominator = b;
                    }
                    else
                    {
                        do
                        {
                            dominator = ImmediateDominator(dominator!);
                        } while (!DominatesStrictly(dominator, b));
                    }
                }
            }
            return dominator;
        }

        /// <summary>
        /// Gets the dominator frontier of the given node.
        /// </summary>
        /// <param name="node">Node whose dominator frontier is requested.</param>
        /// <returns>A list of nodes in the dominator frontier.
        /// </returns>
        public List<T> DominatorFrontier(T node)
        {
            return domFrontier[node];
        }

        /// <summary>
        /// Returns true if <paramref name="dominator"/> dominates <paramref name="node"/>
        /// strictly.
        /// </summary>
        /// <param name="dominator">Potential dominator node.</param>
        /// <param name="node">Node whose possible dominator is being tested.</param>
        /// <returns>True if <paramref name="dominator"/> dominates <paramref name="node"/> strictly;
        /// otherwise false.</returns>
        public bool DominatesStrictly(T? dominator, T node)
        {
            while (idoms.TryGetValue(node, out T? iDom) && iDom is not null)
            {
                if (iDom == dominator)
                    return true;
                node = iDom;
            }
            return false;
        }

        /// <summary>
        /// Get the immediate dominator of the given node.
        /// </summary>
        /// <param name="node">Node whose immediate dominator is being queried.</param>
        /// <returns>The immediate dominator if it exists; otherwise null.</returns>
        public T? ImmediateDominator(T node)
        {
            if (idoms.TryGetValue(node, out T? idom))
                return idom;
            return default;
        }

        // Postdominators:
        // Providing Static Timing Anlaysis Support for an ARM7 Processor Platform
        // http://www.lib.ncsu.edu/theses/available/etd-05022008-163037/unrestricted/etd.pdf

        private Dictionary<T, T?> Build(DirectedGraph<T> graph, T entryNode)
        {
            idoms[entryNode] = entryNode;
            reversePostOrder = ReversePostorderNumbering(graph);
            var nodes = new SortedList<int, T>();
            foreach (KeyValuePair<T, int> de in reversePostOrder)
            {
                nodes.Add(de.Value, de.Key);
            }

            bool changed;
            do
            {
                changed = false;
                foreach (T b in nodes.Values) 
                {
                    if (b == entryNode)
                        continue;
                    T? newIdom = null;
                    foreach (T p in graph.Predecessors(b))
                    {
                        if (idoms.ContainsKey(p))
                        {
                            if (newIdom is null)
                                newIdom = p;
                            else if (idoms.ContainsKey(p))
                            {
                                newIdom = Intersect(idoms, p, newIdom);
                            }
                        }
                    }

                    if ((!idoms.TryGetValue(b, out T? oldIdom) || oldIdom != newIdom) && newIdom is not null)
                    {
                        idoms[b] = newIdom;
                        changed = true;
                    }
                }
            } while (changed);
            return idoms;
        }

        private static Dictionary<T, List<T>> BuildDominanceFrontiers(DirectedGraph<T> graph, Dictionary<T,T?> idoms)
        {
            var fronts = new Dictionary<T, List<T>>();
            foreach (T node in graph.Nodes)
            {
                fronts[node] = new List<T>();
            }

            foreach (T bb in graph.Nodes)
            {
                var pred = graph.Predecessors(bb);
                if (pred.Count < 2)
                    continue;
                foreach (T p in pred)
                {
                    T? r = p;
                    while (r is not null && (!idoms.TryGetValue(bb, out var idom) || r != idom))
                    {
                        // Add b to the dominance frontier of r.

                        if (!fronts[r].Contains(bb))
                            fronts[r].Add(bb);

                        idoms.TryGetValue(r, out r);
                    }
                }
            }
            return fronts;
        }

        /// <summary>
        /// Writes the dominator graph to the debug output.
        /// </summary>
        [Conditional("DEBUG")]
        public void Dump()
        {
            var sw = new StringWriter();
            Write(sw);
            Debug.Write(sw.ToString());
        }

        private T? Intersect(Dictionary<T, T?> postdoms, T b1, T b2)
        {
            T? i1 = b1;
            T? i2 = b2;
            int c = 0;
            while (i1 != i2)
            {
                while (reversePostOrder[i1!] > reversePostOrder[i2!])
                {
                    ++c;
                    if (c > 100000)
                        throw new ApplicationException("Dominator graph calculation timed out.");
                    i1 = postdoms[i1!];
                }
                while (reversePostOrder[i2!] > reversePostOrder[i1!])
                {
                    ++c;
                    if (c > 100000)
                        throw new ApplicationException("Dominator graph calculation timed out.");
                    i2 = postdoms[i2!];
                }
            }
            return i1;
        }

        /// <summary>
        /// Writes the dominator graph to the given text writer.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public void Write(TextWriter writer)
        {
            var blocks = new SortedDictionary<string, T?>();
            foreach (KeyValuePair<T,T?> node in idoms)
            {
                blocks.Add(node.Key.ToString()!, node.Value);
            }

            foreach (KeyValuePair<string,T?> b in blocks)
            {
                writer.WriteLine("{0}: idom {1}", b.Key, b.Value);
            }
        }
    }
}
