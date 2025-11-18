#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it  /or modify
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
using Reko.Core.Types;
using System;

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteAdd(PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            RewriteAdd(dst, src1, src2, dtDst);
        }

        private void RewriteAdd(Expression dst, Expression src1, Expression src2, PrimitiveType? dtDst = null)
        { 
            Expression src;
            if (src1.IsZero)
            {
               src = src2;
            }
            else if (src2.IsZero)
            {
                src = src1;
            }
            else
            {
                src = m.IAdd(src1, src2);
            }
            MaybeSliceSignExtend(dst, src, dtDst);
        }

        private void RewriteAddi16sp()
        {
            var dst = binder.EnsureRegister(arch.StackRegister);
            var imm = ((Constant) instr.Operands[1]).ToInt32();
            m.Assign(dst, m.IAddS(dst, imm));
        }

        private void RewriteAddi4spn()
        {
            var src = binder.EnsureRegister(arch.StackRegister);
            var imm = ((Constant) instr.Operands[2]).ToInt32();
            var dst = RewriteOp(0);
            m.Assign(dst, m.IAddS(src, imm));
        }

        private void RewriteAddUw()
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            var tmp1 = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp1, m.Slice(src2, tmp1.DataType));
            var tmp2 = binder.CreateTemporary(src2.DataType);
            m.Assign(tmp2, m.ExtendZ(tmp1, tmp2.DataType));
            m.Assign(dst, m.IAdd(src1, tmp2));
        }

        /// <summary>
        /// Rewriters an instruction that operates on 32-bit operands in a 64-bit 
        /// processor.
        /// </summary>
        private void RewriteBinopW(BinaryOperator op, int iopLeft, int iopRight)
        {
            Expression src1;
            var reg1 = (RegisterStorage) instr.Operands[iopLeft];
            if (reg1.Number == 0)
            {
                src1 = m.Word32(0);
            }
            else
            {
                src1 = m.Slice(binder.EnsureRegister(reg1), PrimitiveType.Word32);
            }
            var tmp1 = binder.CreateTemporary(src1.DataType);
            m.Assign(tmp1, src1);
            src1 = tmp1;

            Expression src2;
            if (instr.Operands[iopRight] is Constant imm2)
            {
                var c = imm2.ToInt32();
                if (c < 0)
                {
                    op = Operator.ISub;
                    c = -c;
                }
                src2 = m.Int32(c);
            }
            else
            {
                src2 = m.Slice(RewriteOp(iopRight), PrimitiveType.Word32);
                var tmp2 = binder.CreateTemporary(src2.DataType);
                m.Assign(tmp2, src2);
                src2 = tmp2;
            }


            Expression src;
            if (src1.IsZero)
            {
                src = src2;
            }
            else if (src2.IsZero)
            {
                src = src1;
            }
            else
            {
                src = m.Bin(op, src1, src2);
            }
            var dst = RewriteOp(0);
            m.Assign(dst, m.ExtendS(src, PrimitiveType.Int64));
        }

        private void RewriteAtomicMemoryOperation(IntrinsicProcedure intrinsic, PrimitiveType dt)
        {
            var src1 = RewriteOp(1);
            var src2 = m.AddrOf(arch.PointerType, RewriteOp(2));
            var dst = RewriteOp(0);
            MaybeSignExtend(dst, m.Fn(intrinsic.MakeInstance(arch.PointerType.BitSize, dt), src1, src2));
        }

        private void RewriteBinaryIntrinsic(IntrinsicProcedure intrinsic)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(intrinsic, src1, src2));
        }

        private void RewriteBinOp(BinaryOperator op, PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            MaybeSliceSignExtend(dst, m.Bin(op, src1, src2), dtDst);
        }

        private void RewriteBinNotOp(BinaryOperator op, PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            MaybeSliceSignExtend(dst, m.Bin(op, src1, m.Comp(src2)), dtDst);
        }

        private void RewriteCompressedBinOp(BinaryOperator op, PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var dst = RewriteOp(0);
            var val = m.Bin(op, dst, src1);
            MaybeSliceSignExtend(dst, val, dtDst);
        }

        private void RewriteCompressedBinOp(Func<Expression,Expression,Expression> op, PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var dst = RewriteOp(0);
            var val = op(dst, src1);
            MaybeSliceSignExtend(dst, val, dtDst);
        }

        private void RewriteGenericBinaryIntrinsic(IntrinsicProcedure fn)
        {
            var dt = arch.WordWidth;
            var src1 = MaybeSlice(RewriteOp(1), dt);
            var src2 = MaybeSlice(RewriteOp(2), dt);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(fn.MakeInstance(dt), src1, src2));
        }

        private void RewriteGenericUnaryIntrinsic(IntrinsicProcedure fn)
        {
            var dt = arch.WordWidth;
            var src = MaybeSlice(RewriteOp(1), dt);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Fn(fn.MakeInstance(dt), src));
        }


        private void RewriteLi()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            m.Assign(dst, src);
        }

        private void RewriteLoad(DataType dtSrc, DataType dtDst)
        {
            Expression ea;
            if (instr.Operands[1] is MemoryOperand mem)
            {
                var baseReg = binder.EnsureRegister(mem.Base);
                ea = baseReg;
                int offset = OffsetOf(mem);
                if (offset != 0)
                {
                    ea = m.IAddS(ea, offset);
                }
            }
            else
            {
                var baseReg = RewriteOp(1);
                var offset = (Constant)instr.Operands[2];
                ea = baseReg;
                if (!offset.IsZero)
                {
                    ea = m.IAdd(ea, offset);
                }
            }
            Expression src = m.Mem(dtSrc, ea);
            var dst = RewriteOp(0);
            if (dst.DataType.Size != src.DataType.Size)
            {
                src = m.Convert(src, src.DataType, dtDst);
            }
            m.Assign(dst, src);
        }

        private int OffsetOf(MemoryOperand mem)
        {
            Constant offset;
            if (mem.Offset is SliceOperand slice)
            {
                offset = slice.Value;
            }
            else
            {
                offset = (Constant) mem.Offset;
            }
            return offset.ToInt32();
        }

        private void RewriteLoadReserved(DataType dt)
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            var dtPtr = arch.PointerType;
            MaybeSignExtend(
                dst,
                m.Fn(
                    lr_intrinsic.MakeInstance(dtPtr.BitSize, dt),
                    m.AddrOf(dtPtr, src)));
        }

        private void RewriteLui()
        {
            var dst = RewriteOp(0);
            var ui = (Constant)instr.Operands[1];
            m.Assign(dst, Constant.Word(dst.DataType.BitSize, ui.ToUInt32() << 12));
        }

        private void RewriteMove()
        {
            var src = RewriteOp(1);
            var dst = RewriteOp(0);
            m.Assign(dst, src);
        }

        private void RewriteMulh(BinaryOperator mulOperator, DataType dt)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Bin(mulOperator, tmp.DataType, left, right));
            m.Assign(dst, m.Slice(tmp, dst.DataType, tmp.DataType.BitSize - dst.DataType.BitSize));
        }

        private void RewriteOr()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = left;
            if (!right.IsZero)
            {
                src = m.Or(src, right);
            }
            m.Assign(dst, src);
        }

        private void RewritePackw()
        {
            var slice1 = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(slice1, m.Slice(RewriteOp(1), slice1.DataType)); ;
            var slice2 = binder.CreateTemporary(PrimitiveType.Word16);
            m.Assign(slice2, m.Slice(RewriteOp(1), slice2.DataType));
            var result = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(result, m.Fn(
                pack_intrinsic.MakeInstance(slice1.DataType),
                slice1, 
                slice2));
            var dst = RewriteOp(0);
            m.Assign(dst, m.ExtendS(result, dst.DataType));
        }

        private void RewriteRotate(IntrinsicProcedure rotation)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = m.Fn(rotation, left, right);
            m.Assign(dst, src);
        }

        private void RewriteRotateW(IntrinsicProcedure rotation)
        {
            var slice = binder.CreateTemporary(PrimitiveType.Word32);
            var right = RewriteOp(2);
            m.Assign(slice, m.Slice(RewriteOp(1), PrimitiveType.Word32));
            var rot = binder.CreateTemporary(slice.DataType);
            m.Assign(rot, m.Fn(rotation, slice, right));
            var dst = RewriteOp(0);
            m.Assign(dst, m.ExtendZ(rot, dst.DataType));
        }

        private void RewriteSext(PrimitiveType dtFrom)
        {
            var slice = binder.CreateTemporary(dtFrom);
            m.Assign(slice, m.Slice(RewriteOp(1), dtFrom));
            var dst = RewriteOp(0);
            m.Assign(dst, m.ExtendS(slice, dst.DataType));
        }

        private void RewriteShadd(int shift)
        {
            var rs1 = RewriteOp(1);
            var rs2 = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.IAdd(rs2, m.IMul(rs1, 1<<shift)));
        }

        private void RewriteShaddUw(int shift)
        {
            var rs1 = RewriteOp(1);
            var rs2 = RewriteOp(2);
            var dst = RewriteOp(0);
            var slice = binder.CreateTemporary(PrimitiveType.Word32);
            var index = binder.CreateTemporary(rs2.DataType);
            m.Assign(slice, m.Slice(rs1, slice.DataType));
            m.Assign(index, m.ExtendZ(slice, index.DataType));
            m.Assign(dst, m.IAdd(rs2, m.IMul(index, 1<<shift)));
        }

        private void RewriteShift(BinaryOperator op)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Bin(op, left, right));
        }

        private void RewriteShiftUw(BinaryOperator op)
        {
            var left = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(left, m.Slice(RewriteOp(1), PrimitiveType.Word32));
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = m.Bin(op, m.ExtendZ(left, dst.DataType), right);
            m.Assign(dst, src);
        }

        private void RewriteShiftw(Func<Expression,Expression,Expression> op)
        {
            var left = m.Slice(RewriteOp(1), PrimitiveType.Word32);
            var right = RewriteOp(2);
            var src = m.Convert(op(left, right), PrimitiveType.Word32, PrimitiveType.Int64);
            var dst = RewriteOp(0);
            m.Assign(dst, src);
        }

        private void RewriteSlt(bool unsigned)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            Expression src;
            if (unsigned)
            {
                if (left.IsZero)
                {
                    src = m.Ne0(right);
                }
                else
                {
                    src = m.Ult(left, right);
                }
            }
            else
            {
                src = m.Lt(left, right);
            }
            m.Assign(dst, m.Convert(src, src.DataType, dst.DataType));
        }

        private void RewriteSlti(bool unsigned)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            Expression src;
            if (unsigned)
            {
                src = m.Ult(left, right);
            }
            else
            {
                src = m.Lt(left, right);
            }
            m.Assign(dst, m.Convert(src, src.DataType, dst.DataType));
        }

        private void RewriteStore(DataType dt)
        {
            var src = RewriteOp(0);
            if (instr.Operands[1] is MemoryOperand mem)
            {
                int offset = OffsetOf(mem);
                RewriteStore(dt, binder.EnsureRegister(mem.Base), offset, src);
            }
            else
            {
                var baseReg = RewriteOp(1);
                var offset = ((Constant)instr.Operands[2]).ToInt32();
                RewriteStore(dt, baseReg, offset, src);
            }
        }

        private void RewriteStoreConditional(DataType dt)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            MaybeSignExtend(
                dst,
                m.Fn(
                    sc_intrinsic.MakeInstance(arch.PointerType.BitSize, dt),
                    src1,
                    m.AddrOf(arch.PointerType, src2)));
        }

        private void RewriteStore(DataType dt, Expression baseReg, int offset, Expression src)
        {
            Expression ea = baseReg;
            if (offset != 0)
            {
                ea = m.IAddS(ea, offset);
            }
            if (src.DataType.BitSize > dt.BitSize)
            {
                src = m.Slice(src, dt);
            }
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteSub()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.ISub(left, right));
        }

        private void RewriteSubw()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = left;
            if (!right.IsZero)
            {
                src = m.ISub(src, right);
            }
            m.Assign(dst, m.Convert(m.Slice(src, PrimitiveType.Word32), PrimitiveType.Word32, PrimitiveType.Int64));
        }

        private void RewriteXnor()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = left;
            if (!right.IsZero)
            {
                src = m.Xor(src, right);
            }
            m.Assign(dst, m.Comp(src));
        }

        private void RewriteXor()
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            var src = left;
            if (!right.IsZero)
            {
                src = m.Xor(src, right);
            }
            m.Assign(dst, src);
        }

        private void RewriteZext(PrimitiveType dtFrom)
        {
            var slice = binder.CreateTemporary(dtFrom);
            m.Assign(slice, m.Slice(RewriteOp(1), dtFrom));
            var dst = RewriteOp(0);
            m.Assign(dst, m.ExtendZ(slice, dst.DataType));
        }
    }
}
