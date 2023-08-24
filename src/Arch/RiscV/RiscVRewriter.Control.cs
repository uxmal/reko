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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteAuipc()
        {
            var offset = ((ImmediateOperand)instr.Operands[1]).Value.ToInt32() << 12;
            var addr = instr.Address + offset;
            var dst = RewriteOp(0);
            m.Assign(dst, addr);
        }

        private void RewriteBranch(Func<Expression, Expression, Expression> fn)
        {
            var opLeft = RewriteOp(0);
            var opRight = RewriteOp(1);
            m.Branch(
                fn(opLeft, opRight),
                ((AddressOperand)instr.Operands[2]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteCompressedBranch(Func<Expression, Expression, Expression> fn)
        {
            var op = RewriteOp(0);
            var zero = Constant.Zero(op.DataType);
            m.Branch(
                fn(op, zero),
                ((AddressOperand) instr.Operands[1]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteCompressedJ()
        {
            m.Goto(RewriteOp(0));
        }

        private void RewriteCompressedJal()
        {
            m.Call(RewriteOp(0), 0);
        }

        private void RewriteCompressedJalr()
        {
            m.Call(RewriteOp(0), 0);
        }

        private void RewriteCompressedJr()
        {
            var reg = (RegisterStorage) instr.Operands[0];
            if (reg == arch.LinkRegister)
                m.Return(0, 0);
            else 
                m.Goto(RewriteOp(0));
        }

        private void RewriteCsr(IntrinsicProcedure intrinsic)
        {
            var csr = RewriteOp(1);
            var arg = RewriteOp(2);
            var dst = RewriteOp(0);
            var application = m.Fn(intrinsic.MakeInstance(dst.DataType), csr, arg);
            if (dst is Constant)
            {
                m.SideEffect(application);
            }
            else
            {
                m.Assign(dst, application);
            }
        }

        private void RewriteEbreak()
        {
            m.SideEffect(m.Fn(ebreak_intrinsic));
        }

        private void RewriteEcall()
        {
            m.SideEffect(m.Fn(CommonOps.Syscall_0));
        }

        private void RewriteJal()
        {
            var continuation = (RegisterStorage)instr.Operands[0];
            var dst = RewriteOp(1);
            iclass = InstrClass.Transfer;
            if (continuation.Number == 0)
            {
                m.Goto(dst);
            }
            else
            {
                iclass |= InstrClass.Call;
                m.Call(dst, 0);
            }
        }

        private void RewriteJalr()
        {
            var continuation = (RegisterStorage)instr.Operands[0];
            var rDst = (RegisterStorage)instr.Operands[1];
            var dst = RewriteOp(1);
            var off = RewriteOp(2);
            iclass = InstrClass.Transfer;
            if (!off.IsZero)
            {
                dst = m.IAdd(dst, off);
            }
            if (continuation.Number == 0)       // 'zero' 
            {
                if (rDst.Number == 1 && off.IsZero)
                {
                    m.Return(0, 0);
                }
                else
                {
                    m.Goto(dst);
                }
            }
            else if (continuation.Number == 1)     // 'r1'
            {
                iclass |= InstrClass.Call;
                m.Call(dst, 0);
            } 
            else 
            {
                m.Assign(
                    RewriteOp(0),
                    instr.Address + instr.Length);
                m.Goto(dst, 0);
            }
        }

        private void RewriteRet(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
            m.Return(0, 0);
        }

        private void RewriteWfi()
        {
            m.SideEffect(m.Fn(wait_for_interrupt_intrinsic));
        }
    }
}
