#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
    /// Represents the addition of a constant value to a pointer.
    /// </summary>
	public class PointerAddition : AbstractExpression
	{
        /// <summary>
        /// Constructs an instance of the <see cref="PointerAddition"/> class.
        /// </summary>
        /// <param name="addType">Data type of the sum.</param>
        /// <param name="expr">Addend, could be anything.</param>
        /// <param name="offset">Augend.</param>
		public PointerAddition(DataType addType, Expression expr, int offset) : base(addType)
		{
			Pointer = expr; this.Offset = offset;
		}

        /// <summary>
        /// The pointer to which the offset is added.
        /// </summary>
        public Expression Pointer { get; }

        /// <summary>
        /// Added offset.
        /// </summary>
        public int Offset { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return Pointer; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitPointerAddition(this, context);
        }
        
        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitPointerAddition(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitPointerAddition(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
            return new PointerAddition(DataType, Pointer, Offset);
		}
	}
}
