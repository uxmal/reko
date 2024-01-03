#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.ImageLoaders.WebAssembly;
using Reko.UnitTests.Arch;
using System.Collections.Generic;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmDisassemblerTests : DisassemblerTestBase<WasmInstruction>
    {
        private readonly WasmArchitecture arch;
        private readonly Address addr;

        public WasmDisassemblerTests()
        {
            this.arch = new WasmArchitecture(CreateServiceContainer(), "wasm", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void WasmDis_i64_store8()
        {
            AssertCode("i64.store8\t0x4,0x4", "3C0404");
        }

        [Test]
        public void WasmDis_block()
        {
            AssertCode("block", "0240");
        }

        [Test]
        public void WasmDis_block_with_type()
        {
            AssertCode("block\t0x1", "0201");
        }

        [Test]
        public void WasmDis_i32_const_negative()
        {
            AssertCode("i32.const\t0xFFFFFFFF", "417F");
        }
    }
}
