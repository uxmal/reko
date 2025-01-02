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

using NUnit.Framework;
using Reko.Arch.zSeries;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.zSeries
{
#pragma warning disable IDE1006

    [TestFixture]
    public class zSeriesRewriterTests : RewriterTestBase
    {
        public zSeriesRewriterTests()
        {
            this.Architecture = new zSeriesArchitecture(CreateServiceContainer(), "zSeries", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        [Test]
        public void zSeriesRw_a()
        {
            Given_HexString("5A30D004");
            AssertCode(     // a	r3,4(r13)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r3, int32, 0)",
                "2|L--|v6 = v4 + Mem0[r13 + 4<i64>:int32]",
                "3|L--|r3 = SEQ(SLICE(r3, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_ad()
        {
            Given_HexString("6AEDA749");
            AssertCode(     // ad	f14,1865(r13,r10)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f14 = f14 + Mem0[(r10 + r13) + 1865<i64>:real64]",
                "2|L--|CC = cond(f14)");
        }

        [Test]
        public void zSeriesRw_adr()
        {
            Given_HexString("2A7E");
            AssertCode(     // adr	f7,f14
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f7 = f7 + f14",
                "2|L--|CC = cond(f7)");
        }

        [Test]
        public void zSeriesRw_ae()
        {
            Given_HexString("7AAA4330");
            AssertCode(     // ae	f10,816(r10,r4)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f10, real32, 0)",
                "2|L--|v7 = v4 + Mem0[(r4 + r10) + 816<i64>:real32]",
                "3|L--|f10 = SEQ(SLICE(f10, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_aeb()
        {
            Given_HexString("ED034000000A");
            AssertCode(     // aeb	f0,(r3,r4)
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|v7 = v4 + Mem0[r4 + r3:real32]",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_aer()
        {
            Given_HexString("3A7F");
            AssertCode(     // aer	f7,f15
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(f7, real32, 0)",
                "2|L--|v6 = SLICE(f15, real32, 0)",
                "3|L--|v7 = v4 + v6",
                "4|L--|f7 = SEQ(SLICE(f7, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_aghi()
        {
            Given_HexString("A7FBFF58");
            AssertCode(     // aghi	r15,-000000A8
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 - 168<i64>",
                "2|L--|CC = cond(r15)");
        }

        [Test]
        public void zSeriesRw_aghik()
        {
            Given_HexString("EC84FFFF00D9");
            AssertCode(     // aghik	r8,FFFF,r4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r8 = r4 - 1<i64>",
                "2|L--|CC = cond(r8)");
        }

        [Test]
        public void zSeriesRw_agr()
        {
            Given_HexString("B9080031");
            AssertCode(     // agr	r3,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + r1",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_ahi()
        {
            Given_HexString("A71AFFFF");
            AssertCode(     // ahi	r1,-00000001
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v5 = v4 - 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_ahik()
        {
            Given_HexString("EC31076C00D8");
            AssertCode(     // ahik	r3,076C,r1
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v6 = v4 + 1900<i32>",
                "3|L--|r3 = SEQ(SLICE(r3, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_al()
        {
            Given_HexString("5E9BB9E9");
            AssertCode(     // al	r9,-1559(r11,r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r9, word32, 0)",
                "2|L--|v6 = v4 + Mem0[(r11 + r11) + -1559<i64>:word32]",
                "3|L--|r9 = SEQ(SLICE(r9, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_alcgr()
        {
            Given_HexString("B9880022");
            AssertCode(     // alcgr	r2,r2
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 + r2 + CC",
                "2|L--|CC = cond(r2)");
        }

        [Test]
        public void zSeriesRw_alcr()
        {
            Given_HexString("B9980022");
            AssertCode(     // alcr	r2,r2
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v5 = SLICE(r2, word32, 0)",
                "3|L--|v7 = v4 + v5 + CC",
                "4|L--|r2 = SEQ(SLICE(r2, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_algfr()
        {
            Given_HexString("B91A0023");
            AssertCode(     // algfr	r2,r3
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r3, word32, 0)",
                "2|L--|r2 = r2 + CONVERT(v5, word32, word64)",
                "3|L--|CC = cond(r2)");
        }

        [Test]
        public void zSeriesRw_algr()
        {
            Given_HexString("B90A005B");
            AssertCode(     // algr	r5,r11
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = r5 + r11",
                "2|L--|CC = cond(r5)");
        }

        [Test]
        public void zSeriesRw_alr()
        {
            Given_HexString("1EB6");
            AssertCode(     // alr	r11,r6
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r11, word32, 0)",
                "2|L--|v6 = SLICE(r6, word32, 0)",
                "3|L--|v7 = v4 + v6",
                "4|L--|r11 = SEQ(SLICE(r11, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_ap()
        {
            Given_HexString("FAC8EC18FF15");
            AssertCode(     // ap	-1000(13,r14),-235(9,r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ar()
        {
            Given_HexString("1A12");
            AssertCode( // ar\tr1,r2
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r1, int32, 0)",
                "2|L--|v6 = SLICE(r2, int32, 0)",
                "3|L--|v7 = v4 + v6",
                "4|L--|r1 = SEQ(SLICE(r1, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_asi()
        {
            Given_HexString("EB012008006A");
            AssertCode(     // asi	8(r2),01
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = Mem0[r2 + 8<i64>:int32]",
                "2|L--|v5 = v4 + 1<i32>",
                "3|L--|Mem0[r2 + 8<i64>:int32] = v5",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_aur()
        {
            Given_HexString("3EE0");
            AssertCode(     // aur	f14,f0
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(f14, real32, 0)",
                "2|L--|v6 = SLICE(f0, real32, 0)",
                "3|L--|v7 = v4 + v6",
                "4|L--|f14 = SEQ(SLICE(f14, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_awr()
        {
            Given_HexString("2ED3");
            AssertCode(     // awr	f13,f3
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f13 = f13 + f3",
                "2|L--|CC = cond(f13)");
        }

        [Test]
        public void zSeriesRw_axr()
        {
            Given_HexString("36EC");
            AssertCode(     // axr	f14,f12
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f14_f15 = f14_f15 + f12_f13",
                "2|L--|CC = cond(f14_f15)");
        }

        [Test]
        public void zSeriesRw_b()
        {
            Given_HexString("47F12000");
            AssertCode(     // b	(r1,r2)
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r2 + r1");
        }

        [Test]
        public void zSeriesRw_bal()
        {
            Given_HexString("45101707");
            AssertCode(     // bal	r1,1799(r1)
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r1 = 00100004",
                "2|T--|call Mem0[r1 + 1799<i64>:ptr64] (0)");
        }

        [Test]
        public void zSeriesRw_balr()
        {
            Given_HexString("05CF");
            AssertCode(     // balr	r12,r15
                "0|T--|00100000(2): 2 instructions",
                "1|L--|r12 = 00100002",
                "2|T--|call r15 (0)");
        }

        [Test]
        public void zSeriesRw_bas()
        {
            Given_HexString("4D849208");
            AssertCode(     // bas	r8,520(r4,r9)
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r8 = 00100004",
                "2|T--|call Mem0[r9 + r4 + 520<i64>:ptr64] (0)");
        }

        [Test]
        public void zSeriesRw_basr()
        {
            Given_HexString("0DE1");
            AssertCode(     // basr	r14,r1
                "0|T--|00100000(2): 2 instructions",
                "1|L--|r14 = 00100002",
                "2|T--|call r1 (0)");
        }

        [Test]
        public void zSeriesRw_bassm()
        {
            Given_HexString("0C7F");
            AssertCode(     // bassm	r7,r15
                "0|T--|00100000(2): 2 instructions",
                "1|L--|r7 = 00100002",
                "2|T--|call r15 (0)");
        }

        [Test]
        public void zSeriesRw_bctr()
        {
            Given_HexString("06CB");
            AssertCode(     // bctr	r12,r11
                "0|T--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|L--|v4 = v4 - 1<32>",
                "3|T--|if (v4 == 0<32>) branch 00100002",
                "4|T--|goto r11");
        }

        [Test]
        public void zSeriesRw_ber()
        {
            Given_HexString("078E");
            AssertCode(     // ber	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_bler()
        {
            Given_HexString("07CE");
            AssertCode(     // bler	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(GT,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_blhr()
        {
            Given_HexString("0763");
            AssertCode(     // blhr	r3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }
        [Test]
        public void zSeriesRw_bner()
        {
            Given_HexString("077E");
            AssertCode(     // bner	r14
                "0|T--|00100000(2): 2 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100002",
                "2|T--|goto r14");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_bpp()
        {
            Given_HexString("C74BA7F4FFF3");
            AssertCode(     // bpp	04,0000000000001234,-2060(r10)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_bprp()
        {
            Given_HexString("C5D8C0E50000");
            AssertCode(     // bprp	0D,-00000740,-001B0000
                "0|L--|00100000(6): 1 instructions",
                "1|L--|__branch_prediction_relative_preload<word64>(0xD<8>, -1856<i32>, -1769472<i32>)");
        }

        [Test]
        public void zSeriesRw_br()
        {
            Given_HexString("07FE");
            AssertCode(     // br	r14
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r14");
        }

        [Test]
        public void zSeriesRw_brasl()
        {
            Given_HexString("C0E5FFFFFE95");
            AssertCode(     // brasl	r14,00000560
                "0|T--|00100000(6): 2 instructions",
                "1|L--|r14 = 00100006",
                "2|T--|call 000FFD2A (0)");
        }

        [Test]
        public void zSeriesRw_brctg()
        {
            Given_HexString("A7B7FFF4");
            AssertCode(     // brctg	r11,0000085A
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 - 1<i64>",
                "2|T--|if (r11 != 0<64>) branch 000FFFE8");
        }

        [Test]
        public void zSeriesRw_brxh()
        {
            Given_HexString("843AC01B");
            AssertCode(     // brxh	r3,r10,000000000000D036
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 + r10",
                "2|T--|if (r3 > r11) branch 000F8036");
        }

        [Test]
        public void zSeriesRw_brxle()
        {
            Given_HexString("857BE310");
            AssertCode(     // brxle	r7,r11,000000000001156E
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r7 = r7 + r11",
                "2|T--|if (r7 <= r11) branch 000FC620");
        }

        [Test]
        public void zSeriesRw_bsm()
        {
            Given_HexString("0B59");
            AssertCode(     // bsm	r5,r9
                "0|T--|00100000(2): 2 instructions",
                "1|L--|r5 = 00100002",
                "2|T--|goto r9");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_bxh()
        {
            Given_HexString("8654C01B");
            AssertCode(     // bxh	r5,27(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_bxle()
        {
            Given_HexString("87DEEC23");
            AssertCode(     // bxle	r13,-989(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_c()
        {
            Given_HexString("59302028");
            AssertCode(     // c	r3,40(r2)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r3, int32, 0)",
                "2|L--|CC = cond(v4 - Mem0[r2 + 40<i64>:int32])");
        }

        [Test]
        public void zSeriesRw_cd()
        {
            Given_HexString("69FCC020");
            AssertCode(     // cd	f15,32(r12,r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(f15 - Mem0[(r12 + r12) + 32<i64>:real64])");
        }

        [Test]
        public void zSeriesRw_cdb()
        {
            Given_HexString("ED0050000019");
            AssertCode(     // cdb	f0,(r5)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(f0 - Mem0[r5:real64])");
        }

        [Test]
        public void zSeriesRw_ceb()
        {
            Given_HexString("ED0050000009");
            AssertCode(     // ceb	f0,(r5)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|CC = cond(v4 - Mem0[r5:real32])");
        }

        [Test]
        public void zSeriesRw_cer()
        {
            Given_HexString("39BC");
            AssertCode(     // cer	f11,f12
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(f11, real32, 0)",
                "2|L--|v6 = SLICE(f12, real32, 0)",
                "3|L--|CC = cond(v4 - v6)");
        }

        [Test]
        public void zSeriesRw_cghi()
        {
            Given_HexString("A71FFFFF");
            AssertCode(     // cghi	r1,-00000001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - 0xFFFFFFFFFFFFFFFF<64>)");
        }

        [Test]
        public void zSeriesRw_cgij()
        {
            Given_HexString("EC18008B007C");
            AssertCode(     // cgij	r1,+00,01,0000000080000952
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r1 == 0<i64>) branch 00100116");
        }

        [Test]
        public void zSeriesRw_cgr()
        {
            Given_HexString("B9200012");
            AssertCode(     // cgr	r1,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - r2)");
        }

        [Test]
        public void zSeriesRw_cgrj()
        {
            Given_HexString("EC4300348064");
            AssertCode(     // cgrj	r4,r3,08,000000008006CCAE
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r4 == r3) branch 00100068");
        }

        [Test]
        public void zSeriesRw_ch()
        {
            Given_HexString("49A3B904");
            AssertCode(     // ch	r10,-1788(r3,r11)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r10, int32, 0)",
                "2|L--|CC = cond(v4 - CONVERT(Mem0[(r11 + r3) + -1788<i64>:int16], int16, int32))");
        }

        [Test]
        public void zSeriesRw_chi()
        {
            Given_HexString("A71E0001");
            AssertCode(     // chi	r1,+00000001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 - 1<64>)");
        }

        [Test]
        public void zSeriesRw_cij()
        {
            Given_HexString("ECC8FFFC077E");
            AssertCode(     // cij	r12,+07,08,000000008006D0A2
                "0|T--|00100000(6): 2 instructions",
                "1|L--|v4 = SLICE(r12, int32, 0)",
                "2|T--|if (v4 == 7<i32>) branch 000FFFF8");
        }

        [Test]
        public void zSeriesRw_cl()
        {
            Given_HexString("55D01008");
            AssertCode(     // cl	r13,8(r1)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r13, word32, 0)",
                "2|L--|CC = cond(v4 - Mem0[r1 + 8<i64>:word32])");
        }

        [Test]
        public void zSeriesRw_clc()
        {
            Given_HexString("D507D0002000");
            AssertCode(     // clc	(8,r13),(r2)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(Mem0[r13:byte] - Mem0[r2:byte])");
        }

        [Test]
        public void zSeriesRw_clcl()
        {
            Given_HexString("0F7E");
            AssertCode(     // clcl	r7,r14
                "0|L--|00100000(2): 1 instructions",
                "1|L--|CC = cond(r7_r8 -u r14_r15)");
        }

        [Test]
        public void zSeriesRw_clcle()
        {
            Given_HexString("A9AFEB11");
            AssertCode(     // clcle	r10,r15,-1263(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r10_r11 -u r15_r0)");
        }


        [Test]
        public void zSeriesRw_clfi()
        {
            Given_HexString("C24F00000018");
            AssertCode(     // clfi	r4,00000018
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = SLICE(r4, uint32, 0)",
                "2|L--|CC = cond(v4 - 0x18<u32>)");
        }

        [Test]
        public void zSeriesRw_clg()
        {
            Given_HexString("E31050000021");
            AssertCode(     // clg	r1,(r5)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(r1 - Mem0[r5:word64])");
        }

        [Test]
        public void zSeriesRw_clgij()
        {
            Given_HexString("EC4C0039037D");
            AssertCode(     // clgij	r4,+03,04,000000008003CA5A
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r4 <=u 3<64>) branch 00100072");
        }

        [Test]
        public void zSeriesRw_clgr()
        {
            Given_HexString("B921001B");
            AssertCode(     // clgr	r1,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(r1 -u r11)");
        }

        [Test]
        public void zSeriesRw_clgrj()
        {
            Given_HexString("EC12000D2065");
            AssertCode(     // clgrj	r1,r2,02,0000000080069FD6
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (r1 >u r2) branch 0010001A");
        }

        [Test]
        public void zSeriesRw_clgrl()
        {
            Given_HexString("C65A00029D13");
            AssertCode(     // clgrl	r5,000000008009A4B8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(r5 -u Mem0[0x00153A26<p32>:word64])");
        }

        [Test]
        public void zSeriesRw_cli()
        {
            Given_HexString("9500B000");
            AssertCode(     // cli	(r11),00
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = cond(Mem0[r11:byte] - 0<8>)");
        }

        [Test]
        public void zSeriesRw_cliy()
        {
            Given_HexString("EB0ACFFFFF55");
            AssertCode(     // cliy	-1(r12),0A
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = cond(Mem0[r12 + -1<i64>:byte] - 0xA<8>)");
        }

        [Test]
        public void zSeriesRw_clij()
        {
            Given_HexString("EC420058027F");
            AssertCode(     // clij	r4,+02,04,000000008005C5E8
                "0|T--|00100000(6): 2 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|T--|if (v4 >u 2<32>) branch 001000B0");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_clm()
        {
            Given_HexString("BDB1D000");
            AssertCode(     // clm	r11,(r13),01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_clr()
        {
            Given_HexString("151D");
            AssertCode(     // clr	r1,r13
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v6 = SLICE(r13, word32, 0)",
                "3|L--|CC = cond(v4 -u v6)");
        }

        [Test]
        public void zSeriesRw_clrj()
        {
            Given_HexString("EC21002F2077");
            AssertCode(     // clrj	r2,r1,02,000000008004A8F6
                "0|T--|00100000(6): 3 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v6 = SLICE(r1, word32, 0)",
                "3|T--|if (v4 >u v6) branch 0010005E");
        }

        [Test]
        public void zSeriesRw_clrl()
        {
            Given_HexString("C61F0000A701");
            AssertCode(     // clrl	r1,00000000000270A4
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|CC = cond(v4 -u Mem0[0x00114E02<p32>:word32])");
        }

        [Test]
        public void zSeriesRw_cr()
        {
            Given_HexString("1912");
            AssertCode(     // cr	r1,r2
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(r1, int32, 0)",
                "2|L--|v6 = SLICE(r2, int32, 0)",
                "3|L--|CC = cond(v4 - v6)");
        }

        [Test]
        public void zSeriesRw_crj()
        {
            Given_HexString("EC160078A076");
            AssertCode(     // crj	r1,r6,0A,000000008006A0DA
                "0|T--|00100000(6): 3 instructions",
                "1|L--|v4 = SLICE(r1, int32, 0)",
                "2|L--|v6 = SLICE(r6, int32, 0)",
                "3|T--|if (v4 >= v6) branch 001000F0");
        }

        [Test]
        public void zSeriesRw_cs()
        {
            Given_HexString("BA121010");
            AssertCode(     // cs	r1,r2,16(r1)
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v6 = SLICE(r2, word32, 0)",
                "3|L--|CC = __compare_and_swap<word32>(v4, v6, &Mem0[r1 + 16<i64>:word32], out v4)");
        }

        [Test]
        public void zSeriesRw_csg()
        {
            Given_HexString("EB1240000030");
            AssertCode(     // csg	r1,r2,(r4)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __compare_and_swap<word64>(r1, r2, &Mem0[r4:word64], out r1)");
        }

        [Test]
        public void zSeriesRw_cvb()
        {
            Given_HexString("4FBBA749");
            AssertCode(     // cvb	r11,1865(r11,r10)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = __convert_decimal_to_int<int32>(&Mem0[r10 + r11 + 1865<i64>:byte])",
                "2|L--|r11 = SEQ(SLICE(r11, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_cvd()
        {
            Given_HexString("4E12B902");
            AssertCode(     // cvd	r1,-1790(r2,r11)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r1, int32, 0)",
                "2|L--|__convert_int_to_decimal<int32>(v4, &Mem0[r11 + r2 + -1790<i64>:byte])");
        }

        [Test]
        public void zSeriesRw_d()
        {
            Given_HexString("5D4A9101");
            AssertCode(     // d	r4,257(r10,r9)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v6 = r4 /32 Mem0[(r9 + r10) + 257<i64>:int32]",
                "2|L--|v7 = r4 % Mem0[(r9 + r10) + 257<i64>:int32]",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "4|L--|r5 = SEQ(SLICE(r5, word32, 32), v7)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_de()
        {
            Given_HexString("7DB8A729");
            AssertCode(     // de	f11,1833(r8,r10)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_der()
        {
            Given_HexString("3D59");
            AssertCode(     // der	f5,f9
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(f5, real32, 0)",
                "2|L--|v6 = SLICE(f9, real32, 0)",
                "3|L--|v7 = v4 / v6",
                "4|L--|f5 = SEQ(SLICE(f5, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_dlgr()
        {
            Given_HexString("B987004B");
            AssertCode(     // dlgr	r4,r11
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r4 = r4_r5 /u r11",
                "2|L--|r5 = r4_r5 % r11");
        }

        [Test]
        public void zSeriesRw_dp()
        {
            Given_HexString("FD8AB90400B2");
            AssertCode(     // dp	-1788(9,r11),178(11,r0)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __packed_divide(r11, -1788<i32>, r0, 178<i32>, r11)");
        }

        [Test]
        public void zSeriesRw_dr()
        {
            Given_HexString("1DBC");
            AssertCode(     // dr	r11,r12
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v5 = SLICE(r12, int32, 0)",
                "2|L--|v6 = r11 /32 v5",
                "3|L--|v7 = r11 % v5",
                "4|L--|r11 = SEQ(SLICE(r11, word32, 32), v6)",
                "5|L--|r12 = SEQ(SLICE(r12, word32, 32), v7)");
        }

        [Test]
        public void zSeriesRw_dsgr()
        {
            Given_HexString("B90D00A3");
            AssertCode(     // dsgr	r10,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = r10 / r3",
                "2|L--|r11 = r10 % r3");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_ed()
        {
            Given_HexString("DE96B9040012");
            AssertCode(     // ed	-1788(151,r11),00000012
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_edmk()
        {
            Given_HexString("DFF1A7290001");
            AssertCode(     // edmk	1833(242,r10),00000001
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ex()
        {
            Given_HexString("44405000");
            AssertCode(     // ex	r4,(r5)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = __execute<word64>(r4, Mem0[r5:word64])");
        }

        [Test]
        public void zSeriesRw_exrl()
        {
            Given_HexString("C64000000028");
            AssertCode(     // exrl	r4,0000000080018122
                "0|L--|00100000(6): 1 instructions",
                "1|L--|CC = __execute<word64>(r4, 0x00100050<p32>)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_flogr()
        {
            Given_HexString("B9830041");
            AssertCode(     // flogr	r4,r1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_hdr()
        {
            Given_HexString("247D");
            AssertCode(     // hdr	f7,f13
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f7 = f13 / 2.0");
        }

        [Test]
        public void zSeriesRw_her()
        {
            Given_HexString("347D");
            AssertCode(     // her	f7,f13
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(f13, real32, 0)",
                "2|L--|v6 = v4 / 2.0F",
                "3|L--|f7 = SEQ(SLICE(f7, word32, 32), v6)");
        }

        [Test]
        public void zSeriesRw_ic()
        {
            Given_HexString("43003000");
            AssertCode(     // ic	r0,(r3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = Mem0[r3:byte]",
                "2|L--|r0 = SEQ(SLICE(r0, word56, 8), v3)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_icm()
        {
            Given_HexString("BF0F1000");
            AssertCode(     // icm	r0,(r1),0F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __insert_char_mask(r0, Mem0[r1:word32], 0xF<8>)",
                "2|L--|v4 = Mem0[r1 + 1:byte]",
                "3|L--|v5 = Mem0[r1 + 2:byte]",
                "4|L--|v6 = Mem0[r1 + 3:byte]");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_iihh()
        {
            Given_HexString("A590C0E5");
            AssertCode(     // iihh	r9,-00003F1B
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_je()
        {
            Given_HexString("A7840012");
            AssertCode(     // je	00000876
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100024");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_jgnl()
        {
            Given_HexString("C0B400000004");
            AssertCode(     // jgnl	000000000001B20A
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_jh()
        {
            Given_HexString("A7240006");
            AssertCode(     // jh	00000792
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGT,CC)) branch 0010000C");
        }

        [Test]
        public void zSeriesRw_jhe()
        {
            Given_HexString("A7A40015");
            AssertCode(     // jhe	0000000000000630
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGE,CC)) branch 0010002A");
        }

        [Test]
        public void zSeriesRw_jne()
        {
            Given_HexString("A7740008");
            AssertCode(     // jne	0000074C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100010");
        }

        [Test]
        public void zSeriesRw_jnh()
        {
            Given_HexString("A7D40004");
            AssertCode(     // jnh	000000000001B05C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(ULE,CC)) branch 00100008");
        }

        [Test]
        public void zSeriesRw_jnl()
        {
            Given_HexString("A7B4000E");
            AssertCode(     // jnl	000000008004CFC2
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,CC)) branch 0010001C");
        }

        [Test]
        public void zSeriesRw_jo()
        {
            Given_HexString("A714FFFE");
            AssertCode(     // jo	00000000800180A8
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(OV,CC)) branch 000FFFFC");
        }

        [Test]
        public void zSeriesRw_la()
        {
            Given_HexString("4140F008");
            AssertCode(     // la	r4,8(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r15 + 8<i64>");
        }

        [Test]
        public void zSeriesRw_laa()
        {
            Given_HexString("EB12400000F8");
            AssertCode(     // laa	r1,r2,(r4)
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = r2",
                "2|L--|r2 = r2 + Mem0[r4:int32]",
                "3|L--|r1 = v4",
                "4|L--|CC = cond(r2)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_lae()
        {
            Given_HexString("513EC0E5");
            AssertCode(     // lae	r3,229(r14,r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_larl()
        {
            Given_HexString("C05000000140");
            AssertCode(     // larl	r5,00100280
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r5 = SEQ(SLICE(r5, word32, 32), 0x00100280<p32>)");
        }

        [Test]
        public void zSeriesRw_lay()
        {
            Given_HexString("E3F0FF60FF71");
            AssertCode(     // lay	r15,-160(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r15 = r15 + -160<i64>");
        }

        [Test]
        public void zSeriesRw_lbr()
        {
            Given_HexString("B9260013");
            AssertCode(     // lbr	r1,r3
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r3, int8, 0)",
                "2|L--|v6 = CONVERT(v4, int8, int32)",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v6)");
        }

        [Test]
        public void zSeriesRw_lcdr()
        {
            Given_HexString("2385");
            AssertCode(     // lcdr	f8,f5
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f8 = -f5",
                "2|L--|CC = cond(f8)");
        }

        [Test]
        public void zSeriesRw_lcer()
        {
            Given_HexString("337F");
            AssertCode(     // lcer	f7,f15
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(f15, real32, 0)",
                "2|L--|v6 = -v4",
                "3|L--|f7 = SEQ(SLICE(f7, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_lcgr()
        {
            Given_HexString("B9030054");
            AssertCode(     // lcgr	r5,r4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = -r4",
                "2|L--|CC = cond(r5)");
        }

        [Test]
        public void zSeriesRw_lcr()
        {
            Given_HexString("1342");
            AssertCode(     // lcr	r4,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(r2, int32, 0)",
                "2|L--|v6 = -v4",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        [Ignore("S390 instr")]

        public void zSeriesRw_lctl()
        {
            Given_HexString("B7C8C010");
            AssertCode(     // lctl	r12,16(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ld()
        {
            Given_HexString("6800F0A0");
            AssertCode(     // ld	f0,160(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = Mem0[r15 + 160<i64>:word64]");
        }

        [Test]
        public void zSeriesRw_ldeb()
        {
            Given_HexString("ED0030000004");
            AssertCode(     // ldeb	f0,(r3)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|f0 = CONVERT(Mem0[r3:real32], real32, real64)");
        }

        [Test]
        public void zSeriesRw_ldgr()
        {
            Given_HexString("B3C1002B");
            AssertCode(     // ldgr	r2,r11
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f2 = r11");
        }

        [Test]
        public void zSeriesRw_ldr()
        {
            Given_HexString("2850");
            AssertCode(     // ldr	f5,f0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f5 = f0");
        }

        [Test]
        public void zSeriesRw_ldxr()
        {
            Given_HexString("25EC");
            AssertCode(     // ldxr	f14,f12
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f14_f15 = f12_f13");
        }

        [Test]
        public void zSeriesRw_le()
        {
            Given_HexString("7800F0A4");
            AssertCode(     // le	f0,164(r15)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r15 + 164<i64>:real32]",
                "2|L--|f0 = SEQ(SLICE(f0, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_ledr()
        {
            Given_HexString("35BC");
            AssertCode(     // ledr	f11,f12
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = CONVERT(f12, real64, real32)",
                "2|L--|f11 = SEQ(SLICE(f11, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_leer()
        {
            Given_HexString("3850");
            AssertCode(     // ler	f5,f0
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|f5 = SEQ(SLICE(f5, word32, 32), v4)");
        }

        [Test]
        public void zSeriesRw_lgfrl()
        {
            Given_HexString("C42C0001ECC8");
            AssertCode(     // lgfrl	r2,000000008009A9A8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[0x0013D990<p32>:int32], int32, int64)");
        }

        [Test]
        public void zSeriesRw_lgf()
        {
            Given_HexString("E314F15C0014");
            AssertCode(     // lgf	r1,348(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = CONVERT(Mem0[r15 + r4 + 348<i64>:int32], int32, int64)");
        }


        [Test]
        public void zSeriesRw_lgr()
        {
            Given_HexString("B904001F");
            AssertCode(     // lgr	r1,r15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r15");
        }

        [Test]
        public void zSeriesRw_lgrl()
        {
            Given_HexString("C41800000D32");
            AssertCode(     // lgrl	r1,0000000000001FF8
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = Mem0[0x00101A64<p32>:word64]");
        }

        [Test]
        public void zSeriesRw_lg()
        {
            Given_HexString("E330F0000004");
            AssertCode(     // lg	r3,(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r3 = Mem0[r15:word64]");
        }

        [Test]
        public void zSeriesRw_lgdr()
        {
            Given_HexString("B3CD00F0");
            AssertCode(     // lgdr	r15,r0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = f0");
        }

        [Test]
        public void zSeriesRw_lghi()
        {
            Given_HexString("A709FFF0");
            AssertCode(     // lghi	r0,-00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = 0xFFFFFFFFFFFFFFF0<64>");
        }

        [Test]
        public void zSeriesRw_lh()
        {
            Given_HexString("4810400C");
            AssertCode(     // lh	r1,12(r4)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r4 + 12<i64>:word16]",
                "2|L--|r1 = SEQ(SLICE(r1, word48, 16), v5)");
        }

        [Test]
        public void zSeriesRw_lhrl()
        {
            Given_HexString("C4250003F737");
            AssertCode(     // lhrl	r2,000000008009A9CE
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = CONVERT(Mem0[0x0017EE6E<p32>:int16], int16, int32)",
                "2|L--|r2 = SEQ(SLICE(r2, word32, 32), v4)");
        }

        [Test]
        public void zSeriesRw_llcr()
        {
            Given_HexString("B99400B2");
            AssertCode(     // llcr	r11,r2
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r2, byte, 0)",
                "2|L--|v6 = CONVERT(v4, byte, word32)",
                "3|L--|r11 = SEQ(SLICE(r11, word32, 32), v6)");
        }

        [Test]
        public void zSeriesRw_llgcr()
        {
            Given_HexString("B9840066");
            AssertCode(     // llgcr	r6,r6
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r6, byte, 0)",
                "2|L--|r6 = CONVERT(v4, byte, word64)");
        }

        [Test]
        public void zSeriesRw_llgfr()
        {
            Given_HexString("B9160033");
            AssertCode(     // llgfr	r3,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r3, word32, 0)",
                "2|L--|r3 = CONVERT(v4, word32, word64)");
        }

        [Test]
        public void zSeriesRw_llgfrl()
        {
            Given_HexString("C42E00024D1B");
            AssertCode(     // llgfrl	r2,000000008009A14C
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[0x00149A36<p32>:word32], word32, word64)");
        }

        [Test]
        public void zSeriesRw_llgtr()
        {
            Given_HexString("B91700AA");
            AssertCode(     // llgtr	r10,r10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = SLICE(r10, word31, 0)",
                "2|L--|r10 = CONVERT(v3, word31, word64)");
        }

        [Test]
        public void zSeriesRw_llhr()
        {
            Given_HexString("B9950022");
            AssertCode(     // llhr	r2,r2
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r2, word16, 0)",
                "2|L--|v5 = CONVERT(v4, word16, word32)",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_llhrl()
        {
            Given_HexString("C402E340B130");
            AssertCode(     // llhrl	r0,FFFFFFFFC6817B40
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v3 = CONVERT(0xC6916260<p32>, ptr32, word32)",
                "2|L--|r0 = SEQ(SLICE(r0, word32, 32), v3)");
        }

        [Test]
        public void zSeriesRw_llill()
        {
            Given_HexString("A52F8030");
            AssertCode(     // llill	r2,-00007FD0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = SEQ(SLICE(r2, word48, 16), 0x8030<16>)");
        }


        [Test]
        public void zSeriesRw_lndr()
        {
            Given_HexString("21FC");
            AssertCode(     // lndr	f15,f12
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f15 = -f12",
                "2|L--|CC = cond(f15)");
        }

        [Test]
        public void zSeriesRw_lner()
        {
            Given_HexString("3100");
            AssertCode(     // lner	f0,f0
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|v5 = -v4",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_lngr()
        {
            Given_HexString("B9010054");
            AssertCode(     // lngr	r5,r4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = -r4",
                "2|L--|CC = cond(r5)");
        }

        [Test]
        public void zSeriesRw_lnr()
        {
            Given_HexString("1122");
            AssertCode(     // lnr	r2,r2
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(r2, int32, 0)",
                "2|L--|v5 = -v4",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_locg()
        {
            Given_HexString("EB1C201000E2");
            AssertCode(     // locg	r1,0C,16(r2)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r1 = Mem0[r2 + 16<i64>:word64]");
        }

        [Test]
        public void zSeriesRw_locgre()
        {
            Given_HexString("B9E280D1");
            AssertCode(     // locgre	r13,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,CC)) branch 00100004",
                "2|L--|r13 = r1");
        }

        [Test]
        public void zSeriesRw_locgrh()
        {
            Given_HexString("B9E22012");
            AssertCode(     // locgrh	r1,r2
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULE,CC)) branch 00100004",
                "2|L--|r1 = r2");
        }

        [Test]
        public void zSeriesRw_locgrl()
        {
            Given_HexString("B9E24021");
            AssertCode(     // locgrl	r2,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,CC)) branch 00100004",
                "2|L--|r2 = r1");
        }

        [Test]
        public void zSeriesRw_locgrle()
        {
            Given_HexString("B9E2C053");
            AssertCode(     // locgrle	r5,r3
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(GT,CC)) branch 00100004",
                "2|L--|r5 = r3");
        }

        [Test]
        public void zSeriesRw_locgrne()
        {
            Given_HexString("B9E270C1");
            AssertCode(     // locgrne	r12,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,CC)) branch 00100004",
                "2|L--|r12 = r1");
        }

        [Test]
        public void zSeriesRw_locgrnhe()
        {
            Given_HexString("B9E25091");
            AssertCode(     // locgrnhe	r9,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGT,CC)) branch 00100004",
                "2|L--|r9 = r1");
        }

        [Test]
        public void zSeriesRw_locgrnl()
        {
            Given_HexString("B9E2B012");
            AssertCode(     // locgrnl	r1,r2
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,CC)) branch 00100004",
                "2|L--|r1 = r2");
        }

        [Test]
        public void zSeriesRw_locgrnle()
        {
            Given_HexString("B9E230C1");
            AssertCode(     // locgrnle	r12,r1
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,CC)) branch 00100004",
                "2|L--|r12 = r1");
        }

        [Test]
        public void zSeriesRw_lra()
        {
            Given_HexString("B1300004");
            AssertCode(     // lra	r3,00000004
                "0|S--|00100000(4): 1 instructions",
                "1|L--|CC = __load_real_address<word64>(0x00000004<p32>, out r3)");
        }

        [Test]
        public void zSeriesRw_lrl()
        {
            Given_HexString("C41D00028A15");
            AssertCode(     // lrl	r1,000000008009A948
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = Mem0[0x0015142A<p32>:word32]",
                "2|L--|r1 = SEQ(SLICE(r1, word32, 32), v4)");
        }

        [Test]
        public void zSeriesRw_lrvgr()
        {
            Given_HexString("B90F0022");
            AssertCode(     // lrvgr	r2,r2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __load_reverse<word64>(r2)");
        }

        [Test]
        public void zSeriesRw_lrvr()
        {
            Given_HexString("B91F0022");
            AssertCode(     // lrvr	r2,r2
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v5 = __load_reverse<word32>(v4)",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)");
        }

        [Test]
        public void zSeriesRw_lt()
        {
            Given_HexString("E31010000012");
            AssertCode(     // lt	r1,(r1)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v4 = Mem0[r1:word32]",
                "2|L--|r1 = SEQ(SLICE(r1, word32, 32), v4)",
                "3|L--|CC = cond(v4 - 0<32>)");
        }

        [Test]
        public void zSeriesRw_lter()
        {
            Given_HexString("32F7");
            AssertCode(     // lter	f15,f7
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(f7, real32, 0)",
                "2|L--|f15 = SEQ(SLICE(f15, word32, 32), v4)",
                "3|L--|CC = cond(v4 - 0.0F)");
        }

        [Test]
        public void zSeriesRw_ltr()
        {
            Given_HexString("1244");
            AssertCode(     // ltr	r4,r4
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|r4 = SEQ(SLICE(r4, word32, 32), v4)",
                "3|L--|CC = cond(v4 - 0<32>)");
        }

        [Test]
        public void zSeriesRw_ltgr()
        {
            Given_HexString("B9020011");
            AssertCode(     // ltgr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1",
                "2|L--|CC = cond(r1 - 0<64>)");
        }

        [Test]
        public void zSeriesRw_m()
        {
            Given_HexString("5C75A749");
            AssertCode(     // m	r7,1865(r5,r10)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r8, int32, 0)",
                "2|L--|r7 = v4 *s64 Mem0[(r10 + r5) + 1865<i64>:int32]");
        }

        [Test]
        public void zSeriesRw_md()
        {
            Given_HexString("6C7EEC58");
            AssertCode(     // md	f7,-936(r14,r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f7 = f7 * Mem0[(r14 + r14) + -936<i64>:real64]");
        }

        [Test]
        public void zSeriesRw_mdb()
        {
            Given_HexString("ED00D000001C");
            AssertCode(     // mdb	f0,(r13)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|f0 = f0 * Mem0[r13:real64]");
        }

        [Test]
        public void zSeriesRw_mde()
        {
            Given_HexString("7C6C0707");
            AssertCode(     // mde	f6,1799(r12)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(f6, real32, 0)",
                "2|L--|f6 = v4 *64 Mem0[0x0000000000000707<p64>:real32]");
        }

        [Test]
        public void zSeriesRw_mder()
        {
            Given_HexString("3CBF");
            AssertCode(     // mder	f11,f15
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(f11, real32, 0)",
                "2|L--|v6 = SLICE(f15, real32, 0)",
                "3|L--|f11 = v4 *64 v6");
        }

        [Test]
        public void zSeriesRw_mdr()
        {
            Given_HexString("2C84");
            AssertCode(     // mdr	f8,f4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|f8 = f8 * f4");
        }

        [Test]
        public void zSeriesRw_meeb()
        {
            Given_HexString("ED0050000017");
            AssertCode(     // meeb	f0,(r5)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|v6 = v4 * Mem0[r5:real32]",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v6)");
        }

        [Test]
        public void zSeriesRw_meer()
        {
            Given_HexString("B3370034");
            AssertCode(     // mder	f3,f4
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f3, real32, 0)",
                "2|L--|v6 = SLICE(f4, real32, 0)",
                "3|L--|v7 = v4 * v6",
                "4|L--|f3 = SEQ(SLICE(f3, word32, 32), v7)");
        }

        [Test]
        public void zSeriesRw_mh()
        {
            Given_HexString("4CA8C0E5");
            AssertCode(     // mh	r10,229(r8,r12)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r10, int32, 0)",
                "2|L--|v7 = v4 *s CONVERT(Mem0[(r12 + r8) + 229<i64>:int16], int16, int32)",
                "3|L--|r10 = SEQ(SLICE(r10, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_mlgr()
        {
            Given_HexString("B9860085");
            AssertCode(     // mlgr	r8,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8_r9 = r8 *u128 r5");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mp()
        {
            Given_HexString("FC1DE340F130");
            AssertCode(     // mp	832(2,r14),304(14,r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]

        public void zSeriesRw_mr()
        {
            Given_HexString("1CBC");
            AssertCode(     // mr	r11,r12
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = SLICE(r11, int32, 0)",
                "2|L--|v6 = SLICE(r12, int32, 0)",
                "3|L--|r11_r12 = v4@@@");
        }

        [Test]
        public void zSeriesRw_msgr()
        {
            Given_HexString("B90C0015");
            AssertCode(     // msgr	r1,r5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r1 *s r5");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mvck()
        {
            Given_HexString("D9621832C03B");
            AssertCode(     // mvck	-1998(r6,r1),59(r12),r2
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mvcl()
        {
            Given_HexString("0E7E");
            AssertCode(     // mvcl	r7,r14
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mvcos()
        {
            Given_HexString("C860A7F4FEC8");
            AssertCode(     // mvcos	2036(r10),-312(r15),r6
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mvcp()
        {
            Given_HexString("DAD5A7F4FC69");
            AssertCode(     // mvcp	2036(r13,r10),-919(r15),r5
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_mvhi()
        {
            Given_HexString("E54CF0A00004");
            AssertCode(     // mvhi	(r15),0004
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[r15:word16] = 4<16>");
        }

        [Test]
        public void zSeriesRw_mvi()
        {
            Given_HexString("9201B000");
            AssertCode(     // mvi	(r11),01
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11:byte] = 1<8>");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mviy()
        {
            Given_HexString("EB201FFFFF52");
            AssertCode(     // mviy	-1(r1),20
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_mxd()
        {
            Given_HexString("6760C0E5");
            AssertCode(     // mxd	f6,229(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f6_f7 = f6 *128 Mem0[r12 + 229<i64>:real64]");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mxdr()
        {
            Given_HexString("27F1");
            AssertCode(     // mxdr	f15,f1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mxr()
        {
            Given_HexString("2617");
            AssertCode(     // mxr	f1,f7
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f1_f2 = f1_f2 * f7_f8",
                "1|L--|f1 = f1 *128 f7");
        }

        [Test]
        public void zSeriesRw_n()
        {
            Given_HexString("5440A749");
            AssertCode(     // n	r4,1865(r10)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|v6 = v4 & Mem0[r10 + 1865<i64>:word32]",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_nop()
        {
            Given_HexString("47000000");
            AssertCode(     // nop
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void zSeriesRw_nopr()
        {
            Given_HexString("0707");
            AssertCode(     // nopr	r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void zSeriesRw_ngr()
        {
            Given_HexString("B98000F0");
            AssertCode(     // ngr	r15,r0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r15 = r15 & r0",
                "2|L--|CC = cond(r15)");
        }

        [Test]
        public void zSeriesRw_nr()
        {
            Given_HexString("1428");
            AssertCode(     // nr	r2,r8
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v6 = SLICE(r8, word32, 0)",
                "3|L--|v7 = v4 & v6",
                "4|L--|r2 = SEQ(SLICE(r2, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }



        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_lm()
        {
            Given_HexString("989F0DE1");
            AssertCode(     // lm	r9,r15,FFFFFDE1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_lmg()
        {
            Given_HexString("EB68F0D00004");
            AssertCode(     // lmg	r6,r8,208(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v4 = r15 + 208<i64>",
                "2|L--|r6 = Mem0[v4:word64]",
                "3|L--|v4 = v4 + 8<i64>",
                "4|L--|r7 = Mem0[v4:word64]",
                "5|L--|v4 = v4 + 8<i64>",
                "6|L--|r8 = Mem0[v4:word64]");
        }

        [Test]
        public void zSeriesRw_lgfr()
        {
            Given_HexString("B9140011");
            AssertCode(     // lgfr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|r1 = CONVERT(v4, word32, int64)");
        }

        [Test]
        public void zSeriesRw_lpdr()
        {
            Given_HexString("2001");
            AssertCode(     // lpdr	f0,f1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f0 = fabs<real64>(f1)",
                "2|L--|CC = cond(f0)");
        }

        [Test]
        public void zSeriesRw_lper()
        {
            Given_HexString("3001");
            AssertCode(     // lper	f0,f1
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(f1, real32, 0)",
                "2|L--|v6 = fabsf(v4)",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_lpgr()
        {
            Given_HexString("B9000011");
            AssertCode(     // lpgr	r1,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = abs<int64>(r1)",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_lpr()
        {
            Given_HexString("1008");
            AssertCode(     // lpr	r0,r8
                "0|L--|00100000(2): 4 instructions",
                "1|L--|v4 = SLICE(r8, int32, 0)",
                "2|L--|v6 = abs<int32>(v4)",
                "3|L--|r0 = SEQ(SLICE(r0, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_lpsw()
        {
            Given_HexString("8216C478");
            AssertCode(     // lpsw	1144(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ltg()
        {
            Given_HexString("E31010000002");
            AssertCode(     // ltg	r1,(r1)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = Mem0[r1:word64]",
                "2|L--|CC = cond(r1 - 0<64>)");
        }


        [Test]
        public void zSeriesRw_j()
        {
            Given_HexString("A7F4001E");
            AssertCode(     // j	000007CA
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0010003C");
        }

        [Test]
        public void zSeriesRw_jg()
        {
            Given_HexString("C0F4FFFFFF9D");
            AssertCode(     // jg	00000680
                "0|T--|00100000(6): 1 instructions",
                "1|T--|if (Test(GT,CC)) branch 000FFF3A");
        }

        [Test]
        public void zSeriesRw_lr()
        {
            Given_HexString("18A1");
            AssertCode(     // lr	r10,r1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|r10 = SEQ(SLICE(r10, word32, 32), v4)");
        }

        [Test]
        public void zSeriesRw_lhi()
        {
            Given_HexString("A728FFF0");
            AssertCode(     // lhi	r2,+00000010
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = 0xFFFFFFFFFFFFFFF0<64>");
        }

        [Test]
        public void zSeriesRw_mvz()
        {
            Given_HexString("D207F0B0F160");
            AssertCode(     // mvz	176(8,r15),352(r15)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|v4 = __move_zones<byte>(Mem0[r15 + 176<i64>:byte], Mem0[r15 + 352<i64>:byte])",
                "2|L--|Mem0[r15 + 176<i64>:byte] = v4");
        }

        [Test]
        public void zSeriesRw_nc()
        {
            Given_HexString("D407C038D000");
            AssertCode(     // nc	56(8,r12),(r13)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v4 = Mem0[r12 + 56<i64>:byte] & Mem0[r13:byte]",
                "2|L--|Mem0[r12 + 56<i64>:byte] = v4",
                "3|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_mvcle()
        {
            Given_HexString("A8245000");
            AssertCode(     // mvcle	r2,r4,00000000
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2_r4 = __mvcle<word128>(r5)",
                "2|L--|CC = cond(r2_r4)");
        }

        [Test]
        public void zSeriesRw_jle()
        {
            Given_HexString("A7C40009");
            AssertCode(     // jle	000000008006BE94
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LE,CC)) branch 00100012");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_mvo()
        {
            Given_HexString("F1100004EBBF");
            AssertCode(     // mvo	4(2,r0),-1089(1,r14)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ni()
        {
            Given_HexString("94FE2002");
            AssertCode(     // ni	2(r2),FE
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[r2 + 2<i64>:byte] & 0xFE<8>",
                "2|L--|Mem0[r2 + 2<i64>:byte] = v4",
                "3|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_o()
        {
            Given_HexString("56102000");
            AssertCode(     // o	r1,(r2)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v6 = v4 | Mem0[r2:word32]",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_oc()
        {
            Given_HexString("D6C54120F0B8");
            AssertCode(     // oc	288(198,r4),184(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ogr()
        {
            Given_HexString("B9810015");
            AssertCode(     // ogr	r1,r5
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 | r5",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_oi()
        {
            Given_HexString("96012002");
            AssertCode(     // oi	2(r2),01
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[r2 + 2<i64>:byte] | 1<8>",
                "2|L--|Mem0[r2 + 2<i64>:byte] = v4",
                "3|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_or()
        {
            Given_HexString("164B");
            AssertCode(     // or	r4,r11
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|v6 = SLICE(r11, word32, 0)",
                "3|L--|v7 = v4 | v6",
                "4|L--|r4 = SEQ(SLICE(r4, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_pack()
        {
            Given_HexString("F2100004EC18");
            AssertCode(     // pack	4(2,r0),-1000(1,r14)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_pku()
        {
            Given_HexString("E18EB9020033");
            AssertCode(     // pku	-1790(r11),51(143,r0)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_risbg()
        {
            Given_HexString("EC223EBF0055");
            AssertCode(     // risbg	r2,r2,3E,BF,00
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r2 = __risbg(r2, 0x3E<8>, 0xBF<8>, 0<8>)",
                "2|L--|CC = cond(r2)");
        }

        [Test]
        public void zSeriesRw_risbgn()
        {
            Given_HexString("ECBB38BF4B59");
            AssertCode(     // risbgn	r11,r11,38,BF,4B
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r11 = __risbgn(r11, 0x38<8>, 0xBF<8>, 0x4B<8>)",
                "2|L--|CC = cond(r11)");
        }

        [Test]
        public void zSeriesRw_rllg()
        {
            Given_HexString("EB220020001C");
            AssertCode(     // rllg	r2,r2,00000020
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r2 = __rol<word64,int32>(r2, 32<i32>)",
                "2|L--|CC = cond(r2)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_rosbg()
        {
            Given_HexString("ECA330370856");
            AssertCode(     // rosbg	r10,r3,30,37,08
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_rxsbg()
        {
            Given_HexString("EC22203F2057");
            AssertCode(     // rxsbg	r2,r2,20,3F,20
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_s()
        {
            Given_HexString("5B203010");
            AssertCode(     // s	r2,16(r3)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r2, int32, 0)",
                "2|L--|v6 = v4 - Mem0[r3 + 16<i64>:int32]",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_sd()
        {
            Given_HexString("6B14A749");
            AssertCode(     // sd	f1,1865(r4,r10)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|f1 = f1 - Mem0[(r10 + r4) + 1865<i64>:real64]",
                "2|L--|CC = cond(f1)");
        }

        [Test]
        public void zSeriesRw_sdb()
        {
            Given_HexString("ED005000001B");
            AssertCode(     // sdb	f0,(r5)
                "0|L--|00100000(6): 2 instructions",
                "1|L--|f0 = f0 - Mem0[r5:real64]",
                "2|L--|CC = cond(f0)");
        }

        [Test]
        public void zSeriesRw_sdr()
        {
            Given_HexString("2B7E");
            AssertCode(     // sdr	f7,f14
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f7 = f7 - f14",
                "2|L--|CC = cond(f7)");
        }

        [Test]
        public void zSeriesRw_se()
        {
            Given_HexString("7BF5A728");
            AssertCode(     // se	f15,1832(r5,r10)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f15, real32, 0)",
                "2|L--|v7 = v4 - Mem0[(r10 + r5) + 1832<i64>:real32]",
                "3|L--|f15 = SEQ(SLICE(f15, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_seb()
        {
            Given_HexString("ED005000000B");
            AssertCode(     // seb	f0,(r5)
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = SLICE(f0, real32, 0)",
                "2|L--|v6 = v4 - Mem0[r5:real32]",
                "3|L--|f0 = SEQ(SLICE(f0, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_ser()
        {
            Given_HexString("3B59");
            AssertCode(     // ser	f5,f9
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(f5, real32, 0)",
                "2|L--|v6 = SLICE(f9, real32, 0)",
                "3|L--|v7 = v4 - v6",
                "4|L--|f5 = SEQ(SLICE(f5, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_sgfr()
        {
            Given_HexString("B91900B1");
            AssertCode(     // sgfr	r11,r1
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r1, int32, 0)",
                "2|L--|r11 = r11 - CONVERT(v5, int32, int64)",
                "3|L--|CC = cond(r11)");
        }

        [Test]
        public void zSeriesRw_sgr()
        {
            Given_HexString("B9090031");
            AssertCode(     // sgr	r3,r1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r3 - r1",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_sh()
        {
            Given_HexString("4BFCE325");
            AssertCode(     // sh	r15,805(r12,r14)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r15, int32, 0)",
                "2|L--|v7 = v4 - CONVERT(Mem0[(r14 + r12) + 805<i64>:int16], int16, int32)",
                "3|L--|r15 = SEQ(SLICE(r15, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_sigp()
        {
            Given_HexString("AE68A728");
            AssertCode(     // sigp	r6,1832(r10)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_sl()
        {
            Given_HexString("5F4F4112");
            AssertCode(     // sl	r4,274(r15,r4)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|v6 = v4 - Mem0[(r4 + r15) + 274<i64>:word32]",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_sla()
        {
            Given_HexString("8B9DB904");
            AssertCode(     // sla	r9,-1788(r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r9, int32, 0)",
                "2|L--|v5 = v4 << 4<i32>",
                "3|L--|r9 = SEQ(SLICE(r9, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_slbgr()
        {
            Given_HexString("B9890011");
            AssertCode(     // slbgr	r1,r1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_slda()
        {
            Given_HexString("8FFDFF71");
            AssertCode(     // slda	r15,-143(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_sldl()
        {
            Given_HexString("8DF10DE1");
            AssertCode(     // sldl	r15,FFFFFDE1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_slfi()
        {
            Given_HexString("C2257FF00000");
            AssertCode(     // slfi	r2,7FF00000
                "0|L--|00100000(6): 1 instructions",
                "1|L--|r2 = r2 - 0x7FF00000<32>");
        }

        [Test]
        public void zSeriesRw_slgfr()
        {
            Given_HexString("B91B002C");
            AssertCode(     // slgfr	r2,r12
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r12, word32, 0)",
                "2|L--|r2 = r2 -u CONVERT(v5, word32, word64)",
                "3|L--|CC = cond(r2)");
        }

        [Test]
        public void zSeriesRw_slgr()
        {
            Given_HexString("B90B001B");
            AssertCode(     // slgr	r1,r11
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 - r11",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_sll()
        {
            Given_HexString("89100001");
            AssertCode(     // sll	r1,00000001
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v5 = v4 << 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_sllg()
        {
            Given_HexString("EB110003000D");
            AssertCode(     // sllg	r1,r1,00000003
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = r1 << 3<i32>",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_sllk()
        {
            Given_HexString("EB15000100DF");
            AssertCode(     // sllk	r1,r5,00000001
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = SLICE(r5, word32, 0)",
                "2|L--|v6 = v4 << 1<i32>",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_slr()
        {
            Given_HexString("1F00");
            AssertCode(     // slr	r0,r0
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r0, word32, 0)",
                "2|L--|v5 = SLICE(r0, word32, 0)",
                "3|L--|v6 = v4 - v5", 
                "4|L--|r0 = SEQ(SLICE(r0, word32, 32), v6)",
                "5|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_slrk()
        {
            Given_HexString("B9FB DBB4");
            AssertCode(
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|v6 = SLICE(r13, word32, 0)",
                "3|L--|v8 = v4 -u v6", 
                "4|L--|r11 = SEQ(SLICE(r11, word32, 32), v8)",
                "5|L--|CC = cond(v8)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_sp()
        {
            Given_HexString("FB20FF71A7EB");
            AssertCode(     // sp	-143(3,r15),2027(1,r10)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_spm()
        {
            Given_HexString("0400");
            AssertCode(     // spm	r0,r0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_sr()
        {
            Given_HexString("1B95");
            AssertCode(     // sr	r9,r5
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r9, int32, 0)",
                "2|L--|v6 = SLICE(r5, int32, 0)",
                "3|L--|v7 = v4 - v6",
                "4|L--|r9 = SEQ(SLICE(r9, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_sra()
        {
            Given_HexString("8A20001F");
            AssertCode(     // sra	r2,0000001F
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r2, int32, 0)",
                "2|L--|v5 = v4 >> 31<i32>",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_srag()
        {
            Given_HexString("EB330003000A");
            AssertCode(     // srag	r3,r3,00000003
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r3 = r3 >> 3<i32>",
                "2|L--|CC = cond(r3)");
        }

        [Test]
        public void zSeriesRw_srak()
        {
            Given_HexString("EB42001F00DC");
            AssertCode(     // srak	r4,r2,0000001F
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v4 = SLICE(r2, int32, 0)",
                "2|L--|v6 = v4 >> 31<i32>",
                "3|L--|r4 = SEQ(SLICE(r4, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_srda()
        {
            Given_HexString("8E42C020");
            AssertCode(     // srda	r4,32(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_srdl()
        {
            Given_HexString("8C03EC26");
            AssertCode(     // srdl	r0,-986(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_srl()
        {
            Given_HexString("8820001F");
            AssertCode(     // srl	r2,0000001F
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v5 = v4 >>u 31<i32>",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v5)",
                "4|L--|CC = cond(v5)");
        }

        [Test]
        public void zSeriesRw_srlg()
        {
            Given_HexString("EB13003F000C");
            AssertCode(     // srlg	r1,r3,0000003F
                "0|L--|00100000(6): 2 instructions",
                "1|L--|r1 = r3 >>u 63<i32>",
                "2|L--|CC = cond(r1)");
        }

        [Test]
        public void zSeriesRw_srlk()
        {
            Given_HexString("EB12100000DE");
            AssertCode(     // srlk	r1,r2,(r1)
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v5 = SLICE(r2, word32, 0)",
                "2|L--|v6 = v5 >>u r1",
                "3|L--|r1 = SEQ(SLICE(r1, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_srp()
        {
            Given_HexString("F0F8000407F4");
            AssertCode(     // srp	4(16,r0),000007F4,08
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v2 = __shift_and_round_decimal(Mem0[r0 + 16:@@@], Mem0[000007F4:word32], 8<32>",
                "2|L--|Mem0[r0 + 16:@@@] = v2",
                "3|L--|CC = cond(v2)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_ssm()
        {
            Given_HexString("80009200");
            AssertCode(     // ssm	512(r9)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_st()
        {
            Given_HexString("5010B0A4");
            AssertCode(     // st	r1,164(r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + 164<i64>:word32] = SLICE(r1, word32, 0)");
        }

        [Test]
        public void zSeriesRw_stc()
        {
            Given_HexString("42A0F0A0");
            AssertCode(     // stc	r10,160(r15)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r15 + 160<i64>:byte] = SLICE(r10, byte, 0)");
        }

        [Test]
        public void zSeriesRw_stctl()
        {
            Given_HexString("B6EEE310");
            AssertCode(     // stctl	r14,784(r14)
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__store_control<word64,word32 *>(r14, &Mem0[r14 + 784<i64>:word32])");
        }

        [Test]
        public void zSeriesRw_std()
        {
            Given_HexString("6080E000");
            AssertCode(     // std	f8,(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r14:word64] = f8");
        }

        [Test]
        public void zSeriesRw_stg()
        {
            Given_HexString("E310F0000024");
            AssertCode(     // stg	r1,(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[r15:word64] = r1");
        }

        [Test]
        public void zSeriesRw_stgrl()
        {
            Given_HexString("C41B00000D0D");
            AssertCode(     // stgrl	r1,0000000000002028
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[0x00101A1A<p32>:word64] = r1");
        }

        [Test]
        public void zSeriesRw_sth()
        {
            Given_HexString("4065B904");
            AssertCode(     // sth	r6,-1788(r5,r11)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r11 + r5 + -1788<i64>:word16] = SLICE(r6, word16, 0)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_stm()
        {
            Given_HexString("9082EC28");
            AssertCode(     // stm	r8,r2,-984(r14)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_stmg()
        {
            Given_HexString("EB68F0300024");
            AssertCode(     // stmg	r6,r8,48(r15)
                "0|L--|00100000(6): 6 instructions",
                "1|L--|v4 = r15 + 48<i64>",
                "2|L--|Mem0[v4:word64] = r6",
                "3|L--|v4 = v4 + 8<i64>",
                "4|L--|Mem0[v4:word64] = r7",
                "5|L--|v4 = v4 + 8<i64>",
                "6|L--|Mem0[v4:word64] = r8");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_stoc()
        {
            Given_HexString("EBD3B13C00F3");
            AssertCode(     // stoc	r13,03,316(r11)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_strl()
        {
            Given_HexString("C41F0004BF03");
            AssertCode(     // strl	r1,0000000080098658
                "0|L--|00100000(6): 1 instructions",
                "1|L--|Mem0[0x00197E06<p32>:word32] = SLICE(r1, word32, 0)");
        }

        [Test]
        public void zSeriesRw_su()
        {
            Given_HexString("7FFFFF73");
            AssertCode(     // su	f15,-141(r15,r15)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f15, real32, 0)",
                "2|L--|v6 = v4 - Mem0[(r15 + r15) + -141<i64>:real32]",
                "3|L--|f15 = SEQ(SLICE(f15, word32, 32), v6)",
                "4|L--|CC = cond(v6)");
        }

        [Test]
        public void zSeriesRw_sur()
        {
            Given_HexString("3F17");
            AssertCode(     // sur	f1,f7
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(f1, real32, 0)",
                "2|L--|v6 = SLICE(f7, real32, 0)",
                "3|L--|v7 = v4 - v6",
                "4|L--|f1 = SEQ(SLICE(f1, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_svc()
        {
            Given_HexString("0A68");
            AssertCode(     // svc	68
                "0|T--|00100000(2): 1 instructions",
                "1|L--|__syscall<byte>(0x68<8>)");
        }

        [Test]
        public void zSeriesRw_swr()
        {
            Given_HexString("2F45");
            AssertCode(     // swr	f4,f5
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f4 = f4 - f5",
                "2|L--|CC = cond(f4)");
        }

        [Test]
        public void zSeriesRw_sxr()
        {
            Given_HexString("3706");
            AssertCode(     // sxr	f0,f6
                "0|L--|00100000(2): 2 instructions",
                "1|L--|f0_f1 = f0_f1 - f6_f7",
                "2|L--|CC = cond(f0_f1)");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_tcdb()
        {
            Given_HexString("ED2000300011");
            AssertCode(     // tcdb	f2,00000030
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_tceb()
        {
            Given_HexString("ED2000300010");
            AssertCode(     // tceb	f2,00000030
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_tcxb()
        {
            Given_HexString("ED1005550012");
            AssertCode(     // tcxb	f1,00000555
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_tm()
        {
            Given_HexString("91403148");
            AssertCode(     // tm	328(r3),40
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_tmy()
        {
            Given_HexString("EB01CFE3FF51");
            AssertCode(     // tmy	-29(r12),01
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_trace()
        {
            Given_HexString("9955C020");
            AssertCode(     // trace	r5,r5,32(r12)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_trtr()
        {
            Given_HexString("D003A7F4FF76");
            AssertCode(     // trtr	2036(4,r10),-138(r15)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_ts()
        {
            Given_HexString("93880002");
            AssertCode(     // ts	00000002
                "0|L--|00100000(4): 1 instructions",
                "1|L--|CC = __test_and_set<byte>(&Mem0[0x00000002<p32>:byte])");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_unpk()
        {
            Given_HexString("F3000002A784");
            AssertCode(     // unpk	2(1,r0),1924(1,r10)
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_unpka()
        {
            Given_HexString("EAFAA74902C4");
            AssertCode(     // unpka	1865(251,r10),000002C4
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("S390 instr")]
        public void zSeriesRw_unpku()
        {
            Given_HexString("E227A729007F");
            AssertCode(     // unpku	1833(40,r10),0000007F
                "0|L--|00100000(6): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void zSeriesRw_vll()
        {
            Given_HexString("E774099E5737");
            AssertCode(
                "0|L--|00100000(6): 1 instructions",
                "1|L--|v7 = __vll(r4, 0x0000099E<p32>)");
        }

        [Test]
        public void zSeriesRw_verim()
        {
            Given_HexString("E7D1B7B83472");
            AssertCode(
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v6 = v1",
                "2|L--|v7 = v11",
                "3|L--|v8 = 0xB8<8>",
                "4|L--|v13 = __verim<word64[2]>(v6, v7, v8)");
        }

        [Test]
        public void zSeriesRw_vfs()
        {
            Given_HexString("E785 21E9 36E2");
            AssertCode(
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v6 = v21",
                "2|L--|v7 = v2",
                "3|L--|v8 = __vfs<real64[2]>(v6, v7)");
        }

        [Test]
        public void zSeriesRw_vmahb()
        {
            Given_HexString("E749C60B02AB");
            AssertCode( // vmahb\tv20,v9,v12,v16
                "0|L--|00100000(6): 4 instructions",
                "1|L--|v7 = v9",
                "2|L--|v8 = v12",
                "3|L--|v9 = v16",
                "4|L--|v20 = __vmah<int8[16]>(v7, v8, v9)");
        }

        [Test]
        public void zSeriesRw_vmxlg()
        {
            Given_HexString("E7F0E01737FD");
            AssertCode(     // xr	r0,r1
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v6 = v0",
                "2|L--|v7 = v14",
                "3|L--|v15 = __vmx<word64[2]>(v6, v7)");
        }

        [Test]
        public void zSeriesRw_vpk()
        {
            Given_HexString("E7AF92791A94");
            AssertCode(
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v6 = v31",
                "2|L--|v7 = v25",
                "3|L--|v26 = __vpk<int16[8]>(v6, v7)");
        }

        [Test]
        [Ignore("Need m4 operand")]
        public void zSeriesRw_vscef()
        {
            Given_HexString("E72F840A2B1B");
            AssertCode(
            "@@@");
        }

        [Test]
        public void zSeriesRw_vsel()
        {
            Given_HexString("E76A0061B48D");
            AssertCode(
            "0|L--|00100000(6): 1 instructions",
            "1|L--|v22 = __vsel(v10, v16, v11)");
        }

        [Test]
        [Ignore("side effect")]
        public void zSeriesRw_vstl()
        {
            Given_HexString("E7BFB949933F8BFF8FFD087C0D7B75A2");
            AssertCode("@@@");
        }

        [Test]
        public void zSeriesRw_x()
        {
            Given_HexString("5725B904");
            AssertCode(     // x	r2,-1788(r5,r11)
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v7 = v4 ^ Mem0[(r11 + r5) + -1788<i64>:word32]",
                "3|L--|r2 = SEQ(SLICE(r2, word32, 32), v7)",
                "4|L--|CC = cond(v7)");
        }

        [Test]
        public void zSeriesRw_xc()
        {
            Given_HexString("D707F000F000");
            AssertCode(     // xc	(8,r15),(r15)
                "0|L--|00100000(6): 3 instructions",
                "1|L--|v4 = 0<8>",
                "2|L--|Mem0[r15:byte] = 0<8>",
                "3|L--|CC = cond(v4)");
        }

        [Test]
        public void zSeriesRw_xgr()
        {
            Given_HexString("B9820053");
            AssertCode(     // xgr	r5,r3
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = r5 ^ r3",
                "2|L--|CC = cond(r5)");
        }

        [Test]
        public void zSeriesRw_xr()
        {
            Given_HexString("1701");
            AssertCode(     // xr	r0,r1
                "0|L--|00100000(2): 5 instructions",
                "1|L--|v4 = SLICE(r0, word32, 0)",
                "2|L--|v6 = SLICE(r1, word32, 0)",
                "3|L--|v7 = v4 ^ v6",
                "4|L--|r0 = SEQ(SLICE(r0, word32, 32), v7)",
                "5|L--|CC = cond(v7)");
        }
    }
}
