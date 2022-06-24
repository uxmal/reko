#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Arch.Loongson
{
    public partial class LoongArchRewriter
    {
        private void RewriteBinaryFpuOp(Func<Expression, Expression, Expression> fn, bool isReal32)
        {
            var arg1 = Op(1, isReal32);
            var arg2 = Op(2, isReal32);
            FpuAssign(0, fn(arg1, arg2));
        }

        private void RewriteFCopySign(bool isReal32)
        {
            var arg1 = Op(1, isReal32);
            var arg2 = Op(2, isReal32);
            var dt = isReal32
                ? PrimitiveType.Real32
                : PrimitiveType.Real64;
            FpuAssign(0, m.Fn(fcopysign.MakeInstance(dt), arg1, arg2));
        }

        private void RewriteFLoad32(PrimitiveType dt)
        {
            var ea = EffectiveAddress();
            FpuAssign(0, m.Mem(dt, ea));
        }

        private void RewriteFLoadBoundaryCheck(PrimitiveType dt, Func<Expression,Expression, Expression> fn)
        {
            var ea = binder.EnsureRegister((RegisterStorage) instr.Operands[1]);
            var limit = binder.EnsureRegister((RegisterStorage) instr.Operands[2]);
            m.MicroBranch(fn(ea, limit), "skip");
            m.SideEffect(m.Fn(raise_exception, m.Word32(0xA)));
            m.MicroLabel("skip");
            FpuAssign(0, m.Mem(dt, ea));
        }

        private void RewriteUnaryFpuOp(Func<Expression, Expression> fn, bool isReal32)
        {
            var arg1 = Op(1, isReal32);
            FpuAssign(0, fn(arg1));
        }
    }
}
