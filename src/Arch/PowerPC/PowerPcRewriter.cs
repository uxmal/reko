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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Reko.Core.Services;
using Reko.Core.Memory;

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
                case Mnemonic.illegal: iclass = InstrClass.Invalid; m.Invalid(); break;
                case Mnemonic.add: RewriteAdd(); break;
                case Mnemonic.addc: RewriteAddc(); break;
                case Mnemonic.addco: RewriteAddc(); break;
                case Mnemonic.adde: RewriteAdde(); break;
                case Mnemonic.addi: RewriteAddi(); break;
                case Mnemonic.addic: RewriteAddic(); break;
                case Mnemonic.addis: RewriteAddis(); break;
                case Mnemonic.addme: RewriteAddme(); break;
                case Mnemonic.addze: RewriteAddze(); break;
                case Mnemonic.and: RewriteAnd(false); break;
                case Mnemonic.andc: RewriteAndc(); break;
                case Mnemonic.andi: RewriteAnd(false); break;
                case Mnemonic.andis: RewriteAndis(); break;
                case Mnemonic.b: RewriteB(); break;
                case Mnemonic.bc: RewriteBc(false); break;
                case Mnemonic.bcctr: RewriteBcctr(false); break;
                case Mnemonic.bcctrl: RewriteBcctr(true); break;
                case Mnemonic.bctrl: RewriteBcctr(true); break;
                case Mnemonic.bcdadd: RewriteBcdadd(); break;
                case Mnemonic.bdnz: RewriteCtrBranch(false, false, m.Ne, false); break;
                case Mnemonic.bdnzf: RewriteCtrBranch(false, false, m.Ne, false); break;
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
                case Mnemonic.cntlzd: RewriteCntlz("__cntlzd", PrimitiveType.Word64); break;
                case Mnemonic.cntlzw: RewriteCntlz("__cntlzw", PrimitiveType.Word32); break;
                case Mnemonic.creqv: RewriteCrLogical("__creqv"); break;
                case Mnemonic.cror:  RewriteCrLogical("__cror"); break;
                case Mnemonic.crorc: RewriteCrLogical("__crorc"); break;
                case Mnemonic.crnand: RewriteCrLogical("__crnand"); break;
                case Mnemonic.crnor: RewriteCrLogical("__crnor"); break;
                case Mnemonic.crxor: RewriteCrLogical("__crxor"); break;
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
                case Mnemonic.evmhesmfaaw: RewriteVectorPairOp("__evmhesmfaaw", true, PrimitiveType.Word32); break;
                case Mnemonic.evmhessfaaw: RewriteVectorPairOp("__evmhessfaaw", true, PrimitiveType.Word32); break;
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
                case Mnemonic.fnmadd: RewriteFmadd(PrimitiveType.Real64, m.FAdd, true); break;
                case Mnemonic.fnmadds: RewriteFmadd(PrimitiveType.Real32, m.FAdd, true); break;
                case Mnemonic.fnmsub: RewriteFmadd(PrimitiveType.Real64, m.FSub, true); break;
                case Mnemonic.fnmsubs: RewriteFmadd(PrimitiveType.Real32, m.FSub, true); break;
                case Mnemonic.fmul: RewriteFmul(); break;
                case Mnemonic.fmuls: RewriteFmul(); break;
                case Mnemonic.fneg: RewriteFneg(); break;
                case Mnemonic.frsp: RewriteFrsp(); break;
                case Mnemonic.frsqrte: RewriteFrsqrte(); break;
                case Mnemonic.fsel: RewriteFsel(); break;
                case Mnemonic.fsqrt: RewriteFsqrt(); break;
                case Mnemonic.fsub: RewriteFsub(); break;
                case Mnemonic.fsubs: RewriteFsub(); break;
                case Mnemonic.icbi: RewriteIcbi(); break;
                case Mnemonic.isync: RewriteIsync(); break;
                case Mnemonic.lbz: RewriteLz(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzx: RewriteLzx(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzu: RewriteLzu(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.lbzux: RewriteLzux(PrimitiveType.Byte, arch.WordWidth); break;
                case Mnemonic.ld: RewriteLz(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.ldarx: RewriteLarx("__ldarx", PrimitiveType.Word64); break;
                case Mnemonic.ldu: RewriteLzu(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.ldx: RewriteLzx(PrimitiveType.Word64, arch.WordWidth); break;
                case Mnemonic.lfd: RewriteLfd(); break;
                case Mnemonic.lfdu: RewriteLzu(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.lfdux: RewriteLzux(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Mnemonic.lfdx: RewriteLzx(PrimitiveType.Real64, PrimitiveType.Real64); break;
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
                case Mnemonic.lvewx: RewriteLvewx(); break;
                case Mnemonic.lvlx:
                case Mnemonic.lvlx128: RewriteLvlx(); break;
                case Mnemonic.lvrx128: RewriteLvrx(); break;
                case Mnemonic.lvsl: RewriteLvsl(); break;
                case Mnemonic.lvx:
                case Mnemonic.lvx128: RewriteLzx(PrimitiveType.Word128, PrimitiveType.Word128); break;
                case Mnemonic.lwarx: RewriteLarx("__lwarx", PrimitiveType.Word32); break;
                case Mnemonic.lwax: RewriteLax(PrimitiveType.Int32, arch.SignedWord); break;
                case Mnemonic.lwbrx: RewriteLwbrx(); break;
                case Mnemonic.lwz: RewriteLz(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzu: RewriteLzu(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzux: RewriteLzux(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.lwzx: RewriteLzx(PrimitiveType.Word32, arch.WordWidth); break;
                case Mnemonic.mcrf: RewriteMcrf(); break;
                case Mnemonic.mfcr: RewriteMfcr(); break;
                case Mnemonic.mfctr: RewriteMfctr(); break;
                case Mnemonic.mftb: RewriteMftb(); break;
                case Mnemonic.mffs: RewriteMffs(); break;
                case Mnemonic.mflr: RewriteMflr(); break;
                case Mnemonic.mfmsr: RewriteMfmsr(); break;
                case Mnemonic.mfspr: RewriteMfspr(); break;
                case Mnemonic.mtcrf: RewriteMtcrf(); break;
                case Mnemonic.mtctr: RewriteMtctr(); break;
                case Mnemonic.mtfsf: RewriteMtfsf(); break;
                case Mnemonic.mtmsr: RewriteMtmsr(PrimitiveType.Word32); break;
                case Mnemonic.mtmsrd: RewriteMtmsr(PrimitiveType.Word64); break;
                case Mnemonic.mtspr: RewriteMtspr(); break;
                case Mnemonic.mtlr: RewriteMtlr(); break;
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
                case Mnemonic.ps_abs: RewritePairedInstruction_Src1("__ps_abs"); break;
                case Mnemonic.ps_add: RewritePairedInstruction_Src2("__ps_add"); break;
                case Mnemonic.ps_cmpo0: Rewrite_ps_cmpo("__ps_cmpo0"); break;
                case Mnemonic.ps_div: RewritePairedInstruction_Src2("__ps_div"); break;
                case Mnemonic.psq_l: Rewrite_psq_l(false); break;
                case Mnemonic.psq_lu: Rewrite_psq_l(true); break;
                case Mnemonic.psq_lx: Rewrite_psq_l(false); break;
                case Mnemonic.psq_lux: Rewrite_psq_l(true); break;
                case Mnemonic.ps_madd: RewritePairedInstruction_Src3("__ps_madd"); break;
                case Mnemonic.ps_madds0: RewritePairedInstruction_Src3("__ps_madds0"); break;
                case Mnemonic.ps_madds1: RewritePairedInstruction_Src3("__ps_madds1"); break;
                case Mnemonic.ps_merge00: RewritePairedInstruction_Src2("__ps_merge00"); break;
                case Mnemonic.ps_merge01: RewritePairedInstruction_Src2("__ps_merge01"); break;
                case Mnemonic.ps_merge10: RewritePairedInstruction_Src2("__ps_merge10"); break;
                case Mnemonic.ps_merge11: RewritePairedInstruction_Src2("__ps_merge11"); break;
                case Mnemonic.ps_mr: Rewrite_ps_mr(); break;
                case Mnemonic.ps_mul: RewritePairedInstruction_Src2("__ps_mul"); break;
                case Mnemonic.ps_muls0: RewritePairedInstruction_Src2("__ps_muls0"); break;
                case Mnemonic.ps_muls1: RewritePairedInstruction_Src2("__ps_muls1"); break;
                case Mnemonic.ps_nmadd: RewritePairedInstruction_Src3("__ps_nmadd"); break;
                case Mnemonic.ps_nmsub: RewritePairedInstruction_Src3("__ps_nmsub"); break;
                case Mnemonic.ps_nabs: RewritePairedInstruction_Src1("__ps_nabs"); break;
                case Mnemonic.ps_neg: RewritePairedInstruction_Src1("__ps_neg"); break;
                case Mnemonic.ps_res: RewritePairedInstruction_Src1("__ps_res"); break;
                case Mnemonic.ps_rsqrte: RewritePairedInstruction_Src1("__ps_rsqrte"); break;
                case Mnemonic.ps_sel: RewritePairedInstruction_Src3("__ps_sel"); break;
                case Mnemonic.ps_sub: RewritePairedInstruction_Src2("__ps_sub"); break;
                case Mnemonic.ps_sum0: RewritePairedInstruction_Src3("__ps_sum0"); break;
                case Mnemonic.psq_st: Rewrite_psq_st(false); break;
                case Mnemonic.psq_stu: Rewrite_psq_st(true); break;
                case Mnemonic.psq_stx: Rewrite_psq_st(false); break;
                case Mnemonic.psq_stux: Rewrite_psq_st(true); break;
                case Mnemonic.rfi: RewriteRfi(); break;
                case Mnemonic.rfid: RewriteRfi(); break;
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
                case Mnemonic.stbu: RewriteStu(PrimitiveType.Byte); break;
                case Mnemonic.stbux: RewriteStux(PrimitiveType.Byte); break;
                case Mnemonic.stbx: RewriteStx(PrimitiveType.Byte); break;
                case Mnemonic.std: RewriteSt(PrimitiveType.Word64); break;
                case Mnemonic.stdcx: RewriteStcx("__stdcx", PrimitiveType.Word64); break;
                case Mnemonic.stdu: RewriteStu(PrimitiveType.Word64); break;
                case Mnemonic.stdx: RewriteStx(PrimitiveType.Word64); break;
                case Mnemonic.stfd: RewriteSt(PrimitiveType.Real64); break;
                case Mnemonic.stfdu: RewriteStu(PrimitiveType.Real64); break;
                case Mnemonic.stfdux: RewriteStux(PrimitiveType.Real64); break;
                case Mnemonic.stfdx: RewriteStx(PrimitiveType.Real64); break;
                case Mnemonic.stfiwx: RewriteStx(PrimitiveType.Int32); break;
                case Mnemonic.stfs: RewriteSt(PrimitiveType.Real32); break;
                case Mnemonic.stfsu: RewriteStu(PrimitiveType.Real32); break;
                case Mnemonic.stfsx: RewriteStx(PrimitiveType.Real32); break;
                case Mnemonic.sth: RewriteSt(PrimitiveType.Word16); break;
                case Mnemonic.sthu: RewriteStu(PrimitiveType.Word16); break;
                case Mnemonic.sthx: RewriteStx(PrimitiveType.Word16); break;
                case Mnemonic.stmw: RewriteStmw(); break;
                case Mnemonic.stswi: RewriteStswi(); break;
                case Mnemonic.stvewx: RewriteStvewx(); break;
                case Mnemonic.stvx: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stvx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stvlx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stvrx128: RewriteStx(PrimitiveType.Word128); break;
                case Mnemonic.stw: RewriteSt(PrimitiveType.Word32); break;
                case Mnemonic.stwbrx: RewriteStwbrx(); break;
                case Mnemonic.stwcx: RewriteStcx("__stwcx", PrimitiveType.Word32); break;
                case Mnemonic.stwu: RewriteStu(PrimitiveType.Word32); break;
                case Mnemonic.stwux: RewriteStux(PrimitiveType.Word32); break;
                case Mnemonic.stwx: RewriteStx(PrimitiveType.Word32); break;
                case Mnemonic.subf: RewriteSubf(); break;
                case Mnemonic.subfc: RewriteSubfc(); break;
                case Mnemonic.subfco: RewriteSubfc(); break;
                case Mnemonic.subfe: RewriteSubfe(); break;
                case Mnemonic.subfic: RewriteSubfic(); break;
                case Mnemonic.subfze: RewriteSubfze(); break;
                case Mnemonic.sync: RewriteSync(); break;
                case Mnemonic.td: RewriteTrap(PrimitiveType.Word64); break;
                case Mnemonic.tdi: RewriteTrap(PrimitiveType.Word64); break;
                case Mnemonic.tlbie: RewriteTlbie(); break;
                case Mnemonic.tlbsync: RewriteTlbsync(); break;
                case Mnemonic.tw: RewriteTrap(PrimitiveType.Word32); break;
                case Mnemonic.twi: RewriteTrap(PrimitiveType.Word32); break;
                case Mnemonic.vaddfp: RewriteVaddfp(); break;
                case Mnemonic.vaddubm: RewriteVectorBinOp("__vaddubm", true, PrimitiveType.UInt8); break;
                case Mnemonic.vaddubs: RewriteVectorBinOp("__vaddubs", true, PrimitiveType.UInt8); break;
                case Mnemonic.vadduwm: RewriteVectorBinOp("__vadduwm", true, PrimitiveType.UInt32); break;
                case Mnemonic.vadduqm: RewriteAdd(); break;
                case Mnemonic.vand:
                case Mnemonic.vand128: RewriteAnd(false); break;
                case Mnemonic.vandc: RewriteAndc(); break;
                case Mnemonic.vcfsx: RewriteVct("__vcfsx", PrimitiveType.Real32); break;
                case Mnemonic.vcfpsxws128: RewriteVcfpsxws("__vcfpsxws"); break;
                case Mnemonic.vcmpbfp:
                case Mnemonic.vcmpbfp128: RewriteVcmpfp("__vcmpebfp"); break;
                case Mnemonic.vcmpeqfp:
                case Mnemonic.vcmpeqfp128: RewriteVcmpfp("__vcmpeqfp"); break;
                case Mnemonic.vcmpgtfp:
                case Mnemonic.vcmpgtfp128: RewriteVcmpfp("__vcmpgtfp"); break;
                case Mnemonic.vcmpgtuw: RewriteVcmpu("__vcmpgtuw", PrimitiveType.UInt32); break;
                case Mnemonic.vcmpequb: RewriteVcmpu("__vcmpequb", PrimitiveType.UInt8); break;
                case Mnemonic.vcmpequd: RewriteVcmpu("__vcmpequd", PrimitiveType.UInt64); break;
                case Mnemonic.vcmpequw: RewriteVcmpu("__vcmpequw", PrimitiveType.UInt32); break;
                case Mnemonic.vcsxwfp128: RewriteVcsxwfp("__vcsxwfp"); break;
                case Mnemonic.vctsxs: RewriteVct("__vctsxs", PrimitiveType.Int32); break;
                case Mnemonic.vexptefp128: RewriteVectorUnary("__vexptefp", true); break;
                case Mnemonic.vlogefp128: RewriteVectorUnary("__vlogefp", true); break;
                case Mnemonic.vmaddfp: RewriteVmaddfp(); break;
                case Mnemonic.vmaddcfp128: RewriteVectorBinOp("__vmaddcfp", true, PrimitiveType.Real32); break;
                case Mnemonic.vmaxfp128: RewriteVectorBinOp("__vmaxfp", true, PrimitiveType.Real32); break;
                case Mnemonic.vminfp128: RewriteVectorBinOp("__vminfp", true, PrimitiveType.Real32); break;
                case Mnemonic.vmaxub: RewriteVectorBinOp("__vmaxub", true, PrimitiveType.UInt8); break;
                case Mnemonic.vmaxuh: RewriteVectorBinOp("__vmaxuh", true, PrimitiveType.UInt16); break;
                case Mnemonic.vmladduhm: RewriteVectorBinOp("__vmladduhm", true, PrimitiveType.UInt16); break;
                case Mnemonic.vmrghw:
                case Mnemonic.vmrghw128: RewriteVmrghw(); break;
                case Mnemonic.vmrglw:
                case Mnemonic.vmrglw128: RewriteVmrglw(); break;
                case Mnemonic.vmsumshm: RewriteVectorTernaryOp("__vmsumshm", true, PrimitiveType.Int16); break;
                case Mnemonic.vmsub3fp128: RewriteVectorBinOp("__vmsub3fp", true, PrimitiveType.Real32); break;
                case Mnemonic.vmsub4fp128: RewriteVectorBinOp("__vmsub4fp", true, PrimitiveType.Real32); break;  //$REVIEW: is it correct?
                case Mnemonic.vmulfp128: RewriteVectorBinOp("__vmulfp", true, PrimitiveType.Real32); break;         //$REVIEW: is it correct?
                case Mnemonic.vnmsubfp: RewriteVnmsubfp(); break;
                case Mnemonic.vnor: RewriteOr(true); break;
                case Mnemonic.vor:
                case Mnemonic.vor128: RewriteVor(); break;
                case Mnemonic.vorc: RewriteOrc();break;
                case Mnemonic.vperm:
                case Mnemonic.vperm128: RewriteVperm(); break;
                case Mnemonic.vpkd3d128: RewriterVpkD3d(); break;
                case Mnemonic.vrefp:
                case Mnemonic.vrefp128: RewriteVrefp(); break;
                case Mnemonic.vrfin128: RewriteVectorUnary("__vrfin", true); break;
                case Mnemonic.vrfip128: RewriteVectorUnary("__vrfip", true); break;
                case Mnemonic.vrfiz128: RewriteVectorUnary("__vrfiz", true); break;
                case Mnemonic.vrlimi128: RewriteVrlimi(); break;
                case Mnemonic.vrsqrtefp: 
                case Mnemonic.vrsqrtefp128: RewriteVrsqrtefp(); break;
                case Mnemonic.vsel: RewriteVsel(); break;
                case Mnemonic.vsldoi: RewriteVsldoi(); break;
                case Mnemonic.vslw:
                case Mnemonic.vslw128: RewriteVsxw("__vslw"); break;
                case Mnemonic.vspltisw:
                case Mnemonic.vspltisw128: RewriteVspltisw(); break;
                case Mnemonic.vspltw:
                case Mnemonic.vspltw128: RewriteVspltw(); break;
                case Mnemonic.vsrw128: RewriteVsxw("__vsrw"); break;
                case Mnemonic.vsubfp:
                case Mnemonic.vsubfp128: RewriteVsubfp(); break;
                case Mnemonic.vupkd3d128: RewriteVupkd3d(); break;
                case Mnemonic.vxor:
                case Mnemonic.vxor128: RewriteXor(false); break;
                case Mnemonic.xor: RewriteXor(false); break;
                case Mnemonic.xori: RewriteXor(false); break;
                case Mnemonic.xoris: RewriteXoris(); break;
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

        private Expression RewriteOperand(int iop, bool maybe0 = false) =>
            RewriteOperand(instr.Operands[iop], maybe0);

        private Expression RewriteOperand(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterOperand rOp:
                if (maybe0 && rOp.Register.Number == 0)
                    return Constant.Zero(rOp.Register.DataType);
                if (arch.IsCcField(rOp.Register))
                {
                    return binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(rOp.Register)!);
                }
                else
                {
                    return binder.EnsureRegister(rOp.Register);
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
            case RegisterOperand rOp:
                if (maybe0 && rOp.Register.Number == 0)
                    return Constant.Zero(rOp.Register.DataType);
                if (arch.IsCcField(rOp.Register))
                {
                    return binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(rOp.Register)!);
                }
                else
                {
                    return binder.EnsureRegister(rOp.Register);
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
            var offset = mop.Offset;
            return emitter.IAddS(reg, offset);
        }

        private Expression EffectiveAddress_r0(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32(mop.Offset);
            }
            else
            {
                var reg = binder.EnsureRegister(mop.BaseRegister);
                var offset = mop.Offset;
                if (offset != 0)
                    return emitter.IAddS(reg, offset);
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
    }
}
