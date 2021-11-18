#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Diagnostics;
using System.Xml.Schema;
using Reko.Core.Code;
using Reko.Scanning;

namespace Reko.Evaluation 
{
    /// <summary>
    /// Partially evaluates expressions, using an <see cref="EvaluationContext"/> to obtain the values
    /// of identifiers and optionally modifies the expression being evaluated.
    /// </summary>
    public class ExpressionSimplifier : ExpressionVisitor<(Expression, bool)>
    {
        private readonly SegmentMap segmentMap;
        private EvaluationContext ctx;
        private readonly ExpressionValueComparer cmp;
        private readonly ExpressionEmitter m;
        private readonly Unifier unifier;

        private readonly AddTwoIdsRule add2ids;
        private readonly Add_e_c_cRule addEcc;
        private readonly Add_mul_id_c_id_Rule addMici;
        private readonly ConstConstBin_Rule constConstBin;
        private readonly IdConstant idConst;
        private readonly IdCopyPropagationRule idCopyPropagation;
        private readonly IdBinIdc_Rule idBinIdc;
        private readonly SliceConstant_Rule sliceConst;
        private readonly SliceMem_Rule sliceMem;
        private readonly SliceSegmentedPointer_Rule sliceSegPtr;
        private readonly SliceShift sliceShift;
        private readonly Shl_add_Rule shAdd;
        private readonly Shl_mul_e_Rule shMul;
        private readonly ShiftShift_c_c_Rule shiftShift;
        private readonly NegSub_Rule negSub;
        private readonly Mps_Constant_Rule mpsRule;
        private readonly BinOpWithSelf_Rule binopWithSelf;
        private readonly ConstDivisionImplementedByMultiplication constDiv;
        private readonly IdProcConstRule idProcConstRule;
        private readonly ConvertConvertRule convertConvertRule;
        private readonly DistributedCastRule distributedCast;
        private readonly DistributedConversionRule distributedConvert;
        private readonly DistributedSliceRule distributedSlice;
        private readonly MkSeqFromSlices_Rule mkSeqFromSlicesRule;
        private readonly ComparisonConstOnLeft constOnLeft;
        private readonly SliceSequence sliceSeq;
        private readonly SliceConvert sliceConvert;
        private readonly LogicalNotFollowedByNegRule logicalNotFollowedByNeg;
        private readonly LogicalNotFromArithmeticSequenceRule logicalNotFromBorrow;
        private readonly UnaryNegEqZeroRule unaryNegEqZero;
        private readonly ScaledIndexRule scaledIndexRule;

        public ExpressionSimplifier(SegmentMap segmentMap, EvaluationContext ctx, DecompilerEventListener listener)
        {
            this.segmentMap = segmentMap ?? throw new ArgumentNullException(nameof(SegmentMap));
            this.ctx = ctx;
            this.cmp = new ExpressionValueComparer();
            this.m = new ExpressionEmitter();
            this.unifier = new Unifier();
            this.add2ids = new AddTwoIdsRule(ctx);
            this.addEcc = new Add_e_c_cRule(ctx);
            this.addMici = new Add_mul_id_c_id_Rule(ctx);
            this.idConst = new IdConstant(ctx, unifier, listener);
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
            this.idProcConstRule = new IdProcConstRule(ctx);
            this.convertConvertRule = new ConvertConvertRule(ctx);
            this.distributedConvert = new DistributedConversionRule();
            this.distributedCast = new DistributedCastRule();
            this.distributedSlice = new DistributedSliceRule();
            this.mkSeqFromSlicesRule = new MkSeqFromSlices_Rule(ctx);
            this.constOnLeft = new ComparisonConstOnLeft();
            this.sliceSeq = new SliceSequence(ctx);
            this.sliceConvert = new SliceConvert();
            this.logicalNotFollowedByNeg = new LogicalNotFollowedByNegRule();
            this.logicalNotFromBorrow = new LogicalNotFromArithmeticSequenceRule();
            this.unaryNegEqZero = new UnaryNegEqZeroRule();
            this.scaledIndexRule = new ScaledIndexRule(ctx);
        }

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
            return op.ApplyConstants(l, r);
        }

        public virtual (Expression, bool) VisitAddress(Address addr)
        {
            return (addr, false);
        }

        public virtual (Expression, bool) VisitApplication(Application appl)
        {
            bool changed = false;
            var args = new Expression[appl.Arguments.Length];
            for (int i = 0; i < appl.Arguments.Length; ++i)
            {
                var (arg, argChanged) = appl.Arguments[i].Accept(this);
                args[i] = arg;
                changed |= argChanged;
            }
            // Rotations-with-carries that rotate in a false carry 
            // flag can be simplified to shifts.
            if (appl.Procedure is ProcedureConstant pc &&
                pc.Procedure is IntrinsicProcedure intrinsic)
            {
                switch (intrinsic.Name)
                {
                case IntrinsicProcedure.RolC:
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (new BinaryExpression(Operator.Shl, appl.DataType, args[0], args[1]), true);
                    }
                    break;
                case IntrinsicProcedure.RorC:
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (new BinaryExpression(Operator.Shr, appl.DataType, args[0], args[1]), true);
                    }
                    break;
                case IntrinsicProcedure.Rol:
                    var rol = CombineRotations(intrinsic.Name, appl, args);
                    if (rol != null)
                    {
                        return (rol, true);
                    }
                    break;
                case IntrinsicProcedure.Ror:
                    var ror = CombineRotations(intrinsic.Name, appl, args);
                    if (ror != null)
                    {
                        return (ror, true);
                    }
                    break;
                }
            }
            var (proc, procChanged) = appl.Procedure.Accept(this);
            changed |= procChanged;
            if (changed)
            {
                appl = new Application(
                    proc,
                    appl.DataType,
                    args);
            }
            var newAppl = ctx.GetValue(appl);
            if (newAppl != appl)
            {
                return (newAppl, true);
            }
            else
            {
                return (appl, changed);
            }
        }

        private Expression? CombineRotations(string rotationName, Application appl, Expression[] args)
        {
            if (args[1] is Constant cOuter &&
                args[0] is Application appInner &&
                appInner.Procedure is ProcedureConstant pcInner &&
                pcInner.Procedure is IntrinsicProcedure intrinsicInner)
            {
                if (intrinsicInner.Name == rotationName)
                {
                    if (appInner.Arguments[1] is Constant cInner)
                    {
                        var cTot = Operator.IAdd.ApplyConstants(cOuter, cInner);
                        return new Application(
                            appl.Procedure,
                            appl.DataType,
                            appInner.Arguments[0],
                            cTot);
                    }
                }
            }
            return null;
        }

        private static bool IsSingleBitRotationWithClearCarryIn(Expression[] args)
        {
            Debug.Assert(args.Length == 3);
            return args[1] is Constant sh &&
                                    sh.ToInt32() == 1 &&
                                    args[2] is Constant c &&
                                    c.IsIntegerZero;
        }

        public virtual (Expression, bool) VisitArrayAccess(ArrayAccess acc)
        {
            var (a, aChanged) = acc.Array.Accept(this);
            var (i, iChanged) = acc.Index.Accept(this);
            if (aChanged || iChanged)
            {
                return (new ArrayAccess(acc.DataType, a, i), true);
            }
            else
            {
                return (acc, false);
            }
        }

        public virtual (Expression, bool) VisitBinaryExpression(BinaryExpression binExp)
        {
            // BinaryExpressions are the most common and occur in clusters that sometimes
            // are so deep that attempting to simplify using recursion. The code below
            // traverses a tree of BinaryExpressions iteratively, using an explicit stack
            // to keep track of intermediate results.
            var stack = new Stack<(BinaryExpression, (Expression,bool)[])>();
            stack.Push((binExp, new (Expression,bool)[2]));
            (Expression, bool) result = default!;
            while (stack.Count > 0)
            {
                var (subBin, child) = stack.Pop();
                if (child[0].Item1 is null)
                {
                    result = PreVisitBinaryExpression(subBin);
                    if (result.Item2)
                    {
                        if (stack.TryPeek(out var parent))
                        {
                            if (parent.Item2[0].Item1 is null)
                                parent.Item2[0] = result;
                            else
                                parent.Item2[1] = result;
                        }
                    }
                    else
                    {
                        if (subBin.Left is BinaryExpression subLeft)
                        {
                            stack.Push((subBin, child));
                            stack.Push((subLeft, new (Expression, bool)[2]));
                            continue;
                        }
                        child[0] = subBin.Left.Accept(this);
                        stack.Push((subBin, child));
                    }
                }
                else if (child[1].Item1 is null)
                {
                    if (subBin.Right is BinaryExpression subRight)
                    {
                        stack.Push((subBin, child));
                        stack.Push((subRight, new (Expression, bool)[2]));
                        continue;
                    }
                    child[1] = subBin.Right.Accept(this);
                    stack.Push((subBin, child));
                }
                else
                {
                    result = PostVisitBinaryExpression(subBin, child);
                    if (stack.TryPeek(out var parent))
                    {
                        if (parent.Item2[0].Item1 is null)
                            parent.Item2[0] = result;
                        else
                            parent.Item2[1] = result;
                    }
                }
            }
            return result!;
        }

        private (Expression, bool) PostVisitBinaryExpression(
            BinaryExpression binExp, 
            (Expression, bool) [] simpExps) 
        {
            var (left, lChanged) = simpExps[0];
            var (right, rChanged) = simpExps[1];
            bool changed = lChanged | rChanged;
            Constant? cLeft = left as Constant;
            Constant? cRight = right as Constant;
            if (cLeft != null && BinaryExpression.Commutes(binExp.Operator))
            {
                cRight = cLeft; left = right; right = cLeft;
            }

            //$TODO: operands to binary operations appear to be
            // mismatched in some processors. Change the ctor
            // of BinaryExpression to catch this later.
            var sameBitsize = left.DataType.BitSize == right.DataType.BitSize;
            if (cRight != null)
            {
                // (- X 0) ==> X
                // (+ X 0) ==> X
                if (cRight.IsIntegerZero && IsAddOrSub(binExp.Operator))
                {
                    return (left, true);
                }
                if (binExp.Operator == Operator.Or)
                {
                    if (cRight.IsIntegerZero)
                    {
                        return (left, true);
                    }
                    // (| X 0xFFFF...F) ==> 0xFFFF...F
                    if (cRight.IsMaxUnsigned && sameBitsize && !CriticalInstruction.IsCritical(left))
                    {
                        ctx.RemoveExpressionUse(left);
                        return (right, true);
                    }
                }
                if (binExp.Operator == Operator.And)
                {
                    if (cRight.IsIntegerZero && sameBitsize && !CriticalInstruction.IsCritical(left))
                    {
                        ctx.RemoveExpressionUse(left);
                        return (cRight, true);
                    }
                    if (cRight.IsMaxUnsigned && sameBitsize)
                    {
                        return (left, true);
                    }
                }
                if (binExp.Operator == Operator.Xor)
                {
                    if (cRight.IsIntegerZero)
                    {
                        return (left, true);
                    }
                    if (cRight.IsMaxUnsigned && sameBitsize)
                    {
                        var (e, _) = new UnaryExpression(Operator.Comp, left.DataType, left).Accept(this);
                        return (e, true);
                    }
                }
            }

            //$REVIEW: this is evaluation! Shouldn't the be done by the evaluator?
            if (left is InvalidConstant || right is InvalidConstant)
                return (InvalidConstant.Create(binExp.DataType), lChanged | rChanged);

            binExp = new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
            if (constConstBin.Match(binExp))
            {
                return (constConstBin.Transform(), true);
            }

            Identifier? idLeft = left as Identifier;
            Identifier? idRight = right as Identifier;

            // (rel? id1 c) should just pass.

            if (IsIntComparison(binExp.Operator) && cRight != null && idLeft != null)
                return (binExp!, changed);

            // Floating point expressions with "integer" constants 
            if (IsFloatComparison(binExp.Operator) && IsNonFloatConstant(cRight))
            {
                cRight = ctx.ReinterpretAsFloat(cRight!);
                right = cRight;
                changed = true;
                binExp = new BinaryExpression(
                    binExp.Operator,
                    binExp.DataType,
                    binExp.Left,
                    cRight);
            }

            var binLeft = left as BinaryExpression;
            var cLeftRight = (binLeft != null) ? binLeft.Right as Constant : null;

            // (+ (+ e c1) c2) ==> (+ e (+ c1 c2))
            // (+ (- e c1) c2) ==> (+ e (- c2 c1))
            // (- (+ e c1) c2) ==> (- e (- c2 c1))
            // (- (- e c1) c2) ==> (- e (+ c1 c2))

            if (binLeft != null && cLeftRight != null && cRight != null)
            {
                if (IsAddOrSub(binExp.Operator) && IsAddOrSub(binLeft.Operator) &&
                !cLeftRight.IsReal && !cRight.IsReal)
                {
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
                        return (binLeft.Left, true);
                    return (new BinaryExpression(binOperator, binExp.DataType, binLeft.Left, c), true);
                }
                if (binExp.Operator == Operator.IMul && binLeft.Operator == Operator.IMul)
                {
                    var c = Operator.IMul.ApplyConstants(cLeftRight, cRight);
                    if (c.IsIntegerZero)
                        return (c, true);
                    else
                        return (new BinaryExpression(binExp.Operator, binExp.DataType, binLeft.Left, c), true);
                }
            }

            // (rel (- c e) 0 => (rel -c e) => (rel.Negate e c)

            if (binLeft != null && cRight != null && cRight.IsIntegerZero &&
                IsIntComparison(binExp.Operator) &&
                binLeft.Left is Constant cBinLeft &&
                binLeft.Operator == Operator.ISub)
            {
                return (new BinaryExpression(
                    ((ConditionalOperator) binExp.Operator).Negate(),
                    binExp.DataType,
                    binLeft.Right,
                    cBinLeft), true);
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
                    ctx.UseExpression(binLeft.Left);
                    var c = ExpressionSimplifier.SimplifyTwoConstants(Operator.IAdd, cLeftRight, cRight);
                    return (m.Cor(
                        new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c),
                        new BinaryExpression(Operator.Ult, PrimitiveType.Bool, binLeft.Left, cLeftRight)),
                        true);
                }
                else
                {
                    ctx.RemoveIdentifierUse(idLeft!);
                    var op = binLeft.Operator == Operator.IAdd ? Operator.ISub : Operator.IAdd;
                    var c = ExpressionSimplifier.SimplifyTwoConstants(op, cLeftRight, cRight);
                    return (new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c), true);
                }
            }

            // (rel C non-C) => (trans(rel) non-C C)
            if (constOnLeft.Match(binExp))
            {
                var (e, _) = constOnLeft.Transform().Accept(this);
                return (e, true);
            }
            if (addMici.Match(binExp))
            {
                return (addMici.Transform(), true);
            }

            if (shAdd.Match(binExp))
            {
                return (shAdd.Transform(), true);
            }

            if (shMul.Match(binExp))
            {
                return (shMul.Transform(), true);
            }

            if (shiftShift.Match(binExp))
            {
                return (shiftShift.Transform(), true);
            }

            var eNew = ShiftLeftShiftRight(binExp, cRight);
            if (eNew != null)
            {
                return (eNew, true);
            }

            // (-exp == 0) => (exp == 0)
            if (unaryNegEqZero.Match(binExp))
            {
                return (unaryNegEqZero.Transform(), true);
            }

            if (logicalNotFromBorrow.Match(binExp))
            {
                return (logicalNotFromBorrow.Transform(), true);
            }

            return (binExp, changed);
        }

        private (Expression, bool) PreVisitBinaryExpression(BinaryExpression binExp)
        {
            // (+ id1 id1) ==> (* id1 2)
            if (add2ids.Match(binExp))
            {
                var (e, _)= add2ids.Transform().Accept(this);
                return (e, true);
            }
            if (binopWithSelf.Match(binExp))
            {
                var (e, _) = binopWithSelf.Transform(ctx).Accept(this);
                return (e, true);
            }
            if (distributedConvert.Match(binExp))
            {
                var (e, _) = distributedConvert.Transform(ctx).Accept(this);
                return (e, true);
            }
            if (distributedSlice.Match(binExp))
            {
                var (e, _) = distributedSlice.Transform(ctx).Accept(this);
                return (e, true);
            }
            if (distributedCast.Match(binExp))
            {
                var (e, _) = distributedCast.Transform(ctx).Accept(this);
                return (e, true);
            }

            // (exp >> n) << n => __align(exp, 1<<n)
            var eNew = ShiftRightShiftLeft(binExp);
            if (eNew != null)
            {
                return (eNew, true);
            }
            return (binExp, false);
        }

        private bool IsNonFloatConstant(Constant? cRight)
        {
            return 
                cRight != null &&
                cRight.DataType is PrimitiveType pt &&
                pt.Domain != Domain.Real;
        }

        private Expression? ShiftLeftShiftRight(BinaryExpression bin, Constant? cRight)
        {
            if (cRight == null)
                return null;
            if (bin.Left is BinaryExpression binInner)
            {
                DataType dtConvert;
                if (bin.Operator == Operator.Shr)
                {
                    dtConvert = binInner.DataType;
                }
                else if (bin.Operator == Operator.Sar)
                {
                    dtConvert = PrimitiveType.Create(Domain.SignedInt, binInner.DataType.BitSize);
                }
                else
                {
                    return null;
                }

                if (binInner.Operator == Operator.Shl &&
                    binInner.Right is Constant cInnerRight &&
                    cmp.Equals(cRight, cInnerRight))
                {
                    var dtSlice = PrimitiveType.CreateWord(binInner.Left.DataType.BitSize - cRight.ToInt32());
                    var slice = new Slice(dtSlice, binInner.Left, 0);
                    return new Conversion(slice, slice.DataType, dtConvert);
                }
            }
            return null;
        }

        private Expression? ShiftRightShiftLeft(BinaryExpression bin)
        {
            if (bin.Operator != Operator.Shl || !(bin.Right is Constant cRight))
                return null;
            if (bin.Left is Identifier idLeft)
            {
                var innerExp = ctx.GetDefiningExpression(idLeft);
                if (innerExp is null)
                    return null;
                if (innerExp is BinaryExpression binInner && 
                    (binInner.Operator == Operator.Shr || binInner.Operator == Operator.Sar) &&
                    cmp.Equals(cRight, binInner.Right))
                {
                    ctx.RemoveExpressionUse(idLeft);
                    ctx.UseExpression(binInner.Left);
                    var sig = FunctionType.Func(
                        new Identifier("", bin.DataType, null!),
                        new Identifier("x", binInner.Left.DataType, null!),
                        new Identifier("y", PrimitiveType.Int32, null!));
                    var align = new IntrinsicProcedure(IntrinsicProcedure.Align, false, sig);
                    return m.Fn(align, binInner.Left, Constant.Int32(1 << cRight.ToInt32()));
                }
            }
            return null;
        }


        public static Constant SimplifyTwoConstants(Operator op, Constant l, Constant r)
        {
            PrimitiveType lType = (PrimitiveType) l.DataType;
            PrimitiveType rType = (PrimitiveType) r.DataType;
            if ((lType.Domain & rType.Domain) != 0)
            {
                return ((BinaryOperator) op).ApplyConstants(l, r);
            }
            throw new ArgumentException(string.Format("Can't add types of different domains {0} and {1}", l.DataType, r.DataType));
        }

        public virtual (Expression, bool) VisitConversion(Conversion conversion)
        {
            var (exp, changed) = conversion.Expression.Accept(this);
            if (!(exp is InvalidConstant))
            {
                var ptCvt = conversion.DataType.ResolveAs<PrimitiveType>();
                var ptSrc = conversion.SourceDataType.ResolveAs<PrimitiveType>();
                if (exp is Constant c && ptCvt != null)
                {
                    if (ptSrc != null)
                    {
                        if (ptCvt.Domain == Domain.Real)
                        {
                            if (ptSrc.Domain == Domain.Real)
                            {
                                if (ptCvt.Size < ptSrc.Size)
                                {
                                    // Real-to-real conversion.
                                    return (ConstantReal.Create(ptCvt, c.ToReal64()), true);
                                }
                            }
                            else if (ptSrc.IsWord)
                            {
                                // Raw bit pattern reinterpretation.
                                return (CastRawBitsToReal(ptCvt, c), true);
                            }
                            else
                            {
                                // integer to real conversion
                                return (ConstantReal.Create(ptCvt, c.ToInt64()), true);
                            }
                        }
                        else if ((ptSrc.Domain & Domain.Integer) != 0)
                        {
                            if (ptSrc != null)
                            {
                                if (ptSrc.Domain == Domain.SignedInt)
                                {
                                    return (Constant.Create(ptCvt, c.ToInt64()), true);
                                }
                                else if (ptSrc.Domain.HasFlag(Domain.SignedInt))
                                {
                                    return (Constant.Create(ptCvt, c.ToUInt64()), true);
                                }
                            }
                        }
                    }
                }
                if (exp is Identifier id && 
                    ctx.GetDefiningExpression(id) is MkSequence seq)
                {
                    // If we are casting a SEQ, and the corresponding element is >= 
                    // the size of the cast, then use deposited part directly.
                    var lsbElem = seq.Expressions[seq.Expressions.Length - 1];
                    int sizeDiff = lsbElem.DataType.Size - conversion.DataType.Size;
                    if (sizeDiff >= 0)
                    {
                        foreach (var elem in seq.Expressions)
                        {
                            ctx.RemoveExpressionUse(elem);
                        }
                        ctx.UseExpression(lsbElem);
                        if (sizeDiff > 0)
                        {
                            return (new Conversion(lsbElem, lsbElem.DataType, conversion.DataType), true);
                        }
                        else
                        {
                            return (lsbElem, true);
                        }
                    }
                }
                if (exp is ProcedureConstant pc && conversion.DataType.BitSize == pc.DataType.BitSize)
                {
                    // (wordnn) procedure_const => procedure_const
                    return (pc, true);
                }
                if (exp.DataType.BitSize == conversion.DataType.BitSize)
                {
                    // Redundant word-casts can be stripped.
                    if (conversion.DataType.IsWord)
                    {
                        return (exp, true);
                    }
                }
                conversion = new Conversion(exp, exp.DataType, conversion.DataType);
            }
            if (convertConvertRule.Match(conversion))
            {
                return (convertConvertRule.Transform(), true);
            }
            return (conversion, changed);
        }

        /// <summary>
        /// Take a bitvector of type wordXXX and reinterpret it as a floating-point
        /// constant.
        /// </summary>
        /// <param name="ptCast">Floating-point type to which the raw bits are being cast.</param>
        /// <param name="rawBits">The raw bits being cast.</param>
        /// <returns>A floating-point constant, possibly with a <see cref="Cast"/> wrapped around it
        /// if the constant is not 32- or 64-bit.
        /// </returns>
        private Expression CastRawBitsToReal(PrimitiveType ptCast, Constant rawBits)
        {
            var bitSize = Math.Min(rawBits.DataType.BitSize, 64);
            var dtImm = PrimitiveType.Create(Domain.Real, bitSize);
            var cImm = Constant.RealFromBitpattern(dtImm, rawBits.ToInt64());
            cImm = ConstantReal.Create(dtImm, cImm.ToReal64());
            if (cImm.DataType.BitSize == ptCast.BitSize)
            {
                return cImm;
            }
            else
            {
                return new Conversion(cImm, cImm.DataType, ptCast);
            }
        }

        public virtual (Expression, bool) VisitCast(Cast cast)
        {
            var (e, changed) = cast.Expression.Accept(this);
            if (changed)
                cast = new Cast(cast.DataType, e);
            return (cast, changed);
        }

        public virtual (Expression, bool) VisitConditionalExpression(ConditionalExpression c)
        {
            var (cond, cChanged) = c.Condition.Accept(this);
            var (t, tChanged) = c.ThenExp.Accept(this);
            var (f, fChanged) = c.FalseExp.Accept(this);
            if (cond is Constant cCond && cCond.DataType == PrimitiveType.Bool)
            {
                //$TODO: side effects?
                if (cCond.IsZero)
                    return (f, true);
                else
                    return (t, true);
            }
            bool changed = (cChanged | tChanged | fChanged);
            if (changed)
                c = new ConditionalExpression(c.DataType, cond, t, f);
            return (c, changed);
        }

        public virtual (Expression, bool) VisitConditionOf(ConditionOf c)
        {
            var (e, changed) = c.Expression.Accept(this);
            //$REVIEW: if e == 0, then Z flags could be set to 1. But that's architecture specific, so
            // we leave that as an exercise to re reader
            if (changed)
                c = new ConditionOf(e);
            return (c, changed);
        }

        public virtual (Expression, bool) VisitConstant(Constant c)
        {
            return (c, false);
        }

        public virtual (Expression, bool) VisitDereference(Dereference deref)
        {
            var (e, changed) = deref.Expression.Accept(this);
            if (changed)
                deref = new Dereference(deref.DataType, e);
            return (deref, changed);
        }

        public virtual (Expression, bool) VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public virtual (Expression, bool) VisitIdentifier(Identifier id)
        {
            if (idConst.Match(id))
            {
                return (idConst.Transform(), true);
            }
            if (idProcConstRule.Match(id))
            {
                return (idProcConstRule.Transform(), true);
            }
            // jkl: Copy propagation causes real problems when used during trashed register analysis.
            // If needed in other passes, it should be an option for expression e
            if (idCopyPropagation.Match(id))
            {
                return (idCopyPropagation.Transform(), true);
            }
            if (idBinIdc.Match(id))
            {
                return (idBinIdc.Transform(), true);
            }
            return (id, false);
        }

        public virtual (Expression, bool) VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            //$TODO: this has been disabled.
            //if (mpsRule.Match(mps))
            //{
            //    var (e, _) = mpsRule.Transform().Accept(this);
            //    return 
            //}
            var basePtr = mps.BasePointer.Accept(this);
            var memberPtr = mps.MemberPointer.Accept(this);
            var changed = basePtr.Item2 | memberPtr.Item2;
            if (changed)
                mps = new MemberPointerSelector(mps.DataType, basePtr.Item1, memberPtr.Item1);
            return (mps, changed);
        }

        public virtual (Expression, bool) VisitMemoryAccess(MemoryAccess access)
        {
            var (offset, changed) = access.EffectiveAddress.Accept(this);
            if (this.scaledIndexRule.Match(offset))
            {
                changed = true;
                (offset, _) = scaledIndexRule.Transform().Accept(this);
            }
            var value = new MemoryAccess(
                access.MemoryId,
                offset,
                access.DataType);
            var newValue = ctx.GetValue(value, segmentMap);
            if (newValue != value)
            {
                ctx.RemoveExpressionUse(value);
                ctx.UseExpression(newValue);
                changed = true;
            }
            return (newValue, changed);
        }

        public virtual (Expression, bool) VisitMkSequence(MkSequence seq)
        {
            bool changed = false;
            var newSeq = seq.Expressions.Select(e =>
            {
                var (eNew, eChanged) = e.Accept(this);
                if (eNew is InvalidConstant)
                    eNew = e;
                changed |= eChanged;
                return eNew;
            }).ToArray();
            if (newSeq.Length == 2)
            {
                // Special case for the frequent case of segment:offset or 
                // two concatenated bit vectors.
                if (newSeq[0] is Constant c1 && newSeq[1] is Constant c2)
                {
                    PrimitiveType tHead = (PrimitiveType) c1.DataType;
                    PrimitiveType tTail = (PrimitiveType) c2.DataType;
                    PrimitiveType t;
                    if (tHead.Domain == Domain.Selector)            //$REVIEW: seems to require Address, SegmentedAddress?
                    {
                        t = PrimitiveType.Create(Domain.Pointer, tHead.BitSize + tTail.BitSize);
                        return (ctx.MakeSegmentedAddress(c1, c2), true);
                    }
                    else
                    {
                        t = PrimitiveType.Create(tHead.Domain, tHead.BitSize + tTail.BitSize);
                        return (Constant.Create(t, (c1.ToUInt64() << tTail.BitSize) | c2.ToUInt64()), true);
                    }
                }
            }
            else if (newSeq.All(e => e is Constant))
            {
                //$TODO: > 64 bit values?
                ulong value = 0;
                for (int i = 0; i < newSeq.Length; ++i)
                {
                    var c = (Constant) newSeq[i];
                    value = (value << c.DataType.BitSize) | c.ToUInt64();
                }
                return (Constant.Create(seq.DataType, value), true);
            }
            if (newSeq.Take(newSeq.Length - 1).All(e => e.IsZero))
            {
                var tail = newSeq.Last();
                // leading zeros imply a conversion to unsigned.
                return (new Conversion(
                    tail,
                    PrimitiveType.Create(Domain.UnsignedInt, tail.DataType.BitSize),
                    PrimitiveType.Create(Domain.UnsignedInt, seq.DataType.BitSize)),
                    true);
            }
            var mem = FuseAdjacentMemoryAccesses(seq.DataType, newSeq);
            if (mem != null)
                return (mem, true);
            //$TODO: fix changed here.
            return FuseAdjacentSlices(seq.DataType, newSeq);
        }

        private Expression? FuseAdjacentMemoryAccesses(DataType dt, Expression[] elems)
        {
            var (access, seg, ea, offset) = AsMemoryAccess(elems[0]);
            if (access == null)
                return null;
            var fused = new List<Expression>();
            var offsetFused = offset;
            for (int i = 1; i < elems.Length; ++i)
            {
                var (accNew, segNew, eaNew, offNew) = AsMemoryAccess(elems[i]);
                if (accNew == null)
                    return null;
                if (cmp.Equals(seg, segNew) &&
                    cmp.Equals(ea, eaNew) &&
                    ctx.Endianness.OffsetsAdjacent(offNew, offset, accNew.DataType.Size))
                {
                    offsetFused = Math.Min(offsetFused, offNew);
                }
                else
                    return null;
            }
            Expression fusedEa;
            if (ea is null)
            {
                fusedEa = Constant.Create(access.EffectiveAddress.DataType, (ulong)offsetFused);
            }
            else
            {
                fusedEa = m.AddSubSignedInt(ea, offsetFused);
            }

            var result = (seg is null)
                ? m.Mem(access.MemoryId, dt, fusedEa)
                : m.SegMem(access.MemoryId, dt, seg, fusedEa);

            foreach (var e in elems)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(result);
            return result;
        }

        private (MemoryAccess? access, Expression? seg, Expression? ea, long offset) AsMemoryAccess(Expression expression)
        {
            MemoryAccess access;
            Expression? seg;
            Expression ea;
            if (expression is SegmentedAccess segmem)
            {
                access = segmem;
                seg = segmem.BasePointer;
                ea = segmem.EffectiveAddress;
            }
            else if (expression is MemoryAccess mem)
            {
                access = mem;
                seg = null;
                ea = mem.EffectiveAddress;
            }
            else
                return (null, null, null, 0);

            long offset = 0;
            Expression? eaStripped = ea;
            if (ea is Constant global)
            {
                offset = global.ToInt64();
                eaStripped = null;
            }
            else if (ea is Address addr && !addr.Selector.HasValue)
            {
                offset = (long) addr.ToLinear();
                eaStripped = null;
            }
            else if (ea is BinaryExpression bin)
            {
                if (bin.Right is Constant c)
                {
                    if (bin.Operator == Operator.IAdd)
                    {
                        offset = c.ToInt64();
                        eaStripped = bin.Left;
                    }
                    else if (bin.Operator == Operator.ISub)
                    {
                        offset = -c.ToInt64();
                        eaStripped = bin.Left;
                    }
                }
            }
            return (access, seg, eaStripped, offset);
        }

        private (Expression, bool) FuseAdjacentSlices(DataType dataType, Expression[] elems)
        {
            var fused = new List<Expression> { AsSlice(elems[0]) ?? elems[0] };
            bool changed = false;
            for (int i = 1; i < elems.Length; ++i)
            {
                Slice? slNext = AsSlice(elems[i]);
                if (fused[fused.Count - 1] is Slice slPrev && slNext != null &&
                    cmp.Equals(slPrev.Expression, slNext.Expression) &&
                    slPrev.Offset == slNext.Offset + slNext.DataType.BitSize)
                {
                    // Fuse the two consecutive slices. 
                    var newSlice = new Slice(
                        PrimitiveType.CreateWord(slPrev.DataType.BitSize + slNext.DataType.BitSize),
                        slNext.Expression,
                        slNext.Offset);
                    (fused[fused.Count - 1], _) = newSlice.Accept(this);
                    changed = true;
                }
                else
                {
                    fused.Add(elems[i]);
                }
            }
            if (changed)
            {
                foreach (var e in elems)
                    ctx.RemoveExpressionUse(e);
                foreach (var f in fused)
                    ctx.UseExpression(f);
                if (fused.Count == 1)
                    return (fused[0], true);
                else
                    return (new MkSequence(dataType, fused.ToArray()), true);
            }
            else
            {
                return (new MkSequence(dataType, elems), false);
            }
        }

        private Slice? AsSlice(Expression? e)
        {
            if (e is Identifier id)
            {
                e = ctx.GetDefiningExpression(id);
            }
            if (e is Cast c)
            {
                return new Slice(c.DataType, c.Expression, 0);
            }
            else
            {
                return e as Slice;
            }
        }

        public virtual (Expression, bool) VisitOutArgument(OutArgument outArg)
        {
            if (!(outArg.Expression is Identifier))
            {
                var (exp, change) = outArg.Expression.Accept(this);
                if (change)
                {
                    return (new OutArgument(outArg.DataType, exp), true);
                }
            }
            return (outArg, false);
        }

        public virtual (Expression, bool) VisitPhiFunction(PhiFunction pc)
        {
            var args = pc.Arguments
                .Select(a =>
                {
                    var (e, _) = a.Value.Accept(this);
                    var (arg, _) = SimplifyPhiArg(e);
                    ctx.RemoveExpressionUse(arg);
                    return arg;
                })
                .Where(a => ctx.GetValue((a as Identifier)!) != pc)
                .ToArray();

            var cmp = new ExpressionValueComparer();
            var e = args.FirstOrDefault();
            if (e != null && args.All(a => cmp.Equals(a, e)))
            {
                ctx.UseExpression(e);
                return (e, true);
            }
            else
            {
                ctx.UseExpression(pc);
                return (pc, false);
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
        private (Expression, bool) SimplifyPhiArg(Expression arg)
        {
            if (!(arg is BinaryExpression bin &&
                  bin.Left is Identifier idLeft &&
                  ctx.GetValue(idLeft) is BinaryExpression binLeft))
                return (arg, false);

            ctx.RemoveIdentifierUse(idLeft);
            ctx.UseExpression(binLeft);
            bin = new BinaryExpression(
                bin.Operator,
                bin.DataType,
                binLeft,
                bin.Right);
            return bin.Accept(this);
        }

        public virtual (Expression, bool) VisitPointerAddition(PointerAddition pa)
        {
            return (pa, false);
        }

        public virtual (Expression, bool) VisitProcedureConstant(ProcedureConstant pc)
        {
            return (pc, false);
        }

        public virtual (Expression, bool) VisitScopeResolution(ScopeResolution sc)
        {
            return (sc, false);
        }

        public virtual (Expression, bool) VisitSegmentedAccess(SegmentedAccess segMem)
        {
            var (basePtr, bChanged) = segMem.BasePointer.Accept(this);
            var (offset, oChanged) = segMem.EffectiveAddress.Accept(this);
            bool changed = bChanged | oChanged;
            if (this.scaledIndexRule.Match(offset))
            {
                changed = true;
                (offset, _) = scaledIndexRule.Transform().Accept(this);
            }
            if (basePtr is Constant cBase && offset is Constant cOffset)
            {
                var addr = ctx.MakeSegmentedAddress(cBase, cOffset);
                var mem = new MemoryAccess(segMem.MemoryId, addr, segMem.DataType);
                return (ctx.GetValue(mem, segmentMap), true);
            }
            var value = new SegmentedAccess(segMem.MemoryId, basePtr, offset, segMem.DataType);
            if (sliceSegPtr.Match(value))
            {
                return (sliceSegPtr.Transform(), true);
            }
            var newVal = ctx.GetValue(value, segmentMap);
            if (newVal != value)
            {
                ctx.RemoveExpressionUse(value);
                ctx.UseExpression(newVal);
                return (newVal, changed);
            }
            return (value, changed);
        }

        public virtual (Expression, bool) VisitSlice(Slice slice)
        {
            var e = slice.Expression;
            if (e is BinaryExpression bin)
            {
                if (CanBeSliced(slice, bin))
                {
                    var left = m.Slice(bin.Left, slice.DataType, slice.Offset).Accept(this);
                    var right = m.Slice(bin.Right, slice.DataType, slice.Offset).Accept(this);
                    if (left.Item2 | right.Item2)
                    {
                        var exp = new BinaryExpression(bin.Operator, slice.DataType,
                            left.Item1, right.Item1);
                        return (exp, true);
                    }
                }
            }
            bool changed;
            (e, changed) = e.Accept(this);

            // Is the slice the same size as the expression?
            if (slice.Offset == 0 && slice.DataType.BitSize == e.DataType.BitSize)
                return (e, true);
            slice = new Slice(slice.DataType, e, slice.Offset);
            if (sliceConst.Match(slice))
            {
                return (sliceConst.Transform(), true);
            }
            if (sliceMem.Match(slice))
            {
                return (sliceMem.Transform(), true);
            }

            // (slice (shl e n) n) ==> e
            if (sliceShift.Match(slice))
            {
                return (sliceShift.Transform(), true);
            }
            if (sliceSeq.Match(slice))
            {
                return (sliceSeq.Transform(), true);
            }
            if (sliceConvert.Match(slice))
            {
                return (sliceConvert.Transform(), true);
            }
            if (e is Identifier id &&
                ctx.GetDefiningExpression(id) is MkSequence seq)
            {
                // If we are casting a SEQ, and the corresponding element is >= 
                // the size of the cast, then use deposited part directly.
                var lsbElem = seq.Expressions[seq.Expressions.Length - 1];
                int sizeDiff = lsbElem.DataType.Size - slice.DataType.Size;
                if (sizeDiff >= 0)
                {
                    foreach (var elem in seq.Expressions)
                    {
                        ctx.RemoveExpressionUse(elem);
                    }
                    ctx.UseExpression(lsbElem);
                    if (sizeDiff > 0)
                    {
                        return (new Slice(slice.DataType, lsbElem, slice.Offset), true);
                    }
                    else
                    {
                        return (lsbElem, true);
                    }
                }
            }
            return (slice, changed);
        }

        private static bool CanBeSliced(Slice slice, BinaryExpression bin)
        {
            if (
                bin.Operator == Operator.And ||
                bin.Operator == Operator.Or ||
                bin.Operator == Operator.Xor)
            {
                return true;
            }

            if (bin.Operator == Operator.IAdd ||
                bin.Operator == Operator.ISub ||
                bin.Operator == Operator.Shl ||
                bin.Operator == Operator.IMul ||
                bin.Operator == Operator.UMul ||
                bin.Operator == Operator.SMul)
            {
                return slice.Offset == 0;
            }
            return false;
        }

        public virtual (Expression, bool) VisitTestCondition(TestCondition tc)
        {
            var (e, changed) = tc.Expression.Accept(this);
            if (changed)
                tc = new TestCondition(tc.ConditionCode, e);
            return (tc, changed);
        }

        public virtual (Expression, bool) VisitUnaryExpression(UnaryExpression unary)
        {
            var (e, changed) = unary.Expression.Accept(this);
            if (changed)
                unary = new UnaryExpression(unary.Operator, unary.DataType, e);
            if (negSub.Match(unary))
            {
                return (negSub.Transform(), true);
            }

            // (!-exp) >= (!exp)
            if (logicalNotFollowedByNeg.Match(unary))
            {
                return (logicalNotFollowedByNeg.Transform(), true);
            }

            return (unary, changed);
        }
    }
}
