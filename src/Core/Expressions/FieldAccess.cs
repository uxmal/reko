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

using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models an access to the field of a record, like the Pascal expression
    /// a.field. The <see cref="FieldAccess.Structure"/> expression is expected to be a structure
    /// type. If it is a pointer type, that is the expression should be 
    /// <c>a^.field</c> (as expressed in Pascal), remember to first "wrap" it in
    /// a <see cref="Dereference" /> instance.
    /// </summary>
	public class FieldAccess : AbstractExpression
	{
        /// <summary>
        /// Creates an instance of the <see cref="FieldAccess"/> class.
        /// </summary>
        /// <param name="fieldType">The data type of the value fetched from the field</param>
        /// <param name="expr">An expression of a structure type that includes the given <see cref="Field"/>.</param>
        /// <param name="field">The <see cref="Field"/> to access.</param>
		public FieldAccess(DataType fieldType, Expression expr, Field field) : base(fieldType)
		{
			this.Structure = expr; this.Field = field;
		}

        /// <summary>
        /// An expression of the structure type that the field is part of.
        /// </summary>
        public Expression Structure { get; }
        
        /// <summary>
        /// The field to access.
        /// </summary>
        public Field Field { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return Structure; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitFieldAccess(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitFieldAccess(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitFieldAccess(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
			return new FieldAccess(DataType, Structure.CloneExpression(), Field);
		}

        /// <inheritdoc/>
		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}
	}
}
