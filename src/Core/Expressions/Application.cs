#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Expressions
{
	/// <summary>
	/// Represents an application of a procedure to a list of arguments.
	/// </summary>
	public class Application : AbstractExpression
	{
        /// <summary>
        /// Creates an instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="proc">Procedure to be invoked.</param>
        /// <param name="retVal">Data type of the returned value.</param>
        /// <param name="arguments">Arguments to the procedure call.</param>
		public Application(Expression proc, DataType retVal, params Expression [] arguments) : base(retVal)
		{
			this.Procedure = proc;
			this.Arguments = arguments;
		}

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { return new Expression[] { Procedure }.Concat(Arguments); }
        }

        /// <summary>
        /// The procedure being called. This is usually a <see cref="ProcedureConstant"/>
        /// but can also be any <see cref="Expression"/>.
        /// </summary>
        public Expression Procedure { get; set; }

        /// <summary>
        /// The arguments to the procedure call.
        /// </summary>
        public Expression[] Arguments { get; set; }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitApplication(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitApplication(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor v)
		{
			v.VisitApplication(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
			Expression [] p = new Expression[Arguments.Length];
			for (int i = 0; i < p.Length; ++i)
			{
				p[i] = Arguments[i].CloneExpression();
			}
			return new Application(this.Procedure, this.DataType, p);
		}

        /// <inheritdoc/>
		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}
	}
}
