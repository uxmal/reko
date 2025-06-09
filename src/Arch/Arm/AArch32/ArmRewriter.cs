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
using System.Linq;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter : IEnumerable<RtlInstructionCluster>
    {
        protected readonly IProcessorArchitecture arch;
        protected readonly EndianImageReader rdr;
        private readonly IRewriterHost host;
        private readonly IStorageBinder binder;
        private readonly IEnumerator<AArch32Instruction> dasm;
        private readonly List<RtlInstruction> rtls;
        protected readonly RtlEmitter m;
        protected int pcValueOffset;        // The offset to add to the current instruction's address when reading the PC register. 
        protected AArch32Instruction instr;
        protected InstrClass iclass;

        public ArmRewriter(Arm32Architecture arch, EndianImageReader rdr, IRewriterHost host, IStorageBinder binder) : this(arch, rdr, host, binder, new A32Disassembler(arch, rdr).GetEnumerator())
        {
        }

        protected ArmRewriter(IProcessorArchitecture arch, EndianImageReader rdr, IRewriterHost host, IStorageBinder binder, IEnumerator<AArch32Instruction> dasm)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.host = host;
            this.binder = binder;
            this.dasm = dasm;
            this.pcValueOffset = 8;
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = null!;
        }

        protected virtual void PostRewrite() { }
        protected virtual void RewriteIt() { }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addrInstr = instr.Address;
                this.iclass = instr.InstructionClass;
                // Most instructions have a conditional mode of operation.
                //$TODO: make sure non-conditional instructions are handled correctly here.
                ConditionalSkip(false);
                switch (instr.Mnemonic)
                {
                default:
                case Mnemonic.aesd:
                case Mnemonic.aese:
                case Mnemonic.aesimc:
                case Mnemonic.aesmc:
                case Mnemonic.dbg:
                case Mnemonic.fldmdbx:
                case Mnemonic.fldmiax:
                case Mnemonic.fstmdbx:
                case Mnemonic.fstmiax:
                case Mnemonic.mcrr2:
                case Mnemonic.mrrc2:
                case Mnemonic.sha1c:
                case Mnemonic.sha1h:
                case Mnemonic.sha1m:
                case Mnemonic.sha1p:
                case Mnemonic.sha1su0:
                case Mnemonic.sha1su1:
                case Mnemonic.sha256h:
                case Mnemonic.sha256h2:
                case Mnemonic.sha256su0:
                case Mnemonic.sha256su1:
                case Mnemonic.smmulr:
                case Mnemonic.smuadx:
                case Mnemonic.smusdx:
                case Mnemonic.vcls:
                case Mnemonic.vclz:
                case Mnemonic.vcnt:
                case Mnemonic.vcvta:
                case Mnemonic.vcvtb:
                case Mnemonic.vcvtn:
                case Mnemonic.vcvtp:
                case Mnemonic.vcvtt:
                case Mnemonic.vldmdb:
                case Mnemonic.vmovl:
                case Mnemonic.vmovn:
                case Mnemonic.vmsr:
                case Mnemonic.vorn:
                case Mnemonic.vpaddl:
                case Mnemonic.vqdmlal:
                case Mnemonic.vqdmlsl:
                case Mnemonic.vqdmull:
                case Mnemonic.vqmovun:
                case Mnemonic.vqmovn:
                case Mnemonic.vqneg:
                case Mnemonic.vqrdmlah:
                case Mnemonic.vqrshrun:
                case Mnemonic.vrecpe:
                case Mnemonic.vrinta:
                case Mnemonic.vrintn:
                case Mnemonic.vrintp:
                case Mnemonic.vrintr:
                case Mnemonic.vrintx:
                case Mnemonic.vrintz:
                case Mnemonic.vrshrn:
                case Mnemonic.vshrn:
                case Mnemonic.vswp:
                case Mnemonic.vtbx:
                case Mnemonic.vtrn:
                case Mnemonic.vuzp:
                case Mnemonic.vzip:
                case Mnemonic.dcps1:
                case Mnemonic.dcps2:
                case Mnemonic.dcps3:
                case Mnemonic.lsrs:
                case Mnemonic.subs:
                case Mnemonic.movs:
                case Mnemonic.sevl:
                    NotImplementedYet();
                    break;
                case Mnemonic.Invalid:
                    Invalid();
                    break;
                case Mnemonic.adc: RewriteAdcSbc(m.IAdd, false); break;
                case Mnemonic.add: RewriteBinOp(m.IAdd); break;
                case Mnemonic.addw: RewriteAddw(); break;
                case Mnemonic.adr: RewriteAdr(); break;
                case Mnemonic.and: RewriteLogical(m.And); break;
                case Mnemonic.asr: RewriteShift(m.Sar); break;
                case Mnemonic.asrs: RewriteShift(m.Sar); break;
                case Mnemonic.b: RewriteB(false); break;
                case Mnemonic.bfc: RewriteBfc(); break;
                case Mnemonic.bfi: RewriteBfi(); break;
                case Mnemonic.bic: RewriteLogical(Bic); break;
                case Mnemonic.bkpt: RewriteBkpt(); break;
                case Mnemonic.bl: RewriteB(true); break;
                case Mnemonic.blx: RewriteB(true); break;
                case Mnemonic.bx: RewriteB(false); break;
                case Mnemonic.bxj: RewriteB(false); break; //$REVIEW: starting with ARMv8, behaves exactly as BX
                case Mnemonic.cbz: RewriteCbnz(m.Eq0); break;
                case Mnemonic.cbnz: RewriteCbnz(m.Ne0); break;
                case Mnemonic.cdp: RewriteCdp(cdp_intrinsic); break;
                case Mnemonic.cdp2: RewriteCdp(cdp2_intrinsic); break;
                case Mnemonic.clrex: RewriteClrex(); break;
                case Mnemonic.clz: RewriteClz(); break;
                case Mnemonic.cmn: RewriteCmp(m.IAdd); break;
                case Mnemonic.cmp: RewriteCmp(m.ISub); break;
                case Mnemonic.cps: RewriteCps(cps_intrinsic); break;
                case Mnemonic.cpsid: RewriteCps(cps_id_intrinsic); break;
                case Mnemonic.crc32b: RewriteCrc(crc32_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.crc32h: RewriteCrc(crc32_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.crc32w: RewriteCrc(crc32_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.crc32cb: RewriteCrc(crc32c_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.crc32ch: RewriteCrc(crc32c_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.crc32cw: RewriteCrc(crc32c_intrinsic, PrimitiveType.UInt32); break;
                case Mnemonic.dsb: RewriteDsb(); break;
                case Mnemonic.dmb: RewriteDmb(); break;
                case Mnemonic.eor: RewriteLogical(m.Xor); break;
                case Mnemonic.eret: RewriteEret(); break;
                case Mnemonic.hint: RewriteHint(); break;
                case Mnemonic.hlt: RewriteHlt(); break;
                case Mnemonic.hvc: RewriteHvc(); break;
                case Mnemonic.isb: RewriteIsb(); break;
                case Mnemonic.it: RewriteIt(); break;
                case Mnemonic.lda: RewriteLoadAcquire(lda_sig, PrimitiveType.Word32); break;
                case Mnemonic.ldab: RewriteLoadAcquire(lda_sig, PrimitiveType.Byte); break;
                case Mnemonic.ldaex: RewriteLoadAcquire(ldaex_sig, PrimitiveType.Word32); break;
                case Mnemonic.ldaexb: RewriteLoadAcquire(ldaex_sig, PrimitiveType.Byte); break;
                case Mnemonic.ldaexd: RewriteLoadAcquireDouble(ldaex_sig); break;
                case Mnemonic.ldaexh: RewriteLoadAcquire(ldaex_sig, PrimitiveType.Word16); break;
                case Mnemonic.ldah: RewriteLoadAcquire(lda_sig, PrimitiveType.Word16); break;
                case Mnemonic.ldc: RewriteLdc(ldc_intrinsic); break;
                case Mnemonic.ldc2: RewriteLdc(ldc2_intrinsic); break;
                case Mnemonic.ldc2l: RewriteLdc(ldc2l_intrinsic); break;
                case Mnemonic.ldcl: RewriteLdc(ldcl_intrinsic); break;
                case Mnemonic.ldm: RewriteLdm(0, m.IAdd); break;
                case Mnemonic.ldmda: RewriteLdm(0, m.ISub); break;
                case Mnemonic.ldmdb: RewriteLdm(-4, m.ISub); break;
                case Mnemonic.ldmib: RewriteLdm(4, m.IAdd); break;
                case Mnemonic.ldr: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Mnemonic.ldrt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Mnemonic.ldrb: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Byte); break;
                case Mnemonic.ldrbt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Byte); break;
                case Mnemonic.ldrh: RewriteLdr(PrimitiveType.Word32, PrimitiveType.UInt16); break;
                case Mnemonic.ldrht: RewriteLdr(PrimitiveType.Word32, PrimitiveType.UInt16); break;
                case Mnemonic.ldrsb: RewriteLdr(PrimitiveType.Word32, PrimitiveType.SByte); break;
                case Mnemonic.ldrsbt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.SByte); break;
                case Mnemonic.ldrsh: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Int16); break;
                case Mnemonic.ldrsht: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Int16); break;
                case Mnemonic.ldrd: RewriteLdrd(); break;
                case Mnemonic.ldrex: RewriteLdrex(PrimitiveType.Word32); break;
                case Mnemonic.ldrexb: RewriteLdrex(PrimitiveType.Byte); break;
                case Mnemonic.ldrexd: RewriteLdrexd(); break;
                case Mnemonic.ldrexh: RewriteLdrex(PrimitiveType.Word16); break;
                case Mnemonic.lsl: case Mnemonic.lsls: RewriteShift(m.Shl); break;
                case Mnemonic.lsr: RewriteShift(m.Shr); break;
                case Mnemonic.nop: m.Nop(); break;
                case Mnemonic.mcr: RewriteMcr(mcr_intrinsic); break;
                case Mnemonic.mcr2: RewriteMcr(mcr2_intrinsic); break;
                case Mnemonic.mcrr: RewriteMcrr(); break;
                case Mnemonic.mla: RewriteMultiplyAccumulate(m.IAdd); break;
                case Mnemonic.mls: RewriteMultiplyAccumulate(m.ISub); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movt: RewriteMovt(); break;
                case Mnemonic.movw: RewriteMovw(); break;
                case Mnemonic.mrc: RewriteMrc(mrc_intrinsic); break;
                case Mnemonic.mrc2: RewriteMrc(mrc2_intrinsic); break;
                case Mnemonic.mrrc: RewriteMrrc(); break;
                case Mnemonic.mrs: RewriteMrs(); break;
                case Mnemonic.msr: RewriteMsr(); break;
                case Mnemonic.mul: RewriteBinOp(m.IMul); break;
                case Mnemonic.mvn: RewriteUnaryOp(m.Comp); break;
                case Mnemonic.orn: RewriteLogical((a, b) => m.Or(a, m.Comp(b))); break;
                case Mnemonic.orr: RewriteLogical(m.Or); break;
                case Mnemonic.pkhbt: RewritePk(pkhbt_intrinsic); break;
                case Mnemonic.pkhtb: RewritePk(pkhtb_intrinsic); break;
                case Mnemonic.pld: RewritePld(pld_intrinsic); break;
                case Mnemonic.pldw: RewritePld(pldw_intrinsic); break;
                case Mnemonic.pli: RewritePld(pli_intrinsic); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break; 
                case Mnemonic.qadd: RewriteQAddSub(CommonOps.SatAdd); break;
                case Mnemonic.qadd16: RewriteVectorBinOp(qadd_intrinsic, ArmVectorData.S16); break;
                case Mnemonic.qadd8: RewriteVectorBinOp(qadd_intrinsic, ArmVectorData.S8); break;
                case Mnemonic.qdadd: RewriteQDAddSub(CommonOps.SatAdd); break;
                case Mnemonic.qasx: RewriteVectorBinOp(qasx_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.qdsub: RewriteQDAddSub(CommonOps.SatSub); break;
                case Mnemonic.qsax: RewriteVectorBinOp(qsax_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.qsub: RewriteQAddSub(CommonOps.SatSub); break;
                case Mnemonic.qsub16: RewriteVectorBinOp(qsub_intrinsic, ArmVectorData.S16); break;
                case Mnemonic.qsub8: RewriteVectorBinOp(qsub_intrinsic, ArmVectorData.S8); break;
                case Mnemonic.rbit: RewriteIntrinsic(rbit_intrinsic); break;
                case Mnemonic.rfeda: RewriteRfe(rfeda_intrinsic); break;
                case Mnemonic.rfedb: RewriteRfe(rfedb_intrinsic); break;
                case Mnemonic.rfeia: RewriteRfe(rfeia_intrinsic); break;
                case Mnemonic.rfeib: RewriteRfe(rfeib_intrinsic); break;
                case Mnemonic.ror: RewriteShift(Ror); break;
                case Mnemonic.rrx: RewriteShift(Rrx); break;
                case Mnemonic.rev: RewriteRev(PrimitiveType.Byte); break;
                case Mnemonic.rev16: RewriteRev(PrimitiveType.Word16); break;
                case Mnemonic.revsh: RewriteRevsh(); break;
                case Mnemonic.rsb: RewriteRevBinOp(m.ISub, instr.SetFlags); break;
                case Mnemonic.rsc: RewriteAdcSbc(m.ISub, true); break;
                case Mnemonic.sadd16: RewriteVectorBinOp(sadd_intrinsic, ArmVectorData.S16); break;
                case Mnemonic.sadd8: RewriteVectorBinOp(sadd_intrinsic, ArmVectorData.S8); break;
                case Mnemonic.sasx: RewriteVectorBinOp(sasx_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub, false); break;
                case Mnemonic.sbfx: RewriteSbfx(); break;
                case Mnemonic.sdiv: RewriteDiv(m.SDiv); break;
                case Mnemonic.sel: RewriteIntrinsic(sel_intrinsic); break;
                case Mnemonic.setend: RewriteSetend(); break;
                case Mnemonic.setpan: RewriteIntrinsicSideeffect(setpan_intrinsic); break;
                case Mnemonic.sev: RewriteIntrinsicSideeffect(sev_intrinsic); break;
                case Mnemonic.shasx: RewriteHasx(PrimitiveType.Int16); break;
                case Mnemonic.shadd16: RewriteVectorBinOp(hadd_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.shadd8: RewriteVectorBinOp(hadd_intrinsic, PrimitiveType.Int8); break;
                case Mnemonic.shsax: RewriteShsax(); break;
                case Mnemonic.shsub16: RewriteVectorBinOp(hsub_intrinsic, PrimitiveType.Int16); break;
                case Mnemonic.shsub8: RewriteVectorBinOp(hsub_intrinsic, PrimitiveType.Int8); break;
                case Mnemonic.smc: RewriteSmc(); break;
                case Mnemonic.smlabb: RewriteMla(false, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlabt: RewriteMla(false, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlad: RewriteMxd(false, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Mnemonic.smladx: RewriteMxd(true, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Mnemonic.smlalbb: RewriteMlal(false, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlalbt: RewriteMlal(false, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlald: RewriteMlxd(false, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Mnemonic.smlaldx: RewriteMlxd(true, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Mnemonic.smlaltb: RewriteMlal(true, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlaltt: RewriteMlal(true, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlal: RewriteSmlal(); break;
                case Mnemonic.smlatb: RewriteMla(true, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlatt: RewriteMla(true, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smlawb: RewriteSmlaw(false); break;
                case Mnemonic.smlawt: RewriteSmlaw(true); break;
                case Mnemonic.smlsd: RewriteMxd(false, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Mnemonic.smlsdx: RewriteMxd(true, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Mnemonic.smlsld: RewriteMlxd(false, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Mnemonic.smlsldx: RewriteMlxd(true, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Mnemonic.smmla: RewriteSmml(m.IAdd); break;
                case Mnemonic.smmlar: RewriteSmmlr(m.IAdd); break;
                case Mnemonic.smmls: RewriteSmml(m.ISub); break;
                case Mnemonic.smmlsr: RewriteSmmlr(m.ISub); break;
                case Mnemonic.smmul: RewriteSmmul(); break;
                case Mnemonic.smuad: RewriteMxd(false, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Mnemonic.smulbb: RewriteMulbb(false, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smulbt: RewriteMulbb(false, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smulwb: RewriteMulw(false); break;
                case Mnemonic.smulwt: RewriteMulw(true); break;
                case Mnemonic.smultb: RewriteMulbb(true, false, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smultt: RewriteMulbb(true, true, PrimitiveType.Int16, m.SMul); break;
                case Mnemonic.smull: RewriteMull(PrimitiveType.Int64, m.SMul); break;
                case Mnemonic.smusd: RewriteSmusd(); break;
                case Mnemonic.srsda: RewriteIntrinsicSideeffect(srsda_intrinsic); break;
                case Mnemonic.srsdb: RewriteIntrinsicSideeffect(srsdb_intrinsic); break;
                case Mnemonic.srsia: RewriteIntrinsicSideeffect(srsia_intrinsic); break;
                case Mnemonic.srsib: RewriteIntrinsicSideeffect(srsib_intrinsic); break;
                case Mnemonic.ssat: RewriteSsat(); break;
                case Mnemonic.ssax: RewriteSax(PrimitiveType.Int16); break;
                case Mnemonic.ssat16: RewriteSat16(ssat16_intrinsic); break;
                case Mnemonic.ssub16: RewriteVectorBinOp(ssub_intrinsic, ArmVectorData.S16); break;
                case Mnemonic.ssub8: RewriteVectorBinOp(ssub_intrinsic, ArmVectorData.S8); break;
                case Mnemonic.stc: RewriteStc(stc_intrinsic); break;
                case Mnemonic.stc2l: RewriteStc(stc2l_intrinsic); break;
                case Mnemonic.stc2: RewriteStc(stc2_intrinsic); break;
                case Mnemonic.stcl: RewriteStc(stcl_intrinsic); break;
                case Mnemonic.stl: RewriteStoreRelease(stl_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.stlb: RewriteStoreRelease(stl_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.stlh: RewriteStoreRelease(stl_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.stlex: RewriteStoreExclusive(stlex_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.stlexb: RewriteStoreExclusive(stlex_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.stlexh: RewriteStoreExclusive(stlex_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.stlexd: RewriteStoreReleaseDoubleExclusive(stlex_intrinsic); break;
                case Mnemonic.stm: RewriteStm(true, true); break;
                case Mnemonic.stmdb: RewriteStm(false, false); break;
                case Mnemonic.stmda: RewriteStm(false, true); break;
                case Mnemonic.stmib: RewriteStm(true, false); break;
                case Mnemonic.str: RewriteStr(PrimitiveType.Word32); break;
                case Mnemonic.strb: RewriteStr(PrimitiveType.Byte); break;
                case Mnemonic.strbt: RewriteStr(PrimitiveType.Byte); break;
                case Mnemonic.strd: RewriteStrd(); break;
                case Mnemonic.strex: RewriteStoreExclusive(strex_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.strexb: RewriteStoreExclusive(strex_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.strexd: RewriteStoreReleaseDoubleExclusive(strex_intrinsic); break;
                case Mnemonic.strexh: RewriteStoreExclusive(strex_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.strh: RewriteStr(PrimitiveType.UInt16); break;
                case Mnemonic.strht: RewriteStr(PrimitiveType.UInt16); break;
                case Mnemonic.strt: RewriteStr(PrimitiveType.Word32); break;
                case Mnemonic.sub: RewriteBinOp(m.ISub); break;
                case Mnemonic.subw: RewriteSubw(); break;
                case Mnemonic.svc: RewriteSvc(); break;
                case Mnemonic.swp: RewriteSwp(PrimitiveType.Word32); break;
                case Mnemonic.swpb: RewriteSwp(PrimitiveType.Byte); break;
                case Mnemonic.sxtab: RewriteXtab(PrimitiveType.SByte); break;
                case Mnemonic.sxtab16: RewriteXtab16(sxtab16_intrinsic, sxtab16_ror_intrinsic); break;
                case Mnemonic.sxtah: RewriteXtab(PrimitiveType.Int16); break;
                case Mnemonic.sxtb: RewriteXtb(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.sxtb16: RewriteXtb16(sxtb16_intrinsic, sxtb16_ror_intrinsic); break;
                case Mnemonic.sxth: RewriteXtb(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.tbb: RewriteTableBranch(PrimitiveType.Byte); break;
                case Mnemonic.tbh: RewriteTableBranch(PrimitiveType.Word16); break;
                case Mnemonic.teq: RewriteTeq(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.uadd16: RewriteVectorBinOp(uadd_intrinsic, ArmVectorData.I16); break;
                case Mnemonic.uadd8: RewriteVectorBinOp(uadd_intrinsic, ArmVectorData.I8); break;
                case Mnemonic.uasx: RewriteUasx(); break;
                case Mnemonic.ubfx: RewriteUbfx(); break;
                case Mnemonic.udf: RewriteUdf(); break;
                case Mnemonic.udiv: RewriteDiv(m.UDiv); break;
                case Mnemonic.uhadd16: RewriteVectorBinOp(hadd_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.uhadd8: RewriteVectorBinOp(hadd_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.uhasx: RewriteHasx(PrimitiveType.UInt16); break;
                case Mnemonic.uhsax: RewriteUhsax(); break;
                case Mnemonic.uhsub16: RewriteVectorBinOp(hsub_intrinsic, PrimitiveType.UInt16); break;
                case Mnemonic.uhsub8: RewriteVectorBinOp(hsub_intrinsic, PrimitiveType.UInt8); break;
                case Mnemonic.umaal: RewriteUmaal(); break;
                case Mnemonic.umlal: RewriteUmlal(); break;
                case Mnemonic.umull: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Mnemonic.uqadd16: RewriteVectorBinOp(uqadd_intrinsic, ArmVectorData.U16); break;
                case Mnemonic.uqadd8: RewriteVectorBinOp(uqadd_intrinsic, ArmVectorData.U8); break;
                case Mnemonic.uqasx: RewriteVectorBinOp(uqasx_intrinsic, ArmVectorData.U16); break;
                case Mnemonic.uqsax: RewriteVectorBinOp(uqsax_intrinsic, ArmVectorData.U16); break;
                case Mnemonic.uqsub16: RewriteVectorBinOp(uqsub_intrinsic, ArmVectorData.U16); break;
                case Mnemonic.uqsub8: RewriteVectorBinOp(uqsub_intrinsic, ArmVectorData.U8); break;
                case Mnemonic.usad8: RewriteUsad8(); break;
                case Mnemonic.usada8: RewriteUsada8(); break;
                case Mnemonic.usat: RewriteUsat(); break;
                case Mnemonic.usat16: RewriteSat16(usat16_intrinsic); break;
                case Mnemonic.usax: RewriteSax(PrimitiveType.UInt16); break;
                case Mnemonic.usub16: RewriteVectorBinOp(usub_intrinsic, ArmVectorData.I16); break;
                case Mnemonic.usub8: RewriteVectorBinOp(usub_intrinsic, ArmVectorData.I8); break;
                case Mnemonic.uxtab: RewriteXtab(PrimitiveType.Byte); break;
                case Mnemonic.uxtab16: RewriteXtab16(uxtab16_intrinsic, uxtab16_ror_intrinsic); break;
                case Mnemonic.uxtah: RewriteXtab(PrimitiveType.UInt16); break;
                case Mnemonic.uxtb: RewriteXtb(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.uxtb16: RewriteXtb16(uxtb16_initrinsic, uxtb16_ror_intrinsic); break;
                case Mnemonic.uxth: RewriteXtb(PrimitiveType.UInt16, PrimitiveType.UInt32); break;
                case Mnemonic.wfe: RewriteIntrinsicSideeffect(wfe_intrinsic); break;
                case Mnemonic.wfi: RewriteWfi(); break;
                case Mnemonic.yield: RewriteYield(); break;

                case Mnemonic.vabs: RewriteVectorUnaryOp(Simd.Abs); break;
                case Mnemonic.vaba: RewriteVectorBinOp(vaba_intrinsic); break;
                case Mnemonic.vabal: RewriteVectorBinOpWiden(vabal_intrinsic); break;
                case Mnemonic.vabd: RewriteVectorBinOp(vabd_intrinsic); break;
                case Mnemonic.vabdl: RewriteVectorBinOpWiden(vabdl_intrinsic); break;
                case Mnemonic.vacge: RewriteVectorBinOp(vabs_cge_intrinsic); break;
                case Mnemonic.vacgt: RewriteVectorBinOp(vabs_cgt_intrinsic); break;
                case Mnemonic.vadd: RewriteVectorBinOp(vadd_intrinsic); break;
                case Mnemonic.vaddhn: RewriteVectorBinOpNarrow(vaddhn_intrinsic); break;
                case Mnemonic.vaddl: RewriteVectorBinOpWiden(vaddl_intrinsic); break;
                case Mnemonic.vaddw: RewriteVectorBinOp(vaddw_intrinsic); break;
                case Mnemonic.vand: RewriteVecBinOp(m.And); break;
                case Mnemonic.vbic: RewriteVecBinOp(Bic); break;
                case Mnemonic.vbsl: RewriteVbsl(); break;
                case Mnemonic.vcmla: RewriteVectorBinOp(vcmla_intrinsic); break;
                case Mnemonic.vcmp: RewriteVcmp(); break;
                case Mnemonic.vbif: RewriteIntrinsicBinOp(vbif_intrinsic); break;
                case Mnemonic.vbit: RewriteIntrinsicBinOp(vbit_intrinsic); break;
                case Mnemonic.vceq: RewriteVectorBinOp(vceq_intrinsic); break;
                case Mnemonic.vcge: RewriteVectorBinOp(vcge_intrinsic); break;
                case Mnemonic.vcgt: RewriteVectorBinOp(vcgt_intrinsic); break;
                case Mnemonic.vcle: RewriteVectorBinOp(vcle_intrinsic); break;
                case Mnemonic.vclt: RewriteVectorBinOp(vclt_intrinsic); break;
                case Mnemonic.vcmpe: RewriteVcmp(); break;
                case Mnemonic.vcvt: RewriteVcvt(); break;
                case Mnemonic.vcvtm: RewriteVcvtToInteger(FpOps.floorf, FpOps.floor); break;
                case Mnemonic.vcvtr: RewriteVcvtToInteger(FpOps.rintf, FpOps.rint); break;
                case Mnemonic.vdiv: RewriteVecBinOp(m.FDiv); break;
                case Mnemonic.vdup: RewriteVdup(); break;
                case Mnemonic.veor: RewriteVecBinOp(m.Xor); break;
                case Mnemonic.vext: RewriteVext(); break;
                case Mnemonic.vfma: RewriteVfmas("__vfma", m.FAdd); break;
                case Mnemonic.vfmal: RewriteVectorBinOp(vfmal_intrinsic); break;
                case Mnemonic.vfms: RewriteVfmas("__vfms", m.FSub); break;
                case Mnemonic.vfnma: RewriteVectorBinOp(vfnma_intrinsic); break;
                case Mnemonic.vfnms: RewriteVectorBinOp(vfnms_intrinsic); break;
                case Mnemonic.vhadd: RewriteVectorBinOp(vhadd_intrinsic); break;
                case Mnemonic.vhsub: RewriteVectorBinOp(vhsub_intrinsic); break;
                case Mnemonic.vld1: RewriteVldN(vld1_multi_intrinsic); break; ;
                case Mnemonic.vld2: RewriteVldN(vld2_multi_intrinsic); break;
                case Mnemonic.vld3: RewriteVldN(vld3_multi_intrinsic); break;
                case Mnemonic.vld4: RewriteVldN(vld4_multi_intrinsic); break;
                case Mnemonic.vldmia: RewriteVldmia(); break;
                case Mnemonic.vldr: RewriteVldr(); break;
                case Mnemonic.vmax: RewriteVectorBinOp(vmax_intrinsic); break;
                case Mnemonic.vmaxnm: RewriteFloatingPointBinary(vmaxnm_intrinsic); break;
                case Mnemonic.vmin: RewriteVectorBinOp(vmin_intrinsic); break;
                case Mnemonic.vminnm: RewriteFloatingPointBinary(vminnm_intrinsic); break;
                case Mnemonic.vmov: RewriteVmov(); break;
                case Mnemonic.vmla: RewriteVectorBinOp(vmla_intrinsic); break;
                case Mnemonic.vmls: RewriteVectorBinOp(vmls_intrinsic); break;
                case Mnemonic.vmlal: RewriteVectorBinOp(vmlal_intrinsic); break;
                case Mnemonic.vmlsl: RewriteVectorBinOp(vmlsl_intrinsic); break;
                case Mnemonic.vmrs: RewriteVmrs(); break;
                case Mnemonic.vmul: RewriteVmul(); break;
                case Mnemonic.vmull: RewriteVmull(); break;
                case Mnemonic.vmvn: RewriteVmvn(); break;
                case Mnemonic.vorr: RewriteVecBinOp(m.Or); break;
                case Mnemonic.vneg: RewriteVectorUnaryOp(vneg_intrinsic); break;
                case Mnemonic.vnmla: RewriteVectorBinOp(vnmla_intrinsic); break;
                case Mnemonic.vnmls: RewriteVectorBinOp(vnmls_intrinsic); break;
                case Mnemonic.vnmul: RewriteVectorBinOp(vnmul_intrinsic); break;
                case Mnemonic.vpadal: RewriteVectorUnaryOpWiden(vpadal_intrinsic); break;
                case Mnemonic.vpadd: RewriteVectorBinOp(vpadd_intrinsic); break;
                case Mnemonic.vpmax: RewriteVectorBinOp(vpmax_intrinsic); break;
                case Mnemonic.vpmin: RewriteVectorBinOp(vpmin_intrinsic); break;
                case Mnemonic.vpop: RewriteVpop(); break;
                case Mnemonic.vpush: RewriteVpush(); break;
                case Mnemonic.vqabs: RewriteVectorBinOp(vqabs_intrinsic); break;
                case Mnemonic.vqadd: RewriteVectorBinOp(vqadd_intrinsic); break;
                case Mnemonic.vqdmulh: RewriteVectorBinOp(vqdmulh_intrinsic); break;
                case Mnemonic.vqrdmulh: RewriteVectorBinOp(vqrdmulh_intrinsic); break;
                case Mnemonic.vqrshl: RewriteVectorBinOp(vqrshl_intrinsic); break;
                case Mnemonic.vqrshrn: RewriteVectorBinOpNarrow(vqrshrn_intrinsic); break;
                case Mnemonic.vqsub: RewriteVectorBinOp(vqsub_intrinsic); break;
                case Mnemonic.vqshl: RewriteVectorBinOp(vqshl_intrinsic); break;
                case Mnemonic.vqshlu: RewriteVectorShift(vqshlu_intrinsic); break;
                case Mnemonic.vqshrn: RewriteVectorShift(vqshrn_intrinsic); break;
                case Mnemonic.vqshrun: RewriteVectorShift(vqshrun_intrinsic); break;
                case Mnemonic.vraddhn: RewriteVectorBinOpNarrow(vraddhn_intrinsic); break;
                case Mnemonic.vrecps: RewriteVectorUnaryOp(vrecps_intrinsic); break;
                case Mnemonic.vrev16: RewriteVectorUnaryOp(vrev16_intrinsic); break;
                case Mnemonic.vrev32: RewriteVectorUnaryOp(vrev32_intrinsic); break;
                case Mnemonic.vrev64: RewriteVectorUnaryOp(vrev64_intrinsic); break;
                case Mnemonic.vrhadd: RewriteVectorBinOp(vrhadd_intrinsic); break;
                case Mnemonic.vrintm: RewriteFloatingPointUnary(FpOps.floorf, FpOps.floor); break;
                case Mnemonic.vrshl: RewriteVectorBinOp(vrshl_intrinsic); break;
                case Mnemonic.vrshr: RewriteVectorBinOp(vrshr_intrinsic); break;
                case Mnemonic.vrsqrte: RewriteVectorBinOp(vrsqrte_intrinsic); break;
                case Mnemonic.vrsqrts: RewriteVectorBinOp(vrsqrts_intrinsic); break;
                case Mnemonic.vrsra: RewriteVectorBinOp(vrsra_intrinsic); break;
                case Mnemonic.vrsubhn: RewriteVectorBinOp(vrsubhn_intrinsic); break;
                case Mnemonic.vseleq: RewriteVectorBinOp(vseleq_intrinsic); break;
                case Mnemonic.vselge: RewriteVectorBinOp(vselge_intrinsic); break;
                case Mnemonic.vselgt: RewriteVectorBinOp(vselgt_intrinsic); break;
                case Mnemonic.vselvs: RewriteVectorBinOp(vselvs_intrinsic); break;
                case Mnemonic.vstm: RewriteVstmia(true, false); break;
                case Mnemonic.vstmdb: RewriteVstmia(false, true); break;
                case Mnemonic.vstmia: RewriteVstmia(true, true); break;
                case Mnemonic.vsubhn: RewriteVectorBinOpNarrow(vsubhn_intrinsic); break;
                case Mnemonic.vsqrt: RewriteFloatingPointUnary(FpOps.sqrtf, FpOps.sqrt); break;
                case Mnemonic.vshl: RewriteVectorBinOp(vshl_intrinsic); break;
                case Mnemonic.vshll: RewriteVectorBinOpWiden(vshll_intrinsic); break;
                case Mnemonic.vshr: RewriteVectorBinOp(vshr_intrinsic); break;
                case Mnemonic.vsli: RewriteVectorBinOp(vsli_intrinsic); break;
                case Mnemonic.vsra: RewriteVectorBinOp(vsra_intrinsic); break;
                case Mnemonic.vsri: RewriteVectorBinOp(vsri_intrinsic); break;
                case Mnemonic.vst1: RewriteVstN(vst1_multi_intrinsic); break;
                case Mnemonic.vst2: RewriteVstN(vst2_multi_intrinsic); break;
                case Mnemonic.vst3: RewriteVstN(vst3_multi_intrinsic); break;
                case Mnemonic.vst4: RewriteVstN(vst4_multi_intrinsic); break;
                case Mnemonic.vstr: RewriteVstr(); break;
                case Mnemonic.vsub: RewriteVectorBinOp(vsub_intrinsic); break;
                case Mnemonic.vsubl: RewriteVectorBinOp(vsubl_intrinsic); break;
                case Mnemonic.vsubw: RewriteVectorBinOp(vsubw_intrinsic); break;
                case Mnemonic.vtbl: RewriteVtbl(); break;
                case Mnemonic.vtst: RewriteVectorBinOp(vtst_intrinsic); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                this.rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Invalid()
        {
            this.iclass = InstrClass.Invalid;
            m.Invalid();
        }

        void NotImplementedYet()
        {
            uint wInstr;
            if (instr.Length == 4)
            {
                wInstr = rdr.PeekLeUInt32(-4);
            }
            else
            {
                wInstr = rdr.PeekLeUInt16(-2);
            }
            host.Error(instr.Address, "AArch32 instruction '{0}' ({1:X4}) is not supported yet.", instr, wInstr);
            EmitUnitTest(instr);
            Invalid();
        }

        Identifier Reg(RegisterStorage reg) { return binder.EnsureRegister(reg); }


        Expression NZC()
        {
            return binder.EnsureFlagGroup(Registers.NZC);
        }

        Expression NZCV()
        {
            return binder.EnsureFlagGroup(Registers.NZCV);
        }

        Expression C()
        {
            return binder.EnsureFlagGroup(Registers.C);
        }

        Expression Q()
        {
            return binder.EnsureFlagGroup(Registers.Q);
        }

        void MaybeUpdateFlags(Expression opDst)
        {
            if (instr.SetFlags)
            {
                m.Assign(NZCV(), m.Cond(opDst));
            }
        }

        void RewriteB(bool link)
        {
            Expression dst;
            bool dstIsAddress;
            if (instr.Operands[0] is Address aOp)
            {
                dst = aOp;
                dstIsAddress = true;
            }
            else
            {
                dst = Operand(0, PrimitiveType.Word32, true);
                dstIsAddress = false;
            }
            if (link)
            {
                if (instr.Condition == ArmCondition.AL)
                {
                    m.Call(dst, 0);
                }
                else
                {
                    iclass |= InstrClass.Call;
                    ConditionalSkip(true);
                    m.Call(dst, 0);
                }
            }
            else
            {
                if (instr.Condition == ArmCondition.AL)
                {
                    if (instr.Operands[0] is RegisterStorage rop && rop == Registers.lr)
                    {
                        //$TODO: cheating a little since
                        // one could consider lr being set explitly.
                        // however, this is very uncommon.
                        m.Return(0, 0);
                    }
                    else
                    {
                        m.Goto(dst);
                    }
                }
                else
                {
                    iclass = InstrClass.ConditionalTransfer;
                    if (dstIsAddress)
                    {
                        m.Branch(TestCond(instr.Condition)!, (Address) dst, iclass);
                    }
                    else
                    {
                        ConditionalSkip(true);
                        if (instr.Operands[0] is RegisterStorage rop && rop == Registers.lr)
                        {
                            //$TODO: cheating a little since
                            // one could consider lr being set explitly.
                            // however, this is very uncommon.
                            m.Return(0, 0);
                        }
                        else
                        {
                            m.Goto(dst);
                        }
                    }
                }
            }
        }

        void RewriteCbnz(Func<Expression, Expression> ctor)
        {
            iclass = InstrClass.ConditionalTransfer;
            m.Branch(ctor(Operand(0)),
                    (Address)instr.Operands[1],
                    InstrClass.ConditionalTransfer);
        }

        // If a conditional ARM instruction is encountered, generate an IL
        // instruction to skip the remainder of the instruction cluster.
        protected virtual void ConditionalSkip(bool force)
        {
            var cc = instr.Condition;
            if (!force)
            {
                if (cc == ArmCondition.AL)
                    return; // never skip!
                if (instr.Mnemonic == Mnemonic.b ||
                    instr.Mnemonic == Mnemonic.bl ||
                    instr.Mnemonic == Mnemonic.blx ||
                    instr.Mnemonic == Mnemonic.bx ||
                    instr.Mnemonic == Mnemonic.bxj)
                {
                    // These instructions handle the branching themselves.
                    return;
                }
            }
            m.BranchInMiddleOfInstruction(
                TestCond(Invert(cc))!,
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
        }

        Expression EffectiveAddress(MemoryOperand mem)
        {
            var baseReg = Reg(mem.BaseRegister!);
            Expression ea = baseReg;
            if (mem.Offset is not null)
            {
                if (mem.Add)
                {
                    ea = m.IAdd(ea, mem.Offset);
                }
                else
                {
                    ea = m.ISub(ea, mem.Offset);
                }
            }
            else if (mem.Index is not null)
            {
                ea = m.IAdd(ea, Reg(mem.Index));
            }
            return ea;
        }

        protected ArmCondition Invert(ArmCondition cc)
        {
            switch (cc)
            {
            case ArmCondition.EQ: return ArmCondition.NE;
            case ArmCondition.NE: return ArmCondition.EQ;
            case ArmCondition.HS: return ArmCondition.LO;
            case ArmCondition.LO: return ArmCondition.HS;
            case ArmCondition.MI: return ArmCondition.PL;
            case ArmCondition.PL: return ArmCondition.MI;
            case ArmCondition.VS: return ArmCondition.VC;
            case ArmCondition.VC: return ArmCondition.VS;
            case ArmCondition.HI: return ArmCondition.LS;
            case ArmCondition.LS: return ArmCondition.HI;
            case ArmCondition.GE: return ArmCondition.LT;
            case ArmCondition.LT: return ArmCondition.GE;
            case ArmCondition.GT: return ArmCondition.LE;
            case ArmCondition.LE: return ArmCondition.GT;
            case ArmCondition.AL: return ArmCondition.Invalid;
            }
            return ArmCondition.Invalid;
        }

        //bool IsLastOperand(const cs_arm_op & op)
        //{
        //    return &op == &instr->detail->arm.operands[instr->detail->arm.op_count - 1];
        //}

        Expression Operand(int iop, PrimitiveType? dt = null, bool write = false)
        {
            dt ??= PrimitiveType.Word32;
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage rOp:
                {
                    if (!write && rOp == Registers.pc)
                    {
                        return instr.Address + pcValueOffset;
                    }
                    var reg = Reg(rOp);
                    return MaybeShiftOperand(reg, iop);
                }
            case Constant iOp:
                return iOp;
            //case ARM_OP_CIMM:
            //    return m.Byte((uint8_t)op.imm);
            //case ARM_OP_PIMM:
            //    return host->EnsureRegister(2, op.imm);
            case MemoryOperand mop:
                {
                    var baseReg = Reg(mop.BaseRegister!);
                    Expression ea = baseReg;
                    if (mop.BaseRegister == Registers.pc)
                    {
                        // PC-relative address
                        ea = ComputePcRelativeOffset(mop);

                        if (mop.Index is not null)
                        {
                            var ireg = Reg(mop.Index);
                            //$REVIEW: does it even make sense 
                            // to translate rotates / right-shifts?
                            switch (mop.ShiftType)
                            {
                            case Mnemonic.Invalid:
                                ea = m.IAdd(ea, ireg);
                                break;
                            case Mnemonic.lsl:
                                ea = m.IAdd(ea, m.IMul(ireg, Constant.Int32(1 << mop.Shift)));
                                break;
                            case Mnemonic.lsr:
                                ea = m.IAdd(ea, m.Shr(ireg, Constant.Int32(mop.Shift)));
                                break;
                            case Mnemonic.asr:
                                ea = m.IAdd(ea, m.Sar(ireg, Constant.Int32(mop.Shift)));
                                break;
                            case Mnemonic.ror:
                                var ix = m.Fn(CommonOps.Ror, ireg, Constant.Int32(mop.Shift));
                                ea = m.IAdd(ea, ix);
                                break;
                            case Mnemonic.rrx:
                                var rrx = m.Fn(
                                    CommonOps.RorC.MakeInstance(ireg.DataType, PrimitiveType.Int32),
                                    ireg,
                                    Constant.Int32(mop.Shift),
                                    C());
                                ea = m.IAdd(ea, rrx);
                                break;
                            }
                        }
                        return m.Mem(dt, ea);
                    }
                    if (mop.PreIndex || !instr.Writeback)
                    {
                        if (mop.Offset is not null && !mop.Offset.IsZero)
                        {
                            var offset = mop.Offset;
                            ea = mop.Add
                                ? m.IAdd(ea, offset)
                                : m.ISub(ea, offset);
                        }
                        else if (mop.Index is not null)
                        {
                            Expression idx = Reg(mop.Index);
                            if (mop.ShiftType != Mnemonic.Invalid)
                            {
                                var sh = m.Int32(mop.Shift);
                                idx = MaybeShiftExpression(idx, sh, mop.ShiftType);
                            }
                            ea = mop.Add
                                ? m.IAdd(ea, idx)
                                : m.ISub(ea, idx);
                        }
                    }
                    if (mop.PreIndex && instr.Writeback)
                    {
                        m.Assign(baseReg, ea);
                        ea = baseReg;
                    }
                    return m.Mem(SizeFromLoadStore(), ea);
                }
            case Address addr:
                return addr;
            case IndexedOperand ixop:
                // Extract a single item from the vector register
                var ixreg = Reg(ixop.Register);
                return m.ARef(ixop.DataType, ixreg, Constant.Int32(ixop.Index));
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        public virtual Address ComputePcRelativeOffset(MemoryOperand mop)
        {
            var dst = instr.Address + 8u;
            if (mop.Offset is not null)
            {
                dst += mop.Offset.ToInt32();
            }
            return dst;
        }

        DataType SizeFromLoadStore()
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.ldc: return PrimitiveType.Word32;
            case Mnemonic.ldc2: return PrimitiveType.Word32;
            case Mnemonic.ldc2l: return PrimitiveType.Word32;
            case Mnemonic.ldcl: return PrimitiveType.Word32;
            case Mnemonic.ldr: return PrimitiveType.Word32;
            case Mnemonic.ldrt: return PrimitiveType.Word32;
            case Mnemonic.ldrb: return PrimitiveType.Byte;
            case Mnemonic.ldrbt: return PrimitiveType.Byte;
            case Mnemonic.ldrd: return PrimitiveType.Word64;
            case Mnemonic.ldrh: return PrimitiveType.Word16;
            case Mnemonic.ldrht: return PrimitiveType.Word16;
            case Mnemonic.ldrsb: return PrimitiveType.SByte;
            case Mnemonic.ldrsbt: return PrimitiveType.SByte;
            case Mnemonic.ldrsh: return PrimitiveType.Int16;
            case Mnemonic.ldrsht: return PrimitiveType.Int16;
            case Mnemonic.stc: return PrimitiveType.Word32;
            case Mnemonic.stc2: return PrimitiveType.Word32;
            case Mnemonic.stc2l: return PrimitiveType.Word32;
            case Mnemonic.stcl: return PrimitiveType.Word32;
            case Mnemonic.str: return PrimitiveType.Word32;
            case Mnemonic.strt: return PrimitiveType.Word32;
            case Mnemonic.strb: return PrimitiveType.Byte;
            case Mnemonic.strbt: return PrimitiveType.Byte;
            case Mnemonic.strd: return PrimitiveType.Word64;
            case Mnemonic.strh: return PrimitiveType.Word16;
            case Mnemonic.strht: return PrimitiveType.Word16;
            case Mnemonic.swp: return PrimitiveType.Word32;
            case Mnemonic.swpb: return PrimitiveType.Byte;
            case Mnemonic.vldr: return instr.Operands[0].DataType;
            case Mnemonic.vstr: return instr.Operands[0].DataType;
            }
            return VoidType.Instance;
        }


        Expression MaybeShiftOperand(Expression exp, int iop)
        {
            if (iop != instr.Operands.Length - 1)
                return exp;
            var ops = instr.Operands[iop];
            if (instr.ShiftType == Mnemonic.Invalid)
                return exp;

            Expression sh;
            if (instr.ShiftValue is RegisterStorage reg)
                sh = binder.EnsureRegister(reg);
            else
                sh = (Constant)instr.ShiftValue!;

            return MaybeShiftExpression(exp, sh, instr.ShiftType);
        }

        private Expression MaybeSlice(Expression exp, DataType dt)
        {
            if (dt.BitSize < exp.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(exp, dt));
                exp = tmp;
            }
            return exp;
        }

        private Expression MaybeShiftExpression(Expression exp, Expression sh, Mnemonic shiftType)
        {
            switch (shiftType)
            {
            case Mnemonic.asr: return m.Sar(exp, sh);
            case Mnemonic.lsl: return m.Shl(exp, sh);
            case Mnemonic.lsr: return m.Sar(exp, sh);
            case Mnemonic.ror: return m.Fn(CommonOps.Ror, exp, sh);
            case Mnemonic.rrx: return m.Fn(CommonOps.RorC.MakeInstance(exp.DataType, sh.DataType), exp, sh, C());
            default: return exp;
            }
        }

        void MaybePostOperand(int iop)
        {
            var op = instr.Operands[iop];
            if (!(op is MemoryOperand mop))
                return;
            if (!instr.Writeback || mop.PreIndex)
                return;
            var baseReg = Reg(mop.BaseRegister!);

            Expression? idx = null;
            var offset = mop.Offset;
            if (offset is not null && !offset.IsIntegerZero)
            {
                idx = offset;
            }
            else if (mop.Index is not null)
            {
                idx = binder.EnsureRegister(mop.Index);
                if (mop.ShiftType != Mnemonic.Invalid)
                {
                    var sh = Constant.Int32(mop.Shift);
                    idx = MaybeShiftExpression(idx, sh, mop.ShiftType);
                }
            }
            if (idx is not null)
            {
                var ea = mop.Add
                    ? m.IAdd(baseReg, idx)
                    : m.ISub(baseReg, idx);
                m.Assign(baseReg, ea);
            }
        }

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException($"ARM condition code {cond} not implemented.");
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(Registers.C));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(Registers.C));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(Registers.Z));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.NZV));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(Registers.NZV));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(Registers.ZC));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(Registers.NZV));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(Registers.ZC));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.NV));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(Registers.N));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(Registers.N));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(Registers.Z));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(Registers.V));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(Registers.V));
            case ArmCondition.AL:
                return Constant.True();
            case ArmCondition.NV:
            case ArmCondition.Invalid:
                return Constant.False();
            }
        }

        private Identifier FlagGroup(FlagGroupStorage flagGroup)
        {
            return binder.EnsureFlagGroup(flagGroup);
        }

        int BitSize(ArmVectorData vec)
        {
            switch (vec)
            {
            case ArmVectorData.I8:
            case ArmVectorData.S8:
            case ArmVectorData.U8:
                return 8;
            case ArmVectorData.F16:
            case ArmVectorData.I16:
            case ArmVectorData.S16:
            case ArmVectorData.U16:
                return 16;
            case ArmVectorData.F32:
            case ArmVectorData.I32:
            case ArmVectorData.S32:
            case ArmVectorData.U32:
                return 32;
            case ArmVectorData.F64:
            case ArmVectorData.I64:
            case ArmVectorData.S64:
            case ArmVectorData.U64:
                return 64;
            }
            return 0;
        }

        private Expression EmitNarrowingSlice(Expression e, DataType dt)
        {
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(e, dt));
            return tmp;
        }

        private void RewriteIntrinsicBinOp(IntrinsicProcedure intrinsic)
        {
            var src1 = Operand(1);
            var src2 = Operand(2);
            var dst = Operand(0);
            m.Assign(dst, m.Fn(
                intrinsic.MakeInstance(src1.DataType),
                src1,
                src2));
        }

        private void RewriteIntrinsic(IntrinsicProcedure intrinsic)
        {
            var args = new Expression[instr.Operands.Length - 1];
            for (int i = 1; i < instr.Operands.Length; ++i)
            {
                args[i - 1] = Operand(i);
            }
            var dst = Operand(0);
            m.Assign(dst, m.Fn(intrinsic, args));
        }

        private void RewriteIntrinsicSideeffect(IntrinsicProcedure intrinsic)
        {
            var args = new Expression[instr.Operands.Length];
            for (int i = 0; i < instr.Operands.Length; ++i)
            {
                args[i] = Operand(i);
            }
            m.SideEffect(m.Fn(intrinsic, args));
        }


        protected virtual void EmitUnitTest(AArch32Instruction instr)
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("ArmRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private static readonly ArrayType ab_4 = new ArrayType(PrimitiveType.Byte, 4);
        private static readonly ArrayType ab_8 = new ArrayType(PrimitiveType.Byte, 8);
        private static readonly ArrayType ai16_2 = new ArrayType(PrimitiveType.Int16, 2);
        private static readonly ArrayType au16_2 = new ArrayType(PrimitiveType.UInt16, 2);

        private static readonly IntrinsicProcedure bkpt_intrinsic = new IntrinsicBuilder("__breakpoint", true)
            .Void();
        private static readonly IntrinsicProcedure bkpt_arg_intrinsic = new IntrinsicBuilder("__breakpoint_arg", true)
            .Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure cdp_intrinsic = new IntrinsicBuilder("__cdp", true)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Void();
        private static readonly IntrinsicProcedure cdp2_intrinsic = new IntrinsicBuilder("__cdp2", true)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Param(PrimitiveType.UInt32)
            .Void();
        private static readonly IntrinsicProcedure clrex_intrinsic = new IntrinsicBuilder("__clrex", true)
            .Void();
        private static readonly IntrinsicProcedure cps_intrinsic = new IntrinsicBuilder("__cps", true)
            .Void();
        private static readonly IntrinsicProcedure cps_id_intrinsic = new IntrinsicBuilder("__cps_id", true)
            .Void();
        private static readonly IntrinsicProcedure crc32_intrinsic = new IntrinsicBuilder("__crc32", false)
            .GenericTypes("T")
            .Param(PrimitiveType.UInt32)
            .Param("T")
            .Returns(PrimitiveType.UInt32);
        private static readonly IntrinsicProcedure crc32c_intrinsic = new IntrinsicBuilder("__crc32c", false)
            .GenericTypes("T")
            .Param(PrimitiveType.UInt32)
            .Param("T")
            .Returns(PrimitiveType.UInt32);

        private static readonly IntrinsicProcedure dmb_intrinsic = new IntrinsicBuilder("__data_memory_barrier", true)
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure dsb_intrinsic = new IntrinsicBuilder("__data_sync_barrier", true)
            .Param(new UnknownType())
            .Void();

        private static readonly IntrinsicProcedure hadd_intrinsic = new IntrinsicBuilder("__hadd", false)
            .GenericTypes("TArray")
            .Params("TArray")
            .Params("TArray")
            .Returns("TArray");
        private static readonly IntrinsicProcedure hsub_intrinsic = new IntrinsicBuilder("__hsub", false)
            .GenericTypes("TArray")
            .Params("TArray")
            .Params("TArray")
            .Returns("TArray");
        private static readonly IntrinsicProcedure hvc_intrinsic = new IntrinsicBuilder("__hypervisor", true)
            .Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure isb_intrinsic = new IntrinsicBuilder("__instruction_sync_barrier", true)
            .Param(new UnknownType())
            .Void();

        private static readonly IntrinsicProcedure lda_sig = new IntrinsicBuilder("__load_acquire", true)
            .GenericTypes("T").PtrParam("T").Returns("T");
        private static readonly IntrinsicProcedure ldaex_sig = new IntrinsicBuilder("__load_acquire_exclusive", true)
             .GenericTypes("T").PtrParam("T").Returns("T");
        private static readonly IntrinsicProcedure ldc_intrinsic = new IntrinsicBuilder("__ldc", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ldc2_intrinsic = new IntrinsicBuilder("__ldc2", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ldc2l_intrinsic = new IntrinsicBuilder("__ldc2l", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ldcl_intrinsic = new IntrinsicBuilder("__ldcl", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ldrex_intrinsic = new IntrinsicBuilder("__ldrex", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure mcr_intrinsic = new IntrinsicBuilder("__mcr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure mcrr_intrinsic = new IntrinsicBuilder("__mcrr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure mcr2_intrinsic = new IntrinsicBuilder("__mcr2", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure mrc_intrinsic = new IntrinsicBuilder("__mrc", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mrc2_intrinsic = new IntrinsicBuilder("__mrc2", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mrrc_intrinsic = new IntrinsicBuilder("__mrrc", true)
            .GenericTypes("TReturn")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns("TReturn");
        private static readonly IntrinsicProcedure mrs_intrinsic = new IntrinsicBuilder("__mrs", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure msr_intrinsic = new IntrinsicBuilder("__msr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();

        private static readonly IntrinsicProcedure pkhbt_intrinsic = new IntrinsicBuilder("__pkhbt", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure pkhtb_intrinsic = new IntrinsicBuilder("__pkhtb", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure pld_intrinsic = new IntrinsicBuilder("__pld", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure pldw_intrinsic = new IntrinsicBuilder("__pldw", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure pli_intrinsic = new IntrinsicBuilder("__pli", true)
            .Param(PrimitiveType.Ptr32)
            .Void();

        private static readonly IntrinsicProcedure qadd_intrinsic = IntrinsicBuilder.GenericBinary("__qadd");
        private static readonly IntrinsicProcedure qasx_intrinsic = IntrinsicBuilder.GenericBinary("__qasx");
        private static readonly IntrinsicProcedure qsax_intrinsic = IntrinsicBuilder.GenericBinary("__qsax");
        private static readonly IntrinsicProcedure qsub_intrinsic = IntrinsicBuilder.GenericBinary("__qsub");

        private static readonly IntrinsicProcedure rbit_intrinsic = new IntrinsicBuilder("__reverse_bits", false)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure rev_intrinsic = new IntrinsicBuilder("__rev", false)
            .GenericTypes("T")
            .Param("T")
            .Returns("T");
        private static readonly IntrinsicProcedure rfeda_intrinsic = new IntrinsicBuilder("__rfeda", true)
            .Void();
        private static readonly IntrinsicProcedure rfedb_intrinsic = new IntrinsicBuilder("__rfedb", true)
            .Void();
        private static readonly IntrinsicProcedure rfeia_intrinsic = new IntrinsicBuilder("__rfeia", true)
            .Void();
        private static readonly IntrinsicProcedure rfeib_intrinsic = new IntrinsicBuilder("__rfeib", true)
            .Void();

        private static readonly IntrinsicProcedure sadd_intrinsic = IntrinsicBuilder.GenericBinary("__sadd");
        private static readonly IntrinsicProcedure sasx_intrinsic = IntrinsicBuilder.GenericBinary("__sasx");

        private static readonly IntrinsicProcedure sel_intrinsic = new IntrinsicBuilder("__sel", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure setpan_intrinsic = new IntrinsicBuilder("__set_pan", true)
            .Param(PrimitiveType.Bool)
            .Void();
        private static readonly IntrinsicProcedure sev_intrinsic = new IntrinsicBuilder("__send_event", true)
            .Void();
        private static readonly IntrinsicProcedure setend_intrinsic = new IntrinsicBuilder("__set_bigendian", true)
            .Param(PrimitiveType.Bool)
            .Void();
        private static readonly IntrinsicProcedure smc_intrinsic = new IntrinsicBuilder("__smc", true)
            .GenericTypes("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure srsda_intrinsic = new IntrinsicBuilder("__srsda", true)
            .PtrParam(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void(); 
        private static readonly IntrinsicProcedure srsdb_intrinsic = new IntrinsicBuilder("__srsdb", true)
            .PtrParam(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure srsia_intrinsic = new IntrinsicBuilder("__srsiab", true)
            .PtrParam(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure srsib_intrinsic = new IntrinsicBuilder("__srsib", true)
            .PtrParam(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure ssat16_intrinsic = new IntrinsicBuilder("__ssat16", false)
            .Param(PrimitiveType.Int32)
            .Param(ai16_2)
            .Returns(ai16_2);
        private static readonly IntrinsicProcedure ssat_intrinsic = new IntrinsicBuilder("__ssat", false)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.Int32);
        private static readonly IntrinsicProcedure ssub_intrinsic = IntrinsicBuilder.GenericBinary("__ssub");
        private static readonly IntrinsicProcedure stc_intrinsic = new IntrinsicBuilder("__stc", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure stc2_intrinsic = new IntrinsicBuilder("__stc2", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure stc2l_intrinsic = new IntrinsicBuilder("__stc2l", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure stcl_intrinsic = new IntrinsicBuilder("__stcl", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure stl_intrinsic = new IntrinsicBuilder("__store_release", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Void();
        private static readonly IntrinsicProcedure stlex_intrinsic = new IntrinsicBuilder("__store_release_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Returns("T");
        private static readonly IntrinsicProcedure strex_intrinsic = new IntrinsicBuilder("__store_exclusive", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Param("T")
            .Returns("T");
        private static readonly IntrinsicProcedure sxtab16_intrinsic = new IntrinsicBuilder("__sxtab16", false)
            .Param(ai16_2)
            .Param(ai16_2)
            .Returns(ai16_2);
        private static readonly IntrinsicProcedure sxtab16_ror_intrinsic = new IntrinsicBuilder("__sxtab16_ror", false)
            .Param(ai16_2)
            .Param(ai16_2)
            .Param(PrimitiveType.Int32)
            .Returns(ai16_2);

        private static readonly IntrinsicProcedure sxtb16_intrinsic = new IntrinsicBuilder("__sxtb16", false)
            .Param(ai16_2)
            .Returns(ai16_2);
        private static readonly IntrinsicProcedure sxtb16_ror_intrinsic = new IntrinsicBuilder("__sxtb16_ror", false)
            .Param(ai16_2)
            .Param(PrimitiveType.Int32)
            .Returns(ai16_2);

        private static readonly IntrinsicProcedure uadd_intrinsic = IntrinsicBuilder.GenericBinary("__uadd");
        private static readonly IntrinsicProcedure usad8_intrinsic = IntrinsicBuilder.GenericBinary("__usad8");
        private static readonly IntrinsicProcedure usada8_intrinsic = IntrinsicBuilder.GenericBinary("__usada8");
        private static readonly IntrinsicProcedure usat_intrinsic = new IntrinsicBuilder("__usat", false)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.UInt32);
        private static readonly IntrinsicProcedure uqadd_intrinsic = IntrinsicBuilder.GenericBinary("__uqadd");
        private static readonly IntrinsicProcedure uqasx_intrinsic = IntrinsicBuilder.GenericBinary("__uqasx");
        private static readonly IntrinsicProcedure usat16_intrinsic = new IntrinsicBuilder("__usat16", false)
            .Param(PrimitiveType.Int32)
            .Param(ai16_2)
            .Returns(au16_2);
        private static readonly IntrinsicProcedure uqsax_intrinsic = IntrinsicBuilder.GenericBinary("__uqsax");
        private static readonly IntrinsicProcedure uqsub_intrinsic = IntrinsicBuilder.GenericBinary("__uqsub");
        private static readonly IntrinsicProcedure usub_intrinsic = IntrinsicBuilder.GenericBinary("__usub");
        private static readonly IntrinsicProcedure uxtab16_intrinsic = new IntrinsicBuilder("__uxtab16", false)
            .Param(au16_2)
            .Param(au16_2)
            .Returns(au16_2);
        private static readonly IntrinsicProcedure uxtab16_ror_intrinsic = new IntrinsicBuilder("__uxtab16_ror", false)
            .Param(au16_2)
            .Param(au16_2)
            .Param(PrimitiveType.Int32)
            .Returns(au16_2);

        private static readonly IntrinsicProcedure uxtb16_initrinsic = new IntrinsicBuilder("__uxtb16", false)
            .Param(au16_2)
            .Returns(au16_2);
        private static readonly IntrinsicProcedure uxtb16_ror_intrinsic = new IntrinsicBuilder("__uxtb16_ror", false)
            .Param(au16_2)
            .Param(PrimitiveType.Int32)
            .Returns(au16_2);

        private static readonly IntrinsicProcedure vaba_intrinsic = IntrinsicBuilder.GenericBinary("__vaba", false);
        private static readonly IntrinsicProcedure vabal_intrinsic = new IntrinsicBuilder("__vabal", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Params("TArraySrc", "TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vabd_intrinsic = IntrinsicBuilder.GenericBinary("__vabd", false);
        private static readonly IntrinsicProcedure vabdl_intrinsic = new IntrinsicBuilder("__vabdl", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Params("TArraySrc", "TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vabs_cge_intrinsic = IntrinsicBuilder.GenericBinary("__vabs_cge");
        private static readonly IntrinsicProcedure vabs_cgt_intrinsic = IntrinsicBuilder.GenericBinary("__vabs_cgt");
        private static readonly IntrinsicProcedure vadd_intrinsic = IntrinsicBuilder.GenericBinary("__vadd", false);
        private static readonly IntrinsicProcedure vaddw_intrinsic = IntrinsicBuilder.GenericBinary("__vaddw", false);
        private static readonly IntrinsicProcedure vaddhn_intrinsic = new IntrinsicBuilder("__vadd_hn", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Params("TArraySrc", "TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vaddl_intrinsic = new IntrinsicBuilder("__vaddl", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Params("TArraySrc", "TArraySrc")
            .Returns("TArrayDst");

        private static readonly IntrinsicProcedure vbif_intrinsic = IntrinsicBuilder.GenericBinary("__vbif");
        private static readonly IntrinsicProcedure vbit_intrinsic = IntrinsicBuilder.GenericBinary("__vbit");
        private static readonly IntrinsicProcedure vbsl_intrinsic = IntrinsicBuilder.GenericBinary("__vbsl");
        
        private static readonly IntrinsicProcedure vceq_intrinsic = IntrinsicBuilder.GenericBinary("__vceq", false);
        private static readonly IntrinsicProcedure vcge_intrinsic = IntrinsicBuilder.GenericBinary("__vcge", false);
        private static readonly IntrinsicProcedure vcgt_intrinsic = IntrinsicBuilder.GenericBinary("__vcgt", false);
        private static readonly IntrinsicProcedure vcle_intrinsic = IntrinsicBuilder.GenericBinary("__vcle", false);
        private static readonly IntrinsicProcedure vclt_intrinsic = IntrinsicBuilder.GenericBinary("__vclt", false);
        private static readonly IntrinsicProcedure vcmla_intrinsic = IntrinsicBuilder.GenericBinary("__vcmla", false);
        private static readonly IntrinsicProcedure vcvt_intrinsic = new IntrinsicBuilder("__vcvt", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Returns("TArrayDst");

        private static readonly IntrinsicProcedure vdup_intrinsic = new IntrinsicBuilder("__vdup", false)
            .GenericTypes("TSrc", "TArrayDst")
            .Param("TSrc")
            .Returns("TArrayDst");

        private static readonly IntrinsicProcedure vext_intrinsic = new IntrinsicBuilder("__vext", false)
            .GenericTypes("TArray")
            .Param("TArray")
            .Param("TArray")
            .Param(PrimitiveType.Int32)
            .Returns("TArray");
        private static readonly IntrinsicProcedure vfmal_intrinsic = IntrinsicBuilder.GenericBinary("__vfmal");
        private static readonly IntrinsicProcedure vfnma_intrinsic = IntrinsicBuilder.GenericBinary("__vfnma");
        private static readonly IntrinsicProcedure vfnms_intrinsic = IntrinsicBuilder.GenericBinary("__vfnms");

        private static readonly IntrinsicProcedure vhadd_intrinsic = IntrinsicBuilder.GenericBinary("__vhadd");
        private static readonly IntrinsicProcedure vhsub_intrinsic = IntrinsicBuilder.GenericBinary("__vhsub");

        private static readonly IntrinsicProcedure vld1_multi_intrinsic = new IntrinsicBuilder("__vld1_multiple", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure vld2_multi_intrinsic = new IntrinsicBuilder("__vld2_multiple", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns(new UnknownType(16));
        private static readonly IntrinsicProcedure vld3_multi_intrinsic = new IntrinsicBuilder("__vld3_multiple", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns(new UnknownType(24));
        private static readonly IntrinsicProcedure vld4_multi_intrinsic = new IntrinsicBuilder("__vld4_multiple", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns(new UnknownType(32));
        
        private static readonly IntrinsicProcedure vmax_intrinsic = IntrinsicBuilder.GenericBinary("__vmax");
        private static readonly IntrinsicProcedure vmaxnm_intrinsic = IntrinsicBuilder.GenericBinary("__vmaxnm");
        private static readonly IntrinsicProcedure vmin_intrinsic = IntrinsicBuilder.GenericBinary("__vmin");
        private static readonly IntrinsicProcedure vminnm_intrinsic = IntrinsicBuilder.GenericBinary("__vminnm");
        private static readonly IntrinsicProcedure vmov_intrinsic = IntrinsicBuilder.GenericUnary("__vmov");
        private static readonly IntrinsicProcedure vmla_intrinsic = IntrinsicBuilder.GenericBinary("__vmla");
        private static readonly IntrinsicProcedure vmlal_intrinsic = IntrinsicBuilder.GenericBinary("__vmlal");
        private static readonly IntrinsicProcedure vmls_intrinsic = IntrinsicBuilder.GenericBinary("__vmls");
        private static readonly IntrinsicProcedure vmlsl_intrinsic = IntrinsicBuilder.GenericBinary("__vmlsl");
        private static readonly IntrinsicProcedure vmul_intrinsic = IntrinsicBuilder.GenericBinary("__vmul");
        private static readonly IntrinsicProcedure vmul_polynomial_intrinsic = IntrinsicBuilder.GenericBinary("__vmul_polynomial");
        private static readonly IntrinsicProcedure vmull_intrinsic = new IntrinsicBuilder("__vmull", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Param("TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vmull_polynomial_intrinsic = new IntrinsicBuilder("__vmull_polynomial", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Param("TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vmvn_intrinsic = IntrinsicBuilder.GenericUnary("__vmvn");
        private static readonly IntrinsicProcedure vmvn_imm_intrinsic = IntrinsicBuilder.GenericUnary("__vmvn_imm");

        private static readonly IntrinsicProcedure vneg_intrinsic = IntrinsicBuilder.GenericUnary("__vneg");
        private static readonly IntrinsicProcedure vnmla_intrinsic = IntrinsicBuilder.GenericBinary("__vnmla");
        private static readonly IntrinsicProcedure vnmls_intrinsic = IntrinsicBuilder.GenericBinary("__vnmls");
        private static readonly IntrinsicProcedure vnmul_intrinsic = IntrinsicBuilder.GenericBinary("__vnmul");
        private static readonly IntrinsicProcedure vpadal_intrinsic = new IntrinsicBuilder("__vpadal", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Returns("TDst");
        private static readonly IntrinsicProcedure vpadd_intrinsic = IntrinsicBuilder.GenericBinary("__vpadd");
        private static readonly IntrinsicProcedure vpmax_intrinsic = IntrinsicBuilder.GenericBinary("__vpmax");
        private static readonly IntrinsicProcedure vpmin_intrinsic = IntrinsicBuilder.GenericBinary("__vpmin");

        private static readonly IntrinsicProcedure vqabs_intrinsic = IntrinsicBuilder.GenericUnary("__vqabs");
        private static readonly IntrinsicProcedure vqadd_intrinsic = IntrinsicBuilder.GenericBinary("__vqadd");
        private static readonly IntrinsicProcedure vqdmulh_intrinsic = IntrinsicBuilder.GenericBinary("__vqdmulh");
        private static readonly IntrinsicProcedure vqrdmulh_intrinsic = IntrinsicBuilder.GenericBinary("__vqrdmulh");
        private static readonly IntrinsicProcedure vqshl_intrinsic = new IntrinsicBuilder("__vqshl", false)
            .GenericTypes("TArray")
            .Param("TArray")
            .Param(PrimitiveType.Int32)
            .Returns("TArray");
        private static readonly IntrinsicProcedure vqshlu_intrinsic = new IntrinsicBuilder("__vqshlu", false)
            .GenericTypes("TArray")
            .Param("TArray")
            .Param(PrimitiveType.Int32)
            .Returns("TArray");
        private static readonly IntrinsicProcedure vqshrn_intrinsic = new IntrinsicBuilder("__vqshrn", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Param(PrimitiveType.Int32)
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vqshrun_intrinsic = new IntrinsicBuilder("__vqshrun", false)
          .GenericTypes("TArraySrc", "TArrayDst")
          .Param("TArraySrc")
          .Param(PrimitiveType.UInt32)
          .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vqrshl_intrinsic = IntrinsicBuilder.GenericBinary("__vqrshl");
        private static readonly IntrinsicProcedure vqrshrn_intrinsic = new IntrinsicBuilder("__vqrshrn", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Param(PrimitiveType.Int32)
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vqsub_intrinsic = IntrinsicBuilder.GenericBinary("__vqsub");
        private static readonly IntrinsicProcedure vraddhn_intrinsic = new IntrinsicBuilder("__vraddhn", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Param("TArraySrc")
            .Param("TArraySrc")
            .Returns("TArrayDst");
        private static readonly IntrinsicProcedure vrecps_intrinsic = IntrinsicBuilder.GenericUnary("__vrecps");
        private static readonly IntrinsicProcedure vrev16_intrinsic = IntrinsicBuilder.GenericUnary("__vrev16");
        private static readonly IntrinsicProcedure vrev32_intrinsic = IntrinsicBuilder.GenericUnary("__vrev32");
        private static readonly IntrinsicProcedure vrev64_intrinsic = IntrinsicBuilder.GenericUnary("__vrev64");
        private static readonly IntrinsicProcedure vrhadd_intrinsic = IntrinsicBuilder.GenericBinary("__vrhadd");
        private static readonly IntrinsicProcedure vrshl_intrinsic = IntrinsicBuilder.GenericBinary("__vrshl");
        private static readonly IntrinsicProcedure vrshr_intrinsic = IntrinsicBuilder.GenericBinary("__vrshr");
        private static readonly IntrinsicProcedure vrsqrte_intrinsic = IntrinsicBuilder.GenericBinary("__vrsqrte");
        private static readonly IntrinsicProcedure vrsqrts_intrinsic = IntrinsicBuilder.GenericBinary("__vrsqrts");
        private static readonly IntrinsicProcedure vrsra_intrinsic = IntrinsicBuilder.GenericBinary("__vrsra");
        private static readonly IntrinsicProcedure vrsubhn_intrinsic = IntrinsicBuilder.GenericBinary("__vrsubhn");
        
        private static readonly IntrinsicProcedure vseleq_intrinsic = IntrinsicBuilder.GenericBinary("__vseleq");
        private static readonly IntrinsicProcedure vselge_intrinsic = IntrinsicBuilder.GenericBinary("__vselge");
        private static readonly IntrinsicProcedure vselgt_intrinsic = IntrinsicBuilder.GenericBinary("__vselgt");
        private static readonly IntrinsicProcedure vselvs_intrinsic = IntrinsicBuilder.GenericBinary("__vselvs");
        private static readonly IntrinsicProcedure vshl_intrinsic = new IntrinsicBuilder("__vshl", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure vshll_intrinsic = new IntrinsicBuilder("__vshll", false)
            .GenericTypes("TSrc", "TDst")
            .Param("TSrc")
            .Param(PrimitiveType.Int32)
            .Returns("TDst");
        private static readonly IntrinsicProcedure vshr_intrinsic = new IntrinsicBuilder("__vshr", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure vsli_intrinsic = new IntrinsicBuilder("__vsli", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure vsra_intrinsic = new IntrinsicBuilder("__vsra", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure vsri_intrinsic = new IntrinsicBuilder("__vsri", false)
            .GenericTypes("T")
            .Param("T")
            .Param(PrimitiveType.Int32)
            .Returns("T");
        private static readonly IntrinsicProcedure vst1_multi_intrinsic = new IntrinsicBuilder("__vst1_multiple", true)
            .GenericTypes("TElem","TVec")
            .PtrParam("TVec")
            .Param("TVec")
            .Void();
        private static readonly IntrinsicProcedure vst2_multi_intrinsic = new IntrinsicBuilder("__vst2_multiple", true)
            .GenericTypes("TElem", "TVec")
            .PtrParam("TVec")
            .Param("TVec")
            .Void();
        private static readonly IntrinsicProcedure vst3_multi_intrinsic = new IntrinsicBuilder("__vst3_multiple", true)
            .GenericTypes("TElem", "TVec")
            .PtrParam("TVec")
            .Param("TVec")
            .Void();
        private static readonly IntrinsicProcedure vst4_multi_intrinsic = new IntrinsicBuilder("__vst4_multiple", true)
            .GenericTypes("TElem", "TVec")
            .PtrParam("TVec")
            .Param("TVec")
            .Void();

        private static readonly IntrinsicProcedure vsub_intrinsic = IntrinsicBuilder.GenericBinary("__vsub", false);
        private static readonly IntrinsicProcedure vsubl_intrinsic = IntrinsicBuilder.GenericBinary("__vsubl", false);
        private static readonly IntrinsicProcedure vsubw_intrinsic = IntrinsicBuilder.GenericBinary("__vsubw", false);
        private static readonly IntrinsicProcedure vsubhn_intrinsic = new IntrinsicBuilder("__vsub_hn", false)
            .GenericTypes("TArraySrc", "TArrayDst")
            .Params("TArraySrc", "TArraySrc")
            .Returns("TArrayDst");

        private static readonly IntrinsicProcedure vtbl_intrinsic = new IntrinsicBuilder("__vtbl", true)
            .GenericTypes("TArray")
            .Param("TArray")
            .Param(ab_8)
            .Returns(ab_8);
        private static readonly IntrinsicProcedure vtst_intrinsic = IntrinsicBuilder.GenericBinary("__vtst", false);

        private static readonly IntrinsicProcedure wfi_intrinsic = new IntrinsicBuilder("__wait_for_interrupt", true)
            .Void();
        private static readonly IntrinsicProcedure wfe_intrinsic = new IntrinsicBuilder("__wait_for_event", true)
            .Void();

        private static readonly IntrinsicProcedure yield_intrinsic = new IntrinsicBuilder("__yield", true)
            .Void();

        static ArmRewriter()
        {
        }
    }

#if NATIVE

    public class ArmRewriterRetired : IEnumerable<RtlInstructionCluster>
    {
        private Dictionary<int, RegisterStorage> regs;
        private EndianImageReader rdr;
        private IStorageBinder binder;
        private IRewriterHost host;

        internal ArmRewriterRetired(Dictionary<int, RegisterStorage> regs, EndianImageReader rdr, AArch32ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.regs = regs;
            this.rdr = rdr;
            this.binder = binder;
            this.host = host;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            var bytes = rdr.Bytes;
            var offset = (int)rdr.Offset;
            var addr = rdr.Address.ToLinear();
            return new Enumerator(regs, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator : IEnumerator<RtlInstructionCluster>
        {
            private INativeRewriter native;
            private byte[] bytes;
            private GCHandle hBytes;
            private RtlEmitter m;
            private NativeTypeFactory ntf;
            private NativeRtlEmitter rtlEmitter;
            private ArmNativeRewriterHost host;
            private IntPtr iRtlEmitter;
            private IntPtr iNtf;
            private IntPtr iHost;

            public Enumerator(Dictionary<int, RegisterStorage> regs, ArmRewriterRetired outer)
            {
                this.bytes = outer.rdr.Bytes;
                ulong addr = outer.rdr.Address.ToLinear();
                this.hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                this.m = new RtlEmitter(new List<RtlInstruction>());
                this.ntf = new NativeTypeFactory();
                this.rtlEmitter = new NativeRtlEmitter(m, ntf, outer.host);
                this.host = new ArmNativeRewriterHost(regs, outer.binder, outer.host, this.ntf, rtlEmitter);

                this.iRtlEmitter = GetCOMInterface(rtlEmitter, IID_IRtlEmitter);
                this.iNtf = GetCOMInterface(ntf, IID_INativeTypeFactory);
                this.iHost = GetCOMInterface(host, IID_INativeRewriterHost);

				IntPtr unk = CreateNativeRewriter(hBytes.AddrOfPinnedObject(), bytes.Length, (int)outer.rdr.Offset, addr, iRtlEmitter, iNtf, iHost);
                this.native = (INativeRewriter)Marshal.GetObjectForIUnknown(unk);
            }

            public RtlInstructionCluster Current { get; private set; }

            object IEnumerator.Current { get { return Current; } }

            private IntPtr GetCOMInterface(object o, Guid iid)
            {
                var iUnknown = Marshal.GetIUnknownForObject(o);
                var hr2 = Marshal.QueryInterface(iUnknown, ref iid, out IntPtr intf);
                return intf;
            }

            public void Dispose()
            {
                if (this.native is not null)
                {
                    int n = native.GetCount();
                    Marshal.ReleaseComObject(this.native);
                    this.native = null;
                }
                if (iHost is not null)
                {
                    Marshal.Release(iHost);
                }
                if (iNtf is not null)
                {
                    Marshal.Release(iNtf);
                }
                if (iRtlEmitter is not null)
                { 
                   Marshal.Release(iRtlEmitter);
                }
                if (this.hBytes is not null && this.hBytes.IsAllocated)
                {
                    this.hBytes.Free();
                }
            }

            public bool MoveNext()
            {
                m.Instructions = new List<RtlInstruction>();
                int n = native.GetCount();
                if (native.Next() == 1)
                    return false;
                this.Current = this.rtlEmitter.ExtractCluster();
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        static ArmRewriterRetired()
        {
			IID_INativeRewriter = typeof(INativeRewriter).GUID;
            IID_INativeRewriterHost = typeof(INativeRewriterHost).GUID;
            IID_INativeTypeFactory = typeof(INativeTypeFactory).GUID;
            IID_IRtlEmitter = typeof(INativeRtlEmitter).GUID;
        }

		private static Guid IID_INativeRewriter;
        private static Guid IID_INativeRewriterHost;
        private static Guid IID_IRtlEmitter;
        private static Guid IID_INativeTypeFactory;

        [DllImport("ArmNative",CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateNativeRewriter")]
		//[return: MarshalAs(UnmanagedType.IUnknown)]
        public static extern IntPtr CreateNativeRewriter(IntPtr rawbytes, int length, int offset, ulong address, IntPtr rtlEmitter, IntPtr typeFactory, IntPtr host);
#endif
}
