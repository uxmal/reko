using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Intel
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

        private void RewriteFild()
        {
            state.GrowFpuStack(di.Address);
            emitter.Assign(
                orw.FpuRegister(0, state),
                emitter.Cast(PrimitiveType.Real64, SrcOp(di.Instruction.op1)));
            WriteFpuStack(0);
        }

        private void RewriteFistp()
        {
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
            state.GrowFpuStack(di.Address);
            emitter.Assign(FpuRegister(0), new Constant(constant));
            WriteFpuStack(0);
        }

        private void RewriteFst(bool pop)
        {
            EmitCopy(di.Instruction.op1, FpuRegister(0), false);
            if (pop)
                state.ShrinkFpuStack(1);
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
