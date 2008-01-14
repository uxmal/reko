/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Core.Code
{
	public class UnaryExpression : Expression
	{
		public UnaryOperator op;
		public Expression Expression;

		public UnaryExpression(UnaryOperator op, DataType type, Expression expr) : base(type)
		{
			this.op = op; this.Expression = expr;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformUnaryExpression(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitUnaryExpression(this);
		}

		public override Expression CloneExpression()
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object o)
		{
			UnaryExpression u = o as UnaryExpression;
			if (u == null)
				return false;
			return op == u.op && Expression.Equals(u.Expression);
		}

		public override int GetHashCode()
		{
			return op.GetHashCode() ^ Expression.GetHashCode();
		}

		public override Expression Invert()
		{
			if (op == Operator.not)
				return Expression;
			else 
				return new UnaryExpression(Operator.not, PrimitiveType.Bool, this);
		}
	}
}
