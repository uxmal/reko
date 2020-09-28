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
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core
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
            var mem = new MemoryArea(Address.Ptr32(0x00123400), bytes);
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
    }
}
