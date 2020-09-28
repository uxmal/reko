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
        private void RewriteBpt()
        {
            this.rtlc = InstrClass.Call | InstrClass.Transfer;
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.psw, (uint)(FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF)));
            m.Assign(grf, host.PseudoProcedure("__bpt", PrimitiveType.Byte)); 
        }

        private void RewriteBr()
        {
            this.rtlc = InstrClass.Transfer;
            m.Goto(((AddressOperand)instr.Operands[0]).Address);
        }

        private void RewriteBxx(ConditionCode cc, FlagM flags)
        {
            this.rtlc = InstrClass.Transfer;
            m.Branch(
                m.Test(cc, binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.psw, (uint)flags))),
                ((AddressOperand)instr.Operands[0]).Address,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteEmt()
        {
            this.rtlc = InstrClass.Transfer;
            var imm = ((ImmediateOperand)instr.Operands[0]).Value.ToByte();
            var svc = m.Word16((ushort)(0x8800 | imm));
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, svc));
        }

        private void RewriteHalt()
        {
            rtlc = InstrClass.Terminates;
            var c = new ProcedureCharacteristics
            {
                Terminates = true,
            };
            m.SideEffect(host.PseudoProcedure("__halt", c, VoidType.Instance));
        }

        private void RewriteIot()
        {
            this.rtlc = InstrClass.Call | InstrClass.Transfer;
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup(Registers.psw, (uint)(FlagM.NF | FlagM.ZF | FlagM.VF | FlagM.CF)));
            m.Assign(grf, host.PseudoProcedure("__bpt", PrimitiveType.Byte));
        }

        private void RewriteJmp()
        {
            var jmpDst = RewriteJmpSrc(instr.Operands[0]);
            if (jmpDst != null)
            {
                this.rtlc = InstrClass.Transfer;
                m.Goto(jmpDst);
            }
            else
            {
                m.Invalid();
            }
        }

        private void RewriteJsr()
        {
            //$TODO: do something with regLink.
            var callDst = RewriteJmpSrc(instr.Operands[1]);
            if (callDst != null)
            {
                var regLink = binder.EnsureRegister(((RegisterOperand)instr.Operands[0]).Register);
                if (regLink.Storage != Registers.pc)
                {
                    var sp = binder.EnsureRegister(Registers.sp);
                    m.Assign(sp, m.ISubS(sp, 2));
                    m.Assign(m.Mem16(sp), regLink);

                    m.Assign(regLink, instr.Address + instr.Length);
                    this.rtlc = InstrClass.Transfer;
                    m.Goto(callDst);
                }
                else
                {
                    this.rtlc = InstrClass.Transfer | InstrClass.Call;
                    m.Call(callDst, 2);
                }
            }
            else
            {
                m.Invalid();
            }
        }

        private void RewriteMark()
        {
            rtlc = InstrClass.Transfer;
            var sp = binder.EnsureRegister(Registers.sp);
            var pc = binder.EnsureRegister(Registers.pc);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var r5 = binder.EnsureRegister(Registers.r5);
            m.Assign(sp, m.IAdd(pc,
                Constant.Int16((short)(2 *
                ((ImmediateOperand)instr.Operands[0]).Value.ToInt16()))));
            m.Assign(tmp, r5);
            m.Assign(r5, m.Mem16(sp));
            m.Assign(sp, m.IAdd(sp, 2));
            m.Goto(tmp);
        }

        private void RewriteReset()
        {
            m.SideEffect(host.PseudoProcedure("__reset", VoidType.Instance));
        }

        private void RewriteRti()
        {
            this.rtlc = InstrClass.Transfer;
            m.Return(2, 2);
        }

        private void RewriteRts()
        {
            this.rtlc = InstrClass.Transfer;
            var regLink = (RegisterOperand)instr.Operands[0];
            if (regLink.Register == Registers.pc)
            {
                m.Return(2, 0);
                return;
            }
            else
            {
                var tmp = binder.CreateTemporary(regLink.Width);
                var sp = binder.EnsureRegister(Registers.sp);
                var reg = binder.EnsureRegister(regLink.Register);
                m.Assign(tmp, reg);
                m.Assign(reg, m.Mem(regLink.Width, sp));
                m.Assign(sp, m.IAddS(sp, reg.DataType.Size));
                m.Goto(tmp);
            }
        }

        private void RewriteRtt()
        {
            this.rtlc = InstrClass.Transfer;
            m.Return(2, 2);
        }

        private void RewriteSob()
        {
            this.rtlc = InstrClass.ConditionalTransfer;
            var reg = RewriteSrc(instr.Operands[0]);
            if (reg == null)
            {
                m.Invalid();
            }
            else
            {
                m.Assign(reg, m.ISub(reg, 1));
                m.Branch(m.Ne0(reg), ((AddressOperand)instr.Operands[1]).Address, this.rtlc);
            }
        }

        private void RewriteTrap()
        {
            this.rtlc = InstrClass.Transfer;
            var imm = ((ImmediateOperand)instr.Operands[0]).Value.ToByte();
            var svc = m.Word16((ushort)(0x8900 | imm));
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, svc));
        }

        private void RewriteWait()
        {
            m.SideEffect(host.PseudoProcedure("__wait", VoidType.Instance));
        }
    }
}
