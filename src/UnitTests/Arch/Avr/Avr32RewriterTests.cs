#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr32RewriterTests : RewriterTestBase
    {
        public Avr32RewriterTests()
        {
            this.Architecture = new Avr32Architecture(CreateServiceContainer(), "avr32", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void Given_Instruction(string hexBytes)
        {
            Given_HexString(hexBytes);
        }

        [Test]
        public void Avr32Rw_abs()
        {
            Given_Instruction("5C4B");	// abs	r11
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v2 = r11",
                "2|L--|r11 = abs(v2)",
                "3|L--|Z = r11 == 0<32>");
        }

        [Test]
        public void Avr32Rw_acall()
        {
            Given_Instruction("D7D0");	// acall	+000001F4
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call Mem0[acba + 500<i32>:word32] (0)");
        }

        [Test]
        public void Avr32Rw_acr()
        {
            Given_Instruction("5C00");	// acr	r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r0 + C",
                "2|L--|VNZC = cond(r0)");
        }

        [Test]
        public void Avr32Rw_adc()
        {
            Given_Instruction("E20B0041");	// adc	r1,r1,r11
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + r11 + C",
                "2|L--|VNZC = cond(r1)");
        }

        [Test]
        public void Avr32Rw_add2()
        {
            Given_Instruction("0000");	// add	r0,r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r0 + r0",
                "2|L--|VNZC = cond(r0)");
        }

        [Test]
        public void Avr32Rw_and()
        {
            Given_Instruction("0061");	// and	r1,r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r1 = r1 & r0",
                "2|L--|NZ = cond(r1)");
        }

        [Test]
        public void Avr32Rw_andl()
        {
            Given_Instruction("E018F000");	// andl	r8,F000
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = r8 & 0xFFFFF000<32>",
                "2|L--|NZ = cond(r8)");
        }

        [Test]
        public void Avr32Rw_andnot()
        {
            Given_Instruction("0085");	// andnot	r5,r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r5 = r5 & ~r0",
                "2|L--|NZ = cond(r5)");
        }

        [Test]
        public void Avr32Rw_bfexts()
        {
            Given_HexString("F7D5D3C2");
            AssertCode(     // bfexts	r11,r5,+0000001E,+00000002
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = SLICE(r5, int2, 30)",
                "2|L--|NZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_bld()
        {
            Given_HexString("EDBB0014");
            AssertCode(     // bld	r11,+00000014
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = SLICE(r11, bool, 20)",
                "2|L--|Z = v3",
                "3|L--|C = v3");
        }

        [Test]
        public void Avr32Rw_brlt()
        {
            Given_Instruction("C035");	// brlt	00010222
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(LT,VN)) branch 00100006");
        }

        [Test]
        public void Avr32Rw_bst()
        {
            Given_HexString("EFBB001F");
            AssertCode(     // bst	r11,+0000001F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = __setbit(r11, 31<i32>, C)");
        }

        [Test]
        public void Avr32Rw_castu_b()
        {
            Given_Instruction("5C5C");	// castu.b	r12
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r12 = CONVERT(SLICE(r12, byte, 0), byte, word32)",
                "2|L--|NZC = cond(r12)");
        }

        [Test]
        public void Avr32Rw_cbr()
        {
            Given_Instruction("A1D8");	// cbr	r8,+0000001D
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r8 = r8 & 0xFFFFFFFD<u32>",
                "2|L--|Z = false");
        }

        [Test]
        public void Avr32Rw_clz()
        {
            Given_HexString("F00C1200");
            AssertCode(     // clz	r12,r8
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r12 = __clz(r8)",
                "2|L--|Z = r12 == 0<32>",
                "3|L--|C = r12 == 0x20<32>");
        }

        [Test]
        public void Avr32Rw_com()
        {
            Given_Instruction("5CDC");	// com	r12
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r12 = ~r12",
                "2|L--|Z = cond(r12)");
        }

        [Test]
        public void Avr32Rw_cp_w()
        {
            Given_Instruction("103A");	// cp.w	r10,r8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|VNZC = cond(r10 - r8)");
        }


        [Test]
        public void Avr32Rw_cpc1()
        {
            Given_Instruction("5C29");	// cpc	r9
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|VNZC = cond(r9 - C)");
        }

        [Test]
        public void Avr32Rw_eor()
        {
            Given_Instruction("F7E9200C");	// eor	r12,r11,r9
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r12 = r11 ^ r9",
                "2|L--|NZ = cond(r12)");
        }

        [Test]
        public void Avr32Rw_icall()
        {
            Given_Instruction("5D18");	// icall	r8
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r8 (0)");
        }

        [Test]
        public void Avr32Rw_ld_d_disp()
        {
            Given_Instruction("F8EA0000");  // ld.d r10:r11,[r12]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11_r10 = Mem0[r12:word64]");
        }

        [Test]
        public void Avr32Rw_ld_d_predec()
        {
            Given_Instruction("AD08");	// ld.d	r8:r9,--r6
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|r6 = r6 - 8<i32>",
                "2|L--|v3 = Mem0[r6:word64]",
                "3|L--|r9_r8 = v3");
        }

        [Ignore("needs special casing")]
        [Test]
        public void Avr32Rw_ld_d_pc_lr()
        {
            Given_Instruction("AD0E");	// ld.d	pc:lr,--r6
            AssertCode(
                "0|T--|00100000(2): 3 instructions",
                "1|L--|r6 = r6 - 8<i32>",
                "2|L--|lr = Mem0[r6 + 4<i32>:word32]",
                "3|T--|goto Mem0[r6]");
        }

        [Test]

        public void Avr32Rw_ld_sh()
        {
            Given_Instruction("0114");	// ld.sh	r4,r0++
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = Mem0[r0:int16]",
                "2|L--|r0 = r0 + 2<i32>",
                "3|L--|r4 = CONVERT(v3, int16, int32)");
        }

        [Test]
        public void Avr32Rw_ld_ub()
        {
            Given_Instruction("0B94");	// ld.ub	r5[1]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r5 + 1<i32>:byte], byte, word32)");
        }

        [Test]
        public void Avr32Rw_ld_uh_post()
        {
            Given_Instruction("0123");	// ld.uh	r3,r0++
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = Mem0[r0:word16]",
                "2|L--|r0 = r0 + 2<i32>",
                "3|L--|r3 = CONVERT(v3, word16, word32)");
        }

        [Test]
        public void Avr32Rw_ld_w_idx()
        {
            Given_Instruction("F4050333");  // ld.w r3,r10[r5<<3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r10 + r5 * 8<32>:word32]");
        }

        [Test]
        public void Avr32Rw_ld_w_postinc()
        {
            Given_Instruction("1B0B");	// ld.w	r11,sp++
            AssertCode(
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|L--|r11 = v3");
        }

        [Test]
        public void Avr32Rw_lddpc()
        {
            Given_Instruction("4856");	// lddpc	r6,pc[20]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = Mem0[0x00100014<p32>:word32]");
        }

        [Test]
        public void Avr32Rw_lddsp()
        {
            Given_Instruction("474E");	// lddsp	lr,sp[464]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|lr = Mem0[sp + 464<i32>:word32]");
        }

        [Test]
        public void Avr32Rw_ldm()
        {
            Given_Instruction("E3CD8040");	// ldm	sp++,r6,pc
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|r6 = Mem0[sp + 4<i32>:word32]",
                "2|L--|sp = sp + 8<i32>",
                "3|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_lsl2()
        {
            Given_Instruction("A17B");	// lsl	r11,+00000001
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r11 = r11 << 1<i32>",
                "2|L--|NZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_lsl3()
        {
            Given_Instruction("F60C1501");	// lsl	r12,r11,00000001
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r12 = r11 << 1<32>",
                "2|L--|NZC = cond(r12)");
        }

        [Test]
        public void Avr32Rw_lsr2()
        {
            Given_Instruction("B59C");	// lsr	r12,+00000015
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r12 = r12 >>u 21<i32>",
                "2|L--|NZC = cond(r12)");
        }

        [Test]
        public void Avr32Rw_macu_d()
        {
            Given_HexString("F4090744");
            AssertCode(     // macu.d	r4,r10,r9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5_r4 = r5_r4 + r10 *u r9");
        }

        [Test]
        public void Avr32Rw_max()
        {
            Given_HexString("EE080C47");
            AssertCode(     // max	r7,r7,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = max(r7, r8)");
        }

        [Test]
        public void Avr32Rw_mcall()
        {
            Given_Instruction("F0160078");	// mcall	r6[480]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call Mem0[r6 + 480<i32>:word32] (0)");
        }

        [Test]
        public void Avr32Rw_min()
        {
            Given_HexString("F4080D48");
            AssertCode(     // min	r8,r10,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = min(r10, r8)");
        }

        [Test]
        public void Avr32Rw_mov()
        {
            Given_Instruction("3007");	// mov	r7,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = 0<32>");
        }

        [Test]
        public void Avr32Rw_movh()
        {
            Given_Instruction("FC1E1892");	// movh	1892
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = 0x18920000<32>");
        }

        [Test]
        public void Avr32Rw_movne()
        {
            Given_Instruction("F00C1710");  // movne r12,r8
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r12 = r8");
        }

        [Test]
        public void Avr32Rw_mul3()
        {
            Given_Instruction("F60C024C");	// mul	r12,r11,r12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = r11 * r12");
        }

        [Test]
        public void Avr32Rw_muls_d()
        {
            Given_HexString("F6080444");
            AssertCode(     // muls.d	r4,r11,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5_r4 = r11 *s r8");
        }

        [Test]
        public void Avr32Rw_mulu_d()
        {
            Given_HexString("F6080644");
            AssertCode(     // mulu.d	r4,r11,r8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5_r4 = r11 *u r8");
        }

        [Test]
        public void Avr32Rw_mustr()
        {
            Given_Instruction("5D29");	// mustr	r9
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = sr & 0xF<u32>");
        }

        [Test]
        public void Avr32Rw_neg()
        {
            Given_Instruction("5C3B");	// neg	r11
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r11 = -r11",
                "2|L--|VNZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_or()
        {
            Given_Instruction("F5EB101C");	// or	r12,r10,r11<<1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r12 = r10 | r11 << 1<8>",
                "2|L--|NZ = cond(r12)");
        }

        [Test]
        public void Avr32Rw_orl()
        {
            Given_HexString("E8170010");
            AssertCode(     // orl	r7,0010
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = r7 | 0x10<32>",
                "2|L--|NZ = cond(r7)");
        }

        [Test]
        public void Avr32Rw_orh()
        {
            Given_HexString("EA1BFFF0");
            AssertCode(     // orh	r11,FFF0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r11 | 0xFFF00000<32>",
                "2|L--|NZ = cond(r11)");
        }

        [Test]
        public void Avr32Rw_popm()
        {
            Given_Instruction("D822");	// popm	r4-r7,pc
            AssertCode(
                "0|T--|00100000(2): 6 instructions",
                "1|L--|r7 = Mem0[sp + 4<i32>:word32]",
                "2|L--|r6 = Mem0[sp + 8<i32>:word32]",
                "3|L--|r5 = Mem0[sp + 12<i32>:word32]",
                "4|L--|r4 = Mem0[sp + 16<i32>:word32]",
                "5|L--|sp = sp + 0x14<32>",
                "6|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_pushm()
        {
            Given_Instruction("D431");	// pushm	r0-r3,r4-r7,lr
            AssertCode(
                "0|L--|00100000(2): 18 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = r0",
                "3|L--|sp = sp - 4<i32>",
                "4|L--|Mem0[sp:word32] = r1",
                "5|L--|sp = sp - 4<i32>",
                "6|L--|Mem0[sp:word32] = r2",
                "7|L--|sp = sp - 4<i32>",
                "8|L--|Mem0[sp:word32] = r3",
                "9|L--|sp = sp - 4<i32>",
                "10|L--|Mem0[sp:word32] = r4",
                "11|L--|sp = sp - 4<i32>",
                "12|L--|Mem0[sp:word32] = r5",
                "13|L--|sp = sp - 4<i32>",
                "14|L--|Mem0[sp:word32] = r6",
                "15|L--|sp = sp - 4<i32>",
                "16|L--|Mem0[sp:word32] = r7",
                "17|L--|sp = sp - 4<i32>",
                "18|L--|Mem0[sp:word32] = lr");
        }

        [Test]
        public void Avr32Rw_reteq()
        {
            Given_Instruction("5E0B");	// reteq	r11
            AssertCode(
                "0|R--|00100000(2): 6 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100002",
                "2|L--|r12 = r11",
                "3|L--|NZ = cond(r12)",
                "4|L--|V = false",
                "5|L--|C = false",
                "6|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_reteq_sp()
        {
            Given_Instruction("5E0D");	// reteq	sp
            AssertCode(
                "0|R--|00100000(2): 7 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100002",
                "2|L--|r12 = 0<32>",
                "3|L--|N = false",
                "4|L--|Z = true",
                "5|L--|V = false",
                "6|L--|C = false",
                "7|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_reteq_lr()
        {
            Given_Instruction("5E0E");	// reteq	lr
            AssertCode(
                "0|R--|00100000(2): 7 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100002",
                "2|L--|r12 = 0xFFFFFFFF<32>",
                "3|L--|N = true",
                "4|L--|Z = false",
                "5|L--|V = false",
                "6|L--|C = false",
                "7|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_reteq_pc()
        {
            Given_Instruction("5E0F");	// reteq	pc
            AssertCode(
                "0|R--|00100000(2): 7 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100002",
                "2|L--|r12 = 1<32>",
                "3|L--|N = false",
                "4|L--|Z = false",
                "5|L--|V = false",
                "6|L--|C = false",
                "7|R--|return (0,0)");
        }

        [Test]
        public void Avr32Rw_rcall()
        {
            Given_Instruction("F4B40000");	// rcall	002A19FE
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call FFFA0000 (0)");
        }

        [Test]
        public void Avr32Rw_rjmp()
        {
            Given_Instruction("C038");	// rjmp	0000F806
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 00100006");
        }

        [Test]
        public void Avr32Rw_rsub()
        {
            Given_Instruction("1E26");	// rsub	r6,pc
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r6 = 0x00100000<p32> - r6",
                "2|L--|VNZC = cond(r6)");
        }

        [Test]
        public void Avr32Dis_rsub3()
        {
            Given_Instruction("F2091101");  // rsub r9,r9,00000001
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|r9 = 1<32> - r9",
               "2|L--|VNZC = cond(r9)");
        }

        [Test]
        public void Avr32Rw_satsub_w()
        {
            Given_Instruction("F2D00000");	// satsub.w	r0,r9
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r0 = __satsub_w(r0, r9)",
                "2|L--|VNZC = cond(r0)",
                "3|L--|Q = cond(r0)");
        }

        [Test]
        public void Avr32Rw_sbc()
        {
            Given_Instruction("F20B014B");	// sbc	r11,r9,r11
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r9 - r11 - C",
                "2|L--|VNZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_sbr()
        {
            Given_Instruction("BDBB");	// sbr	r11,+0000001D
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r11 = r11 | 0x20000000<u32>",
                "2|L--|Z = false");
        }

        [Test]
        public void Avr32Rw_srcs()
        {
            Given_Instruction("5F3C");	// srcs	r12
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r12 = CONVERT(Test(ULT,C), bool, word32)");
        }

        [Test]
        public void Avr32Rw_srcc()
        {
            Given_Instruction("5F29");	// srcc	r9
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = CONVERT(Test(UGE,C), bool, word32)");
        }

        [Test]
        public void Avr32Rw_st_b_post()
        {
            Given_Instruction("00CD");	// st.b	r0++,sp
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|Mem0[r0:byte] = SLICE(sp, byte, 0)",
                "2|L--|r0 = r0 + 1<i32>");
        }

        [Test]
        public void Avr32Rw_st_d()
        {
            Given_Instruction("EEE9FFDC");	// st.d	r7[-36],r9:r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r7 + -36<i32>:word64] = r9_r8");
        }

        [Test]
        public void Avr32Rw_st_h()
        {
            Given_Instruction("00E0");	// st.h	--r0,r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r0 - 2<i32>",
                "2|L--|Mem0[r0:word16] = SLICE(r0, word16, 0)");
        }

        [Test]
        public void Avr32Rw_st_w_preinc()
        {
            Given_Instruction("1ADA");	// st.w	--sp,r10
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = r10");
        }

        [Test]
        public void Avr32Rw_stcond()
        {
            Given_Instruction("F9760000");	// stcond	r12[0],r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = r12",
                "2|L--|Z = __stcond(v4, r6)");
        }

        [Test]
        public void Avr32Rw_stdsp()
        {
            Given_Instruction("5500");	// stdsp	sp[320],r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[sp + 320<i32>:word32] = r0");
        }

        [Test]
        public void Avr32Rw_stm__predec()
        {
            Given_Instruction("EBCD40FF");	// stm	--sp,r0-r7,lr
            AssertCode(
                "0|L--|00100000(4): 10 instructions",
                "1|L--|Mem0[sp + -4<i32>:word32] = r0",
                "2|L--|Mem0[sp + -8<i32>:word32] = r1",
                "3|L--|Mem0[sp + -12<i32>:word32] = r2",
                "4|L--|Mem0[sp + -16<i32>:word32] = r3",
                "5|L--|Mem0[sp + -20<i32>:word32] = r4",
                "6|L--|Mem0[sp + -24<i32>:word32] = r5",
                "7|L--|Mem0[sp + -28<i32>:word32] = r6",
                "8|L--|Mem0[sp + -32<i32>:word32] = r7",
                "9|L--|Mem0[sp + -36<i32>:word32] = lr",
                "10|L--|sp = sp - 36<i32>");
        }

        [Test]
        public void Avr32Rw_sub3_0()
        {
            Given_Instruction("F8CB0000");	// sub	r11,r12,00000000
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r12",
                "2|L--|VNZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_sub3_neg4()
        {
            Given_Instruction("F8CBFFFC");	// sub	r11,r12,-4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r11 = r12 + 4<32>",
                "2|L--|VNZC = cond(r11)");
        }

        [Test]
        public void Avr32Rw_subcc_imm()
        {
            Given_Instruction("F7BC03EA");  // subcs r12,FFFFFFEA
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|r12 = r12 + 0x16<32>");
        }

        [Test]
        public void Avr32Rw_sum3_imm()
        {
            Given_Instruction("FECCD140");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r12 = 0x00100000<p32> + 0x2EC0<32>",
                "2|L--|VNZC = cond(r12)");
        }

        [Test]
        public void Avr32Rw_subf()
        {
            Given_HexString("F5BA0000");
            AssertCode(     // subfeq	r10,00000000
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r10 = r10",
                "3|L--|VNZC = cond(r10)");
        }

        [Test]
        public void Avr32Rw_tst()
        {
            Given_Instruction("007F");	// tst	pc,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|NZ = cond(0x00100000<p32> & r0)");
        }
    }
}
