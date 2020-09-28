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

using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Rtl;
using NUnit.Framework;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Z80
{
    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        private readonly Z80ProcessorArchitecture arch = new Z80ProcessorArchitecture("z80");
        private readonly Address baseAddr = Address.Ptr16(0x0100);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = (Z80ProcessorState)arch.CreateProcessorState();
            return new Z80Rewriter(arch, new LeImageReader(mem, 0), state, new Frame(arch.WordWidth), host);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Z80rw_lxi()
        {
            Given_Bytes(0x21, 0x34, 0x12);
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|hl = 0x1234");
        }

        [Test]
        public void Z80rw_mov_a_hl()
        {
            Given_Bytes(0x7E);
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|a = Mem0[hl:byte]");
        }

        [Test]
        public void Z80rw_mov_a_ix()
        {
            Given_Bytes(0xDD, 0x7E, 0x3);
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|a = Mem0[ix + 0x0003:byte]");
        }

        [Test]
        public void Z80rw_jp()
        {
            Given_Bytes(0xC3, 0xAA, 0xBB);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto BBAA");
        }

        [Test]
        public void Z80rw_jp_nz()
        {
            Given_Bytes(0xC2, 0xAA, 0xBB);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch BBAA");
        }

        [Test]
        public void Z80rw_stx_b_d()
        {
            Given_Bytes(0xDD, 0x71, 0x80);
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|Mem0[ix - 0x0080:byte] = c");
        }

        [Test]
        public void Z80rw_push_hl()
        {
            Given_Bytes(0xE5);
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[sp:word16] = hl");
        }

        [Test]
        public void Z80rw_add_a_R()
        {
            Given_Bytes(0x83);
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a + e",
                "2|L--|SZPC = cond(a)");
        }

        [Test]
        public void Z80rw_djnz()
        {
            Given_Bytes(0x10, 0xFE);
            AssertCode(
                "0|T--|0100(2): 2 instructions",
                "1|L--|b = b - 0x01",
                "2|T--|if (b != 0x00) branch 0100");
        }

        [Test]
        public void Z80rw_jc()
        {
            Given_Bytes(0xC2, 0xFE, 0xCA);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch CAFE");
        }

        [Test]
        public void Z80rw_ldir()
        {
            Given_Bytes(0xED, 0xB0);
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|Mem0[de:byte] = Mem0[hl:byte]",
                "2|L--|hl = hl + 1",
                "3|L--|de = de + 1",
                "4|L--|bc = bc - 0x0001",
                "5|T--|if (bc != 0x0000) branch 0100",
                "6|L--|P = false");
        }

        [Test]
        public void Z80rw_call()
        {
            Given_Bytes(0xCD, 0xFE, 0xCA);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|call CAFE (2)");
            Given_Bytes(0xCD, 0xFE, 0xCA);
        }

        [Test]
        public void Z80rw_call_Cond()
        {
            Given_Bytes(0xC4, 0xFE, 0xCA);
            AssertCode(
                "0|T--|0100(3): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0103",
                "2|T--|call CAFE (2)");
        }

        [Test]
        public void Z80rw_cp_ix_d()
        {
            Given_Bytes(0xDD, 0xBE, 0x08);
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|SZPC = cond(a - Mem0[ix + 0x0008:byte])");
        }

        [Test]
        public void Z80rw_cpl()
        {
            Given_Bytes(0x2F);
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|a = ~a");
        }

        [Test]
        public void Z80rw_neg()
        {
            Given_Bytes(0xED, 0x44);
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|a = -a",
                "2|L--|SZPC = cond(a)");
        }

        [Test]
        public void Z80rw_jr()
        {
            Given_Bytes(0x18, 0x03);
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 0105");
        }

        [Test]
        public void Z80rw_ret()
        {
            Given_Bytes(0xC9);
            AssertCode(
                "0|T--|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void Z80rw_ex_de_hl()
        {
            Given_Bytes(0xEB);
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|v2 = de",
                "2|L--|de = hl",
                "3|L--|hl = v2");
        }

        [Test]
        public void Z80rw_jp_hl()
        {
            Given_Bytes(0xE9);
            AssertCode(
                "0|T--|0100(1): 1 instructions",
                "1|T--|goto hl");
        }

        [Test]
        public void Z80rw_sla()
        {
            Given_Bytes(0xCB, 0x27);
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|a = a << 0x01",
                "2|L--|SZPC = cond(a)");
        }

        [Test]
        public void Z80rw_inir()
        {
            Given_Bytes(0xED, 0xB2);
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|Mem0[hl:byte] = __in(c)",
                "2|L--|hl = hl + 1",
                "3|L--|b = b - 0x01",
                "4|L--|Z = cond(b)",
                "5|T--|if (b != 0x00) branch 0100");
        }

        [Test]
        public void Z80rw_cpdr()
        {
            Given_Bytes(0xED, 0xB9);
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|Z = cond(a - Mem0[hl:byte])",
                "2|L--|hl = hl - 1",
                "3|L--|bc = bc - 1",
                "4|T--|if (bc == 0x0000) branch 0102",
                "5|T--|if (Test(NE,Z)) branch 0100");
        }

        [Test]
        public void Z80rw_rla()
        {
            Given_Bytes(0x17);
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = __rcl(a, 0x01, C)",
                "2|L--|C = cond(a)");
        }
    }
}
