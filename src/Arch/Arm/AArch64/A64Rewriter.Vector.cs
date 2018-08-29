#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteSimdBinary(string simdFormat, Domain domain, Action<Expression> setFlags = null)
        {
            var arrayLeft = MakeArrayType(instr.ops[1], domain);
            var arrayRight = MakeArrayType(instr.ops[2], domain);
            var arrayDst = MakeArrayType(instr.ops[0], domain);
            var tmpLeft = binder.CreateTemporary(arrayLeft);
            var tmpRight = binder.CreateTemporary(arrayRight);
            var left = RewriteOp(instr.ops[1], true);
            var right = RewriteOp(instr.ops[2], true);
            var dst = RewriteOp(instr.ops[0]);
            var name = GenerateSimdIntrinsicName(simdFormat, (PrimitiveType)arrayLeft.ElementType);
            m.Assign(tmpLeft, left);
            m.Assign(tmpRight, right);
            m.Assign(dst, host.PseudoProcedure(name, arrayDst, tmpLeft, tmpRight));
            setFlags?.Invoke(dst);
        }

        private void RewriteSimdUnary(string simdFormat, Domain domain)
        {
            var array = MakeArrayType(instr.ops[0], domain);
            var tmpSrc = binder.CreateTemporary(array);
            var src = RewriteOp(instr.ops[1], true);
            var dst = RewriteOp(instr.ops[0]);
            var name = GenerateSimdIntrinsicName(simdFormat, (PrimitiveType)array.ElementType);
            m.Assign(tmpSrc, src);
            m.Assign(dst, host.PseudoProcedure(name, array, tmpSrc));
        }

        private void RewriteSimdExpand(string simdFormat, Domain domain = Domain.None)
        {
            var arrayDst = MakeArrayType(instr.ops[0], domain);
            var tmpSrc = binder.CreateTemporary(arrayDst);
            var src = RewriteOp(instr.ops[1], true);
            var dst = RewriteOp(instr.ops[0]);
            var name = GenerateSimdIntrinsicName(simdFormat, (PrimitiveType)arrayDst.ElementType);
            m.Assign(tmpSrc, src);
            m.Assign(dst, host.PseudoProcedure(name, arrayDst.ElementType, tmpSrc));
        }

        private void RewriteSimdReduce(string simdFormat, Domain domain)
        {
            var arraySrc = MakeArrayType(instr.ops[1], domain);
            var tmpSrc = binder.CreateTemporary(arraySrc);
            var src = RewriteOp(instr.ops[1], true);
            var dst = RewriteOp(instr.ops[0]);
            var name = GenerateSimdIntrinsicName(simdFormat, (PrimitiveType)arraySrc.ElementType);
            m.Assign(tmpSrc, src);
            m.Assign(dst, host.PseudoProcedure(name, arraySrc.ElementType, tmpSrc));
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
            RewriteSimdReduce("__sum_{0}", Domain.Integer);
        }

        private void RewriteDup()
        {
            RewriteSimdExpand("__dup_{0}");
        }

        private void RewriteLd3()
        {
            var (ea,_) = RewriteEffectiveAddress((MemoryOperand)instr.ops[1]);
            var args = new List<Expression> { ea };
            args.AddRange(((VectorMultipleRegisterOperand)instr.ops[0])
                .GetRegisters()
                .Select(r => (Expression)m.Out(r.DataType, binder.EnsureRegister(r))));
            m.SideEffect(host.PseudoProcedure("__ld3", VoidType.Instance, args.ToArray()));
        }

        private void RewriteMovi()
        {
            RewriteSimdExpand("__movi_{0}");
        }

        private void RewriteScvtf()
        {
            var srcReg = ((RegisterOperand)instr.ops[1]).Register;
            var dstReg = ((RegisterOperand)instr.ops[0]).Register;
            var src = binder.EnsureRegister(srcReg);
            var dst = binder.EnsureRegister(dstReg);
            var realType = PrimitiveType.Create(Domain.Real, (int)dstReg.BitSize);
            if (instr.ops.Length == 3)
            {
                // fixed point conversion.
                var fprec = RewriteOp(instr.ops[2]);
                m.Assign(dst, host.PseudoProcedure("__scvtf_fixed", realType, src, fprec));
            }
            else if (Registers.IsIntegerRegister(srcReg))
            {
                var intType = PrimitiveType.Create(Domain.SignedInt, (int)srcReg.BitSize);
                m.Assign(dst, m.Cast(realType, m.Cast(intType, src)));
            }
            else if (instr.vectorData == VectorData.Invalid)
            {
                var intType = PrimitiveType.Create(Domain.SignedInt, (int)srcReg.BitSize);
                m.Assign(dst, m.Cast(realType, m.Cast(intType, src)));
            }
            else
            {
                RewriteSimdUnary("__scvtf_{0}", Domain.SignedInt);
            }
        }

        private void RewriteSmax()
        {
            RewriteSimdBinary("__smax_{0}", Domain.SignedInt);
        }

        private void RewriteSmaxv()
        {
            RewriteSimdReduce("__smax_{0}", Domain.SignedInt);
        }

        private void RewriteUaddw()
        {
            if (instr.vectorData != VectorData.Invalid || instr.ops[1] is VectorRegisterOperand)
            {
                var domain = Domain.UnsignedInt;
                var arrayLeft = MakeArrayType(instr.ops[1], domain);
                var arrayRight = MakeArrayType(instr.ops[2], domain);
                var arrayDst = MakeArrayType(instr.ops[0], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var tmpRight = binder.CreateTemporary(arrayRight);
                var left = RewriteOp(instr.ops[1], true);
                var right = RewriteOp(instr.ops[2], true);
                var dst = RewriteOp(instr.ops[0]);
                var name = GenerateSimdIntrinsicName("__uaddw_{0}", (PrimitiveType)arrayLeft.ElementType);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                m.Assign(dst, host.PseudoProcedure(name, arrayDst, tmpLeft, tmpRight));
            }
            else
            {
                NotImplementedYet();
            }

        }

        private void RewriteUmlal()
        {
            if (instr.vectorData != VectorData.Invalid || instr.ops[1] is VectorRegisterOperand)
            {
                var domain = Domain.UnsignedInt;
                var arrayLeft = MakeArrayType(instr.ops[1], domain);
                var arrayRight = MakeArrayType(instr.ops[2], domain);
                var arrayDst = MakeArrayType(instr.ops[0], domain);
                var tmpLeft = binder.CreateTemporary(arrayLeft);
                var tmpRight = binder.CreateTemporary(arrayRight);
                var left = RewriteOp(instr.ops[1], true);
                var right = RewriteOp(instr.ops[2], true);
                var dst = RewriteOp(instr.ops[0]);
                var name = GenerateSimdIntrinsicName("__umlal_{0}", (PrimitiveType)arrayLeft.ElementType);
                m.Assign(tmpLeft, left);
                m.Assign(tmpRight, right);
                m.Assign(dst, host.PseudoProcedure(name, arrayDst, tmpLeft, tmpRight, dst));
            }
            else
            {
                NotImplementedYet();
            }
        }
    }
}
