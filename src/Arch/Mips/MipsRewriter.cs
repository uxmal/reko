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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    /// <summary>
    /// Rewrites MIPS instructions into clusters of RTL instructions.
    /// </summary>
    public class MipsRewriter : IEnumerable<RtlInstructionCluster>
    {
        private IEnumerator<MipsInstruction> dasm;
        private Frame frame;
        private RtlEmitter emitter;
        private RtlInstructionCluster cluster;
        private MipsProcessorArchitecture arch;
        private IRewriterHost host;

        public MipsRewriter(MipsProcessorArchitecture arch, IEnumerable<MipsInstruction> instrs, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.frame = frame;
            this.dasm = instrs.GetEnumerator();
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                this.cluster = new RtlInstructionCluster(instr.Address, 4);
                cluster.Class = RtlClass.Linear;
                this.emitter = new RtlEmitter(cluster.Instructions);
                switch (instr.opcode)
                {
                default: throw new AddressCorrelatedException(
                    instr.Address,
                    "Rewriting of MIPS instruction {0} not implemented yet.",
                    instr.opcode);
                case Opcode.add:
                case Opcode.addi:
                case Opcode.addiu:
                case Opcode.addu:
                    RewriteAdd(instr, PrimitiveType.Word32); break;
                case Opcode.and:
                case Opcode.andi:
                    RewriteAnd(instr); break;
                case Opcode.beq:
                case Opcode.beql:
                    RewriteBranch(instr, Operator.Eq, false); break;
                case Opcode.bgez:
                case Opcode.bgezl:
                    RewriteBranch0(instr, Operator.Ge, false); break;
                case Opcode.bgezal:
                case Opcode.bgezall:
                    RewriteBranch0(instr, Operator.Ge, true); break;
                case Opcode.bgtz:
                case Opcode.bgtzl:
                    RewriteBranch0(instr, Operator.Gt, false); break;
                case Opcode.blez:
                case Opcode.blezl:
                    RewriteBranch0(instr, Operator.Le, false); break;
                case Opcode.bltz:
                case Opcode.bltzl:
                    RewriteBranch0(instr, Operator.Lt, false); break;
                case Opcode.bltzal:
                case Opcode.bltzall:
                    RewriteBranch0(instr, Operator.Lt, true); break;
                case Opcode.bne:
                case Opcode.bnel:
                    RewriteBranch(instr, Operator.Ne, true); break;
                case Opcode.@break:
                    this.host.EnsurePseudoProcedure("__break", VoidType.Instance, 0); break;
                case Opcode.dadd:
                case Opcode.daddi:
                    RewriteAdd(instr, PrimitiveType.Word64); break;
                case Opcode.daddiu:
                case Opcode.daddu:
                case Opcode.ddiv:
                case Opcode.ddivu:
                case Opcode.div:
                case Opcode.divu:
                case Opcode.dmult:
                case Opcode.dmultu:
                case Opcode.dsll:
                case Opcode.dsll32:
                case Opcode.dsllv:
                case Opcode.dsra:
                case Opcode.dsra32:
                case Opcode.dsrav:
                case Opcode.dsrl:
                case Opcode.dsrl32:
                case Opcode.dsrlv:
                case Opcode.dsub:
                case Opcode.dsubu:
                    goto default;
                case Opcode.j:
                    RewriteJump(instr); break;
                case Opcode.jal:
                    RewriteJal(instr); break;
                case Opcode.jalr:
                case Opcode.jr:
                case Opcode.lb:
                case Opcode.lbu:
                case Opcode.ld:
                case Opcode.ldl:
                case Opcode.ldr:
                    goto default;
                case Opcode.lh:
                case Opcode.lhu:
                    RewriteLoad(instr); break;
                case Opcode.ll:
                case Opcode.lld:
                    goto default;
                case Opcode.lui: RewriteLui(instr); break;
                case Opcode.lw:
                    RewriteLoad(instr); break;
                case Opcode.lwl:
                case Opcode.lwr:
                case Opcode.lwu:
                case Opcode.mfhi:
                case Opcode.mflo:
                case Opcode.mthi:
                case Opcode.mtlo:
                case Opcode.movn:
                case Opcode.movz:
                case Opcode.mult:
                case Opcode.multu:
                case Opcode.nop:
                    break;
                case Opcode.nor:
                    goto default;
                case Opcode.or:
                case Opcode.ori:
                    RewriteOr(instr); break;
                case Opcode.pref:
                case Opcode.sb:
                case Opcode.sc:
                case Opcode.scd:
                case Opcode.sd:
                case Opcode.sdl:
                case Opcode.sdr:
                case Opcode.sh:
                case Opcode.slti:
                case Opcode.sltiu:
                    goto default;
                case Opcode.srl:
                    RewriteSrl(instr); break;
                case Opcode.sw:
                    RewriteStore(instr); break;
                case Opcode.swl:
                case Opcode.swr:
                case Opcode.swu:
                case Opcode.xor:
                case Opcode.xori:
                    goto default;
                }
                yield return cluster;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        //$REVIEW: push PseudoProc into the RewriterHost interface"
        public Expression PseudoProc(string name, DataType retType, params Expression[] args)
        {
            var ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
            return PseudoProc(ppp, retType, args);
        }

        public Expression PseudoProc(PseudoProcedure ppp, DataType retType, params Expression[] args)
        {
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));

            return emitter.Fn(new ProcedureConstant(arch.PointerType, ppp), retType, args);
        }

        private void RewriteBranch(MipsInstruction instr, BinaryOperator condOp, bool link)
        {
            if (!link)
            {
                var reg1 = RewriteOperand(instr.op1);
                var reg2 = RewriteOperand(instr.op2);
                var addr = (Address)RewriteOperand(instr.op3);
                var cond = new BinaryExpression(condOp, PrimitiveType.Bool, reg1, reg2);
                cluster.Class = RtlClass.ConditionalTransfer;
                emitter.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
            }
            else
                throw new NotImplementedException("Linked branches not implemented yet.");
        }

        private void RewriteBranch0(MipsInstruction instr, BinaryOperator condOp, bool link)
        {
            if (!link)
            {
                var reg = RewriteOperand(instr.op1);
                var addr = (Address)RewriteOperand(instr.op2);
                var cond = new BinaryExpression(condOp, PrimitiveType.Bool, reg, Constant.Zero(reg.DataType));
                cluster.Class = RtlClass.ConditionalTransfer;
                emitter.Branch(cond, addr, RtlClass.ConditionalTransfer | RtlClass.Delay);
            }
            else
                throw new NotImplementedException("Linked branches not implemented yet.");
        }

        private void RewriteJal(MipsInstruction instr)
        {
            //$TODO: if we want explicit representation of the continuation of call
            // use the line below
            //emitter.Assign( frame.EnsureRegister(Registers.ra), instr.Address + 8);
            cluster.Class = RtlClass.Transfer;
            emitter.CallD(RewriteOperand(instr.op1), 0);
        }

        private void RewriteJump(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.op1);
            cluster.Class = RtlClass.Transfer;
            emitter.GotoD(dst);
        }

        private void RewriteLoad(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            if (opDst.DataType.Size != opSrc.DataType.Size)
                opSrc = emitter.Cast(arch.WordWidth, opSrc);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand)instr.op2;
            long v = immOp.Value.ToInt16();
            var opSrc = Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteAdd(MipsInstruction instr, PrimitiveType size)
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

        private void RewriteSrl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            emitter.Assign(opDst, emitter.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op1);
            var opDst = RewriteOperand(instr.op2);
            if (opDst.DataType.Size < opSrc.DataType.Size)
                opSrc = emitter.Cast(opDst.DataType, opSrc);
            emitter.Assign(opDst, opSrc);
        }

    }
}
