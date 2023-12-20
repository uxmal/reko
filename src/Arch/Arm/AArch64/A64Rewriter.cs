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
        private static Intrinsics intrinsic = new();

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
                    case Mnemonic.abs: RewriteSimdUnary(Simd.Abs, Domain.SignedInt); break;
                    case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
                    case Mnemonic.adcs: RewriteAdcSbc(m.IAdd, this.NZCV); break;
                    case Mnemonic.add: RewriteMaybeSimdBinary(m.IAdd, Simd.Add); break;
                    case Mnemonic.addhn: RewriteSimdBinaryNarrow(intrinsic.addhn, Domain.None); break;
                    case Mnemonic.addhn2: RewriteAddhn2(); break;
                    case Mnemonic.addp: RewriteSimdBinary(intrinsic.addp, Domain.None); break;
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
                    case Mnemonic.bif: RewriteSimdBinary(intrinsic.bif, Domain.None); break;
                    case Mnemonic.bit: RewriteSimdBinary(intrinsic.bit, Domain.None); break;
                    case Mnemonic.bl: RewriteBl(); break;
                    case Mnemonic.blr: RewriteBlr(); break;
                    case Mnemonic.br: RewriteBr(); break;
                    case Mnemonic.brk: RewriteBrk(); break;
                    case Mnemonic.bsl: RewriteBsl(); break;
                    case Mnemonic.cbnz: RewriteCb(m.Ne0); break;
                    case Mnemonic.cbz: RewriteCb(m.Eq0); break;
                    case Mnemonic.ccmn: RewriteCcmn(); break;
                    case Mnemonic.ccmp: RewriteCcmp(); break;
                    case Mnemonic.cls: RewriteSimdUnary(intrinsic.cls, Domain.None); break;
                    case Mnemonic.clz: RewriteClz(); break;
                    case Mnemonic.cmp: RewriteCmp(); break;
                    case Mnemonic.cmeq: RewriteCm(intrinsic.cmeq, Domain.None); break;
                    case Mnemonic.cmge: RewriteCm(intrinsic.cmge, Domain.SignedInt); break;
                    case Mnemonic.cmgt: RewriteCm(intrinsic.cmgt, Domain.SignedInt); break;
                    case Mnemonic.cmhi: RewriteCm(intrinsic.cmhi, Domain.UnsignedInt); break;
                    case Mnemonic.cmhs: RewriteCm(intrinsic.cmhs, Domain.UnsignedInt); break;
                    case Mnemonic.cmle: RewriteCm(intrinsic.cmle, Domain.SignedInt); break;
                    case Mnemonic.cmlt: RewriteCm(intrinsic.cmlt, Domain.SignedInt); break;
                    case Mnemonic.cmtst: RewriteCm(intrinsic.cmtst, Domain.None); break;
                    case Mnemonic.cnt: RewriteSimdUnary(intrinsic.cnt, Domain.None); break;
                    case Mnemonic.csel: RewriteCsel(); break;
                    case Mnemonic.csinc: RewriteCsinc(); break;
                    case Mnemonic.csinv: RewriteCsinv(); break;
                    case Mnemonic.csneg: RewriteCsneg(); break;
                    case Mnemonic.dmb: RewriteDmb(); break;
                    case Mnemonic.dsb: RewriteDsb(); break;
                    case Mnemonic.dup: RewriteSimdUnaryWithScalar(intrinsic.dup, Domain.None); break;
                    case Mnemonic.eor: RewriteBinary(m.Xor); break;
                    case Mnemonic.eon: RewriteBinary((a, b) => m.Xor(a, m.Comp(b))); break;
                    case Mnemonic.eret: RewriteEret(); break;
                    case Mnemonic.ext: RewriteSimdTernaryWithScalar(intrinsic.ext, Domain.None); break;
                    case Mnemonic.extr: RewriteExtr(); break;
                    case Mnemonic.fabs: RewriteFabs(); break;
                    case Mnemonic.fadd: RewriteMaybeSimdBinary(m.FAdd, Simd.FAdd, Domain.Real); break;
                    case Mnemonic.fcmp: RewriteFcmp(); break;
                    case Mnemonic.fcmpe: RewriteFcmp(); break;  //$REVIEW: this leaves out the 'e'xception part. 
                    case Mnemonic.fcsel: RewriteFcsel(); break;
                    case Mnemonic.fcvt: RewriteFcvt(); break;
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
                    case Mnemonic.fmadd: RewriteIntrinsicFTernary(intrinsic.fmadd); break;
                    case Mnemonic.fmsub: RewriteIntrinsicFTernary(intrinsic.fmsub); break;
                    case Mnemonic.fmax: RewriteIntrinsicFBinary(FpOps.fmaxf, FpOps.fmax, Simd.Max); break;
                    case Mnemonic.fmin: RewriteIntrinsicFBinary(FpOps.fminf, FpOps.fmin, Simd.Min); break;
                    case Mnemonic.fmov: RewriteFmov(); break;
                    case Mnemonic.fmul: RewriteFmul(); break;
                    case Mnemonic.fneg: RewriteUnary(m.FNeg); break;
                    case Mnemonic.fnmadd: RewriteIntrinsicFTernary(intrinsic.fnmadd); break;
                    case Mnemonic.fnmsub: RewriteIntrinsicFTernary(intrinsic.fnmsub); break;
                    case Mnemonic.fnmul: RewriteFnmul(); break;
                    case Mnemonic.fsqrt: RewriteFsqrt(); break;
                    case Mnemonic.fsub: RewriteMaybeSimdBinary(m.FSub, Simd.FSub, Domain.Real); break;
                    case Mnemonic.hlt: RewriteHlt(); break;
                    case Mnemonic.isb: RewriteIsb(); break;
                    case Mnemonic.ld1: RewriteLdN(intrinsic.ld1); break;
                    case Mnemonic.ld1r: RewriteLdNr(intrinsic.ld1r); break;
                    case Mnemonic.ld2: RewriteLdN(intrinsic.ld2); break;
                    case Mnemonic.ld2r: RewriteLdN(intrinsic.ld2r); break;
                    case Mnemonic.ld3: RewriteLdN(intrinsic.ld3); break;
                    case Mnemonic.ld3r: RewriteLdN(intrinsic.ld3r); break;
                    case Mnemonic.ld4: RewriteLdN(intrinsic.ld4); break;
                    case Mnemonic.ld4r: RewriteLdN(intrinsic.ld4r); break;
                    case Mnemonic.ldnp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldarh: RewriteLoadAcquire(intrinsic.load_acquire, PrimitiveType.Word16); break;
                    case Mnemonic.ldaxrh: RewriteLoadAcquire(intrinsic.load_acquire_exclusive, PrimitiveType.Word16); break;
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
                    case Mnemonic.mla: RewriteMla(intrinsic.mla, intrinsic.mla_by_element, Domain.None); break;
                    case Mnemonic.mls: RewriteMla(intrinsic.mls, intrinsic.mls_by_element, Domain.None); break;
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
                    case Mnemonic.neg: RewriteSimdUnary(intrinsic.neg, Domain.SignedInt); break;
                    case Mnemonic.nop: m.Nop(); break;
                    case Mnemonic.not: RewriteMaybeSimdUnary(m.Comp, Simd.Not); break;
                    case Mnemonic.orr: RewriteLogical(m.Or); break;
                    case Mnemonic.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                    case Mnemonic.pmul: RewriteSimdBinary(intrinsic.pmul, Domain.None); break;
                    case Mnemonic.pmull: RewriteSimdBinary(intrinsic.pmull, Domain.None); break;
                    case Mnemonic.pmull2: RewriteSimdBinary(intrinsic.pmull2, Domain.None); break;
                    case Mnemonic.prfm: RewritePrfm(); break;
                    case Mnemonic.raddhn: RewriteSimdBinary(intrinsic.raddhn, Domain.None); break;
                    case Mnemonic.raddhn2: RewriteSimdBinary(intrinsic.raddhn2, Domain.None); break;
                    case Mnemonic.rbit: RewriteRbit(); break;
                    case Mnemonic.ret: RewriteRet(); break;
                    case Mnemonic.rev: RewriteRev(); break;
                    case Mnemonic.rev16: RewriteRev16(); break;
                    case Mnemonic.rev32: RewriteRev32(); break;
                    case Mnemonic.rev64: RewriteSimdUnary(intrinsic.rev64, Domain.None); break;
                    case Mnemonic.ror: RewriteRor(); break;
                    case Mnemonic.rorv: RewriteRor(); break;
                    case Mnemonic.rshrn: RewriteSimdBinary(intrinsic.rshrn, Domain.None); break;
                    case Mnemonic.rshrn2: RewriteSimdBinary(intrinsic.rshrn2, Domain.None); break;
                    case Mnemonic.rsubhn: RewriteSimdBinaryWiden(intrinsic.rsubhn, Domain.None); break;
                    case Mnemonic.rsubhn2: RewriteSimdBinaryWiden(intrinsic.rsubhn2, Domain.None); break;
                    case Mnemonic.saba: RewriteSimdBinary(intrinsic.saba, Domain.None); break;
                    case Mnemonic.sabal: RewriteSimdBinary(intrinsic.sabal, Domain.None); break;
                    case Mnemonic.sabal2: RewriteSimdBinary(intrinsic.sabal2, Domain.None); break;
                    case Mnemonic.sabd: RewriteSimdBinary(intrinsic.sabd, Domain.None); break;
                    case Mnemonic.sabdl: RewriteSimdBinaryWiden(intrinsic.sabdl, Domain.None); break;
                    case Mnemonic.sabdl2: RewriteSimdBinaryWiden(intrinsic.sabdl2, Domain.None); break;
                    case Mnemonic.sadalp: RewriteSimdBinary(intrinsic.sadalp, Domain.SignedInt); break;
                    case Mnemonic.saddl: RewriteSimdBinaryWiden(intrinsic.saddl, Domain.SignedInt); break;
                    case Mnemonic.saddl2: RewriteSimdBinaryWiden(intrinsic.saddl2, Domain.SignedInt); break;
                    case Mnemonic.saddlp: RewriteSimdUnaryChangeSize(intrinsic.saddlp, Domain.SignedInt); break;
                    case Mnemonic.saddlv: RewriteSimdUnary(intrinsic.saddlv, Domain.SignedInt); break;
                    case Mnemonic.saddw: RewriteSimdBinaryWideNarrow(intrinsic.saddw, Domain.SignedInt); break;
                    case Mnemonic.saddw2: RewriteSimdBinaryWideNarrow(intrinsic.saddw2, Domain.SignedInt); break;
                    case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
                    case Mnemonic.sbcs: RewriteAdcSbc(m.ISub, NZCV); break;
                    case Mnemonic.sbfiz: RewriteSbfiz(); break;
                    case Mnemonic.sbfm: RewriteUSbfm(intrinsic.sbfm); break;
                    case Mnemonic.scvtf: RewriteIcvtf("s", Domain.SignedInt); break;
                    case Mnemonic.sdiv: RewriteBinary(m.SDiv); break;
                    case Mnemonic.shadd: RewriteSimdBinary(intrinsic.shadd, Domain.SignedInt); break;
                    case Mnemonic.sha1c: RewriteSha1c(); break;
                    case Mnemonic.shl: RewriteSimdBinary(intrinsic.shl, Domain.None); break;
                    case Mnemonic.shll: RewriteSimdBinary(intrinsic.shll, Domain.None); break;
                    case Mnemonic.shll2: RewriteSimdBinary(intrinsic.shll2, Domain.None); break;
                    case Mnemonic.shrn: RewriteSimdBinaryWithScalar(intrinsic.shrn, Domain.None); break;
                    case Mnemonic.shsub: RewriteSimdBinary(intrinsic.shsub, Domain.SignedInt); break;
                    case Mnemonic.sli: RewriteSimdBinary(intrinsic.sli, Domain.None); break;
                    case Mnemonic.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smax: RewriteSimdBinary(intrinsic.smax, Domain.SignedInt); break;
                    case Mnemonic.smaxp: RewriteSimdBinary(intrinsic.smaxp, Domain.SignedInt); break;
                    case Mnemonic.smaxv: RewriteSimdUnaryReduce(intrinsic.smaxv, Domain.SignedInt); break;
                    case Mnemonic.smc: RewriteSmc(); break;
                    case Mnemonic.smin: RewriteSimdBinary(intrinsic.smin, Domain.SignedInt); break;
                    case Mnemonic.sminp: RewriteSimdBinary(intrinsic.sminp, Domain.SignedInt); break;
                    case Mnemonic.sminv: RewriteSimdUnary(intrinsic.sminv, Domain.SignedInt); break;
                    case Mnemonic.smlal: RewriteSimdTernaryWiden(intrinsic.smlal, Domain.SignedInt); break;
                    case Mnemonic.smlal2: RewriteSimdTernaryWiden(intrinsic.smlal2, Domain.SignedInt); break;
                    case Mnemonic.smlsl: RewriteSimdTernaryWiden(intrinsic.smlsl, Domain.SignedInt); break;
                    case Mnemonic.smlsl2: RewriteSimdTernaryWiden(intrinsic.smlsl2, Domain.SignedInt); break;
                    case Mnemonic.smov: RewriteVectorElementToScalar(Domain.SignedInt); break;
                    case Mnemonic.smsubl: RewriteSmsubl(); break;
                    case Mnemonic.smull: RewriteMull(PrimitiveType.Int32, PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smull2: RewriteSimdBinary(intrinsic.smull2, Domain.SignedInt); break;
                    case Mnemonic.smulh: RewriteMulh(PrimitiveType.Int64, PrimitiveType.Int128, m.SMul); break;
                    case Mnemonic.sqabs: RewriteSimdUnary(intrinsic.sqabs, Domain.SignedInt); break;
                    case Mnemonic.sqadd: RewriteSimdBinary(intrinsic.sqadd, Domain.SignedInt); break;
                    case Mnemonic.sqdmulh: RewriteMaybeSimdBinary(intrinsic.sqdmulh, Domain.SignedInt); break;
                    case Mnemonic.sqdmull: RewriteSqdmull(intrinsic.sqdmull); break;
                    case Mnemonic.sqdmull2: RewriteSqdmull(intrinsic.sqdmull2); break;
                    case Mnemonic.sqdmlal: RewriteTernary(intrinsic.sqdmlal, Domain.SignedInt); break;
                    case Mnemonic.sqdmlal2: RewriteTernary(intrinsic.sqdmlal2, Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl: RewriteTernary(intrinsic.sqdmlsl, Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl2: RewriteTernary(intrinsic.sqdmlsl2, Domain.SignedInt); break;
                    case Mnemonic.sqneg: RewriteSimdUnary(intrinsic.sqneg, Domain.SignedInt); break;
                    case Mnemonic.sqrdmlah: RewriteTernary(intrinsic.sqrdmlah, Domain.SignedInt); break;
                    case Mnemonic.sqrdmlsh: RewriteTernary(intrinsic.sqrdmlsh, Domain.SignedInt); break;
                    case Mnemonic.sqrdmulh: RewriteBinary(intrinsic.sqrdmulh); break;
                    case Mnemonic.sqrshl: RewriteSimdBinary(intrinsic.sqrshl, Domain.SignedInt); break;
                    case Mnemonic.sqrshrn: RewriteSimdBinary(intrinsic.sqrshrn, Domain.SignedInt); break;
                    case Mnemonic.sqrshrn2: RewriteSimdBinary(intrinsic.sqrshrn2, Domain.SignedInt); break;
                    case Mnemonic.sqrshrun: RewriteSimdBinary(intrinsic.sqrshrun, Domain.SignedInt); break;
                    case Mnemonic.sqrshrun2: RewriteSimdBinary(intrinsic.sqrshrun2, Domain.SignedInt); break;
                    case Mnemonic.sqshl: RewriteSimdBinary(intrinsic.sqshl, Domain.SignedInt); break;
                    case Mnemonic.sqshlu: RewriteSimdBinary(intrinsic.sqshlu, Domain.SignedInt); break;
                    case Mnemonic.sqshrn: RewriteSimdBinary(intrinsic.sqshrn, Domain.SignedInt); break;
                    case Mnemonic.sqshrn2: RewriteSimdBinary(intrinsic.sqshrn2, Domain.SignedInt); break;
                    case Mnemonic.sqsub: RewriteSimdBinary(intrinsic.sqsub, Domain.SignedInt); break;
                    case Mnemonic.sqxtn: RewriteSimdUnaryChangeSize(intrinsic.sqxtn, Domain.SignedInt); break;
                    case Mnemonic.sqxtn2: RewriteSimdUnaryChangeSize(intrinsic.sqxtn2, Domain.SignedInt); break;
                    case Mnemonic.sqxtun: RewriteSimdUnaryChangeSize(intrinsic.sqxtun, Domain.SignedInt); break;
                    case Mnemonic.sqxtun2: RewriteSimdUnaryChangeSize(intrinsic.sqxtun2, Domain.SignedInt); break;
                    case Mnemonic.sri: RewriteSimdBinary(intrinsic.sri, Domain.SignedInt); break;
                    case Mnemonic.srhadd: RewriteSimdBinary(intrinsic.srhadd, Domain.SignedInt); break;
                    case Mnemonic.srshl: RewriteSimdBinary(intrinsic.srshl, Domain.SignedInt); break;
                    case Mnemonic.srshr: RewriteSimdBinary(intrinsic.srshr, Domain.SignedInt); break;
                    case Mnemonic.srsra: RewriteSimdBinary(intrinsic.srsra, Domain.SignedInt); break;
                    case Mnemonic.sshl: RewriteSimdBinaryWithScalar(intrinsic.sshl, Domain.SignedInt); break;
                    case Mnemonic.sshll: RewriteSimdBinary(intrinsic.sshll, Domain.SignedInt); break;
                    case Mnemonic.sshll2: RewriteSimdBinary(intrinsic.sshll2, Domain.SignedInt); break;
                    case Mnemonic.sshr: RewriteSimdBinaryWithScalar(intrinsic.sshr, Domain.SignedInt); break;
                    case Mnemonic.ssra: RewriteSimdBinaryWithScalar(intrinsic.ssra, Domain.SignedInt); break;
                    case Mnemonic.ssubl: RewriteSimdBinary(intrinsic.ssubl, Domain.SignedInt); break;
                    case Mnemonic.ssubl2: RewriteSimdBinary(intrinsic.ssubl2, Domain.SignedInt); break;
                    case Mnemonic.ssubw: RewriteSimdBinary(intrinsic.ssubw, Domain.SignedInt); break;
                    case Mnemonic.ssubw2: RewriteSimdBinary(intrinsic.ssubw2, Domain.SignedInt); break;
                    case Mnemonic.st1: RewriteStN(intrinsic.st1); break;
                    case Mnemonic.st2: RewriteStN(intrinsic.st2); break;
                    case Mnemonic.st3: RewriteStN(intrinsic.st3); break;
                    case Mnemonic.st4: RewriteStN(intrinsic.st4); break;
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
                    case Mnemonic.subhn: RewriteSimdBinary(intrinsic.subhn, Domain.SignedInt); break;
                    case Mnemonic.subhn2: RewriteSimdBinaryWiden(intrinsic.subhn2, Domain.SignedInt); break;
                    case Mnemonic.suqadd: RewriteSimdBinary(intrinsic.suqadd, Domain.SignedInt); break;
                    case Mnemonic.subs: RewriteBinary(m.ISub, NZCV); break;
                    case Mnemonic.svc: RewriteSvc(); break;
                    case Mnemonic.sxtb: RewriteUSxt(Domain.SignedInt, 8); break;
                    case Mnemonic.sxth: RewriteUSxt(Domain.SignedInt, 16); break;
                    case Mnemonic.sxtl: RewriteSimdUnaryChangeSize(intrinsic.sxtl, Domain.SignedInt); break;
                    case Mnemonic.sxtw: RewriteUSxt(Domain.SignedInt, 32); break;
                    case Mnemonic.tbnz: RewriteTb(m.Ne0); break;
                    case Mnemonic.tbl: RewriteTbl(); break;
                    case Mnemonic.tbx: RewriteTbx(); break;
                    case Mnemonic.tbz: RewriteTb(m.Eq0); break;
                    case Mnemonic.test: RewriteTest(); break;
                    case Mnemonic.trn1: RewriteSimdBinary(intrinsic.trn1, Domain.UnsignedInt); break;
                    case Mnemonic.trn2: RewriteSimdBinary(intrinsic.trn2, Domain.UnsignedInt); break;
                    case Mnemonic.uaba: RewriteSimdBinary(intrinsic.uaba, Domain.UnsignedInt); break;
                    case Mnemonic.uabal: RewriteSimdBinaryWiden(intrinsic.uabal, Domain.UnsignedInt); break;
                    case Mnemonic.uabal2: RewriteSimdBinaryWiden(intrinsic.uabal2, Domain.UnsignedInt); break;
                    case Mnemonic.uabd: RewriteSimdBinary(intrinsic.uabd, Domain.UnsignedInt); break;
                    case Mnemonic.uabdl: RewriteSimdBinary(intrinsic.uabdl, Domain.UnsignedInt); break;
                    case Mnemonic.uabdl2: RewriteSimdBinary(intrinsic.uabdl2, Domain.UnsignedInt); break;
                    case Mnemonic.uadalp: RewriteSimdBinary(intrinsic.uadalp, Domain.UnsignedInt); break;
                    case Mnemonic.uaddl: RewriteSimdBinaryWiden(intrinsic.uaddl, Domain.UnsignedInt); break;
                    case Mnemonic.uaddl2: RewriteSimdBinaryWiden(intrinsic.uaddl2, Domain.UnsignedInt); break;
                    case Mnemonic.uaddlp: RewriteSimdBinary(intrinsic.uaddlp, Domain.UnsignedInt); break;
                    case Mnemonic.uaddlv: RewriteSimdUnary(intrinsic.uaddlv, Domain.UnsignedInt); break;
                    case Mnemonic.uaddw: RewriteSimdBinaryWideNarrow(intrinsic.uaddw, Domain.UnsignedInt); break;
                    case Mnemonic.uaddw2: RewriteSimdBinaryWideNarrow(intrinsic.uaddw2, Domain.UnsignedInt); break;
                    case Mnemonic.ubfm: RewriteUSbfm(intrinsic.ubfm); break;
                    case Mnemonic.ucvtf: RewriteIcvtf("u", Domain.UnsignedInt); break;
                    case Mnemonic.udiv: RewriteBinary(m.UDiv); break;
                    case Mnemonic.uhadd: RewriteSimdBinary(intrinsic.uhadd, Domain.UnsignedInt); break;
                    case Mnemonic.uhsub: RewriteSimdBinary(intrinsic.uhsub, Domain.UnsignedInt); break;
                    case Mnemonic.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umax: RewriteSimdBinary(intrinsic.umax, Domain.UnsignedInt); break;
                    case Mnemonic.umaxp: RewriteSimdBinary(intrinsic.umaxp, Domain.UnsignedInt); break;
                    case Mnemonic.umaxv: RewriteSimdUnary(intrinsic.umaxv, Domain.UnsignedInt); break;
                    case Mnemonic.umin: RewriteSimdBinary(intrinsic.umin, Domain.UnsignedInt); break;
                    case Mnemonic.uminp: RewriteSimdBinary(intrinsic.uminp, Domain.UnsignedInt); break;
                    case Mnemonic.uminv: RewriteSimdUnary(intrinsic.uminv, Domain.UnsignedInt); break;
                    case Mnemonic.umlal: RewriteSimdTernaryWiden(intrinsic.umlal, Domain.UnsignedInt); break;
                    case Mnemonic.umlal2: RewriteSimdTernaryWiden(intrinsic.umlal2, Domain.UnsignedInt); break;
                    case Mnemonic.umlsl: RewriteSimdTernaryWiden(intrinsic.umlsl, Domain.UnsignedInt); break;
                    case Mnemonic.umlsl2: RewriteSimdTernaryWiden(intrinsic.umlsl2, Domain.UnsignedInt); break;
                    case Mnemonic.umov: RewriteVectorElementToScalar(Domain.UnsignedInt); break;
                    case Mnemonic.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull: RewriteMull(PrimitiveType.UInt32, PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull2: RewriteSimdBinary(intrinsic.umull2, Domain.UnsignedInt); break;
                    case Mnemonic.uqadd: RewriteSimdBinary(intrinsic.uqadd, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshl: RewriteSimdBinary(intrinsic.uqrshl, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn: RewriteSimdBinary(intrinsic.uqrshrn, Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn2: RewriteSimdBinary(intrinsic.uqrshrn2, Domain.UnsignedInt); break;
                    case Mnemonic.uqshl: RewriteSimdBinary(intrinsic.uqshl, Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn: RewriteSimdBinary(intrinsic.uqshrn, Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn2: RewriteSimdBinary(intrinsic.uqshrn2, Domain.UnsignedInt); break;
                    case Mnemonic.uqsub: RewriteSimdBinary(intrinsic.uqsub, Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn: RewriteSimdUnary(intrinsic.uqxtn, Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn2: RewriteSimdUnary(intrinsic.uqxtn2, Domain.UnsignedInt); break;
                    case Mnemonic.urecpe: RewriteSimdUnary(intrinsic.urecpe, Domain.UnsignedInt); break;
                    case Mnemonic.urhadd: RewriteSimdBinary(intrinsic.urhadd, Domain.UnsignedInt); break;
                    case Mnemonic.urshl: RewriteSimdBinary(intrinsic.urshl, Domain.UnsignedInt); break;
                    case Mnemonic.urshr: RewriteSimdBinary(intrinsic.urshr, Domain.UnsignedInt); break;
                    case Mnemonic.ursqrte: RewriteSimdUnary(intrinsic.ursqrte, Domain.UnsignedInt); break;
                    case Mnemonic.ursra: RewriteSimdBinary(intrinsic.ursra, Domain.UnsignedInt); break;
                    case Mnemonic.ushl: RewriteMaybeSimdBinary(m.Shr, intrinsic.ushl, Domain.UnsignedInt); break;
                    case Mnemonic.ushll: RewriteMaybeSimdBinary(m.Shr, intrinsic.ushll, Domain.UnsignedInt); break;
                    case Mnemonic.ushll2: RewriteMaybeSimdBinary(m.Shr, intrinsic.ushll2, Domain.UnsignedInt); break;
                    case Mnemonic.ushr: RewriteMaybeSimdBinary(m.Shr, intrinsic.ushr, Domain.UnsignedInt); break;
                    case Mnemonic.usqadd: RewriteSimdBinary(intrinsic.usqadd, Domain.UnsignedInt); break;
                    case Mnemonic.usra: RewriteSimdBinary(intrinsic.usra, Domain.UnsignedInt); break;
                    case Mnemonic.usubl: RewriteSimdBinary(intrinsic.usubl, Domain.UnsignedInt); break;
                    case Mnemonic.usubl2: RewriteSimdBinary(intrinsic.usubl2, Domain.UnsignedInt); break;
                    case Mnemonic.usubw: RewriteSimdBinary(intrinsic.usubw, Domain.UnsignedInt); break;
                    case Mnemonic.usubw2: RewriteSimdBinary(intrinsic.usubw2, Domain.UnsignedInt); break;
                    case Mnemonic.uxtb: RewriteUSxt(Domain.UnsignedInt, 8); break;
                    case Mnemonic.uxth: RewriteUSxt(Domain.UnsignedInt, 16); break;
                    case Mnemonic.uxtl: RewriteSimdUnary(intrinsic.uxtl, Domain.UnsignedInt); break;
                    case Mnemonic.uxtl2: RewriteSimdUnary(intrinsic.uxtl2, Domain.UnsignedInt); break;
                    case Mnemonic.uxtw: RewriteUSxt(Domain.UnsignedInt, 32); break;
                    case Mnemonic.uzp1: RewriteSimdBinary(intrinsic.uzp1, Domain.None); break;
                    case Mnemonic.uzp2: RewriteSimdBinary(intrinsic.uzp2, Domain.None); break;
                    case Mnemonic.xtn: RewriteSimdUnary(intrinsic.xtn, Domain.None); break;
                    case Mnemonic.xtn2: RewriteSimdUnary(intrinsic.xtn2, Domain.None); break;
                    case Mnemonic.zip1: RewriteSimdBinary(intrinsic.zip1, Domain.None); break;
                    case Mnemonic.zip2: RewriteSimdBinary(intrinsic.zip2, Domain.None); break;
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

        private Expression AssignSimd(int iop, Expression exp)
        {
            var dst = (Identifier) RewriteOp(iop);
            var vreg = Registers.ByDomain[dst.Storage.Domain];
            m.Assign(dst, exp);
            var highBitsRemaining = vreg.DataType.BitSize - dst.DataType.BitSize;
            if (highBitsRemaining > 0)
            {
                var v = binder.EnsureRegister(vreg);
                var w = PrimitiveType.CreateWord(highBitsRemaining);
                m.Assign(v, m.Seq(Constant.Zero(w), dst));
            }
            return dst;
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
                return RewriteVectorRegisterOperand(vectorOp);
            case MemoryOperand mem:
                var ea = binder.EnsureRegister(mem.Base!);
                return m.Mem(mem.Width, ea);
            }
            throw new NotImplementedException($"Rewriting {op.GetType().Name} not implemented yet.");
        }

        private Expression RewriteVectorRegisterOperand(VectorRegisterOperand vectorOp)
        {
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
    }
}