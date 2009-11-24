/* 
 * Copyright (C) 1999-2009 John Källén.
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
	public class ArrayAccess : Expression
	{
		public Expression Array;
		public Expression Index;

		public ArrayAccess(DataType elementType, Expression array, Expression index) : base(elementType)
		{
			this.Array = array; this.Index = index;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformArrayAccess(this);
		}

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitArrayAccess(this);
		}

		public override Expression CloneExpression()
		{
			return new ArrayAccess(DataType, Array.CloneExpression(), Index.CloneExpression());
		}

	}
}
