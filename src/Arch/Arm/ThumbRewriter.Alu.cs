#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter
    {
        private void RewriteLdr()
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1], PrimitiveType.Word32);
            emitter.Assign(dst, src);
        }

        private void RewriteMov()
        {
            var dst = GetReg(ops[0].RegisterValue.Value);
            var src = GetReg(ops[1].RegisterValue.Value);
            emitter.Assign(dst, src);
        }

        private void RewritePop()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            int offset = ops.Length * 4;
            foreach (var op in ops.OrderByDescending(o => o.RegisterValue.Value))
            {
                offset -= 4;
                emitter.Assign(
                    GetReg(op.RegisterValue.Value),
                    emitter.LoadDw(emitter.IAdd(sp, Constant.Int32(offset))));
            }
            emitter.Assign(sp, emitter.IAdd(sp, Constant.Int32( ops.Length * 4)));
        }

        private void RewritePush()
        {
            var sp = frame.EnsureRegister(A32Registers.sp);
            emitter.Assign(sp, emitter.ISub(sp, Constant.Int32( ops.Length * 4)));
            int offset = 0;
            foreach (var op in ops.OrderBy(o => o.RegisterValue.Value))
            {
                emitter.Assign(
                    emitter.LoadDw(emitter.IAdd(sp, Constant.Int32(offset))),
                    GetReg(op.RegisterValue.Value));
                offset += 4;
            }
        }

        private void RewriteStr()
        {
            var src = RewriteOp(ops[0]);
            var dst = RewriteOp(ops[1], PrimitiveType.Word32);
            emitter.Assign(dst, src);
        }

        private void RewriteBinop(Func<Expression,Expression,Expression> op)
        {
            var dst = RewriteOp(ops[0]);
            var src = RewriteOp(ops[1]);
            emitter.Assign(dst, op(dst, src));
        }
    }
}
