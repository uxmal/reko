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

using NUnit.Framework;
using System;
using StringBuilder = System.Text.StringBuilder;
using System.Collections;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class MapTests
	{
		public MapTests()
		{

		}

		public void RunTests()
		{
			CreationTest();
			AddOne();
			AddOneDeleteOne();
			AddThreeDeleteOne();
			AddMany();
			DeleteTest();
			CopyToTest();
		}

		[Test]
		public void CreationTest()
		{
			Map t = new Map();
			Assert.IsTrue(t.Count == 0);
			Assert.IsTrue("" == Dump(t));
		}

		[Test]
		public void AddOne()
		{
			Map t = new Map();
			t.Add(1, "one");
			Assert.IsTrue("one " == Dump(t));
			Assert.IsTrue(t.Count == 1);
		}

		[Test]
		public void AddOneDeleteOne()
		{
			Map t = new Map();
			t.Add(1, "one");
			t.Remove(1);
			Assert.IsTrue("" == Dump(t));
			Assert.IsTrue(t.Count == 0);
		}

		[Test]
		public void AddMany()
		{
			Map t = new Map();
			for (int i = 100; i >= 0; --i)
			{
				t.Add(i, i.ToString());
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 101; ++i)
			{
				sb.Append(i.ToString());
				sb.Append(" ");
			}

			Assert.IsTrue(sb.ToString() == Dump(t));
		}

		[Test]
		public void DeleteTest()
		{
			Map t = new Map();
			t.Add(0, "0");
			t.Add(10, "10");
			t.Add(-10, "-10");
			t.Add(-5, "-5");
			t.Add(-7, "-7");
			string s = Dump(t);

			t.Remove(10);			// 10 should be black, so this forces a rebalance.
		}

		[Test]
		public void AddThreeDeleteOne()
		{
			Map t = new Map();
			t.Add(1, "one");
			t.Add(2, "two");
			t.Add(3, "three");
			Assert.IsTrue(t.Count == 3);
			Assert.IsTrue("one two three " == Dump(t));
			t.Remove(2);
			Assert.IsTrue(t.Count == 2);
			Assert.IsTrue("one three " == Dump(t));
		}

		[Test]
		public void CopyToTest()
		{
			Map t = new Map();
			t.Add(3, 3);
			t.Add(2, 2);
			t.Add(4, 4);
			DictionaryEntry [] a = new DictionaryEntry[3];
			t.CopyTo(a, 0);
			Assert.IsTrue((int)a[0].Key == 2);
			Assert.IsTrue((int)a[1].Key == 3);
			Assert.IsTrue((int)a[2].Key == 4);
		}

		[Test]
		public void LowerBoundTest()
		{
			Map m = new Map();
			m.Add(100, 100);
			m.Add(0, 0);
			m.Add(70, 70);
			m.Add(150, 150);

			Assert.IsTrue((int) m.LowerBound(100) == 100);
			Assert.IsTrue((int) m.LowerBound(0) == 0);
			Assert.IsTrue((int) m.LowerBound(70) == 70);
			Assert.IsTrue((int) m.LowerBound(150) == 150);

			Assert.IsTrue(m.LowerBound(-1) == null);
			Assert.IsTrue((int) m.LowerBound(1) == 0);
			Assert.IsTrue((int) m.LowerBound(69) == 0);
			Assert.IsTrue((int) m.LowerBound(75) == 70);
			Assert.IsTrue((int) m.LowerBound(102) == 100);
			Assert.IsTrue((int) m.LowerBound(200) == 150);

		}

		private string Dump(Map t)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DictionaryEntry de in t)
			{
				sb.Append(de.Value.ToString());
				sb.Append(" ");
			}
			return sb.ToString();
		}
	}
}
