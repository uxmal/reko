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
            var dst = RewriteOperand(instr.op1);
            emitter.Goto(dst);
        }

        private void RewriteBc(bool linkRegister)
        {
            throw new NotImplementedException();
        }

        private void RewriteBcctr(bool linkRegister)
        {
            RewriteBranch(linkRegister, frame.EnsureRegister(arch.ctr));
        }

        private void RewriteBl()
        {
            var dst = RewriteOperand(instr.op1);
            var addrDst = dst as Address;
            if (addrDst != null && instr.Address.ToLinear() + 4 == addrDst.ToLinear())
            {
                // PowerPC idiom to get the current instruction pointer in the lr register
                emitter.Assign(frame.EnsureRegister(arch.lr), addrDst);
            }
            else
            {
                cluster.Class = RtlClass.Transfer;
                emitter.Call(dst, 0);
            }
        }

        private void RewriteBlr()
        {
            cluster.Class = RtlClass.Transfer;
            emitter.Return(0, 0);
        }

        private void RewriteBranch(bool updateLinkregister, bool toLinkRegister, ConditionCode cc)
        {
            cluster.Class = RtlClass.ConditionalTransfer;
            var ccrOp = instr.op1 as RegisterOperand;
            Expression cr;
            if (ccrOp != null)
            {
                cr = RewriteOperand(instr.op1);
            }
            else 
            {
                cr = frame.EnsureRegister(arch.CrRegisters[0]);
            }
            if (toLinkRegister)
            {
                var dst = frame.EnsureRegister(arch.lr);
                if (updateLinkregister)
                {
                    emitter.If(emitter.Test(cc, cr), new RtlCall(dst, 0, RtlClass.ConditionalTransfer));
                }
                else
                {
                    emitter.If(emitter.Test(cc, cr), new RtlReturn(0, 0, RtlClass.Transfer));
                }
            }
            else
            {
                var dst = RewriteOperand(ccrOp != null ? instr.op2 : instr.op1);
                if (updateLinkregister)
                {
                    emitter.If(emitter.Test(cc, cr), new RtlCall(dst, 0, RtlClass.ConditionalTransfer));
                }
                else
                {
                    emitter.Branch(emitter.Test(cc, cr), (Address)dst, RtlClass.ConditionalTransfer);
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
        
        private void RewriteCtrBranch(bool updateLinkRegister, bool toLinkRegister, Operator decOp, bool ifSet)
        {
            cluster.Class = RtlClass.ConditionalTransfer;
            var ctr = frame.EnsureRegister(arch.ctr);
            var ccOp = instr.op1 as ConditionOperand;
            Expression dest;

            Expression cond = new BinaryExpression(
                decOp, 
                PrimitiveType.Bool,
                ctr,
                Constant.Zero(ctr.DataType));

            if (ccOp != null)
            {
                Expression test = emitter.Test(
                    CcFromOperand(ccOp),
                    frame.EnsureRegister(CrFromOperand(ccOp)));
                if (!ifSet)
                    test = test.Invert();
                cond = emitter.Cand(cond, test);
                dest = RewriteOperand(instr.op2);
            }
            else
            {
                dest = RewriteOperand(instr.op1);
            }
            
            emitter.Assign(ctr, emitter.ISub(ctr, 1));
            if (updateLinkRegister)
            {
                emitter.If(
                    cond,
                    new RtlCall(dest, 0, RtlClass.ConditionalTransfer));
            }
            else
            {
                emitter.Branch(
                    cond,
                    (Address)dest,
                    RtlClass.ConditionalTransfer);
            }
        }

        private void RewriteBranch(bool linkRegister, Expression destination)
        {
            cluster.Class = RtlClass.ConditionalTransfer;
            var ctr = frame.EnsureRegister(arch.ctr);
            var bo = ((Constant)RewriteOperand(instr.op1)).ToByte();
            switch (bo)
            {
            case 0x00:
            case 0x01: throw new NotImplementedException("dec ctr");
            case 0x02:
            case 0x03: throw new NotImplementedException("dec ctr");
            case 0x04:
            case 0x05:
            case 0x06:
            case 0x07: throw new NotImplementedException("condition false");
            case 0x08:
            case 0x09: throw new NotImplementedException("dec ctr; condition false");
            case 0x0A:
            case 0x0B: throw new NotImplementedException("dec ctr; condition false");
            case 0x0C:
            case 0x0D:
            case 0x0E:
            case 0x0F: throw new NotImplementedException("condition true");
            case 0x10:
            case 0x11:
            case 0x18:
            case 0x19: throw new NotImplementedException("condition true");
            case 0x12:
            case 0x13:
            case 0x1A:
            case 0x1B: throw new NotImplementedException("condition true");
            default:
                if (linkRegister)
                    emitter.Call(ctr, 0);
                else
                    emitter.Goto(ctr);
                return;
            }
        }

        private void RewriteSc()
        {
            emitter.SideEffect(host.PseudoProcedure("__syscall", arch.WordWidth));
        }
    }
}
