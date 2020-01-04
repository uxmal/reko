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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Xtensa
{
    public partial class XtensaRewriter
    {
        private void RewriteAddi()
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteSimm(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);
            if (src2.IsNegative)
            {
                m.Assign(dst, m.ISub(src1, src2.Negate()));
            }
            else
            {
                m.Assign(dst, m.IAdd(src1, src2));
            }
        }

        private void RewriteAddx(int scale)
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);

            m.Assign(dst, m.IAdd(src2, m.IMul(src1, scale)));
        }

        private void RewriteSubx(int scale)
        {
            var src1 = RewriteOp(this.instr.Operands[1]);
            var src2 = RewriteOp(this.instr.Operands[2]);
            var dst = RewriteOp(this.instr.Operands[0]);

            m.Assign(dst, m.ISub(m.IMul(src1, scale), src2));
        }

        private void RewriteBinOp(Func<Expression, Expression, Expression> fn)
        {
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var src2 = RewriteOp(dasm.Current.Operands[2]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewritePseudoFn(string name)
        {
            var aSrc = instr.Operands.Skip(1)
                .Select(o => RewriteOp(o))
                .ToArray();
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, host.PseudoProcedure(name, dst.DataType, aSrc));
        }

        private void RewritePseudoProc(string name)
        {
            var aSrc = instr.Operands
                .Select(o => RewriteOp(o))
                .ToArray();
            m.SideEffect(host.PseudoProcedure(name, VoidType.Instance, aSrc));
        }

        private void RewriteCopy()
        {
            var src = RewriteOp(this.instr.Operands[1]);
            var dst = RewriteOp(this.instr.Operands[0]);
            m.Assign(dst, src);
        }

        private void RewriteExtui()
        {
            var src = RewriteOp(this.instr.Operands[1]);
            var dst = RewriteOp(this.instr.Operands[0]);
            var sh = RewriteUimm(this.instr.Operands[2]);
            var ms = RewriteUimm(this.instr.Operands[3]);
            var mask = (1u << (int)ms.ToInt32()) - 1;
            Expression shifted;
            if (sh.IsZero)
                shifted = src;
            else
                shifted = m.Shr(src, sh);
            m.Assign(
                dst,
                m.And(shifted, mask));
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void RewriteOr()
        {
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var src2 = RewriteOp(dasm.Current.Operands[2]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, m.Or(src1, src2));
        }

        private void RewriteL32i()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var offset = Constant.UInt32(
                        ((ImmediateOperand)dasm.Current.Operands[2]).Value.ToUInt32());
            m.Assign(
                dst,
                m.Mem32(
                    m.IAdd(
                        RewriteOp(dasm.Current.Operands[1]),
                        offset)));
        }

        private void RewriteLsi(DataType dt)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(
                tmp,
                m.Mem(
                    dt,
                    m.IAdd(
                        RewriteOp(instr.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32()))));
            m.Assign(dst, m.Cast(PrimitiveType.Int32, tmp));
        }

        private void RewriteLsiu()
        {
            var a = RewriteOp(this.instr.Operands[1]);
            var off = Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32());
            Expression ea = null;
            var dst = RewriteOp(this.instr.Operands[0]);
            if (off.IsZero)
            {
                ea = a;
            }
            else
            {
                ea = binder.CreateTemporary(a.DataType);
                m.Assign(ea, m.IAdd(a, off));
            }
            m.Assign(dst, m.Mem(PrimitiveType.Real32, ea));
            if (!off.IsZero)
            { 
                m.Assign(a, ea);
            }
        }

        private void RewriteLui(DataType dt)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(
                tmp,
                m.Mem(
                    dt,
                    m.IAdd(
                        RewriteOp(instr.Operands[1]),
                        Constant.UInt32(
                        ((ImmediateOperand)instr.Operands[2]).Value.ToUInt32()))));
            m.Assign(dst, m.Cast(PrimitiveType.UInt32, tmp));
        }

        private void RewriteMovcc(Func<Expression,Expression,Expression> fn)
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var src = RewriteOp(this.instr.Operands[1]);
            var cond = RewriteOp(this.instr.Operands[2]);
            m.BranchInMiddleOfInstruction(
                fn(cond, Constant.Zero(cond.DataType)).Invert(),
                instr.Address + instr.Length,
                InstrClass.ConditionalTransfer);
            m.Assign(dst, src);
        }

        private void RewriteMovi_n()
        {
            var dst = RewriteOp(this.instr.Operands[0]);
            var src = Constant.Int32(
                ((ImmediateOperand)this.instr.Operands[1]).Value.ToInt32());
            m.Assign(dst, src);
        }

        private void RewriteMul16(Func<Expression, Expression, Expression> mul, Domain dom)
        {
            var src1 = RewriteOp(instr.Operands[1]);
            var src2 = RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var tmp1 = binder.CreateTemporary(PrimitiveType.Create(dom, 16));
            var tmp2 = binder.CreateTemporary(PrimitiveType.Create(dom, 16));
            m.Assign(tmp1, m.Cast(tmp1.DataType, src1));
            m.Assign(tmp2, m.Cast(tmp2.DataType, src2));
            m.Assign(dst, mul(tmp1, tmp2));
        }

        private void RewriteSi(DataType dt)
        {
            var src = RewriteOp(dasm.Current.Operands[0]);
            var ea = RewriteOp(dasm.Current.Operands[1]);
            var off = Constant.UInt32(
                        ((ImmediateOperand)dasm.Current.Operands[2]).Value.ToUInt32());
            if (!off.IsZero)
            {
                ea = m.IAdd(ea, off);
            }
            m.Assign(
                m.Mem(dt, ea),
                src);
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            //$REVIEW: the Xtensa spec is unclear on left shifts, shouild it be
            // a[0] = a[1] << (32 - SAR)?
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var sa = binder.EnsureRegister(Registers.SAR);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, fn(src1, sa));
        }

        private void RewriteShiftI(Func<Expression,Expression,Expression> fn)
        {
            var src1 = RewriteOp(dasm.Current.Operands[1]);
            var src2 = RewriteSimm(dasm.Current.Operands[2]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, fn(src1, src2));
        }

        private void RewriteSrc()
        {
            var src1 = (Identifier)RewriteOp(instr.Operands[1]);
            var src2 = (Identifier)RewriteOp(instr.Operands[2]);
            var dst = RewriteOp(instr.Operands[0]);
            var sa = binder.EnsureRegister(Registers.SAR);
            var cat = binder.EnsureSequence(
                PrimitiveType.CreateWord(src1.DataType.BitSize + src2.DataType.BitSize),
                src1.Storage,
                src2.Storage);
            m.Assign(
                dst,
                m.Cast(dst.DataType, m.Shr(cat, sa)));
        }

        private void RewriteSsa()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, src);
        }

        private void RewriteSsl()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, m.ISub(Constant.Create(src.DataType, 32), src));
        }

        private void RewriteSsa8l()
        {
            var src = RewriteOp(instr.Operands[0]);
            var dst = binder.EnsureRegister(Registers.SAR);
            m.Assign(dst, m.IMul(src, 8));
        }

        private void RewriteUnaryOp(Func<Expression, Expression> fn)
        {
            var src = RewriteOp(dasm.Current.Operands[1]);
            var dst = RewriteOp(dasm.Current.Operands[0]);
            m.Assign(dst, fn(src));
        }

    }
}
