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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class Arm64DisassemblerTests : DisassemblerTestBase<AArch64Instruction>
    {
        private IProcessorArchitecture arch = new Arm64Architecture("aarch64");
        private Address baseAddress = Address.Ptr64(0x00100000);
        private AArch64Instruction instr;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        public override Address LoadAddress
        {
            get { return baseAddress; }
        }

        private void Given_Instruction(uint wInstr)
        {
            this.instr = base.DisassembleWord(wInstr);
        }

        private void Expect_Code(string sexp)
        {
            Assert.AreEqual(sexp, instr.ToString());
        }

        [Test]
        public void AArch64Dis_b_label()
        {
            var instr = DisassembleBits("00010111 11111111 11111111 00000000");
            Assert.AreEqual("b\t#&FFC00", instr.ToString());
        }

        [Test]
        public void AArch64Dis_bl_label()
        {
            var instr = DisassembleBits("10010111 11111111 11111111 00000000");
            Assert.AreEqual("bl\t#&FFC00", instr.ToString());
        }

        [Test]
        public void AArch64Dis_br_Xn()
        {
            var instr = DisassembleBits("11010110 00011111 00000011 11000000");
            Assert.AreEqual("br\tx30", instr.ToString());
        }

        [Test]
        public void AArch64Dis_add_Wn_imm()
        {
            var instr = DisassembleBits("000 10001 01 011111111111 10001 10011");
            Assert.AreEqual("add\tw19,w17,#&7FF,lsl #&C", instr.ToString());
        }

        [Test]
        public void AArch64Dis_adds_Xn_imm()
        {
            var instr = DisassembleBits("101 10001 00 011111111111 10001 10011");
            Assert.AreEqual("adds\tx19,x17,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_subs_Wn_imm()
        {
            var instr = DisassembleBits("011 10001 00 011111111111 10001 10011");
            Assert.AreEqual("subs\tw19,w17,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_sub_Xn_imm()
        {
            var instr = DisassembleBits("110 10001 00 011111111111 10001 10011");
            Assert.AreEqual("sub\tx19,x17,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_and_Wn_imm()
        {
            var instr = DisassembleWord(0x120F3041);
            Assert.AreEqual("and\tw1,w2,#&3FFE0000", instr.ToString());
        }

        [Test]
        public void AArch64Dis_and_Xn_imm()
        {
            var instr = DisassembleWord(0x920F3041);
            Assert.AreEqual("and\tx1,x2,#&3FFE0000", instr.ToString());
        }

        [Test]
        public void AArch64Dis_ands_Xn_imm()
        {
            var instr = DisassembleBits("111 100100 0 010101 010101 00100 00111");
            Assert.AreEqual("ands\tx7,x4,#&FFFFF801", instr.ToString());
        }

        [Test]
        public void AArch64Dis_movk_imm()
        {
            var instr = DisassembleBits("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
            Assert.AreEqual("movk\tx7,#&AAA4", instr.ToString());
        }

        [Test]
        public void AArch64Dis_ldp()
        {
            Given_Instruction(0x2D646C2F);
            Expect_Code("ldp\ts15,s27,[x1,-#&E0]");
        }

        [Test]
        public void AArch64Dis_tbz()
        {
            Given_Instruction(0x36686372);
            Expect_Code("tbz\tw18,#&D,#&100C6C");
        }

        [Test]
        public void AArch64Dis_adrp()
        {
            Given_Instruction(0xF00000E2);
            Expect_Code("adrp\tx2,#&1F000");
        }

        [Test]
        public void AArch64Dis_ldr_UnsignedOffset()
        {
            Given_Instruction(0xF947E442);
            Expect_Code("ldr\tx2,[x2,#&FC8]");
        }

        [Test]
        public void AArch64Dis_mov_reg64()
        {
            Given_Instruction(0xAA0103F4);
            Expect_Code("mov\tx20,x1");
        }

        [Test]
        public void AArch64Dis_adrp_00001()
        {
            Given_Instruction(0xB0000001);
            Expect_Code("adrp\tx1,#&1000");
        }

        [Test]
        public void AArch64Dis_mov_reg32()
        {
            Given_Instruction(0x2A0003F5);
            Expect_Code("mov\tw21,w0");
        }

        [Test]
        public void AArch64Dis_movz_imm32()
        {
            Given_Instruction(0x528000C0);
            Expect_Code("movz\tw0,#6");
        }

        [Test]
        public void AArch64Dis_ldrsw()
        {
            Given_Instruction(0xB9800033);
            Expect_Code("ldrsw\tx19,[x1]");
        }

        [Test]
        public void AArch64Dis_add_reg()
        {
            Given_Instruction(0x8B130280);
            Expect_Code("add\tx0,x20,x19");
        }

        [Test]
        public void AArch64Dis_add_reg_with_shift()
        {
            Given_Instruction(0x8B130A80);
            Expect_Code("add\tx0,x20,x19,lsl #2");
        }

        [Test]
        public void AArch64Dis_cbz()
        {
            Given_Instruction(0xB4001341);
            Expect_Code("cbz\tx1,#&100268");
        }

        [Test]
        public void AArch64Dis_ubfm_64bit()
        {
            Given_Instruction(0xD37DF29C);
            Expect_Code("ubfm\tx28,x20,#0,#&3D");
        }

        [Test]
        public void AArch64Dis_ubfm_32bit()
        {
            Given_Instruction(0x530C7C04);
            Expect_Code("ubfm\tw4,w0,#&C,#&1F");
        }

        [Test]
        public void AArch64Dis_ccmp_imm()
        {
            Given_Instruction(0xFA400B84);
            Expect_Code("ccmp\tx28,#0,#4,EQ");
        }

        [Test]
        public void AArch64Dis_str_UnsignedImmediate()
        {
            Given_Instruction(0xF9000AE0);
            Expect_Code("str\tx0,[x23,#&10]");
        }

        [Test]
        public void AArch64Dis_ret()
        {
            Given_Instruction(0xD65F03C0);
            Expect_Code("ret\tx30");
        }

        [Test]
        public void AArch64Dis_str_reg()
        {
            Given_Instruction(0xB8356B7F);
            Expect_Code("str\tw31,[x27,x21]");
        }

        [Test]
        public void AArch64Dis_nop()
        {
            Given_Instruction(0xD503201F);
            Expect_Code("nop");
        }

        [Test]
        public void AArch64Dis_str_w32()
        {
            Given_Instruction(0xB9006FA0);
            Expect_Code("str\tw0,[x29,#&6C]");
        }

        [Test]
        public void AArch64Dis_ldr()
        {
            Given_Instruction(0xF9400000);
            Expect_Code("ldr\tx0,[x0]");
        }

        [Test]
        public void AArch64Dis_subs()
        {
            Given_Instruction(0xEB13001F);
            Expect_Code("subs\tx31,x0,x19");
        }

        [Test]
        public void AArch64Dis_csinc()
        {
            Given_Instruction(0x1A9F17E0);
            Expect_Code("csinc\tw0,w31,w31,NE");
        }

        [Test]
        public void AArch64Dis_ldrb()
        {
            Given_Instruction(0x39402260);
            Expect_Code("ldrb\tw0,[x19,#&8]");
        }

        [Test]
        public void AArch64Dis_cbnz()
        {
            Given_Instruction(0x35000140);
            Expect_Code("cbnz\tw0,#&100028");
        }

        [Test]
        public void AArch64Dis_strb()
        {
            Given_Instruction(0x39002260);
            Expect_Code("strb\tw0,[x19,#&8]");
        }

        [Test]
        public void AArch64Dis_ldr_w32()
        {
            Given_Instruction(0xB9400001);
            Expect_Code("ldr\tw1,[x0]");
        }

        [Test]
        public void AArch64Dis_cbnz_negative_offset()
        {
            Given_Instruction(0x35FFFE73);
            Expect_Code("cbnz\tw19,#&FFFCC");
        }

        [Test]
        public void AArch64Dis_bne1()
        {
            Given_Instruction(0x54000401);
            Expect_Code("b.ne\t#&100080");
        }

        [Test]
        public void AArch64Dis_ldr_reg_shift()
        {
            Given_Instruction(0xF8737AA3);
            Expect_Code("ldr\tx3,[x21,x19,lsl #3]");
        }

        [Test]
        public void AArch64Dis_blr()
        {
            Given_Instruction(0xD63F0060);
            Expect_Code("blr\tx3");
        }

        [Test]
        public void AArch64Dis_bne_backward()
        {
            Given_Instruction(0x54FFFF21);
            Expect_Code("b.ne\t#&FFFE4");
        }

        [Test]
        public void AArch64Dis_stp_preindex()
        {
            Given_Instruction(0xA9B87BFD);
            Expect_Code("stp\tx29,x30,[sp,-#&80]!");
        }

        [Test]
        public void AArch64Dis_stp_w6_stack()
        {
            Given_Instruction(0xA90153F3);
            Expect_Code("stp\tx19,x20,[sp,#&10]");
        }

        [Test]
        public void AArch64Dis_ldp_w64()
        {
            Given_Instruction(0xA9446BB9);
            Expect_Code("ldp\tx25,x26,[x29,#&40]");
        }

        [Test]
        public void AArch64Dis_ldp_post()
        {
            Given_Instruction(0x6CC62B6B);
            Expect_Code("ldp\td11,d10,[x27],#&60");
        }

        [Test]
        public void AArch64Dis_ldp_post_sp()
        {
            Given_Instruction(0xA8C17BFD);
            Expect_Code("ldp\tx29,x30,[sp],#&10");
        }

        [Test]
        public void AArch64Dis_ldr_literal64()
        {
            Given_Instruction(0x580000A0);
            Expect_Code("ldr\tx0,#&100014");
        }

        [Test]
        public void AArch64Dis_ldr_64_neg_lit()
        {
            Given_Instruction(0x18FFFFE0);
            Expect_Code("ldr\tw0,#&FFFFC");
        }

        [Test]
        public void AArch64Dis_movn()
        {
            Given_Instruction(0x12800000);
            Expect_Code("movn\tw0,#0");
        }

        [Test]
        public void AArch64Dis_sbfm()
        {
            Given_Instruction(0x13017E73);
            Expect_Code("sbfm\tw19,w19,#1,#&1F");
        }

        [Test]
        public void AArch64Dis_and_reg()
        {
            Given_Instruction(0x8A140000);
            Expect_Code("and\tx0,x0,x20");
        }

        [Test]
        public void AArch64Dis_sbfm_2()
        {
            Given_Instruction(0x937D7C63);
            Expect_Code("sbfm\tx3,x3,#&3,#&20");
        }

        [Test]
        public void AArch64Dis_mul_64()
        {
            Given_Instruction(0x9B017C14);
            Expect_Code("mul\tx20,x0,x1");
        }

        [Test]
        public void AArch64Dis_madd_64()
        {
            Given_Instruction(0x9B013C14);
            Expect_Code("madd\tx20,x0,x1,x15");
        }

        [Test]
        public void AArch64Dis_ands_64_reg()
        {
            Given_Instruction(0xEA010013);
            Expect_Code("ands\tx19,x0,x1");
        }

        [Test]
        public void AArch64Dis_test_64_reg()
        {
            Given_Instruction(0xEA01001F);
            Expect_Code("test\tx0,x1");
        }



        [Test]
        public void AArch64Dis_ldurb()
        {
            Given_Instruction(0x385F9019);
            Expect_Code("ldurb\tw25,[x0,-#&7]");
        }

        [Test]
        public void AArch64Dis_38018C14()
        {
            Given_Instruction(0x38018C14);
            Expect_Code("strb\tw20,[x0,#&18]!");
        }

        [Test]
        public void AArch64Dis_strb_preidx_sp()
        {
            Given_Instruction(0x38018FFF);
            Expect_Code("strb\tw31,[sp,#&18]!");
        }

        [Test]
        public void AArch64Dis_ldrh_32_off0()
        {
            Given_Instruction(0x79400021);
            Expect_Code("ldrh\tw1,[x1]");
        }

        [Test]
        public void AArch64Dis_add_extension()
        {
            Given_Instruction(0x8B34C2D9);
            Expect_Code("add\tx25,x22,w20,sxtw #0");
        }

        [Test]
        public void AArch64Dis_madd_32()
        {
            Given_Instruction(0x1B003C21);
            Expect_Code("madd\tw1,w1,w0,w15");
        }

        [Test]
        public void AArch64Dis_mneg_32()
        {
            Given_Instruction(0x1B00FC21);
            Expect_Code("mneg\tw1,w1,w0");
        }

        [Test]
        public void AArch64Dis_msub_()
        {
            Given_Instruction(0x1B00BC21);
            Expect_Code("msub\tw1,w1,w0,w15");
        }

        [Test]
        public void AArch64Dis_strb_post_idx()
        {
            Given_Instruction(0x3800141F);
            Expect_Code("strb\tw31,[x0],#&1");
        }

        [Test]
        public void AArch64Dis_strb_stack_post()
        {
            Given_Instruction(0x380047FF);
            Expect_Code("strb\tw31,[sp],#&4");
        }

        [Test]
        public void AArch64Dis_msub_64()
        {
            Given_Instruction(0x9B038441);
            Expect_Code("msub\tx1,x2,x3,x1");
        }

        [Test]
        public void AArch64Dis_ldur_64_negative_offset()
        {
            Given_Instruction(0xF85F8260);
            Expect_Code("ldur\tx0,[x19,-#&8]");
        }

        [Test]
        public void AArch64Dis_and_reg_reg()
        {
            Given_Instruction(0x0A000020);
            Expect_Code("and\tw0,w1,w0");
        }

        [Test]
        public void AArch64Dis_strb_indexed()
        {
            Given_Instruction(0x3820483F);
            Expect_Code("strb\tw31,[x1,w0,uxtw]");
        }

        [Test]
        public void AArch64Dis_str_q0()
        {
            Given_Instruction(0x3D8027A0);
            Expect_Code("str\tq0,[x29,#&90]");
        }

        [Test]
        public void AArch64Dis_cmp_32_uxtb()
        {
            Given_Instruction(0x6B20001F);
            Expect_Code("cmp\tw0,w0,uxtb #0");
        }

        [Test]
        public void AArch64Dis_ldrb_idx()
        {
            Given_Instruction(0x38616857);
            Expect_Code("ldrb\tw23,[x2,x1]");
        }

        [Test]
        public void AArch64Dis_sbfm_0_1F()
        {
            Given_Instruction(0x93407C18);
            Expect_Code("sbfm\tx24,x0,#0,#&1F@@");
        }

        [Test]
        public void AArch64Dis_strb_index()
        {
            Given_Instruction(0x38216A63);
            Expect_Code("strb\tw3,[x19,x1]");
        }

        [Test]
        public void AArch64Dis_add_ext_reg_sxtw()
        {
            Given_Instruction(0x8B33D2D3);
            Expect_Code("add\tx19,x22,w19,sxtw #4");
        }

        [Test]
        public void AArch64Dis_ldr_64_preidx()
        {
            Given_Instruction(0xF8410E81);
            Expect_Code("ldr\tx1,[x20,#&10]!");
        }

        [Test]
        public void AArch64Dis_ldrb_post()
        {
            Given_Instruction(0x38401420);
            Expect_Code("ldrb\tw0,[x1],#&1");
        }

        [Test]
        public void AArch64Dis_strb_uxtw()
        {
            Given_Instruction(0x38344B23);
            Expect_Code("strb\tw3,[x25,w20,uxtw]");
        }

        [Test]
        public void AArch64Dis_sdiv()
        {
            Given_Instruction(0x1AC00F03);
            Expect_Code("sdiv\tw3,w24,w0");
        }


        [Test]
        public void AArch64Dis_ldrb_preidx()
        {
            Given_Instruction(0x38401C41);
            Expect_Code("ldrb\tw1,[x2,#&1]!");
        }

        [Test]
        public void AArch64Dis_ldrb_idx_uxtw()
        {
            Given_Instruction(0x38614873);
            Expect_Code("ldrb\tw19,[x3,w1,uxtw]");
        }

        [Test]
        public void AArch64Dis_ldrh_32_idx_lsl()
        {
            Given_Instruction(0x787B7B20);
            Expect_Code("ldrh\tw0,[x25,x27,lsl #1]");
        }

        [Test]
        public void AArch64Dis_ldrh_32_sxtw()
        {
            Given_Instruction(0x7876D800);
            Expect_Code("ldrh\tw0,[x0,w22,sxtw #1]");
        }

        public void AArch64Dis_3873C800()
        {
            Given_Instruction(0x3873C800);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_ldrh_idx_sxtw()
        {
            Given_Instruction(0x7874D800);
            Expect_Code("ldrh\tw0,[x0,w20,sxtw #1]");
        }

        [Test]
        public void AArch64Dis_ldrh_index_sxtw()
        {
            Given_Instruction(0x7875D800);
            Expect_Code("ldrh\tw0,[x0,w21,sxtw #1]");
        }

        [Test]
        public void AArch64Dis_movn_imm()
        {
            Given_Instruction(0x9280000A);
            Expect_Code("movn\tx10,#0");
        }

        [Test]
        public void AArch64Dis_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            Expect_Code("sturb\tw31,[x4,-#&1]");
        }

        [Test]
        public void AArch64Dis_adr()
        {
            Given_Instruction(0x10000063);
            Expect_Code("adr\tx3,#&10000C");
        }

        [Test]
        public void AArch64Dis_orn()
        {
            Given_Instruction(0x2A2200F8);
            Expect_Code("orn\tw24,w7,w2");
        }

        [Test]
        public void AArch64Dis_mvn()
        {
            Given_Instruction(0x2A2203F8);
            Expect_Code("mvn\tw24,w2");
        }

        [Test]
        public void AArch64Dis_sdiv_64()
        {
            Given_Instruction(0x9AC20C62);
            Expect_Code("sdiv\tx2,x3,x2");
        }

        [Test]
        public void AArch64Dis_eor_reg_32()
        {
            Given_Instruction(0x4A140074);
            Expect_Code("eor\tw20,w3,w20");
        }

        [Test]
        public void AArch64Dis_sub_reg_ext_64()
        {
            Given_Instruction(0xCB214F18);
            Expect_Code("sub\tx24,x24,w1,uxtw #3");
        }

        [Test]
        public void AArch64Dis_bic_reg_32()
        {
            Given_Instruction(0x0A350021);
            Expect_Code("bic\tw1,w1,w21");
        }

        [Test]
        public void AArch64Dis_umulh()
        {
            Given_Instruction(0x9BC57C00);
            Expect_Code("umulh\tx0,w0,w5");
        }

        [Test]
        public void AArch64Dis_lsrv()
        {
            Given_Instruction(0x1AC22462);
            Expect_Code("lsrv\tw2,w3,w2");
        }

        [Test]
        public void AArch64Dis_smull()
        {
            Given_Instruction(0x9B237C43);
            Expect_Code("smull\tx3,w2,w3");
        }

        [Test]
        public void AArch64Dis_smaddl()
        {
            Given_Instruction(0x9B233C43);
            Expect_Code("smaddl\tx3,w2,w3,x15");
        }

        [Test]
        public void AArch64Dis_strh_reg()
        {
            Given_Instruction(0x78206A62);
            Expect_Code("strh\tw2,[x19,x0]");
        }

        [Test]
        public void AArch64Dis_stp_r64_pre()
        {
            Given_Instruction(0x6DB73BEF);
            Expect_Code("stp\td15,d14,[sp,-#&90]!");
        }

        [Test]
        public void AArch64Dis_stp_r64()
        {
            Given_Instruction(0x6D0133ED);
            Expect_Code("stp\td13,d12,[sp,#&10]");
        }

        [Test]
        public void AArch64Dis_ldr_r64_off()
        {
            Given_Instruction(0xFD45E540);
            Expect_Code("ldr\td0,[x10,#&BC8]");
        }

        [Test]
        public void AArch64Dis_str_r64_imm()
        {
            Given_Instruction(0xFD001BE0);
            Expect_Code("str\td0,[sp,#&30]");
        }

        [Test]
        public void AArch64Dis_scvtf()
        {
            Given_Instruction(0x1E220120);
            Expect_Code("scvtf\ts0,w9");
        }

        [Test]
        public void AArch64Dis_ldp_w32()
        {
            Given_Instruction(0x296107A2);
            Expect_Code("ldp\tw2,w1,[x29,-#&F8]");
        }

        [Test]
        public void AArch64Dis_scvtf_r32()
        {
            Given_Instruction(0x5E21D82F);
            Expect_Code("scvtf\ts15,s1");
        }

        [Test]
        public void AArch64Dis_stp_r32()
        {
            Given_Instruction(0x2D010FE2);
            Expect_Code("stp\ts2,s3,[sp,#&8]");
        }

        ////////////////////////////////////////////

        [Test]
        public void AArch64Dis_fcvtms_f32_to_i32()
        {
            Given_Instruction(0x1E300003);
            Expect_Code("fcvtms\tw3,s0");
        }

        [Test]
        public void AArch64Dis_udiv_w32()
        {
            Given_Instruction(0x1ADA0908);
            Expect_Code("udiv\tw8,w8,w26");
        }

        [Test]
        public void AArch64Dis_rev16_w32()
        {
            Given_Instruction(0x5AC0056B);
            Expect_Code("rev16\tw11,w11");
        }

        [Test]
        public void AArch64Dis_add_32_ext()
        {
            Given_Instruction(0x0B20A1EF);
            Expect_Code("add\tw15,w15,w0,sxth #0");
        }

        [Test]
        public void AArch64Dis_scvtf_i32_to_f32()
        {
            Given_Instruction(0x1E6202E0);
            Expect_Code("scvtf\td0,w23");
        }

        [Test]
        public void AArch64Dis_fmov_f64_to_i64()
        {
            Given_Instruction(0x9E6701B0);
            Expect_Code("fmov\td16,x13");
        }

        [Test]
        public void AArch64Dis_sxtl()
        {
            Given_Instruction(0x0F10A673);
            Expect_Code("sxtl\tv19.4h,v19.4h");
        }

        [Test]
        public void AArch64Dis_fadd_vector_real32()
        {
            Given_Instruction(0x4E33D4D3);
            Expect_Code("fadd\tv19.4s,v6.4s,v19.4s");
        }

        [Test]
        public void AArch64Dis_fcvtzs_vector_real32()
        {
            Given_Instruction(0x4EA1BAB5);
            Expect_Code("fcvtzs\tv21.4s,v21.4s");
        }

        [Test]
        public void AArch64Dis_fmul_real32()
        {
            Given_Instruction(0x1E210B25);
            Expect_Code("fmul\ts5,s25,s1");
        }

        [Test]
        public void AArch64Dis_fcvtzs_i32_from_f32()
        {
            Given_Instruction(0x1E380069);
            Expect_Code("fcvtzs\tw3,s9");
        }

        [Test]
        public void AArch64Dis_ucvtf_real32_int32()
        {
            Given_Instruction(0x1E230101);
            Expect_Code("ucvtf\ts1,w8");
        }


        [Test]
        public void AArch64Dis_DecodeReal16Immediate_1()
        {
            var c = AArch64Disassembler.DecodeReal16FpConstant(0x70);
            Assert.AreEqual("1.0", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal16Immediate_minus1_9375()
        {
            var c = AArch64Disassembler.DecodeReal16FpConstant(0xFF);
            Assert.AreEqual("-1.9375", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal16Immediate_31()
        {
            var c = AArch64Disassembler.DecodeReal16FpConstant(0x3F);
            Assert.AreEqual("31.0", c.ToString());
        }


        [Test]
        public void AArch64Dis_DecodeReal32Immediate_1()
        {
            var c = AArch64Disassembler.DecodeReal32FpConstant(0x70);
            Assert.AreEqual("1.0F", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal32Immediate_minus1_9375()
        {
            var c = AArch64Disassembler.DecodeReal32FpConstant(0xFF);
            Assert.AreEqual("-1.9375F", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal32Immediate_31()
        {
            var c = AArch64Disassembler.DecodeReal32FpConstant(0x3F);
            Assert.AreEqual("31.0F", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal64Immediate_1()
        {
            var c = AArch64Disassembler.DecodeReal64FpConstant(0x70);
            Assert.AreEqual("1.0", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal64Immediate_minus1_9375()
        {
            var c = AArch64Disassembler.DecodeReal64FpConstant(0xFF);
            Assert.AreEqual("-1.9375", c.ToString());
        }

        [Test]
        public void AArch64Dis_DecodeReal64Immediate_31()
        {
            var c = AArch64Disassembler.DecodeReal64FpConstant(0x3F);
            Assert.AreEqual("31.0", c.ToString());
        }

        [Test]
        public void AArch64Dis_fcsel()
        {
            Given_Instruction(0x1E2B1C00);
            Expect_Code("fcsel\ts0,s0,s11,NE");
        }

        [Test]
        public void AArch64Dis_fcvtps_f32_to_i32()
        {
            Given_Instruction(0x1E280008);
            Expect_Code("fcvtps\tw8,s0");
        }

        [Test]
        public void AArch64Dis_fcmp_f32()
        {
            Given_Instruction(0x1E222060);
            Expect_Code("fcmp\ts3,s2");
        }

        [Test]
        public void AArch64Dis_fabs_f32()
        {
            Given_Instruction(0x1E20C021);
            Expect_Code("fabs\ts1,s1");
        }

        [Test]
        public void AArch64Dis_fneg_f32()
        {
            Given_Instruction(0x1E214021);
            Expect_Code("fneg\ts1,s1");
        }

        [Test]
        public void AArch64Dis_fsqrt()
        {
            Given_Instruction(0x1E21C001);
            Expect_Code("fsqrt\ts1,s0");
        }

        [Test]
        public void AArch64Dis_fmov_i32_to_f32()
        {
            Given_Instruction(0x1E2703E1);
            Expect_Code("fmov\ts1,w31");
        }

        [Test]
        public void AArch64Dis_fcvt_f32_to_f64()
        {
            Given_Instruction(0x1E22C041);
            Expect_Code("fcvt\td1,s2");
        }

        [Test]
        public void AArch64Dis_fcvt_f64_to_f32()
        {
            Given_Instruction(0x1E624000);
            Expect_Code("fcvt\ts0,d0");
        }

        [Test]
        public void AArch64Dis_mov_w128()
        {
            Given_Instruction(0x4EA91D22);
            Expect_Code("mov\tv2.16b,v9.16b");
        }

        [Test]
        public void AArch64Dis_uxtl()
        {
            Given_Instruction(0x2F08A400);
            Expect_Code("uxtl\tv0.8h,v0.8b");
        }

        [Test]
        public void AArch64Dis_xtn()
        {
            Given_Instruction(0x0E612A10);
            Expect_Code("xtn\tv16.4h,v16.4s");
        }


        [Test]
        public void AArch64Dis_fmov_f32_to_w32()
        {
            Given_Instruction(0x1E26002B);
            Expect_Code("fmov\tw11,s1");
        }

        [Test]
        public void AArch64Dis_fmov_vector_immedate()
        {
            Given_Instruction(0x4F03F600);
            Expect_Code("fmov\tv0.4s,#1.0F");
        }

        [Test]
        public void AArch64Dis_dup_element_w32()
        {
            Given_Instruction(0x4E0406E2);
            Expect_Code("dup\tv2.4s,v23.s[0]");
        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@


        // An AArch64 decoder for the instruction 4EA31C68 (681CA34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
public void AArch64Dis_4EA31C68()
        {
            Given_Instruction(0x4EA31C68);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C01 (011CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C01()
        {
            Given_Instruction(0x4EA01C01);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C02 (021CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C02()
        {
            Given_Instruction(0x4EA01C02);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C0C (0C1CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C0C()
        {
            Given_Instruction(0x4EA01C0C);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C2D (2D1CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C2D()
        {
            Given_Instruction(0x4EA11C2D);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA21C48 (481CA24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA21C48()
        {
            Given_Instruction(0x4EA21C48);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA31C69 (691CA34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA31C69()
        {
            Given_Instruction(0x4EA31C69);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C24 (241CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C24()
        {
            Given_Instruction(0x4EA11C24);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C29 (291CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C29()
        {
            Given_Instruction(0x4EA11C29);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C0A (0A1CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C0A()
        {
            Given_Instruction(0x4EA01C0A);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C20 (201CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C20()
        {
            Given_Instruction(0x4EA11C20);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA31C60 (601CA34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA31C60()
        {
            Given_Instruction(0x4EA31C60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E020653 (5306026E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E020653()
        {
            Given_Instruction(0x6E020653);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E021654 (5416026E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E021654()
        {
            Given_Instruction(0x6E021654);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E065653 (5356066E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E065653()
        {
            Given_Instruction(0x6E065653);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E066654 (5466066E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E066654()
        {
            Given_Instruction(0x6E066654);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0A2633 (33260A6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0A2633()
        {
            Given_Instruction(0x6E0A2633);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0A3634 (34360A6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0A3634()
        {
            Given_Instruction(0x6E0A3634);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0E7633 (33760E6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0E7633()
        {
            Given_Instruction(0x6E0E7633);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0E0614 (14060E6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0E0614()
        {
            Given_Instruction(0x6E0E0614);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E21DA73 (73DA214E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E21DA73()
        {
            Given_Instruction(0x4E21DA73);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E21DA94 (94DA214E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E21DA94()
        {
            Given_Instruction(0x4E21DA94);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E33DC15 (15DC336E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E33DC15()
        {
            Given_Instruction(0x6E33DC15);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E34DC36 (36DC346E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E34DC36()
        {
            Given_Instruction(0x6E34DC36);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E33DC93 (93DC336E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E33DC93()
        {
            Given_Instruction(0x6E33DC93);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E34DCB4 (B4DC346E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E34DCB4()
        {
            Given_Instruction(0x6E34DCB4);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E36D6B5 (B5D6364E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E36D6B5()
        {
            Given_Instruction(0x4E36D6B5);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E34D673 (73D6344E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E34D673()
        {
            Given_Instruction(0x4E34D673);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E35D455 (55D4354E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E35D455()
        {
            Given_Instruction(0x4E35D455);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E33D4D3 (D3D4334E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E33D4D3()
        {
            Given_Instruction(0x4E33D4D3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E35DC75 (75DC356E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E35DC75()
        {
            Given_Instruction(0x6E35DC75);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E33DCF3 (F3DC336E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E33DCF3()
        {
            Given_Instruction(0x6E33DCF3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E022653 (5326026E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E022653()
        {
            Given_Instruction(0x6E022653);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E023654 (5436026E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E023654()
        {
            Given_Instruction(0x6E023654);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E067653 (5376066E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E067653()
        {
            Given_Instruction(0x6E067653);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E060634 (3406066E) - (AdvancedSIMDcopy Q:op=11) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E060634()
        {
            Given_Instruction(0x6E060634);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0A4633 (33460A6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0A4633()
        {
            Given_Instruction(0x6E0A4633);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0A5634 (34560A6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0A5634()
        {
            Given_Instruction(0x6E0A5634);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0E1613 (13160E6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0E1613()
        {
            Given_Instruction(0x6E0E1613);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E0E2614 (14260E6E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0001) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E0E2614()
        {
            Given_Instruction(0x6E0E2614);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E21DA10 (10DA214E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E21DA10()
        {
            Given_Instruction(0x4E21DA10);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E21DA31 (31DA214E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b0100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E21DA31()
        {
            Given_Instruction(0x4E21DA31);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E30DC12 (12DC306E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E30DC12()
        {
            Given_Instruction(0x6E30DC12);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E31DC33 (33DC316E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E31DC33()
        {
            Given_Instruction(0x6E31DC33);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E30DC90 (90DC306E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E30DC90()
        {
            Given_Instruction(0x6E30DC90);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E31DCB1 (B1DC316E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E31DCB1()
        {
            Given_Instruction(0x6E31DCB1);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E33D652 (52D6334E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E33D652()
        {
            Given_Instruction(0x4E33D652);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E31D610 (10D6314E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E31D610()
        {
            Given_Instruction(0x4E31D610);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E32D451 (51D4324E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E32D451()
        {
            Given_Instruction(0x4E32D451);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E30D4D0 (D0D4304E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E30D4D0()
        {
            Given_Instruction(0x4E30D4D0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E31DC71 (71DC316E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E31DC71()
        {
            Given_Instruction(0x6E31DC71);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6E30DCF0 (F0DC306E) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=00 op2=0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_6E30DCF0()
        {
            Given_Instruction(0x6E30DCF0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA21C4A (4A1CA24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA21C4A()
        {
            Given_Instruction(0x4EA21C4A);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C0B (0B1CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C0B()
        {
            Given_Instruction(0x4EA01C0B);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA21C49 (491CA24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA21C49()
        {
            Given_Instruction(0x4EA21C49);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C2A (2A1CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C2A()
        {
            Given_Instruction(0x4EA11C2A);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C28 (281CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C28()
        {
            Given_Instruction(0x4EA11C28);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C09 (091CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C09()
        {
            Given_Instruction(0x4EA01C09);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA21C4B (4B1CA24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA21C4B()
        {
            Given_Instruction(0x4EA21C4B);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA41C88 (881CA44E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA41C88()
        {
            Given_Instruction(0x4EA41C88);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6F00E401 (01E4006F) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=2 op2=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_6F00E401()
        {
            Given_Instruction(0x6F00E401);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6F00E402 (02E4006F) - (DataProcessingScalarFpAdvancedSimd - op0=6 op1=2 op2=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_6F00E402()
        {
            Given_Instruction(0x6F00E402);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA18461 (6184A14E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA18461()
        {
            Given_Instruction(0x4EA18461);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA28482 (8284A24E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA28482()
        {
            Given_Instruction(0x4EA28482);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA18441 (4184A14E) - (AdvancedSimd3Same U=0) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA18441()
        {
            Given_Instruction(0x4EA18441);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EB1B821 (21B8B14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EB1B821()
        {
            Given_Instruction(0x4EB1B821);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA41C81 (811CA44E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA41C81()
        {
            Given_Instruction(0x4EA41C81);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA51CA4 (A41CA54E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA51CA4()
        {
            Given_Instruction(0x4EA51CA4);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA31C64 (641CA34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA31C64()
        {
            Given_Instruction(0x4EA31C64);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA51CA3 (A31CA54E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA51CA3()
        {
            Given_Instruction(0x4EA51CA3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA01C08 (081CA04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA01C08()
        {
            Given_Instruction(0x4EA01C08);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 7E21D800 (00D8217E) - (DataProcessingScalarFpAdvancedSimd - op0=7) has not been implemented yet.
        [Test]
        public void AArch64Dis_7E21D800()
        {
            Given_Instruction(0x7E21D800);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 7E21D821 (21D8217E) - (DataProcessingScalarFpAdvancedSimd - op0=7) has not been implemented yet.
        [Test]
        public void AArch64Dis_7E21D821()
        {
            Given_Instruction(0x7E21D821);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 7E21D842 (42D8217E) - (DataProcessingScalarFpAdvancedSimd - op0=7) has not been implemented yet.
        [Test]
        public void AArch64Dis_7E21D842()
        {
            Given_Instruction(0x7E21D842);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E609C20 (209C604E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E609C20()
        {
            Given_Instruction(0x4E609C20);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4E609C40 (409C604E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b00 op2=0b1100) has not been implemented yet.
        [Test]
        public void AArch64Dis_4E609C40()
        {
            Given_Instruction(0x4E609C40);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 7A43B900 (00B9437A) - (Unknown format character '*' in '* 32-bit' decoding ccmp) has not been implemented yet.
        [Test]
        public void AArch64Dis_7A43B900()
        {
            Given_Instruction(0x7A43B900);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA71CE9 (E91CA74E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA71CE9()
        {
            Given_Instruction(0x4EA71CE9);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA51CAA (AA1CA54E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA51CAA()
        {
            Given_Instruction(0x4EA51CAA);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA41C8B (8B1CA44E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA41C8B()
        {
            Given_Instruction(0x4EA41C8B);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA31C6C (6C1CA34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA31C6C()
        {
            Given_Instruction(0x4EA31C6C);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA21C4D (4D1CA24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA21C4D()
        {
            Given_Instruction(0x4EA21C4D);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA61CC0 (C01CA64E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA61CC0()
        {
            Given_Instruction(0x4EA61CC0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EB01E11 (111EB04E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EB01E11()
        {
            Given_Instruction(0x4EB01E11);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA71CF2 (F21CA74E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA71CF2()
        {
            Given_Instruction(0x4EA71CF2);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EA11C33 (331CA14E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0100 op3=xxxxxxx11) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EA11C33()
        {
            Given_Instruction(0x4EA11C33);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EB21E47 (471EB24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EB21E47()
        {
            Given_Instruction(0x4EB21E47);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EB21E40 (401EB24E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EB21E40()
        {
            Given_Instruction(0x4EB21E40);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 4EB31E67 (671EB34E) - (DataProcessingScalarFpAdvancedSimd - op0=4 op1=0b01 op2=0b0110) has not been implemented yet.
        [Test]
        public void AArch64Dis_4EB31E67()
        {
            Given_Instruction(0x4EB31E67);
            Expect_Code("@@@");
        }


    }
}