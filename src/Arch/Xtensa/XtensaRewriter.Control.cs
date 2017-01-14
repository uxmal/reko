#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteBall()
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = emitter.Eq0(emitter.And(emitter.Comp(a), b));
            emitter.Branch(
                cond, 
                (Address)RewriteOp(instr.Operands[2]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBany()
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = emitter.Ne0(emitter.And(a, b));
            emitter.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBbx(Func<Expression, Expression> cmp0)
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.Operands[0]);
            var immOp = instr.Operands[1] as ImmediateOperand;
            Expression mask;
            if (immOp != null)
            {
                mask = Constant.Word32(1 << immOp.Value.ToInt32());
            }
            else
            {
                mask = emitter.Shl(Constant.UInt32(1), RewriteOp(instr.Operands[1]));
            }
            emitter.Branch(
                cmp0(emitter.And(src, mask)),
                (Address)RewriteOp(instr.Operands[2]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBnall()
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = emitter.Ne0(emitter.And(emitter.Comp(a), b));
            emitter.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBnone()
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var a = RewriteOp(instr.Operands[0]);
            var b = RewriteOp(instr.Operands[1]);
            var cond = emitter.Eq0(emitter.And(a, b));
            emitter.Branch(
                cond,
                (Address)RewriteOp(instr.Operands[2]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBranch(Func<Expression,Expression,Expression> cmp)
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            emitter.Branch(
                cmp(left, right), 
                (Address)RewriteOp(instr.Operands[2]), 
                RtlClass.ConditionalTransfer);
        }

        private void RewriteBranchZ(Func<Expression, Expression> cmp0)
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var src = RewriteOp(instr.Operands[0]);
            emitter.Branch(
                cmp0(src),
                (Address)RewriteOp(instr.Operands[1]),
                RtlClass.ConditionalTransfer);
        }

        private void RewriteCall0()
        {
            rtlc.Class = RtlClass.Transfer;
            var dst = RewriteOp(instr.Operands[0]);
            var rDst = dst as Identifier;
            if (rDst != null && rDst.Storage == Registers.a0)
            {
                var tmp = frame.CreateTemporary(rDst.DataType);
                emitter.Assign(tmp, dst);
                dst = tmp;
            }
            var cont = instr.Address + instr.Length;
            emitter.Assign(frame.EnsureRegister(Registers.a0), cont);
            emitter.Call(dst, 0);
        }

        private void RewriteJ()
        {
            rtlc.Class = RtlClass.Transfer;
            emitter.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteRet()
        {
            rtlc.Class = RtlClass.Transfer;
            emitter.Return(0, 0);
        }
    }
}