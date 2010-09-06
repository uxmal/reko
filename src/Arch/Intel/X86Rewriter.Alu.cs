#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Intel
{
    public partial class X86Rewriter
    {
        private void RewriteMov()
        {
            EmitCopy(di.Instruction.op1, SrcOp(di.Instruction.op2, di.Instruction.op1.Width), false);
        }


        /// <summary>
		/// Doesn't handle the x86 idiom add ... adc => long add (and 
		/// sub ..sbc => long sub)
		/// </summary>
		/// <param name="i"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		public Expression RewriteAddSub(BinaryOperator op)
		{
            //LongAddRewriter larw = new LongAddRewriter(this.frame, orw, state);
            //int iUse = larw.IndexOfUsingOpcode(instrs, i, next);
            //if (iUse >= 0 && larw.Match(instrCur, instrs[iUse]))
            //{
            //    instrs[iUse].code = Opcode.nop;
            //    larw.EmitInstruction(op, emitter);
            //    return larw.Dst;
            //}
			Assignment ass = EmitBinOp(
                op,
                di.Instruction.op1,
                di.Instruction.op1.Width,
                SrcOp(di.Instruction.op1),
                SrcOp(di.Instruction.op2));
            EmitCcInstr(ass.Dst, IntelInstruction.DefCc(di.Instruction.code));
            return ass.Dst;
		}

        public Assignment EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, Expression left, Expression right)
        {
            Constant c = right as Constant;
            if (c != null)
            {
                if (c.DataType == PrimitiveType.Byte && left.DataType != c.DataType)
                {
                    right = emitter.Const(left.DataType, c.ToInt32());
                }
            }

            return EmitCopy(dst, new BinaryExpression(binOp, dtDst, left, right), true);
        }

        /// <summary>
        /// Emits an assignment to a flag-group pseudoregister.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="defFlags">flags defined by the intel instruction</param>
        private void EmitCcInstr(Expression expr, FlagM defFlags)
        {
            emitter.Assign(orw.FlagGroup(defFlags), new ConditionOf(expr.CloneExpression()));
        }

        private void RewriteLogical(BinaryOperator op)
        {
            var ass = EmitBinOp(
                op,
                di.Instruction.op1,
                di.Instruction.op1.Width,
                SrcOp(di.Instruction.op1),
                SrcOp(di.Instruction.op2));
            EmitCcInstr(ass.Dst, (IntelInstruction.DefCc(di.Instruction.code)& ~FlagM.CF));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }
    }
}
