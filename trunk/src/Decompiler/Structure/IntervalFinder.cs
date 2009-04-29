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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
    /// <summary>
    /// Builder class that finds all the intervals of a procedure, resulting in an
    /// IntervalCollection.
    /// </summary>
    public class IntervalFinder
    {
        private IntervalCollection intervals;
        private Interval[] intervalOf;

        public IntervalFinder(Procedure proc)
        {
            ComputeIntervals(proc.EntryBlock, proc.RpoBlocks);
        }

        private void ComputeIntervals(Block entry, List<Block> blocks)
        {
            intervals = new IntervalCollection(blocks.Count);
            intervalOf = new Interval[blocks.Count];

            Dictionary<Block, Block> processed = new Dictionary<Block, Block>();
            WorkList<Block> wlIntHeaders = new WorkList<Block>();		// Possible interval headers
            WorkList<Block> wlIntMembers = new WorkList<Block>();		// Known interval members.
            int[] linksFollowed = new int[blocks.Count];

            wlIntHeaders.Add(entry);
            Block h;
            while (wlIntHeaders.GetWorkItem(out h))
            {
                Interval interval = new Interval(intervals.Count, h);
               // interval.AddBlock(h);
                intervalOf[h.RpoNumber] = interval;

                wlIntMembers.Add(h);
                intervals.Add(interval);

                Block n;
                while (wlIntMembers.GetWorkItem(out n))
                {
                    // n is known to be in the interval; see if the successors also are in the interval.

                    foreach (Block s in n.Succ)
                    {
                        ++linksFollowed[s.RpoNumber];		// followed another predecessor link.
                        if (intervalOf[s.RpoNumber] == null)
                        {
                            if (linksFollowed[s.RpoNumber] == s.Pred.Count)
                            {
                                // This is the only node that reaches this interval, so
                                // it has to be a member of the interval.
                                // Therefore, we remove it from the list of potential headers
                                wlIntHeaders.Remove(s);

                                // And add it to this interval.
                                //interval.AddBlock(s);
                                wlIntMembers.Add(s);
                                intervalOf[s.RpoNumber] = interval;
                            }
                            else if (!processed.ContainsKey(s))
                            {
                                processed[s] = s;

                                // s may be reached by another node. Therefore, s is a possible new header.
                                // Add it to the interval work list.

                                wlIntHeaders.Add(s);
                            }
                        }
                        else
                        {
                            if (linksFollowed[s.RpoNumber] == s.Pred.Count &&
                                intervalOf[s.RpoNumber] == interval &&
                                intervalOf[s.RpoNumber].HeaderBlock == h)
                            {
                                if (s != h)  // Don't add the back edge to header again, since it's already in the interval.
                                {
                                    //interval.AddBlock(s);
                                    intervalOf[s.RpoNumber] = interval;
                                    wlIntHeaders.Remove(s);
                                    wlIntMembers.Add(s);
                                }
                            }
                        }
                    }
                }
            }
        }


        public Interval IntervalOf(Block b)
        {
            return intervalOf[b.RpoNumber];
        }

        public IntervalCollection Intervals
        {
            get { return intervals; }
        }

        public bool IsIntervalHeader(Block b)
        {
            return intervalOf[b.RpoNumber].HeaderBlock == b;
        }
    }

#if NOWAY

    public class IntervalFinder2
    {
        private DisjointPartition<Vertex> LP;
        private DisjointPartition<Vertex> RLH;
        private Graph G;

        public IntervalFinder2()
        {
        }
        public void MarkIrreducibleLoops(Vertex z)
        {
            Vertex t = z.LoopParent;
            while (t != null)
            {
                Vertex u = RLH.Find(t);
                u.IrreducibleLoopHeader = true; ;
                t = u.LoopParent;
                if (t != null)
                {
                    RLH.Union(u, t);
                }
            }
        }

        public void ProcessCrossFwdEdges(Vertex x)
        {
            foreach (Edge e in x.CrossFwdEges)
            {
                G.AddEdge(LP.Find(e.from), LP.Find(e.to));
                MarkIrreducibleLoops(e.to);
            }
        }

        public class Graph
        {
            private List<Vertex> vertices;
            private List<Edge> edges;

            public ICollection<Vertex> Vertices
            {
                get { return Vertices; }
            }

            public ICollection<Edge> Edges
            {
                get { return edges; }
            }

        }
        public class Vertex
        {
            public Vertex LoopParent;
            public List<Edge> CrossFwdEges;
            public bool IrreducibleLoopHeader;
        }

        public class Edge
        {
            public Vertex from;
            public Vertex to;
        }

        public void FindIntervals(object G)
        {
            DFS(G);
            foreach (Vertex x in G.Vertices)
            {
                x.LoopParent = null;
                x.CrossFwdEdges = new List<Edge>();
                LP.Add(x);
                RLH.Add(x);
            }

            foreach (Edge y_to_x in G.Edges)
            {
                if (ForwardEdge(y_to_x) || CrossEdge(y_to_x))
                {
                    G.Remove(y_to_x);
                    object a = LeastCommonAncestor(y, x);
                    a.CrossFwrdEdges.Add(y_to_x);
                }
                foreach (Vertex x in G.ReverseDfs())
                {
                    ProcessCrossFwdEdges(x);
                    FindLoop(x);
                }
            }
        }

        // http://cag.csail.mit.edu/~thies/6.046-web/recitation9.txt
        private void DFS(object G)
        {
            foreach (Vertex u in G.Vertices)
            {
                u.Color = WHITE;
                u.pi = null;
            }
            time = 0;
            foreach (object u in Vertices(G))
            {
                if (Color(u) == WHITE)
                {
                    DFS_VISIT(u);
                }
            }
        }

        private void DFS_VISIT(Vertex u)
        {
            color[u] = GRAY;
            time = time + 1;
            d[u] = time;
            foreach (object v in Successors(u))
            {
                if (color[v] == WHITE)
                {
                    pi[v] = u;
                    DFS_VISIT(v);
                }
            }
            color[u] = BLACK;
            f[u] = time + 1;
            time = time + 1;
        }

        /*
         Edge Classification

<< add edges to forest, labeling as you go >>

1. Tree edge:  encounter new (white) vertex
   - gray to white
2. Back edge: from descendent to ancestor
   - gray to gray
3. Forward edge:  nontree edge from ancestor to descendent
   - gray to black
4. cross edge:  remainder - betweeen trees or sub-trees
   - gray to black

         */
        public void FindLoop(object potentialHeader)
        {
            ArrayList loopBody = new ArrayList();
            WorkList worklist = new WorkList();
            // add set { LP.Find(y) | y -> potentialHeader is a backedge } - { potentialHeader}
            while (!worklist.IsEmpty)
            {
                Vertex y = worklist.GetWorkItem();
                loopBody.Add(y);
                foreach (Vertex z in y.Predecessors)
                {
                    if (!IsBackEdge(z, y))
                    {
                        object set = MakeSet(loopBody);
                        AddSet(set, potentialHeader);
                        AddSet(set, worklist);
                        if (!set.Contains(LP.Find(z)))
                            worklist.Add(z);
                    }
                }
            }
            if (loopBody.Count > 0)
            {
                Collapse(loopBody, potentialHeader);
            }
        }

        private void Collapse(ICollection<Vertex> loopBody, Vertex loopHeader)
        {
            foreach (Vertex z in loopBody)
            {
                z.LoopParent = loopHeader;
                LP.Union(z, loopHeader);
            }
        }

    }
#endif
}