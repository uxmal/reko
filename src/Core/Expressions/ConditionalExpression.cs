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

using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models the C ternary ?: operator.
    /// </summary>
    public class ConditionalExpression : AbstractExpression
    {
        /// <summary>
        /// Constructs a ternary conditional expression.
        /// </summary>
        /// <param name="dataType">The resulting data type of the expression.</param>
        /// <param name="cond">Predicate of the conditional expresssion.</param>
        /// <param name="th">Consequent expression.</param>
        /// <param name="fa">Alternative expression.</param>
        public ConditionalExpression(
            DataType dataType,
            Expression cond,
            Expression th,
            Expression fa) : base(dataType)
        {
            this.Condition = cond;
            this.ThenExp = th;
            this.FalseExp = fa;
        }

        /// <summary>
        /// Predicate expression.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Consequent expression.
        /// </summary>
        public Expression ThenExp { get; }

        /// <summary>
        /// Alternate expression.
        /// </summary>
        public Expression FalseExp { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { return new[] { Condition, ThenExp, FalseExp }; }
        }

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitConditionalExpression(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitConditionalExpression(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConditionalExpression(this, context);
        }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            return new ConditionalExpression(
                this.DataType,
                this.Condition,
                this.ThenExp,
                this.FalseExp);
        }

        /// <inheritdoc />
        public override Expression Invert()
        {
            var t = this.ThenExp.Invert();
            var e = this.FalseExp.Invert();
            return new ConditionalExpression(
                this.DataType,
                this.Condition,
                t,
                e);
        }
    }
}
