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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch64
{
    public partial class A64Rewriter
    {
        private void RewriteAdrp()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var wBase = instr.Address.ToLinear();
            wBase &= ~0xFFFul;        // Mask out lowest 12 bits.
            wBase = (ulong)((long)wBase + imm.ToInt64());
            m.Assign(dst, Address.Ptr64(wBase));
        }

        private void RewriteBinary(Func<Expression, Expression, Expression> fn, Action<Expression> setFlags = null)
        {
            var dst = RewriteOp(instr.ops[0]);
            var left = RewriteOp(instr.ops[1]);
            var right = RewriteOp(instr.ops[2]);
            if (instr.shiftCode != Opcode.Invalid &&
                (instr.shiftCode != Opcode.lsl ||
                !(instr.shiftAmount is ImmediateOperand imm) ||
                !imm.Value.IsIntegerZero))
            {
                var amt = RewriteOp(instr.shiftAmount);
                switch (instr.shiftCode)
                {
                case Opcode.lsl: right = m.Shl(right, amt); break;
                default: throw new NotImplementedException($"Shift operation {instr.shiftCode} not implemented yet.");
                }
            }
            m.Assign(dst, fn(left, right));
            if (setFlags != null)
            {
                setFlags(m.Cond(dst));
            }
        }

        private void RewriteCcmp()
        {
            var nzcv = NZCV();
            var tmp = binder.CreateTemporary(PrimitiveType.Bool);
            var cond = Invert(((ConditionOperand)instr.ops[3]).Condition);
            m.Assign(tmp, this.TestCond(cond));
            m.Assign(nzcv, RewriteOp(instr.ops[2]));
            m.BranchInMiddleOfInstruction(tmp, instr.Address + instr.Length, RtlClass.ConditionalTransfer);
            var left = RewriteOp(instr.ops[0]);
            var right = RewriteOp(instr.ops[1]);
            m.Assign(nzcv, m.Cond(m.ISub(left, right)));
        }

        private void RewriteCsinc()
        {
            var dst = RewriteOp(instr.ops[0]);
            var rTrue = ((RegisterOperand)instr.ops[1]).Register;
            var rFalse = ((RegisterOperand)instr.ops[2]).Register;
            var cond = ((ConditionOperand)instr.ops[3]).Condition;
            if (rTrue.Number == 31 && rFalse.Number == 31)
            {
                m.Assign(dst, m.Cast(dst.DataType, TestCond(Invert(cond))));
                return;
            }
            NotImplementedYet();
        }

        private void RewriteLdr(DataType dt)
        {
            var dst = RewriteOp(instr.ops[0]);
            var mem = (MemoryOperand)instr.ops[1];
            Expression ea = RewriteEffectiveAddress(mem);
            if (dt == null)
            {
                m.Assign(dst, m.Mem(dst.DataType, ea));
            }
            else
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Mem(dt, ea));
                m.Assign(dst, m.Cast(dst.DataType, tmp));
            }
        }

        private void RewriteMovk()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var shift = ((ImmediateOperand)instr.shiftAmount).Value;
            m.Assign(dst, m.Dpb(dst, imm, shift.ToInt32()));
        }

        private void RewriteMovz()
        {
            var dst = RewriteOp(instr.ops[0]);
            var imm = ((ImmediateOperand)instr.ops[1]).Value;
            var shift = ((ImmediateOperand)instr.shiftAmount).Value;
            m.Assign(dst, Constant.Word(dst.DataType.BitSize, imm.ToInt64() << shift.ToInt32()));
        }

        private Expression RewriteEffectiveAddress(MemoryOperand mem)
        {
            Expression ea = binder.EnsureRegister(mem.Base);
            if (mem.Offset != null && !mem.Offset.IsIntegerZero)
            {
                if (mem.Offset.DataType.BitSize < 64 && mem.Offset.IsNegative)
                {
                    ea = m.ISub(ea, -mem.Offset.ToInt32());
                }
                else
                {
                    ea = m.IAdd(ea, m.Int32(mem.Offset.ToInt32()));
                }
            }
            else if (mem.Index != null)
            {
                Expression idx = binder.EnsureRegister(mem.Index);
                switch (mem.IndexExtend)
                {
                case Opcode.lsl:
                    if (mem.IndexShift != 0)
                    {
                        var dtInt = PrimitiveType.Create(Domain.SignedInt, idx.DataType.BitSize);
                        idx = m.IMul(idx, Constant.Create(dtInt, 1 << mem.IndexShift));
                    }
                    break;
                }
                ea = m.IAdd(ea, idx);
            }

            return ea;
        }

        private void RewriteStr(DataType dt)
        {
            var src = RewriteOp(instr.ops[0]);
            var mem = (MemoryOperand)instr.ops[1];
            Expression ea = RewriteEffectiveAddress(mem);
            if (dt == null)
            {
                m.Assign(m.Mem(src.DataType, ea), src);
            }
            else
            {
                m.Assign(m.Mem(dt, ea), m.Cast(dt, src));
            }
        }

        private void RewriteUnary(Func<Expression, Expression> fn)
        {
            var dst = RewriteOp(instr.ops[0]);
            var src = RewriteOp(instr.ops[1]);
            m.Assign(dst, fn(src));
        }

    }
}
