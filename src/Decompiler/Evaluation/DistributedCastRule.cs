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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;

namespace Reko.Evaluation
{
    public class DistributedCastRule
    {
        private DataType dt;
        private Expression eLeft;
        private Expression eRight;
        private Operator op;

        public DistributedCastRule()
        {
        }

        public bool Match(BinaryExpression binExp)
        {
            if (binExp.Operator == Operator.IAdd || binExp.Operator == Operator.ISub)
            {
                if (binExp.Left is Cast cLeft && binExp.Right is Cast cRight)
                {
                    if (cLeft.DataType == cRight.DataType)
                    {
                        this.dt = cLeft.DataType;
                        this.eLeft = cLeft.Expression;
                        this.eRight = cRight.Expression;
                        this.op = binExp.Operator;
                        return true;
                    }
                }
            }
            return false;
        }

        public Expression Transform(EvaluationContext ctx)
        {
            return new Cast(dt, new BinaryExpression(
                this.op, this.dt, this.eLeft, this.eRight));
        }
    }
}
