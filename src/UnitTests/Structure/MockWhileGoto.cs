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

using Reko.Core.Code;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Structure
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
    public class MockWhileGoto : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Label("LoopHead");
            BranchIf(Not(Fn("foo")), "LoopFollow");
                SideEffect(Fn("bar"));
                BranchIf(Not(Fn("foo")), "skip");
                    Label("unstruct_branch");
                    SideEffect(Fn("extraordinary"));
                    Goto("end");
                Label("skip");
                SideEffect(Fn("bar2"));
                Goto("LoopHead");
            Label("LoopFollow");
            SideEffect(Fn("bar3"));
            Label("end");
            SideEffect(Fn("bar4"));
            Return();
        }
    }

    public class MockWhileGoto2 : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var al = LocalByte("al");
            var si = Local16("si");
            var ax = Local16("ax");
            var bx = Local16("bx");
            var di = Local16("di");

            Assign(bx, si);
            Goto("LoopTest");

            Label("LoopBody");
            MStore(di, al);
            BranchIf(Ne(al, 0), "ok");

            Assign(ax, -1);
            Goto("Done");

            Label("ok");
            BranchIf(Ne(al,0x0D), "LoopTest");
            MStore(Word16(0x302), IAdd(Mem16(Word16(0x0302)), 1));

            Label("LoopTest");
            Assign(al, Mem8(si));
            Assign(si, IAdd(si, 1));
            BranchIf(Eq(al, Byte(0x20)), "LoopBody");
            Assign(ax, ISub(si,bx)); 

            Label("Done");
            MStore(Word16(0x300), ax);
            Return(ax);
        }

    }
}
