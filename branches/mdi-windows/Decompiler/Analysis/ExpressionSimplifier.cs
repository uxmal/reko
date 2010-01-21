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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Simplifies expressions by using common algebraic tricks and 
	/// other well known formulae.
	/// </summary>
	public class ExpressionSimplifier : IExpressionTransformer
	{
		private ValueNumbering dad;
		private Dictionary<Expression,Expression> table;

        public ExpressionSimplifier(ValueNumbering d, Dictionary<Expression, Expression> table)
		{
			this.dad = d;
			this.table = table;
		}

		private Expression AlgebraicSimplification(
			BinaryOperator binOp, 
			DataType valType,
			Expression left,
			Expression right)
		{
			Constant cLeft = PossibleConstant(left);
			Constant cRight = PossibleConstant(right);
				
			if (cLeft != null && cRight != null)
			{
				PrimitiveType lType = (PrimitiveType) cLeft.DataType;
				PrimitiveType rType = (PrimitiveType) cRight.DataType;
				if (lType.Domain != Domain.Real && lType.Domain != Domain.Real)	
				{
					// Only integral values can be safely simplified.
					if (binOp != Operator.eq)
						return SimplifyTwoConstants(binOp, cLeft, cRight);
				}
			}

			// C op id should be transformed to id op C, but only if op commutes.

			if (cLeft != null && BinaryExpression.Commutes(binOp))
			{
				Expression tmp = left; left = right; right = tmp;
			}

			//$REVIEW: identity on binaryoperators

			if (binOp == Operator.add)
			{
				if (IsZero(cRight))
					return left;
			} 
			else if (binOp == Operator.sub)
			{
				if (left == right)
					return MakeZero(left.DataType);
				if (IsZero(cRight))
					return left;
			} 
			else if (binOp == Operator.or || binOp == Operator.xor)
			{
				if (IsZero(cRight))
					return left;
			} 
            else if (binOp == Operator.and)
			{
				if (IsZero(cRight))
					return MakeZero(left.DataType);
			}
			Identifier idLeft = left as Identifier;
			Identifier idRight = right as Identifier;

			BinaryExpression binLeft = left as BinaryExpression;
			BinaryExpression binRight = right as BinaryExpression;

			// Order parameters in canonical order, so that two identifiers
			// are sorted in ascending order for commutable operation.

			if (BinaryExpression.Commutes(binOp) && IsLargerIdentifierNumber(left, right))
			{
				Expression tmp = left; left = right; right = tmp;
			}
			
			if (binLeft != null)
			{
				Constant cLeftRight = binLeft.Right as Constant;
				if (cLeftRight != null && cRight != null && binLeft.op == Operator.add && binOp == Operator.add)
				{
					return new BinaryExpression(binOp, valType, binLeft.Left, SimplifyTwoConstants(binOp, cLeftRight, cRight));
				}
			}

			return new BinaryExpression(binOp, valType, left, right);
		}

		private Constant MakeZero(DataType type)
		{
			return new Constant(type, 0);
		}

		private Constant PossibleConstant(Expression e)
		{
			Constant c = e as Constant;
			if (c == null)
			{
				Identifier left = e as Identifier;
				if (left != null && left != ValueNumbering.AnyValueNumber.Instance)
				{
					c = dad.GetDefiningExpression(left) as Constant;
				}
			}
			return c;
		}

		public Expression Simplify(Expression e)
		{
			return e.Accept(this);
		}

		private Expression SimplifyPhiFunction(Expression [] simpleParams)
		{
			Identifier any = ValueNumbering.AnyValueNumber.Instance;
			Expression eq = any;
			foreach (Expression vn in simpleParams)
			{
				if (vn != eq)
				{
					if (eq != any)
						return new PhiFunction(eq.DataType, simpleParams);
					else if (vn != any)
						eq = vn;
				}
			}
			return eq;
		}

		public Expression TransformApplication(Application app)
		{
			return app;
		}

		public Expression TransformArrayAccess(ArrayAccess acc)
		{
			acc.Index = acc.Index.Accept(this);
			acc.Array = acc.Array.Accept(this);
			return acc;
		}

		/// <summary>
		/// Simplifies a binary expression by finding algebraic equivalents.
		/// </summary>
		/// <param name="bin"></param>
		/// <returns></returns>
		public virtual Expression TransformBinaryExpression(BinaryExpression bin)
		{
			Expression simpleLeft = bin.Left.Accept(this);
			Expression simpleRight = bin.Right.Accept(this);

			return AlgebraicSimplification(bin.op, bin.DataType, simpleLeft, simpleRight);
		}

		public Expression TransformCast(Cast cast)
		{
			cast.Expression.Accept(this);
			Constant c = cast.Expression as Constant;
			if (c != null)
			{
				PrimitiveType p = c.DataType as PrimitiveType;
				if (p != null && p.IsIntegral)
				{
					return new Constant(cast.DataType, c.ToInt32());
				}
			}
			return cast;
		}

		public Expression TransformConditionOf(ConditionOf cc)
		{
			cc.Expression.Accept(this);
			return cc;
		}

		public Expression TransformConstant(Constant c)
		{
			return c;
		}

		public Expression TransformDepositBits(DepositBits d)
		{
			Expression src = d.Source.Accept(this);
			if (src is ValueNumbering.AnyValueNumber)
				return d;
			Expression ins = d.InsertedBits.Accept(this);
			if (ins is ValueNumbering.AnyValueNumber)
				return d;
			d.Source = src;
			d.InsertedBits = ins;
			return d;
		}

		public Expression TransformDereference(Dereference deref)
		{
			deref.Expression = deref.Expression.Accept(this);
			return deref;
		}

		public Expression TransformFieldAccess(FieldAccess acc)
		{
			acc.structure = acc.structure.Accept(this);
			return acc;
		}

		public Expression TransformPointerAddition(PointerAddition pa)
		{
			pa.Pointer = pa.Pointer.Accept(this);
			return pa;
		}

		public Expression TransformProcedureConstant(ProcedureConstant pc)
		{
			return pc;
		}

		public Expression TransformIdentifier(Identifier id)
		{
			if (id.Number >= 0)
				return dad.GetValueNumber(id);
			else
				return id;
		}

		public Expression TransformMemberPointerSelector(MemberPointerSelector mps)
		{
			Expression ptr = mps.BasePointer.Accept(this);
			Expression memberPtr = mps.MemberPointer.Accept(this);
			return new MemberPointerSelector(mps.DataType, ptr, memberPtr);
		}

		public Expression TransformMemoryAccess(MemoryAccess access)
		{
			Expression simpleExpr = access.EffectiveAddress.Accept(this);
			return new MemoryAccess(access.MemoryId, simpleExpr, access.DataType);
		}

		public Expression TransformMkSequence(MkSequence seq)
		{
			Expression head = seq.Head.Accept(this);
			Expression tail = seq.Tail.Accept(this);
			return new MkSequence(seq.DataType, head, tail);
		}

		public Expression TransformScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

		public Expression TransformSegmentedAccess(SegmentedAccess access)
		{
			Expression b = access.BasePointer.Accept(this);
			Expression ea = access.EffectiveAddress.Accept(this);
			return new SegmentedAccess(access.MemoryId, b, ea, access.DataType);
		}

		public Expression TransformPhiFunction(PhiFunction phi)
		{
			Expression [] simpleParams = new Expression[phi.Arguments.Length];
			int i = 0;
			foreach (Identifier id in phi.Arguments)
			{
				simpleParams[i] = id.Accept(this);
				++i;
			}
			return SimplifyPhiFunction(simpleParams);
		}
			
		public Expression TransformTestCondition(TestCondition tc)
		{
			return new TestCondition(tc.ConditionCode, tc.Expression.Accept(this));
		}

		public Expression TransformSlice(Slice slice)
		{
			return new Slice(slice.DataType, slice.Expression.Accept(this), (uint) slice.Offset);
		}

		public Expression TransformUnaryExpression(UnaryExpression unary)
		{
			if (unary.op == Operator.addrOf)
				return unary;
			Expression u = unary.Expression.Accept(this);
			if (u is ValueNumbering.AnyValueNumber)
				return unary;
			return new UnaryExpression(unary.op, unary.DataType, u);
		}
		

		public bool IsLargerIdentifierNumber(Expression e1, Expression e2)
		{
			Identifier id1 = e1 as Identifier;
			if (id1 == null)
				return false;
			Identifier id2 = e2 as Identifier;
			if (id2 == null)
				return false;
			return id1.Number > id2.Number;
		}

		public bool IsZero(Expression expr)
		{
			Constant c = expr as Constant;
			if (c == null)
				return false;
			return c.IsIntegerZero;
		}

		public Expression Lookup(Expression expr, Expression id)
		{
			return dad.Lookup(expr, table, id);
		}

		public static Constant SimplifyTwoConstants(BinaryOperator op, Constant l, Constant r)
		{
			PrimitiveType lType = (PrimitiveType) l.DataType;
			PrimitiveType rType = (PrimitiveType) r.DataType;
			if (lType.Domain != rType.Domain)
				throw new ArgumentException(string.Format("Can't add types of different domains {0} and {1}", l.DataType, r.DataType));
			return op.ApplyConstants(l, r);
		}
	}
}
