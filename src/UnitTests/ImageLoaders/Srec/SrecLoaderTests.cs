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
using Reko.Core.Memory;
using Reko.ImageLoaders.Srec;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.Srec
{
    [TestFixture]
    public class SrecLoaderTests
    {
        private ServiceContainer sc;
        private FakeArchitecture arch;
        private DefaultPlatform platform;
        private StringWriter sw;
        private Program program;

        public SrecLoaderTests()
        {
            this.sc = new ServiceContainer();
            this.arch = new FakeArchitecture(sc);
            this.platform = new DefaultPlatform(sc, arch);
        }

        [SetUp]
        public void Setup()
        {
            this.sw = new StringWriter();
        }

        private void Given_Header(string data)
        {
            WriteRecord("S0", "0000", Encoding.ASCII.GetBytes(data));
        }

        private void WriteRecord(string type, string address, byte[] data)
        {
            sw.Write(type);
            var length = address.Length / 2 + data.Length + 1;
            sw.Write("{0:X2}", length);
            sw.Write(address);
            sw.Write(string.Join("", data.Select(b => $"{b:X2}")));
            sw.Write("00");     // n/a checksum
            sw.WriteLine();
        }

        private void Given_Data16(ushort uAddr, params byte[] bytes)
        {
            WriteRecord("S1", uAddr.ToString("X4"), bytes);
        }

        private void When_LoadFile()
        {
            var file = Encoding.ASCII.GetBytes(sw.ToString());
            var srecLdr = new SrecLoader(new ServiceContainer(), ImageLocation.FromUri("file:test.srec"), file);
            this.program = srecLdr.LoadProgram(default, arch, platform, new());
        }

        [Test]
        public void Srec_Header_Single16Line()
        {
            Given_Header("Header data");
            Given_Data16(0x0420, 0x00, 0x02, 0x3);
            When_LoadFile();
            Assert.AreEqual(0x0420, program.SegmentMap.BaseAddress.ToLinear());
            Assert.AreEqual(1, program.SegmentMap.Segments.Count);
            var seg = program.SegmentMap.Segments.Values.First();
            var bmem = (ByteMemoryArea) seg.MemoryArea;
            Assert.AreEqual(3, bmem.Bytes.Length);
            Assert.AreEqual(0x3, bmem.Bytes[2]);
        }
    }
}
