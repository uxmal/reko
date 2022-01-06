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
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Evaluation
{
    public class ScaledIndexRule
    {
        private readonly EvaluationContext ctx;
        private BinaryExpression? binEaLeft;
        private BinaryExpression? binEaRight;
        private Identifier? idEa;
        private Expression? defIndex;
        public ScaledIndexRule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Expression ea)
        {
            //$REFACTOR: this is mutable shared state. Rewrite this to
            // use the (Expression newExpr, bool changed) pattern used
            // elsewhere in ExpressionSimplifier. 
            this.binEaLeft = null;
            this.binEaRight = null;
            this.idEa = null;

            if (ea is BinaryExpression bin)
            {
                if (bin.Operator == Operator.IAdd || bin.Operator == Operator.ISub)
                {
                    if (bin.Left is Identifier idLeft)
                    {
                        this.defIndex = ctx.GetDefiningExpression(idLeft);
                        if (IsScaled(defIndex))
                        {
                            this.binEaLeft = bin;
                            return true;
                        }
                    }
                    if (bin.Right is Identifier idRight)
                    {
                        this.defIndex = ctx.GetDefiningExpression(idRight);
                        if (IsScaled(defIndex))
                        {
                            this.binEaRight = bin;
                            return true;
                        }
                    }
                }
            } else if (ea is Identifier id)
            {
                this.defIndex = ctx.GetDefiningExpression(id);
                if (IsScaled(defIndex))
                {
                    idEa = id;
                    return true;
                }
            }
            return false;
        }

        private bool IsScaled(Expression? defIndex)
        {
            return
                (defIndex is BinaryExpression bin &&
                 (bin.Operator is IMulOperator || bin.Operator is ShlOperator));
        }

        public Expression Transform()
        {
            Expression eaNew;
            if (binEaLeft != null)
            {
                ctx.RemoveIdentifierUse((Identifier) binEaLeft.Left);
                eaNew = new BinaryExpression(binEaLeft.Operator, binEaLeft.DataType, defIndex!, binEaLeft.Right);
            }
            else if (binEaRight != null)
            {
                ctx.RemoveIdentifierUse((Identifier) binEaRight.Right);
                eaNew = new BinaryExpression(binEaRight.Operator, binEaRight.DataType, binEaRight.Left, defIndex!);
            }
            else 
            {
                Debug.Assert(idEa != null);
                ctx.RemoveIdentifierUse(idEa);
                eaNew = defIndex!;
            }
            ctx.UseExpression(defIndex!);
            return eaNew;
        }

    }
}
