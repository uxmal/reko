#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.Sharp;

public class LH5801Rewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly LH5801Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<LH5801Instruction> dasm;

    public LH5801Rewriter(LH5801Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.dasm = new LH5801Disassembler(arch, rdr).GetEnumerator();
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            switch (instr.Mnemonic)
            {
            case Mnemonic.adc: RewriteAdc(instr); break;
            case Mnemonic.adi: RewriteAdi(instr); break;
            case Mnemonic.adr: RewriteAdr(instr); break;
            case Mnemonic.aex: RewriteAex(); break;
            case Mnemonic.am0: RewriteAm(am0_intrinsic); break;
            case Mnemonic.am1: RewriteAm(am1_intrinsic); break;
            case Mnemonic.and: RewriteLogical(instr, Operator.And); break;
            case Mnemonic.ani: RewriteLogical(instr, Operator.And); break;
            case Mnemonic.atp: RewriteAtp(); break;
            case Mnemonic.att: RewriteAtt(); break;
            case Mnemonic.bch: RewriteBch(instr); break;
            case Mnemonic.bcr: RewriteBcond(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.bcs: RewriteBcond(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.bhr: RewriteBcond(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.bhs: RewriteBcond(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.bii: RewriteBit(instr); break;
            case Mnemonic.bit: RewriteBit(instr); break;
            case Mnemonic.bvr: RewriteBcond(instr, ConditionCode.NO, Registers.V); break;
            case Mnemonic.bvs: RewriteBcond(instr, ConditionCode.OV, Registers.V); break;
            case Mnemonic.bzr: RewriteBcond(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.bzs: RewriteBcond(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.cdv: RewriteCdv(instr); break;
            case Mnemonic.cin: RewriteCin(instr); break;
            case Mnemonic.cpa: RewriteCpa(instr); break;
            case Mnemonic.cpi: RewriteCpi(instr); break;
            case Mnemonic.dca: RewriteDca(instr); break;
            case Mnemonic.dcs: RewriteDcs(instr); break;
            case Mnemonic.dec: RewriteIncDec(instr, -1); break;
            case Mnemonic.drl: RewriteDr(instr, drl_intrinsic); break;
            case Mnemonic.drr: RewriteDr(instr, drr_intrinsic); break;
            case Mnemonic.eai: RewriteLogical(instr, Operator.Xor); break;
            case Mnemonic.eor: RewriteLogical(instr, Operator.Xor); break;
            case Mnemonic.hlt: RewriteHlt(); break;
            case Mnemonic.inc: RewriteIncDec(instr, 1); break;
            case Mnemonic.ita: RewriteIta(instr); break;
            case Mnemonic.jmp: RewriteJmp(instr); break;
            case Mnemonic.lda: RewriteLda(instr); break;
            case Mnemonic.lde: RewriteLdecInc(instr, -1); break;
            case Mnemonic.ldi: RewriteLdi(instr); break;
            case Mnemonic.ldx: RewriteLdx(instr); break;
            case Mnemonic.lin: RewriteLdecInc(instr, 1); break;
            case Mnemonic.lop: RewriteLop(instr); break;
            case Mnemonic.nop: m.Nop(); break;
            case Mnemonic.off: RewriteOff(instr); break;
            case Mnemonic.ora: RewriteLogical(instr, Operator.Or); break;
            case Mnemonic.ori: RewriteLogical(instr, Operator.Or); break;
            case Mnemonic.pop: RewritePop(instr); break;
            case Mnemonic.psh: RewritePsh(instr); break;
            case Mnemonic.rdp: RewriteRdp(); break;
            case Mnemonic.rec: RewriteRec(); break;
            case Mnemonic.rie: RewriteRie(); break;
            case Mnemonic.rol: RewriteRotate(CommonOps.RolC, 0x80); break;
            case Mnemonic.ror: RewriteRotate(CommonOps.RorC, 0x01); break;
            case Mnemonic.rpu: RewriteRpu(); break;
            case Mnemonic.rpv: RewriteRpv(); break;
            case Mnemonic.rti: RewriteRti(); break;
            case Mnemonic.rtn: RewriteRtn(); break;
            case Mnemonic.sbc: RewriteSbc(instr); break;
            case Mnemonic.sbi: RewriteSbi(instr); break;
            case Mnemonic.sde: RewriteSdecInc(instr, -1); break;
            case Mnemonic.sdp: RewriteSdp(); break;
            case Mnemonic.sec: RewriteSec(); break;
            case Mnemonic.shl: RewriteShift(Operator.Shl, 0x80); break;
            case Mnemonic.shr: RewriteShift(Operator.Shr, 0x01); break;
            case Mnemonic.sie: RewriteSie(instr); break;
            case Mnemonic.sin: RewriteSdecInc(instr, 1); break;
            case Mnemonic.sjp: RewriteSjp(instr); break;
            case Mnemonic.spu: RewriteSpu(); break;
            case Mnemonic.spv: RewriteSpv(); break;
            case Mnemonic.sta: RewriteSta(instr); break;
            case Mnemonic.stx: RewriteStx(instr); break;
            case Mnemonic.tin: RewriteTin(); break;
            case Mnemonic.tta: RewriteTta(); break;
            case Mnemonic.vcr: RewriteVcond(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.vcs: RewriteVcond(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.vej: RewriteVej(instr); break;
            case Mnemonic.vhr: RewriteVcond(instr, ConditionCode.ULT, Registers.H); break;
            case Mnemonic.vhs: RewriteVcond(instr, ConditionCode.ULT, Registers.H); break;
            case Mnemonic.vmj: RewriteVmj(instr); break;
            case Mnemonic.vvs: RewriteVcond(instr, ConditionCode.NO, Registers.V); break;
            case Mnemonic.vzr: RewriteVcond(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.vzs: RewriteVcond(instr, ConditionCode.NE, Registers.Z); break;
            default:
                EmitUnitTest(instr);
                break;
            }
            var cluster = m.MakeCluster(instr.Address, instr.Length, instr.InstructionClass);
            yield return cluster;
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitUnitTest(LH5801Instruction instr)
    {
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        testSvc?.ReportMissingDecoder("LH5801Rw", instr.Address, rdr, "");
        m.Nop();
    }

    private Expression OpDst(LH5801Instruction instr, int iOp, Expression src)
    {
        var op = instr.Operands[iOp];
        switch (op)
        {
        case RegisterStorage reg:
            var r = binder.EnsureRegister(reg);
            m.Assign(r, src);
            return r;
        case MemoryOperand mem:
            Expression mm = RewriteMem(mem, src.DataType);
            if (src is not Identifier &&
                src is not Constant)
            {
                var t = binder.CreateTemporary(src.DataType);
                m.Assign(t, src);
                src = t;
            }
            m.Assign(mm, src);
            return src;
        }
        throw new NotImplementedException(op.GetType().Name);
    }

    private Expression OpSrc(LH5801Instruction instr, int iOp, DataType? dt = null)
    {
        var op = instr.Operands[iOp];
        switch (op)
        {
        case RegisterStorage reg:
            return binder.EnsureRegister(reg);
        case Constant c:
            return c;
        case Address a:
            return a;
        case MemoryOperand mem:
            return RewriteMem(mem, dt ?? PrimitiveType.Byte);
        }
        throw new NotImplementedException(op.GetType().Name);
    }

    private Expression RewriteMem(MemoryOperand mem, DataType dt)
    {
        Expression ea = mem.EffectiveAddress switch
        {
            RegisterStorage r => binder.EnsureRegister(r),
            Address eaddr => eaddr,
            _ => throw new NotImplementedException(mem.EffectiveAddress.GetType().Name)
        };
        if (mem.IsMe1Access)
        {
            return m.SegMem(dt, m.Word16(1), ea);
        }
        else
        {
            return m.Mem(dt, ea);
        }
    }

    private void EmitCond(FlagGroupStorage flags, Expression e)
    {
        var cond = binder.EnsureFlagGroup(flags);
        m.Assign(cond, m.Cond(cond.DataType, e));
    }

    private void RewriteAdc(LH5801Instruction instr)
    {
        var src1 = binder.EnsureRegister(Registers.A);
        var src2 = OpSrc(instr, 0);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(src1, m.IAddC(src1, src2, c));
        EmitCond(Registers.CZVH, src1);
    }

    private void RewriteAdi(LH5801Instruction instr)
    {
        var src1 = OpSrc(instr, 0);
        var src2 = OpSrc(instr, 1);
        Expression src;
        if (instr.Operands[0] == Registers.A)
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            src = m.IAddC(src1, src2, c);
        }
        else
        {
            src = m.IAdd(src1, src2);
        }
        var v = OpDst(instr, 0, src);
        EmitCond(Registers.CZVH, v);
    }

    private void RewriteAdr(LH5801Instruction instr)
    {
        var src = binder.EnsureRegister(Registers.A);
        var dst = OpDst(instr, 0, src);
        m.Assign(dst, m.IAdd(dst, m.ExtendZ(src, dst.DataType)));
        EmitCond(Registers.CZVH, dst);
    }

    private void RewriteAex()
    {
        var a = binder.EnsureRegister(Registers.A);
        m.Assign(a, m.Fn(aex_intrinsic, a));
    }

    private void RewriteAm(IntrinsicProcedure intrinsic)
    {
        var a = binder.EnsureRegister(Registers.A);
        m.SideEffect(m.Fn(intrinsic, a));
    }

    private void RewriteAtp()
    {
        var a = binder.EnsureRegister(Registers.A);
        m.SideEffect(m.Fn(atp_intrinsic, a));
    }

    private void RewriteAtt()
    {
        var a = binder.EnsureRegister(Registers.A);
        m.SideEffect(m.Fn(att_intrinsic, a));
    }

    private void RewriteBch(LH5801Instruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Goto(target);
    }

    private void RewriteLogical(LH5801Instruction instr, BinaryOperator op)
    {
        Expression src1;
        Expression src2;
        Expression dst;
        if (instr.Operands.Length == 1)
        {
            src1 = binder.EnsureRegister(Registers.A);
            src2 = OpSrc(instr, 0);
            m.Assign(src1, m.Bin(op, src1, src2));
            dst = src1;
        }
        else
        {
            src1 = OpSrc(instr, 0);
            src2 = OpSrc(instr, 1);
            dst = OpDst(instr, 0, m.Bin(op, src1, src2));
        }
        EmitCond(Registers.Z, dst);
    }

    private void RewriteBcond(LH5801Instruction instr, ConditionCode cc, FlagGroupStorage grf)
    {
        var target = OpSrc(instr, 0);
        var flags = binder.EnsureFlagGroup(grf);
        m.Branch(m.Test(cc, flags), target);
    }

    private void RewriteBit(LH5801Instruction instr)
    {
        Expression left;
        Expression right;
        if (instr.Operands.Length == 1)
        {
            left = binder.EnsureRegister(Registers.A);
            right = OpSrc(instr, 0);
        }
        else
        {
            left = OpSrc(instr, 0);
            right = OpSrc(instr, 1);
        }
        var tmp = binder.CreateTemporary(left.DataType);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(tmp, m.And(left, right));
        m.Assign(z, m.Cond(z.DataType, tmp));
    }

    private void RewriteCdv(LH5801Instruction instr)
    {
        m.SideEffect(m.Fn(cdv_intrinsic));
    }

    private void RewriteCin(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        var x = binder.EnsureRegister(Registers.X);
        EmitCond(Registers.CZVH, m.ISub(a, m.Mem(a.DataType, x)));
        m.Assign(x, m.IAddS(x, 1));
    }

    private void RewriteCpa(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        var b = OpSrc(instr, 0);
        EmitCond(Registers.CZVH, m.ISub(a, b));
    }

    private void RewriteCpi(LH5801Instruction instr)
    {
        var a = OpSrc(instr, 0);
        var b = OpSrc(instr, 1);
        EmitCond(Registers.CZVH, m.ISub(a, b));
    }

    private void RewriteDca(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        var src = OpSrc(instr, 0);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(a, m.Fn(dca_intrinsic, a, src, c));
        EmitCond(Registers.CZVH, a);
    }


    private void RewriteDcs(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        var src = OpSrc(instr, 0);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(a, m.Fn(dcs_intrinsic, a, src, c));
        EmitCond(Registers.CZVH, a);
    }

    private void RewriteIncDec(LH5801Instruction instr, int delta)
    {
        var reg = OpSrc(instr, 0);
        m.Assign(reg, m.AddSubSignedInt(reg, delta));
        if (reg.DataType.BitSize == 8)
            EmitCond(Registers.CZVH, reg);
    }

    private void RewriteDr(LH5801Instruction instr, IntrinsicProcedure op)
    {
        var a = binder.EnsureRegister(Registers.A);
        var x = binder.EnsureRegister(Registers.X);
        Expression ext;
        if (instr.Operands.Length == 1)
        {
            ext = m.SegMem8(m.Word16(1), x);
        }
        else
        {
            ext = m.Mem8(x);
        }
        m.SideEffect(
            m.Fn(
                op,
                a,
                m.AddrOf(PrimitiveType.Ptr16, ext)));
    }

    private void RewriteHlt()
    {
        m.SideEffect(m.Fn(CommonOps.Halt));
    }

    private void RewriteIta(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        m.Assign(a, m.Fn(ita_intrinsic));
        EmitCond(Registers.Z, m.Eq0(a));
    }


    private void RewriteJmp(LH5801Instruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Goto(target);
    }

    private void RewriteLda(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        m.Assign(a, OpSrc(instr, 0));
        EmitCond(Registers.Z, m.Eq0(a));
    }

    private void RewriteLdecInc(LH5801Instruction instr, int delta)
    {
        var a = binder.EnsureRegister(Registers.A);
        var r = OpSrc(instr, 0);
        m.Assign(a, m.Mem8(r));
        EmitCond(Registers.Z, m.Eq0(a));
        m.Assign(r, m.AddSubSignedInt(r, delta));
    }

    private void RewriteLdi(LH5801Instruction instr)
    {
        var imm = OpSrc(instr, 1);
        OpDst(instr, 0, imm);
        if (instr.Operands[0] == Registers.A)
        {
            var a = binder.EnsureRegister(Registers.A);
            EmitCond(Registers.Z, m.Eq0(a));
        }
    }

    private void RewriteLdx(LH5801Instruction instr)
    {
        var src = OpSrc(instr, 0);
        var dst = binder.EnsureRegister(Registers.X);
        m.Assign(dst, src);
    }

    private void RewriteLop(LH5801Instruction instr)
    {
        var ul = OpSrc(instr, 0);
        var target = OpSrc(instr, 1);
        m.Assign(ul, m.ISub(ul, 1));
        m.Branch(m.Uge(ul, 0), target);
    }

    private void RewriteOff(LH5801Instruction instr)
    {
        m.SideEffect(m.Fn(off_intrinsic));
    }


    private void RewritePop(LH5801Instruction instr)
    {
        var r = OpSrc(instr, 0);
        var s = binder.EnsureRegister(Registers.S);
        if (r.DataType.BitSize == 8)
        {
            m.Assign(s, m.IAddS(s, 1));
            m.Assign(r, m.Mem(r.DataType, s));
            EmitCond(Registers.Z, r);
        }
        else
        {
            m.Assign(s, m.IAddS(s, 2));
            m.Assign(r, m.Mem(r.DataType, s));
        }
    }

    private void RewritePsh(LH5801Instruction instr)
    {
        var r = OpSrc(instr, 0);
        var s = binder.EnsureRegister(Registers.S);
        m.Assign(m.Mem(r.DataType, s), r);
        m.Assign(s, m.ISubS(s, r.DataType.BitSize / 8));
    }

    private void RewriteRdp()
    {
        m.SideEffect(m.Fn(rdp_intrinsic));
    }


    private void RewriteRec()
    {
        m.Assign(binder.EnsureFlagGroup(Registers.C), 0);
    }


    private void RewriteRie()
    {
        m.SideEffect(m.Fn(rie_intrinsic));
    }

    private void RewriteRotate(IntrinsicProcedure op, int bitflag)
    {
        var a = binder.EnsureRegister(Registers.A);
        var c = binder.EnsureFlagGroup(Registers.C);
        var tmp = binder.CreateTemporary(PrimitiveType.Bool);
        m.Assign(tmp, m.Ne0(m.And(a, m.Byte((byte)bitflag))));
        m.Assign(a,
            m.Fn(
                op.MakeInstance(a.DataType, a.DataType),
                a,
                m.Byte(1),
                c));
        EmitCond(Registers.C, tmp);
    }

    private void RewriteRpu()
    {
        m.SideEffect(m.Fn(rpu_intrinsic));
    }


    private void RewriteRpv()
    {
        m.SideEffect(m.Fn(rpv_intrinsic));
    }


    private void RewriteRti()
    {
        m.SideEffect(m.Fn(rti_intrinsic));
        m.Return(2, 0);
    }


    private void RewriteRtn()
    {
        m.Return(2, 0);
    }

    private void RewriteSbc(LH5801Instruction instr)
    {
        var src1 = binder.EnsureRegister(Registers.A);
        var src2 = OpSrc(instr, 0);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(src1, m.ISubC(src1, src2, c));
        EmitCond(Registers.CZVH, src1);
    }


    private void RewriteSbi(LH5801Instruction instr)
    {
        var src1 = OpSrc(instr, 0);
        var src2 = OpSrc(instr, 1);
        Expression src;
        if (instr.Operands[0] == Registers.A)
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            src = m.ISubC(src1, src2, c);
        }
        else
        {
            src = m.ISub(src1, src2);
        }
        var v = OpDst(instr, 0, src);
        EmitCond(Registers.CZVH, v);

    }

    private void RewriteSdecInc(LH5801Instruction instr, int delta)
    {
        var a = binder.EnsureRegister(Registers.A);
        var r = OpSrc(instr, 0);
        m.Assign(m.Mem8(r), a);
        m.Assign(r, m.AddSubSignedInt(r, delta));
    }

    private void RewriteSdp()
    {
        m.SideEffect(m.Fn(sdp_intrinsic));
    }


    private void RewriteSec()
    {
        m.Assign(binder.EnsureFlagGroup(Registers.C), 1);
    }


    private void RewriteShift(BinaryOperator op, int mask)
    {
        var a = binder.EnsureRegister(Registers.A);
        var c = binder.EnsureFlagGroup(Registers.C);
        var tmp = binder.CreateTemporary(PrimitiveType.Bool);
        m.Assign(tmp, m.Ne0(m.And(a, m.Byte((byte) mask))));
        m.Assign(a, m.Bin(op, a, m.Byte(1)));
        EmitCond(Registers.C, tmp);
    }

    private void RewriteSie(LH5801Instruction instr)
    {
        m.SideEffect(m.Fn(sie_intrinsic));
    }

    private void RewriteSjp(LH5801Instruction instr)
    {
        var target = OpSrc(instr, 0);
        m.Call(target, 2);
    }

    private void RewriteSpu()
    {
        m.SideEffect(m.Fn(spu_intrinsic));
    }

    private void RewriteSpv()
    {
        m.SideEffect(m.Fn(spv_intrinsic));
    }

    private void RewriteSta(LH5801Instruction instr)
    {
        var a = binder.EnsureRegister(Registers.A);
        OpDst(instr, 0, a);
    }


    private void RewriteStx(LH5801Instruction instr)
    {
        var x = binder.EnsureRegister(Registers.X);
        var r = OpSrc(instr, 0);
        m.Assign(m.Mem(x.DataType, r), x);
    }

    private void RewriteTin()
    {
        var x = binder.EnsureRegister(Registers.X);
        var y = binder.EnsureRegister(Registers.Y);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        m.Assign(t, m.Mem8(x));
        m.Assign(m.Mem8(y), t);
        m.Assign(x, m.IAdd(x, 1));
        m.Assign(y, m.IAdd(y, 1));
    }

    private void RewriteTta()
    {
        var a = binder.EnsureRegister(Registers.A);
        m.Assign(a, m.Fn(tta_intrinsic));
        EmitCond(Registers.Z, m.Eq0(a));
    }

    private void RewriteVcond(LH5801Instruction instr, ConditionCode cc, FlagGroupStorage grf)
    {
        var imm = 0xFF00u + ((Constant) instr.Operands[0]).ToUInt16();
        m.BranchInMiddleOfInstruction(
            m.Test(cc, binder.EnsureFlagGroup(grf)),
            instr.Address + instr.Length,
            InstrClass.CondJump);
        m.Call(Address.Ptr16((ushort) imm), 2);
    }

    private void RewriteVej(LH5801Instruction instr)
    {
        var imm = 0xFF00u + ((Constant) instr.Operands[0]).ToUInt16();
        m.Call(Address.Ptr16((ushort) imm), 2);
    }


    private void RewriteVmj(LH5801Instruction instr)
    {
        var imm = 0xFF00u + ((Constant) instr.Operands[0]).ToUInt16();
        m.Call(Address.Ptr16((ushort) imm), 2);
    }


    private static readonly IntrinsicProcedure aex_intrinsic = IntrinsicBuilder.Pure("__exchange_nybbles")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Byte);
    private static readonly IntrinsicProcedure am0_intrinsic = IntrinsicBuilder.Pure("__write_timer0")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure am1_intrinsic = IntrinsicBuilder.Pure("__write_timer1")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure atp_intrinsic = IntrinsicBuilder.Pure("__write_port")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure att_intrinsic = IntrinsicBuilder.Pure("__write_t_reg")
        .Param(PrimitiveType.Byte)
        .Void();
    private static readonly IntrinsicProcedure cdv_intrinsic = IntrinsicBuilder.SideEffect("__clear_divider")
        .Void();
    private static readonly IntrinsicProcedure dca_intrinsic = IntrinsicBuilder.Pure("__decimal_add")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Byte);
    private static readonly IntrinsicProcedure dcs_intrinsic = IntrinsicBuilder.Pure("__decimal_subtract")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Byte);
    private static readonly IntrinsicProcedure drl_intrinsic = IntrinsicBuilder.SideEffect("__digit_rotate_left")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Ptr16)
        .Void();
    private static readonly IntrinsicProcedure drr_intrinsic = IntrinsicBuilder.SideEffect("__digit_rotate_right")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Ptr16)
        .Void();
    private static readonly IntrinsicProcedure ita_intrinsic = IntrinsicBuilder.SideEffect("__read_port")
        .Returns(PrimitiveType.Byte);
    private static readonly IntrinsicProcedure off_intrinsic = IntrinsicBuilder.SideEffect("__off")
        .Void();
    private static readonly IntrinsicProcedure rdp_intrinsic = IntrinsicBuilder.SideEffect("__reset_disp")
        .Void();
    private static readonly IntrinsicProcedure rie_intrinsic = IntrinsicBuilder.SideEffect("__disable_interrupts")
        .Void();
    private static readonly IntrinsicProcedure rpu_intrinsic = IntrinsicBuilder.SideEffect("__reset_pu")
        .Void();
    private static readonly IntrinsicProcedure rpv_intrinsic = IntrinsicBuilder.SideEffect("__reset_pv")
        .Void();
    private static readonly IntrinsicProcedure rti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
        .Void();
    private static readonly IntrinsicProcedure sdp_intrinsic = IntrinsicBuilder.SideEffect("__set_disp")
        .Void();
    private static readonly IntrinsicProcedure spu_intrinsic = IntrinsicBuilder.SideEffect("__set_pu")
        .Void();
    private static readonly IntrinsicProcedure spv_intrinsic = IntrinsicBuilder.SideEffect("__set_pv")
        .Void();
    private static readonly IntrinsicProcedure sie_intrinsic = IntrinsicBuilder.SideEffect("__enable_interrupts")
        .Void();
    private static readonly IntrinsicProcedure tta_intrinsic = IntrinsicBuilder.SideEffect("__read_t_reg")
        .Returns(PrimitiveType.Byte);
}
