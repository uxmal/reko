#region License
/*
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {

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

        private void RewriteFDiv(PrimitiveType dt)
        {
            var left = Reg(0, dt);
            var right = Reg(1, dt);
            var dst = Assign(Reg(0), m.FDiv(left, right));
            SetCc(m.Cond(dst));
        }

        private void RewriteHalveR(PrimitiveType dt, Constant c)
        {
            var src = Reg(1, dt);
            Assign(Reg(0), m.FDiv(src, c));
        }

        private void RewriteLtdr(PrimitiveType dt, Constant zero)
        {
            var src = Reg(1, dt);
            var dst = Assign(Reg(0), src);
            SetCc(m.Cond(m.FSub(dst, zero)));
        }

        private void RewriteMedr(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var left = Reg(0, dtFrom);
            var right = Reg(1, dtFrom);
            var dst = Assign(Reg(0), m.FMul(dtTo, left, right));
        }

        private void RewriteSdr(PrimitiveType dt)
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
    }
}
