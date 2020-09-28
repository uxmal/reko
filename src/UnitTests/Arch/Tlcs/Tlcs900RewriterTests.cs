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
using Reko.Arch.Tlcs;
using Reko.Arch.Tlcs.Tlcs900;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class Tlcs900RewriterTests : RewriterTestBase
    {
        private Tlcs900Architecture arch = new Tlcs900Architecture("tlcs900");
        private Address baseAddr = Address.Ptr32(0x0010000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = (Tlcs900ProcessorState)arch.CreateProcessorState();
            return new Tlcs900Rewriter(arch, new LeImageReader(mem, 0), state, binder, host);
        }


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Tlcs900_rw_ld()
        {
            Given_HexString("9F1621");
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|v3 = Mem0[xsp + 22:word16]",
                "2|L--|bc = v3");
        }

        [Test]
        public void Tlcs900_rw_add()
        {
            Given_HexString("E9C8FFFFFFFF");
            AssertCode(
                "0|L--|00010000(6): 3 instructions",
                "1|L--|xbc = xbc + 0xFFFFFFFF",
                "2|L--|N = false",
                "3|L--|SZHVC = cond(xbc)");
        }

        [Test]
        public void Tlcs900_rw_inc_predec()
        {
            Given_HexString("E40961"); // inc\t00000001,(-xde)
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|xde = xde - 0x00000004",
                "2|L--|v3 = Mem0[xde:word32] + 0x00000001",
                "3|L--|Mem0[xde:word32] = v3",
                "4|L--|N = false",
                "5|L--|SZHV = cond(v3)");
        }

        [Test]
        public void Tlcs900_rw_sub_postinc()
        {
            Given_HexString("E509A8"); // sub\t,(xde+),xwa
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|v4 = Mem0[xde:word32] - xwa",
                "2|L--|Mem0[xde:word32] = v4",
                "3|L--|xde = xde + 0x00000004",
                "4|L--|N = true",
                "5|L--|SZHVC = cond(v4)");
        }

        [Test]
        public void Tlcs900_rw_jp_cc()
        {
            //$REVIEW: not sure if I agree here. Shouldn't this be
            // simply if (Test(GE,SV) goto xwa?
            Given_HexString("B0D9"); // jp\tGE,(xwa)
            AssertCode(
                "0|T--|00010000(2): 3 instructions",
                "1|L--|v4 = Mem0[xwa:word32]",
                "2|T--|if (Test(LT,SV)) branch 00010002",
                "3|T--|goto v4");
        }

        [Test]
        public void Tlcs900_rw_call()
        {
            Given_HexString("1D563412"); // call\t123456
            AssertCode(
                "0|T--|00010000(4): 1 instructions",
                "1|T--|call 00123456 (4)");
        }

        [Test]
        public void Tlcs900_rw_djnz()
        {
            Given_HexString("D91CED");      // djnz\tbc,0000FFF0
            AssertCode(
                "0|T--|00010000(3): 2 instructions",
                "1|L--|bc = bc - 0x0001",
                "2|T--|if (bc != 0x0000) branch 0000FFF0");
        }

        [Test]
        public void Tlcs900_rw_daa()
        {
            Given_HexString("CA10");    // daa\tb
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|b = __daa(b)",
                "2|L--|SZHVC = cond(b)");
        }

        [Test]
        public void Tlcs900_rw_calr()
        {
            Given_HexString("1E8005");    // calr 10583
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|call 00010583 (4)");
        }

        [Test]
        public void Tlcs900_rw_cp()
        {
            Given_HexString("C1916F3F00"); // cp (00006F91),00
            AssertCode(
                "0|L--|00010000(5): 3 instructions",
                "1|L--|v2 = Mem0[0x00006F91:byte]",
                "2|L--|N = true",
                "3|L--|SZHVC = cond(v2 - 0x00)");
        }

        [Test]
        public void Tlcs900_rw_jr()
        {
            Given_HexString("6E09");	// jr	NZ,0020061C
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0001000B");
        }

        [Test]
        public void Tlcs900_rw_set()
        {
            Given_HexString("F1866FBE");	// set	06,(00006F86)
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v2 = Mem0[0x00006F86:byte] | 1 << 0x06",
                "2|L--|Mem0[0x00006F86:byte] = v2");
        }

        [Test]
        public void Tlcs900_rw_res()
        {
            Given_HexString("F1836FB3");	// res	03,(00006F83)
            AssertCode(
                "0|L--|00010000(4): 2 instructions",
                "1|L--|v2 = Mem0[0x00006F83:byte] & ~(1 << 0x03)",
                "2|L--|Mem0[0x00006F83:byte] = v2");
        }

        [Test]
        public void Tlcs900_rw_ret()
        {
            Given_HexString("0E");	// ret
            AssertCode(
                "0|T--|00010000(1): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void Tlcs900_rw_lda()
        {
            Given_HexString("F240002034");	// lda	xix,(00200040)
            AssertCode(
                "0|L--|00010000(5): 1 instructions",
                "1|L--|xix = 00200040");
        }

        [Test]
        public void Tlcs900_rw_ldir()
        {
            Given_HexString("8311");    // ldirw
            AssertCode(
                "0|L--|00010000(2): 9 instructions",
                "1|L--|v2 = Mem0[xhl:byte]",
                "2|L--|Mem0[xde:byte] = v2",
                "3|L--|xhl = xhl + 1",
                "4|L--|xde = xde + 1",
                "5|L--|bc = bc - 1",
                "6|T--|if (bc != 0x0000) branch 00010000",
                "7|L--|H = false",
                "8|L--|V = false",
                "9|L--|N = false");
        }

        [Test]
        public void Tlcs900_rw_ldirw()
        {
            Given_HexString("9311");	// ldirw
            AssertCode(
                "0|L--|00010000(2): 9 instructions",
                "1|L--|v2 = Mem0[xhl:word16]",
                "2|L--|Mem0[xde:word16] = v2",
                "3|L--|xhl = xhl + 2",
                "4|L--|xde = xde + 2",
                "5|L--|bc = bc - 1",
                "6|T--|if (bc != 0x0000) branch 00010000",
                "7|L--|H = false",
                "8|L--|V = false",
                "9|L--|N = false");
        }

        [Test]
        public void Tlcs900_rw_ei()
        {
            Given_HexString("0600");	// ei	00
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|__ei(0x00)");
        }

        [Test]
        public void Tlcs900_rw_push()
        {
            Given_HexString("38");	// push	xwa
            AssertCode(
                "0|L--|00010000(1): 2 instructions",
                "1|L--|xsp = xsp - 4",
                "2|L--|Mem0[xsp:word32] = xwa");
        }

        [Test]
        public void Tlcs900_rw_sll()
        {
            Given_HexString("CCEE01");	// sll	01,d
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|d = d << 0x01",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZVC = cond(d)");
        }

        [Test]
        public void Tlcs900_rw_pop()
        {
            Given_HexString("58");	// pop	xwa
            AssertCode(
                "0|L--|00010000(1): 2 instructions",
                "1|L--|xwa = Mem0[xsp:word32]",
                "2|L--|xsp = xsp + 4");
        }

        [Test]
        public void Tlcs900_rw_and()
        {
            Given_HexString("C9CCF0");	// and	a,F0
            AssertCode(
                "0|L--|00010000(3): 5 instructions",
                "1|L--|a = a & 0xF0",
                "2|L--|H = true",
                "3|L--|N = false",
                "4|L--|C = false",
                "5|L--|SZV = cond(a)");
        }

        [Test]
        public void Tlcs900_rw_mul()
        {
            Given_HexString("D9084000");	// mul	bc,0040
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|xbc = bc *u 0x0040");
        }

        [Test]
        public void Tlcs900_rw_srl()
        {
            Given_HexString("C9EF04");	// srl	04,a
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|a = a >>u 0x04",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZVC = cond(a)");
        }

        [Test]
        public void Tlcs900_rw_dec()
        {
            Given_HexString("C869");	// dec	01,w
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|w = w - 0x01",
                "2|L--|N = true",
                "3|L--|SZHV = cond(w)");
        }

        [Test]
        public void Tlcs900_rw_bit()
        {
            Given_HexString("C93302");	// bit	02,a
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|Z = (a & 1 << 0x02) == 0x00",
                "2|L--|H = true",
                "3|L--|N = false");
        }

        [Test]
        public void Tlcs900_rw_div()
        {
            Given_HexString("D90A0A00");	// div	bc,000A
            AssertCode(
                "0|L--|00010000(4): 4 instructions",
                "1|L--|v3 = xbc",
                "2|L--|c = v3 /u 0x000A",
                "3|L--|b = v3 % 0x000A",
                "4|L--|V = cond(c)");
        }

        [Test]
        public void Tlcs900_rw_rcf()
        {
            Given_HexString("10");	// rcf
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|C = false");
        }

        [Test]
        public void Tlcs900_rw_scf()
        {
            Given_HexString("11");	// scf
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|C = true");
        }

        [Test]
        public void Tlcs900_rw_or()
        {
            Given_HexString("CAE0");	// or	w,b
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|w = w | b",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|C = false",
                "5|L--|SZV = cond(w)");
        }

        [Test]
        public void Tlcs900_rw_push_a()
        {
            Given_HexString("14");	// push a
            AssertCode(
                "0|L--|00010000(1): 2 instructions",
                "1|L--|xsp = xsp - 1",
                "2|L--|Mem0[xsp:byte] = a");
        }

        [Test]
        public void Tlcs900_rw_ld_r3()
        {
            Given_HexString("D7E6A8");
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|bc = 0x0000");
        }

        [Test]
        public void Tlcs900_rw_halt()
        {
            Given_HexString("05");
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|__halt()");
        }

        [Test]
        public void Tlcs900_rw_reti()
        {
            Given_HexString("07"); // reti
            AssertCode(
                "0|T--|00010000(1): 3 instructions",
                "1|L--|sr = Mem0[xsp:word16]",
                "2|L--|xsp = xsp + 2",
                "3|T--|return (4,0)");
        }

        [Test]
        public void Tlcs900_rw_ccf()
        {
            Given_HexString("12"); // ccf
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|C = !C");
        }

        [Test]
        public void Tlcs900_rw_swi()
        {
            Given_HexString("FA"); // swi
            AssertCode(
                "0|T--|00010000(1): 3 instructions",
                "1|L--|xsp = xsp - 2",
                "2|L--|Mem0[xsp:word16] = sr",
                "3|T--|call 00FFFF08 (4)");
        }

        [Test]
        public void Tlcs900_rw_decf()
        {
            Given_HexString("0D"); // decf
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|__decf()");
        }

        [Test]
        public void Tlcs900_rw_ex()
        {
            Given_HexString("D8B9"); // ex
            AssertCode(
                "0|L--|00010000(2): 3 instructions",
                "1|L--|v4 = bc",
                "2|L--|bc = wa",
                "3|L--|wa = v4");
        }

        [Test]
        public void Tlcs900_rw_retd()
        {
            Given_HexString("0F0400"); // retd
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|return (4,4)");
        }

        [Test]
        public void Tlcs900_rw_ldf()
        {
            Given_HexString("1703"); // ldf
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|__ldf(0x03)");
        }

        [Test]
        public void Tlcs900_rw_incf()
        {
            Given_HexString("0C"); // incf
            AssertCode(
                "0|L--|00010000(1): 1 instructions",
                "1|L--|__incf()");
        }

        [Test]
        public void Tlcs900_rw_bs1b()
        {
            Given_HexString("D80F"); // bs1b
            AssertCode(
                "0|L--|00010000(2): 2 instructions",
                "1|L--|a = __bs1b(wa)",
                "2|L--|V = wa == 0x0000");
        }

        [Test]
        public void Tlcs900_rw_zcf()
        {
            Given_HexString("13"); // zcf
            AssertCode(
                "0|L--|00010000(1): 2 instructions",
                "1|L--|C = !Z",
                "2|L--|N = false");
        }

        [Test]
        public void Tlcs900_rw_divs()
        {
            Given_HexString("D85B"); // divs
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v4 = xhl",
                "2|L--|l = v4 / wa",
                "3|L--|h = v4 % wa",
                "4|L--|V = cond(l)");
        }

        [Test]
        public void Tlcs900_rw_muls()
        {
            Given_HexString("DE4A"); // muls
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|xde = de *s iz");
        }

        [Test]
        public void Tlcs900_rw_chg()
        {
            Given_HexString("DE32C6"); // chg
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|iz = iz ^ 0x0040");
        }

        [Test]
        public void Tlcs900_rw_xor()
        {
            Given_HexString("EECD12345678"); // xor
            AssertCode(
                "0|L--|00010000(6): 5 instructions",
                "1|L--|xiz = xiz ^ 0x78563412",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|C = false",
                "5|L--|SZV = cond(xiz)");
        }

        [Test]
        [Ignore("This is probably best left as an instrinsic.")]
        public void Tlcs900_rw_rrd()
        {
            Given_HexString("E1830007");	// rrd	a,(00000083)
            AssertCode(
                "0|L--|00010000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Tlcs900_rw_sll_mem()
        {
            Given_HexString("817E");	// sll	(xbc)
            AssertCode(
                "0|L--|00010000(2): 5 instructions",
                "1|L--|v3 = Mem0[xbc:byte] << 1",
                "2|L--|Mem0[xbc:byte] = v3",
                "3|L--|H = false",
                "4|L--|N = false",
                "5|L--|SZVC = cond(v3)");
        }

        [Test]
        public void Tlcs900_rw_sbc()
        {
            Given_HexString("82B2");	// sbc	b,(xde)
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|v4 = Mem0[xde:byte]",
                "2|L--|b = b - v4 - C",
                "3|L--|N = true",
                "4|L--|SZHVC = cond(b)");
        }

        [Test]
        public void Tlcs900_rw_sbc_mem()
        {
            Given_HexString("C004B1");	// sbc	a,(00000004)
            AssertCode(
                "0|L--|00010000(3): 4 instructions",
                "1|L--|v3 = Mem0[0x00000004:byte]",
                "2|L--|a = a - v3 - C",
                "3|L--|N = true",
                "4|L--|SZHVC = cond(a)");
        }

        [Test]
        public void Tlcs900_rw_ret_cc()
        {
            Given_HexString("B3F1");	// ret	LT
            AssertCode(
                "0|T--|00010000(2): 2 instructions",
                "1|T--|if (Test(GE,SV)) branch 00010002",
                "2|T--|return (4,0)");
        }

        public void Tlcs900_rw_sla()
        {
            Given_HexString("CFFC");	// sla	a,l
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Tlcs900_rw_sla_r()
        {
            Given_HexString("CCFC");	// sla	a,d
            AssertCode(
                "0|L--|00010000(2): 4 instructions",
                "1|L--|d = d << a",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZVC = cond(d)");
        }

        [Test]
        public void Tlcs900_rw_scc()
        {
            Given_HexString("DD77");	// scc	C,iy
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|iy = Test(ULT,Z)");
        }
    }
}
