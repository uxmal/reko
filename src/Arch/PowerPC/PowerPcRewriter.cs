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
        private IStorageBinder binder;
        private RtlEmitter m;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private PowerPcArchitecture arch;
        private IEnumerator<PowerPcInstruction> dasm;
        private IRewriterHost host;
        private PowerPcInstruction instr;
        private EndianImageReader rdr;

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
            //this.state = ppcState;
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
                this.rtlc = RtlClass.Linear;
                this.m = new RtlEmitter(rtlInstructions);
                switch (dasm.Current.Opcode)
                {
                default:
                    host.Error(
                        instr.Address, 
                        string.Format("PowerPC instruction '{0}' is not supported yet.", instr));
                    EmitUnitTest();
                    goto case Opcode.illegal;
                case Opcode.illegal: rtlc = RtlClass.Invalid; m.Invalid(); break;
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
                case Opcode.bdz: RewriteCtrBranch(false, false, m.Eq, false); break;
                case Opcode.bdzf: RewriteCtrBranch(false, false, m.Eq, false); break;
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
                case Opcode.fcmpu: RewriteFcmpu(); break;
                case Opcode.fdiv: RewriteFdiv(); break;
                case Opcode.fdivs: RewriteFdiv(); break;
                case Opcode.fmr: RewriteFmr(); break;
                case Opcode.fmadd: RewriteFmadd(); break;
                case Opcode.fmadds: RewriteFmadd(); break;
                case Opcode.fmsub: RewriteFmsub(); break;
                case Opcode.fmsubs: RewriteFmsub(); break;
                case Opcode.fnmsub: RewriteFnmsub(PrimitiveType.Real64); break;
                case Opcode.fnmsubs: RewriteFnmsub(PrimitiveType.Real32); break;
                case Opcode.fmul: RewriteFmul(); break;
                case Opcode.fmuls: RewriteFmul(); break;
                case Opcode.fneg: RewriteFneg(); break;
                case Opcode.frsp: RewriteFrsp(); break;
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
                case Opcode.tw: RewriteTw(); break;
                case Opcode.twi: RewriteTw(); break;
                case Opcode.vaddfp: RewriteVaddfp(); break;
                case Opcode.vaddubm: RewriteVectorBinOp("__vaddubm", PrimitiveType.UInt8); break;
                case Opcode.vaddubs: RewriteVectorBinOp("__vaddubs", PrimitiveType.UInt8); break;
                case Opcode.vadduwm: RewriteVectorBinOp("__vadduwm", PrimitiveType.UInt32); break;
                case Opcode.vadduqm: RewriteAdd(); break;
                case Opcode.vand: RewriteAnd(false); break;
                case Opcode.vandc: RewriteAndc(); break;
                case Opcode.vcfsx: RewriteVct("__vcfsx", PrimitiveType.Real32); break;
                case Opcode.vcmpgtfp: RewriteVcmpfp("__vcmpgtfp"); break;
                case Opcode.vcmpgtuw: RewriteVcmpu("__vcmpgtuw", PrimitiveType.UInt32); break;
                case Opcode.vcmpeqfp: RewriteVcmpfp("__vcmpeqfp"); break;
                case Opcode.vcmpequb: RewriteVcmpu("__vcmpequb", PrimitiveType.UInt8); break;
                case Opcode.vcmpequd: RewriteVcmpu("__vcmpequd", PrimitiveType.UInt64); break;
                case Opcode.vcmpequw: RewriteVcmpu("__vcmpequw", PrimitiveType.UInt32); break;
                case Opcode.vctsxs: RewriteVct("__vctsxs", PrimitiveType.Int32); break;
                case Opcode.vmaddfp: RewriteVmaddfp(); break;
                case Opcode.vmaxub: RewriteVectorBinOp("__vmaxub", PrimitiveType.UInt8); break;
                case Opcode.vmaxuh: RewriteVectorBinOp("__vmaxuh", PrimitiveType.UInt16); break;
                case Opcode.vmladduhm: RewriteVectorBinOp("__vmladduhm", PrimitiveType.UInt16); break;
                case Opcode.vmrghw:
                case Opcode.vmrghw128: RewriteVmrghw(); break;
                case Opcode.vmrglw:
                case Opcode.vmrglw128: RewriteVmrglw(); break;
                case Opcode.vmsub4fp128: RewriteVectorBinOp("__vmsub4fp128", PrimitiveType.Real32); break;  //$REVIEW: is it correct?
                case Opcode.vmulfp128: RewriteVectorBinOp("__vmulfp", PrimitiveType.Real32); break;         //$REVIEW: is it correct?
                case Opcode.vnmsubfp: RewriteVnmsubfp(); break;
                case Opcode.vor:
                case Opcode.vor128: RewriteVor(); break;
                case Opcode.vperm: RewriteVperm(); break;
                case Opcode.vrefp: RewriteVrefp(); break;
                case Opcode.vrlimi128: RewriteVrlimi(); break;
                case Opcode.vrsqrtefp: RewriteVrsqrtefp(); break;
                case Opcode.vsel: RewriteVsel(); break;
                case Opcode.vsldoi: RewriteVsldoi(); break;
                case Opcode.vslw:
                case Opcode.vslw128: RewriteVslw(); break;
                case Opcode.vspltisw:
                case Opcode.vspltisw128: RewriteVspltisw(); break;
                case Opcode.vspltw:
                case Opcode.vspltw128: RewriteVspltw(); break;
                case Opcode.vsubfp: RewriteVsubfp(); break;
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
            if (rdr == null || seen.Contains(dasm.Current.Opcode))
                return;
            seen.Add(dasm.Current.Opcode);

            var r2 = rdr.Clone();
            r2.Offset -= dasm.Current.Length;
            var uInstr = r2.ReadUInt32();
            Debug.WriteLine("        [Test]");
            Debug.WriteLine("        public void PPCRw_{0}()", dasm.Current.Opcode);
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
