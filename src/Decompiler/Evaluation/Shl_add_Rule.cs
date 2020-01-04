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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Evaluation
{
    public class Shl_add_Rule
    {
        private EvaluationContext ctx;
        private DataType dt;
        private Identifier id;
        private Constant c;
        private Operator op;

        public Shl_add_Rule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        // (x << c) + x ==> x * ((1<<c) + 1)
        public bool Match(BinaryExpression b)
        {
            this.op = b.Operator;
            if (op != Operator.IAdd)
                return false;
            var bin = b.Left as BinaryExpression;
            id = b.Right as Identifier;
            if (bin == null || id == null)
            {
                bin = b.Right as BinaryExpression;
                id = b.Left as Identifier;
            }
            if (bin == null || bin.Left != id || bin.Operator != Operator.Shl)
                return false;
            this.c = bin.Right as Constant;
            this.dt = b.DataType;
            return c != null;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(id);
            var cc = Constant.Create(id.DataType, (1 << c.ToInt32()) + 1);
            return new BinaryExpression(Operator.IMul, dt, id, cc);
        }
    }
}
