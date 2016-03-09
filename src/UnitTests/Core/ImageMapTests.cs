#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Core
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
		public void Im_Creation()
		{
			ImageMap im = new ImageMap(addrBase, img.Length);

			im.AddSegment(Address.SegPtr(0x8000, 2), "",  AccessMode.ReadWrite, 10);
			im.AddSegment(Address.SegPtr(0x8000, 3), "", AccessMode.ReadWrite, 10);
			im.AddSegment(Address.SegPtr(0x8000, 0), "", AccessMode.ReadWrite, 10);

			// Verify

			var e = im.Segments.Values.ToArray();
			ImageSegment seg = e[0];
            Assert.AreEqual(Address.SegPtr(0x8000, 0), seg.Address);
			Assert.AreEqual(10, seg.Size);

            seg = e[1];
			Assert.AreEqual(1, seg.Size);
			
            seg = e[2];
			Assert.AreEqual(10, seg.Size);

            Assert.IsTrue(e.Length == 3);
		}

        [Test(Description = "Newly created segments should be covered by an item that covers the memory area.")]
        public void Im_CreateCoveringItem()
        {
            var map = new ImageMap(Address.Ptr32(0x01000));
            map.AddSegment(
                new MemoryArea(Address.Ptr32(0x01010), new byte[0x10]),
                ".text",
                AccessMode.ReadExecute);

            Assert.AreEqual(1, map.Items.Count);
            var item = map.Items.Values.First();
            Assert.AreEqual(Address.Ptr32(0x1010), item.Address);
            Assert.AreEqual(16, item.Size);
        }

		[Test]
		public void Im_Overlaps()
		{
			ImageMap im = new ImageMap(Address.SegPtr(0x8000, 0), 40);
			im.AddSegment(Address.SegPtr(0x8000, 10), "", AccessMode.ReadWrite, 0);
		}

		private ImageMapItem GetNextMapItem(IEnumerator<KeyValuePair<Address, ImageMapItem>> e)
		{
			Assert.IsTrue(e.MoveNext());
            return e.Current.Value;
		}

		private ImageSegment GetNextMapSegment(IEnumerator<KeyValuePair<Address, ImageSegment>> e)
		{
			Assert.IsTrue(e.MoveNext());
            return e.Current.Value;
		}

		[Test]
		public void Im_AddNamedSegment()
		{
			ImageMap map = new ImageMap(Address.SegPtr(0x0B00, 0), 40000);
			map.AddSegment(Address.SegPtr(0xC00, 0), "0C00", AccessMode.ReadWrite, 6000);
			IEnumerator<KeyValuePair<Address,ImageSegment>> e = map.Segments.GetEnumerator();
			ImageSegment s = GetNextMapSegment(e);
			Assert.AreEqual("0C00", s.Name);
			Assert.AreEqual(6000, s.Size);
		}

        [Test]
        public void Im_CreateItem_MiddleOfEmptyRange()
        {
            var mem = new MemoryArea(addrBase, new byte[0x100]);
            var map = new ImageMap(addrBase,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
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
        public void Im_CreateItem_AtExistingRange()
        {
            var map = new ImageMap(addrBase, 0x0100);
            map.AddSegment(addrBase, "8000", AccessMode.ReadWrite, 0x100);
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
        public void Im_FireChangeEvent()
        {
            var map = new ImageMap(addrBase, 0x100);
            var mapChangedFired = false;
            map.MapChanged += (sender, e) => { mapChangedFired = true; };
            map.AddItem(addrBase, new ImageMapItem { DataType = new CodeType() });
            Assert.IsTrue(mapChangedFired, "ImageMap should have fired MapChanged event");

        }
	}
}
