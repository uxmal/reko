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
using System;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteB()
        {
            if (instr.Operands[0] is ConditionOperand cop)
            {
                var cc = cop.Condition;
                m.Branch(TestCond(cc), ((AddressOperand) instr.Operands[1]).Address, rtlc);
            }
            else
            {
                m.Goto(RewriteOp(instr.Operands[0]));
            }
        }

        private void RewriteBl()
        {
            m.Call(RewriteOp(instr.Operands[0]), 0);
        }

        private void RewriteBlr()
        {
            m.Call(RewriteOp(instr.Operands[0]), 0);
        }

        private void RewriteBr()
        {
            m.Goto(RewriteOp(instr.Operands[0]));
        }

        private void RewriteCb(Func<Expression, Expression> fn)
        {
            var reg = binder.EnsureRegister(((RegisterOperand)instr.Operands[0]).Register);
            m.Branch(fn(reg), ((AddressOperand)instr.Operands[1]).Address, rtlc);
        }

        private void RewriteRet()
        {
            var reg = ((RegisterOperand)instr.Operands[0]).Register;
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

        private void RewriteTb(Func<Expression,Expression> fn)
        {
            var reg = RewriteOp(instr.Operands[0]);
            int sh = ((ImmediateOperand)instr.Operands[1]).Value.ToInt32();
            var mask = Constant.Create(reg.DataType, 1ul << sh);
            var dst = ((AddressOperand)instr.Operands[2]).Address;
            m.Branch(fn(m.And(reg, mask)), dst, instr.InstructionClass);
        }
    }
}
