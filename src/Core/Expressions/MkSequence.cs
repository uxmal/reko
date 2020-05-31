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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// Creates a sequence out of multiple expressions.
	/// </summary>
    /// <remarks>The elements of the sequence form a whole. The DataType indicates what kind of whole it is.
	public class MkSequence : Expression
	{
        public MkSequence(DataType dt, params Expression [] exprs) : base(dt)
        {
            if (exprs.Length < 1)
                throw new ArgumentException("A sequence must have a least one expression.");
            this.Expressions = exprs;
        }

        public Expression[] Expressions { get; }

        public override IEnumerable<Expression> Children
        {
            get { return Expressions; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMkSequence(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMkSequence(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitMkSequence(this);
		}

		public override Expression CloneExpression()
		{
            var clones = Expressions.Select(e => e.CloneExpression()).ToArray();
            return new MkSequence(DataType, clones);
		}
	}
}
