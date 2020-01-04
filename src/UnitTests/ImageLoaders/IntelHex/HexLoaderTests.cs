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
using Reko.Core.Services;
using Reko.ImageLoaders.IntelHex;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.IntelHex
{
    [TestFixture]
    public class HexLoaderTests
    {
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
        }

        [Test]
        public void Hexldr_ThrowOnNullLine()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(null));
        }

        [Test]
        public void Hexldr_ThrowOnInvalidRecordMarker()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord("?"));
        }

        [Test]
        public void Hexldr_ThrowOnEmptyRecord()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(":"));
        }

        [Test]
        public void Hexldr_ThrowOnTooShortRecord()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(":00000001F"));
        }

        [Test]
        public void Hexldr_ThrowOnInvalidhexChars()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(":00XX0001FF"));
        }

        [Test]
        public void Hexldr_ThrowOnInvalidChecksum()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(":00000002FF"));
        }

        [Test]
        public void Hexldr_ThrowOnInvalidRecordType()
        {
            Assert.Throws<IntelHexException>(() => IntelHexParser.ParseRecord(":00000007F9"));
        }

        [Test]
        public void Hexldr_ExtdLinearAddrTest()
        { 
            var rec = IntelHexParser.ParseRecord(":020000040030CA");
            Assert.AreEqual(IntelHexRecordType.ExtendedLinearAddress, rec.RecordType, $"Expecting extended linear address, but was: {rec.RecordType}");
            Assert.AreEqual(0, rec.Address, $"Invalid extended linear address. Was 0x{rec.Address:X}");
            Assert.AreEqual(2, rec.ByteCount, $"Wrong data count. Was {rec.ByteCount}");
            Assert.AreEqual(new List<byte>() { 0, 0x30 }, rec.Data, $"Invalid data content for extended linear address");
        }

        [Test]
        public void Hexldr_DataRecordTest()
        { 
            var rec = IntelHexParser.ParseRecord(":060008000CEF18F01200DD");
            Assert.AreEqual(IntelHexRecordType.Data, rec.RecordType, $"Expecting data record, but was: {rec.RecordType}");
            Assert.AreEqual(8, rec.Address, $"Invalid data record address. Was 0x{rec.Address:X}");
            Assert.AreEqual(6, rec.ByteCount, $"Wrong data count. Was {rec.ByteCount}");
            Assert.AreEqual(new List<byte>() { 0xC, 0xEF, 0x18, 0xF0, 0x12, 0x00 }, rec.Data, $"Invalid data content for data record");
        }

        [Test]
        public void Hexldr_EOFRecordTest()
        {
            var rec = IntelHexParser.ParseRecord(":00000001FF");
            Assert.AreEqual(IntelHexRecordType.EndOfFile, rec.RecordType, $"Not an EOF record: {rec.RecordType}");
        }

        [Test]
        public void Hexldr_Load()
        {
            string data =
@":10010000214601360121470136007EFE09D2190140
:100110002146017E17C20001FF5F16002148011928
:10012000194E79234623965778239EDA3F01B2CAA7
:100130003F0156702B5E712B722B732146013421C7
:00000001FF
";
            var hex = new HexLoader(sc, "foo.text", Encoding.ASCII.GetBytes(data));
            var arch = new FakeArchitecture();
            var program = hex.Load(Address.Ptr32(0x00), arch, new DefaultPlatform(sc, arch));
            Assert.AreEqual(1, program.SegmentMap.Segments.Count);
            Assert.AreEqual(0x21, program.SegmentMap.Segments.Values[0].MemoryArea.Bytes[0]);
        }

        [Test]
        public void Hexldr_LoadOneSegtTests()
        {
            string data =
@"
:020000040000FA
:10010000214601360121470136007EFE09D2190140
:100110002146017E17C20001FF5F16002148011928
:10012000194E79234623965778239EDA3F01B2CAA7
:100130003F0156702B5E712B722B732146013421C7
:00000001FF
";
            var hex = new HexLoader(sc, "foo.text", Encoding.ASCII.GetBytes(data));
            var arch = new FakeArchitecture();
            var program = hex.Load(Address.Ptr32(0x00), arch, new DefaultPlatform(sc, arch));
            Assert.AreEqual(1, program.SegmentMap.Segments.Count, "Wrong number of segments");

            var memarea0 = program.SegmentMap.Segments.Values[0].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0x100), memarea0.BaseAddress);
            Assert.AreEqual(64, memarea0.Length);
            Assert.AreEqual(0x21, memarea0.Bytes[0]);
            Assert.AreEqual(0x21, memarea0.Bytes[0x10]);
            Assert.AreEqual(0x19, memarea0.Bytes[0x20]);
            Assert.AreEqual(0x34, memarea0.Bytes[0x3E]);
        }

        [Test]
        public void Hexldr_LoadMultSegtsTests()
        {
            string data =
 @"
:020000040030CA
:0100000024DB
:010001000EF0
:0100020039C4
:0100030014E8

:01000500807A
:01000600C039

:010008000FE8
:01000900C036
:01000A000FE6
:01000B00E014
:01000C000FE4
:01000D0040B2

:020000040000FA
:08000000000097EF1DF0120053
:060008000CEF18F01200DD

:060018003BEF18F012009E
:02001E001200CE
:100020000201526B4E6B516B4F6B4D6B5C6B5D6B9A
:100030005E6B1200D9CFE6FFE1CFD9FF81E947E837
:10004000076A0750060DF3CFF6FFF4CFF7FFB80E9F
:10005000F6262D0EF7220900F550016E0900F55025
:10006000026E0900F550036E0900F550046E090098
:10007000F550056E0900F550066E015013E0D95099
:00000001FF
";
            var hex = new HexLoader(sc, "foo.text", Encoding.ASCII.GetBytes(data));
            var arch = new FakeArchitecture();
            var program = hex.Load(Address.Ptr32(0x00), arch, new DefaultPlatform(sc, arch));
            Assert.AreEqual(5, program.SegmentMap.Segments.Count, "Wrong number of segments");

            var memarea0 = program.SegmentMap.Segments.Values[0].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0), memarea0.BaseAddress);
            Assert.AreEqual(14, memarea0.Length);
            Assert.AreEqual(0x00, memarea0.Bytes[0]);
            Assert.AreEqual(0x97, memarea0.Bytes[2]);

            var memarea1 = program.SegmentMap.Segments.Values[1].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0x18), memarea1.BaseAddress);
            Assert.AreEqual(104, memarea1.Length);
            Assert.AreEqual(0x3B, memarea1.Bytes[0]);

            var memarea2 = program.SegmentMap.Segments.Values[2].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0x300000), memarea2.BaseAddress);
            Assert.AreEqual(4, memarea2.Length);
            Assert.AreEqual(0x24, memarea2.Bytes[0]);

            var memarea3 = program.SegmentMap.Segments.Values[3].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0x300005), memarea3.BaseAddress);
            Assert.AreEqual(2, memarea3.Length);
            Assert.AreEqual(0x80, memarea3.Bytes[0]);

            var memarea4 = program.SegmentMap.Segments.Values[4].MemoryArea;
            Assert.AreEqual(Address.Ptr32(0x300008), memarea4.BaseAddress);
            Assert.AreEqual(6, memarea4.Length);
            Assert.AreEqual(0x0F, memarea4.Bytes[0]);
        }

    }
}
