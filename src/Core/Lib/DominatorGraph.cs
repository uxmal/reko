#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
namespace Reko.Core.Lib
{
    /// <summary>
    /// Describes the dominator structure of a particular graph.
    /// </summary>
    public class DominatorGraph<T> where T : class
    {
        private Dictionary<T, T> idoms;             // immediate dominators for each vertex.
        private Dictionary<T, List<T>> domFrontier;
        private Dictionary<T, int> reversePostOrder;

        private const int Undefined = -1;

        public DominatorGraph(DirectedGraph<T> graph, T entryNode)
        {
            this.idoms = new Dictionary<T, T>();
            this.idoms = Build(graph, entryNode);
            this.idoms[entryNode] = null;		// No-one dominates the root node.
            this.domFrontier = BuildDominanceFrontiers(graph, idoms);
        }

        public Dictionary<T, int> ReversePostOrder { get { return reversePostOrder; } }

        public Dictionary<T, int> ReversePostorderNumbering(DirectedGraph<T> graph)
        {
            var reversePostOrder = new Dictionary<T, int>();
            foreach (T node in new DfsIterator<T>(graph).PostOrder())
            {
                reversePostOrder.Add(node, graph.Nodes.Count - (reversePostOrder.Count + 1));
            }
            return reversePostOrder;
        }

        public Dictionary<T, int> ReversePostorderNumbering(DirectedGraph<T> graph, T entry)
        {
            Dictionary<T, int> postorder = new Dictionary<T, int>();
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
        public T CommonDominator(IEnumerable<T> blocks)
        {
            if (blocks == null)
                return null;

            T dominator = null;
            foreach (T b in blocks)
            {
                if (b == null)
                    return null;
                if (dominator == null)
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
                            dominator = ImmediateDominator(dominator);
                        } while (!DominatesStrictly(dominator, b));
                    }
                }
            }
            return dominator;
        }

        public List<T> DominatorFrontier(T node)
        {
            return domFrontier[node];
        }

        public bool DominatesStrictly(T dominator, T d)
        {
            T iDom;
            while (idoms.TryGetValue(d, out iDom) && iDom != null)
            {
                if (iDom == dominator)
                    return true;
                d = iDom;
            }
            return false;
        }

        public T ImmediateDominator(T node)
        {
            T idom;
            if (idoms.TryGetValue(node, out idom))
                return idom;
            return default(T);
        }

        // Postdominators:
        // Providing Static Timing Anlaysis Support for an ARM7 Processor Platform
        // http://www.lib.ncsu.edu/theses/available/etd-05022008-163037/unrestricted/etd.pdf

        private Dictionary<T, T> Build(DirectedGraph<T> graph, T entryNode)
        {
            idoms[entryNode] = entryNode;
            reversePostOrder = ReversePostorderNumbering(graph);
            SortedList<int, T> nodes = new SortedList<int, T>();
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
                    T newIdom = null;
                    foreach (T p in graph.Predecessors(b))
                    {
                        if (idoms.ContainsKey(p))
                        {
                            if (newIdom == null)
                                newIdom = p;
                            else if (idoms.ContainsKey(p))
                            {
                                newIdom = Intersect(idoms, p, newIdom);
                            }
                        }
                    }

                    T oldIdom;
                    if ((!idoms.TryGetValue(b, out oldIdom) || oldIdom != newIdom) && newIdom != null)
                    {
                        idoms[b] = newIdom;
                        changed = true;
                    }
                }
            } while (changed);
            return idoms;
        }

        private Dictionary<T, List<T>> BuildDominanceFrontiers(DirectedGraph<T> graph, Dictionary<T,T> idoms)
        {
            Dictionary<T, List<T>> fronts = new Dictionary<T, List<T>>();
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
                    T r = p;
                    while (r != null && r != idoms[bb])
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


        [Conditional("DEBUG")]
        public void Dump()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            Debug.Write(sw.ToString());
        }

        private T Intersect(Dictionary<T, T> postdoms, T b1, T b2)
        {
            T i1 = b1;
            T i2 = b2;
            int c = 0;
            while (i1 != i2)
            {
                while (reversePostOrder[i1] > reversePostOrder[i2])
                {
                    ++c;
                    if (c > 100000)
                        throw new ApplicationException("Dominator graph calculation timed out.");
                    i1 = postdoms[i1];
                }
                while (reversePostOrder[i2] > reversePostOrder[i1])
                {
                    ++c;
                    if (c > 100000)
                        throw new ApplicationException("Dominator graph calculation timed out.");
                    i2 = postdoms[i2];
                }
            }
            return i1;
        }

        public void Write(TextWriter writer)
        {
            var blocks = new SortedDictionary<string, T>();
            foreach (KeyValuePair<T,T> node in idoms)
            {
                blocks.Add(node.Key.ToString(), node.Value);
            }

            foreach (KeyValuePair<string,T> b in blocks)
            {
                writer.WriteLine("{0}: idom {1}", b.Key, b.Value);
            }
        }
    }
}
