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
            Assert.AreEqual("add\tw19,w17,#&7FF", instr.ToString());
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
            Expect_Code("tbz\tw18,#&D,#&FFFFFFFFF9B18DC8");
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

        // An AArch64 decoder for the instruction D37DF273 (Bitfield) has not been implemented yet.
        [Test]
        public void AArch64Dis_D37DF273()
        {
            Given_Instruction(0xD37DF273);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 8B130280 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_8B130280()
        {
            Given_Instruction(0x8B130280);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction B4001341 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_B4001341()
        {
            Given_Instruction(0xB4001341);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D37DF29C (Bitfield) has not been implemented yet.
        [Test]
        public void AArch64Dis_D37DF29C()
        {
            Given_Instruction(0xD37DF29C);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D37FFA99 (Bitfield) has not been implemented yet.
        [Test]
        public void AArch64Dis_D37FFA99()
        {
            Given_Instruction(0xD37FFA99);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction FA400B84 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_FA400B84()
        {
            Given_Instruction(0xFA400B84);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 54000A61 (CondBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_54000A61()
        {
            Given_Instruction(0x54000A61);
            Expect_Code("@@@");
        }

        [Test]
        public void AArch64Dis_str_UnsignedImmediate()
        {
            Given_Instruction(0xF9000AE0);
            Expect_Code("str\tx0,[x23,#&10]");
        }

        // An AArch64 decoder for the instruction 2A1403E1 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_2A1403E1()
        {
            Given_Instruction(0x2A1403E1);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 8B150000 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_8B150000()
        {
            Given_Instruction(0x8B150000);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction B4000720 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_B4000720()
        {
            Given_Instruction(0xB4000720);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D37EF415 (Bitfield) has not been implemented yet.
        [Test]
        public void AArch64Dis_D37EF415()
        {
            Given_Instruction(0xD37EF415);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D65F03C0 (Unknown format character '*' decoding ret) has not been implemented yet.
        [Test]
        public void AArch64Dis_D65F03C0()
        {
            Given_Instruction(0xD65F03C0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction B8356B7F (LoadsAndStores) has not been implemented yet.
        [Test]
        public void AArch64Dis_B8356B7F()
        {
            Given_Instruction(0xB8356B7F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D503201F (System) has not been implemented yet.
        [Test]
        public void AArch64Dis_D503201F()
        {
            Given_Instruction(0xD503201F);
            Expect_Code("@@@");
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

        // An AArch64 decoder for the instruction EB13001F (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_EB13001F()
        {
            Given_Instruction(0xEB13001F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 1A9F17E0 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_1A9F17E0()
        {
            Given_Instruction(0x1A9F17E0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 39402260 (LoadsAndStores) has not been implemented yet.
        [Test]
        public void AArch64Dis_39402260()
        {
            Given_Instruction(0x39402260);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 35000140 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_35000140()
        {
            Given_Instruction(0x35000140);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction B4000080 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_B4000080()
        {
            Given_Instruction(0xB4000080);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 39002260 (LoadsAndStores) has not been implemented yet.
        [Test]
        public void AArch64Dis_39002260()
        {
            Given_Instruction(0x39002260);
            Expect_Code("@@@");
        }
        
        [Test]
        public void AArch64Dis_ldr_w32()
        {
            Given_Instruction(0xB9400001);
            Expect_Code("ldr\rw1,[x0]");
        }

        // An AArch64 decoder for the instruction 35FFFE73 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_35FFFE73()
        {
            Given_Instruction(0x35FFFE73);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 54000401 (CondBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_54000401()
        {
            Given_Instruction(0x54000401);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 2A0003F4 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_2A0003F4()
        {
            Given_Instruction(0x2A0003F4);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 34000154 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_34000154()
        {
            Given_Instruction(0x34000154);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction B4000194 (CompareBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_B4000194()
        {
            Given_Instruction(0xB4000194);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction F8737AA3 (LoadsAndStores) has not been implemented yet.
        [Test]
        public void AArch64Dis_F8737AA3()
        {
            Given_Instruction(0xF8737AA3);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 2A1603E0 (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_2A1603E0()
        {
            Given_Instruction(0x2A1603E0);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction D63F0060 (Unknown format character '*' decoding blr) has not been implemented yet.
        [Test]
        public void AArch64Dis_D63F0060()
        {
            Given_Instruction(0xD63F0060);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction EB13029F (DataProcessingReg) has not been implemented yet.
        [Test]
        public void AArch64Dis_EB13029F()
        {
            Given_Instruction(0xEB13029F);
            Expect_Code("@@@");
        }

        // An AArch64 decoder for the instruction 54FFFF21 (CondBranchImm) has not been implemented yet.
        [Test]
        public void AArch64Dis_54FFFF21()
        {
            Given_Instruction(0x54FFFF21);
            Expect_Code("@@@");
        }
        
    }
}