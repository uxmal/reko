#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Arch.Mips.Rewriter
{
    public partial class MipsRewriter
    {
        private void RewriteAdd(MipsInstruction instr, DataType size)
        {
            Expression opLeft;
            Expression opRight;
            if (instr.Operands.Length == 3)
            {
                opLeft = RewriteOperand0(instr, 1);
                opRight = RewriteOperand0(instr, 2);
            }
            else
            {
                opLeft = RewriteOperand0(instr, 0);
                opRight = RewriteOperand0(instr, 1);
            }
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, opSrc);
        }

        private void RewriteAddiupc(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var offset = ((Constant) instr.Operands[1]).ToUInt32();
            var addr = instr.Address + (instr.Length + offset);
            AssignS(dst, addr);
        }

        private void RewriteAluipc(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var offset = (((Constant)instr.Operands[1]).ToUInt32() << 12);
            var addr = instr.Address.NewOffset(offset);
            AssignS(dst, addr);
        }

        private void RewriteAnd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr, 1);
            var opRight = RewriteOperand0(instr, 2);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opLeft;
            else if (opRight.IsZero)
                opSrc = opRight;
            else
                opSrc = m.And(opLeft, opRight);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, opSrc);
        }

        private void RewriteCache(MipsInstruction instr, IntrinsicProcedure intrinsic)
        {
            var op1 = RewriteOperand(instr, 0);
            var opMem = m.AddrOf(arch.PointerType, RewriteOperand(instr, 1));
            m.SideEffect(m.Fn(intrinsic, op1, opMem), iclass);
        }

        private void RewriteClo(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            AssignS(opDst, m.Fn(CommonOps.CountLeadingOnes, opSrc));
        }

        private void RewriteClz(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            AssignS(opDst, m.Fn(CommonOps.CountLeadingZeros, opSrc));
        }

        private void RewriteDshiftC(MipsInstruction instr, Func<Expression,Expression,Expression> fn, int offset)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            var opShift = (Constant) RewriteOperand0(instr, 2);
            AssignS(opDst, fn(opSrc, m.Int32(opShift.ToInt32() + offset)));
        }

        private void RewriteDshift(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            var opShift = RewriteOperand0(instr, 2);
            AssignS(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteCopy(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var src = RewriteOperand(instr, 1);
            AssignS(dst, src);
        }

        private void RewriteDiv(
            MipsInstruction instr, 
            BinaryOperator div,
            BinaryOperator mod,
            RegisterStorage? hi,
            RegisterStorage? lo)
        {
            var op2 = RewriteOperand0(instr, 1, arch.WordWidth);
            if (instr.Operands.Length > 3)
            {
                var op3 = RewriteOperand0(instr, 2, arch.WordWidth);
                var op1 = RewriteOperand(instr, 0);
                AssignS(op1, m.Bin(div, op2, op3));
            }
            else
            {
                if (hi is null || lo is null)
                {
                    m.Invalid();
                    return;
                }
                var id_hi = binder.EnsureRegister(hi);
                var id_lo = binder.EnsureRegister(lo);
                var op1 = RewriteOperand0(instr, 0, arch.WordWidth);
                AssignS(id_lo, m.Bin(div, op1, op2));
                AssignS(id_hi, m.Bin(mod, op1, op2));
            }
        }

        private void RewriteDshift32(MipsInstruction instr, BinaryOperator shift)
        {
            var dst = RewriteOperand(instr, 0);
            var src = RewriteOperand(instr, 1);
            var s = ((Constant) instr.Operands[2]).ToInt32() + 32;
            AssignS(dst, m.Bin(shift, src, m.Byte((byte)s)));
        }

        private void RewriteExt(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var src = RewriteOperand(instr, 1);
            var pos = RewriteOperand(instr, 2);
            var size = RewriteOperand(instr, 3);
            AssignS(dst, m.Fn(intrinsics.ext.MakeInstance(src.DataType, pos.DataType), src, pos, size));
        }

        private void RewriteIns(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var src = RewriteOperand0(instr, 1);
            var pos = RewriteOperand(instr, 2);
            var size = RewriteOperand(instr, 3);
            AssignS(dst, m.Fn(intrinsics.ins.MakeInstance(src.DataType, pos.DataType), dst, src, pos, size));
        }

        private void RewriteLdl(MipsInstruction instr)
        {
            var opSrc = (MemoryOperand) instr.Operands[1];
            var opDst = RewriteOperand0(instr, 0);
            AssignS(
                opDst,
                m.Fn(intrinsics.ldl,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.IntOffset())));
        }

        private void RewriteLdr(MipsInstruction instr)
        {
            var opSrc = (MemoryOperand) instr.Operands[1];
            var opDst = RewriteOperand0(instr, 0);
            AssignS(
                opDst,
                m.Fn(intrinsics.ldr,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.IntOffset())));
        }

        private void RewriteLoadE(MipsInstruction instr, PrimitiveType dtSmall, PrimitiveType? dtSmall64 = null)
        {
            var opSrc = RewriteOperand(instr, 1);
            var opDst = RewriteOperand(instr, 0);
            opSrc.DataType = (arch.WordWidth.BitSize == 64)
                ? dtSmall64 ?? dtSmall
                : dtSmall;
            if (opDst.DataType.Size != opSrc.DataType.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension/conversion.
                opSrc = m.Convert(opSrc, opSrc.DataType, arch.WordWidth);
            }
            AssignS(opDst, opSrc);
        }

        private void RewriteLoadS(MipsInstruction instr, PrimitiveType dtSmall, PrimitiveType? dtSmall64 = null)
        {
            var opSrc = RewriteOperand(instr, 1);
            var opDst = RewriteOperand(instr, 0);
            opSrc.DataType = (arch.WordWidth.BitSize == 64)
                ? dtSmall64 ?? dtSmall
                : dtSmall;
            opSrc = m.MaybeExtendS(opSrc, opDst.DataType);
            AssignS(opDst, opSrc);
        }

        private void RewriteLoadZ(MipsInstruction instr, PrimitiveType dtSmall, PrimitiveType? dtSmall64 = null)
        {
            var opSrc = RewriteOperand(instr, 1);
            var opDst = RewriteOperand(instr, 0);
            opSrc.DataType = (arch.WordWidth.BitSize == 64)
                ? dtSmall64 ?? dtSmall
                : dtSmall;
            opSrc = m.MaybeExtendZ(opSrc, opDst.DataType);
            AssignS(opDst, opSrc);
        }

        private void RewriteLoadIndexed(MipsInstruction instr, PrimitiveType dt, PrimitiveType dtDst, int scale)
        {
            var opSrc = RewriteMemoryOperand((MemoryOperand)instr.Operands[1], scale);
            var opDst = RewriteOperand(instr, 0);
            if (opDst.DataType.Size != dt.Size)
            {
                opSrc = m.Convert(opSrc, dt, dtDst);
            }
            AssignS(opDst, opSrc);
        }

        private void RewriteLoadLinked(MipsInstruction instr, PrimitiveType dt)
        {
            var opSrc = RewriteOperand0(instr, 1);
            var opDst = RewriteOperand0(instr, 0);
            var ptrType = arch.PointerType;
            AssignS(opDst, m.Fn(
                intrinsics.load_linked.MakeInstance(ptrType.BitSize, dt),
                m.AddrOf(ptrType, opSrc)));
        }

        private void RewriteStoreConditional(MipsInstruction instr, PrimitiveType dt)
        {
            var opMem = RewriteOperand(instr, 1);
            var opReg = RewriteOperand(instr, 0);
            var ptrType = arch.PointerType;
            AssignS(opReg, m.Fn(
                intrinsics.store_conditional.MakeInstance(ptrType.BitSize, dt),
                m.AddrOf(ptrType, opMem),
                opReg));
        }

        private void RewriteLsa(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr, 0);
            var rs = RewriteOperand(instr, 1);
            var rt = RewriteOperand(instr, 2);
            var sh = ((Constant)instr.Operands[3]).ToInt32();
            AssignS(dst, m.IAdd(rt, m.Shl(rs, sh)));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (Constant)instr.Operands[1];
            long v = immOp.ToInt16();
            var opSrc = m.Const(arch.WordWidth, v << 16);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, opSrc);
        }

        private void RewriteLwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            AssignS(opDst, m.Fn(intrinsics.lwl, opDst, opSrc));
        }

        private void RewriteLwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            AssignS(opDst, m.Fn(intrinsics.lwr, opDst, opSrc));
        }

        private void RewriteLcpr1(MipsInstruction instr)
        {
            var opDstFloat = RewriteOperand(instr, 0);
            var opSrcMem = RewriteOperand(instr, 1);
            long bitDiff = opDstFloat.DataType.BitSize - opSrcMem.DataType.BitSize;
            if (bitDiff > 0)
            {
                var tmpLo = binder.CreateTemporary(opSrcMem.DataType);
                var tmpHi = binder.CreateTemporary(PrimitiveType.CreateWord(bitDiff));
                AssignS(tmpLo, opSrcMem);
                AssignS(tmpHi, m.Slice(opDstFloat, tmpHi.DataType, opSrcMem.DataType.BitSize));
                AssignS(opDstFloat, m.Seq(tmpHi, tmpLo));
            }
            else
            {
                AssignS(opDstFloat, opSrcMem);
            }
        }

        private void RewriteLdc2(MipsInstruction instr)
        {
            var iRegDst = ((RegisterStorage) instr.Operands[0]).Number;
            var opSrcMem = RewriteOperand(instr, 1);
            m.SideEffect(m.Fn(intrinsics.write_cpf2,
                Constant.Byte((byte) iRegDst),
                opSrcMem));
        }

        private void RewriteLe(MipsInstruction instr, PrimitiveType dt)
        {
            Expression src = m.Fn(
                intrinsics.load_ub_EVA.MakeInstance(arch.PointerType.BitSize, dt), 
                m.AddrOf(arch.PointerType, RewriteOperand(instr, 1)));
            var dst = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            if (dst.DataType.Size != dt.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension.
                src.DataType = dt;
                src = m.Convert(src, src.DataType, dst.DataType);
            }
            AssignS(dst, src);
        }

        private void RewriteLwm(MipsInstruction instr)
        {
            int i = 0;
            int rt = ((RegisterStorage) instr.Operands[0]).Number;
            var mem = ((MemoryOperand) instr.Operands[1]);
            var rs = binder.EnsureRegister(mem.Base);
            int offset = mem.IntOffset();
            int count = ((Constant)instr.Operands[2]).ToInt32();
            while (i != count)
            {
                int this_rt = (rt + i < 32) ? rt + i : rt + i - 16;
                int this_offset = offset + (i << 2);
                var dst = binder.EnsureRegister(arch.GetRegister(this_rt)!);
                AssignS(dst, m.Mem32(m.IAddS(rs, this_offset)));

                // if this_rt == rs and i != count - 1:
                // raise UNPREDICTABLE()
                ++i;
            }
        }

        private void RewriteLwpc(MipsInstruction instr)
        {
            var src = RewriteOperand(instr, 1);
            var dst = RewriteOperand(instr, 0);
            AssignS(dst, m.Mem32(src));
        }

        private void RewriteLwxs(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var idx = (MemoryOperand) instr.Operands[1];
            Debug.Assert(idx.Index is not null);
            var idBase = binder.EnsureRegister(idx.Base);
            var idIndex = binder.EnsureRegister(idx.Index);
            AssignS(opDst, m.Mem32(m.IAdd(idBase, m.IMul(idIndex, 4))));
        }

        private void RewriteLx(MipsInstruction instr, PrimitiveType dt, int scale)
        {
            var dst = RewriteOperand(instr, 0);
            var idx = (MemoryOperand) instr.Operands[1];
            Debug.Assert(idx.Index is not null);
            var idBase = binder.EnsureRegister(idx.Base);
            Expression index = binder.EnsureRegister(idx.Index);
            if (scale != 1)
            {
                index = m.IMul(index, scale);
            }
            Expression src = m.Mem32(m.IAdd(idBase, index));
            if (dst.DataType.Size != dt.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension.
                src.DataType = dt;
                src = m.Convert(src, src.DataType, dst.DataType);
            }
            AssignS(dst, src);
        }

        private void RewriteMac_int(
            MipsInstruction instr,
            DataType dtProduct,
            BinaryOperator mul,
            BinaryOperator acc)
        {
            var op1 = RewriteOperand0(instr, 0);
            var op2 = RewriteOperand0(instr, 1);
            var hi_lo = binder.EnsureSequence(PrimitiveType.Word64, arch.hi, arch.lo);
            var product = m.Bin(mul, dtProduct, op1, op2);
            product.DataType = hi_lo.DataType;
            AssignS(hi_lo, m.Bin(acc, hi_lo, product));
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage? reg)
        {
            if (reg is null)
            {
                m.Invalid();
                return;
            }
            var opDst = RewriteOperand0(instr, 0);
            Expression sc = binder.EnsureRegister(reg);
            if (sc.DataType.BitSize < opDst.DataType.BitSize)
            {
                sc = m.Dpb(opDst, sc, 0);
            }
            AssignS(opDst, sc);
        }

        private void RewriteMt(MipsInstruction instr, RegisterStorage? reg)
        {
            if (reg is null)
            {
                m.Invalid();
                return;
            }
            var opSrc = RewriteOperand0(instr, 0);
            AssignS(binder.EnsureRegister(reg), m.MaybeSlice(opSrc, reg.DataType));
        }

        private void RewriteMod(MipsInstruction instr, BinaryOperator ctor)
        {
            var dst = RewriteOperand(instr, 0);
            var op1 = RewriteOperand(instr, 1);
            var op2 = RewriteOperand(instr, 2);
            AssignS(dst, m.Bin(ctor, op1, op2));
        }

        private void RewriteMovCc(MipsInstruction instr, Func<Expression, Expression> cmp0)
        {
            var opCond = RewriteOperand0(instr, 2, arch.WordWidth);
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            m.BranchInMiddleOfInstruction(
                cmp0(opCond).Invert(),
                instr.Address + instr.Length,
                InstrClass.CondJump);
            AssignS(opDst, opSrc);
        }

        private void RewriteMove(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr, 1);
            var dst = RewriteOperand(instr, 0);
            AssignS(dst, src);
        }

        private void RewriteMovep(MipsInstruction instr)
        {
            var srcHi = RewriteOperand0(instr, 2);
            var srcLo = RewriteOperand0(instr, 3);
            var dstHi = RewriteOperand(instr, 0);
            var dstLo = RewriteOperand(instr, 1);
            AssignS(dstHi, srcHi);
            AssignS(dstLo, srcLo);
        }

        private void RewriteMul(MipsInstruction instr, BinaryOperator mul, PrimitiveType dt)
        {
            var op1 = RewriteOperand(instr.Operands[0]);
            var op2 = RewriteOperand(instr.Operands[1]);
            if (instr.Operands.Length == 3)
            {
                var op3 = RewriteOperand(instr.Operands[2]);
                m.Assign(op1, m.Bin(mul, dt, op2, op3));
            }
            else
            {
                var hilo = binder.EnsureSequence(dt, arch.hi, arch.lo);
                m.Assign(hilo, m.Bin(mul, dt, op1, op2));
            }
        }

        private void RewriteMult(
            MipsInstruction instr,
            BinaryOperator mul,
            PrimitiveType dt, 
            RegisterStorage? hi,
            RegisterStorage? lo)
        {
            if (hi is null || lo is null)
            {
                m.Invalid();
                return;
            }
            var op1 = RewriteOperand(instr, 0);
            var op2 = RewriteOperand(instr, 1);
            if (instr.Operands.Length == 3 && 
                ((RegisterStorage) instr.Operands[0]).Number != 0)
            {
                var op3 = RewriteOperand(instr, 2);
                AssignS(op1, m.Bin(mul, dt, op2, op3));
            }
            else if (hi.DataType.BitSize + lo.DataType.BitSize ==  dt.BitSize)
            {
                var hilo = binder.EnsureSequence(dt, hi, lo);
                AssignS(hilo, m.Bin(mul, dt, op1, op2));
            }
            var tmp = binder.CreateTemporary(dt);
            int half = (int)dt.BitSize / 2;
            AssignS(tmp, m.Bin(mul, dt, op1, op2));
            AssignS(binder.EnsureRegister(lo), m.ExtendS(m.Slice(tmp, 0, half), lo.DataType));
            AssignS(binder.EnsureRegister(hi), m.ExtendS(m.Slice(tmp, half, half), hi.DataType));
        }

        private void RewriteNor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr, 1);
            var opRight = RewriteOperand0(instr, 2);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, m.Comp(opSrc));
        }

        private void RewriteMuh(MipsInstruction instr, PrimitiveType dtProduct, BinaryOperator fn)
        {
            var src1 = RewriteOperand(instr, 1);
            var src2 = RewriteOperand(instr, 2);
            var product = m.Bin(fn, src1, src2);
            product.DataType = dtProduct;
            var dst = RewriteOperand(instr, 0);
            AssignS(dst, m.Slice(product, dst.DataType, product.DataType.BitSize - dst.DataType.BitSize));
        }

        private void RewriteNot(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr, 1);
            if (src is Constant c)
                src = Operator.Comp.ApplyConstant(c);
            else
                src = m.Comp(src);
            var dst = RewriteOperand0(instr, 0);
            AssignS(dst, src);
        }

        private void RewriteOr(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr, 1);
            var opRight = RewriteOperand0(instr, 2);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr, 0);
            AssignS(opDst, opSrc);
        }

        private void RewriteRestore(MipsInstruction instr, bool ret)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29)!);
            int count = ((Constant)instr.Operands[2]).ToInt32();
            int rt = ((RegisterStorage) instr.Operands[1]).Number;
            int u = ((Constant)instr.Operands[0]).ToInt32();
            int i = 0;
            bool gp = false;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count))
                    ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var this_offset = u - ((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                var reg = binder.EnsureRegister(arch.GetRegister(this_rt)!);
                AssignS(reg, ea);
                ++i;
            }
            AssignS(sp, m.IAddS(sp, u));
            if (ret)
                m.Return(0, 0);
        }

        protected virtual void RewriteSave(MipsInstruction instr)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29)!);
            int count = ((Constant)instr.Operands[2]).ToInt32();
            int rt = ((RegisterStorage) instr.Operands[1]).Number;
            int u = ((Constant)instr.Operands[0]).ToInt32();
            bool gp = false;
            int i = 0;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count)) ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var reg = binder.EnsureRegister(arch.GetRegister(this_rt)!);
                var this_offset = -((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                AssignS(ea, reg);
                ++i;
            }
            AssignS(sp, m.ISubS(sp, u));
        }

        private void RewriteRotr(MipsInstruction instr)
        {
            var arg1 = RewriteOperand(instr, 1);
            var arg2 = RewriteOperand(instr, 2);
            var dst = RewriteOperand(instr, 0);
            AssignS(dst, m.Fn(CommonOps.Ror, arg1, arg2));
        }

        private void RewriteRotx(MipsInstruction instr)
        {
            var arg1 = RewriteOperand(instr, 1);
            var arg2 = RewriteOperand(instr, 2);
            var arg3 = RewriteOperand(instr, 3);
            var arg4 = RewriteOperand(instr, 4);
            var dst = RewriteOperand(instr, 0);
            AssignS(dst, m.Fn(intrinsics.rotx.MakeInstance(dst.DataType), arg1, arg2, arg3, arg4));
        }

        private void RewriteSdc2(MipsInstruction instr)
        {
            var iRegSrc = ((RegisterStorage) instr.Operands[0]).Number;
            var opDstMem = RewriteOperand(instr, 1);
            AssignS(opDstMem, m.Fn(
                intrinsics.read_cpr2.MakeInstance(opDstMem.DataType),
                Constant.Byte((byte) iRegSrc)));
        }

        private void RewriteSdl(MipsInstruction instr)
        {
            var opDst = (MemoryOperand)instr.Operands[1];
            var opSrc = RewriteOperand0(instr, 0);
            m.SideEffect(m.Fn(
                intrinsics.sdl,
                binder.EnsureRegister(opDst.Base),
                m.Int32(opDst.IntOffset()),
                opSrc));
        }

        private void RewriteSdr(MipsInstruction instr)
        {
            var opDst = (MemoryOperand)instr.Operands[1];
            var opSrc = RewriteOperand0(instr, 0);
            m.SideEffect(m.Fn(
              intrinsics.sdr,
              binder.EnsureRegister(opDst.Base),
              m.Int32(opDst.IntOffset()),
              opSrc));
        }

        private void RewriteSignExtend(MipsInstruction instr, PrimitiveType dt)
        {
            var opDst = RewriteOperand(instr, 1);
            var opSrc = RewriteOperand(instr, 0);
            var tmp = binder.CreateTemporary(dt);
            var dtDst = PrimitiveType.Create(Domain.SignedInt, opDst.DataType.BitSize);
            AssignS(tmp, m.Slice(opSrc, dt));
            AssignS(opDst, m.Convert(tmp, tmp.DataType, dtDst));
        }

        private void RewriteSll(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1);
            var opShift = RewriteOperand0(instr, 2);
            AssignS(opDst, m.Shl(opSrc, opShift));
        }

        private void RewriteSra(MipsInstruction instr, DataType dt)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1, dt);
            var opShift = RewriteOperand0(instr, 2, dt);
            AssignS(opDst, m.Sar(opSrc, opShift));
        }

        private void RewriteSrl(MipsInstruction instr, PrimitiveType dt)
        {
            var opDst = RewriteOperand0(instr, 0);
            var opSrc = RewriteOperand0(instr, 1, dt);
            var opShift = RewriteOperand0(instr, 2);
            AssignS(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr, 0);
            var opDst = RewriteOperand0(instr, 1);
            AssignS(opDst, m.MaybeSlice(opSrc, opDst.DataType));
        }

        private void RewriteSte(MipsInstruction instr, PrimitiveType dt)
        {
            var src = binder.EnsureRegister((RegisterStorage) instr.Operands[0]);
            if (src.DataType.Size != dt.Size)
            {
                // If the source is smaller than the destination register,
                // perform a slice.
                var tmp = binder.CreateTemporary(dt);
                AssignS(tmp, m.Slice(src, dt));
                src = tmp;
            }

            var mem = RewriteOperand(instr, 1);
            mem.DataType = dt;

            m.SideEffect(
                m.Fn(
                    intrinsics.store_EVA.MakeInstance(arch.PointerType.BitSize, dt),
                    m.AddrOf(arch.PointerType, mem),
                    src));
        }


        private void RewriteSub(MipsInstruction instr, BinaryOperator sub, PrimitiveType dt)
        {
            var opLeft = RewriteOperand0(instr, 1, dt);
            var opRight = RewriteOperand0(instr, 2, dt);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = m.Neg(opRight);
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Bin(sub, opLeft, opRight);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, opSrc);
        }

        private void RewriteSwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 1);
            var opSrc = RewriteOperand0(instr, 0);
            AssignS(opDst, m.Fn(intrinsics.swl, opDst, opSrc));
        }

        private void RewriteSwpc(MipsInstruction instr)
        {
            var dstEa = RewriteOperand(instr, 1);
            var src = RewriteOperand(instr, 0);
            AssignS(m.Mem32(dstEa), src);
        }

        private void RewriteSwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr, 1);
            var opSrc = RewriteOperand0(instr, 0);
            AssignS(opDst, m.Fn(intrinsics.swr, opDst, opSrc));
        }

        private void RewriteSwm(MipsInstruction instr)
        {
            var rt = ((RegisterStorage) instr.Operands[0]).Number;
            var count = ((Constant)instr.Operands[2]).ToInt32();
            var ind = (MemoryOperand) instr.Operands[1];
            var offset = ind.IntOffset();
            var rs = binder.EnsureRegister(ind.Base);
            for (int i = 0; i < count; ++i) {
                int this_rt;
                if (rt == 0)
                    this_rt = 0;
                else if (rt + i < 32)
                    this_rt = rt + i;
                else
                    this_rt = rt + i - 16;
                var ea = m.IAddS(rs, offset);
                AssignS(m.Mem32(ea), binder.EnsureRegister(arch.GeneralRegs[this_rt]));
                offset += 4;
            }
        }

        private void RewriteSxs(MipsInstruction instr, PrimitiveType dt, int scale)
        {
            var src = RewriteOperand0(instr, 0);
            if (src.DataType.BitSize > dt.BitSize)
            {
                src = m.Slice(src, dt, 0);
            }
            var idx = (MemoryOperand) instr.Operands[1];
            Debug.Assert(idx.Index is not null);
            var idBase = binder.EnsureRegister(idx.Base);
            Expression idIndex = binder.EnsureRegister(idx.Index);
            if (scale != 1)
                idIndex = m.IMul(idIndex, scale);
            AssignS(m.Mem(dt, m.IAdd(idBase, idIndex)), src);
        }

        private void RewriteScc(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOperand0(instr, 0);
            var src1 = RewriteOperand0(instr, 1);
            var src2 = RewriteOperand0(instr, 2);
            var result = op(src1, src2);
            AssignS(
                dst,
                m.Convert(result, result.DataType, dst.DataType));
        }

        private void RewriteWsbh(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr, 1);
            var dst = RewriteOperand0(instr, 0);
            AssignS(dst, m.Fn(intrinsics.wsbh, src));
        }

        private void RewriteXor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr, 1);
            var opRight = RewriteOperand0(instr, 2);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Xor(opLeft, opRight);
            var opDst = RewriteOperand0(instr, 0);
            AssignS(opDst, opSrc);
        }
    }
}
