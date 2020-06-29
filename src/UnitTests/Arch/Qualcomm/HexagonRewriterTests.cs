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
using Reko.Arch.Qualcomm;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Qualcomm
{
    [TestFixture]
    public class HexagonRewriterTests : RewriterTestBase
    {
        private readonly HexagonArchitecture arch;
        private readonly Address addrLoad;

        public HexagonRewriterTests()
        {
            this.arch = new HexagonArchitecture(CreateServiceContainer(), "hexagon");
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override Address LoadAddress => addrLoad;

        public override IProcessorArchitecture Architecture => arch;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(arch.CreateImageReader(mem, 0), arch.CreateProcessorState(), binder, host);
        }

        [Test]
        public void HexagonRw_add()
        {
            Given_HexString("1DF8FDBF");
            AssertCode(     // { r29 = add(r29,FFFFFFC0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r29 = r29 + 0xFFFFFFC0<32>");
        }

        [Test]
        public void HexagonRw_allocframe()
        {
            Given_HexString("01C09DA0");
            AssertCode(     // { allocframe(+00000008) }
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v2 = r29 - 8<i32>",
                "2|L--|Mem0[v2:word32] = r30",
                "3|L--|Mem0[v2 + 4<i32>:word32] = r31",
                "4|L--|r30 = v2",
                "5|L--|r29 = v2 - 8<i32>");
        }

        [Test]
        public void HexagonRw_and()
        {
            Given_HexString("E2C30076");
            AssertCode(     // { r2 = and(r0,0000001F) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r0 & 0x1F<32>");
        }

        [Test]
        public void HexagonRw_assign_immediate()
        {
            Given_HexString("00C02072");
            AssertCode(     // { r0.h = 0000 }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = SEQ(0<16>, SLICE(r0, word16, 0))");
        }

        [Test]
        public void HexagonRw_assign_rev()
        {
            Given_HexString("00C09D6E");
            AssertCode(     // { r0 = rev }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = rev");
        }

        [Test]
        public void HexagonRw_crswap()
        {
            Given_HexString("00C01D65");
            AssertCode(     // { crswap(r29,sgp0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|crswap(r29, sgp0)");
        }

        [Test]
        public void HexagonRw_jump()
        {
            Given_HexString("AAFFFF59");
            AssertCode(     // { jump	00009C40 }
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFF54");
        }

        [Test]
        public void HexagonRw_jumpr()
        {
            Given_HexString("00C09C52");
            AssertCode(     // { jumpr	r28 }
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r28");
        }

        [Test]
        public void HexagonRw_store_rr()
        {
            Given_HexString("101CF4EB");
            AssertCode(     // { allocframe(00000008); memd(r29+496) = r17:r16 }
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v2 = r29 - 8<i32>",
                "2|L--|Mem0[v2:word32] = r30",
                "3|L--|Mem0[v2 + 4<i32>:word32] = r31",
                "4|L--|r30 = v2", 
                "5|L--|r29 = v2 - 8<i32>",
                "6|L--|Mem0[r29 + 496<i32>:word64] = r17_r16");
        }

        [Test]
        public void HexagonRw_load()
        {
            Given_HexString("051E0C3E");
            AssertCode(  // { r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r19_r18 = Mem0[r29:word64]",
                "2|L--|r17_r16 = Mem0[r29 + 8<i32>:word64]");
        }

        [Test]
        public void HexagonRw_memw_locked()
        {
            Given_HexString("01C00092");
            AssertCode(     // { r1 = memw_locked(r0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = memw_locked(r0)");
        }

        //[Test]
        public void HexagonRw_Read_Write_register_pair()
        {
            Given_HexString("104001F5 301CF4EB");
            AssertCode(     // { allocframe(00000018); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
                "0|L--|00100000(8): 7 instructions",
                "1|L--|v@@@");
        }

        [Test]
        public void HexagonRw_dfclass()
        {
            Given_HexString("50C080DC");
            AssertCode(     // { p0 = dfclass(r1:r0,00000002) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|p0 = dfclass(r1_r0, 2<32>)");
        }
    }
}
