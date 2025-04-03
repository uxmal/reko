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

using Reko.Core.Code;
using Reko.Core.Expressions;
using System;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Abstract syntax node for a declaration statement.
    /// </summary>
	public class AbsynDeclaration : AbsynStatement
	{
        /// <summary>
        /// Constructs a declaration statement, with an optional 
        /// initialization expression.
        /// </summary>
        /// <param name="id">Identifier to declare.</param>
        /// <param name="expr">Optional initialization expression.</param>
		public AbsynDeclaration(Identifier id, Expression? expr)
		{
			this.Identifier = id;
			this.Expression = expr;
		}

        /// <summary>
        /// The identifier being declared.
        /// </summary>
        public Identifier Identifier { get; }

        /// <summary>
        /// Optional initialization expression.
        /// </summary>
        public Expression? Expression { get; set; }

        /// <inheritdoc/>
        public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitDeclaration(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitDeclaration(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitDeclaration(this, context);
        }
    }
}
