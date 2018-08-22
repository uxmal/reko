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

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteBgezal(MipsInstruction instr)
        {
            // The bgezal r0,XXXX instruction is aliased to bal (branch and link, or fn call)
            // We handle that case here explicitly.
            if (((RegisterOperand)instr.op1).Register.Number == 0)
            {
                rtlc = RtlClass.Transfer;
                var dst = RewriteOperand(instr.op2);
                m.CallD(dst, 0);
                return;
            }
            RewriteBranch0(instr, m.Ge, false);
        }

        private void RewriteBranch(MipsInstruction instr, Func<Expression, Expression, Expression> condOp, bool link)
        {
            if (!link)
            {
                var reg1 = RewriteOperand0(instr.op1);
                var reg2 = RewriteOperand0(instr.op2);
                var addr = (Address)RewriteOperand0(instr.op3);
                var cond = condOp(reg1, reg2);
                if (condOp == m.Eq &&
                    ((RegisterOperand)instr.op1).Register ==
                    ((RegisterOperand)instr.op2).Register)
                {
                    rtlc = RtlClass.Transfer | RtlClass.Delay;
                    m.GotoD(addr);
                }
                else
                {
                    rtlc = RtlClass.ConditionalTransfer | RtlClass.Delay;
                    m.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
                }
            }
            else
            {
                throw new NotImplementedException("Linked branches not implemented yet.");
            }
        }

        private void RewriteBranch0(MipsInstruction instr, Func<Expression, Expression, Expression> condOp, bool link)
        {
            if (link)
            {
                m.Assign(
                    binder.EnsureRegister(arch.LinkRegister),
                    instr.Address + 8);
            }
            var reg = RewriteOperand0(instr.op1);
            var addr = (Address)RewriteOperand0(instr.op2);
            if (reg is Constant)
            {
                // r0 has been replaced with '0'.
                if (condOp == m.Lt)
                {
                    return; // Branch will never be taken
                }
            }
            var cond = condOp(reg, Constant.Zero(reg.DataType));
            rtlc = RtlClass.ConditionalTransfer;
            m.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
        }

        private void RewriteBranchLikely(MipsInstruction instr, Func<Expression, Expression, Expression> condOp)
        {
            var reg1 = RewriteOperand0(instr.op1);
            var reg2 = RewriteOperand0(instr.op2);
            var addr = (Address)RewriteOperand0(instr.op3);
            var cond = condOp(reg1, reg2);
            if (condOp == m.Eq &&
                ((RegisterOperand)instr.op1).Register ==
                ((RegisterOperand)instr.op2).Register)
            {
                rtlc = RtlClass.Transfer | RtlClass.Delay;
                m.GotoD(addr);
            }
            else
            {
                rtlc = RtlClass.ConditionalTransfer | RtlClass.Delay;
                m.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
            }
        }

        private void RewriteBranchConditional1(MipsInstruction instr, bool opTrue)
        {
            var cond = RewriteOperand0(instr.op1);
            if (!opTrue)
                cond = m.Not(cond);
            var addr = (Address)RewriteOperand0(instr.op2);
            rtlc = RtlClass.ConditionalTransfer | RtlClass.Delay;
            m.Branch(cond, addr, rtlc);
        }

        private void RewriteEret(MipsInstruction instr)
        {
            // Return from exception doesn't seem to modify any 
            // GPRs (as well it shouldn't).
            // MIPS manual says it does _not_ have a delay slot.
            m.Return(0, 0);
        }

        private void RewriteJal(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            rtlc = RtlClass.Transfer;
            m.CallD(RewriteOperand0(instr.op1), 0);
        }

        private void RewriteJalr(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            rtlc = RtlClass.Transfer;
            var dst = RewriteOperand0(instr.op2);
            var lr = ((RegisterOperand)instr.op1).Register;
            if (lr == arch.LinkRegister)
            {
                m.CallD(dst, 0);
                return;
            }
            else
            {
                m.Assign(binder.EnsureRegister(lr), instr.Address + 8);
                m.GotoD(dst);
        }
        }

        private void RewriteJr(MipsInstruction instr)
        {
            rtlc = RtlClass.Transfer;
            var dst = RewriteOperand(instr.op1);

            var reg = (RegisterStorage)((Identifier)dst).Storage;
            if (reg == arch.LinkRegister)
            {
                m.ReturnD(0, 0);
            }
			else
            {
                m.GotoD(dst);
            }
        }

        private void RewriteJump(MipsInstruction instr)
        {
            var dst = RewriteOperand0(instr.op1);
            rtlc = RtlClass.Transfer;
            m.GotoD(dst);
        }
    }
}
