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
#pragma warning disable IDE1006
#pragma warning disable CS1591
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
        /// <summary>
        /// Called when visiting an <see cref="Address"/> expression.
        /// </summary>
        /// <param name="addr">The visited address.</param>
        void VisitAddress(Address addr);

        /// <summary>
        /// Called when visiting an <see cref="Application"/> expression.
        /// </summary>
        /// <param name="appl">The visited application.</param>
		void VisitApplication(Application appl);

        /// <summary>
        /// Called when visiting an <see cref="ArrayAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited array access.</param>
		void VisitArrayAccess(ArrayAccess acc);

        /// <summary>
        /// Called when visiting a <see cref="BinaryExpression"/> expression.
        /// </summary>
        /// <param name="binExp">The visited binary expression.</param>
		void VisitBinaryExpression(BinaryExpression binExp);

        /// <summary>
        /// Called when visiting a <see cref="Cast"/> expression.
        /// </summary>
        /// <param name="cast">The visited cast expression.</param>
		void VisitCast(Cast cast);

        /// <summary>
        /// Called when visiting a ternary <see cref="ConditionalExpression"/> expression.
        /// </summary>
        /// <param name="cond">The visited conditional expression.</param>
        void VisitConditionalExpression(ConditionalExpression cond);

        /// <summary>
        /// Called when visiting a <see cref="ConditionOf"/> expression.
        /// </summary>
        /// <param name="cof">The visited condition-of expression.</param>
        void VisitConditionOf(ConditionOf cof);

        /// <summary>
        /// Called when visiting a <see cref="Constant"/> expression.
        /// </summary>
        /// <param name="c">The visited constant expression.</param>
		void VisitConstant(Constant c);

        /// <summary>
        /// Called when visiting a <see cref="Conversion">conversion</see> expression.
        /// </summary>
        /// <param name="conversion">The visited conversion expression</param>
        void VisitConversion(Conversion conversion);

        /// <summary>
        /// Called when visiting a <see cref="Dereference"/> expression.
        /// </summary>
        /// <param name="deref">The visited dereference expression.</param>
		void VisitDereference(Dereference deref);

        /// <summary>
        /// Called when visiting a <see cref="FieldAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited field access expression.</param>
		void VisitFieldAccess(FieldAccess acc);

        /// <summary>
        /// Called when visiting an <see cref="Identifier"/>.
        /// </summary>
        /// <param name="id">The visited identifier.</param>
		void VisitIdentifier(Identifier id);

        /// <summary>
        /// Called when visiting a <see cref="MemberPointerSelector">member pointer selector expression</see>.
        /// </summary>
        /// <param name="mps">The visited member pointer selector expression.</param>
		void VisitMemberPointerSelector(MemberPointerSelector mps);

        /// <summary>
        /// Called when visiting a <see cref="MemoryAccess">memory access</see>.
        /// </summary>
        /// <param name="access">The visitied memory access.</param>
		void VisitMemoryAccess(MemoryAccess access);

        /// <summary>
        /// Called when visiting a sequence expression.
        /// </summary>
        /// <param name="seq">The visited sequence expression.</param>
        void VisitMkSequence(MkSequence seq);

        /// <summary>
        /// Called when vising an output argument.
        /// </summary>
        /// <param name="outArgument">The visited output argument.</param>
        void VisitOutArgument(OutArgument outArgument);

        /// <summary>
        /// Called when visiting a <see cref="PhiFunction">phi function</see>.
        /// </summary>
        /// <param name="phi">The visited phi function.</param>
        void VisitPhiFunction(PhiFunction phi);

        /// <summary>
        /// Called when visiting a <see cref="PointerAddition">pointer addition</see> expression.
        /// </summary>
        /// <param name="pa">The visited pointer addition expression.</param>
		void VisitPointerAddition(PointerAddition pa);

        /// <summary>
        /// Called when visiting a <see cref="ProcedureConstant">procedure constant</see>.
        /// </summary>
        /// <param name="pc">The visited procedure constant.</param>
		void VisitProcedureConstant(ProcedureConstant pc);

        /// <summary>
        /// Called when visiting a <see cref="ScopeResolution">scope resolution</see> expression.
        /// </summary>
        /// <param name="scopeResolution"></param>
		void VisitScopeResolution(ScopeResolution scopeResolution);

        /// <summary>
        /// Called when visiting a <see cref="SegmentedPointer">segmented pointer</see> expression.
        /// </summary>
        /// <param name="address">The visited segmented pointer.</param>
        void VisitSegmentedAddress(SegmentedPointer address);

        /// <summary>
        /// Called when visiting a <see cref="Slice">slice expression</see>.
        /// </summary>
        /// <param name="slice">The visited slice expression.</param>
        void VisitSlice(Slice slice);

        /// <summary>
        /// Called when visting a <see cref="StringConstant">string literal constant</see>.
        /// </summary>
        /// <param name="str">The visited string literal.</param>
        void VisitStringConstant(StringConstant str);

        /// <summary>
        /// Called when visiting a <see cref="TestCondition"/> expression.
        /// </summary>
        /// <param name="tc">The visited test condition.</param>
        void VisitTestCondition(TestCondition tc);

        /// <summary>
        /// Called when visiting a <see cref="UnaryExpression">unary expression</see>.
        /// </summary>
        /// <param name="unary">The visited unary expression.</param>
        void VisitUnaryExpression(UnaryExpression unary);
    }

    /// <summary>
    /// Using the 'Visitor' pattern, any class interested in examining 
    /// classes implementing the <see cref="Expression"/> interface 
    /// without having to probe their class
    /// type explicity (with casts or 'as' and 'is'). The generic type
    /// <typeparamref name="T"/> is the return type of the visitor methods.
    /// </summary>
    public interface ExpressionVisitor<T>
    {
        /// <summary>
        /// Called when visiting an <see cref="Address"/> expression.
        /// </summary>
        /// <param name="addr">The visited address.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitAddress(Address addr);

        /// <summary>
        /// Called when visiting an <see cref="Application"/> expression.
        /// </summary>
        /// <param name="appl">The visited application.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitApplication(Application appl);

        /// <summary>
        /// Called when visiting an <see cref="ArrayAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited array access.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitArrayAccess(ArrayAccess acc);

        /// <summary>
        /// Called when visiting a <see cref="BinaryExpression"/> expression.
        /// </summary>
        /// <param name="binExp">The visited binary expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitBinaryExpression(BinaryExpression binExp);

        /// <summary>
        /// Called when visiting a <see cref="Cast"/> expression.
        /// </summary>
        /// <param name="cast">The visited cast expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitCast(Cast cast);

        /// <summary>
        /// Called when visiting a ternary <see cref="ConditionalExpression"/> expression.
        /// </summary>
        /// <param name="cond">The visited conditional expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConditionalExpression(ConditionalExpression cond);

        /// <summary>
        /// Called when visiting a <see cref="ConditionOf"/> expression.
        /// </summary>
        /// <param name="cof">The visited condition-of expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConditionOf(ConditionOf cof);

        /// <summary>
        /// Called when visiting a <see cref="Constant"/> expression.
        /// </summary>
        /// <param name="c">The visited constant expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitConstant(Constant c);

        /// <summary> 
        /// Called when visiting a <see cref="Conversion">conversion</see> expression.
        /// </summary>
        /// <param name="conversion">The visited conversion expression</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConversion(Conversion conversion);

        /// <summary>
        /// Called when visiting a <see cref="Dereference"/> expression.
        /// </summary>
        /// <param name="deref">The visited dereference expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitDereference(Dereference deref);

        /// <summary>
        /// Called when visiting a <see cref="FieldAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited field access expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitFieldAccess(FieldAccess acc);

        /// <summary>
        /// Called when visiting an <see cref="Identifier"/>.
        /// </summary>
        /// <param name="id">The visited identifier.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitIdentifier(Identifier id);

        /// <summary>
        /// Called when visiting a <see cref="MemberPointerSelector">member pointer selector expression</see>.
        /// </summary>
        /// <param name="mps">The visited member pointer selector expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitMemberPointerSelector(MemberPointerSelector mps);

        /// <summary>
        /// Called when visiting a <see cref="MemoryAccess">memory access</see>.
        /// </summary>
        /// <param name="access">The visitied memory access.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitMemoryAccess(MemoryAccess access);

        /// <summary>
        /// Called when visiting a sequence expression.
        /// </summary>
        /// <param name="seq">The visited sequence expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitMkSequence(MkSequence seq);

        /// <summary>
        /// Called when vising an output argument.
        /// </summary>
        /// <param name="outArgument">The visited output argument.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitOutArgument(OutArgument outArgument);

        /// <summary>
        /// Called when visiting a <see cref="PhiFunction">phi function</see>.
        /// </summary>
        /// <param name="phi">The visited phi function.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitPhiFunction(PhiFunction phi);

        /// <summary>
        /// Called when visiting a <see cref="PointerAddition">pointer addition</see> expression.
        /// </summary>
        /// <param name="pa">The visited pointer addition expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitPointerAddition(PointerAddition pa);

        /// <summary>
        /// Called when visiting a <see cref="ProcedureConstant">procedure constant</see>.
        /// </summary>
        /// <param name="pc">The visited procedure constant.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitProcedureConstant(ProcedureConstant pc);

        /// <summary>
        /// Called when visiting a <see cref="ScopeResolution">scope resolution</see> expression.
        /// </summary>
        /// <param name="scopeResolution"></param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitScopeResolution(ScopeResolution scopeResolution);

        /// <summary>
        /// Called when visiting a <see cref="SegmentedPointer">segmented pointer</see> expression.
        /// </summary>
        /// <param name="address">The visited segmented pointer.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSegmentedAddress(SegmentedPointer address);

        /// <summary>
        /// Called when visiting a <see cref="Slice">slice expression</see>.
        /// </summary>
        /// <param name="slice">The visited slice expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSlice(Slice slice);

        /// <summary>
        /// Called when visting a <see cref="StringConstant">string literal constant</see>.
        /// </summary>
        /// <param name="str">The visited string literal.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitStringConstant(StringConstant str);

        /// <summary>
        /// Called when visiting a <see cref="TestCondition"/> expression.
        /// </summary>
        /// <param name="tc">The visited test condition.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitTestCondition(TestCondition tc);

        /// <summary>
        /// Called when visiting a <see cref="UnaryExpression">unary expression</see>.
        /// </summary>
        /// <param name="unary">The visited unary expression.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitUnaryExpression(UnaryExpression unary);
    }

    /// <summary>
    /// Using the 'Visitor' pattern, any class interested in examining 
    /// classes implementing the <see cref="Expression"/> interface 
    /// without having to probe their class
    /// type explicity (with casts or 'as' and 'is'). A context <typeparamref name="C"/>
    /// is passed as an argument at each visited expression.
    /// The generic type
    /// <typeparamref name="T"/> is the return type of the visitor methods.
    /// </summary>
    public interface ExpressionVisitor<T, C>
    {
        /// <summary>
        /// Called when visiting an <see cref="Address"/> expression.
        /// </summary>
        /// <param name="addr">The visited address.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitAddress(Address addr, C context);

        /// <summary>
        /// Called when visiting an <see cref="Application"/> expression.
        /// </summary>
        /// <param name="appl">The visited application.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitApplication(Application appl, C context);

        /// <summary>
        /// Called when visiting an <see cref="ArrayAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited array access.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitArrayAccess(ArrayAccess acc, C context);

        /// <summary>
        /// Called when visiting a <see cref="BinaryExpression"/> expression.
        /// </summary>
        /// <param name="binExp">The visited binary expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitBinaryExpression(BinaryExpression binExp, C context);

        /// <summary>
        /// Called when visiting a <see cref="Cast"/> expression.
        /// </summary>
        /// <param name="cast">The visited cast expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitCast(Cast cast, C context);

        /// <summary>
        /// Called when visiting a ternary <see cref="ConditionalExpression"/> expression.
        /// </summary>
        /// <param name="cond">The visited conditional expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConditionalExpression(ConditionalExpression cond, C context);

        /// <summary>
        /// Called when visiting a <see cref="ConditionOf"/> expression.
        /// </summary>
        /// <param name="cof">The visited condition-of expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConditionOf(ConditionOf cof, C context);

        /// <summary>
        /// Called when visiting a <see cref="Constant"/> expression.
        /// </summary>
        /// <param name="c">The visited constant expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitConstant(Constant c, C context);

        /// <summary> 
        /// Called when visiting a <see cref="Conversion">conversion</see> expression.
        /// </summary>
        /// <param name="conversion">The visited conversion expression</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitConversion(Conversion conversion, C context);

        /// <summary>
        /// Called when visiting a <see cref="Dereference"/> expression.
        /// </summary>
        /// <param name="deref">The visited dereference expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitDereference(Dereference deref, C context);

        /// <summary>
        /// Called when visiting a <see cref="FieldAccess"/> expression.
        /// </summary>
        /// <param name="acc">The visited field access expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitFieldAccess(FieldAccess acc, C context);

        /// <summary>
        /// Called when visiting an <see cref="Identifier"/>.
        /// </summary>
        /// <param name="id">The visited identifier.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitIdentifier(Identifier id, C context);

        /// <summary>
        /// Called when visiting a <see cref="MemberPointerSelector">member pointer selector expression</see>.
        /// </summary>
        /// <param name="mps">The visited member pointer selector expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitMemberPointerSelector(MemberPointerSelector mps, C context);

        /// <summary>
        /// Called when visiting a <see cref="MemoryAccess">memory access</see>.
        /// </summary>
        /// <param name="access">The visitied memory access.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitMemoryAccess(MemoryAccess access, C context);

        /// <summary>
        /// Called when visiting a sequence expression.
        /// </summary>
        /// <param name="seq">The visited sequence expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitMkSequence(MkSequence seq, C context);

        /// <summary>
        /// Called when vising an output argument.
        /// </summary>
        /// <param name="outArgument">The visited output argument.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitOutArgument(OutArgument outArgument, C context);

        /// <summary>
        /// Called when visiting a <see cref="PhiFunction">phi function</see>.
        /// </summary>
        /// <param name="phi">The visited phi function.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitPhiFunction(PhiFunction phi, C context);

        /// <summary>
        /// Called when visiting a <see cref="PointerAddition">pointer addition</see> expression.
        /// </summary>
        /// <param name="pa">The visited pointer addition expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitPointerAddition(PointerAddition pa, C context);

        /// <summary>
        /// Called when visiting a <see cref="ProcedureConstant">procedure constant</see>.
        /// </summary>
        /// <param name="pc">The visited procedure constant.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitProcedureConstant(ProcedureConstant pc, C context);

        /// <summary>
        /// Called when visiting a <see cref="ScopeResolution">scope resolution</see> expression.
        /// </summary>
        /// <param name="scopeResolution"></param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
		T VisitScopeResolution(ScopeResolution scopeResolution, C context);

        /// <summary>
        /// Called when visiting a <see cref="SegmentedPointer">segmented pointer</see> expression.
        /// </summary>
        /// <param name="address">The visited segmented pointer.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSegmentedAddress(SegmentedPointer address, C context);

        /// <summary>
        /// Called when visiting a <see cref="Slice">slice expression</see>.
        /// </summary>
        /// <param name="slice">The visited slice expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitSlice(Slice slice, C context);

        /// <summary>
        /// Called when visting a <see cref="StringConstant">string literal constant</see>.
        /// </summary>
        /// <param name="str">The visited string literal.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitStringConstant(StringConstant str, C context);

        /// <summary>
        /// Called when visiting a <see cref="TestCondition"/> expression.
        /// </summary>
        /// <param name="tc">The visited test condition.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitTestCondition(TestCondition tc, C context);

        /// <summary>
        /// Called when visiting a <see cref="UnaryExpression">unary expression</see>.
        /// </summary>
        /// <param name="unary">The visited unary expression.</param>
        /// <param name="context">The context provided by the caller.</param>
        /// <returns>The result of the visit as an instance of <typeparamref name="T"/>.</returns>
        T VisitUnaryExpression(UnaryExpression unary, C context);
    }

    /// <summary>
    /// Boiler-plate implementation of the <see cref="IExpressionVisitor"/> interface.
    /// </summary>
    /// <remarks>
    /// This class is used as a base class for implementation of the <see cref="IExpressionVisitor"/>
    /// interface where only a few methods need to be overridden.
    /// </remarks>
    public class ExpressionVisitorBase : IExpressionVisitor
	{
		#region IExpressionVisitor Members

        /// <inheritdoc/>
        public virtual void VisitAddress(Address addr)
        {
        }

        /// <inheritdoc/>
		public virtual void VisitApplication(Application appl)
		{
			appl.Procedure.Accept(this);
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i].Accept(this);
			}
		}

        /// <inheritdoc/>
		public void VisitArrayAccess(ArrayAccess acc)
		{
			acc.Array.Accept(this);
			acc.Index.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitBinaryExpression(BinaryExpression binExp)
		{
			binExp.Left.Accept(this);
			binExp.Right.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
		}

        /// <inheritdoc/>
        public void VisitConditionalExpression(ConditionalExpression cond)
        {
            cond.Condition.Accept(this);
            cond.ThenExp.Accept(this);
            cond.FalseExp.Accept(this);
        }

        /// <inheritdoc/>
        public void VisitConditionOf(ConditionOf cof)
		{
			cof.Expression.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitConstant(Constant c)
		{
		}

        /// <inheritdoc/>
        public virtual void VisitConversion(Conversion conversion)
        {
            conversion.Expression.Accept(this);
        }

        /// <inheritdoc/>
		public void VisitDereference(Dereference deref)
		{
			deref.Expression.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitFieldAccess(FieldAccess acc)
		{
			acc.Structure.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer.Accept(this);
			mps.MemberPointer.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
            access.MemoryId.Accept(this);
        }

        /// <inheritdoc/>
		public void VisitMkSequence(MkSequence seq)
		{
            for (int i = 0; i < seq.Expressions.Length; ++i)
            {
                seq.Expressions[i].Accept(this);
            }
        }

        /// <inheritdoc/>
		public virtual void VisitIdentifier(Identifier id)
		{
		}

        /// <inheritdoc/>
        public virtual void VisitOutArgument(OutArgument outArg)
        {
            outArg.Expression.Accept(this);
        }

        /// <inheritdoc/>
		public void VisitPhiFunction(PhiFunction phi)
		{
			foreach (var arg in phi.Arguments)
			{
				arg.Value.Accept(this);
			}
		}

        /// <inheritdoc/>
		public void VisitPointerAddition(PointerAddition pa)
		{
			pa.Pointer.Accept(this);
		}

        /// <inheritdoc/>
		public void VisitProcedureConstant(ProcedureConstant pc)
		{
		}

        /// <inheritdoc/>
		public void VisitTestCondition(TestCondition tc)
		{
			tc.Expression.Accept(this);
		}

        /// <inheritdoc/>
        public void VisitSegmentedAddress(SegmentedPointer address)
        {
            address.BasePointer.Accept(this);
            address.Offset.Accept(this);
        }

        /// <inheritdoc/>
        public void VisitScopeResolution(ScopeResolution scope)
		{
		}

        /// <inheritdoc/>
		public void VisitSlice(Slice slice)
		{
			slice.Expression.Accept(this);
		}

        /// <inheritdoc/>
        public void VisitStringConstant(StringConstant str)
        {
        }

        /// <inheritdoc/>
		public void VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression.Accept(this);
		}

		#endregion
	}

    /// <summary>
    /// Boiler-plate implementation of the <see cref="ExpressionVisitor{T}"/> interface.
    /// </summary>
    /// <remarks>
    /// This class is used as a base class for implementation of the <see cref="IExpressionVisitor"/>
    /// interface where only a few methods need to be overridden.
    /// </remarks>
    public abstract class ExpressionVisitorBase<T> : ExpressionVisitor<T>
    {
        /// <summary>
        /// Default value to use if derived classes provide no overriding implementation.
        /// </summary>
        public abstract T DefaultValue { get; }

        /// <inheritdoc/>
        public virtual T VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitApplication(Application appl)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitBinaryExpression(BinaryExpression binExp)
        {
            binExp.Left.Accept(this);
            binExp.Right.Accept(this);
            return DefaultValue;
        }

        /// <inheritdoc/>
        public virtual T VisitCast(Cast cast)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitConditionalExpression(ConditionalExpression cond)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitConditionOf(ConditionOf cof)
        {
            return cof.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public virtual T VisitConstant(Constant c)
        {
            return DefaultValue;
        }

        /// <inheritdoc/>
        public virtual T VisitConversion(Conversion conversion)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitIdentifier(Identifier id)
        {
            return DefaultValue;
        }

        /// <inheritdoc/>
        public virtual T VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitMemoryAccess(MemoryAccess access)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitMkSequence(MkSequence seq)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitOutArgument(OutArgument outArg)
        {
            return outArg.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public virtual T VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitProcedureConstant(ProcedureConstant pc)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitSegmentedAddress(SegmentedPointer address)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitSlice(Slice slice)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitStringConstant(StringConstant str)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual T VisitTestCondition(TestCondition tc)
        {
            return tc.Expression.Accept(this);
        }

        /// <inheritdoc/>
        public virtual T VisitUnaryExpression(UnaryExpression unary)
        {
            throw new NotImplementedException();
        }
    }
}
