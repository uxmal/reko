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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    public class UnstructuredConditionalAnalysis
    {
        private ProcedureStructure curProc;

        public UnstructuredConditionalAnalysis(ProcedureStructure curProc)
        {
            this.curProc = curProc;
        }

        /// <summary>
        /// This routine is called after all the other structuring has been done. It detects
        /// conditionals that are in fact the head of a jump into/out of a loop or into a case body. 
        /// Only forward jumps are considered as unstructured backward jumps will always be generated nicely.
        /// </summary>
        /// <param name="curProc"></param>
        public void Adjust()
        {
            foreach (StructureNode node in curProc.Ordering)
            {
                Loop myLoop = node.Loop;
                if (IsTwoWayBranchWithFollow(node))
                {
                    Loop follLoop = node.Conditional.Follow.Loop;
                    // analyze whether this is a jump into/outof a loop
                    if (myLoop != follLoop)
                    {
                        // we want to find the branch that the latch node is on for a jump out of a loop
                        if (myLoop != null)
                        {
                            SetUnstructuredLoopTransfer(node, myLoop.Latch);
                        }

                        if (follLoop != null)
                        {
                            if (node.UnstructType == UnstructuredType.Structured)
                            // find the branch that the loop head is on for a jump into a loop body. If a branch has
                            // already been found, then it will match this one anyway
                            {
                                SetUnstructuredLoopTransfer(node, follLoop.Header);
                            }
                        }
                    }

                    // this is a jump into a case body if either of its children don't have the same
                    // same case header as itself
                    if (IsJumpIntoCaseBody(node))
                    {
                        StructureNode myCaseHead = node.CaseHead;
                        StructureNode thenCaseHead = node.Then.CaseHead;
                        StructureNode elseCaseHead = node.Else.CaseHead;
                        if (thenCaseHead == myCaseHead && (myCaseHead == null || elseCaseHead != myCaseHead.Conditional.Follow))
                        {
                            node.UnstructType = UnstructuredType.JumpIntoCase;
                            node.Conditional = new IfElse(null);
                        }
                        else if (elseCaseHead == myCaseHead && (myCaseHead == null || thenCaseHead != myCaseHead.Conditional.Follow))
                        {
                            node.UnstructType = UnstructuredType.JumpIntoCase;
                            node.Conditional = new IfThen(null);
                        }
                    }
                }

                // for 2 way conditional headers that don't have a follow (i.e. are the source of a back
                // edge) and haven't been structured as latching nodes, set their follow to be the
                // non-back edge child.
                if (node.Conditional != null && node.Conditional.Follow == null &&
                     node.UnstructType == UnstructuredType.Structured && !(node.Conditional is Case))
                {
                    Debug.Assert(node.HasABackEdge());

                    if (node.HasBackEdgeTo(node.Then))
                    {
                        node.Conditional = new IfThen(node.Else);
                    }
                    else
                    {
                        node.Conditional = new IfElse(node.Then);
                    }
                }
            }
        }

        private bool IsTwoWayBranchWithFollow(StructureNode curNode)
        {
            return curNode.Conditional != null && curNode.Conditional.Follow != null && !(curNode.Conditional is Case);
        }

        private bool IsJumpIntoCaseBody(StructureNode curNode)
        {
            return curNode.UnstructType == UnstructuredType.Structured &&
                                     (curNode.CaseHead != curNode.Then.CaseHead ||
                                      curNode.CaseHead != curNode.Else.CaseHead);
        }

        private void SetUnstructuredLoopTransfer(StructureNode curNode, StructureNode loopNode)
        {
            if (curNode.Then == loopNode || curNode.Then.IsAncestorOf(loopNode))
            {
                curNode.UnstructType = UnstructuredType.JumpInOutLoop;
                curNode.Conditional = new IfElse(null);
            }
            else if (curNode.Else == loopNode || curNode.Else.IsAncestorOf(loopNode))
            {
                curNode.UnstructType = UnstructuredType.JumpInOutLoop;
                curNode.Conditional = new IfThen(null);
            }
        }
    }
}
