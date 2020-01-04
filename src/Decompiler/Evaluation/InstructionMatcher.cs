#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Evaluation
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
            return matcher.Match(ass.Dst);
        }

        public bool VisitBranch(Branch branch)
        {
            var branchPat = pattern as Branch;
            if (branchPat == null)
                return false;
            matcher.Pattern = branchPat.Condition;
            return matcher.Match(branch.Condition);
        }

        public bool VisitCallInstruction(CallInstruction ci)
        {
            var callPat = pattern as CallInstruction;
            if (callPat == null)
                return false;
            return true;
        }

        public bool VisitComment(CodeComment comment)
        {
            var commentPat = pattern as CodeComment;
            if (commentPat == null)
                return false;
            return true;
        }

        public bool VisitDeclaration(Declaration decl)
        {
            return false;
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            var defPat = pattern as DefInstruction;
            if (defPat == null)
                return false;
            throw new NotImplementedException();
        }

        public bool VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            var gotoPat = pattern as GotoInstruction;
            if (gotoPat == null)
                return false;
            matcher.Pattern = gotoPat.Target;
            return matcher.Match(gotoInstruction.Target);
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            var defPat = pattern as PhiAssignment;
            if (defPat == null)
                return false;
            throw new NotImplementedException();
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            var retPat = pattern as ReturnInstruction;
            if (retPat == null)
                return false;
            if (retPat.Expression == null && ret.Expression == null)
                return true;
            if (retPat.Expression == null || ret.Expression == null)
                return false;
            matcher.Pattern = retPat.Expression;
            return matcher.Match(ret.Expression);
        }

        public bool VisitSideEffect(SideEffect side)
        {
            var sidePat = pattern as SideEffect;
            if (sidePat == null)
                return false;
            matcher.Pattern = sidePat.Expression;
            return matcher.Match(side.Expression);
        }

        public bool VisitStore(Store store)
        {
            var storePat = pattern as Store;
            if (storePat == null)
                return false;
            matcher.Pattern = storePat.Src;
            if (!matcher.Match(store.Src))
                return false;
            matcher.Pattern = storePat.Dst;
            return matcher.Match(store.Dst);
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            var swPat = pattern as SwitchInstruction;
            if (swPat == null)
                return false;
            matcher.Pattern = swPat.Expression;
            return matcher.Match(si.Expression);
        }

        public bool VisitUseInstruction(UseInstruction u)
        {
            var uPattern = pattern as UseInstruction;
            if (uPattern == null)
                return false;
            matcher.Pattern = uPattern.Expression;
            return matcher.Match(u.Expression);
        }

        #endregion


    }
}
