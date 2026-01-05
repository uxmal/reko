#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            m.Assign(dst, src);
        }

        private void RewriteFid()
        {
            m.SideEffect(m.Fn(fid_intrinsic));
        }

        private void RewriteFcnv()
        {

        }

        private void RewriteFcnvxf()
        {
            var src = RewriteOp(0);
            if (instr.FpFmt == FpFormat.w)
            {
                var t = binder.CreateTemporary(PrimitiveType.Int32);
                if (instr.FpFmtDst == FpFormat.dbl)
                {
                    m.Assign(t, m.Slice(src, t.DataType, 0));
                    m.Assign(RewriteOp(1), m.Convert(t, t.DataType, PrimitiveType.Real64));
                    return;
                }
            }
            EmitUnitTest();
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private void RewriteFld(PrimitiveType dt)
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            ((MemoryAccess) src).DataType = dt;
            m.Assign(dst, src);
        }

        private void RewriteFpArithmetic(Func<Expression, Expression, Expression> fn)
        {
            var src1 = RewriteOp(0);
            var src2 = RewriteOp(1);
            var dst = RewriteOp(2);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteFst(PrimitiveType dt)
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            ((MemoryAccess) dst).DataType = dt;
            m.Assign(dst, src);
        }
    }
}
