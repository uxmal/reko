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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    public partial class Rewriter
    {
        private void RewriteFmove()
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, MaybeCastFpuArgs);
            emitter.Assign(frame.EnsureIdentifier(Registers.fpsr), emitter.Cond(opDst));
        }

        private void RewriteFBinOp(Func<Expression,Expression,Expression> binOpGen)
        {
            var opSrc = orw.RewriteSrc(di.op1, di.Address);
            var opDst = orw.RewriteDst(di.op2, di.Address, opSrc, binOpGen);
            emitter.Assign(frame.EnsureIdentifier(Registers.fpsr), emitter.Cond(opDst));
        }

        private Expression MaybeCastFpuArgs(Expression src, Expression dst)
        {
            if (src.DataType != dst.DataType)
                return emitter.Cast(dst.DataType, src);
            else
                return src;
        }
    }
}
