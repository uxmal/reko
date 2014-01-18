#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.ImageLoaders.Hunk;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.ImageLoaders.Hunk
{
    [TestFixture]
    public class HunkFileParserTests
    {
        private Encoding enc = Encoding.GetEncoding("ISO_8859-1");

        private BeImageReader MakeBytes(params object[] data)
        {
            var w = new BeImageWriter();
            foreach (var o in data)
            {
                if (o is Int32)
                {
                    w.WriteBeUInt32((uint) (Int32) o);
                    continue;
                }
                var s = o as string;
                if (s != null)
                {
                    WriteString(s, w);
                    continue;
                }
                throw new NotImplementedException();
            }
            var bytes= w.Bytes.Take(w.Position).ToArray();
            return new BeImageReader(bytes, 0);
        }

        private void WriteString(string s, BeImageWriter w)
        {
            if (s.Length <= 0)
            {
                w.WriteBeUInt32(0);
                return;
            }
            byte[] ab = enc.GetBytes(s);
            int padLength = (ab.Length + 3) & ~3;
            w.WriteBeUInt32((uint) padLength / 4);
            w.WriteBytes(ab);
            int cPad = padLength - ab.Length;
            while (--cPad >= 0)
            {
                w.WriteByte(0);
            }
        }

        [Test]
        public void Hunk_ReadString_ZeroLength()
        {
            var bytes = MakeBytes(0);
            var parser = new HunkFileParser(bytes);
            Assert.AreEqual("", parser.ReadString(bytes));
        }

        [Test]
        public void Hunk_ReadString_TrailingZeros()
        {
            var bytes = MakeBytes("a");
            var parser = new HunkFileParser(bytes);
            Assert.AreEqual("a", parser.ReadString(bytes));
        }

        [Test]
        public void Hunk_HeaderBlock()
        {
            var rdr = MakeBytes(
                //0x3F3,
                "Hello", "",
                2,
                0,
                1,
                4,
                16);
            var parser = new HunkFileParser(rdr);
            HeaderHunk hdr = parser.ParseHeader(rdr);

            Assert.AreEqual(1, hdr.HunkNames.Count);
            Assert.AreEqual("Hello", hdr.HunkNames[0]);
            Assert.AreEqual(0, hdr.FirstHunkId);
            Assert.AreEqual(1, hdr.LastHunkId);
            Assert.AreEqual(2, hdr.HunkSizes.Count);
            Assert.AreEqual(16, hdr.HunkSizes[0].size);
            Assert.AreEqual(64, hdr.HunkSizes[1].size);
        }

        [Test]
        public void Hunk_CodeBlock()
        {
            var rdr = MakeBytes(
                //0x3E9,    // Assumes this is already read
                2,
                3,
                4);
            var parser = new HunkFileParser(rdr);
            TextHunk code = parser.ParseText(rdr);
            Assert.AreEqual(12, rdr.Offset);
            Assert.AreEqual(new byte[] {
                    0, 0, 0, 3,  0, 0, 0, 4 
                },
                code.Data);
        }
    }
}
