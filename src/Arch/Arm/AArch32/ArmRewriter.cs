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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CallingConvention = System.Runtime.InteropServices.CallingConvention;

namespace Reko.Arch.Arm.AArch32
{
    public partial class ArmRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly IProcessorArchitecture arch;
        private readonly EndianImageReader rdr;
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
                case Mnemonic.mcr2:
                case Mnemonic.mcrr2:
                case Mnemonic.mrc2:
                case Mnemonic.mrrc2:
                case Mnemonic.pli:
                case Mnemonic.qsub8:
                case Mnemonic.rbit:
                case Mnemonic.rfeda:
                case Mnemonic.rfedb:
                case Mnemonic.rfeia:
                case Mnemonic.rfeib:
                case Mnemonic.sel:
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
                case Mnemonic.shadd16:
                case Mnemonic.shadd8:
                case Mnemonic.shsax:
                case Mnemonic.smmlar:
                case Mnemonic.smmlsr:
                case Mnemonic.smmulr:
                case Mnemonic.smuadx:
                case Mnemonic.smusdx:
                case Mnemonic.srsda:
                case Mnemonic.srsdb:
                case Mnemonic.srsia:
                case Mnemonic.srsib:
                case Mnemonic.ssax:
                case Mnemonic.sxtab16:
                case Mnemonic.sxtb16:
                case Mnemonic.uhadd16:
                case Mnemonic.uhadd8:
                case Mnemonic.uhasx:
                case Mnemonic.uhsub16:
                case Mnemonic.uhsub8:
                case Mnemonic.usad8:
                case Mnemonic.uxtab16:
                case Mnemonic.uxtb16:
                case Mnemonic.vabal:
                case Mnemonic.vabdl:
                case Mnemonic.vacge:
                case Mnemonic.vacgt:
                case Mnemonic.vaddhn:
                case Mnemonic.vbsl:
                case Mnemonic.vcls:
                case Mnemonic.vclz:
                case Mnemonic.vcnt:
                case Mnemonic.vcvta:
                case Mnemonic.vcvtb:
                case Mnemonic.vcvtm:
                case Mnemonic.vcvtn:
                case Mnemonic.vcvtp:
                case Mnemonic.vcvtt:
                case Mnemonic.vfnma:
                case Mnemonic.vfnms:
                case Mnemonic.vld1:
                case Mnemonic.vld2:
                case Mnemonic.vld3:
                case Mnemonic.vld4:
                case Mnemonic.vldmdb:
                case Mnemonic.vmaxnm:
                case Mnemonic.vminnm:
                case Mnemonic.vmovl:
                case Mnemonic.vmovn:
                case Mnemonic.vmsr:
                case Mnemonic.vorn:
                case Mnemonic.vpadal:
                case Mnemonic.vpaddl:
                case Mnemonic.vqdmlal:
                case Mnemonic.vqdmlsl:
                case Mnemonic.vqdmulh:
                case Mnemonic.vqdmull:
                case Mnemonic.vqmovun:
                case Mnemonic.vqmovn:
                case Mnemonic.vqneg:
                case Mnemonic.vqrdmlah:
                case Mnemonic.vqrshl:
                case Mnemonic.vqrshrn:
                case Mnemonic.vqrshrun:
                case Mnemonic.vqshlu:
                case Mnemonic.vqshrn:
                case Mnemonic.vqshrun:
                case Mnemonic.vqsub:
                case Mnemonic.vraddhn:
                case Mnemonic.vrecpe:
                case Mnemonic.vrev16:
                case Mnemonic.vrev32:
                case Mnemonic.vrev64:
                case Mnemonic.vrinta:
                case Mnemonic.vrintm:
                case Mnemonic.vrintn:
                case Mnemonic.vrintp:
                case Mnemonic.vrintr:
                case Mnemonic.vrintx:
                case Mnemonic.vrintz:
                case Mnemonic.vrshrn:
                case Mnemonic.vrsqrte:
                case Mnemonic.vrsqrts:
                case Mnemonic.vshrn:
                case Mnemonic.vst1:
                case Mnemonic.vst2:
                case Mnemonic.vst3:
                case Mnemonic.vst4:
                case Mnemonic.vsubhn:
                case Mnemonic.vswp:
                case Mnemonic.vtbl:
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
                case Mnemonic.wfe:
                case Mnemonic.sev:
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
                case Mnemonic.cdp: RewriteCdp("__cdp"); break;
                case Mnemonic.cdp2: RewriteCdp("__cdp2"); break;
                case Mnemonic.clrex: RewriteClrex(); break;
                case Mnemonic.clz: RewriteClz(); break;
                case Mnemonic.cmn: RewriteCmp(m.IAdd); break;
                case Mnemonic.cmp: RewriteCmp(m.ISub); break;
                case Mnemonic.cps: RewriteCps(cps_intrinsic); break;
                case Mnemonic.cpsid: RewriteCps(cps_id_intrinsic); break;
                case Mnemonic.crc32b: RewriteCrc("__crc32b", PrimitiveType.UInt8); break;
                case Mnemonic.crc32h: RewriteCrc("__crc32h", PrimitiveType.UInt16); break;
                case Mnemonic.crc32w: RewriteCrc("__crc32w", PrimitiveType.UInt32); break;
                case Mnemonic.crc32cb: RewriteCrc("__crc32cb", PrimitiveType.UInt8); break;
                case Mnemonic.crc32ch: RewriteCrc("__crc32ch", PrimitiveType.UInt16); break;
                case Mnemonic.crc32cw: RewriteCrc("__crc32cw", PrimitiveType.UInt32); break;
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
                case Mnemonic.ldc2l: RewriteLdc("__ldc2l"); break;
                case Mnemonic.ldc2: RewriteLdc("__ldc2"); break;
                case Mnemonic.ldcl: RewriteLdc("__ldcl"); break;
                case Mnemonic.ldc: RewriteLdc("__ldc"); break;
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
                case Mnemonic.mcr: RewriteMcr(); break;
                case Mnemonic.mcrr: RewriteMcrr(); break;
                case Mnemonic.mla: RewriteMultiplyAccumulate(m.IAdd); break;
                case Mnemonic.mls: RewriteMultiplyAccumulate(m.ISub); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.movt: RewriteMovt(); break;
                case Mnemonic.movw: RewriteMovw(); break;
                case Mnemonic.mrc: RewriteMrc(); break;
                case Mnemonic.mrrc: RewriteMrrc(); break;
                case Mnemonic.mrs: RewriteMrs(); break;
                case Mnemonic.msr: RewriteMsr(); break;
                case Mnemonic.mul: RewriteBinOp(m.IMul); break;
                case Mnemonic.mvn: RewriteUnaryOp(m.Comp); break;
                case Mnemonic.orn: RewriteLogical((a, b) => m.Or(a, m.Comp(b))); break;
                case Mnemonic.orr: RewriteLogical(m.Or); break;
                case Mnemonic.pkhbt: RewritePk("__pkhbt"); break;
                case Mnemonic.pkhtb: RewritePk("__pkhtb"); break;
                case Mnemonic.pld: RewritePld("__pld"); break;
                case Mnemonic.pldw: RewritePld("__pldw"); break;
                case Mnemonic.pop: RewritePop(); break;
                case Mnemonic.push: RewritePush(); break;
                case Mnemonic.qadd: RewriteQAddSub(m.IAdd); break;
                case Mnemonic.qadd16: RewriteVectorBinOp("__qadd_{0}", ArmVectorData.S16); break;
                case Mnemonic.qadd8: RewriteVectorBinOp("__qadd_{0}", ArmVectorData.S8); break;
                case Mnemonic.qdadd: RewriteQDAddSub(m.IAdd); break;
                case Mnemonic.qasx: RewriteQasx("__qasx"); break;
                case Mnemonic.qdsub: RewriteQDAddSub(m.ISub); break;
                case Mnemonic.qsax: RewriteQasx("__qsax"); break;
                case Mnemonic.qsub: RewriteQAddSub(m.ISub); break;
                case Mnemonic.qsub16: RewriteVectorBinOp("__qsub_{0}", ArmVectorData.S16); break;
                case Mnemonic.ror: RewriteShift(Ror); break;
                case Mnemonic.rrx: RewriteShift(Rrx); break;
                case Mnemonic.rev: RewriteRev(PrimitiveType.Byte); break;
                case Mnemonic.rev16: RewriteRev(PrimitiveType.Word16); break;
                case Mnemonic.revsh: RewriteRevsh(); break;
                case Mnemonic.rsb: RewriteRevBinOp(m.ISub, instr.SetFlags); break;
                case Mnemonic.rsc: RewriteAdcSbc(m.ISub, true); break;
                case Mnemonic.sadd16: RewriteVectorBinOp("__sadd_{0}", ArmVectorData.S16); break;
                case Mnemonic.sadd8: RewriteVectorBinOp("__sadd_{0}", ArmVectorData.S8); break;
                case Mnemonic.sasx: RewriteQasx("__sasx"); break;
                case Mnemonic.sbc: RewriteAdcSbc(m.ISub, false); break;
                case Mnemonic.sbfx: RewriteSbfx(); break;
                case Mnemonic.sdiv: RewriteDiv(m.SDiv); break;
                case Mnemonic.setend: RewriteSetend(); break;
                case Mnemonic.shasx: RewriteShasx(); break;
                case Mnemonic.shsub16: RewriteVectorBinOp("__shsub_{0}", ArmVectorData.S16); break;
                case Mnemonic.shsub8: RewriteVectorBinOp("__shsub_{0}", ArmVectorData.S8); break;
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
                case Mnemonic.smmls: RewriteSmml(m.ISub); break;
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
                case Mnemonic.ssat: RewriteSsat(); break;
                case Mnemonic.ssat16: RewriteSat16(ssat16_intrinsic); break;
                case Mnemonic.ssub16: RewriteVectorBinOp("__ssub16", ArmVectorData.S16); break;
                case Mnemonic.ssub8: RewriteVectorBinOp("__ssub8", ArmVectorData.S8); break;
                case Mnemonic.stc: RewriteStc(stc_intrinsic); break;
                case Mnemonic.stc2l: RewriteStc(stc2l_intrinsic); break;
                case Mnemonic.stc2: RewriteStc(stc2_intrinsic); break;
                case Mnemonic.stcl: RewriteStc(stcl_intrinsic); break;
                case Mnemonic.stl:  RewriteStoreRelease(stl_intrinsic, PrimitiveType.Word32); break;
                case Mnemonic.stlb: RewriteStoreRelease(stl_intrinsic, PrimitiveType.Byte); break;
                case Mnemonic.stlh: RewriteStoreRelease(stl_intrinsic, PrimitiveType.Word16); break;
                case Mnemonic.stlex:  RewriteStoreExclusive(stlex_intrinsic, PrimitiveType.Word32); break;
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
                case Mnemonic.strexd: RewriteStoreReleaseDoubleExclusive(strex_intrinsic);  break;
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
                case Mnemonic.sxtah: RewriteXtab(PrimitiveType.Int16); break;
                case Mnemonic.sxtb: RewriteXtb(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Mnemonic.sxth: RewriteXtb(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.tbb: RewriteTableBranch(PrimitiveType.Byte); break;
                case Mnemonic.tbh: RewriteTableBranch(PrimitiveType.Word16); break;
                case Mnemonic.teq: RewriteTeq(); break;
                case Mnemonic.trap: RewriteTrap(); break;
                case Mnemonic.tst: RewriteTst(); break;
                case Mnemonic.uadd16: RewriteVectorBinOp("__uadd_{0}", ArmVectorData.I16); break;
                case Mnemonic.uadd8: RewriteVectorBinOp("__uadd_{0}", ArmVectorData.I8); break;
                case Mnemonic.uasx: RewriteUasx(); break;
                case Mnemonic.ubfx: RewriteUbfx(); break;
                case Mnemonic.udf: RewriteUdf(); break;
                case Mnemonic.udiv: RewriteDiv(m.UDiv); break;
                case Mnemonic.uhsax: RewriteUhsax(); break;
                case Mnemonic.umaal: RewriteUmaal(); break;
                case Mnemonic.umlal: RewriteUmlal(); break;
                case Mnemonic.umull: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Mnemonic.uqasx: RewriteVectorBinOp("__uqasx_{0}", ArmVectorData.U16); break;
                case Mnemonic.uqadd16: RewriteVectorBinOp("__uqadd_{0}", ArmVectorData.U16); break;
                case Mnemonic.uqadd8: RewriteVectorBinOp("__uqadd_{0}", ArmVectorData.U8); break;
                case Mnemonic.uqsax: RewriteVectorBinOp("__uqsax_{0}", ArmVectorData.U16); break;
                case Mnemonic.uqsub16: RewriteVectorBinOp("__uqsub_{0}", ArmVectorData.U16); break;
                case Mnemonic.uqsub8: RewriteVectorBinOp("__uqsub_{0}", ArmVectorData.U8); break;
                case Mnemonic.usada8: RewriteUsada8(); break;
                case Mnemonic.usat: RewriteUsat(); break;
                case Mnemonic.usat16: RewriteSat16(usat16_intrinsic); break;
                case Mnemonic.usax: RewriteUsax(); break;
                case Mnemonic.usub16: RewriteVectorBinOp("__usub_{0}", ArmVectorData.I16); break;
                case Mnemonic.usub8: RewriteVectorBinOp("__usub_{0}", ArmVectorData.I8); break;
                case Mnemonic.uxtab: RewriteXtab(PrimitiveType.Byte); break;
                case Mnemonic.uxtah: RewriteXtab(PrimitiveType.UInt16); break;
                case Mnemonic.uxtb: RewriteXtb(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.uxth: RewriteXtb(PrimitiveType.UInt16, PrimitiveType.UInt32); break;
                case Mnemonic.wfi: RewriteWfi(); break;
                case Mnemonic.yield: RewriteYield(); break;


                case Mnemonic.vabs: RewriteVectorUnaryOp("__vabs_{0}"); break;
                case Mnemonic.vaba: RewriteVectorBinOp("__vaba_{0}"); break;
                case Mnemonic.vabd: RewriteVectorBinOp("__vabd_{0}"); break;
                case Mnemonic.vadd: RewriteVectorBinOp("__vadd_{0}"); break;
                case Mnemonic.vaddl: RewriteVectorBinOp("__vaddl_{0}"); break;
                case Mnemonic.vaddw: RewriteVectorBinOp("__vaddw_{0}"); break;
                case Mnemonic.vand: RewriteVecBinOp(m.And); break;
                case Mnemonic.vbic: RewriteVbic(); break;
                case Mnemonic.vcmp: RewriteVcmp(); break;
                case Mnemonic.vbif: RewriteIntrinsic("__vbif", false, Domain.UnsignedInt); break;
                case Mnemonic.vbit: RewriteIntrinsic("__vbit", false, Domain.UnsignedInt); break;
                case Mnemonic.vceq: RewriteVectorBinOp("__vceq_{0}"); break;
                case Mnemonic.vcge: RewriteVectorBinOp("__vcge_{0}"); break;
                case Mnemonic.vcgt: RewriteVectorBinOp("__vcgt_{0}"); break;
                case Mnemonic.vcle: RewriteVectorBinOp("__vcle_{0}"); break;
                case Mnemonic.vclt: RewriteVectorBinOp("__vclt_{0}"); break;
                case Mnemonic.vcmpe: RewriteVcmp(); break;
                case Mnemonic.vcvt: RewriteVcvt(); break;
                case Mnemonic.vcvtr: RewriteVcvtr(); break;
                case Mnemonic.vdiv: RewriteVecBinOp(m.FDiv); break;
                case Mnemonic.vdup: RewriteVdup(); break;
                case Mnemonic.veor: RewriteVecBinOp(m.Xor); break;
                case Mnemonic.vext: RewriteVext(); break;
                case Mnemonic.vfma: RewriteVfmas("__vfma", m.FAdd); break;
                case Mnemonic.vfms: RewriteVfmas("__vfms", m.FSub); break;
                case Mnemonic.vhadd: RewriteVectorBinOp("__vhadd_{0}"); break;
                case Mnemonic.vhsub: RewriteVectorBinOp("__vhsub_{0}"); break;
                case Mnemonic.vldmia: RewriteVldmia(); break;
                case Mnemonic.vldr: RewriteVldr(); break;
                case Mnemonic.vmax: RewriteVectorBinOp("__vmax_{0}"); break;
                case Mnemonic.vmin: RewriteVectorBinOp("__vmin_{0}"); break;
                case Mnemonic.vmov: RewriteVmov(); break;
                case Mnemonic.vmla: RewriteVectorBinOp("__vmla_{0}"); break;
                case Mnemonic.vmls: RewriteVectorBinOp("__vmls_{0}"); break;
                case Mnemonic.vmlal: RewriteVectorBinOp("__vmlal_{0}"); break;
                case Mnemonic.vmlsl: RewriteVectorBinOp("__vmlsl_{0}"); break;
                case Mnemonic.vmrs: RewriteVmrs(); break;
                case Mnemonic.vmvn: RewriteVmvn(); break;
                case Mnemonic.vmul: RewriteVectorBinOp("__vmul_{0}"); break;
                case Mnemonic.vmull: RewriteVectorBinOp("__vmull_{0}"); break;
                case Mnemonic.vorr: RewriteVecBinOp(m.Or); break;
                case Mnemonic.vneg: RewriteVectorUnaryOp("__vneg_{0}"); break;
                case Mnemonic.vnmla: RewriteVectorBinOp("__vnmla_{0}"); break;
                case Mnemonic.vnmls: RewriteVectorBinOp("__vnmls_{0}"); break;
                case Mnemonic.vnmul: RewriteVectorBinOp("__vnmul_{0}"); break;
                case Mnemonic.vpadd: RewriteVectorBinOp("__vpadd_{0}"); break;
                case Mnemonic.vpmax: RewriteVectorBinOp("__vpmax_{0}"); break;
                case Mnemonic.vpmin: RewriteVectorBinOp("__vpmin_{0}"); break;
                case Mnemonic.vpop: RewriteVpop(); break;
                case Mnemonic.vpush: RewriteVpush(); break;
                case Mnemonic.vqabs: RewriteVectorBinOp("__vqabs_{0}"); break;
                case Mnemonic.vqadd: RewriteVectorBinOp("__vqadd_{0}"); break;
                case Mnemonic.vqshl: RewriteVectorBinOp("__vqshl_{0}"); break;
                case Mnemonic.vrecps: RewriteVectorUnaryOp("__vrecps_{0}"); break;
                case Mnemonic.vrhadd: RewriteVectorBinOp("__vrhadd_{0}"); break;
                case Mnemonic.vrshl: RewriteVectorBinOp("__vrshl_{0}"); break;
                case Mnemonic.vrshr: RewriteVectorBinOp("__vrshr_{0}"); break;
                case Mnemonic.vrsra: RewriteVectorBinOp("__vrsra_{0}"); break;
                case Mnemonic.vrsubhn: RewriteVectorBinOp("__vrsubhn_{0}"); break;
                case Mnemonic.vseleq: RewriteVectorBinOp("__vseleq_{0}"); break;
                case Mnemonic.vselge: RewriteVectorBinOp("__vselge_{0}"); break;
                case Mnemonic.vselgt: RewriteVectorBinOp("__vselgt_{0}"); break;
                case Mnemonic.vselvs: RewriteVectorBinOp("__vselvs_{0}"); break;
                case Mnemonic.vstm: RewriteVstmia(true, false); break;
                case Mnemonic.vstmdb: RewriteVstmia(false, true); break;
                case Mnemonic.vstmia: RewriteVstmia(true, true); break;
                case Mnemonic.vsqrt: RewriteVsqrt(); break;
                case Mnemonic.vshl: RewriteVectorBinOp("__vshl_{0}"); break;
                case Mnemonic.vshll: RewriteVectorBinOp("__vshll_{0}"); break;
                case Mnemonic.vshr: RewriteVectorBinOp("__vshr_{0}"); break;
                case Mnemonic.vsli: RewriteVectorBinOp("__vsli_{0}"); break;
                case Mnemonic.vsra: RewriteVectorBinOp("__vsra_{0}"); break;
                case Mnemonic.vsri: RewriteVectorBinOp("__vsri_{0}"); break;
                case Mnemonic.vstr: RewriteVstr(); break;
                case Mnemonic.vsub: RewriteVectorBinOp("__vsub_{0}"); break;
                case Mnemonic.vsubl: RewriteVectorBinOp("__vsubl_{0}"); break;
                case Mnemonic.vsubw: RewriteVectorBinOp("__vsubw_{0}"); break;
                case Mnemonic.vtst: RewriteVectorBinOp("__vtst_{0}"); break;
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
            return binder.EnsureFlagGroup(Registers.cpsr, 0xE, "NZC", PrimitiveType.Byte);
        }

        Expression NZCV()
        {
            return binder.EnsureFlagGroup(Registers.cpsr, 0xF, "NZCV", PrimitiveType.Byte);
        }

        Expression C()
        {
            return binder.EnsureFlagGroup(Registers.cpsr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
        }

        Expression Q()
        {
            return binder.EnsureFlagGroup(Registers.cpsr, 0x10, "Q", PrimitiveType.Bool);
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
            if (instr.Operands[0] is AddressOperand aOp)
            {
                dst = aOp.Address;
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
                    ((AddressOperand) instr.Operands[1]).Address,
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
            if (mem.Offset != null)
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
            else if (mem.Index != null)
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
            case ImmediateOperand iOp:
                return iOp.Value;
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

                        if (mop.Index != null)
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
                    if ((mop.PreIndex || !instr.Writeback))
                    {
                        if (mop.Offset != null && !mop.Offset.IsZero)
                        {
                            var offset = mop.Offset;
                            ea = mop.Add
                                ? m.IAdd(ea, offset)
                                : m.ISub(ea, offset);
                        }
                        else if (mop.Index != null)
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
            case AddressOperand aop:
                return aop.Address;
            case IndexedOperand ixop:
                // Extract a single item from the vector register
                var ixreg = Reg(ixop.Register);
                return m.ARef(ixop.Width, ixreg, Constant.Int32(ixop.Index));
            }
            throw new NotImplementedException(op.GetType().Name);
        }

        public virtual Address ComputePcRelativeOffset(MemoryOperand mop)
        {
            var dst = instr.Address + 8u;
            if (mop.Offset != null)
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
            case Mnemonic.vldr: return instr.Operands[0].Width;
            case Mnemonic.vstr: return instr.Operands[0].Width;
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
                sh = ((ImmediateOperand) instr.ShiftValue!).Value;

            return MaybeShiftExpression(exp, sh, instr.ShiftType);
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
            if (offset != null && !offset.IsIntegerZero)
            {
                idx = offset;
            }
            else if (mop.Index != null)
            {
                idx = binder.EnsureRegister(mop.Index);
                if (mop.ShiftType != Mnemonic.Invalid)
                {
                    var sh = Constant.Int32(mop.Shift);
                    idx = MaybeShiftExpression(idx, sh, mop.ShiftType);
                }
            }
            if (idx != null)
            {
                var ea = mop.Add
                    ? m.IAdd(baseReg, idx)
                    : m.ISub(baseReg, idx);
                m.Assign(baseReg, ea);
            }
        }

        protected Expression? TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            default:
                throw new NotImplementedException($"ARM condition code {cond} not implemented.");
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Bool));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Bool));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Bool));
            case ArmCondition.GE:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.GT:
                return m.Test(ConditionCode.GT, FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.HI:
                return m.Test(ConditionCode.UGT, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LE:
                return m.Test(ConditionCode.LE, FlagGroup(FlagM.ZF | FlagM.CF | FlagM.VF, "NZV", PrimitiveType.Byte));
            case ArmCondition.LS:
                return m.Test(ConditionCode.ULE, FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte));
            case ArmCondition.LT:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte));
            case ArmCondition.MI:
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Bool));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF, "N", PrimitiveType.Bool));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Bool));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(FlagM.VF, "V", PrimitiveType.Bool));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Bool));
            }
        }

        Identifier FlagGroup(FlagM bits, string name, PrimitiveType type)
        {
            return binder.EnsureFlagGroup(Registers.cpsr, (uint) bits, name, type);
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

        void RewriteSwp(PrimitiveType type)
        {
            string fnName;
            if (type == PrimitiveType.Byte)
            {
                fnName = "std::atomic_exchange<byte>";
            }
            else
            {
                fnName = "std::atomic_exchange<int32_t>";
            }
            var dst = Operand(0, PrimitiveType.Word32, true);
            var intrinsic = host.Intrinsic(fnName, true, type,
                Operand(1),
                Operand(2));
            m.Assign(dst, intrinsic);
        }

        private Expression EmitNarrowingSlice(Expression e, DataType dt)
        {
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(dt, e, 0));
            return tmp;
        }

        private void RewriteIntrinsic(string name, bool hasSideEffect, Domain returnDomain)
        {
            var args = Enumerable.Range(1, instr.Operands.Length-1)
                .Select(i => Operand(i))
                .ToArray();
            var dst = Operand(0);
            var dtRet = PrimitiveType.Create(returnDomain, dst.DataType.BitSize);
            var intrinsic = host.Intrinsic(name, hasSideEffect, dtRet, args);
            m.Assign(dst, intrinsic);
        }

        private void EmitUnitTest(AArch32Instruction instr)
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("ArmRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private static readonly IntrinsicProcedure clrex_intrinsic = new IntrinsicBuilder("__clrex", true)
            .Void();
        private static readonly IntrinsicProcedure cps_intrinsic = new IntrinsicBuilder("__cps", true)
            .Void();
        private static readonly IntrinsicProcedure cps_id_intrinsic = new IntrinsicBuilder("__cps_id", true)
            .Void();
        private static readonly IntrinsicProcedure lda_sig = new IntrinsicBuilder("__load_acquire", true)
            .GenericTypes("T").PtrParam("T").Returns("T");
        private static readonly IntrinsicProcedure ldaex_sig = new IntrinsicBuilder("__load_acquire_exclusive", true)
             .GenericTypes("T").PtrParam("T").Returns("T");
        private static readonly IntrinsicProcedure ldrex_intrinsic = new IntrinsicBuilder("__ldrex", true)
            .GenericTypes("T")
            .PtrParam("T")
            .Returns("T");
        private static readonly IntrinsicProcedure rev_intrinsic = new IntrinsicBuilder("__rev", false)
            .GenericTypes("T")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure ssat16_intrinsic;
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
        private static readonly IntrinsicProcedure usat_intrinsic = new IntrinsicBuilder("__usat", false)
            .Param(PrimitiveType.Int32)
            .Param(PrimitiveType.Int32)
            .Returns(PrimitiveType.UInt32);
        private static readonly IntrinsicProcedure usat16_intrinsic;
        private static readonly IntrinsicProcedure yield_intrinsic = new IntrinsicBuilder("__yield", true)
            .Void();

        static ArmRewriter()
        {
            var pb = new Pointer(PrimitiveType.Byte, 32);
            var p16 = new Pointer(PrimitiveType.Word16, 32);
            var p32 = new Pointer(PrimitiveType.Word32, 32);
            var p64 = new Pointer(PrimitiveType.Word64, 64);

            ssat16_intrinsic = new IntrinsicBuilder("__ssat16", false)
                .Param(PrimitiveType.Int32)
                .Param(new ArrayType(PrimitiveType.Int16, 2))
                .Returns(new ArrayType(PrimitiveType.Int16, 2));
            usat16_intrinsic = new IntrinsicBuilder("__usat16", false)
                .Param(PrimitiveType.Int32)
                .Param(new ArrayType(PrimitiveType.Int16, 2))
                .Returns(new ArrayType(PrimitiveType.UInt16, 2));

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
                if (this.native != null)
                {
                    int n = native.GetCount();
                    Marshal.ReleaseComObject(this.native);
                    this.native = null;
                }
                if (iHost != null)
                {
                    Marshal.Release(iHost);
                }
                if (iNtf != null)
                {
                    Marshal.Release(iNtf);
                }
                if (iRtlEmitter != null)
                { 
                   Marshal.Release(iRtlEmitter);
                }
                if (this.hBytes != null && this.hBytes.IsAllocated)
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
