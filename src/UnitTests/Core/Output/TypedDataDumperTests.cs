#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.IO;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class TypedDataDumperTests
    {
        private readonly string cr = Environment.NewLine;
        private StringWriter sw;
        private TypedDataDumper tdd;

        private void Given_TypedDataDumper(byte[] bytes)
        {
            this.sw = new StringWriter();
            var fmt = new TextFormatter(sw);
            var mem = new ByteMemoryArea(Address.Ptr32(0x00123400), bytes);
            var rdr = mem.CreateLeReader(0);
            this.tdd = new TypedDataDumper(rdr, (uint) mem.Length, fmt);
        }

        [Test]
        public void Tdd_String_NullTerminated()
        {
            var bytes = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00 };
            Given_TypedDataDumper(bytes);

            var str = StringType.NullTerminated(PrimitiveType.Char);
            str.Accept(tdd);

            Assert.AreEqual("db\t'Hello',0x00" + cr, sw.ToString());
        }

        [Test]
        public void Tdd_String_Leading_NonPrintable()
        {
            var bytes = new byte[] { 0x0D, 0x0A, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00 };
            Given_TypedDataDumper(bytes);

            var str = StringType.NullTerminated(PrimitiveType.Char);
            str.Accept(tdd);

            Assert.AreEqual("db\t0x0D,0x0A,'Hello',0x00" + cr, sw.ToString());
        }

        [Test]
        public void Tdd_Word64()
        {
            var bytes = new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            };
            Given_TypedDataDumper(bytes);

            PrimitiveType.Word64.Accept(tdd);

            Assert.AreEqual("dq\t0x0807060504030201" + cr, sw.ToString());
        }

        [Test]
        public void Tdd_Ptr64()
        {
            var bytes = new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            };
            Given_TypedDataDumper(bytes);

            var ptr64 = new Pointer(VoidType.Instance, 64);
            ptr64.Accept(tdd);

            Assert.AreEqual("dq\t0x0807060504030201" + cr, sw.ToString());
        }
    }
}
