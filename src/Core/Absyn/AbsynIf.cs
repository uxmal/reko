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
using System.Collections.Generic;

namespace Reko.Core.Absyn
{
	/// <summary>
	/// Abstract syntax for a traditional "if" statement.
	/// </summary>
	public class AbsynIf : AbsynStatement
	{
        /// <summary>
        /// Constructs an "if-then" statement.
        /// </summary>
        /// <param name="e">The condition of the "if" statement.</param>
        /// <param name="then">The list of statements to execute if the condition 
        /// evaluated to non-zero.</param>
        public AbsynIf(Expression e, List<AbsynStatement> then) : this(e, then, [])
        {
        }

        /// <summary>
        /// Constructs an "if-then-else" statement.
        /// </summary>
        /// <param name="e">The condition of the "if" statement.</param>
        /// <param name="then">The list of statements to execute if the condition 
        /// evaluated to non-zero.</param>
        /// <param name="els">The list of statements to execute if the condition 
        /// evaluated to zero.</param>
        public AbsynIf(Expression e, List<AbsynStatement> then, List<AbsynStatement> els)
        {
            this.Condition = e;
            this.Then = then;
            this.Else = els;
        }

        /// <summary>
        /// The condition of the "if" statement.
        /// </summary>
        public Expression Condition { get; set; }

        /// <summary>
        /// The list of statements to execute if the condition evaluated to non-zero.
        /// </summary>
        public List<AbsynStatement> Then { get; private set; }

        /// <summary>
        /// The list of statements to execute if the condition evaluated to zero.
        /// </summary>
        public List<AbsynStatement> Else { get; private set; }

        /// <inheritdoc />
		public override void Accept(IAbsynVisitor v)
		{
			v.VisitIf(this);
		}

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitIf(this); 
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitIf(this, context);
        }

        /// <summary>
        /// Inverts the condition of this "if" statement, by logicall negating
        /// the condition and swapping the "then" and "else" statements.
        /// </summary>
        public void InvertCondition()
        {
            (Else, Then) = (Then, Else);
            Condition = Condition.Invert();
        }
    }
}
