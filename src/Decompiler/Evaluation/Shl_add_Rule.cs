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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Evaluation
{
    public class Shl_add_Rule
    {
        public Shl_add_Rule()
        {
        }

        // (x << c) + x ==> x * ((1<<c) + 1)
        // (x << c) - x ==> x * ((1<<c) - 1)
        public Expression? Match(BinaryExpression b, EvaluationContext ctx)
        {
            var op = b.Operator;
            int factor;
            if (op.Type == OperatorType.IAdd)
                factor = 1;
            else if (op.Type == OperatorType.ISub)
                factor = -1;
            else
                return null;
            var bin = b.Left as BinaryExpression;
            var id = b.Right as Identifier;
            if (bin is null || id is null)
            {
                bin = b.Right as BinaryExpression;
                id = b.Left as Identifier;
            }
            if (bin is null || bin.Left != id || bin.Operator.Type != OperatorType.Shl)
                return null;
            if (bin.Right is not Constant c)
                return null;
            var dt = b.DataType;
            var cc = Constant.Create(id.DataType, (1 << c.ToInt32()) + factor);
            return new BinaryExpression(Operator.IMul, dt, id, cc);
        }
    }
}
