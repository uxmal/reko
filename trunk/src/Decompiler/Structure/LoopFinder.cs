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
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Structure
{
    public class SccLoopFinder : ISccFinderHost<StructureNode>
    {
        private IntNode interval;
        private HashSet<StructureNode> intervalNodes;
        private HashSet<StructureNode> loopNodeSet;

        public SccLoopFinder(IntNode i, HashSet<StructureNode> nodesInInterval)
        {
            this.interval = i;
            this.intervalNodes = nodesInInterval;
        }

        public HashSet<StructureNode> FindLoop()
        {
            loopNodeSet = new HashSet<StructureNode>();
            SccFinder<StructureNode> f = new SccFinder<StructureNode>(this);
            f.Find(interval.Nodes[0]);
            return loopNodeSet;
        }

        #region ISccFinderHost<CFGNode> Members

        void ISccFinderHost<StructureNode>.AddSuccessors(StructureNode t, ICollection<StructureNode> succ)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerable<StructureNode> ISccFinderHost<StructureNode>.GetSuccessors(StructureNode t)
        {
            foreach (StructureNode s in t.OutEdges)
            {
                if (IsNodeInInterval(s))
                    yield return s;
            }
        }

        private bool IsNodeInInterval(StructureNode s)
        {
            return intervalNodes.Contains(s);
        }

        void ISccFinderHost<StructureNode>.ProcessScc(IList<StructureNode> scc)
        {
            if (scc.Count > 1 || (scc.Count == 1 && IsSelfLoop(scc[0])))
            {
                if (loopNodeSet.Count > 0)
                    throw new NotSupportedException("Multiple loops in an interval not supported.");
                loopNodeSet.AddRange(scc);
            }
        }

        private bool IsSelfLoop(StructureNode node)
        {
            return node.OutEdges.Contains(node);
        }

        #endregion
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
        
        public Loop DetermineLoopType(HashSet<StructureNode> loopNodes)
        {

            // if the latch node is a two way node then this must be a post tested loop
            if (latch.BlockType == bbType.cBranch)
            {
                header.SetLoopType(CreatePostTestedLoop(loopNodes));

                // if the head of the loop is a two way node and the loop spans more than one block
                // then it must also be a conditional header
                if (header.BlockType == bbType.cBranch && header != latch)
                    header.SetStructType(structType.LoopCond);
            }
            // otherwise it is either a pretested or endless loop
            else if (header.BlockType == bbType.cBranch)
            {
                header.SetLoopType(CreatePreTestedLoop(loopNodes));
                bool WORKING = false;
                if (WORKING)
                {
                    // This code fails for while_goto because the condfollow doesn't coincide with the end of the loop.

                // if the header is a two way conditional header, then it will be a pretested loop
                // if one of its children is its conditional follow
                if (header.OutEdges[0] != header.Conditional.Follow && header.OutEdges[1] != header.Conditional.Follow)
                {
                    // neither children are the conditional follow
                    header.SetLoopType(CreateEndLessLoop(loopNodes));

                    // retain the fact that this is also a conditional header
                    header.SetStructType(structType.LoopCond);
                }
                else
                    // one child is the conditional follow
                    header.SetLoopType(CreatePreTestedLoop(loopNodes));
                }
            }
            // both the header and latch node are one way nodes so this must be an endless loop
            else
            {
                header.SetLoopType(CreateEndLessLoop(loopNodes));
            }
            return header.GetLoopType();
        }

        private EndLessLoop CreateEndLessLoop(HashSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindEndLessFollowNode(header, latch, loopNodes);
            EndLessLoop loop = new EndLessLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private PreTestedLoop CreatePreTestedLoop(HashSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindPreTestedFollowNode(header);
            PreTestedLoop loop = new PreTestedLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private PostTestedLoop CreatePostTestedLoop(HashSet<StructureNode> loopNodes)
        {
            StructureNode follow = FindPostTestedFollowNode(header, latch);
            PostTestedLoop loop = new PostTestedLoop(header, latch, loopNodes, follow);
            return loop;

        }

        private StructureNode FindEndLessFollowNode(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes)
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


        private StructureNode FindPostTestedFollowNode(StructureNode header, StructureNode latch)
        {
            // the follow of a post tested ('repeat') loop is the node on the end of the
            // non-back edge from the latch node
            if (latch.OutEdges[0] == header)
                return latch.OutEdges[1];
            else
                return latch.OutEdges[0];
        }

        private StructureNode FindPreTestedFollowNode(StructureNode header)
        {
            // the child that is the loop header's conditional follow will be the loop follow
            if (header.OutEdges[0] == header.Conditional.Follow)
                return header.OutEdges[0];
            else
                return header.OutEdges[1];
        }

        public HashSet<StructureNode> FindNodesInLoop(HashSet<StructureNode> intNodes)
        {
            SccLoopFinder finder = new SccLoopFinder(header.Interval, intNodes);
            return finder.FindLoop();
        }

        //$REVIEW: add to Loop class instead?
        public void TagNodesInLoop(Loop loop, HashSet<StructureNode> loopNodes)
        {
            foreach (StructureNode node in loopNodes)
            {
                if (node.Loop == null)
                    node.Loop = loop;
            }
        }
    }
}
