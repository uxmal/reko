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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter
    {
        private void RewriteB()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            m.Goto(dst);
        }

        private void RewriteBc(bool linkRegister)
        {
            throw new NotImplementedException();
        }

        private void RewriteBcctr(bool linkRegister)
        {
            RewriteBranch(linkRegister, binder.EnsureRegister(arch.ctr));
        }

        private void RewriteBl()
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var addrDst = dst as Address?;
            if (addrDst is not null && instr.Address.ToLinear() + 4 == addrDst.Value.ToLinear())
            {
                // PowerPC idiom to get the current instruction pointer in the lr register
                iclass = InstrClass.Linear;
                m.Assign(binder.EnsureRegister(arch.lr), addrDst);
            }
            else
            {
                m.Call(dst, 0);
            }
        }

        private void RewriteBlr()
        {
            m.Return(0, 0);
        }

        private void RewriteBranch(bool updateLinkregister, bool toLinkRegister, ConditionCode cc)
        {
            Expression cr;
            var ccrOp = instr.Operands[0] as RegisterStorage;
            if (ccrOp is not null)
            {
                cr = RewriteOperand(instr.Operands[0]);
            }
            else 
            {
                cr = binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(arch.CrRegisters[0])!);
            }
            if (toLinkRegister)
            {
                m.BranchInMiddleOfInstruction(
                    m.Test(cc, cr).Invert(),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                var dst = binder.EnsureRegister(arch.lr);
                if (updateLinkregister)
                {
                    m.Call(dst, 0);
                }
                else
                {
                    m.Return(0, 0);
                }
            }
            else
            {
                var dst = RewriteOperand(ccrOp is not null ? instr.Operands[1] : instr.Operands[0]);
                if (updateLinkregister)
                {
                    m.BranchInMiddleOfInstruction(
                        m.Test(cc, cr).Invert(),
                        instr.Address + instr.Length,
                        InstrClass.ConditionalTransfer);
                    m.Call(dst, 0);
                }
                else
                {
                    m.Branch(m.Test(cc, cr), (Address)dst, InstrClass.ConditionalTransfer);
                }
            }
        }

        private ConditionCode CcFromOperand(ConditionOperand ccOp)
        {
            switch (ccOp.condition & 3)
            {
            case 0: return ConditionCode.LT;
            case 1: return ConditionCode.GT;
            case 2: return ConditionCode.EQ;
            case 3: return ConditionCode.OV;
            default: throw new NotImplementedException();
            }
        }

        private RegisterStorage CrFromOperand(ConditionOperand ccOp)
        {
            return arch.CrRegisters[(int)ccOp.condition >> 2];
        }
        
        private void RewriteCtrBranch(bool updateLinkRegister, bool toLinkRegister, Func<Expression,Expression,Expression> decOp, bool ifSet)
        {
            var ctr = binder.EnsureRegister(arch.ctr);
            Expression dest;

            Expression cond = decOp(ctr, Constant.Zero(ctr.DataType));

            if (instr.Operands[0] is ConditionOperand ccOp)
            {
                Expression test = m.Test(
                    CcFromOperand(ccOp),
                    binder.EnsureRegister(CrFromOperand(ccOp)));
                if (!ifSet)
                    test = test.Invert();
                cond = m.Cand(cond, test);
                dest = RewriteOperand(instr.Operands[1]);
            }
            else
            {
                dest = RewriteOperand(instr.Operands[0]);
            }
            
            m.Assign(ctr, m.ISub(ctr, 1));
            if (updateLinkRegister)
            {
                m.BranchInMiddleOfInstruction(
                    cond.Invert(),
                    instr.Address + instr.Length,
                    InstrClass.ConditionalTransfer);
                m.Call(dest, 0);
            }
            else
            {
                m.Branch(
                    cond,
                    (Address)dest,
                    InstrClass.ConditionalTransfer);
            }
        }

        private void RewriteBranch(bool linkRegister, Expression destination)
        {
            var ctr = binder.EnsureRegister(arch.ctr);
            var bo = ((Constant) RewriteOperand(instr.Operands[0])).ToByte();
            var bi = ((Constant) RewriteOperand(instr.Operands[1])).ToByte();
            switch (bo)
            {
            case 0x00:
            case 0x01:
                // throw new NotImplementedException("dec ctr");
                EmitUnitTest();
                iclass = InstrClass.Invalid;
                m.Invalid();
                break;
            case 0x02:
            case 0x03:
                // throw new NotImplementedException("dec ctr");
                EmitUnitTest();
                iclass = InstrClass.Invalid;
                m.Invalid();
                break;
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07:
                // Bit 0 = LT
                // Bit 1 = GT
                // Bit 2 = EQ
                // Bit 3 = SO
                ConditionCode cc;
                switch (bi)
                {
                // Fixed arithmetic flags.
                case 0: cc = ConditionCode.GE; break;
                case 1: cc = ConditionCode.LE; break;
                case 2: cc = ConditionCode.NE; break;
                case 3: cc = ConditionCode.NO; break;
                // Floating point flags.
                case 4: cc = ConditionCode.GE; break;
                case 5: cc = ConditionCode.LE; break;
                case 6: cc = ConditionCode.NE; break;
                case 7: cc = ConditionCode.NO; break;
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    return;
                }
                EmitBranch(destination, bi, cc);
                break;
            case 0x08:
            case 0x09:
                throw new NotImplementedException("dec ctr; condition false");
            case 0x0A:
            case 0x0B:
                {
                    if (destination is Address addr)
                    {
                        //$TODO implement this
                        EmitUnitTest();
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                    }
                    else
                    {
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                    }
                }
                break;
            case 0x0C:
            case 0x0D:
            case 0x0E:
            case 0x0F:
                switch (bi)
                {
                // Fixed arithmetic flags.
                case 0: cc = ConditionCode.LT; break;
                case 1: cc = ConditionCode.GT; break;
                case 2: cc = ConditionCode.EQ; break;
                case 3: cc = ConditionCode.OV; break;
                // Floating point flags
                case 4: cc = ConditionCode.LT; break;
                case 5: cc = ConditionCode.GT; break;
                case 6: cc = ConditionCode.EQ; break;
                case 7: cc = ConditionCode.OV; break;
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    return;
                }
                EmitBranch(destination, bi, cc);
                break;
            case 0x10:
            case 0x11:
            case 0x18:
            case 0x19:
                {
                    if (destination is Address addr)
                    {
                        m.Assign(ctr, m.ISub(ctr, 1));
                        m.Branch(m.Eq0(ctr), addr);
                    }
                    else
                    {
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                    }
                }
                break;
            case 0x12:
            case 0x13:
            case 0x1A:
            case 0x1B:
                {
                    if (destination is Address addr)
                    {
                        m.Assign(ctr, m.ISub(ctr, 1));
                        m.Branch(m.Eq0(ctr), addr);
                    }
                    else
                    {
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                    }
                }
                break;
            default:
                if (linkRegister)
                    m.Call(ctr, 0);
                else
                    m.Goto(ctr);
                return;
            }
        }

        private void EmitBranch(Expression destination, byte bi, ConditionCode cc)
        {
            var flag = binder.EnsureFlagGroup(arch.GetFlagGroup(arch.cr, 1u << bi));
            if (destination is Address addrDst)
            {
                m.Branch(m.Test(cc, flag), addrDst);
            }
            else
            {
                m.BranchInMiddleOfInstruction(m.Test(cc.Invert(), flag), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
                m.Goto(destination);
            }
        }

        private void RewriteSc()
        {
            m.SideEffect(m.Fn(CommonOps.Syscall_0));
        }
    }
}
