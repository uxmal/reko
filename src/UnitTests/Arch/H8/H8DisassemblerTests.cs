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
using Reko.Arch.H8;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.H8
{
    [TestFixture]
    public class H8DisassemblerTests : DisassemblerTestBase<H8Instruction>
    {
        private readonly H8Architecture arch;
        private readonly Address addrLoad;

        public H8DisassemblerTests()
        {
            this.arch = new H8Architecture(CreateServiceContainer(), "h8", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr16(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void H8Dis_add_reg_reg()
        {
            AssertCode("add.w\tr3,r4", "0934");
        }

        [Test]
        public void H8Dis_add_b_imm()
        {
            AssertCode("add.b\t#0x0C,r3l", "8B0C");
        }

        [Test]
        public void H8Dis_add_b_reg()
        {
            AssertCode("add.b\tr2l,r0h", "08A0");
        }

        [Test]
        public void H8Dis_add_i16()
        {
            AssertCode("add.w\t#0x1234,r3", "79131234");
        }

        [Test]
        public void H8Dis_add_i32()
        {
            AssertCode("add.l\t#0x12345678,er3", "7A1312345678");
        }

        [Test]
        public void H8Dis_adds_1()
        {
            AssertCode("adds\t#0x00000001,er5", "0B05");
        }

        [Test]
        public void H8Dis_adds_2()
        {
            AssertCode("adds\t#0x00000002,er3", "0B83");
        }

        [Test]
        public void H8Dis_addx_imm()
        {
            AssertCode("addx.b\t#0xF8,r2l", "9AF8");
        }

        [Test]
        public void H8Dis_addx_reg()
        {
            AssertCode("addx.b\tr0h,r1h", "0E01");
        }

        [Test]
        public void H8Dis_and_l()
        {
            AssertCode("and.l\ter4,er2", "01F06642");
        }

        [Test]
        public void H8Dis_andc_ccr()
        {
            AssertCode("andc\t#0xAA,ccr", "06AA");
        }

        [Test]
        public void H8Dis_andc_exr()
        {
            AssertCode("andc\t#0xAA,exr", "014106AA");
        }

        [Test]
        public void H8Dis_band_imm_aa8()
        {
            AssertCode("band\t#0x00,@0xAA:8", "7EAA7600");
        }

        [Test]
        public void H8Dis_band_imm_aa16()
        {
            AssertCode("band\t#0x00,@0xAAAA:16", "6a10aaaa7600");
        }

        [Test]
        public void H8Dis_band_imm_aa32()
        {
            AssertCode("band\t#0x01,@0xAAAAAAAA:32", "6a30aaaaaaaa7610");
        }

        [Test]
        public void H8Dis_band_ind()
        {
            AssertCode("band\t#0x01,@er0", "7c007610");
        }

        [Test]
        public void H8Dis_bcc()
        {
            AssertCode("bcc\t8070", "446E");
        }

        [Test]
        public void H8Dis_bcc_long()
        {
            AssertCode("bcc\t8000", "5840FFFC");
        }

        [Test]
        public void H8Dis_bclr_imm()
        {
            AssertCode("bclr\t#0x04,@er0", "7D007240");
        }

        [Test]
        public void H8Dis_bclr_imm_aa8()
        {
            AssertCode("bclr\t#0x07,@0x6D:8", "7F6D 7270");
        }

        [Test]
        public void H8Dis_bclr_imm_aa16()
        {
            AssertCode("bclr\t#0x01,@0xAAAA:16", "6a18aaaa7210");
        }

        [Test]
        public void H8Dis_bclr_imm_aa32()
        {
            AssertCode("bclr\t#0x05,@0x20B97E:32", "6A380020B97E7250");
        }

        [Test]
        public void H8Dis_bclr_reg_aa16()
        {
            AssertCode("bclr\tr0h,@0xAAAA:16", "6a18aaaa6200");
        }

        [Test]
        public void H8Dis_biand_imm_aa16()
        {
            AssertCode("biand\t#0x00,@0xAAAA:16", "6a10aaaa7680");
        }

        [Test]
        public void H8Dis_biand_imm_aa8()
        {
            AssertCode("biand\t#0x00,@0xAA:8", "7eaa7680");
        }

        [Test]
        public void H8Dis_biand_imm_aa32()
        {
            AssertCode("biand\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7680");
        }

        [Test]
        public void H8Dis_biand_reg()
        {
            AssertCode("biand\t#0x00,@er0", "7c007680");
        }

        [Test]
        public void H8Dis_bild_imm_aa8()
        {
            AssertCode("bild\t#0x03,@0x54:8", "7E54 7730");
        }

        [Test]
        public void H8Dis_bild_imm_aa16()
        {
            AssertCode("bild\t#0x00,@0xAAAA:16", "6a10aaaa7780");
        }

        [Test]
        public void H8Dis_bild_imm_aa32()
        {
            AssertCode("bild\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7780");
        }

        [Test]
        public void H8Dis_bild_ind()
        {
            AssertCode("bild\t#0x00,@er0", "7c007780");
        }

        [Test]
        public void H8Dis_bior_ind()
        {
            AssertCode("bior\t#0x00,@er0", "7c007480");
        }

        [Test]
        public void H8Dis_bior_imm_aa8()
        {
            AssertCode("bior\t#0x00,@0xAA:8", "7eaa7480");
        }

        [Test]
        public void H8Dis_bior_imm_aa16()
        {
            AssertCode("bior\t#0x00,@0xAAAA:16", "6a10aaaa7480");
        }

        [Test]
        public void H8Dis_bior_imm_aa32()
        {
            AssertCode("bior\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7480");
        }

        [Test]
        public void H8Dis_bist_imm_aa8()
        {
            AssertCode("bist\t#0x00,@0xAA:8", "7faa6780");
        }

        [Test]
        public void H8Dis_bist_imm_aa16()
        {
            AssertCode("bist\t#0x00,@0xAAAA:16", "6a18aaaa6780");
        }

        [Test]
        public void H8Dis_bist_ind()
        {
            AssertCode("bist\t#0x00,@er0", "7d006780");
        }

        [Test]
        public void H8Dis_bist_reg()
        {
            AssertCode("bist\t#0x00,r2l", "678A");
        }

        [Test]
        public void H8Dis_bixor_imm_aa8()
        {
            AssertCode("bixor\t#0x00,@0xAA:8", "7eaa7580");
        }

        [Test]
        public void H8Dis_bixor_imm_aa16()
        {
            AssertCode("bixor\t#0x00,@0xAAAA:16", "6a10aaaa7580");
        }

        [Test]
        public void H8Dis_bixor_imm_aa32()
        {
            AssertCode("bixor\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7580");
        }

        [Test]
        public void H8Dis_bixor_ind()
        {
            AssertCode("bixor\t#0x00,@er0", "7c007580");
        }

        [Test]
        public void H8Dis_bld()
        {
            AssertCode("bld\t#0x07,r1h", "7771");
        }

        [Test]
        public void H8Dis_bld_ind()
        {
            AssertCode("bld\t#0x00,@er0", "7c007700");
        }

        [Test]
        public void H8Dis_bld_imm_aa16()
        {
            AssertCode("bld\t#0x00,@0xAAAA:16", "6a10aaaa7700");
        }

        [Test]
        public void H8Dis_bld_imm_aa32()
        {
            AssertCode("bld\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7700");
        }

        [Test]
        public void H8Dis_bnot_aa8()
        {
            AssertCode("bnot\tr0l,@0x80:8", "7F806180");
        }

        [Test]
        public void H8Dis_bnot_imm_aa16()
        {
            AssertCode("bnot\t#0x00,@0xAAAA:16", "6a18aaaa7100");
        }

        [Test]
        public void H8Dis_bnot_ind()
        {
            AssertCode("bnot\t#0x00,@er0", "7d007100");
        }

        [Test]
        public void H8Dis_bnot_reg_imm_a16()
        {
            AssertCode("bnot\tr0h,@0xAAAA:16", "6a18aaaa6100");
        }

        [Test]
        public void H8Dis_bnot_reg_ind()
        {
            AssertCode("bnot\tr0h,@er0", "7d006100");
        }

        [Test]
        public void H8Dis_bor_ind()
        {
            AssertCode("bor\t#0x00,@er0", "7c007400");
        }

        [Test]
        public void H8Dis_bor_imm_aa8()
        {
            AssertCode("bor\t#0x00,@0xAA:8", "7eaa7400");
        }

        [Test]
        public void H8Dis_bor_imm_aa16()
        {
            AssertCode("bor\t#0x00,@0xAAAA:16", "6a10aaaa7400");
        }

        [Test]
        public void H8Dis_bor_imm_aa32()
        {
            AssertCode("bor\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7400");
        }

        [Test]
        public void H8Dis_bset_imm()
        {
            AssertCode("bset\t#0x00,r4h", "7004");
        }

        [Test]
        public void H8Dis_bset_imm_aa8()
        {
            AssertCode("bset\t#0x06,@0x6F:8", "7F6F 7060");
        }

        [Test]
        public void H8Dis_bset_imm_aa32()
        {
            AssertCode("bset\t#0x05,@0x2051D0:32", "6A38002051D07050");
        }

        [Test]
        public void H8Dis_bset_imm_abs8()
        {
            AssertCode("bset\t#0x01,@0x60:8", "7F607010");
        }

        [Test]
        public void H8Dis_bset_imm_aa16()
        {
            AssertCode("bset\t#0x00,@0xAAAA:16", "6a18aaaa7000");
        }

        [Test]
        public void H8Dis_bset_imm_ind()
        {
            AssertCode("bset\t#0x03,@er0", "7D007030");
        }

        [Test]
        public void H8Dis_bset_reg_reg()
        {
            AssertCode("bset\tr3l,r1h", "60B1");
        }

        [Test]
        public void H8Dis_bset_reg_imm_a16()
        {
            AssertCode("bset\tr0h,@0xAAAA:16", "6a18aaaa6000");
        }

        [Test]
        public void H8Dis_bset_reg_ind()
        {
            AssertCode("bset\tr0h,@er0", "7d006000");
        }

        [Test]
        public void H8Dis_btst_imm_aa8()
        {
            AssertCode("btst\t#0x00,@0xE5:8", "7EE57300");
        }

        [Test]
        public void H8Dis_btst_indirect()
        {
            AssertCode("btst\t#0x06,@er5", "7C50 7360");
        }

        [Test]
        public void H8Dis_btst_aa32()
        {
            AssertCode("btst\t#0x00,@0x20553C:32", "6A30 0020 553C 7300");
        }

        [Test]
        public void H8Dis_bsr_disp8()
        {
            AssertCode("bsr\t7FFE", "55FE");
        }

        [Test]
        public void H8Dis_bsr_disp16()
        {
            AssertCode("bsr\t7FFC", "5C00FFFC");
        }

        [Test]
        public void H8Dis_bst_imm_aa16()
        {
            AssertCode("bst\t#0x00,@0xAAAA:16", "6a18aaaa6700");
        }

        [Test]
        public void H8Dis_bst_imm_aa8()
        {
            AssertCode("bst\t#0x00,@0xAA:8", "7faa6700");
        }

        [Test]
        public void H8Dis_bst_ind()
        {
            AssertCode("bst\t#0x00,@er0", "7d006700");
        }

        [Test]
        public void H8Dis_btst()
        {
            AssertCode("btst\t#0x00,r4h", "7304");
        }

        [Test]
        public void H8Dis_btst_imm_aa16()
        {
            AssertCode("btst\t#0x00,@0xAAAA:16", "6a10aaaa7300");
        }

        [Test]
        public void H8Dis_btst_ind()
        {
            AssertCode("btst\tr0h,@er0", "7c006300");
        }

        [Test]
        public void H8Dis_btst_reg_imm_aa16()
        {
            AssertCode("btst\tr0h,@0xAAAA:16", "6a10aaaa6300");
        }

        [Test]
        public void H8Dis_bxor_imm_aa8()
        {
            AssertCode("bxor\t#0x00,@0xAA:8", "7eaa7500");
        }

        [Test]
        public void H8Dis_bxor_imm_aa16()
        {
            AssertCode("bxor\t#0x00,@0xAAAA:16", "6a10aaaa7500");
        }

        [Test]
        public void H8Dis_bxor_imm_aa32()
        {
            AssertCode("bxor\t#0x00,@0xAAAAAAAA:32", "6a30aaaaaaaa7500");
        }

        [Test]
        public void H8Dis_bxor_ind()
        {
            AssertCode("bxor\t#0x00,@er0", "7c007500");
        }

        [Test]
        public void H8Dis_clrmac()
        {
            AssertCode("clrmac", "01A0");
        }

        [Test]
        public void H8Dis_cmp_w_reg_reg()
        {
            AssertCode("cmp.w\tr6,r1", "1D61");
        }

        [Test]
        public void H8Dis_cmp_l_reg()
        {
            AssertCode("cmp.l\tsp,er2", "1FF2");
        }

        [Test]
        public void H8Dis_daa()
        {
            AssertCode("daa\tr2l", "0F0A");
        }

        [Test]
        public void H8Dis_das()
        {
            AssertCode("das\tr7h", "1F07");
        }

        [Test]
        public void H8Dis_dec_b()
        {
            AssertCode("dec.b\tr6l", "1A0E");
        }

        [Test]
        public void H8Dis_dec_w()
        {
            AssertCode("dec.w\t#0x0002,r0", "1BD0");
        }

        [Test]
        public void H8Dis_dec_l()
        {
            AssertCode("dec.l\t#0x00000002,er0", "1BF0");
        }

        [Test]
        public void H8Dis_divxs_b()
        {
            AssertCode("divxs.b\tr7l,e3", "01D051FB");
        }

        [Test]
        public void H8Dis_divxs_w()
        {
            AssertCode("divxs.w\te0,er5", "01D05385");
        }

        [Test]
        public void H8Dis_divxu_b()
        {
            AssertCode("divxu.b\tr7l,e3", "51FB");
        }

        [Test]
        public void H8Dis_divxu_w()
        {
            AssertCode("divxu.w\te0,er5", "5385");
        }

        [Test]
        public void H8Dis_inc_w_2()
        {
            AssertCode("inc.w\t#0x0002,r0", "0BD0");
        }

        [Test]
        public void H8Dis_jmp_aa24()
        {
            AssertCode("jmp\t@0x123456:24", "5A123456");
        }

        [Test]
        public void H8Dis_jsr_aa24()
        {
            AssertCode("jsr\t@0x123456:24", "5E123456");
        }

        [Test]
        public void H8Dis_jsr_ind_aa8()
        {
            AssertCode("jsr\t@@0x54:8", "5F54");
        }

        [Test]
        public void H8Dis_jsr_indirect()
        {
            AssertCode("jsr\t@er2", "5D20");
        }

        [Test]
        public void H8Dis_ldc_imm_aa32()
        {
            AssertCode("ldc.w\t@0xAAAAAAAA:32,ccr", "01406b20aaaaaaaa");
        }

        [Test]
        public void H8Dis_ldc_exr_imm_aa32()
        {
            AssertCode("ldc.w\t@0xAAAAAAAA:32,exr", "01416b20aaaaaaaa");
        }

        [Test]
        public void H8Dis_ldc_exr_imm()
        {
            AssertCode("ldc.b\t#0xAA,exr", "014107aa");
        }

        [Test]
        public void H8Dis_ldc_exr_postinc()
        {
            AssertCode("ldc.w\t@er0+,exr", "01416d00");
        }

        [Test]
        public void H8Dis_ldc_exr_0()
        {
            AssertCode("ldc.b\tr0h,exr", "0310");
        }

        [Test]
        public void H8Dis_ldc_imm_aa16()
        {
            AssertCode("ldc.w\t@0xAAAA:16,ccr", "01406b00aaaa");
        }

        [Test]
        public void H8Dis_ldc_exr_imm_aa16()
        {
            AssertCode("ldc.w\t@0xAAAA:16,exr", "01416b00aaaa");
        }

        [Test]
        public void H8Dis_ldc_reg()
        {
            AssertCode("ldc.b\tr4l,ccr", "030C");
        }

        [Test]
        public void H8Dis_ldc_disp16()
        {
            AssertCode("ldc.w\t@(-2:16,er3),ccr", "01406F30FFFE");
        }

        [Test]
        public void H8Dis_ldc_exr_reg()
        {
            AssertCode("ldc.w\t@er0,exr", "01416900");
        }

        [Test]
        public void H8Dis_ldc_exr_disp16()
        {
            AssertCode("ldc.w\t@(-21846:16,er0),exr", "01416f00aaaa");
        }

        [Test]
        public void H8Dis_ldc_disp32()
        {
            AssertCode("ldc.w\t@(0xAAAAAAAA:32,er0),ccr", "014078006b20aaaaaaaa");
        }

        [Test]
        public void H8Dis_ldc_exr_disp32()
        {
            AssertCode("ldc.w\t@(0xAAAAAAAA:32,er0),exr", "014178006b20aaaaaaaa");
        }

        [Test]
        public void H8Dis_ldm_l()
        {
            AssertCode("ldm.l\t@sp+,(er4-er5)", "01106D75");
            AssertCode("ldm.l\t@sp+,(er4-er6)", "01206D76");
            AssertCode("ldm.l\t@sp+,(er4-sp)", "01306D77");
        }

        [Test]
        public void H8Dis_ldmac()
        {
            AssertCode("ldmac\ter1,mach", "0321");
            AssertCode("ldmac\ter1,macl", "0331");
        }


        [Test]
        public void H8Dis_mac()
        {
            AssertCode("mac\t@er3+,@er4+", "01606D34");
        }

        [Test]
        public void H8Dis_mov_predec()
        {
            AssertCode("mov.w\tr1,@-sp", "6DF1");
        }

        [Test]
        public void H8Dis_mov_b_abs8()
        {
            AssertCode("mov.b\t@0x79:8,r0h", "207942");
        }

        [Test]
        public void H8Dis_mov_b_base_offset()
        {
            AssertCode("mov.b\t@(4660:16,er2),r0h", "6E201234");
        }

        [Test]
        public void H8Dis_mov_b_disp32()
        {
            AssertCode("mov.b\tr2h,@(0x12345678:32,er4)", "78406AA212345678");
        }

        [Test]
        public void H8Dis_mov_b_disp32b()
        {
            AssertCode("mov.b\tr2l,@(0x12345678:32,er4)", "78406AAA12345678");
        }

        [Test]
        public void H8Dis_mov_b_disp32c()
        {
            AssertCode("mov.b\tr2l,@(0x12345678:32,er4)", "78406AAA12345678");
        }

        [Test]
        public void H8Dis_mov_b_imm()
        {
            AssertCode("mov.b\t#0xFF,r7l", "FFFF");
        }

        [Test]
        public void H8Dis_mov_b_indirect()
        {
            AssertCode("mov.b\t@er6,r5h", "6865");
        }

        [Test]
        public void H8Dis_mov_b_reg_to_indirect_disp32()
        {
            AssertCode("mov.b\tr1h,@(0x23512345:32,er3)", "78306AA123512345");
        }

        [Test]
        public void H8Dis_mov_l_abs16_to_reg()
        {
            AssertCode("mov.l\t@0xFEDC:16,er0", "01 00 6B 00 fe dc");
        }

        [Test]
        public void H8Dis_mov_l_abs32to_reg()
        {
            AssertCode("mov.l\t@0x12345678:32,er0", "01 00 6B 20 12 34 56 78");
        }

        [Test]
        public void H8Dis_mov_l_disp32_to_reg()
        {
            AssertCode("mov.l\t@(0x205C4A:32,er0),er0", "0100 7800 6B20 0020 5C4A");
        }

        [Test]
        public void H8Dis_mov_l_abs16_to_mem()
        {
            AssertCode("mov.l\t@0xFEDC:16,er0", "01 00 6b 00 fe dc");
        }

        [Test]
        public void H8Dis_mov_l_abs32_to_mem()
        {
            AssertCode("mov.l\t@0x12345678:32,er0", "01 00 6b 20 12 34 56 78");
        }

        [Test]
        public void H8Dis_mov_l_indirect_to_reg()
        {
            AssertCode("mov.l\t@er0,er3", "01006903");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp16_to_reg()
        {
            AssertCode("mov.l\ter3,@(-2:16,er0)", "01006F83FFFE");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp32_to_mem()
        {
            AssertCode("mov.l\ter1,@(0x23512345:32,er3)", "010078306BA123512345");
        }

        [Test]
        public void H8Dis_mov_l_indirect_to_mem()
        {
            AssertCode("mov.l\ter3,@er0", "01006983");
        }

        [Test]
        public void H8Dis_mov_l_indirect_disp_to_mem()
        {
            AssertCode("mov.l\ter3,@(-2:16,er0)", "01006F83FFFE");
        }

        [Test]
        public void H8Dis_mov_l_postinc()
        {
            AssertCode("mov.l\t@er0+,er3", "01006D03");
        }

        [Test]
        public void H8Dis_mov_l_preinc()
        {
            AssertCode("mov.l\ter3,@-er0", "01006D83");
        }

        [Test]
        public void H8Dis_mov_postinc()
        {
            AssertCode("mov.w\t@sp+,r2", "6D72");
        }

        [Test]
        public void H8Dis_mov_w_aa16()
        {
            AssertCode("mov.w\t@0x1234:16,e6", "6B0E 1234");
        }

        [Test]
        public void H8Dis_mov_w_aa32()
        {
            AssertCode("mov.w\t@0x5546F:32,r0", "6B20 0005 546F");
        }

        [Test]
        public void H8Dis_mov_w_aa32_2()
        {
            AssertCode("mov.w\t@0x12345600:32,e6", "6B2E 12345600");
        }

        [Test]
        public void H8Dis_mov_w_base_offset_16()
        {
            AssertCode("mov.w\t@(4660:16,sp),r5", "6F751234");
        }

        [Test]
        public void H8Dis_mov_w_base_offset_32()
        {
            AssertCode("mov.w\t@(0x7C50C:32,er1),r0", "7810 6B20 0007 C50C");
        }

        [Test]
        public void H8Dis_mov_w_imm()
        {
            AssertCode("mov.w\t#0x1234,r3", "79031234");
        }

        [Test]
        public void H8Dis_mov_w_reg_reg()
        {
            AssertCode("mov.w\tr5,r3", "0D53");
        }

        [Test]
        public void H8Dis_movfpe()
        {
            AssertCode("movfpe\t@0x123:16,r3h", "6A430123");
        }

        [Test]
        public void H8Dis_movtpe()
        {
            AssertCode("movtpe\tr3h,@0x123:16", "6AC30123");
        }

        [Test]
        public void H8Dis_mulxs_b()
        {
            AssertCode("mulxs.b\tr3h,r4", "01C05034");
        }

        [Test]
        public void H8Dis_mulxs_w()
        {
            AssertCode("mulxs.w\tr3,er4", "01C05234");
        }

        [Test]
        public void H8Dis_mulxu_b()
        {
            AssertCode("mulxu.b\tr1h,r2h", "5012");
        }

        [Test]
        public void H8Dis_not_b()
        {
            AssertCode("not.b\tr2l", "170A");
        }

        [Test]
        public void H8Dis_or_b()
        {
            AssertCode("or.b\tr1l,r0l", "1498");
        }

        [Test]
        public void H8Dis_or_b_imm()
        {
            AssertCode("or.b\t#0x10,r0h", "C010");
        }

        [Test]
        public void H8Dis_or_l()
        {
            AssertCode("or.l\ter4,er2", "01F06442");
        }

        [Test]
        public void H8Dis_orc_exr_imm()
        {
            AssertCode("orc\t#0xAA,exr", "014104aa");
        }

        [Test]
        public void H8Dis_rotl_b()
        {
            AssertCode("rotl.b\t#0x0002,r0h", "12c0");
        }

        [Test]
        public void H8Dis_rotl_w()
        {
            AssertCode("rotl.w\t#0x0002,r0", "12d0");
        }

        [Test]
        public void H8Dis_rotl_l()
        {
            AssertCode("rotl.l\t#0x0002,er0", "12f0");
        }

        [Test]
        public void H8Dis_rotr_b()
        {
            AssertCode("rotr.b\t#0x0002,r0h", "13c0");
        }

        [Test]
        public void H8Dis_rotr_w()
        {
            AssertCode("rotr.w\t#0x0002,r0", "13d0");
        }

        [Test]
        public void H8Dis_rotr_l()
        {
            AssertCode("rotr.l\t#0x0002,er0", "13f0");
        }

        [Test]
        public void H8Dis_rotxl_b()
        {
            AssertCode("rotxl.b\tr2h", "1202");
        }

        [Test]
        public void H8Dis_rotxl_b_r0()
        {
            AssertCode("rotxl.b\t#0x0002,r0h", "1240");
        }

        [Test]
        public void H8Dis_rotxl_w()
        {
            AssertCode("rotxl.w\t#0x0002,r0", "1250");
        }

        [Test]
        public void H8Dis_rotxl_l()
        {
            AssertCode("rotxl.l\t#0x0002,er0", "1270");
        }

        [Test]
        public void H8Dis_rotxr_b()
        {
            AssertCode("rotxr.b\tr2l", "130A");
        }

        [Test]
        public void H8Dis_rotxr_b_imm2()
        {
            AssertCode("rotxr.b\t#0x0002,r0h", "1340");
        }

        [Test]
        public void H8Dis_rotxr_w()
        {
            AssertCode("rotxr.w\t#0x0002,r0", "1350");
        }

        [Test]
        public void H8Dis_rotxr_l()
        {
            AssertCode("rotxr.l\t#0x0002,er0", "1370");
        }

        [Test]
        public void H8Dis_rts()
        {
            AssertCode("rts", "5470");
        }

        [Test]
        public void H8Dis_shar_b()
        {
            AssertCode("shar.b\tr2h", "1182");
        }

        [Test]
        public void H8Dis_shll_b()
        {
            AssertCode("shll.b\tr2l", "100A");
        }

        [Test]
        public void H8Dis_shll_l_2()
        {
            AssertCode("shll.l\t#0x0002,er1", "1071");
        }

        [Test]
        public void H8Dis_shll_w_1()
        {
            AssertCode("shll.w\tr1", "1011");
        }

        [Test]
        public void H8Dis_shll_w_2()
        {
            AssertCode("shll.w\t#0x0002,r1", "1051");
        }

        [Test]
        public void H8Dis_sleep()
        {
            AssertCode("sleep", "0180");
        }

        [Test]
        public void H8Dis_sub()
        {
            AssertCode("sub.l\ter0,er0", "1A80");
        }

        [Test]
        public void H8Dis_stc_ind()
        {
            AssertCode("stc.w\tccr,@sp", "014069F0");
        }

        [Test]
        public void H8Dis_stc_b_imm_aa16()
        {
            AssertCode("stc.w\tccr,@0xAAAA:16", "01406b80aaaa");
        }

        [Test]
        public void H8Dis_stc_w_exr_disp32()
        {
            AssertCode("stc.w\texr,@(0xAAAAAAAA:32,er0)", "014178006ba0aaaaaaaa");
        }

        [Test]
        public void H8Dis_stc_b_exr()
        {
            AssertCode("stc.b\texr,r0h", "0210");
        }

        [Test]
        public void H8Dis_stc_b_exr_ind()
        {
            AssertCode("stc.w\texr,@er0", "01416980");
        }

        [Test]
        public void H8Dis_stc_b_exr_disp32()
        {
            AssertCode("stc.w\texr,@(-21846:16,er0)", "01416f80aaaa");
        }

        [Test]
        public void H8Dis_stc_b_disp32()
        {
            AssertCode("stc.w\tccr,@(0xAAAAAAAA:32,er0)", "014078006ba0aaaaaaaa");
        }

        [Test]
        public void H8Dis_stc_b_exr_sub_predec()
        {
            AssertCode("stc.w\texr,@-er0", "01416d80");
        }

        [Test]
        public void H8Dis_stc_b_ind_predec()
        {
            AssertCode("stc.w\tccr,@-er0", "01406d80");
        }

        [Test]
        public void H8Dis_stc_b_exr_imm_aa16()
        {
            AssertCode("stc.w\texr,@0xAAAA:16", "01416b80aaaa");
        }

        [Test]
        public void H8Dis_stc_b_imm_aa32()
        {
            AssertCode("stc.w\tccr,@0xAAAAAAAA:32", "01406ba0aaaaaaaa");
        }

        [Test]
        public void H8Dis_stc_b_exr_imm_aa32()
        {
            AssertCode("stc.w\texr,@0xAAAAAAAA:32", "01416ba0aaaaaaaa");
        }

        [Test]
        public void H8Dis_stm_l()
        {
            AssertCode("stm.l\t(er4-er5),@-sp", "01106DF4");
            AssertCode("stm.l\t(er4-er6),@-sp", "01206DF4");
            AssertCode("stm.l\t(er4-sp),@-sp", "01306DF4");
        }

        [Test]
        public void H8Dis_stmac()
        {
            AssertCode("stmac\tmach,er1", "0221");
            AssertCode("stmac\tmacl,er1", "0231");
        }

        [Test]
        public void H8Dis_sub_w_regs()
        {
            AssertCode("sub.w\tr1,r2", "1912");
        }

        [Test]
        public void H8Dis_subs_1()
        {
            AssertCode("subs\t#0x00000002,sp", "1B87");
        }

        [Test]
        public void H8Dis_tas()
        {
            AssertCode("tas\t@er2", "01E07B2C");
        }

        [Test]
        public void H8Dis_trapa()
        {
            AssertCode("trapa\t#0x03", "5730");
        }

        [Test]
        public void H8Dis_xor_l()
        {
            AssertCode("xor.l\ter4,er2", "01F06542");
        }

        [Test]
        public void H8Dis_xor_w()
        {
            AssertCode("xor.w\tr2,e4", "652C");
        }

        [Test]
        public void H8Dis_xorc()
        {
            AssertCode("xorc\t#0xDC,ccr", "05DC");
        }

        [Test]
        public void H8Dis_xorc_exr()
        {
            AssertCode("xorc\t#0xAA,exr", "014105aa");
        }
    }
}