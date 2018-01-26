﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using System.Threading.Tasks;

namespace Reko.Arch.Alpha
{
    public partial class AlphaRewriter
    {
        private const int ZeroRegister = 31; 
        private const int ReturnAddress = 26;   //$REVIEW: is platform dependent.

        private void RewriteBr()
        {
            var ret = ((RegisterOperand)instr.op1).Register;
            var dst = ((AddressOperand)instr.op2).Address;
            RewriteTransfer(ret, dst);
        }


        private Expression lbc(Expression e)
        {
            return m.Eq0(m.And(e, 1));
        }

        private Expression lbs(Expression e)
        {
            return m.Ne0(m.And(e, 1));
        }

        private void RewriteBranch(Func<Expression, Expression> fn)
        {
            var dst = ((AddressOperand)instr.op2).Address;
            rtlc = RtlClass.ConditionalTransfer;
            var src = Rewrite(instr.op1);
            m.Branch(fn(src), dst, rtlc);
        }

        private void RewriteCmov(Func<Expression, Expression> skip)
        {
            var cond = Rewrite(instr.op1);
            var src = Rewrite(instr.op2);
            var dst = Rewrite(instr.op3);
            m.BranchInMiddleOfInstruction(skip(cond).Invert(), instr.Address + instr.Length, RtlClass.ConditionalTransfer);
            m.Assign(dst, src);
        }

        private void RewriteFCmov(Operator op)
        {
            var cond = Rewrite(instr.op1);
            var src = Rewrite(instr.op2);
            var dst = Rewrite(instr.op3);
            cond = new BinaryExpression(op, PrimitiveType.Bool, cond, Constant.Real64(0.0));
            m.BranchInMiddleOfInstruction(cond.Invert(), instr.Address + instr.Length, RtlClass.ConditionalTransfer);
            m.Assign(dst, src);
        }

        private void RewriteFBranch(Operator op)
        {
            rtlc = RtlClass.ConditionalTransfer;
            var src = Rewrite(instr.op1);
            var dst = ((AddressOperand)instr.op2).Address;
            m.Branch(
                new BinaryExpression(
                    op, PrimitiveType.Bool, src, Constant.Real64(0.0)),
                dst,
                RtlClass.ConditionalTransfer);
        }

        private void RewriteJmp()
        {
            var ret = ((RegisterOperand)instr.op1).Register;
            var dst = Rewrite(instr.op2);
            RewriteTransfer(ret, dst);
        }

        private void RewriteTransfer(RegisterStorage ret, Expression dst)
        {
            if (ret.Number == ZeroRegister)
            {
                rtlc = RtlClass.Transfer;
                if (dst is Identifier id && ((RegisterStorage)id.Storage).Number == ReturnAddress)
                    m.Return(0, 0);
                else 
                    m.Goto(dst);
            }
            else
            {
                if (ret.Number == ReturnAddress)
                {
                    rtlc = RtlClass.Transfer | RtlClass.Call;
                    m.Call(dst, 0);
                }
                else
                {
                    rtlc = RtlClass.Transfer;
                    // Weird jump. 
                    m.Assign(binder.EnsureRegister(ret), instr.Address + instr.Length);
                    m.Goto(dst);
                }
            }
        }

    }
}
