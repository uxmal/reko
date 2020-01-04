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

namespace Reko.Core.Expressions
{
	/// <summary>
	/// Using the 'Visitor' pattern, any class interested in examining 
	/// different Expression classes without having to probe their class
	/// type explicity (with casts or 'as' and 'is').
	/// </summary>
	public interface IExpressionVisitor
	{
        void VisitAddress(Address addr);
		void VisitApplication(Application appl);
		void VisitArrayAccess(ArrayAccess acc);
		void VisitBinaryExpression(BinaryExpression binExp);
		void VisitCast(Cast cast);
        void VisitConditionalExpression(ConditionalExpression cond);
        void VisitConditionOf(ConditionOf cof);
		void VisitConstant(Constant c);
		void VisitDepositBits(DepositBits d);
		void VisitDereference(Dereference deref);
		void VisitFieldAccess(FieldAccess acc);
		void VisitIdentifier(Identifier id);
		void VisitMemberPointerSelector(MemberPointerSelector mps);
		void VisitMemoryAccess(MemoryAccess access);
        void VisitMkSequence(MkSequence seq);
        void VisitOutArgument(OutArgument outArgument);
        void VisitPhiFunction(PhiFunction phi);
		void VisitPointerAddition(PointerAddition pa);
		void VisitProcedureConstant(ProcedureConstant pc);
		void VisitScopeResolution(ScopeResolution scopeResolution);
		void VisitSegmentedAccess(SegmentedAccess access);

        void VisitSlice(Slice slice);
		void VisitTestCondition(TestCondition tc);
		void VisitUnaryExpression(UnaryExpression unary);
    }

    public interface ExpressionVisitor<T>
    {
        T VisitAddress(Address addr);
        T VisitApplication(Application appl);
        T VisitArrayAccess(ArrayAccess acc);
        T VisitBinaryExpression(BinaryExpression binExp);
        T VisitCast(Cast cast);
        T VisitConditionalExpression(ConditionalExpression cond);
        T VisitConditionOf(ConditionOf cof);
        T VisitConstant(Constant c);
        T VisitDepositBits(DepositBits d);
        T VisitDereference(Dereference deref);
        T VisitFieldAccess(FieldAccess acc);
        T VisitIdentifier(Identifier id);
        T VisitMemberPointerSelector(MemberPointerSelector mps);
        T VisitMemoryAccess(MemoryAccess access);
        T VisitMkSequence(MkSequence seq);
        T VisitOutArgument(OutArgument outArgument);
        T VisitPhiFunction(PhiFunction phi);
        T VisitPointerAddition(PointerAddition pa);
        T VisitProcedureConstant(ProcedureConstant pc);
        T VisitScopeResolution(ScopeResolution scopeResolution);
        T VisitSegmentedAccess(SegmentedAccess access);
        T VisitSlice(Slice slice);
        T VisitTestCondition(TestCondition tc);
        T VisitUnaryExpression(UnaryExpression unary);
    }

    public interface ExpressionVisitor<T, C>
    {
        T VisitAddress(Address addr, C ctx);
        T VisitApplication(Application appl, C ctx);
        T VisitArrayAccess(ArrayAccess acc, C ctx);
        T VisitBinaryExpression(BinaryExpression binExp, C ctx);
        T VisitCast(Cast cast, C ctx);
        T VisitConditionalExpression(ConditionalExpression c, C context);
        T VisitConditionOf(ConditionOf cof, C ctx);
        T VisitConstant(Constant c, C ctx);
        T VisitDepositBits(DepositBits d, C ctx);
        T VisitDereference(Dereference deref, C ctx);
        T VisitFieldAccess(FieldAccess acc, C ctx);
        T VisitIdentifier(Identifier id, C ctx);
        T VisitMemberPointerSelector(MemberPointerSelector mps, C ctx);
        T VisitMemoryAccess(MemoryAccess access, C ctx);
        T VisitMkSequence(MkSequence seq, C ctx);
        T VisitOutArgument(OutArgument outArgument, C ctx);
        T VisitPhiFunction(PhiFunction phi, C ctx);
        T VisitPointerAddition(PointerAddition pa, C ctx);
        T VisitProcedureConstant(ProcedureConstant pc, C ctx);
        T VisitScopeResolution(ScopeResolution scopeResolution, C ctx);
        T VisitSegmentedAccess(SegmentedAccess access, C ctx);
        T VisitSlice(Slice slice, C ctx);
        T VisitTestCondition(TestCondition tc, C ctx);
        T VisitUnaryExpression(UnaryExpression unary, C ctx);
    }

	public class ExpressionVisitorBase : IExpressionVisitor
	{
		#region IExpressionVisitor Members

        public virtual void VisitAddress(Address addr)
        {
        }

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

        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.Condition.Accept(this);
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
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
			acc.Structure.Accept(this);
		}

		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
		}

		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
            access.MemoryId.Accept(this);
        }

		public void VisitMkSequence(MkSequence seq)
		{
            for (int i = 0; i < seq.Expressions.Length; ++i)
            {
                seq.Expressions[i].Accept(this);
            }
        }

		public virtual void VisitIdentifier(Identifier id)
		{
		}

        public virtual void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
        }

		public void VisitPhiFunction(PhiFunction phi)
		{
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
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

    public class ExpressionVisitorBase<T> : ExpressionVisitor<T>
    {
        public virtual T VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitApplication(Application appl)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitBinaryExpression(BinaryExpression binExp)
        {
            binExp.Left.Accept(this);
            binExp.Right.Accept(this);
            return default(T);
        }

        public virtual T VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitConditionOf(ConditionOf cof)
        {
            return cof.Expression.Accept(this);
        }

        public virtual T VisitConstant(Constant c)
        {
            return default(T);
        }

        public virtual T VisitDepositBits(DepositBits d)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitIdentifier(Identifier id)
        {
            return default(T);
        }

        public virtual T VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitMemoryAccess(MemoryAccess access)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitOutArgument(OutArgument outArg)
        {
            return outArg.Expression.Accept(this);
        }

        public virtual T VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitSegmentedAccess(SegmentedAccess access)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitTestCondition(TestCondition tc)
        {
            return tc.Expression.Accept(this);
        }

        public virtual T VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }
    }
}
