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

namespace Reko.Arch.Epson;

public class C33Rewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly C33Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly IEnumerator<C33Instruction> dasm;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;

    public C33Rewriter(C33Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.dasm = new C33Disassembler(arch, rdr).GetEnumerator();
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            C33Instruction instr = dasm.Current;
            InstrClass iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            default:
                arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("C33Rw", instr, instr.MnemonicAsString, rdr, "");
                break;
            case Mnemonic.Nyi: m.Nop(); break;
            case Mnemonic.add: RewriteAdd(instr); break;
            case Mnemonic.and: RewriteAnd(instr); break;
            case Mnemonic.brk: RewriteBrk(instr); break;
            case Mnemonic.call: RewriteCall(instr, InstrClass.Call | InstrClass.Transfer); break;
            case Mnemonic.call_d: RewriteCall(instr, InstrClass.Call | InstrClass.Transfer | InstrClass.Delay); break;
            case Mnemonic.cmp: RewriteCmp(instr); break;
            case Mnemonic.halt: RewriteHalt(instr); break;
            case Mnemonic.@int: RewriteInt(instr); break;
            case Mnemonic.jp: RewriteJp(instr, InstrClass.Transfer); break;
            case Mnemonic.jpr: RewriteJpr(instr); break;
            case Mnemonic.jpr_d: RewriteJpr(instr); break;
            case Mnemonic.jp_d: RewriteJp(instr, InstrClass.Transfer | InstrClass.Delay); break;
            case Mnemonic.jreq: RewriteJr(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.jreq_d: RewriteJr(instr, ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.jrge: RewriteJr(instr, ConditionCode.GE, Registers.NV); break;
            case Mnemonic.jrge_d: RewriteJr(instr, ConditionCode.GE, Registers.NV); break;
            case Mnemonic.jrgt: RewriteJr(instr, ConditionCode.GT, Registers.NV); break;
            case Mnemonic.jrgt_d: RewriteJr(instr, ConditionCode.GT, Registers.NVZ); break;
            case Mnemonic.jrle: RewriteJr(instr, ConditionCode.LE, Registers.NVZ); break;
            case Mnemonic.jrle_d: RewriteJr(instr, ConditionCode.LE, Registers.NVZ); break;
            case Mnemonic.jrlt: RewriteJr(instr, ConditionCode.LT, Registers.NV); break;
            case Mnemonic.jrlt_d: RewriteJr(instr, ConditionCode.LT, Registers.NV); break;
            case Mnemonic.jrne: RewriteJr(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.jrne_d: RewriteJr(instr, ConditionCode.NE, Registers.Z); break;
            case Mnemonic.jruge: RewriteJr(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.jruge_d: RewriteJr(instr, ConditionCode.UGE, Registers.C); break;
            case Mnemonic.jrugt: RewriteJr(instr, ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.jrugt_d: RewriteJr(instr, ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.jrule: RewriteJr(instr, ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.jrule_d: RewriteJr(instr, ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.jrult: RewriteJr(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.jrult_d: RewriteJr(instr, ConditionCode.ULT, Registers.C); break;
            case Mnemonic.ld_b: RewriteLoad(instr, PrimitiveType.Int8); break;
            case Mnemonic.ld_cf: RewriteLd_cf(instr); break;
            case Mnemonic.ld_h: RewriteLoad(instr, PrimitiveType.Int16); break;
            case Mnemonic.ld_ub: RewriteLoad(instr, PrimitiveType.Byte); break;
            case Mnemonic.ld_uh: RewriteLoad(instr, PrimitiveType.Word16); break;
            case Mnemonic.ld_w: RewriteLoad(instr, PrimitiveType.Word32); break;
            case Mnemonic.nop: m.Nop(); break;
            case Mnemonic.not: RewriteNot(instr); break;
            case Mnemonic.or: RewriteOr(instr); break;
            case Mnemonic.pop: RewritePop(instr); break;
            case Mnemonic.popn: RewritePopn(instr); break;
            case Mnemonic.pops: RewritePops(instr); break;
            case Mnemonic.push: RewritePush(instr); break;
            case Mnemonic.pushn: RewritePushn(instr); break;
            case Mnemonic.pushs: RewritePushs(instr); break;
            case Mnemonic.ret: RewriteRet(instr); break;
            case Mnemonic.ret_d: RewriteRet(instr); break;
            case Mnemonic.retd: RewriteRet(instr, retd_intrinsic); break;
            case Mnemonic.reti: RewriteRet(instr, reti_intrinsic); break;
            case Mnemonic.rl: RewriteRotate(instr, CommonOps.Rol); break;
            case Mnemonic.rr: RewriteRotate(instr, CommonOps.Ror); break;
            case Mnemonic.sla: RewriteShift(instr, Operator.Shl); break;
            case Mnemonic.sll: RewriteShift(instr, Operator.Shl); break;
            case Mnemonic.slp: RewriteSlp(instr); break;
            case Mnemonic.sra: RewriteShift(instr, Operator.Sar); break;
            case Mnemonic.srl: RewriteShift(instr, Operator.Shr); break;
            case Mnemonic.sub: RewriteSub(instr); break;
            case Mnemonic.swap: RewriteSwap(instr); break;
            case Mnemonic.swaph: RewriteSwaph(instr); break;
            case Mnemonic.xor: RewriteXor(instr); break;
            case Mnemonic.Invalid:
                m.Invalid();
                break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Expression EffectiveAddress(MemoryOperand mem)
    {
        Expression ea = binder.EnsureRegister((RegisterStorage) mem.Base);
        if (mem.Offset is Constant c)
        {
            if (c.IsZero)
                return ea;
            return m.IAdd(ea, c);
        }
        else if (mem.Offset is RegisterStorage reg)
        {
            var idx = binder.EnsureRegister(reg);
            return m.IAdd(ea, idx);
        }
        return ea;
    }

    private Expression Op(C33Instruction instr, int iop)
    {
        var operand = instr.Operands[iop];
        switch (operand)
        {
        case RegisterStorage reg:
            return binder.EnsureRegister(reg);
        case Constant c:
            return c;
        case Address a:
            return a;
        default:
            throw new NotImplementedException();
        }
    }

    private void Rewrite23(C33Instruction  instr, BinaryOperator op, FlagGroupStorage flags)
    {
        Expression src1, src2, dst;
        if (instr.Operands.Length == 2)
        {
            src1 = Op(instr, 0);
            src2 = Op(instr, 1);
            dst = Op(instr, 0);
        }
        else
        {

            src1 = Op(instr, 1);
            src2 = Op(instr, 2);
            dst = Op(instr, 0);
        }
        m.Assign(dst, m.Bin(op, src1, src2));
        m.Assign(binder.EnsureFlagGroup(flags), m.Cond(flags.DataType, dst));
    }

    private void RewriteAdd(C33Instruction instr)
    {
        var regSp = Registers.SP;
        if (instr.Operands[0] == regSp)
        {
            var sp = binder.EnsureRegister(regSp);
            m.Assign(sp, m.IAdd(sp, (Constant) instr.Operands[1]));
        }
        else
        {
            Rewrite23(instr, Operator.IAdd, Registers.CNVZ);
        }
    }

    private void RewriteAnd(C33Instruction instr)
    {
        Rewrite23(instr, Operator.And, Registers.NZ);
    }

    private void RewriteBrk(C33Instruction instr)
    {
        m.SideEffect(m.Fn(brk_intrinsic));
    }

    private void RewriteCall(C33Instruction instr, InstrClass iclass)
    {
        m.Call(Op(instr, 0), 4, iclass);
    }

    private void RewriteCmp(C33Instruction instr)
    {
        int iOp = instr.Operands.Length == 2 ? 0 : 1;
        var left = Op(instr, iOp);
        var right = Op(instr, iOp + 1);
        m.Assign(
            binder.EnsureFlagGroup(Registers.CNVZ),
            m.ISub(left, right));
    }

    private void RewriteHalt(C33Instruction instr)
    {
        m.SideEffect(m.Fn(CommonOps.Halt));
    }

    private void RewriteInt(C33Instruction instr)
    {
        m.SideEffect(m.Fn(CommonOps.Syscall_1, (Constant) instr.Operands[0]));
    }

    private void RewriteJp(C33Instruction instr, InstrClass iclass)
    {
        m.Goto(Op(instr, 0), iclass);
    }

    private void RewriteJpr(C33Instruction instr)
    {
        var pc = instr.Address + (instr.Length - 2);
        m.Goto(m.IAdd(pc, Op(instr, 0)), instr.InstructionClass);
    }

    private void RewriteJr(C33Instruction instr, ConditionCode cc, FlagGroupStorage flags)
    {
        var grf = binder.EnsureFlagGroup(flags);
        m.Branch(m.Test(cc, grf), Op(instr, 0), instr.InstructionClass);
    }

    private void RewriteLd_cf(C33Instruction instr)
    {
        m.Assign(
            binder.EnsureRegister(Registers.PSR),
            m.Fn(ld_cf_intrinsic));
    }

    private void RewriteLoad(C33Instruction instr, PrimitiveType dt)
    {
        if (instr.Operands[0] is MemoryOperand memStore)
        {
            var ea = EffectiveAddress(memStore);
            m.Assign(
                m.Mem(dt, ea),
                Op(instr, 1));
            if (memStore.PostIncrement)
            {
                m.Assign(ea, m.IAddS(ea, dt.Size));
            }
        }
        else if (instr.Operands[1] is MemoryOperand memLoad)
        {
            var ea = EffectiveAddress(memLoad);
            m.Assign(
                Op(instr, 0),
                m.Mem(dt, ea));
            if (memLoad.PostIncrement)
            {
                m.Assign(ea, m.IAddS(ea, dt.Size));
            }
        }
        else if (instr.Operands[1] is RegisterStorage regSrc)
        {
            var dst = Op(instr, 0);
            if (Registers.IsGpRegister(regSrc))
            {
                m.Assign(dst, binder.EnsureRegister(regSrc));
            }
            else
            {
                arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("C33Rw", instr, instr.MnemonicAsString, rdr, "");
            }
        }
        else
        {
            var src = Op(instr, 1);
            var dst = Op(instr, 0);
            m.Assign(dst, src);
        }
    }

    private void RewriteNot(C33Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Comp(src));
        var flags = binder.EnsureFlagGroup(Registers.NZ);
        m.Assign(flags, m.Cond(flags.DataType, dst));
    }

    private void RewriteOr(C33Instruction instr)
    {
        this.Rewrite23(instr, Operator.Or, Registers.NZ);
    }

    private void RewritePop(C33Instruction instr)
    {
        var sp = binder.EnsureRegister(Registers.SP);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Mem32(sp));
        m.Assign(sp, m.IAddS(sp, 4));
    }

    private void RewritePopn(C33Instruction instr)
    {
        var iregMac = ((RegisterStorage) instr.Operands[0]).Number;
        var sp = binder.EnsureRegister(Registers.SP);
        var gps = Registers.GpRegisters;
        int offset = 0;
        int iReg;
        for (iReg = 0; iReg <= iregMac; ++iReg, offset += 4)
        {
            var dst = binder.EnsureRegister(gps[iReg]);
            m.Assign(dst, m.Mem32(m.AddSubSignedInt(sp, offset)));
        }
        m.Assign(sp, m.IAddS(sp, offset));
    }

    private void RewritePops(C33Instruction instr)
    {
        if (instr.Operands[0] == Registers.AHR)
        {
            var sp = binder.EnsureRegister(Registers.SP);
            var ahlr = binder.EnsureSequence(PrimitiveType.Word64, Registers.AHR, Registers.ALR);
            m.Assign(ahlr, m.Mem64(sp));
            m.Assign(sp, m.IAddS(sp, 8));
        }
        else
        {
            RewritePop(instr);
        }
    }

    private void RewritePush(C33Instruction instr)
    {
        var sp = binder.EnsureRegister(Registers.SP);
        m.Assign(sp, m.ISubS(sp, 4));
        var dst = Op(instr, 0);
        m.Assign(m.Mem32(sp), dst);
    }

    private void RewritePushn(C33Instruction instr)
    {
        var iRegMac = ((RegisterStorage) instr.Operands[0]).Number;
        var sp = binder.EnsureRegister(Registers.SP);
        var gps = Registers.GpRegisters;
        int offset = 0;
        int iReg;
        for (iReg = iRegMac; iReg >= 0; --iReg)
        {
            offset -= 4;
            var dst = binder.EnsureRegister(gps[iReg]);
            m.Assign(m.Mem32(m.AddSubSignedInt(sp, offset)), dst);
        }
        m.Assign(sp, m.ISubS(sp, -offset));
    }

    private void RewritePushs(C33Instruction instr)
    {
        if (instr.Operands[0] == Registers.AHR)
        {
            var sp = binder.EnsureRegister(Registers.SP);
            var ahlr = binder.EnsureSequence(PrimitiveType.Word64, Registers.AHR, Registers.ALR);
            m.Assign(sp, m.ISubS(sp, 8));
            m.Assign(m.Mem64(sp), ahlr);
        }
        else
        {
            RewritePush(instr);
        }
    }

    private void RewriteRet(C33Instruction instr)
    {
        m.Return(4, 0, instr.InstructionClass);
    }

    private void RewriteRet(C33Instruction instr, IntrinsicProcedure intrinsic)
    {
        m.SideEffect(m.Fn(intrinsic));
        m.Return(4, 0, instr.InstructionClass);
    }

    private void RewriteRotate(C33Instruction instr, IntrinsicProcedure rotation)
    {
        var src1 = Op(instr, 0);
        var src2 = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(rotation, src1, src2));
        var flags = binder.EnsureFlagGroup(Registers.NZ);
        m.Assign(flags, m.Cond(flags.DataType, dst));
    }

    private void RewriteShift(C33Instruction instr, BinaryOperator shift)
    {
        var src1 = Op(instr, 0);
        var src2 = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Bin(shift, src1, src2));
        var flags = binder.EnsureFlagGroup(Registers.NZ);
        m.Assign(flags, m.Cond(flags.DataType, dst));
    }

    private void RewriteSlp(C33Instruction instr)
    {
        m.SideEffect(m.Fn(slp_intrinsic));
    }

    private void RewriteSub(C33Instruction instr)
    {
        var regSp = Registers.SP;
        if (instr.Operands[0] == regSp)
        {
            var sp = binder.EnsureRegister(regSp);
            m.Assign(sp, m.ISub(sp, (Constant) instr.Operands[1]));
        }
        else
        {
            Rewrite23(instr, Operator.ISub, Registers.CNVZ);
        }
    }

    private void RewriteSwap(C33Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(swap_intrinsic, src));
    }

    private void RewriteSwaph(C33Instruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(swaph_intrinsic, src));
    }

    private void RewriteXor(C33Instruction instr)
    {
        Rewrite23(instr, Operator.Xor, Registers.NZ);
    }

    private static readonly IntrinsicProcedure brk_intrinsic = IntrinsicBuilder.SideEffect("__brk")
        .Void();

    private static readonly IntrinsicProcedure ld_cf_intrinsic = IntrinsicBuilder.SideEffect("__load_coprocessor_flags")
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure retd_intrinsic = IntrinsicBuilder.SideEffect("__return_from_debug_exception")
        .Void();
    private static readonly IntrinsicProcedure reti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
        .Void();

    private static readonly IntrinsicProcedure slp_intrinsic = IntrinsicBuilder.SideEffect("__sleep")
        .Void();
    private static readonly IntrinsicProcedure swap_intrinsic = IntrinsicBuilder.Unary("__swap", PrimitiveType.Word32);
    private static readonly IntrinsicProcedure swaph_intrinsic = IntrinsicBuilder.Unary("__swaph", PrimitiveType.Word32);

}