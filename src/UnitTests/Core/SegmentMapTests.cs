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
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class SegmentMapTests
    {
        private Address addrBase = Address.SegPtr(0x8000, 0);
        private byte[] img = new byte[0x20];

        private void AssertMap(string sExp, SegmentMap sm)
        {
            var strs = sm.Segments.Values
                .Select(ss => string.Format("{0} {1:X8} {2}{3}{4} {5}",
                    ss.Address,
                    ss.Size,
                    (ss.Access & AccessMode.Read) != 0 ? "r" : "-",
                    (ss.Access & AccessMode.Write) != 0 ? "w" : "-",
                    (ss.Access & AccessMode.Execute) != 0 ? "x" : "-",
                    ss.Name));
            string s = string.Join(Environment.NewLine, strs);
            if (s != sExp)
                Debug.Print(s);
            Assert.AreEqual(sExp, s);
        }

        [Test]
        public void Sm_Creation()
        {
            var mem = new MemoryArea(addrBase, img);
            SegmentMap sm = new SegmentMap(addrBase,
                new ImageSegment("", mem, AccessMode.ReadWriteExecute));

            sm.AddOverlappingSegment("", mem, Address.SegPtr(0x8000, 2), AccessMode.ReadWrite);
            sm.AddOverlappingSegment("", mem, Address.SegPtr(0x8000, 3), AccessMode.ReadWrite);
            sm.AddOverlappingSegment("", mem, Address.SegPtr(0x8000, 0), AccessMode.ReadWrite);

            // Verify
            var sExp = 
@"8000:0000 00000002 rwx 
8000:0002 00000001 rw- 
8000:0003 0000001D rw- ";
            AssertMap(sExp, sm);
        }

        [Test]
        public void Sm_AddSegment()
        {
            var map = new SegmentMap(addrBase);
            var mem = new MemoryArea(addrBase, new byte[0x4000]);
            var seg = new ImageSegment("8100", Address.SegPtr(0x8100, 0), mem, AccessMode.ReadWriteExecute);
            map.AddSegment(seg);
            Assert.AreEqual(0x3000, seg.Size);
        }

        [Test]
        public void Sm_Overlaps()
        {
            SegmentMap im = new SegmentMap(Address.SegPtr(0x8000, 0));
            var mem = new MemoryArea(im.BaseAddress, new byte[40]);
            var seg = new ImageSegment("8000", Address.SegPtr(0x8000, 10), mem, AccessMode.ReadWrite);
            im.AddSegment(seg);
        }

        [Test]
        public void Sm_AddNamedSegment()
        {
            var mem = new MemoryArea(Address.SegPtr(0x0B00, 0), new byte[0x2000]);
            SegmentMap segmentMap = new SegmentMap(mem.BaseAddress,
                new ImageSegment("base", mem, AccessMode.ReadWriteExecute));
            segmentMap.AddOverlappingSegment("0C00", mem, Address.SegPtr(0xC00, 0), AccessMode.ReadWrite);
            ImageSegment s = segmentMap.Segments.Values[1];
            Assert.AreEqual(0x1000, s.Size);
        }

        [Test]
        public void Sm_MapLinearAddress()
        {
            var mem = new MemoryArea(Address.Ptr32(0x20), new byte[0x2000]);
            var segmentMap = new SegmentMap(mem.BaseAddress,
                new ImageSegment("base", mem, AccessMode.ReadWriteExecute));

            var addr = segmentMap.MapLinearAddressToAddress(0x30);

            Assert.AreEqual("00000030", addr.ToString());
        }

        [Test]
        public void Sm_MapZeroLinearAddress()
        {
            var mem = new MemoryArea(Address.Ptr32(0x20), new byte[0x2000]);
            var segmentMap = new SegmentMap(mem.BaseAddress,
                new ImageSegment("base", mem, AccessMode.ReadWriteExecute));

            try
            {
                var addr = segmentMap.MapLinearAddressToAddress(0);
                Assert.Fail("Should have thrown an ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}
