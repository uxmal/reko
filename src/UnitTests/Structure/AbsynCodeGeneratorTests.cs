/* 
 * Copyright (C) 1999-2009 John Källén.
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
using Decompiler.Core.Absyn;
using Decompiler.Core.Output;
using Decompiler.Core.Types;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class AbsynCodeGeneratorTests
    {
        private StringWriter sb;
        private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Return();
            });
            string sExp =
                "ProcedureMock()" + nl +
                "{" + nl +
                "\treturn;" + nl +
                "}" + nl;

            Assert.AreEqual(sExp, sb.ToString());
        }

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
        public void WhileReturn()
        {
            RunTest(delegate(ProcedureMock m)
            {
                m.Label("head");
                m.BranchIf(m.Local32("done"), "done");
                m.Assign(m.Local32("done"), m.Fn("fn"));
                m.BranchIf(m.Local32("breakomatic"), "done");
                m.Assign(m.Local32("grux"), m.Fn("foo"));
                m.Jump("head");
                m.Label("done");
                m.Return();
            });
            Console.WriteLine(sb.ToString());
            string sExp =
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

        private void RunTest(ProcGenerator gen)
        {
            ProcedureMock m = new ProcedureMock();
            gen(m);
            m.Procedure.RenumberBlocks();

            StructureAnalysis sa = new StructureAnalysis(m.Procedure);
            sa.Structure();

            sb = new StringWriter();
            GenCode(sa.Procedure, sb);
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
