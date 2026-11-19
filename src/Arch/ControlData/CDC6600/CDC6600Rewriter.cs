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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.Arch.ControlData.CDC6600;

public class CDC6600Rewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly CDC6600Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<CDC6600Bundle> dasm;

    public CDC6600Rewriter(CDC6600Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.dasm = new CDC6600Disassembler(arch, rdr).GetEnumerator(); ;
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var bundle = dasm.Current;
            LiftBundle(bundle);
            yield return m.MakeCluster(bundle.Address, 1, InstrClass.Linear);
            rtls.Clear();
        }
    }

    private void LiftBundle(CDC6600Bundle bundle)
    {
        int cInstrs = bundle.Instructions.Length;
        for (int i = 0; i < cInstrs; ++i)
        {
            var instr = bundle.Instructions[i];
            switch (instr.Mnemonic)
            {
            default:
                EmitUnitTest(instr);
                goto case Mnemonic.Invalid;
            case Mnemonic.Invalid:
                m.Invalid();
                return;
            case Mnemonic.EQ: RewriteCond2(instr, Operator.Eq, i == cInstrs - 1); break;
            case Mnemonic.GE: RewriteCond2(instr, Operator.Ge, i == cInstrs - 1); break;
            case Mnemonic.IR: RewriteCond1(instr, ir_intrinsic, i == cInstrs - 1); break;
            case Mnemonic.JP: RewriteJp(instr); return;   // No more in this bundle
            case Mnemonic.LT: RewriteCond2(instr, Operator.Lt, i == cInstrs - 1); break;
            case Mnemonic.NE: RewriteCond2(instr, Operator.Ne, i == cInstrs - 1); break;
            case Mnemonic.NG: RewriteCond1(instr, Operator.Lt, i == cInstrs - 1); break;
            case Mnemonic.NO: m.Nop(); break;
            case Mnemonic.NZ: RewriteCond1(instr, Operator.Ne, i == cInstrs - 1); break;
            case Mnemonic.OR: RewriteCond1(instr, or_intrinsic, i == cInstrs - 1); break;
            case Mnemonic.PL: RewriteCond1(instr, Operator.Ge, i == cInstrs - 1); break;
            case Mnemonic.PS: RewritePs(); return;
            case Mnemonic.REC: RewriteRec(instr); break;
            case Mnemonic.RJ: RewriteRj(instr); return; // No more in this bundle
            case Mnemonic.WEC: RewriteWec(instr); break;
            case Mnemonic.ZR: RewriteCond1(instr, Operator.Eq, i == cInstrs - 1); break;
            case Mnemonic.and: RewriteBinOp(instr, BinaryOperator.And); break;
            case Mnemonic.andn: RewriteBinOpNeg(instr, BinaryOperator.And); break;
            case Mnemonic.asr: RewriteShift(instr, Operator.Sar); break;
            case Mnemonic.asrn: RewriteShift(instr, asrn_intrinsic); break;
            case Mnemonic.dadd: RewriteBinOp(instr, dadd_intrinsic); break;
            case Mnemonic.dmul: RewriteBinOp(instr, dmul_intrinsic); break;
            case Mnemonic.dsub: RewriteBinOp(instr, dsub_intrinsic); break;
            case Mnemonic.fadd: RewriteBinOp(instr, BinaryOperator.FAdd); break;
            case Mnemonic.fadd_round: RewriteBinOp(instr, fadd_round_intrinsic); break;
            case Mnemonic.fdiv: RewriteBinOp(instr, BinaryOperator.FDiv); break;
            case Mnemonic.fdiv_round: RewriteBinOp(instr, fdiv_round_intrinsic); break;
            case Mnemonic.fmul: RewriteBinOp(instr, BinaryOperator.FMul); break;
            case Mnemonic.fmul_round: RewriteBinOp(instr, fmul_round_intrinsic); break;
            case Mnemonic.fsub: RewriteBinOp(instr, BinaryOperator.FSub); break;
            case Mnemonic.fsub_round: RewriteBinOp(instr, fsub_round_intrinsic); break;
            case Mnemonic.iadd: RewriteBinOp(instr, BinaryOperator.IAdd); break;
            case Mnemonic.isub: RewriteBinOp(instr, BinaryOperator.ISub); break;
            case Mnemonic.lsl: RewriteShift(instr, Operator.Shl); break;
            case Mnemonic.lsln: RewriteShift(instr, lsln_intrinsic); break;
            case Mnemonic.mask: RewriteMask(instr); break;
            case Mnemonic.mov: RewriteMov(instr); break;
            case Mnemonic.neg: RewriteNeg(instr); break;
            case Mnemonic.normalize: RewriteBinOp(instr, normalize_intrinsic); break;
            case Mnemonic.or: RewriteBinOp(instr, BinaryOperator.Or); break;
            case Mnemonic.orn: RewriteBinOpNeg(instr, BinaryOperator.Or); break;
            case Mnemonic.pack: RewriteBinOp(instr, pack_intrinsic); break;
            case Mnemonic.popcnt: RewritePopcnt(instr); break;
            case Mnemonic.round_normalize: RewriteBinOp(instr, round_normalize_intrinsic); break;
            case Mnemonic.sadd: RewriteSadd(instr, BinaryOperator.IAdd); break;
            case Mnemonic.ssub: RewriteSadd(instr, BinaryOperator.ISub); break;
            case Mnemonic.unpack: RewriteBinOp(instr, unpack_intrinsic); break;
            case Mnemonic.xor: RewriteBinOp(instr, BinaryOperator.Xor); break;
            case Mnemonic.xorn: RewriteBinOpNeg(instr, BinaryOperator.Xor); break;
            }
        }
    }

    private void EmitUnitTest(CDC6600Instruction instr)
    {
        var testgenSvc = arch.Services.GetService<ITestGenerationService>();
        testgenSvc?.ReportMissingRewriter("CDC6600Rw", instr, instr.Mnemonic.ToString(), rdr, "");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Expression Op(CDC6600Instruction instr, int iop, bool write = false)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case RegisterStorage reg:
            if (!write && reg == Registers.BRegisters[0])
                return m.Zero(reg.DataType);
            return binder.EnsureRegister(reg);
        case Constant c:
            return c;
        case Address addr:
            return addr;
        default:
            throw new NotImplementedException($"Not implemented: {op.GetType().Name}");
        }
    }

    private void RewriteAnd(CDC6600Instruction instr)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.And(src1, src2));
    }

    private void RewriteMask(CDC6600Instruction instr)
    {
        int maskSize = ((Constant) Op(instr, 1)).ToInt32();
        var uMask = Bits.Mask(60-maskSize, maskSize);
        var dst = Op(instr, 0, true);
        m.Assign(dst, uMask);
    }

    private void RewriteMov(CDC6600Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0, true);
        m.Assign(dst, src);
    }

    private void RewriteNeg(CDC6600Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Comp(src));
    }

    private void RewriteShift(CDC6600Instruction instr, BinaryOperator fn)
    {
        var sh = Op(instr, 1);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Bin(fn, dst, sh));
    }

    private void RewriteShift(CDC6600Instruction instr, IntrinsicProcedure fn)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Fn(fn, src1, src2));
    }

    private void RewriteCond(Expression predicate, Expression target, bool isLast)
    {
        if (isLast)
        {
            m.Branch(predicate, target);
        }
        else
        {
            var itarget = rtls.Count + 2;
            m.MicroBranch(predicate.Invert(), itarget);
            m.Goto(target);
        }
    }

    private void RewriteCond1(CDC6600Instruction instr, BinaryOperator op, bool isLast)
    {
        var src1 = Op(instr, 0);
        var src2 = m.Zero(src1.DataType);
        var target = Op(instr, 1);
        var predicate = m.Bin(op, PrimitiveType.Bool, src1, src2);
        RewriteCond(predicate, target, isLast);
    }

    private void RewriteCond1(CDC6600Instruction instr, IntrinsicProcedure op, bool isLast)
    {
        var src1 = Op(instr, 0);
        var src2 = m.Zero(src1.DataType);
        var target = Op(instr, 1);
        var predicate = m.Fn(op, src1, src2);
        RewriteCond(predicate, target, isLast);
    }

    private void RewriteCond2(CDC6600Instruction instr, BinaryOperator op, bool isLast)
    {
        var src1 = Op(instr, 0);
        var src2 = Op(instr, 1);
        var target = Op(instr, 2);
        var predicate = m.Bin(op, PrimitiveType.Bool, src1, src2);
        RewriteCond(predicate, target, isLast);
    }

    private void RewriteBinOp(CDC6600Instruction instr, BinaryOperator op)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteBinOp(CDC6600Instruction instr, IntrinsicProcedure op)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Fn(op, src1, src2));
    }

    private void RewriteBinOpNeg(CDC6600Instruction instr, BinaryOperator op)
    {
        // Note operand order is swapped for negated logical operations.
        var src1 = Op(instr, 2);
        var src2 = Op(instr, 1);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Bin(op, src1, m.Comp(src2)));
    }

    private void RewriteJp(CDC6600Instruction instr)
    {
        Expression target;
        if (instr.Operands[0] == Registers.BRegisters[0])
        {
            target = arch.MakeAddressFromConstant(
                (Constant) instr.Operands[1],
                true);
        }
        else
        {
            var src1 = Op(instr, 0);
            var src2 = Op(instr, 1);
            target = m.IAdd(src1, src2);
        }
        m.Goto(target);
    }

    private void RewritePopcnt(CDC6600Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0, true);
        m.Assign(dst, m.Fn(CommonOps.PopCount, src));
    }

    private void RewritePs()
    {
        m.SideEffect(m.Fn(ps_intrinsic));
    }

    private void RewriteRec(CDC6600Instruction instr)
    {
        var x0 = binder.EnsureRegister(Registers.XRegisters[0]);
        var a0 = binder.EnsureRegister(Registers.ARegisters[0]);
        var bj = Op(instr, 0);
        var tmp = binder.CreateTemporary(bj.DataType);
        m.Assign(tmp, m.IAdd(bj, Op(instr, 1)));
        m.SideEffect(m.Fn(rec_intrinsic, a0, x0, tmp));
    }

    private void RewriteRj(CDC6600Instruction instr)
    {
        var target = Op(instr, 0);
        m.Call(target, 0);
    }

    private void RewriteWec(CDC6600Instruction instr)
    {
        var x0 = binder.EnsureRegister(Registers.XRegisters[0]);
        var a0 = binder.EnsureRegister(Registers.ARegisters[0]);
        var bj = Op(instr, 0);
        var tmp = binder.CreateTemporary(bj.DataType);
        m.Assign(tmp, m.IAdd(bj, Op(instr, 1)));
        m.SideEffect(m.Fn(wec_intrinsic, x0, a0, tmp));
    }

    private void RewriteSadd(CDC6600Instruction instr, BinaryOperator op)
    {
        Expression Adjust(Expression e, DataType dt)
        {
            long dtBits = e.DataType.BitSize - dt.BitSize;
            if (dtBits > 0)
            {
                if (e is Constant c)
                    return m.Const(dt, c.ToInt32());
                return m.Slice(e, dt);
            }
            else if (dtBits < 0)
            {
                if (e is Constant c)
                    return m.Const(dt, c.ToInt32());
                else
                    return m.ExtendS(e, dt);
            }
            return e;
        }

        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0, true);
        
        src1 = Adjust(src1, dst.DataType);
        src2 = Adjust(src2, dst.DataType);
        var src = src1;
        if (!src2.IsZero)
            src = m.Bin(op, src1, src2);
        m.Assign(dst, src);

        // Check for core storage side-effects
        var rDst = (RegisterStorage) instr.Operands[0];
        var aRegs = Registers.ARegisters;
        if (aRegs[1].Number <= rDst.Number && rDst.Number <= aRegs[5].Number)
        {
            int idx = rDst.Number - aRegs[0].Number;
            var Xr = binder.EnsureRegister(Registers.XRegisters[idx]);
            m.Assign(Xr, m.Mem(Xr.DataType, dst));
        }
        else if (aRegs[6].Number == rDst.Number || aRegs[7].Number == rDst.Number)
        {
            int idx = rDst.Number - aRegs[0].Number;
            var Xr = binder.EnsureRegister(Registers.XRegisters[idx]);
            m.Assign(m.Mem(Xr.DataType, dst), Xr);
        }
    }

    private static readonly IntrinsicProcedure asrn_intrinsic = IntrinsicBuilder.Pure("__asr_nominal")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word18);

    private static readonly IntrinsicProcedure dadd_intrinsic = IntrinsicBuilder.Binary("__dadd", CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure dmul_intrinsic = IntrinsicBuilder.Binary("__dmul", CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure dsub_intrinsic = IntrinsicBuilder.Binary("__dsub", CDC6600Architecture.Word60);

    private static readonly IntrinsicProcedure fadd_round_intrinsic = IntrinsicBuilder.Binary("__fadd_round", CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure fdiv_round_intrinsic = IntrinsicBuilder.Binary("__fdiv_round", CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure fmul_round_intrinsic = IntrinsicBuilder.Binary("__fmul_round", CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure fsub_round_intrinsic = IntrinsicBuilder.Binary("__fsub_round", CDC6600Architecture.Word60);

    private static readonly IntrinsicProcedure ir_intrinsic = IntrinsicBuilder.Pure("__is_in_range")
        .Param(CDC6600Architecture.Word60)
        .Param(CDC6600Architecture.Word60)
        .Returns(PrimitiveType.Bool);


    private static readonly IntrinsicProcedure lsln_intrinsic = IntrinsicBuilder.Pure("__lsl_nominal")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word18);

    private static readonly IntrinsicProcedure normalize_intrinsic = IntrinsicBuilder.Pure("__normalize")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word60);

    private static readonly IntrinsicProcedure or_intrinsic = IntrinsicBuilder.Pure("__is_out_of_range")
        .Param(CDC6600Architecture.Word60)
        .Param(CDC6600Architecture.Word60)
        .Returns(PrimitiveType.Bool);

    private static readonly IntrinsicProcedure pack_intrinsic = IntrinsicBuilder.Pure("__pack")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word60);
    private static readonly IntrinsicProcedure ps_intrinsic = IntrinsicBuilder.SideEffect("__program_stop")
        .Void();

    private static readonly IntrinsicProcedure rec_intrinsic = IntrinsicBuilder.SideEffect("__read_extended_core")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Param(CDC6600Architecture.Word18)
        .Void();
    private static readonly IntrinsicProcedure round_normalize_intrinsic = IntrinsicBuilder.Pure("__round_normalize")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word60);

    private static readonly IntrinsicProcedure unpack_intrinsic = IntrinsicBuilder.Pure("__pack")
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word60)
        .Returns(CDC6600Architecture.Word60);

    private static readonly IntrinsicProcedure wec_intrinsic = IntrinsicBuilder.SideEffect("__write_extended_core")
        .Param(CDC6600Architecture.Word60)
        .Param(CDC6600Architecture.Word18)
        .Param(CDC6600Architecture.Word18)
        .Void();

}
