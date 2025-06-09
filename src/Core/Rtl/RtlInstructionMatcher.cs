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
    /// <summary>
    /// Matches an RTL instruction against a pattern.
    /// </summary>
    public class RtlInstructionMatcher : IRtlInstructionVisitor<bool, ExpressionMatch>
    {
        private RtlInstruction pattern;
        private readonly ExpressionMatcher matcher;

        /// <summary>
        /// Constructs an <see cref="RtlInstructionMatcher"/> that matches RTL code agains
        /// the provided pattern.
        /// </summary>
        /// <param name="pattern">Pattern to match.
        /// </param>
        public RtlInstructionMatcher(RtlInstruction pattern)
        {
            this.pattern = pattern;
            this.matcher = new ExpressionMatcher(null!);
        }

        /// <summary>
        /// Builds a matcher from the provided builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static RtlInstructionMatcher Build(Action<RtlInstructionMatcherEmitter> builder)
        {
            var instrs = new List<RtlInstruction>();
            var m = new RtlInstructionMatcherEmitter(instrs);
            builder(m);
            return new RtlInstructionMatcher(instrs.Last());
        }

        /// <summary>
        /// Builds an array of matchers from the provided builders.
        /// </summary>
        /// <param name="builders">Array of builder functions to build the matchers.
        /// </param>
        /// <returns>An array of <see cref="RtlInstructionMatcher"/>s.</returns>
        public static RtlInstructionMatcher[] Build(params Action<RtlInstructionMatcherEmitter>[] builders)
        {
            var instrs = new List<RtlInstruction>();
            var matchers = new List<RtlInstructionMatcher>();
            foreach (var builder in builders)
            {
                var m = new RtlInstructionMatcherEmitter(instrs);
                builder(m);
                matchers.Add(new RtlInstructionMatcher(instrs.Last()));
            }
            return matchers.ToArray();
        }

        /// <summary>
        /// Match an RTL instruction against the pattern.
        /// </summary>
        /// <param name="instr">Instruction to match.</param>
        /// <returns>The result of the match, expressed in an <see cref="ExpressionMatch"/> instance.
        /// </returns>
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

        /// <inheritdoc/>
        public bool VisitAssignment(RtlAssignment ass, ExpressionMatch m)
        {
            if (pattern is not RtlAssignment assPat)
                return false;
            if (!matcher.Match(assPat.Src, ass.Src, m))
                return false;
            return matcher.Match(assPat.Dst, ass.Dst, m);
        }

        /// <inheritdoc/>
        public bool VisitGoto(RtlGoto go, ExpressionMatch m)
        {
            if (pattern is not RtlGoto gPat)
                return false;
            return matcher.Match(gPat.Target, go.Target, m);
        }

        /// <inheritdoc/>
        public bool VisitMicroGoto(RtlMicroGoto mgo, ExpressionMatch m)
        {
            if (pattern is not RtlMicroGoto mgPat)
                return false;
            if (mgPat.Condition is not null)
            {
                if (mgo.Condition is null)
                    return false;
                if (!matcher.Match(mgPat.Condition, mgo.Condition, m))
                    return false;
            }
            return mgo.Target == mgPat.Target;
        }

        /// <inheritdoc/>
        public bool VisitIf(RtlIf rtlIf, ExpressionMatch m)
        {
            if (pattern is not RtlIf pIf)
                return false;
            if (matcher.Match(pIf.Condition, rtlIf.Condition, m))
                return false;
            return this.Match(pIf.Instruction, rtlIf.Instruction, m);
        }

        /// <inheritdoc/>
        public bool VisitInvalid(RtlInvalid invalid, ExpressionMatch m)
        {
            return pattern is RtlInvalid;
        }

        /// <inheritdoc/>
        public bool VisitNop(RtlNop nop, ExpressionMatch m)
        {
            return pattern is RtlNop;
        }

        /// <inheritdoc/>
        public bool VisitBranch(RtlBranch branch, ExpressionMatch m)
        {
            if (pattern is not RtlBranch branchPat)
                return false;
            return matcher.Match(branchPat.Condition, branch.Condition, m);
        }

        /// <inheritdoc/>
        public bool VisitCall(RtlCall call, ExpressionMatch m)
        {
            if (pattern is not RtlCall callPat)
                return false;
            return matcher.Match(callPat.Target, call.Target, m);
        }

        /// <inheritdoc/>
        public bool VisitReturn(RtlReturn ret, ExpressionMatch m)
        {
            return pattern is RtlReturn;
        }

        /// <inheritdoc/>
        public bool VisitSideEffect(RtlSideEffect side, ExpressionMatch m)
        {
            if (pattern is not RtlSideEffect sidePat)
                return false;
            return matcher.Match(sidePat.Expression, side.Expression, m);
        }

        /// <inheritdoc/>
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
