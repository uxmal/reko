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
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
{
    public partial class SparcRewriter
    {
        private Identifier Grf(FlagM grf)
        {
            return binder.EnsureFlagGroup(
                arch.Registers.psr,
                (uint) grf, 
                arch.GrfToString(arch.Registers.psr, "", (uint) grf),
                Bits.IsSingleBitSet((uint)grf) ? PrimitiveType.Bool: PrimitiveType.Byte);
        }

        private void RewriteBranch(Expression cond)
        {
            // SPARC architecture always has delay slot.
            var rtlClass = instrCur.InstructionClass;
            this.iclass = rtlClass;
            if (cond is Constant c && c.ToBoolean())
            {
                m.Goto(((AddressOperand)instrCur.Operands[0]).Address, rtlClass);
            }
            else
            {
                m.Branch(cond, ((AddressOperand)instrCur.Operands[0]).Address, rtlClass);
            }
        }

        private void RewriteBranchReg(Func<Expression, Expression> fn)
        {
            var rtlClass = instrCur.InstructionClass;
            this.iclass = rtlClass;
            var reg = RewriteRegister(0);
            m.Branch(fn(reg), ((AddressOperand) instrCur.Operands[1]).Address, rtlClass);
        }

        private void RewriteCall()
        {
            // A special case is when we call to the location after
            // the delay slot. This is an idiom to capture the 
            // program counter in the la register.
            var dst = ((AddressOperand) instrCur.Operands[0]).Address;
            if (instrCur.Address.ToLinear() + 8 == dst.ToLinear())
            {
                iclass = InstrClass.Linear;
                m.Assign(binder.EnsureRegister(arch.Registers.o7), dst);
            }
            else
            {
                m.CallD(dst, 0);
            }
        }

        private void RewriteJmpl()
        {
            iclass = InstrClass.Transfer;
            var rDst = (RegisterStorage) instrCur.Operands[2];
            var src1 = RewriteOp(instrCur.Operands[0]);
            var src2 = RewriteOp(instrCur.Operands[1]);
            if (rDst != arch.Registers.g0)
            {
                var dst = RewriteOp(instrCur.Operands[2])!;
                m.Assign(dst, instrCur.Address);
            }
            var target = SimplifySum(src1, src2);
            if (rDst == arch.Registers.o7)
            {
                m.CallD(target, 0);
            }
            else if (rDst == arch.Registers.g0)
            {
                m.ReturnD(0, 0);
            }
            else
            {
                m.GotoD(target);
            }
        }

        private void RewriteRett()
        {
        }

        private void RewriteReturn()
        {
            var dst = RewriteOp(instrCur.Operands[0])!;
            var src1 = RewriteOp(instrCur.Operands[0])!;
            var src2 = RewriteOp(instrCur.Operands[1])!;
            RestoreRegisterWindow(dst, src1, src2);
            m.Return(0, 0, instrCur.InstructionClass);
        }

        private void RewriteTrap(Expression cond)
        {
            //$REVIEW: does a SPARC trap instruction have a delay slot?
            var src1 = RewriteOp(instrCur.Operands[0], true);
            var src2 = RewriteOp(instrCur.Operands[1], true);
            m.BranchInMiddleOfInstruction(
                cond.Invert(),
                instrCur.Address + instrCur.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                m.Fn(CommonOps.Syscall_1, SimplifySum(src1, src2)));
        }
    }
}
