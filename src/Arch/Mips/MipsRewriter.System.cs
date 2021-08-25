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
                host.Intrinsic(
                    "__break",
                    true,
                    VoidType.Instance,
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
            var intrinsic = host.Intrinsic("__prefetch", true, VoidType.Instance, opMem.EffectiveAddress);
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
                trap = host.Intrinsic("__trap_code", true, VoidType.Instance, RewriteOperand(instr.Operands[2]));
            }
            else
            {
                trap = host.Intrinsic("__trap", true, VoidType.Instance);
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
            var trap = host.Intrinsic("__trap", true, VoidType.Instance);
            m.SideEffect(trap);
        }

        private void RewriteTlbp(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'EntryHi' and 'Index' registers. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.Intrinsic("__tlbp", true, VoidType.Instance));
        }

        private void RewriteTlbr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.Intrinsic("__tlbr", true, VoidType.Instance));
        }

        private void RewriteTlbwi(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.Intrinsic("__tlbwi", true, VoidType.Instance));
        }

        private void RewriteTlbwr(MipsInstruction instr)
        {
            //$REVIEW: MIPS documentation mentions 'Index' register. Contact
            // @uxmal if you care strongly about this.
            m.SideEffect(host.Intrinsic("__tlbwr", true, VoidType.Instance));
        }

        private void RewriteWait(MipsInstruction instr)
        {
            m.SideEffect(host.Intrinsic("__wait", true, VoidType.Instance));
        }

        private void RewriteReadHardwareRegister(MipsInstruction instr)
        {
            var hs = ((ImmediateOperand) instr.Operands[1]).Value;
            Expression value;
            switch (hs.ToInt32())
            {
            case 0:
                value = host.Intrinsic("__read_cpu_number", true, PrimitiveType.UInt32);
                break;
            case 0x1D:
                value = host.Intrinsic("__read_user_local", true, PrimitiveType.Int32);
                break;
            default:
                value = host.Intrinsic("__read_hardware_register", true, PrimitiveType.UInt32, this.RewriteOperand0(instr.Operands[1]));
                break;
            }
            m.Assign(this.RewriteOperand0(instr.Operands[0]), value);
        }

        private void RewriteSdbbp(MipsInstruction instr)
        {
            var arg = RewriteOperand(instr.Operands[0]);
            m.SideEffect(host.Intrinsic("__software_debug_breakpoint", true, VoidType.Instance, arg), iclass);
        }
        
        private void RewriteSyscall(MipsInstruction instr)
        {
            m.SideEffect(host.Intrinsic(IntrinsicProcedure.Syscall, true, VoidType.Instance, this.RewriteOperand0(instr.Operands[0])));
        }

        private void RewriteSync(MipsInstruction instr)
        {
            m.SideEffect(host.Intrinsic("__sync", true, VoidType.Instance, this.RewriteOperand0(instr.Operands[0])));
        }
    }
}
