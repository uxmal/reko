#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Evaluation;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Evaluation 
{
    /// <summary>
    /// Partially evaluates expressions, using an <see cref="EvaluationContext"/> to obtain the values
    /// of identifiers and optionally modifies the expression being evaluated.
    /// </summary>
    public class ExpressionSimplifier : ExpressionVisitor<Expression>
    {
        private EvaluationContext ctx;

        private AddTwoIdsRule add2ids;
        private Add_e_c_cRule addEcc;
        private Add_mul_id_c_id_Rule addMici;
        private ConstConstBin_Rule constConstBin;
        private DpbConstantRule dpbConstantRule;
        private IdConstant idConst;
        private IdCopyPropagationRule idCopyPropagation;
        private IdBinIdc_Rule idBinIdc;
        private SliceConstant_Rule sliceConst;
        private SliceMem_Rule sliceMem;
        private Shl_mul_e_Rule shMul;
        private ShiftShift_c_c_Rule shiftShift;
        private SliceShift sliceShift;
        private NegSub_Rule negSub;
        private Mps_Constant_Rule mpsRule;
        private BinOpWithSelf_Rule binopWithSelf;

        public ExpressionSimplifier(EvaluationContext ctx)
        {
            this.ctx = ctx;

            this.add2ids = new AddTwoIdsRule(ctx);
            this.addEcc = new Add_e_c_cRule(ctx);
            this.addMici = new Add_mul_id_c_id_Rule(ctx);
            this.dpbConstantRule = new DpbConstantRule();
            this.idConst = new IdConstant(ctx, new Unifier());
            this.idCopyPropagation = new IdCopyPropagationRule(ctx);
            this.idBinIdc = new IdBinIdc_Rule(ctx);
            this.sliceConst = new SliceConstant_Rule();
            this.sliceMem = new SliceMem_Rule();
            this.negSub = new NegSub_Rule();
            this.constConstBin = new ConstConstBin_Rule();
            this.shMul = new Shl_mul_e_Rule(ctx);
            this.shiftShift = new ShiftShift_c_c_Rule(ctx);
            this.mpsRule = new Mps_Constant_Rule(ctx);
            this.sliceShift = new SliceShift(ctx);
            this.binopWithSelf = new BinOpWithSelf_Rule();
        }

        public bool Changed { get; set; }

        private bool IsAddOrSub(Operator op)
        {
            return op == Operator.IAdd || op == Operator.ISub;
        }

        private bool IsComparison(Operator op)
        {
            return op == Operator.Eq || op == Operator.Ne ||
                   op == Operator.Ge || op == Operator.Gt ||
                   op == Operator.Le || op == Operator.Lt ||
                   op == Operator.Uge || op == Operator.Ugt ||
                   op == Operator.Ule || op == Operator.Ult;
        }


        public static Constant SimplifyTwoConstants(BinaryOperator op, Constant l, Constant r)
        {
            var lType = (PrimitiveType)l.DataType;
            var rType = (PrimitiveType)r.DataType;
            if (lType.Domain != rType.Domain)
                throw new ArgumentException(string.Format("Can't add types of different domains {0} and {1}", l.DataType, r.DataType));
            return op.ApplyConstants(l, r);
        }

        public virtual Expression VisitAddress(Address addr)
        {
            return addr;
        }

        public virtual Expression VisitApplication(Application appl)
        {
            var args = new Expression[appl.Arguments.Length];
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                var arg = appl.Arguments[i];
                var outArg = arg as UnaryExpression;
                if (outArg != null && outArg.Operator == Operator.AddrOf)
                {
                    if (outArg.Expression is Identifier)
                    {
                        args[i] = arg;
                        continue;
                    }
                }
                args[i] = arg.Accept(this);
            }
            appl = new Application(appl.Procedure.Accept(this),
                appl.DataType,
                args);
            return ctx.GetValue(appl);
        }

        public virtual Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public virtual Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            // (+ id1 id1) ==> (* id1 2)

            if (add2ids.Match(binExp))
            {
                Changed = true;
                return add2ids.Transform().Accept(this);
            }
            if (binopWithSelf.Match(binExp))
            {
                Changed = true;
                return binopWithSelf.Transform(ctx).Accept(this);
            }

            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            Constant cLeft = left as Constant;
            Constant cRight = right as Constant;
            if (cLeft != null && BinaryExpression.Commutes(binExp.Operator))
            {
                cRight = cLeft; left = right; right = cLeft;
            }

            // (- X 0) ==> X
            // (+ X 0) ==> X

            if (cRight != null && cRight.IsIntegerZero && IsAddOrSub(binExp.Operator))
            {
                Changed = true;
                return left;
            }
            if (left == Constant.Invalid || right == Constant.Invalid)
                return Constant.Invalid;

            binExp = new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
            if (constConstBin.Match(binExp))
            {
                Changed = true;
                return constConstBin.Transform();
            }

            Identifier idLeft = left as Identifier;
            Identifier idRight = right as Identifier;

            // (rel? id1 c) should just pass.

            if (IsComparison(binExp.Operator) && cRight != null && idLeft != null)
                return binExp;

            var binLeft = left as BinaryExpression;
            var cLeftRight = (binLeft != null) ? binLeft.Right as Constant : null;

            // (+ (+ e c1) c2) ==> (+ e (+ c1 c2))
            // (+ (- e c1) c2) ==> (+ e (- c2 c1))
            // (- (+ e c1) c2) ==> (- e (- c2 c1))
            // (- (- e c1) c2) ==> (- e (+ c1 c2))

            if (binLeft != null && cLeftRight != null && cRight != null &&
                IsAddOrSub(binExp.Operator) && IsAddOrSub(binLeft.Operator) &&
                !cLeftRight.IsReal && !cRight.IsReal)
            {
                Changed = true;
                var binOperator = binExp.Operator;
                ctx.RemoveIdentifierUse(idLeft);
                ctx.UseExpression(left);
                Constant c;
                if (binLeft.Operator == binOperator)
                {
                    c = Operator.IAdd.ApplyConstants(cLeftRight, cRight);
                }
                else
                {
                    if (Math.Abs(cRight.ToInt64()) >= Math.Abs(cLeftRight.ToInt64()))
                    {
                        c = Operator.ISub.ApplyConstants(cRight, cLeftRight);
                    }
                    else
                    {
                        binOperator = 
                            binOperator == Operator.IAdd 
                                ? Operator.ISub 
                                : Operator.IAdd;
                        c = Operator.ISub.ApplyConstants(cLeftRight, cRight);
                    }
                }
                if (c.IsIntegerZero)
                    return binLeft.Left;
                return new BinaryExpression(binOperator, binExp.DataType, binLeft.Left, c);
            }

            // (== (- e c1) c2) => (== e c1+c2)

            if (binLeft != null && cLeftRight != null && cRight != null &&
                IsComparison(binExp.Operator) && IsAddOrSub(binLeft.Operator) &&
                !cLeftRight.IsReal && !cRight.IsReal)
            {
                Changed = true;
                ctx.RemoveIdentifierUse(idLeft);
                var op = binLeft.Operator == Operator.IAdd ? Operator.ISub : Operator.IAdd;
                var c = ExpressionSimplifierOld.SimplifyTwoConstants(op, cLeftRight, cRight);
                return new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c);
            }

            if (addMici.Match(binExp))
            {
                Changed = true;
                return addMici.Transform();
            }

            if (shMul.Match(binExp))
            {
                Changed = true;
                return shMul.Transform();
            }

            if (shiftShift.Match(binExp))
            {
                Changed = true;
                return shiftShift.Transform();
            }

            // No change, just return as is.

            return binExp;
        }

        public virtual Expression VisitCast(Cast cast)
        {
            var exp = cast.Expression.Accept(this);

            Constant c = exp as Constant;
            if (c != null)
            {
                PrimitiveType p = c.DataType as PrimitiveType;
                if (p != null && p.IsIntegral)
                {
                    //$REVIEW: this is fixed to 32 bits; need a general solution to it.
                    Changed = true;
                    return Constant.Create(cast.DataType, c.ToUInt64());
                }
            }
            return new Cast(cast.DataType, exp);
        }

        public virtual Expression VisitConditionOf(ConditionOf c)
        {
            var e = c.Expression.Accept(this);
            //$REVIEW: if e == 0, then Z flags could be set to 1. But that's architecture specific, so
            // we leave that as an exercise to re reader
            if (e != c.Expression)
                c = new ConditionOf(e);
            return c;
        }

        public virtual Expression VisitConstant(Constant c)
        {
            return c;
        }

        public virtual Expression VisitDepositBits(DepositBits d)
        {
            d.Source = d.Source.Accept(this);
            d.InsertedBits = d.InsertedBits.Accept(this);
            if (dpbConstantRule.Match(d))
            {
                Changed = true;
                return dpbConstantRule.Transform();
            }
            return d;
        }

        public virtual Expression VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public virtual Expression VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public virtual Expression VisitIdentifier(Identifier id)
        {
            if (idConst.Match(id))
            {
                Changed = true;
                return idConst.Transform();
            }
            // jkl: Copy propagation causes real problems when used during trashed register analysis.
            // If needed in other passes, it should be an option for expression e
            if (idCopyPropagation.Match(id))
            {
                Changed = true;
                return idCopyPropagation.Transform();
            }
            if (idBinIdc.Match(id))
            {
                Changed = true;
                return idBinIdc.Transform();
            }
            return id;
        }

        public virtual Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            if (mpsRule.Match(mps))
            {
                Changed = true;
                return mpsRule.Transform();
            }
            return mps;
        }

        public virtual Expression VisitMemoryAccess(MemoryAccess access)
        {
            var value = new MemoryAccess(
                access.MemoryId,
                access.EffectiveAddress.Accept(this),
                access.DataType);
            return ctx.GetValue(value);
        }

        public virtual Expression VisitMkSequence(MkSequence seq)
        {
            var head = seq.Head.Accept(this);
            var tail = seq.Tail.Accept(this);
            Constant c1 = seq.Head as Constant;
            Constant c2 = seq.Tail as Constant;
            if (c1 != null && c2 != null)
            {
                PrimitiveType tHead = (PrimitiveType)c1.DataType;
                PrimitiveType tTail = (PrimitiveType)c2.DataType;
                PrimitiveType t;
                Changed = true;
                if (tHead.Domain == Domain.Selector)			//$REVIEW: seems to require Address, SegmentedAddress?
                {
                    t = PrimitiveType.Create(Domain.Pointer, tHead.Size + tTail.Size);
                    return new Address(c1.ToUInt16(), c2.ToUInt16());
                }
                else
                {
                    t = PrimitiveType.Create(tHead.Domain, tHead.Size + tTail.Size);
                    return Constant.Create(t, c1.ToInt32() << tHead.BitSize | c2.ToInt32());
                }
            }
            return new MkSequence(seq.DataType, head, tail);
        }

        public virtual Expression VisitPhiFunction(PhiFunction pc)
        {
            return pc;
        }

        public virtual Expression VisitPointerAddition(PointerAddition pa)
        {
            return pa;
        }

        public virtual Expression VisitProcedureConstant(ProcedureConstant pc)
        {
            return pc;
        }

        public virtual Expression VisitScopeResolution(ScopeResolution sc)
        {
            return sc;
        }

        public virtual Expression VisitSegmentedAccess(SegmentedAccess segMem)
        {
            return ctx.GetValue(new SegmentedAccess(
                segMem.MemoryId,
                segMem.BasePointer.Accept(this),
                segMem.EffectiveAddress.Accept(this),
                segMem.DataType));
        }

        public virtual Expression VisitSlice(Slice slice)
        {
            slice.Expression = slice.Expression.Accept(this);
            if (sliceConst.Match(slice))
            {
                Changed = true;
                return sliceConst.Transform();
            }
            if (sliceMem.Match(slice))
            {
                Changed = true;
                return sliceMem.Transform();
            }

            // (slice (shl e n) n) ==> e
            if (sliceShift.Match(slice))
            {
                Changed = true;
                return sliceShift.Transform();
            }
            return slice;
        }

        public virtual Expression VisitTestCondition(TestCondition tc)
        {
            return new TestCondition(tc.ConditionCode, tc.Expression.Accept(this));
        }

        public virtual Expression VisitUnaryExpression(UnaryExpression unary)
        {
            unary = new UnaryExpression(unary.Operator, unary.DataType, unary.Expression.Accept(this));
            if (negSub.Match(unary))
            {
                Changed = true;
                return negSub.Transform();
            }
            return unary;
        }
    }
}

namespace Decompiler.Evaluation
{
    using Decompiler.Analysis;

    #region OLD_SIMPLIFIER
    /// <summary>
	/// Simplifies expressions by using common algebraic tricks and 
	/// other well known formulae.
	/// </summary>
	public class ExpressionSimplifierOld : ExpressionVisitor<Expression>
	{
		private Decompiler.Analysis.ValueNumbering dad;
		private Dictionary<Expression,Expression> table;

        public ExpressionSimplifierOld(Decompiler.Analysis.ValueNumbering d, Dictionary<Expression, Expression> table)
		{
			this.dad = d;
			this.table = table;
		}

		private Expression AlgebraicSimplification(
			Operator binOp, 
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
					if (binOp != Operator.Eq)
						return SimplifyTwoConstants(binOp, cLeft, cRight);
				}
			}

			// C op id should be transformed to id op C, but only if op commutes.

			if (cLeft != null && BinaryExpression.Commutes(binOp))
			{
				Expression tmp = left; left = right; right = tmp;
			}

			//$REVIEW: identity on binaryoperators
            // DO NOT simplify floating-point ops!
			if (binOp == Operator.IAdd)
			{
				if (cRight.IsZero)
					return left;
			} 
			else if (binOp == Operator.ISub)
			{
				if (left == right)
					return MakeZero(left.DataType);
				if (IsZero(cRight))
					return left;
			} 
			else if (binOp == Operator.Or || binOp == Operator.Xor)
			{
				if (IsZero(cRight))
					return left;
			} 
            else if (binOp == Operator.And)
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
				if (cLeftRight != null && cRight != null && binLeft.Operator == Operator.IAdd && binOp == Operator.IAdd)
				{
					return new BinaryExpression(binOp, valType, binLeft.Left, SimplifyTwoConstants(binOp, cLeftRight, cRight));
				}
			}

			return new BinaryExpression(binOp, valType, left, right);
		}

		private Constant MakeZero(DataType type)
		{
			return Constant.Create(type, 0);
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

        public Expression VisitAddress(Address addr)
        {
            throw new NotImplementedException();
        }

		public Expression VisitApplication(Application app)
		{
			return app;
		}

		public Expression VisitArrayAccess(ArrayAccess acc)
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
		public virtual Expression VisitBinaryExpression(BinaryExpression bin)
		{
			Expression simpleLeft = bin.Left.Accept(this);
			Expression simpleRight = bin.Right.Accept(this);

			return AlgebraicSimplification(bin.Operator, bin.DataType, simpleLeft, simpleRight);
		}

		public Expression VisitCast(Cast cast)
		{
			cast.Expression.Accept(this);
			Constant c = cast.Expression as Constant;
			if (c != null)
			{
				PrimitiveType p = c.DataType as PrimitiveType;
				if (p != null && p.IsIntegral)
				{
					return Constant.Create(cast.DataType, c.ToInt32());
				}
			}
			return cast;
		}

		public Expression VisitConditionOf(ConditionOf cc)
		{
			cc.Expression.Accept(this);
			return cc;
		}

		public Expression VisitConstant(Constant c)
		{
			return c;
		}

		public Expression VisitDepositBits(DepositBits d)
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

		public Expression VisitDereference(Dereference deref)
		{
			deref.Expression = deref.Expression.Accept(this);
			return deref;
		}

		public Expression VisitFieldAccess(FieldAccess acc)
		{
			acc.Structure = acc.Structure.Accept(this);
			return acc;
		}

		public Expression VisitPointerAddition(PointerAddition pa)
		{
			return new PointerAddition(pa.DataType, pa.Pointer.Accept(this), pa.Offset);
		}

		public Expression VisitProcedureConstant(ProcedureConstant pc)
		{
			return pc;
		}

		public Expression VisitIdentifier(Identifier id)
		{
			if (id.Number >= 0)
				return dad.GetValueNumber(id);
			else
				return id;
		}

		public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			Expression ptr = mps.BasePointer.Accept(this);
			Expression memberPtr = mps.MemberPointer.Accept(this);
			return new MemberPointerSelector(mps.DataType, ptr, memberPtr);
		}

		public Expression VisitMemoryAccess(MemoryAccess access)
		{
			Expression simpleExpr = access.EffectiveAddress.Accept(this);
			return new MemoryAccess(access.MemoryId, simpleExpr, access.DataType);
		}

		public Expression VisitMkSequence(MkSequence seq)
		{
			Expression head = seq.Head.Accept(this);
			Expression tail = seq.Tail.Accept(this);
			return new MkSequence(seq.DataType, head, tail);
		}

		public Expression VisitScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

		public Expression VisitSegmentedAccess(SegmentedAccess access)
		{
			Expression b = access.BasePointer.Accept(this);
			Expression ea = access.EffectiveAddress.Accept(this);
			return new SegmentedAccess(access.MemoryId, b, ea, access.DataType);
		}

		public Expression VisitPhiFunction(PhiFunction phi)
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
			
		public Expression VisitTestCondition(TestCondition tc)
		{
			return new TestCondition(tc.ConditionCode, tc.Expression.Accept(this));
		}

		public Expression VisitSlice(Slice slice)
		{
			return new Slice(slice.DataType, slice.Expression.Accept(this), (uint) slice.Offset);
		}

		public Expression VisitUnaryExpression(UnaryExpression unary)
		{
			if (unary.Operator == Operator.AddrOf)
				return unary;
			Expression u = unary.Expression.Accept(this);
			if (u is ValueNumbering.AnyValueNumber)
				return unary;
			return new UnaryExpression(unary.Operator, unary.DataType, u);
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

        [Obsolete("Use Expression.IsZero")]
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

		public static Constant SimplifyTwoConstants(Operator op, Constant l, Constant r)
		{
			PrimitiveType lType = (PrimitiveType) l.DataType;
			PrimitiveType rType = (PrimitiveType) r.DataType;
			if (lType.Domain != rType.Domain)
				throw new ArgumentException(string.Format("Can't add types of different domains {0} and {1}", l.DataType, r.DataType));
			return ((BinaryOperator)op).ApplyConstants(l, r);
		}
    }
    #endregion
}
