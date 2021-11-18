#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Arch.X86
{
    public partial class X86Rewriter
    {
        public void RewriteAndnp_(string fnName)
        {
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            m.Assign(dst, host.Intrinsic(fnName, false, dst.DataType, dst, src));
        }

        public void RewriteAndps()
        {
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            m.Assign(dst, host.Intrinsic("__andps", false, dst.DataType, dst, src));
        }

        private void RewriteCmpp(bool isVex, string name, PrimitiveType element)
        {
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            RewritePackedTernaryop(
                isVex,
                name,
                element,
                CreatePackedArrayType(
                    PrimitiveType.CreateWord(element.BitSize),
                    dst.DataType));
        }

        private void RewriteComis(PrimitiveType size)
        {
            var grf = binder.EnsureFlagGroup(Registers.CZP);
            m.Assign(grf, m.Cond(m.FSub(
                MaybeSlice(size, SrcOp(0)),
                MaybeSlice(size, SrcOp(1)))));
            m.Assign(binder.EnsureFlagGroup(Registers.O), Constant.False());
            m.Assign(binder.EnsureFlagGroup(Registers.S), Constant.False());
        }


        private void RewriteCmpsd(PrimitiveType size)
        {
            var fourOps = instrCur.Operands.Length == 4;
            var op1 = MaybeSlice(size, SrcOp(fourOps ? 1 : 0));
            var op2 = MaybeSlice(size, SrcOp(fourOps ? 2 : 1));
            var opc = ((ImmediateOperand) instrCur.Operands[fourOps ? 3 : 2]).Value.ToUInt32();
            var dst = SrcOp(0);
            Expression cmp;
            switch (opc)
            {
            case 0: cmp = m.FEq(op1, op2); break;
            case 1: cmp = m.FLt(op1, op2); break;
            case 2: cmp = m.FLe(op1, op2); break;
            case 3: cmp = host.Intrinsic("isunordered", false, PrimitiveType.Bool, op1, op2); break;
            case 4: cmp = m.FNe(op1, op2); break;
            case 5: cmp = m.FGe(op1, op2); break;
            case 6: cmp = m.FGt(op1, op2); break;
            case 7: cmp = m.Not(host.Intrinsic("isunordered", false, PrimitiveType.Bool, op1, op2)); break;
            default: EmitUnitTest(); iclass = InstrClass.Invalid; m.Invalid(); return;
            }
            m.Assign(dst, m.Conditional(
                dst.DataType, 
                cmp,
                Constant.Create(dst.DataType, -1), 
                Constant.Zero(dst.DataType)));
        }

        private void RewriteCvtPackedToReal(PrimitiveType type)
        {
            var dtSrc = PrimitiveType.Int32;
            var dtDst = PrimitiveType.Real32;
            var src = SrcOp(1);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, m.Convert(m.Slice(dtSrc, src, 0), dtSrc, dtDst));

            var tmp2 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp2, m.Convert(m.Slice(dtSrc, src, 32), dtSrc, dtDst));

            m.Assign(SrcOp(0), m.Seq(tmp2, tmp1));
        }

        private void RewriteCvts2si(PrimitiveType floatType)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            if (src.DataType.BitSize != floatType.BitSize)
            {
                src = m.Slice(floatType, src, 0);
            }
            m.Assign(SrcOp(0), m.Convert(src, floatType, instrCur.Operands[0].Width));
        }

        private void RewriteCvtts2si(PrimitiveType floatType)
        {
            instrCur.Operands[0].Width = PrimitiveType.Create(Domain.SignedInt, instrCur.Operands[0].Width.BitSize);
            var src = MaybeSlice(floatType, SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]));
            m.Assign(SrcOp(0), m.Convert(src, floatType, instrCur.Operands[0].Width));
        }

        private void RewriteCvtToReal(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var src = MaybeSlice(dtFrom, SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]));
            var dst = (Identifier) SrcOp(0);
            var tmp = binder.CreateTemporary(dtTo);
            m.Assign(tmp, m.Convert(src, dtFrom, dtTo));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteCvtIntToReal(PrimitiveType dtDst)
        {
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            var dst = (Identifier) SrcOp(0);
            var tmp = binder.CreateTemporary(dtDst);
            var dtSrc = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(tmp, m.Convert(src, dtSrc, dtDst));
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
                SrcOp(0),
                host.Intrinsic(fnName, true, dtDst, tmp1));
        }

        private void RewriteCvttps2pi(string fnName, DataType dtSrcElem, DataType dtDstElem)
        {
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);
            var dtSrc = CreatePackedArrayType(dtSrcElem, src.DataType);
            var dtDst = new ArrayType(dtDstElem, dtSrc.Length);

            var tmp1 = binder.CreateTemporary(dtSrc);
            m.Assign(tmp1, src);

            m.Assign(
                SrcOp(0),
                host.Intrinsic(fnName, true, dtDst, tmp1));
        }

        private void RewriteCvttps2pi()
        {
            var dtSrc = PrimitiveType.Real32;
            var dtDst = PrimitiveType.Int32;
            var src = SrcOp(instrCur.Operands[instrCur.Operands.Length == 3 ? 2 : 1]);

            var tmp1 = binder.CreateTemporary(dtDst);
            m.Assign(tmp1, m.Convert(m.Slice(dtSrc, src, 0), dtSrc, dtDst));

            var tmp2 = binder.CreateTemporary(dtDst);
            m.Assign(tmp2, m.Convert(m.Slice(dtSrc, src, 32), dtSrc, dtDst));

            m.Assign(SrcOp(0), m.Seq(tmp2, tmp1));
        }

        private void RewriteFemms()
        {
            m.SideEffect(host.Intrinsic("__femms", true, VoidType.Instance));
        }

        private void RewriteLdmxcsr()
        {
            var src = SrcOp(0);
            var dst = binder.EnsureRegister(Registers.mxcsr);
            m.Assign(dst, src);
        }

        private void RewriteMaskmov(bool isVex, string fnName)
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            VexAssign(isVex, dst,
                host.Intrinsic(fnName, true, dst.DataType,
                    dst, src));
        }

        private void RewriteMaxMinsd(string fnName, PrimitiveType size, bool zeroExtend)
        {
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = MaybeSlice(size, SrcOp(1));
                src2 = MaybeSlice(size, SrcOp(2));
            }
            else
            {
                src1 = MaybeSlice(size, SrcOp(0));
                src2 = MaybeSlice(size, SrcOp(1));
            }
            VexAssign(zeroExtend, SrcOp(0), host.Intrinsic(fnName, true, size, src1, src2));
        }

        private void RewriteMovlps()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(2)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(0)));
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(3)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(1)));
        }

        private void RewriteMovlhps()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(2)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(0)));
            m.Assign(
                m.Array(PrimitiveType.Real32, dst, Constant.Int32(3)),
                m.Array(PrimitiveType.Real32, src, Constant.Int32(1)));
        }

        private void RewriteMovmsk(bool isVex, string fnName, PrimitiveType elemType)
        {
            var srcType = CreatePackedArrayType(elemType, instrCur.Operands[1].Width);
            var src = SrcOp(1, srcType);
            var dst = (Identifier) SrcOp(0);
            var ret = binder.CreateTemporary(PrimitiveType.Byte);
            m.Assign(ret, host.Intrinsic(fnName, true, ret.DataType, src));
            m.Assign(dst, m.Dpb(dst, ret, 0));
        }

        private void RewriteOrps()
        {
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            m.Assign(dst, host.Intrinsic("__orps", false, dst.DataType, dst, src));
        }

        private void RewritePavg(string fnName, PrimitiveType elementType)
        {
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            var src = SrcOp(1, arrayType);
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src);
            m.Assign(dst, host.Intrinsic(fnName, false, arrayType, tmp1));
        }

        private void RewritePbroadcast(bool isVex, string fname, PrimitiveType elementType)
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            var tmp = binder.CreateTemporary(arrayType);
            VexAssign(isVex, dst, host.Intrinsic(fname, false, arrayType, src));
        }

        private void RewritePcmp(string fnName, PrimitiveType elementType)
        {
            var dst = SrcOp(0);
            ArrayType srcType = CreatePackedArrayType(elementType, dst.DataType);
            ArrayType dstType = new ArrayType(
                PrimitiveType.CreateWord(srcType.ElementType.BitSize), 
                srcType.Length);
            var src1 = SrcOp(0, srcType);
            var src2 = SrcOp(1, srcType);
            var tmp1 = binder.CreateTemporary(srcType);
            var tmp2 = binder.CreateTemporary(srcType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            m.Assign(dst, host.Intrinsic(fnName, false, dstType, tmp1, tmp2));
        }

        private void RewritePextrw()
        {
            var src1 = SrcOp(1);
            var src2 = SrcOp(2);
            var dst = SrcOp(0);
            m.Assign(
                dst,
                host.Intrinsic("__pextrw", false, dst.DataType, dst, src1, src2));
        }

        private void RewritePinsr(bool isVex, string fnName, PrimitiveType dtElem)
        {
            Expression dst;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                dst = SrcOp(0);
                src1 = SrcOp(1);
                src2 = SrcOp(2);
            }
            else
            {
                dst = SrcOp(0);
                src1 = SrcOp(0);
                src2 = SrcOp(1);
            }
            VexAssign(
                isVex,
                dst,
                host.Intrinsic(fnName, false, dst.DataType, dst, src1, src2));
        }

        private void RewritePackedLogical(string intrinsicName)
        {
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            m.Assign(dst, host.Intrinsic(intrinsicName, false, dst.DataType, dst, src));
        }

        private void RewritePor()
        {
            if (instrCur.Operands.Length > 2)
            {
                // DEST←SRC1 OR SRC2
                m.Assign(SrcOp(0), m.Or(SrcOp(1), SrcOp(2)));
            }
            else
            {
                // DEST←DEST OR SRC
                m.Assign(SrcOp(0), m.Or(SrcOp(0), SrcOp(1)));
            }
        }


        private void RewritePshuf(string intrinsicName, PrimitiveType dt)
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    intrinsicName,
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1),
                    SrcOp(2)));
        }

        private void RewritePunpckhbw()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpckhbw",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpckhdq()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpckhdq",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpckhwd()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpckhwd",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpcklbw()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpcklbw",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpckldq()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpckldq",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpcklqdq()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpcklqdq",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePunpcklwd()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__punpcklwd",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1)));
        }

        private void RewritePalignr()
        {
            m.Assign(
                SrcOp(0),
                host.Intrinsic(
                    "__palignr",
                    false,
                    instrCur.Operands[0].Width,
                    SrcOp(0),
                    SrcOp(1),
                    SrcOp(2)));
        }

        private static readonly string[] roundingIntrinsics32 =
        {
            "roundf", "floorf", "ceilf", "truncf"
        };
        private static readonly string[] roundingIntrinsics64 =
        {
            "round", "floor", "ceil", "trunc"
        };

        private void RewriteRoundsx(bool isVex, PrimitiveType dt)
        {
            var mode = ((Constant) SrcOp(isVex ? 3 : 2)).ToInt32() & 0b11;
            string intrinsic = dt.BitSize == 32
                ? roundingIntrinsics32[mode]
                : roundingIntrinsics64[mode];
            var src = SrcOp(1);
            var dst = SrcOp(0);
            VexAssign(isVex, dst, host.Intrinsic(intrinsic, true, dt, m.Slice(dt, src, 0)));
        }

        private void RewriteScalarBinop(Func<Expression, Expression, Expression> fn, PrimitiveType size, bool zeroExtend)
        {
            var dst = SrcOp(0);
            var tmp = binder.CreateTemporary(size);
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(1);
                src2 = SrcOp(2);
            }
            else
            {
                src1 = dst;
                src2 = SrcOp(1);
            }
            if (src1.DataType.BitSize != size.BitSize)
                src1 = m.Slice(size, src1, 0);
            if (src2.DataType.BitSize != size.BitSize)
                src2 = m.Slice(size, src2, 0);
            m.Assign(tmp, fn(src1, src2));

            //$REVIEW: this does a DPB-ish operation.
            var highBits = dst.DataType.BitSize - size.BitSize;
            if (highBits > 0)
            {
                var dtHighPart = PrimitiveType.CreateWord(highBits);
                Expression hi;
                if (zeroExtend)
                {
                    hi = Constant.Zero(dtHighPart);
                }
                else
                {
                    hi = binder.CreateTemporary(dtHighPart);
                    m.Assign(hi, m.Slice(dtHighPart, dst, size.BitSize));
                }
                m.Assign(dst, m.Seq(hi, tmp));
            }
            else
            {
                m.Assign(dst, tmp);
            }
        }

        private void RewriteSha1msg2()
        {
            var src = SrcOp(1);
            var dst = SrcOp(0);
            var tmpSrc = binder.CreateTemporary(PrimitiveType.Word128);
            var tmpDst = binder.CreateTemporary(PrimitiveType.Word128);
            m.Assign(tmpSrc, src);
            m.Assign(tmpDst, dst);
            m.Assign(dst, host.Intrinsic("__sha1msg2", true, PrimitiveType.Word128, tmpDst, tmpSrc));
        }


        private void RewriteSqrtsd(string fnName, PrimitiveType dt)
        {
            var src = SrcOp(1);
            var dst = (Identifier) SrcOp(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, host.Intrinsic(fnName, true, dt, src));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteStmxcsr()
        {
            var src = binder.EnsureRegister(Registers.mxcsr);
            var dst = SrcOp(0);
            m.Assign(dst, src);
        }

        private void RewritePackedBinop(bool isVex, string fnName, PrimitiveType elementType, DataType? dstType = null)
        {
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType is null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(1);
                src2 = SrcOp(2);
            }
            else
            {
                src1 = SrcOp(0);
                src2 = SrcOp(1);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            VexAssign(isVex, dst, host.Intrinsic(fnName, false, arrayType, tmp1, tmp2));
        }

        private void RewritePmaddUbSw(bool isVex, string fnName)
        {
            var src1 = SrcOp(0);
            var dtSrc1 = CreatePackedArrayType(PrimitiveType.UInt8, src1.DataType);
            var src2 = SrcOp(1);
            var dtSrc2 = CreatePackedArrayType(PrimitiveType.Int8, src1.DataType);
            var tmp1 = binder.CreateTemporary(dtSrc1);
            var tmp2 = binder.CreateTemporary(dtSrc2);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            var dst = SrcOp(0);
            var dtDst = CreatePackedArrayType(PrimitiveType.Int16, dst.DataType);
            VexAssign(isVex, dst, host.Intrinsic(fnName, false, dtDst, tmp1, tmp2));
        }

        private void RewritePackedShift(string fnName, PrimitiveType elementType, DataType? dstType = null)
        {
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType is null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(1);
                src2 = SrcOp(2);
            }
            else
            {
                src1 = SrcOp(0);
                src2 = SrcOp(1);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(dst, host.Intrinsic(fnName, false, arrayType, tmp1, src2));
        }

        private void RewritePackedTernaryop(bool isVex, string fnName, PrimitiveType elementType, DataType? dstType = null)
        {
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType is null)
                dstType = arrayType;
            Expression src1;
            Expression src2;
            Expression src3;
            if (instrCur.Operands.Length == 3)
            {
                src1 = SrcOp(0);
                src2 = SrcOp(1);
                src3 = SrcOp(2);
            }
            else
            {
                src1 = SrcOp(0);
                src2 = src1;
                src3 = SrcOp(1);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            var tmp2 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(tmp2, src2);
            VexAssign(isVex, dst, host.Intrinsic(fnName, false, arrayType, tmp1, tmp2, src3));
        }

        private void RewritePackedUnaryop(string fnName, PrimitiveType elementType, DataType? dstType = null)
        {
            var dst = SrcOp(0);
            ArrayType arrayType = CreatePackedArrayType(elementType, dst.DataType);
            if (dstType == null)
                dstType = arrayType;
            Expression src1;
            if (instrCur.Operands.Length > 1)
            {
                src1 = SrcOp(1);
            }
            else
            {
                src1 = SrcOp(0);
            }
            var tmp1 = binder.CreateTemporary(arrayType);
            m.Assign(tmp1, src1);
            m.Assign(dst, host.Intrinsic(fnName, false, arrayType, tmp1));
        }

        private ArrayType CreatePackedArrayType(DataType elementType, DataType dtArrayType)
        {
            int celem = dtArrayType.Size / elementType.Size;
            var arrayType = new ArrayType(elementType, celem);
            return arrayType;
        }

        private void RewritePsrldq(bool isVex)
        {
            bool has3Ops = instrCur.Operands.Length == 3;
            var src1 = SrcOp(has3Ops ? 1 : 0);
            var src2 = SrcOp(has3Ops ? 2 : 1);
            var dst = SrcOp(0);
            var tmp = binder.CreateTemporary(PrimitiveType.Word128);
            m.Assign(tmp, m.Slice(tmp.DataType, src1, 0));  // Low 128 bits
            VexAssign(isVex, dst, tmp);
        }

        public void RewritePxor()
        {
            if (instrCur.Operands[0] is RegisterOperand rdst &&
                instrCur.Operands[1] is RegisterOperand rsrc &&
                rdst.Register.Number == rsrc.Register.Number)
            { // selfie!
                m.Assign(orw.AluRegister(rdst), Constant.Zero(rdst.Width));
                return;
            }
            var dst = this.SrcOp(0);
            var src = this.SrcOp(1);
            m.Assign(dst, host.Intrinsic("__pxor", false, dst.DataType, dst, src));
        }

        private void RewriteUnpckhps()
        {
            var bitsize = instrCur.Operands[0].Width.BitSize;
            RewritePackedBinop(false, $"__unpckhps{bitsize}", PrimitiveType.Real32);
        }

        private void RewriteAesenc(bool isVex)
        {
            var state = SrcOp(isVex ? 1 : 0);
            var roundKey = SrcOp(isVex ? 2 : 1);
            var newState = SrcOp(0);
            VexAssign(isVex, newState, host.Intrinsic("__aesenc", true, newState.DataType, state, roundKey));
        }

        private void RewriteVZeroUpper()
        {
            int nregs = arch.ProcessorMode.WordWidth.BitSize == 64 ? 16 : 8;
            for (int i = 0; i < nregs; ++i)
            {
                var idNarrow = binder.EnsureRegister(Registers.XmmRegisters[i]);
                var idWide = binder.EnsureRegister(Registers.YmmRegisters[i]);
                m.Assign(idWide, m.Convert(idNarrow, idNarrow.DataType, idWide.DataType));
            }
        }
    }
}
