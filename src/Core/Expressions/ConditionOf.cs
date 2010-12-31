#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Expressions
{
	public class ConditionOf : Expression
	{
		public ConditionOf(Expression ex) : base(PrimitiveType.Byte)
		{
			Expression = ex;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformConditionOf(this);
		}

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitConditionOf(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitConditionOf(this);
		}

		public override Expression CloneExpression()
		{
			return new ConditionOf(Expression.CloneExpression());
		}

        public Expression Expression { get; set; }
	}
}
