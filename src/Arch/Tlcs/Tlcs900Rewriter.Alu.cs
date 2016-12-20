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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs
{
    public partial class Tlcs900Rewriter
    {
        private void RewriteBinOp(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteDaa(string flags)
        {
            var src = RewriteSrc(this.instr.op1);
            var fn = host.PseudoProcedure("__daa", PrimitiveType.Byte, src);
            m.Assign(src, fn);
            EmitCc(src, flags);
        }

        private void RewriteIncDec(Func<Expression, Expression, Expression> fn, string flags)
        {
            var src = RewriteSrc(this.instr.op1);
            var dst = RewriteDst(this.instr.op2, src, fn);
            EmitCc(dst, flags);
        }

        private void RewriteLd()
        {
            var src = RewriteSrc(this.instr.op2);
            var dst = RewriteDst(this.instr.op1, src, (d, s) => s);
        }
    }
}
