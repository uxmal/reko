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

using System.Collections;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;

namespace Reko.Arch.NatSemi;

public class Ns32kRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly Ns32kArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<Ns32kInstruction> dasm;
    private readonly RtlEmitter m;
    private readonly List<RtlInstruction> rtls;
    private Ns32kInstruction instr = default!;
    private InstrClass iclass;

    public Ns32kRewriter(Ns32kArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new Ns32kDisassembler(arch, rdr).GetEnumerator();
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            instr = dasm.Current;
            iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            case Mnemonic.absb: RewriteAbs(PrimitiveType.Byte); break;
            case Mnemonic.absd: RewriteAbs(PrimitiveType.Word32); break;
            case Mnemonic.absf: RewriteAbsf(); break;
            case Mnemonic.absl: RewriteAbsl(); break;
            case Mnemonic.absw: RewriteAbs(PrimitiveType.Word16); break;
            case Mnemonic.acbb: RewriteAcb(PrimitiveType.Byte); break;
            case Mnemonic.acbd: RewriteAcb(PrimitiveType.Word32); break;
            case Mnemonic.acbw: RewriteAcb(PrimitiveType.Word16); break;
            case Mnemonic.addb: RewriteAdd(PrimitiveType.Byte); break;
            case Mnemonic.addcb: RewriteAddc(PrimitiveType.Byte); break;
            case Mnemonic.addcd: RewriteAddc(PrimitiveType.Word32); break;
            case Mnemonic.addcw: RewriteAddc(PrimitiveType.Word16); break;
            case Mnemonic.addd: RewriteAdd(PrimitiveType.Word32); break;
            case Mnemonic.addf: RewriteAddf(PrimitiveType.Real32); break;
            case Mnemonic.addl: RewriteAddf(PrimitiveType.Real64); break;
            case Mnemonic.addpb: RewriteAddp(PrimitiveType.Byte); break;
            case Mnemonic.addpd: RewriteAddp(PrimitiveType.Word32); break;
            case Mnemonic.addpw: RewriteAddp(PrimitiveType.Word16); break;
            case Mnemonic.addqb: RewriteAddq(PrimitiveType.Byte); break;
            case Mnemonic.addqd: RewriteAddq(PrimitiveType.Word32); break;
            case Mnemonic.addqw: RewriteAddq(PrimitiveType.Word16); break;
            case Mnemonic.addr: RewriteAddr(); break;
            case Mnemonic.addw: RewriteAdd(PrimitiveType.Word16); break;
            case Mnemonic.adjspb: RewriteAdjsp(PrimitiveType.Byte); break;
            case Mnemonic.adjspd: RewriteAdjsp(PrimitiveType.Word32); break;
            case Mnemonic.adjspw: RewriteAdjsp(PrimitiveType.Word16); break;
            case Mnemonic.andb: RewriteAnd(PrimitiveType.Byte); break;
            case Mnemonic.andd: RewriteAnd(PrimitiveType.Word32); break;
            case Mnemonic.andw: RewriteAnd(PrimitiveType.Word16); break;
            case Mnemonic.ashb: RewriteAsh(PrimitiveType.Byte); break;
            case Mnemonic.ashd: RewriteAsh(PrimitiveType.Word32); break;
            case Mnemonic.ashw: RewriteAsh(PrimitiveType.Word16); break;
            case Mnemonic.bcc: RewriteBcond(Registers.C, ConditionCode.UGE); break;
            case Mnemonic.bcs: RewriteBcond(Registers.C, ConditionCode.ULT); break;
            case Mnemonic.beq: RewriteBcond(Registers.Z, ConditionCode.EQ); break;
            case Mnemonic.bfc: RewriteBcond(Registers.F, false); break;
            case Mnemonic.bfs: RewriteBcond(Registers.F, true); break;
            case Mnemonic.bge: RewriteBcond(Registers.NZ, ConditionCode.GE); break;
            case Mnemonic.bgt: RewriteBcond(Registers.N, ConditionCode.GT); break;
            case Mnemonic.bhi: RewriteBcond(Registers.L, ConditionCode.UGT); break;
            case Mnemonic.bhs: RewriteBcond(Registers.LZ, ConditionCode.UGE); break;
            case Mnemonic.ble: RewriteBcond(Registers.N, ConditionCode.LE); break;
            case Mnemonic.blo: RewriteBcond(Registers.LZ, ConditionCode.ULT); break;
            case Mnemonic.bls: RewriteBcond(Registers.L, ConditionCode.ULT); break;
            case Mnemonic.blt: RewriteBcond(Registers.N, ConditionCode.LT); break;
            case Mnemonic.bne: RewriteBcond(Registers.Z, ConditionCode.EQ); break;
            case Mnemonic.bicb: RewriteBic(PrimitiveType.Byte); break;
            case Mnemonic.bicd: RewriteBic(PrimitiveType.Word32); break;
            case Mnemonic.bicpsrb: RewriteBicpsrb(); break;
            case Mnemonic.bicpsrw: RewriteBicpsrw(); break; // pr
            case Mnemonic.bicw: RewriteBic(PrimitiveType.Word16); break;
            case Mnemonic.bispsrb: RewriteBispsrb(); break;
            case Mnemonic.bispsrw: RewriteBispsrw(); break; // pr
            case Mnemonic.bpt: RewriteBpt(); break;
            case Mnemonic.br: RewriteBr(); break;
            case Mnemonic.bsr: RewriteBsr(); break;
            case Mnemonic.caseb: RewriteCase(PrimitiveType.Byte); break;
            case Mnemonic.cased: RewriteCase(PrimitiveType.Word32); break;
            case Mnemonic.casew: RewriteCase(PrimitiveType.Word16); break;
            case Mnemonic.cbitb: RewriteCbit(PrimitiveType.Byte); break;
            case Mnemonic.cbitd: RewriteCbit(PrimitiveType.Word32); break;
            case Mnemonic.cbitib: RewriteCbiti(PrimitiveType.Byte); break;
            case Mnemonic.cbitid: RewriteCbiti(PrimitiveType.Word32); break;
            case Mnemonic.cbitiw: RewriteCbiti(PrimitiveType.Word16); break;
            case Mnemonic.cbitw: RewriteCbit(PrimitiveType.Word16); break;
            case Mnemonic.checkb: RewriteCheck(PrimitiveType.Byte); break;
            case Mnemonic.checkd: RewriteCheck(PrimitiveType.Word32); break;
            case Mnemonic.checkw: RewriteCheck(PrimitiveType.Word16); break;
            case Mnemonic.cinv: RewriteCinv(); break; // pr
            case Mnemonic.cmpb: RewriteCmp(PrimitiveType.Byte); break;
            case Mnemonic.cmpd: RewriteCmp(PrimitiveType.Word32); break;
            case Mnemonic.cmpf: RewriteCmpf(PrimitiveType.Real32); break;
            case Mnemonic.cmpl: RewriteCmpf(PrimitiveType.Real64); break;
            case Mnemonic.cmpmb: RewriteCmpm(PrimitiveType.Byte); break;
            case Mnemonic.cmpmd: RewriteCmpm(PrimitiveType.Word32); break;
            case Mnemonic.cmpmw: RewriteCmpm(PrimitiveType.Word16); break;
            case Mnemonic.cmpqb: RewriteCmp(PrimitiveType.Byte); break;
            case Mnemonic.cmpqd: RewriteCmp(PrimitiveType.Word32); break;
            case Mnemonic.cmpqw: RewriteCmp(PrimitiveType.Word16); break;
            case Mnemonic.cmpsb: RewriteCmps(PrimitiveType.Byte); break;
            case Mnemonic.cmpsd: RewriteCmps(PrimitiveType.Word32); break;
            case Mnemonic.cmpst: RewriteCmpst(); break;
            case Mnemonic.cmpsw: RewriteCmps(PrimitiveType.Word16); break;
            case Mnemonic.cmpw: RewriteCmp(PrimitiveType.Word16); break;
            case Mnemonic.comb: RewriteCom(PrimitiveType.Byte); break;
            case Mnemonic.comd: RewriteCom(PrimitiveType.Word32); break;
            case Mnemonic.comw: RewriteCom(PrimitiveType.Word16); break;
            case Mnemonic.cvtp: RewriteCvtp(); break;
            case Mnemonic.cxp: RewriteCxp(); break;
            case Mnemonic.cxpd: RewriteCxpd(); break;
            case Mnemonic.deib: RewriteDei(PrimitiveType.Byte); break;
            case Mnemonic.deid: RewriteDei(PrimitiveType.Word32); break;
            case Mnemonic.deiw: RewriteDei(PrimitiveType.Word16); break;
            case Mnemonic.dia: RewriteDia(); break;
            case Mnemonic.divb: RewriteDiv(PrimitiveType.Byte); break;
            case Mnemonic.divd: RewriteDiv(PrimitiveType.Word32); break;
            case Mnemonic.divf: RewriteDivf(PrimitiveType.Real32); break;
            case Mnemonic.divl: RewriteDivf(PrimitiveType.Real64); break;
            case Mnemonic.divw: RewriteDiv(PrimitiveType.Word16); break;
            case Mnemonic.dotf: RewriteDot(PrimitiveType.Real32); break;
            case Mnemonic.dotl: RewriteDot(PrimitiveType.Real64); break;
            case Mnemonic.enter: RewriteEnter(); break;
            case Mnemonic.exit: RewriteExit(); break;
            case Mnemonic.extb: RewriteExt(PrimitiveType.Byte); break;
            case Mnemonic.extd: RewriteExt(PrimitiveType.Word32); break;
            case Mnemonic.extsb: RewriteExt(PrimitiveType.Byte); break;
            case Mnemonic.extsd: RewriteExt(PrimitiveType.Word32); break;
            case Mnemonic.extsw: RewriteExt(PrimitiveType.Word16); break;
            case Mnemonic.extw: RewriteExt(PrimitiveType.Word16); break;
            case Mnemonic.ffsb: RewriteFfs(PrimitiveType.Byte); break;
            case Mnemonic.ffsd: RewriteFfs(PrimitiveType.Word32); break;
            case Mnemonic.ffsw: RewriteFfs(PrimitiveType.Word16); break;
            case Mnemonic.flag: RewriteFlag(); break;
            case Mnemonic.floorfb: RewriteFloorf(PrimitiveType.Int8); break;
            case Mnemonic.floorfd: RewriteFloorf(PrimitiveType.Int32); break;
            case Mnemonic.floorfw: RewriteFloorf(PrimitiveType.Int16); break;
            case Mnemonic.floorlb: RewriteFloorl(PrimitiveType.Int8); break;
            case Mnemonic.floorld: RewriteFloorl(PrimitiveType.Int32); break;
            case Mnemonic.floorlw: RewriteFloorl(PrimitiveType.Int16); break;
            case Mnemonic.ibitb: RewriteIbit(PrimitiveType.Byte); break;
            case Mnemonic.ibitd: RewriteIbit(PrimitiveType.Word32); break;
            case Mnemonic.ibitw: RewriteIbit(PrimitiveType.Word16); break;
            case Mnemonic.indexb: RewriteIndex(PrimitiveType.Byte); break;
            case Mnemonic.indexd: RewriteIndex(PrimitiveType.Word32); break;
            case Mnemonic.indexw: RewriteIndex(PrimitiveType.Word16); break;
            case Mnemonic.insb: RewriteIns(PrimitiveType.Byte); break;
            case Mnemonic.insd: RewriteIns(PrimitiveType.Word32); break;
            case Mnemonic.inssb: RewriteIns(PrimitiveType.Byte); break;
            case Mnemonic.inssd: RewriteIns(PrimitiveType.Word32); break;
            case Mnemonic.inssw: RewriteIns(PrimitiveType.Word16); break;
            case Mnemonic.insw: RewriteIns(PrimitiveType.Word16); break;
            case Mnemonic.jsr: RewriteJsr(); break;
            case Mnemonic.jump: RewriteJump(); break;
            case Mnemonic.lfsr: RewriteLfsr(); break;
            case Mnemonic.lmr: RewriteLmr(); break;
            case Mnemonic.logbf: RewriteLogb(PrimitiveType.Real32); break;
            case Mnemonic.logbl: RewriteLogb(PrimitiveType.Real64); break;
            case Mnemonic.lprb: RewriteLpr(PrimitiveType.Byte); break;
            case Mnemonic.lprd: RewriteLpr(PrimitiveType.Word32); break; // pr
            case Mnemonic.lprw: RewriteLpr(PrimitiveType.Word16); break;
            case Mnemonic.lshb: RewriteLsh(PrimitiveType.Byte); break;
            case Mnemonic.lshd: RewriteLsh(PrimitiveType.Word32); break;
            case Mnemonic.lshw: RewriteLsh(PrimitiveType.Word16); break;
            case Mnemonic.meib: RewriteMei(PrimitiveType.Byte); break;
            case Mnemonic.meid: RewriteMei(PrimitiveType.Word32); break;
            case Mnemonic.meiw: RewriteMei(PrimitiveType.Word16); break;
            case Mnemonic.modb: RewriteMod(PrimitiveType.Byte); break;
            case Mnemonic.modd: RewriteMod(PrimitiveType.Word32); break;
            case Mnemonic.modw: RewriteMod(PrimitiveType.Word16); break;
            case Mnemonic.movb: RewriteMov(PrimitiveType.Byte); break;
            case Mnemonic.movbf: RewriteMovb(PrimitiveType.Real32); break;
            case Mnemonic.movbl: RewriteMovb(PrimitiveType.Real64); break;
            case Mnemonic.movd: RewriteMov(PrimitiveType.Word32); break;
            case Mnemonic.movdf: RewriteMovd(PrimitiveType.Real32); break;
            case Mnemonic.movdl: RewriteMovd(PrimitiveType.Real64); break;
            case Mnemonic.movf: RewriteMovf(PrimitiveType.Real32); break;
            case Mnemonic.movfl: RewriteMovfl(); break;
            case Mnemonic.movl: RewriteMovf(PrimitiveType.Real64); break;
            case Mnemonic.movlf: RewriteMovlf(); break;
            case Mnemonic.movmb: RewriteMovm(PrimitiveType.Byte); break;
            case Mnemonic.movmd: RewriteMovm(PrimitiveType.Word32); break;
            case Mnemonic.movmw: RewriteMovm(PrimitiveType.Word16); break;
            case Mnemonic.movqb: RewriteMov(PrimitiveType.Byte); break;
            case Mnemonic.movqd: RewriteMov(PrimitiveType.Word32); break;
            case Mnemonic.movqw: RewriteMov(PrimitiveType.Word16); break;
            case Mnemonic.movsb: RewriteMovs(PrimitiveType.Byte); break;
            case Mnemonic.movsd: RewriteMovs(PrimitiveType.Word32); break;
            case Mnemonic.movst: RewriteMovst(); break;
            case Mnemonic.movsub: RewriteMovsu(PrimitiveType.Byte); break;
            case Mnemonic.movsud: RewriteMovsu(PrimitiveType.Word32); break;
            case Mnemonic.movsuw: RewriteMovsu(PrimitiveType.Word16); break;
            case Mnemonic.movsw: RewriteMovs(PrimitiveType.Word16); break;
            case Mnemonic.movusb: RewriteMovus(PrimitiveType.Byte); break;
            case Mnemonic.movusd: RewriteMovus(PrimitiveType.Word32); break;
            case Mnemonic.movusw: RewriteMovus(PrimitiveType.Word16); break;
            case Mnemonic.movw: RewriteMov(PrimitiveType.Word16); break;
            case Mnemonic.movwf: RewriteMovwf(PrimitiveType.Real32); break;
            case Mnemonic.movwl: RewriteMovwf(PrimitiveType.Real64); break;
            case Mnemonic.movxbd: RewriteMovxbd(); break;
            case Mnemonic.movxbw: RewriteMovxbw(); break;
            case Mnemonic.movxwd: RewriteMovxwd(); break;
            case Mnemonic.movzbd: RewriteMovzbd(); break;
            case Mnemonic.movzbw: RewriteMovzbw(); break;
            case Mnemonic.movzwd: RewriteMovzwd(); break;
            case Mnemonic.mulb: RewriteMul(PrimitiveType.Byte); break;
            case Mnemonic.muld: RewriteMul(PrimitiveType.Word32); break;
            case Mnemonic.mulf: RewriteMulf(PrimitiveType.Real32); break;
            case Mnemonic.mull: RewriteMulf(PrimitiveType.Real64); break;
            case Mnemonic.mulw: RewriteMul(PrimitiveType.Word16); break;
            case Mnemonic.negb: RewriteNeg(PrimitiveType.Byte); break;
            case Mnemonic.negd: RewriteNeg(PrimitiveType.Word32); break;
            case Mnemonic.negf: RewriteNegf(PrimitiveType.Real32); break;
            case Mnemonic.negl: RewriteNegf(PrimitiveType.Real64); break;
            case Mnemonic.negw: RewriteNeg(PrimitiveType.Word16); break;
            case Mnemonic.nop: RewriteNop(); break;
            case Mnemonic.notb: RewriteNot(PrimitiveType.Byte); break;
            case Mnemonic.notd: RewriteNot(PrimitiveType.Word32); break;
            case Mnemonic.notw: RewriteNot(PrimitiveType.Word16); break;
            case Mnemonic.orb: RewriteOr(PrimitiveType.Byte); break;
            case Mnemonic.ord: RewriteOr(PrimitiveType.Word32); break;
            case Mnemonic.orw: RewriteOr(PrimitiveType.Word16); break;
            case Mnemonic.polyf: RewritePolyf(PrimitiveType.Real32); break;
            case Mnemonic.polyl: RewritePolyf(PrimitiveType.Real64); break;
            case Mnemonic.quob: RewriteQuo(PrimitiveType.Byte); break;
            case Mnemonic.quod: RewriteQuo(PrimitiveType.Word32); break;
            case Mnemonic.quow: RewriteQuo(PrimitiveType.Word16); break;
            case Mnemonic.rdval: RewriteRdval(); break;
            case Mnemonic.remb: RewriteRem(PrimitiveType.Byte); break;
            case Mnemonic.remd: RewriteRem(PrimitiveType.Word32); break;
            case Mnemonic.remw: RewriteRem(PrimitiveType.Word16); break;
            case Mnemonic.restore: RewriteRestore(); break;
            case Mnemonic.ret: RewriteRet(); break;
            case Mnemonic.reti: RewriteReti(); break;
            case Mnemonic.rett: RewriteRett(); break;
            case Mnemonic.rotb: RewriteRot(PrimitiveType.Byte); break;
            case Mnemonic.rotd: RewriteRot(PrimitiveType.Word32); break;
            case Mnemonic.rotw: RewriteRot(PrimitiveType.Word16); break;
            case Mnemonic.roundfb: RewriteRoundf(PrimitiveType.Byte); break;
            case Mnemonic.roundfd: RewriteRoundf(PrimitiveType.Word32); break;
            case Mnemonic.roundfw: RewriteRoundf(PrimitiveType.Word16); break;
            case Mnemonic.roundlb: RewriteRoundl(PrimitiveType.Byte); break;
            case Mnemonic.roundld: RewriteRoundl(PrimitiveType.Word32); break;
            case Mnemonic.roundlw: RewriteRoundl(PrimitiveType.Word16); break;
            case Mnemonic.rxp: RewriteRxp(); break;
            case Mnemonic.save: RewriteSave(); break;
            case Mnemonic.sbitb: RewriteSbit(PrimitiveType.Byte); break;
            case Mnemonic.sbitd: RewriteSbit(PrimitiveType.Word32); break;
            case Mnemonic.sbiti: RewriteSbiti(); break;
            case Mnemonic.sbitib: RewriteSbiti(PrimitiveType.Byte); break;
            case Mnemonic.sbitid: RewriteSbiti(PrimitiveType.Word32); break;
            case Mnemonic.sbitiw: RewriteSbiti(PrimitiveType.Word16); break;
            case Mnemonic.sbitw: RewriteSbit(PrimitiveType.Word16); break;
            case Mnemonic.scalbf: RewriteScalbf(PrimitiveType.Real32); break;
            case Mnemonic.scalbl: RewriteScalbf(PrimitiveType.Real64); break;
            case Mnemonic.sccb: RewriteScond(Registers.C, ConditionCode.UGE, PrimitiveType.Byte); break;
            case Mnemonic.sccd: RewriteScond(Registers.C, ConditionCode.UGE, PrimitiveType.Word32); break;
            case Mnemonic.sccw: RewriteScond(Registers.C, ConditionCode.UGE, PrimitiveType.Word16); break;
            case Mnemonic.scsb: RewriteScond(Registers.C, ConditionCode.ULT, PrimitiveType.Byte); break;
            case Mnemonic.scsd: RewriteScond(Registers.C, ConditionCode.ULT, PrimitiveType.Word32); break;
            case Mnemonic.scsw: RewriteScond(Registers.C, ConditionCode.ULT, PrimitiveType.Word16); break;
            case Mnemonic.seqb: RewriteScond(Registers.Z, ConditionCode.EQ, PrimitiveType.Byte); break;
            case Mnemonic.seqw: RewriteScond(Registers.Z, ConditionCode.EQ, PrimitiveType.Word16); break;
            case Mnemonic.seqd: RewriteScond(Registers.Z, ConditionCode.EQ, PrimitiveType.Word32); break;
            case Mnemonic.setcfg: RewriteSetcfg(); break; // pr
            case Mnemonic.sfcb: RewriteScond(Registers.F, false, PrimitiveType.Byte); break;
            case Mnemonic.sfcd: RewriteScond(Registers.F, false, PrimitiveType.Word32); break;
            case Mnemonic.sfcw: RewriteScond(Registers.F, false, PrimitiveType.Word16); break;
            case Mnemonic.sfsb: RewriteScond(Registers.F, true, PrimitiveType.Byte); break;
            case Mnemonic.sfsd: RewriteScond(Registers.F, true, PrimitiveType.Word32); break;
            case Mnemonic.sfsr: RewriteSfsr(); break;
            case Mnemonic.sfsw: RewriteScond(Registers.F, true, PrimitiveType.Word16); break;
            case Mnemonic.sgeb: RewriteScond(Registers.NZ, ConditionCode.GE, PrimitiveType.Word32); break;
            case Mnemonic.sged: RewriteScond(Registers.NZ, ConditionCode.GE, PrimitiveType.Word32); break;
            case Mnemonic.sgew: RewriteScond(Registers.NZ, ConditionCode.GE, PrimitiveType.Word16); break;
            case Mnemonic.sgtb: RewriteScond(Registers.N, ConditionCode.GT, PrimitiveType.Byte); break;
            case Mnemonic.sgtd: RewriteScond(Registers.N, ConditionCode.GT, PrimitiveType.Word32); break;
            case Mnemonic.sgtw: RewriteScond(Registers.N, ConditionCode.GT, PrimitiveType.Word16); break;
            case Mnemonic.shib: RewriteScond(Registers.L, ConditionCode.UGT, PrimitiveType.Byte); break;
            case Mnemonic.shid: RewriteScond(Registers.L, ConditionCode.UGT, PrimitiveType.Word32); break;
            case Mnemonic.shiw: RewriteScond(Registers.L, ConditionCode.UGT, PrimitiveType.Word16); break;
            case Mnemonic.shsb: RewriteScond(Registers.LZ, ConditionCode.UGE, PrimitiveType.Byte); break;
            case Mnemonic.shsd: RewriteScond(Registers.LZ, ConditionCode.UGE, PrimitiveType.Word32); break;
            case Mnemonic.shsw: RewriteScond(Registers.LZ, ConditionCode.UGE, PrimitiveType.Word16); break;
            case Mnemonic.skpsb: RewriteSkps(PrimitiveType.Byte); break;
            case Mnemonic.skpsd: RewriteSkps(PrimitiveType.Word32); break;
            case Mnemonic.skpst: RewriteSkpst(); break;
            case Mnemonic.skpsw: RewriteSkps(PrimitiveType.Word16); break;
            case Mnemonic.sleb: RewriteScond(Registers.N, ConditionCode.LE, PrimitiveType.Byte); break;
            case Mnemonic.sled: RewriteScond(Registers.N, ConditionCode.LE, PrimitiveType.Word32); break;
            case Mnemonic.slew: RewriteScond(Registers.N, ConditionCode.LE, PrimitiveType.Word16); break;
            case Mnemonic.slob: RewriteScond(Registers.LZ, ConditionCode.ULT, PrimitiveType.Byte); break;
            case Mnemonic.slod: RewriteScond(Registers.LZ, ConditionCode.ULT, PrimitiveType.Word32); break;
            case Mnemonic.slow: RewriteScond(Registers.LZ, ConditionCode.ULT, PrimitiveType.Word16); break;
            case Mnemonic.slsb: RewriteScond(Registers.L, ConditionCode.ULE, PrimitiveType.Byte); break;
            case Mnemonic.slsd: RewriteScond(Registers.L, ConditionCode.ULE, PrimitiveType.Word32); break;
            case Mnemonic.slsw: RewriteScond(Registers.L, ConditionCode.ULE, PrimitiveType.Word16); break;
            case Mnemonic.sltb: RewriteScond(Registers.NZ, ConditionCode.LT, PrimitiveType.Byte); break;
            case Mnemonic.sltd: RewriteScond(Registers.NZ, ConditionCode.LT, PrimitiveType.Word32); break;
            case Mnemonic.sltw: RewriteScond(Registers.NZ, ConditionCode.LT, PrimitiveType.Word16); break;
            case Mnemonic.smr: RewriteSmr(); break;
            case Mnemonic.sneb: RewriteScond(Registers.Z, ConditionCode.NE, PrimitiveType.Byte); break;
            case Mnemonic.sned: RewriteScond(Registers.Z, ConditionCode.NE, PrimitiveType.Word32); break;
            case Mnemonic.snew: RewriteScond(Registers.Z, ConditionCode.NE, PrimitiveType.Word16); break;
            case Mnemonic.sprb: RewriteSpr(PrimitiveType.Byte); break;
            case Mnemonic.sprd: RewriteSpr(PrimitiveType.Word32); break; // pr
            case Mnemonic.sprw: RewriteSpr(PrimitiveType.Word16); break;
            case Mnemonic.subb: RewriteSub(PrimitiveType.Byte); break;
            case Mnemonic.subcb: RewriteSubc(PrimitiveType.Byte); break;
            case Mnemonic.subcd: RewriteSubc(PrimitiveType.Word32); break;
            case Mnemonic.subcw: RewriteSubc(PrimitiveType.Word16); break;
            case Mnemonic.subd: RewriteSub(PrimitiveType.Word32); break;
            case Mnemonic.subf: RewriteSubf(PrimitiveType.Real32); break;
            case Mnemonic.subl: RewriteSubf(PrimitiveType.Real64); break;
            case Mnemonic.subpb: RewriteSubp(PrimitiveType.Byte); break;
            case Mnemonic.subpd: RewriteSubp(PrimitiveType.Word32); break;
            case Mnemonic.subpw: RewriteSubp(PrimitiveType.Word16); break;
            case Mnemonic.subw: RewriteSub(PrimitiveType.Word16); break;
            case Mnemonic.svc: RewriteSvc(); break;
            case Mnemonic.tbitb: RewriteTbit(PrimitiveType.Byte); break;
            case Mnemonic.tbitd: RewriteTbit(PrimitiveType.Word32); break;
            case Mnemonic.tbitw: RewriteTbit(PrimitiveType.Word16); break;
            case Mnemonic.truncfb: RewriteTruncf(PrimitiveType.Byte); break;
            case Mnemonic.truncfd: RewriteTruncf(PrimitiveType.Word32); break;
            case Mnemonic.truncfw: RewriteTruncf(PrimitiveType.Word16); break;
            case Mnemonic.trunclb: RewriteTruncl(PrimitiveType.Byte); break;
            case Mnemonic.truncld: RewriteTruncl(PrimitiveType.Word32); break;
            case Mnemonic.trunclw: RewriteTruncl(PrimitiveType.Word16); break;
            case Mnemonic.wait: RewriteWait(); break;
            case Mnemonic.wrval: RewriteWrval(); break;
            case Mnemonic.xorb: RewriteXor(PrimitiveType.Byte); break;
            case Mnemonic.xord: RewriteXor(PrimitiveType.Word32); break;
            case Mnemonic.xorw: RewriteXor(PrimitiveType.Word16); break;
            default:
                EmitUnitTest();
                break;
            case Mnemonic.Invalid:
                m.Invalid();
                break;
            }
            yield return new RtlInstructionCluster(instr.Address, instr.Length, rtls.ToArray())
            {
                Class = iclass
            };
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitCond(FlagGroupStorage grf, Expression e)
    {
        m.Assign(binder.EnsureFlagGroup(grf), m.Cond(grf.DataType, e));
    }

    private void EmitUnitTest()
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingRewriter("Ns32kRw", this.instr, instr.Mnemonic.ToString(), rdr, "");
        //m.Invalid();
        m.SideEffect(new StringConstant(StringType.NullTerminated(PrimitiveType.Char), $"Nyi-{instr.Mnemonic}"));
    }

    private Expression EffectiveAddress(MemoryOperand mem)
    {
        Expression ea;
        if (mem.Base is RegisterStorage b)
        {
            ea = binder.EnsureRegister(b);
        }
        else if (mem.Base is Address addr)
        {
            ea = addr;
        }
        else if (mem.Base is MemoryOperand mem2)
        {
            var ea2 = EffectiveAddress(mem2);
            ea = m.Mem(PrimitiveType.Ptr32, ea2);
        }
        else
            throw new NotImplementedException($"Unimplemented base: {mem.Base!.GetType().Name}");

        if (mem.Displacement is not null)
        {
            Expression disp = SrcOperand(mem.Displacement, PrimitiveType.Word32, false);
            if (disp is Constant c)
            {
                var d = c.ToInt32();
                if (mem.Scale > 0)
                    d *= mem.Scale;
                return m.AddSubSignedInt(ea, d);
            }
            else if (mem.Scale > 0)
            {
                disp = m.IMul(disp, mem.Scale);
            }
            ea = m.IAdd(ea, disp);
        }
        return ea;
    }

    private Expression MaybeSignExtend(Expression src, PrimitiveType dt)
    {
        if (src.DataType.BitSize < dt.BitSize)
        {
            src = m.Convert(src, src.DataType, dt);
        }
        return src;
    }

    private Identifier MaybeSlice(Identifier identifier, PrimitiveType dt)
    {
        if (identifier.DataType.BitSize <= dt.BitSize)
            return identifier;
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, m.Slice(identifier, dt));
        return tmp;
    }


    private Expression SrcOperand(int index, PrimitiveType dt, bool tosPredecrement = true)
    {
        var op = instr.Operands[index];
        return SrcOperand(op, dt, tosPredecrement);
    }

    private Expression SrcOperand(Core.Machine.MachineOperand op, PrimitiveType dt, bool tosPostDecrement = false)
    {
        switch (op)
        {
        case RegisterStorage reg:
            if (reg == Registers.TOS)
            {
                var sp = binder.EnsureRegister(Registers.SP);
                if (tosPostDecrement)
                {
                    var tmp = binder.CreateTemporary(dt);
                    m.Assign(tmp, m.Mem(dt, sp));
                    m.Assign(sp, m.IAddS(sp, dt.Size));
                    return tmp;
                }
                return m.Mem(dt, sp);
            }
            return MaybeSlice(binder.EnsureRegister(reg), dt);
        case Constant c:
            return c;
        case Address addr:
            return addr;
        case MemoryOperand mem:
            var ea = EffectiveAddress(mem);
            return m.Mem(dt, ea);
        default:
            throw new NotImplementedException($"Unimplemented operand type: {op.GetType().Name}");
        }
    }

    private Expression DstOperand(int index, Expression src, bool tosPreincrement = false)
    {
        var op = instr.Operands[index];
        switch (op)
        {
        case RegisterStorage reg:
            if (reg == Registers.TOS)
            {
                var sp = binder.EnsureRegister(Registers.SP);
                if (tosPreincrement)
                {
                    m.Assign(sp, m.ISubS(sp, src.DataType.Size));
                }
                src = EmitStore(src, sp);
                return src;
            }
            var id = binder.EnsureRegister(reg);
            if (id.DataType.BitSize > src.DataType.BitSize)
            {
                if (src is not Identifier)
                {
                    var tmp = binder.CreateTemporary(src.DataType);
                    m.Assign(tmp, src);
                    src = tmp;
                }
                m.Assign(id, m.Dpb(id, src, 0));
                return src;
            }
            else
            {
                m.Assign(id, src);
                return id;
            }
        case MemoryOperand mem:
            var ea = EffectiveAddress(mem);
            return EmitStore(src, ea);
        case Address addr:
        case Constant c:
            this.iclass = InstrClass.Invalid;
            m.Invalid();
            return src;
        default:
            throw new NotImplementedException($"Unimplemented operand type: {op.GetType().Name}");
        }
    }

    private Expression EmitStore(Expression src, Expression eaDst)
    {
        if (src is not Identifier)
        {
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            src = tmp;
        }
        m.Assign(m.Mem(src.DataType, eaDst), src);
        return src;
    }

    private void RewriteBranchTo(Expression condition, int targetIndex)
    {
        var target = SrcOperand(targetIndex, PrimitiveType.Ptr32);
        if (target is Address addr)
        {
            m.Branch(condition, addr);
            return;
        }
        m.BranchInMiddleOfInstruction(
            m.Not(condition),
            instr.Address + instr.Length,
            InstrClass.ConditionalTransfer);
        m.Goto(target);
    }

    private void RewriteAbs(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        var e = m.Fn(CommonOps.Abs, src);
        var dst = DstOperand(1, e);
        EmitCond(Registers.F, dst);
    }

    private void RewriteAbsf()
    {
        var src = SrcOperand(0, PrimitiveType.Real32);
        var e = m.Fn(FpOps.FAbs32, src);
        DstOperand(1, e);
    }

    private void RewriteAbsl()
    {
        var src = SrcOperand(0, PrimitiveType.Real64);
        var e = m.Fn(FpOps.FAbs64, src);
        var dst = DstOperand(1, e, true);
        EmitCond(Registers.F, dst);
    }

    private void RewriteAcb(PrimitiveType dt)
    {
        var src = SrcOperand(1, dt);
        var newValue = binder.CreateTemporary(dt);
        var inc = ((Constant) instr.Operands[0]).ToInt32();
        m.Assign(newValue, m.AddSubSignedInt(src, inc));
        DstOperand(1, newValue);
        RewriteBranchTo(m.Eq0(newValue), 2);
    }

    private void RewriteAdd(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var dst = DstOperand(1, m.IAdd(left, right));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteAddc(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = DstOperand(1, m.IAddC(left, right, c));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteAddf(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        DstOperand(1, m.IAdd(left, right));
    }

    private void RewriteAddp(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var dst = DstOperand(1, m.Fn(addp_intrinsic.MakeInstance(left.DataType), left, right));
        EmitCond(Registers.C, dst);
        m.Assign(binder.EnsureFlagGroup(Registers.F), 0);
    }

    private void RewriteAddq(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var dst = DstOperand(1, m.IAdd(left, right));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteAddr()
    {
        if (instr.Operands[0] is not MemoryOperand mem)
        {
            EmitUnitTest();
            m.Invalid();
            return;
        }
        var ea = EffectiveAddress(mem);
        DstOperand(1, ea);
    }

    private void RewriteAdjsp(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        src = MaybeSignExtend(src, PrimitiveType.Int32);
        var sp = binder.EnsureRegister(Registers.SP);
        m.Assign(sp, m.ISub(sp, src));
    }

    private void RewriteAnd(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.And(left, right));
    }

    private void RewriteAsh(PrimitiveType dt)
    {
        RewriteShift(ash_intrinsic, dt, m.Shl, m.Sar);
    }

    private void RewriteShift(
        IntrinsicProcedure shiftIntrinsic,
        PrimitiveType dt,
        Func<Expression,Expression,Expression> fnPositive,
        Func<Expression,Expression,Expression> fnNegative)
    {
        var shift = SrcOperand(0, PrimitiveType.Byte, true);
        var value = SrcOperand(1, dt);
        if (shift is Constant c)
        {
            int n = c.ToInt32();
            if (n == 0)
            {
                DstOperand(1, value);
                return;
            } else if (n > 0)
            {
                DstOperand(1, fnPositive(value, c));
            }
            else
            {
                DstOperand(1, fnNegative(value, c.Negate()));
            }
            return;
        }
        DstOperand(1, m.Fn(
            shiftIntrinsic.MakeInstance(
                value.DataType,
                shift.DataType),
            value,
            shift));
    }

    private void RewriteBcond(FlagGroupStorage grf, ConditionCode cc)
    {
        var flags = binder.EnsureFlagGroup(grf);
        var target = SrcOperand(0, PrimitiveType.Ptr32);
        if (target is Address addr)
        {
            m.Branch(m.Test(cc, flags), addr);
        }
        else
        {
            m.BranchInMiddleOfInstruction(
                m.Test(cc.Invert(), flags),
                instr.Address + instr.Length,
                 InstrClass.ConditionalTransfer);
            m.Goto(target);
        }
    }

    private void RewriteBcond(FlagGroupStorage grf, bool set)
    {
        Expression test = binder.EnsureFlagGroup(grf);
        if (!set)
            test = m.Not(test);
        var target = SrcOperand(0, PrimitiveType.Ptr32);
        if (target is Address)
        {
            m.Branch(test, target);
        }
        else
        {
            m.BranchInMiddleOfInstruction(
                test.Invert(),
                instr.Address + instr.Length,
                 InstrClass.ConditionalTransfer);
            m.Goto(target);
        }
    }

    private void RewriteBic(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.And(left, m.Comp(right)));
    }

    private void RewriteBicpsrb()
    {
        var b = SrcOperand(0, PrimitiveType.Byte);
        var w = m.ExtendZ(b, PrimitiveType.Word16);
        m.SideEffect(m.Fn(bicpsr_intrinsic, w));
    }

    private void RewriteBicpsrw()
    {
        var w = SrcOperand(0, PrimitiveType.Byte);
        m.SideEffect(m.Fn(bicpsr_intrinsic, w));
    }

    private void RewriteBispsrb()
    {
        var b = SrcOperand(0, PrimitiveType.Byte);
        var w = m.ExtendZ(b, PrimitiveType.Word16);
        m.SideEffect(m.Fn(bispsr_intrinsic, w));
    }

    private void RewriteBispsrw()
    {
        var w = SrcOperand(0, PrimitiveType.Byte);
        m.SideEffect(m.Fn(bispsr_intrinsic, w));
    }

    private void RewriteBpt()
    {
        m.SideEffect(m.Fn(bpt_intrinsic));
    }

    private void RewriteBr()
    {
        var target = SrcOperand(0, PrimitiveType.Word32);
        m.Goto(target);
    }

    private void RewriteBsr()
    {
        var target = SrcOperand(0, PrimitiveType.Word32);
        m.Call(target, 4);
    }

    private void RewriteCase(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        var offset = m.ExtendS(src, PrimitiveType.Int32);
        m.Goto(m.IAdd(instr.Address, offset));
    }

    private void RewriteCbit(PrimitiveType dt)
    {
        var offset = SrcOperand(0, PrimitiveType.Int8);
        var exp = SrcOperand(1, dt, false);
        var cleanbit = CommonOps.ClearBit.MakeInstance(exp.DataType, offset.DataType);
        var result = DstOperand(1, m.Fn(cleanbit, exp, offset));
        EmitCond(Registers.F, result);
    }

    private void RewriteCbiti(PrimitiveType dt)
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteCheck(PrimitiveType dt)
    {
        var src = SrcOperand(2, dt);
        var f = binder.EnsureFlagGroup(Registers.F);
        var ptrBounds = SrcOperand(0, PrimitiveType.Ptr32);
        m.Assign(f, m.Cand(
            m.Uge(src, m.Mem(dt, ptrBounds)),
            m.Ult(src, m.Mem(dt, m.IAdd(ptrBounds, dt.Size)))));
        DstOperand(0, m.ISub(src, m.Mem(dt, m.IAdd(ptrBounds, dt.Size))));
    }

    private void RewriteCinv()
    {
        var options = new StringConstant(
            StringType.NullTerminated(PrimitiveType.Char),
            ((LiteralOperand) instr.Operands[0]).ToString());
        m.SideEffect(m.Fn(cinv_intrinsic, options, SrcOperand(1, PrimitiveType.Word32)));
    }

    // pr
    private void RewriteCmp(PrimitiveType dt)
    {
        var left = SrcOperand(0, dt);
        var right = SrcOperand(1, dt);
        var result = binder.EnsureFlagGroup(Registers.LNZ);
        m.Assign(result, m.Cond(result.DataType, m.ISub(left, right)));
    }


    private void RewriteCmpf(PrimitiveType dt)
    {
        var left = SrcOperand(0, dt);
        var right = SrcOperand(1, dt);
        var result = binder.EnsureFlagGroup(Registers.NZ);
        m.Assign(result, m.Cond(result.DataType, m.ISub(left, right)));
        m.Assign(binder.EnsureFlagGroup(Registers.L), 0);
    }

    private void RewriteCmpm(PrimitiveType dt)
    {
        var lnz = binder.EnsureFlagGroup(Registers.LNZ);
        m.Assign(lnz,
            m.Fn(
                cmpm_intrinsic.MakeInstance(32, dt),
                SrcOperand(0, dt),
                SrcOperand(1, dt),
                SrcOperand(2, PrimitiveType.Word32)));
    }

    private void RewriteCmps(PrimitiveType dt)
    {
        var r0 = binder.EnsureRegister(Registers.GpRegisters[0]);
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r4 = MaybeSlice(binder.EnsureRegister(Registers.GpRegisters[4]), dt);
        var flags = instr.Operands.Length > 0
            ? ((LiteralOperand) instr.Operands[0]).ToString()
            : "";
        var addrNext = instr.Address + instr.Length;
        m.BranchInMiddleOfInstruction(m.Eq0(r0), addrNext, InstrClass.ConditionalTransfer);
        var value1 = binder.CreateTemporary(dt);
        var value2 = binder.CreateTemporary(dt);
        m.Assign(value1, m.Mem(dt, r1));
        m.Assign(value2, m.Mem(dt, r2));
        if (flags.Contains('u'))
        {
            m.BranchInMiddleOfInstruction(m.Eq(value1, value2), addrNext, InstrClass.ConditionalTransfer);
        }
        if (flags.Contains('w'))
        {
            m.BranchInMiddleOfInstruction(m.Ne(value1, value2), addrNext, InstrClass.ConditionalTransfer);
        }
        if (flags.Contains('b'))
        {
            m.Assign(r1, m.ISubS(r1, 1));
            m.Assign(r2, m.ISubS(r1, 1));
        }
        else
        {
            m.Assign(r1, m.IAddS(r1, 1));
            m.Assign(r2, m.IAddS(r1, 1));
        }
        m.Assign(r0, m.ISub(r0, 1));
        m.Goto(instr.Address);
    }

    private void RewriteCmpst()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteCom(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, m.Comp(src));
    }

    private void RewriteCvtp()
    {
        var baseAddr = SrcOperand(1, PrimitiveType.Word32, false);
        var reg = SrcOperand(0, PrimitiveType.Word32, false);
        if (baseAddr is not MemoryAccess mem)
        {
            iclass = InstrClass.Invalid;
            m.Invalid();
            return;
        }
        DstOperand(1, m.IAdd(m.IMul(mem.EffectiveAddress, 8), reg));
    }

    private void RewriteCxp()
    {
        var src = SrcOperand(0, PrimitiveType.Word32, false);
        m.SideEffect(m.Fn(cxp_intrinsic, src));
    }

    private void RewriteCxpd()
    {
        var src = SrcOperand(0, PrimitiveType.Ptr32, false);
        m.SideEffect(m.Fn(cxpd_intrinsic, src));
    }

    private void RewriteDei(PrimitiveType dt)
    {
        Expression dividend;
        var src = SrcOperand(0, dt);
        if (instr.Operands[1] is RegisterStorage reg && reg != Registers.TOS)
        {
            var lo = binder.EnsureRegister(reg);
            var hi = binder.EnsureRegister(Registers.GpRegisters[reg.Number + 1]);
            var seq = m.Seq(
                MaybeSlice(hi, dt),
                MaybeSlice(lo, dt));
            dividend = binder.CreateTemporary(seq.DataType);
            m.Assign(dividend, seq);

            var quo = binder.CreateTemporary(dt);
            var rem = binder.CreateTemporary(dt);
            m.Assign(quo, m.UDiv(dt, dividend, src));
            m.Assign(rem, m.UMod(dt, dividend, src));
            m.Assign(lo, m.Dpb(lo, quo, 0));
            m.Assign(hi, m.Dpb(hi, rem, 0));
        }
        else
        {
            dividend = SrcOperand(1, PrimitiveType.CreateWord(dt.BitSize * 2));
            var quo = binder.CreateTemporary(dt);
            var rem = binder.CreateTemporary(dt);
            m.Assign(quo, m.UDiv(dt, dividend, src));
            m.Assign(rem, m.UMod(dt, dividend, src));
            DstOperand(1, m.Seq(rem, quo));
        }
    }

    private void RewriteDia()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteDiv(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.SDiv(left, right));
    }

    private void RewriteDivf(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.FDiv(left, right));
    }


    private void RewriteDot(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var f0 = binder.EnsureRegister(Registers.FpRegisters[0]);
        var tmp = MaybeSlice(f0, dt);
        m.Assign(f0, m.Dpb(f0, m.FAdd(tmp, m.FMul(left, right)), 0));
    }

    private void RewriteEnter()
    {
        var sp = binder.EnsureRegister(Registers.SP);
        var framePtr = binder.EnsureRegister(Registers.FP);
        m.Assign(sp, m.ISubS(sp, 4));
        m.Assign(m.Mem32(sp), framePtr);
        m.Assign(framePtr, sp);
        m.Assign(sp, m.ISub(sp, (Constant) instr.Operands[1]));
        SaveRegisters(sp);
    }

    private void SaveRegisters(Identifier sp)
    {
        var regs = (RegisterSetOperand) instr.Operands[0];
        foreach (var reg in regs.GetRegisters())
        {
            m.Assign(sp, m.ISubS(sp, 4));
            m.Assign(m.Mem32(sp), binder.EnsureRegister(reg));
        }
    }

    private void RewriteExit()
    {
        var sp = binder.EnsureRegister(Registers.SP);
        RestoreRegisters(sp);
        var framePtr = binder.EnsureRegister(Registers.FP);
        m.Assign(sp, framePtr);
        m.Assign(framePtr, m.Mem32(sp));
        m.Assign(sp, m.IAddS(sp, 4));
    }

    private void RestoreRegisters(Identifier sp)
    {
        var regs = (RegisterSetOperand) instr.Operands[0];
        foreach (var reg in regs.GetRegisters().Reverse())
        {
            m.Assign(m.Mem32(sp), binder.EnsureRegister(reg));
            m.Assign(sp, m.IAddS(sp, 4));
        }
    }

    private void RewriteExt(PrimitiveType dt)
    {
        var baseExpr = SrcOperand(0, dt, false);
        var offset = SrcOperand(2, PrimitiveType.Byte, false);
        var length = SrcOperand(3, PrimitiveType.Byte, false);
        DstOperand(1, m.Fn(ext_intrinsic.MakeInstance(dt), baseExpr, offset, length));
    }

    private void RewriteFfs(PrimitiveType dt)
    {
        var src = this.SrcOperand(0, dt);
        var offset = this.SrcOperand(1, PrimitiveType.Byte, false);
        m.Assign(binder.EnsureFlagGroup(Registers.F), m.Eq0(src));
        DstOperand(1, m.Fn(ffs_intrinsic.MakeInstance(dt), src, offset));
    }

    private void RewriteFlag()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteFtoi(IntrinsicProcedure op, PrimitiveType dtReal, PrimitiveType dtResult)
    {
        var src = this.SrcOperand(0, dtReal);
        DstOperand(1, m.Convert(m.Fn(op, src), dtReal, dtResult));
    }

    private void RewriteFloorf(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.floorf, PrimitiveType.Real32, dtInt);
    }

    private void RewriteFloorl(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.floor, PrimitiveType.Real64, dtInt);
    }

    private void RewriteIbit(PrimitiveType dt)
    {
        var offset = SrcOperand(0, PrimitiveType.Int8);
        var exp = SrcOperand(1, dt, false);
        var setbit = CommonOps.InvertBit.MakeInstance(exp.DataType, offset.DataType);
        var result = DstOperand(1, m.Fn(setbit, exp, offset));
        EmitCond(Registers.F, result);
    }

    private void RewriteIndex(PrimitiveType dt)
    {
        var accum = SrcOperand(0, PrimitiveType.Word32);
        var length = SrcOperand(1, dt);
        var index = SrcOperand(2, dt);
        m.Assign(accum, m.IAdd(m.IMul(accum, m.IAdd(length, 1)), index));
    }

    private void RewriteIns(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        var baseExpr = SrcOperand(1, dt, false);
        var offset = SrcOperand(2, PrimitiveType.Byte, false);
        var length = SrcOperand(3, PrimitiveType.Byte, false);
        DstOperand(1, m.Fn(ins_intrinsic.MakeInstance(dt), baseExpr, src, offset, length));
    }

    private void RewriteJsr()
    {
        var target = SrcOperand(0, PrimitiveType.Ptr32);
        if (target is MemoryAccess mem)
        {
            if (mem.EffectiveAddress is Address addr)
            {
                m.Call(addr, 4);
            }
            else
            {
                m.Call(mem, 4);
            }
        }
        else
        {
            host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
            EmitUnitTest();
            m.Invalid();
        }
    }

    private void RewriteJump()
    {
        var target = SrcOperand(0, PrimitiveType.Ptr32);
        if (target is MemoryAccess mem)
        {
            if (mem.EffectiveAddress is Address addr)
            {
                m.Goto(addr);
            }
            else
            {
                m.Goto(mem);
            }
        }
        else
        {
            host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
            EmitUnitTest();
            m.Invalid();
        }
    }

    private void RewriteLfsr()
    {
        var src = SrcOperand(0, PrimitiveType.Word32);
        m.Assign(binder.EnsureRegister(Registers.FSR), src);
    }

    private void RewriteLmr()
    {
        var mmureg = SrcOperand(0, PrimitiveType.Word32, false);
        var value = SrcOperand(1, PrimitiveType.Word32);
        m.SideEffect(m.Fn(lmr_intrinsic.MakeInstance(PrimitiveType.Word32), mmureg, value));
    }

    private void RewriteLogb(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, m.Fn(logb_intrinsic, src));
    }

    private void RewriteLpr(PrimitiveType dt)
    {
        var pr = SrcOperand(0, dt, false);
        var value = SrcOperand(1, dt);
        m.SideEffect(m.Fn(lpr_intrinsic.MakeInstance(dt), pr, value));
    }

    private void RewriteLsh(PrimitiveType dt)
    {
        RewriteShift(lsh_intrinsic, dt, m.Shl, m.Shr);
    }

    private void RewriteMei(PrimitiveType dt)
    {
        var right = SrcOperand(0, dt, true);
        var left = SrcOperand(1, dt, false);
        var product = m.UMul(PrimitiveType.Create(Domain.UnsignedInt, dt.BitSize*2), left, right);
        if (instr.Operands[1] is RegisterStorage reg && reg != Registers.TOS)
        {
            var tmp = binder.CreateTemporary(product.DataType);
            m.Assign(tmp, product);
            var lo = binder.EnsureRegister(reg);
            var hi = binder.EnsureRegister(Registers.GpRegisters[reg.Number + 1]);
            m.Assign(lo, m.Dpb(lo, m.Slice(tmp, 0, dt.BitSize), 0));
            m.Assign(hi, m.Dpb(hi, m.Slice(tmp, dt.BitSize, dt.BitSize), 0));
            return;
        }
        DstOperand(1, product);
    }

    private void RewriteMod(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt, false);
        var right = SrcOperand(0, dt, true);
        DstOperand(1, m.SMod(left, right));
    }

    private void RewriteMov(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, src, true);
    }

    private void RewriteMovb(PrimitiveType dtFloat)
    {
        var src = SrcOperand(0, PrimitiveType.Int8);
        DstOperand(1, m.Convert(src, src.DataType, dtFloat), true);
    }

    private void RewriteMovd(PrimitiveType dtFloat)
    {
        var src = SrcOperand(0, PrimitiveType.Int32);
        DstOperand(1, m.Convert(src, src.DataType, dtFloat), true);
    }

    private void RewriteMovf(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, src, true);
    }

    private void RewriteMovfl()
    {
        var src = SrcOperand(0, PrimitiveType.Real32);
        DstOperand(1, m.Convert(src, src.DataType, PrimitiveType.Real64), true);
    }

    private void RewriteMovlf()
    {
        var src = SrcOperand(0, PrimitiveType.Real64);
        DstOperand(1, m.Convert(src, PrimitiveType.Real64, PrimitiveType.Real32), true);
    }

    private void RewriteMovm(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt, false);
        var dst = SrcOperand(1, dt, false);
        var length = SrcOperand(2, PrimitiveType.Word32, false);
        m.SideEffect(m.Fn(movm_intrinsic.MakeInstance(32, dt), src, dst, length));
    }

    private void RewriteMovs(PrimitiveType dt)
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteMovst()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteMovsu(PrimitiveType dt)
    {
        var src = SrcOperand(0, PrimitiveType.Ptr32, false);
        var dst = SrcOperand(0, PrimitiveType.Ptr32, false);
        m.SideEffect(m.Fn(movsu_intrinsic.MakeInstance(32, dt), src, dst));
    }

    private void RewriteMovus(PrimitiveType dt)
    {
        var src = SrcOperand(0, PrimitiveType.Ptr32, false);
        var dst = SrcOperand(0, PrimitiveType.Ptr32, false);
        m.SideEffect(m.Fn(movus_intrinsic.MakeInstance(32, dt), src, dst));
    }

    private void RewriteMovwf(PrimitiveType dt)
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteMovxbd()
    {
        var src = SrcOperand(0, PrimitiveType.Byte);
        DstOperand(1, m.ExtendS(src, PrimitiveType.Int32), true);
    }

    private void RewriteMovxbw()
    {
        var src = SrcOperand(0, PrimitiveType.Byte);
        DstOperand(1, m.ExtendS(src, PrimitiveType.Int16), true);
    }

    private void RewriteMovxwd()
    {
        var src = SrcOperand(0, PrimitiveType.Word16);
        DstOperand(1, m.ExtendS(src, PrimitiveType.Int32), true);
    }

    private void RewriteMovzbd()
    {
        var src = SrcOperand(0, PrimitiveType.Byte);
        DstOperand(1, m.ExtendZ(src, PrimitiveType.Int32), true);
    }

    private void RewriteMovzbw()
    {
        var src = SrcOperand(0, PrimitiveType.Byte);
        DstOperand(1, m.ExtendZ(src, PrimitiveType.Int16), true);
    }

    private void RewriteMovzwd()
    {
        var src = SrcOperand(0, PrimitiveType.Byte);
        DstOperand(1, m.ExtendZ(src, PrimitiveType.UInt32), true);
    }

    private void RewriteMul(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt, false);
        var right = SrcOperand(0, dt, true);
        DstOperand(1, m.IMul(left, right));
    }

    private void RewriteMulf(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt, false);
        var right = SrcOperand(0, dt, true);
        DstOperand(1, m.FMul(left, right));
    }

    private void RewriteNeg(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt, false);
        var dst = DstOperand(1, m.Neg(src));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteNegf(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt, false);
        DstOperand(1, m.FNeg(src));
    }

    private void RewriteNop()
    {
        m.Nop();
    }

    private void RewriteNot(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, m.ExtendZ(m.Eq0(src), dt), true);
    }

    private void RewriteOr(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.Or(left, right));
    }

    private void RewritePolyf(PrimitiveType dt)
    {
        var src1 = SrcOperand(0, dt);
        var src2 = SrcOperand(1, dt);
        var f0 = binder.EnsureRegister(Registers.FpRegisters[0]);
        var tmp = MaybeSlice(f0, dt);
        m.Assign(f0, m.Dpb(f0, m.FAdd(m.FMul(tmp, src1), src2), 0));
    }

    private void RewriteQuo(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.SDiv(left, right));
    }

    private void RewriteRdval()
    {
        var f = binder.EnsureFlagGroup(Registers.F);
        var ptr = SrcOperand(0, PrimitiveType.Ptr32, false);
        m.Assign(f, m.Fn(rdval_intrinsic, ptr));
    }

    private void RewriteRem(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt, false);
        var right = SrcOperand(0, dt, true);
        DstOperand(1, m.SMod(left, right));
    }

    private void RewriteRestore()
    {
        var sp = binder.EnsureRegister(Registers.SP);
        RestoreRegisters(sp);
    }

    private void RewriteRet()
    {
        var extra = ((Constant) instr.Operands[0]).ToInt32();
        m.Return(4, extra);
    }

    private void RewriteReti()
    {
        m.SideEffect(m.Fn(reti_intrinsic));
        m.Return(4, 0);
    }

    private void RewriteRett()
    {
        m.SideEffect(m.Fn(rett_intrinsic));
        m.Return(4, 0);
    }

    private void RewriteRot(PrimitiveType dt)
    {
        RewriteShift(
            rot_intrinsic, dt,
            (v, sh) => m.Fn(CommonOps.Rol, sh),
            (v, sh) => m.Fn(CommonOps.Ror, sh));
    }

    private void RewriteRoundf(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.roundf, PrimitiveType.Real32, dtInt);
    }

    private void RewriteRoundl(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.round, PrimitiveType.Real64, dtInt);
    }

    private void RewriteRxp()
    {
        m.SideEffect(m.Fn(rxp_intrinsic));
        m.Return(4, ((Constant)instr.Operands[0]).ToInt32());
    }

    private void RewriteSave()
    {
        var sp = binder.EnsureRegister(Registers.SP);
        SaveRegisters(sp);
    }

    private void RewriteSbit(PrimitiveType dt)
    {
        var offset = SrcOperand(0, PrimitiveType.Int8);
        var exp = SrcOperand(1, dt, false);
        var setbit = CommonOps.SetBit.MakeInstance(exp.DataType, offset.DataType);
        var result = DstOperand(1, m.Fn(setbit, exp, offset));
        EmitCond(Registers.F, result);
    }

    private void RewriteSbiti()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteSbiti(PrimitiveType dt)
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteScalbf(PrimitiveType dt)
    {
        var right = SrcOperand(0, dt);
        var left = SrcOperand(1, dt, false);
        var pow = dt.BitSize > 32 ? FpOps.Pow : FpOps.Powf;
        var two = dt.BitSize > 32 ? Constant.Real64(2.0) : Constant.Real32(2.0F);
        DstOperand(1, m.FMul(left, m.Fn(pow, two, right)));
    }

    private void RewriteScond(FlagGroupStorage grf, ConditionCode cc, PrimitiveType dt)
    {
        DstOperand(0, m.Convert(
            m.Test(cc,  binder.EnsureFlagGroup(grf)),
            PrimitiveType.Bool,
            dt));
    }

    private void RewriteScond(FlagGroupStorage grf, bool flag, PrimitiveType dt)
    {
        var f = binder.EnsureFlagGroup(grf);
        DstOperand(0, m.Convert(
            flag ? f : m.Not(f),
            PrimitiveType.Bool,
            dt));
    }

    private void RewriteSetcfg()
    {
        var op = ((LiteralOperand) instr.Operands[0]).ToString();
        m.SideEffect(m.Fn(setcfg_intrinsic, new StringConstant(
            StringType.NullTerminated(PrimitiveType.Char),
            op)));
    }

    // pr
    private void RewriteSfsr()
    {
        DstOperand(0, binder.EnsureRegister(Registers.FSR));
    }

    private void RewriteSkps(PrimitiveType dt)
    {
        var r0 = binder.EnsureRegister(Registers.GpRegisters[0]);
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r4 = MaybeSlice(binder.EnsureRegister(Registers.GpRegisters[4]), dt);
        var flags = ((LiteralOperand) instr.Operands[0]).ToString();
        var addrNext = instr.Address + instr.Length;
        m.BranchInMiddleOfInstruction(m.Eq0(r0), addrNext, InstrClass.ConditionalTransfer);
        var value = binder.CreateTemporary(dt);
        m.Assign(value, m.Mem(dt, r1));
        if (flags.Contains('u'))
        {
            m.BranchInMiddleOfInstruction(m.Eq(value, r4), addrNext, InstrClass.ConditionalTransfer);
        }
        if (flags.Contains('w'))
        { 
            m.BranchInMiddleOfInstruction(m.Ne(value, r4), addrNext, InstrClass.ConditionalTransfer);
        }
        if (flags.Contains('b'))
        {
            m.Assign(r1, m.ISubS(r1, 1));
        }
        else
        {
            m.Assign(r1, m.IAddS(r1, 1));
        }
        m.Assign(r0, m.ISub(r0, 1));
        m.Goto(instr.Address);
    }

    private void RewriteSkpst()
    {
        host.Warn(instr.Address, $"Rewriter for Instruction {instr} not implemented yet.");
    }

    private void RewriteSmr()
    {
        var src = SrcOperand(0, PrimitiveType.Word32);
        DstOperand(1, m.Fn(smr_intrinsic.MakeInstance(src.DataType), src));
    }

    private void RewriteSpr(PrimitiveType dt)
    {
        var src = SrcOperand(0, dt);
        DstOperand(1, m.Fn(spr_intrinsic.MakeInstance(src.DataType), src));
    }

    private void RewriteSub(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var dst = DstOperand(1, m.ISub(left, right));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteSubc(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = DstOperand(1, m.ISubC(left, right, c));
        EmitCond(Registers.CF, dst);
    }

    private void RewriteSubf(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        DstOperand(1, m.FSub(left, right));
    }

    private void RewriteSubp(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt);
        var dst = DstOperand(1, m.Fn(subp_intrinsic.MakeInstance(left.DataType), left, right));
        EmitCond(Registers.C, dst);
        m.Assign(binder.EnsureFlagGroup(Registers.F), 0);
    }

    private void RewriteSvc()
    {
        m.SideEffect(m.Fn(CommonOps.Syscall));
    }

    private void RewriteTbit(PrimitiveType dt)
    {
        var offset = SrcOperand(0, PrimitiveType.Int8);
        var exp = SrcOperand(1, dt, false);
        var setbit = CommonOps.Bit.MakeInstance(exp.DataType, offset.DataType);
        m.Assign(
            binder.EnsureFlagGroup(Registers.F),
            m.Fn(setbit, exp, offset));
    }

    private void RewriteTruncf(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.truncf, PrimitiveType.Real32, dtInt);
    }

    private void RewriteTruncl(PrimitiveType dtInt)
    {
        RewriteFtoi(FpOps.trunc, PrimitiveType.Real64, dtInt);
    }

    private void RewriteWait()
    {
        m.SideEffect(m.Fn(wait_intrinsic));
    }

    private void RewriteWrval()
    {
        var f = binder.EnsureFlagGroup(Registers.F);
        var ptr = SrcOperand(0, PrimitiveType.Ptr32, false);
        m.Assign(f, m.Fn(wrval_intrinsic, ptr));
    }

    private void RewriteXor(PrimitiveType dt)
    {
        var left = SrcOperand(1, dt);
        var right = SrcOperand(0, dt, false);
        DstOperand(1, m.Xor(left, right));
    }

    private static readonly IntrinsicProcedure addp_intrinsic = IntrinsicBuilder.GenericBinary("__add_packed_bcd");
    private static readonly IntrinsicProcedure ash_intrinsic = new IntrinsicBuilder("__shift_arithmetic", true)
        .GenericTypes("TValue", "TShift")
        .Param("TValue")
        .Param("TShift")
        .Returns("TValue");
    private static readonly IntrinsicProcedure bicpsr_intrinsic = new IntrinsicBuilder("__bit_clear_psr", true)
        .Param(PrimitiveType.Word16)
        .Void();
    private static readonly IntrinsicProcedure bispsr_intrinsic = new IntrinsicBuilder("__bit_set_psr", true)
        .Param(PrimitiveType.Word16)
        .Void();
    private static readonly IntrinsicProcedure bpt_intrinsic = IntrinsicBuilder.SideEffect("__breakpoint_trap").Void();
    private static readonly IntrinsicProcedure cinv_intrinsic = IntrinsicBuilder.SideEffect("__cache_invalidate")
        .Param(StringType.NullTerminated(PrimitiveType.Char))
        .Param(PrimitiveType.Word32)
        .Void();
    private static readonly IntrinsicProcedure cmpm_intrinsic = IntrinsicBuilder.SideEffect("__compare_multiple")
        .GenericTypes("T")
        .PtrParam("T")
        .PtrParam("T")
        .Param(PrimitiveType.UInt32)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure cxp_intrinsic = IntrinsicBuilder.SideEffect("__call_external_procedure")
        .Param(PrimitiveType.Word32)
        .Void();
    private static readonly IntrinsicProcedure cxpd_intrinsic = IntrinsicBuilder.SideEffect("__call_external_procedure_descriptor")
        .Param(PrimitiveType.Ptr32)
        .Void();
    private static readonly IntrinsicProcedure ext_intrinsic = new IntrinsicBuilder("__extract_field", false)
        .GenericTypes("T")
        .Param("T")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns("T");
    private static readonly IntrinsicProcedure ffs_intrinsic = new IntrinsicBuilder("__find_first_set_bit", false)
        .GenericTypes("TValue")
        .Param("TValue")
        .Param(PrimitiveType.Byte)
        .Returns("TValue");
    private static readonly IntrinsicProcedure ins_intrinsic = new IntrinsicBuilder("__insert_field", false)
        .GenericTypes("T")
        .Param("T")
        .Param("T")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns("T");

    private static readonly IntrinsicProcedure lmr_intrinsic = new IntrinsicBuilder("__load_mmu_register", true)
        .GenericTypes("T")
        .Param("T")
        .Param("T")
        .Void();
    private static readonly IntrinsicProcedure logb_intrinsic = IntrinsicBuilder.GenericUnary("__logarithm_binary");
    private static readonly IntrinsicProcedure lpr_intrinsic = new IntrinsicBuilder("__load_processor_register", true)
        .GenericTypes("T")
        .Param("T")
        .Param("T")
        .Void();
    private static readonly IntrinsicProcedure lsh_intrinsic = new IntrinsicBuilder("__shift_logical", false)
        .GenericTypes("TValue", "TShift")
        .Param("TValue")
        .Param("TShift")
        .Returns("TValue");
    private static readonly IntrinsicProcedure movm_intrinsic = IntrinsicBuilder.SideEffect("__move_multiple")
        .GenericTypes("T")
        .PtrParam("T")
        .PtrParam("T")
        .Param(PrimitiveType.UInt32)
        .Void();
    private static readonly IntrinsicProcedure movsu_intrinsic = IntrinsicBuilder.SideEffect("__move_to_user_space")
        .GenericTypes("T")
        .PtrParam("T")
        .PtrParam("T")
        .Void();
    private static readonly IntrinsicProcedure movus_intrinsic = IntrinsicBuilder.SideEffect("__move_to_supervisor_space")
        .GenericTypes("T")
        .PtrParam("T")
        .PtrParam("T")
        .Void();
    private static readonly IntrinsicProcedure rdval_intrinsic = IntrinsicBuilder.SideEffect("__validate_read_address")
        .Param(PrimitiveType.Ptr32)
        .Returns(PrimitiveType.Bool);
    private static readonly IntrinsicProcedure rett_intrinsic = IntrinsicBuilder.SideEffect("__return_from_trap").Void();
    private static readonly IntrinsicProcedure reti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt").Void();
    static readonly IntrinsicProcedure rot_intrinsic = new IntrinsicBuilder("__rotate", false)
        .GenericTypes("TValue", "TShift")
        .Param("TValue")
        .Param("TShift")
        .Returns("TValue");
    private static readonly IntrinsicProcedure rxp_intrinsic = IntrinsicBuilder.SideEffect("__return_external_procedure")
        .Void();
    private static readonly IntrinsicProcedure setcfg_intrinsic = IntrinsicBuilder.SideEffect("__set_configuration")
        .Param(StringType.NullTerminated(PrimitiveType.Char))
        .Void();
    private static readonly IntrinsicProcedure smr_intrinsic = new IntrinsicBuilder("__store_mmu_register", true)
        .GenericTypes("T")
        .Param("T")
        .Returns("T");
    private static readonly IntrinsicProcedure spr_intrinsic = new IntrinsicBuilder("__store_processor_register", true)
        .GenericTypes("T")
        .Param("T")
        .Returns("T");
    private static readonly IntrinsicProcedure subp_intrinsic = IntrinsicBuilder.GenericBinary("__sub_packed_bcd");
    private static readonly IntrinsicProcedure wait_intrinsic = IntrinsicBuilder.SideEffect("__wait").Void();
    private static readonly IntrinsicProcedure wrval_intrinsic = IntrinsicBuilder.SideEffect("__validate_write_address")
        .Param(PrimitiveType.Ptr32)
        .Returns(PrimitiveType.Bool);
}
