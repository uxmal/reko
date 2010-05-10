/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
    /// <summary>
    /// Given an interval and its constituent nodes, finds a strongly connected component entirely within the interval. That
    /// SCC is a loop.
    /// </summary>
    public class SccLoopFinder 
    {
        private Interval interval;
        private HashedSet<StructureNode> intervalNodes;
        private HashedSet<StructureNode> loopNodeSet;

        public SccLoopFinder(Interval i, HashedSet<StructureNode> nodesInInterval)
        {
            this.interval = i;
            this.intervalNodes = nodesInInterval;
        }

        public HashedSet<StructureNode> FindLoop()
        {
            loopNodeSet = new HashedSet<StructureNode>();
            var f = new SccFinder<StructureNode>(new GraphAdapter(this), ProcessScc);
            f.Find(interval.Header);
            return loopNodeSet;
        }

        private class GraphAdapter : DirectedGraph<StructureNode>
        {
            private SccLoopFinder slf;

            public GraphAdapter(SccLoopFinder slf)
            {
                this.slf = slf;
            }
            
            #region DirectedGraph<StructureNode> Members

            public ICollection<StructureNode> Predecessors(StructureNode node)
            {
                throw new NotImplementedException();
            }

            public ICollection<StructureNode> Successors(StructureNode node)
            {
                var succ = new List<StructureNode>();
                foreach (StructureNode s in node.OutEdges)
                {
                    if (slf.IsNodeInInterval(s))
                        succ.Add(s);
                }
                return succ;
            }
        

            public ICollection<StructureNode> Nodes
            {
                get { throw new NotImplementedException(); }
            }

            public void AddEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                throw new NotImplementedException();
            }

            public void RemoveEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(StructureNode nodeFrom, StructureNode nodeTo)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private bool IsNodeInInterval(StructureNode s)
        {
            return intervalNodes.Contains(s);
        }

        private void ProcessScc(IList<StructureNode> scc)
        {
            if (scc.Count > 1 || (scc.Count == 1 && IsSelfLoop(scc[0])))
            {
                Dump(scc);
                loopNodeSet.AddRange(scc);
            }
        }

        private void Dump(ICollection<StructureNode> scc)
        {
            Dump(scc, Console.Out);
        }

        private void Dump(ICollection<StructureNode> scc, TextWriter writer)
        {
            writer.WriteLine("===");
            writer.Write("scc nodes:");
            foreach (StructureNode s in scc)
            {
                writer.Write(" {0} ", s.Name);
            }
            writer.WriteLine();
            writer.Write("accumulated loop nodes:");
            foreach (StructureNode s in loopNodeSet)
            {
                writer.Write(" {0} ", s.Name);
            }
            writer.WriteLine();
        }

        private bool IsSelfLoop(StructureNode node)
        {
            return node.OutEdges.Contains(node);
        }
    }

	/// <summary>
	/// Resolves the loop structure of an interval into its respective loop type (while, do/while)
	/// </summary>
    public class LoopFinder
    {
        private StructureNode header;
        private StructureNode latch;
        private List<StructureNode> order;
        
        public LoopFinder(StructureNode header, StructureNode latch, List<StructureNode> order)
        {
            if (latch == null)
                throw new InvalidOperationException("A loop must have a latch node.");

            this.header = header;
            this.latch = latch;
            this.order = order;
        }
        
        public Loop DetermineLoopType(HashedSet<StructureNode> loopNodes)
        {
            // if the latch node is a two way node then this must be a post tested loop
            if (latch.BlockType == bbType.cBranch)
            {
                header.Loop = CreatePostTestedLoop(loopNodes);
            }
            // otherwise it is either a pretested or endless loop
            else if (header.BlockType == bbType.cBranch)
            {
                if (loopNodes.Contains(header.OutEdges[0]) && !loopNodes.Contains(header.OutEdges[1]))
                    header.Loop = CreatePreTestedLoop(loopNodes);
                else if (loopNodes.Contains(header.OutEdges[1]) && !loopNodes.Contains(header.OutEdges[0]))
                    header.Loop = CreatePreTestedLoop(loopNodes);
                else
                    header.Loop = CreateTestlessLoop(loopNodes);
            }
            // both the header and latch node are one way nodes so this must be an endless loop
            else
            {
                header.Loop = CreateTestlessLoop(loopNodes);
            }
            return header.Loop;
        }

        private TestlessLoop CreateTestlessLoop(HashedSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindEndLessFollowNode(header, latch, loopNodes);
            TestlessLoop loop = new TestlessLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private PreTestedLoop CreatePreTestedLoop(HashedSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindPreTestedFollowNode(header);
            PreTestedLoop loop = new PreTestedLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private PostTestedLoop CreatePostTestedLoop(HashedSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindPostTestedFollowNode(header, latch);
            PostTestedLoop loop = new PostTestedLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private StructureNode FindEndLessFollowNode(StructureNode header, StructureNode latch, HashedSet<StructureNode> loopNodes)
        {
            StructureNode follow = null;
            // traverse the ordering array between the header and latch nodes.
            for (int i = header.Order - 1; i > latch.Order; i--)
            {
                // using intervals, the follow is determined to be the child outside the loop of a
                // 2 way conditional header that is inside the loop such that it (the child) has
                // the highest order of all potential follows
                StructureNode desc = order[i];

                if (desc.Conditional != null && desc.Conditional is Case && loopNodes.Contains(desc))
                {
                    for (int j = 0; j < desc.OutEdges.Count; j++)
                    {
                        StructureNode succ = desc.OutEdges[j];

                        // consider the current child 
                        if (succ != header && !loopNodes.Contains(succ) && (follow == null || succ.Order > follow.Order))
                            follow = succ;
                    }
                }
            }
            return follow;
        }


        /// <summary>
        /// Finds the follow node of a post tested ('repeat') loop. This is the node on the end of the
        /// non-back edge from the latch node
        /// </summary>
        /// <param name="header"></param>
        /// <param name="latch"></param>
        /// <returns>The follow node</returns>
        private StructureNode FindPostTestedFollowNode(StructureNode header, StructureNode latch)
        {
            if (latch.OutEdges[0] == header)
                return latch.OutEdges[1];
            else
                return latch.OutEdges[0];
        }

        /// <summary>
        /// The follow node of a pre-test ('while') loop is the child that is the loop header's 
        /// conditional follow.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private StructureNode FindPreTestedFollowNode(StructureNode header)
        {
            if (header.OutEdges[0] == header.Conditional.Follow)
                return header.OutEdges[0];
            else
                return header.OutEdges[1];
        }

        public HashedSet<StructureNode> FindNodesInLoop(HashedSet<StructureNode> intNodes)
        {
            SccLoopFinder finder = new SccLoopFinder(header.Interval, intNodes);
            return finder.FindLoop();
        }

        //$REVIEW: add to Loop class instead?
        public void TagNodesInLoop(Loop loop, HashedSet<StructureNode> loopNodes)
        {
            foreach (StructureNode node in loopNodes)
            {
                if (node.Loop == null)
                    node.Loop = loop;
            }
        }
    }
}
