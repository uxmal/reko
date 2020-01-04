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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    // Implements the following graph:
    // if (foo) {
    //     if (bar) 
    //        goto inside;
    //     baz();
    // } else {
    //    quux();
    // inside:
    //    niz();
    // }
    public class UnstructuredIfsMock : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            BranchIf(Declare(PrimitiveType.Bool, "foo"), "then1");
            Label("else1");
            SideEffect(Fn("quux"));
            Label("inside");
            SideEffect(Fn("niz"));
            Goto("done");

            Label("then1");
            BranchIf(Declare(PrimitiveType.Bool, "bar"), "inside");
            SideEffect(Fn("baz"));
            Label("done");
            Return();
        }
    }
}
