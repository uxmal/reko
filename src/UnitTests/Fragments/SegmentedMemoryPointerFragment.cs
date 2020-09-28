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
    public class SegmentedMemoryPointerMock : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier cs = Local16("cs");
            cs.DataType = PrimitiveType.SegmentSelector;
            Identifier ax = Local16("ax");
            Identifier si = Local16("si");
            Identifier si2 = Local16("si2");
            Assign(si, Word16(0x0001));
            Assign(ax, SegMem16(cs, si));
            Assign(si2, Word16(0x0005));
            Assign(ax, SegMem16(cs, si2));
            Store(SegMem16(cs, Word16(0x1234)), ax);
            Store(SegMem16(cs, IAdd(si, 2)), ax);
        }
    }
    public class SegmentedMemoryPointerMock2 : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier ds = Local16("ds");
            ds.DataType = PrimitiveType.SegmentSelector;
            Identifier ax = Local16("ax");
            Identifier bx = Local16("bx");
            Assign(ax, SegMem16(ds, bx));
            Assign(ax, SegMem16(ds, IAdd(bx, 4)));
        }
    }

}
