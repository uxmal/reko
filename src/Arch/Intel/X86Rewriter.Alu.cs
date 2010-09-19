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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Arch.Intel
{
    public partial class X86Rewriter
    {
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
            RtlAssignment ass = EmitBinOp(
                op,
                di.Instruction.op1,
                di.Instruction.op1.Width,
                SrcOp(di.Instruction.op1),
                SrcOp(di.Instruction.op2));
            EmitCcInstr(ass.Dst, IntelInstruction.DefCc(di.Instruction.code));
            return ass.Dst;
        }

        public void RewriteBswap()
        {
            Identifier reg = (Identifier)orw.AluRegister(((RegisterOperand)di.Instruction.op1).Register);
            emitter.Assign(reg, PseudoProc("__bswap", (PrimitiveType)reg.DataType, reg));
        }

        public RtlAssignment EmitBinOp(BinaryOperator binOp, MachineOperand dst, DataType dtDst, Expression left, Expression right)
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

        private void RewriteCmp()
        {
            Expression op1 = SrcOp(di.Instruction.op1);
            Expression op2 = SrcOp(di.Instruction.op2, di.Instruction.op1.Width);
            emitter.Assign(
                orw.FlagGroup(IntelInstruction.DefCc(Opcode.cmp)),
                new ConditionOf(emitter.Sub(op1, op2)));
        }

        private void RewriteLogical(BinaryOperator op)
        {
            var ass = EmitBinOp(
                op,
                di.Instruction.op1,
                di.Instruction.op1.Width,
                SrcOp(di.Instruction.op1),
                SrcOp(di.Instruction.op2));
            EmitCcInstr(ass.Dst, (IntelInstruction.DefCc(di.Instruction.code) & ~FlagM.CF));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewriteIn()
        {
            var ppName = "__in" + IntelSizeSuffix(di.Instruction.op1.Width.Size);
            EmitCopy(
                di.Instruction.op1,
                PseudoProc(ppName, di.Instruction.op1.Width, SrcOp(di.Instruction.op2)), false);
        }

        private string IntelSizeSuffix(int size)
        {
            switch (size)
            {
            case 1: return "b";
            case 2: return "w";
            case 4: return "dw";
            case 8: return "qw";
            default: throw new ArgumentOutOfRangeException("Size is not 1,2,4 or 8");
            }
        }

        private void RewriteMov()
        {
            EmitCopy(di.Instruction.op1, SrcOp(di.Instruction.op2, di.Instruction.op1.Width), false);
        }

        private void RewritePush()
        {
            RegisterOperand reg = di.Instruction.op1 as RegisterOperand;
            if (reg != null && reg.Register == Registers.cs)
            {
                if (dasm.Peek(1).Instruction.code == Opcode.call &&
                    dasm.Peek(1).Instruction.op1.Width == PrimitiveType.Word16)
                {
                    dasm.MoveNext();
                    emitter.Assign(StackPointer(), emitter.Sub(StackPointer(), reg.Register.DataType.Size));
                    throw new NotImplementedException("RewriteCall(dasm.Current.Instruction.op1, PrimitiveType.Word32)");
                    return;
                }

                if (
                    dasm.Peek(1).Instruction.code == Opcode.push &&
                    (dasm.Peek(1).Instruction.op1 is ImmediateOperand) &&
                    dasm.Peek(2).Instruction.code == Opcode.push &&
                    (dasm.Peek(2).Instruction.op1 is ImmediateOperand) &&
                    dasm.Peek(3).Instruction.code == Opcode.jmp &&
                    (dasm.Peek(3).Instruction.op1 is AddressOperand))
                {
                    // That's actually a far call, but the callee thinks its a near call.
                    throw new NotImplementedException(" EmitCall(((AddressOperand) dasm.Peek(3).Instruction.op1).addr, 2, true);");
                    dasm.MoveNext();
                    dasm.MoveNext();
                    dasm.MoveNext();
                    return;
                }
            }
            Debug.Assert(dasm.Current.Instruction.dataWidth == PrimitiveType.Word16 || dasm.Current.Instruction.dataWidth == PrimitiveType.Word32);
            RewritePush(dasm.Current.Instruction.dataWidth, SrcOp(dasm.Current.Instruction.op1));
        }


        private void RewritePop()
        {
            RewritePop(dasm.Current.Instruction.op1, dasm.Current.Instruction.op1.Width);
        }

        private void RewritePop(MachineOperand op, PrimitiveType width)
        {
            var sp = StackPointer();
            EmitCopy(op, orw.StackAccess(sp, width), false);
            emitter.Assign(sp, emitter.Add(sp, width.Size));
        }

        private void RewritePush(PrimitiveType dataWidth, Expression expr)
        {
            Constant c = expr as Constant;
            if (c != null && c.DataType != dataWidth)
            {
                expr = new Constant(dataWidth, c.ToInt64());
            }

            // Allocate an local variable for the push.

            var sp = StackPointer();
            emitter.Assign(sp, emitter.Sub(sp, dataWidth.Size));
            emitter.Assign(orw.StackAccess(sp, dataWidth), expr);
        }

        private void RewriteTest()
        {
            var src = new BinaryExpression(Operator.And,
                di.Instruction.op1.Width,
                SrcOp(di.Instruction.op1),
                                SrcOp(di.Instruction.op2));

            EmitCcInstr(src, (IntelInstruction.DefCc(di.Instruction.code) & ~FlagM.CF));
            emitter.Assign(orw.FlagGroup(FlagM.CF), Constant.False());
        }

        private Identifier StackPointer()
        {
            return frame.EnsureRegister(arch.ProcessorMode.StackRegister);
        }
    }
}
