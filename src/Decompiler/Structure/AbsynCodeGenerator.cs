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
    /// Generates abstract syntax from intermediate level instructions
    /// </summary>
    public class AbsynCodeGenerator
    {
        public void GenerateHighLevelCode(ProcedureStructure curProc, List<AbsynStatement> stms)
        {
            List<StructureNode> followSet = new List<StructureNode>();
            List<StructureNode> gotoSet = new List<StructureNode>();

            WriteCode(curProc.EntryNode, 1, null, followSet, gotoSet, new AbsynStatementEmitter(stms));
        }


        public void WriteCode(StructureNode node, int indLevel, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, AbsynStatementEmitter emitter)
        {
            // If this is the follow for the most nested enclosing conditional, then
            // don't generate anything. Otherwise if it is in the follow set
            // generate a goto to the follow
            StructureNode closestFollowNode = followSet.Count == 0
                ? null
                : followSet[followSet.Count - 1];

            if (gotoSet.Contains(node) &&
                !node.IsLatchNode() &&
                ((latch != null && node == latch.LoopHead.LoopFollow) || !AllParentsGenerated(node)))
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
                Debug.Assert(node.GetStructType() == structType.Loop && node.GetLoopType() == loopType.PostTested && node.LatchNode == node);
                return;
            }
            else
                node.traversed = travType.DFS_CODEGEN;

            // if this is a latchNode and the current indentation level is
            // the same as the first node in the loop, then this write out its body and return
            // otherwise generate a goto
            if (latch != null && node.IsLatchNode())
            {
                if (true)
                {
                    WriteBB(node, emitter);
                    return;
                }
                else
                {
                    // unset its traversed flag
                    node.traversed = travType.UNTRAVERSED;

                    EmitGotoAndLabel(node, node, emitter);
                    return;
                }
            }

            if (!AllParentsGenerated(node))
            {
                emitter.EmitLabel(node);
            }

            switch (node.GetStructType())
            {
            case structType.Loop:
            case structType.LoopCond:

                // add the follow of the loop (if it exists) to the follow set
                if (node.LoopFollow != null)
                    followSet.Add(node.LoopFollow);

                if (node.GetLoopType() == loopType.PreTested)
                {
                    Debug.Assert(node.LatchNode.OutEdges.Count == 1);

                    // write the loop header except the predicate.
                    WriteBB(node, emitter);

                    // write the code for the body of the loop
                    StructureNode loopBody = (node.Else == node.LoopFollow)
                        ? node.Then
                        : node.Else;
                    List<AbsynStatement> body = new List<AbsynStatement>();
                    AbsynStatementEmitter emitBody = new AbsynStatementEmitter(body);

                    WriteCode(loopBody, indLevel + 1, node.LatchNode, followSet, gotoSet, emitBody);

                    // if code has not been generated for the latch node, generate it now
                    if (node.LatchNode.traversed != travType.DFS_CODEGEN)
                    {
                        node.LatchNode.traversed = travType.DFS_CODEGEN;
                        WriteBB(node.LatchNode, emitBody);
                    }

                    // rewrite the loop header(excluding the predicate) inside the loop
                    // after making sure another label won't be generated.
                    node.hllLabel = false;
                    WriteBB(node, emitBody);

                    emitter.EmitWhile(node, ((Branch) node.Instructions.Last.Instruction).Condition, body);
                }
                else
                {
                    List<AbsynStatement> body = new List<AbsynStatement>();
                    AbsynStatementEmitter emitBody = new AbsynStatementEmitter(body);

                    // if this is also a conditional header, then generate code for the
                    // conditional. Otherwise generate code for the loop body.
                    if (node.GetStructType() == structType.LoopCond)
                    {
                        // set the necessary flags so that WriteCode can successfully be called
                        // again on this node
                        node.SetStructType(structType.Cond);
                        node.traversed = travType.UNTRAVERSED;

                        WriteCode(node, indLevel + 1, node.LatchNode, followSet, gotoSet, emitBody);
                    }
                    else
                    {
                        WriteBB(node, emitBody);
                        WriteCode(node.OutEdges[0], indLevel + 1, node.LatchNode, followSet, gotoSet, emitBody);
                    }

                    if (node.GetLoopType() == loopType.PostTested)
                    {
                        // if code has not been generated for the latch node, generate it now
                        if (node.LatchNode.traversed != travType.DFS_CODEGEN)
                        {
                            node.LatchNode.traversed = travType.DFS_CODEGEN;
                            WriteBB(node.LatchNode, emitBody);
                        }

                        // write the repeat loop predicate
                        Expression expr = ((Branch) node.Instructions.Last.Instruction).Condition;
                        emitter.EmitDoWhile(node, body, expr);
                    }
                    else
                    {
                        Debug.Assert(node.GetLoopType() == loopType.Endless);

                        // if code has not been generated for the latch node, generate it now
                        if (node.LatchNode.traversed != travType.DFS_CODEGEN)
                        {
                            node.LatchNode.traversed = travType.DFS_CODEGEN;
                            WriteBB(node.LatchNode, emitter);
                        }

                        emitter.EmitForever(node, body);
                    }
                }

                // write the code for the follow of the loop (if it exists)
                if (node.LoopFollow != null)
                {
                    // remove the follow from the follow set
                    followSet.RemoveAt(followSet.Count - 1);

                    if (node.LoopFollow.traversed != travType.DFS_CODEGEN)
                        WriteCode(node.LoopFollow, indLevel, latch, followSet, gotoSet, emitter);
                    else
                        EmitGotoAndLabel(node, node.LoopFollow, emitter);
                }
                break;

            case structType.Cond:

                // reset this back to LoopCond if it was originally of this type
                if (node.LatchNode != null)
                    node.SetStructType(structType.LoopCond);

                // for 2 way conditional headers that are effectively jumps into or out of a
                // loop or case body, we will need a new follow node
                StructureNode tmpCondFollow;
                tmpCondFollow = null;

                // keep track of how many nodes were added to the goto set so that the correct number
                // are removed
                int gotoTotal;
                gotoTotal = 0;

                // add the follow to the follow set if this is a case header
                if (node.CondType == condType.Case)
                {
                    followSet.Add(node.CondFollow);
                }
                else if (node.CondType != condType.Case && node.CondFollow != null)
                {
                    // For a structured two conditional header, its follow is added to the follow set
                    StructureNode myLoopHead = (node.GetStructType() == structType.LoopCond ? node : node.LoopHead);

                    if (node.UnstructType == UnstructuredType.Structured)
                        followSet.Add(node.CondFollow);

                    // Otherwise, for a jump into/outof a loop body, the follow is added to the goto set.
                    // The temporary follow is set for any unstructured conditional header
                    // branch that is within the same loop and case.
                    else
                    {
                        if (node.UnstructType == UnstructuredType.JumpInOutLoop)
                        {
                            // define the loop header to be compared against
                            myLoopHead = (node.GetStructType() == structType.LoopCond ? node : node.LoopHead);

                            gotoSet.Add(node.CondFollow);
                            gotoTotal++;

                            // also add the current latch node, and the loop header of the follow if they exist
                            if (node.LatchNode != null)
                            {
                                //gotoSet.Add(static_cast<CFGNode>(latch));
                                gotoSet.Add(node.LatchNode);
                                gotoTotal++;
                            }
                            if (node.CondFollow.LoopHead != null && node.CondFollow.LoopHead != myLoopHead)
                            {
                                gotoSet.Add(node.CondFollow.LoopHead);
                                gotoTotal++;
                            }
                        }

                        if (node.CondType == condType.IfThen)
                            tmpCondFollow = node.Else;
                        else
                            tmpCondFollow = node.Then;

                        // for a jump into a case, the temp follow is added to the follow set
                        if (node.UnstructType == UnstructuredType.JumpIntoCase)
                            followSet.Add(tmpCondFollow);
                    }
                }

                // write the body of the block (excluding the predicate)
                WriteBB(node, emitter);

                AbsynIf ifStm = null;
                AbsynSwitch switchStm = null;
                // write the conditional header
                if (node.CondType == condType.Case)
                {
                    Expression exp = ((SwitchInstruction) node.Instructions.Last.Instruction).Expression;
                    switchStm = emitter.EmitSwitch(node, exp);
                }
                else
                {
                    Expression exp = ((Branch) node.Instructions.Last.Instruction).Condition;
                    ifStm = emitter.EmitIfCondition(exp, node);
                }

                // write code for the body of the conditional
                if (node.CondType != condType.Case)
                {
                    StructureNode succ = (node.CondType == condType.IfElse ? node.Else : node.Then);
                    AbsynStatementEmitter emitThen = new AbsynStatementEmitter(ifStm.Then);
                    // emit a goto statement if the first clause has already been generated or it
                    // is the follow of this node's enclosing loop
                    if (succ.traversed == travType.DFS_CODEGEN || (node.LoopHead != null && succ == node.LoopHead.LoopFollow))
                        EmitGotoAndLabel(node, succ, emitThen);
                    else
                        WriteCode(succ, indLevel + 1, latch, followSet, gotoSet, emitThen);

                    // generate the else clause if necessary
                    if (node.CondType == condType.IfThenElse)
                    {
                        succ = node.Else;

                        AbsynStatementEmitter emitElse = new AbsynStatementEmitter(ifStm.Else);

                        // emit a goto statement if the second clause has already been generated
                        if (succ.traversed == travType.DFS_CODEGEN)
                            EmitGotoAndLabel(node, succ, emitElse);
                        else
                            WriteCode(succ, indLevel + 1, latch, followSet, gotoSet, emitElse);
                    }
                }
                else		// case header
                {
                    AbsynStatementEmitter emitSwitchBranches = new AbsynStatementEmitter(switchStm.Statements);
                    // generate code for each out branch
                    for (int i = 0; i < node.OutEdges.Count; i++)
                    {
                        emitSwitchBranches.EmitCaseLabel(node, i);

                        // generate code for the current outedge
                        StructureNode succ = node.OutEdges[i];
                        //				Debug.Assert(succ.node.CaseHead == this || succ == condFollow || HasBackEdgeTo(succ));
                        if (succ.traversed == travType.DFS_CODEGEN)
                            EmitGotoAndLabel(node, succ, emitSwitchBranches);
                        else
                        {
                            WriteCode(succ, indLevel + 1, latch, followSet, gotoSet, emitSwitchBranches);
                            emitSwitchBranches.EmitBreak(node);
                        }
                    }
                }

                // do all the follow stuff if this conditional had one
                if (node.CondFollow != null)
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
                        tmpCondFollow = node.CondFollow;

                    if (tmpCondFollow.traversed == travType.DFS_CODEGEN)
                        EmitGotoAndLabel(node, tmpCondFollow, emitter);
                    else
                        WriteCode(tmpCondFollow, indLevel, latch, followSet, gotoSet, emitter);
                }

                break;

            case structType.Seq:
                WriteBB(node, emitter);
                if (node.BlockType == bbType.ret)
                {
                    emitter.EmitReturn(((ReturnInstruction)node.Instructions.Last.Instruction).Expression); //$REVIEW: Awkward.
                    return;
                }

                // generate code for its successor if it hasn't already been visited and is in the same loop/case
                // and is not the latch for the current most enclosing loop. 
                // The only exception for generating it when it is not in the same loop
                // is when this when it is only reached from this node
                StructureNode child = node.OutEdges[0];
                if (ShouldNotGenerateCodeForSuccessor(node, followSet, child))

                    EmitGotoAndLabel(node, child, emitter);
                else
                    WriteCode(child, indLevel, latch, followSet, gotoSet, emitter);

                break;
            }
        }

        public bool ShouldNotGenerateCodeForSuccessor(StructureNode node, List<StructureNode> followSet, StructureNode child)
        {
            if (child.traversed == travType.DFS_CODEGEN)
                return true;
            if ((child.LoopHead != node.LoopHead) &&
                             (!AllParentsGenerated(child) || followSet.Contains(child)))
                return true;
            if (node.LatchNode != null && node.LatchNode.LoopHead.LoopFollow == child)
                return true;
            if (node.CaseHead != child.CaseHead && (node.CaseHead == null || child != node.CaseHead.CondFollow))
                return true;
            return false;
        }

        void WriteBB(StructureNode node, AbsynStatementEmitter emitter)
        {
            if (node.hllLabel)
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
                if (node.LoopHead != null && (node.LoopHead == dest || node.LoopHead.LoopFollow == dest))
                {
                    if (node.LoopHead == dest)
                        emitter.EmitContinue(node);
                    else
                        emitter.EmitBreak(node);
                }
                else
                {
                    emitter.EmitGoto(dest);

                    // don't emit the label if it already has been emitted or the code 
                    // for the destination has not yet been generated
                    if (!dest.hllLabel && dest.traversed == travType.DFS_CODEGEN)
                        dest.labelStr = string.Format("L{0}:{1}", dest.Order, Environment.NewLine);

                    dest.hllLabel = true;
                }
            }
        }
    }
}

