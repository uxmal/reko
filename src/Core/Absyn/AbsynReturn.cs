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

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Represents a return statement in the abstract syntax tree.
    /// </summary>
	public class AbsynReturn : AbsynStatement
	{
        /// <summary>
        /// Constructs a return statement with an optional return value.
        /// </summary>
        /// <param name="retval">Optional expression to return.</param>
		public AbsynReturn(Expression? retval)
		{
			this.Value = retval;
		}

        /// <summary>
        /// An optional return value.
        /// </summary>
		public Expression? Value { get; }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitReturn(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitReturn(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitReturn(this, context);
        }
    }
}
