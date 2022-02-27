#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.ImageLoaders.WebAssembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmImageReaderTests
    {
        [Test]
        public void WasmRdr_Signed64_Positive()
        {
            var mem = new byte[] { 0x02 };
            var rdr = new WasmImageReader(mem);
            Assert.IsTrue(rdr.TryReadVarInt64(out long value));
            Assert.AreEqual(2, value);
        }

        [Test]
        public void WasmRdr_Signed64_0x6F()
        {
            var mem = new byte[] { 0x6F };
            var rdr = new WasmImageReader(mem);
            Assert.IsTrue(rdr.TryReadVarInt64(out long value));
            Assert.AreEqual(-17L, value);
            Assert.AreEqual(0x6F, value & 0x7F);
        }

        [Test]
        public void WasmRdr_Signed64_0x7F()
        {
            var mem = new byte[] { 0x7F };
            var rdr = new WasmImageReader(mem);
            Assert.IsTrue(rdr.TryReadVarInt64(out long value));
            Assert.AreEqual(-1, value);
            Assert.AreEqual(0x7F, value & 0x7F);
        }

        [Test]
        public void WasmRdr_Signed64_0x40()
        {
            var mem = new byte[] { 0x40 };
            var rdr = new WasmImageReader(mem);
            Assert.IsTrue(rdr.TryReadVarInt64(out long value));
            Assert.AreEqual(-64L, value);
            Assert.AreEqual(0x40, value & 0x7F);
        }
    }
}
