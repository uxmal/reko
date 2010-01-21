/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    /// <summary>
    ///     while ("foo())
    ///     {
    ///         bar();
    ///         if (foo())
    ///         {
    ///             bar();
    ///             goto end;
    ///         }
    ///         bar();
    ///     }
    ///     bar();
    /// end:
    ///     bar();
    /// }
    /// </summary>
    public class MockWhileGoto : ProcedureMock
    {
        protected override void BuildBody()
        {
            Label("LoopHead");
            BranchIf(Not(Fn("foo")), "LoopFollow");
                SideEffect(Fn("bar"));
                BranchIf(Not(Fn("foo")), "skip");
                    Label("unstruct_branch");
                    SideEffect(Fn("extraordinary"));
                    Jump("end");
                Label("skip");
                SideEffect(Fn("bar2"));
                Jump("LoopHead");
            Label("LoopFollow");
            SideEffect(Fn("bar3"));
            Label("end");
            SideEffect(Fn("bar4"));
            Return();
        }
    }
}
