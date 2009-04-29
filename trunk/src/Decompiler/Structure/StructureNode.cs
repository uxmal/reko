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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace Decompiler.Structure
{
	public class StructureNode
	{
        private int id;
        private Block entryBlock;
        private Interval interval;
        private int entryStamp, exitStamp;
        private int revEntryStamp, revExitStamp;
        private int ord;
        private int revOrd;
        
        private StructureNode immPDom;			    // immediate post dominator

        private StructureNode caseHead;     //$REFACTOR: These belong in a separate "conditional" or "loop" builder class.
        private StructureNode loopHead;
        private StructureNode condFollow;
        private StructureNode loopFollow;
        private StructureNode latchNode;
        private Loop loop;                      // the loop this node belongs to.

        // Structured type of the node      //$REFACTOR: this should be encapsulated in the StructuredGraph subclasses.
        private StructuredGraph sType;					// the structuring class (Loop, Cond , etc)
        private unstructType usType;				// the restructured type of a conditional header
        private loopType lType;					// the loop type of a loop header
        private condType cType;					// the conditional type of a conditional header

        private List<StructureNode> pred;
        private List<StructureNode> succ;

		public StructureNode(int i, Block entry)
		{
			this.id = i;
			this.entryBlock = entry;
			this.pred = new List<StructureNode>();
			this.succ = new List<StructureNode>();

            sType = StructuredGraph.Seq;
            usType = unstructType.Structured;
		}

        public void AddEdgeFrom(StructureNode src)
        {
            if (!Pred.Contains(src))
                Pred.Add(src);
        }

		public void AddEdgeTo(StructureNode to)
		{
			this.Succ.Add(to);
			to.Pred.Add(this);
		}

        // returns the type of the basic block that underlies this node.
        public bbType BlockType
        {
            get
            {
                Statement stm = entryBlock.Statements.Last;
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
        }

        /// <summary>
        /// The header of the most nested case statement of which this node is a member
        /// </summary>
        public StructureNode CaseHead
        {
            get { return caseHead; }
            set { caseHead = value; }
        }

        public condType CondType
        {
            get { return cType; }
            set { cType = value; }
        }

        public StructureNode Else { get { return succ[0]; } }

        public Block EntryBlock
        {
            get { return entryBlock; }
        }

        public int ExitStamp
        {
            get { return exitStamp; }
            set { exitStamp = value; }
        }

        public int EntryStamp
        {
            get { return entryStamp; }
            set { entryStamp = value; }
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
            return Succ.Contains(dest);
        }

        public bool HasABackEdge()
        {
            foreach (StructureNode s in Succ)
                if (HasBackEdgeTo(s))
                    return true;
            return false;
        }



        public int Ident
        {
            get { return id; }
        }


        // Return this node's immediate post dominator
        public StructureNode ImmPostDominator
        {
            get { return immPDom; }
            set { immPDom = value; }
        }

        // return the member instructions of this block/node
        public StatementList Instructions { get { return entryBlock.Statements; } }


        public StructureNode IntervalHead
		{
			get { return interval.HeaderNode; }
		}

        /// <summary>
        /// The interval to which this node belongs.
        /// </summary>
        public Interval Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /// <summary>
        /// Is this node an ancestor of <paramref name="other"/>?
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsAncestorOf(StructureNode other)
        {
            return ((EntryStamp < other.EntryStamp &&
                         ExitStamp > other.ExitStamp) ||
                        (RevEntryStamp < other.RevEntryStamp &&
                         RevExitStamp > other.RevExitStamp));
        }

        // Is this a latch node?
        public bool IsLatchNode
        {
            get { return loopHead != null && loopHead.latchNode == this; }
        }

        public Instruction LastInstruction
        {
            get
            {
                return entryBlock.Statements.Count > 0
                  ? entryBlock.Statements.Last.Instruction
                  : null;
            }
        }


        /// <summary>
        /// On a loop header, the latching node.        //$REFACTOR: this should be a member of a loop type.
        /// </summary>
        public StructureNode LatchNode
        {
            get { return latchNode; }
            set { latchNode = value; }
        }

        ///<summary>The header of the innermost loop this node belongs to.</summary>
        public StructureNode LoopHead
        {
            get { return loopHead; }
            set { loopHead = value; }
        }

        // Set the node that follows this loop
        // Return the node that follows this loop
        public StructureNode LoopFollow
        {
            get { return loopFollow; }
            set { loopFollow = value; }
        }

        public loopType LoopType
        {
            // Pre: this node must be a loop header and its loop type must be already set.
            // Return the loop type of this node
            get
            {
                Debug.Assert(sType == StructuredGraph.Loop || sType == StructuredGraph.LoopCond);
                return lType;
            }

            // Pre: the structured class of the node must be Loop or LoopCond
            // Set the loop type of this loop header node
            set
            {
                Debug.Assert(sType == StructuredGraph.Loop || sType == StructuredGraph.LoopCond);
                lType = value;

                //set the structured class (back to) just Loop if the loop type is PreTested OR
                //it's PostTested and is a single block loop
                if (lType == loopType.PreTested || (lType == loopType.PostTested && this == latchNode))
                    sType = StructuredGraph.Loop;
            }
        }

        /// <summary>
        /// The node that follows this conditional.
        /// </summary>
        public StructureNode CondFollow
        {
            get { return condFollow; }
            set { condFollow = value; }
        }


        /// <summary>
        /// Post-order numbering of this node. Can be used to index into the ProcHeader
        /// ordered array.
        /// </summary>
        public int Order
        {
            get
            {
                Debug.Assert(ord != -1);
                return ord;
            }
            set { ord = value; }
        }


        public List<StructureNode> Pred
        {
            get { return pred; }
        }


        public int RevEntryStamp
        {
            get { return revEntryStamp; }
            set { revEntryStamp = value; }
        }

        public int RevExitStamp
        {
            get { return revExitStamp; }
            set { revExitStamp = value; }
        }

        // Return the index of this node within the post dominator ordering array
        public int RevOrder
        {
            get
            {
                Debug.Assert(revOrd != -1);
                return revOrd;
            }
            set { revOrd = value; }
        }


        // Pre: if this is to be a cond type then the follow (if any) must have 
        // already been determined for this node
        // Set the class of structure determined for this node. 
        public void SetStructType(StructuredGraph s)
        {
            // if this is a conditional header, determine exactly which type of 
            // conditional header it is (i.e. switch, if-then, if-then-else etc.)
            if (s == StructuredGraph.Cond)
            {
                if (BlockType == bbType.nway)
                    cType = condType.Case;
                else if (Else == condFollow)
                    cType = condType.IfThen;
                else if (Then == condFollow)
                    cType = condType.IfElse;
                else
                    cType = condType.IfThenElse;
            }
            sType = s;
        }

        // Return the structured type of this node
        public StructuredGraph GetStructType() { return sType; }

        public bool IsStructType(StructuredGraph graph) { return sType == graph; }

        public List<StructureNode> Succ
        {
            get { return succ; }
        }

        public StructureNode Then { get { return succ[1]; } }

        // Return the restructured type of this node
        public unstructType UnstructType
        {
            get
            {
                Debug.Assert((sType == StructuredGraph.Cond || sType == StructuredGraph.LoopCond) && cType != condType.Case);
                return usType;
            }

            // Pre: this has already been structured as a two way conditional
            // Sets the restructured type of a two way conditional
            set
            {
                Debug.Assert((sType == StructuredGraph.Cond || sType == StructuredGraph.LoopCond) && cType != condType.Case);
                usType = value;
            }
        }



		public virtual void Write(TextWriter writer)
		{
			writer.WriteLine("node {0}: entry: \"{1}\"", Ident, EntryBlock.Name);
			writer.Write("    pred:");
			foreach (StructureNode p in pred)
			{
				writer.Write(" {0}", p.Ident);
			}
			writer.WriteLine();
            writer.WriteLine("    GraphType: {0}", this.sType.GetType().Name);
            sType.WriteDetails(this, writer);
			writer.Write("    succ: ");
			foreach (StructureNode s in succ)
			{
				writer.Write(" {0}", s.Ident);
			}

			writer.WriteLine();
		}
	}

    // an enumerated type for the type of loop headers
    public class loopType
    {
        public static loopType PreTested = new loopType();				// Header of a while loop
        public static loopType PostTested = new loopType();			// Header of a repeat loop
        public static loopType Endless = new loopType(); // Header of an endless loop
    }

    // an type for the class of unstructured conditional jumps
    public enum unstructType
    {
        Structured,
        JumpInOutLoop,
        JumpIntoCase
    }


    // an enumerated type for the type of conditional headers
    public enum condType
    {
        IfThen,				// conditional with only a then clause
        IfThenElse,			// conditional with a then and an else clause
        IfElse,				// conditional with only an else clause
        Case				// bbType.nway conditional header (case statement)
    }

    public enum bbType
    {
        none,
        cBranch,
        fall,
        nway,
        uBranch,
        ret,
    }

}
