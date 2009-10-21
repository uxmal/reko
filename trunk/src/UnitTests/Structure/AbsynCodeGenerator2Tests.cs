using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class AbsynCodeGenerator2Tests
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


        private void CompileTest(ProcGenerator gen)
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
            AbsynCodeGenerator2 acg = new AbsynCodeGenerator2();
            acg.GenerateCode(curProc, proc.Body);
            GenCode(proc, sb);
            if (sExp != sb.ToString())
            {
                curProc.Dump();
                Console.WriteLine(sb.ToString());
                Assert.AreEqual(sExp, sb.ToString());
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
