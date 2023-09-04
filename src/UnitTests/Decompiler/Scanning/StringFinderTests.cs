#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Scanning;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Memory;
using Reko.Core.Loading;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class StringFinderTests
    {
        private Program program;
        private Mock<IProcessorArchitecture> arch;

        [SetUp]
        public void Setup()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Name).Returns("FakeArch");
        }

        private void Given_Image(params byte[] bytes)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x00400000), bytes);
            arch.Setup(a => a.CreateImageReader(
                It.IsAny<ByteMemoryArea>(),
                It.IsAny<long>(),
                It.IsAny<long>()))
                .Returns(new LeImageReader(mem, 0));
            this.program = new Program
            {
                Architecture = arch.Object,
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment(".text", mem, AccessMode.ReadExecute)),
            };
        }

        [Test]
        public void StrFind_TooShort()
        {
            Given_Image(0x23, 0);

            var sf = new StringFinder(program);
            Assert.AreEqual(0, sf.FindStrings(new StringFinderCriteria(
                StringType: StringType.NullTerminated(PrimitiveType.Char),
                Encoding.ASCII,
                MinimumLength: 5,
                CreateReader: (m, a, b) => new LeImageReader(m, a, b))).Count());
        }

        [Test]
        public void StrFind_SingleMatch()
        {
            Given_Image(0x41, 0);

            var sf = new StringFinder(program);
            var hits = sf.FindStrings(new StringFinderCriteria(
                StringType: StringType.NullTerminated(PrimitiveType.Char),
                Encoding: Encoding.ASCII,
                MinimumLength: 1,
                CreateReader: (m, a, b) => new LeImageReader(m, a, b))).ToArray();
            Assert.AreEqual(1, hits.Length);
            Assert.AreEqual(Address.Ptr32(0x00400000), hits[0].Address);
        }

        [Test]
        public void StrFind_ValidCharacterRangeMatch()
        {
            Given_Image(0x06, 0, 0x07, 0, 0x0D, 0, 0x0E, 0, 0x1F, 00, 0x20, 0, 0x7E, 0, 0x7F, 0);

            var sf = new StringFinder(program);
            var hits = sf.FindStrings(new StringFinderCriteria(
                StringType: StringType.NullTerminated(PrimitiveType.Char),
                Encoding: Encoding.ASCII,
                MinimumLength: 1,
                CreateReader: (m, a, b) => new LeImageReader(m, a, b))).ToArray();
            Assert.AreEqual(4, hits.Length);
            Assert.AreEqual(Address.Ptr32(0x00400002), hits[0].Address);
            Assert.AreEqual(Address.Ptr32(0x00400004), hits[1].Address);
            Assert.AreEqual(Address.Ptr32(0x0040000A), hits[2].Address);
            Assert.AreEqual(Address.Ptr32(0x0040000C), hits[3].Address);
        }

        [Test]
        public void StrFind_TwoMatch()
        {
            Given_Image(0x42, 0, 0x12, 0x43, 0x00);

            var sf = new StringFinder(program);
            var hits = sf.FindStrings(new StringFinderCriteria(
                StringType: StringType.NullTerminated(PrimitiveType.Char),
                Encoding: Encoding.ASCII,
                MinimumLength: 1,
                CreateReader: (m, a, b) => new LeImageReader(m, a, b))).ToArray();
            Assert.AreEqual(2, hits.Length);
            Assert.AreEqual(Address.Ptr32(0x00400000), hits[0].Address);
            Assert.AreEqual(Address.Ptr32(0x00400003), hits[1].Address);
        }

        [Test]
        public void StrFind_FindUtf16Le()
        {
            var enc = Encoding.GetEncoding("utf-16le");
            Given_Image(enc.GetBytes("\0\0Hello\0"));

            var sf = new StringFinder(program);
            var hits = sf.FindStrings(new StringFinderCriteria(
                StringType: StringType.NullTerminated(PrimitiveType.UInt16),
                Encoding: enc,
                MinimumLength: 3,
                CreateReader: (m, a, b) => new LeImageReader(m, a, b))).ToArray();
            Assert.AreEqual(1, hits.Length);
            Assert.AreEqual(Address.Ptr32(0x00400004), hits[0].Address);
            Assert.AreEqual(10, hits[0].Length);
        }
    }
}
