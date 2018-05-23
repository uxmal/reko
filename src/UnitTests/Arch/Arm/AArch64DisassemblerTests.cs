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
    public class Arm64DisassemblerTests : DisassemblerTestBase<Arm64Instruction>
    {
        private IProcessorArchitecture arch = new Arm64Architecture("aarch64");
        private Address baseAddress = Address.Ptr64(0x00100000);

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

        [Test]
        public void Arm64_b_label()
        {
            var instr = DisassembleBits("00010111 11111111 11111111 00000000");
            Assert.AreEqual("b\t#&FFC00", instr.ToString());
        }

        [Test]
        public void Arm64_bl_label()
        {
            var instr = DisassembleBits("10010111 11111111 11111111 00000000");
            Assert.AreEqual("bl\t#&FFC00", instr.ToString());
        }

        [Test]
        public void Arm64_br_Xn()
        {
            var instr = DisassembleBits("11010110 00011111 00000011 11000000");
            Assert.AreEqual("br\tx30", instr.ToString());
        }

        [Test]
        public void Arm64_add_Wn_imm()
        {
            var instr = DisassembleBits("000 10001 01 011111111111 10001 10011");
            Assert.AreEqual("add\tw19,w17,#&7FF", instr.ToString());
        }

        [Test]
        public void Arm64_adds_Xn_imm()
        {
            var instr = DisassembleBits("101 10001 00 011111111111 10001 10011");
            Assert.AreEqual("adds\tx19,x17,#&7FF", instr.ToString());
        }

        [Test]
        public void Arm64_subs_Wn_imm()
        {
            var instr = DisassembleBits("011 10001 00 011111111111 10001 10011");
            Assert.AreEqual("subs\tw19,w17,#&7FF", instr.ToString());
        }

        [Test]
        public void Arm64_sub_Xn_imm()
        {
            var instr = DisassembleBits("110 10001 00 011111111111 10001 10011");
            Assert.AreEqual("sub\tx19,x17,#&7FF", instr.ToString());
        }

        [Test]
        public void Arm64_and_Wn_imm()
        {
            var instr = DisassembleWord(0x120F3041);
            Assert.AreEqual("and\tw1,w2,#&3FFE0000", instr.ToString());
        }

        [Test]
        public void Arm64_and_Xn_imm()
        {
            var instr = DisassembleWord(0x920F3041);
            Assert.AreEqual("and\tx1,x2,#&3FFE0000", instr.ToString());
        }

        [Test]
        public void Arm64_ands_Xn_imm()
        {
            var instr = DisassembleBits("111 100100 0 010101 010101 00100 00111");
            Assert.AreEqual("ands\tx7,x4,#&FFFFF801", instr.ToString());
        }

        [Test]
        public void Arm64_movk_imm()
        {
            var instr = DisassembleBits("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
            Assert.AreEqual("movk\tx7,#&AAA4", instr.ToString());
        }
    }
}
