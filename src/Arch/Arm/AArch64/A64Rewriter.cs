#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
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
                    case Mnemonic.abs: RewriteSimdUnary("__abs_{0}", Domain.SignedInt); break;
                    case Mnemonic.adc: RewriteAdcSbc(m.IAdd); break;
                    case Mnemonic.adcs: RewriteAdcSbc(m.IAdd, this.NZCV); break;
                    case Mnemonic.add: RewriteMaybeSimdBinary(m.IAdd, "__add_{0}"); break;
                    case Mnemonic.addhn: RewriteSimdBinary("__addhn_{0}", Domain.None); break;
                    case Mnemonic.addhn2: RewriteSimdBinary("__addhn2_{0}", Domain.None); break;
                    case Mnemonic.addp: RewriteSimdBinary("__addp_{0}", Domain.None); break;
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
                    case Mnemonic.bif: RewriteSimdBinary("__bif_{0}", Domain.None); break;
                    case Mnemonic.bit: RewriteSimdBinary("__bit_{0}", Domain.None); break;
                    case Mnemonic.bl: RewriteBl(); break;
                    case Mnemonic.blr: RewriteBlr(); break;
                    case Mnemonic.br: RewriteBr(); break;
                    case Mnemonic.brk: RewriteBrk(); break;
                    case Mnemonic.bsl: RewriteBsl(); break;
                    case Mnemonic.cbnz: RewriteCb(m.Ne0); break;
                    case Mnemonic.cbz: RewriteCb(m.Eq0); break;
                    case Mnemonic.ccmn: RewriteCcmn(); break;
                    case Mnemonic.ccmp: RewriteCcmp(); break;
                    case Mnemonic.cls: RewriteSimdUnary("__cls_{0}", Domain.None); break;
                    case Mnemonic.clz: RewriteClz(); break;
                    case Mnemonic.cmp: RewriteCmp(); break;
                    case Mnemonic.cmeq: RewriteCm("__cmeq_{0}", Domain.None); break;
                    case Mnemonic.cmge: RewriteCm("__cmge_{0}", Domain.SignedInt); break;
                    case Mnemonic.cmgt: RewriteCm("__cmgt_{0}", Domain.SignedInt); break;
                    case Mnemonic.cmhi: RewriteCm("__cmhi_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.cmhs: RewriteCm("__cmhs_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.cmle: RewriteCm("__cmle_{0}", Domain.SignedInt); break;
                    case Mnemonic.cmlt: RewriteCm("__cmlt_{0}", Domain.SignedInt); break;
                    case Mnemonic.cmtst: RewriteCm("__cmtst_{0}", Domain.None); break;
                    case Mnemonic.cnt: RewriteSimdUnary("__cnt_{0}", Domain.None); break;
                    case Mnemonic.csel: RewriteCsel(); break;
                    case Mnemonic.csinc: RewriteCsinc(); break;
                    case Mnemonic.csinv: RewriteCsinv(); break;
                    case Mnemonic.csneg: RewriteCsneg(); break;
                    case Mnemonic.dmb: RewriteDmb(); break;
                    case Mnemonic.dsb: RewriteDsb(); break;
                    case Mnemonic.dup: RewriteDup(); break;
                    case Mnemonic.eor: RewriteBinary(m.Xor); break;
                    case Mnemonic.eon: RewriteBinary((a, b) => m.Xor(a, m.Comp(b))); break;
                    case Mnemonic.eret: RewriteEret(); break;
                    case Mnemonic.ext: RewriteExt(); break;
                    case Mnemonic.extr: RewriteExtr(); break;
                    case Mnemonic.fabs: RewriteFabs(); break;
                    case Mnemonic.fadd: RewriteFadd(); break;
                    case Mnemonic.fcmp: RewriteFcmp(); break;
                    case Mnemonic.fcmpe: RewriteFcmp(); break;  //$REVIEW: this leaves out the 'e'xception part. 
                    case Mnemonic.fcsel: RewriteFcsel(); break;
                    case Mnemonic.fcvt: RewriteFcvt(); break;
                    case Mnemonic.fcvtms: RewriteFcvtms(); break;
                    case Mnemonic.fcvtps: RewriteFcvtps(); break;
                    case Mnemonic.fcvtzs: RewriteFcvtzs(); break;
                    case Mnemonic.fdiv: RewriteMaybeSimdBinary(m.FDiv, "__fdiv_{0}", Domain.Real); break;
                    case Mnemonic.fmadd: RewriteIntrinsicFTernary("__fmaddf", "__fmadd"); break;
                    case Mnemonic.fmsub: RewriteIntrinsicFTernary("__fmsubf", "__fmsub"); break;
                    case Mnemonic.fmax: RewriteIntrinsicFBinary("fmaxf", "fmax"); break;
                    case Mnemonic.fmin: RewriteIntrinsicFBinary("fminf", "fmin"); break;
                    case Mnemonic.fmov: RewriteFmov(); break;
                    case Mnemonic.fmul: RewriteFmul(); break;
                    case Mnemonic.fneg: RewriteUnary(m.FNeg); break;
                    case Mnemonic.fnmul: RewriteFnmul(); break;
                    case Mnemonic.fsqrt: RewriteFsqrt(); break;
                    case Mnemonic.fsub: RewriteMaybeSimdBinary(m.FSub, "__fsub_{0}", Domain.Real); break;
                    case Mnemonic.hlt: RewriteHlt(); break;
                    case Mnemonic.isb: RewriteIsb(); break;
                    case Mnemonic.ld1: RewriteLdN("__ld1"); break;
                    case Mnemonic.ld1r: RewriteLdNr("__ld1r"); break;
                    case Mnemonic.ld2: RewriteLdN("__ld2"); break;
                    case Mnemonic.ld3: RewriteLdN("__ld3"); break;
                    case Mnemonic.ld4: RewriteLdN("__ld4"); break;
                    case Mnemonic.ldnp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldp: RewriteLoadStorePair(true); break;
                    case Mnemonic.ldarh: RewriteLoadAcquire("__load_acquire_{0}", PrimitiveType.Word16); break;
                    case Mnemonic.ldaxrh: RewriteLoadAcquire("__load_acquire_exclusive_{0}", PrimitiveType.Word16); break;
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
                    case Mnemonic.mla: RewriteSimdTrinary(Domain.None); break;
                    case Mnemonic.mls: RewriteSimdTrinary(Domain.None); break;
                    case Mnemonic.mneg: RewriteBinary((a, b) => m.Neg(m.IMul(a, b))); break;
                    case Mnemonic.mov: RewriteMov(); break;
                    case Mnemonic.movi: RewriteMovi(); break;
                    case Mnemonic.movk: RewriteMovk(); break;
                    case Mnemonic.movn: RewriteMovn(); break;
                    case Mnemonic.movz: RewriteMovz(); break;
                    case Mnemonic.mrs: RewriteMrs(); break;
                    case Mnemonic.msr: RewriteMsr(); break;
                    case Mnemonic.msub: RewriteMaddSub(m.ISub); break;
                    case Mnemonic.mul: RewriteMaybeSimdBinary(m.IMul, "__mul_{0}"); break;
                    case Mnemonic.mvn: RewriteUnary(m.Comp); break;
                    case Mnemonic.mvni: RewriteLogical((a, b) => ((BigConstant) b).Complement()); break;
                    case Mnemonic.neg: RewriteSimdUnary("__neg_{0}", Domain.SignedInt); break;
                    case Mnemonic.nop: m.Nop(); break;
                    case Mnemonic.not: RewriteMaybeSimdUnary(m.Comp, "__not_{0}"); break;
                    case Mnemonic.orr: RewriteLogical(m.Or); break;
                    case Mnemonic.orn: RewriteBinary((a, b) => m.Or(a, m.Comp(b))); break;
                    case Mnemonic.pmul: RewriteSimdBinary("__pmul_{0}", Domain.None); break;
                    case Mnemonic.pmull: RewriteSimdBinary("__pmull_{0}", Domain.None); break;
                    case Mnemonic.pmull2: RewriteSimdBinary("__pmull2_{0}", Domain.None); break;
                    case Mnemonic.prfm: RewritePrfm(); break;
                    case Mnemonic.raddhn: RewriteSimdBinary("__raddhn_{0}", Domain.None); break;
                    case Mnemonic.raddhn2: RewriteSimdBinary("__raddhn2_{0}", Domain.None); break;
                    case Mnemonic.rbit: RewriteRbit(); break;
                    case Mnemonic.ret: RewriteRet(); break;
                    case Mnemonic.rev: RewriteRev(); break;
                    case Mnemonic.rev16: RewriteRev16(); break;
                    case Mnemonic.rev32: RewriteRev32(); break;
                    case Mnemonic.rev64: RewriteSimdUnary("__rev64_{0}", Domain.None); break;
                    case Mnemonic.ror: RewriteRor(); break;
                    case Mnemonic.rorv: RewriteRor(); break;
                    case Mnemonic.rshrn: RewriteSimdBinary("__rshrn_{0}", Domain.None); break;
                    case Mnemonic.rshrn2: RewriteSimdBinary("__rshrn2_{0}", Domain.None); break;
                    case Mnemonic.rsubhn: RewriteSimdBinary("__rsubhn_{0}", Domain.None); break;
                    case Mnemonic.rsubhn2: RewriteSimdBinary("__rsubhn2_{0}", Domain.None); break;
                    case Mnemonic.saba: RewriteSimdBinary("__saba_{0}", Domain.None); break;
                    case Mnemonic.sabal: RewriteSimdBinary("__sabal_{0}", Domain.None); break;
                    case Mnemonic.sabal2: RewriteSimdBinary("__sabal2_{0}", Domain.None); break;
                    case Mnemonic.sabd: RewriteSimdBinary("__sabd_{0}", Domain.None); break;
                    case Mnemonic.sabdl: RewriteSimdBinary("__sabdl_{0}", Domain.None); break;
                    case Mnemonic.sabdl2: RewriteSimdBinary("__sabdl2_{0}", Domain.None); break;
                    case Mnemonic.sadalp: RewriteSimdBinary("__sadalp_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddl: RewriteSimdBinary("__saddl_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddl2: RewriteSimdBinary("__saddl2_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddlp: RewriteSimdUnary("__saddlp_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddlv: RewriteSimdUnary("__saddlv_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddw: RewriteSimdBinary("__saddw_{0}", Domain.SignedInt); break;
                    case Mnemonic.saddw2: RewriteSimdBinary("__saddw2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sbc: RewriteAdcSbc(m.ISub); break;
                    case Mnemonic.sbcs: RewriteAdcSbc(m.ISub, NZCV); break;
                    case Mnemonic.sbfiz: RewriteSbfiz(); break;
                    case Mnemonic.sbfm: RewriteUSbfm("__sbfm"); break;
                    case Mnemonic.scvtf: RewriteIcvtf("s", Domain.SignedInt); break;
                    case Mnemonic.sdiv: RewriteBinary(m.SDiv); break;
                    case Mnemonic.shadd: RewriteSimdBinary("__shadd_{0}", Domain.SignedInt); break;
                    case Mnemonic.shl: RewriteSimdBinary("__shl_{0}", Domain.None); break;
                    case Mnemonic.shll: RewriteSimdBinary("__shll_{0}", Domain.None); break;
                    case Mnemonic.shll2: RewriteSimdBinary("__shll2_{0}", Domain.None); break;
                    case Mnemonic.shrn: RewriteShrn(); break;
                    case Mnemonic.shsub: RewriteSimdBinary("__shsub_{0}", Domain.SignedInt); break;
                    case Mnemonic.sli: RewriteSimdBinary("__sli_{0}", Domain.None); break;
                    case Mnemonic.smaddl: RewriteMaddl(PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smax: RewriteSimdBinary("__smax_{0}", Domain.SignedInt); break;
                    case Mnemonic.smaxp: RewriteSimdBinary("__smaxp_{0}", Domain.SignedInt); break;
                    case Mnemonic.smaxv: RewriteSimdReduce("__smaxv_{0}", Domain.SignedInt); break;
                    case Mnemonic.smc: RewriteSmc(); break;
                    case Mnemonic.smin: RewriteSimdBinary("__smin_{0}", Domain.SignedInt); break;
                    case Mnemonic.sminp: RewriteSimdBinary("__sminp_{0}", Domain.SignedInt); break;
                    case Mnemonic.sminv: RewriteSimdUnary("__sminv_{0}", Domain.SignedInt); break;
                    case Mnemonic.smlal: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.smlal2: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.smlsl: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.smlsl2: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.smov: RewriteVectorElementToScalar(Domain.SignedInt); break;
                    case Mnemonic.smsubl: RewriteSmsubl(); break;
                    case Mnemonic.smull: RewriteMull(PrimitiveType.Int32, PrimitiveType.Int64, m.SMul); break;
                    case Mnemonic.smull2: RewriteSimdBinary("__smull2_{0}", Domain.SignedInt); break;
                    case Mnemonic.smulh: RewriteMulh(PrimitiveType.Int64, PrimitiveType.Int128, m.SMul); break;
                    case Mnemonic.sqabs: RewriteSimdUnary("__sqabs_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqadd: RewriteSimdBinary("__sqadd_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqdmulh: RewriteMaybeSimdBinary(Domain.SignedInt); break;
                    case Mnemonic.sqdmull: RewriteSimdBinary("__sqdmull_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqdmull2: RewriteSimdBinary("__sqdmull2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqdmlal: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlal2: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqdmlsl2: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqneg: RewriteSimdUnary("__sqneg_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrdmlah: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqrdmlsh: RewriteSimdTrinary(Domain.SignedInt); break;
                    case Mnemonic.sqrdmulh: RewriteSimdBinary("__sqrdmulh_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrshl: RewriteSimdBinary("__sqrshl_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrshrn: RewriteSimdBinary("__sqrshrn_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrshrn2: RewriteSimdBinary("__sqrshrn2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrshrun: RewriteSimdBinary("__sqrshrun_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqrshrun2: RewriteSimdBinary("__sqrshrun2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqshl: RewriteSimdBinary("__sqshl_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqshlu: RewriteSimdBinary("__sqshlu_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqshrn: RewriteSimdBinary("__sqshrn_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqshrn2: RewriteSimdBinary("__sqshrn2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqsub: RewriteSimdBinary("__sqsub_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqxtn: RewriteSimdUnary("__sqxtn_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqxtn2: RewriteSimdUnary("__sqxtn2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqxtun: RewriteSimdUnary("__sqxtun_{0}", Domain.SignedInt); break;
                    case Mnemonic.sqxtun2: RewriteSimdUnary("__sqxtun2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sri: RewriteSimdBinary("__sri_{0}", Domain.SignedInt); break;
                    case Mnemonic.srhadd: RewriteSimdBinary("__srhadd_{0}", Domain.SignedInt); break;
                    case Mnemonic.srshl: RewriteSimdBinary("__srshl_{0}", Domain.SignedInt); break;
                    case Mnemonic.srshr: RewriteSimdBinary("__srshr_{0}", Domain.SignedInt); break;
                    case Mnemonic.srsra: RewriteSimdBinary("__srsra_{0}", Domain.SignedInt); break;
                    case Mnemonic.sshl: RewriteSimdBinary("__sshl_{0}", Domain.SignedInt); break;
                    case Mnemonic.sshll: RewriteSimdBinary("__sshll_{0}", Domain.SignedInt); break;
                    case Mnemonic.sshll2: RewriteSimdBinary("__sshll2_{0}", Domain.SignedInt); break;
                    case Mnemonic.sshr: RewriteSimdWithScalar("__sshr_{0}", Domain.SignedInt); break;
                    case Mnemonic.ssra: RewriteSimdBinary("__ssra_{0}", Domain.SignedInt); break;
                    case Mnemonic.ssubl: RewriteSimdBinary("__ssubl_{0}", Domain.SignedInt); break;
                    case Mnemonic.ssubl2: RewriteSimdBinary("__ssubl2_{0}", Domain.SignedInt); break;
                    case Mnemonic.ssubw: RewriteSimdBinary("__ssubw_{0}", Domain.SignedInt); break;
                    case Mnemonic.ssubw2: RewriteSimdBinary("__ssubw2_{0}", Domain.SignedInt); break;
                    case Mnemonic.subhn: RewriteSimdBinary("__subhn_{0}", Domain.SignedInt); break;
                    case Mnemonic.subhn2: RewriteSimdBinary("__subhn2_{0}", Domain.SignedInt); break;
                    case Mnemonic.suqadd: RewriteSimdBinary("__suqadd_{0}", Domain.SignedInt); break;
                    case Mnemonic.st1: RewriteStN("__st1"); break;
                    case Mnemonic.st2: RewriteStN("__st2"); break;
                    case Mnemonic.st3: RewriteStN("__st3"); break;
                    case Mnemonic.st4: RewriteStN("__st4"); break;
                    case Mnemonic.stlr: RewriteStlr(instr.Operands[0].Width); break;
                    case Mnemonic.stlrh: RewriteStlr(PrimitiveType.Word16); break;
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
                    case Mnemonic.sxtl: RewriteSimdUnary("__sxtl_{0}", Domain.SignedInt); break;
                    case Mnemonic.sxtw: RewriteUSxt(Domain.SignedInt, 32); break;
                    case Mnemonic.tbnz: RewriteTb(m.Ne0); break;
                    case Mnemonic.tbl: RewriteTbl(); break;
                    case Mnemonic.tbx: RewriteTbx(); break;
                    case Mnemonic.tbz: RewriteTb(m.Eq0); break;
                    case Mnemonic.test: RewriteTest(); break;
                    case Mnemonic.trn1: RewriteSimdBinary("__trn1_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.trn2: RewriteSimdBinary("__trn2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaba: RewriteSimdBinary("__uaba_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uabal: RewriteSimdBinary("__uabal_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uabal2: RewriteSimdBinary("__uabal2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uabd: RewriteSimdBinary("__uabd_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uabdl: RewriteSimdBinary("__uabdl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uabdl2: RewriteSimdBinary("__uabdl2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uadalp: RewriteSimdBinary("__uadalp_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaddl: RewriteSimdBinary("__uaddl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaddl2: RewriteSimdBinary("__uaddl2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaddlp: RewriteSimdBinary("__uaddlp_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaddlv: RewriteSimdUnary("__uaddlv_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uaddw: RewriteUaddw(); break;
                    case Mnemonic.uaddw2: RewriteSimdBinary("__uaddw_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ubfm: RewriteUSbfm("__ubfm"); break;
                    case Mnemonic.ucvtf: RewriteIcvtf("u", Domain.UnsignedInt); break;
                    case Mnemonic.udiv: RewriteBinary(m.UDiv); break;
                    case Mnemonic.uhadd: RewriteSimdBinary("__uhadd_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uhsub: RewriteSimdBinary("__uhsub_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.umaddl: RewriteMaddl(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umax: RewriteSimdBinary("__umax_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.umaxp: RewriteSimdBinary("__umaxp_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.umaxv: RewriteSimdUnary("__umaxv_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.umin: RewriteSimdBinary("__umin_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uminp: RewriteSimdBinary("__uminp_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uminv: RewriteSimdUnary("__uminv_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.umlal: RewriteSimdTrinary(Domain.UnsignedInt); break;
                    case Mnemonic.umlal2: RewriteSimdTrinary(Domain.UnsignedInt); break;
                    case Mnemonic.umlsl: RewriteSimdTrinary(Domain.UnsignedInt); break;
                    case Mnemonic.umlsl2: RewriteSimdTrinary(Domain.UnsignedInt); break;
                    case Mnemonic.umov: RewriteVectorElementToScalar(Domain.UnsignedInt); break;
                    case Mnemonic.umulh: RewriteMulh(PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull: RewriteMull(PrimitiveType.UInt32, PrimitiveType.UInt64, m.UMul); break;
                    case Mnemonic.umull2: RewriteSimdBinary("__umull2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqadd: RewriteSimdBinary("__uqadd_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqrshl: RewriteSimdBinary("__uqrshl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn: RewriteSimdBinary("__uqrshrn_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqrshrn2: RewriteSimdBinary("__uqrshrn2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqshl: RewriteSimdBinary("__uqshl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn: RewriteSimdBinary("__uqshrn_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqshrn2: RewriteSimdBinary("__uqshrn2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqsub: RewriteSimdBinary("__uqsub_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn: RewriteSimdUnary("__uqxtn_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uqxtn2: RewriteSimdUnary("__uqxtn2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.urecpe: RewriteSimdUnary("__urecpe_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.urhadd: RewriteSimdBinary("__urhadd_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.urshl: RewriteSimdBinary("__urshl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.urshr: RewriteSimdBinary("__urshr_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ursqrte: RewriteSimdUnary("__ursqrte_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ursra: RewriteSimdBinary("__ursra_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ushl: RewriteMaybeSimdBinary(m.Shr, "__ushl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ushll: RewriteMaybeSimdBinary(m.Shr, "__ushll_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ushll2: RewriteMaybeSimdBinary(m.Shr, "__ushll2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.ushr: RewriteMaybeSimdBinary(m.Shr, "__ushr_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usqadd: RewriteSimdBinary("__usqadd_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usra: RewriteSimdBinary("__usra_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usubl: RewriteSimdBinary("__usubl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usubl2: RewriteSimdBinary("__usubl2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usubw: RewriteSimdBinary("__usubw_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.usubw2: RewriteSimdBinary("__usubw2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uxtb: RewriteUSxt(Domain.UnsignedInt, 8); break;
                    case Mnemonic.uxth: RewriteUSxt(Domain.UnsignedInt, 16); break;
                    case Mnemonic.uxtl: RewriteSimdUnary("__uxtl_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uxtl2: RewriteSimdUnary("__uxtl2_{0}", Domain.UnsignedInt); break;
                    case Mnemonic.uxtw: RewriteUSxt(Domain.UnsignedInt, 32); break;
                    case Mnemonic.uzp1: RewriteSimdBinary("__uzp1_{0}", Domain.None); break;
                    case Mnemonic.uzp2: RewriteSimdBinary("__uzp2_{0}", Domain.None); break;
                    case Mnemonic.xtn: RewriteSimdUnary("__xtn_{0}", Domain.None); break;
                    case Mnemonic.xtn2: RewriteSimdUnary("__xtn2_{0}", Domain.None); break;
                    case Mnemonic.zip1: RewriteSimdBinary("__zip1_{0}", Domain.None); break;
                    case Mnemonic.zip2: RewriteSimdBinary("__zip2_{0}", Domain.None); break;
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

        //$TODO: prefer this RewriteOp
        private Expression RewriteOp(int iop, bool maybe0 = false) => RewriteOp(instr.Operands[iop], maybe0);

        private Expression RewriteOp(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterOperand regOp:
                if (maybe0)
                {
                    if (regOp.Register == Registers.GpRegs32[31])
                        return m.Word32(0);
                    if (regOp.Register == Registers.GpRegs64[31])
                        return m.Word64(0);
                }
                return binder.EnsureRegister(regOp.Register);
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
                    var eType = PrimitiveType.CreateWord(Bitsize(vectorOp.ElementType));
                    return m.ARef(eType, vreg, Constant.Int32(vectorOp.Index));
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
                throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
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

        protected ArmCondition Invert(ArmCondition cc)
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