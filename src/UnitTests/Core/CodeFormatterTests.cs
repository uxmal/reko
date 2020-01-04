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
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class CodeFormatterTests
	{
        private TextFormatter formatter;
		private CodeFormatter cf;
		private StringWriter sw;
        private ExpressionEmitter m;
		private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            sw = new StringWriter();
            formatter = new TextFormatter(sw);
            cf = new CodeFormatter(formatter);
            m = new ExpressionEmitter();
        }

		[Test]
		public void CfMulAdd()
		{
			Identifier id1 = new Identifier("v1", PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", PrimitiveType.Word16, null);

			var e = m.IMul(m.IAdd(id1, id2), Constant.Word16(2));
			e.Accept(cf);

			Assert.AreEqual("(v1 + v2) * 0x0002", sw.ToString());
		}

		[Test]
		public void CfAddMul()
		{
			Identifier id1 = new Identifier("v1", PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", PrimitiveType.Word16, null);

			var e = m.IAdd(m.IMul(id1, id2), Constant.Word16(2));
			e.Accept(cf);

			Assert.AreEqual("v1 * v2 + 0x0002", sw.ToString());
		}

		[Test]
		public void CfFieldAccessDeref()
		{
			Identifier id1 = new Identifier("v1", PrimitiveType.Word32, null);
            Expression e = new FieldAccess(
                PrimitiveType.Ptr32,
                m.Deref(id1),
                new StructureField(4, PrimitiveType.Word32, "foo"));
			e.Accept(cf);

			Assert.AreEqual("v1->foo", sw.ToString());
		}

		[Test]
		public void CfDerefFieldAccess()
		{
			Identifier id1 = new Identifier("v1", PrimitiveType.Word32, null);
			Expression e = m.Deref(new FieldAccess(
                PrimitiveType.Word32, 
                id1,
                new StructureField(4, PrimitiveType.Word32, "foo")));
			e.Accept(cf);

			Assert.AreEqual("*v1.foo", sw.ToString());
		}

		[Test]
		public void CfIndentation()
		{
			formatter.UseTabs = true;
            formatter.TabSize = 4;
            formatter.Indentation = 6;
            var b = new Branch(Constant.Word32(0), new Block(null, "target"));
			b.Accept(cf);
			Assert.AreEqual("\t  branch 0x00000000 target" + nl, sw.ToString());
		}

		[Test]
		public void CfIndentWithSpaces()
		{
            formatter.UseTabs = false;
            formatter.TabSize = 4;
            formatter.Indentation = 6;
            Instruction b = new Branch(Constant.Word32(0), new Block(null, "Test"));
			b.Accept(cf);
			Assert.AreEqual("      branch 0x00000000 Test" + nl, sw.ToString());
		}

		[Test]
		public void CfMemberPointerSelector()
		{
			Identifier ds = new Identifier("ds", PrimitiveType.SegmentSelector, null);
			Identifier bx = new Identifier("bx", PrimitiveType.Word16, null);
			Expression e = new MemberPointerSelector(PrimitiveType.Byte, ds, bx);
			e.Accept(cf);
			Assert.AreEqual("ds.*bx", sw.ToString());
		}

        [Test]
        public void CfScopeResolution()
        {
            var eq = new EquivalenceClass(new TypeVariable("Eq_2", 2));
            var sr = new ScopeResolution(eq);
            var e = new FieldAccess(PrimitiveType.Int32, sr, new StructureField(4, PrimitiveType.Int32, "i0004"));
            e.Accept(cf);
            Assert.AreEqual("Eq_2::i0004", sw.ToString());
        }

        [Test]
        public void CfNullPointer()
        {
            var e = Address.Ptr32(0);
            e.Accept(cf);
            Assert.AreEqual("null", sw.ToString());
        }

        private Identifier Id(string id)
        {
            return new Identifier(id, PrimitiveType.Word32, new TemporaryStorage(id, -1, PrimitiveType.Word32));
        }

        [Test]
        public void CfDoWhile_SmallBody()
        {
            var dw = new AbsynDoWhile(new List<AbsynStatement>
                {
                    new AbsynAssignment(Id("foo"), m.Int32(3))
                },
                m.Lt(Id("bar"), 0));
            dw.Accept(cf);
            var sExp =
                "\tdo" + nl +
                "\t\tfoo = 3;" + nl +
                "\twhile (bar < 0x00000000);" + nl;
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void CfDoWhile_LargeBody()
        {
            var dw = new AbsynDoWhile(new List<AbsynStatement>
                {
                    new AbsynAssignment(Id("foo"), m.Int32(3)),
                    new AbsynAssignment(Id("foo"), m.Int32(4))
                },
                m.Lt(Id("bar"), 0));
            dw.Accept(cf);
            var sExp =
                "\tdo" + nl +
                "\t{" + nl +
                    "\t\tfoo = 3;" + nl + 
                    "\t\tfoo = 4;" + nl + 
                "\t} while (bar < 0x00000000);" + nl;
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void CfSegmentedAccess()
        {
            var es = new Identifier("es", PrimitiveType.SegmentSelector, RegisterStorage.None);
            var ds = new Identifier("ds", PrimitiveType.SegmentSelector, RegisterStorage.None);
            var bx = new Identifier("bx", PrimitiveType.SegmentSelector, RegisterStorage.None);
            var e =  new MemberPointerSelector(
                PrimitiveType.Word16,
                m.Deref(es),
                new MemberPointerSelector(
                    PrimitiveType.Word16,
                    m.Deref(ds),
                    bx));
            e.Accept(cf);
            Assert.AreEqual("es->*(ds->*bx)", sw.ToString());
        }

        [Test]
        public void CfAssocSub()
        {
            var a = new Identifier("a", PrimitiveType.Int32, RegisterStorage.None);
            var b = new Identifier("b", PrimitiveType.Int32, RegisterStorage.None);
            var c = new Identifier("c", PrimitiveType.Int32, RegisterStorage.None);
            var e = m.ISub(a, m.ISub(b, c));
            e.Accept(cf);
            Assert.AreEqual("a - (b - c)", sw.ToString());
        }

        [Test]
        public void CfMpsAccess()
        {
            var a = new Identifier("a", PrimitiveType.Int32, RegisterStorage.None);
            var b = new Identifier("b", PrimitiveType.Int32, RegisterStorage.None);
            var c = new Identifier("c", PrimitiveType.Int32, RegisterStorage.None);
            var e = m.Array(
                PrimitiveType.Byte,
                m.Field(PrimitiveType.Byte, m.MembPtrW(a, b), new StructureField(4, PrimitiveType.Ptr32, "a0004")),
                c);
            e.Accept(cf);
            Assert.AreEqual("(a->*b).a0004[c]", sw.ToString());
        }

        [Test]
        public void CfFloatWithDecimals()
        {
            var c = Constant.Real32(3.1F);
            c.Accept(cf);
            Assert.AreEqual("3.1F", sw.ToString());
        }

        [Test]
        public void CfDoubleWithDecimals()
        {
            var c = Constant.Real64(3.1);
            c.Accept(cf);
            Assert.AreEqual("3.1", sw.ToString());
        }

        [Test]
        public void CfFloatWithoutDecimals()
        {
            var c = Constant.Real32(3);
            c.Accept(cf);
            Assert.AreEqual("3.0F", sw.ToString());
        }

        [Test]
        public void CfDoubleWithoutDecimals()
        {
            var c = Constant.Real64(3);
            c.Accept(cf);
            Assert.AreEqual("3.0", sw.ToString());
        }

        [Test]
        public void CfFloatWithExponent()
        {
            var c = Constant.Real32(1e12F);
            c.Accept(cf);
            Assert.AreEqual("1e+12F", sw.ToString());
        }

        [Test]
        public void CfDoubleWithExponent()
        {
            var c = Constant.Real64(1e18);
            c.Accept(cf);
            Assert.AreEqual("1e+18", sw.ToString());
        }

        [Test]
        public void CfStructCast()
        {
            var s = new StructureType
            {
                Name = "foo",
                Fields =
                {
                    { 0x0000, PrimitiveType.Int16 },
                    { 0x0002, PrimitiveType.Int16 }
                }
            };
            var id = new Identifier("id", PrimitiveType.Word32, new TemporaryStorage("id", 4, PrimitiveType.Word32));
            var cast = new Cast(s, id);
            cast.Accept(cf);
            Assert.AreEqual("(struct foo) id", sw.ToString());
        }

        [Test]
        public void CfStringConstant_Escape()
        {
            var s = Constant.String("\a\b\f\n\r\t\v\'\"\\", StringType.NullTerminated(PrimitiveType.Char));
            s.Accept(cf);
            var q = sw.ToString();
            Assert.AreEqual("\"\\a\\b\\f\\n\\r\\t\\v'\\\"\\\\\"", sw.ToString());
        }

        [Test]
        public void CfStringConstant_Escape_Numeric()
        {
            var s = Constant.String("\x00\x1F\x20\x21\x7E\x7F\x80", StringType.NullTerminated(PrimitiveType.Char));
            s.Accept(cf);
            Assert.AreEqual("\"\\0\\x1F !~\\x7F\\x80\"", sw.ToString());
        }

        [Test]
        public void CfComment_Multiline()
        {
            formatter.Indentation = 1;
            var cmt = new CodeComment(
@"First line
Second line");
            cmt.Accept(cf);
            var expected =
@" // First line
 // Second line
";
            Assert.AreEqual(expected, sw.ToString());
        }

        [Test]
        public void CfAbsynComment_Multiline()
        {
            formatter.Indentation = 1;
            var cmt = new AbsynLineComment(
@"First abstract syntax comment line
Second abstract syntax comment line");
            cmt.Accept(cf);
            var expected =
@" // First abstract syntax comment line
 // Second abstract syntax comment line
";
            Assert.AreEqual(expected, sw.ToString());
        }
    }
}
