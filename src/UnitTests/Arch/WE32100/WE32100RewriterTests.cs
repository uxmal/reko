#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.WE32100;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.WE32100
{
    [TestFixture]
    public class WE32100RewriterTests : RewriterTestBase
    {
        private readonly WE32100Architecture arch;
        private readonly Address addr;

        public WE32100RewriterTests()
        {
            this.arch = new WE32100Architecture(CreateServiceContainer(), "we32100", new Dictionary<string, object>());
            this.addr = Address.Create(arch.PointerType, 0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void WE32100Rw_addw3()
        {
            Given_HexString("DC907412692A1944"); // FCD2BD3C2FD668E8");
            AssertCode(
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r4 = Mem0[Mem0[r0 + 711529076<i32>:ptr32]:word32] + 0x19<32>",
                "2|L--|NZVC = cond(r4)");
        }

        [Test]
        public void WE32100Rw_dech()
        {
            Given_HexString("96B979CA"); //69C1B17D7D2E12E468B510AC");
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = Mem0[Mem0[fp - 13703<i32>:ptr32]:word16] - 1<16>",
                "2|L--|Mem0[Mem0[fp - 13703<i32>:ptr32]:word16] = v3",
                "3|L--|NZVC = cond(v3)");
        }

        [Test]
        public void WE32100Rw_movb_deferred()
        {
            Given_HexString("870C 9F36 88CB 39"); // E8110FF6AB36EDCADF");
            AssertCode(
                "0|L--|00100000(7): 3 instructions",
                "1|L--|Mem0[Mem0[pc + 969639990<i32>:ptr32]:byte] = 0xC<8>",
                "2|L--|NZV = cond(0xC<8>)",
                "3|L--|C = false");
        }

        [Test]
        public void WE32100Rw_xorb2()
        {
            Given_HexString("B73CD185"); //AD23AA3A2E6B8DA00BD381BD");
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Mem0[Mem0[r1 - 123<i32>:ptr32]:byte] ^ 0x3C<8>",
                "2|L--|Mem0[Mem0[r1 - 123<i32>:ptr32]:byte] = v3",
                "3|L--|NZV = cond(v3)",
                "4|L--|C = false");
        }

        [Test]
        public void WE32100Rw_xorh2()
        {
            Given_HexString("B69D D30C CB4A D8FE");
            AssertCode(
                "0|L--|00100000(8): 4 instructions",
                "1|L--|v4 = Mem0[Mem0[r8 - 2<i32>:ptr32]:word16] ^ Mem0[Mem0[pcbp + 1254821075<i32>:ptr32]:word16]",
                "2|L--|Mem0[Mem0[r8 - 2<i32>:ptr32]:word16] = v4",
                "3|L--|NZV = cond(v4)",
                "4|L--|C = false");
        }

        [Test]
        public void WE32100Rw_xorw2()
        {
            Given_HexString("B47D 8338 35C3 C2");
            AssertCode(
                "0|L--|00100000(7): 4 instructions",
                "1|L--|v4 = Mem0[Mem0[r3 - 1027394248<i32>:ptr32]:word32] ^ Mem0[ap - 3<i32>:word32]",
                "2|L--|Mem0[Mem0[r3 - 1027394248<i32>:ptr32]:word32] = v4",
                "3|L--|NZV = cond(v4)",
                "4|L--|C = false");
        }
    }
}
