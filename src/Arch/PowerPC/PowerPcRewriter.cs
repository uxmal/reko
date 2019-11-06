#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private InstrClass rtlc;
        private List<RtlInstruction> rtlInstructions;

        public PowerPcRewriter(PowerPcArchitecture arch, IEnumerable<PowerPcInstruction> instrs, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.dasm = instrs.GetEnumerator();
        }

        public PowerPcRewriter(PowerPcArchitecture arch, EndianImageReader rdr, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.binder = binder;
            this.host = host;
            this.rdr = rdr;
            this.dasm = arch.CreateDisassemblerImpl(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addr = this.instr.Address;
                this.rtlInstructions = new List<RtlInstruction>();
                this.rtlc = instr.InstructionClass;
                this.m = new RtlEmitter(rtlInstructions);
                switch (dasm.Current.Mnemonic)
                {
                default:
                    host.Error(
                        instr.Address, 
                        string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Opcode.illegal;
                case Opcode.illegal: rtlc = InstrClass.Invalid; m.Invalid(); break;
                case Opcode.addi: RewriteAddi(); break;
                case Opcode.addc: RewriteAddc(); break;
                case Opcode.addic: RewriteAddic(); break;
                case Opcode.addis: RewriteAddis(); break;
                case Opcode.add: RewriteAdd(); break;
                case Opcode.adde: RewriteAdde(); break;
                case Opcode.addme: RewriteAddme(); break;
                case Opcode.addze: RewriteAddze(); break;
                case Opcode.and: RewriteAnd(false); break;
                case Opcode.andc: RewriteAndc(); break;
                case Opcode.andi: RewriteAnd(false); break;
                case Opcode.andis: RewriteAndis(); break;
                case Opcode.b: RewriteB(); break;
                case Opcode.bc: RewriteBc(false); break;
                case Opcode.bcctr: RewriteBcctr(false); break;
                case Opcode.bctrl: RewriteBcctr(true); break;
                case Opcode.bcdadd: RewriteBcdadd(); break;
                case Opcode.bdnz: RewriteCtrBranch(false, false, m.Ne, false); break;
                case Opcode.bdnzf: RewriteCtrBranch(false, false, m.Ne, false); break;
                case Opcode.bdnzl: RewriteCtrBranch(true, false, m.Ne, false); break;
                case Opcode.bdnzt: RewriteCtrBranch(false, false, m.Ne, true); break;
                case Opcode.bdnztl: RewriteCtrBranch(true, false, m.Ne, true); break;
                case Opcode.bdz: RewriteCtrBranch(false, false, m.Eq, false); break;
                case Opcode.bdzf: RewriteCtrBranch(false, false, m.Eq, false); break;
                case Opcode.bdzfl: RewriteCtrBranch(true, false, m.Eq, false); break;
                case Opcode.bdzt: RewriteCtrBranch(false, false, m.Eq, true); break;
                case Opcode.bdztl: RewriteCtrBranch(true, false, m.Eq, true); break;
                case Opcode.bdzl: RewriteCtrBranch(true, false, m.Eq, false); break;
                case Opcode.beq: RewriteBranch(false, false,ConditionCode.EQ); break;
                case Opcode.beql: RewriteBranch(true, false, ConditionCode.EQ); break;
                case Opcode.beqlr: RewriteBranch(false, true, ConditionCode.EQ); break;
                case Opcode.beqlrl: RewriteBranch(true, true, ConditionCode.EQ); break;
                case Opcode.bge: RewriteBranch(false, false,ConditionCode.GE); break;
                case Opcode.bgel: RewriteBranch(true, false,ConditionCode.GE); break;
                case Opcode.bgelr: RewriteBranch(false, true,ConditionCode.GE); break;
                case Opcode.bgt: RewriteBranch(false, false,ConditionCode.GT); break;
                case Opcode.bgtl: RewriteBranch(true, false,ConditionCode.GT); break;
                case Opcode.bgtlr: RewriteBranch(false, true,ConditionCode.GT); break;
                case Opcode.bl: RewriteBl(); break;
                case Opcode.blr: RewriteBlr(); break;
                case Opcode.bltlr: RewriteBranch(false, true,ConditionCode.LT); break;
                case Opcode.ble: RewriteBranch(false, false, ConditionCode.LE); break;
                case Opcode.blel: RewriteBranch(true, false, ConditionCode.LE); break;
                case Opcode.blelr: RewriteBranch(false, true, ConditionCode.LE); break;
                case Opcode.blelrl: RewriteBranch(true, true, ConditionCode.LE); break;
                case Opcode.blt: RewriteBranch(false, false, ConditionCode.LT); break;
                case Opcode.bltl: RewriteBranch(true, false, ConditionCode.LT); break;
                case Opcode.bne: RewriteBranch(false, false, ConditionCode.NE); break;
                case Opcode.bnel: RewriteBranch(true, false, ConditionCode.NE); break;
                case Opcode.bnelr: RewriteBranch(false, true, ConditionCode.NE); break;
                case Opcode.bns: RewriteBranch(false, false, ConditionCode.NO); break;
                case Opcode.bnsl: RewriteBranch(true, false, ConditionCode.NO); break;
                case Opcode.bso: RewriteBranch(false, false, ConditionCode.OV); break;
                case Opcode.bsol: RewriteBranch(true, false, ConditionCode.OV); break;
                case Opcode.cmp: RewriteCmp(); break;
                case Opcode.cmpi: RewriteCmpi(); break;
                case Opcode.cmpl: RewriteCmpl(); break;
                case Opcode.cmpli: RewriteCmpli(); break;
                case Opcode.cmplw: RewriteCmplw(); break;
                case Opcode.cmplwi: RewriteCmplwi(); break;
                case Opcode.cmpwi: RewriteCmpwi(); break;
                case Opcode.cntlzd: RewriteCntlz("__cntlzd", PrimitiveType.Word64); break;
                case Opcode.cntlzw: RewriteCntlz("__cntlzw", PrimitiveType.Word32); break;
                case Opcode.creqv: RewriteCreqv(); break;
                case Opcode.cror: RewriteCror(); break;
                case Opcode.crnor: RewriteCrnor(); break;
                case Opcode.crxor: RewriteCrxor(); break;
                case Opcode.dcbf: RewriteDcbf(); break;
                case Opcode.dcbi: RewriteDcbi(); break;
                case Opcode.dcbst: RewriteDcbst(); break;
                case Opcode.dcbt: RewriteDcbt(); break;
                case Opcode.dcbtst: RewriteDcbtst(); break;
                case Opcode.dcbz: RewriteDcbz(); break;
                case Opcode.divd: RewriteDivd(m.SDiv); break;
                case Opcode.divdu: RewriteDivd(m.UDiv); break;
                case Opcode.divw: RewriteDivw(); break;
                case Opcode.divwu: RewriteDivwu(); break;
                case Opcode.eieio: RewriteEieio(); break;
                case Opcode.evmhesmfaaw: RewriteVectorPairOp("__evmhesmfaaw", PrimitiveType.Word32); break;
                case Opcode.evmhessfaaw: RewriteVectorPairOp("__evmhessfaaw", PrimitiveType.Word32); break;
                case Opcode.eqv: RewriteXor(true); break;
                case Opcode.extsb: RewriteExts(PrimitiveType.SByte); break;
                case Opcode.extsh: RewriteExts(PrimitiveType.Int16); break;
                case Opcode.extsw: RewriteExts(PrimitiveType.Int32); break;
                case Opcode.fabs: RewriteFabs(); break;
                case Opcode.fadd: RewriteFadd(); break;
                case Opcode.fadds: RewriteFadd(); break;
                case Opcode.fcfid: RewriteFcfid(); break;
                case Opcode.fctid: RewriteFctid(); break;
                case Opcode.fctidz: RewriteFctidz(); break;
                case Opcode.fctiwz: RewriteFctiwz(); break;
                case Opcode.fcmpo: RewriteFcmpo(); break;
                case Opcode.fcmpu: RewriteFcmpu(); break;
                case Opcode.fdiv: RewriteFdiv(); break;
                case Opcode.fdivs: RewriteFdiv(); break;
                case Opcode.fmr: RewriteFmr(); break;
                case Opcode.fmadd: RewriteFmadd(PrimitiveType.Real64, m.FAdd, false); break;
                case Opcode.fmadds: RewriteFmadd(PrimitiveType.Real32, m.FAdd, false); break;
                case Opcode.fmsub: RewriteFmadd(PrimitiveType.Real64, m.FSub, false); break;
                case Opcode.fmsubs: RewriteFmadd(PrimitiveType.Real32, m.FSub, false); break;
                case Opcode.fnmadd: RewriteFmadd(PrimitiveType.Real64, m.FAdd, true); break;
                case Opcode.fnmadds: RewriteFmadd(PrimitiveType.Real32, m.FAdd, true); break;
                case Opcode.fnmsub: RewriteFmadd(PrimitiveType.Real64, m.FSub, true); break;
                case Opcode.fnmsubs: RewriteFmadd(PrimitiveType.Real32, m.FSub, true); break;
                case Opcode.fmul: RewriteFmul(); break;
                case Opcode.fmuls: RewriteFmul(); break;
                case Opcode.fneg: RewriteFneg(); break;
                case Opcode.frsp: RewriteFrsp(); break;
                case Opcode.frsqrte: RewriteFrsqrte(); break;
                case Opcode.fsel: RewriteFsel(); break;
                case Opcode.fsqrt: RewriteFsqrt(); break;
                case Opcode.fsub: RewriteFsub(); break;
                case Opcode.fsubs: RewriteFsub(); break;
                case Opcode.icbi: RewriteIcbi(); break;
                case Opcode.isync: RewriteIsync(); break;
                case Opcode.lbz: RewriteLz(PrimitiveType.Byte, arch.WordWidth); break;
                case Opcode.lbzx: RewriteLzx(PrimitiveType.Byte, arch.WordWidth); break;
                case Opcode.lbzu: RewriteLzu(PrimitiveType.Byte, arch.WordWidth); break;
                case Opcode.lbzux: RewriteLzux(PrimitiveType.Byte, arch.WordWidth); break;
                case Opcode.ld: RewriteLz(PrimitiveType.Word64, arch.WordWidth); break;
                case Opcode.ldarx: RewriteLarx("__ldarx", PrimitiveType.Word64); break;
                case Opcode.ldu: RewriteLzu(PrimitiveType.Word64, arch.WordWidth); break;
                case Opcode.ldx: RewriteLzx(PrimitiveType.Word64, arch.WordWidth); break;
                case Opcode.lfd: RewriteLfd(); break;
                case Opcode.lfdu: RewriteLzu(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.lfdux: RewriteLzux(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.lfdx: RewriteLzx(PrimitiveType.Real64, PrimitiveType.Real64); break;
                case Opcode.lfs: RewriteLfs(); break;
                case Opcode.lfsu: RewriteLzu(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.lfsux: RewriteLzux(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.lfsx: RewriteLzx(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Opcode.lha: RewriteLa(PrimitiveType.Int16, arch.SignedWord); break;
                case Opcode.lhax: RewriteLax(PrimitiveType.Int16, arch.SignedWord); break;
                case Opcode.lhau: RewriteLau(PrimitiveType.Int16, arch.SignedWord); break;
                case Opcode.lhaux: RewriteLaux(PrimitiveType.Int16, arch.SignedWord); break;
                case Opcode.lhbrx: RewriteLhbrx(); break;
                case Opcode.lhz: RewriteLz(PrimitiveType.Word16, arch.WordWidth); break;
                case Opcode.lhzu: RewriteLzu(PrimitiveType.Word16, arch.WordWidth); break;
                case Opcode.lhzx: RewriteLzx(PrimitiveType.Word16, arch.WordWidth); break;
                case Opcode.lmw: RewriteLmw(); break;
                case Opcode.lq: RewriteLq(); break;
                case Opcode.lvewx: RewriteLvewx(); break;
                case Opcode.lvlx:
                case Opcode.lvlx128: RewriteLvlx(); break;
                case Opcode.lvrx128: RewriteLvrx(); break;
                case Opcode.lvsl: RewriteLvsl(); break;
                case Opcode.lvx:
                case Opcode.lvx128: RewriteLzx(PrimitiveType.Word128, PrimitiveType.Word128); break;
                case Opcode.lwarx: RewriteLarx("__lwarx", PrimitiveType.Word32); break;
                case Opcode.lwax: RewriteLax(PrimitiveType.Int32, arch.SignedWord); break;
                case Opcode.lwbrx: RewriteLwbrx(); break;
                case Opcode.lwz: RewriteLz(PrimitiveType.Word32, arch.WordWidth); break;
                case Opcode.lwzu: RewriteLzu(PrimitiveType.Word32, arch.WordWidth); break;
                case Opcode.lwzux: RewriteLzux(PrimitiveType.Word32, arch.WordWidth); break;
                case Opcode.lwzx: RewriteLzx(PrimitiveType.Word32, arch.WordWidth); break;
                case Opcode.mcrf: RewriteMcrf(); break;
                case Opcode.mfcr: RewriteMfcr(); break;
                case Opcode.mfctr: RewriteMfctr(); break;
                case Opcode.mftb: RewriteMftb(); break;
                case Opcode.mffs: RewriteMffs(); break;
                case Opcode.mflr: RewriteMflr(); break;
                case Opcode.mfmsr: RewriteMfmsr(); break;
                case Opcode.mfspr: RewriteMfspr(); break;
                case Opcode.mtcrf: RewriteMtcrf(); break;
                case Opcode.mtctr: RewriteMtctr(); break;
                case Opcode.mtfsf: RewriteMtfsf(); break;
                case Opcode.mtmsr: RewriteMtmsr(PrimitiveType.Word32); break;
                case Opcode.mtmsrd: RewriteMtmsr(PrimitiveType.Word64); break;
                case Opcode.mtspr: RewriteMtspr(); break;
                case Opcode.mtlr: RewriteMtlr(); break;
                case Opcode.mulhw: RewriteMulhw(); break;
                case Opcode.mulhwu: RewriteMulhwu(); break;
                case Opcode.mulhhwu: RewriteMulhhwu(); break;
                case Opcode.mulli: RewriteMull(); break;
                case Opcode.mulld: RewriteMull(); break;
                case Opcode.mullw: RewriteMull(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nand: RewriteAnd(true); break;
                case Opcode.nor: RewriteOr(true); break;
                case Opcode.or: RewriteOr(false); break;
                case Opcode.orc: RewriteOrc(false); break;
                case Opcode.ori: RewriteOr(false); break;
                case Opcode.oris: RewriteOris(); break;
                case Opcode.ps_abs: RewritePairedInstruction_Src1("__ps_abs"); break;
                case Opcode.ps_add: RewritePairedInstruction_Src2("__ps_add"); break;
                case Opcode.ps_cmpo0: Rewrite_ps_cmpo("__ps_cmpo0"); break;
                case Opcode.ps_div: RewritePairedInstruction_Src2("__ps_div"); break;
                case Opcode.psq_l: Rewrite_psq_l(false); break;
                case Opcode.psq_lu: Rewrite_psq_l(true); break;
                case Opcode.psq_lx: Rewrite_psq_l(false); break;
                case Opcode.psq_lux: Rewrite_psq_l(true); break;
                case Opcode.ps_madd: RewritePairedInstruction_Src3("__ps_madd"); break;
                case Opcode.ps_madds0: RewritePairedInstruction_Src3("__ps_madds0"); break;
                case Opcode.ps_madds1: RewritePairedInstruction_Src3("__ps_madds1"); break;
                case Opcode.ps_merge00: RewritePairedInstruction_Src2("__ps_merge00"); break;
                case Opcode.ps_merge01: RewritePairedInstruction_Src2("__ps_merge01"); break;
                case Opcode.ps_merge10: RewritePairedInstruction_Src2("__ps_merge10"); break;
                case Opcode.ps_merge11: RewritePairedInstruction_Src2("__ps_merge11"); break;
                case Opcode.ps_mr: Rewrite_ps_mr(); break;
                case Opcode.ps_mul: RewritePairedInstruction_Src2("__ps_mul"); break;
                case Opcode.ps_muls0: RewritePairedInstruction_Src2("__ps_muls0"); break;
                case Opcode.ps_muls1: RewritePairedInstruction_Src2("__ps_muls1"); break;
                case Opcode.ps_nmadd: RewritePairedInstruction_Src3("__ps_nmadd"); break;
                case Opcode.ps_nmsub: RewritePairedInstruction_Src3("__ps_nmsub"); break;
                case Opcode.ps_nabs: RewritePairedInstruction_Src1("__ps_nabs"); break;
                case Opcode.ps_neg: RewritePairedInstruction_Src1("__ps_neg"); break;
                case Opcode.ps_res: RewritePairedInstruction_Src1("__ps_res"); break;
                case Opcode.ps_rsqrte: RewritePairedInstruction_Src1("__ps_rsqrte"); break;
                case Opcode.ps_sel: RewritePairedInstruction_Src3("__ps_sel"); break;
                case Opcode.ps_sub: RewritePairedInstruction_Src2("__ps_sub"); break;
                case Opcode.ps_sum0: RewritePairedInstruction_Src3("__ps_sum0"); break;
                case Opcode.psq_st: Rewrite_psq_st(false); break;
                case Opcode.psq_stu: Rewrite_psq_st(true); break;
                case Opcode.psq_stx: Rewrite_psq_st(false); break;
                case Opcode.psq_stux: Rewrite_psq_st(true); break;
                case Opcode.rfi: RewriteRfi(); break;
                case Opcode.rldicl: RewriteRldicl(); break;
                case Opcode.rldicr: RewriteRldicr(); break;
                case Opcode.rldimi: RewriteRldimi(); break;
                case Opcode.rlwinm: RewriteRlwinm(); break;
                case Opcode.rlwimi: RewriteRlwimi(); break;
                case Opcode.rlwnm: RewriteRlwnm(); break;
                case Opcode.sc: RewriteSc(); break;
                case Opcode.sld: RewriteSl(PrimitiveType.Word64); break;
                case Opcode.slw: RewriteSl(PrimitiveType.Word32); break;
                case Opcode.srad: RewriteSra(); break;
                case Opcode.sradi: RewriteSra(); break;
                case Opcode.sraw: RewriteSra(); break;
                case Opcode.srawi: RewriteSra(); break;
                case Opcode.srd: RewriteSrw(); break;
                case Opcode.srw: RewriteSrw(); break;
                case Opcode.stb: RewriteSt(PrimitiveType.Byte); break;
                case Opcode.stbu: RewriteStu(PrimitiveType.Byte); break;
                case Opcode.stbux: RewriteStux(PrimitiveType.Byte); break;
                case Opcode.stbx: RewriteStx(PrimitiveType.Byte); break;
                case Opcode.std: RewriteSt(PrimitiveType.Word64); break;
                case Opcode.stdcx: RewriteStcx("__stdcx", PrimitiveType.Word64); break;
                case Opcode.stdu: RewriteStu(PrimitiveType.Word64); break;
                case Opcode.stdx: RewriteStx(PrimitiveType.Word64); break;
                case Opcode.stfd: RewriteSt(PrimitiveType.Real64); break;
                case Opcode.stfdu: RewriteStu(PrimitiveType.Real64); break;
                case Opcode.stfdux: RewriteStux(PrimitiveType.Real64); break;
                case Opcode.stfdx: RewriteStx(PrimitiveType.Real64); break;
                case Opcode.stfiwx: RewriteStx(PrimitiveType.Int32); break;
                case Opcode.stfs: RewriteSt(PrimitiveType.Real32); break;
                case Opcode.stfsu: RewriteStu(PrimitiveType.Real32); break;
                case Opcode.stfsx: RewriteStx(PrimitiveType.Real32); break;
                case Opcode.sth: RewriteSt(PrimitiveType.Word16); break;
                case Opcode.sthu: RewriteStu(PrimitiveType.Word16); break;
                case Opcode.sthx: RewriteStx(PrimitiveType.Word16); break;
                case Opcode.stmw: RewriteStmw(); break;
                case Opcode.stvewx: RewriteStvewx(); break;
                case Opcode.stvx: RewriteStx(PrimitiveType.Word128); break;
                case Opcode.stvx128: RewriteStx(PrimitiveType.Word128); break;
                case Opcode.stvlx128: RewriteStx(PrimitiveType.Word128); break;
                case Opcode.stvrx128: RewriteStx(PrimitiveType.Word128); break;
                case Opcode.stw: RewriteSt(PrimitiveType.Word32); break;
                case Opcode.stwbrx: RewriteStwbrx(); break;
                case Opcode.stwcx: RewriteStcx("__stwcx", PrimitiveType.Word32); break;
                case Opcode.stwu: RewriteStu(PrimitiveType.Word32); break;
                case Opcode.stwux: RewriteStux(PrimitiveType.Word32); break;
                case Opcode.stwx: RewriteStx(PrimitiveType.Word32); break;
                case Opcode.subf: RewriteSubf(); break;
                case Opcode.subfc: RewriteSubfc(); break;
                case Opcode.subfe: RewriteSubfe(); break;
                case Opcode.subfic: RewriteSubfic(); break;
                case Opcode.subfze: RewriteSubfze(); break;
                case Opcode.sync: RewriteSync(); break;
                case Opcode.td: RewriteTrap(PrimitiveType.Word64); break;
                case Opcode.tdi: RewriteTrap(PrimitiveType.Word64); break;
                case Opcode.tw: RewriteTrap(PrimitiveType.Word32); break;
                case Opcode.twi: RewriteTrap(PrimitiveType.Word32); break;
                case Opcode.vaddfp: RewriteVaddfp(); break;
                case Opcode.vaddubm: RewriteVectorBinOp("__vaddubm", PrimitiveType.UInt8); break;
                case Opcode.vaddubs: RewriteVectorBinOp("__vaddubs", PrimitiveType.UInt8); break;
                case Opcode.vadduwm: RewriteVectorBinOp("__vadduwm", PrimitiveType.UInt32); break;
                case Opcode.vadduqm: RewriteAdd(); break;
                case Opcode.vand:
                case Opcode.vand128: RewriteAnd(false); break;
                case Opcode.vandc: RewriteAndc(); break;
                case Opcode.vcfsx: RewriteVct("__vcfsx", PrimitiveType.Real32); break;
                case Opcode.vcfpsxws128: RewriteVcfpsxws("__vcfpsxws"); break;
                case Opcode.vcmpbfp:
                case Opcode.vcmpbfp128: RewriteVcmpfp("__vcmpebfp"); break;
                case Opcode.vcmpeqfp:
                case Opcode.vcmpeqfp128: RewriteVcmpfp("__vcmpeqfp"); break;
                case Opcode.vcmpgtfp:
                case Opcode.vcmpgtfp128: RewriteVcmpfp("__vcmpgtfp"); break;
                case Opcode.vcmpgtuw: RewriteVcmpu("__vcmpgtuw", PrimitiveType.UInt32); break;
                case Opcode.vcmpequb: RewriteVcmpu("__vcmpequb", PrimitiveType.UInt8); break;
                case Opcode.vcmpequd: RewriteVcmpu("__vcmpequd", PrimitiveType.UInt64); break;
                case Opcode.vcmpequw: RewriteVcmpu("__vcmpequw", PrimitiveType.UInt32); break;
                case Opcode.vcsxwfp128: RewriteVcsxwfp("__vcsxwfp"); break;
                case Opcode.vctsxs: RewriteVct("__vctsxs", PrimitiveType.Int32); break;
                case Opcode.vexptefp128: RewriteVectorUnary("__vexptefp"); break;
                case Opcode.vlogefp128: RewriteVectorUnary("__vlogefp"); break;
                case Opcode.vmaddfp: RewriteVmaddfp(); break;
                case Opcode.vmaddcfp128: RewriteVectorBinOp("__vmaddcfp", PrimitiveType.Real32); break;
                case Opcode.vmaxfp128: RewriteVectorBinOp("__vmaxfp", PrimitiveType.Real32); break;
                case Opcode.vminfp128: RewriteVectorBinOp("__vminfp", PrimitiveType.Real32); break;
                case Opcode.vmaxub: RewriteVectorBinOp("__vmaxub", PrimitiveType.UInt8); break;
                case Opcode.vmaxuh: RewriteVectorBinOp("__vmaxuh", PrimitiveType.UInt16); break;
                case Opcode.vmladduhm: RewriteVectorBinOp("__vmladduhm", PrimitiveType.UInt16); break;
                case Opcode.vmrghw:
                case Opcode.vmrghw128: RewriteVmrghw(); break;
                case Opcode.vmrglw:
                case Opcode.vmrglw128: RewriteVmrglw(); break;
                case Opcode.vmsub3fp128: RewriteVectorBinOp("__vmsub3fp", PrimitiveType.Real32); break;
                case Opcode.vmsub4fp128: RewriteVectorBinOp("__vmsub4fp", PrimitiveType.Real32); break;  //$REVIEW: is it correct?
                case Opcode.vmulfp128: RewriteVectorBinOp("__vmulfp", PrimitiveType.Real32); break;         //$REVIEW: is it correct?
                case Opcode.vnmsubfp: RewriteVnmsubfp(); break;
                case Opcode.vor:
                case Opcode.vor128: RewriteVor(); break;
                case Opcode.vperm:
                case Opcode.vperm128: RewriteVperm(); break;
                case Opcode.vpkd3d128: RewriterVpkD3d(); break;
                case Opcode.vrefp:
                case Opcode.vrefp128: RewriteVrefp(); break;
                case Opcode.vrfin128: RewriteVectorUnary("__vrfin"); break;
                case Opcode.vrfiz128: RewriteVectorUnary("__vrfiz"); break;
                case Opcode.vrlimi128: RewriteVrlimi(); break;
                case Opcode.vrsqrtefp: 
                case Opcode.vrsqrtefp128: RewriteVrsqrtefp(); break;
                case Opcode.vsel: RewriteVsel(); break;
                case Opcode.vsldoi: RewriteVsldoi(); break;
                case Opcode.vslw:
                case Opcode.vslw128: RewriteVsxw("__vslw"); break;
                case Opcode.vspltisw:
                case Opcode.vspltisw128: RewriteVspltisw(); break;
                case Opcode.vspltw:
                case Opcode.vspltw128: RewriteVspltw(); break;
                case Opcode.vsrw128: RewriteVsxw("__vsrw"); break;
                case Opcode.vsubfp:
                case Opcode.vsubfp128: RewriteVsubfp(); break;
                case Opcode.vupkd3d128: RewriteVupkd3d(); break;
                case Opcode.vxor:
                case Opcode.vxor128: RewriteXor(false); break;
                case Opcode.xor: RewriteXor(false); break;
                case Opcode.xori: RewriteXor(false); break;
                case Opcode.xoris: RewriteXoris(); break;
                }
                yield return new RtlInstructionCluster(addr, 4, this.rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        private Expression Shift16(MachineOperand machineOperand)
        {
            var imm = (ImmediateOperand)machineOperand;
            return Constant.Word32(imm.Value.ToInt32() << 16);
        }

        private Expression RewriteOperand(MachineOperand op, bool maybe0 = false)
        {
            switch (op)
            {
            case RegisterOperand rOp:
                if (maybe0 && rOp.Register.Number == 0)
                    return Constant.Zero(rOp.Register.DataType);
                if (arch.IsCcField(rOp.Register))
                {
                    return binder.EnsureFlagGroup(arch.GetCcFieldAsFlagGroup(rOp.Register));
                }
                else
                {
                    return binder.EnsureRegister(rOp.Register);
                }
            case ImmediateOperand iOp:
                // Sign-extend the bastard.
                return SignExtend(iOp.Value);
            case AddressOperand aOp:
                return aOp.Address;
            default:
                throw new NotImplementedException(
                    string.Format("RewriteOperand:{0} ({1}}}", op, op.GetType()));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if DEBUG
        private static HashSet<Opcode> seen = new HashSet<Opcode>();
        
        private void EmitUnitTest()
        {
            if (rdr == null || seen.Contains(dasm.Current.Mnemonic))
                return;
            seen.Add(dasm.Current.Mnemonic);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var uInstr = r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void PPCRw_{0}()", dasm.Current.Mnemonic);
            Debug.WriteLine("        {");
            Debug.WriteLine("            AssertCode(0x{0:X8},   // {1}", uInstr, dasm.Current);
            Debug.WriteLine("                \"0|L--|00100000({0}): 1 instructions\",", dasm.Current.Length);
            Debug.WriteLine("                \"1|L--|@@@\");");
            Debug.WriteLine("        }");
            Debug.WriteLine("");
        }
#else
        private void EmitUnitTest() { }
#endif

        private Expression EffectiveAddress(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            var reg = binder.EnsureRegister(mop.BaseRegister);
            var offset = mop.Offset;
            return emitter.IAdd(reg, offset);
        }

        private Expression EffectiveAddress_r0(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32((int) mop.Offset.ToInt16());
            }
            else
            {
                var reg = binder.EnsureRegister(mop.BaseRegister);
                var offset = mop.Offset;
                return emitter.IAdd(reg, offset);
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

        private Expression SignExtend(Constant value)
        {
            PrimitiveType iType = (PrimitiveType)value.DataType;
            if (arch.WordWidth.BitSize == 64)
            {
                return (iType.Domain == Domain.SignedInt)
                    ? Constant.Int64(value.ToInt64())
                    : Constant.Word64(value.ToUInt64());
            }
            else
            {
                return (iType.Domain == Domain.SignedInt)
                    ? Constant.Int32(value.ToInt32())
                    : Constant.Word32(value.ToUInt32());
            }
        }

        private Expression UpdatedRegister(Expression effectiveAddress)
        {
            var bin = (BinaryExpression) effectiveAddress;
            return bin.Left;
        }
    }
}
