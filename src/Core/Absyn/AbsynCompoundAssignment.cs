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

using Reko.Core.Expressions;

namespace Reko.Core.Absyn
{
    /// <summary>
    /// Constructs a C-style compound assignment.
    /// </summary>
    /// <remarks>
    /// A compound assignment is an assignment that combines a binary operation with 
    /// an assignment. For example, the expression <c>x += y</c> is equivalent to
    /// <c>x = x + y</c>.
    /// </remarks>
    public class AbsynCompoundAssignment : AbsynAssignment
    {
        /// <summary>
        /// Constructs a compound assignment.
        /// </summary>
        /// <param name="dst">The destination of the compound assignment.</param>
        /// <param name="src">The source of the compound assignment.</param>
        public AbsynCompoundAssignment(Expression dst, BinaryExpression src) : base(dst, src)
        {
            this.Src = src;
        }

        /// <summary>
        /// The binary expression that represents the compound assignment.
        /// </summary>
        public new BinaryExpression Src { get; private set; }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public override void Accept(IAbsynVisitor visitor)
        {
            visitor.VisitCompoundAssignment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IAbsynVisitor<T> visitor)
        {
            return visitor.VisitCompoundAssignment(this);
        }
    }
}
