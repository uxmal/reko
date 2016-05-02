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

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteBranch(MipsInstruction instr, BinaryOperator condOp, bool link)
        {
            if (!link)
            {
                var reg1 = RewriteOperand(instr.op1);
                var reg2 = RewriteOperand(instr.op2);
                var addr = (Address)RewriteOperand(instr.op3);
                if (condOp == Operator.Eq && 
                    ((RegisterOperand)instr.op1).Register ==
                    ((RegisterOperand)instr.op2).Register)
                {
                    cluster.Class = RtlClass.Transfer;
                    emitter.GotoD(addr);
                }
                else
                {
                    var cond = new BinaryExpression(condOp, PrimitiveType.Bool, reg1, reg2);
                    cluster.Class = RtlClass.ConditionalTransfer;
                    emitter.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
                }
            }
            else
                throw new NotImplementedException("Linked branches not implemented yet.");
        }

        private void RewriteBranch0(MipsInstruction instr, BinaryOperator condOp, bool link)
        {
            if (link)
            {
                emitter.Assign(
                    frame.EnsureRegister(Registers.ra),
                    instr.Address + 8);
            }
            var reg = RewriteOperand(instr.op1);
            var addr = (Address)RewriteOperand(instr.op2);
            if (reg is Constant)
            {
                // r0 has been replaced with '0'.
                if (condOp == Operator.Lt)
                {
                    return; // Branch will never be taken
                }
            }
            var cond = new BinaryExpression(condOp, PrimitiveType.Bool, reg, Constant.Zero(reg.DataType));
            cluster.Class = RtlClass.ConditionalTransfer;
            emitter.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
        }

        private void RewriteBc1f(MipsInstruction instr, bool opTrue)
        {
            var cond = RewriteOperand(instr.op1);
            if (!opTrue)
                cond = emitter.Not(cond);
            var addr = (Address)RewriteOperand(instr.op2);
            cluster.Class = RtlClass.ConditionalTransfer | RtlClass.Delay;
            emitter.Branch(cond, addr, cluster.Class);
        }
    
        private void RewriteJal(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            cluster.Class = RtlClass.Transfer;
            emitter.CallD(RewriteOperand(instr.op1), 0);
        }

        private void RewriteJalr(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            cluster.Class = RtlClass.Transfer;
            var dst = RewriteOperand(instr.op2);
            var lr = ((RegisterOperand)instr.op1).Register;
            if (lr == Registers.ra)
            {
                emitter.CallD(dst, 0);
                return;
            }
            throw new NotImplementedException("jalr to register other than ra not implemented yet.");

        }

        private void RewriteJr(MipsInstruction instr)
        {
            cluster.Class = RtlClass.Transfer;
            var dst = RewriteOperand(instr.op1);
            var reg = (RegisterStorage)((Identifier)dst).Storage;
            if (reg == Registers.ra)
            {
                emitter.ReturnD(0, 0);
            }
			else
            {
                emitter.GotoD(dst);
            }
        }

        private void RewriteJump(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.op1);
            cluster.Class = RtlClass.Transfer;
            emitter.GotoD(dst);
        }
    }
}
