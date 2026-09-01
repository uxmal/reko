#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Mips.Rewriter
{
    public partial class MipsRewriter
    {
        private Identifier GetFpuRegPair(MachineOperand op)
        {
            var freg0 = (RegisterStorage)op;
            var ifreg = freg0.Number - arch.fpuRegs[0].Number;
            var freg1 = arch.fpuRegs[(ifreg + 1) & 0x1F];
            var seq = binder.EnsureSequence(
                PrimitiveType.Real64,
                freg0,
                freg1);
            return seq;
        }

        private void RewriteFpuBinopS(MipsInstruction instr, BinaryOperator op)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src1 = RewriteOperand(instr.Operands[1]);
            var src2 = RewriteOperand(instr.Operands[2]);
            m.Assign(dst, m.Bin(op, src1, src2));
        }

        private void RewriteFpuBinopD(MipsInstruction instr, BinaryOperator op)
        {
            var dst = GetFpuRegPair(instr.Operands[0]);
            var src1 = GetFpuRegPair(instr.Operands[1]);
            var src2 = GetFpuRegPair(instr.Operands[2]);
            m.Assign(dst, m.Bin(op, src1, src2));
        }

        private void RewriteMac_real(MipsInstruction instr, PrimitiveType dt, BinaryOperator accumFn)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var acc = RewriteOperand(instr.Operands[1]);
            var src1 = RewriteOperand(instr.Operands[2]);
            var src2 = RewriteOperand(instr.Operands[3]);
            var product = m.FMul(src1, src2);
            product.DataType = dt;
            var sum = m.Bin(accumFn, acc, product);
            sum.DataType = dt;
            m.Assign(dst, sum);
        }

        private void RewriteMaca_real(MipsInstruction instr, PrimitiveType dt, RegisterStorage? accReg, BinaryOperator accumFn)
        {
            if (accReg is null)
            {
                m.Invalid();
                return;
            }
            var acc = binder.EnsureRegister(accReg);
            var dst = binder.EnsureRegister(accReg);
            var src1 = RewriteOperand(instr.Operands[0]);
            var src2 = RewriteOperand(instr.Operands[1]);
            var product = m.FMul(src1, src2);
            product.DataType = dt;
            var sum = m.Bin(accumFn, acc, product);
            sum.DataType = dt;
            m.Assign(dst, sum);
        }


        private void RewriteMac_vec(MipsInstruction instr, PrimitiveType dt, BinaryOperator accFn)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var acc = RewriteOperand(instr.Operands[1]);
            var src1 = RewriteOperand(instr.Operands[2]);
            var src2 = RewriteOperand(instr.Operands[3]);
            var product = m.FMul(src1, src2);
            product.DataType = new ArrayType(dt, (int)(src1.DataType.BitSize / dt.BitSize));
            var sum = m.Bin(accFn, acc, product);
            sum.DataType = product.DataType;
            m.Assign(dst, sum);
        }

        private void RewriteNmac_real(MipsInstruction instr, PrimitiveType dt, BinaryOperator accFn)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var acc = RewriteOperand(instr.Operands[1]);
            var src1 = RewriteOperand(instr.Operands[2]);
            var src2 = RewriteOperand(instr.Operands[3]);
            var product = m.FMul(src1, src2);
            product.DataType = dt;
            var sum = m.Bin(accFn, acc, product);
            sum.DataType = dt;
            m.Assign(dst, m.FNeg(sum));
        }

        private void RewriteNmac_vec(MipsInstruction instr, PrimitiveType dt, BinaryOperator accFn)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var acc = RewriteOperand(instr.Operands[1]);
            var src1 = RewriteOperand(instr.Operands[2]);
            var src2 = RewriteOperand(instr.Operands[3]);
            var product = m.FMul(src1, src2);
            product.DataType = new ArrayType(dt, src1.DataType.BitSize / dt.BitSize);
            var sum = m.Bin(accFn, acc, product);
            sum.DataType = product.DataType;
            m.Assign(dst, m.FNeg(sum));
        }


        private void RewriteMovft(MipsInstruction instr, bool checkIfClear)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var src = RewriteOperand(instr.Operands[1]);
            var cc = RewriteOperand(instr.Operands[2]);
            m.BranchInMiddleOfInstruction(checkIfClear
                ? cc.Invert()
                : cc,
                instr.Address + instr.Length,
                InstrClass.CondJump);
            m.Assign(dst, src);
        }

        private void RewriteMulD(MipsInstruction instr)
        {
            var dst = GetFpuRegPair(instr.Operands[0]);
            var src1 = GetFpuRegPair(instr.Operands[1]);
            var src2 = GetFpuRegPair(instr.Operands[2]);
            m.Assign(dst, m.FMul(src1, src2));
        }

        private void RewriteFpuCmpD(MipsInstruction instr, BinaryOperator cmp)
        {
            m.Assign(
                RewriteOperand0(instr.Operands[0]),
                m.Bin(cmp, PrimitiveType.Bool,
                    GetFpuRegPair(instr.Operands[1]),
                    GetFpuRegPair(instr.Operands[2])));
        }

        private void RewriteCfc1(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr, 0),
                    RewriteOperand0(instr, 1));
        }

        private void RewriteCtc1(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr, 1),
                    RewriteOperand0(instr, 0));
        }

        private void RewriteCfc2(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr, 0),
                    RewriteOperand0(instr, 1));
        }

        private void RewriteCtc2(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr, 1),
                    RewriteOperand0(instr, 0));
        }


        private void RewriteCvtFromD(MipsInstruction instr, DataType dt)
        {
            var regPair = GetFpuRegPair(instr.Operands[1]);
            m.Assign(
                RewriteOperand0(instr.Operands[0]),
                m.Convert(regPair, regPair.DataType, dt));
        }

        private void RewriteCvtSW(MipsInstruction instr)
        {
            var src = RewriteOperand0(instr, 1);
            m.Assign(
                RewriteOperand(instr, 0),
                m.Convert(src, PrimitiveType.Int32, PrimitiveType.Real32));
        }

        private void RewriteCvtWS(MipsInstruction instr)
        {
            var src = m.Fn(FpOps.roundf, RewriteOperand0(instr, 1));
            m.Assign(
                RewriteOperand(instr, 0),
                m.Convert(src, PrimitiveType.Real32, PrimitiveType.Int32));
        }

        private void RewriteCvtToD(MipsInstruction instr, DataType dtSrc)
        {
            var regPair = GetFpuRegPair(instr.Operands[0]);
            var opSrc = RewriteOperand0(instr.Operands[1]);
            var dtDst = PrimitiveType.Create(Domain.Real, regPair.DataType.BitSize);
            m.Assign(
                regPair,
                m.Convert(opSrc, dtSrc, dtDst));
        }

        private void RewriteMfc1(MipsInstruction instr)
        {
            m.Assign(RewriteOperand0(instr.Operands[0]), RewriteOperand0(instr.Operands[1]));
        }

        private void RewriteMtc1(MipsInstruction instr)
        {
            m.Assign(RewriteOperand0(instr.Operands[1]), RewriteOperand0(instr.Operands[0]));
        }

        private void RewriteSdc1(MipsInstruction instr)
        {
            var src = GetFpuRegPair(instr.Operands[0]);
            var opDstMem = RewriteOperand(instr, 1);
            m.Assign(opDstMem, src);
        }

        private void RewriteTrunc(MipsInstruction instr, IntrinsicProcedure intrinsic, PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, RewriteOperand(instr.Operands[1]));
            var fn = m.Fn(intrinsic.MakeInstance(dtSrc), tmp);
            m.Assign(
                RewriteOperand(instr.Operands[0]),
                m.Convert(fn, fn.DataType, dtDst));
        }
    }
}
