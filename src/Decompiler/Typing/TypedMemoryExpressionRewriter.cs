#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core.Operators;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Rewrites expressions that are located in a memory expression context.
	/// </summary>
	public class TypedMemoryExpressionRewriter : ExpressionVisitor<Expression>
	{
        private Program prog;
        private Platform platform;
        private TypeStore store;
		private Identifier globals;
		private Expression basePointer;
        private DataType dtResult;
        private TypedConstantRewriter tcr;

		public TypedMemoryExpressionRewriter(Program prog)
		{
            this.prog = prog;
            this.platform = prog.Platform;
            this.tcr = new TypedConstantRewriter(prog);
			this.store = prog.TypeStore;
			this.globals = prog.Globals;
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

        public Expression RewriteArrayAccess(TypeVariable typeVariable, Expression arr, Expression idx)
        {
            var ter = new TypedExpressionRewriter(prog);
            basePointer = null;
            dtResult = new Pointer(typeVariable.DataType, platform.PointerType.Size);
            var dtElement = Dereference(arr.TypeVariable.DataType);
            var dtElementOrig = Dereference(arr.TypeVariable.OriginalDataType);
            arr = arr.Accept(ter);
            idx = idx.Accept(ter);
            idx = RescaleIndex(idx, dtElement);
            var ceb = new ComplexExpressionBuilder(
                dtResult,
                dtElement,
                dtElementOrig,
                basePointer,
                new ArrayAccess(dtElement, arr, idx),
                null, 
                0);
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
        }

        private Expression RescaleIndex(Expression idx, DataType dtElement)
        {
            if (dtElement.Size == 1)
                return idx;
            var bin = idx as BinaryExpression;
            if (bin != null && bin.Operator is IMulOperator)
            {
                var k = bin.Right as Constant;
                if (k != null)
                {
                    var kk = k.ToInt32();
                    if (kk % dtElement.Size == 0)
                    {
                        kk /= dtElement.Size;
                        if (kk == 1)
                            return bin.Left;
                        else
                            return new BinaryExpression(bin.Operator, bin.DataType, bin.Left, Constant.Create(k.DataType, kk));
                    }
                }
            }
            return new BinaryExpression(Operator.SDiv, idx.DataType, idx, Constant.Create(idx.DataType, dtElement.Size));
        }

        private DataType Dereference(DataType dt)
        {
            var pt = dt as Pointer;
            if (pt != null)
                return Dereference(pt.Pointee);
            var at = dt as ArrayType;
            if (at != null)
                return at.ElementType;
            var st = dt as StructureType;
            if (st != null)
            {
                var f0 = st.Fields.AtOffset(0);
                if (f0 != null && f0.DataType is ArrayType)
                    return ((ArrayType) f0.DataType).ElementType;
            }
            return dt;
        }

        public Expression VisitAddress(Address addr)
        {
            return tcr.Rewrite(addr, true);
        }

        public Expression VisitApplication(Application appl)
        {
            //$BUGBUG: should rewerite the fn and the args too.
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                dtResult,
                appl.TypeVariable.DataType,
                appl.TypeVariable.OriginalDataType,
                basePointer, appl, null, 0);
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
        }

		public Expression VisitArrayAccess(ArrayAccess acc)
		{
            var ter = new TypedExpressionRewriter(prog);
            var arr = acc.Array.Accept(ter);
            var idx = acc.Index.Accept(ter);
            var ceb = new ComplexExpressionBuilder(
                acc.TypeVariable.DataType,
                acc.Array.TypeVariable.DataType,
                acc.Array.TypeVariable.OriginalDataType,
                basePointer,
                arr,
                idx,
                0);
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
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

            var ter = new TypedExpressionRewriter(prog);
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
                var binLeft = binExp.Left.Accept(ter);
                var binRight = binExp.Right.Accept(ter);
                var cRight = binRight as Constant;
                ceb = new ComplexExpressionBuilder(
                    binExp.DataType,
                    tvLeft.DataType,
                    tvLeft.OriginalDataType,
                    basePointer,
                    binLeft,
                    cRight != null ? null : binRight,
                    StructureField.ToOffset(binRight as Constant));
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
            TypedMemoryExpressionRewriter r = new TypedMemoryExpressionRewriter(prog);
            Expression e = r.Rewrite(access);
            ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                dtResult,
                e.DataType,
                e.DataType,
                null,
                e, null, 0);
            ceb.Dereferenced = true;
            return ceb.BuildComplex();
        }

		public Expression VisitMkSequence(MkSequence seq)
		{
            var ter = new TypedExpressionRewriter(prog);
            //$TODO: identical to TypedExpressionRewriter except for the ceb.Dereferenced statements. How to combine?
            var head = seq.Head.Accept(ter);
            var tail = seq.Tail.Accept(ter);
            Constant c = tail as Constant;
            var ptHead = head.TypeVariable.DataType as PrimitiveType;
            if (head.TypeVariable.DataType is Pointer || (ptHead != null && ptHead.Domain == Domain.Selector))
            {
                if (c != null)
                {
                    ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
                        seq.TypeVariable.DataType,
                        head.DataType,
                        head.TypeVariable.OriginalDataType,
                        null,
                        head,
                        null,
                        StructureField.ToOffset(c));
                    ceb.Dereferenced = true;
                    return ceb.BuildComplex();
                }
                else
                {
                    var ceb = new ComplexExpressionBuilder(
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.DataType,
                        seq.TypeVariable.OriginalDataType,
                        head,
                        tail,
                        null,
                        0);
                    ceb.Dereferenced = true;
                    return ceb.BuildComplex();
                }
            }
            else
            {
            }
            return new MkSequence(seq.DataType, head, tail);
        }

        public Expression VisitOutArgument(OutArgument outArg)
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
            var r = new TypedMemoryExpressionRewriter(prog);
            var e = r.Rewrite(access);
            var ceb = new ComplexExpressionBuilder(
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
