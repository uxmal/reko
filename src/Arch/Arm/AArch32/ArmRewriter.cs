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
using Reko.Core.Machine;
using Reko.Core.NativeInterface;
using Reko.Core.Rtl;
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
        protected int pcValueOffset;        // The offset to add to the current instruction's address when reading the PC register. 
        protected AArch32Instruction instr;
        protected InstrClass rtlClass;
        protected RtlEmitter m;

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
        }

        protected virtual void PostRewrite() { }
        protected virtual void RewriteIt() { }

        MachineOperand Dst() => instr.ops[0];
        MachineOperand Src1() => instr.ops[1];
        MachineOperand Src2() => instr.ops[2];
        MachineOperand Src3() => instr.ops[3];

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var addrInstr = instr.Address;
                // Most instructions are linear.
                this.rtlClass = instr.InstructionClass;
                var rtls = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtls);
                // Most instructions have a conditional mode of operation.
                //$TODO: make sure non-conditional instructions are handled correctly here.
                ConditionalSkip(false);
                switch (instr.opcode)
                {
                default:
                case Opcode.aesd:
                case Opcode.aese:
                case Opcode.aesimc:
                case Opcode.aesmc:
                case Opcode.bxj:
                case Opcode.clrex:
                case Opcode.crc32b:
                case Opcode.crc32cb:
                case Opcode.crc32ch:
                case Opcode.crc32cw:
                case Opcode.crc32h:
                case Opcode.crc32w:
                case Opcode.dbg:
                case Opcode.fldmdbx:
                case Opcode.fldmiax:
                case Opcode.fstmdbx:
                case Opcode.fstmiax:
                case Opcode.hlt:
                case Opcode.lda:
                case Opcode.ldab:
                case Opcode.ldaex:
                case Opcode.ldaexb:
                case Opcode.ldaexd:
                case Opcode.ldaexh:
                case Opcode.ldah:
                case Opcode.ldrexb:
                case Opcode.ldrexd:
                case Opcode.ldrexh:
                case Opcode.mcr2:
                case Opcode.mcrr2:
                case Opcode.mrc2:
                case Opcode.mrrc2:
                case Opcode.pkhbt:
                case Opcode.pkhtb:
                case Opcode.pldw:
                case Opcode.pli:
                case Opcode.qasx:
                case Opcode.qsax:
                case Opcode.qsub8:
                case Opcode.rbit:
                case Opcode.rev16:
                case Opcode.revsh:
                case Opcode.rfeda:
                case Opcode.rfedb:
                case Opcode.rfeia:
                case Opcode.rfeib:
                case Opcode.sasx:
                case Opcode.sel:
                case Opcode.sha1c:
                case Opcode.sha1h:
                case Opcode.sha1m:
                case Opcode.sha1p:
                case Opcode.sha1su0:
                case Opcode.sha1su1:
                case Opcode.sha256h:
                case Opcode.sha256h2:
                case Opcode.sha256su0:
                case Opcode.sha256su1:
                case Opcode.shadd16:
                case Opcode.shadd8:
                case Opcode.shasx:
                case Opcode.shsax:
                case Opcode.smmlar:
                case Opcode.smmlsr:
                case Opcode.smmulr:
                case Opcode.smuad:
                case Opcode.smuadx:
                case Opcode.smusdx:
                case Opcode.srsda:
                case Opcode.srsdb:
                case Opcode.srsia:
                case Opcode.srsib:
                case Opcode.ssat16:
                case Opcode.ssax:
                case Opcode.stl:
                case Opcode.stlb:
                case Opcode.stlex:
                case Opcode.stlexb:
                case Opcode.stlexd:
                case Opcode.stlexh:
                case Opcode.stlh:
                case Opcode.strexb:
                case Opcode.strexd:
                case Opcode.strexh:
                case Opcode.sxtab16:
                case Opcode.sxtb16:
                case Opcode.uhadd16:
                case Opcode.uhadd8:
                case Opcode.uhasx:
                case Opcode.uhsax:
                case Opcode.uhsub16:
                case Opcode.uhsub8:
                case Opcode.uqadd16:
                case Opcode.uqadd8:
                case Opcode.uqsax:
                case Opcode.uqsub16:
                case Opcode.uqsub8:
                case Opcode.usad8:
                case Opcode.usada8:
                case Opcode.usat16:
                case Opcode.uxtab16:
                case Opcode.uxtb16:
                case Opcode.vabal:
                case Opcode.vaba:
                case Opcode.vabdl:
                case Opcode.vabd:
                case Opcode.vacge:
                case Opcode.vacgt:
                case Opcode.vaddhn:
                case Opcode.vaddw:
                case Opcode.vbic:
                case Opcode.vbif:
                case Opcode.vbit:
                case Opcode.vbsl:
                case Opcode.vcls:
                case Opcode.vclz:
                case Opcode.vcnt:
                case Opcode.vcvta:
                case Opcode.vcvtb:
                case Opcode.vcvtm:
                case Opcode.vcvtn:
                case Opcode.vcvtp:
                case Opcode.vcvtt:
                case Opcode.vfma:
                case Opcode.vfms:
                case Opcode.vfnma:
                case Opcode.vfnms:
                case Opcode.vld1:
                case Opcode.vld2:
                case Opcode.vld3:
                case Opcode.vld4:
                case Opcode.vldmdb:
                case Opcode.vmaxnm:
                case Opcode.vminnm:
                case Opcode.vmovl:
                case Opcode.vmovn:
                case Opcode.vmsr:
                case Opcode.vmull:
                case Opcode.vorn:
                case Opcode.vpadal:
                case Opcode.vpaddl:
                case Opcode.vqdmlal:
                case Opcode.vqdmlsl:
                case Opcode.vqdmulh:
                case Opcode.vqdmull:
                case Opcode.vqmovun:
                case Opcode.vqmovn:
                case Opcode.vqneg:
                case Opcode.vqrdmlah:
                case Opcode.vqrshl:
                case Opcode.vqrshrn:
                case Opcode.vqrshrun:
                case Opcode.vqshlu:
                case Opcode.vqshrn:
                case Opcode.vqshrun:
                case Opcode.vqsub:
                case Opcode.vraddhn:
                case Opcode.vrecpe:
                case Opcode.vrecps:
                case Opcode.vrev16:
                case Opcode.vrev32:
                case Opcode.vrev64:
                case Opcode.vrhadd:
                case Opcode.vrinta:
                case Opcode.vrintm:
                case Opcode.vrintn:
                case Opcode.vrintp:
                case Opcode.vrintr:
                case Opcode.vrintx:
                case Opcode.vrintz:
                case Opcode.vrshrn:
                case Opcode.vrsqrte:
                case Opcode.vrsqrts:
                case Opcode.vrsra:
                case Opcode.vrsubhn:
                case Opcode.vseleq:
                case Opcode.vselge:
                case Opcode.vselgt:
                case Opcode.vselvs:
                case Opcode.vshll:
                case Opcode.vshrn:
                case Opcode.vsli:
                case Opcode.vsri:
                case Opcode.vst1:
                case Opcode.vst2:
                case Opcode.vst3:
                case Opcode.vst4:
                case Opcode.vsubhn:
                case Opcode.vsubl:
                case Opcode.vsubw:
                case Opcode.vswp:
                case Opcode.vtbl:
                case Opcode.vtbx:
                case Opcode.vtrn:
                case Opcode.vtst:
                case Opcode.vuzp:
                case Opcode.vzip:
                case Opcode.dcps1:
                case Opcode.dcps2:
                case Opcode.dcps3:
                case Opcode.lsrs:
                case Opcode.subs:
                case Opcode.movs:
                case Opcode.wfe:
                case Opcode.sev:
                case Opcode.sevl:
                    NotImplementedYet();
                    break;
                case Opcode.Invalid:
                    Invalid();
                    break;
                case Opcode.adc: RewriteAdcSbc(m.IAdd, false); break;
                case Opcode.add: RewriteBinOp(m.IAdd); break;
                case Opcode.addw: RewriteAddw(); break;
                case Opcode.adr: RewriteAdr(); break;
                case Opcode.and: RewriteLogical(m.And); break;
                case Opcode.asr: RewriteShift(m.Sar); break;
                case Opcode.asrs: RewriteShift(m.Sar); break;
                case Opcode.b: RewriteB(false); break;
                case Opcode.bfc: RewriteBfc(); break;
                case Opcode.bfi: RewriteBfi(); break;
                case Opcode.bic: RewriteLogical(Bic); break;
                case Opcode.bkpt: RewriteBkpt(); break;
                case Opcode.bl: RewriteB(true); break;
                case Opcode.blx: RewriteB(true); break;
                case Opcode.bx: RewriteB(false); break;
                case Opcode.cbz: RewriteCbnz(m.Eq0); break;
                case Opcode.cbnz: RewriteCbnz(m.Ne0); break;
                case Opcode.cdp: RewriteCdp("__cdp"); break;
                case Opcode.cdp2: RewriteCdp("__cdp2"); break;
                case Opcode.clz: RewriteClz(); break;
                case Opcode.cmn: RewriteCmp(m.IAdd); break;
                case Opcode.cmp: RewriteCmp(m.ISub); break;
                case Opcode.cps: RewriteCps(); break;
                case Opcode.dsb: RewriteDsb(); break;
                case Opcode.dmb: RewriteDmb(); break;
                case Opcode.eor: RewriteLogical(m.Xor); break;
                case Opcode.eret: RewriteEret(); break;
                case Opcode.hint: RewriteHint(); break;
                case Opcode.hvc: RewriteHvc(); break;
                case Opcode.isb: RewriteIsb(); break;
                case Opcode.it: RewriteIt(); break;
                case Opcode.ldc2l: RewriteLdc("__ldc2l"); break;
                case Opcode.ldc2: RewriteLdc("__ldc2"); break;
                case Opcode.ldcl: RewriteLdc("__ldcl"); break;
                case Opcode.ldc: RewriteLdc("__ldc"); break;
                case Opcode.ldm: RewriteLdm(0, m.IAdd); break;
                case Opcode.ldmda: RewriteLdm(0, m.ISub); break;
                case Opcode.ldmdb: RewriteLdm(-4, m.ISub); break;
                case Opcode.ldmib: RewriteLdm(4, m.IAdd); break;
                case Opcode.ldr: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Opcode.ldrt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Word32); break;
                case Opcode.ldrb: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Byte); break;
                case Opcode.ldrbt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Byte); break;
                case Opcode.ldrh: RewriteLdr(PrimitiveType.Word32, PrimitiveType.UInt16); break;
                case Opcode.ldrht: RewriteLdr(PrimitiveType.Word32, PrimitiveType.UInt16); break;
                case Opcode.ldrsb: RewriteLdr(PrimitiveType.Word32, PrimitiveType.SByte); break;
                case Opcode.ldrsbt: RewriteLdr(PrimitiveType.Word32, PrimitiveType.SByte); break;
                case Opcode.ldrsh: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Int16); break;
                case Opcode.ldrsht: RewriteLdr(PrimitiveType.Word32, PrimitiveType.Int16); break;
                case Opcode.ldrd: RewriteLdrd(); break;
                case Opcode.ldrex: RewriteLdrex(); break;
                case Opcode.lsl: case Opcode.lsls: RewriteShift(m.Shl); break;
                case Opcode.lsr: RewriteShift(m.Shr); break;
                case Opcode.nop: m.Nop(); break;
                case Opcode.mcr: RewriteMcr(); break;
                case Opcode.mcrr: RewriteMcrr(); break;
                case Opcode.mla: RewriteMultiplyAccumulate(m.IAdd); break;
                case Opcode.mls: RewriteMultiplyAccumulate(m.ISub); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.movt: RewriteMovt(); break;
                case Opcode.movw: RewriteMovw(); break;
                case Opcode.mrc: RewriteMrc(); break;
                case Opcode.mrrc: RewriteMrrc(); break;
                case Opcode.mrs: RewriteMrs(); break;
                case Opcode.msr: RewriteMsr(); break;
                case Opcode.mul: RewriteBinOp(m.IMul); break;
                case Opcode.mvn: RewriteUnaryOp(m.Comp); break;
                case Opcode.orn: RewriteLogical((a, b) => m.Or(a, m.Comp(b))); break;
                case Opcode.orr: RewriteLogical(m.Or); break;
                case Opcode.qadd: RewriteQAddSub(m.IAdd); break;
                case Opcode.qadd16: RewriteVectorBinOp("__qadd_{0}", ArmVectorData.S16); break;
                case Opcode.qadd8: RewriteVectorBinOp("__qadd_{0}", ArmVectorData.S8); break;
                case Opcode.qdadd: RewriteQDAddSub(m.IAdd); break;
                case Opcode.qdsub: RewriteQDAddSub(m.ISub); break;
                case Opcode.qsub: RewriteQAddSub(m.ISub); break;
                case Opcode.qsub16: RewriteVectorBinOp("__qsub_{0}", ArmVectorData.S16); break;
                case Opcode.pld: RewritePld(); break;
                case Opcode.pop: RewritePop(); break;
                case Opcode.push: RewritePush(); break;
                case Opcode.ror: RewriteShift(Ror); break;
                case Opcode.rrx: RewriteShift(Rrx); break;
                case Opcode.rev: RewriteRev(); break;
                case Opcode.rsb: RewriteRevBinOp(m.ISub, instr.SetFlags); break;
                case Opcode.rsc: RewriteAdcSbc(m.ISub, true); break;
                case Opcode.sadd16: RewriteVectorBinOp("__sadd_{0}", ArmVectorData.S16); break;
                case Opcode.sadd8: RewriteVectorBinOp("__sadd_{0}", ArmVectorData.S8); break;
                case Opcode.sbc: RewriteAdcSbc(m.ISub, false); break;
                case Opcode.sbfx: RewriteSbfx(); break;
                case Opcode.sdiv: RewriteDiv(m.SDiv); break;
                case Opcode.setend: RewriteSetend(); break;
                case Opcode.shsub16: RewriteVectorBinOp("__shsub_{0}", ArmVectorData.S16); break;
                case Opcode.shsub8: RewriteVectorBinOp("__shsub_{0}", ArmVectorData.S8); break;
                case Opcode.smc: RewriteSmc(); break;
                case Opcode.smlabb: RewriteMla(false, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlabt: RewriteMla(false, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlad: RewriteMxd(false, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Opcode.smladx: RewriteMxd(true, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Opcode.smlalbb: RewriteMlal(false, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlalbt: RewriteMlal(false, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlald: RewriteMlxd(false, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Opcode.smlaldx: RewriteMlxd(true, PrimitiveType.Int16, m.SMul, m.IAdd); break;
                case Opcode.smlaltb: RewriteMlal(true, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlaltt: RewriteMlal(true, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlal: RewriteSmlal(); break;
                case Opcode.smlatb: RewriteMla(true, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlatt: RewriteMla(true, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smlawb: RewriteSmlaw(false); break;
                case Opcode.smlawt: RewriteSmlaw(true); break;
                case Opcode.smlsd: RewriteMxd(false, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Opcode.smlsdx: RewriteMxd(true, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Opcode.smlsld: RewriteMlxd(false, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Opcode.smlsldx: RewriteMlxd(true, PrimitiveType.Int16, m.SMul, m.ISub); break;
                case Opcode.smmla: RewriteSmml(m.IAdd); break;
                case Opcode.smmls: RewriteSmml(m.ISub); break;
                case Opcode.smmul: RewriteSmmul(); break;
                case Opcode.smulbb: RewriteMulbb(false, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smulbt: RewriteMulbb(false, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smulwb: RewriteMulw(false); break;
                case Opcode.smulwt: RewriteMulw(true); break;
                case Opcode.smultb: RewriteMulbb(true, false, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smultt: RewriteMulbb(true, true, PrimitiveType.Int16, m.SMul); break;
                case Opcode.smull: RewriteMull(PrimitiveType.Int64, m.SMul); break;
                case Opcode.smusd: RewriteSmusd(); break;
                case Opcode.ssat: RewriteSsat(); break;
                case Opcode.ssub16: RewriteVectorBinOp("__ssub16", ArmVectorData.S16); break;
                case Opcode.ssub8: RewriteVectorBinOp("__ssub8", ArmVectorData.S8); break;
                case Opcode.stc2l: RewriteStc("__stc2l"); break;
                case Opcode.stc2: RewriteStc("__stc2"); break;
                case Opcode.stc: RewriteStc("__stc"); break;
                case Opcode.stcl: RewriteStc("__stcl"); break;
                case Opcode.stm: RewriteStm(true, true); break;
                case Opcode.stmdb: RewriteStm(false, false); break;
                case Opcode.stmda: RewriteStm(false, true); break;
                case Opcode.stmib: RewriteStm(true, false); break;
                case Opcode.str: RewriteStr(PrimitiveType.Word32); break;
                case Opcode.strb: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.strbt: RewriteStr(PrimitiveType.Byte); break;
                case Opcode.strd: RewriteStrd(); break;
                case Opcode.strex: RewriteStrex(); break;
                case Opcode.strh: RewriteStr(PrimitiveType.UInt16); break;
                case Opcode.strht: RewriteStr(PrimitiveType.UInt16); break;
                case Opcode.strt: RewriteStr(PrimitiveType.Word32); break;
                case Opcode.sub: RewriteBinOp(m.ISub); break;
                case Opcode.subw: RewriteSubw(); break;
                case Opcode.svc: RewriteSvc(); break;
                case Opcode.swp: RewriteSwp(PrimitiveType.Word32); break;
                case Opcode.swpb: RewriteSwp(PrimitiveType.Byte); break;
                case Opcode.sxtab: RewriteXtab(PrimitiveType.SByte); break;
                case Opcode.sxtah: RewriteXtab(PrimitiveType.Int16); break;
                case Opcode.sxtb: RewriteXtb(PrimitiveType.SByte, PrimitiveType.Int32); break;
                case Opcode.sxth: RewriteXtb(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Opcode.tbb: RewriteTableBranch(PrimitiveType.Byte); break;
                case Opcode.tbh: RewriteTableBranch(PrimitiveType.Word16); break;
                case Opcode.teq: RewriteTeq(); break;
                case Opcode.trap: RewriteTrap(); break;
                case Opcode.tst: RewriteTst(); break;
                case Opcode.uadd16: RewriteVectorBinOp("__uadd_{0}", ArmVectorData.I16); break;
                case Opcode.uadd8: RewriteVectorBinOp("__uadd_{0}", ArmVectorData.I8); break;
                case Opcode.uasx: RewriteUasx(); break;
                case Opcode.ubfx: RewriteUbfx(); break;
                case Opcode.udf: RewriteUdf(); break;
                case Opcode.udiv: RewriteDiv(m.UDiv); break;
                case Opcode.umaal: RewriteUmaal(); break;
                case Opcode.umlal: RewriteUmlal(); break;
                case Opcode.umull: RewriteMull(PrimitiveType.UInt64, m.UMul); break;
                case Opcode.uqasx: RewriteVectorBinOp("__uqasx_{0}", ArmVectorData.U16); break;
                case Opcode.usat: RewriteUsat(); break;
                case Opcode.usax: RewriteUsax(); break;
                case Opcode.usub16: RewriteVectorBinOp("__usub_{0}", ArmVectorData.I16); break;
                case Opcode.usub8: RewriteVectorBinOp("__usub_{0}", ArmVectorData.I8); break;
                case Opcode.uxtab: RewriteXtab(PrimitiveType.Byte); break;
                case Opcode.uxtah: RewriteXtab(PrimitiveType.UInt16); break;
                case Opcode.uxtb: RewriteXtb(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Opcode.uxth: RewriteXtb(PrimitiveType.UInt16, PrimitiveType.UInt32); break;
                case Opcode.wfi: RewriteWfi(); break;
                case Opcode.yield: RewriteYield(); break;


                case Opcode.vabs: RewriteVectorUnaryOp("__vabs_{0}"); break;
                case Opcode.vadd: RewriteVectorBinOp("__vadd_{0}"); break;
                case Opcode.vaddl: RewriteVectorBinOp("__vaddl_{0}"); break;
                case Opcode.vand: RewriteVecBinOp(m.And); break;
                case Opcode.vcmp: RewriteVcmp(); break;
                case Opcode.vceq: RewriteVectorBinOp("__vceq_{0}"); break;
                case Opcode.vcge: RewriteVectorBinOp("__vcge_{0}"); break;
                case Opcode.vcgt: RewriteVectorBinOp("__vcgt_{0}"); break;
                case Opcode.vcle: RewriteVectorBinOp("__vcle_{0}"); break;
                case Opcode.vclt: RewriteVectorBinOp("__vclt_{0}"); break;
                case Opcode.vcmpe: RewriteVcmp(); break;
                case Opcode.vcvt: RewriteVcvt(); break;
                case Opcode.vcvtr: RewriteVcvtr(); break;
                case Opcode.vdiv: RewriteVecBinOp(m.FDiv); break;
                case Opcode.vdup: RewriteVdup(); break;
                case Opcode.veor: RewriteVecBinOp(m.Xor); break;
                case Opcode.vext: RewriteVext(); break;
                case Opcode.vhadd: RewriteVectorBinOp("__vhadd_{0}"); break;
                case Opcode.vhsub: RewriteVectorBinOp("__vhsub_{0}"); break;
                case Opcode.vldmia: RewriteVldmia(); break;
                case Opcode.vldr: RewriteVldr(); break;
                case Opcode.vmax: RewriteVectorBinOp("__vmax_{0}"); break;
                case Opcode.vmin: RewriteVectorBinOp("__vmin_{0}"); break;
                case Opcode.vmov: RewriteVmov(); break;
                case Opcode.vmla: RewriteVectorBinOp("__vmla_{0}"); break;
                case Opcode.vmls: RewriteVectorBinOp("__vmls_{0}"); break;
                case Opcode.vmlal: RewriteVectorBinOp("__vmlal_{0}"); break;
                case Opcode.vmlsl: RewriteVectorBinOp("__vmlsl_{0}"); break;
                case Opcode.vmrs: RewriteVmrs(); break;
                case Opcode.vmvn: RewriteVmvn(); break;
                case Opcode.vmul: RewriteVectorBinOp("__vmul_{0}"); break;
                case Opcode.vorr: RewriteVecBinOp(m.Or); break;
                case Opcode.vneg: RewriteVectorUnaryOp("__vneg_{0}"); break;
                case Opcode.vnmla: RewriteVectorBinOp("__vnmla_{0}"); break;
                case Opcode.vnmls: RewriteVectorBinOp("__vnmls_{0}"); break;
                case Opcode.vnmul: RewriteVectorBinOp("__vnmul_{0}"); break;
                case Opcode.vpadd: RewriteVectorBinOp("__vpadd_{0}"); break;
                case Opcode.vpmax: RewriteVectorBinOp("__vpmax_{0}"); break;
                case Opcode.vpmin: RewriteVectorBinOp("__vpmin_{0}"); break;
                case Opcode.vpop: RewriteVpop(); break;
                case Opcode.vpush: RewriteVpush(); break;
                case Opcode.vqabs: RewriteVectorBinOp("__vqabs_{0}"); break;
                case Opcode.vqadd: RewriteVectorBinOp("__vqadd_{0}"); break;
                case Opcode.vqshl: RewriteVectorBinOp("__vqshl_{0}"); break;
                case Opcode.vrshl: RewriteVectorBinOp("__vrshl_{0}"); break;
                case Opcode.vrshr: RewriteVectorBinOp("__vrshr_{0}"); break;
                case Opcode.vstm: RewriteVstmia(true, false); break;
                case Opcode.vstmdb: RewriteVstmia(false, true); break;
                case Opcode.vstmia: RewriteVstmia(true, true); break;
                case Opcode.vsqrt: RewriteVsqrt(); break;
                case Opcode.vshl: RewriteVectorBinOp("__vshl_{0}"); break;
                case Opcode.vshr: RewriteVectorBinOp("__vshr_{0}"); break;
                case Opcode.vsra: RewriteVectorBinOp("__vsra_{0}"); break;
                case Opcode.vstr: RewriteVstr(); break;
                case Opcode.vsub: RewriteVectorBinOp("__vsub_{0}"); break;
                }
                yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
                {
                    Class = rtlClass,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Invalid()
        {
            this.rtlClass = InstrClass.Invalid;
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
            if (Dst() is AddressOperand aOp)
            {
                dst = aOp.Address;
                dstIsAddress = true;
            }
            else
            {
                dst = Operand(Dst(), PrimitiveType.Word32, true);
                dstIsAddress = false;
            }
            if (link)
            {
                rtlClass = InstrClass.Transfer | InstrClass.Call;
                if (instr.condition == ArmCondition.AL)
                {
                    m.Call(dst, 0);
                }
                else
                {
                    rtlClass = InstrClass.ConditionalTransfer | InstrClass.Call;
                    ConditionalSkip(true);
                    m.Call(dst, 0);
                }
            }
            else
            {
                if (instr.condition == ArmCondition.AL)
                {
                    rtlClass = InstrClass.Transfer;
                    if (Dst() is RegisterOperand rop && rop.Register == Registers.lr)
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
                    rtlClass = InstrClass.ConditionalTransfer;
                    if (dstIsAddress)
                    {
                        m.Branch(TestCond(instr.condition), (Address) dst, rtlClass);
                    }
                    else
                    {
                        ConditionalSkip(true);
                        m.Goto(dst);
                    }
                }
            }
        }

        void RewriteCbnz(Func<Expression, Expression> ctor)
        {
            rtlClass = InstrClass.ConditionalTransfer;
            var cond = Operand(Dst(), PrimitiveType.Word32, true);
            m.Branch(ctor(Operand(Dst())),
                    ((AddressOperand) Src1()).Address,
                    InstrClass.ConditionalTransfer);
        }

        // If a conditional ARM instruction is encountered, generate an IL
        // instruction to skip the remainder of the instruction cluster.
        protected virtual void ConditionalSkip(bool force)
        {
            var cc = instr.condition;
            if (!force)
            {
                if (cc == ArmCondition.AL)
                    return; // never skip!
                if (instr.opcode == Opcode.b ||
                    instr.opcode == Opcode.bl ||
                    instr.opcode == Opcode.blx ||
                    instr.opcode == Opcode.bx)
                {
                    // These instructions handle the branching themselves.
                    return;
                }
            }
            m.BranchInMiddleOfInstruction(
                TestCond(Invert(cc)),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
        }

        Expression EffectiveAddress(MemoryOperand mem)
        {
            var baseReg = Reg(mem.BaseRegister);
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

        Expression Operand(MachineOperand op, PrimitiveType dt = null, bool write = false)
        {
            dt = dt ?? PrimitiveType.Word32;
            switch (op)
            {
            case RegisterOperand rOp:
                {
                    if (!write && rOp.Register == Registers.pc)
                    {
                        return instr.Address + pcValueOffset;
                    }
                    var reg = Reg(rOp.Register);
                    return MaybeShiftOperand(reg, op);
                }
            case ImmediateOperand iOp:
                return iOp.Value;
            //case ARM_OP_CIMM:
            //    return m.Byte((uint8_t)op.imm);
            //case ARM_OP_PIMM:
            //    return host->EnsureRegister(2, op.imm);
            case MemoryOperand mop:
                {
                    var baseReg = Reg(mop.BaseRegister);
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
                            case Opcode.Invalid:
                                ea = ireg;
                                break;
                            case Opcode.lsl:
                                ea = m.IAdd(ea, m.IMul(ireg, Constant.Int32(1 << mop.Shift)));
                                break;
                            case Opcode.lsr:
                                ea = m.IAdd(ea, m.Shr(ireg, Constant.Int32(mop.Shift)));
                                break;
                            case Opcode.asr:
                                ea = m.IAdd(ea, m.Sar(ireg, Constant.Int32(mop.Shift)));
                                break;
                            case Opcode.ror:
                                var ix = host.PseudoProcedure(PseudoProcedure.Ror, ireg.DataType, ireg, Constant.Int32(mop.Shift));
                                ea = m.IAdd(ea, ix);
                                break;
                            case Opcode.rrx:
                                var rrx = host.PseudoProcedure(PseudoProcedure.RorC, ireg.DataType, ireg, Constant.Int32(mop.Shift), C());
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
                            if (mop.ShiftType != Opcode.Invalid)
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
            switch (instr.opcode)
            {
            case Opcode.ldc: return PrimitiveType.Word32;
            case Opcode.ldc2: return PrimitiveType.Word32;
            case Opcode.ldc2l: return PrimitiveType.Word32;
            case Opcode.ldcl: return PrimitiveType.Word32;
            case Opcode.ldr: return PrimitiveType.Word32;
            case Opcode.ldrt: return PrimitiveType.Word32;
            case Opcode.ldrb: return PrimitiveType.Byte;
            case Opcode.ldrbt: return PrimitiveType.Byte;
            case Opcode.ldrd: return PrimitiveType.Word64;
            case Opcode.ldrh: return PrimitiveType.Word16;
            case Opcode.ldrht: return PrimitiveType.Word16;
            case Opcode.ldrsb: return PrimitiveType.SByte;
            case Opcode.ldrsbt: return PrimitiveType.SByte;
            case Opcode.ldrsh: return PrimitiveType.Int16;
            case Opcode.ldrsht: return PrimitiveType.Int16;
            case Opcode.stc: return PrimitiveType.Word32;
            case Opcode.stc2: return PrimitiveType.Word32;
            case Opcode.stc2l: return PrimitiveType.Word32;
            case Opcode.stcl: return PrimitiveType.Word32;
            case Opcode.str: return PrimitiveType.Word32;
            case Opcode.strt: return PrimitiveType.Word32;
            case Opcode.strb: return PrimitiveType.Byte;
            case Opcode.strbt: return PrimitiveType.Byte;
            case Opcode.strd: return PrimitiveType.Word64;
            case Opcode.strh: return PrimitiveType.Word16;
            case Opcode.strht: return PrimitiveType.Word16;
            case Opcode.swp: return PrimitiveType.Word32;
            case Opcode.swpb: return PrimitiveType.Byte;
            case Opcode.vldr: return Dst().Width;
            case Opcode.vstr: return Dst().Width;
            }
            return VoidType.Instance;
        }


        Expression MaybeShiftOperand(Expression exp, MachineOperand op)
        {
            if (op != instr.ops[instr.ops.Length - 1])
                return exp;
            if (instr.ShiftType == Opcode.Invalid)
                return exp;

            Expression sh;
            if (instr.ShiftValue is RegisterOperand reg)
                sh = binder.EnsureRegister(reg.Register);
            else
                sh = ((ImmediateOperand) instr.ShiftValue).Value;

            return MaybeShiftExpression(exp, sh, instr.ShiftType);
        }

        private Expression MaybeShiftExpression(Expression exp, Expression sh, Opcode shiftType)
        {
            switch (shiftType)
            {
            case Opcode.asr: return m.Sar(exp, sh);
            case Opcode.lsl: return m.Shl(exp, sh);
            case Opcode.lsr: return m.Sar(exp, sh);
            case Opcode.ror: return host.PseudoProcedure(PseudoProcedure.Ror, exp.DataType, exp, sh);
            case Opcode.rrx:
                return host.PseudoProcedure(PseudoProcedure.RorC, exp.DataType, exp, sh, C());
            default: return exp;
            }
        }

        void MaybePostOperand(MachineOperand op)
        {
            if (!(op is MemoryOperand mop))
                return;
            if (!instr.Writeback || mop.PreIndex)
                return;
            var baseReg = Reg(mop.BaseRegister);

            Expression idx = null;
            var offset = mop.Offset;
            if (offset != null && !offset.IsIntegerZero)
            {
                idx = offset;
            }
            else if (mop.Index != null)
            {
                idx = binder.EnsureRegister(mop.Index);
                if (mop.ShiftType != Opcode.Invalid)
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

        protected Expression TestCond(ArmCondition cond)
        {
            switch (cond)
            {
            //default:
            //	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
            case ArmCondition.HS:
                return m.Test(ConditionCode.UGE, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.LO:
                return m.Test(ConditionCode.ULT, FlagGroup(FlagM.CF, "C", PrimitiveType.Byte));
            case ArmCondition.EQ:
                return m.Test(ConditionCode.EQ, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
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
                return m.Test(ConditionCode.LT, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.PL:
                return m.Test(ConditionCode.GE, FlagGroup(FlagM.NF, "N", PrimitiveType.Byte));
            case ArmCondition.NE:
                return m.Test(ConditionCode.NE, FlagGroup(FlagM.ZF, "Z", PrimitiveType.Byte));
            case ArmCondition.VC:
                return m.Test(ConditionCode.NO, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            case ArmCondition.VS:
                return m.Test(ConditionCode.OV, FlagGroup(FlagM.VF, "V", PrimitiveType.Byte));
            }
            return null;
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
            var dst = Operand(Dst(), PrimitiveType.Word32, true);
            var intrinsic = host.PseudoProcedure(fnName, type,
                Operand(Src1()),
                Operand(Src2()));
            m.Assign(dst, intrinsic);
        }

        private static HashSet<Opcode> opcode_seen = new HashSet<Opcode>();

        void EmitUnitTest(AArch32Instruction instr)
        {
            if (opcode_seen.Contains(instr.opcode))
                return;
            opcode_seen.Add(instr.opcode);

            var r2 = rdr.Clone();
            r2.Offset -= instr.Length;

            Console.WriteLine("        [Test]");
            Console.WriteLine("        public void ArmRw_{0}()", instr.opcode);
            Console.WriteLine("        {");

            if (instr.Length > 2)
            {
                var wInstr = r2.ReadBeUInt32();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X8}\");");
            }
            else
            {
                var wInstr = r2.ReadBeUInt16();
                Console.WriteLine($"            RewriteCode(\"{wInstr:X4}\");");
            }
            Console.WriteLine("            AssertCode(");
            Console.WriteLine($"                \"0|L--|00100000({instr.Length}): 1 instructions\",");
            Console.WriteLine($"                \"1|L--|@@@\");");
            Console.WriteLine("        }");
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
