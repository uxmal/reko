#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        private XtensaInstruction instr;
        private InstrClass iclass;
        private RtlEmitter m;
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
            this.instr = null!;
            this.m = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                var addr = dasm.Current.Address;
                var len = dasm.Current.Length;
                var rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
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
                case Mnemonic.abs: RewriteIntrinsicFn("abs", false); break;
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
                case Mnemonic.ceil_s: RewriteCvtFloatToIntegral("__ceil", PrimitiveType.Int32); break;
                case Mnemonic.clamps: RewriteClamps(); break;
                case Mnemonic.cust0: RewriteIntrinsicProc("__cust0"); break;
                case Mnemonic.cust1: RewriteIntrinsicProc("__cust1"); break;
                case Mnemonic.dhi: RewriteCacheFn("__dhi"); break;
                case Mnemonic.dhu: RewriteCacheFn("__dhu"); break;
                case Mnemonic.dhwb: RewriteCacheFn("__dhwb"); break;
                case Mnemonic.dhwbi: RewriteCacheFn("__dhwbi"); break;
                case Mnemonic.dii: RewriteCacheFn("__dii"); break;
                case Mnemonic.diu: RewriteCacheFn("__diu"); break;
                case Mnemonic.dpfr: RewriteCacheFn("__dpfr"); break;
                case Mnemonic.dpfro: RewriteCacheFn("__dpfro"); break;
                case Mnemonic.dpfw: RewriteCacheFn("__dpfw"); break;
                case Mnemonic.dpfwo: RewriteCacheFn("__dpfwo"); break;
                case Mnemonic.dsync: RewriteIntrinsicProc("__dsync"); break;
                case Mnemonic.esync: RewriteIntrinsicProc("__esync"); break;
                case Mnemonic.excw: RewriteIntrinsicProc("__excw"); break;
                case Mnemonic.extui: RewriteExtui(); break;
                case Mnemonic.entry: RewriteEntry(); break;
                case Mnemonic.float_s: RewriteFloat_s(PrimitiveType.Int32); break;
                case Mnemonic.floor_s: RewriteIntrinsicFn("__floor", false); break;
                case Mnemonic.iii: RewriteCacheFn("__iii"); break;
                case Mnemonic.iitlb: RewriteIntrinsicProc("__iitlb"); break;
                case Mnemonic.ipf: RewriteCacheFn("__ipf"); break;
                case Mnemonic.isync: RewriteIntrinsicProc("__isync"); break;
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
                case Mnemonic.ldpte: RewriteIntrinsicProc("__ldpte"); break;
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
                case Mnemonic.mul_aa_hh: RewriteMul("__mul_hh", Int40); break;
                case Mnemonic.mul_aa_hl: RewriteMul("__mul_hl", Int40); break;
                case Mnemonic.mul_aa_lh: RewriteMul("__mul_lh", Int40); break;
                case Mnemonic.mul_aa_ll: RewriteMul("__mul_ll", Int40); break;
                case Mnemonic.mul_ad_hh: RewriteMul("__mul_hh", Int40); break;
                case Mnemonic.mul_ad_hl: RewriteMul("__mul_hl", Int40); break;
                case Mnemonic.mul_ad_lh: RewriteMul("__mul_lh", Int40); break;
                case Mnemonic.mul_ad_ll: RewriteMul("__mul_ll", Int40); break;
                case Mnemonic.mul_da_hh: RewriteMul("__mul_hh", Int40); break;
                case Mnemonic.mul_da_hl: RewriteMul("__mul_hl", Int40); break;
                case Mnemonic.mul_da_lh: RewriteMul("__mul_lh", Int40); break;
                case Mnemonic.mul_da_ll: RewriteMul("__mul_ll", Int40); break;
                case Mnemonic.mul_dd_hh: RewriteMul("__mul_hh", Int40); break;
                case Mnemonic.mul_dd_hl: RewriteMul("__mul_hl", Int40); break;
                case Mnemonic.mul_dd_lh: RewriteMul("__mul_lh", Int40); break;
                case Mnemonic.mul_dd_ll: RewriteMul("__mul_ll", Int40); break;
                case Mnemonic.mula_aa_hh: RewriteMula("__mul_hh", Int40); break;
                case Mnemonic.mula_aa_hl: RewriteMula("__mul_hl", Int40); break;
                case Mnemonic.mula_aa_lh: RewriteMula("__mul_lh", Int40); break;
                case Mnemonic.mula_aa_ll: RewriteMula("__mul_ll", Int40); break;
                case Mnemonic.mula_ad_hh: RewriteMula("__mul_hh", Int40); break;
                case Mnemonic.mula_ad_hl: RewriteMula("__mul_hl", Int40); break;
                case Mnemonic.mula_ad_lh: RewriteMula("__mul_lh", Int40); break;
                case Mnemonic.mula_ad_ll: RewriteMula("__mul_ll", Int40); break;
                case Mnemonic.mula_da_hh: RewriteMula("__mul_hh", Int40); break;
                case Mnemonic.mula_da_hh_lddec: RewriteMulaIncDec("__mul_hh", Int40, -4); break;
                case Mnemonic.mula_da_hh_ldinc: RewriteMulaIncDec("__mul_hh", Int40, 4); break;
                case Mnemonic.mula_da_hl: RewriteMula("__mul_hl", Int40); break;
                case Mnemonic.mula_da_hl_lddec: RewriteMulaIncDec("__mul_hl", Int40, -4); break;
                case Mnemonic.mula_da_hl_ldinc: RewriteMulaIncDec("__mul_hl", Int40, 4); break;
                case Mnemonic.mula_da_lh: RewriteMula("__mul_lh", Int40); break;
                case Mnemonic.mula_da_lh_lddec: RewriteMulaIncDec("__mul_lh", Int40, -4); break;
                case Mnemonic.mula_da_lh_ldinc: RewriteMulaIncDec("__mul_lh", Int40, 4); break;
                case Mnemonic.mula_da_ll: RewriteMula("__mul_ll", Int40); break;
                case Mnemonic.mula_da_ll_lddec: RewriteMulaIncDec("__mul_ll", Int40, -4); break;
                case Mnemonic.mula_da_ll_ldinc: RewriteMulaIncDec("__mul_ll", Int40, 4); break;
                case Mnemonic.mula_dd_hh: RewriteMula("__mul_hh", Int40); break;
                case Mnemonic.mula_dd_hh_lddec: RewriteMulaIncDec("__mul_hh", Int40, -4); break;
                case Mnemonic.mula_dd_hl: RewriteMula("__mul_hl", Int40); break;
                case Mnemonic.mula_dd_lh: RewriteMula("__mul_lh", Int40); break;
                case Mnemonic.mula_dd_ll: RewriteMula("__mul_ll", Int40); break;
                case Mnemonic.mula_dd_ll_lddec: RewriteMulaIncDec("__mul_ll", Int40, -4); break;
                case Mnemonic.muls_aa_hh: RewriteMuls("__mul_hh", Int40); break;
                case Mnemonic.muls_aa_hl: RewriteMuls("__mul_hl", Int40); break;
                case Mnemonic.muls_aa_lh: RewriteMuls("__mul_lh", Int40); break;
                case Mnemonic.muls_aa_ll: RewriteMuls("__mul_ll", Int40); break;
                case Mnemonic.muls_ad_hh: RewriteMuls("__mul_hh", Int40); break;
                case Mnemonic.muls_ad_hl: RewriteMuls("__mul_hl", Int40); break;
                case Mnemonic.muls_ad_lh: RewriteMuls("__mul_lh", Int40); break;
                case Mnemonic.muls_ad_ll: RewriteMuls("__mul_ll", Int40); break;
                case Mnemonic.muls_da_hh: RewriteMuls("__mul_hh", Int40); break;
                case Mnemonic.muls_da_hl: RewriteMuls("__mul_hl", Int40); break;
                case Mnemonic.muls_da_lh: RewriteMuls("__mul_lh", Int40); break;
                case Mnemonic.muls_da_ll: RewriteMuls("__mul_ll", Int40); break;
                case Mnemonic.muls_dd_hh: RewriteMuls("__mul_hh", Int40); break;
                case Mnemonic.muls_dd_hl: RewriteMuls("__mul_hl", Int40); break;
                case Mnemonic.muls_dd_lh: RewriteMuls("__mul_lh", Int40); break;
                case Mnemonic.muls_dd_ll: RewriteMuls("__mul_ll", Int40); break;

                case Mnemonic.mul_s: RewriteBinOp(m.FMul); break;
                case Mnemonic.mul16s: RewriteMul16(m.SMul, Domain.SignedInt); break;
                case Mnemonic.mul16u: RewriteMul16(m.UMul, Domain.UnsignedInt); break;
                case Mnemonic.mull: RewriteBinOp(m.IMul); break;
                case Mnemonic.mulsh: RewriteMulh("__mulsh", PrimitiveType.Int32); break;
                case Mnemonic.muluh: RewriteMulh("__muluh", PrimitiveType.UInt32); break;
                case Mnemonic.neg: RewriteUnaryOp(m.Neg); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.nop_n: m.Nop(); break;
                case Mnemonic.nsa: RewriteIntrinsicFn("__nsa", true); break;
                case Mnemonic.nsau: RewriteIntrinsicFn("__nsau", true); break;
                case Mnemonic.oeq_s: RewriteBinOp(m.FEq); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ole_s: RewriteBinOp(m.FLe); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.olt_s: RewriteBinOp(m.FLt); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.or: RewriteOr(); break;
                case Mnemonic.orb: RewriteOr(); break;
                case Mnemonic.orbc: RewriteBinOp((a, b) => m.Or(a, m.Not(b))); break;
                case Mnemonic.pitlb: RewriteIntrinsicFn("__pitlb", true); break;
                case Mnemonic.quos: RewriteBinOp(m.SDiv); break;
                case Mnemonic.quou: RewriteBinOp(m.UDiv); break;
                case Mnemonic.rdtlb0: RewriteIntrinsicFn("__rdtlb0", true); break;
                case Mnemonic.rdtlb1: RewriteIntrinsicFn("__rdtlb1", true); break;
                case Mnemonic.rems: RewriteBinOp(m.Mod); break;
                case Mnemonic.remu: RewriteBinOp(m.Mod); break;
                case Mnemonic.ret:
                case Mnemonic.ret_n: RewriteRet(); break;
                case Mnemonic.rfe: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from exception?
                case Mnemonic.rfi: RewriteRet(); break;      //$REVIEW: emit some hint this is a return from interrupt?
                case Mnemonic.ritlb0: RewriteIntrinsicFn("__ritlb0", true); break;
                case Mnemonic.ritlb1: RewriteIntrinsicFn("__ritlb1", true); break;
                case Mnemonic.rotw: RewriteIntrinsicProc("__rotw"); break;
                case Mnemonic.round_s: RewriteCvtFloatToIntegral("__round", PrimitiveType.Int32); break;
                case Mnemonic.rsil: RewriteIntrinsicFn("__rsil", true); break;
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
                case Mnemonic.trunc_s: RewriteCvtFloatToIntegral("__trunc", PrimitiveType.Int32); break;
                case Mnemonic.ueq_s: RewriteBinOp(m.Eq); break;     //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ufloat_s: RewriteFloat_s(PrimitiveType.UInt32); break;
                case Mnemonic.ule_s: RewriteBinOp(m.FLe); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.ult_s: RewriteBinOp(m.FLt); break;    //$REVIEW: what to do about 'ordered' and 'unordered'
                case Mnemonic.umul_aa_hh: RewriteMul("__umul_hh", UInt40); break;
                case Mnemonic.umul_aa_hl: RewriteMul("__umul_hl", UInt40); break;
                case Mnemonic.umul_aa_lh: RewriteMul("__umul_lh", UInt40); break;
                case Mnemonic.umul_aa_ll: RewriteMul("__umul_ll", UInt40); break;
                case Mnemonic.un_s: RewriteIntrinsicFn("isunordered", false); break;
                case Mnemonic.utrunc_s: RewriteCvtFloatToIntegral("__utrunc", PrimitiveType.UInt32); break;
                case Mnemonic.waiti: RewriteIntrinsicProc("__waiti"); break;
                case Mnemonic.wdtlb: RewriteIntrinsicProc("__wdtlb"); break;
                case Mnemonic.witlb: RewriteIntrinsicProc("__witlb"); break;
                case Mnemonic.wer: RewriteWer(); break;
                case Mnemonic.wsr: RewriteWsr(); break;
                case Mnemonic.wur: RewriteInverseCopy(); break;
                case Mnemonic.xor: RewriteBinOp(m.Xor); break;
                case Mnemonic.xorb: RewriteBinOp(m.Xor); break;
                case Mnemonic.xsr: RewriteXsr(); break;
                }
                CheckForLoopExit();
                yield return m.MakeCluster(addr, len, iclass);
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
            if (lend == null)
                return;
            if (instr.Address.ToLinear() + (uint)instr.Length == lend.ToLinear())
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
            case AddressOperand aOp:
                return aOp.Address;
            case ImmediateOperand iOp:
                return iOp.Value;
            }
            throw new NotImplementedException(op.GetType().FullName);
        }

        // Sign-extend an operand known to be signed immediate.
        private Constant RewriteSimm(MachineOperand op)
        {
            var iOp = (ImmediateOperand)op;
            return Constant.Int32(iOp.Value.ToInt32());
        }

        // Zero-extend an operand known to be unsigned immediate.
        private Constant RewriteUimm(MachineOperand op)
        {
            var iOp = (ImmediateOperand)op;
            return Constant.UInt32(iOp.Value.ToUInt32());
        }
    }
}
