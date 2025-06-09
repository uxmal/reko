#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Structure;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;

namespace Reko.UnitTests.Decompiler.Structure
{
    [TestFixture]
    public class StructureAnalysisTests
    {
        private ProcedureBuilder m;
        private CompoundConditionCoalescer ccc;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        private void Given_CompoundConditionCoalescer(Procedure proc)
        {
            ccc = new CompoundConditionCoalescer(proc);
        }

        private static StringConstant Sz(string str)
        {
            return Constant.String(str, StringType.NullTerminated(PrimitiveType.Char));
        }

        private void RunTest(string sExp, Procedure proc)
        {
            var cfgc = new ControlFlowGraphCleaner(proc);
            cfgc.Transform();
            if (ccc is not null)
            {
                ccc.Transform();
            }
            var ps = new StructureAnalysis(new FakeDecompilerEventListener(), new Program(), proc);
            var reg = ps.Execute();
            var sb = new StringWriter();
            reg.Write(sb);
            sb.GetStringBuilder().Replace("\t", "    ");
            var s = sb.ToString();
            if (sExp != s)
            {
                Debug.WriteLine(s);
                Console.WriteLine(s);
                Assert.AreEqual(sExp, s);
            }
        }

        [Test]
        public void StrAnls_Simple()
        {
            m.Return();

            var sExp =
@"    return;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_DoNoCleanProcedureCall()
        {
            m.Label("head");
            m.BranchIf(m.Fn("someCheck"), "failed");
            m.Goto("exit");
            m.Label("failed");
            m.Label("exit");
            m.Return();

            var sExp =
@"    someCheck();
    return;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_IfThen()
        {
            var r1 = m.Reg32("r1", 1);
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
        public void StrAnls_IfThenElse()
        {
            var r1 = m.Reg32("r1", 1);
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
        public void StrAnls_ReturnInsideOfIf()
        {
            m.BranchIf(m.Fn("canceled"), "exit");

            m.BranchIf(m.Fn("canStart"), "start");

            m.SideEffect(m.Fn("wait"));
            m.BranchIf(m.Not(m.Fn("canStart")), "exit");

            m.Label("start");
            m.SideEffect(m.Fn("start"));

            m.Label("exit");
            m.Return();

            var sExp =
@"    if (canceled())
        return;
    if (!canStart())
    {
        wait();
        if (!canStart())
            return;
    }
    start();
    return;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_While()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
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
        public void StrAnls_While2()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("start");
            m.Goto("head");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
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
        public void StrAnls_While_BreakAtTheEndOfBody()
        {
            m.Label("head");
            m.BranchIf(m.Not(m.Fn("next")), "done");

            m.Label("loop");
            m.SideEffect(m.Fn("process"));
            m.BranchIf(m.Fn("cancel"), "done");
            m.Goto("head");

            m.Label("done");
            m.SideEffect(m.Fn("finalize"));
            m.Return(m.Int32(1));

            var sExp =
@"    while (next())
    {
        process();
        if (cancel())
            break;
    }
    finalize();
    return 1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_While_LastResort()
        {
            m.Label("head");
            m.BranchIf(m.Not(m.Fn("next")), "done");

            m.BranchIf(m.Fn("needFullCheck"), "fullCheck");

            m.BranchIf(m.Fn("fastCheckFailed"), "failed");
            m.SideEffect(m.Fn("calc"));
            m.Goto("wait");

            m.Label("fullCheck");
            m.BranchIf(m.Fn("fullCheckFailed"), "failed");
            m.Goto("wait");

            m.Label("failed");
            m.SideEffect(m.Fn("throwError"));

            m.Label("wait");
            m.SideEffect(m.Fn("wait"));
            m.Goto("head");

            m.Label("done");
            m.SideEffect(m.Fn("finalize"));
            m.Return(m.Int32(0));

            var sExp =
@"    while (next())
    {
        if (!needFullCheck())
        {
            if (fastCheckFailed())
                goto failed;
            calc();
        }
        else if (fullCheckFailed())
        {
failed:
            throwError();
        }
        wait();
    }
    finalize();
    return 0;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_BigHeadWhile()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head");
            m.MStore(m.Word32(0x1000), r2);
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (true)
    {
        Mem0[0x1000:word32] = r2;
        if (r1 == r2)
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
        public void StrAnls_DoWhile()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
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
        public void StrAnls_DoWhile_InvertedCondition()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.BranchIf(m.Ne(r1, r2), "done");
            m.Goto("loop");
            m.Label("done");
            m.Return(r2);

            var sExp =
@"    do
    {
        Mem0[r1:word32] = Mem0[r2:word32];
        r1 = r1 + 0x04;
        r2 = r2 + 0x04;
    } while (r1 == r2);
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_NestedWhile()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head1");
            m.BranchIf(m.Ge(r1, 4), "done1");

                m.Label("body1");
                m.Assign(r2, m.Word32(0));
                m.Label("head2");
                m.BranchIf(m.Ge(r2, 4), "done2");

                    m.Label("body2");
                    m.MStore(m.IAdd(
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
        r2 = 0x00;
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
        public void StrAnls_WhileBreak()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
            m.BranchIf(m.Mem32(r2), "done");
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Assign(r2, r1);
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
    r2 = r1;
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test(Description="Here, the block leaving the loop does some work first.")]
        public void StrAnls_WhileBreak2()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
            m.BranchIf(m.Not(m.Mem32(r2)), "rest");

            m.Label("leaving");
            m.Assign(r2, 0);
            m.Goto("done");

            m.Label("rest");
            m.Assign(r1, m.IAdd(r1, 4));
            m.Assign(r2, m.IAdd(r2, 4));
            m.Goto("head");

            m.Label("done");
            m.Assign(r2, r1);
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
    r2 = r1;
    return r2;
";
            RunTest(sExp, m.Procedure);
        }


        [Test(Description = "This forces a goto, because the loop leaving goto isn't going to the follow node.")]
        public void StrAnls_WhileGoto()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Label("head");
            m.BranchIf(m.Eq(r1, r2), "done");

            m.Label("loop");
            m.MStore(r1, m.Mem32(r2));
            m.BranchIf(m.Not(m.Mem32(r2)), "rest");

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
            m.Assign(r2, r1);
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
    r2 = ~0x00;
end_fn:
    r2 = r1;
    return r2;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_UnstructuredExit_NILZ()
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
        public void StrAnls_InfiniteLoop_BreakInsideOfNestedIfs()
        {
            m.Label("loopheader");

            m.SideEffect(m.Fn("beforeCheck"));
            m.BranchIf(m.Fn("skipCheck"), "loop");
            m.BranchIf(m.Fn("failed"), "exit");
            m.SideEffect(m.Fn("check"));

            m.Label("loop");
            m.SideEffect(m.Fn("afterCheck"));
            m.Goto("loopheader");

            m.Label("exit");
            m.SideEffect(m.Fn("exit"));
            m.Return();

            var sExp =
@"    while (true)
    {
        beforeCheck();
        if (!skipCheck())
        {
            if (failed())
            {
                exit();
                return;
            }
            check();
        }
        afterCheck();
    }
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_InfiniteLoop()
        {
            var r1 = m.Reg32("r1", 1);

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
        public void StrAnls_Switch()
        {
            var r1 = m.Reg32("r1", 1);

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
        public void StrAnls_Switch_Fallthru()
        {
            var r1 = m.Reg32("r1", 1);

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

        [Test]
        public void StrAnls_Switch_Fallthru_CoincidentTargets()
        {
            var r1 = m.Reg32("r1", 1);

            m.Label("head");
            m.Switch(r1, "target_2", "target_0", "target_1", "target_2");

            m.Label("target_0");
            m.Assign(r1, 0);
            m.Goto("done");

            m.Label("target_1");
            m.Assign(r1, 1);
            // Fallthru

            m.Label("target_2");
            m.Assign(r1, 2);
            m.Goto("done");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case 0x00:
    case 0x03:
        goto target_2;
    case 0x01:
        r1 = 0x00;
        break;
    case 0x02:
        r1 = 0x01;
target_2:
        r1 = 0x02;
        break;
    }
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_Switch_Fallthru_IrregularCaseExits()
        {
            var r1 = m.Reg32("r1", 1);

            m.Label("head");
            m.Switch(r1, "case_0", "case_1");

            m.Label("case_0");
            m.BranchIf(m.Fn("done"), "done");
            m.Assign(r1, 2);
            // Fallthru

            m.Label("case_1");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case 0x00:
        if (!done())
        {
            r1 = 0x02;
            goto case_1;
        }
        break;
    case 0x01:
case_1:
        r1 = 0x01;
        break;
    }
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_Switch_IrregularEntries_AllCasesAreTails()
        {
            var r1 = m.Reg32("r1", 1);

            m.BranchIf(m.Fn("check"), "case_0");
            m.Switch(r1, "case_0", "case_1");

            m.Label("case_0");
            m.Assign(r1, 2);
            m.Return(m.IMul(r1, 3));

            m.Label("case_1");
            m.Assign(r1, 1);
            m.Return(m.IMul(r1, 4));

            var sExp =
@"    if (!check())
    {
        switch (r1)
        {
        case 0x00:
            break;
        case 0x01:
            r1 = 0x01;
            return r1 * 0x04;
        }
    }
    r1 = 0x02;
    return r1 * 0x03;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_Switch_SingleCase()
        {
            var r1 = m.Reg32("r1", 1);

            m.SideEffect(m.Fn("initialize"));
            m.BranchIf(m.Gt(r1, 3), "finalize");
            m.Switch(r1, "case_0");

            m.Label("case_0");
            m.Assign(r1, 2);
            m.Goto("finalize");

            m.Label("finalize");
            m.SideEffect(m.Fn("finalize"));

            var sExp =
@"    initialize();
    if (r1 <= 3)
    {
        switch (r1)
        {
        case 0x00:
            r1 = 0x02;
            break;
        }
    }
    finalize();
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_Switch_LastResort()
        {
            var r1 = m.Reg32("r1", 1);

            m.Switch(r1, "case_0", "case_1", "case_2");

            m.Label("case_0");
            m.Assign(r1, 3);
            m.Goto("done");

            m.Label("case_1");
            m.Assign(r1, 2);
            m.BranchIf(m.Fn("needFullCheck"), "fullCheck");

            m.BranchIf(m.Fn("fastCheckFailed"), "failed");
            m.Goto("done");

            m.Label("fullCheck");
            m.BranchIf(m.Fn("fullCheckFailed"), "failed");
            m.Goto("done");

            m.Label("failed");
            m.SideEffect(m.Fn("throwError"));
            m.Goto("done");

            m.Label("case_2");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.SideEffect(m.Fn("finalize"));
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case 0x00:
        r1 = 0x03;
        break;
    case 0x01:
        r1 = 0x02;
        if (!needFullCheck())
        {
            if (fastCheckFailed())
                goto failed;
        }
        else if (fullCheckFailed())
        {
failed:
            throwError();
        }
        break;
    case 0x02:
        r1 = 0x01;
        break;
    }
    finalize();
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test(Description="A do-while with a nested if-then-else")]
        public void StrAnls_DoWhile_NestedIfElse()
        {
            var r1 = m.Reg32("r1", 1);

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
        public void StrAnls_DoWhile_ManyContinues()
        {
            var r1 = m.Reg32("r1", 1);

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

        [Test]
        public void StrAnls_DoWhile_NestedSwitch()
        {
            var r1 = m.Reg32("r1", 1);

            m.Label("head");
            m.BranchIf(m.Fn("action"), "done");
            m.Switch(r1, "case_0", "case_1");

            m.Label("case_0");
            m.BranchIf(m.Fn("done"), "done");
            m.Assign(r1, 2);
            // Fallthru

            m.Label("case_1");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.BranchIf(m.Eq(r1, 0), "head");
            m.Return(r1);

            var sExp =
@"    do
    {
        if (!action())
        {
            switch (r1)
            {
            case 0x00:
                if (!done())
                {
                    r1 = 0x02;
                    goto case_1;
                }
                break;
            case 0x01:
case_1:
                r1 = 0x01;
                break;
            }
        }
    } while (r1 == 0x00);
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_DoWhile_Return()
        {
            var r1 = m.Reg32("r1", 1);

            m.Label("head");

            m.BranchIf(m.Fn("check"), "ok");
            m.Return(m.Int32(-1));

            m.Label("ok");
            m.BranchIf(m.Fn("next"), "head");

            m.Return(r1);

            var sExp =
@"    do
    {
        if (!check())
            return -1;
    } while (next());
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_DoWhile_BreakAtTheStartOfBody()
        {
            m.Label("head");
            m.SideEffect(m.Fn("process"));
            m.BranchIf(m.Fn("cancel"), "done");
            m.BranchIf(m.Not(m.Fn("next")), "done");
            m.Goto("head");

            m.Label("done");
            m.SideEffect(m.Fn("finalize"));
            m.Return(m.Int32(1));

            var sExp =
@"    do
    {
        process();
        if (cancel())
            break;
    } while (next());
    finalize();
    return 1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_DoNotLoseLabels()
        {
            m.BranchIf(m.Fn("check"), "left");
            m.Goto("right");

            m.Label("left");
            m.BranchIf(m.Fn("leftCheck"), "easy");
            m.Goto("medium");

            m.Label("right");
            m.BranchIf(m.Fn("rightCheck"), "difficult");
            m.Goto("medium");

            m.Label("easy");
            m.BranchIf(m.Fn("easyCheck"), "right");
            m.Goto("medium");

            m.Label("medium");
            m.BranchIf(m.Fn("mediumCheck"), "easy");
            m.Goto("difficult");

            m.Label("difficult");
            m.BranchIf(m.Fn("difficultCheck"), "left");
            m.Goto("medium");

            var sExp =
@"    if (check())
    {
left:
        if (leftCheck())
            goto easy;
        goto medium;
    }
right:
    if (!rightCheck())
    {
medium:
        if (mediumCheck())
        {
easy:
            if (!easyCheck())
                goto medium;
            goto right;
        }
    }
    if (difficultCheck())
        goto left;
    goto medium;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_r00237()
        {
            //byte fn0800_0541(byte al, selector ds)

            var ds = m.Temp(PrimitiveType.SegmentSelector, "ds");
            var cx_10 = m.Temp(PrimitiveType.Word16, "cx_10");
            var si_12 = m.Temp(PrimitiveType.Word16, "si_12");
            var ah_13 = m.Temp(PrimitiveType.Byte, "al_13");
            var al_43 = m.Temp(PrimitiveType.Byte, "al_43");
            var Z_26 = m.Temp(PrimitiveType.Byte, "Z_26");

            m.Assign(cx_10, 20000);
m.Label("l0800_0544");
                m.Assign(si_12, 0x8E8A);
                m.Assign(ah_13, 0x00);
m.Label("l0800_054A");
                    m.Assign(si_12, m.IAdd(si_12, 0x01));
                    m.BranchIf(m.Eq0(m.SegMem8(ds, si_12)), "l0800_0557");
 m.Label("l0800_054F");
                        m.Assign(ah_13, 0x01);
                        m.Assign(Z_26, m.Cond(m.ISub(si_12, m.SegMem16(ds, m.Word16(0x8F0B)))));
                        m.BranchIf(m.Ne(si_12, m.SegMem16(ds, m.Word16(0x8F0B))), "l0800_055F");
 m.Label("l0800_0557");
            m.Assign(Z_26, m.Cond(m.ISub(si_12, 0x8F0A)));
            m.BranchIf(m.Eq(si_12, 0x8F0A), "l0800_055F");
m.Label("l0800_055D");
            m.Goto("l0800_054A");
m.Label("l0800_055F");
            m.BranchIf(Z_26, "l0800_0578");
m.Label("l0800_0561");
            m.SStore(ds, m.Word16(0x8F0B), si_12);
            m.Assign(al_43, m.SegMem8(ds, m.ISub(si_12, 0x8E31)));
            m.BranchIf(m.Eq0(al_43), "l0800_0576");
m.Label("l0800_0571");
            m.BranchIf(m.Ge(al_43, 0x00), "l0800_0575");
m.Label("l0800_0573");
            m.Assign(al_43, 0x00);
m.Label("l0800_0575");
            m.Return(al_43);
m.Label("l0800_0576");
            m.Goto("l0800_0583");
m.Label("l0800_0578");
            m.BranchIf(m.Ne0(ah_13), "l0800_0583");
m.Label("l0800_057D");
            m.SStore(ds, m.Word16(0x8F0B), m.Byte(0));
m.Label("l0800_0583");
            m.Assign(cx_10, m.ISub(cx_10, 0x01));
            m.BranchIf(m.Ne0(cx_10), "l0800_0544");
m.Label("l0800_0585");
            m.Return(m.Byte(0x00));

            var sExp =
            #region Expected
@"    cx_10 = 20000;
    do
    {
        si_12 = 0x8E8A;
        al_13 = 0x00;
        do
        {
            si_12 = si_12 + 0x01;
            if (Mem0[ds:si_12:byte] != 0x00)
            {
                al_13 = 0x01;
                Z_26 = cond(si_12 - Mem0[ds:0x8F0B:word16]);
                if (si_12 != Mem0[ds:0x8F0B:word16])
                    break;
            }
            Z_26 = cond(si_12 - 0x8F0A);
        } while (si_12 != 0x8F0A);
        if (!Z_26)
        {
            Mem0[ds:0x8F0B:word16] = si_12;
            al_43 = Mem0[ds:si_12 - 0x8E31:byte];
            if (al_43 != 0x00)
            {
                if (al_43 < 0x00)
                    al_43 = 0x00;
                return al_43;
            }
        }
        else if (al_13 == 0x00)
            Mem0[ds:0x8F0B:byte] = 0x00;
        cx_10 = cx_10 - 0x01;
    } while (cx_10 != 0x00);
    return 0x00;
";
            #endregion

            RunTest(sExp, m.Procedure);
        }

        [Test(Description = "This was failing because the compound condition coalescer required the CFG cleaner" +
            " to execute first.")]
        public void StrAnls_Issue_529()
        {
            var m = new ProcedureBuilder();
            var fp = m.Frame.FramePointer;
            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            var puts = new ExternalProcedure("puts", new FunctionType());

            m.Label("m4E2");
            m.Goto("m4F7");

            m.Label("m4E4");
            m.SideEffect(m.Fn(puts, Constant.String("Hello", StringType.NullTerminated(PrimitiveType.Byte))));
            m.Return();

            m.Label("m4F7");
            m.BranchIf(m.Eq0(m.Mem32(m.Word32(0x0808A0A4))), "m502");
            m.Label("m500");
            m.Goto("m50D");

            m.Label("m502");
            m.BranchIf(m.Eq0(m.Mem32(m.Word32(0x0808A0A8))), "m4E4");
            m.Goto("m50D");
            m.Label("m50D");
            m.SideEffect(m.Fn(puts, Constant.String("Goodbye", StringType.NullTerminated(PrimitiveType.Byte))));
            m.Goto("m4E4");

            var sExp =
            #region Expected
@"    if (Mem0[0x0808A0A4:word32] != 0x00 || Mem0[0x0808A0A8:word32] != 0x00)
        puts(""Goodbye"");
    puts(""Hello"");
    return;
";
            #endregion
            Given_CompoundConditionCoalescer(m.Procedure);
            RunTest(sExp, m.Procedure);
        }

        [Test(Description = "Github issue #874 opened by @blindmatrix")]
        public void StrAnls_SwitchWithOffset()
        {
            var r1 = m.Reg32("r1", 1);

            m.Label("head");
            m.Switch(m.IAddS(r1, 1), "case_m1", "case_0", "case_1");

            m.Label("case_m1");
            m.Assign(r1, 3);
            m.Goto("done");

            m.Label("case_0");
            m.Assign(r1, 2);
            m.Goto("done");

            m.Label("case_1");
            m.Assign(r1, 1);
            m.Goto("done");

            m.Label("done");
            m.Return(r1);

            var sExp =
@"    switch (r1)
    {
    case ~0x00:
        r1 = 0x03;
        break;
    case 0x00:
        r1 = 0x02;
        break;
    case 0x01:
        r1 = 0x01;
        break;
    }
    return r1;
";
            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_SwitchInLoop()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Assign(r1, m.Mem32(m.Word32(0x00123400)));

            m.Label("m1Loop");
            m.Assign(r1, m.Mem32(m.Word32(0x00123404)));
            m.BranchIf(m.Eq0(r1), "m4SwitchDone");

            m.Label("m2CheckRange");
            m.BranchIf(m.Ge(r1, 4), "m1Loop");

            m.Label("m3Switch");
            m.Switch(r1, "m4case0", "m1Loop", "m1Loop", "m1Loop");

            m.Label("m4case0");
            m.Assign(r2, Sz("case 0"));
            m.Goto("m1Loop");

            m.Label("m4SwitchDone");
            m.Return();

            var sExp =
@"    r1 = Mem0[0x00123400:word32];
    while (true)
    {
        r1 = Mem0[0x00123404:word32];
        if (r1 == 0x00)
            break;
        if (r1 < 0x04)
        {
            switch (r1)
            {
            case 0x00:
                r2 = ""case 0"";
                break;
            case 0x01:
            case 0x02:
            case 0x03:
                break;
            }
        }
    }
    return;
";
            RunTest(sExp, m.Procedure);
        }


        [Test]
        public void StrAnls_NestedSwitch()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Assign(r1, m.Mem32(m.Word32(0x00123400)));

            m.Switch(r1, "case_0", "case_1", "dcase_1");

            m.Label("case_0");
            m.Assign(r2, Sz("case 0"));
            m.Goto("done");

            m.Label("case_1");
            m.Switch(r1, "dcase_0", "dcase_1", "dcase_2");

                m.Label("dcase_0");
                m.Assign(r2, Sz("dcase 0"));
                m.Goto("inner_done");

                m.Label("dcase_1");
                m.Assign(r2, Sz("dcase 1"));
                m.Goto("inner_done");

                m.Label("dcase_2");
                m.Assign(r2, Sz("dcase 2"));
                m.Goto("inner_done");

                m.Label("inner_done");
                m.Goto("done");

            m.Label("done");
            m.Return(r2);

            var sExp = 
@"    r1 = Mem0[0x00123400:word32];
    switch (r1)
    {
    case 0x00:
        r2 = ""case 0"";
        break;
    case 0x01:
        switch (r1)
        {
        case 0x00:
            r2 = ""dcase 0"";
            break;
        case 0x01:
            goto dcase_1;
        case 0x02:
            r2 = ""dcase 2"";
            break;
        }
        goto inner_done;
    case 0x02:
dcase_1:
        r2 = ""dcase 1"";
inner_done:
        break;
    }
    return r2;
";

            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_NestedSwitch_DuplicateCases()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);

            m.Assign(r1, m.Mem32(m.Word32(0x00123400)));

            m.Switch(r1, "case_0", "case_1",  "dcase_1", "dcase_1", "case_1");

            m.Label("case_0");
            m.Assign(r2, Sz("case 0"));
            m.Goto("done");

            m.Label("case_1");
            m.Switch(r1, "dcase_1", "dcase_1", "dcase_2");

            m.Label("dcase_1");
            m.Assign(r2, Sz("dcase 1"));
            m.Goto("inner_done");

            m.Label("dcase_2");
            m.Assign(r2, Sz("dcase 2"));
            m.Goto("inner_done");

            m.Label("inner_done");
            m.Goto("done");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    r1 = Mem0[0x00123400:word32];
    switch (r1)
    {
    case 0x00:
        r2 = ""case 0"";
        break;
    case 0x01:
    case 0x04:
        switch (r1)
        {
        case 0x00:
        case 0x01:
            goto dcase_1;
        case 0x02:
            r2 = ""dcase 2"";
            break;
        }
        goto inner_done;
    case 0x02:
    case 0x03:
dcase_1:
        r2 = ""dcase 1"";
inner_done:
        break;
    }
    return r2;
";

            RunTest(sExp, m.Procedure);
        }

        [Test]
        public void StrAnls_Switch_Breaking_IntoLoop()
        {
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            var r3 = m.Reg32("r3", 2);

            m.Label("m1loopHead");
            m.BranchIf(m.Eq0(r1), "mexit");

            m.Label("m2switch");
            m.BranchIf(m.Cor(m.Lt0(r1), m.Gt(r1, 2)), "m4advance");

            m.Switch(r2, "m3_case0", "m1loophead", "m3_case2");

            m.Label("m3_case0");
            m.Assign(r2, Sz("m3_case0"));
            m.Goto("m4advance");

            m.Label("m3_case2");
            m.Assign(r2, Sz("m3_case2"));
            m.Goto("m4advance");

            m.Label("m4advance");
            m.Assign(r1, m.Mem32(m.IAdd(r1, 4)));
            m.Goto("m1loopHead");

            m.Label("mexit");
            m.Return(r2);
            m.Assign(r1, m.Mem32(m.Word32(0x00123400)));

            m.Switch(r1, "case_0", "case_1", "dcase_1", "dcase_1", "case_1");

            m.Label("case_0");
            m.Assign(r2, Sz("case 0"));
            m.Goto("done");

            m.Label("case_1");
            m.Switch(r1, "dcase_1", "dcase_1", "dcase_2");

            m.Label("dcase_1");
            m.Assign(r2, Sz("dcase 1"));
            m.Goto("inner_done");

            m.Label("dcase_2");
            m.Assign(r2, Sz("dcase 2"));
            m.Goto("inner_done");

            m.Label("inner_done");
            m.Goto("done");

            m.Label("done");
            m.Return(r2);

            var sExp =
@"    while (r1 != 0x00)
    {
        if (r1 >= 0x00 && r1 <= 2)
        {
            switch (r2)
            {
            case 0x00:
                r2 = ""m3_case0"";
                break;
            case 0x01:
            case 0x02:
                r2 = ""m3_case2"";
                break;
            }
        }
        r1 = Mem0[r1 + 0x04:word32];
    }
    return r2;
";

            RunTest(sExp, m.Procedure);
        }
    }
}
