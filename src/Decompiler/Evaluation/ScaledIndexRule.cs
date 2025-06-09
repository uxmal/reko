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
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Evaluation
{
    public class ScaledIndexRule
    {
        public ScaledIndexRule()
        {
        }

        public Expression? Match(Expression ea, EvaluationContext ctx)
        {
            BinaryExpression? binEaLeft = null;
            BinaryExpression? binEaRight = null;
            Identifier? idEa = null;
            Expression? defIndex;

            if (ea is BinaryExpression bin)
            {
                if (bin.Operator.Type.IsAddOrSub())
                {
                    if (bin.Left is Identifier idLeft)
                    {
                        defIndex = ctx.GetDefiningExpression(idLeft);
                        if (IsScaled(defIndex))
                        {
                            binEaLeft = bin;
                            return Transform(binEaLeft, binEaRight, idEa, defIndex, ctx);
                        }
                    }
                    if (bin.Right is Identifier idRight)
                    {
                        defIndex = ctx.GetDefiningExpression(idRight);
                        if (IsScaled(defIndex))
                        {
                            return Transform(binEaLeft, bin, idEa, defIndex, ctx);
                        }
                    }
                }
            } else if (ea is Identifier id)
            {
                defIndex = ctx.GetDefiningExpression(id);
                if (IsScaled(defIndex))
                {
                    return Transform(binEaLeft, binEaRight, id, defIndex, ctx);
                }
            }
            return null;
        }

        private bool IsScaled(Expression? defIndex)
        {
            return
                (defIndex is BinaryExpression bin &&
                 (bin.Operator is IMulOperator || bin.Operator is ShlOperator));
        }

        public Expression Transform(
            BinaryExpression? binEaLeft,
            BinaryExpression? binEaRight,
            Identifier? idEa,
            Expression? defIndex,
            EvaluationContext ctx)
        {
            //$REFACTOR: this can be reduced into the Match method
            Expression eaNew;
            if (binEaLeft is not null)
            {
                eaNew = new BinaryExpression(binEaLeft.Operator, binEaLeft.DataType, defIndex!, binEaLeft.Right);
            }
            else if (binEaRight is not null)
            {
                eaNew = new BinaryExpression(binEaRight.Operator, binEaRight.DataType, binEaRight.Left, defIndex!);
            }
            else 
            {
                Debug.Assert(idEa is not null);
                eaNew = defIndex!;
            }
            return eaNew;
        }

    }
}
