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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            case Mnemonic.fadd: RewriteFadd(instr); break;
            case Mnemonic.fcmp: RewriteFcmp(instr); break;
            case Mnemonic.fdiv: RewriteFdiv(instr); break;
            case Mnemonic.fmul: RewriteFmul(instr); break;
            case Mnemonic.fsqrt: RewriteFsqrt(instr); break;
            case Mnemonic.fsub: RewriteFsub(instr); break;
            case Mnemonic.ftoi: RewriteFto(instr, PrimitiveType.Int32); break;
            case Mnemonic.ftou: RewriteFto(instr, PrimitiveType.UInt32); break;
            case Mnemonic.@int: RewriteInt(instr); break;
            case Mnemonic.itof: RewriteItof(instr, PrimitiveType.Int32); break;
            case Mnemonic.jmp: RewriteJmp(instr); break;
            case Mnemonic.utof: RewriteItof(instr, PrimitiveType.UInt32); break;
            default:
                throw new NotImplementedException($"Unhandled instruction {instr.MnemonicAsString}");
            }
            yield return m.MakeCluster(instr.Address, instr.Length, instr.InstructionClass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitCc(Identifier dst, FlagGroupStorage flags)
    {
        var grf = binder.EnsureFlagGroup(flags);
        m.Assign(grf, m.Cond(grf.DataType, dst));
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

    private void RewriteItof(RxInstruction instr, PrimitiveType dtIntegral)
    {
        var dst = OpDst(instr, 1, m.Convert(OpSrc(instr, 0), dtIntegral, PrimitiveType.Real32));
        EmitCc(dst, Registers.SZ);
    }

    private void RewriteJmp(RxInstruction instr)
    {

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
            m.Assign(b, tmp);
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

    private Identifier RewriteUnary(RxInstruction instr, IntrinsicProcedure intrinsic)
    {
        Expression src = OpSrc(instr, 0);
        src = m.Fn(intrinsic, src);
        var id = OpDst(instr, instr.Operands.Length - 1, src);
        return id;
    }

    private static readonly IntrinsicProcedure brk_intrinsic = IntrinsicBuilder.SideEffect("__brk")
        .Void();
}
