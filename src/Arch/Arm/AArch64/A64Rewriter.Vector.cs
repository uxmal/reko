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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteSimdBinary(IntrinsicProcedure intrinsic, Domain domain, Action<Expression>? setFlags = null)
        {
            if (instr.Operands.Length == 3)
            {
                if (instr.Operands[0] is VectorRegisterOperand)
                {
                    var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                    var tmpLeft = binder.CreateTemporary(arrayLeft);
                    Expression? tmpRight = null;
                    if (instr.Operands[2] is not Constant imm)
                    {
                        var arrayRight = MakeArrayType(instr.Operands[2], domain);
                        tmpRight = binder.CreateTemporary(arrayRight);
                    }
                    var dtDst = MakeOperandType(instr.Operands[0], domain);
                    var left = RewriteOp(1, true);
                    var right = RewriteOp(2, true);
                    intrinsic = intrinsic.MakeInstance(arrayLeft);
                    m.Assign(tmpLeft, left);
                    if (tmpRight is not null)
                    {
                        m.Assign(tmpRight, right);
                    }
                    else
                    {
                        tmpRight = right;
                    }
                    var dst = AssignSimd(0, m.Fn(intrinsic, tmpLeft, tmpRight));
                    setFlags?.Invoke(dst);
                }
                else
                {
                    var op2 = RewriteOp(1);
                    var op3 = RewriteOp(2);
                    var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                    intrinsic = intrinsic.MakeInstance(arrayLeft);
                    AssignSimd(0, m.Fn(intrinsic, op2, op3));
                }
            }
            else if (instr.Operands[0] is VectorRegisterOperand)
            {
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var arrayRight = MakeArrayType(instr.Operands[0], domain);
                var tmpRight = binder.CreateTemporary(arrayRight);
                var dtDst = MakeOperandType(instr.Operands[0], domain);
                var left = RewriteOp(instr.Operands[0], true);
                var right = RewriteOp(instr.Operands[1], true);
                intrinsic = intrinsic.MakeInstance(arrayLeft);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                var dst = AssignSimd(0, m.Fn(intrinsic, tmpRight, tmpLeft));
                setFlags?.Invoke(dst);
            }
            else
            {
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var tmpRight = binder.CreateTemporary(arrayLeft);
                var dtDst = MakeOperandType(instr.Operands[0], domain);
                var left = RewriteOp(0, true);
                var right = RewriteOp(1, true);
                intrinsic = intrinsic.MakeInstance(arrayLeft);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                var dst = AssignSimd(0, m.Fn(intrinsic, tmpRight, tmpLeft));
                setFlags?.Invoke(dst);
            }
        }

        private void RewriteSimdBinaryWiden(IntrinsicProcedure intrinsic, Domain domain = 0)
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            intrinsic = intrinsic.MakeInstance(arraySrc, arrayDst);
            AssignSimd(0, m.Fn(intrinsic, src1, src2));
        }

        private void RewriteSimdBinaryNarrow(IntrinsicProcedure intrinsic, Domain domain = 0)
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            intrinsic = intrinsic.MakeInstance(arraySrc, arrayDst);
            AssignSimd(0, m.Fn(intrinsic, src1, src2));
        }

        // E.g.  saddw v10.8h,v11.8h,v31.8b
        private void RewriteSimdBinaryWideNarrow(IntrinsicProcedure intrinsic, Domain domain = 0)
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var arrayWide = MakeArrayType(instr.Operands[1], domain);
            var arrayNarrow = MakeArrayType(instr.Operands[2], domain);
            intrinsic = intrinsic.MakeInstance(arrayWide, arrayNarrow);
            AssignSimd(0, m.Fn(intrinsic, src1, src2));
        }

        private void RewriteSimdTernary(IntrinsicProcedure intrinsic, Domain domain)
        {
            // Ternary operations mutate the first argument.
            if (instr.Operands[0] is VectorRegisterOperand)
            {
                var array1 = MakeArrayType(instr.Operands[0], domain);
                var tmp1 = binder.CreateTemporary(array1);

                var array2 = MakeArrayType(instr.Operands[1], domain);
                var tmp2 = binder.CreateTemporary(array2);

                var array3 = MakeArrayType(instr.Operands[2], domain);
                var tmp3 = binder.CreateTemporary(array3);

                var op1 = RewriteOp(0);
                m.Assign(tmp1, op1);
                var op2 = RewriteOp(1);
                m.Assign(tmp2, op2);
                var op3 = RewriteOp(2);
                m.Assign(tmp3, op3);

                AssignSimd(0, m.Fn(intrinsic, tmp1, tmp2, tmp3));
            }
            else
            {
                var op1 = RewriteOp(0);
                var op2 = RewriteOp(1);
                var op3 = RewriteOp(2);

                AssignSimd(0, m.Fn(intrinsic, op1, op2, op3));
            }
        }

        private void RewriteSimdTernaryWiden(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src1 = RewriteOp(0, true);
            var src2 = RewriteOp(1, true);
            var src3 = RewriteOp(2, true);
            var dst = RewriteOp(0);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(arraySrc, arrayDst), src1, src2, src3));
        }

        private void RewriteSimdTernaryWithScalar(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src1 = RewriteOp(1, true);
            var src2 = RewriteOp(2, true);
            var src3 = RewriteOp(3, true);
            var dst = RewriteOp(0);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(arraySrc), src1, src2, src3));
        }

        private void RewriteSimdBinaryWithScalar(IntrinsicProcedure intrinsic, Domain domain, Action<Expression>? setFlags = null)
        {
            var arrayLeft = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var left = RewriteOp(1, true);
            var right = RewriteOp(2, true);
            var dst = AssignSimd(0, m.Fn(intrinsic.MakeInstance(arrayLeft), left, right));
            setFlags?.Invoke(dst);
        }

        private void RewriteSimdUnary(IntrinsicProcedure simdGeneric, Domain domain)
        {
            var dtSrc = MakeArrayType(instr.Operands[1], domain);
            var tmpSrc = binder.CreateTemporary(dtSrc);
            var src = RewriteOp(1, true);
            m.Assign(tmpSrc, src);
            AssignSimd(0, m.Fn(simdGeneric.MakeInstance(dtSrc), tmpSrc));
        }

        private void RewriteSimdUnaryChangeSize(IntrinsicProcedure intrinsic, Domain domain = Domain.None)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src = RewriteOp(1, true);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(arraySrc, arrayDst), src));
        }

        /// <summary>
        /// Rewrites an intrinsic that reduces a SIMD vector to a scalar.
        /// </summary>
        private void RewriteSimdUnaryReduce(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var tmpSrc = binder.CreateTemporary(arraySrc);
            var src = RewriteOp(1, true);
            var dst = RewriteOp(0);
            m.Assign(tmpSrc, src);
            var dtDst = PrimitiveType.Create(domain, dst.DataType.BitSize);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(arraySrc, dtDst), tmpSrc));
        }

        private void RewriteSimdUnaryWithScalar(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src = RewriteOp(1, true);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(src.DataType, arrayDst), src));
        }

        /// <summary>
        /// Handles umov and smov instructions.
        /// </summary>
        /// <param name="domain"></param>
        private void RewriteVectorElementToScalar(Domain domain)
        {
            var vop = (VectorRegisterOperand) instr.Operands[1];
            Debug.Assert(vop.Index >= 0);
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var tmpSrc = binder.CreateTemporary(arraySrc);
            Identifier vreg;
            if (vop.DataType.BitSize == 64)
            {
                vreg = binder.EnsureRegister(Registers.SimdRegs64[vop.VectorRegister.Number - 32]);
            }
            else
            {
                vreg = binder.EnsureRegister(Registers.SimdRegs128[vop.VectorRegister.Number - 32]);
            }
            m.Assign(tmpSrc, vreg);
            var src = m.ARef(arraySrc.ElementType, tmpSrc, Constant.Int32(vop.Index));
            var dst = RewriteOp(0);
            var dtDst = PrimitiveType.Create(domain, dst.DataType.BitSize);
            AssignSimd(0, m.Convert(src, src.DataType, dtDst));
        }

        private void RewriteAddhn2()
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var arraySrc = MakeArrayType(instr.Operands[1], 0);
            var arrayDst = MakeArrayType(instr.Operands[0], 0);
            arrayDst.Length = arraySrc.Length;
            var addhn2 = intrinsic.addhn2.MakeInstance(arraySrc, arrayDst);
            var tmp = binder.CreateTemporary(arrayDst);
            m.Assign(tmp, m.Fn(addhn2, src1, src2));
            var dst = this.RewriteOp(0);
            m.Assign(dst, m.Dpb(dst, tmp, tmp.DataType.BitSize));
        }

        private void RewriteAddp()
        {
            if (instr.Operands.Length == 2)
            {
                // Scalar
                var src = this.RewriteOp(1);
                var hi = binder.CreateTemporary(PrimitiveType.Word64);
                var lo = binder.CreateTemporary(PrimitiveType.Word64);
                m.Assign(hi, m.Slice(src, hi.DataType, 64));
                m.Assign(lo, m.Slice(src, lo.DataType, 0));
                AssignSimd(0, m.IAdd(hi, lo));
            }
            else
            {
                RewriteSimdBinary(intrinsic.addp, Domain.None);
            }
        }

        private void RewriteBi(IntrinsicProcedure intrinsic)
        {
            var op0 = RewriteOp(0);
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(op0.DataType), op0, op1, op2));
        }

        private void RewriteBsl()
        {
            // This instruction sets each bit in the destination register to
            // the corresponding bit from the first source register when the
            // original destination bit was 1, otherwise from the second
            // source register.
            var dst = this.RewriteOp(0);
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            AssignSimd(0, m.Xor(src2, m.And(m.Xor(src1, src2), dst)));
        }

        private void RewriteCm(IntrinsicProcedure intrinsic, Domain domain)
        {
            if (instr.Operands.Length == 2)
            {
                // Compare with implicit zero.
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var left = RewriteOp(instr.Operands[1], false);
                var right = Constant.Zero(instr.Operands[1].DataType);
                AssignSimd(0, m.Fn(intrinsic.MakeInstance(arrayLeft), left, right));
            }
            else if (instr.Operands[2] is VectorRegisterOperand)
            {
                RewriteSimdBinary(intrinsic, Domain.None);
            }
            else
            {
                RewriteSimdBinaryWithScalar(intrinsic, Domain.None);
            }
        }


        private void RewriteExtr()
        {
            var rHi = (RegisterStorage) instr.Operands[1];
            var rLo = (RegisterStorage) instr.Operands[1];
            var lsb = (Constant)instr.Operands[3];
            if (rHi == rLo) // ROR
            {
                var op = binder.EnsureRegister(rHi);
                AssignSimd(0, m.Fn(CommonOps.Ror, op, lsb));
            }
            else
            {
                var opDst = RewriteOp(0);
                var seq = binder.EnsureSequence(PrimitiveType.Word128, rHi, rLo);
                AssignSimd(0, m.Slice(seq, opDst.DataType, lsb.ToInt32()));
            }
        }

        private void RewriteLdN(IntrinsicProcedure intrinsic)
        {
            Expression? postIndex = null;
            var mem = (MemoryOperand) instr.Operands[1];
            var (ea, baseReg) = RewriteEffectiveAddress(mem);
            if (mem.PostIndex)
            {
                postIndex = ea;
                ea = baseReg;
            }
            var vec = ((VectorMultipleRegisterOperand)instr.Operands[0]);
            if (vec.Index < 0)
            {
                var args = new List<Expression> { ea };
                args.AddRange(vec.GetRegisters()
                    .Select(r => (Expression)m.Out(r.DataType, binder.EnsureRegister(r))));
                m.SideEffect(m.Fn(intrinsic.MakeInstance(64, args[1].DataType), args.ToArray()));
            }
            else
            {
                var args = new List<Expression> { ea };
                args.Add(Constant.Int32(vec.Index));
                args.AddRange(vec.GetRegisters()
                    .Select(r => (Expression) m.Out(r.DataType, binder.EnsureRegister(r))));
                m.SideEffect(m.Fn(intrinsic.MakeInstance(64, args[1].DataType), args.ToArray()));
            }
            if (postIndex is not null)
            {
                m.Assign(baseReg!, postIndex);
            }
        }

        private void RewriteLdNr(IntrinsicProcedure intrinsic)
        {
            var (ea, _) = RewriteEffectiveAddress((MemoryOperand)instr.Operands[1]);
            var vec = ((VectorMultipleRegisterOperand)instr.Operands[0]);
            var args = new List<Expression> { ea };
            args.AddRange(vec.GetRegisters()
                .Select(r => (Expression)m.Out(r.DataType, binder.EnsureRegister(r))));
            m.SideEffect(m.Fn(intrinsic.MakeInstance(64, args[1].DataType), args.ToArray()));
        }

        private void RewriteMla(IntrinsicProcedure intrinsic, IntrinsicProcedure intrinsicByElement, Domain domain)
        {
            if (instr.Operands[^1] is VectorRegisterOperand vr && vr.Index >= 0)
            {
                var dtSrc = this.MakeArrayType(instr.Operands[1]);
                AssignSimd(0, m.Fn(
                    intrinsicByElement.MakeInstance(dtSrc, dtSrc.ElementType),
                    RewriteOp(0),
                    RewriteOp(1),
                    RewriteOp(2)));
            }
            else
            {
                var dtSrc = this.MakeArrayType(instr.Operands[1]);
                AssignSimd(0, m.Fn(
                   intrinsic.MakeInstance(dtSrc),
                   RewriteOp(0),
                   RewriteOp(1),
                   RewriteOp(2)));
            }
        }

        private void RewriteSmsubl()
        {
            var m1 = RewriteOp(1);
            var m2 = RewriteOp(2);
            var s = RewriteOp(3);
            var product = m.IMul(m1, m2);
            product.DataType = s.DataType;
            AssignSimd(0, m.ISub(s, product));
        }

        private void RewriteStN(IntrinsicProcedure procedure)
        {
            Expression? postIndex = null;
            var mem = (MemoryOperand) instr.Operands[1];
            var (ea, baseReg) = RewriteEffectiveAddress(mem);
            if (mem.PostIndex)
            {
                postIndex = ea;
                ea = baseReg;
            }
            var vec = ((VectorMultipleRegisterOperand)instr.Operands[0]);
            if (vec.Index < 0)
            {
                var args = new List<Expression> { ea };
                args.AddRange(vec.GetRegisters()
                    .Select(r => (Expression)binder.EnsureRegister(r)));
                m.SideEffect(m.Fn(procedure.MakeInstance(64, args[1].DataType), args.ToArray()));
            }
            else
            {
                var dtElem = PrimitiveType.CreateWord(Bitsize(vec.ElementType));
                int offset = 0;
                foreach (var reg in vec.GetRegisters())
                {
                    var vReg = binder.EnsureRegister(reg);
                    var indexed = m.ARef(dtElem, vReg, Constant.Int32(vec.Index));
                    var eaOffset = offset == 0 ? ea : m.IAddS(ea, offset);
                    m.Assign(m.Mem(dtElem, eaOffset), indexed);
                    offset += dtElem.Size;
                }
            }
            if (postIndex is not null)
            {
                m.Assign(baseReg!, postIndex);
            }
        }

        private void RewriteMovi()
        {
            var vDst = (VectorRegisterOperand) instr.Operands[0];
            var eType = Bitsize(vDst.ElementType);
            var src = ReplicateSimdConstant(vDst, eType, 1);
            AssignSimd(0, src);
        }

        private void RewriteIcvtf(Domain intDomain)
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            int srcBitSize = src.DataType.BitSize;
            int dstBitSize = dst.DataType.BitSize;
            var realType = PrimitiveType.Create(Domain.Real, dstBitSize);
            if (instr.Operands.Length == 3)
            {
                // fixed point conversion.
                var dtSrc = MakeOperandType(instr.Operands[1], intDomain);
                var dtDst = MakeOperandType(instr.Operands[0], Domain.Real);
                var fprec = RewriteOp(instr.Operands[2]);
                AssignSimd(0, m.Fn(intrinsic.cvtf_fixed.MakeInstance(dtSrc, dtDst), src, fprec));
            }
            else if (src is Identifier idSrc && Registers.IsIntegerRegister((RegisterStorage) idSrc.Storage))
            {
                var intType = PrimitiveType.Create(intDomain, srcBitSize);
                AssignSimd(0, m.Convert(src, intType, realType));
            }
            else if (instr.Operands[0] is VectorRegisterOperand vrop)
            {
                RewriteSimdUnary(intrinsic.cvtf, intDomain);
            }
            else
            {
                var intType = PrimitiveType.Create(intDomain, srcBitSize);
                AssignSimd(0, m.Convert(src, intType, realType));
            }
        }

        private void RewriteSha1c()
        {
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);
            AssignSimd(0, m.Fn(intrinsic.sha1c, op1, op2));
        }

        private void RewriteSqdmull(IntrinsicProcedure intrinsic)
        {
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);
            AssignSimd(0, m.Fn(intrinsic.MakeInstance(op1.DataType, dst.DataType), op1, op2));
        }

        private void RewriteTbl()
        {
            var arraySrc = MakeArrayType(instr.Operands[2], Domain.None);
            var tmpSrc = binder.CreateTemporary(arraySrc);
            var arrayDst = MakeArrayType(instr.Operands[0], Domain.None);
            var idxs = (VectorMultipleRegisterOperand) instr.Operands[1];
            int n = idxs.Repeat;
            var src = RewriteOp(2);
            m.Assign(tmpSrc, src);
            var args = new List<Expression>();
            args.Add(Constant.Int32(n));
            foreach (var reg in idxs.GetRegisters())
            {
                args.Add(binder.EnsureRegister(reg));
            }
            args.Add(tmpSrc);
            AssignSimd(0, m.FnVariadic(intrinsic.tbl.MakeInstance(arrayDst), args.ToArray()));
        }

        private void RewriteTbx()
        {
            var arraySrc = MakeArrayType(instr.Operands[2], Domain.None);
            var tmpSrc = binder.CreateTemporary(arraySrc);
            var arrayDst = MakeArrayType(instr.Operands[0], Domain.None);
            var idxs = (VectorMultipleRegisterOperand) instr.Operands[1];
            int n = idxs.Repeat;
            var src = RewriteOp(2);
            m.Assign(tmpSrc, src);
            var args = new List<Expression>();
            args.Add(Constant.Int32(n));
            foreach (var reg in idxs.GetRegisters())
            {
                args.Add(binder.EnsureRegister(reg));
            }
            args.Add(tmpSrc);
            AssignSimd(0, m.FnVariadic(intrinsic.tbx.MakeInstance(arrayDst), args.ToArray()));
        }
    }
}
