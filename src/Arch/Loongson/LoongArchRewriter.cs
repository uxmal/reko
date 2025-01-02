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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Loongson
{
    public partial class LoongArchRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly LoongArch arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<LoongArchInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private LoongArchInstruction instr;
        private InstrClass iclass;

        public LoongArchRewriter(LoongArch arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new LoongArchDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid: m.Invalid(); break;
                case Mnemonic.add_d: RewriteBinaryOp(m.IAdd, false); break;
                case Mnemonic.add_w: RewriteBinaryOp(m.IAdd, true); break;
                case Mnemonic.addi_d: RewriteBinaryOp(m.IAdd, false); break;
                case Mnemonic.addi_w: RewriteBinaryOp(m.IAdd, true); break;
                case Mnemonic.alsl_d: RewriteAlsl(false); break;
                case Mnemonic.alsl_w: RewriteAlsl(true); break;
                case Mnemonic.alsl_wu: RewriteAlslU(true); break;
                case Mnemonic.amadd_d: RewriteAtomic(atomic_add, false); break;
                case Mnemonic.amadd_db_d: RewriteAtomic(atomic_add_db, false); break;
                case Mnemonic.amadd_db_w: RewriteAtomic(atomic_add_db, true); break;
                case Mnemonic.amadd_w: RewriteAtomic(atomic_add, true); break;
                case Mnemonic.amand_d: RewriteAtomic(atomic_and, false); break;
                case Mnemonic.amand_db_d: RewriteAtomic(atomic_and_db, false); break;
                case Mnemonic.amand_db_w: RewriteAtomic(atomic_and_db, true); break;
                case Mnemonic.amand_w: RewriteAtomic(atomic_and, true); break;
                case Mnemonic.ammax_d: RewriteAtomic(atomic_max, false); break;
                case Mnemonic.ammax_du: RewriteAtomic(atomic_maxu, false); break;
                case Mnemonic.ammax_db_d: RewriteAtomic(atomic_max_db, false); break;
                case Mnemonic.ammax_db_du: RewriteAtomic(atomic_maxu_db, false); break;
                case Mnemonic.ammax_db_w: RewriteAtomic(atomic_max_db, true); break;
                case Mnemonic.ammax_db_wu: RewriteAtomic(atomic_maxu_db, true); break;
                case Mnemonic.ammax_w: RewriteAtomic(atomic_max, true); break;
                case Mnemonic.ammax_wu: RewriteAtomic(atomic_maxu, true); break;
                case Mnemonic.ammin_d: RewriteAtomic(atomic_min, false); break;
                case Mnemonic.ammin_du: RewriteAtomic(atomic_minu, false); break;
                case Mnemonic.ammin_db_d: RewriteAtomic(atomic_min_db, false); break;
                case Mnemonic.ammin_db_du: RewriteAtomic(atomic_minu_db, false); break;
                case Mnemonic.ammin_db_w: RewriteAtomic(atomic_minu_db, true); break;
                case Mnemonic.ammin_db_wu: RewriteAtomic(atomic_minu_db, true); break;
                case Mnemonic.ammin_w: RewriteAtomic(atomic_min, true); break;
                case Mnemonic.ammin_wu: RewriteAtomic(atomic_minu, true); break;
                case Mnemonic.amor_d: RewriteAtomic(atomic_or, false); break;
                case Mnemonic.amor_db_d: RewriteAtomic(atomic_or_db, false); break;
                case Mnemonic.amor_db_w: RewriteAtomic(atomic_or_db, true); break;
                case Mnemonic.amor_w: RewriteAtomic(atomic_or, true); break;
                case Mnemonic.amswap_d: RewriteAtomic(atomic_swap, false); break;
                case Mnemonic.amswap_w: RewriteAtomic(atomic_swap, true); break;
                case Mnemonic.amswap_db_d: RewriteAtomic(atomic_swap_db, false); break;
                case Mnemonic.amswap_db_w: RewriteAtomic(atomic_swap_db, true); break;
                case Mnemonic.amxor_d: RewriteAtomic(atomic_xor, false); break;
                case Mnemonic.amxor_db_d: RewriteAtomic(atomic_xor_db, false); break;
                case Mnemonic.amxor_db_w: RewriteAtomic(atomic_xor_db, true); break;
                case Mnemonic.amxor_w: RewriteAtomic(atomic_xor, true); break;
                case Mnemonic.and: RewriteBinaryOp(m.And, false); break;
                case Mnemonic.andi: RewriteBinaryOp(m.And, false); break;
                case Mnemonic.andn: RewriteBinaryOp(Andn, false); break;
                case Mnemonic.asrtgt_d: RewriteAsr(m.Gt); break;
                case Mnemonic.asrtle_d: RewriteAsr(m.Le); break;
                case Mnemonic.b: RewriteB(); break;
                case Mnemonic.beq: RewriteBranch(m.Eq); break;
                case Mnemonic.beqz: RewriteBranch0(m.Eq0); break;
                case Mnemonic.bge: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu: RewriteBranch(m.Uge); break;
                case Mnemonic.bitrev_d: RewriteUnaryOp(e => m.Fn(bitrev.MakeInstance(PrimitiveType.Word64), e), false); break;
                case Mnemonic.bitrev_w: RewriteUnaryOp(e => m.Fn(bitrev.MakeInstance(PrimitiveType.Word32), e), true); break;
                case Mnemonic.bl: RewriteBl(); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu: RewriteBranch(m.Ult); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.bnez: RewriteBranch0(m.Ne0); break;
                case Mnemonic.@break: RewriteBreak(); break;
                case Mnemonic.bstrpick_d: RewriteBsrpick(false); break;
                case Mnemonic.bstrpick_w: RewriteBsrpick(true); break;
                case Mnemonic.bytepick_d: RewriteBytepick(false); break;
                case Mnemonic.bytepick_w: RewriteBytepick(true); break;
                case Mnemonic.cacop: RewriteCacop(); break;
                case Mnemonic.clo_d: RewriteUnaryOp(e => m.Fn(CommonOps.CountLeadingOnes, e), false); break;
                case Mnemonic.clo_w: RewriteUnaryOp(e => m.Fn(CommonOps.CountLeadingOnes, e), true); break;
                case Mnemonic.clz_d: RewriteUnaryOp(e => m.Fn(CommonOps.CountLeadingZeros, e), false); break;
                case Mnemonic.clz_w: RewriteUnaryOp(e => m.Fn(CommonOps.CountLeadingZeros, e), true); break;
                case Mnemonic.csrrd: RewriteCsrrd(); break;
                case Mnemonic.csrwr: RewriteCsrwr(); break;
                case Mnemonic.div_d: RewriteBinaryOp(m.SDiv, false); break;
                case Mnemonic.div_du: RewriteBinaryOp(m.UDiv, false); break;
                case Mnemonic.div_w: RewriteBinaryOp(m.SDiv, true); break;
                case Mnemonic.div_wu: RewriteBinaryOp(m.UDiv, true); break;
                case Mnemonic.ext_w_b: RewriteExt(PrimitiveType.Int32, PrimitiveType.SByte); break;
                case Mnemonic.ext_w_h: RewriteExt(PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.fadd_d: RewriteBinaryFpuOp(m.FAdd, false); break;
                case Mnemonic.fadd_s: RewriteBinaryFpuOp(m.FAdd, true); break;
                case Mnemonic.fabs_d: RewriteUnaryFpuOp(e => m.Fn(FpOps.fabs, e), false); break;
                case Mnemonic.fabs_s: RewriteUnaryFpuOp(e => m.Fn(FpOps.fabsf, e), true); break;
                case Mnemonic.fcopysign_d: RewriteFCopySign(false); break;
                case Mnemonic.fcopysign_s: RewriteFCopySign(true); break;
                case Mnemonic.fdiv_d: RewriteBinaryFpuOp(m.FDiv, false); break;
                case Mnemonic.fdiv_s: RewriteBinaryFpuOp(m.FDiv, true); break;
                case Mnemonic.fld_d: RewriteLoad(PrimitiveType.Real64); break;
                case Mnemonic.fld_s: RewriteFLoad32(PrimitiveType.Real32); break;
                case Mnemonic.fldgt_d: RewriteFLoadBoundaryCheck(PrimitiveType.Real64, m.Ugt); break;
                case Mnemonic.fldgt_s: RewriteFLoadBoundaryCheck(PrimitiveType.Real32, m.Ugt); break;
                case Mnemonic.fldle_d: RewriteFLoadBoundaryCheck(PrimitiveType.Real64, m.Ule); break;
                case Mnemonic.fldle_s: RewriteFLoadBoundaryCheck(PrimitiveType.Real32, m.Ule); break;
                case Mnemonic.fldx_d: RewriteLoad(PrimitiveType.Real64); break;
                case Mnemonic.fldx_s: RewriteFLoad32(PrimitiveType.Real32); break;
                case Mnemonic.fmax_d: RewriteBinaryFpuOp((a, b) => m.Fn(FpOps.FMax64, a, b), false); break;
                case Mnemonic.fmax_s: RewriteBinaryFpuOp((a, b) => m.Fn(FpOps.FMax32, a, b), true); break;
                case Mnemonic.fmaxa_d: RewriteBinaryFpuOp((a, b) => m.Fn(fmaxa.MakeInstance(PrimitiveType.Real64), a, b), false); break;
                case Mnemonic.fmaxa_s: RewriteBinaryFpuOp((a, b) => m.Fn(fmaxa.MakeInstance(PrimitiveType.Real32), a, b), true); break;
                case Mnemonic.fmin_d: RewriteBinaryFpuOp((a, b) => m.Fn(FpOps.FMin64, a, b), false); break;
                case Mnemonic.fmin_s: RewriteBinaryFpuOp((a, b) => m.Fn(FpOps.FMin32, a, b), true); break;
                case Mnemonic.fmina_d: RewriteBinaryFpuOp((a, b) => m.Fn(fmina.MakeInstance(PrimitiveType.Real64), a, b), false); break;
                case Mnemonic.fmina_s: RewriteBinaryFpuOp((a, b) => m.Fn(fmina.MakeInstance(PrimitiveType.Real32), a, b), true); break;
                case Mnemonic.fmov_d: RewriteUnaryFpuOp(e => e, false); break;
                case Mnemonic.fmov_s: RewriteUnaryFpuOp(e => e, true); break;
                case Mnemonic.fmul_d: RewriteBinaryFpuOp(m.FMul, false); break;
                case Mnemonic.fmul_s: RewriteBinaryFpuOp(m.FMul, true); break;
                case Mnemonic.fscaleb_d: RewriteBinaryFpuOp((a, b) => m.Fn(fscaleb.MakeInstance(PrimitiveType.Real64), a, b), false); break;
                case Mnemonic.fscaleb_s: RewriteBinaryFpuOp((a, b) => m.Fn(fscaleb.MakeInstance(PrimitiveType.Real32), a, b), true); break;
                case Mnemonic.fsub_d: RewriteBinaryFpuOp(m.FSub, false); break;
                case Mnemonic.fsub_s: RewriteBinaryFpuOp(m.FSub, true); break;
                case Mnemonic.fst_d: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.fst_s: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.fstgt_d: RewriteStoreBoundsCheck(PrimitiveType.Real64, m.Ugt); break;
                case Mnemonic.fstgt_s: RewriteStoreBoundsCheck(PrimitiveType.Real32, m.Ugt); break;
                case Mnemonic.fstle_d: RewriteStoreBoundsCheck(PrimitiveType.Real64, m.Ule); break;
                case Mnemonic.fstle_s: RewriteStoreBoundsCheck(PrimitiveType.Real32, m.Ule); break;
                case Mnemonic.fstx_d: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.fstx_s: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.jirl: RewriteJirl(); break;
                case Mnemonic.ld_b: RewriteLoad(PrimitiveType.SByte); break;
                case Mnemonic.ld_bu: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.ld_d: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.ld_h: RewriteLoad(PrimitiveType.Int16); break;
                case Mnemonic.ld_hu: RewriteLoad(PrimitiveType.UInt16); break;
                case Mnemonic.ld_w: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ld_wu: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.ldgt_b: RewriteLoadBoundsCheck(PrimitiveType.SByte, m.Ugt); break;
                case Mnemonic.ldgt_h: RewriteLoadBoundsCheck(PrimitiveType.Int16, m.Ugt); break;
                case Mnemonic.ldgt_d: RewriteLoadBoundsCheck(PrimitiveType.Int64, m.Ugt); break;
                case Mnemonic.ldgt_w: RewriteLoadBoundsCheck(PrimitiveType.Int32, m.Ugt); break;
                case Mnemonic.ldle_b: RewriteLoadBoundsCheck(PrimitiveType.SByte, m.Ule); break;
                case Mnemonic.ldle_h: RewriteLoadBoundsCheck(PrimitiveType.Int16, m.Ule); break;
                case Mnemonic.ldle_d: RewriteLoadBoundsCheck(PrimitiveType.Int64, m.Ule); break;
                case Mnemonic.ldle_w: RewriteLoadBoundsCheck(PrimitiveType.Int32, m.Ule); break;
                case Mnemonic.ldpte: RewriteLdpte(); break;
                case Mnemonic.ldptr_d: RewriteLdptr(PrimitiveType.Word64); break;
                case Mnemonic.ldptr_w: RewriteLdptr(PrimitiveType.Word32); break;
                case Mnemonic.lu12i_w: RewriteLu12i(); break;
                case Mnemonic.lu32i_d: RewriteLu32i(); break;
                case Mnemonic.lu52i_d: RewriteLu52i(); break;
                case Mnemonic.ldx_b: RewriteLoad(PrimitiveType.SByte); break;
                case Mnemonic.ldx_bu: RewriteLoad(PrimitiveType.Byte); break;
                case Mnemonic.ldx_d: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.ldx_h: RewriteLoad(PrimitiveType.Int16); break;
                case Mnemonic.ldx_hu: RewriteLoad(PrimitiveType.UInt16); break;
                case Mnemonic.ldx_w: RewriteLoad(PrimitiveType.Int32); break;
                case Mnemonic.ldx_wu: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.maskeqz: RewriteMask(m.Eq0); break;
                case Mnemonic.masknez: RewriteMask(m.Ne0); break;
                case Mnemonic.mod_d: RewriteBinaryOp(m.SMod, false); break;
                case Mnemonic.mod_du: RewriteBinaryOp(m.UMod, false); break;
                case Mnemonic.mod_w: RewriteBinaryOp(m.SMod, true); break;
                case Mnemonic.mod_wu: RewriteBinaryOp(m.UMod, true); break;
                case Mnemonic.mul_d: RewriteMul(false); break;
                case Mnemonic.mul_w: RewriteMul(true); break;
                case Mnemonic.mulh_d: RewriteMulh_d(); break;
                case Mnemonic.mulh_du: RewriteMulh_du(); break;
                case Mnemonic.mulh_w: RewriteMulh_w(); break;
                case Mnemonic.mulh_wu: RewriteMulh_wu(); break;
                case Mnemonic.mulw_d_w: RewriteMulw(PrimitiveType.Int64, PrimitiveType.Int32); break;
                case Mnemonic.mulw_d_wu: RewriteMulwu(PrimitiveType.UInt64, PrimitiveType.UInt32); break;
                case Mnemonic.nor: RewriteNor(); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.ori: RewriteOri(); break;
                case Mnemonic.orn: RewriteBinaryOp(Orn, false); break;
                case Mnemonic.revb_d: RewriteUnaryOp(a => m.Fn(rev_b, a), false); break;
                case Mnemonic.revh_d: RewriteUnaryOp(a => m.Fn(rev_h, a), false); break;
                case Mnemonic.rotr_d: RewriteRotr(false); break;
                case Mnemonic.rotr_w: RewriteRotr(true); break;
                case Mnemonic.rotri_d: RewriteRotr(false); break;
                case Mnemonic.rotri_w: RewriteRotr(true); break;
                case Mnemonic.sll_d: RewriteShift(m.Shl, false); break;
                case Mnemonic.sll_w: RewriteShift(m.Shl, true); break;
                case Mnemonic.slli_d: RewriteShift(m.Shl, false); break;
                case Mnemonic.slli_w: RewriteShift(m.Shl, true); break;
                case Mnemonic.slt: RewriteSet(m.Lt); break;
                case Mnemonic.slti: RewriteSet(m.Lt); break;
                case Mnemonic.sltu: RewriteSet(m.Ult); break;
                case Mnemonic.sltui: RewriteSet(m.Ult); break;
                case Mnemonic.sra_d: RewriteShift(m.Sar, false); break;
                case Mnemonic.sra_w: RewriteShift(m.Sar, true); break;
                case Mnemonic.srai_d: RewriteShift(m.Sar, false); break;
                case Mnemonic.srai_w: RewriteShift(m.Sar, true); break;
                case Mnemonic.srl_d: RewriteShift(m.Shr, false); break;
                case Mnemonic.srl_w: RewriteShift(m.Shr, true); break;
                case Mnemonic.srli_d: RewriteShift(m.Shr, false); break;
                case Mnemonic.srli_w: RewriteShift(m.Shr, true); break;
                case Mnemonic.st_b: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.st_d: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.st_h: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.st_w: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stgt_b: RewriteStoreBoundsCheck(PrimitiveType.Byte, m.Ugt); break;
                case Mnemonic.stgt_d: RewriteStoreBoundsCheck(PrimitiveType.Word64, m.Ugt); break;
                case Mnemonic.stgt_h: RewriteStoreBoundsCheck(PrimitiveType.Word16, m.Ugt); break;
                case Mnemonic.stgt_w: RewriteStoreBoundsCheck(PrimitiveType.Word32, m.Ugt); break;
                case Mnemonic.stle_b: RewriteStoreBoundsCheck(PrimitiveType.Byte, m.Ule); break;
                case Mnemonic.stle_d: RewriteStoreBoundsCheck(PrimitiveType.Word64, m.Ule); break;
                case Mnemonic.stle_h: RewriteStoreBoundsCheck(PrimitiveType.Word16, m.Ule); break;
                case Mnemonic.stle_w: RewriteStoreBoundsCheck(PrimitiveType.Word32, m.Ule); break;
                case Mnemonic.stptr_d: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.stptr_w: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.stx_b: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.stx_d: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.stx_h: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.stx_w: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sub_d: RewriteBinaryOp(m.ISub, false); break;
                case Mnemonic.sub_w: RewriteBinaryOp(m.ISub, true); break;
                case Mnemonic.syscall: RewriteSyscall(); break;
                case Mnemonic.tlbwr: RewriteTlbwr(); break;
                case Mnemonic.xor: RewriteXor(); break;
                case Mnemonic.xori: RewriteXor(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Expression Andn(Expression a, Expression b) => m.And(a, m.Comp(b));

        private Expression Orn(Expression a, Expression b) => m.Or(a, m.Comp(b));


        private void Assign(MachineOperand reg, long value)
        {
            var dst = binder.EnsureRegister((RegisterStorage) reg);
            var src = Constant.Create(dst.DataType, value);
            m.Assign(dst, src);
        }

        private void Assign(MachineOperand reg, Expression src)
        {
            var dst = binder.EnsureRegister((RegisterStorage) reg);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.Convert(tmp, tmp.DataType, PrimitiveType.Int64);
            }
            m.Assign(dst, src);
        }

        private void AssignU(MachineOperand reg, Expression src)
        {
            var dst = binder.EnsureRegister((RegisterStorage) reg);
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                src = m.Convert(tmp, tmp.DataType, PrimitiveType.Word64);
            }
            m.Assign(dst, src);
        }

        private Expression EffectiveAddress()
        {
            var baseReg = Op(1, false);
            if (instr.Operands[2] is ImmediateOperand imm)
            {
                var offset = imm.Value.ToInt32();
                return m.AddSubSignedInt(baseReg, offset);
            }
            else
            {
                var idxReg = Op(2, false);
                return m.IAdd(baseReg, idxReg);
            }
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("LoongArchRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void FpuAssign(int iop, Expression src)
        {
            var dt = src.DataType;
            var dst = binder.EnsureRegister((RegisterStorage) instr.Operands[iop]);
            if (dst.DataType.BitSize > src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, src);
                src = m.Dpb(dst, tmp, 0);
            }
            m.Assign(dst, src);
        }

        private Expression Op(int iop, bool sliceTo32bits)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                if (reg.Number == 0)
                {
                    return sliceTo32bits
                        ? Constant.Zero(PrimitiveType.Word32)
                        : Constant.Zero(reg.DataType);
                }
                var id = binder.EnsureRegister(reg);
                if (!sliceTo32bits || reg.BitSize == 32)
                    return id;
                var tmp = binder.CreateTemporary(PrimitiveType.Word32);
                m.Assign(tmp, m.Slice(id, tmp.DataType, 0));
                return tmp;
            case ImmediateOperand imm:
                var c = imm.Value;
                if (sliceTo32bits)
                    return c;
                c = Constant.Create(PrimitiveType.Word64, (long) c.ToInt32());
                return c;
            default:
                throw new NotImplementedException($"Operand type {op.GetType().Name} not implemented yet.");
            }
        }

        private static IntrinsicProcedure AtomicIntrinsic(string name)
        {
            return new IntrinsicBuilder(name, true)
                        .GenericTypes("T")
                        .PtrParam("T")
                        .Param("T")
                        .Returns("T");
        }

        private static readonly IntrinsicProcedure atomic_add = AtomicIntrinsic("__atomic_add");
        private static readonly IntrinsicProcedure atomic_add_db = AtomicIntrinsic("__atomic_add_db");
        private static readonly IntrinsicProcedure atomic_and = AtomicIntrinsic("__atomic_and");
        private static readonly IntrinsicProcedure atomic_and_db = AtomicIntrinsic("__atomic_and_db");
        private static readonly IntrinsicProcedure atomic_max = AtomicIntrinsic("__atomic_max");
        private static readonly IntrinsicProcedure atomic_maxu = AtomicIntrinsic("__atomic_maxu");
        private static readonly IntrinsicProcedure atomic_max_db = AtomicIntrinsic("__atomic_max_db");
        private static readonly IntrinsicProcedure atomic_maxu_db = AtomicIntrinsic("__atomic_maxu_db");
        private static readonly IntrinsicProcedure atomic_min = AtomicIntrinsic("__atomic_min");
        private static readonly IntrinsicProcedure atomic_minu = AtomicIntrinsic("__atomic_minu");
        private static readonly IntrinsicProcedure atomic_min_db = AtomicIntrinsic("__atomic_min_db");
        private static readonly IntrinsicProcedure atomic_minu_db = AtomicIntrinsic("__atomic_minu_db");
        private static readonly IntrinsicProcedure atomic_or = AtomicIntrinsic("__atomic_or");
        private static readonly IntrinsicProcedure atomic_or_db = AtomicIntrinsic("__atomic_or_db");
        private static readonly IntrinsicProcedure atomic_swap = AtomicIntrinsic("__atomic_swap");
        private static readonly IntrinsicProcedure atomic_swap_db = AtomicIntrinsic("__atomic_swap_db");
        private static readonly IntrinsicProcedure atomic_xor = AtomicIntrinsic("__atomic_xor");
        private static readonly IntrinsicProcedure atomic_xor_db = AtomicIntrinsic("__atomic_xor_db");
        private static readonly IntrinsicProcedure bitrev = IntrinsicBuilder.GenericUnary("__bitrev");
        private static readonly IntrinsicProcedure break_intrinsic = new IntrinsicBuilder("__break", true)
            .Param(PrimitiveType.UInt32)
            .Void();
        private static readonly IntrinsicProcedure bstrpick = IntrinsicBuilder.GenericTernary("__bstrpick");
        private static readonly IntrinsicProcedure bytepick = IntrinsicBuilder.GenericTernary("__bytepick");
        private static readonly IntrinsicProcedure cacop = new IntrinsicBuilder("__cacop", true)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure csrrd = new IntrinsicBuilder("__csrrd", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word32)
            .Returns("T");
        private static readonly IntrinsicProcedure csrwr = new IntrinsicBuilder("__csrwr", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word32)
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure fcopysign = IntrinsicBuilder.GenericBinary("__fcopysign");
        private static readonly IntrinsicProcedure fmina = IntrinsicBuilder.GenericBinary("__fmina");
        private static readonly IntrinsicProcedure fmaxa = IntrinsicBuilder.GenericBinary("__fmaxa");
        private static readonly IntrinsicProcedure fscaleb = IntrinsicBuilder.GenericBinary("__fscaleb");
        private static readonly IntrinsicProcedure ldpte = new IntrinsicBuilder("__ldpte", true)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.UInt32)
            .Returns("T");
        private static readonly IntrinsicProcedure raise_exception = new IntrinsicBuilder("__raise_exception", true, new()
        {
            Terminates = true,
        }).Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure rev_b = IntrinsicBuilder.GenericUnary("__rev_b");
        private static readonly IntrinsicProcedure rev_h = IntrinsicBuilder.GenericUnary("__rev_h");
        private static readonly IntrinsicProcedure tlbwr = new IntrinsicBuilder("__tlbwr", true)
            .Void();
    }
}