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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86.Rewriter;

public partial class X86Rewriter
{
    private void RewriteKandn(PrimitiveType dt)
    {
        var src1 = SrcOp(1);
        var src2 = SrcOp(2);
        var dst = SrcOp(0);
        var andn = m.And(
            MaybeSlice(dt, src1),
            m.Comp(MaybeSlice(dt, src2)));
        m.Assign(dst, MaybeConvert(dst.DataType, andn));
    }

    private void RewriteKbinop(BinaryOperator op, PrimitiveType dt)
    {
        var src1 = SrcOp(1);
        var src2 = SrcOp(2);
        var dst = SrcOp(0);
        m.Assign(dst, MaybeConvert(dst.DataType, m.Bin(op, MaybeSlice(dt, src1), MaybeSlice(dt, src2))));
    }

    private void RewriteKmov(PrimitiveType dt)
    {
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, MaybeSlice(dt, SrcOp(1)));
        var dst = SrcOp(0);
        m.Assign(dst, MaybeConvert(dst.DataType, tmp));
    }

    private void RewriteKunpack(PrimitiveType dtFrom, PrimitiveType dtTo)
    {
        var aFrom = new ArrayType(dtFrom, 2);
        var aTo = new ArrayType(dtTo, 2);
        var src1 = SrcOp(1);
        var src2 = SrcOp(2);
        var dst = SrcOp(0);
        m.Assign(SrcOp(0),
            MaybeConvert(dst.DataType, m.Fn(
                kunpack_intrinsic.MakeInstance(aFrom, aTo),
                MaybeSlice(aFrom, src1),
                MaybeSlice(aFrom, src2))));
    }
}
