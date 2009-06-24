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
    public class AbsynCodeGeneratorState
    {
        private StructureNode latch;
        private List<StructureNode> followSet;
        private List<StructureNode> gotoSet;
        private List<AbsynStatement> stms;
        private HashSet<StructureNode> needsLabel;

        public AbsynCodeGeneratorState(
            StructureNode latch, 
            List<StructureNode> followSet, 
            List<StructureNode> gotoSet, 
            List<AbsynStatement> stms)
        {
            this.latch = latch;
            this.followSet = followSet;
            this.gotoSet = gotoSet;
            this.stms = stms;

            this.needsLabel = new HashSet<StructureNode>();

        }

        public StructureNode Latch
        {
            get { return latch; }
        }



        public List<StructureNode> FollowSet
        {
            get { return followSet; }
        }
        public List<StructureNode> GotoSet
        {
            get { return gotoSet; }
        }

        public List<AbsynStatement> Stms
        {
            get { return stms; }
        }

        /// <summary>
        /// Converts an intermediate code instruction into an AbsynStatement. Instructions that involve control flow,
        /// that is branches and indirect jumps, return null. Callers must check for a null return that indicates 
        /// such a flow control instruction.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        public AbsynStatement ConvertInstruction(Instruction instr)
        {
            //$REVIEW: this needs to live in a separate class and use the visitor pattern.
            Assignment ass = instr as Assignment;
            if (ass != null)
                return new AbsynAssignment(ass.Dst, ass.Src);
            Store store = instr as Store;
            if (store != null)
                return new AbsynAssignment(store.Dst, store.Src);
            ReturnInstruction ret = instr as ReturnInstruction;
            if (ret != null)
                return new AbsynReturn(ret.Expression);
            SideEffect se = instr as SideEffect;
            if (se != null)
                return new AbsynSideEffect(se.Expression);
            if (instr is Branch)
                return null;
            if (instr is SwitchInstruction)
                return null;
            throw new NotImplementedException(instr.GetType().ToString());

        }

        public void EmitGotoAndLabel(StructureNode node, StructureNode dest)
        {
            // is this a goto to the ret block?
            if (dest.BlockType == bbType.ret)
            {
                stms.Add(new AbsynReturn(null));
            }
            else
            {
                AbsynStatement gotoStmt;
                if (node.LoopHead == dest)
                {
                    gotoStmt = new AbsynContinue();
                }
                else if (node.LoopHead != null && node.LoopHead.LoopFollow == dest)
                {
                    gotoStmt = new AbsynBreak();
                }
                else
                {
                    gotoStmt = new AbsynGoto(dest.EntryBlock.Name);
                    needsLabel.Add(dest);
                }
                stms.Add(gotoStmt);
            }
        }


        public StructureNode EnclosingFollowNode
        {
            get { 
                return followSet.Count != 0
                    ? followSet[followSet.Count - 1]                 
                    : null;
            }
        }

        public AbsynStatement GenerateBlockCode(StructureNode node)
        {
            AbsynStatement last = null;
            if (needsLabel.Contains(node))
            {
                stms.Add(new AbsynLabel(node.EntryBlock.Name));
            }
            foreach (Statement stm in node.EntryBlock.Statements)
            {
                last = ConvertInstruction(stm.Instruction);
                if (last == null)
                    break;
                stms.Add(last);
            }
            return last;
        }

        public Instruction LastInstruction(StructureNode node)
        {
            if (node.EntryBlock.Statements.Count == 0)
                return null;
            else
                return node.EntryBlock.Statements.Last.Instruction;
        }


        public void RequireLabel(StructureNode node, bool require)
        {
            if (!require)
                needsLabel.Remove(node);
            else
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Generates abstract syntax from intermediate level instructions
    /// </summary>
    public class AbsynCodeGenerator
    {
        private HashSet<StructureNode> visited;

        public AbsynCodeGenerator() 
        { 
            this.visited = new HashSet<StructureNode>();
        }

        public List<AbsynStatement> GenerateCode(StructureNode node)
        {
            List<AbsynStatement> stms = new List<AbsynStatement>();
            GenerateCode(node, new AbsynCodeGeneratorState(null, new List<StructureNode>(), new List<StructureNode>(), stms));
            return stms;
        }

        public void GenerateCode(StructureNode node, AbsynCodeGeneratorState state)
        {
            // If this is the follow for the most nested enclosing conditional, then
            // don't generate anything. Otherwise if it is in the follow set
            // generate a goto to the follow
            StructureNode enclFollow = state.EnclosingFollowNode; 

            if (state.GotoSet.Contains(node) && 
                !node.IsLatchNode && 
                ((state.Latch != null && node == state.Latch.LoopHead.LoopFollow) || !AllParentsGenerated(node)))
            {
                EmitGotoAndLabel(state, node, node);
                return;
            }
            else if (state.FollowSet.Contains(node))
            {
                if (node != enclFollow)
                {
                    EmitGotoAndLabel(state, node, node);
                }
                return;
            }

            if (visited.Contains(node))
            {
                // this should only occur for a loop over a single block
                Debug.Assert(node.IsStructType(StructuredGraph.Loop) && node.LoopType == loopType.PostTested && node.LatchNode == node);
                return;
            }

            visited.Add(node);

            // if this is a latchNode and the current indentation level is
            // the same as the first node in the loop, then this write out its body and return
            // otherwise generate a goto
            if (node.IsLatchNode)
            {
                if (true) // indLevel == latch.LoopHead.indentLevel + (latch.LoopHead.LoopType == loopType.PreTested ? 1 : 0))
                {
                    state.GenerateBlockCode(node);
                    return;
                }
                else
                {
                    throw new NotImplementedException("Haven't tested this.");
                    // unset its traversed flag
                    visited.Remove(node);
                    EmitGotoAndLabel(state, node, node);
                    return;
                }
            }
            node.GetStructType().GenerateCode(this, node, state);
        }

        public void GenerateSequentialCode(StructureNode node, AbsynCodeGeneratorState state)
        {
            state.GenerateBlockCode(node);
            if (state.LastInstruction(node) is ReturnInstruction)
                return;
            
            StructureNode child = node.Succ[0];
            if (SequentialBlockNeedsGoto(node, state.FollowSet, child))
            {
                EmitGotoAndLabel(state, node, child);
            }
            else
            {
                GenerateCode(child, state);
            }
        }

        // generate code for its successor if it hasn't already been visited and is in the same loop/case
        // and is not the latch for the current most enclosing loop. 
        // The only exception for generating it when it is not in the same loop
        // is when this when it is only reached from this node
        private bool SequentialBlockNeedsGoto(StructureNode node, List<StructureNode> followSet, StructureNode child)
        {
            if (visited.Contains(child))
                return true;
            if ((child.LoopHead != node.LoopHead) && (!AllParentsGenerated(child) || followSet.Contains(child)))
                return true;
            if (node.LatchNode != null && node.LatchNode.LoopHead.LoopFollow == child)
                return true;
            if (!(node.CaseHead == child.CaseHead || (node.CaseHead != null && child == node.CaseHead.CondFollow)))
                return true;
            return false;
        }

        public void GenerateCondCode(StructureNode node, AbsynCodeGeneratorState state)
        {
            // reset this back to LoopCond if it was originally of this type
            if (node.LatchNode != null)
                node.SetStructType(StructuredGraph.LoopCond);

            // for 2 way conditional headers that are effectively jumps into or out of a
            // loop or case body, we will need a new follow node
            StructureNode tmpCondFollow = null;

            // keep track of how many nodes were added to the goto set so that the correct number
            // are removed
            int gotoTotal = 0;

            if (node.CondType == condType.Case)
            {
                state.FollowSet.Add(node.CondFollow);
            }
            else if (node.CondFollow != null)
            {
                // For a structured two conditional header, its follow is added to the follow set
                StructureNode myLoopHead = (node.IsStructType(StructuredGraph.LoopCond) ? node : node.LoopHead);

                if (node.UnstructType == unstructType.Structured)
                {
                    state.FollowSet.Add(node.CondFollow);

                }
                else
                {
                    // Otherwise, for a jump into/outof a loop body, the follow is added to the goto set.
                    // The temporary follow is set for any unstructured conditional header
                    // branch that is within the same loop and case.
                    if (node.UnstructType == unstructType.JumpInOutLoop)
                    {
                        // define the loop header to be compared against
                        myLoopHead = (node.IsStructType(StructuredGraph.LoopCond) ? node : node.LoopHead);

                        state.GotoSet.Add(node.CondFollow);
                        gotoTotal++;

                        // also add the current latch node, and the loop header of the follow if they exist
                        if (node.LatchNode != null)
                        {
                            //gotoSet.Add(static_cast<StructureNode>(latch));
                            state.GotoSet.Add(node.LatchNode);
                            gotoTotal++;
                        }
                        if (node.CondFollow.LoopHead != null && node.CondFollow.LoopHead != myLoopHead)
                        {
                            state.GotoSet.Add(node.CondFollow.LoopHead);
                            gotoTotal++;
                        }
                    }

                    if (node.CondType == condType.IfThen)
                        tmpCondFollow = node.Else;
                    else
                        tmpCondFollow = node.Then;

                    // for a jump into a case, the temp follow is added to the follow set
                    if (node.UnstructType == unstructType.JumpIntoCase)
                        state.FollowSet.Add(tmpCondFollow);
                }
            }

            // write the body of the block (excluding the predicate)
            state.GenerateBlockCode(node);
            Instruction last = state.LastInstruction(node);
            SwitchInstruction sw = last as SwitchInstruction;
            if (sw != null)
            {
                AbsynSwitch s = new AbsynSwitch(sw.Expression);
                state.Stms.Add(s);
                GenerateCaseCode(node, new AbsynCodeGeneratorState(state.Latch, state.FollowSet, state.GotoSet, s.Statements));
            }
            else
            {
                Branch b = (Branch) last;
                AbsynIf i = new AbsynIf();
                i.Condition = b.Condition;
                state.Stms.Add(i);
                GenerateIfCode(node, state.Latch, state.FollowSet, state.GotoSet, i);
            }


            if (node.CondFollow != null)
            {
                // remove the original follow from the follow set if it was added by this header
                if (node.UnstructType == unstructType.Structured || node.UnstructType == unstructType.JumpIntoCase)
                {
                    Debug.Assert(gotoTotal == 0);
                    state.FollowSet.RemoveAt(state.FollowSet.Count - 1);
                }

                // else remove all the nodes added to the goto set
                else
                    for (int i = 0; i < gotoTotal; i++)
                        state.GotoSet.RemoveAt(state.GotoSet.Count - 1);

                // do the code generation (or goto emitting) for the new conditional follow if it exists
                // otherwise do it for the original follow
                if (tmpCondFollow == null)
                    tmpCondFollow = node.CondFollow;

                if (visited.Contains(tmpCondFollow))
                    EmitGotoAndLabel(state, node, tmpCondFollow);
                else
                    GenerateCode(tmpCondFollow, state);
            }

        }

        public void GenerateCaseCode(StructureNode node, AbsynCodeGeneratorState state)
        {
            int i = 0;
            foreach (StructureNode succ in node.Succ)
            {
                state.Stms.Add(new AbsynCase(i++));           //$REFACTOR: make this a member of state.
                if (visited.Contains(succ))
                    EmitGotoAndLabel(state, node, succ);     //$REFACTOR: make this a member of state
                else
                {
                    GenerateCode(succ, state);
                    state.Stms.Add(new AbsynBreak());       //$REFACTOR: mmake this a method of state.
                }
            }
        }

        private void GenerateIfCode(StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, AbsynIf absynIf)
        {
            StructureNode succ = (node.CondType == condType.IfElse ? node.Else : node.Then);

            // emit a goto statement if the first clause has already been generated or it
            // is the follow of this node's enclosing loop
            AbsynCodeGeneratorState thenState = new AbsynCodeGeneratorState(latch, followSet, gotoSet, absynIf.Then);
            if (visited.Contains(succ) || (node.LoopHead != null && succ == node.LoopHead.LoopFollow))
                EmitGotoAndLabel(thenState, node, succ);
            else
                GenerateCode(succ, thenState);

            if (node.CondType == condType.IfThenElse)
            {
                AbsynCodeGeneratorState elseState = new AbsynCodeGeneratorState(latch, followSet, gotoSet, absynIf.Else);
                succ = node.Else;
                if (visited.Contains(succ))
                    EmitGotoAndLabel(elseState, node, succ);
                else
                    GenerateCode(succ, elseState);
            }
        }

        public void GenerateLoopCode(StructureNode node, AbsynCodeGeneratorState state)
        {
            // add the follow of the loop (if it exists) to the follow set
            if (node.LoopFollow != null)            //$TODO make this a method of state. PushFollownode.
                state.FollowSet.Add(node.LoopFollow);

            if (node.LoopType == loopType.PreTested)
            {
                Debug.Assert(node.LatchNode.Succ.Count == 1);

                state.GenerateBlockCode(node);

                Branch branch = (Branch) node.LastInstruction;
                List<AbsynStatement> whileBody = new List<AbsynStatement>();
                AbsynWhile w = new AbsynWhile(
                    node.Then == node.LoopFollow
                        ? branch.Condition.Invert()
                        : branch.Condition,
                    whileBody);


                // write the code for the body of the loop
                StructureNode loopBody = (node.Else == node.LoopFollow) ? node.Then : node.Else;
                GenerateCode(loopBody, new AbsynCodeGeneratorState(node.LatchNode, state.FollowSet, state.GotoSet, whileBody));

                if (!visited.Contains(node.LatchNode))
                {
                    visited.Add(node.LatchNode);
                    state.GenerateBlockCode(node.LatchNode);
                }

                // rewrite the body of the block (excluding the predicate) at the next nesting level
                // after making sure another label won't be generated
                state.RequireLabel(node, false);
                state.GenerateBlockCode(node);
            }
            else
            {
                Instruction last = node.LastInstruction;
                List<AbsynStatement> loopBody = new List<AbsynStatement>();

                // if this is also a conditional header, then generate code for the
                // conditional. Otherwise generate code for the loop body.
                AbsynCodeGeneratorState bodyState = new AbsynCodeGeneratorState(node.LatchNode, state.FollowSet, state.GotoSet, loopBody);
                if (node.IsStructType(StructuredGraph.LoopCond))
                {
                    // set the necessary flags so that WriteCode can successfully be called
                    // again on this node
                    node.SetStructType(StructuredGraph.Cond);
                    visited.Remove(node);
                    GenerateCode(node, bodyState);
                }
                else
                {
                    bodyState.GenerateBlockCode(node);
                    GenerateCode(node.Succ[0], bodyState);
                }

                AbsynLoop loop;
                if (node.LoopType == loopType.PostTested)
                {
                    loop = new AbsynDoWhile(loopBody, ((Branch)last).Condition);
                    state.Stms.Add(loop);
                    if (!visited.Contains(node.LatchNode))
                    {
                        visited.Add(node.LatchNode);
                        state.GenerateBlockCode(node.LatchNode);
                    }
                }
                else
                {
                    Debug.Assert(node.LoopType == loopType.Endless);
                    loop = new AbsynWhile(Constant.True(), loopBody);
                    state.Stms.Add(loop);
                    if (!visited.Contains(node.LatchNode))
                    {
                        visited.Add(node.LatchNode);
                        state.GenerateBlockCode(node.LatchNode);
                    }
                }
            }

            // write the code for the follow of the loop (if it exists)
            if (node.LoopFollow != null)
            {
                // remove the follow from the follow set
                state.FollowSet.RemoveAt(state.FollowSet.Count - 1);

                if (!visited.Contains(node.LoopFollow))
                    GenerateCode(node.LoopFollow, state);
                else
                    EmitGotoAndLabel(state, node, node.LoopFollow);
            }
        }


        // Return true if every parent of this node has had its code generated
        private bool AllParentsGenerated(StructureNode node)
        {
            foreach (StructureNode p in node.Pred)
                if (!p.HasBackEdgeTo(node) && !visited.Contains(p))
                    return false;
            return true;
        }

        // Emit a goto statement to the given destination as well as making sure that
        // this destination gives itself a label
        // Emits a goto statement (at the correct indentation level) with the destination label for dest.
        // Also places the label just before the destination code if it isn't already there.
        // If the goto is to the return block, emit a 'return' instead.
        // Also, 'continue' and 'break' statements are used instead if possible
        public void EmitGotoAndLabel(AbsynCodeGeneratorState state, StructureNode node, StructureNode dest)
        {
            state.EmitGotoAndLabel(node, dest);
        }

    }
}

