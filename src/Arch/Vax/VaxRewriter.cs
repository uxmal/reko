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

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly VaxArchitecture arch;
        private readonly IEnumerator<VaxInstruction> dasm;
        private readonly List<RtlInstruction> rtlInstructions;
        private readonly RtlEmitter m;
        private InstrClass iclass;
        private VaxInstruction instr;

        public VaxRewriter(VaxArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new VaxDisassembler(arch, rdr).GetEnumerator();
            this.rtlInstructions = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtlInstructions);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = this.instr.Address;
                var len = this.instr.Length;
                this.iclass = this.instr.InstructionClass;
                switch (this.instr.Mnemonic)
                {
                default:
                    EmitUnitTest();

                    //emitter.SideEffect(Constant.String(
                    //    this.instr.ToString(),
                    //    StringType.NullTerminated(PrimitiveType.Char)));
                    //host.Warn(
                    //    this.instr.Address,
                    //    "VAX instruction {0} not supported yet.",
                    //    this.instr.Mnemonic);
                    m.Invalid();
                    break;
                case Mnemonic.Invalid: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.Reserved: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.acbb: RewriteAcbi(PrimitiveType.Byte); break;
                case Mnemonic.acbd: RewriteAcbf(PrimitiveType.Real64); break;
                case Mnemonic.acbf: RewriteAcbf(PrimitiveType.Real32); break;
                case Mnemonic.acbg: RewriteAcbf(PrimitiveType.Real64); break;
                case Mnemonic.acbh: RewriteAcbf(PrimitiveType.Real128); break;
                case Mnemonic.acbl: RewriteAcbi(PrimitiveType.Word32); break;
                case Mnemonic.acbw: RewriteAcbi(PrimitiveType.Word16); break;
                case Mnemonic.adawi: RewriteAlu2(PrimitiveType.Word16, Adawi, AllFlags); break;
                case Mnemonic.addb2: RewriteAlu2(PrimitiveType.Byte, m.IAdd, AllFlags); break;
                case Mnemonic.addb3: RewriteAlu3(PrimitiveType.Byte, m.IAdd, AllFlags); break;
                case Mnemonic.addd2: RewriteFpu2(PrimitiveType.Real64, m.FAdd, NZ00); break;
                case Mnemonic.addd3: RewriteFpu3(PrimitiveType.Real64, m.FAdd, NZ00); break;
                case Mnemonic.addf2: RewriteFpu2(PrimitiveType.Real32, m.FAdd, NZ00); break;
                case Mnemonic.addf3: RewriteFpu3(PrimitiveType.Real32, m.FAdd, NZ00); break;
                case Mnemonic.addl2: RewriteAlu2(PrimitiveType.Word32, m.IAdd, AllFlags); break;
                case Mnemonic.addl3: RewriteAlu3(PrimitiveType.Word32, m.IAdd, AllFlags); break;
                case Mnemonic.addp4: RewriteP4(addp4); break;
                case Mnemonic.addp6: RewriteP6(addp6); break;
                case Mnemonic.addw2: RewriteAlu2(PrimitiveType.Word16, m.IAdd, AllFlags); break;
                case Mnemonic.addw3: RewriteAlu3(PrimitiveType.Word16, m.IAdd, AllFlags); break;
                case Mnemonic.adwc: RewriteAdwc(); break;
                case Mnemonic.aobleq: RewriteAob(m.Le); break;
                case Mnemonic.aoblss: RewriteAob(m.Lt); break;
                case Mnemonic.ashl: RewriteAsh(PrimitiveType.Word32); break;
                case Mnemonic.ashp: RewriteAshp(); break;
                case Mnemonic.ashq: RewriteAsh(PrimitiveType.Word64); break;
                case Mnemonic.bbc: RewriteBb(false); break;
                case Mnemonic.bbcc: RewriteBbxx(false, false); break;
                case Mnemonic.bbcci: RewriteBbxxi(false); break;
                case Mnemonic.bbcs: RewriteBbxx(false, true); break;
                case Mnemonic.bbs: RewriteBb(true); break;
                case Mnemonic.bbsc: RewriteBbxx(true, false); break;
                case Mnemonic.bbss: RewriteBbxx(true, true); break;
                case Mnemonic.bbssi: RewriteBbxxi(true); break;
                case Mnemonic.beql: RewriteBranch(ConditionCode.EQ, Registers.Z); break;
                case Mnemonic.bgeq: RewriteBranch(ConditionCode.GE, Registers.N); break;
                case Mnemonic.bgequ: RewriteBranch(ConditionCode.UGE, Registers.C); break;
                case Mnemonic.bgtr: RewriteBranch(ConditionCode.GT, Registers.ZN); break;
                case Mnemonic.bgtru: RewriteBranch(ConditionCode.UGT, Registers.CZ); break;
                case Mnemonic.bleq: RewriteBranch(ConditionCode.LE, Registers.ZN); break;
                case Mnemonic.blequ: RewriteBranch(ConditionCode.ULE, Registers.CZ); break;
                case Mnemonic.blss: RewriteBranch(ConditionCode.LT, Registers.N); break;
                case Mnemonic.blssu: RewriteBranch(ConditionCode.ULT, Registers.C); break;
                case Mnemonic.bneq: RewriteBranch(ConditionCode.NE, Registers.Z); break;
                case Mnemonic.bvc: RewriteBranch(ConditionCode.NO, Registers.V); break;
                case Mnemonic.bvs: RewriteBranch(ConditionCode.OV, Registers.V); break;
                case Mnemonic.bicb2: RewriteAlu2(PrimitiveType.Byte, Bic, NZ00); break;
                case Mnemonic.bicb3: RewriteAlu3(PrimitiveType.Byte, Bic, NZ00); break;
                case Mnemonic.bicl2: RewriteAlu2(PrimitiveType.Word32, Bic, NZ00); break;
                case Mnemonic.bicl3: RewriteAlu3(PrimitiveType.Word32, Bic, NZ00); break;
                case Mnemonic.bicpsw: RewriteBicpsw(); break;
                case Mnemonic.bicw2: RewriteAlu2(PrimitiveType.Word16, Bic, NZ00); break;
                case Mnemonic.bicw3: RewriteAlu3(PrimitiveType.Word16, Bic, NZ00); break;
                case Mnemonic.bisb2: RewriteAlu2(PrimitiveType.Byte, m.Or, NZ00); break;
                case Mnemonic.bisb3: RewriteAlu3(PrimitiveType.Byte, m.Or, NZ00); break;
                case Mnemonic.bispsw: RewriteBispsw(); break;
                case Mnemonic.bisl2: RewriteAlu2(PrimitiveType.Word32, m.Or, NZ00); break;
                case Mnemonic.bisl3: RewriteAlu3(PrimitiveType.Word32, m.Or, NZ00); break;
                case Mnemonic.bisw2: RewriteAlu2(PrimitiveType.Word16, m.Or, NZ00); break;
                case Mnemonic.bisw3: RewriteAlu3(PrimitiveType.Word16, m.Or, NZ00); break;
                case Mnemonic.bitb: RewriteBit(PrimitiveType.Byte); break;
                case Mnemonic.bitw: RewriteBit(PrimitiveType.Byte); break;
                case Mnemonic.bitl: RewriteBit(PrimitiveType.Byte); break;
                case Mnemonic.blbc: RewriteBlb(m.Eq0); break;
                case Mnemonic.blbs: RewriteBlb(m.Ne0); break;
                case Mnemonic.bpt: RewriteBpt(); break;
                case Mnemonic.brb: RewriteBranch(); break;
                case Mnemonic.brw: RewriteBranch(); break;
                case Mnemonic.bsbb: RewriteBsb(); break;
                case Mnemonic.bsbw: RewriteBsb(); break;

                case Mnemonic.callg: RewriteCallg(); break;
                case Mnemonic.calls: RewriteCalls(); break;
                case Mnemonic.caseb: RewriteCase(PrimitiveType.Byte); break;
                case Mnemonic.casel: RewriteCase(PrimitiveType.Word32); break;
                case Mnemonic.casew: RewriteCase(PrimitiveType.Word16); break;
                case Mnemonic.chme: RewriteChm(chme); break;
                case Mnemonic.chmk: RewriteChmk(); break;
                case Mnemonic.chms: RewriteChm(chms); break;
                case Mnemonic.chmu: RewriteChm(chmu); break;

                case Mnemonic.clrb: RewriteClr(PrimitiveType.Byte); break;
                case Mnemonic.clrh: RewriteClr(PrimitiveType.Word128); break;
                case Mnemonic.clrl: RewriteClr(PrimitiveType.Word32); break;
                case Mnemonic.clrq: RewriteClr(PrimitiveType.Word64); break;
                case Mnemonic.clrw: RewriteClr(PrimitiveType.Word16); break;
                case Mnemonic.cmpb: RewriteCmp(PrimitiveType.Byte); break;
                case Mnemonic.cmpd: RewriteCmp(PrimitiveType.Real64); break;
                case Mnemonic.cmpf: RewriteCmp(PrimitiveType.Real32); break;
                case Mnemonic.cmpg: RewriteCmp(PrimitiveType.Real64); break;
                case Mnemonic.cmph: RewriteCmp(PrimitiveType.Real128); break;
                case Mnemonic.cmpl: RewriteCmp(PrimitiveType.Word32); break;
                case Mnemonic.cmpw: RewriteCmp(PrimitiveType.Word16); break;
                case Mnemonic.cmpp3: RewriteCmpp3(); break;
                case Mnemonic.cmpp4: RewriteCmpp4(); break;
                case Mnemonic.cmpv: goto default;
                case Mnemonic.cmpzv: goto default;
                case Mnemonic.cvtbd: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Real64); break;
                case Mnemonic.cvtbf: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Real32); break;
                case Mnemonic.cvtbg: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Real64); break;
                case Mnemonic.cvtbh: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Real128); break;
                case Mnemonic.cvtbl: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.cvtbw: RewriteCvt(PrimitiveType.SByte, PrimitiveType.Int16); break;
                case Mnemonic.cvtdb: RewriteCvt(PrimitiveType.Real64, PrimitiveType.SByte); break;
                case Mnemonic.cvtdf: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.cvtdh: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real128); break;
                case Mnemonic.cvtdl: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvtdw: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int16); break;
                case Mnemonic.cvtfb: RewriteCvt(PrimitiveType.Real32, PrimitiveType.SByte); break;
                case Mnemonic.cvtfd: RewriteCvt(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.cvtfg: RewriteCvt(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.cvtfh: RewriteCvt(PrimitiveType.Real32, PrimitiveType.Real128); break;
                case Mnemonic.cvtfl: RewriteCvt(PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvtfw: RewriteCvt(PrimitiveType.Real32, PrimitiveType.Int16); break;
                case Mnemonic.cvtgb: RewriteCvt(PrimitiveType.Real64, PrimitiveType.SByte); break;
                case Mnemonic.cvtgf: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.cvtgh: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Real128); break;
                case Mnemonic.cvtgl: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvtgw: RewriteCvt(PrimitiveType.Real64, PrimitiveType.Int16); break; 
                case Mnemonic.cvthb: RewriteCvt(PrimitiveType.Real128, PrimitiveType.SByte); break;
                case Mnemonic.cvthd: RewriteCvt(PrimitiveType.Real128, PrimitiveType.Real64); break;
                case Mnemonic.cvthf: RewriteCvt(PrimitiveType.Real128, PrimitiveType.Real32); break;
                case Mnemonic.cvthg: RewriteCvt(PrimitiveType.Real128, PrimitiveType.Real64); break;
                case Mnemonic.cvthl: RewriteCvt(PrimitiveType.Real128, PrimitiveType.Int32); break;
                case Mnemonic.cvthw: RewriteCvt(PrimitiveType.Real128, PrimitiveType.Int16); break;
                case Mnemonic.cvtps: RewriteCvtComplex(cvtps); break;
                case Mnemonic.cvtrdl: RewriteCvtr(PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvtrfl: RewriteCvtr(PrimitiveType.Real32, PrimitiveType.Int32); break;
                case Mnemonic.cvtrgl: RewriteCvtr(PrimitiveType.Real64, PrimitiveType.Int32); break;
                case Mnemonic.cvtrhl: RewriteCvtr(PrimitiveType.Real128, PrimitiveType.Int32); break;
                case Mnemonic.cvtlb: RewriteCvt(PrimitiveType.Int32, PrimitiveType.SByte); break;
                case Mnemonic.cvtld: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Real64); break;
                case Mnemonic.cvtlf: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Real32); break;
                case Mnemonic.cvtlg: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Real64); break;
                case Mnemonic.cvtlh: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Real128); break;
                case Mnemonic.cvtlp: goto default;
                case Mnemonic.cvtlw: RewriteCvt(PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.cvtpl: goto default;
                case Mnemonic.cvtpt: goto default;
                case Mnemonic.cvtsp: RewriteCvtComplex(cvtsp); break;
                case Mnemonic.cvttp: goto default;
                case Mnemonic.cvtwb: RewriteCvt(PrimitiveType.Int16, PrimitiveType.SByte); break;
                case Mnemonic.cvtwd: RewriteCvt(PrimitiveType.Int16, PrimitiveType.Real64); break;
                case Mnemonic.cvtwf: RewriteCvt(PrimitiveType.Int16, PrimitiveType.Real32); break;
                case Mnemonic.cvtwg: RewriteCvt(PrimitiveType.Int16, PrimitiveType.Real64); break;
                case Mnemonic.cvtwh: RewriteCvt(PrimitiveType.Int16, PrimitiveType.Real128); break;
                case Mnemonic.cvtwl: RewriteCvt(PrimitiveType.Int16, PrimitiveType.Int32); break; ;
                case Mnemonic.decb: RewriteIncDec(PrimitiveType.Byte, Dec); break;
                case Mnemonic.decl: RewriteIncDec(PrimitiveType.Word32, Dec); break;
                case Mnemonic.decw: RewriteIncDec(PrimitiveType.Word16, Dec); break;
                case Mnemonic.divb2: RewriteAlu2(PrimitiveType.Byte, m.SDiv, AllFlags); break;
                case Mnemonic.divb3: RewriteAlu3(PrimitiveType.Byte, m.SDiv, AllFlags); break;
                case Mnemonic.divd2: RewriteFpu2(PrimitiveType.Real64, m.FDiv, AllFlags); break;
                case Mnemonic.divd3: RewriteFpu3(PrimitiveType.Real64, m.FDiv, AllFlags); break;
                case Mnemonic.divf2: RewriteFpu2(PrimitiveType.Real32, m.FDiv, AllFlags); break;
                case Mnemonic.divf3: RewriteFpu3(PrimitiveType.Real32, m.FDiv, AllFlags); break;
                case Mnemonic.divl2: RewriteAlu2(PrimitiveType.Word32, m.SDiv, AllFlags); break;
                case Mnemonic.divl3: RewriteAlu3(PrimitiveType.Word32, m.SDiv, AllFlags); break;
                case Mnemonic.divp: RewriteDivp(); break;
                case Mnemonic.divw2: RewriteAlu2(PrimitiveType.Word16, m.SDiv, AllFlags); break;
                case Mnemonic.divw3: RewriteAlu3(PrimitiveType.Word16, m.SDiv, AllFlags); break;
                case Mnemonic.emodd: RewriteEmod(emodd, PrimitiveType.Real64 , PrimitiveType.Byte); break; //$TODO: VAX floating point types
                case Mnemonic.emodf: RewriteEmod(emodf, PrimitiveType.Real32 , PrimitiveType.Byte); break; //$TODO: VAX floating point types
                case Mnemonic.emodg: RewriteEmod(emodg, PrimitiveType.Real64 , PrimitiveType.Word16); break; //$TODO: VAX floating point types
                case Mnemonic.emodh: RewriteEmod(emodh, PrimitiveType.Real128, PrimitiveType.Word16); break; //$TODO: VAX floating point types
                case Mnemonic.extv: RewriteExtv(Domain.SignedInt); break;
                case Mnemonic.extzv: RewriteExtv(Domain.UnsignedInt); break;

                case Mnemonic.ffc: RewriteFfx(ffc); break;
                case Mnemonic.ffs: RewriteFfx(ffs); break;
                case Mnemonic.halt: RewriteHalt(); break;
                case Mnemonic.incb: RewriteIncDec(PrimitiveType.Byte, Inc); break;
                case Mnemonic.incl: RewriteIncDec(PrimitiveType.Word32, Inc); break;
                case Mnemonic.incw: RewriteIncDec(PrimitiveType.Word16, Inc); break;
                case Mnemonic.insque: RewriteInsque(); break;
                case Mnemonic.jmp: RewriteJmp(); break;
                case Mnemonic.jsb: RewriteJsb(); break;

                case Mnemonic.mcomb: RewriteAluUnary2(PrimitiveType.Byte, m.Comp, NZ00); break;
                case Mnemonic.mcoml: RewriteAluUnary2(PrimitiveType.Word32, m.Comp, NZ00); break;
                case Mnemonic.mcomw: RewriteAluUnary2(PrimitiveType.Word16, m.Comp, NZ00); break;
                case Mnemonic.mnegb: RewriteAluUnary2(PrimitiveType.Byte, m.Neg, AllFlags); break;
                case Mnemonic.mnegd: RewriteAluUnary2(PrimitiveType.Real64, m.FNeg, NZ00); break;
                case Mnemonic.mnegf: RewriteAluUnary2(PrimitiveType.Real32, m.FNeg, NZ00); break;
                case Mnemonic.mnegg: RewriteAluUnary2(PrimitiveType.Real64, m.FNeg, NZ00); break;
                case Mnemonic.mnegh: RewriteAluUnary2(PrimitiveType.Word128, m.FNeg, NZ00); break;
                case Mnemonic.mnegl: RewriteAluUnary2(PrimitiveType.Word32, m.Neg, AllFlags); break;
                case Mnemonic.mnegw: RewriteAluUnary2(PrimitiveType.Word16, m.Neg, AllFlags); break;

                case Mnemonic.movab: RewriteMova(PrimitiveType.Byte); break;
                case Mnemonic.movah: RewriteMova(PrimitiveType.Real128); break;
                case Mnemonic.moval: RewriteMova(PrimitiveType.Word32); break;
                case Mnemonic.movaq: RewriteMova(PrimitiveType.Word64); break;
                case Mnemonic.movaw: RewriteMova(PrimitiveType.Word16); break;
                    
                case Mnemonic.movb:   RewriteAluUnary2(PrimitiveType.Byte, Copy, NZ00); break;
                case Mnemonic.movc3:  goto default;
                case Mnemonic.movc5:  goto default;
                case Mnemonic.movd:   RewriteAluUnary2(PrimitiveType.Real64, Copy, NZ00); break;
                case Mnemonic.movf:   RewriteAluUnary2(PrimitiveType.Real32, Copy, NZ00); break;
                case Mnemonic.movg:   RewriteAluUnary2(PrimitiveType.Real64, Copy, NZ00); break;
                case Mnemonic.movh:   RewriteAluUnary2(PrimitiveType.Real128,Copy, NZ00); break;
                case Mnemonic.movl:   RewriteAluUnary2(PrimitiveType.Word32, Copy, NZ00); break;
                case Mnemonic.movo:   RewriteAluUnary2(PrimitiveType.Word128,Copy, NZ00); break;
                case Mnemonic.movp:   RewriteMovp(); break;
                case Mnemonic.movq:   RewriteAluUnary2(PrimitiveType.Word64, Copy, NZ00); break;
                case Mnemonic.movw:   RewriteAluUnary2(PrimitiveType.Word16, Copy, NZ00); break;
                case Mnemonic.movzbl: RewriteMovz(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.movzbw: RewriteMovz(PrimitiveType.Byte, PrimitiveType.UInt16); break;
                case Mnemonic.movzwl: RewriteMovz(PrimitiveType.Word16, PrimitiveType.UInt32); break;
                case Mnemonic.mulb2: RewriteAlu2(PrimitiveType.Byte, m.IMul, AllFlags); break;
                case Mnemonic.mulb3: RewriteAlu3(PrimitiveType.Byte, m.IMul, AllFlags); break;
                case Mnemonic.muld2: RewriteAlu2(PrimitiveType.Real64, m.FMul, AllFlags); break;
                case Mnemonic.muld3: RewriteAlu3(PrimitiveType.Real64, m.FMul, AllFlags); break;
                case Mnemonic.mulf2: RewriteAlu2(PrimitiveType.Real32, m.FMul, AllFlags); break;
                case Mnemonic.mulf3: RewriteAlu3(PrimitiveType.Real32, m.FMul, AllFlags); break;
                case Mnemonic.mulg2: RewriteAlu2(PrimitiveType.Real64, m.FMul, AllFlags); break;
                case Mnemonic.mulg3: RewriteAlu3(PrimitiveType.Real64, m.FMul, AllFlags); break;
                case Mnemonic.mulh2: RewriteAlu2(PrimitiveType.Real128, m.FMul, AllFlags); break;
                case Mnemonic.mulh3: RewriteAlu3(PrimitiveType.Real128, m.FMul, AllFlags); break;
                case Mnemonic.mull2: RewriteAlu2(PrimitiveType.Word32, m.IMul, AllFlags); break;
                case Mnemonic.mull3: RewriteAlu3(PrimitiveType.Word32, m.IMul, AllFlags); break;
                case Mnemonic.mulp: RewriteMulp(); break;
                case Mnemonic.mulw2: RewriteAlu2(PrimitiveType.Word16, m.IMul, AllFlags); break;
                case Mnemonic.mulw3: RewriteAlu3(PrimitiveType.Word16, m.IMul, AllFlags); break;
                case Mnemonic.nop: m.Nop(); break;

                case Mnemonic.polyd: RewritePoly(PrimitiveType.Real64); break;
                case Mnemonic.polyf: RewritePoly(PrimitiveType.Real32); break;
                case Mnemonic.polyg: RewritePoly(PrimitiveType.Real64); break;
                case Mnemonic.polyh: RewritePoly(PrimitiveType.Real128); break;

                case Mnemonic.popr:  goto default;
                case Mnemonic.prober: RewriteProber(); break;
                case Mnemonic.probew: goto default;

                case Mnemonic.pushab: RewritePusha(); break;
                case Mnemonic.pushal: RewritePusha(); break;
                case Mnemonic.pushah: RewritePusha(); break;
                case Mnemonic.pushaw: RewritePusha(); break;
                case Mnemonic.pushaq: RewritePusha(); break;
                case Mnemonic.pushl: RewritePush(PrimitiveType.Word32); break;
                case Mnemonic.pushr: RewritePushr(); break;

                case Mnemonic.rei: RewriteRei(); break;
                case Mnemonic.ret: RewriteRet(); break;
                case Mnemonic.rotl: RewriteAlu3(PrimitiveType.Word32, RewriteRotl, NZ00); break;
                case Mnemonic.rsb: RewriteRsb(); break;

                case Mnemonic.sbwc: RewriteSbwc(); break;
                case Mnemonic.scanc: RewriteScanc(); break;
                case Mnemonic.sobgeq: RewriteSob(m.Ge); break;
                case Mnemonic.sobgtr: RewriteSob(m.Gt); break;
                case Mnemonic.subb2: RewriteAlu2(PrimitiveType.Byte, m.ISub, AllFlags); break;
                case Mnemonic.subb3: RewriteAlu3(PrimitiveType.Byte, m.ISub, AllFlags); break;
                case Mnemonic.subd2: RewriteAlu2(PrimitiveType.Real64, m.FSub, NZ00); break;
                case Mnemonic.subd3: RewriteAlu3(PrimitiveType.Real64, m.FSub, NZ00); break;
                case Mnemonic.subf2: RewriteAlu2(PrimitiveType.Real32, m.FSub, NZ00); break;
                case Mnemonic.subf3: RewriteAlu3(PrimitiveType.Real32, m.FSub, NZ00); break;
                case Mnemonic.subg2: RewriteAlu2(PrimitiveType.Real64, m.FSub, NZ00); break;
                case Mnemonic.subg3: RewriteAlu3(PrimitiveType.Real64, m.FSub, NZ00); break;
                case Mnemonic.subh2: RewriteAlu2(PrimitiveType.Real128, m.FSub, NZ00); break;
                case Mnemonic.subh3: RewriteAlu3(PrimitiveType.Real128, m.FSub, NZ00); break;
                case Mnemonic.subl2: RewriteAlu2(PrimitiveType.Word32, m.ISub, AllFlags); break;
                case Mnemonic.subl3: RewriteAlu3(PrimitiveType.Word32, m.ISub, AllFlags); break;
                case Mnemonic.subp4: RewriteP4(subp4); break;
                case Mnemonic.subp6: RewriteP6(subp6); break;
                case Mnemonic.subw2: RewriteAlu2(PrimitiveType.Word16, m.ISub, AllFlags); break;
                case Mnemonic.subw3: RewriteAlu3(PrimitiveType.Word16, m.ISub, AllFlags); break;
                case Mnemonic.tstb: RewriteTst(PrimitiveType.Byte, ICmp0); break;
                case Mnemonic.tstd: RewriteTst(PrimitiveType.Real64, FCmp0); break;
                case Mnemonic.tstf: RewriteTst(PrimitiveType.Real32, FCmp0); break;
                case Mnemonic.tstg: RewriteTst(PrimitiveType.Real64, FCmp0); break;
                case Mnemonic.tsth: RewriteTst(PrimitiveType.Real128, FCmp0); break;
                case Mnemonic.tstl: RewriteTst(PrimitiveType.Word32, ICmp0); break;
                case Mnemonic.tstw: RewriteTst(PrimitiveType.Word16, ICmp0); break;
                case Mnemonic.xorb2: RewriteAlu2(PrimitiveType.Byte, m.Xor, NZ00); break;
                case Mnemonic.xorb3: RewriteAlu3(PrimitiveType.Byte, m.Xor, NZ00); break;
                case Mnemonic.xorl2: RewriteAlu2(PrimitiveType.Word32, m.Xor, NZ00); break;
                case Mnemonic.xorl3: RewriteAlu3(PrimitiveType.Word32, m.Xor, NZ00); break;
                case Mnemonic.xorw2: RewriteAlu2(PrimitiveType.Word16, m.Xor, NZ00); break;
                case Mnemonic.xorw3: RewriteAlu3(PrimitiveType.Word16, m.Xor, NZ00); break;

                case Mnemonic.ldpctx: goto default;
                case Mnemonic.svpctx: goto default;
                case Mnemonic.editpc: goto default;
                case Mnemonic.matchc: goto default;
                case Mnemonic.index: goto default;
                case Mnemonic.locc: goto default;
                case Mnemonic.crc: goto default;
                case Mnemonic.skpc: goto default;
                case Mnemonic.remque: goto default;
                case Mnemonic.spanc: goto default;
                case Mnemonic.insqhi: goto default;
                case Mnemonic.insqti: goto default;
                case Mnemonic.movtc: goto default;
                case Mnemonic.remqhi: goto default;
                case Mnemonic.movtuc: goto default;
                case Mnemonic.remqti: goto default;


                case Mnemonic.emul: goto default;
                case Mnemonic.ediv: goto default;
                case Mnemonic.insv: goto default;

                case Mnemonic.mtpr: goto default;
                case Mnemonic.mfpr: goto default;
                case Mnemonic.movpsl: goto default;
                case Mnemonic.xfc: goto default;
                case Mnemonic.mfvp: goto default;
                case Mnemonic.vldl: goto default;
                case Mnemonic.vgathl: goto default;
                case Mnemonic.vldq: goto default;
                case Mnemonic.vgathq: goto default;
                case Mnemonic.addg2: goto default;
                case Mnemonic.addg3: goto default;
                case Mnemonic.divg2: goto default;
                case Mnemonic.divg3: goto default;
                case Mnemonic.addh2: goto default;
                case Mnemonic.addh3: goto default;
                case Mnemonic.divh2: goto default;
                case Mnemonic.divh3: goto default;

                case Mnemonic.vstl: goto default;
                case Mnemonic.vscatl: goto default;
                case Mnemonic.vstq: goto default;
                case Mnemonic.vscatq: goto default;
                case Mnemonic.vvmull: goto default;
                case Mnemonic.vsmull: goto default;
                case Mnemonic.vvmulg: goto default;
                case Mnemonic.vsmulg: goto default;
                case Mnemonic.vvmulf: goto default;
                case Mnemonic.vsmulf: goto default;
                case Mnemonic.vvmuld: goto default;
                case Mnemonic.vsmuld: goto default;
                case Mnemonic.vsync: goto default;
                case Mnemonic.mtvp: goto default;
                case Mnemonic.vvdivg: goto default;
                case Mnemonic.vsdivg: goto default;
                case Mnemonic.vvdivf: goto default;
                case Mnemonic.vsdivf: goto default;
                case Mnemonic.vvdivd: goto default;
                case Mnemonic.vsdivd: goto default;
                case Mnemonic.vvaddl: goto default;
                case Mnemonic.vsaddl: goto default;
                case Mnemonic.vvaddg: goto default;
                case Mnemonic.vsaddg: goto default;
                case Mnemonic.vvaddf: goto default;
                case Mnemonic.vsaddf: goto default;
                case Mnemonic.vvaddd: goto default;
                case Mnemonic.vsaddd: goto default;
                case Mnemonic.vvsubl: goto default;
                case Mnemonic.vssubl: goto default;
                case Mnemonic.vvsubg: goto default;
                case Mnemonic.vssubg: goto default;
                case Mnemonic.vvsubf: goto default;
                case Mnemonic.vssubf: goto default;
                case Mnemonic.vvsubd: goto default;
                case Mnemonic.vssubd: goto default;
                case Mnemonic.vvcmpl: goto default;
                case Mnemonic.vvsrll: goto default;
                case Mnemonic.vscmpl: goto default;
                case Mnemonic.vssrll: goto default;
                case Mnemonic.vvcmpg: goto default;
                case Mnemonic.vscmpg: goto default;
                case Mnemonic.vvcmpf: goto default;
                case Mnemonic.vvslll: goto default;
                case Mnemonic.vscmpf: goto default;
                case Mnemonic.vsslll: goto default;
                case Mnemonic.vvcmpd: goto default;
                case Mnemonic.vscmpd: goto default;
                case Mnemonic.vvbisl: goto default;
                case Mnemonic.vvxorl: goto default;
                case Mnemonic.vsbisl: goto default;
                case Mnemonic.vsxorl: goto default;
                case Mnemonic.vvbicl: goto default;
                case Mnemonic.vvcvt: goto default;
                case Mnemonic.vsbicl: goto default;
                case Mnemonic.iota: goto default;
                case Mnemonic.vvmerge: goto default;
                case Mnemonic.vsmerge: goto default;

                case Mnemonic.bugl: goto default;
                case Mnemonic.bugw: goto default;
                }
                yield return m.MakeCluster(addr, len, iclass);
                this.rtlInstructions.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("VaxRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression RewriteSrcOp(int iOp, PrimitiveType width)
        {
            return RewriteSrcOp(this.instr.Operands[iOp], width);
        }

        private Expression RewriteSrcOp(MachineOperand op, PrimitiveType width)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var reg = binder.EnsureRegister(regOp);
                if (width.Size == 4)
                {
                    return reg;
                }
                else if (width.Size == 8)
                {
                    var rHi = arch.GetRegister(1 + (int) reg.Storage.Domain);
                    if (rHi is null)
                        return null!;
                    var regHi = binder.EnsureRegister(rHi);
                    return binder.EnsureSequence(width, regHi.Storage, reg.Storage);
                }
                else if (width.Size == 16)
                {
                    var regHi1 = binder.EnsureRegister(arch.GetRegister(1 + (int) reg.Storage.Domain)!);
                    var regHi2 = binder.EnsureRegister(arch.GetRegister(2 + (int) reg.Storage.Domain)!);
                    var regHi3 = binder.EnsureRegister(arch.GetRegister(3 + (int) reg.Storage.Domain)!);

                    var regLo = binder.EnsureSequence(PrimitiveType.Word64, regHi1.Storage, reg.Storage);
                    var regHi = binder.EnsureSequence(PrimitiveType.Word64, regHi3.Storage, regHi2.Storage);
                    return binder.EnsureSequence(width, regHi.Storage, regLo.Storage);
                }
                else
                {
                    return m.Slice(reg, width);
                }

            case Constant immOp:
                return immOp;

            case MemoryOperand memOp:
                Expression ea;
                if (memOp.Base is RegisterStorage rbaseOp)
                {
                    reg = binder.EnsureRegister(rbaseOp);
                    if (memOp.AutoDecrement)
                    {
                        m.Assign(reg, m.ISub(reg, width.Size));
                    }
                    else if (memOp.AutoIncrement)
                    {
                        var tmp = binder.CreateTemporary(reg.DataType);
                        m.Assign(tmp, reg);
                        reg = tmp;
                    }
                    ea = reg;
                    if (memOp.Offset is not null)
                    {
                        if (memOp.Offset.DataType.BitSize < ea.DataType.BitSize)
                        {
                            ea = m.IAddS(ea, memOp.Offset.ToInt32());
                        }
                        else
                        {
                            ea = m.IAdd(ea, memOp.Offset);
                        }
                    }
                    Expression load;
                    if (memOp.Deferred)
                        load = m.Mem(width, m.Mem32(ea));
                    else
                        load = m.Mem(width, ea);
                    if (memOp.AutoIncrement)
                    {
                        reg = binder.EnsureRegister(rbaseOp);
                        int inc = (memOp.Deferred) ? 4 : width.Size;
                        m.Assign(reg, m.IAdd(reg, inc));
                    }
                    return load;
                }
                else
                {
                    if (memOp.Base is Constant imm)
                    {
                        ea = imm;
                    }
                    else
                    {
                        ea = arch.MakeAddressFromConstant(memOp.Offset!, false);
                    }
                    Expression load;
                    if (memOp.Deferred)
                    {
                        load = m.Mem(width, m.Mem32(ea));
                    }
                    else
                    {
                        load = m.Mem(width, ea);
                    }
                    return load;
                }
            case Address addrOp:
                //$BUG: enabling the commented code causes huge regressions in the
                // unzip subject.
                /*if (addrOp.Width.BitSize > width.BitSize)
                {
                    var c = addrOp.Address.ToUInt32();
                    return Constant.Create(width, c);
                }
                else if (addrOp.Width.BitSize < width.BitSize)
                {
                    return m.Convert(addrOp.Address, addrOp.Address.DataType, width);
                }
                else*/
                {
                    return addrOp;
                }
            case IndexOperand indexOperand:
                ea = RewriteSrcOp(indexOperand.Base, PrimitiveType.Word32);
                if (ea is MemoryAccess imem)
                    ea = imem.EffectiveAddress;
                Expression idx = binder.EnsureRegister(indexOperand.Index);
                if (width.Size != 1)
                    idx = m.IMul(idx, Constant.Int32(width.Size));
                ea = m.IAdd(ea, idx);
                return m.Mem(width, ea);
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Identifier RewriteDstOp(int iOp, PrimitiveType width, Func<Expression, Expression> fn)
        {
            var op = this.instr.Operands[iOp];
            return RewriteDstOp(op, width, fn);
        }

        private Identifier RewriteDstOp(MachineOperand op, PrimitiveType width, Func<Expression, Expression> fn, Expression? index = null)
        {
            switch (op)
            {
            case RegisterStorage regOp:
                var reg = binder.EnsureRegister(regOp);
                if (width.BitSize < 32)
                {
                    var tmpLo = binder.CreateTemporary(width);
                    var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(32 - width.BitSize));
                    m.Assign(tmpLo, fn(m.Slice(reg, width)));
                    m.Assign(tmpHi, m.Slice(reg, tmpHi.DataType, width.BitSize));
                    m.Assign(reg, m.Seq(tmpHi, tmpLo));
                    return tmpLo;
                }
                else if (width.BitSize == 64)
                {
                    var rHi = arch.GetRegister(1 + (int)reg.Storage.Domain);
                    if (rHi is null)
                        return null!;
                    var regHi = binder.EnsureRegister(rHi);
                    reg = binder.EnsureSequence(width, regHi.Storage, reg.Storage);
                }
                else if (width.BitSize == 128)
                {
                    var regHi1 = binder.EnsureRegister(arch.GetRegister(1 + (int)reg.Storage.Domain)!);
                    var regHi2 = binder.EnsureRegister(arch.GetRegister(2 + (int)reg.Storage.Domain)!);
                    var regHi3 = binder.EnsureRegister(arch.GetRegister(3 + (int)reg.Storage.Domain)!);

                    var regLo = binder.EnsureSequence(PrimitiveType.Word64, regHi1.Storage, reg.Storage);
                    var regHi = binder.EnsureSequence(PrimitiveType.Word64, regHi3.Storage, regHi2.Storage);
                    reg = binder.EnsureSequence(width, regHi.Storage, regLo.Storage);
                }
                m.Assign(reg, fn(reg));
                return reg;
            case MemoryOperand memOp:
                Expression ea;
                Identifier? regEa = null;
                if (memOp.Base is RegisterStorage rbase)
                {
                    regEa = binder.EnsureRegister(rbase);
                    if (memOp.AutoDecrement)
                    {
                        m.Assign(regEa, m.ISub(regEa, width.Size));
                    }
                    ea = regEa;
                    if (memOp.Offset is not null)
                    {
                        if (memOp.Offset.DataType.BitSize < ea.DataType.BitSize)
                        {
                            ea = m.IAddS(ea, memOp.Offset.ToInt32());
                        }
                        else
                        {
                            ea = m.IAdd(ea, memOp.Offset);
                        }
                    }
                }
                else
                {
                    ea = arch.MakeAddressFromConstant(memOp.Offset!, false);
                }
                var tmp = binder.CreateTemporary(width);
                m.Assign(tmp, fn(m.Mem(width, ea)));
                Expression load;
                if (memOp.Deferred)
                    ea = m.Mem32(ea);
                if (index is not null)
                    ea = m.IAdd(ea, index);
                load = m.Mem(width, ea);
                m.Assign(load, tmp);

                if (regEa is not null && memOp.AutoIncrement)
                {
                    int inc = (memOp.Deferred) ? 4 : width.Size;
                    m.Assign(regEa, m.IAdd(regEa, inc));
                }
                return tmp;
            case IndexOperand indexOperand:
                Expression idx = binder.EnsureRegister(indexOperand.Index);
                if (width.Size != 1)
                    idx = m.IMul(idx, Constant.Int32(width.Size));
                return RewriteDstOp(indexOperand.Base, PrimitiveType.Word32, fn, idx);
            case Constant _:
            case Address _:
                return null!;
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        private Identifier FlagGroup(FlagGroupStorage flags)
        {
            return binder.EnsureFlagGroup(flags);
        }

        private bool AllFlags(Expression dst)
        {
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            var grf = FlagGroup(Registers.CVZN);
            m.Assign(grf, m.Cond(dst));
            return true;
        }

        private bool NZ0(Expression dst)
        {
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            var grf = FlagGroup(Registers.ZN);
            m.Assign(grf, m.Cond(dst));
            var c = FlagGroup(Registers.C);
            var v = FlagGroup(Registers.V);
            m.Assign(v, Constant.False());
            return true;
        }

        private bool NZ00(Expression dst)
        {
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            var grf = FlagGroup(Registers.ZN);
            m.Assign(grf, m.Cond(dst));
            var c = FlagGroup(Registers.C);
            var v = FlagGroup(Registers.V);
            m.Assign(c, Constant.False());
            m.Assign(v, Constant.False());
            return true;
        }

        private bool VZN(Expression dst)
        {
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            var grf = FlagGroup(Registers.VZN);
            m.Assign(grf, m.Cond(dst));
            return true;
        }

        private bool NZV0(Expression dst)
        {
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            var grf = FlagGroup(Registers.VZN);
            m.Assign(grf, m.Cond(dst));
            var c = FlagGroup(Registers.C);
            m.Assign(c, Constant.False());
            return true;
        }

        private void EmitInvalid()
        {
            rtlInstructions.Clear();
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private static readonly IntrinsicProcedure addp4 = new IntrinsicBuilder("vax_addp4", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure addp6 = new IntrinsicBuilder("vax_addp6", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure ashift = new IntrinsicBuilder("__ashift", false)
            .GenericTypes("TValue", "TShift")
            .Param("TValue")
            .Param("TShift")
            .Returns("TValue");
        private static readonly IntrinsicProcedure ashp = new IntrinsicBuilder("vax_ashp", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure atomic_fetch_add = IntrinsicBuilder.GenericBinary("atomic_fetch_add", true);

        private static readonly IntrinsicProcedure bpt = new IntrinsicBuilder("vax_bpt", true)
            .Void();

        private static readonly IntrinsicProcedure chme = new IntrinsicBuilder("vax_chme", true).Param(PrimitiveType.Word16).Void();
        private static readonly IntrinsicProcedure chms = new IntrinsicBuilder("vax_chms", true).Param(PrimitiveType.Word16).Void();
        private static readonly IntrinsicProcedure chmu = new IntrinsicBuilder("vax_chmu", true).Param(PrimitiveType.Word16).Void();



        private static readonly IntrinsicProcedure cmpp3 = new IntrinsicBuilder("vax_cmpp3", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure cmpp4 = new IntrinsicBuilder("vax_cmpp4", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure cvtps = new IntrinsicBuilder("__cvtps", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure cvtsp = new IntrinsicBuilder("__cvtsp", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Word16)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure divp = new IntrinsicBuilder("vax_divp", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure emodd = new IntrinsicBuilder("emodd", true)
            .Param(PrimitiveType.Real64)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Real64)
            .OutParam(PrimitiveType.Int32)
            .OutParam(PrimitiveType.Real64)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure emodf = new IntrinsicBuilder("emodf", true)
            .Param(PrimitiveType.Real32)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Real32)
            .OutParam(PrimitiveType.Int32)
            .OutParam(PrimitiveType.Real32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure emodg = new IntrinsicBuilder("emodg", true)
            .Param(PrimitiveType.Real64)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Real64)
            .OutParam(PrimitiveType.Int32)
            .OutParam(PrimitiveType.Real64)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure emodh = new IntrinsicBuilder("emodh", true)
            .Param(PrimitiveType.Real128)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Real128)
            .OutParam(PrimitiveType.Int32)
            .OutParam(PrimitiveType.Real128)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure ffc = new IntrinsicBuilder("__ffc", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Word32)
            .OutParam(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure ffs = new IntrinsicBuilder("__ffs", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Word32)
            .OutParam(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure insque = new IntrinsicBuilder("__insque", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure movp = new IntrinsicBuilder("__movp", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Ptr32)
            .Void();

        private static readonly IntrinsicProcedure mulp = new IntrinsicBuilder("vax_mulp", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);

        private static readonly IntrinsicProcedure poly = new IntrinsicBuilder("vax_poly", true)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns("T");

        private static readonly IntrinsicProcedure prober = new IntrinsicBuilder("__prober", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure round = IntrinsicBuilder.GenericUnary("round");

        private static readonly IntrinsicProcedure release_interlock = new IntrinsicBuilder("__release_interlock", true)
            .Void();

        private static readonly IntrinsicProcedure scanc = new IntrinsicBuilder("__scanc", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Byte)
            .OutParam(PrimitiveType.Word32)
            .OutParam(PrimitiveType.Word32)
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure set_interlock = new IntrinsicBuilder("__set_interlock", true)
            .Void();

        private static readonly IntrinsicProcedure subp4 = new IntrinsicBuilder("vax_subp4", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure subp6 = new IntrinsicBuilder("vax_subp6", true)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.Word16)
            .Param(PrimitiveType.Ptr32)
            .Returns(PrimitiveType.Byte);

    }
}
