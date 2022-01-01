#region License
/*
 * Copyright (C) 1999-2022 John Källén.
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
        private void RewriteCvb(PrimitiveType dt) {
            Assign(Reg(0), host.Intrinsic($"__convert_decimal_to_int{dt.BitSize}", true,
                dt,
                m.AddrOf(arch.PointerType,
                    m.Mem8(EffectiveAddress(1)))));
        }

        private void RewriteCvd(PrimitiveType dt)
        {
            m.SideEffect(host.Intrinsic($"__convert_int{dt.BitSize}_to_decimal", true,
                dt,
                Reg(0, dt),
                m.AddrOf(arch.PointerType,
                    m.Mem8(EffectiveAddress(1)))));
        }

        private void RewriteSrp()
        {

        }
    }
}
