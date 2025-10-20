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
using System.Diagnostics;

namespace Reko.Arch.Maxim;

public class MaxqRewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly MaxqArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<MaxqInstruction> dasm;
    private MaxqInstruction instr;
    private InstrClass iclass;

    public MaxqRewriter(MaxqArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.dasm = new MaxqDisassembler(arch, rdr).GetEnumerator();
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
                this.iclass = InstrClass.Linear;
                m.Nop();
                break;
            case Mnemonic.add: RewriteAdd(); break;
            case Mnemonic.addc: RewriteAddc(); break;
            case Mnemonic.and: RewriteAnd(); break;
            case Mnemonic.cmp: RewriteCmp(); break;
            case Mnemonic.cpl: RewriteCpl(); break;
            case Mnemonic.lcall: RewriteLcall(); break;
            case Mnemonic.ldjnz: RewriteLdjnz(); break;
            case Mnemonic.ljump: RewriteLjmp(); break;
            case Mnemonic.move: RewriteMove(); break;
            case Mnemonic.neg: RewriteNeg(); break;
            case Mnemonic.nop: m.Nop(); break;
            case Mnemonic.or: RewriteOr(); break;
            case Mnemonic.pop: RewritePop(); break;
            case Mnemonic.push: RewritePush(); break;
            case Mnemonic.ret: RewriteRet(); break;
            case Mnemonic.reti: RewriteReti(); break;
            case Mnemonic.rr: RewriteRr(); break;
            case Mnemonic.rrc: RewriteRrc(); break;
            case Mnemonic.sla: RewriteShift(Operator.Shl, 1); break;
            case Mnemonic.sla2: RewriteShift(Operator.Shl, 2); break;
            case Mnemonic.sla4: RewriteShift(Operator.Shl, 4); break;
            case Mnemonic.sr: RewriteShift(Operator.Shr, 1); break;
            case Mnemonic.sra: RewriteShift(Operator.Sar, 1); break;
            case Mnemonic.sra2: RewriteShift(Operator.Sar, 2); break;
            case Mnemonic.sra4: RewriteShift(Operator.Sar, 4); break;
            case Mnemonic.scall: RewriteScall(); break;
            case Mnemonic.sub: RewriteSub(); break;
            case Mnemonic.subb: RewriteSubb(); break;
            case Mnemonic.xch: RewriteXch(); break;
            case Mnemonic.xor: RewriteXor(); break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    private void EmitUnitTest()
    {
        arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("MaxqRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Emit_CSZ(Expression src)
    {
        var grf = binder.EnsureFlagGroup(Registers.CSZ);
        m.Assign(grf, m.Cond(grf.DataType, src));
    }


    private void Emit_CSZE(Expression src)
    {
        var grf = binder.EnsureFlagGroup(Registers.CSZE);
        m.Assign(grf, m.Cond(grf.DataType, src));
    }


    private void Emit_CSZV(Expression src)
    {
        var grf = binder.EnsureFlagGroup(Registers.CSZV);
        m.Assign(grf, m.Cond(grf.DataType, src));
    }

    private void Emit_S(Expression src)
    {
        var grf = binder.EnsureFlagGroup(Registers.S);
        m.Assign(grf, m.Cond(grf.DataType, src));
    }

    private void Emit_SZ(Expression src)
    {
        var grf = binder.EnsureFlagGroup(Registers.SZ);
        m.Assign(grf, m.Cond(grf.DataType, src));
    }

    private Expression MakeTest(MachineOperand op)
    {
        var ccop = ((ConditionOperand<CCode>) op).Condition;
        switch (ccop)
        {
        case CCode.C: return new TestCondition(ConditionCode.ULT, binder.EnsureFlagGroup(Registers.C));
        case CCode.E: return new TestCondition(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.C));
        case CCode.NC: return new TestCondition(ConditionCode.UGE, binder.EnsureFlagGroup(Registers.C));
        case CCode.NZ: return new TestCondition(ConditionCode.NE, binder.EnsureFlagGroup(Registers.Z));
        case CCode.S: return new TestCondition(ConditionCode.LT, binder.EnsureFlagGroup(Registers.Z));
        case CCode.Z: return new TestCondition(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.Z));
        }
        EmitUnitTest();
        return Constant.False();
    }

    private Expression MakeInvertedTest(MachineOperand op)
    {
        var ccop = ((ConditionOperand<CCode>) op).Condition;
        switch (ccop)
        {
        case CCode.C: return new TestCondition(ConditionCode.UGE, binder.EnsureFlagGroup(Registers.C));
        case CCode.E: return new TestCondition(ConditionCode.NE, binder.EnsureFlagGroup(Registers.C));
        case CCode.NC: return new TestCondition(ConditionCode.ULT, binder.EnsureFlagGroup(Registers.C));
        case CCode.NZ: return new TestCondition(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.Z));
        case CCode.S: return new TestCondition(ConditionCode.GE, binder.EnsureFlagGroup(Registers.Z));
        case CCode.Z: return new TestCondition(ConditionCode.NE, binder.EnsureFlagGroup(Registers.Z));
        }
        EmitUnitTest();
        return Constant.False();
    }



    private Expression OpSrc(int iop)
    {
        return OpSrc(instr.Operands[iop]);
    }

    private Expression OpSrc(MachineOperand op)
    { 
        switch (op)
        {
        case RegisterStorage reg:
            return binder.EnsureRegister(reg);
        case Constant c:
            return c;
        case ModuleRegister modreg:
            var idx = Constant.Byte((byte) modreg.Index);
            return m.Fn(intrinsic_readmodreg, idx);
        case MemoryOperand mem:
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Increment == IncrementMode.PreIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, 1));
                }
                if (mem.Offset is RegisterStorage reg)
                {
                    ea = m.IAdd(ea, binder.EnsureRegister(reg));
                }
                else if (mem.Offset is Constant c)
                {
                    ea = m.IAdd(ea, c);
                }
            }
            else
            {
                Debug.Assert(mem.Offset is not null);
                ea = Address.Ptr16(((Constant) mem.Offset).ToUInt16());
            }
            return m.Mem16(ea);
        case FlagGroupStorage grf:
            return binder.EnsureFlagGroup(grf);
        case BitOperand bit:
            var breg = OpSrc(bit.Operand);
            return m.Fn(CommonOps.Bit, breg, Constant.Byte((byte)bit.BitPosition));
        default:
            host.Error(instr.Address, $"Operand type {op.GetType().Name} not implemented yet.");
            EmitUnitTest();
            return m.Word16(0);
        }
    }

    private Expression OpDst(int iop, Expression src)
    {
        var op = instr.Operands[iop];
        Expression result;
        switch (op)
        {
        case RegisterStorage reg:
            result = binder.EnsureRegister(reg);
            m.Assign(result, src);
            return result;
        case FlagGroupStorage grf:
            result = binder.EnsureFlagGroup(grf);
            m.Assign(result, src);
            return result;
        case ModuleRegister modreg:
            var idx = Constant.Byte((byte) modreg.Index);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            m.SideEffect(m.Fn(intrinsic_writemodreg, idx, tmp));
            return tmp;
        case MemoryOperand mem:
            tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, src);
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Increment == IncrementMode.PreIncrement)
                {
                    m.Assign(ea, m.IAddS(ea, 1));
                }
                if (mem.Offset is RegisterStorage reg)
                {
                    ea = m.IAdd(ea, binder.EnsureRegister(reg));
                }
                else if (mem.Offset is Constant c)
                {
                    ea = m.IAdd(ea, c);
                }
            }
            else
            {
                Debug.Assert(mem.Offset is not null);
                ea = Address.Ptr16(((Constant) mem.Offset).ToUInt16());
            }
            m.Assign(m.Mem16(ea), tmp);
            return tmp;
        default:
            EmitUnitTest();
            host.Error(instr.Address, $"Operand type {op.GetType().Name} not implemented yet.");
            return binder.EnsureRegister(Registers.SP);
        }
    }

    private void RewriteAdd()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var dst = OpDst(0, m.IAdd(left, right));
        Emit_CSZV(dst);
    }

    private void RewriteAddc()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = OpDst(0, m.IAddC(left, right, c));
        Emit_CSZV(dst);
    }

    private void RewriteAnd()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var dst = OpDst(0, m.And(left, right));
        Emit_SZ(dst);
    }

    private void RewriteCmp()
    {
        var left = binder.EnsureRegister(Registers.Acc);
        var right = OpSrc(0);
        var e = binder.EnsureFlagGroup(Registers.E);
        m.Assign(e, m.Eq(left, right));
    }

    private void RewriteCpl()
    {
        if (instr.Operands[0] == Registers.Acc)
        {
            var exp = binder.EnsureRegister(Registers.Acc);
            m.Assign(exp, m.Comp(exp));
            Emit_SZ(exp);
        }
        else
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Not(c));
        }
    }

    private void RewriteLcall()
    {
        var dst = OpSrc(0);
        var target = ComputeTransferTarget(dst);
        m.Call(target, 2);
    }

    private void RewriteLdjnz()
    {
        var reg = OpSrc(0);
        m.Assign(reg, m.ISubS(reg, 1));
        var dst = OpSrc(1);
        var target = ComputeTransferTarget(dst);
        m.Branch(m.Ne0(reg), target);
    }

    private void RewriteLjmp()
    {
        Expression dst;
        
        Expression? test;
        if (instr.Operands.Length == 2)
        {
            test = MakeTest(instr.Operands[0]);
            dst = OpSrc(1);
        }
        else
        {
            test = null;
            dst = OpSrc(0);
        }
        Expression target = ComputeTransferTarget(dst);
        if (test is null)
            m.Goto(target);
        else
            m.Branch(test, target);
    }

    private Expression ComputeTransferTarget(Expression dst)
    {
        var pfx0 = Registers.Prefixes[0];
        var prefix = state.GetRegister(pfx0);
        Expression target;
        if (prefix is not null && prefix.IsValid)
        {
            target = Address.Ptr16((ushort) ((prefix.ToUInt16() << 8) | ((Constant) dst).ToUInt16()));
        }
        else
        {
            var targetExp = m.Seq(binder.EnsureRegister(pfx0), dst);
            target = binder.CreateTemporary(targetExp.DataType);
            m.Assign(target, targetExp);
        }
        return target;
    }

    private void RewriteMove()
    {
        var src = OpSrc(1);
        if (instr.Operands[0] is LiteralOperand)
        {
            m.Nop();
            return;
        }
        var dst = OpDst(0, src);
        if (dst is FlagGroupStorage grf)
            throw new NotImplementedException();
        Emit_CSZE(src);
    }

    private void RewriteNeg()
    {
        var e = binder.EnsureRegister(Registers.Acc);
        m.Assign(e, m.Neg(e));
        Emit_SZ(e);
    }

    private void RewriteOr()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);

        var dst = OpDst(0, m.Or(left, right));
        Emit_SZ(dst);
    }

    private void RewritePop()
    {
        var dst = OpSrc(0);
        var sp = binder.EnsureRegister(Registers.SP);
        m.Assign(dst, m.Mem(dst.DataType, sp));
        m.Assign(sp, m.ISubS(sp, 2));
        Emit_CSZE(dst);
    }

    private void RewritePush()
    {
        var dst = OpSrc(0);
        var sp = binder.EnsureRegister(Registers.SP);
        m.Assign(sp, m.IAddS(sp, 2));
        m.Assign(dst, m.Mem(dst.DataType, sp));
        Emit_CSZE(dst);
    }

    private void RewriteRet()
    {
        if (instr.Operands.Length != 0)
        {
            var test = this.MakeInvertedTest(instr.Operands[0]);
            m.BranchInMiddleOfInstruction(test, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
        }
        m.Return(2, 0);
    }

    private void RewriteReti()
    {
        if (instr.Operands.Length != 0)
        {
            var test = this.MakeInvertedTest(instr.Operands[0]);
            m.BranchInMiddleOfInstruction(test, instr.Address + instr.Length, InstrClass.ConditionalTransfer);
        }
        m.SideEffect(m.Fn(intrinsic_rti));
        m.Return(2, 0);
    }

    private void RewriteRr()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Fn(CommonOps.Ror, acc, Constant.Byte(1)));
        Emit_SZ(acc);
    }

    private void RewriteRrc()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Fn(CommonOps.RorC, acc, Constant.Byte(1)));
        Emit_CSZ(acc);
    }

    private void RewriteScall()
    {
        var dst = OpSrc(0);
        m.Call(dst, 2);
    }

    private void RewriteShift(BinaryOperator op, int shift)
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Bin(op, acc, Constant.Byte((byte)shift)));
        Emit_CSZ(acc);
    }

    private void RewriteSub()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var dst = OpDst(0, m.ISub(left, right));
        Emit_CSZV(dst);
    }

    private void RewriteSubb()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = OpDst(0, m.ISubC(left, right, c));
        Emit_CSZV(dst);
    }

    private void RewriteXch()
    {
        var acc = binder.EnsureRegister(Registers.Acc);
        m.Assign(acc, m.Fn(intrinsic_xch, acc));
        Emit_S(acc);
    }

    private void RewriteXor()
    {
        var left = OpSrc(0);
        var right = OpSrc(1);
        var dst = OpDst(0, m.Xor(left, right));
        Emit_SZ(dst);
    }

    private static readonly IntrinsicProcedure intrinsic_readmodreg = new IntrinsicBuilder("__read_modreg", true)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word16);
    private static readonly IntrinsicProcedure intrinsic_writemodreg = new IntrinsicBuilder("__write_modreg", true)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Word16)
        .Void();
    private static readonly IntrinsicProcedure intrinsic_rti = IntrinsicBuilder.SideEffect("__return_from_interrupt")
        .Void();
    private static readonly IntrinsicProcedure intrinsic_xch = IntrinsicBuilder.Unary("__exchange_bytes", PrimitiveType.Word16);
}
