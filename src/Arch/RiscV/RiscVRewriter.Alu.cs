#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
            var imm = ((ImmediateOperand) instr.Operands[1]).Value.ToInt32();
            m.Assign(dst, m.IAddS(dst, imm));
        }

        private void RewriteAddi4spn()
        {
            var src = binder.EnsureRegister(arch.StackRegister);
            var imm = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
            var dst = RewriteOp(0);
            m.Assign(dst, m.IAddS(src, imm));
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
                src1 = Constant.Word32(0);
            }
            else
            {
                src1 = m.Slice(binder.EnsureRegister(reg1), PrimitiveType.Word32);
            }
            var tmp1 = binder.CreateTemporary(src1.DataType);
            m.Assign(tmp1, src1);
            src1 = tmp1;

            Expression src2;
            if (instr.Operands[iopRight] is ImmediateOperand imm2)
            {
                var c = imm2.Value.ToInt32();
                if (c < 0)
                {
                    op = Operator.ISub;
                    c = -c;
                }
                src2 = Constant.Int32(c);
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

        private void RewriteBinOp(BinaryOperator op, PrimitiveType? dtDst = null)
        {
            var src1 = RewriteOp(1);
            var src2 = RewriteOp(2);
            var dst = RewriteOp(0);
            MaybeSliceSignExtend(dst, m.Bin(op, src1, src2), dtDst);
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
                if (mem.Offset != 0)
                {
                    ea = m.IAddS(ea, mem.Offset);
                }
            }
            else
            {
                var baseReg = RewriteOp(1);
                var offset = ((ImmediateOperand) instr.Operands[2]).Value;
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
            var ui = ((ImmediateOperand)instr.Operands[1]).Value;
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

        private void RewriteShift(BinaryOperator op)
        {
            var left = RewriteOp(1);
            var right = RewriteOp(2);
            var dst = RewriteOp(0);
            m.Assign(dst, m.Bin(op, left, right));
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
                RewriteStore(dt, binder.EnsureRegister(mem.Base), mem.Offset, src);
            }
            else
            {
                var baseReg = RewriteOp(1);
                var offset = ((ImmediateOperand) instr.Operands[2]).Value.ToInt32();
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
    }
}
