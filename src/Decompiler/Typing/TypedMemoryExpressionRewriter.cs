/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Rewrites expressions that are located in a memory expression context.
	/// </summary>
	public class TypedMemoryExpressionRewriter : IExpressionTransformer
	{
		private TypeStore store;
		private Identifier globals;

		public TypedMemoryExpressionRewriter(TypeStore store, Identifier globals)
		{
			this.store = store;
			this.globals = globals;
		}


		public Expression Rewrite(MemoryAccess access)
		{
			return access.EffectiveAddress.Accept(this);
		}

		public Expression TransformApplication(Application appl)
		{
			throw new NotImplementedException();
		}

		public Expression TransformArrayAccess(ArrayAccess acc)
		{
			throw new NotImplementedException();
		}

		public Expression TransformBinaryExpression(BinaryExpression binExp)
		{
			if (binExp.Left.TypeVariable.DataType.IsComplex)
			{
				if (binExp.Right.TypeVariable.DataType.IsComplex)
					throw new InvalidOperationException(string.Format("Both subexpressions are complex in {0}. Left type: {1}, right type {2}",
						binExp, binExp.Left.TypeVariable.DataType, binExp.Right.TypeVariable.DataType));
				Constant c = (Constant) binExp.Right;
				ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
					binExp.Left.TypeVariable.DataType,
					binExp.Left.TypeVariable.OriginalDataType,
					binExp.Left,
					c.AsInt32());
				ceb.Dereferenced = true;
				return ceb.BuildComplex();
			}
			else
			{
				throw new NotImplementedException(string.Format("{0}: [[{1}]] and [[{2}]]", binExp, binExp.Left.TypeVariable.DataType, binExp.Right.TypeVariable.DataType));
			}
		}

		public Expression TransformCast(Cast cast)
		{
			throw new NotImplementedException();
		}

		public Expression TransformConditionOf(ConditionOf cof)
		{
			throw new NotImplementedException();
		}

		public Expression TransformConstant(Constant c)
		{
			TypedConstantRewriter tcr = new TypedConstantRewriter(store, globals);
			return tcr.Rewrite(c, true);
		}

		public Expression TransformDepositBits(DepositBits d)
		{
			throw new NotImplementedException();
		}

		public Expression TransformDereference(Dereference deref)
		{
			throw new NotImplementedException();
		}

		public Expression TransformFieldAccess(FieldAccess acc)
		{
			throw new NotImplementedException();
		}

		public Expression TransformIdentifier(Identifier id)
		{
			ComplexExpressionBuilder ceb = new ComplexExpressionBuilder(
				id.TypeVariable.DataType,
				id.TypeVariable.OriginalDataType,
				id, 0);
			ceb.Dereferenced = true;
			return ceb.BuildComplex();
		}

		public Expression TransformMemberPointerSelector(MemberPointerSelector mps)
		{
			throw new NotImplementedException();
		}

		public Expression TransformMemoryAccess(MemoryAccess access)
		{
			throw new NotImplementedException();
		}

		public Expression TransformMkSequence(MkSequence seq)
		{
			throw new NotImplementedException();
		}

		public Expression TransformPhiFunction(PhiFunction phi)
		{
			throw new NotImplementedException();
		}

		public Expression TransformPointerAddition(PointerAddition pa)
		{
			throw new NotImplementedException();
		}

		public Expression TransformProcedureConstant(ProcedureConstant pc)
		{
			throw new NotImplementedException();
		}

		public Expression TransformSegmentedAccess(SegmentedAccess access)
		{
			throw new NotImplementedException();
		}

		public Expression TransformSlice(Slice slice)
		{
			throw new NotImplementedException();
		}

		public Expression TransformTestCondition(TestCondition tc)
		{
			throw new NotImplementedException();
		}

		public Expression TransformUnaryExpression(UnaryExpression unary)
		{
			throw new NotImplementedException();
		}
	}
}
