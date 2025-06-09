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

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006

    public partial class zSeriesRewriter : IEnumerable<RtlInstructionCluster>
    {
        //$TODO: IBM defines a hexadecimal format; it's not IEEE 783
        private static readonly PrimitiveType ShortHexFloat = PrimitiveType.Real32;
        private static readonly PrimitiveType LongHexFloat = PrimitiveType.Real64;
        private static readonly PrimitiveType ExtendedHexFloat = PrimitiveType.Real128;
        private static readonly PrimitiveType Word31 = PrimitiveType.CreateWord(31);

        private readonly zSeriesArchitecture arch;
        private readonly zSeriesIntrinsics intrinsics;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<zSeriesInstruction> dasm;
        private readonly List<RtlInstruction> instrs;
        private readonly RtlEmitter m;
        private readonly ExpressionValueComparer cmp;
        private readonly int ptrSize;
        private zSeriesInstruction instr;
        private InstrClass iclass;

        public zSeriesRewriter(
            zSeriesArchitecture arch,
            zSeriesIntrinsics intrinsics,
            EndianImageReader rdr,
            ProcessorState state,
            IStorageBinder binder,
            IRewriterHost host)
        {
            this.arch = arch;
            this.intrinsics = intrinsics;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new zSeriesDisassembler(arch, rdr).GetEnumerator();
            this.cmp = new ExpressionValueComparer();
            this.instrs = new List<RtlInstruction>();
            this.m = new RtlEmitter(instrs);
            this.ptrSize = arch.PointerType.BitSize;
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    host.Warn(instr.Address, "zSeries instruction {0} not implemented yet.", instr);
                    EmitUnitTest();
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.a: RewriteA(PrimitiveType.Int32); break;
                case Mnemonic.ad: RewriteFAdd(LongHexFloat); break;
                case Mnemonic.adr: RewriteFAddReg(PrimitiveType.Real64); break;
                case Mnemonic.ae: RewriteFAdd(ShortHexFloat); break;
                case Mnemonic.aeb: RewriteFAdd(PrimitiveType.Real32); break;
                case Mnemonic.aer: RewriteFAddReg(ShortHexFloat); break;
                case Mnemonic.aghi: RewriteAhi2(PrimitiveType.Word64); break;
                case Mnemonic.aghik: RewriteAhi3(PrimitiveType.Word64); break;
                case Mnemonic.ahi: RewriteAhi2(PrimitiveType.Word32); break;
                case Mnemonic.ahik: RewriteAhi3(PrimitiveType.Word32); break;
                case Mnemonic.agr: RewriteAr(PrimitiveType.Int64); break;
                case Mnemonic.agsi: RewriteAsi(PrimitiveType.Int64); break;
                case Mnemonic.al: RewriteA(PrimitiveType.Word32); break;
                case Mnemonic.alcr: RewriteAdcSbcReg(m.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.alcgr: RewriteAdcSbcReg(m.IAdd, PrimitiveType.Word64); break;
                case Mnemonic.algfr: RewriteAlugfr(m.IAdd, PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Mnemonic.algr: RewriteAr(PrimitiveType.Word64); break;
                case Mnemonic.alr: RewriteAr(PrimitiveType.Word32); break;
                case Mnemonic.ar: RewriteAr(PrimitiveType.Int32); break;
                case Mnemonic.asi: RewriteAsi(PrimitiveType.Int32); break;
                case Mnemonic.aur: RewriteFAddReg(ShortHexFloat); break;
                case Mnemonic.awr: RewriteFAddReg(LongHexFloat); break;
                case Mnemonic.axr: RewriteFpuRegPair(m.FAdd, ExtendedHexFloat); break;
                case Mnemonic.b:   RewriteUnconditionalBranch(); break;
                case Mnemonic.bal: RewriteBranchAndLink(); break;
                case Mnemonic.balr: RewriteBranchAndLinkReg(); break;
                case Mnemonic.bas: RewriteBranchAndLink(); break;
                case Mnemonic.bct: RewriteBranchOnCount(PrimitiveType.Word32); break;
                case Mnemonic.bctr: RewriteBranchOnCount(PrimitiveType.Word32); break;
                case Mnemonic.bctgr: RewriteBranchOnCount(PrimitiveType.Word64); break;
                case Mnemonic.bh: RewriteBranchEa(ConditionCode.UGT); break;
                case Mnemonic.bnl: RewriteBranchEa(ConditionCode.GE); break;
                case Mnemonic.bl:  RewriteBranchEa(ConditionCode.LT); break;
                case Mnemonic.blh: RewriteBranchEa(ConditionCode.NE); break;    //$REVIEW where are these mnemonics defined?
                case Mnemonic.bne: RewriteBranchEa(ConditionCode.NE); break;
                case Mnemonic.bnh: RewriteBranchEa(ConditionCode.ULE); break;
                case Mnemonic.be:  RewriteBranchEa(ConditionCode.EQ); break;
                case Mnemonic.bnle: RewriteBranchEa(ConditionCode.GT); break;
                case Mnemonic.bhe: RewriteBranchEa(ConditionCode.UGE); break;
                case Mnemonic.bnlh: RewriteBranchEa(ConditionCode.NE); break;
                case Mnemonic.ble: RewriteBranchEa(ConditionCode.LE); break;
                case Mnemonic.bnhe: RewriteBranchEa(ConditionCode.ULT); break;
                case Mnemonic.bno: RewriteBranchEa(ConditionCode.NO); break;
                case Mnemonic.bo: RewriteBranchEa(ConditionCode.OV); break;

                case Mnemonic.basr: RewriteBasr(); break;
                case Mnemonic.bassm: RewriteBassm(); break;
                case Mnemonic.ber: RewriteBranch(ConditionCode.EQ); break;
                case Mnemonic.bler: RewriteBranch(ConditionCode.LE); break;
                case Mnemonic.bner: RewriteBranch(ConditionCode.NE); break;
                case Mnemonic.bprp: RewriteBprp(); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.brasl: RewriteBrasl(); break;
                case Mnemonic.brctg: RewriteBrctg(); break;
                case Mnemonic.brxh: Brx(m.Gt); break;
                case Mnemonic.brxle: Brx(m.Le); break;
                case Mnemonic.bsm: RewriteBsm(); break;
                case Mnemonic.bxh: Bx(m.Gt); break;
                case Mnemonic.bxle: Bx(m.Le); break;
                case Mnemonic.c: RewriteC(PrimitiveType.Int32); break;
                case Mnemonic.cd: RewriteCmpFloatMem(PrimitiveType.Real64); break;
                case Mnemonic.cdb: RewriteCmpFloatMem(PrimitiveType.Real64); break;
                case Mnemonic.cdr: RewriteCmpFloat(PrimitiveType.Real64); break;
                case Mnemonic.ceb: RewriteCmpFloatMem(PrimitiveType.Real32); break;
                case Mnemonic.cer: RewriteCmpFloat(PrimitiveType.Real32); break;
                case Mnemonic.cghi: RewriteCghi(); break;
                case Mnemonic.cgij: RewriteCij(PrimitiveType.Int64); break;
                case Mnemonic.cgr: RewriteCr(m.ISub, PrimitiveType.Int64); break;
                case Mnemonic.cgrj: RewriteCrj(PrimitiveType.Int64); break;
                case Mnemonic.ch: RewriteCmpH(PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.chi: RewriteChi(); break;
                case Mnemonic.cij: RewriteCij(PrimitiveType.Int32); break;
                case Mnemonic.cl: RewriteCl(PrimitiveType.Word32); break;
                case Mnemonic.clc: RewriteClc(); break;
                case Mnemonic.clcl: RewriteClcl(PrimitiveType.Word128); break;
                case Mnemonic.clcle: RewriteClcl(PrimitiveType.Word128); break;
                case Mnemonic.clg: RewriteCl(PrimitiveType.Word64); break;
                case Mnemonic.clgij: RewriteClij(PrimitiveType.Word64); break;
                case Mnemonic.clgr: RewriteCr(m.USub, PrimitiveType.Word64); break;
                case Mnemonic.clgrl: RewriteClrl(PrimitiveType.Word64); break;
                case Mnemonic.clgrj: RewriteClrj(PrimitiveType.Word64); break;
                case Mnemonic.clfi: RewriteClfi(PrimitiveType.UInt32); break;
                case Mnemonic.cli: RewriteCli(); break;
                case Mnemonic.clij: RewriteClij(PrimitiveType.Word32); break;
                case Mnemonic.cliy: RewriteCli(); break;
                case Mnemonic.clr: RewriteCr(m.USub, PrimitiveType.Word32); break;
                case Mnemonic.clrj: RewriteClrj(PrimitiveType.Word32); break;
                case Mnemonic.clrl: RewriteClrl(PrimitiveType.Word32); break;
                case Mnemonic.cr: RewriteCr(m.ISub, PrimitiveType.Int32); break;
                case Mnemonic.crj: RewriteCrj(PrimitiveType.Int32); break;
                case Mnemonic.cs: RewriteCs(PrimitiveType.Word32); break;
                case Mnemonic.csg: RewriteCs(PrimitiveType.Word64); break;
                case Mnemonic.cvb: RewriteCvb(PrimitiveType.Int32); break;
                case Mnemonic.cvd: RewriteCvd(PrimitiveType.Int32); break;
                case Mnemonic.d: RewriteD(); break;
                case Mnemonic.ddr: RewriteFDivR(PrimitiveType.Real64); break;
                case Mnemonic.der: RewriteFDivR(PrimitiveType.Real32); break;
                case Mnemonic.dlgr: RewriteDlr(PrimitiveType.UInt64); break;
                case Mnemonic.dlr: RewriteDlr(PrimitiveType.UInt32); break;
                case Mnemonic.dp: RewriteDp(); break;
                case Mnemonic.dr: RewriteDr(); break;
                case Mnemonic.dsgr: RewriteDsgr(PrimitiveType.Int64); break;
                case Mnemonic.ex: RewriteEx(); break;
                case Mnemonic.exrl: RewriteEx(); break;
                case Mnemonic.hdr: RewriteHalveR(PrimitiveType.Real64, Constant.Real64(2)); break;
                case Mnemonic.her: RewriteHalveR(PrimitiveType.Real32, Constant.Real32(2)); break;
                case Mnemonic.ic: RewriteIc(); break;
                case Mnemonic.j: RewriteJ(); break;
                case Mnemonic.je: RewriteJcc(ConditionCode.EQ); break;
                case Mnemonic.jg: RewriteJcc(ConditionCode.GT); break;
                case Mnemonic.jh: RewriteJcc(ConditionCode.UGT); break;
                case Mnemonic.jl: RewriteJcc(ConditionCode.LT); break;
                case Mnemonic.jle: RewriteJcc(ConditionCode.LE); break;
                case Mnemonic.jhe: RewriteJcc(ConditionCode.UGE); break;
                case Mnemonic.jne: RewriteJcc(ConditionCode.NE); break;
                case Mnemonic.jnh: RewriteJcc(ConditionCode.ULE); break;
                case Mnemonic.jnl: RewriteJcc(ConditionCode.GE); break;
                case Mnemonic.jo: RewriteJcc(ConditionCode.OV); break;
                case Mnemonic.la: RewriteLa(); break;
                case Mnemonic.larl: RewriteLarl(); break;
                case Mnemonic.l: RewriteL(PrimitiveType.Word32); break;
                case Mnemonic.laa: RewriteLaa(m.IAdd, PrimitiveType.Int32); break;
                case Mnemonic.lay: RewriteLay(); break;
                case Mnemonic.lbr: RewriteLr(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.lcdr: RewriteLcr(LongHexFloat, m.FNeg); break;
                case Mnemonic.lcer: RewriteLcr(ShortHexFloat, m.FNeg); break;
                case Mnemonic.lcr: RewriteLcr(PrimitiveType.Int32, m.Neg); break;
                case Mnemonic.lcgr: RewriteLcr(PrimitiveType.Int64, m.Neg); break;
                case Mnemonic.ld: RewriteL(PrimitiveType.Word64); break;
                case Mnemonic.ldeb: RewriteFConvert(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.ldxr: RewriteLdxr(); break;
                case Mnemonic.ldr: RewriteLreg(PrimitiveType.Word64); break;
                case Mnemonic.le: RewriteL(PrimitiveType.Real32); break;
                case Mnemonic.ler: RewriteLreg(PrimitiveType.Real32); break;
                case Mnemonic.ledr: RewriteFConvertReg(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.ldgr: RewriteLdgr(); break;
                case Mnemonic.lg: RewriteL(PrimitiveType.Word64); break;
                case Mnemonic.lgfrl: RewriteL(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.lh: RewriteL(PrimitiveType.Word16); break;
                case Mnemonic.lhrl: RewriteL(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.lgdr: RewriteLgdr(); break;
                case Mnemonic.lgf: RewriteLgf(); break;
                case Mnemonic.lgfr: RewriteLgfr(); break;
                case Mnemonic.lghi: RewriteLghi(); break;
                case Mnemonic.lgr: RewriteLreg(PrimitiveType.Word64); break;
                case Mnemonic.lgrl: RewriteLgrl(); break;
                case Mnemonic.lhi: RewriteLhi(); break;
                case Mnemonic.llcr: RewriteLr(PrimitiveType.Byte, PrimitiveType.Word32); break;
                case Mnemonic.llgcr: RewriteLr(PrimitiveType.Byte, PrimitiveType.Word64); break;
                case Mnemonic.llgfr: RewriteLr(PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Mnemonic.llgfrl: RewriteLl(PrimitiveType.Word32); break;
                case Mnemonic.llgtr: RewriteLlgt(); break;
                case Mnemonic.llhr: RewriteLlhr(); break;
                case Mnemonic.llhrl: RewriteLlhr(); break;
                case Mnemonic.llill: RewriteLli(PrimitiveType.Word16, 0); break;
                case Mnemonic.lmg: RewriteLmg(); break;
                case Mnemonic.lndr: RewriteFNegR(LongHexFloat); break;
                case Mnemonic.lner: RewriteFNegR(ShortHexFloat); break;
                case Mnemonic.lngr: RewriteLnr(PrimitiveType.Int64); break;
                case Mnemonic.lnr: RewriteLnr(PrimitiveType.Int32); break;
                case Mnemonic.locg: RewriteLoc(PrimitiveType.Word64, ConditionCode.ALWAYS); break;
                case Mnemonic.locgre: RewriteLocr(PrimitiveType.Word64, ConditionCode.EQ); break;
                case Mnemonic.locgrh: RewriteLocr(PrimitiveType.Word64, ConditionCode.UGT); break;
                case Mnemonic.locgrl: RewriteLocr(PrimitiveType.Word64, ConditionCode.LT); break;
                case Mnemonic.locgrle: RewriteLocr(PrimitiveType.Word64, ConditionCode.LE); break;
                case Mnemonic.locgrne: RewriteLocr(PrimitiveType.Word64, ConditionCode.NE); break;
                case Mnemonic.locgrnhe: RewriteLocr(PrimitiveType.Word64, ConditionCode.ULE); break;
                case Mnemonic.locgrnl: RewriteLocr(PrimitiveType.Word64, ConditionCode.GE); break;
                case Mnemonic.locgrnle: RewriteLocr(PrimitiveType.Word64, ConditionCode.GT); break;
                case Mnemonic.lpdr: RewriteUnary(intrinsics.fabs, LongHexFloat); break;
                case Mnemonic.lper: RewriteUnary(FpOps.FAbs32, ShortHexFloat); break;
                case Mnemonic.lpgr: RewriteUnary(CommonOps.Abs, PrimitiveType.Int64); break;
                case Mnemonic.lpr: RewriteUnary(CommonOps.Abs, PrimitiveType.Int32); break;
                case Mnemonic.lr: RewriteLr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Mnemonic.lra: RewriteLra(); break;
                case Mnemonic.lrl: RewriteL(PrimitiveType.Word32); break;
                case Mnemonic.lrvgr: RewriteLrv(PrimitiveType.Word64); break;
                case Mnemonic.lrvr: RewriteLrv(PrimitiveType.Word32); break;
                case Mnemonic.lt: RewriteLt(PrimitiveType.Word32); break;
                case Mnemonic.ltdr: RewriteLtdr(PrimitiveType.Real64, Constant.Real64(0.0)); break;
                case Mnemonic.lter: RewriteLtdr(PrimitiveType.Real32, Constant.Real32(0.0F)); break;
                case Mnemonic.ltg: RewriteLt(PrimitiveType.Word64); break;
                case Mnemonic.ltgr: RewriteLtr(PrimitiveType.Word64); break;
                case Mnemonic.ltr: RewriteLtr(PrimitiveType.Word32); break;
                case Mnemonic.m: RewriteM(PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.md: RewriteFMul(LongHexFloat, LongHexFloat); break;
                case Mnemonic.mdb: RewriteFMul(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.mde: RewriteFMul(ShortHexFloat, LongHexFloat); break;
                case Mnemonic.mdr: RewriteFMulReg(LongHexFloat, LongHexFloat); break;
                case Mnemonic.medr: RewriteFMulReg(ShortHexFloat, LongHexFloat); break;
                case Mnemonic.meeb: RewriteFMul(PrimitiveType.Real32, PrimitiveType.Real32); break;
                case Mnemonic.meer: RewriteFMulReg(ShortHexFloat, ShortHexFloat); break;
                case Mnemonic.mh: RewriteAluH(m.SMul, PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.mlgr: RewriteMr(m.UMul, PrimitiveType.UInt64, PrimitiveType.UInt128); break;
                case Mnemonic.mr: RewriteMr(m.SMul, PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.msgr: RewriteMulR(PrimitiveType.Int64); break;
                case Mnemonic.mvcle: RewriteMvcle(); break;
                case Mnemonic.mvhi: RewriteMvi(PrimitiveType.Word16); break;
                case Mnemonic.mvi: RewriteMvi(PrimitiveType.Byte); break;
                case Mnemonic.mvz: RewriteMvz(); break;
                case Mnemonic.mxd: RewriteFMul(LongHexFloat, ExtendedHexFloat); break;
                case Mnemonic.mxdr: RewriteFMulReg(LongHexFloat, ExtendedHexFloat); break;
                case Mnemonic.mxr: RewriteFpuRegPair(m.FMul, ExtendedHexFloat); break;
                case Mnemonic.nc: RewriteNc(); break;
                case Mnemonic.ngr: RewriteLogicR(PrimitiveType.Word64, m.And); break;
                case Mnemonic.n: RewriteLogic(PrimitiveType.Word32, m.And); break;
                case Mnemonic.ni: RewriteNi(); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.nopr: m.Nop(); break;
                case Mnemonic.nr: RewriteLogicR(PrimitiveType.Word32, m.And); break;
                case Mnemonic.o: RewriteLogic(PrimitiveType.Word32, m.Or); break;
                case Mnemonic.ogr: RewriteLogicR(PrimitiveType.Word64, m.Or); break;
                case Mnemonic.oi: RewriteOi(); break;
                case Mnemonic.or: RewriteLogicR(PrimitiveType.Word32, m.Or); break;
                case Mnemonic.risbg: RewriteRisbg(intrinsics.risbg); break;
                case Mnemonic.risbgn: RewriteRisbg(intrinsics.risbgn); break;
                case Mnemonic.rllg: RewriteShift3(PrimitiveType.Word64, Rol); break;
                case Mnemonic.s: RewriteS(PrimitiveType.Int32); break;
                case Mnemonic.sh: RewriteAluH(m.ISub, PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.sl: RewriteS(PrimitiveType.Word32); break;
                case Mnemonic.sd: RewriteFSub(LongHexFloat); break;
                case Mnemonic.sdb: RewriteFSub(PrimitiveType.Real64); break;
                case Mnemonic.sdr: RewriteFSubReg(LongHexFloat); break;
                case Mnemonic.se: RewriteFSub(ShortHexFloat); break;
                case Mnemonic.seb: RewriteFSub(PrimitiveType.Real32); break;
                case Mnemonic.ser: RewriteFSubReg(ShortHexFloat); break;
                case Mnemonic.sr: RewriteSub2(PrimitiveType.Int32); break;
                case Mnemonic.sgfr: RewriteAlugfr(m.ISub, PrimitiveType.Int32, PrimitiveType.Int64); break;
                case Mnemonic.sgr: RewriteSub2(PrimitiveType.Int64); break;
                case Mnemonic.sla: RewriteShift2(PrimitiveType.Int32, m.Shl); break;
                case Mnemonic.slgfr: RewriteAlugfr(m.USub, PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Mnemonic.slgr: RewriteSub2(PrimitiveType.Word64); break;
                case Mnemonic.slfi: RewriteSub2(PrimitiveType.Word32); break;
                case Mnemonic.sll: RewriteShift2(PrimitiveType.Word32, m.Shl); break;  //$TODO: CC's are handled unsigned.
                case Mnemonic.slr: RewriteSub2(PrimitiveType.Word32); break;
                case Mnemonic.slrk: RewriteAlu3(m.USub, PrimitiveType.Word32); break;
                case Mnemonic.sllg: RewriteShift3(PrimitiveType.Word64, m.Shl); break;
                case Mnemonic.sllk: RewriteShift3(PrimitiveType.Word32, m.Shl); break;
                case Mnemonic.sra: RewriteShift2(PrimitiveType.Int32, m.Sar); break;
                case Mnemonic.srag: RewriteShift3(PrimitiveType.Int64, m.Sar); break;
                case Mnemonic.srak: RewriteShift3(PrimitiveType.Int32, m.Sar); break;
                case Mnemonic.srl: RewriteShift2(PrimitiveType.Word32, m.Shr); break;
                case Mnemonic.srlk: RewriteShift3(PrimitiveType.Word32, m.Shr); break;
                case Mnemonic.srlg: RewriteShift3(PrimitiveType.Word64, m.Shr); break;
                case Mnemonic.srp: RewriteSrp(); break;
                case Mnemonic.st: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stc: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.stctl: RewriteStctl(PrimitiveType.Word32); break;
                case Mnemonic.std: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.ste: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stg: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.sth: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.strl: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stgrl: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stmg: RewriteStmg(); break;
                case Mnemonic.su: RewriteFSub(ShortHexFloat); break;
                case Mnemonic.sur: RewriteSur(ShortHexFloat); break;
                case Mnemonic.swr: RewriteSur(LongHexFloat); break;
                case Mnemonic.svc: RewriteSvc(); break;
                case Mnemonic.sxr: RewriteFpuRegPair(m.FSub, ExtendedHexFloat); break;
                case Mnemonic.ts: RewriteTs(); break;

                case Mnemonic.vavg: RewriteVectorInstruction3Elem(intrinsics.vavg, false); break;
                case Mnemonic.verim: RewriteVectorInstruction4Elem(intrinsics.verim, false); break;
                case Mnemonic.vfa: RewriteVectorInstruction3Elem(intrinsics.vfa); break;
                case Mnemonic.vfd: RewriteVectorInstruction3Elem(intrinsics.vfd); break;
                case Mnemonic.vfs: RewriteVectorInstruction3Elem(intrinsics.vfs); break;
                case Mnemonic.vll: RewriteVectorInstruction(intrinsics.vll); break;
                case Mnemonic.vmao: RewriteVectorInstruction4Elem(intrinsics.vmao, false); break;
                case Mnemonic.vmah: RewriteVectorInstruction4Elem(intrinsics.vmah, true); break;
                case Mnemonic.vmrh: RewriteVectorInstruction3Elem(intrinsics.vmrh); break;
                case Mnemonic.vmrl: RewriteVectorInstruction3Elem(intrinsics.vmrl); break;
                case Mnemonic.vmxl: RewriteVectorInstruction3Elem(intrinsics.vmx); break;
                case Mnemonic.vpk: RewriteVectorInstruction3Elem(intrinsics.vpk, false); break;
                case Mnemonic.vpkls: RewriteVectorInstruction3Elem(intrinsics.vpkls); break;
                case Mnemonic.vn: RewriteVectorInstruction(intrinsics.vn); break;
                case Mnemonic.vslb: RewriteVectorInstruction(intrinsics.vslb); break;
                case Mnemonic.vsel: RewriteVectorInstruction(intrinsics.vsel); break;
                case Mnemonic.vx: RewriteVectorInstruction(intrinsics.vx); break;

                case Mnemonic.x: RewriteXor2(PrimitiveType.Word32); break;
                case Mnemonic.xgr: RewriteLogicR(PrimitiveType.Word64, m.Xor); break;
                case Mnemonic.xr: RewriteLogicR(PrimitiveType.Word32, m.Xor); break;
                case Mnemonic.xc: RewriteXc(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                instrs.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void EmitUnitTest()
        {
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("zSeriesRw", dasm.Current, instr.Mnemonic.ToString(), rdr, "");
        }

        private Address Addr(int iop)
        {
            return (Address)instr.Operands[iop];
        }

        private Expression Assign(Expression dst, Expression src)
        {
            var excessBits = dst.DataType.BitSize - src.DataType.BitSize;
            if (excessBits > 0)
            {
                if (!(src is Identifier || src is Constant))
                {
                    var tmp = binder.CreateTemporary(src.DataType);
                    m.Assign(tmp, src);
                    src = tmp;
                }
                var seq = m.Dpb(dst, src, 0);
                m.Assign(dst, seq);
                return seq.Expressions[1];
            }
            else
            {
                m.Assign(dst, src);
                return dst;
            }
        }

        private Constant Const(int iop)
        {
            return (Constant)instr.Operands[iop];
        }

        private Expression EffectiveAddress(int iop)
        {
            switch (instr.Operands[iop])
            {
            case Address aOp:
                return aOp;
            case MemoryOperand mem:
                return EffectiveAddress(mem);
            default:
                throw new InvalidOperationException($"Operand type {instr.Operands[iop].GetType().Name} cannot be an effective address.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            if (mem.Base is null || mem.Base.Number == 0)
            {
                // Must be abs address.
                return Address.Create(arch.PointerType, (uint) mem.Offset);
            }
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Index is not null && mem.Index.Number > 0)
            {
                var idx = binder.EnsureRegister(mem.Index);
                ea = m.IAdd(ea, idx);
            }
            if (mem.Offset != 0)
            {
                var off = Constant.Int(mem.Base.DataType, mem.Offset);
                ea = m.IAdd(ea, off);
            }
            return ea;
        }

        private Expression Op(int iop, DataType dt)
        {
            switch (instr.Operands[iop])
            {
            case RegisterStorage reg:
                var r =  binder.EnsureRegister(reg);
                if (r.DataType.BitSize > dt.BitSize)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, m.Slice(r, dt, 0));
                    return tmp;
                }
                else
                {
                    return r;
                }
            case Constant imm:
                return imm;
            case MemoryOperand mem:
                return new MemoryAccess(EffectiveAddress(mem), dt);
            case Address addr:
                return addr;
            default:
                throw new NotImplementedException($"{instr.Operands[iop].GetType().Name}");
            }
        }

        private Identifier FReg(int iOp)
        {
            var freg = Registers.FpRegisters[((RegisterStorage) instr.Operands[iOp]).Number];
            return binder.EnsureRegister(freg);
        }

        private Constant Imm(int iop, DataType dt)
        {
            var c = (Constant)instr.Operands[iop];
            return Constant.Create(dt, c.ToUInt64());
        }

        private RegisterStorage NextGpReg(RegisterStorage reg)
        {
            var n = (reg.Number + 1) & 0xF;
            return Registers.GpRegisters[n];
        }

        private Identifier NextGpReg(int iop, PrimitiveType dt)
        {
            var reg = ((RegisterStorage) instr.Operands[iop]);
            var n = (reg.Number + 1) & 0xF;
            reg = Registers.GpRegisters[n];
            var id = binder.EnsureRegister(reg);
            if (reg.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(id, dt, 0));
                return tmp;
            }
            else
            {
                return id;
            }
        }

        private Address PcRel(int iOp)
        {
            var addr = (Address)instr.Operands[iOp];
            return addr;
        }

        private Identifier Reg(int iOp)
        {
            return binder.EnsureRegister((RegisterStorage)instr.Operands[iOp]);
        }

        private Expression Rel(int iOp, PrimitiveType dt)
        {
            return m.Mem(dt, (Address)instr.Operands[iOp]);
        }

        private Identifier Reg(int iOp, DataType dt)
        {
            var reg = binder.EnsureRegister((RegisterStorage) instr.Operands[iOp]);
            if (reg.DataType.BitSize > dt.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(reg, dt, 0));
                return tmp;
            }
            else
            {
                return reg;
            }
        }

        private Identifier? FpRegPair(int iop, PrimitiveType dt)
        {
            var reghi = (RegisterStorage) instr.Operands[iop];
            var iregLo = reghi.Number - Registers.FpRegisters[0].Number + 1;
            if (iregLo >= Registers.FpRegisters.Length)
                return null;
            var reglo = Registers.FpRegisters[iregLo];
            return binder.EnsureSequence(dt, reghi, reglo);
        }

        private Expression Rol(Expression left, Expression right)
        {
            return m.Fn(CommonOps.Rol, left, right);
        }

        private Identifier Seq(PrimitiveType dt, int iopHi, int iopLo)
        {
            var regHi = (RegisterStorage) instr.Operands[iopHi];
            var regLo = (RegisterStorage) instr.Operands[iopLo];
            return binder.EnsureSequence(dt, regHi, regLo);
        }
        private void SetCc(Expression e)
        {
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, e);
        }

        private Expression SignedCondition(int iop, Expression left, Expression right)
        {
            var mm = ((Constant) instr.Operands[iop]).ToInt32();
            return mm switch
            {
                2 => m.Gt(left, right),
                4 => m.Lt(left, right),
                6 => m.Ne(left, right),
                8 => m.Eq(left, right),
                0xA => m.Ge(left, right),
                0xC => m.Le(left, right),
                _ => Constant.False()
            };
        }

        private Expression UnsignedCondition(int iop, Expression left, Expression right)
        {
            var mm = ((Constant) instr.Operands[iop]).ToInt32();
            return mm switch
            {
                2 => m.Ugt(left, right),
                4 => m.Ult(left, right),
                6 => m.Ne(left, right),
                8 => m.Eq(left, right),
                0xA => m.Uge(left, right),
                0xC => m.Ule(left, right),
                _ => Constant.False()
            };
        }
    }
}