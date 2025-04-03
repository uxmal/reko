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

using System;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Represents a "goto" statement in the abstract syntax tree.
    /// </summary>
	public class AbsynGoto : AbsynStatement
	{
        /// <summary>
        /// Constructs a goto statement.
        /// </summary>
        /// <param name="label">Target of the goto statement.</param>
		public AbsynGoto(string label)
		{
			this.Label = label;
		}

        /// <summary>
        /// The target of the goto statement.
        /// </summary>
		public string Label { get; }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitGoto(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitGoto(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitGoto(this, context);
        }
    }
}
