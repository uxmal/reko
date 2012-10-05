#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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
using System.Text;

namespace Decompiler.Arch.X86
{
    public partial class X86Rewriter
    {
        private int maxFpuStackWrite;

        public void EmitCommonFpuInstruction(
            BinaryOperator op,
            bool fReversed,
            bool fPopStack)
        {
            EmitCommonFpuInstruction(op, fReversed, fPopStack, null);
        }

        public void EmitCommonFpuInstruction(
            BinaryOperator op,
            bool fReversed,
            bool fPopStack,
            DataType cast)
        {
            switch (di.Instruction.Operands)
            {
            default:
                throw new ArgumentOutOfRangeException("di.Instruction", "Instruction must have 1 or 2 operands");
            case 1:
                {
                    // implicit st(0) operand.
                    Identifier opLeft = FpuRegister(0);
                    Expression opRight = SrcOp(di.Instruction.op1);
                    if (fReversed)
                    {
                        EmitCopy(di.Instruction.op1, new BinaryExpression(op, di.Instruction.dataWidth, opRight, MaybeCast(cast, opLeft)), true);
                    }
                    else
                    {
                        emitter.Assign(opLeft, new BinaryExpression(op, di.Instruction.dataWidth, opLeft, MaybeCast(cast, opRight)));
                    }
                    break;
                }
            case 2:
                {
                    Expression op1 = SrcOp(di.Instruction.op1);
                    Expression op2 = SrcOp(di.Instruction.op2);
                    EmitCopy(
                        di.Instruction.op1,
                        new BinaryExpression(op,
                        di.Instruction.op1.Width,
                        fReversed ? op2 : op1,
                        fReversed ? op1 : op2), false);
                    break;
                }
            }

            if (fPopStack)
            {
                state.ShrinkFpuStack(1);
            }
        }

        private void EmitFchs()
        {
            emitter.Assign(
                orw.FpuRegister(0, state),
                emitter.Neg(orw.FpuRegister(0, state)));		//$BUGBUG: should be Real, since we don't know the actual size.
            WriteFpuStack(0);
        }

        private void RewriteFclex()
        {
            emitter.SideEffect(PseudoProc("__fclex", PrimitiveType.Void));
        }

        private void RewriteFcom(int pops)
        {
            Identifier op1 = FpuRegister(0);
            Expression op2 = (di.Instruction.code == Opcode.fcompp)
                ? FpuRegister(1)
                : SrcOp(di.Instruction.op1);
            emitter.Assign(
                orw.FlagGroup(FlagM.FPUF),
                new ConditionOf(
                    new BinaryExpression(Operator.Sub, di.Instruction.dataWidth, op1, op2)));
            state.ShrinkFpuStack(pops);
        }

        private void RewriteFUnary(string name)
        {
            emitter.Assign(
                orw.FpuRegister(0, state),
                PseudoProc(name, PrimitiveType.Real64, orw.FpuRegister(0, state)));
            WriteFpuStack(0);
        }

        private void RewriteFild()
        {
            state.GrowFpuStack(di.Address);
            var iType = PrimitiveType.Create(Domain.SignedInt, di.Instruction.op1.Width.Size);
            emitter.Assign(
                orw.FpuRegister(0, state),
                emitter.Cast(PrimitiveType.Real64, SrcOp(di.Instruction.op1, iType)));
            WriteFpuStack(0);
        }

        private void RewriteFistp()
        {
            di.Instruction.op1.Width = PrimitiveType.Create(Domain.SignedInt, di.Instruction.op1.Width.Size);
            EmitCopy(di.Instruction.op1, emitter.Cast(di.Instruction.op1.Width, orw.FpuRegister(0, state)), false);
            state.ShrinkFpuStack(1);
        }

        public void RewriteFld()
        {
            state.GrowFpuStack(di.Address);
            emitter.Assign(FpuRegister(0), SrcOp(di.Instruction.op1));
            WriteFpuStack(0);
        }

        private void RewriteFldConst(double constant)
        {
            RewriteFldConst(new Constant(constant));
        }

        private void RewriteFldConst(Constant c)
        {
            state.GrowFpuStack(di.Address);
            emitter.Assign(FpuRegister(0), c);
            WriteFpuStack(0);
        }
        private void RewriteFldcw()
        {
            emitter.SideEffect(PseudoProc(
                "__fldcw",
                PrimitiveType.Void,
                SrcOp(di.Instruction.op1)));
        }

        private void RewriteFpatan()
        {
            Expression op1 = FpuRegister(1);
            Expression op2 = FpuRegister(0);
            state.ShrinkFpuStack(1);
            emitter.Assign(FpuRegister(0), PseudoProc("atan", PrimitiveType.Real64, op1, op2));
            WriteFpuStack(0);
        }

        private void RewriteFsincos()
        {
            Identifier itmp = frame.CreateTemporary(PrimitiveType.Real64);
            emitter.Assign(itmp, FpuRegister(0));

            state.GrowFpuStack(di.Address);
            emitter.Assign(FpuRegister(1), PseudoProc("cos", PrimitiveType.Real64, itmp));
            emitter.Assign(FpuRegister(0), PseudoProc("sin", PrimitiveType.Real64, itmp));
            WriteFpuStack(0);
            WriteFpuStack(1);
        }

        private void RewriteFst(bool pop)
        {
            EmitCopy(di.Instruction.op1, FpuRegister(0), false);
            if (pop)
                state.ShrinkFpuStack(1);
        }

        private void RewriterFstcw()
        {
			EmitCopy(
                di.Instruction.op1, 
                PseudoProc("__fstcw", PrimitiveType.UInt16),
                false);
        }

        private void RewriteFstsw()
        {
            EmitCopy(
                di.Instruction.op1,
                new BinaryExpression(Operator.Shl, PrimitiveType.Word16,
                        new Cast(PrimitiveType.Word16, orw.FlagGroup(FlagM.FPUF)),
                        new Constant(PrimitiveType.Int16, 8)),
                false);
        }

        private void RewriteFtst()
        {
            emitter.Assign(orw.FlagGroup(FlagM.CF),
                emitter.Sub(FpuRegister(0), new Constant(0.0)));
        }

        private void RewriteFxam()
        {
            //$TODO: need to make this an assignment to C0|C1|C2|C3 = __fxam();
            // idiomatically followed by fstsw &c.
            emitter.SideEffect(PseudoProc("__fxam", PrimitiveType.Byte));
        }

        private void RewriteFyl2x()
        {
            //$REVIEW: Candidate for idiom search.
            Identifier op1 = FpuRegister(0);
            Identifier op2 = FpuRegister(1);
            emitter.Assign(op1, emitter.Sub(op2, PseudoProc("lg2", PrimitiveType.Real64, op1)));
            state.ShrinkFpuStack(1);
            WriteFpuStack(0);
        }

        private Identifier FpuRegister(int reg)
        {
            return orw.FpuRegister(reg, state);
        }


        public Expression MaybeCast(DataType type, Expression e)
        {
            if (type != null)
                return new Cast(type, e);
            else
                return e;
        }

        private void WriteFpuStack(int offset)
        {
            int o = offset - state.FpuStackItems;
            if (o > maxFpuStackWrite)
                maxFpuStackWrite = o;
        }
    }
}
