#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mips
{
    /// <summary>
    /// Rewrites MIPS instructions into clusters of RTL instructions.
    /// </summary>
    public class MipsRewriter : IEnumerable<RtlInstructionCluster>
    {
        private IEnumerator<MipsInstruction> dasm;
        private Frame frame;
        private RtlEmitter emitter;
        private MipsProcessorArchitecture arch;

        public MipsRewriter(MipsProcessorArchitecture arch, IEnumerable<MipsInstruction> instrs, Frame frame)
        {
            this.arch = arch;
            this.frame = frame;
            this.dasm = instrs.GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            if (!dasm.MoveNext())
                yield break;
            var instr = dasm.Current;
            var cluster = new RtlInstructionCluster(instr.Address, 4);
            this.emitter = new RtlEmitter(cluster.Instructions);
            switch (instr.opcode)
            {
            default: throw new AddressCorrelatedException(
                instr.Address,
                "Rewriting of MIPS instruction {0} not implemented yet.",
                instr.opcode);
            case Opcode.add:
            case Opcode.addi:
                RewriteAdd(instr); break;
            case Opcode.and:
            case Opcode.andi:
                RewriteAnd(instr); break;
            case Opcode.bgtz:
                RewriteBranch0(instr, Operator.Gt); break;
            case Opcode.j:
                RewriteJump(instr); break;
            case Opcode.lh:
            case Opcode.lhu:
                RewriteLoad(instr); break;
            case Opcode.lui: RewriteLui(instr); break;
            case Opcode.or:
            case Opcode.ori: 
                RewriteOr(instr); break;
            }
            yield return cluster;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RewriteBranch0(MipsInstruction instr, BinaryOperator condOp)
        {
            var reg = RewriteOperand(instr.op1);
            var addr = (Address) RewriteOperand(instr.op2);
            var cond = new BinaryExpression(condOp, PrimitiveType.Bool, reg, Constant.Zero(reg.DataType));
            emitter.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
        }

        private void RewriteJump(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.op1);
            emitter.GotoD(dst);
        }

        private void RewriteLoad(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, emitter.Cast(arch.WordWidth, opSrc));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand) instr.op2;
            long v = immOp.Value.ToInt16();
            var opSrc =  Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteAdd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = emitter.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteAnd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opLeft;
            else if (opRight.IsZero)
                opSrc = opRight;
            else
                opSrc = emitter.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteOr(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else 
                opSrc = emitter.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private Expression RewriteOperand(MachineOperand op)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                if (regOp.Register.Number == 0)
                    return Constant.Zero(regOp.Register.DataType);
                return frame.EnsureRegister(regOp.Register);
            }
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                return immOp.Value;
            }
            var indOp = op as IndirectOperand;
            if (indOp != null)
            {
                Expression ea;
                Identifier baseReg = frame.EnsureRegister(indOp.Base);
                if (indOp.Offset == 0)
                    ea = baseReg;
                else if (indOp.Offset > 0)
                    ea = emitter.IAdd(baseReg, indOp.Offset);
                else
                    ea = emitter.ISub(baseReg, -indOp.Offset);
                return emitter.Load(indOp.Width, ea);
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            throw new NotImplementedException(string.Format("Rewriting of operand type {0} not implemented yet.", op.GetType().Name));
        }
    }
}
