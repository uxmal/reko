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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public partial class Pdp11Rewriter
    {
        private void RewriteBr(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            m.Goto(((AddressOperand)instr.op1).Address);
        }

        private void RewriteBxx(Pdp11Instruction instr, ConditionCode cc, FlagM flags)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            m.Branch(
                m.Test(cc, frame.EnsureFlagGroup(arch.GetFlagGroup((uint)flags))),
                ((AddressOperand)instr.op1).Address,
                RtlClass.ConditionalTransfer);
        }

        private void RewriteEmt(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            var imm = ((ImmediateOperand)instr.op1).Value.ToByte();
            var svc = m.Word16((ushort)(0x8800 | imm));
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, svc));
        }

        private void RewriteHalt()
        {
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(host.PseudoProcedure("__halt", c, VoidType.Instance));
        }

        private void RewriteJmp(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            var jmpDst = RewriteJmpSrc(instr.op1);
            m.Goto(jmpDst);
        }

        private void RewriteJsr(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            var regLink = (RegisterOperand)instr.op1;
            //$TODO: do something with regLink.
            var callDst = RewriteJmpSrc(instr.op2);
            m.Call(callDst, 2);
            return;
        }

        private void RewriteReset()
        {
            m.SideEffect(host.PseudoProcedure("__reset", VoidType.Instance));
        }

        private void RewriteRts(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            var regLink = (RegisterOperand)instr.op1;
            if (regLink.Register == Registers.pc)
            {
                m.Return(2, 0);
                return;
            }
            else
            {
                var tmp = frame.CreateTemporary(regLink.Width);
                var sp = frame.EnsureRegister(Registers.sp);
                var reg = frame.EnsureRegister(regLink.Register);
                m.Assign(tmp, reg);
                m.Assign(reg, m.Load(regLink.Width, sp));
                m.Assign(sp, m.IAdd(sp, reg.DataType.Size));
                m.Call(tmp, 0);
                m.Return(0, 0);
            }
        }

        private void RewriteTrap(Pdp11Instruction instr)
        {
            this.rtlCluster.Class = RtlClass.Transfer;
            var imm = ((ImmediateOperand)instr.op1).Value.ToByte();
            var svc = m.Word16((ushort)(0x8900 | imm));
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, svc));
        }

        private void RewriteWait()
        {
            m.SideEffect(host.PseudoProcedure("__wait", VoidType.Instance));
        }
    }
}
