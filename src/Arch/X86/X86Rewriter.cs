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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using IEnumerable = System.Collections.IEnumerable;
using IEnumerator = System.Collections.IEnumerator;
using ProcedureCharacteristics = Reko.Core.Serialization.ProcedureCharacteristics;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Rewrites x86 instructions into a stream of low-level RTL-like instructions.
    /// </summary>
    public partial class X86Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IntelArchitecture arch;
        private readonly IRewriterHost host;
        private readonly IStorageBinder binder;
        private readonly X86State state;
        private readonly EndianImageReader rdr;
        private readonly LookaheadEnumerator<X86Instruction> dasm;
        private readonly RtlEmitter m;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly OperandRewriter orw;
        private X86Instruction instrCur;
        private InstrClass iclass;
        private int len;

        public X86Rewriter(
            IntelArchitecture arch,
            IRewriterHost host,
            X86State state,
            EndianImageReader rdr,
            IStorageBinder binder)
        {
            this.arch = arch;
            this.host = host ?? throw new ArgumentNullException("host");
            this.state = state;
            this.rdr = rdr;
            this.binder = binder;
            this.dasm = new LookaheadEnumerator<X86Instruction>(arch.CreateDisassemblerImpl(rdr));
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.orw = arch.ProcessorMode.CreateOperandRewriter(arch, m, binder, host);
            this.instrCur = default!;
        }

        /// <summary>
        /// Iterator that yields one RtlIntructionCluster for each x86 instruction.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCur = dasm.Current;
                var addr = instrCur.Address;
                this.iclass = instrCur.InstructionClass;
                switch (instrCur.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    host.Warn(
                        dasm.Current.Address,
                        "x86 instruction '{0}' is not supported yet.",
                        instrCur.Mnemonic);
                    goto case Mnemonic.illegal;
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.aaa: RewriteAaa(); break;
                case Mnemonic.aad: RewriteAad(); break;
                case Mnemonic.aam: RewriteAam(); break;
                case Mnemonic.aas: RewriteAas(); break;
                case Mnemonic.adc: RewriteAdcSbb(m.IAdd); break;
                case Mnemonic.adcx: RewriteAdcx(Registers.C); break;
                case Mnemonic.add: RewriteAddSub(m.IAdd); break;
                case Mnemonic.addss: RewriteScalarBinop(m.FAdd, PrimitiveType.Real32, false); break;
                case Mnemonic.vaddss: RewriteScalarBinop(m.FAdd, PrimitiveType.Real32, true); break;
                case Mnemonic.addsd: RewriteScalarBinop(m.FAdd, PrimitiveType.Real64, false); break;
                case Mnemonic.vaddsd: RewriteScalarBinop(m.FAdd, PrimitiveType.Real64, true); break;
                case Mnemonic.addps: RewritePackedBinop(false, addp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.addpd: RewritePackedBinop(false, addp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.adox: RewriteAdcx(Registers.O); break;
                case Mnemonic.aesenc: RewriteAesenc(false); break;
                case Mnemonic.vaesenc: RewriteAesenc(true); break;
                case Mnemonic.aesimc: RewriteAesimc(); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.andn: RewriteLogical((a, b) => m.And(b, m.Comp(a))); break;
                case Mnemonic.andnpd: RewriteAndnp_(andnpd_intrinsic); break;
                case Mnemonic.andnps: RewriteAndnp_(andnps_intrinsic); break;
                case Mnemonic.andpd: RewritePackedBinop(false, andp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.andps: RewritePackedBinop(false, andp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.arpl: RewriteArpl(); break;
                case Mnemonic.bextr: RewriteBextr(); break;
                case Mnemonic.blsi: RewriteBlsi(); break;
                case Mnemonic.blsmsk: RewriteBlsmsk(); break;
                case Mnemonic.blsr: RewriteBlsr(); break;
                case Mnemonic.bound: RewriteBound(); break;
                case Mnemonic.bsf: RewriteBsf(); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.bswap: RewriteBswap(); break;
                case Mnemonic.bt: RewriteBt(); break;
                case Mnemonic.btc: RewriteBtc(); break;
                case Mnemonic.btr: RewriteBtr(); break;
                case Mnemonic.bts: RewriteBts(); break;
                case Mnemonic.bzhi: RewriteBzhi(); break;
                case Mnemonic.call: RewriteCall(instrCur.Operands[0], instrCur.Operands[0].Width); break;
                case Mnemonic.cbw: RewriteCbw(); break;
                case Mnemonic.cdq: RewriteCdq(); break;
                case Mnemonic.cdqe: RewriteCdqe(); break;
                case Mnemonic.cqo: RewriteCqo(); break;
                case Mnemonic.clc: RewriteSetFlag(Registers.C, Constant.False()); break;
                case Mnemonic.cld: RewriteSetFlag(Registers.D, Constant.False()); break;
                case Mnemonic.cli: RewriteCli(); break;
                case Mnemonic.clts: RewriteClts(); break;
                case Mnemonic.cmc: m.Assign(binder.EnsureFlagGroup(Registers.C), m.Not(binder.EnsureFlagGroup(Registers.C))); break;
                case Mnemonic.cmova: RewriteConditionalMove(ConditionCode.UGT, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovbe: RewriteConditionalMove(ConditionCode.ULE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovc: RewriteConditionalMove(ConditionCode.ULT, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovge: RewriteConditionalMove(ConditionCode.GE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovg: RewriteConditionalMove(ConditionCode.GT, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovl: RewriteConditionalMove(ConditionCode.LT, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovle: RewriteConditionalMove(ConditionCode.LE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovnc: RewriteConditionalMove(ConditionCode.UGE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovno: RewriteConditionalMove(ConditionCode.NO, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovns: RewriteConditionalMove(ConditionCode.NS, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovnz: RewriteConditionalMove(ConditionCode.NE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovo: RewriteConditionalMove(ConditionCode.OV, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovpe: RewriteConditionalMove(ConditionCode.PE, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovpo: RewriteConditionalMove(ConditionCode.PO, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovs: RewriteConditionalMove(ConditionCode.SG, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmovz: RewriteConditionalMove(ConditionCode.EQ, instrCur.Operands[0], instrCur.Operands[1]); break;
                case Mnemonic.cmpxchg: RewriteCmpxchg(); break;
                case Mnemonic.cmpxchg8b: RewriteCmpxchgNb(cmpxchgN_intrinsic, Registers.edx, Registers.eax, Registers.ecx, Registers.ebx); break;
                case Mnemonic.cmpxchg16b: RewriteCmpxchgNb(cmpxchgN_intrinsic, Registers.rdx, Registers.rax, Registers.rcx, Registers.rbx); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmps: RewriteStringInstruction(); break;
                case Mnemonic.cmppd: RewriteCmpp(false, cmpp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.cmpps: RewriteCmpp(false, cmpp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.cmpsb: RewriteStringInstruction(); break;
                case Mnemonic.cmpsd:
                case Mnemonic.vcmpsd: RewriteCmpsd(PrimitiveType.Real64); break;
                case Mnemonic.cmpss:
                case Mnemonic.vcmpss: RewriteCmpsd(PrimitiveType.Real32); break;
                case Mnemonic.comisd: RewriteComis(PrimitiveType.Real64); break;
                case Mnemonic.comiss: RewriteComis(PrimitiveType.Real32); break;
                case Mnemonic.cpuid: RewriteCpuid(); break;
                case Mnemonic.cvtdq2ps: RewriteCvtps2pi(cvtdq2ps_intrinsic, PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Mnemonic.cvtdq2pd: RewriteCvtps2pi(cvtdq2pd_intrinsic, PrimitiveType.Int32, PrimitiveType.Real64); break;
                case Mnemonic.cvtpd2dq: RewriteCvtps2pi(cvtpd2dq_intrinsic, PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvtpd2ps: RewriteCvtps2pi(cvtpd2ps_intrinsic, PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.cvtpi2ps: RewriteCvtPackedToReal(PrimitiveType.Real32); break;
                case Mnemonic.cvtps2dq: RewriteCvtps2pi(cvtps2dq_intrinsic, PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvtps2pi: RewriteCvtps2pi(cvtps2pi_intrinsic, PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvtps2pd: RewriteCvtps2pi(cvtps2pd_intrinsic, PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.cvtsd2si: RewriteCvts2si(PrimitiveType.Real64); break;
                case Mnemonic.cvtsd2ss: RewriteCvtToReal(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.cvtsi2ss:
                case Mnemonic.vcvtsi2ss: RewriteCvtIntToReal(PrimitiveType.Real32); break;
                case Mnemonic.cvtsi2sd:
                case Mnemonic.vcvtsi2sd: RewriteCvtIntToReal(PrimitiveType.Real64); break;
                case Mnemonic.cvtss2sd: RewriteCvtToReal(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.cvtss2si: RewriteCvts2si(PrimitiveType.Real32); break;
                case Mnemonic.cvttsd2si: RewriteCvtts2si(PrimitiveType.Real64); break;
                case Mnemonic.cvttss2si: RewriteCvtts2si(PrimitiveType.Real32); break;
                case Mnemonic.cvttpd2dq: RewriteCvttps2pi(cvttpd2dq_intrinsic, PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvttps2dq: RewriteCvttps2pi(cvttps2dq_intrinsic, PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvttps2pi: RewriteCvttps2pi(); break;
                case Mnemonic.cwd: RewriteCwd(); break;
                case Mnemonic.cwde: RewriteCwde(); break;
                case Mnemonic.daa: EmitDaaDas(daa_intrinsic); break;
                case Mnemonic.das: EmitDaaDas(das_intrinsic); break;
                case Mnemonic.dec: RewriteIncDec(-1); break;
                case Mnemonic.div: RewriteDivide(m.UDiv, Domain.UnsignedInt); break;
                case Mnemonic.divpd: RewritePackedBinop(false, divp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.divps: RewritePackedBinop(false, divp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.divsd: RewriteScalarBinop(m.FDiv, PrimitiveType.Real64, false); break;
                case Mnemonic.vdivsd: RewriteScalarBinop(m.FDiv, PrimitiveType.Real64, true); break;
                case Mnemonic.divss: RewriteScalarBinop(m.FDiv, PrimitiveType.Real32, false); break;
                case Mnemonic.vdivss: RewriteScalarBinop(m.FDiv, PrimitiveType.Real32, true); break;
                case Mnemonic.emms: RewriteEmms(); break;
                case Mnemonic.enter: RewriteEnter(); break;
                case Mnemonic.endbr32:
                case Mnemonic.endbr64: RewriteEndbr(); break;
                case Mnemonic.f2xm1: RewriteF2xm1(); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: EmitCommonFpuInstruction(m.FAdd, false, false); break;
                case Mnemonic.faddp: EmitCommonFpuInstruction(m.FAdd, false, true); break;
                case Mnemonic.fbld: RewriteFbld(); break;
                case Mnemonic.fbstp: RewriteFbstp(); break;
                case Mnemonic.fchs: EmitFchs(); break;
                case Mnemonic.fclex: RewriteFclex(); break;
                case Mnemonic.fcmovb: RewriteFcmov(Registers.C, ConditionCode.GE); break;
                case Mnemonic.fcmovbe: RewriteFcmov(Registers.CZ, ConditionCode.GT); break;
                case Mnemonic.fcmove: RewriteFcmov(Registers.Z, ConditionCode.NE); break;
                case Mnemonic.fcmovnb: RewriteFcmov(Registers.CZ, ConditionCode.GE); break;
                case Mnemonic.fcmovnbe: RewriteFcmov(Registers.CZ, ConditionCode.LE); break;
                case Mnemonic.fcmovne: RewriteFcmov(Registers.Z, ConditionCode.EQ); break;
                case Mnemonic.fcmovnu: RewriteFcmov(Registers.P, ConditionCode.IS_NAN); break;
                case Mnemonic.fcmovu: RewriteFcmov(Registers.P, ConditionCode.NOT_NAN); break;
                case Mnemonic.fcom: RewriteFcom(0); break;
                case Mnemonic.fcomi: RewriteFcomi(false); break;
                case Mnemonic.fcomip: RewriteFcomi(true); break;
                case Mnemonic.fcomp: RewriteFcom(1); break;
                case Mnemonic.fcompp: RewriteFcom(2); break;
                case Mnemonic.fcos: RewriteFUnary(cos_intrinsic); break;
                case Mnemonic.fdecstp: RewriteFdecstp(); break;
                case Mnemonic.fdiv: EmitCommonFpuInstruction(m.FDiv, false, false); break;
                case Mnemonic.fdivp: EmitCommonFpuInstruction(m.FDiv, false, true); break;
                case Mnemonic.femms: RewriteFemms(); break;
                case Mnemonic.ffree: RewriteFfree(false); break;
                case Mnemonic.ffreep: RewriteFfree(true); break;
                case Mnemonic.fiadd: EmitCommonFpuInstruction(m.FAdd, false, false, Domain.SignedInt); break;
                case Mnemonic.ficom: RewriteFicom(false); break;
                case Mnemonic.ficomp: RewriteFicom(true); break;
                case Mnemonic.fimul: EmitCommonFpuInstruction(m.FMul, false, false, Domain.SignedInt); break;
                case Mnemonic.fisub: EmitCommonFpuInstruction(m.FSub, false, false, Domain.SignedInt); break;
                case Mnemonic.fisubr: EmitCommonFpuInstruction(m.FSub, true, false, Domain.SignedInt); break;
                case Mnemonic.fidiv: EmitCommonFpuInstruction(m.FDiv, false, false, Domain.SignedInt); break;
                case Mnemonic.fidivr: EmitCommonFpuInstruction(m.FDiv, true, false, Domain.SignedInt); break;
                case Mnemonic.fdivr: EmitCommonFpuInstruction(m.FDiv, true, false); break;
                case Mnemonic.fdivrp: EmitCommonFpuInstruction(m.FDiv, true, true); break;
                case Mnemonic.fild: RewriteFild(); break;
                case Mnemonic.fincstp: RewriteFincstp(); break;
                case Mnemonic.fist: RewriteFist(false); break;
                case Mnemonic.fistp: RewriteFist(true); break;
                case Mnemonic.fisttp: RewriteFistt(true); break;
                case Mnemonic.fld: RewriteFld(); break;
                case Mnemonic.fld1: RewriteFldConst(1.0); break;
                case Mnemonic.fldcw: RewriteFldcw(); break;
                case Mnemonic.fldenv: RewriteFldenv(); break;
                case Mnemonic.fldl2e: RewriteFldConst(Constant.LgE()); break;
                case Mnemonic.fldl2t: RewriteFldConst(Constant.Lg10()); break;
                case Mnemonic.fldlg2: RewriteFldConst(Constant.Log2()); break;
                case Mnemonic.fldln2: RewriteFldConst(Constant.Ln2()); break;
                case Mnemonic.fldpi: RewriteFldConst(Constant.Pi()); break;
                case Mnemonic.fldz: RewriteFldConst(0.0); break;
                case Mnemonic.fmul: EmitCommonFpuInstruction(m.FMul, false, false); break;
                case Mnemonic.fmulp: EmitCommonFpuInstruction(m.FMul, false, true); break;
                case Mnemonic.fndisi: m.Nop(); break;    //$TODO: for 8086, this needs to emit some code.
                case Mnemonic.fneni: m.Nop(); break;    //$TODO: for 8086, this needs to emit some code.
                case Mnemonic.fninit: RewriteFninit(); break;
                case Mnemonic.fnop: m.Nop(); break;
                case Mnemonic.fnsetpm: m.Nop(); break;
                case Mnemonic.fpatan: RewriteFpatan(); break;
                case Mnemonic.fprem: RewriteFprem(); break;
                case Mnemonic.fprem1: RewriteFprem1(); break;
                case Mnemonic.fptan: RewriteFptan(); break;
                case Mnemonic.frndint: RewriteFUnary(rndint_intrinsic); break;
                case Mnemonic.frstor: RewriteFrstor(); break;
                case Mnemonic.fsave: RewriteFsave(); break;
                case Mnemonic.fscale: RewriteFscale(); break;
                case Mnemonic.fsin: RewriteFUnary(sin_intrinsic); break;
                case Mnemonic.fsincos: RewriteFsincos(); break;
                case Mnemonic.fsqrt: RewriteFUnary(sqrt_intrinsic); break;
                case Mnemonic.fst: RewriteFst(false); break;
                case Mnemonic.fstenv: RewriteFstenv(); break;
                case Mnemonic.fstcw: RewriterFstcw(); break;
                case Mnemonic.fstp: RewriteFst(true); break;
                case Mnemonic.fstsw: RewriteFstsw(); break;
                case Mnemonic.fsub: EmitCommonFpuInstruction(m.FSub, false, false); break;
                case Mnemonic.fsubp: EmitCommonFpuInstruction(m.FSub, false, true); break;
                case Mnemonic.fsubr: EmitCommonFpuInstruction(m.FSub, true, false); break;
                case Mnemonic.fsubrp: EmitCommonFpuInstruction(m.FSub, true, true); break;
                case Mnemonic.ftst: RewriteFtst(); break;
                case Mnemonic.fucom: RewriteFcom(0); break;
                case Mnemonic.fucomp: RewriteFcom(1); break;
                case Mnemonic.fucompp: RewriteFcom(2); break;
                case Mnemonic.fucomi: RewriteFcomi(false); break;
                case Mnemonic.fucomip: RewriteFcomi(true); break;
                case Mnemonic.fxam: RewriteFxam(); break;
                case Mnemonic.fxch: RewriteExchange(); break;
                case Mnemonic.fxrstor: RewriteFxrstor(); break;
                case Mnemonic.fxsave: RewriteFxsave(); break;
                case Mnemonic.fxtract: RewriteFxtract(); break;
                case Mnemonic.fyl2x: RewriteFyl2x(); break;
                case Mnemonic.fyl2xp1: RewriteFyl2xp1(); break;
                case Mnemonic.getsec: RewriteGetsec(); break;
                case Mnemonic.hlt: RewriteHlt(); break;
                case Mnemonic.icebp: RewriteIcebp(); break;
                case Mnemonic.idiv: RewriteDivide(m.SDiv, Domain.SignedInt); break;
                case Mnemonic.@in: RewriteIn(); break;
                case Mnemonic.imul: RewriteMultiply(m.SMul, Domain.SignedInt); break;
                case Mnemonic.inc: RewriteIncDec(1); break;
                case Mnemonic.insb: RewriteStringInstruction(); break;
                case Mnemonic.ins: RewriteStringInstruction(); break;
                case Mnemonic.invlpg: RewriteInvlpg(); break;
                case Mnemonic.@int: RewriteInt(); break;
                case Mnemonic.into: RewriteInto(); break;
                case Mnemonic.invd: RewriteInvd(); break;
                case Mnemonic.iret: RewriteIret(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jmpe: RewriteJmpe(); break;
                case Mnemonic.ja: RewriteConditionalGoto(ConditionCode.UGT, instrCur.Operands[0]); break;
                case Mnemonic.jbe: RewriteConditionalGoto(ConditionCode.ULE, instrCur.Operands[0]); break;
                case Mnemonic.jc: RewriteConditionalGoto(ConditionCode.ULT, instrCur.Operands[0]); break;
                case Mnemonic.jcxz: RewriteJcxz(Registers.cx); break;
                case Mnemonic.jecxz: RewriteJcxz(Registers.ecx); break;
                case Mnemonic.jge: RewriteConditionalGoto(ConditionCode.GE, instrCur.Operands[0]); break;
                case Mnemonic.jg: RewriteConditionalGoto(ConditionCode.GT, instrCur.Operands[0]); break;
                case Mnemonic.jl: RewriteConditionalGoto(ConditionCode.LT, instrCur.Operands[0]); break;
                case Mnemonic.jle: RewriteConditionalGoto(ConditionCode.LE, instrCur.Operands[0]); break;
                case Mnemonic.jnc: RewriteConditionalGoto(ConditionCode.UGE, instrCur.Operands[0]); break;
                case Mnemonic.jno: RewriteConditionalGoto(ConditionCode.NO, instrCur.Operands[0]); break;
                case Mnemonic.jns: RewriteConditionalGoto(ConditionCode.NS, instrCur.Operands[0]); break;
                case Mnemonic.jnz: RewriteConditionalGoto(ConditionCode.NE, instrCur.Operands[0]); break;
                case Mnemonic.jo: RewriteConditionalGoto(ConditionCode.OV, instrCur.Operands[0]); break;
                case Mnemonic.jpe: RewriteConditionalGoto(ConditionCode.PE, instrCur.Operands[0]); break;
                case Mnemonic.jpo: RewriteConditionalGoto(ConditionCode.PO, instrCur.Operands[0]); break;
                case Mnemonic.jrcxz: RewriteJcxz(Registers.rcx); break;
                case Mnemonic.js: RewriteConditionalGoto(ConditionCode.SG, instrCur.Operands[0]); break;
                case Mnemonic.jz: RewriteConditionalGoto(ConditionCode.EQ, instrCur.Operands[0]); break;
                case Mnemonic.lahf: RewriteLahf(); break;
                case Mnemonic.lar: RewriteLar(); break;
                case Mnemonic.lds: RewriteLxs(Registers.ds); break;
                case Mnemonic.ldmxcsr: RewriteLdmxcsr(); break;
                case Mnemonic.stmxcsr: RewriteStmxcsr(); break;
                case Mnemonic.lea: RewriteLea(); break;
                case Mnemonic.leave: RewriteLeave(); break;
                case Mnemonic.les: RewriteLxs(Registers.es); break;
                case Mnemonic.lfence: RewriteLfence(); break;
                case Mnemonic.lfs: RewriteLxs(Registers.fs); break;
                case Mnemonic.lgs: RewriteLxs(Registers.gs); break;
                case Mnemonic.lgdt: RewriteLxdt(lgdt_intrinsic); break;
                case Mnemonic.lidt: RewriteLxdt(lidt_intrinsic); break;
                case Mnemonic.lldt: RewriteLxdt(lldt_intrinsic); break;
                case Mnemonic.lmsw: RewriteLmsw(); break;
                case Mnemonic.@lock: RewriteLock(); break;
                case Mnemonic.lods: RewriteStringInstruction(); break;
                case Mnemonic.lodsb: RewriteStringInstruction(); break;
                case Mnemonic.loop: RewriteLoop(null, ConditionCode.EQ); break;
                case Mnemonic.loope: RewriteLoop(Registers.Z, ConditionCode.EQ); break;
                case Mnemonic.loopne: RewriteLoop(Registers.Z, ConditionCode.NE); break;
                case Mnemonic.lsl: RewriteLsl(); break;
                case Mnemonic.lss: RewriteLxs(Registers.ss); break;
                case Mnemonic.ltr: RewriteLtr(); break;
                case Mnemonic.lzcnt: RewriteLeadingTrailingZeros(lzcnt_intrinsic); break;
                case Mnemonic.maskmovdqu: RewriteMaskmov(false, maskmovdqu_intrinsic); break;
                case Mnemonic.maskmovq: RewriteMaskmov(false, maskmovq_intrinsic); break;
                case Mnemonic.maxpd: RewritePackedBinop(false, maxp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.maxps: RewritePackedBinop(false, maxp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.maxsd: RewriteMaxMinsd(max_intrinsic, PrimitiveType.Real64, false); break;
                case Mnemonic.vmaxsd: RewriteMaxMinsd(max_intrinsic, PrimitiveType.Real64, true); break;
                case Mnemonic.maxss: RewriteMaxMinsd(fmax_intrinsic, PrimitiveType.Real32, false); break;
                case Mnemonic.vmaxss: RewriteMaxMinsd(fmax_intrinsic, PrimitiveType.Real32, true); break;
                case Mnemonic.mfence: RewriteMfence(); break;
                case Mnemonic.minpd: RewritePackedBinop(false, minp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.vminpd: RewritePackedBinop(true, minp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.minps: RewritePackedBinop(false, minp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.vminps: RewritePackedBinop(true, minp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.minsd: RewriteMaxMinsd(min_intrinsic, PrimitiveType.Real64, false); break;
                case Mnemonic.vminsd: RewriteMaxMinsd(min_intrinsic, PrimitiveType.Real64, true); break;
                case Mnemonic.minss: RewriteMaxMinsd(fmin_intrinsic, PrimitiveType.Real32, false); break;
                case Mnemonic.vminss: RewriteMaxMinsd(fmin_intrinsic, PrimitiveType.Real32, true); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movapd:
                case Mnemonic.movaps:
                case Mnemonic.vmovapd:
                case Mnemonic.vmovaps: RewriteMov(); break;
                case Mnemonic.vmread: RewriteVmread(); break;
                case Mnemonic.vmwrite: RewriteVmwrite(); break;
                case Mnemonic.movbe: RewriteMovbe(); break;
                case Mnemonic.movd: case Mnemonic.vmovd: RewriteMovdq(); break;
                case Mnemonic.movdqa:
                case Mnemonic.vmovdqa:
                case Mnemonic.movdqu:
                case Mnemonic.vmovdqu:
                    RewriteMov(); break;
                case Mnemonic.movhpd: RewritePackedUnaryop(movhp_intrinsic, PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.movhps: RewritePackedUnaryop(movhp_intrinsic, PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.movlpd: RewritePackedUnaryop(movlp_intrinsic, PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.movlps: RewritePackedUnaryop(movlp_intrinsic, PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.movlhps: RewriteMovlhps(); break;
                case Mnemonic.movmskpd: RewriteMovmsk(false, movmskpd_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.movmskps: RewriteMovmsk(false, movmskps_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.movnti: RewriteMov(); break;
                case Mnemonic.movntps: RewriteMov(); break;
                case Mnemonic.movntq: RewriteMov(); break;
                case Mnemonic.movq: case Mnemonic.vmovq: RewriteMovdq(); break;
                case Mnemonic.movs: RewriteStringInstruction(); break;
                case Mnemonic.movsb: RewriteStringInstruction(); break;
                case Mnemonic.movsd:
                case Mnemonic.vmovsd: RewriteMovssd(PrimitiveType.Real64); break;
                case Mnemonic.movss:
                case Mnemonic.vmovss: RewriteMovssd(PrimitiveType.Real32); break;
                case Mnemonic.movsx: RewriteMovsx(); break;
                case Mnemonic.movsxd: RewriteMovsx(); break;
                    //$REVIEW: these are unaligned moves, do we want an intrinsic for them?
                case Mnemonic.movups: case Mnemonic.vmovups: RewriteMov(); break;
                case Mnemonic.movupd: RewriteMov(); break;
                case Mnemonic.movzx: RewriteMovzx(); break;
                case Mnemonic.mul: RewriteMultiply(m.UMul, Domain.UnsignedInt); break;
                case Mnemonic.mulpd: RewritePackedBinop(false, mulp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.mulps: RewritePackedBinop(false, mulp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.mulsd: RewriteScalarBinop(m.FMul, PrimitiveType.Real64, false); break;
                case Mnemonic.vmulsd: RewriteScalarBinop(m.FMul, PrimitiveType.Real64, true); break;
                case Mnemonic.mulss: RewriteScalarBinop(m.FMul, PrimitiveType.Real32, false); break;
                case Mnemonic.vmulss: RewriteScalarBinop(m.FMul, PrimitiveType.Real32, true); break;
                case Mnemonic.mulx: RewriteMulx(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not: RewriteNot(); break;
                case Mnemonic.or: RewriteLogical(m.Or); break;
                case Mnemonic.orpd: RewritePackedBinop(false, orp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.orps: RewritePackedBinop(false, orp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.@out: RewriteOut(); break;
                case Mnemonic.@outs: RewriteStringInstruction(); break;
                case Mnemonic.@outsb: RewriteStringInstruction(); break;
                case Mnemonic.packsswb: RewritePackedBinop(false, packss_intrinsic, PrimitiveType.Int16, PrimitiveType.Int8); break;
                case Mnemonic.packssdw: RewritePackedBinop(false, packss_intrinsic, PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.packuswb: RewritePackedBinop(false, packus_intrinsic, PrimitiveType.UInt16,PrimitiveType.UInt8); break;
                case Mnemonic.paddb: RewritePackedBinop(false, padd_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.paddd: RewritePackedBinop(false, padd_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.paddq: RewritePackedBinop(false, padd_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.vpaddq: RewritePackedBinop(true, padd_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.paddsw: RewritePackedBinop(false, padds_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.paddsb: RewritePackedBinop(false, padds_intrinsic, PrimitiveType.SByte); break;
                case Mnemonic.paddusb: RewritePackedBinop(false, paddus_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.paddusw: RewritePackedBinop(false, paddus_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.paddw: RewritePackedBinop(false, padd_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.pand: case Mnemonic.vpand: RewritePackedLogical(pand_intrinsic); break;
                case Mnemonic.pandn: case Mnemonic.vpandn: RewritePackedLogical(pandn_intrinsic); break;
                case Mnemonic.pause: RewritePause(); break;
                case Mnemonic.palignr: RewritePalignr(); break;
                case Mnemonic.pavgb: RewritePavg(pavg_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pavgw: RewritePavg(pavg_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.vpbroadcastb: RewritePbroadcast(true, pbroadcast_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pcmpeqb: case Mnemonic.vpcmpeqb: RewritePcmp(pcmpeq_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pcmpeqd: RewritePcmp(pcmpeq_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.pcmpeqw: RewritePcmp(pcmpeq_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.pcmpgtb: RewritePcmp(pcmpgt_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pcmpgtd: RewritePcmp(pcmpgt_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.pcmpgtw: RewritePcmp(pcmpgt_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.pdep: RewritePdep(); break;
                case Mnemonic.pext: RewritePext(); break;
                case Mnemonic.pextrw: case Mnemonic.vextrw:  RewritePextrw(); break;
                case Mnemonic.pinsrd: RewritePinsr(false, pinsr_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.vpinsrd: RewritePinsr(true, pinsr_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.pinsrw: RewritePinsr(false, pinsr_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.vpinsrw: RewritePinsr(true, pinsr_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.vpmaddubsw: RewritePmaddUbSw(true, pmaddubsw_intrinsic); break;
                case Mnemonic.pmaddwd: RewritePackedBinop(false, pmaddwd_intrinsic, PrimitiveType.Word16, PrimitiveType.Word32); break;
                case Mnemonic.pmaxsw: RewritePackedBinop(false, pmaxs_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.pmaxub: RewritePackedBinop(false, pmaxu_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.vpmaxub: RewritePackedBinop(true, pmaxu_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.pminsw: RewritePackedBinop(false, pmins_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.pminub: RewritePackedBinop(false, pminu_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.pmovmskb: RewriteMovmsk(false, pmovmskb_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.vpmovmskb: RewriteMovmsk(true, pmovmskb_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pmulhuw: RewritePackedBinop(false, pmulhu_intrinsic, PrimitiveType.UInt16, PrimitiveType.UInt16); break;
                case Mnemonic.pmulhw: RewritePackedBinop(false, pmulh_intrinsic, PrimitiveType.Int16, PrimitiveType.Int16); break;
                case Mnemonic.pmullw: RewritePackedBinop(false, pmull_intrinsic, PrimitiveType.Int16, PrimitiveType.Int16); break;
                case Mnemonic.vpmullw: RewritePackedBinop(true, pmull_intrinsic, PrimitiveType.Int16, PrimitiveType.Int16); break;
                case Mnemonic.pmuludq: RewritePackedBinop(false, pmulu_intrinsic, PrimitiveType.UInt32, PrimitiveType.UInt64); break;
                case Mnemonic.prefetchnta: RewritePrefetch(prefetchnta_intrinsic); break;
                case Mnemonic.prefetcht0: RewritePrefetch(prefetcht0_intrinsic); break;
                case Mnemonic.prefetcht1: RewritePrefetch(prefetcht1_intrinsic); break;
                case Mnemonic.prefetcht2: RewritePrefetch(prefetcht2_intrinsic); break;
                case Mnemonic.prefetchw: RewritePrefetch(prefetchw_intrinsic); break;
                case Mnemonic.psadbw: RewritePackedBinop(false, psadbw_intrinsic, PrimitiveType.Byte, PrimitiveType.Word16); break;
                case Mnemonic.pshufb: RewritePackedTernaryop(false, pshufb_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pshuflw: RewritePackedTernaryop(false, pshuflw_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.pslld: RewritePackedShift(psll_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.vpslld: RewritePackedShift(psll_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.psllq: RewritePackedShift(psll_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.vpsllq: RewritePackedShift(psll_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.psllw: RewritePackedShift(psll_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.vpsllw: RewritePackedShift(psll_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.psrad: RewritePackedShift(psra_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.vpsrad: RewritePackedShift(psra_intrinsic, PrimitiveType.Int32); break;
                case Mnemonic.psraw: RewritePackedShift(psra_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.vpsraw: RewritePackedShift(psra_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.psrld: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.vpsrld: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.psrldq: RewritePsrldq(false); break;
                case Mnemonic.psrlq: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.vpsrlq: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.psrlw: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.vpsrlw: RewritePackedShift(psrl_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.popa: RewritePopa(); break;
                case Mnemonic.popf: RewritePopf(); break;
                case Mnemonic.popcnt: RewritePopcnt(); break;
                case Mnemonic.por:
                case Mnemonic.vpor: RewritePor(); break;
                case Mnemonic.pshufd: RewritePshuf(pshuf_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.pshufw: RewritePshuf(pshuf_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.psubb: RewritePackedBinop(false, psub_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.psubd: RewritePackedBinop(false, psub_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.vpsubd: RewritePackedBinop(true, psub_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.psubq: RewritePackedBinop(false, psub_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.psubsb: RewritePackedBinop(false, psubs_intrinsic, PrimitiveType.SByte); break;
                case Mnemonic.psubsw: RewritePackedBinop(false, psubs_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.vpsubsw: RewritePackedBinop(true, psubs_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.psubusb: RewritePackedBinop(false, psubus_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.psubusw: RewritePackedBinop(false, psubus_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.psubw: RewritePackedBinop(false, psub_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.punpckhbw: RewritePunpckhbw(); break;
                case Mnemonic.punpckhdq: RewritePunpckhdq(); break;
                case Mnemonic.punpckhwd: RewritePunpckhwd(); break;
                case Mnemonic.punpcklbw: RewritePunpcklbw(); break;
                case Mnemonic.punpckldq: RewritePunpckldq(); break;
                case Mnemonic.punpcklqdq: RewritePunpcklqdq(); break;
                case Mnemonic.punpcklwd: RewritePunpcklwd(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.pusha: RewritePusha(); break;
                case Mnemonic.pushf: RewritePushf(); break;
                case Mnemonic.pxor: case Mnemonic.vpxor: RewritePxor(); break;
                case Mnemonic.rcl: RewriteRotation(CommonOps.RolC, true, true); break;
                case Mnemonic.rcpps: RewritePackedUnaryop(rcpp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.rcr: RewriteRotation(CommonOps.RorC, true, false); break;
                case Mnemonic.rol: RewriteRotation(CommonOps.Rol, false, true); break;
                case Mnemonic.ror: RewriteRotation(CommonOps.Ror, false, false); break;
                case Mnemonic.rorx: RewriteRorx(); break;
                case Mnemonic.rdmsr: RewriteRdmsr(); break;
                case Mnemonic.rdpmc: RewriteRdpmc(); break;
                case Mnemonic.rdrand: RewriteRdrand(); break;
                case Mnemonic.rdtsc: RewriteRdtsc(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.retf: RewriteRet(); break;
                case Mnemonic.roundsd: RewriteRoundsx(false, PrimitiveType.Real64); break;
                case Mnemonic.roundss: RewriteRoundsx(false, PrimitiveType.Real32); break;
                case Mnemonic.vroundsd: RewriteRoundsx(true, PrimitiveType.Real64); break;
                case Mnemonic.vroundss: RewriteRoundsx(true, PrimitiveType.Real32); break;
                case Mnemonic.rsm: RewriteRsm(); break;
                case Mnemonic.rsqrtps: RewritePackedUnaryop(rsqrtp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.sahf: m.Assign(binder.EnsureFlagGroup(X86Instruction.DefCc(instrCur.Mnemonic)!), orw.AluRegister(Registers.ah)); break;
                case Mnemonic.sar: RewriteBinOp(m.Sar); break;
                case Mnemonic.sarx: RewriteBinOp(m.Sar); break;
                case Mnemonic.sbb: RewriteAdcSbb(m.ISub); break;
                case Mnemonic.scas: RewriteStringInstruction(); break;
                case Mnemonic.scasb: RewriteStringInstruction(); break;
                case Mnemonic.seta: RewriteSet(ConditionCode.UGT); break;
                case Mnemonic.setc: RewriteSet(ConditionCode.ULT); break;
                case Mnemonic.setbe: RewriteSet(ConditionCode.ULE); break;
                case Mnemonic.setg: RewriteSet(ConditionCode.GT); break;
                case Mnemonic.setge: RewriteSet(ConditionCode.GE); break;
                case Mnemonic.setl: RewriteSet(ConditionCode.LT); break;
                case Mnemonic.setle: RewriteSet(ConditionCode.LE); break;
                case Mnemonic.setnc: RewriteSet(ConditionCode.UGE); break;
                case Mnemonic.setno: RewriteSet(ConditionCode.NO); break;
                case Mnemonic.setns: RewriteSet(ConditionCode.NS); break;
                case Mnemonic.setnz: RewriteSet(ConditionCode.NE); break;
                case Mnemonic.setpe: RewriteSet(ConditionCode.PE); break;
                case Mnemonic.setpo: RewriteSet(ConditionCode.PO); break;
                case Mnemonic.seto: RewriteSet(ConditionCode.OV); break;
                case Mnemonic.sets: RewriteSet(ConditionCode.SG); break;
                case Mnemonic.setz: RewriteSet(ConditionCode.EQ); break;
                case Mnemonic.sfence: RewriteSfence(); break;
                case Mnemonic.sgdt: RewriteSxdt(sgdt_intrinsic); break;
                case Mnemonic.sha1msg2: RewriteSha1msg2(); break;
                case Mnemonic.shl: RewriteBinOp(m.Shl); break;
                case Mnemonic.shlx: RewriteBinOp(m.Shl); break;
                case Mnemonic.shld: RewriteShxd(shld_intrinsic); break;
                case Mnemonic.shr: RewriteBinOp(m.Shr); break;
                case Mnemonic.shrx: RewriteBinOp(m.Shr); break;
                case Mnemonic.shrd: RewriteShxd(shrd_intrinsic); break;
                case Mnemonic.sidt: RewriteSxdt(sidt_intrinsic); break;
                case Mnemonic.shufps: RewritePackedTernaryop(false, shufp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.vshufps: RewritePackedTernaryop(true, shufp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.sldt: RewriteSxdt(sldt_intrinsic); break;
                case Mnemonic.smsw: RewriteSmsw(); break;
                case Mnemonic.sqrtpd: RewritePackedUnaryop(sqrtp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.sqrtps: RewritePackedUnaryop(sqrtp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.sqrtsd: RewriteSqrtsd(sqrt_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.sqrtss: RewriteSqrtsd(fsqrt_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.stc: RewriteSetFlag(Registers.C, Constant.True()); break;
                case Mnemonic.std: RewriteSetFlag(Registers.D, Constant.True()); break;
                case Mnemonic.sti: RewriteSti(); break;
                case Mnemonic.stos: RewriteStringInstruction(); break;
                case Mnemonic.stosb: RewriteStringInstruction(); break;
                case Mnemonic.str: RewriteStr(); break;
                case Mnemonic.sub: RewriteAddSub(m.ISub); break;
                case Mnemonic.subsd: RewriteScalarBinop(m.FSub, PrimitiveType.Real64, false); break;
                case Mnemonic.vsubsd: RewriteScalarBinop(m.FSub, PrimitiveType.Real64, true); break;
                case Mnemonic.subss: RewriteScalarBinop(m.FSub, PrimitiveType.Real32, false); break;
                case Mnemonic.vsubss: RewriteScalarBinop(m.FSub, PrimitiveType.Real32, true); break;
                case Mnemonic.subpd: RewritePackedBinop(false, subp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.subps: RewritePackedBinop(false, subp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.syscall: RewriteSyscall(); break;
                case Mnemonic.sysenter: RewriteSysenter(); break;
                case Mnemonic.sysexit: RewriteSysexit(); break;
                case Mnemonic.sysret: RewriteSysret(); break;
                case Mnemonic.ucomiss: RewriteComis(PrimitiveType.Real32); break;
                case Mnemonic.ucomisd: RewriteComis(PrimitiveType.Real64); break;
                case Mnemonic.unpckhps: RewriteUnpckhps(); break;
                case Mnemonic.ud0: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.ud1: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.ud2: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.unpcklpd: RewritePackedBinop(false, unpcklp_intrinsic, PrimitiveType.Real64); break;
                case Mnemonic.unpcklps: RewritePackedBinop(false, unpcklp_intrinsic, PrimitiveType.Real32); break;
                case Mnemonic.verr: RewriteVerrw(verr_intrinsic); break;
                case Mnemonic.verw: RewriteVerrw(verw_intrinsic); break;
                case Mnemonic.test: RewriteTest(); break;
                case Mnemonic.tzcnt: RewriteLeadingTrailingZeros(tzcnt_intrinsic); break;
                case Mnemonic.wait: RewriteWait(); break;
                case Mnemonic.wbinvd: RewriteWbinvd(); break;
                case Mnemonic.wrmsr: RewriteWrsmr(); break;
                case Mnemonic.xabort: RewriteXabort(); break;
                case Mnemonic.xadd: RewriteXadd(); break;
                case Mnemonic.xchg: RewriteExchange(); break;
                case Mnemonic.xgetbv: RewriteXgetbv(); break;
                case Mnemonic.xsetbv: RewriteXsetbv(); break;
                case Mnemonic.xsaveopt: RewriteXsaveopt(); break;
                case Mnemonic.xlat: RewriteXlat(); break;
                case Mnemonic.xor: RewriteLogical(m.Xor); break;
                case Mnemonic.xorpd: RewritePackedBinop(false, xorp_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.vxorpd: RewritePackedBinop(true, xorp_intrinsic, PrimitiveType.Word64); break;
                case Mnemonic.xorps: RewritePackedBinop(false, xorp_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.vzeroupper: RewriteVZeroUpper(); break;
                case Mnemonic.BOR_exp: RewriteFUnary(exp_intrinsic); break;
                case Mnemonic.BOR_ln: RewriteFUnary(log_intrinsic); break;
                }
                var len = (int)(dasm.Current.Address - addr) + dasm.Current.Length;
                yield return m.MakeCluster(addr, len, iclass);
                rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [Flags]
        public enum CopyFlags
        {
            EmitCc = 1,
        }

        /// <summary>
        /// Generates assignments, with special-case logic to break up
        /// instructions where the destination is a memory address.
        /// </summary>
        /// <remarks>
        /// The special case breaks instructions that write to memory
        /// like this:
        /// <code>
        ///		op [memaddr], reg
        /// </code>
        /// into into the equivalent:
        /// <code>
        ///		tmp := [memaddr] op reg;
        ///		store([memaddr], tmp);
        /// </code>
        /// This makes analysis easier for the subsequent phases of the 
        /// decompiler.
        /// </remarks>
        public void EmitCopy(MachineOperand opDst, Expression src, CopyFlags flags)
        {
            Expression dst = SrcOp(opDst);
            if (dst is Identifier idDst)
            {
                AssignToRegister(idDst, src);
            }
            else
            {
                var tmp = binder.CreateTemporary(opDst.Width);
                m.Assign(tmp, src);
                var ea = orw.CreateMemoryAccess(instrCur, (MemoryOperand)opDst, state);
                m.Assign(ea, tmp);
                dst = tmp;
            }
            if ((flags & CopyFlags.EmitCc) != 0)
            {
                EmitCcInstr(dst, X86Instruction.DefCc(instrCur.Mnemonic));
            }
        }

        private void AssignToRegister(Identifier idDst, Expression src)
        {
            m.Assign(idDst, src);
            if (arch.WordWidth.BitSize == 64 && idDst.Storage.BitSize == 32)
            {
                // Special case for X86-64: assignments to the 32-bit LSB of a 
                // GP register clear the high bits of that register. We model
                // this by zero-extending the register to 64 bits. We rely on later 
                // stages of decompilation to clean this up.
                var reg = (RegisterStorage) idDst.Storage;
                var idWide = binder.EnsureRegister(Registers.Gp64BitRegisters[reg.Number]);
                m.Assign(idWide, m.Convert(idDst, idDst.DataType, PrimitiveType.UInt64));
            }
        }

        private void VexAssign(bool isVex, Expression dst, Expression src)
        {
            // Legacy instructions just overwrite the dst, but VEX and EVEX instructions
            // clear the high byte.
            if (dst is Identifier idDst && idDst.DataType.BitSize > src.DataType.BitSize)
            {
                if (isVex)
                {
                    var dt = PrimitiveType.Create(Domain.UnsignedInt, idDst.DataType.BitSize);
                    src = m.Convert(src, src.DataType, dt);
                }
                else
                {
                    src = m.Dpb(idDst, src, 0);
                }
            }
            m.Assign(dst, src);
        }

        private Expression SrcOp(MachineOperand opSrc)
        {
            return orw.Transform(instrCur, opSrc, opSrc.Width);
        }

        private Expression SrcOp(int iOp)
        {
            var op = instrCur.Operands[iOp];
            return orw.Transform(instrCur, op, op.Width);
        }

        private Expression SrcOp(int iOp, DataType dstWidth)
        {
            return orw.Transform(instrCur, instrCur.Operands[iOp], dstWidth);
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("X86Rw", instrCur, instrCur.Mnemonic.ToString(), rdr, "");
        }

        static X86Rewriter()
        {
            var word16 = PrimitiveType.Word16;
            var word32 = PrimitiveType.Word32;
            var word64 = PrimitiveType.Word64;

            aaa_intrinsic = new IntrinsicBuilder("__aaa", false)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);
            aad_intrinsic = new IntrinsicBuilder("__aad", false)
                .Param(PrimitiveType.Word16)
                .Returns(PrimitiveType.Word16);
            aam_intrinsic = new IntrinsicBuilder("__aam", false)
                .Param(PrimitiveType.Byte)
                .Returns(PrimitiveType.Word16);
            aas_intrinsic = new IntrinsicBuilder("__aas", false)
                .Param(PrimitiveType.Byte)
                .Param(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);
            andnpd_intrinsic = GenericBinaryIntrinsic("__andnpd");
            andnps_intrinsic = GenericBinaryIntrinsic("__andnps");
            andp_intrinsic = GenericBinaryIntrinsic("__andp");
            arpl_intrinsic = new IntrinsicBuilder("__arpl", true)
                .Param(PrimitiveType.Word16)
                .Param(PrimitiveType.Word16)
                .OutParam(PrimitiveType.Word16)
                .Returns(PrimitiveType.Bool);
            atan2_intrinsic = new IntrinsicBuilder("__atan2", false)
                .Param(PrimitiveType.Real64)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            bound_intrinsic = new IntrinsicBuilder("__bound", true)
                .GenericTypes("T")
                .Params("T", "T")
                .Void();
            bextr_intrinsic = new IntrinsicBuilder("__bextr", false)
                .GenericTypes("T")
                .Params("T", "T")
                .Returns("T");
            blsi_intrinsic = new IntrinsicBuilder("__blsi", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            blsmsk_intrinsic = new IntrinsicBuilder("__blsmsk", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            blsr_intrinsic = new IntrinsicBuilder("__blsr", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            bsf_intrinsic = new IntrinsicBuilder("__bsf", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            bsr_intrinsic = new IntrinsicBuilder("__bsr", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            bt_intrinsic = new IntrinsicBuilder("__bt", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns(PrimitiveType.Bool);
            btc_intrinsic = new IntrinsicBuilder("__btc", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .OutParam("T")
                .Returns(PrimitiveType.Bool);
            btr_intrinsic = new IntrinsicBuilder("__btr", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .OutParam("T")
                .Returns(PrimitiveType.Bool);
            bts_intrinsic = new IntrinsicBuilder("__bts", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .OutParam("T")
                .Returns(PrimitiveType.Bool);
            bswap_intrinsic = new IntrinsicBuilder("__bswap", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
            bzhi_intrinsic = new IntrinsicBuilder("__bzhi", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");

            cli_intrinsic = new IntrinsicBuilder("__cli", true)
                .Void();
            cmpp_intrinsic = new IntrinsicBuilder("__cmpp", false)
                .GenericTypes("TSrc", "TDst")
                .Param("TSrc")
                .Param("TSrc")
                .Param(PrimitiveType.Byte)
                .Returns("TDst");
            cmpxchg_intrinsic = new IntrinsicBuilder("__cmpxchg", true)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .OutParam("T")
                .Returns(PrimitiveType.Bool);
            cmpxchgN_intrinsic = new IntrinsicBuilder("__cmpxchg", true)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .OutParam("T")
                .Returns(PrimitiveType.Bool);
            cos_intrinsic = new IntrinsicBuilder("cos", false)      //$REVIEW math.h
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            cpuid_intrinsic = new IntrinsicBuilder("__cpuid", true)
                .Param(word32).Param(word32)
                .OutParam(word32).OutParam(word32).OutParam(word32).OutParam(word32)
                .Void();
            cvtdq2ps_intrinsic = GenericConversionIntrinsic("__cvtdq2ps");
            cvtdq2pd_intrinsic = GenericConversionIntrinsic("__cvtdq2pd");
            cvtpd2dq_intrinsic = GenericConversionIntrinsic("__cvtpd2dq");
            cvtpd2ps_intrinsic = GenericConversionIntrinsic("__cvtpd2ps");
            cvtps2dq_intrinsic = GenericConversionIntrinsic("__cvtps2dq");
            cvtps2pi_intrinsic = GenericConversionIntrinsic("__cvtps2pi");
            cvtps2pd_intrinsic = GenericConversionIntrinsic("__cvtps2pd");
            cvttpd2dq_intrinsic = GenericConversionIntrinsic("__cvttpd2dq");
            cvttps2dq_intrinsic = GenericConversionIntrinsic("__cvttps2dq");

            daa_intrinsic = new IntrinsicBuilder("__daa", false)
                .Param(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);
            das_intrinsic = new IntrinsicBuilder("__das", false)
                .Param(PrimitiveType.Byte)
                .OutParam(PrimitiveType.Byte)
                .Returns(PrimitiveType.Bool);

            emms_intrinsic = new IntrinsicBuilder("__emms", true)
                .Void();
            exp_intrinsic = UnaryIntrinsic("exp", PrimitiveType.Real64);
            exponent_intrinsic = UnaryIntrinsic("__exponent", PrimitiveType.Real64);

            fabs_intrinsic = new IntrinsicBuilder("fabs", false)     //$REVIEW: math.h
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            fbld_intrinsic = new IntrinsicBuilder("__fbld", false)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            fclex_intrinsic = new IntrinsicBuilder("__fclex", true)
                .Void();
            femms_intrinsic = new IntrinsicBuilder("__femms", true)
                .Void();
            ffree_intrinsic = new IntrinsicBuilder("__ffree", true)
                .Param(PrimitiveType.Real64)
                .Void();
            fldcw_intrinsic = new IntrinsicBuilder("__fldcw", true)
                .Param(PrimitiveType.Real64)
                .Void();
            fldenv_intrinsic = new IntrinsicBuilder("__fldenv", true)
                .Param(new UnknownType())
                .Void();
            fmax_intrinsic = BinaryIntrinsic("fmax", PrimitiveType.Real64);
            fmin_intrinsic = BinaryIntrinsic("fmin", PrimitiveType.Real64);
            fninit_intrinsic = new IntrinsicBuilder("__fninit", true)
                .Void();
            fprem_incomplete_intrinsic = new IntrinsicBuilder("__fprem_incomplete", false)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Bool);
            fprem_x87_intrinsic = BinaryIntrinsic("__fprem_x87", PrimitiveType.Real64);
            frstor_intrinsic = new IntrinsicBuilder("__frstor", true)
                .Param(new UnknownType())
                .Void();
            fsave_intrinsic = new IntrinsicBuilder("__fsave", true)
                .Param(new UnknownType())
                .Void();
            fsqrt_intrinsic = UnaryIntrinsic("fsqrt", PrimitiveType.Real64);    //$REVIEW: math.h
            scalbn_intrinsic = BinaryIntrinsic("scalbn", PrimitiveType.Real64);
            fstcw_intrinsic = new IntrinsicBuilder("__fstcw", true)
                .Returns(word16);
            fstenv_intrinsic = new IntrinsicBuilder("__fstenv", true)
                .Param(new UnknownType())
                .Void();
            fxrstor_intrinsic = new IntrinsicBuilder("__fxrstor", true)
                .Void();
            fxsave_intrinsic = new IntrinsicBuilder("__fxsave", true)
                .Void();

            hlt_intrinsic = new IntrinsicBuilder("__hlt", true, new ProcedureCharacteristics
            {
                Terminates = true,
            })
                .Void();

            in_intrinsic = new IntrinsicBuilder("__in", true)
                .GenericTypes("T")
                .Param(PrimitiveType.UInt16)
                .Returns("T");
            invd_intrinsic = new IntrinsicBuilder("__invd", true)
                .Void();
            invldpg_intrinsic = new IntrinsicBuilder("__invldpg", true)
                .GenericTypes("T")
                .Param("T")
                .Void();

            jmpe_intrinsic = new IntrinsicBuilder("__jmpe", true)
                .Void();

            lfence_intrinsic = new IntrinsicBuilder("__lfence", true)
                .Void();
            lock_intrinsic = new IntrinsicBuilder("__lock", true)
                .Void();
            lg2_intrinsic = UnaryIntrinsic("lg2", PrimitiveType.Real64);
            lgdt_intrinsic = new IntrinsicBuilder("__lgdt", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            lidt_intrinsic = new IntrinsicBuilder("__lidt", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            lldt_intrinsic = new IntrinsicBuilder("__lldt", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            log_intrinsic = UnaryIntrinsic("log", PrimitiveType.Real64);        //$REVIEW:math.h
            ltr_intrinsic = new IntrinsicBuilder("__load_task_register", true)
                .Param(word16)
                .Void();
            lzcnt_intrinsic = new IntrinsicBuilder("__lzcnt", false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");

            maskmovdqu_intrinsic = GenericBinaryIntrinsic("__maskmovdqu");
            maskmovq_intrinsic = GenericBinaryIntrinsic("__maskmovq");
            mfence_intrinsic = new IntrinsicBuilder("__mfence", true)
                .Void();
            movmskb_intrinsic = new IntrinsicBuilder("__movmskb", false)
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Byte);
            movmskpd_intrinsic = new IntrinsicBuilder("__movmskpd", false)
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Byte);
            movmskps_intrinsic = new IntrinsicBuilder("__movmskps", false)
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Byte);

            max_intrinsic = new IntrinsicBuilder("max", false)
                .Param(PrimitiveType.Real64)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            min_intrinsic = new IntrinsicBuilder("min", false)
                .Param(PrimitiveType.Real64)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            movbe_intrinsic = new IntrinsicBuilder("__movbe", true)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");

            out_intrinsic = new IntrinsicBuilder("__out", true)
                .GenericTypes("T")
                .Param(PrimitiveType.UInt16)
                .Param("T")
                .Void();

            palignr_intrinsic = new IntrinsicBuilder("__palignr", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            pand_intrinsic = GenericBinaryIntrinsic("__pand");  //$TODO: SIMD these.
            pandn_intrinsic = GenericBinaryIntrinsic("__pandn");
            pause_intrinsic = new IntrinsicBuilder("__pause", true)
                .Void();
            pavg_intrinsic = new IntrinsicBuilder("__pavg", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
            pbroadcast_intrinsic = GenericConversionIntrinsic("__pbroadcast");
            pcmpeq_intrinsic = GenericBinaryIntrinsic("__pcmpeq");
            pcmpgt_intrinsic = GenericBinaryIntrinsic("__pcmpgt");
            pdep_intrinsic = new IntrinsicBuilder("__pdep", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
            pext_intrinsic = new IntrinsicBuilder("__pext", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
            pextrw_intrinsic = new IntrinsicBuilder("__pextrw", false)
                .GenericTypes("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns(word16);
            pinsr_intrinsic = new IntrinsicBuilder("__pinsr", false)
                .GenericTypes("TArray", "TElem")
                .Param("TArray")
                .Param("TArray")
                .Param(PrimitiveType.Byte)
                .Returns("TArray");
            pmaddubsw_intrinsic = new IntrinsicBuilder("__pmaddubsw", false)
                .GenericTypes("TSrc1", "TSrc2", "TDst")
                .Param("TSrc1")
                .Param("TSrc2")
                .Returns("TSrc2");
            pmovmskb_intrinsic = new IntrinsicBuilder("__pmovmskb", false)
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Byte);
            popcnt_intrinsic = new IntrinsicBuilder("__popcnt", false)  //$REVIEW: well-known intrinsic?
                .GenericTypes("TSrc", "TDst")
                .Param("TSrc")
                .Returns("TDst");
            pow_intrinsic = new IntrinsicBuilder("pow", false)  //$REVIEW: math.h
                .Param(PrimitiveType.Real64)
                .Param(PrimitiveType.Real64)
                .Returns(PrimitiveType.Real64);
            prefetchnta_intrinsic = new IntrinsicBuilder("__prefetchnta", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            prefetcht0_intrinsic = new IntrinsicBuilder("__prefetcht0", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            prefetcht1_intrinsic = new IntrinsicBuilder("__prefetcht1", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            prefetcht2_intrinsic = new IntrinsicBuilder("__prefetcht2", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            prefetchw_intrinsic = new IntrinsicBuilder("__prefetchw", true)
                .GenericTypes("T")
                .Param("T")
                .Void();

            psadbw_intrinsic = new IntrinsicBuilder("__psadbw", false)
                .GenericTypes("TSrc", "TDst")
                .Param("TSrc")
                .Param("TSrc")
                .Returns("TDst");
            pshuf_intrinsic = GenericTernaryIntrinsic("__pshuf");
            psll_intrinsic = new IntrinsicBuilder("__psll", false)
                .GenericTypes("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            psra_intrinsic = new IntrinsicBuilder("__psra", false)
                .GenericTypes("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            psrl_intrinsic = new IntrinsicBuilder("__psrl", false)
                .GenericTypes("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");

            rdmsr_intrinsic = new IntrinsicBuilder("__rdmsr", true)
                .Param(word32)
                .Returns(PrimitiveType.Word64);
            rdpmc_intrinsic = new IntrinsicBuilder("__rdpmc", true)
                .Param(word32)
                .Returns(PrimitiveType.Word64);
            rdrand_intrinsic = new IntrinsicBuilder("__rdrand", true)
                .Param(PrimitiveType.Word32)
                .Returns(PrimitiveType.Bool);
            rdtsc_intrinsic = new IntrinsicBuilder("__rdtsc", true)
                .Param(word64)
                .Returns(word64);

            sfence_intrinsic = new IntrinsicBuilder("__sfence", true)
                .Void();
            sgdt_intrinsic = new IntrinsicBuilder("__sgdt", true)
                .GenericTypes("T")
                .Returns("T");
            shld_intrinsic = new IntrinsicBuilder("__shld", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            shrd_intrinsic = new IntrinsicBuilder("__shrd", false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param(PrimitiveType.Byte)
                .Returns("T");
            sidt_intrinsic = new IntrinsicBuilder("__sidt", true)
                .GenericTypes("T")
                .Void();
            significand_intrinsic = UnaryIntrinsic("__significand", PrimitiveType.Real64);
            sin_intrinsic = UnaryIntrinsic("sin", PrimitiveType.Real64);      //$REVIEW math.h
            sldt_intrinsic = new IntrinsicBuilder("__sldt", true)
                .GenericTypes("T")
                .Void();
            smsw_intrinsic = new IntrinsicBuilder("__smsw", true)
                .GenericTypes("T")
                .Returns("T");
            sqrt_intrinsic = UnaryIntrinsic("sqrt", PrimitiveType.Real64);
            sti_intrinsic = new IntrinsicBuilder("__sti", true)
                .Void();
            str_intrinsic = new IntrinsicBuilder("__store_task_register", true)
                .Returns(PrimitiveType.SegmentSelector);
            syscall_intrinsic = new IntrinsicBuilder("__syscall", true)
                .Void();
            sysenter_intrinsic = new IntrinsicBuilder("__sysenter", true)
                .Void();
            sysexit_intrinsic = new IntrinsicBuilder("__sysexit", true)
                .Void();
            sysret_intrinsic = new IntrinsicBuilder("__sysret", true)
                .Void();

            verr_intrinsic = new IntrinsicBuilder("__verify_readable", true)
                .Param(PrimitiveType.SegmentSelector)
                .Returns(PrimitiveType.Bool);
            verw_intrinsic = new IntrinsicBuilder("__verify_writeable", true)
                .Param(PrimitiveType.SegmentSelector)
                .Returns(PrimitiveType.Bool);
            vmread_intrinsic = new IntrinsicBuilder("__vmread", true)
                .GenericTypes("TAddr", "TValue")
                .Param("TAddr")
                .Returns("TValue");
            vmwrite_intrinsic = new IntrinsicBuilder("__vmwrite", true)
                .GenericTypes("TAddr", "TValue")
                .Param("TAddr")
                .Param("TValue")
                .Void();

            wait_intrinsic = new IntrinsicBuilder("__wait", true)
                .Void();
            wbinvd_intrinsic = new IntrinsicBuilder("__wbinvd", true)
                .Void();
            wrmsr_intrinsic = new IntrinsicBuilder("__wrmsr", true)
                .Param(word32)
                .Param(word64)
                .Void();

            xabort_intrinsic = new IntrinsicBuilder("__xabort", true, new ProcedureCharacteristics
            {
                Terminates = true
            })
                .Param(PrimitiveType.Byte)
                 .Void();
            xadd_intrinsic = new IntrinsicBuilder("__xadd", true)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
            xgetbv_intrinsic = new IntrinsicBuilder("__xgetbv", true)
                .Param(word32)
                .Returns(word64);
            xsetbv_intrinsic = new IntrinsicBuilder("__xsetbv", true)
                .Param(PrimitiveType.Word32)
                .Param(PrimitiveType.Word64)
                .Void();

            alignStack_intrinsic = new IntrinsicBuilder("__align_stack", true)
                .GenericTypes("T")
                .Param("T")
                .Void();
            isunordered_intrinsic = new IntrinsicBuilder("isunordered", false)  //$REVIEW Should be src\core\intrinsics
                .GenericTypes("T")
                .Param("T")
                .Returns(PrimitiveType.Bool);
        }

        private static IntrinsicProcedure BinaryIntrinsic(string intrinsicName, PrimitiveType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Param(dt).Returns(dt);
        }

        private static IntrinsicProcedure UnaryIntrinsic(string intrinsicName, PrimitiveType dt)
        {
            return new IntrinsicBuilder(intrinsicName, false).Param(dt).Returns(dt);
        }

        private static IntrinsicProcedure GenericUnaryIntrinsic(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Returns("T");
        }

        private static IntrinsicProcedure GenericBinaryIntrinsic(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        private static IntrinsicProcedure GenericTernaryIntrinsic(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("T")
                .Param("T")
                .Param("T")
                .Param("T")
                .Returns("T");
        }

        private static IntrinsicProcedure GenericBinaryIntrinsic_DifferentTypes(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("TSrc", "TDst")
                .Param("TSrc")
                .Param("TSrc")
                .Returns("TDst");
        }


        private static IntrinsicProcedure GenericConversionIntrinsic(string name)
        {
            return new IntrinsicBuilder(name, false)
                .GenericTypes("TSrc", "TDst")
                .Param("TSrc")
                .Returns("TDst");
        }


        private static readonly IntrinsicProcedure aaa_intrinsic;
        private static readonly IntrinsicProcedure aad_intrinsic;
        private static readonly IntrinsicProcedure aam_intrinsic;
        private static readonly IntrinsicProcedure aas_intrinsic;
        private static readonly IntrinsicProcedure addp_intrinsic = GenericBinaryIntrinsic("__addp");
        private static readonly IntrinsicProcedure aesenc_intrinsic = BinaryIntrinsic("__aesenc", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure aesimc_intrinsic = UnaryIntrinsic("__aesimc", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure andnpd_intrinsic;
        private static readonly IntrinsicProcedure andnps_intrinsic;
        private static readonly IntrinsicProcedure andp_intrinsic;
        private static readonly IntrinsicProcedure arpl_intrinsic;
        private static readonly IntrinsicProcedure atan2_intrinsic;
        private static readonly IntrinsicProcedure bextr_intrinsic;
        private static readonly IntrinsicProcedure blsi_intrinsic;
        private static readonly IntrinsicProcedure blsmsk_intrinsic;
        private static readonly IntrinsicProcedure blsr_intrinsic;
        private static readonly IntrinsicProcedure bound_intrinsic;
        private static readonly IntrinsicProcedure bsf_intrinsic;
        private static readonly IntrinsicProcedure bsr_intrinsic;
        private static readonly IntrinsicProcedure bswap_intrinsic;
        private static readonly IntrinsicProcedure bt_intrinsic;
        private static readonly IntrinsicProcedure btc_intrinsic;
        private static readonly IntrinsicProcedure btr_intrinsic;
        private static readonly IntrinsicProcedure bts_intrinsic;
        private static readonly IntrinsicProcedure bzhi_intrinsic;
        private static readonly IntrinsicProcedure ceil_intrinsic = UnaryIntrinsic("ceil", PrimitiveType.Real64);
        private static readonly IntrinsicProcedure ceilf_intrinsic = UnaryIntrinsic("ceilf", PrimitiveType.Real32);
        private static readonly IntrinsicProcedure cli_intrinsic;
        private static readonly IntrinsicProcedure cmpp_intrinsic;
        private static readonly IntrinsicProcedure cmpxchg_intrinsic;
        private static readonly IntrinsicProcedure cmpxchgN_intrinsic;
        private static readonly IntrinsicProcedure cos_intrinsic;
        private static readonly IntrinsicProcedure cpuid_intrinsic;
        private static readonly IntrinsicProcedure cvtdq2ps_intrinsic;
        private static readonly IntrinsicProcedure cvtdq2pd_intrinsic;
        private static readonly IntrinsicProcedure cvtpd2dq_intrinsic;
        private static readonly IntrinsicProcedure cvtpd2ps_intrinsic;
        private static readonly IntrinsicProcedure cvtps2dq_intrinsic;
        private static readonly IntrinsicProcedure cvtps2pi_intrinsic;
        private static readonly IntrinsicProcedure cvtps2pd_intrinsic;
        private static readonly IntrinsicProcedure cvttpd2dq_intrinsic;
        private static readonly IntrinsicProcedure cvttps2dq_intrinsic;
        private static readonly IntrinsicProcedure daa_intrinsic;
        private static readonly IntrinsicProcedure das_intrinsic;
        private static readonly IntrinsicProcedure divp_intrinsic = GenericBinaryIntrinsic("__divp");
        private static readonly IntrinsicProcedure emms_intrinsic;
        private static readonly IntrinsicProcedure exp_intrinsic;
        private static readonly IntrinsicProcedure exponent_intrinsic;
        private static readonly IntrinsicProcedure fabs_intrinsic;
        private static readonly IntrinsicProcedure fbld_intrinsic;
        private static readonly IntrinsicProcedure fclex_intrinsic;
        private static readonly IntrinsicProcedure femms_intrinsic;
        private static readonly IntrinsicProcedure ffree_intrinsic;
        private static readonly IntrinsicProcedure fldcw_intrinsic;
        private static readonly IntrinsicProcedure fldenv_intrinsic;
        private static readonly IntrinsicProcedure floor_intrinsic = UnaryIntrinsic("floor", PrimitiveType.Real64);
        private static readonly IntrinsicProcedure floorf_intrinsic = UnaryIntrinsic("floorf", PrimitiveType.Real32);
        private static readonly IntrinsicProcedure fmax_intrinsic;
        private static readonly IntrinsicProcedure fmin_intrinsic;
        private static readonly IntrinsicProcedure fninit_intrinsic;
        private static readonly IntrinsicProcedure fprem_incomplete_intrinsic;
        private static readonly IntrinsicProcedure fprem_x87_intrinsic;
        private static readonly IntrinsicProcedure frstor_intrinsic;
        private static readonly IntrinsicProcedure fsave_intrinsic;
        private static readonly IntrinsicProcedure fsqrt_intrinsic;
        private static readonly IntrinsicProcedure fstcw_intrinsic;
        private static readonly IntrinsicProcedure fstenv_intrinsic;
        private static readonly IntrinsicProcedure fxrstor_intrinsic;
        private static readonly IntrinsicProcedure fxsave_intrinsic;
        private static readonly IntrinsicProcedure hlt_intrinsic;
        private static readonly IntrinsicProcedure in_intrinsic;
        private static readonly IntrinsicProcedure invd_intrinsic;
        private static readonly IntrinsicProcedure invldpg_intrinsic;
        private static readonly IntrinsicProcedure jmpe_intrinsic;
        private static readonly IntrinsicProcedure lg2_intrinsic;
        private static readonly IntrinsicProcedure lgdt_intrinsic;
        private static readonly IntrinsicProcedure lidt_intrinsic;
        private static readonly IntrinsicProcedure lldt_intrinsic;
        private static readonly IntrinsicProcedure lock_intrinsic;
        private static readonly IntrinsicProcedure log_intrinsic;
        private static readonly IntrinsicProcedure lfence_intrinsic;
        private static readonly IntrinsicProcedure ltr_intrinsic;
        private static readonly IntrinsicProcedure lzcnt_intrinsic;
        private static readonly IntrinsicProcedure maskmovdqu_intrinsic;
        private static readonly IntrinsicProcedure maskmovq_intrinsic;
        private static readonly IntrinsicProcedure max_intrinsic;
        private static readonly IntrinsicProcedure maxp_intrinsic = GenericBinaryIntrinsic("__maxp");
        private static readonly IntrinsicProcedure mfence_intrinsic;
        private static readonly IntrinsicProcedure min_intrinsic;
        private static readonly IntrinsicProcedure minp_intrinsic = GenericBinaryIntrinsic("__minp");
        private static readonly IntrinsicProcedure movbe_intrinsic;
        private static readonly IntrinsicProcedure movhp_intrinsic = GenericConversionIntrinsic("__movhp");
        private static readonly IntrinsicProcedure movlp_intrinsic = GenericConversionIntrinsic("__movlp");
        private static readonly IntrinsicProcedure movmskb_intrinsic;
        private static readonly IntrinsicProcedure movmskpd_intrinsic;
        private static readonly IntrinsicProcedure movmskps_intrinsic;
        private static readonly IntrinsicProcedure mulp_intrinsic = GenericBinaryIntrinsic("__mulp");
        private static readonly IntrinsicProcedure orp_intrinsic = GenericBinaryIntrinsic("__orp");
        private static readonly IntrinsicProcedure out_intrinsic;
        private static readonly IntrinsicProcedure palignr_intrinsic;
        private static readonly IntrinsicProcedure packss_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__packss");
        private static readonly IntrinsicProcedure packus_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__packus");
        private static readonly IntrinsicProcedure padd_intrinsic = GenericBinaryIntrinsic("__padd");
        private static readonly IntrinsicProcedure padds_intrinsic = GenericBinaryIntrinsic("__padds");
        private static readonly IntrinsicProcedure paddus_intrinsic = GenericBinaryIntrinsic("__paddus");
        private static readonly IntrinsicProcedure paddwd_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__paddwd");
        private static readonly IntrinsicProcedure pand_intrinsic;
        private static readonly IntrinsicProcedure pandn_intrinsic;
        private static readonly IntrinsicProcedure pause_intrinsic;
        private static readonly IntrinsicProcedure pavg_intrinsic;
        private static readonly IntrinsicProcedure pbroadcast_intrinsic;
        private static readonly IntrinsicProcedure pcmpeq_intrinsic;
        private static readonly IntrinsicProcedure pcmpgt_intrinsic;
        private static readonly IntrinsicProcedure pdep_intrinsic;
        private static readonly IntrinsicProcedure pext_intrinsic;
        private static readonly IntrinsicProcedure pextrw_intrinsic;
        private static readonly IntrinsicProcedure pinsr_intrinsic;
        private static readonly IntrinsicProcedure pmaddubsw_intrinsic;
        private static readonly IntrinsicProcedure pmaddwd_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__pmaddwd");
        private static readonly IntrinsicProcedure pmaxu_intrinsic = GenericBinaryIntrinsic("__pmaxu");
        private static readonly IntrinsicProcedure pmaxs_intrinsic = GenericBinaryIntrinsic("__pmaxs");
        private static readonly IntrinsicProcedure pminu_intrinsic = GenericBinaryIntrinsic("__pminu");
        private static readonly IntrinsicProcedure pmins_intrinsic = GenericBinaryIntrinsic("__pmins");
        private static readonly IntrinsicProcedure pmovmskb_intrinsic;
        private static readonly IntrinsicProcedure pmulh_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__pmulh");
        private static readonly IntrinsicProcedure pmulhu_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__pmulhu");
        private static readonly IntrinsicProcedure pmull_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__pmull");
        private static readonly IntrinsicProcedure pmulu_intrinsic = GenericBinaryIntrinsic_DifferentTypes("__pmulu");
        private static readonly IntrinsicProcedure popcnt_intrinsic;
        private static readonly IntrinsicProcedure pow_intrinsic;
        private static readonly IntrinsicProcedure prefetchnta_intrinsic;
        private static readonly IntrinsicProcedure prefetcht0_intrinsic;
        private static readonly IntrinsicProcedure prefetcht1_intrinsic;
        private static readonly IntrinsicProcedure prefetcht2_intrinsic;
        private static readonly IntrinsicProcedure prefetchw_intrinsic;
        private static readonly IntrinsicProcedure psadbw_intrinsic;
        private static readonly IntrinsicProcedure pshuf_intrinsic;
        private static readonly IntrinsicProcedure pshufb_intrinsic = GenericTernaryIntrinsic("__pshufb");
        private static readonly IntrinsicProcedure pshuflw_intrinsic = GenericTernaryIntrinsic("__pshuflw");
        private static readonly IntrinsicProcedure psll_intrinsic;
        private static readonly IntrinsicProcedure psra_intrinsic;
        private static readonly IntrinsicProcedure psrl_intrinsic;
        private static readonly IntrinsicProcedure psub_intrinsic = GenericBinaryIntrinsic("__psub");
        private static readonly IntrinsicProcedure psubs_intrinsic = GenericBinaryIntrinsic("__psubs");
        private static readonly IntrinsicProcedure psubus_intrinsic = GenericBinaryIntrinsic("__psubus");
        private static readonly IntrinsicProcedure punpckhbw_intrinsic = GenericBinaryIntrinsic("__punpckhbw");
        private static readonly IntrinsicProcedure punpckhdq_intrinsic = GenericBinaryIntrinsic("__punpckhdq");
        private static readonly IntrinsicProcedure punpckhwd_intrinsic = GenericBinaryIntrinsic("__punpckhwd");
        private static readonly IntrinsicProcedure punpcklbw_intrinsic = GenericBinaryIntrinsic("__punpcklbw");
        private static readonly IntrinsicProcedure punpckldq_intrinsic = GenericBinaryIntrinsic("__punpckldq");
        private static readonly IntrinsicProcedure punpcklqdq_intrinsic = GenericBinaryIntrinsic("__punpcklqdq");
        private static readonly IntrinsicProcedure punpcklwd_intrinsic = GenericBinaryIntrinsic("__punpcklwd");
        private static readonly IntrinsicProcedure pxor_intrinsic = GenericBinaryIntrinsic("__pxor");

        private static readonly IntrinsicProcedure rdmsr_intrinsic;
        private static readonly IntrinsicProcedure rcpp_intrinsic = GenericUnaryIntrinsic("__rcpp");
        private static readonly IntrinsicProcedure rdpmc_intrinsic;
        private static readonly IntrinsicProcedure rdrand_intrinsic;
        private static readonly IntrinsicProcedure rdtsc_intrinsic;
        private static readonly IntrinsicProcedure rndint_intrinsic = UnaryIntrinsic("__rndint", PrimitiveType.Real64);
        private static readonly IntrinsicProcedure round_intrinsic = UnaryIntrinsic("round", PrimitiveType.Real64);
        private static readonly IntrinsicProcedure roundf_intrinsic = UnaryIntrinsic("roundf", PrimitiveType.Real32);
        private static readonly IntrinsicProcedure rsqrtp_intrinsic = GenericUnaryIntrinsic("__rsqrtp");
        private static readonly IntrinsicProcedure scalbn_intrinsic;
        private static readonly IntrinsicProcedure sfence_intrinsic;
        private static readonly IntrinsicProcedure sgdt_intrinsic;
        private static readonly IntrinsicProcedure sha1msg2_intrinsic = BinaryIntrinsic("__sha1msg2", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure shld_intrinsic;
        private static readonly IntrinsicProcedure shrd_intrinsic;
        private static readonly IntrinsicProcedure shufp_intrinsic = GenericTernaryIntrinsic("__shufp");
        private static readonly IntrinsicProcedure significand_intrinsic;
        private static readonly IntrinsicProcedure sidt_intrinsic;
        private static readonly IntrinsicProcedure sin_intrinsic;
        private static readonly IntrinsicProcedure sldt_intrinsic;
        private static readonly IntrinsicProcedure smsw_intrinsic;
        private static readonly IntrinsicProcedure sqrt_intrinsic;
        private static readonly IntrinsicProcedure sqrtp_intrinsic = GenericUnaryIntrinsic("__sqrtp");
        private static readonly IntrinsicProcedure sti_intrinsic;
        private static readonly IntrinsicProcedure str_intrinsic;
        private static readonly IntrinsicProcedure subp_intrinsic = GenericBinaryIntrinsic("__subp");
        private static readonly IntrinsicProcedure syscall_intrinsic;
        private static readonly IntrinsicProcedure sysenter_intrinsic;
        private static readonly IntrinsicProcedure sysexit_intrinsic;
        private static readonly IntrinsicProcedure sysret_intrinsic;
        private static readonly IntrinsicProcedure tan_intrinsic = UnaryIntrinsic("tan", PrimitiveType.Real64);        //$REVIEW: math.h
        private static readonly IntrinsicProcedure trunc_intrinsic = BinaryIntrinsic("trunc", PrimitiveType.Real64);    //$REVIEW: math.h
        private static readonly IntrinsicProcedure truncf_intrinsic = BinaryIntrinsic("truncf", PrimitiveType.Real32);
        private static readonly IntrinsicProcedure tzcnt_intrinsic = GenericUnaryIntrinsic("__tzcnt");
        private static readonly IntrinsicProcedure unpckhp_intrinsic = GenericBinaryIntrinsic("__unpckhp");
        private static readonly IntrinsicProcedure unpcklp_intrinsic = GenericBinaryIntrinsic("__unpcklp");
        private static readonly IntrinsicProcedure verr_intrinsic;
        private static readonly IntrinsicProcedure verw_intrinsic;
        private static readonly IntrinsicProcedure vmread_intrinsic;
        private static readonly IntrinsicProcedure vmwrite_intrinsic;
        private static readonly IntrinsicProcedure wait_intrinsic;
        private static readonly IntrinsicProcedure wbinvd_intrinsic;
        private static readonly IntrinsicProcedure wrmsr_intrinsic;
        private static readonly IntrinsicProcedure xabort_intrinsic;
        private static readonly IntrinsicProcedure xadd_intrinsic;
        private static readonly IntrinsicProcedure xgetbv_intrinsic;
        private static readonly IntrinsicProcedure xorp_intrinsic = GenericBinaryIntrinsic("__xorp");
        private static readonly IntrinsicProcedure xsetbv_intrinsic;

        private static readonly IntrinsicProcedure alignStack_intrinsic;
        private static readonly IntrinsicProcedure isunordered_intrinsic;
    }
}
