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
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Structure;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using Reko.Core.Types;

namespace Reko.UnitTests.Structure
{
    [TestFixture]
    public class AbsynCodeGeneratorTests
    {
        private string nl = Environment.NewLine;
        private Procedure proc;
        private StringWriter sb;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
        }

        private void CompileTest(ProcedureBuilder mock)
        {
            proc = mock.Procedure;
            var sa = new StructureAnalysis(new FakeDecompilerEventListener(), new Program(), proc);
            sa.Structure();
        }

        private void RunTest(string sExp)
        {
            try
            {
                GenCode(proc, sb);
                Assert.AreEqual(sExp, sb.ToString());
            }
            catch
            {
                proc.Dump(true);
                Console.WriteLine(sb.ToString());
                throw;
            }
        }

        private void GenCode(Procedure proc, StringWriter sb)
        {
            sb.WriteLine("{0}()", proc.Name);
            sb.WriteLine("{");

            CodeFormatter cf = new CodeFormatter(new TextFormatter(sb));
            cf.WriteStatementList(proc.Body);

            sb.WriteLine("}");
        }

        [Test]
        public void Acg_Return()
        {
            CompileTest(m =>
            {
                m.Return();
            });
            RunTest(
                "ProcedureBuilder()" + nl +
                "{" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_IfThen()
        {
            CompileTest(m =>
            {
                m.BranchIf(m.Fn("foo"), "skip");
                m.SideEffect(m.Fn("bar"));
                m.Label("skip");
                m.SideEffect(m.Fn("baz"));
                m.Return();
            });
            RunTest(
                "ProcedureBuilder()" + nl + 
                "{" + nl +
                "\tif (!foo())" + nl+ 
                "\t\tbar();" + nl +
                "\tbaz();" + nl+ 
                "}" + nl);
        }

        [Test]
        public void Acg_IfThenElse()
        {
            CompileTest(m =>
            {
                m.BranchIf(m.Fn("foo"), "else");
                m.Label("then");
                    m.SideEffect(m.Fn("Then"));
                    m.Goto("end");
                m.Label("else");
                    m.SideEffect(m.Fn("Else"));
                m.Label("end");
                m.Return();
            });

            RunTest(
                "ProcedureBuilder()" + nl +
                "{" + nl +
                "\tif (!foo())" + nl +
                "\t\tThen();" + nl +
                "\telse"  + nl +
                "\t\tElse();" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_NestedIfs()
        {
            CompileTest(new NestedIfs());
            RunTest(
                "NestedIfs()" + nl +
                "{" + nl +
                "	if (ax >= 0)" + nl +
                "	{" + nl +
                "		cl = 0x00;" + nl +
                "		if (ax > 12)" + nl +
                "			ax = 12;" + nl +
                "	}" + nl +
                "	else" + nl +
                "	{" + nl +
                "		cl = 0x01;" + nl +
                "		if (ax < -12)" + nl +
                "			ax = -12;" + nl +
                "	}" + nl +
                "}" + nl);

        }

        [Test]
        public void Acg_WhileLoop()
        {
            CompileTest(new WhileLoopFragment());
            RunTest(
                "WhileLoopFragment()" + nl + 
                "{" + nl + 
                "	sum = 0;" + nl + 
                "	for (i = 0; i < 100; ++i)" + nl +
                "		sum += i;" + nl + 
                "	return sum;" + nl + 
                "}" + nl );
        }
        
        [Test]
        public void Acg_BigLoopHead()
        {
            CompileTest(new BigLoopHeadFragment());
            RunTest(
                "BigLoopHeadFragment()" + nl + 
                "{" + nl + 
                "	while (true)" + nl + 
                "	{" + nl +
                "		DoSomething();" + nl +
                "		DoSomethingelse();" + nl +
                "		if (IsDone())" + nl +
                "			break;" + nl +
                "		LoopWork();" + nl +
                "	}" + nl +
                "}" + nl);

        }

        [Test]
        public void Acg_DoWhile()
        {
            CompileTest(new DoWhileFragment());
            RunTest(
                "DoWhileFragment()" + nl + 
                "{" + nl + 
                "	do" + nl +
                "		Frobulate();" + nl +
                "	while (!DoneFrobbing());" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_DoWhileIf()
        {
            CompileTest(new DoWhileIfFragment());
            RunTest(
                "DoWhileIfFragment()" + nl + 
                "{" + nl + 
                "	do" + nl + 
                "	{" + nl + 
                "		DoStuff();" + nl + 
                "		if (NeedsBork())" + nl + 
                "			Bork();" + nl + 
                "	} while (!Done());" + nl + 
                "}" + nl);
        }

        [Test]
        public void Acg_WhileGoto()
        {
            CompileTest(new MockWhileGoto());
            RunTest(
                "MockWhileGoto()" + nl +
                "{" + nl +
                "	while (foo())" + nl +
                "	{" + nl +
                "		bar();" + nl +
                "		if (foo())" + nl +
                "		{" + nl +
                "			extraordinary();" + nl +
                "			goto end;" + nl +
                "		}" + nl +
                "		bar2();" + nl +
                "	}" + nl +
                "	bar3();" + nl +
                "end:" + nl + 
                "	bar4();" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_UnstructuredIfs()
        {
            CompileTest(new UnstructuredIfsMock());
            RunTest(
                "UnstructuredIfsMock()" + nl +
                "{" + nl +
                "	if (!foo)" + nl +
                "		quux();" + nl +
                "	else if (!bar)" + nl +
                "	{" + nl +
                "		baz();" + nl +
                "		return;" + nl +
                "	}" + nl +
                "	niz();" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_WhileBreak()
        {
            CompileTest(new MockWhileBreak());
            RunTest(
                "MockWhileBreak()" + nl +
                "{" + nl +
                "	r2 = 0;" + nl +
                "	while (r1 != 0)" + nl +
                "	{" + nl +
                "		r3 = Mem0[r1:word32];" + nl +
                "		r2 += r3;" + nl +
                "		r3 = Mem0[r1 + 4:word32];" + nl +
                "		if (r3 == 0)" + nl +
                "			return r2;" + nl +
                "		r1 = Mem0[r1 + 12:word32];" + nl +
                "	}" + nl +
                "	return r2;" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_CaseJumpsBack()
        {
            CompileTest(new MockCaseJumpsBack());
            RunTest(
                "MockCaseJumpsBack()" + nl +
                "{" + nl +
                "	Beginning();" + nl +
                "JumpBack:" + nl +
                "	DoWorkBeforeSwitch();" + nl +
                "	switch (n)" + nl +
                "	{" + nl +
                "	case 0x00000000:" + nl +
                "		print(n);" + nl +
                "		break;" + nl +
                "	case 0x00000001:" + nl +
                "		++n;" + nl +
                "		goto JumpBack;" + nl +
                "	case 0x00000002:" + nl +
                "		print(n);" + nl +
                "		break;" + nl +
                "	}" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_CaseStatement()
        {
            CompileTest(new MockCaseStatement());
            RunTest(
                "MockCaseStatement()" + nl +
                "{" + nl +
                "	switch (w)" + nl +
                "	{" + nl +
                "	case 0x0000:" + nl +
                "		fn0();" + nl +
                "		break;" + nl +
                "	case 0x0001:" + nl +
                "		fn1();" + nl +
                "		break;" + nl +
                "	case 0x0002:" + nl +
                "		fn2();" + nl +
                "		break;" + nl +
                "	}" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_NestedWhileLoops()
        {
            CompileTest(new MockNestedWhileLoops());
            RunTest(
                "MockNestedWhileLoops()" + nl +
                "{" + nl +
                "	int32 i;" + nl +
                "	for (i = 0; i < 10; ++i)" + nl +
                "	{" + nl +
                "		int32 j;" + nl +
                "		for (j = 0; j < 10; ++j)" + nl +
                "			Mem0[0x00001234:word32] = Mem0[0x00001234:int32] + j;" + nl +
                "	}" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_AcgWhileReturn()
        {
            CompileTest(m =>
            {
                m.Label("head");
                m.BranchIf(m.Local32("done"), "loop_done");
                m.Assign(m.Local32("done"), m.Fn("fn"));
                m.BranchIf(m.Local32("breakomatic"), "done");
                m.Assign(m.Local32("grux"), m.Fn("foo"));
                m.Goto("head");
                m.Label("loop_done");
                m.SideEffect(m.Fn("extra"));
                m.Label("done");
                m.Return();
            });
            RunTest(
                "ProcedureBuilder()" + nl +
                "{" + nl +
                "	while (!done)" + nl +
                "	{" + nl +
                "\t\tdone = fn();" + nl +
                "		if (breakomatic)" + nl +
                "\t\t\treturn;" + nl +
                "		grux = foo();" + nl +
                "\t}" + nl +
                "\textra();" + nl +
                "}" + nl);
        }


        [Test]
        public void Acg_AcgWhileWithDeclarations()
        {
            CompileTest(new MockWhileWithDeclarations());
            RunTest(
                "MockWhileWithDeclarations()" + nl +
                "{" + nl +
                "	while (true)" + nl +
                "	{" + nl +
                "		byte v = Mem0[i:byte];" + nl +
                "		++i;" + nl +
                "		if (v == 0x20)" + nl +
                "			break;" + nl +
                "		Mem0[0x00300000:byte] = v;" + nl +
                "	}" + nl +
                "}" + nl);
        }

        [Test]
        public void Acg_AcgBranchesToReturns()
        {
            CompileTest(m =>
            {
                var a1 = m.Local16("a1"); 
                m.Assign(a1, m.Fn("fn0540"));
                var tmp = m.Local16("tmp");
                m.Assign(tmp, m.Mem16(m.Word16(0x8416)));
                m.BranchIf(m.Ne(tmp, 0), "branch_c");

                m.Label("Branch_a");
                    m.MStore(m.Word16(0x8414), m.Word16(0));
                    m.BranchIf(m.Eq(m.Word16(0x8414), 0x0000), "branch_c");

                m.Label("Branch_b");
                    Identifier ax_96 = m.Local16("ax_96");
                    m.SideEffect(m.Fn("fn02A9", m.AddrOf(PrimitiveType.Ptr32, ax_96)));
                    m.Return(ax_96);

                m.Label("branch_c");
	                m.Return(a1);
            });
            RunTest(
"ProcedureBuilder()" + nl +
"{" + nl +
"	a1 = fn0540();" + nl +
"	tmp = Mem0[0x8416:word16];" + nl +
"	if (tmp != 0x0000)" + nl +
"		return a1;" + nl +
"	Mem0[0x8414:word16] = 0x0000;" + nl +
"	if (0x8414 == 0x0000)" + nl +
"		return a1;" + nl +
"	fn02A9(&ax_96);" + nl +
"	return ax_96;" + nl +
"}" + nl);


        }

        [Test]
        public void Acg_AcgInfiniteLoop()
        {
            CompileTest(new MockInfiniteLoop());
            RunTest(
                "MockInfiniteLoop()" + nl +
                "{" + nl +
                "	while (true)" + nl +
                "		DispatchEvents();" + nl +
                "}" + nl);             
        }

        [Test]
        public void Acg_AcgInfiniteLoop2()
        {
            CompileTest(m =>
            {
                m.Label("Infinity");
                m.BranchIf(m.Eq(m.Mem16(m.Word16(0x1234)), 0), "hop");
                m.SideEffect(m.Fn("foo"));
                m.Label("hop");
                m.BranchIf(m.Eq(m.Mem16(m.Word16(0x5123)), 1), "Infinity");
                m.SideEffect(m.Fn("bar"));
                m.Goto("Infinity");
                m.Return();
            });
            RunTest(
                "ProcedureBuilder()" + nl +
                "{" + nl +
                "\twhile (true)" + nl +
                "\t{" + nl + 
                "\t\tif (Mem0[0x1234:word16] != 0x0000)" + nl +
                "\t\t\tfoo();" + nl + 
                "\t\tif (Mem0[0x5123:word16] != 0x0001)" + nl +
                "\t\t\tbar();" +nl +
                "\t}" + nl +
                "}" + nl);
        }

        private void CompileTest(Action<ProcedureBuilder> gen)
        {
            ProcedureBuilder mock = new ProcedureBuilder();
            gen(mock);
            CompileTest(mock);
        }
    }
}