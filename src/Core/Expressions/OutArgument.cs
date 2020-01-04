#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Marks an argument as being a 'defined' parameter, or out parameter. 
    /// The back end should convert these to the appropriate syntax for the
    /// output language.
    /// </summary>
    public class OutArgument : Expression
    {
        /// <summary>
        /// Builds an outparameter.
        /// </summary>
        /// <param name="refType">The type of the reference, ie. if the identifer is of type int, the reference type is (ptr int)</param>
        /// <param name="id"></param>
        public OutArgument(DataType refType, Expression id) : base(refType)
        {
            this.Expression = id;
        }

        public Expression Expression { get; private set; }

        public override IEnumerable<Expression> Children
        {
            get { yield return Expression; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitOutArgument(this, context);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitOutArgument(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitOutArgument(this);
        }

        public override Expression CloneExpression()
        {
            return new OutArgument(DataType, Expression.CloneExpression());
        }
    }
}
