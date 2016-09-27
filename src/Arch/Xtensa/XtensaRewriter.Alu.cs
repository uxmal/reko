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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteCopy()
        {
            var src = RewriteOp(dasm.Current.Operands[1]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            emitter.Assign(dst, src);
        }

        private void RewriteNop()
        {
            emitter.Nop();
        }

        private void RewriteOr()
        {
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var src2 = RewriteOp(dasm.Current.Operands[2]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            emitter.Assign(dst, emitter.Or(src1, src2));
        }

        private void RewriteL32i()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            emitter.Assign(
                dst,
                emitter.LoadDw(
                    emitter.IAdd(
                        RewriteOp(dasm.Current.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)dasm.Current.Operands[2]).Value.ToUInt32()))));
        }

        private void RewriteMovi_n()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var src = Constant.Int32(
                ((ImmediateOperand)this.instr.Operands[1]).Value.ToInt32());
            emitter.Assign(dst, src);
        }

        private void RewriteS32i()
        {
            var src = RewriteOp(dasm.Current.Operands[0]);
            emitter.Assign(
                emitter.LoadDw(
                    emitter.IAdd(
                        RewriteOp(dasm.Current.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)dasm.Current.Operands[2]).Value.ToUInt32()))),
                src);
        }

        private void RewriteSub()
        {
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var src2 = RewriteOp(dasm.Current.Operands[2]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            emitter.Assign(dst, emitter.ISub(src1, src2));
        }
    }
}
