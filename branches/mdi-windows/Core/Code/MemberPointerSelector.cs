/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Implements the C++ construct ptr.*mp
	/// </summary>
	public class MemberPointerSelector : Expression
	{
		public Expression BasePointer;
		public Expression MemberPointer;

		public MemberPointerSelector(DataType dt, Expression basePtr, Expression memberPtr) : base(dt)
		{
			BasePointer = basePtr;
			MemberPointer = memberPtr;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformMemberPointerSelector(this);
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
