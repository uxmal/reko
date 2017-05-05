#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Tlcs.Tlcs900
{
    partial class Tlcs900Rewriter
    {
        private void RewriteCall()
        {
            var co = instr.op1 as ConditionOperand;
            if (co != null)
            {
                rtlc.Class = RtlClass.ConditionalTransfer;
                m.BranchInMiddleOfInstruction(
                    GenerateTestExpression(co, true),
                    instr.Address + instr.Length,
                    RtlClass.ConditionalTransfer);
                m.Call(RewriteSrc(instr.op2), 4);
            }
            else
            {
                rtlc.Class = RtlClass.Transfer;
                m.Call(RewriteSrc(instr.op1), 4);
            }
        }

        private void RewriteDjnz()
        {
            rtlc.Class = RtlClass.ConditionalTransfer;
            var reg = RewriteSrc(instr.op1);
            var dst = ((AddressOperand)instr.op2).Address;
            m.Assign(reg, m.ISub(reg, 1));
            m.Branch(m.Ne0(reg), dst, RtlClass.ConditionalTransfer);
        }

        private void RewriteJp()
        {
            var co = instr.op1 as ConditionOperand;
            if (co != null)
            {
                rtlc.Class = RtlClass.ConditionalTransfer;
                var test = GenerateTestExpression(co, false);
                var dst = RewriteSrc(instr.op2);
                var addr = dst as Address;
                if (addr != null)
                {
                    m.Branch(test, addr, RtlClass.ConditionalTransfer);
                }
                else
                {
                    m.If(test, new RtlGoto(dst, RtlClass.ConditionalTransfer));
                }
            }
            else
            {
                rtlc.Class = RtlClass.Transfer;
                var dst = RewriteSrc(instr.op1);
                m.Goto(dst);
            }
        }

        private void RewriteRet()
        {
            var co = instr.op1 as ConditionOperand;
            if (co != null)
            {
                EmitUnitTest("Tlcs900_rw_", "00010000");
                Invalid();
            }
            else
            {
                rtlc.Class = RtlClass.Transfer;
                m.Return(4, 0);
            }
        }
    }
}
