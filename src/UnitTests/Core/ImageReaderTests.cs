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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Core
{
    public class ImageReaderTests
    {
        [Test]
        public void ReadCString()
        {
            var img = new LeImageReader(new byte[] {
                0x12, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x77, 0x6f, 0x72,
                0x6c, 0x64, 0x21, 0x00, 0x12 },
                1);
            StringConstant str = img.ReadCString(PrimitiveType.Char, Encoding.UTF8);
            Assert.AreEqual("Hello world!", str.ToString());
        }

        [Test]
        public void ReadLengthPrefixedString()
        {
            var img =
                new LeImageReader(
                    new MemoryArea(
                        Address.Ptr32(0x10000),
                        new byte[] {
                            0x12, 0x34, 0x03, 0x00, 0x00, 0x00, 0x46, 0x00,
                            0x6f, 0x00, 0x6f, 0x00, 0x02, 0x02}),
                    2);
            StringConstant str = img.ReadLengthPrefixedString(PrimitiveType.Int32, PrimitiveType.WChar, Encoding.Unicode);
            Assert.AreEqual("Foo", str.ToString());
        }

        [Test(Description = "Tests a bounded image")]
        public void ImrBounded_ReadByte()
        {
            // The memarea is 2 bytes...
            var mem = new MemoryArea(Address.Ptr32(0x1213), new byte[] { 0x12, 0x34 });
            // ...but we wish to limit it to 1 byte
            var rdr = new LeImageReader(mem, mem.BaseAddress, mem.BaseAddress + 1);
            Assert.IsTrue(rdr.IsValid);
            Assert.AreEqual((byte)0x12, rdr.ReadByte());
            Assert.IsFalse(rdr.IsValid, "Should have respected the limit.");
        }

        [Test]
        public void ImrReadOffTheEnd()
        {
            var rdr = new ImageReader(new byte[] { 1, 2, 3, 4 });
            var buf = new byte[10];
            var read = rdr.Read(buf, 0, buf.Length);
            Assert.AreEqual(4, read);
            Assert.AreEqual(4, rdr.Offset);
        }

        [Test]
        public void ImrReadIntoMiddleOfBuffer()
        {
            var rdr = new ImageReader(new byte[] { 1, 2, 3, 4 });
            var buf = new byte[10];
            var read = rdr.Read(buf, 2, buf.Length);
            Assert.AreEqual(2, read);
            Assert.AreEqual(2, rdr.Offset);
            Assert.AreEqual(0, buf[0]);
            Assert.AreEqual(0, buf[1]);
            Assert.AreEqual(1, buf[2]);
            Assert.AreEqual(2, buf[3]);
        }
    }
}
