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

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models x86-style segmented pointers.
    /// </summary>
    public class SegmentedPointer : Expression
    {
        public SegmentedPointer(DataType dt, Expression basePointer, Expression offset)
            : base(dt)
        {
            this.BasePointer = basePointer;
            this.Offset = offset;
        }

        public static SegmentedPointer Create(Expression basePointer, Expression offset)
        {
            var bitsize = basePointer.DataType.BitSize + offset.DataType.BitSize;
            var dt = PrimitiveType.Create(Domain.SegPointer, bitsize);
            return new SegmentedPointer(dt, basePointer, offset);
        }

        public Expression BasePointer { get; }

        public Expression Offset { get; }

        public override IEnumerable<Expression> Children => throw new NotImplementedException();

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.VisitSegmentedAddress(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> visitor)
        {
            return visitor.VisitSegmentedAddress(this);
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSegmentedAddress(this, context);
        }

        public override Expression CloneExpression()
        {
            return new SegmentedPointer(this.DataType, this.BasePointer, this.Offset);
        }
    }
}
