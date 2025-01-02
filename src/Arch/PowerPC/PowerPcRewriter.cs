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

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly PowerPcArchitecture arch;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly EndianImageReader rdr;
        private readonly IEnumerator<PowerPcInstruction> dasm;
        private PowerPcInstruction instr;
        private RtlEmitter m;
        private InstrClass iclass;
        private List<RtlInstruction> rtlInstructions;

        public PowerPcRewriter(PowerPcArchitecture arch, IEnumerable<PowerPcInstruction> instrs, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = instrs.GetEnumerator();
            this.rdr = null!;
            this.instr = null!;
            this.m = null!;
            this.rtlInstructions = null!;
        }

        public PowerPcRewriter(PowerPcArchitecture arch, EndianImageReader rdr, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
            this.instr = null!;
            this.m = null!;
            this.rtlInstructions = null!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addr = this.instr.Address;
                this.rtlInstructions = new List<RtlInstruction>();
                this.iclass = instr.InstructionClass;
                this.m = new RtlEmitter(rtlInstructions);
                switch (dasm.Current.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address, 
                        string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Mnemonic.illegal;
                case Mnemonic.nyi:
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addc: RewriteAddc(); break;
                case Mnemonic.addco: RewriteAddc(); break;
                case Mnemonic.adde: RewriteAdde(); break;
                case Mnemonic.addeo: RewriteAdde(); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.addic: RewriteAddic(); break;
                case Mnemonic.addis: RewriteAddis(); break;
                case Mnemonic.addme: RewriteAddme(); break;
                case Mnemonic.addmeo: RewriteAddme(); break;
                case Mnemonic.addpcis: RewriteAddpcis(); break;
                case Mnemonic.addze: RewriteAddze(); break;
                case Mnemonic.and: RewriteAnd(false); break;
                case Mnemonic.andc: RewriteAndc(); break;
                case Mnemonic.andi: RewriteAnd(false); break;
                case Mnemonic.andis: RewriteAndis(); break;
                case Mnemonic.b: RewriteB(); break;
                case Mnemonic.bc: RewriteBc(false); break;
                case Mnemonic.bcctr: RewriteBcctr(false); break;
                case Mnemonic.bcctrl: RewriteBcctr(true); break;
                case Mnemonic.bcds: RewriteBcds(); break;
                case Mnemonic.bcdtrunc: RewriteBcdtrunc(); break;
                case Mnemonic.bcdus: RewriteBcdus(); break;
                case Mnemonic.bctrl: RewriteBcctr(true); break;
                case Mnemonic.bcdadd: RewriteBcdadd(); break;
                case Mnemonic.bdnz: RewriteCtrBranch(false, false, m.Ne, false); break;
                case Mnemonic.bdnzf: RewriteCtrBranch(false, false, m.Ne, false); break;
                case Mnemonic.bdnzfl: RewriteCtrBranch(true, false, m.Ne, false); break;
                case Mnemonic.bdnzl: RewriteCtrBranch(true, false, m.Ne, false); break;
                case Mnemonic.bdnzt: RewriteCtrBranch(false, false, m.Ne, true); break;
                case Mnemonic.bdnztl: RewriteCtrBranch(true, false, m.Ne, true); break;
                case Mnemonic.bdz: RewriteCtrBranch(false, false, m.Eq, false); break;
                case Mnemonic.bdzf: RewriteCtrBranch(false, false, m.Eq, false); break;
                case Mnemonic.bdzfl: RewriteCtrBranch(true, false, m.Eq, false); break;
                case Mnemonic.bdzt: RewriteCtrBranch(false, false, m.Eq, true); break;
                case Mnemonic.bdztl: RewriteCtrBranch(true, false, m.Eq, true); break;
                case Mnemonic.bdzl: RewriteCtrBranch(true, false, m.Eq, false); break;
                case Mnemonic.beq: RewriteBranch(false, false,ConditionCode.EQ); break;
                case Mnemonic.beql: RewriteBranch(true, false, ConditionCode.EQ); break;
                case Mnemonic.beqlr: RewriteBranch(false, true, ConditionCode.EQ); break;
                case Mnemonic.beqlrl: RewriteBranch(true, true, ConditionCode.EQ); break;
                case Mnemonic.bge: RewriteBranch(false, false,ConditionCode.GE); break;
                case Mnemonic.bgel: RewriteBranch(true, false,ConditionCode.GE); break;
                case Mnemonic.bgelr: RewriteBranch(false, true,ConditionCode.GE); break;
                case Mnemonic.bgt: RewriteBranch(false, false,ConditionCode.GT); break;
                case Mnemonic.bgtl: RewriteBranch(true, false,ConditionCode.GT); break;
                case Mnemonic.bgtlr: RewriteBranch(false, true,ConditionCode.GT); break;
                case Mnemonic.bl: RewriteBl(); break;
                case Mnemonic.blr: RewriteBlr(); break;
                case Mnemonic.bltlr: RewriteBranch(false, true,ConditionCode.LT); break;
                case Mnemonic.ble: RewriteBranch(false, false, ConditionCode.LE); break;
                case Mnemonic.blel: RewriteBranch(true, false, ConditionCode.LE); break;
                case Mnemonic.blelr: RewriteBranch(false, true, ConditionCode.LE); break;
                case Mnemonic.blelrl: RewriteBranch(true, true, ConditionCode.LE); break;
                case Mnemonic.blt: RewriteBranch(false, false, ConditionCode.LT); break;
                case Mnemonic.bltl: RewriteBranch(true, false, ConditionCode.LT); break;
                case Mnemonic.bne: RewriteBranch(false, false, ConditionCode.NE); break;
                case Mnemonic.bnel: RewriteBranch(true, false, ConditionCode.NE); break;
                case Mnemonic.bnelr: RewriteBranch(false, true, ConditionCode.NE); break;
                case Mnemonic.bns: RewriteBranch(false, false, ConditionCode.NO); break;
                case Mnemonic.bnsl: RewriteBranch(true, false, ConditionCode.NO); break;
                case Mnemonic.bso: RewriteBranch(false, false, ConditionCode.OV); break;
                case Mnemonic.bsol: RewriteBranch(true, false, ConditionCode.OV); break;
                case Mnemonic.bsolr: RewriteBranch(false, true, ConditionCode.OV); break;
                case Mnemonic.cmp: RewriteCmp(); break;
                case Mnemonic.cmpi: RewriteCmpi(); break;
                case Mnemonic.cmpl: RewriteCmpl(); break;
                case Mnemonic.cmpli: RewriteCmpli(); break;
                case Mnemonic.cmplw: RewriteCmplw(); break;
                case Mnemonic.cmplwi: RewriteCmplwi(); break;
                case Mnemonic.cmpwi: RewriteCmpwi(); break;
                case Mnemonic.cntlzd: RewriteCntlz(PrimitiveType.Word64); break;
                case Mnemonic.cntlzw: RewriteCntlz(PrimitiveType.Word32); break;
                case Mnemonic.crandc: RewriteCrLogical(crandc); break;
                case Mnemonic.creqv: RewriteCrLogical(creqv); break;
                case Mnemonic.cror:  RewriteCrLogical(cror); break;
                case Mnemonic.crorc: RewriteCrLogical(crorc); break;
                case Mnemonic.crnand: RewriteCrLogical(crnand); break;
                case Mnemonic.crnor: RewriteCrLogical(crnor); break;
                case Mnemonic.crxor: RewriteCrLogical(crxor); break;
                case Mnemonic.dcbf: RewriteDcbf(); break;
                case Mnemonic.dcbi: RewriteDcbi(); break;
                case Mnemonic.dcbst: RewriteDcbst(); break;
                case Mnemonic.dcbt: RewriteDcbt(); break;
                case Mnemonic.dcbtst: RewriteDcbtst(); break;
                case Mnemonic.dcbz: RewriteDcbz(); break;
                case Mnemonic.divd: RewriteDivd(m.SDiv); break;
                case Mnemonic.divdu: RewriteDivd(m.UDiv); break;
                case Mnemonic.divw: RewriteDivw(); break;
                case Mnemonic.divwu: RewriteDivwu(); break;
                case Mnemonic.eieio: RewriteEieio(); break;
                case Mnemonic.evmhesmfaaw: RewriteVectorPairOp(evmhesmfaaw, PrimitiveType.Word32); break;
                case Mnemonic.evmhessfaaw: RewriteVectorPairOp(evmhessfaaw, PrimitiveType.Word32); break;
                case Mnemonic.eqv: RewriteXor(true); break;
                case Mnemonic.extsb: RewriteExts(PrimitiveType.SByte); break;
                case Mnemonic.extsh: RewriteExts(PrimitiveType.Int16); break;
                case Mnemonic.extsw: RewriteExts(PrimitiveType.Int32); break;
                case Mnemonic.fabs: RewriteFabs(); break;
                case Mnemonic.fadd: RewriteFadd(); break;
                case Mnemonic.fadds: RewriteFadd(); break;
                case Mnemonic.fcfid: RewriteFcfid(); break;
                case Mnemonic.fctid: RewriteFctid(); break;
                case Mnemonic.fctidz: RewriteFctidz(); break;
                case Mnemonic.fctiw: RewriteFctiw(); break;
                case Mnemonic.fctiwz: RewriteFctiwz(); break;
                case Mnemonic.fcmpo: RewriteFcmpo(); break;
                case Mnemonic.fcmpu: RewriteFcmpu(); break;
                case Mnemonic.fdiv: RewriteFdiv(); break;
                case Mnemonic.fdivs: RewriteFdiv(); break;
                case Mnemonic.fmr: RewriteFmr(); break;
                case Mnemonic.fmadd: RewriteFmadd(PrimitiveType.Real64, m.FAdd, false); break;
                case Mnemonic.fmadds: RewriteFmadd(PrimitiveType.Real32, m.FAdd, false); break;
                case Mnemonic.fmsub: RewriteFmadd(PrimitiveType.Real64, m.FSub, false); break;
                case Mnemonic.fmsubs: RewriteFmadd(PrimitiveType.Real32, m.FSub, false); break;
                case Mnemonic.fnabs: RewriteFnabs(); break;
                case Mnemonic.fnmadd: RewriteFmadd(PrimitiveType.Real64, m.FAdd, true); break;
                case Mnemonic.fnmadds: RewriteFmadd(PrimitiveType.Real32, m.FAdd, true); break;
                case Mnemonic.fnmsub: RewriteFmadd(PrimitiveType.Real64, m.FSub, true); break;
                case Mnemonic.fnmsubs: RewriteFmadd(PrimitiveType.Real32, m.FSub, true); break;
                case Mnemonic.fmul: RewriteFmul(); break;
                case Mnemonic.fmuls: RewriteFmul(); break;
                case Mnemonic.fneg: RewriteFneg(); break;
                case Mnemonic.fres: RewriteFres(); break;
                case Mnemonic.frsp: RewriteFrsp(); break;
                case Mnemonic.frsqrte: RewriteFrsqrte(); break;
                case Mnemonic.fsel: RewriteFsel(); break;
                case Mnemonic.fsqrt: RewriteFsqrt(); break;
                case Mnemonic.fsqrts: RewriteFsqrts(); break;
                case Mnemonic.fsub: RewriteFsub(); break;
                case Mnemonic.fsubs: RewriteFsub(); break;
                case Mnemonic.icbi: RewriteIcbi(); break;
                case Mnemonic.isync: RewriteIsync(); break;
                case Mnemonic.lbz: RewriteLz(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzx: RewriteLzx(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzu: RewriteLzu(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzux: RewriteLzux(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.ld: RewriteLz(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.ldarx: RewriteLarx(PrimitiveType.Word64); break;
                case Mnemonic.ldu: RewriteLzu(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.ldux: RewriteLzux(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.ldx: RewriteLzx(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.lfd: RewriteLfd(); break;
                case Mnemonic.lfdp: RewriteLfdp(); break;
                case Mnemonic.lfdpx: RewriteLfdpx(); break;
                case Mnemonic.lfdu: RewriteLzu(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.lfdux: RewriteLzux(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.lfdx: RewriteLzx(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.lfiwax: RewriteLfiwax(); break;
                case Mnemonic.lfs: RewriteLfs(); break;
                case Mnemonic.lfsu: RewriteLzu(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.lfsux: RewriteLzux(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.lfsx: RewriteLzx(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.lha: RewriteLa(PrimitiveType.Int16, arch.SignedWord); break;
                case Mnemonic.lhax: RewriteLax(PrimitiveType.Int16, arch.SignedWord); break;
                case Mnemonic.lhau: RewriteLau(PrimitiveType.Int16, arch.SignedWord); break;
                case Mnemonic.lhaux: RewriteLaux(PrimitiveType.Int16, arch.SignedWord); break;
                case Mnemonic.lhbrx: RewriteLhbrx(); break;
                case Mnemonic.lhz: RewriteLz(PrimitiveType.Word16, arch.WordWidth); break;
                case Mnemonic.lhzu: RewriteLzu(PrimitiveType.Word16, arch.WordWidth); break;
                case Mnemonic.lhzux: RewriteLzux(PrimitiveType.Word16, arch.WordWidth); break;
                case Mnemonic.lhzx: RewriteLzx(PrimitiveType.Word16, arch.WordWidth); break;
                case Mnemonic.lmw: RewriteLmw(); break;
                case Mnemonic.lq: RewriteLq(); break;
                case Mnemonic.lswi: RewriteLswi(); break;
                case Mnemonic.lswx: RewriteLswx(); break;
                case Mnemonic.lvewx:
                case Mnemonic.lvewx128: RewriteLvewx(PrimitiveType.Word32); break;
                case Mnemonic.lvlx:
                case Mnemonic.lvlx128: RewriteLvlx(); break;
                case Mnemonic.lvrx128: RewriteLvrx(); break;
                case Mnemonic.lvsl: RewriteLvsX(lvsl); break;
                case Mnemonic.lvsr: RewriteLvsX(lvsr); break;
                case Mnemonic.lvx:
                case Mnemonic.lvxl:
                case Mnemonic.lvx128: RewriteLzx(PrimitiveType.Word128, PrimitiveType.Word128); break;
                case Mnemonic.lwa: RewriteLwa(); break;
                case Mnemonic.lwarx: RewriteLarx(PrimitiveType.Word32); break;
                case Mnemonic.lwax: RewriteLax(PrimitiveType.Int32, arch.SignedWord); break;
                case Mnemonic.lwbrx: RewriteLwbrx(); break;
                case Mnemonic.lwz: RewriteLz(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzu: RewriteLzu(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzux: RewriteLzux(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzx: RewriteLzx(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.maddhd: RewriteMulAcc(m.SMul, PrimitiveType.Int64, PrimitiveType.Int128, 64); break;
                case Mnemonic.maddhdu: RewriteMulAcc(m.UMul, PrimitiveType.UInt64, PrimitiveType.UInt128, 64); break;
                case Mnemonic.maddld: RewriteMulAcc(m.SMul, PrimitiveType.Int64, PrimitiveType.Int128, 0); break;
                case Mnemonic.mcrf: RewriteMcrf(); break;
                case Mnemonic.mfcr: RewriteMfcr(); break;
                case Mnemonic.mfctr: RewriteMfctr(); break;
                case Mnemonic.mftb: RewriteMftb(); break;
                case Mnemonic.mffs: RewriteMffs(); break;
                case Mnemonic.mflr: RewriteMflr(); break;
                case Mnemonic.mfmsr: RewriteMfmsr(); break;
                case Mnemonic.mfocrf: RewriteMfocrf(); break;
                case Mnemonic.mfspr: RewriteMfspr(); break;
                case Mnemonic.mtcrf: RewriteMtcrf(); break;
                case Mnemonic.mtctr: RewriteMtctr(); break;
                case Mnemonic.mtfsb1: RewriteMtfsb1(); break;
                case Mnemonic.mtfsf: RewriteMtfsf(); break;
                case Mnemonic.mtmsr: RewriteMtmsr(PrimitiveType.Word32); break;
                case Mnemonic.mtmsrd: RewriteMtmsr(PrimitiveType.Word64); break;
                case Mnemonic.mtspr: RewriteMtspr(); break;
                case Mnemonic.mtlr: RewriteMtlr(); break;
                case Mnemonic.mtvsrws: RewriteMtvsrws(); break;
                case Mnemonic.mulhw: RewriteMulhw(); break;
                case Mnemonic.mulhwu: RewriteMulhwu(); break;
                case Mnemonic.mulhhwu: RewriteMulhhwu(); break;
                case Mnemonic.mulli: RewriteMull(); break;
                case Mnemonic.mulld: RewriteMull(); break;
                case Mnemonic.mullw: RewriteMull(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.nand: RewriteAnd(true); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.nor: RewriteOr(true); break;
                case Mnemonic.or: RewriteOr(false); break;
                case Mnemonic.orc: RewriteOrc(); break;
                case Mnemonic.ori: RewriteOr(false); break;
                case Mnemonic.oris: RewriteOris(); break;
                case Mnemonic.ps_abs: RewritePairedInstruction_Src1(ps_abs); break;
                case Mnemonic.ps_add: RewritePairedInstruction_Src2(ps_add); break;
                case Mnemonic.ps_cmpo0: Rewrite_ps_cmpo(ps_cmpo); break;
                case Mnemonic.ps_div: RewritePairedInstruction_Src2(ps_div); break;
                case Mnemonic.psq_l: Rewrite_psq_l(false); break;
                case Mnemonic.psq_lu: Rewrite_psq_l(true); break;
                case Mnemonic.psq_lx: Rewrite_psq_l(false); break;
                case Mnemonic.psq_lux: Rewrite_psq_l(true); break;
                case Mnemonic.ps_madd: RewritePairedInstruction_Src3(ps_madd); break;
                case Mnemonic.ps_madds0: RewritePairedInstruction_Src3(ps_madds0); break;
                case Mnemonic.ps_madds1: RewritePairedInstruction_Src3(ps_madds1); break;
                case Mnemonic.ps_merge00: RewritePairedInstruction_Src2(ps_merge00); break;
                case Mnemonic.ps_merge01: RewritePairedInstruction_Src2(ps_merge01); break;
                case Mnemonic.ps_merge10: RewritePairedInstruction_Src2(ps_merge10); break;
                case Mnemonic.ps_merge11: RewritePairedInstruction_Src2(ps_merge11); break;
                case Mnemonic.ps_mr: Rewrite_ps_mr(); break;
                case Mnemonic.ps_mul: RewritePairedInstruction_Src2(ps_mul); break;
                case Mnemonic.ps_muls0: RewritePairedInstruction_Src2(ps_muls0); break;
                case Mnemonic.ps_muls1: RewritePairedInstruction_Src2(ps_muls1); break;
                case Mnemonic.ps_nmadd: RewritePairedInstruction_Src3(ps_nmadd); break;
                case Mnemonic.ps_nmsub: RewritePairedInstruction_Src3(ps_nmsub); break;
                case Mnemonic.ps_nabs: RewritePairedInstruction_Src1(ps_nabs); break;
                case Mnemonic.ps_neg: RewritePairedInstruction_Src1(ps_neg); break;
                case Mnemonic.ps_res: RewritePairedInstruction_Src1(ps_res); break;
                case Mnemonic.ps_rsqrte: RewritePairedInstruction_Src1(ps_rsqrte); break;
                case Mnemonic.ps_sel: RewritePairedInstruction_Src3(ps_sel); break;
                case Mnemonic.ps_sub: RewritePairedInstruction_Src2(ps_sub); break;
                case Mnemonic.ps_sum0: RewritePairedInstruction_Src3(ps_sum0); break;
                case Mnemonic.psq_st: Rewrite_psq_st(false); break;
                case Mnemonic.psq_stu: Rewrite_psq_st(true); break;
                case Mnemonic.psq_stx: Rewrite_psq_st(false); break;
                case Mnemonic.psq_stux: Rewrite_psq_st(true); break;
                case Mnemonic.rfi: RewriteRfi(); break;
                case Mnemonic.rfid: RewriteRfi(); break;
                case Mnemonic.rldcl: RewriteRldcl(); break;
                case Mnemonic.rldcr: RewriteRldcr(); break;
                case Mnemonic.rldic: RewriteRldic(); break;
                case Mnemonic.rldicl: RewriteRldicl(); break;
                case Mnemonic.rldicr: RewriteRldicr(); break;
                case Mnemonic.rldimi: RewriteRldimi(); break;
                case Mnemonic.rlwinm: RewriteRlwinm(); break;
                case Mnemonic.rlwimi: RewriteRlwimi(); break;
                case Mnemonic.rlwnm: RewriteRlwnm(); break;
                case Mnemonic.sc: RewriteSc(); break;
                case Mnemonic.sld: RewriteSl(PrimitiveType.Word64); break;
                case Mnemonic.slw: RewriteSl(PrimitiveType.Word32); break;
                case Mnemonic.srad: RewriteSra(); break;
                case Mnemonic.sradi: RewriteSra(); break;
                case Mnemonic.sraw: RewriteSra(); break;
                case Mnemonic.srawi: RewriteSra(); break;
                case Mnemonic.srd: RewriteSrw(); break;
                case Mnemonic.srw: RewriteSrw(); break;
                case Mnemonic.stb: RewriteSt(PrimitiveType.Byte); break;
                case Mnemonic.stbcx: RewriteStcx(PrimitiveType.Byte); break;
                case Mnemonic.stbu: RewriteStu(PrimitiveType.Byte); break;
                case Mnemonic.stbux: RewriteStux(PrimitiveType.Byte); break;
                case Mnemonic.stbx: RewriteStx(PrimitiveType.Byte); break;
                case Mnemonic.std: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stdcx: RewriteStcx(PrimitiveType.Word64); break;
                case Mnemonic.stdu: RewriteStu(PrimitiveType.Word64); break;
                case Mnemonic.stdx: RewriteStx(PrimitiveType.Word64); break;
                case Mnemonic.stfd: RewriteSt(PrimitiveType.Real64); break;
                case Mnemonic.stfdp: RewriteStfdp(); break;
                case Mnemonic.stfdpx: RewriteStfdpx(); break;
                case Mnemonic.stfdu: RewriteStu(PrimitiveType.Real64); break;
                case Mnemonic.stfdux: RewriteStux(PrimitiveType.Real64); break;
                case Mnemonic.stfdx: RewriteStx(PrimitiveType.Real64); break;
                case Mnemonic.stfiwx: RewriteStx(PrimitiveType.Int32); break;
                case Mnemonic.stfs: RewriteSt(PrimitiveType.Real32); break;
                case Mnemonic.stfsu: RewriteStu(PrimitiveType.Real32); break;
                case Mnemonic.stfsx: RewriteStx(PrimitiveType.Real32); break;
                case Mnemonic.sth: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.sthbrx: RewriteStbrx(PrimitiveType.Word16); break;
                case Mnemonic.sthu: RewriteStu(PrimitiveType.Word16); break;
                case Mnemonic.sthx: RewriteStx(PrimitiveType.Word16); break;
                case Mnemonic.stmw: RewriteStmw(); break;
                case Mnemonic.stq: RewriteStq(); break;
                case Mnemonic.stswi: RewriteStswi(); break;
                case Mnemonic.stvebx: RewriteStvex(PrimitiveType.Byte); break;
                case Mnemonic.stvehx: RewriteStvex(PrimitiveType.Word16); break;
                case Mnemonic.stvewx:
                case Mnemonic.stvewx128: RewriteStvex(PrimitiveType.Word32); break;
                case Mnemonic.stvx:
                case Mnemonic.stvxl:
                case Mnemonic.stvx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stvlx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stvrx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stw: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stwbrx: RewriteStbrx(PrimitiveType.Word32); break;
                case Mnemonic.stwcx: RewriteStcx(PrimitiveType.Word32); break;
                case Mnemonic.stwcix: RewriteStwcix(PrimitiveType.Word32); break;
                case Mnemonic.stwu: RewriteStu(PrimitiveType.Word32); break;
                case Mnemonic.stwux: RewriteStux(PrimitiveType.Word32); break;
                case Mnemonic.stwx: RewriteStx(PrimitiveType.Word32); break;
                case Mnemonic.subf: RewriteSubf(); break;
                case Mnemonic.subfc: RewriteSubfc(); break;
                case Mnemonic.subfco: RewriteSubfc(); break;
                case Mnemonic.subfe: RewriteSubfe(); break;
                case Mnemonic.subfeo: RewriteSubfe(); break;
                case Mnemonic.subfic: RewriteSubfic(); break;
                case Mnemonic.subfze: RewriteSubfze(); break;
                case Mnemonic.subfzeo: RewriteSubfze(); break;
                case Mnemonic.sync: RewriteSync(); break;
                case Mnemonic.td: RewriteTrap(PrimitiveType.Word64); break;
                case Mnemonic.tdi: RewriteTrap(PrimitiveType.Word64); break;
                case Mnemonic.tlbie: RewriteTlbie(); break;
                case Mnemonic.tlbsync: RewriteTlbsync(); break;
                case Mnemonic.tw: RewriteTrap(PrimitiveType.Word32); break;
                case Mnemonic.twi: RewriteTrap(PrimitiveType.Word32); break;
                case Mnemonic.vabsduw: RewriteVectorBinOp(vabsduw, PrimitiveType.UInt32); break;
                case Mnemonic.vaddecuq: RewriteVaddecuq(); break;
                case Mnemonic.vaddeuqm: RewriteTernaryOp(vaddeuqm); break;
                case Mnemonic.vaddfp:
                case Mnemonic.vaddfp128: RewriteVectorBinOp(Simd.FAdd, PrimitiveType.Real32); break;
                case Mnemonic.vaddsbs: RewriteVectorBinOp(vadds, PrimitiveType.Int8); break;
                case Mnemonic.vaddshs: RewriteVectorBinOp(vadds, PrimitiveType.Int16); break;
                case Mnemonic.vaddsws: RewriteVectorBinOp(vadds, PrimitiveType.Int32); break;
                case Mnemonic.vaddubm: RewriteVectorBinOp(vaddm, PrimitiveType.UInt8); break;
                case Mnemonic.vaddubs: RewriteVectorBinOp(vadds, PrimitiveType.UInt8); break;
                case Mnemonic.vadduhm: RewriteVectorBinOp(vaddm, PrimitiveType.UInt8); break;
                case Mnemonic.vadduwm: RewriteVectorBinOp(vaddm, PrimitiveType.UInt32); break;
                case Mnemonic.vadduws: RewriteVectorBinOp(vadds, PrimitiveType.UInt32); break;
                case Mnemonic.vadduqm: RewriteAdd(); break;
                case Mnemonic.vand:
                case Mnemonic.vand128: RewriteAnd(false); break;
                case Mnemonic.vandc:
                case Mnemonic.vandc128: RewriteAndc(); break;
                case Mnemonic.vavgsb: RewriteVectorBinOp(vavg, PrimitiveType.Int8); break;
                case Mnemonic.vavgsh: RewriteVectorBinOp(vavg, PrimitiveType.Int16); break;
                case Mnemonic.vavgsw: RewriteVectorBinOp(vavg, PrimitiveType.Int32); break;
                case Mnemonic.vavgub: RewriteVectorBinOp(vavg, PrimitiveType.UInt8); break;
                case Mnemonic.vavguh: RewriteVectorBinOp(vavg, PrimitiveType.UInt16); break;
                case Mnemonic.vavguw: RewriteVectorBinOp(vavg, PrimitiveType.UInt32); break;
                case Mnemonic.vbpermd: RewriteVbperm(PrimitiveType.UInt64); break;
                case Mnemonic.vcfsx: RewriteVctfixed(vcfp, PrimitiveType.Int32); break;
                case Mnemonic.vcfux: RewriteVctfixed(vcfp, PrimitiveType.UInt32); break;
                case Mnemonic.vcfpsxws128: RewriteVctfixed(vcfps, PrimitiveType.Int32); break;
                case Mnemonic.vcfpuxws128: RewriteVctfixed(vcfps, PrimitiveType.UInt32); break;
                case Mnemonic.vcmpbfp:
                case Mnemonic.vcmpbfp128: RewriteVcmp(vcmpbfp, PrimitiveType.Real32); break;
                case Mnemonic.vcmpeqfp:
                case Mnemonic.vcmpeqfp128: RewriteVcmp(vcmpeqfp, PrimitiveType.Real32); break;
                case Mnemonic.vcmpequh: RewriteVcmp(vcmpeq, PrimitiveType.Word16); break;
                case Mnemonic.vcmpgefp:
                case Mnemonic.vcmpgefp128: RewriteVcmp(vcmpgefp, PrimitiveType.Real32); break;
                case Mnemonic.vcmpgtfp:
                case Mnemonic.vcmpgtfp128: RewriteVcmp(vcmpgtfp, PrimitiveType.Real32); break;
                case Mnemonic.vcmpgtsb: RewriteVcmp(vcmpgt, PrimitiveType.Int8); break;
                case Mnemonic.vcmpgtsh: RewriteVcmp(vcmpgt, PrimitiveType.Int16); break;
                case Mnemonic.vcmpgtsw: RewriteVcmp(vcmpgt, PrimitiveType.Int32); break;
                case Mnemonic.vcmpgtub: RewriteVcmp(vcmpgt, PrimitiveType.UInt8); break;
                case Mnemonic.vcmpgtuh: RewriteVcmp(vcmpgt, PrimitiveType.UInt16); break;
                case Mnemonic.vcmpgtuw: RewriteVcmp(vcmpgt, PrimitiveType.UInt32); break;
                case Mnemonic.vcmpequb: RewriteVcmp(vcmpeq, PrimitiveType.UInt8); break;
                case Mnemonic.vcmpequd: RewriteVcmp(vcmpeq, PrimitiveType.UInt64); break;
                case Mnemonic.vcmpequw:
                case Mnemonic.vcmpequw128: RewriteVcmp(vcmpeq, PrimitiveType.UInt32); break;
                case Mnemonic.vcmpnew: RewriteVcmp(vcmpne, PrimitiveType.UInt32); break;
                case Mnemonic.vcmpnezh: RewriteVcmp(vcmpnez, PrimitiveType.Word16); break;
                case Mnemonic.vcsxwfp128: RewriteVcsxwfp(vcsxwfp); break;
                case Mnemonic.vcuxwfp128: RewriteVcsxwfp(vcuxwfp); break;
                case Mnemonic.vctsxs: RewriteVctfixed(vcfps, PrimitiveType.Int32); break;
                case Mnemonic.vctuxs: RewriteVctfixed(vcfps, PrimitiveType.UInt32); break;
                case Mnemonic.vexptefp:
                case Mnemonic.vexptefp128: RewriteVectorUnary(vexptefp, PrimitiveType.Real32); break;
                case Mnemonic.vextractd: RewriteVextract(vextract, PrimitiveType.UInt64); break;
                case Mnemonic.vextractub: RewriteVextract(vextract, PrimitiveType.Byte); break;
                case Mnemonic.vextractuh: RewriteVextract(vextract, PrimitiveType.UInt16); break;
                case Mnemonic.vextractuw: RewriteVextract(vextract, PrimitiveType.UInt32); break;
                case Mnemonic.vextuwrx: RewriteBinOp(vextrx); break;
                case Mnemonic.vgbbd: RewriteVectorUnary(vgbbd, PrimitiveType.UInt32); break;
                case Mnemonic.vlogefp:
                case Mnemonic.vlogefp128: RewriteVectorUnary(vlogefp, PrimitiveType.Real32); break;
                case Mnemonic.vmaddfp:
                case Mnemonic.vmaddfp128: RewriteVmaddfp(); break;
                case Mnemonic.vmaddcfp128: RewriteVectorBinOp(vmaddcfp, PrimitiveType.Real32); break;
                case Mnemonic.vmaxfp: RewriteVectorBinOp(vmaxfp, PrimitiveType.Real32); break;
                case Mnemonic.vmaxfp128: RewriteVectorBinOp(vmaxfp, PrimitiveType.Real32); break;
                case Mnemonic.vminfp128: RewriteVectorBinOp(vminfp, PrimitiveType.Real32); break;
                case Mnemonic.vmaxsb: RewriteVectorBinOp(vmax, PrimitiveType.Int8); break;
                case Mnemonic.vmaxsh: RewriteVectorBinOp(vmax, PrimitiveType.Int16); break;
                case Mnemonic.vmaxsw: RewriteVectorBinOp(vmax, PrimitiveType.Int32); break;
                case Mnemonic.vmaxub: RewriteVectorBinOp(vmax, PrimitiveType.UInt8); break;
                case Mnemonic.vmaxuh: RewriteVectorBinOp(vmax, PrimitiveType.UInt16); break;
                case Mnemonic.vmhaddshs: RewriteVectorTernaryOp(vmhadds, PrimitiveType.Int16); break;
                case Mnemonic.vmhraddshs: RewriteVectorTernaryOp(vmhradds, PrimitiveType.Int16); break;
                case Mnemonic.vminfp: RewriteVectorBinOp(vminfp, PrimitiveType.Real32); break;
                case Mnemonic.vminsh: RewriteVectorBinOp(vmin, PrimitiveType.Int16); break;
                case Mnemonic.vminuw: RewriteVectorBinOp(vmin, PrimitiveType.UInt32); break;
                case Mnemonic.vmladduhm: RewriteVectorBinOp(vmladduhm, PrimitiveType.UInt16); break;
                case Mnemonic.vmrghb: RewriteVectorBinOp(vmrgh, PrimitiveType.Byte); break;
                case Mnemonic.vmrghh: RewriteVectorBinOp(vmrgh, PrimitiveType.Word16); break;
                case Mnemonic.vmrghw:
                case Mnemonic.vmrghw128: RewriteVectorBinOp(vmrgh, PrimitiveType.Word32); break;
                case Mnemonic.vmrglb: RewriteVectorBinOp(vmrgl, PrimitiveType.Byte); break;
                case Mnemonic.vmrglh: RewriteVectorBinOp(vmrgl, PrimitiveType.Word16); ; break;
                case Mnemonic.vmrglw:
                case Mnemonic.vmrglw128: RewriteVectorBinOp(vmrgl, PrimitiveType.Word32); break;
                case Mnemonic.vmsummbm: RewriteVmsumm(vmsummm, PrimitiveType.Int8); break;
                case Mnemonic.vmsumshm: RewriteVectorTernaryOp(vmsumshm, PrimitiveType.Int16); break;
                case Mnemonic.vmsumshs: RewriteVmsumm(vmsums, PrimitiveType.Int16); break;
                case Mnemonic.vmsumubm: RewriteVmsumm(vmsumm, PrimitiveType.UInt8); break;
                case Mnemonic.vmsumuhm: RewriteVmsumm(vmsumm, PrimitiveType.UInt16); break;
                case Mnemonic.vmsumuhs: RewriteVmsumm(vmsums, PrimitiveType.UInt16); break;
                case Mnemonic.vmsub3fp128: RewriteVectorBinOp(vmsub3fp, PrimitiveType.Real32); break;
                case Mnemonic.vmsub4fp128: RewriteVectorBinOp(vmsub4fp, PrimitiveType.Real32); break;  //$REVIEW: is it correct?
                case Mnemonic.vmul10euq: RewriteBinOp(vmul10euq); break;
                case Mnemonic.vmulfp128: RewriteVectorBinOp(Simd.FMul, PrimitiveType.Real32); break;         //$REVIEW: is it correct?
                case Mnemonic.vmulesh: RewriteVmuloe(vmule, PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.vmulosh: RewriteVmuloe(vmulo, PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.vmuluwm: RewriteVectorBinOp(vmulm, PrimitiveType.UInt32); break;
                case Mnemonic.vnmsubfp:
                case Mnemonic.vnmsubfp128: RewriteVnmsubfp(); break;
                case Mnemonic.vnor: RewriteOr(true); break;
                case Mnemonic.vor:
                case Mnemonic.vor128: RewriteVor(); break;
                case Mnemonic.vorc: RewriteOrc();break;
                case Mnemonic.vperm:
                case Mnemonic.vperm128: RewriteVectorTernaryOp(vperm, PrimitiveType.Byte); break;
                case Mnemonic.vpermr: RewriteVectorTernaryOp(vpermr, PrimitiveType.Byte); break;
                case Mnemonic.vpermxor: RewriteVectorTernaryOp(vpermxor, PrimitiveType.Byte); break;
                case Mnemonic.vpkd3d128: RewriterVpkD3d(); break;
                case Mnemonic.vpksdss: RewriterVpks(PrimitiveType.Int64, PrimitiveType.Int32); break;
                case Mnemonic.vpksdus: RewriterVpks(PrimitiveType.Int64, PrimitiveType.UInt32); break;
                case Mnemonic.vpkshss: RewriterVpks(PrimitiveType.Int16, PrimitiveType.Int8); break;
                case Mnemonic.vpkshus: RewriterVpks(PrimitiveType.Int16, PrimitiveType.UInt8); break;
                case Mnemonic.vpkswss: RewriterVpks(PrimitiveType.Int32, PrimitiveType.Int16); break;
                case Mnemonic.vpkswus: RewriterVpks(PrimitiveType.Int32, PrimitiveType.UInt16); break;
                case Mnemonic.vpkuhus: RewriterVpks(PrimitiveType.UInt16, PrimitiveType.UInt8); break;
                case Mnemonic.vpmsumb: RewriteVpmsum(PrimitiveType.Byte, PrimitiveType.UInt16); break;
                case Mnemonic.vpmsumd: RewriteVpmsum(PrimitiveType.Word64, PrimitiveType.Word128); break;
                case Mnemonic.vpopcntb: RewriteVectorUnary(vpopcnt, PrimitiveType.Byte); break;
                case Mnemonic.vpopcntd: RewriteVectorUnary(vpopcnt, PrimitiveType.Word64); break;
                case Mnemonic.vpopcnth: RewriteVectorUnary(vpopcnt, PrimitiveType.Word16); break;
                case Mnemonic.vpopcntw: RewriteVectorUnary(vpopcnt, PrimitiveType.Word32); break;
                case Mnemonic.vrefp:
                case Mnemonic.vrefp128: RewriteVrefp(); break;
                case Mnemonic.vrfim:
                case Mnemonic.vrfim128: RewriteVectorUnary(vrfim, PrimitiveType.Real32); break;
                case Mnemonic.vrfin:
                case Mnemonic.vrfin128: RewriteVectorUnary(vrfin, PrimitiveType.Real32); break;
                case Mnemonic.vrfip128: RewriteVectorUnary(vrfip, PrimitiveType.Real32); break;
                case Mnemonic.vrfiz:
                case Mnemonic.vrfiz128: RewriteVectorUnary(vrfiz, PrimitiveType.Real32); break;
                case Mnemonic.vrlh: RewriteVsx(vrl, PrimitiveType.Word16); break;
                case Mnemonic.vrlimi128: RewriteVrlimi(); break;
                case Mnemonic.vrsqrtefp: 
                case Mnemonic.vrsqrtefp128: RewriteVrsqrtefp(); break;
                case Mnemonic.vsbox: RewriteVsbox(); break;
                case Mnemonic.vsel:
                case Mnemonic.vsel128: RewriteVsel(); break;
                case Mnemonic.vsldoi:
                case Mnemonic.vsldoi128: RewriteVsldoi(); break;
                case Mnemonic.vslb:
                case Mnemonic.vslb128: RewriteVsx(vsl, PrimitiveType.Byte); break;
                case Mnemonic.vslh: RewriteVsx(vsl, PrimitiveType.Word16); break;
                case Mnemonic.vslw:
                case Mnemonic.vslw128: RewriteVsx(vsl, PrimitiveType.Word32); break;
                case Mnemonic.vsr: RewriteBinOp(vsr.MakeInstance(PrimitiveType.Word128)); break;
                case Mnemonic.vsrab: RewriteVsx(vsra, PrimitiveType.Int8); break;
                case Mnemonic.vsrah: RewriteVsx(vsra, PrimitiveType.Int16); break;
                case Mnemonic.vsraw:
                case Mnemonic.vsraw128: RewriteVsx(vsra, PrimitiveType.Int32); break;
                case Mnemonic.vsrb: RewriteVsx(vsr, PrimitiveType.Byte); break;
                case Mnemonic.vsrh: RewriteVsx(vsr, PrimitiveType.Word16); break;
                case Mnemonic.vsrd: RewriteVsx(vsr, PrimitiveType.Word64); break;
                case Mnemonic.vsrw:
                case Mnemonic.vsrw128: RewriteVsx(vsr, PrimitiveType.Word32); break;
                case Mnemonic.vspltb: RewriteVsplt(PrimitiveType.Byte); break;
                case Mnemonic.vsplth: RewriteVsplt(PrimitiveType.Word16); break;
                case Mnemonic.vspltisb: RewriteVsplti(PrimitiveType.Int8); break;
                case Mnemonic.vspltish: RewriteVsplti(PrimitiveType.Int16); break;
                case Mnemonic.vspltisw:
                case Mnemonic.vspltisw128: RewriteVsplti(PrimitiveType.Int32); break;
                case Mnemonic.vspltw:
                case Mnemonic.vspltw128: RewriteVsplt(PrimitiveType.Word32); break;
                case Mnemonic.vsubcuw: RewriteVectorBinOp(vsubc, PrimitiveType.UInt32); break;
                case Mnemonic.vsubecuq: RewriteTernaryOp(vsubec.MakeInstance(PrimitiveType.Word128)); break;
                case Mnemonic.vsubeuqm: RewriteTernaryOp(vsubeuqm); break;
                case Mnemonic.vsubfp:
                case Mnemonic.vsubfp128: RewriteVsubfp(); break;
                case Mnemonic.vsubsbs: RewriteVectorBinOp(vsubs, PrimitiveType.Int8); break;
                case Mnemonic.vsubshs: RewriteVectorBinOp(vsubs, PrimitiveType.Int16); break;
                case Mnemonic.vsubsws: RewriteVectorBinOp(vsubs, PrimitiveType.Int32); break;
                case Mnemonic.vsububm: RewriteVectorBinOp(vsubm, PrimitiveType.UInt8); break;
                case Mnemonic.vsububs: RewriteVectorBinOp(vsubs, PrimitiveType.UInt8);  break;
                case Mnemonic.vsubuhm: RewriteVectorBinOp(vsubm, PrimitiveType.UInt16); break;
                case Mnemonic.vsubuhs: RewriteVectorBinOp(vsubs, PrimitiveType.UInt16); break;
                case Mnemonic.vsubuwm: RewriteVectorBinOp(vsubm, PrimitiveType.UInt32); break;
                case Mnemonic.vsubuws: RewriteVectorBinOp(vsubs, PrimitiveType.UInt32); break;
                case Mnemonic.vsum4shs: RewriteVsum4(vsum4s, PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.vupkd3d128: RewriteVupkd3d(); break;
                case Mnemonic.vupkhpx: RewriteUnaryOp(vupkhpx); break;
                case Mnemonic.vupklpx: RewriteUnaryOp(vupklpx); break;
                case Mnemonic.vupkhsb:
                case Mnemonic.vupkhsb128: RewriteVupk(vupkh, PrimitiveType.Int8, PrimitiveType.Int16); break;
                case Mnemonic.vupkhsh: RewriteVupk(vupkh, PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.vupklsb:
                case Mnemonic.vupklsb128: RewriteVupk(vupkl, PrimitiveType.Int8, PrimitiveType.Int16); break;
                case Mnemonic.vupklsh: RewriteVupk(vupkl, PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.vxor:
                case Mnemonic.vxor128: RewriteXor(false); break;
                case Mnemonic.xor: RewriteXor(false); break;
                case Mnemonic.xori: RewriteXor(false); break;
                case Mnemonic.xoris: RewriteXoris(); break;
                case Mnemonic.xsaddsp: RewriteXsaddsp(); break;
                case Mnemonic.xvadddp: RewriteVectorBinOp(Simd.FAdd, PrimitiveType.Real64); break;
                }
                yield return m.MakeCluster(addr, 4, iclass);
            }
        }

        private Expression Shift16(MachineOperand machineOperand)
        {
            var imm = (ImmediateOperand)machineOperand;
            return Constant.Create(arch.WordWidth, imm.Value.ToInt32() << 16);
        }


        private Expression ImmOperand(int iop) => ImmOperand(instr.Operands[iop]);

        private Expression ImmOperand(MachineOperand op)
        {
            return ((ImmediateOperand) op).Value;
        }

        private void RewriteBinOp(IntrinsicProcedure intrinsic)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(intrinsic, vra, vrb));
        }

        private void RewriteTernaryOp(IntrinsicProcedure intrinsic)
        {
            var vra = RewriteOperand(1);
            var vrb = RewriteOperand(2);
            var vrc = RewriteOperand(3);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(intrinsic, vra, vrb, vrc));
        }

        private void RewriteUnaryOp(IntrinsicProcedure intrinsic)
        {
            var vra = RewriteOperand(1);
            var vrt = RewriteOperand(0);
            m.Assign(
                vrt,
                m.Fn(intrinsic, vra));
        }

        private Expression RewriteOperand(int iop, bool maybe0 = false) =>
            RewriteOperand(instr.Operands[iop], maybe0);

        private Expression RewriteOperand(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterStorage rOp:
                if (maybe0 && rOp.Number == 0)
                    return Constant.Zero(rOp.DataType);
                if (arch.IsCcField(rOp))
                {
                    return binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(rOp)!);
                }
                else
                {
                    return binder.EnsureRegister(rOp);
                }
            case ImmediateOperand iOp:
                // Extend the immediate value to word size. If this is not wanted,
                // convert the operand manually or use RewriteSignedOperand
                return Constant.Create(arch.WordWidth, iOp.Value.ToUInt64());
            case AddressOperand aOp:
                return aOp.Address;
            default:
                throw new NotImplementedException($"RewriteOperand:{op} ({op.GetType()}");
            }
        }

        private Expression RewriteSignedOperand(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterStorage rOp:
                if (maybe0 && rOp.Number == 0)
                    return Constant.Zero(rOp.DataType);
                if (arch.IsCcField(rOp))
                {
                    return binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(rOp)!);
                }
                else
                {
                    return binder.EnsureRegister(rOp);
                }
            case ImmediateOperand iOp:
                // Extend the immediate value to word size. If this is not wanted,
                // convert the operand manually or use RewriteSignedOperand
                return Constant.Word(arch.WordWidth.BitSize, iOp.Value.ToInt64());
            case AddressOperand aOp:
                return aOp.Address;
            default:
                throw new NotImplementedException($"RewriteOperand:{op} ({op.GetType()}");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest()
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("PPCRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression EffectiveAddress(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            var reg = binder.EnsureRegister(mop.BaseRegister);
            var offset = mop.IntOffset();
            return emitter.IAddS(reg, offset);
        }

        private Expression EffectiveAddress_r0(int iOp, int extraOffset = 0)
        {
            var mop = (MemoryOperand) instr.Operands[iOp];
            var offset = mop.IntOffset();
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32(offset + extraOffset);
            }
            else
            {
                var reg = binder.EnsureRegister(mop.BaseRegister);
                offset = offset + extraOffset;
                if (offset != 0)
                    return m.IAddS(reg, offset);
                else
                    return reg;
            }
        }

        private Expression EffectiveAddress_r0(MachineOperand operand)
        {
            var mop = (MemoryOperand) operand;
            var offset = mop.IntOffset();
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32(offset);
            }
            else
            {
                var reg = binder.EnsureRegister(mop.BaseRegister);
                if (offset != 0)
                    return m.IAddS(reg, offset);
                else
                    return reg;
            }
        }

        private Expression EffectiveAddress_r0(MachineOperand op1, MachineOperand op2)
        {
            var e1 = RewriteOperand(op1, true);
            var e2 = RewriteOperand(op2);
            if (e1.IsZero)
                return e2;
            else
                return m.IAdd(e1, e2);
        }

        private Expression UpdatedRegister(Expression effectiveAddress)
        {
            var bin = (BinaryExpression) effectiveAddress;
            return bin.Left;
        }

        static PowerPcRewriter()
        {
        }


        private static readonly ArrayType fpPair = new ArrayType(PrimitiveType.Real32, 2);


        private static readonly IntrinsicProcedure bcdadd = IntrinsicBuilder.Binary("__bcd_add", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure bcds = IntrinsicBuilder.Binary("__bcd_shift", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure bcdus = IntrinsicBuilder.Binary("__bcd_unsigned_shift", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure bcdtrunc = IntrinsicBuilder.Binary("__bcd_truncate", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure crandc = new IntrinsicBuilder("__crandc", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure creqv = new IntrinsicBuilder("__creqv", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure crnand = new IntrinsicBuilder("__crnand", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure crnor = new IntrinsicBuilder("__crnor", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure cror = new IntrinsicBuilder("__cror", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure crorc = new IntrinsicBuilder("__crorc", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure crxor = new IntrinsicBuilder("__crxor", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Void();

        private static readonly IntrinsicProcedure dcbf = new IntrinsicBuilder("__dcbf", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure dcbi = new IntrinsicBuilder("__dcbi", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure dcbst = new IntrinsicBuilder("__dcbst", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure dcbtst = new IntrinsicBuilder("__dcbtst", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure dcbz = new IntrinsicBuilder("__dcbz", true)
            .GenericTypes("T")
            .Param("T")
            .Void();

        private static readonly IntrinsicProcedure eieio = new IntrinsicBuilder("__eieio", true)
            .Void();
        private static readonly IntrinsicProcedure evmhesmfaaw = IntrinsicBuilder.GenericBinary("__evmhesmfaaw");
        private static readonly IntrinsicProcedure evmhessfaaw = IntrinsicBuilder.GenericBinary("__evmhessfaaw");

        private static readonly IntrinsicProcedure fctiw = IntrinsicBuilder.Pure("__fctiw")
            .Param(PrimitiveType.Real64)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure fre = IntrinsicBuilder.GenericUnary("__fp_reciprocal_estimate");
        private static readonly IntrinsicProcedure frsqrte = IntrinsicBuilder.Unary("__frsqrte", PrimitiveType.Real64);

        private static readonly IntrinsicProcedure icbi = new IntrinsicBuilder("__icbi", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure isync = new IntrinsicBuilder("__isync", true)
            .Void();

        private static readonly IntrinsicProcedure larx = new IntrinsicBuilder("__larx", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure lve = new IntrinsicBuilder("__load_vector_element", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word128);
        private static readonly IntrinsicProcedure lvlx = new IntrinsicBuilder("__lvlx", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure lvrx = new IntrinsicBuilder("__lvrx", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure lvsl = new IntrinsicBuilder("__lvsl", true)
            .Param(PrimitiveType.Word64)
            .Returns(PrimitiveType.Word128);
        private static readonly IntrinsicProcedure lvsr = new IntrinsicBuilder("__lvsr", true)
            .Param(PrimitiveType.Word64)
            .Returns(PrimitiveType.Word128);
        private static readonly IntrinsicProcedure lwsx = new IntrinsicBuilder("__lwsx", true)
            .GenericTypes("TPtr", "TXer")
            .Param("TPtr")
            .Param("TXer")
            .Void();

        private static readonly IntrinsicProcedure mfmsr = new IntrinsicBuilder("__read_msr", true)
            .GenericTypes("T")
            .Returns("T");
        private static readonly IntrinsicProcedure mfocrf = new IntrinsicBuilder("__mfocrf", true)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure mfspr = new IntrinsicBuilder("__read_spr", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word32)
            .Returns("T");
        private static readonly IntrinsicProcedure mftb = new IntrinsicBuilder("__mftb", true)
            .GenericTypes("T")
            .Returns("T");
        private static readonly IntrinsicProcedure mtcrf = new IntrinsicBuilder("__mtcrf", true)
            .GenericTypes("T")
            .Params("T", "T")
            .Void();
        private static readonly IntrinsicProcedure mtfsb1 = new IntrinsicBuilder("__mtfsb1", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure mtfsf = new IntrinsicBuilder("__mtfsf", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Byte)
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure mtmsr = new IntrinsicBuilder("__write_msr", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure mtspr = new IntrinsicBuilder("__write_spr", true)
            .GenericTypes("T")
            .Param(PrimitiveType.Word32)
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure mtvsrws = IntrinsicBuilder.GenericUnary("__mtvsrws");

        private static readonly IntrinsicProcedure pack_quantized = IntrinsicBuilder.Pure("__pack_quantized")
            .Param(fpPair)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(fpPair);

        private static readonly IntrinsicProcedure ps_abs =     IntrinsicBuilder.GenericUnary("__ps_abs");
        private static readonly IntrinsicProcedure ps_add =     IntrinsicBuilder.GenericBinary("__ps_add");
        private static readonly IntrinsicProcedure ps_cmpo =    IntrinsicBuilder.Pure("__ps_cmpo")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc", "TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure ps_div =     IntrinsicBuilder.GenericBinary("__ps_div");
        private static readonly IntrinsicProcedure ps_madd =    IntrinsicBuilder.GenericTernary("__ps_madd");
        private static readonly IntrinsicProcedure ps_madds0 =  IntrinsicBuilder.GenericTernary("__ps_madds0");
        private static readonly IntrinsicProcedure ps_madds1 =  IntrinsicBuilder.GenericTernary("__ps_madds1");
        private static readonly IntrinsicProcedure ps_merge00 = IntrinsicBuilder.GenericBinary("__ps_merge00");
        private static readonly IntrinsicProcedure ps_merge01 = IntrinsicBuilder.GenericBinary("__ps_merge01");
        private static readonly IntrinsicProcedure ps_merge10 = IntrinsicBuilder.GenericBinary("__ps_merge10");
        private static readonly IntrinsicProcedure ps_merge11 = IntrinsicBuilder.GenericBinary("__ps_merge11");
        private static readonly IntrinsicProcedure ps_mul =     IntrinsicBuilder.GenericBinary("__ps_mul");
        private static readonly IntrinsicProcedure ps_muls0 =   IntrinsicBuilder.GenericBinary("__ps_muls0");
        private static readonly IntrinsicProcedure ps_muls1 =   IntrinsicBuilder.GenericBinary("__ps_muls1");
        private static readonly IntrinsicProcedure ps_nabs =    IntrinsicBuilder.GenericUnary("__ps_nabs");
        private static readonly IntrinsicProcedure ps_neg =     IntrinsicBuilder.GenericUnary("__ps_neg");
        private static readonly IntrinsicProcedure ps_nmadd =   IntrinsicBuilder.GenericBinary("__ps_nmadd");
        private static readonly IntrinsicProcedure ps_nmsub =   IntrinsicBuilder.GenericTernary("__ps_nmsub");
        private static readonly IntrinsicProcedure ps_res =     IntrinsicBuilder.GenericUnary("__ps_res");
        private static readonly IntrinsicProcedure ps_rsqrte =  IntrinsicBuilder.GenericUnary("__ps_rsqrte");
        private static readonly IntrinsicProcedure ps_sel =     IntrinsicBuilder.GenericTernary("__ps_sel");
        private static readonly IntrinsicProcedure ps_sub =     IntrinsicBuilder.GenericBinary("__ps_sub");
        private static readonly IntrinsicProcedure ps_sum0 = IntrinsicBuilder.GenericTernary("__ps_sum0");

        private static readonly IntrinsicProcedure rlwimi = IntrinsicBuilder.Pure("__rlwimi")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure rlwinm = IntrinsicBuilder.Pure("__rlwinm")
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns("T");

        private static readonly IntrinsicProcedure stcx = new IntrinsicBuilder("__store_conditional", true)
            .GenericTypes("TPtr", "TElem")
            .Param("TPtr")
            .Param("TElem")
            .Returns(PrimitiveType.Byte);
        private static readonly IntrinsicProcedure stve = new IntrinsicBuilder("__store_vector_element", true)
            .GenericTypes("TPtr", "TElem")
            .Param("TPtr")
            .Param("TElem")
            .Void();
        private static readonly IntrinsicProcedure stwcix = new IntrinsicBuilder("__stwcix", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure swap16 = IntrinsicBuilder.Unary("__swap16", PrimitiveType.Word16);
        private static readonly IntrinsicProcedure sync_intrinsic = new IntrinsicBuilder("__sync", true)
            .Void();

        private static readonly IntrinsicProcedure tlbie = new IntrinsicBuilder("__tlbie", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure tlbsync = new IntrinsicBuilder("__tlbsync", true)
            .Void();

        private static readonly IntrinsicProcedure trap_intrinsic = new IntrinsicBuilder("__trap", true)
            .Void();

        private static readonly IntrinsicProcedure unpack_quantized = IntrinsicBuilder.Pure("__unpack_quantized")
            .Param(fpPair)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(fpPair);

        private static readonly IntrinsicProcedure vabsduw = new IntrinsicBuilder("__vector_abs_difference", false)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Returns("T");
        private static readonly IntrinsicProcedure vaddecuq = new IntrinsicBuilder("__vector_add_extended_write_carry", false)
            .GenericTypes("T")
            .Param("T")
            .Param("T")
            .Param("T")
            .Returns("T");
        private static readonly IntrinsicProcedure vadduwm = IntrinsicBuilder.GenericBinary("__vadduwm");
        private static readonly IntrinsicProcedure vavg = IntrinsicBuilder.GenericBinary("__vector_average");
        private static readonly IntrinsicProcedure vaddeuqm = IntrinsicBuilder.Ternary("__vector_add_extended_modulo", PrimitiveType.UInt128);
        private static readonly IntrinsicProcedure vaddm = IntrinsicBuilder.GenericBinary("__vector_add_modulo");
        private static readonly IntrinsicProcedure vadds = IntrinsicBuilder.GenericBinary("__vector_add_saturate");
        private static readonly IntrinsicProcedure vbperm = IntrinsicBuilder.GenericBinary("__vector_bit_permute");
        private static readonly IntrinsicProcedure vcfp = new IntrinsicBuilder("__vector_cvt_fixedpt_saturate", false)
            .GenericTypes("TFrom", "TTo")
            .Param("TFrom")
            .Param(PrimitiveType.Byte)
            .Returns("TTo");
        private static readonly IntrinsicProcedure vcfps = new IntrinsicBuilder("__vector_cvt_fixedpt", false)
            .GenericTypes("TFrom", "TTo")
            .Param("TFrom")
            .Param(PrimitiveType.Byte)
            .Returns("TTo");

        private static readonly IntrinsicProcedure vcmpbfp = IntrinsicBuilder.GenericBinary("__vector_fp_cmp_bounds");
        private static readonly IntrinsicProcedure vcmpeq = IntrinsicBuilder.GenericBinary("__vector_cmpeq");
        private static readonly IntrinsicProcedure vcmpeqfp = IntrinsicBuilder.GenericBinary("__vector_fp_cmpeq");
        private static readonly IntrinsicProcedure vcmpgefp = IntrinsicBuilder.GenericBinary("__vector_fp_cmpge");
        private static readonly IntrinsicProcedure vcmpgt = IntrinsicBuilder.GenericBinary("__vector_cmpgt");
        private static readonly IntrinsicProcedure vcmpgtfp = IntrinsicBuilder.GenericBinary("__vector_fp_cmpgt");
        private static readonly IntrinsicProcedure vcmpne = IntrinsicBuilder.GenericBinary("__vector_cmpne");
        private static readonly IntrinsicProcedure vcmpnez = IntrinsicBuilder.GenericBinary("__vector_cmpne_or_0");
        private static readonly IntrinsicProcedure vcsxwfp = IntrinsicBuilder.GenericBinary("__vcsxwfp");
        private static readonly IntrinsicProcedure vcuxwfp = IntrinsicBuilder.GenericBinary("__vcuxwfp");
        private static readonly IntrinsicProcedure vexptefp = IntrinsicBuilder.GenericUnary("__vector_2_exp_estimate");
        private static readonly IntrinsicProcedure vextract = new IntrinsicBuilder("__vector_extract", false)
            .GenericTypes("T")
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure vextrx = new IntrinsicBuilder("__vector_extract_right_indexed", false)
            .Param(PrimitiveType.Word64)
            .Param(PrimitiveType.Word128)
            .Returns(PrimitiveType.Word64);

        private static readonly IntrinsicProcedure vgbbd = IntrinsicBuilder.GenericUnary("__vector_gather_bits_bytes");
        private static readonly IntrinsicProcedure vlogefp = IntrinsicBuilder.GenericUnary("__vector_log2_estimate");
        private static readonly IntrinsicProcedure vmaddfp = IntrinsicBuilder.GenericTernary("__vmaddfp");
        private static readonly IntrinsicProcedure vmaddcfp = IntrinsicBuilder.GenericBinary("__vmaddcfp");
        private static readonly IntrinsicProcedure vmax = IntrinsicBuilder.GenericBinary("__vector_max");
        private static readonly IntrinsicProcedure vmaxfp = IntrinsicBuilder.GenericBinary("__vector_fp_max");
        private static readonly IntrinsicProcedure vmhadds = IntrinsicBuilder.GenericTernary("__vector_mul_high_add_sat");
        private static readonly IntrinsicProcedure vmhradds = IntrinsicBuilder.GenericTernary("__vector_mul_high_round_add_sat");
        private static readonly IntrinsicProcedure vmin = IntrinsicBuilder.GenericBinary("__vector_min");
        private static readonly IntrinsicProcedure vminfp = IntrinsicBuilder.GenericBinary("__vector_fp_min");
        private static readonly IntrinsicProcedure vmladduhm = IntrinsicBuilder.GenericBinary("__vmladduhm");
        private static readonly IntrinsicProcedure vmrgh = IntrinsicBuilder.GenericBinary("__vector_merge_high");
        private static readonly IntrinsicProcedure vmrgl = IntrinsicBuilder.GenericBinary("__vector_merge_low");
        private static readonly IntrinsicProcedure vmsub3fp = IntrinsicBuilder.GenericBinary("__vmsub3fp");
        private static readonly IntrinsicProcedure vmsub4fp = IntrinsicBuilder.GenericBinary("__vmsub4fp");
        private static readonly IntrinsicProcedure vmsumshm = IntrinsicBuilder.GenericTernary("__vmsumshm");
        private static readonly IntrinsicProcedure vmsummm = new IntrinsicBuilder("__vector_mul_sum_mixed_modulo", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Param("TDst")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vmsumm = new IntrinsicBuilder("__vector_mul_sum_modulo", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Param("TDst")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vmsums = new IntrinsicBuilder("__vector_mul_sum_sat", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Param("TDst")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vmul10euq = IntrinsicBuilder.Binary("__vector_mul10_extended", PrimitiveType.UInt128);
        private static readonly IntrinsicProcedure vmule = new IntrinsicBuilder("__vector_mul_even", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vmulo = new IntrinsicBuilder("__vector_mul_odd", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vmulm = IntrinsicBuilder.GenericBinary("__vector_mul_modulo");
        private static readonly IntrinsicProcedure vnmsubfp = IntrinsicBuilder.GenericTernary("__vnmsubfp");
        private static readonly IntrinsicProcedure vperm = IntrinsicBuilder.GenericTernary("__vector_permute");
        private static readonly IntrinsicProcedure vpermr = IntrinsicBuilder.GenericTernary("__vector_permute_right_indexed");
        private static readonly IntrinsicProcedure vpermxor = IntrinsicBuilder.GenericTernary("__vector_permute_xor");
        private static readonly IntrinsicProcedure vpkd3d = IntrinsicBuilder.Pure("__vpkd3d")
            .GenericTypes("TSrc", "TDst")
            .Params("TSrc", "TSrc", "TSrc", "TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vpks = new IntrinsicBuilder("__vector_pack_sat", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vpmsum = new IntrinsicBuilder("__vector_poly_mac", false)
            .GenericTypes("TSrc", "TResult")
            .Param("TSrc")
            .Param("TSrc")
            .Returns("TResult");
        private static readonly IntrinsicProcedure vpopcnt = IntrinsicBuilder.GenericUnary("__vector_popcnt");
        private static readonly IntrinsicProcedure vrefp = IntrinsicBuilder.GenericUnary("__vector_reciprocal_estimate");
        private static readonly IntrinsicProcedure vrfim = IntrinsicBuilder.GenericUnary("__vector_floor");
        private static readonly IntrinsicProcedure vrfin = IntrinsicBuilder.GenericUnary("__vector_round");
        private static readonly IntrinsicProcedure vrfip = IntrinsicBuilder.GenericUnary("__vector_ceil");
        private static readonly IntrinsicProcedure vrfiz = IntrinsicBuilder.GenericUnary("__vector_round_toward_zero");
        private static readonly IntrinsicProcedure vrl = new IntrinsicBuilder("__vector_rotate_left", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure vrlimi = IntrinsicBuilder.GenericTernary("__vrlimi");
        private static readonly IntrinsicProcedure vrsqrtefp = IntrinsicBuilder.GenericUnary("__vector_reciprocal_sqrt_estimate");
        private static readonly IntrinsicProcedure vsbox = IntrinsicBuilder.Unary("__aes_subbytes", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure vsel = IntrinsicBuilder.Ternary("__vector_select", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure vsl = new IntrinsicBuilder("__vector_shift_left", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure vsldoi = new IntrinsicBuilder("__vector_shift_left_double", false)
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Word128)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Word128);
        private static readonly IntrinsicProcedure vsplt = new IntrinsicBuilder("__vector_splat", false)
            .GenericTypes("TArr")
            .Param("TArr")
            .Param(PrimitiveType.Byte)
            .Returns("TArr");
        private static readonly IntrinsicProcedure vsplti = new IntrinsicBuilder("__vector_splat_imm", false)
            .GenericTypes("TArr")
            .Param(PrimitiveType.Byte)
            .Returns("TArr"); 
        private static readonly IntrinsicProcedure vsr = new IntrinsicBuilder("__vector_shift_right", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure vsra = new IntrinsicBuilder("__vector_shift_right_arithmetic", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Byte)
            .Returns("T");
        private static readonly IntrinsicProcedure vsubc = IntrinsicBuilder.GenericBinary("__vector_sub_carry");
        private static readonly IntrinsicProcedure vsubec = IntrinsicBuilder.GenericTernary("__vector_sub_extend_carry");
        private static readonly IntrinsicProcedure vsubeuqm = IntrinsicBuilder.Ternary("__vector_sub_extended_modulo", PrimitiveType.UInt128);
        private static readonly IntrinsicProcedure vsubm = IntrinsicBuilder.GenericBinary("__vector_sub_modulo");
        private static readonly IntrinsicProcedure vsubs = IntrinsicBuilder.GenericBinary("__vector_sub_saturate");
        private static readonly IntrinsicProcedure vsum4s = new IntrinsicBuilder("__vector_sum_across_quarter_sat", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param("TDst")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vupkh = new IntrinsicBuilder("__vector_unpack_high", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vupkl = new IntrinsicBuilder("__vector_unpack_low", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vupkd3d = IntrinsicBuilder.GenericBinary("__vupkd3d");
        private static readonly IntrinsicProcedure vupkhpx = IntrinsicBuilder.Unary("__vector_unpack_high_pixel", PrimitiveType.Word128);
        private static readonly IntrinsicProcedure vupklpx = IntrinsicBuilder.Unary("__vector_unpack_low_pixel", PrimitiveType.Word128);
    }
}
