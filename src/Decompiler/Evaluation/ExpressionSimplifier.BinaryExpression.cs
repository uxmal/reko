#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitBinaryExpression(BinaryExpression binExp)
        {
            // BinaryExpressions are the most common and occur in clusters that sometimes
            // are so deep that attempting to simplify using recursion will cause a stack
            // overflow. The code below traverses a tree of BinaryExpressions iteratively
            // using an explicit stack to keep track of intermediate results.
            var stack = new Stack<(BinaryExpression, (Expression, bool)[])>();
            stack.Push((binExp, new (Expression, bool)[2]));
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
            (Expression, bool)[] simpExps)
        {
            var (left, lChanged) = simpExps[0];
            var (right, rChanged) = simpExps[1];
            bool changed = lChanged | rChanged;
            Constant? cLeft = left as Constant;
            Constant? cRight = right as Constant;
            if (cLeft is not null && binExp.Operator.Type.Commutes())
            {
                (cLeft, cRight) = (cRight, cLeft);
                (left, right) =  (right, cRight);
            }
            Expression? e;
            //$TODO: operands to binary operations appear to be
            // mismatched in some processors. Change the ctor
            // of BinaryExpression to catch this later.
            var sameBitsize = left.DataType.BitSize == right!.DataType.BitSize;
            if (cRight is not null)
            {
                switch (binExp.Operator.Type)
                {
                case OperatorType.IAdd:
                case OperatorType.ISub:
                    // (- X 0) ==> X
                    // (+ X 0) ==> X
                    if (cRight.IsIntegerZero)
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
                    else if (!cRight.IsReal && cRight.IsIntegerOne)
                    {
                        if (left is UnaryExpression u &&
                            u.Operator.Type == OperatorType.Comp)
                        {
                            (e, _) = m.Neg(u.Expression).Accept(this);
                            return (e, true);
                        }
                    }
                    break;
                case OperatorType.Or:

                    if (cRight.IsIntegerZero)
                    {
                        return (left, true);
                    }
                    // (| X 0xFFFF...F) ==> 0xFFFF...F
                    if (cRight.IsMaxUnsigned && sameBitsize && !CriticalInstruction.IsCritical(left))
                    {
                        return (right, true);
                    }
                    break;
                case OperatorType.And:
                    if (cRight.IsIntegerZero && sameBitsize && !CriticalInstruction.IsCritical(left))
                    {
                        return (cRight, true);
                    }
                    if (cRight.IsMaxUnsigned && sameBitsize)
                    {
                        return (left, true);
                    }
                    break;
                case OperatorType.Xor:
                    if (cRight.IsIntegerZero)
                    {
                        return (left, true);
                    }
                    if (cRight.IsMaxUnsigned && sameBitsize)
                    {
                        (e, _) = m.Unary(Operator.Comp, left.DataType, left).Accept(this);
                        return (e, true);
                    }
                    break;

                case OperatorType.IMul:
                case OperatorType.SMul:
                case OperatorType.UMul:
                    if (cRight.IsIntegerOne && cLeft is null)
                    {
                        if (binExp.DataType.BitSize == left.DataType.BitSize)
                        {
                            return (left, true);
                        }
                        else
                        {
                            return (m.Convert(left, left.DataType, binExp.DataType), true);
                        }
                    }
                    break;
                case OperatorType.Eq:
                    // x == false ==> !x
                    // x == true ==>  x
                    if (cRight.DataType.BitSize == 1)
                    {
                        if (cRight.IsIntegerZero)
                            return (m.Not(left), true);
                        else
                            return (left, true);
                    }
                    break;
                case OperatorType.Ne:
                    // x != false ==> x
                    // x != true ==> !x
                    if (cRight.DataType.BitSize == 1)
                    {
                        if (!cRight.IsIntegerZero)
                            return (m.Not(left), true);
                        else
                            return (left, true);
                    }
                    break;
                }
            }

            //$REVIEW: this is evaluation! Shouldn't the be done by the evaluator?
            if (left is InvalidConstant || right is InvalidConstant)
                return (InvalidConstant.Create(binExp.DataType), lChanged | rChanged);

            binExp = m.Bin(binExp.Operator, binExp.DataType, left, right);
            e = constConstBin.Match(binExp);
            if (e is not null)
            {
                return (e, true);
            }

            Identifier? idLeft = left as Identifier;

            // (rel? id1 c) should just pass.

            if (binExp.Operator.Type.IsIntComparison() && cRight is not null && idLeft is not null)
                return (binExp, changed);

            // Floating point expressions with "integer" constants 
            if (binExp.Operator.Type.IsFloatComparison() && IsNonFloatConstant(cRight))
            {
                cRight = ctx.ReinterpretAsFloat(cRight!);
                right = cRight;
                changed = true;
                binExp = m.Bin(
                    binExp.Operator,
                    binExp.DataType,
                    binExp.Left,
                    cRight);
            }

            var binLeft = left as BinaryExpression;
            var cLeftRight = (binLeft is not null) ? binLeft.Right as Constant : null;

            // (+ (+ e c1) c2) ==> (+ e (+ c1 c2))
            // (+ (- e c1) c2) ==> (+ e (- c2 c1))
            // (- (+ e c1) c2) ==> (- e (- c2 c1))
            // (- (- e c1) c2) ==> (- e (+ c1 c2))

            if (binLeft is not null && cLeftRight is not null && cRight is not null)
            {
                if (binExp.Operator.Type.IsAddOrSub() &&
                    binLeft.Operator.Type.IsAddOrSub() &&
                    !cLeftRight.IsReal && !cRight.IsReal)
                {
                    var binOperator = binExp.Operator;
                    Constant c;
                    if (binLeft.Operator == binOperator)
                    {
                        c = Operator.IAdd.ApplyConstants(binExp.DataType, cLeftRight, cRight);
                    }
                    else
                    {
                        if (cRight.ToInt64() != long.MinValue &&
                            Math.Abs(cRight.ToInt64()) >= Math.Abs(cLeftRight.ToInt64()))
                        {
                            c = Operator.ISub.ApplyConstants(binExp.DataType, cRight, cLeftRight);
                        }
                        else
                        {
                            binOperator =
                                binOperator.Type == OperatorType.IAdd
                                    ? Operator.ISub
                                    : Operator.IAdd;
                            c = Operator.ISub.ApplyConstants(binExp.DataType, cLeftRight, cRight);
                        }
                    }
                    if (c.IsIntegerZero)
                        return (binLeft.Left, true);
                    return (m.Bin(binOperator, binExp.DataType, binLeft.Left, c), true);
                }
                if (binExp.Operator.Type == OperatorType.IMul && binLeft.Operator.Type == OperatorType.IMul)
                {
                    var c = Operator.IMul.ApplyConstants(binExp.DataType, cLeftRight, cRight);
                    if (c.IsIntegerZero && !CriticalInstruction.IsCritical(binLeft))
                    {
                        return (c, true);
                    }
                    else
                    {
                        return (m.Bin(binExp.Operator, binExp.DataType, binLeft.Left, c), true);
                    }
                }
                if (binExp.Operator.Type == OperatorType.And &&
                    binLeft.Operator.Type == OperatorType.And)
                {
                    return (m.And(
                        binLeft.Left,
                        Operator.And.ApplyConstants(
                            binExp.DataType,
                            cLeftRight,
                            cRight)),
                        true);
                }
            }

            // (rel (- c e) 0 => (rel -c e) => (rel.Negate e c)

            if (binLeft is not null && cRight is not null && cRight.IsIntegerZero &&
                binExp.Operator.Type.IsIntComparison() &&
                binLeft.Left is Constant cBinLeft)
            {
                if (binLeft.Operator.Type == OperatorType.ISub)
                {
                    return (m.Bin(
                        (BinaryOperator)((ConditionalOperator) binExp.Operator).Mirror(),
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

            if (binExp.Operator.Type.IsIntComparison() &&
                binLeft is not null && cLeftRight is not null && cRight is not null &&
                !cLeftRight.IsReal && !cRight.IsReal)
            {
                if (binLeft.Operator.Type.IsAddOrSub())
                {
                    // (>u (- e c1) c2) => (>u e c1+c2) || (<u e c2)
                    if (binExp.Operator.Type == OperatorType.Ugt &&
                        binLeft.Operator.Type == OperatorType.ISub &&
                        !cRight.IsIntegerZero)
                    {
                        var c = Operator.IAdd.ApplyConstants(binExp.DataType, cLeftRight, cRight);
                        return (m.Cor(
                            new BinaryExpression(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c),
                            m.Ult(binLeft.Left, cLeftRight)),
                            true);
                    }
                    else
                    {
                        var op = binLeft.Operator.Type == OperatorType.IAdd ? Operator.ISub : Operator.IAdd;
                        var c = op.ApplyConstants(binExp.DataType, cRight, cLeftRight);
                        return (m.Bin(binExp.Operator, PrimitiveType.Bool, binLeft.Left, c), true);
                    }
                }
                else if (binLeft.Operator.Type == OperatorType.USub)
                {
                    var op = binLeft.Operator.Type == OperatorType.IAdd ? Operator.ISub : Operator.IAdd;
                    var c = op.ApplyConstants(binExp.DataType, cLeftRight, cRight);
                    var opCmp = ((ConditionalOperator) binExp.Operator).ToUnsigned();
                    return (m.Bin(opCmp, PrimitiveType.Bool, binLeft.Left, c), true);
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
            if (binExp.Operator.Type.IsIntComparison() &&
                binLeft is not null &&
                binLeft.Operator.Type == OperatorType.ISub &&
                right.IsZero)
            {
                e = m.Bin(binExp.Operator, binExp.DataType,
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
            if (eNew is not null)
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
            (BinaryExpression?, Slice?, Slice?) MatchOr(Expression left, Expression right)
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
                return m.Bin(
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
            // ((id+/-e)+/-id) ==> ((id+/-id)+/-e)
            // ((e+/-id)+/-id) ==> (e+/(id+/-id))
            if (binExp.Operator.Type.IsAddOrSub() &&
                binExp.Left is BinaryExpression binLeft &&
                binLeft.Operator.Type.IsAddOrSub() &&
                binExp.Right is Identifier idRight)
            {
                if (binLeft.Left is Identifier idLeftLeft &&
                    idLeftLeft == idRight)
                {
                    binExp = m.Bin(
                        binLeft.Operator, binExp.DataType,
                        m.Bin(
                            binExp.Operator, binLeft.DataType,
                            binLeft.Left, binExp.Right),
                        binLeft.Right);
                    return (binExp, true);
                }
                if (binLeft.Right is Identifier idLeftRight &&
                    idLeftRight == idRight)
                {
                    if (
                        binLeft.Operator.Type.Commutes() ==
                        binExp.Operator.Type.Commutes())
                    {
                        // ((e+id)+id) ==> (e+id*2)
                        // ((e-id)-id) ==> (e-id*2)
                        binExp = m.Bin(
                            binLeft.Operator, binExp.DataType,
                            binLeft.Left,
                            m.IMul(idRight, 2));
                        return (binExp, true);
                    }
                    else
                    {
                        // ((e+id)-id) ==> e
                        // ((e-id)+id) ==> e
                        return (binLeft.Left, true);
                    }
                }
            }
            // (+ id1 id1) ==> (* id1 2)
            var e = add2ids.Match(binExp, ctx);
            if (e is not null)
            {
                (e, _) = e.Accept(this);
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

        private static bool IsNonFloatConstant(Constant? cRight)
        {
            return cRight is not null && !cRight.DataType.IsReal;
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
                    var sig = new FunctionType(
                        [
                            new Identifier("x", binInner.Left.DataType, null!),
                            new Identifier("y", PrimitiveType.Int32, null!)
                        ] ,
                        [
                            new Identifier("", bin.DataType, null!)
                        ]);
                    var align = new IntrinsicProcedure(IntrinsicProcedure.Align, false, sig);
                    return m.Fn(align, binInner.Left, Constant.Int32(1 << cRight.ToInt32()));
                }
            }
            return null;
        }
    }
}
