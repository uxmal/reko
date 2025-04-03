#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Absyn
{
	/// <summary>
	/// Models a statement that isn't an assignment, typically a function that does IO.
	/// </summary>
	public class AbsynSideEffect : AbsynStatement
	{
        /// <summary>
        /// Creates a new side effect statement.
        /// </summary>
        /// <param name="expr">Expression which is only evaluated for its side effect.</param>
		public AbsynSideEffect(Expression expr)
		{
            Expression = expr;
		}

        /// <summary>
        /// Expression which is only evaluated for its side effect.
        /// </summary>
		public Expression Expression { get; }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitSideEffect(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitSideEffect(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSideEffect(this, context);
        }
    }
}
