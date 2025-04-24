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
    /// Abstract syntax for a case in a C-style switch statement.
    /// </summary>
    public class AbsynCase : AbsynStatement
    {
        /// <summary>
        /// Constructs a case statement.
        /// </summary>
        /// <param name="c">The constant for the case.</param>
        public AbsynCase(Constant c)
        {
            this.Constant = c;
        }

        /// <summary>
        /// The constant for the case.
        /// </summary>
        public Constant Constant { get; }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitCase(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitCase(this, context);
        }
    }

    /// <summary>
    /// Abstract syntax for a case in a C-style switch statement.
    /// </summary>
    public class AbsynDefault : AbsynStatement
    {
        /// <summary>
        /// Constructs a default case statement.
        /// </summary>
        public AbsynDefault()
        {
        }

        /// <inheritdoc />
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitDefault(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitDefault(this);
        }

        /// <inheritdoc />
        public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
        {
            return visitor.VisitDefault(this, context);
        }
    }
}
