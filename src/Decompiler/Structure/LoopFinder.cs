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
        private bool[] nodesInInterval;
        private IList<StructureNode> loopNodes;

        public SccLoopFinder(IntNode i, bool[] nodesInInterval)
        {
            this.interval = i;
            this.nodesInInterval = nodesInInterval;
        }

        public IList<StructureNode> FindLoop()
        {
            loopNodes = new List<StructureNode>();
            SccFinder<StructureNode> f = new SccFinder<StructureNode>(this);
            f.Find(interval.Nodes[0]);
            return loopNodes;
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
                if (nodesInInterval[s.Order])
                    yield return s;
            }
        }

        void ISccFinderHost<StructureNode>.ProcessScc(IList<StructureNode> scc)
        {
            if (scc.Count > 1 || (scc.Count == 1 && IsSelfLoop(scc[0])))
            {
                if (loopNodes.Count > 0)
                    throw new NotSupportedException("Multiple loops in an interval not supported.");
                loopNodes = scc;
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
        
        public LoopFinder(StructureNode header)
        {
            this.header = header;
        }
        
        public void DetermineLoopType()
        {
            // Pre: The loop induced by (head,latch) has already had all its member nodes tagged
            // Post: The type of loop has been deduced
            Debug.Assert(header.LatchNode != null);

            // if the latch node is a two way node then this must be a post tested loop
            if (header.LatchNode.BlockType == bbType.cBranch)
            {
                header.SetLoopType(loopType.PostTested);

                // if the head of the loop is a two way node and the loop spans more than one block
                // then it must also be a conditional header
                if (header.BlockType == bbType.cBranch && header != header.LatchNode)
                    header.SetStructType(structType.LoopCond);
            }

            // otherwise it is either a pretested or endless loop
            else if (header.BlockType == bbType.cBranch)
            {
                // if the header is a two way conditional header, then it will be a pretested loop
                // if one of its children is its conditional follow
                if (header.OutEdges[0] != header.CondFollow && header.OutEdges[1] != header.CondFollow)
                {
                    // neither children are the conditional follow
                    header.SetLoopType(loopType.Endless);

                    // retain the fact that this is also a conditional header
                    header.SetStructType(structType.LoopCond);
                }
                else
                    // one child is the conditional follow
                    header.SetLoopType(loopType.PreTested);

            }
            // both the header and latch node are one way nodes so this must be an endless loop
            else
            {
                header.SetLoopType(loopType.Endless);
            }
        }

        public void FindLoopFollow(List<StructureNode> order, bool[] loopNodes)
        // Pre: The loop headed by header has been induced and all it's member nodes have been tagged
        // Post: The follow of the loop has been determined.
        {
            Debug.Assert(header.GetStructType() == structType.Loop || header.GetStructType() == structType.LoopCond);
            loopType lType = header.GetLoopType();
            StructureNode latch = header.LatchNode;

            if (lType == loopType.PreTested)
            {
                // the child that is the loop header's conditional follow will be the loop follow
                if (header.OutEdges[0] == header.CondFollow)
                    header.LoopFollow = header.OutEdges[0];
                else
                    header.LoopFollow = header.OutEdges[1];
            }
            else if (lType == loopType.PostTested)
            {
                // the follow of a post tested ('repeat') loop is the node on the end of the
                // non-back edge from the latch node
                if (latch.OutEdges[0] == header)
                    header.LoopFollow = latch.OutEdges[1];
                else
                    header.LoopFollow = latch.OutEdges[0];
            }
            else // endless loop
            {
                StructureNode follow = null;
                // traverse the ordering array between the header and latch nodes.
                latch = header.LatchNode;
                for (int i = header.Order - 1; i > latch.Order; i--)
                {
                    // using intervals, the follow is determined to be the child outside the loop of a
                    // 2 way conditional header that is inside the loop such that it (the child) has
                    // the highest order of all potential follows
                    StructureNode desc = order[i];

                    if (desc.GetStructType() == structType.Cond && desc.Conditional!= Conditional.Case && loopNodes[desc.Order])
                    {
                        for (int j = 0; j < desc.OutEdges.Count; j++)
                        {
                            StructureNode succ = desc.OutEdges[j];

                            // consider the current child 
                            if (succ != header && !loopNodes[succ.Order] && (follow == null || succ.Order > follow.Order))
                                follow = succ;
                        }
                    }
                }

                // if a follow was found, assign it to be the follow of the loop under investigation
                if (follow != null)
                    header.LoopFollow = follow;
            }
        }

        public  void TagNodesInLoop(List<StructureNode> order, bool[] intNodes, bool[] loopNodes)
        // Pre: header has been detected as a loop header and has the details of the latching node
        // Post: the nodes within the loop have been tagged (if they weren't already within a more
        //       deeply nested loop) and are within the returned set of nodes
        {
            SccLoopFinder finder = new SccLoopFinder(header.Interval, intNodes);
            foreach (StructureNode node in finder.FindLoop())
            {
                loopNodes[node.Order] = true;
                if (node.LoopHead == null)
                    node.LoopHead = header;
            }
        }

    }
}
