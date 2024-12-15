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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Expressions
{
    public class StringConstant : AbstractExpression
    {
        private readonly string str;

        public StringConstant(DataType type, string str)
            : base(type)
        {
            this.str = str;
        }

        public string Literal => str;

        public int Length => str.Length;

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitStringConstant(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitStringConstant(this);
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitStringConstant(this, context);
        }

        public override Expression CloneExpression()
        {
            return this;
        }

        public override IEnumerable<Expression> Children
        {
            get { yield break; }
        }

        public override string ToString()
        {
            return str;
        }
    }
}
