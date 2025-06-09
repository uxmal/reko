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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Alpha
{
    partial class AlphaRewriter
    {
        private void RewriteCmp(Func<Expression, Expression, Expression> cmp)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            var e = cmp(op1, op2);
            if (e.DataType == PrimitiveType.Bool)
            {
                e = m.Convert(e, e.DataType, dst.DataType);
            }
            m.Assign(dst, e);
        }

        private void RewriteBin(Func<Expression,Expression,Expression> fn)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            var e = fn(op1, op2);
            m.Assign(dst, e);
        }

        private void RewriteBinOv(Func<Expression, Expression, Expression> fn)
        {
            RewriteBin(fn);
            var dst = Rewrite(instr.Operands[2]);
            m.BranchInMiddleOfInstruction(
                m.Not(m.Fn(ov_intrinsic, dst)),
                instr.Address + instr.Length, 
                InstrClass.ConditionalTransfer);
            m.SideEffect(
                m.Fn(trap_overflow),
                InstrClass.Transfer|InstrClass.Call);
        }

        private void RewriteInstrinsic(IntrinsicProcedure instrinic)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            if (dst.IsZero)
            {
                m.SideEffect(m.Fn(instrinic.MakeInstance(dst.DataType), op1, op2));
            }
            else
            {
                m.Assign(dst, m.Fn(instrinic.MakeInstance(dst.DataType), op1, op2));
            }
        }

        private void RewriteLoadInstrinsic(IntrinsicProcedure intrinsic, DataType dt)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            op2.DataType = dt;
            if (op1.IsZero)
            {
                // Discarding the result == side effect
                m.SideEffect(m.Fn(intrinsic.MakeInstance(dt, op1.DataType), op2));
            }
            else
            {
                m.Assign(op1, m.Fn(intrinsic.MakeInstance(dt, op1.DataType), op2));
            }
        }

        private void RewriteStoreInstrinsic(IntrinsicProcedure intrinsic, DataType dt)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            op2.DataType = dt;
            var ptr = new Pointer(dt, arch.PointerType.BitSize);
            m.SideEffect(m.Fn(intrinsic.MakeInstance(dt, ptr), op2, op1));
        }

        private void RewriteLd(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var src = Rewrite(instr.Operands[1]);
            src.DataType = dtSrc;
            if (dtSrc != dtDst)
            {
                src = m.Convert(src, src.DataType, dtDst);
            }
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, src);
        }

		private void RewriteLda(int shift)
        {
            var mop = (MemoryOperand)instr.Operands[1];
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
            var dst = Rewrite(instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteSt(PrimitiveType dtFrom, PrimitiveType? dtTo = null)
        {
            var src = Rewrite(instr.Operands[0]);
            if (dtTo is not null)
            {
                src = m.Convert(src, dtFrom, dtTo);
            }
            else if (src.DataType != dtFrom)
            {
                src = m.Slice(src, dtFrom);
            }
            var dst = Rewrite(instr.Operands[1]);
            dst.DataType = dtTo ?? dtFrom;
            m.Assign(dst, src);
        }

        private void RewriteTrapb()
        {
            m.SideEffect(m.Fn(trap_barrier_intrinsic),
                InstrClass.Transfer|InstrClass.Call);
        }

        private Expression addl(Expression a, Expression b)
        {
            return m.Convert(
                m.Slice(m.IAdd(a, b), PrimitiveType.Int32),
                PrimitiveType.Int32,
                PrimitiveType.Int64);
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

            return m.Convert(
                m.IMul(
                    m.Slice(a, PrimitiveType.Int32),
                    m.Slice(b, PrimitiveType.Int32)),
                PrimitiveType.Int32,
                PrimitiveType.Int64);
        }

        private Expression mulq(Expression a, Expression b)
        {
            if (a.IsZero)
                return a;
            if (b.IsZero)
                return b;

            return m.IMul(a, b);
        }

        private Expression ornot(Expression a, Expression b)
        {
            if (a.IsZero)
                return m.Comp(b);
            if (b.IsZero || a == b)
                return Constant.Create(PrimitiveType.Word64, -1);
            return m.Or(a, m.Comp(b));
        }

        private Expression sll(Expression a, Expression sh)
        {
            if (a.IsZero)
                return a;
            if (sh.IsZero)
                return a;
            return m.Shl(a, sh);
        }

        private Expression sra(Expression a, Expression sh)
        {
            if (a.IsZero || sh.IsZero)
                return a;
            return m.Sar(a, sh);
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
            return m.Convert(
                m.Slice(e, PrimitiveType.Int32),
                PrimitiveType.Int32,
                PrimitiveType.Word64);
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

        private Expression umulh(Expression a, Expression b)
        {
            if (a.IsZero || b.IsZero)
                return a;
            else
                return m.Shr(m.UMul(a, b), 64);
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
