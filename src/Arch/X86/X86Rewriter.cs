#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.X86
{
    /// <summary>
    /// Rewrites x86 instructions into a stream of low-level RTL-like instructions.
    /// </summary>
    public partial class X86Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private IRewriterHost host;
        private IntelArchitecture arch;
        private IStorageBinder binder;
        private LookaheadEnumerator<X86Instruction> dasm;
        private RtlEmitter m;
        private OperandRewriter orw;
        private X86Instruction instrCur;
        private RtlClass rtlc;
        private int len;
        private List<RtlInstruction> rtlInstructions;
        private X86State state;

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
            this.binder = binder;
            this.state = state;
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
                this.len = instrCur.Length;
                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = RtlClass.Linear;
                m = new RtlEmitter(rtlInstructions);
                orw = arch.ProcessorMode.CreateOperandRewriter(arch, m, binder, host);
                switch (instrCur.code)
                {
                default:
                    host.Warn(
                        dasm.Current.Address,
                        "Rewriting x86 opcode '{0}' is not supported yet.",
                        instrCur.code);
                    goto case Opcode.illegal;
                case Opcode.illegal: rtlc = RtlClass.Invalid; m.Invalid(); break;
                case Opcode.aaa: RewriteAaa(); break;
                case Opcode.aad: RewriteAad(); break;
                case Opcode.aam: RewriteAam(); break;
                case Opcode.aas: RewriteAas(); break;
                case Opcode.adc: RewriteAdcSbb(m.IAdd); break;
                case Opcode.add: RewriteAddSub(Operator.IAdd); break;
                case Opcode.addss: RewriteScalarBinop(m.FAdd, PrimitiveType.Real32); break;
                case Opcode.addsd:
                case Opcode.vaddsd: RewriteScalarBinop(m.FAdd, PrimitiveType.Real64); break;
                case Opcode.addps: RewritePackedBinop("__addps", PrimitiveType.Real32); break;
                case Opcode.addpd: RewritePackedBinop("__addpd", PrimitiveType.Real64); break;
                case Opcode.aesimc: RewriteAesimc(); break;
                case Opcode.and: RewriteLogical(Operator.And); break;
                case Opcode.andnps: RewriteAndnps(); break;
                case Opcode.andps: RewriteAndps(); break;
                case Opcode.arpl: RewriteArpl(); break;
                case Opcode.bound: RewriteBound(); break;
                case Opcode.bsf: RewriteBsf(); break;
                case Opcode.bsr: RewriteBsr(); break;
                case Opcode.bswap: RewriteBswap(); break;
                case Opcode.bt: RewriteBt(); break;
                case Opcode.btc: RewriteBtc(); break;
                case Opcode.btr: RewriteBtr(); break;
                case Opcode.bts: RewriteBts(); break;
                case Opcode.call: RewriteCall(instrCur.op1, instrCur.op1.Width); break;
                case Opcode.cbw: RewriteCbw(); break;
                case Opcode.clc: RewriteSetFlag(FlagM.CF, Constant.False()); break;
                case Opcode.cld: RewriteSetFlag(FlagM.DF, Constant.False()); break;
                case Opcode.cli: RewriteCli(); break;
                case Opcode.clts: RewriteClts(); break;
                case Opcode.cmc: m.Assign(orw.FlagGroup(FlagM.CF), m.Not(orw.FlagGroup(FlagM.CF))); break;
                case Opcode.cmova: RewriteConditionalMove(ConditionCode.UGT, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovbe: RewriteConditionalMove(ConditionCode.ULE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovc: RewriteConditionalMove(ConditionCode.ULT, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovge: RewriteConditionalMove(ConditionCode.GE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovg: RewriteConditionalMove(ConditionCode.GT, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovl: RewriteConditionalMove(ConditionCode.LT, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovle: RewriteConditionalMove(ConditionCode.LE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovnc: RewriteConditionalMove(ConditionCode.UGE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovno: RewriteConditionalMove(ConditionCode.NO, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovns: RewriteConditionalMove(ConditionCode.NS, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovnz: RewriteConditionalMove(ConditionCode.NE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovo: RewriteConditionalMove(ConditionCode.OV, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovpe: RewriteConditionalMove(ConditionCode.PE, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovpo: RewriteConditionalMove(ConditionCode.PO, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovs: RewriteConditionalMove(ConditionCode.SG, instrCur.op1, instrCur.op2); break;
                case Opcode.cmovz: RewriteConditionalMove(ConditionCode.EQ, instrCur.op1, instrCur.op2); break;
                case Opcode.cmpxchg: RewriteCmpxchg(); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmps: RewriteStringInstruction(); break;
                case Opcode.cmppd: RewriteCmpp("__cmppd", PrimitiveType.Real64); break;
                case Opcode.cmpps: RewriteCmpp("__cmpps", PrimitiveType.Real32); break;
                case Opcode.cmpsb: RewriteStringInstruction(); break;
                case Opcode.comisd: RewriteComis(PrimitiveType.Real64); break;
                case Opcode.comiss: RewriteComis(PrimitiveType.Real32); break;
                case Opcode.cpuid: RewriteCpuid(); break;
                case Opcode.cvtpi2ps: RewriteCvtPackedToReal(PrimitiveType.Real32); break;
                case Opcode.cvtps2pi: RewriteCvtps2pi("__cvtps2pi", PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Opcode.cvtps2pd: RewriteCvtps2pi("__cvtps2pd", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.cvtdq2ps: RewriteCvtps2pi("__cvtdq2ps", PrimitiveType.Int64, PrimitiveType.Real32); break;
                case Opcode.cvtsi2ss:
                case Opcode.vcvtsi2ss: RewriteCvtToReal(PrimitiveType.Real32); break;
                case Opcode.cvtsi2sd:
                case Opcode.vcvtsi2sd: RewriteCvtToReal(PrimitiveType.Real64); break;
                case Opcode.cvttsd2si: RewriteCvtts2si(PrimitiveType.Real64); break;
                case Opcode.cvttss2si: RewriteCvtts2si(PrimitiveType.Real32); break;
                case Opcode.cvttps2pi: RewriteCvttps2pi(); break;
                case Opcode.cwd: RewriteCwd(); break;
                case Opcode.daa: EmitDaaDas("__daa"); break;
                case Opcode.das: EmitDaaDas("__das"); break;
                case Opcode.dec: RewriteIncDec(-1); break;
                case Opcode.div: RewriteDivide(m.UDiv, Domain.UnsignedInt); break;
                case Opcode.divps: RewritePackedBinop("__divps", PrimitiveType.Real32); break;
                case Opcode.divsd: RewriteScalarBinop(m.FDiv, PrimitiveType.Real64); break;
                case Opcode.divss: RewriteScalarBinop(m.FDiv, PrimitiveType.Real32); break;
                case Opcode.f2xm1: RewriteF2xm1(); break;
                case Opcode.emms: RewriteEmms(); break;
                case Opcode.enter: RewriteEnter(); break;
                case Opcode.fabs: RewriteFabs(); break;
                case Opcode.fadd: EmitCommonFpuInstruction(m.FAdd, false, false); break;
                case Opcode.faddp: EmitCommonFpuInstruction(m.FAdd, false, true); break;
                case Opcode.fbld: RewriteFbld(); break;
                case Opcode.fbstp: RewriteFbstp(); break;
                case Opcode.fchs: EmitFchs(); break;
                case Opcode.fclex: RewriteFclex(); break;
                case Opcode.fcmovb: RewriteFcmov(FlagM.CF, ConditionCode.GE); break;
                case Opcode.fcmovbe: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.GT); break;
                case Opcode.fcmove: RewriteFcmov(FlagM.ZF, ConditionCode.NE); break;
                case Opcode.fcmovnb: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.GE); break;
                case Opcode.fcmovnbe: RewriteFcmov(FlagM.CF| FlagM.ZF, ConditionCode.LE); break;
                case Opcode.fcmovne: RewriteFcmov(FlagM.ZF, ConditionCode.EQ); break;
                case Opcode.fcom: RewriteFcom(0); break;
                case Opcode.fcomi: RewriteFcomi(false); break;
                case Opcode.fcomip: RewriteFcomi(true); break;
                case Opcode.fcomp: RewriteFcom(1); break;
                case Opcode.fcompp: RewriteFcom(2); break;
                case Opcode.fcos: RewriteFUnary("cos"); break;
                case Opcode.fdecstp: RewriteFdecstp(); break;
                case Opcode.fdiv: EmitCommonFpuInstruction(m.FDiv, false, false); break;
                case Opcode.fdivp: EmitCommonFpuInstruction(m.FDiv, false, true); break;
                case Opcode.ffree: RewriteFfree(); break;
                case Opcode.fiadd: EmitCommonFpuInstruction(m.FAdd, false, false, PrimitiveType.Real64); break;
                case Opcode.ficom: RewriteFicom(false); break;
                case Opcode.ficomp: RewriteFicom(true); break;
                case Opcode.fimul: EmitCommonFpuInstruction(m.FMul, false, false, PrimitiveType.Real64); break;
                case Opcode.fisub: EmitCommonFpuInstruction(m.FSub, false, false, PrimitiveType.Real64); break;
                case Opcode.fisubr: EmitCommonFpuInstruction(m.FSub, true, false, PrimitiveType.Real64); break;
                case Opcode.fidiv: EmitCommonFpuInstruction(m.FDiv, false, false, PrimitiveType.Real64); break;
                case Opcode.fidivr: EmitCommonFpuInstruction(m.FDiv, true, false, PrimitiveType.Real64); break;
                case Opcode.fdivr: EmitCommonFpuInstruction(m.FDiv, true, false); break;
                case Opcode.fdivrp: EmitCommonFpuInstruction(m.FDiv, true, true); break;
                case Opcode.fild: RewriteFild(); break;
                case Opcode.fincstp: RewriteFincstp(); break;
                case Opcode.fist: RewriteFist(false); break;
                case Opcode.fistp: RewriteFist(true); break;
                case Opcode.fisttp: RewriteFistt(true); break;
                case Opcode.fld: RewriteFld(); break;
                case Opcode.fld1: RewriteFldConst(1.0); break;
                case Opcode.fldcw: RewriteFldcw(); break;
                case Opcode.fldenv: RewriteFldenv(); break;
                case Opcode.fldl2e: RewriteFldConst(Constant.LgE()); break;
                case Opcode.fldl2t: RewriteFldConst(Constant.Lg10()); break;
                case Opcode.fldlg2: RewriteFldConst(Constant.Log2()); break;
                case Opcode.fldln2: RewriteFldConst(Constant.Ln2()); break;
                case Opcode.fldpi: RewriteFldConst(Constant.Pi()); break;
                case Opcode.fldz: RewriteFldConst(0.0); break;
                case Opcode.fmul: EmitCommonFpuInstruction(m.FMul, false, false); break;
                case Opcode.fmulp: EmitCommonFpuInstruction(m.FMul, false, true); break;
                case Opcode.fninit: RewriteFninit(); break;
                case Opcode.fnop: m.Nop(); break;
                case Opcode.fpatan: RewriteFpatan(); break;
                case Opcode.fprem: RewriteFprem(); break;
                case Opcode.fptan: RewriteFptan(); break;
                case Opcode.frndint: RewriteFUnary("__rndint"); break;
                case Opcode.frstor: RewriteFrstor(); break;
                case Opcode.fsave: RewriteFsave(); break;
                case Opcode.fscale: RewriteFscale(); break;
                case Opcode.fsin: RewriteFUnary("sin"); break;
                case Opcode.fsincos: RewriteFsincos(); break;
                case Opcode.fsqrt: RewriteFUnary("sqrt"); break;
                case Opcode.fst: RewriteFst(false); break;
                case Opcode.fstenv: RewriteFstenv(); break;
                case Opcode.fstcw: RewriterFstcw(); break;
                case Opcode.fstp: RewriteFst(true); break;
                case Opcode.fstsw: RewriteFstsw(); break;
                case Opcode.fsub: EmitCommonFpuInstruction(m.FSub, false, false); break;
                case Opcode.fsubp: EmitCommonFpuInstruction(m.FSub, false, true); break;
                case Opcode.fsubr: EmitCommonFpuInstruction(m.FSub, true, false); break;
                case Opcode.fsubrp: EmitCommonFpuInstruction(m.FSub, true, true); break;
                case Opcode.ftst: RewriteFtst(); break;
                case Opcode.fucom: RewriteFcom(0); break;
                case Opcode.fucomp: RewriteFcom(1); break;
                case Opcode.fucompp: RewriteFcom(2); break;
                case Opcode.fucomi: RewriteFcomi(false); break;
                case Opcode.fucomip: RewriteFcomi(true); break;
                case Opcode.fxam: RewriteFxam(); break;
                case Opcode.fxch: RewriteExchange(); break;
                case Opcode.fyl2x: RewriteFyl2x(); break;
                case Opcode.fyl2xp1: RewriteFyl2xp1(); break;
                case Opcode.hlt: RewriteHlt(); break;
                case Opcode.idiv: RewriteDivide(m.SDiv, Domain.SignedInt); break;
                case Opcode.@in: RewriteIn(); break;
                case Opcode.imul: RewriteMultiply(Operator.SMul, Domain.SignedInt); break;
                case Opcode.inc: RewriteIncDec(1); break;
                case Opcode.insb: RewriteStringInstruction(); break;
                case Opcode.ins: RewriteStringInstruction(); break;
                case Opcode.@int: RewriteInt(); break;
                case Opcode.into: RewriteInto(); break;
                case Opcode.invd: RewriteInvd(); break;
                case Opcode.iret: RewriteIret(); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.ja: RewriteConditionalGoto(ConditionCode.UGT, instrCur.op1); break;
                case Opcode.jbe: RewriteConditionalGoto(ConditionCode.ULE, instrCur.op1); break;
                case Opcode.jc: RewriteConditionalGoto(ConditionCode.ULT, instrCur.op1); break;
                case Opcode.jcxz: RewriteJcxz(); break;
                case Opcode.jge: RewriteConditionalGoto(ConditionCode.GE, instrCur.op1); break;
                case Opcode.jg: RewriteConditionalGoto(ConditionCode.GT, instrCur.op1); break;
                case Opcode.jl: RewriteConditionalGoto(ConditionCode.LT, instrCur.op1); break;
                case Opcode.jle: RewriteConditionalGoto(ConditionCode.LE, instrCur.op1); break;
                case Opcode.jnc: RewriteConditionalGoto(ConditionCode.UGE, instrCur.op1); break;
                case Opcode.jno: RewriteConditionalGoto(ConditionCode.NO, instrCur.op1); break;
                case Opcode.jns: RewriteConditionalGoto(ConditionCode.NS, instrCur.op1); break;
                case Opcode.jnz: RewriteConditionalGoto(ConditionCode.NE, instrCur.op1); break;
                case Opcode.jo: RewriteConditionalGoto(ConditionCode.OV, instrCur.op1); break;
                case Opcode.jpe: RewriteConditionalGoto(ConditionCode.PE, instrCur.op1); break;
                case Opcode.jpo: RewriteConditionalGoto(ConditionCode.PO, instrCur.op1); break;
                case Opcode.js: RewriteConditionalGoto(ConditionCode.SG, instrCur.op1); break;
                case Opcode.jz: RewriteConditionalGoto(ConditionCode.EQ, instrCur.op1); break;
                case Opcode.lahf: RewriteLahf(); break;
                case Opcode.lar: RewriteLar(); break;
                case Opcode.lds: RewriteLxs(Registers.ds); break;
                case Opcode.lea: RewriteLea(); break;
                case Opcode.leave: RewriteLeave(); break;
                case Opcode.les: RewriteLxs(Registers.es); break;
                case Opcode.lfence: RewriteLfence(); break;
                case Opcode.lfs: RewriteLxs(Registers.fs); break;
                case Opcode.lgs: RewriteLxs(Registers.gs); break;
                case Opcode.@lock: RewriteLock(); break;
                case Opcode.lods: RewriteStringInstruction(); break;
                case Opcode.lodsb: RewriteStringInstruction(); break;
                case Opcode.loop: RewriteLoop(0, ConditionCode.EQ); break;
                case Opcode.loope: RewriteLoop(FlagM.ZF, ConditionCode.EQ); break;
                case Opcode.loopne: RewriteLoop(FlagM.ZF, ConditionCode.NE); break;
                case Opcode.lsl: RewriteLsl(); break;
                case Opcode.lss: RewriteLxs(Registers.ss); break;
                case Opcode.maskmovq: RewriteMaskmovq(); break;
                case Opcode.maxps: RewritePackedBinop("__maxps", PrimitiveType.Real32); break;
                case Opcode.mfence: RewriteMfence(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movapd:
                case Opcode.movaps:
                case Opcode.vmovapd:
                case Opcode.vmovaps: RewriteMov(); break;
                case Opcode.movd: RewriteMovzx(); break;
                case Opcode.movdqa: RewriteMov(); break;
                case Opcode.movhpd: RewritePackedUnaryop("__movhpd", PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.movhps: RewritePackedUnaryop("__movhps", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.movlps: RewritePackedUnaryop("__movlps", PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.movlhps: RewriteMovlhps(); break;
                case Opcode.movmskpd: RewriteMovmsk("__movmskpd", PrimitiveType.Real64); break;
                case Opcode.movmskps: RewriteMovmsk("__movmskps", PrimitiveType.Real32); break;
                case Opcode.movnti: RewriteMov(); break;
                case Opcode.movntps: RewriteMov(); break;
                case Opcode.movntq: RewriteMov(); break;
                case Opcode.movq: RewriteMov(); break;
                case Opcode.movs: RewriteStringInstruction(); break;
                case Opcode.movsb: RewriteStringInstruction(); break;
                case Opcode.movsd:
                case Opcode.vmovsd: RewriteMovssd(PrimitiveType.Real64); break;
                case Opcode.movss:
                case Opcode.vmovss: RewriteMovssd(PrimitiveType.Real32); break;
                case Opcode.minps: RewritePackedBinop("__minps", PrimitiveType.Real32); break;
                case Opcode.movsx: RewriteMovsx(); break;
                case Opcode.movups: RewriteMov(); break;
                case Opcode.movupd: RewriteMov(); break;
                case Opcode.movzx: RewriteMovzx(); break;
                case Opcode.mul: RewriteMultiply(Operator.UMul, Domain.UnsignedInt); break;
                case Opcode.mulpd: RewritePackedBinop("__mulpd", PrimitiveType.Real64); break;
                case Opcode.mulps: RewritePackedBinop("__mulps", PrimitiveType.Real32); break;
                case Opcode.mulss: RewriteScalarBinop(m.FMul, PrimitiveType.Real32); break;
                case Opcode.mulsd: RewriteScalarBinop(m.FMul, PrimitiveType.Real64); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.not: RewriteNot(); break;
                case Opcode.or: RewriteLogical(BinaryOperator.Or); break;
                case Opcode.orps: RewriteOrps(); break;
                case Opcode.@out: RewriteOut(); break;
                case Opcode.@outs: RewriteStringInstruction(); break;
                case Opcode.@outsb: RewriteStringInstruction(); break;
                case Opcode.packssdw: RewritePackedBinop("__packssdw", PrimitiveType.Int32, new ArrayType(PrimitiveType.Int16, 0)); break;
                case Opcode.packuswb: RewritePackedBinop("__packuswb", PrimitiveType.UInt16, new ArrayType(PrimitiveType.UInt8, 0)); break;
                case Opcode.paddb: RewritePackedBinop("__paddb", PrimitiveType.Byte); break;
                case Opcode.paddd: RewritePackedBinop("__paddd", PrimitiveType.Word32); break;
                case Opcode.paddsw: RewritePackedBinop("__paddsw", PrimitiveType.Word16); break;
                case Opcode.paddusb: RewritePackedBinop("__paddusb", PrimitiveType.Byte); break;
                case Opcode.paddusw: RewritePackedBinop("__paddsw", PrimitiveType.Word16); break;
                case Opcode.paddw: RewritePackedBinop("__paddw", PrimitiveType.Word16); break;
                case Opcode.pause: RewritePause(); break;
                case Opcode.palignr: RewritePalignr(); break;
                case Opcode.pavgb: RewritePavg("__pavgb", PrimitiveType.Byte); break;
                case Opcode.pavgw: RewritePavg("__pavgw", PrimitiveType.Byte); break;
                case Opcode.pcmpeqb: RewritePcmp("__pcmpeqb", PrimitiveType.Byte); break;
                case Opcode.pcmpeqd: RewritePcmp("__pcmpeqd", PrimitiveType.Word32); break;
                case Opcode.pcmpeqw: RewritePcmp("__pcmpeqw", PrimitiveType.Word16); break;
                case Opcode.pcmpgtb: RewritePcmp("__pcmpgtb", PrimitiveType.Byte); break;
                case Opcode.pcmpgtd: RewritePcmp("__pcmpgtd", PrimitiveType.Word32); break;
                case Opcode.pcmpgtw: RewritePcmp("__pcmpgtw", PrimitiveType.Word16); break;
                case Opcode.pextrw: RewritePextrw(); break;
                case Opcode.pinsrw: RewritePinsrw(); break;
                case Opcode.pmaddwd: RewritePackedBinop("__pmaddwd", PrimitiveType.Word16, new ArrayType(PrimitiveType.Word32, 0)); break;
                case Opcode.pmaxsw: RewritePackedBinop("__pmaxsw", PrimitiveType.Int16); break;
                case Opcode.pmaxub: RewritePackedBinop("__pmaxub", PrimitiveType.UInt8); break;
                case Opcode.pminsw: RewritePackedBinop("__pminsw", PrimitiveType.Int16); break;
                case Opcode.pminub: RewritePackedBinop("__pminub", PrimitiveType.UInt8); break;
                case Opcode.pmovmskb: RewriteMovmsk("__pmovmskb", PrimitiveType.Byte); break;
                case Opcode.pmulhuw: RewritePackedBinop("__pmulhuw", PrimitiveType.UInt16, new ArrayType(PrimitiveType.UInt16, 8)); break;
                case Opcode.pmulhw: RewritePackedBinop("__pmulhw", PrimitiveType.Int16, new ArrayType(PrimitiveType.Int16, 8)); break;
                case Opcode.pmullw: RewritePackedBinop("__pmullw", PrimitiveType.Int16, new ArrayType(PrimitiveType.Int16, 8)); break;
                case Opcode.pmuludq: RewritePackedBinop("__pmuludq", PrimitiveType.UInt32, new ArrayType(PrimitiveType.UInt64, 0)); break;
                case Opcode.prefetchw: RewritePrefetch("__prefetchw"); break;
                case Opcode.psadbw: RewritePackedBinop("__psadbw", PrimitiveType.Byte, PrimitiveType.Word16); break;
                case Opcode.pslld: RewritePackedBinop("__pslld", PrimitiveType.Word32); break;
                case Opcode.psllq: RewritePackedBinop("__psllq", PrimitiveType.Word64); break;
                case Opcode.psllw: RewritePackedBinop("__psllw", PrimitiveType.Word16); break;
                case Opcode.psrad: RewritePackedBinop("__psrad", PrimitiveType.Int32); break;
                case Opcode.psraw: RewritePackedBinop("__psraw", PrimitiveType.Int16); break;
                case Opcode.psrlq: RewritePackedBinop("__psrlq", PrimitiveType.Word64); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.popa: RewritePopa(); break;
                case Opcode.popf: RewritePopf(); break;
                case Opcode.por: RewritePor(); break;
                case Opcode.prefetchnta: RewritePrefetch("__prefetchnta"); break;
                case Opcode.prefetcht0: RewritePrefetch("__prefetcht0"); break;
                case Opcode.prefetcht1: RewritePrefetch("__prefetcht1"); break;
                case Opcode.prefetcht2: RewritePrefetch("__prefetcht2"); break;
                case Opcode.pshufd: RewritePshufd(); break;
                case Opcode.psubb: RewritePackedBinop("__psubb", PrimitiveType.Byte); break;
                case Opcode.psubd: RewritePackedBinop("__psubd", PrimitiveType.Word32); break;
                case Opcode.psubq: RewritePackedBinop("__psubq", PrimitiveType.Word64); break;
                case Opcode.psubsb: RewritePackedBinop("__psubsb", PrimitiveType.SByte); break;
                case Opcode.psubsw: RewritePackedBinop("__psubsw", PrimitiveType.Word16); break;
                case Opcode.psubusb: RewritePackedBinop("__psubusb", PrimitiveType.UInt8); break;
                case Opcode.psubw: RewritePackedBinop("__psubw", PrimitiveType.Word16); break;
                case Opcode.punpckhbw: RewritePunpckhbw(); break;
                case Opcode.punpckhdq: RewritePunpckhdq(); break;
                case Opcode.punpckhwd: RewritePunpckhwd(); break;
                case Opcode.punpcklbw: RewritePunpcklbw(); break;
                case Opcode.punpckldq: RewritePunpckldq(); break;
                case Opcode.punpcklwd: RewritePunpcklwd(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.pusha: RewritePusha(); break;
                case Opcode.pushf: RewritePushf(); break;
                case Opcode.pxor: RewritePxor(); break;
                case Opcode.rcl: RewriteRotation(PseudoProcedure.RolC, true, true); break;
                case Opcode.rcpps: RewritePackedUnaryop("__rcpps", PrimitiveType.Real32); break;
                case Opcode.rcr: RewriteRotation(PseudoProcedure.RorC, true, false); break;
                case Opcode.rol: RewriteRotation(PseudoProcedure.Rol, false, true); break;
                case Opcode.ror: RewriteRotation(PseudoProcedure.Ror, false, false); break;
                case Opcode.rdmsr: RewriteRdmsr(); break;
                case Opcode.rdpmc: RewriteRdpmc(); break;
                case Opcode.rdtsc: RewriteRdtsc(); break;
                case Opcode.ret: RewriteRet(); break;
                case Opcode.retf: RewriteRet(); break;
                case Opcode.rsqrtps: RewritePackedUnaryop("__rsqrtps", PrimitiveType.Real32); break;
                case Opcode.sahf: m.Assign(orw.FlagGroup(X86Instruction.DefCc(instrCur.code)), orw.AluRegister(Registers.ah)); break;
                case Opcode.sar: RewriteBinOp(Operator.Sar); break;
                case Opcode.sbb: RewriteAdcSbb(m.ISub); break;
                case Opcode.scas: RewriteStringInstruction(); break;
                case Opcode.scasb: RewriteStringInstruction(); break;
                case Opcode.seta: RewriteSet(ConditionCode.UGT); break;
                case Opcode.setc: RewriteSet(ConditionCode.ULT); break;
                case Opcode.setbe: RewriteSet(ConditionCode.ULE); break;
                case Opcode.setg: RewriteSet(ConditionCode.GT); break;
                case Opcode.setge: RewriteSet(ConditionCode.GE); break;
                case Opcode.setl: RewriteSet(ConditionCode.LT); break;
                case Opcode.setle: RewriteSet(ConditionCode.LE); break;
                case Opcode.setnc: RewriteSet(ConditionCode.UGE); break;
                case Opcode.setno: RewriteSet(ConditionCode.NO); break;
                case Opcode.setns: RewriteSet(ConditionCode.NS); break;
                case Opcode.setnz: RewriteSet(ConditionCode.NE); break;
                case Opcode.setpe: RewriteSet(ConditionCode.PE); break;
                case Opcode.setpo: RewriteSet(ConditionCode.PO); break;
                case Opcode.seto: RewriteSet(ConditionCode.OV); break;
                case Opcode.sets: RewriteSet(ConditionCode.SG); break;
                case Opcode.setz: RewriteSet(ConditionCode.EQ); break;
                case Opcode.sfence: RewriteSfence(); break;
                case Opcode.shl: RewriteBinOp(BinaryOperator.Shl); break;
                case Opcode.shld: RewriteShxd("__shld"); break;
                case Opcode.shr: RewriteBinOp(BinaryOperator.Shr); break;
                case Opcode.shrd: RewriteShxd("__shrd"); break;
                case Opcode.vshufps: RewritePackedTernaryop("__vshufps", PrimitiveType.Real32); break;
                case Opcode.sqrtps: RewritePackedUnaryop("__sqrtps", PrimitiveType.Real32); break;
                case Opcode.stc: RewriteSetFlag(FlagM.CF, Constant.True()); break;
                case Opcode.std: RewriteSetFlag(FlagM.DF, Constant.True()); break;
                case Opcode.sti: RewriteSti(); break;
                case Opcode.stos: RewriteStringInstruction(); break;
                case Opcode.stosb: RewriteStringInstruction(); break;
                case Opcode.sub: RewriteAddSub(BinaryOperator.ISub); break;
                case Opcode.subsd: RewriteScalarBinop(m.FSub, PrimitiveType.Real64); break;
                case Opcode.subss: RewriteScalarBinop(m.FSub, PrimitiveType.Real32); break;
                case Opcode.subpd: RewritePackedBinop("__subpd", PrimitiveType.Real64); break;
                case Opcode.subps: RewritePackedBinop("__subps", PrimitiveType.Real32); break;
                case Opcode.syscall: RewriteSyscall(); break;
                case Opcode.sysenter: RewriteSysenter(); break;
                case Opcode.sysexit: RewriteSysexit(); break;
                case Opcode.sysret: RewriteSysret(); break;
                case Opcode.ucomiss: RewriteComis(PrimitiveType.Real32); break;
                case Opcode.ucomisd: RewriteComis(PrimitiveType.Real64); break;
                case Opcode.ud2: rtlc = RtlClass.Invalid; m.Invalid(); break;
                case Opcode.unpcklpd: RewritePackedBinop("__unpcklpd", PrimitiveType.Real64); break;
                case Opcode.unpcklps: RewritePackedBinop("__unpcklps", PrimitiveType.Real32); break;
                case Opcode.test: RewriteTest(); break;
                case Opcode.wait: RewriteWait(); break;
                case Opcode.wbinvd: RewriteWbinvd(); break;
                case Opcode.wrmsr: RewriteWrsmr(); break;
                case Opcode.xadd: RewriteXadd(); break;
                case Opcode.xchg: RewriteExchange(); break;
                case Opcode.xgetbv: RewriteXgetbv(); break;
                case Opcode.xsetbv: RewriteXsetbv(); break;
                case Opcode.xlat: RewriteXlat(); break;
                case Opcode.xor: RewriteLogical(BinaryOperator.Xor); break;
                case Opcode.xorpd:
                case Opcode.vxorpd: RewritePackedBinop("__xorpd", PrimitiveType.Word64); break;
                case Opcode.xorps: RewritePackedBinop("__xorps", PrimitiveType.Word32); break;

                case Opcode.BOR_exp: RewriteFUnary("exp"); break;
                case Opcode.BOR_ln: RewriteFUnary("log"); break;
                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
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
                EmitCcInstr(dst, X86Instruction.DefCc(instrCur.code));
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
                // Special case for X86-64: 
                var reg = (RegisterStorage)idDst.Storage;
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
    }
}
