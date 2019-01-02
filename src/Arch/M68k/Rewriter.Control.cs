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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.M68k
{
    public partial class Rewriter
    {
        private void RewriteBcc(ConditionCode cc, FlagM flags)
        {
            var addr = ((M68kAddressOperand)di.op1).Address;
            if ((addr.ToUInt32() & 1) != 0)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
            }
            else
            {
                m.Branch(
                    m.Test(cc, orw.FlagGroup(flags)),
                    addr,
                    InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteBra()
        {
            m.Goto(orw.RewriteSrc(di.op1, di.Address, true));
        }

        private void RewriteBsr()
        {
            var addr = ((M68kAddressOperand)di.op1).Address;
            if ((addr.ToUInt32() & 1) != 0)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
            }
            else
            {
                m.Call(orw.RewriteSrc(di.op1, di.Address, true), 4);
            }
        }

        private void RewriteCallm()
        {
            // CALLM was very rarely used, only existed on 68020.
            // Until someone really needs it, we just give up.
            EmitInvalid();
        }

        private void RewriteChk()
        {
            var src = orw.RewriteSrc(di.op1, di.Address, true);
            var bound = orw.RewriteSrc(di.op2, di.Address, true);
            m.Branch(m.Cand(
                    m.Ge0(src),
                    m.Le(src, bound)),
                di.Address + di.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, m.Byte(6)));
        }

        private void RewriteChk2()
        {
            var reg = orw.RewriteSrc(di.op2, di.Address);
            var lowBound = orw.RewriteSrc(di.op1, di.Address);
            var ea = ((MemoryAccess)lowBound).EffectiveAddress;
            var hiBound = m.Mem(lowBound.DataType, m.IAdd(ea, lowBound.DataType.Size));
            m.Branch(
                m.Cand(
                    m.Ge(reg, lowBound),
                    m.Le(reg, hiBound)),
                di.Address + di.Length,
                InstrClass.ConditionalTransfer);
                new RtlSideEffect(
                    host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, m.Byte(6)));
        }

        private void RewriteJmp()
        {
            var src = orw.RewriteSrc(di.op1, di.Address, true);
            if (src is MemoryAccess mem)
                src = mem.EffectiveAddress;
            m.Goto(src);
        }

        private void RewriteJsr()
        {
            var src = orw.RewriteSrc(di.op1, di.Address, true);
            if (src is MemoryAccess mem)
            {
                src = mem.EffectiveAddress;
            }
            m.Call(src, 4);
        }

        private void RewriteDbcc(ConditionCode cc, FlagM flags)
        {
            var addr = (Address)orw.RewriteSrc(di.op2, di.Address, true);
            if (cc == ConditionCode.ALWAYS)
            {
                rtlc = InstrClass.Transfer;
                m.Goto(addr);
            }
            rtlc = InstrClass.ConditionalTransfer;
            if (cc != ConditionCode.None)
            {
                m.BranchInMiddleOfInstruction(
                    m.Test(cc, orw.FlagGroup(flags)),
                    di.Address + 4,
                    InstrClass.ConditionalTransfer);
            }
            var src = orw.RewriteSrc(di.op1, di.Address);

            m.Assign(src, m.ISub(src, 1));
            m.Branch(
                m.Ne(src, m.Word32(-1)),
                addr,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteIllegal()
        {
            if (dasm.Current.op1 != null)
            {
                m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, RewriteSrcOperand(dasm.Current.op1)));
                rtlc = InstrClass.Call | InstrClass.Transfer;
            }
            else
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
            }
        }

        private void RewriteRtm()
        {
            // RTM was very rarely used, only existed on 68020.
            // Until someone really needs it, we just give up.
            EmitInvalid();
        }

        private void RewriteRts()
        {
            m.Return(4, 0);
        }

        private void RewriteStop()
        {
            m.SideEffect(host.PseudoProcedure("__stop", VoidType.Instance));
        }

        private void RewriteTrap()
        {
            var vector = orw.RewriteSrc(di.op1, di.Address);
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, vector));
        }

        private void RewriteTrapCc(ConditionCode cc, FlagM flags)
        {
            if (cc == ConditionCode.NEVER)
            {
                m.Nop();
                return;
            }
            if (cc != ConditionCode.ALWAYS)
            {
                rtlc |= InstrClass.Conditional;
                m.BranchInMiddleOfInstruction(
                    m.Test(cc, orw.FlagGroup(flags)).Invert(),
                    di.Address + di.Length,
                    InstrClass.ConditionalTransfer);
            }
            var args = new List<Expression> { Constant.UInt16(7) };
            if (di.op1 != null)
            {
                args.Add(orw.RewriteSrc(di.op1, di.Address));
            }
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, args.ToArray()));
        }
    }
}
