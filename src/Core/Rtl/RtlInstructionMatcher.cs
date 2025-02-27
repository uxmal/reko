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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Rtl
{
    public class RtlInstructionMatcher : IRtlInstructionVisitor<bool, ExpressionMatch>
    {
        private RtlInstruction pattern;
        private readonly ExpressionMatcher matcher;

        public RtlInstructionMatcher(RtlInstruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null!);
        }

        public static RtlInstructionMatcher Build(Action<RtlInstructionMatcherEmitter> builder)
        {
            var instrs = new List<RtlInstruction>();
            var m = new RtlInstructionMatcherEmitter(instrs);
            builder(m);
            return new RtlInstructionMatcher(instrs.Last());
        }

        public ExpressionMatch Match(RtlInstruction instr)
        {
            var match = new ExpressionMatch();
            match.Success = instr.Accept(this, match);
            return match;
        }

        private bool Match(RtlInstruction pattern, RtlInstruction instr, ExpressionMatch m)
        {
            throw new NotImplementedException();
        }

        #region RtlInstructionVisitor<bool>

        public bool VisitAssignment(RtlAssignment ass, ExpressionMatch m)
        {
            if (pattern is not RtlAssignment assPat)
                return false;
            if (!matcher.Match(assPat.Src, ass.Src, m))
                return false;
            return matcher.Match(assPat.Dst, ass.Dst, m);
        }

        public bool VisitGoto(RtlGoto go, ExpressionMatch m)
        {
            if (pattern is not RtlGoto gPat)
                return false;
            return matcher.Match(gPat.Target, go.Target, m);
        }

        public bool VisitMicroGoto(RtlMicroGoto mgo, ExpressionMatch m)
        {
            if (pattern is not RtlMicroGoto mgPat)
                return false;
            if (mgPat.Condition != null)
            {
                if (mgo.Condition == null)
                    return false;
                if (!matcher.Match(mgPat.Condition, mgo.Condition, m))
                    return false;
            }
            return mgo.Target == mgPat.Target;
        }

        public bool VisitIf(RtlIf rtlIf, ExpressionMatch m)
        {
            if (pattern is not RtlIf pIf)
                return false;
            if (matcher.Match(pIf.Condition, rtlIf.Condition, m))
                return false;
            return this.Match(pIf.Instruction, rtlIf.Instruction, m);
        }

        public bool VisitInvalid(RtlInvalid invalid, ExpressionMatch m)
        {
            return pattern is RtlInvalid;
        }

        public bool VisitNop(RtlNop nop, ExpressionMatch m)
        {
            return pattern is RtlNop;
        }

        public bool VisitBranch(RtlBranch branch, ExpressionMatch m)
        {
            if (pattern is not RtlBranch branchPat)
                return false;
            return matcher.Match(branchPat.Condition, branch.Condition, m);
        }

        public bool VisitCall(RtlCall call, ExpressionMatch m)
        {
            if (pattern is not RtlCall callPat)
                return false;
            return matcher.Match(callPat.Target, call.Target, m);
        }

        public bool VisitReturn(RtlReturn ret, ExpressionMatch m)
        {
            return pattern is RtlReturn;
        }

        public bool VisitSideEffect(RtlSideEffect side, ExpressionMatch m)
        {
            if (pattern is not RtlSideEffect sidePat)
                return false;
            return matcher.Match(sidePat.Expression, side.Expression, m);
        }

        public bool VisitSwitch(RtlSwitch sw, ExpressionMatch m)
        {
            if (pattern is not RtlSwitch swPat)
                return false;
            if (swPat.Targets.Length != sw.Targets.Length)
                return false;
            return matcher.Match(swPat.Expression, sw.Expression, m);
        }

        #endregion
    }
}
