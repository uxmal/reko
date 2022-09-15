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
    /// Rule that reduces (!-Foo) into (!Foo).
    /// </summary>
    class LogicalNotFollowedByNegRule
    {
        private DataType? dataType;
        private Expression? expression;

        public bool Match(UnaryExpression unary)
        {
            if (unary.Operator.Type != OperatorType.Not)
                return false;

            if (!(unary.Expression is UnaryExpression subExpression) || subExpression.Operator.Type != OperatorType.Neg)
                return false;

            dataType = unary.DataType;
            expression = subExpression.Expression;
         
            return true;
        }

        public Expression Transform()
        {
            return new UnaryExpression(Operator.Not, dataType!, expression!);
        }
    }
}
