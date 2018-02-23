#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet - based on work of John Källén. 
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
using Reko.ImageLoaders.IntelHex32;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.IHex32
{
    [TestFixture]
    public class Hex32LoaderTests
    {
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            sc.AddService<DecompilerEventListener>(new FakeDecompilerEventListener());
        }

        [Test]
        public void Hex32ldr_Load()
        {
            string data =
@":020000040000FA
:060000009DEF00F012006C
:020006000000F8
:060008007EEF00F0120083
:02000E00060EDC
:10001000F66E000EF76E000EF86E00010900F55046
:10002000C56F0900F550C66F03E1C56701D03DD02B
:100030000900F550C06F0900F550C16F0900F55077
:10004000C26F09000900F550E96E0900F550EA6E2B
:10005000090009000900F550C36F0900F550C46F8D
:1000600009000900F6CFC7F0F7CFC8F0F8CFC9F004
:10007000C0C0F6FFC1C0F7FFC2C0F8FF0001C35304
:1000800002E1C45307E00900F550EE6EC307F8E241
:10009000C407F9D7C7C0F6FFC8C0F7FFC9C0F8FF4B
:0C00A0000001C507000EC65BBFD71200B0
:0400AC000001CA6B1A
:1000B000200EF26E840EF16ED08ED76AD66A820E52
:1000C000D56EF28E936A010EE66E02D841E91200F7
:1000D000D9CFE6FFE1CFD9FF82E900500BE0000164
:1000E000CAA107D0CA918A50010B02E08A8E01D0C2
:1000F0008A9EF3D741E9E7CFD9FF1200DACFE4FFB8
:10010000E2CFDAFFE652F2A404D0F2940001CA81F1
:0A0110008A70E552E5CFDAFF110016
:06011A00000EF36E00EE82
:1001200000F0010E01D81200EA6002D0EE6AFCD79E
:0A013000F350E9601200EE6AFCD7FC
:06013A0010EE80F020EE43
:1001400080F0F86A07EC00F0AAEC00F056EC00F042
:04015000FBD71200C7
:02015400120097
:020000040030CA
:0100010002FC
:01000600C138
:00000001FF
";
            var hex = new IntelHex32Loader(sc, "foo.text", Encoding.ASCII.GetBytes(data));
            var arch = new FakeArchitecture();
            var program = hex.Load(Address.Ptr32(0x00), arch, new DefaultPlatform(sc, arch));
            Assert.AreEqual(3, program.SegmentMap.Segments.Count);

            var memarea0 = program.SegmentMap.Segments.Values[0];
            Assert.AreEqual(Address.Ptr32(0x00000), memarea0.Address);
            Assert.AreEqual(0x156, memarea0.ContentSize);
            Assert.AreEqual(0x9D, memarea0.MemoryArea.Bytes[0]);
            Assert.AreEqual(0xE2, memarea0.MemoryArea.Bytes[0x100]);
            Assert.AreEqual(0x12, memarea0.MemoryArea.Bytes[0x154]);

            var memarea1 = program.SegmentMap.Segments.Values[1];
            Assert.AreEqual(Address.Ptr32(0x300001), memarea1.Address);
            Assert.AreEqual(1, memarea1.ContentSize);
            Assert.AreEqual(0x02, memarea1.MemoryArea.Bytes[0]);

            var memarea2 = program.SegmentMap.Segments.Values[2];
            Assert.AreEqual(Address.Ptr32(0x300006), memarea2.Address);
            Assert.AreEqual(1, memarea2.ContentSize);
            Assert.AreEqual(0xC1, memarea2.MemoryArea.Bytes[0]);
        }
    }
}

