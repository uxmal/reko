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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using Reko.Core.Types;
using System.Diagnostics;
using System.Linq;
using Reko.Core.Services;
using Reko.Core.Memory;
using Reko.Core.Intrinsics;
using Reko.Core.Serialization;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter : IEnumerable<RtlInstructionCluster>
    {
        private static readonly PrimitiveType Int40 = PrimitiveType.Create(Domain.SignedInt, 40);
        private static readonly PrimitiveType UInt40 = PrimitiveType.Create(Domain.UnsignedInt, 40);

        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly XtensaArchitecture arch;
        private readonly IEnumerator<XtensaInstruction> dasm;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly RtlEmitter m;
        private XtensaInstruction instr;
        private InstrClass iclass;
        private Address? lbegin;
        private Address? lend;

        public XtensaRewriter(XtensaArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new XtensaDisassembler(this.arch, rdr).GetEnumerator();
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    host.Error(instr.Address,$"Rewriting of Xtensa instruction '{instr}' not implemented yet.");
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                case Mnemonic.reserved:
                    iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.abs: RewriteUnaryFn(CommonOps.Abs); break;
                case Mnemonic.add:
                case Mnemonic.add_n: RewriteBinOp(m.IAdd); break;
                case Mnemonic.add_s: RewriteBinOp(m.FAdd); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.addi_n: RewriteAddi(); break;
                case Mnemonic.addmi: RewriteBinOp(m.IAdd); break;
                case Mnemonic.addx2: RewriteAddx(2); break;
                case Mnemonic.addx4: RewriteAddx(4); break;
                case Mnemonic.addx8: RewriteAddx(8); break;
                case Mnemonic.all4: RewriteAll(4, m.And); break;
                case Mnemonic.all8: RewriteAll(8, m.And); break;
                case Mnemonic.and: RewriteBinOp(m.And); break;
                case Mnemonic.andb: RewriteBinOp(m.And); break;
                case Mnemonic.andbc: RewriteBinOp((a, b) => m.And(a, m.Not(b))); break;
                case Mnemonic.any4: RewriteAll(4, m.Or); break;
                case Mnemonic.any8: RewriteAll(8, m.Or); break;
                case Mnemonic.ball: RewriteBall(); break;
                case Mnemonic.bany: RewriteBany(); break;
                case Mnemonic.bbc: 
                case Mnemonic.bbci: RewriteBbx(m.Eq0); break;
                case Mnemonic.bbs:
                case Mnemonic.bbsi: RewriteBbx(m.Ne0); break;
                case Mnemonic.beq:
                case Mnemonic.beqi: RewriteBranch(m.Eq); break;
                case Mnemonic.beqz:
                case Mnemonic.beqz_n: RewriteBranchZ(m.Eq0); break;
                case Mnemonic.bf: RewriteBranchZ(m.Not); break;
                case Mnemonic.bge:
                case Mnemonic.bgei: RewriteBranch(m.Ge); break;
                case Mnemonic.bgeu:
                case Mnemonic.bgeui: RewriteBranch(m.Uge); break;
                case Mnemonic.bgez: RewriteBranchZ(m.Ge0); break;
                case Mnemonic.blt: RewriteBranch(m.Lt); break;
                case Mnemonic.blti: RewriteBranch(m.Lt); break;
                case Mnemonic.bltu:
                case Mnemonic.bltui: RewriteBranch(m.Ult); break;
                case Mnemonic.bltz: RewriteBranchZ(m.Lt0); break;
                case Mnemonic.bnall: RewriteBnall(); break;
                case Mnemonic.bne: RewriteBranch(m.Ne); break;
                case Mnemonic.bnei: RewriteBranch(m.Ne); break;
                case Mnemonic.bnez:
                case Mnemonic.bnez_n: RewriteBranchZ(m.Ne0); break;
                case Mnemonic.bnone: RewriteBnone(); break;
                case Mnemonic.@break: RewriteBreak(); break;
                case Mnemonic.call0: RewriteCall0(); break;
                case Mnemonic.call4: RewriteCallW(4); break;
                case Mnemonic.call8: RewriteCallW(8); break;
                case Mnemonic.call12: RewriteCallW(12); break;
                case Mnemonic.callx0: RewriteCall0(); break;
                case Mnemonic.callx4: RewriteCallW(4); break;
                case Mnemonic.callx8: RewriteCallW(8); break;
                case Mnemonic.callx12: RewriteCallW(12); break;
                case Mnemonic.ceil_s: RewriteCvtFloatToIntegral(ceil_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.clamps: RewriteClamps(); break;
                case Mnemonic.cust0: RewriteIntrinsicProc(cust0_intrinsic); break;
                case Mnemonic.cust1: RewriteIntrinsicProc(cust1_intrinsic); break;
                case Mnemonic.dhi: RewriteCacheFn(dhi_intrinsic); break;
                case Mnemonic.dhu: RewriteCacheFn(dhu_intrinsic); break;
                case Mnemonic.dhwb: RewriteCacheFn(dhwb_intrinsic); break;
                case Mnemonic.dhwbi: RewriteCacheFn(dhwbi_intrinsic); break;
                case Mnemonic.dii: RewriteCacheFn(dii_intrinsic); break;
                case Mnemonic.diu: RewriteCacheFn(diu_intrinsic); break;
                case Mnemonic.dpfr: RewriteCacheFn(dpfr_intrinsic); break;
                case Mnemonic.dpfro: RewriteCacheFn(dpfro_intrinsic); break;
                case Mnemonic.dpfw: RewriteCacheFn(dpfw_intrinsic); break;
                case Mnemonic.dpfwo: RewriteCacheFn(dpfwo_intrinsic); break;
                case Mnemonic.dsync: RewriteIntrinsicProc(dsync_intrinsic); break;
                case Mnemonic.esync: RewriteIntrinsicProc(esync_intrinsic); break;
                case Mnemonic.excw: RewriteIntrinsicProc(excw_intrinsic); break;
                case Mnemonic.extui: RewriteExtui(); break;
                case Mnemonic.entry: RewriteEntry(); break;
                case Mnemonic.float_s: RewriteFloat_s(PrimitiveType.Int32); break;
                case Mnemonic.floor_s: RewriteBinOp((a, b) => m.Fn(floor_intrinsic, a, b)); break;
                case Mnemonic.iii: RewriteCacheFn(iii_intrinsic); break;
                case Mnemonic.iitlb: RewriteIntrinsicProc(iitlb_intrinsic); break;
                case Mnemonic.ipf: RewriteCacheFn(ipf_intrinsic); break;
                case Mnemonic.isync: RewriteIntrinsicProc(isync_intrinsic); break;
                case Mnemonic.j:
                case Mnemonic.jx: RewriteJ(); break;
                case Mnemonic.ill: RewriteIll(); break;
                case Mnemonic.l16si: RewriteLsi(PrimitiveType.Int16); break;
                case Mnemonic.l16ui: RewriteLui(PrimitiveType.UInt16); break;
                case Mnemonic.l32ai: RewriteL32ai(); break;
                case Mnemonic.l32i: RewriteL32i(); break;
                case Mnemonic.l32e: RewriteL32e(); break;
                case Mnemonic.l32i_n: RewriteL32i(); break;
                case Mnemonic.l32r: RewriteCopy(); break;
                case Mnemonic.l8ui: RewriteLui(PrimitiveType.Byte); break;
                case Mnemonic.lddec: RewriteLddecinc(m.ISub); break;
                case Mnemonic.ldinc: RewriteLddecinc(m.IAdd); break;
                case Mnemonic.ldpte: RewriteIntrinsicProc(ldpte_intrinsic); break;
                case Mnemonic.loop: RewriteLoop(); break;
                case Mnemonic.lsiu: RewriteLsiu(); break;
                case Mnemonic.madd_s: RewriteMaddSub(m.FAdd); break;
                case Mnemonic.memw: RewriteNop(); break; /// memory sync barriers?
                case Mnemonic.max: RewriteMax(); break;
                case Mnemonic.maxu: RewriteMaxu(); break;
                case Mnemonic.min: RewriteMin(); break;
                case Mnemonic.minu: RewriteMinu(); break;
                case Mnemonic.mov_n: RewriteCopy(); break;
                case Mnemonic.mov_s: RewriteCopy(); break;
                case Mnemonic.movf:
                case Mnemonic.movf_s: RewriteMovft(e => e); break;
                case Mnemonic.movi: RewriteCopy(); break;
                case Mnemonic.movi_n: RewriteMovi_n(); break;
                case Mnemonic.movsp: RewriteCopy(); break;
                case Mnemonic.moveqz:
                case Mnemonic.moveqz_s: RewriteMovcc(m.Eq); break;
                case Mnemonic.movltz:
                case Mnemonic.movltz_s: RewriteMovcc(m.Lt); break;
                case Mnemonic.movgez:
                case Mnemonic.movgez_s: RewriteMovcc(m.Ge); break;
                case Mnemonic.movnez:
                case Mnemonic.movnez_s: RewriteMovcc(m.Ne); break;
                case Mnemonic.movt:
                case Mnemonic.movt_s: RewriteMovft(m.Not); break;
                case Mnemonic.msub_s: RewriteMaddSub(m.FSub); break;
                case Mnemonic.mul_aa_hh: RewriteMul(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mul_aa_hl: RewriteMul(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mul_aa_lh: RewriteMul(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mul_aa_ll: RewriteMul(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mul_ad_hh: RewriteMul(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mul_ad_hl: RewriteMul(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mul_ad_lh: RewriteMul(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mul_ad_ll: RewriteMul(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mul_da_hh: RewriteMul(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mul_da_hl: RewriteMul(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mul_da_lh: RewriteMul(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mul_da_ll: RewriteMul(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mul_dd_hh: RewriteMul(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mul_dd_hl: RewriteMul(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mul_dd_lh: RewriteMul(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mul_dd_ll: RewriteMul(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mula_aa_hh: RewriteMula(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mula_aa_hl: RewriteMula(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mula_aa_lh: RewriteMula(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mula_aa_ll: RewriteMula(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mula_ad_hh: RewriteMula(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mula_ad_hl: RewriteMula(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mula_ad_lh: RewriteMula(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mula_ad_ll: RewriteMula(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mula_da_hh: RewriteMula(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mula_da_hh_lddec: RewriteMulaIncDec(mul_hh_intrinsic, Int40, -4); break;
                case Mnemonic.mula_da_hh_ldinc: RewriteMulaIncDec(mul_hh_intrinsic, Int40, 4); break;
                case Mnemonic.mula_da_hl: RewriteMula(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mula_da_hl_lddec: RewriteMulaIncDec(mul_hl_intrinsic, Int40, -4); break;
                case Mnemonic.mula_da_hl_ldinc: RewriteMulaIncDec(mul_hl_intrinsic, Int40, 4); break;
                case Mnemonic.mula_da_lh: RewriteMula(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mula_da_lh_lddec: RewriteMulaIncDec(mul_lh_intrinsic, Int40, -4); break;
                case Mnemonic.mula_da_lh_ldinc: RewriteMulaIncDec(mul_lh_intrinsic, Int40, 4); break;
                case Mnemonic.mula_da_ll: RewriteMula(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mula_da_ll_lddec: RewriteMulaIncDec(mul_ll_intrinsic, Int40, -4); break;
                case Mnemonic.mula_da_ll_ldinc: RewriteMulaIncDec(mul_ll_intrinsic, Int40, 4); break;
                case Mnemonic.mula_dd_hh: RewriteMula(mul_hh_intrinsic, Int40); break;
                case Mnemonic.mula_dd_hh_lddec: RewriteMulaIncDec(mul_hh_intrinsic, Int40, -4); break;
                case Mnemonic.mula_dd_hl: RewriteMula(mul_hl_intrinsic, Int40); break;
                case Mnemonic.mula_dd_lh: RewriteMula(mul_lh_intrinsic, Int40); break;
                case Mnemonic.mula_dd_ll: RewriteMula(mul_ll_intrinsic, Int40); break;
                case Mnemonic.mula_dd_ll_lddec: RewriteMulaIncDec(mul_ll_intrinsic, Int40, -4); break;
                case Mnemonic.muls_aa_hh: RewriteMuls(mul_hh_intrinsic, Int40); break;
                case Mnemonic.muls_aa_hl: RewriteMuls(mul_hl_intrinsic, Int40); break;
                case Mnemonic.muls_aa_lh: RewriteMuls(mul_lh_intrinsic, Int40); break;
                case Mnemonic.muls_aa_ll: RewriteMuls(mul_ll_intrinsic, Int40); break;
                case Mnemonic.muls_ad_hh: RewriteMuls(mul_hh_intrinsic, Int40); break;
                case Mnemonic.muls_ad_hl: RewriteMuls(mul_hl_intrinsic, Int40); break;
                case Mnemonic.muls_ad_lh: RewriteMuls(mul_lh_intrinsic, Int40); break;
                case Mnemonic.muls_ad_ll: RewriteMuls(mul_ll_intrinsic, Int40); break;
                case Mnemonic.muls_da_hh: RewriteMuls(mul_hh_intrinsic, Int40); break;
                case Mnemonic.muls_da_hl: RewriteMuls(mul_hl_intrinsic, Int40); break;
                case Mnemonic.muls_da_lh: RewriteMuls(mul_lh_intrinsic, Int40); break;
                case Mnemonic.muls_da_ll: RewriteMuls(mul_ll_intrinsic, Int40); break;
                case Mnemonic.muls_dd_hh: RewriteMuls(mul_hh_intrinsic, Int40); break;
                case Mnemonic.muls_dd_hl: RewriteMuls(mul_hl_intrinsic, Int40); break;
                case Mnemonic.muls_dd_lh: RewriteMuls(mul_lh_intrinsic, Int40); break;
                case Mnemonic.muls_dd_ll: RewriteMuls(mul_ll_intrinsic, Int40); break;

                case Mnemonic.mul_s: RewriteBinOp(m.FMul); break;
                case Mnemonic.mul16s: RewriteMul16(m.SMul, Domain.SignedInt); break;
                case Mnemonic.mul16u: RewriteMul16(m.UMul, Domain.UnsignedInt); break;
                case Mnemonic.mull: RewriteBinOp(m.IMul); break;
                case Mnemonic.mulsh: RewriteMulh(mulsh_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.muluh: RewriteMulh(muluh_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.neg: RewriteUnaryOp(m.Neg); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.nop_n: m.Nop(); break;
                case Mnemonic.nsa: RewriteUnaryFn(nsa_intrinsic); break;
                case Mnemonic.nsau: RewriteUnaryFn(nsau_intrinsic); break;
                case Mnemonic.oeq_s: RewriteBinOp(m.FEq); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ole_s: RewriteBinOp(m.FLe); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.olt_s: RewriteBinOp(m.FLt); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.orb: RewriteOr(); break;
                case Mnemonic.orbc: RewriteBinOp((a, b) => m.Or(a, m.Not(b))); break;
                case Mnemonic.pitlb: RewriteUnaryFn(pitlb_intrinsic); break;
                case Mnemonic.quos: RewriteBinOp(m.SDiv); break;
                case Mnemonic.quou: RewriteBinOp(m.UDiv); break;
                case Mnemonic.rdtlb0: RewriteUnaryFn(rdtlb0_intrinsic); break;
                case Mnemonic.rdtlb1: RewriteUnaryFn(rdtlb1_intrinsic); break;
                case Mnemonic.rems: RewriteBinOp(m.SMod); break;
                case Mnemonic.remu: RewriteBinOp(m.UMod); break;
                case Mnemonic.ret:
                case Mnemonic.ret_n: RewriteRet(); break;
                case Mnemonic.rfe: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from exception?
                case Mnemonic.rfi: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from interrupt?
                case Mnemonic.ritlb0: RewriteUnaryFn(ritlb0_intrinsic); break;
                case Mnemonic.ritlb1: RewriteUnaryFn(ritlb1_intrinsic); break;
                case Mnemonic.rotw: RewriteRotw(); break;
                case Mnemonic.round_s: RewriteCvtFloatToIntegral(round_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.rsil: RewriteUnaryFn(rsil_intrinsic); break;
                case Mnemonic.rer: RewriteRer(); break;
                case Mnemonic.rfr: RewriteCopy(); break;
                case Mnemonic.rsr: RewriteCopy(); break;
                case Mnemonic.rsync: RewriteRsync(); break;
                case Mnemonic.rur: RewriteCopy(); break;
                case Mnemonic.s16i: RewriteSi(PrimitiveType.Word16); break;
                case Mnemonic.s32c1i: RewriteS32c1i(); break;
                case Mnemonic.s32e: RewriteS32e(); break;
                case Mnemonic.s32i:
                case Mnemonic.s32i_n: RewriteSi(PrimitiveType.Word32); break;
                case Mnemonic.s32ri: RewriteSi(PrimitiveType.Word32); break; //$REVIEW: what about concurrency semantics
                case Mnemonic.s8i: RewriteSi(PrimitiveType.Byte); break;
                case Mnemonic.sext: RewriteSext(); break;
                case Mnemonic.sll: RewriteShift(m.Shl); break;
                case Mnemonic.slli: RewriteShiftI(m.Shl); break;
                case Mnemonic.sra: RewriteShift(m.Sar); break;
                case Mnemonic.srai: RewriteShiftI(m.Sar); break;
                case Mnemonic.src: RewriteSrc(); break;
                case Mnemonic.srl: RewriteShift(m.Sar); break;
                case Mnemonic.srli: RewriteShiftI(m.Shr); break;
                case Mnemonic.ssa8b: RewriteSsa8b(); break;
                case Mnemonic.ssa8l: RewriteSsa8l(); break;
                case Mnemonic.ssi: RewriteSi(PrimitiveType.Real32); break;
                case Mnemonic.ssl: RewriteSsl(); break;
                case Mnemonic.ssr:
                case Mnemonic.ssai: RewriteSsa(); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub); break;
                case Mnemonic.sub_s: RewriteBinOp(m.FSub); break;
                case Mnemonic.subx2: RewriteSubx(2); break;
                case Mnemonic.subx4: RewriteSubx(4); break;
                case Mnemonic.subx8: RewriteSubx(8); break;
                case Mnemonic.syscall: RewriteSyscall(); break;
                case Mnemonic.trunc_s: RewriteCvtFloatToIntegral(trunc_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.ueq_s: RewriteBinOp(m.Eq); break;     //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ufloat_s: RewriteFloat_s(PrimitiveType.UInt32); break;
                case Mnemonic.ule_s: RewriteBinOp(m.FLe); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ult_s: RewriteBinOp(m.FLt); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.umul_aa_hh: RewriteMul(umul_hh_intrinsic, UInt40); break;
                case Mnemonic.umul_aa_hl: RewriteMul(umul_hl_intrinsic, UInt40); break;
                case Mnemonic.umul_aa_lh: RewriteMul(umul_lh_intrinsic, UInt40); break;
                case Mnemonic.umul_aa_ll: RewriteMul(umul_ll_intrinsic, UInt40); break;
                case Mnemonic.un_s: RewriteBinOp((a,b) => m.Fn(FpOps.IsUnordered_f32, a, b)); break;
                case Mnemonic.utrunc_s: RewriteCvtFloatToIntegral(utrunc_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.waiti: RewriteIntrinsicProc(waiti_instrinsic); break;
                case Mnemonic.wdtlb: RewriteIntrinsicProc(wdtlb_instrinsic); break;
                case Mnemonic.witlb: RewriteIntrinsicProc(witlb_instrinsic); break;
                case Mnemonic.wer: RewriteWer(); break;
                case Mnemonic.wsr: RewriteWsr(); break;
                case Mnemonic.wur: RewriteInverseCopy(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor); break;
                case Mnemonic.xorb: RewriteBinOp(m.Xor); break;
                case Mnemonic.xsr: RewriteXsr(); break;
                }
                CheckForLoopExit();
                yield return m.MakeCluster(addr, len, iclass);
                this.rtlInstructions.Clear();
            }
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("Xtrw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void CheckForLoopExit()
        {
            if (lend is null)
                return;
            if (instr.Address.ToLinear() + (uint)instr.Length == lend.Value.ToLinear())
            {
                var addrNext = instr.Address + instr.Length;
                var lcount = binder.EnsureRegister(Registers.LCOUNT);
                m.BranchInMiddleOfInstruction(m.Eq0(lcount), addrNext, InstrClass.ConditionalTransfer);
                m.Assign(lcount, m.ISub(lcount, 1));
                m.Goto(this.lbegin!);

                lbegin = null;
                lend = null;
            }
        }

        private void PreDec(int iOp)
        {
            var reg = RewriteOp(instr.Operands[iOp]);
            m.Assign(reg, m.AddSubSignedInt(reg, -4));
        }

        private Expression RewriteOp(MachineOperand op)
        {
            switch (op)
            {
            case RegisterStorage rOp:
                return binder.EnsureRegister(rOp);
            case Address addr:
                return addr;
            case Constant iOp:
                return iOp;
            }
            throw new NotImplementedException(op.GetType().FullName);
        }

        // Sign-extend an operand known to be signed immediate.
        private Constant RewriteSimm(MachineOperand op)
        {
            var iOp = (Constant)op;
            return Constant.Int32(iOp.ToInt32());
        }

        // Zero-extend an operand known to be unsigned immediate.
        private Constant RewriteUimm(MachineOperand op)
        {
            var iOp = (Constant)op;
            return Constant.UInt32(iOp.ToUInt32());
        }

        private static readonly IntrinsicProcedure break_intrinsic = new IntrinsicBuilder("__break", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure ceil_intrinsic = new IntrinsicBuilder("__ceil", false)
            .Param(PrimitiveType.Real32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure clamps_intrinsic = new IntrinsicBuilder("__clamps", false)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure cust0_intrinsic = new IntrinsicBuilder("__cust0", true)
            .Void();
        private static readonly IntrinsicProcedure cust1_intrinsic = new IntrinsicBuilder("__cust1", true)
            .Void();

        private static readonly IntrinsicProcedure dhi_intrinsic = new IntrinsicBuilder("__dhi", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dhu_intrinsic = new IntrinsicBuilder("__dhu", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dhwb_intrinsic = new IntrinsicBuilder("__dhwb", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dhwbi_intrinsic = new IntrinsicBuilder("__dhwbi", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dii_intrinsic = new IntrinsicBuilder("__dii", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure diu_intrinsic = new IntrinsicBuilder("__diu", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dpfr_intrinsic = new IntrinsicBuilder("__dpfr", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dpfro_intrinsic = new IntrinsicBuilder("__dpfro", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dpfw_intrinsic = new IntrinsicBuilder("__dpfw", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dpfwo_intrinsic = new IntrinsicBuilder("__dpfwo", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure dsync_intrinsic = new IntrinsicBuilder("__dsync", true)
            .Void();
        private static readonly IntrinsicProcedure esync_intrinsic = new IntrinsicBuilder("__esync", true)
            .Void();
        private static readonly IntrinsicProcedure excw_intrinsic  = new IntrinsicBuilder("__excw", true)
            .Void();


        private static readonly IntrinsicProcedure floor_intrinsic = new IntrinsicBuilder("__floor_s", false)
            .Param(PrimitiveType.Real32)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure iii_intrinsic = new IntrinsicBuilder("__iii", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure ill_intrinsic = new IntrinsicBuilder(
            "__ill", true, new ProcedureCharacteristics
            {
                Terminates = true,
            }).Void();
        private static readonly IntrinsicProcedure ipf_intrinsic = new IntrinsicBuilder("__ipf", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure isync_intrinsic = new IntrinsicBuilder("__isync", true)
            .Void();
        private static readonly IntrinsicProcedure iitlb_intrinsic = new IntrinsicBuilder("__iitlb", true)
            .Param(PrimitiveType.Ptr32)
            .Void();

        private static readonly IntrinsicProcedure l32ai_intrinsic = new IntrinsicBuilder("__l32ai", true)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure l32e_intrinsic = new IntrinsicBuilder("__l32e", true)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ldpte_intrinsic = new IntrinsicBuilder("__ldpte", true)
            .Void();

        private static readonly IntrinsicProcedure mul_ll_intrinsic = new IntrinsicBuilder("__mul_ll", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure mul_hh_intrinsic = new IntrinsicBuilder("__mul_hh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure mul_lh_intrinsic = new IntrinsicBuilder("__mul_lh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure mul_hl_intrinsic = new IntrinsicBuilder("__mul_hl", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");

        private static readonly IntrinsicProcedure mulsh_intrinsic = new IntrinsicBuilder("__mulsh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure muluh_intrinsic = new IntrinsicBuilder("__muluh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");


        private static readonly IntrinsicProcedure nsa_intrinsic = new IntrinsicBuilder("__nsa", false)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure nsau_intrinsic = new IntrinsicBuilder("__nsau", false)
            .Param(PrimitiveType.UInt32)
            .Returns(PrimitiveType.Int32);

        private static readonly IntrinsicProcedure pitlb_intrinsic = new IntrinsicBuilder("__pitlb", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure rer_intrinsic = new IntrinsicBuilder("__rer", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure rdtlb0_intrinsic = new IntrinsicBuilder("__rdtlb0", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure rdtlb1_intrinsic = new IntrinsicBuilder("__rdtlb1", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ritlb0_intrinsic = new IntrinsicBuilder("__ritlb0", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ritlb1_intrinsic = new IntrinsicBuilder("__ritlb1", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure rotw_intrinsic = new IntrinsicBuilder("__rotate_window", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure rsil_intrinsic = new IntrinsicBuilder("__rsil", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure round_intrinsic = new IntrinsicBuilder("__round", false)
            .Param(PrimitiveType.Real32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure rsync_intrinsic = new IntrinsicBuilder("__rsync", true)
            .Void();
        private static readonly IntrinsicProcedure s32c1i_intrinsic = new IntrinsicBuilder("__s32c1i", true)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure s32e_intrinsic = new IntrinsicBuilder("__s32e", true)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure sext_intrinsic = new IntrinsicBuilder("__sext", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure trunc_intrinsic = new IntrinsicBuilder("__trunc", false)
            .Param(PrimitiveType.Real32)
            .Returns(PrimitiveType.Int32);

        private static readonly IntrinsicProcedure umul_ll_intrinsic = new IntrinsicBuilder("__umul_ll", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure umul_hh_intrinsic = new IntrinsicBuilder("__umul_hh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure umul_lh_intrinsic = new IntrinsicBuilder("__umul_lh", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure umul_hl_intrinsic = new IntrinsicBuilder("__umul_hl", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc").Param("TSrc")
            .Returns("TDst");

        private static readonly IntrinsicProcedure utrunc_intrinsic = new IntrinsicBuilder("__utrunc", false)
            .Param(PrimitiveType.Real32)
            .Returns(PrimitiveType.UInt32);
        private static readonly IntrinsicProcedure waiti_instrinsic = new IntrinsicBuilder("__waiti", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure wdtlb_instrinsic = new IntrinsicBuilder("__wdtlb", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure witlb_instrinsic = new IntrinsicBuilder("__witlb", true)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure wer_intrinsic = new IntrinsicBuilder("__wer", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure xsr_intrinsic = new IntrinsicBuilder("__xsr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
    }
}
