#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmRewriterTests : Arch.RewriterTestBase
    {
        private MemoryArea mem;
        private Address addr = Address.Ptr32(0x00123400);
        private WasmArchitecture arch;

        public WasmRewriterTests()
        {
            this.arch = new WasmArchitecture("wasm");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new WasmRewriter(arch, mem.CreateLeReader(mem.BaseAddress), arch.CreateFrame());
        }

        public override Address LoadAddress
        {
            get { return addr; }
        }

        private void BuildTest(params byte[] bytes)
        {
            this.mem = new MemoryArea(Address.Ptr32(0x00123400), bytes);
        }

        [Test]
        public void WasmRw_Const()
        {
            BuildTest(0x41, 0x04);
            base.AssertCode(
                "0|L--|00123400(2): 3 instructions",
                "1|L--|v2 = 0x00000004",
                "2|L--|sp = sp - 8",
                "3|L--|Mem0[sp:word32] = v2");
        }

        [Test]
        public void WasmRw_Const_r32()
        {
            BuildTest(0x43, 0xC3, 0xF5, 0x48, 0xC0);
            base.AssertCode(
                "0|L--|00123400(5): 3 instructions",
                "1|L--|v2 = -3.14F",
                "2|L--|sp = sp - 8",
                "3|L--|Mem0[sp:real32] = v2");
        }
    }
}
