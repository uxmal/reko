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
            m.SideEffect(host.Intrinsic(
                "__branch_prediction_relative_preload",
                true,
                VoidType.Instance,
                Op(0, arch.WordWidth),
                Op(1, arch.WordWidth),
                Op(2, arch.WordWidth)));
        }

        private void RewriteEx()
        {
            SetCc(host.Intrinsic("__execute", true, PrimitiveType.Byte, Reg(0), Op(1, arch.WordWidth)));
        }

        private void RewriteLra()
        {
            var r = Reg(0);
            SetCc(host.Intrinsic("__load_real_address", true,
                PrimitiveType.Byte,
                EffectiveAddress(1),
                m.Out(r.DataType, r)));
        }

        private void RewriteStctl(PrimitiveType dt) {
            var op1 = Reg(0);
            var op2 = m.AddrOf(new Pointer(dt, arch.PointerType.BitSize), m.Mem(dt, EffectiveAddress(1)));
            m.SideEffect(host.Intrinsic(
                "__store_control",
                true,
                VoidType.Instance,
                op1,
                op2));
        }
    }
}
