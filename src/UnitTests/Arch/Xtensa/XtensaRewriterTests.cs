#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaRewriterTests : RewriterTestBase
    {
        private readonly XtensaArchitecture arch = new XtensaArchitecture(CreateServiceContainer(), "xtensa", new Dictionary<string, object>());
        private readonly Address baseAddr = Address.Ptr32(0x0010000);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        [SetUp]
        public void Setup()
        {
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
        public void Xtrw_add_n()
        {
            // add.n\ta4,a4,a12",
            Given_UInt32s(0x44CA);
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a4 = a4 + a12");
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
        public void Xtrw_addi()
        {
            // addi\ta4,a3,FFFFFFFD",
            Given_UInt32s(0xFDC342);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a3 - 3<i32>");
        }

        [Test]
        public void Xtrw_addi_n()
        {
            Given_UInt32s(0x440B); // "addi.n\ta4,a4,-01", );
            AssertCode(
               "0|L--|00010000(2): 1 instructions",
               "1|L--|a4 = a4 - 1<i32>");
        }

        [Test]
        public void Xtrw_addmi()
        {
            Given_UInt32s(0xF0D422); // "addmi\ta2,a4,FFFFF000";
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = a4 + 0xFFFFF000<32>");
        }

        [Test]
        public void Xtrw_addx2()
        {
            // addx2\ta6,a0,a0
            Given_UInt32s(0x906000);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a6 = a0 + a0 * 2<32>");
        }

        [Test]
        public void Xtrw_addx4()
        {
            Given_UInt32s(0xA02230); // addx4\ta2,a2,a3", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a2 = a3 + a2 * 4<32>");
        }

        [Test]
        public void Xtrw_all4()
        {
            Given_HexString("709400");    // all4	b7,b4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b7 = b4 & b5 & b6 & b7");
        }

        [Test]
        public void Xtrw_all8()
        {
            Given_HexString("30B800");    // all8	b3,b8
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b3 = b8 & b9 & b10 & b11 & b12 & b13 & b14 & b15");
        }

        [Test]
        public void Xtrw_any8()
        {
            Given_HexString("A0A000");    // any8	b10,b0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b10 = b0 | b1 | b2 | b3 | b4 | b5 | b6 | b7");
        }

        [Test]
        public void Xtrw_any4()
        {
            Given_HexString("108000");    // any4	b1,b0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b1 = b0 | b1 | b2 | b3");
        }

        [Test]
        public void Xtrw_bf()
        {
            Given_HexString("760051");    // bf	b0,000032CD
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (!b0) branch 00010055");
        }

        [Test]
        public void Xtrw_bgeui()
        {
            Given_UInt32s(0x1244f6); // bgeui\ta4,00000004,00100016", );
            AssertCode(
               "0|T--|00010000(3): 1 instructions",
               "1|T--|if (a4 >=u 4<32>) branch 00010016");
        }

        [Test]
        public void Xtrw_bnez_n()
        {
            //bnez.n\ta2,00100007
            Given_UInt32s(0x32CC);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (a2 != 0<32>) branch 00010007");
        }

        [Test]
        public void Xtrw_iii()
        {
            Given_HexString("F2718B");    // iii	a1,022C
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__iii(a1 + 0x22C<u16>)");
        }

        [Test]
        public void Xtrw_iitlb()
        {
            Given_HexString("404050");    // iitlb	a4,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__iitlb(a0)");
        }

        [Test]
        public void Xtrw_ill()
        {
            Given_UInt32s(0x000000);  // ill
            AssertCode(
                "0|---|00010000(3): 1 instructions",
                "1|H--|__ill()");
        }

        [Test]
        public void Xtrw_ipf()
        {
            Given_HexString("C272D7");    // ipf	a2,035C
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__ipf(a2 + 0x35C<u16>)");
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
        public void Xtrw_j()
        {
            Given_UInt32s(0x000286); // "j\t0010000E" 
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|goto 0001000E");
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
        public void Xtrw_call4()
        {
            Given_HexString("95F620");    // call4	00020FC4
            AssertCode(
                "0|T--|00010000(3): 14 instructions",
                "1|L--|a2 = a6",
                "2|L--|a3 = a7",
                "3|L--|a4 = a8",
                "4|L--|a5 = a9",
                "5|L--|a6 = a10",
                "6|L--|a7 = a11",
                "7|L--|a0 = 00010003",
                "8|T--|call 00030F6C (0)",
                "9|L--|a11 = a7",
                "10|L--|a10 = a6",
                "11|L--|a9 = a5",
                "12|L--|a8 = a4",
                "13|L--|a7 = a3",
                "14|L--|a6 = a2");
        }

        [Test]
        public void Xtrw_call8()
        {
            Given_HexString("A5FFF0");    // call8	FFFF1508
            AssertCode(
                "0|T--|00010000(3): 14 instructions",
                "1|L--|a2 = a10",
                "2|L--|a3 = a11",
                "3|L--|a4 = a12",
                "4|L--|a5 = a13",
                "5|L--|a6 = a14",
                "6|L--|a7 = a15",
                "7|L--|a0 = 00010003",
                "8|T--|call 00000FFC (0)",
                "9|L--|a15 = a7",
                "10|L--|a14 = a6",
                "11|L--|a13 = a5",
                "12|L--|a12 = a4",
                "13|L--|a11 = a3",
                "14|L--|a10 = a2");
        }

        [Test]
        public void Xtrw_call12()
        {
            Given_HexString("F5FF30");    // call12	000310B0
            AssertCode(
                "0|T--|00010000(3): 6 instructions",
                "1|L--|a2 = a14",
                "2|L--|a3 = a15",
                "3|L--|a0 = 00010003",
                "4|T--|call 00041000 (0)",
                "5|L--|a15 = a3",
                "6|L--|a14 = a2");
        }

        [Test]
        public void Xtrw_ceil_s()
        {
            Given_HexString("00E1BA");    // ceil_s	a14,f1,1.0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a14 = __ceil(f1)");
        }

        [Test]
        public void Xtrw_cust0()
        {
            Given_HexString("000006");    // cust0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__cust0()");
        }

        [Test]
        public void Xtrw_cust1()
        {
            Given_HexString("403467");    // cust1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__cust1()");
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
            Given_UInt32s(0xE561D2); // "s32i\ta13,a1,0x394<32>"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x394<u32>:word32] = a13");
            Given_UInt32s(0xE76102); // "s32i\ta0,a1,0x39C<32>"
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a1 + 0x39C<u32>:word32] = a0");
        }

        [Test]
        public void Xtrw_max()
        {
            Given_HexString("F03B53");    // max	a3,a11,a15
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = max<int32>(a11, a15)");
        }

        [Test]
        public void Xtrw_maxu()
        {
            Given_HexString("001673");    // maxu	a1,a6,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a1 = max<uint32>(a6, a0)");
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
        public void Xtrw_min()
        {
            Given_HexString("005243");    // min	a5,a2,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = min<int32>(a2, a0)");
        }

        [Test]
        public void Xtrw_minu()
        {
            Given_HexString("004063");    // minu	a4,a0,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = min<uint32>(a0, a0)");
        }

        [Test]
        public void Xtrw_movi()
        {
            Given_UInt32s(0xA0A392u); // movi\ta9,000003A0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a9 = 0x3A0<32>");
        }



        [Test]
        public void Xtrw_movi_n()
        {
            Given_UInt32s(0xF37C); // movi.n\ta3,-01",
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a3 = -1<i32>");
        }

        [Test]
        public void Xtrw_s32i_n()
        {
            Given_UInt32s(0x7d39);    // s32i.n\ta3,a13,1C
            AssertCode(
               "0|L--|00010000(2): 1 instructions",
               "1|L--|Mem0[a13 + 0x1C<u32>:word32] = a3");
        }

        

        [Test]
        public void Xtrw_srli()
        {
            Given_UInt32s(0x414420); // "srli\ta4,a2,04", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a4 = a2 >>u 4<i32>");
        }

        [Test]
        public void Xtrw_dpfro()
        {
            Given_HexString("227CC3");    // dpfro	a12,030C
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dpfro(a12 + 0x30C<u16>)");
        }

        [Test]
        public void Xtrw_dhi()
        {
            Given_HexString("627CF2");    // dhi	a12,03C8
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dhi(a12 + 0x3C8<u16>)");
        }

        [Test]
        public void Xtrw_dhwb()
        {
            Given_HexString("427CF5");    // dhwb	a12,03D4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dhwb(a12 + 0x3D4<u16>)");
        }

        [Test]
        public void Xtrw_dhu()
        {
            Given_HexString("827992");    // dhu	a9,09
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dhu(a9 + 9<8>)");
        }

        [Test]
        public void Xtrw_dhwbi()
        {
            Given_HexString("527954");    // dhwbi	a9,0150
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dhwbi(a9 + 0x150<u16>)");
        }

        [Test]
        public void Xtrw_dii()
        {
            Given_HexString("727982");    // dii	a9,0208
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dii(a9 + 0x208<u16>)");
        }

        [Test]
        public void Xtrw_diu()
        {
            Given_HexString("827E53");    // diu	a14,05
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__diu(a14 + 5<8>)");
        }

        [Test]
        public void Xtrw_dpfr()
        {
            Given_HexString("027202");    // dpfr	a2,0008
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dpfr(a2 + 8<u16>)");
        }

        [Test]
        public void Xtrw_dpfw()
        {
            Given_HexString("127908");    // dpfw	a9,0020
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dpfw(a9 + 0x20<u16>)");
        }

        [Test]
        public void Xtrw_dpfwo()
        {
            Given_HexString("327928");    // dpfwo	a9,00A0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dpfwo(a9 + 0xA0<u16>)");
        }

        [Test]
        public void Xtrw_dsync()
        {
            Given_HexString("302000");    // dsync
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__dsync()");
        }

        [Test]
        public void Xtrw_entry()
        {
            Given_HexString("36F805");    // entry	a8,02F8
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a8 = a8 - 0x2F8<u16>");
        }

        [Test]
        public void Xtrw_esync()
        {
            Given_HexString("202000");    // esync
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__esync()");
        }

        [Test]
        public void Xtrw_excw()
        {
            Given_HexString("802800");    // excw
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__excw()");
        }

        [Test]
        public void Xtrw_extui()
        {
            Given_UInt32s(0x342020); // "extui\ta2,a2,00,04", );
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a2 = a2 & 0xF<32>");
        }

        [Test]
        public void Xtrw_float_s()
        {
            Given_HexString("F051CA");    // float.s	f5,a1,32768.0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|v4 = a1",
                "2|L--|f5 = CONVERT(v4, int32, real32) / 32768.0F");
        }

        [Test]
        public void Xtrw_floor_s()
        {
            // floor.s\ta11,a12,02
            Given_UInt32s(0xAABC20);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a11 = __floor_s(f12, 4.0F)");
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
        public void Xtrw_madd_s()
        {
            Given_HexString("00C14A");    // madd.s	f12,f1,f0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|f12 = f12 + f1 * f0");
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
        public void Xtrw_mov_s()
        {
            Given_HexString("0031FA");    // mov.s	f3,f1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|f3 = f1");
        }

        [Test]
        public void Xtrw_moveqz_s()
        {
            // moveqz.s\tf15,f12,a0
            Given_UInt32s(0x8bfc00);
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a0 != 0<32>) branch 00010003",
                "2|L--|f15 = f12");
        }


        [Test]
        public void Xtrw_movf()
        {
            Given_HexString("9032C3");    // movf	a3,a2,b9
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (b9) branch 00010003",
                "2|L--|a3 = a2");
        }

        [Test]
        public void Xtrw_movf_s()
        {
            Given_HexString("90B2CB");    // movf.s	f11,f2,b9
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (b9) branch 00010003",
                "2|L--|f11 = f2");
        }

        [Test]
        public void Xtrw_movgez()
        {
            // movgez\ta4,a5,a3
            Given_UInt32s(0xb34530);
            AssertCode(
              "0|L--|00010000(3): 2 instructions",
              "1|T--|if (a3 < 0<32>) branch 00010003",
              "2|L--|a4 = a5");
        }

        [Test]
        public void Xtrw_movgez_s()
        {
            Given_HexString("0000BB");    // movgez_s	f0,f0,a0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a0 < 0<32>) branch 00010003",
                "2|L--|f0 = f0");
        }

        [Test]
        public void Xtrw_movltz_s()
        {
            Given_HexString("0020AB");    // movltz_s	f2,f0,a0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a0 >= 0<32>) branch 00010003",
                "2|L--|f2 = f0");
        }

        [Test]
        public void Xtrw_movnez()
        {
            // movnez\ta2,a5,a3",
            Given_UInt32s(0x932530);
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a3 == 0<32>) branch 00010003",
                "2|L--|a2 = a5");
        }

        [Test]
        public void Xtrw_movnez_s()
        {
            Given_HexString("40C19B");    // movnez_s	f12,f1,a4
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (a4 == 0<32>) branch 00010003",
                "2|L--|f12 = f1");
        }

        [Test]
        public void Xtrw_movsp()
        {
            Given_HexString("001000");    // movsp	a0,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = a0");
        }

        [Test]
        public void Xtrw_movt()
        {
            Given_HexString("0041D3");    // movt	a4,a1,b0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (!b0) branch 00010003",
                "2|L--|a4 = a1");
        }

        [Test]
        public void Xtrw_movt_s()
        {
            Given_HexString("6041DB");    // movt.s	f4,f1,b6
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|T--|if (!b6) branch 00010003",
                "2|L--|f4 = f1");
        }

        [Test]
        public void Xtrw_l32ai()
        {
            Given_HexString("02B243");    // l32ai	a0,a2,010C
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = __l32ai(a2 + 0x10C<u16>)");     //$LIT: 0x10c<u32> at least?
        }

        [Test]
        public void Xtrw_l32e()
        {
            // l32e\ta0,a4,-00000030
            Given_UInt32s(0x094400);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = __l32e(a4 + -48<i32>)");
        }


        [Test]
        public void Xtrw_l32i()
        {
            // l32i\ta5,a1,0374",
            Given_UInt32s(0xFF2152);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a5 = Mem0[a1 + 0x3FC<u32>:word32]");
        }

        [Test]
        public void Xtrw_l32i_n()
        {
            Given_UInt32s(0x7D48); // "l32i.n\ta4,a13,28"
            AssertCode(
                "0|L--|00010000(2): 1 instructions",
                "1|L--|a4 = Mem0[a13 + 0x1C<u32>:word32]");
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
        public void Xtrw_l8ui()
        {
            Given_UInt32s(0x030122);  // l8ui\ta2,a1,0003
            AssertCode(
               "0|L--|00010000(3): 2 instructions",
               "1|L--|v3 = Mem0[a1 + 3<u32>:byte]",
               "2|L--|a2 = CONVERT(v3, byte, uint32)");
        }

        [Test]
        public void Xtrw_lddec()
        {
            Given_HexString("341090");    // lddec	mr1,a0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|a0 = a0 - 4<i32>",
                "2|L--|mr1 = Mem0[a0:word32]");
        }

        [Test]
        public void Xtrw_ldinc()
        {
            Given_HexString("441180");    // ldinc	mr1,a1
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|a1 = a1 + 4<i32>",
                "2|L--|mr1 = Mem0[a1:word32]");
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
        public void Xtrw_loop()
        {
            Given_HexString("768002 3022A0");    // loop	a0,00010006; addx4...
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|LCOUNT = a0 - 1<32>",
                "2|L--|00010003(3): 4 instructions",
                "3|L--|a2 = a3 + a2 * 4<32>",
                "4|T--|if (LCOUNT == 0<32>) branch 00010006",
                "5|L--|LCOUNT = LCOUNT - 1<32>",
                "6|T--|goto 00010003");
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
        public void Xtrw_callx12()
        {
            Given_HexString("F00000");    // callx12	a0
            AssertCode(
                "0|T--|00010000(3): 7 instructions",
                "1|L--|a2 = a14",
                "2|L--|a3 = a15",
                "3|L--|v7 = a0",
                "4|L--|a0 = 00010003",
                "5|T--|call v7 (0)",
                "6|L--|a15 = a3",
                "7|L--|a14 = a2");
        }

        [Test]
        public void Xtrw_clamps()
        {
            Given_HexString("001033");    // clamps	a1,a0,07
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a1 = __clamps(a0, 7<8>)");
        }

        [Test]
        public void Xtrw_bnei()
        {
            // bnei\ta13,-00000001,00100023",
            Given_UInt32s(0x1f0d66);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a13 != -1<i32>) branch 00010023");
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
        public void Xtrw_orb()
        {
            Given_HexString("20D022");    // orb	b13,b0,b2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b13 = b0 | b2");
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
        public void Xtrw_pitlb()
        {
            Given_HexString("005050");    // pitlb	a0,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = __pitlb(a0)");
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
                "1|L--|__break(1<8>, 0<8>)");
        }

        [Test]
        public void Xtrw_beqz_n()
        {
            //beqz.n\ta14,00100013
            Given_UInt32s(0xFE8C);
            AssertCode(
                "0|T--|00010000(2): 1 instructions",
                "1|T--|if (a14 == 0<32>) branch 00010013");
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
        public void Xtrw_neg()
        {
            //neg\ta2,a2
            Given_UInt32s(0x602020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = -a2");
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
        public void Xtrw_oeq_s()
        {
            Given_HexString("C0E12B");    // oeq.s	b14,f1,f12
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b14 = f1 == f12");
        }

        [Test]
        public void Xtrw_ole_s()
        {
            Given_HexString("00456B");    // ole.s	b4,f5,f0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b4 = f5 <= f0");
        }

        [Test]
        public void Xtrw_olt_s()
        {
            Given_HexString("C0744B");    // olt.s	b7,f4,f12
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b7 = f4 < f12");
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
        public void Xtrw_bbsi()
        {
            //bbsi\ta4,00,000FFFEB
            Given_UInt32s(0xE7E407);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a4 & 1<32>) != 0<32>) branch 0000FFEB");
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
        public void Xtrw_rdtlb0()
        {
            Given_HexString("30B150");    // rdtlb0	a3,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = __rdtlb0(a1)");
        }

        [Test]
        public void Xtrw_rdtlb1()
        {
            Given_HexString("70F550");    // rdtlb1	a7,a5
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a7 = __rdtlb1(a5)");
        }

        [Test]
        public void Xtrw_rems()
        {
            // rems\ta3,a1,a0
            Given_UInt32s(0xF23100);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a1 %s a0");
        }

        [Test]
        public void Xtrw_rer()
        {
            Given_HexString("606440");    // rer	a6,a4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a6 = __rer(a4)");
        }

        [Test]
        public void Xtrw_reserved()
        {
            Given_UInt32s(0xFE9200);  // reserved\t
            AssertCode(
            "0|---|00010000(3): 1 instructions",
            "1|---|<invalid>");
        }

        [Test]
        public void Xtrw_ret()
        {
            Given_HexString("800000");  // ret
            AssertCode(
                 "0|R--|00010000(3): 1 instructions",
                 "1|R--|return (0,0)");
        }

        [Test]
        public void Xtrw_ret_n()
        {
            // ret.n\t",
            Given_UInt32s(0xF00D);
            AssertCode(
                "0|R--|00010000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void Xtrw_rfe()
        {
            // rfe\t
            Given_UInt32s(0x003000);
            AssertCode(
                "0|R--|00010000(3): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void Xtrw_rfi()
        {
            //rfi\t02
            Given_UInt32s(0x003210);
            AssertCode(
                "0|R--|00010000(3): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void Xtrw_ritlb0()
        {
            Given_HexString("503150");    // ritlb0	a5,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = __ritlb0(a1)");
        }

        [Test]
        public void Xtrw_ritlb1()
        {
            Given_HexString("C07450");    // ritlb1	a12,a4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a12 = __ritlb1(a4)");
        }

        [Test]
        public void Xtrw_rotw()
        {
            Given_HexString("208940");    // rotw	-06
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__rotate_window(-6<i8>)");
        }

        [Test]
        public void Xtrw_round_s()
        {
            Given_HexString("00418A");    // round.s	a4,f1,1.0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = __round(f1)");
        }

        [Test]
        public void Xtrw_rfr()
        {
            Given_HexString("4031FA");    // rfr	a3,f1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = f1");
        }

        [Test]
        public void Xtrw_rsil()
        {
            // rsil\ta4,01
            Given_UInt32s(0x006140);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = __rsil(1<8>)");
        }

        [Test]
        public void Xtrw_rsync()
        {
            Given_HexString("102000");    // rsync
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__rsync()");
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
        public void Xtrw_rur()
        {
            Given_HexString("4028E3");    // rur	a2,user132
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = user132");
        }

        [Test]
        public void Xtrw_ball()
        {
            //ball\ta3,a6,00100029
            Given_UInt32s(0x254367);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((~a3 & a6) == 0<32>) branch 00010029");
        }

        [Test]
        public void Xtrw_s8i()
        {
            //s8i\ta4,a2,0027
            Given_UInt32s(0x274242);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|Mem0[a2 + 0x27<u32>:byte] = a4");        //$LIT: check 0x27<u32>
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
        public void Xtrw_srai()
        {
            // srai\ta3,a10,10
            Given_UInt32s(0x3130a0);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a10 >> 16<i32>");
        }

        [Test]
        public void Xtrw_bnez()
        {
            // bnez\ta6,000FFFFA
            Given_UInt32s(0xff6656);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a6 != 0<32>) branch 0000FFFA");
        }

        [Test]
        public void Xtrw_bany()
        {
            // bany\ta12,a3,00100012
            Given_UInt32s(0x0e8c37);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a12 & a3) != 0<32>) branch 00010012");
        }

        [Test]
        public void Xtrw_beqz()
        {
            // beqz\ta0,0010000C
            Given_UInt32s(0x008016);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if (a0 == 0<32>) branch 0001000C");
        }

        [Test]
        public void Xtrw_bltui()
        {
            // bltui\ta4,00000007,000FFFED
            Given_UInt32s(0xe974b6);
            AssertCode(
              "0|T--|00010000(3): 1 instructions",
              "1|T--|if (a4 <u 7<32>) branch 0000FFED");
        }

        [Test]
        public void Xtrw_ssa8l()
        {
            // ssa8l\ta3
            Given_UInt32s(0x402300);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = a3 * 8<32>");
        }

        [Test]
        public void Xtrw_ssl()
        {
            // ssl\ta2
            Given_UInt32s(0x401200);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = 0x20<32> - a2");
        }

        [Test]
        public void Xtrw_bnone()
        {
            // bnone\ta11,a2,0000FFFA
            Given_UInt32s(0xf60b27);
            AssertCode(
              "0|T--|00010000(3): 1 instructions",
              "1|T--|if ((a11 & a2) == 0<32>) branch 0000FFFA");
        }


        [Test]
        public void Xtrw_ssai()
        {
            // ssai\t08
            Given_UInt32s(0x404800);
            AssertCode(
              "0|L--|00010000(3): 1 instructions",
              "1|L--|SAR = 8<8>");
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
        public void Xtrw_sext()
        {
            Given_HexString("108723");    // sext	a8,a7,08
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a8 = __sext(a7, 8<8>)");
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
        public void Xtrw_slli()
        {
            Given_UInt32s(0x114c40); //, "slli\ta4,a12,14");
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a4 = a12 << 20<i32>");
        }

        [Test]
        public void Xtrw_src()
        {
            // src\ta6,a7,a6
            Given_UInt32s(0x816760);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|a6 = CONVERT(a7_a6 >>u SAR, word64, word32)");
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
               "1|T--|if ((~a5 & a6) != 0<32>) branch 0001001E");
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
        public void Xtrw_s32ri()
        {
            // s32ri\ta0,a15,0300
            Given_UInt32s(0xc0ff02);
            AssertCode(
               "0|L--|00010000(3): 1 instructions",
               "1|L--|Mem0[a15 + 0x300<u32>:word32] = a0");
        }

        [Test]
        public void Xtrw_msub_s()
        {
            Given_HexString("40FE5A");    // msub.s	f15,f14,f4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|f15 = f15 - f14 * f4");
        }

        [Test]
        public void Xtrw_mul_aa_hh()
        {
            Given_HexString("941677");    // mul.aa.hh	a6,a9
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hh<word32,int40>(a6, a9)");
        }

        [Test]
        public void Xtrw_mul_aa_hl()
        {
            Given_HexString("040C75");    // mul.aa.hl	a12,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hl<word32,int40>(a12, a0)");
        }

        [Test]
        public void Xtrw_mul_aa_ll()
        {
            Given_HexString("147074");    // mul.aa.ll	a0,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_ll<word32,int40>(a0, a1)");
        }

        [Test]
        public void Xtrw_mul_ad_hh()
        {
            Given_HexString("F43A37");    // mul.ad.hh	a10,mr3
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hh<word32,int40>(a10, mr3)");
        }

        [Test]
        public void Xtrw_mul_ad_hl()
        {
            Given_HexString("240C35");    // mul.ad.hl	a12,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hl<word32,int40>(a12, mr2)");
        }

        [Test]
        public void Xtrw_mul_ad_ll()
        {
            Given_HexString("149C34");    // mul.ad.ll	a12,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_ll<word32,int40>(a12, mr2)");
        }

        [Test]
        public void Xtrw_mul_da_hh()
        {
            Given_HexString("147067");    // mul.da.hh	mr1,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hh<word32,int40>(mr1, a1)");
        }

        [Test]
        public void Xtrw_mul_da_hl()
        {
            Given_HexString("044565");    // mul.da.hl	mr1,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hl<word32,int40>(mr1, a0)");
        }

        [Test]
        public void Xtrw_mul_da_lh()
        {
            Given_HexString("D42066");    // mul.da.lh	mr0,a13
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_lh<word32,int40>(mr0, a13)");
        }

        [Test]
        public void Xtrw_mul_da_ll()
        {
            Given_HexString("E43764");    // mul.da.ll	mr0,a14
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_ll<word32,int40>(mr0, a14)");
        }

        [Test]
        public void Xtrw_mul_dd_hh()
        {
            Given_HexString("940027");    // mul.dd.hh	mr0,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hh<word32,int40>(mr0, mr2)");
        }

        [Test]
        public void Xtrw_mul_dd_hl()
        {
            Given_HexString("F48725");    // mul.dd.hl	mr0,mr3
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_hl<word32,int40>(mr0, mr3)");
        }

        [Test]
        public void Xtrw_mul_dd_lh()
        {
            Given_HexString("A40B26");    // mul.dd.lh	mr0,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_lh<word32,int40>(mr0, mr2)");
        }

        [Test]
        public void Xtrw_mul_dd_ll()
        {
            Given_HexString("04B924");    // mul.dd.ll	mr0,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __mul_ll<word32,int40>(mr0, mr2)");
        }

        [Test]
        public void Xtrw_mul16u()
        {
            // mul16u\ta5,a6,a5
            Given_UInt32s(0xc15650);
            AssertCode(
               "0|L--|00010000(3): 3 instructions",
               "1|L--|v4 = CONVERT(a6, word32, uint16)",
               "2|L--|v5 = CONVERT(a5, word32, uint16)",
               "3|L--|a5 = v4 *u v5");
        }

        [Test]
        public void Xtrw_mula_aa_hl()
        {
            Given_HexString("248079");    // mula.aa.hl	a0,a2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hl<word32,int40>(a0, a2)");
        }

        [Test]
        public void Xtrw_mula_aa_lh()
        {
            Given_HexString("74307A");    // mula.aa.lh	a0,a7
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_lh<word32,int40>(a0, a7)");
        }

        [Test]
        public void Xtrw_mula_ad_hh()
        {
            Given_HexString("04053B");    // mula.ad.hh	a5,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hh<word32,int40>(a5, mr2)");
        }

        [Test]
        public void Xtrw_mula_ad_hl()
        {
            Given_HexString("344139");    // mula.ad.hl	a1,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hl<word32,int40>(a1, mr2)");
        }

        [Test]
        public void Xtrw_mula_ad_lh()
        {
            Given_HexString("A4FD3A");    // mula.ad.lh	a13,mr2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_lh<word32,int40>(a13, mr2)");
        }

        [Test]
        public void Xtrw_mula_ad_ll()
        {
            Given_HexString("F43738");    // mula.ad.ll	a7,mr3
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_ll<word32,int40>(a7, mr3)");
        }

        [Test]
        public void Xtrw_mula_da_hh_ldinc()
        {
            Given_HexString("04054B");    // mula.da.hh.ldinc	mr0,a5,mr0,a0
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a5 = a5 + 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hh<word32,int40>(mr0, a0)",
                "3|L--|mr0 = Mem0[a5:word32]");
        }

        [Test]
        public void Xtrw_mula_da_hl_lddec()
        {
            Given_HexString("044559");    // mula.da.hl.lddec	mr0,a5,mr1,a0
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a5 = a5 - 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hl<word32,int40>(mr1, a0)");
        }

        [Test]
        public void Xtrw_mula_da_hl_ldinc()
        {
            Given_HexString("248149");    // mula.da.hl.ldinc	mr0,a1,mr0,a2
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a1 = a1 + 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hl<word32,int40>(mr0, a2)",
                "3|L--|mr0 = Mem0[a1:word32]");
        }

        [Test]
        public void Xtrw_mula_da_lh_lddec()
        {
            Given_HexString("04015A");    // mula.da.lh.lddec	mr0,a1,mr0,a0
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a1 = a1 - 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_lh<word32,int40>(mr0, a0)",
                "3|L--|mr0 = Mem0[a1:word32]");
        }

        [Test]
        public void Xtrw_mula_da_lh_ldinc()
        {
            Given_HexString("040C0A");    // mula.da.lh.ldinc	mr0,a12,mr0,a0
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a12 = a12 + 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_lh<word32,int40>(mr0, a0)",
                "3|L--|mr0 = Mem0[a12:word32]");
        }

        [Test]
        public void Xtrw_mula_da_ll_lddec()
        {
            Given_HexString("F45158");    // mula.da.ll.lddec	mr1,a1,mr1,a15
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a1 = a1 - 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_ll<word32,int40>(mr1, a15)",
                "3|L--|mr1 = Mem0[a1:word32]");
        }

        [Test]
        public void Xtrw_mula_da_ll_ldinc()
        {
            Given_HexString("040C08");    // mula.da.ll.ldinc	mr0,a12,mr0,a0
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a12 = a12 + 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_ll<word32,int40>(mr0, a0)",
                "3|L--|mr0 = Mem0[a12:word32]");
        }

        [Test]
        public void Xtrw_mula_dd_hh_lddec()
        {
            Given_HexString("64FF1B");    // mula.dd.hh.lddec	mr3,a15,mr1,a6
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a15 = a15 - 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_hh<word32,int40>(mr1, a6)",
                "3|L--|mr3 = Mem0[a15:word32]");
        }

        [Test]
        public void Xtrw_mula_dd_ll_lddec()
        {
            Given_HexString("448518");    // mula.dd.ll.lddec	mr0,a5,mr0,a4
            AssertCode(
                "0|L--|00010000(3): 3 instructions",
                "1|L--|a5 = a5 - 4<i32>",
                "2|L--|ACCHI_ACCLO = ACCHI_ACCLO + __mul_ll<word32,int40>(mr0, a4)");
        }

        [Test]
        public void Xtrw_muls_aa_hh()
        {
            Given_HexString("14407F");    // muls.aa.hh	a0,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO - __mul_hh<word32,int40>(a0, a1)");
        }

        [Test]
        public void Xtrw_muls_aa_ll()
        {
            Given_HexString("14047C");    // muls.aa.ll	a4,a1
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = ACCHI_ACCLO - __mul_ll<word32,int40>(a4, a1)");
        }

        [Test]
        public void Xtrw_mulsh()
        {
            Given_HexString("2070B2");    // mulsh	a7,a0,a2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a7 = __mulsh<word32,int32>(a0, a2)");
        }

        [Test]
        public void Xtrw_muluh()
        {
            Given_HexString("2000A2");    // muluh	a0,a0,a2
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a0 = __muluh<word32,uint32>(a0, a2)");
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
                "1|L--|a11 = a11 * 2<32> - a9");
        }

        [Test]
        public void Xtrw_subx4()
        {
            // subx4\ta5,a5,a3
            Given_UInt32s(0xE05530);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = a5 * 4<32> - a3");
        }

        [Test]
        public void Xtrw_subx8()
        {
            // subx8\ta3,a13,a13
            Given_UInt32s(0xF03DD0);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a3 = a13 * 8<32> - a13");
        }

        [Test]
        public void Xtrw_syscall()
        {
            Given_HexString("305700");    // syscall
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|L--|__syscall()");
        }


        [Test]
        public void Xtrw_bbs()
        {
            // "bbs\ta3,a2,00100059
            Given_UInt32s(0x55d327);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a3 & 1<u32> << a2) != 0<32>) branch 00010059");
        }

        [Test]
        public void Xtrw_bbc()
        {
            // bbc\ta2,a4,00100008
            Given_UInt32s(0x045247);
            AssertCode(
                "0|T--|00010000(3): 1 instructions",
                "1|T--|if ((a2 & 1<u32> << a4) == 0<32>) branch 00010008");
        }

        [Test]
        public void Xtrw_s32c1i()
        {
            Given_HexString("C2E201");    // s32c1i	a12,a2,0004
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a12 = __s32c1i(a2 + 4<u16>, SCOMPARE1)");        //$LIT: should be 4<u32> or 4<32>?
        }

        [Test]
        public void Xtrw_s32e()
        {
            // s32e\ta2,a0,-00000040
            Given_UInt32s(0x490020);
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__s32e(a0 + -64<i32>, a2)");
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
        public void Xtrw_un_s()
        {
            Given_HexString("40001B");    // un.s	b0,f0,f4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b0 = isunordered(f0, f4)");
        }

        [Test]
        public void Xtrw_ssa8b()
        {
            Given_HexString("403440");    // ssa8b	a4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|SAR = 32<i32> - a4 * 8<32>");
        }

        [Test]
        public void Xtrw_trunc_s()
        {
            Given_HexString("90FE9A");    // trunc.s	a15,f14,512.0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a15 = __trunc(f14 * 512.0F)");
        }

        [Test]
        public void Xtrw_ufloat_s()
        {
            Given_HexString("40B0DA");    // ufloat_s	f11,a0,16.0
            AssertCode(
                "0|L--|00010000(3): 2 instructions",
                "1|L--|v4 = a0",
                "2|L--|f11 = CONVERT(v4, uint32, real32) / 16.0F");
        }

        [Test]
        public void Xtrw_ule_s()
        {
            Given_HexString("00017B");    // ule.s	b0,f1,f0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b0 = f1 <= f0");
        }

        [Test]
        public void Xtrw_ult_s()
        {
            Given_HexString("C0705B");    // ult.s	b7,f0,f12
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b7 = f0 < f12");
        }

        [Test]
        public void Xtrw_umul_aa_hh()
        {
            Given_HexString("F41673");    // umul.aa.hh	a6,a15
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __umul_hh<word32,uint40>(a6, a15)");
        }

        [Test]
        public void Xtrw_umul_aa_hl()
        {
            Given_HexString("040171");    // umul.aa.hl	a1,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __umul_hl<word32,uint40>(a1, a0)");
        }

        [Test]
        public void Xtrw_umul_aa_lh()
        {
            Given_HexString("440172");    // umul.aa.lh	a1,a4
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __umul_lh<word32,uint40>(a1, a4)");
        }
        
        [Test]
        public void Xtrw_umul_aa_ll()
        {
            Given_HexString("744170");    // umul.aa.ll	a1,a7
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|ACCHI_ACCLO = __umul_ll<word32,uint40>(a1, a7)");
        }

        [Test]
        public void Xtrw_utrunc_s()
        {
            Given_HexString("0051EA");    // utrunc.s	a5,f1,1.0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a5 = __utrunc(f1)");
        }

        [Test]
        public void Xtrw_waiti()
        {
            Given_HexString("007200");    // waiti	02
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__waiti(2<8>)");
        }

        [Test]
        public void Xtrw_wdtlb()
        {
            Given_HexString("40E350");    // wdtlb	a4,a3
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__wdtlb(a4, a3)");
        }

        [Test]
        public void Xtrw_wer()
        {
            Given_HexString("207040");    // wer	a2,a0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__wer(a0, a2)");
        }

        [Test]
        public void Xtrw_witlb()
        {
            Given_HexString("406350");    // witlb	a4,a3
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|__witlb(a4, a3)");
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
        public void Xtrw_wur()
        {
            Given_HexString("00E0F3");    // wur	a0,user14
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|user14 = a0");
        }


        [Test]
        public void Xtrw_xorb()
        {
            Given_HexString("002142");    // xorb	b2,b1,b0
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|b2 = b1 ^ b0");
        }

        [Test]
        public void Xtrw_xsr()
        {
            Given_HexString("20E661");    // xsr	a2,PS
            AssertCode(
                "0|L--|00010000(3): 1 instructions",
                "1|L--|a2 = __xsr(a2, PS)");
        }




























    }
}