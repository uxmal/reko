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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// A slice is an improper subsequence of bits. Commonly used to isolate
	/// a byte register from a wider word- or dword-width register
	/// </summary>
	public class Slice : Expression
	{
        public Slice(DataType dt, Expression i, int bitOffset) : base(dt)
        {
            if (bitOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(bitOffset), "Offset must be non-negative.");
            Expression = i;
            Offset = bitOffset;
        }

        /// <summary>
        /// The expression being sliced.
        /// </summary>
        public readonly Expression Expression;

        /// <summary>
        /// Bit offset of the slice.
        /// </summary>
        public int Offset { get; }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitSlice(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitSlice(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitSlice(this);
		}

		public override Expression CloneExpression()
		{
			return new Slice(DataType, Expression, Offset);
		}

        public override Expression Invert()
        {
            return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
        }
    }
}
