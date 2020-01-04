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

using Reko.Core.Expressions;
using System;

namespace Reko.Core.Code
{
    public interface InstructionVisitor
	{
		void VisitAssignment(Assignment ass);
		void VisitBranch(Branch branch);
		void VisitCallInstruction(CallInstruction ci);
        void VisitComment(CodeComment code);

        void VisitDeclaration(Declaration decl);
		void VisitDefInstruction(DefInstruction def);
        void VisitGotoInstruction(GotoInstruction gotoInstruction);
		void VisitPhiAssignment(PhiAssignment phi);
		void VisitReturnInstruction(ReturnInstruction ret);
		void VisitSideEffect(SideEffect side);
		void VisitStore(Store store);
		void VisitSwitchInstruction(SwitchInstruction si);
		void VisitUseInstruction(UseInstruction use);
    }

    public interface InstructionVisitor<T>
    {
        T VisitAssignment(Assignment ass);
        T VisitBranch(Branch branch);
        T VisitComment(CodeComment comment);
        T VisitCallInstruction(CallInstruction ci);
        T VisitDeclaration(Declaration decl);
        T VisitDefInstruction(DefInstruction def);
        T VisitGotoInstruction(GotoInstruction gotoInstruction);
        T VisitPhiAssignment(PhiAssignment phi);
        T VisitReturnInstruction(ReturnInstruction ret);
        T VisitSideEffect(SideEffect side);
        T VisitStore(Store store);
        T VisitSwitchInstruction(SwitchInstruction si);
        T VisitUseInstruction(UseInstruction use);
    }

	/// <summary>
	/// Useful base classes when only a few of the methods of InstructionVisitor and IExpressionVisitor 
	/// are actually implemented.
	/// </summary>
	public class InstructionVisitorBase : InstructionVisitor, IExpressionVisitor
	{
		#region InstructionVisitor Members

		public virtual void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
			a.Dst.Accept(this);
		}

		public virtual void VisitBranch(Branch b)
		{
			b.Condition.Accept(this);
		}

		public virtual void VisitCallInstruction(CallInstruction ci)
		{
            ci.Callee.Accept(this);
		}

        public virtual void VisitComment(CodeComment comment)
        {
        }

		public virtual void VisitDeclaration(Declaration decl)
		{
			decl.Identifier.Accept(this);
			if (decl.Expression != null)
				decl.Expression.Accept(this);
		}

		public virtual void VisitDefInstruction(DefInstruction def)
		{
			def.Identifier.Accept(this);
		}

        public virtual void VisitGotoInstruction(GotoInstruction g)
        {
            g.Target.Accept(this);
        }

		public virtual void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
		}

		public virtual void VisitReturnInstruction(ReturnInstruction ret)
		{
			if (ret.Expression != null)
				ret.Expression.Accept(this);
		}

		public virtual void VisitSideEffect(SideEffect side)
		{
			side.Expression.Accept(this);
		}

		public virtual void VisitStore(Store store)
		{
			store.Src.Accept(this);
			store.Dst.Accept(this);
		}

		public virtual void VisitSwitchInstruction(SwitchInstruction si)
		{
			si.Expression.Accept(this);
		}

		public virtual void VisitUseInstruction(UseInstruction u)
		{
			u.Expression.Accept(this);
		}

		#endregion

		#region IExpressionVisitor Members

        public virtual void VisitAddress(Address addr)
        {
        }

        public virtual void VisitApplication(Application appl)
        {
            appl.Procedure.Accept(this);
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this);
            }
        }

		public virtual void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Index.Accept(this);
			acc.Array.Accept(this);
		}

		public virtual void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
		}

		public virtual void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

        public virtual void VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.Condition.Accept(this);
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
        }

        public virtual void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
		}

		public virtual void VisitConstant(Constant c)
		{
		}

		public virtual void VisitDepositBits(DepositBits d)
		{
			d.Source.Accept(this);
			d.InsertedBits.Accept(this);
		}

		public virtual void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
		}

		public virtual void VisitFieldAccess(FieldAccess acc)
		{
			acc.Structure.Accept(this);
		}

		public virtual void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
		}

		public virtual void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
			access.MemoryId.Accept(this);
		}

		public virtual void VisitMkSequence(MkSequence seq)
		{
            foreach (var e in seq.Expressions)
            {
                e.Accept(this);
            }
		}

		public virtual void VisitIdentifier(Identifier id)
		{
		}

        public virtual void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
        }

		public virtual void VisitPhiFunction(PhiFunction phi)
		{
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
			}
		}

		public virtual void VisitPointerAddition(PointerAddition pa)
		{
			pa.Pointer.Accept(this);
		}

		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
		}

		public virtual void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
		}

        public virtual void VisitSegmentedAccess(SegmentedAccess access)
        {
            access.MemoryId.Accept(this);
            access.BasePointer.Accept(this);
            access.EffectiveAddress.Accept(this);
        }

        public virtual void VisitScopeResolution(ScopeResolution scope)
		{
		}

		public virtual void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
		}

		public virtual void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
		}

		#endregion
	}
}
