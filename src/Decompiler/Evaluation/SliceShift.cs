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
using System.Text;

namespace Reko.Evaluation
{
    /// <summary>
    /// Converts expressions (slice (shift x c) c) to x
    /// </summary>
    public class SliceShift
    {
        public SliceShift()
        {
        }

        public Expression? Match(Slice slice, EvaluationContext ctx)
        {
            BinaryExpression? shift;
            var id = slice.Expression as Identifier;
            if (id is not null)
            {
                shift = ctx.GetValue(id) as BinaryExpression;
            }
            else
            {
                shift = slice.Expression as BinaryExpression;
            }
            if (shift is null)
                return null;
            if (shift.Operator != BinaryOperator.Shl)
                return null;
            if (shift.Right is not Constant c)
                return null;
            if (c.ToInt32() != slice.Offset)
                return null;

            var expr = shift.Left;
            var dt = slice.DataType;
            expr.DataType = dt;
            return expr;
        }
    }
}
