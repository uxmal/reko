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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using NUnit.Framework;
using Reko.Core.Rtl;
using Reko.ImageLoaders.WebAssembly;
using Reko.Core.Memory;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmRewriterTests : Arch.RewriterTestBase
    {
        private readonly WasmArchitecture arch;
        private readonly Address addr = Address.Ptr32(0x00123400);

        public WasmRewriterTests()
        {
            this.arch = new WasmArchitecture(CreateServiceContainer(), "wasm", new Dictionary<string, object>());
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        [Test]
        public void WasmRw_Const()
        {
            base.Given_HexString("4104");
            base.AssertCode(
                "0|L--|00123400(2): 3 instructions",
                "1|L--|v3 = 4<32>",
                "2|L--|sp = sp - 8<i32>",
                "3|L--|Mem0[sp:word32] = v3");
        }

        [Test]
        public void WasmRw_Const_r32()
        {
            Given_HexString("43C3F548C0");
            base.AssertCode(
                "0|L--|00123400(5): 3 instructions",
                "1|L--|v3 = -3.14F",
                "2|L--|sp = sp - 8<i32>",
                "3|L--|Mem0[sp:real32] = v3");
        }

        [Test]
        public void WasmRw_i64_store8()
        {
            Given_HexString("3C0404");
            AssertCode(
                "0|L--|00123400(3): 4 instructions",
                "1|L--|v4 = Mem0[sp:ptr32]",
                "2|L--|v5 = Mem0[sp + 8<i32>:byte]",
                "3|L--|Mem0[v4 + 4<i32>:byte] = v5",
                "4|L--|sp = sp + 16<i32>");
        }
    }
}
