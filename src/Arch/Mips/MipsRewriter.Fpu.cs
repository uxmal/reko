#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private Identifier GetFpuRegPair(MachineOperand op)
        {
            var freg0 = (RegisterStorage)op;
            var freg1 = arch.fpuRegs[1 + (freg0.Number & 0x1F)];
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

        private void RewriteMac_vec(MipsInstruction instr, PrimitiveType dt, BinaryOperator accFn)
        {
            var dst = RewriteOperand(instr.Operands[0]);
            var acc = RewriteOperand(instr.Operands[1]);
            var src1 = RewriteOperand(instr.Operands[2]);
            var src2 = RewriteOperand(instr.Operands[3]);
            var product = m.FMul(src1, src2);
            product.DataType = new ArrayType(dt, src1.DataType.BitSize / dt.BitSize);
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
                InstrClass.ConditionalTransfer);
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
                    RewriteOperand(instr.Operands[0]),
                    RewriteOperand0(instr.Operands[1]));
        }

        private void RewriteCtc1(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr.Operands[1]),
                    RewriteOperand0(instr.Operands[0]));
        }

        private void RewriteCvtFromD(MipsInstruction instr, DataType dt)
        {
            var regPair = GetFpuRegPair(instr.Operands[1]);
            m.Assign(
                RewriteOperand0(instr.Operands[0]),
                m.Convert(regPair, regPair.DataType, dt));
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
