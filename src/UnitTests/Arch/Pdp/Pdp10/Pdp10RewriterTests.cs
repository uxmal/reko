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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp10
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class Pdp10RewriterTests : RewriterTestBase
    {
        private readonly Pdp10Architecture arch;
        private readonly Address addr;

        public Pdp10RewriterTests()
        {
            this.arch = new Pdp10Architecture(CreateServiceContainer(), "pdp10", new Dictionary<string, object>());
            this.addr = Pdp10Architecture.Ptr18(0x0010_000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void Given_OctalWord(string octalWord)
        {
            var word = Pdp10Architecture.OctalStringToWord(octalWord);
            Given_MemoryArea(new Word36MemoryArea(addr, new[] { word }));
        }

        [Test]
        public void Pdp10Rw_addb()
        {
            Given_OctalWord("273140012045");
            AssertCode(     // addb	3,12045
                "0|L--|200000(1): 3 instructions",
                "1|L--|r3 = r3 + Mem0[0x012045<p18>:word36]",
                "2|L--|Mem0[0x012045<p18>:word36] = r3",
                "3|L--|C0C1VT = cond(r3)");
        }

        [Test]
        public void Pdp10Rw_addi()
        {
            Given_OctalWord("271040000074");
            AssertCode(     // addi	1,74
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = r1 + 0x3C<36>",
                "2|L--|C0C1VT = cond(r1)");
        }

        [Test]
        public void Pdp10Rw_addm()
        {
            Given_OctalWord("272100023406");
            AssertCode(     // addm	2,23406
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = r2 + Mem0[0x023406<p18>:word36]",
                "2|L--|Mem0[0x023406<p18>:word36] = v4",
                "3|L--|C0C1VT = cond(v4)");
        }

        [Test]
        public void Pdp10Rw_and()
        {
            Given_OctalWord("404040023426");
            AssertCode(     // and	1,23426
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = r1 & Mem0[0x023426<p18>:word36]");
        }

        [Test]
        public void Pdp10Rw_andca()
        {
            Given_OctalWord("410000000000");
            AssertCode(     // andca	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = ~r0 & r0");
        }

        [Test]
        public void Pdp10Rw_andcai()
        {
            Given_OctalWord("411300651236");
            AssertCode(     // andcai	6,651236(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r6 = ~r6 & 0x3529E<36>");
        }

        [Test]
        public void Pdp10Rw_andcam()
        {
            Given_OctalWord("412705000000");
            AssertCode(     // andcam	16,0(5)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = ~r14 & Mem0[r5:word36]",
                "2|L--|Mem0[r5:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_andcb()
        {
            Given_OctalWord("440700000006");
            AssertCode(     // andcb	16,6
                "0|L--|200000(1): 1 instructions",
                "1|L--|r14 = ~r14 & ~r6");
        }

        [Test]
        public void Pdp10Rw_andcm()
        {
            Given_OctalWord("420400001510");
            AssertCode(     // andcm	10,1510
                "0|L--|200000(1): 1 instructions",
                "1|L--|r8 = r8 & ~Mem0[0x001510<p18>:word36]");
        }

        [Test]
        public void Pdp10Rw_andcmi()
        {
            Given_OctalWord("421440000376");
            AssertCode(     // andcmi	11,376
                "0|L--|200000(1): 1 instructions",
                "1|L--|r9 = r9 & ~0xFE<36>");
        }

        [Test]
        public void Pdp10Rw_andcmm()
        {
            Given_OctalWord("422131442650");
            AssertCode(     // andcmm	2,@442650(11)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = r2 & ~Mem0[Mem0[r9 + 4294854056<i36>:word36]:word36]",
                "2|L--|Mem0[Mem0[r9 + 4294854056<i36>:word36]:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_andi()
        {
            Given_OctalWord("405140000177");
            AssertCode(     // andi	3,177
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = r3 & 0x7F<36>");
        }

        [Test]
        public void Pdp10Rw_andm()
        {
            Given_OctalWord("406640005413");
            AssertCode(     // andm	15,5413
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = r13 & Mem0[0x005413<p18>:word36]",
                "2|L--|Mem0[0x005413<p18>:word36] = v4");
        }

        [Test]
        public void Pdp10Rw_aoj()
        {
            Given_OctalWord("340404000701");
            AssertCode(     // aoj	10,701(4)
                "0|L--|200000(1): 2 instructions",
                "1|L--|r8 = r8 + 1<36>",
                "2|L--|C0C1VT = cond(r8)");
        }

        [Test]
        public void Pdp10Rw_aojge()
        {
            Given_OctalWord("345500001421");
            AssertCode(     // aojge	12,001421
                "0|T--|200000(1): 3 instructions",
                "1|L--|r10 = r10 + 1<36>",
                "2|L--|C0C1VT = cond(r10)",
                "3|T--|if (r10 >= 0<36>) branch 001421");
        }

        [Test]
        public void Pdp10Rw_aobjn()
        {
            Given_OctalWord("253040041215");
            AssertCode(     // aobjn	1,41215
                "0|L--|200000(1): 4 instructions",
                "1|L--|v3 = SLICE(r1, word18, 18) + 1<18>",
                "2|L--|v4 = SLICE(r1, word18, 0) + 1<18>",
                "3|L--|r1 = SEQ(v3, v4)",
                "4|T--|if (v3 < 0<18>) branch 041215");
        }

        [Test]
        public void Pdp10Rw_aoja()
        {
            Given_OctalWord("344100040462");
            AssertCode(     // aoja	2,040462
                "0|T--|200000(1): 3 instructions",
                "1|L--|r2 = r2 + 1<36>",
                "2|L--|C0C1VT = cond(r2)",
                "3|T--|goto 040462");
        }

        [Test]
        public void Pdp10Rw_aos()
        {
            Given_OctalWord("350040012502");
            AssertCode(     // aos	1,12502
                "0|L--|200000(1): 4 instructions",
                "1|L--|v3 = Mem0[0x012502<p18>:word36] + 1<36>",
                "2|L--|C0C1VT = cond(v3)",
                "3|L--|r1 = v3",
                "4|L--|Mem0[0x012502<p18>:word36] = v3");
        }

        [Test]
        public void Pdp10Rw_aosle()
        {
            Given_OctalWord("353473426555");
            AssertCode(     // aosle	11,@426555(13)
                "0|T--|200000(1): 5 instructions",
                "1|L--|v3 = Mem0[Mem0[r11 + 4294847853<i36>:word36]:word36] + 1<36>",
                "2|L--|Mem0[Mem0[r11 + 4294847853<i36>:word36]:word36] = v3",
                "3|L--|r9 = v3",
                "4|L--|C0C1VT = cond(v3)",
                "5|T--|if (v3 <= 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_ash()
        {
            Given_OctalWord("240040777774");
            AssertCode(     // ash	1,777774
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = r1 >> 4<36>");
        }

        [Test]
        public void Pdp10Rw_ashc()
        {
            Given_OctalWord("244000000003");
            AssertCode(     // ashc	0,3
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0_r1 = r0_r1 << 3<8>");
        }

        [Test]
        public void Pdp10Rw_blki()
        {
            Given_OctalWord("701000000000");
            AssertCode(     // blki	2,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_blki(2<36>, r0)");
        }

        [Test]
        public void Pdp10Rw_blt()
        {
            Given_OctalWord("251040023405");
            AssertCode(     // blt	1,23405
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_blt(r1, Mem0[0x023405<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_cai()
        {
            Given_OctalWord("300404000701");
            AssertCode(     // cai	10,701(4)
                "0|L--|200000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Pdp10Rw_caia()
        {
            Given_OctalWord("304000000000");
            AssertCode(     // caia	0,0
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_caie()
        {
            Given_OctalWord("302040000073");
            AssertCode(     // caie	1,73
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 == 0x3B<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_cail()
        {
            Given_OctalWord("301040000101");
            AssertCode(     // cail	1,101
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 < 0x41<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_call()
        {
            Given_OctalWord("040404000701");
            AssertCode(     // call	10,701(4)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_call(r8, Mem0[r4 + 449<i36>:word36])");
        }

        [Test]
        public void Pdp10Rw_calli()
        {
            Given_OctalWord("047040400071");
            AssertCode(     // calli	1,400071
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_calli(r1, Mem0[0x400071<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_cam()
        {
            Given_OctalWord("310000007226");
            AssertCode(     // cam	7226
                "0|L--|200000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Pdp10Rw_cama()
        {
            Given_OctalWord("314002403146");
            AssertCode(     // cama	403146(2)
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_came()
        {
            Given_OctalWord("312100042203");
            AssertCode(     // came	2,42203
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r2 == 0x4483<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_camle()
        {
            Given_OctalWord("313040023414");
            AssertCode(     // camle	1,23414
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 <= 0x270C<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_camn()
        {
            Given_OctalWord("316040042446");
            AssertCode(     // camn	1,42446
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 != 0x4526<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_close()
        {
            Given_OctalWord("070000000000");
            AssertCode(     // close	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_close(r0, r0)");
        }

        [Test]
        public void Pdp10Rw_coni()
        {
            Given_OctalWord("711640042563");
            AssertCode(     // coni	23,42563
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_coni(0x13<36>, Mem0[0x042563<p18>:word36])");
        }

        // cono
        [Test]
        public void Pdp10Rw_cono()
        {
            Given_OctalWord("777605000225");
            AssertCode(     // cono	177,225(5)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_cono(0x7F<36>, Mem0[r5 + 149<i36>:word36])");
        }

        [Test]
        public void Pdp10Rw_conso()
        {
            Given_OctalWord("777770000144");
            AssertCode(     // conso	177,@144(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_conso(0x7F<36>, Mem0[Mem0[r8 + 100<i36>:word36]:word36])");
        }

        [Test]
        public void Pdp10Rw_consz()
        {
            Given_OctalWord("777700037600");
            AssertCode(     // consz	177,37600
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_consz(0x7F<36>, Mem0[0x037600<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_dadd()
        {
            Given_OctalWord("114000001224");
            AssertCode(     // dadd	0,1224
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0_r1 = r0_r1 + Mem0[0x001224<p18>:word72]");
        }

        [Test]
        public void Pdp10Rw_datai()
        {
            Given_OctalWord("715040000000");
            AssertCode(     // datai	32,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = pdp10_datai(0x1A<36>)");
        }

        [Test]
        public void Pdp10Rw_datao()
        {
            Given_OctalWord("715140043112");
            AssertCode(     // datao	32,43112
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_datao(0x1A<36>, Mem0[0x043112<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_dfdv()
        {
            Given_OctalWord("113715126246");
            AssertCode(     // dfdv	16,126246(15)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r14_r15 = r14_r15 / Mem0[r13 + 44198<i36>:real72]");
        }

        [Test]
        public void Pdp10Rw_dpb()
        {
            Given_OctalWord("137040042426");
            AssertCode(     // dpb	1,42426
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_deposit_byte(r1, 0x4516<36>)");
        }

        [Test]
        public void Pdp10Rw_eqvi()
        {
            Given_OctalWord("445162454400");
            AssertCode(     // eqvi	3,@454400(2)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = ~(r3 ^ Mem0[r2 + 4294859008<i36>:word36])");
        }

        [Test]
        public void Pdp10Rw_eqvm()
        {
            Given_OctalWord("446353000000");
            AssertCode(     // eqvm	7,0(13)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = ~(r7 ^ Mem0[r11:word36])",
                "2|L--|Mem0[r11:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_fsb()
        {
            Given_OctalWord("150310421042");
            AssertCode(     // fsb	6,421042(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r6 = r6 - Mem0[r8 + 4294844962<i36>:real36]");
        }

        [Test]
        public void Pdp10Rw_halt()
        {
            Given_OctalWord("254200036322");
            AssertCode(     // halt
                "0|H--|200000(1): 1 instructions",
                "1|L--|pdp10_halt(Mem0[0x036322<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_hll()
        {
            Given_OctalWord("500040000004");
            AssertCode(     // hll	1,4
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(r4, word18, 18)",
                "2|L--|r1 = SEQ(v5, SLICE(r1, word18, 0))");
        }

        [Test]
        public void Pdp10Rw_hllm()
        {
            Given_OctalWord("502040023437");
            AssertCode(     // hllm	1,23437
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = SLICE(r1, word18, 0)",
                "2|L--|v5 = Mem0[0x023437<p18>:word36]",
                "3|L--|Mem0[0x023437<p18>:word36] = SEQ(SLICE(v5, word18, 18), v4)");
        }

        [Test]
        public void Pdp10Rw_hllo()
        {
            Given_OctalWord("520000000000");
            AssertCode(     // hllo	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = SEQ(SLICE(r0, word18, 0), 0x3FFFF<18>)");
        }

        [Test]
        public void Pdp10Rw_hllom()
        {
            Given_OctalWord("522453220205");
            AssertCode(     // hllom	11,220205(13)
                "0|L--|200000(1): 3 instructions",
                "1|L--|v5 = SLICE(Mem0[r11 + 73861<i36>:word36], word18, 0)",
                "2|L--|v6 = SEQ(v5, 0x3FFFF<18>)",
                "3|L--|Mem0[r11 + 73861<i36>:word36] = v6");
        }

        [Test]
        public void Pdp10Rw_hllz()
        {
            Given_OctalWord("510202025463");
            AssertCode(     // hllz	4,25463(2)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(Mem0[r2 + 11059<i36>:word36], word18, 18)",
                "2|L--|r4 = CONVERT(v5, word18, word36) << 0x12<8>");
        }

        [Test]
        public void Pdp10Rw_hllzm()
        {
            Given_OctalWord("512040023435");
            AssertCode(     // hllzm	1,23435
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = SLICE(r1, word18, 18)",
                "2|L--|v5 = CONVERT(v4, word18, word36) << 0x12<8>",
                "3|L--|Mem0[0x023435<p18>:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_hllzs()
        {
            Given_OctalWord("513000000016");
            AssertCode(     // hllzs	0,16
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(r0, word18, 18)",
                "2|L--|r14 = CONVERT(v5, word18, word36) << 0x12<8>");
        }

        [Test]
        public void Pdp10Rw_hlr()
        {
            Given_OctalWord("544040000004");
            AssertCode(     // hlr	1,4
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(r4, word18, 18)",
                "2|L--|r1 = SEQ(v5, SLICE(r1, word18, 0))");
        }

        [Test]
        public void Pdp10Rw_hlre()
        {
            Given_OctalWord("574500000116");
            AssertCode(     // hlre	12,116
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(Mem0[0x000116<p18>:word36], word18, 18)",
                "2|L--|r10 = CONVERT(v4, word18, int36)");
        }

        [Test]
        public void Pdp10Rw_hlrem()
        {
            Given_OctalWord("576200000002");
            AssertCode(     // hlrem	4,2
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(r4, word18, 18)",
                "2|L--|r2 = CONVERT(v4, word18, int36)");
        }

        [Test]
        public void Pdp10Rw_hlrm()
        {
            Given_OctalWord("546350000003");
            AssertCode(     // hlrm	7,3(10)
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = SLICE(r7, word18, 18)",
                "2|L--|v5 = SLICE(Mem0[r8 + 3<i36>:word36], word18, 18)",
                "3|L--|Mem0[r8 + 3<i36>:word36] = SEQ(v5, v4)");
        }

        [Test]
        public void Pdp10Rw_hlrz()
        {
            Given_OctalWord("554200000002");
            AssertCode(     // hlrz	4,2
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(r2, word18, 18)",
                "2|L--|r4 = CONVERT(v5, word18, word36)");
        }

        [Test]
        public void Pdp10Rw_hrl()
        {
            Given_OctalWord("504700000007");
            AssertCode(     // hrl	16,7
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(r7, word18, 0)",
                "2|L--|r14 = SEQ(v5, SLICE(r14, word18, 0))");
        }

        [Test]
        public void Pdp10Rw_hrli()
        {
            Given_OctalWord("505040777743");
            AssertCode(     // hrli	1,777743
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(0x3FFE3<36>, word18, 0)",
                "2|L--|Mem0[0x777743<p18>:word36] = SEQ(v4, SLICE(r1, word18, 0))");
        }

        [Test]
        public void Pdp10Rw_hrls()
        {
            Given_OctalWord("507000000016");
            AssertCode(     // hrls	0,16
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(r14, word18, 0)",
                "2|L--|r14 = SEQ(v4, v4)");
        }

        [Test]
        public void Pdp10Rw_hrlzm()
        {
            Given_OctalWord("516211753500");
            AssertCode(     // hrlzm	4,753500(11)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(r4, word18, 0)",
                "2|L--|Mem0[r9 + 4294956864<i36>:word36] = CONVERT(v4, word18, word36)");
        }

        [Test]
        public void Pdp10Rw_hrr()
        {
            Given_OctalWord("540141032466");
            AssertCode(     // hrr	3,32466(1)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = SLICE(Mem0[r1 + 13622<i36>:word36], word18, 0)",
                "2|L--|r3 = SEQ(SLICE(r3, word18, 18), v5)");
        }

        [Test]
        public void Pdp10Rw_hrrm()
        {
            Given_OctalWord("542040035733");
            AssertCode(     // hrrm	1,35733
                "0|L--|200000(1): 3 instructions",
                "1|L--|v4 = SLICE(r1, word18, 0)",
                "2|L--|v5 = Mem0[0x035733<p18>:word36]",
                "3|L--|Mem0[0x035733<p18>:word36] = SEQ(SLICE(v5, word18, 18), v4)");
        }


        [Test]
        public void Pdp10Rw_hrrz()
        {
            Given_OctalWord("550000023443");
            AssertCode(     // hrrz	0,23443
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(Mem0[0x023443<p18>:word36], word18, 0)",
                "2|L--|r0 = CONVERT(v4, word18, word36)");
        }

        [Test]
        public void Pdp10Rw_hrrzm()
        {
            Given_OctalWord("552040023413");
            AssertCode(     // hrrzm	1,23413
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(r1, word18, 0)",
                "2|L--|Mem0[0x023413<p18>:word36] = CONVERT(v4, word18, word36)");
        }

        [Test]
        public void Pdp10Rw_ibp()
        {
            Given_OctalWord("133000012502");
            AssertCode(     // ibp	0,12502
                "0|L--|200000(1): 2 instructions",
                "1|L--|v3 = pdp10_inc_byte_ptr(Mem0[0x012502<p18>:word36])",
                "2|L--|Mem0[0x012502<p18>:word36] = v3");
        }

        [Test]
        public void Pdp10Rw_idivb()
        {
            Given_OctalWord("233575360400");
            AssertCode(     // idivb	13,@360400(15)
                "0|L--|200000(1): 6 instructions",
                "1|L--|v6 = r11 / Mem0[r13 + 123136<i36>:word36]",
                "2|L--|r11 = v6",
                "3|L--|v7 = v6",
                "4|L--|Mem0[r13 + 123136<i36>:word36] = v7",
                "5|L--|r12 = r11 %s Mem0[r13 + 123136<i36>:word36]",
                "6|L--|VTND = cond(r11)");
        }

        [Test]
        public void Pdp10Rw_idivi()
        {
            Given_OctalWord("231040002000");
            AssertCode(     // idivi	1,2000
                "0|L--|200000(1): 3 instructions",
                "1|L--|r1 = r1 / 0x400<36>",
                "2|L--|r2 = r1 %s 0x400<36>",
                "3|L--|VTND = cond(r1)");
        }

        [Test]
        public void Pdp10Rw_idpb()
        {
            Given_OctalWord("136140000002");
            AssertCode(     // idpb	3,2
                "0|L--|200000(1): 1 instructions",
                "1|L--|r2 = pdp10_inc_ptr_deposit_byte(r2, r3)");
        }

        [Test]
        public void Pdp10Rw_ildb()
        {
            Given_OctalWord("134100000001");
            AssertCode(     // ildb	2,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|r2 = pdp10_inc_byte_ptr_and_load(1<36>)");
        }

        [Test]
        public void Pdp10Rw_imul()
        {
            Given_OctalWord("220040042201");
            AssertCode(     // imul	1,42201
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = r1 *s Mem0[0x042201<p18>:word36]",
                "2|L--|VT = cond(r1)");
        }

        [Test]
        public void Pdp10Rw_imuli()
        {
            Given_OctalWord("221040000014");
            AssertCode(     // imuli	1,14
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = r1 *s 0xC<36>",
                "2|L--|VT = cond(r1)");
        }

        [Test]
        public void Pdp10Rw_inbuf()
        {
            Given_OctalWord("064000000000");
            AssertCode(     // inbuf	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_inbuf(r0, r0)");
        }

        [Test]
        public void Pdp10Rw_initi()
        {
            Given_OctalWord("041000000000");
            AssertCode(     // initi	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_initi(r0, r0)");
        }

        [Test]
        public void Pdp10Rw_jrst()
        {
            Given_OctalWord("254000034726");
            AssertCode(     // jrst	034726
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 034726");
        }

        [Test]
        public void Pdp10Rw_jrst_indirect()
        {
            Given_OctalWord("254020041464");
            AssertCode(     // jrst	@41464
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto Mem0[0x4334<36>:word36]");
        }

        [Test]
        public void Pdp10Rw_jsp()
        {
            Given_OctalWord("265011447646");
            AssertCode(     // jsp	0,447646(11)
                "0|T--|200000(1): 2 instructions",
                "1|L--|r0 = 200001",
                "2|T--|goto r9 + 4294856614<i36>");
        }

        [Test]
        public void Pdp10Rw_jsys()
        {
            Given_OctalWord("104212663126");
            AssertCode(     // jsys	4,663126(12)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_jsys(r4, Mem0[r10 + 4294927958<i36>:word36])");
        }

        [Test]
        public void Pdp10Rw_jsr()
        {
            Given_OctalWord("264000002061");
            AssertCode(     // jsr	002061
                "0|T--|200000(1): 2 instructions",
                "1|L--|Mem0[0x002061<p18>:word36] = 200001",
                "2|T--|goto 0x002061<p18> + 1<18>");
        }

        [Test]
        public void Pdp10Rw_jumpe()
        {
            Given_OctalWord("322124034662");
            AssertCode(     // jumpe	2,@34662(4)
                "0|T--|200000(1): 2 instructions",
                "1|T--|if (r2 != 0<36>) branch 200001",
                "2|T--|goto Mem0[r4 + 14770<i36>:word36]");
        }

        [Test]
        public void Pdp10Rw_jumpn()
        {
            Given_OctalWord("326100001424");
            AssertCode(     // jumpn	2,001424
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r2 != 0<36>) branch 001424");
        }

        [Test]
        public void Pdp10Rw_ldb()
        {
            Given_OctalWord("135040042470");
            AssertCode(     // ldb	1,42470
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = pdp10_load_byte(0x4538<36>)");
        }

        [Test]
        public void Pdp10Rw_lookup()
        {
            Given_OctalWord("076140023421");
            AssertCode(     // lookup	3,23421
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_lookup(r3, Mem0[0x023421<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_lsh()
        {
            Given_OctalWord("242200777772");
            AssertCode(     // lsh	4,777772
                "0|L--|200000(1): 1 instructions",
                "1|L--|r4 = r4 >>u 6<8>");
        }

        [Test]
        public void Pdp10Rw_luuo02()
        {
            Given_OctalWord("002000043017");
            AssertCode(     // luuo02	0,43017
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_luuo(2<36>, r0, Mem0[0x043017<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_move()
        {
            Given_OctalWord("200300042175");
            AssertCode(     // move	6,42175
                "0|L--|200000(1): 1 instructions",
                "1|L--|r6 = Mem0[0x042175<p18>:word36]");
        }

        [Test]
        public void Pdp10Rw_movei()
        {
            Given_OctalWord("201440777777");
            AssertCode(     // movei	11,777777
                "0|L--|200000(1): 1 instructions",
                "1|L--|r9 = 0x3FFFF<36>");
        }

        [Test]
        public void Pdp10Rw_movem()
        {
            Given_OctalWord("202040023433");
            AssertCode(     // movem	1,23433
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[0x023433<p18>:word36] = r1");
        }

        [Test]
        public void Pdp10Rw_movm()
        {
            Given_OctalWord("214240007134");
            AssertCode(     // movm	5,7134
                "0|L--|200000(1): 1 instructions",
                "1|L--|r5 = abs(Mem0[0x007134<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_movmm()
        {
            Given_OctalWord("216470400000");
            AssertCode(     // movmm	11,@400000(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[Mem0[r8 + 4294836224<i36>:word36]:word36] = abs(r9)");
        }

        [Test]
        public void Pdp10Rw_movn()
        {
            Given_OctalWord("210100000002");
            AssertCode(     // movn	2,2
                "0|L--|200000(1): 2 instructions",
                "1|L--|r2 = -r2",
                "2|L--|C1VT = cond(r2)");
        }

        [Test]
        public void Pdp10Rw_movnm()
        {
            Given_OctalWord("212025634753");
            AssertCode(     // movnm	0,@634753(5)
                "0|L--|200000(1): 2 instructions",
                "1|L--|Mem0[Mem0[r5 + 4294916587<i36>:word36]:word36] = -r0",
                "2|L--|C1VT = cond(Mem0[Mem0[r5 + 4294916587<i36>:word36]:word36])");
        }

        [Test]
        public void Pdp10Rw_movns()
        {
            Given_OctalWord("213140000003");
            AssertCode(     // movns	3,3
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = -r3",
                "2|L--|r3 = v4");
        }

        [Test]
        public void Pdp10Rw_movs()
        {
            Given_OctalWord("204140023424");
            AssertCode(     // movs	3,23424
                "0|L--|200000(1): 4 instructions",
                "1|L--|v4 = Mem0[0x023424<p18>:word36]",
                "2|L--|v5 = SLICE(v4, word18, 18)",
                "3|L--|v6 = SLICE(v4, word18, 0)",
                "4|L--|r3 = SEQ(v6, v5)");
        }

        [Test]
        public void Pdp10Rw_movsi()
        {
            Given_OctalWord("205040777000");
            AssertCode(     // movsi	1,777000
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = 0xFF8000000<36>");
        }

        [Test]
        public void Pdp10Rw_muli()
        {
            Given_OctalWord("225200000400");
            AssertCode(     // muli	4,400
                "0|L--|200000(1): 1 instructions",
                "1|L--|r4 = r4 *u 0x100<36>");
        }

        [Test]
        public void Pdp10Rw_muuo42()
        {
            Given_OctalWord("042160210421");
            AssertCode(     // muuo42	3,@210421
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_muuo(0x22<36>, r3, Mem0[0x210421<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_or()
        {
            Given_OctalWord("434700000003");
            AssertCode(     // or	16,3
                "0|L--|200000(1): 1 instructions",
                "1|L--|r14 = r14 | r3");
        }

        [Test]
        public void Pdp10Rw_orcab()
        {
            Given_OctalWord("457043500004");
            AssertCode(     // orcab	1,500004(3)
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = ~r1 | Mem0[r3 + 4294868996<i36>:word36]",
                "2|L--|Mem0[r3 + 4294868996<i36>:word36] = r1");
        }

        [Test]
        public void Pdp10Rw_orcam()
        {
            Given_OctalWord("456166000003");
            AssertCode(     // orcam	3,@3(6)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = ~r3 | Mem0[Mem0[r6 + 3<i36>:word36]:word36]",
                "2|L--|Mem0[Mem0[r6 + 3<i36>:word36]:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_orcb()
        {
            Given_OctalWord("470000000001");
            AssertCode(     // orcb	0,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = ~r0 | ~r1");
        }

        [Test]
        public void Pdp10Rw_ori()
        {
            Given_OctalWord("435640000060");
            AssertCode(     // ori	15,60
                "0|L--|200000(1): 1 instructions",
                "1|L--|r13 = r13 | 0x30<36>");
        }

        [Test]
        public void Pdp10Rw_orm()
        {
            Given_OctalWord("436700001514");
            AssertCode(     // orm	16,1514
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = r14 | Mem0[0x001514<p18>:word36]",
                "2|L--|Mem0[0x001514<p18>:word36] = v4");
        }

        [Test]
        public void Pdp10Rw_orcm()
        {
            Given_OctalWord("464144002013");
            AssertCode(     // orcm	3,2013(4)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = r3 | ~Mem0[r4 + 1035<i36>:word36]");
        }

        [Test]
        public void Pdp10Rw_orcmi()
        {
            Given_OctalWord("465170000003");
            AssertCode(     // orcmi	3,@3(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = r3 | ~Mem0[r8 + 3<i36>:word36]");
        }

        [Test]
        public void Pdp10Rw_orcmm()
        {
            Given_OctalWord("466100753052");
            AssertCode(     // orcmm	2,753052
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = r2 | ~Mem0[0x753052<p18>:word36]",
                "2|L--|Mem0[0x753052<p18>:word36] = v4");
        }

        [Test]
        public void Pdp10Rw_pop()
        {
            Given_OctalWord("262740000001");
            AssertCode(     // pop	17,1
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = Mem0[r15:word36]",
                "2|L--|r15 = r15 - 1<i36>");
        }

        [Test]
        public void Pdp10Rw_popj()
        {
            Given_OctalWord("263300000000");
            AssertCode(     // popj	6,0
                "0|R--|200000(1): 1 instructions",
                "1|R--|return (1,0)");
        }

        [Test]
        public void Pdp10Rw_push()
        {
            Given_OctalWord("261300000001");
            AssertCode(     // push	6,1
                "0|L--|200000(1): 2 instructions",
                "1|L--|r6 = r6 + 1<i36>",
                "2|L--|Mem0[r6:word36] = r1");
        }

        [Test]
        public void Pdp10Rw_pushj()
        {
            Given_OctalWord("260300041266");
            AssertCode(     // pushj	6,041266
                "0|T--|200000(1): 1 instructions",
                "1|T--|call 041266 (1)");
        }

        [Test]
        public void Pdp10Rw_rename()
        {
            Given_OctalWord("055040023434");
            AssertCode(     // rename	1,23434
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_rename(r1, Mem0[0x023434<p18>:word36])");
        }

        [Test]
        public void Pdp10Rw_setai()
        {
            Given_OctalWord("425011444646");
            AssertCode(     // setai	0,444646(11)
                "0|L--|200000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Pdp10Rw_setm()
        {
            Given_OctalWord("414002000000");
            AssertCode(     // setm	0,0(2)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = Mem0[r2:word36]");
        }

        [Test]
        public void Pdp10Rw_setmm()
        {
            Given_OctalWord("416031623650");
            AssertCode(     // setmm	0,@623650(11)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v5 = Mem0[Mem0[r9 + 4294911912<i36>:word36]:word36]",
                "2|L--|Mem0[Mem0[r9 + 4294911912<i36>:word36]:word36] = v5");
        }

        [Test]
        public void Pdp10Rw_seto()
        {
            Given_OctalWord("474040000000");
            AssertCode(     // seto	1,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = 0xFFFFFFFFF<36>");
        }


        [Test]
        public void Pdp10Rw_setob()
        {
            Given_OctalWord("477700000220");
            AssertCode(     // setob	16,220
                "0|L--|200000(1): 2 instructions",
                "1|L--|r14 = 0xFFFFFFFFF<36>",
                "2|L--|Mem0[0x000220<p18>:word36] = r14");
        }

        [Test]
        public void Pdp10Rw_setom()
        {
            Given_OctalWord("476000023375");
            AssertCode(     // setom	0,23375
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = 0xFFFFFFFFF<36>",
                "2|L--|Mem0[0x023375<p18>:word36] = v4");
        }

        [Test]
        public void Pdp10Rw_setz()
        {
            Given_OctalWord("400404000701");
            AssertCode(     // setz	10
                "0|L--|200000(1): 1 instructions",
                "1|L--|r8 = 0<36>");
        }

        [Test]
        public void Pdp10Rw_setzm()
        {
            Given_OctalWord("402000023407");
            AssertCode(     // setzm	23407
                "0|L--|200000(1): 3 instructions",
                "1|L--|v3 = 0<36>",
                "2|L--|v4 = v3",
                "3|L--|Mem0[0x023407<p18>:word36] = v4");
        }

        [Test]
        public void Pdp10Rw_skipa()
        {
            Given_OctalWord("334340014361");
            AssertCode(     // skipa	7,14361
                "0|T--|200000(1): 2 instructions",
                "1|L--|r7 = Mem0[0x014361<p18>:word36]",
                "2|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_skipa_noArgs()
        {
            Given_OctalWord("334000014361");
            AssertCode(     // skipa	14361
                "0|T--|200000(1): 1 instructions",
                "1|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_skipe()
        {
            Given_OctalWord("332000042131");
            AssertCode(     // skipe	42131
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (Mem0[0x042131<p18>:word36] == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_skipe_2_ops()
        {
            Given_OctalWord("332400042131");
            AssertCode(     // skipe	1,42131
                "0|T--|200000(1): 3 instructions",
                "1|L--|v3 = Mem0[0x042131<p18>:word36]",
                "2|L--|r8 = v3",
                "3|T--|if (Mem0[0x042131<p18>:word36] == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_skipn()
        {
            Given_OctalWord("336000042126");
            AssertCode(     // skipn	42126
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (Mem0[0x042126<p18>:word36] != 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_sojge()
        {
            Given_OctalWord("365500001167");
            AssertCode(     // sojge	12,001167
                "0|T--|200000(1): 3 instructions",
                "1|L--|r10 = r10 - 1<36>",
                "2|L--|C0C1VT = cond(r10)",
                "3|T--|if (r10 >= 0<36>) branch 001167");
        }

        [Test]
        public void Pdp10Rw_sojl()
        {
            Given_OctalWord("361200034776");
            AssertCode(     // sojl	4,034776
                "0|T--|200000(1): 3 instructions",
                "1|L--|r4 = r4 - 1<36>",
                "2|L--|C0C1VT = cond(r4)",
                "3|T--|if (r4 < 0<36>) branch 034776");
        }

        [Test]
        public void Pdp10Rw_sos()
        {
            Given_OctalWord("370000023413");
            AssertCode(     // sos	23413
                "0|L--|200000(1): 3 instructions",
                "1|L--|v3 = Mem0[0x023413<p18>:word36] - 1<36>",
                "2|L--|Mem0[0x023413<p18>:word36] = v3",
                "3|L--|C0C1VT = cond(v3)");
        }

        [Test]
        public void Pdp10Rw_sosle()
        {
            Given_OctalWord("373000006531");
            AssertCode(     // sosle	6531
                "0|T--|200000(1): 4 instructions",
                "1|L--|v3 = Mem0[0x006531<p18>:word36] - 1<36>",
                "2|L--|Mem0[0x006531<p18>:word36] = v3",
                "3|L--|C0C1VT = cond(v3)",
                "4|T--|if (v3 <= 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_sosle_ac()
        {
            Given_OctalWord("373040024022");
            AssertCode(     // sosle	1,6531
                "0|T--|200000(1): 5 instructions",
                "1|L--|v3 = Mem0[0x024022<p18>:word36] - 1<36>",
                "2|L--|Mem0[0x024022<p18>:word36] = v3",
                "3|L--|r1 = v3",
                "4|L--|C0C1VT = cond(v3)",
                "5|T--|if (v3 <= 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_subi()
        {
            Given_OctalWord("275040000040");
            AssertCode(     // subi	1,40
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = r1 - 0x20<36>",
                "2|L--|C0C1VT = cond(r1)");
        }

        [Test]
        public void Pdp10Rw_subm()
        {
            Given_OctalWord("276600000011");
            AssertCode(     // subm	14,11
                "0|L--|200000(1): 2 instructions",
                "1|L--|r9 = r12 - r9",
                "2|L--|C0C1VT = cond(r9)");
        }

        [Test]
        public void Pdp10Rw_tlc()
        {
            Given_OctalWord("641200400000");
            AssertCode(     // tlc	4,400000
                "0|L--|200000(1): 1 instructions",
                "1|L--|r4 = r4 ^ 0x800000000<36>");
        }

        [Test]
        public void Pdp10Rw_tlnn()
        {
            Given_OctalWord("607040420000");
            AssertCode(     // tlnn	1,420000
                "0|T--|200000(1): 1 instructions",
                "1|T--|if ((r1 >>u 0x12<8> & 0x22000<36>) != 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_tlne()
        {
            Given_OctalWord("603040400000");
            AssertCode(     // tlne	1,400000
                "0|T--|200000(1): 1 instructions",
                "1|T--|if ((r1 >>u 0x12<8> & 0x20000<36>) == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_tlo()
        {
            Given_OctalWord("661040000001");
            AssertCode(     // tlo	1,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = r1 | 0x40000<36>");
        }

        [Test]
        public void Pdp10Rw_tloa()
        {
            Given_OctalWord("665400200000");
            AssertCode(     // tloa	10,200000
                "0|T--|200000(1): 2 instructions",
                "1|L--|r8 = r8 | 0x400000000<36>",
                "2|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_tlz()
        {
            Given_OctalWord("621400000004");
            AssertCode(     // tlz	10,4
                "0|L--|200000(1): 1 instructions",
                "1|L--|r8 = r8 & ~0x100000<36>");
        }

        [Test]
        public void Pdp10Rw_tlza()
        {
            Given_OctalWord("625040000001");
            AssertCode(     // tlza	1,1
                "0|T--|200000(1): 2 instructions",
                "1|L--|r1 = r1 & ~0x40000<36>",
                "2|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_trnn()
        {
            Given_OctalWord("606440000400");
            AssertCode(     // trnn	11,400
                "0|T--|200000(1): 1 instructions",
                "1|T--|if ((r9 & 0x3FFFF<18> & 0x100<36>) != 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_trne()
        {
            Given_OctalWord("602440000001");
            AssertCode(     // trne	11,1
                "0|T--|200000(1): 1 instructions",
                "1|T--|if ((r9 & 0x3FFFF<18> & 1<36>) == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_tro()
        {
            Given_OctalWord("660440200000");
            AssertCode(     // tro	11,200000
                "0|L--|200000(1): 1 instructions",
                "1|L--|r9 = r9 | 0x10000<36>");
        }

        [Test]
        public void Pdp10Rw_troa()
        {
            Given_OctalWord("664440000020");
            AssertCode(     // troa	11,20
                "0|T--|200000(1): 2 instructions",
                "1|L--|r9 = r9 | 0x10<36>",
                "2|T--|goto 200002");
        }

        [Test]
        public void Pdp10Rw_trz()
        {
            Given_OctalWord("620040000600");
            AssertCode(     // trz	1,600
                "0|L--|200000(1): 1 instructions",
                "1|L--|r1 = r1 & ~0x180<36>");
        }

        [Test]
        public void Pdp10Rw_trze()
        {
            Given_OctalWord("622040000200");
            AssertCode(     // trze	1,200
                "0|T--|200000(1): 3 instructions",
                "1|L--|v4 = r1",
                "2|L--|r1 = v4 & ~0x80<36>",
                "3|T--|if ((v4 & 0x80<36>) == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_trzn()
        {
            Given_OctalWord("626440020000");
            AssertCode(     // trzn	11,20000
                "0|T--|200000(1): 3 instructions",
                "1|L--|v4 = r9",
                "2|L--|r9 = v4 & ~0x2000<36>",
                "3|T--|if ((v4 & 0x2000<36>) != 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_ttcall()
        {
            Given_OctalWord("051300000001");
            AssertCode(     // ttcall	6,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_ttcall(r6, r1)");
        }

        [Test]
        public void Pdp10Rw_ujen()
        {
            Given_OctalWord("100000000000");
            AssertCode(     // ujen	0,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_ujen(r0, r0)");
        }

        [Test]
        public void Pdp10Rw_xct()
        {
            Given_OctalWord("256007042677");
            AssertCode(     // xct	0,42677(7)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_xct(r0, r7 + 17855<i36>)");
        }

        [Test]
        public void Pdp10Rw_xor()
        {
            Given_OctalWord("430000000711");
            AssertCode(     // xor	0,711
                "0|L--|200000(1): 1 instructions",
                "1|L--|r0 = r0 ^ Mem0[0x000711<p18>:word36]");
        }

        [Test]
        public void Pdp10Rw_xori()
        {
            Given_OctalWord("431140000040");
            AssertCode(     // xori	3,40
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = r3 ^ 0x20<36>");
        }

#if NYI
        // Feel free to complete the following less common PDP-10 instruction unit tests.

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        // orcmm


        // conso



        // ujen


        // cam


        // eqvi


        // movnm


        // andcai


        // inbuf


        // luuo20


        // consz


        // andi
  

        // tlz


        // setz


        // aoj


        // cai


        // fad


        // call


        // setm


        // cama


        // setcmb
        [Test]
        public void Pdp10Rw_setcmb()
        {
            Given_OctalWord("463326101470");
            AssertCode(     // setcmb	6,@101470(6)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fsb


        // jsys


        // muuo42
   

        // muuo54


        // tlzn
        [Test]
        public void Pdp10Rw_tlzn()
        {
            Given_OctalWord("627400010000");
            AssertCode(     // tlzn	10,10000
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // jumpn


        // tloa

        // exch
        [Test]
        public void Pdp10Rw_exch()
        {
            Given_OctalWord("250700012047");
            AssertCode(     // exch	16,12047
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fsc
        [Test]
        public void Pdp10Rw_fsc()
        {
            Given_OctalWord("132700000233");
            AssertCode(     // fsc	16,233
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fmpr
        [Test]
        public void Pdp10Rw_fmpr()
        {
            Given_OctalWord("164700012351");
            AssertCode(     // fmpr	16,12351
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fadm
        [Test]
        public void Pdp10Rw_fadm()
        {
            Given_OctalWord("142700012046");
            AssertCode(     // fadm	16,12046
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fadrm
        [Test]
        public void Pdp10Rw_fadrm()
        {
            Given_OctalWord("146140012046");
            AssertCode(     // fadrm	3,12046
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fdvr
        [Test]
        public void Pdp10Rw_fdvr()
        {
            Given_OctalWord("174700012351");
            AssertCode(     // fdvr	16,12351
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // sojle
        [Test]
        public void Pdp10Rw_sojle()
        {
            Given_OctalWord("363700001076");
            AssertCode(     // sojle	16,001076
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // addb


        // movni
        [Test]
        public void Pdp10Rw_movni()
        {
            Given_OctalWord("211452777764");
            AssertCode(     // movni	11,777764(12)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }



        // or


        // sojg


        // sojge


        // hlre


        // aojge


        // jumpge


        // tdne
        [Test]
        public void Pdp10Rw_tdne()
        {
            Given_OctalWord("612456000000");
            AssertCode(     // tdne	11,0(16)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // sosge
        [Test]
        public void Pdp10Rw_sosge()
        {
            Given_OctalWord("375340000525");
            AssertCode(     // sosge	7,525
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrl



        // tdnn
        [Test]
        public void Pdp10Rw_tdnn()
        {
            Given_OctalWord("616356000000");
            AssertCode(     // tdnn	7,0(16)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // orm


        // andcm


        // setca
        [Test]
        public void Pdp10Rw_setca()
        {
            Given_OctalWord("450240000000");
            AssertCode(     // setca	5,0
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // rot
        [Test]
        public void Pdp10Rw_rot()
        {
            Given_OctalWord("241300777772");
            AssertCode(     // rot	6,777772
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // lshc
        [Test]
        public void Pdp10Rw_lshc()
        {
            Given_OctalWord("246240777772");
            AssertCode(     // lshc	5,777772
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // setcmm
        [Test]
        public void Pdp10Rw_setcmm()
        {
            Given_OctalWord("462000001513");
            AssertCode(     // setcmm	0,1513
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tlze
        [Test]
        public void Pdp10Rw_tlze()
        {
            Given_OctalWord("623400000200");
            AssertCode(     // tlze	10,200
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrrzs
        [Test]
        public void Pdp10Rw_hrrzs()
        {
            Given_OctalWord("553000012047");
            AssertCode(     // hrrzs	0,12047
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrlzs
        [Test]
        public void Pdp10Rw_hrlzs()
        {
            Given_OctalWord("517000000006");
            AssertCode(     // hrlzs	0,6
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // movss
        [Test]
        public void Pdp10Rw_movss()
        {
            Given_OctalWord("207000012047");
            AssertCode(     // movss	0,12047
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // jrstf
        [Test]
        public void Pdp10Rw_jrstf()
        {
            Given_OctalWord("254120002061");
            AssertCode(     // jrstf	@2061
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // setzb
        [Test]
        public void Pdp10Rw_setzb()
        {
            Given_OctalWord("403400003266");
            AssertCode(     // setzb	10,3266
                "0|L--|200000(1): 3 instructions",
                "1|L--|200000(1): 1 instructions",
                "2|L--|200000(1): 1 instructions",
                "3|L--|@@@");
        }

        // hlro
        [Test]
        public void Pdp10Rw_hlro()
        {
            Given_OctalWord("564700000116");
            AssertCode(     // hlro	16,116
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrre
        [Test]
        public void Pdp10Rw_hrre()
        {
            Given_OctalWord("570700000012");
            AssertCode(     // hrre	16,12
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }


        // dfmp
        [Test]
        public void Pdp10Rw_dfmp()
        {
            Given_OctalWord("112400546064");
            AssertCode(     // dfmp	10,546064
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // dmove
        [Test]
        public void Pdp10Rw_dmove()
        {
            Given_OctalWord("120000004443");
            AssertCode(     // dmove	0,4443
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // dmovn
        [Test]
        public void Pdp10Rw_dmovn()
        {
            Given_OctalWord("121000656443");
            AssertCode(     // dmovn	0,656443
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fix
        [Test]
        public void Pdp10Rw_fix()
        {
            Given_OctalWord("122000556443");
            AssertCode(     // fix	0,556443
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // extend
        [Test]
        public void Pdp10Rw_extend()
        {
            Given_OctalWord("123000556455");
            AssertCode(     // extend	0,556455
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // ufa


        // dfn
        [Test]
        public void Pdp10Rw_dfn()
        {
            Given_OctalWord("131000445342");
            AssertCode(     // dfn	0,445342
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fadb
        [Test]
        public void Pdp10Rw_fadb()
        {
            Given_OctalWord("143000004460");
            AssertCode(     // fadb	0,4460
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fadr
        [Test]
        public void Pdp10Rw_fadr()
        {
            Given_OctalWord("144400446353");
            AssertCode(     // fadr	10,446353
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fadrb
        [Test]
        public void Pdp10Rw_fadrb()
        {
            Given_OctalWord("147000442121");
            AssertCode(     // fadrb	0,442121
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fsbl
        [Test]
        public void Pdp10Rw_fsbl()
        {
            Given_OctalWord("151000444444");
            AssertCode(     // fsbl	0,444444
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fdvrb
        [Test]
        public void Pdp10Rw_fdvrb()
        {
            Given_OctalWord("177400004163");
            AssertCode(     // fdvrb	10,4163
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // ashc


        // luuo14


        // dadd


        // trn


        // hllom
 


        // hrlz
        [Test]
        public void Pdp10Rw_hrlz()
        {
            Given_OctalWord("514440000140");
            AssertCode(     // hrlz	11,140
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrri
        [Test]
        public void Pdp10Rw_hrri()
        {
            Given_OctalWord("541440003002");
            AssertCode(     // hrri	11,3002
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrlm
        [Test]
        public void Pdp10Rw_hrlm()
        {
            Given_OctalWord("506340003235");
            AssertCode(     // hrlm	7,3235
                "0|L--|200000(1): 1 instructions",
                "1|L--|v3 = SEQ(LICE(Mem),@@, SLICE(r7, word18, 0), S)",
                "2|L--|Mem = v3");
        }

        // jumpg


        // andcam


        // tdza
        [Test]
        public void Pdp10Rw_tdza()
        {
            Given_OctalWord("634700000016");
            AssertCode(     // tdza	16,16
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hlrzs
        [Test]
        public void Pdp10Rw_hlrzs()
        {
            Given_OctalWord("557000000011");
            AssertCode(     // hlrzs	0,11
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // trcn
        [Test]
        public void Pdp10Rw_trcn()
        {
            Given_OctalWord("646300000015");
            AssertCode(     // trcn	6,15
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hllzs


        // tron
        [Test]
        public void Pdp10Rw_tron()
        {
            Given_OctalWord("666300000060");
            AssertCode(     // tron	6,60
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // ori


        // tlcn
        [Test]
        public void Pdp10Rw_tlcn()
        {
            Given_OctalWord("647700777777");
            AssertCode(     // tlcn	16,777777
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // movm


        // andm


        // setcm
        [Test]
        public void Pdp10Rw_setcm()
        {
            Given_OctalWord("460640005417");
            AssertCode(     // setcm	15,5417
                "0|L--|200000(1): 1 instructions",
                "1|L--|r13 = ~Mem0[0x005417<p18>:word36]");
        }

        // trca
        [Test]
        public void Pdp10Rw_trca()
        {
            Given_OctalWord("644640777777");
            AssertCode(     // trca	15,777777
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // movms
        [Test]
        public void Pdp10Rw_movms()
        {
            Given_OctalWord("217000000015");
            AssertCode(     // movms	0,15
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // muli




        // aojn


        // aosle


        // jsa
        [Test]
        public void Pdp10Rw_jsa()
        {
            Given_OctalWord("266434157116");
            AssertCode(     // jsa	10,@157116(14)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // idivb


        // movmm


        // luuo26


        // dfdv


        // fmpb
        [Test]
        public void Pdp10Rw_fmpb()
        {
            Given_OctalWord("163643334273");
            AssertCode(     // fmpb	15,334273(3)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fdvm
        [Test]
        public void Pdp10Rw_fdvm()
        {
            Given_OctalWord("172507534122");
            AssertCode(     // fdvm	12,534122(7)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fdvrl
        [Test]
        public void Pdp10Rw_fdvrl()
        {
            Given_OctalWord("175631463146");
            AssertCode(     // fdvrl	14,@463146(11)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tlce
        [Test]
        public void Pdp10Rw_tlce()
        {
            Given_OctalWord("643700240000");
            AssertCode(     // tlce	16,240000
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // trza
        [Test]
        public void Pdp10Rw_trza()
        {
            Given_OctalWord("624500777777");
            AssertCode(     // trza	12,777777
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrlom
        [Test]
        public void Pdp10Rw_hrlom()
        {
            Given_OctalWord("526555600124");
            AssertCode(     // hrlom	13,600124(15)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tsza
        [Test]
        public void Pdp10Rw_tsza()
        {
            Given_OctalWord("635351600124");
            AssertCode(     // tsza	7,600124(11)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // xmovei
        [Test]
        public void Pdp10Rw_xmovei()
        {
            Given_OctalWord("415752000123");
            AssertCode(     // xmovei	17,123(12)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // andca


        // hrro
        [Test]
        public void Pdp10Rw_hrro()
        {
            Given_OctalWord("560000000001");
            AssertCode(     // hrro	0,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // orcb


        

        // hlrem


        // orcam


        // tdz
        [Test]
        public void Pdp10Rw_tdz()
        {
            Given_OctalWord("630000000001");
            AssertCode(     // tdz	0,1
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tlca
        [Test]
        public void Pdp10Rw_tlca()
        {
            Given_OctalWord("645400000652");
            AssertCode(     // tlca	10,652
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // xor


        // hlrzi
        [Test]
        public void Pdp10Rw_hlrzi()
        {
            Given_OctalWord("555766450454");
            AssertCode(     // hlrzi	17,@450454(6)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrlzi
        [Test]
        public void Pdp10Rw_hrlzi()
        {
            Given_OctalWord("515565540414");
            AssertCode(     // hrlzi	13,@540414(5)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hlrzm
        [Test]
        public void Pdp10Rw_hlrzm()
        {
            Given_OctalWord("556554000413");
            AssertCode(     // hlrzm	13,413(14)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tdzn
        [Test]
        public void Pdp10Rw_tdzn()
        {
            Given_OctalWord("636542000413");
            AssertCode(     // tdzn	13,413(2)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hlrm


        // hrlo
        [Test]
        public void Pdp10Rw_hrlo()
        {
            Given_OctalWord("524646570004");
            AssertCode(     // hrlo	15,570004(6)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // orcmi


        // orcab


        // hrloi
        [Test]
        public void Pdp10Rw_hrloi()
        {
            Given_OctalWord("525600000002");
            AssertCode(     // hrloi	14,2
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // tdcn
        [Test]
        public void Pdp10Rw_tdcn()
        {
            Given_OctalWord("656350520004");
            AssertCode(     // tdcn	7,520004(10)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // orcm


        // sosle

        // trce
        [Test]
        public void Pdp10Rw_trce()
        {
            Given_OctalWord("642340000005");
            AssertCode(     // trce	7,5
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // troe
        [Test]
        public void Pdp10Rw_troe()
        {
            Given_OctalWord("662400000002");
            AssertCode(     // troe	10,2
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrroi
        [Test]
        public void Pdp10Rw_hrroi()
        {
            Given_OctalWord("561340007130");
            AssertCode(     // hrroi	7,7130
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // fmpm
        [Test]
        public void Pdp10Rw_fmpm()
        {
            Given_OctalWord("162436007421");
            AssertCode(     // fmpm	10,@7421(16)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // gfad
        [Test]
        public void Pdp10Rw_gfad()
        {
            Given_OctalWord("102156007421");
            AssertCode(     // gfad	3,7421(16)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // hrls


        // hlrs
        [Test]
        public void Pdp10Rw_hlrs()
        {
            Given_OctalWord("547000000173");
            AssertCode(     // hlrs	0,173
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // movsm
        [Test]
        public void Pdp10Rw_movsm()
        {
            Given_OctalWord("206152007252");
            AssertCode(     // movsm	3,7252(12)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }



        // sosg


        // caia


        // tso
        [Test]
        public void Pdp10Rw_tso()
        {
            Given_OctalWord("671400000015");
            AssertCode(     // tso	10,15
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // andcmi


        // hrrem
        [Test]
        public void Pdp10Rw_hrrem()
        {
            Given_OctalWord("572521505000");
            AssertCode(     // hrrem	12,@505000(1)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // andcb


        [Test]
        public void Pdp10Rw_fad()
        {
            Given_OctalWord("140404000701");
            AssertCode(     // fad	10,701(4)
                "0|L--|200000(1): 1 instructions",
                "1|L--|r8 = r8 + Mem[:real36]");
        }

        // setsts
        [Test]
        public void Pdp10Rw_setsts()
        {
            Given_OctalWord("060600000441");
            AssertCode(     // setsts	14,441
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        // ufa
        [Test]
        public void Pdp10Rw_ufa()
        {
            Given_OctalWord("130044436341");
            AssertCode(     // ufa	1,436341(4)
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

#endif

    }
}
