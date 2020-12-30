#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

        private void RewriteBr()
        {
            this.iclass = InstrClass.Transfer;
            var dst = Reg(0);
            m.Goto(dst);
        }

        private void RewriteBranch(ConditionCode condCode)
        {
            this.iclass = InstrClass.ConditionalTransfer;
            var dst = Reg(0);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.BranchInMiddleOfInstruction(m.Test(condCode, cc).Invert(), instr.Address + instr.Length, iclass);
            m.Goto(dst);
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
            m.SideEffect(host.Intrinsic(IntrinsicProcedure.Syscall, false, VoidType.Instance, svcNo));
        }
    }
}
