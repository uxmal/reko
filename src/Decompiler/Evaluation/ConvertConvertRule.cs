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

namespace Reko.Evaluation
{
    public class ConvertConvertRule
    {
        public ConvertConvertRule()
        {
        }

        public Expression? Match(Conversion c)
        {
            if (c.Expression is not Conversion innerC)
                return null;
            var origExp = innerC.Expression;
            var innerConv = innerC;

            var ptC = c.DataType as PrimitiveType;
            var ptInner = innerC.DataType as PrimitiveType;
            var ptExp = origExp.DataType as PrimitiveType;
            if (ptC == null || ptInner == null || ptExp == null)
                return null;

            // If the cast is identical, we don't have to do it twice.
            if (ptC == ptInner)
            {
                origExp = innerC;
            }
            else
            {
                // Only match widening / narrowing. 
                if (!ptC.IsWord && !ptInner.IsWord &&
                    (ptC.Domain != ptInner.Domain || ptC.Domain != ptExp.Domain) &&
                     ptExp.Domain != Domain.Boolean)
                    return null;
            }
            
            // ptExp <= ptInner <= ptC
            if (ptExp.BitSize <= ptInner.BitSize && ptInner.BitSize <= ptC.BitSize)
            {
                if (ptExp.BitSize == ptC.BitSize)
                    return origExp;
                else
                    return new Conversion(origExp, innerConv.SourceDataType, ptC);
            }
            return origExp;
        }
    }
}