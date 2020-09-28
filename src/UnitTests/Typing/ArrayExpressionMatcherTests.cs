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
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class ArrayExpressionMatcherTests
	{
		private ProcedureBuilder m;
		private ArrayExpressionMatcher aem;
		private Identifier i;
		private Constant c;
		private Constant off;
		private Identifier r;

		[Test]
		public void Pattern1()
		{
			Expression e = m.SMul(i, c);
			Assert.IsTrue(aem.Match(e));
			Assert.AreSame(c, aem.ElementSize);
		}

		[Test]
		public void Pattern2()
		{
			Expression e = m.IAdd(m.SMul(c, i), c);
			Assert.IsTrue(aem.Match(e));
			Assert.AreSame(c, aem.ArrayPointer);
		}

		[Test]
		public void Pattern3()
		{
			Expression e = m.IAdd(r, m.IAdd(m.SMul(c, i), off));
			Assert.IsTrue(aem.Match(e));
			Assert.AreEqual("r + 42", aem.ArrayPointer.ToString());
			Assert.AreEqual("i", aem.Index.ToString());
            Assert.AreEqual(0x10, aem.ElementSize.ToInt32());
		}

		[Test]
		public void Pattern4()
		{
			Expression e = m.IAdd(m.IAdd(r, off), m.SMul(i, c));
			Assert.IsTrue(aem.Match(e));
			Assert.AreEqual("r + 42", aem.ArrayPointer.ToString());
			Assert.AreEqual("i", aem.Index.ToString());
            Assert.AreEqual(0x10, aem.ElementSize.ToInt32());
        }

		[SetUp]
		public void Setup()
		{
			m = new ProcedureBuilder();
			i = m.Local32("i");
			c = m.Int32(16);
			off = m.Int32(42);
			r = m.Local32("r");
			aem = new ArrayExpressionMatcher(PrimitiveType.Ptr32);
		}
	}
}
