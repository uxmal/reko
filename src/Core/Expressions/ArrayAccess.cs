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
    /// Models an array access.
    /// </summary>
	public class ArrayAccess : AbstractExpression
	{
        /// <summary>
        /// Creates an instance of the <see cref="ArrayAccess"/> class.
        /// </summary>
        /// <param name="elementType">The data type of the array's elements.</param>
        /// <param name="array">The array being accessed.</param>
        /// <param name="index">The index into the array.</param>
        /// <exception cref="ArgumentNullException"></exception>
		public ArrayAccess(DataType elementType, Expression array, Expression index) : base(elementType)
		{
            Array = array ?? throw new ArgumentNullException(nameof(array));
            Index = index ?? throw new ArgumentNullException(nameof(index));
		}

        /// <summary>
        /// The array being accessed.
        /// </summary>
        public Expression Array { get; }

        /// <summary>
        /// The index into the array.
        /// </summary>
        public Expression Index { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return Array;  yield return Index; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitArrayAccess(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitArrayAccess(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitArrayAccess(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
			return new ArrayAccess(DataType, Array.CloneExpression(), Index.CloneExpression());
		}
	}
}
