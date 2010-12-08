#region License
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
#endregion

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Analysis
{
    public class SymbolicEvaluator : InstructionVisitor, EvaluationContext
    {
        private ExpressionValuePropagator eval;

        public SymbolicEvaluator()
        {
            State = new Dictionary<Expression, Expression>();
            eval = new ExpressionValuePropagator(this);
        }

        public void Evaluate(Instruction instr)
        {
            instr.Accept(this);
        }

        public Dictionary<Expression, Expression> State { get; private set; }

        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            var valSrc = a.Src.Accept(eval);
            State[a.Dst] = valSrc;
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitIndirectCall(IndirectCall ic)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region EvaluationContext Members

        public Expression DefiningExpression(Identifier id)
        {
            throw new NotImplementedException();
        }

        public void RemoveIdentifierUse(Identifier id)
        {
            throw new NotImplementedException();
        }

        public void UseExpression(Expression e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
