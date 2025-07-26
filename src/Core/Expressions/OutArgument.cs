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
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Marks an argument as being a 'defined' parameter, or out parameter. 
    /// The back end should convert these to the appropriate syntax for the
    /// output language.
    /// </summary>
    public class OutArgument : AbstractExpression
    {
        /// <summary>
        /// Builds an out argument expression.
        /// </summary>
        /// <param name="dt">The type of argument</param>
        /// <param name="id">An identifier for the argument.</param>
        public OutArgument(DataType dt, Expression id) : base(dt)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            this.Expression = id;
        }

        /// <summary>
        /// The out argument.
        /// </summary>
        public Expression Expression { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitOutArgument(this, context);
        }

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitOutArgument(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitOutArgument(this);
        }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            return new OutArgument(DataType, Expression.CloneExpression());
        }
    }
}
