#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private void RewriteAdd(MipsInstruction instr, PrimitiveType size)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = emitter.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteAnd(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opLeft;
            else if (opRight.IsZero)
                opSrc = opRight;
            else
                opSrc = emitter.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteCopy(MipsInstruction instr)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            emitter.Assign(dst, src);
        }

        private void RewriteDiv(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            if (instr.op3 != null)
            {
                var op3 = RewriteOperand(instr.op3);
                emitter.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hi = frame.EnsureRegister(arch.hi);
                var lo = frame.EnsureRegister(arch.lo);
                emitter.Assign(lo, ctor(op1, op2));
                emitter.Assign(hi, emitter.Mod(op1, op2));
            }
        }

        private void RewriteDshift32(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            var sh = emitter.IAdd(RewriteOperand(instr.op3), 32);
            emitter.Assign(dst, ctor(src, sh));
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
                opSrc = emitter.Cast(arch.WordWidth, opSrc);
            }
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteLoadLinked32(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, host.PseudoProcedure("__load_linked_32", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLoadLinked64(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, host.PseudoProcedure("__load_linked_64", PrimitiveType.Word64, opSrc));
        }

        private void RewriteStoreConditional32(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.op2);
            var opReg = RewriteOperand(instr.op1);
            emitter.Assign(opReg, host.PseudoProcedure("__store_conditional_32", PrimitiveType.Word32, opMem, opReg));
        }

        private void RewriteStoreConditional64(MipsInstruction instr)
        {
            var opMem = RewriteOperand(instr.op2);
            var opReg = RewriteOperand(instr.op1);
            emitter.Assign(opReg, host.PseudoProcedure("__store_conditional_64", PrimitiveType.Word64, opMem, opReg));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand)instr.op2;
            long v = immOp.Value.ToInt16();
            var opSrc = Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteLwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            emitter.Assign(opDst, host.PseudoProcedure("__lwl", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            emitter.Assign(opDst, host.PseudoProcedure("__lwr", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLdc1(MipsInstruction instr)
        {
            var opDstFloat = RewriteOperand(instr.op1);
            var opSrcMem = RewriteOperand(instr.op2);
            emitter.Assign(opDstFloat, opSrcMem);
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage reg)
        {
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, frame.EnsureRegister(reg));
        }

        private void RewriteMt(MipsInstruction instr, RegisterStorage reg)
        {
            var opSrc = RewriteOperand(instr.op1);
            emitter.Assign(frame.EnsureRegister(reg), opSrc);
        }

        private void RewriteMul(MipsInstruction instr, Func<Expression, Expression, Expression> ctor, PrimitiveType dt)
        {
            var op1 = RewriteOperand(instr.op1);
            var op2 = RewriteOperand(instr.op2);
            if (instr.op3 != null)
            {
                var op3 = RewriteOperand(instr.op3);
                emitter.Assign(op1, ctor(op2, op3));
            }
            else
            {
                var hilo = frame.EnsureSequence(arch.hi, arch.lo, dt);
                emitter.Assign(hilo, ctor(op1, op2));
            }
        }

        private void RewriteNor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = emitter.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, emitter.Comp(opSrc));
        }

        private void RewriteOr(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = emitter.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteSll(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            emitter.Assign(opDst, emitter.Shl(opSrc, opShift));
        }

        private void RewriteSra(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            emitter.Assign(opDst, emitter.Sar(opSrc, opShift));
        }

        private void RewriteSrl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            emitter.Assign(opDst, emitter.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op1);
            var opDst = RewriteOperand(instr.op2);
            if (opDst.DataType.Size < opSrc.DataType.Size)
                opSrc = emitter.Cast(opDst.DataType, opSrc);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteSub(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            emitter.Assign(opDst, emitter.ISub(opSrc, opShift));
        }

        private void RewriteSwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op2);
            var opSrc = RewriteOperand(instr.op1);
            emitter.Assign(opDst, host.PseudoProcedure("__swl", PrimitiveType.Word32, opSrc));
        }

        private void RewriteSwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op2);
            var opSrc = RewriteOperand(instr.op1);
            emitter.Assign(opDst, host.PseudoProcedure("__swr", PrimitiveType.Word32, opSrc));
        }

        private void RewriteSxx(MipsInstruction instr, Func<Expression, Expression, Expression> op)
        {
            var dst = RewriteOperand(instr.op1);
            var src1 = RewriteOperand(instr.op2);
            var src2 = RewriteOperand(instr.op3);
            emitter.Assign(
                dst,
                emitter.Cast(dst.DataType, op(src1, src2)));
        }

        private void RewriteXor(MipsInstruction instr)
        {
            var opLeft = RewriteOperand(instr.op2);
            var opRight = RewriteOperand(instr.op3);
            Expression opSrc;
            if (opLeft.IsZero)
                opSrc = opRight;
            else if (opRight.IsZero)
                opSrc = opLeft;
            else
                opSrc = emitter.Xor(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            emitter.Assign(opDst, opSrc);
        }
    }
}
