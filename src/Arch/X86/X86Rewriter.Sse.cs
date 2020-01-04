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
        public void RewriteAndnps()
        {
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__andnps", dst.DataType, dst, src));
        }

        public void RewriteAndps()
        {
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__andps", dst.DataType, dst, src));
        }

        private void RewriteCmpp(string name, PrimitiveType element)
        {
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            RewritePackedTernaryop(
                name,
                element,
                CreatePackedArrayType(
                    PrimitiveType.CreateWord(element.BitSize),
                    dst.DataType));
        }

        private void RewriteComis(PrimitiveType size)
        {
            var grf = binder.EnsureFlagGroup(arch.GetFlagGroup("ZCP"));
            m.Assign(grf, m.Cond(m.FSub(
                m.Cast(size, SrcOp(instrCur.Operands[0])),
                SrcOp(instrCur.Operands[1]))));
            m.Assign(orw.FlagGroup(FlagM.OF), Constant.False());
            m.Assign(orw.FlagGroup(FlagM.SF), Constant.False());
        }

        private void RewriteCvtPackedToReal(PrimitiveType type)
        {
            var dtSrc = PrimitiveType.Int32;
            var dtDst = PrimitiveType.Real32;
            var src = SrcOp(instrCur.Operands[1]);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, m.Cast(dtDst, m.Slice(dtSrc, src, 0)));

            var tmp2 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp2, m.Cast(dtDst, m.Slice(dtSrc, src, 32)));

            m.Assign(SrcOp(instrCur.Operands[0]), m.Seq(tmp2, tmp1));
        }


        private void RewriteCvts2si(PrimitiveType floatType)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            m.Assign(SrcOp(instrCur.Operands[0]), m.Cast(instrCur.Operands[0].Width, src));
        }

        private void RewriteCvtts2si(PrimitiveType floatType)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            m.Assign(SrcOp(instrCur.Operands[0]), m.Cast(instrCur.Operands[0].Width, src));
        }

        private void RewriteCvtToReal(PrimitiveType size)
        {
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            var dst = SrcOp(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(size);
            m.Assign(tmp, m.Cast(size, src));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteCvtps2pi(string fnName, DataType dtSrcElem, DataType dtDstElem)
        {
            var src = SrcOp(instrCur.Operands.Length == 3
                ? instrCur.Operands[2] 
                : instrCur.Operands[1]);
            var dtSrc = CreatePackedArrayType(dtSrcElem, src.DataType);
            var dtDst = new ArrayType(dtDstElem, dtSrc.Length);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, src);

            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(fnName, dtDst, tmp1));
        }

        private void RewriteCvttps2pi(string fnName, DataType dtSrcElem, DataType dtDstElem)
        {
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            var dtSrc = CreatePackedArrayType(dtSrcElem, src.DataType);
            var dtDst = new ArrayType(dtDstElem, dtSrc.Length);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, src);

            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(fnName, dtDst, 
                    host.PseudoProcedure("trunc", tmp1.DataType, tmp1)));
        }

        private void RewriteCvttps2pi()
        {
            var dtSrc = PrimitiveType.Real32;
            var dtDst = PrimitiveType.Int32;
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);

            var tmp1 = binder.CreateTemporary(dtDst);
            m.Assign(tmp1, m.Cast(dtDst, m.Slice(dtSrc, src, 0)));

            var tmp2 = binder.CreateTemporary(dtDst);
            m.Assign(tmp2, m.Cast(dtDst, m.Slice(dtSrc, src, 32)));

            m.Assign(SrcOp(instrCur.Operands[0]), m.Seq(tmp2, tmp1));
        }

        private void RewriteLdmxcsr()
        {
            var src = SrcOp(instrCur.Operands[0]);
            var dst = binder.EnsureRegister(Registers.mxcsr);
            m.Assign(dst, src);
        }

        private void RewriteMaskmovq()
        {
            var src = SrcOp(instrCur.Operands[1]);
            var dst = SrcOp(instrCur.Operands[0]);
            m.Assign(dst,
                host.PseudoProcedure("__maskmovq", dst.DataType,
                    dst, src));
        }

        private void RewriteMovlps()
        {
            var src = SrcOp(instrCur.Operands[1]);
            var dst = SrcOp(instrCur.Operands[0]);
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(2)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(0)));
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(3)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(1)));
        }

        private void RewriteMovlhps()
        {
            var src = SrcOp(instrCur.Operands[1]);
            var dst = SrcOp(instrCur.Operands[0]);
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(2)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(0)));
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(3)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(1)));
        }

        private void RewriteMovmsk(string fnName, PrimitiveType elemType)
        {
            var srcType = CreatePackedArrayType(elemType, instrCur.Operands[1].Width);
            var src = SrcOp(instrCur.Operands[1], srcType);
            var dst = SrcOp(instrCur.Operands[0]);
            var ret = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(ret, host.PseudoProcedure(fnName, ret.DataType, src));
            m.Assign(dst, m.Dpb(dst, ret, 0));
        }

        private void RewriteOrps()
        {
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__orps", dst.DataType, dst, src));
        }

        private void RewritePavg(string fnName, PrimitiveType elementType)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            var src = SrcOp(instrCur.Operands[1], arrayType);
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1));
        }

        private void RewritePcmp(string fnName, PrimitiveType elementType)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType srcType = CreatePackedArrayType(elementType, dst.DataType);
            ArrayType dstType = new ArrayType(
                PrimitiveType.CreateWord(srcType.ElementType.BitSize), 
                srcType.Length);
            var src1 = SrcOp(instrCur.Operands[0], srcType);
            var src2 = SrcOp(instrCur.Operands[1], srcType);
            var tmp1 = binder.CreateTemporary(srcType);
            var tmp2 = binder.CreateTemporary(srcType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            m.Assign(dst, host.PseudoProcedure(fnName, dstType, tmp1, tmp2));
        }

        private void RewritePextrw()
        {
            var dst = SrcOp(instrCur.Operands[0]);
            var src1 = SrcOp(instrCur.Operands[1]);
            var src2 = SrcOp(instrCur.Operands[2]);
            m.Assign(
                dst,
                host.PseudoProcedure("__pextrw", dst.DataType, dst, src1, src2));
        }

        private void RewritePinsrw()
        {
            Expression dst;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                dst = SrcOp(instrCur.Operands[0]);
                src1 = SrcOp(instrCur.Operands[1]);
                src2 = SrcOp(instrCur.Operands[2]);
            }
            else
            {
                dst = SrcOp(instrCur.Operands[0]);
                src1 = SrcOp(instrCur.Operands[0]);
                src2 = SrcOp(instrCur.Operands[1]);
            }
            m.Assign(
                dst,
                host.PseudoProcedure("__pinsrw", dst.DataType, dst, src1, src2));
        }

        private void RewritePackedLogical(string intrinsicName)
        {
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            m.Assign(dst, host.PseudoProcedure(intrinsicName, dst.DataType, dst, src));
        }

        private void RewritePshuf(string intrinsicName, PrimitiveType dt)
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    intrinsicName,
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1]),
                    SrcOp(instrCur.Operands[2])));
        }

        private void RewritePunpckhbw()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpckhbw",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewritePunpckhdq()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpckhdq",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }


        private void RewritePunpckhwd()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpckhwd",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewritePunpcklbw()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpcklbw",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewritePunpckldq()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpckldq",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewritePunpcklwd()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__punpcklwd",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1])));
        }

        private void RewritePalignr()
        {
            m.Assign(
                SrcOp(instrCur.Operands[0]),
                host.PseudoProcedure(
                    "__palignr",
                    instrCur.Operands[0].Width,
                    SrcOp(instrCur.Operands[0]),
                    SrcOp(instrCur.Operands[1]),
                    SrcOp(instrCur.Operands[2])));
        }

        private void RewriteScalarBinop(Func<Expression, Expression, Expression> fn, PrimitiveType size)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(size);
            if (instrCur.Operands.Length == 3)
            {
                var src1 = SrcOp(instrCur.Operands[1]);
                var src2 = SrcOp(instrCur.Operands[2]);
                src1 = m.Cast(size, src1);
                src2 = m.Cast(size, src2);
                m.Assign(tmp, fn(src1, src2));
                m.Assign(dst, m.Dpb(dst, tmp, 0));
            }
            else
            {
                m.Assign(tmp, fn(m.Cast(size, dst), SrcOp(instrCur.Operands[1])));
                m.Assign(dst, m.Dpb(dst, tmp, 0));
            }
        }

        private void RewriteSha1msg2()
        {
            var src = SrcOp(instrCur.Operands[1]);
            var dst = SrcOp(instrCur.Operands[0]);
            var tmpSrc = binder.CreateTemporary(PrimitiveType.Word128);
            var tmpDst = binder.CreateTemporary(PrimitiveType.Word128);
            m.Assign(tmpSrc, src);
            m.Assign(tmpDst, dst);
            m.Assign(dst, host.PseudoProcedure("__sha1msg2", PrimitiveType.Word128, tmpDst, tmpSrc));
        }


        private void RewriteSqrtsd()
        {
            var src = SrcOp(instrCur.Operands[1]);
            var dst = SrcOp(instrCur.Operands[0]);
            var tmp = binder.CreateTemporary(PrimitiveType.Real64);
            m.Assign(tmp, host.PseudoProcedure("__sqrt", PrimitiveType.Real64, src));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteStmxcsr()
        {
            var src = binder.EnsureRegister(Registers.mxcsr);
            var dst = SrcOp(instrCur.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewritePackedBinop(string fnName, PrimitiveType elementType, DataType dstType = null)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType == null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(instrCur.Operands[1]);
                src2 = SrcOp(instrCur.Operands[2]);
            }
            else
            {
                src1 = SrcOp(instrCur.Operands[0]);
                src2 = SrcOp(instrCur.Operands[1]);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1, tmp2));
        }

        private void RewritePackedShift(string fnName, PrimitiveType elementType, DataType dstType = null)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType == null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(instrCur.Operands[1]);
                src2 = SrcOp(instrCur.Operands[2]);
            }
            else
            {
                src1 = SrcOp(instrCur.Operands[0]);
                src2 = SrcOp(instrCur.Operands[1]);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1, src2));
        }

        private void RewritePackedTernaryop(string fnName, PrimitiveType elementType, DataType dstType = null)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType == null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            Expression src3;
            {
                src1 = SrcOp(instrCur.Operands[0]);
                src2 = SrcOp(instrCur.Operands[1]);
                src3 = SrcOp(instrCur.Operands[2]);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1, tmp2, src3));
        }

        private void RewritePackedUnaryop(string fnName, PrimitiveType elementType, DataType dstType = null)
        {
            var dst = SrcOp(instrCur.Operands[0]);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType == null)
                dstType = arrayType;
            Expression src1;
            if (instrCur.Operands.Length > 1)
            {
                src1 = SrcOp(instrCur.Operands[1]);
            }
            else
            {
                src1 = SrcOp(instrCur.Operands[0]);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(dst, host.PseudoProcedure(fnName, arrayType, tmp1));
        }

        private ArrayType CreatePackedArrayType(DataType elementType, DataType dtArrayType)
        {
            int celem = dtArrayType.Size / elementType.Size;
            var arrayType = new ArrayType(elementType, celem);
            return arrayType;
        }

        public void RewritePxor()
        {
            var rdst = instrCur.Operands[0] as RegisterOperand;
            var rsrc = instrCur.Operands[1] as RegisterOperand;
            if (rdst != null && rsrc != null && rdst.Register.Number == rsrc.Register.Number)
            { // selfie!
                m.Assign(orw.AluRegister(rdst), m.Cast(rdst.Width, Constant.Int32(0)));
                return;
            }
            var dst = this.SrcOp(instrCur.Operands[0]);
            var src = this.SrcOp(instrCur.Operands[1]);
            m.Assign(dst, host.PseudoProcedure("__pxor", dst.DataType, dst, src));
        }
    }
}
