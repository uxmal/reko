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

using Decompiler;
using Decompiler.Core;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ImageMapTests
	{
		private Address addrBase = new Address(0x8000, 0);
		private byte [] img = new Byte [] { 0x00, 0x00, 0x00, 0x00 };
		private int cItemsSplit;

		public ImageMapTests()
		{
		}

		[Test]
		public void ImageMapCreation()
		{
			ImageMap im = new ImageMap(addrBase, img.Length);

			im.AddSegment(new Address(0x8000, 2), "",  AccessMode.ReadWrite);
			im.AddSegment(new Address(0x8000, 3), "", AccessMode.ReadWrite);
			im.AddSegment(new Address(0x8000, 0), "", AccessMode.ReadWrite);

			// Verify

			IDictionaryEnumerator e = (IDictionaryEnumerator) im.Segments.GetEnumerator();
			Assert.IsTrue(e.MoveNext());
			ImageMapSegment seg = (ImageMapSegment) e.Value;
			Assert.AreEqual(2, seg.Size);

			Assert.IsTrue(e.MoveNext());
			seg = (ImageMapSegment) e.Value;
			Assert.AreEqual(1, seg.Size);
			
			Assert.IsTrue(e.MoveNext());
			seg = (ImageMapSegment) e.Value;
			Assert.AreEqual(1, seg.Size);

			Assert.IsTrue(!e.MoveNext());
		}

		[Test]
		public void ImageMapOverlaps()
		{
			ImageMap im = new ImageMap(new Address(0x8000, 0), 40);
			im.AddSegment(new Address(0x8000, 10), "", AccessMode.ReadWrite);
		}

		private ImageMapItem GetNextMapItem(IDictionaryEnumerator e)
		{
			Assert.IsTrue(e.MoveNext());
			return (ImageMapItem) e.Value;
		}

		private ImageMapSegment GetNextMapSegment(IDictionaryEnumerator e)
		{
			Assert.IsTrue(e.MoveNext());
			return (ImageMapSegment) e.Value;
		}

		[Test]
		public void ImageItems()
		{
			ImageMap im = new ImageMap(new Address(0xC00, 0), 0x4000);
			cItemsSplit = 0;
			im.ItemSplit += new ItemSplitHandler(ImageItems_ItemSplit);

			im.AddItem(new Address(0xC00, 30), new ImageMapItem());
			im.AddItem(new Address(0xC00, 30), new ImageMapItem());
			im.AddItem(new Address(0xC00, 0xF00), new ImageMapItem());

			// Now add a segment, which should stir things up.

			im.AddSegment(new Address(0xD00, 0), "0D00", AccessMode.ReadWrite);

			Assert.IsTrue(cItemsSplit == 3);

			IDictionaryEnumerator e = im.Items.GetEnumerator();
			ImageMapItem mi = GetNextMapItem(e);
			Assert.IsTrue(mi.Size == 30);
			mi = GetNextMapItem(e);
			Assert.IsTrue(mi.Size == 0x0EE2);
			mi = GetNextMapItem(e);
			Assert.IsTrue(mi.Size == 0x100);
		}

		[Test]
		public void AddNamedSegment()
		{
			ImageMap map = new ImageMap(new Address(0x0B00, 0), 40000);
			map.AddSegment(new Address(0xC00, 0), "0C00", AccessMode.ReadWrite);
			IDictionaryEnumerator e = map.Segments.GetEnumerator();
			GetNextMapSegment(e);
			ImageMapSegment s = GetNextMapSegment(e);
			Assert.AreEqual("0C00", s.Name);
			Assert.AreEqual(35904, s.Size);
		}


		private void ImageItems_ItemSplit(object o, ItemSplitArgs isa)
		{
			++cItemsSplit;
		}
	}
}
