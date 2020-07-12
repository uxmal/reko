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
            var dst = RewriteOp(instr.Operands[0]);
            var left = RewriteOp(instr.Operands[1]);
            var right = RewriteOp(instr.Operands[2]);
            var result = fn(
                m.Convert(left, left.DataType, dt), 
                m.Convert(right, right.DataType, dt));
            m.Assign(dst, m.Convert(result, result.DataType, dst.DataType));
        }

        private void RewriteFcvt(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var src = RewriteOp(instr.Operands[1]);
            if (src.DataType.BitSize > dtFrom.BitSize)
            {
                src = m.Slice(dtFrom, src, 0);
            }
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, m.Convert(src, dtFrom, dtTo));
        }

        private void RewriteFload(PrimitiveType dt)
        {
            var dst = RewriteOp(instr.Operands[0]);
            Expression ea;
            if (instr.Operands[1] is MemoryOperand mem)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Offset != 0)
                {
                    ea = m.IAddS(ea, mem.Offset);
                }
            }
            else
            {
                //$TODO: once 32-bit loads/stores are fixed, remove
                // all "is MemoryOperand" occurrences and add a
                // MemoryOperand case to RewriteOp.
                ea = RewriteOp(instr.Operands[1]);
                var offset = RewriteOp(instr.Operands[2]);
                if (!offset.IsZero)
                {
                    ea = m.IAdd(ea, offset);
                }
            }
            m.Assign(dst, m.Mem(dt, ea));
        }

        private void RewriteFmadd(PrimitiveType dt, Func<Expression,Expression,Expression> addsub)
        {
            var dst = RewriteOp(instr.Operands[0]);
            var factor1 = RewriteOp(instr.Operands[1]);
            var factor2 = RewriteOp(instr.Operands[2]);
            var summand = RewriteOp(instr.Operands[3]);
            m.Assign(dst, addsub(m.FMul(factor1, factor2), summand));
        }
    }
}
