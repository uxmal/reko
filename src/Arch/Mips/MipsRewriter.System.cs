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
                    this.RewriteOperand0(instr.op1)));
        }

        private void RewriteMfc0(MipsInstruction instr)
        {
            var cpregFrom = ((RegisterOperand)instr.op2).Register;
            Identifier from;
            switch (cpregFrom.Number)
            {
            case 9: from = binder.CreateTemporary("__counter__", PrimitiveType.UInt32); break;
            default: from = binder.CreateTemporary("__cp" + cpregFrom.Number, PrimitiveType.UInt32); break;
            }
            m.Assign(RewriteOperand0(instr.op1), from);
        }

        private void RewriteTrap(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            if (op != m.Eq || !cmp.Equals(op1, op2))
            {
                this.rtlc = RtlClass.ConditionalTransfer;
                m.BranchInMiddleOfInstruction(
                        op(op1, op2).Invert(),
                        instr.Address + instr.Length,
                        RtlClass.ConditionalTransfer);
            }
            var trap = host.PseudoProcedure("__trap", VoidType.Instance, RewriteOperand(instr.op3));
            m.SideEffect(trap);
        }

        private void RewriteReadHardwareRegister(MipsInstruction instr)
        {
            var rdhwr = host.PseudoProcedure("__read_hardware_register", PrimitiveType.UInt32, this.RewriteOperand0(instr.op2));
            m.Assign(this.RewriteOperand0(instr.op1), rdhwr);
        }

        private void RewriteSyscall(MipsInstruction instr)
        {
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, this.RewriteOperand0(instr.op1)));
        }

        private void RewriteSync(MipsInstruction instr)
        {
            m.SideEffect(host.PseudoProcedure("__sync", VoidType.Instance, this.RewriteOperand0(instr.op1)));
        }
    }
}
