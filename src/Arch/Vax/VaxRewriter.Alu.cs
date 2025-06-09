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
using Reko.Core.Intrinsics;
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
            return m.Fn(atomic_fetch_add.MakeInstance(a.DataType), a, b);
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

        private Expression RewriteRotl(Expression a, Expression b)
        {
            return m.Fn(CommonOps.Rol, a, b);
        }

        private void RewriteP4(IntrinsicProcedure op)
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var grf = FlagGroup(Registers.VZN);
            m.Assign(
                grf,
                m.Fn(op, op0, op1, op2, op3));
            var c = FlagGroup(Registers.C);
            m.Assign(c, Constant.False());
        }

        private void RewriteP6(IntrinsicProcedure op)
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            var op4 = RewriteSrcOp(4, PrimitiveType.Word16);
            var op5 = RewriteSrcOp(5, PrimitiveType.Ptr32);
            var grf = FlagGroup(Registers.VZN);
            m.Assign(
                grf,
                m.Fn(op, op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(Registers.C);
            m.Assign(c, Constant.False());
        }

        private void RewriteAdwc()
        {
            var op1 = RewriteSrcOp(0, PrimitiveType.Word32);
            var dst = RewriteDstOp(1, PrimitiveType.Word32,
                e => m.IAdd(
                        m.IAdd(e, op1),
                        FlagGroup(Registers.C)));
            AllFlags(dst);
        }

        private bool RewriteAlu2(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            var dst = RewriteDstOp(1, width, e => fn(e, op1));
            if (dst is null)
            {
                EmitInvalid();
                return false;
            }
            return genFlags(dst);
        }

        private bool RewriteAlu3(PrimitiveType width, Func<Expression, Expression, Expression> fn, Func<Expression, bool> genFlags)
        {
            var op1 = RewriteSrcOp(0, width);
            if (op1 is not null)
            {
                var op2 = RewriteSrcOp(1, width);
                if (op2 is not null)
                {
                    var dst = RewriteDstOp(2, width, e => fn(op2, op1));
                    if (dst is not null)
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
                fn = (a, b) => m.Fn(ashift, a, b);
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
            var grf = FlagGroup(Registers.VZN);
            m.Assign(
                grf,
                m.Fn(ashp, op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(Registers.C);
            m.Assign(c, Constant.False());
        }

        private void RewriteBit(PrimitiveType width)
        {
            var mask = RewriteSrcOp(0, width);
            var src = RewriteSrcOp(1, width);
            var tmp = binder.CreateTemporary(width);
            m.Assign(tmp, m.And(src, mask));
            m.Assign(FlagGroup(Registers.ZN), m.Cond(tmp));
            m.Assign(FlagGroup(Registers.V), Constant.False());
        }

        private void RewriteClr(PrimitiveType width)
        {
            RewriteDstOp(0, width, e => Constant.Create(width, 0));
            m.Assign(FlagGroup(Registers.Z), Constant.True());
            m.Assign(FlagGroup(Registers.N), Constant.False());
            m.Assign(FlagGroup(Registers.C), Constant.False());
            m.Assign(FlagGroup(Registers.V), Constant.False());
        }

        private void RewriteCmp(PrimitiveType width)
        {
            var op0 = RewriteSrcOp(0, width);
            var op1 = RewriteSrcOp(1, width);
            var grf = FlagGroup(Registers.CZN);
            m.Assign(grf, m.Cond(m.ISub(op0, op1)));
            m.Assign(FlagGroup(Registers.V), Constant.False());
        }

        private void RewriteCmpp3()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Ptr32);
            NZ00(m.Fn(cmpp3, op0, op1, op2));
        }

        private void RewriteCmpp4()
        {
            var op0 = RewriteSrcOp(0, PrimitiveType.Word16);
            var op1 = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var op2 = RewriteSrcOp(2, PrimitiveType.Word16);
            var op3 = RewriteSrcOp(3, PrimitiveType.Ptr32);
            NZ00(m.Fn(cmpp4, op0, op1, op2, op3));
        }

        private void RewriteMovz(PrimitiveType from, PrimitiveType to)
        {
            var opFrom = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Convert(opFrom, from, to));
            NZ00(dst);
        }

        private void RewriteCvt(PrimitiveType from, PrimitiveType to)
        {
            var src = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Convert(src, from, to));
            NZV0(dst);
        }

        private void RewriteCvtComplex(IntrinsicProcedure cvtfn)
        {
            var srclen = RewriteSrcOp(0, PrimitiveType.Word16);
            var srcaddr = RewriteSrcOp(1, PrimitiveType.Ptr32);
            var dstlen = RewriteSrcOp(2, PrimitiveType.Word16);
            var dstaddr = RewriteSrcOp(3, PrimitiveType.Word16);
            NZV0(m.Fn(cvtfn,
                srclen,
                srcaddr,
                dstlen,
                dstaddr));
        }

        private void RewriteCvtr(PrimitiveType from, PrimitiveType to)
        {
            var src = RewriteSrcOp(0, from);
            var dst = RewriteDstOp(1, to, e => m.Convert(
                m.Fn(round.MakeInstance(from), src),
                from,
                to));
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
            var grf = FlagGroup(Registers.VZN);
            m.Assign(
                grf,
                m.Fn(divp, op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(Registers.C);
            m.Assign(c, Constant.False());
        }

        private void RewriteExtv(Domain domain)
        {
            var pos = RewriteSrcOp(0, PrimitiveType.Word32);
            var size = RewriteSrcOp(1, PrimitiveType.Byte);
            if (pos is Constant cPos && size is Constant cSize)
            {
                var nSize = cSize.ToInt32();
                var nPos = cPos.ToInt32();
                var nEnd = nSize + nPos;
                var dstWidth = instr.Operands[3].DataType.BitSize;
                if (0 <= nPos && nPos < dstWidth && 
                    nPos < nEnd && nEnd < dstWidth)
                {
                    var bas = RewriteSrcOp(2, PrimitiveType.Word32);
                    Expression dst = RewriteDstOp(3, PrimitiveType.Word32, e =>
                    {
                        var slice = m.Slice(bas, cPos.ToInt32(), cSize.ToInt32());
                        return m.Convert(
                            slice,
                            slice.DataType,
                            PrimitiveType.Create(domain, 32));
                    });
                    this.NZ0(dst);
                    return;
                }
            }
            iclass = InstrClass.Invalid;
            m.Invalid();
        }

        private void RewriteFfx(IntrinsicProcedure intrinsic)
        {
            var start = RewriteSrcOp(0, PrimitiveType.Word32);
            var size = RewriteSrcOp(1, PrimitiveType.Byte);
            var bas = RewriteSrcOp(2, PrimitiveType.Word32);
            var findPos = RewriteSrcOp(3, PrimitiveType.Word32);
            var z = FlagGroup(Registers.Z);
            var grf = FlagGroup(Registers.CVN);
            m.Assign(
                z,
                m.Fn(intrinsic,
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
            var grf = FlagGroup(Registers.VZN);
            m.Assign(
                grf,
                m.Fn(mulp, op0, op1, op2, op3, op4, op5));
            var c = FlagGroup(Registers.C);
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
            var grf = FlagGroup(Registers.ZN);
            m.Assign(
                ret,
                m.Fn(poly.MakeInstance(width), op0, op1, op2));
            m.Assign(grf, m.Cond(ret));
            m.Assign(FlagGroup(Registers.V), Constant.False());
            m.Assign(FlagGroup(Registers.C), Constant.False());
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

        private void RewritePushr()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var mask = RewriteSrcOp(0, PrimitiveType.Word32);
            if (mask is Constant cMask && !cMask.IsZero)
            {
                var uMask = cMask.ToUInt32();
                int i = 14;
                for (uint mm = 1u << 14; i >= 0; mm >>= 1, --i)
                {
                    if ((uMask & mm) != 0)
                    {
                        m.Assign(sp, m.ISubS(sp, 4));
                        m.Assign(m.Mem32(sp), binder.EnsureRegister(
                            arch.GetRegister(i)!));
                    }
                }
                return;
            }
            EmitUnitTest();
            m.Invalid();
        }

        private void RewritePusha()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISub(sp, PrimitiveType.Word32.Size));
            Expression ea = RewriteSrcOp(0, PrimitiveType.Word32);
            switch (ea)
            {
            case MemoryAccess op0:
                ea = op0.EffectiveAddress;
                if (!(ea is Identifier || ea is Constant))
                {
                    var t = binder.CreateTemporary(PrimitiveType.Word32);
                    m.Assign(t, ea);
                    ea = t;
                }
                break;
            case Address addr:
                ea = addr;
                break;
            default:
                EmitInvalid();
                return;
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
                        FlagGroup(Registers.C)));
            AllFlags(dst);
        }

        private void RewriteTst(PrimitiveType width, Func<Expression,Expression> sub)
        {
            var op1 = RewriteSrcOp(0, width);
            NZ00(sub(op1));
        }
    }
}
