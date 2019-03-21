#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    partial class PaRiscRewriter
    {
        private void RewriteAddib()
        {
            if (instr.Annul)
                iclass |= InstrClass.Annul;
            var imm = ((ImmediateOperand)instr.Operands[0]).Value.ToInt32();
            var reg = RewriteOp(instr.Operands[1]);
            m.Assign(reg, m.IAddS(reg, imm));
            MaybeConditionalJump(((AddressOperand) instr.Operands[2]).Address, reg);
        }

        private void RewriteBranch()
        {
            var dest = RewriteOp(instr.Operands[0]);
            var linkReg = ((RegisterOperand) instr.Operands[1]).Register;
            if (linkReg == Registers.GpRegs[0])
            {
                // continuation thrown away == goto
                iclass = InstrClass.Transfer | InstrClass.Delay;
                m.GotoD(dest);
            }
            else if (linkReg == Registers.GpRegs[2] || linkReg == Registers.GpRegs[31])
            {
                iclass = InstrClass.Transfer | InstrClass.Delay | InstrClass.Call;
                m.CallD(dest, 0);
            }
            else
            {
                //$TODO: r2 is the default link register. If this is not used, come up with a workaround
                throw new NotImplementedException();
            }
        }

        private void RewriteBreak()
        {
            m.SideEffect(host.PseudoProcedure("__break", VoidType.Instance));
        }

        private void RewriteBe()
        {
            if (instr.Annul)
                iclass |= InstrClass.Annul;
            var dst = (MemoryOperand) instr.Operands[0];
            Expression ea = binder.EnsureRegister(dst.Base);
            ea = m.IAddS(ea, dst.Offset);
            m.GotoD(ea);
        }

        private void RewriteBv()
        {
            iclass = InstrClass.Transfer | InstrClass.Delay;
            if (instr.Annul)
                iclass |= InstrClass.Annul;
            var dst = (MemoryOperand) instr.Operands[0];
            Expression idx;
            if (dst.Index != null)
            {
                if (dst.Index == Registers.GpRegs[0])
                {
                    //$REVIEW: r2 hardwired as link register. A "purist" approach
                    // would leave this to the SSA / Value propagation steps.
                    if (dst.Base == Registers.GpRegs[2])
                    {
                        m.ReturnD(0, 0);
                    }
                    else
                    {
                        m.GotoD(binder.EnsureRegister(dst.Base));
                    }
                    return;
                }
                // According to PA-RISC manual, the index must be scaled by 8.
                idx = m.IMul(
                    binder.EnsureRegister(dst.Index),
                    8);
            }
            else
            {
                if (dst.Offset == 0)
                {
                    //$REVIEW: r2 hardwired as link register. A "purist" approach
                    // would leave this to the SSA / Value propagation steps.
                    if (dst.Base == Registers.GpRegs[2])
                    {
                        m.ReturnD(0, 0);
                    }
                    else
                    {
                        m.GotoD(binder.EnsureRegister(dst.Base));
                    }
                    return;
                }
                idx = Constant.Int32(dst.Offset * 8);
            }
            var gotoDst = binder.EnsureRegister(dst.Base);
            m.GotoD(m.IAdd(gotoDst, idx));
        }

        private void RewriteCmpb()
        {
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            MaybeConditionalJump(((AddressOperand)instr.Operands[2]).Address, left, right);
        }
    }
}
