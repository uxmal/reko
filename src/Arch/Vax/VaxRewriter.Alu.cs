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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public partial class VaxRewriter
    {
        private Expression Adawi(Expression a, Expression b)
        {
            return host.PseudoProcedure("atomic_fetch_add",
                a.DataType,
                a, b);
        }

        private Expression Bic(Expression a, Expression mask)
        {
            return m.And(a, m.Comp(mask));
        }

        private Expression Copy(Expression e)
        {
            return e;
        }

        private Expression Dec(Expression e)
        {
            return m.ISub(e, 1);
        }

        private Expression FCmp0(Expression val)
        {
            return m.FSub(val, ConstantReal.Create(val.DataType, 0.0));
        }

        private Expression ICmp0(Expression val)
        {
            return m.ISub(val, Constant.Zero(val.DataType));
        }

        private Expression Inc(Expression e)
        {
            return m.IAdd(e, 1);
        }

        private void RewriteMova(PrimitiveType width)
        {
            var opSrc = RewriteSrcOp(0, width);
            Expression dst;
            if (opSrc is MemoryAccess mem)
            {
                dst = RewriteDstOp(1, PrimitiveType.Word32, e => mem.EffectiveAddress);
            }
            else if (opSrc is Address addr)
            {
                dst = opSrc;
            }
            else
            {
                Debug.Print(
                    "{0}: Source operand must be a memory reference.",
                    instr.Address);
                m.Invalid();
                return;
            }
            NZ00(dst);
        }

        private Expression Rotl(Expression a, Expression b)
        {
            return host.PseudoProcedure(
                PseudoProcedure.Rol,
                a.DataType,
                a, b);
        }

        private void RewriteP4(string op)
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            m.Assign(
                grf,
                host.PseudoProcedure(
                    op,
                    PrimitiveType.Byte,
                    op0, op1, op2, op3));
            var c = FlagGroup(FlagM.CF);
            m.Assign(c, Constant.False());
        }

        private void RewriteP6(string op)
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Ptr32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            m.Assign(
                grf,
                host.PseudoProcedure(
                    op, 
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            m.Assign(c, Constant.False());
        }

        private void RewriteAdwc()
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(1, PrimitiveType.Word32,
                e => m.IAdd(
                        m.IAdd(e, op1),
                        FlagGroup(FlagM.CF)));
            AllFlags(dst);
        }

        private bool RewriteAlu2(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var dst = RewriteDstOp(1, width, e => fn(e, op1));
            if (dst == null)
            {
                EmitInvalid();
                return false;
            }
            return genFlags(dst);
        }

        private bool RewriteAlu3(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            if (op1 != null)
            {
                var op2 = RewriteSrcOp(1, width);
                if (op2 != null)
                {
                    var dst = RewriteDstOp(2, width, e => fn(op2, op1));
                    if (dst != null)
                    {
                        return genFlags(dst);
                    }
                }
            }
            EmitInvalid();
            return false;
        }

        private bool RewriteAluUnary1(PrimitiveType width, Func<Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var dst = RewriteDstOp(1, width, e => fn(e));
            return genFlags(dst);
        }

        private bool RewriteAluUnary2(PrimitiveType width, Func<Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var dst = RewriteDstOp(1, width, e => fn(op1));
            return genFlags(dst);
        }

        private void RewriteAsh(PrimitiveType width)
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.SByte);
            Func<Expression, Expression, Expression> fn;
            Expression shift;
            if (op1 is Constant c)
            {
                var sh = c.ToInt16();
                if (sh > 0)
                {
                    fn = m.Shl;
                }
                else
                {
                    fn = m.Sar;
                    sh = (short) -sh;
                }
                shift = Constant.SByte((sbyte) sh);
            }
            else
            {
                shift = RewriteSrcOp(0, width);
                fn = (a, b) => host.PseudoProcedure("__ashift", width, a, b);
            }
            var op2 = RewriteSrcOp(1, width);
            var dst = RewriteDstOp(2, width, e => fn(op2, shift));
            this.NZV0(dst);
        }

        private void RewriteAshp()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Ptr32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            m.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_ashp",
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            m.Assign(c, Constant.False());
        }

        private void RewriteBit(PrimitiveType width)
        {
            var mask = RewriteSrcOp(0, width);
            var src = RewriteSrcOp(1, width);
            var tmp = binder.CreateTemporary(width);
            m.Assign(tmp, m.And(src, mask));
            m.Assign(FlagGroup(FlagM.NZ), m.Cond(tmp));
            m.Assign(FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteClr(PrimitiveType width)
        {
            RewriteDstOp(0, width, e => Constant.Create(width, 0));
            m.Assign(FlagGroup(FlagM.ZF), Constant.True());
            m.Assign(FlagGroup(FlagM.NF), Constant.False());
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
            m.Assign(FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteCmp(PrimitiveType width)
        {
            var op0 = RewriteSrcOp(0, width);
            var op1 = RewriteSrcOp(1, width);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF);
            m.Assign(grf, m.Cond(m.ISub(op0, op1)));
            m.Assign(FlagGroup(FlagM.VF), Constant.False());
        }

        private void RewriteCmpp3()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Ptr32);
            NZ00(
                host.PseudoProcedure(
                    "vax_cmpp3",
                    PrimitiveType.Byte,
                    op0, op1, op2));
        }

        private void RewriteCmpp4()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            NZ00(
                host.PseudoProcedure(
                    "vax_cmpp4",
                    PrimitiveType.Byte,
                    op0, op1, op2, op3));
        }

        private void RewriteMovz(PrimitiveType from, PrimitiveType to)
        {
            var opFrom = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Cast(to, opFrom));
            NZ00(dst);
        }

        private void RewriteCvt(PrimitiveType from, PrimitiveType to)
        {
            var src = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Cast(to, src));
            NZV0(dst);
        }

        private void RewriteCvtComplex(string cvtfn)
        {
            var srclen = RewriteSrcOp(0, PrimitiveType.Word16);
            var srcaddr = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var dstlen = RewriteSrcOp(2, PrimitiveType.Word16);
            var dstaddr = RewriteSrcOp(3, PrimitiveType.Word16);
            NZV0(host.PseudoProcedure(cvtfn,
                PrimitiveType.Byte,
                srclen,
                srcaddr,
                dstlen,
                dstaddr));
        }

        private void RewriteCvtr(PrimitiveType from, PrimitiveType to)
        {
            var src = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Cast(
                to,
                host.PseudoProcedure("round", to, src)));
            NZV0(dst);
        }

        private void RewriteDivp()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Ptr32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            m.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_divp",
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            m.Assign(c, Constant.False());
        }

        private void RewriteExtv(Domain domain)
        {
            var pos = RewriteSrcOp(0, PrimitiveType.Word32);
            var size = RewriteSrcOp(1, PrimitiveType.Byte);
            if (pos is Constant cPos && size is Constant cSize)
            {
                var nSize = cSize.ToInt32();
                if (0 <= nSize && nSize < 32)
                {
                    var bas = RewriteSrcOp(2, PrimitiveType.Word32);
                    Expression dst = RewriteDstOp(3, PrimitiveType.Word32, e =>
                        m.Cast(
                            PrimitiveType.Create(domain, 32),
                            m.Slice(bas, cPos.ToInt32(), cSize.ToInt32())));
                    this.NZ0(dst);
                    return;
                }
            }
            rtlc = InstrClass.Invalid;
            m.Invalid();
        }

        private void RewriteFfx(string fnname)
        {
            var start = RewriteSrcOp(0, PrimitiveType.Word32);
            var size = RewriteSrcOp(1, PrimitiveType.Byte);
            var bas = RewriteSrcOp(2, PrimitiveType.Word32);
            var findPos = RewriteSrcOp(3, PrimitiveType.Word32);
            var z = FlagGroup(FlagM.ZF);
            var grf = FlagGroup(FlagM.NVC);
            m.Assign(
                z,
                host.PseudoProcedure(
                    fnname,
                    z.DataType,
                    bas, size, start,
                    m.Out(PrimitiveType.Ptr32, findPos)));
            m.Assign(grf, 0);
        }

        private void RewriteIncDec(PrimitiveType width, Func<Expression, Expression> incdec)
        {
            var dst = RewriteDstOp(0, width, e => incdec(e));
            AllFlags(dst);
        }

        private void RewriteMulp()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Ptr32);
            var grf = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF);
            m.Assign(
                grf,
                host.PseudoProcedure(
                    "vax_mulp",
                    PrimitiveType.Byte,
                    op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(FlagM.CF);
            m.Assign(c, Constant.False());
        }

        private void RewritePoly(PrimitiveType width)
        {
            var op0 = RewriteSrcOp(0, width);
            var op1 = RewriteSrcOp(1, PrimitiveType.Word16);
            var op2 = RewriteSrcOp(2, PrimitiveType.Ptr32);
            var ret = binder.EnsureRegister(Registers.r0);
            if (width.Size == 8)
            {
                var r1 = binder.EnsureRegister(Registers.r1);
                ret = binder.EnsureSequence(width, r1.Storage, ret.Storage);
            }
            var grf = FlagGroup(FlagM.ZF | FlagM.NF);
            m.Assign(
                ret,
                host.PseudoProcedure(
                    "vax_poly",
                    width,
                    op0, op1, op2));
            m.Assign(grf, m.Cond(ret));
            m.Assign(FlagGroup(FlagM.VF), Constant.False());
            m.Assign(FlagGroup(FlagM.CF), Constant.False());
        }

        private void RewritePush(PrimitiveType width)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISub(sp, width.Size));
            var op0 = RewriteSrcOp(0, width);
            if (op0 is MemoryAccess)
            {
                var t = binder.CreateTemporary(width);
                m.Assign(t, op0);
                op0 = t;
            }
            m.Assign(m.Mem(width, sp), op0);
            NZ00(op0);
        }

        private void RewritePusha()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISub(sp, PrimitiveType.Word32.Size));
            if (!(RewriteSrcOp(0, PrimitiveType.Word32) is MemoryAccess op0))
            {
                EmitInvalid();
                return;
            }
            var ea = op0.EffectiveAddress;
            if (!(ea is Identifier || ea is Constant))
            {
                var t = binder.CreateTemporary(PrimitiveType.Word32);
                m.Assign(t, ea);
                ea = t;
            }
            m.Assign(m.Mem(PrimitiveType.Word32, sp), ea);
            NZ00(ea);
        }

        private void RewriteSbwc()
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(1, PrimitiveType.Word32,
                e => m.ISub(
                        m.ISub(e, op1),
                        FlagGroup(FlagM.CF)));
            AllFlags(dst);
        }

        private void RewriteTst(PrimitiveType width, Func<Expression,Expression> sub)
        {
            var op1 = RewriteSrcOp(0, width);
            NZ00(sub(op1));
        }
    }
}
