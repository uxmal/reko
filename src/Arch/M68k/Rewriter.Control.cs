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
            var addr = ((M68kAddressOperand)instr.Operands[0]).Address;
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
            m.Goto(orw.RewriteSrc(instr.Operands[0], instr.Address, true));
        }

        private void RewriteBsr()
        {
            var addr = ((M68kAddressOperand)instr.Operands[0]).Address;
            if ((addr.ToUInt32() & 1) != 0)
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
            }
            else
            {
                m.Call(orw.RewriteSrc(instr.Operands[0], instr.Address, true), 4);
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
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address, true);
            var bound = orw.RewriteSrc(instr.Operands[1], instr.Address, true);
            m.Branch(m.Cand(
                    m.Ge0(src),
                    m.Le(src, bound)),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, m.Byte(6)));
        }

        private void RewriteChk2()
        {
            var reg = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var lowBound = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var ea = ((MemoryAccess)lowBound).EffectiveAddress;
            var hiBound = m.Mem(lowBound.DataType, m.IAdd(ea, lowBound.DataType.Size));
            m.Branch(
                m.Cand(
                    m.Ge(reg, lowBound),
                    m.Le(reg, hiBound)),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
                new RtlSideEffect(
                    host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, m.Byte(6)));
        }

        private void RewriteJmp()
        {
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address, true);
            if (src is MemoryAccess mem)
                src = mem.EffectiveAddress;
            m.Goto(src);
        }

        private void RewriteJsr()
        {
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address, true);
            if (src is MemoryAccess mem)
            {
                src = mem.EffectiveAddress;
            }
            m.Call(src, 4);
        }

        private void RewriteDbcc(ConditionCode cc, FlagM flags)
        {
            var addr = (Address)orw.RewriteSrc(instr.Operands[1], instr.Address, true);
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
                    instr.Address + 4,
                    InstrClass.ConditionalTransfer);
            }
            var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
            var tmp = binder.CreateTemporary(PrimitiveType.Word16);
            var tmpHi = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(tmp, m.Slice(tmp.DataType, src, 0));
            m.Assign(tmp, m.ISubS(tmp, 1));
            m.Assign(tmpHi, m.Slice(tmpHi.DataType, src, 16));
            m.Assign(src, m.Seq(tmpHi, tmp));
            m.Branch(
                m.Ne(tmp, m.Word16(0xFFFF)),
                addr,
                InstrClass.ConditionalTransfer);
        }

        private void RewriteIllegal()
        {
            if (this.instr.Operands.Length > 0)
            {
                m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, RewriteSrcOperand(this.instr.Operands[0])));
                rtlc = InstrClass.Call | InstrClass.Transfer;
            }
            else
            {
                rtlc = InstrClass.Invalid;
                m.Invalid();
            }
        }

        private void RewriteRtd()
        {
            int extraBytesPopped = ((M68kImmediateOperand) this.instr.Operands[0]).Constant.ToInt32();
            m.Return(4, extraBytesPopped);
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
            var vector = orw.RewriteSrc(instr.Operands[0], instr.Address);
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
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
            }
            var args = new List<Expression> { Constant.UInt16(7) };
            if (instr.Operands.Length > 0)
            {
                args.Add(orw.RewriteSrc(instr.Operands[0], instr.Address));
            }
            m.SideEffect(host.PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, args.ToArray()));
        }
    }
}
