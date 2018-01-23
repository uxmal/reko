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

namespace Reko.Arch.RiscV
{
    public partial class RiscVRewriter
    {
        private void RewriteAdd()
        {
            var dst = RewriteOp(instr.op1);
            var src1 = RewriteOp(instr.op2);
            var src2 = RewriteOp(instr.op3);
            Expression src;
            if (src1.IsZero)
            {
               src = src2;
            } else if (src2.IsZero)
            {
                src = src1;
            }
            else
            {
                src = m.IAdd(src1, src2);
            }
            m.Assign(dst, src);
        }

        private void RewriteAddw()
        {
            var dst = RewriteOp(instr.op1);
            var src1 = RewriteOp(instr.op2);
            var imm2 = instr.op3 as ImmediateOperand;
            Expression src2;
            if (imm2 != null)
            {
                src2 = Constant.Int32(imm2.Value.ToInt32());
            }
            else
            {
                src2 = RewriteOp(instr.op3);
            }
                    
            Expression src;
            if (src1.IsZero)
            {
                src = src2;
            }
            else if (src2.IsZero)
            {
                src = m.Cast(PrimitiveType.Word32, src1);
            }
            else
            {
                src = m.IAdd(m.Cast(PrimitiveType.Word32, src1), src2);
            }
            m.Assign(dst, m.Cast(PrimitiveType.Int64, src));
        }

        private void RewriteAnd()
        {
            var dst = RewriteOp(instr.op1);
            var src1 = RewriteOp(instr.op2);
            var src2 = RewriteOp(instr.op3);
            m.Assign(dst, m.And(src1, src2));
        }

        private void RewriteLoad(DataType dt)
        {
            var dst = RewriteOp(instr.op1);
            var baseReg = RewriteOp(instr.op2);
            var offset = ((ImmediateOperand)instr.op3).Value;
            Expression ea = baseReg;
            if (!offset.IsZero)
            {
                ea = m.IAdd(ea, offset);
            }
            Expression src = m.Mem(dt, ea);
            if (dst.DataType.Size != src.DataType.Size)
            {
                src = m.Cast(arch.WordWidth, src);
            }
            m.Assign(dst, src);
        }

        private void RewriteLui()
        {
            var dst = RewriteOp(instr.op1);
            var ui = ((ImmediateOperand)instr.op2).Value;
            m.Assign(dst, Constant.Word(dst.DataType.Size, ui.ToUInt32() << 12));
        }

        private void RewriteOr()
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            var src = left;
            if (!right.IsZero)
            {
                src = m.Or(src, right);
            }
            m.Assign(dst, src);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            m.Assign(dst, fn(left, right));
        }

        private void RewriteShiftw(Func<Expression, Expression, Expression> fn)
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            var src = m.Cast(PrimitiveType.Int32, fn(left, right));
            m.Assign(dst, m.Cast(PrimitiveType.Int64, src));
        }

        private void RewriteSlt(bool unsigned)
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            Expression src;
            if (unsigned)
            {
                if (left.IsZero)
                {
                    src = m.Ne0(right);
                }
                else
                {
                    src = m.Ult(left, right);
                }
            }
            else
            {
                src = m.Lt(left, right);
            }
            m.Assign(dst, m.Cast(arch.WordWidth, src));
        }

        private void RewriteStore(DataType dt)
        {
            var src = RewriteOp(instr.op1);
            var baseReg = RewriteOp(instr.op2);
            var offset = ((ImmediateOperand)instr.op3).Value;
            Expression ea = baseReg;
            if (!offset.IsZero)
            {
                ea = m.IAdd(ea, offset);
            }
            m.Assign(m.Mem(dt, ea), src);
        }

        private void RewriteSub()
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            m.Assign(dst, m.ISub(left, right));
        }

        private void RewriteSubw()
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            var src = left;
            if (!right.IsZero)
            {
                src = m.ISub(src, right);
                src.DataType = PrimitiveType.Int32;
            }
            m.Assign(dst, m.Cast(PrimitiveType.Int64, src));
        }

        private void RewriteXor()
        {
            var dst = RewriteOp(instr.op1);
            var left = RewriteOp(instr.op2);
            var right = RewriteOp(instr.op3);
            var src = left;
            if (!right.IsZero)
            {
                src = m.Xor(src, right);
            }
            m.Assign(dst, src);
        }
    }
}
