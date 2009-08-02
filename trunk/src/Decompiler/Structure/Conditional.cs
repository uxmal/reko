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
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public abstract class Conditional
    {
        private static Conditional ifThen = new IfThen();
        private static Conditional ifElse = new IfElse();
        private static Conditional ifThenElse = new IfThenElse();
        private static Conditional @case = new Case();

        public static Conditional IfThen
        {
            get { return Conditional.ifThen; }
        }

        public static Conditional IfElse
        {
            get { return Conditional.ifElse; }
        }

        public static Conditional IfThenElse
        {
            get { return Conditional.ifThenElse; }
        }

        public static Conditional Case
        {
            get { return Conditional.@case; }
        }

        public abstract void GenerateCode(StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, AbsynCodeGenerator codeGen, AbsynStatementEmitter emitter);
    }

    public abstract class IfConditional : Conditional
    {
        public override void GenerateCode(StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, AbsynCodeGenerator codeGen, AbsynStatementEmitter emitter)
        {
            Expression exp = ((Branch) node.Instructions.Last.Instruction).Condition;
            AbsynIf ifStm = emitter.EmitIfCondition(exp, node);
            StructureNode succ = FirstBranch(node);
            AbsynStatementEmitter emitThen = new AbsynStatementEmitter(ifStm.Then);
            // emit a goto statement if the first clause has already been generated or it
            // is the follow of this node's enclosing loop
            if (succ.traversed == travType.DFS_CODEGEN || (node.LoopHead != null && succ == node.LoopHead.LoopFollow))
                codeGen.EmitGotoAndLabel(node, succ, emitThen);
            else
                codeGen.WriteCode(succ, latch, followSet, gotoSet, emitThen);

            if (node.Conditional == Conditional.IfThenElse)
            {
                succ = node.Else;
                AbsynStatementEmitter emitElse = new AbsynStatementEmitter(ifStm.Else);
                if (succ.traversed == travType.DFS_CODEGEN)
                    codeGen.EmitGotoAndLabel(node, succ, emitElse);
                else
                    codeGen.WriteCode(succ, latch, followSet, gotoSet, emitElse);

                if (HasSingleIfThenElseStatement(ifStm.Then))
                {
                    ifStm.InvertCondition();
                }
            }
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

    }

    public class IfThen : IfConditional
    {
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Then;
        }
    }

    public class IfElse : IfConditional
    {
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Else;
        }

    }

    public class IfThenElse : IfConditional
    {
        public override StructureNode FirstBranch(StructureNode node)
        {
            return node.Then;
        }
    }

    public class Case : Conditional
    {
        public override void GenerateCode(StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, 
            AbsynCodeGenerator codeGen, AbsynStatementEmitter emitter)
        {
            Expression exp = ((SwitchInstruction) node.Instructions.Last.Instruction).Expression;
            AbsynSwitch switchStm = emitter.EmitSwitch(node, exp);
            AbsynStatementEmitter emitSwitchBranches = new AbsynStatementEmitter(switchStm.Statements);
            // generate code for each out branch
            for (int i = 0; i < node.OutEdges.Count; i++)
            {
                emitSwitchBranches.EmitCaseLabel(node, i);

                StructureNode succ = node.OutEdges[i];
                if (succ.traversed == travType.DFS_CODEGEN)
                {
                    codeGen.EmitGotoAndLabel(node, succ, emitSwitchBranches);
                }
                else
                {
                    codeGen.WriteCode(succ, latch, followSet, gotoSet, emitSwitchBranches);
                    emitSwitchBranches.EmitBreak(node);
                }
            }
        }
    }
}
