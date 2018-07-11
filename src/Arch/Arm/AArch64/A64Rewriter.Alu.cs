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

using Reko.Core.Expressions;
using Reko.Core.Machine;
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
        private void RewriteBinary(Func<Expression,Expression,Expression> fn, bool setFlags)
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
            if (setFlags)
            {
                var nzcv = NZCV();
                m.Assign(nzcv, m.Cond(dst));
            }
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
