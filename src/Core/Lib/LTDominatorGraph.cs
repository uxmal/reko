using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Lib
{
    public class LTDominatorGraph<node> where node: class
    {
        public static void Create(DirectedGraph<node> graph, node root)
        {
            new Builder(graph, root).Dominators();
        }

        public class Builder
        {
            private DirectedGraph<node> graph;
            private node root;
            private Dictionary<node, int> dfnum;
            private int N;
            private Dictionary<node, node> parent;
            private Dictionary<int, node> vertex;
            private Dictionary<node, HashSet<node>> bucket;
            private Dictionary<node, node> semi;
            private Dictionary<node, node> ancestor;
            private Dictionary<node, node> idom;
            private Dictionary<node, node> samedom;
            private Dictionary<node, node> best;

            public Builder(DirectedGraph<node> graph, node root)
            {
                this.graph = graph;
                this.root = root;
                N = 0;
                bucket = graph.Nodes.ToDictionary(n => n, n => new HashSet<node>());
                dfnum = graph.Nodes.ToDictionary(n => n, n => 0);
                semi = graph.Nodes.ToDictionary(n => n, n => default(node));
                ancestor = graph.Nodes.ToDictionary(n => n, n => default(node));
                idom = graph.Nodes.ToDictionary(n => n, n => default(node));
                samedom = graph.Nodes.ToDictionary(n => n, n => default(node));
                best = graph.Nodes.ToDictionary(n => n, n => default(node));
            }

            void DFS(node p, node n)
            {
                if (dfnum[n] == 0)
                {
                    dfnum[n] = N;
                    vertex[N] = n;
                    parent[n] = p;
                    foreach (var w in graph.Successors(p))
                    {
                        DFS(n, w);
                    }
                }
            }


            public Dictionary<node, node> Dominators()
            {
                DFS(default(node), root);
                for (int i = N - 1; i > 0; --i)   // skip ove rroot node 0
                {
                    var n = vertex[i];
                    var p = parent[n];
                    var s = p;
                    foreach (var v in graph.Predecessors(n))
                    {
                        node ss;
                        if (dfnum[v] <= dfnum[n])
                        {
                            ss = v;
                        }
                        else
                        {
                            ss = semi[AncestorWithLowestSemi(v)];
                        }
                        if (dfnum[ss] < dfnum[s])
                            s = ss;
                    }
                    semi[n] = s;
                    bucket[s].Add(n);
                    Link(p, n);
                    foreach (var v in bucket[p])
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
                    bucket[p].Clear();
                }
                for (int i = 1; i < N; ++i)
                {
                    var n = vertex[i];
                    if (samedom[n] != null)
                    {
                        idom[n] = idom[samedom[n]];
                    }
                }
                return idom;
            }

            node AncestorWithLowestSemi(node v)
            {
                var a = ancestor[v];
                if (ancestor[a] != null)
                {
                    var b = AncestorWithLowestSemi(a);
                    ancestor[v] = ancestor[a];
                    if (dfnum[semi[b]] < dfnum[semi[best[v]]])
                        best[v] = b;
                }
                return best[v];
            }

            void Link(node p, node n)
            {
                ancestor[n] = p; best[n] = n;
            }
        }
    }
}
