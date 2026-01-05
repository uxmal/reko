#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Evaluation;

/// <summary>
/// Evaluation rule for a comparison operation where the left operand is a constant.
/// </summary>
public class ComparisonConstOnLeft
{
    /// <summary>
    /// Transforms a binary comparsion like
    /// <code>
    /// 3 == x
    /// </code>
    /// to
    /// <code>
    /// x == 3
    /// </code>
    /// </summary>
    /// <param name="bin">Binary expression to test.</param>
    /// <param name="m">Expression emitter.</param>
    /// <returns>A mirrored binary expression or null if this wasn't a comparison.
    /// </returns>
    public Expression? Match(BinaryExpression bin, ExpressionEmitter m)
    {
        if (bin.Operator is not ConditionalOperator cond)
            return null;
        if (bin.Left is not Constant cLeft)
            return null;
        if (bin.Right is Constant)
            return null;
        return m.Bin(
            cond.Mirror(),
            bin.DataType,
            bin.Right,
            cLeft);
    }
}
