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
using Reko.Arch.Rl78;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Rl78
{
    [TestFixture]
    public class Rl78RewriterTests : RewriterTestBase
    {
        private readonly Rl78Architecture arch;
        private readonly Address addr;

        public Rl78RewriterTests()
        {
            this.arch = new Rl78Architecture(CreateServiceContainer(), "rl78", new Dictionary<string, object>());
            this.addr = Address.Ptr32(0x01000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void Rl78Rw_add()
        {
            Given_HexString("61 0A");	// add	a,c
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a + c",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_brk()
        {
            Given_HexString("61CC");
            AssertCode(     // brk
                "0|L--|00001000(2): 1 instructions",
                "1|L--|__syscall<word16>(0<16>)");
        }

        [Test]
        public void Rl78Rw_push_hl()
        {
            Given_HexString("C7");  // push hl
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[sp:word16] = hl");
        }

        [Test]
        public void Rl78Rw_subw()
        {
            Given_HexString("20 08");	// subw	sp,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|sp = sp - 8<i16>",
                "2|L--|CZ = cond(sp)");
        }

        [Test]
        public void Rl78Rw_movw_ax_hl()
        {
            Given_HexString("17"); // movw ax,hl
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = hl");
        }

        [Test]
        public void Rl78Rw_incw_ax()
        {
            Given_HexString("A1"); // incw ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = ax + 1<i16>");
        }

        [Test]
        public void Rl78Rw_clrw_bc()
        {
            Given_HexString("F7"); // clrw bc
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|bc = 0<16>");
        }

        [Test]
        public void Rl78Rw_mov_a_idx()
        {
            Given_HexString("49 10 20"); // mov a,[2010h+bc]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|a = Mem0[0x00002010<p32> + bc:byte]");
        }

        [Test]
        public void Rl78Rw_mov_M_a()
        {
            Given_HexString("9B"); // mov [hl],a
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|Mem0[hl:byte] = a");
        }

        [Test]
        public void Rl78Rw_mov_a_imm()
        {
            Given_HexString("51 04"); // mov a,0x04
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|a = 4<8>");
        }

        [Test]
        public void Rl78Rw_cmp_a_c()
        {
            Given_HexString("61 4A"); // cmp a,c
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|CZ = cond(a - c)");
        }

        [Test]
        public void Rl78Rw_cmps()
        {
            Given_HexString("61DE78");
            AssertCode(     // cmps	x,[hl+78h]
                "0|L--|00001000(3): 1 instructions",
                "1|L--|CZ = cond(x - Mem0[hl + 120<i16>:byte])");
        }

        [Test]
        public void Rl78Rw_bnz()
        {
            Given_HexString("DF F4"); // bnz 0000070A
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00000FF6");
        }

        [Test]
        public void Rl78Rw_pop_hl()
        {
            Given_HexString("C6"); // pop hl
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|hl = Mem0[sp:word16]",
                "2|L--|sp = sp + 2<i16>");
        }

        [Test]
        public void Rl78Rw_movw_ax_000A()
        {
            Given_HexString("30 0A 00"); // movw ax,0x000A<16>
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|ax = 0xA<16>");
        }

        [Test]
        public void Rl78Rw_onew_ax()
        {
            Given_HexString("E6"); // onew ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = 1<16>");
        }

        [Test]
        public void Rl78Rw_call_absolute_16()
        {
            Given_HexString("FD 3E 6E "); // call 00006E3E
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|call 00006E3E (4)");
        }

        [Test]
        public void Rl78Rw_call_absolute_20()
        {
            Given_HexString("FC 3E 6E 02"); // call 00026E3E
            AssertCode(
                "0|T--|00001000(4): 1 instructions",
                "1|T--|call 00026E3E (4)");
        }

        [Test]
        public void Rl78Rw_addw()
        {
            Given_HexString("10 08"); // addw sp,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|sp = sp + 8<i16>",
                "2|L--|CZ = cond(sp)");
        }

        [Test]
        public void Rl78Rw_mov_a_Mabs()
        {
            Given_HexString("8F CB FC"); // mov a,[0FCCBh]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|a = Mem0[0x0000FCCB<p32>:byte]");
        }

        [Test]
        public void Rl78Rw_sub_a_imm()
        {
            Given_HexString("2C 13"); // sub a,0x13
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a - 0x13<8>",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_dec()
        {
            Given_HexString("92");	// dec	c
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|c = c - 1<i8>",
                "2|L--|Z = cond(c)");
        }

        [Test]
        public void Rl78Rw_cmp0()
        {
            Given_HexString("D5 DA F9");	// cmp0	[0F9DAh]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|CZ = cond(Mem0[0x0000F9DA<p32>:byte] - 0<8>)");
        }

        [Test]
        public void Rl78Rw_cmpw()
        {
            Given_HexString("61 49 0A");	// cmpw	ax,[hl+0Ah]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|CZ = cond(ax - Mem0[hl + 10<i16>:word16])");
        }

        [Test]
        public void Rl78Rw_br()
        {
            Given_HexString("EF E3");	// br	00003C52
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|goto 00000FE5");
        }

        [Test]
        public void Rl78Rw_shrw()
        {
            Given_HexString("31 8E");   // shrw	ax,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = ax >>u 8<8>",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_set1()
        {
            Given_HexString("71 00 8C 03");	// set1	[038Ch].0
            AssertCode(
                "0|L--|00001000(4): 1 instructions",
                "1|L--|__set_bit(Mem0[0x0000038C<p32>:byte], 0<8>, true)");
        }

        [Test]
        public void Rl78Rw_sarw()
        {
            Given_HexString("31 8F");	// sarw	ax,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = ax >> 8<8>",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_ret()
        {
            Given_HexString("D7");	// ret
            AssertCode(
                "0|R--|00001000(1): 1 instructions",
                "1|R--|return (4,0)");
        }

        [Test]
        public void Rl78Rw_and()
        {
            Given_HexString("5C FE");	// and	a,0xFE
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a & 0xFE<8>",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Rl78Rw_bz()
        {
            Given_HexString("DD 1F");	// bz	0000701B
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00001021");
        }

        [Test]
        public void Rl78Rw_bnc()
        {
            Given_HexString("DE 2B");	// bnc	0000705F
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000102D");
        }

        [Test]
        public void Rl78Rw_decw()
        {
            Given_HexString("B1");	// decw	ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = ax - 1<i16>");
        }

        [Test]
        public void Rl78Rw_nop()
        {
            Given_HexString("00");	// nop
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Rl78Rw_oneb()
        {
            Given_HexString("E5 01 92");	// oneb	[9201h]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|Mem0[0x00009201<p32>:byte] = 1<8>");
        }

        [Test]
        public void Rl78Rw_sknz()
        {
            Given_HexString("61 F8 81 83");	// sknz
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00001003",
                "2|L--|00001002(1): 2 instructions",
                "3|L--|a = a + 1<i8>",
                "4|L--|Z = cond(a)",
                "5|L--|00001003(1): 2 instructions",
                "6|L--|b = b + 1<i8>",
                "7|L--|Z = cond(b)");

        }

        [Test]
        public void Rl78Rw_addc()
        {
            Given_HexString("1F D7 C1");	// addc	a,[0C1D7h]
            AssertCode(
                "0|L--|00001000(3): 2 instructions",
                "1|L--|a = a + Mem0[0x0000C1D7<p32>:byte] + C",  //$LIT should be p16
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_xch()
        {
            Given_HexString("08");	// xch	a,x
            AssertCode(
                "0|L--|00001000(1): 3 instructions",
                "1|L--|v4 = a",
                "2|L--|a = x",
                "3|L--|x = v4");
        }

        [Test]
        public void Rl78Rw_xor()
        {
            Given_HexString("7A E8 04");	// xor	[0FFF08h],0x04
            AssertCode(
                "0|L--|00001000(3): 3 instructions",
                "1|L--|v2 = Mem0[0x000FFF08<p32>:byte] ^ 4<8>",
                "2|L--|Mem0[0x000FFF08<p32>:byte] = v2",
                "3|L--|Z = cond(v2)");
        }

        [Test]
        public void Rl78Rw_reti()
        {
            Given_HexString("61 FC");	// reti
            AssertCode(
                "0|R--|00001000(2): 1 instructions",
                "1|R--|return (4,0)");
        }

        [Test]
        public void Rl78Rw_bf()
        {
            Given_HexString("31 74 0D 05");	// bf	[0FFE2Dh].7,000000EA
            AssertCode(
                "0|T--|00001000(4): 1 instructions",
                "1|T--|if (!__bit(Mem0[0x000FFE2D<p32>:byte], 7<8>)) branch 00001009"); //$LIT
        }

        [Test]
        public void Rl78Rw_clr1()
        {
            Given_HexString("71 23 30");	// clr1	[0FFE50h].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|__set_bit(Mem0[0x000FFE50<p32>:byte], 2<8>, false)");
        }

        [Test]
        public void Rl78Rw_xchw()
        {
            Given_HexString("37");	// xchw	ax,hl
            AssertCode(
                "0|L--|00001000(1): 3 instructions",
                "1|L--|v4 = ax",
                "2|L--|ax = hl",
                "3|L--|hl = v4");
        }

        [Test]
        public void Rl78Rw_inc()
        {
            Given_HexString("84");	// inc	e
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|e = e + 1<i8>",      //$LIT: should be 1<8>
                "2|L--|Z = cond(e)");
        }

        [Test]
        public void Rl78Rw_or()
        {
            Given_HexString("6B 00");	// or	a,[0FFE20h]
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a | Mem0[0x000FFE20<p32>:byte]",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Rl78Rw_subc()
        {
            Given_HexString("3D");	// subc	a,[hl]
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|a = a - Mem0[hl:byte] - C",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_bt()
        {
            Given_HexString("31 23 03");	// bt	a.2,000002FF
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (__bit(a, 2<8>)) branch 00001006");
        }

        [Test]
        public void Rl78Rw_bc()
        {
            Given_HexString("DC 06");	// bc	00000338
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00001008");
        }

        [Test]
        public void Rl78Rw_mulu()
        {
            Given_HexString("D6");	// mulu	x
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = a *u x");
        }

        [Test]
        public void Rl78Rw_btclr()
        {
            Given_HexString("31 00 FC D6");	// btclr	[0FFF1Ch].0,00000690
            AssertCode(
                "0|T--|00001000(4): 3 instructions",
                "1|T--|if (!__bit(Mem0[0x000FFF1C<p32>:byte], 0<8>)) branch 00001004",
                "2|L--|__set_bit(Mem0[0x000FFF1C<p32>:byte], 0<8>, false)",
                "3|T--|goto 00000FDA");
        }

        [Test]
        public void Rl78Rw_sel()
        {
            Given_HexString("61 CF");	// sel	rb0
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|__select_register_bank(0<8>)");
        }

        [Test]
        public void Rl78Rw_mov1()
        {
            Given_HexString("71 8C");	// mov1	cy,a.0
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = __bit(a, 0<8>)");
        }

        [Test]
        public void Rl78Rw_rolwc()
        {
            Given_HexString("61 EE");	// rolwc	ax,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = __rcl<word16,byte>(ax, 1<8>, C)",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_or1()
        {
            Given_HexString("71 2E 8F");	// or1	cy,[0FFF8Fh].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|cy = cy | __bit(Mem0[0x000FFF8F<p32>:byte], 2<8>)");
        }

        [Test]
        public void Rl78Rw_xor1()
        {
            Given_HexString("71 2F EE");	// xor1	cy,[0FFFEEh].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|cy = cy ^ __bit(Mem0[0x000FFFEE<p32>:byte], 2<8>)");
        }

        [Test]
        public void Rl78Rw_shl()
        {
            Given_HexString("31 69");	// shl	a,0x06
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a << 6<8>",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_shr()
        {
            Given_HexString("31 7A");	// shr	a,0x07
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a >>u 7<8>",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_callt()
        {
            Given_HexString("61 D5");	// callt	[009Ah]
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|call Mem0[0x0000009A<p32>:word16] (4)");
        }

        [Test]
        public void Rl78Rw_bnh()
        {
            Given_HexString("61 D3 08");	// bnh	00004297
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (Test(ULE,CZ)) branch 0000100B");
        }

        [Test]
        public void Rl78Rw_halt()
        {
            Given_HexString("61 ED");	// halt
            AssertCode(
                "0|H--|00001000(2): 1 instructions",
                "1|H--|__halt()");
        }

        [Test]
        public void Rl78Rw_movs()
        {
            Given_HexString("61CEA6");
            AssertCode(     // movs	[hl+0A6h],x
                "0|L--|00001000(3): 3 instructions",
                "1|L--|Mem0[hl + 166<i16>:byte] = x",
                "2|L--|C = a == 0<8>",
                "3|L--|Z = x == 0<8>");
        }

        [Test]
        public void Rl78Rw_rolc()
        {
            Given_HexString("61 DC");	// rolc	a,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = __rcl<byte,byte>(a, 1<8>, C)",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_rorc()
        {
            Given_HexString("61 FB");   // rorc a,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = __rcr<byte,byte>(a, 1<8>, C)",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_not1()
        {
            Given_HexString("71 C0");	// not1	cy
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = !cy");
        }

        [Test]
        public void Rl78Rw_and1()
        {
            Given_HexString("71 A5");	// and1	cy,[hl].2
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = cy & __bit(Mem0[hl:byte], 2<8>)");
        }

        [Test]
        public void Rl78Rw_rol()
        {
            Given_HexString("61 EB");	// rol	a,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = __rol<byte,byte>(a, 1<8>)",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_bh()
        {
            Given_HexString("61 C3 27");	// bh	0000B65D
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (Test(UGT,CZ)) branch 0000102A");
        }










    }
}
