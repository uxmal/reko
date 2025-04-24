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
    /// Represents a string constant.
    /// </summary>
    public class StringConstant : AbstractExpression
    {
        private readonly string str;

        /// <summary>
        /// Creates an instance of the <see cref="StringConstant"/> class.
        /// </summary>
        /// <param name="type">The data type of the string.</param>
        /// <param name="str">The literal string value.</param>
        public StringConstant(DataType type, string str)
            : base(type)
        {
            this.str = str;
        }

        /// <summary>
        /// The value of the string literal.
        /// </summary>
        public string Literal => str;

        /// <summary>
        /// The length of the string.
        /// </summary>
        public int Length => str.Length;

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitStringConstant(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitStringConstant(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitStringConstant(this, context);
        }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            return this;
        }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children => [];

        /// <inheritdoc/>
        public override string ToString()
        {
            return str;
        }
    }
}
