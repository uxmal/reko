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

namespace Reko.Arch.Motorola.M88k;

internal class M88kRewriter : IEnumerable<RtlInstructionCluster>
{
    private static PrimitiveType dt5 = PrimitiveType.CreateWord(5);

    private readonly M88kArchitecture arch;
    private readonly IServiceProvider services;
    private readonly EndianImageReader rdr;
    private readonly ProcessorState state;
    private readonly IStorageBinder binder;
    private readonly IRewriterHost host;
    private readonly List<RtlInstruction> rtls;
    private readonly RtlEmitter m;
    private readonly IEnumerator<M88kInstruction> dasm;
    private InstrClass iclass;

    public M88kRewriter(M88kArchitecture arch, IServiceProvider services, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
    {
        this.arch = arch;
        this.services = services;
        this.rdr = rdr;
        this.state = state;
        this.binder = binder;
        this.host = host;
        this.rtls = new List<RtlInstruction>();
        this.m = new RtlEmitter(rtls);
        this.dasm = new M88kDisassembler(arch, rdr).GetEnumerator();
    }

    public IEnumerator<RtlInstructionCluster> GetEnumerator()
    {
        while (dasm.MoveNext())
        {
            var instr = dasm.Current;
            this.iclass = instr.InstructionClass;
            switch (instr.Mnemonic)
            {
            case Mnemonic.Invalid:
                m.Invalid();
                iclass = InstrClass.Invalid;
                break;
            case Mnemonic.and: RewriteAnd(instr, 0xFFFF0000, false); break;
            case Mnemonic.and_c: RewriteAnd(instr, 0xFFFF0000, true); break;
            case Mnemonic.and_u: RewriteLogicalHiImm(instr, Operator.And, 0xFFFF); break;
            case Mnemonic.add: RewriteAddSub(instr, Operator.IAdd, false); break;
            case Mnemonic.add_ci: RewriteAddcSubc(instr, m.IAddC, false); break;
            case Mnemonic.add_cio: RewriteAddcSubc(instr, m.IAddC, true); break;
            case Mnemonic.add_co: RewriteAddSub(instr, Operator.IAdd, true); break;
            case Mnemonic.addu: RewriteAddSub(instr, Operator.IAdd, false); break;
            case Mnemonic.addu_ci: RewriteAddcSubc(instr, m.IAddC, false); break;
            case Mnemonic.addu_cio: RewriteAddcSubc(instr, m.IAddC, true); break;
            case Mnemonic.addu_co: RewriteAddSub(instr, Operator.IAdd, true); break;
            case Mnemonic.bb0: RewriteBitBranch(instr, false, true); break;
            case Mnemonic.bb0_n: RewriteBitBranch(instr, false, false); break;
            case Mnemonic.bb1: RewriteBitBranch(instr, true, true); break;
            case Mnemonic.bb1_n: RewriteBitBranch(instr, true, false); break;
            case Mnemonic.bcnd: RewriteBcnd(instr, true); break;
            case Mnemonic.bcnd_n: RewriteBcnd(instr, false); break;
            case Mnemonic.br: RewriteBr(instr, true); break;
            case Mnemonic.br_n: RewriteBr(instr, false); break;
            case Mnemonic.bsr: RewriteBsr(instr, true); break;
            case Mnemonic.bsr_n: RewriteBsr(instr, false); break;
            case Mnemonic.clr: RewriteClr(instr); break;
            case Mnemonic.cmp: RewriteCmp(instr); break;
            case Mnemonic.div: RewriteDiv(instr, Operator.SDiv); break;
            case Mnemonic.divu: RewriteDiv(instr, Operator.UDiv); break;
            case Mnemonic.ext: RewriteExt(instr, ext_intrinsic); break;
            case Mnemonic.extu: RewriteExt(instr, extu_intrinsic); break;
            case Mnemonic.fadd: RewriteFBin(instr, Operator.FAdd); break;
            case Mnemonic.fcmp: RewriteFcmp(instr); break;
            case Mnemonic.fdiv: RewriteFBin(instr, Operator.FDiv); break;
            case Mnemonic.ff0: RewriteFf0(instr); break;
            case Mnemonic.ff1: RewriteFf1(instr); break;
            case Mnemonic.fldcr: RewriteLdcr(instr); break;
            case Mnemonic.flt: RewriteFlt(instr); break;
            case Mnemonic.fstcr: RewriteStcr(instr); break;
            case Mnemonic.fmul: RewriteFBin(instr, Operator.FMul); break;
            case Mnemonic.fsub: RewriteFBin(instr, Operator.FSub); break;
            case Mnemonic.fxcr: RewriteXcr(instr); break;
            case Mnemonic.@int: RewriteRound(instr, int_32_intrinsic, int_64_intrinsic); break;
            case Mnemonic.jmp: RewriteJmp(instr, true); break;
            case Mnemonic.jmp_n: RewriteJmp(instr, false); break;
            case Mnemonic.jsr: RewriteJsr(instr, true); break;
            case Mnemonic.jsr_n: RewriteJsr(instr, false); break;
            case Mnemonic.ld: RewriteLd(instr, PrimitiveType.Word32); break;
            case Mnemonic.ld_b: RewriteLd(instr, PrimitiveType.Int8); break;
            case Mnemonic.ld_bu: RewriteLd(instr, PrimitiveType.Byte); break;
            case Mnemonic.ld_d: RewriteLd(instr, PrimitiveType.Word64); break;
            case Mnemonic.ld_h: RewriteLd(instr, PrimitiveType.Int16); break;
            case Mnemonic.ld_hu: RewriteLd(instr, PrimitiveType.Word16); break;
            case Mnemonic.lda: RewriteLda(instr, PrimitiveType.Word32); break;
            case Mnemonic.lda_b: RewriteLda(instr, PrimitiveType.Byte); break;
            case Mnemonic.lda_d: RewriteLda(instr, PrimitiveType.Word64); break;
            case Mnemonic.lda_h: RewriteLda(instr, PrimitiveType.Word16); break;
            case Mnemonic.ldcr: RewriteLdcr(instr); break;
            case Mnemonic.mak: RewriteMak(instr); break;
            case Mnemonic.mask: RewriteAnd(instr, 0, false); break;
            case Mnemonic.mask_u: RewriteLogicalHiImm(instr, Operator.And, 0); break;
            case Mnemonic.mul: RewriteMul(instr); break;
            case Mnemonic.nint: RewriteRound(instr, FpOps.roundf, FpOps.round); break;
            case Mnemonic.or: RewriteOr(instr, Operator.Or, false); break;
            case Mnemonic.or_c: RewriteOr(instr, Operator.Or, true); break;
            case Mnemonic.or_u: RewriteOr_u(instr, Operator.Or); break;
            case Mnemonic.rot: RewriteRot(instr); break;
            case Mnemonic.rte: RewriteRte(instr); break;
            case Mnemonic.set: RewriteSet(instr); break;
            case Mnemonic.st: RewriteSt(instr, PrimitiveType.Word32); break;
            case Mnemonic.st_b: RewriteSt(instr, PrimitiveType.Byte); break;
            case Mnemonic.st_d: RewriteSt(instr, PrimitiveType.Word64); break;
            case Mnemonic.st_h: RewriteSt(instr, PrimitiveType.Word16); break;
            case Mnemonic.stcr: RewriteStcr(instr); break;
            case Mnemonic.sub: RewriteAddSub(instr, Operator.ISub, false); break;
            case Mnemonic.sub_ci: RewriteAddcSubc(instr, m.ISubC, false); break;
            case Mnemonic.sub_cio: RewriteAddcSubc(instr, m.ISubC, true); break;
            case Mnemonic.sub_co: RewriteAddSub(instr, Operator.ISub, true); break;
            case Mnemonic.subu: RewriteAddSub(instr, Operator.ISub, false); break;
            case Mnemonic.subu_ci: RewriteAddcSubc(instr, m.ISubC, false); break;
            case Mnemonic.subu_cio: RewriteAddcSubc(instr, m.ISubC, true); break;
            case Mnemonic.subu_co: RewriteAddSub(instr, Operator.ISub, true); break;
            case Mnemonic.tb0: RewriteTb(instr, false); break;
            case Mnemonic.tb1: RewriteTb(instr, true); break;
            case Mnemonic.tbnd: RewriteTbnd(instr); break;
            case Mnemonic.tcnd: RewriteTcnd(instr); break;
            case Mnemonic.trnc: RewriteRound(instr, FpOps.truncf, FpOps.trunc); break;
            case Mnemonic.xcr: RewriteXcr(instr); break;
            case Mnemonic.xmem: RewriteXmem(instr, PrimitiveType.Word32); break;
            case Mnemonic.xmem_bu: RewriteXmem(instr, PrimitiveType.Word32); break;
            case Mnemonic.xor: RewriteOr(instr, Operator.Xor, false); break;
            case Mnemonic.xor_c: RewriteOr(instr, Operator.Xor, true); break;
            case Mnemonic.xor_u: RewriteOr_u(instr, Operator.Xor); break;



            default:
                EmitUnitTest(instr);
                m.Invalid();
                iclass = InstrClass.Invalid;
                break;
            }
            yield return m.MakeCluster(instr.Address, instr.Length, iclass);
            rtls.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void EmitUnitTest(M88kInstruction instr, string message = "")
    {
        var testgenSvc = arch.Services.GetService<ITestGenerationService>();
        testgenSvc?.ReportMissingRewriter("M88kRw", instr, instr.Mnemonic.ToString(), rdr, message);
    }

    private void Assign(Expression dst, Expression src)
    {
        if (dst is Constant ||
            dst is Identifier id && id.Storage == Registers.GpRegisters[0])
        {
            m.Nop();
            return;
        }
        m.Assign(dst, src);
    }

    private Expression Op(M88kInstruction instr, int iop)
    {
        var op = instr.Operands[iop];
        return op switch
        {
            RegisterStorage reg => 
                reg.Number == 0 
                    ? Constant.Zero(PrimitiveType.Word32)
                    : binder.EnsureRegister(reg),
            Constant c => c,
            Address a => a,
            MemoryOperand m => EffectiveAddress(m),
            _ => throw new NotImplementedException($"Operand type {op.GetType()} not implemented in M88kRewriter."),
        };
    }

    private Expression FpOp(M88kInstruction instr, int iop, int sh)
    {
        var op = (RegisterStorage) instr.Operands[iop];
        if (instr.FloatSizes.HasValue && 
            ((instr.FloatSizes.Value >> sh) & 0b11) == 0b10)
        {
            if (op.Number == 0)
            {
                return Constant.Zero(PrimitiveType.Real64);
            }
            return binder.EnsureSequence(
                PrimitiveType.Word64,
                op,
                Registers.GpRegisters[(op.Number + 1) & 0x1F]);
        }
        else
        {
            if (op.Number == 0)
            {
                return Constant.Zero(PrimitiveType.Real32);
            }
            return binder.EnsureRegister(op);
        }
    }

    private Expression EffectiveAddress(MemoryOperand mem)
    {
        Expression? ea = null;
        if (mem.Base.Number != 0)
        {
            ea = binder.EnsureRegister(mem.Base);
            if (mem.Index is null)
            {
                ea = m.IAdd(ea, mem.Offset);
            }
            else
            {
                var index = binder.EnsureRegister(mem.Index);
                if (mem.Scale > 1)
                {
                    ea = m.IAdd(ea, m.IMul(index, mem.Scale));
                }
                else
                {
                    ea = m.IAdd(ea, index);
                }
            }
        }
        else
        {
            if (mem.Index is null)
            {
                ea = m.Word32(mem.Offset);
            }
            else
            {
                var index = binder.EnsureRegister(mem.Index);
                if (mem.Scale > 1)
                {
                    ea = m.IMul(index, mem.Scale);
                }
                else
                {
                    ea = index;
                }
            }
        }
        return ea;
    }

    private void RewriteAddSub(M88kInstruction instr, BinaryOperator op, bool setCarry)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        Assign(dst, m.Bin(op, src1, src2));
        if (setCarry)
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Cond(c.DataType, dst));
        }
    }

    private void RewriteAnd(M88kInstruction instr, uint hiMask, bool complement)
    {
        var rSrc1 = (RegisterStorage) instr.Operands[1];
        var src2 = Op(instr, 2);
        if (complement)
        {
            src2 = m.Comp(src2);
        }
        if (src2 is Constant c)
        {
            src2 = Constant.Word32(c.ToUInt32() | hiMask);
        }
        var rDst = (RegisterStorage) instr.Operands[0];
        if (rDst.Number == 0)
        {
            // AND to r0 is a NOP.
            m.Nop();
            return;
        }
        var dst = binder.EnsureRegister(rDst);
        if (rSrc1.Number == 0)
        {
            // AND with r0 is a 0.
            m.Assign(dst, 0);
            return;
        }
        var src1 = binder.EnsureRegister(rSrc1);
        m.Assign(dst, m.Bin(Operator.And, src1, src2));
    }


    private void RewriteBcnd(M88kInstruction instr, bool annul)
    {
        var reg = Op(instr, 1);
        var target = Op(instr, 2);
        if (instr.Operands[0] is ConditionOperand<CCode> ccond)
        {
            Expression e;
            switch (ccond.Condition)
            {
            case CCode.eq0: e = m.Eq0(reg); break;
            case CCode.ne0: e = m.Ne0(reg); break;
            case CCode.le0: e = m.Le0(reg); break;
            case CCode.lt0: e = m.Lt0(reg); break;
            case CCode.ge0: e = m.Ge0(reg); break;
            case CCode.gt0: e = m.Gt0(reg); break;
            default:
                EmitUnitTest(instr);
                e = InvalidConstant.Create(PrimitiveType.Bool);
                break;
            };
            m.Branch(e, target, iclass);
        }
        else
        {
            m.Branch(m.Fn(bcnd_intrinsic, reg), target, iclass);
        }
    }

    private void RewriteBitBranch(M88kInstruction instr, bool bitSet, bool annul)
    {
        var bitPos = (Constant) instr.Operands[0];
        var reg = Op(instr, 1);
        var target = Op(instr, 2);
        Expression predicate = m.Fn(CommonOps.Bit, reg, bitPos);
        if (!bitSet)
        {
            predicate = m.Not(predicate);
        }
        m.Branch(predicate, target, iclass);
    }

    private void RewriteBr(M88kInstruction instr, bool annul)
    {
        var target = Op(instr, 0);
        m.Goto(target, iclass);
    }

    private void RewriteBsr(M88kInstruction instr, bool annul)
    {
        var target = Op(instr, 0);
        m.Call(target, 0, iclass);
    }


    private bool MaybeAnnulNext(M88kInstruction instr, bool annul)
    {
        if (!annul)
            return true;
        if (!dasm.MoveNext())
        {
            m.Invalid();
            return false;
        }
        instr.Length += 4;
        iclass &= ~InstrClass.Delay;
        return true;
    }

    private void RewriteAddcSubc(
        M88kInstruction instr,
        Func<Expression, Expression, Expression, Expression> addc,
        bool setCarry)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var c = binder.EnsureFlagGroup(Registers.C);
        var dst = Op(instr, 0);
        m.Assign(dst, addc(src1, src2, c));
        if (setCarry)
        {
            m.Assign(c, m.Cond(c.DataType, dst));
        }
    }

    private void RewriteClr(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        Expression clr;
        if (instr.Operands[2] is Constant cWidth &&
            instr.Operands[3] is Constant cOffset)
        {
            var w = cWidth.ToInt32() & 0x1F;
            if (w == 0)
                w = 0x20;
            var dt = PrimitiveType.CreateWord(w);
            clr = m.Dpb(src, Constant.Zero(dt), cOffset.ToInt32());
        }
        else
        {
            var s2 = Op(instr, 2);
            var width = binder.CreateTemporary(PrimitiveType.Byte);
            var offset = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(offset, m.Slice(s2, dt5, 0));
            m.Assign(width, m.Slice(s2, dt5, 5));
            clr = m.Fn(clr_intrinsic, src, width, offset);
        }
        var dst = Op(instr, 0);
        Assign(dst, clr);
    }

    private void RewriteCmp(M88kInstruction instr)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Cond(dst.DataType, m.ISub(src1, src2)));
    }

    private void RewriteDiv(M88kInstruction instr, BinaryOperator op)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteExt(M88kInstruction instr, IntrinsicProcedure fn)
    {
        Expression width;
        Expression offset;
            var src = Op(instr, 1);
        if (instr.Operands.Length == 3)
        {
            var s2 = Op(instr, 2);
            width = binder.CreateTemporary(PrimitiveType.Byte);
            offset = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(offset, m.Slice(s2, dt5, 0));
            m.Assign(width, m.Slice(s2, dt5, 5));
        }
        else
        {
            width = (Constant) instr.Operands[2];
            offset = (Constant) instr.Operands[3];
        }
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(fn, src, width, offset));
    }

    private void RewriteFBin(M88kInstruction instr, BinaryOperator op)
    {
        var src1 = FpOp(instr, 1, 2);
        var src2 = FpOp(instr, 2, 4);
        var dst = FpOp(instr, 0, 0);
        var f32 = PrimitiveType.Real32;
        var f64 = PrimitiveType.Real64;
        if (src1.DataType.BitSize < src2.DataType.BitSize)
        {
            src1 = m.Convert(src1, f32, f64);
        }
        else if (src1.DataType.BitSize > src2.DataType.BitSize)
        {
            src2 = m.Convert(src2, f32, f64);
        }
        Expression e = m.Bin(op, src1, src2);
        if (src1.DataType.BitSize > dst.DataType.BitSize)
        {
            e = m.Convert(e, f64, f32);
        }
        else if (src1.DataType.BitSize < dst.DataType.BitSize)
        {
            e = m.Convert(e, f32, f64);
        }
        m.Assign(dst, e);
    }

    private void RewriteFcmp(M88kInstruction instr)
    {
        var src1 = FpOp(instr, 1, 2);
        var src2 = FpOp(instr, 2, 4);
        var dst = Op(instr, 0);
        var f32 = PrimitiveType.Real32;
        var f64 = PrimitiveType.Real64;
        if (src1.DataType.BitSize < src2.DataType.BitSize)
        {
            src1 = m.Convert(src1, f32, f64);
        }
        else if (src1.DataType.BitSize > src2.DataType.BitSize)
        {
            src2 = m.Convert(src2, f32, f64);
        }
        Expression e = m.Cond(dst.DataType, m.FSub(src1, src2));
        m.Assign(dst, e);
    }


    private void RewriteFf0(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(CommonOps.FindFirstZero, src));
    }

    private void RewriteFf1(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(CommonOps.FindFirstOne, src));
    }

    private void RewriteFlt(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        var dst = FpOp(instr, 0, 0);
        var dt = dst.DataType.BitSize == 32
            ? PrimitiveType.Real32
            : PrimitiveType.Real64;
        m.Assign(dst, m.Convert(src, PrimitiveType.Int32, dt));
    }

    private void RewriteJmp(M88kInstruction instr, bool annul)
    {
        var target = Op(instr, 0);
        m.Goto(target, iclass);
    }

    private void RewriteJsr(M88kInstruction instr, bool annul)
    {
        var target = Op(instr, 0);
        m.Call(target, 0, iclass);
    }

    private void RewriteLd(M88kInstruction instr, PrimitiveType dt)
    {
        var ea = Op(instr, 1);
        var rDst = (RegisterStorage)instr.Operands[0];
        Identifier dst;
        if (dt.BitSize == 64)
        {
            if (rDst.Number == 31)
            {
                dst = binder.EnsureRegister(rDst);
                dt = PrimitiveType.Word32;
            }
            else
            {
                var rNext = Registers.GpRegisters[rDst.Number + 1];
                dst = binder.EnsureSequence(PrimitiveType.Word64, rDst, rNext);
            }
        }
        else
        {
            dst = binder.EnsureRegister(rDst);
        }

        Expression src = instr.UserSpace
            ? m.Fn(load_from_userspace_intrinsic.MakeInstance(dt), ea)
            : m.Mem(dt, ea);
        if (src.DataType.BitSize != dst.DataType.BitSize)
        {
            src = m.Convert(src, src.DataType, dst.DataType);
        }
        m.Assign(dst, src);
    }

    private void RewriteLda(M88kInstruction instr, PrimitiveType dt)
    {
        var ea = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, ea);
    }

    private void RewriteLdcr(M88kInstruction instr)
    {
        var cr = Op(instr, 1);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(read_cr_intrinsic, cr));
    }

    private void RewriteLogicalHiImm(M88kInstruction instr, BinaryOperator op, uint loPart)
    {
        var src1 = Op(instr, 1);
        var src2 = (((Constant) instr.Operands[2]).ToUInt32() << 16) | loPart;
        var dst = Op(instr, 0);
        m.Assign(dst, m.Bin(op, src1, m.Word32(src2)));
    }

    private void RewriteLogical(M88kInstruction instr, BinaryOperator op, bool complement)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        if (complement)
            src2 = m.Comp(src2);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteMak(M88kInstruction instr)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        var shifted = m.Bin(Operator.Shl, src2, m.Word32(16));
        Assign(dst, m.Bin(Operator.Or, src1, shifted));
    }

    private void RewriteMul(M88kInstruction instr)
    {
        var src1 = Op(instr, 1);
        var src2 = Op(instr, 2);
        var dst = Op(instr, 0);
        Assign(dst, m.IMul(src1, src2));
    }

    private void RewriteOr(M88kInstruction instr, BinaryOperator op, bool complement)
    {
        var rSrc1 = (RegisterStorage) instr.Operands[1];
        var src2 = Op(instr, 2);
        if (complement)
        {
            src2 = m.Comp(src2);
        }
        var rDst = (RegisterStorage) instr.Operands[0];
        if (rDst.Number == 0)
        {
            // OR/XOR to r0 is a NOP.
            m.Nop();
            return;
        }
        var dst = binder.EnsureRegister(rDst);
        if (rSrc1.Number == 0)
        {
            // OR/XOR with r0 is a MOV.
            m.Assign(dst, src2);
            return;
        }
        var src1 = binder.EnsureRegister(rSrc1);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteOr_u(M88kInstruction instr, BinaryOperator op)
    {
        var rSrc1 = (RegisterStorage) instr.Operands[1];
        var src2 = Constant.Word32(((Constant) instr.Operands[2]).ToUInt32() << 16);
        var rDst = (RegisterStorage) instr.Operands[0];
        if (rDst.Number == 0)
        {
            // OR.u to r0 is a NOP.
            m.Nop();
            return;
        }
        var dst = binder.EnsureRegister(rDst);
        if (rSrc1.Number == 0)
        {
            // OR with r0 is a MOV.
            m.Assign(dst, src2);
            return;
        }
        var src1 = binder.EnsureRegister(rSrc1);
        m.Assign(dst, m.Bin(op, src1, src2));
    }

    private void RewriteRot(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        var shift = Op(instr, 2);
        var dst = Op(instr, 0);
        m.Assign(dst, m.Fn(CommonOps.Ror, src, shift));
    }

    private void RewriteRound(M88kInstruction instr,
        IntrinsicProcedure op32,
        IntrinsicProcedure op64)
    {
        var src = FpOp(instr, 1, 4);
        var dst = Op(instr, 0);
        var (dt, round) = src.DataType.BitSize == 32
            ? (PrimitiveType.Real32, op32)
            : (PrimitiveType.Real64, op64);
        m.Assign(dst, m.Convert(
            m.Fn(round, src),
            dt,
            PrimitiveType.Int32));
    }

    private void RewriteRte(M88kInstruction instr)
    {
        m.SideEffect(m.Fn(rte_intrinsic));
        m.Return(0, 0);
    }


    private void RewriteSet(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        Expression clr;
        if (instr.Operands[2] is Constant cWidth &&
            instr.Operands[3] is Constant cOffset)
        {
            var w = cWidth.ToInt32() & 0x1F;
            if (w == 0)
                w = 0x20;
            var dt = PrimitiveType.CreateWord(w);
            clr = m.Dpb(src, Constant.Create(dt, ~0u), cOffset.ToInt32());
        }
        else
        {
            var s2 = Op(instr, 2);
            var width = binder.CreateTemporary(PrimitiveType.Byte);
            var offset = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(offset, m.Slice(s2, dt5, 0));
            m.Assign(width, m.Slice(s2, dt5, 5));
            clr = m.Fn(clr_intrinsic, src, width, offset);
        }
        var dst = Op(instr, 0);
        Assign(dst, clr);
    }


    private void RewriteSt(M88kInstruction instr, PrimitiveType dt)
    {
        var ea = Op(instr, 1);
        var rSrc = (RegisterStorage) instr.Operands[0];
        Expression src;
        if (dt.BitSize == 64)
        {
            if (rSrc.Number == 31)
            {
                src = binder.EnsureRegister(rSrc);
                dt = PrimitiveType.Word32;
            }
            else
            {
                var rNext = Registers.GpRegisters[rSrc.Number + 1];
                src = binder.EnsureSequence(PrimitiveType.Word64, rSrc, rNext);
            }
        }
        else
        {
            if (rSrc.Number == 0)
                src = Constant.Zero(dt);
            else
                src = binder.EnsureRegister(rSrc);
        }
        if (dt.BitSize != src.DataType.BitSize)
        {
            src = m.Slice(src, dt);
        }
        if (instr.UserSpace)
        {
            m.SideEffect(m.Fn(
                store_to_userspace_intrinsic.MakeInstance(dt),
                ea,
                src));
        }
        else
        {
            Expression dst = m.Mem(dt, ea);
            m.Assign(dst, src);
        }
    }

    private void RewriteStcr(M88kInstruction instr)
    {
        var cr = Op(instr, 0);
        var src = Op(instr, 1);
        m.SideEffect(m.Fn(write_cr_intrinsic, cr, src));
    }

    private void RewriteTb(M88kInstruction instr, bool bitSet)
    {
        Expression predicate = m.Fn(CommonOps.Bit, Op(instr,1), Op(instr, 0));
        if (bitSet)
        {
            predicate = m.Not(predicate);
        }
        m.BranchInMiddleOfInstruction(predicate, instr.NextAddress, InstrClass.ConditionalTransfer);
        m.SideEffect(m.Fn(trap_intrinsic, Op(instr, 2)));
    }

    private void RewriteTbnd(M88kInstruction instr)
    {
        var src1 = Op(instr, 0);
        var src2 = Op(instr, 1);
        m.BranchInMiddleOfInstruction(m.Ule(src1, src2), instr.NextAddress, InstrClass.ConditionalTransfer);
        m.SideEffect(m.Fn(tbnd_intrinsic));
    }

    private void RewriteTcnd(M88kInstruction instr)
    {
        var reg = Op(instr, 1);
        if (instr.Operands[0] is ConditionOperand<CCode> ccond)
        {
            Expression e;
            switch (ccond.Condition)
            {
            case CCode.eq0: e = m.Eq0(reg); break;
            case CCode.ne0: e = m.Ne0(reg); break;
            case CCode.le0: e = m.Le0(reg); break;
            case CCode.lt0: e = m.Lt0(reg); break;
            case CCode.ge0: e = m.Ge0(reg); break;
            case CCode.gt0: e = m.Gt0(reg); break;
            default:
                EmitUnitTest(instr);
                e = InvalidConstant.Create(PrimitiveType.Bool);
                break;
            };
            m.BranchInMiddleOfInstruction(e, instr.NextAddress, InstrClass.ConditionalTransfer);
        }
        else
        {
            m.BranchInMiddleOfInstruction(m.Fn(bcnd_intrinsic, reg), instr.NextAddress, InstrClass.ConditionalTransfer);
        }
        m.SideEffect(m.Fn(trap_intrinsic, Op(instr, 2)));
    }

    private void RewriteXcr(M88kInstruction instr)
    {
        var src = Op(instr, 1);
        var cr = Op(instr, 2);
        var dst = Op(instr, 0);
        Assign(dst, m.Fn(xcr_intrinsic, src, cr));
    }

    private void RewriteXmem(M88kInstruction instr, PrimitiveType dt)
    {
        var ea = EffectiveAddress((MemoryOperand) instr.Operands[1]);
        var dst = Op(instr, 0);
        var fn = instr.UserSpace
            ? xmem_usr_intrinsic
            : AtomicOps.atomic_exchange.MakeInstance(32, dt);
        Assign(dst, m.Fn(fn, ea, dst));
    }

    private static readonly IntrinsicProcedure bcnd_intrinsic = IntrinsicBuilder.Predicate("__bcnd", PrimitiveType.Word32);

    private static readonly IntrinsicProcedure clr_intrinsic = new IntrinsicBuilder("__clr", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure ext_intrinsic = new IntrinsicBuilder("__ext", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure extu_intrinsic = new IntrinsicBuilder("__extu", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure int_32_intrinsic = IntrinsicBuilder.Unary("__int_32", PrimitiveType.Real32);
    private static readonly IntrinsicProcedure int_64_intrinsic = IntrinsicBuilder.Unary("__int_64", PrimitiveType.Real64);

    private static readonly IntrinsicProcedure load_from_userspace_intrinsic = IntrinsicBuilder.SideEffect("__load_from_userspace")
        .GenericTypes("T")
        .Param(PrimitiveType.Ptr32)
        .Returns("T");

    private static readonly IntrinsicProcedure read_cr_intrinsic = IntrinsicBuilder.SideEffect("__read_cr")
        .Param(PrimitiveType.Word32)
        .Returns(PrimitiveType.Word32);
    private static readonly IntrinsicProcedure rte_intrinsic = IntrinsicBuilder.SideEffect("__rte")
        .Void();

    private static readonly IntrinsicProcedure set_intrinsic = new IntrinsicBuilder("__set", false)
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Byte)
        .Param(PrimitiveType.Byte)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure store_to_userspace_intrinsic = IntrinsicBuilder.SideEffect("__store_to_userspace")
        .GenericTypes("T")
        .Param(PrimitiveType.Ptr32)
        .Param("T")
        .Void();

    private static readonly IntrinsicProcedure trap_intrinsic = IntrinsicBuilder.SideEffect("__trap")
        .Param(PrimitiveType.UInt32)
        .Void();
    private static readonly IntrinsicProcedure tbnd_intrinsic = IntrinsicBuilder.SideEffect("__trap_bounds_violation")
        .Void();

    private static readonly IntrinsicProcedure write_cr_intrinsic = IntrinsicBuilder.SideEffect("__write_cr")
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Word32)
        .Void();

    private static readonly IntrinsicProcedure xcr_intrinsic = IntrinsicBuilder.SideEffect("__exchange_cr")
        .Param(PrimitiveType.Word32)
        .Param(PrimitiveType.Word32)
        .Returns(PrimitiveType.Word32);

    private static readonly IntrinsicProcedure xmem_usr_intrinsic = IntrinsicBuilder.SideEffect("__atomic_exchange_userspace")
        .Param(PrimitiveType.Ptr32)
        .Param(PrimitiveType.Word32)
        .Returns(PrimitiveType.Word32);
}
