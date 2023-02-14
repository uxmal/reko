#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    // https://developer.arm.com/technologies/neon/intrinsics?_ga=2.34513633.1180694635.1535627528-1335591578.1525783726
    public partial class A64Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly Arm64Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly List<RtlInstruction> cluster;
        private readonly RtlEmitter m;
        private readonly IEnumerator<AArch64Instruction> dasm;
        private AArch64Instruction instr;
        private InstrClass iclass;

        public A64Rewriter(Arm64Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new AArch64Disassembler(arch, rdr).GetEnumerator();
            this.instr = null!;
            this.cluster = new List<RtlInstruction>();
            this.m = new RtlEmitter(cluster);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                iclass = instr.InstructionClass;
                try
                {
                    switch (instr.Mnemonic)
                    {
                    default:
                        EmitUnitTest();
                        host.Warn(
                            instr.Address,
                            "AArch64 instruction {0} is not supported yet.",
                            instr);
                        goto case Mnemonic.Invalid;
                    case Mnemonic.Invalid:
                        iclass = InstrClass.Invalid;
                        m.Invalid();
                        break;
                    case Mnemonic.abs: RewriteSimdUnary(abs_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
                    case Mnemonic.adcs: RewriteAdcSbc(m.IAdd, this.NZCV); break;
                    case Mnemonic.add: RewriteMaybeSimdBinary(m.IAdd, Simd.Add); break;
                    case Mnemonic.addhn: RewriteSimdBinary(addhn_intrinsic, Domain.None); break;
                    case Mnemonic.addhn2: RewriteSimdBinary(addhn2_intrinsic, Domain.None); break;
                    case Mnemonic.addp: RewriteSimdBinary(addp_intrinsic, Domain.None); break;
                    case Mnemonic.adds: RewriteBinary(m.IAdd, this.NZCV); break;
                    case Mnemonic.addv: RewriteAddv(); break;
                    case Mnemonic.adr: RewriteUnary(n => n); break;
                    case Mnemonic.asrv: RewriteBinary(m.Sar); break;
                    case Mnemonic.adrp: RewriteAdrp(); break;
                    case Mnemonic.and: RewriteBinary(m.And); break;
                    case Mnemonic.ands: RewriteBinary(m.And, this.NZ00); break;
                    case Mnemonic.asr: RewriteBinary(m.Sar); break;
                    case Mnemonic.b: RewriteB(); break;
                    case Mnemonic.bfm: RewriteBfm(); break;
                    case Mnemonic.bic: RewriteLogical((a, b) => m.And(a, m.Comp(b))); break;
                    case Mnemonic.bics: RewriteBinary((a, b) => m.And(a, m.Comp(b)), NZ00); break;
                    case Mnemonic.bif: RewriteSimdBinary(bif_intrinsic, Domain.None); break;
                    case Mnemonic.bit: RewriteSimdBinary(bit_intrinsic, Domain.None); break;
                    case Mnemonic.bl: RewriteBl(); break;
                    case Mnemonic.blr: RewriteBlr(); break;
                    case Mnemonic.br: RewriteBr(); break;
                    case Mnemonic.brk: RewriteBrk(); break;
                    case Mnemonic.bsl: RewriteBsl(); break;
                    case Mnemonic.cbnz: RewriteCb(m.Ne0); break;
                    case Mnemonic.cbz: RewriteCb(m.Eq0); break;
                    case Mnemonic.ccmn: RewriteCcmn(); break;
                    case Mnemonic.ccmp: RewriteCcmp(); break;
                    case Mnemonic.cls: RewriteSimdUnary(cls_intrinsic, Domain.None); break;
                    case Mnemonic.clz: RewriteClz(); break;
                    case Mnemonic.cmp: RewriteCmp(); break;
                    case Mnemonic.cmeq: RewriteCm(cmeq_intrinsic, Domain.None); break;
                    case Mnemonic.cmge: RewriteCm(cmge_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.cmgt: RewriteCm(cmgt_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.cmhi: RewriteCm(cmhi_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.cmhs: RewriteCm(cmhs_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.cmle: RewriteCm(cmle_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.cmlt: RewriteCm(cmlt_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.cmtst: RewriteCm(cmtst_intrinsic, Domain.None); break;
                    case Mnemonic.cnt: RewriteSimdUnary(cnt_intrinsic, Domain.None); break;
                    case Mnemonic.csel: RewriteCsel(); break;
                    case Mnemonic.csinc: RewriteCsinc(); break;
                    case Mnemonic.csinv: RewriteCsinv(); break;
                    case Mnemonic.csneg: RewriteCsneg(); break;
                    case Mnemonic.dmb: RewriteDmb(); break;
                    case Mnemonic.dsb: RewriteDsb(); break;
                    case Mnemonic.dup: RewriteSimdUnaryWithScalar(dup_intrinsic, Domain.None); break;
                    case Mnemonic.eor: RewriteBinary(m.Xor); break;
                    case Mnemonic.eon: RewriteBinary((a, b) => m.Xor(a, m.Comp(b))); break;
                    case Mnemonic.eret: RewriteEret(); break;
                    case Mnemonic.ext: RewriteSimdTernaryWithScalar(ext_intrinsic, Domain.None); break;
                    case Mnemonic.extr: RewriteExtr(); break;
                    case Mnemonic.fabs: RewriteFabs(); break;
                    case Mnemonic.fadd: RewriteMaybeSimdBinary(m.FAdd, Simd.FAdd, Domain.Real); break;
                    case Mnemonic.fcmp: RewriteFcmp(); break;
                    case Mnemonic.fcmpe: RewriteFcmp(); break;  //$REVIEW: this leaves out the 'e'xception part. 
                    case Mnemonic.fcsel: RewriteFcsel(); break;
                    case Mnemonic.fcvt: RewriteFcvt_Obsolete(); break;
                    case Mnemonic.fcvtas: RewriteFcvta(Domain.SignedInt); break;
                    case Mnemonic.fcvtau: RewriteFcvta(Domain.UnsignedInt); break;
                    case Mnemonic.fcvtms: RewriteFcvtm(Domain.SignedInt); break;
                    case Mnemonic.fcvtmu: RewriteFcvtm(Domain.UnsignedInt); break;
                    case Mnemonic.fcvtns: RewriteFcvtn(Domain.SignedInt); break;
                    case Mnemonic.fcvtnu: RewriteFcvtn(Domain.UnsignedInt); break;
                    case Mnemonic.fcvtps: RewriteFcvtp(Domain.SignedInt); break;
                    case Mnemonic.fcvtpu: RewriteFcvtp(Domain.UnsignedInt); break;
                    case Mnemonic.fcvtzs: RewriteFcvtz(Domain.SignedInt); break;
                    case Mnemonic.fcvtzu: RewriteFcvtz(Domain.UnsignedInt); break;
                    case Mnemonic.fdiv: RewriteMaybeSimdBinary(m.FDiv, Simd.FDiv, Domain.Real); break;
                    case Mnemonic.fmadd: RewriteIntrinsicFTernary(__fmadd); break;
                    case Mnemonic.fmsub: RewriteIntrinsicFTernary(__fmsub); break;
                    case Mnemonic.fmax: RewriteIntrinsicFBinary(FpOps.fmaxf, FpOps.fmax, Simd.Max); break;
                    case Mnemonic.fmin: RewriteIntrinsicFBinary(FpOps.fminf, FpOps.fmin, Simd.Min); break;
                    case Mnemonic.fmov: RewriteFmov(); break;
                    case Mnemonic.fmul: RewriteFmul(); break;
                    case Mnemonic.fneg: RewriteUnary(m.FNeg); break;
                    case Mnemonic.fnmadd: RewriteIntrinsicFTernary(__fnmadd); break;
                    case Mnemonic.fnmsub: RewriteIntrinsicFTernary(__fnmsub); break;
                    case Mnemonic.fnmul: RewriteFnmul(); break;
                    case Mnemonic.fsqrt: RewriteFsqrt(); break;
                    case Mnemonic.fsub: RewriteMaybeSimdBinary(m.FSub, Simd.FSub, Domain.Real); break;
                    case Mnemonic.hlt: RewriteHlt(); break;
                    case Mnemonic.isb: RewriteIsb(); break;
                    case Mnemonic.ld1: RewriteLdN("__ld1"); break;
                    case Mnemonic.ld1r: RewriteLdNr("__ld1r"); break;
                    case Mnemonic.ld2: RewriteLdN("__ld2"); break;
                    case Mnemonic.ld3: RewriteLdN("__ld3"); break;
                    case Mnemonic.ld4: RewriteLdN("__ld4"); break;
                    case Mnemonic.ldnp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldarh: RewriteLoadAcquire(load_acquire_intrinsic, PrimitiveType.Word16); break;
                    case Mnemonic.ldaxrh: RewriteLoadAcquire(load_acquire_exclusive_intrinsic, PrimitiveType.Word16); break;
                    case Mnemonic.ldpsw: RewriteLoadStorePair(true, PrimitiveType.Int32, PrimitiveType.Int64); break;
                    case Mnemonic.ldr: RewriteLdr(null); break;
                    case Mnemonic.ldrb: RewriteLdr(PrimitiveType.Byte); break;
                    case Mnemonic.ldrh: RewriteLdr(PrimitiveType.Word16); break;
                    case Mnemonic.ldrsb: RewriteLdr(PrimitiveType.SByte); break;
                    case Mnemonic.ldrsh: RewriteLdr(PrimitiveType.Int16); break;
                    case Mnemonic.ldrsw: RewriteLdr(PrimitiveType.Int32, PrimitiveType.Int64); break;
                    case Mnemonic.ldxr: RewriteLdx(instr.Operands[0].Width); break;
                    case Mnemonic.lslv: RewriteBinary(m.Shl); break;
                    case Mnemonic.lsrv: RewriteBinary(m.Shr); break;
                    case Mnemonic.ldur: RewriteLdr(null); break;
                    case Mnemonic.ldurb: RewriteLdr(PrimitiveType.Byte); break;
                    case Mnemonic.ldurh: RewriteLdr(PrimitiveType.Word16); break;
                    case Mnemonic.ldursb: RewriteLdr(PrimitiveType.SByte); break;
                    case Mnemonic.ldursh: RewriteLdr(PrimitiveType.Int16); break;
                    case Mnemonic.ldursw: RewriteLdr(PrimitiveType.Int32); break;
                    case Mnemonic.lsl: RewriteBinary(m.Shl); break;
                    case Mnemonic.lsr: RewriteBinary(m.Shr); break;
                    case Mnemonic.madd: RewriteMaddSub(m.IAdd); break;
                    case Mnemonic.mla: RewriteSimdTernary(Domain.None); break;
                    case Mnemonic.mls: RewriteSimdTernary(Domain.None); break;
                    case Mnemonic.mneg: RewriteBinary((a, b) => m.Neg(m.IMul(a, b))); break;
                    case Mnemonic.mov: RewriteMov(); break;
                    case Mnemonic.movi: RewriteMovi(); break;
                    case Mnemonic.movk: RewriteMovk(); break;
                    case Mnemonic.movn: RewriteMovn(); break;
                    case Mnemonic.movz: RewriteMovz(); break;
                    case Mnemonic.mrs: RewriteMrs(); break;
                    case Mnemonic.msr: RewriteMsr(); break;
                    case Mnemonic.msub: RewriteMaddSub(m.ISub); break;
                    case Mnemonic.mul: RewriteMaybeSimdBinary(m.IMul, Simd.Mul); break;
                    case Mnemonic.mvn: RewriteUnary(m.Comp); break;
                    case Mnemonic.mvni: RewriteLogical((a, b) => ((Constant) b).Complement()); break;
                    case Mnemonic.neg: RewriteSimdUnary(neg_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.nop: m.Nop(); break;
                    case Mnemonic.not: RewriteMaybeSimdUnary(m.Comp, Simd.Not); break;
                    case Mnemonic.orr: RewriteLogical(m.Or); break;
                    case Mnemonic.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                    case Mnemonic.pmul: RewriteSimdBinary(pmul_intrinsic, Domain.None); break;
                    case Mnemonic.pmull: RewriteSimdBinary(pmull_intrinsic, Domain.None); break;
                    case Mnemonic.pmull2: RewriteSimdBinary(pmull2_intrinsic, Domain.None); break;
                    case Mnemonic.prfm: RewritePrfm(); break;
                    case Mnemonic.raddhn: RewriteSimdBinary(raddhn_intrinsic, Domain.None); break;
                    case Mnemonic.raddhn2: RewriteSimdBinary(raddhn2_intrinsic, Domain.None); break;
                    case Mnemonic.rbit: RewriteRbit(); break;
                    case Mnemonic.ret: RewriteRet(); break;
                    case Mnemonic.rev: RewriteRev(); break;
                    case Mnemonic.rev16: RewriteRev16(); break;
                    case Mnemonic.rev32: RewriteRev32(); break;
                    case Mnemonic.rev64: RewriteSimdUnary(rev64_intrinsic, Domain.None); break;
                    case Mnemonic.ror: RewriteRor(); break;
                    case Mnemonic.rorv: RewriteRor(); break;
                    case Mnemonic.rshrn: RewriteSimdBinary(rshrn_intrinsic, Domain.None); break;
                    case Mnemonic.rshrn2: RewriteSimdBinary(rshrn2_intrinsic, Domain.None); break;
                    case Mnemonic.rsubhn: RewriteSimdBinaryWiden(rsubhn_intrinsic, Domain.None); break;
                    case Mnemonic.rsubhn2: RewriteSimdBinaryWiden(rsubhn2_intrinsic, Domain.None); break;
                    case Mnemonic.saba: RewriteSimdBinary(saba_intrinsic, Domain.None); break;
                    case Mnemonic.sabal: RewriteSimdBinary(sabal_intrinsic, Domain.None); break;
                    case Mnemonic.sabal2: RewriteSimdBinary(sabal2_intrinsic, Domain.None); break;
                    case Mnemonic.sabd: RewriteSimdBinary(sabd_intrinsic, Domain.None); break;
                    case Mnemonic.sabdl: RewriteSimdBinaryWiden(sabdl_intrinsic, Domain.None); break;
                    case Mnemonic.sabdl2: RewriteSimdBinaryWiden(sabdl2_intrinsic, Domain.None); break;
                    case Mnemonic.sadalp: RewriteSimdBinary(sadalp_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddl: RewriteSimdBinaryWiden(saddl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddl2: RewriteSimdBinaryWiden(saddl2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddlp: RewriteSimdUnaryChangeSize(saddlp_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddlv: RewriteSimdUnary(saddlv_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddw: RewriteSimdBinaryWideNarrow(saddw_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.saddw2: RewriteSimdBinaryWideNarrow(saddw2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
                    case Mnemonic.sbcs: RewriteAdcSbc(m.ISub, NZCV); break;
                    case Mnemonic.sbfiz: RewriteSbfiz(); break;
                    case Mnemonic.sbfm: RewriteUSbfm("__sbfm"); break;
                    case Mnemonic.scvtf: RewriteIcvtf("s", Domain.SignedInt); break;
                    case Mnemonic.sdiv: RewriteBinary(m.SDiv); break;
                    case Mnemonic.shadd: RewriteSimdBinary(shadd_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sha1c: RewriteSha1c(); break;
                    case Mnemonic.shl: RewriteSimdBinary(shl_intrinsic, Domain.None); break;
                    case Mnemonic.shll: RewriteSimdBinary(shll_intrinsic, Domain.None); break;
                    case Mnemonic.shll2: RewriteSimdBinary(shll2_intrinsic, Domain.None); break;
                    case Mnemonic.shrn: RewriteSimdBinaryWithScalar(shrn_intrinsic, Domain.None); break;
                    case Mnemonic.shsub: RewriteSimdBinary(shsub_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sli: RewriteSimdBinary(sli_intrinsic, Domain.None); break;
                    case Mnemonic.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smax: RewriteSimdBinary(smax_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.smaxp: RewriteSimdBinary(smaxp_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.smaxv: RewriteSimdUnaryReduce(smaxv_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.smc: RewriteSmc(); break;
                    case Mnemonic.smin: RewriteSimdBinary(smin_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sminp: RewriteSimdBinary(sminp_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sminv: RewriteSimdUnary(sminv_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.smlal: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.smlal2: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.smlsl: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.smlsl2: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.smov: RewriteVectorElementToScalar(Domain.SignedInt); break;
                    case Mnemonic.smsubl: RewriteSmsubl(); break;
                    case Mnemonic.smull: RewriteMull(PrimitiveType.Int32, PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smull2: RewriteSimdBinary(smull2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.smulh: RewriteMulh(PrimitiveType.Int64, PrimitiveType.Int128, m.SMul); break;
                    case Mnemonic.sqabs: RewriteSimdUnary(sqabs_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqadd: RewriteSimdBinary(sqadd_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqdmulh: RewriteMaybeSimdBinary(sqdmulh_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqdmull: RewriteSqdmull(sqdmull_intrinsic); break;
                    case Mnemonic.sqdmull2: RewriteSqdmull(sqdmull2_intrinsic); break;
                    case Mnemonic.sqdmlal: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlal2: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl2: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqneg: RewriteSimdUnary(sqneg_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqrdmlah: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqrdmlsh: RewriteSimdTernary(Domain.SignedInt); break;
                    case Mnemonic.sqrdmulh: RewriteBinary(sqrdmulh_intrinsic); break;
                    case Mnemonic.sqrshl: RewriteSimdBinary(sqrshl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqrshrn: RewriteSimdBinary(sqrshrn_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqrshrn2: RewriteSimdBinary(sqrshrn2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqrshrun: RewriteSimdBinary(sqrshrun_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqrshrun2: RewriteSimdBinary(sqrshrun2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqshl: RewriteSimdBinary(sqshl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqshlu: RewriteSimdBinary(sqshlu_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqshrn: RewriteSimdBinary(sqshrn_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqshrn2: RewriteSimdBinary(sqshrn2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqsub: RewriteSimdBinary(sqsub_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqxtn: RewriteSimdUnaryChangeSize(sqxtn_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqxtn2: RewriteSimdUnaryChangeSize(sqxtn2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqxtun: RewriteSimdUnaryChangeSize(sqxtun_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sqxtun2: RewriteSimdUnaryChangeSize(sqxtun2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sri: RewriteSimdBinary(sri_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.srhadd: RewriteSimdBinary(srhadd_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.srshl: RewriteSimdBinary(srshl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.srshr: RewriteSimdBinary(srshr_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.srsra: RewriteSimdBinary(srsra_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sshl: RewriteSimdBinaryWithScalar(sshl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sshll: RewriteSimdBinary(sshll_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sshll2: RewriteSimdBinary(sshll2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sshr: RewriteSimdBinaryWithScalar(sshr_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.ssra: RewriteSimdBinaryWithScalar(ssra_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.ssubl: RewriteSimdBinary(ssubl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.ssubl2: RewriteSimdBinary(ssubl2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.ssubw: RewriteSimdBinary(ssubw_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.ssubw2: RewriteSimdBinary(ssubw2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.subhn: RewriteSimdBinary(subhn_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.subhn2: RewriteSimdBinaryWiden(subhn2_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.suqadd: RewriteSimdBinary(suqadd_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.st1: RewriteStN("__st1"); break;
                    case Mnemonic.st2: RewriteStN("__st2"); break;
                    case Mnemonic.st3: RewriteStN("__st3"); break;
                    case Mnemonic.st4: RewriteStN("__st4"); break;
                    case Mnemonic.stlr: RewriteStlr(instr.Operands[0].Width); break;
                    case Mnemonic.stlrh: RewriteStlr(PrimitiveType.Word16); break;
                    case Mnemonic.stnp: RewriteLoadStorePair(false); break; //$REVIEW: does the non-temporality matter?
                    case Mnemonic.stp: RewriteLoadStorePair(false); break;
                    case Mnemonic.str: RewriteStr(null); break;
                    case Mnemonic.strb: RewriteStr(PrimitiveType.Byte); break;
                    case Mnemonic.strh: RewriteStr(PrimitiveType.Word16); break;
                    case Mnemonic.stur: RewriteStr(null); break;
                    case Mnemonic.sturb: RewriteStr(PrimitiveType.Byte); break;
                    case Mnemonic.sturh: RewriteStr(PrimitiveType.Word16); break;
                    case Mnemonic.stxr: RewriteStx(instr.Operands[1].Width); break;
                    case Mnemonic.stxrb: RewriteStx(PrimitiveType.Byte); break;
                    case Mnemonic.sub: RewriteBinary(m.ISub); break;
                    case Mnemonic.subs: RewriteBinary(m.ISub, NZCV); break;
                    case Mnemonic.svc: RewriteSvc(); break;
                    case Mnemonic.sxtb: RewriteUSxt(Domain.SignedInt, 8); break;
                    case Mnemonic.sxth: RewriteUSxt(Domain.SignedInt, 16); break;
                    case Mnemonic.sxtl: RewriteSimdUnaryChangeSize(sxtl_intrinsic, Domain.SignedInt); break;
                    case Mnemonic.sxtw: RewriteUSxt(Domain.SignedInt, 32); break;
                    case Mnemonic.tbnz: RewriteTb(m.Ne0); break;
                    case Mnemonic.tbl: RewriteTbl(); break;
                    case Mnemonic.tbx: RewriteTbx(); break;
                    case Mnemonic.tbz: RewriteTb(m.Eq0); break;
                    case Mnemonic.test: RewriteTest(); break;
                    case Mnemonic.trn1: RewriteSimdBinary(trn1_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.trn2: RewriteSimdBinary(trn2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaba: RewriteSimdBinary(uaba_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uabal: RewriteSimdBinaryWiden(uabal_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uabal2: RewriteSimdBinaryWiden(uabal2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uabd: RewriteSimdBinary(uabd_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uabdl: RewriteSimdBinary(uabdl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uabdl2: RewriteSimdBinary(uabdl2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uadalp: RewriteSimdBinary(uadalp_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaddl: RewriteSimdBinaryWiden(uaddl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaddl2: RewriteSimdBinaryWiden(uaddl2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaddlp: RewriteSimdBinary(uaddlp_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaddlv: RewriteSimdUnary(uaddlv_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uaddw: RewriteUaddw(); break;
                    case Mnemonic.uaddw2: RewriteSimdBinary(uaddw_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ubfm: RewriteUSbfm("__ubfm"); break;
                    case Mnemonic.ucvtf: RewriteIcvtf("u", Domain.UnsignedInt); break;
                    case Mnemonic.udiv: RewriteBinary(m.UDiv); break;
                    case Mnemonic.uhadd: RewriteSimdBinary(uhadd_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uhsub: RewriteSimdBinary(uhsub_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umax: RewriteSimdBinary(umax_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.umaxp: RewriteSimdBinary(umaxp_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.umaxv: RewriteSimdUnary(umaxv_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.umin: RewriteSimdBinary(umin_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uminp: RewriteSimdBinary(uminp_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uminv: RewriteSimdUnary(uminv_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.umlal: RewriteSimdTernary(Domain.UnsignedInt); break;
                    case Mnemonic.umlal2: RewriteSimdTernary(Domain.UnsignedInt); break;
                    case Mnemonic.umlsl: RewriteSimdTernary(Domain.UnsignedInt); break;
                    case Mnemonic.umlsl2: RewriteSimdTernary(Domain.UnsignedInt); break;
                    case Mnemonic.umov: RewriteVectorElementToScalar(Domain.UnsignedInt); break;
                    case Mnemonic.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull: RewriteMull(PrimitiveType.UInt32, PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull2: RewriteSimdBinary(umull2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqadd: RewriteSimdBinary(uqadd_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshl: RewriteSimdBinary(uqrshl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn: RewriteSimdBinary(uqrshrn_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn2: RewriteSimdBinary(uqrshrn2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqshl: RewriteSimdBinary(uqshl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn: RewriteSimdBinary(uqshrn_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn2: RewriteSimdBinary(uqshrn2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqsub: RewriteSimdBinary(uqsub_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn: RewriteSimdUnary(uqxtn_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn2: RewriteSimdUnary(uqxtn2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.urecpe: RewriteSimdUnary(urecpe_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.urhadd: RewriteSimdBinary(urhadd_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.urshl: RewriteSimdBinary(urshl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.urshr: RewriteSimdBinary(urshr_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ursqrte: RewriteSimdUnary(ursqrte_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ursra: RewriteSimdBinary(ursra_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ushl: RewriteMaybeSimdBinary(m.Shr, ushl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ushll: RewriteMaybeSimdBinary(m.Shr, ushll_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ushll2: RewriteMaybeSimdBinary(m.Shr, ushll2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.ushr: RewriteMaybeSimdBinary(m.Shr, ushr_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usqadd: RewriteSimdBinary(usqadd_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usra: RewriteSimdBinary(usra_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usubl: RewriteSimdBinary(usubl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usubl2: RewriteSimdBinary(usubl2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usubw: RewriteSimdBinary(usubw_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.usubw2: RewriteSimdBinary(usubw2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uxtb: RewriteUSxt(Domain.UnsignedInt, 8); break;
                    case Mnemonic.uxth: RewriteUSxt(Domain.UnsignedInt, 16); break;
                    case Mnemonic.uxtl: RewriteSimdUnary(uxtl_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uxtl2: RewriteSimdUnary(uxtl2_intrinsic, Domain.UnsignedInt); break;
                    case Mnemonic.uxtw: RewriteUSxt(Domain.UnsignedInt, 32); break;
                    case Mnemonic.uzp1: RewriteSimdBinary(uzp1_intrinsic, Domain.None); break;
                    case Mnemonic.uzp2: RewriteSimdBinary(uzp2_intrinsic, Domain.None); break;
                    case Mnemonic.xtn: RewriteSimdUnary(xtn_intrinsic, Domain.None); break;
                    case Mnemonic.xtn2: RewriteSimdUnary(xtn2_intrinsic, Domain.None); break;
                    case Mnemonic.zip1: RewriteSimdBinary(zip1_intrinsic, Domain.None); break;
                    case Mnemonic.zip2: RewriteSimdBinary(zip2_intrinsic, Domain.None); break;
                    }
                } catch (Exception e)
                {
                    EmitUnitTest(e.Message);
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                cluster.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void NotImplementedYet()
        {
            uint wInstr;
            wInstr = rdr.PeekLeUInt32(-4);
            host.Error(instr.Address, "Rewriting A64 mnemonic '{0}' ({1:X4}) is not supported yet.", instr.Mnemonic, wInstr);
            EmitUnitTest();
            m.Invalid();
        }

        private void EmitUnitTest(string message = "")
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("AArch64Rw", this.instr, instr.Mnemonic.ToString(), rdr, message);
        }

        private Constant ReplicateSimdConstant(VectorRegisterOperand v, int cbitsElement, int iConstOp)
        {
            var imm = ((ImmediateOperand) instr.Operands[iConstOp]).Value.ToUInt64();
            if (instr.ShiftAmount is not null)
            {
                var amount = ((ImmediateOperand) instr.ShiftAmount).Value.ToInt32();
                switch (instr.ShiftCode)
                {
                case Mnemonic.lsl:
                    imm <<= amount;
                    break;
                case Mnemonic.msl:
                    imm <<= amount;
                    imm |= Bits.Mask(0, amount);
                    break;
                default:
                    throw new NotImplementedException($"ExtendSimdConstant {instr.ShiftCode}.");
                }
            }
            int nElements = v.Width.BitSize / cbitsElement;
            var n = new BigInteger(imm);
            var result = BigInteger.Zero;
            for (int i = 0; i < nElements; ++i)
            {
                result = (result << cbitsElement) | n;
            }
            return BigConstant.Create(v.Width, result); 
        }

        //$TODO: prefer this RewriteOp
        private Expression RewriteOp(int iop, bool maybe0 = false) => RewriteOp(instr.Operands[iop], maybe0);

        private Expression RewriteOp(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                if (maybe0 && regOp.Number == 31)
                    return Constant.Zero(regOp.DataType);
                return binder.EnsureRegister(regOp);
            case ImmediateOperand immOp:
                return immOp.Value;
            case AddressOperand addrOp:
                return addrOp.Address;
            case VectorRegisterOperand vectorOp:
                Identifier vreg;
                if (vectorOp.Width.BitSize == 64)
                {
                    vreg = binder.EnsureRegister(Registers.SimdRegs64[vectorOp.VectorRegister.Number - 32]);
                }
                else
                {
                    vreg = binder.EnsureRegister(Registers.SimdRegs128[vectorOp.VectorRegister.Number - 32]);
                }
                if (vectorOp.Index >= 0)
                {
                    // Treat the entire contents of the register as an array.
                    //$BUG Indexing is done little-endian on the CPU, but must be
                    // converted to big-endian to conform with the semantics of of Reko's ArrayAccess.
                    var eType = PrimitiveType.CreateWord(Bitsize(vectorOp.ElementType));
                    int index = vectorOp.Index;
                    return m.ARef(eType, vreg, Constant.Int32(index));
                }
                else
                {
                    return vreg;
                }
            case MemoryOperand mem:
                var ea = binder.EnsureRegister(mem.Base!);
                return m.Mem(mem.Width, ea);
            }
            throw new NotImplementedException($"Rewriting {op.GetType().Name} not implemented yet.");
        }

        private Expression MaybeZeroRegister(RegisterStorage reg, DataType dt)
        {
            if (reg == Registers.GpRegs32[31] ||
                reg == Registers.GpRegs64[31])
            {
                return Constant.Zero(dt);
            }
            else
            {
                return binder.EnsureRegister(reg);
            }
        }
        private Identifier NZCV()
        {
            return binder.EnsureFlagGroup(Registers.NZCV);
        }

        private void NZCV(Expression test)
        {
            var nzcv = NZCV();
            m.Assign(nzcv, test);
        }

        private void NZ00(Expression test)
        {
            var nz = binder.EnsureFlagGroup(Registers.NZ);
            var c = binder.EnsureFlagGroup(Registers.C);
            var v = binder.EnsureFlagGroup(Registers.V);
            m.Assign(nz, test);
            m.Assign(c, Constant.False());
            m.Assign(v, Constant.False());
        }

        Identifier FlagGroup(FlagGroupStorage grf)
        {
            return binder.EnsureFlagGroup(grf);
        }

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException($"ARM condition code {cond} not implemented.");
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(Registers.C));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(Registers.C));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(Registers.Z));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.NZV));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(Registers.NZV));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(Registers.ZC));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(Registers.NZV));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(Registers.ZC));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.NV));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.N));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.N));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(Registers.Z));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(Registers.V));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(Registers.V));
            }
        }

        protected static ArmCondition Invert(ArmCondition cc)
        {
            switch (cc)
            {
            case ArmCondition.EQ: return ArmCondition.NE;
            case ArmCondition.NE: return ArmCondition.EQ;
            case ArmCondition.HS: return ArmCondition.LO;
            case ArmCondition.LO: return ArmCondition.HS;
            case ArmCondition.MI: return ArmCondition.PL;
            case ArmCondition.PL: return ArmCondition.MI;
            case ArmCondition.VS: return ArmCondition.VC;
            case ArmCondition.VC: return ArmCondition.VS;
            case ArmCondition.HI: return ArmCondition.LS;
            case ArmCondition.LS: return ArmCondition.HI;
            case ArmCondition.GE: return ArmCondition.LT;
            case ArmCondition.LT: return ArmCondition.GE;
            case ArmCondition.GT: return ArmCondition.LE;
            case ArmCondition.LE: return ArmCondition.GT;
            case ArmCondition.AL: return ArmCondition.Invalid;
            }
            return ArmCondition.Invalid;
        }

        private record IntrinsicPair(
            IntrinsicProcedure Scalar,
            IntrinsicProcedure Vector);

        private static readonly IntrinsicProcedure abs_intrinsic = IntrinsicBuilder.GenericUnary("__abs");
        private static readonly IntrinsicProcedure addhn_intrinsic = IntrinsicBuilder.GenericBinary("__addhn");
        private static readonly IntrinsicProcedure addhn2_intrinsic = IntrinsicBuilder.GenericBinary("__addhn2");
        private static readonly IntrinsicProcedure addp_intrinsic = IntrinsicBuilder.GenericBinary("__addp");

        private static readonly IntrinsicProcedure bfm_intrinsic = IntrinsicBuilder.Pure("__bfm")
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure bif_intrinsic = IntrinsicBuilder.GenericBinary("__bif");
        private static readonly IntrinsicProcedure bit_intrinsic = IntrinsicBuilder.GenericBinary("__bit");
        private static readonly IntrinsicProcedure brk_intrinsic =
            new IntrinsicBuilder("__brk", true, new Core.Serialization.ProcedureCharacteristics
            {
                Terminates = true,
            })
            .Param(PrimitiveType.UInt16)
            .Void();

        private static readonly IntrinsicProcedure ceil_intrinsic = IntrinsicBuilder.GenericUnary("__ceil");
        private static readonly IntrinsicProcedure cls_intrinsic = IntrinsicBuilder.GenericUnary("__cls");
        private static readonly IntrinsicProcedure cmeq_intrinsic = IntrinsicBuilder.GenericBinary("__cmeq"); 
        private static readonly IntrinsicProcedure cmge_intrinsic = IntrinsicBuilder.GenericBinary("__cmge"); 
        private static readonly IntrinsicProcedure cmgt_intrinsic = IntrinsicBuilder.GenericBinary("__cmgt"); 
        private static readonly IntrinsicProcedure cmhi_intrinsic = IntrinsicBuilder.GenericBinary("__cmhi"); 
        private static readonly IntrinsicProcedure cmhs_intrinsic = IntrinsicBuilder.GenericBinary("__cmhs"); 
        private static readonly IntrinsicProcedure cmle_intrinsic = IntrinsicBuilder.GenericBinary("__cmle"); 
        private static readonly IntrinsicProcedure cmlt_intrinsic = IntrinsicBuilder.GenericBinary("__cmlt"); 
        private static readonly IntrinsicProcedure cmtst_intrinsic = IntrinsicBuilder.GenericBinary("__cmtst");
        private static readonly IntrinsicProcedure cnt_intrinsic = IntrinsicBuilder.GenericUnary("__cnt");
        private static readonly IntrinsicProcedure cvtf_intrinsic = IntrinsicBuilder.GenericUnary("__cvtf");
        private static readonly IntrinsicProcedure cvtf_fixed_intrinsic = IntrinsicBuilder.Pure("__cvtf_fixed")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param(PrimitiveType.Int32)
            .Returns("TDst");

        public static readonly IntrinsicProcedure dup_intrinsic = IntrinsicBuilder.Pure("__dup")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");

        private static readonly IntrinsicProcedure ext_intrinsic = IntrinsicBuilder.Pure("__ext")
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");

        private static readonly IntrinsicProcedure floor_intrinsic = IntrinsicBuilder.GenericUnary("__floor");
        private static readonly IntrinsicProcedure __fmadd = IntrinsicBuilder.GenericTernary("__fmadd");
        private static readonly IntrinsicProcedure __fmsub = IntrinsicBuilder.GenericTernary("__fmsub");
        private static readonly IntrinsicProcedure fmov_intrinsic = IntrinsicBuilder.Pure("__fmov")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure __fnmadd = IntrinsicBuilder.GenericTernary("__fnmadd");
        private static readonly IntrinsicProcedure __fnmsub = IntrinsicBuilder.GenericTernary("__fnmsub");

        private static readonly IntrinsicProcedure load_acquire_intrinsic = new IntrinsicBuilder("__load_acquire", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure load_acquire_exclusive_intrinsic = new IntrinsicBuilder("__load_acquire_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure load_exclusive_intrinsic = new IntrinsicBuilder("__load_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");

        private static readonly IntrinsicProcedure mull_intrinsic = IntrinsicBuilder.Pure("__mull")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");

        private static readonly IntrinsicProcedure nearest_intrinsic = IntrinsicBuilder.GenericUnary("__nearest");
        private static readonly IntrinsicProcedure neg_intrinsic = IntrinsicBuilder.GenericUnary("__neg");

        private static readonly IntrinsicProcedure pmul_intrinsic = IntrinsicBuilder.GenericBinary("__pmul");
        private static readonly IntrinsicProcedure pmull_intrinsic = IntrinsicBuilder.GenericBinary("__pmull");
        private static readonly IntrinsicProcedure pmull2_intrinsic = IntrinsicBuilder.GenericBinary("__pmull2");
        private static readonly IntrinsicProcedure prfm_intrinsic = new IntrinsicBuilder("__prfm", true)
            .GenericTypes("T")
            .Param("T")
            .PtrParam("T")
            .Void();

        private static readonly IntrinsicProcedure raddhn_intrinsic = IntrinsicBuilder.GenericBinary("__raddhn");
        private static readonly IntrinsicProcedure raddhn2_intrinsic = IntrinsicBuilder.GenericBinary("__raddhn2");
        private static readonly IntrinsicProcedure rev16_intrinsic = IntrinsicBuilder.GenericUnary("__rev16");
        private static readonly IntrinsicProcedure rev64_intrinsic = IntrinsicBuilder.GenericUnary("__rev64");
        private static readonly IntrinsicProcedure round_intrinsic = IntrinsicBuilder.GenericUnary("__round");
        private static readonly IntrinsicProcedure rshrn_intrinsic = IntrinsicBuilder.GenericBinary("__rshrn");
        private static readonly IntrinsicProcedure rshrn2_intrinsic = IntrinsicBuilder.GenericBinary("__rshrn2");
        private static readonly IntrinsicProcedure rsubhn_intrinsic = IntrinsicBuilder.Pure("__rsubhn")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure rsubhn2_intrinsic = IntrinsicBuilder.Pure("__rsubhn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure saba_intrinsic = IntrinsicBuilder.GenericBinary("__saba");
        private static readonly IntrinsicProcedure sabal_intrinsic = IntrinsicBuilder.GenericBinary("__sabal");
        private static readonly IntrinsicProcedure sabal2_intrinsic = IntrinsicBuilder.GenericBinary("__sabal2");
        private static readonly IntrinsicProcedure sabd_intrinsic = IntrinsicBuilder.GenericBinary("__sabd");
        private static readonly IntrinsicProcedure sabdl_intrinsic = IntrinsicBuilder.Pure("__sabdl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sabdl2_intrinsic = IntrinsicBuilder.Pure("__sabdl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sadalp_intrinsic = IntrinsicBuilder.GenericBinary("__sadalp");
        private static readonly IntrinsicProcedure saddl_intrinsic = IntrinsicBuilder.Pure("__saddl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure saddl2_intrinsic = IntrinsicBuilder.Pure("__saddl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure saddlp_intrinsic = IntrinsicBuilder.Pure("__saddlp")
            .GenericTypes("TNarrow", "TWide")
            .Params("TNarrow")
            .Returns("TWide");
        private static readonly IntrinsicProcedure saddlv_intrinsic = IntrinsicBuilder.GenericUnary("__saddlv");
        private static readonly IntrinsicProcedure saddw_intrinsic = IntrinsicBuilder.Pure("__saddw")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
        private static readonly IntrinsicProcedure saddw2_intrinsic = IntrinsicBuilder.Pure("__saddw2")
            .GenericTypes("TWide", "TNarrow")
            .Params("TWide")
            .Params("TNarrow")
            .Returns("TWide");
        private static readonly IntrinsicProcedure __sha1c = new IntrinsicBuilder("__sha1c", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word128);
        private static readonly IntrinsicProcedure shadd_intrinsic = IntrinsicBuilder.GenericBinary("__shadd");
        private static readonly IntrinsicProcedure shl_intrinsic = IntrinsicBuilder.GenericBinary("__shl");
        private static readonly IntrinsicProcedure shll_intrinsic = IntrinsicBuilder.GenericBinary("__shll");
        private static readonly IntrinsicProcedure shll2_intrinsic = IntrinsicBuilder.GenericBinary("__shll2");
        private static readonly IntrinsicProcedure shrn_intrinsic = IntrinsicBuilder.Pure("__shrn")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure shsub_intrinsic = IntrinsicBuilder.GenericBinary("__shsub");
        private static readonly IntrinsicProcedure sli_intrinsic = IntrinsicBuilder.GenericBinary("__sli");
        private static readonly IntrinsicProcedure smax_intrinsic = IntrinsicBuilder.GenericBinary("__smax");
        private static readonly IntrinsicProcedure smaxp_intrinsic = IntrinsicBuilder.GenericBinary("__smaxp");
        private static readonly IntrinsicProcedure smaxv_intrinsic = IntrinsicBuilder.Pure("__smaxv")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure smc_intrinsic =
            new IntrinsicBuilder("__secure_monitor_call", true)
            .Param(PrimitiveType.UInt16)
            .Void();
        private static readonly IntrinsicProcedure smin_intrinsic = IntrinsicBuilder.GenericBinary("__smin");
        private static readonly IntrinsicProcedure sminp_intrinsic = IntrinsicBuilder.GenericBinary("__sminp");
        private static readonly IntrinsicProcedure sminv_intrinsic = IntrinsicBuilder.GenericUnary("__sminv");
        private static readonly IntrinsicProcedure smull2_intrinsic = IntrinsicBuilder.GenericBinary("__smull2");
        private static readonly IntrinsicProcedure sqabs_intrinsic = IntrinsicBuilder.GenericUnary("__sqabs");
        private static readonly IntrinsicProcedure sqadd_intrinsic = IntrinsicBuilder.GenericBinary("__sqadd");
        private static readonly IntrinsicPair sqdmulh_intrinsic = new(
            IntrinsicBuilder.GenericBinary("__sqdmulh"),
            IntrinsicBuilder.GenericBinary("__sqdmulh_vec"));
        private static readonly IntrinsicProcedure sqdmull_intrinsic = IntrinsicBuilder.Pure("__sqdmull")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sqdmull2_intrinsic = IntrinsicBuilder.Pure("__sqdmull2")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc")
            .Params("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sqneg_intrinsic = IntrinsicBuilder.GenericUnary("__sqneg");
        private static readonly IntrinsicProcedure sqrdmulh_intrinsic = IntrinsicBuilder.GenericBinary("__sqrdmulh");
        private static readonly IntrinsicProcedure sqrshl_intrinsic = IntrinsicBuilder.GenericBinary("__sqrshl");
        private static readonly IntrinsicProcedure sqrshrn_intrinsic = IntrinsicBuilder.GenericBinary("__sqrshrn");
        private static readonly IntrinsicProcedure sqrshrn2_intrinsic = IntrinsicBuilder.GenericBinary("__sqrshrn2");
        private static readonly IntrinsicProcedure sqrshrun_intrinsic = IntrinsicBuilder.GenericBinary("__sqrshrun");
        private static readonly IntrinsicProcedure sqrshrun2_intrinsic = IntrinsicBuilder.GenericBinary("__sqrshrun2");
        private static readonly IntrinsicProcedure sqshl_intrinsic = IntrinsicBuilder.GenericBinary("__sqshl");
        private static readonly IntrinsicProcedure sqshlu_intrinsic = IntrinsicBuilder.GenericBinary("__sqshlu");
        private static readonly IntrinsicProcedure sqshrn_intrinsic = IntrinsicBuilder.GenericBinary("__sqshrn");
        private static readonly IntrinsicProcedure sqshrn2_intrinsic = IntrinsicBuilder.GenericBinary("__sqshrn2");
        private static readonly IntrinsicProcedure sqsub_intrinsic = IntrinsicBuilder.GenericBinary("__sqsub");
        private static readonly IntrinsicProcedure sqxtn_intrinsic = IntrinsicBuilder.Pure("__sqxtn")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sqxtn2_intrinsic = IntrinsicBuilder.Pure("__sqxtn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sqxtun_intrinsic = IntrinsicBuilder.Pure("__sqxtun")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sqxtun2_intrinsic = IntrinsicBuilder.Pure("__sqxtun2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sri_intrinsic = IntrinsicBuilder.GenericBinary("__sri");
        private static readonly IntrinsicProcedure srhadd_intrinsic = IntrinsicBuilder.GenericBinary("__srhadd");
        private static readonly IntrinsicProcedure srshl_intrinsic = IntrinsicBuilder.GenericBinary("__srshl");
        private static readonly IntrinsicProcedure srshr_intrinsic = IntrinsicBuilder.GenericBinary("__srshr");
        private static readonly IntrinsicProcedure srsra_intrinsic = IntrinsicBuilder.GenericBinary("__srsra");
        private static readonly IntrinsicProcedure sshl_intrinsic = IntrinsicBuilder.Pure("__sshl")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure sshll_intrinsic = IntrinsicBuilder.GenericBinary("__sshll");
        private static readonly IntrinsicProcedure sshll2_intrinsic = IntrinsicBuilder.GenericBinary("__sshll2");
        private static readonly IntrinsicProcedure sshr_intrinsic = IntrinsicBuilder.Pure("__sshr")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure ssra_intrinsic = IntrinsicBuilder.Pure("__ssra")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure ssubl_intrinsic = IntrinsicBuilder.GenericBinary("__ssubl");
        private static readonly IntrinsicProcedure ssubl2_intrinsic = IntrinsicBuilder.GenericBinary("__ssubl2");
        private static readonly IntrinsicProcedure ssubw_intrinsic = IntrinsicBuilder.GenericBinary("__ssubw");
        private static readonly IntrinsicProcedure ssubw2_intrinsic = IntrinsicBuilder.GenericBinary("__ssubw2");
        private static readonly IntrinsicProcedure subhn_intrinsic = IntrinsicBuilder.GenericBinary("__subhn");
        private static readonly IntrinsicProcedure subhn2_intrinsic = IntrinsicBuilder.Pure("__subhn2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure sum_intrinsic = IntrinsicBuilder.Pure("__sum")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure suqadd_intrinsic = IntrinsicBuilder.GenericBinary("__suqadd");

        private static readonly IntrinsicProcedure svc_intrinsic = new IntrinsicBuilder("__supervisor_call", true)
            .Param(PrimitiveType.UInt16)
            .Void();
        private static readonly IntrinsicProcedure sxtl_intrinsic = IntrinsicBuilder.Pure("__sxtl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");

        private static readonly IntrinsicProcedure trn1_intrinsic = IntrinsicBuilder.GenericBinary("__trn1");
        private static readonly IntrinsicProcedure trn2_intrinsic = IntrinsicBuilder.GenericBinary("__trn2");
        private static readonly IntrinsicProcedure trunc_intrinsic = IntrinsicBuilder.GenericUnary("__trunc");

        private static readonly IntrinsicProcedure uaba_intrinsic = IntrinsicBuilder.GenericBinary("__uaba");
        private static readonly IntrinsicProcedure uabal_intrinsic = IntrinsicBuilder.Pure("__uabal")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure uabal2_intrinsic = IntrinsicBuilder.Pure("__uabal2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure uabd_intrinsic = IntrinsicBuilder.GenericBinary("__uabd");
        private static readonly IntrinsicProcedure uabdl_intrinsic = IntrinsicBuilder.GenericBinary("__uabdl");
        private static readonly IntrinsicProcedure uabdl2_intrinsic = IntrinsicBuilder.GenericBinary("__uabdl2");
        private static readonly IntrinsicProcedure uadalp_intrinsic = IntrinsicBuilder.GenericBinary("__uadalp");
        private static readonly IntrinsicProcedure uaddl_intrinsic = IntrinsicBuilder.Pure("__uaddl")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure uaddl2_intrinsic = IntrinsicBuilder.Pure("__uaddl2")
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure uaddlp_intrinsic = IntrinsicBuilder.GenericBinary("__uaddlp");
        private static readonly IntrinsicProcedure uaddlv_intrinsic = IntrinsicBuilder.GenericUnary("__uaddlv");
        private static readonly IntrinsicProcedure uaddw_intrinsic = IntrinsicBuilder.GenericBinary("__uaddw");
        private static readonly IntrinsicProcedure uhadd_intrinsic = IntrinsicBuilder.GenericBinary("__uhadd");
        private static readonly IntrinsicProcedure uhsub_intrinsic = IntrinsicBuilder.GenericBinary("__uhsub");
        private static readonly IntrinsicProcedure umax_intrinsic = IntrinsicBuilder.GenericBinary("__umax");
        private static readonly IntrinsicProcedure umaxp_intrinsic = IntrinsicBuilder.GenericBinary("__umaxp");
        private static readonly IntrinsicProcedure umaxv_intrinsic = IntrinsicBuilder.GenericUnary("__umaxv");
        private static readonly IntrinsicProcedure umin_intrinsic = IntrinsicBuilder.GenericBinary("__umin");
        private static readonly IntrinsicProcedure uminp_intrinsic = IntrinsicBuilder.GenericBinary("__uminp");
        private static readonly IntrinsicProcedure uminv_intrinsic = IntrinsicBuilder.GenericUnary("__uminv");
        private static readonly IntrinsicProcedure umull2_intrinsic = IntrinsicBuilder.GenericBinary("__umull2");
        private static readonly IntrinsicProcedure uqadd_intrinsic = IntrinsicBuilder.GenericBinary("__uqadd");
        private static readonly IntrinsicProcedure uqrshl_intrinsic = IntrinsicBuilder.GenericBinary("__uqrshl");
        private static readonly IntrinsicProcedure uqrshrn_intrinsic = IntrinsicBuilder.GenericBinary("__uqrshrn");
        private static readonly IntrinsicProcedure uqrshrn2_intrinsic = IntrinsicBuilder.GenericBinary("__uqrshrn2");
        private static readonly IntrinsicProcedure uqshl_intrinsic = IntrinsicBuilder.GenericBinary("__uqshl");
        private static readonly IntrinsicProcedure uqshrn_intrinsic = IntrinsicBuilder.GenericBinary("__uqshrn");
        private static readonly IntrinsicProcedure uqshrn2_intrinsic = IntrinsicBuilder.GenericBinary("__uqshrn2");
        private static readonly IntrinsicProcedure uqsub_intrinsic = IntrinsicBuilder.GenericBinary("__uqsub");
        private static readonly IntrinsicProcedure uqxtn_intrinsic = IntrinsicBuilder.GenericUnary("__uqxtn");
        private static readonly IntrinsicProcedure uqxtn2_intrinsic = IntrinsicBuilder.GenericUnary("__uqxtn2");
        private static readonly IntrinsicProcedure urecpe_intrinsic = IntrinsicBuilder.GenericUnary("__urecpe");
        private static readonly IntrinsicProcedure urhadd_intrinsic = IntrinsicBuilder.GenericBinary("__urhadd");
        private static readonly IntrinsicProcedure urshl_intrinsic = IntrinsicBuilder.GenericBinary("__urshl");
        private static readonly IntrinsicProcedure urshr_intrinsic = IntrinsicBuilder.GenericBinary("__urshr");
        private static readonly IntrinsicProcedure ursqrte_intrinsic = IntrinsicBuilder.GenericUnary("__ursqrte");
        private static readonly IntrinsicProcedure ursra_intrinsic = IntrinsicBuilder.GenericBinary("__ursra");
        private static readonly IntrinsicProcedure ushl_intrinsic = IntrinsicBuilder.GenericBinary("__ushl");
        private static readonly IntrinsicProcedure ushll_intrinsic = IntrinsicBuilder.GenericBinary("__ushll");
        private static readonly IntrinsicProcedure ushll2_intrinsic = IntrinsicBuilder.GenericBinary("__ushll2");
        private static readonly IntrinsicProcedure ushr_intrinsic = IntrinsicBuilder.GenericBinary("__ushr");
        private static readonly IntrinsicProcedure usqadd_intrinsic = IntrinsicBuilder.GenericBinary("__usqadd");
        private static readonly IntrinsicProcedure usra_intrinsic = IntrinsicBuilder.GenericBinary("__usra");
        private static readonly IntrinsicProcedure usubl_intrinsic = IntrinsicBuilder.GenericBinary("__usubl");
        private static readonly IntrinsicProcedure usubl2_intrinsic = IntrinsicBuilder.GenericBinary("__usubl2");
        private static readonly IntrinsicProcedure usubw_intrinsic = IntrinsicBuilder.GenericBinary("__usubw");
        private static readonly IntrinsicProcedure usubw2_intrinsic = IntrinsicBuilder.GenericBinary("__usubw2");
        private static readonly IntrinsicProcedure uxtl_intrinsic = IntrinsicBuilder.GenericUnary("__uxtl");
        private static readonly IntrinsicProcedure uxtl2_intrinsic = IntrinsicBuilder.GenericUnary("__uxtl2");
        private static readonly IntrinsicProcedure uzp1_intrinsic = IntrinsicBuilder.GenericBinary("__uzp1");
        private static readonly IntrinsicProcedure uzp2_intrinsic = IntrinsicBuilder.GenericBinary("__uzp2");

        private static readonly IntrinsicProcedure xtn_intrinsic = IntrinsicBuilder.GenericUnary("__xtn");
        private static readonly IntrinsicProcedure xtn2_intrinsic = IntrinsicBuilder.GenericUnary("__xtn2");

        private static readonly IntrinsicProcedure zip1_intrinsic = IntrinsicBuilder.GenericBinary("__zip1");
        private static readonly IntrinsicProcedure zip2_intrinsic = IntrinsicBuilder.GenericBinary("__zip2");
    }
}