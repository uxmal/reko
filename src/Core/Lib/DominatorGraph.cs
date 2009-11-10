/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Core.Lib
{
    /// <summary>
    /// Describes the dominator structure of a particular graph.
    /// </summary>
    public class DominatorGraph<T> where T : class
    {
        private Dictionary<T, T> idoms;

        private const int Undefined = -1;

        public DominatorGraph(DirectedGraph<T> graph, T entryNode)
        {
            this.idoms = Build(graph, entryNode);
            this.idoms[entryNode] = null;		// No-one postdominates the root node.
        }

        public Dictionary<T, int> ReversePostorderNumbering(DirectedGraph<T> graph)
        {
            Dictionary<T, int> postorder = new Dictionary<T, int>();
            foreach (T node in new DfsIterator<T>(graph).PostOrder())
            {
                postorder.Add(node, graph.Nodes.Count - (postorder.Count + 1));
            }
            return postorder;
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
        public T CommonDominator(ICollection<T> blocks)
        {
            if (blocks == null || blocks.Count == 0)
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

        public bool DominatesStrictly(T dominator, T d)
        {
            T iDom;
            while (idoms.TryGetValue(d, out iDom))
            {
                if (iDom == dominator)
                    return true;
                d = iDom;
            }
            return false;
        }

        public T ImmediateDominator(T node)
        {
            return idoms[node];
        }

        // Postdominators
        // http://www.lib.ncsu.edu/theses/available/etd-05022008-163037/unrestricted/etd.pdf

        public Dictionary<T, T> Build(DirectedGraph<T> graph, T entryNode)
        {
            Dictionary<T, T> idoms = new Dictionary<T, T>();
            idoms[entryNode] = entryNode;
            Dictionary<T, int> postorder = ReversePostorderNumbering(graph);
            SortedList<int, T> nodes = new SortedList<int, T>();
            foreach (KeyValuePair<T, int> de in postorder)
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
                                newIdom = Intersect(idoms, p, newIdom, postorder);
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


        public void Dump()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            Debug.Write(sw.ToString());
        }

        private static T Intersect(Dictionary<T, T> postdoms, T b1, T b2, Dictionary<T, int> postorder)
        {
            T i1 = b1;
            T i2 = b2;
            while (i1 != i2)
            {
                while (postorder[i1] > postorder[i2])
                {
                    i1 = postdoms[i1];
                }
                while (postorder[i2] > postorder[i1])
                {
                    i2 = postdoms[i2];
                }
            }
            return i1;
        }

        public void Write(TextWriter writer)
        {
            SortedDictionary<string, string> blocks = new SortedDictionary<string, string>();
            foreach (KeyValuePair<T,T> node in idoms)
            {
                blocks.Add(node.Key.ToString(), node.Value.ToString());
            }

            foreach (KeyValuePair<string,string> b in blocks)
            {
                writer.WriteLine("{0}: ipdom {1}", b.Key, b.Value);
            }
        }
    }
}
