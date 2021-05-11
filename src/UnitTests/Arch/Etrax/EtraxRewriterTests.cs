#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.Etrax;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.Etrax
{
    [TestFixture]
    public class EtraxRewriterTests : RewriterTestBase
    {
        private readonly EtraxArchitecture arch;
        private readonly Address addrLoad;

        public EtraxRewriterTests()
        {
            this.arch = new EtraxArchitecture(CreateServiceContainer(), "etrax", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var rdr = mem.CreateLeReader(mem.BaseAddress);
            return arch.CreateRewriter(rdr, arch.CreateProcessorState(), binder, host);
        }

        [Test]
        public void EtraxRw_add()
        {
            Given_HexString("2F1E00000100");
            AssertCode(     // add.d    0x00010000,r1
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = r1 + 0x10000<32>",
                "2|L--|NZVC = cond(r1)");
        }

        [Test]
        public void EtraxRw_addq()
        {
            Given_HexString("0A32");
            AssertCode(     // addq     0x0000000A,r3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 + 0xA<32>",
                "2|L--|NZVC = cond(r3)");
        }

        [Test]
        public void EtraxRw_and_d()
        {
            Given_HexString("2027");
            AssertCode(     // and.d    r0,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r2 = r2 & r0",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void EtraxRw_ba()
        {
            Given_HexString("0CE0");
            AssertCode(     // ba       000000F2
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|goto 0010000E");
        }

        [Test]
        public void EtraxRw_beq()
        {
            Given_HexString("0830");
            AssertCode(     // beq      00000022
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|if (Test(EQ,Z)) branch 0010000A");
        }

        [Test]
        public void EtraxRw_bne()
        {
            Given_HexString("6620");
            AssertCode(     // bne      00000118
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|if (Test(NE,Z)) branch 00100068");
        }

        [Test]
        public void EtraxRw_bpl()
        {
            Given_HexString("0860");
            AssertCode(     // bpl      0000015E
                "0|TD-|00100000(2): 1 instructions",
                "1|TD-|if (Test(GE,N)) branch 0010000A");
        }

        [Test]
        public void EtraxRw_btstq()
        {
            Given_HexString("9013");
            AssertCode(     // btstq    0x+00000010,r1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|N = r1 & 1<u32> << 16<i32>",
                "2|L--|Z = r1 & (1<u32> << 16<i32> + 1<32>) - 1<32>");
        }

        [Test]
        public void EtraxRw_clearf()
        {
            Given_HexString("F025");
            AssertCode(     // clearf   I
                "0|L--|00100000(2): 1 instructions",
                "1|L--|I = false");
        }

        [Test]
        public void EtraxRw_cmp()
        {
            Given_HexString("EF0EFFFFFFFF");
            AssertCode(     // cmp.d    0xFFFFFFFF,r0
                "0|L--|00100000(6): 1 instructions",
                "1|L--|NZVC = cond(r0 - 0xFFFFFFFF<32>)");
        }

        [Test]
        public void EtraxRw_jsr()
        {
            Given_HexString("3FBDAC010000");
            AssertCode(     // jsr      00000278
                "0|T--|00100000(6): 1 instructions",
                "1|T--|call 001001AE (0)");
        }

        [Test]
        public void EtraxRw_jump()
        {
            Given_HexString("3F0D0A000000");
            AssertCode(     // jump     00000010
                "0|T--|00100000(6): 1 instructions",
                "1|T--|goto 0010000C");
        }

        [Test]
        public void EtraxRw_move()
        {
            Given_HexString("600A");
            AssertCode(     // move.d   [r0],r0
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r0 = Mem0[r0:word32]",
                "2|L--|NZ = cond(r0)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void EtraxRw_moveq()
        {
            Given_HexString("4002");
            AssertCode(     // moveq    0x00000000,r0
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r0 = 0<32>",
                "2|L--|NZ = cond(r0)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void EtraxRw_movu()
        {
            Given_HexString("532C");
            AssertCode(     // movu.w   [r3+],r2
                "0|L--|00100000(2): 7 instructions",
                "1|L--|v3 = Mem0[r3:uint16]",
                "2|L--|r3 = r3 + 2<i32>",
                "3|L--|r2 = CONVERT(v3, uint16, uint32)",
                "4|L--|Z = cond(r2)",
                "5|L--|N = false",
                "6|L--|V = false",
                "7|L--|C = false");
        }

        [Test]
        public void EtraxRw_nop()
        {
            Given_HexString("0F05");
            AssertCode(     // nop
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void EtraxRw_not()
        {
            Given_HexString("7087");
            AssertCode(     // not      r0
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r0 = !r0",
                "2|L--|NZ = cond(r0)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void EtraxRw_or()
        {
            Given_HexString("6027");
            AssertCode(     // or.d     r0,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r2 = r2 | r0",
                "2|L--|NZ = cond(r2)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void EtraxRw_sub()
        {
            Given_HexString("A826");
            AssertCode(     // sub.d    r8,r2
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r2 = r2 - r8",
                "2|L--|NZVC = cond(r2)");
        }

        [Test]
        public void EtraxRw_subq()
        {
            Given_HexString("8142");
            AssertCode(     // subq     0x00000001,r4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r4 = r4 - 1<32>",
                "2|L--|NZVC = cond(r4)");
        }
    }
}
