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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public partial class Rewriter
    {
        public void RewriteEor()
        {
            var width = di.Instruction.dataWidth;
            Expression result;
            if (width.Size < 4)
            {
                var op1 = emitter.Cast(width, orw.Rewrite(di.Instruction.op1));
                var op2 = emitter.Cast(width, orw.Rewrite(di.Instruction.op2));
                var tmp = frame.CreateTemporary(di.Instruction.dataWidth);
                emitter.Assign(tmp, emitter.Xor(op1, op2));
                emitter.Assign(orw.Rewrite(di.Instruction.op2), emitter.Dpb(orw.Rewrite(di.Instruction.op2), tmp, 0, width.BitSize));
                result= tmp;
            }
            else
            {
                var op1 = orw.Rewrite(di.Instruction.op1);
                var op2 = orw.Rewrite(di.Instruction.op2);
                emitter.Assign(op2, emitter.Xor(op1, op2));
                result = op2;
            }
            emitter.Assign(orw.FlagGroup(FlagM.NF | FlagM.ZF), emitter.Cond(result));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
            emitter.Assign(orw.FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteAdda()
        {
            var width = di.Instruction.dataWidth;
            var op1 = orw.Rewrite(di.Instruction.op1);
            var op2 = orw.Rewrite(di.Instruction.op2);
            emitter.Assign(op2, emitter.Add(op2, op1));
            var postOp = di.Instruction.op1 as PostIncrementMemoryOperand;
            if (postOp != null)
                RewritePostOp(postOp);
        }

        private void RewritePostOp(PostIncrementMemoryOperand op)
        {
            var reg = frame.EnsureRegister(op.Register);
            emitter.Assign(reg, emitter.Add(reg, di.Instruction.dataWidth.Size));
        }
        public void RewriteMove(bool setFlag)
        {
            var op1 = orw.Rewrite(di.Instruction.op1);
            var op2 = orw.Rewrite(di.Instruction.op2);
            Copy(op1, op2);
        }

        private void Copy(Expression op1, Expression op2)
        {
            emitter.Assign(op1, op2);
        }
    }
}
