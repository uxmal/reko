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

using Reko.Core.Expressions;

namespace Reko.Core.Code
{
    /// <summary>
    /// Visitor interface for <see cref="Instruction"/>s.
    /// </summary>
    public interface InstructionVisitor
	{
        /// <summary>
        /// Called when visiting an <see cref="Assignment"/> instruction.
        /// </summary>
        /// <param name="ass">Visited assignment.
        /// </param>
		void VisitAssignment(Assignment ass);

        /// <summary>
        /// Called when visiting a <see cref="Branch"/> instruction.
        /// </summary>
        /// <param name="branch">Visited branch instruction.
        /// </param>
		void VisitBranch(Branch branch);

        /// <summary>
        /// Called when visiting a <see cref="CallInstruction"/> instruction.
        /// </summary>
        /// <param name="call">Visited call instruction.
        /// </param>
		void VisitCallInstruction(CallInstruction call);

        /// <summary>
        /// Called when visiting a <see cref="CodeComment"/> instruction.
        /// </summary>
        /// <param name="code">Visited code comment.
        /// </param>
        void VisitComment(CodeComment code);

        /// <summary>
        /// Called when visiting a <see cref="DefInstruction"/> instruction.
        /// </summary>
        /// <param name="def">Visited def instruction.
        /// </param>
        void VisitDefInstruction(DefInstruction def);

        /// <summary>
        /// Called when visiting a <see cref="GotoInstruction"/> instruction.
        /// </summary>
        /// <param name="gotoInstruction">Visited goto instruction.
        /// </param>
        void VisitGotoInstruction(GotoInstruction gotoInstruction);

        /// <summary>
        /// Called when visiting a <see cref="PhiAssignment"/> instruction.
        /// </summary>
        /// <param name="phi">Visited phi assignment.
        /// </param>
        void VisitPhiAssignment(PhiAssignment phi);

        /// <summary>
        /// Called when visiting a <see cref="ReturnInstruction"/> instruction.
        /// </summary>
        /// <param name="ret">Visited return instruction.
        /// </param>
        void VisitReturnInstruction(ReturnInstruction ret);

        /// <summary>
        /// Called when visiting a <see cref="SideEffect"/> instruction.
        /// </summary>
        /// <param name="side">Visited side effect instruction.
        /// </param>
        void VisitSideEffect(SideEffect side);

        /// <summary>
        /// Called when visiting a <see cref="Store"/> instruction.
        /// </summary>
        /// <param name="store">Visited store instruction.
        /// </param>
		void VisitStore(Store store);

        /// <summary>
        /// Called when visiting a <see cref="SwitchInstruction"/> instruction.
        /// </summary>
        /// <param name="si">Visited switch instruction.
        /// </param>
		void VisitSwitchInstruction(SwitchInstruction si);

        /// <summary>
        /// Called when visiting a <see cref="UseInstruction"/> instruction.
        /// </summary>
        /// <param name="use">Visited switch instruction.
        /// </param>
        void VisitUseInstruction(UseInstruction use);
    }

    /// <summary>
    /// Visitor interface for <see cref="Instruction"/>s,
    /// where the visitor returns <typeparamref name="T"/> values.
    /// </summary>
    public interface InstructionVisitor<T>
    {
        /// <summary>
        /// Called when visiting an <see cref="Assignment"/> instruction.
        /// </summary>
        /// <param name="ass">Visited assignment.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
		T VisitAssignment(Assignment ass);

        /// <summary>
        /// Called when visiting a <see cref="Branch"/> instruction.
        /// </summary>
        /// <param name="branch">Visited branch instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
		T VisitBranch(Branch branch);

        /// <summary>
        /// Called when visiting a <see cref="CallInstruction"/> instruction.
        /// </summary>
        /// <param name="call">Visited call instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
		T VisitCallInstruction(CallInstruction call);

        /// <summary>
        /// Called when visiting a <see cref="CodeComment"/> instruction.
        /// </summary>
        /// <param name="code">Visited code comment.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitComment(CodeComment code);

        /// <summary>
        /// Called when visiting a <see cref="DefInstruction"/> instruction.
        /// </summary>
        /// <param name="def">Visited def instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitDefInstruction(DefInstruction def);

        /// <summary>
        /// Called when visiting a <see cref="GotoInstruction"/> instruction.
        /// </summary>
        /// <param name="gotoInstruction">Visited goto instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitGotoInstruction(GotoInstruction gotoInstruction);

        /// <summary>
        /// Called when visiting a <see cref="PhiAssignment"/> instruction.
        /// </summary>
        /// <param name="phi">Visited phi assignment.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitPhiAssignment(PhiAssignment phi);

        /// <summary>
        /// Called when visiting a <see cref="ReturnInstruction"/> instruction.
        /// </summary>
        /// <param name="ret">Visited return instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitReturnInstruction(ReturnInstruction ret);

        /// <summary>
        /// Called when visiting a <see cref="SideEffect"/> instruction.
        /// </summary>
        /// <param name="side">Visited side effect instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitSideEffect(SideEffect side);

        /// <summary>
        /// Called when visiting a <see cref="Store"/> instruction.
        /// </summary>
        /// <param name="store">Visited store instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
		T VisitStore(Store store);

        /// <summary>
        /// Called when visiting a <see cref="SwitchInstruction"/> instruction.
        /// </summary>
        /// <param name="si">Visited switch instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
		T VisitSwitchInstruction(SwitchInstruction si);

        /// <summary>
        /// Called when visiting a <see cref="UseInstruction"/> instruction.
        /// </summary>
        /// <param name="use">Visited switch instruction.
        /// </param>
        /// <returns>Value returned by visitor.</returns>
        T VisitUseInstruction(UseInstruction use);
    }

    /// <summary>
    /// Visitor interface for <see cref="Instruction"/>s,
    /// where the visitor is passed a caller context of type <typeparamref name="C"/>
    /// and returns <typeparamref name="T"/> values.
    /// </summary>
    public interface InstructionVisitor<T, C>
    {
        /// <summary>
        /// Called when visiting an <see cref="Assignment"/> instruction.
        /// </summary>
        /// <param name="ass">Visited assignment.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
		T VisitAssignment(Assignment ass, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="Branch"/> instruction.
        /// </summary>
        /// <param name="branch">Visited branch instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
		T VisitBranch(Branch branch, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="CallInstruction"/> instruction.
        /// </summary>
        /// <param name="call">Visited call instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
		T VisitCallInstruction(CallInstruction call, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="CodeComment"/> instruction.
        /// </summary>
        /// <param name="code">Visited code comment.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitComment(CodeComment code, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="DefInstruction"/> instruction.
        /// </summary>
        /// <param name="def">Visited def instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitDefInstruction(DefInstruction def, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="GotoInstruction"/> instruction.
        /// </summary>
        /// <param name="gotoInstruction">Visited goto instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitGotoInstruction(GotoInstruction gotoInstruction, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="PhiAssignment"/> instruction.
        /// </summary>
        /// <param name="phi">Visited phi assignment.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitPhiAssignment(PhiAssignment phi, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="ReturnInstruction"/> instruction.
        /// </summary>
        /// <param name="ret">Visited return instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitReturnInstruction(ReturnInstruction ret, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="SideEffect"/> instruction.
        /// </summary>
        /// <param name="side">Visited side effect instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitSideEffect(SideEffect side, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="Store"/> instruction.
        /// </summary>
        /// <param name="store">Visited store instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
		T VisitStore(Store store, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="SwitchInstruction"/> instruction.
        /// </summary>
        /// <param name="si">Visited switch instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
		T VisitSwitchInstruction(SwitchInstruction si, C ctx);

        /// <summary>
        /// Called when visiting a <see cref="UseInstruction"/> instruction.
        /// </summary>
        /// <param name="use">Visited switch instruction.
        /// </param>
        /// <param name="ctx">Caller context.</param>
        /// <returns>Value returned by visitor.</returns>
        T VisitUseInstruction(UseInstruction use, C ctx);
    }

    /// <summary>
    /// Useful base classes when only a few of the methods of InstructionVisitor and IExpressionVisitor 
    /// are actually implemented.
    /// </summary>
    public class InstructionVisitorBase : InstructionVisitor, IExpressionVisitor
	{
		#region InstructionVisitor Members

        /// <inheritdoc />
		public virtual void VisitAssignment(Assignment a)
		{
			a.Src.Accept(this);
			a.Dst.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitBranch(Branch b)
		{
			b.Condition.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitCallInstruction(CallInstruction ci)
		{
            ci.Callee.Accept(this);
		}

        /// <inheritdoc />
        public virtual void VisitComment(CodeComment comment)
        {
        }

        /// <inheritdoc />
		public virtual void VisitDefInstruction(DefInstruction def)
		{
			def.Identifier.Accept(this);
		}

        /// <inheritdoc />
        public virtual void VisitGotoInstruction(GotoInstruction g)
        {
            g.Target.Accept(this);
        }

        /// <inheritdoc />
		public virtual void VisitPhiAssignment(PhiAssignment phi)
		{
			phi.Src.Accept(this);
			phi.Dst.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitReturnInstruction(ReturnInstruction ret)
		{
			if (ret.Expression is not null)
				ret.Expression.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitSideEffect(SideEffect side)
		{
			side.Expression.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitStore(Store store)
		{
			store.Src.Accept(this);
			store.Dst.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitSwitchInstruction(SwitchInstruction si)
		{
			si.Expression.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitUseInstruction(UseInstruction u)
		{
			u.Expression.Accept(this);
		}

		#endregion

		#region IExpressionVisitor Members

        /// <inheritdoc />
        public virtual void VisitAddress(Address addr)
        {
        }

        /// <inheritdoc />
        public virtual void VisitApplication(Application appl)
        {
            appl.Procedure.Accept(this);
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                appl.Arguments[i].Accept(this);
            }
        }

        /// <inheritdoc />
		public virtual void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Index.Accept(this);
			acc.Array.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

        /// <inheritdoc />
        public virtual void VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.Condition.Accept(this);
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
        }

        /// <inheritdoc />
        public virtual void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitConstant(Constant c)
		{
		}

        /// <inheritdoc />
        public virtual void VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
        }

        /// <inheritdoc />
		public virtual void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitFieldAccess(FieldAccess acc)
		{
			acc.Structure.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
			access.MemoryId.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitMkSequence(MkSequence seq)
		{
            foreach (var e in seq.Expressions)
            {
                e.Accept(this);
            }
		}

        /// <inheritdoc />
		public virtual void VisitIdentifier(Identifier id)
		{
		}

        /// <inheritdoc />
        public virtual void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
        }

        /// <inheritdoc />
		public virtual void VisitPhiFunction(PhiFunction phi)
		{
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
			}
		}

        /// <inheritdoc />
		public virtual void VisitPointerAddition(PointerAddition pa)
		{
			pa.Pointer.Accept(this);
		}

        /// <inheritdoc />
		public virtual void VisitProcedureConstant(ProcedureConstant pc)
		{
		}

        /// <inheritdoc />
		public virtual void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
		}

        /// <inheritdoc />
        public virtual void VisitSegmentedAddress(SegmentedPointer access)
        {
            access.BasePointer.Accept(this);
            access.Offset.Accept(this);
        }

        /// <inheritdoc />
        public virtual void VisitScopeResolution(ScopeResolution scope)
		{
		}

        /// <inheritdoc />
		public virtual void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
		}

        /// <inheritdoc />
        public virtual void VisitStringConstant(StringConstant str)
        {
        }

        /// <inheritdoc />
		public virtual void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
		}

		#endregion
	}
}
