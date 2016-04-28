#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.Arch.Arm
{
    public partial class ArmRewriter
    {
        private void RewriteBinOp(Operator op, bool setflags)
        {
            var opDst = this.Operand(Dst);
            var opSrc1 = this.Operand(Src1);
            var opSrc2 = this.Operand(Src2);
            ConditionalAssign(opDst, new BinaryExpression(op, PrimitiveType.Word32, opSrc1, opSrc2));
            if (setflags)
            {
                ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "SZCO", PrimitiveType.Byte), emitter.Cond(opDst));
            }
        }

        private void RewriteRevBinOp(Operator op, bool setflags)
        {
            var ops = instr.ArchitectureDetail.Operands.ToArray();
            var opDst = this.Operand(Dst);
            var opSrc1 = this.Operand(Src1);
            var opSrc2 = this.Operand(Src2);
            ConditionalAssign(opDst, new BinaryExpression(op, PrimitiveType.Word32, opSrc1, opSrc2));
            if (setflags)
            {
                ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "SZCO", PrimitiveType.Byte), emitter.Cond(opDst));
            }
        }

        private void RewriteUnaryOp(UnaryOperator op)
        {
            var opDst = this.Operand(Dst);
            var opSrc = this.Operand(Src1);
            ConditionalAssign(opDst, new UnaryExpression(op,  PrimitiveType.Word32, opSrc));
            if (instr.ArchitectureDetail.UpdateFlags)
            {
                ConditionalAssign(frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "SZCO", PrimitiveType.Byte), emitter.Cond(opDst));
            }
        }

        private void RewriteBic()
        {
            var opDst = this.Operand(Dst);
            var opSrc1 = this.Operand(Src1);
            var opSrc2 = this.Operand(Src2);
            ConditionalAssign(opDst, emitter.And(opSrc1, emitter.Comp(opSrc2)));
        }

        private void RewriteCmn()
        {
            var opDst = this.Operand(Dst);
            var opSrc = this.Operand(Src1);
            ConditionalAssign(
                frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(
                    emitter.IAdd(opDst, opSrc)));
        }

        private void RewriteCmp()
        {
            var opDst = this.Operand(Dst);
            var opSrc = this.Operand(Src1);
            ConditionalAssign(
                frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(
                    emitter.ISub(opDst, opSrc)));
        }

        private void RewriteTeq()
        {
            var opDst = this.Operand(Dst);
            var opSrc = this.Operand(Src1);
            emitter.Assign(
                frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(emitter.Xor(opDst, opSrc)));
        }

        private void RewriteTst()
        {
            var opDst = this.Operand(Dst);
            var opSrc = this.Operand(Src1);
            emitter.Assign(
                frame.EnsureFlagGroup(A32Registers.cpsr, 0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(emitter.And(opDst, opSrc)));
        }

        private void RewriteLdr(DataType size)
        {
            var opSrc = this.Operand(Src1);
            var opDst = this.Operand(Dst);
            Identifier dst = (Identifier)opDst;
            var rDst = dst.Storage as RegisterStorage;
            if (rDst == A32Registers.pc)
            {
                // Assignment to PC is the same as a jump
                ric.Class = RtlClass.Transfer;
                emitter.Goto(opSrc);
                return;
            }
            emitter.Assign(opDst, opSrc);
            MaybePostOperand(Src1);
        }

        private void RewriteStr(DataType size)
        {
            var opSrc = this.Operand(Dst);
            var opDst = this.Operand(Src1);
            emitter.Assign(opDst, opSrc);
            MaybePostOperand(Src1);
        }

        private void RewriteMov()
        {
            if (Dst.Type == ArmInstructionOperandType.Register && Dst.RegisterValue.Value == ArmRegister.PC)
            {
                ric.Class = RtlClass.Transfer;
                if (Src1.Type == ArmInstructionOperandType.Register && Src1.RegisterValue.Value == ArmRegister.LR)
                {
                    AddConditional(new RtlReturn(0, 0, RtlClass.Transfer));
                }
                else
                {
                    AddConditional(new RtlGoto(Operand(Src1), RtlClass.Transfer));
                }
                return;
            }
            var opDst = Operand(Dst);
            var opSrc = Operand(Src1);
            ConditionalAssign(opDst, opSrc);
        }

        private void RewriteLdm()
        {
            throw new NotImplementedException();
#if NYI
            var dst = frame.EnsureRegister(((RegisterOperand) Dst).Register);
            var range = (RegisterRangeOperand) Src1;
            int offset = 0;
            bool pcRestored = false;
            foreach (var r in range.GetRegisters().Reverse())
            {
                Expression ea = offset != 0
                    ? emitter.IAdd(dst, offset)
                    : (Expression) dst;
                var reg = arch.GetRegister(r);
                var srcReg = frame.EnsureRegister(reg);
                emitter.Assign(srcReg, emitter.LoadDw(ea));
                offset += srcReg.DataType.Size;
                if (reg == A32Registers.pc)
                    pcRestored = true;
            }
            if (offset != 0 && instr.Update)
            {
                emitter.Assign(dst, emitter.IAdd(dst, offset));
            }
            //$REVIEW: most likely case.
            if (pcRestored)
                emitter.Return(0, 0);
#endif
        }

        private void RewritePush()
        {
            int offset = 0;
            var dst = frame.EnsureRegister(A32Registers.sp);
            foreach (var op in instr.ArchitectureDetail.Operands)
            {
                Expression ea = offset != 0
                    ? emitter.ISub(dst, offset)
                    : (Expression)dst;
                var reg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[op.RegisterValue.Value]);
                emitter.Assign(emitter.LoadDw(ea), reg);
                offset += reg.DataType.Size;
            }
            emitter.Assign(dst, emitter.ISub(dst, offset));
        }

        private void RewriteStm()
        {
            var dst = this.Operand(Dst);
            var range = instr.ArchitectureDetail.Operands.Skip(1);
            int offset = 0;
            foreach (var r in range)
            {
                Expression ea = offset != 0
                    ? emitter.ISub(dst, offset)
                    : (Expression) dst;
                var srcReg = frame.EnsureRegister(A32Registers.RegisterByCapstoneID[r.RegisterValue.Value]);
                emitter.Assign(emitter.LoadDw(ea), srcReg);
                offset += srcReg.DataType.Size;
            }
            if (offset != 0 && instr.ArchitectureDetail.WriteBack)
            {
                emitter.Assign(dst, emitter.ISub(dst, offset));
            }
        }

        private void RewriteStmib()
        {
            throw new NotImplementedException();
#if NYI
            var dst = frame.EnsureRegister(((RegisterOperand)Dst).Register);
            var range = (RegisterRangeOperand)Src1;
            int offset = 0;
            foreach (var r in range.GetRegisters())
            {
                var srcReg = frame.EnsureRegister(arch.GetRegister(r));
                offset += srcReg.DataType.Size;
                Expression ea = offset != 0
                    ? emitter.ISub(dst, offset)
                    : (Expression)dst;
                emitter.Assign(emitter.LoadDw(ea), srcReg);
            }
            if (offset != 0 && instr.Update)
            {
                emitter.Assign(dst, emitter.ISub(dst, offset));
            }
#endif
        }
    }
}