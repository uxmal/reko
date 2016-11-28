#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteAdd()
        {
            var dst = RewriteOp(instr.op1);
            var src1 = RewriteOp(instr.op2);
            var src2 = RewriteOp(instr.op3);
            Expression src;
            if (src1.IsZero)
            {
               src = src2;
            } else if (src2.IsZero)
            {
                src = src1;
            }
            else
            {
                src = m.IAdd(src1, src2);
            }
            m.Assign(dst, src);
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(instr.op1);
            var src1 = RewriteOp(instr.op2);
            var src2 = RewriteOp(instr.op3);
            m.Assign(dst, m.And(src1, src2));
        }

        private void RewriteLoad(DataType dt)
        {
            var dst = RewriteOp(instr.op1);
            var baseReg = RewriteOp(instr.op2);
            var offset = ((ImmediateOperand)instr.op3).Value;
            Expression ea = baseReg;
            if (!offset.IsZero)
            {
                ea = m.IAdd(ea, offset);
            }
            m.Assign(dst, m.Load(dt, ea));
        }

        private void RewriteStore(DataType dt)
        {
            var src = RewriteOp(instr.op1);
            var baseReg = RewriteOp(instr.op2);
            var offset = ((ImmediateOperand)instr.op3).Value;
            Expression ea = baseReg;
            if (!offset.IsZero)
            {
                ea = m.IAdd(ea, offset);
            }
            m.Assign(m.Load(dt, ea), src);
        }
    }
}
