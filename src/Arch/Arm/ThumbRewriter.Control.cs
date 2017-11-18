#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter
    {
        private void RewriteB()
        {
            var addr = Address.Ptr32((uint)ops[0].ImmediateValue.Value);
            if (instr.ArchitectureDetail.CodeCondition == ArmCodeCondition.AL)
            {
                rtlc = RtlClass.Transfer;
                m.Goto(addr);
            }
            else
            {
                rtlc = RtlClass.ConditionalTransfer;
                m.Branch(TestCond(instr.ArchitectureDetail.CodeCondition), addr, RtlClass.ConditionalTransfer);
            }
        }

        private void RewriteBl()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.Call(
                Address.Ptr32((uint)ops[0].ImmediateValue.Value),
                0);
        }

        private void RewriteBlx()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            m.Call(RewriteOp(ops[0]), 0);
        }

        private void RewriteBx()
        {
            rtlc = RtlClass.Transfer;
            m.Goto(RewriteOp(ops[0]));
        }

        private void RewriteCbnz(Func<Expression, Expression> ctor)
        {
            rtlc = RtlClass.ConditionalTransfer;
            m.Branch(ctor(RewriteOp(ops[0])),
                Address.Ptr32((uint)ops[1].ImmediateValue.Value),
                RtlClass.Transfer);
        }

        private void RewriteIt()
        {
            // Disassembler has already discovered the 'e' or 't' pattern, so all we need to do
            // is to hang on to the mnemonic and peel off one letter at a time.
            this.itState = instr.Mnemonic;
            this.itPos = 0; // position before the first 'e'/'t' "opcode".
            this.itStateCondition = instr.ArchitectureDetail.CodeCondition;
            // Emit a placeholder NOP at the address of the IT instruction.
            m.Nop();
        }

        private void RewriteTrap()
        {
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, Constant.UInt32(instr.Bytes[0])));
        }

        private void RewriteUdf()
        {
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, Constant.UInt32(instr.Bytes[0])));
        }
    }
}
