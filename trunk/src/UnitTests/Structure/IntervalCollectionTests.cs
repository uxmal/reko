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
using Decompiler.Structure;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class IntervalCollectionTests
	{
		[Test]
		public void Create()
		{
			IntervalCollection info = new IntervalCollection(0);
		}

		[Test]
		public void AddInterval()
		{
			IntervalCollection info = new IntervalCollection(1);
			Block head = new Block(null, "foo");
			head.RpoNumber = 0;
			Interval i = new Interval(head, 2);

			info.Add(i);
			Assert.AreEqual(1, info.Count);
		}

		[Test]
		public void AddTwoIntervals()
		{
			IntervalCollection ints = new IntervalCollection(2);
			Block b1 = new Block(null, "b1");
			b1.RpoNumber = 0;
			Interval i1 = new Interval(b1, 2);
			Block b2 = new Block(null, "b2");
			b2.RpoNumber = 1;
			Interval i2 = new Interval(b2, 2);

			ints.Add(i2);
			ints.Add(i1);
			Assert.AreEqual(2, ints.Count);
			IEnumerator e = ints.GetEnumerator();
			Assert.IsTrue(e.MoveNext(), "Should have at least one item");
			Assert.AreEqual(i1, e.Current, "i1 should be first");
			Assert.IsTrue(e.MoveNext(), "Should have two items");
			Assert.AreEqual(i2, e.Current, "i2 should be second");
			Assert.IsFalse(e.MoveNext(), "Only two items in collections");
	}
}
}
