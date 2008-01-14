/* 
 * Copyright (C) 1999-2008 John Källén.
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
		private CodeFormatter cf;
		private StringWriter sw;
		private string nl = Environment.NewLine;

		[Test]
		public void CfMulAdd()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", 2, PrimitiveType.Word16, null);

			Expression e = new BinaryExpression(
				Operator.mul, PrimitiveType.Word16, new BinaryExpression(
				Operator.add, PrimitiveType.Word16, id1, id2), new Constant(PrimitiveType.Word16, 2));
			e.Accept(cf);

			Assert.AreEqual("(v1 + v2) * 0x0002", sw.ToString());
		}

		[Test]
		public void CfAddMul()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word16, null);
			Identifier id2 = new Identifier("v2", 2, PrimitiveType.Word16, null);

			Expression e = new BinaryExpression(
				Operator.add, PrimitiveType.Word16, new BinaryExpression(
				Operator.mul, PrimitiveType.Word16, id1, id2), new Constant(PrimitiveType.Word16, 2));
			e.Accept(cf);

			Assert.AreEqual("v1 * v2 + 0x0002", sw.ToString());
		}

		[Test]
		public void CfFieldAccessDeref()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word32, null);
			Expression e = new FieldAccess(null, new Dereference(null, id1),"foo");
			e.Accept(cf);

			Assert.AreEqual("v1->foo", sw.ToString());
		}

		[Test]
		public void CfDerefFieldAccess()
		{
			Identifier id1 = new Identifier("v1", 1, PrimitiveType.Word32, null);
			Expression e = new Dereference(null, new FieldAccess(null, id1, "foo"));
			e.Accept(cf);

			Assert.AreEqual("*v1.foo", sw.ToString());
		}

		[Test]
		public void CfIndentation()
		{
			cf.UseTabs = true;
			cf.TabSize = 4;
			cf.Indentation = 6;
			Instruction b = new Branch(Constant.Word32(0));
			b.Accept(cf);
			Assert.AreEqual("\t  branch 0x00000000" + nl, sw.ToString());
		}

		[Test]
		public void CfIndentWithSpaces()
		{
			cf.UseTabs = false;
			cf.TabSize = 4;
			cf.Indentation = 6;
			Instruction b = new Branch(Constant.Word32(0));
			b.Accept(cf);
			Assert.AreEqual("      branch 0x00000000" + nl, sw.ToString());
		}

		[SetUp]
		public void Setup()
		{
			sw = new StringWriter();
			cf = new CodeFormatter(sw);
		}

	}
}
