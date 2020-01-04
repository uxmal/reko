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
using Reko.Arch.Xtensa;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaRewriterTests : RewriterTestBase
    {
        private readonly XtensaArchitecture arch = new XtensaArchitecture("xtensa");
        private readonly Address baseAddr = Address.Ptr32(0x0010000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = (XtensaProcessorState)arch.CreateProcessorState();
            return new XtensaRewriter(arch, new LeImageReader(mem, 0), state, new Frame(arch.WordWidth), host);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Xtrw_l32r()
        {
            Given_UInt32s(0xFFFF71u); // l32r\ta7,000FFFFC
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a7 = 0000FFFC");
            Given_UInt32s(0xFFFE21u); // l32r\ta2,000FFFF8
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = 0000FFF8");
        }

        [Test]
        public void Xtrw_ret()
        {
            Given_UInt32s(0x000080);  // ret
            AssertCode(
                 "0|T--|00010000(3): 1 instructions",
                 "1|T--|return (0,0)");
        }

        [Test]
        public void Xtrw_ill()
        {
            Given_UInt32s(0x000000);  // ill
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__ill()");
        }

        [Test]
        public void Xtrw_wsr()
        {
            Given_UInt32s(0x13E720); // wsr\ta2,VECBASE
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|VECBASE = a2");
        }

        [Test]
        public void Xtrw_or()
        {
            Given_UInt32s(0x201110u); // "or\ta1,a1,a1
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|a1 = a1 | a1");
        }

        [Test]
        public void Xtrw_call0()
        {
            Given_UInt32s(0x00B205u);   // call0\t00100B24
            AssertCode(
              "0|T--|00010000(3): 2 instructions",
              "1|L--|a0 = 00010003",
              "2|T--|call 00010B24 (0)");
        }

        [Test]
        public void Xtrw_reserved()
        {
            Given_UInt32s(0xFE9200);  // reserved\t
            AssertCode(
            "0|L--|00010000(3): 1 instructions",
            "1|L--|__reserved()");
        }

        [Test]
        public void Xtrw_movi()
        {
            Given_UInt32s(0xA0A392u); // movi\ta9,000003A0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a9 = 0x000003A0");
        }

        [Test]
        public void Xtrw_sub()
        {
            Given_UInt32s(0xC01190); // "sub\ta1,a1,a9"
            AssertCode(
                "0|L--|00010000(3): 1 instructions");
        }

        [Test]
        public void Xtrw_s32i()
        {
            Given_UInt32s(0xE561D2); // "s32i\ta13,a1,0x00000394"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x00000394:word32] = a13");
            Given_UInt32s(0xE76102); // "s32i\ta0,a1,0x0000039C"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x0000039C:word32] = a0");
        }

        [Test]
        public void Xtrw_memw()
        {
            Given_UInt32s(0x0020C0); // "memw\t"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Xtrw_l32i_n()
        {
            Given_UInt32s(0x7D48); // "l32i.n\ta4,a13,28"
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a4 = Mem0[a13 + 0x0000001C:word32]");
        }

        [Test]
        public void Xtrw_movi_n()
        {
            Given_UInt32s(0xF37C); // movi.n\ta3,-01",
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a3 = -1");
        }

        [Test]
        public void Xtrw_s32i_n()
        {
            Given_UInt32s(0x7d39);    // s32i.n\ta3,a13,1C
            AssertCode(
               "0|L--|00010000(2): 1 instructions",
               "1|L--|Mem0[a13 + 0x0000001C:word32] = a3");
        }

        [Test]
        public void Xtrw_mov_n()
        {
            Given_UInt32s(0x013D);    // mov.n\ta3,a1;
            AssertCode(
               "0|L--|00010000(2): 1 instructions",
               "1|L--|a3 = a1");
        }

        [Test]
        public void Xtrw_l8ui()
        {
            Given_UInt32s(0x030122);  // l8ui\ta2,a1,0003
            AssertCode(
               "0|L--|00010000(3): 2 instructions",
               "1|L--|v3 = Mem0[a1 + 0x00000003:byte]",
               "2|L--|a2 = (uint32) v3");
        }

        [Test]
        public void Xtrw_srli()
        {
            Given_UInt32s(0x414420); // "srli\ta4,a2,04", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a4 = a2 >>u 4");
        }

        [Test]
        public void Xtrw_addi_n()
        {
            Given_UInt32s(0x440B); // "addi.n\ta4,a4,-01", );
            AssertCode(
               "0|L--|00010000(2): 1 instructions",
               "1|L--|a4 = a4 - 1");
        }

        [Test]
        public void Xtrw_extui()
        {
            Given_UInt32s(0x342020); // "extui\ta2,a2,00,04", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a2 = a2 & 0x0000000F");
        }

        [Test]
        public void Xtrw_addx2()
        {
            // addx2\ta6,a0,a0
            Given_UInt32s(0x906000);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a6 = a0 + a0 * 0x00000002");
        }

        [Test]
        public void Xtrw_addx4()
        {
            Given_UInt32s(0xA02230); // addx4\ta2,a2,a3", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a2 = a3 + a2 * 0x00000004");
        }

        [Test]
        public void Xtrw_bgeui()
        {
            Given_UInt32s(0x1244f6); // bgeui\ta4,00000004,00100016", );
            AssertCode(
               "0|T--|00010000(3): 1 instructions",
               "1|T--|if (a4 >=u 0x00000004) branch 00010016");
        }

        [Test]
        public void Xtrw_slli()
        {
            Given_UInt32s(0x114c40); //, "slli\ta4,a12,14");
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a12 << 20");
        }

        [Test]
        public void Xtrw_addmi()
        {
            Given_UInt32s(0xF0D422); // "addmi\ta2,a4,FFFFF000";
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = a4 + 0xFFFFF000");
        }

        [Test]
        public void Xtrw_j()
        {
            Given_UInt32s(0x000286); // "j\t0010000E" 
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|goto 0001000E");
        }

        [Test]
        public void Xtrw_and()
        {
            // and\ta4,a6,a4",
            Given_UInt32s(0x104640);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a6 & a4");
        }

        [Test]
        public void Xtrw_movnez()
        {
            // movnez\ta2,a5,a3",
            Given_UInt32s(0x932530);
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a3 == 0x00000000) branch 00010003",
                "2|L--|a2 = a5");
        }

        [Test]
        public void Xtrw_addi()
        {
            // addi\ta4,a3,FFFFFFFD",
            Given_UInt32s(0xFDC342);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a3 - 3");
        }

        [Test]
        public void Xtrw_add_n()
        {
            // add.n\ta4,a4,a12",
            Given_UInt32s(0x44CA);
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a4 = a4 + a12");
        }

        [Test]
        public void Xtrw_l32i()
        {
            // l32i\ta5,a1,0374",
            Given_UInt32s(0xFF2152);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a5 = Mem0[a1 + 0x000003FC:word32]");
        }

        [Test]
        public void Xtrw_bltu()
        {
            // bltu\ta4,a2,00100012",
            Given_UInt32s(0x0E3427);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a4 <u a2) branch 00010012");
        }

        [Test]
        public void Xtrw_callx0()
        {
            // callx0\ta0",
            Given_UInt32s(0x0000C0);
            AssertCode(
                "0|T--|00010000(3): 3 instructions",
                "1|L--|v3 = a0",
                "2|L--|a0 = 00010003",
                "3|T--|call v3 (0)");
        }

        [Test]
        public void Xtrw_bnei()
        {
            // bnei\ta13,-00000001,00100023",
            Given_UInt32s(0x1f0d66);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a13 != -1) branch 00010023");
        }

        [Test]
        public void Xtrw_bne()
        {
            // bne\ta4,a5,000FFFFC",
            Given_UInt32s(0xF89457);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a4 != a5) branch 0000FFFC");
        }

        [Test]
        public void Xtrw_orbc()
        {
            // orbc\tb1,b2,b3",
            Given_UInt32s(0x320030);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b0 = b0 | !b3");
        }

        [Test]
        public void Xtrw_ret_n()
        {
            // ret.n\t",
            Given_UInt32s(0xF00D);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void Xtrw_mull()
        {
            // mull\ta2,a4,a2",
            Given_UInt32s(0x822420);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = a4 * a2");
        }

        [Test]
        public void Xtrw_wsr_excsave2()
        {
            // wsr\ta0,EXCSAVE2",
            Given_UInt32s(0x13D200);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|EXCSAVE2 = a0");
        }

        [Test]
        public void Xtrw_break()
        {
            // break\t01,00",
            Given_UInt32s(0x004100);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__break(0x01, 0x00)");
        }

        [Test]
        public void Xtrw_beqz_n()
        {
            //beqz.n\ta14,00100013
            Given_UInt32s(0xFE8C);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (a14 == 0x00000000) branch 00010013");
        }

        [Test]
        public void Xtrw_andbc()
        {
            //andbc\tb4,b0,b2
            Given_UInt32s(0x124020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b4 = b0 & !b2");
        }

        [Test]
        public void Xtrw_rfi()
        {
            //rfi\t02
            Given_UInt32s(0x003210);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void Xtrw_neg()
        {
            //neg\ta2,a2
            Given_UInt32s(0x602020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = -a2");
        }

        [Test]
        public void Xtrw_bbsi()
        {
            //bbsi\ta4,00,000FFFEB
            Given_UInt32s(0xE7E407);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a4 & 0x00000001) != 0x00000000) branch 0000FFEB");
        }

        [Test]
        public void Xtrw_rsr()
        {
            //rsr\ta7,EXCCAUSE
            Given_UInt32s(0x03e870);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a7 = EXCCAUSE");
        }

        [Test]
        public void Xtrw_ball()
        {
            //ball\ta3,a6,00100029
            Given_UInt32s(0x254367);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((~a3 & a6) == 0x00000000) branch 00010029");
        }

        [Test]
        public void Xtrw_s8i()
        {
            //s8i\ta4,a2,0027
            Given_UInt32s(0x274242);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a2 + 0x00000027:byte] = a4");
        }

        [Test]
        public void Xtrw_xor()
        {
            //xor\ta5,a5,a4
            Given_UInt32s(0x305540);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = a5 ^ a4");
        }

        [Test]
        public void Xtrw_bnez_n()
        {
            //bnez.n\ta2,00100007
            Given_UInt32s(0x32CC);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (a2 != 0x00000000) branch 00010007");
        }

        [Test]
        public void Xtrw_add()
        {
            // add\ta4,a2,a3
            Given_UInt32s(0x804230);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a2 + a3");
        }

        [Test]
        public void Xtrw_rfe()
        {
            // rfe\t
            Given_UInt32s(0x003000);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void Xtrw_srai()
        {
            // srai\ta3,a10,10
            Given_UInt32s(0x3130a0);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a10 >> 16");
        }

        [Test]
        public void Xtrw_bnez()
        {
            // bnez\ta6,000FFFFA
            Given_UInt32s(0xff6656);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a6 != 0x00000000) branch 0000FFFA");
        }

        [Test]
        public void Xtrw_bany()
        {
            // bany\ta12,a3,00100012
            Given_UInt32s(0x0e8c37);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a12 & a3) != 0x00000000) branch 00010012");
        }

        [Test]
        public void Xtrw_beqz()
        {
            // beqz\ta0,0010000C
            Given_UInt32s(0x008016);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a0 == 0x00000000) branch 0001000C");
        }

        [Test]
        public void Xtrw_bltui()
        {
            // bltui\ta4,00000007,000FFFED
            Given_UInt32s(0xe974b6);
            AssertCode(
              "0|T--|00010000(3): 1 instructions",
              "1|T--|if (a4 <u 0x00000007) branch 0000FFED");
        }

        [Test]
        public void Xtrw_ssa8l()
        {
            // ssa8l\ta3
            Given_UInt32s(0x402300);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = a3 * 0x00000008");
        }

        [Test]
        public void Xtrw_ssl()
        {
            // ssl\ta2
            Given_UInt32s(0x401200);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = 0x00000020 - a2");
        }

        [Test]
        public void Xtrw_bnone()
        {
            // bnone\ta11,a2,0000FFFA
            Given_UInt32s(0xf60b27);
            AssertCode(
              "0|T--|00010000(3): 1 instructions",
              "1|T--|if ((a11 & a2) == 0x00000000) branch 0000FFFA");
        }

        [Test]
        public void Xtrw_movgez()
        {
            // movgez\ta4,a5,a3
            Given_UInt32s(0xb34530);
            AssertCode(
              "0|L--|00010000(3): 2 instructions",
              "1|T--|if (a3 < 0x00000000) branch 00010003",
              "2|L--|a4 = a5");
        }

        [Test]
        public void Xtrw_ssai()
        {
            // ssai\t08
            Given_UInt32s(0x404800);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = 0x08");
        }

        [Test]
        public void Xtrw_ssr()
        {
            // ssr\ta5
            Given_UInt32s(0x400500);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|SAR = a5");
        }

        [Test]
        public void Xtrw_src()
        {
            // src\ta6,a7,a6
            Given_UInt32s(0x816760);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a6 = (word32) (a7_a6 >>u SAR)");
        }

        [Test]
        public void Xtrw_sll()
        {
            // sll\ta7,a6
            Given_UInt32s(0xa17600);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a7 = a6 << SAR");
        }

        [Test]
        public void Xtrw_srl()
        {
            // srl\ta2,a8
            Given_UInt32s(0x912080);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a2 = a8 >> SAR");
        }

        [Test]
        public void Xtrw_bnall()
        {
            // bnall\ta5,a6,0010001E
            Given_UInt32s(0x1ac567);
            AssertCode(
               "0|T--|00010000(3): 1 instructions",
               "1|T--|if ((~a5 & a6) != 0x00000000) branch 0001001E");
        }

        [Test]
        public void Xtrw_ssi()
        {
            // ssi\tf7,a6,0000
            Given_UInt32s(0x004673);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|Mem0[a6:real32] = f7");
        }

        [Test]
        public void Xtrw_add_s()
        {
            // add.s\tf4,f6,f0
            Given_UInt32s(0x0a4600);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|f4 = f6 + f0");
        }

        [Test]
        public void Xtrw_lsiu()
        {
            // lsiu\tf3,a1,0000
            Given_UInt32s(0x008133);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|f3 = Mem0[a1:real32]");
        }

        [Test]
        public void Xtrw_s32ri()
        {
            // s32ri\ta0,a15,0300
            Given_UInt32s(0xc0ff02);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|Mem0[a15 + 0x00000300:word32] = a0");
        }

        [Test]
        public void Xtrw_nsau()
        {
            // nsau\ta3,a3
            Given_UInt32s(0x40f330);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = __nsau(a3)");
        }

        [Test]
        public void Xtrw_mul16u()
        {
            // mul16u\ta5,a6,a5
            Given_UInt32s(0xc15650);
            AssertCode(
               "0|L--|00010000(3): 3 instructions",
               "1|L--|v4 = (uint16) a6",
               "2|L--|v5 = (uint16) a5",
               "3|L--|a5 = v4 *u v5");
        }

        [Test]
        public void Xtrw_floor_s()
        {
            // floor.s\ta11,a12,02
            Given_UInt32s(0xBABC20);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a11 = __floor(f12, 0x02)");
        }

        [Test]
        public void Xtrw_rems()
        {
            // rems\ta3,a1,a0
            Given_UInt32s(0xF23100);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a1 % a0");
        }

        [Test]
        public void Xtrw_jx()
        {
            // jx\ta9
            Given_UInt32s(0x0009a0);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|goto a9");
        }

        [Test]
        public void Xtrw_subx2()
        {
            // subx2\ta11,a11,a9
            Given_UInt32s(0xD0BB90);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a11 = a11 * 0x00000002 - a9");
        }

        [Test]
        public void Xtrw_subx4()
        {
            // subx4\ta5,a5,a3
            Given_UInt32s(0xE05530);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = a5 * 0x00000004 - a3");
        }

        [Test]
        public void Xtrw_subx8()
        {
            // subx8\ta3,a13,a13
            Given_UInt32s(0xF03DD0);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a13 * 0x00000008 - a13");
        }

        [Test]
        public void Xtrw_isync()
        {
            // "isync\t
            Given_UInt32s(0x002000);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__isync()");
        }

        [Test]
        public void Xtrw_bbs()
        {
            // "bbs\ta3,a2,00100059
            Given_UInt32s(0x55d327);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a3 & 0x00000001 << a2) != 0x00000000) branch 00010059");
        }

        [Test]
        public void Xtrw_bbc()
        {
            // bbc\ta2,a4,00100008
            Given_UInt32s(0x045247);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a2 & 0x00000001 << a4) == 0x00000000) branch 00010008");
        }

        [Test]
        public void Xtrw_moveqz_s()
        {
            // moveqz.s\tf15,f12,a0
            Given_UInt32s(0x8bfc00);
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a0 != 0x00000000) branch 00010003",
                "2|L--|f15 = f12");
        }

        [Test]
        public void Xtrw_rsil()
        {
            // rsil\ta4,01
            Given_UInt32s(0x006140);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = __rsil(0x01)");
        }

        [Test]
        public void Xtrw_ldpte()
        {
            // ldpte\t
            Given_UInt32s(0xf1f810);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__ldpte()");
        }

        [Test]
        public void Xtrw_s32e()
        {
            // s32e\ta2,a0,-00000040
            Given_UInt32s(0x490020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__s32e(a0 + -64, a2)");
        }

        [Test]
        public void Xtrw_ueq_s()
        {
            // ueq.s\tb11,f0,f2
            Given_UInt32s(0x3bb020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b11 = f0 == f2");
        }

        [Test]
        public void Xtrw_l32e()
        {
            // l32e\ta0,a4,-00000030
            Given_UInt32s(0x094400);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = __l32e(a4 + -48)");
        }

        [Test]
        public void Xtrw_quou()
        {
            // quou\ta1,a6,a0
            Given_UInt32s(0xc21600);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a1 = a6 /u a0");
        }

        [Test]
        public void Xtrw_min()
        {
            // min\ta2,a0,a1
            Given_UInt32s(0x432010);
        }
    }
}