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
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    /// <summary>
    /// Rewrites MIPS instructions into clusters of RTL instructions.
    /// </summary>
    public partial class MipsRewriter : IEnumerable<RtlInstructionCluster>
    {
        protected readonly EndianImageReader rdr;
        protected readonly IEnumerator<MipsInstruction> dasm;
        protected readonly IStorageBinder binder;
        protected readonly MipsProcessorArchitecture arch;
        protected readonly IRewriterHost host;
        protected readonly ExpressionValueComparer cmp;
        protected RtlEmitter m;
        protected InstrClass iclass;

        public MipsRewriter(MipsProcessorArchitecture arch, EndianImageReader rdr, IEnumerable<MipsInstruction> instrs, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.rdr = rdr;
            this.dasm = instrs.GetEnumerator();
            this.host = host;
            this.cmp = new ExpressionValueComparer();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var instr = dasm.Current;
                var rtlInstructions = new List<RtlInstruction>();
                this.iclass = instr.InstructionClass;
                this.m = new RtlEmitter(rtlInstructions);
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address,
                        string.Format("MIPS instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal:
                    iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add:
                case Mnemonic.addi:
                case Mnemonic.addiu:
                case Mnemonic.addu:
                    RewriteAdd(instr, PrimitiveType.Word32); break;
                case Mnemonic.add_s: RewriteFpuBinopS(instr, m.FAdd); break;
                case Mnemonic.add_d: RewriteFpuBinopD(instr, m.FAdd); break;
                    case Mnemonic.and:
                case Mnemonic.andi:
                    RewriteAnd(instr); break;
                case Mnemonic.bc1f: RewriteBranchConditional1(instr, false); break;
                case Mnemonic.bc1t: RewriteBranchConditional1(instr, true); break;
                case Mnemonic.beq: RewriteBranch(instr, m.Eq, false); break;
                case Mnemonic.beql: RewriteBranchLikely(instr, m.Eq); break;
                case Mnemonic.bgez:
                    RewriteBranch0(instr, m.Ge, false); break;
                case Mnemonic.bgezl:
                    RewriteBranch0(instr, m.Ge, true); break;
                case Mnemonic.bgezal:
                    RewriteBgezal(instr); break;
                case Mnemonic.bgezall:
                    RewriteBranch0(instr, m.Ge, true); break;
                case Mnemonic.bgtz:
                    RewriteBranch0(instr, m.Gt, false); break;
                case Mnemonic.bgtzl:
                    RewriteBranch0(instr, m.Gt, true); break;
                case Mnemonic.blez:
                    RewriteBranch0(instr, m.Le, false); break;
                case Mnemonic.blezl:
                    RewriteBranch0(instr, m.Le, true); break;
                case Mnemonic.bltz:
                    RewriteBranch0(instr, m.Lt, false); break;
                case Mnemonic.bltzl:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Mnemonic.bltzal:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Mnemonic.bltzall:
                    RewriteBranch0(instr, m.Lt, true); break;
                case Mnemonic.bne:
                    RewriteBranch(instr, m.Ne, false); break;
                case Mnemonic.bnel: RewriteBranchLikely(instr, m.Ne); break;
                case Mnemonic.@break: RewriteBreak(instr); break;
                case Mnemonic.c_le_d: RewriteFpuCmpD(instr, Operator.Fle); break;
                case Mnemonic.c_le_s: RewriteFpuCmpD(instr, Operator.Fle); break;
                case Mnemonic.c_lt_d: RewriteFpuCmpD(instr, Operator.Flt); break;
                case Mnemonic.c_lt_s: RewriteFpuCmpD(instr, Operator.Flt); break;
                case Mnemonic.c_eq_d: RewriteFpuCmpD(instr, Operator.Feq); break;
                case Mnemonic.c_eq_s: RewriteFpuCmpD(instr, Operator.Feq); break;
                case Mnemonic.cache: RewriteCache(instr); break;
                case Mnemonic.cfc1: RewriteCfc1(instr); break;
                case Mnemonic.ctc1: RewriteCtc1(instr); break;
                case Mnemonic.clo: RewriteClo(instr); break;
                case Mnemonic.clz: RewriteClz(instr); break;
                case Mnemonic.cvt_d_l: RewriteCvtD(instr, PrimitiveType.Real64); break;
                case Mnemonic.cvt_s_d: RewriteCvtD(instr, PrimitiveType.Real32); break;
                case Mnemonic.cvt_w_d: RewriteCvtD(instr, PrimitiveType.Int32); break;
                case Mnemonic.dadd:
                case Mnemonic.daddi:
                    RewriteAdd(instr, PrimitiveType.Word64); break;
                case Mnemonic.daddiu:
                case Mnemonic.daddu: RewriteAdd(instr, PrimitiveType.Word64); break;
                case Mnemonic.ddiv: RewriteDiv(instr, m.SDiv); break;
                case Mnemonic.ddivu: RewriteDiv(instr, m.UDiv); break;
                case Mnemonic.div: RewriteDiv(instr, m.SDiv); break;
                case Mnemonic.divu: RewriteDiv(instr, m.UDiv); break;
                case Mnemonic.div_d: RewriteFpuBinopD(instr, m.FDiv); break;
                case Mnemonic.dmfc0: RewriteMfc0(instr); break;
                case Mnemonic.dmfc1: RewriteMfc1(instr); break;
                case Mnemonic.dmtc0: RewriteMtc0(instr); break;
                case Mnemonic.dmtc1: RewriteMtc1(instr); break;
                case Mnemonic.dmult: RewriteMul(instr, m.SMul, PrimitiveType.Int128); break;
                case Mnemonic.dmultu: RewriteMul(instr, m.UMul, PrimitiveType.UInt128); break;
                case Mnemonic.dsll: RewriteSll(instr); break;
                case Mnemonic.dsll32: RewriteDshift32(instr, m.Shl); break;
                case Mnemonic.dsllv: RewriteSrl(instr); break;
                case Mnemonic.dsra: RewriteSra(instr); break;
                case Mnemonic.dsra32: RewriteDshift32(instr, m.Sar); break;
                case Mnemonic.dsrav: RewriteSra(instr); break;
                case Mnemonic.dsrl: RewriteSrl(instr); break;
                case Mnemonic.dsrl32: RewriteDshift32(instr, m.Shr); break;
                case Mnemonic.dsrlv: RewriteSrl(instr); break;
                case Mnemonic.dsub:
                case Mnemonic.dsubu:
                    RewriteSub(instr, PrimitiveType.Word64); break;
                case Mnemonic.eret: RewriteEret(instr); break;
                case Mnemonic.j: RewriteJump(instr); break;
                case Mnemonic.jal: RewriteJal(instr); break;
                case Mnemonic.jalr: RewriteJalr(instr); break;
                case Mnemonic.jr: RewriteJr(instr); break;
                case Mnemonic.lb: RewriteLoad(instr, PrimitiveType.SByte); break;
                case Mnemonic.lbu: RewriteLoad(instr, PrimitiveType.Byte); break;
                case Mnemonic.ld: RewriteLoad(instr, PrimitiveType.Word64); break;
                case Mnemonic.ldl: RewriteLdl(instr); break;
                case Mnemonic.ldr: RewriteLdr(instr); break;
                case Mnemonic.ldc1: RewriteLcpr1(instr); break;
                case Mnemonic.ldc2: RewriteLdc2(instr); break;
                case Mnemonic.ldxc1: RewriteLcpr1(instr); break;
                case Mnemonic.luxc1: RewriteLcpr1(instr); break;
                case Mnemonic.lwxc1: RewriteLcpr1(instr); break;
                case Mnemonic.lh: RewriteLoad(instr, PrimitiveType.Int16); break;
                case Mnemonic.lhu: RewriteLoad(instr, PrimitiveType.UInt16); break;
                case Mnemonic.ll: RewriteLoadLinked32(instr); break;
                case Mnemonic.lld: RewriteLoadLinked64(instr); break;
                case Mnemonic.lui: RewriteLui(instr); break;
                case Mnemonic.lw: RewriteLoad(instr, PrimitiveType.Word32, PrimitiveType.Int32); break;
                case Mnemonic.lwc1: RewriteLcpr1(instr); break;
                case Mnemonic.lwc2: RewriteLdc2(instr); break;
                case Mnemonic.lwl: RewriteLwl(instr); break;
                case Mnemonic.lwr: RewriteLwr(instr); break;
                case Mnemonic.lwu: RewriteLoad(instr, PrimitiveType.UInt32); break;
                case Mnemonic.madd: RewriteMac_int(instr, m.IAdd); break;
                case Mnemonic.madd_s: RewriteMac_real(instr, PrimitiveType.Real32, m.FAdd); break;
                case Mnemonic.madd_ps: RewriteMac_vec(instr, PrimitiveType.Real32, m.FAdd); break;
                case Mnemonic.mfc0: RewriteMfc0(instr); break;
                case Mnemonic.mfc1: RewriteMfc1(instr); break;
                case Mnemonic.mfhi: RewriteMf(instr, arch.hi); break;
                case Mnemonic.mflo: RewriteMf(instr, arch.lo); break;
                case Mnemonic.mtc0: RewriteMtc0(instr); break;
                case Mnemonic.mthi: RewriteMt(instr, arch.hi); break;
                case Mnemonic.mtlo: RewriteMt(instr, arch.lo); break;
                case Mnemonic.movf: RewriteMovft(instr, false); break;
                case Mnemonic.movn: RewriteMovCc(instr, m.Ne0); break;
                case Mnemonic.movt: RewriteMovft(instr, true); break;
                case Mnemonic.movz: RewriteMovCc(instr, m.Eq0); break;
                case Mnemonic.mov_d: RewriteCopy(instr); break;
                case Mnemonic.mov_s: RewriteCopy(instr); break;
                case Mnemonic.msub: RewriteMac_int(instr, m.ISub); break;
                case Mnemonic.msub_s: RewriteMac_real(instr, PrimitiveType.Real32, m.FSub); break;
                case Mnemonic.mtc1: RewriteMtc1(instr); break;
                case Mnemonic.mul: RewriteMul(instr, m.SMul, PrimitiveType.Int32); break;
                case Mnemonic.mult: RewriteMul(instr, m.SMul, PrimitiveType.Int64); break;
                case Mnemonic.multu: RewriteMul(instr, m.UMul, PrimitiveType.UInt64); break;
                case Mnemonic.mul_s: RewriteMul(instr, m.FMul, PrimitiveType.Real32); break;
                case Mnemonic.mul_d: RewriteMulD(instr); break;
                case Mnemonic.nmadd_d: RewriteNmac_real(instr, PrimitiveType.Real64, m.FAdd); break;
                case Mnemonic.nmadd_s: RewriteNmac_real(instr, PrimitiveType.Real32, m.FAdd); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.nor: RewriteNor(instr); break;
                case Mnemonic.nmsub_d: RewriteNmac_real(instr, PrimitiveType.Real64, m.FSub); break;
                case Mnemonic.nmsub_s: RewriteNmac_real(instr, PrimitiveType.Real32, m.FSub); break;
                case Mnemonic.nmsub_ps: RewriteNmac_vec(instr, PrimitiveType.Real32, m.FSub); break;
                case Mnemonic.or:
                case Mnemonic.ori: RewriteOr(instr); break;
                case Mnemonic.pref:
                case Mnemonic.prefx: RewritePrefx(instr); break;
                case Mnemonic.sb: RewriteStore(instr); break;
                case Mnemonic.sc: RewriteStoreConditional32(instr); break;
                case Mnemonic.scd: RewriteStoreConditional64(instr); break;
                case Mnemonic.sd: RewriteStore(instr); break;
                case Mnemonic.sdc1: RewriteStore(instr); break;
                case Mnemonic.sdc2: RewriteSdc2(instr); break;
                case Mnemonic.sdl: RewriteSdl(instr); break;
                case Mnemonic.sdr: RewriteSdr(instr); break;
                case Mnemonic.seb: RewriteSignExtend(instr, PrimitiveType.Byte); break;
                case Mnemonic.seh: RewriteSignExtend(instr, PrimitiveType.Word16); break;
                case Mnemonic.sh: RewriteStore(instr); break;
                case Mnemonic.sll:
                case Mnemonic.sllv:
                    RewriteSll(instr); break;
                case Mnemonic.slt: RewriteSxx(instr, m.Lt); break;
                case Mnemonic.slti: RewriteSxx(instr, m.Lt); break;
                case Mnemonic.sltiu: RewriteSxx(instr, m.Ult); break;
                case Mnemonic.sltu: RewriteSxx(instr, m.Ult); break;
                case Mnemonic.sra:
                case Mnemonic.srav:
                    RewriteSra(instr); break;
                case Mnemonic.srl:
                case Mnemonic.srlv:
                    RewriteSrl(instr); break;
                case Mnemonic.sub:
                case Mnemonic.subu:
                    RewriteSub(instr, PrimitiveType.Word32); break;
                case Mnemonic.sub_d: RewriteFpuBinopD(instr, m.FSub); break;
                case Mnemonic.sw:
                case Mnemonic.swc1: RewriteStore(instr); break;
                case Mnemonic.swc2: RewriteSdc2(instr); break;
                case Mnemonic.swl: RewriteSwl(instr); break;
                case Mnemonic.swr: RewriteSwr(instr); break;
                case Mnemonic.swxc1: RewriteStore(instr); break;
                case Mnemonic.sync: RewriteSync(instr); break;
                case Mnemonic.syscall: RewriteSyscall(instr); break;
                case Mnemonic.teq: RewriteTrap(instr, m.Eq); break;
                case Mnemonic.teqi: RewriteTrap(instr, m.Eq); break;
                case Mnemonic.tge: RewriteTrap(instr, m.Ge); break;
                case Mnemonic.tgeu: RewriteTrap(instr, m.Uge); break;
                case Mnemonic.tgei: RewriteTrapi(instr, m.Ge); break;
                case Mnemonic.tgeiu: RewriteTrapi(instr, m.Uge); break;
                case Mnemonic.tlbp: RewriteTlbp(instr); break;
                case Mnemonic.tlbr: RewriteTlbr(instr); break;
                case Mnemonic.tlbwi: RewriteTlbwi(instr); break;
                case Mnemonic.tlbwr: RewriteTlbwr(instr); break;
                case Mnemonic.tlt: RewriteTrap(instr, m.Lt); break;
                case Mnemonic.tlti: RewriteTrapi(instr, m.Lt); break;
                case Mnemonic.tltiu: RewriteTrapi(instr, m.Ult); break;
                case Mnemonic.tltu: RewriteTrap(instr, m.Ult); break;
                case Mnemonic.tne: RewriteTrap(instr, m.Ne); break;
                case Mnemonic.tnei: RewriteTrapi(instr, m.Ne); break;
                case Mnemonic.trunc_l_d: RewriteTrunc(instr, "trunc", PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Mnemonic.wait: RewriteWait(instr); break;
                case Mnemonic.xor:
                case Mnemonic.xori: RewriteXor(instr); break;
                case Mnemonic.rdhwr: RewriteReadHardwareRegister(instr); break;

                // Nano instructions
                case Mnemonic.addiupc: RewriteAddiupc(instr); break;
                case Mnemonic.aluipc: RewriteAluipc(instr); break;
                case Mnemonic.balc: RewriteJump(instr); break;
                case Mnemonic.bbeqzc: RewriteBb(instr, e => e); break;
                case Mnemonic.bbnezc: RewriteBb(instr, m.Not); break;
                case Mnemonic.bc: RewriteJump(instr); break;
                case Mnemonic.beqc: RewriteBranch(instr, m.Eq, false); break;
                case Mnemonic.beqic: RewriteBranchImm(instr, m.Eq, false); break;
                case Mnemonic.beqzc: RewriteBranch0(instr, m.Eq, false); break;
                case Mnemonic.bgec: RewriteBranch(instr, m.Ge, false); break;
                case Mnemonic.bgeic: RewriteBranchImm(instr, m.Ge, false); break;
                case Mnemonic.bgeiuc: RewriteBranchImm(instr, m.Uge, false); break;
                case Mnemonic.bltc: RewriteBranch(instr, m.Lt, false); break;
                case Mnemonic.bltic: RewriteBranchImm(instr, m.Lt, false); break;
                case Mnemonic.bltiuc: RewriteBranchImm(instr, m.Ult, false); break;
                case Mnemonic.bltuc: RewriteBranch(instr, m.Ult, false); break;
                case Mnemonic.bnec: RewriteBranch(instr, m.Ne, false); break;
                case Mnemonic.bneiuc: RewriteBranchImm(instr, m.Ne, false); break;
                case Mnemonic.bnezc: RewriteBranch0(instr, m.Ne, false); break;
                case Mnemonic.ext: RewriteExt(instr); break;
                case Mnemonic.ins: RewriteIns(instr); break;
                case Mnemonic.jalrc: RewriteJalr(instr); break;
                case Mnemonic.jrc: RewriteJr(instr); break;
                case Mnemonic.lbux: RewriteLx(instr, PrimitiveType.Byte); break;
                case Mnemonic.lwx: RewriteLx(instr, PrimitiveType.Word32); break;
                case Mnemonic.li: RewriteMove(instr); break;
                case Mnemonic.lsa: RewriteLsa(instr); break;
                case Mnemonic.lwm: RewriteLwm(instr); break;
                case Mnemonic.lwxs: RewriteLwxs(instr); break;
                case Mnemonic.move: RewriteMove(instr); break;
                case Mnemonic.move_balc: RewriteMoveBalc(instr); break;
                case Mnemonic.movep: RewriteMovep(instr); break;
                case Mnemonic.not: RewriteNot(instr); break;
                case Mnemonic.restore: RewriteRestore(instr, false); break;
                case Mnemonic.restore_jrc: RewriteRestore(instr, true); break;
                case Mnemonic.save: RewriteSave(instr); break;
                case Mnemonic.sigrie: RewriteSigrie(instr); break;
                case Mnemonic.swxs: RewriteSwxs(instr); break;
                case Mnemonic.ualwm: RewriteLwm(instr); break;
                }
                yield return new RtlInstructionCluster(
                    instr.Address,
                    instr.Length,
                    rtlInstructions.ToArray())
                {
                    Class = iclass
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if DEBUG
        private static readonly HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        protected void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            int cbInstr = dasm.Current.Length;
            r2.Offset -= cbInstr;
            var uInstr = cbInstr == 2 ? r2.ReadUInt16() : r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void MipsRw_{0}()", dasm.Current.Mnemonic);
            Debug.WriteLine("        {");
            Debug.WriteLine("            AssertCode(0x{0:X8},   // {1}", uInstr, dasm.Current);
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", cbInstr);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        private void EmitUnitTest() { }
#endif


        private Expression RewriteOperand(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                return binder.EnsureRegister(regOp.Register);
            case ImmediateOperand immOp:
                return immOp.Value;
            case IndirectOperand indOp:
                Expression ea;
                Identifier baseReg = binder.EnsureRegister(indOp.Base);
                if (indOp.Offset == 0)
                    ea = baseReg;
                else if (indOp.Offset > 0)
                    ea = m.IAdd(baseReg, indOp.Offset);
                else
                    ea = m.ISub(baseReg, -indOp.Offset);
                return m.Mem(indOp.Width, ea);
            case AddressOperand addrOp:
                return addrOp.Address;
            case IndexedOperand idxOp:
                if (idxOp.Base.Number == 0)
                {
                    if (idxOp.Index.Number == 0)
                    {
                        //$REVIEW: is this even valid?
                        ea = Constant.Zero(
                            PrimitiveType.CreateWord(idxOp.Base.DataType.BitSize));
                    }
                    else
                    {
                        ea = binder.EnsureRegister(idxOp.Index);
                    }
                }
                else
                {
                    ea = binder.EnsureRegister(idxOp.Base);
                    if (idxOp.Index.Number != 0)
                    {
                        ea = m.IAdd(ea, binder.EnsureRegister(idxOp.Index));
                    }
                }
                return m.Mem(idxOp.Width, ea);
            }
            throw new NotImplementedException(string.Format("Rewriting of operand type {0} not implemented yet.", op.GetType().Name));
        }

        private Expression RewriteOperand0(MachineOperand op)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                if (regOp.Register.Number == 0)
                    return Constant.Zero(regOp.Register.DataType);
                return binder.EnsureRegister(regOp.Register);
            case ImmediateOperand immOp:
                return immOp.Value;
            case AddressOperand addrOp:
                return addrOp.Address;

            case IndirectOperand indOp:
                Expression ea;
                Identifier baseReg = binder.EnsureRegister(indOp.Base);
                if (indOp.Offset == 0)
                    ea = baseReg;
                else if (indOp.Offset > 0)
                    ea = m.IAdd(baseReg, indOp.Offset);
                else
                    ea = m.ISub(baseReg, -indOp.Offset);
                return m.Mem(indOp.Width, ea);
            case IndexedOperand idxOp:
                if (idxOp.Base.Number == 0)
                {
                    if (idxOp.Index.Number == 0)
                    {
                        //$REVIEW: is this even valid?
                        ea = Constant.Zero(
                            PrimitiveType.CreateWord(idxOp.Base.DataType.BitSize));
                    }
                    else
                    {
                        ea = binder.EnsureRegister(idxOp.Index);
                    }
                }
                else
                {
                    ea = binder.EnsureRegister(idxOp.Base);
                    if (idxOp.Index.Number != 0)
                    {
                        ea = m.IAdd(ea, binder.EnsureRegister(idxOp.Index));
                    }
                }
                return m.Mem(idxOp.Width, ea);
            }
            throw new NotImplementedException(string.Format("Rewriting of operand type {0} not implemented yet.", op.GetType().Name));
        }

    }
}
