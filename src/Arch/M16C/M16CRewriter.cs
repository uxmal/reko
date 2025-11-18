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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.M16C;

internal class M16CRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly M16CArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<M16CInstruction> dasm;
    private readonly List<RtlInstruction> instrs;
    private readonly RtlEmitter m;
    private M16CInstruction instr;
    private InstrClass iclass;

    public M16CRewriter(M16CArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new M16CDisassembler(arch, rdr).GetEnumerator();
        this.instrs = new List<RtlInstruction>();
        this.m = new RtlEmitter(instrs);
        this.instr = default!;
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            this.instr = dasm.Current;
            this.iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {

            default:
                EmitUnitTest();
                goto case Mnemonic.Invalid;
            case Mnemonic.Invalid:
                m.Invalid();
                iclass = InstrClass.Invalid;
                break;
            case Mnemonic.adc: RewriteAdc(); break;
            case Mnemonic.adcf: RewriteAdcf(); break;
            case Mnemonic.add: RewriteAdd(); break;
            case Mnemonic.and: RewriteAnd(); break;
            case Mnemonic.band: RewriteBand(); break;
            case Mnemonic.bmeq: RewriteBmCnd(TestEq); break;
            case Mnemonic.bmne: RewriteBmCnd(TestNe); break;
            case Mnemonic.bclr: RewriteBclr(); break;
            case Mnemonic.bnot: RewriteBnot(); break;
            case Mnemonic.brk: RewriteBrk(); break;
            case Mnemonic.bset: RewriteBset(); break;
            case Mnemonic.btst: RewriteBtst(); break;
            case Mnemonic.cmp: RewriteCmp(); break;
            case Mnemonic.dec: RewriteIncDec(-1); break;
            case Mnemonic.div: RewriteDiv(Operator.SDiv, Operator.SMod); break;
            case Mnemonic.divu: RewriteDiv(Operator.UDiv, Operator.UMod); break;
            case Mnemonic.enter: RewriteEnter(); break;
            case Mnemonic.exitd: RewriteExitd(); break;
            case Mnemonic.fclr: RewriteFclr(); break;
            case Mnemonic.fset: RewriteFset(); break;
            case Mnemonic.inc: RewriteIncDec(1); break;
            case Mnemonic.jeq: RewriteJCnd(TestEq); break;
            case Mnemonic.jge: RewriteJCnd(TestGe); break;
            case Mnemonic.jgeu: RewriteJCnd(TestGeu); break;
            case Mnemonic.jgt: RewriteJCnd(TestGt); break;
            case Mnemonic.jgtu: RewriteJCnd(TestGtu); break;
            case Mnemonic.jle: RewriteJCnd(TestLe); break;
            case Mnemonic.jleu: RewriteJCnd(TestLeu); break;
            case Mnemonic.jlt: RewriteJCnd(TestLt); break;
            case Mnemonic.jltu: RewriteJCnd(TestLtu); break;
            case Mnemonic.jmp: RewriteJmp(); break;
            case Mnemonic.jmpi: RewriteJmp(); break;
            case Mnemonic.jn: RewriteJCnd(TestN); break;
            case Mnemonic.jne: RewriteJCnd(TestNe); break;
            case Mnemonic.jpz: RewriteJCnd(TestPz); break;
            case Mnemonic.jsr: RewriteJsr(); break;
            case Mnemonic.ldc: RewriteLdc(); break;
            case Mnemonic.lde: RewriteLde(); break;
            case Mnemonic.mov: RewriteMov(); break;
            case Mnemonic.mul: RewriteMul(Operator.SMul); break;
            case Mnemonic.mulu: RewriteMul(Operator.UMul); break;
            case Mnemonic.nop: RewriteNop(); break;
            case Mnemonic.not: RewriteNot(); break;
            case Mnemonic.or: RewriteOr(); break;
            case Mnemonic.pop: RewritePop(); break;
            case Mnemonic.popm: RewritePopm(); break;
            case Mnemonic.push: RewritePush(); break;
            case Mnemonic.pushm: RewritePushm(); break;
            case Mnemonic.reit: RewriteReit(); break;
            case Mnemonic.rolc: RewriteRotateC(CommonOps.RolC); break;
            case Mnemonic.rorc: RewriteRotateC(CommonOps.RorC); break;
            case Mnemonic.rot: RewriteRotate(); break;
            case Mnemonic.rts: RewriteRts(); break;
            case Mnemonic.sbb: RewriteSbb(); break;
            case Mnemonic.sha: RewriteSha(); break;
            case Mnemonic.shl: RewriteShl(); break;
            case Mnemonic.smovf: RewriteSmovf(); break;
            case Mnemonic.sstr: RewriteSstr(); break;
            case Mnemonic.ste: RewriteSte(); break;
            case Mnemonic.stnz: RewriteSt(TestEq); break;
            case Mnemonic.stz: RewriteSt(TestNe); break;
            case Mnemonic.stzx: RewriteStx(TestEq); break;
            case Mnemonic.sub: RewriteSub(); break;
            case Mnemonic.tst: RewriteTst(); break;
            case Mnemonic.und: RewriteUnd(); break;
            case Mnemonic.xor: RewriteXor(); break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            instrs.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    private Expression Assign(Expression dst, Expression src)
    {
        if (src is Identifier || src is Constant)
        {
            m.Assign(dst, src);
            return src;
        }
        if (dst is Identifier)
        {
            m.Assign(dst, src);
            return dst;
        }
        var tmp = binder.CreateTemporary(dst.DataType);
        m.Assign(tmp, src);
        m.Assign(dst, tmp);
        return tmp;
    }

    private (Expression dst, Expression src) CompatibleBinaryOperands()
    {
        var src = Operand(0);
        var dst = Operand(1);
        if (src.DataType.BitSize < dst.DataType.BitSize)
        {
            if (src is Constant cSrc)
            {
                src = m.Const(dst.DataType, cSrc.ToUInt64());
            }
            else
            {
                src = m.Convert(src, src.DataType, dst.DataType);
            }
        }
        return (dst, src);
    }

    private DataType DataTypeFromInstruction(M16CInstruction instr)
    {
        return instr.SizeSuffix switch
        {
            SizeSuffix.B => PrimitiveType.Byte,
            SizeSuffix.W => PrimitiveType.Word16,
            _ => throw new NotImplementedException()
        };
    }

    private Expression EffectiveAddress(MemoryOperand mem)
    {
        Expression ea;
        if (mem.Base is null)
        {
            ea = Address.Ptr16((ushort) mem.Offset);
        }
        else
        {
            if (mem.Base is RegisterStorage reg)
            {
                ea = binder.EnsureRegister(reg);
            }
            else if (mem.Base is SequenceStorage seq)
            {
                ea = binder.EnsureSequence(seq);
            }
            else
                throw new NotImplementedException();
            ea = m.AddSubSignedInt(ea, mem.Offset);
        }
        return ea;
    }

    private void EmitCond(FlagGroupStorage grf, Expression value)
    {
        m.Assign(binder.EnsureFlagGroup(grf), m.Cond(grf.DataType, value));
    }


    private Expression Operand(int iop, DataType? dt = null)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case RegisterStorage reg:
            return binder.EnsureRegister(reg);
        case Constant imm:
            return imm;
        case Address addr:
            return addr;
        case MemoryOperand mem:
            dt ??= DataTypeFromInstruction(instr);
            var ea = EffectiveAddress(mem);
            return m.Mem(dt, ea);
        case SequenceStorage seq:
            return binder.EnsureSequence(seq);
        default:
            EmitUnitTest();
            return Constant.Word64(4711471147111);
        }
    }

    private (Expression, int) BitOperand(int iop)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case Constant cbit:
            var exp = Operand(iop+1, PrimitiveType.Byte);
            return (exp, cbit.ToInt32());
        case MemoryOperand mem:
            var ea = EffectiveAddress(mem);
            if (ea is Address addr)
            {
                var bit = addr.Offset & 7;
                ea = addr.NewOffset(addr.Offset >> 3);
                return (m.Mem8(ea), (int) bit);
            }
            throw new NotImplementedException();
        }
            throw new NotImplementedException();
    }

    private void EmitUnitTest()
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingRewriter("M16cRw", instr, instr.Mnemonic.ToString(), rdr, "");
    }

    private void RewriteAdc()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var value = Assign(dst, m.IAddC(dst, src, c));
        EmitCond(Registers.OSZC, value);
    }

    private void RewriteAdcf()
    {
        var c = binder.EnsureFlagGroup(Registers.C);
        var src = Operand(0);
        var dst = Operand(0);
        var value = Assign(dst, m.IAdd(src, c));
        EmitCond(Registers.OSZC, value);
    }

    private void RewriteAdd()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = Assign(dst, m.IAdd(dst, src));
        EmitCond(Registers.OSZC, value);
    }

    private void RewriteAnd()
    {
        var (dst, src) = CompatibleBinaryOperands();
        var value = Assign(dst, m.And(dst, src));
        EmitCond(Registers.SZ, value);
    }

    private void RewriteBand()
    {
        var (dst, bit) = BitOperand(0);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(c, m.And(c, ReadBit(dst, bit)));
    }

    private void RewriteBclr()
    {
        var (dst, bit) = BitOperand(0);
        SetBit(dst, bit, Constant.False());

    }

    private void RewriteBnot()
    {
        m.SideEffect(m.Fn(bnot_intrinsic));
    }

    private void RewriteBmCnd(Func<IStorageBinder, ExpressionEmitter, Expression> test)
    {
        var (dst, bit) = BitOperand(0);
        WriteBit(dst, bit, test(binder, m));
    }

    private Expression ReadBit(Expression dst, int bit)
    {
        return m.Fn(CommonOps.Bit, dst, Constant.Byte((byte) bit));
    }

    private void SetBit(Expression dst, int bit, Expression expression)
    {
        m.Assign(dst, m.Fn(CommonOps.SetBit, dst, Constant.Byte((byte) bit)));
    }

    private void WriteBit(Expression dst, int bit, Expression expression)
    {
        m.Assign(dst, m.Fn(
            CommonOps.WriteBit.MakeInstance(dst.DataType, PrimitiveType.Byte),
            dst,
            Constant.Byte((byte) bit),
            expression));
    }

    private void RewriteBrk()
    {
        m.SideEffect(m.Fn(brk_intrinsic));
    }

    private void RewriteBset()
    {
        var (dst, bit) = BitOperand(0);
        SetBit(dst, bit, Constant.True());
    }

    private void RewriteBtst()
    {
        var (dst, bit) = BitOperand(0);
        var f = binder.CreateTemporary(PrimitiveType.Bool);
        m.Assign(f, ReadBit(dst, bit));
        m.Assign(binder.EnsureFlagGroup(Registers.C), f);
        m.Assign(binder.EnsureFlagGroup(Registers.Z), m.Not(f));
    }

    private void RewriteCmp()
    {
        var right = Operand(0);
        var left = Operand(1);
        EmitCond(Registers.OSZC, m.ISub(left, right));
    }

    private void RewriteDiv(BinaryOperator div, BinaryOperator mod)
    {
        Expression dividend;
        Expression quot;
        Expression rem;
        if (instr.SizeSuffix == SizeSuffix.W)
        {
            dividend = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(dividend, binder.EnsureSequence(Registers.r2r0));
            quot = binder.EnsureRegister(Registers.r0);
            rem = binder.EnsureRegister(Registers.r2);
        }
        else
        {
            dividend = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(dividend, binder.EnsureRegister(Registers.r0));
            quot = binder.EnsureRegister(Registers.r0l);
            rem = binder.EnsureRegister(Registers.r0h);
        }
        var divisor = Operand(0, quot.DataType);
        m.Assign(quot, m.Bin(div, dividend, divisor));
        m.Assign(rem, m.Bin(mod, dividend, divisor));
        EmitCond(Registers.O, quot);
    }

    private void RewriteEnter()
    {
        var sp = binder.EnsureRegister(Registers.usp);
        var fb = binder.EnsureRegister(Registers.fb);
        m.Assign(sp, m.ISub(sp, 2));
        m.Assign(m.Mem16(sp), fb);
        m.Assign(fb, sp);
        var imm = (Constant) instr.Operands[0];
        m.Assign(sp, m.ISubS(sp, imm.ToUInt32()));
    }

    private void RewriteExitd()
    {
        var sp = binder.EnsureRegister(Registers.usp);
        var fb = binder.EnsureRegister(Registers.fb);
        m.Assign(sp, fb);
        m.Assign(fb, m.Mem16(sp));
        m.Assign(sp, m.IAddS(sp, 2));
        m.Return(3, 0);
        iclass = InstrClass.Transfer | InstrClass.Return;
    }

    private void RewriteFclr()
    {
        var grf = (FlagGroupStorage) instr.Operands[0];
        var id = binder.EnsureFlagGroup(grf);
        m.Assign(id, 0);
    }

    private void RewriteFset()
    {
        var grf = (FlagGroupStorage) instr.Operands[0];
        var id = binder.EnsureFlagGroup(grf);
        m.Assign(id, grf.FlagGroupBits);
    }

    private void RewriteIncDec(int increment)
    {
        var src = Operand(0);
        var dst = Operand(0);
        var value = Assign(dst, m.AddSubSignedInt(src, increment));
        EmitCond(Registers.SZ, value);
    }

    private void RewriteJCnd(Func<IStorageBinder, ExpressionEmitter, Expression> test)
    {
        m.Branch(test(binder, m), Operand(0));
    }

    private void RewriteJmp()
    {
        m.Goto(Operand(0));
    }

    private void RewriteJsr()
    {
        m.Call(Operand(0), 3);  // [sic]: JSR pushes 24 bits.
    }

    private void RewriteLdc()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = Assign(dst, src);
        EmitCond(Registers.SZ, value);
    }

    private void RewriteLde()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = Assign(dst, src);
        EmitCond(Registers.SZ, value);
    }

    private void RewriteMov()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = Assign(dst, src);
        EmitCond(Registers.SZ, value);
    }

    private void RewriteMul(BinaryOperator mul)
    {
        DataType dtResult;
        Expression left;
        Expression right;
        Expression result;
        if (instr.SizeSuffix == SizeSuffix.B)
        {
            dtResult = PrimitiveType.Word16;
            left = Operand(1);
            //$TODO: slice if a0 a1?
            right = Operand(0);
            result = Operand(1);
        }
        else
        {
            dtResult = PrimitiveType.Word32;
            left = Operand(1);
            right = Operand(0);
            result = Operand(1);
        }
        Assign(result, m.Bin(mul, dtResult, left, right));
    }

    private void RewriteNop()
    {
        m.Nop();
    }

    private void RewriteOr()
    {
        var (dst, src) = CompatibleBinaryOperands();
        var value = Assign(dst, m.Or(dst, src));
        EmitCond(Registers.SZ, value);
    }

    private void RewriteNot()
    {
        var value = Assign(Operand(0), m.Comp(Operand(0)));
        EmitCond(Registers.SZ, value);
    }

    private void RewritePop()
    {
        var dst = Operand(0);
        var sp = binder.EnsureRegister(Registers.usp);
        Assign(dst, m.Mem(dst.DataType, sp));
        m.Assign(sp, m.IAdd(sp, dst.DataType.Size));
    }

    private void RewritePopm()
    {
        var sp = binder.EnsureRegister(Registers.usp);
        var regs = (MultiRegisterOperand) instr.Operands[0];
        foreach (var reg in regs.GetRegisters())
        {
            m.Assign(binder.EnsureRegister(reg), m.Mem16(sp));
            m.Assign(sp, m.IAddS(sp, 2));
        }
    }

    private void RewritePush()
    {
        var dst = Operand(0);
        var sp = binder.EnsureRegister(Registers.usp);
        m.Assign(sp, m.ISub(sp, dst.DataType.Size));
        Assign(m.Mem(dst.DataType, sp), dst);
    }

    private void RewritePushm()
    {
        var sp = binder.EnsureRegister(Registers.usp);
        var regs = (MultiRegisterOperand) instr.Operands[0];
        foreach (var reg in regs.GetRegisters())
        {
            m.Assign(sp, m.ISubS(sp, 2));
            m.Assign(m.Mem16(sp), binder.EnsureRegister(reg));
        }
    }

    private void RewriteReit()
    {
        m.SideEffect(m.Fn(reit_intrinsic));
        m.Return(4, 0);
    }

    private void RewriteRotate()
    {
        var sh = Operand(0);
        var src = Operand(1);
        var dst = Operand(1);
        var value = Assign(dst, m.Fn(CommonOps.Rol, src, sh));
        EmitCond(Registers.SZC, value);
    }

    private void RewriteRotateC(IntrinsicProcedure rotate)
    {
        Identifier? t;
        var cy = binder.EnsureFlagGroup(Registers.C);
        t = binder.CreateTemporary(PrimitiveType.Bool);
        m.Assign(t, cy);
        m.Assign(cy, m.Ne0(m.And(Operand(0), 1)));
        Expression p;
        var src0 = Operand(0);
        var src1 = Constant.Int16(1);
        p = m.Fn(
            rotate.MakeInstance(src0.DataType, src1.DataType),
            src0, src1, t);
        m.Assign(Operand(0), p);
    }

    private void RewriteRts()
    {
        m.Return(3, 0);
    }

    private void RewriteSmovf()
    {
        var memcpy = CommonOps.Memcpy.ResolvePointers(
            arch.PointerType.BitSize);
        var src = binder.EnsureRegister(Registers.a0);
        var dst = binder.EnsureRegister(Registers.a1);
        Expression size = binder.EnsureRegister(Registers.r3);
        if (instr.SizeSuffix == SizeSuffix.W)
        {
            size = m.Shl(size, 1);
        }
        m.SideEffect(m.Fn(memcpy, dst, src, size));
    }

    private void RewriteSstr()
    {
        var memcpy = CommonOps.Memcpy.ResolvePointers(
            arch.PointerType.BitSize);
        var src = binder.EnsureRegister(Registers.r0l);
        var dst = binder.EnsureRegister(Registers.a1);
        Expression size = binder.EnsureRegister(Registers.r3);
        if (instr.SizeSuffix == SizeSuffix.W)
        {
            src = binder.EnsureRegister(Registers.r0);
            size = m.Shl(size, 1);
        }
        m.SideEffect(m.Fn(sstr_intrinsic, dst, src, size));
    }

    private void RewriteSte()
    {
        var src = Operand(0);
        var dst = Operand(1);
        Assign(dst, src);
    }


    private void RewriteStx(Func<IStorageBinder, ExpressionEmitter, Expression> test)
    {
        var tr = Operand(0);
        var fa = Operand(1);
        var dst = Operand(2, PrimitiveType.Byte);
        Assign(dst, m.Conditional(
            dst.DataType,
            test(binder, m),
            tr,
            fa));
    }

    private void RewriteSt(Func<IStorageBinder, ExpressionEmitter, Expression> test)
    {
        m.BranchInMiddleOfInstruction(
            test(binder, m),
            instr.Address + instr.Length,
            InstrClass.ConditionalTransfer);
        var src = Operand(0);
        var dst = Operand(1, src.DataType);
        Assign(dst, src);
    }

    private void RewriteSbb()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var value = Assign(dst, m.ISubC(dst, src, c));
        EmitCond(Registers.OSZC, value);
    }

    private void RewriteSha()
    {
        var sh = Operand(0);
        var dst = Operand(1);
        Expression exp;
        if (sh is Constant csh)
        {
            if (csh.ToInt32() < 0)
            {
                exp = m.Sar(dst, -csh.ToInt32());
            }
            else
            {
                exp = m.Shl(dst, csh);
            }
        }
        else
        {
            exp = m.Fn(sha_intrinsic, dst, sh);
        }
        var value = Assign(Operand(1), exp);
        EmitCond(Registers.SZC, value);
    }

    private void RewriteShl()
    {
        var sh = Operand(0);
        var dst = Operand(1);
        Expression exp;
        if (sh is Constant csh)
        {
            if (csh.ToInt32() < 0)
            {
                exp = m.Shr(dst, -csh.ToInt32());
            }
            else
            {
                exp = m.Shl(dst, csh);
            }
        }
        else
        {
            exp = m.Fn(shl_intrinsic, dst, sh);
        }
        var value = Assign(Operand(1), exp);
        EmitCond(Registers.SZC, value);
    }

    private void RewriteSub()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = Assign(dst, m.ISub(dst, src));
        EmitCond(Registers.OSZC, value);
    }

    private void RewriteTst()
    {
        var src = Operand(0);
        var dst = Operand(1);
        var value = m.And(dst, src);
        EmitCond(Registers.SZ, value);
    }

    private void RewriteUnd()
    {
        m.SideEffect(m.Fn(und_intrinsic));
    }

    private void RewriteXor()
    {
        var (dst, src) = CompatibleBinaryOperands();
        var value = Assign(dst, m.Xor(dst, src));
        EmitCond(Registers.SZ, value);
    }

    private static Expression TestEq(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.Z));
    }

    private static Expression TestGe(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.GE, binder.EnsureFlagGroup(Registers.OS));
    }

    private static Expression TestGeu(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.UGE, binder.EnsureFlagGroup(Registers.C));
    }

    private static Expression TestGt(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.GT, binder.EnsureFlagGroup(Registers.OSZ));
    }

    private static Expression TestGtu(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.UGT, binder.EnsureFlagGroup(Registers.ZC));
    }

    private static Expression TestLe(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.LE, binder.EnsureFlagGroup(Registers.OSZ));
    }


    private static Expression TestLeu(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.ULE, binder.EnsureFlagGroup(Registers.ZC));
    }

    private static Expression TestLt(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.LT, binder.EnsureFlagGroup(Registers.OS));
    }

    private static Expression TestLtu(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.ULT, binder.EnsureFlagGroup(Registers.C));
    }

    private static Expression TestN(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.LT, binder.EnsureFlagGroup(Registers.S));
    }

    private static Expression TestNe(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.NE, binder.EnsureFlagGroup(Registers.Z));
    }

    private static Expression TestPz(IStorageBinder binder, ExpressionEmitter m)
    {
        return m.Test(ConditionCode.GE, binder.EnsureFlagGroup(Registers.S));
    }

    private static readonly IntrinsicProcedure bnot_intrinsic = IntrinsicBuilder.SideEffect("__bnot_TODO")
        .Void();
    private static readonly IntrinsicProcedure brk_intrinsic = IntrinsicBuilder.SideEffect("__break")
        .Void();
    private static readonly IntrinsicProcedure btst_intrinsic = new IntrinsicBuilder("__btst_TODO", false)
        .Returns(PrimitiveType.Bool);

    private static readonly IntrinsicProcedure reit_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
        .Void();

    private static readonly IntrinsicProcedure sha_intrinsic = new IntrinsicBuilder("__arithmetic_shift", false)
        .GenericTypes("T", "TSh")
        .Param("T")
        .Param("TSh")
        .Returns("T");

    private static readonly IntrinsicProcedure shl_intrinsic = new IntrinsicBuilder("__logical_shift", false)
        .GenericTypes("T", "TSh")
        .Param("T")
        .Param("TSh")
        .Returns("T");

    private static readonly IntrinsicProcedure sstr_intrinsic = new IntrinsicBuilder("__store_string", true)
        .Param(PrimitiveType.Ptr16)
        .Param(PrimitiveType.Word16)
        .Param(PrimitiveType.UInt16)
        .Void();

    private static readonly IntrinsicProcedure und_intrinsic = IntrinsicBuilder.SideEffect("__undefined")
        .Void();
}
