#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Loading;

namespace Reko.UnitTests.Decompiler.Loading
{
    [TestFixture]
    public class NullImageLoaderTests
    {
        [Test]
        public void Nil_MemoryMap()
        {
            var mmap = new SegmentMap(
                Address.Ptr16(0x0000),
                new ImageSegment("low_memory_area", new ByteMemoryArea(Address.Ptr16(0x0000), new byte[0x100]), AccessMode.ReadWriteExecute));
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            platform.Setup(p => p.CreateAbsoluteMemoryMap()).Returns(mmap);
            arch.Setup(a => a.CreateCodeMemoryArea(
                It.IsAny<Address>(),
                It.IsAny<byte[]>())).
                Returns(delegate (Address addr, byte[] bytes)
                {
                    return new ByteMemoryArea(addr, bytes);
                });

            var ldr = new NullImageLoader(null, ImageLocation.FromUri("foo.exe"), new byte[0x1000]);
            var segMap = ldr.CreatePlatformSegmentMap(platform.Object, Address.Ptr16(0x0100), new(), new byte[] { 0x50 });
            Assert.AreEqual(2, segMap.Segments.Count);
            var bmemProg = (ByteMemoryArea) segMap.Segments.Values.ElementAt(1).MemoryArea;
            Assert.AreEqual(1, bmemProg.Length);
            Assert.AreEqual((byte)0x50, bmemProg.Bytes[0]);
        }

        [Test]
        public void Nil_UserSegments()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.Architecture).Returns(arch.Object);
            arch.Setup(a => a.CreateCodeMemoryArea(
                It.IsAny<Address>(),
                It.IsAny<byte[]>())).
                Returns(delegate (Address addr, byte[] bytes)
                {
                    return new ByteMemoryArea(addr, bytes);
                });

            var img = new byte[0x1000];
            var ldr = new NullImageLoader(null, ImageLocation.FromUri("foo.exe"), img);
            var segMap = ldr.CreatePlatformSegmentMap(
                platform.Object,
                Address.Ptr16(0x1000),
                new List<UserSegment>
                {
                    new UserSegment { Name = ".text", Address = Address.Ptr16(0x1000), Length=0x800 },
                    new UserSegment { Name = ".data", Address = Address.Ptr16(0x1800), Length=0x800}
                },
                img);
            Assert.AreEqual(2, segMap.Segments.Count);
            var dataSeg = segMap.Segments.Values.ElementAt(1);

        }
    }
}
