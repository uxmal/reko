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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteBreak(MipsInstruction instr)
        {
            m.SideEffect(
                m.Fn(
                    intrinsics.break_intrinsic,
                    this.RewriteOperand0(instr.Operands[0])));
        }

        private void RewriteMfc0(MipsInstruction instr)
        {
            var cpregFrom = (RegisterStorage)instr.Operands[1];
            Identifier from;
            switch (cpregFrom.Number)
            {
            case 9: from = binder.CreateTemporary("__counter__", PrimitiveType.UInt32); break;
            default: from = binder.CreateTemporary("__cp" + cpregFrom.Number, PrimitiveType.UInt32); break;
            }
            m.Assign(RewriteOperand0(instr.Operands[0]), from);
        }

        private void RewriteMtc0(MipsInstruction instr)
        {
            var cpregTo = (RegisterStorage)instr.Operands[1];
            Identifier to;
            switch (cpregTo.Number)
            {
                case 9: to = binder.CreateTemporary("__counter__", PrimitiveType.UInt32); break;
                default: to = binder.CreateTemporary("__cp" + cpregTo.Number, PrimitiveType.UInt32); break;
            }
            m.Assign(to, RewriteOperand0(instr.Operands[0]));
        }

        private void RewritePrefx(MipsInstruction instr)
        {
            var opMem = (MemoryAccess)RewriteOperand(instr.Operands[1]);
            var intrinsic = m.Fn(
                CommonOps.Prefetch.MakeInstance(arch.PointerType.BitSize, arch.PointerType),
                opMem.EffectiveAddress);
            m.SideEffect(intrinsic);
        }

        private void RewriteTrap(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);
            if (op != m.Eq || !cmp.Equals(op1, op2))
            {
                m.BranchInMiddleOfInstruction(
                        op(op1, op2).Invert(),
                        instr.Address + instr.Length,
                        InstrClass.ConditionalTransfer);
            }
            Expression trap;
            if (instr.Operands.Length == 3)
            {
                trap = m.Fn(CommonOps.Syscall_1, RewriteOperand(instr.Operands[2]));
            }
            else
            {
                trap = m.Fn(CommonOps.Syscall_0);
            }
            m.SideEffect(trap);
        }

        private void RewriteTrapi(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);

            this.iclass = InstrClass.ConditionalTransfer;
            m.Branch(
                        op(op1, op2).Invert(),
                        instr.Address + instr.Length,
                        InstrClass.ConditionalTransfer);
            m.SideEffect(m.Fn(CommonOps.Syscall_0));
        }

        private void RewriteTlbp(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'EntryHi' and 'Index' registers. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(m.Fn(intrinsics.tlbp));
        }

        private void RewriteTlbr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(m.Fn(intrinsics.tlbr));
        }

        private void RewriteTlbwi(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(m.Fn(intrinsics.tlbwi));
        }

        private void RewriteTlbwr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(m.Fn(intrinsics.tlbwr));
        }

        private void RewriteWait(MipsInstruction instr)
        {
            m.SideEffect(m.Fn(intrinsics.wait));
        }

        private void RewriteReadHardwareRegister(MipsInstruction instr)
        {
            var hs = ((Constant)instr.Operands[1]);
            Expression value;
            switch (hs.ToInt32())
            {
            case 0:
                value = m.Fn(intrinsics.read_cpu_number);
                break;
            case 0x1D:
                value = m.Fn(intrinsics.read_user_local);
                break;
            default:
                value = m.Fn(intrinsics.read_hardware_register, this.RewriteOperand0(instr.Operands[1]));
                break;
            }
            m.Assign(this.RewriteOperand0(instr.Operands[0]), value);
        }

        private void RewriteSdbbp(MipsInstruction instr)
        {
            var arg = RewriteOperand(instr.Operands[0]);
            m.SideEffect(m.Fn(intrinsics.sdbbp, arg), iclass);
        }
        
        private void RewriteSyscall(MipsInstruction instr)
        {
            m.SideEffect(m.Fn(CommonOps.Syscall_1, this.RewriteOperand0(instr.Operands[0])));
        }

        private void RewriteSync(MipsInstruction instr)
        {
            m.SideEffect(m.Fn(intrinsics.sync, this.RewriteOperand0(instr.Operands[0])));
        }
    }
}
