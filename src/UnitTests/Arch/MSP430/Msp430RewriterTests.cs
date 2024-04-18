#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Msp430;
using Reko.Core;

namespace Reko.UnitTests.Arch.Msp430
{
    public class Msp430RewriterTests : RewriterTestBase
    {
        private readonly Msp430Architecture arch;
        private readonly Address addr;

        public Msp430RewriterTests()
        {
            this.arch = new Msp430Architecture(CreateServiceContainer(), "msp430", new());
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        [Test]
        public void Msp430Rw_mov()
        {
            Given_Bytes(0x3C, 0x40, 0xA0, 0xEE);	// mov.w	#EEA0,r12
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|r12 = 0xEEA0<16>");
        }

        [Test]
        public void Msp430Rw_xor()
        {
            Given_Bytes(0xA0, 0xEE, 0x3C, 0x90);	// xor.w	@r14,-6FC4(pc)
            AssertCode(
                "0|L--|0100(4): 5 instructions",
                "1|L--|v4 = Mem0[r14:word16]",
                "2|L--|v5 = Mem0[0x913E<p16>:word16]",
                "3|L--|v5 = v5 ^ v4",
                "4|L--|Mem0[0x913E<p16>:word16] = v5",
                "5|L--|VNZC = cond(v5)");
        }

        [Test]
        public void Msp430Rw_cmp()
        {
            Given_Bytes(0x3C, 0x90, 0xA0, 0xEE);	// cmp.w	#EEA0,r12
            AssertCode(
                "0|L--|0100(4): 1 instructions",
                "1|L--|VNZC = cond(r12 - 0xEEA0<16>)");
        }

        [Test]
        public void Msp430Rw_jz()
        {
            Given_Bytes(0x07, 0x24);	// jz	0118
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0110");
        }

        [Test]
        public void Msp430Rw_call()
        {
            Given_Bytes(0x8D, 0x12);	// call	r13
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|call r13 (2)");
        }

        [Test]
        public void Msp430Rw_sub_w()
        {
            Given_Bytes(0x3D, 0x80, 0xA0, 0xEE);	// sub.w	#EEA0,r13
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|r13 = r13 - 0xEEA0<16>",
                "2|L--|VNZC = cond(r13)");
        }

        [Test]
        public void MSP430Rw_rra_b()
        {
            Given_HexString("4411");
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r4 = r4 >> 1<8>",
                "2|L--|NZC = cond(r4)",
                "3|L--|V = false");
        }

        [Test]
        public void Msp430Rw_rra_w()
        {
            Given_Bytes(0x0D, 0x11);	// rra.w	r13
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r13 = r13 >> 1<8>",
                "2|L--|NZC = cond(r13)",
                "3|L--|V = false");
        }

        [Test]
        public void Msp430Rw_add()
        {
            Given_Bytes(0x0D, 0x5C);	// add.w	r12,r13
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r13 = r13 + r12",
                "2|L--|VNZC = cond(r13)");
        }

        [Test]
        public void Msp430Rw_jnz()
        {
            Given_Bytes(0x22, 0x20);	// jnz	0190
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0146");
        }

        [Test]
        public void Msp430Rw_jc()
        {
            Given_Bytes(0x0B, 0x2C);	// jc	017A
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 0118");
        }

        [Test]
        public void Msp430Rw_jnc()
        {
            Given_Bytes(0xF5, 0x2B);	// jnc	0164
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 00EC");
        }

        [Test]
        public void Msp430Rw_addc()
        {
            Given_Bytes(0x6A, 0x64);	// addc.b	@r4,r10
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|v5 = Mem0[r4:byte]",
                "2|L--|v7 = SLICE(r10, byte, 0)",
                "3|L--|v8 = v7 + v5 + C",
                "4|L--|r10 = CONVERT(v8, byte, word16)",
                "5|L--|VNZC = cond(v8)");
        }

        [Test]
        public void Msp430Rw_jge()
        {
            Given_Bytes(0x07, 0x34);	// jge	01FC
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(GE,VN)) branch 0110");
        }

        [Test]
        public void Msp430Rw_jmp()
        {
            Given_Bytes(0xF8, 0x3F);	// jmp	01F6
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 00F2");
        }

        [Test]
        public void Msp430Rw_jl()
        {
            Given_Bytes(0x12, 0x38);	// jl	0272
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(LT,VN)) branch 0126");
        }

        [Test]
        public void Msp430Rw_and()
        {
            Given_Bytes(0xFE, 0xFF, 0xF9, 0x3F);	// and.b	@r15+,3FF9(r14)
            AssertCode(
                "0|L--|0100(4): 7 instructions",
                "1|L--|v4 = Mem0[r15:byte]",
                "2|L--|r15 = r15 + 1<i16>",
                "3|L--|v6 = Mem0[r14 + 16377<i16>:byte]",
                "4|L--|v6 = v6 & v4",
                "5|L--|Mem0[r14 + 16377<i16>:byte] = v6",
                "6|L--|NZC = cond(v6)",
                "7|L--|V = false");
        }

        [Test]
        public void Msp430Rw_subc()
        {
            Given_Bytes(0xB1, 0x79, 0x0E, 0x20);	// subc.w	@r9+,200E(sp)
            AssertCode(
                "0|L--|0100(4): 6 instructions",
                "1|L--|v5 = Mem0[r9:word16]",
                "2|L--|r9 = r9 + 2<i16>",
                "3|L--|v7 = Mem0[sp + 8206<i16>:word16]",
                "4|L--|v7 = v7 - v5 - C",
                "5|L--|Mem0[sp + 8206<i16>:word16] = v7",
                "6|L--|VNZC = cond(v7)");
        }

        [Test]
        public void Msp430_dint()
        {
            //  C232 bic.w #0008,sr
            Given_HexString("32C2");
            AssertCode(     // bis.w #0008,sr
                "0|L--|0100(2): 1 instructions",
                "1|L--|__disable_interrupts()");
        }

        [Test]
        public void Msp430_eint()
        {
            Given_HexString("32D2");
            AssertCode(     // bis.w #0008,sr
                "0|L--|0100(2): 1 instructions",
                "1|L--|__enable_interrupts()");
        }

        [Test]
        public void Msp430Rw_bis()
        {
            Given_Bytes(0x0C, 0xDD);	// bis.w	r13,r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r12 = r12 | r13");
        }

        [Test]
        public void Msp430Rw_bit()
        {
            Given_Bytes(0x66, 0xB1);	// bit.b	@sp,r6
            AssertCode(
                "0|L--|0100(2): 6 instructions",
                "1|L--|v5 = Mem0[sp:byte]",
                "2|L--|v6 = CONVERT(v5, byte, word16)",
                "3|L--|v7 = r6 & v6",
                "4|L--|NZ = cond(v7)",
                "5|L--|C = Test(NE,v7)",
                "6|L--|V = false");
        }

        [Test]
        public void Msp430Rw_bic()
        {
            Given_Bytes(0x16, 0xCB, 0x1C, 0x4A);	// bic.w	4A1C(r11),r6
            AssertCode(
                "0|L--|0100(4): 2 instructions",
                "1|L--|v4 = Mem0[r11 + 18972<i16>:word16]",
                "2|L--|r6 = r6 & ~v4");
        }

        [Test]
        public void Msp430Rw_rrc()
        {
            Given_Bytes(0x04, 0x10);	// rrc.w	pc
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|r4 = __rcr<word16,byte>(r4, 1<8>, C)",
                "2|L--|NZC = cond(r4)",
                "3|L--|V = false");
        }

        [Test]
        public void Msp430Rw_dadd()
        {
            Given_Bytes(0xB0, 0xA4, 0x3E, 0x40);	// dadd.w	@r4+,403E(pc)
            AssertCode(
                "0|L--|0100(4): 6 instructions",
                "1|L--|v4 = Mem0[r4:word16]",
                "2|L--|r4 = r4 + 2<i16>",
                "3|L--|v5 = Mem0[0x4140<p16>:word16]",
                "4|L--|v5 = __dadd<word16>(v5, v4, C)",
                "5|L--|Mem0[0x4140<p16>:word16] = v5",
                "6|L--|NZC = cond(v5)");
        }

        [Test]
        public void Msp430Rw_jn()
        {
            Given_Bytes(0x72, 0x31);	// jn	78E0
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(SG,N)) branch 03E6");
        }

        [Test]
        public void Msp430Rw_sxt()
        {
            Given_Bytes(0x8F, 0x11);	// sxt.w	r15
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v4 = SLICE(r15, byte, 0)",
                "2|L--|r15 = CONVERT(v4, byte, word16)",
                "3|L--|NZC = cond(r15)",
                "4|L--|V = false");
        }

        [Test]
        public void Msp430Rw_swpb()
        {
            Given_Bytes(0x8F, 0x10);	// swpb	r15
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|r15 = __swpb(r15)");
        }

        [Test]
        public void Msp430Rw_swpb_mem()
        {
            Given_HexString("A110"); // A39022DE16323851D8D4877F2AC2");
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|v4 = Mem0[sp:word16]",
                "2|L--|Mem0[sp:word16] = __swpb(v4)");
        }

        [Test]
        public void Msp430Rw_sub_sp()
        {
            Given_Bytes(0x21, 0x83); // sub.w #0002,sp
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|sp = sp - 2<16>",
                "2|L--|VNZC = cond(sp)");
        }

        [Test]
        public void Msp430Rw_goto()
        {
            Given_Bytes(0x30, 0x40, 0x4C, 0x41);
            AssertCode(         // "mov.w\t#414C,pc"
                "0|T--|0100(4): 1 instructions",
                "1|T--|goto 414C");
        }

        [Test]
        public void Msp430Rw_ret()
        {
            Given_Bytes(0x30, 0x41);
            AssertCode(         // ret
                "0|R--|0100(2): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void MSP430Rw_add_two_abs()
        {
            Given_HexString("9252 9C57 7877");
            AssertCode(         // add.w\t&579C,&7778
                "0|L--|0100(6): 5 instructions",
                "1|L--|v3 = Mem0[0x579C<p16>:word16]",
                "2|L--|v4 = Mem0[0x7778<p16>:word16]",
                "3|L--|v4 = v4 + v3",
                "4|L--|Mem0[0x7778<p16>:word16] = v4",
                "5|L--|VNZC = cond(v4)");
        }

        [Test]
        public void MSP430Rw_add_b_pcrel_pcrel()
        {
            Given_HexString("D050 6647 40F7");
            AssertCode(         // add.b\t4768(pc),-08BC(pc)
                "0|L--|0100(6): 5 instructions",
                "1|L--|v4 = Mem0[0x4868<p16>:byte]",
                "2|L--|v5 = Mem0[0xF844<p16>:byte]",
                "3|L--|v5 = v5 + v4",
                "4|L--|Mem0[0xF844<p16>:byte] = v5",
                "5|L--|VNZC = cond(v5)");
        }

        [Test]
        public void MSP430Rw_add_w()
        {
            Given_HexString("1E53");
            AssertCode(         // add.w #0001,r14
                "0|L--|0100(2): 2 instructions",
                "1|L--|r14 = r14 + 1<16>",
                "2|L--|VNZC = cond(r14)");
        }

        [Test]
        public void Msp430rw_regression1()
        {
            Given_HexString("5F 4C 04 00");
            AssertCode(         // mov.b 0004(r12),r15
                "0|L--|0100(4): 3 instructions",
                "1|L--|v4 = Mem0[r12 + 4<i16>:byte]",
                "2|L--|v6 = CONVERT(v4, byte, word16)",
                "3|L--|r15 = v6");
        }
    }
}
