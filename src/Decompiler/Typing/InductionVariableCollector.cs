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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Typing
{
    /// <summary>
    /// Collects induction variables, including derived induction variables.
    /// </summary>
    public class InductionVariableCollector : ExpressionVisitorBase<LinearInductionVariable>
    {
        private Program program;

        public InductionVariableCollector(Program program)
        {
            this.program = program;
        }

        public override LinearInductionVariable VisitIdentifier(Identifier id)
        {
            LinearInductionVariable iv;
            if (!program.InductionVariables.TryGetValue(id, out iv))
                return null;
            else 
                return iv;
        }

        public override LinearInductionVariable VisitBinaryExpression(BinaryExpression binExp)
        {
            var ivLeft = binExp.Left.Accept(this);
            var ivRight = binExp.Right.Accept(this);
            if (ivLeft != null)
            {
                if (binExp.Operator == Operator.SMul ||
                    binExp.Operator == Operator.UMul ||
                    binExp.Operator == Operator.IMul || 
                    binExp.Operator == Operator.Shl)
                    return MergeInductionVariableConstant(ivLeft, binExp.Operator, binExp.Right as Constant);
            }
            return null;
        }

        public override LinearInductionVariable VisitMemoryAccess(MemoryAccess access)
        {
            return null;
        }

        public override LinearInductionVariable VisitSegmentedAccess(SegmentedAccess access)
        {
            return null;
        }

        public LinearInductionVariable MergeInductionVariableConstant(LinearInductionVariable iv, Operator op, Constant c)
        {
            if (iv == null || c == null)
                return null;
            Constant delta = op.ApplyConstants(iv.Delta, c);
            Constant initial = (iv.Initial != null) ? op.ApplyConstants(iv.Initial, c) : null;
            Constant final = (iv.Final != null) ? op.ApplyConstants(iv.Final, c) : null;
            return new LinearInductionVariable(initial, delta, final, false);
        }
    }
}
