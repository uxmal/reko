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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Evaluation
{
    public class SliceConvert
    {
        public SliceConvert()
        {
        }

        public Expression? Match(Slice slice, EvaluationContext ctx)
        {

            if (slice.Offset != 0)
            {
                var slicedExp = slice.Expression;
                if (slicedExp is Identifier id)
                {
                    slicedExp = ctx.GetDefiningExpression(id);
                }
                // We might be slicing a5 zero extension.
                if (slicedExp is Conversion slConv &&
                    slice.Offset >= slConv.SourceDataType.BitSize &&
                        IsUselessIntegralExtension(slice, slConv))
                {
                    ctx.RemoveExpressionUse(slice.Expression);
                    return Constant.Zero(slice.DataType);
                }
                return null;
            }
            Expression result;
            if (IsApplicableConversion(ctx, slice.Expression, out Conversion? conv))
            {
                if (IsUselessIntegralExtension(slice, conv))
                {
                    result = slice.DataType.BitSize == conv.SourceDataType.BitSize
                        ? conv.Expression
                        : new Slice(slice.DataType, conv.Expression, 0);
                    ctx.RemoveExpressionUse(slice);
                    ctx.UseExpression(result);
                    return result;
                }
                if (CanSliceConversion(slice, conv))
                {
                    result = new Conversion(
                        conv.Expression, conv.SourceDataType, slice.DataType);
                    ctx.RemoveExpressionUse(slice);
                    ctx.UseExpression(result);
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// A conversion is applicable if 
        /// </summary>
        /// <param name="conv"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool IsApplicableConversion(
            EvaluationContext ctx,
            Expression sliceExp,
            [MaybeNullWhen(false)] out Conversion conv)
        {
            if (sliceExp is Identifier id)
            {
                var def = ctx.GetDefiningExpression(id);
                if (def is Conversion c2 &&
                    c2.Expression is Identifier)
                {
                    conv = c2;
                    return true;
                }
            }
            else if (sliceExp is Conversion c)
            {
                conv = c;
                return true;
            }
            conv = null;
            return false;
        }

        private static bool CanSliceConversion(Slice slice, Conversion conv)
        {
            if (IsSliceable(slice.DataType) &&
                IsSliceable(conv.DataType))
            {
                return true;
            }
            return false;
        }

        private static bool IsUselessIntegralExtension(Slice slice, Conversion conv)
        {
            if (IsSliceable(slice.DataType) &&
                IsSliceable(conv.DataType) &&
                IsSliceable(conv.SourceDataType))
            {
                return
                    slice.DataType.BitSize <= conv.SourceDataType.BitSize;
            }
            else return false;
        }

        private static bool IsSliceable(DataType dataType)
        {
            if (dataType.IsWord || dataType.IsIntegral)
                return true;
            return (dataType.Domain.HasFlag(Domain.Character));
        }
    }
}