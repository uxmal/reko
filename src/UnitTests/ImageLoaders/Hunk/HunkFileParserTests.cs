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
using Reko.ImageLoaders.Hunk;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.ImageLoaders.Hunk
{
    [TestFixture]
    public class HunkFileParserTests
    {
        private HunkMaker hm;

        [SetUp]
        public void Setup()
        {
            hm = new HunkMaker();
        }
        [Test]
        public void Hfp_ReadString_ZeroLength()
        {
            var bytes = hm.MakeImageReader(0);
            var parser = new HunkFileParser( bytes);
            Assert.AreEqual("", parser.ReadString());
        }

        [Test]
        public void Hfp_ReadString_TrailingZeros()
        {
            var bytes = hm.MakeImageReader("a");
            var parser = new HunkFileParser(bytes);
            Assert.AreEqual("a", parser.ReadString());
        }

        [Test]
        public void Hfp_HeaderBlock()
        {
            var rdr = hm.MakeImageReader(
                //0x3F3,
                "Hello", "",
                2,
                0,
                1,
                4,
                16);
            var parser = new HunkFileParser(rdr);
            HeaderHunk hdr = parser.ParseHeader(q => { });

            Assert.AreEqual(1, hdr.HunkNames.Count);
            Assert.AreEqual("Hello", hdr.HunkNames[0]);
            Assert.AreEqual(0, hdr.FirstHunkId);
            Assert.AreEqual(1, hdr.LastHunkId);
            Assert.AreEqual(2, hdr.HunkInfos.Count);
            Assert.AreEqual(16, hdr.HunkInfos[0].Size);
            Assert.AreEqual(64, hdr.HunkInfos[1].Size);
        }

        [Test]
        public void Hfp_CodeBlock()
        {
            var rdr = hm.MakeImageReader(
                //0x3E9,    // Assumes this is already read
                2,
                3,
                4);
            var parser = new HunkFileParser(rdr);
            TextHunk code = parser.ParseText(q => { });
            Assert.AreEqual(12, rdr.Offset);
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 3,  0, 0, 0, 4 
                },
                code.Data);
        }

        [Test]
        public void Hfp_DebugHunk()
        {
            var rdr = hm.MakeImageReader(
                //0x3F3,
                2,
                0x00,
                1234,
                0x12345678);
            var parser = new HunkFileParser(rdr);
            parser.ParseDebug(q => { });

            var nextWord = rdr.ReadBeInt32();
            Assert.AreEqual(0x12345678, nextWord);
        }
    }
}
