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

namespace Reko.Core.Absyn;

/// <summary>
/// Adapter that holds an Instruction.
/// </summary>
public class AbsynAssignment : AbsynStatement
{
    /// <summary>
    /// Creates an instance of an abstract syntax node that represents an assignment
    /// </summary>
    /// <param name="dst">Destination of the assignment.</param>
    /// <param name="src">Source of the assignment.</param>
    public AbsynAssignment(Expression dst, Expression src)
    {
        this.Dst = dst;
        this.Src = src;
    }

    /// <summary>
    /// Destination of the assignment.
    /// </summary>
    public Expression Dst { get; }

    /// <summary>
    /// Source of the assignment.
    /// </summary>
    public Expression Src { get; }

    /// <inheritdoc/>
        /// <inheritdoc/>
    public override void Accept(IAbsynVisitor visitor)
    {
        visitor.VisitAssignment(this);
    }

    /// <inheritdoc/>
    public override T Accept<T>(IAbsynVisitor<T> visitor)
    {
        return visitor.VisitAssignment(this);
    }

    /// <inheritdoc/>
    public override T Accept<T, C>(IAbsynVisitor<T, C> visitor, C context)
    {
        return visitor.VisitAssignment(this, context);
    }
}
