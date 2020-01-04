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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Fragments
{
    public class PointerChainFragment : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var r1 = Register("r1");
            var r2 = Register("r2");
            var r3 = Register("r3");
            var r4 = Register("r4");
            Assign(r2, Cast(r2.DataType, Mem8(IAdd(Mem32(IAdd(Mem32(IAdd(r1, 4)), 8)), 16))));
            Assign(r4, Cast(r4.DataType, Mem16(Mem32(Mem32(Mem32(r3))))));
            Return();
        }
    }
}
