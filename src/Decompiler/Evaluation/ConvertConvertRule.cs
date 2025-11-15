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

namespace Reko.Evaluation
{
    /// <summary>
    /// Evaluation rule that nested conversions.
    /// </summary>
    public class ConvertConvertRule
    {
        /// <summary>
        /// Match a pair of nested conversions and simplifies it.
        /// </summary>
        public Expression? Match(Conversion c, ExpressionEmitter m)
        {
            if (c.Expression is not Conversion innerC)
                return null;
            var origExp = innerC.Expression;
            var innerConv = innerC;

            var ptOuter = c.DataType as PrimitiveType;
            var ptInner = innerC.DataType as PrimitiveType;
            var ptExp = origExp.DataType as PrimitiveType;
            if (ptOuter is null || ptInner is null || ptExp is null)
                return null;

            // If the cast is identical, we don't have to do it twice.
            if (ptOuter == ptInner)
            {
                return innerC;
            }
            if (ptOuter.Domain == Domain.Real)
            {
                if (ptInner.Domain != Domain.Real)
                {
                    if (innerConv.SourceDataType.BitSize <  ptInner.BitSize)
                    {
                        return m.Convert(origExp, innerConv.SourceDataType, ptOuter);
                    }
                    return null;
                }
                if (ptInner.BitSize > ptOuter.BitSize)
                {
                    if (ptExp.BitSize == ptOuter.BitSize)
                        return origExp;
                    return m.Convert(origExp, innerC.SourceDataType, ptOuter);
                }

                if (ptExp.BitSize == ptOuter.BitSize)
                {
                    if (ptInner.BitSize == ptOuter.BitSize)
                        return origExp;
                    return null;
                }
                return m.Convert(origExp, innerConv.SourceDataType, ptOuter);
            }

            // ptExp <= ptInner <= ptC
            if (ptExp.BitSize <= ptInner.BitSize && ptInner.BitSize <= ptOuter.BitSize)
            {
                if (ptExp.BitSize == ptOuter.BitSize)
                    return origExp;
                else
                    return m.Convert(origExp, innerConv.SourceDataType, ptOuter);
            }
            return null;
        }
    }
}