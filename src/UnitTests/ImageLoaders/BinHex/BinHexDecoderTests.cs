#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.UnitTests.ImageLoaders.BinHex
{
    [TestFixture]
    public class BinHexDecoderTests
    {
        StringWriter file;
        BinHexEncoder enc;
        MemoryStream stm;

        [SetUp]
        public void Setup()
        {
            file = new StringWriter();
            file.Write("(This file must be converted with BinHex 4.0)");
            stm = new MemoryStream();
            enc = new BinHexEncoder(file);

        }

        [Test]
        public void DecodeOnes()
        {
            file.Write(":rrrr:");
            BinHexDecoder decoder = CreateDecoder();
            foreach (byte b in decoder.GetBytes())
            {
                stm.WriteByte(b);
            }
            Assert.AreEqual(3, stm.Position);
            byte[] bytes = stm.GetBuffer();
            Assert.AreEqual(0xFF, bytes[0]);
            Assert.AreEqual(0xFF, bytes[1]);
            Assert.AreEqual(0xFF, bytes[2]);
        }

        [Test]
        public void ExpandRunLengthSequence()
        {
            file.Write(":");
            Encode(0x42, 0x90, 0x03);
            file.Write(":");
            BinHexDecoder decoder = CreateDecoder();
            IEnumerator<byte> e = decoder.GetBytes().GetEnumerator();
            Assert.AreEqual(0x42, Get(e));
            Assert.AreEqual(0x42, Get(e));
            Assert.AreEqual(0x42, Get(e));
            Assert.IsFalse(e.MoveNext());
        }

        private byte Get(IEnumerator<byte> e)
        {
            Assert.IsTrue(e.MoveNext());
            return e.Current;
        }

        private BinHexDecoder CreateDecoder()
        {
            return new BinHexDecoder(new StringReader(file.ToString()));
        }

        private void Encode(params byte[] bytes)
        {
            for (int i = 0;  i < bytes.Length; ++i)
            {
                enc.Encode(bytes[i]);
            }
        }
    }
}
