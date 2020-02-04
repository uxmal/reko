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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteBgezal(MipsInstruction instr)
        {
            // The bgezal r0,XXXX instruction is aliased to bal (branch and link, or fn call)
            // We handle that case here explicitly.
            if (((RegisterOperand)instr.Operands[0]).Register.Number == 0)
            {
                // A special case is when we call to the location after
                // the delay slot. This is an idiom to capture the 
                // program counter in the la register.
                var dst = ((AddressOperand)instr.Operands[1]).Address;
                if (instr.Address.ToLinear() + 8 == dst.ToLinear())
                {
                    iclass = InstrClass.Linear;
                    var ra = binder.EnsureRegister(arch.LinkRegister);
                    m.Assign(ra, dst);
                }
                else
                {
                    m.CallD(dst, 0);
                }
                return;
            }
            RewriteBranch0(instr, m.Ge, false);
        }

        private void RewriteBb(MipsInstruction instr, Func<Expression, Expression> condOp)
        {
            var reg = RewriteOperand0(instr.Operands[0]);
            var bit = RewriteOperand(instr.Operands[1]);
            var addr = (Address) RewriteOperand0(instr.Operands[2]);
            var test = condOp(host.PseudoProcedure("__bit", PrimitiveType.Bool, reg, bit));
            m.Branch(test, addr, instr.InstructionClass);
        }

        private void RewriteBranch(MipsInstruction instr, Func<Expression, Expression, Expression> condOp, bool link)
        {
            if (!link)
            {
                var reg1 = RewriteOperand0(instr.Operands[0]);
                var reg2 = RewriteOperand0(instr.Operands[1]);
                var addr = (Address)RewriteOperand0(instr.Operands[2]);
                var cond = condOp(reg1, reg2);
                if (condOp == m.Eq &&
                    ((RegisterOperand)instr.Operands[0]).Register ==
                    ((RegisterOperand)instr.Operands[1]).Register)
                {
                    m.Goto(addr, instr.InstructionClass & ~InstrClass.Conditional);
                }
                else
                {
                    m.Branch(cond, addr, instr.InstructionClass);
                }
            }
            else
            {
                throw new NotImplementedException("Linked branches not implemented yet.");
            }
        }

        private void RewriteBranch0(MipsInstruction instr, Func<Expression, Expression, Expression> condOp, bool link)
        {
            if (link)
            {
                m.Assign(
                    binder.EnsureRegister(arch.LinkRegister),
                    instr.Address + 8);
            }
            var reg = RewriteOperand0(instr.Operands[0]);
            var addr = (Address)RewriteOperand0(instr.Operands[1]);
            if (reg is Constant)
            {
                // r0 has been replaced with '0'.
                if (condOp == m.Lt)
                {
                    iclass = InstrClass.Linear;
                    return; // Branch will never be taken
                }
            }
            var cond = condOp(reg, Constant.Zero(reg.DataType));
            m.Branch(cond, addr, instr.InstructionClass);
        }

        private void RewriteBranchImm(MipsInstruction instr, Func<Expression, Expression, Expression> condOp, bool link)
        {
            if (link)
            {
                m.Assign(
                    binder.EnsureRegister(arch.LinkRegister),
                    instr.Address + 8);
            }
            var reg = RewriteOperand0(instr.Operands[0]);
            var imm = RewriteOperand(instr.Operands[1]);
            var addr = (Address) RewriteOperand0(instr.Operands[2]);
            var cond = condOp(reg, imm);
            m.Branch(cond, addr, instr.InstructionClass);
        }

        private void RewriteBranchLikely(MipsInstruction instr, Func<Expression, Expression, Expression> condOp)
        {
            var reg1 = RewriteOperand0(instr.Operands[0]);
            var reg2 = RewriteOperand0(instr.Operands[1]);
            var addr = (Address)RewriteOperand0(instr.Operands[2]);
            var cond = condOp(reg1, reg2);
            if (condOp == m.Eq &&
                ((RegisterOperand)instr.Operands[0]).Register ==
                ((RegisterOperand)instr.Operands[1]).Register)
            {
                m.GotoD(addr);
            }
            else
            {
                m.Branch(cond, addr, InstrClass.ConditionalTransfer | InstrClass.Delay);
            }
        }

        private void RewriteBranchConditional1(MipsInstruction instr, bool opTrue)
        {
            var cond = RewriteOperand0(instr.Operands[0]);
            if (!opTrue)
                cond = m.Not(cond);
            var addr = (Address)RewriteOperand0(instr.Operands[1]);
            iclass = InstrClass.ConditionalTransfer | InstrClass.Delay;
            m.Branch(cond, addr, iclass);
        }

        private void RewriteCall(MipsInstruction instr)
        {
            var dst = RewriteOperand0(instr.Operands[0]);
            m.Call(dst, 0, instr.InstructionClass);
        }


        private void RewriteEret(MipsInstruction instr)
        {
            // Return from exception doesn't seem to modify any 
            // GPRs (as well it shouldn't).
            // MIPS manual says it does _not_ have a delay slot.
            m.Return(0, 0);
        }

        private void RewriteJal(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            m.CallD(RewriteOperand0(instr.Operands[0]), 0);
        }

        private void RewriteJalr(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            var dst = RewriteOperand0(instr.Operands[1]);
            var lr = ((RegisterOperand)instr.Operands[0]).Register;
            if (lr == arch.LinkRegister)
            {
                m.Call(dst, 0, instr.InstructionClass);
                return;
            }
            else
            {
                m.Assign(binder.EnsureRegister(lr), instr.Address + 8);
                m.Goto(dst, instr.InstructionClass & ~InstrClass.Call);
            }
        }

        private void RewriteJr(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.Operands[0]);

            var reg = (RegisterStorage)((Identifier)dst).Storage;
            if (reg == arch.LinkRegister)
            {
                m.Return(0, 0, iclass);
            }
			else
            {
                m.Goto(dst, iclass);
            }
        }

        private void RewriteJump(MipsInstruction instr)
        {
            var dst = RewriteOperand0(instr.Operands[0]);
            m.Goto(dst, instr.InstructionClass);
        }

        private void RewriteMoveBalc(MipsInstruction instr)
        {
            var dst = RewriteOperand0(instr.Operands[0]);
            var src = RewriteOperand0(instr.Operands[1]);
            m.Assign(dst, src);
            var addr = RewriteOperand(instr.Operands[2]);
            m.Call(addr, 0);
        }

        private void RewriteSigrie(MipsInstruction instr)
        {
            var chr = new ProcedureCharacteristics
            {
                Terminates = true
            };
            m.SideEffect(host.PseudoProcedure("__reserved_instruction", chr,
                VoidType.Instance, RewriteOperand(instr.Operands[0])));
        }
    }
}
