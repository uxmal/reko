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
using Reko.Arch.Infineon;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.TriCore
{
    public class TriCoreRewriterTests : RewriterTestBase
    {
        private readonly TriCoreArchitecture arch;
        private readonly Address addr;

        public TriCoreRewriterTests()
        {
            this.arch = new TriCoreArchitecture(CreateServiceContainer(), "tricore", new());
            this.addr = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void TriCoreRw_add()
        {
            Given_HexString("420F");
            AssertCode(     // add	d15,d0
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d15 = d15 + d0",
                "2|L--|V_SV_AV_SAV = cond(d15)");
        }

        [Test]
        public void TriCoreRw_add_a()
        {
            Given_HexString("B0FF");
            AssertCode(     // add.a	a15,#0xFFFFFFFF
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a15 = a15 + 0xFFFFFFFF<32>");
        }

        [Test]
        public void TriCoreRw_addi()
        {
            Given_HexString("1B12F02F");
            AssertCode(     // addi	d2,d2,#0xFF01
                "0|L--|00100000(4): 2 instructions",
                "1|L--|d2 = d2 + 0xFFFFFF01<32>",
                "2|L--|V_SV_AV_SAV = cond(d2)");
        }

        [Test]
        public void TriCoreRw_addih()
        {
            Given_HexString("9B0F78F0");
            AssertCode(     // addih	d15,d15,#0x780
                "0|L--|00100000(4): 2 instructions",
                "1|L--|d15 = d15 + 0x7800000<32>",
                "2|L--|V_SV_AV_SAV = cond(d15)");
        }

        [Test]
        public void TriCoreRw_addih_a()
        {
            Given_HexString("110F00FF");
            AssertCode(     // addih.a	a15,a15,#0xF000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|a15 = a15 + 0xF0000000<32>");
        }

        [Test]
        public void TriCoreRw_addsc_a()
        {
            Given_HexString("10CF");
            AssertCode(     // addsc.a	a15,a12,d15,#0x0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a15 = a12 + d15");
        }

        [Test]
        public void TriCoreRw_and()
        {
            Given_HexString("2620");
            AssertCode(     // and	d0,d2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|d0 = d0 & d2");
        }

        [Test]
        public void TriCoreRw_andn()
        {
            Given_HexString("8F00C801");
            AssertCode(     // andn	d0,d0,#0x80
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = d0 & ~0x80<32>");
        }

        [Test]
        public void TriCoreRw_call()
        {
            Given_HexString("6D00420F");
            AssertCode(     // call	000000BC
                "0|T--|00100000(4): 2 instructions",
                "1|L--|__store_upper_context()",
                "2|T--|call 00101E84 (0)");
        }

        [Test]
        public void TriCoreRw_calli()
        {
            Given_HexString("2D0F0000");
            AssertCode(     // calli	a15
                "0|T--|00100000(4): 2 instructions",
                "1|L--|__store_upper_context()",
                "2|T--|call a15 (0)");
        }

        [Test]
        public void TriCoreRw_cmov()
        {
            Given_HexString("2A21");
            AssertCode(     // cmov	d1,d15,d2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|d1 = d15 != 0<32> ? d2 : d1");
        }

        [Test]
        public void TriCoreRw_debug()
        {
            Given_HexString("00A0");
            AssertCode(     // debug
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__debug()");
        }

        [Test]
        public void TriCoreRw_div()
        {
            Given_HexString("4BF40142");
            AssertCode(     // div	e4,d4,d15
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v2 = d4 / d15",
                "2|L--|v3 = d4 %s d15",
                "3|L--|e4 = SEQ(v3, v2)",
                "4|L--|V_SV = cond(v2)",
                "5|L--|AV = false");
        }

        [Test]
        public void TriCoreRw_extr()
        {
            Given_HexString("3700640E");
            AssertCode(     // extr	d0,d0,#0x1C,#0x4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = SLICE(d0, word4, 28)",
                "2|L--|d0 = CONVERT(v3, word4, int32)");
        }

        [Test]
        public void TriCoreRw_insert()
        {
            Given_HexString("B71F01F2");
            AssertCode(     // insert	d15,d15,#0x1,#0x4,#0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __insert(d15, 1<8>, 4<8>, 1<8>)");
        }

        [Test]
        public void TriCoreRw_isync()
        {
            Given_HexString("0D00C004");
            AssertCode(     // isync
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__isync()");
        }

        [Test]
        public void TriCoreRw_j()
        {
            Given_HexString("1D000A00");
            AssertCode(     // j	00000014
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 00100014");
        }

        [Test]
        public void TriCoreRw_jeq_a()
        {
            Given_HexString("7DFE0D00");
            AssertCode(     // jeq.a	a14,a15,000002BE
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (a14 == a15) branch 0010001A");
        }

        [Test]
        public void TriCoreRw_jge()
        {
            Given_HexString("7F3F0A00");
            AssertCode(     // jge	d15,d3,0000073C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (d15 >= d3) branch 00100014");
        }

        [Test]
        public void TriCoreRw_jge_u()
        {
            Given_HexString("7F230B80");
            AssertCode(     // jge.u	d3,d2,00000602
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (d3 >=u d2) branch 00100016");
        }

        [Test]
        public void TriCoreRw_ji_a11()
        {
            Given_HexString("DC0B");
            AssertCode(     // ji	a11
                "0|R--|00100000(2): 1 instructions",
                "1|R--|return (0,0)");
        }

        [Test]
        public void TriCoreRw_jl()
        {
            Given_HexString("5D006800");
            AssertCode(     // jl	000000EC
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 001000D0 (0)");
        }

        [Test]
        public void TriCoreRw_jne()
        {
            Given_HexString("5FF20E80");
            AssertCode(     // jne	d2,d15,00000B2E
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (d2 != d15) branch 0010001C");
        }

        [Test]
        public void TriCoreRw_jnz()
        {
            Given_HexString("EEFD");
            AssertCode(     // jnz	d15,00000C2E
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (d15 != 0<32>) branch 000FFFFA");
        }

        [Test]
        public void TriCoreRw_jz_a()
        {
            Given_HexString("BCFA");
            AssertCode(     // jz.a	a15,000002BE
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (a15 == 0<32>) branch 00100014");
        }

        [Test]
        public void TriCoreRw_jz()
        {
            Given_HexString("7644");
            AssertCode(     // jz	d4,00001F16
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (d4 == 0<32>) branch 00100008");
        }

        [Test]
        public void TriCoreRw_jz_t()
        {
            Given_HexString("6F010400");
            AssertCode(     // jz.t	d1,#0x0,00100008
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (!__bit<word32,word16>(d1, 0<16>)) branch 00100008");
        }

        [Test]
        public void TriCoreRw_ld_a()
        {
            Given_HexString("C4DF");
            AssertCode(     // ld.a	a15,[a13+]
                "0|L--|00100000(2): 2 instructions",
                "1|L--|a15 = Mem0[a13:word32]",
                "2|L--|a13 = a13 + 4<i32>");
        }

        [Test]
        public void TriCoreRw_ld_b()
        {
            Given_HexString("79FE0000");
            AssertCode(     // ld.b	d14,[a15]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[a15:int8]",
                "2|L--|d14 = CONVERT(v4, int8, int32)");
        }

        [Test]
        public void TriCoreRw_ld_bu()
        {
            Given_HexString("39CF3480");
            AssertCode(     // ld.bu	d15,[a12]564
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[a12 + 564<i32>:byte]",
                "2|L--|d15 = CONVERT(v4, byte, uint32)");
        }

        [Test]
        public void TriCoreRw_ld_d_post()
        {
            Given_HexString("09FE4801");
            AssertCode(     // ld.d	e14,[a15+]8
                "0|L--|00100000(4): 2 instructions",
                "1|L--|e14 = Mem0[a15:word64]",
                "2|L--|a15 = a15 + 8<i32>");
        }

        [Test]
        public void TriCoreRw_ld_h()
        {
            Given_HexString("84FE");
            AssertCode(     // ld.h	d14,[a15+]
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v4 = Mem0[a15:int16]",
                "2|L--|d14 = CONVERT(v4, int16, int32)",
                "3|L--|a15 = a15 + 2<i32>");
        }

        [Test]
        public void TriCoreRw_ld_hu()
        {
            Given_HexString("B9A20C00");
            AssertCode(     // ld.hu	d2,[a10]12
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[a10 + 12<i32>:word16]",
                "2|L--|d2 = CONVERT(v4, word16, uint32)");
        }

        [Test]
        public void TriCoreRw_ld_w()
        {
            Given_HexString("85F17070");
            AssertCode(     // ld.w	d1,[0003C5F0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = Mem0[0x0003C5F0<p32>:word32]");
        }

        [Test]
        public void TriCoreRw_ld_w_preinc()
        {
            Given_HexString("09042CA5");
            AssertCode(     // ld.w\td4,[+a0]-340
                "0|L--|00100000(4): 2 instructions",
                "1|L--|a0 = a0 - 340<i32>",
                "2|L--|d4 = Mem0[a0:word32]");
        }

        [Test]
        public void TriCoreRw_lea()
        {
            Given_HexString("D9AA2000");
            AssertCode(     // lea	a10,[a10]32
                "0|L--|00100000(4): 1 instructions",
                "1|L--|a10 = a10 + 32<i32>");
        }

        [Test]
        public void TriCoreRw_lha()
        {
            Given_HexString("C50F3E10");
            AssertCode(     // lha	a15,[0000007E]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|a15 = 0x1F8000<32>");
        }

        [Test]
        public void TriCoreRw_loop()
        {
            Given_HexString("FCFD");
            AssertCode(     // loop	d15,000002AE
                "0|T--|00100000(2): 3 instructions",
                "1|L--|v2 = d15",
                "2|L--|d15 = d15 - 1<32>",
                "3|T--|if (v2 != 0<32>) branch 000FFFFA");
        }

        [Test]
        public void TriCoreRw_madd_u()
        {
            Given_HexString("13025029");
            AssertCode(     // madd.u	d2,d6,d2,#0x9500
                "0|L--|00100000(4): 2 instructions",
                "1|L--|d2 = d9 + d2 *u 0xFF00<u32>",
                "2|L--|V_SV_AV_SAV = cond(d2)");
        }

        [Test]
        public void TriCoreRw_mfcr()
        {
            Given_HexString("4D40E00F");
            AssertCode(     // mfcr	d0,PSW
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = __mfcr(PSW)");
        }

        [Test]
        public void TriCoreRw_min_u()
        {
            Given_HexString("0B249041");
            AssertCode(     // min.u	d4,d4,d2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = min<uint32>(d4, d2)");
        }

        [Test]
        public void TriCoreRw_mov()
        {
            Given_HexString("3B00006F");
            AssertCode(     // mov	d6,#0x8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d6 = 0xFFFFF000<32>");
        }

        [Test]
        public void TriCoreRw_mov_a()
        {
            Given_HexString("6003");
            AssertCode(     // mov.a	a3,d0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a3 = d0");
        }

        [Test]
        public void TriCoreRw_mov_aa()
        {
            Given_HexString("4034");
            AssertCode(     // mov.aa	a4,a3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a4 = a3");
        }

        [Test]
        public void TriCoreRw_mov_d()
        {
            Given_HexString("8031");
            AssertCode(     // mov.d	d1,a3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|d1 = a3");
        }

        [Test]
        public void TriCoreRw_mov_sequence()
        {
            Given_HexString("0B651088");
            AssertCode(     // mov	e8,d5,d6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|e8 = SEQ(d5, d6)");
        }

        [Test]
        public void TriCoreRw_mov_u()
        {
            Given_HexString("BB00001F");
            AssertCode(     // mov.u	d1,#0x900D
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = 0xF000<32>");
        }

        [Test]
        public void TriCoreRw_movh_a()
        {
            Given_HexString("911000AB");
            AssertCode(     // movh.a	a10,#0xB001
                "0|L--|00100000(4): 1 instructions",
                "1|L--|a10 = 0xB0010000<32>");
        }

        [Test]
        public void TriCoreRw_mtcr()
        {
            Given_HexString("CD80E20F");
            AssertCode(     // mtcr	ISP,d0
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__mtcr(ISP, d0)");
        }

        [Test]
        public void TriCoreRw_mul()
        {
            Given_HexString("E24F");
            AssertCode(     // mul	d15,d4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d15 = d15 *s d4",
                "2|L--|V_SV_AV_SAV = cond(d15)");
        }

        [Test]
        public void TriCoreRw_ne()
        {
            Given_HexString("8BFF3F22");
            AssertCode(     // ne	d2,d15,#0x1FF
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = d15 != 0xFFFFFFFF<32>",
                "2|L--|d2 = CONVERT(v2, bool, word32)");
        }

        [Test]
        public void TriCoreRw_or()
        {
            Given_HexString("8F004F01");
            AssertCode(     // or	d0,d0,#0xF0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = d0 | 0xF0<32>");
        }

        [Test]
        public void TriCoreRw_or_eq()
        {
            Given_HexString("8B0FE044");
            AssertCode(     // or.eq	d4,d15,#0x0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = __bit<word32,int32>(d4, 0<i32>)",
                "2|L--|d4 = __write_bit<word32,int32>(d4, 0<i32>, v2 | d15 == 0<16>)");
        }

        [Test]
        public void TriCoreRw_orn_t()
        {
            Given_HexString("07042CA3");
            AssertCode(     // orn.t	d10,d4,#0xC,d0,#0x6
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = __bit<word32,int32>(d4, 12<i32>) | ~__bit<word32,int32>(d0, 6<i32>)",
                "2|L--|d10 = CONVERT(v4, bool, uint32)");
        }

        [Test]
        public void TriCoreRw_ret()
        {
            Given_HexString("0090");
            AssertCode(     // ret
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__load_upper_context()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void TriCoreRw_seln()
        {
            Given_HexString("2B2F50F3");
            AssertCode(     // seln	d15,d3,d15,d2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = d3 != 0<32> ? d2 : d15");
        }

        [Test]
        public void TriCoreRw_sh()
        {
            Given_HexString("06A2");
            AssertCode(     // sh	d2,#0xFFFFFFFA
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d2 = d2 >>u 6<i32>",
                "2|L--|C_V_SV_AV_SAV = cond(d2)");
        }

        [Test]
        public void TriCoreRw_sha()
        {
            Given_HexString("86E4");
            AssertCode(     // sha	d4,#0xFFFFFFFE
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d4 = d4 >> 2<i32>",
                "2|L--|C_V_SV_AV_SAV = cond(d4)");
        }

        [Test]
        public void TriCoreRw_st_a()
        {
            Given_HexString("A5F07070");
            AssertCode(     // st.a	[0003C5F0],a0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[0x0003C5F0<p32>:word32] = a0");
        }

        [Test]
        public void TriCoreRw_st_b_post()
        {
            Given_HexString("89FE4801");
            AssertCode(     // st.b	[a15+]8,d14
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v3 = SLICE(d14, byte, 0)",
                "2|L--|Mem0[a15:byte] = v3",
                "3|L--|a15 = a15 + 8<i32>");
        }

        [Test]
        public void TriCoreRw_st_h_post()
        {
            Given_HexString("A4FF");
            AssertCode(     // st.h	[a15+],d15
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v3 = SLICE(d15, word16, 0)",
                "2|L--|Mem0[a15:word16] = v3",
                "3|L--|a15 = a15 + 2<i32>");
        }


        [Test]
        public void TriCoreRw_st_w()
        {
            Given_HexString("7422");
            AssertCode(     // st.w	[a2],d2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[a2:word32] = d2");
        }

        [Test]
        public void TriCoreRw_stucx()
        {
            Given_HexString("15090224");
            AssertCode(     // stucx	[00000082]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_upper_context()");
        }

        [Test]
        public void TriCoreRw_sub()
        {
            Given_HexString("A202");
            AssertCode(     // sub	d2,d0
                "0|L--|00100000(2): 2 instructions",
                "1|L--|d2 = d2 - d0",
                "2|L--|V_SV_AV_SAV = cond(d2)");
        }

        [Test]
        public void TriCoreRw_sub_a()
        {
            Given_HexString("2008");
            AssertCode(     // sub.a	a8,#0x8
                "0|L--|00100000(2): 1 instructions",
                "1|L--|a8 = a8 - 8<32>");
        }

        [Test]
        public void TriCoreRw_xor()
        {
            Given_HexString("8F238031");
            AssertCode(     // xor	d3,d3,#0x2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d3 = d3 ^ 2<32>");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues
        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues
        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues





    }
}
