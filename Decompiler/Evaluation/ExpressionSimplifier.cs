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

using Decompiler.Evaluation;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private DpbDpbRule dpbdpbRule;
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
            this.dpbdpbRule = new DpbDpbRule(ctx);
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

        public bool Changed { get { return changed; } set { changed = value; } }
        private bool changed;

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
                args[i] = arg.Accept(this);
            }
            appl = new Application(appl.Procedure.Accept(this),
                appl.DataType,
                args);
            return ctx.GetValue(appl);
        }

        public virtual Expression VisitArrayAccess(ArrayAccess acc)
        {
            return new ArrayAccess(
                acc.DataType,
                acc.Array.Accept(this),
                acc.Index.Accept(this));
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
                var c = ExpressionSimplifier.SimplifyTwoConstants(op, cLeftRight, cRight);
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

        public static Constant SimplifyTwoConstants(Operator op, Constant l, Constant r)
        {
            PrimitiveType lType = (PrimitiveType) l.DataType;
            PrimitiveType rType = (PrimitiveType) r.DataType;
            if ((lType.Domain & rType.Domain) == 0)
                throw new ArgumentException(string.Format("Can't add types of different domains {0} and {1}", l.DataType, r.DataType));
            return ((BinaryOperator) op).ApplyConstants(l, r);
        }

        public virtual Expression VisitCast(Cast cast)
        {
            var exp = cast.Expression.Accept(this);
            if (exp == Constant.Invalid)
                return exp;

            Constant c = exp as Constant;
            if (c != null)
            {
                PrimitiveType p = c.DataType as PrimitiveType;
                if (p != null && (p.Domain & Domain.Integer) != 0)
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
            if (dpbdpbRule.Match(d))
            {
                Changed = true;
                return dpbdpbRule.Transform();
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
                    return Address.SegPtr(c1.ToUInt16(), c2.ToUInt16());
                }
                else
                {
                    t = PrimitiveType.Create(tHead.Domain, tHead.Size + tTail.Size);
                    return Constant.Create(t, c1.ToInt32() << tHead.BitSize | c2.ToInt32());
                }
            }
            if (head == Constant.Invalid)
                head = seq.Head;
            if (tail == Constant.Invalid)
                tail = seq.Tail;
            return new MkSequence(seq.DataType, head, tail);
        }

        public virtual Expression VisitOutArgument(OutArgument outArg)
        {
            Expression exp;
            if (outArg.Expression is Identifier)
                exp = outArg.Expression;
            else 
                exp = outArg.Expression.Accept(this);
            return new OutArgument(outArg.DataType, exp);
        }

        public virtual Expression VisitPhiFunction(PhiFunction pc)
        {
            return pc;
            /*
            var oldChanged = Changed;
            var args = pc.Arguments
                .Select(a => a.Accept(this))
                .ToArray();
            Changed = oldChanged;
            Expression e = args[0];
            if (args.All(a => new ExpressionValueComparer().Equals(a, e)))
            {
                Changed = true;
                return e;
            }
            else
            {
                return pc;
            }*/
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
