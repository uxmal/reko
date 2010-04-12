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
        private Block block;
        private int id;
        private int ord;

        internal travType traversed;          //$REFACTOR: use visited hashtables instead.
        private bool forceLabel;

        private StructureNode immPDom;
        private StructureNode caseHead;

        private UnstructuredType usType;
        private Conditional cond;
        private Loop loop;
        private Interval interval;

        private int[] loopStamps;
        private int[] revLoopStamps;

        private bbType type;

        public StructureNode(Block block, int id)
        {
            if (block == null)
                throw new ArgumentNullException("block");
            this.block = block;
            this.id = id;

            ord = -1;

            traversed = travType.UNTRAVERSED;
            forceLabel = false;

            immPDom = null;
            caseHead = null;

            usType = UnstructuredType.Structured;
            interval = null;

            //initialize the two timestamp tuples
            loopStamps = new int[2];
            revLoopStamps = new int[2];
            for (int i = 0; i < 2; i++)
                loopStamps[i] = revLoopStamps[i] = -1;


            type = TypeOfBlock(block);
        }



        // Constructor used by the IntNode derived class
        protected StructureNode(int newId, bbType t)
        {
            id = newId; type = t;
        }

        // Add an edge from this node to dest. If this is a cBranch type of node and it already
        // has an edge to dest then node edge is added and the node type is changed to fall
        public void AddEdgeTo(StructureNode dest)
        {
            if (type != bbType.cBranch || !HasEdgeTo(dest))
                OutEdges.Add(dest);    
            else
                //reset the type to fall if no edge was added (i.e. this edge already existed)
                type = bbType.fall;
        }

        // Add an edge from src to this node if it doesn't already exist. NB: only interval
        // nodes need this routine as the in edges for normal nodes are built in SetLoopStamps
        public void AddEdgeFrom(StructureNode src)
        {
            if (!InEdges.Contains(src))
                InEdges.Add(src);
        }


        public Block Block
        {
            get { return block; }
        }

        public bbType BlockType
        {
            get { return type; }
        }

        public StructureNode CaseHead
        {
            get { return caseHead; }
            set { caseHead = value; }
        }

        public Conditional Conditional
        {
            get { return cond; }
            set { cond = value; }
        }


        // Do a DFS on the graph headed by this node, simply tagging the nodes visited.  //$move to GraphNode.
        public void DfsTag()
        {
            traversed = travType.DFS_TAG;
            for (int i = 0; i < OutEdges.Count; i++)
                if (OutEdges[i].traversed != travType.DFS_TAG)
                    OutEdges[i].DfsTag();
        }

        public StructureNode Then { get { return OutEdges[1]; } }

        public StructureNode Else { get { return OutEdges[0]; } }

        public bool ForceLabel
        {
            get { return forceLabel; }
            set { forceLabel = value; }
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

        // Does this node have an edge to dest?
        public bool HasEdgeTo(StructureNode dest)
        {
            return OutEdges.Contains(dest);
        }


        public int Ident() { return id; }

        public StructureNode ImmPDom
        {
            get { return immPDom; }
            set { immPDom = value; }
        }


        public StatementList Instructions
        {
            get { return block.Statements; }
        }

        public Interval Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        // Is this node an ancestor of other?
        public bool IsAncestorOf(StructureNode other)
        {
            return ((loopStamps[0] < other.loopStamps[0] &&
                         loopStamps[1] > other.loopStamps[1]) ||
                        (revLoopStamps[0] < other.revLoopStamps[0] &&
                         revLoopStamps[1] > other.revLoopStamps[1]));
        }

        public bool IsLatchNode()
        {
            return (loop != null && loop.Latch == this);
        }

        public bool IsLoopHeader()
        {
            return Loop != null && Loop.Header == this;
        }


        ///<summary>
        ///The innermost loop this node belongs to.
        ///</summary>
        public Loop Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public virtual string Name
        {
            get { return block.Name; }
        }

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
            ord = order.Count;
            order.Add(this);
        }

        // Sets the reverse loop stamps for each node. The children are traversed in
        // reverse order.
        public void SetRevLoopStamps(ref int time, HashedSet<StructureNode> visited)
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

        /// <summary>
        /// The index of this node within the ordering array.
        /// </summary>
        public int Order
        {
            get { return ord; }
            set { ord = value; }
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        private bbType TypeOfBlock(Block block)
        {
            Statement stm = block.Statements.Last;
            if (stm == null)
                return bbType.fall;
            Instruction i = stm.Instruction;
            if (i is Branch)
                return bbType.cBranch;
            if (i is SwitchInstruction)
                return bbType.nway;
            if (i is ReturnInstruction)
                return bbType.ret;
            return bbType.fall;
        }

        public UnstructuredType UnstructType
        {
            get { return usType; }
            set
            {
                Debug.Assert(Conditional != null && !(cond is Case));
                usType = value;
            }
        }


        public virtual void Write(TextWriter tw)
        {
            tw.Write("{0} ({1})", Block.Name, Ident());
        }

    }

    public enum bbType
    {
        none,
        cBranch,
        fall,
        nway,
        uBranch,
        ret,
        intNode
    }

    public enum UnstructuredType
    {
        None,
        Structured,
        JumpInOutLoop,
        JumpIntoCase
    }
}
