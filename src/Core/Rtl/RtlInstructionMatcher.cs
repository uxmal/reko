#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Rtl
{
    public class RtlInstructionMatcher : RtlInstructionVisitor<bool>
    {
        private RtlInstruction pattern;
        private readonly ExpressionMatcher matcher;

        public RtlInstructionMatcher(RtlInstruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null!);
        }

        public Expression? CapturedExpressions(string label) { return matcher.CapturedExpression(label); }

        public Operator? CapturedOperators(string label) { return matcher.CapturedOperators(label); }

        public bool Match(RtlInstruction instr)
        {
            matcher.Clear();
            return instr.Accept(this);
        }

        #region RtlInstructionVisitor<bool>

        public bool VisitAssignment(RtlAssignment ass)
        {
            if (!(pattern is RtlAssignment assPat))
                return false;
            matcher.Pattern = assPat.Src;
            if (!matcher.Match(ass.Src))
                return false;
            matcher.Pattern = assPat.Dst;
            return matcher.Match(ass.Dst);
        }

        public bool VisitGoto(RtlGoto go)
        {
            if (!(pattern is RtlGoto gPat))
                return false;
            matcher.Pattern = gPat.Target;
            return matcher.Match(go.Target);
        }

        public bool VisitMicroGoto(RtlMicroGoto mgo)
        {
            if (!(pattern is RtlMicroGoto mgPat))
                return false;
            if (mgPat.Condition != null)
            {
                if (mgo.Condition == null)
                    return false;
                matcher.Pattern = mgPat.Condition;
                if (!matcher.Match(mgo.Condition))
                    return false;
            }
            return mgo.Target == mgPat.Target;
        }

        public bool VisitMicroLabel(RtlMicroLabel mlabel)
        {
            return (pattern is RtlMicroLabel mpattern &&
                mlabel.Name == mpattern.Name);
        }

        public bool VisitIf(RtlIf rtlIf)
        {
            if (!(pattern is RtlIf pIf))
                return false;
            var p = pattern;
            pattern = pIf.Instruction;
            var ret = rtlIf.Instruction.Accept(this);
            pattern = p;
            return ret;
        }

        public bool VisitInvalid(RtlInvalid invalid)
        {
            return pattern is RtlInvalid;
        }

        public bool VisitNop(RtlNop nop)
        {
            return pattern is RtlNop;
        }

        public bool VisitBranch(RtlBranch branch)
        {
            if (!(pattern is RtlBranch branchPat))
                return false;
            matcher.Pattern = branchPat.Condition;
            return matcher.Match(branch.Condition);
        }

        public bool VisitCall(RtlCall call)
        {
            if (!(pattern is RtlCall callPat))
                return false;
            matcher.Pattern = callPat.Target;
            return matcher.Match(call.Target);
        }

        public bool VisitReturn(RtlReturn ret)
        {
            return pattern is RtlReturn;
        }

        public bool VisitSideEffect(RtlSideEffect side)
        {
            if (!(pattern is RtlSideEffect sidePat))
                return false;
            matcher.Pattern = sidePat.Expression;
            return matcher.Match(side.Expression);
        }

        #endregion
    }
}
