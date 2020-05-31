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

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models an access to memory, using the effective address <paramref name="ea"/> and the datatype
    /// of the accessed memory.
    /// </summary>
    public class MemoryAccess : Expression
    {
        /// <summary>
        /// Creates an access to the memory whose effective address is <paramref name="ea"/>.
        /// The data type of the accessed memory is <paramref name="dt"/>. The memory access
        /// takes place in the <see cref="MemoryIdentifier.GlobalMemory"/> address space.
        /// </summary>
        /// <param name="ea">Effective address of the access.</param>
        /// <param name="dt">Data type of the access.</param>
        public MemoryAccess(Expression ea, DataType dt)
            : base(dt)
        {
            this.MemoryId = MemoryIdentifier.GlobalMemory;
            this.EffectiveAddress = ea;
        }

        /// <summary>
        /// Creates an access to the memory whose effective address is <paramref name="ea"/>.
        /// The data type of the accessed memory is <paramref name="dt"/>. The memory access
        /// takes place in the address space <paramref name="space"/>.
        /// </summary>
        /// <param name="ea">Effective address of the access.</param>
        /// <param name="dt">Data type of the access.</param>
        public MemoryAccess(MemoryIdentifier space, Expression ea, DataType dt)
            : base(dt)
        {
            this.MemoryId = space;
            this.EffectiveAddress = ea;
        }

        public readonly MemoryIdentifier MemoryId;
        public readonly Expression EffectiveAddress;

        public override IEnumerable<Expression> Children
        {
            get { yield return EffectiveAddress; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMemoryAccess(this, context);
        }

        public override void Accept(IExpressionVisitor v)
        {
            v.VisitMemoryAccess(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMemoryAccess(this);
        }

        public override Expression CloneExpression()
        {
            return new MemoryAccess(this.MemoryId, EffectiveAddress.CloneExpression(), DataType);
        }

        public static MemoryAccess Create(Expression baseRegister, int offset, DataType dt)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, CreateEffectiveAddress(baseRegister, offset), dt);
        }

        protected static Expression CreateEffectiveAddress(Expression baseRegister, int offset)
        {
            if (offset == 0)
                return baseRegister;
            else
                return new BinaryExpression(Operators.Operator.IAdd,
                    baseRegister.DataType,
                    baseRegister,
                    Constant.Create(PrimitiveType.Create(Domain.SignedInt, baseRegister.DataType.BitSize), offset));
        }
    }

    /// <summary>
    /// Segmented memory access that models x86 segmented memory addressing.
    /// </summary>
    public class SegmentedAccess : MemoryAccess
    {
        public SegmentedAccess(MemoryIdentifier id, Expression basePtr, Expression ea, DataType dt) : base(id, ea, dt)
        {
            this.BasePointer = basePtr;
        }

        public readonly Expression BasePointer;         // Segment selector

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitSegmentedAccess(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> visit)
        {
            return visit.VisitSegmentedAccess(this);
        }

        public override void Accept(IExpressionVisitor visit)
        {
            visit.VisitSegmentedAccess(this);
        }

        public override Expression CloneExpression()
        {
            return new SegmentedAccess(MemoryId, BasePointer.CloneExpression(), EffectiveAddress.CloneExpression(), DataType);
        }

        public static SegmentedAccess Create(Expression segRegister, Expression baseRegister, int offset, DataType dt)
        {
            return new SegmentedAccess(MemoryIdentifier.GlobalMemory, segRegister, CreateEffectiveAddress(baseRegister, offset), dt);
        }
    }

}
