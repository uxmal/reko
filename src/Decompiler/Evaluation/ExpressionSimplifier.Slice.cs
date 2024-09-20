#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitSlice(Slice slice)
        {
            bool changed;
            var e = slice.Expression;
            // Is the slice the same size as the expression?
            if (slice.Offset == 0 && slice.DataType.BitSize == e.DataType.BitSize)
                return (e, true);
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
            Expression e2;
            (e2, changed) = e.Accept(this);
            e = e2;
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

            e = sliceConvert.Match(slice, ctx);
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
            if (slice.Offset == 0 && //$TODO: even multiple of lane? 
                slice.DataType.BitSize == dtOutputLane.BitSize)
            {
                return (simd.MakeSlice(app.Arguments, 0), true);
            }
            return (slice, false);
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

        /// <summary>
        /// Returns true if a slice may be safely taken of an expression.
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="bin"></param>
        /// <returns></returns>
        private static bool CanBeSliced(Slice slice, BinaryExpression bin)
        {
            var sliceSize = slice.DataType.BitSize;
            return bin.Operator.Type switch
            {
            OperatorType.And or
            OperatorType.Or or
            OperatorType.Xor =>
                bin.Left.DataType.BitSize >= sliceSize &&
                bin.Right.DataType.BitSize >= sliceSize,

            OperatorType.IAdd or
            OperatorType.ISub or
            OperatorType.IMul or
            OperatorType.UMul or
            OperatorType.SMul =>
                slice.Offset == 0 &&
                bin.Left.DataType.BitSize >= sliceSize &&
                bin.Right.DataType.BitSize >= sliceSize,

            OperatorType.Shl =>
                slice.Offset == 0 &&
                bin.Left.DataType.BitSize >= sliceSize,
            _ => false,
            };
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

    }
}
