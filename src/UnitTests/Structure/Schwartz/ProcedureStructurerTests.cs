﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Structure.Schwartz;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Structure.Schwartz
{
    [TestFixture]
    public class ProcedureStructurerTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        private void RunTest(string sExp, Procedure proc)
        {
            var ps = new ProcedureStructurer(proc);
            var reg = ps.Execute();
            var sb = new StringWriter();
            reg.Write(sb);
            sb.GetStringBuilder().Replace("\t", "    ");
            var s = sb.ToString();
            if (sExp != s)
            {
                Debug.WriteLine(s);
                Assert.AreEqual(sExp, s);
            }
        }

        [Test]
        public void ProcStr_Simple()
        {
            m.Return();

            var sExp =
@"    return;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_IfThen()
        {
            var r1 = m.Reg32("r1");
            m.Label("head");
            m.BranchIf(m.Le(r1, 0), "tail");
            m.Label("doit");
            m.Assign(r1, 0);
            m.Label("tail");
            m.Return(r1);

            var sExp =
@"    if (r1 > 0x00)
        r1 = 0x00;
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_IfThenElse()
        {
            var r1 = m.Reg32("r1");
            m.Label("head");
            m.BranchIf(m.Le(r1, 0), "thenn");
            m.Label("elsee");
            m.Assign(r1, 0);
            m.Goto("tail");

            m.Label("thenn");
            m.Assign(r1, 1);

            m.Label("tail");
            m.Return(r1);

            var sExp =
@"    if (r1 > 0x00)
        r1 = 0x00;
    else
        r1 = 0x01;
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_While()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (r1 != r2)
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_While2()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("start");
            m.Goto("head");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));

            m.Label("head");
            m.BranchIf(m.Ne(r1, r2), "loop");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (r1 != r2)
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_BigHeadWhile()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head");
            m.Store(m.Word32(0x1000), r2);
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (true)
    {
        Mem0[0x1000:word32] = r2;
        if (r1 != r2)
            break;
        Mem0[r1:word32] = Mem0[r2:word32];
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_DoWhile()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.BranchIf(m.Ne(r1, r2), "loop");
            m.Label("done");
            m.Return(r2);

            var sExp =
@"    do
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    } while (r1 != r2);
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_NestedWhile()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head1");
            m.BranchIf(m.Ge(r1, 4), "done1");

                m.Label("body1");
                m.Declare(r2, m.Word32(0));
                m.Label("head2");
                m.BranchIf(m.Ge(r2, 4), "done2");

                    m.Label("body2");
                    m.Store(m.IAdd(
                        m.Word32(0x1232100),
                        m.IAdd(
                            m.IMul(r1, 4),
                            r2)),
                        m.Byte(0));
                    m.Assign(r2, m.IAdd(r2, 1));
                    m.Goto("head2");

                m.Label("done2");
                m.Assign(r1, m.IAdd(r1, 1));
                m.Goto("head1");

            m.Label("done1");
            m.Return();

            var sExp =
            #region Expected 
 @"    while (r1 < 0x04)
    {
        word32 r2 = 0x00;
        while (r2 < 0x04)
        {
            Mem0[0x01232100 + (r1 * 0x04 + r2):byte] = 0x00;
            r2 = r2 + 0x01;
        }
        r1 = r1 + 0x01;
    }
    return;
";
            #endregion
            RunTest(sExp, m.Procedure);
        }


        [Test]
        public void ProcStr_WhileBreak()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.BranchIf(m.LoadDw(r2), "done");
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (r1 != r2)
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        if (Mem0[r2:word32])
            break;
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test(Description="Here, the block leaving the loop does some work first.")]
        public void ProcStr_WhileBreak2()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.BranchIf(m.Not(m.LoadDw(r2)), "rest");

            m.Label("leaving");
            m.Assign(r2, 0);
            m.Goto("done");

            m.Label("rest");
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (r1 != r2)
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        if (Mem0[r2:word32])
        {
            r2 = 0x00;
            break;
        }
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    return r2;
";
            RunTest(sExp, m.Procedure);
        }


        [Test(Description = "This forces a goto, because the loop leaving goto isn't going to the follow node.")]
        public void ProcStr_WhileGoto()
        {
            var r1 = m.Reg32("r1");
            var r2 = m.Reg32("r2");

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.Store(r1, m.LoadDw(r2));
            m.BranchIf(m.Not(m.LoadDw(r2)), "rest");

            m.Label("leaving");
            m.Assign(r2, 0);
            m.Goto("end_fn");

            m.Label("rest");
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Assign(r2, -1);

            m.Label("end_fn");
            m.Return(r2);

            var sExp =
@"    while (r1 != r2)
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        if (Mem0[r2:word32])
        {
            r2 = 0x00;
            goto end_fn;
        }
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    }
    r2 = 0xFFFFFFFF;
end_fn:
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_UnstructuredExit()
        {
            m.Label("loopheader");
            m.BranchIf(m.Fn("foo"), "done");

            m.SideEffect(m.Fn("bar"));
            m.BranchIf(m.Fn("foo"), "unstructuredexit");
            m.SideEffect(m.Fn("bar"));
            m.Goto("loopheader");

            m.Label("done");
            m.SideEffect(m.Fn("bar"));

            m.Label("unstructuredexit");
            m.SideEffect(m.Fn("bar"));
            m.Return();

            var sExp =
@"    while (!foo())
    {
        bar();
        if (foo())
            goto unstructuredexit;
        bar();
    }
    bar();
unstructuredexit:
    bar();
    return;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_InfiniteLoop()
        {
            var r1 = m.Reg32("r1");

            m.Label("head");
            m.Assign(r1, 0);

            m.Label("loop");
            m.Assign(r1, m.IAdd(r1, 1));
            m.Goto("loop");

            var sExp =
@"    r1 = 0x00;
    while (true)
        r1 = r1 + 0x01;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_Switch()
        {
            var r1 = m.Reg32("r1");

            m.Label("head");
            m.Switch(r1, "case_0", "case_1", "case_2");

            m.Label("case_0");
            m.Assign(r1, 3);
            m.Goto("done");

            m.Label("case_1");
            m.Assign(r1, 2);
            m.Goto("done");

            m.Label("case_2");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case 0x00:
        r1 = 0x03;
        break;
    case 0x01:
        r1 = 0x02;
        break;
    case 0x02:
        r1 = 0x01;
        break;
    }
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void ProcStr_Switch_Fallthru()
        {
            var r1 = m.Reg32("r1");

            m.Label("head");
            m.Switch(r1, "case_0", "case_1", "case_2");

            m.Label("case_0");
            m.Assign(r1, 3);
            m.Goto("done");

            m.Label("case_1");
            m.Assign(r1, 2);
            // Fallthru

            m.Label("case_2");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case 0x00:
        r1 = 0x03;
        break;
    case 0x01:
        r1 = 0x02;
        goto case_2;
    case 0x02:
case_2:
        r1 = 0x01;
        break;
    }
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test(Description="A do-while with a nested if-then-else")]
        public void ProcStr_DoWhile_NestedIfElse()
        {
            var r1 = m.Reg32("r1");

            m.Label("head");
            m.SideEffect(m.Fn("foo", r1));
            m.BranchIf(m.Eq(r1, 3), "not_3");

            m.Label("eq_3");
            m.SideEffect(m.Fn("bar", r1));
            m.Goto("join");

            m.Label("not_3");
            m.SideEffect(m.Fn("b"));

            m.Label("join");
            m.SideEffect(m.Fn("bloo"));
            m.BranchIf(m.Eq(r1, 2), "head");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    do
    {
        foo(r1);
        if (r1 != 0x03)
            bar(r1);
        else
            b();
        bloo();
    } while (r1 == 0x02);
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test(Description="A do-while loop with many continue statements.")]
        public void ProcStr_DoWhile_ManyContinues()
        {
            var r1 = m.Reg32("r1");

                m.Label("head");
                m.SideEffect(m.Fn("foo", r1));
                m.BranchIf(m.Eq(r1, 3), "not_3");

                    m.Label("eq_3");
                    m.SideEffect(m.Fn("bar", r1));
                    m.BranchIf(m.Eq(r1, 2), "not_2");

                        m.Label("eq_2");
                        m.SideEffect(m.Fn("b"));
                        m.BranchIf(m.Eq(r1, 1), "head");    // this should be a "continue" node.

                    m.Label("not_2");
                    m.SideEffect(m.Fn("bloo"));

                m.Label("not_3");
                m.SideEffect(m.Fn("baz", r1));
                m.BranchIf(m.Eq(r1, 2), "head");

            m.Label("done");
            m.Return(r1);
            
            var sExp =
@"    do
    {
        foo(r1);
        if (r1 != 0x03)
        {
            bar(r1);
            if (r1 != 0x02)
            {
                b();
                if (r1 == 0x01)
                    continue;
            }
            bloo();
        }
        baz(r1);
    } while (r1 == 0x02);
    return r1;
";
            RunTest(sExp, m.Procedure);
        }
    }
}
