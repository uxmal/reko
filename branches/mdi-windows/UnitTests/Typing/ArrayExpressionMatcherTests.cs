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
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class ArrayExpressionMatcherTests
	{
		private ProcedureMock m;
		private ArrayExpressionMatcher aem;
		private Identifier i;
		private Constant c;
		private Constant off;
		private Identifier r;

		[Test]
		public void Pattern1()
		{
			Expression e = m.Muls(i, c);
			Assert.IsTrue(aem.Match(e));
			Assert.AreSame(c, aem.ElementSize);
		}

		[Test]
		public void Pattern2()
		{
			Expression e = m.Add(m.Muls(c, i), c);
			Assert.IsTrue(aem.Match(e));
			Assert.AreSame(c, aem.ArrayPointer);
		}

		[Test]
		public void Pattern3()
		{
			Expression e = m.Add(r, m.Add(m.Muls(c, i), off));
			Assert.IsTrue(aem.Match(e));
			Assert.AreEqual("r + 0x0000002A", aem.ArrayPointer.ToString());
			Assert.AreEqual("0x00000010 *s i + r + 0x0000002A", e.ToString());
		}

		[Test]
		public void Pattern4()
		{
			Expression e = m.Add(m.Add(r, off), m.Muls(i, c));
			Assert.IsTrue(aem.Match(e));
			Assert.AreEqual("r + 0x0000002A", aem.ArrayPointer.ToString());
			Assert.AreEqual("r + 0x0000002A + i *s 0x00000010", e.ToString());
		}

		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
			i = m.Local32("i");
			c = m.Int32(16);
			off = m.Int32(42);
			r = m.Local32("r");
			aem = new ArrayExpressionMatcher(PrimitiveType.Pointer32);
		}
	}
}
