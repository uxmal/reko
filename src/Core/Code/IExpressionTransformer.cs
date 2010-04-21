/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Using the 'Visitor' pattern, any class interested in modifying 
	/// different Expression classes without having to probe their class
	/// type explicity (with casts or 'as' and 'is').
	/// </summary>
	public interface IExpressionTransformer
	{
		Expression TransformApplication(Application appl);
		Expression TransformArrayAccess(ArrayAccess acc);
		Expression TransformBinaryExpression(BinaryExpression binExp);
		Expression TransformCast(Cast cast);
		Expression TransformConditionOf(ConditionOf cof);
		Expression TransformConstant(Constant c);
		Expression TransformDepositBits(DepositBits d);
		Expression TransformDereference(Dereference deref);
		Expression TransformFieldAccess(FieldAccess acc);
		Expression TransformMemberPointerSelector(MemberPointerSelector mps);
		Expression TransformMemoryAccess(MemoryAccess access);
		Expression TransformMkSequence(MkSequence seq);
		Expression TransformIdentifier(Identifier id);
		Expression TransformPhiFunction(PhiFunction phi);
		Expression TransformPointerAddition(PointerAddition pa);
		Expression TransformProcedureConstant(ProcedureConstant pc);
		Expression TransformSegmentedAccess(SegmentedAccess access);
		Expression TransformSlice(Slice slice);
		Expression TransformTestCondition(TestCondition tc);
		Expression TransformUnaryExpression(UnaryExpression unary);

		Expression TransformScopeResolution(ScopeResolution scopeResolution);
	}
}
