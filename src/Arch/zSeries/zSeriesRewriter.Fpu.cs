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
using Reko.Core.Types;
using System;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {

        
        private void RewriteFAdd(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = m.Mem(dt, EffectiveAddress(1));
            var dst = Assign(Reg(0), m.FAdd(src1, src2));
            SetCc(m.Cond(dst));
        }

        private void RewriteFAddReg(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            var dst = Assign(Reg(0), m.FAdd(src1, src2));
            SetCc(m.Cond(dst));
        }


        private void RewriteCmpFloat(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = Reg(1, dt);
            SetCc(m.Cond(m.FSub(src1, src2)));
        }

        private void RewriteCmpFloatMem(PrimitiveType dt)
        {
            var src1 = Reg(0, dt);
            var src2 = m.Mem(dt, EffectiveAddress(1));
            SetCc(m.Cond(m.FSub(src1, src2)));
        }

        private void RewriteFConvert(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var exp = m.Convert(m.Mem(dtFrom, EffectiveAddress(1)), dtFrom, dtTo);
            Assign(Reg(0), exp);
        }

        private void RewriteFConvertReg(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var exp = m.Convert(Reg(1, dtFrom), dtFrom, dtTo);
            Assign(Reg(0), exp);
        }

        private void RewriteFDivR(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), m.FDiv(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteFMul(PrimitiveType dtSrc, PrimitiveType dtDst)
        {
            var left = Reg(0, dtSrc);
            var right = m.Mem(dtSrc, EffectiveAddress(1));
            Identifier? dst;
            if (dtDst.BitSize == 128)
            {
                dst = FpRegPair(0, dtDst);
                if (dst is null)
                {
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    return;
                }

            }
            else
            {
                dst = Reg(0);
            }
            Assign(dst, m.FMul(dtDst, left, right));
        }

        private void RewriteFNegR(PrimitiveType dt)
        {
            var exp = Reg(1, dt);
            var dst = Assign(Reg(0), m.FNeg(exp));
            SetCc(m.Cond(dst));
        }

        private void RewriteFpuRegPair(Func<Expression,Expression,Expression> fn, PrimitiveType dt)
        {
            var src1 = FpRegPair(0, dt);
            var src2 = FpRegPair(1, dt);
            if (src1 is null || src2 is null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            var dst = Assign(src1, fn(src1, src2));
            SetCc(m.Cond(dst));
        }

        private void RewriteFSub(PrimitiveType dt)
        {
            var exp = Reg(0, dt);
            var dst = Assign(Reg(0), m.FSub(exp, m.Mem(dt, EffectiveAddress(1))));
            SetCc(m.Cond(dst));
        }

        private void RewriteHalveR(PrimitiveType dt, Constant c)
        {
            var src = Reg(1, dt);
            Assign(Reg(0), m.FDiv(src, c));
        }

        private void RewriteLdxr()
        {
            var src = FpRegPair(1, PrimitiveType.Word128);
            var dst = FpRegPair(0, PrimitiveType.Word128);
            if (src is null || dst is null)
            {
                iclass = InstrClass.Invalid;
                m.Invalid();
                return;
            }
            m.Assign(dst, src);
        }

        private void RewriteLtdr(PrimitiveType dt, Constant zero)
        {
            var src = Reg(1, dt);
            var dst = Assign(Reg(0), src);
            SetCc(m.Cond(m.FSub(dst, zero)));
        }

        private void RewriteFMulReg(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var left = Reg(0, dtFrom);
            var right = Reg(1, dtFrom);
            var dst = Assign(Reg(0), m.FMul(dtTo, left, right));
        }

        private void RewriteFSubReg(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), m.FSub(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteSur(PrimitiveType dt)
        {
            // 'u' = unnormalized.
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), m.FSub(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteTs()
        {
            var ea = EffectiveAddress(0);
            var mem = m.Mem8(ea);
            SetCc(m.Fn(intrinsics.ts.MakeInstance(arch.PointerType.BitSize, mem.DataType),
                m.AddrOf(arch.PointerType, mem)));
        }
    }
}
