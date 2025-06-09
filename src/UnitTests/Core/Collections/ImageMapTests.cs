#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Core.Collections
{
    [TestFixture]
    public class ImageMapTests
    {
        private Address addrBase = Address.SegPtr(0x8000, 0);
        private byte[] img = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        public ImageMapTests()
        {
        }

        private ImageMapItem GetNextMapItem(IEnumerator<KeyValuePair<Address, ImageMapItem>> e)
        {
            Assert.IsTrue(e.MoveNext());
            return e.Current.Value;
        }

        private void CheckImageMapTypes(ImageMap map, params string[] types)
        {
            int length = types.Length;
            Assert.AreEqual(length, map.Items.Count);
            for (int i = 0; i < length; i++)
                Assert.AreEqual(types[i], map.Items.Values[i].DataType.ToString());
        }

        private void CheckImageMapSizes(ImageMap map, params int[] sizes)
        {
            int length = sizes.Length;
            Assert.AreEqual(length, map.Items.Count);
            for (int i = 0; i < length; i++)
                Assert.AreEqual(sizes[i], map.Items.Values[i].Size);
        }

        private void CheckImageMapAddresses(ImageMap map, params string[] addresses)
        {
            int length = addresses.Length;
            Assert.AreEqual(length, map.Items.Count);
            for (int i = 0; i < length; i++)
            {
                Assert.AreEqual(addresses[i], map.Items.Keys[i].ToString());
                Assert.AreEqual(addresses[i], map.Items.Values[i].Address.ToString());
            }
        }

        private Address CreateImageMapItem(ImageMap map, DataType dt, Address? a = null)
        {
            var lastAddr = map.Items.Count != 0 ? map.Items.Keys.Last() : map.BaseAddress;
            var curAddr = a ?? lastAddr;

            var size = (uint) dt.Size;

            var imageMapItem = new ImageMapItem(curAddr, size);
            if (dt is not null)
                imageMapItem.DataType = dt;
            map.AddItemWithSize(curAddr, imageMapItem);
            return imageMapItem.EndAddress;
        }

        [Test(Description = "Newly created segments should be covered by an item that covers the memory area.")]
        public void Im_CreateCoveringItem()
        {
            var segmentMap = new SegmentMap(Address.Ptr32(0x01000),
                new ImageSegment(
                    ".text",
                    new ByteMemoryArea(Address.Ptr32(0x01010), new byte[0x10]),
                    AccessMode.ReadExecute));
            var map = segmentMap.CreateImageMap();
            Assert.AreEqual(1, map.Items.Count);
            var item = map.Items.Values.First();
            Assert.AreEqual(Address.Ptr32(0x1010), item.Address);
            Assert.AreEqual(16, item.Size);
        }

        [Test]
        public void Im_CreateItem_MiddleOfEmptyRange()
        {
            var mem = new ByteMemoryArea(addrBase, new byte[0x100]);
            var segmentMap = new SegmentMap(addrBase,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            var map = segmentMap.CreateImageMap();
            map.AddItemWithSize(
                addrBase + 0x10,
                new ImageMapItem(addrBase + 0x10, 0x10) { DataType = new ArrayType(PrimitiveType.Byte, 10) });
            map.Dump();
            Assert.AreEqual(3, map.Items.Count);
            Assert.IsTrue(map.TryFindItemExact(addrBase, out ImageMapItem item));
            Assert.AreEqual(0x10, item.Size);
            Assert.IsInstanceOf<UnknownType>(item.DataType);
        }

        [Test]
        public void Im_CreateItem_AtExistingRange()
        {
            var segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(
                    "code",
                    new ByteMemoryArea(addrBase, new byte[0x100]),
                    AccessMode.ReadWrite));
            var map = segmentMap.CreateImageMap();
            map.AddItemWithSize(
                addrBase,
                new ImageMapItem(addrBase, 0x10) { DataType = new ArrayType(PrimitiveType.Byte, 0x10) });
            map.Dump();
            ImageMapItem item;
            Assert.IsTrue(map.TryFindItemExact(addrBase, out item));
            Assert.AreEqual("(arr byte 16)", item.DataType.ToString());
            Assert.IsTrue(map.TryFindItemExact(addrBase + 0x10, out item));
            Assert.IsInstanceOf<UnknownType>(item.DataType);
        }

        [Test]
        public void Im_RemoveItem()
        {
            var segmentMap = new SegmentMap(
                addrBase,
                new ImageSegment(
                    "code",
                    new ByteMemoryArea(addrBase, new byte[0x100]),
                    AccessMode.ReadWrite));
            var map = segmentMap.CreateImageMap();

            var itemAddress1 = addrBase;
            var itemAddress2 = CreateImageMapItem(map, PrimitiveType.Int32);
            var itemAddress3 = CreateImageMapItem(map, PrimitiveType.Int32);
            var itemAddress4 = CreateImageMapItem(map, PrimitiveType.Int32);
            var itemAddress5 = CreateImageMapItem(map, PrimitiveType.Int32);

            map.Dump();

            Assert.AreEqual(5, map.Items.Count);

            map.RemoveItem(itemAddress5);
            CheckImageMapTypes(map, "int32", "int32", "int32", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:0004", "8000:0008", "8000:000C", "8000:0010");
            CheckImageMapSizes(map, 4, 4, 4, 4, 240);

            map.RemoveItem(itemAddress1);
            CheckImageMapTypes(map, "<unknown>", "int32", "int32", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:0004", "8000:0008", "8000:000C", "8000:0010");
            CheckImageMapSizes(map, 4, 4, 4, 4, 240);

            map.RemoveItem(itemAddress3);
            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:0004", "8000:0008", "8000:000C", "8000:0010");
            CheckImageMapSizes(map, 4, 4, 4, 4, 240);

            map.RemoveItem(itemAddress2);
            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:000C", "8000:0010");
            CheckImageMapSizes(map, 12, 4, 240);

            CreateImageMapItem(map, PrimitiveType.Int32, itemAddress3);
            map.RemoveItem(itemAddress4);
            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:0008", "8000:000C");
            CheckImageMapSizes(map, 8, 4, 244);

            CreateImageMapItem(map, PrimitiveType.Int32, itemAddress4);
            map.RemoveItem(itemAddress3);
            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:000C", "8000:0010");
            CheckImageMapSizes(map, 12, 4, 240);

            map.RemoveItem(itemAddress4);
            CheckImageMapTypes(map, "<unknown>");
            CheckImageMapAddresses(map, "8000:0000");
            CheckImageMapSizes(map, 256);
        }

        [Test]
        public void Im_RemoveItem_DoNotMergeDisjointItems()
        {
            var mem = new ByteMemoryArea(addrBase, new byte[0x0100]);
            var segmentMap = new SegmentMap(addrBase,
                new ImageSegment("", mem, AccessMode.ReadWriteExecute));
            var codeMem = mem;
            var dataMem = new ByteMemoryArea(addrBase + 0x1000, new byte[0x0004]);
            var textMem = new ByteMemoryArea(addrBase + 0x2000, new byte[0x0100]);
            segmentMap.AddSegment(codeMem, "code", AccessMode.ReadWrite);
            segmentMap.AddSegment(dataMem, "data", AccessMode.ReadWrite);
            segmentMap.AddSegment(textMem, "text", AccessMode.ReadWrite);

            var map = segmentMap.CreateImageMap();

            CreateImageMapItem(map, PrimitiveType.Int32, addrBase + 0x1000);

            map.Dump();

            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:1000", "8000:2000");
            CheckImageMapSizes(map, 0x100, 0x4, 0x100);

            map.RemoveItem(codeMem.BaseAddress);
            CheckImageMapTypes(map, "<unknown>", "int32", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:1000", "8000:2000");
            CheckImageMapSizes(map, 0x100, 0x4, 0x100);

            map.RemoveItem(dataMem.BaseAddress);
            CheckImageMapTypes(map, "<unknown>", "<unknown>", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:1000", "8000:2000");
            CheckImageMapSizes(map, 0x100, 0x4, 0x100);

            map.RemoveItem(textMem.BaseAddress);
            CheckImageMapTypes(map, "<unknown>", "<unknown>", "<unknown>");
            CheckImageMapAddresses(map, "8000:0000", "8000:1000", "8000:2000");
            CheckImageMapSizes(map, 0x100, 0x4, 0x100);
        }
    }
}
