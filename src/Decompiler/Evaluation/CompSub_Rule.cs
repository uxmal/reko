#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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

namespace Reko.Evaluation;

/// <summary>
/// Evaluation rule for complemented subtractions.
/// <code>
/// ~(a - b) -> ((b - a) - 1).
/// </code>
/// </summary>
public class CompSub_Rule
{
    /// <summary>
    /// Match a complemented subtraction operation and simplifies it.
    /// </summary>
    /// <param name="unary">Unary expression to simplify.
    /// </param>
    /// <param name="m">Expression emitter used when building expressions.</param>
    /// <returns></returns>
    public Expression? Match(UnaryExpression unary, ExpressionEmitter m)
    {
        if (unary.Operator == Operator.Comp &&
            unary.Expression is BinaryExpression bin &&
            bin.Operator == Operator.ISub)
        {
            var one = Constant.Int(bin.DataType, 1);
            return m.Bin(
                Operator.ISub,
                bin.DataType,
                m.Bin(
                    Operator.ISub, bin.DataType, bin.Right, bin.Left),
                one);
        }
        return null;
    }
}
