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

using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System.Linq;

namespace Decompiler.Arch.Arm
{
    public partial class ArmRewriter
    {
        private void RewriteBinOp(Operator op, bool setflags)
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc1 = this.Operand(instr.Src1);
            var opSrc2 = this.Operand(instr.Src2);
            ConditionalAssign(opDst, new BinaryExpression(op, PrimitiveType.Word32, opSrc1, opSrc2));
            if (setflags)
            {
                ConditionalAssign(frame.EnsureFlagGroup(0x1111, "SZCO", PrimitiveType.Byte), emitter.Cond(opDst));
            }
        }

        private void RewriteUnaryOp(UnaryOperator op)
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc = this.Operand(instr.Src1);
            ConditionalAssign(opDst, new UnaryExpression(op,  PrimitiveType.Word32, opSrc));
            if (instr.OpFlags == OpFlags.S)
            {
                ConditionalAssign(frame.EnsureFlagGroup(0x1111, "SZCO", PrimitiveType.Byte), emitter.Cond(opDst));
            }
        }

        private void RewriteBic()
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc1 = this.Operand(instr.Src1);
            var opSrc2 = this.Operand(instr.Src2);
            ConditionalAssign(opDst, emitter.And(opSrc1, emitter.Comp(opSrc2)));
        }

        private void RewriteCmn()
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc = this.Operand(instr.Src1);
            ConditionalAssign(
                frame.EnsureFlagGroup(0x1111, "SZCO", PrimitiveType.Byte),
                emitter.Cond(
                    emitter.IAdd(opDst, opSrc)));
        }

        private void RewriteCmp()
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc = this.Operand(instr.Src1);
            ConditionalAssign(
                frame.EnsureFlagGroup(0x1111, "SZCO", PrimitiveType.Byte),
                emitter.Cond(
                    emitter.ISub(opDst, opSrc)));
        }

        private void RewriteTeq()
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc = this.Operand(instr.Src1);
            emitter.Assign(
                frame.EnsureFlagGroup(0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(emitter.Xor(opDst, opSrc)));
        }

        private void RewriteTst()
        {
            var opDst = this.Operand(instr.Dst);
            var opSrc = this.Operand(instr.Src1);
            emitter.Assign(
                frame.EnsureFlagGroup(0x1111, "NZCV", PrimitiveType.Byte),
                emitter.Cond(emitter.And(opDst, opSrc)));
        }

        private void RewriteLdr(DataType size)
        {
            var opSrc = this.Operand(instr.Src1);
            var opDst = this.Operand(instr.Dst);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteStr(DataType size)
        {
            var opSrc = this.Operand(instr.Dst);
            var opDst = this.Operand(instr.Src1);
            emitter.Assign(opDst, opSrc);
        }

        private void RewriteMov()
        {
            var regDst = instr.Dst as RegisterOperand;
            if (regDst != null && regDst.Register == A32Registers.pc)
            {
                var regSrc = instr.Src1 as RegisterOperand;
                if (regSrc != null && regSrc.Register == A32Registers.lr)
                {
                    AddConditional(new RtlReturn(0, 0, RtlClass.Transfer));
                }
                else
                {
                    AddConditional(new RtlGoto(Operand(instr.Src1), RtlClass.Transfer));
                }
                return;
            }
            var opDst = Operand(instr.Dst);
            var opSrc = Operand(instr.Src1);
            ConditionalAssign(opDst, opSrc);
        }

        private void RewriteLdm()
        {
            var dst = frame.EnsureRegister(((RegisterOperand) instr.Dst).Register);
            var range = (RegisterRangeOperand) instr.Src1;
            int offset = 0;
            bool pcRestored = false;
            foreach (var r in range.GetRegisters().Reverse())
            {
                Expression ea = offset != 0
                    ? emitter.IAdd(dst, offset)
                    : (Expression) dst;
                var reg = arch.GetRegister(r);
                var srcReg = frame.EnsureRegister(reg);
                emitter.Assign(emitter.LoadDw(ea), srcReg);
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
        }

        private void RewriteStm()
        {
            var dst = frame.EnsureRegister(((RegisterOperand) instr.Dst).Register);
            var range = (RegisterRangeOperand) instr.Src1;
            int offset = 0;
            foreach (var r in range.GetRegisters())
            {
                Expression ea = offset != 0
                    ? emitter.ISub(dst, offset)
                    : (Expression) dst;
                var srcReg = frame.EnsureRegister(arch.GetRegister(r));
                emitter.Assign(emitter.LoadDw(ea), srcReg);
                offset += srcReg.DataType.Size;
            }
            if (offset != 0 && instr.Update)
            {
                emitter.Assign(dst, emitter.ISub(dst, offset));
            }
        }
    }
}