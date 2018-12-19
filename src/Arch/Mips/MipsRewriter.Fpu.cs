#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Mips
{
    public partial class MipsRewriter
    {
        private Identifier GetFpuRegPair(MachineOperand op)
        {
            var freg0 = ((RegisterOperand)op).Register;
            var freg1 = arch.fpuRegs[1 + (freg0.Number & 0x1F)];
            var seq = binder.EnsureSequence(
                PrimitiveType.Real64,
                freg0,
                freg1);
            return seq;
        }

        private void RewriteFpuBinopS(MipsInstruction instr, Func<Expression, Expression, Expression> ctor)
        {
            var dst = RewriteOperand(instr.op1);
            var src1 = RewriteOperand(instr.op2);
            var src2 = RewriteOperand(instr.op3);
            m.Assign(dst, ctor(src1, src2));
        }

        private void RewriteFpuBinopD(MipsInstruction instr, Func<Expression,Expression,Expression> ctor)
        {
            var dst = GetFpuRegPair(instr.op1);
            var src1 = GetFpuRegPair(instr.op2);
            var src2 = GetFpuRegPair(instr.op3);
            m.Assign(dst, ctor(src1, src2));
        }

        private void RewriteMovft(MipsInstruction instr, bool checkIfClear)
        {
            var dst = RewriteOperand(instr.op1);
            var src = RewriteOperand(instr.op2);
            var cc = RewriteOperand(instr.op3);
            m.BranchInMiddleOfInstruction(checkIfClear
                ? cc.Invert()
                : cc,
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Assign(dst, src);
        }

        private void RewriteMulD(MipsInstruction instr)
        {
            var dst = GetFpuRegPair(instr.op1);
            var src1 = GetFpuRegPair(instr.op2);
            var src2 = GetFpuRegPair(instr.op3);
            m.Assign(dst, m.FMul(src1, src2));
        }

        private void RewriteFpuCmpD(MipsInstruction instr, Operator cmp)
        {
            m.Assign(
                RewriteOperand0(instr.op1),
                new BinaryExpression(cmp, PrimitiveType.Bool,
                    GetFpuRegPair(instr.op2),
                    GetFpuRegPair(instr.op3)));
        }

        private void RewriteCfc1(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr.op1),
                    RewriteOperand0(instr.op2));
        }

        private void RewriteCtc1(MipsInstruction instr)
        {
            m.Assign(
                    RewriteOperand(instr.op2),
                    RewriteOperand0(instr.op1));
        }

        private void RewriteCvtD(MipsInstruction instr, DataType dt)
        {
            var regPair = GetFpuRegPair(instr.op2);
            m.Assign(
                RewriteOperand0(instr.op1),
                m.Cast(dt, regPair));
        }

        private void RewriteMfc1(MipsInstruction instr)
        {
            m.Assign(RewriteOperand0(instr.op1), RewriteOperand0(instr.op2));
        }

        private void RewriteMtc1(MipsInstruction instr)
        {
            m.Assign(RewriteOperand0(instr.op2), RewriteOperand0(instr.op1));
        }

        private void RewriteTrunc(MipsInstruction instr, string fn, PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var tmp = binder.CreateTemporary(dtSrc);
            m.Assign(tmp, RewriteOperand(instr.op2));
            var ppp = host.PseudoProcedure(fn, dtSrc, tmp);
            m.Assign(
                RewriteOperand(instr.op1),
                m.Cast(dtDst, ppp));
        }
    }
}
