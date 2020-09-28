#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private Expression Convert(DataType dtDst, DataType dtSrc, Expression src)
        {
            return m.Cast(dtDst, m.Cast(dtSrc, src));
        }

        private static PrimitiveType MakeReal(DataType dt)
        {
            return PrimitiveType.Create(Domain.Real, dt.BitSize);
        }

        private static PrimitiveType MakeInteger(Domain domain, DataType dt)
        {
            return PrimitiveType.Create(domain, dt.BitSize);
        }

        private ArrayType MakeArrayType(MachineOperand mop, Domain domain = 0)
        {
            var arrayBitsize = mop.Width.BitSize;
            var elemBitsize = Bitsize((mop is VectorRegisterOperand vector)
                ? vector.ElementType
                : instr.vectorData);
            var celem = arrayBitsize / elemBitsize;
            DataType elemType;
            if (domain == 0)
                elemType = PrimitiveType.CreateWord(elemBitsize);
            else
                elemType = PrimitiveType.Create(domain, elemBitsize);
            return new ArrayType(elemType, celem);
        }

        private static int Bitsize(VectorData data)
        {
            switch (data)
            {
            case VectorData.I8: return 8;
            case VectorData.I16: return 16;
            case VectorData.I32: return 32;
            case VectorData.I64: return 64;
            case VectorData.F16: return 16;
            case VectorData.F32: return 32;
            case VectorData.F64: return 64;
            }
            return 0;
        }

        private void RewriteFabs()
        {
            //$TODO: #include <math.h>
            var dst = RewriteOp(instr.Operands[0]);
            var src = RewriteOp(instr.Operands[1]);
            var dtSrc = MakeReal(src.DataType);
            var dtDst = MakeReal(dst.DataType);
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, src);
            var fn = dtSrc.BitSize == 32 ? "fabsf" : "fabs";
            m.Assign(dst, host.PseudoProcedure(fn, dtDst, tmp));

        }

        private void RewriteFadd()
        {
            RewriteMaybeSimdBinary(m.FAdd, "__fadd_{0}", Domain.Real);
        }

        private void RewriteFcmp()
        {
            var left = RewriteOp(instr.Operands[0]);
            var right = RewriteOp(instr.Operands[1]);
            NZCV(m.Cond(m.FSub(left, right)));
        }

        private void RewriteFcsel()
        {
            var eTrue = RewriteOp(instr.Operands[1]);
            var eFalse = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var cond = ((ConditionOperand)instr.Operands[3]).Condition;
            m.Assign(dst, m.Conditional(dst.DataType, TestCond(cond), eTrue, eFalse));
        }

        private void RewriteFcvt()
        {
            if (instr.Operands[0] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            var dst = RewriteOp(instr.Operands[0]);
            var src = RewriteOp(instr.Operands[1]);
            var dtDst = MakeReal(dst.DataType);
            var dtSrc = MakeReal(src.DataType);
            m.Assign(dst, Convert(dtDst, dtSrc, src));
        }

        private Expression RewriteFcvt(Expression src, Domain domain, string f32name, string f64name)
        {
            //$TODO: #include <math.h>
            var dtSrc = MakeReal(src.DataType);
            var dtDst = MakeInteger(domain, instr.Operands[0].Width);
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, src);
            var fn = dtSrc.BitSize == 32 ? f32name : f64name;
            return m.Cast(dtDst, host.PseudoProcedure(fn, dtDst, tmp));
        }

        private void RewriteFcvtms()
        {
            RewriteMaybeSimdUnary(
                n => RewriteFcvt(n, Domain.SignedInt, "floorf", "floor"),
                "__floor_{0}",
                Domain.Real);
        }

        private void RewriteFcvtps()
        {
            RewriteMaybeSimdUnary(
                n => RewriteFcvt(n, Domain.SignedInt, "ceilf", "ceil"),
                "__floor_{0}", Domain.Real);
        }

        private void RewriteFcvtzs()
        {
            RewriteMaybeSimdUnary(
                n => RewriteFcvt(n, Domain.SignedInt, "truncf", "trunc"),
                "__trunc_{0}", Domain.Real);
        }

        private void RewriteIntrinsicFBinary(string name32, string name64)
        {
            RewriteMaybeSimdBinary(
                (a,b) =>
                {
                    DataType dt;
                    string fname;
                    if (instr.Operands[0].Width.BitSize == 64)
                    {
                        dt = PrimitiveType.Real64;
                        fname = name64;
                    }
                    else
                    {
                        dt = PrimitiveType.Real32;
                        fname = name32;
                    }
                    return host.PseudoProcedure(fname, dt, a, b);
                },
                "__max_{0}", Domain.Real);
        }

        private void RewriteIntrinsicFTernary(string name32, string name64)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var src3 = RewriteOp(instr.Operands[3]);
            var dst = RewriteOp(instr.Operands[0]);
            DataType dt;
            string fname;
            if (instr.Operands[0].Width.BitSize == 64)
            {
                dt = PrimitiveType.Real64;
                fname = name64;
            }
            else
            {
                dt = PrimitiveType.Real32;
                fname = name32;
            }
            m.Assign(dst, host.PseudoProcedure(fname, dt, src1, src2, src3));

        }
        private void RewriteFmov()
        {
            RewriteMaybeSimdUnary(n => n, "__fmov_{0}", Domain.Real);
        }

        private void RewriteFmul()
        {
            RewriteMaybeSimdBinary(m.FMul, "__fmul_{0}", Domain.Real);
        }

        private void RewriteFnmul()
        {
            RewriteBinary((a, b) => m.FNeg(m.FMul(a, b)));
        }

        private void RewriteFsqrt()
        {
            //$TODO: require "<math.h>"
            var src = binder.CreateTemporary(MakeReal(instr.Operands[1].Width));
            var dst = RewriteOp(instr.Operands[0]);
            m.Assign(src, RewriteOp(instr.Operands[1]));
            var fn = src.DataType.BitSize == 32 ? "sqrtf" : "sqrt";
            m.Assign(dst, host.PseudoProcedure(fn, src.DataType, src));
        }

        private void RewriteIcvt(Domain domain)
        {
            if (instr.Operands[0] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            else
            {
                var dst = RewriteOp(instr.Operands[0]);
                var src = RewriteOp(instr.Operands[1]);
                var dtSrc = MakeInteger(domain, src.DataType);
                var dtDst = MakeReal(dst.DataType);
                m.Assign(dst, Convert(dtDst, dtSrc, src));
            }
        }
    }
}
