#region License
/*
 * Copyright (C) 1999-2018 John Källén.
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
        private IStorageBinder binder;
        private RtlEmitter m;
        private InstrClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private MipsProcessorArchitecture arch;
        private IRewriterHost host;
        private ExpressionValueComparer cmp;

        public MipsRewriter(MipsProcessorArchitecture arch, IEnumerable<MipsInstruction> instrs, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.dasm = instrs.GetEnumerator();
            this.host = host;
            this.cmp = new ExpressionValueComparer();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = InstrClass.Linear;
                this.m = new RtlEmitter(rtlInstructions);
                switch (instr.opcode)
                {
                default:
                    throw new AddressCorrelatedException(
               instr.Address,
               "Rewriting of MIPS instruction {0} not implemented yet.",
               instr.opcode);
                case Opcode.illegal:
                    rtlc = InstrClass.Invalid; m.Invalid(); break;
                case Opcode.add:
                case Opcode.addi:
                case Opcode.addiu:
                case Opcode.addu:
                    RewriteAdd(instr, PrimitiveType.Word32); break;
                case Opcode.add_d: RewriteFpuBinopD(instr, m.FAdd); break;
                case Opcode.and:
                case Opcode.andi:
                    RewriteAnd(instr); break;
                case Opcode.bc1f: RewriteBranchConditional1(instr, false); break;
                case Opcode.bc1t: RewriteBranchConditional1(instr, true); break;
                case Opcode.beq: RewriteBranch(instr, m.Eq, false); break;
                case Opcode.beql: RewriteBranchLikely(instr, m.Eq); break;
                case Opcode.bgez:
                    RewriteBranch0(instr, m.Ge, false); break;
                case Opcode.bgezl:
                    RewriteBranch0(instr, m.Ge, true); break;
                case Opcode.bgezal:
                    RewriteBranch0(instr, m.Ge, false); break;
                case Opcode.bgezall:
                    RewriteBranch0(instr, m.Ge, true); break;
                case Opcode.bgtz:
                    RewriteBranch0(instr, m.Gt, false); break;
                case Opcode.bgtzl:
                    RewriteBranch0(instr, m.Gt, true); break;
                case Opcode.blez:
                    RewriteBranch0(instr, m.Le, false); break;
                case Opcode.blezl:
                    RewriteBranch0(instr, m.Le, true); break;
                case Opcode.bltz:
                    RewriteBranch0(instr, m.Lt, false); break;
                case Opcode.bltzl:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Opcode.bltzal:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Opcode.bltzall:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Opcode.bne:
                    RewriteBranch(instr, m.Ne, false); break;
                case Opcode.bnel: RewriteBranchLikely(instr, m.Ne); break;
                case Opcode.@break: RewriteBreak(instr); break;
                case Opcode.c_le_d: RewriteFpuCmpD(instr, Operator.Fle); break;
                case Opcode.c_le_s: RewriteFpuCmpD(instr, Operator.Fle); break;
                case Opcode.c_lt_d: RewriteFpuCmpD(instr, Operator.Flt); break;
                case Opcode.c_lt_s: RewriteFpuCmpD(instr, Operator.Flt); break;
                case Opcode.c_eq_d: RewriteFpuCmpD(instr, Operator.Feq); break;
                case Opcode.c_eq_s: RewriteFpuCmpD(instr, Operator.Feq); break;
                case Opcode.cfc1: RewriteCfc1(instr); break;
                case Opcode.ctc1: RewriteCtc1(instr); break;
                case Opcode.cvt_d_l: RewriteCvtD(instr, PrimitiveType.Real64); break;
                case Opcode.cvt_s_d: RewriteCvtD(instr, PrimitiveType.Real32); break;
                case Opcode.cvt_w_d: RewriteCvtD(instr, PrimitiveType.Int32); break;
                case Opcode.dadd:
                case Opcode.daddi:
                    RewriteAdd(instr, PrimitiveType.Word64); break;
                case Opcode.daddiu:
                case Opcode.daddu: RewriteAdd(instr, PrimitiveType.Word64); break;
                case Opcode.ddiv: RewriteDiv(instr, m.SDiv); break;
                case Opcode.ddivu: RewriteDiv(instr, m.UDiv); break;
                case Opcode.div: RewriteDiv(instr, m.SDiv); break;
                case Opcode.divu: RewriteDiv(instr, m.UDiv); break;
                case Opcode.div_d: RewriteFpuBinopD(instr, m.FDiv); break;
                case Opcode.dmfc1: RewriteMfc1(instr); break;
                case Opcode.dmtc1: RewriteMtc1(instr); break;
                case Opcode.dmult: RewriteMul(instr, m.SMul, PrimitiveType.Int128); break;
                case Opcode.dmultu: RewriteMul(instr, m.UMul, PrimitiveType.UInt128); break;
                case Opcode.dsll: RewriteSll(instr); break;
                case Opcode.dsll32: RewriteDshift32(instr, m.Shl); break;
                case Opcode.dsllv: RewriteSrl(instr); break;
                case Opcode.dsra: RewriteSra(instr); break;
                case Opcode.dsra32: RewriteDshift32(instr, m.Sar); break;
                case Opcode.dsrav: RewriteSra(instr); break;
                case Opcode.dsrl: RewriteSrl(instr); break;
                case Opcode.dsrl32: RewriteDshift32(instr, m.Shr); break;
                case Opcode.dsrlv: RewriteSrl(instr); break;
                case Opcode.dsub:
                case Opcode.dsubu:
                    RewriteSub(instr, PrimitiveType.Word64); break;
                case Opcode.j: RewriteJump(instr); break;
                case Opcode.jal: RewriteJal(instr); break;
                case Opcode.jalr: RewriteJalr(instr); break;
                case Opcode.jr: RewriteJr(instr); break;
                case Opcode.lb: RewriteLoad(instr, PrimitiveType.SByte); break;
                case Opcode.lbu: RewriteLoad(instr, PrimitiveType.Byte); break;
                case Opcode.ld: RewriteLoad(instr, PrimitiveType.Word64); break;
                case Opcode.ldl: RewriteLdl(instr); break;
                case Opcode.ldr: RewriteLdr(instr); break;
                case Opcode.ldc1: RewriteLdc1(instr); break;
                case Opcode.lh: RewriteLoad(instr, PrimitiveType.Int16); break;
                case Opcode.lhu: RewriteLoad(instr, PrimitiveType.UInt16); break;
                case Opcode.ll: RewriteLoadLinked32(instr); break;
                case Opcode.lld: RewriteLoadLinked64(instr); break;
                case Opcode.lui: RewriteLui(instr); break;
                case Opcode.lw: RewriteLoad(instr, PrimitiveType.Int32); break;
                case Opcode.lwc1: RewriteLoad(instr, PrimitiveType.Real32); break;
                case Opcode.lwl: RewriteLwl(instr); break;
                case Opcode.lwr: RewriteLwr(instr); break;
                case Opcode.lwu: RewriteLoad(instr, PrimitiveType.UInt32); break;
                case Opcode.mfc0: RewriteMfc0(instr); break;
                case Opcode.mfc1: RewriteMfc1(instr); break;
                case Opcode.mfhi: RewriteMf(instr, arch.hi); break;
                case Opcode.mflo: RewriteMf(instr, arch.lo); break;
                case Opcode.mthi: RewriteMt(instr, arch.hi); break;
                case Opcode.mtlo: RewriteMt(instr, arch.lo); break;
                case Opcode.movn: RewriteMovCc(instr, m.Ne0); break;
                case Opcode.movz: RewriteMovCc(instr, m.Eq0); break;
                case Opcode.mov_d: RewriteCopy(instr); break;
                case Opcode.mov_s: RewriteCopy(instr); break;
                case Opcode.mtc1: RewriteMtc1(instr); break;
                case Opcode.mult: RewriteMul(instr, m.SMul, PrimitiveType.Int64); break;
                case Opcode.multu: RewriteMul(instr, m.UMul, PrimitiveType.UInt64); break;
                case Opcode.mul_s: RewriteMul(instr, m.FMul, PrimitiveType.Real32); break;
                case Opcode.mul_d: RewriteMulD(instr); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.nor: RewriteNor(instr); break;
                case Opcode.or:
                case Opcode.ori: RewriteOr(instr); break;
                case Opcode.pref:
                    goto default;
                case Opcode.sb: RewriteStore(instr); break;
                case Opcode.sc: RewriteStoreConditional32(instr); break;
                case Opcode.scd: RewriteStoreConditional64(instr); break;
                case Opcode.sd: RewriteStore(instr); break;
                case Opcode.sdc1: RewriteStore(instr); break;
                case Opcode.sdl: RewriteSdl(instr); break;
                case Opcode.sdr: RewriteSdr(instr); break;
                case Opcode.sh: RewriteStore(instr); break;
                case Opcode.sll:
                case Opcode.sllv:
                    RewriteSll(instr); break;
                case Opcode.slt: RewriteSxx(instr, m.Lt); break;
                case Opcode.slti: RewriteSxx(instr, m.Lt); break;
                case Opcode.sltiu: RewriteSxx(instr, m.Ult); break;
                case Opcode.sltu: RewriteSxx(instr, m.Ult); break;
                case Opcode.sra:
                case Opcode.srav:
                    RewriteSra(instr); break;
                case Opcode.srl:
                case Opcode.srlv:
                    RewriteSrl(instr); break;
                case Opcode.sub:
                case Opcode.subu:
                    RewriteSub(instr, PrimitiveType.Word32); break;
                case Opcode.sub_d: RewriteFpuBinopD(instr, m.FSub); break;
                case Opcode.sw:
                case Opcode.swc1:
                    RewriteStore(instr); break;
                case Opcode.swl: RewriteSwl(instr); break;
                case Opcode.swr: RewriteSwr(instr); break;
                case Opcode.sync: RewriteSync(instr); break;
                case Opcode.syscall: RewriteSyscall(instr); break;
                case Opcode.teq: RewriteTrap(instr, m.Eq); break;
                case Opcode.tge: RewriteTrap(instr, m.Ge); break;
                case Opcode.tgeu: RewriteTrap(instr, m.Uge); break;
                case Opcode.tlt: RewriteTrap(instr, m.Lt); break;
                case Opcode.tltu: RewriteTrap(instr, m.Ult); break;
                case Opcode.tne: RewriteTrap(instr, m.Ne); break;
                case Opcode.trunc_l_d: RewriteTrunc(instr, "trunc", PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Opcode.xor:
                case Opcode.xori:
                    RewriteXor(instr); break;
                case Opcode.rdhwr: RewriteReadHardwareRegister(instr); break;
                }
                yield return new RtlInstructionCluster(
                    instr.Address,
                    4,
                    rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
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
                return binder.EnsureRegister(regOp.Register);
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
                Identifier baseReg = binder.EnsureRegister(indOp.Base);
                if (indOp.Offset == 0)
                    ea = baseReg;
                else if (indOp.Offset > 0)
                    ea = m.IAdd(baseReg, indOp.Offset);
                else
                    ea = m.ISub(baseReg, -indOp.Offset);
                return m.Mem(indOp.Width, ea);
            }
            var addrOp = op as AddressOperand;
            if (addrOp != null)
            {
                return addrOp.Address;
            }
            throw new NotImplementedException(string.Format("Rewriting of operand type {0} not implemented yet.", op.GetType().Name));
        }

        private Expression RewriteOperand0(MachineOperand op)
        {
            var regOp = op as RegisterOperand;
            if (regOp != null)
            {
                if (regOp.Register.Number == 0)
                    return Constant.Zero(regOp.Register.DataType);
                return binder.EnsureRegister(regOp.Register);
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
                Identifier baseReg = binder.EnsureRegister(indOp.Base);
                if (indOp.Offset == 0)
                    ea = baseReg;
                else if (indOp.Offset > 0)
                    ea = m.IAdd(baseReg, indOp.Offset);
                else
                    ea = m.ISub(baseReg, -indOp.Offset);
                return m.Mem(indOp.Width, ea);
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
