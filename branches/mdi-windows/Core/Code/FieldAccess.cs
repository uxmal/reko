/* 
 * Copyright (C) 1999-2010 John Källén.
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

namespace Decompiler.Core.Code
{
	public class FieldAccess : Expression
	{
		public Expression structure;
		public string FieldName;

		public FieldAccess(DataType fieldType, Expression expr, string fieldName) : base(fieldType)
		{
			this.structure = expr; this.FieldName = fieldName;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformFieldAccess(this);
		}

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitFieldAccess(this);
		}

		public override Expression CloneExpression()
		{
			return new FieldAccess(DataType, structure.CloneExpression(), FieldName);
		}


		public override Expression Invert()
		{
			return new UnaryExpression(Operator.not, PrimitiveType.Bool, this);
		}

	}

}
