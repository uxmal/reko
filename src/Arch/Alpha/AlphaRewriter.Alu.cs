#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Alpha
{
    partial class AlphaRewriter
    {
        private void RewriteCmp(Func<Expression, Expression, Expression> cmp)
        {
            var op1 = Rewrite(instr.op1);
            var op2 = Rewrite(instr.op2);
            var dst = Rewrite(instr.op3);
            var e = cmp(op1, op2);
            if (e.DataType == PrimitiveType.Bool)
            {
                e = m.Cast(dst.DataType, e);
            }
            m.Assign(dst, e);
        }

        private void RewriteBin(Func<Expression,Expression,Expression> fn)
        {
            var op1 = Rewrite(instr.op1);
            var op2 = Rewrite(instr.op2);
            var dst = Rewrite(instr.op3);
            var e = fn(op1, op2);
            m.Assign(dst, e);
        }

        private void RewriteInstrinsic(string instrinic)
        {
            var op1 = Rewrite(instr.op1);
            var op2 = Rewrite(instr.op2);
            var dst = Rewrite(instr.op3);
            m.Assign(dst, host.PseudoProcedure(instrinic, dst.DataType, op1, op2));
        }

        private void RewriteLd(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var src = Rewrite(instr.op2);
            src.DataType = dtSrc;
            if (dtSrc != dtDst)
            {
                src = m.Cast(dtDst, src);
            }
            var dst = Rewrite(instr.op1);
            m.Assign(dst, src);
        }

		private void RewriteLda(int shift)
        {
            var mop = (MemoryOperand)instr.op2;
            int offset = mop.Offset << shift;
            Expression src;
            if (mop.Base.Number == ZeroRegister)
            {
                src = Constant.Create(PrimitiveType.Word64, mop.Offset << shift);
            }
            else
            {
                src = binder.EnsureRegister(mop.Base);
                if (offset < 0)
                {
                    src = m.ISub(src, -offset);
                }
                else if (offset > 0)
                {
                    src = m.IAdd(src, offset);
                }
            }
            var dst = Rewrite(instr.op1);
            m.Assign(dst, src);
        }

        private void RewriteSt(PrimitiveType dtDst)
        {
            var src = Rewrite(instr.op1);
            if (src.DataType != dtDst)
            {
                src = m.Cast(dtDst, src);
            }
            var dst = Rewrite(instr.op2);
            dst.DataType = dtDst;
            m.Assign(dst, src);
        }

        private Expression addl(Expression a, Expression b)
        {
            return m.Cast(
                PrimitiveType.Word64,
                m.Slice(PrimitiveType.Int32, m.IAdd(a, b), 0));
        }

        private Expression addq(Expression a, Expression b)
        {
            if (a.IsZero)
                return b;
            if (b.IsZero)
                return a;
            //$REVIEW: consider a peephole optimizer in IAdd? 
            return m.IAdd(a, b);
        }

        private Expression and(Expression a, Expression b)
        {
            if (a.IsZero)
                return a;
            if (b.IsZero)
                return b;
            return m.And(a, b);
        }

        private Expression bic(Expression a, Expression b)
        {
            if (a.IsZero)
                return a;
            if (b.IsZero)
                return a;
            return m.And(a, m.Comp(b));
        }

        private Expression bis(Expression a, Expression b)
        {
            if (a.IsZero)
                return b;
            if (b.IsZero)
                return a;
            return m.Or(a, b);
        }

        private Expression mull(Expression a, Expression b)
        {
            if (a.IsZero)
                return a;
            if (b.IsZero)
                return b;

            return m.Cast(
                PrimitiveType.Word64,
                m.Slice(PrimitiveType.Int32, m.IMul(a, b), 0));
        }

        private Expression mulq(Expression a, Expression b)
        {
            if (a.IsZero)
                return a;
            if (b.IsZero)
                return b;

            return m.IMul(a, b);
        }

        private Expression sll(Expression a, Expression sh)
        {
            if (a.IsZero)
                return a;
            if (sh.IsZero)
                return a;
            return m.Shl(a, sh);
        }

        private Expression srl(Expression a, Expression sh)
        {
            if (a.IsZero)
                return a;
            if (sh.IsZero)
                return a;
            return m.Shr(a, sh);
        }

        private Expression SExtend(Expression e)
        {
            return m.Cast(
                PrimitiveType.Word64,
                m.Slice(PrimitiveType.Int32, e, 0));
        }

        private Expression s4addl(Expression a, Expression b)
        {
            return addl(m.IMul(a, 4), b);
        }

        private Expression s4addq(Expression a, Expression b)
        {
            return addq(m.IMul(a, 4), b);
        }

        private Expression s8addl(Expression a, Expression b)
        {
            return addl(m.IMul(a, 8), b);
        }

        private Expression s8addq(Expression a, Expression b)
        {
            return addq(m.IMul(a, 8), b);
        }

        private Expression s4subl(Expression a, Expression b)
        {
            return subl(m.IMul(a, 4), b);
        }

        private Expression s4subq(Expression a, Expression b)
        {
            return subq(m.IMul(a, 4), b);
        }

        private Expression s8subl(Expression a, Expression b)
        {
            return subl(m.IMul(a, 8), b);
        }

        private Expression s8subq(Expression a, Expression b)
        {
            return subq(m.IMul(a, 8), b);
        }
        private Expression subl(Expression a, Expression b)
        {
            return SExtend(subq(a, b)); 
        }


        private Expression subq(Expression a, Expression b)
        {
            if (a.IsZero)
                return m.Neg(b);
            if (b.IsZero)
                return a;
            else
                return m.ISub(a, b);
        }

        private Expression xor(Expression a, Expression b)
        {
            if (a.IsZero)
                return b;
            if (b.IsZero)
                return a;
            return m.Xor(a, b);
        }
    }
}
