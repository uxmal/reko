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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Core.Output;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Core.Output
{
    [TestFixture]
    public class MockGeneratorTests
    {
        private StringWriter sb;
        private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
        }

        [Test]
        public void EmptyFunction()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Return();
            });
            string sExp =
                "public class MockProcedure : ProcedureMock" + nl + 
                "{" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl + 
                "    Return();" + nl + 
                "}" + nl + 
                "" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Assign()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                Identifier id = m.Local(PrimitiveType.Word32, "id");
                m.Assign(id, 42);
                m.Return();
            });
            string sExp =
                "public class MockProcedure : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier id = Local(PrimitiveType.Word32, \"id\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(id, Word32(0x2A));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp);

        }

        [Test]
        public void AddSubMul()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");
                m.Assign(a, m.Muls(m.Mulu(a, b), m.Add(a, m.Sub(b, m.Mul(a, b)))));
                m.Return();
            });
            string sExp =
                "public class MockProcedure : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(a, Muls(Mulu(a, b), Add(a, Sub(b, Mul(a, b)))));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void LoadStore()
        {
            CompileTest(delegate(ProcedureMock m)
            {
                m.Store(m.Word32(0x123456), m.Load(PrimitiveType.Byte, m.Word32(0x12348)));
                m.Return();
            });
            string sExp =
                "public class MockProcedure : ProcedureMock" + nl +
                "{" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Store(Word32(0x123456), Load(PrimitiveType.Byte, Word32(0x12348)));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp); 
        }

        private void CompileTest(Action<ProcedureMock> buildMock)
        {
            ProcedureMock m = new ProcedureMock();
            buildMock(m);
            m.Procedure.RenumberBlocks();
            MockGenerator g = new MockGenerator(sb);
            g.Write(m.Procedure);
        }

        private void VerifyTest(string sExp)
        {
            string s = sb.ToString();
            try
            {
                Assert.AreEqual(sExp, s);
            }
            catch
            {
                Console.WriteLine(s);
                throw;
            }
        }
    }
}
