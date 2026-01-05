#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp7
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class Pdp7RewriterTests : RewriterTestBase
    {
        private readonly Pdp7Architecture arch;
        private readonly Address addr;

        public Pdp7RewriterTests()
        {
            this.arch = new Pdp7Architecture(CreateServiceContainer(), "pdp7", new Dictionary<string, object>());
            this.addr = Pdp10Architecture.Ptr18(0x0010_000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void Given_OctalWord(string octalWord)
        {
            var word = Pdp7Architecture.OctalStringToWord(octalWord);
            Given_MemoryArea(new Word18MemoryArea(addr, new[] { word }));
        }

        [Test]
        public void Pdp7Rw_add()
        {
            Given_OctalWord("301234");
            AssertCode(        // add\t1234
                "0|L--|200000(1): 6 instructions",
                "1|L--|v4 = CONVERT(ac, word18, word36)",
                "2|L--|v5 = CONVERT(Mem0[0o001234<p18>:word18], word18, word36)",
                "3|L--|v6 = v4 + v5",
                "4|L--|ac = SLICE(v6, word18, 0)",
                "5|L--|ac = ac + SLICE(v6, word18, 18)",
                "6|L--|l = cond(ac)");
        }

        [Test]
        public void Pdp7Rw_and()
        {
            Given_OctalWord("501234");
            AssertCode(        // and\t1234
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = Mem0[0o001234<p18>:word18]",
                "2|L--|ac = ac & v4");
        }

        [Test]
        public void Pdp7Rw_cal()
        {
            Given_OctalWord("000000");
            AssertCode(        // cal
                "0|T--|200000(1): 1 instructions",
                "1|T--|call 000020 (0)");
        }

        [Test]
        public void Pdp7Rw_cla()
        {
            Given_OctalWord("750000");
            AssertCode(        // cla
                "0|L--|200000(1): 1 instructions",
                "1|L--|ac = 0<18>");
        }

        [Test]
        public void Pdp7Rw_cll()
        {
            Given_OctalWord("744000");
            AssertCode(        // cll
                "0|L--|200000(1): 1 instructions",
                "1|L--|l = 0<18>");
        }

        [Test]
        public void Pdp7Rw_cma()
        {
            Given_OctalWord("740001");
            AssertCode(        // cma
                "0|L--|200000(1): 1 instructions",
                "1|L--|ac = ~ac");
        }

        [Test]
        public void Pdp7Rw_cml()
        {
            Given_OctalWord("740002");
            AssertCode(        // cml
                "0|L--|200000(1): 1 instructions",
                "1|L--|l = !l");
        }

        [Test]
        public void Pdp7Rw_dac()
        {
            Given_OctalWord("065234");
            AssertCode(        // dac\ti 5234
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[Mem0[0o005234<p18>:ptr18]:ptr18] = ac");
        }

        [Test]
        public void Pdp7Rw_dzm()
        {
            Given_OctalWord("165234");
            AssertCode(        // dzm\ti 5234
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[Mem0[0o005234<p18>:ptr18]:ptr18] = 0<18>");
        }

        [Test]
        public void Pdp7Rw_jmp()
        {
            Given_OctalWord("605234");
            AssertCode(        // jmp\t5234
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 005234");
        }

        [Test]
        public void Pdp7Rw_hlt()
        {
            Given_OctalWord("740040");
            AssertCode(        // hlt
                "0|H--|200000(1): 1 instructions",
                "1|H--|__halt()");
        }

        [Test]
        public void Pdp7Rw_jms()
        {
            Given_OctalWord("105234");
            AssertCode(        // jms\t5234
                "0|T--|200000(1): 1 instructions",
                "1|T--|call 005234 (0)");
        }

        [Test]
        public void Pdp7Rw_isz()
        {
            Given_OctalWord("445234");
            AssertCode(        // isz\t5234
                "0|L--|200000(1): 4 instructions",
                "1|L--|v3 = Mem0[0o005234<p18>:word18]",
                "2|L--|v3 = v3 + 1<18>",
                "3|L--|Mem0[0o005234<p18>:word18] = v3",
                "4|T--|if (v3 == 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_lac()
        {
            Given_OctalWord("201234");
            AssertCode(        // lac\t1234
                "0|L--|200000(1): 1 instructions",
                "1|L--|ac = Mem0[0o001234<p18>:word18]");
        }

        [Test]
        public void Pdp7Rw_lac_indirect()
        {
            Given_OctalWord("221234");
            AssertCode(        // lac\t1234
                "0|L--|200000(1): 1 instructions",
                "1|L--|ac = Mem0[Mem0[0o001234<p18>:ptr18]:ptr18]");
        }

        [Test]
        public void Pdp7Rw_oas()
        {
            Given_OctalWord("740004");
            AssertCode(        // oas
                "0|L--|200000(1): 2 instructions",
                "1|L--|v3 = __read_ac_switches()",
                "2|L--|ac = ac | v3");
        }

        [Test]
        public void Pdp7Rw_opr()
        {
            Given_OctalWord("740000");
            AssertCode(        // opr
                "0|L--|200000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Pdp7Rw_ral()
        {
            Given_OctalWord("740010");
            AssertCode(        // ral
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = l",
                "2|L--|l = (ac & 1<18>) != 0<18>",
                "3|L--|ac = __rcl<word18,word18>(ac, ac, v4)");
        }

        [Test]
        public void Pdp7Rw_rar()
        {
            Given_OctalWord("740020");
            AssertCode(        // rar
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = l",
                "2|L--|l = (ac & 1<18>) != 0<18>",
                "3|L--|ac = __rcr<word18,word18>(ac, ac, v4)");
        }

        [Test]
        public void Pdp7Rw_rtl()
        {
            Given_OctalWord("742010");
            AssertCode(        // rtl
                "0|L--|200000(1): 6 instructions",
                "1|L--|v4 = l",
                "2|L--|l = (ac & 1<18>) != 0<18>",
                "3|L--|ac = __rcl<word18,word18>(ac, ac, v4)",
                "4|L--|v6 = l",
                "5|L--|l = (ac & 1<18>) != 0<18>",
                "6|L--|ac = __rcl<word18,word18>(ac, ac, v6)");
        }

        [Test]
        public void Pdp7Rw_rtr()
        {
            Given_OctalWord("742020");
            AssertCode(        // rtr
                "0|L--|200000(1): 6 instructions",
                "1|L--|v4 = l",
                "2|L--|l = (ac & 1<18>) != 0<18>",
                "3|L--|ac = __rcr<word18,word18>(ac, ac, v4)",
                "4|L--|v6 = l",
                "5|L--|l = (ac & 1<18>) != 0<18>",
                "6|L--|ac = __rcr<word18,word18>(ac, ac, v6)");
        }

        [Test]
        public void Pdp7Rw_sad()
        {
            Given_OctalWord("565234");
            AssertCode(        // sad\ti 5234
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (ac != Mem0[Mem0[0o005234<p18>:ptr18]:ptr18]) branch 200002");
        }

        [Test]
        public void Pdp7Rw_sma()
        {
            Given_OctalWord("740100");
            AssertCode(        // sma
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (ac < 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_sna()
        {
            Given_OctalWord("741200");
            AssertCode(        // sna
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (ac != 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_snl()
        {
            Given_OctalWord("740400");
            AssertCode(        // snl
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (l != 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_spa()
        {
            Given_OctalWord("741100");
            AssertCode(        // spa
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (ac > 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_sza()
        {
            Given_OctalWord("740200");
            AssertCode(        // sza
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (ac == 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_szl()
        {
            Given_OctalWord("741400");
            AssertCode(        // szl
                "0|L--|200000(1): 1 instructions",
                "1|T--|if (l == 0<18>) branch 200002");
        }

        [Test]
        public void Pdp7Rw_tad()
        {
            Given_OctalWord("365234");
            AssertCode(        // tad\ti 5234
                "0|L--|200000(1): 2 instructions",
                "1|L--|ac = ac + Mem0[Mem0[0o005234<p18>:ptr18]:ptr18]",
                "2|L--|l = cond(ac)");
        }

        [Test]
        public void Pdp7Rw_xct()
        {
            Given_OctalWord("425234");
            AssertCode(        // xct\ti 5234
                "0|L--|200000(1): 1 instructions",
                "1|L--|__execute(Mem0[0o005234<p18>:ptr18])");
        }

        [Test]
        public void Pdp7Rw_xor()
        {
            Given_OctalWord("265234");
            AssertCode(        // xor\ti 5234
                "0|L--|200000(1): 1 instructions",
                "1|L--|ac = ac ^ Mem0[Mem0[0o005234<p18>:ptr18]:ptr18]");
        }

    }
}
