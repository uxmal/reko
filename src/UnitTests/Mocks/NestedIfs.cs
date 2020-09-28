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
using System;

namespace Reko.UnitTests.Mocks
{
    public class NestedIfs : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier ax = Local(PrimitiveType.Int16, "ax");
            Identifier cl = Local(PrimitiveType.Byte, "cl");
            BranchIf(base.Lt(ax, 0), "negatory");
            Label("positive");
                Assign(cl, 0);
                BranchIf(Le(ax,12),"small");
                Label("very_positive");
                    Assign(ax,12);
                Label("small");
                Goto("join");
            Label("negatory");
                Assign(cl, 1);
                BranchIf(Ge(ax, -12), "negsmall");
                Label("very_negative");
                    Assign(ax,-12);
                Label("negsmall");
                Goto("join");
            Label("join");
            Return();
        }
    }
}