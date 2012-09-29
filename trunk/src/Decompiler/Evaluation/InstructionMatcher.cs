#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Evaluation
{
    public class InstructionMatcher : InstructionVisitor<bool>
    {
        private Instruction pattern;
        private ExpressionMatcher matcher;

        public InstructionMatcher(Instruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null);
        }

        public Expression CapturedExpressions(string label) { return matcher.CapturedExpression(label); }

        public Operator CapturedOperators(string label) { return matcher.CapturedOperators(label); }

        public bool Match(Instruction instr)
        {
            matcher.Clear();
            return instr.Accept(this);
        }

        #region InstructionVisitor<bool>

        public bool VisitAssignment(Assignment ass)
        {
            var assPat = pattern as Assignment;
            if (assPat == null)
                return false;
            matcher.Pattern = assPat.Src;
            if (!matcher.Match(ass.Src))
                return false;
            matcher.Pattern = assPat.Dst;
            if (!matcher.Match(ass.Dst))
                return false;
            return true;
        }

        public bool VisitBranch(Branch b)
        {
            throw new NotImplementedException();
        }

        public bool VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        public bool VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public bool VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public bool VisitIndirectCall(IndirectCall ic)
        {
            throw new NotImplementedException();
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        public bool VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        public bool VisitStore(Store store)
        {
            throw new NotImplementedException();
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public bool VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
