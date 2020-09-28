#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
                e = m.Cast(dst.DataType, e);
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
                m.Not(host.PseudoProcedure("OV", PrimitiveType.Bool, dst)),
                instr.Address + instr.Length, 
                InstrClass.ConditionalTransfer);
            var ch = new ProcedureCharacteristics { Terminates = true };
            m.SideEffect(
                host.PseudoProcedure("__trap_overflow", ch, VoidType.Instance),
                InstrClass.Transfer|InstrClass.Call);
        }

        private void RewriteInstrinsic(string instrinic)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            var dst = Rewrite(instr.Operands[2]);
            if (dst.IsZero)
            {
                m.SideEffect(host.PseudoProcedure(instrinic, dst.DataType, op1, op2));
            }
            else
            {
                m.Assign(dst, host.PseudoProcedure(instrinic, dst.DataType, op1, op2));
            }
        }

        private void RewriteLoadInstrinsic(string intrinsic, DataType dt)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            op2.DataType = dt;
            if (op1.IsZero)
            {
                // Discarding the result == side effect
                m.SideEffect(host.PseudoProcedure(intrinsic, dt, op2));
            }
            else
            {
                m.Assign(op1, host.PseudoProcedure(intrinsic, dt, op2));
            }
        }

        private void RewriteStoreInstrinsic(string intrinsic, DataType dt)
        {
            var op1 = Rewrite(instr.Operands[0]);
            var op2 = Rewrite(instr.Operands[1]);
            op2.DataType = dt;
            m.SideEffect(host.PseudoProcedure(intrinsic, dt, op2, op1));
        }

        private void RewriteLd(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var src = Rewrite(instr.Operands[1]);
            src.DataType = dtSrc;
            if (dtSrc != dtDst)
            {
                src = m.Cast(dtDst, src);
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

        private void RewriteSt(PrimitiveType dtDst)
        {
            var src = Rewrite(instr.Operands[0]);
            if (src.DataType != dtDst)
            {
                src = m.Cast(dtDst, src);
            }
            var dst = Rewrite(instr.Operands[1]);
            dst.DataType = dtDst;
            m.Assign(dst, src);
        }

        private void RewriteTrapb()
        {
            m.SideEffect(
                host.PseudoProcedure("__trap_barrier", VoidType.Instance),
                InstrClass.Transfer|InstrClass.Call);
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
