#region License
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteB()
        {
            rtlc = RtlClass.Transfer;
            if (instr.ops[0] is ConditionOperand cop)
            {
                var cc = cop.Condition;
                m.Branch(TestCond(cc), ((AddressOperand)instr.ops[1]).Address, RtlClass.ConditionalTransfer);
            }
            else
            {
                m.Goto(RewriteOp(instr.ops[0]));
            }
        }

        private void RewriteBl()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.Call(RewriteOp(instr.ops[0]), 0);
        }

        private void RewriteBlr()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.Call(RewriteOp(instr.ops[0]), 0);
        }

        private void RewriteBr()
        {
            rtlc = RtlClass.Transfer;
            m.Goto(RewriteOp(instr.ops[0]));
        }

        private void RewriteCb(Func<Expression, Expression> fn)
        {
            rtlc = RtlClass.ConditionalTransfer;
            var reg = binder.EnsureRegister(((RegisterOperand)instr.ops[0]).Register);
            m.Branch(fn(reg), ((AddressOperand)instr.ops[1]).Address, rtlc);
        }

        private void RewriteRet()
        {
            rtlc = RtlClass.Transfer;
            var reg = ((RegisterOperand)instr.ops[0]).Register;
            if (reg == Registers.GpRegs64[30])
            {
                // Link register
                m.Return(0, 0);
            }
            else
            {
                m.Goto(binder.EnsureRegister(reg));
            }
        }
    }
}
