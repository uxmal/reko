#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace Decompiler.Structure
{
    internal enum travType
    {
        UNTRAVERSED,
        DFS_TAG,
        DFS_LNUM,
        DFS_RNUM,
    }

    public class StructureNode : GraphNode<StructureNode>
    {
        internal travType traversed;          //$REFACTOR: use visited hashtables instead.

        private UnstructuredType usType;

        private int[] loopStamps;
        private int[] revLoopStamps;

        public StructureNode(Block block, int id)
        {
            if (block == null)
                throw new ArgumentNullException("block");
            this.Block = block;
            this.Number = id;

            Order = -1;

            traversed = travType.UNTRAVERSED;
            ForceLabel = false;

            ImmPDom = null;
            CaseHead = null;

            usType = UnstructuredType.Structured;
            Interval = null;

            //initialize the two timestamp tuples
            loopStamps = new int[2];
            revLoopStamps = new int[2];
            for (int i = 0; i < 2; i++)
                loopStamps[i] = revLoopStamps[i] = -1;

            BlockType = TypeOfBlock(block);
        }

        // Constructor used by the IntNode derived class
        protected StructureNode(int newId, BlockTerminationType t)
        {
            Number = newId; BlockType = t;
        }

        // Add an edge from this node to dest. If this is a cBranch type of node and it already
        // has an edge to dest then node edge is added and the node type is changed to fall
        public void AddEdgeTo(StructureNode dest)
        {
            if (BlockType != BlockTerminationType.Branch || !HasEdgeTo(dest))
                OutEdges.Add(dest);    
            else
                //reset the type to fall if no edge was added (i.e. this edge already existed)
                BlockType = BlockTerminationType.FallThrough;
        }

        // Add an edge from src to this node if it doesn't already exist. NB: only interval
        // nodes need this routine as the in edges for normal nodes are built in SetLoopStamps
        public void AddEdgeFrom(StructureNode src)
        {
            if (!InEdges.Contains(src))
                InEdges.Add(src);
        }


        public Block Block { get; private set; }
        public BlockTerminationType BlockType { get; private set; }
        public StructureNode CaseHead { get; set; }
        public Conditional Conditional { get; set; }
        public bool ForceLabel { get; set; }
        public int Number { get; private set; }
        public StructureNode Then { get { return OutEdges[1]; } }
        public StructureNode Else { get { return OutEdges[0]; } }

        public UnstructuredType UnstructType
        {
            get { return usType; }
            set
            {
                Debug.Assert(Conditional != null && !(Conditional is Case));
                usType = value;
            }
        }

        // Do a DFS on the graph headed by this node, simply tagging the nodes visited.  //$move to GraphNode.
        public void DfsTag()
        {
            traversed = travType.DFS_TAG;
            for (int i = 0; i < OutEdges.Count; i++)
                if (OutEdges[i].traversed != travType.DFS_TAG)
                    OutEdges[i].DfsTag();
        }


        /// <summary>
        /// Returns true of this node has a back edge to <paramref name="dest"/>.
        /// </summary>
        /// <param name="dest">Node whose back-edgeness is to be tested.</param>
        /// <returns>True of <paramref name="dest"/> is reachable via a back edge from this node.</returns>
        public bool HasBackEdgeTo(StructureNode dest)
        {
            Debug.Assert(HasEdgeTo(dest) || dest == this);
            return (dest == this || dest.IsAncestorOf(this));
        }


        public bool HasABackEdge()
        {
            for (int i = 0; i < this.OutEdges.Count; i++)
                if (this.HasBackEdgeTo(this.OutEdges[i]))
                    return true;
            return false;
        }

        /// <summary>
        /// Does this node have an edge to dest?
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool HasEdgeTo(StructureNode dest)
        {
            return OutEdges.Contains(dest);
        }



        /// <summary>
        /// The immediate postdominator of this node (if it exists)
        /// </summary>
        public StructureNode ImmPDom {get;set; }


        public StatementList Instructions
        {
            get { return Block.Statements; }
        }

        public Interval Interval { get; set; }

        /// <summary>
        /// Is this node an ancestor of <paramref name="other"/>?
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsAncestorOf(StructureNode other)
        {
            return ((loopStamps[0] < other.loopStamps[0] &&
                         loopStamps[1] > other.loopStamps[1]) ||
                        (revLoopStamps[0] < other.revLoopStamps[0] &&
                         revLoopStamps[1] > other.revLoopStamps[1]));
        }

        public bool IsLatchNode()
        {
            return (Loop != null && Loop.Latch == this);
        }

        public bool IsLoopHeader()
        {
            return Loop != null && Loop.Header == this;
        }


        ///<summary>
        ///The innermost loop this node belongs to.
        ///</summary>
        public Loop Loop { get; set; } 

        public virtual string Name
        {
            get { return Block.Name; }
        }

        /// <summary>
        /// The index of this node within the ordering array.
        /// </summary>
        public int Order { get; set; }

        // Do a DFS on the graph headed by this node, giving each node its time stamp tuple
        // that will be used for loop structuring as well as building the structure that will
        // be used for traversing the nodes in linear time. The inedges are also built during
        // this traversal.
        public void SetLoopStamps(ref int time, List<StructureNode> order)
        {
            //timestamp the current node with the current time and set its traversed flag
            traversed = travType.DFS_LNUM;
            loopStamps[0] = time;

            //recurse on unvisited children and set inedges for all children
            for (int i = 0; i < OutEdges.Count; i++)
            {
                // set the in edge from this child to its parent (the current node)
                OutEdges[i].InEdges.Add(this);

                // recurse on this child if it hasn't already been visited
                if (OutEdges[i].traversed != travType.DFS_LNUM)
                {
                    ++time;
                    OutEdges[i].SetLoopStamps(ref time, order);
                }
            }

            //set the the second loopStamp value
            loopStamps[1] = ++time;

            //add this node to the ordering structure as well as recording its position within the ordering
            Order = order.Count;
            order.Add(this);
        }

        // Sets the reverse loop stamps for each node. The children are traversed in
        // reverse order.
        public void SetRevLoopStamps(ref int time, HashSet<StructureNode> visited)
        {
            //timestamp the current node with the current time and set its traversed flag
            visited.Add(this);
            revLoopStamps[0] = time;

            //recurse on the unvisited children in reverse order
            for (int i = OutEdges.Count - 1; i >= 0; i--)
            {
                // recurse on this child if it hasn't already been visited
                if (!visited.Contains(OutEdges[i]))
                {
                    ++time;
                    OutEdges[i].SetRevLoopStamps(ref time, visited);
                }
            }

            //set the the second loopStamp value
            revLoopStamps[1] = ++time;
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        private BlockTerminationType TypeOfBlock(Block block)
        {
            Statement stm = block.Statements.Last;
            if (stm == null)
                return BlockTerminationType.FallThrough;
            Instruction i = stm.Instruction;
            if (i is Branch)
                return BlockTerminationType.Branch;
            if (i is SwitchInstruction)
                return BlockTerminationType.Multiway;
            if (i is ReturnInstruction)
                return BlockTerminationType.Return;
            if (i is GotoInstruction)
                return BlockTerminationType.Goto;
            return BlockTerminationType.FallThrough;
        }

        public virtual void Write(TextWriter tw)
        {
            tw.Write("{0} ({1})", Block.Name, Number);
        }
    }

    public enum BlockTerminationType
    {
        None,
        FallThrough,
        Goto,
        Return,
        Branch,
        Multiway,
        IntervalNode
    }

    public enum UnstructuredType
    {
        None,
        Structured,
        JumpInOutLoop,
        JumpIntoCase
    }
}
