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
using System;

namespace Reko.Evaluation
{
    public class BinOpWithSelf_Rule
    {
        public Expression? Match(BinaryExpression binExp, EvaluationContext ctx)
        {
            var id = binExp.Left as Identifier;
            if (id is null || binExp.Left != binExp.Right)
                return null;

            switch (binExp.Operator.Type)
            {
            case OperatorType.ISub:
            case OperatorType.Xor:
                return Constant.Zero(binExp.DataType);
            case OperatorType.And:
            case OperatorType.Or:
                return id;
            }
            return null;
        }
    }
}
