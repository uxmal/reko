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
using Reko.Core.Intrinsics;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections;
using System.Collections.Generic;
using Reko.Core.Operators;
using System.Diagnostics;

namespace Reko.Arch.Rca;

public class Rca1802Rewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly Rca1802Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<Rca1802Instruction> dasm;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private InstrClass iclass;

    public Rca1802Rewriter(Rca1802Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new Rca1802Disassembler(arch, rdr).GetEnumerator();
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            case Mnemonic.Invalid:
                iclass = InstrClass.Invalid;
                m.Invalid();
                break;
            default:
                EmitUnitTest(instr);
                break;
            case Mnemonic.ADD: RewriteAdd(instr); break;
            case Mnemonic.ADI: RewriteAdi(instr); break;
            case Mnemonic.ANI: RewriteAni(instr); break;
            case Mnemonic.AND: RewriteAnd(instr); break;
            case Mnemonic.B1: RewriteBranch(instr, __b1_intrinsic, false); break;
            case Mnemonic.B2: RewriteBranch(instr, __b2_intrinsic, false); break;
            case Mnemonic.B3: RewriteBranch(instr, __b3_intrinsic, false); break;
            case Mnemonic.B4: RewriteBranch(instr, __b4_intrinsic, false); break;
            case Mnemonic.BDF: RewriteBranch(instr, __bdf_intrinsic, false); break;
            case Mnemonic.BN1: RewriteBranch(instr, __b1_intrinsic, true); break;
            case Mnemonic.BN2: RewriteBranch(instr, __b2_intrinsic, true); break;
            case Mnemonic.BN3: RewriteBranch(instr, __b3_intrinsic, true); break;
            case Mnemonic.BN4: RewriteBranch(instr, __b4_intrinsic, true); break;
            case Mnemonic.BNF: RewriteBranch(instr, __bdf_intrinsic, true); break;
            case Mnemonic.BNQ: RewriteBq(instr, true); break;
            case Mnemonic.BNZ: RewriteBranch(instr, Operator.Ne); break;
            case Mnemonic.BQ: RewriteBq(instr, false); break;
            case Mnemonic.BR: RewriteBr(instr); break;
            case Mnemonic.BZ: RewriteBranch(instr, Operator.Eq); break;
            case Mnemonic.CALL: RewriteCall(instr); break;
            case Mnemonic.DEC: RewriteDec(instr); break;
            case Mnemonic.EXIT: RewriteExit(); break;
            case Mnemonic.GHI: RewriteGet(instr, 8); break;
            case Mnemonic.GLO: RewriteGet(instr, 0); break;
            case Mnemonic.IDL: RewriteIdl(instr); break;
            case Mnemonic.INC: RewriteInc(instr); break;
            case Mnemonic.INP1: RewriteInp(instr, 1); break;
            case Mnemonic.INP2: RewriteInp(instr, 2); break;
            case Mnemonic.INP3: RewriteInp(instr, 3); break;
            case Mnemonic.INP4: RewriteInp(instr, 4); break;
            case Mnemonic.INP5: RewriteInp(instr, 5); break;
            case Mnemonic.INP6: RewriteInp(instr, 6); break;
            case Mnemonic.INP7: RewriteInp(instr, 7); break;
            case Mnemonic.IRX: RewriteIrx(instr); break;
            case Mnemonic.LBDF: RewriteBranch(instr, __bdf_intrinsic, false); break;
            case Mnemonic.LBNF: RewriteBranch(instr, __bdf_intrinsic, true); break;
            case Mnemonic.LBNQ: RewriteBq(instr, true); break;
            case Mnemonic.LBQ: RewriteBq(instr, false); break;
            case Mnemonic.LBR: RewriteLbr(instr); break;
            case Mnemonic.LDA: RewriteLda(instr); break;
            case Mnemonic.LDI: RewriteLdi(instr); break;
            case Mnemonic.LDN: RewriteLdn(instr); break;
            case Mnemonic.LDX: RewriteLdx(instr); break;
            case Mnemonic.LDXA: RewriteLdxa(instr); break;
            case Mnemonic.LNBR: RewriteSkip(instr, 2); break;
            case Mnemonic.LSIE: RewriteSkip(instr, __lsie_intrinsic, false, 2); break;
            case Mnemonic.LSKP: RewriteSkip(instr, 2); break;
            case Mnemonic.LSNF: RewriteSkip(instr, __bdf_intrinsic, true, 2); break;
            case Mnemonic.LSNZ: RewriteSkip(instr, Operator.Ne, 2); break;
            case Mnemonic.LSZ: RewriteSkip(instr, Operator.Eq, 2); break;
            case Mnemonic.NBR: RewriteSkip(instr, 1); break;
            case Mnemonic.NOP: RewriteNop(); break;
            case Mnemonic.OR: RewriteOr(instr); break;
            case Mnemonic.ORI: RewriteOri(instr); break;
            case Mnemonic.OUT1: RewriteOut(1); break;
            case Mnemonic.OUT2: RewriteOut(2); break;
            case Mnemonic.OUT3: RewriteOut(3); break;
            case Mnemonic.OUT4: RewriteOut(4); break;
            case Mnemonic.OUT5: RewriteOut(5); break;
            case Mnemonic.OUT6: RewriteOut(6); break;
            case Mnemonic.OUT7: RewriteOut(7); break;
            case Mnemonic.PHI: RewritePut(instr, 8); break;
            case Mnemonic.PLO: RewritePut(instr, 0); break;
            case Mnemonic.REQ: RewriteReq(); break;
            case Mnemonic.RET: RewriteRet(); break;
            case Mnemonic.SAV: RewriteSav(); break;
            case Mnemonic.SDBI: RewriteSdbi(instr); break;
            case Mnemonic.SEP: RewriteSep(instr); break;
            case Mnemonic.SEQ: RewriteSeq(); break;
            case Mnemonic.SEX: RewriteSex(instr); break;
            case Mnemonic.SHL: RewriteShl(instr); break;
            case Mnemonic.SHLC: RewriteShlc(instr); break;
            case Mnemonic.SHR: RewriteShr(instr); break;
            case Mnemonic.SHRC: RewriteShrc(instr); break;
            case Mnemonic.SKP: RewriteSkip(instr, 1); break;
            case Mnemonic.SM: RewriteSm(instr); break;
            case Mnemonic.SMI: RewriteSmi(instr); break;
            case Mnemonic.SMB: RewriteSmb(instr); break;
            case Mnemonic.STR: RewriteStr(instr); break;
            case Mnemonic.STXD: RewriteStxd(instr); break;
            case Mnemonic.XOR: RewriteXor(instr); break;
            case Mnemonic.XRI: RewriteXri(instr); break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitUnitTest(Rca1802Instruction instr)
    {
        var svc = arch.Services.GetService<ITestGenerationService>();
        svc?.ReportMissingRewriter("Rca1802Rw", instr, instr.Mnemonic.ToString(), rdr, "");
        m.Nop();
    }

    private void RewriteAdd(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.IAdd(t, d));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteAdi(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.IAdd(d, (Constant) instr.Operands[0]));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteAnd(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.And(t, d));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteAni(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.And(d, (Constant) instr.Operands[0]));
    }

    private void RewriteBr(Rca1802Instruction instr)
    {
        m.Goto((Address) instr.Operands[0]);
    }

    private void RewriteBq(Rca1802Instruction instr, bool invert)
    {
        Expression pred = m.Fn(__bq_intrinsic);
        if (invert)
            pred = m.Not(pred);
        m.Branch(pred, (Address) instr.Operands[0]);
    }
    private void RewriteBranch(Rca1802Instruction instr, BinaryOperator op)
    {
        var d = binder.EnsureRegister(Registers.D);
        var target = (Address) instr.Operands[0];
        m.Branch(m.Bin(op, PrimitiveType.Bool, d, m.Byte(0)), target);
    }

    private void RewriteBranch(Rca1802Instruction instr, IntrinsicProcedure pred, bool invert)
    {
        Expression p = m.Fn(pred);
        if (invert)
            p = m.Not(p);
        var target = (Address) instr.Operands[0];
        m.Branch(p, target);
    }

    private void RewriteCall(Rca1802Instruction instr)
    {
        var target = (Address) instr.Operands[0];
        m.Call(target, 2);
    }

    private void RewriteDec(Rca1802Instruction instr)
    {
        var rn = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
        m.Assign(rn, m.ISub(rn, 1));
    }

    private void RewriteExit()
    {
        m.Return(2, 0);
    }

    private void RewriteGet(Rca1802Instruction instr, int offset)
    {
        var reg = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(d, m.Slice(reg, PrimitiveType.Byte, offset));
    }

    private void RewriteIdl(Rca1802Instruction instr)
    {
        m.SideEffect(m.Fn(__idl_intrinsic));
    }

    private void RewriteInc(Rca1802Instruction instr)
    {
        var rn = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
        m.Assign(rn, m.IAdd(rn, 1));
    }

    private void RewriteInp(Rca1802Instruction instr, int port)
    {
        var x = binder.EnsureRegister(Registers.X);
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(d, m.Fn(
            __in_intrinsic,
            m.Byte((byte) port)));
        m.Assign(x, d);
        m.Assign(x, m.Fn(__irx_intrinsic, x));
    }

    private void RewriteIrx(Rca1802Instruction instr)
    {
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(x, m.Fn(__irx_intrinsic, x));
    }

    private void RewriteLbr(Rca1802Instruction instr)
    {
        var target = (Address) instr.Operands[0];
        m.Goto(target);
    }

    private void RewriteLda(Rca1802Instruction instr)
    {
        var rn = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(d, m.Mem8(rn));
        m.Assign(rn, m.IAddS(rn, 1));
    }

    private void RewriteLdi(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(d, (Constant) instr.Operands[0]);
    }

    private void RewriteLdn(Rca1802Instruction instr)
    {
        var rn = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(d, m.Mem8(rn));
    }

    private void RewriteLdx(Rca1802Instruction instr)
    {
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(tmp, m.Fn(__readX_intrinsic, x));
        m.Assign(d, m.Mem8(tmp));
    }

    private void RewriteLdxa(Rca1802Instruction instr)
    {
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(tmp, m.Fn(__readX_intrinsic, x));
        m.Assign(d, m.Mem8(tmp));
        m.Assign(x, m.Fn(__irx_intrinsic, x));
    }

    private void RewriteNop()
    {
        m.Nop();
    }

    private void RewriteOr(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.Or(t, d));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteOri(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.Or(d, (Constant) instr.Operands[0]));
    }

    private void RewriteOut(int port)
    {
        var x = binder.EnsureRegister(Registers.X);
        m.SideEffect(m.Fn(
            __out_intrinsic,
            m.Byte((byte) port),
            m.Fn(__readX_intrinsic, x)));
        m.Assign(x, m.Fn(__irx_intrinsic, x));
    }

    private void RewritePut(Rca1802Instruction instr, int bitOffset)
    {
        var reg = (RegisterStorage) instr.Operands[0];
        var dst = binder.EnsureRegister(reg);
        var d = binder.EnsureRegister(Registers.D);
        m.Assign(dst, m.Dpb(dst, d, bitOffset));
    }

    private void RewriteReq()
    {
        m.SideEffect(m.Fn(__req_intrinsic));
    }

    private void RewriteRet()
    {
        m.SideEffect(m.Fn(__ret_intrinsic));
    }

    private void RewriteSav()
    {
        var t = binder.EnsureRegister(Registers.T);
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(
            m.Mem8(m.Fn(__readX_intrinsic, x)),
            t);
    }

    private void RewriteSdbi(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.ISubB((Constant) instr.Operands[0], d, df));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteSep(Rca1802Instruction instr)
    {
        var reg = (RegisterStorage) instr.Operands[0];
        m.Goto(binder.EnsureRegister(reg));
    }

    private void RewriteSeq()
    {
        m.SideEffect(m.Fn(__seq_intrinsic));
    }

    private void RewriteSex(Rca1802Instruction instr)
    {
        var reg = (RegisterStorage) instr.Operands[0];
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(x, reg.Number);
    }

    private void RewriteShl(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(df, m.Cond(df.DataType, m.And(d, 0x80)));
        m.Assign(d, m.Shl(d, 1));
    }

    private void RewriteShlc(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(df, m.Cond(df.DataType, m.And(d, 0x80)));
        var b = PrimitiveType.Byte;
        m.Assign(d, m.Fn(
            CommonOps.RolC.MakeInstance(b, b),
            d, m.Byte(1), df));
    }

    private void RewriteShr(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(df, m.Cond(df.DataType, m.And(d, 1)));
        m.Assign(d, m.Shr(d, 1));
    }

    private void RewriteShrc(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(df, m.Cond(df.DataType, m.And(d, 1)));
        var b = PrimitiveType.Byte;
        m.Assign(d, m.Fn(
            CommonOps.RorC.MakeInstance(b, b),
            d, m.Byte(1), df));
    }

    private void RewriteSkip(Rca1802Instruction instr, int skip)
    {
        var d = binder.EnsureRegister(Registers.D);
        var addr = instr.Address + (instr.Length + skip);
        m.Goto(addr);
    }

    private void RewriteSkip(Rca1802Instruction instr, BinaryOperator cond, int skip)
    {
        var d = binder.EnsureRegister(Registers.D);
        var pred = m.Bin(cond, PrimitiveType.Bool, d, m.Byte(0));
        var addr = instr.Address + (instr.Length + skip);
        m.Branch(pred, addr);
    }

    private void RewriteSkip(Rca1802Instruction instr, IntrinsicProcedure fn, bool invert, int skip)
    {
        var d = binder.EnsureRegister(Registers.D);
        Expression pred = m.Fn(fn);
        if (invert)
            pred = m.Not(pred);
        var addr = instr.Address + (instr.Length + skip);
        m.Branch(pred, addr);
    }


    private void RewriteSm(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.ISub(t, d));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteSmb(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.ISubB(t, d, df));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteSmi(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.ISub(d, (Constant) instr.Operands[0]));
    }

    private void RewriteStr(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var reg = (RegisterStorage) instr.Operands[0];
        var dst = binder.EnsureRegister(reg);
        m.Assign(m.Mem8(dst), d);
    }

    private void RewriteStxd(Rca1802Instruction instr)
    {
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        m.Assign(tmp, m.Fn(__readX_intrinsic, x));
        m.Assign(m.Mem8(tmp), d);
        m.Assign(x, m.Fn(__drx_intrinsic, x));
    }

    private void RewriteXor(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var x = binder.EnsureRegister(Registers.X);
        var t = binder.CreateTemporary(PrimitiveType.Byte);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(t, m.Fn(__readX_intrinsic, x));
        m.Assign(t, m.Mem8(t));
        m.Assign(d, m.Xor(t, d));
        m.Assign(df, m.Cond(PrimitiveType.Byte, d));
    }

    private void RewriteXri(Rca1802Instruction instr)
    {
        var d = binder.EnsureRegister(Registers.D);
        var df = binder.EnsureFlagGroup(Registers.DF);
        m.Assign(d, m.Xor(d, (Constant) instr.Operands[0]));
    }



    private static readonly IntrinsicProcedure __b1_intrinsic = IntrinsicBuilder.Predicate("__is_EF1_set");
    private static readonly IntrinsicProcedure __b2_intrinsic = IntrinsicBuilder.Predicate("__is_EF2_set");
    private static readonly IntrinsicProcedure __b3_intrinsic = IntrinsicBuilder.Predicate("__is_EF3_set");
    private static readonly IntrinsicProcedure __b4_intrinsic = IntrinsicBuilder.Predicate("__is_EF4_set");
    private static readonly IntrinsicProcedure __bdf_intrinsic = IntrinsicBuilder.Predicate("__is_DF_set");
    private static readonly IntrinsicProcedure __bq_intrinsic = IntrinsicBuilder.Predicate("__is_Q_set");

    private static readonly IntrinsicProcedure __drx_intrinsic = IntrinsicBuilder.SideEffect("__drx")
        .Param(Registers.X.DataType)
        .Returns(Registers.X.DataType);

    private static readonly IntrinsicProcedure __idl_intrinsic = IntrinsicBuilder.SideEffect("__idle")
        .Void();

    private static readonly IntrinsicProcedure __in_intrinsic = IntrinsicBuilder.SideEffect("__input")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Byte);

    private static readonly IntrinsicProcedure __irx_intrinsic = IntrinsicBuilder.SideEffect("__irx")
        .Param(Registers.X.DataType)
        .Returns(Registers.X.DataType);

    private static readonly IntrinsicProcedure __lsie_intrinsic = IntrinsicBuilder.Predicate("__interrupts_enabled");
    private static readonly IntrinsicProcedure __out_intrinsic = IntrinsicBuilder.SideEffect("__output")
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Void();

    private static readonly IntrinsicProcedure __readX_intrinsic = IntrinsicBuilder.Pure("__readX")
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word16);
    private static readonly IntrinsicProcedure __req_intrinsic = IntrinsicBuilder.SideEffect("__reset_q")
        .Void();
    private static readonly IntrinsicProcedure __ret_intrinsic = IntrinsicBuilder.SideEffect("__ret")
        .Void();

    private static readonly IntrinsicProcedure __seq_intrinsic = IntrinsicBuilder.SideEffect("__set_q")
        .Void();

    private static readonly IntrinsicProcedure __writeX_intrinsic = IntrinsicBuilder.SideEffect("__writeX")
        .Param(PrimitiveType.Byte)
        .Void();

}