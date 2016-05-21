#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
        public void Ubjs_SaveNull()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Save(null);
            Assert.AreEqual(new byte[] { 0x5A }, stream.ToArray());
        }

        [Test]
        public void Ubjs_SaveSmallString()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Save("hello");
            Assert.AreEqual(new byte[] { 0x53, 0x69, 0x05, 0x68, 0x65, 0x6C, 0x6C, 0x6F }, stream.ToArray());
        }

        [Test]
        public void Ubjs_SaveLargeString()
        {
            Given_Stream();
            var ubjs = new UbjsonWriter(stream);
            ubjs.Save(new string('Ä', 32800));
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
    }
}
