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

using Reko.Core.Expressions;
using Reko.Core.Operators;

namespace Reko.Evaluation;

internal class DistributedSliceRule
{
    public Expression? Match(BinaryExpression binExp, ExpressionEmitter m)
    {
        if (binExp.Operator.Type.IsAddOrSub())
        {
            if (binExp.Left is Slice slLeft && binExp.Right is Slice slRight)
            {
                if (slLeft.DataType == slRight.DataType && 
                    slLeft.Offset == slRight.Offset)
                {
                    var dt = slLeft.DataType;
                    var eLeft = slLeft.Expression;
                    var eRight = slRight.Expression;
                    var op = binExp.Operator;
                    return m.Slice(
                        m.Bin(
                            op,
                            eLeft.DataType,
                            eLeft,
                            eRight),
                        dt,
                        slLeft.Offset);
                }
            }
        }
        return null;
    }
}
