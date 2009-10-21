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
        
        public LoopFinder(StructureNode header)
        {
            this.header = header;
        }
        
        public void DetermineLoopType(HashSet<StructureNode> loopNodes)
        {
            if (header.LatchNode == null)
                throw new InvalidOperationException("Header node must have determined a latch node.");

            // if the latch node is a two way node then this must be a post tested loop
            if (header.LatchNode.BlockType == bbType.cBranch)
            {
                header.SetLoopType(new PostTestedLoop(header, header.LatchNode, loopNodes));

                // if the head of the loop is a two way node and the loop spans more than one block
                // then it must also be a conditional header
                if (header.BlockType == bbType.cBranch && header != header.LatchNode)
                    header.SetStructType(structType.LoopCond);
            }
            // otherwise it is either a pretested or endless loop
            else if (header.BlockType == bbType.cBranch)
            {
                header.SetLoopType(new PreTestedLoop(header, header.LatchNode, loopNodes));
                bool WORKING = false;
                if (WORKING)
                {
                    // This code fails for while_goto because the condfollow doesn't coincide with the end of the loop.

                // if the header is a two way conditional header, then it will be a pretested loop
                // if one of its children is its conditional follow
                if (header.OutEdges[0] != header.CondFollow && header.OutEdges[1] != header.CondFollow)
                {
                    // neither children are the conditional follow
                    header.SetLoopType(new EndLessLoop(header, header.LatchNode, loopNodes));

                    // retain the fact that this is also a conditional header
                    header.SetStructType(structType.LoopCond);
                }
                else
                    // one child is the conditional follow
                    header.SetLoopType(new PreTestedLoop(header, header.LatchNode, loopNodes));
                }
            }
            // both the header and latch node are one way nodes so this must be an endless loop
            else
            {
                header.SetLoopType(new EndLessLoop(header, header.LatchNode, loopNodes));
            }
        }

        public StructureNode FindLoopFollow(List<StructureNode> order, HashSet<StructureNode> loopNodes)
        {
            Debug.Assert(header.GetStructType() == structType.Loop || header.GetStructType() == structType.LoopCond);
            return header.GetLoopType().FindFollowNode(header, header.LatchNode, loopNodes, order);
        }

        public HashSet<StructureNode> TagNodesInLoop(List<StructureNode> nodes, HashSet<StructureNode> intNodes)
        {
            SccLoopFinder finder = new SccLoopFinder(header.Interval, intNodes);
            HashSet<StructureNode> loopNodes = finder.FindLoop();
            foreach (StructureNode node in loopNodes)
            {
                if (node.LoopHead == null)
                    node.LoopHead = header;
            }
            return loopNodes;
        }
    }
}
