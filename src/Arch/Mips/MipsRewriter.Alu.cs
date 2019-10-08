#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteAdd(MipsInstruction instr, PrimitiveType size)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteAnd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opLeft;
            else if (opRight.IsZero)
                opSrc = opRight;
            else
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteDshiftC(MipsInstruction instr, Func<Expression,Expression,Expression> fn, int offset)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            var opShift = (Constant) RewriteOperand0(instr.op3);
            m.Assign(opDst, fn(opSrc, m.Int32(opShift.ToInt32() + offset)));
        }

        private void RewriteDshift(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            var opShift = RewriteOperand0(instr.op3);
            m.Assign(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteCopy(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            m.Assign(dst, src);
        }

        private void RewriteDiv(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            if (instr.op3 != null)
            {
                var op3 = RewriteOperand(instr.op3);
                m.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hi = binder.EnsureRegister(arch.hi);
                var lo = binder.EnsureRegister(arch.lo);
                m.Assign(lo, ctor(op1, op2));
                m.Assign(hi, m.Mod(op1, op2));
            }
        }

        private void RewriteDshift32(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            var sh = m.IAdd(RewriteOperand(instr.op3), 32);
            m.Assign(dst, ctor(src, sh));
        }

        private void RewriteLoad(MipsInstruction instr, PrimitiveType dtSmall)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            if (opDst.DataType.Size != opSrc.DataType.Size)
            {
                // If the source is smaller than the destination register,
                // perform a sign/zero extension.
                opSrc.DataType = dtSmall;
                opSrc = m.Cast(arch.WordWidth, opSrc);
            }
            m.Assign(opDst, opSrc);
        }

        private void RewriteLoadLinked32(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.op2);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_32", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLoadLinked64(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.op2);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_64", PrimitiveType.Word64, opSrc));
        }

        private void RewriteStoreConditional32(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.op2);
            var opReg = RewriteOperand(instr.op1);
            m.Assign(opReg, host.PseudoProcedure("__store_conditional_32", PrimitiveType.Word32, opMem, opReg));
        }

        private void RewriteStoreConditional64(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.op2);
            var opReg = RewriteOperand(instr.op1);
            m.Assign(opReg, host.PseudoProcedure("__store_conditional_64", PrimitiveType.Word64, opMem, opReg));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand)instr.op2;
            long v = immOp.Value.ToInt16();
            var opSrc = Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            m.Assign(opDst, host.PseudoProcedure("__lwl", PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteLwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            m.Assign(opDst, host.PseudoProcedure("__lwr", PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteLdc1(MipsInstruction instr)
        {
            var opDstFloat = RewriteOperand(instr.op1);
            var opSrcMem = RewriteOperand(instr.op2);
            m.Assign(opDstFloat, opSrcMem);
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage reg)
        {
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, binder.EnsureRegister(reg));
        }

        private void RewriteMt(MipsInstruction instr, RegisterStorage reg)
        {
            var opSrc = RewriteOperand0(instr.op1);
            m.Assign(binder.EnsureRegister(reg), opSrc);
        }

        private void RewriteMovCc(MipsInstruction instr, Func<Expression, Expression> cmp0)
        {
            var opCond = RewriteOperand0(instr.op3);
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            m.BranchInMiddleOfInstruction(
                cmp0(opCond).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Assign(opDst, opSrc);
        }

        private void RewriteMul(MipsInstruction instr, Func<Expression, Expression, Expression> ctor, PrimitiveType dt)
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            if (instr.op3 != null)
            {
                var op3 = RewriteOperand(instr.op3);
                m.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hilo = binder.EnsureSequence(arch.hi, arch.lo, dt);
                m.Assign(hilo, ctor(op1, op2));
            }
        }

        private void RewriteNor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, m.Comp(opSrc));
        }

        private void RewriteOr(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLdl(MipsInstruction instr)
        {
            var opSrc = (IndirectOperand)instr.op2;
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(
                opDst,
                host.PseudoProcedure("__ldl",
                    VoidType.Instance,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.Offset)));
        }

        private void RewriteLdr(MipsInstruction instr)
        {
            var opSrc = (IndirectOperand)instr.op2;
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(
                opDst,
                host.PseudoProcedure("__ldr",
                    VoidType.Instance,
                    binder.EnsureRegister(opSrc.Base),
                    m.Int32(opSrc.Offset)));
        }

        private void RewriteRestore(MipsInstruction instr, bool ret)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29));
            int count = -1;
            int i = 0;
            bool gp = false;
            int rt = -1;
            int u = -1;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count))
                    ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var this_offset = u - ((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                var reg = binder.EnsureRegister(arch.GetRegister(rt));
                m.Assign(reg, ea);
                ++i;
            }
            m.Assign(sp, m.IAddS(sp, u));
            if (ret)
                m.Return(0, 0);
        }

        private void RewriteSave(MipsInstruction instr)
        {
            var sp = binder.EnsureRegister(arch.GetRegister(29));
            int count = -1;
            int rt = -1;
            int u = -1;
            bool gp = false;
            int i = 0;
            while (i != count)
            {
                var this_rt = (gp && (i + 1 == count)) ? 28
                    : rt + i < 32
                        ? rt + i
                        : rt + i - 16;
                var reg = binder.EnsureRegister(arch.GetRegister(this_rt));
                var this_offset = -((i + 1) << 2);
                var ea = m.Mem32(m.IAddS(sp, this_offset));
                m.Assign(ea, reg);
                ++i;
            }
            m.Assign(sp, m.ISubS(sp, u));
        }

        private void RewriteSdl(MipsInstruction instr)
        {
            var opDst = (IndirectOperand)instr.op2;
            var opSrc = RewriteOperand0(instr.op1);
            m.SideEffect(
                host.PseudoProcedure("__sdl",
                    VoidType.Instance,
                    binder.EnsureRegister(opDst.Base),
                    m.Int32(opDst.Offset),
                    opSrc));
        }

        private void RewriteSdr(MipsInstruction instr)
        {
            var opDst = (IndirectOperand)instr.op2;
            var opSrc = RewriteOperand0(instr.op1);
            m.SideEffect(
                host.PseudoProcedure("__sdr",
                    VoidType.Instance,
                    binder.EnsureRegister(opDst.Base),
                    m.Int32(opDst.Offset),
                    opSrc));
        }

        private void RewriteSignExtend(MipsInstruction instr, PrimitiveType dt)
        {
            var opDst = RewriteOperand(instr.op2);
            var opSrc = RewriteOperand(instr.op1);
            var tmp = binder.CreateTemporary(dt);
            var dtDst = PrimitiveType.Create(Domain.SignedInt, opDst.DataType.BitSize);
            m.Assign(tmp, m.Slice(dt, opSrc, 0));
            m.Assign(opDst, m.Cast(dtDst, tmp));
        }

        private void RewriteSll(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            var opShift = RewriteOperand0(instr.op3);
            m.Assign(opDst, m.Shl(opSrc, opShift));
        }

        private void RewriteSra(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            var opShift = RewriteOperand0(instr.op3);
            m.Assign(opDst, m.Sar(opSrc, opShift));
        }

        private void RewriteSrl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op1);
            var opSrc = RewriteOperand0(instr.op2);
            var opShift = RewriteOperand0(instr.op3);
            m.Assign(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand0(instr.op1);
            var opDst = RewriteOperand0(instr.op2);
            if (opDst.DataType.Size < opSrc.DataType.Size)
                opSrc = m.Cast(opDst.DataType, opSrc);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSub(MipsInstruction instr, PrimitiveType size)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = m.Neg(opRight);
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.ISub(opLeft, opRight);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op2);
            var opSrc = RewriteOperand0(instr.op1);
            m.Assign(opDst, host.PseudoProcedure(PseudoProcedure.SwL, PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteSwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand0(instr.op2);
            var opSrc = RewriteOperand0(instr.op1);
            m.Assign(opDst, host.PseudoProcedure(PseudoProcedure.SwR, PrimitiveType.Word32, opDst, opSrc));
        }

        private void RewriteSxx(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOperand0(instr.op1);
            var src1 = RewriteOperand0(instr.op2);
            var src2 = RewriteOperand0(instr.op3);
            m.Assign(
                dst,
                m.Cast(dst.DataType, op(src1,src2)));
        }

        private void RewriteXor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand0(instr.op2);
            var opRight = RewriteOperand0(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = m.Xor(opLeft, opRight);
            var opDst = RewriteOperand0(instr.op1);
            m.Assign(opDst, opSrc);
        }
    }
}
