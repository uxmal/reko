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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models an ordered sequence consisting of multiple expressions.
    /// The expressions are arranged in big-endian order, meaning that the first expression
    /// in the sequence is the most significant one.
    /// </summary>
    /// <remarks>The elements of the sequence form a whole. 
    /// The <see cref="DataType"/> indicates what kind of whole it is.
    /// </remarks>
    public class MkSequence : AbstractExpression
	{
        /// <summary>
        /// Constructs a expression sequence.
        /// </summary>
        /// <param name="dt">The data type of the total sequence.</param>
        /// <param name="exprs">An array expressions forming part of the sequence,
        /// in big-endian order: the most significant expressions first.</param>
        public MkSequence(DataType dt, params Expression [] exprs) : base(dt)
        {
            if (exprs.Length < 1)
                throw new ArgumentException("A sequence must have a least one expression.");
            this.Expressions = exprs;
        }

        /// <summary>
        /// The expressions forming part of the sequence, arranged in big-endian order.
        /// The most significant expression is first.
        /// </summary>
        public Expression[] Expressions { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { return Expressions; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMkSequence(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMkSequence(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitMkSequence(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
            var clones = Expressions.Select(e => e.CloneExpression()).ToArray();
            return new MkSequence(DataType, clones);
		}
	}
}
