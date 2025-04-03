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
    /// Represents a C-style "switch" statement in the abstract syntax tree.
    /// </summary>
    public class AbsynSwitch : AbsynStatement
    {
        /// <summary>
        /// Creates a switch statement.
        /// </summary>
        /// <param name="expr">The expression whose value is used to 
        /// select a case.</param>
        /// <param name="statements">The list of statements in the switch statement.</param>
        public AbsynSwitch(Expression expr, List<AbsynStatement> statements)
        {
            this.Expression = expr;
            this.Statements = statements;
        }

        /// <summary>
        /// The expression whose value is used to select a case.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// The list of statements in the switch statement.
        /// </summary>
        public List<AbsynStatement> Statements { get; }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitSwitch(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitSwitch(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSwitch(this, context);
        }
    }
}
