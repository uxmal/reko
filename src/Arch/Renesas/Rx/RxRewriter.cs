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
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Renesas.Rx;

public class RxRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly RxArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<RxInstruction> dasm;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;

    public RxRewriter(RxArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new RxDisassembler(arch, rdr).GetEnumerator();
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            switch (instr.Mnemonic)
            {
            case Mnemonic.abs: RewriteAbs(instr); break;
            case Mnemonic.adc: RewriteAdc(instr); break;
            case Mnemonic.add: RewriteAdd(instr); break;
            case Mnemonic.and: RewriteAnd(instr); break;
            case Mnemonic.bclr: RewriteBclr(instr); break;
            case Mnemonic.beq: RewriteBcc(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.bne: RewriteBcc(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.bgeu: RewriteBcc(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.bltu: RewriteBcc(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.bgtu: RewriteBcc(instr, ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.bleu: RewriteBcc(instr, ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.bpz: RewriteBcc(instr, ConditionCode.GE, Registers.S); break;
            case Mnemonic.bn: RewriteBcc(instr, ConditionCode.LT, Registers.S); break;
            case Mnemonic.bge: RewriteBcc(instr, ConditionCode.GE, Registers.OS); break;
            case Mnemonic.blt: RewriteBcc(instr, ConditionCode.LT, Registers.OS); break;
            case Mnemonic.bgt: RewriteBcc(instr, ConditionCode.GT, Registers.OSZ); break;
            case Mnemonic.ble: RewriteBcc(instr, ConditionCode.LE, Registers.OSZ); break;
            case Mnemonic.bo: RewriteBcc(instr, ConditionCode.OV, Registers.O); break;
            case Mnemonic.bno: RewriteBcc(instr, ConditionCode.NO, Registers.O); break;
            case Mnemonic.bmeq: RewriteBmcc(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.bmne: RewriteBmcc(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.bmgeu: RewriteBmcc(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.bmltu: RewriteBmcc(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.bmgtu: RewriteBmcc(instr, ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.bmleu: RewriteBmcc(instr, ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.bmpz: RewriteBmcc(instr, ConditionCode.GE, Registers.S); break;
            case Mnemonic.bmn: RewriteBmcc(instr, ConditionCode.LT, Registers.S); break;
            case Mnemonic.bmge: RewriteBmcc(instr, ConditionCode.GE, Registers.OS); break;
            case Mnemonic.bmlt: RewriteBmcc(instr, ConditionCode.LT, Registers.OS); break;
            case Mnemonic.bmgt: RewriteBmcc(instr, ConditionCode.GT, Registers.OSZ); break;
            case Mnemonic.bmle: RewriteBmcc(instr, ConditionCode.LE, Registers.OSZ); break;
            case Mnemonic.bmo: RewriteBmcc(instr, ConditionCode.OV, Registers.O); break;
            case Mnemonic.bmno: RewriteBmcc(instr, ConditionCode.NO, Registers.O); break;
            case Mnemonic.bra: RewriteBra(instr); break;
            case Mnemonic.bnot: RewriteBnot(instr);break;
            case Mnemonic.brk: RewriteBrk(instr); break;
            case Mnemonic.bset: RewriteBset(instr); break;
            case Mnemonic.bsr: RewriteBsr(instr); break;
            case Mnemonic.btst: RewriteBtst(instr); break;
            case Mnemonic.clrpsw: RewriteClrpsw(instr); break;
            case Mnemonic.cmp: RewriteCmp(instr); break;
            case Mnemonic.div: RewriteDiv(instr); break;
            case Mnemonic.divu: RewriteDivu(instr); break;
            case Mnemonic.emaca: RewriteEmaca(instr, Operator.IAdd); break;
            case Mnemonic.emsba: RewriteEmaca(instr, Operator.ISub); break;
            case Mnemonic.emul: RewriteEmul(instr); break;
            case Mnemonic.emula: RewriteEmula(instr); break;
            case Mnemonic.emulu: RewriteEmulu(instr); break;
            case Mnemonic.fadd: RewriteFadd(instr); break;
            case Mnemonic.fcmp: RewriteFcmp(instr); break;
            case Mnemonic.fdiv: RewriteFdiv(instr); break;
            case Mnemonic.fmul: RewriteFmul(instr); break;
            case Mnemonic.fsqrt: RewriteFsqrt(instr); break;
            case Mnemonic.fsub: RewriteFsub(instr); break;
            case Mnemonic.ftoi: RewriteFto(instr, PrimitiveType.Int32); break;
            case Mnemonic.ftou: RewriteFto(instr, PrimitiveType.UInt32); break;
            case Mnemonic.@int: RewriteInt(instr); break;
            case Mnemonic.itof: RewriteItof(instr, Domain.SignedInt); break;
            case Mnemonic.jmp: RewriteJmp(instr); break;
            case Mnemonic.jsr: RewriteJsr(instr); break;
            case Mnemonic.machi: RewriteMachi(instr, Operator.IAdd); break;
            case Mnemonic.maclh: RewriteMaclh(instr, Operator.IAdd); break;
            case Mnemonic.maclo: RewriteMaclo(instr, Operator.IAdd); break;
            case Mnemonic.max: RewriteMax(instr); break;
            case Mnemonic.min: RewriteMin(instr); break;
            case Mnemonic.mov: RewriteMov(instr); break;
            case Mnemonic.movu: RewriteMovu(instr); break;
            case Mnemonic.msbhi: RewriteMachi(instr, Operator.ISub); break;
            case Mnemonic.msblh: RewriteMaclh(instr, Operator.ISub); break;
            case Mnemonic.msblo: RewriteMaclo(instr, Operator.ISub); break;
            case Mnemonic.mul: RewriteMul(instr); break;
            case Mnemonic.mulhi: RewriteMulhi(instr); break;
            case Mnemonic.mullh: RewriteMullh(instr); break;
            case Mnemonic.mullo: RewriteMullo(instr); break;
            case Mnemonic.mvfacgu: RewriteMvfac(instr, mvfacgu_intrinsic); break;
            case Mnemonic.mvfachi: RewriteMvfac(instr, mvfachi_intrinsic); break;
            case Mnemonic.mvfaclo: RewriteMvfac(instr, mvfaclo_intrinsic); break;
            case Mnemonic.mvfacmi: RewriteMvfac(instr, mvfacmi_intrinsic); break;
            case Mnemonic.mvfc: RewriteMvfc(instr); break;
            case Mnemonic.mvtacgu: RewriteMvtac(instr, mvtacgu_intrinsic); break;
            case Mnemonic.mvtachi: RewriteMvtac(instr, mvtachi_intrinsic); break;
            case Mnemonic.mvtaclo: RewriteMvtac(instr, mvtaclo_intrinsic); break;
            case Mnemonic.mvtacmi: RewriteMvtac(instr, mvtacmi_intrinsic); break;
            case Mnemonic.mvtc: RewriteMvtc(instr); break;
            case Mnemonic.mvtipl: RewriteMvtipl(instr); break;
            case Mnemonic.neg: RewriteNeg(instr); break;
            case Mnemonic.nop: RewriteNop(instr); break;
            case Mnemonic.not: RewriteNot(instr); break;
            case Mnemonic.or: RewriteOr(instr); break;
            case Mnemonic.pop: RewritePop(instr); break;
            case Mnemonic.popc: RewritePopc(instr); break;
            case Mnemonic.popm: RewritePopm(instr); break;
            case Mnemonic.push: RewritePush(instr); break;
            case Mnemonic.pushc: RewritePushc(instr); break;
            case Mnemonic.pushm: RewritePushm(instr); break;
            case Mnemonic.racl: RewriteRoundAccumulator(instr, racl_intrinsic); break;
            case Mnemonic.racw: RewriteRoundAccumulator(instr, racw_intrinsic); break;
            case Mnemonic.rdacl: RewriteRoundAccumulator(instr, rdacl_intrinsic); break;
            case Mnemonic.rdacw: RewriteRoundAccumulator(instr, rdacw_intrinsic); break;
            case Mnemonic.revl: RewriteRevl(instr); break;
            case Mnemonic.revw: RewriteRevw(instr); break;
            case Mnemonic.rmpa: RewriteRmpa(instr); break;
            case Mnemonic.rolc: RewriteRoxc(instr, CommonOps.RolC); break;
            case Mnemonic.rorc: RewriteRoxc(instr, CommonOps.RorC); break;
            case Mnemonic.rotl: RewriteRotl(instr); break;
            case Mnemonic.rotr: RewriteRotr(instr); break;
            case Mnemonic.round: RewriteRound(instr); break;
            case Mnemonic.rte: RewriteRte(instr); break;
            case Mnemonic.rtfi: RewriteRtfi(instr); break;
            case Mnemonic.rts: RewriteRts(instr); break;
            case Mnemonic.rtsd: RewriteRtsd(instr); break;
            case Mnemonic.sat: RewriteSat(instr); break;
            case Mnemonic.satr: RewriteSatr(instr); break;
            case Mnemonic.sbb: RewriteSbb(instr); break;
            case Mnemonic.sceq: RewriteScc(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.scne: RewriteScc(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.scgeu: RewriteScc(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.scltu: RewriteScc(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.scgtu: RewriteScc(instr, ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.scleu: RewriteScc(instr, ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.scpz: RewriteScc(instr, ConditionCode.GE, Registers.S); break;
            case Mnemonic.scn: RewriteScc(instr, ConditionCode.LT, Registers.S); break;
            case Mnemonic.scge: RewriteScc(instr, ConditionCode.GE, Registers.OS); break;
            case Mnemonic.sclt: RewriteScc(instr, ConditionCode.LT, Registers.OS); break;
            case Mnemonic.scgt: RewriteScc(instr, ConditionCode.GT, Registers.OSZ); break;
            case Mnemonic.scle: RewriteScc(instr, ConditionCode.LE, Registers.OSZ); break;
            case Mnemonic.sco: RewriteScc(instr, ConditionCode.OV, Registers.O); break;
            case Mnemonic.scno: RewriteScc(instr, ConditionCode.NO, Registers.O); break;
            case Mnemonic.scmpu: RewriteScmpu(instr); break;
            case Mnemonic.shar: RewriteShift(instr, Operator.Sar); break;
            case Mnemonic.shll: RewriteShift(instr, Operator.Shl); break;
            case Mnemonic.shlr: RewriteShift(instr, Operator.Shr); break;
            case Mnemonic.setpsw: RewriteSetpsw(instr); break;
            case Mnemonic.smovb: RewriteSmovb(instr); break;
            case Mnemonic.smovf: RewriteSmovf(instr); break;
            case Mnemonic.smovu: RewriteSmovu(instr); break;
            case Mnemonic.sstr: RewriteSstr(instr); break;
            case Mnemonic.stnz: RewriteSt(instr, ConditionCode.EQ); break;
            case Mnemonic.stz: RewriteSt(instr, ConditionCode.NE); break;
            case Mnemonic.sub: RewriteSub(instr); break;
            case Mnemonic.suntil: RewriteSuntil(instr); break;
            case Mnemonic.swhile: RewriteSwhile(instr); break;
            case Mnemonic.tst: RewriteTst(instr); break;
            case Mnemonic.utof: RewriteItof(instr, Domain.UnsignedInt); break;
            case Mnemonic.wait: RewriteWait(instr); break;
            case Mnemonic.xchg: RewriteXchg(instr); break;
            case Mnemonic.xor: RewriteXor(instr); break;
            default:
                throw new NotImplementedException($"Unhandled instruction {instr.MnemonicAsString}");
            }
            yield return m.MakeCluster(instr.Address, instr.Length, instr.InstructionClass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Identifier EmitCc(Expression dst, FlagGroupStorage flags)
    {
        var grf = binder.EnsureFlagGroup(flags);
        m.Assign(grf, m.Cond(grf.DataType, dst));
        return grf;
    }

    private Expression OpSrc(RxInstruction instr, int iop)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case RegisterStorage reg:
            return binder.EnsureRegister(reg);
        case Constant c:
            return c;
        case Address a:
            return a;
        case MemoryOperand mem:
            return RewriteMemoryOperand(mem);
        default:
            throw new NotImplementedException($"Unhandled operand type {op.GetType().Name} in instruction {instr}.");
        }
    }

    private Identifier OpDst(RxInstruction instr, int iop, Expression src)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case RegisterStorage reg:
            var idDst = binder.EnsureRegister(reg);
            m.Assign(idDst, src);
            return idDst;
        case MemoryOperand mem:
            if (src is not Identifier idSrc)
            {
                idSrc = binder.CreateTemporary(src.DataType);
                m.Assign(idSrc, src);
            }
            m.Assign(RewriteMemoryOperand(mem), idSrc);
            return idSrc;
        default:
            throw new NotImplementedException($"Unhandled operand type {op.GetType().Name} in instruction {instr}.");
        }
    }

    private void RewriteAbs(RxInstruction instr)
    {
        Identifier dst = RewriteUnary(instr, CommonOps.Abs);
        EmitCc(dst, Registers.OSZ);
    }

    private void RewriteAdc(RxInstruction instr)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, 0);
        var cy = binder.EnsureFlagGroup(Registers.C);
        var idDst = OpDst(instr, 1, m.IAddC(left, right, cy));
        EmitCc(idDst, Registers.COSZ);
    }

    private void RewriteAdd(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.IAdd);
        EmitCc(idDst, Registers.COSZ);
    }

    private void RewriteAnd(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.And);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteBcc(RxInstruction instr, ConditionCode cc, FlagGroupStorage flags)
    {
        var target = OpSrc(instr, 0);
        var grf = binder.EnsureFlagGroup(flags);
        m.Branch(m.Test(cc, grf), target);
    }

    private void RewriteBclr(RxInstruction instr)
    {
        RewriteBinary(instr, CommonOps.ClearBit);
    }

    private Identifier RewriteBinary(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        var src = OpSrc(instr, 1);
        var bit = OpSrc(instr, 0);
        src = m.Fn(intrinsic, src, bit);
        return OpDst(instr, instr.Operands.Length - 1, src);
    }

    private Identifier RewriteBinary(RxInstruction instr, BinaryOperator bin)
    {
        Expression left = OpSrc(instr, 1);
        Expression right = OpSrc(instr, 0);
        var src = m.Bin(bin, left, right);
        var id = OpDst(instr, instr.Operands.Length - 1, src);
        return id;
    }

    private Identifier RewriteBinary(RxInstruction instr, IntrinsicProcedure intrinsic, int op2 = 0)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, op2);
        var src = m.Fn(intrinsic, left, right);
        return OpDst(instr, instr.Operands.Length - 1, src);
    }

    private void RewriteBmcc(RxInstruction instr, ConditionCode cc, FlagGroupStorage grf)
    {
        var src = OpSrc(instr, 0);
        OpDst(instr, 1, m.Fn(
            bmcc_intrinsic, 
            m.Test(cc, binder.EnsureFlagGroup(grf)),
            src));
    }

    private void RewriteBnot(RxInstruction instr)
    {
        RewriteBinary(instr, CommonOps.InvertBit);
    }

    private void RewriteBra(RxInstruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Goto(target);
    }

    private void RewriteBrk(RxInstruction instr)
    {
        m.SideEffect(m.Fn(brk_intrinsic));
    }

    private void RewriteBset(RxInstruction instr)
    {
        RewriteBinary(instr, CommonOps.SetBit);
    }

    private void RewriteBsr(RxInstruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Call(target, 4);
    }

    private void RewriteClrpsw(RxInstruction instr)
    {
        var flag = (FlagGroupStorage) instr.Operands[0];
        var grf = binder.EnsureFlagGroup(flag);
        m.Assign(grf, flag.FlagGroupBits);
    }

    private void RewriteBtst(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, CommonOps.Bit);
        var c = binder.EnsureFlagGroup(Registers.C);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(c, m.Conditional(c.DataType, idDst, m.Word32((uint) FlagM.CF), m.Word32(0)));
        m.Assign(z, m.Conditional(c.DataType, idDst, m.Word32(0), m.Word32((uint) FlagM.ZF)));
    }

    private void RewriteCmp(RxInstruction instr)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, 0);
        var diff = m.ISub(left, right);
        m.Assign(binder.EnsureFlagGroup(Registers.COSZ), m.Cond(PrimitiveType.Word32, diff));
    }

    private void RewriteDiv(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.SDiv);
        EmitCc(idDst, Registers.O);
    }

    private void RewriteDivu(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.UDiv);
        EmitCc(idDst, Registers.O);
    }

    private void RewriteEmaca(RxInstruction instr, BinaryOperator accum)
    {
        var src = OpSrc(instr, 0);
        var src2 = OpSrc(instr, 1);
        var tmp = binder.CreateTemporary(RxArchitecture.Word72);
        m.Assign(tmp, m.Convert(
            m.SMul(PrimitiveType.Int64, src, src2), 
            PrimitiveType.Int64,
            RxArchitecture.Int72));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Bin(accum, aDest, tmp));
    }

    private void RewriteEmul(RxInstruction instr)
    {
        var right = OpSrc(instr, 0);
        var rDst = (RegisterStorage) instr.Operands[1];
        if (rDst.Number >= 15)
        {
            m.Invalid();
            return;
        }
        var rDstHi = Registers.GpRegisters[rDst.Number + 1];
        var left = binder.EnsureRegister(rDst);
        var dst = binder.EnsureSequence(PrimitiveType.Word64, rDstHi, rDst);
        m.Assign(dst, m.SMul(PrimitiveType.Word64, left, right));
    }

    private void RewriteEmula(RxInstruction instr)
    {
        var rSrc = (RegisterStorage) instr.Operands[0];
        var rSrc2 = (RegisterStorage) instr.Operands[1];
        var aDst = (RegisterStorage) instr.Operands[2];
        var left = binder.EnsureRegister(rSrc2);
        var right = binder.EnsureRegister(rSrc);
        var dst = binder.EnsureRegister(aDst);
        m.Assign(dst, m.Convert(
            m.SMul(PrimitiveType.Int64, left, right),
            PrimitiveType.Int64,
            RxArchitecture.Int72));
    }

    private void RewriteEmulu(RxInstruction instr)
    {
        var right = OpSrc(instr, 0);
        var rDst = (RegisterStorage) instr.Operands[1];
        if (rDst.Number >= 15)
        {
            m.Invalid();
            return;
        }
        var rDstHi = Registers.GpRegisters[rDst.Number + 1];
        var left = binder.EnsureRegister(rDst);
        var dst = binder.EnsureSequence(PrimitiveType.Word64, rDstHi, rDst);
        m.Assign(dst, m.UMul(PrimitiveType.UInt64, left, right));
    }


    private void RewriteFadd(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.FAdd);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteFcmp(RxInstruction instr)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, 0);
        var diff = m.FSub(left, right);
        m.Assign(binder.EnsureFlagGroup(Registers.OSZ), m.Cond(PrimitiveType.Word32, diff));
    }

    private void RewriteFdiv(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.FDiv);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteFmul(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.FMul);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteFsqrt(RxInstruction instr)
    {
        var idDst = RewriteUnary(instr, FpOps.sqrtf);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteFsub(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.FSub);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteFto(RxInstruction instr, PrimitiveType dtInt)
    {
        var tmp = binder.CreateTemporary(PrimitiveType.Real32);
        m.Assign(tmp, m.Fn(FpOps.truncf, OpSrc(instr, 0)));
        var dst = OpDst(instr, 1, m.Convert(tmp, PrimitiveType.Real32, dtInt));
        EmitCc(dst, Registers.SZ);
    }

    private void RewriteInt(RxInstruction instr)
    {
        m.SideEffect(m.Fn(CommonOps.Syscall_1,(Constant) instr.Operands[0]));
    }

    private void RewriteItof(RxInstruction instr, Domain domain)
    {
        var src = OpSrc(instr, 0);
        var dtSrc = PrimitiveType.Create(domain, src.DataType.BitSize);
        var dst = OpDst(instr, 1, m.Convert(src, dtSrc, PrimitiveType.Real32));
        EmitCc(dst, Registers.SZ);
    }

    private void RewriteJmp(RxInstruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Goto(target);
    }

    private void RewriteJsr(RxInstruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Call(target, 4);
    }

    private void RewriteMachi(RxInstruction instr, BinaryOperator accum)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 16));
        m.Assign(product, m.SMul(PrimitiveType.Int32, sLeft, sRight));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Bin(accum, aDest, m.Shl(product, 16)));
    }

    private void RewriteMaclh(RxInstruction instr, BinaryOperator accum)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 0));
        m.Assign(product, m.SMul(PrimitiveType.Int32, sLeft, sRight));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Bin(accum, aDest, m.Shl(product, 16)));
    }

    private void RewriteMaclo(RxInstruction instr, BinaryOperator accum)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 0));
        m.Assign(product, m.SMul(PrimitiveType.Int32, sLeft, sRight));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Bin(accum, aDest, m.Shl(product, 16)));
    }

    private void RewriteMax(RxInstruction instr)
    {
        var idDst = RewriteBinary(
            instr, 
            CommonOps.Max.MakeInstance(PrimitiveType.Int32));
    }

    private Expression RewriteMemoryOperand(MemoryOperand mem)
    {
        Debug.Assert(mem.Base is not null);
        var b = binder.EnsureRegister(mem.Base);
        var cb = mem.DataType.BitSize / arch.MemoryGranularity;
        switch (mem.AutoIncrement)
        {
        case AutoIncrement.PreDecrement:
            m.Assign(b, m.ISub(b, cb));
            return m.Mem(mem.DataType, b);
        case AutoIncrement.PostIncrement:
            var tmp = binder.CreateTemporary(b.DataType);
            m.Assign(tmp, b);
            m.Assign(b, m.IAdd(b, cb));
            return m.Mem(mem.DataType, tmp);
        case AutoIncrement.None:
            Expression ea = b;
            if (mem.Index is not null)
            {
                Expression idx = binder.EnsureRegister(mem.Index);
                if (cb > 1)
                {
                    idx = m.IMul(idx, cb);
                }
                ea = m.IAdd(b, idx);
            }
            else if (mem.Offset != 0)
            {
                ea = m.IAddS(b, mem.Offset);
            }
            return m.Mem(mem.DataType, ea);
        default:
            break;
        }
        throw new InvalidOperationException($"Impossible {mem.AutoIncrement} mode.");
    }

    private void RewriteMin(RxInstruction instr)
    {
        RewriteBinary(
            instr,
            CommonOps.Min.MakeInstance(PrimitiveType.Int32));
    }

    private void RewriteMov(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        OpDst(instr, 1, src);
    }

    private void RewriteMovu(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        var dt = src.DataType;
        if (dt.BitSize < 32)
        {
            src = m.Convert(src, dt, PrimitiveType.Word32);
        }
        OpDst(instr, 1, src);
    }

    private void RewriteMul(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.IMul);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteMulhi(RxInstruction instr)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 16));
        m.Assign(product, m.Convert(
            m.SMul(PrimitiveType.Int32, sLeft, sRight),
            PrimitiveType.Int32,
            RxArchitecture.Int72));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Shl(product, 16));
    }

    private void RewriteMullh(RxInstruction instr)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 0));
        m.Assign(product, m.Convert(
            m.SMul(PrimitiveType.Int32, sLeft, sRight),
            PrimitiveType.Int32,
            RxArchitecture.Int72));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Shl(product, 16));
    }

    private void RewriteMullo(RxInstruction instr)
    {
        var right = OpSrc(instr, 0);
        var left = OpSrc(instr, 1);
        var sLeft = binder.CreateTemporary(PrimitiveType.Word16);
        var sRight = binder.CreateTemporary(PrimitiveType.Word16);
        var product = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(sLeft, m.Slice(left, sLeft.DataType, 16));
        m.Assign(sRight, m.Slice(right, sRight.DataType, 0));
        m.Assign(product, m.Convert(
            m.SMul(PrimitiveType.Int32, sLeft, sRight),
            PrimitiveType.Int32,
            RxArchitecture.Int72));
        var aDest = OpSrc(instr, 2);
        m.Assign(aDest, m.Shl(product, 16));
    }

    private void RewriteMvfac(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        var src0 = OpSrc(instr, 0);
        var src1 = OpSrc(instr, 1);
        src1 = m.Fn(intrinsic, src0, src1);
        OpDst(instr, 2, src1);
    }

    private void RewriteMvtac(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        var src0 = OpSrc(instr, 0);
        var src1 = OpSrc(instr, 1);
        src1 = m.Fn(intrinsic, src0, src1);
        OpDst(instr, 1, src1);
    }

    private void RewriteMvfc(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        src = m.Fn(mvfc_intrinsic, src);
        OpDst(instr, 1, src);
    }

    private void RewriteMvtc(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        var dst = OpSrc(instr, 1);
        m.SideEffect(m.Fn(mvtc_intrinsic, dst, src));
    }

    private void RewriteMvtipl(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        var psw = binder.EnsureRegister(Registers.PSW);
        m.Assign(psw, m.Fn(mvtipl_intrinsic, psw, src));
    }

    private void RewriteNeg(RxInstruction instr)
    {
        var idDst = RewriteUnary(instr, Operator.Neg);
        EmitCc(idDst, Registers.COSZ);
    }

    private void RewriteNot(RxInstruction instr)
    {
        var idDst = RewriteUnary(instr, Operator.Comp);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteNop(RxInstruction instr)
    {
        m.Nop();
    }

    private void RewriteOr(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.Or);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewritePop(RxInstruction instr)
    {
        var reg = (RegisterStorage) instr.Operands[0];
        var id = binder.EnsureRegister(reg);
        var sp = binder.EnsureRegister(Registers.sp);
        m.Assign(id, m.Mem(id.DataType, sp));
        m.Assign(sp, m.IAddS(sp, id.DataType.Size));
    }

    private void RewritePopc(RxInstruction instr)
    {
        var reg = (RegisterStorage) instr.Operands[0];
        var id = binder.EnsureRegister(reg);
        var sp = binder.EnsureRegister(Registers.sp);
        m.SideEffect(m.Fn(mvtc_intrinsic, id, m.Mem(id.DataType, sp)));
        m.Assign(sp, m.IAddS(sp, id.DataType.Size));
    }

    private void RewritePopm(RxInstruction instr)
    {
        var regRange = (RegisterRange) instr.Operands[0];
        PopRegisterRange(regRange);
    }

    private void PopRegisterRange(RegisterRange regRange)
    {
        var sp = binder.EnsureRegister(Registers.sp);
        for (int iReg = regRange.RegisterIndex; iReg < regRange.RegisterIndex + regRange.Count; ++iReg)
        {
            var reg = Registers.GpRegisters[iReg];
            var id = binder.EnsureRegister(reg);
            m.Assign(id, m.Mem(id.DataType, sp));
            m.Assign(sp, m.IAddS(sp, id.DataType.Size));
        }
    }

    private void RewritePush(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        var sp = binder.EnsureRegister(Registers.sp);
        m.Assign(sp, m.ISubS(sp, src.DataType.Size));
        m.Assign(m.Mem(src.DataType, sp), src);
    }

    private void RewritePushc(RxInstruction instr)
    {
        var src = OpSrc(instr, 0);
        src = m.Fn(mvfc_intrinsic, src);
        var sp = binder.EnsureRegister(Registers.sp);
        m.Assign(sp, m.ISubS(sp, src.DataType.Size));
        m.Assign(m.Mem(src.DataType, sp), src);
    }

    private void RewritePushm(RxInstruction instr)
    {
        RegisterRange regRange = (RegisterRange) instr.Operands[0];
        var sp = binder.EnsureRegister(Registers.sp);
        for (int iReg = regRange.RegisterIndex + regRange.Count - 1; iReg >= regRange.RegisterIndex; --iReg)
        {
            var reg = Registers.GpRegisters[iReg];
            var id = binder.EnsureRegister(reg);
            m.Assign(sp, m.ISubS(sp, id.DataType.Size));
            m.Assign(m.Mem(id.DataType, sp), id);
        }
    }

    private void RewriteRevl(RxInstruction instr)
    {
        RewriteUnary(instr, revl_intrinsic);
    }

    private void RewriteRevw(RxInstruction instr)
    {
        RewriteUnary(instr, revw_intrinsic);
    }

    private void RewriteRmpa(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var acc = binder.EnsureSequence(RxArchitecture.Word96,
            Registers.GpRegisters[6],
            Registers.GpRegisters[5],
            Registers.GpRegisters[4]);
        var tmp = binder.CreateTemporary(acc.DataType);

        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        Debug.Assert(instr.DataType is not null);
        m.Assign(tmp, m.Convert(
            m.SMul(
                PrimitiveType.Int64,
                m.Mem(instr.DataType, r1),
                m.Mem(instr.DataType, r2)),
            PrimitiveType.Int64,
            RxArchitecture.Int96));
        int inc = instr.DataType.BitSize / 8;
        m.Assign(r1, m.IAdd(r1, inc));
        m.Assign(r2, m.IAdd(r2, inc));
        m.Assign(r3, m.ISub(r3, 1));
        m.Goto(instr.Address);
    }

    private void RewriteRoundAccumulator(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        var src = OpSrc(instr, 0);
        OpDst(instr, 1, m.Fn(intrinsic, src));
    }

    private void RewriteRoxc(RxInstruction instr, IntrinsicProcedure roxc)
    {
        var src = OpSrc(instr, 0);
        var cy = binder.EnsureFlagGroup(Registers.C);
        var fn = roxc.MakeInstance(src.DataType, PrimitiveType.Byte);
        var idDst = OpDst(instr, 0, m.Fn(fn, src, m.Byte(1), cy));
        EmitCc(idDst, Registers.CZ);
    }


    private void RewriteRotl(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, CommonOps.Rol);
        EmitCc(idDst, Registers.CZ);
    }

    private void RewriteRotr(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, CommonOps.Ror);
        EmitCc(idDst, Registers.CZ);
    }

    private void RewriteRound(RxInstruction instr)
    {
        var idDst = RewriteUnary(instr, FpOps.roundf);
        EmitCc(idDst, Registers.SZ);
    }

    private void RewriteRte(RxInstruction instr)
    {
        m.SideEffect(m.Fn(rte_intrinsic));
        m.Return(4, 0);
    }

    private void RewriteRtfi(RxInstruction instr)
    {
        m.SideEffect(m.Fn(rtfi_intrinsic));
        m.Return(4, 0);
    }

    private void RewriteRtsd(RxInstruction instr)
    {
        int delta = ((Constant) instr.Operands[0]).ToInt32();
        if (instr.Operands.Length == 1)
        {
            m.Return(4, delta);
        }
        else
        {
            PopRegisterRange((RegisterRange) instr.Operands[1]);
            m.Return(4, delta);
        }
    }

    private void RewriteRts(RxInstruction instr)
    {
        m.Return(4, 0);
    }

    private void RewriteSat(RxInstruction instr)
    {
        var grf = binder.EnsureFlagGroup(Registers.OS);
        var src = OpSrc(instr, 0);
        OpDst(instr, 0, m.Fn(saturate_intrinsic.MakeInstance(src.DataType), grf, src));
    }

    private void RewriteSatr(RxInstruction instr)
    {
        var grf = binder.EnsureFlagGroup(Registers.OS);
        var src = binder.EnsureSequence(
            RxArchitecture.Word96,
            Registers.GpRegisters[6],
            Registers.GpRegisters[5],
            Registers.GpRegisters[4]);
        m.Assign(src, m.Fn(saturate_intrinsic.MakeInstance(src.DataType), grf, src));
    }

    private void RewriteSbb(RxInstruction instr)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, 0);
        var cy = binder.EnsureFlagGroup(Registers.C);
        var idDst = OpDst(instr, 1, m.ISubC(left, right, cy));
        EmitCc(idDst, Registers.COSZ);
    }

    private void RewriteSetpsw(RxInstruction instr)
    {
        var flag = (FlagGroupStorage) instr.Operands[0];
        var grf = binder.EnsureFlagGroup(flag);
        m.Assign(grf, m.Word32(flag.FlagGroupBits));
    }

    private void RewriteShift(RxInstruction instr, BinaryOperator shift)
    {
        var idDst = RewriteBinary(instr, shift);
        EmitCc(idDst, Registers.CZ);
    }

    private void RewriteScc(RxInstruction instr, ConditionCode cc, FlagGroupStorage O)
    {
        var test = m.Test(cc, binder.EnsureFlagGroup(O));
        var dt = instr.Operands[0].DataType;
        OpDst(instr, 0, m.Conditional(dt, test, m.Const(dt, 1), m.Const(dt, 0)));
    }


    private void RewriteScmpu(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp1 = binder.CreateTemporary(PrimitiveType.Byte);
        var tmp2 = binder.CreateTemporary(PrimitiveType.Byte);

        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp1, m.Mem8(r1));
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(tmp2, m.Mem8(r2));
        m.Assign(r2, m.IAdd(r2, 1));
        m.Assign(r3, m.ISub(r3, 1));
        m.Branch(m.Cand(m.Eq(tmp1, tmp2), m.Ne0(tmp1)), instr.Address);
    }

    private void RewriteSmovb(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp, m.Mem8(r2));
        m.Assign(r2, m.ISub(r2, 1));
        m.Assign(m.Mem8(r1), tmp);
        m.Assign(r1, m.ISub(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        m.Goto(instr.Address);
    }

    private void RewriteSmovf(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp, m.Mem8(r2));
        m.Assign(r2, m.IAdd(r2, 1));
        m.Assign(m.Mem8(r1), tmp);
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        m.Goto(instr.Address);
    }

    private void RewriteSmovu(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp, m.Mem8(r2));
        m.Assign(r2, m.IAdd(r2, 1));
        m.Assign(m.Mem8(r1), tmp);
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        m.Branch(m.Ne0(tmp), instr.Address);
    }

    private void RewriteSstr(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Word32);
        Debug.Assert(instr.DataType != null);
        var val = binder.CreateTemporary(instr.DataType);
        m.Assign(val, m.MaybeSlice(r2, val.DataType));
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(m.Mem(instr.DataType, r1), val);
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        m.Goto(instr.Address);
    }

    private void RewriteSt(RxInstruction instr, ConditionCode cc)
    {
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.BranchInMiddleOfInstruction(m.Test(cc, z), instr.Address + instr.Length, InstrClass.ConditionalTransfer);
        var src = OpSrc(instr, 0);
        OpDst(instr, 1, src);
    }

    private void RewriteSub(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.ISub);
        EmitCc(idDst, Registers.COSZ);
    }

    private void RewriteSuntil(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Word32);
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp, m.Convert(m.Mem8(r1), PrimitiveType.Byte, PrimitiveType.Word32));
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        var grf = EmitCc(m.ISub(tmp, r2), Registers.CZ);
        m.Branch(m.Test(ConditionCode.NE, grf), instr.Address);
    }

    private void RewriteSwhile(RxInstruction instr)
    {
        var r1 = binder.EnsureRegister(Registers.GpRegisters[1]);
        var r2 = binder.EnsureRegister(Registers.GpRegisters[2]);
        var r3 = binder.EnsureRegister(Registers.GpRegisters[3]);
        var tmp = binder.CreateTemporary(PrimitiveType.Word32);
        m.BranchInMiddleOfInstruction(m.Eq0(r3), instr.Address + instr.Length, InstrClass.ConditionalTransfer);

        m.Assign(tmp, m.Convert(m.Mem8(r1), PrimitiveType.Byte, PrimitiveType.Word32));
        m.Assign(r1, m.IAdd(r1, 1));
        m.Assign(r3, m.ISub(r3, 1));
        var grf = EmitCc(m.ISub(tmp, r2), Registers.CZ);
        m.Branch(m.Test(ConditionCode.EQ, grf), instr.Address);
    }

    private void RewriteTst(RxInstruction instr)
    {
        var left = OpSrc(instr, 1);
        var right = OpSrc(instr, 0);
        var result = m.And(left, right);
        m.Assign(binder.EnsureFlagGroup(Registers.SZ), m.Cond(PrimitiveType.Word32, result));
    }

    private Identifier RewriteUnary(RxInstruction instr, UnaryOperator unary)
    {
        Expression src = OpSrc(instr, 0);
        src = m.Unary(unary, src);
        var id = OpDst(instr, instr.Operands.Length - 1, src);
        return id;
    }

    private Identifier RewriteUnary(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        Expression src = OpSrc(instr, 0);
        src = m.Fn(intrinsic, src);
        var id = OpDst(instr, instr.Operands.Length - 1, src);
        return id;
    }

    private void RewriteWait(RxInstruction instr)
    {
        m.SideEffect(m.Fn(wait_intrinsic));
    }

    private void RewriteXchg(RxInstruction instr)
    {
        var src1 = OpSrc(instr, 0);
        var src2 = OpSrc(instr, 1);
        var tmp = binder.CreateTemporary(src1.DataType);
        m.Assign(tmp, src1);
        m.Assign(src1, src2);
        m.Assign(src2, tmp);
    }


    private void RewriteXor(RxInstruction instr)
    {
        var idDst = RewriteBinary(instr, Operator.Xor);
        EmitCc(idDst, Registers.SZ);
    }


    private static readonly IntrinsicProcedure brk_intrinsic = IntrinsicBuilder.SideEffect("__brk")
        .Void();

    private static readonly IntrinsicProcedure bmcc_intrinsic = new IntrinsicBuilder("__set_bit", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Bool)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure saturate_intrinsic = new IntrinsicBuilder("__saturate", false)
        .GenericTypes("T")
        .Param(PrimitiveType.Word32)
        .Param("T")
        .Returns("T");

    private static readonly IntrinsicProcedure mvfacgu_intrinsic = new IntrinsicBuilder("__move_from_accumulator_guard", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure mvfachi_intrinsic = new IntrinsicBuilder("__move_from_accumulator_high", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure mvfaclo_intrinsic = new IntrinsicBuilder("__move_from_accumulator_low", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure mvfacmi_intrinsic = new IntrinsicBuilder("__move_from_accumulator_middle", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure mvtacgu_intrinsic = new IntrinsicBuilder("__move_to_accumulator_guard", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure mvtachi_intrinsic = new IntrinsicBuilder("__move_to_accumulator_high", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure mvtaclo_intrinsic = new IntrinsicBuilder("__move_to_accumulator_low", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure mvtacmi_intrinsic = new IntrinsicBuilder("__move_to_accumulator_middle", false)
        .Param(PrimitiveType.Word32)
        .Param(RxArchitecture.Word72)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure mvfc_intrinsic = IntrinsicBuilder.SideEffect("__read_control_register")
        .Param(PrimitiveType.Word32)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure mvtc_intrinsic = IntrinsicBuilder.SideEffect("__write_control_register")
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Word32)
        .Void();
    private static readonly IntrinsicProcedure mvtipl_intrinsic = IntrinsicBuilder.SideEffect("__move_to_interrupt_priority_level")
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Word32)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure racl_intrinsic = new IntrinsicBuilder("__round_accumulator_long", false)
        .Param(PrimitiveType.Word32)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure racw_intrinsic = new IntrinsicBuilder("__round_accumulator_word", false)
        .Param(PrimitiveType.Word32)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure rdacl_intrinsic = new IntrinsicBuilder("__round_down_accumulator_long", false)
        .Param(PrimitiveType.Word32)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure rdacw_intrinsic = new IntrinsicBuilder("__round_down_accumulator_word", false)
        .Param(PrimitiveType.Word32)
        .Returns(RxArchitecture.Word72);
    private static readonly IntrinsicProcedure revl_intrinsic = IntrinsicBuilder.Unary("__reverse_long", PrimitiveType.Word32);
    private static readonly IntrinsicProcedure revw_intrinsic = IntrinsicBuilder.Unary("__reverse_words", PrimitiveType.Word32);
    private static readonly IntrinsicProcedure rte_intrinsic = IntrinsicBuilder.SideEffect("__return_from_exception")
        .Void();
    private static readonly IntrinsicProcedure rtfi_intrinsic = IntrinsicBuilder.SideEffect("__return_from_fast_interrupt")
        .Void();
    private static readonly IntrinsicProcedure wait_intrinsic = IntrinsicBuilder.SideEffect("__wait")
        .Void();
}
