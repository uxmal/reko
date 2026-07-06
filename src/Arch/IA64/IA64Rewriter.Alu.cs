using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Arch.IA64;

public partial class IA64Rewriter
{
    private void RewriteAdd(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.IAdd(src2, src1));
    }

    private void RewriteAddp4(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.Fn(addp4_intrinsic, src2, src1));
    }

    private void RewriteAnd(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        WriteOp(instr, 0, m.And(src2, src1));
    }

    private void RewriteAndcm(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        WriteOp(instr, 0, m.And(src1, m.Comp(src2)));
    }

    private void RewriteCmp(IA64Instruction instr, ConditionalOperator cT, ConditionalOperator cF)
    {
        Debug.Assert(instr.Completer == 0);
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var cmpT = m.Bin(cT, PrimitiveType.Bool, src1, src2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, src1, src2);
        WriteOpIgnoreQp0(instr, 0, cmpT);
        WriteOpIgnoreQp0(instr, 1, cmpF);
    }

    private void RewriteCmp(
        IA64Instruction instr,
        ConditionalOperator cT,
        ConditionalOperator cF,
        BinaryOperator connective,
        BinaryOperator invConnective)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var cmpT = m.Bin(cT, PrimitiveType.Bool, src1, src2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, src1, src2);
        if (qp is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, qp, cmpT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(invConnective, PrimitiveType.Bool, qp, cmpF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, cmpT);
            WriteOpIgnoreQp0(instr, 1, cmpF);
        }
    }

    private void RewriteCmpCm(
        IA64Instruction instr,
        ConditionalOperator cT,
        ConditionalOperator cF,
        BinaryOperator connective,
        BinaryOperator invConnective)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var cmpT = m.Bin(cT, PrimitiveType.Bool, src1, src2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, src1, src2);
        if (qp is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), cmpT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(invConnective, PrimitiveType.Bool, m.Not(qp), cmpF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, cmpT);
            WriteOpIgnoreQp0(instr, 1, cmpF);
        }
    }



    private void RewriteCmp4(IA64Instruction instr, ConditionalOperator cT, ConditionalOperator cF)
    {
        Debug.Assert(instr.Completer == 0);
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var tmp1 = binder.CreateTemporary(PrimitiveType.Word32);
        var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(tmp1, m.Slice(src1, tmp1.DataType));
        m.Assign(tmp2, m.Slice(src2, tmp2.DataType));
        var cmpT = m.Bin(cT, PrimitiveType.Bool, tmp1, tmp2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, tmp1, tmp2);
        WriteOpIgnoreQp0(instr, 0, cmpT);
        WriteOpIgnoreQp0(instr, 1, cmpF);
    }

    private void RewriteCmp4(
    IA64Instruction instr,
    ConditionalOperator cT,
    ConditionalOperator cF,
    BinaryOperator connective,
    BinaryOperator invConnective)
    {
        Debug.Assert(instr.Completer == 0);
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var tmp1 = binder.CreateTemporary(PrimitiveType.Word32);
        var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(tmp1, m.Slice(src1, tmp1.DataType));
        m.Assign(tmp2, m.Slice(src2, tmp2.DataType));
        var cmpT = m.Bin(cT, PrimitiveType.Bool, tmp1, tmp2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, tmp1, tmp2);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        if (qp is not null)
        {
            WriteOp(instr, 0, m.Bin(connective, PrimitiveType.Bool, qp, cmpT));
            WriteOp(instr, 1, m.Bin(invConnective, PrimitiveType.Bool, qp, cmpF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, cmpT);
            WriteOpIgnoreQp0(instr, 1, cmpF);
        }
    }

    private void RewriteCmp4Cm(
        IA64Instruction instr,
        ConditionalOperator cT,
        ConditionalOperator cF,
        BinaryOperator connective,
        BinaryOperator invConnective)
    {
        Debug.Assert(instr.Completer == 0);
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        var tmp1 = binder.CreateTemporary(PrimitiveType.Word32);
        var tmp2 = binder.CreateTemporary(PrimitiveType.Word32);
        m.Assign(tmp1, m.Slice(src1, tmp1.DataType));
        m.Assign(tmp2, m.Slice(src2, tmp2.DataType));
        var cmpT = m.Bin(cT, PrimitiveType.Bool, tmp1, tmp2);
        var cmpF = m.Bin(cF, PrimitiveType.Bool, tmp1, tmp2);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        if (qp is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), cmpT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(invConnective, PrimitiveType.Bool, m.Not(qp), cmpF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, cmpT);
            WriteOpIgnoreQp0(instr, 1, cmpF);
        }
    }

    private void RewriteCmpxchg(IA64Instruction instr, PrimitiveType dt, IntrinsicProcedure intrinsic)
    {
        var mem = ReadOp(instr, 1, dt);
        var value = binder.CreateTemporary(dt);
        m.Assign(value, m.MaybeSlice(ReadOp(instr, 2), dt));
        var ptrDt = new PointerType(dt, 64);
        WriteOp(instr, 0, MaybeExtendZ(
            m.Fn(
                intrinsic.MakeInstance(64, dt),
                m.AddrOf(ptrDt, mem),
                value,
                binder.EnsureRegister(Registers.CCV)),
            PrimitiveType.Word64));
    }

    private void RewriteCzx(IA64Instruction instr, PrimitiveType dtElement, IntrinsicProcedure intrinsic)
    {
        var src = ReadOp(instr, 1);
        WriteOp(instr, 0, m.Fn(intrinsic.MakeInstance(dtElement), src));
    }

    private void RewriteDep(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1);
        var op2 = ReadOp(instr, 2);
        var pos = ((Constant) instr.Operands[3]).ToInt32();
        var len = ((Constant) instr.Operands[4]).ToInt32();
        var dt = PrimitiveType.Create(Domain.SignedInt, len);
        var value = src is Constant c
            ? m.Const(dt, c.ToInt64())
            : (Expression) m.Slice(src, dt);
        WriteOp(instr, 0, m.Dpb(op2, value, pos));
    }

    private void RewriteDep_z(IA64Instruction instr)
    {
        Debug.Assert(instr.Operands.Length == 4);
        var src = m.Zero(PrimitiveType.Word64);
        var pos = ((Constant) instr.Operands[2]).ToInt32();
        var len = ((Constant) instr.Operands[3]).ToInt32();
        var dt = PrimitiveType.Create(Domain.SignedInt, len);
        var op1 = ReadOp(instr, 0);
        var value = op1 is Constant c
            ? m.Const(dt, c.ToInt64())
            : (Expression) m.Slice(op1, dt);
        WriteOp(instr, 0, m.Dpb(src, value, pos));
    }

    private void RewriteExtr(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1);
        var pos = ((Constant) instr.Operands[2]).ToInt32();
        var len = ((Constant) instr.Operands[3]).ToInt32();
        var dt = PrimitiveType.Create(Domain.SignedInt, len);
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, m.Slice(src, dt, pos));
        WriteOp(instr, 0, m.ExtendS(tmp, PrimitiveType.Int64));
    }

    private void RewriteExtr_u(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1);
        var pos = ((Constant) instr.Operands[2]).ToInt32();
        var len = ((Constant) instr.Operands[3]).ToInt32();
        var dt = PrimitiveType.CreateWord(len);
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, m.Slice(src, dt, pos));
        WriteOp(instr, 0, m.ExtendZ(tmp, PrimitiveType.Word64));
    }

    private void RewriteLd(IA64Instruction instr, PrimitiveType dt)
    {
        var src = ReadOp(instr, 1, dt);
        src = MaybeExtendZ(src, PrimitiveType.Word64);
        WriteOp(instr, 0, src);
        if (instr.Operands.Length == 3)
        {
            var mem = (MemoryOperand) instr.Operands[1];
            var inc = ReadOp(instr, 2);
            var reg = binder.EnsureRegister(mem.Base);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteLd_acq(IA64Instruction instr, PrimitiveType dt)
    {
        var mem = (MemoryOperand) instr.Operands[1];
        var reg = binder.EnsureRegister(mem.Base);
        WriteOp(instr, 0, m.Fn(ld_acq_intrinsic.MakeInstance(64, dt), reg));
        if (instr.Operands.Length == 3)
        {
            var inc = ReadOp(instr, 2);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteLdfe(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1, PrimitiveType.Word80);
        src = MaybeExtendZ(src, PrimitiveType.Word80);
        WriteOp(instr, 0, m.Convert(src, PrimitiveType.Real80, PrimitiveType.Real64));
        if (instr.Operands.Length == 3)
        {
            var mem = (MemoryOperand) instr.Operands[1];
            var inc = ReadOp(instr, 2);
            var reg = binder.EnsureRegister(mem.Base);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteLdfps(IA64Instruction instr)
    {
        var src = ReadOp(instr, 2, PrimitiveType.Real32);
        WriteOp(instr, 0, src);
        var ea = ((MemoryAccess) src).EffectiveAddress;
        WriteOp(instr, 1, m.Mem(PrimitiveType.Real32, m.IAdd(ea, 4)));
        if (instr.Operands.Length == 4)
        {
            var mem = (MemoryOperand) instr.Operands[2];
            var inc = ReadOp(instr, 3);
            var reg = binder.EnsureRegister(mem.Base);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteMix(IA64Instruction instr, IntrinsicProcedure intrinsic, PrimitiveType dtElement)
    {
        var op1 = ReadOp(instr, 1);
        var op2 = ReadOp(instr, 2);
        WriteOp(instr, 0, m.Fn(intrinsic.MakeInstance(dtElement), op1, op2));
    }

    private void RewriteMov(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1);
        WriteOp(instr, 0, src);
    }

    private void RewriteOr(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.Or(src2, src1));
    }

    private void RewriteSetf_sig(IA64Instruction instr)
    {
        var src = ReadOp(instr, 1);
        WriteOp(instr, 0, m.Fn(setf_sig_intrinsic, src));
    }

    private void RewriteShift(IA64Instruction instr, BinaryOperator shift)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        WriteOp(instr, 0, m.Bin(shift, src1, src2));

    }

    private void RewriteShladd(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        var src3 = ReadOp(instr, 3);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.IAdd(m.Shl(src1, src2), src3));
    }

    private void RewriteSt(IA64Instruction instr, PrimitiveType dt)
    {
        var src = ReadOp(instr, 1, dt);
        src = m.MaybeSlice(src, dt);
        WriteOp(instr, 0, src, dt);
        if (instr.Operands.Length == 3)
        {
            var mem = (MemoryOperand) instr.Operands[0];
            var inc = ReadOp(instr, 2);
            var reg = binder.EnsureRegister(mem.Base);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteSt_rel(IA64Instruction instr, PrimitiveType dt)
    {
        var src = ReadOp(instr, 1, dt);
        var mem = (MemoryOperand) instr.Operands[0];
        var reg = binder.EnsureRegister(mem.Base);
        m.SideEffect(m.Fn(st_rel_intrinsic.MakeInstance(64, dt), reg, src));
        WriteOp(instr, 0, src, PrimitiveType.Word64);
        if (instr.Operands.Length == 3)
        {
            var inc = ReadOp(instr, 2);
            m.Assign(reg, m.IAdd(reg, inc));
        }
    }

    private void RewriteSub(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.ISub(src2, src1));
    }

    private void RewriteSxt(IA64Instruction instr, PrimitiveType dt)
    {
        var src = ReadOp(instr, 1);
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, m.Slice(src, dt));
        WriteOp(instr, 0, m.ExtendS(tmp, PrimitiveType.Int64));
    }

    private void RewriteTbit(IA64Instruction instr, bool invert, BinaryOperator? connective = null)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        Expression testbitT = m.Fn(CommonOps.Bit, src1, src2);
        if (invert)
            testbitT = m.Not(testbitT);
        Expression testbitF = m.Fn(CommonOps.Bit, src1, src2);
        if (!invert)
            testbitF = m.Not(testbitF);
        if (qp is not null && connective is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, qp, testbitT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(connective, PrimitiveType.Bool, qp, testbitF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, testbitT);
            WriteOpIgnoreQp0(instr, 1, testbitF);
        }
    }

    private void RewriteTnat(IA64Instruction instr, bool invert, BinaryOperator? connective = null)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        Expression testbitT = m.Fn(tnat_intrinsic, src1);
        if (invert)
            testbitT = m.Not(testbitT);
        Expression testbitF = m.Fn(tnat_intrinsic, src1);
        if (!invert)
            testbitF = m.Not(testbitF);
        if (qp is not null && connective is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, qp, testbitT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(connective, PrimitiveType.Bool, qp, testbitF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, testbitT);
            WriteOpIgnoreQp0(instr, 1, testbitF);
        }
    }

    private void RewriteTbitCm(IA64Instruction instr, bool invert, BinaryOperator? connective = null)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        var src2 = ReadOp(instr, 3);
        Expression testbitT = m.Fn(CommonOps.Bit, src1, src2);
        if (invert)
            testbitT = m.Not(testbitT);
        Expression testbitF = m.Fn(CommonOps.Bit, src1, src2);
        if (!invert)
            testbitF = m.Not(testbitF);
        if (qp is not null && connective is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), testbitT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), testbitF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, testbitT);
            WriteOpIgnoreQp0(instr, 1, testbitF);
        }
    }

    private void RewriteTnatCm(IA64Instruction instr, bool invert, BinaryOperator? connective = null)
    {
        Debug.Assert(instr.Completer == 0);
        var qp = (instr.QualifyingPredicate is not null &&
                  instr.QualifyingPredicate != Registers.PredicateRegisters[0])
            ? binder.EnsureFlagGroup(instr.QualifyingPredicate)
            : null;
        var src1 = ReadOp(instr, 2);
        Expression testbitT = m.Fn(tnat_intrinsic, src1);
        if (invert)
            testbitT = m.Not(testbitT);
        Expression testbitF = m.Fn(tnat_intrinsic, src1);
        if (!invert)
            testbitF = m.Not(testbitF);
        if (qp is not null && connective is not null)
        {
            WriteOpIgnoreQp0(instr, 0, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), testbitT));
            WriteOpIgnoreQp0(instr, 1, m.Bin(connective, PrimitiveType.Bool, m.Not(qp), testbitF));
        }
        else
        {
            WriteOpIgnoreQp0(instr, 0, testbitT);
            WriteOpIgnoreQp0(instr, 1, testbitF);
        }
    }

    private void RewriteXma(IA64Instruction instr, BinaryOperator mul, int bitoffset)
    {
        var op1 = ReadOp(instr, 1);
        var op2 = ReadOp(instr, 2);
        var product = binder.CreateTemporary(PrimitiveType.Word128);
        var op3 = ReadOp(instr, 3);
        m.Assign(product, m.Bin(mul, product.DataType, op1, op2));
        m.Assign(product, m.IAdd(product, m.ExtendZ(op3, product.DataType)));
        WriteOp(instr, 0, m.Slice(product, op1.DataType, bitoffset));
    }

    private void RewriteXor(IA64Instruction instr)
    {
        var src1 = ReadOp(instr, 1);
        var src2 = ReadOp(instr, 2);
        // Swapping operands b/c src1 will be where immediates are provided.
        WriteOp(instr, 0, m.Xor(src2, src1));
    }

    private void RewriteZxt(IA64Instruction instr, PrimitiveType dt)
    {
        var src = ReadOp(instr, 1);
        var tmp = binder.CreateTemporary(dt);
        m.Assign(tmp, m.Slice(src, dt));
        WriteOp(instr, 0, m.ExtendZ(tmp, PrimitiveType.Word64));
    }
}