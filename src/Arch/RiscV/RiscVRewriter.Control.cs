#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteAuipc()
        {
            var offset = ((ImmediateOperand)instr.Operands[1]).Value.ToInt32() << 12;
            var addr = instr.Address + offset;
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, addr);
        }

        private void RewriteBranch(Func<Expression, Expression, Expression> fn)
        {
            var opLeft = RewriteOp(instr.Operands[0]);
            var opRight = RewriteOp(instr.Operands[1]);
            m.Branch(
                fn(opLeft, opRight),
                ((AddressOperand)instr.Operands[2]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteCompressedBranch(Func<Expression, Expression, Expression> fn)
        {
            var op = RewriteOp(instr.Operands[0]);
            var zero = Constant.Zero(op.DataType);
            m.Branch(
                fn(op, zero),
                ((AddressOperand) instr.Operands[1]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteCompressedJ()
        {
            m.Goto(RewriteOp(instr.Operands[0]));
        }


        private void RewriteCompressedJalr()
        {
            m.Call(RewriteOp(instr.Operands[0]), 0);
        }

        private void RewriteCompressedJr()
        {
            var reg = (RegisterOperand) instr.Operands[0];
            if (reg.Register == arch.LinkRegister)
                m.Return(0, 0);
            else 
                m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteJal()
        {
            var continuation = ((RegisterOperand)instr.Operands[0]).Register;
            var dst = RewriteOp(instr.Operands[1]);
            rtlc = InstrClass.Transfer;
            if (continuation.Number == 0)
            {
                m.Goto(dst);
            }
            else
            {
                rtlc |= InstrClass.Call;
                m.Call(dst, 0);
            }
        }

        private void RewriteJalr()
        {
            var continuation = ((RegisterOperand)instr.Operands[0]).Register;
            var rDst = ((RegisterOperand)instr.Operands[1]).Register;
            var dst = RewriteOp(instr.Operands[1]);
            var off = RewriteOp(instr.Operands[2]);
            rtlc = InstrClass.Transfer;
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
                rtlc |= InstrClass.Call;
                m.Call(dst, 0);
            } 
            else 
            {
                m.Assign(
                    RewriteOp(instr.Operands[0]),
                    instr.Address + instr.Length);
                m.Goto(dst, 0);
            }
        }
    }
}
