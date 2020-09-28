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
using Reko.Core.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class UbjsonWriterTests
    {
        private MemoryStream stream;

        private void Given_Stream()
        {
            this.stream = new MemoryStream();
        }

        [Test]
        public void Ubjs_WriteNull()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(null);
            Assert.AreEqual(new byte[] { 0x5A }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteSmallString()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write("hello");
            Assert.AreEqual(new byte[] { 0x53, 0x69, 0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteLargeString()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new string('Ä', 32800));
            var a = stream.ToArray();
            Assert.AreEqual(65606, a.Length);
            Assert.AreEqual(0x53, a[0]);
            Assert.AreEqual(0x6C, a[1]);
            Assert.AreEqual(0x00, a[2]);
            Assert.AreEqual(0x01, a[3]);
            Assert.AreEqual(0x00, a[4]);
            Assert.AreEqual(0x40, a[5]);
            Assert.AreEqual(0xC3, a[6]);
            Assert.AreEqual(0x84, a[7]);
            Assert.AreEqual(0xC3, a[8]);
            Assert.AreEqual(0x84, a[9]);
        }

        [Test]
        public void Ubjs_WriteArray_Unoptimized()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new ArrayList { 3, "foo!", false });
            Assert.AreEqual(new byte[] {
                0x5B,
                     0x69, 0x03,
                     0x53, 0x69, 0x04, 0x66, 0x6F, 0x6F, 0x21,
                     0x46,
                0x5D
            }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteArray_IntCollction()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new List<int> { 3, -4 });
            Assert.AreEqual(new byte[]
            {
                0x5B, 0x24, 0x6C, 0x23, 0x69, 0x02,
                      0x00, 0x00, 0x00, 0x03,
                      0xFF, 0xFF, 0xFF, 0xFC,
            }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteArray_ByteArray()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new byte[] { 0x1, 0x2, 0x3 });
            Assert.AreEqual(new byte[]
            {
                0x5B, 0x24, 0x55, 0x23, 0x69, 0x03,
                      0x01, 0x02, 0x03,
            }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteArray_IntArray()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new List<int> { 0x41, 0x812, 0x12345678 });
            Assert.AreEqual(new byte[]
            {
                0x5B, 0x24, 0x6C, 0x23, 0x69, 0x03,
                      0x00, 0x00, 0x00, 0x41,
                      0x00, 0x00, 0x08, 0x12,
                      0x12, 0x34, 0x56, 0x78,
            }, stream.ToArray());
        }

        [Test]
        public void Ubjs_WriteObject()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Write(new { Name = "Bob", Age = 32 });
            Assert.AreEqual(new byte[]
            {
                0x7B,
                      0x69, 0x04, 0x4E, 0x61, 0x6D, 0x65,
                      0x53, 0x69, 0x03, 0x42, 0x6F, 0x62,

                      0x69, 0x03, 0x41, 0x67, 0x65,
                      0x69, 0x20,
                0x7D,
            }, stream.ToArray());
        }
    }
}
