#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Output;
using System;
using System.IO;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Base class for all decompiled expressions.
    /// </summary>
    public abstract class AbstractExpression : Expression
    {
        /// <summary>
        /// Initializes an <see cref="AbstractExpression"/>.
        /// </summary>
        /// <param name="dataType">Data type of the expression.</param>
        public AbstractExpression(DataType dataType)
        {
            this.DataType = dataType;
        }

        /// <summary>
        /// Returns the direct children of this expression, in left-to-right
        /// order.
        /// </summary>
        public abstract IEnumerable<Expression> Children { get; }

        /// <inheritdoc />
        public DataType DataType { get; set; }

        /// <inheritdoc />
        public virtual bool IsZero => false;

        // Visitor methods that must be implemented by concrete derived classes.
        /// <inheritdoc />
        public abstract void Accept(IExpressionVisitor visitor);
        /// <inheritdoc />
        public abstract T Accept<T>(ExpressionVisitor<T> visitor);
        /// <inheritdoc />
        public abstract T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context);

        /// <inheritdoc />
        public abstract Expression CloneExpression();

        /// <summary>
        /// Applies logical (not-bitwise) negation to the expression.
        /// </summary>
        /// <returns>The logical inverse of this expression.</returns>
        public virtual Expression Invert()
        {
            throw new NotSupportedException(string.Format("Expression of type {0} doesn't support Invert.", GetType().Name));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sw = new StringWriter();
            var fmt = new CodeFormatter(new TextFormatter(sw));
            fmt.WriteExpression(this);
            return sw.ToString();
        }
    }
}
