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
using Reko.Arch.C166;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Arch.C166
{
    [TestFixture]
    public class C166RewriterTests : RewriterTestBase
    {
        private readonly C166Architecture arch;
        private readonly Address addrLoad;

        public C166RewriterTests()
        {
            this.arch = new C166Architecture(CreateServiceContainer(), "c166", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        [Test]
        public void C166Rw_add()
        {
            Given_HexString("06F10100");
            AssertCode(     // add	r1,0001
                "0|L--|0100(4): 2 instructions",
                "1|L--|r1 = r1 + 1<16>",
                "2|L--|EZVCN = cond(r1)");
        }

        [Test]
        public void C166Rw_and()
        {
            Given_HexString("66F0DF00");
            AssertCode(     // and	r0,00DF
                "0|L--|0100(4): 4 instructions",
                "1|L--|r0 = r0 & 0xDF<16>",
                "2|L--|EZN = cond(r0)",
                "3|L--|V = false",
                "4|L--|C = false");
        }

        [Test]
        public void C166Rw_bclr()
        {
            Given_HexString("FED6");
            AssertCode(     // bclr	TFR:15
                "0|L--|0100(2): 6 instructions",
                "1|L--|N = __bit(TFR, 15<i16>)",
                "2|L--|Z = ~N",
                "3|L--|TFR = __bit_clear(TFR, 15<i16>)",
                "4|L--|E = false",
                "5|L--|V = false",
                "6|L--|C = false");
        }

        [Test]
        public void C166Rw_bset()
        {
            Given_HexString("DFE2");
            AssertCode(     // bset	P3:13
                "0|L--|0100(2): 6 instructions",
                "1|L--|N = __bit(P3, 13<i16>)",
                "2|L--|Z = ~N",
                "3|L--|P3 = __bit_set(P3, 13<i16>)",
                "4|L--|E = false",
                "5|L--|V = false",
                "6|L--|C = false");
        }

        [Test]
        public void C166Rw_calla()
        {
            Given_HexString("CA002801");
            AssertCode(     // calla	cc_UC,0128
                "0|T--|0100(4): 1 instructions",
                "1|T--|call 0128 (2)");
        }

        [Test]
        public void C166Rw_cmp()
        {
            Given_HexString("46F04400");
            AssertCode(     // cmp	r0,0044
                "0|L--|0100(4): 1 instructions",
                "1|L--|EZVCN = cond(r0 - 0x44<16>)");
        }

        [Test]
        public void C166Rw_cmpb()
        {
            Given_HexString("47F00000");
            AssertCode(     // cmpb	rl0,00
                "0|L--|0100(4): 1 instructions",
                "1|L--|EZVCN = cond(rl0 - 0<8>)");
        }

        [Test]
        public void C166Rw_cmpd1()
        {
            Given_HexString("A012");
            AssertCode(     // cmpd1	r2,0001
                "0|L--|0100(2): 2 instructions",
                "1|L--|EZVCN = cond(r2 - 1<16>)",
                "2|L--|r2 = r2 - 1<i16>");
        }

        [Test]
        public void C166Rw_diswdt()
        {
            Given_HexString("A55AA5A5");
            AssertCode(     // diswdt
                "0|S--|0100(4): 1 instructions",
                "1|L--|__disable_watchdog_timer()");
        }

        [Test]
        public void C166Rw_einit()
        {
            Given_HexString("B54AB5B5");
            AssertCode(     // einit
                "0|S--|0100(4): 1 instructions",
                "1|L--|__end_of_initialization()");
        }

        [Test]
        public void C166Rw_jmpa()
        {
            Given_HexString("EA203C01");
            AssertCode(     // jmpa	cc_Z,013C
                "0|T--|0100(4): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 013C");
        }

        [Test]
        public void C166Rw_jmpr()
        {
            Given_HexString("2D0C");
            AssertCode(     // jmpr	cc_Z,0098
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 011A");
        }

        [Test]
        public void C166Rw_jmps()
        {
            Given_HexString("FA001000");
            AssertCode(     // jmps	00,0010
                "0|T--|0100(4): 1 instructions",
                "1|T--|goto 0000:0010");
        }

        [Test]
        public void C166Rw_jnb()
        {
            Given_HexString("9AB7FE70");
            AssertCode(     // jnb	S0RIC:7,011C
                "0|T--|0100(4): 1 instructions",
                "1|T--|if (!__bit(S0RIC, 7<i16>)) branch 0100");
        }

        [Test]
        public void C166Rw_nop()
        {
            Given_HexString("CC00");
            AssertCode(     // nop
                "0|L--|0100(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void C166Rw_mov_mem_imm()
        {
            Given_HexString("E6890480");
            AssertCode(     // mov      [0xFF12],8004
                "0|L--|0100(4): 2 instructions",
                "1|L--|Mem0[0xFF12<p16>:word16] = 0x8004<16>",
                "2|L--|EZN = cond(0x8004<16>)");
        }

        [Test]
        public void C166Rw_movb_reg_mem()
        {
            Given_HexString("A901");
            AssertCode(     // movb	rl0,[r1]
                "0|L--|0100(2): 2 instructions",
                "1|L--|rl0 = Mem0[r1:byte]",
                "2|L--|EZN = cond(rl0)");
        }

        [Test]
        public void C166Rw_push()
        {
            Given_HexString("ECF0");
            AssertCode(     // push	r0
                "0|L--|0100(2): 4 instructions",
                "1|L--|v3 = r0",
                "2|L--|SP = SP - 2<i16>",
                "3|L--|Mem0[SP:word16] = v3",
                "4|L--|EZN = cond(v3)");
        }

        [Test]
        public void C166Rw_ret()
        {
            Given_HexString("CB00");
            AssertCode(     // ret
                "0|R--|0100(2): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void C166Rw_reti()
        {
            Given_HexString("FB88");
            AssertCode(     // reti
                "0|R--|0100(2): 2 instructions",
                "1|L--|PSW = Mem0[SP + 2<i16>:word16]",
                "2|R--|return (2,2)");
        }
    }
}
