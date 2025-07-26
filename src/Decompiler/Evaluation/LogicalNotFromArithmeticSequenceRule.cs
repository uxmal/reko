#region License
/*
 * Copyright (C) 2020-2025 Sven Almgren.
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
using System.Numerics;

namespace Reko.Evaluation;

/// <summary>
/// Rule that matches (0 - (EXPRESSION != 0) + 1) and generates (!EXPRESSION).
/// This is a solution for a very specific sequence of instructions.
/// 
/// Example x86 assembler:
/// <code>
/// neg ax
/// sbb ax, ax
/// inc ax
/// </code>
///
/// Example result:
/// turn <c>func(0x00 - (-MyVariable != 0x00) + 0x01)</c> into <c>func(!MyVariable)</c>
/// (With the help of UnaryNegEqZeroRule)
/// </summary>
internal class LogicalNotFromArithmeticSequenceRule
{
    public Expression? Match(BinaryExpression binExp)
    {
        if (binExp.Operator.Type != OperatorType.IAdd)
            return null;

        //$TODO: use Integer.IsIntegerOne
        if (binExp.Right is BigConstant bigR && bigR.Value != BigInteger.One)
            return null;
        if (binExp.Right is not Constant rightConstant || rightConstant.ToInt64() != 1)
            return null;

        if (binExp.Left is not BinaryExpression leftExpression || 
            leftExpression.Operator.Type != OperatorType.ISub)
            return null;

        if (!leftExpression.Left.IsZero)
            return null;
        var middleExpression = ExtractComparison(leftExpression.Right);
        if (middleExpression is null)
            return null;
        if (!middleExpression.Right.IsZero)
            return null;

        var expression = middleExpression.Left;
        if (expression is UnaryExpression un && un.Operator.Type == OperatorType.Neg)
            expression = un.Expression;

        var dataType = binExp.DataType;
        return new UnaryExpression(Operator.Not, dataType, expression);
    }

    private static BinaryExpression? ExtractComparison(Expression e)
    {
        var bin = e as BinaryExpression;
        if (bin is null)
        {
            if (e is not Conversion conv)
                return null;
            // Accept only conversion to word or integer
            if (!conv.DataType.IsIntegral && !conv.DataType.IsWord)
                return null;
            bin = conv.Expression as BinaryExpression;
            if (bin is null)
                return null;
        }
        if (bin.Operator.Type != OperatorType.Ne)
            return null;
        return bin;
    }
}
