#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Code
{
    /// <summary>
    /// Matches an instruction against a pattern.
    /// </summary>
    public class InstructionMatcher : InstructionVisitor<bool, ExpressionMatch>
    {
        private Instruction pattern;
        private ExpressionMatcher matcher;

        /// <summary>
        /// Constructs an instruction matcher.
        /// </summary>
        /// <param name="pattern">Pattern to use when matching.
        /// </param>
        public InstructionMatcher(Instruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null!);
        }

        /// <summary>
        /// Matches an instruction against the pattern.
        /// </summary>
        /// <param name="instr">Instruction to match.</param>
        /// <returns>An <see cref="ExpressionMatch"/> containing
        /// any captured expressions, if there was a match.
        /// </returns>
        public ExpressionMatch Match(Instruction instr)
        {
            var match = new ExpressionMatch();
            match.Success = instr.Accept(this, match);
            return match;
        }

        #region InstructionVisitor<bool>

        /// <inheritdoc/>
        public bool VisitAssignment(Assignment ass, ExpressionMatch m)
        {
            if (pattern is not Assignment assPat)
                return false;
            if (!matcher.Match(assPat.Src, ass.Src, m))
                return false;
            return matcher.Match(assPat.Dst, ass.Dst, m);
        }

        /// <inheritdoc/>
        public bool VisitBranch(Branch branch, ExpressionMatch m)
        {
            if (pattern is not Branch branchPat)
                return false;
            return matcher.Match(branchPat.Condition, branch.Condition, m);
        }

        /// <inheritdoc/>
        public bool VisitCallInstruction(CallInstruction ci, ExpressionMatch m)
        {
            return pattern is CallInstruction;
        }

        /// <inheritdoc/>
        public bool VisitComment(CodeComment comment, ExpressionMatch m)
        {
            if (pattern is not CodeComment commentPat)
                return false;
            return true;
        }

        /// <inheritdoc/>
        public bool VisitDefInstruction(DefInstruction def, ExpressionMatch m)
        {
            if (pattern is not DefInstruction defPat)
                return false;
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitGotoInstruction(GotoInstruction gotoInstruction, ExpressionMatch m)
        {
            if (pattern is not GotoInstruction gotoPat)
                return false;
            return matcher.Match(gotoPat.Target, gotoInstruction.Target, m);
        }

        /// <inheritdoc/>
        public bool VisitPhiAssignment(PhiAssignment phi, ExpressionMatch m)
        {
            if (pattern is not PhiAssignment defPat)
                return false;
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool VisitReturnInstruction(ReturnInstruction ret, ExpressionMatch m)
        {
            if (pattern is not ReturnInstruction retPat)
                return false;
            if (retPat.Expression is null && ret.Expression is null)
                return true;
            if (retPat.Expression is null || ret.Expression is null)
                return false;
            return matcher.Match(retPat.Expression, ret.Expression, m);
        }

        /// <inheritdoc/>
        public bool VisitSideEffect(SideEffect side, ExpressionMatch m)
        {
            if (pattern is not SideEffect sidePat)
                return false;
            return matcher.Match(sidePat.Expression, side.Expression, m);
        }

        /// <inheritdoc/>
        public bool VisitStore(Store store, ExpressionMatch m)
        {
            if (pattern is not Store storePat)
                return false;
            if (!matcher.Match(storePat.Src, store.Src, m))
                return false;
            return matcher.Match(storePat.Dst ,store.Dst, m);
        }

        /// <inheritdoc/>
        public bool VisitSwitchInstruction(SwitchInstruction si, ExpressionMatch m)
        {
            if (pattern is not SwitchInstruction swPat)
                return false;
            return matcher.Match(swPat.Expression, si.Expression, m);
        }

        /// <inheritdoc/>
        public bool VisitUseInstruction(UseInstruction u, ExpressionMatch m)
        {
            if (pattern is not UseInstruction uPattern)
                return false;
            return matcher.Match(uPattern.Expression, u.Expression, m);
        }

        #endregion


    }
}
