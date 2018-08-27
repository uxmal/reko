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

using Reko.Core.Expressions;
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

        private static ArrayType MakeArrayType(VectorRegisterOperand vector, Domain domain = 0)
        {
            var arrayBitsize = vector.Width.BitSize;
            var elemBitsize = Bitsize(vector.ElementType);
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
            Debug.Assert(false, "Impossiburu");
            return 0;
        }

        private void RewriteFabs()
        {
            //$TODO: #include <math.h>
            var dst = RewriteOp(instr.ops[0]);
            var src = RewriteOp(instr.ops[1]);
            var dtSrc = MakeReal(src.DataType);
            var dtDst = MakeReal(dst.DataType);
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, src);
            var fn = dtSrc.BitSize == 32 ? "fabsf" : "fabs";
            m.Assign(dst, host.PseudoProcedure(fn, dtDst, tmp));

        }

        private void RewriteFadd()
        {
            if (instr.ops[1] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            else
            {
                RewriteBinary(m.FAdd);
            }
        }

        private void RewriteFcmp()
        {
            var left = RewriteOp(instr.ops[0]);
            var right = RewriteOp(instr.ops[1]);
            NZCV(m.Cond(m.FSub(left, right)));
        }

        private void RewriteFcvt()
        {
            if (instr.ops[0] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            var dst = RewriteOp(instr.ops[0]);
            var src = RewriteOp(instr.ops[1]);
            var dtDst = MakeReal(dst.DataType);
            var dtSrc = MakeReal(src.DataType);
            m.Assign(dst, Convert(dtDst, dtSrc, src));
        }

        private void RewriteFcvt(Domain domain, string f32name, string f64name)
        {
            if (instr.ops[0] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            //$TODO: #include <math.h>
            var dst = RewriteOp(instr.ops[0]);
            var src = RewriteOp(instr.ops[1]);
            var dtSrc = MakeReal(src.DataType);
            var dtDst = MakeInteger(domain, dst.DataType);
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, src);
            var fn = dtSrc.BitSize == 32 ? f32name : f64name;
            m.Assign(dst, host.PseudoProcedure(fn, dtDst, tmp));
        }

        private void RewriteFmov()
        {
            var dst = RewriteOp(instr.ops[0]);
            var src = RewriteOp(instr.ops[1], true);
            m.Assign(dst, src);
        }

        private void RewriteFmul()
        {
            if (instr.ops[1] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            else
            {
                RewriteBinary(m.FMul);
            }
        }

        private void RewriteFsqrt()
        {
            //$TODO: require "<math.h>"
            var src = binder.CreateTemporary(MakeReal(instr.ops[1].Width));
            var dst = RewriteOp(instr.ops[0]);
            m.Assign(src, RewriteOp(instr.ops[1]));
            var fn = src.DataType.BitSize == 32 ? "sqrtf" : "sqrt";
            m.Assign(dst, host.PseudoProcedure(fn, src.DataType, src));
        }

        private void RewriteIcvt(Domain domain)
        {
            if (instr.ops[0] is VectorRegisterOperand)
            {
                throw new NotImplementedException();
            }
            else
            {
                var dst = RewriteOp(instr.ops[0]);
                var src = RewriteOp(instr.ops[1]);
                var dtSrc = MakeInteger(domain, src.DataType);
                var dtDst = MakeReal(dst.DataType);
                m.Assign(dst, Convert(dtDst, dtSrc, src));
            }
        }
    }
}
