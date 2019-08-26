#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Rl78
{
    [TestFixture]
    public class Rl78RewriterTests : RewriterTestBase
    {
        private readonly Rl78Architecture arch;
        private readonly Address addr;
        private MemoryArea image;

        public Rl78RewriterTests()
        {
            this.arch = new Rl78Architecture("rl78");
            this.addr = Address.Ptr32(0x01000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            var state = (Rl78ProcessorState) arch.CreateProcessorState();
            return new Rl78Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = PlatformDefinition.LoadHexBytes(hexBytes)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        private void RunTest(string hexBytes)
        {
            this.image = RewriteCode(hexBytes);
        }

        [Test]
        public void Rl78Rw_add()
        {
            RunTest("61 0A");	// add	a,c
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a + c",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_push_hl()
        {
            RunTest("C7");  // push hl
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[sp:word16] = hl");
        }

        [Test]
        public void Rl78Rw_subw()
        {
            RunTest("20 08");	// subw	sp,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|sp = sp - 8",
                "2|L--|CZ = cond(sp)");
        }

        [Test]
        public void Rl78Rw_movw_ax_hl()
        {
            RunTest("17"); // movw ax,hl
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = hl");
        }

        [Test]
        public void Rl78Rw_incw_ax()
        {
            RunTest("A1"); // incw ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = ax + 1");
        }

        [Test]
        public void Rl78Rw_clrw_bc()
        {
            RunTest("F7"); // clrw bc
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|bc = 0x0000");
        }

        [Test]
        public void Rl78Rw_mov_a_idx()
        {
            RunTest("49 10 20"); // mov a,[2010h+bc]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|a = Mem0[0x00002010 + bc:byte]");
        }

        [Test]
        public void Rl78Rw_mov_M_a()
        {
            RunTest("9B"); // mov [hl],a
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|Mem0[hl:byte] = a");
        }

        [Test]
        public void Rl78Rw_mov_a_imm()
        {
            RunTest("51 04"); // mov a,0x04
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|a = 0x04");
        }

        [Test]
        public void Rl78Rw_cmp_a_c()
        {
            RunTest("61 4A"); // cmp a,c
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|CZ = cond(a - c)");
        }

        [Test]
        public void Rl78Rw_bnz()
        {
            RunTest("DF F4"); // bnz 0000070A
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00000FF6");
        }

        [Test]
        public void Rl78Rw_pop_hl()
        {
            RunTest("C6"); // pop hl
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|hl = Mem0[sp:word16]",
                "2|L--|sp = sp + 2");
        }

        [Test]
        public void Rl78Rw_movw_ax_000A()
        {
            RunTest("30 0A 00"); // movw ax,0x000A
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|ax = 0x000A");
        }

        [Test]
        public void Rl78Rw_onew_ax()
        {
            RunTest("E6"); // onew ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = 0x0001");
        }

        [Test]
        public void Rl78Rw_call()
        {
            RunTest("FC 3E 6E 00"); // call 00006E3E
            AssertCode(
                "0|T--|00001000(4): 1 instructions",
                "1|T--|call 00006E3E (4)");
        }

        [Test]
        public void Rl78Rw_addw()
        {
            RunTest("10 08"); // addw sp,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|sp = sp + 8",
                "2|L--|CZ = cond(sp)");
        }

        [Test]
        public void Rl78Rw_mov_a_Mabs()
        {
            RunTest("8F CB FC"); // mov a,[0FCCBh]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|a = Mem0[0x0000FCCB:byte]");
        }

        [Test]
        public void Rl78Rw_sub_a_imm()
        {
            RunTest("2C 13"); // sub a,0x13
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a - 0x13",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_dec()
        {
            RunTest("92");	// dec	c
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|c = c - 1",
                "2|L--|Z = cond(c)");
        }

        [Test]
        public void Rl78Rw_cmp0()
        {
            RunTest("D5 DA F9");	// cmp0	[0F9DAh]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|CZ = cond(Mem0[0x0000F9DA:byte] - 0x00)");
        }

        [Test]
        public void Rl78Rw_cmpw()
        {
            RunTest("61 49 0A");	// cmpw	ax,[hl+0Ah]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|CZ = cond(ax - Mem0[hl + 10:word16])");
        }

        [Test]
        public void Rl78Rw_br()
        {
            RunTest("EF E3");	// br	00003C52
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|goto 00000FE5");
        }

        [Test]
        public void Rl78Rw_shrw()
        {
            RunTest("31 8E");   // shrw	ax,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = ax >>u 0x08",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_set1()
        {
            RunTest("71 00 8C 03");	// set1	[038Ch].0
            AssertCode(
                "0|L--|00001000(4): 1 instructions",
                "1|L--|__set_bit(Mem0[0x0000038C:byte], 0x00, true)");
        }

        [Test]
        public void Rl78Rw_sarw()
        {
            RunTest("31 8F");	// sarw	ax,0x08
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = ax >> 0x08",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_ret()
        {
            RunTest("D7");	// ret
            AssertCode(
                "0|T--|00001000(1): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void Rl78Rw_and()
        {
            RunTest("5C FE");	// and	a,0xFE
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a & 0xFE",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Rl78Rw_bz()
        {
            RunTest("DD 1F");	// bz	0000701B
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00001021");
        }

        [Test]
        public void Rl78Rw_bnc()
        {
            RunTest("DE 2B");	// bnc	0000705F
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0000102D");
        }

        [Test]
        public void Rl78Rw_decw()
        {
            RunTest("B1");	// decw	ax
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = ax - 1");
        }

        [Test]
        public void Rl78Rw_nop()
        {
            RunTest("00");	// nop
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Rl78Rw_oneb()
        {
            RunTest("E5 01 92");	// oneb	[9201h]
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|Mem0[0x00009201:byte] = 0x01");
        }

        [Test]
        public void Rl78Rw_sknz()
        {
            RunTest("61 F8 81 83");	// sknz
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00001003",
                "2|L--|00001002(1): 2 instructions",
                "3|L--|a = a + 1",
                "4|L--|Z = cond(a)",
                "5|L--|00001003(1): 2 instructions",
                "6|L--|b = b + 1",
                "7|L--|Z = cond(b)");

        }

        [Test]
        public void Rl78Rw_addc()
        {
            RunTest("1F D7 C1");	// addc	a,[0C1D7h]
            AssertCode(
                "0|L--|00001000(3): 2 instructions",
                "1|L--|a = a + Mem0[0x0000C1D7:byte] + C",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_xch()
        {
            RunTest("08");	// xch	a,x
            AssertCode(
                "0|L--|00001000(1): 3 instructions",
                "1|L--|v4 = a",
                "2|L--|a = x",
                "3|L--|x = v4");
        }

        [Test]
        public void Rl78Rw_xor()
        {
            RunTest("7A E8 04");	// xor	[0FFF08h],0x04
            AssertCode(
                "0|L--|00001000(3): 3 instructions",
                "1|L--|v2 = Mem0[0x000FFF08:byte] ^ 0x04",
                "2|L--|Mem0[0x000FFF08:byte] = v2",
                "3|L--|Z = cond(v2)");
        }

        [Test]
        public void Rl78Rw_reti()
        {
            RunTest("61 FC");	// reti
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void Rl78Rw_bf()
        {
            RunTest("31 74 0D 05");	// bf	[0FFE2Dh].7,000000EA
            AssertCode(
                "0|T--|00001000(4): 1 instructions",
                "1|T--|if (!__bit(Mem0[0x000FFE2D:byte], 0x07)) branch 00001009");
        }

        [Test]
        public void Rl78Rw_clr1()
        {
            RunTest("71 23 30");	// clr1	[0FFE50h].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|__set_bit(Mem0[0x000FFE50:byte], 0x02, false)");
        }

        [Test]
        public void Rl78Rw_xchw()
        {
            RunTest("37");	// xchw	ax,hl
            AssertCode(
                "0|L--|00001000(1): 3 instructions",
                "1|L--|v4 = ax",
                "2|L--|ax = hl",
                "3|L--|hl = v4");
        }

        [Test]
        public void Rl78Rw_inc()
        {
            RunTest("84");	// inc	e
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|e = e + 1",
                "2|L--|Z = cond(e)");
        }

        [Test]
        public void Rl78Rw_or()
        {
            RunTest("6B 00");	// or	a,[0FFE20h]
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a | Mem0[0x000FFE20:byte]",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Rl78Rw_subc()
        {
            RunTest("3D");	// subc	a,[hl]
            AssertCode(
                "0|L--|00001000(1): 2 instructions",
                "1|L--|a = a - Mem0[hl:byte] - C",
                "2|L--|CZ = cond(a)");
        }

        [Test]
        public void Rl78Rw_bt()
        {
            RunTest("31 23 03");	// bt	a.2,000002FF
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (__bit(a, 0x02)) branch 00001006");
        }

        [Test]
        public void Rl78Rw_bc()
        {
            RunTest("DC 06");	// bc	00000338
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00001008");
        }

        [Test]
        public void Rl78Rw_mulu()
        {
            RunTest("D6");	// mulu	x
            AssertCode(
                "0|L--|00001000(1): 1 instructions",
                "1|L--|ax = a *u x");
        }

        [Test]
        public void Rl78Rw_btclr()
        {
            RunTest("31 00 FC D6");	// btclr	[0FFF1Ch].0,00000690
            AssertCode(
                "0|T--|00001000(4): 3 instructions",
                "1|T--|if (!__bit(Mem0[0x000FFF1C:byte], 0x00)) branch 00001004",
                "2|L--|__set_bit(Mem0[0x000FFF1C:byte], 0x00, false)",
                "3|T--|goto 00000FDA");
        }

        [Test]
        public void Rl78Rw_sel()
        {
            RunTest("61 CF");	// sel	rb0
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|__select_register_bank(0x00)");
        }

        [Test]
        public void Rl78Rw_mov1()
        {
            RunTest("71 8C");	// mov1	cy,a.0
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = __bit(a, 0x00)");
        }

        [Test]
        public void Rl78Rw_rolwc()
        {
            RunTest("61 EE");	// rolwc	ax,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|ax = __rcl(ax, C, 0x01)",
                "2|L--|C = cond(ax)");
        }

        [Test]
        public void Rl78Rw_or1()
        {
            RunTest("71 2E 8F");	// or1	cy,[0FFF8Fh].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|cy = cy | __bit(Mem0[0x000FFF8F:byte], 0x02)");
        }

        [Test]
        public void Rl78Rw_xor1()
        {
            RunTest("71 2F EE");	// xor1	cy,[0FFFEEh].2
            AssertCode(
                "0|L--|00001000(3): 1 instructions",
                "1|L--|cy = cy ^ __bit(Mem0[0x000FFFEE:byte], 0x02)");
        }

        [Test]
        public void Rl78Rw_shl()
        {
            RunTest("31 69");	// shl	a,0x06
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a << 0x06",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_shr()
        {
            RunTest("31 7A");	// shr	a,0x07
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = a >>u 0x07",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_callt()
        {
            RunTest("61 D5");	// callt	[009Ah]
            AssertCode(
                "0|T--|00001000(2): 1 instructions",
                "1|T--|call Mem0[0x0000009A:word16] (4)");
        }

        [Test]
        public void Rl78Rw_bnh()
        {
            RunTest("61 D3 08");	// bnh	00004297
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (Test(ULE,CZ)) branch 0000100B");
        }

        [Test]
        public void Rl78Rw_halt()
        {
            RunTest("61 ED");	// halt
            AssertCode(
                "0|H--|00001000(2): 1 instructions",
                "1|L--|__halt()");
        }

        [Test]
        public void Rl78Rw_rolc()
        {
            RunTest("61 DC");	// rolc	a,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = __rcl(a, C, 0x01)",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_not1()
        {
            RunTest("71 C0");	// not1	cy
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = !cy");
        }

        [Test]
        public void Rl78Rw_and1()
        {
            RunTest("71 A5");	// and1	cy,[hl].2
            AssertCode(
                "0|L--|00001000(2): 1 instructions",
                "1|L--|cy = cy & __bit(Mem0[hl:byte], 0x02)");
        }

        [Test]
        public void Rl78Rw_rol()
        {
            RunTest("61 EB");	// rol	a,0x01
            AssertCode(
                "0|L--|00001000(2): 2 instructions",
                "1|L--|a = __rol(a, 0x01)",
                "2|L--|C = cond(a)");
        }

        [Test]
        public void Rl78Rw_bh()
        {
            RunTest("61 C3 27");	// bh	0000B65D
            AssertCode(
                "0|T--|00001000(3): 1 instructions",
                "1|T--|if (Test(UGT,CZ)) branch 0000102A");
        }
    }
}
