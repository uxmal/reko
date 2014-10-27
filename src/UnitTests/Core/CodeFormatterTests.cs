#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Output;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class CodeFormatterTests
	{
        private TextFormatter formatter;
		private CodeFormatter cf;
		private StringWriter sw;
		private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            sw = new StringWriter();
            formatter = new TextFormatter(sw);
            cf = new CodeFormatter(formatter);
        }

		[Test]
		public void CfMulAdd()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", 2, PrimitiveType.Word16, null);

			Expression e = new BinaryExpression(
				Operator.IMul, PrimitiveType.Word16, new BinaryExpression(
				Operator.IAdd, PrimitiveType.Word16, id1, id2), Constant.Word16(2));
			e.Accept(cf);

			Assert.AreEqual("(v1 + v2) * 0x0002", sw.ToString());
		}

		[Test]
		public void CfAddMul()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", 2, PrimitiveType.Word16, null);

			Expression e = new BinaryExpression(
				Operator.IAdd, PrimitiveType.Word16, new BinaryExpression(
				Operator.IMul, PrimitiveType.Word16, id1, id2), Constant.Word16(2));
			e.Accept(cf);

			Assert.AreEqual("v1 * v2 + 0x0002", sw.ToString());
		}

		[Test]
		public void CfFieldAccessDeref()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word32, null);
            Expression e = new FieldAccess(PrimitiveType.Pointer32, new Dereference(PrimitiveType.Word32, id1), "foo");
			e.Accept(cf);

			Assert.AreEqual("v1->foo", sw.ToString());
		}

		[Test]
		public void CfDerefFieldAccess()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word32, null);
			Expression e = new Dereference(PrimitiveType.Pointer32, new FieldAccess(PrimitiveType.Word32, id1, "foo"));
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
			Identifier ds = new Identifier("ds", 1, PrimitiveType.SegmentSelector, null);
			Identifier bx = new Identifier("bx", 1, PrimitiveType.Word16, null);
			Expression e = new MemberPointerSelector(PrimitiveType.Byte, ds, bx);
			e.Accept(cf);
			Assert.AreEqual("ds.*bx", sw.ToString());
		}

        [Test]
        public void CfScopeResolution()
        {
            var eq = new EquivalenceClass(new TypeVariable("Eq_2", 2));
            var sr = new ScopeResolution(eq);
            var e = new FieldAccess(PrimitiveType.Int32, sr, "i0004");
            e.Accept(cf);
            Assert.AreEqual("Eq_2::i0004", sw.ToString());
        }

        [Test]
        public void CfNullPointer()
        {
            var e = new Address(0);
            e.Accept(cf);
            Assert.AreEqual("null", sw.ToString());
        }
	}
}
