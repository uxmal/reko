﻿#region License
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
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
            var seq = frame.EnsureSequence(
                freg0,
                freg1,
                PrimitiveType.Real64);
            return seq;
        }

        private void RewriteFpuBinopD(MipsInstruction instr, Func<Expression,Expression,Expression> ctor)
        {
            var dst = GetFpuRegPair(instr.op1);
            var src1 = GetFpuRegPair(instr.op2);
            var src2 = GetFpuRegPair(instr.op3);
            emitter.Assign(dst, ctor(src1, src2));
        }

        private void RewriteMulD(MipsInstruction instr)
        {
            var dst = GetFpuRegPair(instr.op1);
            var src1 = GetFpuRegPair(instr.op2);
            var src2 = GetFpuRegPair(instr.op3);
            emitter.Assign(dst, emitter.FMul(src1, src2));
        }

        private void RewriteFpuCmpD(MipsInstruction instr, Operator cmp)
        {
            emitter.Assign(
                RewriteOperand(instr.op1),
                new BinaryExpression(cmp, PrimitiveType.Bool,
                    GetFpuRegPair(instr.op2),
                    GetFpuRegPair(instr.op3)));
        }

        private void RewriteCfc1(MipsInstruction instr)
        {
            emitter.Assign(
                    RewriteOperand(instr.op1),
                    RewriteOperand(instr.op2));
        }

        private void RewriteCtc1(MipsInstruction instr)
        {
            emitter.Assign(
                    RewriteOperand(instr.op2),
                    RewriteOperand(instr.op1));
        }

        private void RewriteCvtD(MipsInstruction instr, DataType dt)
        {
            var regPair = GetFpuRegPair(instr.op2);
            emitter.Assign(
                RewriteOperand(instr.op1),
                emitter.Cast(dt, regPair));
        }

        private void RewriteMfc1(MipsInstruction instr)
        {
            emitter.Assign(RewriteOperand(instr.op1), RewriteOperand(instr.op2));
        }

        private void RewriteMtc1(MipsInstruction instr)
        {
            emitter.Assign(RewriteOperand(instr.op2), RewriteOperand(instr.op1));
        }

        private void RewriteTrunc(MipsInstruction instr, string fn, PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var tmp = frame.CreateTemporary(dtSrc);
            emitter.Assign(tmp, RewriteOperand(instr.op2));
            var ppp = host.PseudoProcedure(fn, dtSrc, tmp);
            emitter.Assign(
                RewriteOperand(instr.op1),
                emitter.Cast(dtDst, ppp));
        }
    }
}
