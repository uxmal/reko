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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public partial class Arm64DisassemblerTests : DisassemblerTestBase<AArch64Instruction>
    {
        private readonly IProcessorArchitecture arch;
        private readonly Address baseAddress;
        private AArch64Instruction instr;

        public Arm64DisassemblerTests()
        {
            this.arch = new Arm64Architecture(CreateServiceContainer(), "aarch64", new Dictionary<string, object>());
            this.baseAddress = Address.Ptr64(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddress;

        private void Given_Instruction(uint wInstr)
        {
            this.instr = base.DisassembleWord(wInstr);
        }

        private void Expect_Code(string sexp)
        {
            Assert.AreEqual(sexp, instr.ToString());
        }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            if (instr.ToString() != sExpected)
            {
                Assert.AreEqual(sExpected, instr.ToString());
            }
        }

        [Test]
        public void AArch64Dis_abs_vector()
        {
            AssertCode("abs\tv1.2s,v12.2s", "81B9A00E");
            AssertCode("abs\tv1.4s,v12.4s", "81B9A04E");
        }

        [Test]
        public void AArch64Dis_adrp()
        {
            Given_Instruction(0xF00000E2);
            Expect_Code("adrp\tx2,#&11F000");
        }

        [Test]
        public void AArch64Dis_and_reg()
        {
            Given_Instruction(0x8A140000);
            Expect_Code("and\tx0,x0,x20");
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
        public void AArch64Dis_and_vector()
        {
            AssertCode("and\tv0.16b,v0.16b,v2.16b", "001C224E");
        }

        [Test]
        public void AArch64Dis_ands_Xn_imm()
        {
            var instr = DisassembleBits("111 100100 0 010101 010101 00100 00111");
            Assert.AreEqual("ands\tx7,x4,#&FFFFF801", instr.ToString());
        }


        [Test]
        public void AArch64Rw_adcs_64()
        {
            Given_Instruction(0xBA020063); // 630002BA");
            Expect_Code("adcs\tx3,x3,x2");
        }

        [Test]
        public void AArch64Dis_add_32_ext()
        {
            Given_Instruction(0x0B20A1EF);
            Expect_Code("add\tw15,w15,w0,sxth #0");
        }

        [Test]
        public void AArch64Dis_add_v()
        {
            Given_Instruction(0x0EAD84E7);
            Expect_Code("add\tv7.2s,v7.2s,v13.2s");
        }

        [Test]
        public void AArch64Dis_add_Wn_imm()
        {
            var instr = DisassembleBits("000 10001 01 011111111111 10001 10011");
            Assert.AreEqual("add\tw19,w17,#&7FF,lsl #&C", instr.ToString());
        }

        [Test]
        public void AArch64Dis_add_wsp_ext()
        {
            Given_Instruction(0x0B20A3FF);
            Expect_Code("add\twsp,wsp,w0,sxth #0");
        }


        [Test]
        public void AArch64Dis_addp()
        {
            AssertCode("addp\tv31.4s,v10.4s,v12.4s", "5FBDAC4E");
        }

        [Test]
        public void AArch64Dis_addp_scalar()
        {
            AssertCode("addp\td31,v31.2d", "FFBBF15E");
        }

        [Test]
        public void AArch64Dis_adds_Xn_imm()
        {
            AssertCode("adds\tx19,x17,#&7FF", "33FE1FB1");
        }

        [Test]
        public void AArch64Dis_addv_i32()
        {
            Given_Instruction(0x4EB1B821);
            Expect_Code("addv\ts1,v1.4s");
        }

        [Test]
        public void AArch64Dis_adr()
        {
            Given_Instruction(0x10000063);
            AssertCode("adr\tx3,#&10000C", "63000010");
        }

        [Test]
        public void AArch64Dis_asr()
        {
            Given_Instruction(0x13017E73);
            Expect_Code("asr\tw19,w19,#1");
        }

        [Test]
        public void AArch64Dis_b_label()
        {
            var instr = DisassembleBits("00010111 11111111 11111111 00000000");
            Assert.AreEqual("b\t#&FFC00", instr.ToString());
        }

        [Test]
        public void AArch64Dis_bic_reg_32()
        {
            Given_Instruction(0x0A350021);
            Expect_Code("bic\tw1,w1,w21");
        }

        [Test]
        public void AArch64Dis_bic_vector_imm16()
        {
            //$TODO: fix constant
            AssertCode("bic\tv17.4h,#&B000B00,lsl #8", "71B5002F");
            AssertCode("bic\tv17.8h,#&B000B00,lsl #8", "71B5006F");
        }

        [Test]
        public void AArch64Dis_bic_vector_imm32()
        {
            AssertCode("bic\tv10.2s,#&4D0000,lsl #&10", "AA55022F");
        }

        [Test]
        public void AArch64Dis_bif()
        {
            AssertCode("bif\tv10.8b,v11.8b,v13.8b", "6A1DED2E");
        }

        [Test]
        public void AArch64Dis_bl_label()
        {
            var instr = DisassembleBits("10010111 11111111 11111111 00000000");
            Assert.AreEqual("bl\t#&FFC00", instr.ToString());
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
        public void AArch64Dis_br_Xn()
        {
            var instr = DisassembleBits("11010110 00011111 00000011 11000000");
            Assert.AreEqual("br\tx30", instr.ToString());
        }

        [Test]
        public void AArch64Dis_cbnz_negative_offset()
        {
            Given_Instruction(0x35FFFE73);
            Expect_Code("cbnz\tw19,#&FFFCC");
        }

        [Test]
        public void AArch64Dis_cbz()
        {
            Given_Instruction(0xB4001341);
            Expect_Code("cbz\tx1,#&100268");
        }

        [Test]
        public void AArch64Dis_ccmp_imm()
        {
            Given_Instruction(0xFA400B84);
            Expect_Code("ccmp\tx28,#0,#4,EQ");
        }

        [Test]
        public void AArch64Dis_ccmp_reg()
        {
            Given_Instruction(0x7A42D020);
            Expect_Code("ccmp\tw1,w2,#0,LE");
        }

        [Test]
        public void AArch64Dis_ccmp_w32()
        {
            Given_Instruction(0x7A43B900);
            Expect_Code("ccmp\tw8,#3,#0,LT");
        }

        [Test]
        public void AArch64Dis_cls()
        {
            AssertCode("cls\tv0.4h,v10.4h", "4049600E");
        }

        [Test]
        public void AArch64Dis_clz()
        {
            Given_Instruction(0x5AC0137B);
            Expect_Code("clz\tw27,w27");
        }

        [Test]
        public void AArch64Dis_clz_vector()
        {
            AssertCode("clz\tv12.4h,v12.4h", "6C49602E");
        }

        [Test]
        public void AArch64Dis_cmge_register()
        {
            AssertCode("cmge\tv17.8h,v13.8h,v30.8h", "B13D7E4E");
        }

        [Test]
        public void AArch64Dis_cmge_zero()
        {
            AssertCode("cmge\tv1.4h,v17.4h,#0", "218A602E");
        }

        [Test]
        public void AArch64Dis_cmgt_register()
        {
            AssertCode("cmgt\tv0.16b,v10.16b,v31.16b", "40353F4E");
        }

        [Test]
        public void AArch64Dis_cmp_Wn_imm()
        {
            var instr = DisassembleBits("011 10001 00 011111111111 10001 11111");
            Assert.AreEqual("cmp\tw17,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_cmp_wsp_imm()
        {
            var instr = DisassembleBits("011 10001 00 011111111111 11111 11111");
            Assert.AreEqual("cmp\twsp,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_cmtst()
        {
            AssertCode("cmtst\tv10.4s,v12.4s,v13.4s", "8A8DAD4E");
        }

        [Test]
        public void AArch64Dis_cnt()
        {
            AssertCode("cnt\tv12.16b,v13.16b", "AC59204E");
        }

        [Test]
        public void AArch64Dis_cnt_vector()
        {
            AssertCode("cnt\tv13.16b,v10.16b", "4D59204E");
        }

        [Test]
        public void AArch64Dis_csinc()
        {
            Given_Instruction(0x1A9F17E0);
            Expect_Code("csinc\tw0,w31,w31,NE");
        }

        [Test]
        public void AArch64Dis_dc_civac()
        {
            Given_Instruction(0xD50B7E20);
            Expect_Code("sys\t#&B,#7,#&E,#1,x0");
        }

        [Test]
        public void AArch64Dis_dup()
        {
            AssertCode("mov\th17,v17.h[7]", "31061E5E");
        }

        [Test]
        public void AArch64Dis_dup_element_w32()
        {
            AssertCode("dup\tv2.4s,v23.s[0]", "E206044E");
        }

        [Test]
        public void AArch64Dis_dup_element()
        {
            AssertCode("mov\ts31,v31.s[3]", "FF071C5E");
        }

        [Test]
        public void AArch64Dis_eon_reg_32()
        {
            AssertCode("eon\tw0,w0,w1", "0000214A");
        }

        [Test]
        public void AArch64Dis_eor_reg_32()
        {
            Given_Instruction(0x4A140074);
            Expect_Code("eor\tw20,w3,w20");
        }

        [Test]
        public void AArch64Dis_ext()
        {
            AssertCode("ext\tv0.8b,v0.8b,v0.8b,#0", "0000002E");
        }

        [Test]
        public void AArch64Dis_ext_vb()
        {
            AssertCode("ext\tv24.8b,v9.8b,v27.8b,#4", "38211b2e");
        }

        [Test]
        public void AArch64Dis_ext_v()
        {
            Given_Instruction(0x6E000800);
            Expect_Code("ext\tv0.16b,v0.16b,v0.16b,#1");
        }

        [Test]
        public void AArch64Dis_extr()
        {
            //$TODO: this is probably a ror
            Given_Instruction(0x93C08040);
            Expect_Code("extr\tx0,x2,x0,#&20");
        }

        [Test]
        public void AArch64Dis_ld1_multiple()
        {
            AssertCode("ld1\t{v0.16b,v1.16b},[x0]", "00A0404C");
        }

        [Test]
        public void AArch64Dis_ld1_postindex()
        {
            AssertCode("ld1\t{v25.16b-v28.16b},[x2],#&40", "5920DF4C");
        }

        [Test]
        public void AArch64Dis_ld2_indexed()
        {
            AssertCode("ld2\t{v0.b,v1.b}[9],[x0]", "0004604D");
        }

        [Test]
        public void AArch64Dis_ld2r()
        {
            AssertCode("ld2r\t{v0.4h,v1.4h},[x0]", "00C4600D");
            AssertCode("ld2r\t{v0.16b,v1.16b},[x0]", "00C0604D");
        }

        [Test]
        public void AArch64Dis_ld3_index()
        {
            AssertCode("ld3\t{v0.d-v2.d}[0],[x0]", "00A4400D");
            AssertCode("ld3\t{v0.b-v2.b}[9],[x0]", "0024404D");
        }

        [Test]
        public void AArch64Dis_ld3r()
        {
            AssertCode("ld3r\t{v0.4h-v2.4h},[x0]", "00E4400D");
        }

        [Test]
        public void AArch64Dis_ld4r()
        {
            AssertCode("ld4r\t{v0.4h-v3.4h},[x0]", "00E4600D");
        }

        [Test]
        public void AArch64Dis_ld4_indexed()
        {
            AssertCode("ld4\t{v0.b-v3.b}[9],[x0]", "0024604D");
            AssertCode("ld4\t{v0.d-v3.d}[1],[x0]", "00A4604D");
        }

        [Test]
        public void AArch64Dis_ldnp_D()
        {
            AssertCode("ldnp\td8,d25,[x10,#-&140]", "48656C6C");
        }

        [Test]
        public void AArch64Dis_ldp()
        {
            Given_Instruction(0x2D646C2F);
            Expect_Code("ldp\ts15,s27,[x1,#-&E0]");
        }

        [Test]
        public void AArch64Dis_ldr_UnsignedOffset()
        {
            Given_Instruction(0xF947E442);
            Expect_Code("ldr\tx2,[x2,#&FC8]");
        }

        [Test]
        public void AArch64Dis_ldrb()
        {
            Given_Instruction(0x39402260);
            Expect_Code("ldrb\tw0,[x19,#&8]");
        }

        [Test]
        public void AArch64Dis_ldxrb()
        {
            Given_Instruction(0x085F7C13);
            Expect_Code("ldxrb\tw19,[x0]");
        }


        [Test]
        public void AArch64Dis_mov_by_element()
        {
            AssertCode("mov\tv10.s[2],w6", "CA1C144E");
            AssertCode("mov\tv10.d[1],x6", "CA1C184E");
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
            Expect_Code("adrp\tx1,#&101000");
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
        public void AArch64Dis_ldr()
        {
            Given_Instruction(0xF9400000);
            Expect_Code("ldr\tx0,[x0]");
        }

        [Test]
        public void AArch64Dis_lsr_32bit()
        {
            Given_Instruction(0x530C7C04);
            Expect_Code("lsr\tw4,w0,#&C");
        }

        [Test]
        public void AArch64Dis_lsl_64bit()
        {
            Given_Instruction(0xD37DF29C);
            Expect_Code("lsl\tx28,x20,#3");
        }

        [Test]
        public void AArch64Dis_neg_vector()
        {
            AssertCode("neg\tv30.4h,v11.4h", "7EB9602E");
            AssertCode("neg\tv30.8h,v11.8h", "7EB9606E");
        }

        [Test]
        public void AArch64Dis_raddhn()
        {
            AssertCode("raddhn\tv12.2s,v10.2d,v12.2d", "4C41AC2E");
            AssertCode("raddhn2\tv12.4s,v10.2d,v12.2d", "4C41AC6E");
        }

        [Test]
        public void AArch64Dis_ret()
        {
            Given_Instruction(0xD65F03C0);
            Expect_Code("ret\tx30");
        }

        [Test]
        public void AArch64Dis_rev16_vector()
        {
            AssertCode("rev16\tv13.8b,v1.8b", "2D18200E");
        }

        [Test]
        public void AArch64Dis_rev32()
        {
            AssertCode("rev32\tv0.4h,v13.4h", "A009602E");
            AssertCode("rev32\tv0.8h,v13.8h", "A009606E");
        }

        [Test]
        public void AArch64Dis_rev64()
        {
            AssertCode("rev64\tv10.8b,v13.8b", "AA09200E");
            AssertCode("rev64\tv10.16b,v13.16b", "AA09204E");
        }

        [Test]
        public void AArch64Dis_rshrn()
        {
            AssertCode("rshrn\tv1.2s,v12.2d,#&E", "818D320F");
            AssertCode("rshrn2\tv1.4s,v12.2d,#&E", "818D324F");
        }

        [Test]
        public void AArch64Dis_rsubhn()
        {
            AssertCode("rsubhn\tv11.8b,v31.8h,v0.8h", "EB63202E");
            AssertCode("rsubhn2\tv11.16b,v31.8h,v0.8h", "EB63206E");
        }

        [Test]
        public void AArch64Dis_st1_indexed()
        {
            AssertCode("st1\t{v0.d}[1],[x0]", "0084004D");
        }

        [Test]
        public void AArch64Dis_st1()
        {
            AssertCode("st1\t{v4.16b},[x5]", "A470004C");
            AssertCode("st1\t{v3.8b},[x0]", "0370000C");
        }

        [Test]
        public void AArch64Dis_st1_range()
        {
            Given_Instruction(0x4C9F2000);
            Expect_Code("st1\t{v0.16b-v3.16b},[x0],#&40");
        }

        [Test]
        public void AArch64Dis_st1_multiple_byte()
        {
            AssertCode("st1\t{v0.16b-v3.16b},[x0]", "0020004C");
            AssertCode("st1\t{v0.16b},[x0]", "0070004C");
        }

        [Test]
        public void AArch64Dis_st2_index_byte()
        {
            AssertCode("st2\t{v0.b,v1.b}[9],[x0]", "0004204D");
        }

        [Test]
        public void AArch64Dis_st2_index_word64()
        {
            AssertCode("st2\t{v0.d,v1.d}[1],[x0]", "0084204D");
        }

        [Test]
        public void AArch64Dis_st3_indexed()
        {
            AssertCode("st3\t{v0.d-v2.d}[1],[x0]", "00A4004D");
        }

        [Test]
        public void AArch64Dis_st3_singlePostIdx()
        {
            Given_Instruction(0x0D89A18C);
            Expect_Code("st3\t{v12.s-v14.s}[0],[x12],x9");
        }

        [Test]
        public void AArch64Dis_st4_indexed()
        {
            AssertCode("st4\t{v0.b-v3.b}[9],[x0]", "0024204D");
        }

        [Test]
        public void AArch64Dis_st4_indexed_w64()
        {
            AssertCode("st4\t{v0.d-v3.d}[1],[x0]", "00A4204D");
        }

        [Test]
        public void AArch64Dis_str_reg()
        {
            Given_Instruction(0xB8356B7F);
            Expect_Code("str\tw31,[x27,x21]");
        }

        [Test]
        public void AArch64Dis_str_w32()
        {
            Given_Instruction(0xB9006FA0);
            Expect_Code("str\tw0,[x29,#&6C]");
        }


        [Test]
        public void AArch64Dis_sub_sp()
        {
            Given_Instruction(0xCB33601F);
            Expect_Code("sub\tsp,x0,x19");
        }

        [Test]
        public void AArch64Dis_subs()
        {
            Given_Instruction(0xEB13001F);
            Expect_Code("subs\tx31,x0,x19");
        }

        [Test]
        public void AArch64Dis_suqadd()
        {
            AssertCode("suqadd\tv0.2d,v11.2d", "6039E04E");
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
        public void AArch64Dis_ldxr()
        {
            AssertCode("ldxr\tw1,[x29]", "A1235D88");
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
        public void AArch64Dis_stp_preindex()
        {
            Given_Instruction(0xA9B87BFD);
            Expect_Code("stp\tx29,x30,[sp,#-&80]!");
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
        public void AArch64Dis_sxth()
        {
            Given_Instruction(0x93403C18);
            Expect_Code("sxth\tx24,w0");
        }

        [Test]
        public void AArch64Dis_sbfiz()
        {
            Given_Instruction(0x937D7C63);
            Expect_Code("sbfiz\tx3,x3,#3,#&20");
        }

        [Test]
        public void AArch64Dis_sbfm_3()
        {
            Given_Instruction(0x93417C00);
            Expect_Code("sbfm\tx0,x0,#1,#&1F");
        }

        [Test]
        public void AArch64Dis_sli_vector()
        {
            AssertCode("sli\tv13.8b,v10.8b,#4", "4D550C2F");
            AssertCode("sli\tv13.16b,v10.16b,#4", "4D550C6F");
        }

        [Test]
        public void AArch64Dis_sxtw()
        {
            Given_Instruction(0x93407C63);
            Expect_Code("sxtw\tx3,w3");
        }


        [Test]
        public void AArch64Dis_madd_64()
        {
            Given_Instruction(0x9B013C14);
            Expect_Code("madd\tx20,x0,x1,x15");
        }

        [Test]
        public void AArch64Dis_mla_by_element()
        {
            AssertCode("mla\tv30.2s,v12.2s,v31.s[1]", "9E01BF2F");
            AssertCode("mla\tv30.8h,v12.8h,v15.h[3]", "9E017F6F");
        }

        [Test]
        public void AArch64Dis_mla_vector()
        {
            AssertCode("mla\tv31.16b,v13.16b,v11.16b", "BF952B4E");
        }

        [Test]
        public void AArch64Dis_mls_by_element()
        {
            AssertCode("mls\tv1.8h,v1.8h,v12.h[4]", "21484C2F");
            AssertCode("mls\tv1.8h,v1.8h,v12.h[4]", "21484C6F");
            AssertCode("mls\tv11.4s,v1.4s,v30.s[2]", "2B489E6F");
        }

        [Test]
        public void AArch64Dis_mls_vector()
        {
            AssertCode("mls\tv31.8b,v17.8b,v30.8b", "3F963E2E");
        }

        [Test]
        public void AArch64Dis_mul_64()
        {
            Given_Instruction(0x9B017C14);
            Expect_Code("mul\tx20,x0,x1");
        }

        [Test]
        public void AArch64Dis_mul_by_element()
        {
            AssertCode("mul\tv31.4h,v30.4h,v12.h[1]", "DF835C0F");
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
            Expect_Code("ldurb\tw25,[x0,#-&7]");
        }

        [Test]
        public void AArch64Dis_strb_preidx()
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
            Expect_Code("ldur\tx0,[x19,#-&8]");
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
        public void AArch64Dis_cmeq()
        {
            AssertCode("cmeq\tv1.16b,v1.16b,v0.16b", "218C206E");
        }

        [Test]
        public void AArch64Dis_cmeq_zero()
        {
            AssertCode("cmeq\tv0.4s,v2.4s", "4098A04E");
        }

        [Test]
        public void AArch64Dis_cmhi_vs()
        {
            AssertCode("cmhi\tv0.4s,v7.4s,v0.4s", "E034A06E");
        }

        [Test]
        public void AArch64Dis_cmhs()
        {
            AssertCode("cmhs\tv1.16b,v1.16b,v2.16b", "213C226E");
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
        public void AArch64Dis_mvn()
        {
            Given_Instruction(0x2A2203F8);
            Expect_Code("mvn\tw24,w2");
        }

        [Test]
        public void AArch64Dis_nop()
        {
            Given_Instruction(0xD503201F);
            Expect_Code("nop");
        }

        [Test]
        public void AArch64Dis_orn()
        {
            AssertCode("orn\tw24,w7,w2", "F800222A");
        }

        [Test]
        public void AArch64Dis_orr()
        {
            AssertCode("orr\tx0,x31,#&C", "E0077EB2");
        }

        [Test]
        public void AArch64Dis_orr_vector_imm32()
        {
            AssertCode("orr\tv11.4s,#&D50000,lsl #&10", "AB56064F");
        }


        [Test]
        public void AArch64Dis_sdiv_64()
        {
            Given_Instruction(0x9AC20C62);
            Expect_Code("sdiv\tx2,x3,x2");
        }

        [Test]
        public void AArch64Dis_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            Expect_Code("sturb\tw31,[x4,#-&1]");
        }

        [Test]
        public void AArch64Dis_sub_reg_ext_64()
        {
            Given_Instruction(0xCB214F18);
            Expect_Code("sub\tx24,x24,w1,uxtw #3");
        }

        [Test]
        public void AArch64Dis_sub_Xn_imm()
        {
            var instr = DisassembleBits("110 10001 00 011111111111 10001 10011");
            Assert.AreEqual("sub\tx19,x17,#&7FF", instr.ToString());
        }

        [Test]
        public void AArch64Dis_lsrv()
        {
            Given_Instruction(0x1AC22462);
            Expect_Code("lsrv\tw2,w3,w2");
        }

        [Test]
        public void AArch64Dis_smaddl()
        {
            Given_Instruction(0x9B233C43);
            Expect_Code("smaddl\tx3,w2,w3,x15");
        }

        [Test]
        public void AArch64Dis_smaxp()
        {
            AssertCode("smaxp\tv0.4s,v11.4s,v31.4s", "60A5BF4E");
        }

        [Test]
        public void AArch64Dis_sminp()
        {
            AssertCode("sminp\tv11.4h,v17.4h,v12.4h", "2BAE6C0E");
        }

        [Test]
        public void AArch64Dis_smlal_by_element()
        {
            AssertCode("smlal\tv31.2d,v31.2s,v1.s[1]", "FF23A10F");
            AssertCode("smlal2\tv31.2d,v31.4s,v1.s[1]", "FF23A14F");
        }

        [Test]
        public void AArch64Dis_smlal_different()
        {
            AssertCode("smlal\tv13.2d,v30.2s,v30.2s", "CD83BE0E");
        }

        [Test]
        public void AArch64Dis_smlsl_by_element()
        {
            AssertCode("smlsl\tv12.2d,v11.2s,v30.s[3]", "6C69BE0F");
            AssertCode("smlsl2\tv12.2d,v11.4s,v30.s[3]", "6C69BE4F");
        }

        [Test]
        public void AArch64Dis_smlsl()
        {
            AssertCode("smlsl\tv31.8h,v30.8b,v1.8b", "DFA3210E");
            AssertCode("smlsl2\tv31.8h,v30.16b,v1.16b", "DFA3214E");
        }

        [Test]
        public void AArch64Dis_smov()
        {
            AssertCode("smov\tw6,v13.b[10]", "A62D150E");
            AssertCode("smov\tx4,v12.b[6]", "842D0D4E");
            AssertCode("smov\tx4,v31.b[5]", "E42F0B4E");
        }

        [Test]
        public void AArch64Dis_smull()
        {
            Given_Instruction(0x9B237C43);
            Expect_Code("smull\tx3,w2,w3");
        }

        [Test]
        public void AArch64Dis_smull_by_element()
        {
            AssertCode("smull\tv13.2d,v10.2s,v1.s[1]", "4DA1A10F");
            AssertCode("smull2\tv13.2d,v10.4s,v1.s[1]", "4DA1A14F");
        }

        [Test]
        public void AArch64Dis_smull_vector()
        {
            AssertCode("smull\tv12.8h,v13.8b,v1.8b", "ACC1210E");
            AssertCode("smull2\tv12.8h,v13.16b,v1.16b", "ACC1214E");
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
            Expect_Code("stp\td15,d14,[sp,#-&90]!");
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
        public void AArch64Dis_scvtf_x_to_s()
        {
            Given_Instruction(0x1E220120);
            Expect_Code("scvtf\ts0,w9");
        }

        [Test]
        public void AArch64Dis_scvtf_x_to_d()
        {
            AssertCode("scvtf\td1,x0", "0100629E");
        }

        [Test]
        public void AArch64Dis_scvtf_d_to_d()
        {
            AssertCode("scvtf\td16,d0", "10D8615E");
        }

        [Test]
        public void AArch64Dis_scvtf_vector()
        {
            AssertCode("scvtf\tv31.2d,v12.2d", "9FD9614E");
        }

        [Test]
        public void AArch64Dis_scvtf_vector_half()
        {
            AssertCode("scvtf\tv1.4h,v12.4h", "81D9790E");
            AssertCode("scvtf\tv1.8h,v12.8h", "81D9794E");
        }

        [Test]
        public void AArch64Dis_scvtf_vector_fixed_point()
        {
            AssertCode("scvtf\tv2.2s,v1.2s,#&E", "22E4320F");
        }

        [Test]
        public void AArch64Dis_ldp_w32()
        {
            Given_Instruction(0x296107A2);
            Expect_Code("ldp\tw2,w1,[x29,#-&F8]");
        }

        [Test]
        public void AArch64Dis_scvtf_r32()
        {
            Given_Instruction(0x5E21D82F);
            Expect_Code("scvtf\ts15,s1");
        }


        [Test]
        public void AArch64Dis_scvtf_i32_to_f32()
        {
            Given_Instruction(0x1E6202E0);
            Expect_Code("scvtf\td0,w23");
        }

        [Test]
        public void AArch64Dis_scvtf_i64_to_f64()
        {
            AssertCode("scvtf\td0,x0", "0000629E");
        }

        [Test]
        public void AArch64Dis_stp_r32()
        {
            Given_Instruction(0x2D010FE2);
            Expect_Code("stp\ts2,s3,[sp,#&8]");
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
        public void AArch64Dis_fcvtas_vector()
        {
            AssertCode("fcvtas\tv31.2d,v30.2d", "DFCB614E");
        }

        [Test]
        public void AArch64Dis_fcvtas_vector_half()
        {
            AssertCode("fcvtas\tv17.4h,v11.4h", "71C9790E");
        }

        [Test]
        public void AArch64Dis_fcvtau_vector()
        {
            AssertCode("fcvtau\tv30.4s,v17.4s", "3ECA216E");
        }



        [Test]
        public void AArch64Dis_fcvtms_f32_to_i32()
        {
            Given_Instruction(0x1E300003);
            Expect_Code("fcvtms\tw3,s0");
        }

        [Test]
        public void AArch64Dis_fcvtms_vector()
        {
            AssertCode("fcvtms\tv0.2d,v31.2d", "E0BB614E");
        }

        [Test]
        public void AArch64Dis_fcvtmu_vector()
        {
            AssertCode("fcvtmu\tv10.2d,v10.2d", "4AB9616E");
        }

        [Test]
        public void AArch64Dis_fcvtns_vector()
        {
            AssertCode("fcvtns\tv0.4s,v30.4s", "C0AB214E");
        }

        [Test]
        public void AArch64Dis_fcvtnu_vector_half()
        {
            AssertCode("fcvtnu\tv13.8h,v31.8h", "EDAB796E");
        }

        [Test]
        public void AArch64Dis_fcvtnu()
        {
            AssertCode("fcvtnu\tv17.4s,v17.4s", "31AA216E");
        }

        [Test]
        public void AArch64Dis_fcvtzs_vector()
        {
            AssertCode("fcvtzs\tv1.2d,v0.2d,#&2C", "01FC544F");
        }

        [Test]
        public void AArch64Dis_fcvtzu_vector()
        {
            AssertCode("fcvtzu\tv1.4s,v10.4s", "41B9A16E");
        }

        [Test]
        public void AArch64Dis_fmov_f64_to_i64()
        {
            Given_Instruction(0x9E6701B0);
            Expect_Code("fmov\td16,x13");
        }

        [Test]
        public void AArch64Dis_sqabs()
        {
            AssertCode("sqabs\tv10.8b,v31.8b", "EA7B200E");
        }

        [Test]
        public void AArch64Dis_sqadd()
        {
            AssertCode("sqadd\tv12.2s,v1.2s,v12.2s", "2C0CAC0E");
        }

        [Test]
        public void AArch64Dis_sqrdmlah_elemet()
        {
            AssertCode("sqrdmlah\tv12.8h,v11.8h,v13.h[7]", "6CD97D6F");
        }

        [Test]
        public void AArch64Dis_sqrdmlah()
        {
            AssertCode("sqrdmlah\ts11,s10,v31.s[1]", "4BD1BF7F");
        }

        [Test]
        public void AArch64Dis_sqrdmlsh_scalar_element()
        {
            AssertCode("sqrdmlsh\ts13,s11,v30.s[2]", "6DF99E7F");
        }

        [Test]
        public void AArch64Dis_sqdmlal_element()
        {
            AssertCode("sqdmlal\tv11.2d,v13.2s,v10.s[1]", "AB31AA0F");
            AssertCode("sqdmlal2\tv11.2d,v13.4s,v10.s[1]", "AB31AA4F");
        }

        [Test]
        public void AArch64Dis_sqdmlal_scalar_by_element()
        {
            AssertCode("sqdmlal\ts30,h1,v13.h[2]", "3E306D5F");
        }


        [Test]
        public void AArch64Dis_sqdmlal_vector()
        {
            AssertCode("sqdmlal\tv1.4s,v12.4h,v1.4h", "8191610E");
            AssertCode("sqdmlal2\tv1.4s,v12.8h,v1.8h", "8191614E");
        }

        [Test]
        public void AArch64Dis_sqdmlsl_by_element()
        {
            AssertCode("sqdmlsl\tv11.4s,v1.4h,v12.h[7]", "2B787C0F");
            AssertCode("sqdmlsl2\tv11.4s,v1.8h,v12.h[7]", "2B787C4F");
            AssertCode("sqdmlsl\tv10.2d,v12.2s,v12.s[1]", "8A71AC0F");
            AssertCode("sqdmlsl2\tv10.2d,v12.4s,v12.s[1]", "8A71AC4F");
        }

        [Test]
        public void AArch64Dis_sqdmlsl()
        {
            AssertCode("sqdmlsl\tv1.4s,v11.4h,v0.4h", "61B1600E");
            AssertCode("sqdmlsl2\tv1.4s,v11.8h,v0.8h", "61B1604E");
        }

        [Test]
        public void AArch64Dis_sqdmulh()
        {
            AssertCode("sqdmulh\tv31.4h,v27.4h,v2.4h", "7FB7620E");
            AssertCode("sqdmulh\tv31.8h,v27.8h,v2.8h", "7FB7624E");
        }

        [Test]
        public void AArch64Dis_sqdmulh_by_element()
        {
            AssertCode("sqdmulh\tv13.4h,v10.4h,v10.h[1]", "4DC15A0F");
            AssertCode("sqdmulh\tv13.8h,v10.8h,v10.h[1]", "4DC15A4F");
        }

        [Test]
        public void AArch64Dis_sqdmulh_scalar_by_element()
        {
            AssertCode("sqrdmulh\th11,h12,v13.h[4]", "8BD94D5F");
        }

        [Test]
        public void AArch64Dis_sqdmull_by_element()
        {
            AssertCode("sqdmull\tv30.4s,v31.4h,v11.h[3]", "FEB37B0F");
            AssertCode("sqdmull2\tv30.4s,v31.8h,v11.h[3]", "FEB37B4F");
        }

        [Test]
        public void AArch64Dis_sqdmull_scalar_by_element()
        {
            AssertCode("sqdmull\td11,s11,v11.s[3]", "6BB9AB5F");
            AssertCode("sqdmull\ts30,h30,v10.h[1]", "DEB35A5F");
        }

        [Test]
        public void AArch64Dis_sqneg()
        {
            AssertCode("sqneg\tv30.16b,v11.16b", "7E79206E");
        }

        [Test]
        public void AArch64Dis_sqrdmulh_element()
        {
            AssertCode("sqrdmulh\tv1.2s,v11.2s,v30.s[3]", "61D9BE0F");
            AssertCode("sqrdmulh\tv1.8h,v11.8h,v2.h[7]", "61D9724F");
        }

        [Test]
        public void AArch64Dis_sqrdmulh_scalar_elem()
        {
            AssertCode("sqrdmulh\th11,h30,v10.h[3]", "CBD37A5F");
        }

        [Test]
        public void AArch64Dis_sqrdmulh_vector()
        {
            AssertCode("sqrdmulh\tv11.8h,v1.8h,v0.8h", "2BB4606E");
        }

        [Test]
        public void AArch64Dis_sqrshl()
        {
            AssertCode("sqrshl\tv11.2s,v30.2s,v10.2s", "CB5FAA0E");
        }

        [Test]
        public void AArch64Dis_sqrshrn()
        {
            AssertCode("sqrshrn\tv0.2s,v10.2d,#6", "409D3A0F");
            AssertCode("sqrshrn2\tv0.4s,v10.2d,#6", "409D3A4F");
        }

        [Test]
        public void AArch64Dis_sqrshrun()
        {
            AssertCode("sqrshrun\tv12.4h,v13.4s,#9", "AC8D172F");
            AssertCode("sqrshrun\tv10.8h,v13.4s,#&B", "AA85156F");
            AssertCode("sqrshrun2\tv12.8h,v13.4s,#9", "AC8D176F");
            AssertCode("sqrshrun\tv10.4h,v13.4s,#&B", "AA85152F");
        }

        [Test]
        public void AArch64Dis_sqshl()
        {
            AssertCode("sqshl\tv12.2s,v11.2s,v31.2s", "6C4DBF0E");
            AssertCode("sqshl\tv12.4s,v11.4s,v31.4s", "6C4DBF4E");
        }

        [Test]
        public void AArch64Dis_sqlshl_imm()
        {
            AssertCode("sqshl\tv16.2d,v0.2d,#&3D", "10747D4F");
        }

        [Test]
        public void AArch64Dis_sqshlu()
        {
            AssertCode("sqshlu\tv17.2d,v30.2d,#&38", "D167786F");
            AssertCode("sqshlu\tv12.16b,v11.16b,#7", "6C650F6F");
        }

        [Test]
        public void AArch64Dis_sqshl_imm()
        {
            AssertCode("sqshl\tv11.2s,v30.2s,#&1E", "CB773E0F");
            AssertCode("sqshl\tv11.4s,v30.4s,#&1E", "CB773E4F");
        }



        [Test]
        public void AArch64Dis_sqshrn()
        {
            AssertCode("sqshrn\tv17.8b,v31.8h,#5", "F1970B0F");
            AssertCode("sqshrn2\tv17.16b,v31.8h,#5", "F1970B4F");
        }

        [Test]
        public void AArch64Dis_sqsub()
        {
            AssertCode("sqsub\tv13.2s,v13.2s,v17.2s", "AD2DB10E");
        }

        [Test]
        public void AArch64Dis_sqxtn()
        {
            AssertCode("sqxtn\tv31.8b,v10.8h", "5F49210E");
            AssertCode("sqxtn2\tv31.16b,v10.8h", "5F49214E");
        }

        [Test]
        public void AArch64Dis_sqxtun()
        {
            AssertCode("sqxtun\tv31.4h,v13.4s", "BF29612E");
            AssertCode("sqxtun2\tv31.8h,v13.4s", "BF29616E");
        }

        [Test]
        public void AArch64Dis_sub_vector()
        {
            AssertCode("sub\tv1.2s,v30.2s,v13.2s", "C187AD2E");
            AssertCode("sub\tv1.4s,v30.4s,v13.4s", "C187AD6E");
        }

        [Test]
        public void AArch64Dis_sxtl()
        {
            Given_Instruction(0x0F10A673);
            Expect_Code("sxtl\tv19.4s,v19.4h");
        }

        [Test]
        public void AArch64Dis_sxtl_v()
        {
            AssertCode("sxtl\tv16.2d,v3.2s", "70A4200F");
        }

        [Test]
        public void AArch64Dis_tbl_1r()
        {
            AssertCode("tbl\tv0.8b,{v0.16b},v0.8b", "0000000E");
        }

        [Test]
        public void AArch64Dis_tbl_4r()
        {
            AssertCode("tbl\tv12.8b,{v10.16b-v13.16b},v13.8b", "4C610D0E");
            AssertCode("tbl\tv12.16b,{v10.16b-v13.16b},v13.16b", "4C610D4E");
        }

        [Test]
        public void AArch64Dis_tbx_1r()
        {
            AssertCode("tbx\tv10.8b,{v10.16b},v31.8b", "4A111F0E");
            AssertCode("tbx\tv10.16b,{v10.16b},v31.16b", "4A111F4E");
        }

        [Test]
        public void AArch64Dis_tbx_2r()
        {
            AssertCode("tbx\tv31.8b,{v10.16b,v11.16b},v13.8b", "5F310D0E");
            AssertCode("tbx\tv31.16b,{v10.16b,v11.16b},v13.16b", "5F310D4E");
        }

        [Test]
        public void AArch64Dis_tbx_3r()
        {
            AssertCode("tbx\tv11.8b,{v10.16b-v12.16b},v10.8b", "4B510A0E");
            AssertCode("tbx\tv11.16b,{v10.16b-v12.16b},v10.16b", "4B510A4E");
        }

        [Test]
        public void AArch64Dis_tbx_4r()
        {
            AssertCode("tbx\tv10.8b,{v10.16b-v13.16b},v30.8b", "4A711E0E");
        }

        [Test]
        public void AArch64Dis_tbz()
        {
            Given_Instruction(0x36686372);
            Expect_Code("tbz\tw18,#&D,#&100C6C");
        }

        [Test]
        public void AArch64Dis_trn1()
        {
            AssertCode("trn1\tv30.8b,v11.8b,v0.8b", "7E29000E");
            AssertCode("trn1\tv30.16b,v11.16b,v0.16b", "7E29004E");
        }

        [Test]
        public void AArch64Dis_trn2()
        {
            AssertCode("trn2\tv31.4h,v1.4h,v0.4h", "3F68400E");
            AssertCode("trn2\tv31.8h,v1.8h,v0.8h", "3F68404E");
        }

        [Test]
        public void AArch64Dis_uaba()
        {
            AssertCode("uaba\tv30.8h,v17.8h,v10.8h", "3E7E6A6E");
        }

        [Test]
        public void AArch64Dis_uabal()
        {
            AssertCode("uabal\tv2.4s,v10.4h,v13.4h", "42516D2E");
            AssertCode("uabal2\tv13.4s,v10.8h,v13.8h", "4D516D6E");
        }

        [Test]
        public void AArch64Dis_uabdl()
        {
            AssertCode("uabdl\tv0.2d,v31.2s,v31.2s", "E073BF2E");
            AssertCode("uabdl2\tv0.2d,v31.4s,v31.4s", "E073BF6E");
        }

        [Test]
        public void AArch64Dis_uadalp()
        {
            AssertCode("uadalp\tv0.1d,v0.2s", "0068A02E");
            AssertCode("uadalp\tv0.2d,v0.4s", "0068A06E");
        }

        [Test]
        public void AArch64Dis_uaddl()
        {
            AssertCode("uaddl\tv1.8h,v0.8b,v12.8b", "01002C2E");
            AssertCode("uaddl2\tv1.8h,v0.16b,v12.16b", "01002C6E");
        }

        [Test]
        public void AArch64Dis_uaddlp()
        {
            AssertCode("uaddlp\tv31.2s,v12.4h", "9F29602E");
            AssertCode("uaddlp\tv31.4s,v12.8h", "9F29606E");
        }

        [Test]
        public void AArch64Dis_udiv_w32()
        {
            Given_Instruction(0x1ADA0908);
            Expect_Code("udiv\tw8,w8,w26");
        }

        [Test]
        public void AArch64Dis_umaxp()
        {
            AssertCode("umaxp\tv1.8b,v13.8b,v31.8b", "A1A53F2E");
        }

        [Test]
        public void AArch64Dis_umaxv()
        {
            AssertCode("umaxv\tb31,v31.8b", "FFAB302E");
        }

        [Test]
        public void AArch64Dis_umlal_by_element()
        {
            AssertCode("umlal\tv17.2d,v11.2s,v13.s[1]", "7121AD2F");
            AssertCode("umlal\tv11.2d,v13.2s,v13.s[3]", "AB29AD2F");
            AssertCode("umlal2\tv17.2d,v11.4s,v13.s[1]", "7121AD6F");
            AssertCode("umlal2\tv31.2d,v10.4s,v13.s[0]", "5F218D6F");
        }

        [Test]
        public void AArch64Dis_umlsl_by_element()
        {
            AssertCode("umlsl\tv17.4s,v1.8h,v10.h[4]", "31684A2F");
            AssertCode("umlsl2\tv17.4s,v1.8h,v10.h[4]", "31684A6F");
        }

        [Test]
        public void AArch64Dis_umlsl_different()
        {
            AssertCode("umlsl\tv0.2d,v10.2s,v13.2s", "40A1AD2E");
        }

        [Test]
        public void AArch64Dis_umov_w()
        {
            AssertCode("umov\tw6,v13.h[2]", "A63D0A0E");
        }

        [Test]
        public void AArch64Dis_umulh()
        {
            AssertCode("umulh\tx0,w0,w5", "007CC59B");
        }

        [Test]
        public void AArch64Dis_umull()
        {
            AssertCode("umull\tv17.4s,v13.4h,v11.h[1]", "B1A15B2F");
            AssertCode("umull2\tv17.4s,v13.8h,v11.h[1]", "B1A15B6F");
        }

        [Test]
        public void AArch64Dis_uqadd()
        {
            AssertCode("uqadd\tv1.2s,v30.2s,v31.2s", "C10FBF2E");
            AssertCode("uqadd\tv17.2d,v13.2d,v17.2d", "B10DF16E");
            AssertCode("uqadd\tv13.2d,v11.2d,v1.2d", "6D0DE16E");
        }

        [Test]
        public void AArch64Dis_uqrshrn()
        {
            AssertCode("uqrshrn\tv10.8b,v11.4h,#8", "6A9D082F");
        }

        [Test]
        public void AArch64Dis_uqshl()
        {
            AssertCode("uqshl\tv0.16b,v13.16b,#2", "A0750A6F");
            AssertCode("uqshl\tv11.2d,v31.2d,#&21", "EB77616F");
        }

        [Test]
        public void AArch64Dis_uqshl_imm()
        {
            AssertCode("uqshl\tv13.2s,v0.2s,#&12", "0D74322F");
        }

        [Test]
        public void AArch64Dis_uqshrn()
        {
            AssertCode("uqshrn2\tv1.8b,v13.4h,#1", "A1950F6F");
        }

        [Test]
        public void AArch64Dis_uqsub()
        {
            AssertCode("uqsub\tv13.2d,v10.2d,v17.2d", "4D2DF16E");
        }

        [Test]
        public void AArch64Dis_uqxtn()
        {
            AssertCode("uqxtn\tv13.8b,v10.4h", "4D49212E");
            AssertCode("uqxtn2\tv13.8b,v10.4h", "4D49216E");
        }

        [Test]
        public void AArch64Dis_ushl()
        {
            AssertCode("ushl\tv12.2d,v13.2d,v11.2d", "AC45EB6E");
        }

        [Test]
        public void AArch64Dis_ushll()
        {
            AssertCode("ushll\tv30.2d,v12.2s,#&18", "9EA5382F");
            AssertCode("ushll\tv13.2d,v17.2s,#4", "2DA6242F");
            AssertCode("ushll2\tv13.2d,v17.4s,#4", "2DA6246F");
        }

        [Test]
        public void AArch64Dis_usqadd()
        {
            AssertCode("usqadd\tv12.8h,v13.8h", "AC39606E");
        }

        [Test]
        public void AArch64Dis_usra()
        {
            AssertCode("usra\tv13.8b,v13.8b,#1", "AD150F2F");
            AssertCode("usra\tv13.4h,v13.4h,#1", "AD151F2F");
            AssertCode("usra\tv13.2s,v13.2s,#&11", "AD152F2F");
            AssertCode("usra\tv13.4s,v13.4s,#1", "AD153F6F");
        }

        [Test]
        public void AArch64Dis_usubw()
        {
            AssertCode("usubw\tv13.2d,v10.2d,v10.2s", "4D31AA2E");
            AssertCode("usubw2\tv13.2d,v10.2d,v10.4s", "4D31AA6E");
        }

        [Test]
        public void AArch64Dis_uzp1()
        {
            AssertCode("uzp1\tv0.2s,v30.2s,v31.2s", "C01B9F0E");
            AssertCode("uzp1\tv0.4s,v30.4s,v31.4s", "C01B9F4E");
        }

        [Test]
        public void AArch64Dis_uzp2()
        {
            AssertCode("uzp2\tv1.2s,v11.2s,v13.2s", "61598D0E");
            AssertCode("uzp2\tv1.4s,v11.4s,v13.4s", "61598D4E");
            AssertCode("uzp2\tv17.8h,v30.8h,v12.8h", "D15B4C4E");
        }

        [Test]
        public void AArch64Dis_fadd_vector_real32()
        {
            Given_Instruction(0x4E33D4D3);
            Expect_Code("fadd\tv19.4s,v6.4s,v19.4s");
        }

        [Test]
        public void AArch64Dis_fcvtzs_d_to_w()
        {
            AssertCode("fcvtzs\tw1,d0", "0100781E");
        }

        [Test]
        public void AArch64Dis_fcvtzs_vector_real32()
        {
            Given_Instruction(0x4EA1BAB5);
            Expect_Code("fcvtzs\tv21.4s,v21.4s");
        }

        [Test]
        public void AArch64Dis_fcvtzu_scalar()
        {
            AssertCode("fcvtzu\td31,d31", "FFFF7F7F");
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
        public void AArch64Dis_fcvtzs_i32_from_f64()
        {
            AssertCode("fcvtzs\tw8,d8", "0801789E");
        }

        [Test]
        public void AArch64Dis_fcvtzs_i32_from_f64_2()
        {
            AssertCode("fcvtzs\tw15,d0", "0F00781E");
        }

        [Test]
        public void AArch64Dis_ucvtf_real32_int32()
        {
            Given_Instruction(0x1E230101);
            Expect_Code("ucvtf\ts1,w8");
        }

        [Test]
        public void AArch64Dis_ucvtf_vector()
        {
            AssertCode("ucvtf\tv13.2s,v12.2s", "8DD9212E");
            AssertCode("ucvtf\tv13.4s,v12.4s", "8DD9216E");
        }

        [Test]
        public void AArch64Dis_ucvtf_vector_fixed()
        {
            AssertCode("ucvtf\tv10.2d,v10.2d,#&13", "4AE56D6F");
        }

        [Test]
        public void AArch64Dis_ucvtf_vector_fixed_point()
        {
            AssertCode("ucvtf\tv30.8h,v11.8h,#8", "7EE5186F");
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
        public void AArch64Dis_fnmadd_scalar_by_element()
        {
            AssertCode("fnmadd\td30,d1,d13,d12", "3E306D1F");
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
        public void AArch64Dis_fmov_f64_to_d64()
        {
            AssertCode("fmov\tx8,d8", "0801669E");
        }

        [Test]
        public void AArch64Rw_fmov_vector_reg_to_gp_reg()
        {
            AssertCode("fmov\tx1,d30", "C103669E");
        }

        [Test]
        public void AArch64Dis_urecpe()
        {
            AssertCode("urecpe\tv11.4s,v1.4s", "2BC8A14E");
        }

        [Test]
        public void AArch64Dis_urhadd()
        {
            AssertCode("urhadd\tv0.4s,v0.4s,v12.4s", "0014AC6E");
        }

        [Test]
        public void AArch64Dis_urshl()
        {
            AssertCode("urshl\tv0.4s,v0.4s,v0.4s", "0054A06E");
        }


        [Test]
        public void AArch64Dis_urshr()
        {
            AssertCode("urshr\tv17.8h,v13.8h,#2", "B1251E6F");
        }

        [Test]
        public void AArch64Dis_ursra()
        {
            AssertCode("ursra\tv12.4h,v1.4h,#8", "2C34182F");
        }

        [Test]
        public void AArch64Dis_ushr()
        {
            AssertCode("ushr\tv5.4s,v0.4s,#1", "05043F6F");
        }

        [Test]
        public void AArch64Dis_uxtl()
        {
            Given_Instruction(0x2F08A400);
            Expect_Code("uxtl\tv0.8h,v0.8b");
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
            AssertCode("fmov\tv0.4s,#1.0F", "00F6034F");
        }

        [Test]
        public void AArch64Dis_fmov_vector_element()
        {
            AssertCode("fmov\tx1,v0.d[1]", "0100AE9E");
        }

        [Test]
        public void AArch64Dis_fadd_vector_f32()
        {
            Given_Instruction(0x4E30D4D0);
            Expect_Code("fadd\tv16.4s,v6.4s,v16.4s");
        }


        [Test]
        public void AArch64Dis_mul_vector_i16()
        {
            Given_Instruction(0x4E609C20);
            Expect_Code("mul\tv0.8h,v1.8h,v0.8h");
        }

        [Test]
        public void AArch64Dis_mov_vector_element_i16()
        {
            Given_Instruction(0x6E0A5633);
            Expect_Code("mov\tv19.h[2],v17.h[5]");
        }

        [Test]
        public void AArch64Dis_mov_vector_element_to_w()
        {
            AssertCode("mov\tw0,v0.s[0]", "003C040E");
        }

        [Test]
        public void AArch64Dis_mov_vector_element_to_x()
        {
            AssertCode("mov\tx16,v31.d[1]", "F03F184E");
        }

        [Test]
        public void AArch64Dis_mov_velement_to_velement()
        {
            AssertCode("mov\tv0.s[3],v3.s[0]", "60041C6E");
        }

        [Test]
        public void AArch64Dis_mov_w128()
        {
            Given_Instruction(0x4EA91D22);
            Expect_Code("mov\tv2.16b,v9.16b");
        }

        [Test]
        public void AArch64Dis_mov_vec_idx()
        {
            AssertCode("mov\th17,v17.h[7]", "31061E5E");
        }

        [Test]
        public void AArch64Dis_movk_imm()
        {
            var instr = DisassembleBits("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
            Assert.AreEqual("movk\tx7,#&AAA4", instr.ToString());
        }


        [Test]
        public void AArch64Dis_mvni_ones()
        {
            AssertCode("mvni\tv13.4s,#&A1,msl #&10", "2DD4056F");
        }

        [Test]
        public void AArch64Dis_mvni_vector_16imm()
        {
            //$TODO: simplify consttant to 0062
            AssertCode("mvni\tv13.8h,#&620062", "4D84036F");
        }

        [Test]
        public void AArch64Dis_saba()
        {
            AssertCode("saba\tv17.16b,v11.16b,v10.16b", "717D2A4E");
        }

        [Test]
        public void AArch64Dis_sabal()
        {
            AssertCode("sabal\tv12.2d,v12.2s,v12.2s", "8C51AC0E");
            AssertCode("sabal2\tv12.2d,v12.2s,v12.2s", "8C51AC4E");
        }

        [Test]
        public void AArch64Dis_sabd()
        {
            AssertCode("sabd\tv12.4h,v30.4h,v1.4h", "CC77610E");
            AssertCode("sabd\tv12.8h,v30.8h,v1.8h", "CC77614E");
        }

        [Test]
        public void AArch64Dis_sabdl()
        {
            AssertCode("sabdl\tv11.4s,v31.4h,v1.4h", "EB73610E");
            AssertCode("sabdl2\tv11.4s,v31.8h,v1.8h", "EB73614E");
        }

        [Test]
        public void AArch64Dis_sadalp()
        {
            AssertCode("sadalp\tv31.1d,v30.2s", "DF6BA00E");
        }

        [Test]
        public void AArch64Dis_saddl()
        {
            AssertCode("saddl\tv5.2d,v0.2s,v31.2s", "0500BF0E");
            AssertCode("saddl2\tv5.2d,v0.4s,v31.4s", "0500BF4E");
        }

        [Test]
        public void AArch64Dis_saddlp()
        {
            AssertCode("saddlp\tv17.1d,v10.2s", "5129A00E");
            AssertCode("saddlp\tv17.2d,v10.4s", "5129A04E");
        }

        [Test]
        public void AArch64Dis_saddw()
        {
            AssertCode("saddw\tv1.8h,v0.8h,v17.8b", "0110310E");
            AssertCode("saddw2\tv1.8h,v0.8h,v17.16b", "0110314E");
        }

        [Test]
        public void AArch64Dis_scvtf_vector_i32()
        {
            Given_Instruction(0x4E21DA10);
            Expect_Code("scvtf\tv16.4s,v16.4s");
        }



        [Test]
        public void AArch64Dis_add_vector_i32()
        {
            Given_Instruction(0x4EA28482);
            Expect_Code("add\tv2.4s,v4.4s,v2.4s");
        }

        [Test]
        public void AArch64Dis_fmul_vector_f32()
        {
            Given_Instruction(0x6E30DC90);
            Expect_Code("fmul\tv16.4s,v4.4s,v16.4s");
        }

        [Test]
        public void AArch64Dis_mov_0()
        {
            Given_Instruction(0x6F00E401);
            Expect_Code("movi\tv1.2d,#0");
        }

        [Test]
        public void AArch64Dis_ucvtf_f32()
        {
            Given_Instruction(0x7E21D821);
            Expect_Code("ucvtf\ts1,s1");
        }

        [Test]
        public void AArch64Dis_add_with_extension()
        {
            Given_Instruction(0x8B3F63E0);	// add	x0,sp,x31,uxtx #0
            Expect_Code("add\tx0,sp,x31");
        }

        [Test]
        public void AArch64Dis_fmadd_f32()
        {
            Given_Instruction(0x1F0000B9);
            Expect_Code("fmadd\ts25,s5,s0,s0");
        }

        [Test]
        public void AArch64Dis_fmsub_f32()
        {
            Given_Instruction(0x1F00A08B);
            Expect_Code("fmsub\ts11,s4,s0,s8");
        }

        [Test]
        public void AArch64Dis_fmadd_f16()
        {
            Given_Instruction(0x1FD61F00);
            Expect_Code("fmadd\th0,h24,h22,h7");
        }

        [Test]
        public void AArch64Dis_pmul()
        {
            AssertCode("pmul\tv1.8b,v0.8b,v13.8b", "019C2D2E");
            AssertCode("pmul\tv17.8b,v17.8b,v31.8b", "319E3F2E");
            AssertCode("pmul\tv1.16b,v0.16b,v30.16b", "019C3E6E");
            AssertCode("pmul\tv1.16b,v0.16b,v13.16b", "019C2D6E");
        }

        [Test]
        public void AArch64Dis_pmull()
        {
            AssertCode("pmull\tv5.1q,v0.1d,v5.1d", "05E0E50E");
            AssertCode("pmull2\tv5.1q,v0.2d,v5.2d", "05E0E54E");
        }

        [Test]
        public void AArch64Dis_prfm_imm()
        {
            Given_Instruction(0xD8545280);
            Expect_Code("prfm\t#0,#&1A8A50");
        }

        [Test]
        public void AArch64Dis_prfm_reg()
        {
            Given_Instruction(0xF9800020);
            Expect_Code("prfm\t#0,[x1]");
        }

        [Test]
        public void AArch64Dis_fnmadd()
        {
            Given_Instruction(0x1F2003D5);
            Expect_Code("fnmadd\ts21,s30,s0,s0");
        }

        [Test]
        public void AArch64Dis_sbcs()
        {
            Given_Instruction(0xFA01001F);
            Expect_Code("sbcs\tx31,x0,x1");
        }

        [Test]
        public void AArch64Dis_bics_64()
        {
            Given_Instruction(0xEA2202DF);
            Expect_Code("bics\tx31,x22,x2");
        }

        [Test]
        public void AArch64Dis_bit_v()
        {
            AssertCode("bit\tv0.16b,v3.16b,v1.16b", "601CA16E");
        }

        [Test]
        public void AArch64Dis_mrs()
        {
            Given_Instruction(0xD538D081);
            Expect_Code("mrs\tx1,TPIDR_EL1");
        }

        [Test]
        public void AArch64Dis_msr_imm()
        {
            Given_Instruction(0xD50342DF);
            Expect_Code("msr\tpstate,#2");
        }

        [Test]
        public void AArch64Dis_dmb()
        {
            Given_Instruction(0xD50339BF);
            Expect_Code("dmb\tishld");
        }

        [Test]
        public void AArch64Dis_dsb()
        {
            Given_Instruction(0xD5033F9F);
            Expect_Code("dsb\tsy");
        }

        [Test]
        public void AArch64Dis_isb()
        {
            Given_Instruction(0xD5033FDF);
            Expect_Code("isb\tsy");
        }

        [Test]
        public void AArch64Dis_msr_reg()
        {
            Given_Instruction(0xD518CC20);
            Expect_Code("msr\tsysreg3_0_12_12_1,x0");
        }


        [Test]
        public void AArch64Dis_hint()
        {
            Given_Instruction(0xD503229F);
            Expect_Code("hint\t#&14");
        }

        [Test]
        public void AArch64Dis_rbit()
        {
            Given_Instruction(0x5AC0035B);
            Expect_Code("rbit\tw27,w26");
        }

        [Test]
        public void AArch64Dis_rbit_vector()
        {
            AssertCode("rbit\tv1.8b,v13.8b", "A159602E");
        }

        [Test]
        public void AArch64Dis_bic_reg_64()
        {
            Given_Instruction(0x8A3A0273);
            Expect_Code("bic\tx19,x19,x26");
        }

        [Test]
        public void AArch64Dis_brk()
        {
            Given_Instruction(0xD4210000);
            Expect_Code("brk\t#&800");
        }

        [Test]
        public void AArch64Dis_bsl()
        {
            AssertCode("bsl\tv4.16b,v3.16b,v5.16b", "641C656E");
        }

        [Test]
        public void AArch64Dis_eret()
        {
            Given_Instruction(0xD69F03E0);
            Expect_Code("eret");
        }

        [Test]
        public void AArch64Dis_eor_v()
        {
            Given_Instruction(0x6E291D4A);
            Expect_Code("eor\tv10.16b,v10.16b,v9.16b");
        }

        [Test]
        public void AArch64Dis_eor_v2()
        {
            Given_Instruction(0x2E231C03);
            Expect_Code("eor\tv3.8b,v0.8b,v3.8b");
        }

        [Test]
        public void AArch64Dis_ldaxr_64()
        {
            Given_Instruction(0xC85FFE80);
            Expect_Code("ldaxr\tx0,[x20]");
        }

        [Test]
        public void AArch64Dis_ldxr_64()
        {
            Given_Instruction(0xC85F7C81);
            Expect_Code("ldxr\tx1,[x4]");
        }

        [Test]
        public void AArch64Dis_ldxr_32()
        {
            AssertCode("ldxr\tw2,[x0]", "027C5F88");
        }

        [Test]
        public void AArch64Dis_rev_reg64()
        {
            Given_Instruction(0xDAC00C63);
            Expect_Code("rev\tx3,x3");
        }

        [Test]
        public void AArch64Dis_rev16_w32()
        {
            Given_Instruction(0x5AC0056B);
            Expect_Code("rev16\tw11,w11");
        }


        [Test]
        public void AArch64Dis_sha1h()
        {
            Given_Instruction(0x5E28098D);
            Expect_Code("sha1h\ts13,s12");
        }

        [Test]
        public void AArch64Dis_sha1c_q()
        {
            Given_Instruction(0x5E0501CC);
            Expect_Code("sha1c\tq12,s14,v5.4s");
        }

        [Test]
        public void AArch64Dis_shl_vector()
        {
            AssertCode("shl\tv11.2s,v10.2s,#7", "4B55270F");
            AssertCode("shl\tv11.4s,v10.4s,#7", "4B55274F");
        }

        [Test]
        public void AArch64Dis_shll()
        {
            AssertCode("shll\tv13.2d,v10.2s,#&20", "4D39A12E");
            AssertCode("shll2\tv13.2d,v10.4s,#&20", "4D39A16E");
        }

        [Test]
        public void AArch64Dis_shrn_i8()
        {
            Given_Instruction(0x0F0E8463);
            Expect_Code("shrn\tv3.8b,v3.8h,#2");
        }

        [Test]
        public void AArch64Dis_shsub()
        {
            AssertCode("shsub\tv31.4s,v17.4s,v0.4s", "3F26A04E");
        }

        // Reko: a decoder for AArch64 instruction 5E106271 at address 00000000000239B4 has not been implemented. (DataProcessingScalarFpAdvancedSimd - op0=5 op1=0b00 op2=???)
        [Test]
        public void AArch64Dis_sha256su1()
        {
            Given_Instruction(0x5E106271);
            Expect_Code("sha256su1\tv17.4s,v19.4s,v16.4s");
        }

        [Test]
        public void AArch64Dis_aesd()
        {
            Given_Instruction(0x4E285B03);
            Expect_Code("aesd\tv3.16b,v24.16b");
        }

        [Test]
        public void AArch64Dis_aese()
        {
            Given_Instruction(0x4E284A44);
            Expect_Code("aese\tv4.16b,v18.16b");

        }

        [Test]
        public void AArch64Dis_add_vd()
        {
            Given_Instruction(0x4EE48485);
            Expect_Code("add\tv5.2d,v4.2d,v4.2d");
        }

        [Test]
        public void AArch64Dis_movi_vs()
        {
            Given_Instruction(0x0F020508);
            Expect_Code("movi\tv8.2s,#&48");
        }

        [Test]
        public void AArch64Dis_movi_vs_v2()
        {
            AssertCode("movi\tv1.4s,#4", "8104004F");
        }

        [Test]
        public void AArch64Dis_movi_vb()
        {
            AssertCode("movi\tv2.16b,#&E0E0E0E0", "02E4074F");
        }

        [Test]
        public void AArch64Dis_movi_vs_shift()
        {
            AssertCode("movi\tv1.2s,#&800000,lsl #&10", "0144040F");
        }

        [Test]
        public void AArch64Dis_fnmsub()
        {
            Given_Instruction(0x1F74DDE8);
            Expect_Code("fnmsub\td8,d15,d20,d23");
            AssertCode("fnmsub\td11,d30,d26,d20", "CBD37A1F");
        }

        [Test]
        public void AArch64Dis_crc32x()
        {
            Given_Instruction(0x9AC54C00);
            Expect_Code("crc32x\tw0,w0,x5");
        }

        [Test]
        public void AArch64Dis_crc32b()
        {
            Given_Instruction(0x1AC14000);
            Expect_Code("crc32b\tw0,w0,w1");
        }

        [Test]
        public void AArch64Dis_ror_reg32()
        {
            //$TODO: should be 'ror' according to ARM AArch64 manual == the w0,w0 is an alias
            Given_Instruction(0x13801C02);
            Expect_Code("extr\tw2,w0,w0,#7");
        }

        [Test]
        public void AArch64Dis_stlxr()
        {
            Given_Instruction(0x8814FE62);
            Expect_Code("stlxr\tw20,w2,[x19]");
        }

        [Test]
        public void AArch64Dis_shadd()
        {
            AssertCode("shadd\tv17.2s,v31.2s,v1.2s", "F107A10E");
            AssertCode("shadd\tv17.4s,v31.4s,v1.4s", "F107A14E");
        }

        [Test]
        public void AArch64Dis_srhadd()
        {
            AssertCode("srhadd\tv17.2s,v13.2s,v17.2s", "B115B10E");
            AssertCode("srhadd\tv17.4s,v13.4s,v17.4s", "B115B14E");
        }

        [Test]
        public void AArch64Dis_sri()
        {
            AssertCode("sri\tv12.8b,v12.8b,#4", "8C450C2F");
        }

        [Test]
        public void AArch64Dis_srshl()
        {
            AssertCode("srshl\tv0.16b,v10.16b,v0.16b", "4055204E");
        }

        [Test]
        public void AArch64Dis_srshr_imm()
        {
            AssertCode("srshr\tv11.8b,v10.8b,#5", "4B250B0F");
        }

        [Test]
        public void AArch64Dis_srsra()
        {
            AssertCode("srsra\tv30.2s,v17.2s,#&15", "3E362B0F");
            AssertCode("srsra\tv30.4s,v17.4s,#&15", "3E362B4F");
            AssertCode("srsra\tv11.2d,v30.2d,#&19", "CB37674F");
        }

        [Test]
        public void AArch64Dis_str_UnsignedImmediate()
        {
            Given_Instruction(0xF9000AE0);
            Expect_Code("str\tx0,[x23,#&10]");
        }



        [Test]
        public void AArch64Dis_stxr()
        {
            Given_Instruction(0x88037CA4);
            Expect_Code("stxr\tw3,w4,[x5]");
        }

        [Test]
        public void AArch64Dis_strxb()
        {
            Given_Instruction(0x08007E98);
            Expect_Code("stxrb\tw0,w24,[x20]");
        }

        [Test]
        public void AArch64Dis_stnp()
        {
            Given_Instruction(0xA81A664B);
            Expect_Code("stnp\tx11,x25,[x18,#&1A0]");
        }

        [Test]
        public void AArch64Dis_stnp_d()
        {
            AssertCode("stnp\td0,d0,[x0]", "0000006C");
        }

        [Test]
        public void AArch64Dis_stnp_s()
        {
            AssertCode("stnp\ts0,s0,[x0]", "0000002C");
        }

        [Test]
        public void AArch64Dis_crc32ch()
        {
            Given_Instruction(0x1AC25463);
            Expect_Code("crc32ch\tw3,w3,w2");
        }

        [Test]
        public void AArch64Dis_ldarh()
        {
            Given_Instruction(0x48DFFC33);
            Expect_Code("ldarh\tw19,[x1]");
        }

        [Test]
        public void AArch64Dis_ldarxh()
        {
            Given_Instruction(0x485FFC62);
            Expect_Code("ldaxrh\tw2,[x3]");
        }

        [Test]
        public void AArch64Dis_scvtf_vector_fixed()
        {
            AssertCode("scvtf\tv0.2d,v30.2d,#&22", "C0E75E4F");
        }

        [Test]
        public void AArch64Dis_strlh()
        {
            Given_Instruction(0x489FFC18);
            Expect_Code("stlrh\tw24,[x0]");
        }

        [Test]
        public void AArch64Dis_stxrh()
        {
            AssertCode("stxrh\tw0,w5,[x0]", "05000048");
        }

        [Test]
        public void AArch64Dis_sshl()
        {
            AssertCode("sshl\tv1.8h,v12.8h,v17.8h", "8145714E");
        }

        [Test]
        public void AArch64Dis_sshll()
        {
            AssertCode("sshll\tv13.8h,v13.8b,#4", "ADA50C0F");
            AssertCode("sshll2\tv13.8h,v13.16b,#4", "ADA50C4F");
        }

        [Test]
        public void AArch64Dis_sshr_v()
        {
            Given_Instruction(0x4F09044A);
            Expect_Code("sshr\tv10.16b,v2.16b,#7");
        }

        [Test]
        public void AArch64Dis_ssra()
        {
            AssertCode("ssra\tv1.2s,v0.2s,#&15", "01142B0F");
            AssertCode("ssra\tv1.4s,v0.4s,#&15", "01142B4F");
        }

        [Test]
        public void AArch64Dis_ssubw()
        {
            AssertCode("ssubw\tv30.4s,v17.4s,v12.4h", "3E326C0E");
            AssertCode("ssubw2\tv30.4s,v17.4s,v12.8h", "3E326C4E");
        }

        [Test]
        public void AArch64Rw_ssubl()
        {
            AssertCode("ssubl\tv0.8h,v17.8b,v17.8b", "2022310E");
            AssertCode("ssubl2\tv0.8h,v17.16b,v17.16b", "2022314E");
        }

        [Test]
        public void AArch64Dis_subhn()
        {
            AssertCode("subhn\tv0.8b,v12.8h,v0.8h", "8061200E");
            AssertCode("subhn2\tv0.16b,v12.8h,v0.8h", "8061204E");
        }

        [Test]
        public void AArch64Dis_subs_Wn_imm()
        {
            var instr = DisassembleBits("011 10001 00 011111111111 10001 10011");
            Assert.AreEqual("subs\tw19,w17,#&7FF", instr.ToString());
        }


        [Test]
        public void AArch64Dis_ccmn()
        {
            Given_Instruction(0x3A4D09C0);
            Expect_Code("ccmn\tw14,#&D,#0,EQ");
        }

        [Test]
        public void AArch64Dis_ccmp_reg2()
        {
            Given_Instruction(0xFA482002);
            Expect_Code("ccmp\tx0,x8,#2,HS");
        }

        [Test]
        public void AArch64Dis_uabd()
        {
            AssertCode("uabd\tv0.8h,v24.8h,v9.8h", "0077696E");
        }

        [Test]
        public void AArch64Dis_GitHub_898()
        {
            AssertCode("umull\tx2,w2,w14", "427CAE9B");
        }

        [Test]
        public void AArch64Dis_ld1_indexed()
        {
            AssertCode("ld1\t{v0.d}[0],[x0]", "0084400D");
        }

        [Test]
        public void AArch64Dis_ld1_multireg()
        {
            AssertCode("ld1\t{v1.4s},[x2],#&10", "4178DF4C");
        }

        [Test]
        public void AArch64Dis_ld1_q()
        {
            AssertCode("ld1\t{v1.4s},[x2],#&10", "4178DF4C");
        }

        [Test]
        public void AArch64Dis_ld1_4s_post()
        {
            AssertCode("ld1\t{v0.4s},[x1],#&10", "2078DF4C");
        }

        [Test]
        public void AArch64Dis_ld1_d()
        {
            AssertCode("ld1\t{v1.2s},[x2],#&8", "4178DF0C");
        }

        [Test]
        public void AArch64Dis_ld1r_i8()
        {
            Given_Instruction(0x4D40C220);
            Expect_Code("ld1r\t{v0.16b},[x17]");
        }

        [Test]
        public void AArch64Dis_mov_simd()
        {
            AssertCode("mov\tv0.d[1],v0.d[0]", "0004186E");
        }

        [Test]
        public void AArch64Dis_fmov()
        {
            AssertCode("fmov\tx3,v0.d[1]", "0300AE9E");
        }

        [Test]
        public void AArch64Dis_fdiv()
        {
            AssertCode("fdiv\tv0.4s,v0.4s,v1.4s", "00FC216E");
        }

        [Test]
        public void AArch64Dis_st4_post()
        {
            AssertCode("st4\t{v0.8h-v3.8h},[x11],#&40", "60059F4C");
        }

        [Test]
        public void AArch64Dis_uminv()
        {
            AssertCode("uminv\tb0,v13.8b", "A0A9312E");
            AssertCode("uminv\tb31,v31.16b", "FFAB316E");
        }

        [Test]
        public void AArch64Dis_umov()
        {
            Given_Instruction(0x0E013C06);
            Expect_Code("umov\tw6,v0.b[0]");
        }

        [Test]
        public void AArch64Dis_urqsrte()
        {
            AssertCode("ursqrte\tv12.2s,v10.2s", "4CC9A12E");
            AssertCode("ursqrte\tv12.4s,v10.4s", "4CC9A16E");
        }

        [Test]
        public void AArch64Dis_xtn()
        {
            AssertCode("xtn\tv16.4h,v16.4s", "102A610E");
        }

        [Test]
        public void AArch64Dis_xtn2()
        {
            AssertCode("xtn2\tv10.16b,v10.8h", "4A29214E");
        }

        [Test]
        public void AArch64Dis_zip1()
        {
            AssertCode("zip1\tv10.4h,v12.4h,v13.4h", "8A394D0E");
            AssertCode("zip1\tv10.2d,v12.2d,v13.2d", "8A39CD4E");
        }

        [Test]
        public void AArch64Dis_zip2()
        {
            AssertCode("zip2\tv10.8b,v30.8b,v11.8b", "CA7B0B0E");
            AssertCode("zip2\tv10.16b,v30.16b,v11.16b", "CA7B0B4E");
        }


 

        /*
         * //$BORED: amuse yourself by making these tests pass.


        [Test]
        public void AArch64Dis_prfm_pld1keep()
        {
            Given_Instruction(0xF8A06B20);
            Expect_Code("prfm\tpld1keep,[x25,x0]");
        }

        [Test]
        public void AArch64Dis_mov_v_slice()
        {
            Given_Instruction(0x4E081CE4);
            Expect_Code("mov\tv4.d[0],x7");
        }

        [Test]
        public void AArch64Dis_sha1su0()
        {
            Given_Instruction(0x5E09310B);
            Expect_Code("sha1su0\tv11.4s,v8.4s,v9.4s");
        }
        */
    }
}
