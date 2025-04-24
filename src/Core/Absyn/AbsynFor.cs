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
using System.Collections.Generic;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Abstract syntax for a C-type "for" statement.
    /// </summary>
    public class AbsynFor : AbsynStatement
    {
        /// <summary>
        /// Constructs an instance of a "for" statement.
        /// </summary>
        /// <param name="init">Initialization statement.</param>
        /// <param name="condition">The condition that controls the loop.</param>
        /// <param name="iteration">Statement that updates any control variables.</param>
        /// <param name="body">The body of the loop.</param>
        /// <summary>
        /// Constructs a for-loop.
        /// </summary>
        /// <param name="init">The initializing statement of the for loop.</param>
        /// <param name="condition">The expression that controls the loop.</param>
        /// <param name="iteration">Statement that updates the loop variable.</param>
        /// <param name="body">The body of the for-loop.</param>
        public AbsynFor(AbsynAssignment init, Expression condition, AbsynAssignment iteration, List<AbsynStatement> body)
        {
            this.Initialization = init;
            this.Condition = condition;
            this.Iteration = iteration;
            this.Body = body;
        }

        /// <summary>
        /// The initialization statement.
        /// </summary>
        public AbsynAssignment Initialization { get; set; }

        /// <summary>
        /// The condition that controls the loop.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// The statement that updates any control variables.
        /// </summary>
        public AbsynAssignment Iteration { get; set; }

        /// <summary>
        /// The body of the loop.
        /// </summary>
        public List<AbsynStatement> Body { get; }


        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitFor(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitFor(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitFor(this, context);
        }
    }
}
