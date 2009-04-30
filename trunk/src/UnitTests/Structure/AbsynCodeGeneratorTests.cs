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
using Decompiler.Core.Code;
using Decompiler.Core.Output;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class AbsynCodeGeneratorTests
    {
        private string nl;

        [SetUp]
        public void Setup()
        {
            nl = Environment.NewLine;
        }

        [Test]
        public void GenerateIf()
        {
            ProcedureStructure h = new ProcedureStructure(new CmpMock().Procedure);
            StructureAnalysis g = new StructureAnalysis(h);
            g.Structure();
            AbsynCodeGenerator gen = new AbsynCodeGenerator();
            List<AbsynStatement> stms = gen.GenerateCode(h.EntryNode);
            string s = DumpCode(stms);
            Console.WriteLine(s);
            string sExp =
                "\tZ = cond(r0 - 0x00000000);" + nl +
                "\tif (Test(EQ,Z))" + nl +
                "\t\tr0 = r0 + r1;" + nl +
                "\tMem0[0x10003000:word32] = r0;" + nl +
                "\treturn;" + nl;
            Assert.AreEqual(sExp, s);
        }

        [Test]
        public void Switch()
        {
            string s = RunTest(new MockSwitch().Procedure);
            string sExp =
                "\tif (r1 >u 0x00000004)" + nl +
                "\t\tr2 = r1 *s r1;" + nl +
                "\telse" + nl +
                "\t\tswitch (r1)" + nl +
                "\t\t{" + nl +
                "\t\tcase 0:" + nl +
                "\t\t\tr2 = 0x00000000;" + nl +
                "\t\t\tbreak;" + nl +
                "\t\tcase 1:" + nl +
                "\t\t\tr2 = 0x00000001;" + nl +
                "\t\t\tbreak;" + nl +
                "\t\tcase 2:" + nl +
                "\t\t\tr2 = 0x00000004;" + nl +
                "\t\t\tbreak;" + nl +
                "\t\tcase 3:" + nl +
                "\t\t\tr2 = 0x00000009;" + nl +
                "\t\t\tbreak;" + nl +
                "\t\t}" + nl +
                "\treturn r2;" + nl;

            Assert.AreEqual(sExp, s);
        }

        [Test]
        public void UnstructuredIfs()
        {
            string s = RunTest(new UnstructuredIfsMock().Procedure);
            string sExp =
                "\tif (foo)" + nl +
                "\t\tif (bar)" + nl +
                "\t\t{" + nl +
                "\t\tinside:" + nl +
                "\t\t\tniz();" + nl +
                "\t\t}" + nl +
                "\t\telse" + nl +
                "\t\t\tbaz();" + nl +
                "\telse" + nl +
                "\t{" + nl +
                "\t\tquux();" + nl +
                "\t\tgoto inside:" + nl +
                "\t}" + nl +
                "\treturn;" + nl;
            Console.WriteLine(s);
            Assert.AreEqual(sExp, s);
        }

        private string RunTest(Procedure proc)
        {
            ProcedureStructure h = new ProcedureStructure(proc);
            StructureAnalysis g = new StructureAnalysis(h);
            g.Structure();
            AbsynCodeGenerator gen = new AbsynCodeGenerator();
            List<AbsynStatement> stms = gen.GenerateCode(h.EntryNode);
            return DumpCode(stms);
        }

        private string DumpCode(List<AbsynStatement> stms)
        {
            StringWriter sb = new StringWriter();
            CodeFormatter writer = new CodeFormatter(sb);
            writer.WriteStatementList(stms);
            return sb.ToString();
        }

    }
}
