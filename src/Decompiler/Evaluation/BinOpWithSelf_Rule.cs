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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;

namespace Reko.Evaluation
{
    public class BinOpWithSelf_Rule
    {
        private BinaryExpression binExp;
        private Identifier id;

        public bool Match(BinaryExpression binExp)
        {
            if (binExp.Operator != Operator.ISub &&
                binExp.Operator != Operator.Xor && 
                binExp.Operator != Operator.And &&
                binExp.Operator != Operator.Or)
                return false;
            this.binExp = binExp;
            id = binExp.Left as Identifier;
            return (id != null && binExp.Left == binExp.Right);
        }

        public Expression Transform(EvaluationContext ctx)
        {
            if (binExp.Operator == Operator.ISub || binExp.Operator == Operator.Xor)
            {
                ctx.RemoveIdentifierUse(id);
                ctx.RemoveIdentifierUse(id);
                return Constant.Zero(binExp.DataType);
            }
            if (binExp.Operator == Operator.And || binExp.Operator == Operator.Or)
            {
                ctx.RemoveIdentifierUse(id);
                return id;
            }
            throw new NotImplementedException();
         }
    }
}
