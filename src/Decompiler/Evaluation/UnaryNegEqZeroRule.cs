#region License
/*
 * Copyright (C) 2020-2022 Sven Almgren.
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
using Reko.Core.Types;
using System;

namespace Reko.Evaluation
{
    /// <summary>
    /// Rule that reduce (-Foo == 0) into (Foo == 0).
    /// </summary>
    public class UnaryNegEqZeroRule
    {
        private Expression? expression;
        private Expression? zero;

        public bool Match(BinaryExpression binExp)
        {
            if (binExp.Operator.Type != OperatorType.Eq)
                return false;

            if (!binExp.Right.IsZero)
                return false;

            if (binExp.Left is not UnaryExpression unary || unary.Operator.Type != OperatorType.Neg)
                return false;

            expression = unary.Expression;
            zero = binExp.Right;

            return true;
        }

        public Expression Transform()
        {
            return new BinaryExpression(Operator.Eq, expression!.DataType, expression!, zero!);
        }
    }
}
