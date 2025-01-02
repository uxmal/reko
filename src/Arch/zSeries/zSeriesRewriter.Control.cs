#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteBasr()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Call;
            var lr = Reg(0);
            m.Assign(lr, instr.Address + instr.Length);
            var dst = Reg(1);
            m.Call(dst, 0);
        }

        private void RewriteBassm()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Call;
            var lr = Reg(0);
            m.Assign(lr, instr.Address + instr.Length);
            var dst = Reg(1);
            m.Call(dst, 0);
        }

        private void RewriteBr()
        {
            this.iclass = InstrClass.Transfer;
            var dst = Op(0, arch.PointerType);
            m.Goto(dst);
        }

        private void RewriteBranch(ConditionCode condCode)
        {
            this.iclass = InstrClass.ConditionalTransfer;
            var dst = Op(0, arch.PointerType);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.BranchInMiddleOfInstruction(m.Test(condCode, cc).Invert(), instr.Address + instr.Length, iclass);
            m.Goto(dst);
        }

        private void RewriteBranchAndLink()
        {
            m.Assign(Reg(0), instr.Address + instr.Length);
            m.Call(m.Mem(arch.PointerType, EffectiveAddress(1)), 0);
        }

        private void RewriteBranchAndLinkReg()
        {
            m.Assign(Reg(0), instr.Address + instr.Length);
            m.Call(Reg(1), 0);
        }

        private void RewriteBranchEa(ConditionCode condCode)
        {
            var dst = EffectiveAddress(0);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.BranchInMiddleOfInstruction(m.Test(condCode, cc).Invert(), instr.Address + instr.Length, iclass);
            m.Goto(dst);
        }

        private void RewriteBranchOnCount(PrimitiveType dt)
        {
            var index = Reg(0,dt);
            Expression dst;
            if (instr.Operands[1] is RegisterStorage r)
                dst = binder.EnsureRegister(r);
            else
                dst = EffectiveAddress(1);
            m.Assign(index, m.ISub(index, 1));
            if (dst is Address addr)
            {
                m.Branch(m.Ne0(index), addr);
            }
            else
            {
                m.BranchInMiddleOfInstruction(m.Eq0(index), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                m.Goto(dst);
            }
        }

        private void Brx(Func<Expression, Expression, Expression> cmp)
        {
            var index = Reg(0);
            var addr = EffectiveAddress(2);
            // How's this for CISC:
            // "When the R3 field is even, it designates a pair of registers; the contents of the even
            // " and odd registers of the pair are used as the increment and the compare value, respectively.
            // When the R3 field is odd, it designates a single register, the contents of which are
            // used as both the increment and the compare value
            var r3 = (RegisterStorage) instr.Operands[1];
            Identifier inc = binder.EnsureRegister(r3);
            Identifier val;
            if ((r3.Number & 1) == 0)
            {
                val = binder.EnsureRegister(NextGpReg(r3));
            }
            else
            {
                val = inc; 
            }
            m.Assign(index, m.IAdd(index, inc));
            var condition = cmp(index, val);
            if (addr is Address a)
            {
                m.Branch(condition, a);
            }
            else
            {
                m.BranchInMiddleOfInstruction(condition.Invert(), instr.Address + instr.Length, iclass);
                m.Goto(addr);
            }
        }

        private void Bx(Func<Expression, Expression, Expression> cmp)
        {
            var index = Reg(0);
            var addr = EffectiveAddress(2);
            // How's this for CISC:
            // "When the R3 field is even, it designates a pair of registers; the contents of the even
            // " and odd registers of the pair are used as the increment and the compare value, respectively.
            // When the R3 field is odd, it designates a single register, the contents of which are
            // used as both the increment and the compare value
            var r3 = (RegisterStorage) instr.Operands[1];
            Identifier inc = binder.EnsureRegister(r3);
            Identifier val;
            if ((r3.Number & 1) == 0)
            {
                val = binder.EnsureRegister(NextGpReg(r3));
            }
            else
            {
                val = inc;
            }
            m.Assign(index, m.IAdd(index, inc));
            var condition = cmp(index, val);
            if (addr is Address a)
            {
                m.Branch(condition, a);
            }
            else
            {
                m.BranchInMiddleOfInstruction(condition.Invert(), instr.Address + instr.Length, iclass);
                m.Goto(addr);
            }
        }

        private void RewriteBrasl()
        {
            this.iclass = InstrClass.Transfer | InstrClass.Call;
            var dst = Reg(0);
            m.Assign(dst, instr.Address + instr.Length);
            m.Call(Addr(1), 0);
        }

        private void RewriteBrctg()
        {
            this.iclass = InstrClass.ConditionalTransfer;
            var reg = Reg(0);
            m.Assign(reg, m.ISubS(reg, 1));
            var ea = EffectiveAddress(1);
            if (ea is Address addr)
            {
                m.Branch(m.Ne0(reg), addr, iclass);
            }
            else
            {
                EmitUnitTest();
                m.Invalid();
            }
        }

        private void RewriteBsm()
        {
            m.Assign(Reg(0), instr.Address + instr.Length);
            m.Goto(Reg(1));
        }

        private void RewriteCij(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Imm(1, dt);
            var cond = SignedCondition(2, left, right);
            m.Branch(cond, Addr(3));
        }

        private void RewriteClij(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Imm(1, dt);
            var cond = UnsignedCondition(2, left, right);
            m.Branch(cond, Addr(3));
        }

        private void RewriteClrj(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var cond = UnsignedCondition(2, left, right);
            m.Branch(cond, Addr(3));
        }

        private void RewriteCrj(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var cond = SignedCondition(2, left, right);
            m.Branch(cond, Addr(3));
        }
        
        private void RewriteJ()
        {
            this.iclass = InstrClass.Transfer;
            var dst = Addr(0);
            m.Goto(dst);
        }

        private void RewriteJcc(ConditionCode condCode)
        {
            this.iclass = InstrClass.ConditionalTransfer;
            var dst = Addr(0);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Branch(m.Test(condCode, cc), dst, iclass);
        }

        private void RewriteSvc()
        {
            var svcNo = ((ImmediateOperand) instr.Operands[0]).Value;
            m.SideEffect(m.Fn(CommonOps.Syscall_1, svcNo));
        }

        private void RewriteUnconditionalBranch()
        {
            var dst = EffectiveAddress(0);
            m.Goto(dst);
        }
    }
}
