#region License
/*
 * Copyright (C) 2018-2025 Stefano Moioli <smxdev4@gmail.com>.
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it 
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented;
 *    you must not claim that you wrote the original software.
 *    If you use this software in a product, an acknowledgment
 *    in the product documentation would be appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such,
 *    and must not be misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
#endregion
using NUnit.Framework;
using Reko.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.IO
{
    [TestFixture]
    public class SpanStreamTests
    {
        [Test]
        public void TestEndianness()
        {
            byte[] buf = new byte[48];
            var sst = new SpanStream(buf);
            sst.Endianness = Endianness.BigEndian;
            {
                sst.WriteUInt32(0xDEADBEEF);
            }
            sst.Seek(0, SeekOrigin.Begin);
            sst.Endianness = Endianness.LittleEndian;
            {
                Assert.AreEqual(0xEFBEADDE, sst.ReadUInt32());
            }
        }

        [Test]
        public void TestSpanStream()
        {
            var mem = new Memory<byte>(new byte[64]);

            var sst = new SpanStream(mem);
            sst.PerformAt(60, () =>
            {
                sst.WriteString("FOOT");
            });
            sst.Seek(4, SeekOrigin.Current);
            sst.WriteUInt32(0xDEADBEEF);
            sst.WriteUInt16(0xC0FF);
            sst.WriteByte(0xF0);
            sst.WriteCString("test");

            for (byte i = 0; i < 10; i++)
            {
                sst.WriteByte(i);
            }

            sst.Write(new byte[] { 0xfa, 0xfe }, 0, 2);

            sst.PerformAt(0, () =>
            {
                sst.WriteString("HEAD");
            });

            sst.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual("HEAD", sst.ReadString(4));
            Assert.AreEqual(0xDEADBEEF, sst.ReadUInt32());
            Assert.AreEqual(0xC0FF, sst.ReadUInt16());
            Assert.AreEqual(0xF0, sst.ReadByte());
            Assert.AreEqual("test", sst.ReadCString());
            for (byte i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, sst.ReadByte());
            }

            var bufRead = new byte[2];
            sst.Read(bufRead, 0, 2);
            Assert.AreEqual(0xfa, bufRead[0]);
            Assert.AreEqual(0xfe, bufRead[1]);

            Assert.AreEqual("FOOT", sst.PerformAt(60, () =>
            {
                return sst.ReadString(4);
            }));
        }
    }
}
