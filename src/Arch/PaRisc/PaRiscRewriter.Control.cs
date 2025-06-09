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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public partial class PaRiscRewriter
    {
        private void RewriteAddb()
        {
            var reg = RewriteOp(1);
            if (instr.Operands[0] is Constant imm)
            {
                m.Assign(reg, m.IAddS(reg, imm.ToInt32()));
            }
            else
            {
                var src = (RegisterStorage) instr.Operands[0];
                m.Assign(reg, m.IAdd(reg, binder.EnsureRegister(src)));
            }
            MaybeBranchAndAnnul(2, RewriteCondition, reg);
        }

        private void RewriteBb()
        {
            var reg = RewriteOp(0);
            Expression bitNumber;
            if (instr.Operands[1] is Constant imm)
            {
                bitNumber = Constant.Int32(imm.ToInt32());
            }
            else
            {
                bitNumber = RewriteOp(1);
            }
            var test = m.Fn(is_bit_set_intrinsic, reg, bitNumber);
            MaybeBranchAndAnnul(2, RewriteBbCondition, test);
        }

        private Expression RewriteBbCondition(Expression a, Expression b)
        {
            switch (instr.Condition!.Type)
            {
            case ConditionType.Lt:
            case ConditionType.Lt64:
                return a;
            default:
                return a.Invert();
            }
        }

        private void RewriteBranch()
        {
            var dest = RewriteOp(0);
            var linkReg = (RegisterStorage) instr.Operands[1];
            if (linkReg == arch.Registers.GpRegs[0])
            {
                // continuation thrown away == goto
                iclass = InstrClass.Transfer | InstrClass.Delay;
                m.GotoD(dest);
            }
            else if (linkReg == arch.Registers.GpRegs[2] || linkReg == arch.Registers.GpRegs[31])
            {
                iclass = InstrClass.Transfer | InstrClass.Delay | InstrClass.Call;
                m.CallD(dest, 0);
            }
            else
            {
                //$TODO: r2 is the default link register. If this is not used, come up with a workaround
                iclass = InstrClass.Transfer | InstrClass.Delay | InstrClass.Call;
                m.CallD(dest, 0);
                host.Warn(instr.Address, "Unusual link register usage in {0}", instr);
            }
        }

        private void RewriteBreak()
        {
            m.SideEffect(m.Fn(break_intrinsic));
        }

        private void RewriteBe(bool link)
        {
            // Nullifying is implemented by injecting a jump
            // that skips the nullified instruction.
            // We don't have to make a delay slot in this case.
            if (instr.Annul)
                iclass &= ~InstrClass.Delay;
            var dst = (MemoryOperand) instr.Operands[0];
            Expression ea = binder.EnsureRegister(dst.Base);
            ea = m.IAddS(ea, dst.Offset);
            if (link)
            {
                m.Call(ea, 0, iclass);
                if (!iclass.HasFlag(InstrClass.Delay))
                {
                    m.Goto(instr.Address + 8);  // Skip/nullify the next instruction.
                }
            }
            else
            {
                m.Goto(ea, iclass);
            }
        }

        private void RewriteBv()
        {
            iclass = InstrClass.Transfer | InstrClass.Delay;
            if (instr.Annul)
                iclass |= InstrClass.Annul;
            var dst = (MemoryOperand) instr.Operands[0];
            Expression idx;
            if (dst.Index is not null)
            {
                if (dst.Index == arch.Registers.GpRegs[0])
                {
                    //$REVIEW: r2 hardwired as link register. A "purist" approach
                    // would leave this to the SSA / Value propagation steps.
                    if (dst.Base == arch.Registers.GpRegs[2])
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
                    if (dst.Base == arch.Registers.GpRegs[2])
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

        private void RewriteCmpb(int iLeft, int iRight)
        {
            var left = RewriteOp(iLeft);
            var right = RewriteOp(iRight);
            MaybeBranchAndAnnul(2, RewriteCondition, left, right);
        }

        private void RewriteCmpclr(int iLeft, int iRight)
        {
            var left = RewriteOp(iLeft);
            var right = RewriteOp(iRight);
            var tmpLeft = binder.CreateTemporary(left.DataType);
            var tmpRight = binder.CreateTemporary(right.DataType);
            m.Assign(tmpLeft, left);
            m.Assign(tmpRight, right);
            var reg = (RegisterStorage) instr.Operands[2];
            if (reg.Number != 0)
                m.Assign(binder.EnsureIdentifier(reg), 0);
            var condition = RewriteCondition(tmpLeft, tmpRight);
            var addrNext = instr.Address + 8;
            m.Branch(condition, addrNext);
        }

        private void MaybeBranchAndAnnul(
            int iop,  
            Func<Expression,Expression,Expression> rewriteCondition,
            Expression reg,
            Expression? right = null)
        {
            var addrDest = (Address) instr.Operands[iop];
            if (instr.Annul)
            {
                if (addrDest <= instr.Address)
                {
                    // We're jumping backward, so we annul the falling out of the loop.
                    // Generate the jump out of the loop, without delay slot.
                    MaybeConditionalJump(InstrClass.ConditionalTransfer, instr.Address + 8, rewriteCondition, true, reg, right);
                    // Generate jump to top of loop, with delay slot.
                    m.GotoD(addrDest);
                }
                else
                {
                    // We're jumping forward, so we annul only when the branch is taken. 
                    // This is equivalent to a branch on a "normal" architecture with no
                    // delay slots.
                    MaybeConditionalJump(InstrClass.ConditionalTransfer, addrDest, rewriteCondition, false, reg, right);
                    this.iclass = InstrClass.ConditionalTransfer;
                }
            }
            else
            {
                MaybeConditionalJump(
                    InstrClass.ConditionalTransfer | InstrClass.Delay,
                    addrDest, rewriteCondition, false, reg, right);
            }
        }

        private void RewriteMovb()
        {
            var src = RewriteOp(0);
            var dst = RewriteOp(1);
            m.Assign(dst, src);
            MaybeBranchAndAnnul(2, RewriteCondition, src);
        }

        private void RewriteRfi(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
            m.Return(0, 0);
        }
    }
}
