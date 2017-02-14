#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
    public class WasmLoaderTests
    {
        private WasmLoader ldr;

        void Create_Loader(params byte[] bytes)
        {
            ldr = new WasmLoader(null, "foo.wasm", bytes);
        }

        [Test]
        public void WasmLdr_Header()
        {
            Create_Loader(
                0x00, 0x61, 0x73, 0x6D, // Magic cookie
                0x0B, 0x00, 0x00, 0x00 // version number.
            );

            var rdr = ldr.LoadHeader();
            Assert.AreEqual(8, rdr.Offset);
        }

        //[Test]
        //public void WasmLdr_Load
    }
}
