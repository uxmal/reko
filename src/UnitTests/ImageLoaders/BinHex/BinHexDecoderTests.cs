using Decompiler.ImageLoaders.BinHex;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.ImageLoaders.BinHex
{
    [TestFixture]
    public class BinHexDecoderTests
    {
        StringWriter file;

        [SetUp]
        public void Setup()
        {
            file = new StringWriter();
            file.Write("(This file must be converted with BinHex 4.0)");
        }

        [Test]
        public void DecodeOnes()
        {
            file.Write(":rrrr:");
            BinHexDecoder decoder = new BinHexDecoder(new StringReader(file.ToString()));
            MemoryStream stm = new MemoryStream();
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
    }
}
