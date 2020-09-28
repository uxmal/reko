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

using Reko.ImageLoaders.BinHex;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Reko.UnitTests.ImageLoaders.BinHex
{
    [TestFixture]
    public class BinHexImageLoaderTests
    {
        [Test]
        public void BinHex_LoadHeader()
        {
            StringWriter file = new StringWriter();
            BinHexEncoder enc = new BinHexEncoder(file);
            enc.WriteHeader();
            Encoding ascii = Encoding.ASCII;
            byte [] filename = ascii.GetBytes("foo.bar");
            enc.Encode((byte)filename.Length);
            enc.Encode(filename);
            enc.Encode(0x00);
            enc.Encode(ascii.GetBytes("FTYP"));
            enc.Encode(ascii.GetBytes("CREA"));
            enc.Encode(ascii.GetBytes("FL"));
            enc.Encode(new byte[] { 0x01, 0x02, 0x03, 0x04 });
            enc.Encode(new byte[] { 0x05, 0x06, 0x07, 0x08 });
            enc.Encode(0x00);
            enc.Encode(0x00);
            enc.Flush();

            var loader = new BinHexImageLoader(null, "foo.bar", null);
            var header = CreateDecoder(file).GetBytes().ToArray();
            var hdr = loader.LoadBinHexHeader(header.AsEnumerable<byte>().GetEnumerator());
            Assert.AreEqual("foo.bar", hdr.FileName);
            Assert.AreEqual("FTYP", hdr.FileType);
            Assert.AreEqual("CREA", hdr.FileCreator);
            Assert.AreEqual(0x01020304, hdr.DataForkLength, string.Format("{0:X8}:{1:X8}", 0x01020304, hdr.DataForkLength));
            Assert.AreEqual(0x05060708, hdr.ResourceForkLength);

        }

        private BinHexDecoder CreateDecoder(StringWriter file)
        {
            return new BinHexDecoder(new StringReader(file.ToString()));
        }
    }
}
