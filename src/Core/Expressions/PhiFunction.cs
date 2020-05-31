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
	/// Representation of an SSA phi-function.
	/// </summary>
	public class PhiFunction : Expression
	{
		public PhiFunction(DataType joinType, params PhiArgument[] arguments) : base(joinType)
		{
			this.Arguments = arguments;
		}

        public PhiArgument[] Arguments { get; private set; }

        public override IEnumerable<Expression> Children
        {
            get { return Arguments.Select(de => de.Value); }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitPhiFunction(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitPhiFunction(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitPhiFunction(this);
		}

        public override Expression CloneExpression()
        {
            var args = Arguments
                .Select(a => new PhiArgument(a.Block, a.Value.CloneExpression()))
                .ToArray();
            return new PhiFunction(DataType, args);
        }
	}

    /// <summary>
    /// The phi function arguments need to keep track of both the SSA identifier 
    /// and the predecessor block from which the identifier came.
    /// </summary>
    public struct PhiArgument
    {
        public readonly Expression Value;
        public readonly Block Block;

        public PhiArgument(Block block, Expression value)
        {
            this.Value = value;
            this.Block = block;
        }
    }
}