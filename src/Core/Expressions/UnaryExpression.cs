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
using System.IO;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models a unary expression like logical negation or bit complement.
    /// </summary>
	public class UnaryExpression : Expression
	{
		public UnaryExpression(UnaryOperator op, DataType type, Expression expr) : base(type)
		{
			this.Operator = op; this.Expression = expr;
		}

        public UnaryOperator Operator { get; set; }
        public Expression Expression { get; set; }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitUnaryExpression(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitUnaryExpression(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitUnaryExpression(this);
		}

		public override Expression CloneExpression()
		{
            return new UnaryExpression(Operator, DataType.Clone(), Expression.CloneExpression());
		}

		public override Expression Invert()
		{
			if (Operator == Operators.Operator.Not)
				return Expression;
			else
                return new UnaryExpression(Operators.Operator.Not, PrimitiveType.Bool, this);
		}
	}
}
