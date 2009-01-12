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

using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Represents an application of a procedure to a list of arguments.
	/// </summary>
	public class Application : Expression
	{
		public Expression Procedure;
		public Expression [] Arguments;

		public Application(Expression proc, DataType retVal, params Expression [] arguments) : base(retVal)
		{
			this.Procedure = proc;
			this.Arguments = arguments;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformApplication(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitApplication(this);
		}

		public override Expression CloneExpression()
		{
			Expression [] p = new Expression[Arguments.Length];
			for (int i = 0; i < p.Length; ++i)
			{
				p[i] = Arguments[i].CloneExpression();
			}
			return new Application(this.Procedure, this.DataType, p);
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.not, PrimitiveType.Bool, this);
		}
	}
}
