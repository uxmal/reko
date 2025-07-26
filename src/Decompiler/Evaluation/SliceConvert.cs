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
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Evaluation;

/// <summary>
/// Converts a <see cref="Slice"/> expression into <see cref="Conversion"/>
/// expression.
/// </summary>
public class SliceConvert
{
    /// <summary>
    /// Attempts to match a given slice with a specific expression pattern within the provided evaluation context.
    /// </summary>
    /// <param name="slice">The slice to be evaluated, which includes the expression and its offset.</param>
    /// <param name="ctx">The evaluation context used to resolve identifiers and evaluate expressions.</param>
    /// <returns>An <see cref="Expression"/> that represents the matched pattern if successful; otherwise, <see
    /// langword="null"/>.</returns>
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
                return result;
            }
            if (CanSliceConversion(slice, conv))
            {
                result = new Conversion(
                    conv.Expression, conv.SourceDataType, slice.DataType);
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// Check if the sliced expression is a conversion,
    /// or an identifier that is the result of a conversion.
    /// </summary>
    /// <param name="ctx">Evaluation context to use for looking up definitions.</param>
    /// <param name="sliceExp">Expression to check for conversion</param>
    /// <param name="conv">Resulting conversion</param>
    /// <returns></returns>
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
        if (
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