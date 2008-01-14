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
	/// <summary>
	/// Representation of an SSA phi-function.
	/// </summary>
	public class PhiFunction : Expression
	{
		public Expression [] Arguments;

		public PhiFunction(DataType joinType, Expression [] arguments) : base(joinType)
		{
			this.Arguments = arguments;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformPhiFunction(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitPhiFunction(this);
		}

		public override Expression CloneExpression()
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object o)
		{
			PhiFunction phi = o as PhiFunction;
			if (phi == null)
				return false;
			if (Arguments.Length != phi.Arguments.Length)
				return false;
			for (int i = 0; i != Arguments.Length; ++i)
			{
				Expression e1 = Arguments[i];
				Expression e2 = phi.Arguments[i];
				if (e1 == null)
				{
					if (e2 != null)
						return false;
				}
				else
				{
					if (e2 == null)
						return false;
					if (!e1.Equals(e2))
						return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int h = Arguments.Length.GetHashCode();
			foreach (Expression e in Arguments)
			{
				h *= 47;
				if (e != null)
					h ^= e.GetHashCode();
			}
			return h;
		}

	}
}