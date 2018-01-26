#region License
/* 
 * Copyright (C) 1999-2018 John K�ll�n.
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

using Reko.Evaluation;
using Reko.Core;
using Reko.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Reko.Core.Services;

namespace Reko.Evaluation 
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
        private SliceSegmentedPointer_Rule sliceSegPtr;
        private SliceShift sliceShift;
        private Shl_add_Rule shAdd;
        private Shl_mul_e_Rule shMul;
        private ShiftShift_c_c_Rule shiftShift;
        private NegSub_Rule negSub;
        private Mps_Constant_Rule mpsRule;
        private BinOpWithSelf_Rule binopWithSelf;
        private ConstDivisionImplementedByMultiplication constDiv;
        private SelfDpbRule selfdpbRule;
        private IdProcConstRule idProcConstRule;
        private CastCastRule castCastRule;

        public ExpressionSimplifier(EvaluationContext ctx, DecompilerEventListener listener)
        {
            this.ctx = ctx;

            this.add2ids = new AddTwoIdsRule(ctx);
            this.addEcc = new Add_e_c_cRule(ctx);
            this.addMici = new Add_mul_id_c_id_Rule(ctx);
            this.dpbConstantRule = new DpbConstantRule();
            this.dpbdpbRule = new DpbDpbRule(ctx);
            this.idConst = new IdConstant(ctx, new Unifier(), listener);
            this.idCopyPropagation = new IdCopyPropagationRule(ctx);
            this.idBinIdc = new IdBinIdc_Rule(ctx);
            this.sliceConst = new SliceConstant_Rule();
            this.sliceMem = new SliceMem_Rule();
            this.sliceSegPtr = new SliceSegmentedPointer_Rule(ctx);
            this.negSub = new NegSub_Rule();
            this.constConstBin = new ConstConstBin_Rule();
            this.shAdd = new Shl_add_Rule(ctx);
            this.shMul = new Shl_mul_e_Rule(ctx);
            this.shiftShift = new ShiftShift_c_c_Rule(ctx);
            this.mpsRule = new Mps_Constant_Rule(ctx);
            this.sliceShift = new SliceShift(ctx);
            this.binopWithSelf = new BinOpWithSelf_Rule();
            this.constDiv = new ConstDivisionImplementedByMultiplication(ctx);
            this.selfdpbRule = new SelfDpbRule(ctx);
            this.idProcConstRule = new IdProcConstRule(ctx);
            this.castCastRule = new CastCastRule(ctx);
        }

        public bool Changed { get { return changed; } set { changed = value; } }
        private bool changed;

        private bool IsAddOrSub(Operator op)
        {
            return op == Operator.IAdd || op == Operator.ISub;
        }

        private bool IsIntComparison(Operator op)
        {
            return op == Operator.Eq || op == Operator.Ne ||
                   op == Operator.Ge || op == Operator.Gt ||
                   op == Operator.Le || op == Operator.Lt ||
                   op == Operator.Uge || op == Operator.Ugt ||
                   op == Operator.Ule || op == Operator.Ult;
        }

        private bool IsFloatComparison(Operator op)
        {
            return op == Operator.Feq || op == Operator.Fne ||
                   op == Operator.Fge || op == Operator.Fgt ||
                   op == Operator.Fle || op == Operator.Flt;
        }

        public static Constant SimplifyTwoConstants(BinaryOperator op, Constant l, Constant r)
        {
            var lType = (PrimitiveType)l.DataType;
            var rType = (PrimitiveType)r.DataType;
            if ((lType.Domain & rType.Domain) == 0)
                throw new ArgumentException(string.Format("Can't add types of disjoint domains {0} and {1}", l.DataType, r.DataType));
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
            else if (cRight != null && cRight.IsIntegerZero && binExp.Operator == Operator.Or)
            {
                Changed = true;
                return left;
            }
            //$REVIEW: this is evaluation! Shouldn't the be done by the evaluator?
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

            if (IsIntComparison(binExp.Operator) && cRight != null && idLeft != null)
                return binExp;

            // Floating point expressions with "integer" constants 
            if (IsFloatComparison(binExp.Operator) && IsNonFloatConstant(cRight))
            {
                cRight = ReinterpretAsIeeeFloat(cRight);
                right = cRight;
                binExp.Right = cRight;
            }

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

            // (rel (- e c1) c2) => (rel e c1+c2)

            if (binLeft != null && cLeftRight != null && cRight != null &&
                IsIntComparison(binExp.Operator) && IsAddOrSub(binLeft.Operator) &&
                !cLeftRight.IsReal && !cRight.IsReal)
            {
                // (>u (- e c1) c2) => (>u e c1+c2) || (<u e c2)
                if (binExp.Operator == Operator.Ugt && 
                    binLeft.Operator == Operator.ISub &&
                    !cRight.IsIntegerZero)
                {
                    Changed = true;
                    ctx.UseExpression(binLeft.Left);
                    var c = ExpressionSimplifier.SimplifyTwoConstants(Operator.IAdd, cLeftRight, cRight);
                    return new BinaryExpression(Operator.Cor, PrimitiveType.Bool,
                        new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c),
                        new BinaryExpression(Operator.Ult, PrimitiveType.Bool, binLeft.Left, cLeftRight));
                }
                else
                {
                    Changed = true;
                    ctx.RemoveIdentifierUse(idLeft);
                    var op = binLeft.Operator == Operator.IAdd ? Operator.ISub : Operator.IAdd;
                    var c = ExpressionSimplifier.SimplifyTwoConstants(op, cLeftRight, cRight);
                    return new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c);
                }
            }

            if (addMici.Match(binExp))
            {
                Changed = true;
                return addMici.Transform();
            }

            if (shAdd.Match(binExp))
            {
                Changed = true;
                return shAdd.Transform();
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

        private Constant ReinterpretAsIeeeFloat(Constant c)
        {
            if (c.DataType.Size == 4)
            {
                return Constant.FloatFromBitpattern(c.ToInt32());
            }
            else if (c.DataType.Size == 8)
            {
                return Constant.FloatFromBitpattern(c.ToInt64());
            }
            throw new NotImplementedException(string.Format(
                "Unsupported IEEE floating point size {0}.",
                c.DataType.Size));
        }

        private bool IsNonFloatConstant(Constant cRight)
        {
            if (cRight == null)
                return false;
            var pt = cRight.DataType as PrimitiveType;
            return (pt.Domain != Domain.Real);
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
            if (exp != Constant.Invalid)
            {
                var ptCast = cast.DataType.ResolveAs<PrimitiveType>();
                if (exp is Constant c && ptCast != null)
                {
                    if (c.DataType is PrimitiveType ptSrc)
                    {
                        if (ptCast.Domain == Domain.Real)
                        {
                            if (ptSrc.Domain == Domain.Real &&
                                ptCast.Size < ptSrc.Size)
                            {
                                Changed = true;
                                return ConstantReal.Create(ptCast, c.ToReal64());
                            }
                        }
                        else if ((ptSrc.Domain & Domain.Integer) != 0)
                        {
                            Changed = true;
                            return Constant.Create(ptCast, c.ToUInt64());
                        }
                    }
                }
                if (exp is Identifier id && 
                    ctx.GetDefiningExpression(id) is DepositBits dpb && 
                    dpb.BitPosition == 0)
                {
                    // If we are casting the result of a DPB, and the deposited part is >= 
                    // the size of the cast, then use deposited part directly.
                    int sizeDiff = dpb.InsertedBits.DataType.Size - cast.DataType.Size;
                    if (sizeDiff >= 0)
                    {
                        ctx.RemoveIdentifierUse(id);
                        ctx.UseExpression(dpb.InsertedBits);
                        Changed = true;
                        if (sizeDiff > 0)
                        {
                            return new Cast(cast.DataType, dpb.InsertedBits);
                        }
                        else
                        {
                            return dpb.InsertedBits;
                        }
                    }
                }
                cast = new Cast(cast.DataType, exp);
            }
            if (castCastRule.Match(cast))
            {
                Changed = true;
                return castCastRule.Transform();
            }
            return cast;
        }

        public virtual Expression VisitConditionalExpression(ConditionalExpression c)
        {
            var cond = c.Condition.Accept(this);
            var t = c.ThenExp.Accept(this);
            var f = c.FalseExp.Accept(this);
            if (cond is Constant cCond && cCond.DataType == PrimitiveType.Bool)
            {
                if (cCond.IsZero)
                    return f;
                else
                    return t;
            }
            return new ConditionalExpression(c.DataType, cond, t, f);
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
            var src = d.Source.Accept(this);
            var bits = d.InsertedBits.Accept(this);
            if (src == Constant.Invalid || bits == Constant.Invalid)
            {
                return Constant.Invalid;
            }
            d = new DepositBits(src, bits, d.BitPosition);
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
            if (selfdpbRule.Match(d))
            {
                Changed = true;
                return selfdpbRule.Transform();
            }
            return d;
        }

        public virtual Expression VisitDereference(Dereference deref)
        {
            var e = deref.Expression.Accept(this);
            return new Dereference(deref.DataType, e);
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
            if (idProcConstRule.Match(id))
            {
                Changed = true;
                return idProcConstRule.Transform();
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
                    return ctx.MakeSegmentedAddress(c1, c2);
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
            if (c1 != null && c1.IsZero)
            {
                // leading zeros imply a conversion to unsigned.
                return new Cast(
                    PrimitiveType.Create(Domain.UnsignedInt, seq.DataType.Size),
                    new Cast(
                        PrimitiveType.Create(Domain.UnsignedInt, tail.DataType.Size),
                        tail));

            }
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
            var oldChanged = Changed;
            var args = pc.Arguments
                .Select(a =>
                {
                    var arg = SimplifyPhiArg(a.Accept(this));
                    ctx.RemoveExpressionUse(arg);
                    return arg;
                })
                .Where(a => ctx.GetValue(a as Identifier) != pc)
                .ToArray();
            Changed = oldChanged;

            var cmp = new ExpressionValueComparer();
            var e = args.FirstOrDefault();
            if (e != null && args.All(a => cmp.Equals(a, e)))
            {
                Changed = true;
                ctx.UseExpression(e);
                return e;
            }
            else
            {
                ctx.UseExpression(pc);
                return pc;
            }
        }

        /// <summary>
        /// VisitBinaryExpression method could not simplify following statements:
        ///    y = x - const
        ///    a = y + const
        ///    x = phi(a, b)
        /// to
        ///    y = x - const
        ///    a = x
        ///    x = phi(a, b)
        /// IdBinIdc rule class processes y as 'used in phi' and prevents propagation.
        /// This method could be used to do such simplification (y + const ==> x)
        /// </summary
        private Expression SimplifyPhiArg(Expression arg)
        {
            if (!(arg is BinaryExpression bin &&
                  bin.Left is Identifier idLeft &&
                  ctx.GetValue(idLeft) is BinaryExpression binLeft))
                return arg;

            ctx.RemoveIdentifierUse(idLeft);
            ctx.UseExpression(binLeft);
            bin = new BinaryExpression(
                bin.Operator,
                bin.DataType,
                binLeft,
                bin.Right);
            return bin.Accept(this);
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
            segMem =
                new SegmentedAccess(
                segMem.MemoryId,
                segMem.BasePointer.Accept(this),
                segMem.EffectiveAddress.Accept(this),
                segMem.DataType);
            if (sliceSegPtr.Match(segMem))
            {
                Changed = true;
                return sliceSegPtr.Transform();
            }
            return ctx.GetValue(segMem);
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
