#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

        private void RewriteSxx(MipsInstruction instr, Operator op)
        {
            var dst = RewriteOperand(instr.op1);
            var src1 = RewriteOperand(instr.op2);
            var src2 = RewriteOperand(instr.op3);
            emitter.Assign(
                dst,
                emitter.Cast(
                    dst.DataType,
                    new BinaryExpression(
                        op,
                        PrimitiveType.Bool,
                        src1,
                        src2)));
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
