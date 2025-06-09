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
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Implements the Lengauer-Tarjan algorithm for finding dominators as
    /// described in Appel "Modern Compiler Implementation in Java".
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class LTDominatorGraph<TNode> where TNode: class
    {
        public static Dictionary<TNode,TNode?> Create(DirectedGraph<TNode> graph, TNode root)
        {
            return new Builder(graph, root).Dominators();
        }

        public class Builder
        {
            private DirectedGraph<TNode> graph;
            private TNode root;
            private Dictionary<TNode, int> dfnum;
            private int N;
            private Dictionary<TNode, TNode?> parent;
            private Dictionary<int, TNode> vertex;
            private Dictionary<TNode, HashSet<TNode>> bucket;
            private Dictionary<TNode, TNode?> semi;
            private Dictionary<TNode, TNode?> ancestor;
            private Dictionary<TNode, TNode?> idom;
            private Dictionary<TNode, TNode?> samedom;
            private Dictionary<TNode, TNode?> best;

            public Builder(DirectedGraph<TNode> graph, TNode root)
            {
                this.graph = graph;
                this.root = root;
                N = 0;
                bucket = graph.Nodes.ToDictionary(n => n, n => new HashSet<TNode>());
                dfnum = graph.Nodes.ToDictionary(n => n, n => 0);
                semi = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                ancestor = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                idom = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                samedom = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                best = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                parent = graph.Nodes.ToDictionary(n => n, n => default(TNode));
                vertex = new Dictionary<int, TNode>();
            }

            void DFS(TNode p, TNode n)
            {
                if (dfnum[n] == 0)
                {
                    dfnum[n] = N;
                    vertex[N] = n;
                    parent[n] = p;
                    ++N;
                    foreach (var w in graph.Successors(n))
                    {
                        DFS(n, w);
                    }
                }
            }

            public Dictionary<TNode, TNode?> Dominators()
            {
                DFS(default!, root);
                for (int i = N - 1; i > 0; --i)   // skip over root node 0
                {
                    var n = vertex[i];
                    var p = parent[n];
                    var s = p!;
                    foreach (var v in graph.Predecessors(n))
                    {
                        TNode ss;
                        if (dfnum[v] <= dfnum[n])
                        {
                            ss = v;
                        }
                        else
                        {
                            ss = semi[AncestorWithLowestSemi(v)!]!;
                        }
                        //$REVIEW s and ss could be null here?
                        if (dfnum[ss!] < dfnum[s!])
                            s = ss!;
                    }
                    semi[n] = s;
                    bucket[s].Add(n);
                    Link(p!, n);
                    foreach (var v in bucket[p!])
                    {
                        var y = AncestorWithLowestSemi(v);
                        if (semi[y] == semi[v])
                        {
                            idom[v] = p;
                        }
                        else
                        {
                            samedom[v] = p;
                        }
                    }
                    bucket[p!].Clear();
                }
                for (int i = 1; i < N; ++i)
                {
                    var n = vertex[i];
                    if (samedom[n] is not null)
                    {
                        idom[n] = idom[samedom[n]!];
                    }
                }
                return idom;
            }

            TNode AncestorWithLowestSemi(TNode v)
            {
                var a = ancestor[v]!;
                if (ancestor[a] is not null)
                {
                    var b = AncestorWithLowestSemi(a);
                    ancestor[v] = ancestor[a];
                    if (dfnum[semi[b]!] < dfnum[semi[best[v]!]!])
                        best[v] = b;
                }
                return best[v]!;
            }

            void Link(TNode p, TNode n)
            {
                ancestor[n] = p; best[n] = n;
            }
        }
    }
}
