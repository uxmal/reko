#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Arch.WE32100
{
    public partial class WE32100Rewriter
    {
        private void RewriteArithmetic2(Func<Expression, Expression, Expression> fn, PrimitiveType dt)
        {
            var src = ReadOp(0, dt);
            var dst = WriteBinaryOp(1, dt, src, fn);
            NZVC(dst);
        }

        private void RewriteArithmetic3(Func<Expression, Expression, Expression> fn, PrimitiveType dt)
        {
            var src1 = ReadOp(0, dt);
            var src2 = ReadOp(1, dt);
            var dst = WriteOp(2, dt, fn(src1, src2));
            NZVC(dst);
        }

        private void RewriteLogical2(Func<Expression, Expression, Expression> fn, PrimitiveType dt)
        {
            var src = ReadOp(0, dt);
            var dst = WriteBinaryOp(1, dt, src, fn);
            NZV0(dst);
        }

        private void RewriteLogical3(Func<Expression, Expression, Expression> fn, PrimitiveType dt)
        {
            var src1 = ReadOp(0, dt);
            var src2 = ReadOp(1, dt);
            var dst = WriteOp(2, dt, fn(src1, src2));
            NZV0(dst);
        }

        private void RewriteMov(PrimitiveType dt)
        {
            var src = ReadOp(0, dt);
            var dst = WriteOp(1, dt, src);
            NZV0(dst);
        }

        private void RewriteUnary(
            Func<Expression, Expression> unary,
            PrimitiveType dt, 
            Action<Expression> setcc)
        {
            var src = ReadOp(0, dt);
            var dst = WriteUnaryOp(0, dt, src, unary);
            if (dst is null)
                return;
            setcc(dst);
        }
    }
}
