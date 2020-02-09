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
using Reko.ImageLoaders.OdbgScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.OdbgScript
{
    [TestFixture]
    public class OllyScriptHostTests
    {
        private SegmentMap segmentMap;
        private OdbgScriptHost host;

        [SetUp]
        public void Setup()
        {
            this.segmentMap = null;
            this.host = null;
        }

        private void Given_SegmentMap()
        {
            var addrBase = Address.Ptr32(0x00100000);
            this.segmentMap = new SegmentMap(addrBase);
            this.segmentMap.AddSegment(new MemoryArea(addrBase, new byte[0xFF]), ".text", AccessMode.ReadWrite);
        }

        private void Given_Host()
        {
            this.host = new OdbgScriptHost(null, segmentMap);
        }

        [Test]
        public void OdbgHost_Alloc()
        {
            Given_SegmentMap();
            Given_Host();

            var addr = host.AllocateMemory(4);
            Assert.AreEqual(0x00100100, addr.ToUInt32()); 
        }

        [Test]
        public void OdbgHost_Alloc_TwoAllocations()
        {
            Given_SegmentMap();
            Given_Host();

            var addr1 = host.AllocateMemory(4);
            var addr2 = host.AllocateMemory(4);
            Assert.AreEqual(0x00100100, addr1.ToUInt32());
            Assert.AreEqual(0x00100110, addr2.ToUInt32());
        }
    }
}
