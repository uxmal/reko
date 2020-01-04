#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Text;

namespace Reko.Evaluation
{
    /// <summary>
    /// Converts expressions (slice (shift x c) c) to x
    /// </summary>
    public class SliceShift
    {
        private EvaluationContext ctx;
        private Expression expr;
        private DataType dt;
        private Identifier id;

        public SliceShift(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(Slice slice)
        {
            BinaryExpression shift;
            id = slice.Expression as Identifier;
            if (id != null)
            {
                shift = ctx.GetValue(id) as BinaryExpression;
            }
            else
            {
                shift = slice.Expression as BinaryExpression;
            }
            if (shift == null)
                return false;
            if (shift.Operator != BinaryOperator.Shl)
                return false;
            Constant c = shift.Right as Constant;
            if (c == null)
                return false;
            if (c.ToInt32() != slice.Offset)
                return false;

            expr = shift.Left;
            dt = slice.DataType;
            return true;
        }

        public Expression Transform()
        {
            if (id != null)
            {
                ctx.RemoveIdentifierUse(id);
                ctx.UseExpression(expr);
            }
            expr.DataType = dt;
            return expr;
        }
    }
}
