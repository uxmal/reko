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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    /// <summary>
    /// Rewrites MIPS instructions into clusters of RTL instructions.
    /// </summary>
    public partial class MipsRewriter : IEnumerable<RtlInstructionCluster>
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
                case Opcode.add_d: RewriteAddD(instr); break;
                case Opcode.and:
                case Opcode.andi:
                    RewriteAnd(instr); break;
                case Opcode.bc1f: RewriteBc1f(instr, false); break;
                case Opcode.beq:
                    RewriteBranch(instr, Operator.Eq, false); break;
                case Opcode.beql:
                    RewriteBranch(instr, Operator.Eq, true); break;
                case Opcode.bgez:
                    RewriteBranch0(instr, Operator.Ge, false); break;
                case Opcode.bgezl:
                    RewriteBranch0(instr, Operator.Ge, true); break;
                case Opcode.bgezal:
                    RewriteBranch0(instr, Operator.Ge, false); break;
                case Opcode.bgezall:
                    RewriteBranch0(instr, Operator.Ge, true); break;
                case Opcode.bgtz:
                    RewriteBranch0(instr, Operator.Gt, false); break;
                case Opcode.bgtzl:
                    RewriteBranch0(instr, Operator.Gt, true); break;
                case Opcode.blez:
                    RewriteBranch0(instr, Operator.Le, false); break;
                case Opcode.blezl:
                    RewriteBranch0(instr, Operator.Le, true); break;
                case Opcode.bltz:
                    RewriteBranch0(instr, Operator.Lt, false); break;
                case Opcode.bltzl:
                    RewriteBranch0(instr, Operator.Lt, true); break;
                case Opcode.bltzal:
                    RewriteBranch0(instr, Operator.Lt, true); break;
                case Opcode.bltzall:
                    RewriteBranch0(instr, Operator.Lt, true); break;
                case Opcode.bne:
                    RewriteBranch(instr, Operator.Ne, false); break;
                case Opcode.bnel:
                    RewriteBranch(instr, Operator.Ne, true); break;
                case Opcode.@break: RewriteBreak(instr); break;
                case Opcode.c_le_d: RewriteFpuCmpD(instr, Operator.Fle); break;
                case Opcode.cfc1: RewriteCfc1(instr); break;
                case Opcode.ctc1: RewriteCtc1(instr); break;
                case Opcode.cvt_w_d: RewriteCvtD(instr, PrimitiveType.Int32); break;
                case Opcode.dadd:
                case Opcode.daddi:
                    RewriteAdd(instr, PrimitiveType.Word64); break;
                case Opcode.daddiu:
                case Opcode.daddu:
                case Opcode.ddiv:
                case Opcode.ddivu:
                case Opcode.div: RewriteDiv(instr, emitter.SDiv); break;
                case Opcode.divu: RewriteDiv(instr, emitter.UDiv); break;
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
                    RewriteJalr(instr); break;
                case Opcode.jr:
                    RewriteJr(instr); break;
                case Opcode.lb:
                case Opcode.lbu:
                    RewriteLoad(instr); break;
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
                case Opcode.lw: RewriteLoad(instr); break;
                case Opcode.lwl: RewriteLwl(instr); break;
                case Opcode.lwr: RewriteLwr(instr); break;
                case Opcode.lwu:
                    goto default;
                case Opcode.mfc0: RewriteMfc0(instr); break;
                case Opcode.mfc1: RewriteMfc1(instr); break;
                case Opcode.mfhi: RewriteMf(instr, Registers.hi); break;
                case Opcode.mflo: RewriteMf(instr, Registers.lo); break;
                case Opcode.mthi: RewriteMt(instr, Registers.hi); break;
                case Opcode.mtlo: RewriteMt(instr, Registers.lo); break;
                case Opcode.movn:
                case Opcode.movz:
                    goto default;
                case Opcode.mtc1: RewriteMtc1(instr); break;
                case Opcode.mult: RewriteMul(instr, emitter.SMul, PrimitiveType.Int64); break;
                case Opcode.multu: RewriteMul(instr, emitter.UMul, PrimitiveType.UInt64); break;
                case Opcode.nop: emitter.Nop(); break;
                case Opcode.nor: RewriteNor(instr); break;
                case Opcode.or:
                case Opcode.ori:
                    RewriteOr(instr); break;
                case Opcode.pref:
                    goto default;
                case Opcode.sb: RewriteStore(instr); break;
                case Opcode.sc:
                case Opcode.scd:
                case Opcode.sd:
                case Opcode.sdl:
                case Opcode.sdr:
                    goto default;
                case Opcode.sh: RewriteStore(instr); break;
                case Opcode.sll:
                case Opcode.sllv:
                    RewriteSll(instr); break;
                case Opcode.slt: RewriteSxx(instr, Operator.Lt); break;
                case Opcode.slti: RewriteSxx(instr, Operator.Lt); break;
                case Opcode.sltiu: RewriteSxx(instr, Operator.Ult); break;
                case Opcode.sltu: RewriteSxx(instr, Operator.Ult); break;
                case Opcode.sra:
                case Opcode.srav:
                    RewriteSra(instr); break;
                case Opcode.srl:
                case Opcode.srlv:
                    RewriteSrl(instr); break;
                case Opcode.sub:
                case Opcode.subu:
                    RewriteSub(instr); break;
                case Opcode.sw:
                case Opcode.swc1:
                    RewriteStore(instr); break;
                case Opcode.swl: RewriteSwl(instr); break;
                case Opcode.swr: RewriteSwr(instr); break;
                case Opcode.swu:
                    goto default;
                case Opcode.tge: RewriteTrap(instr, Operator.Ge); break;
                case Opcode.xor:
                case Opcode.xori:
                    RewriteXor(instr); break;
                }
                yield return cluster;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
