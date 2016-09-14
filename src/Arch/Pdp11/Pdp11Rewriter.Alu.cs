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

namespace Reko.Arch.Pdp11
{
    public partial class Pdp11Rewriter
    {
        private void RewriteAdd(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, emitter.IAdd);
            SetFlags(dst, FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF, 0, 0);
        }

        private void RewriteBic(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, (a, b) => emitter.And(a, emitter.Comp(b)));
            SetFlags(dst, FlagM.NF | FlagM.ZF, FlagM.VF, 0);
        }

        private void RewriteBis(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, emitter.Or);
            SetFlags(dst, FlagM.NF | FlagM.ZF, FlagM.VF, 0);
        }

        private void RewriteBit(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, emitter.And);
            SetFlags(dst, FlagM.NF | FlagM.ZF, FlagM.VF, 0);
        }

        private void RewriteClr(Pdp11Instruction instr, Expression src)
        {
            var dst = RewriteDst(instr.op1, src, s => s);
            SetFlags(dst, 0, FlagM.NF | FlagM.CF | FlagM.VF, FlagM.ZF);
        }

        private void RewriteCmp(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteSrc(instr.op2);
            var tmp = frame.CreateTemporary(src.DataType);
            emitter.Assign(tmp, emitter.ISub(dst, src));
            SetFlags(tmp, FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF, 0, 0);
        }

        private void RewriteDec(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op1, src, s => emitter.ISub(s, 1));
            SetFlags(dst, FlagM.NF | FlagM.ZF | FlagM.VF, 0, 0);
        }

        private void RewriteSub(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, emitter.ISub);
            SetFlags(dst, FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF, 0, 0);
        }

        private void RewriteTst(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var tmp = frame.CreateTemporary(src.DataType);
            emitter.Assign(tmp, src);
            emitter.Assign(tmp, emitter.And(tmp, tmp));
            SetFlags(tmp, FlagM.NF | FlagM.ZF, FlagM.VF | FlagM.CF, 0);
        }

        private void RewriteXor(Pdp11Instruction instr)
        {
            var src = RewriteSrc(instr.op1);
            var dst = RewriteDst(instr.op2, src, emitter.Xor);
            SetFlags(dst, FlagM.ZF | FlagM.NF, FlagM.CF | FlagM.VF, 0);
        }
    }
}
