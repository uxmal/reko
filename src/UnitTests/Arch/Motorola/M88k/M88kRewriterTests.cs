#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.Motorola;
using Reko.Core;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Motorola.M88k;

[TestFixture]
public class M88kRewriterTests : RewriterTestBase
{
    private readonly M88kArchitecture arch;
    private readonly Address addr;

    public M88kRewriterTests()
    {
        var sc = new ServiceContainer();
        this.arch = new M88kArchitecture(sc, "m88k", []);
        this.addr = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    [Test]
    public void M88kRw_add()
    {
        Given_HexString("F7897014");
        AssertCode(   //add\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 + r20");
    }

    [Test]
    public void M88kRw_add_co()
    {
        Given_HexString("F7897114");
        AssertCode( // add.co\tr28,r9,r20
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r28 = r9 + r20",
            "2|L--|C = cond(r28)");
    }

    [Test]
    public void M88kRw_add_ci()
    {
        Given_HexString("F7897214");
        AssertCode( // add.ci\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 + r20 + C");
    }

    [Test]
    public void M88kRw_add_cio()
    {
        Given_HexString("F7897314");
        AssertCode( //add.cio\tr28,r9,r20
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r28 = r9 + r20 + C",
            "2|L--|C = cond(r28)");
    }

    [Test]
    public void M88kRw_add_imm()
    {
        Given_HexString("73C5A1E5");
        AssertCode(   //add\tr30,r5,0xA1E5
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r30 = r5 + 0xA1E5<32>");
    }

    [Test]
    public void M88kRw_addu()
    {
        Given_HexString("F5C7601F");
        AssertCode(   //addu\tr14,r7,r31
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 + r31");
    }

    [Test]
    public void M88kRw_addu_co()
    {
        Given_HexString("F5C7611F");
        AssertCode( // addu.co\tr14,r7,r31
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r14 = r7 + r31",
            "2|L--|C = cond(r14)");
    }

    [Test]
    public void M88kRw_addu_ci()
    {
        Given_HexString("F5C7621F");
        AssertCode( // addu.ci\tr14,r7,r31
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 + r31 + C");
    }

    [Test]
    public void M88kRw_addu_cio()
    {
        Given_HexString("F5C7631F");
        AssertCode( // addu.cio\tr14,r7,r31
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r14 = r7 + r31 + C",
            "2|L--|C = cond(r14)");
    }

    [Test]
    public void M88kRw_addu_imm()
    {
        Given_HexString("61831155");
        AssertCode(   //addu\tr12,r3,0x1155
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r12 = r3 + 0x1155<32>");
    }

    [Test]
    public void M88kRw_and()
    {
        Given_HexString("F5C7402F");
        AssertCode(   //and\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 & r15");
    }

    [Test]
    public void M88kRw_and_c()
    {
        Given_HexString("F5C7442F");
        AssertCode(   //and.c\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 & ~r15");
    }

    [Test]
    public void M88kRw_and_imm()
    {
        Given_HexString("418B2CBE");
        AssertCode(   //and\tr12,r11,0x2CBE
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r12 = r11 & 0xFFFF2CBE<32>");
    }

    [Test]
    public void M88kRw_and_u_imm()
    {
        Given_HexString("4451D31D");
        AssertCode(   //and.u\tr2,r17,0xD31D
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r17 & 0xD31DFFFF<32>");
    }

    [Test]
    public void M88kRw_bb0()
    {
        Given_HexString("D0DCBD48");
        AssertCode(   //bb0\t0xA,r28,000EF520
            "0|T--|00100000(4): 1 instructions",
            "1|T--|if (!__bit<word32,byte>(r28, 0xA<8>)) branch 000EF520");
    }

    [Test]
    public void M88kRw_bb0_n()
    {
        Given_HexString("D486CF53 ");
        AssertCode(   //bb0.n\t0x1A,r6,000F3D4C
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|if (!__bit<word32,byte>(r6, 0x1A<8>)) branch 000F3D4C");
    }

    [Test]
    public void M88kRw_bb1()
    {
        Given_HexString("D9E8E497");
        AssertCode(   //bb1\t4,r8,000F925C
            "0|T--|00100000(4): 1 instructions",
            "1|T--|if (__bit<word32,byte>(r8, 4<8>)) branch 000F925C");
    }

    [Test]
    public void M88kRw_bb1_n()
    {
        Given_HexString("DEE8E497");
        AssertCode(   //bb1.n\t4,r8,000F925C
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|if (__bit<word32,byte>(r8, 4<8>)) branch 000F925C");
    }

    [Test]
    public void M88kRw_bcnd_lt0()
    {
        Given_HexString("E98BB063");
        AssertCode(   //bcnd\tlt0,r11,000EC18C
            "0|T--|00100000(4): 1 instructions",
            "1|T--|if (r11 < 0<32>) branch 000EC18C");
    }

    [Test]
    public void M88kRw_bcnd_unk()
    {
        Given_HexString("EA2BB063");
        AssertCode( // bcnd\t0x11,r11,000EC18C
            "0|T--|00100000(4): 1 instructions",
            "1|T--|if (__bcnd(r11)) branch 000EC18C");
    }

    [Test]
    public void M88kRw_bcnd_n()
    {
        Given_HexString("EE9B871F");
        AssertCode(   //bcnd.n\t0x14,r27,000E1C7C
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|if (__bcnd(r27)) branch 000E1C7C");
    }

    [Test]
    public void M88kRw_br()
    {
        Given_HexString("C0890D8D");
        AssertCode(   //br\t02343634
            "0|T--|00100000(4): 1 instructions",
            "1|T--|goto 02343634");
    }

    [Test]
    public void M88kRw_br_n()
    {
        Given_HexString("C7F8FB11");
        AssertCode(   //br.n\tFFF3EC44
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|goto FFF3EC44");
    }

    [Test]
    public void M88kRw_bsr()
    {
        Given_HexString("C844372B");
        AssertCode(   //bsr\t0120DCAC
            "0|T--|00100000(4): 1 instructions",
            "1|T--|call 0120DCAC (0)");
    }

    [Test]
    public void M88kRw_bsr_n()
    {
        Given_HexString("CF83E9C1");
        AssertCode(   //bsr.n\tFE1FA704
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|call FE1FA704 (0)");
    }

    [Test]
    public void M88kRw_clr()
    {
        Given_HexString("F789801D");
        AssertCode(   //clr\tr28,r9,r29
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v6 = SLICE(r29, word5, 0)",
            "2|L--|v5 = SLICE(r29, word5, 5)",
            "3|L--|r28 = __clr(r9, v5, v6)");
    }

    [Test]
    public void M88kRw_clr_imm()
    {
        Given_HexString("F11D812F");
        AssertCode(   //clr\tr0,r29,8,0xF
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = SEQ(SLICE(r29, word9, 23), 0<8>, SLICE(r29, word15, 0))");
    }

    [Test]
    public void M88kRw_cmp()
    {
        Given_HexString("F7897C14");
        AssertCode(   //cmp\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = cond(r9 - r20)");
    }

    [Test]
    public void M88kRw_cmp_imm()
    {
        Given_HexString("7DA7CA08");
        AssertCode(   //cmp\tr13,r7,0xCA08
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r13 = cond(r7 - 0xCA08<32>)");
    }

    [Test]
    public void M88kRw_div()
    {
        Given_HexString("F7897814");
        AssertCode(   //div\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 / r20");
    }

    [Test]
    public void M88kRw_div_imm()
    {
        Given_HexString("7A0A2C60");
        AssertCode(   //div\tr16,r10,0x2C60
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = r10 / 0x2C60<32>");
    }

    [Test]
    public void M88kRw_divu()
    {
        Given_HexString("F7896814");
        AssertCode(   //divu\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 /u r20");
    }

    [Test]
    public void M88kRw_divu_imm()
    {
        Given_HexString("69534416");
        AssertCode(   //divu\tr10,r19,0x4416
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r10 = r19 /u 0x4416<32>");
    }

    [Test]
    public void M88kRw_ext()
    {
        Given_HexString("F389901D");
        AssertCode(   //ext\tr28,r9,0x1C,0x1D
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = __ext(r9, 0x1C<8>, 0x1D<8>)");
    }

    [Test]
    public void M88kRw_ext_imm()
    {
        Given_HexString("F4D59078");
        AssertCode(   //ext\tr6,r21,r24
             "0|L--|00100000(4): 3 instructions",
            "1|L--|v6 = SLICE(r24, word5, 0)",
            "2|L--|v5 = SLICE(r24, word5, 5)",
            "3|L--|r6 = __ext(r21, v5, v6)");
    }

    [Test]
    public void M88kRw_extu()
    {
        Given_HexString("F6F49A18");
        AssertCode(   //extu\tr23,r20,r24
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v6 = SLICE(r24, word5, 0)",
            "2|L--|v5 = SLICE(r24, word5, 5)",
            "3|L--|r23 = __extu(r20, v5, v6)");
    }

    [Test]
    public void M88kRw_extu_imm()
    {
        Given_HexString("F389981D");
        AssertCode(   //extu\tr28,r9,0x1C,0x1D
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = __extu(r9, 0x1C<8>, 0x1D<8>)");
    }

    [Test]
    public void M88kRw_fadd_sss()
    {
        Given_HexString("86BD2806");
        AssertCode(   //fadd.sss\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21 = r29 + r6");
    }

    [Test]
    public void M88kRw_fadd_dss()
    {
        Given_HexString("86BD2826");
        AssertCode( // fadd.dss\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21_r22 = CONVERT(r29 + r6, real32, real64)");
    }

    [Test]
    public void M88kRw_fadd_ssd()
    {
        Given_HexString("86BD2886");
        AssertCode( // fadd.ssd\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21 = CONVERT(CONVERT(r29, real32, real64) + r6_r7, real64, real32)");
    }

    [Test]
    public void M88kRw_fadd_dsd()
    {
        Given_HexString("86BD28A6");
        AssertCode( // fadd.dsd\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21_r22 = CONVERT(r29, real32, real64) + r6_r7");
    }

    [Test]
    public void M88kRw_fadd_sds()
    {
        Given_HexString("86BD2A06");
        AssertCode( // fadd.sds\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21 = CONVERT(r29_r30 + CONVERT(r6, real32, real64), real64, real32)");
    }

    [Test]
    public void M88kRw_fadd_dds()
    {
        Given_HexString("86BD2A26");
        AssertCode( // fadd.dds\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21_r22 = r29_r30 + CONVERT(r6, real32, real64)");
    }

    [Test]
    public void M88kRw_fadd_sdd()
    {
        Given_HexString("86BD2A86");
        AssertCode( // fadd.dds\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21 = CONVERT(r29_r30 + r6_r7, real64, real32)");
    }

    [Test]
    public void M88kRw_fadd_ddd()
    {
        Given_HexString("86BD2AA6");
        AssertCode( // fadd.ddd\tr21,r29,r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r21_r22 = r29_r30 + r6_r7");
    }

    [Test]
    public void M88kRw_fdiv_sss()
    {
        Given_HexString("85317001");
        AssertCode(   //fdiv.sss\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = r17 / r1");
    }

    [Test]
    public void M88kRw_fdiv_dss()
    {
        Given_HexString("85317021");
        AssertCode( // fdiv.dss\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9_r10 = CONVERT(r17 / r1, real32, real64)");
    }

    [Test]
    public void M88kRw_fdiv_ssd()
    {
        Given_HexString("85317081");
        AssertCode( // fdiv.ssd\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = CONVERT(CONVERT(r17, real32, real64) / r1_r2, real64, real32)");
    }

    [Test]
    public void M88kRw_fdiv_dsd2()
    {
        Given_HexString("853170A1");
        AssertCode( // fdiv.dsd\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9_r10 = CONVERT(r17, real32, real64) / r1_r2");
    }

    [Test]
    public void M88kRw_fdiv_sds()
    {
        Given_HexString("85317201");
        AssertCode( // fdiv.sds\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = CONVERT(r17_r18 / CONVERT(r1, real32, real64), real64, real32)");
    }

    [Test]
    public void M88kRw_fdiv_dds()
    {
        Given_HexString("85317221");
        AssertCode( // fdiv.dds\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9_r10 = r17_r18 / CONVERT(r1, real32, real64)");
    }

    [Test]
    public void M88kRw_fdiv_sdd()
    {
        Given_HexString("85317281");
        AssertCode( // fdiv.dds\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = CONVERT(r17_r18 / r1_r2, real64, real32)");
    }

    [Test]
    public void M88kRw_fdiv_ddd()
    {
        Given_HexString("853172A1");
        AssertCode( // fdiv.ddd\tr9,r17,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9_r10 = r17_r18 / r1_r2");
    }

    [Test]
    public void M88kRw_fcmp_ss()
    {
        Given_HexString("873E380B");
        AssertCode(   //fcmp.sss\tr25,r30,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r25 = cond(r30 - r11)");
    }

    [Test]
    public void M88kRw_fcmp_sd()
    {
        Given_HexString("873E388B");
        AssertCode( // fcmp.ssd\tr25,r30,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r25 = cond(CONVERT(r30, real32, real64) - r11_r12)");
    }

    [Test]
    public void M88kRw_fcmp_ds()
    {
        Given_HexString("873E3A0B");
        AssertCode( // fcmp.sds\tr25,r30,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r25 = cond(r30_r31 - CONVERT(r11, real32, real64))");
    }

    [Test]
    public void M88kRw_fcmp_dd()
    {
        Given_HexString("873E3A8B");
        AssertCode( // fcmp.dds\tr25,r30,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r25 = cond(r30_r31 - r11_r12)");
    }

    [Test]
    public void M88kRw_ff0()
    {
        Given_HexString("F530EC19");
        AssertCode(   //ff0\tr9,r25
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = __find_first_zero<word32>(r25)");
        //AssertCode("ff0\rr9,r9",  "F536EC49");
    }

    [Test]
    public void M88kRw_ff1()
    {
        Given_HexString("F530E819");
        AssertCode(   //ff1\tr9,r25
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r9 = __find_first_one<word32>(r25)");
    }

    [Test]
    public void M88kRw_fldcr()
    {
        Given_HexString("81704820");
        AssertCode(   //fldcr\tr11,FPHS1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r11 = __read_cr(FPHS1)");
    }

    [Test]
    public void M88kRw_flt_s()
    {
        Given_HexString("84722005");
        AssertCode(   //flt.s\tr3,r5
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = CONVERT(r5, int32, real32)");
    }

    [Test]
    public void M88kRw_flt_d()
    {
        Given_HexString("84722025");
        AssertCode( // flt.d\tr3,r5
          "0|L--|00100000(4): 1 instructions",
          "1|L--|r3_r4 = CONVERT(r5, int32, real64)");
    }

    [Test]
    public void M88kRw_fstcr()
    {
        Given_HexString("82168812");
        AssertCode(   //fstcr\tFPECR,r22
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__write_cr(FPECR, r22)");
    }

    [Test]
    public void M88kRw_fsub_sss()
    {
        Given_HexString("8600300A");
        AssertCode(   //fsub.sss\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = 0.0F - r10");
    }

    [Test]
    public void M88kRw_fsub_dss()
    {
        Given_HexString("8600302A");
        AssertCode( // fsub.dss\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16_r17 = CONVERT(0.0F - r10, real32, real64)");
    }

    [Test]
    public void M88kRw_fsub_ssd()
    {
        Given_HexString("8600308A");
        AssertCode( // fsub.ssd\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = CONVERT(CONVERT(0.0F, real32, real64) - r10_r11, real64, real32)");
    }

    [Test]
    public void M88kRw_fsub_dsd()
    {
        Given_HexString("860030AA");
        AssertCode( // fsub.dsd\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16_r17 = CONVERT(0.0F, real32, real64) - r10_r11");
    }

    [Test]
    public void M88kRw_fsub_sds()
    {
        Given_HexString("8600320A");
        AssertCode( // fsub.sds\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = CONVERT(0.0 - CONVERT(r10, real32, real64), real64, real32)");
    }

    [Test]
    public void M88kRw_fsub_dds()
    {
        Given_HexString("8600322A");
        AssertCode( // fsub.dds\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16_r17 = 0.0 - CONVERT(r10, real32, real64)");
    }

    [Test]
    public void M88kRw_fsub_dds2()
    {
        Given_HexString("8600328A");
        AssertCode( // fsub.dds\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = CONVERT(0.0 - r10_r11, real64, real32)");
    }

    [Test]
    public void M88kRw_fsub_ddd()
    {
        Given_HexString("860032AA");
        AssertCode( // fsub.ddd\tr16,r0,r10
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16_r17 = 0.0 - r10_r11");
    }

    [Test]
    public void M88kRw_fxcr()
    {
        Given_HexString("82C2C883");
        AssertCode(   //fxcr\tr22,r2,FPLS2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r22 = __exchange_cr(r2, FPLS2)");
    }

    [Test]
    public void M88kRw_int_s()
    {
        Given_HexString("87A04810");
        AssertCode(   //int.s\tr29,r16
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r29 = CONVERT(__int_32(r16), real32, int32)");
    }

    [Test]
    public void M88kRw_int_d()
    {
        Given_HexString("87A04890");
        AssertCode( // int.d\tr29,r16
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r29 = CONVERT(__int_64(r16_r17), real64, int32)");
    }

    [Test]
    public void M88kRw_jmp()
    {
        Given_HexString("F60EC318");
        AssertCode(   //jmp\tr24
            "0|T--|00100000(4): 1 instructions",
            "1|T--|goto r24");
    }

    [Test]
    public void M88kRw_jmp_n()
    {
        Given_HexString("F4F9C64F");
        AssertCode(   //jmp.n\tr15
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|goto r15");
    }

    [Test]
    public void M88kRw_jsr()
    {
        Given_HexString("F772CB19");
        AssertCode(   //jsr\tr25
            "0|T--|00100000(4): 1 instructions",
            "1|T--|call r25 (0)");
    }

    [Test]
    public void M88kRw_jsr_n()
    {
        Given_HexString("F536CC49");
        AssertCode(   //jsr.n\tr9
            "0|TD-|00100000(4): 1 instructions",
            "1|TD-|call r9 (0)");
    }

    [Test]
    public void M88kRw_ld()
    {
        Given_HexString("16D97FFF");
        AssertCode(   //ld\tr22,r25,0x7FFF
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r22 = Mem0[r25 + 0x7FFF<32>:word32]");
    }

    [Test]
    public void M88kRw_ld_idx()
    {
        Given_HexString("F5F0149C");
        AssertCode(   //ld\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = Mem0[r16 + r28:word32]");
    }

    [Test]
    public void M88kRw_ld_usr_idx()
    {
        Given_HexString("F5F0159C");
        AssertCode( // ld.usr\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = __load_from_userspace<word32>(r16 + r28)");
    }

    [Test]
    public void M88kRw_ld_idx_scaled()
    {
        Given_HexString("F5F0169C");
        AssertCode( // ld\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = Mem0[r16 + r28 * 4<32>:word32]");
    }

    [Test]
    public void M88kRw_ld_usr_idx_scaled()
    {
        Given_HexString("F5F0179C");
        AssertCode( // ld.usr\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = __load_from_userspace<word32>(r16 + r28 * 4<32>)");
    }

    [Test]
    public void M88kRw_ld_b()
    {
        Given_HexString("1C3439A6");
        AssertCode(   //ld.b\tr1,r20,0x39A6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r1 = CONVERT(Mem0[r20 + 0x39A6<32>:int8], int8, word32)");
    }

    [Test]
    public void M88kRw_ld_h_idx()
    {
        Given_HexString("F5F0189C");
        AssertCode(   //ld.h\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28:int16], int16, word32)");
    }

    [Test]
    public void M88kRw_ld_h_usr_idx()
    {
        Given_HexString("F5F0199C");
        AssertCode( // ld.h.usr\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<int16>(r16 + r28), int16, word32)");
    }

    [Test]
    public void M88kRw_ld_h_idx_scaled()
    {
        Given_HexString("F5F01A9C");
        AssertCode( // ld.h\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28 * 2<32>:int16], int16, word32)");
    }

    [Test]
    public void M88kRw_ld_h_idx_usr()
    {
        Given_HexString("F5F01B9C");
        AssertCode( // ld.h.usr\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<int16>(r16 + r28 * 2<32>), int16, word32)");
    }

    [Test]
    public void M88kRw_ld_hu_idx()
    {
        Given_HexString("F5F0089C");
        AssertCode(   //ld.hu\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28:word16], word16, word32)");
    }

    [Test]
    public void M88kRw_ld_hu_usr_idx()
    {
        Given_HexString("F5F0099C");
        AssertCode( // ld.hu.usr\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<word16>(r16 + r28), word16, word32)");
    }

    [Test]
    public void M88kRw_ld_hu_idx_scaled()
    {
        Given_HexString("F5F00A9C");
        AssertCode( // ld.hu\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28 * 2<32>:word16], word16, word32)");
    }

    [Test]
    public void M88kRw_ld_hu_usr_idx_scaled()
    {
        Given_HexString("F5F00B9C");
        AssertCode( // ld.hu.usr\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<word16>(r16 + r28 * 2<32>), word16, word32)");
    }

    [Test]
    public void M88kRw_ld_bu_idx()
    {
        Given_HexString("F5F00C9C");
        AssertCode(   //ld.bu\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28:byte], byte, word32)");
    }

    [Test]
    public void M88kRw_ld_bu_usr_idx()
    {
        Given_HexString("F5F00D9C");
        AssertCode( // ld.bu.usr\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<byte>(r16 + r28), byte, word32)");
    }

    [Test]
    public void M88kRw_ld_bu_idx_scaled()
    {
        Given_HexString("F5F00E9C");
        AssertCode( // ld.bu\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(Mem0[r16 + r28:byte], byte, word32)");
    }

    [Test]
    public void M88kRw_ld_bu_usr_idx_scaled()
    {
        Given_HexString("F5F00F9C");
        AssertCode( // ld.bu.usr\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15 = CONVERT(__load_from_userspace<byte>(r16 + r28), byte, word32)");
    }

    [Test]
    public void M88kRw_ld_bu()
    {
        Given_HexString("0C60419E");
        AssertCode(   //ld.bu\tr3,r0,0x419E
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = CONVERT(Mem0[0x419E<32>:byte], byte, word32)");
    }

    [Test]
    public void M88kRw_ld_d()
    {
        Given_HexString("10095DD6");
        AssertCode(   //ld.d\tr0,r9,0x5DD6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r0_r1 = Mem0[r9 + 0x5DD6<32>:word64]");
    }

    [Test]
    public void M88kRw_ld_d_idx_unscaled()
    {
        Given_HexString("F5F0109C");
        AssertCode(   //ld.d\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15_r16 = Mem0[r16 + r28:word64]");
    }

    [Test]
    public void M88kRw_ld_d_usr_idx_unscaled()
    {
        Given_HexString("F5F0119C");
        AssertCode( // ld.d.usr\tr15,r16,r28
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15_r16 = __load_from_userspace<word64>(r16 + r28)");
    }

    [Test]
    public void M88kRw_ld_d_idx_scaled()
    {
        Given_HexString("F5F0129C");
        AssertCode( // ld.d\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15_r16 = Mem0[r16 + r28 * 8<32>:word64]");
    }

    [Test]
    public void M88kRw_ld_d_usr_idx_scaled()
    {
        Given_HexString("F5F0139C");
        AssertCode( // ld.d.usr\tr15,r16[r28]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r15_r16 = __load_from_userspace<word64>(r16 + r28 * 8<32>)");
    }

    [Test]
    public void M88kRw_ld_h()
    {
        Given_HexString("1800C309");
        AssertCode(   //ld.h\tr0,r0,0xC309
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r0 = CONVERT(Mem0[0xC309<32>:int16], int16, word32)");
    }

    [Test]
    public void M88kRw_ld_hu()
    {
        Given_HexString("080CFBE8");
        AssertCode(   //ld.hu\tr0,r12,0xFBE8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r0 = CONVERT(Mem0[r12 + 0xFBE8<32>:word16], word16, word32)");
    }

    [Test]
    public void M88kRw_lda()
    {
        Given_HexString("37883282");
        AssertCode(   //lda\tr28,r8,0x3282
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r8 + 0x3282<32>");
    }

    [Test]
    public void M88kRw_lda_idx()
    {
        Given_HexString("F7173401");
        AssertCode(   //lda\tr24,r23,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1");
    }

    [Test]
    public void M88kRw_lda_b()
    {
        Given_HexString("3EDA3DB6");
        AssertCode(   //lda.b\tr22,r26,0x3DB6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r22 = r26 + 0x3DB6<32>");
    }

    [Test]
    public void M88kRw_lda_b_idx_unscaled()
    {
        Given_HexString("F7173C01");
        AssertCode(   //lda.b\tr24,r23,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1");
    }

    [Test]
    public void M88kRw_lda_b_idx_scaled()
    {
        Given_HexString("F7173E01");
        AssertCode( // lda.b\tr24,r23[r1]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1");
    }

    [Test]
    public void M88kRw_lda_d()
    {
        Given_HexString("302EE134");
        AssertCode(   //lda.d\tr1,r14,0xE134
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r1 = r14 + 0xE134<32>");
    }

    [Test]
    public void M88kRw_lda_d_idx()
    {
        Given_HexString("F7173001");
        AssertCode(   //lda.d\tr24,r23,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1");
    }

    [Test]
    public void M88kRw_lda_d_idx_scaled()
    {
        Given_HexString("F7173201");
        AssertCode( // lda.d\tr24,r23[r1]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1 * 8<32>");
    }

    [Test]
    public void M88kRw_lda_h()
    {
        Given_HexString("384A0E25");
        AssertCode(   //lda.h\tr2,r10,0xE25
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r10 + 0xE25<32>");
    }

    [Test]
    public void M88kRw_lda_h_idx()
    {
        Given_HexString("F7173801");
        AssertCode(   //lda.h\tr24,r23,r1
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1");
    }

    [Test]
    public void M88kRw_lda_h_idx_scaled()
    {
        Given_HexString("F7173A01");
        AssertCode( // lda.h\tr24,r23[r1]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r24 = r23 + r1 * 2<32>");
    }

    [Test]
    public void M88kRw_ldcr()
    {
        Given_HexString("806D4138");
        AssertCode(   //ldcr\tr3,DMD0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = __read_cr(DMD0)");
    }

    [Test]
    public void M88kRw_mak()
    {
        Given_HexString("F789A01D");
        AssertCode(   //mak\tr28,r9,r29
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 | r29 << 0x10<32>");
    }

    [Test]
    public void M88kRw_mak_imm()
    {
        Given_HexString("F39BA227");
        AssertCode(   //mak\tr28,r27,0x1C,7
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r27 | 0x1C<8> << 0x10<32>");
    }

    [Test]
    public void M88kRw_mask()
    {
        Given_HexString("482200F9");
        AssertCode(   //mask\tr1,r2,0xF9
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r1 = r2 & 0xF9<32>");
    }

    [Test]
    public void M88kRw_mask_u()
    {
        Given_HexString("4C41A6BD");
        AssertCode(   //mask.u\tr2,r1,0xA6BD
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r1 & 0xA6BD0000<32>");
    }

    [Test]
    public void M88kRw_mul()
    {
        Given_HexString("F7896C14");
        AssertCode(   //mul\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 * r20");
    }

    [Test]
    public void M88kRw_mul_imm()
    {
        Given_HexString("6D01BAD9");
        AssertCode(   //mul\tr8,r1,0xBAD9
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = r1 * 0xBAD9<32>");
    }

    [Test]
    public void M88kRw_nint_s()
    {
        Given_HexString("87795008");
        AssertCode(   //nint.s\tr27,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r27 = CONVERT(roundf(r8), real32, int32)");
    }

    [Test]
    public void M88kRw_nint_d()
    {
        Given_HexString("87795088");
        AssertCode( // nint.d\tr27,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r27 = CONVERT(round(r8_r9), real64, int32)");
    }

    [Test]
    public void M88kRw_or()
    {
        Given_HexString("F5C7582F");
        AssertCode(   //or\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 | r15");
    }

    [Test]
    public void M88kRw_or_c()
    {
        Given_HexString("F5C75C2F");
        AssertCode(   //or.c\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 | ~r15");
    }

    [Test]
    public void M88kRw_or_imm_r0()
    {
        Given_HexString("581ED55B");
        AssertCode(   //or\tr0,r30,0xD55B
            "0|L--|00100000(4): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void M88kRw_or_imm()
    {
        Given_HexString("589ED55B");
        AssertCode(   //or\tr0,r30,0xD55B
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r4 = r30 | 0xD55B<32>");
    }

    [Test]
    public void M88kRw_or_u_imm()
    {
        Given_HexString("5CE17589");
        AssertCode(   //or.u\tr7,r1,0x7589
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = r1 | 0x75890000<32>");
    }

    [Test]
    public void M88kRw_or_u_r0_imm()
    {
        Given_HexString("5CE07589");
        AssertCode(   //or.u\tr7,r0,0x7589
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = 0x75890000<32>");
    }


    [Test]
    public void M88kRw_or_nop()
    {
        Given_HexString("F4005800");
        AssertCode( // or\tr0,r0,r0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void M88kRw_rot()
    {
        Given_HexString("F789A81D");
        AssertCode(   //rot\tr28,r9,r29
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = __ror<word32,word32>(r9, r29)");
    }

    [Test]
    public void M88kRw_rot_imm()
    {
        Given_HexString("F2FFAA9A");
        AssertCode(   //rot\tr23,r31,0x1A
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r23 = __ror<word32,byte>(r31, 0x1A<8>)");
    }

    [Test]
    public void M88kRw_set()
    {
        Given_HexString("F7C58935");
        AssertCode(   //set\tr30,r5,r21
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v6 = SLICE(r21, word5, 0)",
            "2|L--|v5 = SLICE(r21, word5, 5)",
            "3|L--|r30 = __clr(r5, v5, v6)");
    }

    [Test]
    public void M88kRw_set_imm()
    {
        Given_HexString("F389881D");
        AssertCode(   //set\tr28,r9,0x1C,0x1D
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = SEQ(SLICE(0xFFFFFFF<28>, word3, 0), SLICE(r9, word29, 0))");
    }

    [Test]
    public void M88kRw_st()
    {
        Given_HexString("2416228F");
        AssertCode(   //st\tr0,r22,0x228F
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r22 + 0x228F<32>:word32] = 0<32>");
    }

    [Test]
    public void M88kRw_st_idx()
    {
        Given_HexString("F552240B");
        AssertCode(   //st\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11:word32] = r10");
    }

    [Test]
    public void M88kRw_st_usr_idx_unscaled()
    {
        Given_HexString("F552250B");
        AssertCode( // st.usr\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word32>(r18 + r11, r10)");
    }

    [Test]
    public void M88kRw_st_idx_scaled()
    {
        Given_HexString("F552260B");
        AssertCode( // st\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11 * 4<32>:word32] = r10");
    }

    [Test]
    public void M88kRw_st_idx_scaled_usr()
    {
        Given_HexString("F552270B");
        AssertCode(  // st.usr\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word32>(r18 + r11 * 4<32>, r10)");
    }

    [Test]
    public void M88kRw_st_b()
    {
        Given_HexString("2C4F5E88");
        AssertCode(   //st.b\tr2,r15,0x5E88
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r15 + 0x5E88<32>:byte] = SLICE(r2, byte, 0)");
    }

    [Test]
    public void M88kRw_st_b_idx()
    {
        Given_HexString("F5522C0B");
        AssertCode(   //st.b\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11:byte] = SLICE(r10, byte, 0)");
    }

    [Test]
    public void M88kRw_st_b_usr_idx()
    {
        Given_HexString("F5522D0B");
        AssertCode( // st.b.usr\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<byte>(r18 + r11, SLICE(r10, byte, 0))");
    }

    [Test]
    public void M88kRw_st_b_idx_scaled()
    {
        Given_HexString("F5522E0B");
        AssertCode( // st.b\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11:byte] = SLICE(r10, byte, 0)");
    }

    [Test]
    public void M88kRw_st_b_usr_idx_scaled()
    {
        Given_HexString("F5522F0B");
        AssertCode( // st.b.usr\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<byte>(r18 + r11, SLICE(r10, byte, 0))");
    }

    [Test]
    public void M88kRw_st_d()
    {
        Given_HexString("2007E3CE");
        AssertCode(   //st.d\tr0,r7,0xE3CE
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r7 + 0xE3CE<32>:word64] = r0_r1");
    }

    [Test]
    public void M88kRw_st_d_idx()
    {
        Given_HexString("F552200B");
        AssertCode(   //st.d\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11:word64] = r10_r11");
    }

    [Test]
    public void M88kRw_st_d_usr_idx()
    {
        Given_HexString("F552210B");
        AssertCode( // st.d.usr\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word64>(r18 + r11, r10_r11)");
    }

    [Test]
    public void M88kRw_st_d_idx_scaled()
    {
        Given_HexString("F552220B");
        AssertCode( // st.d\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11 * 8<32>:word64] = r10_r11");
    }

    [Test]
    public void M88kRw_st_d_usr_idx_scaled()
    {
        Given_HexString("F552230B");
        AssertCode( // st.d.usr\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word64>(r18 + r11 * 8<32>, r10_r11)");
    }

    [Test]
    public void M88kRw_st_h()
    {
        Given_HexString("284B18B3");
        AssertCode(   //st.h\tr2,r11,0x18B3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r11 + 0x18B3<32>:word16] = SLICE(r2, word16, 0)"
        );
    }

    [Test]
    public void M88kRw_st_h_idx()
    {
        Given_HexString("F552280B");
        AssertCode(   //st.h\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11:word16] = SLICE(r10, word16, 0)");
    }

    [Test]
    public void M88kRw_st_h_usr_idx()
    {
        Given_HexString("F552290B");
        AssertCode( // st.h.usr\tr10,r18,r11
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word16>(r18 + r11, SLICE(r10, word16, 0))");
    }

    [Test]
    public void M88kRw_st_h_idx_scaled()
    {
        Given_HexString("F5522A0B");
        AssertCode( // st.h\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|Mem0[r18 + r11 * 2<32>:word16] = SLICE(r10, word16, 0)");
    }

    [Test]
    public void M88kRw_st_h_usr_idx_scaled()
    {
        Given_HexString("F5522B0B");
        AssertCode( // st.h.usr\tr10,r18[r11]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__store_to_userspace<word16>(r18 + r11 * 2<32>, SLICE(r10, word16, 0))");
    }

    [Test]
    public void M88kRw_stcr()
    {
        Given_HexString("80DE804C");
        AssertCode(   //stcr\tEPSR,r30
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__write_cr(EPSR, r30)");
    }

    [Test]
    public void M88kRw_sub()
    {
        Given_HexString("F7897414");
        AssertCode(   //sub\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 - r20");
    }

    [Test]
    public void M88kRw_sub_co()
    {
        Given_HexString("F7897514");
        AssertCode( // sub.co\tr28,r9,r20
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r28 = r9 - r20",
            "2|L--|C = cond(r28)");
    }

    [Test]
    public void M88kRw_sub_ci()
    {
        Given_HexString("F7897614");
        AssertCode( // sub.ci\tr28,r9,r20
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r28 = r9 - r20 - C");
    }

    [Test]
    public void M88kRw_sub_cio()
    {
        Given_HexString("F7897714");
        AssertCode( // sub.cio\tr28,r9,r20
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r28 = r9 - r20 - C",
            "2|L--|C = cond(r28)");
    }

    [Test]
    public void M88kRw_sub_imm()
    {
        Given_HexString("757B4F74");
        AssertCode(   //sub\tr11,r27,0x4F74
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r11 = r27 - 0x4F74<32>");
    }

    [Test]
    public void M88kRw_subu()
    {
        Given_HexString("F5C7641F");
        AssertCode(   //subu\tr14,r7,r31
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 - r31");
    }

    [Test]
    public void M88kRw_subu_co()
    {
        Given_HexString("F5C7651F");
        AssertCode( // subu.co\tr14,r7,r31
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r14 = r7 - r31",
            "2|L--|C = cond(r14)");
    }

    [Test]
    public void M88kRw_subu_ci()
    {
        Given_HexString("F5C7661F");
        AssertCode( // subu.ci\tr14,r7,r31
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 - r31 - C");
    }

    [Test]
    public void M88kRw_subu_cio()
    {
        Given_HexString("F5C7671F");
        AssertCode( // subu.cio\tr14,r7,r31 
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r14 = r7 - r31 - C",
            "2|L--|C = cond(r14)");
    }

    [Test]
    public void M88kRw_subu_imm()
    {
        Given_HexString("650703E0");
        AssertCode(   //subu\tr0,r7,0x3E0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = r7 - 0x3E0<32>");
    }

    [Test]
    public void M88kRw_subu_imm_r0()
    {
        Given_HexString("640703E0");
        AssertCode(   //subu\tr0,r7,0x3E0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void M88kRw_tb0_imm()
    {
        Given_HexString("F139D227");
        AssertCode(   //tb0\t0x11,r25,0x27
            "0|L--|00100000(4): 2 instructions",
            "1|T--|if (__bit<word32,byte>(r25, 0x11<8>)) branch 00100004",
            "2|L--|__trap(0x27<16>)");
    }

    [Test]
    public void M88kRw_tb1_imm()
    {
        Given_HexString("F0AAD85F");
        AssertCode(   //tb1\t2,r10,0x5F
            "0|L--|00100000(4): 2 instructions",
            "1|T--|if (!__bit<word32,byte>(r10, 2<8>)) branch 00100004",
            "2|L--|__trap(0x5F<16>)");
    }

    [Test]
    public void M88kRw_tbnd()
    {
        Given_HexString("F87381F9");
        AssertCode(   //tbnd\tr19,0x81F9
            "0|L--|00100000(4): 2 instructions",
            "1|T--|if (r19 <=u 0x81F9<32>) branch 00100004",
            "2|L--|__trap_bounds_violation()");
    }

    [Test]
    public void M88kRw_tcnd_imm()
    {
        Given_HexString("F1F9E9B5");
        AssertCode(   //tcnd\t0xF,r25,0x1B5
            "0|L--|00100000(4): 2 instructions",
            "1|T--|if (__bcnd(r25)) branch 00100004",
            "2|L--|__trap(0x1B5<16>)");
    }

    [Test]
    public void M88kRw_trnc_s()
    {
        Given_HexString("85D25803");
        AssertCode(   //trnc.s\tr14,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = CONVERT(truncf(r3), real32, int32)");
    }

    [Test]
    public void M88kRw_trnc_d()
    {
        Given_HexString("85D25883");
        AssertCode( // trnc.d\tr14,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = CONVERT(trunc(r3_r4), real64, int32)");
    }

    [Test]
    public void M88kRw_xcr()
    {
        Given_HexString("834AC037");
        AssertCode(   //xcr\tr26,r10,PSR
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r26 = __exchange_cr(r10, PSR)");
    }

    [Test]
    public void M88kRw_xmem()
    {
        Given_HexString("05C5AF45");
        AssertCode(   //xmem\tr14,r5,0xAF45
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = std::atomic_exchange<word32>(r5 + 0xAF45<32>, r14)");
    }

    [Test]
    public void M88kRw_xmem_idx_unscaled()
    {
        Given_HexString("F6420042");
        AssertCode(   //xmem\tr18,r2,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = std::atomic_exchange<word32>(r2 + r2, r18)");
    }

    [Test]
    public void M88kRw_xmem_usr_idx_unscaled()
    {
        Given_HexString("F6420142");
        AssertCode( // xmem.usr\tr18,r2,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = __atomic_exchange_userspace(r2 + r2, r18)");
    }

    [Test]
    public void M88kRw_xmem_idx_scaled()
    {
        Given_HexString("F6420242");
        AssertCode( // xmem\tr18,r2[r2]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = std::atomic_exchange<word32>(r2 + r2 * 4<32>, r18)");
    }

    [Test]
    public void M88kRw_xmem_usr_idx_scaled()
    {
        Given_HexString("F6420342");
        AssertCode( // xmem.usr\tr18,r2[r2]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = __atomic_exchange_userspace(r2 + r2 * 4<32>, r18)");
    }

    [Test]
    public void M88kRw_xmem_bu()
    {
        Given_HexString("02C9B9A9");
        AssertCode(   //xmem.bu\tr22,r9,0xB9A9
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r22 = std::atomic_exchange<word32>(r9 + 0xB9A9<32>, r22)"
);
    }

    [Test]
    public void M88kRw_xmem_bu_idx_unscaled()
    {
        Given_HexString("F6460442");
        AssertCode(   //xmem.bu\tr18,r6,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = std::atomic_exchange<word32>(r6 + r2, r18)");
    }

    [Test]
    public void M88kRw_xmem_bu_usr_idx_unscaled()
    {
        Given_HexString("F6460542");
        AssertCode( // xmem.bu.usr\tr18,r6,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = __atomic_exchange_userspace(r6 + r2, r18)");
    }

    [Test]
    public void M88kRw_xmem_bu_idx_scaled()
    {
        Given_HexString("F6460642");
        AssertCode( // xmem.bu\tr18,r6[r2]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = std::atomic_exchange<word32>(r6 + r2, r18)");
    }

    [Test]
    public void M88kRw_xmem_bu_usr_idx_scaled()
    {
        Given_HexString("F6460742");
        AssertCode( // xmem.bu.usr\tr18,r6[r2]
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r18 = __atomic_exchange_userspace(r6 + r2, r18)");
    }

    [Test]
    public void M88kRw_xor()
    {
        Given_HexString("F5C7502F");
        AssertCode(   //xor\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 ^ r15");
    }

    [Test]
    public void M88kRw_xor_c()
    {
        Given_HexString("F5C7542F");
        AssertCode(   //xor.c\tr14,r7,r15
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r14 = r7 ^ ~r15");
    }

    [Test]
    public void M88kRw_xor_imm()
    {
        Given_HexString("5061D608");
        AssertCode(   //xor\tr3,r1,0xD608
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = r1 ^ 0xD608<32>");
    }

    [Test]
    public void M88kRw_xor_u_imm()
    {
        Given_HexString("54501E64");
        AssertCode(   //xor.u\tr2,r16,0x1E64
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = r16 ^ 0x1E640000<32>");
    }
}