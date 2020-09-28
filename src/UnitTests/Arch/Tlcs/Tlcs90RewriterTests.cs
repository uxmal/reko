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
using Reko.Arch.Tlcs.Tlcs90;
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
    public class Tlcs90RewriterTests : RewriterTestBase
    {
        private Tlcs90Architecture arch = new Tlcs90Architecture("tlcs90");
        private Address baseAddr = Address.Ptr16(0x0100);

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            Tlcs90State state = (Tlcs90State)arch.CreateProcessorState();
            return new Tlcs90Rewriter(arch, new LeImageReader(mem, 0), state, binder, host);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Tlcs90_rw_jp()
        {
            Given_HexString("1A0001");	// jp	0100
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0100");
        }

        [Test]
        public void Tlcs90_rw_ld()
        {
            Given_HexString("EB002026");	// ld	(2000),a
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|Mem0[0x2000:byte] = a");
        }

        [Test]
        public void Tlcs90_rw_pop()
        {
            Given_HexString("58");	// pop	bc
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|bc = Mem0[sp:word16]",
                "2|L--|sp = sp + 2");
        }

        [Test]
        public void Tlcs90_rw_ret()
        {
            Given_HexString("1E");	// ret
            AssertCode(
                "0|T--|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void Tlcs90_rw_push()
        {
            Given_HexString("50");	// push	bc
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[sp:word16] = bc");
        }

        [Test]
        public void Tlcs90_rw_ld_iy_nn()
        {
            Given_HexString("E300404D");    // ld\tiy,(4000)
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|v2 = Mem0[0x4000:word16]",
                "2|L--|iy = v2");
        }


        [Test]
        public void Tlcs90_rw_halt()
        {
            Given_HexString("01");	// halt
            AssertCode(
                "0|H--|0100(1): 1 instructions",
                "1|H--|__halt()");
        }

        [Test]
        public void Tlcs90_rw_di()
        {
            Given_HexString("02");	// di
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|__disable_interrupts()");
        }

        [Test]
        public void Tlcs90_rw_ldw()
        {
            Given_HexString("3F3F3412");	// ldw (3fh),1234h
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|Mem0[0xFF3F:word16] = 0x1234");
        }

        [Test]
        public void Tlcs90_rw_add()
        {
            Given_HexString("6869");    // add A,69h
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a + 0x69",
                "2|L--|N = false",
                "3|L--|SZHXVC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_and()
        {
            Given_HexString("F964");    // and a,c
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|a = a & c",
                "2|L--|H = true",
                "3|L--|X = false",
                "4|L--|N = false",
                "5|L--|C = false",
                "6|L--|SZV = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_or()
        {
            Given_HexString("F966");    // or a,c
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|a = a | c",
                "2|L--|H = false",
                "3|L--|X = false",
                "4|L--|N = false",
                "5|L--|C = false",
                "6|L--|SZV = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_reti()
        {
            Given_HexString("1F");  // reti
            AssertCode(
                "0|T--|0100(1): 3 instructions",
                "1|L--|af = Mem0[sp:word16]",
                "2|L--|sp = sp + 2",
                "3|T--|return (2,0)");
        }

        [Test]
        public void Tlcs90_rw_rrc()
        {
            Given_HexString("A1");  // rrc
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = __ror(a, 0x01)",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_swi()
        {
            Given_HexString("FF");  // swi
            AssertCode(
                "0|T--|0100(1): 3 instructions",
                "1|L--|sp = sp - 2",
                "2|L--|Mem0[sp:word16] = af",
                "3|T--|call 0100 (2)");
        }

        [Test]
        public void Tlcs90_rw_jr()
        {
            Given_HexString("C6FD");  // jr z
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00FF");
        }

        [Test]
        public void Tlcs90_rw_sra()
        {
            Given_HexString("A5");  // sra
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = a >> 1",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_ex()
        {
            Given_HexString("09");  // ex
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = af",
                "2|L--|af = af'",
                "3|L--|af' = v4");
        }

        [Test]
        public void Tlcs90_rw_set()
        {
            Given_HexString("E33412B9");  // set
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|v2 = Mem0[0x1234:byte] | 0x02",
                "2|L--|Mem0[0x1234:byte] = v2");
        }

        [Test]
        public void Tlcs90_rw_call()
        {
            Given_HexString("1C0002");  // call
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 0200 (2)");
        }
        [Test]
        public void Tlcs90_rw_callr()
        {
            Given_HexString("1DFDFF");  // callr
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 0100 (2)");
        }

        [Test]
        public void Tlcs90_rw_sla()
        {
            Given_HexString("F3A4");  // sla
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|v4 = Mem0[hl + (int16) a:byte]",
                "2|L--|v5 = v4 << 1",
                "3|L--|Mem0[hl + (int16) a:byte] = v5",
                "4|L--|H = false",
                "5|L--|N = false",
                "6|L--|SZXC = cond(v5)");
        }

        [Test]
        public void Tlcs90_rw_inc_bytereg()
        {
            Given_HexString("82");  // inc
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|d = d + 1",
                "2|L--|N = false",
                "3|L--|SZHXV = cond(d)");
        }

        [Test]
        public void Tlcs90_rw_inc_wordreg()
        {
            Given_HexString("92");  // inc
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|hl = hl + 1",
                "2|L--|X = cond(hl)");
        }

        [Test]
        public void Tlcs90_rw_cp()
        {
            Given_HexString("6F04");  // cp
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|N = true",
                "2|L--|SZHXV = cond(a - 0x04)");
        }

        [Test]
        public void Tlcs90_rw_res()
        {
            Given_HexString("E4B4");  // res
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|v3 = Mem0[ix:byte] & 0xEF",
                "2|L--|Mem0[ix:byte] = v3");
        }

        [Test]
        public void Tlcs90_rw_decx()
        {
            Given_HexString("0F20");  // decx
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|T--|if (!X) branch 0102",
                "2|L--|v3 = Mem0[0xFF20:byte]",
                "3|L--|v4 = v3 - 1",
                "4|L--|Mem0[0xFF20:byte] = v4",
                "5|L--|N = true",
                "6|L--|SZHXV = cond(v4)");
        }

        [Test]
        public void Tlcs90_rw_incw()
        {
            Given_HexString("F0F097");  // incw
            AssertCode(
                "0|L--|0100(3): 5 instructions",
                "1|L--|v3 = Mem0[ix + -16:word16]",
                "2|L--|v4 = v3 + 1",
                "3|L--|Mem0[ix + -16:word16] = v4",
                "4|L--|N = false",
                "5|L--|SZHXV = cond(v4)");
        }

        [Test]
        public void Tlcs90_rw_cpl()
        {
            Given_HexString("10");  // cpl
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = ~a",
                "2|L--|H = true",
                "3|L--|N = true");
        }

        [Test]
        public void Tlcs90_rw_bit()
        {
            Given_HexString("F9A9");  // bit
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|Z = (c & 0x02) == 0x00",
                "2|L--|N = false",
                "3|L--|SHXV = cond(c)");
        }

        [Test]
        public void Tlcs90_rw_scf()
        {
            Given_HexString("0D");  // scf
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|C = true");
        }

        [Test]
        public void Tlcs90_rw_sbc()
        {
            Given_HexString("6B00");  // sbc
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a - 0x00 - C",
                "2|L--|N = true",
                "3|L--|SZHXVC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_ldir()
        {
            Given_HexString("FE59");  // ldir
            AssertCode(
                "0|L--|0100(2): 9 instructions",
                "1|L--|v2 = Mem0[hl:byte]",
                "2|L--|Mem0[de:byte] = v2",
                "3|L--|hl = hl + 1",
                "4|L--|de = de + 1",
                "5|L--|bc = bc - 1",
                "6|T--|if (bc != 0x0000) branch 0100",
                "7|L--|H = false",
                "8|L--|V = false",
                "9|L--|N = false");
        }

        [Test]
        public void Tlcs90_rw_call_cc()
        {
            Given_HexString("F4 37 D2");    // call cc,xxxx
            AssertCode(
                "0|T--|0100(3): 3 instructions",
                "1|T--|if (Test(GT,SZV)) branch 0103",
                "2|L--|v4 = Mem0[ix + 55:ptr16]",
                "3|T--|call v4 (2)");
        }  
        
        [Test]
        public void Tlcs90_rw_ld_regression()
        {
            Given_HexString("F6 24 42");    // ld(sp + 0x24),hl
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|Mem0[sp + 36:word16] = hl");
        }

        [Test]
        public void Tlcs90_rw_jp_conditional()
        {
            Given_HexString("EB 50 03 CE"); //  jp NZ,0350
            AssertCode(
                "0|T--|0100(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0350");
        }

        [Test]
        public void Tlcs90_rw_rcf()
        {
            Given_HexString("0C");	// rcf
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|C = false");
        }

        [Test]
        public void Tlcs90_rw_xor()
        {
            Given_HexString("FE65");    // xor	a,a
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|a = a ^ a",
                "2|L--|H = true",
                "3|L--|X = false",
                "4|L--|N = false",
                "5|L--|C = false",
                "6|L--|SZV = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_xor_mem()
        {
            Given_HexString("65F4");	// xor	a,(FFF4)
            AssertCode(
                "0|L--|0100(2): 7 instructions",
                "1|L--|v2 = Mem0[0xFFF4:byte]",
                "2|L--|a = a ^ v2",
                "3|L--|H = true",
                "4|L--|X = false",
                "5|L--|N = false",
                "6|L--|C = false",
                "7|L--|SZV = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_adc_byte()
        {
            Given_HexString("EF2E6979");	// adc	(FF2E),79
            AssertCode(
                "0|L--|0100(4): 4 instructions",
                "1|L--|v3 = Mem0[0xFF2E:byte] + 0x79 + C",
                "2|L--|Mem0[0xFF2E:byte] = v3", 
                "3|L--|N = false",
                "4|L--|SZHXVC = cond(v3)");
        }

        [Test]
        public void Tlcs90_rw_adc_word()
        {
            Given_HexString("79F4EF");	// adc	hl,EFF4
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|hl = hl + 0xEFF4 + C",
                "2|L--|N = false",
                "3|L--|SZHXVC = cond(hl)");
        }

        [Test]
        public void Tlcs90_rw_rl()
        {
            Given_HexString("F8A2");	// rl	b
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|b = __rcl(b, 0x01, C)",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(b)");
        }

        [Test]
        public void Tlcs90_rw_rl_a()
        {
            Given_HexString("A2");	// rl
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = __rcl(a, 0x01, C)",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_srl()
        {
            Given_HexString("FCA7");	// srl	h
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|h = h >>u 1",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(h)");
        }

        [Test]
        public void Tlcs90_rw_srl_a()
        {
            Given_HexString("A7");	// srl
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = a >>u 1",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_rr()
        {
            Given_HexString("FDA3");	// rr	l
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|l = __rcr(l, 0x01, C)",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(l)");
        }

        [Test]
        public void Tlcs90_rw_rr_a()
        {
            Given_HexString("A3");	// rr
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = __rcr(a, 0x01, C)",
                "2|L--|H = false",
                "3|L--|N = false",
                "4|L--|SZXC = cond(a)");
        }

        [Test]
        public void Tlcs90_ex_mem()
        {
            Given_HexString("E6 52"); // ex(sp),hl
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v3 = Mem0[sp:word16]",
                "2|L--|v5 = v3",            //$TODO inefficient copy introduced here.
                "3|L--|Mem0[sp:word16] = hl",
                "4|L--|hl = v5");
        }

        [Test]
        public void Tlcs90_rw_djnz()
        {
            Given_HexString("18FE");	// djnz	0100
            AssertCode(
                "0|T--|0100(2): 2 instructions",
                "1|L--|b = b - 1",
                "2|T--|if (b != 0x00) branch 0100");
        }

        [Test]
        public void Tlcs90_rw_ccf()
        {
            Given_HexString("0E");	// ccf
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|C = !C");
        }

        [Test]
        public void Tlcs90_rw_mul()
        {
            Given_HexString("1203");	// mul	hl,03
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|hl = l * 0x03");
        }

        [Test]
        public void Tlcs90_rw_sub()
        {
            Given_HexString("F962");	// sub	a,c
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a - c",
                "2|L--|N = true",
                "3|L--|SZHXVC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_sub_mem()
        {
            Given_HexString("62F4");	// sub	a,(FFF4)
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v2 = Mem0[0xFFF4:byte]",
                "2|L--|a = a - v2",
                "3|L--|N = true",
                "4|L--|SZHXVC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_exx()
        {
            Given_HexString("0A");	// exx
            AssertCode(
                "0|L--|0100(1): 9 instructions",
                "1|L--|v2 = bc",
                "2|L--|bc = bc'",
                "3|L--|bc' = v2");
        }

        [Test]
        public void Tlcs90_rw_daa()
        {
            Given_HexString("0B");	// daa	a
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = __daa(a)",
                "2|L--|SZHXC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_sll()
        {
            Given_HexString("A6");	// sll
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|a = a << 1");
        }

        [Test]
        public void Tlcs90_rw_neg()
        {
            Given_HexString("11");	// neg a
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = -a",
                "2|L--|N = true",
                "3|L--|SZHXVC = cond(a)");
        }

        [Test]
        public void Tlcs90_rw_ldar()
        {
            Given_HexString("170000");	// ldar hl,(0103)
            AssertCode(
                "0|L--|0100(3): 1 instructions",
                "1|L--|hl = 0103");
        }
        
        [Test]
        public void Tlcs_rw_inc_ix()
        {
            Given_HexString("F0 ED 87");    //  inc (ix-0x13)
            AssertCode(
              "0|L--|0100(3): 5 instructions",
              "1|L--|v3 = Mem0[ix + -19:byte]",
              "2|L--|v4 = v3 + 1",
              "3|L--|Mem0[ix + -19:byte] = v4",
              "4|L--|N = false",
              "5|L--|SZHXV = cond(v4)");
        }

        [Test]
        public void Tlcs90_rw_div()
        {
            Given_HexString("134A");  // div	hl,4A
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v5 = hl",
                "2|L--|l = v5 / 0x4A",
                "3|L--|h = v5 % 0x4A",
                "4|L--|V = cond(l)");
        }

        [Test]
        public void Tlcs90_rw_goto_hl()
        {
            Given_HexString("EAC8");
            AssertCode(
                "0|T--|0100(2): 2 instructions",
                "1|L--|v3 = Mem0[hl:ptr16]",
                "2|T--|goto v3");
        }

        [Test]
        public void Tlcs90_rw_tset()
        {
            Given_HexString("FB1B");    // tset 3,e
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|Z = (e & 0x08) == 0x00",
                "2|L--|N = false",
                "3|L--|SHXV = cond(e)",
                "4|L--|e = e | 0x08");
        }


        [Test]
        public void Tlcs90_rw_ConditionalJump()
        {
            Given_HexString("EB350ACC");    // jp NV,0A35
            AssertCode(
                "0|T--|0100(4): 1 instructions",
                "1|T--|if (Test(NO,V)) branch 0A35");
        }
    }
}