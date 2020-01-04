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
using Reko.Loading;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class NullImageLoaderTests
    {
        [Test]
        public void Nil_MemoryMap()
        {
            var mmap = new SegmentMap(
                Address.Ptr16(0x0000),
                new ImageSegment("low_memory_area", new MemoryArea(Address.Ptr16(0x0000), new byte[0x100]), AccessMode.ReadWriteExecute));
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.CreateAbsoluteMemoryMap()).Returns(mmap);

            var ldr = new NullImageLoader(null, "foo.exe", new byte[0x1000]);
            var segMap = ldr.CreatePlatformSegmentMap(platform.Object, Address.Ptr16(0x0100), new byte[] { 0x50 });
            Assert.AreEqual(2, segMap.Segments.Count);
            var memProg = segMap.Segments.Values.ElementAt(1).MemoryArea;
            Assert.AreEqual(1, memProg.Length);
            Assert.AreEqual((byte)0x50, memProg.Bytes[0]);
        }
    }
}
