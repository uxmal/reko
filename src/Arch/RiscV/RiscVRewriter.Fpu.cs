#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteFcmp(PrimitiveType dt, Func<Expression, Expression,Expression> fn)
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            m.Assign(dst, m.Cast(dst.DataType, fn(m.Cast(dt, left), m.Cast(dt, right))));
        }

        private void RewriteFcvt(PrimitiveType dt)
        {
            var dst = RewriteOp(instr.op1);
            var src = RewriteOp(instr.op2);
            m.Assign(dst, m.Cast(dt, src));
        }

        private void RewriteFload(PrimitiveType dt)
        {
            var dst = RewriteOp(instr.op1);
            var ea = RewriteOp(instr.op2);
            var offset = RewriteOp(instr.op3);
            if (!offset.IsZero)
            {
                ea = m.IAdd(ea, offset);
            }
            m.Assign(dst, m.Load(dt, ea));
        }

        private void RewriteFmadd(PrimitiveType dt, Func<Expression,Expression,Expression> addsub)
        {
            var dst = RewriteOp(instr.op1);
            var factor1 = RewriteOp(instr.op2);
            var factor2 = RewriteOp(instr.op3);
            var summand = RewriteOp(instr.op4);
            m.Assign(dst, addsub(m.FMul(factor1, factor2), summand));
        }
    }
}
