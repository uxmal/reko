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
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Registers = Reko.Arch.Tlcs.Tlcs900.Tlcs900Registers;

namespace Reko.Arch.Tlcs.Tlcs900
{
    partial class Tlcs900Rewriter
    {
        private void RewriteCall()
        {
            if (instr.Operands[0] is ConditionOperand<CondCode> co)
            {
                iclass = InstrClass.ConditionalTransfer | InstrClass.Call;
                m.BranchInMiddleOfInstruction(
                    GenerateTestExpression(co, true),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                m.Call(RewriteSrc(instr.Operands[1]), 4);
            }
            else
            {
                iclass = InstrClass.Transfer | InstrClass.Call;
                m.Call(RewriteSrc(instr.Operands[0]), 4);
            }
        }

        private void RewriteDjnz()
        {
            iclass = InstrClass.ConditionalTransfer;
            var reg = RewriteSrc(instr.Operands[0]);
            var dst = (Address)instr.Operands[1];
            m.Assign(reg, m.ISub(reg, 1));
            m.Branch(m.Ne0(reg), dst, InstrClass.ConditionalTransfer);
        }

        private void RewriteJp()
        {
            if (instr.Operands[0] is ConditionOperand<CondCode> co)
            {
                iclass = InstrClass.ConditionalTransfer;
                var test = GenerateTestExpression(co, false);
                var dst = RewriteSrc(instr.Operands[1]);
                if (dst is Address addr)
                {
                    m.Branch(test, addr, InstrClass.ConditionalTransfer);
                }
                else
                {
                    m.BranchInMiddleOfInstruction(
                        test.Invert(), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                    m.Goto(dst);
                }
            }
            else
            {
                iclass = InstrClass.Transfer;
                var dst = RewriteSrc(instr.Operands[0]);
                m.Goto(dst);
            }
        }

        private void RewriteRet()
        {
            if (instr.Operands.Length == 1 && instr.Operands[0] is ConditionOperand<CondCode> co)
            {
                iclass = InstrClass.ConditionalTransfer;

                var test = GenerateTestExpression(co, true);
                m.Branch(test, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                m.Return(4, 0);
            }
            else
            {
                iclass = InstrClass.Transfer;
                m.Return(4, 0);
            }
        }

        private void RewriteRetd()
        {
            iclass = InstrClass.Transfer;
            m.Return(4, ((Constant) instr.Operands[0]).ToInt32());
        }

        private void RewriteReti()
        {
            iclass = InstrClass.Transfer;
            var sr = binder.EnsureRegister(Registers.sr);
            var sp = binder.EnsureRegister(Registers.xsp);
            m.Assign(sr, m.Mem16(sp));
            m.Assign(sp, m.IAddS(sp, 2));
            m.Return(4, 0);
        }
    }
}
