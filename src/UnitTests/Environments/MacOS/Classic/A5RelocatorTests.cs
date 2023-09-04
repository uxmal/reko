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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Environments.MacOS.Classic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class A5RelocatorTests
    {
        private A5Relocator relocator;
        private ByteMemoryArea mem;
        private ImageSegment a5world;

        [SetUp]
        public void Setup()
        {
            this.relocator = null;
            this.mem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[4096]);
            this.a5world = new ImageSegment("A5World", mem, AccessMode.ReadWriteExecute);
        }

        private void Given_Relocator()
        {
            var sc = new ServiceContainer();
            var arch = new M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var platform = new MacOSClassic(sc, arch)
            {
                A5World = a5world,
                A5Offset = 2048,
            };
            var mem = (ByteMemoryArea) platform.A5World.MemoryArea;
            var rdr = new BeImageReader(mem, 0);
            this.relocator = new A5Relocator(platform, rdr, 1024);
        }


        /// <summary>
        /// The first bytes will be 00 00, so the relocator will just stop.
        /// </summary>
        [Test]
        public void A5Relocator_NoOp()
        {
            Given_Relocator();

            relocator.Relocate();
        }

        [Test]
        public void A5Relocator_One()
        {
            Given_Relocator();
            var w = new BeImageWriter(mem.Bytes, 0);
            w.WriteByte(0x80);  // Skip 0.
            w.WriteByte(0x00);

            w = new BeImageWriter(mem.Bytes, 1024);
            w.WriteBeUInt32(0x00123400);

            relocator.Relocate();

            var r = mem.CreateBeReader(1024);
            var uRelocated = r.ReadBeUInt32();
            Assert.AreEqual(0x00223C00u, uRelocated);
        }

        [Test]
        public void A5Relocator_Skip_Repeat()
        {
            Given_Relocator();
            var w = new BeImageWriter(mem.Bytes, 0);
            w.WriteByte(0x00);  // Skip 2 halfwords.
            w.WriteByte(0x02);
            w.WriteByte(0x03);  // Repeat 3

            w = new BeImageWriter(mem.Bytes, 1024);
            w.WriteBeUInt32(0xFFFF0000u);
            w.WriteBeUInt32(0x00010000);
            w.WriteBeUInt32(0x00010004);
            w.WriteBeUInt32(0x00010008);
            w.WriteBeUInt32(0xFFFF0000u);

            relocator.Relocate();

            var r = mem.CreateBeReader(1024);
            Assert.AreEqual(0xFFFF0000u, r.ReadBeUInt32());
            Assert.AreEqual(0x00110800u, r.ReadBeUInt32());
            Assert.AreEqual(0x00110804u, r.ReadBeUInt32());
            Assert.AreEqual(0x00110808u, r.ReadBeUInt32());
            Assert.AreEqual(0xFFFF0000u, r.ReadBeUInt32());
        }

        [Test]
        public void A5Relocator_Skip_NoRepeat()
        {
            Given_Relocator();
            var w = new BeImageWriter(mem.Bytes, 0);
            w.WriteByte(0x02);  // Skip 2 halfwords.
            w.WriteByte(0x04);  // Skip 4 halfwords

            w = new BeImageWriter(mem.Bytes, 1024);
            w.WriteBeUInt32(0xFFFF0000u);
            w.WriteBeUInt32(0x00010000u);
            w.WriteBeUInt32(0xFFFF0000u);
            w.WriteBeUInt32(0x00010004u);
            w.WriteBeUInt32(0xFFFF0000u);

            relocator.Relocate();

            var r = mem.CreateBeReader(1024);
            Assert.AreEqual(0xFFFF0000u, r.ReadBeUInt32());
            Assert.AreEqual(0x00110800u, r.ReadBeUInt32());
            Assert.AreEqual(0xFFFF0000u, r.ReadBeUInt32());
            Assert.AreEqual(0x00110804u, r.ReadBeUInt32());
            Assert.AreEqual(0xFFFF0000u, r.ReadBeUInt32());
        }
    }
}
