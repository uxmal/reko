#region License
/*
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteBprp()
        {
            m.SideEffect(m.Fn(
                intrinsics.bprp.MakeInstance(arch.WordWidth),
                Op(0, arch.WordWidth),
                Op(1, arch.WordWidth),
                Op(2, arch.WordWidth)));
        }

        private void RewriteEx()
        {
            var op0 = Reg(0);
            SetCc(m.Fn(intrinsics.execute.MakeInstance(op0.DataType), op0, Op(1, arch.WordWidth)));
        }

        private void RewriteLra()
        {
            var r = Reg(0);
            SetCc(m.Fn(
                intrinsics.lra.MakeInstance(ptrSize, r.DataType),
                EffectiveAddress(1),
                m.Out(r.DataType, r)));
        }

        private void RewriteStctl(PrimitiveType dt) {
            var op1 = Reg(0);
            var op2 = m.AddrOf(new Pointer(dt, arch.PointerType.BitSize), m.Mem(dt, EffectiveAddress(1)));
            m.SideEffect(m.Fn(
                intrinsics.stctl,
                op1,
                op2));
        }
    }
}
