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
using Reko.Core.Operators;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitUnaryExpression(UnaryExpression unary)
        {
            var (e, changed) = unary.Expression.Accept(this);
            if (changed)
                unary = new UnaryExpression(unary.Operator, unary.DataType, e);
            e = negSub.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            e = LogicalNotComparison(unary);
            if (e is not null)
            {
                return (e, true);
            }
            e = compSub.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            // (!-exp) >= (!exp)
            e = logicalNotFollowedByNeg.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            if (unary.Expression is Constant c && c.IsValid && unary.Operator.Type != OperatorType.AddrOf)
            {
                var c2 = unary.Operator.ApplyConstant(c);
                return (c2, true);
            }
            return (unary, changed);
        }

        private static Expression? LogicalNotComparison(UnaryExpression unary)
        {
            if (unary.Operator.Type == OperatorType.Not &&
                unary.Expression is BinaryExpression bin &&
                bin.Operator is ConditionalOperator cond)
            {
                return new BinaryExpression((BinaryOperator)cond.Invert(), bin.DataType, bin.Left, bin.Right);
            }
            return null;
        }
    }
}
