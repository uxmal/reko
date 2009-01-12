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

using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Using the 'Visitor' pattern, any class interested in examining 
	/// different Expression classes without having to probe their class
	/// type explicity (with casts or 'as' and 'is').
	/// </summary>
	public interface IExpressionVisitor
	{
		void VisitApplication(Application appl);
		void VisitArrayAccess(ArrayAccess acc);
		void VisitBinaryExpression(BinaryExpression binExp);
		void VisitCast(Cast cast);
		void VisitConditionOf(ConditionOf cof);
		void VisitConstant(Constant c);
		void VisitDepositBits(DepositBits d);
		void VisitDereference(Dereference deref);
		void VisitFieldAccess(FieldAccess acc);
		void VisitIdentifier(Identifier id);
		void VisitMemberPointerSelector(MemberPointerSelector mps);
		void VisitMemoryAccess(MemoryAccess access);
		void VisitMkSequence(MkSequence seq);
		void VisitPhiFunction(PhiFunction phi);
		void VisitPointerAddition(PointerAddition pa);
		void VisitProcedureConstant(ProcedureConstant pc);
		void VisitScopeResolution(ScopeResolution scopeResolution);
		void VisitSegmentedAccess(SegmentedAccess access);
		void VisitSlice(Slice slice);
		void VisitTestCondition(TestCondition tc);
		void VisitUnaryExpression(UnaryExpression unary);

	}

	public class ExpressionVisitorBase : IExpressionVisitor
	{
		#region IExpressionVisitor Members

		public void VisitApplication(Application appl)
		{
			appl.Procedure.Accept(this);
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
			}
		}

		public void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
		}

		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
		}

		public void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

		public void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
		}

		public void VisitConstant(Constant c)
		{
		}

		public void VisitDepositBits(DepositBits d)
		{
			d.Source.Accept(this);
			d.InsertedBits.Accept(this);
		}

		public void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
		}

		public void VisitFieldAccess(FieldAccess acc)
		{
			acc.structure.Accept(this);
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
		}

		public void VisitMkSequence(MkSequence seq)
		{
			seq.Head.Accept(this);
			seq.Tail.Accept(this);
		}

		public virtual void VisitIdentifier(Identifier id)
		{
		}

		public void VisitPhiFunction(PhiFunction phi)
		{
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				phi.Arguments[i].Accept(this);
			}
		}

		public void VisitPointerAddition(PointerAddition pa)
		{
			pa.Pointer.Accept(this);
		}

		public void VisitProcedureConstant(ProcedureConstant pc)
		{
		}

		public void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
		}

		public void VisitSegmentedAccess(SegmentedAccess access)
		{
			access.MemoryId.Accept(this);
			access.BasePointer.Accept(this);
			access.EffectiveAddress.Accept(this);
		}

		public void VisitScopeResolution(ScopeResolution scope)
		{
		}

		public void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
		}

		public void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
		}

		#endregion
	}
}
