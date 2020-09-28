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
        private ExpressionMatcher matcher;

        public RtlInstructionMatcher(RtlInstruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null);
        }

        public Expression CapturedExpressions(string label) { return matcher.CapturedExpression(label); }

        public Operator CapturedOperators(string label) { return matcher.CapturedOperators(label); }

        public bool Match(RtlInstruction instr)
        {
            matcher.Clear();
            return instr.Accept(this);
        }

        #region RtlInstructionVisitor<bool>

        public bool VisitAssignment(RtlAssignment ass)
        {
            var assPat = pattern as RtlAssignment;
            if (assPat == null)
                return false;
            matcher.Pattern = assPat.Src;
            if (!matcher.Match(ass.Src))
                return false;
            matcher.Pattern = assPat.Dst;
            return matcher.Match(ass.Dst);
        }

        public bool VisitGoto(RtlGoto go)
        {
            var gPat = pattern as RtlGoto;
            if (gPat == null)
                return false;
            matcher.Pattern = gPat.Target;
            return matcher.Match(go.Target);
        }

        public bool VisitIf(RtlIf rtlIf)
        {
            var pIf = pattern as RtlIf;
            if (pIf == null)
                return false;
            var p = pattern;
            pattern = pIf.Instruction;
            var ret = rtlIf.Instruction.Accept(this);
            pattern = p;
            return ret;
        }

        public bool VisitInvalid(RtlInvalid invalid)
        {
            var pInvalid = pattern as RtlInvalid;
            return pInvalid != null;
        }

        public bool VisitNop(RtlNop nop)
        {
            return pattern is RtlNop;
        }

        public bool VisitBranch(RtlBranch branch)
        {
            var branchPat = pattern as RtlBranch;
            if (branchPat == null)
                return false;
            matcher.Pattern = branchPat.Condition;
            return matcher.Match(branch.Condition);
        }

        public bool VisitCall(RtlCall call)
        {
            var callPat = pattern as RtlCall;
            if (callPat == null)
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
            var sidePat = pattern as RtlSideEffect;
            if (sidePat == null)
                return false;
            matcher.Pattern = sidePat.Expression;
            return matcher.Match(side.Expression);
        }

        #endregion
    }
}
