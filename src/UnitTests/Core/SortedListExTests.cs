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

using NUnit.Framework;
using Reko.Core.Lib;
using System;
using StringBuilder = System.Text.StringBuilder;
using System.Collections.Generic;

namespace Reko.UnitTests.Core
{
	[TestFixture]
	public class SortedListExTests
	{
		public SortedListExTests()
		{

		}

		[Test]
		public void CreationTest()
		{
			SortedList<int,string> t = new SortedList<int,string>();
			Assert.IsTrue(t.Count == 0);
			Assert.AreEqual("", Dump(t));
		}

		[Test]
		public void AddOne()
		{
			SortedList<int,string> t = new SortedList<int,string>();
			t.Add(1, "one");
			Assert.IsTrue("one " == Dump(t));
			Assert.AreEqual(1, t.Count);
		}

		[Test]
		public void AddOneDeleteOne()
		{
			SortedList<int,string> t = new SortedList<int,string>();
			t.Add(1, "one");
			t.Remove(1);
			Assert.AreEqual("" ,Dump(t));
			Assert.IsTrue(t.Count == 0);
		}

		[Test]
		public void AddMany()
		{
			SortedList<int,string> t = new SortedList<int,string>();
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
			SortedList<int,string> t = new SortedList<int,string>();
			t.Add(0, "0");
			t.Add(10, "10");
			t.Add(-10, "-10");
			t.Add(-5, "-5");
			t.Add(-7, "-7");

			t.Remove(10);			// 10 should be black, so this forces a rebalance.
		}

		[Test]
		public void AddThreeDeleteOne()
		{
			SortedList<int,string> t = new SortedList<int,string>();
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
			SortedList<int,string> t = new SortedList<int,string>();
			t.Add(3, "3");
			t.Add(2, "2");
			t.Add(4, "4");
			int [] a = new int[3];
			t.Keys.CopyTo(a, 0);
			Assert.AreEqual(2, a[0]);
            Assert.AreEqual(3, a[1]);
            Assert.AreEqual(4, a[2]);
		}

		[Test]
		public void LowerBoundTest()
		{
			SortedList<int,string> m = new SortedList<int,string>();
			m.Add(100, "100");
			m.Add(0, "0");
			m.Add(70, "70");
			m.Add(150, "150");

            string s;
			Assert.IsTrue(m.TryGetLowerBound(100, out s));
			Assert.AreEqual("100", s);
			Assert.IsTrue(m.TryGetLowerBound(0, out s));
			Assert.AreEqual("0", s);
			Assert.IsTrue(m.TryGetLowerBound(70, out s));
			Assert.AreEqual("70", s);
			Assert.IsTrue(m.TryGetLowerBound(150, out s));
			Assert.AreEqual("150", s);

			Assert.IsFalse(m.TryGetLowerBound(-1, out s));
			Assert.IsTrue(m.TryGetLowerBound(1, out s));
			Assert.AreEqual("0", s);
			Assert.IsTrue(m.TryGetLowerBound(69, out s));
			Assert.AreEqual("0", s);
			Assert.IsTrue(m.TryGetLowerBound(75, out s));
			Assert.AreEqual("70", s);
			Assert.IsTrue(m.TryGetLowerBound(102, out s));
			Assert.AreEqual("100", s);
			Assert.IsTrue(m.TryGetLowerBound(200, out s));
			Assert.AreEqual("150", s);
		}

        [Test]
        public void UpperBoundTest()
        {
            var m = new SortedList<int, string>();
            m.Add(250, "250");

            string s;
            Assert.IsTrue(m.TryGetUpperBound(249, out s));
            Assert.AreEqual("250", s);
        }

		private string Dump(SortedList<int,string> t)
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int,string> de in t)
			{
				sb.Append(de.Value);
				sb.Append(" ");
			}
			return sb.ToString();
		}
	}
}
