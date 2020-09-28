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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Fragments
{
    public class SegMem3Mock : ProcedureBuilder
    {
        private Constant Seg(int seg)
        {
            return Constant.Create(PrimitiveType.SegmentSelector, seg);
        }

        protected override void BuildBody()
        {
            Identifier ds = base.Local(PrimitiveType.SegmentSelector, "ds");

            base.Store(SegMem16(Seg(0x1796), Word16(0x0001)), Seg(0x0800));
            Store(SegMem16(Seg(0x800), Word16(0x5422)), ds);
            Store(SegMem16(Seg(0x800), Word16(0x0066)), SegMem16(Seg(0x0800), Word16(0x5420)));
        }
    }
}
