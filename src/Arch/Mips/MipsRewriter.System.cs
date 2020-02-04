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
                host.PseudoProcedure(
                    "__break",
                    VoidType.Instance,
                    this.RewriteOperand0(instr.Operands[0])));
        }

        private void RewriteMfc0(MipsInstruction instr)
        {
            var cpregFrom = ((RegisterOperand)instr.Operands[1]).Register;
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
            var cpregTo = ((RegisterOperand)instr.Operands[1]).Register;
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
            var intrinsic = host.PseudoProcedure("__prefetch", VoidType.Instance, opMem.EffectiveAddress);
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
                trap = host.PseudoProcedure("__trap_code", VoidType.Instance, RewriteOperand(instr.Operands[2]));
            }
            else
            {
                trap = host.PseudoProcedure("__trap", VoidType.Instance);
            }
            m.SideEffect(trap);
        }

        private void RewriteTrapi(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);

            this.iclass = InstrClass.ConditionalTransfer;
            m.BranchInMiddleOfInstruction(
                        op(op1, op2).Invert(),
                        instr.Address + instr.Length,
                        InstrClass.ConditionalTransfer);
            var trap = host.PseudoProcedure("__trap", VoidType.Instance);
            m.SideEffect(trap);
        }

        private void RewriteTlbp(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'EntryHi' and 'Index' registers. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.PseudoProcedure("__tlbp", VoidType.Instance));
        }

        private void RewriteTlbr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.PseudoProcedure("__tlbr", VoidType.Instance));
        }

        private void RewriteTlbwi(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.PseudoProcedure("__tlbwi", VoidType.Instance));
        }

        private void RewriteTlbwr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.PseudoProcedure("__tlbwr", VoidType.Instance));
        }

        private void RewriteWait(MipsInstruction instr)
        {
            m.SideEffect(host.PseudoProcedure("__wait", VoidType.Instance));
        }

        private void RewriteReadHardwareRegister(MipsInstruction instr)
        {
            var rdhwr = host.PseudoProcedure("__read_hardware_register", PrimitiveType.UInt32, this.RewriteOperand0(instr.Operands[1]));
            m.Assign(this.RewriteOperand0(instr.Operands[0]), rdhwr);
        }

        private void RewriteSyscall(MipsInstruction instr)
        {
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, this.RewriteOperand0(instr.Operands[0])));
        }

        private void RewriteSync(MipsInstruction instr)
        {
            m.SideEffect(host.PseudoProcedure("__sync", VoidType.Instance, this.RewriteOperand0(instr.Operands[0])));
        }
    }
}
