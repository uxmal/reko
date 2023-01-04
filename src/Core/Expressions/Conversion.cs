#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// This class represents how the <see cref="Expression"/> <paramref name="exp" /> is converted 
    /// from the original data type <paramref name="dtFrom" /> to the data type <paramref name="dtTo" />.
    /// </summary>
    /// <remarks>
    /// To properly model a data type conversion, we must specify the expression being converted, the 
    /// datatype that expression is interpreted to be, and the resulting datatype after the conversion.
    /// <para>
    /// This is a low-level expression. After type analysis is completed it will be replaced with
    /// <see cref="Cast"/> expressions.
    /// </para>
    /// </remarks>
    public class Conversion : Expression
    {
        /// <summary>
        /// Makes an instance of the <see cref="Conversion"/> class.
        /// </summary>
        /// <param name="exp">Expression to convert.</param>
        /// <param name="dtFrom">Data type converting from.</param>
        /// <param name="dtTo">Data type converting to.</param>
        public Conversion(Expression exp, DataType dtFrom, DataType dtTo)
            : base(dtTo)
        {
            if (dtFrom.BitSize != exp.DataType.BitSize)
                throw new ArgumentException($"Argument size mismatch between {dtFrom} and {exp.DataType}.");
            this.SourceDataType = dtFrom;
            this.Expression = exp;
        }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        /// <summary>
        /// The <see cref="Reko.Core.Expressions.Expression" /> being converted.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// This property describes how the <see cref="Expression"/> is being interpreted
        /// before the conversion is carried out.
        /// </summary>
        public DataType SourceDataType { get; }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitConversion(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitConversion(this);
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConversion(this, context);
        }

        public override Expression CloneExpression()
        {
            return new Conversion(this.Expression.CloneExpression(), this.SourceDataType, this.DataType);
        }
    }
}
