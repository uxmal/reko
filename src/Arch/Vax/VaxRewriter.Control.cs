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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter
    {
        private void RewriteAcbf(PrimitiveType width)
        {
            var limit = RewriteSrcOp(0, width);
            var add = RewriteSrcOp(1, width);
            var index = RewriteDstOp(2, width, e => emitter.FAdd(e, add));
            NZV(index);
            var cAdd = add as Constant;
            if (cAdd == null)
                throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    "Instruction {0] too complex to rewrite.",
                    dasm.Current);
            if (cAdd.ToReal64() >= 0.0)
            {
                emitter.Branch(
                    emitter.FLe(index, limit),
                    ((AddressOperand)dasm.Current.Operands[3]).Address,
                    RtlClass.ConditionalTransfer);
            }
            else
            {
                emitter.Branch(
                    emitter.FGe(index, limit),
                    ((AddressOperand)dasm.Current.Operands[3]).Address,
                    RtlClass.ConditionalTransfer);
            }
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteAcbi(PrimitiveType width)
        {
            var limit = RewriteSrcOp(0, width);
            var add = RewriteSrcOp(1, width);
            var index = RewriteDstOp(2, width, e => emitter.IAdd(e, add));
            NZV(index);
            var cAdd = add as Constant;
            if (cAdd == null)
                throw new AddressCorrelatedException(
                    dasm.Current.Address,
                    "Instruction {0] too complex to rewrite.",
                    dasm.Current);
            if (cAdd.ToInt32() >= 0)
            {
                emitter.Branch(
                          emitter.Le(index, limit),
                          ((AddressOperand)dasm.Current.Operands[3]).Address,
                          RtlClass.ConditionalTransfer);
            }
            else
            {
                emitter.Branch(
                    emitter.Ge(index, limit),
                    ((AddressOperand)dasm.Current.Operands[3]).Address,
                    RtlClass.ConditionalTransfer);
            }
            rtlc.Class = RtlClass.ConditionalTransfer;

        }

        private void RewriteBb(bool set)
        {
            var pos = RewriteSrcOp(0, PrimitiveType.Word32);
            var @base = RewriteSrcOp(1, PrimitiveType.Word32);
            Expression test = emitter.And(
                @base,
                emitter.Shl(emitter.Word32(1), pos));
            if (set)
            {
                test = emitter.Ne0(test);
            }
            else
            {
                test = emitter.Eq0(test);
            }
            emitter.Branch(test,
                ((AddressOperand)dasm.Current.Operands[2]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteBlb(Func<Expression,Expression> fn)
        {
            var n = RewriteSrcOp(0, PrimitiveType.Word32);
            var test = fn(emitter.And(n, 1));
            emitter.Branch(test,
                    ((AddressOperand)dasm.Current.Operands[1]).Address,
                    RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteBranch()
        {
            emitter.Goto(
                ((AddressOperand)dasm.Current.Operands[0]).Address);
            rtlc.Class = RtlClass.Transfer;
        }

        private void RewriteBsb()
        {
            emitter.Call(
                ((AddressOperand)dasm.Current.Operands[0]).Address,
                4);
            rtlc.Class = RtlClass.Transfer;
        }

        private void RewriteBranch(ConditionCode cc, FlagM flags)
        {
            emitter.Branch(
                emitter.Test(cc, FlagGroup(flags)),
                ((AddressOperand)dasm.Current.Operands[0]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }


        private void RewriteAob(
            Func<Expression, Expression, Expression> cmp)
        {
            var limit = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(
                1,
                PrimitiveType.Word32,
                e => emitter.IAdd(e, emitter.Word32(1)));
            AllFlags(dst);
            emitter.Branch(
                cmp(dst, limit),
                ((AddressOperand)dasm.Current.Operands[2]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteSob(
            Func<Expression, Expression, Expression> cmp)
        {
            var dst = RewriteDstOp(
                0,
                PrimitiveType.Word32,
                e => emitter.ISub(e, emitter.Word32(1)));
            AllFlags(dst);
            emitter.Branch(
                cmp(dst, Constant.Word32(0)),
                ((AddressOperand)dasm.Current.Operands[1]).Address,
                RtlClass.ConditionalTransfer);
            rtlc.Class = RtlClass.ConditionalTransfer;
        }

        private void RewriteJmp()
        {
            emitter.Goto(RewriteSrcOp(0, PrimitiveType.Word32));
            rtlc.Class = RtlClass.Transfer;
        }

        private void RewriteJsb()
        {
            emitter.Call(RewriteSrcOp(0, PrimitiveType.Word32), 4);
            rtlc.Class = RtlClass.Transfer;
        }

        // condition handler (initially 0) <-- fp
        // saved PSW + flags
        // saved AP
        // saved FP
        // saved PC
        // saved regs
        // ...
        // last saved reg                  <-- sp
        private void RewriteRet()
        {
            var sp = frame.EnsureRegister(Registers.sp);
            var fp = frame.EnsureRegister(Registers.fp);
            var ap = frame.EnsureRegister(Registers.ap);
            emitter.Assign(sp, emitter.ISub(fp, 4));
            emitter.Assign(fp, emitter.LoadDw(emitter.IAdd(sp, 16)));
            emitter.Assign(ap, emitter.LoadDw(emitter.IAdd(sp, 12)));
            emitter.Return(4, 0);
            rtlc.Class = RtlClass.Transfer;
        }

        private void RewriteRsb()
        {
            emitter.Return(4, 0);
            rtlc.Class = RtlClass.Transfer;
        }
    }
}
