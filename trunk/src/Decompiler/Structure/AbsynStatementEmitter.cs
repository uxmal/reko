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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public class AbsynStatementEmitter : InstructionVisitor
    {
        private List<AbsynStatement> stms;

        public AbsynStatementEmitter(List<AbsynStatement> stms)
        {
            this.stms = stms;
        }

        public void EmitStatement(Statement stm)
        {
            stm.Instruction.Accept(this);
        }

        [Obsolete]
        public AbsynIf EmitIfCondition(Expression exp, StructureNode node)
        {
            return EmitIfCondition(exp, node.Conditional);
        }

        //$REVIEW: consider moving this to Conditional.
        public AbsynIf EmitIfCondition(Expression exp, Conditional cond)
        {
            if (cond == Conditional.IfElse ||
                cond == Conditional.IfThenElse)
            {
                exp = exp.Invert();
            }
            AbsynIf ifStm = new AbsynIf();
            ifStm.Condition = exp;
            stms.Add(ifStm);

            return ifStm;
        }

        public void EmitCaseLabel(StructureNode node, int i)
        {
            stms.Add(new AbsynCase(i));
        }

        public void EmitBreak(StructureNode node)
        {
            stms.Add(new AbsynBreak());
        }

        public void EmitContinue(StructureNode node)
        {
            stms.Add(new AbsynContinue());
        }

        public void EmitGoto(StructureNode dest)
        {
            stms.Add(new AbsynGoto(dest.Block.Name));
        }

        public void EmitReturn(Expression expr)
        {
            stms.Add(new AbsynReturn(expr));
        }

        public void EmitLabel(StructureNode node)
        {
            stms.Add(new AbsynLabel(node.Block.Name));
        }

        public void EmitForever(StructureNode node, List<AbsynStatement> body)
        {
            AbsynWhile whileStm = new AbsynWhile(Constant.True(), body);
            stms.Add(whileStm);
        }

        public void EmitDoWhile(StructureNode node, List<AbsynStatement> body, Expression expr)
        {
            AbsynDoWhile doWhile = new AbsynDoWhile(body, expr);
            stms.Add(doWhile);
        }

        public void EmitWhile(StructureNode node, Expression expr, List<AbsynStatement> body)
        {
            if (node.Then == node.LoopFollow)
                expr = expr.Invert();
            stms.Add(new AbsynWhile(expr, body));
        }

        public AbsynSwitch EmitSwitch(StructureNode node, Expression exp)
        {
            AbsynSwitch switchStm = new AbsynSwitch(exp);
            stms.Add(switchStm);
            return switchStm;
        }


        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            stms.Add(new AbsynAssignment(a.Dst, a.Src));
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            stms.Add(new AbsynDeclaration(decl.Identifier, decl.Expression));
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            stms.Add(new AbsynSideEffect(new Application(ic.Callee, PrimitiveType.Void)));
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            stms.Add(new AbsynReturn(ret.Expression));
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            stms.Add(new AbsynSideEffect(side.Expression));
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            stms.Add(new AbsynAssignment(store.Dst, store.Src));
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
