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
using Reko.Arch.Avr;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr8RewriterTests : RewriterTestBase
    {
        private Avr8Architecture arch;
        private Address baseAddr = Address.Ptr16(0x0100);

        public Avr8RewriterTests()
        {
            this.arch = new Avr8Architecture("avr8");
        }

        public override IProcessorArchitecture Architecture => arch;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = (Avr8State)arch.CreateProcessorState();
            return new Avr8Rewriter(arch, new LeImageReader(mem, 0), state, new Frame(arch.FramePointerType), host);
        }

        public override Address LoadAddress => baseAddr;



        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Avr8_rw_rjmp()
        {
            Given_UInt16s(0xC00C); // "rjmp\t001A"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 011A");
        }

        [Test]
        public void Avr8_rw_eor()
        {
            Given_UInt16s(0x2411); // "eor\tr1,r1"
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r1 = r1 ^ r1",
                "2|L--|SNZC = cond(r1)",
                "3|L--|V = false");
        }

        [Test]
        public void Avr8_rw_out()
        {
            Given_UInt16s(0xBE1F); // "out 3F,r1"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|sreg = r1");
            Given_UInt16s(0xBE16); // "out\t36,r1"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|__out(0x36, r1)");
        }

        [Test]
        public void Avr8_rw_in()
        {
            //$TODO: well known ports, like 0x3F = sreg
            Given_UInt16s(0xB617); // "in\tr1,37"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r1 = __in(0x37)");
            Given_UInt16s(0xB61F); // "in\tr1,3F"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r1 = sreg");
        }

        [Test]
        public void Avr8_rw_ldi()
        {
            Given_UInt16s(0xE5CF); // "ldi\tr28,5F"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r28 = 0x5F");
        }

        [Test]
        public void Avr8_rw_rcall()
        {
            Given_UInt16s(0xD002); // "rcall\t0006"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|call 0106 (2)");
        }

        [Test]
        public void Avr8_rw_push()
        {
            Given_UInt16s(0x93DF); // "push\tr29"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|SP = SP - 1",
                "2|L--|Mem0[SP:byte] = r29");
        }

        [Test]
        public void Avr8_rw_pop()
        {
            Given_UInt16s(0x91CF); // "pop\tr28"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r28 = Mem0[SP:byte]",
                "2|L--|SP = SP + 1");
        }

        [Test]
        public void Avr8_rw_ret()
        {
            Given_UInt16s(0x9508); // "ret"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void Avr8_rw_cli()
        {
            Given_UInt16s(0x94F8); // "cli"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|__cli()");
        }

        [Test]
        public void Avr8_rw_com()
        {
            Given_UInt16s(0x9400); // "com\tr0"
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|r0 = ~r0",
                "2|L--|SNZ = cond(r0)",
                "3|L--|V = false",
                "4|L--|C = true");
        }

        [Test]
        public void Avr8_rw_neg()
        {
            Given_UInt16s(0x9401); // "neg\tr0"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r0 = -r0",
                "2|L--|HSVNZC = cond(r0)");
        }

        [Test]
        public void Avr8_rw_swap()
        {
            Given_UInt16s(0x9402); // "swap\tr0"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r0 = __swap(r0)");
        }

        [Test]
        public void Avr8_rw_inc()
        {
            Given_UInt16s(0x9403); // "inc\tr0"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r0 = r0 + 1",
                "2|L--|SVNZ = cond(r0)");
        }

        [Test]
        public void Avr8_rw_asr()
        {
            Given_UInt16s(0x9405); // "asr\tr0"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r0 = r0 >> 0x01",
                "2|L--|SVNZC = cond(r0)");
        }

        [Test]
        public void Avr8_rw_lsr()
        {
            Given_UInt16s(0x9406); // "lsr\tr0"
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r0 = r0 >>u 0x01",
                "2|L--|SVZC = cond(r0)",
                "3|L--|N = false");
        }

        [Test]
        public void Avr8_rw_ror()
        {
            Given_UInt16s(0x9407); // "ror\tr0"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r0 = __rcr(r0, 1, C)",
                "2|L--|HSVNZC = cond(r0)");
        }

        [Test]
        public void Avr8_rw_sec()
        {
            Given_UInt16s(0x9408); // "sec"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|C = true");
        }

        [Test]
        public void Avr8_rw_ijmp()
        {
            Given_UInt16s(0x9409); // "ijmp"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto z");
        }

        [Test]
        public void Avr8_rw_dec()
        {
            Given_UInt16s(0x940A); // "dec\tr0"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r0 = r0 - 1",
                "2|L--|SVNZ = cond(r0)");
        }

        [Test]
        public void Avr8_rw_des()
        {
            Given_UInt16s(0x940B); // "des\t00"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|__des(0x00, H)");
        }

        [Test]
        public void Avr8_rw_jmp()
        {
            Given_UInt16s(0x958C, 0x0000); // "jmp\t00600000"
            AssertCode(
                "0|T--|0100(4): 1 instructions",
                "1|T--|goto 00600000");
        }

        [Test]
        public void Avr8_rw_call()
        {
            Given_UInt16s(0x95FF, 0x9234); // "call\t007F2468"
            AssertCode(
                "0|T--|0100(4): 1 instructions",
                "1|T--|call 007F2468 (2)");
        }

        [Test]
        public void Avr8_rw_cpi()
        {
            Given_UInt16s(0x30A9); // "cpi\tr26,09"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|HSVNZC = r26 - 0x09");
        }

        [Test]
        public void Avr8_rw_cpc()
        {
            Given_UInt16s(0x0524); // "cpc\tr18,r4"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|HSVNZC = r18 - r4 - C");
        }

        [Test]
        public void Avr8_rw_brcc()
        {
            Given_UInt16s(0xF000); // "brcs\t0102"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 0102");
            Given_UInt16s(0xF4FF); // "brid\t003E"
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (!I) branch 0140");
        }

        [Test]
        public void Avr8_rw_movw()
        {
            Given_UInt16s(0x01FE); // "movw\tr30,r28"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r31_r30 = r29_r28");
        }

        [Test]
        public void Avr8_rw_adiw()
        {
            Given_UInt16s(0x9601); // "adiw\tr24,01"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r25_r24 = r25_r24 + 0x0001");
        }

        [Test]
        public void Avr8_rw_adc()
        {
            Given_UInt16s(0x1DA1); // "adc\tr26,r1"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r26 = r26 + r1 + C",
                "2|L--|HSVNZC = cond(r26)");
        }

        [Test]
        public void Avr8_rw_sts()
        {
            Given_UInt16s(0x9210, 0x1234); // "sts\t1234,r1"
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|Mem0[0x1234:byte] = r1");
        }

        [Test]
        public void Avr8_rw_st_z()
        {
            Given_UInt16s(0x8380); // "st\tZ,r24"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[z:byte] = r24");
        }

        [Test]
        public void Avr8_rw_st_z_postinc()
        {
            Given_UInt16s(0x9291); // "st\tZ+,r9"
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|Mem0[z:byte] = r9",
                "2|L--|z = z + 1");
        }

        [Test]
        public void Avr8_rw_sbis()
        {
            Given_UInt16s(0x9BA8, 0x9291); // "sbis\t05,00; "st\tZ+,r9
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (__bit_set(__in(0x05), 0x00)) branch 0104",
                "2|L--|0102(2): 2 instructions",
                "3|L--|Mem0[z:byte] = r9",
                "4|L--|z = z + 1");
        }

        [Test]
        public void Avr8_rw_ld()
        {
            Given_UInt16s(0x8180); // "ld\tr24,X"
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r24 = Mem0[x:byte]");
        }

        [Test]
        public void Avr8_rw_lds()
        {
            Given_UInt16s(0x9120, 0x0080); // "lds\tr18,0080"
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|r18 = Mem0[0x0080:byte]");
        }

        [Test]
        public void Avr8_rw_lpm()
        {
            Given_UInt16s(0x95C8); // lpm
            AssertCode(
                  "0|L--|0100(2): 1 instructions",
                  "1|L--|r0 = Mem0[code:z:byte]");
            Given_UInt16s(0x9195); // lpm\tr25,Z+
            AssertCode(
                  "0|L--|0100(2): 2 instructions",
                  "1|L--|r25 = Mem0[code:z:byte]",
                  "2|L--|z = z + 1");
        }

        [Test]
        public void Avr8_rw_cpse()
        {
            Given_UInt16s(0x1181, 0x8180);	// cpse	r24,r1; ld r24,X
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (r24 == r1) branch 0104",
                "2|L--|0102(2): 1 instructions",
                "3|L--|r24 = Mem0[x:byte]");
        }

        [Test]
        public void Avr8_rw_ldd()
        {
            Given_UInt16s(0x818E);	// ldd	r24,y+06
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r24 = Mem0[y + 6:byte]");
        }

                [Test]
        public void Avr8_rw_ldd_z()
        {
            Given_UInt16s(0x8964);	
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r22 = Mem0[z + 20:byte]");
        }

        [Test]
        public void Avr8_rw_std()
        {
            Given_UInt16s(0x8213);	// std	z+0B,r1
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|Mem0[z + 3:byte] = r1");
        }

        [Test]
        public void Avr8_rw_brpl()
        {
            Given_UInt16s(0xF7E2);	// brpl	0364
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(GE,N)) branch 00FA");
        }

        [Test]
        public void Avr8_rw_sbrs()
        {
            Given_UInt16s(0xFF84, 0x8213);	// sbrs	r24,04; std	z+0B,r1
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if ((r24 & 0x10) != 0x00) branch 0104",
                "2|L--|0102(2): 1 instructions",
                "3|L--|Mem0[z + 3:byte] = r1");
        }


        [Test]
        public void Avr8_rw_sbrc()
        {
            Given_UInt16s(0xFD84, 0x8213);	// sbrc	r24,04; std	z+0B,r1
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if ((r24 & 0x10) == 0x00) branch 0104",
                "2|L--|0102(2): 1 instructions",
                "3|L--|Mem0[z + 3:byte] = r1");
        }

        [Test]
        public void Avr8_rw_muls()
        {
            Given_UInt16s(0x02E2);	// muls	r30,r18
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r1_r0 = r30 *s r18",
                "2|L--|C = r1_r0 < 0x0000",
                "3|L--|Z = r1_r0 == 0x0000");
        }
    }
}
