#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Motorola.M88k;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Motorola.M88k;

[TestFixture]
public class M88kDisassemblerTests : DisassemblerTestBase<M88kInstruction>
{
    private readonly M88kArchitecture arch;
    private readonly Address addr;

    public M88kDisassemblerTests()
    {
        var sc = new ServiceContainer();
        this.arch = new M88kArchitecture(sc, "m88k", []);
        this.addr = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = base.DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(sExpected));
    }

    [Test]
    public void M88kDasm_add()
    {
        AssertCode("add\tr28,r9,r20", "F7897014");
        AssertCode("add.co\tr28,r9,r20", "F7897114");
        AssertCode("add.ci\tr28,r9,r20", "F7897214");
        AssertCode("add.cio\tr28,r9,r20", "F7897314");
    }

    [Test]
    public void M88kDasm_add_imm()
    {
        AssertCode("add\tr30,r5,0xA1E5", "73C5A1E5");
    }

    [Test]
    public void M88kDasm_addu()
    {
        AssertCode("addu\tr14,r7,r31", "F5C7601F");
        AssertCode("addu.co\tr14,r7,r31", "F5C7611F");
        AssertCode("addu.ci\tr14,r7,r31", "F5C7621F");
        AssertCode("addu.cio\tr14,r7,r31", "F5C7631F");
    }

    [Test]
    public void M88kDasm_addu_imm()
    {
        AssertCode("addu\tr12,r3,0x1155", "61831155");
    }

    [Test]
    public void M88kDasm_and()
    {
        AssertCode("and\tr14,r7,r15", "F5C7402F");
    }

    [Test]
    public void M88kDasm_and_c()
    {
        AssertCode("and.c\tr14,r7,r15", "F5C7442F");
    }

    [Test]
    public void M88kDasm_and_imm()
    {
        AssertCode("and\tr12,r11,0x2CBE", "418B2CBE");
    }

    [Test]
    public void M88kDasm_and_u_imm()
    {
        AssertCode("and.u\tr2,r17,0xD31D", "4451D31D");
    }

    [Test]
    public void M88kDasm_bb0()
    {
        AssertCode("bb0\t0xA,r28,000EF520", "D0DCBD48");
    }

    [Test]
    public void M88kDasm_bb0_n()
    {
        AssertCode("bb0.n\t0x1A,r6,000F3D4C", "D486CF53");
    }

    [Test]
    public void M88kDasm_bb1()
    {
        AssertCode("bb1\t4,r8,000F925C", "D9E8E497");
    }

    [Test]
    public void M88kDasm_bb1_n()
    {
        AssertCode("bb1.n\t4,r8,000F925C", "DEE8E497");
    }

    [Test]
    public void M88kDasm_bcnd()
    {
        AssertCode("bcnd\tlt0,r11,000EC18C", "E98BB063");
        AssertCode("bcnd\t0x11,r11,000EC18C", "EA2BB063");
    }

    [Test]
    public void M88kDasm_bcnd_n()
    {
        AssertCode("bcnd.n\t0x14,r27,000E1C7C", "EE9B871F");
    }

    [Test]
    public void M88kDasm_br()
    {
        AssertCode("br\t02343634", "C0890D8D");
    }

    [Test]
    public void M88kDasm_br_n()
    {
        AssertCode("br.n\tFFF3EC44", "C7F8FB11");
    }

    [Test]
    public void M88kDasm_bsr()
    {
        AssertCode("bsr\t0120DCAC", "C844372B");
    }

    [Test]
    public void M88kDasm_bsr_n()
    {
        AssertCode("bsr.n\tFE1FA704", "CF83E9C1");
    }

    [Test]
    public void M88kDasm_clr()
    {
        AssertCode("clr\tr28,r9,r29", "F789801D");
    }

    [Test]
    public void M88kDasm_clr_imm()
    {
        AssertCode("clr\tr0,r29,0,0xF", "F01D812F");
    }

    [Test]
    public void M88kDasm_cmp()
    {
        AssertCode("cmp\tr28,r9,r20", "F7897C14");
        AssertCode("Invalid", "F7897D14");
        AssertCode("Invalid", "F7897E14");
        AssertCode("Invalid", "F7897F14");
    }

    [Test]
    public void M88kDasm_cmp_imm()
    {
        AssertCode("cmp\tr13,r7,0xCA08", "7DA7CA08");
    }

    [Test]
    public void M88kDasm_div()
    {
        AssertCode("div\tr28,r9,r20", "F7897814");
        AssertCode("Invalid", "F7897914");
        AssertCode("Invalid", "F7897A14");
        AssertCode("Invalid", "F7897B14");
    }

    [Test]
    public void M88kDasm_div_imm()
    {
        AssertCode("div\tr16,r10,0x2C60", "7A0A2C60");
    }

    [Test]
    public void M88kDasm_divu()
    {
        AssertCode("divu\tr28,r9,r20", "F7896814");
        AssertCode("Invalid", "F7896914");
        AssertCode("Invalid", "F7896A14");
        AssertCode("Invalid", "F7896B14");
    }

    [Test]
    public void M88kDasm_divu_imm()
    {
        AssertCode("divu\tr10,r19,0x4416", "69534416");
    }

    [Test]
    public void M88kDasm_ext()
    {
        AssertCode("ext\tr28,r9,0x1C,0x1D", "F389901D");
    }

    [Test]
    public void M88kDasm_ext_imm()
    {
        AssertCode("ext\tr6,r21,r24", "F4D59078");
    }

    [Test]
    public void M88kDasm_extu()
    {
        AssertCode("extu\tr23,r20,r24", "F6F49A18");
    }

    [Test]
    public void M88kDasm_extu_imm()
    {
        AssertCode("extu\tr28,r9,0x1C,0x1D", "F389981D");
    }

    [Test]
    public void M88kDasm_fadd()
    {
        AssertCode("fadd.sss\tr21,r29,r6", "86BD2806");
        AssertCode("fadd.dss\tr21,r29,r6", "86BD2826");
        AssertCode("fadd.ssd\tr21,r29,r6", "86BD2886");
        AssertCode("fadd.dsd\tr21,r29,r6", "86BD28A6");
        AssertCode("fadd.sds\tr21,r29,r6", "86BD2A06");
        AssertCode("fadd.dds\tr21,r29,r6", "86BD2A26");
        AssertCode("fadd.dds\tr21,r29,r6", "86BD2A86");
        AssertCode("fadd.ddd\tr21,r29,r6", "86BD2AA6");
    }

    [Test]
    public void M88kDasm_fdiv()
    {
        AssertCode("fdiv.sss\tr9,r17,r1", "85317001");
        AssertCode("fdiv.dss\tr9,r17,r1", "85317021");
        AssertCode("fdiv.ssd\tr9,r17,r1", "85317081");
        AssertCode("fdiv.dsd\tr9,r17,r1", "853170A1");
        AssertCode("fdiv.sds\tr9,r17,r1", "85317201");
        AssertCode("fdiv.dds\tr9,r17,r1", "85317221");
        AssertCode("fdiv.dds\tr9,r17,r1", "85317281");
        AssertCode("fdiv.ddd\tr9,r17,r1", "853172A1");
    }

    [Test]
    public void M88kDasm_fcmp()
    {
        AssertCode("fcmp.sss\tr25,r30,r11", "873E380B");
        AssertCode("fcmp.ssd\tr25,r30,r11", "873E388B");
        AssertCode("fcmp.sds\tr25,r30,r11", "873E3A0B");
        AssertCode("fcmp.dds\tr25,r30,r11", "873E3A8B");
    }

    [Test]
    public void M88kDasm_ff0()
    {
        AssertCode("ff0\tr9,r25", "F530EC19");
        //AssertCode("ff0\rr9,r9",  "F536EC49");
    }

    [Test]
    public void M88kDasm_ff1()
    {
        AssertCode("ff1\tr9,r25", "F530E819");
        //AssertCode("@@@", "F536E849");
    }

    [Test]
    public void M88kDasm_fldcr()
    {
        AssertCode("fldcr\tr11,FPHS1", "81704820");
    }

    [Test]
    public void M88kDasm_flt()
    {
        AssertCode("flt.s\tr3,r5", "84722005");
        AssertCode("flt.d\tr3,r5", "84722025");
    }

    [Test]
    public void M88kDasm_fstcr()
    {
        AssertCode("fstcr\tFPECR,r22", "82168812");
    }

    [Test]
    public void M88kDasm_fsub()
    {
        AssertCode("fsub.sss\tr16,r0,r10", "8600300A");
        AssertCode("fsub.dss\tr16,r0,r10", "8600302A");
        AssertCode("fsub.ssd\tr16,r0,r10", "8600308A");
        AssertCode("fsub.dsd\tr16,r0,r10", "860030AA");
        AssertCode("fsub.sds\tr16,r0,r10", "8600320A");
        AssertCode("fsub.dds\tr16,r0,r10", "8600322A");
        AssertCode("fsub.dds\tr16,r0,r10", "8600328A");
        AssertCode("fsub.ddd\tr16,r0,r10", "860032AA");
    }

    [Test]
    public void M88kDasm_fxcr()
    {
        AssertCode("fxcr\tr22,r2,FPLS2", "82C2C883");
    }

    [Test]
    public void M88kDasm_int()
    {
        AssertCode("int.s\tr29,r16", "87A04810");
        AssertCode("int.d\tr29,r16", "87A04890");
    }

    [Test]
    public void M88kDasm_jmp()
    {
        AssertCode("jmp\tr24", "F60EC318");
    }

    [Test]
    public void M88kDasm_jmp_n()
    {
        AssertCode("jmp.n\tr15", "F4F9C64F");
    }

    [Test]
    public void M88kDasm_jsr()
    {
        AssertCode("jsr\tr25", "F772CB19");
    }

    [Test]
    public void M88kDasm_jsr_n()
    {
        AssertCode("jsr.n\tr9", "F536CC49");
    }

    [Test]
    public void M88kDasm_ld()
    {
        AssertCode("ld\tr22,r25,0x7FFF", "16D97FFF");
    }

    [Test]
    public void M88kDasm_ld_idx()
    {
        AssertCode("ld\tr15,r16,r28", "F5F0149C");
        AssertCode("ld.usr\tr15,r16,r28", "F5F0159C");
        AssertCode("ld\tr15,r16[r28]", "F5F0169C");
        AssertCode("ld.usr\tr15,r16[r28]", "F5F0179C");
    }

    [Test]
    public void M88kDasm_ld_b()
    {
        AssertCode("ld.b\tr1,r20,0x39A6", "1C3439A6");
    }

    [Test]
    public void M88kDasm_ld_h_idx()
    {
        AssertCode("ld.h\tr15,r16,r28", "F5F0189C");
        AssertCode("ld.h.usr\tr15,r16,r28", "F5F0199C");
        AssertCode("ld.h\tr15,r16[r28]", "F5F01A9C");
        AssertCode("ld.h.usr\tr15,r16[r28]", "F5F01B9C");
    }

    [Test]
    public void M88kDasm_ld_hu_idx()
    {
        AssertCode("ld.hu\tr15,r16,r28", "F5F0089C");
        AssertCode("ld.hu.usr\tr15,r16,r28", "F5F0099C");
        AssertCode("ld.hu\tr15,r16[r28]", "F5F00A9C");
        AssertCode("ld.hu.usr\tr15,r16[r28]", "F5F00B9C");
    }

    [Test]
    public void M88kDasm_ld_bu_idx()
    {
        AssertCode("ld.bu\tr15,r16,r28", "F5F00C9C");
        AssertCode("ld.bu.usr\tr15,r16,r28", "F5F00D9C");
        AssertCode("ld.bu\tr15,r16[r28]", "F5F00E9C");
        AssertCode("ld.bu.usr\tr15,r16[r28]", "F5F00F9C");
    }

    [Test]
    public void M88kDasm_ld_bu()
    {
        AssertCode("ld.bu\tr3,r0,0x419E", "0C60419E");
    }

    [Test]
    public void M88kDasm_ld_d()
    {
        AssertCode("ld.d\tr0,r9,0x5DD6", "10095DD6");
    }

    [Test]
    public void M88kDasm_ld_d_idx()
    {
        AssertCode("ld.d\tr15,r16,r28", "F5F0109C");
        AssertCode("ld.d.usr\tr15,r16,r28", "F5F0119C");
        AssertCode("ld.d\tr15,r16[r28]", "F5F0129C");
        AssertCode("ld.d.usr\tr15,r16[r28]", "F5F0139C");
    }

    [Test]
    public void M88kDasm_ld_h()
    {
        AssertCode("ld.h\tr0,r0,0xC309", "1800C309");
    }

    [Test]
    public void M88kDasm_ld_hu()
    {
        AssertCode("ld.hu\tr0,r12,0xFBE8", "080CFBE8");
    }

    [Test]
    public void M88kDasm_lda()
    {
        AssertCode("lda\tr28,r8,0x3282", "37883282");
    }

    [Test]
    public void M88kDasm_lda_idx()
    {
        AssertCode("lda\tr24,r23,r1", "F7173401");
        AssertCode("Invalid", "F7173501");
        AssertCode("lda\tr24,r23[r1]", "F7173601");
        AssertCode("Invalid", "F7173701");
    }

    [Test]
    public void M88kDasm_lda_b()
    {
        AssertCode("lda.b\tr22,r26,0x3DB6", "3EDA3DB6");
    }

    [Test]
    public void M88kDasm_lda_b_idx()
    {
        AssertCode("lda.b\tr24,r23,r1", "F7173C01");
        AssertCode("Invalid", "F7173D01");
        AssertCode("lda.b\tr24,r23[r1]", "F7173E01");
        AssertCode("Invalid", "F7173F01");
    }

    [Test]
    public void M88kDasm_lda_d()
    {
        AssertCode("lda.d\tr1,r14,0xE134", "302EE134");
    }

    [Test]
    public void M88kDasm_lda_d_idx()
    {
        AssertCode("lda.d\tr24,r23,r1", "F7173001");
        AssertCode("Invalid", "F7173101");
        AssertCode("lda.d\tr24,r23[r1]", "F7173201");
        AssertCode("Invalid", "F7173301");
    }

    [Test]
    public void M88kDasm_lda_h()
    {
        AssertCode("lda.h\tr2,r10,0xE25", "384A0E25");
    }

    [Test]
    public void M88kDasm_lda_h_idx()
    {
        AssertCode("lda.h\tr24,r23,r1", "F7173801");
        AssertCode("Invalid", "F7173901");
        AssertCode("lda.h\tr24,r23[r1]", "F7173A01");
        AssertCode("Invalid", "F7173B01");
    }

    [Test]
    public void M88kDasm_ldcr()
    {
        AssertCode("ldcr\tr3,DMD0", "806D4138");
    }

    [Test]
    public void M88kDasm_mak()
    {
        AssertCode("mak\tr28,r9,r29", "F789A01D");
    }

    [Test]
    public void M88kDasm_mak_imm()
    {
        AssertCode("mak\tr28,r27,0x1C,7", "F39BA227");
    }

    [Test]
    public void M88kDasm_mask()
    {
        AssertCode("mask\tr1,r2,0xF9", "482200F9");
    }

    [Test]
    public void M88kDasm_mask_u()
    {
        AssertCode("mask.u\tr2,r1,0xA6BD", "4C41A6BD");
    }

    [Test]
    public void M88kDasm_mul()
    {
        AssertCode("mul\tr28,r9,r20", "F7896C14");
        AssertCode("Invalid", "F7896D14");
        AssertCode("Invalid", "F7896E14");
        AssertCode("Invalid", "F7896F14");
    }

    [Test]
    public void M88kDasm_mul_imm()
    {
        AssertCode("mul\tr0,r1,0xBAD9", "6C01BAD9");
    }

    [Test]
    public void M88kDasm_nint()
    {
        AssertCode("nint.s\tr27,r8", "87795008");
        AssertCode("nint.d\tr27,r8", "87795088");
    }

    [Test]
    public void M88kDasm_or()
    {
        AssertCode("or\tr14,r7,r15", "F5C7582F");
    }

    [Test]
    public void M88kDasm_or_c()
    {
        AssertCode("or.c\tr14,r7,r15", "F5C75C2F");
    }

    [Test]
    public void M88kDasm_or_imm()
    {
        AssertCode("or\tr0,r30,0xD55B", "581ED55B");
    }

    [Test]
    public void M88kDasm_or_u_imm()
    {
        AssertCode("or.u\tr7,r0,0x7589", "5CE07589");
    }

    [Test]
    public void M88kDasm_rot()
    {
        AssertCode("rot\tr28,r9,r29", "F789A81D");
    }

    [Test]
    public void M88kDasm_rot_imm()
    {
        AssertCode("rot\tr23,r31,0x1A", "F2FFAA9A");
    }

    [Test]
    public void M88kDasm_set()
    {
        AssertCode("set\tr30,r5,r21", "F7C58935");
    }

    [Test]
    public void M88kDasm_set_imm()
    {
        AssertCode("set\tr28,r9,0x1C,0x1D", "F389881D");
    }

    [Test]
    public void M88kDasm_st()
    {
        AssertCode("st\tr0,r22,0x228F", "2416228F");
    }

    [Test]
    public void M88kDasm_st_idx()
    {
        AssertCode("st\tr10,r18,r11", "F552240B");
        AssertCode("st.usr\tr10,r18,r11", "F552250B");
        AssertCode("st\tr10,r18[r11]", "F552260B");
        AssertCode("st.usr\tr10,r18[r11]", "F552270B");
    }

    [Test]
    public void M88kDasm_st_b()
    {
        AssertCode("st.b\tr2,r15,0x5E88", "2C4F5E88");
    }

    [Test]
    public void M88kDasm_st_b_idx()
    {
        AssertCode("st.b\tr10,r18,r11", "F5522C0B");
        AssertCode("st.b.usr\tr10,r18,r11", "F5522D0B");
        AssertCode("st.b\tr10,r18[r11]", "F5522E0B");
        AssertCode("st.b.usr\tr10,r18[r11]", "F5522F0B");
    }

    [Test]
    public void M88kDasm_st_d()
    {
        AssertCode("st.d\tr0,r7,0xE3CE", "2007E3CE");
    }

    [Test]
    public void M88kDasm_st_d_idx()
    {
        AssertCode("st.d\tr10,r18,r11", "F552200B");
        AssertCode("st.d.usr\tr10,r18,r11", "F552210B");
        AssertCode("st.d\tr10,r18[r11]", "F552220B");
        AssertCode("st.d.usr\tr10,r18[r11]", "F552230B");
    }

    [Test]
    public void M88kDasm_st_h()
    {
        AssertCode("st.h\tr2,r11,0x18B3", "284B18B3");
    }

    [Test]
    public void M88kDasm_st_h_idx()
    {
        AssertCode("st.h\tr10,r18,r11", "F552280B");
        AssertCode("st.h.usr\tr10,r18,r11", "F552290B");
        AssertCode("st.h\tr10,r18[r11]", "F5522A0B");
        AssertCode("st.h.usr\tr10,r18[r11]", "F5522B0B");
    }

    [Test]
    public void M88kDasm_stcr()
    {
        AssertCode("stcr\tEPSR,r30", "80DE804C");
    }

    [Test]
    public void M88kDasm_sub()
    {
        AssertCode("sub\tr28,r9,r20", "F7897414");
        AssertCode("sub.co\tr28,r9,r20", "F7897514");
        AssertCode("sub.ci\tr28,r9,r20", "F7897614");
        AssertCode("sub.cio\tr28,r9,r20", "F7897714");
    }

    [Test]
    public void M88kDasm_sub_imm()
    {
        AssertCode("sub\tr11,r27,0x4F74", "757B4F74");
    }

    [Test]
    public void M88kDasm_subu()
    {
        AssertCode("subu\tr14,r7,r31", "F5C7641F");
        AssertCode("subu.co\tr14,r7,r31", "F5C7651F");
        AssertCode("subu.ci\tr14,r7,r31", "F5C7661F");
        AssertCode("subu.cio\tr14,r7,r31", "F5C7671F");
    }

    [Test]
    public void M88kDasm_subu_imm()
    {
        AssertCode("subu\tr0,r7,0x3E0", "640703E0");
    }

    [Test]
    public void M88kDasm_tb0_imm()
    {
        AssertCode("tb0\t0x11,r25,0x27", "F139D227");
    }

    [Test]
    public void M88kDasm_tb1_imm()
    {
        AssertCode("tb1\t2,r10,0x5F", "F0AAD85F");
    }

    [Test]
    public void M88kDasm_tbnd()
    {
        AssertCode("tbnd\tr19,0x81F9", "F87381F9");
    }

    [Test]
    public void M88kDasm_tcnd_imm()
    {
        AssertCode("tcnd\t0xF,r25,0x1B5", "F1F9E9B5");
    }

    [Test]
    public void M88kDasm_trnc()
    {
        AssertCode("trnc.s\tr14,r3", "85D25803");
        AssertCode("trnc.d\tr14,r3", "85D25883");
    }

    [Test]
    public void M88kDasm_xcr()
    {
        AssertCode("xcr\tr26,r10,PSR", "834AC037");
    }

    [Test]
    public void M88kDasm_xmem()
    {
        AssertCode("xmem\tr14,r5,0xAF45", "05C5AF45");
    }


    [Test]
    public void M88kDasm_xmem_idx()
    {
        AssertCode("xmem\tr18,r2,r2", "F6420042");
        AssertCode("xmem.usr\tr18,r2,r2", "F6420142");
        AssertCode("xmem\tr18,r2[r2]", "F6420242");
        AssertCode("xmem.usr\tr18,r2[r2]", "F6420342");
    }

    [Test]
    public void M88kDasm_xmem_bu()
    {
        AssertCode("xmem.bu\tr22,r9,0xB9A9", "02C9B9A9");
    }

    [Test]
    public void M88kDasm_xmem_bu_idx()
    {
        AssertCode("xmem.bu\tr18,r6,r2", "F6460442");
        AssertCode("xmem.bu.usr\tr18,r6,r2", "F6460542");
        AssertCode("xmem.bu\tr18,r6[r2]", "F6460642");
        AssertCode("xmem.bu.usr\tr18,r6[r2]", "F6460742");
    }

    [Test]
    public void M88kDasm_xor()
    {
        AssertCode("xor\tr14,r7,r15", "F5C7502F");
    }

    [Test]
    public void M88kDasm_xor_c()
    {
        AssertCode("xor.c\tr14,r7,r15", "F5C7542F");
    }

    [Test]
    public void M88kDasm_xor_imm()
    {
        AssertCode("xor\tr3,r1,0xD608", "5061D608");
    }

    [Test]
    public void M88kDasm_xor_u_imm()
    {
        AssertCode("xor.u\tr2,r16,0x1E64", "54501E64");
    }


    [Test]
    public void M88kDasm_xxx()
    {
        var bytes = new byte[65536];
        var rnd = new Random(0x4711);
        rnd.NextBytes(bytes);
        var mem = new ByteMemoryArea(this.addr, bytes);
        var rdr = arch.CreateImageReader(mem, 0);
        var dasm = arch.CreateDisassembler(rdr);
        foreach (var instr in dasm)
        {
        }
    }
}
