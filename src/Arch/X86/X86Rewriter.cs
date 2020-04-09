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
using Reko.Core.Rtl;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using IEnumerable = System.Collections.IEnumerable;
using IEnumerator = System.Collections.IEnumerator;
using System.Diagnostics;
using System.Linq;

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
        private RtlEmitter m;
        private OperandRewriter orw;
        private X86Instruction instrCur;
        private InstrClass iclass;
        private int len;
        private List<RtlInstruction> rtlInstructions;

        public X86Rewriter(
            IntelArchitecture arch,
            IRewriterHost host,
            X86State state,
            EndianImageReader rdr,
            IStorageBinder binder)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            this.arch = arch;
            this.host = host;
            this.state = state;
            this.rdr = rdr;
            this.binder = binder;
            this.dasm = new LookaheadEnumerator<X86Instruction>(arch.CreateDisassemblerImpl(rdr));
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
                this.rtlInstructions = new List<RtlInstruction>();
                this.iclass = instrCur.InstructionClass;
                m = new RtlEmitter(rtlInstructions);
                orw = arch.ProcessorMode.CreateOperandRewriter(arch, m, binder, host);
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
                case Mnemonic.add: RewriteAddSub(Operator.IAdd); break;
                case Mnemonic.addss: RewriteScalarBinop(m.FAdd, PrimitiveType.Real32); break;
                case Mnemonic.addsd:
                case Mnemonic.vaddsd: RewriteScalarBinop(m.FAdd, PrimitiveType.Real64); break;
                case Mnemonic.addps: RewritePackedBinop("__addps", PrimitiveType.Real32); break;
                case Mnemonic.addpd: RewritePackedBinop("__addpd", PrimitiveType.Real64); break;
                case Mnemonic.aesimc: RewriteAesimc(); break;
                case Mnemonic.and: RewriteLogical(Operator.And); break;
                case Mnemonic.andnps: RewriteAndnps(); break;
                case Mnemonic.andpd: RewritePackedBinop("__andpd", PrimitiveType.Real64); break;
                case Mnemonic.andps: RewritePackedBinop("__andps", PrimitiveType.Real32); break;
                case Mnemonic.arpl: RewriteArpl(); break;
                case Mnemonic.bound: RewriteBound(); break;
                case Mnemonic.bsf: RewriteBsf(); break;
                case Mnemonic.bsr: RewriteBsr(); break;
                case Mnemonic.bswap: RewriteBswap(); break;
                case Mnemonic.bt: RewriteBt(); break;
                case Mnemonic.btc: RewriteBtc(); break;
                case Mnemonic.btr: RewriteBtr(); break;
                case Mnemonic.bts: RewriteBts(); break;
                case Mnemonic.call: RewriteCall(instrCur.Operands[0], instrCur.Operands[0].Width); break;
                case Mnemonic.cbw: RewriteCbw(); break;
                case Mnemonic.clc: RewriteSetFlag(FlagM.CF, Constant.False()); break;
                case Mnemonic.cld: RewriteSetFlag(FlagM.DF, Constant.False()); break;
                case Mnemonic.cli: RewriteCli(); break;
                case Mnemonic.clts: RewriteClts(); break;
                case Mnemonic.cmc: m.Assign(orw.FlagGroup(FlagM.CF), m.Not(orw.FlagGroup(FlagM.CF))); break;
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
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmps: RewriteStringInstruction(); break;
                case Mnemonic.cmppd: RewriteCmpp("__cmppd", PrimitiveType.Real64); break;
                case Mnemonic.cmpps: RewriteCmpp("__cmpps", PrimitiveType.Real32); break;
                case Mnemonic.cmpsb: RewriteStringInstruction(); break;
                case Mnemonic.comisd: RewriteComis(PrimitiveType.Real64); break;
                case Mnemonic.comiss: RewriteComis(PrimitiveType.Real32); break;
                case Mnemonic.cpuid: RewriteCpuid(); break;
                case Mnemonic.cvtpi2ps: RewriteCvtPackedToReal(PrimitiveType.Real32); break;
                case Mnemonic.cvtps2pi: RewriteCvtps2pi("__cvtps2pi", PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvtps2pd: RewriteCvtps2pi("__cvtps2pd", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.cvtdq2ps: RewriteCvtps2pi("__cvtdq2ps", PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Mnemonic.cvtsd2si: RewriteCvts2si(PrimitiveType.Real64); break;
                case Mnemonic.cvtsd2ss: RewriteCvtToReal(PrimitiveType.Real32); break;
                case Mnemonic.cvtsi2ss:
                case Mnemonic.vcvtsi2ss: RewriteCvtToReal(PrimitiveType.Real32); break;
                case Mnemonic.cvtsi2sd:
                case Mnemonic.vcvtsi2sd: RewriteCvtToReal(PrimitiveType.Real64); break;
                case Mnemonic.cvtss2sd: RewriteCvtToReal(PrimitiveType.Real64); break;
                case Mnemonic.cvtss2si: RewriteCvts2si(PrimitiveType.Real32); break;
                case Mnemonic.cvttsd2si: RewriteCvtts2si(PrimitiveType.Real64); break;
                case Mnemonic.cvttss2si: RewriteCvtts2si(PrimitiveType.Real32); break;
                case Mnemonic.cvttps2pi: RewriteCvttps2pi(); break;
                case Mnemonic.cwd: RewriteCwd(); break;
                case Mnemonic.daa: EmitDaaDas("__daa"); break;
                case Mnemonic.das: EmitDaaDas("__das"); break;
                case Mnemonic.dec: RewriteIncDec(-1); break;
                case Mnemonic.div: RewriteDivide(m.UDiv, Domain.UnsignedInt); break;
                case Mnemonic.divps: RewritePackedBinop("__divps", PrimitiveType.Real32); break;
                case Mnemonic.divsd: RewriteScalarBinop(m.FDiv, PrimitiveType.Real64); break;
                case Mnemonic.divss: RewriteScalarBinop(m.FDiv, PrimitiveType.Real32); break;
                case Mnemonic.f2xm1: RewriteF2xm1(); break;
                case Mnemonic.emms: RewriteEmms(); break;
                case Mnemonic.enter: RewriteEnter(); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: EmitCommonFpuInstruction(m.FAdd, false, false); break;
                case Mnemonic.faddp: EmitCommonFpuInstruction(m.FAdd, false, true); break;
                case Mnemonic.fbld: RewriteFbld(); break;
                case Mnemonic.fbstp: RewriteFbstp(); break;
                case Mnemonic.fchs: EmitFchs(); break;
                case Mnemonic.fclex: RewriteFclex(); break;
                case Mnemonic.fcmovb: RewriteFcmov(FlagM.CF, ConditionCode.GE); break;
                case Mnemonic.fcmovbe: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.GT); break;
                case Mnemonic.fcmove: RewriteFcmov(FlagM.ZF, ConditionCode.NE); break;
                case Mnemonic.fcmovnb: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.GE); break;
                case Mnemonic.fcmovnbe: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.LE); break;
                case Mnemonic.fcmovne: RewriteFcmov(FlagM.ZF, ConditionCode.EQ); break;
                case Mnemonic.fcmovnu: RewriteFcmov(FlagM.PF, ConditionCode.IS_NAN); break;
                case Mnemonic.fcmovu: RewriteFcmov(FlagM.PF, ConditionCode.NOT_NAN); break;
                case Mnemonic.fcom: RewriteFcom(0); break;
                case Mnemonic.fcomi: RewriteFcomi(false); break;
                case Mnemonic.fcomip: RewriteFcomi(true); break;
                case Mnemonic.fcomp: RewriteFcom(1); break;
                case Mnemonic.fcompp: RewriteFcom(2); break;
                case Mnemonic.fcos: RewriteFUnary("cos"); break;
                case Mnemonic.fdecstp: RewriteFdecstp(); break;
                case Mnemonic.fdiv: EmitCommonFpuInstruction(m.FDiv, false, false); break;
                case Mnemonic.fdivp: EmitCommonFpuInstruction(m.FDiv, false, true); break;
                case Mnemonic.ffree: RewriteFfree(); break;
                case Mnemonic.fiadd: EmitCommonFpuInstruction(m.FAdd, false, false, PrimitiveType.Real64); break;
                case Mnemonic.ficom: RewriteFicom(false); break;
                case Mnemonic.ficomp: RewriteFicom(true); break;
                case Mnemonic.fimul: EmitCommonFpuInstruction(m.FMul, false, false, PrimitiveType.Real64); break;
                case Mnemonic.fisub: EmitCommonFpuInstruction(m.FSub, false, false, PrimitiveType.Real64); break;
                case Mnemonic.fisubr: EmitCommonFpuInstruction(m.FSub, true, false, PrimitiveType.Real64); break;
                case Mnemonic.fidiv: EmitCommonFpuInstruction(m.FDiv, false, false, PrimitiveType.Real64); break;
                case Mnemonic.fidivr: EmitCommonFpuInstruction(m.FDiv, true, false, PrimitiveType.Real64); break;
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
                case Mnemonic.fninit: RewriteFninit(); break;
                case Mnemonic.fnop: m.Nop(); break;
                case Mnemonic.fnsetpm: m.Nop(); break;
                case Mnemonic.fpatan: RewriteFpatan(); break;
                case Mnemonic.fprem: RewriteFprem(); break;
                case Mnemonic.fprem1: RewriteFprem1(); break;
                case Mnemonic.fptan: RewriteFptan(); break;
                case Mnemonic.frndint: RewriteFUnary("__rndint"); break;
                case Mnemonic.frstor: RewriteFrstor(); break;
                case Mnemonic.fsave: RewriteFsave(); break;
                case Mnemonic.fscale: RewriteFscale(); break;
                case Mnemonic.fsin: RewriteFUnary("sin"); break;
                case Mnemonic.fsincos: RewriteFsincos(); break;
                case Mnemonic.fsqrt: RewriteFUnary("sqrt"); break;
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
                case Mnemonic.fxtract: RewriteFxtract(); break;
                case Mnemonic.fyl2x: RewriteFyl2x(); break;
                case Mnemonic.fyl2xp1: RewriteFyl2xp1(); break;
                case Mnemonic.getsec: RewriteGetsec(); break;
                case Mnemonic.hlt: RewriteHlt(); break;
                case Mnemonic.idiv: RewriteDivide(m.SDiv, Domain.SignedInt); break;
                case Mnemonic.@in: RewriteIn(); break;
                case Mnemonic.imul: RewriteMultiply(Operator.SMul, Domain.SignedInt); break;
                case Mnemonic.inc: RewriteIncDec(1); break;
                case Mnemonic.insb: RewriteStringInstruction(); break;
                case Mnemonic.ins: RewriteStringInstruction(); break;
                case Mnemonic.@int: RewriteInt(); break;
                case Mnemonic.into: RewriteInto(); break;
                case Mnemonic.invd: RewriteInvd(); break;
                case Mnemonic.iret: RewriteIret(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.ja: RewriteConditionalGoto(ConditionCode.UGT, instrCur.Operands[0]); break;
                case Mnemonic.jbe: RewriteConditionalGoto(ConditionCode.ULE, instrCur.Operands[0]); break;
                case Mnemonic.jc: RewriteConditionalGoto(ConditionCode.ULT, instrCur.Operands[0]); break;
                case Mnemonic.jcxz: RewriteJcxz(); break;
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
                case Mnemonic.lgdt: RewriteLxdt("__lgdt"); break;
                case Mnemonic.lidt: RewriteLxdt("__lidt"); break;
                case Mnemonic.lldt: RewriteLxdt("__lldt"); break;
                case Mnemonic.lmsw: RewriteLmsw(); break;
                case Mnemonic.@lock: RewriteLock(); break;
                case Mnemonic.lods: RewriteStringInstruction(); break;
                case Mnemonic.lodsb: RewriteStringInstruction(); break;
                case Mnemonic.loop: RewriteLoop(0, ConditionCode.EQ); break;
                case Mnemonic.loope: RewriteLoop(FlagM.ZF, ConditionCode.EQ); break;
                case Mnemonic.loopne: RewriteLoop(FlagM.ZF, ConditionCode.NE); break;
                case Mnemonic.lsl: RewriteLsl(); break;
                case Mnemonic.lss: RewriteLxs(Registers.ss); break;
                case Mnemonic.maskmovq: RewriteMaskmovq(); break;
                case Mnemonic.maxps: RewritePackedBinop("__maxps", PrimitiveType.Real32); break;
                case Mnemonic.mfence: RewriteMfence(); break;
                case Mnemonic.minpd: RewritePackedBinop("__minpd", PrimitiveType.Real64); break;
                case Mnemonic.minps: RewritePackedBinop("__minps", PrimitiveType.Real32); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movapd:
                case Mnemonic.movaps:
                case Mnemonic.vmovapd:
                case Mnemonic.vmovaps: RewriteMov(); break;
                case Mnemonic.vmread: RewriteVmread(); break;
                case Mnemonic.vmwrite: RewriteVmwrite(); break;
                case Mnemonic.movbe: RewriteMovbe(); break;
                case Mnemonic.movd: RewriteMovzx(); break;
                case Mnemonic.movdqa: RewriteMov(); break;
                case Mnemonic.movhpd: RewritePackedUnaryop("__movhpd", PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.movhps: RewritePackedUnaryop("__movhps", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.movlpd: RewritePackedUnaryop("__movlpd", PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.movlps: RewritePackedUnaryop("__movlps", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.movlhps: RewriteMovlhps(); break;
                case Mnemonic.movmskpd: RewriteMovmsk("__movmskpd", PrimitiveType.Real64); break;
                case Mnemonic.movmskps: RewriteMovmsk("__movmskps", PrimitiveType.Real32); break;
                case Mnemonic.movnti: RewriteMov(); break;
                case Mnemonic.movntps: RewriteMov(); break;
                case Mnemonic.movntq: RewriteMov(); break;
                case Mnemonic.movq: RewriteMov(); break;
                case Mnemonic.movs: RewriteStringInstruction(); break;
                case Mnemonic.movsb: RewriteStringInstruction(); break;
                case Mnemonic.movsd:
                case Mnemonic.vmovsd: RewriteMovssd(PrimitiveType.Real64); break;
                case Mnemonic.movss:
                case Mnemonic.vmovss: RewriteMovssd(PrimitiveType.Real32); break;
                case Mnemonic.movsx: RewriteMovsx(); break;
                case Mnemonic.movups: RewriteMov(); break;
                case Mnemonic.movupd: RewriteMov(); break;
                case Mnemonic.movzx: RewriteMovzx(); break;
                case Mnemonic.mul: RewriteMultiply(Operator.UMul, Domain.UnsignedInt); break;
                case Mnemonic.mulpd: RewritePackedBinop("__mulpd", PrimitiveType.Real64); break;
                case Mnemonic.mulps: RewritePackedBinop("__mulps", PrimitiveType.Real32); break;
                case Mnemonic.mulss: RewriteScalarBinop(m.FMul, PrimitiveType.Real32); break;
                case Mnemonic.mulsd: RewriteScalarBinop(m.FMul, PrimitiveType.Real64); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.not: RewriteNot(); break;
                case Mnemonic.or: RewriteLogical(BinaryOperator.Or); break;
                case Mnemonic.orpd: RewritePackedBinop("__orpd", PrimitiveType.Real64); break;
                case Mnemonic.orps: RewritePackedBinop("__orps", PrimitiveType.Real32); break;
                case Mnemonic.@out: RewriteOut(); break;
                case Mnemonic.@outs: RewriteStringInstruction(); break;
                case Mnemonic.@outsb: RewriteStringInstruction(); break;
                case Mnemonic.packssdw: RewritePackedBinop("__packssdw", PrimitiveType.Int32, new ArrayType(PrimitiveType.Int16, 0)); break;
                case Mnemonic.packuswb: RewritePackedBinop("__packuswb", PrimitiveType.UInt16, new ArrayType(PrimitiveType.UInt8, 0)); break;
                case Mnemonic.paddb: RewritePackedBinop("__paddb", PrimitiveType.Byte); break;
                case Mnemonic.paddd: RewritePackedBinop("__paddd", PrimitiveType.Word32); break;
                case Mnemonic.paddq: case Mnemonic.vpaddq: RewritePackedBinop("__paddq", PrimitiveType.Word64); break;
                case Mnemonic.paddsw: RewritePackedBinop("__paddsw", PrimitiveType.Word16); break;
                case Mnemonic.paddsb: RewritePackedBinop("__paddsb", PrimitiveType.SByte); break;
                case Mnemonic.paddusb: RewritePackedBinop("__paddusb", PrimitiveType.Byte); break;
                case Mnemonic.paddusw: RewritePackedBinop("__paddsw", PrimitiveType.Word16); break;
                case Mnemonic.paddw: RewritePackedBinop("__paddw", PrimitiveType.Word16); break;
                case Mnemonic.pand: case Mnemonic.vpand: RewritePackedLogical("__pand"); break;
                case Mnemonic.pandn: case Mnemonic.vpandn: RewritePackedLogical("__pandn"); break;
                case Mnemonic.pause: RewritePause(); break;
                case Mnemonic.palignr: RewritePalignr(); break;
                case Mnemonic.pavgb: RewritePavg("__pavgb", PrimitiveType.Byte); break;
                case Mnemonic.pavgw: RewritePavg("__pavgw", PrimitiveType.Byte); break;
                case Mnemonic.pcmpeqb: RewritePcmp("__pcmpeqb", PrimitiveType.Byte); break;
                case Mnemonic.pcmpeqd: RewritePcmp("__pcmpeqd", PrimitiveType.Word32); break;
                case Mnemonic.pcmpeqw: RewritePcmp("__pcmpeqw", PrimitiveType.Word16); break;
                case Mnemonic.pcmpgtb: RewritePcmp("__pcmpgtb", PrimitiveType.Byte); break;
                case Mnemonic.pcmpgtd: RewritePcmp("__pcmpgtd", PrimitiveType.Word32); break;
                case Mnemonic.pcmpgtw: RewritePcmp("__pcmpgtw", PrimitiveType.Word16); break;
                case Mnemonic.pextrw: case Mnemonic.vextrw:  RewritePextrw(); break;
                case Mnemonic.pinsrw: case Mnemonic.vpinsrw: RewritePinsrw(); break;
                case Mnemonic.pmaddwd: RewritePackedBinop("__pmaddwd", PrimitiveType.Word16, new ArrayType(PrimitiveType.Word32, 0)); break;
                case Mnemonic.pmaxsw: RewritePackedBinop("__pmaxsw", PrimitiveType.Int16); break;
                case Mnemonic.pmaxub: RewritePackedBinop("__pmaxub", PrimitiveType.UInt8); break;
                case Mnemonic.pminsw: RewritePackedBinop("__pminsw", PrimitiveType.Int16); break;
                case Mnemonic.pminub: RewritePackedBinop("__pminub", PrimitiveType.UInt8); break;
                case Mnemonic.pmovmskb: RewriteMovmsk("__pmovmskb", PrimitiveType.Byte); break;
                case Mnemonic.pmulhuw: RewritePackedBinop("__pmulhuw", PrimitiveType.UInt16, new ArrayType(PrimitiveType.UInt16, 8)); break;
                case Mnemonic.pmulhw: RewritePackedBinop("__pmulhw", PrimitiveType.Int16, new ArrayType(PrimitiveType.Int16, 8)); break;
                case Mnemonic.pmullw: case Mnemonic.vpmullw: RewritePackedBinop("__pmullw", PrimitiveType.Int16, new ArrayType(PrimitiveType.Int16, 8)); break;
                case Mnemonic.pmuludq: RewritePackedBinop("__pmuludq", PrimitiveType.UInt32, new ArrayType(PrimitiveType.UInt64, 0)); break;
                case Mnemonic.prefetchw: RewritePrefetch("__prefetchw"); break;
                case Mnemonic.psadbw: RewritePackedBinop("__psadbw", PrimitiveType.Byte, PrimitiveType.Word16); break;
                case Mnemonic.pslld: RewritePackedBinop("__pslld", PrimitiveType.Word32); break;
                case Mnemonic.psllq: case Mnemonic.vpsllq: RewritePackedBinop("__psllq", PrimitiveType.Word64); break;
                case Mnemonic.psllw: RewritePackedBinop("__psllw", PrimitiveType.Word16); break;
                case Mnemonic.psrad: RewritePackedBinop("__psrad", PrimitiveType.Int32); break;
                case Mnemonic.psraw: RewritePackedBinop("__psraw", PrimitiveType.Int16); break;
                case Mnemonic.psrlq: RewritePackedBinop("__psrlq", PrimitiveType.Word64); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.popa: RewritePopa(); break;
                case Mnemonic.popf: RewritePopf(); break;
                case Mnemonic.por: RewritePackedLogical("__por"); break;
                case Mnemonic.prefetchnta: RewritePrefetch("__prefetchnta"); break;
                case Mnemonic.prefetcht0: RewritePrefetch("__prefetcht0"); break;
                case Mnemonic.prefetcht1: RewritePrefetch("__prefetcht1"); break;
                case Mnemonic.prefetcht2: RewritePrefetch("__prefetcht2"); break;
                case Mnemonic.pshufd: RewritePshuf("__pshufd", PrimitiveType.Word32); break;
                case Mnemonic.pshufw: RewritePshuf("__pshufw", PrimitiveType.Word16); break;
                case Mnemonic.psrld: RewritePackedShift("__psrld", PrimitiveType.Word32); break;
                case Mnemonic.psrlw: RewritePackedShift("__psrlw", PrimitiveType.Word16); break;
                case Mnemonic.psubb: RewritePackedBinop("__psubb", PrimitiveType.Byte); break;
                case Mnemonic.psubd: case Mnemonic.vpsubd: RewritePackedBinop("__psubd", PrimitiveType.Word32); break;
                case Mnemonic.psubq: RewritePackedBinop("__psubq", PrimitiveType.Word64); break;
                case Mnemonic.psubsb: RewritePackedBinop("__psubsb", PrimitiveType.SByte); break;
                case Mnemonic.psubsw: RewritePackedBinop("__psubsw", PrimitiveType.Word16); break;
                case Mnemonic.psubusb: RewritePackedBinop("__psubusb", PrimitiveType.UInt8); break;
                case Mnemonic.psubusw: RewritePackedBinop("__psubusw", PrimitiveType.UInt16); break;
                case Mnemonic.psubw: RewritePackedBinop("__psubw", PrimitiveType.Word16); break;
                case Mnemonic.punpckhbw: RewritePunpckhbw(); break;
                case Mnemonic.punpckhdq: RewritePunpckhdq(); break;
                case Mnemonic.punpckhwd: RewritePunpckhwd(); break;
                case Mnemonic.punpcklbw: RewritePunpcklbw(); break;
                case Mnemonic.punpckldq: RewritePunpckldq(); break;
                case Mnemonic.punpcklwd: RewritePunpcklwd(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.pusha: RewritePusha(); break;
                case Mnemonic.pushf: RewritePushf(); break;
                case Mnemonic.pxor: case Mnemonic.vpxor: RewritePxor(); break;
                case Mnemonic.rcl: RewriteRotation(PseudoProcedure.RolC, true, true); break;
                case Mnemonic.rcpps: RewritePackedUnaryop("__rcpps", PrimitiveType.Real32); break;
                case Mnemonic.rcr: RewriteRotation(PseudoProcedure.RorC, true, false); break;
                case Mnemonic.rol: RewriteRotation(PseudoProcedure.Rol, false, true); break;
                case Mnemonic.ror: RewriteRotation(PseudoProcedure.Ror, false, false); break;
                case Mnemonic.rdmsr: RewriteRdmsr(); break;
                case Mnemonic.rdpmc: RewriteRdpmc(); break;
                case Mnemonic.rdtsc: RewriteRdtsc(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.retf: RewriteRet(); break;
                case Mnemonic.rsqrtps: RewritePackedUnaryop("__rsqrtps", PrimitiveType.Real32); break;
                case Mnemonic.sahf: m.Assign(orw.FlagGroup(X86Instruction.DefCc(instrCur.Mnemonic)), orw.AluRegister(Registers.ah)); break;
                case Mnemonic.sar: RewriteBinOp(Operator.Sar); break;
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
                case Mnemonic.sgdt: RewriteSxdt("__sgdt"); break;
                case Mnemonic.sha1msg2: RewriteSha1msg2(); break;
                case Mnemonic.shl: RewriteBinOp(BinaryOperator.Shl); break;
                case Mnemonic.shld: RewriteShxd("__shld"); break;
                case Mnemonic.shr: RewriteBinOp(BinaryOperator.Shr); break;
                case Mnemonic.shrd: RewriteShxd("__shrd"); break;
                case Mnemonic.sidt: RewriteSxdt("__sidt"); break;
                case Mnemonic.vshufps: RewritePackedTernaryop("__vshufps", PrimitiveType.Real32); break;
                case Mnemonic.sldt: RewriteSxdt("__sldt"); break;
                case Mnemonic.smsw: RewriteSmsw(); break;
                case Mnemonic.sqrtps: RewritePackedUnaryop("__sqrtps", PrimitiveType.Real32); break;
                case Mnemonic.sqrtsd: RewriteSqrtsd(); break;
                case Mnemonic.stc: RewriteSetFlag(FlagM.CF, Constant.True()); break;
                case Mnemonic.std: RewriteSetFlag(FlagM.DF, Constant.True()); break;
                case Mnemonic.sti: RewriteSti(); break;
                case Mnemonic.stos: RewriteStringInstruction(); break;
                case Mnemonic.stosb: RewriteStringInstruction(); break;
                case Mnemonic.sub: RewriteAddSub(BinaryOperator.ISub); break;
                case Mnemonic.subsd: RewriteScalarBinop(m.FSub, PrimitiveType.Real64); break;
                case Mnemonic.subss: RewriteScalarBinop(m.FSub, PrimitiveType.Real32); break;
                case Mnemonic.subpd: RewritePackedBinop("__subpd", PrimitiveType.Real64); break;
                case Mnemonic.subps: RewritePackedBinop("__subps", PrimitiveType.Real32); break;
                case Mnemonic.syscall: RewriteSyscall(); break;
                case Mnemonic.sysenter: RewriteSysenter(); break;
                case Mnemonic.sysexit: RewriteSysexit(); break;
                case Mnemonic.sysret: RewriteSysret(); break;
                case Mnemonic.ucomiss: RewriteComis(PrimitiveType.Real32); break;
                case Mnemonic.ucomisd: RewriteComis(PrimitiveType.Real64); break;
                case Mnemonic.ud0: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.ud1: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.ud2: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.unpcklpd: RewritePackedBinop("__unpcklpd", PrimitiveType.Real64); break;
                case Mnemonic.unpcklps: RewritePackedBinop("__unpcklps", PrimitiveType.Real32); break;
                case Mnemonic.test: RewriteTest(); break;
                case Mnemonic.wait: RewriteWait(); break;
                case Mnemonic.wbinvd: RewriteWbinvd(); break;
                case Mnemonic.wrmsr: RewriteWrsmr(); break;
                case Mnemonic.xadd: RewriteXadd(); break;
                case Mnemonic.xchg: RewriteExchange(); break;
                case Mnemonic.xgetbv: RewriteXgetbv(); break;
                case Mnemonic.xsetbv: RewriteXsetbv(); break;
                case Mnemonic.xlat: RewriteXlat(); break;
                case Mnemonic.xor: RewriteLogical(BinaryOperator.Xor); break;
                case Mnemonic.xorpd:
                case Mnemonic.vxorpd: RewritePackedBinop("__xorpd", PrimitiveType.Word64); break;
                case Mnemonic.xorps: RewritePackedBinop("__xorps", PrimitiveType.Word32); break;

                case Mnemonic.BOR_exp: RewriteFUnary("exp"); break;
                case Mnemonic.BOR_ln: RewriteFUnary("log"); break;
                }
                var len = (int)(dasm.Current.Address - addr) + dasm.Current.Length;
                yield return m.MakeCluster(addr, len, iclass);
            }
        }

		private void RewriteFninit()
		{
			var ppp = host.PseudoProcedure("__fninit", VoidType.Instance);
			m.SideEffect(ppp);
		}

		public Expression PseudoProc(PseudoProcedure ppp, DataType retType, params Expression[] args)
        {
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));

            return m.Fn(new ProcedureConstant(arch.PointerType, ppp), retType, args);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Flags]
        public enum CopyFlags
        {
            EmitCc = 1,
            SetCfIf0 = 2,
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
            if ((flags & CopyFlags.SetCfIf0) != 0)
            {
                m.Assign(orw.FlagGroup(FlagM.CF), m.Eq0(dst));
            }
        }

        private void AssignToRegister(Identifier idDst, Expression src)
        {
            if (arch.WordWidth.BitSize == 64 && idDst.Storage.BitSize == 32)
            {
                // Special case for X86-64: assignments to the 32-bit LSB of a 
                // GP register clear the high bits of that register. We model
                // this zero-extending the register to 64 bits. We rely on later 
                // stages of decompilation to clean this up.
                //$REVIEW: Arguably, this could be done better by clearing 
                // the whole 64-bit register, then overwriting the bottom 32 bits. 
                var reg = (RegisterStorage) idDst.Storage;
                idDst = binder.EnsureRegister(Registers.Gp64BitRegisters[reg.Number]);
                src = m.Cast(PrimitiveType.UInt64, src);
            }
            m.Assign(idDst, src);
        }


        private Expression SrcOp(MachineOperand opSrc)
        {
            return orw.Transform(instrCur, opSrc, opSrc.Width, state);
        }

        private Expression SrcOp(MachineOperand opSrc, DataType dstWidth)
        {
            return orw.Transform(instrCur, opSrc, dstWidth, state);
        }

        private static HashSet<Mnemonic> seen = new HashSet<Mnemonic>();

        [Conditional("DEBUG")]
        private void EmitUnitTest()
        {
            if (seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var bytes = r2.ReadBytes(dasm.Current.Length);
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void X86Rw_" + dasm.Current.Mnemonic + "()");
            Debug.WriteLine("        {");
            Debug.Write("            BuildTest(");
            Debug.Write(string.Join(
                ", ",
                bytes.Select(b => string.Format("0x{0:X2}", (int)b))));
            Debug.WriteLine(");\t// " + dasm.Current.ToString());
            Debug.WriteLine("            AssertCode(");
            Debug.WriteLine("                \"0|L--|{0}({1}): 1 instructions\",", dasm.Current.Address, dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
    }
}
