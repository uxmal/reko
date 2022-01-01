#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        private EvaluationContext ctx;
        private Expression? origExp;
        private Conversion? innerConv;
        private PrimitiveType? ptC;
        private PrimitiveType? ptInner;
        private PrimitiveType? ptExp;

        public ConvertConvertRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Conversion c)
        {
            if (c.Expression is not Conversion innerC)
                return false;
            this.origExp = innerC.Expression;
            this.innerConv = innerC;

            this.ptC = c.DataType as PrimitiveType;
            this.ptInner = innerC.DataType as PrimitiveType;
            this.ptExp = origExp.DataType as PrimitiveType;
            if (ptC == null || ptInner == null || ptExp == null)
                return false;

            // If the cast is identical, we don't have to do it twice.
            if (ptC == ptInner)
            {
                this.origExp = innerC;
                return true;
            }
            // Only match widening / narrowing. 
            return ptC.IsWord || ptInner.IsWord ||
                (ptC.Domain == ptInner.Domain && ptC.Domain == ptExp.Domain);
        }

        public Expression Transform()
        {
            // ptExp <= ptInner <= ptC
            if (ptExp!.BitSize <= ptInner!.BitSize && ptInner.BitSize <= ptC!.BitSize)
            {
                if (ptExp.BitSize == ptC.BitSize)
                    return this.origExp!;
                else
                    return new Conversion(this.origExp!, this.innerConv!.SourceDataType, ptC);
            }
            return this.origExp!;
        }
    }
}