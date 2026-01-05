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

using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// The common interface for all expression types.
    /// </summary>
    public interface Expression
    {
        /// <summary>
        /// Returns the direct children of this expression, in left-to-right
        /// order.
        /// </summary>
        IEnumerable<Expression> Children { get; }

        /// <summary>
        /// Data type of this expression.
        /// </summary>
        DataType DataType { get; set; }

        /// <summary>
        /// Returns true if the expression evaluates to a constant zero.
        /// </summary>
        bool IsZero { get; }

        /// <summary>
        /// Accepts a visitor to this expression.
        /// </summary>
        /// <param name="visitor">Typeless visitor.</param>
        void Accept(IExpressionVisitor visitor);

        /// <summary>
        /// Accepts a visitor to this expression.
        /// </summary>
        /// <param name="visitor">The visitor, all of whose methods
        /// return <typeparamref name="T"/> or derived types of <typeparamref name="T"/>.
        /// </param>
        T Accept<T>(ExpressionVisitor<T> visitor);

        /// <summary>
        /// Accepts a context-sensitive visitor to this expression.
        /// </summary>
        /// <param name="visitor">The visitor, all of whose methods
        /// accept a context of type <typeparamref name="C"/> and
        /// return <typeparamref name="T"/> or derived types of <typeparamref name="T"/>.
        /// </param>
        /// <param name="context">The context provided by the analysis.
        /// </param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context);
        
        /// <summary>
        /// Create a deep copy of this expression.
        /// </summary>
        /// <returns>A deep copy of this expression.</returns>
        Expression CloneExpression();

        /// <summary>
        /// Applies logical (not-bitwise) negation to the expression.
        /// </summary>
        /// <returns>
        /// A new expression that is the logical negation of the
        /// expression.
        /// </returns>
        Expression Invert();
    }
}
