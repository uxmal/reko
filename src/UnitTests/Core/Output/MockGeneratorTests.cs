#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Output;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Reko.UnitTests.Core.Output
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
            CompileTest(m =>
            {
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl + 
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
            CompileTest(m =>
            {
                var id = m.Local(PrimitiveType.Word32, "id");
                m.Assign(id, 42);
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
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
            CompileTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");
                m.Assign(a, m.SMul(m.UMul(a, b), m.IAdd(a, m.ISub(b, m.IMul(a, b)))));
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(a, SMul(UMul(a, b), IAdd(a, ISub(b, IMul(a, b)))));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void LoadStore()
        {
            CompileTest(m =>
            {
                m.Store(m.Word32(0x123456), m.Load(PrimitiveType.Byte, m.Word32(0x12348)));
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Store(Word32(0x123456), Load(PrimitiveType.Byte, Word32(0x12348)));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp); 
        }

        [Test]
        public void Declaration()
        {
            CompileTest(m =>
            {
                Identifier id = m.Local16("id");
                m.Declare(id, m.Word16(0x1234));
                m.Return();
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier id = Local(PrimitiveType.Word16, \"id\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    Declare(id, Word16(0x1234));" + nl + 
"    Return();" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Application()
        {
            CompileTest(m =>
            {
                Identifier inp = m.Local32("inp");
                Identifier outp = m.Local32("outp");
                m.Assign(inp, 0);
                m.Assign(inp, m.Fn("foo", inp, m.AddrOf(outp)));
                m.Return();
            });
            string sExp =
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier inp = Local(PrimitiveType.Word32, \"inp\");" + nl + 
"    Identifier outp = Local(PrimitiveType.Word32, \"outp\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    Assign(inp, Word32(0x0));" + nl + 
"    Assign(inp, Fn(\"foo\", inp, AddrOf(outp)));" + nl + 
"    Return();" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void SelectorSideEffect()
        {
            CompileTest(m =>
            {
                Identifier es = m.Local(PrimitiveType.SegmentSelector, "es");
                m.SideEffect(m.Fn("foo", es));
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier es = Local(PrimitiveType.SegmentSelector, \"es\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    SideEffect(Fn(\"foo\", es));" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);

        }

        [Test]
        public void Branch()
        {
            CompileTest(m =>
            {
                Identifier i = m.Local(PrimitiveType.Int32, "i");

                m.BranchIf(m.Eq(i, 0), "skip");
                m.Label("fade");
                m.Store(m.Word32(0x123456), i);
                m.Label("skip");
                m.Return(i);
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier i = Local(PrimitiveType.Int32, \"i\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    BranchIf(Eq(i, new Constant(Primitive.Int32, 0x0)), \"skip\");" + nl + 
"    " + nl + 
"    Label(\"fade\");" + nl + 
"    Store(Word32(0x123456), i);" + nl + 
"    " + nl + 
"    Label(\"skip\");" + nl + 
"    Return(i);" + nl + 
"}" + nl + 
"" + nl;

            VerifyTest(sExp);
        }

        [Test]
        public void ComparisonOperators()
        {
            CompileTest(m =>
            {
                Identifier f = m.Local(PrimitiveType.Bool, "f");
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(f, m.Le(a, b)); 
                m.Assign(f, m.Ge(a, b)); 
                m.Assign(f, m.Uge(a, b)); 
                m.Assign(f, m.Ule(a, b)); 
                m.Assign(f, m.Lt(a, b)); 
                m.Assign(f, m.Gt(a, b)); 
                m.Assign(f, m.Ugt(a, b)); 
                m.Assign(f, m.Ult(a, b)); 
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    Identifier f = Local(PrimitiveType.Bool, \"f\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(f, Le(a, b));" + nl +
                "    Assign(f, Ge(a, b));" + nl +
                "    Assign(f, Uge(a, b));" + nl +
                "    Assign(f, Ule(a, b));" + nl +
                "    Assign(f, Lt(a, b));" + nl +
                "    Assign(f, Gt(a, b));" + nl +
                "    Assign(f, Ugt(a, b));" + nl +
                "    Assign(f, Ult(a, b));" + nl +
                "}" + nl +
                "" + nl);


        }

        [Test]
        public void LogicalOperators()
        {
            CompileTest(m =>
            {
                Identifier f = m.Local(PrimitiveType.Bool, "f");
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(f, m.Cand(a, m.Cor(a, b)));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    Identifier f = Local(PrimitiveType.Bool, \"f\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(f, Cand(a, Cor(a, b)));" + nl +
                "}" + nl +
                "" + nl);

        }

        [Test]
        public void BitwiseOperators()
        {
            CompileTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");
                Identifier c = m.Local(PrimitiveType.Word32, "c");

                m.Assign(c, m.Xor(a, m.Or(a, m.And(a, b))));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    Identifier c = Local(PrimitiveType.Word32, \"c\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(c, Xor(a, Or(a, And(a, b))));" + nl +
                "}" + nl +
                "" + nl);

        }

        [Test]
        public void SliceDpb()
        {
            CompileTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(a, m.Dpb(a, m.Slice(PrimitiveType.Word16, b, 16), 0));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = Local(PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = Local(PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(a, Dpb(a, Slice(PrimitiveType.Word16, b, 16), 0));" + nl +
                "}" + nl +
                "" + nl);

        }

        private void CompileTest(Action<ProcedureBuilder> buildMock)
        {
            ProcedureBuilder m = new ProcedureBuilder();
            buildMock(m);
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
