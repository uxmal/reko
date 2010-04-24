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
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public abstract class Conditional
    {
        private StructureNode follow;

        public Conditional(StructureNode follow)
        {
            this.follow = follow;
        }

        public StructureNode Follow
        {
            get { return follow; }
        }

        public abstract void GenerateCode(AbsynCodeGenerator absynCodeGenerator2, StructureNode node, StructureNode latchNode, AbsynStatementEmitter emitter);

    }

    public abstract class IfConditional : Conditional
    {
        public IfConditional(StructureNode follow) : base(follow) { } 

        public override void GenerateCode(AbsynCodeGenerator codeGen, StructureNode node, StructureNode latchNode, AbsynStatementEmitter emitter)
        {
            codeGen.EmitLinearBlockStatements(node, emitter);
            if (node == latchNode)
                return;

            Expression exp = codeGen.BranchCondition(node);
            AbsynIf ifStm = EmitIfCondition(exp, this, emitter);

            StructureNode succ = FirstBranch(node);
            AbsynStatementEmitter emitThen = new AbsynStatementEmitter(ifStm.Then);
            if (node.UnstructType == UnstructuredType.JumpInOutLoop)
            {
                codeGen.DeferRendering(node, succ, emitThen);
                codeGen.GenerateCode(SecondBranch(node), latchNode, emitter);
            }
            else
            {
                if (Follow == null)
                    throw new NotSupportedException("Null condfollow");
                codeGen.PushFollow(Follow);

                if (node.Conditional is IfThenElse)
                {
                    codeGen.IncompleteNodes.Add(node);
                }

                if (codeGen.IsVisited(succ) || (node.Loop != null && succ == node.Loop.Follow))
                    codeGen.EmitGotoAndForceLabel(node, succ, emitThen);
                else
                    codeGen.GenerateCode(succ, latchNode, emitThen);

                if (node.Conditional is IfThenElse)
                {
                    codeGen.IncompleteNodes.Remove(node);

                    succ = node.Then;
                    AbsynStatementEmitter emitElse = new AbsynStatementEmitter(ifStm.Else);
                    if (codeGen.IsVisited(succ))
                        codeGen.EmitGotoAndForceLabel(node, succ, emitElse);
                    else
                        codeGen.GenerateCode(succ, latchNode, emitElse);

                    if (HasSingleIfThenElseStatement(ifStm.Then))
                    {
                        ifStm.InvertCondition();
                    }
                }

                codeGen.PopFollow();
                codeGen.GenerateCode(Follow, latchNode, emitter);
            }
        }

        private AbsynIf EmitIfCondition(Expression exp, Conditional cond, AbsynStatementEmitter emitter)
        {
            if (cond is IfElse || cond is IfThenElse)
            {
                exp = exp.Invert();
            }
            AbsynIf ifStm = new AbsynIf();
            ifStm.Condition = exp;
            emitter.EmitStatement(ifStm);

            return ifStm;
        }


        private bool HasSingleIfThenElseStatement(List<AbsynStatement> stms)
        {
            if (stms.Count == 0)
                return false;
            AbsynIf ifStm = stms[0] as AbsynIf;
            if (ifStm == null)
                return false;
            return ifStm.Then.Count > 0 && ifStm.Else.Count > 0;
        }

        public abstract StructureNode FirstBranch(StructureNode node);
        public abstract StructureNode SecondBranch(StructureNode node);

    }

    public class IfThen : IfConditional
    {
        public IfThen(StructureNode follow) : base(follow) { } 
        
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Then;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Else;
        }
    }

    public class IfElse : IfConditional
    {
        public IfElse(StructureNode follow) : base(follow) { } 
        
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Else;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Then;
        }

    }

    public class IfThenElse : IfConditional
    {
        public IfThenElse(StructureNode follow) : base(follow) { } 

        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Else;
        }

        public override StructureNode SecondBranch(StructureNode node)
        {
            return node.Then;
        }
    }

    public class Case : Conditional
    {
        public Case(StructureNode follow) : base(follow) { }

        public override void GenerateCode(AbsynCodeGenerator codeGen, StructureNode node, StructureNode latchNode, AbsynStatementEmitter emitter)
        {
            codeGen.EmitLinearBlockStatements(node, emitter);

            Expression exp = ((SwitchInstruction) node.Instructions.Last.Instruction).Expression;
            AbsynSwitch switchStm = emitter.EmitSwitch(node, exp);
            AbsynStatementEmitter emitSwitchBranches = new AbsynStatementEmitter(switchStm.Statements);

            if (Follow == null)
                throw new NotSupportedException(string.Format("Null follow node for case statement at {0} is not supported.", node.Name));
            codeGen.PushFollow(Follow);
            for (int i = 0; i < node.OutEdges.Count; i++)
            {
                emitSwitchBranches.EmitCaseLabel(node, i);

                StructureNode succ = node.OutEdges[i];
                if (codeGen.IsVisited(succ))
                {
                    codeGen.EmitGotoAndForceLabel(node, succ, emitSwitchBranches);
                }
                else
                {
                    codeGen.GenerateCode(succ, latchNode, emitSwitchBranches);
                    emitSwitchBranches.EmitBreak();
                }
            }
            codeGen.PopFollow();
            codeGen.GenerateCode(Follow, latchNode, emitter);
        }
    }
}
