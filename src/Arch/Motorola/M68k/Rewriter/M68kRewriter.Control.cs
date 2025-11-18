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

using Reko.Arch.Motorola.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.Motorola.M68k.Rewriter
{
    public partial class M68kRewriter
    {
        private void RewriteBcc(ConditionCode cc, FlagGroupStorage flags)
        {
            var addr = (Address)instr.Operands[0];
            if ((addr.ToUInt32() & 1) != 0)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
            }
            else
            {
                m.Branch(
                    m.Test(cc, binder.EnsureFlagGroup(flags)),
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
            var addr = (Address) instr.Operands[0];
            if ((addr.ToUInt32() & 1) != 0)
            {
                iclass = InstrClass.Invalid;
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
                m.Fn(CommonOps.Syscall_1, m.Byte(6)));
        }

        private void RewriteChk2()
        {
            var reg = orw.RewriteSrc(instr.Operands[1], instr.Address);
            var lowBound = orw.RewriteSrc(instr.Operands[0], instr.Address);
            if (lowBound is MemoryAccess memLo)
            {
                var ea = memLo.EffectiveAddress;
                var hiBound = m.Mem(lowBound.DataType, m.IAdd(ea, lowBound.DataType.Size));
                m.Branch(
                    m.Cand(
                        m.Ge(reg, lowBound),
                        m.Le(reg, hiBound)),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                m.SideEffect(
                    m.Fn(CommonOps.Syscall_1, m.Byte(6)),
                    InstrClass.Linear);
            }
            else
            {
                EmitInvalid();
            }
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

        private void RewriteDbcc(ConditionCode cc, FlagGroupStorage? flags)
        {
            var addr = (Address)orw.RewriteSrc(instr.Operands[1], instr.Address, true);
            if (cc == ConditionCode.ALWAYS)
            {
                iclass = InstrClass.Transfer;
                m.Goto(addr);
            }
            else
            {
                iclass = InstrClass.ConditionalTransfer;
                if (cc != ConditionCode.None)
                {
                    m.BranchInMiddleOfInstruction(
                        m.Test(cc, binder.EnsureFlagGroup(flags!)),
                        instr.Address + 4,
                        InstrClass.ConditionalTransfer);
                }
                var src = orw.RewriteSrc(instr.Operands[0], instr.Address);
                var tmp = binder.CreateTemporary(PrimitiveType.Word16);
                var tmpHi = binder.CreateTemporary(PrimitiveType.Word16);
                m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
                m.Assign(tmp, m.ISubS(tmp, 1));
                m.Assign(tmpHi, m.Slice(src, tmpHi.DataType, 16));
                m.Assign(src, m.Seq(tmpHi, tmp));
                m.Branch(
                    m.Ne(tmp, m.Word16(0xFFFF)),
                    addr,
                    InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteIllegal()
        {
            if (this.instr.Operands.Length > 0)
            {
                // Used to model A-line and F-line instructions.
                m.SideEffect(m.Fn(CommonOps.Syscall_1, RewriteSrcOperand(this.instr.Operands[0])));
                iclass = InstrClass.Call | InstrClass.Transfer;
            }
            else
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
            }
        }

        private void RewriteRtd()
        {
            int extraBytesPopped = ((Constant)this.instr.Operands[0]).ToInt32();
            m.Return(4, extraBytesPopped);
        }

        private void RewriteRtm()
        {
            // RTM was very rarely used, only existed on 68020.
            // Until someone really needs it, we just give up.
            EmitInvalid();
        }

        private void RewriteRtr()
        {
            var sp = binder.EnsureRegister(arch.StackRegister);
            m.Assign(binder.EnsureRegister(Registers.ccr), m.Mem16(sp));
            m.Assign(sp, m.IAddS(sp, 2));
            m.Return(4, 0);
        }

        private void RewriteRts()
        {
            m.Return(4, 0);
        }

        private void RewriteStop()
        {
            m.SideEffect(m.Fn(stop_intrinsic));
        }

        private void RewriteTrap()
        {
            var vector = orw.RewriteSrc(instr.Operands[0], instr.Address);
            m.SideEffect(m.Fn(CommonOps.Syscall_1, vector));
        }

        private void RewriteTrapCc(ConditionCode cc, FlagGroupStorage? flags)
        {
            if (cc == ConditionCode.NEVER)
            {
                m.Nop();
                return;
            }
            if (cc != ConditionCode.ALWAYS)
            {
                iclass |= InstrClass.Conditional;
                m.Branch(
                    m.Test(cc, binder.EnsureFlagGroup(flags!)).Invert(),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
            }
            var args = new List<Expression> { m.UInt16(7) };
            Expression call;
            if (instr.Operands.Length > 0)
            {
                args.Add(orw.RewriteSrc(instr.Operands[0], instr.Address));
                call = m.Fn(CommonOps.Syscall_2, args.ToArray());
            }
            else
            {
                call = m.Fn(CommonOps.Syscall_1, args.ToArray());
            }
            m.SideEffect(call);
        }
    }
}
