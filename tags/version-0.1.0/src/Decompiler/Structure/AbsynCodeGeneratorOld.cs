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
using Decompiler.Core.Lib;
using Decompiler.Core.Code;
using Decompiler.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    /// <summary>
    /// Generates abstract syntax from intermediate level instructions.
    /// </summary>
    [Obsolete]
    public class AbsynCodeGenerator
    {
        public void GenerateHighLevelCode(ProcedureStructure curProc, List<AbsynStatement> stms)
        {
            List<StructureNode> followSet = new List<StructureNode>();
            List<StructureNode> gotoSet = new List<StructureNode>();

            WriteCode(curProc.EntryNode, null, followSet, gotoSet, new AbsynStatementEmitter(stms));
        }

        public void WriteCode(StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, AbsynStatementEmitter emitter)
        {
            // If this is the follow for the most nested enclosing conditional, then
            // don't generate anything. Otherwise if it is in the follow set
            // generate a goto to the follow
            StructureNode closestFollowNode = followSet.Count == 0
                ? null
                : followSet[followSet.Count - 1];

            if (gotoSet.Contains(node) &&
                !node.IsLatchNode() &&
                ((latch != null && node == latch.Loop.Follow) || !AllParentsGenerated(node)))
            {
                EmitGotoAndLabel(node, node, emitter);
                return;
            }
            else if (followSet.Contains(node))
            {
                if (node != closestFollowNode)
                {
                    EmitGotoAndLabel(node, node, emitter);
                }
                return;
            }

            // Has this node already been generated?
            if (node.traversed == travType.DFS_CODEGEN)
            {
                // this should only occur for a loop over a single block
                Debug.Assert(node.Loop != null && node.Loop is PostTestedLoop && node.Loop.Latch == node);
                return;
            }
            node.traversed = travType.DFS_CODEGEN;


            if (!AllParentsGenerated(node))
            {
                emitter.EmitLabel(node);
            }

            switch (0)// node.GetStructType())
            {
            case 0: // structType.Loop:
            case 1: // structType.LoopCond:

                // add the follow of the loop (if it exists) to the follow set
                if (node.Loop.Follow != null)
                    followSet.Add(node.Loop.Follow);

                if (node.Loop is PreTestedLoop)
                {
                    Debug.Assert(node.Loop.Latch.OutEdges.Count == 1);

                    // write the loop header except the predicate.
                    WriteBlockExcludingPredicate(node, emitter);

                    // write the code for the body of the loop
                    StructureNode loopBody = (node.Else == node.Loop.Follow)
                        ? node.Then
                        : node.Else;
                    List<AbsynStatement> body = new List<AbsynStatement>();
                    AbsynStatementEmitter emitBody = new AbsynStatementEmitter(body);

                    WriteCode(loopBody, node.Loop.Latch, followSet, gotoSet, emitBody);

                    // if code has not been generated for the latch node, generate it now
                    if (node.Loop.Latch.traversed != travType.DFS_CODEGEN)
                    {
                        node.Loop.Latch.traversed = travType.DFS_CODEGEN;
                        WriteBlockExcludingPredicate(node.Loop.Latch, emitBody);
                    }

                    // rewrite the loop header(excluding the predicate) inside the loop
                    // after making sure another label won't be generated.
                    node.ForceLabel = false;
                    WriteBlockExcludingPredicate(node, emitBody);

                    emitter.EmitWhile(node, ((Branch) node.Instructions.Last.Instruction).Condition, body);
                }
                else
                {
                    List<AbsynStatement> body = new List<AbsynStatement>();
                    AbsynStatementEmitter emitBody = new AbsynStatementEmitter(body);

                    // if this is also a conditional header, then generate code for the
                    // conditional. Otherwise generate code for the loop body.
                    if (node.Loop !=null && node.Conditional != null)
                    {
                        // set the necessary flags so that WriteCode can successfully be called
                        // again on this node
                        node.traversed = travType.UNTRAVERSED;

                        WriteCode(node, node.Loop.Latch, followSet, gotoSet, emitBody);
                    }
                    else
                    {
                        WriteBlockExcludingPredicate(node, emitBody);
                        WriteCode(node.OutEdges[0], node.Loop.Latch, followSet, gotoSet, emitBody);
                    }

                    if (node.Loop is PostTestedLoop)
                    {
                        // if code has not been generated for the latch node, generate it now
                        if (node.Loop.Latch.traversed != travType.DFS_CODEGEN)
                        {
                            node.Loop.Latch.traversed = travType.DFS_CODEGEN;
                            WriteBlockExcludingPredicate(node.Loop.Latch, emitBody);
                        }
                        Expression expr = ((Branch) node.Loop.Latch.Instructions.Last.Instruction).Condition;
                        emitter.EmitDoWhile(body, expr);
                    }
                    else
                    {
                        Debug.Assert(node.Loop is EndLessLoop);

                        // if code has not been generated for the latch node, generate it now
                        if (node.Loop.Latch.traversed != travType.DFS_CODEGEN)
                        {
                            node.Loop.Latch.traversed = travType.DFS_CODEGEN;
                            WriteBlockExcludingPredicate(node.Loop.Latch, emitter);
                        }
                        emitter.EmitForever(node, body);
                    }
                }

                // write the code for the follow of the loop (if it exists)
                if (node.Loop.Follow != null)
                {
                    // remove the follow from the follow set
                    followSet.RemoveAt(followSet.Count - 1);

                    if (node.Loop.Follow.traversed != travType.DFS_CODEGEN)
                        WriteCode(node.Loop.Follow, latch, followSet, gotoSet, emitter);
                    else
                        EmitGotoAndLabel(node, node.Loop.Follow, emitter);
                }
                break;

            case 3: // structType.Cond:

                // reset this back to LoopCond if it was originally of this type
                if (node.Loop.Latch != null)
                    ;

                // for 2 way conditional headers that are effectively jumps into or out of a
                // loop or case body, we will need a new follow node
                StructureNode tmpCondFollow = null;

                // keep track of how many nodes were added to the goto set so that the correct number
                // are removed
                int gotoTotal = 0;

                // add the follow to the follow set if this is a case header
                if (node.Conditional is Case)
                {
                    followSet.Add(node.Conditional.Follow);
                }
                else if (node.Conditional.Follow != null)
                {
                    // For a structured two conditional header, its follow is added to the follow set
                    StructureNode myLoopHead = (node.Loop != null ? node : node.Loop.Header);

                    if (node.UnstructType == UnstructuredType.Structured)
                    {
                        followSet.Add(node.Conditional.Follow);
                    }
                    else
                    {
                        // Otherwise, for a jump into/outof a loop body, the follow is added to the goto set.
                        // The temporary follow is set for any unstructured conditional header
                        // branch that is within the same loop and case.
                        if (node.UnstructType == UnstructuredType.JumpInOutLoop)
                        {
                            // define the loop header to be compared against
                            myLoopHead = (node.Loop != null  ? node : node.Loop.Header);

                            gotoSet.Add(node.Conditional.Follow);
                            gotoTotal++;

                            // also add the current latch node, and the loop header of the follow if they exist
                            if (node.Loop.Latch != null)
                            {
                                gotoSet.Add(node.Loop.Latch);
                                gotoTotal++;
                            }
                            if (node.Conditional.Follow.Loop.Header != null && node.Conditional.Follow.Loop.Header != myLoopHead)
                            {
                                gotoSet.Add(node.Conditional.Follow.Loop.Header);
                                gotoTotal++;
                            }
                        }

                        if (node.Conditional is IfThen)
                            tmpCondFollow = node.Else;
                        else
                            tmpCondFollow = node.Then;

                        // for a jump into a case, the temp follow is added to the follow set
                        if (node.UnstructType == UnstructuredType.JumpIntoCase)
                            followSet.Add(tmpCondFollow);
                    }
                }

                WriteBlockExcludingPredicate(node, emitter);

                Conditional cond = node.Conditional;
                cond.GenerateCode(node, latch, followSet, gotoSet, this, emitter);

                // do all the follow stuff if this conditional had one
                if (node.Conditional.Follow != null)
                {
                    // remove the original follow from the follow set if it was added by this header
                    if (node.UnstructType == UnstructuredType.Structured || node.UnstructType == UnstructuredType.JumpIntoCase)
                    {
                        Debug.Assert(gotoTotal == 0);
                        followSet.RemoveAt(followSet.Count - 1);
                    }
                    // else remove all the nodes added to the goto set
                    else
                    {
                        for (int i = 0; i < gotoTotal; i++)
                            gotoSet.RemoveAt(gotoSet.Count - 1);
                    }

                    // do the code generation (or goto emitting) for the new conditional follow if it exists
                    // otherwise do it for the original follow
                    if (tmpCondFollow == null)
                        tmpCondFollow = node.Conditional.Follow;
                    if (node.Loop.Header == tmpCondFollow)
                        break;
                    if (tmpCondFollow.traversed == travType.DFS_CODEGEN)
                        EmitGotoAndLabel(node, tmpCondFollow, emitter);
                    else
                        WriteCode(tmpCondFollow, latch, followSet, gotoSet, emitter);
                }
                break;

            case 2: //structType.Seq:
                WriteBlockExcludingPredicate(node, emitter);
                if (node.BlockType == bbType.ret)
                {
                    emitter.EmitReturn(((ReturnInstruction)node.Instructions.Last.Instruction).Expression); //$REVIEW: Awkward.
                    return;
                }

                if (node.Loop.Header!= null && node == node.Loop.Latch)
                    break;

                // generate code for its successor if it hasn't already been visited and is in the same loop/case
                // and is not the latch for the current most enclosing loop. 
                // The only exception for generating it when it is not in the same loop
                // is when this when it is only reached from this node
                StructureNode child = node.OutEdges[0];
                if (ShouldNotGenerateCodeForSuccessor(node, followSet, child))

                    EmitGotoAndLabel(node, child, emitter);
                else
                    WriteCode(child, latch, followSet, gotoSet, emitter);

                break;
            }
        }



        public bool ShouldNotGenerateCodeForSuccessor(StructureNode node, List<StructureNode> followSet, StructureNode nextNode)
        {
            if (nextNode.traversed == travType.DFS_CODEGEN && (followSet.Count == 0 || followSet[followSet.Count-1] != nextNode))
                return true;
            if ((nextNode.Loop != node.Loop) &&
                             (!AllParentsGenerated(nextNode) || followSet.Contains(nextNode)))
                return true;
            if (node.Loop.Latch != null && node.Loop.Latch.Loop.Follow == nextNode)
                return true;
            if (node.CaseHead != nextNode.CaseHead && (node.CaseHead == null || nextNode != node.CaseHead.Conditional.Follow))
                return true;
            return false;
        }

        void WriteBlockExcludingPredicate(StructureNode node, AbsynStatementEmitter emitter)
        {
            if (node.ForceLabel)
                emitter.EmitLabel(node);

            for (int i = 0; i < node.Instructions.Count; i++)
            {
                Statement stm = node.Instructions[i];
                if (stm.Instruction.IsControlFlow)
                    break;
                emitter.EmitStatement(stm);
            }
        }


        // Return true if every parent of this node has had its code generated
        private bool AllParentsGenerated(StructureNode node)
        {
            foreach (StructureNode pred in node.InEdges)
                if (!pred.HasBackEdgeTo(node) && pred.traversed != travType.DFS_CODEGEN)
                    return false;
            return true;
        }

        // Emit a goto statement to the given destination as well as making sure that
        // this destination gives itself a label
        // Emits a goto statement (at the correct indentation level) with the destination label for dest.
        // Also places the label just before the destination code if it isn't already there.
        // If the goto is to the return block, emit a 'return' instead.
        // Also, 'continue' and 'break' statements are used instead if possible
        public void EmitGotoAndLabel(StructureNode node, StructureNode dest, AbsynStatementEmitter emitter)
        {
            // is this a goto to the ret block?
            if (dest.BlockType == bbType.ret)
            {
                emitter.EmitReturn(null);
            }
            else
            {
                if (node.Loop != null && (node.Loop.Header == dest || node.Loop.Follow == dest))
                {
                    if (node.Loop.Header == dest)
                        emitter.EmitContinue();
                    else
                        emitter.EmitBreak();
                }
                else
                {
                    emitter.EmitGoto(dest);

                    // don't emit the label if it already has been emitted or the code 
                    // for the destination has not yet been generated
                    if (!dest.ForceLabel && dest.traversed == travType.DFS_CODEGEN)
                        ;
                    dest.ForceLabel = true;
                }
            }
        }
    }
}

