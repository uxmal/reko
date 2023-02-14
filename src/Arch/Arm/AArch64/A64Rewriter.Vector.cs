#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    if (instr.Operands[2] is not ImmediateOperand imm)
                    {
                        var arrayRight = MakeArrayType(instr.Operands[2], domain);
                        tmpRight = binder.CreateTemporary(arrayRight);
                    }
                    var dst = RewriteOp(instr.Operands[0]);
                    var dtDst = MakeOperandType(instr.Operands[0], domain);
                    var left = RewriteOp(instr.Operands[1], true);
                    var right = RewriteOp(instr.Operands[2], true);
                    intrinsic = intrinsic.MakeInstance(arrayLeft);
                    m.Assign(tmpLeft, left);
                    if (tmpRight != null)
                    {
                        m.Assign(tmpRight, right);
                    }
                    else
                    {
                        tmpRight = right;
                    }
                    m.Assign(dst, m.Fn(intrinsic, tmpLeft, tmpRight));
                    setFlags?.Invoke(dst);
                }
                else
                {
                    var op2 = RewriteOp(1);
                    var op3 = RewriteOp(2);
                    var op1 = RewriteOp(0);
                    var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                    intrinsic = intrinsic.MakeInstance(arrayLeft);
                    m.Assign(op1, m.Fn(intrinsic, op2, op3));
                }
            }
            else if (instr.Operands[0] is VectorRegisterOperand)
            {
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var arrayRight = MakeArrayType(instr.Operands[0], domain);
                var tmpRight = binder.CreateTemporary(arrayRight);
                var dst = RewriteOp(instr.Operands[0]);
                var dtDst = MakeOperandType(instr.Operands[0], domain);
                var left = RewriteOp(instr.Operands[0], true);
                var right = RewriteOp(instr.Operands[1], true);
                intrinsic = intrinsic.MakeInstance(arrayLeft);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                m.Assign(dst, m.Fn(intrinsic, tmpRight, tmpLeft));
                setFlags?.Invoke(dst);
            }
            else
            {
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var tmpRight = binder.CreateTemporary(arrayLeft);
                var dst = RewriteOp(instr.Operands[0]);
                var dtDst = MakeOperandType(instr.Operands[0], domain);
                var left = RewriteOp(instr.Operands[0], true);
                var right = RewriteOp(instr.Operands[1], true);
                intrinsic = intrinsic.MakeInstance(arrayLeft);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                m.Assign(dst, m.Fn(intrinsic, tmpRight, tmpLeft));
                setFlags?.Invoke(dst);
            }
        }

        private void RewriteSimdBinaryWiden(IntrinsicProcedure intrinsic, Domain domain = 0)
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var dst = this.RewriteOp(0);
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            intrinsic = intrinsic.MakeInstance(arraySrc, arrayDst);
            m.Assign(dst, m.Fn(intrinsic, src1, src2));
        }

        // E.g.  saddw v10.8h,v11.8h,v31.8b
        private void RewriteSimdBinaryWideNarrow(IntrinsicProcedure intrinsic, Domain domain = 0)
        {
            var src1 = this.RewriteOp(1);
            var src2 = this.RewriteOp(2);
            var dst = this.RewriteOp(0);
            var arrayWide = MakeArrayType(instr.Operands[1], domain);
            var arrayNarrow = MakeArrayType(instr.Operands[2], domain);
            intrinsic = intrinsic.MakeInstance(arrayWide, arrayNarrow);
            m.Assign(dst, m.Fn(intrinsic, src1, src2));
        }

        private void RewriteSimdTernary(Domain domain)
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

                var name = GenerateSimdIntrinsicName($"__{instr.Mnemonic}_{{0}}", (PrimitiveType) array1.ElementType);
                m.Assign(op1, host.Intrinsic(name, false, op1.DataType, tmp1, tmp2, tmp3));
            }
            else
            {
                var op1 = RewriteOp(instr.Operands[0]);
                var op2 = RewriteOp(instr.Operands[1]);
                var op3 = RewriteOp(instr.Operands[2]);

                m.Assign(op1, host.Intrinsic($"__{instr.Mnemonic}", false, op1.DataType, op1, op2, op3));
            }
        }

        private void RewriteSimdTernaryWithScalar(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src1 = RewriteOp(1, true);
            var src2 = RewriteOp(2, true);
            var src3 = RewriteOp(3, true);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(arraySrc), src1, src2, src3));
        }

        private void RewriteSimdBinaryWithScalar(IntrinsicProcedure intrinsic, Domain domain, Action<Expression>? setFlags = null)
        {
            var arrayLeft = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var left = RewriteOp(1, true);
            var right = RewriteOp(2, true);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(arrayLeft), left, right));
            setFlags?.Invoke(dst);
        }

        private void RewriteSimdUnary(IntrinsicProcedure simdGeneric, Domain domain)
        {
            var dtSrc = MakeArrayType(instr.Operands[1], domain);
            var tmpSrc = binder.CreateTemporary(dtSrc);
            var src = RewriteOp(1, true);
            var dst = RewriteOp(0);
            m.Assign(tmpSrc, src);
            m.Assign(dst, m.Fn(simdGeneric.MakeInstance(dtSrc), tmpSrc));
        }

        private void RewriteSimdUnaryChangeSize(IntrinsicProcedure intrinsic, Domain domain = Domain.None)
        {
            var arraySrc = MakeArrayType(instr.Operands[1], domain);
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src = RewriteOp(instr.Operands[1], true);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(arraySrc, arrayDst), src));
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
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(arraySrc, dst.DataType), tmpSrc));
        }

        private void RewriteSimdUnaryWithScalar(IntrinsicProcedure intrinsic, Domain domain)
        {
            var arrayDst = MakeArrayType(instr.Operands[0], domain);
            var src = RewriteOp(1, true);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(src.DataType, arrayDst), src));
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
            if (vop.Width.BitSize == 64)
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
            m.Assign(dst, m.Convert(src, src.DataType, dtDst));
        }

        private string GenerateSimdIntrinsicName(string simdFormat, PrimitiveType elementType)
        {
            string prefix = "i";
            if (elementType.Domain == Domain.Real)
            {
                prefix = "f";
            }
            else if (elementType.Domain == Domain.UnsignedInt)
            {
                prefix = "u";
            }
            return string.Format(simdFormat, $"{prefix}{elementType.BitSize}");
        }

        private void RewriteAddv()
        {
            RewriteSimdUnaryReduce(sum_intrinsic, Domain.Integer);
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
            m.Assign(dst, m.Xor(dst, m.And(m.Xor(dst, src2), src1)));
        }

        private void RewriteCm(IntrinsicProcedure intrinsic, Domain domain)
        {
            if (instr.Operands.Length == 2)
            {
                // Compare with implicit zero.
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var left = RewriteOp(instr.Operands[1], false);
                var right = Constant.Zero(instr.Operands[1].Width);
                var dst = RewriteOp(instr.Operands[0]);
                m.Assign(dst, m.Fn(intrinsic.MakeInstance(arrayLeft), left, right));
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
            var opDst = RewriteOp(0);
            var lsb = ((ImmediateOperand) instr.Operands[3]).Value;
            if (rHi == rLo) // ROR
            {
                var op = binder.EnsureRegister(rHi);
                m.Assign(opDst, m.Fn(CommonOps.Ror, op, lsb));
            }
            else
            {
                var seq = binder.EnsureSequence(PrimitiveType.Word128, rHi, rLo);
                m.Assign(opDst, m.Slice(seq, opDst.DataType, lsb.ToInt32()));
            }
        }

        private void RewriteLdN(string fnName)
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
                m.SideEffect(host.Intrinsic(fnName, true, VoidType.Instance, args.ToArray()));
            }
            else
            {
                NotImplementedYet();
            }
            if (postIndex != null)
            {
                m.Assign(baseReg!, postIndex);
            }
        }

        private void RewriteLdNr(string fnName)
        {
            var (ea, _) = RewriteEffectiveAddress((MemoryOperand)instr.Operands[1]);
            var vec = ((VectorMultipleRegisterOperand)instr.Operands[0]);
            var args = new List<Expression> { ea };
            args.AddRange(vec.GetRegisters()
                .Select(r => (Expression)m.Out(r.DataType, binder.EnsureRegister(r))));
            m.SideEffect(host.Intrinsic(fnName, true, VoidType.Instance, args.ToArray()));
        }

        private void RewriteSmsubl()
        {
            var m1 = RewriteOp(1);
            var m2 = RewriteOp(2);
            var s = RewriteOp(3);
            var product = m.IMul(m1, m2);
            product.DataType = s.DataType;
            var dst = RewriteOp(0);
            m.Assign(dst, m.ISub(s, product));
        }

        private void RewriteStN(string fnName)
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
                m.SideEffect(host.Intrinsic(fnName, true, VoidType.Instance, args.ToArray()));
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
            if (postIndex != null)
            {
                m.Assign(baseReg!, postIndex);
            }
        }

        private void RewriteMovi()
        {
            var vDst = (VectorRegisterOperand) instr.Operands[0];
            var eType = Bitsize(vDst.ElementType);
            var src = ReplicateSimdConstant(vDst, eType, 1);
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteIcvtf(string sSign, Domain intDomain)
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
                m.Assign(dst, m.Fn(cvtf_fixed_intrinsic.MakeInstance(dtSrc, dtDst), src, fprec));
            }
            else if (src is Identifier idSrc && Registers.IsIntegerRegister((RegisterStorage) idSrc.Storage))
            {
                var intType = PrimitiveType.Create(intDomain, srcBitSize);
                m.Assign(dst, m.Convert(src, intType, realType));
            }
            else if (instr.Operands[0] is VectorRegisterOperand vrop)
            {
                RewriteSimdUnary(cvtf_intrinsic, intDomain);
            }
            else
            {
                var intType = PrimitiveType.Create(intDomain, srcBitSize);
                m.Assign(dst, m.Convert(src, intType, realType));
            }
        }

        private void RewriteSha1c()
        {
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(__sha1c, op1, op2));
        }

        private void RewriteSqdmull(IntrinsicProcedure intrinsic)
        {
            var op1 = RewriteOp(1);
            var op2 = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic.MakeInstance(op1.DataType, dst.DataType), op1, op2));
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
            var dst = RewriteOp(0);
            var args = new List<Expression>();
            foreach (var reg in idxs.GetRegisters())
            {
                args.Add(binder.EnsureRegister(reg));
            }
            args.Add(tmpSrc);
            m.Assign(dst, host.Intrinsic($"__tbl_{idxs.Repeat}", true, arrayDst, args.ToArray()));
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
            var dst = RewriteOp(0);
            var args = new List<Expression>();
            foreach (var reg in idxs.GetRegisters())
            {
                args.Add(binder.EnsureRegister(reg));
            }
            args.Add(tmpSrc);
            m.Assign(dst, host.Intrinsic($"__tbx_{idxs.Repeat}", true, arrayDst, args.ToArray()));
        }


        private void RewriteUaddw()
        {
            if (instr.VectorData != VectorData.Invalid || instr.Operands[1] is VectorRegisterOperand)
            {
                var domain = Domain.UnsignedInt;
                var arrayLeft = MakeArrayType(instr.Operands[1], domain);
                var arrayRight = MakeArrayType(instr.Operands[2], domain);
                var arrayDst = MakeArrayType(instr.Operands[0], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var tmpRight = binder.CreateTemporary(arrayRight);
                var left = RewriteOp(instr.Operands[1], true);
                var right = RewriteOp(instr.Operands[2], true);
                var dst = RewriteOp(instr.Operands[0]);
                var name = GenerateSimdIntrinsicName("__uaddw_{0}", (PrimitiveType)arrayLeft.ElementType);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                m.Assign(dst, host.Intrinsic(name, true, arrayDst, tmpLeft, tmpRight));
            }
            else
            {
                NotImplementedYet();
            }
        }
    }
}
