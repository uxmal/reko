#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Reko.Evaluation
{
    /// <summary>
    /// Partially evaluates expressions, using an <see cref="EvaluationContext"/> to obtain the values
    /// of identifiers and optionally modifies the expression being evaluated.
    /// </summary>
    public class ExpressionSimplifier : ExpressionVisitor<(Expression, bool)>
    {
        private static readonly IdConstant idConst;
        private static readonly ComparisonConstOnLeft constOnLeft;
        private static readonly AddTwoIdsRule add2ids;
        private static readonly Add_e_c_cRule addEcc;
        private static readonly Add_mul_id_c_id_Rule addMici;
        private static readonly ConstConstBin_Rule constConstBin;
        private static readonly IdCopyPropagationRule idCopyPropagation;
        private static readonly IdBinIdc_Rule idBinIdc;
        private static readonly SliceConstant_Rule sliceConst;
        private static readonly SliceMem_Rule sliceMem;
        private static readonly SliceSegmentedPointer_Rule sliceSegPtr;
        private static readonly SliceShift sliceShift;
        private static readonly Shl_add_Rule shAdd;
        private static readonly Shl_mul_e_Rule shMul;
        private static readonly ShiftShift_c_c_Rule shiftShift;
        private static readonly NegSub_Rule negSub;
        private static readonly CompSub_Rule compSub;
        private static readonly Mps_Constant_Rule mpsRule;
        private static readonly BinOpWithSelf_Rule binopWithSelf;
        private static readonly ConstDivisionImplementedByMultiplication constDiv;
        private static readonly IdProcConstRule idProcConstRule;
        private static readonly ConvertConvertRule convertConvertRule;
        private static readonly DistributedCastRule distributedCast;
        private static readonly DistributedConversionRule distributedConvert;
        private static readonly DistributedSliceRule distributedSlice;
        private static readonly MkSeqFromSlices_Rule mkSeqFromSlicesRule;
        private static readonly SliceConvert sliceConvert;
        private static readonly LogicalNotFollowedByNegRule logicalNotFollowedByNeg;
        private static readonly LogicalNotFromArithmeticSequenceRule logicalNotFromBorrow;
        private static readonly UnaryNegEqZeroRule unaryNegEqZero;
        private static readonly ScaledIndexRule scaledIndexRule;

        private readonly IReadOnlySegmentMap segmentMap;
        private readonly EvaluationContext ctx;
        private readonly ExpressionValueComparer cmp;
        private readonly ExpressionEmitter m;
        private readonly Unifier unifier;
        private readonly DecompilerEventListener listener;



        public ExpressionSimplifier(IReadOnlySegmentMap segmentMap, EvaluationContext ctx, DecompilerEventListener listener)
            : this(segmentMap, ctx, new Unifier(), listener)
        {
            // Creating the unifier is slow, so we provide a constructor
            // where the unifier is passed in.
        }

        public ExpressionSimplifier(
            IReadOnlySegmentMap segmentMap,
            EvaluationContext ctx,
            Unifier unifier,
            DecompilerEventListener listener)
        {
            this.segmentMap = segmentMap ?? throw new ArgumentNullException(nameof(segmentMap));
            this.ctx = ctx;
            this.unifier = unifier;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.m = new ExpressionEmitter();
        }

        //$REVIEW: consider moving these predicates to OperatorTypeExtensions
        private static bool IsIntComparison(Operator op)
        {
            var type = op.Type;
            return type == OperatorType.Eq || type == OperatorType.Ne ||
                   type == OperatorType.Ge || type == OperatorType.Gt ||
                   type == OperatorType.Le || type == OperatorType.Lt ||
                   type == OperatorType.Uge || type == OperatorType.Ugt ||
                   type == OperatorType.Ule || type == OperatorType.Ult;
        }

        private static bool IsFloatComparison(OperatorType op)
        {
            return op == OperatorType.Feq || op == OperatorType.Fne ||
                   op == OperatorType.Fge || op == OperatorType.Fgt ||
                   op == OperatorType.Fle || op == OperatorType.Flt;
        }

        public static Constant ApplyConstants(BinaryOperator op, Constant l, Constant r)
        {
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
                if (intrinsic.Name == CommonOps.RolC.Name)
                {
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (new BinaryExpression(Operator.Shl, appl.DataType, args[0], args[1]), true);
                    }
                }
                else if (intrinsic.Name == CommonOps.RorC.Name)
                {
                    if (IsSingleBitRotationWithClearCarryIn(args))
                    {
                        return (new BinaryExpression(Operator.Shr, appl.DataType, args[0], args[1]), true);
                    }
                }
                else if (intrinsic.Name == CommonOps.Rol.Name)
                {
                    var rol = CombineRotations(intrinsic.Name, appl, args);
                    if (rol != null)
                    {
                        return (rol, true);
                    }
                }
                else if (intrinsic.Name == CommonOps.Ror.Name)
                {
                    var ror = CombineRotations(intrinsic.Name, appl, args);
                    if (ror != null)
                    {
                        return (ror, true);
                    }
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
            Expression result = acc;
            var (a, aChanged) = acc.Array.Accept(this);
            var (i, iChanged) = acc.Index.Accept(this);
            bool changed = aChanged | iChanged;
            if (changed)
            {
                acc = m.ARef(acc.DataType, a, i);
                result = acc;
            }

            if (i is Constant cIndex)
            {
                var bitPosition = cIndex.ToInt32() * acc.DataType.BitSize;
                if (IsSequence(ctx, a, out var seq))
                {
                    var eNew = SliceSequence(seq, acc.DataType, bitPosition);
                    if (eNew is not null)
                    {
                        ctx.RemoveExpressionUse(a);
                        ctx.UseExpression(eNew);
                        (eNew, _) = eNew.Accept(this);
                        return (eNew, true);
                    }
                }
                if (a is Constant cArray)
                {
                    var cValue = (cArray.ToBigInteger() >> bitPosition) & Bits.Mask(acc.DataType.BitSize);
                    result = Constant.Create(acc.DataType, cValue);
                    changed = true;
                }
            }
            return (result, changed);
        }


        public virtual (Expression, bool) VisitBinaryExpression(BinaryExpression binExp)
        {
            // BinaryExpressions are the most common and occur in clusters that sometimes
            // are so deep that attempting to simplify using recursion will cause a stack
            // overflow. The code below traverses a tree of BinaryExpressions iteratively
            // using an explicit stack to keep track of intermediate results.
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
            if (cLeft != null && BinaryExpression.Commutes(binExp.Operator.Type))
            {
                cRight = cLeft; left = right; right = cLeft;
            }
            Expression? e;
            //$TODO: operands to binary operations appear to be
            // mismatched in some processors. Change the ctor
            // of BinaryExpression to catch this later.
            var sameBitsize = left.DataType.BitSize == right.DataType.BitSize;
            if (cRight != null)
            {
                // (- X 0) ==> X
                // (+ X 0) ==> X
                if (cRight.IsIntegerZero && binExp.Operator.Type.IsAddOrSub())
                {
                    //$HACK: we neglected to zero-extend Carry flags in adc/sbc
                    // type instructions, and here it bites us. Work around by
                    // zero-extending now. The real fix is to zero-extend carry flags 
                    // properly.
                    if (cRight.DataType.BitSize > left.DataType.BitSize)
                    {
                        return (m.Convert(left, left.DataType, cRight.DataType), true);
                    }
                    return (left, true);
                }
                if (binExp.Operator.Type == OperatorType.Or)
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
                if (binExp.Operator.Type == OperatorType.And)
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
                if (binExp.Operator.Type == OperatorType.Xor)
                {
                    if (cRight.IsIntegerZero)
                    {
                        return (left, true);
                    }
                    if (cRight.IsMaxUnsigned && sameBitsize)
                    {
                        (e, _) = new UnaryExpression(Operator.Comp, left.DataType, left).Accept(this);
                        return (e, true);
                    }
                }
                if (binExp.Operator.Type == OperatorType.IAdd)
                {
                    if (!cRight.IsReal && cRight.IsIntegerOne)
                    {
                        if (left is UnaryExpression u &&
                            u.Operator.Type == OperatorType.Comp)
                        {
                            (e, _) = m.Neg(u.Expression).Accept(this);
                            return (e, true);
                        }
                    }
                }
            }

            //$REVIEW: this is evaluation! Shouldn't the be done by the evaluator?
            if (left is InvalidConstant || right is InvalidConstant)
                return (InvalidConstant.Create(binExp.DataType), lChanged | rChanged);

            binExp = new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
            e = constConstBin.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }

            Identifier? idLeft = left as Identifier;
            Identifier? idRight = right as Identifier;

            // (rel? id1 c) should just pass.

            if (IsIntComparison(binExp.Operator) && cRight != null && idLeft != null)
                return (binExp!, changed);

            // Floating point expressions with "integer" constants 
            if (IsFloatComparison(binExp.Operator.Type) && IsNonFloatConstant(cRight))
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
                if (binExp.Operator.Type.IsAddOrSub() &&
                    binLeft.Operator.Type.IsAddOrSub() &&
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
                                binOperator.Type == OperatorType.IAdd
                                    ? Operator.ISub
                                    : Operator.IAdd;
                            c = Operator.ISub.ApplyConstants(cLeftRight, cRight);
                        }
                    }
                    if (c.IsIntegerZero)
                        return (binLeft.Left, true);
                    return (new BinaryExpression(binOperator, binExp.DataType, binLeft.Left, c), true);
                }
                if (binExp.Operator.Type == OperatorType.IMul && binLeft.Operator.Type == OperatorType.IMul)
                {
                    var c = Operator.IMul.ApplyConstants(cLeftRight, cRight);
                    if (c.IsIntegerZero && !CriticalInstruction.IsCritical(binLeft))
                    {
                        ctx.RemoveExpressionUse(binLeft);
                        return (c, true);
                    }
                    else
                    {
                        return (new BinaryExpression(binExp.Operator, binExp.DataType, binLeft.Left, c), true);
                    }
                }
            }

            // (rel (- c e) 0 => (rel -c e) => (rel.Negate e c)

            if (binLeft != null && cRight != null && cRight.IsIntegerZero &&
                IsIntComparison(binExp.Operator) &&
                binLeft.Left is Constant cBinLeft)
            {
                if (binLeft.Operator.Type == OperatorType.ISub)
                {
                    return (new BinaryExpression(
                        ((ConditionalOperator) binExp.Operator).Negate(),
                        binExp.DataType,
                        binLeft.Right,
                        cBinLeft), true);
                } 
                else if (binLeft.Operator.Type == OperatorType.USub)
                {
                    _ = this;
                }
            }

            // (rel (- e c1) c2) => (rel e c1+c2)

            if (IsIntComparison(binExp.Operator) &&
                binLeft != null && cLeftRight != null && cRight != null &&
                !cLeftRight.IsReal && !cRight.IsReal)
            {
                if (binLeft.Operator.Type.IsAddOrSub())
                {
                    // (>u (- e c1) c2) => (>u e c1+c2) || (<u e c2)
                    if (binExp.Operator.Type == OperatorType.Ugt &&
                        binLeft.Operator.Type == OperatorType.ISub &&
                        !cRight.IsIntegerZero)
                    {
                        ctx.UseExpression(binLeft.Left);
                        var c = Operator.IAdd.ApplyConstants(cLeftRight, cRight);
                        return (m.Cor(
                            new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c),
                            m.Ult(binLeft.Left, cLeftRight)),
                            true);
                    }
                    else
                    {
                        ctx.RemoveIdentifierUse(idLeft!);
                        var op = binLeft.Operator.Type == OperatorType.IAdd ? Operator.ISub : Operator.IAdd;
                        var c = op.ApplyConstants(cLeftRight, cRight);
                        return (new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c), true);
                    }
                }
                else if (binLeft.Operator.Type == OperatorType.USub)
                {
                    ctx.RemoveIdentifierUse(idLeft!);
                    var op = binLeft.Operator.Type == OperatorType.IAdd ? Operator.ISub : Operator.IAdd;
                    var c = op.ApplyConstants(cLeftRight, cRight);
                    var opCmp = ((ConditionalOperator) binExp.Operator).ToUnsigned();
                    return (new BinaryExpression(opCmp, PrimitiveType.Bool, binLeft.Left, c), true);
                }
            }
            var dwordIdiom = UnfoldDwordIdiom(binExp);
            if (dwordIdiom is not null)
                return (dwordIdiom, true);

            // (rel C non-C) => (trans(rel) non-C C)
            e = constOnLeft.Match(binExp);
            if (e is not null)
            {
                (e, _) = e.Accept(this);
                return (e, true);
            }
            // (rel (- a b) 0) => (rel a b)
            if (IsIntComparison(binExp.Operator) &&
                binLeft is not null &&
                binLeft.Operator.Type == OperatorType.ISub &&
                right.IsZero)
            {
                e = new BinaryExpression(binExp.Operator, binExp.DataType,
                    binLeft.Left, binLeft.Right);
                return (e, true);
            }
            e = addMici.Match(binExp, ctx);
            if (e is not null)
            {
                return (e, true);
            }
            e = shAdd.Match(binExp, ctx);
            if (e is not null)
            {
                return (e, true);
            }

            e = shMul.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }

            e = shiftShift.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }

            var eNew = ShiftLeftShiftRight(binExp, cRight);
            if (eNew != null)
            {
                return (eNew, true);
            }

            // (e << c1) + (e << c2) => e * c3
            eNew = SumDiffOfShifts(binExp);
            if (eNew is not null)
                return (eNew, true);

            // (e * c1) + (e * c2) => e * (c1 + c2)
            e = SumDiffProducts(binExp);
            if (e is not null)
                return (e, true);

            // (-exp == 0) => (exp == 0)
            e = unaryNegEqZero.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }

            e = logicalNotFromBorrow.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }
            return (binExp, changed);
        }

        /// <summary>
        /// Real mode x86 has an idiom for comparing dx:ax with zero:
        ///     or dx,ax
        /// which sets the Z flag if both ax and dx are zero.
        /// This gets translated by the x86 rewriter, and stages of
        /// the data flow analysis to:
        ///     SLICE(dx_ax, 16) | SLICE(dx_ax, 0) == 0
        /// We "unfold" this to
        ///     dx_ax == 0
        /// </summary>
        private Expression? UnfoldDwordIdiom(BinaryExpression binExp)
        {
            (BinaryExpression?,Slice?,Slice?) MatchOr(Expression left, Expression right)
            {
                if (right is Constant cRight && cRight.IsIntegerZero &&
                    left is BinaryExpression binLeft &&
                    binLeft.Operator.Type == OperatorType.Or)
                {
                    var leftSlice = AsSlice(binLeft.Left);
                    var rightSlice = AsSlice(binLeft.Right);
                    if (leftSlice is not null && rightSlice is not null)
                    {
                        return (binLeft, leftSlice, rightSlice);
                    }
                }
                return (null, null, null);
            }

            if (binExp.Operator.Type != OperatorType.Eq && binExp.Operator.Type != OperatorType.Ne)
                return null;
            var (or, left, right) = MatchOr(binExp.Left, binExp.Right);
            if (or is null)
            {
                (or, left, right) = MatchOr(binExp.Right, binExp.Left);
                if (or is null)
                    return null;
            }
            var leftSize = left!.DataType.BitSize;
            var rightSize = right!.DataType.BitSize;

            if (left.Expression == right.Expression &&
                leftSize + rightSize == left.Expression.DataType.BitSize && 
                (left.Offset == right.DataType.BitSize ||
                 right.Offset == left.DataType.BitSize))
            {
                return new BinaryExpression(
                    binExp.Operator,
                    binExp.DataType,
                    left.Expression,
                    binExp.Right);
            }
            else
            {
                return null;
            }
        }

        private (Expression, bool) PreVisitBinaryExpression(BinaryExpression binExp)
        {
            // (+ id1 id1) ==> (* id1 2)
            var e = add2ids.Match(binExp, ctx);
            if (e is not null)
            {
                (e, _)= e.Accept(this);
                return (e, true);
            }
            e = binopWithSelf.Match(binExp, ctx);
            if (e is not null)
            {
                (e, _) = e.Accept(this);
                return (e, true);
            }
            e = distributedConvert.Match(binExp);
            if (e is not null)
            { 
                (e, _) = e.Accept(this);
                return (e, true);
            }
            e = distributedSlice.Match(binExp);
            if (e is not null)
            {
                (e, _) = e.Accept(this);
                return (e, true);
            }
            e = distributedCast.Match(binExp);
            if (e is not null)
            {
                (e, _) = e.Accept(this);
                return (e, true);
            }

            // (exp >> n) << n => __align(exp, 1<<n)
            e = ShiftRightShiftLeft(binExp);
            if (e is not null)
            {
                return (e, true);
            }
            return (binExp, false);
        }

        private bool IsNonFloatConstant(Constant? cRight)
        {
            return cRight != null && !cRight.DataType.IsReal;
        }

        private Expression? ShiftLeftShiftRight(BinaryExpression bin, Constant? cRight)
        {
            if (cRight is null)
                return null;
            if (bin.Left is BinaryExpression binInner)
            {
                DataType dtConvert;
                if (bin.Operator.Type == OperatorType.Shr)
                {
                    dtConvert = binInner.DataType;
                }
                else if (bin.Operator.Type == OperatorType.Sar)
                {
                    dtConvert = PrimitiveType.Create(Domain.SignedInt, binInner.DataType.BitSize);
                }
                else
                {
                    return null;
                }

                if (binInner.Operator.Type == OperatorType.Shl &&
                    binInner.Right is Constant cInnerRight &&
                    cmp.Equals(cRight, cInnerRight))
                {
                    var sliceBits = binInner.Left.DataType.BitSize - cRight.ToInt32();
                    if (sliceBits <= 0)
                        return Constant.Zero(dtConvert);
                    var dtSlice = PrimitiveType.CreateWord(sliceBits);
                    var slice = new Slice(dtSlice, binInner.Left, 0);
                    return new Conversion(slice, slice.DataType, dtConvert);
                }
            }
            return null;
        }

        private Expression? SumDiffOfShifts(BinaryExpression bin)
        {
            if (bin.Operator.Type.IsAddOrSub() &&
                bin.Left is BinaryExpression left &&
                left.Operator.Type == OperatorType.Shl &&
                left.Right is Constant cLeft &&
                bin.Right is BinaryExpression right &&
                right.Operator.Type == OperatorType.Shl &&
                right.Right is Constant cRight &&
                cmp.Equals(left.Left, right.Left))
            {
                var shLeft = 1 << cLeft.ToInt32();
                var shRight = 1 << cRight.ToInt32();
                ctx.RemoveExpressionUse(right.Left);
                return m.IMul(left.Left,
                    bin.Operator.Type == OperatorType.IAdd
                        ? shLeft + shRight
                        : shLeft - shRight);
            }
            return null;
        }

        private Expression? SumDiffProducts(BinaryExpression bin)
        {
            if (bin.Operator.Type.IsAddOrSub() &&
                bin.Left is BinaryExpression left &&
                left.Operator is IMulOperator &&
                left.Right is Constant cLeft &&
                bin.Right is BinaryExpression right &&
                right.Operator == left.Operator &&
                right.Right is Constant cRight &&
                cmp.Equals(left.Left, right.Left))
            {
                var mLeft = cLeft.ToInt32();
                var mRight = cRight.ToInt32();
                ctx.RemoveExpressionUse(right.Left);
                return new BinaryExpression(left.Operator, left.DataType, 
                    left.Left,
                    Constant.Create(
                        left.DataType,
                        bin.Operator.Type == OperatorType.IAdd
                            ? mLeft + mRight
                            : mLeft - mRight));
            }
            return null;
        }

        private Expression? ShiftRightShiftLeft(BinaryExpression bin)
        {
            if (bin.Operator.Type != OperatorType.Shl || bin.Right is not Constant cRight)
                return null;
            if (bin.Left is Identifier idLeft)
            {
                var innerExp = ctx.GetDefiningExpression(idLeft);
                if (innerExp is null)
                    return null;
                if (innerExp is BinaryExpression binInner && 
                    (binInner.Operator.Type == OperatorType.Shr || binInner.Operator.Type == OperatorType.Sar) &&
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


        //$TODO: rename to 'ApplyConstants'
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
            if (exp is not InvalidConstant)
            {
                var ptCvt = conversion.DataType;
                var ptSrc = conversion.SourceDataType;
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
                                return (ReinterpretBitsAsReal(ptCvt, c), true);
                            }
                            else
                            {
                                // integer to real conversion
                                return (ConstantReal.Create(ptCvt, c.ToInt64()), true);
                            }
                        }
                        else if ((ptSrc.Domain & Domain.Integer) != 0)
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
                        else if (ptSrc.Domain == Domain.Boolean)
                        {
                            return (Constant.Create(ptCvt, c.ToUInt64()), true);
                        }
                    }
                }
                if (exp is Identifier id && 
                    ctx.GetDefiningExpression(id) is MkSequence seq)
                {
                    // If we are converting a SEQ, and the corresponding element is >= 
                    // the size of the cast, then use deposited part directly.
                    var lsbElem = seq.Expressions[^1];
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
            exp = convertConvertRule.Match(conversion);
            if (exp is not null)
            {
                return (exp, true);
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
        private Expression ReinterpretBitsAsReal(DataType ptCast, Constant rawBits)
        {
            var bitSize = Math.Min(rawBits.DataType.BitSize, 64);
            var dtImm = PrimitiveType.Create(Domain.Real, bitSize);
            var cImm = Constant.RealFromBitpattern(dtImm, rawBits);
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
                //$TODO: side effects
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
            var e = idConst.Match(id, this.ctx, unifier, listener);
            if (e is not null)
            {
                return (e, true);
            }
            e = idProcConstRule.Match(id, ctx);
            if (e is not null)
            {
                return (e, true);
            }
            // jkl: Copy propagation causes real problems when used during trashed register analysis.
            // If needed in other passes, it should be an option for expression e
            e = idCopyPropagation.Match(id, ctx);
            if (e is not null)
            {
                return (e, true);
            }
            e = idBinIdc.Match(id, ctx);
            if (e is not null)
            {
                return (e, true);
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
            var e = scaledIndexRule.Match(offset, ctx);
            if (e is not null)
            {
                changed = true;
                (offset, _) = e.Accept(this);
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
            (newSeq, changed) = FlattenNestedSequences(newSeq, changed);

            // SEQ(SEQ(a, b), c) = SEQ(a, b, c)

            if (newSeq.Length == 2)
            {
                // Special case for the frequent case of segment:offset or 
                // two concatenated bit vectors.
                if (newSeq[0] is Constant c1 && newSeq[1] is Constant c2)
                {
                    DataType tHead = c1.DataType;
                    DataType tTail = c2.DataType;
                    PrimitiveType t;
                    if (tHead.Domain == Domain.Selector)            //$REVIEW: seems to require Address, SegmentedAddress?
                    {
                        t = PrimitiveType.Create(Domain.Pointer, tHead.BitSize + tTail.BitSize);
                        return (ctx.MakeSegmentedAddress(c1, c2), true);
                    }
                    else if (tTail.Domain != Domain.Real)
                    {
                        t = PrimitiveType.Create(tHead.Domain, tHead.BitSize + tTail.BitSize);
                        if (tHead.BitSize + tTail.BitSize <= 64)
                            return (Constant.Create(t, (c1.ToUInt64() << tTail.BitSize) | c2.ToUInt64()), true);
                        else
                            return (Constant.Create(t, (c1.ToBigInteger() << tTail.BitSize) | c2.ToBigInteger()), true);
                    }
                }
            }
            else if (newSeq.All(e => e is Constant))
            {
                BigInteger value = BigInteger.Zero;
                for (int i = 0; i < newSeq.Length; ++i)
                {
                    var c = (Constant) newSeq[i];
                    value = (value << c.DataType.BitSize) | c.ToBigInteger();
                }
                return (Constant.Create(seq.DataType, value), true);
            }
            
            if (!newSeq[^1].DataType.IsReal && newSeq.Take(newSeq.Length - 1).All(e => e.IsZero))
            {
                var tail = newSeq.Last();
                // leading zeros imply a conversion to unsigned.
                return (new Conversion(
                    tail,
                    PrimitiveType.Create(Domain.UnsignedInt, tail.DataType.BitSize),
                    PrimitiveType.Create(Domain.UnsignedInt, seq.DataType.BitSize)),
                    true);
            }
            var mul = FuseSequenceOfSlicedMultiplications(seq.DataType, newSeq);
            if (mul is not null)
                return (mul, true);
            var log = FuseLogicalOperations(seq.DataType, newSeq);
            if (log is not null)
                return (log, true);
            var neg = SignExtendedNegation(seq);
            if (neg is not null)
                return (neg, true);
            var mem = FuseAdjacentMemoryAccesses(seq.DataType, newSeq);
            if (mem != null)
                return (mem, true);
            var slices = FuseAdjacentSlices(seq.DataType, newSeq);
            if (slices != null)
                return (slices, true);
            return (new MkSequence(seq.DataType, newSeq), changed);
        }

        private (Expression[] newSeq, bool changed) FlattenNestedSequences(
            Expression[] sequence,
            bool changed)
        {
            var result = new List<Expression>();
            foreach (var exp in sequence)
            {
                if (exp is MkSequence seq)
                {
                    result.AddRange(seq.Expressions);
                    changed = true;
                }
                else
                {
                    result.Add(exp);
                }
            }
            return (result.ToArray(), changed);
        }

        /// <summary>
        /// The expression SEQ(-0<16> - (x != 0<16>), -x) appears a lot in
        /// 16-bit x86 binaries. It amounts to a zero extension of an unsigned 
        /// integer (like size_t) followed by a negation.
        /// </summary>
        private Expression? SignExtendedNegation(MkSequence seq)
        {
            if (seq.Expressions.Length < 2)
                return null;
            var eLast = seq.Expressions[^1];
            if (eLast is UnaryExpression un && un.Operator.Type == OperatorType.Neg)
            {
                var eFirst = seq.Expressions[^2];
                var negated = un.Expression;
                if (eFirst is BinaryExpression bin && 
                    bin.Operator.Type == OperatorType.ISub &&
                    (bin.Left.IsZero ||
                     bin.Left is UnaryExpression un2 && un2.Operator.Type == OperatorType.Neg && un2.Expression.IsZero) &&
                    bin.Right is BinaryExpression binRight &&
                    binRight.Left == negated &&
                    binRight.Right.IsZero)
                {
                    var bitsize = eFirst.DataType.BitSize + eLast.DataType.BitSize;
                    var unsignedInt = PrimitiveType.Create(Domain.UnsignedInt, eLast.DataType.BitSize);
                    var signedInt = PrimitiveType.Create(Domain.SignedInt, bitsize);
                    return m.Neg(m.Convert(negated, unsignedInt, signedInt));
                }
            }
            return null;
        }

        private Expression? FuseAdjacentMemoryAccesses(DataType dt, Expression[] elems)
        {
            var (access, seg, ea, offset) = AsMemoryAccess(elems[0]);
            if (access is null)
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
                    if (bin.Operator.Type == OperatorType.IAdd)
                    {
                        offset = c.ToInt64();
                        eaStripped = bin.Left;
                    }
                    else if (bin.Operator.Type == OperatorType.ISub)
                    {
                        offset = -c.ToInt64();
                        eaStripped = bin.Left;
                    }
                }
            }
            return (access, seg, eaStripped, offset);
        }

        private Expression? FuseAdjacentSlices(DataType dataType, Expression[] elems)
        {
            var fused = new List<Expression> { AsSlice(elems[0]) ?? elems[0] };
            bool changed = false;
            for (int i = 1; i < elems.Length; ++i)
            {
                Slice? slNext = AsSlice(elems[i]);
                if (fused[^1] is Slice slPrev && slNext != null &&
                    cmp.Equals(slPrev.Expression, slNext.Expression) &&
                    slPrev.Offset == slNext.Offset + slNext.DataType.BitSize)
                {
                    // Fuse the two consecutive slices. 
                    var newSlice = new Slice(
                        PrimitiveType.CreateWord(slPrev.DataType.BitSize + slNext.DataType.BitSize),
                        slNext.Expression,
                        slNext.Offset);
                    (fused[^1], _) = newSlice.Accept(this);
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
                    return fused[0];
                else
                    return new MkSequence(dataType, fused.ToArray());
            }
            return null;
        }

        private Expression? FuseSequenceOfSlicedMultiplications(DataType dtSeq, Expression[] seq)
        {
            BinaryExpression? mul = null;

            bool AccumulateMul(BinaryExpression bin)
            {
                if (mul is null)
                    mul = bin;
                else if (!cmp.Equals(mul.Left, bin.Left) ||
                        !cmp.Equals(mul.Right, bin.Right))
                    return false;
                else
                    mul = bin;
                return true;
            }
            int bits = 0;
            for (int i = seq.Length - 1; i >= 0; --i)
            {
                switch (seq[i])
                {
                case Slice slice when
                    slice.Expression is BinaryExpression bin &&
                    bin.Operator is IMulOperator:
                    {
                        if (!AccumulateMul(bin))
                            return null;
                        bits += slice.DataType.BitSize;
                        break;
                    }
                case BinaryExpression bin when
                    bin.Operator is IMulOperator:
                    {
                        if (!AccumulateMul(bin))
                            return null;
                        bits += bin.DataType.BitSize;
                        break;
                    }
                default:
                    return null;
                }
            }
            if (mul is null)
                return null;
            foreach (var e in seq)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(mul);
            if (dtSeq.BitSize < mul.DataType.BitSize)
            {
                return m.Slice(mul, dtSeq, 0);
            }
            return mul;
        }

        private Expression? FuseSequenceOfSlicedMultiplications(Expression[] seq)
        {
            BinaryExpression? mul = null;
            for (int i = seq.Length -1; i >= 0; --i)
            {
                if (seq[i] is Slice slice && 
                    slice.Expression is BinaryExpression bin && 
                    bin.Operator is IMulOperator)
                {
                    if (mul is null)
                        mul = bin;
                    else if (!cmp.Equals(mul.Left, bin.Left) ||
                            !cmp.Equals(mul.Right, bin.Right))
                        return null;
                    else
                        mul = bin;
                }
            }
            if (mul is null)
                return null;
            foreach (var e in seq)
                ctx.RemoveExpressionUse(e);
            ctx.UseExpression(mul);
            return mul;
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

        public Expression? FuseLogicalOperations(DataType dt, Expression[] exps)
        {
            var bins = new BinaryExpression[exps.Length];
            BinaryOperator? opPrev = null;
            for (int i = 0; i < exps.Length; ++i)
            {
                if (exps[i] is BinaryExpression bin)
                {
                    var op = bin.Operator;
                    if ((op.Type == OperatorType.And || op.Type == OperatorType.Or || op.Type == OperatorType.Xor) &&
                        (opPrev is null || opPrev == op))
                    {
                        bins[i] = bin;
                        opPrev = (BinaryOperator)op;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            var lefts = new Expression[bins.Length];
            var rights = new Expression[bins.Length];
            for (int i = 0; i < bins.Length; ++i)
            {
                lefts[i] = bins[i].Left;
                rights[i] = bins[i].Right;
            }
            var (left, _) = new MkSequence(dt, lefts).Accept(this);
            var (right, _) = new MkSequence(dt, rights).Accept(this);
            return new BinaryExpression(opPrev!, dt, left, right);
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
        /// </summary>
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
            var e = scaledIndexRule.Match(offset, ctx);
            if (e is not null)
            {
                changed = true;
                (offset, _) = e.Accept(this);
            }
            if (basePtr is Constant cBase && offset is Constant cOffset)
            {
                var addr = ctx.MakeSegmentedAddress(cBase, cOffset);
                var mem = new MemoryAccess(segMem.MemoryId, addr, segMem.DataType);
                return (ctx.GetValue(mem, segmentMap), true);
            }
            var value = new SegmentedAccess(segMem.MemoryId, basePtr, offset, segMem.DataType);
            e = sliceSegPtr.Match(value, ctx);
            if (e is not null)
            {
                return (e, true);
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
            bool changed;
            var e = slice.Expression;
            if (e is BinaryExpression bin)
            {
                if (CanBeSliced(slice, bin))
                {
                    //$HACK: work around by zero-extending now. The real fix
                    // is to zero-extend carry flags properly.
                    if (TryExtendOperand(bin, out var extBin))
                    {
                        bin = extBin;
                    }
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
            if (e is Application app &&
                app.Procedure is ProcedureConstant pc &&
                pc.Procedure is SimdIntrinsic simd)
            {
                return SliceSimdIntrisic(slice, simd, app);
            }
            (e, changed) = e.Accept(this);

            // Is the slice the same size as the expression?
            if (slice.Offset == 0 && slice.DataType.BitSize == e.DataType.BitSize)
                return (e, true);
            slice = new Slice(slice.DataType, e, slice.Offset);
            e = sliceConst.Match(slice);
            if (e is not null)
            {
                return (e, true);
            }
            e = sliceMem.Match(slice, ctx);
            if (e is not null)
            {
                return (e, true);
            }

            // (slice (shl e n) n) ==> e
            e = sliceShift.Match(slice, ctx);
            if (e is not null)
            {
                return (e, true);
            }
            if (IsSequence(ctx, slice.Expression, out var seq))
            {
                var eNew = SliceSequence(seq, slice.DataType, slice.Offset);
                if (eNew is not null)
                {
                    ctx.RemoveExpressionUse(slice);
                    ctx.UseExpression(eNew);
                    return (eNew, true);
                }
            }

            e = sliceConvert.Match(slice);
            if (e is not null)
            {
                return (e, true);
            }
            var innerSlice = SlicedSlice(slice);
            if (innerSlice is not null)
                return (innerSlice, true);
            if (e is Identifier id &&
                ctx.GetDefiningExpression(id) is MkSequence seq2)
            {
                // If we are slicing a SEQ, and the corresponding element is >= 
                // the size of the slice, then use deposited part directly.
                var lsbElem = seq2.Expressions[^1];
                int sizeDiff = lsbElem.DataType.BitSize - slice.DataType.BitSize;
                if (sizeDiff >= 0)
                {
                    foreach (var elem in seq2.Expressions)
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

        private (Expression e, bool changed) SliceSimdIntrisic(Slice slice, SimdIntrinsic simd, Application app)
        {
            var dtOutputLane = simd.OutputLaneType();
            if (slice.Offset == 0  && //$TODO: even multiple of lane? 
                slice.DataType.BitSize == dtOutputLane.BitSize)
            {
                return (simd.MakeSlice(app.Arguments, 0), true);
            }
            return (slice, false);
        }

        public virtual (Expression, bool) VisitStringConstant(StringConstant str)
        {
            return (str, false);
        }

        private Slice? SlicedSlice(Slice outerSlice)
        {
            if (outerSlice.Expression is Slice innerSlice &&
                innerSlice.DataType.BitSize >= outerSlice.DataType.BitSize)
            {
                var offset = outerSlice.Offset + innerSlice.Offset;
                return m.Slice(innerSlice.Expression, outerSlice.DataType, offset);
            }
            else
            {
                return null;
            }
        }

        private static bool CanBeSliced(Slice slice, BinaryExpression bin)
        {
            return bin.Operator.Type switch
            {
                OperatorType.And or
                OperatorType.Or or
                OperatorType.Xor => true,

                OperatorType.IAdd or 
                OperatorType.ISub or 
                OperatorType.Shl or 
                OperatorType.IMul or 
                OperatorType.UMul or 
                OperatorType.SMul => slice.Offset == 0,

                _ => false,
            };
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
            e = negSub.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            e = LogicalNotComparison(unary);
            if (e is not null)
            {
                return (e, true);
            }
            e = compSub.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            // (!-exp) >= (!exp)
            e = logicalNotFollowedByNeg.Match(unary);
            if (e is not null)
            {
                return (e, true);
            }

            if (unary.Expression is Constant c && c.IsValid && unary.Operator.Type != OperatorType.AddrOf)
            {
                var c2 = unary.Operator.ApplyConstant(c);
                return (c2, true);
            }
            return (unary, changed);
        }

        private static Expression? LogicalNotComparison(UnaryExpression unary)
        {
            if (unary.Operator.Type == OperatorType.Not &&
                unary.Expression is BinaryExpression bin &&
                bin.Operator is ConditionalOperator cond)
            {
                return new BinaryExpression(cond.Invert(), bin.DataType, bin.Left, bin.Right);
            }
            return null;
        }

        public static bool IsSequence(EvaluationContext ctx, Expression e, out MkSequence sequence)
        {
            MkSequence? s;
            if (e is Identifier id)
            {
                s = ctx.GetDefiningExpression(id) as MkSequence;
            }
            else
            {
                s = e as MkSequence;
            }
            sequence = s!;
            return s != null;
        }

        public static Expression? SliceSequence(MkSequence seq, DataType dtSlice, int sliceOffset)
        {
            var bitsUsed = dtSlice.BitSize;
            int bitoffset = 0;
            for (int i = seq.Expressions.Length - 1; i >= 0; --i)
            {
                var elem = seq.Expressions[i];
                var bitsElem = elem.DataType.BitSize;
                var offset = sliceOffset - bitoffset;
                if (0 <= offset && offset + bitsUsed <= bitsElem)
                {
                    var eNew = offset == 0 && bitsUsed == bitsElem
                        ? elem
                        : new Slice(dtSlice, elem, offset);
                    return eNew;
                }
                bitoffset += bitsElem;
            }
            return null;
        }

        private bool TryExtendOperand(
            BinaryExpression bin,
            [MaybeNullWhen(false)] out BinaryExpression extBin)
        {
            if (bin.Left.DataType.BitSize > bin.Right.DataType.BitSize)
            {
                extBin = new BinaryExpression(
                    bin.Operator,
                    bin.DataType,
                    bin.Left,
                    m.Convert(
                        bin.Right,
                        bin.Right.DataType,
                        bin.Left.DataType));
                return true;
            }
            if (bin.Left.DataType.BitSize < bin.Right.DataType.BitSize)
            {
                extBin = new BinaryExpression(
                    bin.Operator,
                    bin.DataType,
                    m.Convert(
                        bin.Left,
                        bin.Left.DataType,
                        bin.Right.DataType),
                    bin.Right);
                return true;
            }
            extBin = null;
            return false;
        }

        static ExpressionSimplifier()
        {
            idConst = new IdConstant();
            constOnLeft = new ComparisonConstOnLeft();
            add2ids = new AddTwoIdsRule();
            addEcc = new Add_e_c_cRule();
            addMici = new Add_mul_id_c_id_Rule();
            idCopyPropagation = new IdCopyPropagationRule();
            idBinIdc = new IdBinIdc_Rule();
            sliceConst = new SliceConstant_Rule();
            sliceMem = new SliceMem_Rule();
            sliceSegPtr = new SliceSegmentedPointer_Rule();
            negSub = new NegSub_Rule();
            compSub = new();
            constConstBin = new ConstConstBin_Rule();
            shAdd = new Shl_add_Rule();
            shMul = new Shl_mul_e_Rule();
            shiftShift = new ShiftShift_c_c_Rule();
            mpsRule = new Mps_Constant_Rule();
            sliceShift = new SliceShift();
            binopWithSelf = new BinOpWithSelf_Rule();
            constDiv = new ConstDivisionImplementedByMultiplication();
            idProcConstRule = new IdProcConstRule();
            convertConvertRule = new ConvertConvertRule();
            distributedConvert = new DistributedConversionRule();
            distributedCast = new DistributedCastRule();
            distributedSlice = new DistributedSliceRule();
            mkSeqFromSlicesRule = new MkSeqFromSlices_Rule();
            sliceConvert = new SliceConvert();
            logicalNotFollowedByNeg = new LogicalNotFollowedByNegRule();
            logicalNotFromBorrow = new LogicalNotFromArithmeticSequenceRule();
            unaryNegEqZero = new UnaryNegEqZeroRule();
            scaledIndexRule = new ScaledIndexRule();
        }
    }
}
