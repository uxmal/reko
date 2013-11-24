#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Rewrites expressions that are located in a memory expression context.
	/// </summary>
	public class TypedMemoryExpressionRewriter : ExpressionVisitor<Expression>
	{
		private TypeStore store;
		private Identifier globals;
		private Expression basePointer;
        private DataType dtResult;

		public TypedMemoryExpressionRewriter(TypeStore store, Identifier globals)
		{
			this.store = store;
			this.globals = globals;
		}

		public Expression Rewrite(MemoryAccess access)
		{
			basePointer = null;
            dtResult = new Pointer(access.TypeVariable.DataType, access.EffectiveAddress.DataType.Size);
			return access.EffectiveAddress.Accept(this);
		}

		public Expression Rewrite(SegmentedAccess access)
		{
			basePointer = access.BasePointer;
            dtResult = new MemberPointer(
                access.BasePointer.TypeVariable.DataType,
                access.TypeVariable.DataType,
                access.EffectiveAddress.DataType.Size);
			return access.EffectiveAddress.Accept(this);
		}

        public Expression VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

		public Expression VisitApplication(Application appl)
		{
			throw new NotImplementedException();
		}

		public Expression VisitArrayAccess(ArrayAccess acc)
		{
			throw new NotImplementedException();
		}

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            Expression left = binExp.Left;
            Expression right = binExp.Right;
            TypeVariable tvLeft = left.TypeVariable;
            TypeVariable tvRight = right.TypeVariable;

            if (!tvLeft.DataType.IsComplex)
            {
                if (!tvRight.DataType.IsComplex)
                    throw new NotImplementedException(string.Format("Neither subexpression is complex in {0}: [[{1}]] and [[{2}]]",
                        binExp,
                        tvLeft.DataType,
                        tvRight.DataType));
                return VisitBinaryExpression(binExp.Commute());
            }
            else if (tvRight.DataType.IsComplex)
            {
                throw new TypeInferenceException("Both subexpressions are complex in {0}. Left type: {1}, right type {2}",
                    binExp, tvLeft.DataType, tvRight.DataType);
            }

            TypedExpressionRewriter ter = new TypedExpressionRewriter(store, globals);

            ComplexExpressionBuilder ceb;
            Constant cLeft = left as Constant;
            if (cLeft != null)
            {
                binExp.Right = binExp.Right.Accept(ter);
                if (basePointer == null)
                    basePointer = globals;
                ceb = new ComplexExpressionBuilder(
                    dtResult,
                    basePointer.TypeVariable.DataType,
                    basePointer.TypeVariable.OriginalDataType,
                    null,
                    basePointer,
                    binExp.Right,
                    StructureField.ToOffset(cLeft));
            }
            else
            {
                binExp.Left = binExp.Left.Accept(ter);
                binExp.Right = binExp.Right.Accept(ter);
                ceb = new ComplexExpressionBuilder(
                    binExp.DataType,
                    tvLeft.DataType,
                    tvLeft.OriginalDataType,
                    basePointer,
                    binExp.Left,
                    null,
                    StructureField.ToOffset(right as Constant));
            }
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
        }
		

		public Expression VisitCast(Cast cast)
		{
			throw new NotImplementedException();
		}

		public Expression VisitConditionOf(ConditionOf cof)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// A constant in a memory context is a pointer or a member pointer. It is always unsigned.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public Expression VisitConstant(Constant c)
		{
            return VisitConstant(c, true);
		}

        private Expression VisitConstant(Constant c, bool dereferenced)
        {
            if (basePointer != null)
            {
                ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                    dtResult,
                    basePointer.TypeVariable.DataType,
                    basePointer.TypeVariable.OriginalDataType,
                    null,
                    basePointer,
                    null,
                    StructureField.ToOffset(c));
                ceb.Dereferenced = dereferenced;
                return ceb.BuildComplex();
            }
            else
            {
                TypedConstantRewriter tcr = new TypedConstantRewriter(store, globals);
                return tcr.Rewrite(c, dereferenced);
            }
        }

		public Expression VisitDepositBits(DepositBits d)
		{
			throw new NotImplementedException();
		}

		public Expression VisitDereference(Dereference deref)
		{
			throw new NotImplementedException();
		}

		public Expression VisitFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

		public Expression VisitIdentifier(Identifier id)
		{
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                dtResult,
				id.TypeVariable.DataType,
				id.TypeVariable.OriginalDataType,
				basePointer, id, null, 0);
			ceb.Dereferenced = true;
			return ceb.BuildComplex();
		}

		public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			throw new NotImplementedException();
		}

		public Expression VisitMemoryAccess(MemoryAccess access)
		{
			throw new NotImplementedException();
		}

		public Expression VisitMkSequence(MkSequence seq)
		{
			throw new NotImplementedException();
		}

		public Expression VisitPhiFunction(PhiFunction phi)
		{
			throw new NotImplementedException();
		}

		public Expression VisitPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

		public Expression VisitProcedureConstant(ProcedureConstant pc)
		{
			throw new NotImplementedException();
		}

		public Expression VisitSegmentedAccess(SegmentedAccess access)
		{
            TypedMemoryExpressionRewriter r = new TypedMemoryExpressionRewriter(store, globals);
            Expression e = r.Rewrite(access);
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                dtResult,
                e.DataType,
                e.DataType,
                basePointer,
                e, null, 0);
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
		}

		public Expression VisitScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

		public Expression VisitSlice(Slice slice)
		{
			throw new NotImplementedException();
		}

		public Expression VisitTestCondition(TestCondition tc)
		{
			throw new NotImplementedException();
		}

		public Expression VisitUnaryExpression(UnaryExpression unary)
		{
			throw new NotImplementedException();
		}
	}
}
