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
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    public partial class X86Rewriter
    {
        private void RewriteComis(PrimitiveType size)
        {
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ZCP"));
            m.Assign(grf, m.Cond(m.FSub(
                m.Cast(size, SrcOp(instrCur.op1)),
                SrcOp(instrCur.op2))));
        }

        private void RewriteCvtPackedToReal(PrimitiveType type)
        {
            var dtSrc = PrimitiveType.Int32;
            var dtDst = PrimitiveType.Real32;
            var src = SrcOp(instrCur.op2);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, m.Cast(dtDst, m.Slice(dtSrc, src, 0)));

            var tmp2 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp2, m.Cast(dtDst, m.Slice(dtSrc, src, 32)));

            m.Assign(SrcOp(instrCur.op1), m.Seq(tmp2, tmp1));
        }


        private void RewriteCvtts2si(PrimitiveType floatType)
        {
            instrCur.op1.Width = PrimitiveType.Create(Domain.SignedInt, instrCur.op1.Width.Size);
            var src = SrcOp(instrCur.op3 ?? instrCur.op2);
            m.Assign(SrcOp(instrCur.op1), m.Cast(instrCur.op1.Width, src));
        }

        private void RewriteCvtToReal(PrimitiveType size)
        {
            var src = SrcOp(instrCur.op3 ?? instrCur.op2);
            var dst = SrcOp(instrCur.op1);
            var tmp = binder.CreateTemporary(size);
            m.Assign(tmp, m.Cast(size, src));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteCvttps2pi()
        {
            var dtSrc = PrimitiveType.Real32;
            var dtDst = PrimitiveType.Int32;
            var src = SrcOp(instrCur.op3 ?? instrCur.op2);

            var tmp1 = binder.CreateTemporary(dtDst);
            m.Assign(tmp1, m.Cast(dtDst, m.Slice(dtSrc, src, 0)));

            var tmp2 = binder.CreateTemporary(dtDst);
            m.Assign(tmp2, m.Cast(dtDst, m.Slice(dtSrc, src, 32)));

            m.Assign(SrcOp(instrCur.op1), m.Seq(tmp2, tmp1));
        }

        private void RewriteMovlhps()
        {
            var src = SrcOp(instrCur.op2);
            var dst = SrcOp(instrCur.op1);
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(2)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(0)));
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(3)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(1)));
        }

        private void RewritePcmpeqb()
        {
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__pcmpeqb",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2)));
        }

        private void RewritePshufd()
        {
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__pshufd",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2),
                    SrcOp(instrCur.op3)));
        }

        private void RewritePunpcklbw()
        {
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__punpcklbw",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2)));
        }

        private void RewritePunpcklwd()
        {
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__punpcklwd",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2)));
        }

        private void RewritePalignr()
        {
            m.Assign(
                SrcOp(instrCur.op1),
                host.PseudoProcedure(
                    "__palignr",
                    instrCur.op1.Width,
                    SrcOp(instrCur.op1),
                    SrcOp(instrCur.op2),
                    SrcOp(instrCur.op3)));
        }

        private void RewriteScalarBinop(Func<Expression, Expression, Expression> fn, PrimitiveType size)
        {
            var dst = SrcOp(instrCur.op1);
            var tmp = binder.CreateTemporary(size);
            if (instrCur.op3 != null)
            {
                var src1 = SrcOp(instrCur.op2);
                var src2 = SrcOp(instrCur.op3);
                src1 = m.Cast(size, src1);
                src2 = m.Cast(size, src2);
                m.Assign(tmp, fn(src1, src2));
                m.Assign(dst, m.Dpb(dst, tmp, 0));
            }
            else
            {
                m.Assign(tmp, fn(m.Cast(size, dst), SrcOp(instrCur.op2)));
                m.Assign(dst, m.Dpb(dst, tmp, 0));
            }
        }

        private void RewritePackedBinop(string fnName, PrimitiveType elementType)
        {
            var dst = SrcOp(instrCur.op1);
            int celem = dst.DataType.Size / elementType.Size;
            var arrayType = new ArrayType(elementType, celem);
            Expression src1;
            Expression src2;
            if (instrCur.op3 != null)
            {
                src1 = SrcOp(instrCur.op2);
                src2 = SrcOp(instrCur.op3);
            }
            else
            {
                src1 = SrcOp(instrCur.op1);
                src2 = SrcOp(instrCur.op2);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1, tmp2));
        }
    }
}
