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
    /// Implements the C++ construct <c>ptr.*mp</c>
    /// </summary>
    /// <remarks>This construct is used to model 8086 real mode code, where "far pointers"
    /// consist of a segment and an offset. The offset corresponds to a member pointer selector.
    /// </remarks>
    public class MemberPointerSelector : AbstractExpression
	{
        /// <summary>
        /// Constructs a member pointer selector.
        /// </summary>
        /// <param name="dt">The data type of the expression.</param>
        /// <param name="basePtr">The base pointer or selector.</param>
        /// <param name="memberPtr">The member pointer.</param>
		public MemberPointerSelector(DataType dt, Expression basePtr, Expression memberPtr) : base(dt)
		{
			BasePointer = basePtr;
			MemberPointer = memberPtr;
		}

        /// <summary>
        /// The base pointer or selector.
        /// </summary>
        public Expression BasePointer { get; }

        /// <summary>
        /// The member pointer.
        /// </summary>
        public Expression MemberPointer { get; }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield return BasePointer; yield return MemberPointer; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMemberPointerSelector(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMemberPointerSelector(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitMemberPointerSelector(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
			return new MemberPointerSelector(DataType, BasePointer.CloneExpression(), MemberPointer.CloneExpression());
		}
	}
}
