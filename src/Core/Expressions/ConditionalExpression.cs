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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models the C ternary ?: operator.
    /// </summary>
    public class ConditionalExpression : Expression
    {
        public ConditionalExpression(
            DataType dataType,
            Expression cond,
            Expression th,
            Expression fa) : base(dataType)
        {
            this.Condition = cond;
            this.ThenExp = th;
            this.FalseExp = fa;
        }

        public Expression Condition { get; private set; }
        public Expression ThenExp { get; private set; }
        public Expression FalseExp { get; private set; }

        public override IEnumerable<Expression> Children
        {
            get { return new[] { Condition, ThenExp, FalseExp }; }
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitConditionalExpression(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitConditionalExpression(this);
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitConditionalExpression(this, context);
        }

        public override Expression CloneExpression()
        {
            return new ConditionalExpression(
                this.DataType,
                this.Condition,
                this.ThenExp,
                this.FalseExp);
        }
    }
}
