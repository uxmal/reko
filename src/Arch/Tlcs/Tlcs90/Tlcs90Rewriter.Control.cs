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
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.Arch.Tlcs.Tlcs90
{
    public partial class Tlcs90Rewriter
    {
        private void RewriteCall()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            if (instr.op2 != null)
            {
                rtlc |= RtlClass.Conditional;
                var cc = RewriteCondition((ConditionOperand)instr.op1).Invert();
                m.Branch(cc, instr.Address + instr.Length, RtlClass.ConditionalTransfer);
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
            rtlc = RtlClass.ConditionalTransfer;
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
            m.Branch(m.Ne0(reg), ((AddressOperand)op).Address, RtlClass.ConditionalTransfer);
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
            m.SideEffect(host.PseudoProcedure("__halt", c, VoidType.Instance));
        }

        private void RewriteJp()
        {
            rtlc = RtlClass.Transfer;
            MachineOperand op;
            if (instr.op2 != null)
            {
                var cc = RewriteCondition((ConditionOperand)instr.op1);
                var addrOp = instr.op2 as AddressOperand;
                if (addrOp != null)
                {
                    m.Branch(cc, addrOp.Address, RtlClass.ConditionalTransfer);
                    return;
                }
                m.Branch(cc.Invert(), instr.Address + instr.Length, RtlClass.ConditionalTransfer);
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
            rtlc = RtlClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteReti()
        {
            rtlc = RtlClass.Transfer;
            var sp = binder.EnsureRegister(Registers.sp);
            var af = binder.EnsureRegister(Registers.af);
            m.Assign(af, m.LoadW(sp));
            m.Assign(sp, m.IAdd(sp, m.Int32(2)));
            m.Return(2, 0);
        }

        private void RewriteSwi()
        {
            rtlc = RtlClass.Transfer | RtlClass.Call;
            var sp = binder.EnsureRegister(Registers.sp);
            var af = binder.EnsureRegister(Registers.af);
            m.Assign(sp, m.ISub(sp, m.Int32(2)));
            m.Assign(m.LoadW(sp), af);
            m.Call(Address.Ptr16(0x0100), 2);

        }
    }
}
