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
    [Category(Categories.Capstone)]
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
            Expect_Code("ldp\ts47,s59,[x1,-#&E0]");
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
        public void AArch64Dis_ubfm()
        {
            Given_Instruction(0xD37DF29C);
            Expect_Code("ubfm\tx28,x20,#0,#&3D");
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
            Expect_Code("ldr\tx3,[x21,x19,lsl,#3]");
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
            Expect_Code("stp\tx29,x30,[x31,-#&40]!");
        }

        [Test]
        public void AArch64Dis_stp_w64()
        {
            Given_Instruction(0xA90153F3);
            Expect_Code("stp\tx19,x20,[x31,#&8]");
        }

        [Test]
        public void AArch64Dis_ldp_w64()
        {
            Given_Instruction(0xA9446BB9);
            Expect_Code("ldp\tx25,x26,[x29,#&20]");
        }

        [Test]
        public void AArch64Dis_ldp_post()
        {
            Given_Instruction(0xA8C17BFD);
            Expect_Code("ldp\tx29,x30,[x31],#&8");
        }

        // An AArch64 decoder for the instruction 580000A0 (LoadRegLit) has not been implemented yet.
        [Test]
        public void AArch64Dis_580000A0()
        {
            Given_Instruction(0x580000A0);
            Expect_Code("@@@");
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

        // An AArch64 decoder for the instruction 8A140000 (Unknown format character '*' in '*shifted register, 64-bit' decoding and) has not been implemented yet.
        [Test]
        public void AArch64Dis_and_reg()
        {
            Given_Instruction(0x8A140000);
            Expect_Code("and\tx0,x0,x20");
        }

        // An AArch64 decoder for the instruction 3D8023A0 (LdStRegUImm size = 0, V = 1) has not been implemented yet.
        [Test]
        public void AArch64Dis_3D8023A0()
        {
            Given_Instruction(0x3D8023A0);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_sbfm_2()
        {
            Given_Instruction(0x937D7C63);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8418681 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8418681()
        {
            Given_Instruction(0xF8418681);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 9343FE94 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_9343FE94()
        {
            Given_Instruction(0x9343FE94);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 385F8000 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_385F8000()
        {
            Given_Instruction(0x385F8000);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 9B017C14 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_9B017C14()
        {
            Given_Instruction(0x9B017C14);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction EA01001F (Unknown format character '*' in '*shifted register, 64-bit' decoding ands) has not been implemented yet.
        [Test]
        public void AArch64Dis_EA01001F()
        {
            Given_Instruction(0xEA01001F);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_ldurb()
        {
            Given_Instruction(0x385F9019);
            Expect_Code("ldurb\tw25,[x0,-#&7]");
        }

        // An AArch64 decoder for the instruction 38018C14 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38018C14()
        {
            Given_Instruction(0x38018C14);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B157E73 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B157E73()
        {
            Given_Instruction(0x1B157E73);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3861C840 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3861C840()
        {
            Given_Instruction(0x3861C840);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38336A9F (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38336A9F()
        {
            Given_Instruction(0x38336A9F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C41 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C41()
        {
            Given_Instruction(0x93407C41);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407F60 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407F60()
        {
            Given_Instruction(0x93407F60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B037E94 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B037E94()
        {
            Given_Instruction(0x1B037E94);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 79400021 (LdStRegUImm size = 1) has not been implemented yet.
        [Test]
        public void AArch64Dis_79400021()
        {
            Given_Instruction(0x79400021);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C62 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C62()
        {
            Given_Instruction(0x93407C62);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3861CA60 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3861CA60()
        {
            Given_Instruction(0x3861CA60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F800847F (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F800847F()
        {
            Given_Instruction(0xF800847F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C21 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C21()
        {
            Given_Instruction(0x93407C21);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C35 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C35()
        {
            Given_Instruction(0x93407C35);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_add_extension()
        {
            Given_Instruction(0x8B34C2D9);
            Expect_Code("add\tx25,x22,w20,sxtw #0");
        }

        // An AArch64 decoder for the instruction 93407EF6 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EF6()
        {
            Given_Instruction(0x93407EF6);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3860CABC (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3860CABC()
        {
            Given_Instruction(0x3860CABC);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3860CA61 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3860CA61()
        {
            Given_Instruction(0x3860CA61);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3863C800 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3863C800()
        {
            Given_Instruction(0x3863C800);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 937D7E94 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_937D7E94()
        {
            Given_Instruction(0x937D7E94);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 937D7C41 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_937D7C41()
        {
            Given_Instruction(0x937D7C41);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F84086B3 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F84086B3()
        {
            Given_Instruction(0xF84086B3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B007C21 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B007C21()
        {
            Given_Instruction(0x1B007C21);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 937C7C21 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_937C7C21()
        {
            Given_Instruction(0x937C7C21);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6B33027F (AddSubExtendedRegister) has not been implemented yet.
        [Test]
        public void AArch64Dis_6B33027F()
        {
            Given_Instruction(0x6B33027F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3862C875 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3862C875()
        {
            Given_Instruction(0x3862C875);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407E60 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407E60()
        {
            Given_Instruction(0x93407E60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C22 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C22()
        {
            Given_Instruction(0x93407C22);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3862C860 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3862C860()
        {
            Given_Instruction(0x3862C860);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3800141F (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3800141F()
        {
            Given_Instruction(0x3800141F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407EC2 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EC2()
        {
            Given_Instruction(0x93407EC2);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B038441 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B038441()
        {
            Given_Instruction(0x1B038441);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407E77 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407E77()
        {
            Given_Instruction(0x93407E77);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38401700 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38401700()
        {
            Given_Instruction(0x38401700);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F85F8260 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F85F8260()
        {
            Given_Instruction(0xF85F8260);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B037C03 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B037C03()
        {
            Given_Instruction(0x1B037C03);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1B017C01 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_1B017C01()
        {
            Given_Instruction(0x1B017C01);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction A000020 (Unknown format character '*' in '*shifted register, 32-bit' decoding and) has not been implemented yet.
        [Test]
        public void AArch64Dis_and_reg_reg()
        {
            Given_Instruction(0x0A000020);
            Expect_Code("and\tw0,w1,w0");
        }

        // An AArch64 decoder for the instruction 6B34029F (AddSubExtendedRegister) has not been implemented yet.
        [Test]
        public void AArch64Dis_6B34029F()
        {
            Given_Instruction(0x6B34029F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407E62 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407E62()
        {
            Given_Instruction(0x93407E62);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6B22005F (AddSubExtendedRegister) has not been implemented yet.
        [Test]
        public void AArch64Dis_6B22005F()
        {
            Given_Instruction(0x6B22005F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3823C855 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3823C855()
        {
            Given_Instruction(0x3823C855);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3862C820 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3862C820()
        {
            Given_Instruction(0x3862C820);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3860C860 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3860C860()
        {
            Given_Instruction(0x3860C860);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F800845F (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F800845F()
        {
            Given_Instruction(0xF800845F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3860C840 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3860C840()
        {
            Given_Instruction(0x3860C840);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 53001C84 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001C84()
        {
            Given_Instruction(0x53001C84);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3824C83F (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3824C83F()
        {
            Given_Instruction(0x3824C83F);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_strb_indexed()
        {
            Given_Instruction(0x3820C83F);
            Expect_Code("strb\tw31,[x1,w16,uxtw]");
        }

        // An AArch64 decoder for the instruction 3D8027A0 (LdStRegUImm size = 0, V = 1) has not been implemented yet.
        [Test]
        public void AArch64Dis_3D8027A0()
        {
            Given_Instruction(0x3D8027A0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6B20001F (AddSubExtendedRegister) has not been implemented yet.
        [Test]
        public void AArch64Dis_6B20001F()
        {
            Given_Instruction(0x6B20001F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407EB5 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EB5()
        {
            Given_Instruction(0x93407EB5);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3862C801 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3862C801()
        {
            Given_Instruction(0x3862C801);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38616857 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38616857()
        {
            Given_Instruction(0x38616857);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38346A76 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38346A76()
        {
            Given_Instruction(0x38346A76);
            Expect_Code("@@@");
        }


        // An AArch64 decoder for the instruction 53001C53 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001C53()
        {
            Given_Instruction(0x53001C53);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_sbfm_0_1F()
        {
            Given_Instruction(0x93407C18);
            Expect_Code("sbfm\tx24,x0,#0,#&1F@@");
        }

        // An AArch64 decoder for the instruction 38216A63 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_strb_index()
        {
            Given_Instruction(0x38216A63);
            Expect_Code("strb\tw3,[x2,@@@");
        }

        // An AArch64 decoder for the instruction F8410661 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8410661()
        {
            Given_Instruction(0xF8410661);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 937D7F01 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_937D7F01()
        {
            Given_Instruction(0x937D7F01);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8408736 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8408736()
        {
            Given_Instruction(0xF8408736);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_add_ext_reg_sxtw()
        {
            Given_Instruction(0x8B33D2D3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8410E81 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8410E81()
        {
            Given_Instruction(0xF8410E81);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407F55 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407F55()
        {
            Given_Instruction(0x93407F55);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38401E82 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38401E82()
        {
            Given_Instruction(0x38401E82);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407E73 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407E73()
        {
            Given_Instruction(0x93407E73);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38626A60 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38626A60()
        {
            Given_Instruction(0x38626A60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38401443 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38401443()
        {
            Given_Instruction(0x38401443);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38401420 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38401420()
        {
            Given_Instruction(0x38401420);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8410EE1 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8410EE1()
        {
            Given_Instruction(0xF8410EE1);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8418E81 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8418E81()
        {
            Given_Instruction(0xF8418E81);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8408693 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8408693()
        {
            Given_Instruction(0xF8408693);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38626823 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38626823()
        {
            Given_Instruction(0x38626823);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C60 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C60()
        {
            Given_Instruction(0x93407C60);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3834CB23 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3834CB23()
        {
            Given_Instruction(0x3834CB23);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 937D7C61 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_937D7C61()
        {
            Given_Instruction(0x937D7C61);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 53001C20 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001C20()
        {
            Given_Instruction(0x53001C20);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8408660 (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8408660()
        {
            Given_Instruction(0xF8408660);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_sdiv()
        {
            Given_Instruction(0x1AC00F03);
            Expect_Code("sdiv\tw3,w24,w0");
        }

        // An AArch64 decoder for the instruction 93407E63 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407E63()
        {
            Given_Instruction(0x93407E63);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3863C884 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3863C884()
        {
            Given_Instruction(0x3863C884);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3837681F (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3837681F()
        {
            Given_Instruction(0x3837681F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F80086DF (op1 = 3, op3 = 0x, op4=0xxxx) has not been implemented yet.
        [Test]
        public void AArch64Dis_F80086DF()
        {
            Given_Instruction(0xF80086DF);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38401C41 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38401C41()
        {
            Given_Instruction(0x38401C41);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3873C857 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3873C857()
        {
            Given_Instruction(0x3873C857);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3861C873 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3861C873()
        {
            Given_Instruction(0x3861C873);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3873C817 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3873C817()
        {
            Given_Instruction(0x3873C817);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3875CB33 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3875CB33()
        {
            Given_Instruction(0x3875CB33);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3862C873 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3862C873()
        {
            Given_Instruction(0x3862C873);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3876C873 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3876C873()
        {
            Given_Instruction(0x3876C873);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407EE2 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EE2()
        {
            Given_Instruction(0x93407EE2);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38776A98 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38776A98()
        {
            Given_Instruction(0x38776A98);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 787B7B20 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_787B7B20()
        {
            Given_Instruction(0x787B7B20);
            Expect_Code("@@@");
        }


        // An AArch64 decoder for the instruction 53001C21 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001C21()
        {
            Given_Instruction(0x53001C21);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38616A62 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38616A62()
        {
            Given_Instruction(0x38616A62);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 7876D800 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_7876D800()
        {
            Given_Instruction(0x7876D800);
            Expect_Code("@@@");
        }

        public void AArch64Dis_3873C800()
        {
            Given_Instruction(0x3873C800);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3873C816 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3873C816()
        {
            Given_Instruction(0x3873C816);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 6B21003F (AddSubExtendedRegister) has not been implemented yet.
        [Test]
        public void AArch64Dis_6B21003F()
        {
            Given_Instruction(0x6B21003F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3876CB00 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3876CB00()
        {
            Given_Instruction(0x3876CB00);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_ldrh_indexed()
        {
            Given_Instruction(0x7874D800);
            Expect_Code("ldrh\tw0,[x0,w20,sxtw #1]");
        }

        // An AArch64 decoder for the instruction 93407EE5 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EE5()
        {
            Given_Instruction(0x93407EE5);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_ldr_index_sxtw()
        {
            Given_Instruction(0x7875D800);
            Expect_Code("ldr\tw0,[x0,w21,sxtw,#3]");
        }

        // An AArch64 decoder for the instruction 93407C24 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C24()
        {
            Given_Instruction(0x93407C24);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C23 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C23()
        {
            Given_Instruction(0x93407C23);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407C55 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407C55()
        {
            Given_Instruction(0x93407C55);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 53057C40 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53057C40()
        {
            Given_Instruction(0x53057C40);
            Expect_Code("@@@");
        }

  

        // An AArch64 decoder for the instruction 530C7C04 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_530C7C04()
        {
            Given_Instruction(0x530C7C04);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 53001CA3 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001CA3()
        {
            Given_Instruction(0x53001CA3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 53001CC8 (Unknown format character '3' in '32-bit variant' decoding ubfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_53001CC8()
        {
            Given_Instruction(0x53001CC8);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 9B047D44 (DataProcessing3Source) has not been implemented yet.
        [Test]
        public void AArch64Dis_9B047D44()
        {
            Given_Instruction(0x9B047D44);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 9280000A (Unknown format character '*' in '* - 64 bit variant' decoding movn) has not been implemented yet.
        [Test]
        public void AArch64Dis_movn_imm()
        {
            Given_Instruction(0x9280000A);
            Expect_Code("movn\tx10,#0");
        }

        // An AArch64 decoder for the instruction 38606821 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38606821()
        {
            Given_Instruction(0x38606821);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 3835CA9F (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_3835CA9F()
        {
            Given_Instruction(0x3835CA9F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 93407EA0 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EA0()
        {
            Given_Instruction(0x93407EA0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38336A84 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38336A84()
        {
            Given_Instruction(0x38336A84);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            Expect_Code("sturb\tw31,[x4,-#&1]");
        }

        // An AArch64 decoder for the instruction 93407EB6 (Unknown format character '6' in '64-bit variant' decoding sbfm) has not been implemented yet.
        [Test]
        public void AArch64Dis_93407EB6()
        {
            Given_Instruction(0x93407EB6);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 38664849 (LdSt op0 = 0, op1 = 3, op3 = 0) has not been implemented yet.
        [Test]
        public void AArch64Dis_38664849()
        {
            Given_Instruction(0x38664849);
            Expect_Code("@@@");
        }
   }
}