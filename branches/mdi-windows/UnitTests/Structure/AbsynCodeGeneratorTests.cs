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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class AbsynCodeGeneratorTests
    {
        private string nl = Environment.NewLine;
        private Procedure proc;
        private ProcedureStructure curProc;
        private StringWriter sb;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
        }

        [Test]
        public void Return()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Return();
            });
            RunTest(
                "ProcedureMock()" + nl +
                "{" + nl +
                "\treturn;" + nl +
                "}" + nl);
        }

        [Test]
        public void IfThen()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.Fn("foo"), "skip");
                m.SideEffect(m.Fn("bar"));
                m.Label("skip");
                m.SideEffect(m.Fn("baz"));
                m.Return();
            });
            RunTest(
                "ProcedureMock()" + nl + 
                "{" + nl +
                "\tif (!foo())" + nl+ 
                "\t\tbar();" + nl +
                "\tbaz();" + nl+ 
                "\treturn;" + nl +
                "}" + nl);
        }

        [Test]
        public void IfThenElse()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.Fn("foo"), "else");
                m.Label("then");
                    m.SideEffect(m.Fn("Then"));
                    m.Jump("end");
                m.Label("else");
                    m.SideEffect(m.Fn("Else"));
                m.Label("end");
                m.Return();
            });

            RunTest(
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tif (!foo())" + nl +
                "\t\tThen();" + nl +
                "\telse"  + nl +
                "\t\tElse();" + nl +
                "\treturn;" + nl +
                "}" + nl);
        }

        [Test]
        public void NestedIfs()
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
                "	return;" + nl +
                "}" + nl);

        }

        [Test]
        public void WhileLoop()
        {
            CompileTest(new WhileLoopFragment());
            RunTest(
                "WhileLoopFragment()" + nl + 
                "{" + nl + 
                "	i = 0;" + nl + 
                "	sum = 0;" + nl + 
                "	while (i < 100)" + nl + 
                "	{" + nl + 
                "		sum = sum + i;" + nl + 
                "		i = i + 1;" + nl + 
                "	}" + nl + 
                "	return sum;" + nl + 
                "}" + nl );
        }
        
        [Test]
        public void BigLoopHead()
        {
            CompileTest(new BigLoopHeadFragment());
            RunTest(
                "BigLoopHeadFragment()" + nl + 
                "{" + nl + 
                "	DoSomething();" + nl + 
                "	DoSomethingelse();" + nl + 
                "	while (!IsDone())" + nl + 
                "	{" + nl +
                "		LoopWork();" + nl + 
                "		DoSomething();" + nl + 
                "		DoSomethingelse();" + nl + 
                "	}" + nl +
                "	return;" + nl + 
                "}" + nl);

        }

        [Test]
        public void DoWhile()
        {
            CompileTest(new DoWhileFragment());
            RunTest(
                "DoWhileFragment()" + nl + 
                "{" + nl + 
                "	do" + nl +
                "		Frobulate();" + nl +
                "	while (!DoneFrobbing());" + nl +
                "	return;" + nl + 
                "}" + nl);
        }

        [Test]
        public void DoWhileIf()
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
                "	}" + nl + 
                "	while (!Done());" + nl + 
                "	return;" + nl + 
                "}" + nl);
        }

        [Test]
        public void WhileGoto()
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
                "\treturn;" + nl +
                "}" + nl);
        }

        [Test]
        public void UnstructuredIfs()
        {
            CompileTest(new UnstructuredIfsMock());
            RunTest(
                "UnstructuredIfsMock()" + nl +
                "{" + nl +
                "	if (!foo)" + nl +
                "	{" + nl +
                "		quux();" + nl +
                "inside:" + nl +
                "		niz();" + nl +
                "	}" + nl +
                "	else if (!bar)" + nl +
                "\t\tbaz();" + nl +
                "\telse" + nl +
                "\t\tgoto inside;" + nl +
                "	return;" + nl +
                "}" + nl);
        }

        [Test]
        public void WhileBreak()
        {
            CompileTest(new MockWhileBreak());
            RunTest(
                "MockWhileBreak()" + nl +
                "{" + nl +
                "	r2 = 0x00000000;" + nl +
                "	while (r1 != 0x00000000)" + nl +
                "	{" + nl +
                "		r3 = Mem0[r1:word32];" + nl +
                "		r2 = r2 + r3;" + nl +
                "		r3 = Mem0[r1 + 0x00000004:word32];" + nl +
                "		if (r3 == 0x00000000)" + nl +
                "			break;" + nl +
                "		r1 = Mem0[r1 + 0x0000000C:word32];" + nl +
                "	}" + nl +
                "	return r2;" + nl +
                "}" + nl);

        }

        [Test]
        public void CaseJumpsBack()
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
                "	case 0:" + nl +
                "		print(n);" + nl +
                "		break;" + nl +
                "	case 1:" + nl +
                "		n = n + 0x00000001;" + nl +
                "		goto JumpBack;" + nl +
                "		break;" + nl +
                "	case 2:" + nl +
                "		print(n);" + nl +
                "		break;" + nl +
                "	}" + nl +
                "	return;" + nl +
                "}" + nl);
        }

        [Test]
        public void CaseStatement()
        {
            CompileTest(new MockCaseStatement());
            RunTest(
                "MockCaseStatement()" + nl +
                "{" + nl +
                "	switch (w)" + nl +
                "	{" + nl +
                "	case 0:" + nl +
                "		fn0();" + nl +
                "		break;" + nl +
                "	case 1:" + nl +
                "		fn1();" + nl +
                "		break;" + nl +
                "	case 2:" + nl +
                "		fn2();" + nl +
                "		break;" + nl +
                "	}" + nl +
                "\treturn;" + nl +
                "}" + nl);

        }

        [Test]
        public void NestedWhileLoops()
        {
            CompileTest(new MockNestedWhileLoops());
            RunTest(
                "MockNestedWhileLoops()" + nl +
                "{" + nl +
                "	int32 i = 0x00000000;" + nl +
                "	while (i < 10)" + nl +
                "	{" + nl +
                "		int32 j = 0x00000000;" + nl +
                "		while (j < 10)" + nl +
                "		{" + nl +
                "			Mem0[0x00001234:int32] = Mem0[0x00001234:int32] + j;" + nl +
                "			j = j + 1;" + nl +
                "		}" + nl +
                "		i = i + 1;" + nl +
                "	}" + nl +
                "	return;" + nl +
                "}" + nl);
        }

        [Test]
        public void WhileReturn()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Label("head");
                m.BranchIf(m.Local32("done"), "loop_done");
                m.Assign(m.Local32("done"), m.Fn("fn"));
                m.BranchIf(m.Local32("breakomatic"), "done");
                m.Assign(m.Local32("grux"), m.Fn("foo"));
                m.Jump("head");
                m.Label("loop_done");
                m.SideEffect(m.Fn("extra"));
                m.Label("done");
                m.Return();
            });
            RunTest(
                "ProcedureMock()" + nl +
                "{" + nl +
                "	while (!done)" + nl +
                "	{" + nl +
                "\t\tdone = fn();" + nl +
                "		if (breakomatic)" + nl +
                "\t\t\treturn;" + nl +
                "		grux = foo();" + nl +
                "\t}" + nl +
                "	return;" + nl +
                "}" + nl);
        }


        [Test]
        public void WhileWithDeclarations()
        {
            CompileTest(new MockWhileWithDeclarations());
            RunTest(
                "MockWhileWithDeclarations()" + nl +
                "{" + nl +
                "	byte v = Mem0[i:byte];" + nl +
                "	i = i + 0x00000001;" + nl +
                "	while (v != 0x20)" + nl +
                "	{" + nl +
                "		Mem0[0x00300000:byte] = v;" + nl +
                "		v = Mem0[i:byte];" + nl +
                "		i = i + 0x00000001;" + nl +
                "	}" + nl +
                "	return;" + nl +
                "}" + nl);
        }

        [Test]
        public void BranchesToReturns()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                Identifier a1 = m.Local16("a1"); 
                m.Assign(a1, m.Fn("fn0540"));
                Identifier tmp = m.Local16("tmp");
                m.Assign(tmp, m.LoadW(m.Word16(0x8416)));
                m.BranchIf(m.Ne(tmp, 0), "branch_c");

                m.Label("Branch_a");
                    m.Store(m.Word16(0x8414), m.Word16(0));
                    m.BranchIf(m.Eq(m.Word16(0x8414), 0x0000), "branch_c");

                m.Label("Branch_b");
                    Identifier ax_96 = m.Local16("ax_96");
                    m.SideEffect(m.Fn("fn02A9", m.AddrOf(ax_96)));
                    m.Return(ax_96);

                m.Label("branch_c");
	                m.Return(a1);
            });
            RunTest(
"ProcedureMock()" + nl +
"{" + nl +
"	a1 = fn0540();" + nl +
"	tmp = Mem0[0x8416:word16];" + nl +
"	if (tmp == 0x0000)" + nl +
"	{" + nl +
"		Mem0[0x8414:word16] = 0x0000;" + nl +
"		if (0x8414 != 0x0000)" + nl +
"		{" + nl +
"			fn02A9(&ax_96);" + nl +
"			return ax_96;" + nl +
"		}" + nl +
"		else" + nl +
"		{" + nl +
"branch_c:" + nl + 
"			return a1;" + nl +
"		}" + nl +
"	}" + nl +
"	else" + nl +
"		goto branch_c;" + nl +
"}" + nl);


        }

        [Test]
        public void InfiniteLoop()
        {
            CompileTest(new MockInfiniteLoop());
            RunTest(
                "MockInfiniteLoop()" + nl +
                "{" + nl +
                "	while (true)" + nl +
                "		DispatchEvents();" + nl +
                "}" + nl);             
        }

        private void CompileTest(Action<ProcedureMock> gen)
        {
            ProcedureMock mock = new ProcedureMock();
            gen(mock);
            CompileTest(mock);
        }

        private void CompileTest(ProcedureMock mock)
        {
            proc = mock.Procedure;
            proc.RenumberBlocks();
            StructureAnalysis sa = new StructureAnalysis(mock.Procedure);
            sa.BuildProcedureStructure();
            sa.FindStructures();
            curProc = sa.ProcedureStructure;
        }

        private void RunTest(string sExp)
        {
            try
            {
                AbsynCodeGenerator acg = new AbsynCodeGenerator();
                acg.GenerateCode(curProc, proc.Body);
                GenCode(proc, sb);
                Assert.AreEqual(sExp, sb.ToString());
            }
            catch
            {
                curProc.Dump();
                Console.WriteLine(sb.ToString());
                throw;
            }
        }

        private void GenCode(Procedure proc, StringWriter sb)
        {
            sb.WriteLine("{0}()", proc.Name);
            sb.WriteLine("{");

            CodeFormatter cf = new CodeFormatter(sb);
            cf.WriteStatementList(proc.Body);

            sb.WriteLine("}");
        }
    }
}
#if OLD

        [Test]
        public void IfThen()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.Local32("fnord"), "skip");
                m.Store(m.Word32(48), m.Word32(0));
                m.Label("skip");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tif (!fnord)" + nl +
                "\t\tMem0[0x00000030:word32] = 0x00000000;" + nl +
                "\treturn;" + nl +
                "}" + nl;

            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void IfThenElse()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.Local32("phnord"), "elsie");
                m.Assign(m.Local32("a"), m.Word32(0));
                m.Jump("done");
                m.Label("elsie");
                m.Assign(m.Local32("a"), m.Word32(1));
                m.Label("done");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tif (phnord)" + nl +
                "\t\ta = 0x00000001;" + nl +
                "\telse" + nl +
                "\t\ta = 0x00000000;" + nl +
                "\treturn;" + nl +
                "}" + nl;
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void While()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("head");
                m.BranchIf(m.Local32("done"), "done");
                    m.Label("loopBody");
                    m.Assign(m.Local32("done"), m.Fn("fn"));
                    m.Jump("head");
                m.Label("done");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\twhile (!done)" + nl +
                "\t\tdone = fn();" + nl +
                "\treturn;" + nl +
                "}" + nl;

            Assert.AreEqual(sExp, sb.ToString());
        }


        [Test]
        public void Switch()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Switch(m.Local16("grux"), "a1", "a2");
                m.Label("a1");
                m.SideEffect(m.Fn("fna1"));
                m.Jump("done");
                m.Label("a2");
                m.SideEffect(m.Fn("fna2"));
                m.Jump("done");
                m.Label("done");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tswitch (grux)" + nl +
                "\t{" + nl +
                "\tcase 0:" + nl +
                "\t\tfna1();" + nl +
                "\t\tbreak;" + nl +
                "\tcase 1:" + nl +
                "\t\tfna2();" + nl +
                "\t\tbreak;" + nl +
                "\t}" + nl +
                "\treturn;" + nl  +
                "}" + nl;
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        [Ignore("Problems with infinite loop post dominator computation")]
        public void Forever()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("lupe");
                m.SideEffect(m.Fn("foo"));
                m.Jump("lupe");
                m.Return();
            });

            string sExp =
                "@@@";
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void DoWhile()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("lupe");
                m.SideEffect(m.Fn("foo"));
                m.BranchIf(m.LocalBool("flag"), "lupe");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tdo" + nl +
                "\t\tfoo();" + nl +
                "\twhile (flag);" + nl +
                "\treturn;" + nl +
                "}" + nl;
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void EmitGoto()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.BranchIf(m.LocalBool("foo"), "TheElse");
                m.Assign(m.Local32("a"), m.Local32("b"));
                m.BranchIf(m.LocalBool("bar"), "CommonTail");
                m.Assign(m.Local32("c"), m.Local32("d"));
                m.Jump("done");
                m.Label("TheElse");
                m.Assign(m.Local32("d"), m.Local32("e"));
                m.Label("CommonTail");
                m.Assign(m.Local32("f"), m.Local32("g"));
                m.Label("done");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tif (foo)" + nl +
                "\t{" + nl +
                "\t\td = e;" + nl +
                "CommonTail:" + nl +
                "\t\tf = g;" + nl +
                "\t}" + nl +
                "\telse" + nl +
                "\t{" + nl +
                "\t\ta = b;" + nl +
                "\t\tif (bar)" + nl +
                "\t\t\tgoto CommonTail;" + nl +
                "\t\telse" + nl +
                "\t\t\tc = d;" + nl +
                "\t}" + nl +
                "\treturn;" + nl +
                "}" + nl;
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void WhileWhile()
        {
            // Nested while loops.
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("OuterLoop");
                m.BranchIf(m.LocalBool("done"), "Done");

                m.Label("InnerLoop");
                m.BranchIf(m.LocalBool("doneInner"), "DoneInner");

                m.Assign(m.Local32("b"), m.Add(m.Local32("b"), m.Int32(1)));
                m.Jump("InnerLoop");

                m.Label("DoneInner");
                m.Assign(m.Local32("a"), m.Add(m.Local32("a"), m.Int32(1)));
                m.Jump("OuterLoop");

                m.Label("Done");
                m.Return();
            });
            string sExp = 
                "ProcedureMock()" + nl + 
                "{" + nl + 
                "\twhile (!done)" + nl + 
                "\t{" + nl + 
                "\t\twhile (!doneInner)" + nl + 
                "\t\t\tb = b + 0x00000001;" + nl + 
                "\t\ta = a + 0x00000001;" + nl + 
                "\t}" + nl + 
                "\treturn;" + nl + 
                "}" + nl;

            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void DoWhileWithBranch()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("DoLoop");
                m.BranchIf(m.LocalBool("NoGo"), "NoGo");
                m.Label("Go");
                m.SideEffect(m.Fn("foo"));
                m.Label("NoGo");
                m.BranchIf(m.LocalBool("keepLooping"), "DoLoop");
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\tdo" + nl +
                "\t\tif (!NoGo)" + nl +
                "\t\t\tfoo();" + nl +
                "\twhile (keepLooping);" + nl +
                "\treturn;" + nl +
                "}" + nl;
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void WhileGoto()
        {
            RunTest(new MockWhileGoto());
            string sExp =
           "MockWhileGoto()" + nl +
           "{" + nl +
           "\twhile (foo())" + nl +
           "\t{" + nl +
           "\t\tbar();" + nl +
           "\t\tif (foo())" + nl +
           "\t\t{" + nl +
           "\t\t\textraordinary();" + nl +
           "\t\t\tgoto end;" + nl +
           "\t\t}" + nl +
           "\t\tbar2();" + nl +
           "\t}" + nl +
           "\tbar3();" + nl +
           "end:" + nl +
           "\tbar4();" + nl +
           "\treturn;" + nl +
           "}" + nl;


            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }

        private void RunTest(ProcGenerator gen)
        {
            ProcedureMock m = new ProcedureMock();
            gen(m);
            RunTest(m);
        }

        private void RunTest(ProcedureMock m)
        {
            m.Procedure.RenumberBlocks();

            StructureAnalysis sa = new StructureAnalysis(m.Procedure);
            sa.Structure();

            sb = new StringWriter();
            GenCode(m.Procedure, sb);
        }

        private void GenCode(Procedure proc, StringWriter sb)
        {
            sb.WriteLine("{0}()", proc.Name);
            sb.WriteLine("{");

            CodeFormatter cf = new CodeFormatter(sb);
            cf.WriteStatementList(proc.Body);

            sb.WriteLine("}");
        }

    }
}
#endif
