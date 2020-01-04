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
	/// Implements the C++ construct ptr.*mp
	/// </summary>
    /// <remarks>This construct is used to model 8086 real mode code, where "far pointers"
    /// consist of a segment and an offset. The offset corresponds to a member pointer selector.
    /// </remarks>
	public class MemberPointerSelector : Expression
	{
		public MemberPointerSelector(DataType dt, Expression basePtr, Expression memberPtr) : base(dt)
		{
			BasePointer = basePtr;
			MemberPointer = memberPtr;
		}

        public Expression BasePointer { get; set; }
        public Expression MemberPointer { get; set; }

        public override IEnumerable<Expression> Children
        {
            get { yield return BasePointer; yield return MemberPointer; }
        }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMemberPointerSelector(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMemberPointerSelector(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitMemberPointerSelector(this);
		}

		public override Expression CloneExpression()
		{
			return new MemberPointerSelector(DataType, BasePointer.CloneExpression(), MemberPointer.CloneExpression());
		}
	}
}
