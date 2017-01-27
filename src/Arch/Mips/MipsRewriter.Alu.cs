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
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
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
                opSrc = m.IAdd(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteDiv(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var hi = frame.EnsureRegister(Registers.hi);
            var lo = frame.EnsureRegister(Registers.lo);
            var opLeft = RewriteOperand(instr.op1);
            var opRight = RewriteOperand(instr.op2);
            m.Assign(lo, ctor(opLeft, opRight));
            m.Assign(hi, m.Mod(opLeft, opRight));
        }

        private void RewriteLoad(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            if (opDst.DataType.Size != opSrc.DataType.Size)
                opSrc = m.Cast(arch.WordWidth, opSrc);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLoadLinked32(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_32", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLoadLinked64(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op2);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__load_linked_64", PrimitiveType.Word64, opSrc));
        }

        private void RewriteLui(MipsInstruction instr)
        {
            var immOp = (ImmediateOperand)instr.op2;
            long v = immOp.Value.ToInt16();
            var opSrc = Constant.Create(arch.WordWidth, v << 16);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteLwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            m.Assign(opDst, host.PseudoProcedure("__lwl", PrimitiveType.Word32, opSrc));
        }

        private void RewriteLwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            m.Assign(opDst, host.PseudoProcedure("__lwr", PrimitiveType.Word32, opSrc));
        }

        private void RewriteMf(MipsInstruction instr, RegisterStorage reg)
        {
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, frame.EnsureRegister(reg));
        }

        private void RewriteMt(MipsInstruction instr, RegisterStorage reg)
        {
            var opSrc = RewriteOperand(instr.op1);
            m.Assign(frame.EnsureRegister(reg), opSrc);
        }

        private void RewriteMul(MipsInstruction instr, Func<Expression,Expression,Expression> fn, PrimitiveType ret)
        {
            var hilo = frame.EnsureSequence(Registers.hi, Registers.lo, ret);
            m.Assign(
                hilo,
                fn(RewriteOperand(instr.op1), RewriteOperand(instr.op2)));
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
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, m.Comp(opSrc));
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
                opSrc = m.Or(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSll(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            m.Assign(opDst, m.Shl(opSrc, opShift));
        }

        private void RewriteSra(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            m.Assign(opDst, m.Sar(opSrc, opShift));
        }

        private void RewriteSrl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            m.Assign(opDst, m.Shr(opSrc, opShift));
        }

        private void RewriteStore(MipsInstruction instr)
        {
            var opSrc = RewriteOperand(instr.op1);
            var opDst = RewriteOperand(instr.op2);
            if (opDst.DataType.Size < opSrc.DataType.Size)
                opSrc = m.Cast(opDst.DataType, opSrc);
            m.Assign(opDst, opSrc);
        }

        private void RewriteSub(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op1);
            var opSrc = RewriteOperand(instr.op2);
            var opShift = RewriteOperand(instr.op3);
            m.Assign(opDst, m.ISub(opSrc, opShift));
        }

        private void RewriteSwl(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op2);
            var opSrc = RewriteOperand(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__swl", PrimitiveType.Word32, opSrc));
        }

        private void RewriteSwr(MipsInstruction instr)
        {
            var opDst = RewriteOperand(instr.op2);
            var opSrc = RewriteOperand(instr.op1);
            m.Assign(opDst, host.PseudoProcedure("__swr", PrimitiveType.Word32, opSrc));
        }

        private void RewriteSxx(MipsInstruction instr, Func<Expression,Expression,Expression> op)
        {
            var dst = RewriteOperand(instr.op1);
            var src1 = RewriteOperand(instr.op2);
            var src2 = RewriteOperand(instr.op3);
            m.Assign(
                dst,
                m.Cast(dst.DataType, op(src1,src2)));
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
                opSrc = m.Xor(opLeft, opRight);
            var opDst = RewriteOperand(instr.op1);
            m.Assign(opDst, opSrc);
        }
    }
}
