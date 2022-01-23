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
using Reko.Arch.Pdp10;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Pdp10
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class Pdp10RewriterTests : RewriterTestBase
    {
        private readonly Pdp10Architecture arch;
        private readonly Address18 addr;

        public Pdp10RewriterTests()
        {
            this.arch = new Pdp10Architecture(CreateServiceContainer(), "pdp10", new Dictionary<string, object>());
            this.addr = new Address18(0x0010_000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void Given_OctalWord(string octalWord)
        {
            var word = Pdp10Architecture.OctalStringToWord(octalWord);
            Given_MemoryArea(new Word36MemoryArea(addr, new[] { word }));
        }

        [Obsolete("", true)]
        private new void Given_HexString(string s)
        {
            Given_OctalWord(s);
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
        public void Pdp10Rw_aobjn()
        {
            Given_OctalWord("253040041215");
            AssertCode(     // aobjn	1,41215
                "0|L--|200000(1): 4 instructions",
                "1|L--|v2 = SLICE(r1, word18, 18) + 1<18>",
                "2|L--|v3 = SLICE(r1, word18, 0) + 1<18>",
                "3|L--|r1 = SEQ(v2, v3)",
                "4|T--|if (v2 < 0<18>) branch 041215");
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
                "1|L--|v2 = Mem0[0x012502<p36>:word36] + 1<36>",
                "2|L--|C0C1VT = cond(v2)",
                "3|L--|r1 = v2",
                "4|L--|Mem0[0x012502<p36>:word36] = v2");
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
                "1|L--|pdp10_blt(r1, Mem0[0x023405<p36>:word36])");
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
        public void Pdp10Rw_caige()
        {
            Given_OctalWord("305200000000");
            AssertCode(     // caige	4,0
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r4 >= 0<36>) branch 200002");
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
        public void Pdp10Rw_cain()
        {
            Given_OctalWord("306040000104");
            AssertCode(     // cain	1,104
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 != 0x44<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_caile()
        {
            Given_OctalWord("303040000132");
            AssertCode(     // caile	1,132
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (r1 <= 0x5A<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_calli()
        {
            Given_OctalWord("047040400071");
            AssertCode(     // calli	1,400071
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_calli(r1, Mem0[0x400071<p36>:word36])");
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
        public void Pdp10Rw_coni()
        {
            Given_OctalWord("711640042563");
            AssertCode(     // coni	23,42563
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_coni(0x13<36>, Mem0[0x042563<p36>:word36])");
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
                "1|L--|pdp10_datao(0x1A<36>, Mem0[0x043112<p36>:word36])");
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
        public void Pdp10Rw_eqvm()
        {
            Given_OctalWord("446353000000");
            AssertCode(     // eqvm	7,0(13)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v2 = ~(r7 ^ Mem0[r11:word36])",
                "2|L--|Mem0[r11:word36] = v2");
        }

        [Test]
        public void Pdp10Rw_halt()
        {
            Given_OctalWord("254200036322");
            AssertCode(     // halt
                "0|H--|200000(1): 1 instructions",
                "1|L--|pdp10_halt(Mem0[0x036322<p36>:word36])");
        }

        [Test]
        public void Pdp10Rw_hll()
        {
            Given_OctalWord("500040000004");
            AssertCode(     // hll	1,4
                "0|L--|200000(1): 2 instructions",
                "1|L--|v4 = SLICE(r4, word18, 18)",
                "2|L--|r1 = SEQ(v4, SLICE(r1, word18, 0))");
        }

        [Test]
        [Ignore("Not done yet")]
        public void Pdp10Rw_hllz()
        {
            Given_OctalWord("510202025463");
            AssertCode(     // hllz	4,25463(2)
                "0|L--|200000(1): 1 instructions",
                "1|L--|v2 = CONVERT(SLICE(Mem[]@@@",
                "2|L--|r4 = v2 << 18");
        }

        [Test]
        [Ignore("Not done yet")]
        public void Pdp10Rw_hlrz()
        {
            Given_OctalWord("554200000002");
            AssertCode(     // hlrz	4,2
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Not done yet")]
        public void Pdp10Rw_hrrz()
        {
            Given_OctalWord("550000023443");
            AssertCode(     // hrrz	0,23443
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Pdp10Rw_hrrzm()
        {
            Given_OctalWord("552040023413");
            AssertCode(     // hrrzm	1,23413
                "0|L--|200000(1): 2 instructions",
                "1|L--|v2 = SLICE(Mem0[0x023413<p36>:word36], word18, 0)",
                "2|L--|r1 = CONVERT(v2, word18, word36)");
        }

        [Test]
        public void Pdp10Rw_ibp()
        {
            Given_OctalWord("133000012502");
            AssertCode(     // ibp	0,12502
                "0|L--|200000(1): 2 instructions",
                "1|L--|v2 = pdp10_inc_byte_ptr(Mem0[0x012502<p36>:word36])",
                "2|L--|Mem0[0x012502<p36>:word36] = v2");
        }

        [Test]
        public void Pdp10Rw_idivi()
        {
            Given_OctalWord("231040002000");
            AssertCode(     // idivi	1,2000
                "0|L--|200000(1): 3 instructions",
                "1|L--|r1 = r1 / 0x400<36>",
                "2|L--|r2 = r1 % 0x400<36>",
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
        public void Pdp10Rw_imul()
        {
            Given_OctalWord("220040042201");
            AssertCode(     // imul	1,42201
                "0|L--|200000(1): 2 instructions",
                "1|L--|r1 = r1 *s Mem0[0x042201<p36>:word36]",
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
        public void Pdp10Rw_jsr()
        {
            Given_OctalWord("264000002061");
            AssertCode(     // jsr	002061
                "0|T--|200000(1): 2 instructions",
                "1|L--|Mem0[0x002061<p36>:word36] = 200001",
                "2|T--|goto 0x002061<p36> + 1<36>");
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
                "1|L--|pdp10_lookup(r3, Mem0[0x023421<p36>:word36])");
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
                "1|L--|pdp10_luuo(2<36>, r0, Mem0[0x043017<p36>:word36])");
        }

        [Test]
        public void Pdp10Rw_move()
        {
            Given_OctalWord("200300042175");
            AssertCode(     // move	6,42175
                "0|L--|200000(1): 1 instructions",
                "1|L--|r6 = Mem0[0x042175<p36>:word36]");
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
                "1|L--|Mem0[0x023433<p36>:word36] = r1");
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
        public void Pdp10Rw_movns()
        {
            Given_OctalWord("213140000003");
            AssertCode(     // movns	3,3
                "0|L--|200000(1): 2 instructions",
                "1|L--|r3 = -r3",
                "2|L--|C1VT = cond(r3)");
        }

        [Test]
        public void Pdp10Rw_movs()
        {
            Given_OctalWord("204140023424");
            AssertCode(     // movs	3,23424
                "0|L--|200000(1): 4 instructions",
                "1|L--|v3 = Mem0[0x023424<p36>:word36]",
                "2|L--|v4 = SLICE(v3, word18, 18)",
                "3|L--|v5 = SLICE(v3, word18, 0)",
                "4|L--|r3 = SEQ(v5, v4)");
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
        public void Pdp10Rw_setmm()
        {
            Given_OctalWord("416031623650");
            AssertCode(     // setmm	0,@623650(11)
                "0|L--|200000(1): 2 instructions",
                "1|L--|v2 = Mem0[Mem0[r9 + 4294911912<i36>:word36]:word36]",
                "2|L--|Mem0[Mem0[r9 + 4294911912<i36>:word36]:word36] = v2");
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
                "1|L--|Mem0[0x000220<p36>:word36] = 0xFFFFFFFFF<36>");
        }

        [Test]
        public void Pdp10Rw_setom()
        {
            Given_OctalWord("476000023375");
            AssertCode(     // setom	0,23375
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[0x023375<p36>:word36] = 0xFFFFFFFFF<36>");
        }

        [Test]
        public void Pdp10Rw_setzm()
        {
            Given_OctalWord("402000023407");
            AssertCode(     // setzm	23407
                "0|L--|200000(1): 1 instructions",
                "1|L--|Mem0[0x023407<p36>:word36] = 0<36>");
        }

        [Test]
        public void Pdp10Rw_skipa()
        {
            Given_OctalWord("334340014361");
            AssertCode(     // skipa	7,14361
                "0|T--|200000(1): 2 instructions",
                "1|L--|r7 = Mem0[0x014361<p36>:word36]",
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
                "1|T--|if (Mem0[0x042131<p36>:word36] == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_skipe_2_ops()
        {
            Given_OctalWord("332400042131");
            AssertCode(     // skipe	1,42131
                "0|T--|200000(1): 3 instructions",
                "1|L--|v2 = Mem0[0x042131<p36>:word36]",
                "2|L--|r8 = v2",
                "3|T--|if (Mem0[0x042131<p36>:word36] == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_skipn()
        {
            Given_OctalWord("336000042126");
            AssertCode(     // skipn	42126
                "0|T--|200000(1): 1 instructions",
                "1|T--|if (Mem0[0x042126<p36>:word36] != 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_sojl()
        {
            Given_OctalWord("361200034776");
            AssertCode(     // sojl	4,034776
                "0|T--|200000(1): 3 instructions",
                "1|L--|r4 = r4 - 1<36>",
                "2|L--|C0C1VT = r4",
                "3|T--|if (r4 < 0<36>) branch 034776");
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
        [Ignore("Not done yet")]
        public void Pdp10Rw_tlc()
        {
            Given_OctalWord("641200400000");
            AssertCode(     // tlc	4,400000
                "0|L--|200000(1): 1 instructions",
                "1|L--|@@@");
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
        [Ignore("Not done yet")]
        public void Pdp10Rw_tlo()
        {
            Given_OctalWord("661040000001");
            AssertCode(     // tlo	1,1
                "0|L--|037574(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Not done yet")]
        public void Pdp10Rw_tlza()
        {
            Given_OctalWord("625040000001");
            AssertCode(     // tlza	1,1
                "0|L--|037573(1): 1 instructions",
                "1|L--|@@@");
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
        [Ignore("Not done yet")]
        public void Pdp10Rw_trz()
        {
            Given_OctalWord("620040000600");
            AssertCode(     // trz	1,600
                "0|L--|036555(1): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore("Not done yet")]
        public void Pdp10Rw_trze()
        {
            Given_OctalWord("622040000200");
            AssertCode(     // trze	1,200
                "0|L--|035166(1): 1 instructions",
                "1|L--|v3 = r9",
                "2|L--|r9 = v3 & ~0x2000<36>",
                "3|T--|if ((v3 & 0x2000<36>) == 0<36>) branch 200002");
        }

        [Test]
        public void Pdp10Rw_trzn()
        {
            Given_OctalWord("626440020000");
            AssertCode(     // trzn	11,20000
                "0|T--|200000(1): 3 instructions",
                "1|L--|v3 = r9",
                "2|L--|r9 = v3 & ~0x2000<36>",
                "3|T--|if ((v3 & 0x2000<36>) != 0<36>) branch 200002");
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
        public void Pdp10Rw_xct()
        {
            Given_OctalWord("256007042677");
            AssertCode(     // xct	0,42677(7)
                "0|L--|200000(1): 1 instructions",
                "1|L--|pdp10_xct(r0, r7 + 17855<i36>)");
        }

        [Test]
        public void Pdp10Rw_xori()
        {
            Given_OctalWord("431140000040");
            AssertCode(     // xori	3,40
                "0|L--|200000(1): 1 instructions",
                "1|L--|r3 = r3 ^ 0x20<36>");
        }
    }
}
