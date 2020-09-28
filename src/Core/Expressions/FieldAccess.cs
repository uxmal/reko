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
    /// Models an access to the field of a record, like the Pascal expression
    /// a.field. The <paramref name="expr"/> is expected to be a structure
    /// type. If it is a pointer type, that is the expression should be 
    /// a^.field (as expressed in Pascal), remember to first "wrap" it in
    /// a <see cref="Dereference" /> instance.
    /// </summary>
	public class FieldAccess : Expression
	{
		public FieldAccess(DataType fieldType, Expression expr, Field field) : base(fieldType)
		{
			this.Structure = expr; this.Field = field;
		}

        public Expression Structure { get; private set; }
        public Field Field { get; private set; }

        public override IEnumerable<Expression> Children
        {
            get { yield return Structure; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitFieldAccess(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitFieldAccess(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitFieldAccess(this);
		}

		public override Expression CloneExpression()
		{
			return new FieldAccess(DataType, Structure.CloneExpression(), Field);
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}
	}
}
