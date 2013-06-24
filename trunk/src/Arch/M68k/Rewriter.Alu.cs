#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public partial class Rewriter
    {
        public void RewriteLogical(BinaryOperator op)
        {
            var width = di.Instruction.dataWidth;
            Expression result;
            if (width.Size < 4)
            {
                var opSrc = MaybeCast(width, orw.RewriteSrc(di.Instruction.op1));
                var opDst = MaybeCast(width, orw.RewriteDst(di.Instruction.op2));
                var tmp = frame.CreateTemporary(width);
                emitter.Assign(tmp, new BinaryExpression(op, width, opDst, opSrc));
                emitter.Assign(
                    orw.RewriteSrc(di.Instruction.op2),
                    emitter.Dpb(orw.RewriteSrc(di.Instruction.op2), tmp, 0, width.BitSize));
                result = tmp;
            }
            else
            {
                var opSrc = orw.RewriteSrc(di.Instruction.op1);
                var opDst = orw.RewriteSrc(di.Instruction.op2);
                emitter.Assign(opDst, emitter.Xor(opDst, opSrc));
                result = opDst;
            }
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF), emitter.Cond(result));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        private Expression MaybeCast(PrimitiveType width, Expression expr)
        {
            if (expr.DataType.Size == width.Size)
                return expr;
            else
                return emitter.Cast(width, expr);
        }

        private void RewriteAdda()
        {
            var width = di.Instruction.dataWidth;
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var op2 = orw.RewriteSrc(di.Instruction.op2);
            emitter.Assign(op2, emitter.IAdd(op2, opSrc));
        }

        private void RewritePostOp(PostIncrementMemoryOperand op)
        {
            var postOp = di.Instruction.op1 as PostIncrementMemoryOperand;
            if (postOp == null)
                return;
            var reg = frame.EnsureRegister(op.Register);
            emitter.Assign(reg, emitter.IAdd(reg, di.Instruction.dataWidth.Size));
        }

        public void RewriteMove(bool setFlag)
        {
            var opSrc = orw.RewriteSrc(di.Instruction.op1);
            var opDst = orw.RewriteSrc(di.Instruction.op2);
            Copy(opDst, opSrc, di.Instruction.dataWidth.BitSize);
            if (setFlag)
            {
                emitter.Assign(
                    frame.EnsureFlagGroup(
                        (uint)(FlagM.CF | FlagM.NF | FlagM.VF | FlagM.ZF),
                        "CVZN",
                        PrimitiveType.Byte),
                    emitter.Cond(opSrc));
            }
        }

        private void Copy(Expression dst, Expression src, int bitSize)
        {
            if (dst is Identifier && dst.DataType.BitSize > bitSize)
            {
                var dpb = emitter.Dpb(dst, src, 0, bitSize);
                emitter.Assign(dst, dpb);
            }
            else
            {
                emitter.Assign(dst, src);
            }
        }
    }
}
