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

using Reko.UnitTests.Mocks;

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Structure
{
    public class MockCaseStatement : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var w = Local16("w");
            Switch(w, "case0", "case1", "case2");
            Label("case0");
            SideEffect(Fn("fn0"));
            Goto("done");
            Label("case1");
            SideEffect(Fn("fn1"));
            Goto("done");
            Label("case2");
            SideEffect(Fn("fn2"));
            Label("done");
            Return();
        }
    }
}
