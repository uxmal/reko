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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Code
{
	public class ConditionOf : Expression
	{
		private Expression expr;

		public ConditionOf(Expression ex) : base(PrimitiveType.Byte)
		{
			expr = ex;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformConditionOf(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitConditionOf(this);
		}

		public override Expression CloneExpression()
		{
			return new ConditionOf(expr.CloneExpression());
		}

		public override bool Equals(object o)
		{
			ConditionOf co = o as ConditionOf;
			if (co == null)
				return false;
			return expr.Equals(co.expr);
		}

		public Expression Expression
		{
			get { return expr; }
			set { expr = value; }
		}

		public override int GetHashCode()
		{
			return 0x10101010 ^ expr.GetHashCode();
		}
	}
}
