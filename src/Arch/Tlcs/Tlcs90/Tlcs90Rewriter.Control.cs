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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public partial class Tlcs90Rewriter
    {
        private void RewriteCall()
        {
            if (instr.op2 != null)
            {
                var cc = RewriteCondition((ConditionOperand)instr.op1).Invert();
                m.Branch(cc, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                instr.op2.Width = PrimitiveType.Ptr16;
                m.Call(RewriteSrc(instr.op2), 2);
            }
            else
            {
                m.Call(RewriteSrc(instr.op1), 2);
            }
        }

        private void RewriteDi()
        {
            m.SideEffect(host.PseudoProcedure("__disable_interrupts", VoidType.Instance));
        }

        private void RewriteDjnz()
        {
            MachineOperand op;
            Identifier reg;
            Constant one;
            if (instr.op2 != null)
            {
                reg = (Identifier)RewriteSrc(instr.op1);
                op = instr.op2;
                one = m.Int16(1);
            }
            else
            {
                reg = binder.EnsureRegister(Registers.b);
                op = instr.op1;
                one = Constant.SByte(1);
            }
            m.Assign(reg, m.ISub(reg, one));
            m.Branch(m.Ne0(reg), ((AddressOperand)op).Address, InstrClass.ConditionalTransfer);
        }

        private void RewriteEi()
        {
            m.SideEffect(host.PseudoProcedure("__enable_interrupts", VoidType.Instance));
        }

        private void RewriteHalt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(
                host.PseudoProcedure("__halt", c, VoidType.Instance),
                InstrClass.Terminates);
        }

        private void RewriteJp()
        {
            MachineOperand op;
            if (instr.op2 != null)
            {
                var cc = RewriteCondition((ConditionOperand)instr.op1);
                var addrOp = instr.op2 as AddressOperand;
                if (addrOp != null)
                {
                    m.Branch(cc, addrOp.Address, InstrClass.ConditionalTransfer);
                    return;
                }
                m.Branch(cc.Invert(), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                op = instr.op2;
            }
            else
            {
                op = instr.op1;
            }
            op.Width = PrimitiveType.Ptr16;
            var dst = RewriteSrc(op);
            m.Goto(dst);
        }

        private void RewriteRet()
        {
            if (instr.op2 != null)
            {
                EmitUnitTest();
                Invalid();
                return;
            }
            m.Return(2, 0);
        }

        private void RewriteReti()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var af = binder.EnsureRegister(Registers.af);
            m.Assign(af, m.Mem16(sp));
            m.Assign(sp, m.IAddS(sp, 2));
            m.Return(2, 0);
        }

        private void RewriteSwi()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var af = binder.EnsureRegister(Registers.af);
            m.Assign(sp, m.ISubS(sp, 2));
            m.Assign(m.Mem16(sp), af);
            m.Call(Address.Ptr16(0x0100), 2);

        }
    }
}
