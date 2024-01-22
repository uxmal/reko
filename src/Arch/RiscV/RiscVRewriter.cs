#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.RiscV
{
    using Decoder = Decoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;

    public partial class RiscVRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly RiscVArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<RiscVInstruction> dasm;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly ProcessorState state;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly RtlEmitter m;
        private RiscVInstruction instr;
        private InstrClass iclass;

        public RiscVRewriter(RiscVArchitecture arch, Decoder[] decoders, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.dasm = new RiscVDisassembler(arch, decoders, rdr).GetEnumerator();
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                this.iclass = this.instr.InstructionClass;

                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        addr, 
                        "Risc-V instruction '{0}' not supported yet.",
                        instr.Mnemonic);
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.invalid: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addi: RewriteAdd(); break;
                case Mnemonic.addiw: RewriteAddw(); break;
                case Mnemonic.addw: RewriteAddw(); break;
                case Mnemonic.amoadd_d: RewriteAtomicMemoryOperation(amoadd_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.amoadd_w: RewriteAtomicMemoryOperation(amoadd_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.amoand_d: RewriteAtomicMemoryOperation(amoand_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.amoand_w: RewriteAtomicMemoryOperation(amoand_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.amomax_d: RewriteAtomicMemoryOperation(amomax_intrinsic, PrimitiveType.Int64); break;
                case Mnemonic.amomax_w: RewriteAtomicMemoryOperation(amomax_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.amomaxu_d: RewriteAtomicMemoryOperation(amomax_intrinsic, PrimitiveType.UInt64); break;
                case Mnemonic.amomaxu_w: RewriteAtomicMemoryOperation(amomax_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.amomin_d: RewriteAtomicMemoryOperation(amomin_intrinsic, PrimitiveType.Int64); break;
                case Mnemonic.amomin_w: RewriteAtomicMemoryOperation(amomin_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.amominu_d: RewriteAtomicMemoryOperation(amomin_intrinsic, PrimitiveType.UInt64); break;
                case Mnemonic.amominu_w: RewriteAtomicMemoryOperation(amomin_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.amoor_d: RewriteAtomicMemoryOperation(amoor_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.amoor_w: RewriteAtomicMemoryOperation(amoor_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.amoswap_d: RewriteAtomicMemoryOperation(amoswap_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.amoswap_w: RewriteAtomicMemoryOperation(amoswap_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.amoxor_d: RewriteAtomicMemoryOperation(amoxor_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.amoxor_w: RewriteAtomicMemoryOperation(amoxor_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.and: RewriteBinOp(Operator.And); break;
                case Mnemonic.andi: RewriteBinOp(Operator.And); break;
                case Mnemonic.auipc: RewriteAuipc(); break;
                case Mnemonic.beq: RewriteBranch(m.Eq); break;
                case Mnemonic.bge: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu: RewriteBranch(m.Uge); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu: RewriteBranch(m.Ult); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.c_add: RewriteCompressedBinOp(Operator.IAdd); break;
                case Mnemonic.c_addi: RewriteCompressedBinOp(Operator.IAdd); break;
                case Mnemonic.c_addi16sp: RewriteAddi16sp(); break;
                case Mnemonic.c_addi4spn: RewriteAddi4spn(); break;
                case Mnemonic.c_addiw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Mnemonic.c_addw: RewriteCompressedAdd(PrimitiveType.Word32); break;
                case Mnemonic.c_and: RewriteCompressedBinOp(Operator.And); break;
                case Mnemonic.c_andi: RewriteCompressedBinOp(Operator.And); break;
                case Mnemonic.c_beqz: RewriteCompressedBranch(m.Eq); break;
                case Mnemonic.c_bnez: RewriteCompressedBranch(m.Ne); break;
                case Mnemonic.c_ebreak: RewriteEbreak(); break;
                case Mnemonic.c_fld: RewriteFload(PrimitiveType.Real64); break;
                case Mnemonic.c_flw: RewriteFload(PrimitiveType.Real32); break;
                case Mnemonic.c_fldsp: RewriteLxsp(PrimitiveType.Real64); break;
                case Mnemonic.c_fsd: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.c_fsw: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.c_fsdsp: RewriteSxsp(PrimitiveType.Real64); break;
                case Mnemonic.c_j: RewriteCompressedJ(); break;
                case Mnemonic.c_jal: RewriteCompressedJal(); break;
                case Mnemonic.c_jalr: RewriteCompressedJalr(); break;
                case Mnemonic.c_jr: RewriteCompressedJr(); break;
                case Mnemonic.c_ld: RewriteLoad(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.c_li: RewriteLi(); break;
                case Mnemonic.c_ldsp: RewriteLxsp(PrimitiveType.Word64); break;
                case Mnemonic.c_lui: RewriteLui(); break;
                case Mnemonic.c_lw: RewriteLoad(PrimitiveType.Word32, arch.NaturalSignedInteger); break;
                case Mnemonic.c_lwsp: RewriteLxsp(PrimitiveType.Word32); break;
                case Mnemonic.c_mv: RewriteMove(); break;
                case Mnemonic.c_nop: m.Nop(); break;
                case Mnemonic.c_or: RewriteCompressedBinOp(Operator.Or); break;
                case Mnemonic.c_slli: RewriteCompressedBinOp(SllI); break;
                case Mnemonic.c_srai: RewriteCompressedBinOp(SraI); break;
                case Mnemonic.c_srli: RewriteCompressedBinOp(SrlI); break;
                case Mnemonic.c_sub: RewriteCompressedBinOp(Operator.ISub); break;
                case Mnemonic.c_sd: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.c_sdsp: RewriteSxsp(PrimitiveType.Word64); break;
                case Mnemonic.c_subw: RewriteCompressedBinOp(Operator.ISub, PrimitiveType.Word32); break;
                case Mnemonic.c_sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.c_swsp: RewriteSxsp(PrimitiveType.Word32); break;
                case Mnemonic.c_xor: RewriteCompressedBinOp(Operator.Xor); break;
                case Mnemonic.csrrc: RewriteCsr(csrrc_intrinsic); break;
                case Mnemonic.csrrci: RewriteCsr(csrrc_intrinsic); break;
                case Mnemonic.csrrs: RewriteCsr(csrrs_intrinsic); break;
                case Mnemonic.csrrsi: RewriteCsr(csrrs_intrinsic); break;
                case Mnemonic.csrrw: RewriteCsr(csrrw_intrinsic); break;
                case Mnemonic.csrrwi: RewriteCsr(csrrw_intrinsic); break;
                case Mnemonic.div: RewriteBinOp(Operator.SDiv); break;
                case Mnemonic.divu: RewriteBinOp(Operator.UDiv); break;
                case Mnemonic.divuw: RewriteBinOp(Operator.UDiv, PrimitiveType.Word32); break;
                case Mnemonic.divw: RewriteBinOp(Operator.SDiv, PrimitiveType.Word32); break;
                case Mnemonic.ebreak: RewriteEbreak(); break;
                case Mnemonic.ecall: RewriteEcall(); break;
                case Mnemonic.fabs_d: RewriteFUnaryIntrinsic(PrimitiveType.Real64, FpOps.fabs); break;
                case Mnemonic.fadd_d: RewriteFBinOp(PrimitiveType.Real64, Operator.FAdd); break;
                case Mnemonic.fadd_s: RewriteFBinOp(PrimitiveType.Real32, Operator.FAdd); break;
                case Mnemonic.fclass_d: RewriteFUnaryIntrinsic(PrimitiveType.Real64, fclass_intrinsic.MakeInstance(PrimitiveType.Real64, arch.WordWidth)); break;
                case Mnemonic.fclass_s: RewriteFUnaryIntrinsic(PrimitiveType.Real32, fclass_intrinsic.MakeInstance(PrimitiveType.Real32, arch.WordWidth)); break;
                case Mnemonic.fcvt_d_l: RewriteFcvt(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Mnemonic.fcvt_d_w: RewriteFcvt(PrimitiveType.Int32, PrimitiveType.Real64); break;
                case Mnemonic.fcvt_d_lu: RewriteFcvt(PrimitiveType.UInt64, PrimitiveType.Real64); break;
                case Mnemonic.fcvt_d_wu: RewriteFcvt(PrimitiveType.UInt32, PrimitiveType.Real64); break;
                case Mnemonic.fcvt_d_s: RewriteFcvt(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.fcvt_l_d: RewriteFcvt(PrimitiveType.Real64, PrimitiveType.Int64); break;
                case Mnemonic.fcvt_l_s: RewriteFcvt(PrimitiveType.Real32, PrimitiveType.Int64); break;
                case Mnemonic.fcvt_lu_d: RewriteFcvt(PrimitiveType.Real64, PrimitiveType.UInt64); break;
                case Mnemonic.fcvt_lu_s: RewriteFcvt(PrimitiveType.Real32, PrimitiveType.UInt64); break;
                case Mnemonic.fcvt_s_d: RewriteFcvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.fcvt_s_l: RewriteFcvt(PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Mnemonic.fcvt_s_w: RewriteFcvt(PrimitiveType.Int32, PrimitiveType.Real32); break;
                case Mnemonic.fcvt_s_lu: RewriteFcvt(PrimitiveType.UInt64, PrimitiveType.Real32); break;
                case Mnemonic.fcvt_s_wu: RewriteFcvt(PrimitiveType.UInt32, PrimitiveType.Real32); break;
                case Mnemonic.fcvt_w_d: RewriteFcvt(PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.fcvt_w_s: RewriteFcvt(PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.fcvt_wu_d: RewriteFcvt(PrimitiveType.Real64, PrimitiveType.UInt32); break;
                case Mnemonic.fcvt_wu_s: RewriteFcvt(PrimitiveType.Real32, PrimitiveType.UInt32); break;
                case Mnemonic.fdiv_d: RewriteFBinOp(PrimitiveType.Real64, Operator.FDiv); break;
                case Mnemonic.fdiv_s: RewriteFBinOp(PrimitiveType.Real32, Operator.FDiv); break;
                case Mnemonic.fence: RewriteFence(); break;
                case Mnemonic.fence_i: RewriteFenceI(); break;
                case Mnemonic.fence_tso: RewriteFenceTso(); break;
                case Mnemonic.feq_d: RewriteFcmp(PrimitiveType.Real64, Operator.Feq); break;
                case Mnemonic.feq_s: RewriteFcmp(PrimitiveType.Real32, Operator.Feq); break;
                case Mnemonic.fle_d: RewriteFcmp(PrimitiveType.Real64, Operator.Fle); break;
                case Mnemonic.fle_s: RewriteFcmp(PrimitiveType.Real32, Operator.Fle); break;
                case Mnemonic.fld: RewriteFload(PrimitiveType.Real64); break;
                case Mnemonic.fli_d: RewriteMove(); break;
                case Mnemonic.fli_s: RewriteMove(); break;
                case Mnemonic.flq: RewriteFload(PrimitiveType.Real128); break;
                case Mnemonic.flw: RewriteFload(PrimitiveType.Real32); break;
                case Mnemonic.flt_d: RewriteFcmp(PrimitiveType.Real64, Operator.Flt); break;
                case Mnemonic.flt_q: RewriteFcmp(PrimitiveType.Real128, Operator.Flt); break;
                case Mnemonic.flt_s: RewriteFcmp(PrimitiveType.Real32, Operator.Flt); break;
                case Mnemonic.fmadd_d: RewriteFmadd(PrimitiveType.Real64, Operator.FAdd, false); break;
                case Mnemonic.fmadd_h: RewriteFmadd(PrimitiveType.Real16, Operator.FAdd, false); break;
                case Mnemonic.fmadd_q: RewriteFmadd(PrimitiveType.Real128, Operator.FAdd, false); break;
                case Mnemonic.fmadd_s: RewriteFmadd(PrimitiveType.Real32, Operator.FAdd, false); break;
                case Mnemonic.fmax_d: RewriteFBinaryIntrinsic(PrimitiveType.Real64, FpOps.fmax); break;
                case Mnemonic.fmax_s: RewriteFBinaryIntrinsic(PrimitiveType.Real32, FpOps.fmaxf); break;
                case Mnemonic.fmaxm_d: RewriteFBinaryIntrinsic(PrimitiveType.Real64, fmaxm_intrinsic.MakeInstance(PrimitiveType.Real64)); break;
                case Mnemonic.fmaxm_s: RewriteFBinaryIntrinsic(PrimitiveType.Real32, fmaxm_intrinsic.MakeInstance(PrimitiveType.Real32)); break;
                case Mnemonic.fmin_d: RewriteFBinaryIntrinsic(PrimitiveType.Real64, FpOps.fmin); break;
                case Mnemonic.fmin_s: RewriteFBinaryIntrinsic(PrimitiveType.Real32, FpOps.fminf); break;
                case Mnemonic.fminm_d: RewriteFBinaryIntrinsic(PrimitiveType.Real64, fminm_intrinsic.MakeInstance(PrimitiveType.Real64)); break;
                case Mnemonic.fminm_s: RewriteFBinaryIntrinsic(PrimitiveType.Real32, fminm_intrinsic.MakeInstance(PrimitiveType.Real32)); break;
                case Mnemonic.fmsub_d: RewriteFmadd(PrimitiveType.Real64, Operator.FSub, false); break;
                case Mnemonic.fmsub_h: RewriteFmadd(PrimitiveType.Real16, Operator.FSub, false); break;
                case Mnemonic.fmsub_q: RewriteFmadd(PrimitiveType.Real128, Operator.FSub, false); break;
                case Mnemonic.fmsub_s: RewriteFmadd(PrimitiveType.Real32, Operator.FSub, false); break;
                case Mnemonic.fmul_d: RewriteFBinOp(PrimitiveType.Real64, Operator.FMul); break;
                case Mnemonic.fmul_q: RewriteFBinOp(PrimitiveType.Real128, Operator.FMul); break;
                case Mnemonic.fmul_s: RewriteFBinOp(PrimitiveType.Real32, Operator.FMul); break;
                case Mnemonic.fmv_d_x: RewriteFMove(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Mnemonic.fmv_d: RewriteMove(); break;
                case Mnemonic.fmv_s: RewriteMove(); break;
                case Mnemonic.fmv_w_x: RewriteFMove(PrimitiveType.Real32, PrimitiveType.Real32); break;
                case Mnemonic.fmv_x_w: RewriteFMove(PrimitiveType.Real32, PrimitiveType.Real32); break;
                case Mnemonic.fmv_x_d: RewriteFMove(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.fneg_d: RewriteFneg(PrimitiveType.Real64); break;
                case Mnemonic.fneg_s: RewriteFneg(PrimitiveType.Real32); break;
                case Mnemonic.fnmadd_d: RewriteFmadd(PrimitiveType.Real64, Operator.FSub /* sic! */, true); break;
                case Mnemonic.fnmadd_h: RewriteFmadd(PrimitiveType.Real16, Operator.FSub /* sic! */, true); break;
                case Mnemonic.fnmadd_q: RewriteFmadd(PrimitiveType.Real128, Operator.FSub /* sic! */, true); break;
                case Mnemonic.fnmadd_s: RewriteFmadd(PrimitiveType.Real32, Operator.FSub /* sic! */, true); break;
                case Mnemonic.fnmsub_d: RewriteFmadd(PrimitiveType.Real64, Operator.FAdd /* sic! */, true); break;
                case Mnemonic.fnmsub_h: RewriteFmadd(PrimitiveType.Real16, Operator.FAdd /* sic! */, true); break;
                case Mnemonic.fnmsub_q: RewriteFmadd(PrimitiveType.Real128, Operator.FAdd /* sic! */, true); break;
                case Mnemonic.fnmsub_s: RewriteFmadd(PrimitiveType.Real32, Operator.FAdd /* sic! */, true); break;
                case Mnemonic.fsd: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.fsgnj_d: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real64, fsgnj_intrinsic); break;
                case Mnemonic.fsgnj_s: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real32, fsgnj_intrinsic); break;
                case Mnemonic.fsgnjn_d: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real64, fsgnjn_intrinsic); break;
                case Mnemonic.fsgnjn_s: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real32, fsgnjn_intrinsic); break;
                case Mnemonic.fsgnjx_d: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real64, fsgnjx_intrinsic); break;
                case Mnemonic.fsgnjx_s: RewriteFGenericBinaryIntrinsic(PrimitiveType.Real32, fsgnjx_intrinsic); break;
                case Mnemonic.fsub_d: RewriteFBinOp(PrimitiveType.Real64, Operator.FSub); break;
                case Mnemonic.fsqrt_d: RewriteFUnaryIntrinsic(PrimitiveType.Real64, FpOps.sqrt); break;
                case Mnemonic.fsqrt_s: RewriteFUnaryIntrinsic(PrimitiveType.Real32, FpOps.sqrtf); break;
                case Mnemonic.fsub_s: RewriteFBinOp(PrimitiveType.Real32, Operator.FSub); break;
                case Mnemonic.fsw: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.hfence_gvma: RewriteFence(hfence_gvma_intrinsic); break;
                case Mnemonic.hfence_vvma: RewriteFence(hfence_vvma_intrinsic); break;
                case Mnemonic.hinval_gvma: RewriteFence(hfence_gvma_intrinsic); break;
                case Mnemonic.hinval_vvma: RewriteFence(hfence_vvma_intrinsic); break;
                case Mnemonic.hlv_b: RewriteHlv(PrimitiveType.Int8, hlv_intrinsic); break;
                case Mnemonic.hlv_bu: RewriteHlv(PrimitiveType.Byte, hlv_intrinsic); break;
                case Mnemonic.hlv_d: RewriteHlv(PrimitiveType.Word64, hlv_intrinsic); break;
                case Mnemonic.hlv_h: RewriteHlv(PrimitiveType.Int16, hlv_intrinsic); break;
                case Mnemonic.hlv_hu: RewriteHlv(PrimitiveType.Word16, hlv_intrinsic); break;
                case Mnemonic.hlv_w: RewriteHlv(PrimitiveType.Int32, hlv_intrinsic); break;
                case Mnemonic.hlv_wu: RewriteHlv(PrimitiveType.Word32, hlv_intrinsic); break;
                case Mnemonic.hlvx_hu: RewriteHlv(PrimitiveType.Word16, hlvx_intrinsic); break;
                case Mnemonic.hlvx_wu: RewriteHlv(PrimitiveType.Word32, hlvx_intrinsic); break;
                case Mnemonic.hsv_b: RewriteHsv(PrimitiveType.Byte); break;
                case Mnemonic.hsv_d: RewriteHsv(PrimitiveType.Word64); break;
                case Mnemonic.hsv_h: RewriteHsv(PrimitiveType.Word16); break;
                case Mnemonic.hsv_w: RewriteHsv(PrimitiveType.Word32); break;
                case Mnemonic.jal: RewriteJal(); break;
                case Mnemonic.jalr: RewriteJalr(); break;
                case Mnemonic.lb: RewriteLoad(PrimitiveType.SByte, arch.NaturalSignedInteger); break;
                case Mnemonic.lbu: RewriteLoad(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.ld: RewriteLoad(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.lh: RewriteLoad(PrimitiveType.Int16, arch.NaturalSignedInteger); break;
                case Mnemonic.lhu: RewriteLoad(PrimitiveType.UInt16, arch.WordWidth); break;
                case Mnemonic.lr_d: RewriteLoadReserved(PrimitiveType.Word64); break;
                case Mnemonic.lr_w: RewriteLoadReserved(PrimitiveType.Word64); break;
                case Mnemonic.lui: RewriteLui(); break;
                case Mnemonic.lw: RewriteLoad(PrimitiveType.Int32, arch.NaturalSignedInteger); break;
                case Mnemonic.lwu: RewriteLoad(PrimitiveType.UInt32, arch.WordWidth); break;
                case Mnemonic.mul: RewriteBinOp(Operator.IMul, PrimitiveType.Word64); break;
                case Mnemonic.mulh: RewriteMulh(Operator.SMul, arch.DoubleWordSignedInteger); break;
                case Mnemonic.mulhsu: RewriteMulh(Operator.SMul, arch.DoubleWordWidth); break;
                case Mnemonic.mulhu: RewriteMulh(Operator.UMul, arch.DoubleWordWidth); break;
                case Mnemonic.mulw: RewriteBinOp(Operator.IMul, PrimitiveType.Word32); break;
                case Mnemonic.mret: RewriteRet(mret_intrinsic); break;
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.ori: RewriteOr(); break;
                case Mnemonic.pause: RewritePause(); break;
                case Mnemonic.rem: RewriteBinOp(Operator.SMod); break;
                case Mnemonic.remu: RewriteBinOp(Operator.UMod, PrimitiveType.Word64); break;
                case Mnemonic.remuw: RewriteBinOp(Operator.UMod, PrimitiveType.Word32); break;
                case Mnemonic.remw: RewriteBinOp(Operator.SMod, PrimitiveType.Word32); break;
                case Mnemonic.sb: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.sc_d: RewriteStoreConditional(PrimitiveType.Word64); break;
                case Mnemonic.sc_w: RewriteStoreConditional(PrimitiveType.Word32); break;
                case Mnemonic.sd: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.sfence_inval: RewriteFence(sfence_inval_intrinsic); break;
                case Mnemonic.sfence_inval_ir: RewriteFence(sfence_inval_ir_intrinsic); break;
                case Mnemonic.sfence_vma: RewriteFence(sfence_vma_intrinsic); break;
                case Mnemonic.sfence_w_inval: RewriteFence(sfence_w_inval_intrinsic); break;
                case Mnemonic.sh: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.sret: RewriteRet(sret_intrinsic); break;
                case Mnemonic.sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.sll: RewriteBinOp(Operator.Shl); break;
                case Mnemonic.slli: RewriteShift(Operator.Shl); break;
                case Mnemonic.slliw: RewriteShiftw(m.Shl); break;
                case Mnemonic.sllw: RewriteShiftw(m.Shl); break;
                case Mnemonic.slt: RewriteSlt(false); break;
                case Mnemonic.slti: RewriteSlti(false); break;
                case Mnemonic.sltiu: RewriteSlti(true); break;
                case Mnemonic.sltu: RewriteSlt(true); break;
                case Mnemonic.sra: RewriteShift(Operator.Sar); break;
                case Mnemonic.sraw: RewriteShiftw(m.Sar); break;
                case Mnemonic.srai: RewriteShift(Operator.Sar); break;
                case Mnemonic.sraiw: RewriteShiftw(m.Sar); break;
                case Mnemonic.srl: RewriteBinOp(Operator.Shr); break;
                case Mnemonic.srli: RewriteShift(Operator.Shr); break;
                case Mnemonic.srliw: RewriteShiftw(SrlI); break;
                case Mnemonic.srlw: RewriteShiftw(m.Shr); break;
                case Mnemonic.sub: RewriteSub(); break;
                case Mnemonic.subw: RewriteSubw(); break;
                case Mnemonic.uret: RewriteRet(uret_intrinsic); break;
                case Mnemonic.wfi: RewriteWfi(); break;
                case Mnemonic.xor: RewriteXor(); break;
                case Mnemonic.xori: RewriteXor(); break;
                }
                yield return m.MakeCluster(addr, len, iclass);
                rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(int iop) => RewriteOp(instr.Operands[iop], null);

        private Expression RewriteOp(MachineOperand op, PrimitiveType? dt)
        {
            switch (op)
            {
            case RegisterStorage rop:
                if (rop.Number == 0)
                {
                    return Constant.Zero(dt ?? arch.WordWidth);
                }
                var reg = binder.EnsureRegister(rop);
                return reg;
            case ImmediateOperand immop:
                return immop.Value;
            case AddressOperand addrop:
                return addrop.Address;
            case MemoryOperand mem:
                var ea = (Expression)binder.EnsureRegister(mem.Base!);
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                return m.Mem(mem.Width, ea);
            }
            throw new NotImplementedException($"Rewriting RiscV addressing mode {op.GetType().Name} is not implemented yet.");
        }

        private void MaybeSignExtend(Expression dst, Expression src)
        {
            // Risc-V spec states all short values are sign-extended.
            if (src.DataType.BitSize < dst.DataType.BitSize)
            {
                if (src is Constant cSrc)
                {
                    src = Constant.Int(dst.DataType, cSrc.ToInt32());
                }
                else
                {
                    var tmp = binder.CreateTemporary(src.DataType);
                    m.Assign(tmp, src);
                    src = m.Convert(tmp, src.DataType, PrimitiveType.Create(Domain.SignedInt, dst.DataType.BitSize));
                }
            }
            m.Assign(dst, src);
        }

        private void MaybeSliceSignExtend(Expression dst, Expression src, DataType? dt = null)
        {
            if (dt != null && dt.BitSize < dst.DataType.BitSize)
            {
                src = m.Convert(m.Slice(src, dt), dt, arch.NaturalSignedInteger);
            }
            if (dst.DataType.BitSize > src.DataType.BitSize && src is Constant cSrc)
            {
                m.Assign(dst, cSrc.ToInt32());
            }
            else
            {
                m.Assign(dst, src);
            }
        }

        private Expression SllI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Shl(a, b);
        }

        private Expression SraI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Sar(a, b);
        }

        private Expression SrlI(Expression a, Expression b)
        {
            b = Constant.Int32(((Constant) b).ToInt32());
            return m.Shr(a, b);
        }

        /// <summary>
        /// Emits the text of a unit test that can be pasted into the unit tests 
        /// for this rewriter.
        /// </summary>
        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("RiscV_rw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private static IntrinsicProcedure AmoIntrinsic(string intrinsicName)
        {
            return new IntrinsicBuilder(intrinsicName, true)
                .GenericTypes("T")
                .Param("T")
                .PtrParam("T")
                .Returns("T");
        }

        static readonly IntrinsicProcedure amoadd_intrinsic = AmoIntrinsic("__amo_add");
        static readonly IntrinsicProcedure amoand_intrinsic = AmoIntrinsic("__amo_and");
        static readonly IntrinsicProcedure amomax_intrinsic = AmoIntrinsic("__amo_max");
        static readonly IntrinsicProcedure amomin_intrinsic = AmoIntrinsic("__amo_min");
        static readonly IntrinsicProcedure amoor_intrinsic = AmoIntrinsic("__amo_or");
        static readonly IntrinsicProcedure amoswap_intrinsic = AmoIntrinsic("__amo_swap");
        static readonly IntrinsicProcedure amoxor_intrinsic = AmoIntrinsic("__amo_xor");

        static readonly IntrinsicProcedure csrrc_intrinsic = IntrinsicBuilder.GenericBinary("__csrrc");
        static readonly IntrinsicProcedure csrrs_intrinsic = IntrinsicBuilder.GenericBinary("__csrrs");
        static readonly IntrinsicProcedure csrrw_intrinsic = IntrinsicBuilder.GenericBinary("__csrrw");
        static readonly IntrinsicProcedure ebreak_intrinsic = new IntrinsicBuilder("__ebreak", true)
            .Void();

        static readonly IntrinsicProcedure fence_intrinsic = new IntrinsicBuilder("__fence", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        static readonly IntrinsicProcedure fence_i_intrinsic = new IntrinsicBuilder("__fence_i", true)
            .Void();
        static readonly IntrinsicProcedure fence_tso_intrinsic = new IntrinsicBuilder("__fence_tso", true)
            .Void();
        static readonly IntrinsicProcedure fclass_intrinsic = new IntrinsicBuilder("__fclass", false)
            .GenericTypes("TFloat", "TResult")
            .Param("TFloat")
            .Returns("TResult");
        static readonly IntrinsicProcedure fmaxm_intrinsic = IntrinsicBuilder.GenericBinary("__fmaxm");
        static readonly IntrinsicProcedure fminm_intrinsic = IntrinsicBuilder.GenericBinary("__fminm");
        static readonly IntrinsicProcedure fsgnj_intrinsic = IntrinsicBuilder.GenericBinary("__fsgnj");
        static readonly IntrinsicProcedure fsgnjn_intrinsic = IntrinsicBuilder.GenericBinary("__fsgnjn");
        static readonly IntrinsicProcedure fsgnjx_intrinsic = IntrinsicBuilder.GenericBinary("__fsgnjx");

        static readonly IntrinsicProcedure hfence_gvma_intrinsic = new IntrinsicBuilder("__hfence_gvma", true)
                    .GenericTypes("T","U")
                    .Param("T")
                    .Param("U")
                    .Void();
        static readonly IntrinsicProcedure hfence_vvma_intrinsic    = new IntrinsicBuilder("__hfence_vvma", true)
                    .GenericTypes("T","U")
                    .Param("T")
                    .Param("U")
                    .Void();

        static readonly IntrinsicProcedure sfence_inval_intrinsic = new IntrinsicBuilder("__sfence_inval", true)
                    .GenericTypes("T", "U")
                    .Param("T")
                    .Param("U")
                    .Void();
        static readonly IntrinsicProcedure sfence_inval_ir_intrinsic= new IntrinsicBuilder("__sfence_inval_ir", true)
                    .Void();
        static readonly IntrinsicProcedure sfence_vma_intrinsic     = new IntrinsicBuilder("__sfence_vma", true)
                    .GenericTypes("T", "U")
                    .Param("T")
                    .Param("U")
                    .Void();
        static readonly IntrinsicProcedure sfence_w_inval_intrinsic = new IntrinsicBuilder("__sfence_w_inval", true)
                    .Void();


        static readonly IntrinsicProcedure hlv_intrinsic = new IntrinsicBuilder("__hypervisor_load_from_VM", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        static readonly IntrinsicProcedure hlvx_intrinsic = new IntrinsicBuilder("__hypervisor_load_exe_from_VM", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        static readonly IntrinsicProcedure hsv_intrinsic = new IntrinsicBuilder("__hypervisor_store_in_VM", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Void();


        static readonly IntrinsicProcedure lr_intrinsic = new IntrinsicBuilder("__load_reserved", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");

        static readonly IntrinsicProcedure mret_intrinsic = new IntrinsicBuilder("__mret", true)
            .Void();

        static readonly IntrinsicProcedure pause_intrinsic = new IntrinsicBuilder("__pause", true)
            .Void();

        static readonly IntrinsicProcedure sc_intrinsic = new IntrinsicBuilder("__store_conditional", true)
            .GenericTypes("T")
            .Param("T")
            .PtrParam("T")
            .Returns("T");
        static readonly IntrinsicProcedure sret_intrinsic = new IntrinsicBuilder("__sret", true)
            .Void();

        static readonly IntrinsicProcedure uret_intrinsic = new IntrinsicBuilder("__uret", true)
            .Void();
        static readonly IntrinsicProcedure wait_for_interrupt_intrinsic = new IntrinsicBuilder("__wait_for_interrupt", true)
            .Void();
    }
}