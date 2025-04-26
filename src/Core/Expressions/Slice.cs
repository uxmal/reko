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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// A slice is an improper subsequence of bits. Commonly used to isolate
	/// a byte register from a wider word- or dword-width register.
	/// </summary>
	public class Slice : AbstractExpression
	{
        /// <summary>
        /// Constructs a slice expression.
        /// </summary>
        /// <param name="dt">Size of the slice.</param>
        /// <param name="expression">The sliced expression.</param>
        /// <param name="bitOffset">The offset at which the slice is taken.</param>
        public Slice(DataType dt, Expression expression, int bitOffset) : base(dt)
        {
            if (bitOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(bitOffset), "Offset must be non-negative.");
            //$TODO:
            // Turning the following code on causes stricter validation
            // of the arguments. However in the TrashedRegisterFinder, 
            // too-short values are being stored with SetState. That cannot 
            // be fixed until a revamp of Ssa stack variables takes place.
            // We currently commit too quickly to a stack variable before 
            // we give the SSA stage a chance to determin whether two 
            // adjacent stack stores are in fact a larger segmented load.
#if STRICT
            if (bitOffset + dt.BitSize > expression.DataType.BitSize)
                throw new ArgumentException(nameof(expression), 
                    $"Sliced expression is too small.{bitOffset} + {dt.BitSize} > {expression.DataType.BitSize} ");
#endif
            Expression = expression;
            Offset = bitOffset;
        }

        /// <summary>
        /// The expression being sliced.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Bit offset of the slice.
        /// </summary>
        public int Offset { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }
       
        /// <inheritdoc/>
        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitSlice(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitSlice(this);
        }

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor v)
		{
			v.VisitSlice(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
			return new Slice(DataType, Expression, Offset);
		}

        /// <inheritdoc/>
        public override Expression Invert()
        {
            return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
        }
    }
}
