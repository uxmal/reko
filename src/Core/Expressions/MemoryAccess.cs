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
using System;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models an access to memory, using the effective address <see cref="EffectiveAddress"/> and the datatype
    /// of the accessed memory.
    /// </summary>
    public class MemoryAccess : AbstractExpression
    {
        /// <summary>
        /// Creates an access to the memory whose effective address is <see cref="EffectiveAddress"/>.
        /// The data type of the accessed memory is <see cref="Expression.DataType"/>. The memory access
        /// takes place in the <see cref="MemoryStorage.GlobalMemory"/> address space.
        /// </summary>
        /// <param name="ea">Effective address of the access.</param>
        /// <param name="dt">Data type of the access.</param>
        public MemoryAccess(Expression ea, DataType dt)
            : base(dt)
        {
            this.MemoryId = MemoryStorage.GlobalMemory;
            this.EffectiveAddress = ea;
        }

        /// <summary>
        /// Creates an access to the memory whose effective address is <paramref name="effectiveAddress"/>.
        /// The data type of the accessed memory is <paramref name="dt"/>. The memory access
        /// takes place in the address space <paramref name="memoryId"/>.
        /// </summary>
        /// <param name="memoryId">Memory identifier.</param>
        /// <param name="effectiveAddress">Effective address of the access.</param>
        /// <param name="dt">Data type of the access.</param>
        public MemoryAccess(Identifier memoryId, Expression effectiveAddress, DataType dt)
            : base(dt)
        {
            this.MemoryId = memoryId;
            this.EffectiveAddress = effectiveAddress ?? throw new ArgumentNullException(nameof(effectiveAddress));
        }

        /// <summary>
        /// Identifier of the memory space being accessed.
        /// </summary>
        /// <remarks>
        /// For machines with von Neumann architecture, there only a single memory space accessed:
        /// <see cref="MemoryStorage.GlobalMemory"/>. Machines with Harvard architecture may define 
        /// multiple memory spaces.
        /// </remarks>
        public Identifier MemoryId { get; }

        /// <summary>
        /// The effective address of the memory access.
        /// </summary>
        public Expression EffectiveAddress { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return EffectiveAddress; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMemoryAccess(this, context);
        }

        /// <inheritdoc/>
        public override void Accept(IExpressionVisitor v)
        {
            v.VisitMemoryAccess(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMemoryAccess(this);
        }

        /// <inheritdoc/>
        public override Expression CloneExpression()
        {
            return new MemoryAccess(this.MemoryId, EffectiveAddress.CloneExpression(), DataType);
        }

        /// <summary>
        /// Creates a memory access whose effective address is <paramref name="baseExpr"/> plus the given
        /// <paramref name="offset"/> and whose data type is <paramref name="dt"/>.
        /// </summary>
        /// <param name="baseExpr">The base expression to use.</param>
        /// <param name="offset">Signed offset from the base expression.
        /// </param>
        /// <param name="dt">The data type of the access.</param>
        /// <returns>A <see cref="MemoryAccess"/> instance.</returns>
        public static MemoryAccess Create(Expression baseExpr, int offset, DataType dt)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, CreateEffectiveAddress(baseExpr, offset), dt);
        }

        /// <summary>
        /// Creates an effective address expression <paramref name="baseExpr"/> plus the given
        /// <paramref name="offset"/>.
        /// </summary>
        /// <param name="baseExpr">The base expression to use.</param>
        /// <param name="offset">Signed offset from the base expression.
        /// </param>
        /// <returns>A <see cref="MemoryAccess"/> instance.</returns>
        public static Expression CreateEffectiveAddress(Expression baseExpr, int offset)
        {
            if (offset == 0)
                return baseExpr;
            else
                return new BinaryExpression(
                    Operator.IAdd,
                    baseExpr.DataType,
                    baseExpr,
                    Constant.Create(PrimitiveType.Create(Domain.SignedInt, baseExpr.DataType.BitSize), offset));
        }

        /// <summary>
        /// Unpacks a <see cref="MemoryAccess"/> into its components. The displacement of a memory access
        /// is very frequently used in analyses.
        /// </summary>
        /// <returns>
        /// A 3-tuple of consisting of a base pointer (if the effective address is a <see cref="SegmentedPointer"/>),
        /// the effective address, stripped of any displacement, and the displacement.
        /// </returns>
        public (Expression? basePtr, Expression? address, long displacement) Unpack()
        {
            Expression? seg;
            Expression ea;
            if (this.EffectiveAddress is SegmentedPointer segptr)
            {
                seg = segptr.BasePointer;
                ea = segptr.Offset;
            }
            else
            {
                seg = null;
                ea = this.EffectiveAddress;
            }
            long offset = 0;
            Expression? eaStripped = ea;
            if (ea is Constant global)
            {
                offset = global.ToInt64();
                eaStripped = null;
            }
            else if (ea is Address addr)
            {
                offset = (long) addr.Offset;
                eaStripped = null;
                if (addr.Selector.HasValue)
                {
                    seg = Constant.Create(PrimitiveType.SegmentSelector, addr.Selector.Value);
                }
            }
            else if (ea is BinaryExpression bin)
            {
                if (bin.Right is Constant c)
                {
                    if (bin.Operator.Type == OperatorType.IAdd)
                    {
                        offset = c.ToInt64();
                        eaStripped = bin.Left;
                    }
                    else if (bin.Operator.Type == OperatorType.ISub)
                    {
                        offset = -c.ToInt64();
                        eaStripped = bin.Left;
                    }
                }
            }
            return (seg, eaStripped, offset);
        }
    }
}
