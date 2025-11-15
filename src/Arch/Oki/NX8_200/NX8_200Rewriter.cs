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
using System.Collections;
using System.Diagnostics;

namespace Reko.Arch.Oki.NX8_200;

public class NX8_200Rewriter : IEnumerable<RtlInstructionCluster>
{
    private readonly NX8_200Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<NX8_200Instruction> dasm;
    private NX8_200Instruction instr;
    private InstrClass iclass;

    public NX8_200Rewriter(NX8_200Architecture nX8_200Architecture, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = nX8_200Architecture;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = [];
        this.m = new RtlEmitter(rtls);
        this.dasm = new NX8_200Disassembler(arch, rdr).GetEnumerator();
        this.instr = default!;
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        var w8 = PrimitiveType.Byte;
        var w16 = PrimitiveType.Word16;
        while (dasm.MoveNext())
        {
            this.instr = dasm.Current;
            this.iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            default:
                EmitUnitTest();
                m.Nop();
                break;
            case Mnemonic.Invalid: m.Invalid(); break;
            case Mnemonic.adc: RewriteAdc(w16); break;
            case Mnemonic.adcb: RewriteAdc(w8); break;
            case Mnemonic.add: RewriteAddSub(Operator.IAdd, w16); break;
            case Mnemonic.addb: RewriteAddSub(Operator.IAdd, w8); break;
            case Mnemonic.and: RewriteLogical(Operator.And, w16); break;
            case Mnemonic.andb: RewriteLogical(Operator.And, w8); break;
            case Mnemonic.brk: RewriteBrk(); break;
            case Mnemonic.cal: RewriteCal(); break;
            case Mnemonic.clr: RewriteClr(w16); break;
            case Mnemonic.clrb: RewriteClr(w8); break;
            case Mnemonic.cmp: RewriteCmp(w16); break;
            case Mnemonic.cmpb: RewriteCmp(w8); break;
            case Mnemonic.cmpc: RewriteCmpc(w16); break;
            case Mnemonic.cmpcb: RewriteCmpc(w8); break;
            case Mnemonic.daa: RewriteDaas(daa_intrinsic); break;
            case Mnemonic.das: RewriteDaas(das_intrinsic); break;
            case Mnemonic.dec: RewriteIncDec(Operator.ISub, w16); break;
            case Mnemonic.decb: RewriteIncDec(Operator.ISub, w8); break;
            case Mnemonic.div: RewriteDiv(); break;
            case Mnemonic.divb: RewriteDivb(); break;
            case Mnemonic.extnd: RewriteExtnd(); break;
            case Mnemonic.inc: RewriteIncDec(Operator.IAdd, w16); break;
            case Mnemonic.incb: RewriteIncDec(Operator.IAdd, w8); break;
            case Mnemonic.j: RewriteJ(); break;
            case Mnemonic.jbr: RewriteJbrs(false); break;
            case Mnemonic.jbs: RewriteJbrs(true); break;
            case Mnemonic.jeq: RewriteJcc(ConditionCode.EQ, Registers.Z); break;
            case Mnemonic.jge: RewriteJcc(ConditionCode.UGE, Registers.C); break;
            case Mnemonic.jgt: RewriteJcc(ConditionCode.UGT, Registers.CZ); break;
            case Mnemonic.jle: RewriteJcc(ConditionCode.ULE, Registers.CZ); break;
            case Mnemonic.jlt: RewriteJcc(ConditionCode.ULT, Registers.C); break;
            case Mnemonic.jne: RewriteJcc(ConditionCode.NE, Registers.Z); break;
            case Mnemonic.jrnz: RewriteJrnz(); break;
            case Mnemonic.l: RewriteL(w16); break;
            case Mnemonic.lb: RewriteL(w8); break;
            case Mnemonic.lc: RewriteLc(w16); break;
            case Mnemonic.lcb: RewriteLc(w8); break;
            case Mnemonic.mb: RewriteMb(); break;
            case Mnemonic.mbr: RewriteMbr(); break;
            case Mnemonic.mov: RewriteMov(w16); break;
            case Mnemonic.movb: RewriteMov(w8); break;
            case Mnemonic.mul: RewriteMul(); break;
            case Mnemonic.mulb: RewriteMulb(); break;
            case Mnemonic.or: RewriteLogical(Operator.Or, w16); break;
            case Mnemonic.orb: RewriteLogical(Operator.Or, w8); break;
            case Mnemonic.pops: RewritePops(); break;
            case Mnemonic.pushs: RewritePushs(); break;
            case Mnemonic.rb: RewriteSetResetB(false); break;
            case Mnemonic.rbr: RewriteSetResetBr(false); break;
            case Mnemonic.rc: RewriteSetResetC(false); break;
            case Mnemonic.rol: RewriteRotate(CommonOps.RolC, w16); break;
            case Mnemonic.rolb: RewriteRotate(CommonOps.RolC, w8); break;
            case Mnemonic.ror: RewriteRotate(CommonOps.RorC, w16); break;
            case Mnemonic.rorb: RewriteRotate(CommonOps.RorC, w8); break;
            case Mnemonic.rt: RewriteRt(); break;
            case Mnemonic.rti: RewriteRti(); break;
            case Mnemonic.sb: RewriteSetResetB(true); break;
            case Mnemonic.sbr: RewriteSetResetBr(true); break;
            case Mnemonic.sbc: RewriteSbc(w16); break;
            case Mnemonic.sbcb: RewriteSbc(w8); break;
            case Mnemonic.sc: RewriteSetResetC(true); break;
            case Mnemonic.scal: RewriteCal(); break;
            case Mnemonic.sj: RewriteJ(); break;
            case Mnemonic.sll: RewriteShift(Operator.Shl, w16); break;
            case Mnemonic.sllb: RewriteShift(Operator.Shl, w8); break;
            case Mnemonic.sra: RewriteShift(Operator.Sar, w16); break;
            case Mnemonic.srab: RewriteShift(Operator.Sar, w8); break;
            case Mnemonic.srl: RewriteShift(Operator.Shr, w16); break;
            case Mnemonic.srlb: RewriteShift(Operator.Shr, w8); break;
            case Mnemonic.st: RewriteSt(w16); break;
            case Mnemonic.stb: RewriteSt(w8); break;
            case Mnemonic.sub: RewriteAddSub(Operator.ISub, w16); break;
            case Mnemonic.subb: RewriteAddSub(Operator.ISub, w8); break;
            case Mnemonic.swap: RewriteSwap(swap_intrinsic, w16); break;
            case Mnemonic.swapb: RewriteSwap(swapb_intrinsic, w8); break;
            case Mnemonic.tbr: RewriteTbr(); break;
            case Mnemonic.vcal: RewriteCal(); break;
            case Mnemonic.xchg: RewriteXchg(w16); break;
            case Mnemonic.xchgb: RewriteXchg(w8); break;
            case Mnemonic.xnbl: RewriteXnbl(); break;
            case Mnemonic.xor: RewriteLogical(Operator.Xor, w16); break;
            case Mnemonic.xorb: RewriteLogical(Operator.Xor, w8); break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Expression EffectiveAddress(MemoryOperand mem)
    {
        Expression ea;
        if (mem.Base is not null)
        {
            ea = binder.EnsureRegister(mem.Base);
            ea = m.AddSubSignedInt(ea, mem.Offset);
            return ea;
        }
        ea = Address.Ptr16((ushort) mem.Offset);
        return ea;
    }

    private void EmitUnitTest()
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingRewriter("NX8_200Rw", instr, instr.Mnemonic.ToString(), rdr, "");
    }

    private Expression OpSrc(int iop, PrimitiveType? dt = null)
    {
        var op = instr.Operands[iop];
        return OpSrc(op, dt);
    }

    private Expression OpSrc(MachineOperand op, PrimitiveType? dt = null)
    {
        switch (op)
        {
        case RegisterStorage reg:
            return MaybeSlice(binder.EnsureRegister(reg), dt);
        case FlagGroupStorage flag:
            return binder.EnsureFlagGroup(flag);
        case Constant c:
            return c;
        case Address addr:
            return (Expression) op;
        case MemoryOperand mem:
            var ea = EffectiveAddress(mem);
            Debug.Assert(dt is not null);
            return m.Mem(dt, ea);
        case BitOperand bop:
            return m.Fn(CommonOps.Bit, OpSrc(bop.Operand, PrimitiveType.Byte), m.Byte((byte)bop.BitPosition));
        default:
            throw new NotImplementedException($"Operand type {op.GetType()} not implemented yet.");
        }
    }

    private Expression MaybeDpb(Expression dst, Expression src)
    {
        if (dst.DataType.BitSize > src.DataType.BitSize)
        {
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(dst, m.Dpb(dst, src, 0));
        }
        else
        {
            m.Assign(dst, src);
        }
        return src;
    }

    private Expression MaybeSlice(Expression e, PrimitiveType? dt)
    {
        if (dt is null)
            return e;
        if (dt.BitSize < e.DataType.BitSize)
            return m.Slice(e, dt);
        return e;
    }

    private Expression OpDst(int iop, Expression src, PrimitiveType? dt = null)
    {
        var op = instr.Operands[iop];
        switch (op)
        {
        case RegisterStorage reg:
            var id = binder.EnsureRegister(reg);
            MaybeDpb(id, src);
            return id;
        case FlagGroupStorage flag:
            id = binder.EnsureFlagGroup(flag);
            m.Assign(id, src);
            return id;
        case MemoryOperand mem:
            var ea = EffectiveAddress(mem);
            Debug.Assert(dt is not null);
            m.Assign(m.Mem(dt, ea), src);
            return src;
        default:
            throw new NotImplementedException($"Operand type {op.GetType()} not implemented yet.");
        }
    }

    private void RewriteAdc(PrimitiveType dt)
    {
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var c = binder.EnsureFlagGroup(Registers.C);
        var result = OpDst(0, m.IAddC(left, right, c));
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, result));
    }

    private void RewriteAddSub(BinaryOperator op, PrimitiveType dt)
    {
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var result = OpDst(0, m.Bin(op, left, right), dt);
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, result));
    }

    private void RewriteBrk()
    {
        m.SideEffect(m.Fn(brk_intrinsic));
    }

    private void RewriteCal()
    {
        m.Call(OpSrc(0, PrimitiveType.Word16), 2);
    }

    private void RewriteClr(PrimitiveType dt)
    {
        var dst = OpDst(0, Constant.Zero(dt), dt);
    }

    private void RewriteCmp(PrimitiveType dt)
    {
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, m.ISub(left, right)));
    }

    private void RewriteCmpc(PrimitiveType dt)
    {
        //$TODO: cmpc needs segments.
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, m.ISub(left, right)));
    }

    private void RewriteDaas(IntrinsicProcedure intrinsic)
    {
        var a = binder.EnsureRegister(Registers.Acc);
        var src = m.Slice(a, PrimitiveType.Byte);
        var call = m.Fn(intrinsic, src);
        var tmp = binder.CreateTemporary(src.DataType);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(tmp, call);
        m.Assign(a, m.Dpb(a, tmp, 0));
        m.Assign(c, m.Cond(c.DataType, tmp));
    }

    private void RewriteDiv()
    {
        var dividend = binder.EnsureSequence(
            PrimitiveType.Word32,
            Registers.ERegisters[0],
            Registers.Acc);
        var divisor = binder.EnsureRegister(Registers.ERegisters[2]);
        var remainder = binder.EnsureRegister(Registers.ERegisters[1]);
        m.Assign(dividend, m.UDiv(PrimitiveType.UInt32, dividend, divisor));
        m.Assign(remainder, m.UMod(PrimitiveType.UInt16, dividend, divisor));
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, dividend));
    }

    private void RewriteDivb()
    {
        var dividend = binder.EnsureRegister(Registers.Acc);
        var divisor = binder.EnsureRegister(Registers.BRegisters[0]);
        var remainder = binder.EnsureRegister(Registers.BRegisters[1]);
        m.Assign(dividend, m.UDiv(PrimitiveType.UInt16, dividend, divisor));
        m.Assign(remainder, m.UMod(PrimitiveType.UInt8, dividend, divisor));
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, dividend));
    }

    private void RewriteExtnd()
    {
        var a = binder.EnsureRegister(Registers.Acc);
        var tmp = binder.CreateTemporary(PrimitiveType.Byte);
        m.Assign(tmp, m.Slice(a, tmp.DataType));
        m.Assign(a, m.Convert(tmp, PrimitiveType.Int8, PrimitiveType.Int16));
    }

    private void RewriteJ()
    {
        m.Goto(OpSrc(0, PrimitiveType.Word16));
    }

    private void RewriteJbrs(bool isset)
    {
        var cond = OpSrc(0);
        if (!isset)
        {
            cond = cond.Invert();
        }
        var target = (Expression) instr.Operands[1];
        m.Branch(cond, target);
    }

    private void RewriteJcc(ConditionCode cc, FlagGroupStorage grf)
    {
        var test = m.Test(cc, binder.EnsureFlagGroup(grf));
        m.Branch(test, (Expression) instr.Operands[0]);
    }

    private void RewriteIncDec(BinaryOperator op, PrimitiveType dt)
    {
        var src = OpSrc(0, dt);
        var dst = OpDst(0, m.Bin(op, src, m.Const(dt, 1)), dt);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(z, m.Cond(z.DataType, dst));
    }

    private void RewriteLogical(BinaryOperator op, PrimitiveType dt)
    {
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var result = OpDst(0, m.Bin(op, dt, left, right), dt);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(z, m.Cond(z.DataType, result));
    }

    private void RewriteMb()
    {
        var src = OpSrc(1, PrimitiveType.Byte);
        if (instr.Operands[0] is BitOperand bit)
        {
            var lvalue = OpSrc(bit.Operand, PrimitiveType.Byte);
            m.Assign(lvalue,
                m.Fn(CommonOps.WriteBit.MakeInstance(PrimitiveType.Byte,PrimitiveType.Byte),
                    lvalue,
                    m.Byte((byte) bit.BitPosition),
                    src));
        }
        else
        {
            OpDst(0, src, PrimitiveType.Byte);
        }
    }

    private void RewriteMbr()
    {
        var src = OpSrc(1, PrimitiveType.Byte);
        var c = binder.EnsureFlagGroup(Registers.C);
        var a = binder.EnsureRegister(Registers.Acc);
        var al = binder.CreateTemporary(PrimitiveType.Byte);
        m.Assign(al, m.Slice(a, al.DataType));
        if (instr.Operands[1] == Registers.C)
        {
            var lvalue = OpSrc(0, PrimitiveType.Byte);
            m.Assign(lvalue,
                m.Fn(CommonOps.WriteBit.MakeInstance(PrimitiveType.Byte, PrimitiveType.Byte),
                    lvalue,
                    al,
                    c));
        }
        else
        {
            m.Assign(c,
                m.Fn(
                    CommonOps.Bit,
                    OpSrc(1, PrimitiveType.Byte),
                    al));
        }
    }

    private void RewriteJrnz()
    {
        var dp = binder.EnsureRegister(Registers.Dp);
        var dpl = binder.CreateTemporary(PrimitiveType.Byte);
        m.Assign(dpl, m.Slice(dp, dpl.DataType));
        m.Assign(dpl, m.ISub(dpl, 1));
        m.Assign(dp, m.Dpb(dp, dpl, 0));
        m.Branch(m.Ne0(dpl), (Expression) instr.Operands[1]);
    }

    private void RewriteLc(PrimitiveType dt)
    {
        //$TODO: implement __data like 8051
        var src = OpSrc(1, dt);
        var dst = OpDst(0, src, dt);
    }

    private void RewriteL(PrimitiveType dt)
    {
        var src = OpSrc(1, dt);
        var dst = OpDst(0, src, dt);
    }

    private void RewriteMov(PrimitiveType dt)
    {
        var src = OpSrc(1, dt);
        var dst = OpDst(0, src, dt);
        if (instr.Operands[0] == Registers.Pswh)
        {
            m.Assign(binder.EnsureFlagGroup(Registers.CZSV), dst);
        }
    }

    private void RewriteMul()
    {
        var lhs = binder.EnsureRegister(Registers.Acc);
        var rhs = binder.EnsureRegister(Registers.ERegisters[0]);
        var product = binder.EnsureSequence(PrimitiveType.Word32,
            Registers.ERegisters[1],
            Registers.Acc);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(product, m.UMul(PrimitiveType.UInt32, lhs, rhs));
        m.Assign(z, m.Cond(z.DataType, product));
    }

    private void RewriteMulb()
    {
        var lhs = m.Slice(binder.EnsureRegister(Registers.Acc), PrimitiveType.Byte, 0);
        var rhs = binder.EnsureRegister(Registers.BRegisters[0]);
        var product = binder.EnsureRegister(Registers.Acc);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(product, m.UMul(PrimitiveType.UInt16, lhs, rhs));
        m.Assign(z, m.Cond(z.DataType, product));
    }

    private void RewritePops()
    {
        var ssp = binder.EnsureRegister(Registers.Ssp);
        var obj = OpSrc(0);
        m.Assign(ssp, m.IAddS(ssp, 2));
        m.Assign(obj, m.Mem16(ssp));
    }

    private void RewritePushs()
    {
        var obj = OpSrc(0);
        var ssp = binder.EnsureRegister(Registers.Ssp);
        m.Assign(m.Mem16(ssp), obj);
        m.Assign(ssp, m.ISubS(ssp, 2));
    }

    private void RewriteSbc(PrimitiveType dt)
    {
        var left = OpSrc(0, dt);
        var right = OpSrc(1, dt);
        var c = binder.EnsureFlagGroup(Registers.C);
        var result = OpDst(0, m.ISubC(left, right, c), dt);
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(cz, m.Cond(cz.DataType, result));
    }

    private void RewriteSetResetB(bool flag)
    {
        var src = Constant.Bool(flag);
        var bit = (BitOperand) instr.Operands[0];
        var lvalue = OpSrc(bit.Operand, PrimitiveType.Byte);
        m.Assign(lvalue,
            m.Fn(CommonOps.WriteBit.MakeInstance(PrimitiveType.Byte, PrimitiveType.Byte),
                lvalue,
                m.Byte((byte) bit.BitPosition),
                src));
    }

    private void RewriteSetResetBr(bool flag)
    {
        var src = Constant.Bool(flag);
        var a = binder.EnsureRegister(Registers.Acc);
        var al = binder.CreateTemporary(PrimitiveType.Byte);
        m.Assign(al, m.Slice(a, al.DataType));
        var lvalue = OpSrc(0, PrimitiveType.Byte);
        m.Assign(lvalue,
            m.Fn(CommonOps.WriteBit.MakeInstance(PrimitiveType.Byte, PrimitiveType.Byte),
                lvalue,
                al,
                src));
    }

    private void RewriteSetResetC(bool flag)
    {
        var src = Constant.Bool(flag);
        var c = binder.EnsureFlagGroup(Registers.C);
        m.Assign(c, src);
    }

    private void RewriteRotate(IntrinsicProcedure rot, PrimitiveType dt)
    {
        var src = OpSrc(0, dt);
        var one = m.Byte(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = OpDst(0, m.Fn(
            rot.MakeInstance(dt, one.DataType),
            src, 
            one,
            c),
            dt);
        var cz = binder.EnsureFlagGroup(Registers.CZ);
        m.Assign(c, m.Cond(c.DataType, dst));
    }

    private void RewriteRt()
    {
        m.Return(2, 0);
    }

    private void RewriteRti()
    {
        m.SideEffect(m.Fn(rti_intrinsic));
        m.Return(2, 0);
    }

    private void RewriteShift(BinaryOperator shift, PrimitiveType dt)
    {
        var src = OpSrc(0, dt);
        var one = m.Byte(1);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = OpDst(0, m.Bin(shift, src, one), dt);
        m.Assign(c, m.Cond(c.DataType, dst));
    }

    private void RewriteSt(PrimitiveType dt)
    {
        var src = OpSrc(0, dt);
        var dst = OpDst(1, src, dt);
        if (instr.Operands[1] == Registers.Pswl)
        {
            m.Assign(binder.EnsureFlagGroup(Registers.CZSV), src);
        }
    }

    private void RewriteSwap(IntrinsicProcedure swap, PrimitiveType dt)
    {
        var a = binder.EnsureRegister(Registers.Acc);
        var src = MaybeSlice(a, dt);
        MaybeDpb(a, m.Fn(swap, src));
    }

    private void RewriteTbr()
    {
        var al = binder.CreateTemporary(PrimitiveType.Byte);
        var src = m.Slice(binder.EnsureRegister(Registers.Acc), PrimitiveType.Byte);
        var z = binder.EnsureFlagGroup(Registers.Z);
        m.Assign(z,
            m.Fn(
                CommonOps.Bit,
                OpSrc(0, PrimitiveType.Byte),
                src));
    }

    private void RewriteXchg(PrimitiveType dt)
    {
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, OpSrc(0, dt));
        OpDst(0, OpSrc(1, dt), dt);
        OpDst(1, tmp, dt);
    }

    private void RewriteXnbl()
    {
        var a = binder.EnsureRegister(Registers.Acc);
        var al = binder.CreateTemporary(PrimitiveType.Byte);
        var mem = (MemoryOperand)instr.Operands[0];
        var ea = EffectiveAddress(mem);
        m.Assign(al, m.Slice(a, al.DataType));
        m.Assign(al, m.Fn(xnbl_intrinsic, ea, al));
        m.Assign(a, m.Dpb(a, al, 0));
    }

    private static readonly IntrinsicProcedure brk_intrinsic = IntrinsicBuilder.SideEffect("__brk")
        .Void();
    private static readonly IntrinsicProcedure daa_intrinsic = IntrinsicBuilder.Unary("__daa", PrimitiveType.Byte);
    private static readonly IntrinsicProcedure das_intrinsic = IntrinsicBuilder.Unary("__das", PrimitiveType.Byte);
    private static readonly IntrinsicProcedure rti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
        .Void();
    private static readonly IntrinsicProcedure swap_intrinsic = IntrinsicBuilder.Unary("__swap_bytes", PrimitiveType.Word16);
    private static readonly IntrinsicProcedure swapb_intrinsic = IntrinsicBuilder.Unary("__swap_nybbles", PrimitiveType.Byte);
    private static readonly IntrinsicProcedure xnbl_intrinsic = new IntrinsicBuilder("__exchange_nybble", false)
        .Param(PrimitiveType.Ptr16)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Byte);
}
