#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Scanning;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ImageMapTests
	{
		private Address addrBase = Address.SegPtr(0x8000, 0);
		private byte [] img = new Byte [] { 0x00, 0x00, 0x00, 0x00 };

		public ImageMapTests()
		{
		}

		[Test]
		public void ImageMapCreation()
		{
			ImageMap im = new ImageMap(addrBase, img.Length);

			im.AddSegment(Address.SegPtr(0x8000, 2), "",  AccessMode.ReadWrite);
			im.AddSegment(Address.SegPtr(0x8000, 3), "", AccessMode.ReadWrite);
			im.AddSegment(Address.SegPtr(0x8000, 0), "", AccessMode.ReadWrite);

			// Verify

			IEnumerator<KeyValuePair<Address,ImageMapSegment>> e = im.Segments.GetEnumerator();
			Assert.IsTrue(e.MoveNext());
			ImageMapSegment seg = e.Current.Value;
			Assert.AreEqual(2, seg.Size);

			Assert.IsTrue(e.MoveNext());
            seg = e.Current.Value;
			Assert.AreEqual(1, seg.Size);
			
			Assert.IsTrue(e.MoveNext());
			seg = e.Current.Value;
			Assert.AreEqual(1, seg.Size);

			Assert.IsTrue(!e.MoveNext());
		}

		[Test]
		public void ImageMapOverlaps()
		{
			ImageMap im = new ImageMap(Address.SegPtr(0x8000, 0), 40);
			im.AddSegment(Address.SegPtr(0x8000, 10), "", AccessMode.ReadWrite);
		}

		private ImageMapItem GetNextMapItem(IEnumerator<KeyValuePair<Address, ImageMapItem>> e)
		{
			Assert.IsTrue(e.MoveNext());
            return e.Current.Value;
		}

		private ImageMapSegment GetNextMapSegment(IEnumerator<KeyValuePair<Address, ImageMapSegment>> e)
		{
			Assert.IsTrue(e.MoveNext());
            return e.Current.Value;
		}

		[Test]
		public void AddNamedSegment()
		{
			ImageMap map = new ImageMap(Address.SegPtr(0x0B00, 0), 40000);
			map.AddSegment(Address.SegPtr(0xC00, 0), "0C00", AccessMode.ReadWrite);
			IEnumerator<KeyValuePair<Address,ImageMapSegment>> e = map.Segments.GetEnumerator();
			GetNextMapSegment(e);
			ImageMapSegment s = GetNextMapSegment(e);
			Assert.AreEqual("0C00", s.Name);
			Assert.AreEqual(35904, s.Size);
		}

        [Test]
        public void CreateTypedItem_EmptyMap()
        {
            var map = new ImageMap(addrBase, 0x0100);
            Assert.AreEqual(1, map.Items.Count);
            ImageMapItem item;
            Assert.IsTrue(map.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(0x100, item.Size);
        }

        [Test]
        public void CreateItem_MiddleOfEmptyRange()
        {
            var map = new ImageMap(addrBase, 0x0100);
            map.AddItemWithSize(
                addrBase + 0x10,
                new ImageMapItem(0x10) { DataType = new ArrayType(PrimitiveType.Byte, 10) });
            map.Dump();
            Assert.AreEqual(3, map.Items.Count);
            ImageMapItem item;
            Assert.IsTrue(map.TryFindItemExact(addrBase, out item));
            Assert.AreEqual(0x10, item.Size);
            Assert.IsInstanceOf<UnknownType>(item.DataType);
        }

        [Test]
        public void ImageMap_CreateItem_AtExistingRange()
        {
            var map = new ImageMap(addrBase, 0x0100);
            map.AddItemWithSize(
                addrBase,
                new ImageMapItem(0x10) { DataType = new ArrayType(PrimitiveType.Byte, 0x10) });
            map.Dump();
            ImageMapItem item;
            Assert.IsTrue(map.TryFindItemExact(addrBase, out item));
            Assert.AreEqual("(arr byte 16)", item.DataType.ToString());
            Assert.IsTrue(map.TryFindItemExact(addrBase + 0x10, out item));
            Assert.IsInstanceOf<UnknownType>(item.DataType);
        }

        [Test]
        public void ImageMap_FireChangeEvent()
        {
            var map = new ImageMap(addrBase, 0x100);
            var mapChangedFired = false;
            map.MapChanged += (sender, e) => { mapChangedFired = true; };
            map.AddItem(addrBase, new ImageMapItem { DataType = new CodeType() });
            Assert.IsTrue(mapChangedFired, "ImageMap should have fired MapChanged event");

        }
	}
}
