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
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter
    {
        private void RewriteFcpy()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            m.Assign(dst, src);
        }

        private void RewriteFid()
        {
            m.SideEffect(host.PseudoProcedure("__fid", VoidType.Instance));
        }

        private void RewriteFld(PrimitiveType dt)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            ((MemoryAccess) src).DataType = dt;
            m.Assign(dst, src);
        }

        private void RewriteFpArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src1 = RewriteOp(instr.Operands[0]);
            var src2 = RewriteOp(instr.Operands[1]);
            var dst = RewriteOp(instr.Operands[2]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteFst(PrimitiveType dt)
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = RewriteOp(instr.Operands[1]);
            ((MemoryAccess) dst).DataType = dt;
            m.Assign(dst, src);
        }
    }
}
